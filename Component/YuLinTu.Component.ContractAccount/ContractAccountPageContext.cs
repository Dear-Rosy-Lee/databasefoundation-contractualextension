/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Component.MapFoundation;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账工作页上下文
    /// </summary>
    public class ContractAccountPageContext : BusinessNavigatableWorkpageContext
    {
        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #region Ctor

        /// <summary>
        /// 构造函数,初始化工作页
        /// </summary>
        /// <param name="workpage"></param>
        public ContractAccountPageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        /// <summary>
        /// 注册导航模板     
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            ContractAccountFramePage page = PageContent as ContractAccountFramePage;
            page.Workpage = Workpage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    var message = new TabMessageBoxDialog
                    {
                        Header = "连接数据源",
                        Message = DataBaseSource.ConnectionError,
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "取消",
                    };
                    Workpage.Page.ShowMessageBox(message);
                    return;
                }
                page.contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                page.contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(dbContext);
                page.contractAccountPanel.DbContext = dbContext;
                page.contractAccountPanel.VirtualType = eVirtualType.Land;
                page.contractAccountPanel.DictBusiness = new DictionaryBusiness(dbContext);
            }
        }

        protected override void OnUninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            //PageContent.Dispose();
        }

        protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        {
            var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Navigation/Res.xaml") };
            var key = new DataTemplateKey(typeof(NavigateZoneItem));
            if (Navigator != null)
            {
                Navigator.RootItemAutoExpand = false;
                Navigator.RegisterItemTemplate(typeof(NavigateZoneItem), dic[key] as DataTemplate);
            }
            var menu = dic["TreeViewNavigator_Menu_Zone"] as ContextMenu;
            Navigator.RegisterContextMenu(typeof(Zone), menu);
            Navigator.AddCommandBinding(ZoneNavigatorCommands.CopyCommandBinding);
        }

        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            base.OnInstallAccountData(sender, e);
            ContractAccountFramePage page = PageContent as ContractAccountFramePage;
            if (page == null)
                return;
            page.mbtnUpdateData.Visibility = Visibility.Collapsed;
            page.mbtnDownLoad.Visibility = Visibility.Collapsed;
            Guid? guid = TheApp.Current.GetWorkspaceUserSessionCode(Workpage.Workspace);
            if (guid != null)
            {
                page.mbtnUpdateData.Visibility = Visibility.Visible;
                page.mbtnDownLoad.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langInstallNavigateItems)]
        private void OnInstallNavigateItems(object sender, InstallNavigateItemMsgEventArgs e)
        {
            try
            {
                e.Instance.Items.AddRange(nav.GetChildren(e.Instance.Root));
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                { //此时当前工作空间中没有打开鱼鳞图地图模块(工作页)
                    var message = new TabMessageBoxDialog
                    {
                        Header = "提示",
                        Message = "访问数据库失败，请检查数据库是否为最新数据库!",
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "取消",
                    };
                    Workpage.Page.ShowMessageBox(message);
                }));
            }
        }

        /// <summary>
        /// 点击节点发送消息
        /// </summary>
        protected override void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.OnNavigateTo(sender, e);

            if (e.Object == null)
            {
                ContractAccountFramePage senderPage = PageContent as ContractAccountFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;
                ContractAccountFramePage senderPage = PageContent as ContractAccountFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    if (dbContext == null)
                    {
                        var message = new TabMessageBoxDialog
                        {
                            Header = "连接数据源",
                            Message = DataBaseSource.ConnectionError,
                            MessageGrade = eMessageGrade.Error,
                            CancelButtonText = "取消",
                        };
                        Workpage.Page.ShowMessageBox(message);
                        return;
                    }
                    senderPage.contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                    senderPage.contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(dbContext);
                    senderPage.contractAccountPanel.DbContext = dbContext;
                    senderPage.contractAccountPanel.VirtualType = eVirtualType.Land;
                    senderPage.contractAccountPanel.DictBusiness = new DictionaryBusiness(dbContext);
                }
            }
        }

        /// <summary>
        /// 界面消息接收
        /// </summary>
        protected override void OnWorkspaceMessageReceived(object sender, MsgEventArgs e)
        {
            switch (e.Name)
            {
                case VirtualPersonMessage.CLEAR_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_ADD_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_EDIT_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_STATUSCHANGE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_SET_COMPLATE:
                    var vp = e.Parameter as VirtualPerson;
                    ChangeLandOwner(vp);
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_COMBINE_COMPLATE:
                    RefreshUi();
                    break;
                case ConcordMessage.CONCORD_CLEAR_COMPLATE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTACCOUNT_EDITGEOMETRY_COMPLETE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTACCOUNT_Refresh:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTACCOUNT_INITIALIMAGENUMBER_COMPLETE:
                    RefreshUi();
                    break;
                default:
                    base.OnWorkspaceMessageReceived(sender, e);
                    break;
            }
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_Refresh)]
        private void RefreshUi()
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                ContractAccountFramePage page = PageContent as ContractAccountFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.contractAccountPanel.Refresh();
                }
            }));
        }

        /// <summary>
        /// 修改地块所有人名称
        /// </summary>
        /// <param name="vp"></param>
        private void ChangeLandOwner(VirtualPerson vp)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                ContractAccountFramePage page = PageContent as ContractAccountFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.contractAccountPanel.ChangeLandOwnerPerson(vp);
                }
            }));
        }

        /// <summary>
        /// 系统配置
        /// </summary>
        [MessageHandler(ID = EdCore.langInstallWorkpageOptionsEditor)]
        private void langInstallWorkpageOptionsEditor(object sender, InstallWorkpageOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "常规",
                Editor = new ContractAccountConfigPage(Workpage),
            });

            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "导入调查表",
                Editor = new ContractAccountImportSurveyConfigPage(Workpage),
            });

            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "导出调查表",
                Editor = new ContractAccountOutputSurveyConfigPage(Workpage),
            });

            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "公示确认表",
                Editor = new ContractAccountPublicityConfirmConfigPage(Workpage),
            });

            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "单户调查表",
                Editor = new ContractAccountSingleFamilySurveyConfigPage(Workpage),
            });

            e.Editors.Add(new WorkpageOptionsEditorMetadata()
            {
                Name = "摸底核实表",
                Editor = new ContractAccountLandVerifyConfigPage(Workpage),
            });

                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "数据汇总表",
                    Editor = new ContractAccountDataSummaryConfigPage(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "上传设置",
                    Editor = new UploadSetting(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "下载设置",
                    Editor = new DownloadSetting(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "地块示意图",
                    Editor = new ContractAccountParcelWordConfigPage(Workpage),
                });
            }));
        }

        /// <summary>
        /// 接收消息-用于获取鱼鳞图模块工作页
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_GETMAPPAGE)]
        private void OnGetMapWorkPage(object sender, ModuleMsgArgs e)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            List<ContractLand> lands = e.Parameter as List<ContractLand>;
            if (lands == null)
            {
                return;
            }
            var mapPage = Workpage.Workspace.Workpages.FirstOrDefault(c => c.Page.Content is YuLinTuMapFoundation);
            if (mapPage == null)
            {
                //此时当前工作空间中没有打开鱼鳞图地图模块(工作页)
                //var message = new TabMessageBoxDialog
                //{
                //    Header = "空间查看",
                //    Message = "未打开鱼鳞图地图模块,请打开后再做空间查看操作!",
                //    MessageGrade = eMessageGrade.Error,
                //    CancelButtonText = "取消",
                //};
                //Workpage.Page.ShowMessageBox(message);
                //return;
                mapPage = Workpage.Workspace.AddWorkpage<YuLinTuMapFoundation>();
            }
            mapPage.Page.Activate();   //激活鱼鳞图地图工作页
            mapPage.Message.Send(this,
                MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_FINDLANDS_COMPLETE, lands));
        }

        /// <summary>
        /// 定位界址点
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_FINDDOT_COMPLETE)]
        private void OnZoomToDot(object sender, ModuleMsgArgs e)
        {
            var mapPage = MapPageOpen.OpenMapPage(Workpage);
            if (mapPage == null)
                return;
            var dot = e.Parameter as BuildLandBoundaryAddressDot;
            if (dot == null)
                return;
            mapPage.Message.Send(this,
                MessageExtend.ContractAccountMsg(DataBaseSource.GetDataBaseSource(), ContractAccountMessage.CONTRACTACCOUNT_FINDDOT_COMPLETE, dot));
        }

        /// <summary>
        /// 定位坐标点
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_FINDCOORDINATE_COMPLETE)]
        private void OnZoomToCoordinate(object sender, ModuleMsgArgs e)
        {
            var mapPage = MapPageOpen.OpenMapPage(Workpage);
            if (mapPage == null)
                return;
            var list = e.Parameter as ArrayList;
            var gpLayer = e.Tag as YuLinTu.tGIS.Client.GraphicsLayer;
            if (list == null)
                return;
            mapPage.Message.Send(this,
                MessageExtend.ContractAccountMsg(DataBaseSource.GetDataBaseSource(), ContractAccountMessage.CONTRACTACCOUNT_FINDCOORDINATE_COMPLETE, list, tag: gpLayer));
        }
    }
}
