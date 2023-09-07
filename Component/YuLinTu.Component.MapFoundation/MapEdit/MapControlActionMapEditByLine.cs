/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 绘制线条来修边
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
    class MapControlActionMapEditByLine : MapControlAction, IUndoRedoable, ICancelable, IConfirmable
    {
        #region Properties

        private GraphicsLayer layerLabel = new GraphicsLayer();
        private LabelObject objectLabel = new LabelObject();
        //用于存放历史绘制标注记录
        private List<Graphic> listOldGraphics = new List<Graphic>();
        /// <summary>
        /// 保存最后的绘制图形，用于结果展示
        /// </summary>
        private GraphicsLayer layerEndDisplay = new GraphicsLayer();

        /// <summary>
        /// 获取父控件
        /// </summary>
        private Grid container = null;
        /// <summary>
        /// 清除按钮
        /// </summary>
        private MetroButton clearGraphicBtn = null;
        private Border border = null;
        /// <summary>
        /// 空间参考系单位换算亩系数
        /// </summary>
        private double projectionUnit = 0.0015;

        /// <summary>
        /// 获取地图上选择到的要素集
        /// </summary>
        private List<YuLinTu.Spatial.Geometry> selectGeometryCollection = new List<Spatial.Geometry>();
        private List<ContractLand> selectContractLandCollection = new List<ContractLand>();
        #endregion

        #region Fields

        private DrawPolyline draw = null;
        private bool isDrawing = false;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionMapEditByLine(MapControl map)
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

            draw = new DrawPolyline(MapControl);
            draw.Begin += draw_Begin;
            draw.End += draw_End;
            draw.VertexAdded += draw_VertexAdded;
            draw.CanUndoChanged += draw_UndoStateChanged;
            draw.CanRedoChanged += draw_RedoStateChanged;
            draw.MarkSymbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as MarkSymbol;
            draw.LineSymbol = Application.Current.TryFindResource("UISymbol_Line_Measure") as LineSymbol;
            draw.FillSymbol = Application.Current.TryFindResource("UISymbol_Fill_Measure") as FillSymbol;
            draw.TrackingMouseMove = MapControl.TrackingMouseMoveWhenDraw;
            //获取控件参考单位
            MapControl.SpatialReferenceChanged += MapControl_SpatialReferenceChanged;
            this.RefreshMapControlSpatialUnit();
            draw.Activate();
            MapControl.SnapModeChanged += MapControl_SnapModeChanged;
            MapControl.MouseMove += MapControl_MouseMove;

            MapControl.InternalLayers.Add(layerEndDisplay);


            //MapControl.SpatialReferenceChanged += MapControl_SpatialReferenceChanged;
            //this.RefreshMapControlSpatialUnit();
            //获取父控件，并且定义一个消除按钮
            container = MapControl.GetParent<Grid>();
            border = new Border();
            border.BorderThickness = new Thickness(1);
            border.Margin = new Thickness(10, 10, 20, 10);
            border.Background = Application.Current.TryFindResource("Metro_Window_Style_Background_Content") as Brush;
            border.BorderBrush = Application.Current.TryFindResource("Metro_Window_Style_BorderBrush_Default") as Brush;
            border.HorizontalAlignment = HorizontalAlignment.Right;
            border.VerticalAlignment = VerticalAlignment.Top;

            clearGraphicBtn = new MetroButton();
            clearGraphicBtn.ToolTip = "清空绘制结果";
            clearGraphicBtn.Click += btn_Click;
            clearGraphicBtn.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/InkDeleteAllInk.png")) };

            border.Child = clearGraphicBtn;
            container.Children.Add(border);
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

            MapControl.SpatialReferenceChanged -= MapControl_SpatialReferenceChanged;

            clearGraphicBtn.Click -= btn_Click;
            if (container.Children.Contains(clearGraphicBtn))
            {
                container.Children.Remove(clearGraphicBtn);
            }
            if (container.Children.Contains(border))
            {
                container.Children.Remove(border);
            }
            clearGraphicBtn = null;
            container = null;
            border = null;
        }

        #endregion

        #region Methods - Events

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
                }
            }
            catch
            {
                projectionUnit = 0.0015;
                return;
            }
        }

        private void MapControl_SnapModeChanged(object sender, EventArgs e)
        {
            if (draw == null)
                return;

            draw.SnapMode = MapControl.SnapMode;
        }

        private void draw_End(object sender, EditGeometryEndEventArgs e)
        {

            var db = DataBaseSource.GetDataBaseSource();
            AccountLandBusiness landbus = new AccountLandBusiness(db);
            ContractLand selectLand = null;

            if (MapControl.SelectedItems[0].Object != null)
            {
                var landid = MapControl.SelectedItems[0].Object.Object.GetPropertyValue("ID");
                if (landid.ToString() == "") return;
                Guid landId = new Guid(landid.ToString());
                if (landid != null)
                {
                    selectLand = landbus.GetLandById(landId);
                }
                if (selectLand == null) return;
            }

            YuLinTu.Spatial.Geometry drawGeoLine = e.Geometry;
            if (drawGeoLine == null) return;
            YuLinTu.Spatial.Geometry selectGeometry = new Spatial.Geometry();
            selectGeometry = MapControl.SelectedItems[0].Geometry;

            //设置待处理的要素
            YuLinTu.Spatial.Geometry resGeometry = new Spatial.Geometry();
            resGeometry = MapControl.SelectedItems[0].Geometry;

            int layerSrid = selectGeometry.Srid;
            //var get = selectGeometry.Intersection(drawGeoLine);
            //var get = drawGeoLine.Intersection(selectGeometry);

            //var get = selectGeometry.Difference(drawGeoLine);
            //var get = drawGeoLine.Difference(selectGeometry);可行

            var selectGeometryLinelist0 = selectGeometry.ToSegmentLines();
            var drawLineItemlist0 = drawGeoLine.ToSegmentLines();

            //待处理的要素总线组
            List<YuLinTu.Spatial.Geometry> targetGeometry = new List<YuLinTu.Spatial.Geometry>();
            //待处理的要素面线组
            List<YuLinTu.Spatial.Geometry> targetDrawLineGeometry = new List<YuLinTu.Spatial.Geometry>();
            //待处理的要素线线组
            List<YuLinTu.Spatial.Geometry> targetSelectLineGeometry = new List<YuLinTu.Spatial.Geometry>();

            //以两个交点之间为一组进行获取,第一个点为0，第二个点为1
            int targetFlag = 1;
            //开始有交点的flag
            bool flag = false;

            for (int i = 0; i < selectGeometryLinelist0.Length; i++)
            {

                for (int m = 0; m < drawLineItemlist0.Length; m++)
                {

                    if (drawLineItemlist0[m].Intersects(selectGeometryLinelist0[i]))
                    {
                        flag = true;
                        var intersectsPoint = drawLineItemlist0[m].Intersection(selectGeometryLinelist0[i]);
                        var intersectsCdts = intersectsPoint.ToCoordinates();

                        var drawLineItemcdts = drawLineItemlist0[m].ToCoordinates();
                        var selectGeometryLineItemcdts = selectGeometryLinelist0[i].ToCoordinates();

                        if (targetFlag % 2 != 0)
                        {
                            //先添加面的交线到集合
                            List<Coordinate> selectGeometryLineToTargetcdts = new List<Coordinate>();
                            //selectGeometryLineToTargetcdts.Add(selectGeometryLineItemcdts[1]);
                            //selectGeometryLineToTargetcdts.Add(intersectsCdts[0]);

                            //var selectGeometryLineToTarget = YuLinTu.Spatial.Geometry.CreatePolyline(selectGeometryLineToTargetcdts, layerSrid);
                            //targetSelectLineGeometry.Add(selectGeometryLineToTarget);

                            //再添加线的交线到集合
                            if (m == 0)
                            {
                                List<Coordinate> drawLineItemToTargetcdts = new List<Coordinate>();
                                drawLineItemToTargetcdts.Add(intersectsCdts[0]);
                                drawLineItemToTargetcdts.Add(drawLineItemcdts[1]);

                                var drawLineItemToTarget = YuLinTu.Spatial.Geometry.CreatePolyline(drawLineItemToTargetcdts, layerSrid);
                                targetDrawLineGeometry.Add(drawLineItemToTarget);
                            }
                            else
                            {
                                List<Coordinate> drawLineItemToTargetcdts = new List<Coordinate>();
                                drawLineItemToTargetcdts.Add(drawLineItemcdts[0]);
                                drawLineItemToTargetcdts.Add(intersectsCdts[0]);

                                var drawLineItemToTarget = YuLinTu.Spatial.Geometry.CreatePolyline(drawLineItemToTargetcdts, layerSrid);
                                targetDrawLineGeometry.Add(drawLineItemToTarget);
                            }

                        }
                        else
                        {
                            flag = false;
                            //先添加面的交线到集合
                            //List<Coordinate> selectGeometryLineToTargetcdts = new List<Coordinate>();
                            //selectGeometryLineToTargetcdts.Add(selectGeometryLineItemcdts[1]);
                            //selectGeometryLineToTargetcdts.Add(intersectsCdts[0]);


                            //var selectGeometryLineToTarget = YuLinTu.Spatial.Geometry.CreatePolyline(selectGeometryLineToTargetcdts, layerSrid);
                            //targetSelectLineGeometry.Add(selectGeometryLineToTarget);

                            //再添加线的交线到集合
                            List<Coordinate> drawLineItemToTargetcdts = new List<Coordinate>();
                            drawLineItemToTargetcdts.Add(drawLineItemcdts[0]);
                            drawLineItemToTargetcdts.Add(intersectsCdts[0]);


                            var drawLineItemToTarget = YuLinTu.Spatial.Geometry.CreatePolyline(drawLineItemToTargetcdts, layerSrid);
                            targetDrawLineGeometry.Add(drawLineItemToTarget);

                            foreach (var item0 in targetSelectLineGeometry)
                            {
                                targetGeometry.Add(item0);
                            }
                            foreach (var item1 in targetDrawLineGeometry)
                            {
                                targetGeometry.Insert(0, item1);
                            }

                            List<Coordinate> targetcdts = new List<Coordinate>();
                            foreach (var item in targetGeometry)
                            {
                                if (item.GeometryType == eGeometryType.Polyline)
                                {
                                    var PolylineCds = item.ToCoordinates();
                                    targetcdts.Add(PolylineCds[0]);
                                    targetcdts.Add(PolylineCds[1]);
                                }
                            }
                            var targetply = YuLinTu.Spatial.Geometry.CreatePolygon(targetcdts, layerSrid);

                            //resGeometry = targetply;
                            resGeometry = MapGraphicEdit(resGeometry, targetply);
                        }

                        targetFlag++;

                    }
                    else
                    {
                        if (targetFlag % 2 != 0 && flag)
                        {
                            targetDrawLineGeometry.Add(drawLineItemlist0[m]);
                        }
                    }

                }

                //if (flag) 
                //{
                //    targetSelectLineGeometry.Add(selectGeometryLinelist0[i]);
                //}


            }

            selectLand.Shape = resGeometry;
            selectLand.ActualArea = ToolMath.CutNumericFormat(resGeometry.Area() * projectionUnit, 2);
            landbus.ModifyLand(selectLand);

            MapControl.SelectedItems[0].Object.Geometry = resGeometry;

            MapControl.Refresh();
        }

        /// <summary>
        /// 将已有的图形和获取的每一个目标图形进行处理
        /// </summary>
        /// <param name="selectGeometry">被选取的原始图形</param>
        /// <param name="targetGeometry">目标图形</param>       
        private YuLinTu.Spatial.Geometry MapGraphicEdit(YuLinTu.Spatial.Geometry selectGeometry, YuLinTu.Spatial.Geometry targetGeometry)
        {
            YuLinTu.Spatial.Geometry resultGeometry = new Spatial.Geometry();
            if (selectGeometry.Within(targetGeometry))
            {
                resultGeometry = selectGeometry.Difference(targetGeometry);
                return resultGeometry;
            }
            else
            {
                resultGeometry = selectGeometry.Union(targetGeometry);
                return resultGeometry;
            }

        }

        private void draw_Begin(object sender, EditGeometryBeginEventArgs e)
        {
            selectGeometryCollection.Clear();
            selectContractLandCollection.Clear();

            TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
            var workpage = MapControl.Properties["Workpage"] as IWorkpage;
            if (workpage == null)
                return;

            if (MapControl.SelectedItems.Count == 0 || MapControl.SelectedItems == null)
            {
                messagebox.Message = "没有选择相应的面要素，请重新选择";
                messagebox.Header = "提示";
                messagebox.MessageGrade = eMessageGrade.Error;
                messagebox.CancelButtonText = "取消";
                workpage.Page.ShowMessageBox(messagebox);
                return;
            }
            if (MapControl.SelectedItems.Count > 1)
            {
                messagebox.Message = "不能超过1个修边目标，请重新选择";
                messagebox.Header = "提示";

                workpage.Page.ShowMessageBox(messagebox);
                return;
            }

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
    }
    #endregion

    #endregion
}
