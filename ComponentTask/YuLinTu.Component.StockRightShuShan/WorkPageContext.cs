using System;
using YuLinTu.Appwork;
using YuLinTu.Component.StockRightShuShan.Control;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightShuShan
{
    public class WorkPageContext : BusinessNavigatableWorkpageContext
    {
        private ZoneNavigator _nav = new ZoneNavigator();

        private MainFramePage FramePage => (PageContent as MainFramePage);

        public WorkPageContext(IWorkpage workpage)
          : base(workpage)
        {

        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langInstallNavigateItems)]
        private void OnInstallNavigateItems(object sender, InstallNavigateItemMsgEventArgs e)
        {
            e.Instance.Items.AddRange(_nav.GetChildren(e.Instance.Root));
        }


        /// <summary>
        /// 系统设置变更时，更新数据源DbContext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSettingsChanged(object sender, SettingsProfileChangedEventArgs e)
        {
            base.OnSettingsChanged(sender, e);
            if (e.Profile.Name == TheBns.stringDataSourceNameChangedMessageKey)
            {
                FramePage.DbContext = DataBaseSource.GetDataBaseSource();
            }
        }


        protected override void OnNavigateSelectedItemChanged(object sender, NavigateSelectedItemChangedEventArgs e)
        {
            base.OnNavigateSelectedItemChanged(sender, e);
            var zone = e.Object?.Object as Zone;
            if (string.IsNullOrWhiteSpace(zone?.FullCode))
                return;
            FramePage.SwitchZoneCode(zone);
        }




        /// <summary>
        /// 注册导航模板
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            if (FramePage != null)
            {
                FramePage.DbContext = DataBaseSource.GetDataBaseSource();
            }
        }


        /// <summary>
        /// 刷新界面
        /// </summary>
        [MessageHandler(Name = "StockRightTaiNing_Refresh")]
        private void RefreshUi()
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                Navigator.Refresh();
                if (FramePage != null)
                {
                    FramePage.Refresh();
                }
            }));
        }


        protected override void OnWorkspaceMessageReceived(object sender, MsgEventArgs e)
        {

            switch (e.Name)
            {
                case "StockRightTaiNing_Refresh":
                    RefreshUi();
                    break;
                default:
                    base.OnWorkspaceMessageReceived(sender, e);
                    break;
            }
        }


    }
}
