using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.SQLite;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Visio.Designer;
using YuLinTu.Visio.Designer.Elements;
using YuLinTu.Windows;

namespace YuLinTu.Component.DiagramFoundation
{
    public class MapToVisiosViewTask : Task
    {
        #region Fields

        private List<VectorLayer> listLayers = null;//所有的图层
        private Zone currentZone = null;//当前地域
        private List<Zone> allZones = null;//当前地域，村、镇
        private IDbContext dbContext = null;
        private VectorLayer contractLandLableLayer = null;//用于标注的承包地图层
        private Envelope landExtent = null;//出图范围
        private DiagramFoundationLabelCommonSetting dflcsetting = DiagramFoundationLabelCommonSetting.GetIntence();

        ElementsDesigner view = null;//整个视图
        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            var args = Argument as MapToVisiosViewTaskArgument;
            dbContext = DataBaseSource.GetDataBaseSource();
            var zoneStation = dbContext.CreateZoneWorkStation();
            if (args.currentZoneCode == null || args.currentZoneCode == "")
            {
                this.ReportWarn("未选择行政区域！");
                return;
            }
            currentZone = zoneStation.Get(args.currentZoneCode);
            allZones = zoneStation.GetAllZones(currentZone);

            this.ReportProgress(0, "开始验证参数...");
            this.ReportAlert(eMessageGrade.Infomation, null, "开始验证参数...");
            if (!ValidateArgument(args))
                return;

            this.ReportProgress(5, "开始导出数据...");//导出数据时，将地域图层都导出、承包地图层选择导出村或者组下对应地块
            this.ReportAlert(eMessageGrade.Infomation, null, "开始导出数据...");
            if (!ExportData(args))
                return;

            this.ReportProgress(90, "开始设置制图模板...");
            this.ReportAlert(eMessageGrade.Infomation, null, "开始设置制图模板...");
            if (!CreateDiagramsView(args))
                return;

            if (!CreateOtherDiagramsView(view))
                return;

            this.ReportProgress(100, "导出到制图完成");
            this.ReportAlert(eMessageGrade.Infomation, null, "导出到制图完成");
        }

        #endregion

        #region Methods - Private

        private bool CreateDiagramsView(MapToVisiosViewTaskArgument args)
        {
            landExtent = new Envelope();//开始绘制重新定义范围
            try
            {
                args.MapControl.Dispatcher.Invoke(new Action(() =>
                {
                    view = new ElementsDesigner();
                    view.LoadFrom(args.TemplateFileName);
                }));

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("模版加载成功：{0}", args.TemplateFileName));
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("加载模版的过程中发生错误，操作将中止。错误信息如下：{0}", ex));

                return false;
            }

            if (currentZone.Shape != null && args.ExportGeometryOfExtend == null)
            {
                landExtent = currentZone.Shape.GetEnvelope();//以当前地域范围为整个模板的范围
            }
            else if (args.ExportGeometryOfExtend != null)
            {
                landExtent = args.ExportGeometryOfExtend.GetEnvelope();
            }

            try
            {
                view.Dispatcher.Invoke(new Action(() =>
                {
                    var mainSheet = view.FindSheetByName("template");//主要的模板层，放置了主要的东西                 
                    var diagram = mainSheet.FindElementByName("title");
                    if (diagram == null)
                        this.ReportAlert(eMessageGrade.Warn, null,
                            string.Format("未在模版中找到标题，将忽略标题信息。"));
                    else
                        if (currentZone.Level == eZoneLevel.Group)
                    {
                        (diagram.Model as TextModel).Text =
                  string.Format("{0}{1}{2}农村土地承包经营权地块分布图（签章图）", allZones.Find(z => z.Level == eZoneLevel.Town).Name, allZones.Find(z => z.Level == eZoneLevel.Village).Name, allZones.Find(z => z.Level == eZoneLevel.Group).Name);
                    }
                    else if (currentZone.Level == eZoneLevel.Village)
                    {
                        (diagram.Model as TextModel).Text =
                    string.Format("{0}{1}农村土地承包经营权地块分布图（签章图）", allZones.Find(z => z.Level == eZoneLevel.Town).Name, allZones.Find(z => z.Level == eZoneLevel.Village).Name);
                    }
                }));

                view.Dispatcher.Invoke(new Action(() =>
                {
                    var mainSheet = view.FindSheetByName("template");//主要的模板层，放置了主要的东西                 
                    var diagram = mainSheet.FindElementByName("unitName");

                    if (diagram == null)
                        this.ReportAlert(eMessageGrade.Warn, null,
                            string.Format("未在模版中找到单位，将忽略单位信息。"));
                    else
                        (diagram.Model as TextModel).Text = "亩";
                }));

                bool hasMap = true;
                MapUI ui = null;
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
                    diagramMap.Model.Locked = true;

                    GroupLayer groupLayerImage = new GroupLayer();
                    groupLayerImage.Name = "影像图层";
                    GroupLayer groupLayer = new GroupLayer();
                    groupLayer.Name = "矢量图层";
                    VectorLayer zoneclayer = null;
                    VectorLayer zoneZlayer = null;
                    VectorLayer dzdwlayer = null;
                    VectorLayer mzdwlayer = null;
                    VectorLayer xzdwlayer = null;
                    foreach (var layer in listLayers)
                    {
                        var db = ProviderDbCSQLite.CreateDataSourceByFileName(args.DestinationDatabaseFileName);
                        var newLayer = layer.Clone() as VectorLayer;
                        newLayer.Labeler = null;
                        newLayer.MaximizeScale = int.MaxValue;
                        newLayer.MinimizeScale = 0;
                        var geosource = new SQLiteGeoSource(db, layer.DataSource.Schema, layer.DataSource.ElementName) { UseSpatialIndex = false };
                        newLayer.DataSource = geosource;
                       
                        if (newLayer.DataSource.ElementName == "ZD_CBD")
                        {
                            var fullextend = geosource.FullExtend;
                            if ((double.IsNaN(fullextend.MaxX) && double.IsNaN(fullextend.MaxX)
                                    && double.IsNaN(fullextend.MinX) && double.IsNaN(fullextend.MinX)) == false && currentZone.Shape == null && args.ExportGeometryOfExtend == null)
                            {
                                landExtent.Union(fullextend);
                            }//所有图层，有数据的范围相加，为出图范围，但是以地域数据为主

                            newLayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                            {
                                BackgroundColor = System.Windows.Media.Colors.Transparent,
                                BorderStrokeColor = System.Windows.Media.Colors.Black
                            });
                            groupLayer.Layers.Add(newLayer);
                            contractLandLableLayer = newLayer;
                        }
                        else if (newLayer.DataSource.ElementName == "JCSJ_XZQY" && newLayer.InternalName == "zonec_Layer")
                        {
                            zoneclayer = newLayer;
                            zoneclayer.Where = "DYJB = 2";
                            zoneclayer.Name = "村";
                            zoneclayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                            {
                                BackgroundColor = System.Windows.Media.Colors.Transparent,
                                BorderStrokeColor = System.Windows.Media.Color.FromArgb(255, 173, 0, 0),
                                BorderThickness = 1,
                            });
                            zoneclayer.Labeler = null;
                            groupLayer.Layers.Add(zoneclayer);
                        }
                        else if (newLayer.DataSource.ElementName == "JCSJ_XZQY" && newLayer.InternalName == "zoneZ_Layer")
                        {
                            zoneZlayer = newLayer;
                            zoneZlayer.Where = "DYJB = 1";
                            zoneZlayer.Name = "组";
                            zoneZlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                            {
                                BackgroundColor = System.Windows.Media.Colors.Transparent,
                                //BorderStrokeColor = System.Windows.Media.Color.FromArgb(255, 173, 53, 107),
                                BorderStrokeColor = System.Windows.Media.Color.FromArgb(255, 0, 255, 64),
                                BorderThickness = 1,
                            });
                            zoneZlayer.Labeler = null;
                            groupLayer.Layers.Add(zoneZlayer);
                        }
                        else if (newLayer.DataSource.ElementName == "DZDW" && newLayer.InternalName == "dzdw_Layer")
                        {
                            dzdwlayer = newLayer;
                            dzdwlayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                            {
                                BackgroundColor = System.Windows.Media.Colors.Black,
                                BorderStrokeColor = System.Windows.Media.Colors.Black,
                                BorderThickness = 0.5,
                            });
                            groupLayer.Layers.Add(dzdwlayer);
                        }
                        else if (newLayer.DataSource.ElementName == "XZDW" && newLayer.InternalName == "xzdw_Layer")
                        {
                            xzdwlayer = newLayer;
                            xzdwlayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                            {
                                StrokeColor = System.Windows.Media.Colors.Black,
                                StrokeThickness = 1,
                            });
                            groupLayer.Layers.Add(xzdwlayer);
                        }
                        else if (newLayer.DataSource.ElementName == "MZDW" && newLayer.InternalName == "mzdw_Layer")
                        {
                            mzdwlayer = newLayer;
                            mzdwlayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                            {
                                BackgroundColor = System.Windows.Media.Colors.Transparent,
                                BorderStrokeColor = System.Windows.Media.Colors.Black
                            });
                            groupLayer.Layers.Add(mzdwlayer);
                        }
                    }

                    ui = diagram.Content as MapUI;
                    var oldLayers = ui.MapControl.Layers.ToList();
                    ui.MapControl.Layers.Clear();
                    oldLayers.ForEach(c => c.Dispose());

                    ui.MapControl.SpatialReference = new SpatialReference(0);
                    ui.MapControl.Layers.Add(groupLayerImage);
                    ui.MapControl.Layers.Add(groupLayer);
                }));

                if (!hasMap)
                    return false;

                //VectorLayer cbdLayer = null;
                //view.Dispatcher.Invoke(new Action(() => { cbdLayer = listLayers.Where(c => c.Name == "承包地").FirstOrDefault(); }));
                            

                view.Dispatcher.Invoke(new Action(() =>
                {
                    ui.MapControl.Extend = landExtent.Clone() as Envelope;
                    (diagramMap.Model as MapModel).DefaultExtent = landExtent.Clone() as Envelope;

                    var page = new YuLinTuDiagramFoundation();
                    page.Designer = view;
                    var workpage = args.Workspace.AddWorkpage(Guid.NewGuid().ToString(), page);
                    if (workpage == null)
                        return;
                    workpage.Activate();
                }));

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("模版设置完成。"));
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("设置模版的过程中发生错误，操作将中止。错误信息如下：{0}", ex));

                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建其它标注-制图信息
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool CreateOtherDiagramsView(ElementsDesigner view)
        {
            if (view == null) return false;
            try
            {
                Element diagramMapScale = null;
                Element diagramcartographer = null;
                Element diagramcartographyDate = null;
                Element diagramcheckPerson = null;
                Element diagramcheckDate = null;
                Element diagramcartographyUnit = null;
                this.ReportProgress(95, "开始图面其它标注绘制...");
                view.Dispatcher.Invoke(new Action(() =>
                {
                    var mainSheet = view.FindSheetByName("template");//主要的模板层，放置了主要的东西
                    diagramMapScale = mainSheet.FindElementByName("MapScale");
                    diagramcartographer = mainSheet.FindElementByName("cartographer");
                    diagramcartographyDate = mainSheet.FindElementByName("cartographyDate");
                    diagramcheckDate = mainSheet.FindElementByName("checkDate");
                    diagramcheckPerson = mainSheet.FindElementByName("checkPerson");
                    diagramcartographyUnit = mainSheet.FindElementByName("cartographyUnit");

                    if (diagramMapScale != null)
                    {
                        var mstextmode = diagramMapScale.Model as TextModel;
                        if (dflcsetting.IsShowViewOfAllScale)
                        {
                            mstextmode.Text = "1:" + dflcsetting.ViewOfAllScaleWH.ToString();
                        }
                        else
                        {
                            mstextmode.Text = " ";
                        }
                    }
                    if (diagramcartographer != null)
                    {
                        var cgrtextmode = diagramcartographer.Model as TextModel;
                        cgrtextmode.Text = dflcsetting.Cartographer;
                    }
                    if (diagramcartographyDate != null)
                    {
                        var cgdtextmode = diagramcartographyDate.Model as TextModel;
                        cgdtextmode.Text = dflcsetting.CartographyDate.HasValue ? dflcsetting.CartographyDate.Value.ToString("yyyy-MM-dd") : "";
                    }

                    if (diagramcartographyUnit != null)
                    {
                        var cgutextmode = diagramcartographyUnit.Model as TextModel;
                        cgutextmode.Text = dflcsetting.CartographyUnit;
                    }

                    if (diagramcheckPerson != null)
                    {
                        var cptextmode = diagramcheckPerson.Model as TextModel;
                        cptextmode.Text = dflcsetting.CheckPerson;
                    }

                    if (diagramcheckDate != null)
                    {
                        var ckdtextmode = diagramcheckDate.Model as TextModel;
                        ckdtextmode.Text = dflcsetting.CheckDate.HasValue ? dflcsetting.CheckDate.Value.ToString("yyyy-MM-dd") : "";
                    }

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

        private bool ExportData(MapToVisiosViewTaskArgument args)
        {
            try
            {
                var db = ProviderDbCSQLite.CreateNewDatabase(args.DestinationDatabaseFileName);
                var schema = db.CreateSchema();

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("新数据库创建成功：{0}", args.DestinationDatabaseFileName));

                List<VectorLayer> layers = null;
                args.MapControl.Dispatcher.Invoke(new Action(() =>
                {
                    layers = args.MapControl.Layers.GetSubLayers().
                        Where(c => c.GetType() == typeof(VectorLayer) && ((c as VectorLayer).DataSource.ElementName == "ZD_CBD"
                                            || c.InternalName == "zoneZ_Layer" || c.InternalName == "zonec_Layer"
                                            || c.InternalName == "dzdw_Layer" || c.InternalName == "xzdw_Layer"
                                            || c.InternalName == "mzdw_Layer")).
                        Select(c => c as VectorLayer).
                        ToList();
                }));

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("共有 {0} 个图层需要导出", layers.Count));

                double percentLayer = (double)85 / layers.Count;
                listLayers = new List<VectorLayer>();

                var geoshape = currentZone.Shape;
                for (int i = 0; i < layers.Count; i++)
                {
                    var layer = layers[i];
                    double percentCurrent = 5 + percentLayer * i;

                    string name = null;
                    args.MapControl.Dispatcher.Invoke(new Action(() => { name = layer.Name; }));

                    if ((geoshape != null || (geoshape == null && args.ExportGeometryOfExtend != null)) && ExportLayerHasZoneShape(args, layer, db, percentCurrent, percentLayer))
                    {
                        listLayers.Add(layer);
                    }
                    else if (geoshape == null && args.ExportGeometryOfExtend == null && ExportLayerNoZoneShape(args, layer, db, percentCurrent, percentLayer))
                    {
                        listLayers.Add(layer);
                    }
                }

            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("导出数据的过程中发生错误，操作将中止。错误信息如下：{0}", ex));

                return false;
            }

            return true;
        }

        //导出数据到临时数据库-如果有行政地域空间数据
        private bool ExportLayerHasZoneShape(MapToVisiosViewTaskArgument args,
            VectorLayer layerSource, Data.IDataSource dbTarget, double percentCurrent, double percentLayer)
        {
            string name = null;
            args.MapControl.Dispatcher.Invoke(new Action(() => { name = layerSource.Name; }));
            var entityType = layerSource.DataSource.GetElementType();
            var elementName = layerSource.DataSource.ElementName;
            var layerDataCount = layerSource.DataSource.Count("Shape != null");
            Geometry geo = args.ExportGeometryOfExtend;//数据范围
            var geoshape = currentZone.Shape;
            if (geo == null && geoshape != null)
            {
                //当前地域内
                geo = currentZone.Shape.GetEnvelope().ToGeometry();
            }
            else if (geo == null && geoshape == null)
            {
                return false;
            }

            try
            {
                var schemaTarget = dbTarget.CreateSchema();
                schemaTarget.Export(entityType, layerSource.DataSource.SpatialReference.WKID, false);

                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("{0}的图层表单 {1} 创建成功。", name, elementName));
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("创建图层表单 {0} 的过程中发生错误，该图层将被忽略。错误信息如下：{1}", name, ex));

                return false;
            }

            bool sqliteLockWhenForeachCallback = true;
            bool? sqliteUseSpatialIndex = null;
            var sqlite = layerSource.DataSource as SQLiteGeoSource;
            if (sqlite != null)
            {
                sqliteLockWhenForeachCallback = sqlite.LockWhenForeachCallback;
                sqlite.LockWhenForeachCallback = true;
                sqliteUseSpatialIndex = sqlite.UseSpatialIndex;
                sqlite.UseSpatialIndex = false;
            }

            try
            {
                (dbTarget as IDbContext).BeginTransaction();

                this.ReportProgress((int)percentCurrent, string.Format("开始导出图层 {0} ...", name));
                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("开始导出图层 {0} ...", name));

                int total = 0;
                var dqTarget = new DynamicQuery(dbTarget);
                layerSource.DataSource.ForEachByIntersects(geo, (index, cnt, obj) =>
                {
                    total++;

                    if (elementName == "ZD_CBD")
                    {
                        var zldm = obj.Object.GetPropertyValue<string>("ZLDM");
                        if (string.IsNullOrEmpty(zldm))
                        {
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                        else if (zldm.StartsWith(currentZone.FullCode))
                        {
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                    }
                    else if (elementName == "JCSJ_XZQY")
                    {
                        var zldm = obj.Object.GetPropertyValue<string>("DYQBM");
                        if (zldm.IsNullOrEmpty() == false && zldm.Length > 9)//只要村级及组级的地域界限
                        {
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                    }
                    else if (elementName == "DZDW" || elementName == "XZDW" || elementName == "MZDW")
                    {
                        var zonecode = obj.Object.GetPropertyValue<string>("zonecode");
                        if (string.IsNullOrEmpty(zonecode))
                        {
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                        else if (zonecode.StartsWith(currentZone.FullCode))
                        {
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                    }
                    return true;
                });

                (dbTarget as IDbContext).CommitTransaction();

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("共导出了 {0} 条数据。", total));
            }
            catch (Exception ex)
            {
                (dbTarget as IDbContext).RollbackTransaction();

                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("导出图层 {0} 的过程中发生错误，该图层将被忽略。错误信息如下：{1}", name, ex));

                return false;
            }
            finally
            {
                if (sqlite != null)
                    sqlite.LockWhenForeachCallback = sqliteLockWhenForeachCallback;
                if (sqlite != null)
                    sqlite.UseSpatialIndex = sqliteUseSpatialIndex;

            }

            return true;
        }

        //导出数据到临时数据库-如果没有行政地域空间数据
        private bool ExportLayerNoZoneShape(MapToVisiosViewTaskArgument args,
            VectorLayer layerSource, Data.IDataSource dbTarget, double percentCurrent, double percentLayer)
        {
            string name = null;
            args.MapControl.Dispatcher.Invoke(new Action(() => { name = layerSource.Name; }));
            var entityType = layerSource.DataSource.GetElementType();
            var elementName = layerSource.DataSource.ElementName;
            var layerDataCount = layerSource.DataSource.Count("Shape != null");

            try
            {
                var schemaTarget = dbTarget.CreateSchema();
                schemaTarget.Export(entityType, layerSource.DataSource.SpatialReference.WKID, false);

                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("{0}的图层表单 {1} 创建成功。", name, elementName));
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("创建图层表单 {0} 的过程中发生错误，该图层将被忽略。错误信息如下：{1}", name, ex));

                return false;
            }

            bool sqliteLockWhenForeachCallback = true;
            bool? sqliteUseSpatialIndex = null;
            var sqlite = layerSource.DataSource as SQLiteGeoSource;
            if (sqlite != null)
            {
                sqliteLockWhenForeachCallback = sqlite.LockWhenForeachCallback;
                sqlite.LockWhenForeachCallback = true;
                sqliteUseSpatialIndex = sqlite.UseSpatialIndex;
                sqlite.UseSpatialIndex = false;
            }

            try
            {
                (dbTarget as IDbContext).BeginTransaction();

                this.ReportProgress((int)percentCurrent, string.Format("开始导出图层 {0} ...", name));
                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("开始导出图层 {0} ...", name));

                int total = 0;
                var dqTarget = new DynamicQuery(dbTarget);

                if (elementName == "ZD_CBD")
                {
                    layerSource.DataSource.ForEach("ZLDM==\"" + currentZone.FullCode + "\"", null, null, (index, cnt, obj) =>
                     {
                         var zldm = obj.Object.GetPropertyValue<string>("ZLDM");
                         if (string.IsNullOrEmpty(zldm))
                         {
                             total++;
                             dqTarget.Add(null, elementName, obj.Object);
                             return true;
                         }
                         else if (zldm.StartsWith(currentZone.FullCode))
                         {
                             total++;
                             dqTarget.Add(null, elementName, obj.Object);
                             return true;
                         }
                         return true;
                     });
                }

                if (elementName == "JCSJ_XZQY")
                {
                    string zzonefullcode = string.Empty;
                    if (currentZone.Level <= eZoneLevel.Village)
                    {
                        zzonefullcode = currentZone.FullCode.Substring(0, 9);
                        var csql = string.Format("DYQBM.StartsWith(\"{0}\")", zzonefullcode);
                        layerSource.DataSource.ForEach(csql, null, null, (index, cnt, obj) =>
                        {
                            total++;

                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        });
                    }
                }
                if (elementName == "DZDW" || elementName == "XZDW" || elementName == "MZDW")
                {
                    layerSource.DataSource.ForEach("zonecode==\"" + currentZone.FullCode + "\"", null, null, (index, cnt, obj) =>
                    {
                        var zonecode = obj.Object.GetPropertyValue<string>("zonecode");
                        if (string.IsNullOrEmpty(zonecode))
                        {
                            total++;
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                        else if (zonecode.StartsWith(currentZone.FullCode))
                        {
                            total++;
                            dqTarget.Add(null, elementName, obj.Object);
                            return true;
                        }
                        return true;
                    });
                }

                (dbTarget as IDbContext).CommitTransaction();

                this.ReportAlert(eMessageGrade.Infomation, null,
                    string.Format("共导出了 {0} 条数据。", total));
            }
            catch (Exception ex)
            {
                (dbTarget as IDbContext).RollbackTransaction();

                this.ReportAlert(eMessageGrade.Error, null,
                    string.Format("导出图层 {0} 的过程中发生错误，该图层将被忽略。错误信息如下：{1}", name, ex));

                return false;
            }
            finally
            {
                if (sqlite != null)
                    sqlite.LockWhenForeachCallback = sqliteLockWhenForeachCallback;
                if (sqlite != null)
                    sqlite.UseSpatialIndex = sqliteUseSpatialIndex;

            }

            return true;
        }


        private bool ValidateArgument(MapToVisiosViewTaskArgument args)
        {
            if (currentZone.Level > eZoneLevel.Village)
            {
                this.ReportAlert(eMessageGrade.Error, null, "行政地域不能大于村级。");
                return false;
            }

            if (currentZone.Shape == null)
            {
                //this.ReportAlert(eMessageGrade.Error, null, "行政地域空间数据不能为空。");
                this.ReportWarn("当前行政地域空间数据不能为空,否则制图将有可能不完整。");
                //return false;
            }

            if (args == null)
            {
                this.ReportAlert(eMessageGrade.Error, null, "参数不能为空。");
                return false;
            }

            args.DestinationDatabaseFileName = System.IO.Path.Combine(
                TheApp.Current.GetDataPath(), "Diagrams", Guid.NewGuid().ToString(), "db.sqlite");

            if (args.DestinationDatabaseFileName.IsNullOrBlank())
            {
                this.ReportAlert(eMessageGrade.Error, null, "目标数据库路径不能为空。");
                return false;
            }

            if (args.TemplateFileName.IsNullOrBlank())
            {
                this.ReportAlert(eMessageGrade.Error, null, "制图模板路径不能为空。");
                return false;
            }

            if (args.MapControl == null)
            {
                this.ReportAlert(eMessageGrade.Error, null, "地图控件不能为空。");
                return false;
            }

            List<Layer> layers = null;
            args.MapControl.Dispatcher.Invoke(new Action(() => { layers = args.MapControl.Layers.GetSubLayers(); }));

            if (!layers.Any(c => c.GetType() == typeof(VectorLayer)))
            {
                this.ReportAlert(eMessageGrade.Warn, null, "地图中没有任何可以用作制图的图层，操作将中止。");
                return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
