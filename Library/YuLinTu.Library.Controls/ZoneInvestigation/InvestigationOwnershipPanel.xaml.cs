/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Windows;
using System.Collections.ObjectModel;
using YuLinTu.Library.Entity;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Repository;
using YuLinTu.Library.Business;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Data;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方基本管理
    /// </summary>
    public partial class InvestigationOwnershipPanel : UserControl
    {
        #region Fields

        private IWorkpage theworkpage; //控制空间 
        private InvestigationPersonControlPanel pcpcontrol;   //控件
        private Zone currentZone;   //当前地域
        private VirtualPerson currentVritualPerson;  //当前承包方
        private TaskQueue queueQuery;//获取数据       
        private VirtualPersonBusiness business;
        private VirtualPerson tempVirtualPerson;  //承包方;

        private AccountLandBusiness landBusiness;
        private IContractLandWorkStation landStation;
        private InvestigationLandPanel landPanelControl;
        private List<ContractLand> listLand;
        //private bool isCurrentVpChange;   //记录当前承包方是否改变

        private VectorLayer landLayer;

        private IDbContext dbContext;

        #endregion

        #region Properties

        public NavigateItem navigateItem { get; private set; }
        /// <summary>
        /// 当前土地地块图层
        /// </summary>
        //public VectorLayer LandLayer
        //{
        //    get { return landLayer; }
        //    set { landLayer = value; }
        //}

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage Theworkpage
        {
            get { return theworkpage; }
            set { theworkpage = value; panelLand.theworkpage = value; }
        }

        /// <summary>
        /// 控件包括 对承包方的基本管理
        /// </summary>
        public InvestigationPersonControlPanel Pcpcontrol
        {
            get { return pcpcontrol; }
            set { pcpcontrol = value; SetControl(); }
        }

        /// <summary>
        /// 地块控制面板
        /// </summary>
        public InvestigationLandPanel LandPanelControl
        {
            get { return landPanelControl; }
            set
            {
                landPanelControl = value;
                SetLandControl();
            }
        }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public ObservableCollection<VirtualPerson> VirtualPersonItems { get; private set; }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentVirtualPerson
        {
            get { return currentVritualPerson; }
            private set
            {
                currentVritualPerson = value;
                SetEntity(value);
            }
        }

        /// <summary>
        /// 承包方数据业务
        /// </summary>
        public VirtualPersonBusiness Business
        {
            get { return business; }
            set { business = value; }
        }

        /// <summary>
        /// 承包地数据业务
        /// </summary>
        public AccountLandBusiness LandBusiness
        {
            get { return landBusiness; }
            set { landBusiness = value; }
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
        /// 地图控件
        /// </summary>
        public MapControl CurrentMapControl { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public InvestigationOwnershipPanel()
        {
            InitializeComponent();
            InitializeDataBase();
            SetVisibility(false);
            pcpcontrol = new InvestigationPersonControlPanel();
            landPanelControl = new InvestigationLandPanel();
            DataContext = this;
            tempVirtualPerson = null;
            VirtualPersonItems = new ObservableCollection<VirtualPerson>();
            listLand = new List<ContractLand>();
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            panelSharePerson.delSharePerson += DelSharPerson;
            panelSharePerson.editSharePerson += EditSharPerson;
            panelSharePerson.addSharePerson += AddSharPeron;
            panelLand.AddCtLand += AddCLand;
            panelLand.EditCtLand += EditCLand;
            panelLand.DelCtLand += DelCLand;
            panelLand.SearchLandSet += Search;
        }

        #endregion

        #region Methods - 获取数据

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public void InitializeDataBase()
        {
            dbContext = DataBaseSourceWork.GetDataBaseSource();
            business = new VirtualPersonBusiness(dbContext);
            business.VirtualType = eVirtualType.Land;
            landBusiness = new AccountLandBusiness(dbContext);
            landStation = dbContext.CreateContractLandWorkstation();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InlitialControl(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return;
            }
            queueQuery.Cancel();
            queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    DoWork(go);
                },
                completed =>
                {
                    if (VirtualPersonItems.Count > 0 && tempVirtualPerson == null)
                    {
                        CurrentVirtualPerson = VirtualPersonItems[0];
                        tempVirtualPerson = VirtualPersonItems[0];
                        //List<ContractLand> listLand = landBusiness.GetPersonCollection(CurrentVirtualPerson.ID);
                    }
                    else
                    {
                        foreach (var item in VirtualPersonItems)
                        {
                            if (tempVirtualPerson.ID == item.ID)
                            {
                                CurrentVirtualPerson = item;
                                break;
                            }
                        }
                    }
                    if (CurrentVirtualPerson != null && CurrentVirtualPerson.Status == eVirtualPersonStatus.Lock)
                    {
                        pcpcontrol.vpUpdateBtn.IsEnabled = false;
                        pcpcontrol.vpDelBtn.IsEnabled = false;

                        panelSharePerson.sharePersonDelBtn.IsEnabled = false;
                        panelSharePerson.sharePersonEditBtn.IsEnabled = false;
                        panelSharePerson.sharePersonAddBtn.IsEnabled = false;

                        panelLand.landAddBtn.IsEnabled = false;
                        panelLand.landUpdateBtn.IsEnabled = false;
                        panelLand.landDelBtn.IsEnabled = false;
                    }
                    else if (CurrentVirtualPerson != null && CurrentVirtualPerson.Status != eVirtualPersonStatus.Lock)
                    {
                        pcpcontrol.vpUpdateBtn.IsEnabled = true;
                        pcpcontrol.vpDelBtn.IsEnabled = true;

                        panelSharePerson.sharePersonDelBtn.IsEnabled = true;
                        panelSharePerson.sharePersonEditBtn.IsEnabled = true;
                        panelSharePerson.sharePersonAddBtn.IsEnabled = true;

                        panelLand.landAddBtn.IsEnabled = true;
                        panelLand.landUpdateBtn.IsEnabled = true;
                        panelLand.landDelBtn.IsEnabled = true;
                    }

                },
                terminated =>
                {
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    Clear();
                }, null, null, null, null);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void DoWork(TaskGoEventArgs arg)
        {
            if (Business == null)
            {
                return;
            }
            string zoneCode = currentZone.FullCode;
            List<VirtualPerson> vps = Business.GetByZone(zoneCode);
            //vps.Sort();
            if (vps != null && vps.Count > 0)
            {
                //vps.RemoveAll(v => v.Status == eVirtualPersonStatus.Lock);//去除锁定状态下的用户
                foreach (var person in vps)
                {
                    if (arg.Instance.IsStopPending)
                        break;
                    arg.Instance.ReportProgress(5, person);
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    SetVisibility(true);
                }));
            }
            else
            {
                if (VirtualPersonItems.Count == 0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        SetVisibility(false);
                    }));
                }
            }
        }

        /// <summary>
        /// 获取数据完成
        /// </summary>
        private void Changed(TaskProgressChangedEventArgs e)
        {
            if (e.Percent == 0)
            {
                SetVisibility(false);
            }
            else
            {
                VirtualPerson item = e.UserState as VirtualPerson;
                VirtualPersonItems.Add(item);
            }
        }

        #endregion

        #region Methods - Privates

        /// <summary>
        /// 设计可见性，当数据没有时，不显示控件
        /// </summary>
        private void SetVisibility(bool show)
        {
            if (show)
            {
                if (VirtualPersonItems.Count == 0)
                {
                    txtNotFound.Visibility = Visibility.Visible;
                    grid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtNotFound.Visibility = Visibility.Collapsed;
                    grid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtNotFound.Visibility = Visibility.Visible;
                grid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 绑定控件(承包方)
        /// </summary>
        private void SetControl()
        {
            pcpcontrol.SelectVpClick += SelectVirtualPerson;
            pcpcontrol.AddVpClick += AddVirtualPerson;
            pcpcontrol.EditVPClick += UpDatePerson;
            pcpcontrol.DelVPClick += DelVirtualPerson;
            pcpcontrol.ManageVpClick += ManVirtualPerson;
            pcpcontrol.MapVPClick += CheckVirtualPersonMap;
        }

        /// <summary>
        /// 绑定控件(承包地块)
        /// </summary>
        private void SetLandControl()
        {
            landPanelControl.AddLandClick += AddContractLand;
            landPanelControl.EditLandClick += EditContractLand;
            landPanelControl.DelLandClick += DelContractLand;
        }

        /// <summary>
        /// 查看承包空间地块信息
        /// </summary>
        private void CheckVirtualPersonMap(object sender, RoutedEventArgs e)
        {
            if (currentZone == null)
            {
                return;
            }
            if (CurrentVirtualPerson == null)
            {
                ShowBox("承包方地块查看", "无承包方");
                return;
            }
            if (listLand == null)
            {
                ShowBox("承包方地块查看", "承包方下没有地块");
                return;
            }

            CurrentMapControl.SelectedItems.Clear();
            List<YuLinTu.tGIS.FeatureObject> featureObject = new List<FeatureObject>();
            Envelope env = null;
            string queryCondition = string.Format("{0} = \"{1}\"",
             typeof(ContractLand).GetProperty("OwnerId").GetAttribute<DataColumnAttribute>().ColumnName, CurrentVirtualPerson.ID);
            landLayer = new VectorLayer(new SQLiteGeoSource(dbContext.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon });

            if (landLayer == null)
                return;
            featureObject = landLayer.DataSource.Get(queryCondition, int.MaxValue);

            if (featureObject == null || featureObject.Count == 0)
            {
                return;
            }
            foreach (var item in featureObject)
            {
                var g = new Graphic();
                g.Object = item;
                g.Layer = landLayer;

                if (item.Geometry != null)
                {
                    if (env == null)
                    {
                        env = item.Geometry.GetEnvelope();
                    }
                    if (env != null)
                    {
                        CurrentMapControl.SelectedItems.Add(g);
                        env.Union(item.Geometry.GetEnvelope());
                    }
                }
            }
            if (env == null) return;
            CurrentMapControl.ZoomTo(env);
        }

        /// <summary>
        /// 承包方名称按钮
        /// </summary>
        private void SelectVirtualPerson(object sender, RoutedEventArgs e)
        {
            if (currentZone == null)
            {
                return;
            }
            pcpcontrol.vpShowBtn.IsEnabled = false;
            var vpc = new VirtualPesonSelectPanel();
            vpc.theWorkpage = theworkpage;
            vpc.CheckPersons = VirtualPersonItems;
            vpc.CurrenZone = currentZone;
            theworkpage.Page.ShowMessageBox(vpc, (b, r) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    pcpcontrol.vpShowBtn.IsEnabled = true;
                }));
                if (b == false)
                    return;
                if (vpc.CurrentPerson == null) return;
                CurrentVirtualPerson = vpc.CurrentPerson;
                tempVirtualPerson = vpc.CurrentPerson;
                if (vpc.CurrentPerson.FamilyExpand != null)
                {
                    if (vpc.CurrentPerson.FamilyExpand.ContractorType != eContractorType.Farmer)
                    {
                        panelSharePerson.sharePersonAddBtn.IsEnabled = false;
                    }
                    else
                    {
                        panelSharePerson.sharePersonAddBtn.IsEnabled = true;
                    }
                }
                else
                {
                    panelSharePerson.sharePersonAddBtn.IsEnabled = true;
                }
                if (vpc.CurrentPerson.Status == eVirtualPersonStatus.Lock)
                {
                    pcpcontrol.vpUpdateBtn.IsEnabled = false;
                    pcpcontrol.vpDelBtn.IsEnabled = false;

                    panelSharePerson.sharePersonDelBtn.IsEnabled = false;
                    panelSharePerson.sharePersonEditBtn.IsEnabled = false;
                    panelSharePerson.sharePersonAddBtn.IsEnabled = false;

                    panelLand.landAddBtn.IsEnabled = false;
                    panelLand.landUpdateBtn.IsEnabled = false;
                    panelLand.landDelBtn.IsEnabled = false;
                }
                else
                {
                    pcpcontrol.vpUpdateBtn.IsEnabled = true;
                    pcpcontrol.vpDelBtn.IsEnabled = true;

                    panelSharePerson.sharePersonDelBtn.IsEnabled = true;
                    panelSharePerson.sharePersonEditBtn.IsEnabled = true;
                    //panelSharePerson.sharePersonAddBtn.IsEnabled = true;

                    panelLand.landAddBtn.IsEnabled = true;
                    panelLand.landUpdateBtn.IsEnabled = true;
                    panelLand.landDelBtn.IsEnabled = true;
                }

            });
        }

        /// <summary>
        /// 承包方管理
        /// </summary>
        public void ManVirtualPerson(object sender, RoutedEventArgs e)
        {
            if (currentZone == null)
            {
                return;
            }
            InvestigationVirtualPersonPage personPage = new InvestigationVirtualPersonPage();
            personPage.CurrentZone = currentZone;
            personPage.ThePage = theworkpage;
            theworkpage.Page.ShowMessageBox(personPage, (b, r) =>
            {
            });
        }

        /// <summary>
        /// 添加承包方
        /// </summary>
        private void AddVirtualPerson(object sender, RoutedEventArgs e)
        {
            AddVperson();
        }

        /// <summary>
        /// 添加承包方
        /// </summary>
        private void AddVperson()
        {
            if (currentZone == null)
            {
                ShowBox("承包方处理", "请在根级地域下操作！");
                return;
            }
            if (currentZone.Level != eZoneLevel.Village && currentZone.Level != eZoneLevel.Group)
            {
                ShowBox("承包方处理", "请在村、组级地域下添加承包方！");
                return;
            }
            VirtualPerson vp = VirtualPersonItemHelper.CreateVirtualPerson(currentZone, eContractorType.Farmer);
            List<VirtualPerson> vpList = VirtualPersonItems.ToList();
            VirtualPersonInfoPage personPage = new VirtualPersonInfoPage(vp, currentZone, Business, true);
            personPage.Workpage = theworkpage;
            personPage.Items = vpList;
            personPage.Header = "添加承包方";
            personPage.OtherDefine = FamilyOtherSet;
            theworkpage.Page.ShowMessageBox(personPage, (b, r) =>
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
                theworkpage.Workspace.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_ADD_COMPLATE, contractor));
                UpdateItems(contractor);
                if (1 == VirtualPersonItems.Count)
                {
                    Refresh();
                }
            });
        }

        /// <summary>
        /// 编辑承包方
        /// </summary>
        private void UpDatePerson(object sender, RoutedEventArgs e)
        {
            UpdateVirtualPerson();
        }

        /// <summary>
        /// 编辑承包方
        /// </summary>
        private void UpdateVirtualPerson()
        {
            if (currentZone == null)
            {
                ShowBox("承包方处理", "请在根级地域下操作！");
                return;
            }
            if (CurrentVirtualPerson == null)
            {
                ShowBox(VirtualPersonInfo.EditVirtualPerson, VirtualPersonInfo.EditShareNoData);
                return;
            }
            VirtualPersonInfoPage personPage = new VirtualPersonInfoPage(currentVritualPerson, currentZone, Business);
            personPage.Items = VirtualPersonItems.ToList();
            personPage.Workpage = theworkpage;
            personPage.Header = "编辑承包方";
            personPage.OtherDefine = FamilyOtherSet;
            theworkpage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (!personPage.Result)
                {
                    ShowBox(VirtualPersonInfo.EditVirtualPerson, VirtualPersonInfo.EditVPFail);
                    return;
                }
                VirtualPerson contractor = personPage.Contractor;
                UpdateItems(contractor, false);
            });
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        private void DelVirtualPerson(object sender, RoutedEventArgs e)
        {
            DeleteVirtualPerson();
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        private void DeleteVirtualPerson()
        {
            if (currentZone == null)
            {
                ShowBox("承包方处理", "请在根级地域下操作！");
                return;
            }
            if (CurrentVirtualPerson == null)
            {
                ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelShareNoData);
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    var landStation = dbContext.CreateContractLandWorkstation();
                    landStation.DeleteSelectVirtualPersonAllData(currentVritualPerson.ID);

                    VirtualPersonItems.Remove(currentVritualPerson);
                    CurrentVirtualPerson = null;
                    if (VirtualPersonItems.Count > 0)
                    {
                        CurrentVirtualPerson = VirtualPersonItems[0];
                        tempVirtualPerson = CurrentVirtualPerson;
                    }
                    if (VirtualPersonItems.Count == 0)
                    {
                        tempVirtualPerson = null;
                        Refresh();
                    }
                    CurrentMapControl.Refresh();

                    ModuleMsgArgs args = MessageExtend.VirtualPersonMsg(CreateDb(), VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE, currentVritualPerson);
                    theworkpage.Workspace.Message.Send(this, args);
                    TheBns.Current.Message.Send(this, args);
                }
                catch (Exception ex)
                {
                    ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonFail);
                    YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(VirtualPersonInfo.DelVirtualPerson, VirtualPersonInfo.DelVPersonWarring, eMessageGrade.Warn, action);
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private void AddSharPeron()
        {
            if (CurrentVirtualPerson == null || currentZone == null)
            {
                ShowBox("共有人处理", "无承包方，不能添加共有人！");
                return;
            }
            List<VirtualPerson> vpList = VirtualPersonItems.ToList();
            Person p = VirtualPersonItemHelper.CreatePerson(CurrentVirtualPerson);
            PersonInfoPage personPage = new PersonInfoPage(CurrentVirtualPerson, true);
            personPage.Business = Business;
            personPage.Person = p;
            personPage.PersonItems = vpList;
            personPage.Workpage = theworkpage;
            personPage.OtherDefine = FamilyOtherSet;
            theworkpage.Page.ShowMessageBox(personPage, (b, r) =>
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
                CurrentVirtualPerson = personPage.Virtualperson;
            });
        }

        /// <summary>
        /// 删除共有人
        /// </summary>
        private void DelSharPerson()
        {
            Person currentPerson = panelSharePerson.CurrentPerson;
            if (currentPerson == null)
            {
                ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelDataNo);
                return;
            }
            if (currentPerson != null && currentPerson.Name == CurrentVirtualPerson.Name)
            {
                ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonForbidden);
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                try
                {
                    VirtualPerson vp = CurrentVirtualPerson;
                    List<Person> list = vp.SharePersonList;
                    list.RemoveAll(t => t.ID == currentPerson.ID);
                    vp.SharePersonList = list;
                    Business.Update(vp);
                    CurrentVirtualPerson = vp;
                }
                catch (Exception ex)
                {
                    ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonFail);
                    YuLinTu.Library.Log.Log.WriteException(this, "DelSharePerson(删除共有人)", ex.Message + ex.StackTrace);
                }
            });
            ShowBox(VirtualPersonInfo.DelSharePerson, VirtualPersonInfo.DelPersonWarring, eMessageGrade.Warn, action);
        }

        /// <summary>
        /// 编辑共有人
        /// </summary>
        private void EditSharPerson()
        {
            if (panelSharePerson.CurrentPerson == null)
            {
                ShowBox(VirtualPersonInfo.EditSharePerson, VirtualPersonInfo.EditDataNo);
                return;
            }
            Person currentPerson = panelSharePerson.CurrentPerson.Clone() as Person;
            if (currentPerson != null && currentPerson.Name == CurrentVirtualPerson.Name)
            {
                UpdateVirtualPerson();
            }
            else
            {
                List<VirtualPerson> vpList = VirtualPersonItems.ToList();
                PersonInfoPage personPage = new PersonInfoPage(CurrentVirtualPerson, false);
                personPage.Business = Business;
                personPage.Person = currentPerson;
                personPage.PersonItems = vpList;
                personPage.Workpage = theworkpage;
                personPage.OtherDefine = FamilyOtherSet;
                theworkpage.Page.ShowMessageBox(personPage, (b, r) =>
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
                    CurrentVirtualPerson = personPage.Virtualperson;
                });
            }
        }

        /// <summary>
        /// 添加地块
        /// </summary>
        private void AddContractLand(object sender, RoutedEventArgs e)
        {
            AddCLand();
        }

        /// <summary>
        /// 添加承包地块
        /// </summary>
        private void AddCLand()
        {
            if (currentZone == null || currentVritualPerson == null)
            {
                return;
            }
            ContractLandPage landDlg = new ContractLandPage(true);
            landDlg.Workpage = Theworkpage;
            landDlg.CurrentZone = currentZone;
            landDlg.CurrentPerson = currentVritualPerson;
            landDlg.CurrentLand = new ContractLand()
            {
                SenderCode = currentZone.FullCode,
                SenderName = currentZone.FullName,
                OwnerId = currentVritualPerson.ID,
                OwnerName = currentVritualPerson.Name
            };
            theworkpage.Page.ShowMessageBox(landDlg, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                //向地块ListBox中添加地块信息
                var landUI = ContractLandUI.ContractLandUIConvert(landDlg.CurrentLand);
                LandPanelControl.ContractLandUIItems.Add(landUI);
            });
        }

        /// <summary>
        /// 编辑地块
        /// </summary>
        private void EditContractLand(object sender, RoutedEventArgs e)
        {
            EditCLand();
        }

        /// <summary>
        /// 编辑地块
        /// </summary>
        private void EditCLand()
        {
            if (currentZone == null || currentVritualPerson == null || LandPanelControl.CurrentContractLand == null)
            {
                ShowBox("编辑地块", "请选择编辑的地块!");
                return;
            }
            ContractLandPage landDlg = new ContractLandPage();
            landDlg.Workpage = Theworkpage;
            landDlg.CurrentZone = currentZone;
            landDlg.CurrentPerson = currentVritualPerson;
            ContractLandUI landTemp = LandPanelControl.CurrentContractLand.Clone() as ContractLandUI;
            landDlg.CurrentLand = landTemp.ConvertTo<ContractLand>();
            Theworkpage.Page.ShowMessageBox(landDlg, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                var landUI = ContractLandUI.ContractLandUIConvert(landDlg.CurrentLand);
                if (CurrentVirtualPerson.ID != landDlg.CurrentPerson.ID)
                {
                    LandPanelControl.ContractLandUIItems.Remove(LandPanelControl.CurrentContractLand);
                    return;
                }
                LandPanelControl.CurrentContractLand.CopyPropertiesFrom(landUI);
            });
        }

        /// <summary>
        /// 删除地块
        /// </summary>
        public void DelContractLand(object sender, RoutedEventArgs e)
        {
            DelCLand();
        }

        /// <summary>
        /// 删除地块
        /// </summary>
        public void DelCLand()
        {
            if (LandPanelControl.CurrentContractLand == null)
            {
                ShowBox("删除地块", "请选择要删除的地块!");
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    List<ContractLand> listLand = landBusiness.GetPersonCollection(CurrentVirtualPerson.ID);
                    ContractLand currentLand = listLand.Find(c => c.ID == LandPanelControl.CurrentContractLand.ID);
                    Guid[] landIds = new Guid[] { currentLand.ID };
                    landStation.DeleteRelationDataByLand(landIds);
                    LandPanelControl.ContractLandUIItems.Remove(LandPanelControl.CurrentContractLand);
                    CurrentMapControl.Refresh();
                }));
                var arg = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_Refresh, null);
                Theworkpage.Workspace.Message.Send(this, arg);
            });
            ShowBox("删除地块", "确定要删除选择的地块?", eMessageGrade.Infomation, action);

        }

        /// <summary>
        /// 设置当前实体
        /// </summary>
        private void SetEntity(VirtualPerson entity)
        {
            pcpcontrol.DataContext = entity;
            panelSharePerson.CurrentContractor = entity;
            panelLand.CurrentContractor = entity;

            if (panelSharePerson == null || panelLand == null || entity == null)
            {
                panelSharePerson.Set(null);
                panelLand.Set(null);
                return;
            }
            listLand = landBusiness.GetPersonCollection(entity.ID);
            PersonCollection pc = new PersonCollection();
            foreach (Person item in entity.SharePersonList.ToList())
            {
                pc.Add(item);
            }
            if (pc.GetLiteralLength() == 0)
            {
                return;
            }
            else
            {
                panelSharePerson.Set(pc);
                panelLand.Set(listLand);
            }
        }

        /// <summary>
        /// ListBox内容清除
        /// </summary>
        private void Clear()
        {
            lock (this)
            {
                VirtualPersonItems.Clear();
                CurrentVirtualPerson = null;
                panelSharePerson.Set(null);
                panelLand.Set(null);
            }
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
                VirtualPersonItems.Add(vp);
            }
            else
            {
                int index = VirtualPersonItems.IndexOf(currentVritualPerson);
                VirtualPersonItems.Remove(currentVritualPerson);
                VirtualPersonItems.Insert(index, vp);
                CurrentVirtualPerson = vp;
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSourceWork.GetDataBaseSource();
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        public void SendMessasge(ModuleMsgArgs args)
        {
            Theworkpage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            Theworkpage.Workspace.Message.Send(this, args);
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            theworkpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        #endregion

        #region Methods - Protected

        #endregion

        #region Methods - Public

        /// <summary>
        /// 地域相关权属 这里就可以选择地域
        /// </summary>
        /// <param name="item"></param>
        public void SetZone(NavigateItem item)
        {
            lock (this)
            {
                navigateItem = item;
                if (item == null)
                    return;
                Zone zone = item.Object as Zone;
                if (zone == null)
                    return;
                currentZone = zone;
                tempVirtualPerson = null;
                InlitialControl(zone.FullCode);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (currentZone == null)
            {
                return;
            }
            InlitialControl(currentZone.FullCode);
        }

        /// <summary>
        /// 根据编码查询地块
        /// </summary>
        /// <param name="code">地块全编码或者地块顺序码</param>
        public void Search(string code)
        {
            CurrentMapControl.SelectedItems.Clear();
            if (string.IsNullOrEmpty(code))
            {
                return;
            }
            //先根据地块编码在ContractLand数据库中查找对应地块
            List<ContractLand> listLand = landBusiness.GetLandCollection(currentZone.FullCode);
            if (listLand == null)
            {
                ShowBox("查询地块", string.Format("在{0}地域下无地块信息", currentZone.FullName));
                return;
            }
            ContractLand land = listLand.Find(c => c.LandNumber.Equals(code) || (c.SurveyNumber == null ? false : c.SurveyNumber.Equals(code)));  //找到地块
            if (land == null)
            {
                ShowBox("查询地块", string.Format("在{0}地域下未找到所查找的地块信息", currentZone.FullName));
                return;
            }
            VirtualPerson person = VirtualPersonItems.FirstOrDefault(c => c.ID == land.OwnerId);   //找到承包方
            CurrentVirtualPerson = person;
            var landGeo = land.Shape as YuLinTu.Spatial.Geometry;
            if (landGeo == null)
            {
                return;
            }
            CurrentMapControl.ZoomTo(landGeo);
            switch (land.LandCategory)
            {
                case "10":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_cbd_Layer") as VectorLayer;
                    break;
                case "21":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_zld_Layer") as VectorLayer;
                    break;
                case "22":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_jdd_Layer") as VectorLayer;
                    break;
                case "23":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_khd_Layer") as VectorLayer;
                    break;
                case "99":
                    landLayer = CurrentMapControl.Layers.FindLayerByInternalName("dklb_qtjttd_Layer") as VectorLayer;
                    break;
            }
            if (landLayer == null) return;
            var fo = landLayer.DataSource.FirstOrDefault(string.Format("{0} = \"{1}\"", typeof(ContractLand).GetProperty("ID").GetAttribute<DataColumnAttribute>().ColumnName, land.ID));
            if (fo != null)
            {
                var g = new Graphic();
                g.Object = fo;
                g.Layer = landLayer;

                CurrentMapControl.SelectedItems.Add(g);
            }
        }

        #endregion

        #region Method - Event

        #endregion
    }
}
