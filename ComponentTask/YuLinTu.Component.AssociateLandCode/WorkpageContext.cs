/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using YuLinTu.Appwork;
using YuLinTu.Component.ContractAccount;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.AssociateLandCode
{
    public class WorkpageContext : TheNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 承包台账主界面
        /// </summary>
        private ContractAccountPanel contractAccountPanel;
        private DropDownButton btnInitialData;
        private IWorkpage TheWorkPage;

        #endregion Fields

        #region Ctor

        public WorkpageContext(IWorkpage workpage) : base(workpage)
        {
            this.TheWorkPage = workpage;
        }

        #endregion Ctor

        #region Methods

        #region Methods-Message

        /// <summary>
        /// 注册导航模板
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
        }                

        /// <summary>
        /// 加载工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         
        protected override void OnInstalToolbar(object sender, InstallUIElementsEventArgs e)
        {
            //承包台账模块
            ContractAccountFramePage contractAccountPage = PageContent as ContractAccountFramePage;
            if (contractAccountPage != null)
            {
                contractAccountPanel = contractAccountPage.contractAccountPanel;
                btnInitialData = e.FindByName<DropDownButton>("btnSurveyTable");
                btnInitialData.Visibility = Visibility.Visible;
                var InitialDataPanel = btnInitialData.DropDownContent as StackPanel;
                var exportsurvryexcelbtn = CreateInitialLandNumberBtn();
                InitialDataPanel.Children.Add(exportsurvryexcelbtn);
            }
        }

        #endregion Methods-Message

        #region Methods - Private

        /// <summary>
        /// 创建按钮
        /// </summary>
        private SuperButton CreateInitialLandNumberBtn()
        {
            SuperButton familyAccountBtn = new SuperButton();
            familyAccountBtn.Padding = new Thickness(8, 4, 8, 4);
            familyAccountBtn.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Excel.png"));
            familyAccountBtn.Content = "导出关联数据摸底调查表";
            familyAccountBtn.ToolTip = @"按区域导出摸底调查表。";
            familyAccountBtn.Click += InitialLandNumberHandle;
            return familyAccountBtn;
        }

        #endregion Methods - Private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialLandNumberHandle(object sender, RoutedEventArgs e)
        {
            if (btnInitialData != null)
                btnInitialData.IsOpen = false;
            string showBoxHeader = "导出摸底调查表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(showBoxHeader, ContractAccountInfo.ExportNoZone);
                return;
            }
            if ((int)currentZone.Level > (int)eZoneLevel.County)
            {
                ShowBox(showBoxHeader, "请选择区县、乡镇、村级、组级地域进行数据导出");
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = DbContext.CreateZoneWorkStation();

            int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);

            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, TheWorkPage, showBoxHeader);
            extPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (b == null || !(bool)b)
                    return;
                ExportVerifyExcelTaskGroup(extPage.FileName, "批量导出摸底调查表", showBoxHeader, currentZone, DbContext);
            });
        } 

        /// <summary>
        /// 批量导出摸底调查表
        /// </summary>
        public void ExportVerifyExcelTaskGroup(string fileName, string taskDes, string taskName, Zone currentZone, IDbContext DbContext)
        {
            var groupArgument = new TaskGroupExportLandVerifyExcelArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = eVirtualType.Land;
            var groupOperation = new TaskGroupExportLandVerifyExcel();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }
         
        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                }, action);
            }));
        }
        #endregion Methods
    }
}