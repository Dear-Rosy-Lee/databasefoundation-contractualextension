/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 * 绘制线条来修角
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using DotSpatial.Tools;
using DotSpatial.Data;
using YuLinTu.Library.Business;
using System.IO;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    class MapControlActionQueryMapClipAngleByLine : MapControlAction, IUndoRedoable, ICancelable, IConfirmable
    {
        #region Properties

        #endregion

        #region Fields
        private GraphicsLayer layerLabel = new GraphicsLayer();
        List<LabelObject> objectLabelList = new List<LabelObject>();
        LabelObject objectLabel;
        Graphic centerPoint;
        List<YuLinTu.Spatial.Geometry> getGeoList = new List<Spatial.Geometry>();
        private string currentZoneCode = "";

        /// <summary>
        /// 空间参考系单位换算亩系数
        /// </summary>
        private double projectionUnit = 0.0015;

        /// <summary>
        /// 获取地图上选择到的要素集
        /// </summary>
        private List<YuLinTu.Spatial.Geometry> selectGeometryCollection = new List<Spatial.Geometry>();
        private List<ContractLand> selectContractLandCollection = new List<ContractLand>();
        private DrawPolyline draw = null;
        private bool isDrawing = false;
        private readonly SystemSetDefine SystemSet;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionQueryMapClipAngleByLine(MapControl map)
            : base(map)
        {
            SystemSet = SystemSetDefine.GetIntence();
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

            if (!MapControl.InternalLayers.Contains(layerLabel))
            {
                MapControl.InternalLayers.Add(layerLabel);
            }

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

            if (MapControl.InternalLayers.Contains(layerLabel))
                MapControl.InternalLayers.Remove(layerLabel);


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
            layerLabel.Graphics.Clear();
            MapControl.Action = null;
        }

        //根据绘制的线和选择的每一个面获取被裁剪的面集合
        private IFeatureSet GetClipPolygonSet(YuLinTu.Spatial.Geometry line, YuLinTu.Spatial.Geometry polygon)
        {
            var geosrid = polygon.Srid;
            var getDotDrawLinefeature = new Feature(WkbFeatureReader.ReadShape(new MemoryStream(line.AsBinary())));
            var getDotSelectPolygonfeature = new Feature(WkbFeatureReader.ReadShape(new MemoryStream(polygon.AsBinary())));

            IFeature getDotDrawLineFeature = getDotDrawLinefeature as IFeature;
            IFeature getDotSelectPolygonFeature = getDotSelectPolygonfeature as IFeature;

            FeatureSet dotFeatureset = new FeatureSet();
            IFeatureSet dotFeatureSet = dotFeatureset as IFeatureSet;

            try
            {
                var sucbool = ClipPolygonWithLine.Accurate_ClipPolygonWithLine(ref getDotSelectPolygonFeature, ref getDotDrawLineFeature, ref dotFeatureSet);
                if (sucbool)
                {
                    return dotFeatureSet;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                //TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                //messagebox.Message = warning.ToString();
                //messagebox.Header = "提示";

                //var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                //if (workpage == null)
                //    return null;
                //workpage.Page.ShowMessageBox(messagebox);
                return null;
            }

        }

        //根据地图上选择的地图要素，获取要素和对应的属性，一对一关系
        private void GetMapSelectItems(AccountLandBusiness landBus)
        {
            //获取被选取的面要素时，同时获取数据库面对象，图形和属性对应一致
            foreach (var item in MapControl.SelectedItems)
            {
                YuLinTu.Spatial.Geometry selectGeometry = new Spatial.Geometry();
                ContractLand selectLand = null;
                selectGeometry = item.Geometry;
                if (selectGeometry == null) return;
                if (item.Object != null)
                {
                    Guid landId = new Guid();
                    try
                    {
                        var landid = item.Object.Object.GetPropertyValue("ID");
                        if (landid == null) return;
                        if (item.Object.Object == null) return;
                        if (landid.ToString() == "") return;
                        landId = new Guid(landid.ToString());
                    }
                    catch (Exception info)
                    {
                        TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                        var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                        if (workpage == null) return;
                        messagebox.Message = "数据没有名称为‘ID’的字段，其字段类型为Guid" + info.Message;
                        messagebox.Header = "提示";
                        workpage.Page.ShowMessageBox(messagebox);
                        return;
                    }
                    if (landId != null)
                    {
                        selectLand = landBus.GetLandById(landId);
                        if (selectLand == null)
                        {
                            TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                            var workpage = MapControl.Properties["Workpage"] as IWorkpage;
                            if (workpage == null) return;
                            messagebox.Message = "没有在承包地图层中找到对应地块记录，请确认所选图形在承包地图层";
                            messagebox.Header = "提示";
                            workpage.Page.ShowMessageBox(messagebox);
                            return;
                        }
                        currentZoneCode = selectLand.ZoneCode;
                    }
                }
                selectContractLandCollection.Add(selectLand);
                selectGeometryCollection.Add(selectGeometry);
            }
        }

        private void draw_Begin(object sender, EditGeometryBeginEventArgs e)
        {
            TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
            var workpage = MapControl.Properties["Workpage"] as IWorkpage;
            if (workpage == null)
                return;
            if (MapControl.SelectedItems.Count == 0 || MapControl.SelectedItems == null)
            {
                messagebox.Message = "没有选择相应的面要素，请重新选择";
                messagebox.Header = "提示";

                workpage.Page.ShowMessageBox(messagebox);
                MapControl.Action = null;
                return;
            }

            if (MapControl.SelectedItems.Count > 1)
            {
                messagebox.Message = "只能对1个面要素进行分割操作，不能选择多个面要素，请重新选择";
                messagebox.Header = "提示";

                workpage.Page.ShowMessageBox(messagebox, (b, r) =>
                {
                    MapControl.SelectedItems.Clear();
                    MapControl.Action = null;
                });
                return;
            }
            if (MapControl.SelectedItems.Count == 1)
            {
                var db = DataBaseSource.GetDataBaseSource();
                AccountLandBusiness landbus = new AccountLandBusiness(db);
                GetMapSelectItems(landbus);
            }
            isDrawing = true;
            if (CanCancelChanged != null)
                CanCancelChanged(this, new EventArgs());
            if (CanConfirmChanged != null)
                CanConfirmChanged(this, new EventArgs());
        }

        private void draw_VertexAdded(object sender, EditGeometryVertexEventArgs e)
        {
            if (draw.GeometryGraphic == null || draw.GeometryGraphic.Geometry == null)
                return;
            if (selectGeometryCollection.Count == 0) return;
            YuLinTu.Spatial.Geometry drawGeoLine = draw.GeometryGraphic.Geometry;

            var points = drawGeoLine.ToPoints();
            if (points.Count() == 2)
            {
                layerLabel.Graphics.Clear();
                var db = DataBaseSource.GetDataBaseSource();
                AccountLandBusiness landbus = new AccountLandBusiness(db);

                if (drawGeoLine != null && selectGeometryCollection != null)
                {
                    //循环每一个被选取的面要素
                    for (int m = 0; m < selectGeometryCollection.ToArray().Length; m++)
                    {
                        IFeatureSet dotFeatureSet = GetClipPolygonSet(drawGeoLine, selectGeometryCollection[m]);
                        if (dotFeatureSet == null) return;
                        if (dotFeatureSet.Features.Count == 1 || dotFeatureSet.Features.Count == 0) return;
                        YuLinTu.Spatial.Geometry MaxAreaGeo = new Spatial.Geometry();
                        if (dotFeatureSet != null)
                        {
                            for (int i = 0; i < dotFeatureSet.Features.Count; i++)
                            {
                                IFeature feature = dotFeatureSet.Features[i];
                                //将获取裁剪好的转回
                                YuLinTu.Spatial.Geometry returnClipGeo = YuLinTu.Spatial.Geometry.FromBytes(feature.ToBinary(), selectGeometryCollection[m].Srid);

                                if (i == 0)
                                { MaxAreaGeo = returnClipGeo; }
                                if (returnClipGeo.Area() > MaxAreaGeo.Area())
                                {
                                    MaxAreaGeo = returnClipGeo;
                                }
                            }
                            //将第一个分割的赋给原始的，这样就不用裁剪了。                        
                            selectContractLandCollection[m].Shape = MaxAreaGeo;
                            selectContractLandCollection[m].ActualArea = ToolMath.RoundNumericFormat(MaxAreaGeo.Area() * projectionUnit, SystemSet.DecimalPlaces);
                            selectContractLandCollection[m].AwareArea = selectContractLandCollection[m].ActualArea;
                            selectContractLandCollection[m].TableArea = 0;
                            landbus.ModifyLand(selectContractLandCollection[m]);
                            selectGeometryCollection[m] = MaxAreaGeo;
                        }
                        MapControl.SelectedItems[m].Geometry = MaxAreaGeo;
                        MapControl.SelectedItems[m].Object.Geometry = MaxAreaGeo;
                    }
                }
                MapControl.Refresh();
                //获取处理完后最新的形状
                GetMapSelectItems(landbus);
                draw.Cancel();
                //draw.Activate();
            }
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw.GeometryGraphic == null || draw.GeometryGraphic.Geometry == null)
                return;
            objectLabelList.Clear();
            layerLabel.Graphics.Clear();
            getGeoList.Clear();

            //先获取绘制的线条
            YuLinTu.Spatial.Geometry drawGeoLine = draw.GeometryGraphic.Geometry;
            if (selectGeometryCollection != null)
            {
                //循环每一个被选取的面要素
                for (int m = 0; m < selectGeometryCollection.ToArray().Length; m++)
                {
                    IFeatureSet dotFeatureSet = GetClipPolygonSet(drawGeoLine, selectGeometryCollection[m]);

                    if (dotFeatureSet == null || dotFeatureSet.Features.Count < 2)
                    {
                        objectLabelList.Clear();
                        layerLabel.Graphics.Clear();
                        continue;
                    }
                    DisplayLabel(dotFeatureSet, m);
                }
            }
        }

        /// <summary>
        /// 显示面积
        /// </summary>
        private void DisplayLabel(IFeatureSet dotFeatureSet, int m)
        {
            foreach (var Feature in dotFeatureSet.Features)
            {
                IFeature feature = Feature;
                YuLinTu.Spatial.Geometry returnClipGeo = YuLinTu.Spatial.Geometry.FromBytes(feature.ToBinary(), selectGeometryCollection[m].Srid);
                getGeoList.Add(returnClipGeo);
            }

            int index = 0;
            while (index < getGeoList.Count)
            {
                YuLinTu.Spatial.Geometry geo = getGeoList[index];
                var geoarea = ToolMath.RoundNumericFormat(geo.Area() * projectionUnit, 5);
                for (int i = 0; i < getGeoList.Count; i++)
                {
                    if (index == i)
                    {
                        continue;
                    }
                    YuLinTu.Spatial.Geometry geo1 = getGeoList[i];//.Find(t => t.Within(geo));
                    var geoarea1 = ToolMath.RoundNumericFormat(geo1.Area() * projectionUnit, 5);
                    if (geo.Within(geo1))
                    {
                        getGeoList.Remove(geo);
                        break;
                    }
                    if (geoarea == geoarea1)
                    {
                        getGeoList.Remove(geo);
                        break;
                    }
                }
                index++;
            }
            foreach (var feature in getGeoList)
            {
                centerPoint = new Graphic() { Object = new FeatureObject() { Object = objectLabel = new LabelObject() } };
                centerPoint.Symbol = Application.Current.TryFindResource("UISymbol_Mark_MeasureArea_Label") as MarkSymbol;
                centerPoint.Geometry = null;
                layerLabel.Graphics.Add(centerPoint);
                objectLabelList.Add(objectLabel);
            }

            for (int i = 0; i < getGeoList.Count; i++)
            {
                var area = ToolMath.RoundNumericFormat(getGeoList[i].Area() * projectionUnit, SystemSet.DecimalPlaces);
                if (area != 0.00)
                {
                    var cds = getGeoList[i].Centroid().ToCoordinates();
                    if (double.IsNaN(cds[0].X) && double.IsNaN(cds[0].Y))
                    {
                        return;
                    }
                    else
                    {
                        layerLabel.Graphics[i].Geometry = getGeoList[i].Centroid();
                    }
                    objectLabelList[i].LabelText = string.Format("面积: {0:F2} {1}", area, "亩");
                    layerLabel.Graphics[i].Object.SetPropertyValue("Object", objectLabelList[i]);
                }
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
            get { return draw != null && draw.CanRedo; }
        }

        public bool CanUndo
        {
            get { return draw != null && draw.CanUndo; }
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
