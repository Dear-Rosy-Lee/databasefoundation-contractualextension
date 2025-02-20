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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// TableColumnCountSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class TableColumnCountSetPage: InfoPageBase
    {
        private VisioMapLayoutSetInfo vmlsi;

        public TableColumnCountSetPage()
        {
            InitializeComponent();
            DataContext = this;

            vmlsi = VisioMapLayoutSetInfoExtend.DeserializeSelectedSetInfo();
            if (vmlsi.QzbSetInfo.QZBTitleFontColor == null) vmlsi.QzbSetInfo.QZBTitleFontColor = System.Windows.Media.Colors.Black.ToString();
            qzbTitleFontColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.QzbSetInfo.QZBTitleFontColor));
            qzbTitleLabelSize.Value = vmlsi.QzbSetInfo.QZBTitleLabelSize;

            qzbHNumBox.Value = vmlsi.QzbSetInfo.QZBHNumBox;
            if (vmlsi.QzbSetInfo.QZBTableBorderColor == null) vmlsi.QzbSetInfo.QZBTableBorderColor = System.Windows.Media.Colors.Black.ToString();
            qzbTableBorderColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.QzbSetInfo.QZBTableBorderColor));

            qzbTableCellHeightSize.Value = vmlsi.QzbSetInfo.QZBTableCellHeightSize;
            qzbTableCellWidthSize.Value = vmlsi.QzbSetInfo.QZBTableCellWidthSize;
            if (vmlsi.QzbSetInfo.QZBTableLabelColor == null) vmlsi.QzbSetInfo.QZBTableLabelColor = System.Windows.Media.Colors.Black.ToString();
            qzbTableLabelColor.SelectedColor = (Color)(ColorConverter.ConvertFromString(vmlsi.QzbSetInfo.QZBTableLabelColor));

            qzbTableLabelSize.Value = vmlsi.QzbSetInfo.QZBTableLabelSize;  
        }

        #region Events        

        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {
            //保存到序列化文件           
            vmlsi.QzbSetInfo.QZBTitleLabelSize = qzbTitleLabelSize.Value.Value;
            vmlsi.QzbSetInfo.QZBTitleFontColor = qzbTitleFontColor.SelectedColor.ToString();
            vmlsi.QzbSetInfo.QZBHNumBox = qzbHNumBox.Value.Value;
            vmlsi.QzbSetInfo.QZBTableBorderColor = qzbTableBorderColor.SelectedColor.ToString();
            vmlsi.QzbSetInfo.QZBTableCellHeightSize = qzbTableCellHeightSize.Value.Value;
            vmlsi.QzbSetInfo.QZBTableCellWidthSize = qzbTableCellWidthSize.Value.Value;
            vmlsi.QzbSetInfo.QZBTableLabelColor = qzbTableLabelColor.SelectedColor.ToString();
            vmlsi.QzbSetInfo.QZBTableLabelSize = qzbTableLabelSize.Value.Value;
            VisioMapLayoutSetInfoExtend.SerializeSelectedSetInfo(vmlsi); 
            Close(true);
        }     

        #endregion
    }
}
