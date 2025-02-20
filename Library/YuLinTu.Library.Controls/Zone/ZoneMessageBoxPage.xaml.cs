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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 消息框
    /// </summary>
    public partial class ZoneMessageBoxPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 业务
        /// </summary>
        public ZoneDataBusiness business { get; set; }

        #endregion

        #region Property

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; private set; }

        #endregion


        #region Ctor

        /// <summary>
        ///构造方法
        /// </summary>
        public ZoneMessageBoxPage(string title, string content, ZoneDataBusiness business, IWorkpage theworkpage)
        {
            InitializeComponent();
            DataContext = this;
            Confirm += MessageBoxPage_Confirm;
            tb_info.Text = content;
            this.Header = title;
            this.business = business;
            this.Workpage = theworkpage;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            Result = business.ClearZone();
            e.Parameter = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 确定
        /// </summary>
        private void btnSure_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
        }

        #endregion
    }
}
