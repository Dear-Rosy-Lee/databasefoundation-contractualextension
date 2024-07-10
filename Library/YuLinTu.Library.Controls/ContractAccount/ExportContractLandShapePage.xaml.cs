/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导出地域下地块图斑Shape
    /// </summary>
    public partial class ExportContractLandShapePage : InfoPageBase
    {
        #region Fields

        private string description = "请选择保存文件的目录";

        #endregion


        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportContractLandShapePage(IWorkpage page, string header = "", string desc = "")
        {
            InitializeComponent();
            DataContext = this;
            this.Header = header;
            this.Workpage = page;
            btnExcuteImport.IsEnabled = false;
        }

        #endregion

        #region Fields

        private ExportContractLandShapeDefine config;
        private ExportContractLandShapeDefine importDotDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 导出文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 导出Shp文件设置
        /// </summary>
        public ExportContractLandShapeDefine ImportDotDefine
        {
            get { return importDotDefine; }
            set
            {
                importDotDefine = value;
                this.ProGrid.Object = ImportDotDefine;
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

        #endregion

        #region Method

        /// <summary>
        /// 装载配置
        /// </summary>      
        private void OnLoadConfig()
        {
            var propertyCount = typeof(ExportContractLandShapeDefine).GetProperties().Count();
            ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源 
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ExportContractLandShapeDefine>();
            var section = profile.GetSection<ExportContractLandShapeDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            config = (section.Settings as ExportContractLandShapeDefine);   //得到经反序列化后的对象

            ImportDotDefine = config.Clone() as ExportContractLandShapeDefine;
            importDotDefine.FamilyIndex = config.FamilyIndex;

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
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = description; ;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txt_file.Text = fbd.SelectedPath;
            }
            OnLoadConfig();
        }

        /// <summary>
        /// Changed事件
        /// </summary>
        private void txt_file_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name))
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
                config.CopyPropertiesFrom(ImportDotDefine);
                systemCenter.Save<ExportContractLandShapeDefine>();
            }));
            Workpage.Page.CloseMessageBox(true);
        }

        #endregion
    }


}
