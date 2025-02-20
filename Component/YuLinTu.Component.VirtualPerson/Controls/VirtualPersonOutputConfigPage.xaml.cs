/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Threading;

namespace YuLinTu.Component.VirtualPerson
{
    /// <summary>
    /// 导入承包方表格设置
    /// </summary>
    public partial class VirtualPersonOutputConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public VirtualPersonOutputConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Field

        private FamilyOutputDefine config;
        private FamilyOutputDefine outputDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public FamilyOutputDefine OutputDefine
        {
            get { return outputDefine; }
            set
            {
                outputDefine = value;
                this.ProGrid.Object = OutputDefine;
            }
        }

        /// <summary>
        /// 常用配置页
        /// </summary>
        public PropertyGrid ProGrid { get { return this.propertyGrid; } }

        #endregion

        #region Override

        /// <summary>
        /// 展示
        /// </summary>
        protected override void OnShown()
        {
            var are = new AutoResetEvent(false);

            Dispatcher.Invoke(new Action(() =>
            {
                var propertyCount = typeof(FamilyOutputDefine).GetProperties().Count();
                ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源 
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<FamilyOutputDefine>();
                var section = profile.GetSection<FamilyOutputDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
                config = (section.Settings as FamilyOutputDefine);   //得到经反序列化后的对象

                ProGrid.InitializeEnd += (s, e) => { are.Set(); };
                OutputDefine = config.Clone() as FamilyOutputDefine;
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
                config.CopyPropertiesFrom(OutputDefine);
                systemCenter.Save<FamilyOutputDefine>();
            }));
        }

        #endregion
    }
}

