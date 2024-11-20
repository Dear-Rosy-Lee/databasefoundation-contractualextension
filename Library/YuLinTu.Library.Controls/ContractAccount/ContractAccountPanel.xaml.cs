/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

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
using NetTopologySuite.Triangulate;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包台账地块管理界面
    /// </summary>
    public partial class ContractAccountPanel : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private string zoneFullName;   //地域名称
        private TaskQueue queueQuery;  //获取数据
        private TaskQueue queueFilter;  //过滤数据
        private TaskQueue queueClear;  //清空数据
        private Zone currentZone;     //当前地域
        private eVirtualType virtualType;
        private bool allCheck = false;
        private bool isSelected = false;   //用于标记界面上是否有选中项信息(默认为没选中)
        private bool isbatch = false;  //是否批量
        private SeekLandNeighborSetting seekLandNeighborSet;//用于记录弹出查找四至的选择条件
        private bool isBatch;//是否批量
        private bool _isStockLand = true;
        private object lockobj = new object();

        /// <summary>
        /// 当前选择承包方绑定实体(界面实体)
        /// </summary>
        private ContractLandPersonItem currentAccountItem;

        /// <summary>
        /// 当前选择承包地绑定实体(界面实体)
        /// </summary>
        private ContractLandBinding currentLandBinding;

        /// <summary>
        /// 当前选择地域下的所有承包方集合
        /// </summary>
        private List<VirtualPerson> currentPersonList = new List<VirtualPerson>();

        /// <summary>
        /// 当前地域下的所有地块集合
        /// </summary>
        private List<ContractLand> landList = new List<ContractLand>();

        /// <summary>
        /// 当前地域下的所有确股地块集合
        /// </summary>
        //private List<ContractLand> landStockAll = new List<ContractLand>();

        /// <summary>
        /// 当前地域下的所有确股地块集合
        /// </summary>
        private List<BelongRelation> ralations = new List<BelongRelation>();

        /// <summary>
        /// 数据字典集合
        /// </summary>
        private List<Dictionary> dictList = new List<Dictionary>();

        /// <summary>
        /// 数据字典地块类别
        /// </summary>
        private List<Dictionary> listDKLB = new List<Dictionary>();

        /// <summary>
        /// 数据字典地力等级
        /// </summary>
        private List<Dictionary> listDLDJ = new List<Dictionary>();

        /// <summary>
        /// 承包方绑定集合
        /// </summary>
        public ObservableCollection<ContractLandPersonItem> accountLandItems = new ObservableCollection<ContractLandPersonItem>();

        private ContractBusinessSettingDefine ContractBusinessSettingDefine =
            ContractBusinessSettingDefine.GetIntence();

        private FamilyOtherDefine FamilyOtherDefine = FamilyOtherDefine.GetIntence();

        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private ServiceSetDefine ServiceSetDefine = ServiceSetDefine.GetIntence();

        /// <summary>
        /// 是否批处理
        /// </summary>
        public bool IsBatch
        {
            get
            {
                return isBatch;
            }

            set
            {
                isBatch = value;
                NotifyPropertyChanged("IsBatch");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Fields

        #region Properties

        /// <summary>
        /// 是否确股插件调用
        /// </summary>
        public bool IsStockLand
        {
            get { return _isStockLand; }
            set
            {
                _isStockLand = value;
                if (_isStockLand)
                {
                    colum_IsStock.Visibility = Visibility.Visible;
                }
            }
        }

        public SystemSetDefine SystemSet
        {
            get { return SystemSetDefine.GetIntence(); }
        }

        public ContractBusinessSettingDefine SettingDefine
        {
            get { return ContractBusinessSettingDefine.GetIntence(); }
        }

        public ContractBusinessImportSurveyDefine ContractLandImportSurveyDefine
        { get { return ContractBusinessImportSurveyDefine.GetIntence(); } }

        /// <summary>
        /// 地块示意图配置
        /// </summary>
        public ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                DataContext = this;
                InitialControl(currentZone == null ? "" : currentZone.FullCode);
                if (currentZone != null)
                {
                    //InitialControl(currentZone.FullCode);
                    currentAccountItem = null;
                    currentLandBinding = null;
                }
            }
        }

        /// <summary>
        /// 当前选择承包方绑定实体(界面实体)
        /// </summary>
        public ContractLandPersonItem CurrentAccountItem
        {
            get { return currentAccountItem; }
            set { currentAccountItem = value; }
        }

        /// <summary>
        /// 当前选择承包地绑定实体(界面实体)
        /// </summary>
        public ContractLandBinding CurrentLandBinding
        {
            get { return currentLandBinding; }
            set { currentLandBinding = value; }
        }

        /// <summary>
        /// 承包方数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 承包台账地块数据处理业务
        /// </summary>
        public AccountLandBusiness ContractAccountBusiness { get; set; }

        /// <summary>
        /// 数据字典处理业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 台账数据统计
        /// </summary>
        public ContractAccountSummary AccountSummary { get; private set; }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; PersonBusiness.VirtualType = value; }
        }

        /// <summary>
        /// 承包台账工作页
        /// </summary>
        public IWorkpage TheWorkPage { get; set; }

        /// <summary>
        /// 标记属性(用于标记界面上是否选中项信息)
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        /// <summary>
        /// 承包方导出设置
        /// 是否批量
        /// </summary>
        //public bool IsBatch
        //{
        //    get { return isbatch; }
        //    set { isbatch = value; }
        //}

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting
        {
            get;
            set;
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

        #endregion Properties

        #region Delegate

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenueEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenueEnableControl MenueEnableMethod { get; set; }

        #endregion Delegate

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractAccountPanel()
        {
            InitializeComponent();
            AccountSummary = new ContractAccountSummary();
            virtualType = eVirtualType.Land;
            DataContext = this;
            view.Roots = accountLandItems;      //绑定数据源
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
            queueClear = new TaskQueueDispatcher(Dispatcher);
        }

        #endregion Ctor

        #region Methods

        #region 获取数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitialControl(string zoneCode)
        {
            if (AccountSummary != null)
            {
                AccountSummary.EmptyData();
            }
            queueQuery.Cancel();
            queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    MenueEnableMethod(false);
                    DoWork(go);
                },
                completed =>
                {
                    MenueEnableMethod();
                    view.Filter(obj => { return true; }, true);
                    DataCount();
                    view.IsEnabled = true;
                },
                terminated =>
                {
                    MenueEnableMethod();
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    view.IsEnabled = false;
                    ca_zoneFullName.Text = "";
                    TheWorkPage.Page.IsBusy = true;
                    accountLandItems.Clear();
                }, ended =>
                {
                    TheWorkPage.Page.IsBusy = false;
                }, null, null, null);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public void DoWork(TaskGoEventArgs arg = null)
        {
            if (PersonBusiness == null)
            {
                return;
            }
            //获得地域的名称
            if (currentZone == null) return;
            ralations.Clear();
            zoneFullName = PersonBusiness.GetUinitName(currentZone);
            if (arg != null)
                arg.Instance.ReportProgress(1, zoneFullName);
            string zoneCode = currentZone.FullCode;
            currentPersonList = PersonBusiness.GetByZone(zoneCode);
            landList = ContractAccountBusiness.GetCollection(zoneCode, eLevelOption.Self);
            if (IsStockLand)
            {
                //landStockAll = DataBaseSource.GetDataBaseSource().CreateContractLandWorkstation().Get(o => o.ZoneCode == CurrentZone.FullCode);
                ralations = DataBaseSource.GetDataBaseSource().CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByZone(CurrentZone == null ? "" : CurrentZone.FullCode, eLevelOption.Self);
            }
            dictList = DictBusiness.GetAll();
            listDKLB = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB && !string.IsNullOrEmpty(c.Code));
            listDLDJ = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ && !string.IsNullOrEmpty(c.Code));

            if (ContractBusinessSettingDefine.DisplayCollectUsingCBdata)
            {
                //根据配置决定是否在主界面上显示集体户信息
                if (currentPersonList != null)
                {
                    currentPersonList.RemoveAll(c => c.Name == "集体" || c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }
            }

            if (currentPersonList != null && currentPersonList.Count > 0 && landList != null)
            {
                ChangeDataAndSend(arg);
            }
        }

        private void ChangeDataAndSend(TaskGoEventArgs arg)
        {
            if (arg == null)
            {
                foreach (var item in currentPersonList)
                {
                    var svpi = item.ConvertItem(landList, listDKLB, listDLDJ, IsStockLand, ralations);
                    if (svpi != null && FamilyOtherDefine.ShowFamilyInfomation && svpi.Name.Equals("集体"))
                    {
                        continue;
                    }
                    else
                    {
                        if (svpi.Children.Count > 0)
                            accountLandItems.Add(svpi);
                    }
                }
            }
            else
            {
                foreach (var item in currentPersonList)
                {
                    if (arg.Instance.IsStopPending)
                        break;
                    var svpi = item.ConvertItem(landList, listDKLB, listDLDJ, IsStockLand, ralations);
                    arg.Instance.ReportProgress(50, svpi);
                }
            }
        }

        private int count = 0;

        /// <summary>
        /// 获取数据完成
        /// </summary>
        private void Changed(TaskProgressChangedEventArgs arg)
        {
            ca_zoneFullName.Text = zoneFullName;
            ContractLandPersonItem item = arg.UserState as ContractLandPersonItem;
            if (item != null && FamilyOtherDefine.ShowFamilyInfomation && item.Tag.Name.Equals("集体"))
                return;
            else if (item != null && item.Children.Count > 0)
            {
                accountLandItems.Add(item);
                DataCount(item);
            }
            if (item != null)
            {
                count += item.Children.Count;
            }
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        private void DataCount()
        {
            int familyCount = 0;
            int landCount = 0;
            double summaryTableArea = 0;
            double summaryActualArea = 0;
            double summaryAwareArea = 0;
            double? summrayContractDelayArea = 0;
            int i = -1;

            foreach (var item in accountLandItems)
            {
                double virtualPersonActualArea = 0;
                double virtualPersonTableArea = 0;
                double virtualPersonAwareArea = 0;
                double virtualPersonContractDelayArea = 0;
                i++;
                if (!view.IsItemVisible(item))
                {
                    continue;
                }
                familyCount++;
                foreach (var land in item.Children)
                {
                    if (!view.IsItemVisible(land))
                        continue;
                    if (land.Visibility != Visibility.Visible || land.Tag == null)
                    {
                        continue;
                    }
                    if (!land.IsStockLand)
                    {
                        landCount++;
                        double tableArea = land.Tag.TableArea == null ? 0 : land.Tag.TableArea.Value;
                        double contractDelayArea = land.Tag.ContractDelayArea;
                        summaryTableArea += tableArea;
                        summaryActualArea += land.Tag.ActualArea;
                        summaryAwareArea += land.Tag.AwareArea;
                        summrayContractDelayArea += contractDelayArea;
                        virtualPersonActualArea += land.Tag.ActualArea;
                        virtualPersonTableArea += tableArea;
                        virtualPersonAwareArea += land.Tag.AwareArea;
                        virtualPersonContractDelayArea += contractDelayArea;
                    }
                }
                item.ActualAreaUI = virtualPersonActualArea.AreaFormat(2);
                item.AwareAreaUI = virtualPersonAwareArea.AreaFormat(2);
                item.TableAreaUI = virtualPersonTableArea.AreaFormat(2);
                item.ContractDelayAreaUI = virtualPersonContractDelayArea.AreaFormat(2);
            }

            //加上确股地块统计信息
            if (IsStockLand)
            {
                if (CurrentZone != null)
                {
                    var landStock = new List<ContractLand>();
                    landList.ForEach(l =>
                    {
                        if (ralations.Any(r => r.LandID == l.ID))
                        {
                            landStock.Add(l);
                        }
                    });
                    if (landStock.Count > 0)
                    {
                        landCount = landCount + landStock.Count;
                        summaryTableArea = summaryTableArea + Convert.ToDouble(landStock.Sum(o => (o.TableArea == null ? 0d : o.TableArea)));
                        summaryActualArea = summaryActualArea + Convert.ToDouble(landStock.Sum(o => o.ActualArea));
                        summaryAwareArea = summaryAwareArea + Convert.ToDouble(landStock.Sum(o => o.AwareArea));
                        summrayContractDelayArea = summrayContractDelayArea + Convert.ToDouble(landStock.Sum(o => o.ContractDelayArea));
                    }
                }
            }

            AccountSummary.FamilyCount = familyCount;
            AccountSummary.LandCount = landCount;
            AccountSummary.TableAreaCount = summaryTableArea.AreaFormat(2);
            AccountSummary.ActualAreaCount = summaryActualArea.AreaFormat(2);
            AccountSummary.ArwareAreaCount = summaryAwareArea.AreaFormat(2);
            AccountSummary.ContractDelayAreaCount = summrayContractDelayArea.AreaFormat(2);
        }

        private void DataCount(ContractLandPersonItem item)
        {
            double summaryTableArea = 0;
            double summaryActualArea = 0;
            double summaryAwareArea = 0;
            double? summrayContractDelayArea = 0;

            foreach (var land in item.Children)
            {
                if (!view.IsItemVisible(land))
                    continue;
                if (land.Visibility != Visibility.Visible || land.Tag == null)
                {
                    continue;
                }
                if (!land.IsStockLand)
                {
                    double tableArea = land.Tag.TableArea == null ? 0 : land.Tag.TableArea.Value;
                    double contractDelayArea = land.Tag.ContractDelayArea;
                    summaryTableArea += tableArea;
                    summaryActualArea += land.Tag.ActualArea;
                    summaryAwareArea += land.Tag.AwareArea;
                    summrayContractDelayArea += contractDelayArea;
                }
            }

            //加上确股地块统计信息
            if (IsStockLand)
            {
                if (CurrentZone != null)
                {
                    var landStock = new List<ContractLand>();
                    landList.ForEach(l =>
                    {
                        if (ralations.Any(r => r.LandID == l.ID))
                        {
                            landStock.Add(l);
                        }
                    });
                    if (landStock.Count > 0)
                    {
                        summaryTableArea = summaryTableArea + Convert.ToDouble(landStock.Sum(o => (o.TableArea == null ? 0d : o.TableArea)));
                        summaryActualArea = summaryActualArea + Convert.ToDouble(landStock.Sum(o => o.ActualArea));
                        summaryAwareArea = summaryAwareArea + Convert.ToDouble(landStock.Sum(o => o.AwareArea));
                        summrayContractDelayArea = summrayContractDelayArea + Convert.ToDouble(landStock.Sum(o => o.ContractDelayArea));
                    }
                }
            }
            AccountSummary.FamilyCount++;
            AccountSummary.LandCount += item.Children.Count;
            AccountSummary.TableAreaCount = (ConvertDouble(AccountSummary.TableAreaCount) + summaryTableArea) + "";
            AccountSummary.ActualAreaCount = (ConvertDouble(AccountSummary.ActualAreaCount) + summaryActualArea) + "";
            AccountSummary.ArwareAreaCount = (ConvertDouble(AccountSummary.ArwareAreaCount) + summaryAwareArea) + "";
            AccountSummary.ContractDelayAreaCount = (ConvertDouble(AccountSummary.ContractDelayAreaCount) + summrayContractDelayArea) + "";
        }

        private double ConvertDouble(string area)
        {
            if (string.IsNullOrEmpty(area.Trim()))
                return 0;
            return Convert.ToDouble(AccountSummary.TableAreaCount);
        }

        #endregion 获取数据

        #region 右键菜单

        /// <summary>
        /// 添加地块
        /// </summary>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            ContractLandAdd();
        }

        /// <summary>
        /// 编辑地块
        /// </summary>
        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            ContractLandEdit();
        }

        /// <summary>
        /// 删除地块
        /// </summary>
        private void miDel_Click(object sender, RoutedEventArgs e)
        {
            ContractLandDel();
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void miClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 编码详情
        /// </summary>
        private void miNumberDetail_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.LandNumberDetail, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (landList == null || landList.Count == 0)
            {
                //当前地域下没有地块信息
                ShowBox(ContractAccountInfo.LandNumberDetail, ContractAccountInfo.CurrentZoneNoLand);
                return;
            }
            long landNumberMax = 0;  //最大编码
            long landNumberMin = 0;  //最小编码
            List<long> landNumbers = new List<long>();   //当前地域下的地块编码集合
            List<int> landNumbersMiss = new List<int>();   //缺失编码
            foreach (var land in landList)
            {
                long result = 0;
                bool isSuccess = long.TryParse(land.LandNumber.GetLastString(5), out result);
                if (isSuccess)
                {
                    landNumbers.Add(result);
                }
            }
            if (landNumbers.Count == 0)
            {
                ShowBox(ContractAccountInfo.LandNumberDetail, ContractAccountInfo.CurrentZoneLandNoSurveyNumber);
                return;
            }
            landNumberMin = landNumbers.Min<long>();
            landNumberMax = landNumbers.Max<long>();
            for (long num = landNumberMin; num <= landNumberMax; num++)
            {
                if (!landNumbers.Contains(num))
                {
                    landNumbersMiss.Add((int)num);
                }
            }
            LandNumberDetail page = new LandNumberDetail((int)landNumberMin, (int)landNumberMax, landNumbersMiss);
            TheWorkPage.Page.ShowMessageBox(page);
        }

        /// <summary>
        /// 空间查看
        /// </summary>
        private void miCheckSpace_Click(object sender, RoutedEventArgs e)
        {
            List<ContractLand> lands = FindGeometry();
            TheWorkPage.Message.Send(this, MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_GETMAPPAGE, lands));
        }

        /// <summary>
        /// 地块示意图
        /// </summary>
        private void miSingle_Click(object sender, RoutedEventArgs e)
        {
            MultiParcelExport();
        }

        ///// <summary>
        ///// 地块示意图
        ///// </summary>
        //private void miDouble_Click(object sender, RoutedEventArgs e)
        //{
        //}

        /// <summary>
        /// 初始数据
        /// </summary>
        private void miInitialData_Click(object sender, RoutedEventArgs e)
        {
            InitialLandInfo();
        }

        /// <summary>
        /// 检索数据之地块名称空值
        /// </summary>
        private void miSearchLandNameNull_Click(object sender, RoutedEventArgs e)
        {
            LandNameNullSearch();
        }

        /// <summary>
        /// 检索数据之二轮合同面积空值
        /// </summary>
        private void miSearchTableAreaNull_Click(object sender, RoutedEventArgs e)
        {
            ContractAreaNullSearch();
        }

        /// <summary>
        /// 检索数据之实测面积空值
        /// </summary>
        private void miSearchActualAreaNull_Click(object sender, RoutedEventArgs e)
        {
            ActualAreaNullSearch();
        }

        /// <summary>
        /// 检索数据之确权面积空值
        /// </summary>
        private void miSearchAwareAreaNull_Click(object sender, RoutedEventArgs e)
        {
            AwareAreaNullSearch();
        }

        /// <summary>
        /// 检索数据之是否基本农田空值
        /// </summary>
        private void miSearchIsFarmerNull_Click(object sender, RoutedEventArgs e)
        {
            FarmerLandNullSearch();
        }

        /// <summary>
        /// 检索数据之地力等级空值
        /// </summary>
        private void miSearchLandLevelNull_Click(object sender, RoutedEventArgs e)
        {
            LandLevelNullSearch();
        }

        #endregion 右键菜单

        #region Methods - Private

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="TableType">表格类型(适用于台账报表中4个公用底层的表格)</param>
        /// <param name="listPerson">承包方集合</param>
        public void ExportDataCommonOperate(string zoneName, string header, eContractAccountType type, string taskDes, string taskName,
           int TableType = 1, List<VirtualPerson> listPerson = null, bool? isStockLand = false, List<ContractLand> vcontractLands = null)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, TheWorkPage, header);
            extPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                string saveFilePath = extPage.FileName;
                if (string.IsNullOrEmpty(saveFilePath) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractAccountType.ExportSurveyTable:
                        ExportPublishWordTask(saveFilePath, taskDes, taskName, listPerson);
                        break;

                    case eContractAccountType.VolumnExportSurveyTable:
                        ExportPublishWordTaskGroup(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.ExportLandSurveyWord:
                        ExportLandWordTask(saveFilePath, taskDes, taskName, listPerson);
                        break;

                    case eContractAccountType.VolumnExportLandSurveyTable:
                        ExportLandWordTaskGroup(saveFilePath, taskDes, taskName);
                        break;
                    //case eContractAccountType.ExportPublishTable:    //调查信息公示Excel
                    //    ExportPublishExcelTask(saveFilePath, taskDes, taskName);
                    //    break;
                    case eContractAccountType.VolumnExportPublishTable:    //批量导出调查信息公示Excel
                        ExportPublishExcelTaskGroup(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.VolumnExportLandVerifyExcel:
                        ExportVerifyExcelTaskGroup(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.VolumnExportLandVerifyPrintExcel:
                        ExportVerifyExcelPrintTaskGroup(saveFilePath, taskDes, taskName);
                        break;
                    //case eContractAccountType.ExportSendTableWord:
                    //    ExportSenderWordTask(saveFilePath, taskDes, taskName);
                    //    break;
                    case eContractAccountType.VolumnExportSenderTableWord:
                        ExportSenderWordTaskGroup(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.ExportContractorTable:
                        ExportVPWordTask(saveFilePath, taskDes, taskName, listPerson);
                        break;

                    case eContractAccountType.VolumnExportContractorTable:
                        ExportVPWordTaskGroup(saveFilePath, taskDes, taskName);
                        break;
                    //case eContractAccountType.ExportSummaryExcel:   //导出数据汇总表
                    //    TaskExportSummaryExcel(eContractAccountType.ExportSummaryExcel, taskDes, taskName, saveFilePath);
                    //    break;
                    case eContractAccountType.VolumnExportSummaryExcel:   //批量导出数据汇总表
                        TaskGroupExportSummaryExcel(eContractAccountType.ExportSummaryExcel, taskDes, taskName, saveFilePath);
                        break;

                    case eContractAccountType.ExportSingleFamilySurveyExcel:  //单户调查表-excel
                        TaskExportSingleFamilySurveyExcel(type, taskDes, taskName, saveFilePath, listPerson);
                        break;
                    //case eContractAccountType.ExportVirtualPersonExcel: //导出承包方调查表Excel
                    //    ExportVPExcelTask(saveFilePath, taskDes, taskName);
                    //    break;
                    case eContractAccountType.VolumnExportSingleFamilySurveyExcel:  //单户调查表-excel
                        TaskGroupExportSingleFamilySurveyExcel(eContractAccountType.ExportSingleFamilySurveyExcel, taskDes, taskName, saveFilePath);
                        break;

                    case eContractAccountType.VolumnExportVirtualPersonExcel:  //批量承包方调查表Excel
                        ExportVPExcelTaskGroup(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.ExportMultiParcelOfFamily: //地块示意图
                        ExportMultiParcelTask(saveFilePath, listPerson, taskDes, taskName, isStockLand, vcontractLands);
                        break;

                    case eContractAccountType.VolumnExportMultiParcelOfFamily:  //批量导出地块示意图
                        ExportMultiParcelTaskGroup(saveFilePath, taskName, taskDes, isStockLand);
                        break;
                    //case eContractAccountType.ExportContractAccountExcel:
                    //    TaskExportContractAccountExcel(type, taskDes, taskName, saveFilePath, null, TableType);
                    //    break;
                    case eContractAccountType.ExportSingleFamilyConfirmExcel:
                        TaskExportContractAccountExcel(type, null, null, taskDes, taskName, saveFilePath, listPerson, TableType);
                        break;

                    case eContractAccountType.ExportSendTableExcel:   //发包方调查表Excel
                        ExportSenderExcelTask(saveFilePath, taskDes, taskName);
                        break;

                    case eContractAccountType.VolumnExportContractAccountExcel:
                        TaskGroupExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, taskDes, taskName, saveFilePath, TableType);
                        break;

                    case eContractAccountType.VolumnExportSingleFamilyConfirmExcel:
                        TaskGroupExportContractAccountExcel(eContractAccountType.ExportSingleFamilyConfirmExcel, taskDes, taskName, saveFilePath, TableType);
                        break;

                    case eContractAccountType.ExportVillageDeclare:   //导出村组公示表
                        TaskExportVillagesDeclare(eContractAccountType.ExportVillageDeclare, taskDes, taskName, saveFilePath);
                        break;

                    case eContractAccountType.VolumnExportVillageDeclare:   //批量导出村组公示表
                        TaskGroupExportVillagesDeclare(eContractAccountType.ExportVillageDeclare, taskDes, taskName, saveFilePath);
                        break;
                    //case eContractAccountType.ExportBoundaryInfoExcel:   //导出界址信息表
                    //    TaskExportBoundaryInfoExcel(eContractAccountType.ExportBoundaryInfoExcel, taskDes, taskName, saveFilePath);
                    //    break;
                    case eContractAccountType.VolumnExportBoundaryInfoExcel:   //批量导出界址信息表
                        TaskGroupExportBoundaryInfoExcel(eContractAccountType.ExportBoundaryInfoExcel, taskDes, taskName, saveFilePath);
                        break;
                }
            });
        }

        /// <summary>
        /// 导出方法
        /// </summary>
        public void ExportDataCommonMethod(string zoneName, string header, Action<string> ActMethod)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, TheWorkPage, header);
            extPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                string saveFilePath = extPage.FileName;
                if (string.IsNullOrEmpty(saveFilePath) || b == false)
                {
                    return;
                }
                if (ActMethod != null)
                    ActMethod(saveFilePath);
            });
        }

        /// <summary>
        /// 单进度导出台账5个表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportContractAccountExcel(eContractAccountType type, DateTime? time, DateTime? pubTime, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null, int TableType = 1)
        {
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
            meta.VirtualType = virtualType;
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
            meta.DelcTime = time;
            meta.PubTime = pubTime;
            meta.IsBatch = isbatch;
            meta.DictList = DictList;

            var import = new TaskAccountFiveTableOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        private void TaskExportContractDelayAccountExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
          List<VirtualPerson> listPerson = null, int TableType = 1)
        {
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
            meta.VirtualType = virtualType;
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
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            TaskContractDelayAccountOperation import = new TaskContractDelayAccountOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 多进度导出台账5个表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportContractAccountExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
           int TableType = 1)
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupAccountFiveTableArgument meta = new TaskGroupAccountFiveTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.TableType = TableType;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            //if (TableType == 4)
            //{
            //    //如果是公示确认表，需要重新赋值底层设置实体，从公示表配置读
            //    meta.ContractAccountDefine = publicityConfirmDefine.ConvertTo<PublicityConfirmDefine>();// (PublicityConfirmDefine)publicityConfirmDefine;
            //}
            //else
            //{
            //    meta.ContractAccountDefine = ContractAccountDefine;
            //}
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            TaskGroupAccountFiveTableOperation import = new TaskGroupAccountFiveTableOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 单进度导出单户调查表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportSingleFamilySurveyExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null)
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            ExportLandSingleSurveyTableArgument meta = new ExportLandSingleSurveyTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractor = listPerson;
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            ExportLandSingleSurveyTableOperation import = new ExportLandSingleSurveyTableOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 多进度导出单户调查表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportSingleFamilySurveyExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "")
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            ExportGroupLandSingleSurveyTableArgument meta = new ExportGroupLandSingleSurveyTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;

            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            ExportGroupLandSingleSurveyTableOperation import = new ExportGroupLandSingleSurveyTableOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 单进度导出数据汇总表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportSummaryExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null)
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskExportSummaryExcelArgument meta = new TaskExportSummaryExcelArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractor = listPerson;
            meta.IsBatch = isbatch;
            meta.IsShow = true;
            meta.DictList = DictList;
            TaskExportSummaryExcelOperation import = new TaskExportSummaryExcelOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 多进度导出数据汇总表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportSummaryExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "")
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupExportSummaryExcelArgument meta = new TaskGroupExportSummaryExcelArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            TaskGroupExportSummaryExcelOperation import = new TaskGroupExportSummaryExcelOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        private void TaskExportCategorySummaryExcel()
        {
            TaskExportCategorySummaryExcelArgument meta = new TaskExportCategorySummaryExcelArgument();
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.FileName = SystemSet.DefaultPath;
            meta.UnitName = SystemSet.GetTableHeaderStr(currentZone);
            TaskExportCategorySummaryExcelOperation import = new TaskExportCategorySummaryExcelOperation();
            import.Argument = meta;
            import.Description = "导出地块类别汇总表";
            import.Name = "地块类别汇总表";

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 单进度导出村组公示表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportVillagesDeclare(eContractAccountType type, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null)
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskExportVillagesDeclareArgument meta = new TaskExportVillagesDeclareArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SelectContractor = listPerson;
            meta.PublishDateSetting = PublishDateSetting;
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            meta.IsShow = true;
            TaskExportVillagesDeclareOperation import = new TaskExportVillagesDeclareOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 多进度导出村组公示表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportVillagesDeclare(eContractAccountType type, string taskDes, string taskName, string filePath = "")
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupExportVillagesDeclareArgument meta = new TaskGroupExportVillagesDeclareArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.PublishDateSetting = PublishDateSetting;
            meta.IsBatch = isbatch;
            meta.DictList = DictList;
            TaskGroupExportVillagesDeclareOperation import = new TaskGroupExportVillagesDeclareOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperateTzbb(string zoneName, string header, eContractAccountType type, int TableType, string taskDes, string taskName,
            string messageName = "")
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, TheWorkPage, header);
            extPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractAccountType.ExportContractAccountExcel:
                        ExportCommonOperateTzbb(type, TableType, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.ExportSingleFamilyConfirmExcel:
                        ExportCommonOperateTzbb(type, TableType, taskDes, taskName, extPage.FileName, messageName);
                        break;
                }
            });
        }

        /// <summary>
        /// 设置公示表中日期
        /// </summary>
        /// <returns></returns>
        private DateTime? SetPublicyTableDate()
        {
            DateTime date = DateTime.Now;
            //if (setDate)
            //{
            //    LandDateSettingForm export = new LandDateSettingForm();
            //    if (export.ShowDialog() != DialogResult.OK)
            //    {
            //        return null;
            //    }
            //    date = export.dtpCheckDate.Value;
            //}
            return date;
        }

        /// <summary>
        /// 导出台账报表4个文件操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperateTzbb(eContractAccountType type, int TableType, string taskDes, string taskName, string filePath = "",
            string messageName = "", DateTime? time = null, DateTime? pubTime = null)
        {
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
        }

        /// <summary>
        /// 导出台账报表-单户确认表单选多选操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportSingleFamilyConfirmExcel(eContractAccountType type, List<VirtualPerson> selectPerson, int TableType, string taskDes,
            string taskName, string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null)
        {
            IDbContext dbContext = CreateDb();
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            TaskContractAccountArgument meta = new TaskContractAccountArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.TableType = TableType;
            meta.SelectContractor = selectPerson;

            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.IsBatch = isbatch;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导出台账报表-单户调查表单选多选操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportSingleFamilySurveyExcel(eContractAccountType type, List<VirtualPerson> selectPerson, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null)
        {
            IDbContext dbContext = CreateDb();
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            TaskContractAccountArgument meta = new TaskContractAccountArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.UserName = "";
            meta.Date = date;
            meta.SelectContractor = selectPerson;

            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.IsBatch = isbatch;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框标题</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(string zoneName, string header, eContractAccountType type, string taskDes, string taskName,
            string messageName = "", List<VirtualPerson> listPerson = null)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, TheWorkPage, header);
            extPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractAccountType.VolumnExportSurveyTable:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractAccountType.VolumnExportLandSurveyTable:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractAccountType.VolumnExportPublishTable:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.VolumnExportContractorTable:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractAccountType.ExportSummaryExcel:   //导出数据汇总表
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.ExportVillageDeclare:  //导出村组公示公告Word
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.ExportSingleFamilySurveyExcel:  //单户调查表-excel
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.VolumnExportVirtualPersonExcel://承包方调查表
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractAccountType.ExportMultiParcelOfFamily: //地块示意图
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;
                }
            });
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(eContractAccountType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            IDbContext dbContext = CreateDb();
            TaskContractAccountArgument meta = new TaskContractAccountArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = dbContext;
            meta.SelectContractor = listPerson;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.PublishDateSetting = this.PublishDateSetting;
            import.SelectedPersons = listPerson;
            import.IsBatch = isbatch;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 对公告公示Word设置时间等信息
        /// </summary>
        private void SetDateInfoForVillagesDeclare()
        {
            PublishDateSetting = null;
            AnnouncementDateSetting datePage = new AnnouncementDateSetting();
            datePage.Workpage = TheWorkPage;
            datePage.LandBusiness = ContractAccountBusiness;
            datePage.dbContext = DbContext;
            datePage.CurrentZone = CurrentZone;
            TheWorkPage.Page.ShowMessageBox(datePage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                PublishDateSetting = datePage.DateSettingForAnnouncementWord;  //设置日期等信息
                List<Zone> SelfAndSubsZones = new List<Zone>();
                if (PublishDateSetting.PublishEndDate != null &&
                PublishDateSetting.PublishStartDate != null &&
                PublishDateSetting.PublishEndDate.Value <
                PublishDateSetting.PublishStartDate.Value)
                {
                    ShowBox("公示公告信息设置", "截止日期不能小于公告日期");
                    return;
                }
                var zoneStation = DbContext.CreateZoneWorkStation();
                int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
                {
                    TaskExportVillagesDeclare(eContractAccountType.ExportVillageDeclare, ContractAccountInfo.ExportVillagesDeclare, ContractAccountInfo.ExportTable, SystemSet.DefaultPath);
                    //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportVillageDeclare, ContractAccountInfo.ExportVillagesDeclare, ContractAccountInfo.ExportTable, 1, null);
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
                {
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportVillageDeclare, ContractAccountInfo.ExportVillagesDeclare, ContractAccountInfo.ExportTable, 1, null);
                }
                else
                {
                    ShowBox(ContractAccountInfo.ExportVillagesDeclare, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            });
        }

        #endregion Methods - Private

        #region Methods -Events

        /// <summary>
        /// 鼠标双击  双击显示的数据
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (TheWorkPage.Page.IsBusy)
            //    return;
            GetSelectItem();
            if (CurrentLandBinding == null)
            {
                view.ExpandItemWhenLeftMouseDoubleClick = true;
                return;
            }
            else
            {
                ContractLandEdit();
            }
        }

        /// <summary>
        /// 是否有子节点
        /// </summary>
        private void view_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
        }

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectItem();
        }

        /// <summary>
        /// 鼠标右键按下
        /// </summary>
        private void view_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetSelectItem();
        }

        /// <summary>
        /// 刷新按钮事件
        /// </summary>
        private void btn_refresh_Click_1(object sender, RoutedEventArgs e)
        {
            txtWhere.Text = string.Empty;
            Refresh();
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        private void caIsbatch_Click(object sender, RoutedEventArgs e)
        {
            IsBatch = (bool)caIsbatch.IsChecked;
        }

        #endregion Methods -Events

        #region Methods -Publics

        /// <summary>
        /// 刷新统计及界面
        /// </summary>
        public void Refresh()
        {
            var a = DbContext.CreateBelongRelationWorkStation();
            currentAccountItem = null;
            if (currentZone == null)
            {
                return;
            }
            InitialControl(currentZone.FullCode);
        }

        #endregion Methods -Publics

        #region Method-过滤数据

        /// <summary>
        /// 快速过滤
        /// </summary>
        private void txtWhere_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string whName = txtWhere.Text;
            if (whName.Length == 0)
            {
                DataFilter("");
            }
            else
            {
                DataFilter(whName.Trim());
            }
        }

        //private string lastWHString = null;
        /// <summary>
        /// 数据过滤
        /// </summary>
        private void DataFilter(string whName)
        {
            //if (lastWHString == whName)
            //    return;

            //lastWHString = whName;

            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        SetItemVisible(go);
                    }));
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                }, null, null, null, null, whName);
        }

        /// <summary>
        /// 设置数据可见性
        /// </summary>
        private void SetItemVisible(TaskGoEventArgs arg)
        {
            string whString = arg.Instance.Argument.UserState.ToString();

            view.Filter(obj =>
            {
                if (whString.IsNullOrBlank())
                    return true;

                bool has = false;
                string txt = null;

                var dataItem = obj as ContractLandPersonItem;
                if (dataItem != null)
                {
                    txt = dataItem.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }
                var landbind = obj as ContractLandBinding;
                if (landbind != null)
                {
                    txt = landbind.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.LandNumber;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.LandName;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.LandCategoryUI;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.IsFarmerLandUI;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.LandLevelUI;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }

                return has;
            }, true);
        }

        /// <summary>
        /// 项的过滤:如果项符合条件,则全显示(true),否则不显示(false)
        /// </summary>
        private bool JudgeItem(string whString, bool allInfo, ContractLandPersonItem dataItem)
        {
            string vpname = string.IsNullOrEmpty(dataItem.Tag.Name) ? "" : dataItem.Tag.Name;
            string vpNumber = string.IsNullOrEmpty(dataItem.LandNumber) ? "" : dataItem.LandNumber;
            bool condition1 = (allInfo) && (vpname.Equals(whString) || vpNumber.Equals(whString));
            bool condition2 = (!allInfo) && (vpname.Contains(whString) || vpNumber.Contains(whString));
            if (condition1 || condition2)
            {
                dataItem.Visibility = Visibility.Visible;
                foreach (var land in dataItem.Children)
                {
                    land.Visibility = Visibility.Visible;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  全匹配
        /// </summary>
        private void ca_ComplateInfo_Click_1(object sender, RoutedEventArgs e)
        {
            string whName = txtWhere.Text.Trim();
            if (ca_ComplateInfo.IsChecked == false)
            {
                allCheck = false;
            }
            else
            {
                allCheck = true;
            }
            if (!string.IsNullOrEmpty(whName))
            {
                DataFilter(whName);
            }
        }

        #endregion Method-过滤数据

        #region Method-地块基本操作

        /// <summary>
        /// 添加承包地块
        /// </summary>
        public void ContractLandAdd()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentZone.Level != eZoneLevel.Village && CurrentZone.Level != eZoneLevel.Group)
            {
                //选择地域不是村或者组
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.ZoneSelectedErrorForAdd);
                return;
            }

            if (currentLandBinding != null && currentLandBinding.IsStockLand)
            {
                ShowBox(ContractAccountInfo.ContractLandAdd, "请不要选择确股地块添加");
                return;
            }

            if (isSelected && CurrentAccountItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.ContractLandAddLock);
                return;
            }
            ContractLandPage addPage = new ContractLandPage(true);
            addPage.Workpage = TheWorkPage;
            addPage.CurrentZone = CurrentZone;
            addPage.VirtpersonList = VirtualPersonFilter();
            addPage.IsSelected = this.IsSelected; //传递标记
            if (addPage.VirtpersonList == null || addPage.VirtpersonList.Count == 0)
            {
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.ContractLandAddNoSelecteVp);
                return;
            }
            if (isSelected)
            {
                //界面有选中项
                addPage.CurrentPerson = CurrentAccountItem.Tag;  //将当前承包方界面实体转换为底层数据库实体
            }
            else
            {
                //界面没有选中项
                addPage.CurrentPerson = addPage.VirtpersonList.FirstOrDefault();
            }
            addPage.CurrentLand = new ContractLand();
            TheWorkPage.Page.ShowMessageBox(addPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                ContractLandBinding landBinding = new ContractLandBinding(addPage.CurrentLand);
                ContractLandPersonItemHelper.ConvertCodeToName(landBinding, listDKLB, listDLDJ);    //地块类别名称和地力等级名称
                Dispatcher.Invoke(new Action(() =>
                {
                    if (isSelected)
                    {
                        //界面有选中项
                        CurrentAccountItem.Children.Add(landBinding);
                        if (CurrentAccountItem == null)
                        {
                            Refresh();
                            return;
                        }
                        string familyName = ContractLandPersonItemHelper.CreateItemName(CurrentAccountItem.Tag, CurrentAccountItem.Children.Count());
                        string actualArea = ContractLandPersonItemHelper.SumActualArea(CurrentAccountItem);

                        string awareArea = ContractLandPersonItemHelper.SumAwareArea(CurrentAccountItem);
                        string tableArea = ContractLandPersonItemHelper.SumTableArea(CurrentAccountItem);
                        string contractDelayArea = ContractLandPersonItemHelper.SumContractDelayArea(CurrentAccountItem);
                        CurrentAccountItem.Name = familyName;  //改变界面显示承包方名称信息(包含共有的地块数量)
                        CurrentAccountItem.ActualAreaUI = actualArea;   //改变界面显示承包方实测面积信息
                        CurrentAccountItem.AwareAreaUI = awareArea;      //改变界面显示承包方确权面积信息
                        CurrentAccountItem.TableAreaUI = tableArea;      //改变界面显示承包方二轮合同面积信息
                        CurrentAccountItem.ContractDelayAreaUI = contractDelayArea;
                    }
                    else
                    {
                        //界面没有选中项
                        ContractLandPersonItem personItem = accountLandItems.FirstOrDefault(c => c.ID == addPage.CurrentPerson.ID);
                        if (personItem == null)
                        {
                            Refresh();
                            return;
                        }
                        personItem.Visibility = Visibility.Visible;
                        personItem.Children.Add(landBinding);
                        string personName = ContractLandPersonItemHelper.CreateItemName(personItem.Tag, personItem.Children.Count());
                        string acArea = ContractLandPersonItemHelper.SumActualArea(personItem);
                        string awArea = ContractLandPersonItemHelper.SumAwareArea(personItem);
                        string taArea = ContractLandPersonItemHelper.SumTableArea(personItem);
                        string contractDelayArea = ContractLandPersonItemHelper.SumContractDelayArea(personItem);

                        personItem.Name = personName;  //改变界面显示承包方名称信息(包含共有的地块数量)
                        personItem.ActualAreaUI = acArea; //改变界面显示承包方实测面积信息
                        personItem.AwareAreaUI = awArea;  //改变界面显示承包方确权面积信息
                        personItem.TableAreaUI = taArea;  //改变界面显示承包方二轮合同面积信息
                        personItem.ContractDelayAreaUI = contractDelayArea;
                    }
                }));

                DataCount();
                ContractLand contractLand = addPage.CurrentLand;  //当前添加的承包地块(数据库实体)
                landList.Add(contractLand);   //向集合中添加新增地块
            });
        }

        /// <summary>
        /// 编辑承包地块
        /// </summary>
        public void ContractLandEdit()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandEdit, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentLandBinding == null)
            {
                //此时在界面上没有选中承包地块信息
                ShowBox(ContractAccountInfo.ContractLandEdit, ContractAccountInfo.LandEditNoSelected);
                return;
            }
            if (CurrentLandBinding.IsStockLand)//选中确股地块
            {
                ShowBox(ContractAccountInfo.ContractLandEdit, "选中地块为确股地块，请选择确权地块编辑");
                return;
            }
            bool isLock = CurrentAccountItem.Tag.Status == eVirtualPersonStatus.Lock ? true : false;
            if (isLock)
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((m, n) =>
                {
                    if (!(bool)m)
                        return;
                    StartEditLand(isLock);
                });
                ShowBox(ContractAccountInfo.ContractLandEdit, "当前地块被锁定,只能查看地块信息", eMessageGrade.Infomation, action, false);
            }
            else
            {
                StartEditLand(isLock);
            }
        }

        /// <summary>
        /// 开始编辑地块
        /// </summary>
        private void StartEditLand(bool isLock)
        {
            if (IsStockLand)
            {
                ralations = DataBaseSource.GetDataBaseSource().CreateVirtualPersonStation<LandVirtualPerson>().
                    GetRelationByZone(CurrentZone == null ? "" : CurrentZone.FullCode, eLevelOption.Self);
            }
            ContractLandPage editPage = new ContractLandPage();
            editPage.Workpage = TheWorkPage;
            editPage.CurrentZone = CurrentZone;
            editPage.VirtpersonList = VirtualPersonFilterForEdit();
            editPage.IsSelected = this.IsSelected; //传递标记
            editPage.CurrentPerson = CurrentAccountItem.Tag;
            editPage.IsLock = isLock;
            //editPage.spInitialDotCoil.Visibility = Visibility.Collapsed;
            ContractLand land = CurrentLandBinding.Tag;
            var landTemp = land.Clone() as ContractLand;
            editPage.CurrentLand = landTemp;
            TheWorkPage.Page.ShowMessageBox(editPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    if (CurrentAccountItem == null) return;
                    var landBinding = new ContractLandBinding(landTemp);
                    ContractLandPersonItemHelper.ConvertCodeToName(landBinding, listDKLB, listDLDJ);  //地块类别名称和地力等级名称
                    CurrentLandBinding.CopyPropertiesFrom(landBinding);
                    string actualArea = ContractLandPersonItemHelper.SumActualArea(CurrentAccountItem);
                    string awareArea = ContractLandPersonItemHelper.SumAwareArea(CurrentAccountItem);
                    string tableArea = ContractLandPersonItemHelper.SumTableArea(CurrentAccountItem);
                    string contractDelayArea = ContractLandPersonItemHelper.SumContractDelayArea(CurrentAccountItem);
                    CurrentAccountItem.ActualAreaUI = actualArea;  //更改承包方实测面积显示信息
                    CurrentAccountItem.AwareAreaUI = awareArea;    //更改承包方确权面积显示信息
                    CurrentAccountItem.TableAreaUI = tableArea;    //更改承包方二轮合同面积显示信息
                    CurrentAccountItem.ContractDelayAreaUI = contractDelayArea;
                    if (CurrentAccountItem.Tag.Name != editPage.CurrentPerson.Name)
                    {
                        //此时改变了当前编辑地块的承包人(要更改界面显示的承包方名称、实测面积和确权面积)
                        ContractLandPersonItem personItem = accountLandItems.FirstOrDefault(c => c.ID == editPage.CurrentPerson.ID);
                        //陈泽林 20161020 如果该承包方没有显示在台账中就新增
                        if (personItem == null)
                        {
                            var svpi = editPage.CurrentPerson.ConvertItem(landList, listDKLB, listDLDJ, IsStockLand, ralations);
                            accountLandItems.Add(svpi);
                            personItem = svpi;
                        }
                        personItem.Children.Add(CurrentLandBinding);
                        personItem.Name = ContractLandPersonItemHelper.CreateItemName(personItem.Tag, personItem.Children.Count())
                        + ContractLandPersonItemHelper.CreateItemNumber(personItem.Tag);
                        personItem.ActualAreaUI = ContractLandPersonItemHelper.SumActualArea(personItem);  //实测面积
                        personItem.AwareAreaUI = ContractLandPersonItemHelper.SumAwareArea(personItem);    //确权面积
                        personItem.TableAreaUI = ContractLandPersonItemHelper.SumTableArea(personItem);   //二轮合同面积
                        personItem.ContractDelayAreaUI = ContractLandPersonItemHelper.SumContractDelayArea(personItem);   //二轮合同面积
                        if (personItem.Children.Count > 0)
                            personItem.Visibility = Visibility.Visible;

                        CurrentAccountItem.Children.Remove(CurrentLandBinding);
                        if (CurrentAccountItem == null)
                            CurrentAccountItem = accountLandItems.FirstOrDefault(c => c.Tag.ID == land.OwnerId);
                        CurrentAccountItem.Name = ContractLandPersonItemHelper.CreateItemName(CurrentAccountItem.Tag, CurrentAccountItem.Children.Count())
                        + ContractLandPersonItemHelper.CreateItemNumber(CurrentAccountItem.Tag);
                        CurrentAccountItem.ActualAreaUI = ContractLandPersonItemHelper.SumActualArea(CurrentAccountItem);  //实测面积
                        CurrentAccountItem.AwareAreaUI = ContractLandPersonItemHelper.SumAwareArea(CurrentAccountItem);    //确权面积
                        CurrentAccountItem.TableAreaUI = ContractLandPersonItemHelper.SumTableArea(CurrentAccountItem);    //二轮合同面积
                        CurrentAccountItem.ContractDelayAreaUI = ContractLandPersonItemHelper.SumContractDelayArea(CurrentAccountItem);    //二轮合同面积
                        if (CurrentAccountItem.Children.Count == 0)
                            CurrentAccountItem.Visibility = Visibility.Collapsed;
                    }
                }));
                DataCount();
                var landRemove = landList.Find(c => c.ID == land.ID);
                landList.Remove(landRemove);
                landList.Add(editPage.CurrentLand);
            });
        }

        /// <summary>
        /// 过滤显示承包方
        /// </summary>
        private List<VirtualPerson> VirtualPersonFilter()
        {
            List<VirtualPerson> vps = new List<VirtualPerson>();
            try
            {
                var db = DataBaseSourceWork.GetDataBaseSource();
                if (db == null)
                    return null;
                var personStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
                var listPerson = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                if (ContractBusinessSettingDefine.DisplayCollectUsingCBdata)
                {
                    var persons = listPerson.FindAll(c => c.Name == "集体");
                    foreach (var vp in persons)
                    {
                        listPerson.Remove(vp);
                    }
                    persons.Clear();
                }
                listPerson.ForEach(c =>
                {
                    var vp = c.Status == eVirtualPersonStatus.Right ? c : null;
                    if (vp != null)
                        vps.Add(vp);
                });
                listPerson.Clear();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VitualPersonFilter(承包方过滤失败!)", ex.Message + ex.StackTrace);
                TheWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "过滤承包方",
                    Message = "过滤承包方出错!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                });
            }
            return vps;
        }

        /// <summary>
        /// 过滤显示承包方(编辑时使用)
        /// </summary>
        private List<VirtualPerson> VirtualPersonFilterForEdit()
        {
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            try
            {
                var db = DataBaseSource.GetDataBaseSource();
                if (db == null)
                    return null;
                var personStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
                listPerson = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                if (ContractBusinessSettingDefine.DisplayCollectUsingCBdata)
                {
                    var persons = listPerson.FindAll(c => c.Name == "集体");
                    foreach (var vp in persons)
                    {
                        listPerson.Remove(vp);
                    }
                    persons.Clear();
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VitualPersonFilter(承包方过滤失败!)", ex.Message + ex.StackTrace);
                TheWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "过滤承包方",
                    Message = "过滤承包方出错!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                });
            }
            return listPerson;
        }

        /// <summary>
        /// 删除承包地块
        /// </summary>
        public void ContractLandDel()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentLandBinding == null)
            {
                //此时在界面上没有选中承包地块信息
                ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.LandDelNoSelected);
                return;
            }
            if (CurrentLandBinding.IsStockLand)//选中确股地块
            {
                ShowBox(ContractAccountInfo.ContractLandEdit, "请选择确权地块删除");
                return;
            }
            if (CurrentAccountItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.ContractLandDelLock);
                return;
            }

            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var land = CurrentLandBinding.Tag;
                try
                {
                    Guid[] landIds = new Guid[] { land.ID };
                    //var landDelStation = DbContext.CreateContractLandDeleteWorkstation();
                    //landDelStation.Add(land);
                    var landStation = DbContext.CreateContractLandWorkstation();
                    var landdel = GetLandDel(land);
                    landStation.AddDelLand(landdel);
                    landStation.DeleteRelationDataByLand(landIds);
                }
                catch (Exception)
                {
                    ShowBox(ContractAccountInfo.ContractLandDel, "删除承包地块失败!", showConfirm: false);
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    var landBinding = CurrentLandBinding.Clone() as ContractLandBinding;
                    CurrentAccountItem.Children.Remove(CurrentLandBinding);  //从界面移除当前删除的承包地块信息
                    if (CurrentAccountItem == null)
                        CurrentAccountItem = accountLandItems.FirstOrDefault(c => c.Tag.ID == landBinding.Tag.OwnerId);
                    CurrentAccountItem.AwareAreaUI = ContractLandPersonItemHelper.SumAwareArea(CurrentAccountItem);
                    CurrentAccountItem.ActualAreaUI = ContractLandPersonItemHelper.SumActualArea(CurrentAccountItem);
                    CurrentAccountItem.TableAreaUI = ContractLandPersonItemHelper.SumTableArea(CurrentAccountItem);
                    CurrentAccountItem.ContractDelayAreaUI = ContractLandPersonItemHelper.SumContractDelayArea(CurrentAccountItem);
                    CurrentAccountItem.Name = ContractLandPersonItemHelper.CreateItemName(CurrentAccountItem.Tag, CurrentAccountItem.Children.Count());
                    if (CurrentAccountItem.Children.Count == 0)
                    {
                        CurrentAccountItem.Visibility = Visibility.Collapsed;   //没有地块则不显示
                        accountLandItems.Remove(CurrentAccountItem);
                    }
                }));
                DataCount();
                var landRemove = landList.Find(c => c.ID == land.ID);
                landList.Remove(landRemove);    //从集合中移除删除的地块
                var args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTLAND_DELETE_COMPLETE, land, CurrentZone.FullCode);
                TheWorkPage.Workspace.Message.Send(this, args);
            });
            ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.CurrentLandDelSure, eMessageGrade.Infomation, action);
        }
        
        /// <summary>
        /// 修改地块所有人名称
        /// </summary>
        /// <param name="vp"></param>
        public void ChangeLandOwnerPerson(VirtualPerson vp)
        {
            var landStation = DbContext.CreateContractLandWorkstation();
            landStation.Update(vp.ID, vp.Name);
            Refresh();
        }

        #endregion Method-地块基本操作

        #region Method-导入数据

        /// <summary>
        /// 导入地块调查表数据
        /// </summary>
        public void ImportLandExcel()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportZone, ContractAccountInfo.ImportZone);
                return;
            }
            List<Zone> childrenZones = ContractAccountBusiness.GetChildrenZone(currentZone);
            if ((currentZone.Level == eZoneLevel.Group) || (currentZone.Level == eZoneLevel.Village && childrenZones.Count == 0))
            {
                //选择为组级地域或者选择为村级地域的同时地域下没有子级地域(单个表格导入,执行单个任务)
                ImportDataPage importLand = new ImportDataPage(TheWorkPage, "导入地块数据调查表");
                TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
                {
                    if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                    {
                        return;
                    }
                    ImportLandInformationTask(importLand.FileName);
                });
            }
            else if (currentZone.Level == eZoneLevel.Village && childrenZones != null && childrenZones.Count > 0)
            {
                //选择为村级地域并且地域下有子级地域(多个表格导入)
                ExportDataPage batchImportDlg = new ExportDataPage(CurrentZone.FullName, TheWorkPage, "批量导入地块数据调查表", "请选择保存文件路径", "导入", "导入路径:");
                TheWorkPage.Page.ShowMessageBox(batchImportDlg, (b, r) =>
                {
                    if (!(bool)b || string.IsNullOrEmpty(batchImportDlg.FileName))
                    {
                        return;
                    }
                    ImportLandInformationTaskGroup(batchImportDlg.FileName);
                });
            }
            else
            {
                //此时选择地域大于村
                ShowBox(ContractAccountInfo.ImportData, ContractAccountInfo.ImportErrorZone);
                return;
            }
        }

        /// <summary>
        /// 导入承包关系表数据
        /// </summary>
        public void ImportLandTiesExcel()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportZone, ContractAccountInfo.ImportZone);
                return;
            }
            List<Zone> childrenZones = ContractAccountBusiness.GetChildrenZone(currentZone);
            if ((currentZone.Level == eZoneLevel.Group) || (currentZone.Level == eZoneLevel.Village && childrenZones.Count == 0))
            {
                //选择为组级地域或者选择为村级地域的同时地域下没有子级地域(单个表格导入,执行单个任务)
                var importLand = new ImportDataPage(TheWorkPage, "导入摸底核实表");
                TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
                {
                    if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                    {
                        return;
                    }
                    ImportLandTiesTask(importLand.FileName, importLand.ImportType);
                });
            }
            else
            {
                //此时选择地域大于村
                ShowBox("导入承包关系表", ContractAccountInfo.ImportErrorZone);
                return;
            }
        }

        /// <summary>
        /// 导入地块调查表任务
        /// </summary>
        /// <param name="fileName">选择文件路径</param>
        private void ImportLandInformationTask(string fileName)
        {
            TaskImportLandTableArgument meta = new TaskImportLandTableArgument();
            meta.DbContext = DbContext;       //当前使用的数据库
            meta.CurrentZone = currentZone;    //当前地域
            meta.FileName = fileName;
            meta.VirtualType = virtualType;
            TaskImportLandTableOperation import = new TaskImportLandTableOperation();
            import.Argument = meta;
            import.Description = ContractAccountInfo.ImportDataComment;
            import.Name = ContractAccountInfo.ImportData;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); RefreshStockRight(); }), null);
                var args = MessageExtend.VirtualPersonMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORT_COMPLETE, currentZone.FullCode);
                SendMessasge(args);
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(ContractAccountInfo.ImportData, ContractAccountInfo.ImportDataFail);
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导入地块调查表任务
        /// </summary>
        /// <param name="fileName">选择文件夹路径</param>
        private void ImportLandInformationTaskGroup(string fileName)
        {
            TaskGroupImportLandTableArgument groupArgument = new TaskGroupImportLandTableArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            TaskGroupImportLandTableOperation groupOperation = new TaskGroupImportLandTableOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = ContractAccountInfo.ImportDataComment;
            groupOperation.Name = ContractAccountInfo.VolumnImportDataTable;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); RefreshStockRight(); }), null);
                TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORT_COMPLETE, currentZone.FullCode));
            });
            groupOperation.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(ContractAccountInfo.VolumnImportDataTable, ContractAccountInfo.VolumnImportFail);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        public void ImportLandTiesTask(string fileName, eImportTypes eImport = eImportTypes.Over)
        {
            var meta = new TaskImportLandTiesTableArgument();
            meta.DbContext = DbContext;       //当前使用的数据库
            meta.CurrentZone = currentZone;    //当前地域
            meta.FileName = fileName;
            meta.ImportType = eImport;
            var import = new TaskImportLandTiesTableOperation();
            import.Argument = meta;
            import.Description = $"导入承包关系表中地块数据-{Path.GetFileName(fileName)}";
            import.Name = $"导入承包关系表-{currentZone.Name}";
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); RefreshStockRight(); }), null);
                var args = MessageExtend.VirtualPersonMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORT_COMPLETE, currentZone.FullCode);
                SendMessasge(args);
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(ContractAccountInfo.ImportData, ContractAccountInfo.ImportDataFail);
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 抽象出的公有导入表格方法
        /// </summary>
        private void CommonImport(TaskContractAccountArgument meta, IDbContext dbContext,
            string str1, string str2, string str3, string str4, string str5)
        {
            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = str1;
            import.Name = str2;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(dbContext, str3, currentZone.FullCode));
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(str4, str5);
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导入地块图斑
        /// </summary>
        public void ImportVectorName()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportZone, "未选择行政地域");
                return;
            }
            var dbContext = CreateDb();
            ImportLandShapePage addPage = new ImportLandShapePage(TheWorkPage, "导入地块图斑");
            addPage.ThePage = TheWorkPage;
            addPage.Db = dbContext;
            TheWorkPage.Page.ShowMessageBox(addPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                TaskContractAccountArgument meta = new TaskContractAccountArgument();
                meta.FileName = addPage.FileName;
                meta.IsClear = true;
                meta.ArgType = eContractAccountType.ImportLandShapeData;
                meta.Database = dbContext;
                meta.CurrentZone = currentZone;
                meta.VirtualType = virtualType;
                meta.UseContractorInfoImport = addPage.UseContractorInfoImport;
                meta.UseLandCodeBindImport = addPage.UseLandCodeBindImport;
                meta.UseContractorNumberImport = addPage.UseContractorNumberImport;
                meta.UseOldLandCodeBindImport = addPage.UseOldLandCodeBindImport;
                meta.shapeAllcolNameList = addPage.shapeAllcolNameList;
                ImportLandShapeData(meta, dbContext);
            });
        }

        /// <summary>
        /// 导入图斑方法
        /// </summary>
        private void ImportLandShapeData(TaskContractAccountArgument meta, IDbContext dbContext)
        {
            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = ContractAccountInfo.ImportShapeData;
            import.Name = ContractAccountInfo.ImportShpData;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                TheBns.Current.Message.Send(this, MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTSHP_COMPLETE, "", currentZone.FullCode));
                Refresh();
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导入界址点调查图斑数据
        /// </summary>
        public void ImportBoundaryAddressDot()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportBoundaryAddressDot, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            var dotPage = new ImportBoundaryAddressDotPage(TheWorkPage, "导入界址点图斑");
            TheWorkPage.Page.ShowMessageBox(dotPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                List<Zone> allZones = new List<Zone>();
                allZones = GetAllChildrenZones() as List<Zone>;
                List<Zone> allChildrenZones = new List<Zone>();
                allChildrenZones = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                IDbContext dbContext = CreateDb();
                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                {
                    //执行单个任务
                    TaskImportDotArgument meta = new TaskImportDotArgument();
                    meta.CurrentZone = currentZone;
                    meta.FileName = dotPage.FileName;
                    meta.Database = dbContext;
                    meta.VirtualType = virtualType;
                    TaskImportDotOperation import = new TaskImportDotOperation();
                    import.Argument = meta;
                    import.Description = ContractAccountInfo.ImportBoundaryAddressDot;
                    import.Name = ContractAccountInfo.ImportBoundaryAddressDot;
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, dotPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(import);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    import.StartAsync();
                }
                else if (CurrentZone.Level == eZoneLevel.Village && allChildrenZones.Count > 0)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupImportDotArgument groupMeta = new TaskGroupImportDotArgument();
                    groupMeta.CurrentZone = currentZone;
                    groupMeta.AllZones = allZones;
                    groupMeta.Database = DbContext;
                    groupMeta.FileName = dotPage.FileName;
                    groupMeta.VirtualType = virtualType;
                    TaskGroupImportDotOperation taskGroup = new TaskGroupImportDotOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = ContractAccountInfo.ImportBoundaryAddressDot;
                    taskGroup.Name = ContractAccountInfo.ImportBoundaryAddressDot;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, dotPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(taskGroup);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox(ContractAccountInfo.ImportBoundaryAddressDot, ContractAccountInfo.VolumnImportErrorZone);
                    return;
                }
            });
        }

        /// <summary>
        /// 导入界址线调查图斑数据
        /// </summary>
        public void ImportBoundaryAddressCoil()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportBoundaryAddressCoil, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext dbContext = CreateDb();
            var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();

            int currentZoneDotListCount = dotStation.Count(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (currentZoneDotListCount == 0)
            {
                ShowBox(ContractAccountInfo.ImportBoundaryAddressCoil, "本地域下无界址点数据，请先导入对应界址点数据");
                return;
            }
            var currentZoneDotList = dotStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
            ImportBoundaryAddressCoilPage coilPage = new ImportBoundaryAddressCoilPage(TheWorkPage, "导入界址线图斑");
            TheWorkPage.Page.ShowMessageBox(coilPage, (b, r) =>
             {
                 if (!(bool)b)
                 {
                     return;
                 }
                 List<Zone> allZones = new List<Zone>();
                 List<Zone> allChildrenZones = new List<Zone>();

                 try
                 {
                     allZones = GetAllChildrenZones() as List<Zone>;
                     allChildrenZones = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                 }
                 catch (Exception ex)
                 {
                     YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取子级地域失败)", ex.Message + ex.StackTrace);
                     ShowBox(ContractAccountInfo.ImportBoundaryAddressCoil, "获取当前地域下的子级地域失败!");
                     return;
                 }
                 if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                 {
                     //执行单个任务
                     TaskImportCoilArgument meta = new TaskImportCoilArgument();
                     meta.CurrentZone = currentZone;
                     meta.FileName = coilPage.FileName;
                     meta.Database = dbContext;
                     meta.VirtualType = virtualType;
                     meta.CurrentZoneDotList = currentZoneDotList;
                     TaskImportCoilOperation import = new TaskImportCoilOperation();
                     import.Argument = meta;
                     import.Description = ContractAccountInfo.ImportBoundaryAddressCoil;
                     import.Name = ContractAccountInfo.ImportBoundaryAddressCoil;
                     import.Completed += new TaskCompletedEventHandler((o, t) =>
                     {
                         ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, coilPage.FileName, CurrentZone.FullCode);
                         TheBns.Current.Message.Send(this, args);
                     });
                     TheWorkPage.TaskCenter.Add(import);
                     if (ShowTaskViewer != null)
                         ShowTaskViewer();
                     import.StartAsync();
                 }
                 else if (CurrentZone.Level == eZoneLevel.Village && allChildrenZones.Count > 0)
                 {
                     //执行批量任务(含有子任务)
                     TaskGroupImportCoilArgument groupMeta = new TaskGroupImportCoilArgument();
                     groupMeta.CurrentZone = currentZone;
                     groupMeta.AllZones = allZones;
                     groupMeta.Database = DbContext;
                     groupMeta.FileName = coilPage.FileName;
                     groupMeta.VirtualType = virtualType;
                     TaskGroupImportCoilOperation taskGroup = new TaskGroupImportCoilOperation();
                     taskGroup.Argument = groupMeta;
                     taskGroup.Description = ContractAccountInfo.ImportBoundaryAddressCoil;
                     taskGroup.Name = ContractAccountInfo.ImportBoundaryAddressCoil;
                     taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                     {
                         ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, coilPage.FileName, CurrentZone.FullCode);
                         TheBns.Current.Message.Send(this, args);
                     });
                     TheWorkPage.TaskCenter.Add(taskGroup);
                     if (ShowTaskViewer != null)
                         ShowTaskViewer();
                     taskGroup.StartAsync();
                 }
                 else
                 {
                     //选择地域为镇(或大于镇)
                     ShowBox(ContractAccountInfo.ImportBoundaryAddressCoil, ContractAccountInfo.VolumnImportErrorZone);
                     return;
                 }
             });
        }

        #endregion Method-导入数据

        #region Method-导出数据

        #region Method-导出数据模板

        /// <summary>
        /// 导出承包方Excel模板
        /// </summary>
        public void ExportVirtualPersonSurveyExcelTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.ExcelTemplate(TemplateFile.VirtualPersonSurveyExcel);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, VirtualPersonInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportExcelTemplate(导出承包方Excel模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出承包方Word模板
        /// </summary>
        public void ExportVirtualPersonSurveyWordTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, VirtualPersonInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWordTemplate(导出承包方Word模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方Word模板
        /// </summary>
        public void ExportSenderSurveyWordTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, SenderInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWordTemplate(导出发包方Word模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方Excel模板
        /// </summary>
        public void ExportSenderSurveyExcelTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, SenderInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportExcelTemplate(导出发包方Excel模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出承包地块Word调查模板
        /// </summary>
        public void ExportContractLandSurveyWordTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.WordTemplate(TemplateFile.ContractLandSurveyWord);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, ContractLandInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWordTemplate(导出承包地块Word模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出承包地块Excel调查模板
        /// </summary>
        public void ExportContractLandSurveyExcelTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExcel);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, ContractLandInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportExcelTemplate(导出承包地块Excel模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出界址信息Excel调查模板
        /// </summary>
        public void ExportBoundarySurveyExcelTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.ExcelTemplate(TemplateFile.BoundarySurveyExcel);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, BoundaryInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportExcelTemplate(导出界址信息Excel模板)", ex.Message + ex.StackTrace);
            }
        }

        #endregion Method-导出数据模板

        #region Method-调查报表-导出Word

        /// <summary>
        /// 导出发包方调查表（Word）
        /// </summary>
        public void ExportSenderWord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSenderNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    var landStation = DbContext.CreateAgriculturalLandWorkstation();
                    var senderStation = DbContext.CreateCollectivityTissueWorkStation();
                    var tissues = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
                    if (tissues == null || tissues.Count == 0)
                    {
                        ShowBox("导出发包方Word调查表", "未获取发包方数据!", showConfirm: false);
                        return;
                    }
                    if (tissues.Count == 1)
                    {
                        string tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.SenderSurveyWord);
                        YuLinTu.Library.Business.ExportSenderWord senderTable = new YuLinTu.Library.Business.ExportSenderWord();

                        #region 通过反射等机制定制化具体的业务处理类

                        var temp = WorksheetConfigHelper.GetInstance(senderTable);
                        if (temp != null && temp.TemplatePath != null)
                        {
                            if (temp is YuLinTu.Library.Business.ExportSenderWord)
                                senderTable = (YuLinTu.Library.Business.ExportSenderWord)temp;
                            tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                        }

                        #endregion 通过反射等机制定制化具体的业务处理类

                        senderTable.OpenTemplate(tempPath);
                        senderTable.PrintPreview(tissues.FirstOrDefault(), SystemSet.DefaultPath + "\\" + tissues.FirstOrDefault().Name + "-发包方调查表");
                        //landStation.ExportSenderSurveyWord(tissues.FirstOrDefault(), SystemSet.DefaultPath);
                    }
                    else
                    {
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportSenderDataExcel, eContractAccountType.ExportSendTableWord,
                            ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSurveyTableData);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportSenderDataExcel, eContractAccountType.VolumnExportSenderTableWord,
                        ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderWord(导出发包方Word表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出发包方Word(单个任务)
        /// </summary>
        private void ExportSenderWordTask(string fileName, string taskDes, string taskName)
        {
            TaskExportSenderWordArgument argument = new TaskExportSenderWordArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            TaskExportSenderWordOperation operation = new TaskExportSenderWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出发包方Word(组任务)
        /// </summary>
        private void ExportSenderWordTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportSenderWordArgument groupArgument = new TaskGroupExportSenderWordArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            TaskGroupExportSenderWordOperation groupOperation = new TaskGroupExportSenderWordOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出承包方调查表（Word）
        /// </summary>
        public void ExportVPWord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    if (IsBatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = TheWorkPage;
                        foreach (var item in accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportVPWordNoSelected);
                                return;
                            }
                            ExportDataCommonOperate(CurrentZone.FullName, ContractAccountInfo.ExportDataWord, eContractAccountType.ExportContractorTable,
                                ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportSurveyTableData, 1, selectPage.SelectedPersons);
                            listPerson.Clear();
                            listPerson = null;
                        });
                    }
                    else
                    {
                        if (CurrentAccountItem == null)
                        {
                            ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ViewDataNo);
                            return;
                        }
                        var concordStation = DbContext.CreateConcordStation();
                        var concords = concordStation.GetAllConcordByFamilyID(CurrentAccountItem.Tag.ID);
                        string concordnumber = "", warrentnumber = "";
                        if (concords != null && concords.Count > 0)
                        {
                            concordnumber = concords[0].ConcordNumber;
                        }
                        var bookStation = DbContext.CreateRegeditBookStation();
                        var book = concords != null && concords.Count > 0 ? bookStation.Get(concords[0].ID) : null;
                        if (book != null)
                            warrentnumber = book.Number;
                        string masdsc = GetMarkDesc(currentZone);
                        var landStation = DbContext.CreateAgriculturalLandWorkstation();
                        var senderStation = DbContext.CreateSenderWorkStation();
                        var sender = senderStation.Get(currentZone.ID);
                        if (sender == null)
                        {
                            var tissues = senderStation.GetTissues(CurrentZone.FullCode, eLevelOption.Self);
                            if (tissues != null && tissues.Count > 0)
                            {
                                sender = tissues[0];
                            }
                        }
                        var regetBookStation = DbContext.CreateRegeditBookStation();
                        //var book=regetBookStation.GetCollection()
                        if (SystemSetDefine.ExportTableSenderDesToVillage)
                        {
                            var Sender = senderStation.GetTissues(currentZone.UpLevelCode, eLevelOption.Self);
                            if (Sender.Count > 0)
                            {
                                sender = Sender[0];
                            }
                        }
                        VirtualPerson vp = CurrentAccountItem.Tag.Clone() as VirtualPerson;
                        if (SystemSet.PersonTable)
                        {
                            List<Person> person = vp.SharePersonList;
                            person = person.FindAll(c => c.IsSharedLand.Equals("是"));
                            vp.SharePersonList = person;
                        }
                        landStation.ExportObligeeWord(currentZone, vp, masdsc, concordnumber, sender, DictList, warrentnumber, book,
                            SystemSet.DefaultPath, SystemSet.ExportVPTableCountContainsDiedPerson, SystemSet.KeepRepeatFlag, () => { return WorkStationExtend.GetSystemSetReplacement(); });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(CurrentZone.FullName, ContractAccountInfo.ExportDataWord, eContractAccountType.VolumnExportContractorTable,
                        ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVPWord(导出承包方Word调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出承包方Word调查表(单个任务)
        /// </summary>
        private void ExportVPWordTask(string fileName, string taskDes, string taskName, List<VirtualPerson> selectedPersons)
        {
            TaskExportVPWordArgument argument = new TaskExportVPWordArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.SelectedPersons = selectedPersons;
            argument.DictList = DictList;
            TaskExportVPWordOperation operation = new TaskExportVPWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出承包方Word调查表(组任务)
        /// </summary>
        private void ExportVPWordTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportVPWordArgument groupArgument = new TaskGroupExportVPWordArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            groupArgument.DictList = DictList;
            TaskGroupExportVPWordOperation groupOperation = new TaskGroupExportVPWordOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出地块调查表（Word）
        /// </summary>
        public void ExportLandWord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                if (!IsBatch && CurrentAccountItem == null)
                {
                    ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ViewDataNo);
                    return;
                }
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    if (IsBatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = TheWorkPage;
                        foreach (var item in accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportLandDataWordNoSelected);
                                return;
                            }
                            // ExportLandWordTask(SystemSet.DefaultPath, ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, selectPage.SelectedPersons);
                            ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandDataWord, eContractAccountType.ExportLandSurveyWord,
                          ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, 1, selectPage.SelectedPersons);
                        });
                    }
                    else if (CurrentAccountItem != null && CurrentLandBinding == null)
                    {
                        listPerson.Add(CurrentAccountItem.Tag);
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandDataWord, eContractAccountType.ExportLandSurveyWord,
                            ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, 1, listPerson);
                    }
                    else
                    {
                        CurrentLandBinding.Tag.LandNumberFormat(SystemSetDefine);
                        bool flag = ContractAccountBusiness.ExportLandWord(currentZone, CurrentLandBinding.Tag, CurrentAccountItem.Tag, DictList);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(CurrentZone.FullName, ContractAccountInfo.ExportLandDataWord, eContractAccountType.VolumnExportLandSurveyTable,
                        ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出承包地块Word调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出承包地块Word调查表(单个任务)
        /// </summary>
        private void ExportLandWordTask(string fileName, string taskDes, string taskName, List<VirtualPerson> selectedPersons)
        {
            TaskExportLandWordArgument argument = new TaskExportLandWordArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.SelectedPersons = selectedPersons;
            TaskExportLandWordOperation operation = new TaskExportLandWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出承包地块Word调查表(组任务)
        /// </summary>
        private void ExportLandWordTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportLandWordArgument groupArgument = new TaskGroupExportLandWordArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            TaskGroupExportLandWordOperation groupOperation = new TaskGroupExportLandWordOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出公示结果归户表(Word)
        /// </summary>
        public void ExportPublishWord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (!IsBatch && CurrentAccountItem == null)
                {
                    ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ViewDataNo);
                    return;
                }
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    if (IsBatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        List<VirtualPerson> listPerson = new List<VirtualPerson>();
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = TheWorkPage;
                        foreach (var item in accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ExportPublishDataWordNoSelected);
                                return;
                            }
                            //ExportPublishWordTask(SystemSet.DefaultPath, ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ExportSurveyTableData, selectPage.SelectedPersons);
                            ExportDataCommonOperate(CurrentZone.FullName, ContractAccountInfo.ExportPublicDataWord, eContractAccountType.ExportSurveyTable,
                                ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ExportSurveyTableData, 1, selectPage.SelectedPersons);
                        });
                    }
                    else
                    {
                        List<ContractLand> lands = ContractAccountBusiness.GetPersonCollection(CurrentAccountItem.Tag.ID);
                        lands.LandNumberFormat(SystemSetDefine);
                        bool flag = ContractAccountBusiness.ExportPublishWord(currentZone, CurrentAccountItem.Tag, lands);   //导出单个
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(CurrentZone.FullName, ContractAccountInfo.ExportPublicDataWord, eContractAccountType.VolumnExportSurveyTable,
                        ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishWord(导出公示结果归户表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出公示结果归户表(单个任务)
        /// </summary>
        private void ExportPublishWordTask(string fileName, string taskDes, string taskName, List<VirtualPerson> selectedPersons)
        {
            TaskExportPublishWordArgument argument = new TaskExportPublishWordArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.SelectedPersons = selectedPersons;
            TaskExportPublishWordOperation operation = new TaskExportPublishWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出公示结果归户表(组任务)
        /// </summary>
        private void ExportPublishWordTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportPublishWordArgument groupArgument = new TaskGroupExportPublishWordArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            TaskGroupExportPublishWordOperation groupOperation = new TaskGroupExportPublishWordOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion Method-调查报表-导出Word

        #region Method-调查报表-导出Excel

        /// <summary>
        /// 导出发包方excel调查表
        /// </summary>
        public void ExportSenderExcel()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSenderNoZone);
                return;
            }

            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    ShowBox("导出发包方Excel调查表", DataBaseSource.ConnectionError);
                    return;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                var childrenZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                var landStation = dbContext.CreateAgriculturalLandWorkstation();
                int cnt = landStation.ExportSenderSurveyExcelNew(currentZone.FullCode, SystemSet.DefaultPath, SystemSet.GetTableHeaderStr(CurrentZone)); //修改新方法（用于返回值进行提示）
                if (cnt == -1)
                {
                    ShowBox(ContractAccountInfo.ExportSenderDataExcel, "行政地域编码错误!", showConfirm: false);
                }
                else if (cnt == 0)
                {
                    ShowBox(ContractAccountInfo.ExportSenderDataExcel, "未获取发包方数据!", showConfirm: false);
                }
                else
                {
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportSenderDataExcel, eContractAccountType.ExportSendTableExcel,
                                   ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSurveyTableData, 1, null);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderExcel(导出发包方excel调查表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方Excel(单个任务)
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        private void ExportSenderExcelTask(string fileName, string taskDes, string taskName)
        {
            TaskExportSenderExcelArgument argument = new TaskExportSenderExcelArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            TaskExportSenderExcelOperation operation = new TaskExportSenderExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 导出承包方excel调查表
        /// </summary>
        public void ExportVPExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    ExportVPExcelTask(SystemSet.DefaultPath, ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.ExportSurveyTableData);
                    //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportContractDataExcel,
                    //eContractAccountType.ExportVirtualPersonExcel, ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportContractDataExcel,
                   eContractAccountType.VolumnExportVirtualPersonExcel, ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportContractDataExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVPExcel(导出承包方Excel调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出承包方excel调查表(单个任务)
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        private void ExportVPExcelTask(string fileName, string taskDes, string taskName)
        {
            TaskExportVPExcelArgument argument = new TaskExportVPExcelArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.IsShow = true;
            argument.SystemSet = SystemSet;
            TaskExportVPExcelOperation operation = new TaskExportVPExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出承包方excel调查表(组任务)
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        private void ExportVPExcelTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportVPExcelArgument groupArgument = new TaskGroupExportVPExcelArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            groupArgument.SystemSet = SystemSet;
            TaskGroupExportVPExcelOperation groupOperation = new TaskGroupExportVPExcelOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出界址信息Excel
        /// </summary>
        public void ExportBoundaryInfoExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }
                    TaskExportBoundaryInfoExcel(eContractAccountType.ExportBoundaryInfoExcel, ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.ExportSurveyTableData, SystemSet.DefaultPath);
                    //       ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportBoundaryInfoExcel,
                    //eContractAccountType.ExportBoundaryInfoExcel, ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportBoundaryInfoExcel,
             eContractAccountType.VolumnExportBoundaryInfoExcel, ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportBoundaryInfoExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportBoundaryInfoExcel(导出界址调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 单进度导出界址信息Excel
        /// </summary>
        private void TaskExportBoundaryInfoExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "",
            List<VirtualPerson> listPerson = null)
        {
            TaskExportBoundaryInfoExcelArgument meta = new TaskExportBoundaryInfoExcelArgument();
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.ArgType = type;
            meta.DictList = DictList;
            meta.FileName = filePath;
            meta.IsShow = true;
            TaskExportBoundaryInfoExcelOperation import = new TaskExportBoundaryInfoExcelOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        private void TaskGroupExportExcelByType(eContractAccountType type, string taskName, string taskDes, string filePath,
            Type argtype, Type opraType)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);
            var meta = Activator.CreateInstance(argtype, true) as TaskGroupExportArgument;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.DictList = DictList;
            var import = Activator.CreateInstance(opraType, true) as TaskGroup;
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }


        /// <summary>
        /// 多进度导出界址信息Excel
        /// </summary>
        private void TaskGroupExportBoundaryInfoExcel(eContractAccountType type, string taskDes, string taskName, string filePath = "")
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);
            var meta = new TaskGroupExportBoundaryInfoExcelArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.DictList = DictList;
            var import = new TaskGroupExportBoundaryInfoExcelOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 导出调查信息公示表Excel
        /// </summary>
        public void ExportPublishExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }
                    ContractAccountDateSetting dateSetting = new ContractAccountDateSetting();
                    dateSetting.Workpage = TheWorkPage;
                    TheWorkPage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        PublishDateSetting = dateSetting.DateTimeSetting;
                        ExportPublishExcelTask(SystemSet.DefaultPath, ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportSurveyTableData);
                        //       ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandSurveyInFoPubExcel,
                        //eContractAccountType.ExportPublishTable, ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportSurveyTableData);
                    });
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ContractAccountDateSetting dateSetting = new ContractAccountDateSetting();
                    dateSetting.Workpage = TheWorkPage;
                    TheWorkPage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        PublishDateSetting = dateSetting.DateTimeSetting;
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandSurveyInFoPubExcel,
                 eContractAccountType.VolumnExportPublishTable, ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportSurveyTableData);
                    });
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishExcel(导出调查信息公示表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出调查信息公示表Excel(单任务)
        /// </summary>
        private void ExportPublishExcelTask(string fileName, string taskDes, string taskName)
        {
            TaskExportSurveyPublishExcelArgument argument = new TaskExportSurveyPublishExcelArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.PublishDateSetting = PublishDateSetting;
            argument.IsShow = true;
            TaskExportSurveyPublishExcelOperation operation = new TaskExportSurveyPublishExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出调查信息公示表Excel(组任务)
        /// </summary>
        private void ExportPublishExcelTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportSurveyPublishExcelArgument groupArgument = new TaskGroupExportSurveyPublishExcelArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            groupArgument.PublishDateSetting = PublishDateSetting;
            TaskGroupExportSurveyPublishExcelOperation groupOperation = new TaskGroupExportSurveyPublishExcelOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion Method-调查报表-导出Excel

        #region Method-摸底核实表

        public void ExportVerifyExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportLandVerifyExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandVerifyExcel, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }

                    ExportVerifyExcelTask(SystemSet.DefaultPath, ContractAccountInfo.ExportLandVerifyExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandVerifyExcel,
                eContractAccountType.VolumnExportLandVerifyExcel, ContractAccountInfo.ExportLandVerifyExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandVerifyExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishExcel(导出调查信息公示表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        public void ExportVerifyExcelTask(string fileName, string taskDes, string taskName)
        {
            TaskExportLandVerifyExcelArgument argument = new TaskExportLandVerifyExcelArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.IsShow = true;
            TaskExportLandVerifyExcelOperation operation = new TaskExportLandVerifyExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        public void ExportVerifyExcelTaskGroup(string fileName, string taskDes, string taskName)
        {
            var groupArgument = new TaskGroupExportLandVerifyExcelArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            var groupOperation = new TaskGroupExportLandVerifyExcelOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion Method-摸底核实表

        #region Method-摸底核实表(打印版)

        public void ExportVerifyPrintExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportLandVerifyPrintExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (accountLandItems == null || accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandVerifyPrintExcel, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }

                    ExportVerifyExcelPrintTask(SystemSet.DefaultPath, ContractAccountInfo.ExportLandVerifyPrintExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportLandVerifyPrintExcel,
                eContractAccountType.VolumnExportLandVerifyPrintExcel, ContractAccountInfo.ExportLandVerifyPrintExcel, ContractAccountInfo.ExportSurveyTableData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandVerifyPrintExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishExcel(导出调查信息公示表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        public void ExportVerifyExcelPrintTask(string fileName, string taskDes, string taskName)
        {
            TaskExportLandVerifyExcelArgument argument = new TaskExportLandVerifyExcelArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.VirtualType = VirtualType;
            argument.IsShow = true;
            TaskExportLandVerifyPrintExcelOperation operation = new TaskExportLandVerifyPrintExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        public void ExportVerifyExcelPrintTaskGroup(string fileName, string taskDes, string taskName)
        {
            var groupArgument = new TaskGroupExportLandVerifyExcelArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = VirtualType;
            var groupOperation = new TaskGroupExportLandVerifyExcelPrintOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion Method-摸底核实表(打印版)

        #region Method-台账导出

        public void ExportContractInformationExcel(bool isAccountExcel = true)
        {
            int TableType = 6;
            if (CurrentZone == null)
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

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                //单个任务
                if (accountLandItems == null || accountLandItems.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.CurrentZoneNoLand);
                    return;
                }
                TaskExportContractDelayAccountExcel(eContractAccountType.ExportContractInformationExcel, markDesc, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权台账调查表
        /// </summary>
        public void ExportAccountNameExcel(bool isAccountExcel = true)
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 1;
            if (CurrentZone == null)
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
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                //单个任务
                if (accountLandItems == null || accountLandItems.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.CurrentZoneNoLand);
                    return;
                }
                TaskExportContractDelayAccountExcel(eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
            else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
            {
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
        }

        /// <summary>
        /// 导出单户摸底调查表
        /// </summary>
        public void ExportVerifyExcelSingle()
        {
            if (CurrentZone == null)
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
            string markDesc = $"批量导出{currentZone.FullName}的单户摸底调查表";
            string taskName = "导出单户摸底调查表";

            ExportDataCommonMethod(currentZone.FullName, taskName,
                (filepath) =>
                {
                    TaskGroupExportExcelByType(eContractAccountType.ExportLandVerifySingeExcel,
                       taskName, markDesc, filepath, typeof(TaskGroupExportArgument), typeof(TaskGroupExportLandVerifySingleExcel));
                });
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权台账调查表
        /// </summary>
        public void ExportAccountLandNameExcel(bool isAccountExcel)
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 1;
            if (CurrentZone == null)
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
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                //单个任务
                if (accountLandItems == null || accountLandItems.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.CurrentZoneNoLand);
                    return;
                }
                TaskExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, null, null, markDesc, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
            else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
            {
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportContractAccountExcel, markDesc, ContractAccountInfo.ExportTable, TableType, null);
            }
        }



        /// <summary>
        /// 导出台账调查表-土地承包经营权登记公示表
        /// </summary>
        public void ExportLandRegPubTable()
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 2;
            if (CurrentZone == null)
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
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                TaskExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, null, null, ContractAccountInfo.ExportLandRegPubTable, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, ContractAccountInfo.ExportLandRegPubTable, ContractAccountInfo.ExportTable, TableType, null);
            }
            else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
            {
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportContractAccountExcel, ContractAccountInfo.ExportLandRegPubTable, ContractAccountInfo.ExportTable, TableType, null);
            }
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权签字表
        /// </summary>
        public void ExportRegSignTable()
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 3;
            if (CurrentZone == null)
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
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                TaskExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, null, null, ContractAccountInfo.ExportRegSignTable, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, ContractAccountInfo.ExportRegSignTable, ContractAccountInfo.ExportTable, TableType, null);
            }
            else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
            {
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportContractAccountExcel, ContractAccountInfo.ExportRegSignTable, ContractAccountInfo.ExportTable, TableType, null);
            }
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权村组公示表
        /// </summary>
        public void ExportVillageGroupTable()
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 4;
            if (CurrentZone == null)
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
            DateTime? delcTime = DateTime.Now;
            DateTime? pubTime = DateTime.Now;
            DoubleDateSettingPage page = new DoubleDateSettingPage();
            page.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(page, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                delcTime = page.SetDecTime;
                pubTime = page.SetPubTime;
                List<Zone> SelfAndSubsZones = new List<Zone>();
                var zoneStation = DbContext.CreateZoneWorkStation();
                int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
                {
                    TaskExportContractAccountExcel(eContractAccountType.ExportContractAccountExcel, delcTime, pubTime, ContractAccountInfo.ExportVillageGroupTable, ContractAccountInfo.ExportTable, SystemSet.DefaultPath, null, TableType);

                    //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportContractAccountExcel, ContractAccountInfo.ExportVillageGroupTable, ContractAccountInfo.ExportTable, TableType, null);
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
                {
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportContractAccountExcel, ContractAccountInfo.ExportVillageGroupTable, ContractAccountInfo.ExportTable, TableType, null);
                }
            });
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权单户确认表
        /// </summary>
        public void ExportFamilyConfirmTable()
        {
            //导出excel表业务类型，默认为承包方调查表
            int TableType = 5;
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            //获取当前选择项
            GetSelectItem();

            //批量导出
            if (currentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
            if (IsBatch)
            {
                if (currentZone.Level == eZoneLevel.Group)
                {
                    ContractAccountPersonLockPage caplp = new ContractAccountPersonLockPage();
                    List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                    foreach (var item in accountLandItems)
                    {
                        getpersonst.Add(item.Tag);
                    }
                    if (getpersonst == null) return;
                    caplp.PersonItems = getpersonst;
                    caplp.Business = PersonBusiness;
                    TheWorkPage.Page.ShowMessageBox(caplp, (b, e) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        if (caplp.selectVirtualPersons == null) return;
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportSingleFamilyConfirmExcel, ContractAccountInfo.ExportFamilyConfirmTable, ContractAccountInfo.ExportTable, TableType, caplp.selectVirtualPersons);
                    });
                }
                else
                {
                    isbatch = true;
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportSingleFamilyConfirmExcel, ContractAccountInfo.ExportFamilyConfirmTable, ContractAccountInfo.ExportTable, TableType, null);
                }
            }
            else
            {
                if (CurrentAccountItem == null)
                {
                    ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ViewDataNo);
                    return;
                }
                else
                {
                    VirtualPerson getSelectVirtualPerson = CurrentAccountItem.Tag as VirtualPerson;
                    SingleFamilyConfirmExcel(currentZone, getSelectVirtualPerson, SystemSet.DefaultPath);
                    //ShowBox(ContractAccountInfo.ExportData, "导出成功", eMessageGrade.Infomation);
                }
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户确认报表单个导出
        /// </summary>
        public void SingleFamilyConfirmExcel(Zone zone, VirtualPerson selectVirtualPerson, string fileName)
        {
            try
            {
                if (selectVirtualPerson == null)
                {
                    ShowBox("信息提示", "未选择导出数据的承包方!", eMessageGrade.Infomation);
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = new List<VirtualPerson>();

                string reInfo = string.Format("从{0}下成功导出{1}条承包方数据!", excelName, 1);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.SecondTableRealQueryExcel);
                //string zoneName = GetUinitName(zone);
                //if (SystemSetDefine.CountryTableHead)
                //{
                //    var zoneStation = DbContext.CreateZoneWorkStation();
                //    zoneName = zoneStation.GetVillageName(zone);
                //}
                List<ContractLand> landArrays = ContractAccountBusiness.GetPersonCollection(selectVirtualPerson.ID);
                var concordStation = DbContext.CreateConcordStation();
                var bookStation = DbContext.CreateRegeditBookStation();
                var listConcords = concordStation.GetContractsByZoneCode(zone.FullCode);
                var listBooks = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                landArrays.LandNumberFormat(SystemSetDefine);
                string filePath = string.Empty;
                int familyNumber = 0;
                Int32.TryParse(selectVirtualPerson.FamilyNumber, out familyNumber);
                ExportContractorSurveyExcel export = new ExportContractorSurveyExcel();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorSurveyExcel)
                        export = (ExportContractorSurveyExcel)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                string savePath = fileName + @"\" + familyNumber + "-" + selectVirtualPerson.Name + "-" + "单户确认表" + ".xls";
                export.SaveFilePath = savePath;
                export.CurrentZone = zone;
                export.Familys = vps;
                //export.UnitName = zoneName;
                export.UnitName = SystemSet.GetTableHeaderStr(zone);
                export.ExcelName = SystemSet.GetTBDWStr(zone);
                export.TableType = 5;//单户确认表-导出类型
                export.DictionList = DictList;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                export.LandArrays = landArrays;
                export.Contractor = selectVirtualPerson;
                bool result = export.BeginExcel(null, null, zone.FullCode.ToString(), tempPath);
                export.PrintView(savePath);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出台账调查表-土地承包经营权单户调查表
        /// </summary>
        public void ExportLandSingleSurveyTable()
        {
            //导出excel表业务类型，默认为承包方调查表
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            GetSelectItem();
            //批量导出
            if (currentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
            else if (!IsBatch && CurrentAccountItem == null)
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ViewDataNo);
                return;
            }
            else if (CurrentAccountItem == null && (currentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village))
            {
                isbatch = true;
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportSingleFamilySurveyExcel, ContractAccountInfo.ExportLandSingleSurveyTable, ContractAccountInfo.ExportTable);
            }
            else if (CurrentAccountItem != null && !IsBatch)
            {
                VirtualPerson getSelectVirtualPerson = CurrentAccountItem.Tag as VirtualPerson;
                SingleFamilySurveyExcel(currentZone, getSelectVirtualPerson, SystemSet.DefaultPath);
                //ExportDataPage extPage = new ExportDataPage(currentZone.FullName, TheWorkPage, ContractAccountInfo.ExportTable);
                //extPage.Workpage = TheWorkPage;
                //TheWorkPage.Page.ShowMessageBox(extPage, (c, r) =>
                //{
                //    if (string.IsNullOrEmpty(extPage.FileName) || c == false)
                //    {
                //        return;
                //    }
                //    VirtualPerson getSelectVirtualPerson = CurrentAccountItem.Tag as VirtualPerson;
                //    SingleFamilySurveyExcel(currentZone, getSelectVirtualPerson, extPage.FileName);
                //});
            }
            else if (IsBatch && currentZone.Level == eZoneLevel.Group)
            {
                ContractAccountPersonLockPage caplp = new ContractAccountPersonLockPage();
                List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                foreach (var item in accountLandItems)
                {
                    getpersonst.Add(item.Tag);
                }
                if (getpersonst == null) return;
                caplp.PersonItems = getpersonst;
                caplp.Business = PersonBusiness;
                TheWorkPage.Page.ShowMessageBox(caplp, (b, e) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (caplp.selectVirtualPersons == null) return;
                    ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportSingleFamilySurveyExcel, ContractAccountInfo.ExportLandSingleSurveyTable, ContractAccountInfo.ExportTable, 1, caplp.selectVirtualPersons);
                });
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户调查报表单个导出
        /// </summary>
        public void SingleFamilySurveyExcel(Zone zone, VirtualPerson selectVirtualPerson, string fileName)
        {
            try
            {
                if (selectVirtualPerson == null)
                {
                    ShowBox("信息提示", "未选择导出数据的承包方!", eMessageGrade.Infomation);
                    return;
                }
                string excelName = GetMarkDesc(zone);

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSingleSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                if (SystemSetDefine.CountryTableHead)
                {
                    var zoneStation = DbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(zone);
                }
                var tablePersonStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> tableVps = tablePersonStation.GetByZoneCode(zone.FullCode);
                List<ContractLand> landArrays = ContractAccountBusiness.GetPersonCollection(selectVirtualPerson.ID);
                landArrays.LandNumberFormat(SystemSetDefine);
                SecondTableLandBusiness secondlandbus = new SecondTableLandBusiness(DbContext);
                List<SecondTableLand> tableLandArrays = secondlandbus.GetCollection(selectVirtualPerson.ID);
                string filePath = string.Empty;
                int familyNumber = 0;
                Int32.TryParse(selectVirtualPerson.FamilyNumber, out familyNumber);

                using (ExportContractorLandSingleSurveyTable export = new ExportContractorLandSingleSurveyTable())
                {
                    export.SaveFilePath = fileName + @"\" + familyNumber + "-" + selectVirtualPerson.Name + "-" + "单户调查表" + ".xls";
                    export.CurrentZone = zone;
                    export.TableFamilys = tableVps;
                    export.ShowValue = false;
                    export.UnitName = zoneName;
                    export.TableLandArrays = tableLandArrays;
                    export.DictionList = DictList;
                    export.LandArrays = landArrays;
                    export.Contractor = selectVirtualPerson;
                    bool result = export.BeginToVirtualPerson(selectVirtualPerson, tempPath);
                    filePath = export.SaveFilePath;
                    if (result)
                        System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
                ShowBox("错误", "导出失败,错误详情请查看日志！");
            }
        }

        /// <summary>
        /// 导出(批量)数据汇总表
        /// </summary>
        public void ExportSummaryExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))
            {
                TaskExportSummaryExcel(eContractAccountType.ExportSummaryExcel, ContractAccountInfo.ExportSummaryDataExcel, ContractAccountInfo.ExportTable, SystemSet.DefaultPath);
                //ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.ExportSummaryExcel, ContractAccountInfo.ExportSummaryDataExcel, ContractAccountInfo.ExportTable, 1, null);
            }
            else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZonesCount > 0)
            {
                ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportTable, eContractAccountType.VolumnExportSummaryExcel, ContractAccountInfo.ExportSummaryDataExcel, ContractAccountInfo.ExportTable, 1, null);
            }
            else
            {
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
        }

        /// <summary>
        /// 地块类别汇总表
        /// </summary>
        public void ExportCategorySummary()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportData, ContractAccountInfo.ExportNoZone);
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的
            if (currentZone.Level > eZoneLevel.County)
            {
                ShowBox(ContractAccountInfo.ExportData, "请选择县或县以下级别地域进行导出");
                return;
            }
            else
            {
                TaskExportCategorySummaryExcel();
            }
        }

        /// <summary>
        /// 导出(批量)村组公告表(Word)
        /// </summary>
        public void ExportVillagesDeclare()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportVillagesDeclare, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            SetDateInfoForVillagesDeclare();
        }

        /// <summary>
        /// 导出村组公示公告表Word
        /// </summary>
        public void VillagesDeclareWord(Zone zone, string filePath, DateSetting dateSetting = null)
        {
            try
            {
                if (zone == null)
                {
                    ShowBox("信息提示", "未选择导出数据的地域!", eMessageGrade.Infomation);
                    return;
                }
                string excelName = GetMarkDesc(zone);
                ContractAccountBusiness.VirtualType = eVirtualType.Land;
                List<VirtualPerson> listVp = ContractAccountBusiness.GetByZone(zone.FullCode);
                List<ContractLand> listLand = ContractAccountBusiness.GetCollection(zone.FullCode, eLevelOption.Self);
                if (listVp == null || listVp.Count == 0)
                {
                    ShowBox("信息提示", "当前地域下无数据!", eMessageGrade.Infomation);
                    return;
                }
                string statueDes = excelName + "公示公告";
                string savePath = filePath + @"\" + excelName + TemplateFile.AnnouncementWord + ".doc";   //保存文件全路径
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.AnnouncementWord);  //模板路径
                ExportAnnouncementWord exportWord = new ExportAnnouncementWord(DbContext);
                exportWord.CurrentZone = zone;
                exportWord.DateSettingForAnnoucementWord = dateSetting;
                exportWord.ListPerson = listVp;
                exportWord.ListLand = listLand;
                if (exportWord.OpenTemplate(templatePath))   //打开模板
                    exportWord.SaveAs(zone, savePath);
                listVp = null;
                listLand = null;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVillagesDeclareWord(导出村组公示公告Word)", ex.Message + ex.StackTrace);
            }
        }

        #endregion Method-台账导出

        #region Method-导出地域下Shape数据

        /// <summary>
        /// 导出地域下地块图斑Shape数据
        /// </summary>
        public void ExportLandShapeData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            ExportContractLandShapePage exportPage = new ExportContractLandShapePage(TheWorkPage, "导出当前地域下地块Shape数据");
            TheWorkPage.Page.ShowMessageBox(exportPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                List<Zone> allZones = new List<Zone>();
                allZones = GetAllChildrenZones() as List<Zone>;
                List<Zone> allChildrenZones = new List<Zone>();
                allChildrenZones = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                IDbContext dbContext = CreateDb();
                var vpstation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> vps = vpstation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);

                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                {
                    //执行单个任务
                    TaskExportLandShapeArgument meta = new TaskExportLandShapeArgument();
                    meta.CurrentZone = currentZone;
                    meta.FileName = exportPage.FileName;
                    meta.Database = dbContext;
                    meta.DictList = DictList;
                    meta.vps = vps;
                    var import = new TaskExportLandShapeOperation();
                    import.Argument = meta;
                    import.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandShapeData;
                    import.Name = ContractAccountInfo.ExportLandShapeData;
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(import);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    import.StartAsync();
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZones.Count > 0)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupExportLandShapeArgument groupMeta = new TaskGroupExportLandShapeArgument();
                    groupMeta.CurrentZone = currentZone;
                    groupMeta.AllZones = allZones;
                    groupMeta.Database = DbContext;
                    groupMeta.vps = vps;
                    groupMeta.FileName = exportPage.FileName;
                    groupMeta.DictList = DictList;
                    TaskGroupExportLandShapeOperation taskGroup = new TaskGroupExportLandShapeOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandShapeData;
                    taskGroup.Name = ContractAccountInfo.ExportLandShapeData;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(taskGroup);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.ExportZoneError);
                    return;
                }
            });
        }

        /// <summary>
        /// 导出地域下地块界址点Shape数据
        /// </summary>
        public void ExportLandDotShapeData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandDotShapeData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            ExportDataPage exportPage = new ExportDataPage(CurrentZone.FullName, TheWorkPage, "导出当前地域下地块界址点Shape数据");
            TheWorkPage.Page.ShowMessageBox(exportPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                List<Zone> allZones = new List<Zone>();
                allZones = GetAllChildrenZones() as List<Zone>;
                List<Zone> allChildrenZones = new List<Zone>();
                allChildrenZones = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                IDbContext dbContext = CreateDb();
                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                {
                    //执行单个任务
                    var meta = new TaskExportLandDotShapeArgument();
                    meta.CurrentZone = currentZone;
                    meta.FileName = exportPage.FileName;
                    meta.Database = dbContext;
                    meta.DictList = DictList;
                    var import = new TaskExportLandDotShapeOperation();
                    import.Argument = meta;
                    import.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandDotShapeData;
                    import.Name = ContractAccountInfo.ExportLandDotShapeData;
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDDOTSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(import);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    import.StartAsync();
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZones.Count > 0)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupExportLandDotShapeArgument groupMeta = new TaskGroupExportLandDotShapeArgument();
                    groupMeta.CurrentZone = currentZone;
                    groupMeta.AllZones = allZones;
                    groupMeta.Database = DbContext;
                    groupMeta.FileName = exportPage.FileName;
                    groupMeta.DictList = DictList;
                    TaskGroupExportLandDotShapeOperation taskGroup = new TaskGroupExportLandDotShapeOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandDotShapeData;
                    taskGroup.Name = ContractAccountInfo.ExportLandDotShapeData;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDDOTSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(taskGroup);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox(ContractAccountInfo.ExportLandDotShapeData, ContractAccountInfo.ExportZoneError);
                    return;
                }
            });
        }

        /// <summary>
        /// 导出地域下地块界址线Shape数据
        /// </summary>
        public void ExportLandCoilShapeData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            ExportDataPage exportPage = new ExportDataPage(CurrentZone.FullName, TheWorkPage, "导出当前地域下地块界址线Shape数据");
            TheWorkPage.Page.ShowMessageBox(exportPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                List<Zone> allZones = new List<Zone>();
                allZones = GetAllChildrenZones() as List<Zone>;
                List<Zone> allChildrenZones = new List<Zone>();
                allChildrenZones = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                IDbContext dbContext = CreateDb();
                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                {
                    //执行单个任务
                    TaskExportLandCoilShapeArgument meta = new TaskExportLandCoilShapeArgument();
                    meta.CurrentZone = currentZone;
                    meta.FileName = exportPage.FileName;
                    meta.Database = dbContext;
                    meta.DictList = DictList;
                    TaskExportLandCoilShapeOperation import = new TaskExportLandCoilShapeOperation();
                    import.Argument = meta;
                    import.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandCoilShapeData;
                    import.Name = ContractAccountInfo.ExportLandCoilShapeData;
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDCOILSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(import);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    import.StartAsync();
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZones.Count > 0)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupExportLandCoilShapeArgument groupMeta = new TaskGroupExportLandCoilShapeArgument();
                    groupMeta.CurrentZone = currentZone;
                    groupMeta.AllZones = allZones;
                    groupMeta.Database = DbContext;
                    groupMeta.FileName = exportPage.FileName;
                    groupMeta.DictList = DictList;
                    TaskGroupExportLandCoilShapeOperation taskGroup = new TaskGroupExportLandCoilShapeOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandCoilShapeData;
                    taskGroup.Name = ContractAccountInfo.ExportLandCoilShapeData;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDCOILSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    TheWorkPage.TaskCenter.Add(taskGroup);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox(ContractAccountInfo.ExportLandCoilShapeData, ContractAccountInfo.ExportZoneError);
                    return;
                }
            });
        }

        #endregion Method-导出地域下Shape数据

        #endregion Method-导出数据

        #region Method-地籍数据处理

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        public void BoundaryAddressDotResultExport()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            List<Zone> allZones = new List<Zone>();
            allZones = GetAllChildrenZones() as List<Zone>;
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            List<ContractLand> listGeoLand = new List<ContractLand>();
            List<ContractLand> listAllLand = new List<ContractLand>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            var dbContext = CreateDb();
            try
            {
                if (dbContext == null)
                    return;
                var landStation = dbContext.CreateContractLandWorkstation();
                listAllLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                listGeoLand = listAllLand.FindAll(c => c.Shape != null);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包地块数据失败)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportDotResultExcel, string.Format("获取{0}下的承包地块数据失败(包括子级地域)", currentZone.FullName));
                return;
            }
            try
            {
                if (dbContext == null)
                    return;
                var personStaion = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                listPerson = personStaion.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByZoneCode(获取承包方数据失败)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportDotResultExcel, string.Format("获取{0}下的承包方数据失败(包括子级地域)", currentZone.FullName));
                return;
            }
            if (listGeoLand == null || listGeoLand.Count == 0)
            {
                //地域下没有空间地块
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.SubAndSelfZoneNoGeoLand);
                return;
            }
            if (!IsBatch)
            {
                if (currentLandBinding == null)
                {
                    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.ViewDataNo);
                    return;
                }
                else if (currentAccountItem != null && currentLandBinding != null)
                {
                    //选中承包地块项(若是空间地块则导出直接预览界址点成果表)
                    if (currentLandBinding.Tag.Shape == null)
                    {
                        //不是空间地块
                        ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.FindNoGeometryUnderCurrentSelected);
                        return;
                    }
                    else
                    {
                        ContractAccountBusiness.ExportDotResultExcel(currentZone, currentLandBinding.Tag, SystemSet.DefaultPath);
                    }
                }
            }
            else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
            {
                //批量导出(选择地域大于组级并且当前地域下有子级地域)
                ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, TheWorkPage, ContractAccountInfo.ExportDotResultExcel);
                TheWorkPage.Page.ShowMessageBox(exportPage, (x, y) =>
                {
                    if (!(bool)x || string.IsNullOrEmpty(exportPage.FileName))
                    {
                        return;
                    }
                    //批量导出任务(三级任务)
                    ExportDotResultTaskTopGroup(listPerson, listGeoLand, exportPage.FileName, allZones);
                });
            }
            else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
            {
                var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                int vpscount = vpStation.CountByZone(currentZone.FullCode);
                if (vpscount == 0)
                {
                    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoPersonData);
                    return;
                }

                //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                if (accountLandItems == null || accountLandItems.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoPersonData);
                    return;
                }
                if (currentAccountItem != null || currentLandBinding != null)
                {
                    //选中承包方没选中地块项
                    var currentListGeoLand = listGeoLand.FindAll(c => c.OwnerId == currentAccountItem.Tag.ID);
                    if (currentListGeoLand == null || currentListGeoLand.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentPersonNoGeoLands);
                        return;
                    }
                    ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, TheWorkPage, ContractAccountInfo.ExportDotResultExcel);
                    TheWorkPage.Page.ShowMessageBox(exportPage, (b, r) =>
                    {
                        if (!(bool)b || string.IsNullOrEmpty(exportPage.FileName))
                        {
                            return;
                        }
                        //单个任务处理
                        ExportDotResultTask(exportPage.FileName, currentListGeoLand);
                    });
                }
                //else if (currentAccountItem != null && currentLandBinding != null)
                //{
                //    //选中承包地块项(若是空间地块则导出直接预览界址点成果表)
                //    if (currentLandBinding.Tag.Shape == null)
                //    {
                //        //不是空间地块
                //        ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.FindNoGeometryUnderCurrentSelected);
                //        return;
                //    }
                //    else
                //    {
                //        ContractAccountBusiness.ExportDotResultExcel(currentZone, currentLandBinding.Tag);
                //    }
                //}
                else
                {
                    //界面上没有选中项(此时弹出承包方选择界面)
                    List<VirtualPerson> persons = new List<VirtualPerson>();
                    foreach (var item in accountLandItems)
                    {
                        var chilren = item.Children;
                        if (chilren.Any(c => c.Tag.Shape != null))
                            persons.Add(item.Tag);
                    }
                    ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                    selectPage.Workpage = TheWorkPage;
                    selectPage.PersonItems = persons;
                    selectPage.Business = PersonBusiness;
                    TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, TheWorkPage, ContractAccountInfo.ExportDotResultExcel);
                        TheWorkPage.Page.ShowMessageBox(exportPage, (m, n) =>
                        {
                            if (!(bool)m || string.IsNullOrEmpty(exportPage.FileName))
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.ExportDotResultExcelNoSelectedPerson);
                                return;
                            }
                            //批量任务处理(两级任务)
                            ExportDotResultTaskGroup(selectedPersons, listGeoLand, exportPage.FileName);
                        });
                    });
                }
            }
            else
            {
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
        }

        /// <summary>
        /// 批量导出界址点成果表任务(三级任务)
        /// </summary>
        /// <param name="listPerson">承包方集合(当前地域和其子级地域下)</param>
        /// <param name="listGeoLand">空间地块集合(当前地域和子级地域下)</param>
        /// <param name="filName">保存文件路径</param>
        /// <param name="allZones">所有地域集合(当前地域及其子级地域)</param>
        private void ExportDotResultTaskTopGroup(List<VirtualPerson> listPerson, List<ContractLand> listGeoLand, string filName, List<Zone> allZones)
        {
            IDbContext dbContext = CreateDb();
            TaskTopGropExportDotResultArgument groupTopArguemnt = new TaskTopGropExportDotResultArgument();
            groupTopArguemnt.ListPerson = listPerson;
            groupTopArguemnt.ListGeoLand = listGeoLand;
            groupTopArguemnt.CurrentZone = CurrentZone;
            groupTopArguemnt.Database = dbContext;
            groupTopArguemnt.FileName = filName;
            groupTopArguemnt.VirtualType = virtualType;
            groupTopArguemnt.AllZones = allZones;
            TaskTopGroupExportDotResultOperation groupTopOperation = new TaskTopGroupExportDotResultOperation();
            groupTopOperation.Argument = groupTopArguemnt;
            groupTopOperation.Name = ContractAccountInfo.ExportDotResultExcel;
            groupTopOperation.Description = currentZone.FullName;
            groupTopOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupTopOperation);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            groupTopOperation.StartAsync();
        }

        /// <summary>
        /// 批量导出界址点成果表任务(两级任务)
        /// </summary>
        /// <param name="selectedPersons">选中的承包方</param>
        /// <param name="listGeoLand">空间地块集合(当前地域和子级地域下)</param>
        /// <param name="fileName">保存文件路径</param>
        private void ExportDotResultTaskGroup(List<VirtualPerson> selectedPersons, List<ContractLand> listGeoLand, string fileName)
        {
            IDbContext dbContext = CreateDb();
            TaskGroupExportDotResultArgument groupArgument = new TaskGroupExportDotResultArgument();
            groupArgument.ListPerson = selectedPersons;
            groupArgument.ListGeoLand = listGeoLand;
            groupArgument.CurrentZone = currentZone;
            groupArgument.Database = dbContext;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = virtualType;
            TaskGroupExportDotResultOperation groupOperation = new TaskGroupExportDotResultOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Name = ContractAccountInfo.ExportDotResultExcel;
            groupOperation.Description = currentZone.FullName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出界址点成果表任务
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="currentListGeoLand">当前承包方下所有空间地块</param>
        private void ExportDotResultTask(string fileName, List<ContractLand> currentListGeoLand)
        {
            IDbContext dbContext = CreateDb();
            TaskExportDotResultArgument argument = new TaskExportDotResultArgument();
            argument.Database = dbContext;
            argument.CurrentZone = CurrentZone;
            argument.FileName = fileName;
            argument.ListGeoLand = currentListGeoLand;
            argument.CurrentPerson = currentAccountItem.Tag;
            TaskExportDotResultOperation opertion = new TaskExportDotResultOperation();
            opertion.Argument = argument;
            opertion.Name = ContractAccountInfo.ExportDotResultExcel;
            opertion.Description = ContractAccountInfo.ExportDotResultExcel;
            opertion.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(opertion);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            opertion.StartAsync();
        }

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        public void MultiParcelExport()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            try
            {
                //如果选中的是确股地块，则不能使用此功能使用-使用确股查件要在专门的确股模块里面看确股模板的图
                if (CurrentLandBinding != null && CurrentLandBinding.IsStockLand == true)
                {
                    ShowBox("导出地块示意图", "请不要选择确股地块导出");
                    return;
                }

                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (!IsBatch)
                {
                    if (currentAccountItem == null)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ViewDataNo);
                        return;
                    }
                    var landStation = DbContext.CreateContractLandWorkstation();
                    var geoLands = ContractLandHeler.GetParcelLands(currentZone.FullCode, DbContext, ParcelWordSettingDefine.ContainsOtherZoneLand);// landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                    if (geoLands == null || geoLands.Count == 0)
                    {
                        //当前地域没有空间地块数据
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                        return;
                    }
                    // 界面上有当前选择承包方项(此时做预览操作),预览确权的地块时，要排除掉股地
                    List<ContractLand> geoLandsOfFamily = geoLands.FindAll(c => c.OwnerId == CurrentAccountItem.Tag.ID && !c.IsStockLand);
                    if (geoLandsOfFamily == null || geoLandsOfFamily.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLands);
                        return;
                    }
                    var geoLandOfFamily = InitalizeAgricultureLandSortValue(geoLandsOfFamily);
                    if (geoLandOfFamily == null || geoLandOfFamily.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLandsBySetting);
                        return;
                    }
                    var confirmPage = new ConfirmPage(TheWorkPage, ContractAccountInfo.PreviewMultiParcelOfFamily,
                        string.Format("是否预览{0}地块示意图?", CurrentAccountItem.Tag.Name));
                    confirmPage.Confirm += (a, c) =>
                    {
                        try
                        {
                            string fileName = SystemSet.DefaultPath;
                            ContractAccountBusiness.ExportMultiParcelWord(currentZone, geoLands, CurrentAccountItem.Tag, fileName, false);
                        }
                        catch (Exception ex)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ex.Message);
                            return;
                        }
                    };
                    TheWorkPage.Page.ShowMessageBox(confirmPage, (b, r) =>
                    {
                    });
                }
                else
                {
                    if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                    {
                        //批量导出(选择地域大于组级并且当前地域下有子级地域)
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.VolumnExportMultiParcelOfFamily,
                            ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily);
                    }
                    else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                    {
                        //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                        var landStation = DbContext.CreateContractLandWorkstation();

                        List<VirtualPerson> listPerson = new List<VirtualPerson>();
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = TheWorkPage;
                        foreach (var item in accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            var geoLands = ContractLandHeler.GetParcelLands(currentZone.FullCode, DbContext, ParcelWordSettingDefine.ContainsOtherZoneLand);
                            // landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                            if (geoLands == null || geoLands.Count == 0)
                            {
                                //当前地域没有空间地块数据
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelNoSelected);
                                return;
                            }
                            ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.ExportMultiParcelOfFamily,
                         ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily, 1, selectPage.SelectedPersons, false, geoLands);
                        });
                    }
                    else
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.VolumnExportZoneError);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "MultiParcelExport(导出地块示意图)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyFail);
                return;
            }
        }

        /// <summary>
        /// 导出确权确股地块示意图
        /// </summary>
        public void MultiParcelStockExport()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (!IsBatch)
                {
                    if (currentAccountItem == null)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ViewDataNo);
                        return;
                    }
                    var landStation = DbContext.CreateContractLandWorkstation();
                    var geoLands = landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                    if (geoLands == null || geoLands.Count == 0)
                    {
                        //当前地域没有空间地块数据
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                        return;
                    }
                    //界面上有当前选择承包方项(此时做预览操作)
                    List<ContractLand> geoLandsOfFamily = geoLands.FindAll(c => c.OwnerId == CurrentAccountItem.Tag.ID);
                    var belongRelationStation = DbContext.CreateBelongRelationWorkStation();
                    var landList = belongRelationStation.GetLandByPerson(CurrentAccountItem.Tag.ID, CurrentAccountItem.Tag.ZoneCode);
                    if (geoLandsOfFamily == null || geoLandsOfFamily.Count == 0)
                    {
                        if (landList == null || landList.Count == 0)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLands);
                            return;
                        }
                    }
                    var geoLandOfFamily = InitalizeAgricultureLandSortValue(geoLandsOfFamily);
                    var lands = InitalizeAgricultureLandSortValue(landList);
                    if (geoLandOfFamily == null || geoLandOfFamily.Count == 0)
                    {
                        if (lands == null || lands.Count == 0)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLandsBySetting);
                            return;
                        }
                    }
                    var confirmPage = new ConfirmPage(TheWorkPage, ContractAccountInfo.PreviewMultiParcelOfFamily,
                        string.Format("是否预览{0}地块示意图?", CurrentAccountItem.Tag.Name));
                    confirmPage.Confirm += (a, c) =>
                    {
                        try
                        {
                            string fileName = SystemSet.DefaultPath;
                            ContractAccountBusiness.ExportMultiParcelWord(currentZone, landList, CurrentAccountItem.Tag, fileName, false, "", null);
                        }
                        catch (Exception ex)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ex.Message);
                            return;
                        }
                    };
                    TheWorkPage.Page.ShowMessageBox(confirmPage, (b, r) =>
                    {
                    });
                }
                else
                {
                    if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                    {
                        //批量导出(选择地域大于组级并且当前地域下有子级地域)
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.VolumnExportMultiParcelOfFamily,
                            ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily, 1, null, null);
                    }
                    else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
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
                        List<VirtualPerson> listPerson = new List<VirtualPerson>();
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = TheWorkPage;
                        foreach (var item in accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        TheWorkPage.Page.ShowMessageBox(selectPage, (b, r) =>
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
                            ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.ExportMultiParcelOfFamily,
                                ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily, 1, selectPage.SelectedPersons, null);
                        });
                    }
                    else
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.VolumnExportZoneError);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "MultiParcelExport(导出地块示意图)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyFail);
                return;
            }
        }

        /// <summary>
        /// 按照设置进行地块类别筛选导出
        /// </summary>
        private List<ContractLand> InitalizeAgricultureLandSortValue(List<ContractLand> geoLandCollection)
        {
            if (geoLandCollection.Count == 0) return new List<ContractLand>();
            if (ParcelWordSettingDefine.ExportContractLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportPrivateLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportMotorizeLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportWasteLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.WasteLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportCollectiveLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.CollectiveLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportEncollecLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.EncollecLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportFeedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.FeedLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportAbandonedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.AbandonedLand).ToString());
            }
            return geoLandCollection;
        }

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        private void ExportMultiParcelTask(string fileName, List<VirtualPerson> selectedPersons, string taskDes,
            string taskName, bool? isStockLand, List<ContractLand> vlands)
        {
            TaskExportMultiParcelWordArgument argument = new TaskExportMultiParcelWordArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.SelectedPersons = selectedPersons;
            TaskExportMultiParcelWordOperation operation = new TaskExportMultiParcelWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.VillageContractLands = vlands;
            if (isStockLand != null)
                operation.Name = (bool)isStockLand ? "导出确股地块示意图" : taskName;
            else
                operation.Name = "导出确权确股地块示意图";
            operation.IsStockLand = isStockLand;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出地块示意图(多个任务)
        /// </summary>
        private void ExportMultiParcelTaskGroup(string fileName, string taskName, string taskDesc, bool? isStockLand)
        {
            TaskGroupExportMultiParcelWordArgument groupArgument = new TaskGroupExportMultiParcelWordArgument();
            groupArgument.CurrentZone = currentZone;
            groupArgument.DbContext = DbContext;
            groupArgument.FileName = fileName;
            TaskGroupExportMultiParcelWordOperation groupOperation = new TaskGroupExportMultiParcelWordOperation();
            groupOperation.Argument = groupArgument;
            if (isStockLand != null)
                groupOperation.Name = (bool)isStockLand ? "导出确股地块示意图" : taskName;
            else
                groupOperation.Name = "导出确权确股地块示意图";
            groupOperation.Description = taskDesc;
            groupOperation.IsStockLand = isStockLand;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            groupOperation.StartAsync();
        }

        #endregion Method-地籍数据处理

        #region Method-工具

        /// <summary>
        /// 显示所有的地块
        /// </summary>
        public void AllLandFilter()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.AllLandVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (accountLandItems == null || accountLandItems.Count() == 0)
                return;
            LandFilterTool("");
        }

        /// <summary>
        /// 显示对于承包地块
        /// </summary>
        public void LandTypeFilter(string type)
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandFilterTool(type);
        }

        /// <summary>
        /// 初始化承包地块属性信息
        /// </summary>
        public void InitialLandInfo()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractAccountInfo.InitialLandInfo, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (accountLandItems != null && accountLandItems.Count > 0 && !accountLandItems.Any(c => c.Tag.Status == eVirtualPersonStatus.Right))
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, "地域下的所有承包方数据都被锁定,无法执行初始化操作!", eMessageGrade.Warn);
                return;
            }

            IDbContext dbContext = this.DbContext;
            List<Zone> allZones = new List<Zone>();
            allZones = GetAllChildrenZones() as List<Zone>;
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            landList = ContractAccountBusiness.GetCollection(CurrentZone.FullCode, eLevelOption.Self);

            if ((currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) && (landList == null || landList.Count == 0))
            {
                //选择地域为组级地域或者大于组级地域时没有子级地域
                //没有地块信息
                ShowBox(ContractAccountInfo.InitialLandInfo, ContractAccountInfo.CurrentZoneNoLand);
                return;
            }

            //if (CurrentZone.Level > eZoneLevel.Town)
            //{
            //    ShowBox(ContractAccountInfo.InitialLandInfo, ContractAccountInfo.InitialColumnSelectedZoneError);
            //    return;
            //}
            bool isBatch = currentZone.Level > eZoneLevel.Group ? true : false;
            ContractLandInitializePage initialPage = new ContractLandInitializePage(isBatch);
            initialPage.Workpage = TheWorkPage;
            initialPage.LandBusiness = ContractAccountBusiness;
            initialPage.CurrentZone = CurrentZone;
            initialPage.VillageInlitialSet = SystemSetDefine.VillageInlitialSet;
            TheWorkPage.Page.ShowMessageBox(initialPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }

                if (initialPage.InitialLandNeighborInfo && CurrentZone.Level < eZoneLevel.County)
                {
                    InitialLandNeigborInfoTask();
                }
                else if (initialPage.InitialLandNeighborInfo && CurrentZone.Level == eZoneLevel.County)
                {
                    InitialLandNeigborInfoGroupTask();
                }
                else
                {
                    if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                    {
                        //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                        InitialLandInfoTask(initialPage, landList);
                    }
                    else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                    {
                        //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                        InitialLandInfoTaskGroup(initialPage, allZones);
                    }
                }
            });
        }

        #region Method-工具-初始化界址点线

        /// <summary>
        /// 初始化承包地块界址点线信息 2018-3-22  优化初始化界面修改
        /// </summary>
        public void InitializeLandDotCoilInfo()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            var dotCoilPage = new ContractLandInitializeDotCoil(TheWorkPage);
            dotCoilPage.CurrentZone = currentZone;
            if (CurrentLandBinding != null && CurrentLandBinding.Tag != null)
            {
                //界面上有选择承包地块
                var action = new Action<bool?, eCloseReason>((m, n) =>
                {
                    if (!(bool)m)
                    {
                        if (n == eCloseReason.Confirm)
                            InitialLandBoundaryDialog(dotCoilPage);
                    }
                    else
                    {
                        if (CurrentAccountItem != null)
                        {
                            bool isLock = CurrentAccountItem.Tag.Status == eVirtualPersonStatus.Lock ? true : false;
                            if (isLock)
                            {
                                ShowBox(ContractAccountInfo.InitialLandDotCoil, "检测到当前地块被锁定,无法进行界址信息初始化操作!", eMessageGrade.Warn);
                                return;
                            }
                        }
                        dotCoilPage.CurrentLand = CurrentLandBinding.Tag;
                        dotCoilPage.cbSetProperty.IsChecked = false;
                        if (CurrentLandBinding.Tag != null && CurrentLandBinding.Tag.Shape == null)
                        {
                            ShowBox(ContractAccountInfo.InitialLandCoil, "当前选定地块数据无空间信息!", eMessageGrade.Warn);
                            return;
                        }
                        InitialSingleLandBoundaryDialog(dotCoilPage);
                    }
                });
                ShowBoxAction(ContractAccountInfo.InitialLandDotCoil, "检测当前有选中承包地块,是否仅生成单个地块界址信息？", action);
            }
            else
            {
                InitialLandBoundaryDialog(dotCoilPage);
            }
        }

        /// <summary>
        /// 初始化单个地块界址信息对话框
        /// </summary>
        private void InitialSingleLandBoundaryDialog(ContractLandInitializeDotCoil dotCoilPage)
        {
            TheWorkPage.Page.ShowMessageBox(dotCoilPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    var arg = CreateArgumentBySet(dotCoilPage, true, null, dotCoilPage.CurrentLand);
                    var importDot = new InitializeLandDotCoilTask();
                    importDot.Argument = arg;
                    importDot.SingleContractLandInitialTool();
                    ShowBox("初始化单个地块界址信息", string.Format("成功初始化地块编码：{0}的界址信息", CurrentLandBinding.Tag.LandNumber), eMessageGrade.Infomation);
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "InitialSingleLandBoundaryDialog(初始化单个承包地块界址点和界址线)", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.ContractAccountInitializeDotAndCoilFail);
                    return;
                }
            });
        }

        /// <summary>
        /// 初始化界址信息对话框
        /// </summary>
        private void InitialLandBoundaryDialog(ContractLandInitializeDotCoil dotCoilPage)
        {
            TheWorkPage.Page.ShowMessageBox(dotCoilPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = CreateDb();
                try
                {
                    if (currentZone.Level > eZoneLevel.County)
                    {
                        //选择地域为镇(或大于镇)
                        ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.VolumnImportErrorCountyZone);
                        return;
                    }
                    var landStation = dbContext.CreateContractLandWorkstation();
                    int listGeoLand = 0;
                    if (currentZone.Level == eZoneLevel.Group)
                        listGeoLand = landStation.Count(t => t.Shape != null && t.ZoneCode.Equals(currentZone.FullCode));
                    else
                        listGeoLand = landStation.Count(t => t.Shape != null && t.ZoneCode.StartsWith(currentZone.FullCode));
                    //选择地域为组级地域或者大于组级地域时没有子级地域
                    if (listGeoLand == 0)
                    {
                        //没有空间地块信息
                        ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.CurrentZoneNoGeoLand);
                        return;
                    }
                    //单个任务
                    InitializeLandDotCoilInfoTask(dbContext, dotCoilPage);
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "InitialLandBoundaryDialog(初始化承包地块界址点和界址线)", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.ContractAccountInitializeDotAndCoilFail);
                    return;
                }
            });
        }

        /// <summary>
        /// 初始化承包地块界址点线信息(批量初始化)
        /// </summary>
        private void InitializeLandDotCoilInfoTask(IDbContext dbContext, ContractLandInitializeDotCoil dotCoilPage)
        {
            if (dbContext == null || dotCoilPage == null)
                return;
            var meta = CreateArgumentBySet(dotCoilPage, false, dbContext);
            var initialize = new TaskInitializeLandDotCoilOperation();
            initialize.Argument = meta;
            initialize.Description = ContractAccountInfo.InitialLandDotCoil;
            initialize.Name = ContractAccountInfo.InitialLandDotCoil;
            initialize.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_INITIALDOTCOIL_COMPLETE, "", CurrentZone.FullCode);
                TheBns.Current.Message.Send(this, args);
            });
            TheWorkPage.TaskCenter.Add(initialize);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            initialize.StartAsync();
        }

        ///// <summary>
        ///// 批量初始化承包地块界址点线信息(组任务)
        ///// </summary>
        //private void InitializeLandDotCoilInfoTaskGroup(IDbContext dbContext, ContractLandInitializeDotCoil dotCoilPage)
        //{
        //    if (dbContext == null || dotCoilPage == null)
        //        return;
        //    var groupMeta = CreateArgumentBySet(dotCoilPage, false, new List<ContractLand>(), dbContext);
        //    var taskGroup = new TaskGroupInitializeLandDotCoilOperation();
        //    taskGroup.Argument = groupMeta;
        //    taskGroup.Description = ContractAccountInfo.InitialLandDotCoil;
        //    taskGroup.Name = ContractAccountInfo.InitialLandDotCoil;
        //    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
        //    {
        //        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, "", CurrentZone.FullCode);
        //        TheBns.Current.Message.Send(this, args);
        //    });
        //    TheWorkPage.TaskCenter.Add(taskGroup);
        //    if (ShowTaskViewer != null)
        //        ShowTaskViewer();
        //    taskGroup.StartAsync();
        //}

        /// <summary>
        /// 根据设置获取参数
        /// </summary>
        private TaskInitializeLandDotCoilArgument CreateArgumentBySet(ContractLandInitializeDotCoil dotCoilPage,
            bool IsSingled, IDbContext dbContext = null, ContractLand singleLand = null)
        {
            var meta = new TaskInitializeLandDotCoilArgument();
            meta.CurrentZone = currentZone;
            meta.Database = dbContext == null ? CreateDb() : dbContext;
            //meta.CurrentZoneLandList = listGeoLand;
            meta.InstallArg = dotCoilPage.InstallArg;
            meta.SetArg = dotCoilPage.SetArg;
            meta.IsInstall = dotCoilPage.IsSetInstallArg;
            meta.IsValueSet = dotCoilPage.IsSetProperty;
            meta.IsSingleLand = IsSingled;
            meta.SingleLand = singleLand;
            return meta;
        }

        #endregion Method-工具-初始化界址点线

        #region Method-工具-根据有效点初始化界址线

        /// <summary>
        /// 初始化承包地块界址线信息
        /// </summary>
        public void InitializeLandCoilInfo()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.InitialLandCoil, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            ContractLandInitializeCoil dotCoilPage = new ContractLandInitializeCoil(TheWorkPage);
            dotCoilPage.CurrentZone = currentZone;
            if (CurrentLandBinding != null && CurrentLandBinding.Tag != null)
            {
                //界面上有选择承包地块
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((m, n) =>
                {
                    if (!(bool)m)
                    {
                        InitialLandCoilDialog(dotCoilPage);
                    }
                    else
                    {
                        if (CurrentAccountItem != null)
                        {
                            bool isLock = CurrentAccountItem.Tag.Status == eVirtualPersonStatus.Lock ? true : false;
                            if (isLock)
                            {
                                ShowBox(ContractAccountInfo.InitialLandCoil, "检测到当前地块被锁定,无法进行界址信息初始化操作!", eMessageGrade.Warn);
                                return;
                            }
                        }
                        if (CurrentLandBinding.Tag != null && CurrentLandBinding.Tag.Shape == null)
                        {
                            ShowBox(ContractAccountInfo.InitialLandCoil, "当前选定地块数据无空间信息!", eMessageGrade.Warn);
                            return;
                        }
                        try
                        {
                            var dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
                            var validDots = dotStation.GetByLandID(currentLandBinding.Tag.ID, true);
                            if (validDots == null || validDots.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.InitialLandCoil, "当前选定地块数据无有效界址点信息!", eMessageGrade.Warn);
                                return;
                            }
                        }
                        catch (Exception)
                        {
                            ShowBox(ContractAccountInfo.InitialLandCoil, "获取选定地块的有效界址点信息失败!", eMessageGrade.Error);
                            return;
                        }
                        dotCoilPage.gpInitial.Visibility = Visibility.Collapsed;
                        dotCoilPage.CurrentLand = CurrentLandBinding.Tag;
                        dotCoilPage.CurrentGeoLand = CurrentLandBinding.Tag.Shape;
                        InitialSingleLandCoilDialog(dotCoilPage);
                    }
                });
                ShowBox(ContractAccountInfo.InitialLandCoil, "检测当前有选中承包地块,是否仅生成单个地块有效界址线信息？", eMessageGrade.Infomation, action);
            }
            else
            {
                InitialLandCoilDialog(dotCoilPage);
            }
        }

        /// <summary>
        /// 初始化单个地块界址信息对话框
        /// </summary>
        private void InitialSingleLandCoilDialog(ContractLandInitializeCoil dotCoilPage)
        {
            TheWorkPage.Page.ShowMessageBox(dotCoilPage, (b, r) =>
            {
                if (!(bool)b)
                    return;
                try
                {
                    var argument = dotCoilPage.GetArgument();
                    InitializeLandCoil importDot = new InitializeLandCoil();
                    importDot.Argument = argument;
                    importDot.ContractLandInitialTool();
                    ShowBox("初始化单个地块界址信息", string.Format("成功初始化地块编码：{0}的界址信息", CurrentLandBinding.Tag.LandNumber), eMessageGrade.Infomation);
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "InitialSingleLandBoundaryDialog(初始化单个承包地块界址点和界址线)", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.ContractAccountInitializeDotAndCoilFail);
                    return;
                }
            });
        }

        /// <summary>
        /// 初始化界址信息对话框
        /// </summary>
        private void InitialLandCoilDialog(ContractLandInitializeCoil CoilPage)
        {
            TheWorkPage.Page.ShowMessageBox(CoilPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = CreateDb();
                try
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                    var landStation = dbContext.CreateContractLandWorkstation();
                    List<ContractLand> initLandList = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                    List<ContractLand> listGeoLand = initLandList == null ? new List<ContractLand>() : initLandList.FindAll(c => c.Shape != null);
                    if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                    {
                        //选择地域为组级地域或者大于组级地域时没有子级地域
                        if (listGeoLand == null || listGeoLand.Count == 0)
                        {
                            //没有空间地块信息
                            ShowBox(ContractAccountInfo.InitialLandCoil, ContractAccountInfo.CurrentZoneNoGeoLand);
                            return;
                        }
                        //单个任务
                        InitializeLandCoilInfoTask(dbContext, CoilPage, listGeoLand);
                    }
                    else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                    {
                        //执行批量任务(含有子任务)
                        InitializeLandCoilInfoTaskGroup(dbContext, CoilPage);
                    }
                    else if (currentZone.Level > eZoneLevel.Town)
                    {
                        //选择地域为镇(或大于镇)
                        ShowBox(ContractAccountInfo.InitialLandCoil, ContractAccountInfo.VolumnImportErrorZone);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "InitialLandCoilDialog(初始化承包地块有效界址线)", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.InitialLandCoil, ContractAccountInfo.ContractAccountInitializeDotAndCoilFail);
                    return;
                }
            });
        }

        /// <summary>
        /// 初始化承包地块界址点线信息(单个任务)
        /// </summary>
        private void InitializeLandCoilInfoTask(IDbContext dbContext, ContractLandInitializeCoil dotCoilPage, List<ContractLand> listGeoLand)
        {
            if (dbContext == null || dotCoilPage == null || listGeoLand == null)
                return;
            TaskInitializeLandCoilArgument meta = new TaskInitializeLandCoilArgument();
            meta.CurrentZone = currentZone;
            meta.Database = dbContext;
            meta.CurrentZoneLandList = listGeoLand;
            //meta.AddressDotMarkType = dotCoilPage.AddressDotMarkType;
            //meta.AddressDotType = dotCoilPage.AddressDotType;
            meta.AddressLineCatalog = dotCoilPage.AddressLineCatalog;
            meta.AddressLinedbiDistance = dotCoilPage.AddressLinedbiDistance;
            meta.AddressLinePosition = dotCoilPage.AddressLinePosition;
            meta.AddressLineType = dotCoilPage.AddressLineType;
            meta.AddressPointPrefix = dotCoilPage.AddressPointPrefix;
            meta.IsLineDescription = dotCoilPage.IsLineDescription;
            meta.IsNeighbor = dotCoilPage.IsNeighbor;
            meta.IsPostion = dotCoilPage.IsPostion;
            meta.IsNeighborExportVillageLevel = dotCoilPage.IsNeighborExportVillageLevel;
            meta.IsUnit = dotCoilPage.IsUnit;
            //meta.IsVirtualPersonFilter = dotCoilPage.IsVirtualPersonFilter;
            //meta.IsAngleFilter = dotCoilPage.IsAngleFilter;
            meta.IsFilterDot = dotCoilPage.IsFilterDot;
            meta.MinAngleFileter = dotCoilPage.MinAngleFileter;
            meta.MaxAngleFilter = dotCoilPage.MaxAngleFilter;
            meta.IsAllLands = dotCoilPage.IsAllLands;
            meta.IsLandsWithoutInfo = dotCoilPage.IsLandsWithoutInfo;
            meta.IsSelectedLands = dotCoilPage.IsSelectedLands;
            meta.SelectedObligees = dotCoilPage.SelectedObligees;
            TaskInitializeLandCoilOperation import = new TaskInitializeLandCoilOperation();
            import.Argument = meta;
            import.Description = ContractAccountInfo.InitialLandDotCoil;
            import.Name = ContractAccountInfo.InitialLandDotCoil;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_INITIALDOTCOIL_COMPLETE, "", CurrentZone.FullCode);
                TheBns.Current.Message.Send(this, args);
            });
            TheWorkPage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            import.StartAsync();
        }

        /// <summary>
        /// 批量初始化承包地块界址点线信息(组任务)
        /// </summary>
        private void InitializeLandCoilInfoTaskGroup(IDbContext dbContext, ContractLandInitializeCoil dotCoilPage)
        {
            if (dbContext == null || dotCoilPage == null)
                return;
            TaskGroupInitializeLandCoilArgument groupMeta = new TaskGroupInitializeLandCoilArgument();
            groupMeta.CurrentZone = currentZone;
            groupMeta.Database = dbContext;
            //groupMeta.AddressDotMarkType = dotCoilPage.AddressDotMarkType;
            //groupMeta.AddressDotType = dotCoilPage.AddressDotType;
            groupMeta.AddressLineCatalog = dotCoilPage.AddressLineCatalog;
            groupMeta.AddressLinedbiDistance = dotCoilPage.AddressLinedbiDistance;
            groupMeta.AddressLinePosition = dotCoilPage.AddressLinePosition;
            groupMeta.AddressLineType = dotCoilPage.AddressLineType;
            groupMeta.AddressPointPrefix = dotCoilPage.AddressPointPrefix;
            groupMeta.IsLineDescription = dotCoilPage.IsLineDescription;
            groupMeta.IsNeighborExportVillageLevel = dotCoilPage.IsNeighborExportVillageLevel;
            groupMeta.IsNeighbor = dotCoilPage.IsNeighbor;
            groupMeta.IsPostion = dotCoilPage.IsPostion;
            groupMeta.IsUnit = dotCoilPage.IsUnit;
            //groupMeta.IsVirtualPersonFilter = dotCoilPage.IsVirtualPersonFilter;
            //groupMeta.IsAngleFilter = dotCoilPage.IsAngleFilter;
            groupMeta.IsFilterDot = dotCoilPage.IsFilterDot;
            groupMeta.MinAngleFileter = dotCoilPage.MinAngleFileter;
            groupMeta.MaxAngleFilter = dotCoilPage.MaxAngleFilter;
            groupMeta.IsAllLands = dotCoilPage.IsAllLands;
            groupMeta.IsLandsWithoutInfo = dotCoilPage.IsLandsWithoutInfo;
            groupMeta.IsSelectedLands = dotCoilPage.IsSelectedLands;
            groupMeta.SelectedObligees = dotCoilPage.SelectedObligees;
            TaskGroupInitializeLandCoilOperation taskGroup = new TaskGroupInitializeLandCoilOperation();
            taskGroup.Argument = groupMeta;
            taskGroup.Description = ContractAccountInfo.InitialLandCoil;
            taskGroup.Name = ContractAccountInfo.InitialLandCoil;
            taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, "", CurrentZone.FullCode);
                TheBns.Current.Message.Send(this, args);
            });
            TheWorkPage.TaskCenter.Add(taskGroup);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            taskGroup.StartAsync();
        }

        #endregion Method-工具-根据有效点初始化界址线

        /// <summary>
        /// 查找四至
        /// </summary>
        public void SeekLandNeighbor()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.SeekLandNeighbor, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇级
                ShowBox(ContractAccountInfo.SeekLandNeighbor, ContractAccountInfo.SeekLandNeighborErrorZone);
                return;
            }
            if (currentZone.Level == eZoneLevel.Group)
            {
                List<ContractLand> geoLands = ContractAccountBusiness.GetCollection(currentZone.FullCode, eLevelOption.Self).FindAll(c => c.Shape != null);

                //仅处理当前选择组地域下的空间数据
                if (geoLands == null || geoLands.Count == 0)
                {
                    //当前地域没有空间地块数据
                    ShowBox(ContractAccountInfo.SeekLandNeighbor, ContractAccountInfo.CurrentZoneNoGeoLand);
                    return;
                }
                seekLandNeighborSet = new SeekLandNeighborSetting();
                var skNbSPage = SeekLandNeighborSetPage.Getinstence(TheWorkPage);
                TheWorkPage.Page.ShowMessageBox(skNbSPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    IDbContext dbContext = CreateDb();
                    seekLandNeighborSet.BufferDistance = skNbSPage.BufferDistance;
                    //seekLandNeighborSet.SetLandBufferDistance = skNbSPage.SetLandBufferDistance;
                    seekLandNeighborSet.FillEmptyNeighbor = skNbSPage.FillEmptyNeighbor;
                    seekLandNeighborSet.LandIdentify = skNbSPage.LandIdentify;
                    seekLandNeighborSet.LandType = skNbSPage.LandType;
                    seekLandNeighborSet.SearchLandName = skNbSPage.SearchLandName;
                    seekLandNeighborSet.BufferDistance = skNbSPage.BufferDistance;
                    seekLandNeighborSet.UseGroupName = skNbSPage.UseGroupName;
                    seekLandNeighborSet.UseGroupNameContext = skNbSPage.UseGroupNameContext;
                    seekLandNeighborSet.SimplePositionQuery = skNbSPage.SimplePositionQuery;
                    seekLandNeighborSet.IsQueryXMzdw = skNbSPage.IsQueryXMzdw;
                    seekLandNeighborSet.SearchDeteilRule = skNbSPage.SearchDeteilRule;
                    seekLandNeighborSet.IsDeleteSameDWMC = skNbSPage.IsDeleteSameDWMC;
                    seekLandNeighborSet.QueryThreshold = skNbSPage.QueryThreshold;

                    //执行单个任务
                    TaskSeekLandNeighborArgument meta = new TaskSeekLandNeighborArgument();
                    meta.CurrentZone = currentZone;
                    meta.Database = dbContext;
                    meta.CurrentZoneLandList = geoLands;
                    meta.DicList = DictList;
                    meta.seekLandNeighborSet = seekLandNeighborSet;

                    TaskSeekLandNeighborOperation Seek = new TaskSeekLandNeighborOperation();
                    Seek.Argument = meta;
                    Seek.Description = ContractAccountInfo.SeekLandNeighbor;
                    Seek.Name = ContractAccountInfo.SeekLandNeighbor;
                    Seek.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_SEEKLANDNEIGHBOR_COMPLETE, "", CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                        Refresh();
                    });
                    TheWorkPage.TaskCenter.Add(Seek);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    Seek.StartAsync();
                });
            }
            else
            {
                var geoLandCounts = ContractAccountBusiness.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs).FindAll(c => c.Shape != null).Count();

                //仅处理当前选择组地域下的空间数据
                if (geoLandCounts == 0)
                {
                    //当前地域没有空间地块数据
                    ShowBox(ContractAccountInfo.SeekLandNeighbor, ContractAccountInfo.CurrentZoneNoGeoLand);
                    return;
                }
                seekLandNeighborSet = new SeekLandNeighborSetting();
                SeekLandNeighborSetPage skNbSPage = new SeekLandNeighborSetPage(TheWorkPage);
                TheWorkPage.Page.ShowMessageBox(skNbSPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    IDbContext dbContext = CreateDb();
                    seekLandNeighborSet.BufferDistance = skNbSPage.BufferDistance;
                    //seekLandNeighborSet.SetLandBufferDistance = skNbSPage.SetLandBufferDistance;
                    seekLandNeighborSet.FillEmptyNeighbor = skNbSPage.FillEmptyNeighbor;
                    seekLandNeighborSet.LandIdentify = skNbSPage.LandIdentify;
                    seekLandNeighborSet.LandType = skNbSPage.LandType;
                    seekLandNeighborSet.SearchLandName = skNbSPage.SearchLandName;
                    seekLandNeighborSet.BufferDistance = skNbSPage.BufferDistance;
                    seekLandNeighborSet.UseGroupName = skNbSPage.UseGroupName;
                    seekLandNeighborSet.UseGroupNameContext = skNbSPage.UseGroupNameContext;
                    seekLandNeighborSet.SimplePositionQuery = skNbSPage.SimplePositionQuery;
                    seekLandNeighborSet.IsQueryXMzdw = skNbSPage.IsQueryXMzdw;
                    seekLandNeighborSet.SearchDeteilRule = skNbSPage.SearchDeteilRule;
                    seekLandNeighborSet.IsDeleteSameDWMC = skNbSPage.IsDeleteSameDWMC;
                    seekLandNeighborSet.QueryThreshold = skNbSPage.QueryThreshold;

                    //执行单个任务
                    TaskGroupSeekLandNeighborArgument meta = new TaskGroupSeekLandNeighborArgument();
                    meta.CurrentZone = currentZone;
                    meta.Database = dbContext;
                    meta.DictList = DictList;
                    meta.seekLandNeighborSet = seekLandNeighborSet;

                    TaskGroupSeekLandNeighborOperation taskGroup = new TaskGroupSeekLandNeighborOperation();
                    taskGroup.Argument = meta;
                    taskGroup.Description = "查找" + currentZone.FullName + "范围内地块四至";
                    taskGroup.Name = ContractAccountInfo.SeekLandNeighbor;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        Refresh();
                    });
                    TheWorkPage.TaskCenter.Add(taskGroup);
                    if (ShowTaskViewer != null)
                        ShowTaskViewer();
                    taskGroup.StartAsync();
                });
            }
            GC.Collect();
        }

        /// <summary>
        /// 初始化地块面积(利用地块图斑计算地块相关面积)
        /// </summary>
        public void InitialArea()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractAccountInfo.InitialLandArea, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext dbContext = this.DbContext;
            List<Zone> allZones = GetAllChildrenZones();
            List<Zone> childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            List<ContractLand> listGeoLand = landList == null ? new List<ContractLand>() : landList.FindAll(c => c.Shape != null);
            if ((CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) &&
                (listGeoLand == null || listGeoLand.Count == 0))
            {
                //选择地域为组级地域或者大于组级地域时没有子级地域
                //没有空间地块信息
                ShowBox(ContractAccountInfo.InitialLandArea, ContractAccountInfo.CurrentZoneNoGeoLand);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇级
                ShowBox(ContractAccountInfo.InitialLandArea, ContractAccountInfo.InitialColumnSelectedZoneError);
                return;
            }
            bool isBatch = currentZone.Level > eZoneLevel.Group ? true : false;
            ContractLandInitializeArea initialAreaPage = new ContractLandInitializeArea(isBatch);
            initialAreaPage.Workpage = TheWorkPage;
            initialAreaPage.CurrentZone = CurrentZone;
            TheWorkPage.Page.ShowMessageBox(initialAreaPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }

                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                    InitialAreaTask(initialAreaPage, listGeoLand);
                }
                else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                {
                    //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                    InitialAreaTaskGroup(initialAreaPage, allZones);
                }
            });
        }

        /// <summary>
        /// 初始化地块面积小数截取(利用地块现有数据进行面积)
        /// </summary>
        public void SetAreaNumericFormat()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractAccountInfo.InitialLandAreaNumericFormat, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext dbContext = this.DbContext;
            List<Zone> allZones = GetAllChildrenZones();
            List<Zone> childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            string zoneCode = currentZone.FullCode;
            List<ContractLand> landsList = ContractAccountBusiness.GetCollection(zoneCode, eLevelOption.Self);
            //List<ContractLand> listGeoLand = landList == null ? new List<ContractLand>() : landList.FindAll(c => c.Shape != null);
            if ((CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) &&
                (landsList == null || landsList.Count == 0))
            {
                //选择地域为组级地域或者大于组级地域时没有子级地域
                ShowBox(ContractAccountInfo.InitialLandAreaNumericFormat, ContractAccountInfo.CurrentZoneNoLand);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇级
                ShowBox(ContractAccountInfo.InitialLandAreaNumericFormat, ContractAccountInfo.InitialColumnSelectedZoneError);
                return;
            }
            bool isBatch = currentZone.Level > eZoneLevel.Group ? true : false;
            ContractLandInitializeAreaNumericFormat initialAreaPage = new ContractLandInitializeAreaNumericFormat(isBatch);
            initialAreaPage.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(initialAreaPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                    InitialAreaNumericFormatTask(initialAreaPage, landsList);
                }
                else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                {
                    //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                    InitialAreaNumericFormatTaskGroup(initialAreaPage, allZones);
                }
            });
        }

        /// <summary>
        /// 初始化承包地块是否基本农田
        /// </summary>
        public void InitialIsFarmer()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractAccountInfo.InitialLandIsFarmer, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext dbContext = this.DbContext;
            List<Zone> allZones = GetAllChildrenZones();
            List<Zone> childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            List<ContractLand> listGeoLand = landList == null ? new List<ContractLand>() : landList.FindAll(c => c.Shape != null);
            if ((CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) &&
                (listGeoLand == null || listGeoLand.Count == 0))
            {
                //选择地域为组级地域或者大于组级地域时没有子级地域
                //没有空间地块信息
                ShowBox(ContractAccountInfo.InitialLandIsFarmer, ContractAccountInfo.CurrentZoneNoGeoLand);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇级
                ShowBox(ContractAccountInfo.InitialLandIsFarmer, ContractAccountInfo.InitialColumnSelectedZoneError);
                return;
            }
            bool isBatch = currentZone.Level > eZoneLevel.Group ? true : false;
            ContractLandInitialIsFarmer initialIsFamerPage = new ContractLandInitialIsFarmer(isBatch);
            initialIsFamerPage.Workpage = TheWorkPage;
            initialIsFamerPage.CurrentZone = CurrentZone;
            TheWorkPage.Page.ShowMessageBox(initialIsFamerPage, (b, r) =>
            {
                if (!(bool)b || string.IsNullOrEmpty(initialIsFamerPage.FileName))
                {
                    return;
                }
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                    InitialIsFarmerTask(initialIsFamerPage, listGeoLand);
                }
                else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                {
                    //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                    InitialIsFarmerTaskGroup(initialIsFamerPage, allZones);
                }
            });
        }

        /// <summary>
        /// 自动编码
        /// </summary>
        public void GetLandCode()
        {
            var landStation = DbContext.CreateContractLandWorkstation();
            var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vps = vpStation.GetByZoneCode(currentZone.FullCode);
            var lands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
            var index = 1;
            foreach (var vp in vps)
            {
                var vplands = lands.Where(x => x.OwnerId == vp.ID).ToList();
                vplands.ForEach(x =>
                {
                    x.LandNumber = currentZone.FullCode + index.ToString().PadLeft(5, '0');
                    index++;
                    
                });
                landStation.UpdateLandCode(vplands);
            }
        }

        /// <summary>
        /// 检索地块名称为空
        /// </summary>
        public void LandNameNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.LandNameNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchNameNull);
        }

        /// <summary>
        /// 检索二轮合同面积为空
        /// </summary>
        public void ContractAreaNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractAreaNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchContractAreaNull);
        }

        /// <summary>
        /// 检索实测面积为空
        /// </summary>
        public void ActualAreaNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ActualAreaNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchActualAreaNull);
        }

        /// <summary>
        /// 检索确权面积为空
        /// </summary>
        public void AwareAreaNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.AwareAreaNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchAwareAreaNull);
        }

        /// <summary>
        /// 检索是否基本农田为空
        /// </summary>
        public void FarmerLandNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.FarmerLandNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchIsFarmerNull);
        }

        /// <summary>
        /// 检索地力等级为空
        /// </summary>
        public void LandLevelNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.LandLevelNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchLandLevelNull);
        }

        /// <summary>
        /// 检索地块图斑为空
        /// </summary>
        public void LandShapeNullSearch()
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.LandLevelNullVisibility, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            LandSearchTool(eContractAccountType.SearchLandShapeNull);
        }

        /// <summary>
        /// 空间查看
        /// 1、如果选择数据是承包方，则查看该承包方下面所有地块空间形状与位置
        /// 2、如果选择数据时承包地块，则查看该承包地块的空间形状与位置
        /// 3、如果没有空间数据，则提示无空间数据可供查看
        /// </summary>
        public List<ContractLand> FindGeometry()
        {
            if (CurrentAccountItem == null && CurrentLandBinding == null)
            {
                //没有选择项
                ShowBox(ContractAccountInfo.FindGeometryTool, ContractAccountInfo.FindGeometryNoSelectedItem);
                return null;
            }
            if (CurrentLandBinding != null)
            {
                //此时选择是承包地块
                List<ContractLand> lands = new List<ContractLand>();
                var land = landList.Find(c => c.ID == CurrentLandBinding.Tag.ID);
                var landGeo = land.Shape as YuLinTu.Spatial.Geometry;

                if (landGeo == null)
                {
                    ShowBox(ContractAccountInfo.FindGeometryTool, ContractAccountInfo.FindNoGeometryUnderCurrentSelected);
                    return null;
                }
                lands.Add(land);
                if (lands == null || lands.Count == 0)
                {
                    return null;
                }
                return lands;
            }
            else
            {
                //此时选择的是承包方
                List<ContractLand> lands = new List<ContractLand>();
                foreach (var child in currentAccountItem.Children)
                {
                    var land = landList.Find(c => c.ID == child.Tag.ID);
                    if (land == null)
                    {
                        continue;
                    }
                    var landGeo = land.Shape as YuLinTu.Spatial.Geometry;
                    if (landGeo == null)
                    {
                        continue;
                    }
                    lands.Add(land);
                }
                if (lands == null || lands.Count == 0)
                {
                    ShowBox(ContractAccountInfo.FindGeometryTool, ContractAccountInfo.FindNoGeometryUnderCurrentSelected);
                    return null;
                }
                return lands;
            }
        }

        /// <summary>
        /// 清空(根据配置文件中确定是否清空当前地域下的承包方数据)
        /// </summary>
        public void Clear()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ContractAccountClear, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (ContractBusinessSettingDefine.ClearVirtualPersonData)
            {
                if (currentZone.Level == eZoneLevel.Group &&
                        (currentPersonList == null || (currentPersonList != null && currentPersonList.Count(c => c.Status == eVirtualPersonStatus.Right) == 0)))
                {
                    ShowBox(ContractAccountInfo.ContractAccountClear, "当前地域下没有可供清空地块数据!");
                    return;
                }
            }
            else
            {
                if (currentZone.Level == eZoneLevel.Group &&
                        (ContractAccountBusiness == null || landList == null || landList.Count() == 0
                        || (currentPersonList != null && currentPersonList.Count(c => c.Status == eVirtualPersonStatus.Right) == 0)))
                {
                    ShowBox(ContractAccountInfo.ContractAccountClear, ContractAccountInfo.CurrentZoneNoLand);
                    return;
                }
            }

            string msg = ContractBusinessSettingDefine.ClearVirtualPersonData ?
                ContractAccountInfo.ContractAccountAllClearSure : ContractAccountInfo.ContractAccountLandClearSure;
            var clearDlg = new MessageDialog
            {
                Header = ContractAccountInfo.ContractAccountClear,
                Message = msg,
                MessageGrade = eMessageGrade.Warn,
            };
            clearDlg.Confirm += (s, e) =>
            {
                var landStation = DbContext.CreateContractLandWorkstation();
                var coilStation = DbContext.CreateBoundaryAddressCoilWorkStation();
                var dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
                ModuleMsgArgs arg = null;
                landStation.DeleteByZoneCode(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);  //清空承包地块数据库中的数据
                dotStation.DeleteByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs); //界址线删除
                coilStation.DeleteByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);  //界址点删除
                if (ContractBusinessSettingDefine.ClearVirtualPersonData)
                {
                    arg = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE, null, CurrentZone.FullCode);
                }
                else
                {
                    arg = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_CLEAR_COMPLETE, null, CurrentZone.FullCode);
                }
                SendMessasge(arg);
            };
            clearDlg.ConfirmStart += (s, e) =>
            {
                MenueEnableMethod(false);
                TheWorkPage.Page.IsBusy = true;
            };
            clearDlg.ConfirmCompleted += (s, e) =>
            {
                MenueEnableMethod();
                TheWorkPage.Page.IsBusy = false;
                var removeData = accountLandItems.Where(c => c.Tag.Status == eVirtualPersonStatus.Right).TryToList();
                foreach (var data in removeData)
                {
                    accountLandItems.Remove(data);
                }
                DataCount();
                Dispatcher.Invoke(new Action(() => { Refresh(); RefreshStockRight(); }));
            };
            clearDlg.ConfirmTerminated += (s, e) =>
            {
                MenueEnableMethod();
                TheWorkPage.Page.IsBusy = false;
                YuLinTu.Library.Log.Log.WriteException(this, "Clear(清空承包地块)", e.Exception.ToString());
                ShowBox(ContractAccountInfo.ContractAccountClear, ContractAccountInfo.ContractAccountClearFail);
            };
            TheWorkPage.Page.ShowMessageBox(clearDlg, (b, r) =>
             {
                 if (!(bool)b)
                     return;
             });
        }

        private ContractLand_Del GetLandDel(ContractLand land)
        {
            var landDel = new ContractLand_Del();
            landDel.ID = land.ID;
            landDel.DKBM = land.LandNumber;
            landDel.YDKBM = land.OldLandNumber;
            landDel.DKMC = land.LandName;
            landDel.QQMJ = land.AwareArea;
            landDel.SCMJ = land.ActualArea;
            landDel.CBFID = (Guid)land.OwnerId;
            landDel.DKDZ = land.NeighborEast;
            landDel.DKNZ = land.NeighborSouth;
            landDel.DKXZ = land.NeighborEast;
            landDel.DKBZ = land.NeighborNorth;
            landDel.BZXX = land.Comment;
            landDel.DYBM = land.ZoneCode;
            return landDel;
        }

        /// <summary>
        /// 通知确权确股插件更新
        /// </summary>
        private void RefreshStockRight()
        {
            var arg = MessageExtend.ContractAccountMsg(DbContext, "StockRight_Refresh", null);
            TheWorkPage.Workspace.Message.Send(this, arg);
        }

        #region Method-辅助-工具-地块过滤

        /// <summary>
        /// 工具—地块过滤
        /// </summary>
        /// <param name="code">地块类别编码</param>
        private void LandFilterTool(string code)
        {
            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    this.Dispatcher.Invoke(new Action(() => { SetLandVisibleTool(go); }));
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                }, null, null, null, null, code);
        }

        /// <summary>
        /// 工具-设置地块可见性(承包人)
        /// </summary>
        private void SetLandVisibleTool(TaskGoEventArgs arg)
        {
            string landCategory = arg.Instance.Argument.UserState.ToString();
            view.Filter(obj =>
            {
                var landBind = obj as ContractLandBinding;
                if (landCategory.IsNullOrBlank())
                {
                    if (landBind != null)
                    {
                        landBind.Visibility = Visibility.Visible;
                    }
                    return true;
                }

                bool has = false;
                if (landBind != null)
                {
                    has = landBind.Tag.LandCategory.Equals(landCategory);
                    if (has)
                    {
                        landBind.Visibility = Visibility.Visible;
                        return true;
                    }
                    else
                    {
                        landBind.Visibility = Visibility.Collapsed;
                    }
                }
                return has;
            }, true);

            foreach (var item in accountLandItems)
            {
                var personBind = item;
                if (personBind != null)
                {
                    var children = personBind.Children;
                    var showitemCount = children.Count(cc => cc.Visibility == Visibility.Visible);
                    personBind.Name = ConvertContractor.CreateItemName(personBind.Tag, showitemCount) + ConvertContractor.CreateItemNumber(personBind.Tag);
                }
            }
        }

        #endregion Method-辅助-工具-地块过滤

        #region Method-辅助-工具-初始数据

        /// <summary>
        /// 工具之初始化地块属性信息(单个任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialLandInfoTask(object page, List<ContractLand> lands)
        {
            var initialLand = page as ContractLandInitializePage;
            var argument = new TaskInitialLandInfoArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.CurrentZoneLandList = lands;        //当前地域下的地块集合
            argument.LandName = initialLand.LandName;
            argument.LandLevel = initialLand.LandLevel;
            argument.LandPurpose = initialLand.LandPurpose;
            argument.IsFamer = initialLand.IsFamer;
            argument.IsCombination = initialLand.IsCombination;
            argument.IsNew = initialLand.IsNew;
            argument.AwareAreaEqualActual = initialLand.AwareAreaEqualActual;
            argument.InitialAwareArea = initialLand.InitialAwareArea;
            argument.InitialIsFamer = initialLand.InitialIsFamer;
            argument.InitialLandLevel = initialLand.InitialLandLevel;
            argument.QSXZ = initialLand.QSXZ;
            argument.InitialQSXZ = initialLand.InitialQSXZ;
            argument.InitialLandName = initialLand.InitialLandName;
            argument.InitialLandNumber = initialLand.InitialLandNumber;
            argument.InitialLandNumberByUpDown = initialLand.InitialLandNumberByUpDown;
            argument.InitialLandPurpose = initialLand.InitialLandPurpose;
            argument.HandleContractLand = initialLand.HandleContractLand;
            argument.InitialMapNumber = initialLand.InitialMapNumber;
            argument.InitialSurveyPerson = initialLand.InitialSurveyPerson;
            argument.InitialSurveyDate = initialLand.InitialSurveyDate;
            argument.InitialSurveyInfo = initialLand.InitialSurveyInfo;
            argument.InitialCheckPerson = initialLand.InitialCheckPerson;
            argument.InitialCheckDate = initialLand.InitialCheckDate;
            argument.InitialCheckInfo = initialLand.InitialCheckInfo;
            argument.InitialReferPerson = initialLand.InitialReferPerson;
            argument.LandExpand = initialLand.LandExpand;
            argument.InitialReferPersonByOwner = initialLand.InitialReferPersonByOwner;
            argument.VirtualType = this.VirtualType;
            argument.InitialNull = initialLand.InitializeNull;
            argument.InitialLandNeighbor = initialLand.InitialLandNeighbor;
            argument.InitialLandNeighborInfo = initialLand.InitialLandNeighborInfo;

            TaskInitialLandInfoOperation operation = new TaskInitialLandInfoOperation();
            operation.Argument = argument;
            argument.InitLandComment = initialLand.InitLandComment;
            argument.LandComment = initialLand.LandComment;
            operation.Description = ContractAccountInfo.InitialLandInfo;   //任务描述
            operation.Name = ContractAccountInfo.InitialData;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //此处做界面修改
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALLAND_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块批量查找周边地块任务-按镇
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialLandNeigborInfoTask()
        {
            var argument = new TaskInitializeLandNeighborInfoArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;

            string zoneCode = CurrentZone.FullCode;
            if (CurrentZone.Level != eZoneLevel.Town)
            {
                var zonest = DbContext.CreateZoneWorkStation();
                var townzones = zonest.GetAllZones(CurrentZone);
                var town = townzones.Find(z => z.Level == eZoneLevel.Town);
                if (town != null)
                {
                    zoneCode = town.FullCode;
                }
            }
            argument.TownZoneCode = zoneCode;

            TaskInitializeLandNeighborInfoOperation operation = new TaskInitializeLandNeighborInfoOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.InitialLandNeigborInfo;   //任务描述
            operation.Name = ContractAccountInfo.InitialData;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //此处做界面修改
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALLAND_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块批量查找周边地块任务-按区县
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialLandNeigborInfoGroupTask()
        {
            var argument = new TaskGroupInitializeLandNeighborInfoArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;

            TaskGroupInitializeLandNeighborInfoOperation operation = new TaskGroupInitializeLandNeighborInfoOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.InitialLandNeigborInfo;   //任务描述
            operation.Name = ContractAccountInfo.InitialData;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //此处做界面修改
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALLAND_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块属性信息(组任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialLandInfoTaskGroup(object page, List<Zone> allZones)
        {
            var initialLand = page as ContractLandInitializePage;
            TaskGroupInitialLandInfoArgument groupArgument = new TaskGroupInitialLandInfoArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.AllZones = allZones;
            groupArgument.LandName = initialLand.LandName;
            groupArgument.LandLevel = initialLand.LandLevel;
            groupArgument.LandPurpose = initialLand.LandPurpose;
            groupArgument.IsFamer = initialLand.IsFamer;
            groupArgument.IsCombination = initialLand.IsCombination;
            groupArgument.IsNew = initialLand.IsNew;
            groupArgument.QSXZ = initialLand.QSXZ;
            groupArgument.InitialQSXZ = initialLand.InitialQSXZ;
            groupArgument.AwareAreaEqualActual = initialLand.AwareAreaEqualActual;
            groupArgument.InitialAwareArea = initialLand.InitialAwareArea;
            groupArgument.InitialIsFamer = initialLand.InitialIsFamer;
            groupArgument.InitialLandLevel = initialLand.InitialLandLevel;
            groupArgument.InitialLandName = initialLand.InitialLandName;
            groupArgument.InitialLandNumber = initialLand.InitialLandNumber;
            groupArgument.InitialLandNumberByUpDown = initialLand.InitialLandNumberByUpDown;
            groupArgument.InitialLandPurpose = initialLand.InitialLandPurpose;
            groupArgument.HandleContractLand = initialLand.HandleContractLand;
            groupArgument.InitialMapNumber = initialLand.InitialMapNumber;
            groupArgument.InitialSurveyPerson = initialLand.InitialSurveyPerson;
            groupArgument.InitialSurveyDate = initialLand.InitialSurveyDate;
            groupArgument.InitialSurveyInfo = initialLand.InitialSurveyInfo;
            groupArgument.InitialCheckPerson = initialLand.InitialCheckPerson;
            groupArgument.InitialCheckDate = initialLand.InitialCheckDate;
            groupArgument.InitialCheckInfo = initialLand.InitialCheckInfo;
            groupArgument.InitialReferPerson = initialLand.InitialReferPerson;
            groupArgument.LandExpand = initialLand.LandExpand;
            groupArgument.InitialReferPersonByOwner = initialLand.InitialReferPersonByOwner;
            groupArgument.VirtualType = this.VirtualType;
            groupArgument.InitialNull = initialLand.InitializeNull;
            groupArgument.InitialLandNeighbor = initialLand.InitialLandNeighbor;
            groupArgument.InitialLandNeighborInfo = initialLand.InitialLandNeighborInfo;

            groupArgument.VillageInlitialSet = SystemSetDefine.VillageInlitialSet;
            groupArgument.InitLandComment = initialLand.InitLandComment;
            groupArgument.LandComment = initialLand.LandComment;
            TaskGroupInitialLandInfoOperation groupOperation = new TaskGroupInitialLandInfoOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Workpage = TheWorkPage;
            groupOperation.Description = ContractAccountInfo.InitialLandInfo;   //任务描述
            groupOperation.Name = ContractAccountInfo.InitialData;         //任务名称
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALLAND_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块面积(单个任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialAreaTask(object page, List<ContractLand> listGeoLand)
        {
            var initialArea = page as ContractLandInitializeArea;
            TaskInitialAreaArgument argument = new TaskInitialAreaArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.ListGeoLand = listGeoLand;        //当前地域下的地块集合
            argument.ToActualArea = initialArea.ToActualArea;
            argument.ToAwareArea = initialArea.ToAwareArea;
            argument.VirtualType = this.VirtualType;
            argument.ToAreaNumeric = initialArea.ToAreaNumeric;
            argument.ToAreaModule = initialArea.ToAreaModule;
            TaskInitialAreaOperation operation = new TaskInitialAreaOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.InitialLandArea;   //任务描述
            operation.Name = ContractAccountInfo.InitialData;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //此处做界面修改
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALAREA_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块面积(组任务)
        /// </summary>
        private void InitialAreaTaskGroup(object page, List<Zone> allZones)
        {
            var initialArea = page as ContractLandInitializeArea;
            TaskGroupInitialAreaArgument groupArgument = new TaskGroupInitialAreaArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.AllZones = allZones;
            groupArgument.VirtualType = this.VirtualType;
            groupArgument.ToActualArea = initialArea.ToActualArea;
            groupArgument.ToAwareArea = initialArea.ToAwareArea;
            groupArgument.ToAreaModule = initialArea.ToAreaModule;
            groupArgument.ToAreaNumeric = initialArea.ToAreaNumeric;
            TaskGroupInitialAreaOperation groupOperation = new TaskGroupInitialAreaOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Workpage = TheWorkPage;
            groupOperation.Description = ContractAccountInfo.InitialLandArea;   //任务描述
            groupOperation.Name = ContractAccountInfo.InitialData;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALISFARMER_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 工具之截取地块面积小数位(单个任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialAreaNumericFormatTask(object page, List<ContractLand> LandsList)
        {
            var initialCutArea = page as ContractLandInitializeAreaNumericFormat;
            TaskInitialAreaNumericFormatArgument argument = new TaskInitialAreaNumericFormatArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.ToAreaNumeric = initialCutArea.ToAreaNumeric;
            argument.ToAreaModule = initialCutArea.ToAreaModule;
            argument.ToActualArea = initialCutArea.ToActualArea;
            argument.ToAwareArea = initialCutArea.ToAwareArea;
            argument.ToTableArea = initialCutArea.ToTableArea;
            argument.ListLand = LandsList;        //当前地域下的地块集合
            argument.VirtualType = this.VirtualType;
            TaskInitialAreaNumericFormatOperation operation = new TaskInitialAreaNumericFormatOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.InitialLandAreaNumericFormat;   //任务描述
            operation.Name = ContractAccountInfo.InitialData;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALAREA_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之截取地块面积小数位(组任务)
        /// </summary>
        private void InitialAreaNumericFormatTaskGroup(object page, List<Zone> allZones)
        {
            var initialCutArea = page as ContractLandInitializeAreaNumericFormat;
            TaskGroupInitialAreaNumericFormatArgument groupArgument = new TaskGroupInitialAreaNumericFormatArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.AllZones = allZones;
            groupArgument.VirtualType = this.VirtualType;
            groupArgument.ToAreaNumeric = initialCutArea.ToAreaNumeric;
            groupArgument.ToAreaModule = initialCutArea.ToAreaModule;
            groupArgument.ToTableArea = initialCutArea.ToTableArea;
            groupArgument.ToActualArea = initialCutArea.ToActualArea;
            groupArgument.ToAwareArea = initialCutArea.ToAwareArea;
            TaskGroupInitialAreaNumericFormatOperation groupOperation = new TaskGroupInitialAreaNumericFormatOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = ContractAccountInfo.InitialLandAreaNumericFormat;   //任务描述
            groupOperation.Name = ContractAccountInfo.InitialData;
            groupOperation.Workpage = TheWorkPage;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALISFARMER_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块是否基本农田(单个任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialIsFarmerTask(object page, List<ContractLand> listGeoLand)
        {
            var initialIsFarmer = page as ContractLandInitialIsFarmer;
            TaskInitialIsFarmerArgument argument = new TaskInitialIsFarmerArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.ListGeoLand = listGeoLand;        //当前地域下的地块集合
            argument.ShapeFileName = initialIsFarmer.FileName;
            argument.VirtualType = this.VirtualType;
            TaskInitialIsFarmerOperation operation = new TaskInitialIsFarmerOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.InitialLandIsFarmer;
            operation.Name = ContractAccountInfo.InitialData;
            //operation.IsBatch = this.IsBatch;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //此处做界面修改
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALISFARMER_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 工具之初始化地块是否基本农田(组任务)
        /// </summary>
        /// <param name="page">界面</param>
        private void InitialIsFarmerTaskGroup(object page, List<Zone> allZones)
        {
            var initialIsFarmer = page as ContractLandInitialIsFarmer;
            TaskGroupInitialIsFarmerArgument groupArgument = new TaskGroupInitialIsFarmerArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.AllZones = allZones;
            groupArgument.ShapeFileName = initialIsFarmer.FileName;
            groupArgument.VirtualType = this.VirtualType;
            TaskGroupInitialIsFarmerOperation groupOperation = new TaskGroupInitialIsFarmerOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Workpage = TheWorkPage;
            groupOperation.Description = ContractAccountInfo.InitialLandIsFarmer;
            groupOperation.Name = ContractAccountInfo.InitialData;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALISFARMER_COMPLETE, accountLandItems, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion Method-辅助-工具-初始数据

        #region Method-辅助-工具-地块检索

        /// <summary>
        /// 工具-检索
        /// </summary>
        private void LandSearchTool(eContractAccountType field)
        {
            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    Dispatcher.Invoke(new Action(() => { SetFieldNullVisibleTool(go); }));
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                }, null, null, null, null, field);
        }

        /// <summary>
        /// 检索字段为空的地块
        /// </summary>
        private void SetFieldNullVisibleTool(TaskGoEventArgs arg)
        {
            var type = (eContractAccountType)arg.Instance.Argument.UserState;   //获取要查询的地块字段
            if (accountLandItems == null || accountLandItems.Count == 0)
                return;

            view.Filter(obj =>
            {
                bool isNull = false;

                var landBind = obj as ContractLandBinding;
                if (landBind == null)
                    return false;
                isNull = CommonSearchByType(type, landBind);
                return isNull;
            }, true);
        }

        /// <summary>
        /// 检索字段为空的地块
        /// </summary>
        private void SetFieldCodeReateVisibleTool(TaskGoEventArgs arg)
        {
            if (accountLandItems == null || accountLandItems.Count == 0)
                return;
            HashSet<string> codeset = new HashSet<string>();
            List<string> repeatCode = new List<string>();
            foreach (var item in accountLandItems)
            {
                foreach (var land in item.Children)
                {
                    if (!codeset.Contains(land.LandNumber))
                        codeset.Add(land.LandNumber);
                    else
                        repeatCode.Add(land.LandNumber);
                }
            }
            view.Filter(obj =>
            {
                var landBind = obj as ContractLandBinding;
                if (landBind == null)
                    return false;
                return repeatCode.Contains(landBind.LandNumber);
            }, true);
        }

        /// <summary>
        /// 根据检索类型进行检索
        /// </summary>
        /// <param name="type">检索类型</param>
        /// <param name="child">地块信息(界面实体)</param>
        /// <returns>false(父项显示)---true(父项隐藏)</returns>
        private bool CommonSearchByType(eContractAccountType type, ContractLandBinding child)
        {
            bool isNull = false;
            switch (type)
            {
                case eContractAccountType.SearchNameNull:
                    if (string.IsNullOrEmpty(child.Tag.Name))
                        isNull = true;
                    break;

                case eContractAccountType.SearchContractAreaNull:
                    if (string.IsNullOrEmpty(child.Tag.TableArea.ToString()) || child.Tag.TableArea == 0)
                        isNull = true;
                    break;

                case eContractAccountType.SearchActualAreaNull:
                    if (string.IsNullOrEmpty(child.Tag.ActualArea.ToString()) || child.Tag.ActualArea == 0)
                        isNull = true;
                    break;

                case eContractAccountType.SearchAwareAreaNull:
                    if (string.IsNullOrEmpty(child.Tag.AwareArea.ToString()) || child.Tag.AwareArea == 0)
                        isNull = true;
                    break;

                case eContractAccountType.SearchIsFarmerNull:
                    if (string.IsNullOrEmpty(child.IsFarmerLandUI) || (!string.IsNullOrEmpty(child.IsFarmerLandUI) && child.IsFarmerLandUI == ""))
                        isNull = true;
                    break;

                case eContractAccountType.SearchLandLevelNull:
                    if (string.IsNullOrEmpty(child.LandLevelUI) || (!string.IsNullOrEmpty(child.LandLevelUI) && child.LandLevelUI == ""))
                        isNull = true;
                    break;

                case eContractAccountType.SearchLandShapeNull:
                    if (child.Tag.Shape == null)
                        isNull = true;
                    break;
            }
            return isNull;
        }

        #endregion Method-辅助-工具-地块检索

        #region Method-辅助方法-其它

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error,
            Action<bool?, eCloseReason> action = null, bool showCancel = true, bool showConfirm = true)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TheWorkPage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                    CancelButtonVisibility = showCancel ? Visibility.Visible : Visibility.Collapsed,
                    ConfirmButtonVisibility = showConfirm ? Visibility.Visible : Visibility.Collapsed,
                }, action);
            }));
        }

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBoxAction(string header, string msg, Action<bool?, eCloseReason> action = null)
        {
            var tabDialog = new ConfirmInfoPage(TheWorkPage, header, msg);
            Dispatcher.Invoke(new Action(() =>
            {
                TheWorkPage.Page.ShowMessageBox(tabDialog, action);
            }));
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 获取承包台账显示界面选中项信息
        /// </summary>
        private void GetSelectItem()
        {
            CurrentAccountItem = null;
            CurrentLandBinding = null;
            var item = view.SelectedItem;
            if (item == null)
            {
                //界面上没有选中任何项。做标记之
                IsSelected = false;
            }
            else if (item is ContractLandPersonItem)
            {
                //界面上选中的是承包方(界面实体)
                CurrentAccountItem = item as ContractLandPersonItem;
                IsSelected = true;
            }
            else if (item is ContractLandBinding)
            {
                //界面上选中的是承包地块(界面实体)
                CurrentLandBinding = item as ContractLandBinding;
                var personItem = accountLandItems.FirstOrDefault(c => c.ID == CurrentLandBinding.Tag.OwnerId);
                CurrentAccountItem = personItem;
                IsSelected = true;
            }
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        public void SendMessasge(ModuleMsgArgs args)
        {
            TheWorkPage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            TheWorkPage.Workspace.Message.Send(this, args);
        }

        #endregion Method-辅助方法-其它

        #region Methods-是否批量

        ///// <summary>
        ///// 是否批量
        ///// </summary>
        //private void caIsbatch_Click_1(object sender, RoutedEventArgs e)
        //{
        //    isbatch = (bool)caIsbatch.IsChecked;
        //    if (IsBatchEvt != null)
        //    {
        //        IsBatchEvt(isbatch);
        //    }
        //}

        #endregion Methods-是否批量

        #region Methods-辅助-台账报表

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
        }

        /// <summary>
        /// 获取子级地域(包括当前地域)
        /// </summary>
        public List<Zone> GetAllChildrenZones()
        {
            List<Zone> allZones = new List<Zone>();
            IDbContext dbContext = CreateDb();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取子级地域失败)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ImportBoundaryAddressDot, "获取当前地域下的子级地域失败!");
                return null;
            }
            return allZones;
        }

        #endregion Methods-辅助-台账报表

        #region Methods -上传下载

        /// <summary>
        /// 下载数据
        /// </summary>
        public void DownLoadData()
        {
            if (currentZone == null)
            {
                ShowBox(ContractAccountInfo.DownService, "请先选择待下载数据所在地域!");
                return;
            }
            if (string.IsNullOrEmpty(ServiceSetDefine.BusinessDataAddress))
            {
                ShowBox(ContractAccountInfo.DownService, "数据服务地址为空,不能下载数据");
                return;
            }
            IDbContext dbContext = CreateDb();
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                Guid? id = TheWorkPage.Workspace.GetUserProfile().SessionCode;
                ArcLandImporArgument zoneMeta = new ArcLandImporArgument();
                zoneMeta.Database = dbContext;
                zoneMeta.OpratorName = "DownLoad";
                zoneMeta.CurrentZone = currentZone;
                zoneMeta.UserName = TheWorkPage.Workspace.GetUserProfile().Name;
                zoneMeta.SessionCode = id.HasValue ? id.Value.ToString() : "";
                ArcLandImportProgress dataProgress = new ArcLandImportProgress();
                dataProgress.Name = "下载数据";
                dataProgress.Argument = zoneMeta;
                dataProgress.Description = string.Format("下载{0}({1})下的数据", currentZone.Name, currentZone.FullCode);
                TheWorkPage.TaskCenter.Add(dataProgress);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                dataProgress.StartAsync();
            });
            ShowBox(ContractAccountInfo.DownService, string.Format("确定下载{0}下的数据?", currentZone.FullName), eMessageGrade.Infomation, action);
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        public void UpLoadData()
        {
            if (currentZone == null)
            {
                ShowBox(ContractAccountInfo.UpService, "请先选择待上传数据所在地域!");
                return;
            }
            if (string.IsNullOrEmpty(ServiceSetDefine.BusinessDataAddress))
            {
                ShowBox(ContractAccountInfo.UpService, "数据服务地址为空,不能上传数据");
                return;
            }
            IDbContext dbContext = CreateDb();
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                Guid? id = TheWorkPage.Workspace.GetUserProfile().SessionCode;
                ArcLandImporArgument zoneMeta = new ArcLandImporArgument();
                zoneMeta.Database = dbContext;
                zoneMeta.OpratorName = "UpLoad";
                zoneMeta.CurrentZone = currentZone;
                zoneMeta.UserName = TheWorkPage.Workspace.GetUserProfile().Name;
                zoneMeta.SessionCode = id.HasValue ? id.Value.ToString() : "";
                ArcLandExportProgress dataProgress = new ArcLandExportProgress();
                dataProgress.Name = "上传数据";
                dataProgress.Argument = zoneMeta;
                dataProgress.Description = string.Format("上传{0}下的数据", currentZone.FullName);
                TheWorkPage.TaskCenter.Add(dataProgress);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                dataProgress.StartAsync();
            });
            ShowBox(ContractAccountInfo.UpService, string.Format("确定上传{0}下的数据?", currentZone.FullName), eMessageGrade.Infomation, action);
        }

        /// <summary>
        /// 获取服务配置
        /// </summary>
        private ServiceSetDefine GetServiceSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ServiceSetDefine>();
            var section = profile.GetSection<ServiceSetDefine>();
            return section.Settings;
        }

        /// <summary>
        /// 获取上传配置
        /// </summary>
        private UploadSettingDefine GetUploadSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<UploadSettingDefine>();
            var section = profile.GetSection<UploadSettingDefine>();
            return section.Settings;
        }

        /// <summary>
        /// 获取下载配置
        /// </summary>
        /// <returns></returns>
        private DownloadSettingDefine GetDownSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<DownloadSettingDefine>();
            var section = profile.GetSection<DownloadSettingDefine>();
            return section.Settings;
        }

        #endregion Methods -上传下载

        #endregion Method-工具

        #endregion Methods

        private void view_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (TheWorkPage.Page.IsBusy)

            //if (IsUnderTabHeader(e.OriginalSource as DependencyObject))
            //if(TheWorkPage.Page.IsBusy)
            //    CommitTables(this.view);
        }

        private void miSearchLandCodeRepeat_Click(object sender, RoutedEventArgs e)
        {
            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    Dispatcher.Invoke(new Action(() => { SetFieldCodeReateVisibleTool(go); }));
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                }, null, null, null, null, "地块编码重复");
        }

        //private bool IsUnderTabHeader(DependencyObject control)
        //{
        //    if (control is TabItem)
        //        return true;
        //    DependencyObject parent = VisualTreeHelper.GetParent(control);
        //    if (parent == null)
        //        return false;
        //    return IsUnderTabHeader(parent);
        //}
        //private void CommitTables(DependencyObject control)
        //{
        //    if (control is DataGrid)
        //    {
        //        DataGrid grid = control as DataGrid;
        //        grid.CommitEdit(DataGridEditingUnit.Row, true);
        //        return;
        //    }
        //    int childrenCount = VisualTreeHelper.GetChildrenCount(control);
        //    for (int i = 0; i < childrenCount; i++)
        //    {
        //        CommitTables(VisualTreeHelper.GetChild(control, i));
        //    }
        //}
    }
}