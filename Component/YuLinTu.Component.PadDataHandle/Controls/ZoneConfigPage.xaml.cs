/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包系统配置界面
    /// </summary>
    public partial class ZoneConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoneConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private ZoneDefine config;
        private ZoneDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 当前配置
        /// </summary>
        public ZoneDefine CurrentDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.DataContext = otherDefine;
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
                var profile = systemCenter.GetProfile<ZoneDefine>();
                var section = profile.GetSection<ZoneDefine>();
                config = (section.Settings);
                CurrentDefine = config.Clone() as ZoneDefine;
            }));
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(CurrentDefine);
                systemCenter.Save<ZoneDefine>();
            }));
        }

        #endregion

        #region Events

        #endregion

    }
}
