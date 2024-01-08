using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.WorkStation;
using System.IO;
using YuLinTu.Appwork;
using Xceed.Wpf.Toolkit;
using YuLinTu.Library.Controls;
using YuLinTu.Component.ContractAccount;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static YuLinTu.Library.Controls.ContractAccountPanel;

namespace YuLinTu.Component.FuSui
{
    public class WorkpageContext : TheNavigatableWorkpageContext
    {
        #region Fields

        private ContractRegeditBookItem currentRegiditItem;
        private BindContractRegeditBook currentRegeditBook;
        private DropDownButton btnSurveyTable;
        private ContractLandBinding currentLandBinding;
        private ContractLandPersonItem currentAccountItem;
        private SuperButton issueSuperButton;

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        public IWorkpage TheWorkPage { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        public List<Dictionary> DictList
        {
            get
            {
                DictionaryBusiness dictBusiness = new DictionaryBusiness(DbContext);
                List<Dictionary> dictList = dictBusiness.GetAll();
                return dictList;
            }
        }

        /// <summary>
        /// 权证主界面
        /// </summary>
        private ContractRegeditBookPanel contractRegeditBookPanel;

        /// <summary>
        /// 承包台账主界面
        /// </summary>
        private ContractAccountPanel contractAccountPanel;

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        /// <summary>
        /// 系统设置
        /// </summary>
        private SystemSetDefine m_systemSet = SystemSetDefine.GetIntence();

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public YuLinTu.Library.Business.SystemSetDefine SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                var config = section.Settings as SystemSetDefine;
                return config;
            }
        }

        public static ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        #endregion Fields

        public WorkpageContext(TheWorkpage workpage) : base(workpage)
        {
        }

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
                this.contractAccountPanel = contractAccountPage.contractAccountPanel;

                var spSurveyTable = contractAccountPage.btnSurveyTable.DropDownContent as StackPanel;

                spSurveyTable.Children.Insert(5, CreateExportLandWordBtn());

                btnSurveyTable = contractAccountPage.btnSurveyTable;

                contractAccountPage.btnExportContractInformation.Command = null;
                contractAccountPage.btnExportContractInformation.CommandBindings.Clear();
                contractAccountPage.btnExportContractInformation.CommandParameter = "";
                contractAccountPage.btnExportContractInformation.Click += BtnExportContractInformation_Click;
            }
        }

        #endregion Methods-Message

        #region 创建按钮

        /// <summary>
        /// 创建摸底表
        /// </summary>
        /// <returns></returns>
        private SuperButton CreateExportLandWordBtn()
        {
            SuperButton familyAccountBtn = new SuperButton();
            familyAccountBtn.Padding = new Thickness(8, 4, 8, 4);
            familyAccountBtn.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Excel.png"));
            familyAccountBtn.Content = "扶绥延包摸底表";
            familyAccountBtn.ToolTip = "导出摸底调查表";
            familyAccountBtn.Click += btnAgriLandTable_Click;
            return familyAccountBtn;
        }

        #endregion 创建按钮

        #region 承包地块调查表

        private void BtnExportContractInformation_Click(object sender, RoutedEventArgs e)
        {
            int TableType = 6;
            var currentZone = contractAccountPanel.CurrentZone;
            bool isAccountExcel = true;
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            this.DbContext = DbContext;
            if (currentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            string markDesc;
            if (isAccountExcel)
            {
                markDesc = ContractAccountInfo.ExportContractInformationExcel;
            }
            else
            {
                markDesc = ContractAccountInfo.ExportContractLandSurveyExcel;
            }

            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                TaskExportContractDelayAccountExcel(eContractAccountType.ExportContractInformationExcel, markDesc, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
        }

        private void btnAgriLandTable_Click(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            string titletip = "导出承包地块调查表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = DbContext.CreateZoneWorkStation();
            var dicStation = DbContext.CreateDictWorkStation();
            List<Dictionary> DictList = dicStation.Get();
            bool isAccountExcel = true;
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 1;
            if (currentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            //批量导出
            if (currentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
            string markDesc = string.Empty;
            if (isAccountExcel)
            {
                markDesc = ContractAccountInfo.ExportContractAccountSurveyExcel;
            }
            else
            {
                markDesc = ContractAccountInfo.ExportContractLandSurveyExcel;
            }

            List<Zone> SelfAndSubsZones = new List<Zone>();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                TaskExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
            }
        }

        #endregion 承包地块调查表

        /// <summary>
        /// 单进度导出台账5个表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportContractAccountExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null, int TableType = 1)
        {
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            var dicStation = DbContext.CreateDictWorkStation();
            List<Dictionary> DictList = dicStation.Get();
            var currentZone = contractAccountPanel.CurrentZone;
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskAccountFiveTableArgument meta = new TaskAccountFiveTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.UserName = "";
            meta.Date = date;
            meta.TableType = TableType;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractor = listPerson;
            meta.IsShow = true;
            //if (TableType == 4)
            //{
            //    //如果是公示确认表，需要重新赋值底层设置实体，从公示表配置读
            //    meta.ContractLandOutputSurveyDefine = publicityConfirmDefine.ConvertTo<PublicityConfirmDefine>();// (PublicityConfirmDefine)publicityConfirmDefine;
            //}
            //else
            //{
            //    meta.ContractLandOutputSurveyDefine = ContractAccountDefine;
            //}
            meta.DictList = DictList;
            TaskContractAccountOperationFuSui import = new TaskContractAccountOperationFuSui();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

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

        /// <summary>
        /// 获取承包台账选择项
        /// </summary>
        private void GetContractAccountSelectItem()
        {
            currentAccountItem = null;
            currentLandBinding = null;
            var item = contractAccountPanel.view.SelectedItem;
            if (item is ContractLandPersonItem)
            {
                //界面上选中的是承包方(界面实体)
                currentAccountItem = item as ContractLandPersonItem;
            }
            if (item is ContractLandBinding)
            {
                //界面上选中的是承包地块(界面实体)
                currentLandBinding = item as ContractLandBinding;
                var personItem = contractAccountPanel.accountLandItems.FirstOrDefault(c => c.Tag.ID == currentLandBinding.Tag.OwnerId);
                currentAccountItem = personItem;
            }
        }

        /// <summary>
        /// 设置公示表中日期
        /// </summary>
        /// <returns></returns>
        private DateTime? SetPublicyTableDate()
        {
            DateTime date = DateTime.Now;
            return date;
        }

        /// <summary>
        ///  获取上级地域
        /// </summary>
        private Zone GetParent(Zone zone, IDbContext dbContext)
        {
            Zone parentZone = null;
            if (zone == null || dbContext == null)
                return parentZone;
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                parentZone = zoneStation.Get(zone.UpLevelCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取父级地域失败!)", ex.Message + ex.StackTrace);
            }
            return parentZone;
        }

        private void TaskExportContractDelayAccountExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
          List<VirtualPerson> listPerson = null, int TableType = 1)
        {
            DateTime? date = SetPublicyTableDate();
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            CurrentZone = contractAccountPanel.CurrentZone;
            SelfAndSubsZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(CurrentZone);

            TaskAccountFiveTableArgument meta = new TaskAccountFiveTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = CurrentZone;
            meta.UserName = "";
            meta.Date = date;
            meta.TableType = TableType;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractor = listPerson;
            meta.IsShow = true;
            //if (TableType == 4)
            //{
            //    //如果是公示确认表，需要重新赋值底层设置实体，从公示表配置读
            //    meta.ContractLandOutputSurveyDefine = publicityConfirmDefine.ConvertTo<PublicityConfirmDefine>();// (PublicityConfirmDefine)publicityConfirmDefine;
            //}
            //else
            //{
            //    meta.ContractLandOutputSurveyDefine = ContractAccountDefine;
            //}
            meta.DictList = DictList;
            TaskContractAccountOperationFuSui import = new TaskContractAccountOperationFuSui();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        #endregion Methods
    }
}