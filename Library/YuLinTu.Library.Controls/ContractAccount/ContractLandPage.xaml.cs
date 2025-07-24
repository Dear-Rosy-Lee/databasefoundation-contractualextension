/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.NetAux;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地块添加/编辑页面
    /// </summary>
    public partial class ContractLandPage : InfoPageBase
    {
        #region Fields
        public const string POSITIONOUT = "3";
        public const string POSITIONCEN = "2";
        public const string POSITIONIN = "1";
        private ContractLand currentLand;
        private ContractLand tempLand;
        private ContractLand prevoiusLand;
        private Zone currentZone;
        private List<VirtualPerson> virtualPersonList;
        private bool isAdd;
        private VirtualPerson currentPerson;
        private VirtualPersonBusiness personBusiness;
        private AccountLandBusiness landBusiness;
        private List<Dictionary> list;
        //private int count;
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueGet;//获取数据
        //private string newLandNumber;//新地块编码
        private IDbContext dbContext;//数据库
        private AgricultureLandExpand landExpand;//地块扩展
        private bool isLock;
        private bool _isStockRight;
        private Graphic pointgraphic;
        private Graphic linegraphic;

        #endregion

        #region Property

        /// <summary>
        /// 适配确权确股项目插件 不要删除此属性
        /// </summary>
        public bool IsStockRight
        {
            get { return _isStockRight; }
            set
            {
                _isStockRight = value;
                if (_isStockRight)
                {
                    Label2.Content = "量化面积:";
                    Label3.Content = "预留面积:";
                    Label1.Content = "量化股数:";
                    Binding bind = new Binding();
                    bind.Source = this.CurrentLand;
                    bind.Path = new PropertyPath("QuantificicationArea");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    DoubleUpDownTableArea.SetBinding(DoubleUpDown.ValueProperty, bind);

                    Binding bind2 = new Binding();
                    bind2.Source = this.CurrentLand;
                    bind2.Path = new PropertyPath("ObligateArea");
                    bind2.Mode = BindingMode.TwoWay;
                    bind2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    doubleUpDownArea.SetBinding(DoubleUpDown.ValueProperty, bind2);

                    Binding bind3 = new Binding();
                    bind3.Source = this.CurrentLand;
                    bind3.Mode = BindingMode.TwoWay;
                    bind3.Path = new PropertyPath("QuantificicationStockQuantity");
                    bind3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    cmbAwareArea.SetBinding(DoubleUpDown.ValueProperty, bind3);

                    cmbVirtualPersonName.IsEnabled = false;
                    txtOwnerName.IsEnabled = false;
                    cmbVirtualPersonName.Text = string.Empty;
                    txtOwnerName.Text = string.Empty;
                    doubleUpDownArea.IsEnabled = false;
                    cmbAwareArea.IsEnabled = false;
                }
                else
                {

                }
            }
        }

        public IDbContext DataSource
        {
            get { return dbContext; }
            set { dbContext = value; }
        }

        /// <summary>
        /// 当前地域属性
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                txtZoneName.Text = currentZone.FullName;
            }
        }

        /// <summary>
        /// 当前选择地域下所有承包方信息
        /// </summary>
        public List<VirtualPerson> VirtpersonList
        {
            get { return virtualPersonList; }
            set
            {
                virtualPersonList = value;
                //初始化承包方集合
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!IsStockRight)
                    {
                        SetSelectPerson();
                        cmbVirtualPersonName.ItemsSource = SelectVirtualPersonObs;
                        //cmbVirtualPersonName.DisplayMemberPath = "Name";
                        //cmbVirtualPersonName.SelectedIndex = 0;
                    }
                }));
            }
        }

        /// <summary>
        /// 当前地块属性
        /// </summary>
        public ContractLand CurrentLand
        {
            get { return currentLand; }
            set
            {
                currentLand = value;
                tempLand = currentLand.Clone() as ContractLand;
                LandExpand = currentLand == null ? new AgricultureLandExpand() : currentLand.LandExpand;
                if (!isAdd)
                {
                    InitialDotControl(CurrentLand);
                    InitialCoilControl(CurrentLand);
                }
                NotifyPropertyChanged("CurrentLand");
            }
        }

        /// <summary>
        /// 地块扩展信息
        /// </summary>
        public AgricultureLandExpand LandExpand
        {
            get { return landExpand; }
            private set
            {
                landExpand = value;
                NotifyPropertyChanged("LandExpand");
            }
        }

        /// <summary>
        /// 当前选择承包方
        /// </summary>
        public VirtualPerson CurrentPerson
        {
            get { return currentPerson; }
            set
            {
                currentPerson = value;
                SetControlEnable(); //设置控件可用性
            }
        }

        /// <summary>
        /// 标记主界面是否选中项属性
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 修改前的地块信息
        /// </summary>
        public ContractLand PrevoiusLand
        {
            get { return prevoiusLand; }
            set
            {
                prevoiusLand = value;
            }
        }

        /// <summary>
        /// 界址点界面显示
        /// </summary>
        public ObservableCollection<ConstructionLandDotItem> DotDataItems { get; set; }

        /// <summary>
        /// 界址线界面显示
        /// </summary>
        public ObservableCollection<ConstructionLandCoilItem> CoilDataItems { get; set; }

        /// <summary>
        /// 地块被锁定
        /// </summary>
        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
                if (isLock)
                {
                    TabItem[] items = new TabItem[] { tiFoundationInfo, tiOtherInfo };
                    for (int i = 0; i < 3; i++)
                    {
                        if (i >= items.Length)
                            break;
                        var enmu = (items[i].Content as System.Windows.Controls.Grid).Children.GetEnumerator();
                        while (enmu.MoveNext())
                        {
                            var curTxtEle = enmu.Current as TextBox;
                            if (curTxtEle != null)
                            {
                                curTxtEle.IsReadOnly = true;
                                continue;
                            }
                            var curLbEle = enmu.Current as Label;
                            if (curLbEle != null)
                                continue;
                            var curEle = enmu.Current as UIElement;
                            curEle.IsEnabled = false;
                        }
                    }
                    //spInitialDotCoil.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 地图属性编辑框委托
        /// </summary>
        public delegate bool ShowMapPropertyDelegate();

        /// <summary>
        /// 在地图中显示地块属性编辑框
        /// </summary>
        public ShowMapPropertyDelegate ShowMapProperty { get; set; }
        public MapControl MapControl { get; set; }
        public GraphicsLayer layerHover { get; set; }

        public ObservableCollection<SelectPersonInterface> SelectVirtualPersonObs = new ObservableCollection<SelectPersonInterface>();

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandPage(bool isAdd = false)
        {
            InitializeComponent();
            this.isAdd = isAdd;
            dbContext = DataBaseSource.GetDataBaseSource();
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueGet = new TaskQueueDispatcher(Dispatcher);
            list = new List<Dictionary>();
            DotDataItems = new ObservableCollection<ConstructionLandDotItem>();
            CoilDataItems = new ObservableCollection<ConstructionLandCoilItem>();
            if (isAdd)
            {
                dotItem.Visibility = Visibility.Collapsed;
                coilItem.Visibility = Visibility.Collapsed;
                //spInitialDotCoil.Visibility = Visibility.Collapsed;
            }
            this.DataContext = this;
            pointgraphic = new Graphic();
            linegraphic = new Graphic();
            //IsStockRight = false; 
        }

        #endregion

        #region Method

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitializeGo()
        {
            personBusiness = new VirtualPersonBusiness(dbContext);
            personBusiness.VirtualType = eVirtualType.Land;
            if (virtualPersonList == null && currentZone != null)
            {
                VirtpersonList = personBusiness.GetByZone(currentZone.FullCode);
            }
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            list = dictBusiness.GetAll();
            landBusiness = new AccountLandBusiness(dbContext);
            if (isAdd && currentZone != null)
            {
                CurrentLand.LandNumber = landBusiness.GetNewLandNumber(currentZone.FullCode);
            }
        }

        /// <summary>
        /// 设置选择人员信息
        /// </summary>
        private void SetSelectPerson(string part = "")
        {
            SelectVirtualPersonObs.Clear();
            if (VirtpersonList != null && VirtpersonList.Count() == 0)
            {
                return;
            }
            foreach (var svp in VirtpersonList)
            {
                var sitm = new SelectPersonInterface()
                {
                    ID = svp.ID.ToString(),
                    Name = $"{svp.Name}({svp.FamilyNumber.PadLeft(4, '0')})"
                };
                if (!string.IsNullOrEmpty(part) && !sitm.Name.Contains(part))
                    continue;
                SelectVirtualPersonObs.Add(sitm);
            }
        }

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            if (list != null)
            {
                var ownerRightList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.SYQXZ));
                cmbOwnerRight.ItemsSource = ownerRightList;
                cmbOwnerRight.DisplayMemberPath = "Name";
                cmbOwnerRight.SelectedIndex = 0;

                var landNameList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.Code.Length == 2 && c.GroupCode.Equals(DictionaryTypeInfo.TDLYLX));
                cmbLandName.ItemsSource = landNameList;
                cmbLandName.DisplayMemberPath = "Name";
                cmbLandName.SelectedIndex = 0;

                var contractWayList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.CBJYQQDFS));
                cmbContractWay.ItemsSource = contractWayList;
                cmbContractWay.DisplayMemberPath = "Name";
                cmbContractWay.SelectedIndex = 0;

                var landTypeList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.DKLB));
                cmbLandType.ItemsSource = landTypeList;
                cmbLandType.DisplayMemberPath = "Name";
                cmbLandType.SelectedIndex = 0;

                var quliatyList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.DLDJ));
                cmbQuliaty.ItemsSource = quliatyList;
                cmbQuliaty.DisplayMemberPath = "Name";
                cmbQuliaty.SelectedIndex = quliatyList.Count - 1;

                var useforList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.TDYT));
                cmbPropous.ItemsSource = useforList;
                cmbPropous.DisplayMemberPath = "Name";
                cmbPropous.SelectedIndex = 0;

                var plandTypeList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.ZZLX));
                cbPlandType.ItemsSource = plandTypeList;
                cbPlandType.DisplayMemberPath = "Name";
                cbPlandType.SelectedIndex = plandTypeList.Count - 1;

                var plowTypeList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.GBZL));
                cbPLowType.ItemsSource = plowTypeList;
                cbPLowType.DisplayMemberPath = "Name";
                cbPLowType.SelectedIndex = plowTypeList.Count - 1;

                var tanscateTypeList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.JYFS));
                cbTanscateType.ItemsSource = tanscateTypeList;
                cbTanscateType.DisplayMemberPath = "Name";
                cbTanscateType.SelectedIndex = tanscateTypeList.Count - 1;

                var transcateWayList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.GroupCode.Equals(DictionaryTypeInfo.LZLX));
                cbTranscateWay.ItemsSource = transcateWayList;
                cbTranscateWay.DisplayMemberPath = "Name";
                cbTranscateWay.SelectedIndex = transcateWayList.Count - 1;
            }
            if (currentPerson != null && virtualPersonList != null)
            {
                if (!IsStockRight)
                {
                    var fvp = virtualPersonList.Find(t => t.ID == currentPerson.ID);
                    if (fvp != null)
                    {
                        var citem = SelectVirtualPersonObs.Where(t => t.ID == fvp.ID.ToString()).FirstOrDefault();
                        cmbVirtualPersonName.SelectedItem = citem;
                    }
                }
            }
            else
            {
                if (!IsStockRight)
                    cmbVirtualPersonName.SelectedIndex = 0;
            }
            SetComBox(currentLand);
        }

        /// <summary>
        /// 设置下拉控件
        /// </summary>
        public void SetComBox(ContractLand land)
        {
            if (land == null)
            {
                return;
            }
            SetComboboxSelect(cmbOwnerRight, land.OwnRightType); //所有权性质
            SetComboboxSelect(cmbContractWay, land.ConstructMode);//承包方式
            SetComboboxSelect(cmbLandType, land.LandCategory);//地块类别
            SetComboboxSelect(cmbQuliaty, land.LandLevel);//地力等级
            SetComboboxSelect(cmbPropous, land.Purpose);//土地用途
            SetComboboxSelect(cbPlandType, land.PlatType);//土地用途
            SetComboboxSelect(cbPLowType, land.PlantType);//耕保类型
            SetComboboxSelect(cbTanscateType, land.ManagementType);//经营方式
            SetComboboxSelect(cbTranscateWay, land.TransferType);//流转方式
            if (!isAdd)
            {
                if (land.IsFarmerLand == null)
                    cmbIsBase.SelectedIndex = 2;
                if (land.IsFarmerLand == false)
                    cmbIsBase.SelectedIndex = 1;
                if (land.IsFarmerLand == true)
                    cmbIsBase.SelectedIndex = 0;
                if (land.IsFlyLand)
                    cbIsFly.SelectedIndex = 0;
                if (land.IsTransfer)
                    cbIsTransfor.SelectedIndex = 0;
            }
            else
            {
                if (land.IsFarmerLand == null)
                    cmbIsBase.SelectedIndex = 2;
                if (land.IsFarmerLand == false)
                    cmbIsBase.SelectedIndex = 1;
                if (land.IsFarmerLand == true)
                    cmbIsBase.SelectedIndex = 0;
                cbLandNameAlise.IsChecked = true;
                cbLandNameSecond.IsChecked = true;
                txtCode.Text = land.LandNumber;
            }
            if (!string.IsNullOrEmpty(land.LandCode))
            {
                Dictionary dic = list.Find(c => !string.IsNullOrEmpty(c.Code) && c.Code == land.LandCode && c.GroupCode == DictionaryTypeInfo.TDLYLX);
                if (dic != null && dic.Name == land.LandName)
                {
                    cbLandNameAlise.IsChecked = true;
                }
                if (land.LandCode.Length == 3)
                {
                    cbLandNameSecond.IsChecked = true;
                    Dictionary fdic = list.Find(c => !string.IsNullOrEmpty(c.Code) && c.Code == land.LandCode.Substring(0, 2) && c.GroupCode == DictionaryTypeInfo.TDLYLX);//一级;
                    cmbLandName.SelectedItem = fdic;
                    cmbLandNameSecond.SelectedItem = dic;
                }
                else if (land.LandCode.Length == 2)
                {
                    cmbLandName.SelectedItem = dic;
                }
            }
            if (!string.IsNullOrEmpty(land.LandName))
            {
                txtLandNameAlise.Text = land.LandName;
            }
        }

        /// <summary>
        /// 更新数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void autoCities_PatternChanged(object sender, Utils.Controls.AutoComplete.AutoCompleteArgs args)
        {
            SetSelectPerson(args.Pattern);
            args.DataSource = SelectVirtualPersonObs;
        }

        /// <summary>
        /// 设置下拉控件选择项 
        /// </summary>
        private void SetComboboxSelect(MetroComboBox combobox, string code)
        {
            if (combobox == null || string.IsNullOrEmpty(code) || combobox.Items.Count == 0)
            {
                return;
            }
            Dictionary dic = null;
            foreach (var item in combobox.Items)
            {
                Dictionary d = item as Dictionary;
                if (d == null)
                    continue;
                if (d.Code == code)
                    dic = d;
            }
            combobox.SelectedItem = dic;
        }

        /// <summary>
        /// 设置下拉控件选择项 
        /// </summary>
        private string GetComboboxSelectValue(MetroComboBox combobox)
        {
            if (combobox == null || combobox.SelectedItem == null)
            {
                return "";
            }
            Dictionary dic = combobox.SelectedItem as Dictionary;
            if (dic == null)
            {
                return "";
            }
            return dic.Code;
        }


        /// <summary>
        /// 设置控件可用性
        /// </summary>
        private void SetControlEnable()
        {
            if (IsSelected && isAdd)
            {
                cmbVirtualPersonName.IsEnabled = false;
                txtOwnerName.IsEnabled = false;
            }
            if (CurrentPerson.Status == eVirtualPersonStatus.Lock)
                mbtnLandOK.IsEnabled = false;
        }

        #endregion

        #region Event

        ///// <summary>
        ///// 初始化单个地块界址信息
        ///// </summary>
        //private void spInitialDotCoil_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CurrentLand == null || CurrentLand.Shape == null)
        //    {
        //        ShowBox("初始化单个地块界址信息", "未获取待生成界址信息的空间承包地块!");
        //        return;
        //    }
        //    try
        //    {
        //        var dotCoilPage = new ContractLandInitializeDotCoil(Workpage);
        //        dotCoilPage.CurrentZone = CurrentZone;
        //        dotCoilPage.CurrentLand = CurrentLand;
        //        dotCoilPage.CurrentGeoLand = CurrentLand.Shape;
        //        dotCoilPage.gpInitial.Visibility = Visibility.Collapsed;
        //        dotCoilPage.gpSingleInitial.Visibility = Visibility.Collapsed;
        //        Workpage.Page.ShowMessageBox(dotCoilPage, (b, r) =>
        //        {
        //            if (!(bool)b)
        //            {
        //                if (dotCoilPage.MapControl.InternalLayers.Contains(dotCoilPage.GpLayer))
        //                {
        //                    dotCoilPage.MapControl.InternalLayers.Remove(dotCoilPage.GpLayer);
        //                    dotCoilPage.GpLayer.Graphics.Clear();
        //                    dotCoilPage.GpLayer.Dispose();
        //                }
        //                return;
        //            }
        //            var meta = dotCoilPage.GetArgument();
        //            var importDot = new InitializeLandDotCoilTask();
        //            importDot.Argument = meta;
        //            importDot.ContractLandInitialTool();

        //            InitialDotControl(CurrentLand);
        //            InitialCoilControl(CurrentLand);

        //            ShowBox("初始化单个地块界址信息", string.Format("成功初始化地块编码：{0}的界址信息", CurrentLand.LandNumber), eMessageGrade.Infomation);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowBox("初始化单个地块界址信息", "初始化单个地块界址信息失败!");
        //        Log.Log.WriteException(this, "spInitialDotCoil_Click(初始化单个地块界址信息)", ex.StackTrace + ex.Message);
        //    }
        //}

        /// <summary>
        /// 响应添加或者编辑承包台账地块信息确定按钮
        /// </summary>
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            GetLandValue();
            if (IsStockRight && !StockRight())
                return;
            ConfirmAsync(goCallback =>
            {
                if (currentPerson == null && !IsStockRight)
                    return false;
                return GoConfirm();
            }, completedCallback =>
            {
                Close(true);
            });
        }

        /// <summary>
        /// 适配确权确股
        /// </summary>
        private bool StockRight()
        {
            CurrentLand.IsStockLand = true;
            if (CurrentLand.QuantificicationArea > CurrentLand.ActualArea)
            {
                ShowBox(isAdd ? ContractAccountInfo.ContractLandAdd : ContractAccountInfo.ContractLandEdit, "量化面积不能大于实测面积！");
                return false;
            }
            CurrentLand.OwnerId = null;
            CurrentLand.ObligateArea = CurrentLand.ActualArea - CurrentLand.QuantificicationArea;
            CurrentLand.QuantificicationStockQuantity = CurrentLand.QuantificicationArea / CurrentLand.ActualArea;
            CurrentLand.ObligateStockQuantity = 1 - CurrentLand.QuantificicationStockQuantity;
            return true;
        }

        /// <summary>
        /// 获取承包地块值
        /// </summary>
        private void GetLandValue()
        {
            var spi = cmbVirtualPersonName.SelectedItem as SelectPersonInterface;
            if (spi != null)
            {
                currentPerson = VirtpersonList.Find(t => t.ID.ToString() == spi.ID);
            }
            if (currentPerson == null)
            {
                //没有指明添加地块的承包方
                if (!IsStockRight)
                {
                    ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.LandAddNoSelectedPerson);
                    return;
                }
            }
            currentLand.ZoneCode = currentZone.FullCode;
            currentLand.ZoneName = currentZone.FullName;

            if (currentPerson != null)
            {
                currentLand.OwnerId = currentPerson.ID;
                currentLand.OwnerName = currentPerson.Name;
            }

            Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
            {
                //没有指明土地利用类型
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.LandAddNoContractLandUseType);
                return;
            }
            Dictionary currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
            if (cmbLandNameSecond.IsEnabled)
            {
                //二级编码
                currentLand.LandCode = currentLandNameSecond.Code;//list.Find(c => c.Name == currentLandNameSecond.Name).Code;
            }
            else
            {
                //一级编码
                currentLand.LandCode = currentLandName.Code; //list.Find(c => c.Name == currentLandName.Name).Code;
            }
            currentLand.OwnRightType = GetComboboxSelectValue(cmbOwnerRight); //所有权性质
            currentLand.ConstructMode = GetComboboxSelectValue(cmbContractWay);//承包方式
            currentLand.LandCategory = GetComboboxSelectValue(cmbLandType);//地块类别
            currentLand.LandLevel = GetComboboxSelectValue(cmbQuliaty);//地力等级
            currentLand.Purpose = GetComboboxSelectValue(cmbPropous);//土地用途
            currentLand.PlatType = GetComboboxSelectValue(cbPlandType);//土地用途
            currentLand.PlantType = GetComboboxSelectValue(cbPLowType);//耕保类型
            currentLand.ManagementType = GetComboboxSelectValue(cbTanscateType);//经营方式
            currentLand.TransferType = GetComboboxSelectValue(cbTranscateWay);//流转方式
            if (cbIsFly.SelectedIndex == 1)
                currentLand.IsFlyLand = false;
            else
                currentLand.IsFlyLand = true;

            if (cbIsTransfor.SelectedIndex == 1)
                currentLand.IsTransfer = false;
            else
                currentLand.IsTransfer = true;

            if (cmbIsBase.SelectedIndex == 0)
                currentLand.IsFarmerLand = true;
            else if (cmbIsBase.SelectedIndex == 1)
                currentLand.IsFarmerLand = false;
            else
                currentLand.IsFarmerLand = null;

            currentLand.LandExpand = LandExpand;
            if (currentLand.OwnRightType.IsNullOrEmpty())
            {
                currentLand.OwnRightType = "30";
            }
            if (currentLand.Status.IsNullOrEmpty())
            {
                currentLand.Status = "10";
            }
            currentLand.LandName = txtLandNameAlise.Text;
            //调查编码从地块调查表导入后，其他操作与他无关。
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        private bool GoConfirm()
        {
            bool result = true;
            var db = dbContext;
            try
            {
                if (db == null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowBox(ContractAccountInfo.ContractLandPro, DataBaseSource.ConnectionError);
                    }));
                    return false;
                }
                if (!isLock)
                {
                    //string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
                    //var sr = ReferenceHelper.GetDbReference<Zone>(db, name, "Shape");
                    //bool f = ModifyDots(db, sr.WKID);
                    bool f = ModifyDots(db);
                    if (!f)
                        return false;
                }
                var landStation = db.CreateContractLandWorkstation();
                if (CheckCommitData())
                {
                    if (isAdd)
                    {
                        landStation.Add(currentLand);
                        //landBusiness.AddLand(currentLand);
                        ModuleMsgArgs argsAdd = MessageExtend.ContractAccountMsg(db, ContractAccountMessage.CONTRACTLAND_ADD_COMPLETE, currentLand, currentZone.FullCode);
                        SendMessasge(argsAdd);  //发送承包地块添加完成消息
                    }
                    else
                    {
                        if (!isLock)
                        {
                            landStation.Update(currentLand);
                            //landBusiness.ModifyLand(currentLand);
                            ModuleMsgArgs argsEdit = MessageExtend.ContractAccountMsg(db, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, currentLand, CurrentZone.FullCode, tempLand);
                            SendMessasge(argsEdit);    //发送承包地块编辑完成消息
                        }
                    }
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox(ContractAccountInfo.ContractLandPro, ContractAccountInfo.ContractLandProFail);
                }));
                YuLinTu.Library.Log.Log.WriteException(this, "GoConfirm(承包地块数据处理失败!)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 修改界址点信息（是否有效）
        /// </summary>
        private bool ModifyDots(IDbContext db)
        {
            if (DotDataItems == null || DotDataItems.Count == 0)
                return true;
            var dotStation = db.CreateBoundaryAddressDotWorkStation();
            var coilStation = db.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(DotDataItems.Count);
            int isHandleValidUI = 0;//是否操作了界址点是否有效，默认是操作了的
            foreach (var item in DotDataItems)
            {
                if (item.Entity == null)
                    continue;
                if (item.Entity.IsValid == item.IsValidUI)
                {
                    ++isHandleValidUI;//没有变界址点有效性，所以不执行后面的
                }
                item.Entity.IsValid = item.IsValidUI;
                listDot.Add(item.Entity);
                if (item.IsValidUI)
                    listValidDot.Add(item.Entity);
            }
            if (isHandleValidUI == DotDataItems.Count) return true;

            if (listValidDot.Count < 3)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("界址点", "有效界址点不能少于3个", eMessageGrade.Error);
                }));
                return false;
            }
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(CoilDataItems.Count);
            BuildLandBoundaryAddressCoil curCoil = coilStation.GetByLandId(listDot[0].LandID).FirstOrDefault();

            var zoneStation = db.CreateZoneWorkStation();
            var landStation = db.CreateContractLandWorkstation();
            var zonelist = zoneStation.GetParents(currentZone);
            var zoneParentName = currentZone.Name;
            foreach (var item in zonelist)
            {
                zoneParentName = item.Name + zoneParentName;
            }
            coilStation.DeleteByLandIds(new Guid[] { currentLand.ID });
            var param = new InitLandDotCoilParam();
            var buffGeo = currentLand.Shape.Buffer(param.AddressLinedbiDistance);
            var lands = landStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
            if (lands == null || lands.Count == 0)
                lands = new List<ContractLand>();
            var coils = coilStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
            listCoil = ProcessLine(currentLand, lands, new InitLandDotCoilParam(), listDot, coils, zoneParentName, true);//初始化界址线

            //if (curCoil == null)
            //    curCoil = new BuildLandBoundaryAddressCoil();
            //int j = 0;
            //for (int i = 0; i < listValidDot.Count; i++)
            //{
            //    BuildLandBoundaryAddressCoil cCoil = new BuildLandBoundaryAddressCoil();
            //    cCoil = curCoil.Clone() as BuildLandBoundaryAddressCoil;

            //    cCoil.ID = Guid.NewGuid();
            //    cCoil.OrderID = short.Parse((i + 1).ToString());
            //    cCoil.LandID = listValidDot[i].LandID;
            //    cCoil.LandNumber = listValidDot[i].LandNumber;
            //    cCoil.StartPointID = listValidDot[i].ID;
            //    cCoil.StartNumber = listValidDot[i].DotNumber;
            //    if (i + 1 == listValidDot.Count)
            //    {
            //        cCoil.EndPointID = listValidDot[0].ID;
            //        cCoil.EndNumber = listValidDot[0].DotNumber;
            //    }
            //    else
            //    {
            //        cCoil.EndPointID = listValidDot[i + 1].ID;
            //        cCoil.EndNumber = listValidDot[i + 1].DotNumber;
            //    }
            //    List<Coordinate> dots = new List<Coordinate>();
            //    double dis = 0;
            //    for (; j < DotDataItems.Count; j++)
            //    {
            //        dots.Add(DotDataItems[j].Entity.Shape.ToCoordinates().FirstOrDefault());
            //        ConstructionLandDotItem ed = DotDataItems[0];
            //        if (j + 1 != DotDataItems.Count)
            //            ed = DotDataItems[j + 1];
            //        dots.Add(ed.Entity.Shape.ToCoordinates().FirstOrDefault());
            //        dis += Distance(DotDataItems[j], ed);
            //        if (ed.Entity.ID == cCoil.EndPointID)
            //        {
            //            j++;
            //            break;
            //        }
            //    }
            //    cCoil.CoilLength = ToolMath.RoundNumericFormat(dis, 2);
            //    //cCoil.Shape = YuLinTu.Spatial.Geometry.CreatePolyline(dots);
            //    //cCoil.Shape.Instance.SRID = SRID;
            //    //cCoil.Shape = YuLinTu.Spatial.Geometry.FromInstance(cCoil.Shape.Instance);
            //    cCoil.Shape = YuLinTu.Spatial.Geometry.CreatePolyline(dots, SRID);
            //    cCoil.CoilLength = cCoil.Shape.Length();
            //    GetLineDescription(cCoil, true, false, listDot);

            //    listCoil.Add(cCoil);
            //}
            db.BeginTransaction();
            try
            {
                //先删除界址线，然后再添加更新
                if (curCoil != null)
                {
                    coilStation.Delete(c => c.LandID.Equals(curCoil.LandID));
                }
                coilStation.AddRange(listCoil);
                dotStation.UpdateRange(listDot);
                db.CommitTransaction();
                return true;
            }
            catch
            {
                db.RollbackTransaction();
                return false;
            }
        }

        /// <summary>
        /// 初始化界址线
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> ProcessLine(ContractLand land, List<ContractLand> lands, InitLandDotCoilParam param,
            List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> coils, string senderName, bool IsSetAddressLinePosition)
        {
            var entityList = new List<BuildLandBoundaryAddressCoil>();
            if (dots == null || dots.Count == 0)
                return entityList;

            dots.Add(dots[0]);
            var createdots = new List<BuildLandBoundaryAddressDot>();
            short sxh = 1;
            bool hasStartKeyDot = false; //是否已经找到开始界址点
            bool hasEndKeyDot = false; //是否已经找到结束界址点

            foreach (var item in dots)
            {
                if (item.IsValid && hasStartKeyDot == false)
                {
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    continue;
                }

                if (item.IsValid == false)
                {
                    createdots.Add(item);
                    continue;
                }

                if (item.IsValid && hasStartKeyDot && hasEndKeyDot == false)
                {
                    createdots.Add(item);

                    var line = CreateAddressCoil(createdots, land, sxh, param, lands, senderName);
                    entityList.Add(line);

                    createdots.Clear();
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    hasEndKeyDot = false;
                    sxh++;
                }
            }

            LinePropertiesSet(entityList, coils, dots, IsSetAddressLinePosition, param.IsLineDescription);

            dots.Remove(dots[dots.Count - 1]);
            return entityList;
        }

        /// <summary>
        /// 创建界址线
        /// </summary>
        private BuildLandBoundaryAddressCoil CreateAddressCoil(List<BuildLandBoundaryAddressDot> list, ContractLand land,
            short sxh, InitLandDotCoilParam param, List<ContractLand> lands, string senderName)
        {
            var linestring = CreatLine(list, land.Shape.Srid);
            var line = new BuildLandBoundaryAddressCoil()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                ZoneCode = land.ZoneCode,
                Shape = linestring,
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                LandID = land.ID,
                LandNumber = land.LandNumber,
                LandType = land.LandCode,
                StartPointID = list[0].ID,
                StartNumber = list[0].DotNumber,
                EndPointID = list[list.Count - 1].ID,
                EndNumber = list[list.Count - 1].DotNumber,
                OrderID = sxh,
                CoilLength = linestring.Length(),
                Position = param.IsAddressLinePosition ? param.AddressLinePosition : "3",
                LineType = param.AddressLineType,
                CoilType = param.AddressLineCatalog,
                Description = linestring.Length().ToString(),
            };
            var linebuffer = linestring.Buffer(param.AddressLinedbiDistance);
            var landList = lands.FindAll(t => t.Shape.Intersects(linebuffer));
            landList.RemoveAll(t => t.ID == land.ID);
            if (landList.Count > 0)
            {
                line.NeighborPerson = landList[0].OwnerName;
                line.NeighborFefer = line.NeighborPerson;
            }
            else
            {
                line.NeighborPerson = senderName;
                line.NeighborFefer = senderName;
            }
            return line;
        }


        /// <summary>
        /// 界址线设置
        /// </summary> 
        private void LinePropertiesSet(List<BuildLandBoundaryAddressCoil> entityList, List<BuildLandBoundaryAddressCoil> coils,
          List<BuildLandBoundaryAddressDot> dots, bool IsSetAddressLinePosition, bool isUseLengthAndPosition)
        {
            var startIndex = 1;
            foreach (var line in entityList)
            {
                if (coils != null && coils.Count > 0)
                {
                    var coil = coils.Find(t => TestLineEqual(t.Shape, line.Shape));
                    if (coil != null)
                    {
                        line.Position = POSITIONCEN;
                    }
                }
                var charArray = line.StartNumber.Reverse();
                List<char> charlist = new List<char>();
                foreach (var item in charArray)
                {
                    if (item >= 48 && item <= 58)
                    {
                        charlist.Add(item);
                    }
                }
                charlist.Reverse();
                var num = "";
                foreach (var t in charlist)
                {
                    num += t;
                }
                if (num == "1")
                {
                    startIndex = line.OrderID;
                }
                //GetLineDescription(line, isUnit, isUseLengthAndPosition, dots);
            }
            if (IsSetAddressLinePosition)
            {
                foreach (var item in entityList)
                {
                    if (item.Position == POSITIONOUT)
                        item.Position = POSITIONIN;
                }
            }
            short orderid = 1;

            for (int i = startIndex; i <= entityList.Count; i++)
            {
                entityList[i - 1].OrderID = orderid;
                orderid++;
            }
            for (int i = 0; i < startIndex - 1; i++)
            {
                entityList[i].OrderID = orderid;
                orderid++;
            }
        }


        /// <summary>
        /// 线是否相等
        /// </summary>
        public bool TestLineEqual(Geometry geo1, Geometry geo2, double tolerance = 0.001)
        {
            var result = true;
            var geoArray1 = geo1.ToCoordinates();
            var geoArray2 = geo2.ToCoordinates();

            if (geoArray1.Length != geoArray2.Length)
                return false;

            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                geoArray2 = geoArray2.Reverse().ToArray();
            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                return false;

            for (int i = 0; i < geoArray1.Length; i++)
            {
                if (!testIsEqual(geoArray1[i], geoArray2[i].X, geoArray2[i].Y))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        public bool testIsEqual(Coordinate c, double x, double y, double tolerance = 0.001)
        {
            return CglHelper.equal(c.X, x, tolerance) && CglHelper.equal(c.Y, y, tolerance);
        }


        /// <summary>
        /// 创建线
        /// </summary>
        private Geometry CreatLine(List<BuildLandBoundaryAddressDot> dots, int srid)
        {
            Coordinate[] corrds = new Coordinate[dots.Count];
            for (int i = 0; i < dots.Count; i++)
            {
                corrds[i] = dots[i].Shape.ToCoordinates()[0];
            }
            var geo = YuLinTu.Spatial.Geometry.CreatePolyline(corrds.ToList(), srid);
            return geo;
        }

        /// <summary>
        /// 创建界址线说明
        /// </summary>
        /// <returns></returns>
        private void GetLineDescription(BuildLandBoundaryAddressCoil line, bool isUnit, bool isUseLengthAndPosition, List<BuildLandBoundaryAddressDot> dots)
        {
            Aspect a = new Aspect(0);
            if (line.Shape != null && !isUseLengthAndPosition)
            {
                var coords = line.Shape.ToCoordinates();
                var p0 = coords[0];
                var p1 = coords[coords.Count() - 1];
                a.Assign(p0.X, p0.Y, p1.X, p1.Y);
                string qjzdh = line.StartNumber;
                var zjzdh = line.EndNumber;
                if (isUnit)
                {
                    qjzdh = dots.Find(t => t.ID == line.StartPointID).UniteDotNumber;
                    zjzdh = dots.Find(t => t.ID == line.EndPointID).UniteDotNumber;
                }
                var jszsm = qjzdh + "沿" + a.toAzimuthString() + "方" + ToolMath.RoundNumericFormat(line.Shape.Length(), 2) + "米到" + zjzdh;
                line.Description = jszsm;
            }
        }

        private double Distance(ConstructionLandDotItem sd, ConstructionLandDotItem ed)
        {
            double x1 = double.Parse(sd.XCoordinateUI);
            double x2 = double.Parse(ed.XCoordinateUI);
            double y1 = double.Parse(sd.YCoordinateUI);
            double y2 = double.Parse(ed.YCoordinateUI);
            double val = Math.Sqrt(Math.Abs(x1 - x2) * Math.Abs(x1 - x2) + Math.Abs(y1 - y2) * Math.Abs(y1 - y2));
            return val;
        }
        /// <summary>
        /// 检查提交数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCommitData()
        {
            bool right = true;
            if (landBusiness == null)
            {
                landBusiness = new AccountLandBusiness(dbContext);
            }
            bool repeat = landBusiness.IsLandNumberReapet(currentLand.LandNumber, currentLand.ID, currentZone.FullCode);

            //此处做输入值判断   
            Dispatcher.Invoke(new Action(() =>
            {
                if (repeat)    //&& isAdd)
                {
                    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.LandNumberExist);
                    right = false;
                }
                else if (currentLand.ActualArea == 0)
                {
                    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.ActualAreaZero);
                    right = false;
                }
                else if (string.IsNullOrEmpty(currentLand.LandNumber))
                {
                    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.LandNumberEmpty);
                    right = false;
                }

                else if (dtCheckData.Value < dtSurveyData.Value)
                {
                    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.SurveyAndCheckDataError);
                    right = false;
                }
            }));
            return right;
        }

        /// <summary>
        /// 选择承包方
        /// </summary>
        private void cmbVirtualPersonName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsStockRight) return;
            txtOwnerName.Text = "";
            //currentPerson = cmbVirtualPersonName.SelectedItem as VirtualPerson;

            var spi = cmbVirtualPersonName.SelectedItem as SelectPersonInterface;
            if (spi != null)
            {
                currentPerson = VirtpersonList.Find(t => t.ID.ToString() == spi.ID);
            }

            if (currentPerson == null)
            {
                return;
            }
            List<VirtualPerson> listVirtualPerson = new List<VirtualPerson>();
            TheApp.Current.CreateTask(go =>
            {
                listVirtualPerson = personBusiness.GetByZone(currentZone.FullCode);
            },
                terminated => { },
                completed =>
                {
                    if (listVirtualPerson == null || listVirtualPerson.Count() == 0) return;
                    List<Person> listPerson = listVirtualPerson.Find(c => c.ID == currentPerson.ID).SharePersonList;
                    //count = listPerson.Count;
                    txtOwnerName.Text = listPerson.NameString();
                    listVirtualPerson = null;
                    GC.Collect();
                },
                started =>
                {
                    mbtnLandOK.IsEnabled = false;
                },
                ended =>
                {
                    mbtnLandOK.IsEnabled = true;
                }
                ).Start();
        }

        /// <summary>
        /// 选择一级土地利用类型
        /// </summary>
        private void cmbLandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
            {
                return;
            }
            string code = currentLandName.Code;
            var landNameSecond = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.Code.StartsWith(code) && c.Code.Length == 3 && c.GroupCode == currentLandName.GroupCode);
            cmbLandNameSecond.ItemsSource = landNameSecond;
            cmbLandNameSecond.DisplayMemberPath = "Name";
            cmbLandNameSecond.SelectedIndex = 0;
            if (!(bool)cbLandNameSecond.IsChecked)
            {
                txtLandNameAlise.Text = currentLandName.Name;
            }
        }

        /// <summary>
        /// 选择二级级土地利用类型
        /// </summary>
        private void cmbLandNameSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(bool)cbLandNameSecond.IsChecked)
                return;
            Dictionary currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
            if (currentLandNameSecond == null)
            {
                return;
            }
            if ((bool)cbLandNameAlise.IsChecked)
            {
                txtLandNameAlise.Text = currentLandNameSecond.Name;
            }
            else
            {
                txtLandNameAlise.Text = "其它";
            }
        }

        /// <summary>
        /// 是否选中二级地类
        /// </summary>
        private void cbLandNameSecond_Click(object sender, RoutedEventArgs e)
        {
            Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
                return;
            Dictionary currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
            bool chk = (bool)cbLandNameSecond.IsChecked;
            if ((bool)cbLandNameAlise.IsChecked)
            {
                if (!chk)
                {
                    cmbLandNameSecond.IsEnabled = false;
                    txtLandNameAlise.Text = currentLandName.Name;
                }
                else
                {
                    cmbLandNameSecond.IsEnabled = true;
                    txtLandNameAlise.Text = currentLandNameSecond.Name;
                }
            }
            else
            {
                if (!chk)
                {
                    cmbLandNameSecond.IsEnabled = false;
                    txtLandNameAlise.Text = "其它";
                }
                else
                {
                    cmbLandNameSecond.IsEnabled = true;
                    txtLandNameAlise.Text = "其它";
                }
            }
        }

        /// <summary>
        /// 是否选中地类名称
        /// </summary>
        private void cbLandNameAlise_Click(object sender, RoutedEventArgs e)
        {
            Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
                return;
            Dictionary currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
            bool isAliseChecked = (bool)cbLandNameAlise.IsChecked;
            if ((bool)cbLandNameSecond.IsChecked)
            {
                if (isAliseChecked)
                {
                    txtLandNameAlise.Text = currentLandNameSecond.Name;
                }
                else
                {
                    txtLandNameAlise.Text = "其它";
                }
            }
            else
            {
                if (isAliseChecked)
                {
                    txtLandNameAlise.Text = currentLandName.Name;
                }
                else
                {
                    txtLandNameAlise.Text = "其它";
                }
            }
        }

        /// <summary>
        /// 获取可用地块编码
        /// </summary>
        private void mbtnGetCode_Click(object sender, RoutedEventArgs e)
        {
            if (landBusiness == null)
                landBusiness = new AccountLandBusiness(dbContext);
            string number = landBusiness.GetNewLandNumber(currentZone.FullCode);
            this.currentLand.LandNumber = number;  //生成编码
            landBusiness = null;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg, eMessageGrade grade = eMessageGrade.Error)
        {
            if (Workpage == null)
            {
                return;
            }
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = grade,
                CancelButtonText = "取消",
            });
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            Workpage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            Workpage.Workspace.Message.Send(this, args);
        }

        #endregion

        #region 界址点信息

        #region 获取/绑定数据

        /// <summary>
        /// 异步获取界址点数据
        /// </summary>
        private void InitialDotControl(ContractLand land)
        {
            if (land == null)
                return;
            List<BuildLandBoundaryAddressDot> currentDots = null;
            List<BuildLandBoundaryAddressDot> currentDotsByCoil = null;
            KeyValueList<string, string> lstDictContentJBLX = null;
            KeyValueList<string, string> lstDictContentJZDLX = null;
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                IDictionaryWorkStation dictStation = null;
                IContractLandWorkStation landStation = null;
                IBuildLandBoundaryAddressDotWorkStation dotStation = null;
                IBuildLandBoundaryAddressCoilWorkStation coilStation = null;
                try
                {
                    var dbContext = DataBaseSource.GetDataBaseSource();
                    dictStation = dbContext.CreateDictWorkStation();
                    landStation = dbContext.CreateContractLandWorkstation();
                    dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                    coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                    currentDots = dotStation.GetByLandId(land.ID);
                    if (currentDots == null || currentDots.Count == 0)
                        return;
                    currentDots.Sort();
                    currentDotsByCoil = currentDots;

                    lstDictContentJBLX = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JBLX);
                    lstDictContentJZDLX = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZDLX);
                }
                catch { }
            }, null, null, started =>
            {
                DotDataItems.Clear();
                currentDots = new List<BuildLandBoundaryAddressDot>(50);
                currentDotsByCoil = new List<BuildLandBoundaryAddressDot>(50);
                lstDictContentJBLX = new KeyValueList<string, string>();
                lstDictContentJZDLX = new KeyValueList<string, string>();
                dotView.Roots = null;
                Workpage.Page.IsBusy = true;
            }, ended =>
            {
                Workpage.Page.IsBusy = false;
            }, completed =>
            {
                if (currentDots == null || currentDots.Count == 0)
                    return;
                //if (currentDotsByCoil.Count == 0)
                //{
                //    var orderResult = currentDots.OrderBy(r => int.Parse(r.DotNumber.Remove(0, 1))).ToList();
                //    orderResult.ForEach(c => DotDataItems.Add(c.ConvertToItem(lstDictContentJBLX, lstDictContentJZDLX)));
                //}
                //else
                //{
                currentDotsByCoil.ForEach(c => DotDataItems.Add(c.ConvertToItem(lstDictContentJBLX, lstDictContentJZDLX)));
                if (DotDataItems != null)
                    DotDataItems.FirstOrDefault().IsEditable = false;
                //}
                dotView.Roots = DotDataItems;
            }, null, terminated =>
            {
                ShowBox("承包地块界址点", "获取承包地块界址点失败");
                return;
            }).StartAsync();
        }

        #endregion

        #region Events

        /// <summary>
        /// 双击查看界址点
        /// </summary>
        private void dotView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ShowMapProperty != null && ShowMapProperty())
                SearchDot();
            else
                EditDot();
        }

        /// <summary>
        /// 右键菜单
        /// </summary>
        private void dotView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            dotMenu.IsOpen = true;
        }

        /// <summary>
        /// 右键刷新
        /// </summary>
        private void miRefreshDot_Click(object sender, RoutedEventArgs e)
        {
            InitialDotControl(CurrentLand);
        }

        /// <summary>
        /// 右键编辑
        /// </summary>
        private void miEditDot_Click(object sender, RoutedEventArgs e)
        {
            EditDot();
        }
        private void miStartDot_Click(object sender, RoutedEventArgs e)
        {
            var currtDot = dotView.SelectedItem as ConstructionLandDotItem;
            if (currtDot == null)
            {
                ShowBox("设置界址点", "请选择要设置为起点的界址点!");
                return;
            }
            var currentDot = currtDot.Entity.Clone() as BuildLandBoundaryAddressDot;
            if (!currentDot.IsValid)
            {
                ShowBox("设置界址点", "请选择有效界址点设为起点!");
                return;
            }
            var db = DataBaseSource.GetDataBaseSource();
            var dotStation = db.CreateBoundaryAddressDotWorkStation();
            var dictStation = dbContext.CreateDictWorkStation();
            var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>();
            List<BuildLandBoundaryAddressDot> currentDotsByCoil = new List<BuildLandBoundaryAddressDot>();
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>();

            KeyValueList<string, string> lstDictContentJBLX = null;
            KeyValueList<string, string> lstDictContentJZDLX = null;
            KeyValueList<string, string> lstDictContentJXXZ = null;
            KeyValueList<string, string> lstDictContentJZXLB = null;
            KeyValueList<string, string> lstDictContentJZXWZ = null;

            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                lstDictContentJBLX = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JBLX);
                lstDictContentJZDLX = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZDLX);
                if (currentDots == null || currentDots.Count == 0 || listCoil == null || listCoil.Count == 0)
                    return;
                lstDictContentJXXZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);
                lstDictContentJZXLB = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXLB);
                lstDictContentJZXWZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXWZ);

                string expression = @"[a-z]|[A-Z]";
                System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(expression);
                string dotNumber = pattern.Replace(currentDot.DotNumber, "");
                string strN = currentDot.DotNumber.Replace(dotNumber, "");
                int curNumber = int.Parse(dotNumber);
                foreach (var item in listDot)
                {
                    dotNumber = pattern.Replace(item.DotNumber, "");
                    int num = int.Parse(dotNumber);
                    if (num >= curNumber)
                        num = num - curNumber + 1;
                    else
                        num = listDot.Count - curNumber + num + 1;
                    strN = item.DotNumber.Replace(dotNumber, "");
                    item.DotNumber = strN + num.ToString();
                }
                BuildLandBoundaryAddressCoil cur = listCoil.Find(c => c.StartPointID.Equals(currentDot.ID));
                int orderNum = 0;
                if (cur != null)
                    orderNum = cur.OrderID;
                else
                    return;
                listCoil.ForEach(c =>
                {
                    c.StartNumber = listDot.Find(a => a.ID.Equals(c.StartPointID)).DotNumber;
                    c.EndNumber = listDot.Find(a => a.ID.Equals(c.EndPointID)).DotNumber;
                    dotNumber = pattern.Replace(c.StartNumber, "");
                    int ornum = c.OrderID;
                    if (ornum >= orderNum)
                        ornum = ornum - orderNum + 1;
                    else
                        ornum = listCoil.Count - orderNum + ornum + 1;
                    c.OrderID = short.Parse(ornum.ToString());
                });

            },
                null, null, started =>
                {
                    DotDataItems.Clear();
                    CoilDataItems.Clear();
                    listDot = dotStation.GetByLandID(currentDot.LandID);
                    listCoil = coilStation.GetByLandID(currentDot.LandID);
                    dotView.Roots = null;
                    coilView.Roots = null;

                }, ended => { },
                comleted =>
                {
                    listDot.Sort();
                    currentDotsByCoil = listDot;
                    currentDotsByCoil.ForEach(c => DotDataItems.Add(c.ConvertToItem(lstDictContentJBLX, lstDictContentJZDLX)));
                    if (DotDataItems != null)
                        DotDataItems.FirstOrDefault().IsEditable = false;
                    if (listCoil == null || listCoil.Count == 0)
                        return;
                    var orderCoils = listCoil.OrderBy(t => t.OrderID).ToList();
                    orderCoils.ForEach(c => CoilDataItems.Add(c.ConvertToItem(currentDots,
                        lstDictContentJXXZ, lstDictContentJZXLB, lstDictContentJZXWZ)));

                    dotView.Roots = DotDataItems;
                    coilView.Roots = CoilDataItems;
                    dotStation.UpdateRange(currentDotsByCoil);
                    listCoil.ForEach(c => coilStation.Update(c));
                }, terminated =>
                {
                }).StartAsync();
        }
        /// <summary>
        /// 界址点空间查看
        /// </summary>
        private void miSearchDot_Click(object sender, RoutedEventArgs e)
        {
            //if (SearchDot())
            //{
            //    if (OpenInWindow)
            //        Window.GetWindow(this).Close();
            //}
        }

        /// <summary>
        /// 空格按下
        /// </summary>
        private void dotView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Space)
            //    e.Handled = true;
            //var selectItem = dotView.SelectedItem as ConstructionLandDotItem;
            //if (selectItem == null)
            //    return;
            //bool isValid = selectItem.IsValidUI;
            //if (isValid)
            //    selectItem.IsValidUI = false;
            //else
            //    selectItem.IsValidUI = true;
        }

        #endregion

        #region Method

        /// <summary>
        /// 编辑界址点
        /// </summary>
        public void EditDot()
        {
            var currentDot = dotView.SelectedItem as ConstructionLandDotItem;
            if (currentDot == null)
            {
                ShowBox("编辑界址点", "请选择要编辑的界址点!");
                return;
            }
            ConstructionLandDotPage dotPage = new ConstructionLandDotPage();
            dotPage.Workpage = Workpage;
            dotPage.CurrentDot = currentDot.Entity.Clone() as BuildLandBoundaryAddressDot;
            dotPage.IsLock = this.IsLock;
            Workpage.Page.ShowMessageBox(dotPage, (b, r) =>
            {
                if (!(bool)b)
                    return;
                int oldDotIndex = DotDataItems.IndexOf(currentDot);
                if (oldDotIndex >= 0)
                    DotDataItems.Remove(currentDot);
                var newDotItem = dotPage.CurrentDotItem;
                DotDataItems.Insert(oldDotIndex, newDotItem);
                dotView.BringIntoView(oldDotIndex);
            });
        }

        /// <summary>
        /// 空间查看界址点
        /// </summary>
        public bool SearchDot()
        {
            try
            {
                var currentDot = dotView.SelectedItem as ConstructionLandDotItem;
                if (currentDot == null || currentDot.Entity == null)
                {
                    ShowBox("界址点空间查看", "请选择查看的数据项");
                    return false;
                }
                if (currentDot.Entity.Shape == null)
                {
                    ShowBox("界址点空间查看", "当前选择项无空间数据");
                    return false;
                }
                //陈泽林 20161205
                if (layerHover == null)
                    return false;
                layerHover.Graphics.Clear();
                pointgraphic.Geometry = Spatial.Geometry.CreatePoint(new Coordinate(double.Parse(currentDot.XCoordinateUI), double.Parse(currentDot.YCoordinateUI)), 0);
                switch (pointgraphic.Geometry.GeometryType)
                {
                    case YuLinTu.Spatial.eGeometryType.Point:
                    case YuLinTu.Spatial.eGeometryType.MultiPoint:
                        pointgraphic.Symbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as UISymbol;
                        break;
                    default:
                        return false;
                }
                layerHover.Graphics.Add(pointgraphic);
                //Workpage.Message.Send(this, MessageExtend.ContractAccountMsg(DataBaseSource.GetDataBaseSource(), ContractAccountMessage.CONTRACTACCOUNT_FINDDOT_COMPLETE, currentDot.Entity));
                return true;
            }
            catch (Exception ex)
            {
                Log.Log.WriteException(this, "SearchDot(界址点查看)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        #endregion

        #endregion

        #region 界址线信息

        #region Field

        private List<BuildLandBoundaryAddressDot> currentDots = null;  //当前地块的所有界址点


        #endregion

        #region 获取/绑定数据

        /// <summary>
        /// 异步获取界址线数据
        /// </summary>
        private void InitialCoilControl(ContractLand land)
        {
            if (land == null)
                return;
            List<BuildLandBoundaryAddressCoil> currentCoils = null;
            KeyValueList<string, string> lstDictContentJXXZ = null;
            KeyValueList<string, string> lstDictContentJZXLB = null;
            KeyValueList<string, string> lstDictContentJZXWZ = null;
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                IDictionaryWorkStation dictStation = null;
                IContractLandWorkStation landStation = null;
                IBuildLandBoundaryAddressDotWorkStation dotStation = null;
                IBuildLandBoundaryAddressCoilWorkStation coilStation = null;
                try
                {
                    var dbContext = DataBaseSource.GetDataBaseSource();
                    dictStation = dbContext.CreateDictWorkStation();
                    landStation = dbContext.CreateContractLandWorkstation();
                    dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                    coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                    currentCoils = coilStation.GetByLandId(land.ID);
                    currentDots = dotStation.GetByLandId(land.ID);
                    if (currentCoils == null || currentCoils.Count == 0)
                        return;
                    lstDictContentJXXZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JXXZ);
                    lstDictContentJZXLB = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXLB);
                    lstDictContentJZXWZ = dictStation.GetCodeNameByGroupCode(DictionaryTypeInfo.JZXWZ);
                }
                catch { }
            }, null, null, started =>
            {
                CoilDataItems.Clear();
                currentCoils = new List<BuildLandBoundaryAddressCoil>(50);
                currentDots = new List<BuildLandBoundaryAddressDot>(50);
                lstDictContentJXXZ = new KeyValueList<string, string>();
                lstDictContentJZXLB = new KeyValueList<string, string>();
                lstDictContentJZXWZ = new KeyValueList<string, string>();
                coilView.Roots = null;
                Workpage.Page.IsBusy = true;
            }, ended =>
            {
                Workpage.Page.IsBusy = false;
            }, completed =>
            {
                if (currentCoils == null || currentCoils.Count == 0)
                    return;
                var orderCoils = currentCoils.OrderBy(t => t.OrderID).ToList();
                orderCoils.ForEach(c => CoilDataItems.Add(c.ConvertToItem(currentDots,
                    lstDictContentJXXZ, lstDictContentJZXLB, lstDictContentJZXWZ)));
                coilView.Roots = CoilDataItems;
            }, null, terminated =>
            {
                ShowBox("承包地块界址线", "获取承包地块界址线失败");
                return;
            }).StartAsync();
        }

        #endregion

        #region Events

        /// <summary>
        /// 双击查看界址线
        /// </summary>
        private void coilView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //EditCoil();
            if (ShowMapProperty != null && ShowMapProperty())
                SearchCoil();
            else
                EditCoil();
        }

        /// <summary>
        /// 右键菜单
        /// </summary>
        private void coilView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            coilMenu.IsOpen = true;
        }

        /// <summary>
        /// 右键编辑
        /// </summary>
        private void miEditCoil_Click(object sender, RoutedEventArgs e)
        {
            EditCoil();
        }

        /// <summary>
        /// 右键刷新
        /// </summary>
        private void miRefreshCoil_Click(object sender, RoutedEventArgs e)
        {
            InitialCoilControl(CurrentLand);
        }

        /// <summary>
        /// 左键单击—空间查看
        /// </summary>
        private void coilView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //SearchCoil();
        }

        /// <summary>
        /// 界址线空间查看
        /// </summary>
        private void miSearchCoil_Click(object sender, RoutedEventArgs e)
        {
            //if (SearchCoil())
            //{
            //    if (OpenInWindow)
            //        Window.GetWindow(this).Close();
            //}
        }

        private void DoubleUpDownTableArea_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsStockRight)
            {
                StockRight();
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// 编辑界址线
        /// </summary>
        public void EditCoil()
        {
            ConstructionLandCoilItem currentCoil = null;
            currentCoil = coilView.SelectedItem as ConstructionLandCoilItem;
            if (currentCoil == null)
            {
                ShowBox("编辑界址线", "请选择要编辑的界址线!");
                return;
            }
            ConstructionLandCoilPage coilPage = new ConstructionLandCoilPage();
            coilPage.Workpage = Workpage;
            coilPage.ListDot = currentDots; //当前地块的所有界址点
            coilPage.IsLock = this.IsLock;
            coilPage.CurrentCoil = currentCoil.Entity.Clone() as BuildLandBoundaryAddressCoil;
            Workpage.Page.ShowMessageBox(coilPage, (b, r) =>
            {
                if (!(bool)b)
                    return;
                int oldCoilIndex = CoilDataItems.IndexOf(currentCoil);
                if (oldCoilIndex >= 0)
                    CoilDataItems.Remove(currentCoil);
                var newCoilItem = coilPage.CurrentCoilItem;
                CoilDataItems.Insert(oldCoilIndex, newCoilItem);
                coilView.BringIntoView(oldCoilIndex);
            });
        }

        /// <summary>
        /// 空间查看界址线
        /// </summary>
        public bool SearchCoil()
        {
            try
            {
                ConstructionLandCoilItem currentCoil = null;
                currentCoil = coilView.SelectedItem as ConstructionLandCoilItem;
                if (currentCoil == null)
                {
                    ShowBox("界址线空间查看", "请选择查看的数据项");
                    return false;
                }
                if (currentCoil.Entity.Shape == null)
                {
                    ShowBox("界址线空间查看", "当前选择项无空间数据");
                    return false;
                }
                if (IsMapOpen(Workpage))
                {
                    if (layerHover == null)
                        return false;
                    layerHover.Graphics.Clear();
                    linegraphic.Geometry = currentCoil.Entity.Shape;
                    switch (linegraphic.Geometry.GeometryType)
                    {
                        case YuLinTu.Spatial.eGeometryType.Polyline:
                        case YuLinTu.Spatial.eGeometryType.MultiPolyline:
                            linegraphic.Symbol = Application.Current.TryFindResource("UISymbol_Line_Measure") as LineSymbol;
                            break;
                        default:
                            break;
                    }
                    layerHover.Graphics.Add(linegraphic);

                    //Workpage.Workspace.Message.Send(this, new MsgEventArgs()
                    //{
                    //    Name = "8A7DA576-FFF8-4BC0-8332-1B337A336502",
                    //    Parameter = new ArrayList { "ZD_JSYD_JZX", currentCoil.Entity.ID },//.OBJECTID
                    //    Tag = Workpage,
                    //});
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Log.WriteException(this, "SearchCoil(界址线查看)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 地图是否打开
        /// </summary>
        /// <returns></returns>
        private bool IsMapOpen(IWorkpage workpage)
        {
            var mapPage = workpage.Workspace.Workpages.FirstOrDefault(
                c =>
                {
                    var s = c.Page.Content.GetType().ToString();
                    if (s == "YuLinTu.Component.MapFoundation.YuLinTuMapFoundation")
                        return true;
                    return false;
                });
            if (mapPage == null)
            {
                //此时当前工作空间中没有打开鱼鳞图地图模块(工作页)
                //ShowBox("空间查看", "未打开不动产权籍图模块,请打开后再做空间查看操作!");
                //workpage.Workspace.AddWorkpage

                return false;
            }
            mapPage.Page.Activate();   //激活鱼鳞图地图工作页
            return true;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 选择人员对象
    /// </summary>
    public class SelectPersonInterface
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
