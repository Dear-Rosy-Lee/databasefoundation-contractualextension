/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.NetAux;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地图管理界面
    /// </summary>
    public partial class MapFoundationManagerPage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public MapFoundationManagerPage(IWorkpage page, ContractLand curLand)
        {
            InitializeComponent();
            Workpage = page;
            layerControl.Map = MapControl;
            CurrentLand = curLand;
            reference = curLand.Shape.SpatialReference;
            CoordinateItems = new ObservableCollection<MapCoordinateItem>();
            pointView.Roots = CoordinateItems;
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            mapControl.Action = MapActions.SimpleSelect;
            OnInitialMapControl();
        }

        #endregion

        #region Field

        private IDbContext db;
        private MapCoordinateItem curCoordinateItem;
        private Spatial.SpatialReference reference;
        private bool isCCW;

        #endregion

        #region Properties

        /// <summary>
        /// 地图控件
        /// </summary>
        public MapControl MapControl { get { return mapControl; } }

        /// <summary>
        /// 当前界面选择地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        /// <summary>
        /// 承包地块坐标点集合
        /// </summary>
        public ObservableCollection<MapCoordinateItem> CoordinateItems { get; set; }

        /// <summary>
        /// 坐标点集合
        /// </summary>
        public GraphicsLayer GpLayer { get; set; }

        /// <summary>
        /// 排序后的坐标点集合
        /// </summary>
        public KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> OrderKvCoords { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void OnInitialMapControl()
        {
            db = DataBaseSource.GetDataBaseSource();
            if (db == null)
            {
                Workpage.Page.IsBusy = false;
                return;
            }
            VectorLayer cbdLayer = new VectorLayer(new SQLiteGeoSource(db.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { GeometryType = YuLinTu.Spatial.eGeometryType.Polygon });
            cbdLayer.Name = "承包地";
            cbdLayer.SetIsManual(true);
            cbdLayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            cbdLayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };
            cbdLayer.Labeler.Enabled = true;
            if (MapControl == null)
            {
                Workpage.Page.IsBusy = false;
                return;
            }
            if (cbdLayer == null)
            {
                Workpage.Page.IsBusy = false;
                return;
            }
            mapControl.Layers.Add(cbdLayer);
            Envelope en = CurrentLand.Shape.GetEnvelope();
            if (en == null)
            {
                Workpage.Page.IsBusy = false;
                return;
            }
            MapControl.ApplyTemplate();
            mapControl.Extend = en;

            Graphic gp = new Graphic()
            {
                Layer = cbdLayer,
                Object = new FeatureObject() { Geometry = CurrentLand.Shape },
                Geometry = CurrentLand.Shape,
            };
            mapControl.SelectedItems.Add(gp);

            GetCoordinateItems();
            SetCoordSelected();
        }

        /// <summary>
        /// 获取坐标点集合
        /// </summary>
        private void GetCoordinateItems()
        {
            if (CurrentLand == null || CurrentLand.Shape == null)
                return;
            var kvCoordList = CurrentLand.Shape.GetWNOrderCoordinates();
            if (kvCoordList != null)
            {
                isCCW = kvCoordList.Key;
                kvCoordList.Value = kvCoordList.Value.Distinct().ToList();
            }
            var coords = kvCoordList.Value.ToArray();
            for (int i = 0; i < coords.Length; i++)
            {
                MapCoordinateItem item = new MapCoordinateItem
                {
                    OrderID = (i + 1).ToString(),
                    XCoordinate = (coords[i].X).ToString("0.0000"),
                    YCoordinate = (coords[i].Y).ToString("0.0000"),
                    CoordEntity = coords[i],
                };
                CoordinateItems.Add(item);
            }
        }

        /// <summary>
        /// 设置坐标点高亮显示
        /// </summary>
        private void SetCoordSelected()
        {
            if (curCoordinateItem == null || curCoordinateItem.CoordEntity == null)
                return;
            if (!mapControl.InternalLayers.Contains(GpLayer))
                mapControl.InternalLayers.Add(GpLayer);
            GpLayer.Graphics.Clear();
            double x = curCoordinateItem.CoordEntity.X;
            double y = curCoordinateItem.CoordEntity.Y;
            Coordinate cdt = new Coordinate(x, y);
            var geoPoint = Spatial.Geometry.CreatePoint(cdt, reference);

            var gc = new Graphic();
            gc.Layer = GpLayer;
            gc.Geometry = geoPoint;
            gc.Symbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as UISymbol;
            GpLayer.Graphics.Add(gc);
        }

        /// <summary>
        /// 定位到指定坐标点
        /// </summary>
        private void NavigateToCoord()
        {
            if (curCoordinateItem == null || curCoordinateItem.CoordEntity == null)
                return;
            double x = curCoordinateItem.CoordEntity.X;
            double y = curCoordinateItem.CoordEntity.Y;
            Coordinate cdt = new Coordinate(x, y);
            var geoPoint = Spatial.Geometry.CreatePoint(cdt, reference);
            mapControl.PanTo(geoPoint);
        }

        /// <summary>
        /// 获取排好序的坐标点集合
        /// </summary>
        private void GetOrderPoint()
        {
            if (curCoordinateItem == null || CoordinateItems == null)
                return;
            int count = CoordinateItems.Count;
            int index = CoordinateItems.IndexOf(curCoordinateItem);

            List<GeoAPI.Geometries.Coordinate> orderCoords = new List<GeoAPI.Geometries.Coordinate>();
            for (int i = index; i < count; i++)
                orderCoords.Add(CoordinateItems[i].CoordEntity);
            for (int j = 0; j < index; j++)
                orderCoords.Add(CoordinateItems[j].CoordEntity);
            OrderKvCoords = new KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> { Key = isCCW, Value = orderCoords };
        }

        #endregion

        #region Events

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            mapControl.InternalLayers.Remove(GpLayer);
            GpLayer.Dispose();

            GetOrderPoint();
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 坐标点选择项改变
        /// </summary>
        private void pointView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curCoordinateItem = pointView.SelectedItem as MapCoordinateItem;
            SetCoordSelected();
        }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        private void pointView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            curCoordinateItem = pointView.SelectedItem as MapCoordinateItem;
            NavigateToCoord();
        }

        #endregion
    }
}
