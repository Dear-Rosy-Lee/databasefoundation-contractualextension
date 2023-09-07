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
    /// 承包方基本管理控件
    /// </summary>
    public partial class InvestigationPersonControlPanel : UserControl
    {
        #region Fields

        public delegate void ClickEvent(object sender, RoutedEventArgs e);

        /// <summary>
        /// 过滤条件改变
        /// </summary>
        public ClickEvent TextChanged;

        /// <summary>
        /// 全选改变
        /// </summary>
        public ClickEvent AllCheck;

        /// <summary>
        /// 选择承包方
        /// </summary>
        public ClickEvent SelectVpClick;

        /// <summary>
        /// 承包方基本管理
        /// </summary>
        public ClickEvent ManageVpClick;

        /// <summary>
        /// 增加承包方
        /// </summary>
        public ClickEvent AddVpClick;

        /// <summary>
        /// 编辑承包方
        /// </summary>
        public ClickEvent EditVPClick;

        /// <summary>
        /// 删除承包方
        /// </summary>
        public ClickEvent DelVPClick;

        /// <summary>
        /// 查看地图
        /// </summary>
        public ClickEvent MapVPClick;

        #endregion

        #region Properties

        #endregion

        #region Ctor

        public InvestigationPersonControlPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Methods - Event

        /// <summary>
        /// 承包方基本管理
        /// </summary>
        private void vpManageBtn_Click_1(object sender, RoutedEventArgs e)
        {
            ManageVpClick(sender, e);
        }

        /// <summary>
        /// 显示所有承包方
        /// </summary>
        private void vpShowBtn_Click_1(object sender, RoutedEventArgs e)
        {
            SelectVpClick(sender, e);
        }

        /// <summary>
        /// 添加承包方
        /// </summary>
        private void vpAddBtn_Click_1(object sender, RoutedEventArgs e)
        {
            AddVpClick(sender,e);
        }

        /// <summary>
        /// 编辑承包方
        /// </summary>
        private void vpUpdateBtn_Click_1(object sender, RoutedEventArgs e)
        {
            EditVPClick(sender, e);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        private void vpDelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            DelVPClick(sender, e);
        }

        /// <summary>
        /// 查看地图
        /// </summary>
        private void vpMapBtn_Click_1(object sender, RoutedEventArgs e)
        {
            MapVPClick(sender,e);
        }

        #endregion
    
        #endregion

    }
}
