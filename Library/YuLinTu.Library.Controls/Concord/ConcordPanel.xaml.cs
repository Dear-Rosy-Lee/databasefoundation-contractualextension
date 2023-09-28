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
using System.Linq.Expressions;
using System.Data;
using DataColumn = YuLinTu.Library.Business.DataColumn;
using YuLinTu.Library.Entity.Model;
using System.Reflection;
using YuLinTu.Library.Repository;
using System.Windows.Input;
using YuLinTu.Library.WorkStation;
using ToolMath = YuLinTu.Library.WorkStation.ToolMath;
using ToolConfiguration = YuLinTu.Library.WorkStation.ToolConfiguration;
using Microsoft.Practices.Unity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 合同管理界面
    /// </summary>
    public partial class ConcordPanel : UserControl
    {
        #region Fields

        //private int index = 0;//序号
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
        private List<Dictionary> wayList;   //承包方式
        private List<Dictionary> purposeList; //土地用途

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

        #endregion Fields

        #region Properties

        /// <summary>
        /// 是否批量
        /// </summary>
        public PanelIsBatch IsBatchEvt { get; set; }

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

        /// <summary>
        /// 承包合同合同明细表设置
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
        public delegate void MenueEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenueEnableControl MenueEnableMethod { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConcordPanel()
        {
            InitializeComponent();
            DataContext = this;
            Summary = new ConcordSummary();
            virtualType = eVirtualType.Land;
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
        }

        #endregion Ctor

        #region Methods

        #region Methods - 初始数据

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
                    MenueEnableMethod(false);
                    DoWork(go);
                },
                completed =>
                {
                    MenueEnableMethod();
                    view.Roots = Items;
                    DataCount();
                    view.IsEnabled = true;
                },
                terminated =>
                {
                    MenueEnableMethod();
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    view.IsEnabled = false;
                    lbzone.Text = "";
                    Items.Clear();
                    view.Roots = null;
                    ThePage.Page.IsBusy = true;
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
            if (PersonBusiness == null || ConcordBusiness == null)
            {
                return;
            }
            zoneName = PersonBusiness.GetUinitName(currentZone);
            arg.Instance.ReportProgress(1, zoneName);
            string zoneCode = arg.Instance.Argument.UserState as string;
            List<VirtualPerson> vps = null;
            List<ContractConcord> concords = null;
            List<Dictionary> dicList = null;
            if (!string.IsNullOrEmpty(zoneCode))
            {
                vps = PersonBusiness.GetByZone(zoneCode);
                concords = ConcordBusiness.GetCollection(zoneCode);
                ModuleMsgArgs mma = MessageExtend.CreateMsg(DbContext, ConcordMessage.CONCORD_GET_DICTIONARY, "");
                TheBns.Current.Message.Send(this, mma);
                dicList = mma.ReturnValue as List<Dictionary>;
            }
            wayList = dicList == null ? new List<Dictionary>() : dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            purposeList = dicList == null ? new List<Dictionary>() : dicList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
            if (vps != null && vps.Count > 0 && concords != null && concords.Count > 0)
            {
                foreach (var item in vps)
                {
                    if (arg.Instance.IsStopPending)
                        break;
                    List<ContractConcord> list = concords.FindAll(t => t.ContracterId == item.ID);
                    if (list == null || list.Count == 0)
                        continue;
                    ConcordItem vpi = ConcordItemHelper.ConvertToItem(item, list, wayList, purposeList);
                    arg.Instance.ReportProgress(50, vpi);
                }
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
            ConcordItem item = arg.UserState as ConcordItem;
            if (item != null && FamilyOtherSet.ShowFamilyInfomation && item.Tag.Name.Equals("集体"))
            {
                return;
            }
            else
            {
                Items.Add(item);
                //DataSummary(item);
            }
        }

        /// <summary>
        /// 加载数据的时候统计数据属性
        /// </summary>
        /// <param name="item"></param>
        private void DataSummary(ConcordItem item)
        {
            double summaryConcordArea = 0.0;
            double summaryActualArea = 0.0;
            double summaryAwareArea = 0.0;
            foreach (var child in item.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                Summary.ConcordCount += (child.Visibility == Visibility.Visible) ? 1 : 0;
                summaryConcordArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.ConcordArea) : 0;
                summaryActualArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.ActualArea) : 0;
                summaryAwareArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.AwareArea) : 0;
            }
            summaryConcordArea += double.Parse(Summary.ConcordAreaCount);
            Summary.ActualAreaCount = WorkStation.ToolMath.SetNumbericFormat(summaryConcordArea.ToString(), 2);//summaryConcordArea.ToString("f2");
            Summary.AwareAreaCount += Math.Round(summaryAwareArea, 4);
            Summary.ConcordAreaCount += Math.Round(summaryConcordArea, 4);
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        private void DataCount()
        {
            Summary.InitialData();
            double summaryConcordArea = 0.0;
            double summaryActualArea = 0.0;
            double summaryAwareArea = 0.0;
            foreach (var item in Items)
            {
                if (!view.IsItemVisible(item))//(item.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                foreach (var child in item.Children)
                {
                    if (!view.IsItemVisible(item))                       //(child.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }
                    Summary.ConcordCount += (child.Visibility == Visibility.Visible) ? 1 : 0;
                    summaryConcordArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.ConcordArea) : 0;
                    summaryActualArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.ActualArea) : 0;
                    summaryAwareArea += (child.Visibility == Visibility.Visible) ? double.Parse(child.AwareArea) : 0;
                }
            }
            summaryConcordArea = Business.ToolMath.SetNumericFormat(summaryConcordArea, 4, 1);
            summaryActualArea = Business.ToolMath.SetNumericFormat(summaryActualArea, 4, 1);
            summaryAwareArea = Business.ToolMath.SetNumericFormat(summaryAwareArea, 4, 1);

            Summary.ActualAreaCount = ToolMath.SetNumbericFormat(summaryActualArea.ToString(), 2);//Math.Round(summaryActualArea, 4);
            Summary.AwareAreaCount = ToolMath.SetNumbericFormat(summaryAwareArea.ToString(), 2);//Math.Round(summaryAwareArea, 4);
            Summary.ConcordAreaCount = ToolMath.SetNumbericFormat(summaryConcordArea.ToString(), 2);//Math.Round(summaryConcordArea, 4);
        }

        #endregion Methods - 初始数据

        #region Methods - 承包合同基本操作

        /// <summary>
        /// 添加合同
        /// </summary>
        public void AddConcord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractConcordInfo.AddConcrod, ContractConcordInfo.AddDataNoZone);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Village)
            {
                ShowBox(ContractConcordInfo.AddConcrod, ContractConcordInfo.AddDataInVillage);
                return;
            }
            List<VirtualPerson> vps = PersonBusiness.GetByZone(currentZone.FullCode);
            List<ContractConcord> concords = ConcordBusiness.GetCollection(currentZone.FullCode);
            if (vps == null || vps.Count == 0)
            {
                ShowBox(ContractConcordInfo.AddConcrod, ContractConcordInfo.VirtualPersonDataNull);
                return;
            }
            List<ContractLand> landList = LandBusiness.GetLandCollection(currentZone.FullCode);
            List<ContractLand> tempLands = new List<ContractLand>();
            if (landList == null || landList.Count == 0)
            {
                ShowBox(ContractConcordInfo.AddConcrod, ContractConcordInfo.LandDataNull);
                return;
            }
            else
            {
                foreach (var land in landList)
                {
                    if (land.AliasNameA != "否")//|| string.IsNullOrEmpty(land.AliasNameA))
                    {
                        tempLands.Add(land);
                    }
                }
            }
            landList = tempLands;
            vps = CheckConcord(vps, landList, concords);
            if (vps.Count == 0)
            {
                ShowBox(ContractConcordInfo.AddConcrod, ContractConcordInfo.VirtualPersoAllHave);
                return;
            }
            ContractConcord concord = new ContractConcord() { ZoneCode = currentZone.FullCode };
            ConcordInfoPage page = new ConcordInfoPage();
            page.CurrentZone = currentZone;
            page.Concord = concord;
            page.Items = Items;
            page.Business = ConcordBusiness;
            page.IsAdd = true;
            page.ContracterList = vps;
            page.LandList = landList;
            page.ConcordSettingDefine = this.ConcordSettingDefine; //多种合同，配置文件管理
            page.dbContext = DbContext;
            page.Workpage = ThePage;
            ThePage.Page.ShowDialog(page, (b, e) =>
            {
                if (!(bool)b)
                    return;
                if (page.FamilyLandList != null && page.FamilyLandList.Count > 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var person = page.CurrentFamily;
                        List<ContractConcord> listConcord = new List<ContractConcord>();
                        listConcord.Add(page.Concord);    //添加合同
                        currentItem = ConcordItemHelper.ConvertToItem(person, listConcord, wayList, purposeList);
                        var item = Items.FirstOrDefault(c => c.Tag.ID == currentItem.Tag.ID);
                        if (item == null)
                        {
                            Items.Add(currentItem);
                        }
                        else
                        {
                            item.Children.Add(currentItem.Children.FirstOrDefault());
                        }
                        currentItem = null;
                        //VirtualPersonExpand vpexpand = null;
                        //if (person.FamilyExpand != null)
                        //    vpexpand = person.FamilyExpand;
                        //else
                        //    vpexpand = new VirtualPersonExpand();
                        //vpexpand.ConcordNumber = concord.ConcordNumber;
                        //vpexpand.ConcordStartTime = concord.ArableLandStartTime;
                        //vpexpand.ConcordEndTime = concord.ArableLandEndTime;
                        //person.FamilyExpand = vpexpand;
                        //PersonBusiness.Update(person);
                    }));
                    ModuleMsgArgs args = MessageExtend.ContractConcordMsg(CreateDb(), ConcordMessage.CONCORD_ADD_COMPLATE, page.Concord, currentZone.FullCode);
                    ThePage.Workspace.Message.Send(this, args);
                    TheBns.Current.Message.Send(this, args);
                    DataCount();
                }
            });
        }

        /// <summary>
        /// 是否全部签订了合同
        /// </summary>
        private List<VirtualPerson> CheckConcord(List<VirtualPerson> vps, List<ContractLand> lands, List<ContractConcord> concordList)
        {
            List<VirtualPerson> vpc = new List<VirtualPerson>();
            List<VirtualPerson> retvpc = new List<VirtualPerson>();
            foreach (VirtualPerson vp in vps)
            {
                if (vp.Status == eVirtualPersonStatus.Lock)
                {
                    //排除锁定的承包方
                    continue;
                }
                vpc.Add(vp);
            }
            foreach (VirtualPerson vp in vpc)
            {
                if (vp.Name.Contains("集体"))
                {
                    //排除集体
                    continue;
                }
                List<ContractLand> landCollection = lands.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == vp.ID);
                landCollection.RemoveAll(c => string.IsNullOrEmpty(c.ConstructMode));
                if (landCollection == null || landCollection.Count == 0)
                {
                    Log.Log.WriteException(vp, "错误", currentZone.FullName + vp.Name + "在库中的地承包方式为空。");
                    continue;
                }
                List<ContractConcord> concords = concordList.FindAll(cd => cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                foreach (ContractLand land in landCollection)
                {
                    if (concords.Find(cd => cd.ArableLandType != ((int)eConstructMode.Family).ToString()) == null && land.ConstructMode != ((int)eConstructMode.Family).ToString())
                    {
                        retvpc.Add(vp);
                        break;
                    }
                    if (land.ConcordId != null && land.ConcordId.HasValue && concords.Find(cd => cd.ID == land.ConcordId.Value) != null)
                    {
                        continue;
                    }
                    else
                    {
                        retvpc.Add(vp);
                    }
                    break;
                }
                landCollection = null;
                concords = null;
            }
            concordList = null;
            return retvpc;
        }

        /// <summary>
        /// 编辑合同
        /// </summary>
        public void EditConcord()
        {
            if (currentZone == null)
            {
                ShowBox(ContractConcordInfo.EditConcord, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            if (currentConcord == null)
            {
                ShowBox(ContractConcordInfo.EditConcord, ContractConcordInfo.EditDataNoSelected);
                return;
            }
            //if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            //{
            //    ShowBox(ContractConcordInfo.EditConcord, ContractConcordInfo.EditConcordLock);
            //    return;
            //}
            List<VirtualPerson> listPerosn = new List<VirtualPerson>();
            listPerosn.Add(currentItem.Tag);
            List<ContractLand> landList = LandBusiness.GetLandCollection(currentZone.FullCode);
            List<ContractConcord> listConcord = ConcordBusiness.GetCollection(currentZone.FullCode);
            ConcordInfoPage editPage = new ConcordInfoPage();
            editPage.Workpage = ThePage;
            editPage.CurrentVp = currentItem.Tag;
            editPage.IsAdd = false;
            editPage.CurrentZone = currentZone;
            editPage.Items = Items;
            editPage.Business = ConcordBusiness;
            editPage.ContracterList = listPerosn;
            editPage.LandList = landList;
            editPage.dbContext = DbContext;
            editPage.ConcordSettingDefine = this.ConcordSettingDefine; //多种合同，配置文件管理
            ContractConcord concord = listConcord.Find(c => c.ID == currentConcord.ID);
            if (concord == null)
            {
                ShowBox(ContractConcordInfo.EditConcord, ContractConcordInfo.EditDataNoData);
                return;
            }
            var concordTemp = concord.Clone() as ContractConcord;
            editPage.Concord = concordTemp;
            ThePage.Page.ShowMessageBox(editPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    List<ContractConcord> concords = new List<ContractConcord>();
                    concords.Add(concordTemp);    //编辑的合同
                    ConcordItem entity = ConcordItemHelper.ConvertToItem(currentItem.Tag, concords, wayList, purposeList);
                    BindConcord concordBind = entity.Children.FirstOrDefault(t => t.ID == currentConcord.ID);
                    currentConcord.CopyPropertiesFrom(concordBind);
                }));
                ModuleMsgArgs args = MessageExtend.ContractConcordMsg(CreateDb(), ConcordMessage.CONCORD_EDIT_COMPLATE, concordTemp, currentZone.FullCode);
                ThePage.Workspace.Message.Send(this, args);
                TheBns.Current.Message.Send(this, args);
                DataCount();
            });
        }

        /// <summary>
        /// 删除合同
        /// </summary>
        public void DelConcord()
        {
            if (currentConcord == null)
            {
                ShowBox(ContractConcordInfo.DelData, ContractConcordInfo.DelDataNo);
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(ContractConcordInfo.DelData, ContractConcordInfo.DelDataLock);
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
                    var delConcord = currentConcord.Clone() as BindConcord;  //克隆一份
                    ConcordBusiness.Delete(currentConcord.Tag);
                    currentItem.Children.Remove(currentConcord);
                    if (currentItem == null)
                        currentItem = Items.FirstOrDefault(c => c.Tag.ID == delConcord.Tag.ContracterId);
                    if (currentItem.Children.Count == 0)
                    {
                        Items.Remove(currentItem);
                    }
                    ModuleMsgArgs args = MessageExtend.ContractConcordMsg(CreateDb(), ConcordMessage.CONCORD_DELETE_COMPLATE, delConcord.Tag, currentZone.FullCode);
                    ThePage.Workspace.Message.Send(this, args);
                    TheBns.Current.Message.Send(this, args);
                    DataCount();
                }
                catch (Exception ex)
                {
                    ShowBox(ContractConcordInfo.DelData, ContractConcordInfo.DelDataFail);
                    YuLinTu.Library.Log.Log.WriteException(this, "DelConcord(删除承包合同)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(ContractConcordInfo.DelData, ContractConcordInfo.DelDataWarring, eMessageGrade.Warn, action);
            //TabMessageBoxDialog message = new TabMessageBoxDialog()
            //{
            //    Header = ContractConcordInfo.DelData,
            //    Message = ContractConcordInfo.DelDataWarring,
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
            //        var delConcord = currentConcord.Clone() as BindConcord;  //克隆一份
            //        ConcordBusiness.Delete(currentConcord.Tag);
            //        currentItem.Children.Remove(currentConcord);
            //        if (currentItem.Children.Count == 0)
            //        {
            //            Items.Remove(currentItem);
            //        }
            //        ModuleMsgArgs args = MessageExtend.ContractConcordMsg(CreateDb(), ConcordMessage.CONCORD_DELETE_COMPLATE, delConcord.Tag, currentZone.FullCode);
            //        ThePage.Workspace.Message.Send(this, args);
            //        TheBns.Current.Message.Send(this, args);
            //        DataCount();
            //    }
            //    catch (Exception ex)
            //    {
            //        ShowBox(ContractConcordInfo.DelData, ContractConcordInfo.DelDataFail);
            //        YuLinTu.Library.Log.Log.WriteException(this, "DelConcord(删除承包合同)", ex.Message + ex.StackTrace);
            //    }
            //});
        }

        #endregion Methods - 承包合同基本操作

        #region Methods - 合同数据处理

        /// <summary>
        /// 签订合同(初始化合同数据)
        /// </summary>
        public void InitialContractConcord()
        {
            if (CurrentZone == null)
            {
                //没有选择地域
                ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.InitialBatchSelectedZoneError);
                return;
            }
            if (currentZone.Level == eZoneLevel.Town && ConcordSettingDefine.ChooseBatch == 1)
            {
                ShowBox(ContractConcordInfo.InitialConcordInfo, "镇级地域时只能进行批量签订合同，请更改选择中签订合同方式");
                return;
            }
            List<Zone> allZones = new List<Zone>();
            var senderStation = DbContext.CreateSenderWorkStation();
            var zoneStation = DbContext.CreateZoneWorkStation();
            allZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            List<ContractConcord> concords = new List<ContractConcord>();
            List<ContractLand> lands = new List<ContractLand>();
            List<CollectivityTissue> senders = new List<CollectivityTissue>();
            //List<Zone> zones = new List<Zone>();
            try
            {
                var cordStation = DbContext.CreateConcordStation();
                var landStation = DbContext.CreateContractLandWorkstation();
                senders = senderStation.GetTissues(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                //zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //单个任务签订合同
                    lands = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                    List<ContractLand> tempLands = new List<ContractLand>();

                    if (lands == null || lands.Count == 0)
                    {
                        ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.LandDataNull);
                        return;
                    }
                    else
                    {
                        foreach (var land in lands)
                        {
                            if (land.AliasNameA != "否")//|| string.IsNullOrEmpty(land.AliasNameA))
                            {
                                tempLands.Add(land);
                            }
                        }
                    }
                    lands = tempLands;
                    concords = cordStation.GetByZoneCode(currentZone.FullCode);
                    ContractConcordInitializePage initialPage = new ContractConcordInitializePage(false, DbContext);
                    initialPage.Workpage = ThePage;
                    initialPage.CurrentZone = CurrentZone;
                    initialPage.PersonBusiness = PersonBusiness;
                    initialPage.ListLand = lands;
                    initialPage.ListConcord = concords;
                    initialPage.AllZones = allZones;
                    initialPage.Senders = null;
                    initialPage.ConcordSettingDefine = this.ConcordSettingDefine; //多种合同，配置文件管理
                    ThePage.Page.ShowMessageBox(initialPage, (t, s) =>
                    {
                        if (!(bool)t)
                        {
                            return;
                        }
                        InitialContractConcordTask(initialPage);
                        concords.Clear();
                        concords = null;
                        lands.Clear();
                        lands = null;
                        //zones.Clear();
                        //zones = null;
                        GC.Collect();
                    });
                }
                else if (CurrentZone.Level == eZoneLevel.Village || CurrentZone.Level == eZoneLevel.Town)
                {
                    //多个任务签订合同
                    lands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                    List<ContractLand> tempLands = new List<ContractLand>();
                    if (lands == null || lands.Count == 0)
                    {
                        ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.LandDataNull);
                        return;
                    }
                    else
                    {
                        foreach (var land in lands)
                        {
                            if (land.AliasNameA != "否")//|| string.IsNullOrEmpty(land.AliasNameA))
                            {
                                tempLands.Add(land);
                            }
                        }
                    }
                    concords = cordStation.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
                    ContractConcordInitializePage initialPage = new ContractConcordInitializePage(true, DbContext);
                    initialPage.Workpage = ThePage;
                    initialPage.cbTissue.IsEnabled = false;//如果是镇 村发包方为各自发包方
                    initialPage.CurrentZone = CurrentZone;
                    initialPage.PersonBusiness = PersonBusiness;
                    initialPage.ListLand = tempLands;
                    initialPage.ListConcord = concords;
                    initialPage.AllZones = allZones;
                    initialPage.Senders = senders;
                    initialPage.ConcordSettingDefine = this.ConcordSettingDefine; //多种合同，配置文件管理
                    ThePage.Page.ShowMessageBox(initialPage, (t, s) =>
                    {
                        if (!(bool)t)
                        {
                            return;
                        }
                        TaskGroupInitialContractConcord(initialPage);
                        concords.Clear();
                        concords = null;
                        lands.Clear();
                        lands = null;
                        //zones.Clear();
                        //zones = null;
                        GC.Collect();
                    });
                }
                else
                {
                    ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.InitialBatchSelectedZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "InitialContractConcord(签订合同失败!)", ex.Message + ex.StackTrace);
                ShowBox(ContractConcordInfo.InitialConcordInfo, ContractConcordInfo.InitialConcordInfoFail);
            }
        }

        /// <summary>
        /// 初始化合同数据任务(单个任务)
        /// </summary>
        /// <param name="initialPage">初始化界面</param>
        private void InitialContractConcordTask(ContractConcordInitializePage initialPage)
        {
            TaskConcordInitializeArgument argument = new TaskConcordInitializeArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.ConcordsModified = initialPage.ConcordsModified;
            argument.LandsOfInitialConcord = initialPage.LandsOfInitialConcord;
            argument.IsCalculateArea = initialPage.IsCalculateArea;
            argument.VillageInlitialSet = SystemSet.VillageInlitialSet;
            argument.Sender = initialPage.Sender;
            TaskConcordInitializeOperation operation = new TaskConcordInitializeOperation();
            operation.Argument = argument;
            operation.Description = "签订" + CurrentZone.FullName + "合同";  //任务描述
            operation.Name = ContractConcordInfo.ConcordDataHandle;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();

                ThePage.Message.Send(this,
                    MessageExtend.ContractConcordMsg(DbContext, ConcordMessage.CONCORD_INITIALIZE_COMPLATE, initialPage.ConcordsModified, CurrentZone.FullCode));
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量初始化合同数据任务(组任务)
        /// </summary>
        /// <param name="initialPage">初始化界面</param>
        private void TaskGroupInitialContractConcord(ContractConcordInitializePage initialPage)
        {
            TaskGroupConcordInitializeArgument argument = new TaskGroupConcordInitializeArgument();
            argument.Database = DbContext;
            argument.CurrentZone = CurrentZone;
            argument.ConcordsModified = initialPage.ConcordsModified;
            argument.LandsOfInitialConcord = initialPage.LandsOfInitialConcord;
            argument.IsCalculateArea = initialPage.IsCalculateArea;
            argument.AllZones = initialPage.AllZones;
            argument.VillageInlitialSet = SystemSet.VillageInlitialSet;
            argument.Sender = initialPage.Sender;
            TaskGroupConcordInitializeOperation operation = new TaskGroupConcordInitializeOperation();
            operation.Argument = argument;
            operation.Description = "签订" + CurrentZone.FullName + "合同";  //任务描述
            operation.Name = ContractConcordInfo.ConcordDataHandle;         //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();

                ThePage.Message.Send(this,
                    MessageExtend.ContractConcordMsg(DbContext, ConcordMessage.CONCORD_INITIALIZE_COMPLATE, initialPage.ConcordsModified, CurrentZone.FullCode));
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 预览合同
        /// </summary>
        public void PrintViewConcord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractConcordInfo.PreviewConcord, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            if (currentConcord == null)
            {
                ShowBox(ContractConcordInfo.PreviewConcord, ContractConcordInfo.PreviewConcordNoSelected);
                return;
            }
            ConcordBusiness.AreaType = ConcordSettingDefine.ChooseArea;
            ConcordBusiness.DictList = DictList;
            ConcordBusiness.SystemSet = SystemSet;
            ConcordBusiness.PreviewConcordData(CurrentZone, currentConcord.Tag, currentItem.Tag);
        }

        /// <summary>
        /// 导出合同
        /// </summary>
        public void ExportConcord()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            var zoneStation = DbContext.CreateZoneWorkStation();
            List<Zone> SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> childrenZone = SelfAndSubsZones.FindAll(c => c.FullCode != currentZone.FullCode);
            var allZones = zoneStation.GetAllZones(currentZone);
            if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
            {
                ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, ContractConcordInfo.ExportConcord);
                ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                {
                    if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                    {
                        return;
                    }
                    //批量导出(选择地域大于组级并且当前地域下有子级地域)
                    TaskGroupExportConcordDataArgument meta = new TaskGroupExportConcordDataArgument();
                    meta.FileName = savePage.FileName;
                    meta.Database = DbContext;
                    meta.CurrentZone = currentZone;
                    meta.PublishDateSetting = this.PublishDateSetting;
                    meta.SummaryDefine = this.SummaryDefine;
                    meta.SelfAndSubsZones = SelfAndSubsZones;
                    meta.AllZones = allZones;
                    meta.SystemSet = SystemSet;
                    meta.AreaType = ConcordSettingDefine.ChooseArea;
                    TaskGroupExportConcordDataOperation taskConcord = new TaskGroupExportConcordDataOperation();
                    taskConcord.Argument = meta;
                    taskConcord.Description = "导出" + currentZone.FullName + "合同";
                    taskConcord.Name = ContractConcordInfo.ExportConcord;

                    ThePage.TaskCenter.Add(taskConcord);
                    if (ShowTaskViewer != null)
                    {
                        ShowTaskViewer();
                    }
                    taskConcord.StartAsync();
                });
            }
            else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
            {
                //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                if (Items == null || Items.Count == 0)
                {
                    ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.CurrentZoneNoPersons);
                    return;
                }
                List<ContractConcord> concords = ConcordBusiness.GetCollection(CurrentZone.FullCode);
                if (concords == null || concords.Count == 0)
                {
                    ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.CurrentZoneNoConcords);
                    return;
                }
                if (!isbatch)
                {
                    if (currentItem == null && currentConcord == null)
                    {
                        ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordNoSelected);
                        return;
                    }
                    if (currentItem != null && currentConcord == null)
                    {
                        //选中合同项(直接导出当前合同)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, ContractConcordInfo.ExportConcord);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            List<VirtualPerson> listPerson = new List<VirtualPerson>();
                            //选中承包方没选中合同项
                            listPerson.Add(currentItem.Tag);
                            TaskExportConcordDataArgument meta = new TaskExportConcordDataArgument();
                            meta.FileName = savePage.FileName;
                            meta.ArgType = eContractConcordArgType.ExportConcordData;
                            meta.Database = DbContext;
                            meta.CurrentZone = currentZone;
                            meta.PublishDateSetting = this.PublishDateSetting;
                            meta.SummaryDefine = this.SummaryDefine;
                            meta.ListPerson = listPerson;
                            meta.SystemSet = SystemSet;
                            meta.AreaType = ConcordSettingDefine.ChooseArea;
                            meta.SelfAndSubsZones = SelfAndSubsZones;
                            meta.AllZones = allZones;
                            TaskExportConcordDataOperation taskConcord = new TaskExportConcordDataOperation();
                            taskConcord.Argument = meta;
                            taskConcord.Name = "导出" + currentZone.Name + "合同";
                            taskConcord.Description = "导出" + currentZone.FullName + "的合同";
                            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
                            {
                            });
                            ThePage.TaskCenter.Add(taskConcord);
                            if (ShowTaskViewer != null)
                            {
                                ShowTaskViewer();
                            }
                            taskConcord.StartAsync();
                        });
                    }
                    else if (currentItem != null && currentConcord != null)
                    {
                        //选中合同项(直接导出当前合同)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, ContractConcordInfo.ExportConcord);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            ConcordBusiness.SystemSet = SystemSet;
                            List<ContractLand> lands;
                            try
                            {
                                lands = ConcordBusiness.GetLandsByConcordId(currentConcord.Tag);
                            }
                            catch (Exception)
                            {
                                lands = new List<ContractLand>(1);
                            }
                            bool isSuccess = ConcordBusiness.ExportConcordData(currentZone, currentItem.Tag, lands, currentConcord.Tag, savePage.FileName, ConcordSettingDefine.ChooseArea);
                            if (isSuccess)
                                ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordComplete, eMessageGrade.Infomation);
                        });
                    }
                }
                else
                {
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //界面上没有选中项(此时弹出承包方选择界面)
                    foreach (var item in Items)
                    {
                        listPerson.Add(item.Tag);
                    }
                    ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                    selectPage.Workpage = ThePage;
                    selectPage.PersonItems = listPerson;
                    selectPage.Business = PersonBusiness;
                    ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                        if (selectedPersons == null || selectedPersons.Count == 0)
                        {
                            ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordNoSelected);
                            return;
                        }
                        //选中合同项(直接导出当前合同)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, ContractConcordInfo.ExportConcord);
                        ThePage.Page.ShowMessageBox(savePage, (bb, rr) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || bb == false)
                            {
                                return;
                            }
                            TaskExportConcordDataArgument meta = new TaskExportConcordDataArgument();
                            meta.FileName = savePage.FileName;
                            meta.ArgType = eContractConcordArgType.ExportConcordData;
                            meta.Database = DbContext;
                            meta.CurrentZone = currentZone;
                            meta.PublishDateSetting = this.PublishDateSetting;
                            meta.SummaryDefine = this.SummaryDefine;
                            meta.ListPerson = selectedPersons;
                            meta.SystemSet = SystemSet;
                            meta.SelfAndSubsZones = SelfAndSubsZones;
                            meta.AllZones = allZones;
                            meta.AreaType = ConcordSettingDefine.ChooseArea;
                            TaskExportConcordDataOperation taskConcord = new TaskExportConcordDataOperation();
                            taskConcord.Argument = meta;
                            taskConcord.Name = "导出" + currentZone.Name + "合同";
                            taskConcord.Description = "导出" + currentZone.FullName + "合同";
                            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
                            {
                            });
                            ThePage.TaskCenter.Add(taskConcord);
                            if (ShowTaskViewer != null)
                            {
                                ShowTaskViewer();
                            }
                            taskConcord.StartAsync();
                        });
                    });
                }
                childrenZone = null;
                concords = null;
                GC.Collect();
            }
            else
            {
                ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordSelectZoneError);
                return;
            }
        }

        #endregion Methods - 合同数据处理

        public void OnUpdate(object sender, ExecutedRoutedEventArgs e)
        {
            OnUpdate(e.Parameter);
        }

        private void OnUpdate(object args)
        {
            BatchUpdateConcord updateModel;
            try
            {
                updateModel = OnGetBatchUpdateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            var pgd = new PropertyGridDialog
            {
                Header = $"批量编辑",
                Object = updateModel,
                IsGroupingEnabled = true
            };
            pgd.PropertyGrid.Properties["Workpage"] = ThePage;
            pgd.PropertyGrid.Properties["CurrentZone"] = CurrentZone;
            pgd.Confirm += (s, e) =>
            {
                BatchUpdateConcord updateInput = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    updateInput = pgd.Object as BatchUpdateConcord;
                }));
                if (updateInput == null)
                {
                    return;
                }

                OnBatchUpdateCore(updateInput);

                OnBatchUpdated(updateInput);
            };
            pgd.ShowDialog(ThePage.Page);
        }

        protected virtual int OnBatchUpdateCore(BatchUpdateConcord updateInput)
        {
            ContainerFactory factory = new ContainerFactory(DbContext);
            var concordRep = factory.CreateRepository<IContractConcordRepository>();
            Expression<Func<ContractConcord, bool>> predicate = ResolveBatchPredicate();
            var kvs = new KeyValueList<string, object>();
            var entityColDic = typeof(ContractConcord).GetDataColumns().ToDictionary(k => k.PropertyName, v => v.ColumnName);
            updateInput.TraversalPropertiesInfo((PropertyInfo pi, object val) =>
            {
                if (val == null)
                    return true;

                if (!entityColDic.ContainsKey(pi.Name))
                    return true;

                kvs.Add(pi.Name, val);

                return true;
            });
          
            var updateCount = concordRep.UpdateRange(predicate, kvs);
            concordRep.SaveChanges();
            return updateCount;
        }

        protected virtual void OnBatchUpdated(BatchUpdateConcord value)
        {
            Refresh();
        }

        protected virtual BatchUpdateConcord OnGetBatchUpdateModel()
        {
            return new BatchUpdateConcord();
        }

        protected virtual Expression<Func<ContractConcord, bool>> ResolveBatchPredicate()
        {
            Expression<Func<ContractConcord, bool>> predicate = null;
            predicate = x => x.ZoneCode.StartsWith(CurrentZone.FullCode);
            return predicate;
        }

        #region Methods - 数据过滤

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
                    this.Dispatcher.Invoke(new Action(() => { SetItemVisible(go); }));
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

                var dataItem = obj as ConcordItem;
                if (dataItem != null)
                {
                    txt = dataItem.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }
                var concord = obj as BindConcord;
                if (concord != null)
                {
                    txt = concord.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = concord.SenderName;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = concord.ConcordNumber;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = concord.ConcordArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = concord.ActualArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = concord.AwareArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = concord.ActualArea;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = concord.ContractWay;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = concord.LandPurpose;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = concord.ContractTime;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                    txt = concord.Comment;
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
        private bool JudgeItem(string whString, bool allInfo, ConcordItem dataItem)
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

        #endregion Methods - 数据过滤

        #region Methods - 合同清空、刷新

        /// <summary>
        /// 清空当前地域合同
        /// </summary>
        public void Clear()
        {
            if (currentZone == null)
            {
                ShowBox(ContractConcordInfo.Clear, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            if ((currentZone.Level == eZoneLevel.Group && Items != null && Items.Count(c => c.Tag.Status == eVirtualPersonStatus.Right) == 0))
            {
                ShowBox(ContractConcordInfo.Clear, ContractConcordInfo.ClearNoValidData);
                return;
            }

            var message = new YuLinTu.Windows.Wpf.Metro.Components.MessageDialog()
            {
                Header = ContractConcordInfo.Clear,
                Message = ContractConcordInfo.ClearConfirm,
                MessageGrade = eMessageGrade.Warn,
            };
            message.Confirm += (s, a) =>
            {
                var concordStation = this.DbContext.CreateConcordStation();
                concordStation.DeleteRelationDataByZoneCode(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            };
            message.ConfirmStart += (s, a) =>
            {
            };
            message.ConfirmCompleted += (s, a) =>
            {
                var removeData = Items.Where(c => c.Tag.Status == eVirtualPersonStatus.Right).TryToList();
                List<Guid> concordId = new List<Guid>();
                foreach (var data in removeData)
                {
                    Items.Remove(data);
                    foreach (var child in data.Children)
                    {
                        concordId.Add(child.Tag.ID);
                    }
                }
                DataCount();
                var args = MessageExtend.CreateMsg(CreateDb(), ConcordMessage.CONCORD_CLEAR_COMPLATE, concordId, CurrentZone.FullCode);
                SendMessasge(args);
            };
            message.ConfirmTerminated += (s, a) =>
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除未被锁定的合同失败!)", a.Exception.ToString());
                ShowBox(ContractConcordInfo.Clear, ContractConcordInfo.ContractConcordClearFail);
            };

            ThePage.Page.ShowMessageBox(message, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
            });
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

        #endregion Methods - 合同清空、刷新

        #region 家庭承包方式申请书

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public void PrintRequireBook()
        {
            if (currentZone == null)
            {
                ShowBox("预览单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            GetSelectItem();
            if (currentItem == null)
            {
                ShowBox("预览单户申请书", "请选择预览申请书的承包方或合同!");
                return;
            }
            if (currentConcord != null && currentConcord.Tag.ArableLandType != ((int)eConstructMode.Family).ToString())
            {
                ShowBox("预览单户申请书", "请选择家庭承包的合同!");
                return;
            }
            if (ConcordSettingDefine.SingleRequireDate)
            {
                ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                dateSetting.WorkPage = ThePage;
                ThePage.Page.ShowMessageBox(dateSetting, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    PublishDateSetting = dateSetting.DateTimeSetting;
                    ConcordBusiness.PublishDateSetting = PublishDateSetting;
                    string message;
                    ConcordBusiness.SystemSet = SystemSet;
                    bool flag = ConcordBusiness.PrintRequireBookWord(currentZone, currentItem.Tag, out message); //打印预览单户申请表
                    if (flag == false)
                    {
                        ShowBox("预览申请书", message);
                    }
                });
            }
            else
            {
                PublishDateSetting = new DateSetting();
                PublishDateSetting.PublishStartDate = null;
                PublishDateSetting.PublishEndDate = null;
                ConcordBusiness.PublishDateSetting = PublishDateSetting;
                string message = null;
                ConcordBusiness.SystemSet = SystemSet;
                bool flag = ConcordBusiness.PrintRequireBookWord(currentZone, currentItem.Tag, out message); //打印预览单户申请表
                if (flag == false)
                {
                    ShowBox("预览申请书", message);
                }
            }
        }

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public void ExportApplicationByFamily()
        {
            if (CurrentZone == null)
            {
                ShowBox("导出单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox("导出单户申请书", "批量导出时请选择镇级(包括镇级)以下的地域!");
                return;
            }
            List<ContractConcord> tempConcords = ConcordBusiness.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (tempConcords == null || tempConcords.Count == 0)
            {
                ShowBox("导出单户申请书", "当前地域没有合同数据!");
                return;
            }
            if (!isbatch)
            {
                if (currentItem == null)
                {
                    ShowBox("导出单户申请书", "请选择一条数据进行导出");
                    return;
                }
                else
                {
                    if (ConcordSettingDefine.SingleRequireDate)
                    {
                        ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                        dateSetting.WorkPage = ThePage;
                        ThePage.Page.ShowMessageBox(dateSetting, (s, t) =>
                        {
                            if (!(bool)s)
                            {
                                return;
                            }
                            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                            extPage.Workpage = ThePage;
                            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                                {
                                    return;
                                }
                                string fileDir = extPage.FileName;
                                PublishDateSetting = dateSetting.DateTimeSetting;
                                List<VirtualPerson> listPerson = new List<VirtualPerson>();
                                listPerson.Add(currentItem.Tag);
                                //ExportCommonOperate(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, listPerson); //导出单户申请表
                                TaskRequireBookByFamilyOperation(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, listPerson); //导出单户申请表
                            });
                        });
                    }
                    else
                    {
                        ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                        extPage.Workpage = ThePage;
                        ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                            {
                                return;
                            }
                            string fileDir = extPage.FileName;
                            PublishDateSetting = new DateSetting();
                            PublishDateSetting.PublishStartDate = null;
                            PublishDateSetting.PublishEndDate = null;
                            List<VirtualPerson> listPerson = new List<VirtualPerson>();
                            listPerson.Add(currentItem.Tag);
                            // ExportCommonOperate(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, listPerson); //导出单户申请表
                            TaskRequireBookByFamilyOperation(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, listPerson); //导出单户申请表
                        });
                    }
                }
            }
            else if (CurrentZone.Level == eZoneLevel.Group)
            //if (CurrentZone.Level == eZoneLevel.Group && currentItem == null)//选择组，但是弹出承包方选择框
            {
                ContractAccountPersonLockPage caplp = new ContractAccountPersonLockPage();
                List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                foreach (var item in Items)
                {
                    getpersonst.Add(item.Tag);
                }
                if (getpersonst == null) return;
                caplp.PersonItems = getpersonst;
                caplp.Business = PersonBusiness;
                ThePage.Page.ShowMessageBox(caplp, (bb, e) =>
                {
                    if (!(bool)bb)
                    {
                        return;
                    }
                    if (caplp.selectVirtualPersons == null) return;
                    if (ConcordSettingDefine.SingleRequireDate)
                    {
                        ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                        dateSetting.WorkPage = ThePage;
                        ThePage.Page.ShowMessageBox(dateSetting, (s, t) =>
                        {
                            if (!(bool)s)
                            {
                                return;
                            }
                            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                            extPage.Workpage = ThePage;
                            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                                {
                                    return;
                                }
                                string fileDir = extPage.FileName;
                                PublishDateSetting = dateSetting.DateTimeSetting;
                                ExportCommonOperate(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, caplp.selectVirtualPersons); //导出单户申请表
                            });
                        });
                    }
                    else
                    {
                        ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                        extPage.Workpage = ThePage;
                        ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                            {
                                return;
                            }
                            string fileDir = extPage.FileName;
                            PublishDateSetting = new DateSetting();
                            PublishDateSetting.PublishStartDate = null;
                            PublishDateSetting.PublishEndDate = null;
                            ExportCommonOperate(eContractConcordArgType.SingleExportRequireBook, "导出单户申请书", "单户申请书导出", fileDir, null, null, null, caplp.selectVirtualPersons); //导出单户申请表
                        });
                    }
                });
            }
            else if ((CurrentZone.Level <= eZoneLevel.Town && CurrentZone.Level >= eZoneLevel.Village))
            {
                //批量导出
                if (ConcordSettingDefine.SingleRequireDate)
                {
                    ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                    dateSetting.WorkPage = ThePage;
                    ThePage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        PublishDateSetting = dateSetting.DateTimeSetting;
                        ExportCommonOperate(CurrentZone.FullName, "导出单户申请书", eContractConcordArgType.BatchSingleExportRequireBook, "导出单户申请书", "单户申请书导出"); //导出单户申请表
                    });
                }
                else
                {
                    PublishDateSetting = new DateSetting();
                    PublishDateSetting.PublishStartDate = null;
                    PublishDateSetting.PublishEndDate = null;
                    ExportCommonOperate(CurrentZone.FullName, "导出单户申请书", eContractConcordArgType.BatchSingleExportRequireBook, "导出单户申请书", "单户申请书导出"); //导出单户申请表
                }
            }
        }

        /// <summary>
        /// 预览集体申请书
        /// </summary>
        public void PrintViewApplication()
        {
            if (CurrentZone == null)
            {
                ShowBox("预览集体申请书", "请选择申请书所在行政区域!");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox("预览集体申请书", "批量初始化时请选择镇级(包括镇级)以下的地域!");
                return;
            }
            if ((CurrentZone.Level == eZoneLevel.Group) || (CurrentZone.Level == eZoneLevel.Village))
            {
                List<CollectivityTissue> tissues = ConcordBusiness.GetTissuesByConcord(CurrentZone);
                if (tissues == null || tissues.Count == 0)
                {
                    ShowBox("预览集体申请书", "当前行政区域下没有发包方数据可供操作!");
                    return;
                }
                if (tissues.Count > 1)
                {
                    ShowBox("预览集体申请书", "不允许签订多个发包方,请检查数据");
                    return;
                }
                //foreach (CollectivityTissue tissue in tissues)
                //{
                List<ContractRequireTable> tabs = ConcordBusiness.GetTissueRequireTable(tissues[0].Code);
                bool first = tabs == null || tabs.Count == 0;
                if (tabs != null && tabs.Count > 0)
                {
                    first = !tabs.Any(tb => tb.ZoneCode == CurrentZone.FullCode);
                }
                if (first)
                {
                    ConcordApplicationTableSetting applicationBookSetting = new ConcordApplicationTableSetting();
                    applicationBookSetting.Tissue = tissues[0];
                    applicationBookSetting.ContractConcordBusiess = ConcordBusiness;
                    applicationBookSetting.Workpage = ThePage;
                    applicationBookSetting.PrintView = true;
                    applicationBookSetting.CurrentZone = CurrentZone;
                    ThePage.Page.ShowMessageBox(applicationBookSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                    });
                }
                else
                {
                    ConcordBusiness.SystemSet = SystemSet;
                    ConcordBusiness.PrintApplicationOld(tissues[0], CurrentZone, true);
                }
                //}
            }
        }

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        public void ExportApplicationBook()
        {
            if (CurrentZone == null)
            {
                ShowBox("预览单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox("预览单户申请书", "批量初始化时请选择镇级(包括镇级)以下的地域!");
                return;
            }
            List<ContractConcord> tempConcords = ConcordBusiness.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (tempConcords == null || tempConcords.Count == 0)
            {
                ShowBox("导出集体申请书", "当前地域没有合同数据!");
                return;
            }
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            int allChildrenZonesCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);  //当前地域下的

            if ((CurrentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village) && allChildrenZonesCount >= 1)
            {
                ExportCommonOperate(CurrentZone.FullName, "导出集体申请书", eContractConcordArgType.BatchExportApplicationBook, "导出集体申请书", "导出集体申请书");
            }
            if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZonesCount == 0))     //在组下并且选择了某个承包方数据
            {
                List<CollectivityTissue> tissues = ConcordBusiness.GetTissuesByConcord(CurrentZone);
                if (tissues == null || tissues.Count == 0)
                {
                    ShowBox("预览集体申请书", "当前行政区域下没有数据可供操作!");
                    return;
                }
                ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出集体申请书");
                extPage.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                    {
                        return;
                    }
                    bool flag = false;
                    string fileDir = extPage.FileName;
                    foreach (CollectivityTissue tissue in tissues)
                    {
                        List<ContractRequireTable> tabs = ConcordBusiness.GetTissueRequireTable(tissue.Code);
                        bool first = tabs == null || tabs.Count == 0;
                        if (tabs != null && tabs.Count > 0)
                        {
                            first = !tabs.Any(tb => tb.ZoneCode == CurrentZone.FullCode);
                        }
                        if (first)
                        {
                            ConcordApplicationTableSetting applicationBookSetting = new ConcordApplicationTableSetting();
                            applicationBookSetting.Tissue = tissue;
                            applicationBookSetting.ContractConcordBusiess = ConcordBusiness;
                            applicationBookSetting.Workpage = ThePage;
                            applicationBookSetting.PrintView = false;
                            applicationBookSetting.CurrentZone = CurrentZone;
                            applicationBookSetting.FileDir = fileDir;
                            ThePage.Page.ShowMessageBox(applicationBookSetting, (s, t) =>
                            {
                                if (!(bool)s)
                                {
                                    return;
                                }
                                flag = applicationBookSetting.Flag;
                                if (flag)
                                {
                                    ShowBox("导出集体申请书", "集体申请书导出成功!", eMessageGrade.Infomation);
                                }
                            });
                        }
                        else
                        {
                            ConcordBusiness.SystemSet = SystemSet;
                            flag = ConcordBusiness.PrintApplicationOld(tissue, CurrentZone, false, fileName: fileDir);
                        }
                    }
                    if (flag)
                    {
                        ShowBox("导出集体申请书", "集体申请书导出成功!", eMessageGrade.Infomation);
                    }
                });
            }
        }

        #endregion 家庭承包方式申请书

        #region 其他承包方式申请书

        /// <summary>
        /// 其他承包方式申请书预览
        /// </summary>
        public void PrintViewOtherApplication()
        {
            if (currentZone == null)
            {
                ShowBox("预览其他承包方式申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            GetSelectItem();
            if (currentItem == null)
            {
                ShowBox("预览其他承包方式申请书", "请选择预览申请书的承包方!");
                return;
            }
            if (currentConcord != null && currentConcord.Tag.ArableLandType == ((int)eConstructMode.Family).ToString())
            {
                ShowBox("预览单户申请书", "请选择非家庭承包的合同!");
                return;
            }
            if (ConcordSettingDefine.SingleRequireDate)
            {
                ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                dateSetting.WorkPage = ThePage;
                ThePage.Page.ShowMessageBox(dateSetting, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    PublishDateSetting = dateSetting.DateTimeSetting;
                    ConcordBusiness.PublishDateSetting = PublishDateSetting;
                    string message = null;
                    ConcordBusiness.SystemSet = SystemSet;
                    bool flag = ConcordBusiness.PrintViewOtherApplicationBook(currentZone, currentItem.Tag, out message); //打印预览单户申请表
                    if (flag == false)
                    {
                        ShowBox("预览申请书", message);
                    }
                });
            }
            else
            {
                PublishDateSetting = new DateSetting();
                PublishDateSetting.PublishStartDate = null;
                PublishDateSetting.PublishEndDate = null;
                ConcordBusiness.PublishDateSetting = PublishDateSetting;
                string message = null;
                bool flag = ConcordBusiness.PrintViewOtherApplicationBook(currentZone, currentItem.Tag, out message); //打印预览单户申请表
                if (flag == false)
                {
                    ShowBox("预览申请书", message);
                }
            }
        }

        /// <summary>
        /// 其他承包方式申请书导出
        /// </summary>
        public void ExportApplicationByOther()
        {
            if (CurrentZone == null)
            {
                ShowBox("预览单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox("预览单户申请书", "批量初始化时请选择镇级(包括镇级)以下的地域!");
                return;
            }
            List<ContractConcord> tempConcords = ConcordBusiness.GetContractsByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (tempConcords == null || tempConcords.Count == 0)
            {
                ShowBox("导出集体申请书", "当前地域没有合同数据!");
                return;
            }
            if ((CurrentZone.Level <= eZoneLevel.Town && CurrentZone.Level >= eZoneLevel.Village))
            {
                //批量导出
                if (ConcordSettingDefine.SingleRequireDate)
                {
                    ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                    dateSetting.WorkPage = ThePage;
                    ThePage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        PublishDateSetting = dateSetting.DateTimeSetting;
                        ExportCommonOperate(CurrentZone.FullName, "导出单户申请书", eContractConcordArgType.BatchExportApplicationByOther, "导出单户申请书", "单户申请书导出"); //导出单户申请表
                    });
                }
                else
                {
                    PublishDateSetting = new DateSetting();
                    PublishDateSetting.PublishStartDate = null;
                    PublishDateSetting.PublishEndDate = null;
                    ExportCommonOperate(CurrentZone.FullName, "导出单户申请书", eContractConcordArgType.BatchExportApplicationByOther, "导出单户申请书", "单户申请书导出"); //导出单户申请表
                }
            }
            if (CurrentZone.Level == eZoneLevel.Group)
            {
                if (!isbatch)
                {
                    if (currentItem == null)
                    {
                        ShowBox("导出集体申请书", "请选择一条数据进行导出!");
                        return;
                    }
                    if (currentConcord != null && currentConcord.Tag.ArableLandType == ((int)eConstructMode.Family).ToString())
                    {
                        ShowBox("预览单户申请书", "请选择非家庭承包的合同!");
                        return;
                    }

                    if (ConcordSettingDefine.SingleRequireDate)
                    {
                        ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                        dateSetting.WorkPage = ThePage;
                        ThePage.Page.ShowMessageBox(dateSetting, (s, t) =>
                        {
                            if (!(bool)s)
                            {
                                return;
                            }
                            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                            extPage.Workpage = ThePage;
                            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                                {
                                    return;
                                }
                                string fileDir = extPage.FileName;
                                PublishDateSetting = dateSetting.DateTimeSetting;
                                ConcordBusiness.PublishDateSetting = PublishDateSetting;
                                string message = null;
                                bool flag = ConcordBusiness.ExportApplicationByOtherBookWord(CurrentZone, fileDir, currentItem.Tag, out message);
                                if (flag == false)
                                {
                                    ShowBox("导出申请书", message);
                                }
                                else
                                {
                                    ShowBox("导出申请书", message, eMessageGrade.Infomation);
                                }
                            });
                        });
                    }
                    else
                    {
                        ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, "导出单户申请书");
                        extPage.Workpage = ThePage;
                        ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                            {
                                return;
                            }
                            string fileDir = extPage.FileName;
                            PublishDateSetting = new DateSetting();
                            PublishDateSetting.PublishStartDate = null;
                            PublishDateSetting.PublishEndDate = null;
                            ConcordBusiness.PublishDateSetting = PublishDateSetting;
                            string message = null;
                            bool flag = ConcordBusiness.ExportApplicationByOtherBookWord(CurrentZone, fileDir, currentItem.Tag, out message);
                            if (flag == false)
                            {
                                ShowBox("导出申请书", message);
                            }
                            else
                            {
                                ShowBox("导出申请书", message, eMessageGrade.Infomation);
                            }
                        });
                    }
                }
                else
                {
                    ContractAccountPersonLockPage caplp = new ContractAccountPersonLockPage();
                    List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                    foreach (var item in Items)
                    {
                        getpersonst.Add(item.Tag);
                    }
                    if (getpersonst == null) return;
                    caplp.PersonItems = getpersonst;
                    caplp.Business = PersonBusiness;
                    ThePage.Page.ShowMessageBox(caplp, (bb, e) =>
                    {
                        if (!(bool)bb)
                        {
                            return;
                        }
                        if (caplp.selectVirtualPersons == null) return;
                        //导出单个
                        if (ConcordSettingDefine.SingleRequireDate)
                        {
                            ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                            dateSetting.WorkPage = ThePage;
                            ThePage.Page.ShowMessageBox(dateSetting, (b, r) =>
                            {
                                if (!(bool)b)
                                {
                                    return;
                                }
                                PublishDateSetting = dateSetting.DateTimeSetting;

                                ExportCommonOperate(CurrentZone.FullName, "导出单户申请表", eContractConcordArgType.ExportApplicationByOther, "导出单户申请表", "单户申请表导出", null, caplp.selectVirtualPersons);  //导出单户申请表
                            });
                        }
                        else
                        {
                            PublishDateSetting = new DateSetting();
                            PublishDateSetting.PublishStartDate = null;
                            PublishDateSetting.PublishEndDate = null;

                            ExportCommonOperate(CurrentZone.FullName, "导出单户申请表", eContractConcordArgType.ExportApplicationByOther, "导出单户申请表", "单户申请表导出", null, caplp.selectVirtualPersons);  //导出单户申请表
                        }
                    });
                }
            }
        }

        #endregion 其他承包方式申请书

        #region 合同明细表

        /// <summary>
        /// 导出合同明细表
        /// </summary>
        public void ConcordInformationTable()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractConcordInfo.ExportConcordInformationTable, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }

            List<Zone> childrenZone = ConcordBusiness.GetChildrenZone(currentZone); //获取当前地域下的子级地域
            List<ContractConcord> concords = ConcordBusiness.GetCollection(currentZone.FullCode);
            if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
            {
                //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                if (concords.Count == 0)
                {
                    ShowBox(ContractConcordInfo.ExportConcordInformationTable, ContractConcordInfo.CurrentZoneNoConcords);
                    return;
                }
                TaskExportConcordInformationTable(eContractConcordArgType.ExportConcordInformationTable, ContractConcordInfo.ExportConcordInfoDesc, ContractConcordInfo.ExportConcordTableName, SystemSet.DefaultPath, "");

                //ExportCommonOperate(CurrentZone.FullName, ContractConcordInfo.ExportConcordInformationTable, eContractConcordArgType.ExportConcordInformationTable,
                //    ContractConcordInfo.ExportConcordInfoDesc, ContractConcordInfo.ExportConcordTableName);
            }
            else if (currentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇
                ShowBox(ContractConcordInfo.ExportConcordInformationTable, ContractConcordInfo.ExportConcordSelectZoneError);
                return;
            }
            else if (currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town)
            {
                ExportCommonOperate(CurrentZone.FullName, ContractConcordInfo.ExportConcordInformationTable, eContractConcordArgType.BatchExportConcordInformationTable,
                     ContractConcordInfo.ExportConcordInfoDesc, ContractConcordInfo.ExportConcordTableName);
            }
            concords = null;
            childrenZone = null;
            GC.Collect();
        }

        #endregion 合同明细表

        #region Methods - Private

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框标题</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        /// <param name="listPerson">选中的承包方(仅适用于导出承包合同表)</param>
        private void ExportCommonOperate(string zoneName, string header, eContractConcordArgType type, string taskDes, string taskName,
            string messageName = "", List<VirtualPerson> listPerson = null)
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
                    case eContractConcordArgType.SingleExportRequireBook:
                        TaskRequireBookByFamilyOperation(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractConcordArgType.BatchSingleExportRequireBook:
                        TaskGroupRequireBookByFamilyOperation(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractConcordArgType.ExportApplicationByOther:
                        TaskRequireBookByOtherOperation(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case eContractConcordArgType.BatchExportApplicationByOther:
                        TaskGroupRequireBookByOtherOperation(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case eContractConcordArgType.BatchExportApplicationBook://批量导出集体申请书
                        TaskGroupExportApplicationBook(type, taskDes, taskName, extPage.FileName);
                        break;

                    case eContractConcordArgType.ExportConcordData:
                        //导出合同
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;
                    //case eContractConcordArgType.ExportConcordInformationTable:
                    //    //导出合同明细表
                    //    TaskExportConcordInformationTable(type, taskDes, taskName, extPage.FileName, messageName);
                    //    break;
                    case eContractConcordArgType.BatchExportConcordInformationTable:
                        //导出合同明细表
                        TaskGroupExportConcordInformationTable(eContractConcordArgType.ExportConcordInformationTable, taskDes, taskName, extPage.FileName, messageName);
                        break;
                }
            });
        }

        /// <summary>
        /// 单进度导出家庭承包单户申请书
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskRequireBookByFamilyOperation(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskRequireBookByFamilyArgument meta = new TaskRequireBookByFamilyArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SystemSet = SystemSet;
            meta.SelectContractor = listPerson;
            meta.PublishDateSetting = PublishDateSetting;
            TaskRequireBookByFamilyOperation taskConcord = new TaskRequireBookByFamilyOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "家庭承包单户申请书";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 多进度导出家庭承包单户申请书
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupRequireBookByFamilyOperation(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupRequireBookByFamilyArgument meta = new TaskGroupRequireBookByFamilyArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.PublishDateSetting = PublishDateSetting;
            meta.SystemSet = SystemSet;
            TaskGroupRequireBookByFamilyOperation taskConcord = new TaskGroupRequireBookByFamilyOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "家庭承包单户申请书";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 单进度导出非家庭承包单户申请书
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskRequireBookByOtherOperation(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskRequireBookByOtherArgument meta = new TaskRequireBookByOtherArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SystemSet = SystemSet;
            meta.PublishDateSetting = PublishDateSetting;
            TaskRequireBookByOtherOperation taskConcord = new TaskRequireBookByOtherOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "非家庭承包单户申请书";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 多进度导出非家庭承包单户申请书
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupRequireBookByOtherOperation(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupRequireBookByOtherArgument meta = new TaskGroupRequireBookByOtherArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.PublishDateSetting = PublishDateSetting;
            meta.SystemSet = SystemSet;
            TaskGroupRequireBookByOtherOperation taskConcord = new TaskGroupRequireBookByOtherOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "非家庭承包单户申请书";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 单进度导出合同明细表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskExportConcordInformationTable(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskConcordInformationTableArgument meta = new TaskConcordInformationTableArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SystemSet = SystemSet;
            meta.SummaryDefine = SummaryDefine;
            meta.IsShow = true;

            TaskConcordInformationTableOperation taskConcord = new TaskConcordInformationTableOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "合同明细表";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 多进度导出合同明细表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportConcordInformationTable(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupConcordInformationTableArgument meta = new TaskGroupConcordInformationTableArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            meta.SummaryDefine = SummaryDefine;
            meta.SystemSet = SystemSet;
            TaskGroupConcordInformationTableOperation taskConcord = new TaskGroupConcordInformationTableOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "合同明细表";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 多进度导出集体申请书
        /// </summary>
        /// <param name="type"></param>
        /// <param name="taskDes"></param>
        /// <param name="taskName"></param>
        /// <param name="filePath"></param>
        /// <param name="listPerson"></param>
        /// <param name="TableType"></param>
        private void TaskGroupExportApplicationBook(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DbContext.CreateZoneWorkStation();
            SelfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            TaskGroupExportApplicationBookArgument meta = new TaskGroupExportApplicationBookArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            meta.SelfAndSubsZones = SelfAndSubsZones;
            meta.AllZones = allZones;
            TaskGroupExportApplicationBookOperation taskConcord = new TaskGroupExportApplicationBookOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "集体申请书";
            taskConcord.Name = taskName;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(eContractConcordArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            TaskContractConcordArgument meta = new TaskContractConcordArgument();
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = DbContext;
            meta.CurrentZone = currentZone;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            TaskContractConcordOperation taskConcord = new TaskContractConcordOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = taskDes;
            taskConcord.Name = taskName;
            taskConcord.Contractor = currentItem != null ? currentItem.Tag : null;
            taskConcord.PublishDateSetting = this.PublishDateSetting;
            taskConcord.ListPerson = listPerson;
            taskConcord.SummaryDefine = this.SummaryDefine;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
            });
            ThePage.TaskCenter.Add(taskConcord);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 更新更新数据状态
        /// </summary>
        private List<VirtualPerson> UpdateInitialize()
        {
            List<VirtualPerson> list = new List<VirtualPerson>();
            //foreach (var item in Items)
            //{
            //    item.FamilyNumber = item.Tag.FamilyNumber;
            //    item.Name = VirtualPersonItemHelper.CreateItemName(item.Tag.Name, item.Children.Count, item.FamilyNumber, item.Status);
            //    item.HouseHolderName = item.Tag.Name;
            //    list.Add(item.Tag);
            //}
            GC.Collect();
            return list;
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
        }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetSelectItem();
            if (currentItem == null)
            {
                return;
            }
            if (currentConcord == null)
            {
                view.ExpandItemWhenLeftMouseDoubleClick = true;
                return;
            }
            EditConcord();
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        private void view_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
            var item = e.Object as VirtualPersonItem;
            if (item == null)
            {
                return;
            }
            e.HasItems = item.Children.Count > 0;
        }

        #endregion 按键功能

        #region 右键菜单

        /// <summary>
        /// 添加合同
        /// </summary>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            AddConcord();
        }

        /// <summary>
        /// 编辑合同
        /// </summary>
        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            EditConcord();
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void miDel_Click(object sender, RoutedEventArgs e)
        {
            DelConcord();
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void miClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 签订合同
        /// </summary>
        private void miInitialConcord_Click(object sender, RoutedEventArgs e)
        {
            InitialContractConcord();
        }

        /// <summary>
        /// 预览合同
        /// </summary>
        private void miPreviewConcord_Click(object sender, RoutedEventArgs e)
        {
            PrintViewConcord();
        }

        /// <summary>
        /// 导出合同
        /// </summary>
        private void miExportConcord_Click(object sender, RoutedEventArgs e)
        {
            ExportConcord();
        }

        #endregion 右键菜单

        #region 辅助功能

        /// <summary>
        /// 设置导出表日期
        /// </summary>
        /// <returns></returns>
        private DateTime? SetTableDatetime(int type)
        {
            bool setData = false;
            string value = string.Empty;
            switch (type)
            {
                case 1:
                    value = WorkStation.ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetHouselderStatementDate);
                    break;

                case 2:
                    value = ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetProxyStatementDate);
                    break;

                case 3:
                    value = ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetPulicliyStatementDate);
                    break;

                case 4:
                    value = ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetMeasureRequireDate);
                    break;
            }
            Boolean.TryParse(value, out setData);
            DateTime? time = DateTime.Now;
            //if (FamilyOtherSet.FamilyInstructionDate)
            //{
            //    DateSettingPage page = new DateSettingPage();
            //    ThePage.Page.ShowMessageBox(page, (b, e) =>
            //    {
            //        if (!(bool)b)
            //            return;
            //        time = page.SetTime;
            //    });
            //}
            //if (setData)
            //{
            //    DateSettingPage page = new DateSettingPage();
            //    ThePage.Page.ShowMessageBox(page, (b, e) =>
            //    {
            //        if (!(bool)b)
            //            return;
            //        time = page.SetTime;
            //    });
            //}
            return time;
        }

        /// <summary>
        /// 获取当前选择项
        /// </summary>
        public BindConcord GetSelectItem()
        {
            currentItem = null;
            currentConcord = null;
            var item = view.SelectedItem;
            if (item is BindConcord)
            {
                BindConcord bp = item as BindConcord;
                currentConcord = bp;
                currentItem = Items.FirstOrDefault(t => t.ID == bp.Tag.ContracterId);
            }
            if (item is ConcordItem)
            {
                currentItem = view.SelectedItem as ConcordItem;
            }
            return currentConcord;
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

        #endregion 辅助功能

        #endregion Methods - Events

        #endregion Methods

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectItem();
        }
    }
}