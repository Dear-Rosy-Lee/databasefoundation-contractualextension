/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导入界址线图斑界面
    /// </summary>
    public partial class ImportBoundaryAddressCoilPage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportBoundaryAddressCoilPage(IWorkpage page, string header = "", string desc = "")
        {
            InitializeComponent();
            DataContext = this;
            this.Header = header;
            this.Workpage = page;
            chkUseLandCodeBindImport.IsChecked = true;
            importCoilByType.UseLandCodeBindImport = true;
            UseLandCodeBindImport = true;
            btnExcuteImport.IsEnabled = false;
        }

        #endregion

        #region Fields

        private ImportBoundaryAddressCoilDefine config;
        private ImportBoundaryAddressCoilDefine importCoilDefine;
        private SettingsProfileCenter systemCenter;
        //当前选择导入的类型
        public ImportDotByType importCoilByType = new ImportDotByType();

        #endregion

        #region Properties

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        //public eImportTypes ImportType { get; private set; }

        /// <summary>
        /// 按照地块编码绑定导入
        /// </summary>
        public bool UseLandCodeBindImport { get; private set; }

        /// <summary>
        /// 界址线图斑实体配置
        /// </summary>
        public ImportBoundaryAddressCoilDefine ImportCoilDefine
        {
            get { return importCoilDefine; }
            set
            {
                importCoilDefine = value;
                this.ProGrid.Object = ImportCoilDefine;
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
        /// 读取的界址点所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> CoilAllcolNameList { get; set; }

        #endregion

        #region Method

        public void SetImportCoilDefine()
        {

        }

        /// <summary>
        /// 装载配置
        /// </summary>
        /// <param name="columnList">读取的shp字段列表进行装载用</param>
        private void OnLoadConfig(List<string> coilColumnList)
        {
            CommonConfigSelector.DisplayValueList = coilColumnList;
            CoilAllcolNameList = CommonConfigSelector.GetUserConfigColumnInfo(coilColumnList.Count); //获取定义的数据源
            ProGrid.EnableObjectMetadataCache = false;
            ProGrid.Properties["index"] = CoilAllcolNameList;
            ProGrid.Alert += ProGrid_Alert;
            ProGrid.Properties["importCoilByType"] = importCoilByType;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportBoundaryAddressCoilDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<ImportBoundaryAddressCoilDefine>();
            config = (section.Settings as ImportBoundaryAddressCoilDefine);   //得到经反序列化后的对象
            var configCopy = config.Clone();
            var keylist = configCopy.DictProperties();
            foreach (var item in keylist)
            {
                if (!coilColumnList.Contains(item.Value.ToString()))
                {
                    configCopy.SetPropertyValue(item.Key, "None");
                }
            }
            ImportCoilDefine = configCopy as ImportBoundaryAddressCoilDefine;
        }

        void ProGrid_Alert(object sender, PropertyGridAlertEventArgs e)
        {
            var hasError = ProGrid.PropertyDescriptors.Any(c => c.Grade >= eMessageGrade.Error);
            btnExcuteImport.IsEnabled = !hasError;
        }

        #endregion

        #region Events

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
            var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
            var dq = new DynamicQuery(ds);

            var s = dq.GetElementProperties(null, filename);
            var ss = s.FindAll(c => c.ColumnName != "Shape");

            if (ss == null || ss.Count == 0) return;
            List<string> coilColumnNameList = new List<string>();
            ss.ForEach(t => coilColumnNameList.Add(t.ColumnName));
            CommonConfigSelector.DisplayValueList = null;
            OnLoadConfig(coilColumnNameList);
        }

        /// <summary>
        /// Changed事件
        /// </summary>
        private void txt_file_TextChanged(object sender, TextChangedEventArgs e)
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
        private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        {
            //保存当前配置
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(ImportCoilDefine);
                systemCenter.Save<ImportBoundaryAddressCoilDefine>();
            }));
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 按照地块编码绑定导入选择
        /// </summary>
        private void chkUseLandCodeBindImport_Click(object sender, RoutedEventArgs e)
        {
            if (chkUseLandCodeBindImport.IsChecked.Value)
            {
                UseLandCodeBindImport = true;
                importCoilByType.UseLandCodeBindImport = true;
                if (ProGrid.Object != null)
                {
                    ImportCoilDefine.LandNumber = ImportCoilDefine.LandNumber;
                }
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void allCheck_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo[] infos = typeof(ImportBoundaryAddressCoilDefine).GetProperties();
            List<KeyValue<int, string>> indexKv = ProGrid.Properties["index"] as List<KeyValue<int, string>>;
            if (indexKv == null) return;
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                var display = info.GetAttribute<DisplayLanguageAttribute>();
                if (!allCheck.IsChecked.Value)
                {
                    info.SetValue(ImportCoilDefine, "None", null);
                    continue;
                }
                if (display == null)
                    continue;
                var fvalue = indexKv.Any(t => t.Value == display.Name);
                if (fvalue)
                {
                    info.SetValue(ImportCoilDefine, display.Name, null);
                }
            }
        }

        #endregion
    }
}
