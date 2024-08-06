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
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// WarrantSetting.xaml 的交互逻辑
    /// </summary>
    public partial class WarrantSetting  : InfoPageBase
    {
        private SettingsProfileCenter systemCenter;
        private WarantSettingModel config;

        public WarantSettingModel Model { get; set; }

        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        public WarrantSetting(IDbContext dbContext,Zone currentZone)
        {
            InitializeComponent();
            this.DbContext = dbContext;
            this.CurrentZone = currentZone;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<WarantSettingModel>();
            var section = profile.GetSection<WarantSettingModel>();
            config = (section.Settings as WarantSettingModel);
            //Model = config.Clone() as WarantSettingModel;
            Model = new WarantSettingModel();
            this.Model.DbContext = dbContext;
            this.Model.CurrentZone = CurrentZone;
            DataContext = Model;
            base.Confirm += Confirm;
        }



        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
            Close();
        }




        private new void Confirm(object sender, MsgEventArgs<bool> e)
        {
            e.Parameter = true;
            Dispatcher.Invoke(new Action(() =>
            {
                if (Model.WriteTime > Model.SendTime)
                {
                    ShowBox("签订权证", "填证日期不能大于颁证日期");
                    return;
                }
                var concords = DbContext.CreateStockConcordWorkStation().Get(o => o.ZoneCode == CurrentZone.FullCode);
                if (concords.Count == 0)
                {
                    ShowBox("签订权证", "请先签订合同");
                    return;
                }
                var warrants = new List<StockWarrant>();
                var personStockLand = DbContext.CreateBelongRelationWorkStation().GetPersonStockLand(CurrentZone.FullCode);
                var sender2 = DbContext.CreateSenderWorkStation().Get(CurrentZone.ID);
                foreach (var p in personStockLand)
                {
                    if (p.StockLands.Count != 0)
                    {
                        var concord = concords.Find(o => o.ContracterId == p.Contractor.ID);
                        var warrant = Helper.Helper.CreateContractRegeditBook(sender2, p.Contractor, concord, Model, CurrentZone);
                        warrants.Add(warrant);
                    }
                }
                DbContext.CreateStockWarrantWorkStation().Add(warrants);
                ShowBox("签订权证", "权证签订成功！",eMessageGrade.Infomation);
                config = new WarantSettingModel(DbContext,CurrentZone);
                config.CopyPropertiesFrom(Model);
                systemCenter.Save<WarantSettingModel>();
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
