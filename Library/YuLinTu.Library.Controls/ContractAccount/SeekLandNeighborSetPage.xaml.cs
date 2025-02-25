using System.Collections.Generic;
using System.Windows;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// SeekLandNeighborSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SeekLandNeighborSetPage : InfoPageBase
    {

        //private static SeekLandNeighborSetPage seekLandNeighborSetPage;

        public static SeekLandNeighborSetPage Getinstence(IWorkpage page)
        {
            //if (seekLandNeighborSetPage == null)
            //{
            //    seekLandNeighborSetPage = new SeekLandNeighborSetPage(page);
            //    return seekLandNeighborSetPage;
            //}
            return new SeekLandNeighborSetPage(page);
        }


        /// <summary>
        /// 单方向查找
        /// </summary>
        public bool SimplePositionQuery { set; get; }

        /// <summary>
        /// 某方向为空时使用村民小组名称填充
        /// </summary>
        public bool UseGroupName { set; get; }

        /// <summary>
        /// 某方向为空时使用村民小组名称填充
        /// </summary>
        public string UseGroupNameContext { set; get; }

        /// <summary>
        /// 查找地块名称
        /// </summary>
        public bool SearchLandName { set; get; }

        /// <summary>
        /// 识别地块类别
        /// </summary>
        public bool LandIdentify { set; get; }

        /// <summary>
        /// 识别土地利用类型
        /// </summary>
        public bool LandType { set; get; }

        /// <summary>
        /// 只填充空白四至
        /// </summary>
        public bool FillEmptyNeighbor { set; get; }

        /// <summary>
        /// 查询线状\面状地物
        /// </summary>
        public bool IsQueryXMzdw { set; get; }

        /// <summary>
        /// 同至去重复地物名称
        /// </summary>
        public bool IsDeleteSameDWMC { set; get; }

        /// <summary>
        /// 查找规则   "地块地物单名称" 为0, "地块优先" 1,"距离优先" 2
        /// </summary>
        public int SearchDeteilRule { set; get; }

        /// <summary>
        /// 设置宗地查找缓冲距离(米)
        /// </summary>
        public double BufferDistance { set; get; }

        /// <summary>
        /// 查询阈值(米)
        /// </summary>
        public double QueryThreshold { set; get; }

        /// <summary>
        /// 限制当前地域
        /// </summary>
        public bool OnlyCurrentZone { set; get; }

        public SeekLandNeighborSetPage(IWorkpage page)
        {
            InitializeComponent();

            this.DataContext = this;
            this.Workpage = page;
            SimplePositionQuery = true;
            txtQueryThreshold.Text = "0.1";
            txtBufferDistance.Text = "1";//默认缓冲距离为1米
            cmbSearchDeteilRule.IsEnabled = false;
            cmbSearchDeteilRule.ItemsSource = new List<string> { "地块优先", "距离优先", "地块地物单名称" };
            cmbSearchDeteilRule.SelectedIndex = 1;
        }

        #region privateMethod

        #endregion

        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {
            BufferDistance = txtBufferDistance.Value.Value;
            QueryThreshold = txtQueryThreshold.Value.Value;
            switch (cmbSearchDeteilRule.SelectedIndex)
            {
                case 0:
                    SearchDeteilRule = 1;
                    break;
                case 1:
                    SearchDeteilRule = 2;
                    break;
                case 2:
                    SearchDeteilRule = 0;
                    break;
                default:
                    break;
            }
            Workpage.Page.CloseMessageBox(true);
        }


        private void chkSimplePositionQuery_Checked(object sender, RoutedEventArgs e)
        {
            if (chkSimplePositionQuery.IsChecked.Value && chkQueryXMzdw.IsChecked.Value)
            {
                cmbSearchDeteilRule.IsEnabled = true;
            }
            else
            {
                cmbSearchDeteilRule.IsEnabled = false;
                SearchDeteilRule = 2;
                cmbSearchDeteilRule.SelectedIndex = 1;
            }
        }

        private void chkQueryXMzdw_Checked(object sender, RoutedEventArgs e)
        {
            if (chkSimplePositionQuery.IsChecked.Value && chkQueryXMzdw.IsChecked.Value)
            {
                cmbSearchDeteilRule.IsEnabled = true;
            }
            else
            {
                cmbSearchDeteilRule.IsEnabled = false;
                SearchDeteilRule = 2;
                cmbSearchDeteilRule.SelectedIndex = 1;
            }
        }
    }
}
