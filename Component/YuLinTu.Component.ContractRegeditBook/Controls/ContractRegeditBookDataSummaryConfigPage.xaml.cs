/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Linq;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractRegeditBook
{
    /// <summary>
    /// ContractRegeditBookDataSummaryConfigPage.xaml 的交互逻辑-数据汇总表设置
    /// </summary>
    public partial class ContractRegeditBookDataSummaryConfigPage:WorkpageOptionsEditor
    {
        #region Ctor

        public ContractRegeditBookDataSummaryConfigPage(IWorkpage workpage)
            : base(workpage)           
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
            Dispatcher.Invoke(new Action(() =>
            {
                var propertyCount = typeof(DataSummaryDefine).GetProperties().Count();
                ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源 
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<DataSummaryDefine>();
                var section = profile.GetSection<DataSummaryDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
                config = (section.Settings as DataSummaryDefine);   //得到经反序列化后的对象
                OtherDefine = config.Clone() as DataSummaryDefine;
                        
                ProGrid.Object = OtherDefine;
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
                systemCenter.Save<DataSummaryDefine>();
            }));
        }
        #endregion
    }
}
