using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Data;
using YuLinTu.Visio.Designer;
using YuLinTu.Visio.Designer.Elements;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// 签章表标注任务
    /// </summary>
    public class VisioAddContractorNameTableTask : Task
    {
        #region Fields

        private VectorLayer contractLandLableLayer = null;//用于标注的承包地图层

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            var args = Argument as VisioAddContractorNameTableArgument;

            this.ReportProgress(0, "开始");
            this.ReportAlert(eMessageGrade.Infomation, null, "开始图面标注...");
            if (!CreateDiagramsView(args))
                return;

            this.ReportProgress(100, "图面标注完成");
            this.ReportAlert(eMessageGrade.Infomation, null, "图面标注完成");
        }

        #endregion

        #region Methods - Private

        private bool CreateDiagramsView(VisioAddContractorNameTableArgument args)
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
                    contractLandLableLayer = ui.MapControl.Layers.GetSubLayers().Find(l => l.Name == "承包地") as VectorLayer;
                    if (contractLandLableLayer == null)
                    {
                        hasCBDlayer = false;
                        return;
                    }
                }));

                if (!hasMap)
                    return false;
                if (!hasCBDlayer)
                    return false;
                this.ReportProgress(20, "开始制表...");

                //当前地域下点集合
                var currentZoneGeos = contractLandLableLayer.DataSource.GetAll();
                if (currentZoneGeos == null || currentZoneGeos.Count == 0)
                {
                    this.ReportAlert(eMessageGrade.Warn, null,
                        string.Format("未在模版中找到地块数据，操作将中止。"));
                    this.ReportProgress(100);
                    return false;
                }
                //承包方名称集合,去掉重复的人名
                List<string> contractorNames = new List<string>();
                foreach (var geoitem in currentZoneGeos)
                {
                    var contractorObject = geoitem.Object.GetPropertyValue("QLRMC");
                    if (contractorObject==null) continue;
                    var contractorName = contractorObject.ToString();
                    if (contractorName.IsNullOrEmpty()) continue;
                    if (!contractorNames.Contains(contractorName))
                    {
                        contractorNames.Add(contractorName);
                    }
                }
                //计算行数
                int tablerowCountMin = contractorNames.Count() / args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox;//取整               
                int tablerowCountYS = contractorNames.Count() % args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox;//取余数
                int tablerowCount = 0;//总行数
                if (tablerowCountYS != 0)
                {
                    //有余数则多加一行
                    tablerowCount = tablerowCountMin + 1;
                }
                else
                {
                    tablerowCount = tablerowCountMin;
                }               

                //添加图表和标题
                view.Dispatcher.Invoke(new Action(() =>
                {
                    ElementsSheet cbfTBSheet = new ElementsSheet();
                    cbfTBSheet.Name = "cbfTBSheet";
                    cbfTBSheet.AliasName = "承包方签章表层";
                    cbfTBSheet.Description = "模板中的承包方签章表都在这层";

                    System.Windows.Media.SolidColorBrush ForegroundSCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTableLabelColor)));
                    System.Windows.Media.SolidColorBrush BorderBrushSCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTableBorderColor)));
                    
                    var diagram = new DataGridModel
                    {
                        FontSize = args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTableLabelSize,
                        Foreground = ForegroundSCB,
                        BorderBrush = BorderBrushSCB,
                        Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent),
                        RowCount = tablerowCount + 1,
                        ColumnCount = 3 * args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox,
                        RowHeight = args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTableCellHeightSize,
                        ColumnWidth = args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTableCellWidthSize,
                    }.CreateElement();

                    diagram.Model.Width = (3 * args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox) * 50+10;
                    diagram.Model.Height = (tablerowCount + 1) * 20;
                    diagram.Model.Name = "contractorSignatureStampTable";
                    //diagram.Model.X = args.locationX - (3 * args.VisioMapLayoutSetInfo.QZBHNumBox) * 50;
                    //diagram.Model.Y = args.locationY + 50 - (tablerowCount + 1) * 25;

                    diagram.Model.X = args.locationX;
                    diagram.Model.Y = args.locationY;

                    var table = new DataTable();

                    for (int i = 0; i < (diagram.Model as DataGridModel).ColumnCount; i++)
                        table.Columns.Add();

                    //排头
                    List<object> list = new List<object>();
                    for (int j = 0; j < args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox; j++)
                    {
                        list.Add("承包方编号");
                        list.Add("承包方姓名");
                        list.Add("签字按印");
                    }
                    table.Rows.Add(list.ToArray());

                    //插值  
                    int sortNum = 0;
                    int tableRowCount = (diagram.Model as DataGridModel).RowCount - 1;

                    List<List<List<object>>> valueAlist = new List<List<List<object>>>();
                    List<List<object>> valueClist;
                    List<object> valuelist;
                    for (int j = 0; j < args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox; j++)
                    {
                        valueClist = new List<List<object>>();//每一个大栏集合行
                        for (int i = 0; i < tableRowCount; i++)
                        {
                            valuelist = new List<object>();//每一行

                            if (sortNum <= contractorNames.Count - 1)
                            {
                                valuelist.Add((sortNum + 1).ToString());
                                valuelist.Add(contractorNames[sortNum]);
                                valuelist.Add("");
                            }
                            else
                            {
                                valuelist.Add(" ");
                                valuelist.Add(" ");
                                valuelist.Add(" ");
                            }
                            ++sortNum;
                            valueClist.Add(valuelist);
                        }
                        valueAlist.Add(valueClist);
                    }
                    List<object> tablevaluelist;
                    for (int i = 0; i < tableRowCount; i++)
                    {
                        tablevaluelist = new List<object>();
                        for (int ii = 0; ii < args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox; ii++)
                        {
                            tablevaluelist.AddRange(valueAlist[ii][i]);
                        }
                        table.Rows.Add(tablevaluelist.ToArray());
                    }

                    var uii = diagram.Content as DataGridUI;
                    uii.SetDataTable(table);
                    cbfTBSheet.Items.Add(diagram);
                    System.Windows.Media.SolidColorBrush titlelabelForegroundSCB = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)(System.Windows.Media.ColorConverter.ConvertFromString(args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTitleFontColor)));
                    //添加签字表名称
                    var label = new TextModel().CreateElement();
                    (label.Model as TextModel).Text = "签字按印区";
                    label.Model.Name = "contractorSignatureStampTitle";
                    (label.Model as TextModel).Foreground = titlelabelForegroundSCB;
                    (label.Model as TextModel).FontSize = args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTitleLabelSize;
                    //var size = new Size(287, 57);
                    var size = new Size(args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTitleLabelSize * 10, args.VisioMapLayoutSetInfo.QzbSetInfo.QZBTitleLabelSize * 4);
                    //label.Model.X = (3 * args.VisioMapLayoutSetInfo.QZBHNumBox) * 40 + args.locationX - (3 * args.VisioMapLayoutSetInfo.QZBHNumBox) * 50;
                    //label.Model.Y = args.locationY + 10 - (tablerowCount + 1) * 25;

                    label.Model.X = (3 * args.VisioMapLayoutSetInfo.QzbSetInfo.QZBHNumBox) * 4 + args.locationX+10;
                    label.Model.Y = args.locationY - 47;

                    label.Model.Width = size.Width;
                    label.Model.Height = size.Height;
                    label.Model.BorderWidth = 0;

                    cbfTBSheet.Items.Add(label);

                    view.Sheets.Add(cbfTBSheet);
                    cbfTBSheet.Refresh();
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

        #endregion
    }
}
