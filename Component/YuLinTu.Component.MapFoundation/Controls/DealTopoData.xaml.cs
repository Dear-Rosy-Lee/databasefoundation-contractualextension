using System.Windows;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// DealTopoData.xaml 的交互逻辑
    /// </summary>
    public partial class DealTopoData : InfoPageBase
    {
        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();
        public bool SmallArea;//碎面
        public bool AreaRepeat;//面节点自重叠
        public bool AreaSelfOverlap;// 面要素边界自重叠
        public bool SharePoint;// 共用边节点打断

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }
        public DealTopoData(IWorkpage workpage)
        {
            InitializeComponent();
            Workpage = workpage;
        }


        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SmallArea = cbSmallArea.IsChecked.Value;
            SharePoint = cbSharePoint.IsChecked.Value;
            AreaRepeat = cbAreaRepeat.IsChecked.Value;
            AreaSelfOverlap = cbAreaSelfOverlap.IsChecked.Value;
            Workpage.Page.CloseMessageBox(true);
        }
    }
}
