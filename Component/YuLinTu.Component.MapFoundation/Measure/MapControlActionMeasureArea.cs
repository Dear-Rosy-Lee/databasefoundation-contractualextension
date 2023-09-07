/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 添加测量地图面积
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.MapFoundation
{
    public class MapControlActionMeasureArea : MapControlAction, IUndoRedoable, ICancelable, IConfirmable
    {
        #region Properties
        /// <summary>
        /// 用于动态显示测量结果图层
        /// </summary>
        private GraphicsLayer layerLabel = new GraphicsLayer();
        //用于存放历史绘制标注记录
        private List<Graphic> listOldGraphics = new List<Graphic>();
        /// <summary>
        /// 保存最后的绘制图形及结果，用于结果展示
        /// </summary>
        private GraphicsLayer layerEndDisplay = new GraphicsLayer();
        private Graphic graphicEndDisplay = null;

        /// <summary>
        /// 量算的文本
        /// </summary>
        private LabelObject objectLabel = null;
        /// <summary>
        /// 用于获取动态的中心点
        /// </summary>
        private Graphic centerPoint = null;
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
        private string projectionUnit = "Unkown";

        #endregion

        #region Fields
        private DrawPolygon draw = null;
        private bool isDrawing = false;
        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionMeasureArea(MapControl map)
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

            draw = new DrawPolygon(MapControl);
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

            if (MapControl.InternalLayers.Contains(layerLabel))
                MapControl.InternalLayers.Remove(layerLabel);
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
                return;
            }
        }

        /// <summary>
        /// 清除按钮，清除所有绘制
        /// </summary>       
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            draw.Cancel();
            if (layerLabel != null)
            {
                layerLabel.Graphics.Clear();
            }
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
            //清空以前的动态标注
            layerLabel.Graphics.Clear();
            listOldGraphics.Clear();

            isDrawing = false;
            if (CanCancelChanged != null)
                CanCancelChanged(this, new EventArgs());
            if (CanConfirmChanged != null)
                CanConfirmChanged(this, new EventArgs());

            //先保留显示绘制的要素
            if (e.Geometry != null)
            {
                graphicEndDisplay = new Graphic();
                graphicEndDisplay.Geometry = e.Geometry;
                graphicEndDisplay.Symbol = Application.Current.TryFindResource("UISymbol_Fill_Measure") as UISymbol;
                layerEndDisplay.Graphics.Add(graphicEndDisplay);

                if (centerPoint != null)
                {
                    layerEndDisplay.Graphics.Add(centerPoint);
                }
            }
        }

        private void draw_Begin(object sender, EditGeometryBeginEventArgs e)
        {
            isDrawing = true;
            if (CanCancelChanged != null)
                CanCancelChanged(this, new EventArgs());
            if (CanConfirmChanged != null)
                CanConfirmChanged(this, new EventArgs());

            centerPoint = new Graphic() { Object = new FeatureObject() { Object = objectLabel = new LabelObject() } };
            centerPoint.Symbol = Application.Current.TryFindResource("UISymbol_Mark_MeasureArea_Label") as MarkSymbol;

            layerLabel.Graphics.Add(centerPoint);
            listOldGraphics.Add(centerPoint);

            if (!MapControl.InternalLayers.Contains(layerLabel))
                MapControl.InternalLayers.Add(layerLabel);
        }

        private void draw_VertexAdded(object sender, EditGeometryVertexEventArgs e)
        {

        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw.GeometryGraphic == null || draw.GeometryGraphic.Geometry == null)
                return;

            var area = ToolMath.SetNumericFormat(draw.GeometryGraphic.Geometry.Area(), 2, 0);
            objectLabel.LabelText = string.Format("面积:{0} {1}/{2} 亩", area, projectionUnit, ToolMath.SetNumericFormat(area * 0.0015, 2, 0));

            //如果获取的中点为null，则将绘制的第一个点赋值过来
            if (RefreshCenterGeometry())
            {
                return;
            }
            else
            {
                return;
            }

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

            if (draw.GeometryGraphic == null || draw.GeometryGraphic.Geometry == null)
            {
                layerLabel.Graphics.Clear();
                return val;
            }
            var area = draw.GeometryGraphic.Geometry.Area();
            //objectLabel.LabelText = string.Format("总面积: {0:F3} {1}", area, projectionUnit);
            objectLabel.LabelText = string.Format("面积:{0:F2} {1}/{2:F2} 亩", area, projectionUnit, area * 0.0015);

            if (RefreshCenterGeometry())
            {
                return val;
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// 重复上一步
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            bool val = draw.Redo();

            if (listOldGraphics.Count > 0 && layerLabel.Graphics.Count == 0)
            {
                layerLabel.Graphics.Insert(0, listOldGraphics[listOldGraphics.Count - 1]);
                listOldGraphics.RemoveAt(listOldGraphics.Count - 1);
            }
            else
            {
                var area = draw.GeometryGraphic.Geometry.Area();
                objectLabel.LabelText = string.Format("面积:{0:F2} {1}/{2:F2} 亩", area, projectionUnit, area * 0.0015);

                var refreshCenterGeometry = RefreshCenterGeometry();
                if (refreshCenterGeometry)
                {
                    return val;
                }
                return val;
            }
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

        /// <summary>
        /// 动态更新中心点位置及面积，只有更新到中心点才返回true
        /// </summary>
        /// <returns></returns> 
        private bool RefreshCenterGeometry()
        {
            //如果获取的中点为null，则将绘制的第一个点赋值过来
            if (layerLabel.Graphics.Count > 0 && draw.GeometryGraphic.Geometry != null)
            {
                layerLabel.Graphics[layerLabel.Graphics.Count - 1].Geometry = draw.GeometryGraphic.Geometry.Centroid();
                var cds = layerLabel.Graphics[layerLabel.Graphics.Count - 1].Geometry.ToCoordinates();
                if (double.IsNaN(cds[0].X) && double.IsNaN(cds[0].Y))
                {
                    var drawPoints = draw.GeometryGraphic.Geometry.ToPoints();
                    layerLabel.Graphics[layerLabel.Graphics.Count - 1].Geometry = drawPoints[0];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #endregion
    }
}
