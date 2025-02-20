/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 通用配置页面
    /// </summary>
    public partial class CommonConfigPage : InfoPageBase
    {
        #region Fields

        #endregion

        #region ctor
        public CommonConfigPage(IWorkpage workpage)
        {
            Workpage = workpage;
            InitializeComponent();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 常用配置页
        /// </summary>
        public PropertyGrid ProGrid { get { return this.propertyGrid; } }

        #endregion

        #region Methods

        #endregion

        #region Events

        /// <summary>
        /// 响应确定按钮 接下来是将配置好的信息序列化 
        /// Workpage.Page.CloseMessageBox(true); 
        /// </summary>
        private void MetroButton_OnOk_Click(object sender, RoutedEventArgs e)
        {
            Close(true);
        }

        /// <summary>
        /// PropertyGrid_Alert 会在PropertyGrid状态出现问题时响应  
        /// 如果配置信息出现错误，则隐藏确定按钮，反之，显示
        /// </summary>
        private void PropertyGrid_Alert(object sender, YuLinTu.Windows.Wpf.Metro.Components.PropertyGridAlertEventArgs e)
        {

            btnSubmit.IsEnabled = !(bool)ProGrid.PropertyDescriptors.Any(c => c.Grade >= eMessageGrade.Error);
               
        }

        #endregion

    }
}
