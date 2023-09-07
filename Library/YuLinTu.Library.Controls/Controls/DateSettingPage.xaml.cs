/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 时间设置
    /// </summary>
    public partial class DateSettingPage : InfoPageBase
    {
        #region Filds

        #endregion

        #region Property

        /// <summary>
        /// 设置时间
        /// </summary>
        public DateTime? SetTime { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public DateSettingPage()
        {
            InitializeComponent();
            DataContext = this;
            txt_Birthday.Value = DateTime.Now;
        }

        /// <summary>
        /// 执行
        /// </summary>
        private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        {
            SetTime = txt_Birthday.Value;
            Workpage.Page.CloseMessageBox(true);
        }

        #endregion
    }
}
