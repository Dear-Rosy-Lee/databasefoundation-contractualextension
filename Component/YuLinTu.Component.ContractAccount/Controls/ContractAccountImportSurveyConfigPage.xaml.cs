/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Linq;
using System.Threading;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 导入调查表设置
    /// </summary>
    public partial class ContractAccountImportSurveyConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        public ContractAccountImportSurveyConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }
        #endregion

        #region Fields

        private ContractBusinessImportSurveyDefine config;
        private ContractBusinessImportSurveyDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public ContractBusinessImportSurveyDefine OtherDefine
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

        protected override void OnShown()
        {
            var are = new AutoResetEvent(false);

            Dispatcher.Invoke(new Action(() =>
            {
                var propertyCount = typeof(ContractBusinessImportSurveyDefine).GetProperties().Count();
                ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源 
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<ContractBusinessImportSurveyDefine>();
                var section = profile.GetSection<ContractBusinessImportSurveyDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
                config = (section.Settings as ContractBusinessImportSurveyDefine);   //得到经反序列化后的对象

                ProGrid.InitializeEnd += (s, e) => { are.Set(); };
                OtherDefine = config.Clone() as ContractBusinessImportSurveyDefine;
            }));

            are.WaitOne();
        }

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<ContractBusinessImportSurveyDefine>();
            }));
        }

        #endregion

    }
}
