using System.Windows;
using System.Windows.Controls;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation.Controls
{
    /// <summary>
    /// ClearDataCommon.xaml 的交互逻辑
    /// </summary>
    public partial class ClearDataCommon : InfoPageBase

    {
        //当前选择的地域
        public Zone SelectZone { get; set; }
        public ClearDataCommon(IWorkpage page, Zone currentZone,bool IsZone)
        {
            InitializeComponent();
            DataContext = this;
            this.Workpage = page;
            txtZone_root.IsEnabled = false;
            ComZone = CommonSelectZoneDefine.GetIntence();
            if (!IsZone)
            {
                labelDY.Visibility = Visibility.Collapsed;
                ZonePanel.Visibility = Visibility.Collapsed;
                ClearAllData.Visibility = Visibility.Visible;
            }
            else
            {
                labelDY.Visibility = Visibility.Visible;
                ZonePanel.Visibility = Visibility.Visible;
                ClearAllData.Visibility = Visibility.Collapsed;
            }
            if (SelectZone == null)
                SelectZone = new Zone();
            SelectZone.FullCode = ComZone.CurrentZoneFullCode;
            SelectZone.FullName = ComZone.CurrentZoneFullName;
            if (currentZone != null)
            {
                ComZone.CurrentZoneFullCode = currentZone.FullCode;
                ComZone.CurrentZoneFullName = currentZone.FullName;
                SelectZone = currentZone;                
            }
        }
        #region Properties

        /// <summary>
        /// 依赖属性 绑定ComZone实体
        /// </summary>
        public CommonSelectZoneDefine ComZone
        {
            get { return (CommonSelectZoneDefine)GetValue(ComZoneProperty); }
            set { SetValue(ComZoneProperty, value); }
        }

        public static readonly DependencyProperty ComZoneProperty =
            DependencyProperty.Register("ComZone", typeof(CommonSelectZoneDefine), typeof(ClearDataCommon));
        #endregion
        private void btnExcuteClear_Click(object sender, RoutedEventArgs e)
        {
            Workpage.Page.CloseMessageBox(true);
        }
        /// <summary>
        /// 选择导航地域
        /// </summary>
        private void btnChangeCommon_Click(object sender, RoutedEventArgs e)
        {
            Zone selectzone = null;
            ZoneSelectorPanel zoneSelectorPanel = new ZoneSelectorPanel();
            zoneSelectorPanel.DbContext = DataSource.Create<IDbContext>(Workpage.Workspace.Properties.TryGetValue("CurrentDataSourceName", TheBns.Current.GetDataSourceName()));
            zoneSelectorPanel.SelectorZone = new ZoneDataItem() { FullCode = SelectZone != null ? SelectZone.FullCode : ComZone.CurrentZoneFullCode };
            zoneSelectorPanel.ConfirmStart += (s, a) =>
            {
                selectzone = zoneSelectorPanel.RootZone.ConvertTo<Zone>();
            };
            zoneSelectorPanel.Confirm += (s, a) =>
            {
                if (selectzone == null)
                    throw new YltException("你没有选择任何地域");

                SelectZone = selectzone;
            };
            zoneSelectorPanel.ConfirmCompleted += (s, a) =>
            {
                ComZone.CurrentZoneFullName = zoneSelectorPanel.RootZone.FullName;
                ComZone.CurrentZoneFullCode = zoneSelectorPanel.RootZone.FullCode;
             };
            zoneSelectorPanel.ConfirmTerminated += (s, a) =>
            {
                if (a.Exception is YltException)
                {
                    Workpage.Page.ShowDialog(new MessageDialog()
                    {
                        Message = a.Exception.Message,
                        Header = "提示",
                        MessageGrade = eMessageGrade.Error
                    });
                }
                else
                {
                    Workpage.Page.ShowDialog(new MessageDialog()
                    {
                        Message = string.Format("发生了一个异常。{0}", a.Exception),
                        Header = "提示",
                        MessageGrade = eMessageGrade.Error
                    });
                }
            };
            Workpage.Workspace.Window.ShowDialog(zoneSelectorPanel);
        }
        /// <summary>
        private void txtZone_root_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txtZone_root.Text.Trim();           
        }
    }
}
