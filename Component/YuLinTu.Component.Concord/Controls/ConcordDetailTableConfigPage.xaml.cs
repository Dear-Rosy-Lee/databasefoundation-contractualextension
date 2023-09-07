/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Concord
{
    /// <summary>
    /// ConcordDetailTableConfigPage.xaml 的交互逻辑-合同明细表设置
    /// </summary>
    public partial class ConcordDetailTableConfigPage:WorkpageOptionsEditor
    {
        #region Ctor

        public ConcordDetailTableConfigPage(IWorkpage workpage):base(workpage)           
        {
            InitializeComponent();
        }
        #endregion

        #region Fields

        private DataSummaryDefine config;
        private DataSummaryDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public DataSummaryDefine OtherDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.ProGrid.Object = OtherDefine;
            }
        }

        /// <summary>
        /// 常用配置页
        /// </summary>
        public PropertyGrid ProGrid { get { return this.propertyGrid; } }

        #endregion

        #region Method-Override

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
            var are = new AutoResetEvent(false);
            Dispatcher.Invoke(new Action(() =>
            {
                var propertyCount = typeof(DataSummaryDefine).GetProperties().Count();
                ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源 
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<DataSummaryDefine>();
                var section = profile.GetSection<DataSummaryDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
                config = (section.Settings as DataSummaryDefine);   //得到经反序列化后的对象
                OtherDefine = config.Clone() as DataSummaryDefine;
                OtherDefine.WarrantNumber = false;               
                otherDefine.ColumnCount = 14;
                ProGrid.InitializeEnd += (s, e) => { are.Set(); };
                ProGrid.Object = OtherDefine;
            }));
            are.WaitOne();
            YuLinTu.Windows.Wpf.Metro.Components.PropertyDescriptor warrantNumBoolean = ProGrid.PropertyDescriptors.Find(t => t.AliasName == "权证编号");
            warrantNumBoolean.Visibility = Visibility.Collapsed;      
            
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<DataSummaryDefine>();
            }));
        }
        #endregion
    }
}
