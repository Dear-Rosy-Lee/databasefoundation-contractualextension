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
using YuLinTu.Windows.Wpf;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包权证主界面
    /// </summary>
    public partial class ContractRegeditBookPanel : UserControl
    {
        #region Fields

        //private int index = 0;//序号
        private bool isbatch;//是否批量

        private string zoneName;
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueFilter;//过滤数据
        private Zone currentZone;
        private bool allCheck = false;

        //private ConcordItem currentItem;//当前选择项
        private ContractRegeditBookItem currentItem;//当前选择项

        //private BindConcord currentConcord;//当前选择合同
        private BindContractRegeditBook currentRegeditBook;//当前选择权证

        private eVirtualType virtualType;
        private List<object> list = new List<object>();

        //private bool showTableColuml;
        private List<VirtualPerson> selectVps = new List<VirtualPerson>();//当前获取的承包方集合-排除锁定

        private ContractRegeditBookSettingDefine config = ContractRegeditBookSettingDefine.GetIntence();

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum = 12;

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum = 73;

        /// <summary>
        /// 证书编码设置-证书编码样式设置
        /// </summary>
        public string BookNumSetting = "NO.J";

        /// <summary>
        /// 权证绑定集合
        /// </summary>
        public ObservableCollection<ContractRegeditBookItem> Items = new ObservableCollection<ContractRegeditBookItem>();

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

        /// <summary>
        /// 是否有其他的获取显示
        /// </summary>
        public delegate void OtherDoWork(TaskGoEventArgs arg);

        #endregion Fields

        #region Popertys

        /// <summary>
        /// 是否批处理
        /// </summary>
        public bool IsBatch
        {
            get
            {
                return isbatch;
            }

            set
            {
                isbatch = value;
                NotifyPropertyChanged("IsBatch");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        public PanelIsBatch IsBatchEvt { get; private set; }

        /// <summary>
        /// 当前地域变化
        /// </summary>
        public PanelZoneChanged ZoneChanged { get; set; }

        /// <summary>
        /// 有其他的获取显示
        /// </summary>
        public OtherDoWork HaveOherDoWork { get; set; }

        /// <summary>
        /// 数据统计
        /// </summary>
        public ContractRegeditBookSummary Summary { get; private set; }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 数据字典处理业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 土地业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        /// <summary>
        /// 合同业务类
        /// </summary>
        public ConcordBusiness ConcordBusiness { get; set; }

        /// <summary>
        /// 权证业务类
        /// </summary>
        public ContractRegeditBookBusiness ContractRegeditBookBusiness { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

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
                InlitialControl(currentZone == null ? "" : currentZone.FullCode);
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

        /// <summary>
        /// 合同
        /// </summary>
        public VirtualPerson virtualPerson
        {
            get { return currentItem == null ? null : currentItem.Tag; }
        }

        /// <summary>
        /// 承包权证数据汇总表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<DataSummaryDefine>();
                var section = profile.GetSection<DataSummaryDefine>();
                var config = section.Settings as DataSummaryDefine;
                return config;
            }
        }

        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ExtendUseExcelDefine
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

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenuEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenuEnableControl MeunEnable { get; set; }

        #endregion Popertys

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ContractRegeditBookPanel()
        {
            InitializeComponent();
            DataContext = this;
            Summary = new ContractRegeditBookSummary();
            virtualType = eVirtualType.Land;
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
        }

        #endregion Ctor

        #region Methods

        #region Methods - Public

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InlitialControl(string zoneCode)
        {
            if (Summary != null)
            {
                Summary.InitialData();
            }
            queueQuery.Cancel();
            queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    MeunEnable(false);
                    if (HaveOherDoWork != null)
                    {
                        HaveOherDoWork(go);
                    }
                    else
                    {
                        DoWork(go);
                    }
                },
                completed =>
                {
                    MeunEnable();
                    view.Roots = Items;
                    DataCount();
                    view.IsEnabled = true;
                },
                terminated =>
                {
                    MeunEnable();
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    view.IsEnabled = false;
                    ThePage.Page.IsBusy = true;
                    lbzone.Text = "";
                    Items.Clear();
                    view.Roots = null;
                },
                ended =>
                {
                    ThePage.Page.IsBusy = false;
                }, null, null, zoneCode);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void DoWork(TaskGoEventArgs arg)
        {
            if (PersonBusiness == null || ContractRegeditBookBusiness == null)
            {
                return;
            }
            zoneName = PersonBusiness.GetUinitName(currentZone);
            arg.Instance.ReportProgress(1, zoneName);
            string zoneCode = arg.Instance.Argument.UserState as string;

            List<VirtualPerson> vps = null;
            List<ContractConcord> concords = null;
            List<ContractRegeditBook> regeditBooks = null;
            List<ContractLand> contractlands = null;

            if (!string.IsNullOrEmpty(zoneCode))
            {
                vps = PersonBusiness.GetByZone(zoneCode);
                regeditBooks = ContractRegeditBookBusiness.GetByZoneCode(zoneCode, eSearchOption.Precision);
                //regeditBooks.Sort((a, b) =>
                //{
                //    long aNumber = Convert.ToInt64(a.SerialNumber);
                //    long bNumber = Convert.ToInt64(b.SerialNumber);
                //    return aNumber.CompareTo(bNumber);
                //});
                concords = ConcordBusiness.GetContractsByZoneCode(zoneCode, eLevelOption.Self);
                contractlands = AccountLandBusiness.GetCollection(zoneCode, eLevelOption.Self);
            }
            if (regeditBooks == null)
            {
                return;
            }
            if (vps != null && vps.Count > 0 && concords != null && concords.Count > 0)
            {
                foreach (var item in vps)
                {
                    if (arg.Instance.IsStopPending)
                        break;
                    //获取承包方对应的合同
                    List<ContractConcord> concordlist = concords.FindAll(t => t.ContracterId == item.ID);
                    if (concordlist == null || concordlist.Count == 0)
                        continue;

                    //获取合同对应的权证
                    List<ContractRegeditBook> regeditBooklist = new List<YuLinTu.Library.Entity.ContractRegeditBook>();

                    foreach (var ccditem in concordlist)
                    {
                        var getitem = regeditBooks.Find(t => t.ID == ccditem.ID && !string.IsNullOrEmpty(t.RegeditNumber));
                        if (getitem != null)
                        {
                            regeditBooklist.Add(getitem as YuLinTu.Library.Entity.ContractRegeditBook);
                        }
                    }
                    if (regeditBooklist == null || regeditBooklist.Count == 0)
                        continue;
                    //regeditBooklist.Sort((a,b)=> {
                    //    long aNumber = Convert.ToInt64(a.SerialNumber);
                    //    long bNumber = Convert.ToInt64(b.SerialNumber);
                    //    return aNumber.CompareTo(bNumber);
                    //});
                    //添加显示-传入人、对应权证、总合同、总地
                    ContractRegeditBookItem vpi = ContractRegeditBookItemHelper.ConvertToItem(item, regeditBooklist, concordlist, contractlands);
                    arg.Instance.ReportProgress(50, vpi);
                }
                vps.Clear();
                concords.Clear();
                regeditBooks.Clear();
                contractlands.Clear();
            }
        }

        /// <summary>
        /// 获取数据完成
        /// </summary>
        private void Changed(TaskProgressChangedEventArgs arg)
        {
            if (arg.Percent == 1)
            {
                lbzone.Text = arg.UserState as string;
                Summary.InitialData();
                return;
            }
            ContractRegeditBookItem item = arg.UserState as ContractRegeditBookItem;
            if (item != null)
            {
                Items.Add(item);
            }
        }

        /// <summary>
        /// 加载数据的时候统计数据属性
        /// </summary>
        /// <param name="item"></param>
        private void DataSummary(ContractRegeditBookItem item)
        {
            double summaryActualArea = 0.0;
            double summaryAwareArea = 0.0;

            foreach (var child in item.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                Summary.WarrantCount += (child.Visibility == Visibility.Visible) ? 1 : 0;
                summaryActualArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.CountActualArea) : 0;
                summaryAwareArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.CountAwareArea) : 0;
                Summary.LandCount += (child.Visibility == Visibility.Visible) ? child.CountLand : 0;
            }

            Summary.ArwareAreaCount += Math.Round(summaryAwareArea, 4);
            Summary.ActualAreaCount += Math.Round(summaryActualArea, 4);
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        public void DataCount()
        {
            Summary.InitialData();
            double summaryActualArea = 0.0;
            double summaryAwareArea = 0.0;
            foreach (var item in Items)
            {
                if (!view.IsItemVisible(item))
                //(item.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                foreach (var child in item.Children)
                {
                    if (!view.IsItemVisible(item))
                    //(child.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }
                    Summary.WarrantCount += (child.Visibility == Visibility.Visible) ? 1 : 0;
                    summaryActualArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.CountActualArea) : 0;
                    summaryAwareArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.CountAwareArea) : 0;
                    Summary.LandCount += (child.Visibility == Visibility.Visible) ? child.CountLand : 0;
                }
            }
            summaryActualArea = Business.ToolMath.SetNumericFormat(summaryActualArea, 4, 1);
            summaryAwareArea = Business.ToolMath.SetNumericFormat(summaryAwareArea, 4, 1);

            Summary.ArwareAreaCount = Business.ToolMath.SetNumbericFormat(summaryAwareArea.ToString(), 2);//Math.Round(summaryAwareArea, 4);
            Summary.ActualAreaCount = Business.ToolMath.SetNumbericFormat(summaryActualArea.ToString(), 2);//Math.Round(summaryActualArea, 4);
        }

        #endregion Methods - Public

        #region 添加权证数据

        /// <summary>
        /// 添加权证
        /// </summary>
        public void AddRegeditBook()
        {
            if (CurrentZone == null)
            {
                ShowBox("承包权证", "请选择添加权证所在行政区域!");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Village)
            {
                ShowBox("承包权证", "请在村、组级地域下添加权证!");
                return;
            }
            List<ContractConcord> concords = new List<ContractConcord>();
            concords = ConcordBusiness.GetCollection(CurrentZone.FullCode);
            if (concords == null || concords.Count == 0)
            {
                ShowBox("承包权证", "当前行政区域下还没有合同被签订，不能添加权证!");
                return;
            }
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            RegeditBookInfoPage regeditBookPage = new RegeditBookInfoPage();
            regeditBookPage.dbContext = DbContext;
            regeditBookPage.BookStation = DbContext.CreateRegeditBookStation();
            regeditBookPage.DictBusiness = DictBusiness;

            regeditBookPage.AccountLandBusiness = AccountLandBusiness;
            regeditBookPage.ConcordBusiness = ConcordBusiness;
            regeditBookPage.ContractRegeditBookBusiness = ContractRegeditBookBusiness;
            regeditBookPage.personStation = personStation;
            regeditBookPage.CurrentZone = currentZone;
            regeditBookPage.Items = Items;
            regeditBookPage.Workpage = ThePage;
            bool canContinue = regeditBookPage.InitalizeContractor(true);
            if (!canContinue)
            {
                ShowBox("承包权证", "当前行政区域下所有权证已经签订,不能再进行添加!");
                return;
            }

            ThePage.Page.ShowDialog(regeditBookPage, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    var person = regeditBookPage.CurrentFamily;
                    List<ContractConcord> listConcord = new List<ContractConcord>();
                    List<YuLinTu.Library.Entity.ContractRegeditBook> listRegedit = new List<YuLinTu.Library.Entity.ContractRegeditBook>();
                    listConcord.Add(regeditBookPage.Concord);    //添加合同
                    listRegedit.Add(regeditBookPage.CurrentRegeditBook); //添加权证
                    if (regeditBookPage.CurrentRegeditBook == null)
                        return;
                    List<ContractLand> listLands = AccountLandBusiness.GetCollection(person.ID);
                    currentItem = ContractRegeditBookItemHelper.ConvertToItem(person, listRegedit, listConcord, listLands);
                    var item = Items.FirstOrDefault(c => c.Tag.ID == currentItem.Tag.ID);
                    if (item == null)
                    {
                        Items.Add(currentItem);
                    }
                    else
                    {
                        List<ContractLand> cbLandlist = listLands.FindAll(t => t.ConcordId == regeditBookPage.Concord.ID);
                        item.Children.Add(new BindContractRegeditBook(regeditBookPage.CurrentRegeditBook, regeditBookPage.Concord, cbLandlist.Count));
                    }
                    currentItem = null;
                    VirtualPersonExpand vpexpand = null;
                    if (person.FamilyExpand != null)
                    {
                        vpexpand = person.FamilyExpand;
                    }
                    else
                    {
                        vpexpand = new VirtualPersonExpand();
                    }
                    vpexpand.WarrantNumber = regeditBookPage.CurrentRegeditBook.Number;
                    person.FamilyExpand = vpexpand;
                    personStation.Update(person);
                }));
                DataCount();
            });
        }

        #endregion 添加权证数据

        #region 更新权证数据

        /// <summary>
        /// 更新数据
        /// </summary>
        public void EditRegeditBook()
        {
            if (CurrentZone == null)
            {
                ShowBox("编辑权证", "请选择编辑权证所在行政区域!");
                return;
            }
            if (currentRegeditBook == null)
            {
                ShowBox("编辑权证", "请选择待编辑的权证!");
                return;
            }
            //if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            //{
            //    ShowBox("编辑权证", "承包方被锁定,不能编辑承包权证!");
            //    return;
            //}
            YuLinTu.Library.Entity.ContractRegeditBook regeditBook = currentRegeditBook.Tag as YuLinTu.Library.Entity.ContractRegeditBook;
            //ContractConcord concord = ConcordBusiness.Get(regeditBook.RegeditNumber);
            ContractConcord concord = ConcordBusiness.Get(regeditBook.ID);
            Edit(concord, regeditBook);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="concord"></param>
        public void Edit(ContractConcord concord, ContractRegeditBook regeditBook)
        {
            if (concord == null)
            {
                return;
            }
            if (regeditBook == null)
            {
                return;
            }
            RegeditBookInfoPage editPage = new RegeditBookInfoPage();
            editPage.dbContext = DbContext;
            editPage.BookStation = DbContext.CreateRegeditBookStation();
            editPage.AccountLandBusiness = AccountLandBusiness;
            editPage.ConcordBusiness = ConcordBusiness;
            editPage.ContractRegeditBookBusiness = ContractRegeditBookBusiness;
            editPage.personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            editPage.CurrentZone = currentZone;
            editPage.Items = Items;
            editPage.Concord = concord;
            editPage.CurrentVp = currentItem.Tag;
            editPage.CurrentRegeditBook = regeditBook;
            bool canContinue = editPage.InitalizeContractor(false);
            editPage.Workpage = ThePage;
            ThePage.Page.ShowDialog(editPage, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    List<ContractConcord> concords = new List<ContractConcord>();
                    concords.Add(editPage.Concord);    //编辑的合同
                    List<YuLinTu.Library.Entity.ContractRegeditBook> listRegedits = new List<YuLinTu.Library.Entity.ContractRegeditBook>();
                    listRegedits.Add(editPage.CurrentRegeditBook);
                    List<ContractLand> listLands = AccountLandBusiness.GetCollection(currentItem.Tag.ID);
                    ContractRegeditBookItem entity = ContractRegeditBookItemHelper.ConvertToItem(currentItem.Tag, listRegedits, concords, listLands);
                    BindContractRegeditBook regeditBookBind = entity.Children.FirstOrDefault(t => t.ID == currentRegeditBook.ID);
                    currentRegeditBook.CopyPropertiesFrom(regeditBookBind);
                }));
                DataCount();
            });
        }

        #endregion 更新权证数据

        #region 删除权证数据

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DelRegeditBook()
        {
            if (currentZone == null)
            {
                ShowBox("删除权证", "请选择删除权证所在行政区域!");
                return;
            }
            if (currentRegeditBook == null)
            {
                ShowBox("删除权证", "请选择待删除的权证!");
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox("删除权证", "承包方被锁定,不能删除承包权证!");
                return;
            }
            ////ContractConcord concord = ConcordBusiness.Get(currentRegeditBook.RegeditNumber);
            ////if (concord == null) return;
            //VirtualPerson vp = PersonBusiness.GetVirtualPersonByID((Guid)concord.ContracterId);
            //if (vp.Status == eVirtualPersonStatus.Lock)
            //{
            //    ShowBox("删除权证", "选择权证数据已经锁定, 不允许删除!");
            //    return;
            //}
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    int ret = ContractRegeditBookBusiness.Delete(currentRegeditBook.Tag.ID);
                    if (ret != 0)
                    {
                        var item = currentItem.Clone() as ContractRegeditBookItem;
                        currentItem.Children.Remove(currentRegeditBook);
                        if (currentItem == null)
                            currentItem = Items.FirstOrDefault(c => c.Tag.ID == item.Tag.ID);
                        if (currentItem.Children.Count == 0)
                        {
                            Items.Remove(currentItem);
                        }
                    }
                    DataCount();
                }
                catch (Exception ex)
                {
                    ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonFail);
                    YuLinTu.Library.Log.Log.WriteException(this, "DelRegeditBook(删除权证)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(ContractRegeditBookInfo.DelData, ContractRegeditBookInfo.DelDataWarring, eMessageGrade.Warn, action);
            //TabMessageBoxDialog message = new TabMessageBoxDialog()
            //{
            //    Header = ContractRegeditBookInfo.DelData,
            //    Message = ContractRegeditBookInfo.DelDataWarring,
            //    MessageGrade = eMessageGrade.Warn
            //};
            //ThePage.Page.ShowMessageBox(message, (b, e) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //    try
            //    {
            //        int ret = ContractRegeditBookBusiness.Delete(currentRegeditBook.Tag.ID);
            //        if (ret != 0)
            //        {
            //            currentItem.Children.Remove(currentRegeditBook);
            //            if (currentItem.Children.Count == 0)
            //            {
            //                Items.Remove(currentItem);
            //            }
            //        }
            //        DataCount();
            //    }
            //    catch (Exception ex)
            //    {
            //        ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonFail);
            //        YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
            //    }
            //});
        }

        #endregion 删除权证数据

        #region 权证登记

        /// <summary>
        /// 权证登记(初始化权证数据)
        /// </summary>
        public void InitalizeWarrent()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractRegeditBookInfo.InitialWarrantInfo, ContractRegeditBookInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractRegeditBookInfo.InitialWarrantInfo, ContractRegeditBookInfo.InitialBatchSelectedZoneError);
                return;
            }
            List<Zone> allZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            allZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);

            if (CurrentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
            {
                var landscount = AccountLandBusiness.CountLandByZone(CurrentZone.FullCode);
                if (landscount == 0)
                {
                    ShowBox(ContractRegeditBookInfo.InitialWarrantInfo, ContractRegeditBookInfo.LandDataNull);
                    return;
                }
                List<ContractConcord> concords = ConcordBusiness.GetCollection(CurrentZone.FullCode);
                if (concords == null || concords.Count == 0)
                {
                    ShowBox("承包权证", "当前行政区域下还没有合同被签订，不能添加权证!");
                    return;
                }
                List<ContractConcord> useConcords = CreateConcordCollection(concords);//筛选出未被锁定的承包方对应合同集合
                if (useConcords == null) return;
                var rbStation = DbContext.CreateRegeditBookStation();
                List<ContractRegeditBook> regeditbooksbefore = new List<ContractRegeditBook>();//签订权证前的权证
                regeditbooksbefore = rbStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);

                bool isBatch = CurrentZone.Level > eZoneLevel.Group ? true : false;
                RegeditBookInfoSettingPage initialPage = new RegeditBookInfoSettingPage(isBatch, DbContext);
                initialPage.Workpage = ThePage;
                initialPage.CurrentZone = CurrentZone;
                initialPage.PersonBusiness = PersonBusiness;
                initialPage.AccountLandBusiness = AccountLandBusiness;
                initialPage.ConcordBusiness = ConcordBusiness;
                initialPage.ContractRegeditBookBusiness = ContractRegeditBookBusiness;
                initialPage.DictBusiness = DictBusiness;
                initialPage.ListConcord = useConcords;
                initialPage.AllZones = allZones;
                initialPage.RegeditBooksBefore = regeditbooksbefore;
                //initialPage.LandsOfInitialConcord = lands;
                ThePage.Page.ShowMessageBox(initialPage, (t, s) =>
                {
                    if (!(bool)t)
                    {
                        return;
                    }
                    InitialContractBookTask(initialPage);
                });
            }
            else if (CurrentZone.Level == eZoneLevel.Village || CurrentZone.Level == eZoneLevel.Town)
            {
                List<ContractLand> lands = AccountLandBusiness.GetCollection(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                if (lands == null || lands.Count == 0)
                {
                    ShowBox(ContractRegeditBookInfo.InitialWarrantInfo, ContractRegeditBookInfo.LandDataNull);
                    return;
                }
                List<ContractConcord> concords = ConcordBusiness.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                if (concords == null || concords.Count == 0)
                {
                    ShowBox("承包权证", "当前行政区域下还没有合同被签订，不能添加权证!");
                    return;
                }
                List<ContractConcord> useConcords = CreateConcordCollection(concords);//筛选出未被锁定的承包方对应合同集合
                if (useConcords == null) return;
                var rbStation = DbContext.CreateRegeditBookStation();
                List<ContractRegeditBook> regeditbooksbefore = new List<ContractRegeditBook>();//签订权证前的权证
                regeditbooksbefore = rbStation.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);

                bool isBatch = CurrentZone.Level > eZoneLevel.Group ? true : false;
                RegeditBookInfoSettingPage initialPage = new RegeditBookInfoSettingPage(isBatch, DbContext);
                initialPage.Workpage = ThePage;
                initialPage.CurrentZone = CurrentZone;
                initialPage.PersonBusiness = PersonBusiness;
                initialPage.AccountLandBusiness = AccountLandBusiness;
                initialPage.ConcordBusiness = ConcordBusiness;
                initialPage.ContractRegeditBookBusiness = ContractRegeditBookBusiness;
                initialPage.DictBusiness = DictBusiness;
                initialPage.ListConcord = useConcords;
                initialPage.AllZones = allZones;
                initialPage.RegeditBooksBefore = regeditbooksbefore;
                //initialPage.LandsOfInitialConcord = lands;
                ThePage.Page.ShowMessageBox(initialPage, (t, s) =>
                {
                    if (!(bool)t)
                    {
                        return;
                    }
                    TaskGroupInitialContractBook(initialPage);
                });
            }
        }

        /// <summary>
        /// 按照组初始化权证数据任务
        /// </summary>
        /// <param name="initialPage">初始化界面</param>
        private void InitialContractBookTask(RegeditBookInfoSettingPage initialPage)
        {
            TaskContractRegeditBookArgument argument = new TaskContractRegeditBookArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.Concords = initialPage.ListConcord;
            argument.ListWarrants = initialPage.RegeditBooksBefore;
            argument.WarrantsModified = initialPage.WarrantsModified;
            List<VirtualPerson> SVps = selectVps.Clone() as List<VirtualPerson>;
            argument.listPerson = SVps;
            argument.ArgType = eContractRegeditBookType.InitialWarrantData;
            var operation = new TaskContractRegeditBookOperation();
            operation.Argument = argument;
            operation.Description = "初始化" + CurrentZone.FullName + "权证";  //任务描述
            operation.Name = ContractRegeditBookInfo.WarrantDataHandle;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                selectVps.Clear();
                Refresh();

                ThePage.Message.Send(this,
                    MessageExtend.ContractWarrantMsg(DbContext, ContractRegeditBookMessage.CONTRACTREGEDITBOOK_GET_COMPLATE, initialPage.WarrantsModified, CurrentZone.FullCode));
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 按照村、镇初始化权证数据任务
        /// </summary>
        /// <param name="initialPage">初始化界面</param>
        private void TaskGroupInitialContractBook(RegeditBookInfoSettingPage initialPage)
        {
            TaskGroupInitalizeWarrentArgument argument = new TaskGroupInitalizeWarrentArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.Concords = initialPage.ListConcord;
            argument.ListWarrants = initialPage.RegeditBooksBefore;
            argument.WarrantsModified = initialPage.WarrantsModified;
            List<VirtualPerson> SVps = selectVps.Clone() as List<VirtualPerson>;
            argument.listPerson = SVps;
            argument.AllZones = initialPage.AllZones;
            argument.ArgType = eContractRegeditBookType.InitialWarrantData;
            TaskGroupInitalizeWarrentOperation operation = new TaskGroupInitalizeWarrentOperation();
            operation.Argument = argument;
            operation.Description = "初始化" + CurrentZone.FullName + "权证";   //任务描述
            operation.Name = ContractRegeditBookInfo.WarrantDataHandle;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                selectVps.Clear();
                Refresh();

                ThePage.Message.Send(this,
                    MessageExtend.ContractWarrantMsg(DbContext, ContractRegeditBookMessage.CONTRACTREGEDITBOOK_GET_COMPLATE, initialPage.WarrantsModified, CurrentZone.FullCode));
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        #endregion 权证登记

        #region 数据过滤

        /// <summary>
        /// 快速过滤
        /// </summary>
        private void txtWhere_TextChanged(object sender, TextChangedEventArgs e)
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

        /// <summary>
        /// 数据过滤
        /// </summary>
        private void DataFilter(string whName)
        {
            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    this.Dispatcher.Invoke(new Action(() => { SetItemVisible(go); }));// SetItemVisible(go);
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
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

                var dataItem = obj as ContractRegeditBookItem;
                if (dataItem != null)
                {
                    txt = dataItem.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }
                var book = obj as BindContractRegeditBook;
                if (book != null)
                {
                    txt = book.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = book.Comment;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = book.ContractTime;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = book.CountActualArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = book.CountAwareArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = book.Number;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = book.RegeditNumber;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = book.SenderName;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = book.SerialNumber;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }

                return has;
            }, true);
            //foreach (var dataItem in Items)
            //{
            //    bool Iscontains = JudgeItem(whString, allCheck, dataItem);
            //    if (Iscontains)
            //    {
            //        continue;
            //    }
            //    bool allHidden = true;
            //    dataItem.Visibility = Visibility.Visible;
            //    foreach (BindContractRegeditBook p in dataItem.Children)
            //    {
            //        p.Visibility = Visibility.Collapsed;
            //        if ((allCheck && ((p.Comment != null && p.Comment.Equals(whString)) || p.CountActualArea.ToString().Equals(whString) || p.CountAwareArea.ToString().Equals(whString) || p.Number.Equals(whString) || p.Name.Equals(whString) || p.ContractTime.Equals(whString) || p.CountLand.Equals(whString))) || p.CountPrint.Equals(whString) || p.SenderName.Equals(whString) ||
            //            (!allCheck && ((p.Comment != null && p.Comment.Contains(whString) || p.CountActualArea.ToString().Contains(whString) || p.CountAwareArea.ToString().Contains(whString) || p.Number.Contains(whString)) || p.Name.Contains(whString)) || p.ContractTime.Contains(whString) || p.CountLand.ToString().Contains(whString) || p.CountPrint.ToString().Contains(whString) || p.SenderName.Contains(whString)))
            //        {
            //            allHidden = false;
            //            p.Visibility = Visibility.Visible;
            //        }
            //    }
            //    if (allHidden)
            //    {
            //        dataItem.Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        /// <summary>
        /// 项的过滤:如果项符合条件,则全显示(true),否则不显示(false)
        /// </summary>
        private bool JudgeItem(string whString, bool allInfo, ContractRegeditBookItem dataItem)
        {
            bool contains = dataItem.Name.Contains(whString);
            bool equals = dataItem.Name.Equals(whString);
            if ((allInfo && equals) || (!allInfo && contains))
            {
                dataItem.Visibility = Visibility.Visible;
                foreach (var item in dataItem.Children)
                {
                    item.Visibility = Visibility.Visible;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 全匹配
        /// </summary>
        private void cb_ComplateInfo_Click(object sender, RoutedEventArgs e)
        {
            string whName = txtWhere.Text.Trim();
            if (cb_ComplateInfo.IsChecked == false)
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

        /// <summary>
        /// 清空当前地域合同
        /// </summary>
        public void Clear()
        {
            if (currentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.Clear, ContractRegeditBookInfo.CurrentZoneNoSelected);
                return;
            }
            if ((currentZone.Level == eZoneLevel.Group && Items != null && Items.Count(c => c.Tag.Status == eVirtualPersonStatus.Right) == 0))
            {
                ShowBox(ContractRegeditBookInfo.Clear, ContractRegeditBookInfo.ClearNoValidData);
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    var dbContext = DataBaseSource.GetDataBaseSource();
                    var bookStation = dbContext.CreateRegeditBookStation();
                    var cnt = bookStation.DeleteByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs, eVirtualPersonStatus.Right);
                    cnt.ToString();
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除未被锁定的权证失败!)", ex.Message + ex.StackTrace);
                    ShowBox(ContractRegeditBookInfo.Clear, ContractRegeditBookInfo.ContractRegeditBookClearFail);
                }
                finally
                {
                    var removeData = Items.Where(c => c.Tag.Status == eVirtualPersonStatus.Right).TryToList();
                    foreach (var data in removeData)
                    {
                        Items.Remove(data);
                    }
                    DataCount();
                    var args = MessageExtend.CreateMsg(CreateDb(), ContractRegeditBookMessage.CLEAR_COMPLATE, CurrentZone.FullCode);
                    SendMessasge(args);
                }
            });
            ShowBox(ContractRegeditBookInfo.Clear, ContractRegeditBookInfo.ClearConfirm, eMessageGrade.Warn, action);

            //TabMessageBoxDialog message = new TabMessageBoxDialog()
            //{
            //    Header = ContractRegeditBookInfo.Clear,
            //    Message = ContractRegeditBookInfo.ClearConfirm,
            //    MessageGrade = eMessageGrade.Warn
            //};
            //ThePage.Page.ShowMessageBox(message, (b, e) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //    ContractRegeditBookBusiness.DeleteByZoneCode(currentZone.FullCode, eSearchOption.Precision);
            //    Items.Clear();
            //    DataCount();
            //    TheBns.Current.Message.Send(this, MessageExtend.CreateMsg(CreateDb(), ContractRegeditBookMessage.CLEAR_COMPLATE, CurrentZone.FullCode));
            //});
        }

        /// <summary>
        /// 刷新统计及界面
        /// </summary>
        public void Refresh()
        {
            if (currentZone == null)
            {
                return;
            }
            InlitialControl(currentZone.FullCode);
        }

        #endregion 数据过滤

        #region 证书数据处理-证书预览及导出

        /// <summary>
        /// 打印证书设置
        /// </summary>
        public void PrintSettingName()
        {
            ContractRegeditBookPageSetting bookSetting = new ContractRegeditBookPageSetting();
            ThePage.Page.ShowMessageBox(bookSetting, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                BookPersonNum = bookSetting.BookPersonNum;
                BookLandNum = bookSetting.BookLandNum;
                BookNumSetting = bookSetting.BookNumSetting;
            });
        }

        /// <summary>
        /// 预览证书
        /// </summary>
        public void PrintViewWarrant()
        {
            IDbContext dbContext = CreateDb();
            if (currentItem == null)
            {
                ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "请选择数据项进行证书预览!");
                return;
            }
            try
            {
                var bookStaion = dbContext.CreateRegeditBookStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var zoneStaion = dbContext.CreateZoneWorkStation();
                var zonelist = GetParentZone(currentZone, dbContext);
                zonelist.Add(currentZone);
                var concords = ConcordBusiness.GetCollection(currentItem.Tag.ID);
                if (concords == null || concords.Count == 0)
                {
                    ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "未获取合同数据,无法进行证书的预览!");
                    return;
                }
                List<Guid> ids = new List<Guid>(concords.Count);
                concords.ForEach(c => ids.Add(c.ID));
                var books = bookStaion.GetByConcordIds(ids.ToArray());
                if (books == null || books.Count == 0)
                {
                    ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "未获取权证数据,无法进行证书的预览!");
                    return;
                }
                var lands = landStation.GetByConcordIds(ids.ToArray());
                VirtualPerson vpn = currentItem.Tag;
                var familyConcord = concords.Find(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                var familyBook = familyConcord == null ? null : books.Find(c => c.ID == familyConcord.ID);
                var otherConcord = concords.Find(c => c.ArableLandType != ((int)eConstructMode.Family).ToString());
                var otherBook = otherConcord == null ? null : books.Find(c => c.ID == otherConcord.ID);
                var parentsToProvince = zoneStaion.GetParentsToProvince(currentZone);
                CollectivityTissue Tissue = ConcordBusiness.GetSenderById(concords[0].SenderId);
                if (Tissue == null)
                {
                    var senderStation = dbContext.CreateSenderWorkStation();
                    Tissue = senderStation.Get(currentZone.ID);
                }
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractRegeditBookWord);
                List<Dictionary> DictList = DictBusiness.GetAll();
                lands.LandNumberFormat(SystemSet);
                ContractWarrantPrinter printContract = new ContractWarrantPrinter();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(printContract);
                if (temp != null && temp is ContractWarrantPrinter)
                {
                    printContract = (ContractWarrantPrinter)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                printContract.dbContext = dbContext;
                printContract.ZoneList = zonelist;
                printContract.ParentsToProvince = parentsToProvince;
                printContract.CurrentZone = CurrentZone;
                printContract.RegeditBook = familyBook;
                printContract.Book = familyBook;
                printContract.Concord = familyConcord;
                printContract.OtherBook = otherBook;
                printContract.OtherConcord = otherConcord;
                printContract.LandCollection = lands;
                printContract.Contractor = vpn;
                printContract.Tissue = Tissue;
                printContract.DictList = DictList;
                printContract.TempleFilePath = tempPath;
                printContract.UseExcel = ExtendUseExcelDefine.WarrantExtendByExcel;
                printContract.BookPersonNum = BookPersonNum;
                printContract.BookLandNum = BookLandNum;
                printContract.BookNumSetting = BookNumSetting;
                printContract.PrintContractLand(false);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 导出证书
        /// </summary>
        public void ExportWarrant()
        {
            //导出excel表业务类型，默认为承包方调查表
            if (CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                GetSelectItem();
                IDbContext dbContext = DbContext; // CreateDb();
                if (dbContext == null)
                {
                    ShowBox(ContractRegeditBookInfo.ExportWarrant, DataBaseSource.ConnectionError, eMessageGrade.Error, null, false);
                    return;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                    if (isbatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage caplp = new ContractRegeditBookPersonLockPage();
                        foreach (var item in Items)
                        {
                            getpersonst.Add(item.Tag);
                        }
                        if (getpersonst == null) return;
                        caplp.PersonItems = getpersonst;
                        caplp.Business = PersonBusiness;
                        ThePage.Page.ShowMessageBox(caplp, (b, e) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (caplp.SelectedPersons == null || caplp.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportWarrantNoSelected);
                                return;
                            }
                            ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.ExportWarrant,
                                ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant, caplp.SelectedPersons);
                        });
                    }
                    else //if (currentItem != null)
                    {
                        if (currentItem == null)
                        {
                            ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportWarrantNoSelected);
                            return;
                        }
                        getpersonst.Add(currentItem.Tag);
                        ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.ExportWarrant,
                              ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant, getpersonst);
                    }
                    //else
                    //{
                    //    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, ContractRegeditBookInfo.ExportWarrant);
                    //    extPage.Workpage = ThePage;
                    //    ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                    //    {
                    //        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                    //        {
                    //            return;
                    //        }
                    //        ExportSingleWarrant(extPage.FileName);
                    //        ShowBox(ContractRegeditBookInfo.ExportData, "导出成功", eMessageGrade.Infomation);
                    //    });
                    //}
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    var warrantsNum = ContractRegeditBookBusiness.Count(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    if (warrantsNum == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrant, "当前地域下没有权证数据", eMessageGrade.Error);
                        return;
                    }
                    ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.BatchExportWarrant,
                        ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrant(导出证书)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出证书(单任务)
        /// </summary>
        private void ExportWarrentTask(string fileName, string taskDes, string taskName, List<VirtualPerson> listPerson)
        {
            TaskExportWarrentArgument meta = new TaskExportWarrentArgument();
            meta.FileName = fileName;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.SystemDefine = this.SystemSet;
            meta.SelectedPersons = listPerson;
            meta.BookPersonNum = BookPersonNum;
            meta.BookLandNum = BookLandNum;
            meta.BookNumSetting = BookNumSetting;
            meta.ExtendUseExcelDefine = ExtendUseExcelDefine;
            TaskExportWarrentOperation import = new TaskExportWarrentOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导出证书(组任务)
        /// </summary>
        private void ExportWarrentTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportWarrentArgument groupArgument = new TaskGroupExportWarrentArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.SystemDefine = this.SystemSet;
            groupArgument.ExtendUseExcelDefine = ExtendUseExcelDefine;
            groupArgument.BookLandNum = BookLandNum;
            groupArgument.BookPersonNum = BookPersonNum;
            groupArgument.BookNumSetting = BookNumSetting;
            TaskGroupExportWarrentOperation groupOperation = new TaskGroupExportWarrentOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 证书数据处理-证书预览及导出

        #region 登记簿处理

        /// <summary>
        /// 预览登记薄
        /// </summary>
        public void PrivewRegeditBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            if (currentRegeditBook == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "请选择待预览的登记簿!");
                return;
            }
            ContractConcord concord = ConcordBusiness.Get(currentRegeditBook.Tag.ID);
            if (concord == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "请选择的权证没有合同!");
                return;
            }
            ContractRegeditBookBusiness.SystemSet = SystemSet;
            ContractRegeditBookBusiness.PrivewRegeditBookWord(CurrentZone, concord);
        }

        /// <summary>
        /// 导出登记簿
        /// </summary>
        public void ExportRegeditBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);

                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    if (!isbatch && currentItem == null)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportRegeditBookTableNoSelected);
                        return;
                    }
                    if (isbatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportRegeditBookTableNoSelected);
                                return;
                            }
                            ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportRegeditBookTable, eContractRegeditBookType.ExportRegeditBookData,
                                ContractRegeditBookInfo.ExportRegeditBookTableDesc, ContractRegeditBookInfo.ExportWarrentData, selectPage.SelectedPersons);
                        });
                    }
                    else if (currentItem != null && currentRegeditBook == null)
                    {
                        listPerson.Add(currentItem.Tag);
                        ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportRegeditBookTable, eContractRegeditBookType.ExportRegeditBookData,
                               ContractRegeditBookInfo.ExportRegeditBookTableDesc, ContractRegeditBookInfo.ExportWarrentData, listPerson);
                    }
                    else
                    {
                        ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, ContractRegeditBookInfo.ExportRegeditBookTable);
                        extPage.Workpage = ThePage;
                        ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                            {
                                return;
                            }
                            string fileDir = extPage.FileName;
                            string message = null;
                            ContractConcord concord = ConcordBusiness.Get(currentRegeditBook.Tag.ID);
                            if (concord == null)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "选择的权证没有合同!");
                                return;
                            }
                            ContractRegeditBookBusiness.SystemSet = this.SystemSet;
                            bool flag = ContractRegeditBookBusiness.SingleExportRegeditBookWord(CurrentZone, concord, fileDir, out message);
                            if (flag == false)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, message);
                            }
                            else
                            {
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, message, eMessageGrade.Infomation);
                            }
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperateTask(CurrentZone.FullName, ContractRegeditBookInfo.ExportRegeditBookTable, eContractRegeditBookType.BatchExportRegeditBookData,
                ContractRegeditBookInfo.ExportRegeditBookTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportRegeditBook(导出登记簿)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出登记簿(单个任务)
        /// </summary>
        private void ExportRegeditBookTask(string fileName, string taskDes, string taskName, List<VirtualPerson> listPerson)
        {
            TaskExportRegeditBookArgument meta = new TaskExportRegeditBookArgument();
            meta.FileName = fileName;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            //meta.DateValue = time;
            //meta.PubDateValue = pubTime;
            meta.SystemDefine = this.SystemSet;
            meta.SelectedPersons = listPerson;
            TaskExportRegeditBookOperation import = new TaskExportRegeditBookOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导出登记簿(组任务)
        /// </summary>
        private void ExportRegeditBookTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportRegeditBookArgument groupArgument = new TaskGroupExportRegeditBookArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            //meta.DateValue = time;
            //meta.PubDateValue = pubTime;
            groupArgument.SystemDefine = this.SystemSet;
            TaskGroupExportRegeditBookOperation groupOperation = new TaskGroupExportRegeditBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 登记簿处理

        #region 工具(数据汇总表、单户确认表及颁证清册)

        /// <summary>
        /// 导出权证数据汇总表
        /// </summary>
        public void ExportWarrentSummaryTable()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<ContractConcord> concords = ConcordBusiness.GetCollection(currentZone.FullCode);
                List<ContractRegeditBook> books = ContractRegeditBookBusiness.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                // || ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0))
                {
                    //单个任务
                    if (concords == null || concords.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportTableDataNoConcords);
                        return;
                    }
                    if (books == null || books.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    //bool isExsit = concords.Any(c => ContractRegeditBookBusiness.Get(c.ID) != null);
                    bool isExsit = concords.Any(c => books.Find(t => t.ID == c.ID) != null);
                    if (!isExsit)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportDataError);
                        return;
                    }
                    ExportWarrantSummaryTask(SystemSet.DefaultPath, ContractRegeditBookInfo.ExportWarrentSummaryTableDesc, ContractRegeditBookInfo.ExportWarrentSummaryTable);
                    //ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrentSummaryTable, eContractRegeditBookType.ExportWarrentSummaryData,
                    //    ContractRegeditBookInfo.ExportWarrentSummaryTableDesc, ContractRegeditBookInfo.ExportWarrentSummaryTable);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    concords = ConcordBusiness.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);

                    books = ContractRegeditBookBusiness.GetByZoneCode(currentZone.FullCode, eSearchOption.Fuzzy);
                    //单个任务
                    if (concords == null || concords.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportTableDataNoConcords);
                        return;
                    }
                    if (books == null || books.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    bool isExsit = concords.Any(c => ContractRegeditBookBusiness.Get(c.ID) != null);
                    if (!isExsit)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.ExportDataError);
                        return;
                    }
                    ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrentSummaryTable, eContractRegeditBookType.BatchExportWarrentSummaryData,
                        ContractRegeditBookInfo.ExportWarrentSummaryTableDesc, ContractRegeditBookInfo.ExportWarrentSummaryTable);
                    ////组任务
                    //ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportWarrentSummaryTable, eContractRegeditBookType.BatchExportWarrentSummaryData,
                    //    ContractRegeditBookInfo.ExportWarrentSummaryTableDesc, ContractRegeditBookInfo.ExportWarrentSummaryTable);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportWarrentSummaryTable, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrentSummaryTable(导出权证数据汇总表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出权证数据汇总表(单任务)
        /// </summary>
        private void ExportWarrantSummaryTask(string fileName, string taskDes, string taskName)
        {
            TaskExportWarrantSummaryArgument argument = new TaskExportWarrantSummaryArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.SummaryDefine = SummaryDefine;
            argument.SystemSet = SystemSet;
            argument.IsShow = true;
            TaskExportWarrantSummaryOperation operation = new TaskExportWarrantSummaryOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出权证数据汇总表(组任务)
        /// </summary>
        private void ExportWarrantSummaryTaskGroup(string fileName, string taskDes, string taskName)
        {
            //TaskExportWarrantSummaryArgument argument = new TaskExportWarrantSummaryArgument();
            //argument.DbContext = DbContext;
            //argument.CurrentZone = currentZone;
            //argument.FileName = fileName;
            //argument.SummaryDefine = SummaryDefine;
            //argument.SystemSet = SystemSet;
            //TaskExportWarrantSummaryOperation operation = new TaskExportWarrantSummaryOperation();
            //operation.Argument = argument;
            //operation.Description = taskDes;
            //operation.Name = taskName;
            //operation.Completed += new TaskCompletedEventHandler((o, t) =>
            //{
            //});
            //ThePage.TaskCenter.Add(operation);
            //if (ShowTaskViewer != null)
            //{
            //    ShowTaskViewer();
            //}
            //operation.StartAsync();

            TaskGroupExportWarrantSummaryArgument groupArgument = new TaskGroupExportWarrantSummaryArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.SystemSet = SystemSet;
            groupArgument.SummaryDefine = SummaryDefine;
            TaskGroupExportWarrantSummaryOperation groupOperation = new TaskGroupExportWarrantSummaryOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出单户确认表
        /// </summary>
        public void ExportFamilyTable()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<ContractConcord> concords = ConcordBusiness.GetCollection(currentZone.FullCode);
                List<ContractRegeditBook> books = ContractRegeditBookBusiness.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                if (!isbatch && currentItem == null)
                {
                    ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportWarrantNoSelected);
                    return;
                }
                if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //批量导出(选择地域大于组级并且当前地域下有子级地域)
                    ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable,
                        eContractRegeditBookType.BatchExportSingleFamilyConfirmData, ContractRegeditBookInfo.ExportFamilyConfirmTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                }
                else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                    if (concords == null || concords.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportTableDataNoConcords);
                        return;
                    }
                    if (books == null || books.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    bool isExsit = concords.Any(c => books.Find(t => t.ID == c.ID) != null);
                    //bool isExsit = concords.Any(c => ContractRegeditBookBusiness.Get(c.ID) != null);
                    if (!isExsit)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportDataError);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    if (isbatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.ExportFamilyConfirmNoSelected);
                                return;
                            }
                            ExportDataCommonOperateTask(currentZone.FullName, ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable,
                                eContractRegeditBookType.ExportSingleFamilyConfirmData, ContractRegeditBookInfo.ExportFamilyConfirmTableDesc, ContractRegeditBookInfo.ExportWarrentData, selectPage.SelectedPersons);
                        });
                    }
                    else
                    {
                        //界面上有当前选择承包方项(此时做预览操作)
                        listPerson.Add(currentItem.Tag);
                        //string fileName = Path.GetTempPath();    //系统临时保存路径
                        ContractRegeditBookBusiness.SystemSet = this.SystemSet;
                        ContractRegeditBookBusiness.ExportFamilyConfirmTable(currentZone, listPerson, SystemSet.DefaultPath);
                    }
                    listPerson = null;
                }
                else
                {
                    ShowBox(ContractRegeditBookInfo.ExportSingleFamlilyConfirmTable, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
                books = null;
                concords = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportFamilyTable(导出单户确认表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出单户确认表(单个任务)
        /// </summary>
        private void ExportFamilyTableTask(string fileName, string taskDes, string taskName, List<VirtualPerson> listPerson)
        {
            TaskExportFamilyConfirmArgument argument = new TaskExportFamilyConfirmArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.SystemDefine = SystemSet;
            argument.SelectedPersons = listPerson;
            TaskExportFamilyConfirmOperation operation = new TaskExportFamilyConfirmOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出单户确认表(组任务)
        /// </summary>
        private void ExportFamilyTableTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportFamilyConfirmArgument groupArgument = new TaskGroupExportFamilyConfirmArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.SystemSet = SystemSet;
            TaskGroupExportFamilyConfirmOperation groupOperation = new TaskGroupExportFamilyConfirmOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出颁证清册
        /// </summary>
        public void ExportAwareTable()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractRegeditBookInfo.ExportAwareTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<ContractConcord> concords = ConcordBusiness.GetCollection(currentZone.FullCode);
                List<ContractRegeditBook> books = ContractRegeditBookBusiness.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (concords == null || concords.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportAwareTable, ContractRegeditBookInfo.ExportTableDataNoConcords);
                        return;
                    }
                    if (books == null || books.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportAwareTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    //bool isExsit = concords.Any(c => ContractRegeditBookBusiness.Get(c.ID) != null);
                    bool isExsit = concords.Any(c => books.Find(t => t.ID == c.ID) != null);
                    if (!isExsit)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportAwareTable, ContractRegeditBookInfo.ExportDataError);
                        return;
                    }
                    ExportAwareTableTask(SystemSet.DefaultPath, ContractRegeditBookInfo.ExportAwareInventoryTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                    //    ExportDataCommonOperateTask(CurrentZone.FullName, ContractRegeditBookInfo.ExportAwareTable, eContractRegeditBookType.ExportAwareInventoryData,
                    //ContractRegeditBookInfo.ExportAwareInventoryTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperateTask(CurrentZone.FullName, ContractRegeditBookInfo.ExportAwareTable, eContractRegeditBookType.BatchExportAwareInventoryData,
                ContractRegeditBookInfo.ExportAwareInventoryTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportAwareTable, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportAwareTable(导出颁证清册表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出颁证清册(单个任务)
        /// </summary>
        private void ExportAwareTableTask(string fileName, string taskDes, string taskName)
        {
            TaskExportAwareTableArgument argument = new TaskExportAwareTableArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.IsShow = true;
            argument.UnitName = SystemSet.GetTableHeaderStr(CurrentZone);
            TaskExportAwareTableOperation operation = new TaskExportAwareTableOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出颁证清册(组任务)
        /// </summary>
        private void ExportAwareTableTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportAwareTableArgument groupArgument = new TaskGroupExportAwareTableArgument();
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.FileName = fileName;
            groupArgument.UnitName = SystemSet.GetTableHeaderStr(CurrentZone);
            TaskGroupExportAwareTableOperation groupOperation = new TaskGroupExportAwareTableOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 工具(数据汇总表、单户确认表及颁证清册)

        #region 重置流水号

        /// <summary>
        /// 重置流水号
        /// </summary>
        public void ResetSerialNumber()
        {
            ThePage.Workspace.Window.ShowDialog(new TabMessageBoxDialog
            {
                Header = "提示",
                Message = "请确认是否要重置所选行政区域的流水号！",
                MessageGrade = eMessageGrade.Infomation,
                CancelButtonText = "取消",
                ConfirmButtonVisibility = Visibility.Visible,
                CancelButtonVisibility = Visibility.Visible,
            },
            (b, r) =>
            {
                if (b == null || !b.Value)
                    return;
                if (currentZone == null)
                {
                    ShowBox(ContractRegeditBookInfo.ResetSerialNumber, ContractRegeditBookInfo.ResetSerialNumberNoZone);
                    return;
                }
                else if (currentZone.Level > eZoneLevel.County)
                {
                    ShowBox(ContractRegeditBookInfo.ResetSerialNumber, ContractRegeditBookInfo.SelectZoneError);
                    return;
                }

                var currentZoneBooksCount = ContractRegeditBookBusiness.Count(currentZone.FullCode, eLevelOption.SelfAndSubs);

                if (currentZoneBooksCount <= 0)
                {
                    ShowBox(ContractRegeditBookInfo.ResetSerialNumber, "当前行政地域下无承包经营权证!");
                    return;
                }

                if (currentZoneBooksCount > 0 && config.MaxNumber - config.MinNumber < currentZoneBooksCount - 1)
                {
                    ShowBox(ContractRegeditBookInfo.ResetSerialNumber, ContractRegeditBookInfo.SetRangError);
                    return;
                }

                TaskExportRegeditBookArgument meta = new TaskExportRegeditBookArgument();
                meta.DbContext = DbContext;
                meta.CurrentZone = currentZone;
                meta.IsStockRight = false;
                TaskResetAllSerialNumberOperation reset = new TaskResetAllSerialNumberOperation();
                reset.Argument = meta;
                reset.Name = "重置流水号";
                reset.Description = currentZone.FullName;
                reset.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
                    Refresh();
                });
                ThePage.TaskCenter.Add(reset);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                reset.StartAsync();
            });
        }

        #endregion 重置流水号

        #region Methods - Private

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
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(string zoneName, string header, eContractRegeditBookType type,
            string taskDes, string taskName, string messageName = "", List<VirtualPerson> listPerson = null)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, header);
            extPage.Workpage = ThePage;
            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractRegeditBookType.ExportWarrant:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractRegeditBookType.ExportWarrentSummaryData:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractRegeditBookType.ExportAwareInventoryData:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractRegeditBookType.ExportSingleFamilyConfirmData:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractRegeditBookType.BatchExportRegeditBookData:
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
        private void ExportCommonOperate(eContractRegeditBookType type, string taskDes, string taskName, string filePath = "",
            string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            IDbContext dbContext = CreateDb();
            TaskContractRegeditBookArgument meta = new TaskContractRegeditBookArgument();
            meta.FileName = filePath;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.ArgType = type;
            meta.SystemSet = this.SystemSet;
            var import = new TaskContractRegeditBookOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.IsBatch = isbatch;
            import.SummaryDefine = this.SummaryDefine;
            import.BookLandNum = BookLandNum;
            import.BookPersonNum = BookPersonNum;
            import.BookNumSetting = BookNumSetting;
            import.SelectedPersons = listPerson;
            import.ExtendUseExcelDefine = ExtendUseExcelDefine;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 公共导出文件操作任务
        /// </summary>
        private void ExportDataCommonOperateTask(string zoneName, string header, eContractRegeditBookType type,
            string taskDes, string taskName, List<VirtualPerson> listPerson = null, DateTime? time = null, DateTime? pubTime = null)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, header);
            extPage.Workpage = ThePage;
            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractRegeditBookType.ExportWarrant:
                        ExportWarrentTask(extPage.FileName, taskDes, taskName, listPerson);
                        break;

                    case eContractRegeditBookType.BatchExportWarrant:
                        ExportWarrentTaskGroup(extPage.FileName, taskDes, taskName);
                        break;
                    //case eContractRegeditBookType.ExportWarrentSummaryData:
                    //    ExportWarrantSummaryTask(extPage.FileName, taskDes, taskName);
                    //    break;
                    case eContractRegeditBookType.BatchExportWarrentSummaryData:
                        ExportWarrantSummaryTaskGroup(extPage.FileName, taskDes, taskName);
                        break;
                    //case eContractRegeditBookType.ExportAwareInventoryData:
                    //    ExportAwareTableTask(extPage.FileName, taskDes, taskName);
                    //    break;
                    case eContractRegeditBookType.BatchExportAwareInventoryData:
                        ExportAwareTableTaskGroup(extPage.FileName, taskDes, taskName);
                        break;

                    case eContractRegeditBookType.ExportSingleFamilyConfirmData:
                        ExportFamilyTableTask(extPage.FileName, taskDes, taskName, listPerson);
                        break;

                    case eContractRegeditBookType.BatchExportSingleFamilyConfirmData:
                        ExportFamilyTableTaskGroup(extPage.FileName, taskDes, taskName);
                        break;

                    case eContractRegeditBookType.BatchExportRegeditBookData:
                        ExportRegeditBookTaskGroup(extPage.FileName, taskDes, taskName);
                        break;

                    case eContractRegeditBookType.ExportRegeditBookData:
                        ExportRegeditBookTask(extPage.FileName, taskDes, taskName, listPerson);
                        break;
                }
            });
        }

        #region 导出权证

        /// <summary>
        /// 导出单个权证-证书数据处理
        /// </summary>
        private void ExportSingleWarrant(string filePath)
        {
            IDbContext dbContext = DbContext;  //CreateDb();
            if (currentRegeditBook == null || currentRegeditBook.Tag == null) return;
            YuLinTu.Library.Entity.ContractRegeditBook currentBook = currentRegeditBook.Tag;
            VirtualPerson vpn = currentItem.Tag;
            ContractConcord currentConcord = ConcordBusiness.Get(currentBook.ID);
            List<ContractLand> contractLands = AccountLandBusiness.GetByConcordId(currentConcord.ID);
            CollectivityTissue Tissue = ConcordBusiness.GetSenderById(currentConcord.SenderId);
            string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractRegeditBookWord);
            List<Dictionary> DictList = DictBusiness.GetAll();
            contractLands.LandNumberFormat(SystemSet);
            ContractWarrantPrinter printContract = new ContractWarrantPrinter();

            try
            {
                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(printContract);
                if (temp != null && temp is ContractWarrantPrinter)
                {
                    printContract = (ContractWarrantPrinter)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                printContract.dbContext = dbContext;
                printContract.CurrentZone = CurrentZone;
                printContract.RegeditBook = currentBook;
                printContract.Concord = currentConcord;
                printContract.LandCollection = contractLands;
                printContract.BatchExport = false;
                printContract.Contractor = vpn;
                printContract.Tissue = Tissue;
                printContract.DictList = DictList;
                printContract.TempleFilePath = tempPath;
                printContract.UseExcel = ExtendUseExcelDefine.WarrantExtendByExcel;
                printContract.BookPersonNum = BookPersonNum;
                printContract.BookLandNum = BookLandNum;
                printContract.BookNumSetting = BookNumSetting;
                printContract.ExportContractLand(filePath);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 导出多个权证-证书数据处理
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportListWarrant(eContractRegeditBookType type, List<VirtualPerson> selectPerson, string taskDes, string taskName, string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null)
        {
            IDbContext dbContext = CreateDb();
            DateTime? date = SetPublicyTableDate();
            if (date == null)
            {
                return;
            }
            TaskContractRegeditBookArgument meta = new TaskContractRegeditBookArgument();
            meta.FileName = filePath;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.ArgType = type;
            meta.SystemSet = SystemSet;
            var import = new TaskContractRegeditBookOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.ListPerson = selectPerson;
            import.IsBatch = false;
            import.BookPersonNum = BookPersonNum;
            import.BookLandNum = BookLandNum;
            import.BookNumSetting = BookNumSetting;
            import.ExtendUseExcelDefine = ExtendUseExcelDefine;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            ThePage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            ThePage.Workspace.Message.Send(this, args);
        }

        #endregion 导出权证

        #endregion Methods - Private

        #region Methods - Events

        #region 按键功能

        /// <summary>
        /// 是否批量
        /// </summary>
        private void cbIsbatch_Click(object sender, RoutedEventArgs e)
        {
            isbatch = (bool)cbIsbatch.IsChecked;
            if (IsBatchEvt != null)
            {
                IsBatchEvt(isbatch);
            }
        }

        /// <summary>
        /// 刷新按钮事件
        /// </summary>
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            txtWhere.Text = string.Empty;
            Items.Clear();
            Refresh();
        }

        /// <summary>
        /// 选择项改变
        /// </summary>
        private void view_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GetSelectItem();
            if (currentItem == null)
            {
                return;
            }
            if (currentRegeditBook == null)
            {
                view.ExpandItemWhenLeftMouseDoubleClick = true;
                return;
            }
            VirtualPerson vp = currentItem.Tag;
            if (vp.FamilyExpand != null)
            {
                if (vp.FamilyExpand.ContractorType != eContractorType.Farmer)
                {
                    //miAddSharePerson.IsEnabled = false;
                }
                else
                {
                    // miAddSharePerson.IsEnabled = true;
                }
            }
            else
            {
                //  miAddSharePerson.IsEnabled = true;
            }
        }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetSelectItem();
            if (currentRegeditBook == null)
            {
                return;
            }
            EditRegeditBook();
        }

        #endregion 按键功能

        #region 右键菜单

        /// <summary>
        /// 添加合同
        /// </summary>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRegeditBook();
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            EditRegeditBook();
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void miDel_Click(object sender, RoutedEventArgs e)
        {
            DelRegeditBook();
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void miClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 权证登记
        /// </summary>
        private void miInitialBook_Click(object sender, RoutedEventArgs e)
        {
            InitalizeWarrent();
        }

        /// <summary>
        /// 预览登记簿
        /// </summary>
        private void miPreviewBook_Click(object sender, RoutedEventArgs e)
        {
            PrivewRegeditBook();
        }

        /// <summary>
        /// 导出登记簿
        /// </summary>
        private void miExportBook_Click(object sender, RoutedEventArgs e)
        {
            ExportRegeditBook();
        }

        #endregion 右键菜单

        #region 辅助功能

        /// <summary>
        /// 创建承包方合同集合，排除了锁定人的
        /// </summary>
        private List<ContractConcord> CreateConcordCollection(List<ContractConcord> concords)
        {
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            List<VirtualPerson> persons = null;
            if (CurrentZone.Level == eZoneLevel.Group)
                persons = personStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
            else
                persons = personStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            if (persons == null || persons.Count == 0)
            {
                ShowBox("承包方", "当前行政区域下没找到承包方，不能添加权证!");
                return null;
            }
            //persons.RemoveAll(c => c.Name.Contains("集体"));昌松说要加入
            List<ContractConcord> useConcords = new List<ContractConcord>();
            persons.ForEach(p => useConcords.AddRange(concords.FindAll(c => c.ContracterId == p.ID)));

            var orderdVps = persons.OrderBy(vp =>
            {
                //排序
                int num = 0;
                Int32.TryParse(vp.FamilyNumber, out num);
                return num;
            });
            foreach (var item in orderdVps)
            {
                selectVps.Add(item);
            }
            return useConcords;
        }

        /// <summary>
        /// 获取当前选择项
        /// </summary>
        private void GetSelectItem()
        {
            currentItem = null;
            //currentConcord = null;
            currentRegeditBook = null;
            var item = view.SelectedItem;
            if (item is BindContractRegeditBook)
            {
                BindContractRegeditBook bp = item as BindContractRegeditBook;
                currentRegeditBook = bp;
                ContractConcord concord = ConcordBusiness.Get(bp.Tag.ID);
                if (concord == null) return;
                currentItem = Items.FirstOrDefault(t => t.ID == concord.ContracterId);
            }
            if (item is ContractRegeditBookItem)
            {
                currentItem = view.SelectedItem as ContractRegeditBookItem;
            }
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null,
            bool isConfirm = true, bool isCancel = true)
        {
            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
                ConfirmButtonVisibility = isConfirm ? Visibility.Visible : Visibility.Collapsed,
                CancelButtonVisibility = isCancel ? Visibility.Visible : Visibility.Collapsed,
            }, action);
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 当前模块中全部承包方
        /// </summary>
        private List<VirtualPerson> CurrentContractorList()
        {
            List<VirtualPerson> vpList = new List<VirtualPerson>();
            foreach (var item in Items)
            {
                vpList.Add(item.Tag);
            }
            return vpList;
        }

        /// <summary>
        /// 右键按下
        /// </summary>
        private void view_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            view.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// 获取地域下所有子级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public List<Zone> GetChildrenZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_CHILDREN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        #endregion 辅助功能

        #endregion Methods - Events

        #endregion Methods

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectItem();
            if (currentItem == null)
            {
                return;
            }
            if (currentRegeditBook == null)
            {
                view.ExpandItemWhenLeftMouseDoubleClick = true;
                return;
            }
            VirtualPerson vp = currentItem.Tag;
            if (vp.FamilyExpand != null)
            {
                if (vp.FamilyExpand.ContractorType != eContractorType.Farmer)
                {
                    //miAddSharePerson.IsEnabled = false;
                }
                else
                {
                    // miAddSharePerson.IsEnabled = true;
                }
            }
            else
            {
                //  miAddSharePerson.IsEnabled = true;
            }
        }

        /// <summary>
        /// 重复流水号检查
        /// </summary>
        private void miRepeatNumber_Click(object sender, RoutedEventArgs e)
        {
            var argument = new TaskWarrantCheckArgument();
            argument.DbContext = DbContext;
            argument.CurrentZone = currentZone;

            var operation = new TaskWarrantCheckOperation();
            operation.Argument = argument;
            operation.Description = "检查流水号是否符合要求,流水号是否重复";
            operation.Name = "流水号检查";
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }
    }
}