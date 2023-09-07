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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// CBDlabelSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class CBDlabelSetPage : InfoPageBase
    {

        private VisioMapLayoutSetInfo vmlsi;

        public CBDlabelSetPage()
        {
            InitializeComponent();
            DataContext = this;

            vmlsi = VisioMapLayoutSetInfoExtend.DeserializeSelectedSetInfo();
            if (vmlsi.CbdxzqySetInfo.CbdLabelFontColor == null) vmlsi.CbdxzqySetInfo.CbdLabelFontColor = System.Windows.Media.Colors.Black.ToString();
            cbdLabelFontColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.CbdxzqySetInfo.CbdLabelFontColor));
            cbdLabelSize.Value = vmlsi.CbdxzqySetInfo.CbdLabelSize;
            cbdLabelSeparatorHeight.Value = vmlsi.CbdxzqySetInfo.CbdLabelSeparatorHeight;
            gzoneLabelSize.Value = vmlsi.CbdxzqySetInfo.GroupZoneLabelSize;
            if (vmlsi.CbdxzqySetInfo.GroupZoneLabelColor == null) vmlsi.CbdxzqySetInfo.GroupZoneLabelColor = System.Windows.Media.Colors.Black.ToString();
            gzoneLabelColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.CbdxzqySetInfo.GroupZoneLabelColor));

            dzdwLabelSize.Value = vmlsi.DxmzdwSetInfo.DZDWLabelSize;
            if (vmlsi.DxmzdwSetInfo.DZDWLabelFontColor == null) vmlsi.DxmzdwSetInfo.DZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString();
            dzdwLabelColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.DxmzdwSetInfo.DZDWLabelFontColor));

            xzdwLabelSize.Value = vmlsi.DxmzdwSetInfo.XZDWLabelSize;
            if (vmlsi.DxmzdwSetInfo.XZDWLabelFontColor == null) vmlsi.DxmzdwSetInfo.XZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString();
            xzdwLabelColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.DxmzdwSetInfo.XZDWLabelFontColor));

            mzdwLabelSize.Value = vmlsi.DxmzdwSetInfo.MZDWLabelSize;
            if (vmlsi.DxmzdwSetInfo.MZDWLabelFontColor == null) vmlsi.DxmzdwSetInfo.MZDWLabelFontColor = System.Windows.Media.Colors.Black.ToString();
            mzdwLabelColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.DxmzdwSetInfo.MZDWLabelFontColor));
                    
        }

        #region Events        

        /// <summary>
        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {          
            //保存到序列化文件            
            vmlsi.CbdxzqySetInfo.CbdLabelSize = cbdLabelSize.Value.Value;
            vmlsi.CbdxzqySetInfo.CbdLabelFontColor = cbdLabelFontColor.SelectedColor.ToString();
            vmlsi.CbdxzqySetInfo.CbdLabelSeparatorHeight = cbdLabelSeparatorHeight.Value.Value;
            vmlsi.CbdxzqySetInfo.GroupZoneLabelColor = gzoneLabelColor.SelectedColor.ToString();
            vmlsi.CbdxzqySetInfo.GroupZoneLabelSize = gzoneLabelSize.Value.Value;

            vmlsi.DxmzdwSetInfo.DZDWLabelSize = dzdwLabelSize.Value.Value;
            vmlsi.DxmzdwSetInfo.DZDWLabelFontColor = dzdwLabelColor.SelectedColor.ToString();

            vmlsi.DxmzdwSetInfo.XZDWLabelSize = xzdwLabelSize.Value.Value;
            vmlsi.DxmzdwSetInfo.XZDWLabelFontColor = xzdwLabelColor.SelectedColor.ToString();

            vmlsi.DxmzdwSetInfo.MZDWLabelSize = mzdwLabelSize.Value.Value;
            vmlsi.DxmzdwSetInfo.MZDWLabelFontColor = mzdwLabelColor.SelectedColor.ToString();

            VisioMapLayoutSetInfoExtend.SerializeSelectedSetInfo(vmlsi);

            if (vmlsi.CbdxzqySetInfo.CbdLabelSize == 0)
            {
                ShowBox("提示", "承包地标注大小不能为0！");
                return;
            }
            if (vmlsi.CbdxzqySetInfo.GroupZoneLabelSize == 0)
            {
                ShowBox("提示", "地域标注大小不能为0！");
                return;
            }
            if (vmlsi.DxmzdwSetInfo.DZDWLabelSize == 0)
            {
                ShowBox("提示", "点状地物标注大小不能为0！");
                return;
            }
            if (vmlsi.DxmzdwSetInfo.XZDWLabelSize == 0)
            {
                ShowBox("提示", "线状地物注大小不能为0！");
                return;
            }
            if (vmlsi.DxmzdwSetInfo.MZDWLabelSize == 0)
            {
                ShowBox("提示", "面状地物注大小不能为0！");
                return;
            }




            Close(true);
        }

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error,
            Action<bool?, eCloseReason> action = null, bool showCancel = true, bool showConfirm = true)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                    CancelButtonVisibility = showCancel ? Visibility.Visible : Visibility.Collapsed,
                    ConfirmButtonVisibility = showConfirm ? Visibility.Visible : Visibility.Collapsed,
                }, action);
            }));
        }









        #endregion
    }
}
