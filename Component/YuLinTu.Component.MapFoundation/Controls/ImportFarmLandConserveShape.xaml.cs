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
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 导入基本农田保护区图斑界面
    /// </summary>
    public partial class ImportFarmLandConserveShape : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportFarmLandConserveShape(IWorkpage page, string desc = "")
        {
            InitializeComponent();
            DataContext = this;
            this.Workpage = page;
            btnExcuteImport.IsEnabled = false;
        }

        #endregion

        #region Fields

        private ImportFarmLandConserveDefine config;
        private ImportFarmLandConserveDefine importConserveDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 界址点图斑实体配置
        /// </summary>
        public ImportFarmLandConserveDefine ImportConserveDefine
        {
            get { return importConserveDefine; }
            set
            {
                importConserveDefine = value;
                this.ProGrid.Object = ImportConserveDefine;
            }
        }

        /// <summary>
        /// 常用配置页
        /// </summary>
        public YuLinTu.Windows.Wpf.Metro.Components.PropertyGrid ProGrid
        {
            get { return this.propertyGrid; }
            set { propertyGrid = value; }
        }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 读取的基本农田保护区图斑所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> FlConserveNameList { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 装载配置
        /// </summary>
        /// <param name="columnList">读取的shp字段列表进行装载用</param>
        private void OnLoadConfig(List<string> flColumnNameList)
        {
            CommonConfigSelector.DisplayValueList = flColumnNameList;
            FlConserveNameList = CommonConfigSelector.GetUserConfigColumnInfo(flColumnNameList.Count); //获取定义的数据源
            ProGrid.EnableObjectMetadataCache = false;
            ProGrid.Properties["index"] = FlConserveNameList;
            ProGrid.Alert += ProGrid_Alert;
            //ProGrid.Properties["importFarmLandConserveShape"] = FileName;
            ProGrid.Properties["importComByType"] = FileName;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportFarmLandConserveDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<ImportFarmLandConserveDefine>();
            config = (section.Settings as ImportFarmLandConserveDefine);   //得到经反序列化后的对象
            var configCopy = config.Clone();
            var keylist = configCopy.DictProperties();
            foreach (var item in keylist)
            {
                if (!flColumnNameList.Contains(item.Value.ToString()))
                {
                    configCopy.SetPropertyValue(item.Key, "None");
                }
            }
            ImportConserveDefine = configCopy as ImportFarmLandConserveDefine;
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
            try
            {
                string filepath = System.IO.Path.GetDirectoryName(txt_file.Text);
                string filename = System.IO.Path.GetFileNameWithoutExtension(txt_file.Text);
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);

                var s = dq.GetElementProperties(null, filename);
                var ss = s.FindAll(c => c.ColumnName != "Shape");

                if (ss == null || ss.Count == 0) return;
                List<string> flColumnNameList = new List<string>();
                ss.ForEach(t => flColumnNameList.Add(t.ColumnName));
                CommonConfigSelector.DisplayValueList = null;
                OnLoadConfig(flColumnNameList);
            }
            catch
            {
                TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                if (Workpage == null) return;
                messagebox.Message = "当前Shape文件相关文件信息错误，请检查，如文件名称是否一致或缺失";
                messagebox.Header = "提示";
                Workpage.Page.ShowMessageBox(messagebox);
                Workpage.Page.CloseMessageBox(this);
                return;
            }
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
                config.CopyPropertiesFrom(ImportConserveDefine);
                systemCenter.Save<ImportFarmLandConserveDefine>();
            }));
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void allCheck_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo[] infos = typeof(ImportFarmLandConserveDefine).GetProperties();
            List<KeyValue<int, string>> indexKv = ProGrid.Properties["index"] as List<KeyValue<int, string>>;
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                var display = info.GetAttribute<DisplayLanguageAttribute>();
                if (!allCheck.IsChecked.Value)
                {
                    info.SetValue(ImportConserveDefine, "None", null);
                    continue;
                }
                if (display == null)
                    continue;
                var fvalue = indexKv.Any(t => t.Value == display.Name);
                if (fvalue)
                {
                    info.SetValue(ImportConserveDefine, display.Name, null);
                }
            }
        }

        #endregion
    }
}
