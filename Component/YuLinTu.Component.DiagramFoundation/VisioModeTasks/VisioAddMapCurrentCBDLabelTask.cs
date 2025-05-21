using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Components.Diagrams;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.SQLite;
using YuLinTu.Diagrams;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Visio.Designer;
using YuLinTu.Visio.Designer.Elements;
using YuLinTu.Windows;
using YuLinTu;
using YuLinTu.Library.WorkStation;
namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// 添加当前地域下承包地图面所有标注
    /// </summary>
    public class VisioAddMapCurrentCBDLabelTask : Task
    {
        #region Fields

        private List<Layer> contractLandLableLayers = null;//用于标注的承包地图层
        private IDbContext dbContext = null;
        private Zone currentZone = null;//当前地域
        private DiagramFoundationLabelCommonSetting dflcsetting = DiagramFoundationLabelCommonSetting.GetIntence();
                
        #endregion

        #region Propertys

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            var args = Argument as VisioAddMapCurrentCBDLabelTaskArgument;
            dbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = dbContext.CreateZoneWorkStation();
            if (args.CurrentZoneCode == null || args.CurrentZoneCode == "")
            {
                this.ReportWarn("未选择行政区域！");
                return;
            }
            currentZone = zoneStation.Get(args.CurrentZoneCode);
            this.ReportProgress(0, "开始");
            this.ReportAlert(eMessageGrade.Infomation, null, "开始图面地块标注...");
            if (!CreateDiagramsView(args))
                return;
                       
            
            this.ReportProgress(100, "图面地块标注完成");
            this.ReportAlert(eMessageGrade.Infomation, null, "图面地块标注完成");
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 创建主要标注
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool CreateDiagramsView(VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            ElementsDesigner view = args.VisiosView;
            if (view == null) return false;
            try
            {
                bool hasMap = true;
                bool hasCBDlayer = true;
                MapUI ui = null;
                Envelope landExtent = null;
                Element diagramMap = null;

                view.Dispatcher.Invoke(new Action(() =>
                {
                    var mainSheet = view.FindSheetByName("template");//主要的模板层，放置了主要的东西
                    var diagram = mainSheet.FindElementByName("map");

                    if (diagram == null)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null,
                            string.Format("未在模版中找到地图，操作将中止。"));

                        hasMap = false;
                        return;
                    }

                    diagramMap = diagram;
                    ui = diagramMap.Content as MapUI;
                    landExtent = ui.MapControl.Extend;
                    contractLandLableLayers = ui.MapControl.Layers.GetSubLayers().FindAll(l => (l is VectorLayer) ? (l as VectorLayer).DataSource.ElementName == "ZD_CBD" : false);
                    if (contractLandLableLayers == null)
                    {
                        hasCBDlayer = false;
                        return;
                    }
                }));

                if (!hasMap)
                    return false;
                if (!hasCBDlayer)
                    return false;
                this.ReportProgress(20, "开始图面地块标注...");

                AddGroupZoneLabel(view, diagramMap, ui, landExtent, args);

                ElementsSheet contractLandLableSheet = null;
                view.Dispatcher.Invoke(new Action(() =>
                   {
                       contractLandLableSheet = new ElementsSheet();
                       contractLandLableSheet.Name = "contractLandLabelSheet";
                       contractLandLableSheet.AliasName = "承包地注记层";
                       contractLandLableSheet.Description = "模板中的承包地注记都在这层";
                   }));

                AddContractLandAllViewLabel(view, diagramMap, ui, landExtent, contractLandLableSheet, args);

                AdddzdwAllViewLabel(view, diagramMap, ui, landExtent, contractLandLableSheet, args);

                AddxzdwAllViewLabel(view, diagramMap, ui, landExtent, contractLandLableSheet, args);

                AddmzdwAllViewLabel(view, diagramMap, ui, landExtent, contractLandLableSheet, args);

                view.Dispatcher.Invoke(new Action(() =>
               {
                   view.Sheets.Add(contractLandLableSheet);//主要的模板层，放置了主要的东西
                   contractLandLableSheet.Refresh();
               }));
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("设置模版的过程中发生错误，操作将中止。错误信息如下：{0}", ex));

                return false;
            }
            return true;
        }
               
        
        private Point GetLocation(Envelope mapExtent, double mapWidth, double mapHeight,
           double diagramWidth, double diagramHeight, YuLinTu.Spatial.Geometry geo)
        {
            var res = Math.Max(mapExtent.Width / mapWidth, mapExtent.Height / mapHeight);

            var dWidth = res * mapWidth;
            var dHeight = res * mapHeight;

            var extent = new Envelope()
            {
                MinX = mapExtent.MinX - dWidth / 2 + mapExtent.Width / 2,
                MaxX = mapExtent.MaxX + dWidth / 2 - mapExtent.Width / 2,
                MaxY = mapExtent.MaxY + dHeight / 2 - mapExtent.Height / 2,
                MinY = mapExtent.MinY - dHeight / 2 + mapExtent.Height / 2
            };

            var center = new Coordinate(geo.Instance.PointOnSurface.X, geo.Instance.PointOnSurface.Y);

            var x = (center.X - extent.MinX) / res - diagramWidth / 2;
            var y = (extent.MaxY - center.Y) / res - diagramHeight / 2;

            return new Point(x, y);
        }
        #endregion

        #region   Methods - private

        /// <summary>
        /// 添加组级注记
        /// </summary>
        private void AddGroupZoneLabel(ElementsDesigner view, Element diagramMap, MapUI ui, Envelope landExtent, VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            view.Dispatcher.Invoke(new Action(() =>
            {
                ElementsSheet zzonelabelSheet = new ElementsSheet();
                zzonelabelSheet.Name = "groupZoneLabelSheet";
                zzonelabelSheet.AliasName = "组级注记层";
                zzonelabelSheet.Description = "模板中的组级注记都在这层";

                var zoneZlayer = ui.MapControl.Layers.GetSubLayers().Find(l => l.Name == "组") as VectorLayer;
                //添加组级别地域标识
                if (zoneZlayer != null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneZlayer.DataSource.ForEachByIntersects(landExtent.ToGeometry(), (i, cnt, obj) =>
                    {
                        if (!obj.Geometry.IsValid())
                            return true;
                        var zonecode = obj.Object.GetPropertyValue<string>("DYQBM");
                        if (zonecode == null) return true;
                        var objzone = zoneStation.Get(zonecode);
                        if (objzone == null || objzone.FullCode == currentZone.FullCode) return true;
                        var objparentzone = GetParent(objzone);

                        var intersectobj = obj.Geometry.Intersection(landExtent.ToGeometry());
                        if (intersectobj != null)
                        {
                            var label = new TextModel().CreateElement();
                            (label.Model as TextableModel).Text = (objparentzone.Name != null ? objparentzone.Name : "") + objzone.Name;
                            label.Model.Name = "groupZoneAllViewLable";
                            System.Windows.Media.SolidColorBrush SCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.CbdxzqySetInfo.GroupZoneLabelColor)));
                            (label.Model as TextModel).Foreground = SCB;
                            (label.Model as TextModel).FontSize = args.VisioMapLayoutSetInfo.CbdxzqySetInfo.GroupZoneLabelSize;
                            var size = new Size(args.VisioMapLayoutSetInfo.CbdxzqySetInfo.GroupZoneLabelSize * 10, args.VisioMapLayoutSetInfo.CbdxzqySetInfo.GroupZoneLabelSize * 4);//20对应200,80
                            var location = GetLocation(landExtent,
                                diagramMap.Model.Width, diagramMap.Model.Height,
                                size.Width, size.Height, intersectobj);

                            label.Model.X = location.X + diagramMap.Model.X;
                            label.Model.Y = location.Y + diagramMap.Model.Y;
                            label.Model.Width = size.Width;
                            label.Model.Height = size.Height;
                            label.Model.BorderWidth = 0;

                            zzonelabelSheet.Items.Add(label);

                            return true;
                        }
                        return true;
                    });
                    view.Sheets.Add(zzonelabelSheet);//主要的模板层，放置了主要的东西  
                    zzonelabelSheet.Refresh();
                }
            }));
        }

        /// <summary>
        /// 添加承包地注记
        /// </summary>
        private void AddContractLandAllViewLabel(ElementsDesigner view, Element diagramMap, MapUI ui, Envelope landExtent, ElementsSheet contractLandLableSheet, VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            foreach (var item in contractLandLableLayers)
            {
                var contractLandLableLayer = item as VectorLayer;
                if (contractLandLableLayer == null) continue;

                contractLandLableLayer.DataSource.ForEachByIntersects(landExtent.ToGeometry(), (i, cnt, obj) =>
                {
                    if (!obj.Geometry.IsValid())
                        return true;

                    view.Dispatcher.Invoke(new Action(() =>
                    {
                        var label = new ListTextModel().CreateElement();
                        string showCenterText = string.Empty;
                        string showLandNumber = string.Empty;
                        string showVpName = string.Empty;                      
                        string showLandName = string.Empty;
                        string showLandSurveyNumber = string.Empty;
                        double landscmj = obj.Object.GetPropertyValue<double>("SCMJ");
                        double landqqmj = obj.Object.GetPropertyValue<double>("BZMJ");
                        var tzmjobj = obj.Object.GetPropertyValue("TZMJ");
                        double landtzmj = (tzmjobj == null||tzmjobj.ToString()=="") ? 0 : Convert.ToDouble(tzmjobj.ToString());// obj.Object.GetPropertyValue<double>("TZMJ");

                        if (dflcsetting.IsUseContractorName)
                        {
                            showVpName = obj.Object.GetPropertyValue<string>("QLRMC");
                            if (showVpName != null)
                            {
                                showCenterText = string.Format("{0}\n{1}", showCenterText, showVpName);
                            }
                        }

                        if (dflcsetting.IsUseLandSurveyNumber)
                        {
                            showLandSurveyNumber = obj.Object.GetPropertyValue<string>("DCBM");
                            if (showLandSurveyNumber != null)
                            {
                                showCenterText = string.Format("{0}\n{1}", showCenterText, showLandSurveyNumber);
                            }
                        }

                        if (dflcsetting.IsUseLandNumber)
                        {
                            showLandNumber = obj.Object.GetPropertyValue<string>("DKBM");
                            if (showLandNumber.IsNullOrEmpty() == false && showLandNumber.Length == 19)
                            {
                                showLandNumber = showLandNumber.Substring(dflcsetting.GetLandMiniNumber, showLandNumber.Length- dflcsetting.GetLandMiniNumber);
                            }
                            showCenterText = string.Format("{0}\n{1}", showCenterText, showLandNumber);
                        }

                        if (dflcsetting.IsUseLandName)
                        {
                            showLandName = obj.Object.GetPropertyValue<string>("DKMC");
                            if (showLandName != null)
                            {
                                showCenterText = string.Format("{0}\n{1}", showCenterText, showLandName);
                            }
                        }

                        if (dflcsetting.IsUseLandTableArea)
                        {
                            showCenterText = string.Format("{0}\n{1}", showCenterText, ToolMath.SetNumbericFormat(landtzmj.ToString(), 2));
                            if (dflcsetting.IsUseLandAreaUnitMu)
                            {
                                showCenterText += "亩";
                            }
                        }
                        
                        if (dflcsetting.IsUseLandAwareArea)
                        {
                            showCenterText = string.Format("{0}\n{1}", showCenterText,ToolMath.SetNumbericFormat(landqqmj.ToString(), 2));
                            if (dflcsetting.IsUseLandAreaUnitMu)
                            {
                                showCenterText += "亩";
                            }
                        }

                        if (dflcsetting.IsUseLandActualArea)
                        {
                            showCenterText = string.Format("{0}\n{1}", showCenterText,ToolMath.SetNumbericFormat(landscmj.ToString(), 2));
                            if (dflcsetting.IsUseLandAreaUnitMu)
                            {
                                showCenterText += "亩";
                            }
                        }                                      

                        (label.Model as ListTextModel).Text = showCenterText;
                        
                        label.Model.Name = "contractLandAllViewLable";
                        System.Windows.Media.SolidColorBrush SCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelFontColor)));
                        System.Windows.Media.SolidColorBrush SeparatorBrushSCB = new System.Windows.Media.SolidColorBrush(dflcsetting.LandSeparateLineColor);
                        (label.Model as ListTextModel).Foreground = SCB;
                        (label.Model as ListTextModel).SeparatorWidth = args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelSeparatorHeight;
                        (label.Model as ListTextModel).SeparatorBrush = SeparatorBrushSCB;
                        (label.Model as ListTextModel).FontSize = args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelSize;
                        (label.Model as ListTextModel).FontFamily = dflcsetting.UseLandLabelFontSet;
                        //var size = new Size(args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelSize * 4, args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelSize * 4);//4倍与字体关系,5对应20
                        var size = calcTextSize(showCenterText, args);
                        var location = GetLocation(landExtent,
                            diagramMap.Model.Width, diagramMap.Model.Height,
                            size.Width, size.Height, obj.Geometry);

                        label.Model.X = location.X + diagramMap.Model.X;
                        label.Model.Y = location.Y + diagramMap.Model.Y;
                        label.Model.Width = size.Width;
                        label.Model.Height = size.Height;
                        label.Model.BorderWidth = 0;

                        contractLandLableSheet.Items.Add(label);
                    }));
                    return true;
                });
            }
        }

        /// <summary>
        /// 添加点状注记
        /// </summary>
        private void AdddzdwAllViewLabel(ElementsDesigner view, Element diagramMap, MapUI ui, Envelope landExtent, ElementsSheet contractLandLableSheet, VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            //添加点状地物标注
            view.Dispatcher.Invoke(new Action(() =>
            {
                var dzdwZlayer = ui.MapControl.Layers.GetSubLayers().Find(l => l.InternalName == "dzdw_Layer") as VectorLayer;
                //添加组级别地域标识
                if (dzdwZlayer != null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    dzdwZlayer.DataSource.ForEachByIntersects(landExtent.ToGeometry(), (i, cnt, obj) =>
                    {
                        if (!obj.Geometry.IsValid())
                            return true;

                        var label = new TextModel().CreateElement();
                        (label.Model as TextableModel).Text = obj.Object.GetPropertyValue<string>("DWMC");
                        label.Model.Name = "dzdwAllViewLable";
                        System.Windows.Media.SolidColorBrush SCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.DZDWLabelFontColor)));
                        (label.Model as TextModel).Foreground = SCB;
                        (label.Model as TextModel).FontSize = args.VisioMapLayoutSetInfo.DxmzdwSetInfo.DZDWLabelSize;
                        var size = new Size(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.DZDWLabelSize * 5, args.VisioMapLayoutSetInfo.DxmzdwSetInfo.DZDWLabelSize * 2);//20对应200,80
                        var location = GetLocation(landExtent,
                            diagramMap.Model.Width, diagramMap.Model.Height,
                            size.Width, size.Height, obj.Geometry);

                        label.Model.X = location.X + diagramMap.Model.X;
                        label.Model.Y = location.Y + diagramMap.Model.Y;
                        label.Model.Width = size.Width;
                        label.Model.Height = size.Height;
                        label.Model.BorderWidth = 0;

                        contractLandLableSheet.Items.Add(label);
                        return true;
                    });
                }
            }));


        }

        /// <summary>
        /// 添加线状注记
        /// </summary>
        private void AddxzdwAllViewLabel(ElementsDesigner view, Element diagramMap, MapUI ui, Envelope landExtent, ElementsSheet contractLandLableSheet, VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            //添加线状地物标注
            view.Dispatcher.Invoke(new Action(() =>
            {
                var xzdwZlayer = ui.MapControl.Layers.GetSubLayers().Find(l => l.InternalName == "xzdw_Layer") as VectorLayer;
                //添加组级别地域标识
                if (xzdwZlayer != null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    xzdwZlayer.DataSource.ForEachByIntersects(landExtent.ToGeometry(), (i, cnt, obj) =>
                    {
                        if (!obj.Geometry.IsValid())
                            return true;

                        var label = new TextModel().CreateElement();
                        (label.Model as TextableModel).Text = obj.Object.GetPropertyValue<string>("DWMC");
                        label.Model.Name = "xzdwAllViewLable";
                        System.Windows.Media.SolidColorBrush SCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.XZDWLabelFontColor)));
                        (label.Model as TextModel).Foreground = SCB;
                        (label.Model as TextModel).FontSize = args.VisioMapLayoutSetInfo.DxmzdwSetInfo.XZDWLabelSize;
                        var size = new Size(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.XZDWLabelSize * 5, args.VisioMapLayoutSetInfo.DxmzdwSetInfo.XZDWLabelSize * 2);//20对应200,80
                        var location = GetLocation(landExtent,
                            diagramMap.Model.Width, diagramMap.Model.Height,
                            size.Width, size.Height, obj.Geometry);

                        label.Model.X = location.X + diagramMap.Model.X;
                        label.Model.Y = location.Y + diagramMap.Model.Y;
                        label.Model.Width = size.Width;
                        label.Model.Height = size.Height;
                        label.Model.BorderWidth = 0;

                        contractLandLableSheet.Items.Add(label);
                        return true;
                    });
                }
            }));
        }

        /// <summary>
        /// 添加面状注记
        /// </summary>
        private void AddmzdwAllViewLabel(ElementsDesigner view, Element diagramMap, MapUI ui, Envelope landExtent, ElementsSheet contractLandLableSheet, VisioAddMapCurrentCBDLabelTaskArgument args)
        {
            //添加面状地物标注
            view.Dispatcher.Invoke(new Action(() =>
            {
                var mzdwZlayer = ui.MapControl.Layers.GetSubLayers().Find(l => l.InternalName == "mzdw_Layer") as VectorLayer;
                //添加组级别地域标识
                if (mzdwZlayer != null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    mzdwZlayer.DataSource.ForEachByIntersects(landExtent.ToGeometry(), (i, cnt, obj) =>
                    {
                        if (!obj.Geometry.IsValid())
                            return true;

                        var label = new TextModel().CreateElement();
                        (label.Model as TextableModel).Text = obj.Object.GetPropertyValue<string>("DWMC");
                        label.Model.Name = "mzdwAllViewLable";
                        System.Windows.Media.SolidColorBrush SCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.MZDWLabelFontColor)));
                        (label.Model as TextModel).Foreground = SCB;
                        (label.Model as TextModel).FontSize = args.VisioMapLayoutSetInfo.DxmzdwSetInfo.MZDWLabelSize;
                        var size = new Size(args.VisioMapLayoutSetInfo.DxmzdwSetInfo.MZDWLabelSize * 5, args.VisioMapLayoutSetInfo.DxmzdwSetInfo.MZDWLabelSize * 2);//20对应200,80
                        var location = GetLocation(landExtent,
                            diagramMap.Model.Width, diagramMap.Model.Height,
                            size.Width, size.Height, obj.Geometry);

                        label.Model.X = location.X + diagramMap.Model.X;
                        label.Model.Y = location.Y + diagramMap.Model.Y;
                        label.Model.Width = size.Width;
                        label.Model.Height = size.Height;
                        label.Model.BorderWidth = 0;

                        contractLandLableSheet.Items.Add(label);
                        return true;
                    });
                }
            }));
        }

        #endregion

        #region Methods - help

        public Size calcTextSize(string text, VisioAddMapCurrentCBDLabelTaskArgument args)// FontFamily fontFamily, double fontSize, FontWeights fontWeights)
        {
            var typeFace = new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily(dflcsetting.UseLandLabelFontSet),// new System.Windows.Media.FontFamily(fontName),
                          System.Windows.FontStyles.Normal,// System.Windows.FontStyles.Normal,
                          System.Windows.FontWeights.Normal,// bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
                          System.Windows.FontStretches.Normal);// System.Windows.FontStretches.Normal);

            var fontSize = (double)new System.Windows.FontSizeConverter().ConvertFrom(args.VisioMapLayoutSetInfo.CbdxzqySetInfo.CbdLabelSize + "pt");
            var ft = new System.Windows.Media.FormattedText(text, System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight
                , typeFace, fontSize, System.Windows.Media.Brushes.Black);
            return new Size(ft.Width, ft.Height * 0.76);
        }


        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        #endregion

        #endregion
    }
}
