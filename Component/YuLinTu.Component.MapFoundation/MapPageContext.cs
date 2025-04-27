/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Component.MapFoundation.Configuration;
using YuLinTu.Component.MapFoundation.Controls;
using YuLinTu.Components.tGIS;
using YuLinTu.Components.tGIS.Edit;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public class MapPageContext : MapPageContextBase
    {
        #region Properties

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        public List<VirtualPerson> VPS { get; set; }

        #endregion Properties

        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator pnav = new ZoneNavigator();

        private Zone currentZone;//当前地域

        //private MetroToggleButton btnMapQueryBusiness;
        //private MetroToggleButton btnMapClipBusiness;
        //private MetroToggleButton btnMapClipAngleBusiness;
        //private MetroButton btnMapClipBySetBusiness;
        //private MetroToggleButton btnMapEditBusiness;
        private MetroToggleButton btnMapQueryPointBusiness;//节点查询

        private SuperButton sbMapTool;
        private SuperButton sbImportTool;
        private SuperButton usaualTool;//工具,包括查询、分割
        private SuperButton MapEditTool;//编辑，包括删除、修改图形及属性
        private InvestigationPersonPanel panel;
        private InvestigationLandPanel LandPanelControl = null;
        private NavigateItem navItem;
        private MenuItem importShapeItem = new MenuItem();//鼠标右键导入shape
        private MenuItem ClearItem = new MenuItem();//清空数据
        private MenuItem GenerateDataItem = new MenuItem();//生成数据
        private VectorLayer landLayer = null;//承包地图层
        private GraphicsLayer layerHover;//显示界址点图层
        private UnunionButton unubUnion;//分解

        private TextBlock SRtb = null;//坐标系名称显示空间

        /// <summary>
        /// 空间参考系单位换算亩系数
        /// </summary>
        private double projectionUnit = 0.0015;

        //当前被选择的图层
        private Layer currentSelectedLayer = null;

        private MapToolsShell mapToolShell = null;
        private MapToolsShell mapEditToolShell = null;

        //定义加载的图层

        //private VectorLayer topologyLayerPoint;
        //private VectorLayer topologyLayerPolyline;
        //private VectorLayer topologyLayerPolygon;

        private MultiVectorLayer rasterDatamultLayer;//栅格数据数据层

        private MultiVectorLayer locationBasicmultLayer;//定位基础数据层
        private VectorLayer controlPointLayer; //控制点图层

        private MultiVectorLayer zonemultlayer;//管辖基础数据层
        private VectorLayer zonexlayer;
        private VectorLayer zonezlayer;
        private VectorLayer zoneclayer;
        private VectorLayer zoneZlayer;

        private MultiVectorLayer zoneBoundarymultLayer;//区域界线层
        private VectorLayer zoneBoundarylayer;

        private MultiVectorLayer farmLandLayermultLayer;//基本农田保护区层
        private VectorLayer farmLandLayer;  //基本农田保护区图层

        private MultiVectorLayer DCmultlayer;//其他地物层
        private VectorLayer dczdlayer;
        private VectorLayer mzdwlayer;
        private VectorLayer xzdwlayer;
        private VectorLayer dzdwlayer;
        private VectorLayer cbdMarklayer;

        // private MultiVectorLayer foundationDataLayer;

        private MultiVectorLayer cbdmultlayer;//地块类别

        //private VectorLayer cbdBaselayer;
        private VectorLayer cbdlayer;

        private VectorLayer zldlayer;
        private VectorLayer jddlayer;
        private VectorLayer khdlayer;
        private VectorLayer qtjttdlayer;

        private MultiVectorLayer boundaryMultLayer; //界址信息图层层
        private VectorLayer dotLayer; //界址点图层
        private VectorLayer coilLayer; //界址线图层

        private DropDownButton ddbProperties;
        private MetroToggleButton mtbDot;
        private MetroToggleButton mtbCoil;
        private MetroToggleButton mtbPoint;

        private MetroListTabItem dotTabItem; //界址点面板
        private MetroListTabItem coilTabItem; //界址线面板
        private MetroListTabItem pointTabItem; //节点面板
        private DotPropertyPanel dotPanel;
        private CoilPropertyPanel coilPanel;
        private PointPropertyPanel pointPanel;

        private MetroToggleButton btnIdentifyTab;
        private MetroToggleButton mtbTool;
        private MetroListTabItem toolTabItem;
        private MapToolPanel toolPanel;

        private MapFoundationUserSettingDefine yltuserseting = MapFoundationUserSettingDefine.GetIntence(); //鱼鳞图用户自定义设置

        private IDbContext dbcontext = null;
        private YuLinTu.Library.WorkStation.IZoneWorkStation zoneStation = null;

        #endregion Fields

        #region Ctor

        public MapPageContext(IWorkpage workpage)
            : base(workpage)
        {
            panel = new YuLinTu.Library.Controls.InvestigationPersonPanel();
            ShowTaskViewer += () =>
            {
                workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };

            dbcontext = DataBaseSource.GetDataBaseSource();
            if (dbcontext != null)
            {
                zoneStation = dbcontext.CreateZoneWorkStation();
            }
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        /// <summary>
        /// 初始化添加查询坐标节点
        /// </summary>
        [MessageHandler(ID = IDMap.InstallToobarInQueryContainer)]
        private void InstallToobarInQueryContainer(object sender, InstallUIElementsEventArgs e)
        {
            Thickness contentpadding = new Thickness(4, 2, 4, 2);

            #region 视图工具

            sbMapTool = new SuperButton();
            sbMapTool.ToolTip = "地图视图工具";
            sbMapTool.IsSplit = false;
            sbMapTool.Padding = new Thickness(4, 2, 4, 2);
            sbMapTool.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            sbMapTool.ImagePosition = eDirection.Top;
            sbMapTool.DropDownArrowVisibility = Visibility.Visible;
            sbMapTool.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/GroupInsertLinks.png"));
            sbMapTool.Content = "视图工具";

            mapToolShell = new MapToolsShell();
            mapToolShell.grid.ContextMenu = null;
            (mapToolShell.menu.Items[0] as MenuItem).Tag = mapToolShell.Tools;
            sbMapTool.DropDownMenu = mapToolShell.menu;
            //e.Items.Add(sbMapTool); //TODO 优化代码

            #endregion 视图工具

            #region 编辑

            MapEditTool = new SuperButton();
            MapEditTool.ToolTip = "编辑";
            MapEditTool.IsSplit = false;
            MapEditTool.Padding = new Thickness(4, 2, 4, 2);
            MapEditTool.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            MapEditTool.ImagePosition = eDirection.Top;
            MapEditTool.DropDownArrowVisibility = Visibility.Visible;
            MapEditTool.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AreaMeasure.png"));
            MapEditTool.Content = "图形编辑";

            mapEditToolShell = new MapToolsShell();
            mapEditToolShell.grid.ContextMenu = null;
            (mapEditToolShell.menu.Items[0] as MenuItem).Tag = mapEditToolShell.Tools;
            MapEditTool.DropDownMenu = mapEditToolShell.menu;
            //e.Items.Add(MapEditTool);  //TODO 优化代码

            #endregion 编辑

            #region 裁剪工具

            usaualTool = new SuperButton();
            usaualTool.ToolTip = "工具";
            usaualTool.IsSplit = false;
            usaualTool.Padding = new Thickness(4, 2, 4, 2);
            usaualTool.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            usaualTool.ImagePosition = eDirection.Top;
            usaualTool.DropDownArrowVisibility = Visibility.Visible;
            usaualTool.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewsLayoutView.png"));
            usaualTool.Content = "裁剪工具";
            var usaualToolMenu = new ContextMenu() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left };

            var MapClipMenu = new MenuItem() { };
            MapClipMenu.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewNotesMasterView.png")),
                ImagePosition = eDirection.Left,
                Text = "分割裁剪",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };
            MapClipMenu.Click += MapClipMenu_Click;

            var MapClipAngleMenu = new MenuItem() { };
            MapClipAngleMenu.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/修角.png")),
                ImagePosition = eDirection.Left,
                Text = "修角裁剪",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };
            MapClipAngleMenu.Click += MapClipAngleMenu_Click;

            var MapClipBySetMenu = new MenuItem() { };
            MapClipBySetMenu.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewsLayoutView.png")),
                ImagePosition = eDirection.Left,
                Text = "设置分割",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };
            MapClipBySetMenu.Click += btnMapClipBySetBusiness_Click;

            usaualToolMenu.Items.Add(MapClipMenu);
            usaualToolMenu.Items.Add(MapClipAngleMenu);
            usaualToolMenu.Items.Add(MapClipBySetMenu);

            usaualTool.DropDownMenu = usaualToolMenu;
            //e.Items.Add(usaualTool);  //TODO 优化代码

            #endregion 裁剪工具

            #region 查询节点

            btnMapQueryPointBusiness = new MetroToggleButton();
            btnMapQueryPointBusiness.ToolTip = "查询节点";
            btnMapQueryPointBusiness.Padding = new Thickness(4, 2, 4, 2);
            btnMapQueryPointBusiness.MaxWidth = 45;
            btnMapQueryPointBusiness.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnMapQueryPointBusiness.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Text = "查询节点", Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/nodeInformation.png")) };
            //e.Items.Add(btnMapQueryPointBusiness);  //TODO 优化代码
            var bind = new Binding("Action");
            bind.Source = MapControl;
            bind.Converter = new MapActionToQueryGeoPointButtonIsCheckedConverter();
            bind.Mode = BindingMode.TwoWay;
            btnMapQueryPointBusiness.SetBinding(MetroToggleButton.IsCheckedProperty, bind);

            #endregion 查询节点

            #region 导入文件

            sbImportTool = new SuperButton();
            sbImportTool.ToolTip = "导入Shape数据文件";
            sbImportTool.IsSplit = false;
            sbImportTool.Padding = new Thickness(4, 2, 4, 2);
            sbImportTool.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            sbImportTool.ImagePosition = eDirection.Top;
            sbImportTool.DropDownArrowVisibility = Visibility.Visible;
            sbImportTool.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/MeasureExport.png"));
            sbImportTool.Content = "导入文件";
            var importMenu = new ContextMenu() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left };

            //var controlPointMenu = new MenuItem() { };
            //controlPointMenu.Icon = new ImageTextItem()
            //{
            //    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/PubTable.png")),
            //    ImagePosition = eDirection.Left,
            //    Text = "导入控制点Shape",
            //    Width = 140,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Left,
            //};
            //controlPointMenu.Click += TaskImportControlPointShape;

            //var zoneBoundaryMenu = new MenuItem() { };
            //zoneBoundaryMenu.Icon = new ImageTextItem()
            //{
            //    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/PubTable.png")),
            //    ImagePosition = eDirection.Left,
            //    Text = "导入区域界线Shape",
            //    Width = 145,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    HorizontalContentAlignment = HorizontalAlignment.Left,
            //};
            //zoneBoundaryMenu.Click += TaskImportZoneBoundaryShape;

            //importMenu.Items.Add(controlPointMenu);
            //importMenu.Items.Add(zoneBoundaryMenu);

            sbImportTool.DropDownMenu = importMenu;

            #endregion 导入文件
        }

        #region 工具对应事件

        private void MapClipAngleMenu_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryMapClipAngleByLine(MapControl);
        }

        private void MapQueryMenu_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryGeoPoint(MapControl);
        }

        /// <summary>
        /// 处理设置分割点击事件
        /// </summary>
        private void btnMapClipBySetBusiness_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryMapClipBySetting(MapControl);
        }

        /// <summary>
        /// 影像加载
        /// </summary>
        private void SbImageImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog imageDlg = new OpenFileDialog();
            imageDlg.Filter = "影像文件(*.tif;*.tiff;*.img)|*.tif;*.tiff;*.img";
            imageDlg.Multiselect = true;
            var val = imageDlg.ShowDialog();
            if (val == null || !val.Value)
                return;
            for (int i = 0; i < imageDlg.FileNames.Count(); i++)
            {
                string imageFile = imageDlg.FileNames[i];
                if (string.IsNullOrEmpty(imageFile))
                    return;
                string layerName = Path.GetFileNameWithoutExtension(imageFile);

                try
                {
                    var gs = new RasterSource(imageFile);
                    var rasterLayer = new RasterLayer(gs);
                    rasterLayer.Name = layerName;
                    rasterLayer.Renderer = new SimpleRasterRenderer(new RGBRasterSymbol());
                    rasterDatamultLayer.Layers.Add(rasterLayer);
                }
                catch (Exception ex)
                {
                    Tracker.WriteLine(new TrackerObject()
                    {
                        EventID = IDMap.ErrorCreateLayerDataSourceFailed,
                        Description = string.Format(new IDMap().GetName(IDMap.ErrorCreateLayerDataSourceFailed), ex),
                        Grade = eMessageGrade.Exception,
                        Source = this.GetType().FullName,
                    });

                    var dlgMsg = new TabMessageBoxDialog();
                    dlgMsg.Header = "影像文件";
                    dlgMsg.MessageGrade = eMessageGrade.Exception;
                    dlgMsg.Message = string.Format("创建图层数据源的过程中发生错误", ex);

                    Workpage.Page.ShowMessageBox(dlgMsg);
                }
            }
        }

        #endregion 工具对应事件

        /// <summary>
        /// 加载左侧内容栏
        /// </summary>
        protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        {
            if (Navigator == null)
            {
                return;
            }
            Navigator.RootItemAutoExpand = false;
            panel.TheWorkPage = Workpage;
            panel.OwerShipPanel.CurrentMapControl = MapControl;

            LandPanelControl = panel.OwerShipPanel.LandPanelControl;
            LandPanelControl.CurrentMapControl = MapControl;

            e.Items.Add(new MetroListTabItem()
            {
                Name = "",
                Header = new ImageTextItem()
                {
                    ImagePosition = eDirection.Top,
                    Text = "权属",
                    ToolTip = "权属",  //LanguageAttribute.GetLanguage("lang3070005")
                    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/Population32.png"))
                },
                Content = panel,
            });

            OnInstallDotTabItems(e);
            OnInstallCoilTabItem(e);
            OnInstallPointTabItem(e);
            OnInstallToolTabItem(e);

            var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Navigation/Res.xaml") };
            var key = new DataTemplateKey(typeof(NavigateZoneItem));
            if (Navigator != null)
            {
                Navigator.RegisterItemTemplate(typeof(NavigateZoneItem), dic[key] as DataTemplate);
            }
            var menu = dic["TreeViewNavigator_Menu_Zone"] as ContextMenu;
            Navigator.RegisterContextMenu(typeof(Zone), menu);
            Navigator.AddCommandBinding(ZoneNavigatorCommands.CopyCommandBinding);
        }

        /// <summary>
        /// 加载界址点左侧内容栏
        /// </summary>
        private void OnInstallDotTabItems(InstallUIElementsEventArgs e)
        {
            dotPanel = new DotPropertyPanel();
            dotPanel.OnDotSaved += DotPropertyPanel_OnDotSaved;
            //界址点面板
            dotTabItem = new MetroListTabItem()
            {
                Name = "mltDotItem",
                Content = dotPanel,
            };
            e.Items.Add(dotTabItem);

            mtbDot = new MetroToggleButton();
            mtbDot.ToolTip = "任意选择一地块可以快速显示该地块的所有界址点，并可以快速对其进行有效选择与编辑相关属性";
            mtbDot.Padding = new Thickness(4, 2, 4, 2);
            mtbDot.HorizontalContentAlignment = HorizontalAlignment.Left;
            mtbDot.VerticalContentAlignment = VerticalAlignment.Stretch;
            mtbDot.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Left,
                Text = "界址点",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ObjectsGroupMenuOutlook.png")),
            };

            SidebarTabControlItemAttacher.SetIsManual(dotTabItem, true);
            SidebarTabControlItemAttacher.SetItemController(dotTabItem, mtbDot);
        }

        /// <summary>
        /// 加载界址线左侧内容栏
        /// </summary>
        private void OnInstallCoilTabItem(InstallUIElementsEventArgs e)
        {
            coilPanel = new CoilPropertyPanel();
            //界址线面板
            coilTabItem = new MetroListTabItem()
            {
                Name = "mltCoilItem",
                Content = coilPanel,
            };
            e.Items.Add(coilTabItem);

            mtbCoil = new MetroToggleButton();
            mtbCoil.ToolTip = "任意选择一地块可以快速显示该地块的所有界址线，并可以快速对其进行编辑相关属性";
            mtbCoil.Padding = new Thickness(4, 2, 4, 2);
            mtbCoil.HorizontalContentAlignment = HorizontalAlignment.Left;
            mtbCoil.VerticalContentAlignment = VerticalAlignment.Stretch;
            mtbCoil.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Left,
                Text = "界址线",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ObjectsUngroup.png")),
            };

            SidebarTabControlItemAttacher.SetIsManual(coilTabItem, true);
            SidebarTabControlItemAttacher.SetItemController(coilTabItem, mtbCoil);
        }

        /// <summary>
        /// 加载节点左侧内容栏
        /// </summary>
        private void OnInstallPointTabItem(InstallUIElementsEventArgs e)
        {
            pointPanel = new PointPropertyPanel();
            //节点面板
            pointTabItem = new MetroListTabItem()
            {
                Name = "mltPointItem",
                Content = pointPanel,
            };
            e.Items.Add(pointTabItem);

            mtbPoint = new MetroToggleButton();
            mtbPoint.ToolTip = "将以前的节点查询放到该面板中呈现，只做呈现查看";
            mtbPoint.Padding = new Thickness(4, 2, 4, 2);
            mtbPoint.HorizontalContentAlignment = HorizontalAlignment.Left;
            mtbPoint.VerticalContentAlignment = VerticalAlignment.Stretch;
            mtbPoint.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Left,
                Text = "节点",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/编辑图形32.png")),
            };

            SidebarTabControlItemAttacher.SetIsManual(pointTabItem, true);
            SidebarTabControlItemAttacher.SetItemController(pointTabItem, mtbPoint);
        }

        /// <summary>
        /// 加载工具左侧内容栏
        /// </summary>
        private void OnInstallToolTabItem(InstallUIElementsEventArgs e)
        {
            toolPanel = new MapToolPanel(MapControl);

            //工具面板
            toolTabItem = new MetroListTabItem()
            {
                Name = "mltToolItem",
                Content = toolPanel,
            };
            e.Items.Add(toolTabItem);

            mtbTool = new MetroToggleButton();
            mtbTool.ToolTip = "提供操作地图的一些常用工具";
            mtbTool.Padding = new Thickness(4, 2, 4, 2);
            mtbTool.HorizontalContentAlignment = HorizontalAlignment.Left;
            mtbTool.VerticalContentAlignment = VerticalAlignment.Stretch;
            mtbTool.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "工具",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/VisualBasic.png")),
            };

            SidebarTabControlItemAttacher.SetIsManual(toolTabItem, true);
            SidebarTabControlItemAttacher.SetItemController(toolTabItem, mtbTool);
        }

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langNavigateTo)]
        private void OnInstallNavigateItems(object sender, NavigateToMsgEventArgs e)
        {
            Workpage.Properties["CurrentZoneCode"] = null;
            Workpage.Workspace.Properties["CurrentZoneCode"] = null;

            if (e.Object == null)
            {
                currentZone = null;
                return;
            }
            var zone = e.Object.Object as YuLinTu.Library.Entity.Zone;
            if (zone == null)
            {
                return;
            }
            Workpage.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Workspace.Properties["CurrentZoneCode"] = zone.FullCode;

            navItem = e.Object;
            panel.OwerShipPanel.SetZone(e.Object);

            VirtualPerson selectPerson = null;
            if (e.Object.Object is Zone)
            {
                currentZone = e.Object.Object as Zone;
                InitializeLayerBySelectZoneSet();
                var geo = currentZone.Shape;
                if (geo == null)
                    return;
                geo = geo.Envelope().Buffer(10);
                MapControl.ZoomTo(geo);
            }
            if (e.Object.Object is VirtualPerson)
            {
                selectPerson = e.Object.Object as VirtualPerson;
                var db = DataBaseSourceWork.GetDataBaseSource();

                //获取到组别下的承包方下地块集合
                var workstation = new ContainerFactory(db).CreateWorkstation<ISecondTableLandWorkStation, ISecondTableLandRepository>();
                var getPersonLand = workstation.GetCollection(selectPerson.ID);
                if (getPersonLand == null) return;
                YuLinTu.Spatial.Geometry personLandGeo = null;
                if (getPersonLand.Count == 1)
                {
                    if (getPersonLand[0].Shape == null) return;
                }
                else if (getPersonLand.Count > 1)
                {
                    for (int i = 1; i < getPersonLand.Count; i++)
                    {
                        if (getPersonLand[i].Shape == null) continue;
                        personLandGeo = (getPersonLand[0].Shape as YuLinTu.Spatial.Geometry).Union(getPersonLand[i].Shape as YuLinTu.Spatial.Geometry);
                    }
                }
                if (personLandGeo == null) return;
                personLandGeo = personLandGeo.Envelope().Buffer(10);
                MapControl.ZoomTo(personLandGeo);
            }
        }

        /// <summary>
        /// 根据按照地域选择设置将图层的信息显示
        /// </summary>
        private void InitializeLayerBySelectZoneSet()
        {
            if (currentZone != null && yltuserseting.IsUseSelectZoneShowData)
            {
                SetWhere(cbdlayer, "DKLB = \"" + ((int)eLandCategoryType.ContractLand).ToString() + "\"" + " && " + string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(zldlayer, "DKLB = \"" + ((int)eLandCategoryType.PrivateLand).ToString() + "\"" + " && " + string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(jddlayer, "DKLB = \"" + ((int)eLandCategoryType.MotorizeLand).ToString() + "\"" + " && " + string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(khdlayer, "DKLB = \"" + ((int)eLandCategoryType.WasteLand).ToString() + "\"" + " && " + string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(qtjttdlayer, "DKLB = \"" + ((int)eLandCategoryType.CollectiveLand).ToString() + "\"" + " && " + string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));

                SetWhere(zoneBoundarylayer, string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(controlPointLayer, string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(farmLandLayer, string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode));

                //dczdlayer.Where = string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode);
                SetWhere(dzdwlayer, string.Format("zonecode.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(xzdwlayer, string.Format("zonecode.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(mzdwlayer, string.Format("zonecode.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(cbdMarklayer, string.Format("ZLDM.StartsWith(\"{0}\")", currentZone.FullCode));

                SetWhere(dotLayer, string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode));
                SetWhere(coilLayer, string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode));
            }
            else if (currentZone != null && yltuserseting.IsUseSelectZoneShowData == false)
            {
                SetWhere(cbdlayer, "DKLB = \"" + ((int)eLandCategoryType.ContractLand).ToString() + "\"");
                SetWhere(zldlayer, "DKLB = \"" + ((int)eLandCategoryType.PrivateLand).ToString() + "\"");
                SetWhere(jddlayer, "DKLB = \"" + ((int)eLandCategoryType.MotorizeLand).ToString() + "\"");
                SetWhere(khdlayer, "DKLB = \"" + ((int)eLandCategoryType.WasteLand).ToString() + "\"");
                SetWhere(qtjttdlayer, "DKLB = \"" + ((int)eLandCategoryType.CollectiveLand).ToString() + "\"");

                zoneBoundarylayer.Where = "";
                controlPointLayer.Where = "";
                farmLandLayer.Where = "";

                //dczdlayer.Where = string.Format("DYBM.StartsWith(\"{0}\")", currentZone.FullCode);
                dzdwlayer.Where = "";
                xzdwlayer.Where = "";
                mzdwlayer.Where = "";
                cbdMarklayer.Where = "";

                dotLayer.Where = "";
                coilLayer.Where = "";
            }
        }

        private void SetWhere(VectorLayer layer, string where)
        {
            if (layer == null)
                return;
            layer.Where = where;
        }


        [MessageHandler(ID = EdCore.langNavigateSelectedItemChanged)]
        protected virtual void OnNavigateSelectedItemChanged(object sender, NavigateSelectedItemChangedEventArgs e)
        {
            Workpage.Properties["CurrentZoneCode"] = null;
            Workpage.Workspace.Properties["CurrentZoneCode"] = null;
            Workpage.Properties["CurrentZone"] = null;
            Workpage.Workspace.Properties["CurrentZone"] = null;

            if (e.Object == null)
                return;

            var zone = e.Object.Object as YuLinTu.Library.Entity.Zone;
            if (zone == null)
                return;

            Workpage.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Workspace.Properties["CurrentZoneCode"] = zone.FullCode;
            Workpage.Properties["CurrentZone"] = zone;
            Workpage.Workspace.Properties["CurrentZone"] = zone;
        }

        //卸载清除
        protected override void OnUninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            MapControl.SelectedItemsChanged -= MapControl_SelectedItemsChanged;
            MapControl.SpatialReferenceChanged -= MapControl_SpatialReferenceChanged;
            pnav = null;
            importShapeItem = null;
            currentZone = null;
            //btnMapQueryBusiness = null;
            //btnMapClipBusiness = null;
            //btnMapClipAngleBusiness = null;
            //btnMapEditBusiness = null;
            panel = null;
            LandPanelControl = null;
            navItem = null;
            landLayer = null;

            zonexlayer = null;
            zonezlayer = null;
            zoneclayer = null;
            zoneZlayer = null;

            dczdlayer = null;
            dzdwlayer = null;
            xzdwlayer = null;
            mzdwlayer = null;

            cbdMarklayer = null;

            //cbdBaselayer = null;
            cbdlayer = null;
            zldlayer = null;
            jddlayer = null;
            khdlayer = null;
            qtjttdlayer = null;
            zoneBoundarylayer = null;
            controlPointLayer = null;
            //topologyLayerPoint = null;
            //topologyLayerPolygon = null;
            //topologyLayerPolyline = null;

            dotLayer = null;
            coilLayer = null;

            farmLandLayer = null;
            SRtb = null;
        }

        #endregion Methods - Override

        #region Methods - Message

        #region 装载界面上方工具栏

        /// <summary>
        /// 加载面板
        /// </summary>

        [MessageHandler(ID = EdCore.langInstallWorkpageToolbarViewLeft)]
        private void langInstallWorkpageToolbarViewLeft(object sender, InstallUIElementsEventArgs e)
        {
            ddbProperties = new DropDownButton();
            ddbProperties.Padding = new Thickness(4, 2, 4, 2);
            ddbProperties.VerticalContentAlignment = VerticalAlignment.Top;
            DropDownButtonAttacher.SetArrowMargin(ddbProperties, new Thickness(0, 0, 0, 9));
            DropDownButtonAttacher.SetOrientation(ddbProperties, Orientation.Vertical);
            DropDownButtonAttacher.SetArrowHorizontalAlignment(ddbProperties, HorizontalAlignment.Center);
            DropDownButtonAttacher.SetArrowVerticalAlignment(ddbProperties, VerticalAlignment.Bottom);
            ddbProperties.Content = new ImageTextItem
            {
                ImagePosition = eDirection.Top,
                Text = "属性",
                ToolTip = "属性",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AreaNumericFormat.png")),
            };

            mtbDot.Click += (s, a) => { ddbProperties.IsOpen = false; };
            mtbCoil.Click += (s, a) => { ddbProperties.IsOpen = false; };

            StackPanel panel = new StackPanel();
            panel.Children.Add(mtbDot);
            panel.Children.Add(mtbCoil);
            //panel.Children.Add(mtbPoint);
            ddbProperties.DropDownContent = panel;
            e.Items.Add(ddbProperties); //加载属性

            e.Items.Add(mtbTool); //加载工具
        }

        /// <summary>
        /// 装载工具栏
        /// </summary>
        protected override void OnInstalToolbar(object sender, InstallUIElementsEventArgs e)
        {
            var 导航 = e.FindByName<TabControlItemRibbonContainer>("导航");
            导航.Text = "视图";
            var 工具 = e.FindByName<TabControlItemRibbonContainer>("工具");
            工具.Text = "基本操作";
            var 命令 = e.FindByName<TabControlItemRibbonContainer>("命令");
            命令.Text = "数据操作";
            var 剪贴板 = e.FindByName<TabControlItemRibbonContainer>("剪贴板");
            剪贴板.Visibility = Visibility.Collapsed;

            OnInstallFoundationTool(e, 导航, 工具);
            OnInstallDataTool(e, 工具, 命令);

            #region 注释--整饰界面,整理代码

            //var stackPanelProperties = e.FindByName<StackPanel>("stackPanelProperties");
            //stackPanelProperties.Orientation = Orientation.Horizontal;
            //var btnProperties = e.FindByName<MetroToggleButton>("btnProperties");
            //btnProperties.VerticalAlignment = VerticalAlignment.Stretch;
            //btnProperties.VerticalContentAlignment = VerticalAlignment.Top;
            //var imageItem = btnProperties.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/PubTable.png"));
            //imageItem.ImagePosition = eDirection.Top;
            //imageItem.Text = string.Format("{0}", "图层属性");
            //stackPanelProperties.Children.Remove(btnProperties);
            //命令.Items.Insert(5, btnProperties);

            //var btnCancel = e.FindByName<MetroButton>("btnCancel");
            //stackPanelProperties.Children.Remove(btnCancel);
            //btnCancel.VerticalAlignment = VerticalAlignment.Stretch;
            //btnCancel.VerticalContentAlignment = VerticalAlignment.Top;
            //imageItem = btnCancel.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/CancelSelect.png"));
            //imageItem.ImagePosition = eDirection.Top;
            //imageItem.Text = "取消";

            //var btnEditGeometry = e.FindByName<MetroToggleButton>("btnEditGeometry");
            //btnEditGeometry.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnEditGeometry.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnEditGeometry.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AreaMeasure.png"));
            //imageItem.ImagePosition = eDirection.Left;
            //btnEditGeometry.MaxWidth = double.PositiveInfinity;
            //imageItem.Text = "编辑图形";
            //工具.Items.Remove(btnEditGeometry);
            //mapEditToolShell.Tools.Add(btnEditGeometry);

            //var btnEditProperties = e.FindByName<MetroToggleButton>("btnEditProperties");
            //btnEditProperties.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnEditProperties.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnEditProperties.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/编辑数据32.png"));
            //btnEditProperties.MaxWidth = double.PositiveInfinity;
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "编辑属性";
            //工具.Items.Remove(btnEditProperties);
            //mapEditToolShell.Tools.Add(btnEditProperties);

            //var btnDelete = e.FindByName<MetroButton>("btnDelete");
            //btnDelete.HorizontalAlignment = HorizontalAlignment.Stretch;
            //btnDelete.HorizontalContentAlignment = HorizontalAlignment.Left;
            //imageItem = btnDelete.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Clear.png"));
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "删除";
            //var stackPanelClipboard = e.FindByName<StackPanel>("stackPanelClipboard");
            //stackPanelClipboard.Children.Remove(btnDelete);
            //mapEditToolShell.Tools.Add(btnDelete);

            //var btnPan = e.FindByName<MetroToggleButton>("btnPan");
            //导航.Visibility = Visibility.Collapsed;
            //导航.Items.Remove(btnPan);
            //工具.Items.Remove(btnDelete);
            //工具.Items.Insert(0, btnPan);
            //工具.Items.Insert(2, btnCancel);

            //var stackPanelGlobal = e.FindByName<StackPanel>("stackPanelGlobal");
            //var btnGlobalView = e.FindByName<MetroButton>("btnGlobalView");
            //btnGlobalView.VerticalAlignment = VerticalAlignment.Stretch;
            //btnGlobalView.VerticalContentAlignment = VerticalAlignment.Top;
            //btnGlobalView.Padding = contentpadding;
            //var item = btnGlobalView.Content as ImageTextItem;
            //item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/全图32.png"));
            //item.ImagePosition = eDirection.Top;
            //item.Text = "全图";
            //stackPanelGlobal.Children.Remove(btnGlobalView);
            //工具.Items.Insert(1, btnGlobalView);

            //var stackPanelZoom = e.FindByName<StackPanel>("stackPanelZoom");

            //var btnZoomIn = e.FindByName<MetroToggleButton>("btnZoomIn");
            //btnZoomIn.VerticalAlignment = VerticalAlignment.Stretch;
            //btnZoomIn.VerticalContentAlignment = VerticalAlignment.Top;
            //btnZoomIn.Padding = contentpadding;
            //item = btnZoomIn.Content as ImageTextItem;
            //item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/放大.png"));
            //item.ImagePosition = eDirection.Top;
            //item.Text = "放大";
            //stackPanelZoom.Children.Remove(btnZoomIn);
            //工具.Items.Insert(2, btnZoomIn);

            //var btnZoomOut = e.FindByName<MetroToggleButton>("btnZoomOut");
            //btnZoomOut.VerticalAlignment = VerticalAlignment.Stretch;
            //btnZoomOut.VerticalContentAlignment = VerticalAlignment.Top;
            //btnZoomOut.Padding = contentpadding;
            //item = btnZoomOut.Content as ImageTextItem;
            //item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/缩小.png"));
            //item.ImagePosition = eDirection.Top;
            //item.Text = "缩小";
            //stackPanelZoom.Children.Remove(btnZoomOut);
            //工具.Items.Insert(3, btnZoomOut);

            //var stackPanelView = e.FindByName<StackPanel>("stackPanelView");

            //var btnPreviousView = e.FindByName<MetroButton>("btnPreviousView");
            //btnPreviousView.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnPreviousView.HorizontalAlignment = HorizontalAlignment.Stretch;
            //item = btnPreviousView.Content as ImageTextItem;
            //item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/上一视图.png"));
            //btnPreviousView.MaxWidth = double.PositiveInfinity;
            //item.ImagePosition = eDirection.Left;
            //item.Text = "上一视图";
            //stackPanelView.Children.Remove(btnPreviousView);
            //mapToolShell.Tools.Add(btnPreviousView);

            //var btnNextView = e.FindByName<MetroButton>("btnNextView");
            //btnNextView.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnNextView.HorizontalAlignment = HorizontalAlignment.Stretch;
            //item = btnNextView.Content as ImageTextItem;
            //item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/下一视图.png"));
            //btnNextView.MaxWidth = double.PositiveInfinity;
            //item.ImagePosition = eDirection.Left;
            //item.Text = "下一视图";
            //stackPanelView.Children.Remove(btnNextView);
            //mapToolShell.Tools.Add(btnNextView);

            //sbImportTool.Padding = contentpadding;
            //命令.Items.Add(sbImportTool);

            #endregion 注释--整饰界面,整理代码
        }

        /// <summary>
        /// 装载基本操作工具栏
        /// </summary>
        private void OnInstallFoundationTool(InstallUIElementsEventArgs e,
                                             TabControlItemRibbonContainer 视图,
                                             TabControlItemRibbonContainer 基本操作)
        {
            #region 漫游

            var btnPan = e.FindByName<MetroToggleButton>("btnPan");
            btnPan.VerticalAlignment = VerticalAlignment.Stretch;
            btnPan.VerticalContentAlignment = VerticalAlignment.Top;
            btnPan.Padding = new Thickness(4, 2, 4, 2);
            var panItem = btnPan.Content as ImageTextItem;
            panItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ShowTimeZones.png"));

            视图.Visibility = Visibility.Collapsed;
            视图.Items.Remove(btnPan);
            基本操作.Items.Insert(0, btnPan);

            #endregion 漫游

            #region 全图

            var stackPanelGlobal = e.FindByName<StackPanel>("stackPanelGlobal");
            var btnGlobalView = e.FindByName<MetroButton>("btnGlobalView");
            btnGlobalView.VerticalAlignment = VerticalAlignment.Stretch;
            btnGlobalView.VerticalContentAlignment = VerticalAlignment.Top;
            btnGlobalView.Padding = new Thickness(4, 2, 4, 2);
            var item = btnGlobalView.Content as ImageTextItem;
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/全图32.png"));
            item.ImagePosition = eDirection.Top;
            item.Text = "全图";
            stackPanelGlobal.Children.Remove(btnGlobalView);
            基本操作.Items.Insert(1, btnGlobalView);

            #endregion 全图

            #region 放大

            var stackPanelZoom = e.FindByName<StackPanel>("stackPanelZoom");

            var btnZoomIn = e.FindByName<MetroToggleButton>("btnZoomIn");
            btnZoomIn.VerticalAlignment = VerticalAlignment.Stretch;
            btnZoomIn.VerticalContentAlignment = VerticalAlignment.Top;
            btnZoomIn.Padding = new Thickness(4, 2, 4, 2);
            item = btnZoomIn.Content as ImageTextItem;
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/放大.png"));
            item.ImagePosition = eDirection.Top;
            item.Text = "放大";

            var binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToZoomInButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;

            btnZoomIn.SetBinding(MetroToggleButton.IsCheckedProperty, binding);
            stackPanelZoom.Children.Remove(btnZoomIn);
            基本操作.Items.Insert(2, btnZoomIn);

            #endregion 放大

            #region 缩小

            var btnZoomOut = e.FindByName<MetroToggleButton>("btnZoomOut");
            btnZoomOut.VerticalAlignment = VerticalAlignment.Stretch;
            btnZoomOut.VerticalContentAlignment = VerticalAlignment.Top;
            btnZoomOut.Padding = new Thickness(4, 2, 4, 2);
            item = btnZoomOut.Content as ImageTextItem;
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/缩小.png"));
            item.ImagePosition = eDirection.Top;
            item.Text = "缩小";
            binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToZoomOutButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;

            btnZoomOut.SetBinding(MetroToggleButton.IsCheckedProperty, binding);
            stackPanelZoom.Children.Remove(btnZoomOut);
            基本操作.Items.Insert(3, btnZoomOut);

            #endregion 缩小

            #region 取消

            var stackPanelProperties = e.FindByName<StackPanel>("stackPanelProperties");
            stackPanelProperties.Orientation = Orientation.Horizontal;
            var btnCancel = e.FindByName<MetroButton>("btnCancel");
            stackPanelProperties.Children.Remove(btnCancel);
            btnCancel.VerticalAlignment = VerticalAlignment.Stretch;
            btnCancel.VerticalContentAlignment = VerticalAlignment.Top;
            var imageItem = btnCancel.Content as ImageTextItem;
            imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/CancelSelect.png"));
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = "取消";
            基本操作.Items.Insert(5, btnCancel);

            #endregion 取消

            #region 移除识别

            btnIdentifyTab = e.FindByName<MetroToggleButton>("btnIdentify");
            btnIdentifyTab.MinWidth = 60;
            btnIdentifyTab.ToolTip = "识别地图中选中要素属性相关信息";
            基本操作.Items.Remove(btnIdentifyTab);
            toolPanel.wpOtherTool.Children.Add(btnIdentifyTab);

            #endregion 移除识别

            #region 拓扑处理

            var btnTopo = new MetroToggleButton();
            btnTopo.ToolTip = "拓扑处理";
            btnTopo.Padding = new Thickness(4, 2, 4, 2);
            btnTopo.VerticalAlignment = VerticalAlignment.Stretch;
            btnTopo.VerticalContentAlignment = VerticalAlignment.Top;
            btnTopo.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "拓扑处理",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ShapeOutlineColorPicker.png")),
            };
            btnTopo.Click += BtnTopo_Click;
            toolPanel.wpOtherTool.Children.Add(btnTopo);

            #endregion 拓扑处理

            #region 分解

            //var btnResolve = new MetroToggleButton();
            //btnResolve.ToolTip = "分解";
            //btnResolve.Padding = new Thickness(4, 2, 4, 2);
            //btnResolve.VerticalAlignment = VerticalAlignment.Stretch;
            //btnResolve.VerticalContentAlignment = VerticalAlignment.Top;
            //btnResolve.Content = new ImageTextItem()
            //{
            //    ImagePosition = eDirection.Top,
            //    Text = "分解",
            //    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/MeasureBook.png")),
            //};
            var ddbSuperEdit = e.FindByName<DropDownButton>("btnTopologyEditDropDown");
            var mergePanel = ddbSuperEdit.DropDownContent as StackPanel;
            unubUnion = null;
            foreach (var child in mergePanel.Children)
            {
                var ub = child as UnunionButton;
                if (ub == null)
                    continue;
                unubUnion = ub;
            }
            if (unubUnion != null)
            {
                mergePanel.Children.Remove(unubUnion);
                unubUnion.Padding = new Thickness(4, 2, 4, 2);
                unubUnion.VerticalAlignment = VerticalAlignment.Stretch;
                unubUnion.VerticalContentAlignment = VerticalAlignment.Top;
                imageItem = unubUnion.Content as ImageTextItem;
                imageItem.ImagePosition = eDirection.Top;
                toolPanel.wpOtherTool.Children.Add(unubUnion);
            }

            #endregion 分解

            #region 移除删除

            var mbDelete = e.FindByName<MetroButton>("btnDelete");
            mbDelete.MinWidth = 60;
            imageItem = mbDelete.Content as ImageTextItem;
            imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Clear.png"));
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = "删除";
            var stackPanelClipboard = e.FindByName<StackPanel>("stackPanelProperties");
            stackPanelClipboard.Children.Remove(mbDelete);
            toolPanel.wpOtherTool.Children.Add(mbDelete);

            #endregion 移除删除
        }

        /// <summary>
        /// 装载数据操作工具栏
        /// </summary>
        private void OnInstallDataTool(InstallUIElementsEventArgs e,
                                       TabControlItemRibbonContainer 基本操作,
                                       TabControlItemRibbonContainer 数据操作)
        {
            #region 影像加载

            var sbImageImport = new SuperButton();
            sbImageImport.ToolTip = "将影像加载到栅格数据图层组中";
            sbImageImport.IsSplit = false;
            sbImageImport.Padding = new Thickness(4, 2, 4, 2);
            sbImageImport.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            sbImageImport.ImagePosition = eDirection.Top;
            sbImageImport.DropDownArrowVisibility = Visibility.Collapsed;
            sbImageImport.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/MeasureExport.png"));
            sbImageImport.Content = "影像加载";
            sbImageImport.Visibility = Visibility.Visible;
            数据操作.Items.Insert(0, sbImageImport);

            sbImageImport.Click += SbImageImport_Click;

            #endregion 影像加载

            #region 图形编辑

            var btnEditGeometry = e.FindByName<MetroToggleButton>("btnEditGeometry");
            btnEditGeometry.HorizontalContentAlignment = HorizontalAlignment.Left;
            btnEditGeometry.HorizontalAlignment = HorizontalAlignment.Stretch;
            var imageItem = btnEditGeometry.Content as ImageTextItem;
            imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AreaMeasure.png"));
            imageItem.ImagePosition = eDirection.Top;
            btnEditGeometry.MaxWidth = double.PositiveInfinity;
            imageItem.Text = "编辑图形";
            基本操作.Items.Remove(btnEditGeometry);
            数据操作.Items.Insert(1, btnEditGeometry);

            #endregion 图形编辑

            #region 属性编辑

            var btnEditProperties = e.FindByName<MetroToggleButton>("btnEditProperties");
            btnEditProperties.HorizontalContentAlignment = HorizontalAlignment.Left;
            btnEditProperties.HorizontalAlignment = HorizontalAlignment.Stretch;
            imageItem = btnEditProperties.Content as ImageTextItem;
            imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/编辑数据32.png"));
            btnEditProperties.MaxWidth = double.PositiveInfinity;
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = "编辑属性";
            基本操作.Items.Remove(btnEditProperties);
            数据操作.Items.Insert(2, btnEditProperties);

            #endregion 属性编辑

            #region 宗地分割

            var mtbLandDivision = new MetroToggleButton();
            mtbLandDivision.ToolTip = "宗地分割";
            mtbLandDivision.Padding = new Thickness(4, 2, 4, 2);
            mtbLandDivision.VerticalAlignment = VerticalAlignment.Stretch;
            mtbLandDivision.VerticalContentAlignment = VerticalAlignment.Top;
            mtbLandDivision.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "宗地分割",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewNotesMasterView.png")),
            };
            数据操作.Items.Insert(3, mtbLandDivision);

            var binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToMapClipByLineButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            mtbLandDivision.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            mtbLandDivision.Click += MapClipMenu_Click;

            #endregion 宗地分割

            #region 宗地合并

            var ddbSuperEdit = e.FindByName<DropDownButton>("btnTopologyEditDropDown");
            var mergePanel = ddbSuperEdit.DropDownContent as StackPanel;
            UnionButton ubUnion = null;

            foreach (var child in mergePanel.Children)
            {
                var ub = child as UnionButton;
                if (ub == null)
                    continue;
                ubUnion = ub;
            }

            数据操作.Items.Remove(ddbSuperEdit);
            mergePanel.Children.Remove(ubUnion);
            ubUnion.VerticalAlignment = VerticalAlignment.Stretch;
            ubUnion.VerticalContentAlignment = VerticalAlignment.Top;
            ubUnion.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                ImageTextSpacing = 1,
                Text = "宗地合并",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/ViewPrintLayoutView.png"))
            };
            数据操作.Items.Insert(4, ubUnion);

            //ddbSuperEdit.MaxWidth = Double.MaxValue;
            //DropDownButtonAttacher.SetArrowHorizontalAlignment(ddbSuperEdit, HorizontalAlignment.Center);
            //DropDownButtonAttacher.SetArrowMargin(ddbSuperEdit, new Thickness(0, 0, 0, 10));
            //imageItem = ddbSuperEdit.Content as ImageTextItem;
            //imageItem.ImagePosition = eDirection.Top;
            //imageItem.Text = "高级编辑";

            #endregion 宗地合并

            #region 拓扑检查

            var mtbTopo = e.FindByName<MetroToggleButton>("btnTopology");
            mtbTopo.MaxWidth = Double.MaxValue;
            imageItem = mtbTopo.Content as ImageTextItem;
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = "拓扑检查";

            #endregion 拓扑检查

            #region 导出选中Shape

            var LandAssignment = new MetroButton();
            LandAssignment.ToolTip = "导出所选中的地块的空间数据";
            LandAssignment.Padding = new Thickness(4, 2, 4, 2);
            LandAssignment.VerticalAlignment = VerticalAlignment.Stretch;
            LandAssignment.VerticalContentAlignment = VerticalAlignment.Top;
            LandAssignment.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "矢量导出",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/images/office/2013/32/blogpublish.png")),
            };
            数据操作.Items.Insert(8, LandAssignment);
            LandAssignment.Click += ExportSelectedShape;

            var LandBoundaryCalc = new MetroButton();
            LandBoundaryCalc.ToolTip = "计算更新选中地块的四至";
            LandBoundaryCalc.Padding = new Thickness(4, 2, 4, 2);
            LandBoundaryCalc.VerticalAlignment = VerticalAlignment.Stretch;
            LandBoundaryCalc.VerticalContentAlignment = VerticalAlignment.Top;
            LandBoundaryCalc.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "四至计算",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/SearchNeighor.png")),
            };
            数据操作.Items.Insert(8, LandBoundaryCalc);
            LandBoundaryCalc.Click += CalcLandBundary;

            #endregion 导出选中Shape

            #region 编辑地块编码

            var SplitLand = new MetroButton();
            SplitLand.ToolTip = "地块分割后，编辑地块编码";
            SplitLand.Padding = new Thickness(4, 2, 4, 2);
            SplitLand.VerticalAlignment = VerticalAlignment.Stretch;
            SplitLand.VerticalContentAlignment = VerticalAlignment.Top;
            SplitLand.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "地块编码",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/images/32/groupsharepointlists.png")),
            };
            数据操作.Items.Insert(9, SplitLand);
            SplitLand.Click += SplitLandCodeEdit;

            #endregion 编辑地块编码

            #region 区域赋值

            var sbZoneAssignment = new MetroToggleButton();
            sbZoneAssignment.ToolTip = "通过鼠标框选区域，然后给该区域中的地块进行统一属性赋值，主要包括：地块名称、地力等级、是否基本农田、土地利用类型、承包方式、地块类别";
            sbZoneAssignment.Padding = new Thickness(4, 2, 4, 2);
            sbZoneAssignment.VerticalAlignment = VerticalAlignment.Stretch;
            sbZoneAssignment.VerticalContentAlignment = VerticalAlignment.Top;
            sbZoneAssignment.Content = new ImageTextItem()
            {
                ImagePosition = eDirection.Top,
                Text = "区域赋值",
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/nodeInformation.png")),
            };
            数据操作.Items.Insert(6, sbZoneAssignment);

            binding = new Binding("Action");
            binding.Source = MapControl;
            MapControlActionMapZoneAssignmentConverter convert = new MapControlActionMapZoneAssignmentConverter();
            convert.map = MapControl;
            convert.TheWorkPage = Workpage;
            binding.Converter = convert;
            binding.Mode = BindingMode.TwoWay;
            sbZoneAssignment.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            sbZoneAssignment.Click += SbZoneAssignment_Click;

            //var sbZoneAssignment = new SuperButton();
            //sbZoneAssignment.ToolTip = "通过鼠标框选区域，然后给该区域中的地块进行统一属性赋值，主要包括：地块名称、地力等级、是否基本农田、土地利用类型、承包方式、地块类别";
            //sbZoneAssignment.IsSplit = false;
            //sbZoneAssignment.Padding = new Thickness(4, 2, 4, 2);
            //sbZoneAssignment.DropDownArrowMargin = new Thickness(0, 2, 0, 6);
            //sbZoneAssignment.ImagePosition = eDirection.Top;
            //sbZoneAssignment.DropDownArrowVisibility = Visibility.Collapsed;
            //sbZoneAssignment.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/nodeInformation.png"));
            //sbZoneAssignment.Content = "区域赋值";
            //sbZoneAssignment.Visibility = Visibility.Visible;
            //binding = new Binding("Action");
            //binding.Source = MapControl;
            //MapControlActionMapZoneAssignmentConverter convert = new MapControlActionMapZoneAssignmentConverter();
            //convert.map = MapControl;
            //convert.TheWorkPage = Workpage;
            //binding.Converter = convert;
            //binding.Mode = BindingMode.TwoWay;
            //sbZoneAssignment.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            //sbZoneAssignment.Click += SbZoneAssignment_Click;
            //数据操作.Items.Insert(6, sbZoneAssignment);

            #endregion 区域赋值

            #region 数据查找

            var mtbSearch = e.FindByName<MetroToggleButton>("btnSearch");
            mtbSearch.MaxWidth = Double.MaxValue;
            imageItem = mtbSearch.Content as ImageTextItem;
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = "数据查找";

            #endregion 数据查找

            #region 隐藏测量

            var ddbMesaure = e.FindByName<DropDownButton>("btnMeasureDropDown");
            ddbMesaure.Visibility = Visibility.Collapsed;

            #endregion 隐藏测量

            #region 隐藏图层属性

            var stackPanelProperties = e.FindByName<StackPanel>("stackPanelProperties");
            stackPanelProperties.Orientation = Orientation.Horizontal;
            var btnProperties = e.FindByName<MetroToggleButton>("btnProperties");
            btnProperties.VerticalAlignment = VerticalAlignment.Stretch;
            btnProperties.VerticalContentAlignment = VerticalAlignment.Top;
            imageItem = btnProperties.Content as ImageTextItem;
            imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/PubTable.png"));
            imageItem.ImagePosition = eDirection.Top;
            imageItem.Text = string.Format("{0}", "图层属性");
            stackPanelProperties.Children.Remove(btnProperties);
            //陈泽林
            layerHover = new GraphicsLayer();
            //数据操作.Items.Insert(5, btnProperties);

            #endregion 隐藏图层属性

            #region 注释--代码整理

            //var btnEditGeometry = e.FindByName<MetroToggleButton>("btnEditGeometry");
            //btnEditGeometry.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnEditGeometry.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnEditGeometry.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AreaMeasure.png"));
            //imageItem.ImagePosition = eDirection.Left;
            //btnEditGeometry.MaxWidth = double.PositiveInfinity;
            //imageItem.Text = "编辑图形";
            //基本操作.Items.Remove(btnEditGeometry);
            //mapEditToolShell.Tools.Add(btnEditGeometry);

            //var btnEditProperties = e.FindByName<MetroToggleButton>("btnEditProperties");
            //btnEditProperties.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnEditProperties.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnEditProperties.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/编辑数据32.png"));
            //btnEditProperties.MaxWidth = double.PositiveInfinity;
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "编辑属性";
            //基本操作.Items.Remove(btnEditProperties);
            //mapEditToolShell.Tools.Add(btnEditProperties);

            //var btnDelete = e.FindByName<MetroButton>("btnDelete");
            //btnDelete.HorizontalAlignment = HorizontalAlignment.Stretch;
            //btnDelete.HorizontalContentAlignment = HorizontalAlignment.Left;
            //imageItem = btnDelete.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Clear.png"));
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "删除";
            //var stackPanelClipboard = e.FindByName<StackPanel>("stackPanelClipboard");
            //stackPanelClipboard.Children.Remove(btnDelete);
            //mapEditToolShell.Tools.Add(btnDelete);

            //var stackPanelView = e.FindByName<StackPanel>("stackPanelView");
            //var btnPreviousView = e.FindByName<MetroButton>("btnPreviousView");
            //btnPreviousView.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnPreviousView.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnPreviousView.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/上一视图.png"));
            //btnPreviousView.MaxWidth = double.PositiveInfinity;
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "上一视图";
            //stackPanelView.Children.Remove(btnPreviousView);
            //mapToolShell.Tools.Add(btnPreviousView);

            //var btnNextView = e.FindByName<MetroButton>("btnNextView");
            //btnNextView.HorizontalContentAlignment = HorizontalAlignment.Left;
            //btnNextView.HorizontalAlignment = HorizontalAlignment.Stretch;
            //imageItem = btnNextView.Content as ImageTextItem;
            //imageItem.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/下一视图.png"));
            //btnNextView.MaxWidth = double.PositiveInfinity;
            //imageItem.ImagePosition = eDirection.Left;
            //imageItem.Text = "下一视图";
            //stackPanelView.Children.Remove(btnNextView);
            //mapToolShell.Tools.Add(btnNextView);

            //sbImportTool.Padding = new Thickness(4, 2, 4, 2);
            //数据操作.Items.Add(sbImportTool);

            #endregion 注释--代码整理
        }

        #endregion 装载界面上方工具栏

        //添加坐标系显示的工具
        protected override void OnInstalStatusbar(object sender, InstallUIElementsEventArgs e)
        {
            SRtb = new TextBlock();
            Thickness thick = new Thickness(10, 2, 0, 0);
            SRtb.Margin = thick;
            SRtb.FontSize = 11;
            MapControl.SpatialReferenceChanged += MapControl_SpatialReferenceChanged;

            e.Items.Add(SRtb);

            var sp = new YuLinTu.Windows.Wpf.Metro.Components.Separator();
            DockPanel.SetDock(sp, Dock.Right);
            e.Items.Add(sp);
        }

        protected override void OnInstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
            MapControl.SelectedItemsChanged += MapControl_SelectedItemsChanged;
        }

        private void MapControl_SelectedItemsChanged(object sender, EventArgs e)
        {
            var gs = MapControl.SelectedItems.ToList();
            var cbd = gs.Where(c => (c.Layer as VectorLayer).DataSource.ElementName == "ZD_CBD");
            var lands = cbd.ToList();
            //if (lands.Count <= 0)
            //    return;
            if (lands.Count != 1)
            {
                dotPanel.currentLand = null;
                coilPanel.currentLand = null;
                if (MapControl.InternalLayers.Contains(layerHover))
                    MapControl.InternalLayers.Remove(layerHover);
                dotPanel.layerHover = layerHover;
                coilPanel.layerHover = layerHover;
            }
            else
            {
                if (!MapControl.InternalLayers.Contains(layerHover))
                {
                    layerHover.Graphics.Clear();
                    MapControl.InternalLayers.Add(layerHover);
                }

                ContractLand land = new ContractLand();
                land.LandNumber = lands[0].Object.Object.GetPropertyValue("DKBM") as string;
                var ID = lands[0].Object.Object.GetPropertyValue("ID") as string;
                land.ID = Guid.Parse(ID);
                dotPanel.currentLand = land;
                dotPanel.Workpage = Workpage;
                dotPanel.coilPanel = coilPanel;
                coilPanel.Workpage = Workpage;
                coilPanel.currentLand = land;
                dotPanel.layerHover = layerHover;
                coilPanel.layerHover = layerHover;
            }
            //TaskQueue tq = new TaskQueueDispatcher(Application.Current.Dispatcher);

            //tq.Cancel();
            //tq.Do(
            //    go => { },
            //    completed => { },
            //    terminated => { }, null,
            //    started => { },
            //    ended => { });
        }

        //关闭时保存当前的文档yltmd，用于重新读取用
        protected override void OnUninstallAccountData(object sender, AccountEventArgs e)
        {
            var fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                string.Format(@"Template\{0}", "yltmdPriStyle.yltmd"));
            if (fileName != null)
                MapControl.SaveTo(fileName);
        }

        /// <summary>
        /// 保存界址点时更新界址线
        /// </summary>
        private void DotPropertyPanel_OnDotSaved(object sender, MsgEventArgs<ContractLand> e)
        {
            coilPanel.currentLand = e.Parameter;
        }

        /// <summary>
        /// 拖拽地块赋值方法
        /// </summary>
        [MessageHandler(ID = EdtGISClient.tGIS_Drag_Drop)]
        private void tGIS_Drag_Drop(object sender, MsgEventArgs<DragEventArgs> e)
        {
            //拖人过来
            IDataObject data = e.Parameter.Data;
            Object obj = data.GetData(typeof(Person)) as Person;
            if (obj != null)
            {
                var pt = e.Parameter.GetPosition(MapControl);
                var center = new GeometryFinderCenter(MapControl);
                center.FindTopAsync(pt,
                    gs =>
                    {
                        if (gs.Count == 0)
                            return;
                        var g = gs[0];
                        var vl = g.Layer as VectorLayer;
                        if (vl == null || vl.DataSource == null)
                            return;
                        var elementname = vl.DataSource.ElementName;
                        if (elementname == "ZD_CBD")
                        {
                            if (vl.DataSource.ElementName != typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName)
                                return;
                            var key = g.Object.Object.GetPropertyValue("ID") as string;  //地块的ID
                            Guid id = new Guid(key);
                            Person p = (Person)obj;
                            // 根据 Key 从数据库中把数据取出来，修改后，再保存回去。
                            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                            VirtualPersonBusiness virtualPersonBusiness = new VirtualPersonBusiness(dbContext);
                            Guid vpID = (Guid)p.FamilyID;
                            VirtualPerson vp = virtualPersonBusiness.GetVirtualPersonByID(vpID);
                            if (vp.Status == eVirtualPersonStatus.Lock) return;
                            ContractLand cl = landBusiness.GetLandById(id);
                            ContractLand previousCl = cl.Clone() as ContractLand;
                            if (p.FamilyID == cl.OwnerId)
                            {
                                return;
                            }
                            cl.OwnerId = vp.ID;
                            cl.OwnerName = vp.Name;
                            cl.ZoneCode = vp.ZoneCode;
                            landBusiness.ModifyLand(cl);
                            vl.Refresh();
                            panel.OwerShipPanel.Refresh();
                            ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, cl, vp.ZoneCode, previousCl);
                            SendMessasge(args);
                        }
                        else if (vl.Name == "调查宗地")
                        {
                            if (vl.DataSource.ElementName != typeof(DCZD).GetAttribute<DataTableAttribute>().TableName)
                                return;
                            var key = g.Object.Object.GetPropertyValue("ID") as string;  //地块的ID
                            Guid id = new Guid(key);
                            Person p = (Person)obj;
                            // 根据 Key 从数据库中把数据取出来，修改后，再保存回去。
                            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                            VirtualPersonBusiness virtualPersonBusiness = new VirtualPersonBusiness(dbContext);
                            Guid vpID = (Guid)p.FamilyID;
                            VirtualPerson vp = virtualPersonBusiness.GetVirtualPersonByID(vpID);
                            if (vp.Status == eVirtualPersonStatus.Lock) return;
                            IDCZDWorkStation dczdstation = new ContainerFactory(dbContext).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
                            DCZD dzdw = dczdstation.Get(id);
                            ContractLand cl = new ContractLand();
                            if (p.FamilyID == cl.OwnerId)
                            {
                                return;
                            }
                            cl.OwnerId = vp.ID;
                            cl.OwnerName = vp.Name;
                            cl.ZoneCode = vp.ZoneCode;
                            cl.Shape = dzdw.Shape;
                            cl.ActualArea = YuLinTu.Library.WorkStation.ToolMath.RoundNumericFormat(dzdw.Shape.Area() * projectionUnit, 2);

                            landBusiness.AddLand(cl);
                            dczdstation.Delete(dzdw.ID);
                            vl.Refresh();
                            panel.OwerShipPanel.Refresh();
                            MapControl.Refresh();
                            ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, cl, vp.ZoneCode);
                            SendMessasge(args);
                        }
                    },
                    error =>
                    {
                        Workpage.Page.ShowDialog(new MessageDialog()
                        {
                            MessageGrade = eMessageGrade.Exception,
                            Message = "地块权属更新失败!",
                            Header = "地块权属"
                        });
                    });
            }
            //拖地过来
            Object landObj = data.GetData(typeof(ContractLandUI)) as ContractLandUI;
            if (landObj != null)
            {
                var pt = e.Parameter.GetPosition(MapControl);
                var center = new GeometryFinderCenter(MapControl);
                center.FindTopAsync(pt,
                    gs =>
                    {
                        if (gs.Count == 0)
                            return;
                        var g = gs[0];
                        var vl = g.Layer as VectorLayer;
                        if (vl == null || vl.DataSource == null)
                            return;
                        var elementname = vl.DataSource.ElementName;
                        if (elementname == "ZD_CBD")
                        {
                            if (vl.DataSource.ElementName != typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName)
                                return;
                            var key = g.Object.Object.GetPropertyValue("ID") as string;  //选择地块的ID
                            Guid id = new Guid(key);
                            ContractLandUI dragLandUI = (ContractLandUI)landObj;
                            // 根据 Key 从数据库中把数据取出来，修改后，再保存回去。
                            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                            VirtualPersonBusiness virtualPersonBusiness = new VirtualPersonBusiness(dbContext);
                            ContractLand cl = landBusiness.GetLandById(id);//获取到选择的地块
                            ContractLand previousCl = cl.Clone() as ContractLand;
                            ContractLand dragLand = landBusiness.GetLandById(dragLandUI.ID);//获取到拖拽的地块
                            VirtualPerson vp = virtualPersonBusiness.GetVirtualPersonByID(dragLand.OwnerId.Value);
                            if (vp.Status == eVirtualPersonStatus.Lock) return;
                            ContractLand previousdragLand = dragLand.Clone() as ContractLand;
                            if (cl.ID == dragLand.ID) return;

                            YuLinTu.Spatial.Geometry targetGeo = cl.Shape;
                            double targetActualArea = cl.ActualArea;

                            //如果是新添加的 没有空间数据的地块
                            if (dragLand.Shape == null || dragLand.Shape.IsEmpty())
                            {
                                //旧地块的shape字段赋空
                                cl.Shape = null;
                                cl.ActualArea = 0.0;
                                landBusiness.ModifyLand(cl);
                                dragLand.Shape = targetGeo;
                                dragLand.ActualArea = targetActualArea;
                                landBusiness.ModifyLand(dragLand);
                            }
                            else
                            {   //如果拖拽的地块之前有空间数据，则赋值新空间数据过来  ,使用copyfrom的时候把东西全部包括ID拷贝过来了
                                cl.Shape = dragLand.Shape;
                                cl.ActualArea = dragLand.ActualArea;
                                landBusiness.ModifyLand(cl);
                                dragLand.Shape = targetGeo;
                                dragLand.ActualArea = targetActualArea;
                                landBusiness.ModifyLand(dragLand);
                            }
                            vl.Refresh();
                            panel.OwerShipPanel.Refresh();
                            ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, cl, cl.ZoneCode, previousCl);
                            SendMessasge(args);
                            ModuleMsgArgs arg = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, dragLand, dragLand.ZoneCode, previousdragLand);
                            SendMessasge(arg);
                        }
                        else if (vl.Name == "调查宗地")
                        {
                            if (vl.DataSource.ElementName != typeof(DCZD).GetAttribute<DataTableAttribute>().TableName)
                                return;
                            var key = g.Object.Object.GetPropertyValue("ID") as string;  //选择地块的ID
                            Guid id = new Guid(key);
                            ContractLandUI dragLandUI = (ContractLandUI)landObj;
                            // 根据 Key 从数据库中把数据取出来，修改后，再保存回去。
                            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                            VirtualPersonBusiness virtualPersonBusiness = new VirtualPersonBusiness(dbContext);
                            IDCZDWorkStation dczdstation = new ContainerFactory(dbContext).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
                            DCZD dczd = dczdstation.Get(id);     //获取到选择的地块
                            ContractLand dragLand = landBusiness.GetLandById(dragLandUI.ID);//获取到拖拽的地块
                            VirtualPerson vp = virtualPersonBusiness.GetVirtualPersonByID(dragLand.OwnerId.Value);
                            if (vp.Status == eVirtualPersonStatus.Lock) return;
                            dragLand.Shape = dczd.Shape;
                            dragLand.ActualArea = YuLinTu.Library.WorkStation.ToolMath.RoundNumericFormat(dczd.Shape.Area() * projectionUnit, 2); ;
                            landBusiness.ModifyLand(dragLand);
                            dczdstation.Delete(dczd.ID);
                            vl.Refresh();
                            panel.OwerShipPanel.Refresh();
                            MapControl.Refresh();
                            ModuleMsgArgs arg = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, dragLand, dragLand.ZoneCode);
                            SendMessasge(arg);
                        }
                    },
                    error =>
                    {
                        Workpage.Page.ShowDialog(new MessageDialog()
                        {
                            MessageGrade = eMessageGrade.Exception,
                            Message = "地块权属更新失败!",
                            Header = "地块权属"
                        });
                    });
            }
        }

        /// <summary>
        /// 初始化添加图层
        /// </summary>
        protected override void OnInitializeWorkpageContentCompleted(object sender, InitializeWorkpageContentCompletedEventArgs e)
        {
            PageContent.DocumentText.ShowFileName = false;

            MapControl.InitializeError += (s, a) =>
            {
                a.Exception.ToString();
            };

            var ds = DataBaseSourceWork.GetDataBaseSource();
            if (ds == null)
            {
                return;
            }

            //如果有地图文档，则加载地图文档
            var fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
               string.Format(@"Template\{0}", "yltmdPriStyle.yltmd"));

            if (fileName != null && File.Exists(fileName))
            {
                try
                {
                    MapControl.LoadFrom(fileName);
                }
                catch
                {
                    var layers = MapControl.Layers.ToList();
                    MapControl.Layers.Clear();
                    layers.ForEach(c => c.Dispose());
                    LoardAllLayers(ds);
                    return;
                }

                rasterDatamultLayer = MapControl.Layers.FindLayerByInternalName("rasterDatamult_Layer") as MultiVectorLayer;

                controlPointLayer = MapControl.Layers.FindLayerByInternalName("controlPoint_Layer") as VectorLayer;
                locationBasicmultLayer = MapControl.Layers.FindLayerByInternalName("locationBasicmult_Layer") as MultiVectorLayer;

                zonexlayer = MapControl.Layers.FindLayerByInternalName("zonex_Layer") as VectorLayer;
                zonezlayer = MapControl.Layers.FindLayerByInternalName("zonez_Layer") as VectorLayer;
                zoneclayer = MapControl.Layers.FindLayerByInternalName("zonec_Layer") as VectorLayer;
                zoneZlayer = MapControl.Layers.FindLayerByInternalName("zoneZ_Layer") as VectorLayer;
                zonemultlayer = MapControl.Layers.FindLayerByInternalName("zonemult_Layer") as MultiVectorLayer;
                if (zonexlayer != null)
                {
                    zonexlayer.Selectable = false;
                    zonexlayer.Drawable = false;
                    zonexlayer.Editable = false;
                }

                if (zonezlayer != null)
                {
                    zonezlayer.Selectable = false;
                    zonezlayer.Drawable = false;
                    zonezlayer.Editable = false;
                }

                if (zoneclayer != null)
                {
                    zoneclayer.Selectable = true;
                    zoneclayer.Drawable = true;
                    zoneclayer.Editable = true;
                }

                if (zoneZlayer != null)
                {
                    zoneZlayer.Selectable = true;
                    zoneZlayer.Drawable = true;
                    zoneZlayer.Editable = true;
                }

                if (zonemultlayer != null)
                {
                    zonemultlayer.Selectable = false;
                    zonemultlayer.Drawable = false;
                    zonemultlayer.Editable = false;
                }

                zoneBoundarylayer = MapControl.Layers.FindLayerByInternalName("zoneBoundary_Layer") as VectorLayer;
                zoneBoundarymultLayer = MapControl.Layers.FindLayerByInternalName("zoneBoundarymult_Layer") as MultiVectorLayer;

                farmLandLayer = MapControl.Layers.FindLayerByInternalName("farmLand_Layer") as VectorLayer;
                farmLandLayermultLayer = MapControl.Layers.FindLayerByInternalName("farmLandLayermult_Layer") as MultiVectorLayer;

                dczdlayer = MapControl.Layers.FindLayerByInternalName("dczd_Layer") as VectorLayer;
                dzdwlayer = MapControl.Layers.FindLayerByInternalName("dzdw_Layer") as VectorLayer;
                xzdwlayer = MapControl.Layers.FindLayerByInternalName("xzdw_Layer") as VectorLayer;
                mzdwlayer = MapControl.Layers.FindLayerByInternalName("mzdw_Layer") as VectorLayer;
                cbdMarklayer = MapControl.Layers.FindLayerByInternalName("cbdMark_Layer") as VectorLayer;
                DCmultlayer = MapControl.Layers.FindLayerByInternalName("DCmult_Layer") as MultiVectorLayer;

                //cbdBaselayer = MapControl.Layers.FindLayerByInternalName("cbd_Layer") as VectorLayer;
                //landLayer = cbdBaselayer;
                cbdlayer = MapControl.Layers.FindLayerByInternalName("dklb_cbd_Layer") as VectorLayer;
                zldlayer = MapControl.Layers.FindLayerByInternalName("dklb_zld_Layer") as VectorLayer;
                khdlayer = MapControl.Layers.FindLayerByInternalName("dklb_khd_Layer") as VectorLayer;
                jddlayer = MapControl.Layers.FindLayerByInternalName("dklb_jdd_Layer") as VectorLayer;
                qtjttdlayer = MapControl.Layers.FindLayerByInternalName("dklb_qtjttd_Layer") as VectorLayer;
                cbdmultlayer = MapControl.Layers.FindLayerByInternalName("cbdmult_Layer") as MultiVectorLayer;

                boundaryMultLayer = MapControl.Layers.FindLayerByInternalName("boundaryMult_Layer") as MultiVectorLayer;
                dotLayer = MapControl.Layers.FindLayerByInternalName("dot_Layer") as VectorLayer;
                coilLayer = MapControl.Layers.FindLayerByInternalName("coil_Layer") as VectorLayer;

                //topologymultlayer = MapControl.Layers.FindLayerByInternalName("topologymult_Layer") as MultiVectorLayer;
                //topologyLayerPoint = MapControl.Layers.FindLayerByInternalName("topology_LayerPoint") as VectorLayer;
                //topologyLayerPolygon = MapControl.Layers.FindLayerByInternalName("topology_LayerPolygon") as VectorLayer;
                //topologyLayerPolyline = MapControl.Layers.FindLayerByInternalName("topology_LayerPolyline") as VectorLayer;

                #region 更改为当前数据源

                if (zonexlayer != null)
                    zonexlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zonezlayer != null)
                    zonezlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zoneclayer != null)
                    zoneclayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zoneZlayer != null)
                    zoneZlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (dczdlayer != null)
                    dczdlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(DCZD).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (dzdwlayer != null)
                    dzdwlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(DZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (xzdwlayer != null)
                    xzdwlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(XZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };
                if (mzdwlayer != null)
                    mzdwlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(MZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (cbdMarklayer != null)
                    cbdMarklayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLandMark).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };

                if (farmLandLayer != null)
                    farmLandLayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(FarmLandConserve).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };

                if (cbdlayer != null)
                    cbdlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                //if (cbdBaselayer != null)
                //    cbdBaselayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon };
                if (zldlayer != null)
                    zldlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (jddlayer != null)
                    jddlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (khdlayer != null)
                    khdlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (qtjttdlayer != null)
                    qtjttdlayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };

                if (dotLayer != null)
                    dotLayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressDot).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (coilLayer != null)
                    coilLayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressCoil).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };

                if (controlPointLayer != null)
                    controlPointLayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ControlPoint).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (zoneBoundarylayer != null)
                    zoneBoundarylayer.DataSource = new SQLiteGeoSource(ds.ConnectionString, null, typeof(ZoneBoundary).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };

                #endregion 更改为当前数据源

                #region 判断是否加载了图层

                if (rasterDatamultLayer == null)
                {
                    rasterDatamultLayer = new MultiVectorLayer();
                    rasterDatamultLayer.InternalName = "rasterDatamult_Layer";
                    rasterDatamultLayer.SetIsManual(true);
                    rasterDatamultLayer.Drawable = true;
                    rasterDatamultLayer.Editable = true;
                    rasterDatamultLayer.Selectable = true;
                    rasterDatamultLayer.Name = "栅格数据";

                    MapControl.Layers.Add(rasterDatamultLayer);
                }

                if (locationBasicmultLayer == null)
                {
                    locationBasicmultLayer = new MultiVectorLayer();
                    locationBasicmultLayer.InternalName = "locationBasicmult_Layer";
                    locationBasicmultLayer.SetIsManual(true);
                    locationBasicmultLayer.Drawable = true;
                    locationBasicmultLayer.Editable = true;
                    locationBasicmultLayer.Selectable = true;
                    locationBasicmultLayer.Name = "定位数据";

                    MapControl.Layers.Add(locationBasicmultLayer);
                }
                if (controlPointLayer == null)
                {
                    controlPointLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ControlPoint).GetAttribute<DataTableAttribute>().TableName)
                    {
                        UseSpatialIndex = true,
                        GeometryType = eGeometryType.Point
                    });
                    controlPointLayer.Name = "控制点";
                    controlPointLayer.InternalName = "controlPoint_Layer";
                    controlPointLayer.SetIsManual(true);
                    controlPointLayer.Renderer = new SimpleRenderer(new SimplePointSymbol
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });

                    controlPointLayer.MinimizeScale = 5000;
                    controlPointLayer.MaximizeScale = 60000;
                    locationBasicmultLayer.Layers.Add(controlPointLayer);
                }

                if (zonemultlayer == null)
                {
                    zonemultlayer = new MultiVectorLayer();
                    zonemultlayer.InternalName = "zonemult_Layer";
                    zonemultlayer.Drawable = true;
                    zonemultlayer.Editable = true;
                    zonemultlayer.Selectable = true;
                    zonemultlayer.SetIsManual(true);
                    zonemultlayer.Name = "管辖区域";
                    MapControl.Layers.Add(zonemultlayer);
                }

                if (zonexlayer == null)
                {
                    zonexlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    zonexlayer.Where = "DYJB = 4";
                    zonexlayer.Name = "县级行政区";
                    zonexlayer.InternalName = "zonex_Layer";
                    zonexlayer.SetIsManual(true);
                    zonexlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    zonexlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 14,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "DYMC" };
                    zonexlayer.Labeler.Enabled = true;
                    zonexlayer.Drawable = false;
                    zonexlayer.Editable = false;
                    zonexlayer.Selectable = false;
                    zonexlayer.MinimizeScale = 100000;
                    zonexlayer.MaximizeScale = 1000000;
                    zonemultlayer.Layers.Add(zonexlayer);
                }
                if (zonezlayer == null)
                {
                    zonezlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    zonezlayer.Where = "DYJB = 3";
                    zonezlayer.Name = "乡级区域";
                    zonezlayer.InternalName = "zonez_Layer";
                    zonezlayer.SetIsManual(true);
                    zonezlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    zonezlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 13,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "DYMC" };
                    zonezlayer.Labeler.Enabled = true;
                    zonezlayer.Drawable = false;
                    zonezlayer.Editable = false;
                    zonezlayer.Selectable = false;
                    zonezlayer.MinimizeScale = 30000;
                    zonezlayer.MaximizeScale = 150000;
                    zonemultlayer.Layers.Add(zonezlayer);
                }
                if (zoneclayer == null)
                {
                    zoneclayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    zoneclayer.Where = "DYJB = 2";
                    zoneclayer.Name = "村级区域";
                    zoneclayer.InternalName = "zonec_Layer";
                    zoneclayer.SetIsManual(true);
                    zoneclayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                        BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                        BorderThickness = 1,
                    });
                    zoneclayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 12,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "DYMC" };
                    zoneclayer.Labeler.Enabled = true;
                    zoneclayer.Selectable = true;
                    zoneclayer.Drawable = true;
                    zoneclayer.Editable = true;
                    zoneclayer.MinimizeScale = 10000;
                    zoneclayer.MaximizeScale = 60000;
                    zonemultlayer.Layers.Add(zoneclayer);
                }
                if (zoneZlayer == null)
                {
                    zoneZlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    zoneZlayer.Where = "DYJB = 1";
                    zoneZlayer.Name = "组级区域";
                    zoneZlayer.InternalName = "zoneZ_Layer";
                    zoneZlayer.SetIsManual(true);
                    zoneZlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    zoneZlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 11,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "DYMC" };
                    zoneZlayer.Labeler.Enabled = true;
                    zoneZlayer.Selectable = true;
                    zoneZlayer.Drawable = true;
                    zoneZlayer.Editable = true;
                    zoneZlayer.MinimizeScale = 3000;
                    zoneZlayer.MaximizeScale = 20000;
                    zonemultlayer.Layers.Add(zoneZlayer);
                }
                if (zoneBoundarymultLayer == null)
                {
                    zoneBoundarymultLayer = new MultiVectorLayer();
                    zoneBoundarymultLayer.InternalName = "zoneBoundarymult_Layer";
                    zoneBoundarymultLayer.SetIsManual(true);
                    zoneBoundarymultLayer.Drawable = false;
                    zoneBoundarymultLayer.Editable = false;
                    zoneBoundarymultLayer.Selectable = false;
                    zoneBoundarymultLayer.Name = "区域界线";

                    MapControl.Layers.Add(zoneBoundarymultLayer);
                }
                if (zoneBoundarylayer == null)
                {
                    zoneBoundarylayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ZoneBoundary).GetAttribute<DataTableAttribute>().TableName)
                    {
                        UseSpatialIndex = true,
                        GeometryType = eGeometryType.Polyline
                    });
                    zoneBoundarylayer.Name = "区域界线";
                    zoneBoundarylayer.InternalName = "zoneBoundary_Layer";
                    zoneBoundarylayer.SetIsManual(true);
                    zoneBoundarylayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 158, 148, 112),
                        StrokeThickness = 1,
                    });

                    zoneBoundarylayer.MinimizeScale = 3000;
                    zoneBoundarylayer.MaximizeScale = 100000;
                    zoneBoundarymultLayer.Layers.Add(zoneBoundarylayer);
                }

                if (farmLandLayermultLayer == null)
                {
                    farmLandLayermultLayer = new MultiVectorLayer();
                    farmLandLayermultLayer.InternalName = "farmLandLayermult_Layer";
                    farmLandLayermultLayer.Drawable = false;
                    farmLandLayermultLayer.Editable = false;
                    farmLandLayermultLayer.Selectable = false;
                    farmLandLayermultLayer.SetIsManual(true);
                    farmLandLayermultLayer.Name = "基本农田";
                }
                if (farmLandLayer == null)
                {
                    farmLandLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(FarmLandConserve).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    farmLandLayer.InternalName = "farmLand_Layer";
                    farmLandLayer.Name = "基本农田保护区";
                    farmLandLayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    farmLandLayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 14,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "" };
                    farmLandLayer.Labeler.Enabled = true;
                    farmLandLayer.MinimizeScale = 0;
                    farmLandLayer.MaximizeScale = 10000;
                    farmLandLayermultLayer.Layers.Add(farmLandLayer);
                }

                if (DCmultlayer == null)
                {
                    DCmultlayer = new MultiVectorLayer();
                    DCmultlayer.InternalName = "DCmult_Layer";
                    DCmultlayer.SetIsManual(true);
                    DCmultlayer.Drawable = true;
                    DCmultlayer.Editable = true;
                    DCmultlayer.Selectable = true;
                    DCmultlayer.Name = "其他地物";

                    MapControl.Layers.Add(DCmultlayer);
                }
                if (dczdlayer == null)
                {
                    dczdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(DCZD).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });

                    dczdlayer.Name = "调查宗地";
                    dczdlayer.InternalName = "dczd_Layer";
                    dczdlayer.SetIsManual(true);
                    dczdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    dczdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 14,
                        FontStyle = eFontStyle.Bold,
                        ForegroundColor = Color.FromArgb(255, 32, 32, 32),
                    })
                    { LabelProperty = "" };
                    dczdlayer.Labeler.Enabled = true;
                    dczdlayer.MinimizeScale = 0;
                    dczdlayer.MaximizeScale = 8000;
                    DCmultlayer.Layers.Add(dczdlayer);
                }
                if (dzdwlayer == null)
                {
                    dzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(DZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point });
                    dzdwlayer.InternalName = "dzdw_Layer";
                    dzdwlayer.SetIsManual(true);
                    dzdwlayer.Name = "点状地物";
                    dzdwlayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    dzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol()
                    {
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "DWMC" };
                    dzdwlayer.Labeler.Enabled = true;
                    dzdwlayer.MinimizeScale = 0;
                    dzdwlayer.MaximizeScale = 60000;
                    DCmultlayer.Layers.Add(dzdwlayer);
                }
                if (xzdwlayer == null)
                {
                    xzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(XZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline });
                    xzdwlayer.Name = "线状地物";
                    xzdwlayer.InternalName = "xzdw_Layer";
                    xzdwlayer.SetIsManual(true);
                    xzdwlayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 158, 148, 112),
                        StrokeThickness = 1,
                    });
                    xzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePartInView()
                    {
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "DWMC" };
                    xzdwlayer.MinimizeScale = 0;
                    xzdwlayer.MaximizeScale = 60000;
                    DCmultlayer.Layers.Add(xzdwlayer);
                }
                if (mzdwlayer == null)
                {
                    mzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(MZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    mzdwlayer.InternalName = "mzdw_Layer";
                    mzdwlayer.SetIsManual(true);
                    mzdwlayer.Name = "面状地物";
                    mzdwlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    mzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "DWMC" };
                    mzdwlayer.Labeler.Enabled = true;
                    mzdwlayer.MinimizeScale = 0;
                    mzdwlayer.MaximizeScale = 60000;
                    DCmultlayer.Layers.Add(mzdwlayer);
                }
                if (cbdMarklayer == null)
                {
                    cbdMarklayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLandMark).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point });
                    cbdMarklayer.Name = "宗地标注";
                    cbdMarklayer.InternalName = "cbdMark_Layer";
                    cbdMarklayer.Visible = false;
                    cbdMarklayer.SetIsManual(true);
                    cbdMarklayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    cbdMarklayer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol()
                    {
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };
                    cbdMarklayer.Labeler.Enabled = true;
                    cbdMarklayer.MinimizeScale = 0;
                    cbdMarklayer.MaximizeScale = 8000;

                    //传给其它弹出控件使用
                    //panel.OwerShipPanel.LandLayer = cbdMarklayer;
                    //LandPanelControl.LandLayer = cbdMarklayer;
                    DCmultlayer.Layers.Add(cbdMarklayer);
                }

                #region 承包地图层

                if (cbdmultlayer == null)
                {
                    cbdmultlayer = new MultiVectorLayer();
                    cbdmultlayer.Name = "地块类别";
                    cbdmultlayer.MaximizeScale = 20000;
                    cbdmultlayer.SetIsManual(true);
                    cbdmultlayer.InternalName = "cbdmult_Layer";

                    MapControl.Layers.Add(cbdmultlayer);
                }

                if (cbdlayer == null)
                {
                    cbdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    cbdlayer.Name = "承包地";
                    cbdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.ContractLand).ToString() + "\"";
                    cbdlayer.InternalName = "dklb_cbd_Layer";
                    cbdlayer.SetIsManual(true);
                    cbdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    cbdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };

                    cbdlayer.Labeler.Enabled = true;
                    cbdlayer.MinimizeScale = 0;
                    cbdlayer.MaximizeScale = 8000;
                    cbdmultlayer.Layers.Add(cbdlayer);
                }

                if (zldlayer == null)
                {
                    zldlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    zldlayer.Name = "自留地";
                    zldlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.PrivateLand).ToString() + "\"";
                    zldlayer.InternalName = "dklb_zld_Layer";
                    zldlayer.SetIsManual(true);
                    zldlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(211, 255, 190, 255),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    zldlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };

                    zldlayer.Labeler.Enabled = true;
                    zldlayer.MinimizeScale = 0;
                    zldlayer.MaximizeScale = 8000;
                    cbdmultlayer.Layers.Add(zldlayer);
                }

                if (jddlayer == null)
                {
                    jddlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    jddlayer.Name = "机动地";
                    jddlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.MotorizeLand).ToString() + "\"";
                    jddlayer.InternalName = "dklb_jdd_Layer";
                    jddlayer.SetIsManual(true);
                    jddlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(227, 158, 0, 255),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    jddlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };

                    jddlayer.Labeler.Enabled = true;
                    jddlayer.MinimizeScale = 0;
                    jddlayer.MaximizeScale = 8000;
                    cbdmultlayer.Layers.Add(jddlayer);
                }
                if (khdlayer == null)
                {
                    khdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    khdlayer.Name = "开荒地";
                    khdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.WasteLand).ToString() + "\"";
                    khdlayer.InternalName = "dklb_khd_Layer";
                    khdlayer.SetIsManual(true);
                    khdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(240, 176, 207, 255),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    khdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };

                    khdlayer.Labeler.Enabled = true;
                    khdlayer.MinimizeScale = 0;
                    khdlayer.MaximizeScale = 8000;
                    cbdmultlayer.Layers.Add(khdlayer);
                }
                if (jddlayer == null)
                {
                    qtjttdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
                    qtjttdlayer.Name = "其他集体土地";
                    qtjttdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.CollectiveLand).ToString() + "\"";
                    qtjttdlayer.InternalName = "dklb_qtjttd_Layer";
                    qtjttdlayer.SetIsManual(true);
                    qtjttdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(190, 210, 255, 255),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    qtjttdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
                    {
                        AllowTextOverflow = true,
                        FontSize = 10,
                        GlowEffectSize = 3,
                    })
                    { LabelProperty = "QLRMC" };

                    qtjttdlayer.Labeler.Enabled = true;
                    qtjttdlayer.MinimizeScale = 0;
                    qtjttdlayer.MaximizeScale = 8000;
                    cbdmultlayer.Layers.Add(qtjttdlayer);
                }

                #endregion 承包地图层

                if (boundaryMultLayer == null)
                {
                    boundaryMultLayer = new MultiVectorLayer();
                    boundaryMultLayer.Name = "界址数据";
                    boundaryMultLayer.MaximizeScale = 20000;
                    boundaryMultLayer.Visible = false;
                    boundaryMultLayer.SetIsManual(true);
                    boundaryMultLayer.Drawable = false;
                    boundaryMultLayer.Editable = false;
                    boundaryMultLayer.InternalName = "boundaryMult_Layer";
                    MapControl.Layers.Add(boundaryMultLayer);
                }

                if (dotLayer == null)
                {
                    dotLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressDot).GetAttribute<DataTableAttribute>().TableName)
                    {
                        UseSpatialIndex = true,
                        GeometryType = eGeometryType.Point
                    });
                    dotLayer.Name = "界址点";
                    dotLayer.InternalName = "dot_Layer";
                    dotLayer.SetIsManual(true);
                    dotLayer.Renderer = new SimpleRenderer(new SimplePointSymbol
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 1,
                    });
                    dotLayer.MinimizeScale = 0;
                    dotLayer.MaximizeScale = 8000;
                    boundaryMultLayer.Layers.Add(dotLayer);
                }

                if (coilLayer == null)
                {
                    coilLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressCoil).GetAttribute<DataTableAttribute>().TableName)
                    {
                        UseSpatialIndex = true,
                        GeometryType = eGeometryType.Polyline
                    });
                    coilLayer.Name = "界址线";
                    coilLayer.InternalName = "coil_Layer";
                    coilLayer.SetIsManual(true);
                    coilLayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 158, 148, 112),
                        StrokeThickness = 1,
                    });
                    coilLayer.MinimizeScale = 0;
                    coilLayer.MaximizeScale = 8000;
                    boundaryMultLayer.Layers.Add(coilLayer);
                }

                //if (topologymultlayer == null)
                //{
                //    topologymultlayer = new MultiVectorLayer();
                //    topologymultlayer.Name = "拓扑错误";
                //    topologymultlayer.SetIsManual(true);
                //    topologymultlayer.Visible = false;
                //    topologymultlayer.Drawable = false;
                //    topologymultlayer.Editable = false;
                //    topologymultlayer.InternalName = "topologymult_layer";

                //    MapControl.Layers.Add(topologymultlayer);
                //}

                #endregion 判断是否加载了图层

                var targetSpatialReference = ds.CreateSchema().GetElementSpatialReference(
                   ObjectContext.Create(typeof(ContractLand)).Schema,
                   ObjectContext.Create(typeof(ContractLand)).TableName);

                MapControl.SpatialReference = targetSpatialReference;

                panel.OwerShipPanel.InitializeDataBase();
                panel.OwerShipPanel.SetZone(navItem);
                //panel.OwerShipPanel.LandLayer = cbdlayer;
                //LandPanelControl.LandLayer = cbdlayer;
            }
            else
            {
                LoardAllLayers(ds);
            }
        }

        /// <summary>
        /// 加载全部的图层
        /// </summary>
        /// <param name="ds"></param>
        private void LoardAllLayers(IDbContext ds)
        {
            #region 添加栅格数据图层

            rasterDatamultLayer = new MultiVectorLayer();
            rasterDatamultLayer.InternalName = "rasterDatamult_Layer";
            rasterDatamultLayer.Drawable = false;
            rasterDatamultLayer.Editable = false;
            rasterDatamultLayer.Selectable = false;
            rasterDatamultLayer.SetIsManual(true);
            rasterDatamultLayer.Name = "栅格数据";

            MapControl.Layers.Add(rasterDatamultLayer);

            #endregion 添加栅格数据图层

            #region 添加定位基础图层

            locationBasicmultLayer = new MultiVectorLayer();
            locationBasicmultLayer.InternalName = "locationBasicmult_Layer";
            locationBasicmultLayer.Drawable = false;
            locationBasicmultLayer.Editable = false;
            locationBasicmultLayer.Selectable = false;
            locationBasicmultLayer.SetIsManual(true);
            locationBasicmultLayer.Name = "定位基础";

            controlPointLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ControlPoint).GetAttribute<DataTableAttribute>().TableName)
            {
                UseSpatialIndex = true,
                GeometryType = eGeometryType.Point
            });
            controlPointLayer.Name = "控制点";
            controlPointLayer.InternalName = "controlPoint_Layer";
            controlPointLayer.SetIsManual(true);
            controlPointLayer.Renderer = new SimpleRenderer(new SimplePointSymbol
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });

            controlPointLayer.MinimizeScale = 5000;
            controlPointLayer.MaximizeScale = 60000;
            locationBasicmultLayer.Layers.Add(controlPointLayer);

            MapControl.Layers.Add(locationBasicmultLayer);

            #endregion 添加定位基础图层

            #region 管辖区域 市 县 镇 村 组图层

            zonexlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            zonexlayer.Where = "DYJB = 4";
            zonexlayer.Name = "县级行政区";
            zonexlayer.InternalName = "zonex_Layer";
            zonexlayer.SetIsManual(true);
            zonexlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            zonexlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 14,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "DYMC" };
            zonexlayer.Labeler.Enabled = true;
            zonexlayer.Selectable = false;
            zonexlayer.Drawable = false;
            zonexlayer.Editable = false;
            zonexlayer.MinimizeScale = 100000;
            zonexlayer.MaximizeScale = 1000000;

            zonezlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            zonezlayer.Where = "DYJB = 3";
            zonezlayer.Name = "乡级区域";
            zonezlayer.InternalName = "zonez_Layer";
            zonezlayer.SetIsManual(true);
            zonezlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            zonezlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 13,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "DYMC" };
            zonezlayer.Labeler.Enabled = true;
            zonezlayer.Selectable = false;
            zonezlayer.Drawable = false;
            zonezlayer.Editable = false;
            zonezlayer.MinimizeScale = 30000;
            zonezlayer.MaximizeScale = 150000;

            zoneclayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            zoneclayer.Where = "DYJB = 2";
            zoneclayer.Name = "村级区域";
            zoneclayer.InternalName = "zonec_Layer";
            zoneclayer.SetIsManual(true);
            zoneclayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                BorderThickness = 1,
            });
            zoneclayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 12,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "DYMC" };
            zoneclayer.Labeler.Enabled = true;
            zoneclayer.Selectable = true;
            zoneclayer.Drawable = true;
            zoneclayer.Editable = true;
            zoneclayer.MinimizeScale = 10000;
            zoneclayer.MaximizeScale = 60000;

            zoneZlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            zoneZlayer.Where = "DYJB = 1";
            zoneZlayer.Name = "组级区域";
            zoneZlayer.InternalName = "zoneZ_Layer";
            zoneZlayer.SetIsManual(true);
            zoneZlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            zoneZlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 11,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "DYMC" };
            zoneZlayer.Labeler.Enabled = true;
            zoneZlayer.Selectable = true;
            zoneZlayer.Drawable = true;
            zoneZlayer.Editable = true;
            zoneZlayer.MinimizeScale = 3000;
            zoneZlayer.MaximizeScale = 20000;

            zonemultlayer = new MultiVectorLayer();
            zonemultlayer.InternalName = "zonemult_Layer";
            zonemultlayer.Drawable = true;
            zonemultlayer.Editable = true;
            zonemultlayer.Selectable = true;
            zonemultlayer.SetIsManual(true);
            zonemultlayer.Name = "管辖区域";

            zonemultlayer.Layers.Add(zonexlayer);
            zonemultlayer.Layers.Add(zonezlayer);
            zonemultlayer.Layers.Add(zoneclayer);
            zonemultlayer.Layers.Add(zoneZlayer);

            MapControl.Layers.Add(zonemultlayer);

            #endregion 管辖区域 市 县 镇 村 组图层

            #region 添加区域界线图层

            zoneBoundarymultLayer = new MultiVectorLayer();
            zoneBoundarymultLayer.InternalName = "zoneBoundarymult_Layer";
            zoneBoundarymultLayer.Drawable = false;
            zoneBoundarymultLayer.Editable = false;
            zoneBoundarymultLayer.Selectable = false;
            zoneBoundarymultLayer.SetIsManual(true);
            zoneBoundarymultLayer.Name = "区域界线";

            zoneBoundarylayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ZoneBoundary).GetAttribute<DataTableAttribute>().TableName)
            {
                UseSpatialIndex = true,
                GeometryType = eGeometryType.Polyline
            });
            zoneBoundarylayer.Name = "区域界线";
            zoneBoundarylayer.InternalName = "zoneBoundary_Layer";
            zoneBoundarylayer.SetIsManual(true);
            zoneBoundarylayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
            {
                StrokeColor = Color.FromArgb(255, 158, 148, 112),
                StrokeThickness = 1,
            });

            zoneBoundarylayer.MinimizeScale = 3000;
            zoneBoundarylayer.MaximizeScale = 100000;
            zoneBoundarymultLayer.Layers.Add(zoneBoundarylayer);
            MapControl.Layers.Add(zoneBoundarymultLayer);

            #endregion 添加区域界线图层

            #region 添加基本农田图层

            farmLandLayermultLayer = new MultiVectorLayer();
            farmLandLayermultLayer.InternalName = "farmLandLayermult_Layer";
            farmLandLayermultLayer.Drawable = false;
            farmLandLayermultLayer.Editable = false;
            farmLandLayermultLayer.Selectable = false;
            farmLandLayermultLayer.SetIsManual(true);
            farmLandLayermultLayer.Name = "基本农田";

            farmLandLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(FarmLandConserve).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            farmLandLayer.InternalName = "farmLand_Layer";
            farmLandLayer.Name = "基本农田保护区";
            farmLandLayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            farmLandLayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 14,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "" };
            farmLandLayer.Labeler.Enabled = true;
            farmLandLayer.MinimizeScale = 0;
            farmLandLayer.MaximizeScale = 10000;
            farmLandLayermultLayer.Layers.Add(farmLandLayer);
            MapControl.Layers.Add(farmLandLayermultLayer);

            #endregion 添加基本农田图层

            #region 添加点 线 面 调查表图层

            dczdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(DCZD).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });

            dczdlayer.Name = "调查宗地";
            dczdlayer.InternalName = "dczd_Layer";
            dczdlayer.SetIsManual(true);
            dczdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 237, 232, 230),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            dczdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 14,
                FontStyle = eFontStyle.Bold,
                ForegroundColor = Color.FromArgb(255, 32, 32, 32),
            })
            { LabelProperty = "" };
            dczdlayer.Labeler.Enabled = true;
            dczdlayer.MinimizeScale = 0;
            dczdlayer.MaximizeScale = 8000;

            dzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(DZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point });
            dzdwlayer.InternalName = "dzdw_Layer";
            dzdwlayer.SetIsManual(true);
            dzdwlayer.Name = "点状地物";
            dzdwlayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            dzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol()
            {
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "DWMC" };
            dzdwlayer.Labeler.Enabled = true;
            dzdwlayer.MinimizeScale = 0;
            dzdwlayer.MaximizeScale = 60000;

            xzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(XZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline });
            xzdwlayer.Name = "线状地物";
            xzdwlayer.InternalName = "xzdw_Layer";
            xzdwlayer.SetIsManual(true);
            xzdwlayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
            {
                StrokeColor = Color.FromArgb(255, 158, 148, 112),
                StrokeThickness = 1,
            });
            xzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePartInView()
            {
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "DWMC" };
            xzdwlayer.Labeler.Enabled = true;
            xzdwlayer.MinimizeScale = 0;
            xzdwlayer.MaximizeScale = 60000;

            mzdwlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(MZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            mzdwlayer.InternalName = "mzdw_Layer";
            mzdwlayer.SetIsManual(true);
            mzdwlayer.Name = "面状地物";
            mzdwlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                BorderThickness = 1,
            });
            mzdwlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "DWMC" };
            mzdwlayer.Labeler.Enabled = true;
            mzdwlayer.MinimizeScale = 0;
            mzdwlayer.MaximizeScale = 60000;

            cbdMarklayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLandMark).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point });
            cbdMarklayer.Name = "宗地标注";
            cbdMarklayer.InternalName = "cbdMark_Layer";
            cbdMarklayer.SetIsManual(true);
            cbdMarklayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            cbdMarklayer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol()
            {
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };
            cbdMarklayer.Labeler.Enabled = true;
            cbdMarklayer.MinimizeScale = 0;
            cbdMarklayer.MaximizeScale = 8000;

            //传给其它弹出控件使用
            //panel.OwerShipPanel.LandLayer = cbdMarklayer;
            //LandPanelControl.LandLayer = cbdMarklayer;

            DCmultlayer = new MultiVectorLayer();
            DCmultlayer.InternalName = "DCmult_Layer";
            DCmultlayer.SetIsManual(true);
            DCmultlayer.Drawable = true;
            DCmultlayer.Editable = true;
            DCmultlayer.Selectable = true;
            DCmultlayer.Name = "其他地物";

            DCmultlayer.Layers.Add(dczdlayer);
            DCmultlayer.Layers.Add(mzdwlayer);
            DCmultlayer.Layers.Add(xzdwlayer);
            DCmultlayer.Layers.Add(dzdwlayer);
            DCmultlayer.Layers.Add(cbdMarklayer);

            MapControl.Layers.Add(DCmultlayer);

            #endregion 添加点 线 面 调查表图层

            #region 承包地图层

            cbdmultlayer = new MultiVectorLayer();
            cbdmultlayer.Name = "地块类别";
            cbdmultlayer.MaximizeScale = 20000;
            cbdmultlayer.SetIsManual(true);
            cbdmultlayer.InternalName = "cbdmult_Layer";

            //默认添加承包地的地图
            //cbdBaselayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon });
            //传给其它弹出控件使用
            //panel.OwerShipPanel.LandLayer = cbdBaselayer;
            //LandPanelControl.LandLayer = cbdBaselayer;
            //landLayer = cbdBaselayer;
            //cbdBaselayer.Name = "承包地图层";
            //cbdBaselayer.InternalName = "cbd_Layer";
            //cbdBaselayer.SetIsManual(true);
            //cbdBaselayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            //{
            //    BackgroundColor = Color.FromArgb(255, 204, 225, 160),
            //    BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
            //    BorderThickness = 1,
            //});
            //cbdBaselayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            //{
            //    AllowTextOverflow = true,
            //    FontSize = 10,
            //    GlowEffectSize = 3,
            //})
            //{ LabelProperty = "QLRMC" };

            //cbdBaselayer.Labeler.Enabled = true;
            //cbdBaselayer.Visible = false;//默认为不可见

            cbdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            cbdlayer.Name = "承包地";
            cbdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.ContractLand).ToString() + "\"";
            cbdlayer.InternalName = "dklb_cbd_Layer";
            cbdlayer.SetIsManual(true);
            cbdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            cbdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };

            cbdlayer.Labeler.Enabled = true;
            cbdlayer.MinimizeScale = 0;
            cbdlayer.MaximizeScale = 8000;

            zldlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            zldlayer.Name = "自留地";
            zldlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.PrivateLand).ToString() + "\"";
            zldlayer.InternalName = "dklb_zld_Layer";
            zldlayer.SetIsManual(true);
            zldlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(211, 255, 190, 255),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            zldlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };

            zldlayer.Labeler.Enabled = true;
            zldlayer.MinimizeScale = 0;
            zldlayer.MaximizeScale = 8000;

            jddlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            jddlayer.Name = "机动地";
            jddlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.MotorizeLand).ToString() + "\"";
            jddlayer.InternalName = "dklb_jdd_Layer";
            jddlayer.SetIsManual(true);
            jddlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(227, 158, 0, 255),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            jddlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };

            jddlayer.Labeler.Enabled = true;
            jddlayer.MinimizeScale = 0;
            jddlayer.MaximizeScale = 8000;

            khdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            khdlayer.Name = "开荒地";
            khdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.WasteLand).ToString() + "\"";
            khdlayer.InternalName = "dklb_khd_Layer";
            khdlayer.SetIsManual(true);
            khdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(240, 176, 207, 255),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            khdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };

            khdlayer.Labeler.Enabled = true;
            khdlayer.MinimizeScale = 0;
            khdlayer.MaximizeScale = 8000;

            qtjttdlayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });
            qtjttdlayer.Name = "其他集体土地";
            qtjttdlayer.Where = "DKLB = \"" + ((int)eLandCategoryType.CollectiveLand).ToString() + "\"";
            qtjttdlayer.InternalName = "dklb_qtjttd_Layer";
            qtjttdlayer.SetIsManual(true);
            qtjttdlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            {
                BackgroundColor = Color.FromArgb(190, 210, 255, 255),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            qtjttdlayer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView()
            {
                AllowTextOverflow = true,
                FontSize = 10,
                GlowEffectSize = 3,
            })
            { LabelProperty = "QLRMC" };

            qtjttdlayer.Labeler.Enabled = true;
            qtjttdlayer.MinimizeScale = 0;
            qtjttdlayer.MaximizeScale = 8000;

            cbdmultlayer.Layers.Add(cbdlayer);
            cbdmultlayer.Layers.Add(zldlayer);
            cbdmultlayer.Layers.Add(jddlayer);
            cbdmultlayer.Layers.Add(khdlayer);
            cbdmultlayer.Layers.Add(qtjttdlayer);
            //cbdmultlayer.Layers.Add(cbdBaselayer);
            MapControl.Layers.Add(cbdmultlayer);

            #endregion 承包地图层

            #region 界址信息图层

            boundaryMultLayer = new MultiVectorLayer();
            boundaryMultLayer.Name = "界址数据";
            boundaryMultLayer.MaximizeScale = 20000;
            boundaryMultLayer.SetIsManual(true);
            boundaryMultLayer.Visible = false;
            boundaryMultLayer.Editable = false;
            boundaryMultLayer.Drawable = false;
            boundaryMultLayer.InternalName = "boundaryMult_Layer";
            MapControl.Layers.Add(boundaryMultLayer);

            dotLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressDot).GetAttribute<DataTableAttribute>().TableName)
            {
                UseSpatialIndex = true,
                GeometryType = eGeometryType.Point
            });
            dotLayer.Name = "界址点";
            dotLayer.InternalName = "dot_Layer";
            dotLayer.SetIsManual(true);
            dotLayer.Renderer = new SimpleRenderer(new SimplePointSymbol
            {
                BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                BorderThickness = 1,
            });
            dotLayer.MinimizeScale = 0;
            dotLayer.MaximizeScale = 8000;
            coilLayer = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(BuildLandBoundaryAddressCoil).GetAttribute<DataTableAttribute>().TableName)
            {
                UseSpatialIndex = true,
                GeometryType = eGeometryType.Polyline
            });
            coilLayer.Name = "界址线";
            coilLayer.InternalName = "coil_Layer";
            coilLayer.SetIsManual(true);
            coilLayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
            {
                StrokeColor = Color.FromArgb(255, 158, 148, 112),
                StrokeThickness = 1,
            });
            coilLayer.MinimizeScale = 0;
            coilLayer.MaximizeScale = 8000;

            boundaryMultLayer.Layers.Add(coilLayer);
            boundaryMultLayer.Layers.Add(dotLayer);

            #endregion 界址信息图层

            #region 拓扑错误图层

            //topologymultlayer = new MultiVectorLayer();
            //topologymultlayer.Name = "拓扑错误";
            //topologymultlayer.SetIsManual(true);
            //topologymultlayer.Visible = false;
            //topologymultlayer.Drawable = false;
            //topologymultlayer.Editable = false;
            //topologymultlayer.InternalName = "topologymult_layer";

            //MapControl.Layers.Add(topologymultlayer);

            //topologyLayerPolygon = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(TopologyErrorPolygon).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon });
            //topologyLayerPolygon.Name = "面拓扑错误";
            //topologyLayerPolygon.InternalName = "topology_LayerPolygon";
            //topologyLayerPolygon.SetIsManual(true);
            //topologyLayerPolygon.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
            //{
            //    BackgroundColor = Color.FromArgb(100, 255, 0, 0),
            //    BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
            //    BorderThickness = 3,
            //});
            //topologymultlayer.Layers.Add(topologyLayerPolygon);

            //topologyLayerPolyline = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(TopologyErrorPolyline).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polyline });
            //topologyLayerPolyline.Name = "线拓扑错误";
            //topologyLayerPolyline.InternalName = "topology_LayerPolyline";
            //topologyLayerPolyline.SetIsManual(true);
            //topologyLayerPolyline.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
            //{
            //    StrokeColor = Color.FromArgb(255, 255, 0, 0),
            //    StrokeThickness = 3,
            //});
            //topologymultlayer.Layers.Add(topologyLayerPolyline);

            //topologyLayerPoint = new VectorLayer(new SQLiteGeoSource(ds.ConnectionString, null, typeof(TopologyErrorPoint).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Point });
            //topologyLayerPoint.Name = "点拓扑错误";
            //topologyLayerPoint.InternalName = "topology_LayerPoint";
            //topologyLayerPoint.SetIsManual(true);
            //topologyLayerPoint.Renderer = new SimpleRenderer(new SimplePointSymbol()
            //{
            //    BackgroundColor = Color.FromArgb(100, 255, 0, 0),
            //    BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
            //    BorderThickness = 3,
            //    Size = 15,
            //});
            //topologymultlayer.Layers.Add(topologyLayerPoint);

            #endregion 拓扑错误图层
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langInstallNavigateItems)]
        private void OnInstallNavigateItems(object sender, InstallNavigateItemMsgEventArgs e)
        {
            e.Instance.Items.AddRange(pnav.GetChildren(e.Instance.Root));
        }

        [MessageHandler(Name = "RefreshMapContrl_UIdata")]
        public void RefreshUIdata(object sender, MapMessageEventArgs e)
        {
            panel.OwerShipPanel.Refresh();
            MapControl.Refresh();
        }
        /// <summary>
        /// 合并地块时，修改面积参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.InstallUnionGeometryResult)]
        private void OnInstallUnionGeometryItems(object sender, InstallUnionGeometryResultEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //不是承包地合并分离则不管
                var targetLayer = e.Layer as VectorLayer;
                if (targetLayer == null || targetLayer.DataSource == null) return;
                var elementname = targetLayer.DataSource.ElementName;
                //不是承包地合并分离则不管
                if (elementname != "ZD_CBD") return;

                var grahpics = e.DeleteGraphics;
                var targetgra = e.Result;
                double actualArea = 0.0;
                double tableArea = 0.0;
                var targetGraActualArea = targetgra.Object.Object.GetPropertyValue("SCMJ");
                if (targetGraActualArea != null)
                {
                    actualArea = double.Parse(targetGraActualArea.ToString());
                }
                var targetGraTableArea = targetgra.Object.Object.GetPropertyValue("TZMJ");
                if (targetGraTableArea != null)
                {
                    tableArea = double.Parse(targetGraTableArea.ToString());
                }

                foreach (var graitem in grahpics)
                {
                    var unionGraActualArea = graitem.Object.Object.GetPropertyValue("SCMJ");
                    if (unionGraActualArea != null)
                    {
                        actualArea += double.Parse(unionGraActualArea.ToString());
                    }
                    var unionGraTableArea = graitem.Object.Object.GetPropertyValue("TZMJ");
                    if (unionGraTableArea != null)
                    {
                        tableArea += double.Parse(unionGraTableArea.ToString());
                    }
                }
                targetgra.Object.Object.SetPropertyValue("SCMJ", actualArea);
                targetgra.Object.Object.SetPropertyValue("TZMJ", tableArea);
                targetgra.Object.Object.SetPropertyValue("BZMJ", actualArea);
            }));
        }

        /// <summary>
        /// 分离地块时，修改面积参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.InstallUnunionGeometryResult)]
        private void OnInstallUnunionGeometryItems(object sender, InstallUnunionGeometryResultEventArgs e)
        {
            var landBusiness = new AccountLandBusiness(dbcontext);
            var n = int.Parse(landBusiness.GetNewLandNumber(currentZone.FullCode).GetLastString(5));

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var targetLayer = e.Layer as VectorLayer;
                if (targetLayer == null || targetLayer.DataSource == null) return;
                var elementname = targetLayer.DataSource.ElementName;
                //不是承包地合并分离则不管
                if (elementname != "ZD_CBD") return;

                var grahpics = e.AddGraphics;
                var targetgra = e.UpdateGraphic;

                var targetGraActualArea = YuLinTu.Library.WorkStation.ToolMath.RoundNumericFormat(targetgra.Geometry.Area() * projectionUnit, 2);
                targetgra.Object.Object.SetPropertyValue("SCMJ", targetGraActualArea);
                targetgra.Object.Object.SetPropertyValue("TZMJ", targetGraActualArea);

                foreach (var graitem in grahpics)
                {
                    var graitemActualArea = YuLinTu.Library.WorkStation.ToolMath.RoundNumericFormat(graitem.Geometry.Area() * projectionUnit, 2);
                    graitem.Object.Object.SetPropertyValue("SCMJ", graitemActualArea);
                    graitem.Object.Object.SetPropertyValue("TZMJ", graitemActualArea);

                    string unitCode = currentZone.FullCode;
                    if (unitCode.Length == 16)
                        unitCode = unitCode.Substring(0, 12) + unitCode.Substring(14, 2);
                    unitCode = unitCode.PadRight(14, '0');
                    string code = unitCode + (n++).ToString().PadLeft(5, '0');

                    var number = code;
                    string surverNumber = "";
                    if (number != null)
                        surverNumber = number.Length >= 5 ? number.Substring(number.Length - 5) : number.PadLeft(5, '0');

                    graitem.Object.Object.SetPropertyValue("DKBM", number);
                    graitem.Object.Object.SetPropertyValue("DCBM", surverNumber);
                }
            }));
        }

        protected override void OnNavigatorFirstLoadComplete(object sender, MsgEventArgs<Navigator> e)
        {
        }

        protected override void OnWorkpageShown()
        {
            var codeZone = Workpage.Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);
            if (Navigator == null)
            {
                return;
            }
            if (codeZone.IsNullOrBlank())
            {
                if (Navigator.SelectedItem != null)
                    Navigator.Reload();
                return;
            }
            var codeZonePage = Workpage.Properties.TryGetValue<string>("CurrentZoneCode", null);
            if (codeZone == codeZonePage)
                return;

            Navigator.Expand(items =>
            {
                foreach (var item in items)
                {
                    var obj = item.Object as YuLinTu.Library.Entity.Zone;
                    if (obj == null)
                        continue;
                    if (codeZone.Equals(obj.FullCode))
                        return new NavigateItem() { Object = new MetroTreeListViewExpandTarget(item) };
                    if (codeZone.StartsWith(obj.FullCode))
                        return item;
                    if (obj.FullCode == "86")
                        return item;
                }

                return null;
            });
        }

        /// <summary>
        /// 设置改变时会响应//或者换库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSettingsChanged(object sender, SettingsProfileChangedEventArgs e)
        {
            if (e.Profile.Name == TheBns.stringDataSourceNameChangedMessageKey)
            {
                var dsNew = DataBaseSourceWork.GetDataBaseSource();
                if (dsNew == null)
                {
                    if (zonexlayer != null)
                        zonexlayer.DataSource = null;
                    if (zonezlayer != null)
                        zonezlayer.DataSource = null;
                    if (zoneclayer != null)
                        zoneclayer.DataSource = null;
                    if (zoneZlayer != null)
                        zoneZlayer.DataSource = null;

                    if (dczdlayer != null)
                        dczdlayer.DataSource = null;
                    if (dzdwlayer != null)
                        dzdwlayer.DataSource = null;
                    if (xzdwlayer != null)
                        xzdwlayer.DataSource = null;
                    if (mzdwlayer != null)
                        mzdwlayer.DataSource = null;
                    if (cbdMarklayer != null)
                        cbdMarklayer.DataSource = null;

                    if (farmLandLayer != null)
                        farmLandLayer.DataSource = null;
                    if (cbdlayer != null)
                        cbdlayer.DataSource = null;
                    //if (cbdBaselayer != null)
                    //    cbdBaselayer.DataSource = null;
                    if (zldlayer != null)
                        zldlayer.DataSource = null;
                    if (jddlayer != null)
                        jddlayer.DataSource = null;
                    if (khdlayer != null)
                        khdlayer.DataSource = null;
                    if (qtjttdlayer != null)
                        qtjttdlayer.DataSource = null;

                    if (controlPointLayer != null)
                        controlPointLayer.DataSource = null;
                    if (zoneBoundarylayer != null)
                        zoneBoundarylayer.DataSource = null;
                    //if (topologyLayerPoint != null)
                    //    topologyLayerPoint.DataSource = null;
                    //if (topologyLayerPolyline != null)
                    //    topologyLayerPolyline.DataSource = null;
                    //if (topologyLayerPolygon != null)
                    //    topologyLayerPolygon.DataSource = null;
                    if (dotLayer != null)
                        dotLayer.DataSource = null;
                    if (coilLayer != null)
                        coilLayer.DataSource = null;
                    Navigator.Reload();
                    while (Workpage.Page.CloseDialog() != null) ;
                    return;
                }

                var targetSpatialReference = dsNew.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(ContractLand)).Schema,
                    ObjectContext.Create(typeof(ContractLand)).TableName);

                MapControl.SpatialReference = targetSpatialReference;

                Navigator.Reload();
                while (Workpage.Page.CloseDialog() != null) ;
                if (zonexlayer != null)
                    zonexlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zonezlayer != null)
                    zonezlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zoneclayer != null)
                    zoneclayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (zoneZlayer != null)
                    zoneZlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (dczdlayer != null)
                    dczdlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(DCZD).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (dzdwlayer != null)
                    dzdwlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(DZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (xzdwlayer != null)
                    xzdwlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(XZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };
                if (mzdwlayer != null)
                    mzdwlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(MZDW).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (cbdMarklayer != null)
                    cbdMarklayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLandMark).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };

                if (farmLandLayer != null)
                    farmLandLayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(FarmLandConserve).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };

                if (cbdlayer != null)
                    cbdlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                //if (cbdBaselayer != null)
                //    cbdBaselayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon };
                if (zldlayer != null)
                    zldlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (jddlayer != null)
                    jddlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (khdlayer != null)
                    khdlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };
                if (qtjttdlayer != null)
                    qtjttdlayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon };

                if (dotLayer != null)
                    dotLayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(BuildLandBoundaryAddressDot).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (coilLayer != null)
                    coilLayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(BuildLandBoundaryAddressCoil).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };

                if (controlPointLayer != null)
                    controlPointLayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ControlPoint).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Point };
                if (zoneBoundarylayer != null)
                    zoneBoundarylayer.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(ZoneBoundary).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polyline };

                //if (topologyLayerPoint != null)
                //    topologyLayerPoint.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(TopologyErrorPoint).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Point };
                //if (topologyLayerPolyline != null)
                //    topologyLayerPolyline.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(TopologyErrorPolyline).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polyline };
                //if (topologyLayerPolygon != null)
                //    topologyLayerPolygon.DataSource = new SQLiteGeoSource(dsNew.ConnectionString, null, typeof(TopologyErrorPolygon).GetAttribute<DataTableAttribute>().TableName) { GeometryType = eGeometryType.Polygon };

                panel.OwerShipPanel.InitializeDataBase();
                panel.OwerShipPanel.SetZone(navItem);
                //panel.OwerShipPanel.LandLayer = cbdlayer;
                //LandPanelControl.LandLayer = cbdlayer;
            }
            else if (e.Profile.Name == "CURRENTROOTCHANGE")
            {
                Navigator.Reload();
            }
        }

        //导航-当前选择的项
        [MessageHandler(ID = IDMap.LayerControlSelectedLayerChanged)]
        private void LayerControlSelectedLayerChanged(object sender, MsgEventArgs<Layer> e)
        {
            currentSelectedLayer = e.Parameter;
            var selectLayer = currentSelectedLayer as VectorLayer;
            if ((selectLayer is MultiVectorLayer) || selectLayer == null || selectLayer.DataSource == null || !selectLayer.IsInitialized
              || (selectLayer.DataSource.ElementName == "JCSJ_XZQY") || (selectLayer.DataSource.ElementName == "ZD_CBD") ||
              (selectLayer.DataSource.ElementName == "JZX") || (selectLayer.DataSource.ElementName == "JZD"))
            {
                importShapeItem.Visibility = Visibility.Collapsed;
                ClearItem.Visibility = Visibility.Collapsed;
                if (selectLayer != null && (selectLayer.InternalName == "zonemult_Layer" || selectLayer.InternalName == "cbdmult_Layer"))
                    importShapeItem.Visibility = Visibility.Visible;
            }
            else
            {
                importShapeItem.Visibility = Visibility.Visible;
                ClearItem.Visibility = Visibility.Visible;
            }
            if (selectLayer != null && selectLayer.InternalName == "zoneBoundary_Layer")
                GenerateDataItem.Visibility = Visibility.Visible;
            else
                GenerateDataItem.Visibility = Visibility.Collapsed;
        }

        [MessageHandler(ID = IDMap.EditSaveGraphicPropertiesSuccess)]
        private void EditSaveGraphicPropertiesSuccess(object sender, MsgEventArgs e)
        {
        }

        /// <summary>
        /// 属性编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.EditFeatureInstallProperties)]
        private void EditFeatureInstallProperties(object sender, EditFeatureInstallPropertiesEventArgs e)
        {
            var targetLayer = e.Graphic.Layer as VectorLayer;
            if (targetLayer == null || targetLayer.DataSource == null) return;
            var elementname = targetLayer.DataSource.ElementName;
            if (elementname == "ZD_CBD")
            {
                e.IsCancel = true;
                var dlg = new ContractLandPage();

                /* 修改于2016/09/29 */
                dlg.ShowMapProperty += () => { return true; };
                if (!MapControl.InternalLayers.Contains(layerHover))
                {
                    layerHover.Graphics.Clear();
                    MapControl.InternalLayers.Add(layerHover);
                }
                dlg.layerHover = layerHover;

                var obj = e.Graphic.Object.Object;
                var db = DataBaseSourceWork.GetDataBaseSource();
                var landStation = db.CreateContractLandWorkstation();
                var zoneStation = db.CreateZoneWorkStation();
                var vpStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
                ContractLand selectLand = new ContractLand();
                Zone selectZone = new Zone();
                VirtualPerson selectVirtualPerson = new VirtualPerson();
                var landid = obj.GetPropertyValue("ID");
                if (landid.ToString() == "" || landid == null)
                {
                    ShowBox("提示", "地块ID为空!");
                    return;
                }
                Guid landId = new Guid(landid.ToString());
                var sfqgxt = obj.GetPropertyValue("SFQGDK");

                if (sfqgxt != null && (bool)sfqgxt)
                {
                    ShowBox("提示", "确股地块无法在当前模块进行属性编辑，请在确股模块编辑!");
                    return;
                }
                string personId = obj.GetPropertyValue("QLRBS") as string;
                if (personId == null)
                {
                    ShowBox("提示", "地块无所属承包方,请设置设置所属承包方!");
                    return;
                }
                Guid pId = new Guid(personId);

                TheApp.Current.CreateTask(
                    go =>
                    {
                        if (landid != null)
                        {
                            selectLand = landStation.Get(landId);
                        }

                        selectZone = zoneStation.Get((obj.GetPropertyValue("ZLDM") as string));
                        selectVirtualPerson = vpStation.Get(pId);
                    },
                    terminated =>
                    {
                        ShowBox("提示", "不能编辑，请检查地块对应ID、坐落代码、承包方标识等信息!");
                    },
                    completed =>
                    {
                        if (selectZone == null)
                        {
                            ShowBox("提示", "地块坐落代码为空或无对应的地域节点，请检查坐落代码与地域节点!");
                            return;
                        }
                        if (selectLand == null) return;
                        if (selectLand.IsStockLand)
                        {
                            ShowBox(ContractAccountInfo.ContractLandEdit, "请选择确权地块编辑!");
                            return;
                        }
                        if (selectVirtualPerson == null)
                        {
                            ShowBox("提示", "对应承包方为空，请检查数据!");
                            return;
                        }
                        bool isLock = selectVirtualPerson.Status == eVirtualPersonStatus.Lock;
                        if (isLock)
                        {
                            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((m, n) =>
                            {
                                if (!(bool)m)
                                    return;
                                dlg.Workpage = this.Workpage;
                                dlg.MapControl = MapControl;
                                dlg.CurrentLand = selectLand.Clone() as ContractLand;
                                dlg.CurrentPerson = selectVirtualPerson;
                                dlg.CurrentZone = selectZone;
                                dlg.PrevoiusLand = selectLand;
                                dlg.IsLock = isLock;
                                Workpage.Page.ShowMessageBox(dlg, (b, r) =>
                                {
                                    if (!(bool)b)
                                    {
                                        MapControl.SelectedItems.Clear();
                                        return;
                                    }
                                    MapControl.SelectedItems.Clear();
                                });
                            });
                            ShowBox("编辑属性", "当前地块被锁定,只能查看属性信息!", eMessageGrade.Infomation, hasCancel: false, action: action);
                        }
                        else
                        {
                            dlg.Workpage = this.Workpage;
                            dlg.MapControl = MapControl;
                            dlg.CurrentLand = selectLand.Clone() as ContractLand;
                            dlg.CurrentPerson = selectVirtualPerson;
                            dlg.CurrentZone = selectZone;
                            dlg.PrevoiusLand = selectLand;
                            dlg.IsLock = isLock;
                            Workpage.Page.ShowMessageBox(dlg, (b, r) =>
                            {
                                if (!(bool)b)
                                {
                                    MapControl.SelectedItems.Clear();
                                    return;
                                }
                                MapControl.SelectedItems.Clear();
                            });
                        }
                    },
                    started => { },
                    ended =>
                    {
                    }).Start();
            }
        }

        /// <summary>
        /// 主要针对承包地的删除,删除前先清理地块对于的权证和合同对应业务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.DeleteGraphicsBegin)]
        private void DeleteGraphicsBegin(object sender, DeleteGraphicsBeginEventArgs e)
        {
            var deletgraphics = e.Graphics;
            if (deletgraphics == null || deletgraphics.Count == 0) return;

            List<Guid> delLandIDs = new List<Guid>();
            List<ContractConcord> delLandConcords = new List<ContractConcord>();
            var db = DataBaseSourceWork.GetDataBaseSource();
            var landStation = db.CreateContractLandWorkstation();
            var concordStation = db.CreateConcordStation();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var dgitem in deletgraphics)
                {
                    var locationlayer = dgitem.Layer as VectorLayer;
                    if (locationlayer == null || locationlayer.DataSource == null) continue;
                    var elementname = locationlayer.DataSource.ElementName;
                    if (elementname != "ZD_CBD") continue;

                    var landkey = dgitem.Object.Object.GetPropertyValue("ID") as string;  //地块的ID
                    var concordkey = dgitem.Object.Object.GetPropertyValue("HTID") as string;  //合同的ID
                    if (landkey == null) continue;

                    if (concordkey != null)
                    {
                        Guid concordid = new Guid(concordkey);
                        var concord = concordStation.Get(concordid);
                        if (concord != null && delLandConcords.Any(dc => dc.ID == concord.ID) == false)
                        {
                            delLandConcords.Add(concord);
                        }
                    }
                    Guid id = new Guid(landkey);
                    delLandIDs.Add(id);
                }
            }));
            if (delLandIDs.Count == 0) return;

            landStation.UpdateConcordByDelLand(delLandIDs, delLandConcords);
            //删除完成后刷新界面
            var args1 = MessageExtend.ContractAccountMsg(db, ContractAccountMessage.CONTRACTACCOUNT_Refresh, delLandIDs);
            SendMessasge(args1);
            var args2 = MessageExtend.ContractAccountMsg(db, ConcordMessage.CONCORD_REFRESH, delLandIDs);
            SendMessasge(args2);
            var args3 = MessageExtend.ContractAccountMsg(db, ContractRegeditBookMessage.ContractRegeditBook_Refresh, delLandIDs);
            SendMessasge(args3);

            delLandIDs = null;
            delLandConcords = null;
        }

        /// <summary>
        /// 主要针对承包地的删除,删除后判断是否删除完，删除完了地块，需要删除权证和合同
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.DeleteGraphicsCompleted)]
        private void DeleteGraphicsCompleted(object sender, DeleteGraphicsCompletedEventArgs e)
        {
            var deletgraphics = e.Graphics;
            if (deletgraphics == null || deletgraphics.Count == 0) return;

            var db = DataBaseSourceWork.GetDataBaseSource();
            var landStation = db.CreateContractLandWorkstation();
            var concordStation = db.CreateConcordStation();
            var regeditStation = db.CreateRegeditBookStation();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var dgitem in deletgraphics)
                {
                    var locationlayer = dgitem.Layer as VectorLayer;
                    if (locationlayer == null || locationlayer.DataSource == null) continue;
                    var elementname = locationlayer.DataSource.ElementName;
                    if (elementname != "ZD_CBD") continue;

                    var concordkey = dgitem.Object.Object.GetPropertyValue("HTID") as string;  //合同的ID
                    if (concordkey.IsNullOrEmpty()) continue;

                    Guid concordid = new Guid(concordkey);

                    var landcount = landStation.CountByConcordId(concordid);//如果承包方下面没有数据了，就删除他对应的合同和权证
                    if (landcount == 0)
                    {
                        if (concordkey != null)
                        {
                            concordStation.Delete(concordid);
                            regeditStation.Delete(concordid);
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// 绘制地块图形-包括添加标注点上的地块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.DrawFeatureInstallProperties)]
        private void DrawFeatureInstallProperties(object sender, DrawFeatureInstallPropertiesEventArgs e)
        {
            //if (currentZone == null)
            //{
            //    ShowBox("提示", "当前地域为空，请选择！");
            //    return;
            //}

            if (e.Geometry == null)
            {
                ShowBox("提示", "当前绘制为空！");
                return;
            }

            var db = e.LayerDataSource as IDbContext;
            if (db == null) return;
            e.IsHandled = false;//底层就会再添加一次。
            YuLinTu.Spatial.Geometry drawGeo = null;//获取绘制的要素

            var targetLayer = e.Layer as VectorLayer;
            if (targetLayer == null || targetLayer.DataSource == null) return;
            var elementname = targetLayer.DataSource.ElementName;
            string internalName = string.Empty;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                internalName = targetLayer.InternalName;
                drawGeo = e.Geometry;
            }));

            //如果进行拓扑处理
            if (yltuserseting.IshandleGraphicToPu)
            {
                HandleNewDrawGeoTopology(elementname, internalName, db, drawGeo, e);
            }
            else
            {
                //常规绘制
                normalDrawAction(elementname, internalName, db, drawGeo, e);
            }
        }

        //根据设置进行拓扑面块的处理
        private void HandleNewDrawGeoTopology(string elementname, string internalName, IDbContext dbcontext, YuLinTu.Spatial.Geometry drawGeo, DrawFeatureInstallPropertiesEventArgs e)
        {
            var nowscale = 0.0;
            Layer nowLayer = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                nowscale = MapControl.Scale;
                nowLayer = e.Layer;
            }));
            if (nowLayer == null)
            {
                ShowBox("提示", "当前图层为空！");
                return;
            }
            var gerrel = NewDrawGeoTopologyCheckScale(elementname, internalName, nowscale);
            if (gerrel == false) return;

            var intersectsinfo = new Dictionary<string, YuLinTu.Spatial.Geometry>();
            if (yltuserseting.HandleGraphicToPuUseMD)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var ptb = new PolygonTopologyByUserSetting(nowLayer, drawGeo);
                    ptb.Background = Brushes.Transparent;
                    ptb.Confirm += (s, a) =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            ptb.OtherDefine.IshandleGraphicToPu = true;
                            ptb.OtherDefine.HandleGraphicToPuUseMD = true;
                            ptb.config.CopyPropertiesFrom(ptb.OtherDefine);
                            ptb.systemCenter.Save<MapFoundationUserSettingDefine>();
                        }));

                        Application.Current.Dispatcher.Invoke(new Action(() => intersectsinfo = ptb.intersectGeoIDs));
                        NewDrawGeoTopologyUpdate(intersectsinfo, elementname, internalName, dbcontext, drawGeo, e);
                    };
                    var Workpage = MapControl.Properties["Workpage"] as IWorkpage;
                    Workpage.Page.ShowDialog(ptb, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            e.IsCancel = true;
                            e.AutoResetEvent.Set();
                            return;
                        }
                        else
                        {
                            e.AutoResetEvent.Set();
                        }
                    });
                }));
                e.AutoResetEvent.WaitOne();
            }
            else
            {
                var targetLayer = nowLayer as VectorLayer;
                if (targetLayer == null || targetLayer.DataSource == null) return;
                targetLayer.DataSource.GetByIntersects(drawGeo).ForEach(s => intersectsinfo.Add(s.Object.GetPropertyValue("ID").ToString(), s.Geometry));

                if (intersectsinfo.Count == 0)//如果没有相交的，就进行常规绘制
                {
                    //常规绘制
                    normalDrawAction(elementname, internalName, dbcontext, drawGeo, e);
                }
                else
                {
                    NewDrawGeoTopologyUpdate(intersectsinfo, elementname, internalName, dbcontext, drawGeo, e);
                }
            }
        }

        /// <summary>
        /// 拓扑处理具体更新处理
        /// </summary>
        private void NewDrawGeoTopologyUpdate(Dictionary<string, YuLinTu.Spatial.Geometry> intersectsinfo, string elementname, string internalName, IDbContext dbcontext, YuLinTu.Spatial.Geometry drawGeo, DrawFeatureInstallPropertiesEventArgs e)
        {
            if (yltuserseting.GraphicToPuSavebfData)//如果保留以前的数据
            {
                foreach (var item in intersectsinfo)
                {
                    drawGeo = drawGeo.Difference(item.Value);
                }
                //常规绘制
                normalDrawAction(elementname, internalName, dbcontext, drawGeo, e, false);
            }

            if (yltuserseting.GraphicToPuSaveNewData)//如果新的数据
            {
                //常规绘制
                normalDrawAction(elementname, internalName, dbcontext, drawGeo, e, false);

                var mzdwStation = dbcontext.CreateMZDWWorkStation();
                var landStation = dbcontext.CreateContractLandWorkstation();
                var zoneStation = dbcontext.CreateZoneWorkStation();
                var jbntbhqStation = dbcontext.CreateFarmLandConserveWorkStation();
                //处理相交部分的代码
                foreach (var item in intersectsinfo)
                {
                    var nowpolygonshape = item.Value.Difference(drawGeo);

                    if (elementname == "ZD_CBD")
                    {
                        var newguid = new Guid(item.Key);
                        if (newguid == null) continue;
                        var editland = landStation.Get(newguid);
                        if (editland == null) continue;
                        editland.Shape = nowpolygonshape;
                        landStation.Update(editland);
                        List<string> fList = ConcordExtend.DeserializeContractInfo(true);
                        List<string> ofList = ConcordExtend.DeserializeContractInfo(false);
                        landStation.SignConcord(editland, fList, ofList);
                    }
                    if (elementname == "JBNTBHQ")
                    {
                        var newguid = new Guid(item.Key);
                        if (newguid == null) continue;
                        var editland = jbntbhqStation.Get(newguid);
                        if (editland == null) continue;
                        editland.Shape = nowpolygonshape;
                        jbntbhqStation.Update(editland);
                    }
                    if (elementname == "MZDW")
                    {
                        var newguid = new Guid(item.Key);
                        if (newguid == null) continue;
                        var editland = mzdwStation.Get(m => m.ID == newguid).FirstOrDefault();
                        if (editland == null) continue;
                        editland.Shape = nowpolygonshape;
                        mzdwStation.Update(editland, d => d.ID == newguid);
                    }

                    if (elementname == "JCSJ_XZQY")
                    {
                        var newguid = new Guid(item.Key);
                        if (newguid == null) continue;
                        var editland = zoneStation.Get(newguid);
                        if (editland == null) continue;
                        editland.Shape = nowpolygonshape;
                        zoneStation.Update(editland);
                    }
                }
            }
        }

        /// <summary>
        /// 拓扑处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTopo_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelectedLayer == null)
            {
                ShowBox("拓扑处理", "未选择图层");
                return;
            }
            ;
            var nowSelectlayer = currentSelectedLayer as VectorLayer;
            if (nowSelectlayer.DataSource == null)
            {
                ShowBox("拓扑处理", "请先选择需要处理的图层数据");
                return;
            }
            var deal = new DealTopoData(Workpage);
            Workpage.Page.ShowMessageBox(deal, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                TaskDealTopoArgument meta = new TaskDealTopoArgument();
                string con = DataBaseSource.GetDataBaseSource().ConnectionString;

                meta.Database = con;
                meta.TableName = nowSelectlayer.DataSource.ElementName;
                meta.SmallArea = deal.SmallArea;
                meta.SharePoint = deal.SharePoint;
                meta.AreaRepeat = deal.AreaRepeat;
                meta.AreaSelfOverlap = deal.AreaSelfOverlap;
                TaskDealTopoOperation operation = new TaskDealTopoOperation();
                operation.Argument = meta;
                operation.Description = "拓扑处理";
                operation.Name = "拓扑处理";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 默认绘制操作
        /// </summary>
        private void normalDrawAction(string elementname, string internalName, IDbContext dbcontext, YuLinTu.Spatial.Geometry drawGeo, DrawFeatureInstallPropertiesEventArgs e, bool isUseTopu = true)
        {
            bool valid = true;

            VirtualPerson selectPersonNow = null;
            bool withMark = false;
            bool isLandlayer = false;
            ContractLandMark landMark = null;

            Zone usezone = null;//以绘制的图形，去获取村级一下面积交汇最大的地域，以组优先
            if (drawGeo != null && drawGeo.IsValid() == true)
            {
                usezone = zoneStation.GetGeoIntersectZoneOFMaxAea(drawGeo);
            }

            #region 不进行拓扑处理-就只进行原来的绘制操作

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (elementname != "ZD_CBD")
                {
                    try
                    {
                        e.Geometry = drawGeo;
                        e.Object.Geometry = drawGeo;
                        e.Object.Object.SetPropertyValue("ID", Guid.NewGuid().ToString());

                        switch (internalName)
                        {
                            case "zonec_Layer":
                                e.Object.Object.SetPropertyValue("DYJB", Convert.ToInt64(2));
                                break;

                            case "zoneZ_Layer":
                                e.Object.Object.SetPropertyValue("DYJB", Convert.ToInt64(1));
                                break;

                            case "zoneBoundary_Layer":
                            case "controlPoint_Layer":
                            case "farmLand_Layer":
                                if (usezone != null)
                                {
                                    e.Object.Object.SetPropertyValue("DYDM", usezone.FullCode);
                                    e.Object.Object.SetPropertyValue("DYMC", usezone.FullName);
                                }
                                else if (usezone == null && currentZone != null)
                                {
                                    e.Object.Object.SetPropertyValue("DYDM", currentZone.FullCode);
                                    e.Object.Object.SetPropertyValue("DYMC", currentZone.FullName);
                                }
                                break;

                            case "dzdw_Layer":
                            case "xzdw_Layer":
                            case "mzdw_Layer":
                                if (usezone != null)
                                {
                                    e.Object.Object.SetPropertyValue("zonename", usezone.FullName);
                                    e.Object.Object.SetPropertyValue("zonecode", usezone.FullCode);
                                }
                                else if (usezone == null && currentZone != null)
                                {
                                    e.Object.Object.SetPropertyValue("zonename", currentZone.FullName);
                                    e.Object.Object.SetPropertyValue("zonecode", currentZone.FullCode);
                                }
                                break;

                            default:
                                break;
                        }

                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                else
                {
                    var drawGeoArea = e.Geometry.Area() * projectionUnit;
                    if (drawGeoArea < 0.00015)
                    {
                        ShowBox("提示", "当前绘制面积小于0.00015亩，请重新绘制！");
                        return;
                    }
                    isLandlayer = true;
                    selectPersonNow = panel.OwerShipPanel.CurrentVirtualPerson;
                    if (selectPersonNow == null)
                    {
                        valid = false;
                        TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                        messagebox.Message = "没有承包方数据，请添加承包方！";
                        messagebox.Header = "提示";
                        messagebox.MessageGrade = eMessageGrade.Infomation;
                        messagebox.CancelButtonText = "取消";
                        var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                        if (workpage == null)
                            return;
                        workpage.Page.ShowMessageBox(messagebox);
                        return;
                    }
                    if (selectPersonNow.Status == eVirtualPersonStatus.Lock)
                    {
                        valid = false;
                        TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                        messagebox.Message = "锁定状态的承包方无法添加地块，请重新选择承包方！";
                        messagebox.Header = "提示";
                        messagebox.MessageGrade = eMessageGrade.Infomation;
                        messagebox.CancelButtonText = "取消";
                        var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                        if (workpage == null)
                            return;
                        workpage.Page.ShowMessageBox(messagebox);
                        return;
                    }
                    else
                    {
                        valid = true;
                        e.IsHandled = true;

                        if (dbcontext == null)
                        {
                            e.IsCancel = false;
                            return;
                        }
                    }
                }
            }));

            var dq = dbcontext.CreateQuery<ContractLandMark>();
            var lst = dq.Where(c => c.Shape.Disjoint(drawGeo) == false);
            withMark = lst.Count() > 0;

            if (!valid)
            {
                e.IsCancel = true;
                return;
            }
            if (isLandlayer)
            {
                if (withMark)
                {
                    landMark = lst.ToList()[0];
                    AddContractLandWithMark(selectPersonNow, currentZone, drawGeo, landMark, dbcontext, e, isUseTopu);
                }
                else
                {
                    AddContractLandWithOutMark(selectPersonNow, currentZone, drawGeo, e, internalName, isUseTopu);
                }
            }

            #endregion 不进行拓扑处理-就只进行原来的绘制操作
        }

        //检查当前绘制面要素时地图空间比例尺是否大于当前图层最大比例尺
        private bool NewDrawGeoTopologyCheckScale(string elementname, string internalName, double nowscale)
        {
            bool returnres = true;
            if (elementname == "ZD_CBD")
            {
                if (nowscale > 20000)
                {
                    ShowBox("提示", "当前比例尺过小，请重新操作");
                    returnres = false;
                }
            }
            if (elementname == "JBNTBHQ")
            {
                if (nowscale > 10000)
                {
                    ShowBox("提示", "当前比例尺过小，请重新操作");
                    returnres = false;
                }
            }
            if (elementname == "MZDW")
            {
                if (nowscale > 60000)
                {
                    ShowBox("提示", "当前比例尺过小，请重新操作");
                    returnres = false;
                }
            }
            if (internalName == "zonec_Layer")
            {
                if (nowscale > 60000)
                {
                    ShowBox("提示", "当前比例尺过小，请重新操作");
                    returnres = false;
                }
            }
            if (internalName == "zoneZ_Layer")
            {
                if (nowscale > 20000)
                {
                    ShowBox("提示", "当前比例尺过小，请重新操作");
                    returnres = false;
                }
            }
            return returnres;
        }

        /// <summary>
        /// 空间查找选择的地块(地块集合)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_FINDLANDS_COMPLETE)]
        private void FindLand(object sender, ModuleMsgArgs e)
        {
            List<FeatureObject> lstFeatureObject = null;
            TaskThreadDispatcher.Create(MapControl.Dispatcher, go =>
            {
                var dbContext = DataBaseSourceWork.GetDataBaseSource();
                List<ContractLand> lands = e.Parameter as List<ContractLand>;
                landLayer = new VectorLayer(new SQLiteGeoSource(dbContext.ConnectionString, null, typeof(ContractLand).GetAttribute<DataTableAttribute>().TableName) { UseSpatialIndex = true, GeometryType = eGeometryType.Polygon });

                if (landLayer == null)
                    return;
                foreach (var land in lands)
                {
                    var fo = landLayer.DataSource.FirstOrDefault(string.Format("{0}=\"{1}\"",
                        typeof(ContractLand).GetProperty("ID").GetAttribute<DataColumnAttribute>().ColumnName, land.ID));
                    lstFeatureObject.Add(fo);
                }
            }, null, null,
            started =>
            {
                MapControl.SelectedItems.Clear();
                lstFeatureObject = new List<FeatureObject>(100);
            },
            ended =>
            {
            },
            completed =>
            {
                Envelope env = null;
                lstFeatureObject.ForEach(c =>
                {
                    if (c == null)
                        return;

                    var graphic = new Graphic();
                    graphic.Object = c;
                    graphic.Layer = cbdlayer;
                    MapControl.SelectedItems.Add(graphic);

                    if (env == null)
                        env = c.Geometry != null ? c.Geometry.GetEnvelope() : null;
                    else
                        env.Union(c.Geometry != null ? c.Geometry.GetEnvelope() : null);
                });

                if (env == null)
                    return;

                MapControl.ZoomTo(env);   //按最小外接矩形进行定位
            }, null,
            terminated =>
            {
            }).StartAsync();
        }

        //鼠标右键导入的东西
        [MessageHandler(ID = IDMap.InstallLayerMenuItems)]
        private void InstallLayerMenuItems(object sender, InstallUIElementsEventArgs e)
        {
            var image = new AutoGrayImage()
            {
                Source = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/TableRowsOrColumnsOrCellsInsert.png")),
                SnapsToDevicePixels = true,
                Stretch = Stretch.None
            };
            var Clearimage = new AutoGrayImage()
            {
                Source = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/清空16.png")),
                SnapsToDevicePixels = true,
                Stretch = Stretch.None
            };
            var Generateimage = new AutoGrayImage()
            {
                Source = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/miInitial.png")),
                SnapsToDevicePixels = true,
                Stretch = Stretch.None
            };

            System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            importShapeItem.Visibility = Visibility.Collapsed;
            importShapeItem.Header = "加载数据";
            importShapeItem.Icon = image;
            importShapeItem.Click += ImportShapeField;
            ClearItem.Visibility = Visibility.Collapsed;
            ClearItem.Header = "清空数据";
            ClearItem.Icon = Clearimage;
            ClearItem.Click += ClearDataField;
            GenerateDataItem.Visibility = Visibility.Collapsed;
            GenerateDataItem.Header = "生成数据";
            GenerateDataItem.Icon = Generateimage;
            GenerateDataItem.Click += GenerateDataClick;

            e.Items.Insert(17, importShapeItem);
            e.Items.Insert(18, ClearItem);
            e.Items.Insert(19, GenerateDataItem);
        }

        /// <summary>
        /// 空间查看界址点
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_FINDDOT_COMPLETE)]
        private void FindDot(object sender, ModuleMsgArgs e)
        {
            FeatureObject featureOb = null;
            TaskThreadDispatcher.Create(MapControl.Dispatcher, go =>
                 {
                     var curDot = e.Parameter as BuildLandBoundaryAddressDot;
                     featureOb = dotLayer.DataSource.FirstOrDefault(string.Format("{0}=\"{1}\"", typeof(BuildLandBoundaryAddressDot).GetProperty("ID").GetAttribute<DataColumnAttribute>().ColumnName, curDot.ID));
                 }, null, null, started =>
                 {
                     MapControl.SelectedItems.Clear();
                 }, ended =>
                 {
                 }, comleted =>
                 {
                     Envelope en = null;
                     if (featureOb == null)
                         return;
                     Graphic gc = new Graphic
                     {
                         Object = featureOb,
                         Layer = dotLayer,
                     };
                     MapControl.SelectedItems.Add(gc);
                     if (en == null)
                         en = featureOb.Geometry == null ? null : featureOb.Geometry.GetEnvelope();
                     if (en == null)
                         return;
                     MapControl.ZoomTo(en);
                 }, null, terminated =>
                 {
                 }).StartAsync();
        }

        /// <summary>
        /// 编辑图形保存成功
        /// </summary>
        [MessageHandler(ID = EdtGISClient.tGIS_Edit_Geometry_Save_Success)]
        private void EditEndGraphic(object sender, MessageEditGeometrySaveSuccessEventArgs e)
        {
            try
            {
                var geo = e.Graphic.Geometry;
                if (geo == null)
                    return;
                double area = Math.Round(geo.Area() * projectionUnit, 2);
                var landOb = e.Graphic.Object.Object;
                if (landOb == null)
                    return;
                var landId = landOb.GetPropertyValue("ID");
                var args = new ModuleMsgArgs
                {
                    Name = ContractAccountMessage.CONTRACTACCOUNT_EDITGEOMETRY_COMPLETE,
                    Parameter = landId,
                    Tag = area,
                    ReturnValue = geo,
                };
                SendMessasge(args);

                //处理左侧权属栏
                var landItems = LandPanelControl.ContractLandUIItems;
                if (landItems == null)
                    return;
                Guid id;
                Guid.TryParse(landId.ToString(), out id);
                var targetItem = landItems.FirstOrDefault(c => c.ID == id);
                if (targetItem == null)
                    return;
                targetItem.ActualAreaUI = area.ToString() + "亩" + "(实)";

                //陈泽林 20161209 更新合同面积
                var targetLayer = e.Layer as VectorLayer;
                if (targetLayer == null || targetLayer.DataSource == null) return;
                var elementname = targetLayer.DataSource.ElementName;
                //不是承包地合并分离则不管
                if (elementname != "ZD_CBD") return;

                var key = landOb.GetPropertyValue("ID") as string;  //地块的ID
                Guid keyid = new Guid(key);
                //Person p = (Person)landOb;
                // 根据 Key 从数据库中把数据取出来，修改后，再保存回去。
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                VirtualPersonBusiness virtualPersonBusiness = new VirtualPersonBusiness(dbContext);
                ContractLand cl = landBusiness.GetLandById(keyid);//获取到选择的地块
                ContractLand previousCl = cl.Clone() as ContractLand;
                previousCl.ActualArea = area;
                previousCl.Shape = geo;
                ModuleMsgArgs argsEdit = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE, previousCl, previousCl.ZoneCode, previousCl);
                SendMessasge(argsEdit);
            }
            catch (Exception ex)
            {
                ShowBox("编辑图形", "编辑图形保存失败!" + ex.Message);
            }
        }

        /// <summary>
        /// 空间查看坐标点
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_FINDCOORDINATE_COMPLETE)]
        private void FindCoodinate(object sender, ModuleMsgArgs e)
        {
            var gpLayer = e.Tag as GraphicsLayer;
            if (!MapControl.InternalLayers.Contains(gpLayer))
                MapControl.InternalLayers.Add(gpLayer);
            gpLayer.Graphics.Clear();

            var list = e.Parameter as ArrayList;
            var curCoord = list[0] as GeoAPI.Geometries.Coordinate;
            var reference = list[1] as Spatial.SpatialReference;
            var spCdt = new Spatial.Coordinate(curCoord.X, curCoord.Y);
            Spatial.Geometry geoCdt = Spatial.Geometry.CreatePoint(spCdt, reference);

            if (geoCdt == null)
                return;
            Graphic gc = new Graphic
            {
                Geometry = geoCdt,
                Layer = gpLayer,
                Symbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as UISymbol,
            };
            gpLayer.Graphics.Add(gc);
            MapControl.PanTo(geoCdt);
            e.ReturnValue = MapControl;
        }

        /// <summary>
        /// 加载右键
        /// </summary>
        [MessageHandler(ID = IDMap.InstallMapContextMenu)]
        private void InstallMapContextMenu(object sender, InstallMapContextMenuEventArgs e)
        {
            #region 工具

            var menuPan = new MenuItem() { Header = "漫游" };
            menuPan.IsCheckable = true;
            menuPan.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ShowTimeZones16.png")),
                ImagePosition = eDirection.Left,
            };

            var b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToPanButtonIsCheckedConverter();

            menuPan.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(menuPan);

            var menPan = new MenuItem() { Header = "全图" };
            menPan.IsCheckable = true;
            menPan.Click += (s, a) => MapControl.ZoomTo(MapControl.FullExtend);
            menPan.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/globe.png")),
                ImagePosition = eDirection.Left,
            };
            e.Items.Add(menPan);

            var btnZoomIn = new MenuItem() { Header = "放大" };
            btnZoomIn.IsCheckable = true;
            btnZoomIn.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/拉框放大16.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToZoomInButtonIsCheckedConverter();
            btnZoomIn.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(btnZoomIn);

            var btnZoomOut = new MenuItem() { Header = "缩小" };
            btnZoomOut.IsCheckable = true;
            btnZoomOut.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/拉框缩小16.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToZoomOutButtonIsCheckedConverter();
            btnZoomOut.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(btnZoomOut);

            var btnChecked = new MenuItem() { Header = "选择" };
            btnChecked.IsCheckable = true;
            btnChecked.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/SelectMenu.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToSelectButtonIsCheckedConverter();
            btnChecked.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(btnChecked);

            var selectCancel = new MenuItem() { Header = "取消" };
            selectCancel.IsCheckable = true;
            selectCancel.Click += (s, a) => Workpage.Message.Send(this, new MsgEventArgs(IDMap.RequestCancelMapOperation));

            selectCancel.Icon = new ImageTextItem()
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ProposeNewTime.png")),
                ImagePosition = eDirection.Left,
            };
            e.Items.Add(selectCancel);

            #endregion 工具

            var sp = new System.Windows.Controls.Separator();
            e.Items.Add(sp);

            #region 影像加载

            var sbImageImport = new MenuItem() { Header = "影像加载" };
            sbImageImport.IsCheckable = true;
            sbImageImport.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ExportSavedExports.png")),
                ImagePosition = eDirection.Left,
            };
            sbImageImport.Click += SbImageImport_Click;
            e.Items.Add(sbImageImport);

            #endregion 影像加载

            #region 图形编辑

            var btnEditGeometry = new MenuItem() { Header = "编辑图形" };
            btnEditGeometry.IsCheckable = true;
            btnEditGeometry.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/DrawShape.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToEditGeometryButtonIsCheckedConverter();
            btnEditGeometry.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(btnEditGeometry);

            #endregion 图形编辑

            #region 属性编辑

            var btnEditProperties = new MenuItem() { Header = "编辑属性" };
            btnEditProperties.IsCheckable = true;
            btnEditProperties.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ReviseContents.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToEditPropertiesButtonIsCheckedConverter();
            btnEditProperties.SetBinding(MenuItem.IsCheckedProperty, b);
            e.Items.Add(btnEditProperties);

            #endregion 属性编辑

            #region 宗地分割

            var mtbLandDivision = new MenuItem() { Header = "宗地分割" };
            mtbLandDivision.IsCheckable = true;
            mtbLandDivision.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/CreateLabels.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            b.Converter = new MapActionToMapClipByLineButtonIsCheckedConverter();
            mtbLandDivision.SetBinding(MenuItem.IsCheckedProperty, b);
            mtbLandDivision.Click += MapClipMenu_Click;
            e.Items.Add(mtbLandDivision);

            #endregion 宗地分割

            #region 宗地合并

            var ddbSuperEdit = new MenuItem() { Header = "宗地合并" };
            ddbSuperEdit.IsCheckable = true;
            ddbSuperEdit.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewPrintLayoutView.png")),
                ImagePosition = eDirection.Left,
            };
            ddbSuperEdit.Click += DdbSuperEdit_Click;

            e.Items.Add(ddbSuperEdit);

            #endregion 宗地合并

            #region 区域赋值

            var sbZoneAssignment = new MenuItem() { Header = "区域赋值" };
            sbZoneAssignment.IsCheckable = true;
            sbZoneAssignment.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewMasterDocumentViewClassic.png")),
                ImagePosition = eDirection.Left,
            };
            b = new Binding("Action");
            b.Source = MapControl;
            b.Mode = BindingMode.TwoWay;
            MapControlActionMapZoneAssignmentConverter convert = new MapControlActionMapZoneAssignmentConverter();
            convert.map = MapControl;
            convert.TheWorkPage = Workpage;
            b.Converter = convert;
            sbZoneAssignment.SetBinding(MenuItem.IsCheckedProperty, b);
            sbZoneAssignment.Click += SbZoneAssignment_Click;
            e.Items.Add(sbZoneAssignment);

            #endregion 区域赋值

            sp = new System.Windows.Controls.Separator();
            e.Items.Add(sp);

            #region 数据查找

            var mtbSearch = new MenuItem() { Header = "数据查找" };
            mtbSearch.IsCheckable = true;
            mtbSearch.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/FileDocumentInspect.png")),
                ImagePosition = eDirection.Left,
            };
            mtbSearch.Click += (s, a) => Workpage.Message.Send(this, new MsgEventArgs<string>(
                 EdCore.langRequestSelectWorkpageLeftSidebarTabItem)
            { Parameter = "tabLayerSearch" });
            e.Items.Add(mtbSearch);

            #endregion 数据查找

            #region 拓扑处理

            var mtbTopo = new MenuItem() { Header = "拓扑处理" };
            mtbTopo.IsCheckable = true;
            mtbTopo.Icon = new ImageTextItem
            {
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/AutoFormatWizard.png")),
                ImagePosition = eDirection.Left,
            };
            mtbTopo.Click += (s, a) => Workpage.Message.Send(this, new MsgEventArgs<string>(
            EdCore.langRequestSelectWorkpageLeftSidebarTabItem)
            { Parameter = "tabTopology" });

            e.Items.Add(mtbTopo);

            #endregion 拓扑处理
        }

        private void MapClipMenu_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryMapClipByLine(MapControl);
        }

        #region 编辑地块编码

        private void SplitLandCodeEdit(object sender, RoutedEventArgs e)
        {
            var layers = MapControl.SelectedItems.Select(c => c.Layer).Distinct().ToList();
            if (layers.Count == 0)
            {
                ShowBox("信息提示", "请选择多个地块进行地块编码的处理！");
                return;
            }
            var map = MapControl;
            var graphics = map.SelectedItems.ToList();
            var layer = layers[0];
            TaskThreadDispatcher.Create(MapControl.Dispatcher,
            go =>
            {
                var args = new MessageSplitLandInstallEventArgs(layer, graphics, dbcontext, currentZone);
                map.Message.Send(this, args);
                if (args.IsCancel)
                    return;
                var editor = new SplitLandEdit(map, layer, graphics, dbcontext, currentZone);
                map.Dispatcher.Invoke(new Action(() =>
                {
                }));
            }, null, null,
            started =>
            {
                map.BusyUp();
            },
            ended =>
            {
                map.BusyDown();
            },
            completed =>
            {
                map.Refresh();
            }, null,
            terminated =>
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    MessageGrade = eMessageGrade.Error,
                    Message = "修改地块编码的过程中发生了一个未知错误。",
                    Header = "地块编码"
                });
            }).Start();
        }

        #endregion 编辑地块编码

        /// <summary>
        /// 宗地合并
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdbSuperEdit_Click(object sender, RoutedEventArgs e)
        {
            var layers = MapControl.SelectedItems.Select(c => c.Layer).Distinct().ToList();
            if (layers.Count != 1)
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    MessageGrade = eMessageGrade.Error,
                    Message = LanguageAttribute.GetLanguage("lang3070241"),
                    Header = LanguageAttribute.GetLanguage("lang3070240")
                });
                return;
            }

            var map = MapControl;
            var graphics = map.SelectedItems.ToList();
            string text = graphics[0].Object != null ? (graphics[0].Object.Object != null ? graphics[0].Object.Object.ToString() : null) : null;
            if (graphics.Count == 1)
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    MessageGrade = eMessageGrade.Warn,
                    Message = "请选择两个以上的地块。",
                    Header = "提示"
                });
                return;
            }
            var layer = layers[0];

            TaskThreadDispatcher.Create(MapControl.Dispatcher,
            go =>
            {
                var args = new MessageUnionGeometryInstallEventArgs(layer, graphics);

                map.Message.Send(this, args);
                if (args.IsCancel)
                    return;

                var editor = new GeometryTopologyUnion(map, layer, graphics, args.TargetIndex);
                var graphic = editor.Do() as Graphic;
                var listDeletes = graphics.Where(c => c != graphic).ToList();

                map.Dispatcher.Invoke(new Action(() =>
                {
                    map.SaveAsync(null, new Graphic[] { graphic }, listDeletes.ToArray(), () =>
                    {
                        layer.Refresh();
                        map.SelectedItems.Clear();
                        map.SelectedItems.Add(graphic);
                    }, error =>
                    {
                        Workpage.Page.ShowDialog(new MessageDialog()
                        {
                            MessageGrade = eMessageGrade.Error,
                            Message = string.Format(LanguageAttribute.GetLanguage("lang3070242"), error),
                            Header = LanguageAttribute.GetLanguage("lang3070240")
                        });
                    });
                }));
            }, null, null,
            started =>
            {
                map.BusyUp();
            },
            ended =>
            {
                map.BusyDown();
            },
            completed =>
            {
            }, null,
            terminated =>
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    MessageGrade = eMessageGrade.Error,
                    Message = string.Format(LanguageAttribute.GetLanguage("lang3070242"), terminated.Exception),
                    Header = LanguageAttribute.GetLanguage("lang3070240")
                });
            }).Start();
        }

        #endregion Methods - Message

        #region Methods

        #region 区域赋值

        private void SbZoneAssignment_Click(object sender, RoutedEventArgs e)
        {
            var mamaa = new MapActionMeasureAreaAssignment(MapControl, Workpage);
            mamaa.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            MapControl.Action = mamaa;
            return;
        }

        #endregion 区域赋值

        #region 导出选中Shape

        /// <summary>
        /// 导出选择的地块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSelectedShape(object sender, RoutedEventArgs e)
        {
            var map = MapControl;
            if (currentZone == null || currentZone.Level > eZoneLevel.Town)
            {
                ShowBox("信息提示", "请选中乡镇及以下的地域进行数据导出！");
                return;
            }
            var graphics = map.SelectedItems.ToList();
            var lands = new List<ContractLand>();

            var extPage = new ExportDataTypePage(currentZone.Name, Workpage, "导出Shp矢量文件");
            extPage.Workpage = Workpage;

            if (graphics.Count == 0 || (graphics[0].Layer.Name != "承包地" && graphics[0].Layer.Name != "自留地" &&
                graphics[0].Layer.Name != "机动地" && graphics[0].Layer.Name != "其他集体地"))
            {
                ShowBox("信息提示", "是否导出选中地域下的全部地块?", eMessageGrade.Infomation, true, true, (ab, ae) =>
                {
                    if (!(bool)ab)
                        return;
                    lands = dbcontext.CreateContractLandWorkstation().GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                    VPS = dbcontext.CreateVirtualPersonStation<LandVirtualPerson>().GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
                    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                    {
                        string saveFilePath = extPage.FileName;
                        if (string.IsNullOrEmpty(saveFilePath) || b == false)
                        {
                            return;
                        }
                        TaskExportSelectedLandShapeFile(saveFilePath, lands, extPage.EportType);
                    });
                });
                return;
            }
            else
            {
                lands = InitializeGeoLand(graphics);
                Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    string saveFilePath = extPage.FileName;
                    if (string.IsNullOrEmpty(saveFilePath) || b == false)
                    {
                        return;
                    }
                    TaskExportSelectedLandShapeFile(saveFilePath, lands, extPage.EportType);
                });
            }
        }

        #endregion 导出选中Shape

        #region 四至计算

        private void CalcLandBundary(object sender, RoutedEventArgs e)
        {
            var map = MapControl;
            if (map != null && map.SelectedItems.Count == 0)
            {
                return;
            }
            var graphics = map.SelectedItems.ToList();
            if (graphics.Count == 0 || (graphics[0].Layer.Name != "承包地" &&
                graphics[0].Layer.Name != "自留地" &&
                graphics[0].Layer.Name != "开荒地" &&
                graphics[0].Layer.Name != "机动地" &&
                graphics[0].Layer.Name != "其他集体地"))
            {
                ShowBox("信息提示", "请选中地块矢量要素进行计算！");
                return;
            }
            var lands = InitializeGeoLand(graphics);

            if (currentZone == null || currentZone.Level > eZoneLevel.Village)
            {
                ShowBox("信息提示", "选中的发包方级别地域(村、组)才能进行四至计算！");
                return;
            }
            var landcount = new AccountLandBusiness(dbcontext).CountLandByZone(currentZone.FullCode);
            if (landcount == 0)
            {
                ShowBox("信息提示", "选中的地域下未获取到其他地块数据！");
                return;
            }
            var seekLandNeighborSet = new SeekLandNeighborSetting();
            var skNbSPage = SeekLandNeighborSetPage.Getinstence(Workpage);
            Workpage.Page.ShowMessageBox(skNbSPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                //IDbContext dbContext = CreateDb();
                DictionaryBusiness dictBusiness = new DictionaryBusiness(dbcontext);
                List<Dictionary> dictList = dictBusiness.GetAll();
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
                seekLandNeighborSet.OnlyCurrentZone = skNbSPage.OnlyCurrentZone;

                //执行单个任务
                var meta = new TaskSeekLandNeighborArgument();
                meta.CurrentZone = currentZone;
                meta.Database = dbcontext;
                meta.CurrentZoneLandList = lands;
                meta.UpdateLandList = lands;
                meta.DicList = dictList;
                meta.seekLandNeighborSet = seekLandNeighborSet;

                var Seek = new TaskSeekLandNeighborOperation();
                Seek.Argument = meta;
                Seek.Description = ContractAccountInfo.SeekLandNeighbor;
                Seek.Name = ContractAccountInfo.SeekLandNeighbor;
                Seek.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    ModuleMsgArgs args = MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_SEEKLANDNEIGHBOR_COMPLETE, "", currentZone.FullCode);
                    TheBns.Current.Message.Send(this, args);
                });
                Workpage.TaskCenter.Add(Seek);
                if (ShowTaskViewer != null)
                    ShowTaskViewer();
                Seek.StartAsync();
            });
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        #endregion 导出选中Shape

        #region 区域界线生成

        private void GenerateDataClick(object sender, RoutedEventArgs e)
        {
            if (currentSelectedLayer == null) return;
            var nowSelectlayer = currentSelectedLayer as VectorLayer;
            if (nowSelectlayer.InternalName == "zoneBoundary_Layer")
            {
                GenerateData generate = new GenerateData(Workpage);
                Workpage.Page.ShowMessageBox(generate, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    TaskGenerateQYJXShapeArgument argument = new TaskGenerateQYJXShapeArgument();
                    argument.selectLineNature = generate.selectLineNature;
                    argument.selectLineType = generate.selectLineType;
                    TaskGenerateQYJXShapeOperation operation = new TaskGenerateQYJXShapeOperation();
                    operation.Argument = argument;
                    operation.Description = "生成区域界线Shape数据";
                    operation.Name = "生成Shape数据";
                    operation.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MapControl.Refresh();
                        }));
                    });
                    Workpage.TaskCenter.Add(operation);
                    if (ShowTaskViewer != null)
                    {
                        ShowTaskViewer();
                    }
                    operation.StartAsync();
                });
            }
        }

        #endregion 区域界线生成

        #region 清空数据

        /// <summary>
        /// 清空shape数据入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearDataField(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            if (currentSelectedLayer == null) return;
            var nowSelectlayer = currentSelectedLayer as VectorLayer;

            if (nowSelectlayer.InternalName == "dczd_Layer")
            {
                TaskClearDCZDShape();
            }
            else if (nowSelectlayer.InternalName == "dzdw_Layer")
            {
                TaskClearDZDWShape();
            }
            else if (nowSelectlayer.InternalName == "xzdw_Layer")
            {
                TaskClearXZDWShape();
            }
            else if (nowSelectlayer.InternalName == "mzdw_Layer")
            {
                TaskClearMZDWShape();
            }
            else if (nowSelectlayer.InternalName == "farmLand_Layer")
            {
                TaskClearFarmLandConserveShape();
            }
            else if (nowSelectlayer.InternalName == "controlPoint_Layer")
            {
                TaskClearControlPointShape();
            }
            else if (nowSelectlayer.InternalName == "zoneBoundary_Layer")
            {
                TaskClearZoneBoundaryShape();
            }
            else if (nowSelectlayer.InternalName == "cbdMark_Layer")
            {
                TaskClearZDBZ();
            }
        }

        /// <summary>
        /// 清空控制点
        /// </summary>
        private void TaskClearControlPointShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, false);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportControlPointShapeArgument argument = new TaskImportControlPointShapeArgument();
                argument.Database = dbContext;
                argument.Type = "Del";
                TaskImportControlPointShapeOperation operation = new TaskImportControlPointShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空控制点Shape数据";
                operation.Name = "清空Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 清空区域界线
        /// </summary>
        private void TaskClearZoneBoundaryShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, false);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportZoneBoundaryShapeArgument argument = new TaskImportZoneBoundaryShapeArgument();
                argument.Database = dbContext;
                argument.Type = "Del";

                TaskImportZoneBoundaryShapeOperation operation = new TaskImportZoneBoundaryShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空区域界线Shape数据";
                operation.Name = "清空Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearFarmLandConserveShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, true);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportFarmLandConserveShapeArgument argument = new TaskImportFarmLandConserveShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = clear.SelectZone;
                argument.Type = "Del";
                TaskImportFarmLandConserveShapeOperation operation = new TaskImportFarmLandConserveShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空基本农田保护区Shape数据";
                operation.Name = "清空Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearDCZDShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, false);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportDCZDShapeArgument argument = new TaskImportDCZDShapeArgument();
                argument.Database = dbContext;
                argument.Type = "Del";

                TaskImportDCZDShapeOperation operation = new TaskImportDCZDShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空调查宗地Shape数据";
                operation.Name = "清空Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearDZDWShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, true);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportDZDWShapeArgument argument = new TaskImportDZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = clear.SelectZone;
                argument.Type = "Del";

                TaskImportDZDWShapeOperation operation = new TaskImportDZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空点状地物Shape数据";
                operation.Name = "清空Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearMZDWShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, true);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportMZDWShapeArgument argument = new TaskImportMZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = clear.SelectZone;
                argument.Type = "Del";

                TaskImportMZDWShapeOperation operation = new TaskImportMZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空面状地物Shape数据";
                operation.Name = "清空Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearXZDWShape()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, true);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportXZDWShapeArgument argument = new TaskImportXZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = clear.SelectZone;
                argument.Type = "Del";
                TaskImportXZDWShapeOperation operation = new TaskImportXZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空线状地物Shape数据";
                operation.Name = "清空Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        private void TaskClearZDBZ()
        {
            ClearDataCommon clear = new ClearDataCommon(Workpage, currentZone, true);
            Workpage.Page.ShowMessageBox(clear, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportZDBZShapeArgument argument = new TaskImportZDBZShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = clear.SelectZone;
                argument.Type = "Del";
                TaskImportZDBZShapeOperation operation = new TaskImportZDBZShapeOperation();
                operation.Argument = argument;
                operation.Description = "清空宗地标注Shape数据";
                operation.Name = "清空Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        #endregion 清空数据

        #region 导入Shape

        /// <summary>
        /// 导入shape字段事件入口，根据选择的图层，弹出不同的导入界面
        /// </summary>
        private void ImportShapeField(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            if (currentSelectedLayer == null) return;
            var nowSelectlayer = currentSelectedLayer as VectorLayer;

            if (nowSelectlayer.InternalName == "dczd_Layer")
            {
                TaskImportDCZDShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "dzdw_Layer")
            {
                TaskImportDZDWShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "xzdw_Layer")
            {
                TaskImportXZDWShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "mzdw_Layer")
            {
                TaskImportMZDWShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "farmLand_Layer")
            {
                TaskImportFarmLandConserveShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "controlPoint_Layer")
            {
                TaskImportControlPointShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "zoneBoundary_Layer")
            {
                TaskImportZoneBoundaryShape(fileName, nowSelectlayer);
            }
            else if (nowSelectlayer.InternalName == "cbdMark_Layer")
            {
                TaskImportZDBZ(fileName, nowSelectlayer);
            }
            var nowSelectGroupLayer = currentSelectedLayer as MultiVectorLayer;
            if (nowSelectGroupLayer == null) return;
            if (nowSelectGroupLayer.InternalName == "cbdmult_Layer")
            {
                ImportVectorDataByRigthMenu();
            }
            if (nowSelectGroupLayer.InternalName == "zonemult_Layer")
            {
                TaskImportZoneShape(fileName, nowSelectlayer);
            }
        }

        #region 右键导入shape数据

        /// <summary>
        /// 右键导入地块图斑
        /// </summary>
        public void ImportVectorDataByRigthMenu()
        {
            var dbContext = DataBaseSourceWork.GetDataBaseSource();
            ImportCBDShapeByRightMenu addPage = new ImportCBDShapeByRightMenu(Workpage, "导入地块图斑", currentZone);
            addPage.ThePage = Workpage;
            addPage.Db = dbContext;
            Workpage.Page.ShowMessageBox(addPage, (b, r) =>
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
                meta.CurrentZone = addPage.SelectZone;
                meta.VirtualType = eVirtualType.Land;
                meta.UseContractorInfoImport = addPage.UseContractorInfoImport;
                meta.UseLandCodeBindImport = addPage.UseLandCodeBindImport;
                meta.UseOldLandCodeBindImport = addPage.UseOldLandCodeBindImport;
                meta.shapeAllcolNameList = addPage.shapeAllcolNameList;

                if (meta.CurrentZone == null)
                {
                    ShowBox("提示", "当前选择地域为空", eMessageGrade.Infomation);
                    return;
                }
                ImportLandShapeData(meta, dbContext);
            });
        }

        /// <summary>
        /// 导入图斑方法
        /// </summary>
        private void ImportLandShapeData(TaskContractAccountArgument meta, IDbContext dbContext)
        {
            TaskImportMapCBDShapeOperation import = new TaskImportMapCBDShapeOperation();
            import.Argument = meta;
            import.Description = ContractAccountInfo.ImportShapeData;
            import.Name = ContractAccountInfo.ImportShpData;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MapControl.Refresh();
                }));
            });
            Workpage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        #endregion 右键导入shape数据

        /// <summary>
        /// 导入调查宗地任务
        /// </summary>
        private void TaskImportDCZDShape(string fileName, VectorLayer nowSelectlayer)
        {
            ImportShapeField importcontrol = new ImportShapeField();
            importcontrol.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }

                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportDCZDShapeArgument argument = new TaskImportDCZDShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = currentZone;
                argument.FileName = fileName;
                argument.SelectLayerName = nowSelectlayer.Name;

                TaskImportDCZDShapeOperation operation = new TaskImportDCZDShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入Shape数据";
                operation.Name = "导入Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入点状地物任务
        /// </summary>
        private void TaskImportDZDWShape(string fileName, VectorLayer nowSelectlayer)
        {
            //ImportDZDWShape importcontrol = new ImportDZDWShape(Workpage,currentZone);
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", true);
            LoadConfig<ImportDZDWDefine> define = new LoadConfig<ImportDZDWDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.PropertiesName = "importDZDWShape";
            importcontrol.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportDZDWShapeArgument argument = new TaskImportDZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = importcontrol.SelectZone;
                argument.FileName = fileName;
                argument.SelectLayerName = nowSelectlayer.Name;
                argument.importDZDWDefine = importcontrol.ImportDataDefine as ImportDZDWDefine;
                TaskImportDZDWShapeOperation operation = new TaskImportDZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入点状地物Shape数据";
                operation.Name = "导入Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                     {
                         MapControl.Refresh();
                     }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入线状地物任务
        /// </summary>
        private void TaskImportXZDWShape(string fileName, VectorLayer nowSelectlayer)
        {
            //ImportXZDWShape importcontrol = new ImportXZDWShape(Workpage,currentZone);
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", true);
            LoadConfig<ImportXZDWDefine> define = new LoadConfig<ImportXZDWDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.PropertiesName = "importXZDWShape";
            importcontrol.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportXZDWShapeArgument argument = new TaskImportXZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = importcontrol.SelectZone;
                argument.FileName = fileName;
                argument.SelectLayerName = nowSelectlayer.Name;
                argument.importXZDWDefine = importcontrol.ImportDataDefine as ImportXZDWDefine;
                TaskImportXZDWShapeOperation operation = new TaskImportXZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入线状地物Shape数据";
                operation.Name = "导入Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入面状地物任务
        /// </summary>
        private void TaskImportMZDWShape(string fileName, VectorLayer nowSelectlayer)
        {
            //ImportMZDWShape importcontrol = new ImportMZDWShape(Workpage,currentZone);
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", true);
            LoadConfig<ImportMZDWDefine> define = new LoadConfig<ImportMZDWDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportMZDWShapeArgument argument = new TaskImportMZDWShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = importcontrol.SelectZone;
                argument.FileName = fileName;
                argument.SelectLayerName = nowSelectlayer.Name;
                //argument.importMZDWDefine = importcontrol.ImportMZDWDefine;
                argument.importMZDWDefine = importcontrol.ImportDataDefine as ImportMZDWDefine;

                TaskImportMZDWShapeOperation operation = new TaskImportMZDWShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入面状地物Shape数据";
                operation.Name = "导入Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入基本农田保护区图斑数据
        /// </summary>
        private void TaskImportFarmLandConserveShape(string fileName, VectorLayer nowSelectLayer)
        {
            //ImportFarmLandConserveShape importcontrol = new ImportFarmLandConserveShape(Workpage, "请选择Shape文件");
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", true);
            LoadConfig<ImportFarmLandConserveDefine> define = new LoadConfig<ImportFarmLandConserveDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportFarmLandConserveShapeArgument argument = new TaskImportFarmLandConserveShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = importcontrol.SelectZone;
                argument.FileName = fileName;
                argument.SelectLayerName = nowSelectLayer.Name;
                argument.ImportConserveDefine = importcontrol.ImportDataDefine as ImportFarmLandConserveDefine;

                TaskImportFarmLandConserveShapeOperation operation = new TaskImportFarmLandConserveShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入基本农田保护区Shape数据";
                operation.Name = "导入Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入控制点图斑数据
        /// </summary>
        private void TaskImportControlPointShape(string fileName, VectorLayer nowSelectLayer)
        {
            //string fileName = string.Empty;
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", false);
            LoadConfig<ImportControlPointDefine> define = new LoadConfig<ImportControlPointDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.PropertiesName = "importControlPointShape";
            //ImportControlPointShape importcontrol = new ImportControlPointShape(Workpage, "请选择Shape文件");
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportControlPointShapeArgument argument = new TaskImportControlPointShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = currentZone;
                argument.FileName = fileName;
                //argument.ImportControlPointDefine = importcontrol.ImportCpDefine;
                argument.ImportControlPointDefine = importcontrol.ImportDataDefine as ImportControlPointDefine;

                TaskImportControlPointShapeOperation operation = new TaskImportControlPointShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入控制点Shape数据";
                operation.Name = "导入Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导入区域界线图斑数据
        /// </summary>
        private void TaskImportZoneBoundaryShape(string fileName, VectorLayer nowSelectLayer)
        {
            //string fileName = string.Empty;
            //ImportZoneBoundaryShape importcontrol = new ImportZoneBoundaryShape(Workpage, "请选择Shape文件");
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", false);
            LoadConfig<ImportZoneBoundaryDefine> define = new LoadConfig<ImportZoneBoundaryDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.PropertiesName = "importZoneBoundaryShape";
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                TaskImportZoneBoundaryShapeArgument argument = new TaskImportZoneBoundaryShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = currentZone;
                argument.FileName = fileName;
                //argument.ImportZoneBoundaryDefine = importcontrol.ImportZbDefine;
                argument.ImportZoneBoundaryDefine = importcontrol.ImportDataDefine as ImportZoneBoundaryDefine;

                TaskImportZoneBoundaryShapeOperation operation = new TaskImportZoneBoundaryShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入区域界线Shape数据";
                operation.Name = "导入Shape数据";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 导出选择的地块
        /// </summary>
        private void TaskExportSelectedLandShapeFile(string fileName, List<ContractLand> lands, int exportway)
        {
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbcontext);
            TaskExportSelectedLandShapeArgument argument = new TaskExportSelectedLandShapeArgument();
            argument.DbContext = dbcontext;
            argument.SaveFilePath = fileName;
            argument.Lands = lands;// InitializeGeoLand(graphics);
            argument.VPS = VPS;
            argument.DictList = dictBusiness.GetAll();
            argument.CurrentZone = currentZone;
            argument.ExportWay = exportway;
            var operation = new TaskExportSelectedLandShapeOperation(dbcontext);
            operation.Argument = argument;
            operation.Name = "导出矢量文件";
            operation.Description = "导出选中图斑为shape文件";
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 导入行政地域
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="nowSelectLayer"></param>
        private void TaskImportZoneShape(string fileName, VectorLayer nowSelectLayer)
        {
            IDbContext db = DataBaseSourceWork.GetDataBaseSource();
            ImportZoneShip zoneShip = new ImportZoneShip();
            zoneShip.Workpage = Workpage;
            zoneShip.Db = db;
            zoneShip.Header = "加载数据";
            Workpage.Page.ShowDialog(zoneShip, (b, r) =>
            {
                if (string.IsNullOrEmpty(zoneShip.FileName) || b == false)
                {
                    return;
                }
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.FileName = zoneShip.FileName;
                meta.IsClear = false;
                meta.ArgType = eZoneArgType.ImportShape;
                meta.Database = db;
                meta.Define = ZoneDefine.GetIntence();
                TaskZoneOperation import = new TaskZoneOperation();
                import.Argument = meta;
                import.Description = ZoneInfo.ImportShapeComment;
                import.Name = ZoneInfo.ImportShape;
                import.Completed += new TaskCompletedEventHandler((o, e) =>
                {
                    Workpage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_IMPORTSHAPE_COMPLETE, true));
                });
                import.Terminated += new TaskTerminatedEventHandler((o, e) =>
                {
                    Workpage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_IMPORTSHAPE_COMPLETE, false));
                });
                Workpage.TaskCenter.Add(import);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                import.StartAsync();
            });

            //ImportZoneShip importZone = new ImportZoneShip();
            //importZone.Header = "加载数据";
            //importZone.Workpage = Workpage;
            //Workpage.Page.ShowMessageBox(importZone, (b, r) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //});
        }

        /// <summary>
        /// 导入宗地标注
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="nowSelectLayer"></param>
        private void TaskImportZDBZ(string fileName, VectorLayer nowSelectlayer)
        {
            ImportDataCommon importcontrol = new ImportDataCommon(Workpage, currentZone, "加载数据", true);
            LoadConfig<ImportZDBZDefine> define = new LoadConfig<ImportZDBZDefine>();
            importcontrol.configCopy = define.GetConfig();
            importcontrol.infos = define.getPropertyInfo();
            importcontrol.PropertiesName = "importZoneBoundaryShape";
            Workpage.Page.ShowMessageBox(importcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                define.SaveConfig(importcontrol.ImportDataDefine);
                fileName = importcontrol.FileName;
                IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
                var zoneStation = dbContext.CreateZoneWorkStation();
                Zone zone = zoneStation.Get(importcontrol.ComBusinessDefine.CurrentZoneFullCode);
                TaskImportZDBZShapeArgument argument = new TaskImportZDBZShapeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = zone;
                argument.FileName = fileName;
                argument.DotAllcolNameList = importcontrol.DotAllcolNameList;
                TaskImportZDBZShapeOperation operation = new TaskImportZDBZShapeOperation();
                operation.Argument = argument;
                operation.Description = "导入宗地标注Shape数据";
                operation.Name = "导入Shape";
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MapControl.Refresh();
                    }));
                });
                Workpage.TaskCenter.Add(operation);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        #endregion 导入Shape

        /// <summary>
        /// 添加地块-没有标注层或标注的情况下
        /// </summary>
        private void AddContractLandWithOutMark(VirtualPerson selectPersonNow, Zone selectZone, YuLinTu.Spatial.Geometry geometry, DrawFeatureInstallPropertiesEventArgs e, string internalName = null, bool isUseTopu = true)
        {
            IDbContext dbContext = null;
            double area = 0;
            object idObject = null;
            ContractLand newLand = new ContractLand();
            bool useAlt = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                dbContext = e.LayerDataSource as IDbContext;
                area = Math.Round(geometry.Area() * projectionUnit, 2);
                idObject = e.Object.Object.GetPropertyValue("ID");
                if (idObject == null)
                {
                    idObject = Guid.NewGuid();
                }
            }));

            if (dbContext == null)
            {
                ShowBox("提示", "当前图层对应数据源为空，请检查!");
                return;
            }
            if (idObject == null)
            {
                ShowBox("提示", "当前要素ID为空，请检查!");
                return;
            }

            newLand.LandCategory = ((int)eLandCategoryType.ContractLand).ToString();//地块类别
            if (!string.IsNullOrEmpty(internalName))
            {
                if (internalName == "dklb_zld_Layer")
                    newLand.LandCategory = ((int)eLandCategoryType.PrivateLand).ToString();
                else if (internalName == "dklb_khd_Layer")
                    newLand.LandCategory = ((int)eLandCategoryType.WasteLand).ToString();
                else if (internalName == "dklb_jdd_Layer")
                    newLand.LandCategory = ((int)eLandCategoryType.MotorizeLand).ToString();
                else if (internalName == "dklb_qtjttd_Layer")
                    newLand.LandCategory = ((int)eLandCategoryType.CollectiveLand).ToString();
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ////按住Alt弹窗
                if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    var dlg = new ContractLandPage(true);
                    dlg.DataSource = dbContext;
                    dlg.MapControl = MapControl;
                    if (!MapControl.InternalLayers.Contains(layerHover))
                    {
                        layerHover.Graphics.Clear();
                        MapControl.InternalLayers.Add(layerHover);
                    }
                    dlg.layerHover = layerHover;

                    newLand.ID = new DotNetTypeConverter().To<Guid>(idObject);
                    newLand.Shape = (geometry as YuLinTu.Spatial.Geometry);
                    newLand.ActualArea = area;
                    newLand.AwareArea = area;

                    dlg.Workpage = this.Workpage;
                    dlg.CurrentLand = newLand;
                    dlg.CurrentPerson = selectPersonNow;
                    dlg.CurrentZone = selectZone;

                    Workpage.Page.ShowMessageBox(dlg, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            e.IsCancel = true;
                            if (isUseTopu)
                            {
                                e.AutoResetEvent.Set();
                            }
                            return;
                        }
                        //向地块ListBox中添加地块信息
                        var landUI = ContractLandUI.ContractLandUIConvert(dlg.CurrentLand);
                        LandPanelControl.ContractLandUIItems.Add(landUI);
                        if (isUseTopu)
                        {
                            e.AutoResetEvent.Set();
                        }
                    });
                }
                else
                {
                    useAlt = false;
                }
            }));

            if (!useAlt)
            {
                newLand.Shape = YuLinTu.Spatial.Geometry.FromBytes(geometry.AsBinary(), 0);
                newLand.Shape.SpatialReference = geometry.SpatialReference;

                newLand.ID = new DotNetTypeConverter().To<Guid>(idObject);
                newLand.ActualArea = area;
                newLand.AwareArea = area;

                AgricultureLandExpand landexp = new AgricultureLandExpand();
                newLand.LandExpand = landexp;

                newLand.ZoneCode = currentZone.FullCode;
                newLand.ZoneName = currentZone.FullName;
                newLand.OwnerId = selectPersonNow.ID;
                newLand.OwnerName = selectPersonNow.Name;

                //二级编码
                newLand.LandCode = "011";
                newLand.ConstructMode = ((int)eConstructMode.Family).ToString();//承包方式
                newLand.LandLevel = ((int)eContractLandLevel.UnKnow).ToString();//地力等级
                newLand.Purpose = ((int)eLandPurposeType.Planting).ToString();//土地用途
                newLand.PlatType = ((int)ePlantingType.Other).ToString();//土地用途
                newLand.PlantType = ((int)ePlantProtectType.UnKnown).ToString();//耕保类型
                newLand.ManagementType = ((int)eManageType.Other).ToString();//经营方式
                newLand.TransferType = ((int)eTransferType.Other2).ToString();//流转方式

                newLand.IsFlyLand = false;
                newLand.IsTransfer = false;
                newLand.IsFarmerLand = true;

                newLand.LandName = "耕地";
                try
                {
                    var landStation = dbContext.CreateContractLandWorkstation();
                    var landBusiness = new AccountLandBusiness(dbContext);
                    newLand.LandNumber = landBusiness.GetNewLandNumber(currentZone.FullCode);
                    bool repeat = landBusiness.IsLandNumberReapet(newLand.LandNumber, newLand.ID, currentZone.FullCode);
                    if (repeat)
                    {
                        newLand.LandNumber = landBusiness.GetNewLandNumber(currentZone.FullCode);
                    }
                    string surverNumber = "";
                    if (newLand.LandNumber != null)
                        surverNumber = newLand.LandNumber.Length >= 5 ? newLand.LandNumber.Substring(newLand.LandNumber.Length - 5) : newLand.LandNumber.PadLeft(5, '0');
                    newLand.SurveyNumber = surverNumber;
                    var result = landStation.Add(newLand);
                }
                catch (Exception)
                { }

                //向地块ListBox中添加地块信息
                Application.Current.Dispatcher.Invoke(new Action(() =>
                  {
                      var landUI = ContractLandUI.ContractLandUIConvert(newLand);
                      LandPanelControl.ContractLandUIItems.Add(landUI);

                      ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTLAND_ADD_COMPLETE, newLand, selectZone.FullCode);
                      SendMessasge(args);
                      if (isUseTopu)
                      {
                          e.AutoResetEvent.Set();
                      }
                  }));
            }
            if (isUseTopu)
            {
                e.AutoResetEvent.WaitOne();
            }
        }

        /// <summary>
        /// 添加地块-有标注层的情况下
        /// </summary>
        private void AddContractLandWithMark(VirtualPerson selectPersonNow, Zone selectZone, YuLinTu.Spatial.Geometry geometry, ContractLandMark landMark, IDbContext db, DrawFeatureInstallPropertiesEventArgs e, bool isUseTopu = true)
        {
            object idObject = null;
            var area = Math.Round(geometry.Area() * projectionUnit, 2);
            bool useAlt = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                idObject = Guid.NewGuid();
            }));

            ContractLand newLand = new ContractLand();
            newLand.CopyPropertiesFrom(landMark);
            newLand.Shape = geometry as YuLinTu.Spatial.Geometry;
            newLand.ActualArea = area;
            newLand.AwareArea = area;
            newLand.ID = new DotNetTypeConverter().To<Guid>(idObject);

            ZoneDataBusiness zonebus = new ZoneDataBusiness();
            zonebus.Station = db == null ? null : db.CreateZoneWorkStation();
            if (newLand.ZoneCode.IsNullOrEmpty())
            {
                ShowBox("提示", "承包地标注坐落编码为空");
                return;
            }

            Zone newLandZone = zonebus.Get(newLand.ZoneCode);

            VirtualPersonBusiness virtualbus = new VirtualPersonBusiness(db);
            eVirtualType virtualtype = eVirtualType.Land;
            virtualbus.VirtualType = virtualtype;
            VirtualPerson nowVirtualPerson;
            if (newLand.OwnerId == null || (nowVirtualPerson = virtualbus.Get(newLand.OwnerId.Value)) == null)
            {
                nowVirtualPerson = new VirtualPerson();
                nowVirtualPerson.Name = newLand.OwnerName;
                nowVirtualPerson.ZoneCode = newLandZone.FullCode;
                nowVirtualPerson.FamilyExpand.ContractorType = eContractorType.Farmer;
                nowVirtualPerson.Status = eVirtualPersonStatus.Right;
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //按住Alt弹窗
                if (Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    var dlg = new ContractLandPage(true);
                    dlg.DataSource = db;
                    dlg.CurrentLand = newLand;
                    dlg.CurrentZone = newLandZone;
                    dlg.Workpage = this.Workpage;
                    dlg.CurrentPerson = nowVirtualPerson;
                    dlg.MapControl = MapControl;
                    if (!MapControl.InternalLayers.Contains(layerHover))
                    {
                        layerHover.Graphics.Clear();
                        MapControl.InternalLayers.Add(layerHover);
                    }
                    dlg.layerHover = layerHover;

                    Workpage.Page.ShowMessageBox(dlg, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            e.IsCancel = true;
                            if (isUseTopu)
                            {
                                e.AutoResetEvent.Set();
                            }
                            return;
                        }
                        //向地块ListBox中添加地块信息
                        var landUI = ContractLandUI.ContractLandUIConvert(dlg.CurrentLand);
                        LandPanelControl.ContractLandUIItems.Add(landUI);
                        if (isUseTopu)
                        {
                            e.AutoResetEvent.Set();
                        }
                    });
                }
                else
                {
                    useAlt = false;
                }
            }));

            if (!useAlt)
            {
                var landStation = db.CreateContractLandWorkstation();
                landStation.Add(newLand);

                //向地块ListBox中添加地块信息
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var landUI = ContractLandUI.ContractLandUIConvert(newLand);
                    LandPanelControl.ContractLandUIItems.Add(landUI);
                    ModuleMsgArgs args = MessageExtend.ContractAccountMsg(db, ContractAccountMessage.CONTRACTLAND_ADD_COMPLETE, newLand, selectZone.FullCode);
                    SendMessasge(args);
                    if (isUseTopu)
                    {
                        e.AutoResetEvent.Set();
                    }
                }));
            }
            if (isUseTopu)
            {
                e.AutoResetEvent.WaitOne();
            }
        }

        #region 其他方法

        private List<ContractLand> InitializeGeoLand(List<Graphic> Graphics)
        {
            List<ContractLand> lands = new List<ContractLand>();
            List<VirtualPerson> vps = new List<VirtualPerson>();
            foreach (var entity in Graphics)
            {
                ContractLand land = new ContractLand();
                VirtualPerson vp = new VirtualPerson();
                KeyValueList<string, string> keyValues = new KeyValueList<string, string>();

                var entities = entity.Object.Object.ToString().Split(',').ToList();
                entities.ForEach(x =>
                {
                    x.Trim();
                    string[] parts = x.Split('=');
                    if (parts.Length == 2)
                    {
                        string leftSide = parts[0].Trim();
                        string rightSide = parts[1].Trim();
                        keyValues.Add(leftSide, rightSide);
                    }
                });
                land = LandDataProcessing(entity, keyValues);
                vp = VpDataProcessing(keyValues);
                lands.Add(land);
                if (vp != null)
                    vps.Add(vp);
            }
            VPS = vps;
            return lands;
        }

        private VirtualPerson VpDataProcessing(KeyValueList<string, string> keyValues)
        {
            //var vpname = keyValues.FirstOrDefault(x => x.Key == "QLRMC").Value;
            var vpid = keyValues.FirstOrDefault(x => x.Key == "QLRBS").Value;
            if (string.IsNullOrEmpty(vpid))
                return null;
            IVirtualPersonWorkStation<LandVirtualPerson> vpStation = dbcontext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vp = vpStation.Get(Guid.Parse(vpid));
            return vp;
        }

        private ContractLand LandDataProcessing(Graphic graphic, KeyValueList<string, string> keyValues)
        {
            ContractLand land = graphic.Object.Object.ConvertTo<ContractLand>();
            land.LandCategory = keyValues.FirstOrDefault(x => x.Key.Equals("DKLB"))?.Value;
            land.LandLevel = keyValues.FirstOrDefault(x => x.Key.Equals("DLDJ"))?.Value;
            land.PlantType = keyValues.FirstOrDefault(x => x.Key.Equals("GBLX"))?.Value;
            land.OwnRightType = keyValues.FirstOrDefault(x => x.Key.Equals("QSXZ"))?.Value;
            var tzmj = keyValues.FirstOrDefault(x => x.Key.Equals("TZMJ"))?.Value;
            if (!string.IsNullOrEmpty(tzmj))
                land.TableArea = double.Parse(tzmj);
            var sfjbnt = keyValues.FirstOrDefault(x => x.Key.Equals("SFJBNT"))?.Value;
            if (!string.IsNullOrEmpty(sfjbnt))
                land.IsFarmerLand = bool.Parse(sfjbnt);
            land.Purpose = keyValues.FirstOrDefault(x => x.Key.Equals("TDYT"))?.Value;
            land.ManagementType = keyValues.FirstOrDefault(x => x.Key.Equals("JYFS"))?.Value;
            var htid = keyValues.FirstOrDefault(x => x.Key.Equals("HTID"))?.Value;
            if (!string.IsNullOrEmpty(htid))
                land.ConcordId = Guid.Parse(htid);
            land.Founder = keyValues.FirstOrDefault(x => x.Key.Equals("CJZ"))?.Value;
            var ctime = keyValues.FirstOrDefault(x => x.Key.Equals("CJSJ"))?.Value;
            if (!string.IsNullOrEmpty(ctime))
                land.CreationTime = DateTime.Parse(ctime);
            land.Modifier = keyValues.FirstOrDefault(x => x.Key.Equals("ZHXGZ"))?.Value;
            var mtime = keyValues.FirstOrDefault(x => x.Key.Equals("ZHXGSJ"))?.Value;
            if (!string.IsNullOrEmpty(mtime))
                land.ModifiedTime = DateTime.Parse(mtime);

            var istrans = keyValues.FirstOrDefault(x => x.Key.Equals("SFLZ"))?.Value;
            land.IsTransfer = string.IsNullOrEmpty(istrans) ? false : bool.Parse(istrans);
            land.ConstructMode = keyValues.FirstOrDefault(x => x.Key.Equals("CBFS"))?.Value;

            var isstock = keyValues.FirstOrDefault(x => x.Key.Equals("SFQGDK"))?.Value;
            land.IsStockLand = string.IsNullOrEmpty(isstock) ? false : bool.Parse(isstock);

            land.Name = keyValues.FirstOrDefault(x => x.Key.Equals("DKMC"))?.Value;
            land.ParcelNumber = keyValues.FirstOrDefault(x => x.Key.Equals("ZDBM"))?.Value;
            land.SurveyNumber = keyValues.FirstOrDefault(x => x.Key.Equals("DCBM"))?.Value;
            land.LandNumber = keyValues.FirstOrDefault(x => x.Key.Equals("DKBM"))?.Value;
            land.CadastralNumber = keyValues.FirstOrDefault(x => x.Key.Equals("DJBM"))?.Value;
            land.ZoneCode = keyValues.FirstOrDefault(x => x.Key.Equals("ZLDM"))?.Value;
            land.ZoneName = keyValues.FirstOrDefault(x => x.Key.Equals("ZLMC"))?.Value;
            land.SenderName = keyValues.FirstOrDefault(x => x.Key.Equals("QSDWMC"))?.Value;
            land.SenderCode = keyValues.FirstOrDefault(x => x.Key.Equals("QSDWDM"))?.Value;
            land.OwnerName = keyValues.FirstOrDefault(x => x.Key.Equals("QLRMC"))?.Value;
            var qlrid = keyValues.FirstOrDefault(x => x.Key.Equals("QLRBS"))?.Value;
            if (!string.IsNullOrEmpty(qlrid))
                land.OwnerId = Guid.Parse(qlrid);
            land.LandCode = keyValues.FirstOrDefault(x => x.Key.Equals("TDLYLX")).Value;
            land.LandName = keyValues.FirstOrDefault(x => x.Key.Equals("TDLYLXMC")).Value;
            land.ActualArea = double.Parse(keyValues.FirstOrDefault(x => x.Key.Equals("SCMJ")).Value);
            land.AwareArea = double.Parse(keyValues.FirstOrDefault(x => x.Key.Equals("BZMJ")).Value);
            land.Opinion = keyValues.FirstOrDefault(x => x.Key.Equals("DKXXXGYJ")).Value;
            land.Comment = keyValues.FirstOrDefault(x => x.Key.Equals("DKBZXX")).Value;
            land.NeighborEast = keyValues.FirstOrDefault(x => x.Key.Equals("DKDZ")).Value;
            land.NeighborWest = keyValues.FirstOrDefault(x => x.Key.Equals("DKXZ")).Value;
            land.NeighborSouth = keyValues.FirstOrDefault(x => x.Key.Equals("DKNZ")).Value;
            land.NeighborNorth = keyValues.FirstOrDefault(x => x.Key.Equals("DKBZ")).Value;
            land.OldLandNumber = keyValues.FirstOrDefault(x => x.Key.Equals("QQDKBM"))?.Value;
            return land;
        }

        /// <summary>
        /// 检查提交数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCommitData(ContractLand currentLand, DrawFeatureInstallPropertiesEventArgs e)
        {
            bool right = true;
            var db = e.LayerDataSource as IDbContext;
            AccountLandBusiness landBusiness = new AccountLandBusiness(db);
            if (landBusiness == null)
            {
                landBusiness = new AccountLandBusiness(db);
            }
            //if (string.IsNullOrEmpty(currentLand.LandNumber))
            //{
            //    string number = landBusiness.GetNewLandNumber(currentZone.FullCode);
            //    currentLand.LandNumber = number;  //生成编码
            //}
            //bool repeat = landBusiness.IsLandNumberReapet(currentLand.LandNumber, currentLand.ID);
            //if (repeat)
            //{
            //    Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            //    {
            //        Header = ContractAccountInfo.LandInfo,
            //        Message = ContractAccountInfo.LandNumberExist,
            //        MessageGrade = eMessageGrade.Error,
            //    });
            //    right = false;
            //}

            landBusiness.AddLand(currentLand);
            //此处做输入值判断
            //if (currentLand.ActualArea == 0)
            //{
            //    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.ActualAreaZero);
            //    right = false;
            //}
            //if (string.IsNullOrEmpty(currentLand.LandNumber))
            //{
            //    ShowBox(ContractAccountInfo.LandInfo, ContractAccountInfo.LandNumberEmpty);
            //    right = false;
            //}
            return right;
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, bool hasConfirm = true, bool hasCancel = true, Action<bool?, eCloseReason> action = null)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var Workpage = MapControl.Properties["Workpage"] as IWorkpage;

                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = title,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                    ConfirmButtonVisibility = hasConfirm ? Visibility.Visible : Visibility.Collapsed,
                    CancelButtonVisibility = hasCancel ? Visibility.Visible : Visibility.Collapsed,
                }, action);
            }));
        }

        /// <summary>
        /// 坐标系发生改变时获取坐标单位
        /// </summary>
        private void MapControl_SpatialReferenceChanged(object sender, EventArgs e)
        {
            RefreshMapControlSpatialUnit();
        }

        /// <summary>
        /// 坐标系发生改变时获取坐标单位，具体实现
        /// </summary>
        private void RefreshMapControlSpatialUnit()
        {
            var sr = MapControl.SpatialReference;
            if (sr == null || sr.WKID == 0)
            {
                SRtb.Text = "Unknown";
                return;
            }
            var projectionInfo = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(MapControl.SpatialReference);
            if (projectionInfo == null) return;

            //更换转换单位
            try
            {
                if (MapControl.SpatialReference.IsPROJCS())
                {
                    SRtb.Text = projectionInfo.Name;
                    switch (projectionInfo.Unit.Name)
                    {
                        case "Kilometer":
                            projectionUnit = 1500;
                            break;

                        case "Meter":
                            projectionUnit = 0.0015;
                            break;

                        case "Decimeter":
                            projectionUnit = 0.000015;
                            break;

                        case "Centimeter":
                            projectionUnit = 1.5 * Math.Pow(Math.E, -7);
                            break;

                        case "Millimeter":
                            projectionUnit = 1.5 * Math.Pow(Math.E, -9);
                            break;

                        case "Mile":
                            projectionUnit = 3884.9821655;
                            break;

                        case "Foot":
                            projectionUnit = 0.0001394;
                            break;

                        case "Yard":
                            projectionUnit = 0.0012542;
                            break;

                        case "Inch":
                            projectionUnit = 9.6774 * Math.Pow(Math.E, -7);
                            break;

                        default:
                            projectionUnit = 0.0015;
                            break;
                    }
                }
                else if (MapControl.SpatialReference.IsGEOGCS() || !MapControl.SpatialReference.IsValid())
                {
                    projectionUnit = 0.0015;
                    SRtb.Text = projectionInfo.GeographicInfo.Name;
                }
            }
            catch
            {
                projectionUnit = 0.0015;
            }
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

        #endregion 其他方法

        #endregion Methods

        #endregion Methods
    }
}