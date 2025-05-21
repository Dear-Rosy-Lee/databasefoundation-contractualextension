/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 * 添加测量地图面积
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.DiagramFoundation
{
    public class MapControlActionLayoutSelectRectangle : MapControlAction, IUndoRedoable, ICancelable, IConfirmable
    {
        #region Properties
        private DiagramFoundationLabelCommonSetting dflcsetting = DiagramFoundationLabelCommonSetting.GetIntence();
        /// <summary>
        /// 保存最后的绘制图形及结果，用于结果展示
        /// </summary>
        private GraphicsLayer layerEndDisplay = new GraphicsLayer();
        private Graphic graphicEndDisplay = null;

        /// <summary>
        /// 获取父控件
        /// </summary>
        private Grid container = null;
        /// <summary>
        /// 清除按钮
        /// </summary>
        private MetroButton clearGraphicBtn = null;

        /// <summary>
        /// 空间参考系单位
        /// </summary>
        public string projectionUnit { get; set; }

        #endregion

        #region Fields
        private DrawRectangle draw = null;
        private bool isDrawing = false;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionLayoutSelectRectangle(MapControl map)
            : base(map)
        {

        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = true;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
                pDragger.EnableMiddle(true);

            draw = new DrawRectangle(MapControl);
            draw.Begin += draw_Begin;
            draw.End += draw_End;
            draw.VertexAdded += draw_VertexAdded;
            draw.CanUndoChanged += draw_UndoStateChanged;
            draw.CanRedoChanged += draw_RedoStateChanged;
            draw.MarkSymbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as MarkSymbol;
            draw.LineSymbol = Application.Current.TryFindResource("UISymbol_Line_Measure") as LineSymbol;
            draw.FillSymbol = Application.Current.TryFindResource("UISymbol_Fill_Measure") as FillSymbol;
            draw.TrackingMouseMove = MapControl.TrackingMouseMoveWhenDraw;

            draw.Activate();

            MapControl.SnapModeChanged += MapControl_SnapModeChanged;
            MapControl.MouseMove += MapControl_MouseMove;

            MapControl.InternalLayers.Add(layerEndDisplay);

            //获取控件参考单位
            MapControl.SpatialReferenceChanged += MapControl_SpatialReferenceChanged;
            this.RefreshMapControlSpatialUnit();
            //获取父控件，并且定义一个消除按钮
            container = MapControl.GetParent<Grid>();
            clearGraphicBtn = new MetroButton();
            clearGraphicBtn.ToolTip = "消除绘制";
            clearGraphicBtn.Click += btn_Click;
            clearGraphicBtn.HorizontalAlignment = HorizontalAlignment.Right;
            clearGraphicBtn.VerticalAlignment = VerticalAlignment.Top;
            clearGraphicBtn.Margin = new Thickness(10, 10, 20, 10);
            clearGraphicBtn.Content = "清除绘制";
            clearGraphicBtn.Background = Application.Current.TryFindResource("Metro_Window_Style_Background_Content") as Brush;
            clearGraphicBtn.BorderBrush = Application.Current.TryFindResource("Metro_Window_Style_BorderBrush_Default") as Brush;
            container.Children.Add(clearGraphicBtn);
        }

        protected override void OnShutdown()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = false;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
                pDragger.EnableMiddle(false);

            MapControl.SnapModeChanged -= MapControl_SnapModeChanged;
            MapControl.MouseMove -= MapControl_MouseMove;

            draw.Deactivate();
            draw.Begin -= draw_Begin;
            draw.End -= draw_End;
            draw.VertexAdded -= draw_VertexAdded;

            draw.CanUndoChanged -= draw_UndoStateChanged;
            draw.CanRedoChanged -= draw_RedoStateChanged;
            draw = null;

            if (MapControl.InternalLayers.Contains(layerEndDisplay))
                MapControl.InternalLayers.Remove(layerEndDisplay);

            MapControl.SpatialReferenceChanged -= MapControl_SpatialReferenceChanged;

            clearGraphicBtn.Click -= btn_Click;
            if (container.Children.Contains(clearGraphicBtn))
            {
                container.Children.Remove(clearGraphicBtn);
            }

            clearGraphicBtn = null;
            container = null;

        }

        #endregion

        #region Methods - Events

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
            try
            {
                if (MapControl.SpatialReference.IsPROJCS())
                {
                    var projectionInfo = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(MapControl.SpatialReference);

                    if (projectionInfo == null) return;
                    switch (projectionInfo.Unit.Name)
                    {
                        case "Kilometer":
                            projectionUnit = "平方千米";
                            break;
                        case "Hictares":
                            projectionUnit = "公顷";
                            break;
                        case "Meter":
                            projectionUnit = "平方米";
                            break;
                        case "Decimeter":
                            projectionUnit = "平方分米";
                            break;
                        case "Centimeter":
                            projectionUnit = "平方厘米";
                            break;
                        case "Millimeter":
                            projectionUnit = "平方毫米";
                            break;
                        case "Ares":
                            projectionUnit = "公亩";
                            break;
                        case "Mile":
                            projectionUnit = "平方英里";
                            break;
                        case "Acres":
                            projectionUnit = "亩";
                            break;
                        case "Foot":
                            projectionUnit = "平方英尺";
                            break;
                        case "Yard":
                            projectionUnit = "平方码";
                            break;
                        case "Nautical Mile":
                            projectionUnit = "海哩";
                            break;
                        case "Inch":
                            projectionUnit = "英寸";
                            break;
                        default:
                            projectionUnit = "Unkown";
                            break;
                    }
                }
                else if (MapControl.SpatialReference.IsGEOGCS() || !MapControl.SpatialReference.IsValid())
                {
                    projectionUnit = "Unkown";
                }
            }
            catch
            {
                projectionUnit = "Unkown";
            }
        }

        /// <summary>
        /// 清除按钮，清除所有绘制
        /// </summary>       
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            draw.Cancel();

            if (layerEndDisplay != null)
            {
                layerEndDisplay.Graphics.Clear();
            }
        }

        private void MapControl_SnapModeChanged(object sender, EventArgs e)
        {
            if (draw == null)
                return;

            draw.SnapMode = MapControl.SnapMode;
        }

        /// <summary>
        /// 复制并显示最终绘制结果和面积，先添加要素，后添加标记
        /// </summary>       
        private void draw_End(object sender, EditGeometryEndEventArgs e)
        {

            isDrawing = false;
            if (CanCancelChanged != null)
                CanCancelChanged(this, new EventArgs());
            if (CanConfirmChanged != null)
                CanConfirmChanged(this, new EventArgs());

            //先保留显示绘制的要素
            if (e.Geometry != null && e.Geometry.IsValid())
            {
                graphicEndDisplay = new Graphic();
                graphicEndDisplay.Geometry = e.Geometry;
                graphicEndDisplay.Symbol = Application.Current.TryFindResource("UISymbol_Fill_Measure") as UISymbol;
                layerEndDisplay.Graphics.Add(graphicEndDisplay);

                var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                if (workpage == null) return;

                var currentZoneCode = workpage.Properties.TryGetValue<string>("CurrentZoneCode", null);
                string templateFileName = "Visio.A0.H.yltvd"; ;//模板名称，是那种模式的模板

                switch (dflcsetting.UseLayoutModel)
                {
                    case "A0_横版":
                        templateFileName = "Visio.A0.H.yltvd";
                        break;
                    case "A0_竖版":
                        templateFileName = "Visio.A0.V.yltvd";
                        break;
                    case "A1_横版":
                        templateFileName = "Visio.A1.H.yltvd";
                        break;
                    case "A1_竖版":
                        templateFileName = "Visio.A1.V.yltvd";
                        break;
                    case "A3_横版":
                        templateFileName = "Visio.A3.H.yltvd";
                        break;
                    case "A3_竖版":
                        templateFileName = "Visio.A3.V.yltvd";
                        break;
                    case "A4_横版":
                        templateFileName = "Visio.A4.H.yltvd";
                        break;
                    case "A4_竖版":
                        templateFileName = "Visio.A4.V.yltvd";
                        break;
                }

                var task = new MapToVisiosViewTask()
                {
                    Name = "导出到公示图",
                    Description = "将地图中的数据做适当处理并导出到制图工具中",
                    Argument = new MapToVisiosViewTaskArgument()
                    {
                        currentZoneCode = currentZoneCode,
                        Workspace = workpage.Workspace as ITheWorkspace,
                        MapControl = MapControl,
                        ExportGeometryOfExtend = e.Geometry,
                        DestinationDatabaseFileName = System.IO.Path.Combine(TheApp.Current.GetDataPath(), "Diagrams", Guid.NewGuid().ToString(), "db.sqlite"),
                        TemplateFileName = System.IO.Path.Combine(TheApp.GetApplicationPath(), "Template", templateFileName)
                    }
                };

                workpage.TaskCenter.Add(task);
                task.StartAsync();

                workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));

            }
        }

        private void draw_Begin(object sender, EditGeometryBeginEventArgs e)
        {
            isDrawing = true;
            if (CanCancelChanged != null)
                CanCancelChanged(this, new EventArgs());
            if (CanConfirmChanged != null)
                CanConfirmChanged(this, new EventArgs());

        }

        private void draw_VertexAdded(object sender, EditGeometryVertexEventArgs e)
        {

        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw.GeometryGraphic == null || draw.GeometryGraphic.Geometry == null)
                return;
        }

        private void draw_RedoStateChanged(object sender, EventArgs e)
        {
            if (CanRedoChanged != null)
                CanRedoChanged(this, new EventArgs());
        }

        private void draw_UndoStateChanged(object sender, EventArgs e)
        {
            if (CanUndoChanged != null)
                CanUndoChanged(this, new EventArgs());
        }

        #endregion

        #region Methods - Private

        public bool CanRedo
        {
            get { return draw.CanRedo; }
        }

        public bool CanUndo
        {
            get { return draw.CanUndo; }
        }

        /// <summary>
        /// 撤销当前的记录，返回上一步
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            bool val = draw.Undo();

            return val;
        }

        /// <summary>
        /// 重复上一步
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            bool val = draw.Redo();

            return val;
        }

        //绘制取消
        public bool CanCancel
        {
            get { return isDrawing; }
        }
        //绘制取消
        public bool Cancel()
        {
            bool val = isDrawing;
            draw.Cancel();
            return val;
        }

        public event EventHandler CanRedoChanged;

        public event EventHandler CanUndoChanged;

        public event EventHandler CanCancelChanged;

        public event EventHandler CanConfirmChanged;

        //回车确认绘制
        public bool CanConfirm
        {
            get { return isDrawing; }
        }

        //回车确认绘制
        public bool Confirm()
        {
            bool val = isDrawing;
            draw.Complete();
            return val;
        }

        #endregion

        #endregion
    }
}
