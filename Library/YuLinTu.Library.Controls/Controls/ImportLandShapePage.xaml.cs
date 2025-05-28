/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using System.Reflection;
using YuLinTu.Spatial;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// ImportLandShapePage.xaml 的交互逻辑-导入图斑弹出框绑定设置
    /// </summary>
    public partial class ImportLandShapePage : InfoPageBase
    {
        #region Fields

        private ImportAccountLandShapeSettingDefine config;
        private ImportAccountLandShapeSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Property

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        public eImportTypes ImportType { get; private set; }

        /// <summary>
        /// 按照地块编码绑定导入
        /// </summary>
        public bool UseLandCodeBindImport { get; private set; }

        /// <summary>
        /// 按照承包方信息绑定导入
        /// </summary>
        public bool UseContractorInfoImport { get; private set; }

        /// <summary>
        /// 按照承包方户号绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorNumberImport { get; private set; }

        /// <summary>
        /// 按照原地块编码绑定导入
        /// </summary>
        public bool UseOldLandCodeBindImport { get; private set; }

        public bool DelLandImport { get; private set; }

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public ImportAccountLandShapeSettingDefine ImportLandShapeInfoDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.ProGrid.Object = ImportLandShapeInfoDefine;
            }
        }

        /// <summary>
        /// 常用配置页
        /// </summary>
        public YuLinTu.Windows.Wpf.Metro.Components.PropertyGrid ProGrid { get { return this.propertyGrid; } }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 读取的shp所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> shapeAllcolNameList { get; set; }

        //当前选择导入的类型
        public ImportShpByType importShpByType = new ImportShpByType();

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Db { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportLandShapePage(IWorkpage page, string header, string desc = "")
        {
            InitializeComponent();
            this.Header = header;
            this.Workpage = page;
            DataContext = this;
            chkUseLandCodeBindImport.IsChecked = true;
            importShpByType.UseLandCodeBindImport = true;
            UseLandCodeBindImport = true;
            UseContractorInfoImport = false;
            UseContractorNumberImport = false;
            UseOldLandCodeBindImport = false;
            btnExcuteImport.IsEnabled = false;
        }

        #endregion

        #region Method-Override

        /// <summary>
        /// 装载配置
        /// </summary>
        /// <param name="columnList">读取的shp字段列表进行装载用</param>
        private void OnLoadConfig(List<string> shpColumnList)
        {
            CommonConfigSelector.DisplayValueList = shpColumnList;
            shapeAllcolNameList = CommonConfigSelector.GetUserConfigColumnInfo(shpColumnList.Count); //获取定义的数据源 
            ProGrid.EnableObjectMetadataCache = false;
            ProGrid.Properties["index"] = shapeAllcolNameList;
            ProGrid.Alert += ProGrid_Alert;
            ProGrid.Properties["importShpByType"] = importShpByType;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportAccountLandShapeSettingDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<ImportAccountLandShapeSettingDefine>();
            config = (section.Settings as ImportAccountLandShapeSettingDefine);   //得到经反序列化后的对象
            var configCopy = config.Clone();

            var keylist = configCopy.DictProperties();
            foreach (var item in keylist)
            {
                if (!shpColumnList.Contains(item.Value.ToString()))
                {
                    configCopy.SetPropertyValue(item.Key, "None");
                }
            }

            ImportLandShapeInfoDefine = configCopy as ImportAccountLandShapeSettingDefine;
        }

        void ProGrid_Alert(object sender, PropertyGridAlertEventArgs e)
        {
            var hasError = ProGrid.PropertyDescriptors.Any(c => c.Grade >= eMessageGrade.Error);
            btnExcuteImport.IsEnabled = !hasError;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 选择文件
        /// </summary>
        private void btnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "文件类型(*.shp)|*.shp";
            var val = ofd.ShowDialog();
            if (val != null && val.Value)
            {
                txt_file.Text = ofd.FileName;
            }
            if (txt_file.Text == "") return;
            string filepath = System.IO.Path.GetDirectoryName(txt_file.Text);
            string filename = System.IO.Path.GetFileNameWithoutExtension(txt_file.Text);
            List<string> shpColumnNameList = new List<string>();

            SpatialReference shpReference = ReferenceHelper.GetShapeReference(txt_file.Text);
            string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
            SpatialReference dbReference = ReferenceHelper.GetDbReference<Zone>(Db, name, "Shape");

            var shpProjectionInfo = (shpReference == null ? shpReference = new SpatialReference(0) : shpReference).CreateProjectionInfo();
            var dbProjectionInfo = dbReference.CreateProjectionInfo();
            //坐标提示信息
            string message = string.Empty;

            if (shpReference == null || shpProjectionInfo == null || (shpReference.WKID == 0 &&
                shpProjectionInfo.Name != null && shpProjectionInfo.Name.ToLower() == "unknown"))
            {
                message = "当前Shape文件坐标为: Unknown";
            }
            else
            {
                var shppi = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(shpReference);
                if (shpReference.IsPROJCS())
                {
                    message = "当前Shape文件坐标为: " + shppi.Name + "(" + shpReference.WKID + ")";
                }
                if (shpReference.IsGEOGCS() || !shpReference.IsValid())
                {
                    message = "当前Shape文件坐标为: " + shppi.GeographicInfo.Name + "(" + shpReference.WKID + ")";
                }
            }
            if (dbReference == null || dbReference.WKID == 0)
            {
                message += ",当前数据库坐标为: Unknown";
            }
            else
            {
                if (dbReference.IsPROJCS())
                {
                    message += ",当前数据库坐标为: " + dbProjectionInfo.Name + "(" + dbReference.WKID + ")";
                }
                if (dbReference.IsGEOGCS() || !dbReference.IsValid())
                {
                    message += ",当前数据库坐标为: " + dbProjectionInfo.GeographicInfo.Name + "(" + dbReference.WKID + ")";
                }
            }

            if (shpReference != null &&
                (shpReference.WKID != dbReference.WKID && (shpProjectionInfo == null || shpProjectionInfo.Name != dbProjectionInfo.Name)))
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "提示",
                    Message = "当前Shape文件坐标信息与数据库不一致," + message,
                    MessageGrade = eMessageGrade.Warn,
                    CancelButtonText = "取消",
                });
            }
            else if (shpReference == null && dbReference == null)
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "提示",
                    Message = message,
                    MessageGrade = eMessageGrade.Warn,
                    CancelButtonText = "取消",
                });
            }

            try
            {
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var s = dq.GetElementProperties(null, filename);
                if (s == null || s.Count == 0) return;
                s.ForEach(t => shpColumnNameList.Add(t.ColumnName));
                if (shpColumnNameList.Contains("___FID___"))
                    shpColumnNameList.Remove("___FID___");

                btnExcuteImport.IsEnabled = true;
                CommonConfigSelector.DisplayValueList = null;
                OnLoadConfig(shpColumnNameList);
            }
            catch (Exception ex)
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "提示",
                    Message = "当前打开Shape文件有误，请检查文件是否正确" + ex.Message,
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                });
                btnExcuteImport.IsEnabled = false;
            }
        }

        /// <summary>
        /// Changed事件
        /// </summary>
        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                FileName = string.Empty;
                btnExcuteImport.IsEnabled = false;
            }
            else
            {
                FileName = name;
                btnExcuteImport.IsEnabled = true;
            }
        }

        /// <summary>
        /// 执行导入
        /// </summary>
        private void btnExcuteImport_Click_1(object sender, RoutedEventArgs e)
        {
            DelLandImport = (bool)landdelCheck.IsChecked;
            //保存当前配置
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(ImportLandShapeInfoDefine);
                systemCenter.Save<ImportAccountLandShapeSettingDefine>();
            }));

            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 按照地块编码绑定导入选择
        /// </summary>
        private void chkUseLandCodeBindImport_Check(object sender, RoutedEventArgs e)
        {
            if (chkUseLandCodeBindImport.IsChecked.Value)
            {
                UseLandCodeBindImport = true;
                UseContractorInfoImport = false;
                UseContractorNumberImport = false;
                UseOldLandCodeBindImport = false;
                importShpByType.UseLandCodeBindImport = true;
                importShpByType.UseContractorInfoImport = false;
                importShpByType.UseContractorNumberImport = false;
                importShpByType.UseOldLandCodeBindImport = false;
                if (ProGrid.Object != null)
                {
                    ImportLandShapeInfoDefine.NameIndex = ImportLandShapeInfoDefine.NameIndex;
                }
            }
        }

        /// <summary>
        /// 按照原地块编码绑定导入选择
        /// </summary>
        private void chkUseOldLandCodeBindImport_Check(object sender, RoutedEventArgs e)
        {
            if (chkUseOldLandCodeBindImport.IsChecked.Value)
            {
                UseLandCodeBindImport = false;
                UseContractorInfoImport = false;
                UseContractorNumberImport = false;
                UseOldLandCodeBindImport = true;
                importShpByType.UseLandCodeBindImport = false;
                importShpByType.UseContractorInfoImport = false;
                importShpByType.UseContractorNumberImport = false;
                importShpByType.UseOldLandCodeBindImport = true;
                if (ProGrid.Object != null)
                {
                    ImportLandShapeInfoDefine.NameIndex = ImportLandShapeInfoDefine.NameIndex;
                }
            }
        }
        /// <summary>
        /// 按照承包方信息绑定导入选择
        /// </summary>
        private void chkUseContractorInfoImport_Checked(object sender, RoutedEventArgs e)
        {
            if (chkUseContractorInfoImport.IsChecked.Value)
            {

                UseLandCodeBindImport = false;
                UseContractorInfoImport = true;
                UseContractorNumberImport = false;
                UseOldLandCodeBindImport = false;
                importShpByType.UseLandCodeBindImport = false;
                importShpByType.UseContractorInfoImport = true;
                importShpByType.UseContractorNumberImport = false;
                importShpByType.UseOldLandCodeBindImport = false;
                if (ProGrid.Object != null)
                {
                    ImportLandShapeInfoDefine.NameIndex = ImportLandShapeInfoDefine.NameIndex;
                }
            }
        }

        /// <summary>
        /// 按照承包方户号导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseContractorNumberImport_Click(object sender, RoutedEventArgs e)
        {
            if (chkUseContractorNumberImport.IsChecked.Value)
            {
                UseLandCodeBindImport = false;
                UseContractorInfoImport = false;
                UseContractorNumberImport = true;
                UseOldLandCodeBindImport = false;
                importShpByType.UseLandCodeBindImport = false;
                importShpByType.UseContractorInfoImport = false;
                importShpByType.UseContractorNumberImport = true;
                importShpByType.UseOldLandCodeBindImport = false;
                if (ProGrid.Object != null)
                {
                    ImportLandShapeInfoDefine.NameIndex = ImportLandShapeInfoDefine.NameIndex;
                }
            }
        }

        #endregion

        /// <summary>
        /// 全选
        /// </summary>
        private void allCheck_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo[] infos = typeof(ImportAccountLandShapeSettingDefine).GetProperties();
            List<KeyValue<int, string>> indexKv = ProGrid.Properties["index"] as List<KeyValue<int, string>>;
            if (indexKv == null) return;
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                var display = info.GetAttribute<DisplayLanguageAttribute>();
                var description = info.GetAttribute<DescriptionLanguageAttribute>();
                if (!allCheck.IsChecked.Value)
                {
                    info.SetValue(ImportLandShapeInfoDefine, "None", null);
                    continue;
                }
                if (display == null)
                    continue;
                var fvalue = indexKv.Any(t => t.Value == display.Name);
                if (fvalue)
                {
                    info.SetValue(ImportLandShapeInfoDefine, display.Name, null);
                }
                else
                {
                    fvalue = indexKv.Any(t => t.Value == description.Description);
                    if (fvalue)
                    {
                        info.SetValue(ImportLandShapeInfoDefine, description.Description, null);
                    }
                }
                if (display.Name == "二轮合同面积")
                {
                    fvalue = indexKv.Any(t => t.Value == "二轮面积");
                    if (fvalue)
                    {
                        info.SetValue(ImportLandShapeInfoDefine, "二轮面积", null);
                    }
                }
                if (display.Name == "确权地块编码")
                {
                    fvalue = indexKv.Any(t => t.Value == "原地块编码");
                    if (fvalue)
                    {
                        info.SetValue(ImportLandShapeInfoDefine, "原地块编码", null);
                    }
                }                
            }
        }
    }

    #region Class

    //选择的导入方式
    public class ImportShpByType
    {
        #region Properties

        /// <summary>
        /// 按照地块编码绑定导入
        /// </summary>
        public bool UseLandCodeBindImport { get; set; }

        /// <summary>
        /// 按照原地块编码绑定导入
        /// </summary>
        public bool UseOldLandCodeBindImport { get; set; }

        /// <summary>
        /// 按照承包方信息绑定导入
        /// </summary>
        public bool UseContractorInfoImport { get; set; }

        /// <summary>
        /// 按照承包方户号绑定导入
        /// </summary>
        public bool UseContractorNumberImport { get; set; }

        #endregion
    }

    #endregion

}
