/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 计算承包地块确权面积和实测面积界面
    /// </summary>
    public partial class ContractLandInitializeArea : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitializeArea(bool isBatch = false)
        {
            InitializeComponent();
            this.Header = isBatch ? "批量初始化地块面积" : "初始化地块面积";
            btnConfirm.IsEnabled = false;
        }

        #endregion

        #region Fields

        private bool toActualArea;  //是否图斑面积到实测面积
        private bool toAwareArea;   //是否图斑面积到确权面积
        private Zone currentZone;
        private int toAreaNumeric;  //小数位数
        private int toAreaModule;   //面积截取模式

        #endregion

        #region Properties

        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric
        {
            get { return toAreaNumeric; }
            set
            {
                toAreaNumeric = value;
                txt_Point.Value = value;
            }
        }

        /// <summary>
        /// 面积截取模式
        /// </summary>
        public int ToAreaModule
        {
            get { return toAreaModule; }
            set
            {
                toAreaModule = value;
            }
        }
        /// <summary>
        /// 是否图斑面积到实测面积
        /// </summary>
        public bool ToActualArea
        {
            get { return toActualArea; }
            set
            {
                toActualArea = value;
                SetControlEnable();
            }
        }

        /// <summary>
        /// 是否图斑面积到确权面积
        /// </summary>
        public bool ToAwareArea
        {
            get { return toAwareArea; }
            set
            {
                toAwareArea = value;
                SetControlEnable();
            }
        }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                txtZoneName.Text = currentZone.FullName;
            }
        }

        public bool InstallEmpty { get; set; }
        public bool InstallContract { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 设置按钮可用性
        /// </summary>
        public void SetControlEnable()
        {
            if (toAwareArea || toActualArea)
            {
                btnConfirm.IsEnabled = true;
            }
            else
            {
                btnConfirm.IsEnabled = false;
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Point.Value == null)
            {
                ShowBox("提示", "小数位数不允许为空");
                return;
            }
            toAreaNumeric = (int)txt_Point.Value;
            toAreaModule = cbModule.SelectedIndex;
            InstallEmpty = (bool)cbOnlyEmpty.IsChecked;
            InstallContract = (bool)cbOnlyContactLand.IsChecked;
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 是否图斑面积到实测面积
        /// </summary>
        private void cbToActualArea_Click(object sender, RoutedEventArgs e)
        {
            ToActualArea = (bool)cbToActualArea.IsChecked;
        }

        /// <summary>
        /// 是否图斑面积到确权面积
        /// </summary>
        private void cbToAwareArea_Click(object sender, RoutedEventArgs e)
        {
            ToAwareArea = (bool)cbToAwareArea.IsChecked;
        }

        #endregion
        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }
    }
}
