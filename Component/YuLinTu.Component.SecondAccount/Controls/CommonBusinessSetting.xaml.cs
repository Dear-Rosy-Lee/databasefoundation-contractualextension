/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.SecondAccount
{
    /// <summary>
    /// CommonBusinessSetting.xaml 的交互逻辑
    /// </summary>
    public partial class CommonBusinessSetting : OptionsEditor
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Ctor

        public CommonBusinessSetting(IWorkspace workspace)
            :base(workspace)
        {
            InitializeComponent();

        }

        #endregion 

        #region Methods

        #region Methods - Events

        private void btnChangeCommon_Click(object sender, RoutedEventArgs e)
        {
            ZoneSelectorPanel zoneSelectorPanel = new ZoneSelectorPanel();
            Workspace.Window.ShowDialog(zoneSelectorPanel,(b, r) =>
            {
                   
            });

        }

        #endregion

        #endregion
    }
}
