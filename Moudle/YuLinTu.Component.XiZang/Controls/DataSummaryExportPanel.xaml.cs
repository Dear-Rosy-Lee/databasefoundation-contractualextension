/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Controls;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// DataSummaryExportPanel.xaml 的交互逻辑
    /// </summary>
    public partial class DataSummaryExportPanel : UserControl
    {
        #region Fields

        private int index = 0;//序号
        private bool isbatch;//是否批量
        private string zoneName;
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueFilter;//过滤数据
        private Zone currentZone;
        private bool allCheck = false;
        private ConcordItem currentItem;//当前选择项
        private BindConcord currentConcord;//当前选择合同
        private eVirtualType virtualType;
        private List<object> list = new List<object>();
        List<Dictionary> wayList;   //承包方式
        List<Dictionary> purposeList; //土地用途

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
        public delegate void PanelZoneChanged(Zone zone);

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
        /// 导出表类型
        /// </summary>
        public ExportDataSummarySelectTable ExportDataSummaryTableTypes { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
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
        /// 权利人类型
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
        public ContractBusinessOutputSurveyDefine ContractAccountDefine
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<ContractBusinessOutputSurveyDefine>();
                var section = profile.GetSection<ContractBusinessOutputSurveyDefine>();
                var config = (section.Settings as ContractBusinessOutputSurveyDefine);
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
        public List<Dictionary> DictList
        {
            get
            {
                DictionaryBusiness dictBusiness = new DictionaryBusiness(DbContext);
                List<Dictionary> dictList = dictBusiness.GetAll();
                return dictList;
            }
        }
        ///// <summary>
        ///// 设置控件可用性委托
        ///// </summary>
        //public delegate void MenueEnableControl(bool isEnable = true);

        ///// <summary>
        ///// 委托属性
        ///// </summary>
        //public MenueEnableControl MenueEnableMethod { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataSummaryExportPanel()
        {
            InitializeComponent();
            DataContext = this;
            Summary = new ConcordSummary();
            virtualType = eVirtualType.Land;
            ExportDataSummaryTableTypes = new ExportDataSummarySelectTable();
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出数据
        /// </summary>       
        private void ExportAllSelectData(object sender, RoutedEventArgs e)
        {
            //导出excel表业务类型，默认为承包方调查表         
            if (CurrentZone == null)
            {
                ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = "提示",
                    Message = "当前未选择地域，请重新选择！",
                    MessageGrade = eMessageGrade.Infomation,
                    CancelButtonText = "取消",
                    ConfirmButtonText = "确定"
                });
                return;
            }
            //批量导出
            if (currentZone.Level > eZoneLevel.Town)
            {
                ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = "提示",
                    Message = "不能超过镇级地域，请重新选择！",
                    MessageGrade = eMessageGrade.Infomation,
                    CancelButtonText = "取消",
                    ConfirmButtonText = "确定"
                });
                return;
            }

            if (!ExportDataSummaryTableTypes.ExportLandSurvyTable
                && !ExportDataSummaryTableTypes.ExportPublicDataWord
                && !ExportDataSummaryTableTypes.ExportRegeditBook
                && !ExportDataSummaryTableTypes.ExportAttorney
                && !ExportDataSummaryTableTypes.ExportSenderSurvyTable
                && !ExportDataSummaryTableTypes.ExportConcord
               && !ExportDataSummaryTableTypes.ExportSurvyInfoPublishTable
               && !ExportDataSummaryTableTypes.ExportVPSurvyTable

                && !ExportDataSummaryTableTypes.ExportDataDJBZSPTableTable
               && !ExportDataSummaryTableTypes.ExportDJBZSQSTable
               && !ExportDataSummaryTableTypes.ExportStatement
               && !ExportDataSummaryTableTypes.ExportLandParcel
               )
            {
                ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = "提示",
                    Message = "没有选择导出表类型，请重新选择！",
                    MessageGrade = eMessageGrade.Infomation,
                    CancelButtonText = "取消",
                    ConfirmButtonText = "确定"
                });
                return;
            }

            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的   
            if (currentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village)
            {
                ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, "导出汇总数据路径选择");
                extPage.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                    {
                        return;
                    }

                    if (ExportDataSummaryTableTypes.ExportSurvyInfoPublishTable)
                    {
                        ContractAccountDateSetting datePage = new ContractAccountDateSetting();
                        datePage.Workpage = ThePage; 
                        ThePage.Page.ShowMessageBox(datePage, (b1, r1) =>
                        {
                            if (!(bool)b1)
                            {
                                return;
                            }
                            PublishDateSetting = datePage.DateTimeSetting;  //设置日期等信息
                            TaskGroupExportDataSummary("批量导出汇总数据", "导出", extPage.FileName);
                        });
                    }
                    else
                    {
                        PublishDateSetting = new DateSetting();
                        TaskGroupExportDataSummary("批量导出汇总数据", "导出", extPage.FileName);
                    }
                });
            }
            else if (currentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                List<VirtualPerson> currentZoneVPS = CreateVirtualPersonCollection();
                if (currentZoneVPS.Count == 0)
                {
                    ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                    {
                        Header = "提示",
                        Message = "当前地域下无承包方数据！",
                        MessageGrade = eMessageGrade.Infomation,
                        CancelButtonText = "取消",
                        ConfirmButtonText = "确定"
                    });
                    return;
                }
                DataSummarySelectPersonPage selectPersonPage = new DataSummarySelectPersonPage();
                selectPersonPage.Workpage = ThePage;
                selectPersonPage.FamilyCollection = currentZoneVPS;
                ThePage.Page.ShowMessageBox(selectPersonPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, "导出汇总数据路径选择");
                    extPage.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(extPage, (bb, rr) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || bb == false)
                        {
                            return;
                        }
                        if (ExportDataSummaryTableTypes.ExportSurvyInfoPublishTable)
                        {
                            ContractAccountDateSetting datePage = new ContractAccountDateSetting();
                            datePage.Workpage = ThePage;
                            ThePage.Page.ShowMessageBox(datePage, (b1, r1) =>
                            {
                                if (!(bool)b1)
                                {
                                    return;
                                }
                                PublishDateSetting = datePage.DateTimeSetting;  //设置日期等信息
                                TaskExportDataSummary("导出汇总数据", "导出", extPage.FileName, selectPersonPage.SelectPersonCollection);
                            });
                        }
                        else {
                            PublishDateSetting = new DateSetting();
                        TaskExportDataSummary("导出汇总数据", "导出", extPage.FileName, selectPersonPage.SelectPersonCollection);
                        }
                    });
                });
            }
        }

        /// <summary>
        /// 单个组导出或单个组下多个人导出
        /// </summary>       
        private void TaskExportDataSummary(string taskDes, string taskName, string filePath = "", List<VirtualPerson> listPerson = null)
        {

            DateTime? date = DateTime.Now;
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskDataSummaryExportArgument meta = new TaskDataSummaryExportArgument();
            meta.FileName = filePath;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractors = listPerson;
            meta.ExportDataSummaryTableTypes = ExportDataSummaryTableTypes;
            #region 设置
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.FamilyOutputSet = FamilyOutputSet;
            meta.PublishDateSetting = PublishDateSetting;
            meta.ContractLandOutputSurveyDefine = ContractAccountDefine;
            meta.SettingDefine = ContractSettingDefine;
            meta.SingleFamilySurveySetting = SingleFamilySurveySetting;

            meta.ConcordSettingDefine = ConcordSettingDefine;
            meta.ExtendUseExcelDefine = ContractRegeditBookExtendUseExcelDefine;

            //序列化过后，如果序列化后获取的是0，默认的数字设定
            ContractRegeditBookPreviewSetInfo xmlSet = ContractRegeditBookExtend.DeserializeSelectedSetInfo();
            if (xmlSet.CRBLandCount == 0)
            {
                meta.BookLandNum = 45;
            }
            else
            {
                meta.BookLandNum = xmlSet.CRBLandCount;
            };

            if (xmlSet.CRBSharePersonCount == 0)
            {
                meta.BookPersonNum = 8;
            }
            else
            {
                meta.BookPersonNum = xmlSet.CRBSharePersonCount;
            };
            meta.DictList = DictList;
            meta.SystemSet = SystemSet;
            #endregion

            TaskDataSummaryExportOperation import = new TaskDataSummaryExportOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {

            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导出
        /// </summary>       
        private void TaskGroupExportDataSummary(string taskDes, string taskName, string filePath = "")
        {
            DateTime? date = DateTime.Now;
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupDataSummaryExportArgument meta = new TaskGroupDataSummaryExportArgument();
            meta.FileName = filePath;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.ExportDataSummaryTableTypes = ExportDataSummaryTableTypes;
            #region 设置
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.FamilyOutputSet = FamilyOutputSet;
            meta.PublishDateSetting = PublishDateSetting;
            meta.ContractLandOutputSurveyDefine = ContractAccountDefine;
            meta.SettingDefine = ContractSettingDefine;
            meta.SingleFamilySurveySetting = SingleFamilySurveySetting;

            meta.ConcordSettingDefine = ConcordSettingDefine;
            meta.ExtendUseExcelDefine = ContractRegeditBookExtendUseExcelDefine;

            //序列化过后，如果序列化后获取的是0，默认的数字设定
            ContractRegeditBookPreviewSetInfo xmlSet = ContractRegeditBookExtend.DeserializeSelectedSetInfo();
            if (xmlSet.CRBLandCount == 0)
            {
                meta.BookLandNum = 45;
            }
            else
            {
                meta.BookLandNum = xmlSet.CRBLandCount;
            };

            if (xmlSet.CRBSharePersonCount == 0)
            {
                meta.BookPersonNum = 8;
            }
            else
            {
                meta.BookPersonNum = xmlSet.CRBSharePersonCount;
            };
            meta.DictList = DictList;
            meta.SystemSet = SystemSet;

            TaskGroupDataSummaryExportOperation import = new TaskGroupDataSummaryExportOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {

            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();

        }

        #region Methods - Private

        /// <summary>
        /// 创建承包方集合
        /// </summary>  
        private List<VirtualPerson> CreateVirtualPersonCollection()
        {
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            List<VirtualPerson> persons = personStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            List<VirtualPerson> vps = new List<VirtualPerson>();
            var orderdVps = persons.OrderBy(vp =>
            {
                //排序
                int num = 0;
                Int32.TryParse(vp.FamilyNumber, out num);
                return num;
            });
            foreach (VirtualPerson vp in orderdVps)
            {
                vps.Add(vp);
            }
            //vps.RemoveAll(c => c.Name.Contains("集体"));  //排除集体户
            return vps;
        }

        /// <summary>
        /// 全选按钮
        /// </summary>       
        private void cbSelectAllTable_Checked(object sender, RoutedEventArgs e)
        {
            if (cbSelectAllTable.IsChecked.Value)
            {
                cbAttorneyTable.IsChecked = true;
                cbSenderSurvyTable.IsChecked = true;
                cbLandSurvyTable.IsChecked = true;
                cbSurvyInfoPublishTable.IsChecked = true;
                cbPublicDataWord.IsChecked = true;
                cbRegeditBook.IsChecked = true;
                cbVPSurvyTable.IsChecked = true;
                cbConcordTable.IsChecked = true;
                cbDataDJBZSPTable.IsChecked = true;
                cbLandParcel.IsChecked = true;
                cbDJBZSQSTable.IsChecked = true;
                cbStatementTable.IsChecked = true;
            }
            else
            {
                cbAttorneyTable.IsChecked = false;
                cbSenderSurvyTable.IsChecked = false;
                cbLandSurvyTable.IsChecked = false;
                cbSurvyInfoPublishTable.IsChecked = false;
                cbPublicDataWord.IsChecked = false;
                cbRegeditBook.IsChecked = false;
                cbVPSurvyTable.IsChecked = false;
                cbConcordTable.IsChecked = false;
                cbLandParcel.IsChecked = false;
                cbDataDJBZSPTable.IsChecked = false;
                cbDJBZSQSTable.IsChecked = false;
                cbStatementTable .IsChecked = false;

            }
        }

        #endregion

        #endregion

        #endregion
    }
}
