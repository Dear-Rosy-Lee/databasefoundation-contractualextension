/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 权属面板
    /// </summary>
    public partial class InvestigationPersonPanel : UserControl
    {
        #region Fields

        private IWorkpage theWorkPage;

        #endregion

        #region Properties

        /// <summary>ml
        /// 承包方基本管理面板属性
        /// </summary>
        public InvestigationOwnershipPanel OwerShipPanel
        {
            get { return owerShipPanel; }
            set
            {
                owerShipPanel = value;
            }
        }

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage TheWorkPage
        {
            get { return theWorkPage; }
            set
            {
                theWorkPage = value;
                OwerShipPanel.Theworkpage = value;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public InvestigationPersonPanel()
        {
            InitializeComponent();
            title.Text = "权属调查";
            OwerShipPanel.Pcpcontrol = personControlPanel;
            OwerShipPanel.LandPanelControl = OwerShipPanel.panelLand;
        }

        #endregion

        #region Events

        /// <summary>
        /// 更新按钮
        /// </summary>
        private void mbtnUpdate_Click_1(object sender, RoutedEventArgs e)
        {
            OwerShipPanel.Refresh();
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                theWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = "查询地块",
                    Message = "请输入当前地域下的地块全编码或者顺序码进行查找!",
                    MessageGrade = eMessageGrade.Infomation,
                    CancelButtonText = "取消",
                });
                return;
            }
            OwerShipPanel.Search(txtSearch.Text);
        }


        #endregion
    }
}
