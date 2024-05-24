/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 地图地块图元裁剪-按面积及份数自动裁剪后获取
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
//using DotSpatial.Tools;
//using DotSpatial.Topology;
//using DotSpatial.Data;
using YuLinTu.Library.Business;
using System.IO;
using YuLinTu.Library.Entity;



namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 按照设置的面积及示意线分割现有地块，获取当前的分割面积集合
    /// </summary>
    public class MapControlActionQueryMapClipBySetting : MapControlAction, IUndoRedoable, ICancelable, IConfirmable
    {
        #region Properties

        /// <summary>
        /// 按何种类型裁剪
        /// </summary> 
        private eImportLandClipType eimportlandcliptype { set; get; }

        /// <summary>
        /// 裁剪的个数
        /// </summary>
        private int StepCount { set; get; }

        /// <summary>
        /// 目标面积集合,计算过比例的
        /// </summary>
        private List<double> targetAreaList = new List<double>();

        #endregion

        #region Fields
        private GraphicsLayer layerLabel = new GraphicsLayer() { Name = "areaLableGLayer" };

        LabelObject objectLabel;

        private string currentZoneCode = "";
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
        /// 线段平移的步长，主要用来判断移动的方向
        /// </summary>
        private double step = 0.0001;//平方米

        /// <summary>
        /// 构造线偏移方向，addDirection朝对边最远点方向增加，reduceDirection朝对边最远点方向远离
        /// </summary>
        private string stepFlag = "addDirection";//偏移方向
        //在对应要素上最远离示意线的点
        private Geometry leftFarPoint = new Geometry();
        private Geometry rightFarPoint = new Geometry();
        private double verticalDistence = 0.0;//最远两点垂直距离
        /// <summary>
        /// 获取地图上选择到的要素集
        /// </summary>
        private List<YuLinTu.Spatial.Geometry> selectGeometryCollection = new List<Spatial.Geometry>();
        private List<ContractLand> selectContractLandCollection = new List<ContractLand>();
        private DrawPolyline draw = null;
        private bool isDrawing = false;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionQueryMapClipBySetting(MapControl map)
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
            if (MapControl == null) return;
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

            MapControl.SpatialReferenceChanged += MapControl_SpatialReferenceChanged;
            this.RefreshMapControlSpatialUnit();
            draw.Activate();
            MapControl.SnapModeChanged += MapControl_SnapModeChanged;


            if (!MapControl.InternalLayers.Any(m => m.Name == "areaLableGLayer"))
            {
                MapControl.InternalLayers.Add(layerLabel);
            }

            //获取父控件，并且定义一个消除按钮
            container = MapControl.GetParent<Grid>();
            border = new Border();
            border.BorderThickness = new Thickness(1);
            border.Margin = new Thickness(10, 10, 20, 10);
            border.Background = Application.Current.TryFindResource("Metro_Window_Style_Background_Content") as System.Windows.Media.Brush;
            border.BorderBrush = Application.Current.TryFindResource("Metro_Window_Style_BorderBrush_Default") as System.Windows.Media.Brush;
            border.HorizontalAlignment = HorizontalAlignment.Right;
            border.VerticalAlignment = VerticalAlignment.Top;

            clearGraphicBtn = new MetroButton();
            clearGraphicBtn.ToolTip = "清空绘制结果";
            clearGraphicBtn.Click += btn_Click;
            clearGraphicBtn.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/InkDeleteAllInk.png")) };

            border.Child = clearGraphicBtn;
            container.Children.Add(border);

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
                MapControl.Action = null;
                return;
            }
            if (MapControl.SelectedItems.Count > 1)
            {
                messagebox.Message = "只能对1个面要素进行分割操作，不能选择多个面要素，请重新选择";
                messagebox.Header = "提示";

                workpage.Page.ShowMessageBox(messagebox, (b, r) =>
                {
                    if (MapControl == null || MapControl.SelectedItems == null) return;
                    MapControl.SelectedItems.Clear();
                });
                MapControl.Action = null;
                return;
            }

            selectGeometryCollection.Clear();
            selectContractLandCollection.Clear();

            var db = DataBaseSource.GetDataBaseSource();
            AccountLandBusiness landbus = new AccountLandBusiness(db);
            GetMapSelectItems(landbus);
            if (selectGeometryCollection.Count() == 0) return;
            ClipPolygonSetting clipcontrol = new ClipPolygonSetting(selectGeometryCollection[0].Area());
            clipcontrol.Workpage = workpage;
            workpage.Page.ShowMessageBox(clipcontrol, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                eimportlandcliptype = clipcontrol.eimportlandcliptype;
                StepCount = clipcontrol.StepCount;
                targetAreaList = clipcontrol.TargetAreaList;
            });

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

            draw.Deactivate();
            draw.Begin -= draw_Begin;
            draw.End -= draw_End;
            draw.VertexAdded -= draw_VertexAdded;

            draw.CanUndoChanged -= draw_UndoStateChanged;
            draw.CanRedoChanged -= draw_RedoStateChanged;

            draw = null;

            if (MapControl.InternalLayers.Contains(layerLabel))
                MapControl.InternalLayers.Remove(layerLabel);

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
            targetAreaList = null;
        }

        #endregion

        #region Methods - Events

        /// <summary>
        /// 清除按钮，清除所有绘制
        /// </summary>       
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //draw.Cancel();
            if (layerLabel != null)
            {
                layerLabel.Graphics.Clear();
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
            //layerLabel.Graphics.Clear();
            //draw.Cancel();
        }

        /// <summary>
        /// 根据绘制的线和选择的每一个面获取被裁剪的面集合
        /// </summary>
        /// <param name="line">裁剪线</param>
        /// <param name="polygon">目标多边形</param>
        /// <returns>裁剪结果多边形列表</returns>
        private List<YuLinTu.Spatial.Geometry> GetClipPolygonSet(YuLinTu.Spatial.Geometry line, YuLinTu.Spatial.Geometry polygon)
        {
            try
            {
                var geosrid = polygon.Srid;
                var getDotDrawLinefeature = new DotSpatial.Data.Feature(DotSpatial.Data.WkbFeatureReader.ReadShape(new MemoryStream(line.AsBinary())));
                var getDotSelectPolygonfeature = new DotSpatial.Data.Feature(DotSpatial.Data.WkbFeatureReader.ReadShape(new MemoryStream(polygon.AsBinary())));

                DotSpatial.Data.IFeature getDotDrawLineFeature = getDotDrawLinefeature as DotSpatial.Data.IFeature;
                DotSpatial.Data.IFeature getDotSelectPolygonFeature = getDotSelectPolygonfeature as DotSpatial.Data.IFeature;

                DotSpatial.Data.FeatureSet dotFeatureset = new DotSpatial.Data.FeatureSet();
                DotSpatial.Data.IFeatureSet dotFeatureSet = dotFeatureset as DotSpatial.Data.IFeatureSet;

                List<YuLinTu.Spatial.Geometry> clipGeometryList = new List<Spatial.Geometry>();

                //var sucbool = DotSpatial.Tools.ClipPolygonWithLine.Accurate_ClipPolygonWithLine(ref getDotSelectPolygonFeature, ref getDotDrawLineFeature, ref dotFeatureSet);
                //var sucbool = DotSpatial.Tools.ClipPolygonWithLine.Fast_ClipPolygonWithLine(ref getDotSelectPolygonFeature, ref getDotDrawLineFeature, ref dotFeatureSet);                              
                var sucbool = DotSpatial.Tools.ClipPolygonWithLine.DoClipPolygonWithLine(ref getDotSelectPolygonFeature, ref getDotDrawLineFeature, ref dotFeatureSet);

                if (sucbool)
                {
                    YuLinTu.Spatial.Geometry returnClipGeo = null;
                    for (int i = 0; i < dotFeatureSet.Features.Count; i++)
                    {
                        DotSpatial.Data.IFeature feature = dotFeatureSet.Features[i];
                        //将获取裁剪好的转回
                        returnClipGeo = new Spatial.Geometry();
                        returnClipGeo = YuLinTu.Spatial.Geometry.FromBytes(feature.ToBinary(), selectGeometryCollection[0].Srid);
                        clipGeometryList.Add(returnClipGeo);
                    }
                    return clipGeometryList;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
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
            if (selectGeometryCollection.Count == 0) return;
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

            if (!drawGeoLine.Intersects(selectGeometryCollection[0])) return;
            if (!drawGeoLine.Crosses(selectGeometryCollection[0])) return;

            if (drawGeoLine == null) return;
            if (drawGeoLine.IsSelfIntersects()) return;
            var points = drawGeoLine.ToPoints();
            if (points.Count() == 2)
            {
                var db = DataBaseSource.GetDataBaseSource();
                AccountLandBusiness landbus = new AccountLandBusiness(db);
                if (selectGeometryCollection.Count == 0) return;
                if (drawGeoLine != null && selectGeometryCollection != null)
                {
                    YuLinTu.Spatial.Geometry clipGeometry = selectGeometryCollection[0];

                    //获得与示意线图元上垂直的距离
                    verticalDistence = GetVerticalDistence(clipGeometry, drawGeoLine);

                    //首先获取最开始的平行线，从离示意线一边最远的点开始的平行线-获取从那边的首发平行线
                    YuLinTu.Spatial.Geometry transtationStartLine = getTranstationStartLine(clipGeometry, drawGeoLine);

                    //判定移动裁剪方向
                    stepFlag = GetStepFlag(clipGeometry, transtationStartLine);

                    //开始裁剪，获取最终裁剪得到的要素集合
                    List<YuLinTu.Spatial.Geometry> clipedGeometryList = StartClipBySet(clipGeometry, transtationStartLine, drawGeoLine);

                    if (clipedGeometryList == null || clipedGeometryList.Count == 0) return;
                    if (clipedGeometryList.Count == 1 && clipedGeometryList[0].Instance == null) return;

                    //更新显示面积-不显示剩余的，只显示已有的                   
                    DisplayLabel(clipedGeometryList);
                    //循环每一个被选取的面要素
                    for (int i = 0; i < clipedGeometryList.Count - 1; i++)
                    {
                        ContractLand clipLanditem = selectContractLandCollection[0].Clone() as ContractLand;
                        clipLanditem.ID = Guid.NewGuid();
                        clipLanditem.Shape = clipedGeometryList[i];
                        clipLanditem.ActualArea = Math.Round(clipedGeometryList[i].Area() * projectionUnit, 2);
                        clipLanditem.AwareArea = clipLanditem.ActualArea;
                        string number = landbus.GetNewLandNumber(currentZoneCode);
                        clipLanditem.LandNumber = number;
                        string surverNumber = clipLanditem.LandNumber.Length >= 5 ? clipLanditem.LandNumber.Substring(clipLanditem.LandNumber.Length - 5) : clipLanditem.LandNumber.PadLeft(5, '0');
                        clipLanditem.SurveyNumber = surverNumber;
                        clipLanditem.TableArea = 0;
                        //if (eimportlandcliptype == eImportLandClipType.ClipByText || eimportlandcliptype == eImportLandClipType.ClipByTextAndProportion) clipLanditem.OwnerName = targetPersonNameList[i];
                        landbus.AddLand(clipLanditem);
                    }
                    selectContractLandCollection[0].Shape = clipedGeometryList[clipedGeometryList.Count - 1];
                    selectContractLandCollection[0].ActualArea = Math.Round(clipedGeometryList[clipedGeometryList.Count - 1].Area() * projectionUnit, 2);
                    selectContractLandCollection[0].AwareArea = selectContractLandCollection[0].ActualArea;
                    selectContractLandCollection[0].TableArea = 0;
                    landbus.ModifyLand(selectContractLandCollection[0]);
                }
                MapControl.SelectedItems.Clear();
                MapControl.Refresh();
                selectGeometryCollection.Clear();
                selectContractLandCollection.Clear();
                draw.Complete();
                return;
            }
        }

        #region "按设置分割处理方法"

        /// <summary>
        /// 开始进行按照设置裁剪
        /// </summary>      
        private List<YuLinTu.Spatial.Geometry> StartClipBySet(Geometry clipGeometry, Geometry transtationStartLine, Geometry drawGeoLine)
        {
            #region 测试的方法
            ////裁剪的次数
            //int stepCount = (int)(verticalDistence / step);

            ////获取的过程剪切集合
            //List<YuLinTu.Spatial.Geometry> clipedGeometryList = new List<Geometry>();
            ////最终目标集合
            //List<YuLinTu.Spatial.Geometry> resClipedGeometryList = new List<Geometry>();

            //int targetHasStepCount = 0;//当前地块所占步长次数1
            //string flag = "0";
            ////获取根据步长移动的后的第二条平行线
            //YuLinTu.Spatial.Geometry transtationClipLine = getTranstationClipLine(clipGeometry, transtationStartLine);
            //clipedGeometryList = GetClipPolygonSet(transtationClipLine, clipGeometry);
            //if (clipedGeometryList == null || clipedGeometryList.Count == 0) return null; 
            //if (clipedGeometryList[0].Area() < clipedGeometryList[1].Area())
            //{
            //    flag = "0";
            //}
            //else 
            //{
            //    flag = "1";            
            //}

            ////获取第一组
            //for (int i = 0; i < stepCount; i++)
            //{
            //    //获取根据步长移动的后的平行线-第二根平行线                
            //    if (i != 0)
            //    {
            //        transtationClipLine = getTranstationClipLine(clipGeometry, transtationClipLine);                   
            //    }
            //    //获取每步裁剪后的多边形集合
            //    clipedGeometryList = GetClipPolygonSet(transtationClipLine, clipGeometry);
            //    if (clipedGeometryList == null || clipedGeometryList.Count == 0) 
            //    {                   
            //        break;                  
            //    }
            //    if (flag == "0")
            //    {                
            //        //面积误差
            //        double areaDeviation = targetAreaList[0] - clipedGeometryList[0].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[0]);
            //            //裁剪完后将另外个取出
            //            clipGeometry = clipedGeometryList[1];

            //            break;
            //        }
            //    }
            //    else if (flag == "1")
            //    {                  
            //        //面积误差                  
            //       double areaDeviation = targetAreaList[0] - clipedGeometryList[1].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[1]);
            //            clipGeometry = clipedGeometryList[0];

            //            break;
            //        }
            //    }
            //    targetHasStepCount++;                
            //}          
            ////if (resClipedGeometryList.Count == 0) return null;
            //stepCount = stepCount - targetHasStepCount;
            ////获取第二组
            //for (int i = 0; i < stepCount; i++)
            //{
            //    //获取根据步长移动的后的平行线
            //    transtationClipLine = getTranstationClipLine(clipGeometry, transtationClipLine);

            //    //获取每步裁剪后的多边形集合
            //    clipedGeometryList = GetClipPolygonSet(transtationClipLine, clipGeometry);
            //    if (clipedGeometryList == null || clipedGeometryList.Count == 0)
            //    {         
            //        return null;
            //    }
            //    if (flag == "0")
            //    {
            //        //面积误差
            //        double areaDeviation = targetAreaList[1] - clipedGeometryList[0].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[0]);
            //            clipGeometry = clipedGeometryList[1];
            //            break;
            //        }
            //    }
            //    else if (flag == "1")
            //    {
            //        //面积误差
            //        double areaDeviation = targetAreaList[1] - clipedGeometryList[1].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[1]);
            //            clipGeometry = clipedGeometryList[0];
            //            break;
            //        }
            //    }
            //    targetHasStepCount++;
            //}

            //stepCount = stepCount - targetHasStepCount;
            //////获取第三组
            //for (int i = 0; i < stepCount; i++)
            //{
            //    //获取根据步长移动的后的平行线
            //    transtationClipLine = getTranstationClipLine(clipGeometry, transtationClipLine);

            //    //获取每步裁剪后的多边形集合
            //    clipedGeometryList = GetClipPolygonSet(transtationClipLine, clipGeometry);
            //    if (clipedGeometryList == null || clipedGeometryList.Count == 0)
            //    {                   
            //        break;

            //    }
            //    if (flag == "0")
            //    {
            //        //面积误差
            //        double areaDeviation = targetAreaList[2] - clipedGeometryList[0].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[0]);
            //            //裁剪完后将另外个取出
            //            clipGeometry = clipedGeometryList[1];
            //            break;
            //        }
            //    }
            //    else if (flag == "1")
            //    {
            //        //面积误差
            //        double areaDeviation = targetAreaList[2] - clipedGeometryList[1].Area();
            //        if (areaDeviation <= 0.01 && areaDeviation >= 0)
            //        {
            //            resClipedGeometryList.Add(clipedGeometryList[1]);
            //            clipGeometry = clipedGeometryList[0];
            //            break;
            //        }
            //    }
            //    targetHasStepCount++;
            //}         

            //resClipedGeometryList.Add(clipGeometry);
            //return resClipedGeometryList;
            #endregion

            //获取的过程剪切集合
            List<YuLinTu.Spatial.Geometry> clipedGeometryList = new List<Geometry>();
            //最终目标集合
            List<YuLinTu.Spatial.Geometry> resClipedGeometryList = new List<Geometry>();
            //判定从获取的多边形那边开始裁剪
            string flag = "0";

            //获取根据步长移动的后的第二条平行线
            YuLinTu.Spatial.Geometry transtationClipLine = getTranstationClipLine(clipGeometry, transtationStartLine, step);
            clipedGeometryList = GetClipPolygonSet(transtationClipLine, clipGeometry);
            if (clipedGeometryList == null || clipedGeometryList.Count == 0) return null;
            if (clipedGeometryList[0].Area() < clipedGeometryList[1].Area())
            {
                flag = "0";
            }
            else
            {
                flag = "1";
            }

            Geometry userTargetGeometry = null;//获取的符合用户要求的要素
            //裁剪后剩余的要素
            Geometry otherGeometry = new Geometry();
            //裁剪完成每个满足要求的图形后的新的起始平行线
            Geometry transtationStartLineNew = new Geometry();
            //面积误差值
            double areaDeviation = 0.0;

            //处理二分法用的距离
            double stepLenthUseMethod = verticalDistence / 2;
            //每次逼近的步长
            double stepcount = stepLenthUseMethod;

            //选择裁剪方式
            switch (eimportlandcliptype)
            {
                //等宗裁剪
                case eImportLandClipType.ClipByDistence:
                    double clipLenth = verticalDistence / StepCount;
                    for (int i = 0; i < StepCount - 1; i++)
                    {
                        try
                        {
                            userTargetGeometry = GetTargetGeometry(ref otherGeometry, clipGeometry, transtationStartLine, ref transtationStartLineNew, clipLenth, flag);
                            if (userTargetGeometry != null)
                            {
                                resClipedGeometryList.Add(userTargetGeometry);
                                clipGeometry = otherGeometry;
                                transtationStartLine = transtationStartLineNew;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            resClipedGeometryList.Add(clipGeometry);
                            return resClipedGeometryList;
                        }
                    }
                    resClipedGeometryList.Add(clipGeometry);
                    break;
                case eImportLandClipType.ClipByAverageArea:
                    double clipAverageArea = clipGeometry.Area() / StepCount;
                    //均分面积处理
                    for (int i = 0; i < StepCount - 1; i++)
                    {
                        while (true)
                        {
                            try
                            {
                                userTargetGeometry = GetTargetGeometry(ref otherGeometry, clipGeometry, transtationStartLine, ref transtationStartLineNew, stepLenthUseMethod, flag);
                                if (userTargetGeometry == null)
                                {
                                    resClipedGeometryList.Add(clipGeometry);
                                    return resClipedGeometryList;
                                }
                                areaDeviation = clipAverageArea - userTargetGeometry.Area();
                            }
                            catch
                            {
                                resClipedGeometryList.Add(clipGeometry);
                                return resClipedGeometryList;
                            }
                            //步长处理
                            stepcount = stepcount * 0.5;
                            if (areaDeviation >= -0.001 && areaDeviation <= 0.001)
                            {
                                resClipedGeometryList.Add(userTargetGeometry);
                                clipGeometry = otherGeometry;
                                transtationStartLine = transtationStartLineNew;
                                verticalDistence = verticalDistence - stepLenthUseMethod;
                                stepLenthUseMethod = verticalDistence / 2;
                                stepcount = stepLenthUseMethod;
                                break;
                            }
                            else if (areaDeviation > 0.001)//裁剪的面积过小
                            {
                                stepLenthUseMethod = stepLenthUseMethod + stepcount;
                            }
                            else if (areaDeviation < -0.001)//裁剪的面积过大
                            {
                                stepLenthUseMethod = stepLenthUseMethod - stepcount;
                            }
                        }
                    }
                    resClipedGeometryList.Add(clipGeometry);
                    break;
                case eImportLandClipType.ClipByText:
                    for (int i = 0; i < targetAreaList.Count; i++)
                    {
                        while (true)
                        {
                            try
                            {
                                userTargetGeometry = GetTargetGeometry(ref otherGeometry, clipGeometry, transtationStartLine, ref transtationStartLineNew, stepLenthUseMethod, flag);
                                if (userTargetGeometry == null)
                                {
                                    resClipedGeometryList.Add(clipGeometry);
                                    return resClipedGeometryList;
                                }
                                areaDeviation = targetAreaList[i] - userTargetGeometry.Area();
                            }
                            catch
                            {
                                resClipedGeometryList.Add(clipGeometry);
                                return resClipedGeometryList;
                            }
                            //步长处理
                            stepcount = stepcount * 0.5;
                            if (areaDeviation >= -0.001 && areaDeviation <= 0.001)
                            {
                                resClipedGeometryList.Add(userTargetGeometry);
                                clipGeometry = otherGeometry;
                                transtationStartLine = transtationStartLineNew;
                                verticalDistence = verticalDistence - stepLenthUseMethod;
                                stepLenthUseMethod = verticalDistence / 2;
                                stepcount = stepLenthUseMethod;
                                break;
                            }
                            else if (areaDeviation > 0.001)//裁剪的面积过小
                            {
                                stepLenthUseMethod = stepLenthUseMethod + stepcount;
                            }
                            else if (areaDeviation < -0.001)//裁剪的面积过大
                            {
                                stepLenthUseMethod = stepLenthUseMethod - stepcount;
                            }
                        }
                    }
                    resClipedGeometryList.Add(clipGeometry);
                    break;
                case eImportLandClipType.ClipByTextAndProportion:
                    for (int i = 0; i < targetAreaList.Count - 1; i++)
                    {
                        while (true)
                        {
                            try
                            {
                                userTargetGeometry = GetTargetGeometry(ref otherGeometry, clipGeometry, transtationStartLine, ref transtationStartLineNew, stepLenthUseMethod, flag);
                                if (userTargetGeometry == null)
                                {
                                    resClipedGeometryList.Add(clipGeometry);
                                    return resClipedGeometryList;
                                }
                                areaDeviation = targetAreaList[i] - userTargetGeometry.Area();
                            }
                            catch
                            {
                                resClipedGeometryList.Add(clipGeometry);
                                return resClipedGeometryList;
                            }
                            //步长处理
                            stepcount = stepcount * 0.5;
                            if (areaDeviation >= -0.001 && areaDeviation <= 0.001)
                            {
                                resClipedGeometryList.Add(userTargetGeometry);
                                clipGeometry = otherGeometry;
                                transtationStartLine = transtationStartLineNew;
                                verticalDistence = verticalDistence - stepLenthUseMethod;
                                stepLenthUseMethod = verticalDistence / 2;
                                stepcount = stepLenthUseMethod;
                                break;
                            }
                            else if (areaDeviation > 0.001)//裁剪的面积过小
                            {
                                stepLenthUseMethod = stepLenthUseMethod + stepcount;
                            }
                            else if (areaDeviation < -0.001)//裁剪的面积过大
                            {
                                stepLenthUseMethod = stepLenthUseMethod - stepcount;
                            }
                        }
                    }
                    resClipedGeometryList.Add(clipGeometry);
                    break;
            }

            return resClipedGeometryList;
        }

        /// <summary>
        /// 获取满足条件的要素
        /// </summary>
        /// <param name="item">当前的需求要素数值</param>
        /// <param name="clipGeometry">被裁剪后剩余的要素</param>
        /// <param name="targetGeometry">整体的被裁剪要素</param>
        /// <param name="transtationStartLine">开始的裁剪线</param>
        /// <param name="stepLenthUseMethod">构造平行线的距离</param> 
        /// <returns>返回符合条件的要素</returns>
        private Geometry GetTargetGeometry(ref Geometry clipGeometry, Geometry targetGeometry, Geometry transtationStartLine, ref Geometry transtationStartLineNew, double stepLenthUseMethod, string flag)
        {
            //符合条件的要素
            Geometry targetGeo = null;
            //获取的过程剪切集合
            List<YuLinTu.Spatial.Geometry> clipedGeometryList = new List<Geometry>();
            //获取根据步长移动的后的平行线 
            transtationStartLineNew = getTranstationClipLine(targetGeometry, transtationStartLine, stepLenthUseMethod);

            //获取每步裁剪后的多边形集合
            clipedGeometryList = GetClipPolygonSet(transtationStartLineNew, targetGeometry);
            if (clipedGeometryList == null || clipedGeometryList.Count == 0)
            {
                return targetGeo;
            }
            if (clipedGeometryList.Count == 2)
            {
                if (flag == "0")
                {
                    targetGeo = clipedGeometryList[0];
                    //裁剪完后将另外个取出
                    clipGeometry = clipedGeometryList[1];
                }
                else if (flag == "1")
                {
                    targetGeo = clipedGeometryList[1];
                    clipGeometry = clipedGeometryList[0];
                }
            }
            return targetGeo;
        }

        /// <summary>
        /// 获得与示意线图元上垂直的距离
        /// </summary>      
        private double GetVerticalDistence(YuLinTu.Spatial.Geometry clipGeometry, YuLinTu.Spatial.Geometry drawGeoLine)
        {
            double verticalDistence = 0.0;
            //获取到目标要素所有点列表
            Coordinate[] geoAllPointCdts = clipGeometry.ToCoordinates();

            //判断左右点集
            List<Geometry> leftpointList = new List<Geometry>();
            List<Geometry> rightpointList = new List<Geometry>();

            Geometry geopoint = null;
            for (int i = 0; i < geoAllPointCdts.Count(); i++)
            {
                geopoint = Geometry.CreatePoint(geoAllPointCdts[i], MapControl.SpatialReference);

                int direction = GetPointDirection(geopoint, drawGeoLine);

                if (direction == -1)
                {
                    leftpointList.Add(geopoint);
                }
                else if (direction == 1)
                {
                    rightpointList.Add(geopoint);
                }
            }
            leftFarPoint = GetFarPoint2(leftpointList, drawGeoLine);
            rightFarPoint = GetFarPoint2(rightpointList, drawGeoLine);

            verticalDistence = GetDistance(leftFarPoint, drawGeoLine) + GetDistance(rightFarPoint, drawGeoLine);
            return verticalDistence;
        }

        /// <summary>
        /// 第一次需要判定裁剪线移动方向，根据移动方向来构建新的直线段
        /// </summary>      
        private string GetStepFlag(YuLinTu.Spatial.Geometry clipGeometry, YuLinTu.Spatial.Geometry transtationStartLine)
        {
            Coordinate[] transtationStartLineCdts = transtationStartLine.ToCoordinates();
            double k = (transtationStartLineCdts[1].Y - transtationStartLineCdts[0].Y) / (transtationStartLineCdts[1].X - transtationStartLineCdts[0].X);
            double b = transtationStartLineCdts[0].Y - k * transtationStartLineCdts[0].X;
            //新直线方程系数，c1 c2 对应了位移  kx - y + b = 0  d*开方(a*a + b*b) = |c1 - c2|;d为距离
            double c1 = step * (Math.Sqrt(k * k + 1)) + b;
            double c2 = b - step * (Math.Sqrt(k * k + 1));

            //获得要素当前的范围框
            Geometry extendgeo = clipGeometry.Envelope();
            Coordinate[] extendCdts = extendgeo.ToCoordinates();

            //根据不同的直线方程构造点
            Coordinate xminCdt1 = new Coordinate();
            xminCdt1.X = extendCdts[0].X / 2;
            xminCdt1.Y = k * xminCdt1.X + c1;

            Coordinate xmaxCdt1 = new Coordinate();
            xmaxCdt1.X = extendCdts[2].X * 2;
            xmaxCdt1.Y = k * xmaxCdt1.X + c1;

            Geometry c1line = GetLine(xminCdt1, xmaxCdt1);

            Coordinate xminCdt2 = new Coordinate();
            xminCdt2.X = extendCdts[0].X / 2;
            xminCdt2.Y = k * xminCdt2.X + c2;

            Coordinate xmaxCdt2 = new Coordinate();
            xmaxCdt2.X = extendCdts[2].X * 2;
            xmaxCdt2.Y = k * xmaxCdt2.X + c2;

            Geometry c2line = GetLine(xminCdt2, xmaxCdt2);

            if (c1line.Intersects(clipGeometry))
            {
                stepFlag = "addDirection";
            }
            else if (c2line.Intersects(clipGeometry))
            {
                stepFlag = "reduceDirection";
            }

            return stepFlag;
        }

        /// <summary>
        /// 获取示意线对应的已移动平行线
        /// </summary>      
        private YuLinTu.Spatial.Geometry getTranstationClipLine(YuLinTu.Spatial.Geometry clipGeometry, YuLinTu.Spatial.Geometry transtationLine, double stepLength)
        {
            //新的移动的平行线
            YuLinTu.Spatial.Geometry transtationClipLine = new Spatial.Geometry();

            Coordinate[] transtationLineCdts = transtationLine.ToCoordinates();
            double k = (transtationLineCdts[1].Y - transtationLineCdts[0].Y) / (transtationLineCdts[1].X - transtationLineCdts[0].X);
            double b = transtationLineCdts[0].Y - k * transtationLineCdts[0].X;
            //新直线方程系数，c1 c2 对应了位移  kx - y + b = 0
            double c = 0.0;
            if (stepFlag == "addDirection")
            {
                c = stepLength * (Math.Sqrt(k * k + 1)) + b;
            }
            else if (stepFlag == "reduceDirection")
            {
                c = b - stepLength * (Math.Sqrt(k * k + 1));
            }
            //获得要素当前的范围框
            Geometry extendgeo = clipGeometry.Envelope();
            Coordinate[] extendCdts = extendgeo.ToCoordinates();

            //根据不同的直线方程构造点
            Coordinate xminCdt = new Coordinate();
            xminCdt.X = extendCdts[0].X / 2;
            xminCdt.Y = k * xminCdt.X + c;

            Coordinate xmaxCdt = new Coordinate();
            xmaxCdt.X = extendCdts[2].X * 2;
            xmaxCdt.Y = k * xmaxCdt.X + c;

            transtationClipLine = GetLine(xminCdt, xmaxCdt);

            return transtationClipLine;
        }

        /// <summary>
        /// 首先获取最开始的平行线，从离示意线一边最远的点开始的平行线
        /// </summary>       
        private Geometry getTranstationStartLine(YuLinTu.Spatial.Geometry clipGeometry, YuLinTu.Spatial.Geometry drawGeoLine)
        {
            Geometry transStartLine = new Geometry();
            Coordinate[] drawGeoLineCdts = drawGeoLine.ToCoordinates();

            //获取当前示意线的向量
            Vector drawLineVector = GetVector(drawGeoLineCdts[0], drawGeoLineCdts[1]);
            //旋转方向为逆时针
            System.Windows.Media.Matrix matrix = System.Windows.Media.Matrix.Identity;
            matrix.Rotate(90);

            //获取当前示意线垂线的向量
            Vector verticalVector = Vector.Multiply(drawLineVector, matrix);

            //获取当前示意线最左边点到垂点的向量   
            Coordinate[] leftFarPointCdts = leftFarPoint.ToCoordinates();
            Coordinate leftFarPointVtcPt = GetVerticalPoint(leftFarPointCdts[0], drawGeoLine);
            Vector leftFarPointToVtcVector = GetVector(drawGeoLineCdts[1], leftFarPointVtcPt);

            //获取当前示意线最右边点到垂点的向量            
            Coordinate[] rightFarPointCdts = rightFarPoint.ToCoordinates();
            //Coordinate rightFarPointVtcPt = GetVerticalPoint(leftFarPointCdts[0], drawGeoLine);
            //Vector rightFarPointToVtcVector = GetVector(rightFarPointVtcPt, drawGeoLineCdts[1]);

            //示意线垂线向量坐标正负判断
            string zfVerticalxFlag = "负号";
            string zfVerticalyFlag = "负号";

            if (Math.Abs(verticalVector.X) == verticalVector.X)
            {
                zfVerticalxFlag = "正号";
            }
            else
            {
                zfVerticalxFlag = "负号";
            }
            if (Math.Abs(verticalVector.Y) == verticalVector.Y)
            {
                zfVerticalyFlag = "正号";
            }
            else
            {
                zfVerticalyFlag = "负号";
            }

            //左边最远点向量坐标正负判断
            string zfxFlag = "负号";
            string zfyFlag = "负号";
            if (Math.Abs(leftFarPointToVtcVector.X) == leftFarPointToVtcVector.X)
            {
                zfxFlag = "正号";
            }
            else
            {
                zfxFlag = "负号";
            }
            if (Math.Abs(leftFarPointToVtcVector.Y) == leftFarPointToVtcVector.Y)
            {
                zfyFlag = "正号";
            }
            else
            {
                zfyFlag = "负号";
            }

            //获得要素当前的范围框
            Geometry extendgeo = clipGeometry.Envelope();
            Coordinate[] extendCdts = extendgeo.ToCoordinates();
            //当前示意线的斜率
            double k = (drawGeoLineCdts[1].Y - drawGeoLineCdts[0].Y) / (drawGeoLineCdts[1].X - drawGeoLineCdts[0].X);
            double b = 0.0;

            //如果方向相同,就选左边点，否则选右边点
            if (zfVerticalxFlag == zfxFlag && zfVerticalyFlag == zfyFlag)
            {
                b = leftFarPointCdts[0].Y - k * leftFarPointCdts[0].X;
            }
            else
            {
                b = rightFarPointCdts[0].Y - k * rightFarPointCdts[0].X;
            }

            //获取xmin对应的y值及获取xmax对应的y值
            Coordinate xminCdt = new Coordinate();
            xminCdt.X = extendCdts[0].X / 2;
            xminCdt.Y = k * xminCdt.X + b;

            Coordinate xmaxCdt = new Coordinate();
            xmaxCdt.X = extendCdts[2].X * 2;
            xmaxCdt.Y = k * xmaxCdt.X + b;

            transStartLine = GetLine(xminCdt, xmaxCdt);

            return transStartLine;
        }

        /// <summary>
        /// 获得离线段最远的点-使用接口distence
        /// </summary>
        /// <param name="pointLst">点集合</param>
        /// <param name="polyline">线段</param>
        /// <returns>离分割线最远的点</returns>
        private Geometry GetFarPoint1(List<Geometry> pointList, Geometry polyline)
        {
            Geometry resultPoint = null;
            double distance = 0;

            for (int i = 0; i < pointList.Count; i++)
            {
                Geometry point = pointList[i];
                double theDistance = point.Distance(polyline);

                if (theDistance < distance)
                {
                    continue;
                }
                distance = theDistance;
                resultPoint = point;
            }
            return resultPoint;
        }

        /// <summary>
        /// 获得离线段最远的点-自写点到直线距离方法获取
        /// </summary>
        /// <param name="pointLst">点集合</param>
        /// <param name="polyline">线段</param>
        /// <returns>离分割线最远的点</returns>
        private Geometry GetFarPoint2(List<Geometry> pointList, Geometry polyline)
        {
            Geometry resultPoint = null;
            double distance = 0;
            for (int i = 0; i < pointList.Count; i++)
            {
                Geometry point = pointList[i];
                double theDistance = GetDistance(point, polyline);

                if (theDistance < distance)
                {
                    continue;
                }
                distance = theDistance;
                resultPoint = point;
            }
            return resultPoint;
        }

        /// <summary>
        /// 点到直线的距离-自定义的方法，特殊情况需要特别处理
        /// </summary>      
        private double GetDistance(Geometry point, Geometry polyline)
        {
            double theDistance = 0.0;
            Coordinate[] linecdts = polyline.ToCoordinates();

            Coordinate pointA = linecdts[0];
            Coordinate pointB = linecdts[1];

            Coordinate[] pointcdts = point.ToCoordinates();

            if (pointA.X == pointB.X)
            {
                //如果直线是垂直状态则特殊处理
                theDistance = Math.Abs(Math.Abs(pointA.X) - Math.Abs(pointcdts[0].X));
            }
            else
            {
                double a = (pointA.Y - pointB.Y) / (pointA.X - pointB.X);
                double b = pointA.Y - a * pointA.X;
                //0= ax + b -y
                theDistance = (Math.Abs(a * pointcdts[0].X + b - pointcdts[0].Y)) / Math.Sqrt(a * a + 1);
            }
            return theDistance;
        }

        /// <summary>
        /// 判断点与线段的关系-如果在线段的延长线上，也会被判断为在线段上
        /// </summary>
        /// <param name="point">判断的点</param>
        /// <param name="polyline">线段</param>
        /// <returns>-1:左边；0:线上；1:右边</returns>
        private int GetPointDirection(Geometry point, Geometry polyline)
        {
            Coordinate[] linecdts = polyline.ToCoordinates();

            Coordinate pointA = linecdts[0];
            Coordinate pointB = linecdts[1];

            Coordinate[] pointcdts = point.ToCoordinates();

            double ax = pointB.X - pointA.X;
            double ay = pointB.Y - pointA.Y;
            double bx = pointcdts[0].X - pointA.X;
            double by = pointcdts[0].Y - pointA.Y;

            double flag = ax * by - ay * bx;

            if (flag > 0)
            {
                return -1;
            }
            else if (flag < 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取地图界面设定的最大范围位置的4条边框线-上 左 下 右
        /// </summary>
        private List<Geometry> GetMapMaxExtendsLineList()
        {
            double xmin = int.MinValue;
            double xmax = int.MaxValue;
            double ymin = int.MinValue;
            double ymax = int.MaxValue;

            SpatialReference nowSRF = MapControl.SpatialReference;

            Coordinate leftMinCdt = new Coordinate(xmin, ymin);
            Coordinate leftMaxCdt = new Coordinate(xmin, ymax);

            Coordinate RightMinCdt = new Coordinate(xmax, ymin);
            Coordinate RightMaxCdt = new Coordinate(ymax, ymax);

            List<Coordinate> leftLineCdts = new List<Coordinate>();
            leftLineCdts.Add(leftMinCdt);
            leftLineCdts.Add(leftMaxCdt);

            List<Coordinate> rightLineCdts = new List<Coordinate>();
            rightLineCdts.Add(RightMinCdt);
            rightLineCdts.Add(RightMaxCdt);

            List<Coordinate> topLineCdts = new List<Coordinate>();
            topLineCdts.Add(RightMaxCdt);
            topLineCdts.Add(leftMaxCdt);

            List<Coordinate> downLineCdts = new List<Coordinate>();
            downLineCdts.Add(RightMinCdt);
            downLineCdts.Add(leftMinCdt);

            Geometry topExtendLine = Geometry.CreatePolyline(leftLineCdts, nowSRF);
            Geometry leftExtendLine = Geometry.CreatePolyline(rightLineCdts, nowSRF);
            Geometry rightExtendLine = Geometry.CreatePolyline(topLineCdts, nowSRF);
            Geometry downExtendLine = Geometry.CreatePolyline(downLineCdts, nowSRF);

            List<Geometry> mapMaxExtendsLineList = new List<Geometry>();
            mapMaxExtendsLineList.Add(topExtendLine);
            mapMaxExtendsLineList.Add(leftExtendLine);
            mapMaxExtendsLineList.Add(downExtendLine);
            mapMaxExtendsLineList.Add(rightExtendLine);

            return mapMaxExtendsLineList;
        }

        /// <summary>
        /// 获取两个线段相交点
        /// </summary>      
        private Coordinate GetIntersectCdt(Geometry lineA, Geometry LineB)
        {
            Coordinate[] coordinates = new Coordinate[] { };
            if (lineA.Intersects(LineB))
            {
                Geometry geo = lineA.Intersection(LineB);
                if (geo != null)
                {
                    coordinates = geo.ToCoordinates();
                }
            }
            return coordinates[0];
        }

        /// <summary>
        /// 根据两个点获取对应的线段
        /// </summary>
        private Geometry GetLine(Coordinate pnta, Coordinate pntb)
        {
            List<Coordinate> linecdts = new List<Coordinate>();
            linecdts.Add(pnta);
            linecdts.Add(pntb);
            Geometry line = Geometry.CreatePolyline(linecdts, MapControl.SpatialReference);
            return line;
        }

        /// <summary>
        /// 根据点和线获取对应垂点
        /// </summary>
        /// <param name="ponit">原始点</param>
        /// <param name="targetLine">目标线</param>
        private Coordinate GetVerticalPoint(Coordinate point, Geometry targetLine)
        {
            Coordinate verticalPoint = new Coordinate();

            Coordinate[] lineCdts = targetLine.ToCoordinates();
            double k = (lineCdts[1].Y - lineCdts[0].Y) / (lineCdts[1].X - lineCdts[0].X);
            double b = lineCdts[1].Y - k * lineCdts[1].X;

            double bb = point.X + k * point.Y;
            verticalPoint.X = ((bb - k * b) / (k * k + 1));
            verticalPoint.Y = (k * verticalPoint.X + b);

            return verticalPoint;
        }

        /// <summary>
        /// 获得两点构成的向量,A到B的向量
        /// </summary>
        /// <param name="pointA">起始点</param>
        /// <param name="pointB">终点</param>
        private Vector GetVector(Coordinate pointA, Coordinate pointB)
        {
            Vector vtr = new Vector();
            vtr.X = pointB.X - pointA.X;
            vtr.Y = pointB.Y - pointA.Y;
            return vtr;
        }

        #endregion      

        /// <summary>
        /// 显示面积
        /// </summary>
        private void DisplayLabel(List<YuLinTu.Spatial.Geometry> getGeoList)
        {
            List<LabelObject> objectLabelList = new List<LabelObject>();
            foreach (var feature in getGeoList)
            {
                Graphic centerPoint = new Graphic() { Object = new FeatureObject() { Object = objectLabel = new LabelObject() } };
                centerPoint.Symbol = Application.Current.TryFindResource("UISymbol_Mark_MeasureArea_Label") as MarkSymbol;
                centerPoint.Geometry = null;
                layerLabel.Graphics.Add(centerPoint);
                objectLabelList.Add(objectLabel);
            }

            for (int i = 0; i < getGeoList.Count; i++)
            {
                //var area = ToolMath.CutNumericFormat(getGeoList[i].Area() * projectionUnit, 2);
                var area = Math.Round(getGeoList[i].Area() * projectionUnit, 2);
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

        #endregion

        #endregion
    }
}
