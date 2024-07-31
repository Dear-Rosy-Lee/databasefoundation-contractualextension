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
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// ConcordSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ConcordSetting : InfoPageBase
    {
        private SettingsProfileCenter systemCenter;
        private ConcordSettingModel config;

        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        public ConcordSetting()
        {
            InitializeComponent();
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ConcordSettingModel>();
            var section = profile.GetSection<ConcordSettingModel>();
            config = (section.Settings as ConcordSettingModel);
            config.CheckTime = null;
            config.RecordTime = null;
            config.PublicResultTime = null;
            Model = config.Clone() as ConcordSettingModel;
            //Model.LandPurpose = "种植业";
            //Model = new ConcordSettingModel();
            DataContext = Model;
            base.Confirm += Confirm;
        }

        public ConcordSettingModel Model
        {
            get; set;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
            Close();
        }

        private new void Confirm(object sender, MsgEventArgs<bool> e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (Model.StartTime > Model.EndTime)
                {
                    ShowBox("签订合同", "承包起始日期不能大于承包结束日期");
                    return;
                }
                var sender2 = DbContext.CreateSenderWorkStation().Get(CurrentZone.ID);
                var personStockLand=DbContext.CreateBelongRelationWorkStation().GetPersonStockLand(CurrentZone.FullCode);
                if (personStockLand.Count == 0)
                {
                    ShowBox("签订合同","当前地域下没有股农,或没有只有确股的地块的股农");
                    return;
                }
                var stockLands = new List<StockConcord>();
                foreach (var p in personStockLand)
                {
                    if (p.StockLands.Count != 0)
                    {
                        var stockConcord = Helper.Helper.CreateContractConcord(sender2, p.Contractor, Model, CurrentZone);
                        stockLands.Add(stockConcord as StockConcord);
                    }
                };
                DbContext.CreateStockConcordWorkStation().Add(stockLands);
                ShowBox("签订合同", "合同签订成功！",eMessageGrade.Infomation);
                config.CopyPropertiesFrom(Model);
                systemCenter.Save<ConcordSettingModel>();
            }));
        }



        /// <summary>
        /// 提示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var showDlg = new TabMessageBoxDialog()
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            };
            Workpage.Page.ShowMessageBox(showDlg);
        }



    }
}
