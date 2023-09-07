using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using YuLinTu.Data;
using YuLinTu.Diagrams;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows.Wpf;
using Microsoft.Win32;
using YuLinTu.NetAux.CglLib;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.ContractAccount
{
    public partial class EditLandParcelDialog : MetroDialog, IDisposable
    {
        #region Properties

        public ContractLand SelectedLand
        {
            get { return selectedLand; }
            set { selectedLand = value; }
        }

        /// <summary>
        /// 有效界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListValidDot { get; set; }

        public ContractBusinessParcelWordSettingDefine SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
        #endregion

        #region Fields

        private IDbContext DbContext = null;
        private DiagramsView ViewOfNeighorParcels = null;
        private List<ContractLand> ListGeoLand = null;
        private List<ContractLand> geoLandCollection;
        private List<XZDW> ListLineFeature;
        private List<DZDW> ListPointFeature;
        private List<MZDW> ListPolygonFeature;
        private double mapW = 160;//初始化仅一次250
        private double mapH = 150;//240
        private ContractLand selectedLand;
        private ExportLandParcelMainOperation exportLandParcelMainOperation;

        #endregion

        #region Ctor

        public EditLandParcelDialog()
        {
            InitializeComponent();

            DataContext = this;

            ViewOfNeighorParcels = new DiagramsView();

            ViewOfNeighorParcels.Paper.Model.BorderWidth = 1;
            ViewOfNeighorParcels.Paper.Model.BorderColor = Color.FromArgb(255, 255, 255, 255);
            ViewOfNeighorParcels.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            Grid.SetRow(ViewOfNeighorParcels.ScrollViewer, 1);
            Grid.SetColumn(ViewOfNeighorParcels.ScrollViewer, 1);
            grid.Children.Add(ViewOfNeighorParcels.ScrollViewer);

            var binding = new Binding("Action");
            binding.Source = ViewOfNeighorParcels;
            binding.Converter = new ActionToSelectButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            btnSelect.SetBinding(SuperButton.IsCheckedProperty, binding);

            dp.Install(ViewOfNeighorParcels);
            exportLandParcelMainOperation = new ExportLandParcelMainOperation();

        }

        #endregion

        #region Methods

        protected override void OnInitializeStarted()
        {
            if (ViewOfNeighorParcels.Action is DiagramsViewActionSelect)
                return;

            ViewOfNeighorParcels.Action = new DiagramsViewActionSelect(null);
        }

        protected override void OnInitializeGo()
        {
            mapW = 190;
            mapH = 180;

            //if (SettingDefine.IsFixedLandGeoWordExtend)
            //{
            //    mapW = 190;
            //    mapH = 180;
            //}
            //else
            //{
            //    mapW = SettingDefine.LandGeoWordWidth;
            //    mapH = SettingDefine.LandGeoWordHeight;
            //}
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ViewOfNeighorParcels.Paper.Model.Width = mapW;
                    ViewOfNeighorParcels.Paper.Model.Height = mapH + 10;
                }));
            exportLandParcelMainOperation.mapH = mapH;
            exportLandParcelMainOperation.mapW = mapW;
            
            var visibleBounds = new CglEnvelope(0, 0, mapW, mapH);

            DbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            var landWork = DbContext.CreateContractLandWorkstation();
            var lineStation = DbContext.CreateXZDWWorkStation();
            var PointStation = DbContext.CreateDZDWWorkStation();
            var PolygonStation = DbContext.CreateMZDWWorkStation();
            ListGeoLand = landWork.GetCollection(SelectedLand.LocationCode, eLevelOption.Self).Where(c => c.Shape != null).ToList();
            geoLandCollection = ListGeoLand.FindAll(c => c.OwnerId == SelectedLand.OwnerId);
            ListLineFeature = lineStation.GetByZoneCode(SelectedLand.LocationCode);
            ListPointFeature = PointStation.GetByZoneCode(SelectedLand.LocationCode);
            ListPolygonFeature = PolygonStation.GetByZoneCode(SelectedLand.LocationCode);
            ListLineFeature.RemoveAll(l => l.Shape == null);
            ListPointFeature.RemoveAll(l => l.Shape == null);
            ListPolygonFeature.RemoveAll(l => l.Shape == null);
            exportLandParcelMainOperation.ListLineFeature = ListLineFeature;
            exportLandParcelMainOperation.ListPointFeature = ListPointFeature;
            exportLandParcelMainOperation.ListPolygonFeature = ListPolygonFeature;

            var dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
            ListValidDot = dotStation.GetByLandID(selectedLand.ID, true);
            var landStation = DbContext.CreateContractLandWorkstation();
            MapShapeUI map = null;
            List<FeatureObject> listFeature = new List<FeatureObject>();

            var targetSpatialReference = DbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(ContractLand)).Schema,
                ObjectContext.Create(typeof(ContractLand)).TableName);

            exportLandParcelMainOperation.targetSpatialReference = targetSpatialReference;
            //各个空间地块的邻宗地图导出
            
            foreach (var geoLand in geoLandCollection)
            {
                if (geoLand.ID != SelectedLand.ID)
                    continue;

                DiagramBase diagram = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ViewOfNeighorParcels == null)
                    {
                        ViewOfNeighorParcels = new DiagramsView();
                        ViewOfNeighorParcels.Paper.Model.Width = mapW;
                        ViewOfNeighorParcels.Paper.Model.Height = mapH + 10;
                    }
                    if (ViewOfNeighorParcels.Items.Count == 0)
                    {
                        diagram = new MapShape() { CanMove = false }.CreateDiagram();
                        ViewOfNeighorParcels.Items.Add(diagram);
                        diagram.Model.Width = mapW;
                        diagram.Model.Height = mapH;
                        diagram.Model.BorderWidth = 0;
                        diagram.Model.X = 1;
                        diagram.Model.Y = 1;
                        diagram.Tag = true;
                    }
                    else
                    {
                        diagram = ViewOfNeighorParcels.Items[0];
                        diagram.Model.Width = mapW;
                        diagram.Model.Height = mapH;
                        diagram.Model.BorderWidth = 0;
                        diagram.Model.X = 1;
                        diagram.Model.Y = 1;
                        diagram.Tag = true;
                    }
                    map = diagram.Content as MapShapeUI;
                }));

                //首先创建邻宗图层                  
                YuLinTu.Spatial.Geometry geolandbuffershape = null;
                if (geoLand.Shape != null && geoLand.Shape.IsValid())
                {
                    if (SettingDefine.OwnerLandBufferType == "外边缓冲")
                    {
                        geolandbuffershape = geoLand.Shape.Buffer(SettingDefine.Neighborlandbufferdistence);
                    }
                    else if (SettingDefine.OwnerLandBufferType == "边框缓冲")
                    {
                        var shapeextend = geoLand.Shape.GetEnvelope().ToGeometry();
                        geolandbuffershape = shapeextend.Buffer(SettingDefine.Neighborlandbufferdistence);
                    }
                }
                List<ContractLand> tempLands = new List<ContractLand>();
                if (geoLand.AliasNameD.IsNullOrEmpty() == false && geoLand.AliasNameD.Length > 0)
                {
                    var landids = geoLand.AliasNameD.Split(',');
                    foreach (var item in landids)
                    {
                        var itemid = Guid.Parse(item);
                        if (itemid == null) continue;
                        var itemland = landStation.Get(itemid);
                        if (itemland == null) continue;

                        tempLands.Add(itemland);
                    }
                }
                //else
                //{
                //    tempLands = landStation.GetIntersectLands(geoLand, geolandbuffershape);
                //}
                Dictionary<string, bool> landneighborhasland = new Dictionary<string, bool>();
                if (SettingDefine.ShowlandneighborLabel)
                {
                    exportLandParcelMainOperation.GetlandneighborhasLandInfo(geoLand, tempLands, landneighborhasland);
                }

                int layerCountIndex = 0;
                if (SettingDefine.IsShowNeighborLandGeo)
                {
                    exportLandParcelMainOperation.GetSetNeighborlandGeos(tempLands, geolandbuffershape, listFeature, map, layerCountIndex++);
                }
                listFeature.Clear();

                //点线面状地物
                var bufferLine = geoLand.Shape.Buffer(SettingDefine.Neighbordxmzdwbufferdistence);

                var tempPoints = ListPointFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //点状地物
                var tempLines = ListLineFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //线状地物
                var tempPolygons = ListPolygonFeature.FindAll(c => c.Shape.Intersects(bufferLine));  //面状地物
                exportLandParcelMainOperation.GetSetALLdxmzdwGeos(listFeature, tempPoints, tempLines, tempPolygons, map, layerCountIndex);
                layerCountIndex = layerCountIndex + 3;

                DynamicGeoSource selfGeods = null;  //本宗数据源
                selfGeods = exportLandParcelMainOperation.GetSetNeighborlandGeo(geoLand, selfGeods, map, layerCountIndex++, true);

                //自动过滤界址点
                exportLandParcelMainOperation.FilterjzdNodes = exportLandParcelMainOperation.FilterNodeByValidDot(geoLand, ListValidDot);

                //创建界址圈图层
                if (SettingDefine.IsShowJZDGeo)
                {
                    exportLandParcelMainOperation.AddNodeLayer(geoLand, tempLands, map, ListValidDot, layerCountIndex++);
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    map.MapControl.SpatialReference = targetSpatialReference;
                    var geosFullExtend = selfGeods.FullExtend.ToGeometry().Buffer(10).GetEnvelope();
                    map.MapControl.Extend = geosFullExtend;
                    map.MapControl.NavigateTo(geosFullExtend);
                    map.MapControl.Extend = geosFullExtend;
                }));

                for (int mi = layerCountIndex; mi < map.MapControl.Layers.Count; mi++)
                {
                    var pointLyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    var geoSource = pointLyer.DataSource as DynamicGeoSource;
                    geoSource.Clear();
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var env = map.MapControl.Extend;

                    var mElements = new MyElements(visibleBounds);
                    var index = 1;
                    exportLandParcelMainOperation.AddScaleText(ViewOfNeighorParcels, diagram, env, index++);    //添加比例尺文本标注
                    var ce = exportLandParcelMainOperation.AddCenterLable(ViewOfNeighorParcels, diagram, selfGeods, env, geoLand, index++);  //添加中心文本标注
                    mElements.AddElement(ce);

                    exportLandParcelMainOperation.AddCompass(ViewOfNeighorParcels, diagram, index++);  //添加指北针

                    for (int i = index; i < ViewOfNeighorParcels.Items.Count; i++)
                    {
                        var ti = ViewOfNeighorParcels.Items[i];
                        if (ti.Model.GetType() == typeof(TextShape))
                            continue;

                        ti.IsSelected = false;
                        ViewOfNeighorParcels.Items.RemoveAt(i);
                        ti.Dispose();
                        i--;
                    }

                    if (SettingDefine.ShowAlllandneighborLabel)//临宗标注直接使用调查四至
                    {
                        landneighborhasland.Clear();
                        landneighborhasland.Add("东至", false);
                        landneighborhasland.Add("南至", false);
                        landneighborhasland.Add("西至", false);
                        landneighborhasland.Add("北至", false);

                        var elements = exportLandParcelMainOperation.GetNeighborDiagramBases(ViewOfNeighorParcels, geoLand, index++, landneighborhasland); //添加调查四至文本标注
                        foreach (var element in elements)
                        {
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        index = index + elements.Count - 1;
                    }
                    else
                    {
                        foreach (var tempitem in tempLands)
                        {
                            var landOwnerName = tempitem.OwnerName;
                            if (landOwnerName.IsNullOrEmpty()) continue;
                            var element = exportLandParcelMainOperation.AddNeiberLandlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);  //添加临宗文本标注
                            if (element == null)
                            {
                                index--;
                                continue;
                            }
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        foreach (var tempitem in tempPoints)//点状标注
                        {
                            var dzdwname = tempitem.DWMC;
                            if (dzdwname.IsNullOrEmpty()) continue;
                            var element = exportLandParcelMainOperation.AddNeiberDzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                            if (element == null)
                            {
                                index--;
                                continue;
                            }
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        foreach (var tempitem in tempLines)//线状标注
                        {
                            var xzdwname = tempitem.DWMC;
                            if (xzdwname.IsNullOrEmpty()) continue;
                            var element = exportLandParcelMainOperation.AddNeiberXzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                            if (element == null)
                            {
                                index--;
                                continue;
                            }
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        foreach (var tempitem in tempPolygons)//面状标注
                        {
                            var mzdwname = tempitem.DWMC;
                            if (mzdwname.IsNullOrEmpty()) continue;
                            var element = exportLandParcelMainOperation.AddNeiberMzdwlabel(ViewOfNeighorParcels, diagram, env, tempitem, geoLand, index++);
                            if (element == null)
                            {
                                index--;
                                continue;
                            }
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                    }

                    if (SettingDefine.ShowlandneighborLabel && SettingDefine.ShowAlllandneighborLabel == false)//如果没有临宗，就把对应的调查四至打到临宗四个地方
                    {
                        var elements = exportLandParcelMainOperation.GetNeighborDiagramBases(ViewOfNeighorParcels, geoLand, index++, landneighborhasland);
                        foreach (var element in elements)
                        {
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        index = index + elements.Count - 1;
                    }

                    if (SettingDefine.NeighborlandLabeluseDLTGGQname && SettingDefine.ShowAlllandneighborLabel == false)//获取四至临宗直接打印调查四至中田埂、道路、沟渠
                    {
                        var elements = exportLandParcelMainOperation.GetNeighborDiagramBaseUSETGDLGQs(ViewOfNeighorParcels, geoLand, index++);
                        foreach (var element in elements)
                        {
                            exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                            mElements.AddElement(element);
                        }
                        index = index + elements.Count - 1;
                    }                                      

                    if (SettingDefine.IsShowJZDNumber)
                    {
                     
                        if (exportLandParcelMainOperation.FilterjzdNodes != null)
                        {
                            foreach (var tempitem in exportLandParcelMainOperation.FilterjzdNodes)
                            {
                                var temitemdot = ListValidDot.Find(ld => ld.Shape.Equals(tempitem));
                                if (temitemdot == null) continue;
                                var element = exportLandParcelMainOperation.AddNeiberJzdNumberlabel(ViewOfNeighorParcels, diagram, env, temitemdot, geoLand, index++, SettingDefine.IsUniteJZDNumber);  //添加界址点号标注
                                if (element == null)
                                {
                                    index--;
                                    continue;
                                }
                                exportLandParcelMainOperation.EnsureFullVisible(visibleBounds, element);
                                mElements.AddElement(element);
                            }
                        }
                    }

                    for (int i = index; i < ViewOfNeighorParcels.Items.Count; i++)
                    {
                        ViewOfNeighorParcels.Items[i].Model.X = short.MinValue;
                        ViewOfNeighorParcels.Items[i].Model.Y = short.MinValue;
                        var m = (ViewOfNeighorParcels.Items[i].Model as TextShape);
                        if (m != null)
                            m.Text = null;
                    }

                    var exent = ViewOfNeighorParcels.Paper.GetBounds();
                    ViewOfNeighorParcels.ZoomTo(exent);
                    ViewOfNeighorParcels.ZoomTo(1);

                }));

                listFeature.Clear();
            }
        }
        
        #endregion

        private void btnSaveToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var image = ViewOfNeighorParcels.SaveToImage(3);
                Clipboard.SetImage(image);
            }
            catch (Exception ex)
            {
                Owner.ShowDialog(new MessageDialog()
                {
                    Header = "错误",
                    Message = string.Format("复制到粘贴板出错: {0}", ex),
                    MessageGrade = eMessageGrade.Error
                });
            }
        }

        private void btnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var image = ViewOfNeighorParcels.SaveToImage(3);
                var dlg = new SaveFileDialog();
                dlg.Filter = "JPG 文件(*.jpg)|*.jpg";
                if (dlg.ShowDialog().Value)
                    image.SaveToFile(eImageFormat.JPG, dlg.FileName);
            }
            catch (Exception ex)
            {
                Owner.ShowDialog(new MessageDialog()
                {
                    Header = "错误",
                    Message = string.Format("保存到文件出错: {0}", ex),
                    MessageGrade = eMessageGrade.Error
                });
            }
        }

        private void btnToPager_Click(object sender, RoutedEventArgs e)
        {
            var exent = ViewOfNeighorParcels.Paper.GetBounds();
            exent.Inflate(10, 10);
            ViewOfNeighorParcels.ZoomTo(exent);
        }

        private void btnToPercent100_Click(object sender, RoutedEventArgs e)
        {
            ViewOfNeighorParcels.ZoomTo(1);

        }

        private void btnToPercent200_Click(object sender, RoutedEventArgs e)
        {
            ViewOfNeighorParcels.ZoomTo(0.5);
        }

        private void btnToPercent300_Click(object sender, RoutedEventArgs e)
        {
            ViewOfNeighorParcels.ZoomTo(0.3);
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ViewOfNeighorParcels.SelectedItems.Count == 0)
                pp.SetObject(null);
            else
                pp.SetObject(ViewOfNeighorParcels.SelectedItems[0].Model);
        }

        public void Dispose()
        {
            if (ViewOfNeighorParcels != null)
            {
                ViewOfNeighorParcels.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
                ViewOfNeighorParcels.Dispose();
                ViewOfNeighorParcels = null;
            }
            pp.Dispose();
            dp.Dispose();
            SelectedLand = null;
            ListGeoLand.Clear();
            ListGeoLand = null;
            geoLandCollection.Clear();
            geoLandCollection = null;
            ListLineFeature.Clear();
            ListLineFeature = null;
            ListPointFeature.Clear();
            ListPointFeature = null;
            ListPolygonFeature.Clear();
            ListPolygonFeature = null;
            DbContext = null;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = ViewOfNeighorParcels.SelectedItems.
                Where(c => c != ViewOfNeighorParcels.Paper && (c.Tag is bool && !((bool)c.Tag) || !(c.Tag is bool))).
                ToList();

            list.ForEach(c =>
            {
                c.IsSelected = false;
                ViewOfNeighorParcels.Items.Remove(c);
                c.Dispose();
            });

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

    }
}
