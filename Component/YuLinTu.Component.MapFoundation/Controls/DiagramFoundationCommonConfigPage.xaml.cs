/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// DiagramFoundationCommonConfigPage.xaml 的交互逻辑-公示图的标注设置
    /// </summary>
    public partial class DiagramFoundationCommonConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        private IWorkpage workPage;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public DiagramFoundationCommonConfigPage(IWorkpage workpage) : base(workpage)
        {
            workPage = workpage;
            InitializeComponent();
        }

        #endregion

        #region Fields

        private DiagramFoundationLabelCommonSetting config;
        private DiagramFoundationLabelCommonSetting otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public DiagramFoundationLabelCommonSetting OtherDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.DataContext = OtherDefine;
            }
        }

        #endregion

        #region Method-Override

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<DiagramFoundationLabelCommonSetting>();
                var section = profile.GetSection<DiagramFoundationLabelCommonSetting>();
                config = (section.Settings as DiagramFoundationLabelCommonSetting);
                OtherDefine = config.Clone() as DiagramFoundationLabelCommonSetting;

            }));
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<DiagramFoundationLabelCommonSetting>();
            }));
        }

        #endregion

        #region Events

        #endregion           

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            workPage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

       
        private void txtCheckDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           
        }
        
        private void txtCartographyDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
        }
        //审核日期必须大于制图日期
        private void txtCartographyDate_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (txtCheckDate.Value == null || txtCartographyDate.Value == null)
                return;
            DateTime ds = (DateTime)txtCheckDate.Value;
            ds = DateTime.Parse(ds.ToShortDateString());
            DateTime de = (DateTime)txtCartographyDate.Value;
            de = DateTime.Parse(de.ToShortDateString());

            int day = ((TimeSpan)(ds - de)).Days;

            if (day < 0)
            {
                ShowBox("提示", "审核日期不能小于制图日期!");
                return;
            }
        }
        //审核日期发生变化
        private void txtCheckDate_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (txtCheckDate.Value == null || txtCartographyDate.Value == null)
                return;
            DateTime ds = (DateTime)txtCheckDate.Value;
            ds = DateTime.Parse(ds.ToShortDateString());
            DateTime de = (DateTime)txtCartographyDate.Value;
            de = DateTime.Parse(de.ToShortDateString());

            int day = ((TimeSpan)(ds - de)).Days;

            if (day < 0)
            {
                ShowBox("提示", "审核日期不能小于制图日期!");
                return;
            }
        }
    }
}
