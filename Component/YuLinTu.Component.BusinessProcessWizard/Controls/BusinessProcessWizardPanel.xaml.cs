/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Component.ExportDataSummaryTask;
using YuLinTu.Component.Setting;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.BusinessProcessWizard
{
    /// <summary>
    /// BusinessProcessWizardPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BusinessProcessWizardPanel : UserControl
    {
        #region Fields

        //private int index = 0;//序号
        //private bool isbatch;//是否批量
        //private string zoneName;
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueFilter;//过滤数据
        private YuLinTu.Library.Entity.Zone currentZone;
        //private bool allCheck = false;
        //private ConcordItem currentItem;//当前选择项
        //private BindConcord currentConcord;//当前选择合同
        private eVirtualType virtualType;
        private List<object> list = new List<object>();
        //List<YuLinTu.Library.Entity.Dictionary> wayList;   //承包方式
        //List<YuLinTu.Library.Entity.Dictionary> purposeList; //土地用途

        /// <summary>
        /// 合同绑定集合
        /// </summary>
        public ObservableCollection<ConcordItem> Items = new ObservableCollection<ConcordItem>();

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        /// <summary>
        /// 是否批量
        /// </summary>
        public delegate void PanelIsBatch(bool isbatch);

        /// <summary>
        /// 当前地域改变
        /// </summary>
        public delegate void PanelZoneChanged(YuLinTu.Library.Entity.Zone zone);

        #endregion

        #region Properties

        /// <summary>
        /// 是否批量
        /// </summary>
        public PanelIsBatch IsBatchEvt { get; private set; }

        /// <summary>
        /// 当前地域变化
        /// </summary>
        public PanelZoneChanged ZoneChanged { get; set; }

        /// <summary>
        /// 数据统计
        /// </summary>
        public ConcordSummary Summary { get; private set; }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 合同业务类
        /// </summary>
        public ConcordBusiness ConcordBusiness { get; set; }

        /// <summary>
        /// 承包地块业务类
        /// </summary>
        public AccountLandBusiness LandBusiness { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public YuLinTu.Library.Entity.Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                if (value != null) currentZoneLabel.Content = value.FullName;//表头赋值               
                if (ZoneChanged != null)
                {
                    ZoneChanged(currentZone);
                }
            }
        }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set
            {
                virtualType = value;
                PersonBusiness.VirtualType = value;
            }
        }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 共有人
        /// </summary>
        public Person CurrentPerson { get; private set; }

        ///// <summary>
        ///// 合同
        ///// </summary>
        //public YuLinTu.Library.Entity.VirtualPerson virtualPerson
        //{
        //    get { return currentItem == null ? null : currentItem.Tag; }
        //}

        #region 承包方设置

        /// <summary>
        /// 承包方导出设置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<FamilyOutputDefine>();
                var section = profile.GetSection<FamilyOutputDefine>();
                var config = (section.Settings as FamilyOutputDefine);
                return config;
            }
        }

        /// <summary>
        /// 承包方其它设置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<FamilyOtherDefine>();
                var section = profile.GetSection<FamilyOtherDefine>();
                var config = (section.Settings as FamilyOtherDefine);
                return config;
            }
        }

        #endregion

        #region 承包台账设置

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine ContractSettingDefine
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<ContractBusinessSettingDefine>();
                var section = profile.GetSection<ContractBusinessSettingDefine>();
                var config = section.Settings as ContractBusinessSettingDefine;
                return config;
            }
        }

        /// <summary>
        /// 承包台账单户调查表设置
        /// </summary>
        public SingleFamilySurveyDefine SingleFamilySurveySetting
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<SingleFamilySurveyDefine>();
                var section = profile.GetSection<SingleFamilySurveyDefine>();
                var config = (section.Settings as SingleFamilySurveyDefine);
                return config;
            }
        }

        /// <summary>
        /// 承包台账导出设置
        /// </summary>
        public PublicityConfirmDefine ContractAccountDefine
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<PublicityConfirmDefine>();
                var section = profile.GetSection<PublicityConfirmDefine>();
                var config = (section.Settings as PublicityConfirmDefine);
                return config;
            }

        }

        #endregion

        #region 承包合同

        /// <summary>
        /// 承包合同常规设置
        /// </summary>
        public ContractConcordSettingDefine ConcordSettingDefine
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<ContractConcordSettingDefine>();
                var section = profile.GetSection<ContractConcordSettingDefine>();
                var config = section.Settings as ContractConcordSettingDefine;
                return config;
            }
        }

        #endregion

        #region 承包权证

        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ContractRegeditBookExtendUseExcelDefine
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<ContractRegeditBookSettingDefine>();
                var section = profile.GetSection<ContractRegeditBookSettingDefine>();
                var config = section.Settings as ContractRegeditBookSettingDefine;
                return config;
            }
        }

        #endregion

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet
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

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<YuLinTu.Library.Entity.Dictionary> DictList
        {
            get
            {
                DictionaryBusiness dictBusiness = new DictionaryBusiness(DbContext);
                List<YuLinTu.Library.Entity.Dictionary> dictList = dictBusiness.GetAll();
                return dictList;
            }
        }

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenueEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenueEnableControl MenueEnableMethod { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public BusinessProcessWizardPanel()
        {
            InitializeComponent();
            DataContext = this;
            Summary = new ConcordSummary();
            virtualType = eVirtualType.Land;
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);

            #region 初始化
            cbUseNowDataBase.IsChecked = true;
            btnCreateNewDataBase.IsEnabled = false;
            DbContext = DataBaseSource.GetDataBaseSource();
            #endregion
        }

        #endregion

        #region Methods

        #region Methods-数据准备

        /// <summary>
        /// 创建新的数据库
        /// </summary>
        private void btnCreateNewDataBase_Click(object sender, RoutedEventArgs e)
        {
            SpatialReferenceSetting set = new SpatialReferenceSetting(ThePage.Workspace);
            set.mtbCreatNewSqlLitedb_Click(sender, e);
        }

        /// <summary>
        /// 选择创建新的数据库
        /// </summary>
        private void cbUseNewDataBase_Click(object sender, RoutedEventArgs e)
        {
            btnCreateNewDataBase.IsEnabled = true;
        }

        /// <summary>
        ///  选择使用当前数据库
        /// </summary>
        private void cbUseNowDataBase_Click(object sender, RoutedEventArgs e)
        {
            btnCreateNewDataBase.IsEnabled = false;
        }

        #endregion

        #region Methods-数据导入

        #region Methods-数据导入-行政地域

        private void btnImportZoneExcelData_Click(object sender, RoutedEventArgs e)
        {
            ZoneManagerPanel zonePanel = new ZoneManagerPanel();
            zonePanel.CurrentZone = CurrentZone;

            zonePanel.ThePage = ThePage;
            zonePanel.MenuEnable += SetControlsEnable;
            zonePanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            zonePanel.ImportData();
        }

        private void btnImportZoneShapeData_Click(object sender, RoutedEventArgs e)
        {
            ZoneManagerPanel zonePanel = new ZoneManagerPanel();
            zonePanel.CurrentZone = CurrentZone;
            zonePanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            zonePanel.ThePage = ThePage;
            zonePanel.MenuEnable += SetControlsEnable;
            zonePanel.ImportShape();
        }

        #endregion

        #region Methods-数据导入-发包方
        private void btnImportSenderExcelData_Click(object sender, RoutedEventArgs e)
        {
            SenderManagerPanel senderPanel = new SenderManagerPanel();
            senderPanel.CurrentZone = CurrentZone;
            senderPanel.ThePage = ThePage;
            senderPanel.MenuEnable += SetControlsEnable;
            senderPanel.ShowTaskViewer += () =>
             {
                 ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
             };
            senderPanel.ImportData();
        }
        #endregion

        #region Methods-数据导入-承包方
        private void btnImportVirtualPersonExcelData_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                ShowBox("导入承包方", "未选择行政地域!");
                return;
            }
            if (CurrentZone.Level == eZoneLevel.State)
            {
                ShowBox("提示", ContractAccountInfo.ImportErrorZone, eMessageGrade.Error);
                return;
            }
            VirtualPersonPanel virtualPersonPanel = new VirtualPersonPanel();
            virtualPersonPanel.CurrentZone = CurrentZone;
            virtualPersonPanel.ThePage = ThePage;
            virtualPersonPanel.DbContext = DbContext;
            virtualPersonPanel.MenuEnable += SetControlsEnable;
            virtualPersonPanel.Business = new VirtualPersonBusiness(DbContext);
            virtualPersonPanel.VirtualType = eVirtualType.Land;
            virtualPersonPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            virtualPersonPanel.ImportData();
        }
        #endregion

        #region Methods-数据导入-承包台账

        private void btnImportLandExcelData_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                ShowBox("导入承包地块", "未选择行政地域!");
                return;
            }
            if (CurrentZone.Level == eZoneLevel.State)
            {
                ShowBox("提示", "导入地块调查表时地域等级不能为国家级!", eMessageGrade.Warn);
                return;
            }
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportLandExcel();
        }

        private void btnImportBoundaryData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportBoundaryData();
        }

        private void btnImportVectorShapeData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportVectorName();
        }

        private void btnImportBoundaryAddressDotData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportBoundaryAddressDot();
        }

        private void btnImportBoundaryAddressCoilData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportBoundaryAddressCoil();
        }

        private void btnImportZipData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ImportZipData();
        }

        #endregion

        #endregion

        #region Methods-数据处理

        #region Methods-数据处理-承包方

        private void btnVirtualPersonInitializeData_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonPanel virtualPersonPanel = new VirtualPersonPanel();
            virtualPersonPanel.CurrentZone = CurrentZone;
            virtualPersonPanel.ThePage = ThePage;
            virtualPersonPanel.DbContext = DbContext;
            virtualPersonPanel.MenuEnable += SetControlsEnable;
            virtualPersonPanel.Business = new VirtualPersonBusiness(DbContext);
            virtualPersonPanel.VirtualType = eVirtualType.Land;
            virtualPersonPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            virtualPersonPanel.VirtualPersonInitialize();
        }

        #endregion

        #region Methods-数据处理-承包台账

        private void btnSeekLandNeighborData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.SeekLandNeighbor();
        }

        private void btnInitialLandInfoData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.InitialLandInfo();
        }

        private void btnInitializeLandDotCoilInfoData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.InitializeLandDotCoilInfo();
        }

        private void btnInitialMapNumber_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.InitialMapNumber();
        }

        private void btnInitialArea_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.InitialArea();
        }

        private void btnSetAreaNumericFormatData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.SetAreaNumericFormat();
        }

        private void btnInitialIsFarmerData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.InitialIsFarmer();
        }

        #endregion

        #region Methods-数据处理-承包合同
        private void btnInitialContractConcordData_Click(object sender, RoutedEventArgs e)
        {
            ConcordPanel concordPanel = new ConcordPanel();
            concordPanel.CurrentZone = CurrentZone;
            concordPanel.ThePage = ThePage;
            concordPanel.DbContext = DbContext;
            concordPanel.MenueEnableMethod += SetControlsEnable;
            concordPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            concordPanel.ConcordBusiness = new ConcordBusiness(DbContext);
            concordPanel.LandBusiness = new AccountLandBusiness(DbContext);
            concordPanel.VirtualType = eVirtualType.Land;
            concordPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            concordPanel.InitialContractConcord();
        }
        #endregion

        #region Methods-数据处理-承包权证
        private void btnInitalizeWarrentData_Click(object sender, RoutedEventArgs e)
        {
            ContractRegeditBookPanel contractRegeditBookPanel = new ContractRegeditBookPanel();
            contractRegeditBookPanel.CurrentZone = CurrentZone;
            contractRegeditBookPanel.ThePage = ThePage;
            contractRegeditBookPanel.DbContext = DbContext;
            contractRegeditBookPanel.MeunEnable += SetControlsEnable;
            contractRegeditBookPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractRegeditBookPanel.ConcordBusiness = new ConcordBusiness(DbContext);
            contractRegeditBookPanel.ContractRegeditBookBusiness = new ContractRegeditBookBusiness(DbContext);
            contractRegeditBookPanel.AccountLandBusiness = new AccountLandBusiness(DbContext);
            contractRegeditBookPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractRegeditBookPanel.VirtualType = eVirtualType.Land;
            contractRegeditBookPanel.ShowTaskViewer += () =>
               {
                   ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
               };
            contractRegeditBookPanel.InitalizeWarrent();
        }

        #endregion

        #endregion

        #region Methods-数据导出

        #region Methods-数据导出-个人汇总

        private void btnExportDataSummaryTask_Click(object sender, RoutedEventArgs e)
        {
            var workpages = (ThePage as ITheWorkpage).Workspace.Workpages;
            var page = workpages.Find(w => w.Page.Title == "汇总数据");
            if (page == null)
            {
                page = (ThePage as ITheWorkpage).Workspace.AddWorkpage<DataSummaryExportFramePage>();
            }
            page.Activate();
        }

        #endregion

        #region Methods-数据导出-行政地域

        private void btnExportZoneExcelData_Click(object sender, RoutedEventArgs e)
        {
            ZoneManagerPanel zonePanel = new ZoneManagerPanel();
            zonePanel.CurrentZone = CurrentZone;
            zonePanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            zonePanel.ThePage = ThePage;
            zonePanel.MenuEnable += SetControlsEnable;
            zonePanel.ExportData();
        }

        private void btnExportZoneShapeData_Click(object sender, RoutedEventArgs e)
        {
            ZoneManagerPanel zonePanel = new ZoneManagerPanel();
            zonePanel.CurrentZone = CurrentZone;
            zonePanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            zonePanel.ThePage = ThePage;
            zonePanel.MenuEnable += SetControlsEnable;
            zonePanel.ExportShape();
        }

        private void btnExportZonePackage_Click(object sender, RoutedEventArgs e)
        {
            ZoneManagerPanel zonePanel = new ZoneManagerPanel();
            zonePanel.CurrentZone = CurrentZone;
            zonePanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            zonePanel.ThePage = ThePage;
            zonePanel.MenuEnable += SetControlsEnable;
            zonePanel.ExportPackage();
        }

        #endregion

        #region Methods-数据导出-发包方

        private void btnExportSenderExcelData_Click(object sender, RoutedEventArgs e)
        {
            SenderManagerPanel senderPanel = new SenderManagerPanel();
            senderPanel.CurrentZone = CurrentZone;
            senderPanel.ThePage = ThePage;
            senderPanel.MenuEnable += SetControlsEnable;
            senderPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            senderPanel.ExportExcelData();
        }

        private void btnExportSenderWordData_Click(object sender, RoutedEventArgs e)
        {
            SenderManagerPanel senderPanel = new SenderManagerPanel();
            senderPanel.CurrentZone = CurrentZone;
            senderPanel.ThePage = ThePage;
            senderPanel.MenuEnable += SetControlsEnable;
            senderPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            senderPanel.ExportWord();
        }

        #endregion

        #region Methods-数据导出-台账报表

        private void btnExportLandShapeData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ExportLandShapeData();
        }

        private void btnExportLandDotShapeData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ExportLandDotShapeData();
        }

        private void btnExportLandCoilShapeData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ExportLandCoilShapeData();
        }

        private void btnExportLandZipData_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.ExportZipData();
        }

        private void btnBoundaryAddressDotResulttb_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.IsBatch = true;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            contractAccountPanel.BoundaryAddressDotResultExport();
        }


        private void btnMultiParcelExport_Click(object sender, RoutedEventArgs e)
        {
            ContractAccountPanel contractAccountPanel = new ContractAccountPanel();
            contractAccountPanel.CurrentZone = CurrentZone;
            contractAccountPanel.TheWorkPage = ThePage;
            contractAccountPanel.DbContext = DbContext;
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            contractAccountPanel.PersonBusiness = new VirtualPersonBusiness(DbContext);
            contractAccountPanel.ContractAccountBusiness = new AccountLandBusiness(DbContext);
            contractAccountPanel.VirtualType = eVirtualType.Land;
            contractAccountPanel.DictBusiness = new DictionaryBusiness(DbContext);
            contractAccountPanel.DoWork();
            contractAccountPanel.ShowTaskViewer += () =>
            {
                ThePage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };

            var zoneStation = DbContext.CreateZoneWorkStation();
            var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            int childrenCount = zoneStation.Count(CurrentZone.FullCode, eLevelOption.Subs);
            if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
            {
                //批量导出(选择地域大于组级并且当前地域下有子级地域)
                contractAccountPanel.ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.VolumnExportMultiParcelOfFamily,
                    ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily);
            }else if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && childrenCount == 0))
            {
                //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                var landStation = DbContext.CreateContractLandWorkstation();
                var geoLands = landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                if (geoLands == null || geoLands.Count == 0)
                {
                    //当前地域没有空间地块数据
                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                    return;
                }
                List<VirtualPerson> listPerson = vpStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
                //界面上没有选择承包方项(此时弹出承包方选择界面)
                ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                selectPage.Workpage = ThePage;
                selectPage.PersonItems = listPerson;
                ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelNoSelected);
                        return;
                    }
                    contractAccountPanel.ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.ExportMultiParcelOfFamily,
                 ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily, 1, selectPage.SelectedPersons);
                });
            }
            else
            {
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.VolumnExportZoneError);
                return;
            }

        }

        #endregion

        //导出成果库
        private void btnExportResultDataBase_Click(object sender, RoutedEventArgs e)
        {
            var workpages = (ThePage as ITheWorkpage).Workspace.Workpages;
            var page = workpages.Find(w => w.Page.Title == "任务");
            if (page == null)
            {
                page = (ThePage as ITheWorkpage).Workspace.AddWorkpage<TaskPage>();
            }
            page.Activate();
        }

        #endregion

        #endregion

        #region Methods - Private

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {


        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消"
            }, action);
        }
        /// <summary>
        /// 创建承包方集合
        /// </summary>  
        private List<YuLinTu.Library.Entity.VirtualPerson> CreateVirtualPersonCollection()
        {
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            List<YuLinTu.Library.Entity.VirtualPerson> persons = personStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.VirtualPerson> vps = new List<YuLinTu.Library.Entity.VirtualPerson>();
            var orderdVps = persons.OrderBy(vp =>
            {
                //排序
                int num = 0;
                Int32.TryParse(vp.FamilyNumber, out num);
                return num;
            });
            foreach (YuLinTu.Library.Entity.VirtualPerson vp in orderdVps)
            {
                vps.Add(vp);
            }
            //vps.RemoveAll(c => c.Name.Contains("集体"));  //排除集体户
            return vps;
        }


        #endregion

    }
}
