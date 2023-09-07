/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.WorkStation;
using System.IO;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 初始化有效界址线界面
    /// </summary>
    public partial class ContractLandInitializeCoil : InfoPageBase
    {
        public ContractLandInitializeCoil(IWorkpage page)
        {
            InitializeComponent();
            listDict = new List<Dictionary>();
            dbContext = DataBaseSource.GetDataBaseSource();

            cmbAddressPointPrefix.Text = "J";
            wtAddressLinedbiDistance.Text = "1.5";
            this.Workpage = page;
            cbLineDescription.IsChecked = true;
            IsLineDescription = true;
            cbIsNeighborExportVillageLevel.IsChecked = true;          
        }

        #region Fields

        private List<Dictionary> listDict;
        private IDbContext dbContext;
        private string addressLineType;
        private string addressLineCatalog;
        private string addressLinePosition;
        private double addressLinedbiDistance;
        private string addressPointPrefix;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 界线性质
        /// </summary>
        public string AddressLineType
        {
            get { return addressLineType; }
            set { addressLineType = value; }
        }

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string AddressLineCatalog
        {
            get { return addressLineCatalog; }
            set { addressLineCatalog = value; }
        }

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string AddressLinePosition
        {
            get { return addressLinePosition; }
            set { addressLinePosition = value; }
        }

        /// <summary>
        /// 领宗地距离
        /// </summary>
        public double AddressLinedbiDistance
        {
            get { return addressLinedbiDistance; }
            set { addressLinedbiDistance = value; }
        }

        /// <summary>
        /// 界址标识
        /// </summary>
        public string AddressPointPrefix
        {
            get { return addressPointPrefix; }
            set { addressPointPrefix = value; }
        }

        /// <summary>
        /// 全域统编
        /// </summary>
        public bool IsUnit { set; get; }

        /// <summary>
        /// 自动识别界址线位置
        /// </summary>
        public bool IsPostion { set; get; }

        /// <summary>
        /// 查找毗邻承包方
        /// </summary>
        public bool IsNeighbor { set; get; }

        /// <summary>
        /// 界址线说明填写长度
        /// </summary>
        public bool IsLineDescription { set; get; }

        /// <summary>
        /// 是否按照角度进行界址点过滤
        /// </summary>
        //public bool IsAngleFilter { get; set; }

        /// <summary>
        /// 是否按照毗邻承包方进行界址点过滤
        /// </summary>
        //public bool IsVirtualPersonFilter { get; set; }

        /// <summary>
        /// 是否过滤界址点 
        /// 修改于2016.7.15
        /// </summary>
        public bool IsFilterDot { get; set; }

        /// <summary>
        /// 查找毗邻承包方到村
        /// </summary>
        public bool IsNeighborExportVillageLevel { set; get; }

        /// <summary>
        /// 最小过滤角度值
        /// </summary>
        public double? MinAngleFileter { get; set; }

        /// <summary>
        /// 最大过滤角度值
        /// </summary>
        public double? MaxAngleFilter { get; set; }

        /// <summary>
        /// 是否全部地块初始化
        /// </summary>
        public bool IsAllLands { get; set; }

        /// <summary>
        /// 是否只初始化没有界址信息的地块
        /// </summary>
        public bool IsLandsWithoutInfo { get; set; }

        /// <summary>
        /// 是否只初始化选择的地块
        /// </summary>
        public bool IsSelectedLands { get; set; }

        /// <summary>
        /// 所选中的承包方信息
        /// </summary>
        public List<VirtualPerson> SelectedObligees { get; set; }

        #endregion

        #region Methods-Override

        /// <summary>
        /// 初始化控件开始
        /// </summary>
        protected override void OnInitializeGo()
        {
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            listDict = dictBusiness.GetAll();    //获得数据字典里的内容
        }

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {        
            //初始化界线性质
            var AddressLineTypeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JXXZ && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLineType.ItemsSource = AddressLineTypeList;
            cmbAddressLineType.DisplayMemberPath = "Name";
            cmbAddressLineType.SelectedIndex = 0;

            //初始化界址线类型
            var AddressLineCatalogList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JZXLB && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLineCatalog.ItemsSource = AddressLineCatalogList;
            cmbAddressLineCatalog.DisplayMemberPath = "Name";
            cmbAddressLineCatalog.SelectedIndex = 0;

            //初始化界址线位置
            var AddressLinePositionList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JZXWZ && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLinePosition.ItemsSource = AddressLinePositionList;
            cmbAddressLinePosition.DisplayMemberPath = "Name";
            cmbAddressLinePosition.SelectedIndex = 0;
        }

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (wtAddressLinedbiDistance.Value == null)
            {
                ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.NeighborDistanceValueError);
                return;
            }
           
            addressLinedbiDistance = wtAddressLinedbiDistance.Value.Value;
            addressPointPrefix = cmbAddressPointPrefix.Text;    
            IsNeighbor = (bool)cbNeighbor.IsChecked;
           
            IsNeighborExportVillageLevel = (bool)cbIsNeighborExportVillageLevel.IsChecked;
            IsAllLands = (bool)rbAllLands.IsChecked;
            IsLandsWithoutInfo = (bool)rbLandsWithoutInfo.IsChecked;
            IsSelectedLands = (bool)rbSelectedLands.IsChecked;
            if (IsSelectedLands)
            {
                bool handleResult = HandleSelectObligees();
                if (handleResult)
                    Workpage.Page.CloseMessageBox(true);
            }
            else
            {
                Workpage.Page.CloseMessageBox(true);
            }
        }

        /// <summary>
        /// 界线性质改变
        /// </summary>
        private void cmbAddressLineType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddressLineType.SelectedItem == null)
                return;
            addressLineType = (cmbAddressLineType.SelectedItem as Dictionary).Name;
            if (addressLineType == null)
            {
                return;
            }
        }

        /// <summary>
        /// 界址线类型改变
        /// </summary>
        private void cmbAddressLineCatalog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddressLineCatalog.SelectedItem == null)
                return;
            addressLineCatalog = (cmbAddressLineCatalog.SelectedItem as Dictionary).Name;
            if (addressLineCatalog == null)
            {
                return;
            }
        }

        /// <summary>
        /// 界址线位置改变
        /// </summary>
        private void cmbAddressLinePosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddressLinePosition.SelectedItem == null)
                return;
            addressLinePosition = (cmbAddressLinePosition.SelectedItem as Dictionary).Name;
            if (addressLinePosition == null)
            {
                return;
            }
        }

        /// <summary>
        /// 自动识别界址线位置
        /// </summary>      
        private void cbPostion_Click(object sender, RoutedEventArgs e)
        {
            IsPostion = cbPostion.IsChecked.Value ? true : false;
            cmbAddressLinePosition.IsEnabled = cbPostion.IsChecked.Value ? false : true;
        }

        /// <summary>
        /// 查找毗邻承包方
        /// </summary>       
        private void cbNeighbor_Click(object sender, RoutedEventArgs e)
        {
            IsNeighbor = cbNeighbor.IsChecked.Value ? true : false;
            if (!IsNeighbor) cbIsNeighborExportVillageLevel.IsChecked = false;
        }

        /// <summary>
        /// 查找毗邻承包方到村
        /// </summary> 
        private void cbIsNeighborExportVillageLevel_Click(object sender, RoutedEventArgs e)
        {
            IsNeighborExportVillageLevel = cbIsNeighborExportVillageLevel.IsChecked.Value ? true : false;
            if (IsNeighborExportVillageLevel)
            {
                cbNeighbor.IsChecked = true;
            }
        }

        /// <summary>
        /// 界址线说明填写长度
        /// </summary>      
        private void cbLineDescription_Click(object sender, RoutedEventArgs e)
        {
            IsLineDescription = cbLineDescription.IsChecked.Value ? true : false;
        }

        /// <summary>
        /// 选择地块
        /// </summary>
        private void sbSelectLands_Click(object sender, RoutedEventArgs e)
        {
            List<VirtualPerson> listObligee = null;
            IVirtualPersonWorkStation<LandVirtualPerson> personStaion = null;
            string msg = string.Format("在{0}未获取地块所属承包方信息!", CurrentZone.FullName);
            TaskThreadDispatcher.Create(Dispatcher, go =>
            {
                personStaion = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                listObligee = personStaion.GetByZoneCode(CurrentZone.FullCode);
            }, null, null, started =>
            {
                Workpage.Page.IsBusy = true;
            }, ended =>
            {
            }, comleted =>
            {
                if (listObligee == null || listObligee.Count == 0)
                {
                    ShowBox("地块选择", msg);
                    Workpage.Page.IsBusy = false;
                    return;
                }
                LandSelectedPage selectPage = new LandSelectedPage(Workpage);
                selectPage.ListObligee = CDObject.TryClone(listObligee) as List<VirtualPerson>;
                Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                {
                    if (!(bool)b)
                        return;
                    SelectedObligees = selectPage.ListObligee;
                });
                Workpage.Page.IsBusy = false;
            }, null, terminated =>
            {
                ShowBox("地块选择", msg);
                Workpage.Page.IsBusy = false;
                return;
            }).StartAsync();
        }

        #endregion

        #region Helper

        /// <summary>
        /// 根据配置文件处理选择承包方
        /// </summary>
        private bool HandleSelectObligees()
        {
            if (SelectedObligees != null && SelectedObligees.Count > 0)
                return true;
            var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            List<VirtualPerson> listObligee = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Config\SelectObligees.xml";
            if (!File.Exists(filePath))
            {
                ShowBox("地块选择", "请至少选择一个地块所属承包方信息");
                return false;
            }
            var obligeesTemp = YuLinTu.Library.Business.ToolSerialization.DeserializeXml(filePath, typeof(List<SelectObligee>)) as List<SelectObligee>;
            if (obligeesTemp != null)
            {
                var obligees = new List<VirtualPerson>(obligeesTemp.Count);
                obligeesTemp.ForEach(t =>
                {
                    if (!t.Status)
                        return;
                    var find = listObligee.Find(c => c.ID == t.ID);
                    if (find == null)
                        return;
                    obligees.Add(find);
                });
                if (obligees.Count == 0)
                {
                    ShowBox("地块选择", "请至少选择一个地块所属承包方信息");
                    return false;
                }
                SelectedObligees = CDObject.TryClone(obligees) as List<VirtualPerson>;
                obligees.Clear();
                obligees = null;
            }
            return true;
        }

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region 单个地块初始化处理(针对承包台账模块)

        #region Field

        #endregion

        #region Properties

        /// <summary>
        /// 当前界面选择地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 选择地块初始起点按钮
        /// </summary>
        private void sbCurrentLand_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentLand == null || CurrentLand.Shape == null)
            {
                ShowBox("单地块生成化界址信息", "请选择目标空间地块进行初始化界址信息操作!");
                return;
            }
            MapFoundationManagerPage mapPage = new MapFoundationManagerPage(Workpage, CurrentLand);
            mapPage.GpLayer = new tGIS.Client.GraphicsLayer();
            Workpage.Page.ShowMessageBox(mapPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    if (mapPage.mapControl.InternalLayers.Contains(GpLayer))
                    {
                        mapPage.GpLayer.Graphics.Clear();
                        mapPage.MapControl.InternalLayers.Remove(GpLayer);
                        GpLayer.Dispose();
                    }
                    return;
                }
                ListOrderCoord = mapPage.OrderKvCoords;
            });
        }

        #endregion

        #endregion

        #region 单个地块初始化处理(针对鱼鳞图模块)

        #region Fields

        private Spatial.Geometry currentGeoLand;
        private Spatial.SpatialReference reference;
        //private KeyValueList<double, GeoAPI.Geometries.Coordinate> indexCoordList;
        //private KeyValue<double, GeoAPI.Geometries.Coordinate> selectedKv;
        private const double precison = 0.0001;

        #endregion

        #region Property

        /// <summary>
        /// 地图控件
        /// </summary>
        public tGIS.Client.MapControl MapControl { get; set; }

        /// <summary>
        /// 临时图形图层
        /// </summary>
        public YuLinTu.tGIS.Client.GraphicsLayer GpLayer { get; set; }

        /// <summary>
        /// 根据所选起点排好序的坐标点集合
        /// </summary>
        public KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> ListOrderCoord { get; set; }

        /// <summary>
        /// 当前空间承包地块
        /// </summary>
        public Spatial.Geometry CurrentGeoLand
        {
            get { return currentGeoLand; }
            set
            {
                currentGeoLand = value;
                var tempKv = currentGeoLand.GetWNOrderCoordinates();
                if (tempKv != null)
                    ListOrderCoord = new KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> { Key = tempKv.Key, Value = tempKv.Value.Distinct().ToList() };
                reference = currentGeoLand.SpatialReference;
            }
        }

        #endregion

        #endregion

        #region Method - 单个地块处理

        /// <summary>
        /// 获取参数
        /// </summary>
        public TaskInitializeLandCoilArgument GetArgument()
        {
            TaskInitializeLandCoilArgument meta = new TaskInitializeLandCoilArgument();
            meta.CurrentZone = CurrentZone;
            meta.Database = dbContext;           
            meta.AddressLineCatalog = AddressLineCatalog;
            meta.AddressLinedbiDistance = AddressLinedbiDistance;
            meta.AddressLinePosition = AddressLinePosition;
            meta.AddressLineType = AddressLineType;
            meta.AddressPointPrefix = AddressPointPrefix;
            meta.IsLineDescription = IsLineDescription;
            meta.IsNeighbor = IsNeighbor;
            meta.IsPostion = IsPostion;
            meta.IsNeighborExportVillageLevel = IsNeighborExportVillageLevel;
            meta.IsUnit = IsUnit;
            meta.IsFilterDot = IsFilterDot;
            meta.MinAngleFileter = MinAngleFileter;
            meta.MaxAngleFilter = MaxAngleFilter;
            meta.IsSingleLand = true;
            meta.SingleLand = CurrentLand;
            meta.OrderCoordsKv = ListOrderCoord;
            return meta;
        }

        #endregion
    }
}
