/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方管理界面
    /// </summary>
    public partial class VirtualPersonPanel : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private int index = 0;//序号
        private bool isBatch;//是否批量
        private bool isShowBatch;//是否显示批量按钮
        private string zoneName;
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueFilter;//过滤数据
        private TaskQueue queueClear;//清空数据
        private Zone currentZone;
        private bool allCheck = false;
        private VirtualPersonItem currentItem;//当前选择项
        private Person currentPerson;//当前选择人
        private eVirtualType virtualType;
        private List<object> list = new List<object>();
        private bool showTableColuml;
        private SearchNumber sNumber;

        /// <summary>
        /// 承包方绑定集合
        /// </summary>
        private ObservableCollection<VirtualPersonItem> Items = new ObservableCollection<VirtualPersonItem>();

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

        #region Popertys

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
        public VirtualPersonSummary Summary
        {
            get { return _Summary; }
            set
            {
                _Summary = value;
                NotifyPropertyChanged("Summary");
            }
        }

        private VirtualPersonSummary _Summary;

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 数据业务
        /// </summary>
        public VirtualPersonBusiness Business { get; set; }

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
                Business.VirtualType = value;
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
        /// 承包方
        /// </summary>
        public VirtualPerson virtualPerson
        {
            get { return currentItem == null ? null : currentItem.Tag; }
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
        /// 显示股权列
        /// </summary>
        public bool ShowEqualColum
        {
            set
            {
                if (!value)
                {
                    view.Columns.Remove(gcEqnum);
                    view.Columns.Remove(gcEqArea);
                }
            }
        }

        /// <summary>
        /// 显示关系列
        /// </summary>
        public bool ShowRelationColum
        {
            set
            {
                if (!value)
                {
                    view.Columns.Remove(gcRelation);
                }
            }
        }

        /// <summary>
        /// 显示二轮台账列
        /// </summary>
        public bool ShowTableColum
        {
            get
            {
                return showTableColuml;
            }
            set
            {
                showTableColuml = value;
                if (!value)
                {
                    view.Columns.Remove(gcPersonCount);
                    view.Columns.Remove(gcTotalTableArea);
                }
                else
                {
                    gcAge.Width = 52;
                    gcGender.Width = 52;
                    miLock.Visibility = Visibility.Collapsed;
                    miInitall.Visibility = Visibility.Collapsed;
                    //miSplit.Visibility = Visibility.Collapsed;
                    //miCombine.Visibility = Visibility.Collapsed;
                    miNumber.Visibility = Visibility.Collapsed;
                    miClear.Visibility = Visibility.Collapsed;
                    miClear.Visibility = Visibility.Collapsed;
                    setMenu.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 承包方导入设置
        /// </summary>
        public FamilyImportDefine FamilyImportSet
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<FamilyImportDefine>();
                var section = profile.GetSection<FamilyImportDefine>();
                var config = (section.Settings as FamilyImportDefine);
                return config;
            }
        }

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

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemSet
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                var config = section.Settings as SystemSetDefine;
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
        public MenuEnableControl MenuEnable { get; set; }

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

        /// <summary>
        /// 是否显示批处理按钮
        /// </summary>
        public bool IsShowBatch
        {
            get
            {
                return isShowBatch;
            }

            set
            {
                isShowBatch = value;
                NotifyPropertyChanged("IsShowBatch");
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Popertys

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public VirtualPersonPanel()
        {
            Summary = new VirtualPersonSummary();
            InitializeComponent();
            DataContext = this;
            virtualType = eVirtualType.Land;

            view.Roots = Items;
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
            queueClear = new TaskQueueDispatcher(Dispatcher);
            sNumber = new SearchNumber();
            if (DbContext == null)
                DbContext = CreateDb();
        }

        #endregion Ctor

        #region Methods

        #region Methods - Public

        #region 获取/绑定数据

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
                    MenuEnable(false);
                    DoWork(go);
                },
                completed =>
                {
                    MenuEnable();
                    view.Filter(obj => { return true; }, true);
                    view.IsEnabled = true;
                    //view.Roots = Items;
                },
                terminated =>
                {
                    MenuEnable();
                    ShowBox("提示", "请检查数据库是否为最新的数据库，否则请升级数据库!");
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    lbzone.Text = "";
                    Items.Clear();
                    //view.Roots = null;
                    view.IsEnabled = false;
                    ThePage.Page.IsBusy = true;
                }, ended =>
                {
                    ThePage.Page.IsBusy = false;
                }, null, null, zoneCode);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void DoWork(TaskGoEventArgs arg)
        {
            if (Business == null)
                return;
            zoneName = Business.GetUinitName(currentZone);
            arg.Instance.ReportProgress(1, zoneName);
            string zoneCode = arg.Instance.Argument.UserState as string;
            List<VirtualPerson> vps = new List<VirtualPerson>();
            if (!string.IsNullOrEmpty(zoneCode))
                vps = Business.GetByZone(zoneCode);
            if (vps != null && vps.Count > 0)
            {
                foreach (var item in vps)
                {
                    if (arg.Instance.IsStopPending)
                        return;
                    VirtualPersonItem vpi = VirtualPersonItemHelper.ConvertToItem(item, null, ShowTableColum);
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
            VirtualPersonItem item = arg.UserState as VirtualPersonItem;
            if (FamilyOtherSet.ShowFamilyInfomation)
            {
                if (item.Tag.Name.Equals("集体") || item.Tag.FamilyExpand.ContractorType != eContractorType.Farmer)
                    return;
            }
            Items.Add(item);
            DataSummary(item);
        }

        /// <summary>
        /// 加载数据的时候统计数据属性
        /// </summary>
        /// <param name="item"></param>
        private void DataSummary(VirtualPersonItem item)
        {
            Summary.FamilyCount += item.Visibility == Visibility.Visible ? 1 : 0;
            foreach (var child in item.Children)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                Summary.PersonCount += child.Visibility == Visibility.Visible ? 1 : 0;
                switch (child.Gender)
                {
                    case eGender.Male:
                        Summary.MaleCount += child.Visibility == Visibility.Visible ? 1 : 0;
                        break;

                    case eGender.Female:
                        Summary.FeMaleCount += child.Visibility == Visibility.Visible ? 1 : 0;
                        break;

                    case eGender.Unknow:
                        Summary.UnknowGenderCount += child.Visibility == Visibility.Visible ? 1 : 0;
                        break;
                }
            }
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        private void DataCount()
        {
            Summary.InitialData();
            List<BindPerson> pcollection = new List<BindPerson>();
            foreach (var item in Items)
            {
                //if (item.Visibility == Visibility.Collapsed)
                //{
                //    continue;
                //}
                if (!view.IsItemVisible(item))
                    continue;
                Summary.FamilyCount += item.Visibility == Visibility.Visible ? 1 : 0;
                foreach (var child in item.Children)
                {
                    if (child.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }
                    Summary.PersonCount += child.Visibility == Visibility.Visible ? 1 : 0;
                    switch (child.Gender)
                    {
                        case eGender.Male:
                            Summary.MaleCount += child.Visibility == Visibility.Visible ? 1 : 0;
                            break;

                        case eGender.Female:
                            Summary.FeMaleCount += child.Visibility == Visibility.Visible ? 1 : 0;
                            break;

                        case eGender.Unknow:
                            Summary.UnknowGenderCount += child.Visibility == Visibility.Visible ? 1 : 0;
                            break;
                    }
                }
            }
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

        #endregion 获取/绑定数据

        #region 添加数据

        /// <summary>
        /// 添加承包方
        /// </summary>
        public void AddVirtualPerson()
        {
            if (CurrentZone == null)
            {
                ShowBox("承包方处理", "请选择添加承包方所在行政区域！");
                return;
            }
            if (CurrentZone.Level > eZoneLevel.Village)
            {
                ShowBox("承包方处理", "请在村、组级地域下添加承包方！");
                return;
            }
            VirtualPerson vp = VirtualPersonItemHelper.CreateVirtualPerson(CurrentZone, eContractorType.Farmer);
            if (Business.VirtualType == eVirtualType.SecondTable)
            {
                SecondPersonInfoPage personPage = new SecondPersonInfoPage(vp, currentZone, Business, true);
                personPage.Workpage = ThePage;
                personPage.Items = Items;
                personPage.Header = "添加承包方";
                ThePage.Page.ShowMessageBox(personPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!personPage.Result)
                    {
                        ShowBox(VirtualPersonInfo.AddVirtualPerson, VirtualPersonInfo.AddVPFail);
                        return;
                    }
                    VirtualPerson contractor = personPage.Contractor;
                    var args = MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_ADD_COMPLETE, contractor);
                    SendMessasge(args);
                    UpdateItems(contractor);
                    DataCount();
                });
            }
            else
            {
                List<VirtualPerson> vpList = CurrentContractorList();
                VirtualPersonInfoPage personPage = new VirtualPersonInfoPage(vp, currentZone, Business, true);
                personPage.Workpage = ThePage;
                personPage.Items = vpList;
                personPage.Header = "添加承包方";
                personPage.OtherDefine = FamilyOtherSet;
                ThePage.Page.ShowMessageBox(personPage, (b, r) =>
                {
                    vpList = null;
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!personPage.Result)
                    {
                        ShowBox(VirtualPersonInfo.AddVirtualPerson, VirtualPersonInfo.AddVPFail);
                        return;
                    }
                    VirtualPerson contractor = personPage.Contractor;
                    var args = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_ADD_COMPLATE, contractor);
                    SendMessasge(args);
                    UpdateItems(contractor);
                    DataCount();
                });
            }
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        public void AddSharePerson()
        {
            if (currentItem == null)
            {
                ShowBox(VirtualPersonInfo.AddSharePerson, VirtualPersonInfo.AddNoVirtual);
                return;
            }
            VirtualPerson vp = currentItem.Tag;
            if (vp.FamilyExpand != null)
            {
                if (vp.FamilyExpand.ContractorType != eContractorType.Farmer)
                {
                    ShowBox(VirtualPersonInfo.AddSharePerson, "该承包方类型非农户，不允许添加共有人");
                    return;
                }
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(VirtualPersonInfo.AddSharePerson, VirtualPersonInfo.AddSharePersonLock);
                return;
            }
            List<VirtualPerson> vpList = CurrentContractorList();
            Person p = VirtualPersonItemHelper.CreatePerson(currentItem.Tag);
            PersonInfoPage personPage = new PersonInfoPage(currentItem.Tag, true);
            personPage.Business = Business;
            personPage.Person = p;
            personPage.PersonItems = vpList;
            personPage.Workpage = ThePage;
            personPage.OtherDefine = FamilyOtherSet;
            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                vpList = null;
                if (!(bool)b)
                {
                    return;
                }
                if (!personPage.Result)
                {
                    ShowBox(VirtualPersonInfo.AddSharePerson, VirtualPersonInfo.AddPersonFail);
                    return;
                }
                BindPerson bdPerson = new BindPerson(personPage.Person);
                var nowSelectItem = currentItem;
                currentItem.Children.Add(bdPerson);
                DataCount();
                if (nowSelectItem != null)
                {
                    nowSelectItem.Name = VirtualPersonItemHelper.CreateItemName(nowSelectItem.Tag.Name, nowSelectItem.Children.Count, nowSelectItem.FamilyNumber, nowSelectItem.Status);
                    MultiObjectArg arg = new MultiObjectArg() { ParameterA = nowSelectItem.Tag, ParameterB = personPage.Person };
                    TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.SHAREPERSON_ADD_COMPLATE, arg));
                }
            });
        }

        #endregion 添加数据

        #region 更新数据

        /// <summary>
        /// 更新数据
        /// </summary>
        public void EditPerson()
        {
            if (currentItem == null && currentPerson == null)
            {
                ShowBox(VirtualPersonInfo.EditData, VirtualPersonInfo.EditDataNo);
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
                {
                    if (!(bool)b)
                        return;
                    StartEdit(true);
                });
                ShowBox(VirtualPersonInfo.EditData, VirtualPersonInfo.EditPersonLock, eMessageGrade.Warn, action, isCancelVisibility: false);
            }
            else
            {
                StartEdit(false);
            }
        }

        /// <summary>
        /// 开始编辑
        /// </summary>
        /// <param name="isLock">是否锁定</param>
        private void StartEdit(bool isLock)
        {
            if (currentItem != null)
            {
                if (currentPerson == null)
                {
                    UpdateVirtualPerson(isLock);
                }
                else
                {
                    UpdateSharePerson(isLock);
                }
            }
        }

        /// <summary>
        /// 更新承包方
        /// </summary>
        public void UpdateVirtualPerson(bool isLock)
        {
            VirtualPerson vp = currentItem.Tag;
            if (Business.VirtualType == eVirtualType.SecondTable)
            {
                SecondPersonInfoPage personPage = new SecondPersonInfoPage(vp, currentZone, Business);
                personPage.Items = Items;
                personPage.Workpage = ThePage;
                personPage.Header = "编辑承包方";
                ThePage.Page.ShowMessageBox(personPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!personPage.Result)
                    {
                        ShowBox(VirtualPersonInfo.AddVirtualPerson, VirtualPersonInfo.AddVPFail);
                        return;
                    }
                    VirtualPerson contractor = personPage.Contractor;
                    UpdateItems(contractor, false);
                    DataCount();
                    var args = MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_EDIT_COMPLATE, contractor);
                    SendMessasge(args);
                });
            }
            else
            {
                List<VirtualPerson> vpList = CurrentContractorList();
                VirtualPersonInfoPage personPage = new VirtualPersonInfoPage(vp, currentZone, Business, isLock: isLock);
                personPage.Items = vpList;
                personPage.Workpage = ThePage;
                personPage.Header = "编辑承包方";
                personPage.OtherDefine = FamilyOtherSet;
                ThePage.Page.ShowMessageBox(personPage, (b, r) =>
                {
                    vpList = null;
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!personPage.Result)
                    {
                        ShowBox(VirtualPersonInfo.AddVirtualPerson, VirtualPersonInfo.AddVPFail);
                        return;
                    }
                    VirtualPerson contractor = personPage.Contractor;
                    UpdateItems(contractor, false);
                    DataCount();
                    var args = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_EDIT_COMPLATE, contractor);
                    SendMessasge(args);
                });
            }
        }

        /// <summary>
        /// 编辑共有人
        /// </summary>
        public void UpdateSharePerson(bool isLock)
        {
            List<VirtualPerson> vpList = CurrentContractorList();
            PersonInfoPage personPage = new PersonInfoPage(currentItem.Tag, false);
            personPage.Business = Business;
            personPage.Person = currentPerson;
            personPage.PersonItems = vpList;
            personPage.Workpage = ThePage;
            personPage.OtherDefine = FamilyOtherSet;
            personPage.IsLock = isLock;
            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                vpList = null;
                if (!(bool)b)
                {
                    return;
                }
                if (!personPage.Result)
                {
                    ShowBox(VirtualPersonInfo.EditData, VirtualPersonInfo.EditDataFail);
                    return;
                }
                UpdateVpItem(personPage.Person);
                DataCount();
                MultiObjectArg arg = new MultiObjectArg() { ParameterA = currentItem.Tag, ParameterB = personPage.Person };
                TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.SHAREPERSON_EDIT_COMPLATE, arg));
                UpdateItems(personPage.Virtualperson, false);
            });
        }

        #endregion 更新数据

        #region 删除数据

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DelPerson()
        {
            if (currentItem == null && currentPerson == null)
            {
                ShowBox(VirtualPersonInfo.DelData, VirtualPersonInfo.DelDataNo);
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(VirtualPersonInfo.DelData, VirtualPersonInfo.DelPersonLock);
                return;
            }
            if (currentItem != null)
            {
                if (currentPerson == null)
                {
                    DelVirtualPerson();
                }
                else if (currentPerson != null && currentItem.ID == currentPerson.ID)
                {
                    ShowBox(VirtualPersonInfo.DelData, VirtualPersonInfo.DelPersonForbidden);
                }
                else
                {
                    DelSharePerson();
                }
            }
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        public void DelVirtualPerson()
        {
            if (currentItem == null || CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelDataNo);
                return;
            }

            //TabMessageBoxDialog message = new TabMessageBoxDialog()
            //{
            //    Header = VirtualPersonInfo.DelVirtualPerson,
            //    Message = VirtualPersonInfo.DelVPersonWarring,
            //    MessageGrade = eMessageGrade.Warn
            //};
            if (Business.VirtualType == eVirtualType.SecondTable)
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    try
                    {
                        Business.Delete(currentItem.Tag.ID);
                        ModuleMsgArgs args = MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_DELT_COMPLETE, currentItem.Tag);
                        SendMessasge(args);
                        Items.Remove(currentItem);
                        DataCount();
                    }
                    catch (Exception ex)
                    {
                        ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonFail);
                        YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
                    }
                });
                ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonWarring, eMessageGrade.Warn, action);
            }
            else
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    try
                    {
                        if (currentItem != null)
                        {
                            // DbContext = CreateDb();
                            var landStation = DbContext.CreateContractLandWorkstation();
                            landStation.DeleteSelectVirtualPersonAllData(currentItem.Tag.ID);
                            ModuleMsgArgs args = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE, currentItem.Tag);
                            SendMessasge(args);
                            Items.Remove(currentItem);
                            DataCount();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonFail);
                        YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
                    }
                });
                ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonWarring, eMessageGrade.Warn, action);
            }
        }

        /// <summary>
        /// 删除共有人
        /// </summary>
        public void DelSharePerson()
        {
            if (currentPerson != null && currentPerson.Name == currentItem.Tag.Name)
            {
                ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonForbidden);
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
                    List<Person> list = currentItem.Tag.SharePersonList;
                    list.RemoveAll(t => t.ID == currentPerson.ID);
                    BindPerson bp = currentItem.Children.Where(t => t.ID == currentPerson.ID).FirstOrDefault();
                    if (currentItem != null)
                    {
                        currentItem.Name = VirtualPersonItemHelper.CreateItemName(currentItem.Tag.Name, list.Count, currentItem.FamilyNumber, currentItem.Status);
                        currentItem.Tag.SharePersonList = list;
                        Business.Update(currentItem.Tag);
                    }

                    if (bp != null)
                    {
                        currentItem.Children.Remove(bp);
                        DataCount();
                    }
                    var args = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.SHAREPERSON_DEL_COMPLATE, currentPerson);
                    SendMessasge(args);
                }
                catch (Exception ex)
                {
                    ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonFail);
                    YuLinTu.Library.Log.Log.WriteException(this, "DelSharePerson(删除共有人)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonWarring, eMessageGrade.Warn, action);

            //TabMessageBoxDialog message = new TabMessageBoxDialog()
            //{
            //    Header = VirtualPersonInfo.DelSharePerson,
            //    Message = VirtualPersonInfo.DelPersonWarring,
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
            //        List<Person> list = currentItem.Tag.SharePersonList;
            //        list.RemoveAll(t => t.ID == currentPerson.ID);
            //        BindPerson bp = currentItem.Children.Where(t => t.ID == currentPerson.ID).FirstOrDefault();
            //        if (bp != null)
            //        {
            //            currentItem.Children.Remove(bp);
            //        }
            //        currentItem.Name = VirtualPersonItemHelper.CreateItemName(currentItem.Tag.Name, currentItem.Children.Count, currentItem.FamilyNumber, currentItem.Status);
            //        currentItem.Tag.SharePersonList = list;
            //        Business.Update(currentItem.Tag);
            //        DataCount();
            //        TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.SHAREPERSON_DEL_COMPLATE, currentPerson));
            //    }
            //    catch (Exception ex)
            //    {
            //        ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonFail);
            //        YuLinTu.Library.Log.Log.WriteException(this, "DelSharePerson(删除共有人)", ex.Message + ex.StackTrace);
            //    }
            //});
        }

        #endregion 删除数据

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

        //private string lastWHString = null;

        public event PropertyChangedEventHandler PropertyChanged;

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
                }, null, null, null, null, whName);
        }

        /// <summary>
        /// 设置数据可见性
        /// </summary>
        private void SetItemVisible(TaskGoEventArgs arg)
        {
            //FilterItems.Clear();
            string whString = arg.Instance.Argument.UserState.ToString();
            view.Filter(obj =>
            {
                if (whString.IsNullOrBlank())
                    return true;

                bool has = false;
                string txt = null;

                var dataItem = obj as VirtualPersonItem;
                if (dataItem != null)
                {
                    txt = dataItem.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = dataItem.ICN;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = dataItem.Comment;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = dataItem.ContractorNumber;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = dataItem.TotalTableArea == null ? null : dataItem.TotalTableArea.Value.ToString();
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;
                }
                var landbind = obj as BindPerson;
                if (landbind != null)
                {
                    txt = landbind.Name;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.Comment;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.Relationship;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.ICN;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = landbind.Age;
                    has = (
                        !txt.IsNullOrBlank() && allCheck && txt.Equals(whString)) || (
                        !txt.IsNullOrBlank() && !allCheck && txt.Contains(whString));
                    if (has)
                        return true;

                    txt = EnumNameAttribute.GetDescription(landbind.Gender);
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
        private bool JudgeItem(string whString, bool allInfo, VirtualPersonItem dataItem)
        {
            bool contains = dataItem.Name.Contains(whString) ||
              (dataItem.ICN != null && dataItem.ICN.Contains(whString)) ||
               (dataItem.ICN != null && dataItem.ICN.Contains(whString)) ||
                (!string.IsNullOrEmpty(dataItem.Comment) && dataItem.Comment.Contains(whString)) ||
                (dataItem.TotalTableArea != null && dataItem.TotalTableArea.ToString().Equals(whString)) ||
                (!string.IsNullOrEmpty(dataItem.ContractorNumber) && dataItem.ContractorNumber.Contains(whString));
            bool equals = dataItem.Name.Equals(whString) ||
                (dataItem.ICN != null && dataItem.ICN.Equals(whString)) ||
                (!string.IsNullOrEmpty(dataItem.Comment) && dataItem.Comment.Equals(whString)) ||
                (dataItem.TotalTableArea != null && dataItem.TotalTableArea.ToString().Equals(whString)) ||
                (!string.IsNullOrEmpty(dataItem.ContractorNumber) && dataItem.ContractorNumber.Equals(whString));
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

        #endregion 数据过滤

        #region 数据处理

        /// <summary>
        /// 分户
        /// </summary>
        public void VirtualPersonSplit()
        {
            if (currentItem == null || (currentItem != null && currentPerson != null || CurrentZone == null))
            {
                ShowBox(VirtualPersonInfo.SplitFamily, VirtualPersonInfo.SplitNoPerson);
                return;
            }
            if (currentItem.Tag.FamilyExpand.ContractorType != eContractorType.Farmer)
            {
                ShowBox(VirtualPersonInfo.SplitFamily, VirtualPersonInfo.SplitMainPerson);
                return;
            }
            if (currentItem.Children.Count == 1)
            {
                ShowBox(VirtualPersonInfo.SplitFamily, VirtualPersonInfo.SplitPersonNull);
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(VirtualPersonInfo.SplitFamily, VirtualPersonInfo.SplitPersonLock);
                return;
            }
            try
            {
                var dbContext = DataBaseSourceWork.GetDataBaseSource();
                var landStaion = dbContext.CreateAgriculturalLandWorkstation();
                var sourceLandList = landStaion.GetCollection(currentItem.Tag.ID);
                PersonSplitPage page = new PersonSplitPage();
                page.Workpage = ThePage;
                page.Business = Business;
                page.CurrentItem = currentItem;
                page.SourceLandList = sourceLandList;
                ThePage.Page.ShowMessageBox(page, (b, e) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!page.Result)
                    {
                        ShowBox(VirtualPersonInfo.SplitFamily, VirtualPersonInfo.SplitFail);
                        return;
                    }
                    Items.Add(page.NewItem);
                    //desItem.Name = VirtualPersonItemHelper.CreateItemName(desItem.Tag.Name, desItem.Children.Count, desItem.FamilyNumber, desItem.Status);
                    //int countSplit = currentItem.Children.Count - page.NewItem.Children.Count;
                    int countSplit = currentItem.Tag.SharePersonList.Count;
                    currentItem.Name = VirtualPersonItemHelper.CreateItemName(currentItem.Tag.Name, countSplit, currentItem.FamilyNumber, currentItem.Status);
                    DataCount();
                    MultiObjectArg arg = new MultiObjectArg() { ParameterA = currentItem.Tag, ParameterB = page.NewItem.Tag };
                    TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_SPLIT_COMPLATE, arg));
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 承包方设置
        /// </summary>
        public void VirtualPersonSet()
        {
            if (currentPerson == null || CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.PersonSet, VirtualPersonInfo.PersonSetNull);
                return;
            }
            if (currentItem.Tag.Name == currentPerson.Name && currentItem.Tag.Number == currentPerson.ICN)
            {
                ShowBox(VirtualPersonInfo.PersonSet, VirtualPersonInfo.PersonIsMain);
                return;
            }
            if (currentItem.Tag.Status == eVirtualPersonStatus.Lock)
            {
                ShowBox(VirtualPersonInfo.PersonSet, VirtualPersonInfo.PersonSetLock);
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
                    BindPerson mainPerson = currentItem.Children.FirstOrDefault(t => t.FamilyID == currentItem.ID);
                    bool result = Business.SetMainPerson(currentItem.Tag, currentPerson);
                    if (!result)
                    {
                        ShowBox(VirtualPersonInfo.PersonSet, VirtualPersonInfo.PersonSetFail);
                        return;
                    }
                    else
                    {
                        //mainPerson.Relationship = "";
                        UpdateItems(currentItem.Tag, false);
                    }
                    ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_SET_COMPLATE, currentItem.Tag);
                    TheBns.Current.Message.Send(this, arg);  //
                    ThePage.Workspace.Message.Send(this, arg);  //
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteError(this, "VirtualPersonSet(设置户主)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(VirtualPersonInfo.PersonSet, VirtualPersonInfo.PersonSetConfirm, eMessageGrade.Warn, action);
        }

        /// <summary>
        /// 锁定/解锁
        /// </summary>
        public void VirtualPersonLock()
        {
            if (Items.Count == 0)
            {
                return;
            }
            var temPersonItems = new List<VirtualPersonItem>();
            foreach (var item in Items)
            {
                if (item == null)
                    continue;
                temPersonItems.Add(item.Clone() as VirtualPersonItem);
            }
            PersonLockPage page = new PersonLockPage();
            page.PersonItems = temPersonItems;
            page.Business = Business;
            ThePage.Page.ShowMessageBox(page, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (!page.Result)
                {
                    ShowBox(VirtualPersonInfo.PersonLock, VirtualPersonInfo.PersonLockFail);
                    return;
                }
                List<VirtualPerson> list = UpdateLock(page.PersonItems);
                ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_STATUSCHANGE, list);
                SendMessasge(arg);
                //TheBns.Current.Message.Send(this, arg);
            });
        }

        /// <summary>
        /// 合户
        /// </summary>
        public void VirtualPersonCombine()
        {
            if (Items.Count == 0)
            {
                return;
            }
            PersonCombingPage page = new PersonCombingPage();
            page.PersonItems = Items;
            page.Business = Business;
            ThePage.Page.ShowMessageBox(page, (b, e) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (!page.Result)
                {
                    ShowBox(VirtualPersonInfo.PersonCombine, VirtualPersonInfo.PersonCombineFail);
                    return;
                }
                UpdateCombine(page.DestinationItem, page.SourceItem);
                DataCount();
                ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_COMBINE_COMPLATE,
                    new MultiObjectArg() { ParameterA = page.DestinationItem.Tag, ParameterB = page.SourceItem.Tag });
                SendMessasge(arg);
                //TheBns.Current.Message.Send(this, arg);
            });
        }

        /// <summary>
        /// 家庭关系处理
        /// </summary>
        public void RelationCheck()
        {
            bool canContinue = CanContinue("数据处理", false);
            if (!canContinue)
            {
                return;
            }
            var zonestation = DbContext.CreateZoneWorkStation();
            List<Zone> zones = zonestation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (zones.Count <= 1)
            {
                TaskRelationDealArgument metadata = new TaskRelationDealArgument();
                metadata.CurrentZone = CurrentZone;
                metadata.Database = DbContext;
                metadata.Type = 1;
                TaskRelationDealOperation deal = new TaskRelationDealOperation();
                deal.Argument = metadata;
                deal.Name = "数据处理";
                deal.Description = "家庭关系检查";
                deal.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                ThePage.TaskCenter.Add(deal);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                deal.StartAsync();
            }
            else
            {
                TaskGroupRelationDealArgument argument = new TaskGroupRelationDealArgument();
                argument.Database = DbContext;
                argument.CurrentZone = CurrentZone;
                argument.Type = 1;
                TaskGroupRelationDealOperation task = new TaskGroupRelationDealOperation();
                task.Argument = argument;
                task.Name = "数据处理";
                task.Description = "家庭关系检查";
                task.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                ThePage.TaskCenter.Add(task);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                task.StartAsync();
            }
        }

        public void RelationReplace()
        {
            bool canContinue = CanContinue("数据替换", false);
            if (!canContinue)
            {
                return;
            }
            InitializeVirtualPersonRelationship initForm = new InitializeVirtualPersonRelationship();
            initForm.DataSource = DbContext;
            initForm.CurrentZone = currentZone;
            ThePage.Page.ShowMessageBox(initForm, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var zonestation = DbContext.CreateZoneWorkStation();
                List<Zone> zones = zonestation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                if (zones.Count <= 1)
                {
                    TaskInitialVPRelationShipArgument argument = new TaskInitialVPRelationShipArgument();
                    //argument.VPS = initForm.vps;
                    argument.Database = DbContext;
                    argument.CurrentZone = CurrentZone;
                    argument.ReplaceName = initForm.txt_content.Text.Trim();
                    argument.ChooseName = initForm.cbRelationship.Text;
                    TaskInitialVPRelationShipOperation task = new TaskInitialVPRelationShipOperation();
                    task.Argument = argument;
                    task.Name = "家庭关系替换";
                    task.Description = "家庭关系替换";
                    task.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                    });
                    ThePage.TaskCenter.Add(task);
                    if (ShowTaskViewer != null)
                    {
                        ShowTaskViewer();
                    }
                    task.StartAsync();
                }
                else
                {
                    TaskGroupInitialVPRelationShipArgument argument = new TaskGroupInitialVPRelationShipArgument();
                    argument.Database = DbContext;
                    argument.CurrentZone = CurrentZone;
                    argument.ReplaceName = initForm.txt_content.Text.Trim();
                    argument.ChooseName = initForm.cbRelationship.Text;
                    TaskGroupInitialVPRelationShipOperation task = new TaskGroupInitialVPRelationShipOperation();
                    task.Argument = argument;
                    task.Name = "家庭关系替换";
                    task.Description = CurrentZone.FullName + "家庭关系替换";
                    task.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                    });
                    ThePage.TaskCenter.Add(task);
                    if (ShowTaskViewer != null)
                    {
                        ShowTaskViewer();
                    }
                    task.StartAsync();
                }
            });
        }

        #endregion 数据处理

        #region 工具

        /// <summary>
        /// 家庭成员数据修复
        /// </summary>
        public void SharePersonRepair()
        {
            bool canContinue = CanContinue("家庭成员修复", false);
            if (!canContinue)
            {
                return;
            }
            var zonestation = DbContext.CreateZoneWorkStation();
            List<Zone> zones = zonestation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (zones.Count <= 1)
            {
                var metadata = new TaskRepairSharePersonOperationArgument();
                metadata.CurrentZone = CurrentZone;
                metadata.Database = DbContext;
                var task = new TaskRepairSharePersonOperation();
                task.Argument = metadata;
                task.Name = "数据修复";
                task.Description = "家庭成员关联数据修复";
                task.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                ThePage.TaskCenter.Add(task);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                task.StartAsync();
            }
            else
            {
                var argument = new TaskGroupRepairSharePersonOperationArgument();
                argument.Database = DbContext;
                argument.CurrentZone = CurrentZone;
                var task = new TaskGroupRepairSharePersonOperation();
                task.Argument = argument;
                task.Name = "数据修复";
                task.Description = "家庭成员关联数据修复";
                task.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                ThePage.TaskCenter.Add(task);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                task.StartAsync();
            }
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void VirtualPersonInitialize()
        {
            if (currentZone == null)
            {
                ShowBox(VirtualPersonInfo.PersonInitiall, VirtualPersonInfo.InitialPersonInfoNoZone);
                return;
            }
            //if (isBpwpUse==false&&Items != null && !Items.Any(c => c.Tag.Status == eVirtualPersonStatus.Right))
            //{
            //    ShowBox(VirtualPersonInfo.PersonInitiall, "地域下的所有承包方数据都被锁定,无法执行初始化操作!", eMessageGrade.Warn);
            //    return;
            //}
            List<Zone> childrenZone = new List<Zone>();
            try
            {
                if (DbContext == null)
                    DbContext = CreateDb();
                var zoneStation = DbContext.CreateZoneWorkStation();
                childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                int vpscount = vpStation.CountByZone(currentZone.FullCode);
                if ((currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) && vpscount == 0)
                {
                    //选择地域为组级地域或者大于组级地域时没有子级地域
                    //没有承包方信息
                    ShowBox(VirtualPersonInfo.PersonInitiall, VirtualPersonInfo.CurrentZoneNoPerson);
                    return;
                }
                if (currentZone.Level > eZoneLevel.Town)
                {
                    ShowBox(VirtualPersonInfo.PersonInitiall, VirtualPersonInfo.InitialPersonSelectZoneError);
                    return;
                }
                PersonInitallizePage page = new PersonInitallizePage();
                page.Workpage = ThePage;
                page.Address = currentZone.FullName;
                ThePage.Page.ShowMessageBox(page, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                    {
                        //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                        VirtualPersonInitializeTask(page);
                    }
                    else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                    {
                        //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                        VirtualPersonInitializeTaskGroup(page);
                    }
                    childrenZone.Clear();
                    childrenZone = null;
                });
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualPersonInitialize(初始化承包方基本信息)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 初始化承包方信息任务(单个任务)
        /// </summary>
        /// <param name="page">初始化界面</param>
        private void VirtualPersonInitializeTask(PersonInitallizePage page)
        {
            if (page == null)
                return;
            List<VirtualPerson> persons = new List<VirtualPerson>();
            foreach (var item in Items)
            {
                persons.Add(item.Tag);
            }
            TaskInitialVirtualPersonArgument argument = new TaskInitialVirtualPersonArgument();
            argument.Database = DbContext == null ? DataBaseSource.GetDataBaseSource() : DbContext;
            argument.CurrentZone = CurrentZone;
            argument.InitiallNumber = page.InitiallNumber;
            argument.InitiallNation = page.InitiallNation;
            argument.InitiallZip = page.InitiallZip;
            argument.InitPersonComment = page.InitPersonComment;
            argument.InitiallSex = page.InitPersonSex;
            //添加初始化共有人备注
            argument.InitSharePersonComment = page.InitSharePersonComment;
            argument.InitiallVpAddress = page.InitiallVpAddress;
            argument.InitiallSurveyPerson = page.InitiallSurveyPerson;
            argument.InitiallSurveyDate = page.InitiallSurveyDate;
            argument.InitiallSurveyAccount = page.InitiallSurveyAccount;
            argument.InitiallCheckPerson = page.InitiallCheckPerson;
            argument.InitiallCheckDate = page.InitiallCheckDate;
            argument.InitiallCheckOpinion = page.InitiallCheckOpinion;
            argument.InitiallPublishAccountPerson = page.InitiallPublishAccountPerson;
            argument.InitiallPublishDate = page.InitiallPublishDate;
            argument.InitiallPublishCheckPerson = page.InitiallPublishCheckPerson;
            argument.InitiallcbPublishAccount = page.InitiallcbPublishAccount;
            argument.InitialNull = page.InitialNull;
            argument.InitialContractWay = page.InitialContractWay;
            argument.InitConcordNumber = page.InitConcordNumber;
            argument.InitWarrentNumber = page.InitWarrentNumber;
            argument.InitStartTime = page.InitStartTime;
            argument.InitEndTime = page.InitEndTime;
            argument.Address = page.Address;
            argument.ZipCode = page.ZipCode;
            argument.PersonComment = page.PersonComment;
            argument.Expand = page.Expand;
            argument.CNation = page.CNation;
            argument.VirtualType = this.VirtualType;
            argument.ListPerson = VirtualPersonFilter(persons);
            argument.VillageInlitialSet = false;
            argument.FarmerFamilyNumberIndex = new int[] { 1 };  //农户
            argument.PersonalFamilyNumberIndex = new int[] { 8001 };  //个人
            argument.UnitFamilyNumberIndex = new int[] { 9001 };  //单位
            TaskInitialVirtualPersonOperation operation = new TaskInitialVirtualPersonOperation();
            operation.Argument = argument;
            operation.Description = VirtualPersonInfo.PersonInitialDesc;   //任务描述
            operation.Name = VirtualPersonInfo.PersonInitiall;       //任务名称
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_INITIAL_COMPLATE, persons);
                SendMessasge(arg);
                //TheBns.Current.Message.Send(this, arg);
            });
            ThePage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量初始化承包方信息任务(组任务)
        /// </summary>
        /// <param name="page">初始化界面</param>
        private void VirtualPersonInitializeTaskGroup(PersonInitallizePage page)
        {
            if (page == null)
                return;
            TaskGroupInitialVirtualPersonArgument groupArgument = new TaskGroupInitialVirtualPersonArgument();
            groupArgument.Database = DbContext == null ? DataBaseSource.GetDataBaseSource() : DbContext;
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.InitiallNumber = page.InitiallNumber;
            groupArgument.InitiallNation = page.InitiallNation;
            groupArgument.InitiallZip = page.InitiallZip;
            groupArgument.InitPersonComment = page.InitPersonComment;
            //添加初始化共有人备注
            groupArgument.InitSharePersonComment = page.InitSharePersonComment;
            groupArgument.InitiallVpAddress = page.InitiallVpAddress;
            groupArgument.InitiallSurveyPerson = page.InitiallSurveyPerson;
            groupArgument.InitiallSurveyDate = page.InitiallSurveyDate;
            groupArgument.InitiallSurveyAccount = page.InitiallSurveyAccount;
            groupArgument.InitiallCheckPerson = page.InitiallCheckPerson;
            groupArgument.InitiallCheckDate = page.InitiallCheckDate;
            groupArgument.InitiallCheckOpinion = page.InitiallCheckOpinion;
            groupArgument.InitiallPublishAccountPerson = page.InitiallPublishAccountPerson;
            groupArgument.InitiallPublishDate = page.InitiallPublishDate;
            groupArgument.InitiallPublishCheckPerson = page.InitiallPublishCheckPerson;
            groupArgument.InitiallcbPublishAccount = page.InitiallcbPublishAccount;
            groupArgument.InitialNull = page.InitialNull;
            groupArgument.InitialContractWay = page.InitialContractWay;
            groupArgument.InitConcordNumber = page.InitConcordNumber;
            groupArgument.InitWarrentNumber = page.InitWarrentNumber;
            groupArgument.InitStartTime = page.InitStartTime;
            groupArgument.InitEndTime = page.InitEndTime;
            groupArgument.Address = page.Address;
            groupArgument.ZipCode = page.ZipCode;
            groupArgument.PersonComment = page.PersonComment;
            groupArgument.Expand = page.Expand;
            groupArgument.CNation = page.CNation;
            groupArgument.VirtualType = this.VirtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.VillageInlitialSet = currentZone.Level == eZoneLevel.Village ? SystemSet.VillageInlitialSet : false;
            TaskGroupInitialVirtualPersonOperation groupOperation = new TaskGroupInitialVirtualPersonOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = VirtualPersonInfo.PersonInitialDesc;   //任务描述
            groupOperation.Name = VirtualPersonInfo.PersonInitiall;       //任务名称
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_INITIAL_COMPLATE, null);
                SendMessasge(arg);
                //TheBns.Current.Message.Send(this, arg);
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 过滤显示承包方
        /// </summary>
        private List<VirtualPerson> VirtualPersonFilter(List<VirtualPerson> listPersons)
        {
            List<VirtualPerson> vps = new List<VirtualPerson>();
            if (listPersons == null || listPersons.Count == 0)
                return vps;
            try
            {
                if (FamilyOtherSet.ShowFamilyInfomation)
                {
                    var persons = listPersons.FindAll(c => c.Name == "集体");
                    foreach (var vp in persons)
                    {
                        listPersons.Remove(vp);
                    }
                    persons.Clear();
                }
                //listPersons.ForEach(c =>
                //{
                //    var vp = c.Status == eVirtualPersonStatus.Right ? c : null;
                //    if (vp != null)
                //        vps.Add(vp);
                //});
                listPersons.ForEach(c => vps.Add(c));
                listPersons.Clear();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VitualPersonFilter(承包方过滤失败!)", ex.Message + ex.StackTrace);
                ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
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
        /// 查询数据
        /// </summary>
        public void VirtualPersonSearch()
        {
            if (Items.Count == 0)
            {
                return;
            }
            PersonSearchPage page = new PersonSearchPage();
            page.Search += SearchPerson;
            page.Owner = ThePage.Workspace.Window.Instance as Window;
            page.ShowDialog();
        }

        /// <summary>
        /// 查询数据方法
        /// </summary>
        private SearchNumber SearchPerson(string name, string code, int type)
        {
            if (type == 2)
            {
                index = 0;
                list.Clear();
                foreach (var vpitem in Items)
                {//陈泽林改20161201
                    if (vpitem.Name.IsNullOrBlank() && vpitem.ICN.IsNullOrBlank())
                        continue;
                    if ((!vpitem.Name.IsNullOrBlank() && !name.IsNullOrBlank() && vpitem.Name.Contains(name)) ||
                        (!vpitem.ICN.IsNullOrBlank() && !code.IsNullOrBlank() && vpitem.ICN.Contains(code)))
                        list.Add(vpitem);
                    foreach (var bp in vpitem.Children)
                    {
                        if ((!bp.Name.IsNullOrBlank() && !name.IsNullOrBlank() && bp.Name.Contains(name)) ||
                            (!bp.ICN.IsNullOrBlank() && !code.IsNullOrBlank() && bp.ICN.Contains(code)))
                            list.Add(bp);
                    }
                }
                sNumber.DataCount = list.Count;
            }
            else if (type == 1)
            {
                if (index > 0)
                {
                    index -= 1;
                    sNumber.CurrentIndex = index;
                }
                else
                {
                    //ShowBox("提示", "已到最前面一个");
                    return sNumber;
                }
            }
            else if (type == 3)
            {
                if (index < list.Count - 1)
                {
                    index += 1;
                    sNumber.CurrentIndex = index;
                }
                else
                {
                    //ShowBox("提示", "已到最后面一个");
                    return sNumber;
                }
            }
            if (list.Count == 0)
            {
                return sNumber;
            }
            object selectValue = list[index];
            view.BringIntoView(selectValue);

            return sNumber;
        }

        /// <summary>
        /// 清空当前地域承包方
        /// </summary>
        public void Clear()
        {
            if (currentZone == null)
            {
                ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearNoSelectZone);
                return;
            }
            if ((currentZone.Level == eZoneLevel.Group && Items != null && Items.Count(c => c.Tag.Status == eVirtualPersonStatus.Right) == 0))
            {
                ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearNoValidData);
                return;
            }
            var clearDlg = new MessageDialog
            {
                Header = VirtualPersonInfo.Clear,
                Message = VirtualPersonInfo.ClearConfirm,
                MessageGrade = eMessageGrade.Warn,
            };
            clearDlg.Confirm += (s, a) =>
            {
                try
                {
                    if (DbContext == null)
                        DbContext = CreateDb();
                    ContainerFactory factory = new ContainerFactory(this.DbContext);
                    var vpStation = this.DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                    var familysLock = vpStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.SelfAndSubs);
                    if (familysLock != null && familysLock.Count == 0)
                    {
                        vpStation.ClearZoneVirtualPersonALLData(CurrentZone.FullCode);
                    }
                    else
                    {
                        //DeleteVirtualPerson(factory, familys);
                        var familyRight = vpStation.GetByZoneCode(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
                        var rightVpids = familyRight.Select(c => c.ID).ToList();
                        vpStation.DeleteRelationDataByVps(rightVpids);
                    }
                }
                catch (Exception)
                { }
            };
            clearDlg.ConfirmStart += (s, a) =>
            {
                MenuEnable(false);
                ThePage.Page.IsBusy = true;
            };
            clearDlg.ConfirmCompleted += (s, a) =>
            {
                MenuEnable();
                ThePage.Page.IsBusy = false;
                var removeData = Items.Where(c => c.Tag.Status == eVirtualPersonStatus.Right).TryToList();
                foreach (var data in removeData)
                {
                    Items.Remove(data);
                }
                DataCount();
                ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.CLEAR_COMPLATE, null, CurrentZone.FullCode);
                SendMessasge(arg);
            };
            clearDlg.ConfirmTerminated += (s, a) =>
            {
                ThePage.Page.IsBusy = false;
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除未被锁定的承包方失败!)", a.Exception.ToString());
                ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearFail);
            };
            ThePage.Page.ShowMessageBox(clearDlg, (b, r) =>
            {
                if (!(bool)b)
                    return;
            });
        }

        #region 代码注释——此种方式清空数据会有界面卡顿的不好用户体验

        /// <summary>
        /// 清空
        /// </summary>
        //private void TaskClear()
        //{
        //    if (currentZone == null)
        //    {
        //        ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearNoSelectZone);
        //        return;
        //    }
        //    if ((currentZone.Level == eZoneLevel.Group && Items != null && Items.Count(c => c.Tag.Status == eVirtualPersonStatus.Right) == 0))
        //    {
        //        ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearNoValidData);
        //        return;
        //    }

        //    Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
        //    {
        //        if (b != null && !(bool)b)
        //        {
        //            return;
        //        }
        //        try
        //        {
        //            if (DbContext == null)
        //                DbContext = CreateDb();
        //            ContainerFactory factory = new ContainerFactory(this.DbContext);
        //            var vpStation = this.DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
        //            var familysLock = vpStation.GetByZoneCode(CurrentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
        //            if (familysLock != null && familysLock.Count == 0)
        //            {
        //                vpStation.ClearZoneVirtualPersonALLData(CurrentZone.FullCode);
        //            }
        //            else
        //            {
        //                //DeleteVirtualPerson(factory, familys);
        //                var familyRight = vpStation.GetByZoneCode(currentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
        //                var rightVpids = familyRight.Select(c => c.ID).ToList();
        //                vpStation.DeleteRelationDataByVps(rightVpids);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除未被锁定的承包方失败!)", ex.Message + ex.StackTrace);
        //            ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearFail);
        //        }
        //        finally
        //        {
        //            var removeData = Items.Where(c => c.Tag.Status == eVirtualPersonStatus.Right).TryToList();
        //            foreach (var data in removeData)
        //            {
        //                Items.Remove(data);
        //            }
        //            DataCount();
        //            ModuleMsgArgs arg = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.CLEAR_COMPLATE, null, CurrentZone.FullCode);
        //            SendMessasge(arg);
        //        }
        //    });
        //    ShowBox(VirtualPersonInfo.Clear, VirtualPersonInfo.ClearConfirm, eMessageGrade.Warn, action);
        //}

        #endregion 代码注释——此种方式清空数据会有界面卡顿的不好用户体验

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

        #endregion 工具

        #region 导入导出

        #region 导出模板

        /// <summary>
        /// 导出承包方Excel模板
        /// </summary>
        public void ExportExcelTemplate()
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
        public void ExportWordTemplate()
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

        #endregion 导出模板

        #region 导入承包方调查表

        ///<summary>
        ///导入户籍表
        ///</summary>
        public void ImportData()
        {
            if (currentZone == null)
            {
                ShowBox(VirtualPersonInfo.ImportVirtual, VirtualPersonInfo.ImportNoZone);
                return;
            }
            List<Zone> childrenZones = Business.GetChildZone(currentZone);
            if ((currentZone.Level == eZoneLevel.Group) || (currentZone.Level == eZoneLevel.Village && childrenZones.Count == 0))
            {
                //选择为组级地域或者选择为村级地域的同时地域下没有子级地域(单个表格导入,执行单个任务)
                if (Items != null && Items.Count > 0)
                {
                    //当前地域下有数据,则提示是否清空当前数据再执行导入操作
                    Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((s, t) =>
                    {
                        if (!(bool)s)
                        {
                            return;
                        }
                        ImportDataPage importVp = new ImportDataPage(ThePage, "导入承包方调查表");
                        ThePage.Page.ShowMessageBox(importVp, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(importVp.FileName) || b == false)
                            {
                                return;
                            }
                            ImportVirtualPersonInformationTask(importVp.FileName);
                        });
                    });
                    ShowBox(VirtualPersonInfo.ImportVirtual, VirtualPersonInfo.ImportPersonIsClear, eMessageGrade.Infomation, action);
                }
                else
                {
                    ImportDataPage importVp = new ImportDataPage(ThePage, "导入承包方调查表");
                    ThePage.Page.ShowMessageBox(importVp, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(importVp.FileName) || b == false)
                        {
                            return;
                        }
                        ImportVirtualPersonInformationTask(importVp.FileName);
                    });
                }
            }
            else if (currentZone.Level == eZoneLevel.Village && childrenZones != null && childrenZones.Count > 0)
            {
                //选择为村级地域并且地域下有子级地域(多个表格导入)
                ExportDataPage volumnImportDlg = new ExportDataPage(CurrentZone.FullName, ThePage, "批量导入承包方调查表", "请选择保存文件路径", "导入", "导入路径:");
                ThePage.Page.ShowMessageBox(volumnImportDlg, (b, r) =>
                {
                    if (!(bool)b || string.IsNullOrEmpty(volumnImportDlg.FileName))
                    {
                        return;
                    }
                    ImportVirtualPersonInformationTaskGroup(volumnImportDlg.FileName);
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
        /// 导入承包方调查表(单个任务)
        /// </summary>
        /// <param name="fileName">选择文件路径</param>
        private void ImportVirtualPersonInformationTask(string fileName)
        {
            TaskImportVirtualPersonTableArgument meta
                = new TaskImportVirtualPersonTableArgument();
            meta.FileName = fileName;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyImportSet = FamilyImportSet;
            meta.FamilyOtherSet = FamilyOtherSet;
            TaskImportVirtualPersonTableOperation import
                = new TaskImportVirtualPersonTableOperation();

            import.Argument = meta;
            import.Description = VirtualPersonInfo.ImportDataComment;
            import.Name = VirtualPersonInfo.ImportData;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                var args = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_IMPORT_COMPLETE, currentZone.FullCode);
                SendMessasge(args);
                //TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_IMPORT_COMPLETE, currentZone.FullCode));
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(VirtualPersonInfo.ImportData, VirtualPersonInfo.ImportDataFail);
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导入承包方调查表(多个任务)
        /// </summary>
        /// <param name="fileName">选择文件夹路径</param>
        private void ImportVirtualPersonInformationTaskGroup(string fileName)
        {
            TaskGroupImportVirtualPersonTableArgument groupArgument
                = new TaskGroupImportVirtualPersonTableArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyImportSet = FamilyImportSet;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            TaskGroupImportVirtualPersonTableOperation groupOperation
                = new TaskGroupImportVirtualPersonTableOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = VirtualPersonInfo.ImportDataComment;
            groupOperation.Name = VirtualPersonInfo.VolumnImportData;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                var args = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_IMPORT_COMPLETE, currentZone.FullCode);
                SendMessasge(args);
                //TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_IMPORT_COMPLETE, currentZone.FullCode));
            });
            groupOperation.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(VirtualPersonInfo.VolumnImportData, VirtualPersonInfo.VolumnImportFail);
            });
            ThePage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 抽象出的公有导入表格方法
        /// </summary>
        private void CommonImport(TaskVirtualPersonArgument meta, IDbContext dbContext,
            string str1, string str2, string str3, string str4, string str5)
        {
            TaskVirtualPersonOperation import = new TaskVirtualPersonOperation();
            import.Argument = meta;
            import.Description = str1;
            import.Name = str2;
            import.FamilyImportSet = FamilyImportSet;
            import.FamilyOtherSet = FamilyOtherSet;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(dbContext, str3, currentZone.FullCode));
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                ShowBox(str4, str5);
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        #endregion 导入承包方调查表

        #region 导出承包方调查表

        /// <summary>
        /// 导出户籍表
        /// </summary>
        public void ExportVirtualPersonExcel()
        {
            if (CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(VirtualPersonInfo.ExportTable, VirtualPersonInfo.ExportNoZone);
                return;
            }
            if (currentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇
                ShowBox(VirtualPersonInfo.ExportData, VirtualPersonInfo.VolumnExportZoneError);
                return;
            }
            List<Zone> childrenZone = null;
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                if (zoneStation != null)
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
            }
            catch (Exception ex)
            {
                ShowBox(VirtualPersonInfo.ExportTable, "获取当前地域的子级地域出错");
                YuLinTu.Library.Log.Log.WriteException(this, "获取当前地域的子级地域", ex.Message + ex.StackTrace);
                return;
            }
            bool haveChildZone = childrenZone != null && childrenZone.Count > 0;
            if (haveChildZone)
            {
                //组任务
                ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportTable);
                extPage.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (b == false || string.IsNullOrEmpty(extPage.FileName))
                    {
                        return;
                    }
                    ExportVirtualPersonExcelTaskGroup(extPage.FileName, haveChildZone);
                });
            }
            else
            {
                //单个任务
                if (Items == null || Items.Count == 0)
                {
                    ShowBox(VirtualPersonInfo.ExportTable, VirtualPersonInfo.CurrentZoneNoPerson);
                    return;
                }
                //陈泽林 20161010 一张表直接预览
                ExportVirtualPersonExcelTask(SystemSet.DefaultPath, haveChildZone);
                //ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportTable);
                //extPage.Workpage = ThePage;
                //ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                //{
                //    if (b == false || string.IsNullOrEmpty(extPage.FileName))
                //    {
                //        return;
                //    }
                //    ExportVirtualPersonExcelTask(extPage.FileName);
                //});
            }
        }

        /// <summary>
        /// 导出承包方调查表(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        private void ExportVirtualPersonExcelTask(string filePath, bool hasChildren)
        {
            TaskExportVirtualPersonExcelArgument meta = new TaskExportVirtualPersonExcelArgument();
            meta.FileName = filePath;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.FamilyOutputSet = FamilyOutputSet;
            meta.hasChildren = hasChildren;
            meta.SystemSet = SystemSet;
            TaskExportVirtualPersonExcelOperation import = new TaskExportVirtualPersonExcelOperation();
            import.Argument = meta;
            import.Description = VirtualPersonInfo.ExportDataExcel;
            import.Name = VirtualPersonInfo.ExportTable;
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
        /// 导出承包方调查表(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        private void ExportVirtualPersonExcelTaskGroup(string filePath, bool hasChildren)
        {
            TaskGroupExportVirtualPersonExcelArgument groupArgument = new TaskGroupExportVirtualPersonExcelArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.FamilyOutputSet = FamilyOutputSet;
            groupArgument.SystemSet = SystemSet;
            groupArgument.hasChildren = hasChildren;
            TaskGroupExportVirtualPersonExcelOperation groupOperation = new TaskGroupExportVirtualPersonExcelOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = VirtualPersonInfo.ExportDataExcel;
            groupOperation.Name = VirtualPersonInfo.ExportTable;
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
        /// 导出户籍表
        /// </summary>
        public void ExportVirtualPersonWord()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportTable, VirtualPersonInfo.ExportNoZone);
                return;
            }
            bool isbatch = IsBatch;
            if (isbatch)
            {
                ExportVirtualPersonWordBatch();
            }
            else
            {
                ExportVirtualPersonWordSingle();
            }
        }

        private void ExportVirtualPersonWordSingle()
        {
            if (currentItem == null)
            {
                //界面上没有选择项
                ShowBox(VirtualPersonInfo.ExportTable, "请先选择需要预览的承包方");
                return;
            }
            if (currentItem.Tag == null)
            {
                ShowBox(VirtualPersonInfo.ExportTable, "选择项不是承包方");
                return;
            }
            var senderStation = DbContext.CreateSenderWorkStation();
            CollectivityTissue tissue = senderStation.Get(currentZone.ID);
            if (tissue == null)
            {
                var tissues = senderStation.GetTissues(CurrentZone.FullCode, eLevelOption.Self);
                if (tissues != null && tissues.Count > 0)
                {
                    tissue = tissues[0];
                }
            }
            if (tissue == null)
            {
                //界面上没有选择项
                ShowBox(VirtualPersonInfo.ExportTable, currentZone.FullName + "下没有发包方数据");
                return;
            }
            string masdesc = GetMarkDesc(currentZone);
            List<ContractConcord> concords = null;
            try
            {
                var concordStation = DbContext.CreateConcordStation();
                if (concordStation != null)
                    concords = concordStation.GetAllConcordByFamilyID(currentItem.Tag.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetAllConcordByFamilyID(根据承包方Id获取农村土地承包合同)", ex.Message + ex.StackTrace);
            }
            string concordnumber = null;
            if (concords != null && concords.Count > 0)
            {
                concordnumber = concords[0].ConcordNumber;
            }
            //界面上有当前选择承包方项(此时做预览操作)
            Business.ExportDataWord(currentZone, currentItem.Tag, masdesc, concordnumber, tissue, DictList);   //导出单个
        }

        private void ExportVirtualPersonWordBatch()
        {
            List<Zone> childrenZone = null;
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                if (zoneStation != null)
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
            }
            catch (Exception ex)
            {
                ShowBox(VirtualPersonInfo.ExportTable, "获取当前地域的子级地域出错");
                YuLinTu.Library.Log.Log.WriteException(this, "获取当前地域的子级地域", ex.Message + ex.StackTrace);
                return;
            }
            if (currentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇
                ShowBox(VirtualPersonInfo.ExportData, VirtualPersonInfo.VolumnExportZoneError);
                return;
            }
            bool haveChildZone = childrenZone != null && childrenZone.Count > 0;
            if (haveChildZone)
            {
                //组任务
                ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportTable);
                extPage.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (b != true || string.IsNullOrEmpty(extPage.FileName))
                    {
                        return;
                    }
                    ExportVirtualPersonWordTaskGroup(extPage.FileName);
                });
            }
            else
            {
                //单个任务
                if (Items == null || Items.Count == 0)
                {
                    ShowBox(VirtualPersonInfo.ExportTable, VirtualPersonInfo.CurrentZoneNoPerson);
                    return;
                }
                List<VirtualPerson> persons = new List<VirtualPerson>();
                //界面上没有选择承包方项(此时弹出承包方选择界面)
                ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                selectPage.Workpage = ThePage;
                foreach (var item in Items)
                {
                    persons.Add(item.Tag);
                }
                selectPage.PersonItems = persons;
                ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                {
                    if (b != true)
                    {
                        return;
                    }
                    var selected = selectPage.SelectedPersons;
                    if (selected == null || selected.Count == 0)
                    {
                        ShowBox(VirtualPersonInfo.ExportTable, VirtualPersonInfo.ExportDataNo);
                        return;
                    }
                    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportTable);
                    extPage.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(extPage, (t, s) =>
                    {
                        if (t != true || string.IsNullOrEmpty(extPage.FileName))
                        {
                            return;
                        }
                        ExportVirtualPersonWordTask(extPage.FileName, selected);
                    });
                });
            }

            //catch (Exception ex)
            //{
            //    YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonWord(导出承包方Word调查表)", ex.Message + ex.StackTrace);
            //    return;
            //}
        }

        /// <summary>
        /// 导出Word承包方调查表(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="selectedPersons">选择承包方集合</param>
        private void ExportVirtualPersonWordTask(string filePath, List<VirtualPerson> selectedPersons)
        {
            TaskExportVirtualPersonWordArgument meta = new TaskExportVirtualPersonWordArgument();
            meta.FileName = filePath;
            meta.SelectedPersons = selectedPersons;
            meta.DbContext = DbContext;
            meta.DictList = DictList;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.SystemSet = SystemSet;
            var import = new TaskExportVirtualPersonWordOperation();
            import.Argument = meta;
            import.Description = VirtualPersonInfo.ExportDataWord;
            import.Name = VirtualPersonInfo.ExportTable;
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
        /// 批量导出Word承包方调查表(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        private void ExportVirtualPersonWordTaskGroup(string filePath)
        {
            TaskGroupExportVirtualPersonWordArgument groupArgument = new TaskGroupExportVirtualPersonWordArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.DictList = DictList;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.SystemSet = SystemSet;
            TaskGroupExportVirtualPersonWordOperation groupOperation = new TaskGroupExportVirtualPersonWordOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = VirtualPersonInfo.ExportDataWord;
            groupOperation.Name = VirtualPersonInfo.ExportTable;
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

        #endregion 导出承包方调查表

        #region 导出声明申请材料

        /// <summary>
        /// 预览户主声明书
        /// </summary>
        public void ApplyPreviewBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreivewNoZone);
                return;
            }
            if (currentItem == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreviewDataNo);
                return;
            }
            DateTime? time = DateTime.Now;
            if (FamilyOtherSet.FamilyInstructionDate)
            {
                //此时要弹出设置时间对话框
                DateSettingPage page = new DateSettingPage();
                page.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(page, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    time = page.SetTime;
                    Business.ApplyBookWord(currentZone, currentItem.Tag, time);
                });
            }
            else
            {
                //此时使用默认当前时间
                Business.ApplyBookWord(currentZone, currentItem.Tag, time);
            }
        }

        /// <summary>
        /// 导出户主声明书
        /// </summary>
        public void ExportApplyBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.ExportNoZone);
                return;
            }
            List<Zone> childrenZone = new List<Zone>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.CurrentZoneNoPerson);
                        return;
                    }

                    if (!IsBatch)
                    {
                        if (currentItem == null)
                        {
                            ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.ExportDataNo);
                            return;
                        }
                        //选中项(直接导出当前承包方项)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportApplyBook);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            DateTime? time = DateTime.Now;   //设置时间
                            bool isSuccess = true;   //导出是否成功
                            if (FamilyOtherSet.FamilyInstructionDate)
                            {
                                //此时要弹出设置时间对话框
                                DateSettingPage page = new DateSettingPage();
                                page.Workpage = ThePage;
                                ThePage.Page.ShowMessageBox(page, (c, s) =>
                                {
                                    if (!(bool)c)
                                    {
                                        return;
                                    }
                                    time = page.SetTime;
                                    isSuccess = Business.ApplyBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                    if (isSuccess)
                                        ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                                });
                            }
                            else
                            {
                                //此时使用默认当前时间
                                isSuccess = Business.ApplyBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                if (isSuccess)
                                    ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                            }
                        });
                    }
                    else
                    {
                        //界面上没有选中项(此时弹出承包方选择界面)
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        selectPage.PersonItems = listPerson;
                        selectPage.Business = this.Business;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.ExportDataNo);
                                return;
                            }
                            ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportApplyBook, ePersonArgType.ExportApply,
                        VirtualPersonInfo.ExportApplyBookComment, VirtualPersonInfo.ExportApplyBook, selectedPersons);
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
                {
                    //组任务
                    ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportApplyBook, ePersonArgType.ExportApply,
                       VirtualPersonInfo.ExportApplyBookComment, VirtualPersonInfo.ExportApplyBook, null, true);
                }
                else
                {
                    //选中地域大于镇级
                    ShowBox(VirtualPersonInfo.ExportApplyBook, VirtualPersonInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportApplyBook(导出户主声明书)", ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                childrenZone.Clear();
                childrenZone = null;
            }
        }

        /// <summary>
        /// 导出户主声明书(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="selectedPersons">选择承包方集合</param>
        /// <param name="time">日期</param>
        private void ExportApplyBookTask(string filePath, List<VirtualPerson> selectedPersons, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskExportApplyBookArgument meta = new TaskExportApplyBookArgument();
            meta.FileName = filePath;
            meta.SelectedPersons = selectedPersons;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.Time = time;
            TaskExportApplyBookOperation import = new TaskExportApplyBookOperation();
            import.Argument = meta;
            import.Description = taskDesc;
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
        /// 批量导出户主声明书(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="time">日期</param>
        private void ExportApplyBookTaskGroup(string filePath, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskGroupExportApplyBookArgument groupArgument = new TaskGroupExportApplyBookArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.Time = time;
            TaskGroupExportApplyBookOperation groupOperation = new TaskGroupExportApplyBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDesc;
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
        /// 预览委托声明书
        /// </summary>
        public void DelegatePreviewBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreivewNoZone);
                return;
            }
            if (currentItem == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreviewDataNo);
                return;
            }
            DateTime? time = DateTime.Now;
            if (FamilyOtherSet.ProxyDefineDate)
            {
                //此时要弹出设置时间对话框
                DateSettingPage page = new DateSettingPage();
                page.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(page, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    time = page.SetTime;
                    Business.DelegateBookWord(currentZone, currentItem.Tag, time);
                });
            }
            else
            {
                //此时使用默认当前时间
                Business.DelegateBookWord(currentZone, currentItem.Tag, time);
            }
        }

        /// <summary>
        /// 导出委托声明书
        /// </summary>
        public void ExportDelegateBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.ExportNoZone);
                return;
            }
            List<Zone> childrenZone = new List<Zone>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.CurrentZoneNoPerson);
                        return;
                    }
                    if (!IsBatch)
                    {
                        if (currentItem == null)
                        {
                            ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.ExportDataNo);
                            return;
                        }
                        //选中项(直接导出当前承包方项)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportDelegateBook);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            DateTime? time = DateTime.Now;   //设置时间
                            bool isSuccess = true;   //导出是否成功
                            if (FamilyOtherSet.ProxyDefineDate)
                            {
                                //此时要弹出设置时间对话框
                                DateSettingPage page = new DateSettingPage();
                                page.Workpage = ThePage;
                                ThePage.Page.ShowMessageBox(page, (c, s) =>
                                {
                                    if (!(bool)c)
                                    {
                                        return;
                                    }
                                    time = page.SetTime;
                                    isSuccess = Business.DelegateBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                    if (isSuccess)
                                        ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                                });
                            }
                            else
                            {
                                //此时使用默认当前时间
                                isSuccess = Business.DelegateBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                if (isSuccess)
                                    ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                            }
                        });
                    }
                    else
                    {
                        //界面上没有选中项(此时弹出承包方选择界面)
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        selectPage.PersonItems = listPerson;
                        selectPage.Business = this.Business;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.ExportDataNo);
                                return;
                            }
                            ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportDelegateBook, ePersonArgType.ExportDelegate,
                                VirtualPersonInfo.ExportDelegateBookComment, VirtualPersonInfo.ExportDelegateBook, selectedPersons);
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
                {
                    //组任务
                    ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportDelegateBook,
                       ePersonArgType.ExportDelegate, VirtualPersonInfo.ExportDelegateBookComment, VirtualPersonInfo.ExportDelegateBook, null, true);
                }
                else
                {
                    //选中地域大于镇级
                    ShowBox(VirtualPersonInfo.ExportDelegateBook, VirtualPersonInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDelegateBook(导出委托声明书)", ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                childrenZone.Clear();
                childrenZone = null;
            }
        }

        /// <summary>
        /// 导出委托声明书(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="selectedPersons">选择承包方集合</param>
        /// <param name="time">日期</param>
        private void ExportDelegateBookTask(string filePath, List<VirtualPerson> selectedPersons, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskExportDelegateBookArgument meta = new TaskExportDelegateBookArgument();
            meta.FileName = filePath;
            meta.SelectedPersons = selectedPersons;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.Time = time;
            TaskExportDelegateBookOperation import = new TaskExportDelegateBookOperation();
            import.Argument = meta;
            import.Description = taskDesc;
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
        /// 批量导出委托声明书(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="time">日期</param>
        private void ExportDelegateBookTaskGroup(string filePath, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskGroupExportDelegateBookArgument groupArgument = new TaskGroupExportDelegateBookArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.Time = time;
            TaskGroupExportDelegateBookOperation groupOperation = new TaskGroupExportDelegateBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDesc;
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
        /// 预览无异议声明书
        /// </summary>
        public void IdeaPreviewBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreivewNoZone);
                return;
            }
            if (currentItem == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreviewDataNo);
                return;
            }
            DateTime? delcTime = DateTime.Now;
            DateTime? pubTime = DateTime.Now;
            if (FamilyOtherSet.UniqueInstructionDate)
            {
                //此时要弹出设置时间对话框(声明日期和公示日期)
                DoubleDateSettingPage page = new DoubleDateSettingPage();
                page.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(page, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    delcTime = page.SetDecTime;
                    pubTime = page.SetPubTime;
                    Business.IdeaBookWord(currentZone, currentItem.Tag, delcTime, pubTime);
                });
            }
            else
            {
                //此时使用默认当前时间
                Business.IdeaBookWord(currentZone, currentItem.Tag, delcTime, pubTime);
            }
        }

        /// <summary>
        /// 导出无异议声明书
        /// </summary>
        public void ExportIdeaBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportNoZone);
                return;
            }
            List<Zone> childrenZone = new List<Zone>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.CurrentZoneNoPerson);
                        return;
                    }
                    if (!IsBatch)
                    {
                        if (currentItem == null)
                        {
                            ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportDataNo);
                            return;
                        }
                        //选中项(直接导出当前承包方项)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportIdeaBook);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            DateTime? delcTime = DateTime.Now;
                            DateTime? pubTime = DateTime.Now;
                            bool isSuccess = true;   //导出是否成功
                            if (FamilyOtherSet.UniqueInstructionDate)
                            {
                                //此时要弹出设置时间对话框(声明日期和公示日期)
                                DoubleDateSettingPage page = new DoubleDateSettingPage();
                                page.Workpage = ThePage;
                                ThePage.Page.ShowMessageBox(page, (c, t) =>
                                {
                                    if (!(bool)c)
                                    {
                                        return;
                                    }
                                    delcTime = page.SetDecTime;
                                    pubTime = page.SetPubTime;
                                    isSuccess = Business.IdeaBookWord(currentZone, currentItem.Tag, delcTime, pubTime, true, savePage.FileName);
                                    if (isSuccess)
                                        ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                                });
                            }
                            else
                            {
                                //此时使用默认当前时间
                                isSuccess = Business.IdeaBookWord(currentZone, currentItem.Tag, delcTime, pubTime, true, savePage.FileName);
                                if (isSuccess)
                                    ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                            }
                        });
                    }
                    else
                    {
                        //界面上没有选中项(此时弹出承包方选择界面)
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        selectPage.PersonItems = listPerson;
                        selectPage.Business = this.Business;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportDataNo);
                                return;
                            }
                            ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportIdeaBook, ePersonArgType.ExportIdea,
                                VirtualPersonInfo.ExportIdeaBookComment, VirtualPersonInfo.ExportIdeaBook, selectedPersons);
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
                {
                    //组任务
                    ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportIdeaBook,
                ePersonArgType.ExportIdea, VirtualPersonInfo.ExportIdeaBookComment, VirtualPersonInfo.ExportIdeaBook, null, true);
                }
                else
                {
                    //选中地域大于镇级
                    ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportIdeaBook(导出无异议声明书)", ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                childrenZone.Clear();
                childrenZone = null;
            }
        }

        /// <summary>
        /// 导出无异议声明书(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="selectedPersons">选择承包方集合</param>
        /// <param name="time">日期</param>
        private void ExportIdeaBookTask(string filePath, List<VirtualPerson> selectedPersons, string taskDesc = "", string taskName = "",
            DateTime? time = null, DateTime? pubTime = null)
        {
            TaskExportIdeaBookArgument meta = new TaskExportIdeaBookArgument();
            meta.FileName = filePath;
            meta.SelectedPersons = selectedPersons;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.Time = time;
            meta.PubTime = pubTime;
            TaskExportIdeaBookOperation import = new TaskExportIdeaBookOperation();
            import.Argument = meta;
            import.Description = taskDesc;
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
        /// 批量导出无异议声明书(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="time">日期</param>
        private void ExportIdeaBookTaskGroup(string filePath, string taskDesc = "", string taskName = "",
            DateTime? time = null, DateTime? pubTime = null)
        {
            TaskGroupExportIdeaBookArgument groupArgument = new TaskGroupExportIdeaBookArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.Time = time;
            groupArgument.PubTime = pubTime;
            TaskGroupExportIdeaBookOperation groupOperation = new TaskGroupExportIdeaBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDesc;
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
        /// 预览测绘申请书
        /// </summary>
        public void SurveyPreviewBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreivewNoZone);
                return;
            }
            if (currentItem == null)
            {
                ShowBox(VirtualPersonInfo.PreviewData, VirtualPersonInfo.PreviewDataNo);
                return;
            }
            DateTime? time = DateTime.Now;
            if (FamilyOtherSet.SurveyRequireDate)
            {
                //此时要弹出设置时间对话框
                DateSettingPage page = new DateSettingPage();
                page.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(page, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    time = page.SetTime;
                    Business.SurveyBookWord(currentZone, currentItem.Tag, time);
                });
            }
            else
            {
                //此时使用默认当前时间
                Business.SurveyBookWord(currentZone, currentItem.Tag, time);
            }
        }

        /// <summary>
        /// 导出测绘申请书
        /// </summary>
        public void ExportSurveyBook()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.ExportNoZone);
                return;
            }
            List<Zone> childrenZone = new List<Zone>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //单个任务
                    if (Items == null || Items.Count == 0)
                    {
                        ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.CurrentZoneNoPerson);
                        return;
                    }
                    if (!IsBatch)
                    {
                        if (currentItem == null)
                        {
                            ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.ExportDataNo);
                            return;
                        }
                        //选中项(直接导出当前承包方项)
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, ThePage, VirtualPersonInfo.ExportSurveyBook);
                        ThePage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            DateTime? time = DateTime.Now;   //设置时间
                            bool isSuccess = true;   //导出是否成功
                            if (FamilyOtherSet.SurveyRequireDate)
                            {
                                //此时要弹出设置时间对话框
                                DateSettingPage page = new DateSettingPage();
                                page.Workpage = ThePage;
                                ThePage.Page.ShowMessageBox(page, (c, s) =>
                                {
                                    if (!(bool)c)
                                    {
                                        return;
                                    }
                                    time = page.SetTime;
                                    isSuccess = Business.SurveyBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                    if (isSuccess)
                                        ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                                });
                            }
                            else
                            {
                                //此时使用默认当前时间
                                isSuccess = Business.SurveyBookWord(currentZone, currentItem.Tag, time, true, savePage.FileName);
                                if (isSuccess)
                                    ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.ExportVpDataComplete, eMessageGrade.Infomation);
                            }
                        });
                    }
                    else
                    {
                        //界面上没有选中项(此时弹出承包方选择界面)
                        foreach (var item in Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = ThePage;
                        selectPage.PersonItems = listPerson;
                        selectPage.Business = this.Business;
                        ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.ExportDataNo);
                                return;
                            }
                            ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportSurveyBook, ePersonArgType.ExportSurvey,
                         VirtualPersonInfo.ExportSurveyBookComment, VirtualPersonInfo.ExportSurveyBook, selectedPersons);
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
                {
                    //组任务
                    ExportMeterialCommonOperate(currentZone.FullName, VirtualPersonInfo.ExportSurveyBook, ePersonArgType.ExportSurvey,
                  VirtualPersonInfo.ExportSurveyBookComment, VirtualPersonInfo.ExportSurveyBook, null, true);
                }
                else
                {
                    //选中地域大于镇级
                    ShowBox(VirtualPersonInfo.ExportSurveyBook, VirtualPersonInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSurveyBook(导出测绘申请书)", ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                childrenZone.Clear();
                childrenZone = null;
            }
        }

        /// <summary>
        /// 导出测绘申请书(单个任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="selectedPersons">选择承包方集合</param>
        /// <param name="time">日期</param>
        private void ExportSurveyBookTask(string filePath, List<VirtualPerson> selectedPersons, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskExportSurveyBookArgument meta = new TaskExportSurveyBookArgument();
            meta.FileName = filePath;
            meta.SelectedPersons = selectedPersons;
            meta.DbContext = DbContext;
            meta.CurrentZone = currentZone;
            meta.VirtualType = virtualType;
            meta.FamilyOtherSet = FamilyOtherSet;
            meta.Time = time;
            TaskExportSurveyBookOperation import = new TaskExportSurveyBookOperation();
            import.Argument = meta;
            import.Description = taskDesc;
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
        /// 批量导出测绘申请书(组任务)
        /// </summary>
        /// <param name="filePath">选择保存文件路径</param>
        /// <param name="time">日期</param>
        private void ExportSurveyBookTaskGroup(string filePath, string taskDesc = "", string taskName = "", DateTime? time = null)
        {
            TaskGroupExportSurveyBookArgument groupArgument = new TaskGroupExportSurveyBookArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = DbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.VirtualType = virtualType;
            groupArgument.FamilyOtherSet = FamilyOtherSet;
            groupArgument.Time = time;
            TaskGroupExportSurveyBookOperation groupOperation = new TaskGroupExportSurveyBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDesc;
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
        /// 导出声明申请材料文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="listPerson">承包方集合</param>
        /// <param name="isBatch">是否为批量任务</param>
        private void ExportMeterialCommonOperate(string zoneName, string header, ePersonArgType type, string taskDes, string taskName,
            List<VirtualPerson> listPerson = null, bool isBatch = false)
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
                    case ePersonArgType.ExportApply:
                        ExportMeterialCommonSetTime(FamilyOtherSet.FamilyInstructionDate, type, taskDes, taskName, extPage.FileName, listPerson, isBatch);
                        break;

                    case ePersonArgType.ExportDelegate:
                        ExportMeterialCommonSetTime(FamilyOtherSet.ProxyDefineDate, type, taskDes, taskName, extPage.FileName, listPerson, isBatch);
                        break;

                    case ePersonArgType.ExportIdea:
                        ExportMeterialCommonSetTime(FamilyOtherSet.UniqueInstructionDate, type, taskDes, taskName, extPage.FileName, listPerson, isBatch);
                        break;

                    case ePersonArgType.ExportSurvey:
                        ExportMeterialCommonSetTime(FamilyOtherSet.SurveyRequireDate, type, taskDes, taskName, extPage.FileName, listPerson, isBatch);
                        break;
                }
            });
        }

        /// <summary>
        /// 批量导出有关日期设置公共处理类
        /// </summary>
        /// <param name="familySetting">配置文件</param>
        /// <param name="type">承包方数据参数类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="fileName">批量导出保存路径</param>
        /// <param name="listPerson">承包方集合</param>
        /// <param name="isBatch">是否为批量任务</param>
        private void ExportMeterialCommonSetTime(bool familySetting, ePersonArgType type, string taskDes, string taskName,
            string fileName = "", List<VirtualPerson> listPerson = null, bool isBatch = false)
        {
            DateTime? time = DateTime.Now;
            DateTime? decTime = DateTime.Now;
            DateTime? pubTime = DateTime.Now;
            if (type != ePersonArgType.ExportIdea)
            {
                if (familySetting)
                {
                    //此时要设置导出日期
                    DateSettingPage page = new DateSettingPage();
                    page.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(page, (m, t) =>
                    {
                        if (!(bool)m)
                        {
                            return;
                        }
                        time = page.SetTime;
                        if (isBatch)
                            ExportMeterialCommonTaskGroup(type, taskDes, taskName, fileName, time, null, listPerson);
                        else
                            ExportMeterialCommonTask(type, taskDes, taskName, fileName, time, null, listPerson);
                    });
                }
                else
                {
                    //此时为当前默认日期
                    if (isBatch)
                        ExportMeterialCommonTaskGroup(type, taskDes, taskName, fileName, time, null, listPerson);
                    else
                        ExportMeterialCommonTask(type, taskDes, taskName, fileName, time, null, listPerson);
                }
            }
            else
            {
                if (familySetting)
                {
                    //此时要设置导出日期(公示日期和声明日期)
                    DoubleDateSettingPage page = new DoubleDateSettingPage();
                    page.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(page, (o, w) =>
                    {
                        if (!(bool)o)
                        {
                            return;
                        }
                        decTime = page.SetDecTime;
                        pubTime = page.SetPubTime;
                        if (isBatch)
                            ExportMeterialCommonTaskGroup(type, taskDes, taskName, fileName, decTime, pubTime, listPerson);
                        else
                            ExportMeterialCommonTask(type, taskDes, taskName, fileName, decTime, pubTime, listPerson);
                    });
                }
                else
                {
                    //此时为当前默认日期
                    if (isBatch)
                        ExportMeterialCommonTaskGroup(type, taskDes, taskName, fileName, decTime, pubTime, listPerson);
                    else
                        ExportMeterialCommonTask(type, taskDes, taskName, fileName, decTime, pubTime, listPerson);
                }
            }
        }

        /// <summary>
        /// 导出文件操作(单个任务)
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="filePath">批量导出保存路径</param>
        /// <param name="time">日期</param>
        /// <param name="pubTime">日期</param>
        /// <param name="listPerson">承包方集合</param>
        private void ExportMeterialCommonTask(ePersonArgType type, string taskDes, string taskName,
            string filePath = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            switch (type)
            {
                case ePersonArgType.ExportApply:
                    ExportApplyBookTask(filePath, listPerson, taskDes, taskName, time);
                    break;

                case ePersonArgType.ExportDelegate:
                    ExportDelegateBookTask(filePath, listPerson, taskDes, taskName, time);
                    break;

                case ePersonArgType.ExportIdea:
                    ExportIdeaBookTask(filePath, listPerson, taskDes, taskName, time, pubTime);
                    break;

                case ePersonArgType.ExportSurvey:
                    ExportSurveyBookTask(filePath, listPerson, taskDes, taskName, time);
                    break;
            }
        }

        /// <summary>
        /// 批量导出文件操作(组任务)
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="filePath">批量导出保存路径</param>
        /// <param name="time">日期</param>
        /// <param name="pubTime">日期</param>
        /// <param name="listPerson">承包方集合</param>
        private void ExportMeterialCommonTaskGroup(ePersonArgType type, string taskDes, string taskName,
            string filePath = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            switch (type)
            {
                case ePersonArgType.ExportApply:
                    ExportApplyBookTaskGroup(filePath, taskDes, taskName, time);
                    break;

                case ePersonArgType.ExportDelegate:
                    ExportDelegateBookTaskGroup(filePath, taskDes, taskName, time);
                    break;

                case ePersonArgType.ExportIdea:
                    ExportIdeaBookTaskGroup(filePath, taskDes, taskName, time, pubTime);
                    break;

                case ePersonArgType.ExportSurvey:
                    ExportSurveyBookTaskGroup(filePath, taskDes, taskName, time);
                    break;
            }
        }

        #endregion 导出声明申请材料

        #endregion 导入导出

        #endregion Methods - Public

        #region Methods - Private

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(string zoneName, string header, ePersonArgType type, string taskDes, string taskName, string messageName = "", List<VirtualPerson> listPerson = null)
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
                    case ePersonArgType.ExportExcel:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
                        break;

                    case ePersonArgType.ExportWord:
                        ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName, null, null, listPerson);
                        break;

                    case ePersonArgType.ExportApply:
                        CommonBatchExport(FamilyOtherSet.FamilyInstructionDate, type, taskDes, taskName, extPage.FileName, messageName, listPerson);
                        break;

                    case ePersonArgType.ExportDelegate:
                        CommonBatchExport(FamilyOtherSet.ProxyDefineDate, type, taskDes, taskName, extPage.FileName, messageName, listPerson);
                        break;

                    case ePersonArgType.ExportIdea:
                        CommonBatchExport(FamilyOtherSet.UniqueInstructionDate, type, taskDes, taskName, extPage.FileName, messageName, listPerson);
                        break;

                    case ePersonArgType.ExportSurvey:
                        CommonBatchExport(FamilyOtherSet.SurveyRequireDate, type, taskDes, taskName, extPage.FileName, messageName, listPerson);
                        break;
                }
            });
        }

        /// <summary>
        /// 公共批量导出有关日期设置处理类
        /// </summary>
        /// <param name="familySetting">配置文件</param>
        /// <param name="type">承包方数据参数类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="fileName">批量导出保存路径</param>
        /// <param name="messageName">消息内容</param>
        private void CommonBatchExport(bool familySetting, ePersonArgType type, string taskDes, string taskName,
            string fileName = "", string messageName = "", List<VirtualPerson> listPerson = null)
        {
            DateTime? time = DateTime.Now;
            DateTime? decTime = DateTime.Now;
            DateTime? pubTime = DateTime.Now;
            if (type != ePersonArgType.ExportIdea)
            {
                if (familySetting)
                {
                    //此时要设置导出日期
                    DateSettingPage page = new DateSettingPage();
                    page.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(page, (m, t) =>
                    {
                        if (!(bool)m)
                        {
                            return;
                        }
                        time = page.SetTime;
                        ExportCommonOperate(type, taskDes, taskName, fileName, messageName, time, null, listPerson);
                    });
                }
                else
                {
                    //此时为当前默认日期
                    ExportCommonOperate(type, taskDes, taskName, fileName, messageName, time, null, listPerson);
                }
            }
            else
            {
                if (familySetting)
                {
                    //此时要设置导出日期(公示日期和声明日期)
                    DoubleDateSettingPage page = new DoubleDateSettingPage();
                    page.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(page, (o, w) =>
                    {
                        if (!(bool)o)
                        {
                            return;
                        }
                        decTime = page.SetDecTime;
                        pubTime = page.SetPubTime;
                        ExportCommonOperate(type, taskDes, taskName, fileName, messageName, decTime, pubTime, listPerson);
                    });
                }
                else
                {
                    //此时为当前默认日期
                    ExportCommonOperate(type, taskDes, taskName, fileName, messageName, decTime, pubTime, listPerson);
                }
            }
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(ePersonArgType type, string taskDes, string taskName,
            string filePath = "", string messageName = "", DateTime? time = null, DateTime? pubTime = null, List<VirtualPerson> listPerson = null)
        {
            IDbContext dbContext = CreateDb();
            TaskVirtualPersonArgument meta = new TaskVirtualPersonArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.virtualType = virtualType;
            meta.DateValue = time;
            meta.PubDateValue = pubTime;
            TaskVirtualPersonOperation import = new TaskVirtualPersonOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.FamilyOutputSet = FamilyOutputSet;
            import.FamilyOtherSet = FamilyOtherSet;
            import.SelectedPersons = listPerson;
            import.IsBatch = IsBatch;
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
        /// 更新锁定状态
        /// </summary>
        private List<VirtualPerson> UpdateLock(List<VirtualPersonItem> obCollection)
        {
            List<VirtualPerson> list = new List<VirtualPerson>();
            foreach (var item in Items)
            {
                VirtualPersonItem obItem = obCollection.Where(t => t.ID == item.ID).FirstOrDefault();
                if (obItem != null && obItem.Status == item.Status)
                    continue;
                item.Status = obItem.Status;
                item.Tag.Status = obItem.Status;
                item.Name = VirtualPersonItemHelper.CreateItemName(item.Tag.Name, item.Children.Count, item.FamilyNumber, item.Status);
                item.Img = (item.Status == eVirtualPersonStatus.Lock) ? eImage.Lock : eImage.Family;
                list.Add(item.Tag);
            }
            obCollection = null;
            GC.Collect();
            return list;
        }

        /// <summary>
        /// 更新合并状态
        /// </summary>
        private void UpdateCombine(VirtualPersonItem desItem, VirtualPersonItem srcItem)
        {
            if (desItem == null || srcItem == null)
            {
                return;
            }
            Items.Remove(srcItem);
            List<Person> personList = desItem.Tag.SharePersonList;
            foreach (var item in personList)
            {
                BindPerson bp = desItem.Children.Where(t => t.ID == item.ID).FirstOrDefault();
                if (bp == null)
                {
                    desItem.Children.Add(new BindPerson(item));
                }
            }
            desItem.Name = VirtualPersonItemHelper.CreateItemName(desItem.Tag.Name, desItem.Children.Count, desItem.FamilyNumber, desItem.Status);
        }

        /// <summary>
        /// 将承包方更新到到集合中
        /// </summary>
        /// <param name="vp">承包方</param>
        /// <param name="isNew">是否新增</param>
        private void UpdateItems(VirtualPerson vp, bool isNew = true)
        {
            if (vp == null)
            {
                return;
            }
            if (isNew)
            {
                var item = VirtualPersonItemHelper.ConvertToItem(vp);
                item.ContractorNumber = vp.PersonCount;
                Items.Add(item);
            }
            else
            {
                int index = Items.IndexOf(currentItem);
                Items.Remove(currentItem);
                var item = VirtualPersonItemHelper.ConvertToItem(vp);
                item.ContractorNumber = vp.PersonCount;
                Items.Insert(index, item);

                view.BringIntoView(item);
            }
        }

        /// <summary>
        /// 将承包方项
        /// </summary>
        private void UpdateVpItem(Person person)
        {
            if (person == null)
            {
                return;
            }
            //VirtualPerson virtulPerson = currentItem.Tag;
            //List<Person> list = virtualPerson.SharePersonList;
            //Person p = list.Find(t => t.ID == person.ID);
            //int index = list.FindIndex(t => t.ID == person.ID);
            //list.Remove(p);
            //list.Insert(index, person);
            //virtualPerson.SharePersonList = list;
            if (person.ID == currentItem.ID)
            {
                currentItem.HouseHolderName = person.Name;
                currentItem.Name = VirtualPersonItemHelper.CreateItemName(currentItem.Tag.Name, currentItem.Children.Count, currentItem.FamilyNumber, currentItem.Status);
                currentItem.ICN = person.ICN;
            }
            BindPerson bdPerson = currentItem.Children.Where(t => t.ID == person.ID).FirstOrDefault();
            if (bdPerson != null)
            {
                bdPerson.Tag = person;
                bdPerson.Name = person.Name;
                bdPerson.ICN = person.ICN;
                bdPerson.Gender = person.Gender;
                bdPerson.Age = person.Age;
                bdPerson.Comment = person.Comment;
                bdPerson.Relationship = person.Relationship;
                bdPerson.Img = VirtualPersonItemHelper.ChangeByGender(person.Gender);
            }
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
        /// 刷新按钮事件
        /// </summary>
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            txtWhere.Text = string.Empty;
            Items.Clear();
            Refresh();
        }

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectItem();
            if (currentItem == null)
            {
                return;
            }
            VirtualPerson vp = currentItem.Tag;
            if (vp.FamilyExpand != null)
            {
                if (vp.FamilyExpand.ContractorType != eContractorType.Farmer)
                {
                    miAddSharePerson.IsEnabled = false;
                }
                else
                {
                    miAddSharePerson.IsEnabled = true;
                }
            }
            else
            {
                miAddSharePerson.IsEnabled = true;
            }
        }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetSelectItem();
            if (currentItem == null && currentPerson == null)
            {
                return;
            }
            EditPerson();
        }

        #endregion 按键功能

        #region 右键菜单

        /// <summary>
        /// 添加承包方
        /// </summary>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            AddVirtualPerson();
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private void miAddSharePerson_Click(object sender, RoutedEventArgs e)
        {
            AddSharePerson();
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void miDel_Click(object sender, RoutedEventArgs e)
        {
            DelPerson();
        }

        /// <summary>
        /// 设置承包方
        /// </summary>
        private void miSet_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonSet();
        }

        /// <summary>
        /// 锁定
        /// </summary>
        private void miLock_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonLock();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void miInitall_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonInitialize();
        }

        /// <summary>
        /// 分户
        /// </summary>
        private void miSplit_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonSplit();
        }

        /// <summary>
        /// 合并
        /// </summary>
        private void miCombine_Click(object sender, RoutedEventArgs e)
        {
            VirtualPersonCombine();
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void miClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            EditPerson();
        }

        /// <summary>
        /// 数据导入配置
        /// </summary>
        private void miDataImport_Click(object sender, RoutedEventArgs e)
        {
            CommonConfigPage personPage = new CommonConfigPage(ThePage);    //要显示的界面
            personPage.Header = "承包方导入配置";
            var propertyCount = typeof(FamilyImportDefine).GetProperties().Count();
            personPage.ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源
            var profile = ThePage.Workspace.GetUserProfile();  //获取当前用户profile
            var section = profile.GetSection<FamilyImportDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var config = (section.Settings as FamilyImportDefine);   //得到经反序列化后的对象
            var configCopy = config.Clone();
            personPage.ProGrid.Object = configCopy;

            //显示导入数据配置信息界面  b代表确定按钮
            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                else
                {
                    config.CopyPropertiesFrom(configCopy);  //确定保存配置后的信息
                }
            });
        }

        /// <summary>
        /// 承包方内容导出配置
        /// </summary>
        private void miDataOutput_Click(object sender, RoutedEventArgs e)
        {
            CommonConfigPage personPage = new CommonConfigPage(ThePage);
            personPage.Header = "承包方导出表格内容配置";
            var profile = ThePage.Workspace.GetUserProfile();
            var section = profile.GetSection<FamilyOutputDefine>();
            var config = (section.Settings as FamilyOutputDefine);
            var configCopy = config.Clone();
            personPage.ProGrid.Object = configCopy;

            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                else
                {
                    config.CopyPropertiesFrom(configCopy);
                }
            });
        }

        /// <summary>
        /// 右键户号详情
        /// </summary>
        private void miNumber_Click(object sender, RoutedEventArgs e)
        {
            int maxNumber = 0;   //最大编号
            int minNumber = 0;   //最小编号
            List<int> missNumbers = new List<int>();   //缺失编号
            List<int> familyNumberList = new List<int>();       //当前地域下的所有编号
            foreach (var item in Items)
            {
                int numberInt = 0;
                int.TryParse(item.FamilyNumber, out numberInt);
                familyNumberList.Add(numberInt);
            }
            if (familyNumberList == null || familyNumberList.Count() == 0)
            {
                ShowBox(VirtualPersonInfo.FamilyNumber, VirtualPersonInfo.CurrentZoneNoPerson);
                return;
            }
            else
            {
                maxNumber = familyNumberList.Max<int>();   //最大编号
                minNumber = familyNumberList.Min<int>();  //最小编号
                for (int num = minNumber; num <= maxNumber; num++)
                {
                    if (!familyNumberList.Contains(num))
                    {
                        missNumbers.Add(num);
                    }
                }
            }
            FamilyNumberDetail numberDetailDlg = new FamilyNumberDetail(maxNumber, minNumber, missNumbers);
            numberDetailDlg.Workpage = ThePage;
            ThePage.Page.ShowDialog(numberDetailDlg);
        }

        #endregion 右键菜单

        #region 辅助功能

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
        /// 删除承包方所有信息
        /// </summary>
        private void DeleteVirtualPerson(ContainerFactory factory, List<VirtualPerson> lockfamilys)
        {
            IVirtualPersonWorkStation<LandVirtualPerson> vpStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
            var familys = vpStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (familys == null) return;
            foreach (VirtualPerson vp in lockfamilys)
            {
                familys.Remove(familys.Find(fam => fam.ID == vp.ID));
            }
            IContractLandWorkStation contractLandWorkStation = this.DbContext.CreateContractLandWorkstation();//承包台账地块业务逻辑层
            IConcordWorkStation concordStation = this.DbContext.CreateConcordStation();
            IContractRegeditBookWorkStation contractRegeditBookStation = this.DbContext.CreateRegeditBookStation();
            IBuildLandBoundaryAddressCoilWorkStation jzxStation = this.DbContext.CreateBoundaryAddressCoilWorkStation();
            IBuildLandBoundaryAddressDotWorkStation jzdStation = this.DbContext.CreateBoundaryAddressDotWorkStation();
            ISecondTableLandWorkStation secondtableLandStation = this.DbContext.CreateSecondTableLandWorkstation();

            var vpguids = familys.Select(f => f.ID).ToList();
            var listLands = contractLandWorkStation.GetCollection(vpguids);
            var landIds = listLands.Select(c => c.ID).ToArray();
            jzdStation.DeleteByLandIds(landIds);
            jzxStation.DeleteByLandIds(landIds);
            concordStation.DeleteByOwnerIds(vpguids);

            foreach (VirtualPerson vp in familys)
            {
                secondtableLandStation.DeleteLandByPersonID(vp.ID);
                contractLandWorkStation.DeleteLandByPersonID(vp.ID);
                var concords = concordStation.GetContractsByFamilyID(vp.ID);
                if (concords != null && concords.Count > 0)
                {
                    foreach (var item in concords)
                    {
                        var regbook = contractRegeditBookStation.Get(item.ID);
                        if (regbook != null)
                        {
                            contractRegeditBookStation.Delete(regbook.ID);
                        }
                    }
                }
                vpStation.Delete(vp.ID);
            }
            familys.Clear();
        }

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
                    value = YuLinTu.Library.Business.ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetHouselderStatementDate);
                    break;

                case 2:
                    value = YuLinTu.Library.Business.ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetProxyStatementDate);
                    break;

                case 3:
                    value = YuLinTu.Library.Business.ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetPulicliyStatementDate);
                    break;

                case 4:
                    value = YuLinTu.Library.Business.ToolConfiguration.GetAppSettingValue(AgricultureSetting.SetMeasureRequireDate);
                    break;
            }
            Boolean.TryParse(value, out setData);
            DateTime? time = DateTime.Now;
            if (FamilyOtherSet.FamilyInstructionDate)
            {
                DateSettingPage page = new DateSettingPage();
                ThePage.Page.ShowMessageBox(page, (b, e) =>
                {
                    if (!(bool)b)
                        return;
                    time = page.SetTime;
                });
            }
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
        public VirtualPersonItem GetSelectItem()
        {
            currentItem = null;
            currentPerson = null;
            var item = view.SelectedItem;
            if (item is BindPerson)
            {
                BindPerson bp = item as BindPerson;
                currentPerson = bp.Tag;
                currentItem = Items.FirstOrDefault(t => t.ID == bp.FamilyID);
            }
            if (item is VirtualPersonItem)
            {
                currentItem = view.SelectedItem as VirtualPersonItem;
            }
            return currentItem;
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null,
            bool isConfirmVisibility = true, bool isCancelVisibility = true)
        {
            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
                ConfirmButtonVisibility = isConfirmVisibility ? Visibility.Visible : Visibility.Collapsed,
                CancelButtonVisibility = isCancelVisibility ? Visibility.Visible : Visibility.Collapsed
            }, action);
        }

        /// <summary>
        /// 是否继续操作
        /// </summary>
        /// <returns></returns>
        private bool CanContinue(string tip, bool equal = false)
        {
            if (currentZone == null)
            {
                ShowBox(tip, "当前行政区域无效!");
                return false;
            }
            var vpstation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            bool flag = vpstation.Any(c => c.ZoneCode.StartsWith(CurrentZone.FullCode));
            //int count = database.LandVirtualPerson.SL_Count("SenderCode", currentZone.FullCode, (equal ? Library.Data.ConditionOption.Equal : Library.Data.ConditionOption.Like_LeftFixed));
            if (!flag)
            {
                ShowBox(tip, "当前行政区域下没有数据可供操作!");
                //MessageBox.Show("当前行政区域下没有数据可供操作!", tip);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 当前模块
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
            miLock.Visibility = Visibility.Collapsed;
        }

        #endregion 辅助功能

        #endregion Methods - Events

        #endregion Methods
    }
}