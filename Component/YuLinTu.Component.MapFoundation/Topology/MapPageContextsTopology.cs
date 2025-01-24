using System;
using System.Linq;
using System.Windows.Media;
using YuLinTu.Windows;
using YuLinTu.tGIS.Client;
using System.Threading;
using YuLinTu.Components.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using System.Collections.Generic;
using OSGeo.OGR;
using YuLinTu.Data;

namespace YuLinTu.Component.MapFoundation
{
    public class MapPageContextsTopology : MapPageContextBase
    {
        public MapPageContextsTopology(IWorkpage workpage)
          : base(workpage)
        {
        }

        [MessageHandler(ID = EditGISClient.tGIS_LandCodeEdit_Geometry_Begin)]
        private void tGIS_SplitLand_Geometry_Install(object sender, MessageSplitLandInstallEventArgs e)
        {
            e.IsCancel = true;

            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                var map = MapControl;
                var dlg = new SplitLandEdit(MapControl, e.Layer, e.Graphics, e.DbContext, e.Zone);
                dlg.Background = Brushes.Transparent;

                dlg.Confirm += (s, a) =>
                {
                    var are = new AutoResetEvent(false);
                    var index = 0;

                    map.Dispatcher.Invoke(new Action(() =>
                    {
                        var db = e.DbContext;
                        AccountLandBusiness landBus = new AccountLandBusiness(db);
                        var landStation = db.CreateContractLandWorkstation();
                        var flag = bool.Parse(s.GetPropertyValue("Flag").ToString());
                        var items = s.GetPropertyValue("SplitItems") as List<SplitItem>;
                        var oldlandNumber = s.GetPropertyValue("SelectOldLandNumber") as string;
                        var entities = new List<ContractLand>();
                        items.ForEach(x =>
                        {
                            //var landId = Guid.Parse(x.Graphic.Object.Object.GetPropertyValue("ID").ToString());
                            //var entity = landBus.GetLandById(landId);
                            entities.Add(x.Land);
                        });


                        if (flag == true)
                        {
                            if (string.IsNullOrEmpty(oldlandNumber))
                                oldlandNumber = entities[0].ZoneCode.PadRight(14, '0') + items[0].OldNumber;
                            bool deloldLand = true;
                            for (int i = 0; i < items.Count; i++)
                            {
                                entities[i].LandNumber = entities[i].ZoneCode.PadRight(14, '0') + items[i].SurveyNumber.PadLeft(5, '0');
                                if (entities[i].AwareArea == 0)
                                    entities[i].AwareArea = entities[i].ActualArea;
                                if (oldlandNumber == entities[i].LandNumber)
                                {
                                    deloldLand = false;
                                }
                                var dbland = landStation.GetByLandNumber(entities[i].LandNumber);// (l => l.ID == entities[i].ID)
                                if (dbland != null)
                                {
                                    entities[i].ID = dbland.ID;
                                    landStation.Update(entities[i]);
                                }
                                else
                                {
                                    landStation.Add(entities[i]);
                                }
                            }
                            if (deloldLand)
                                landStation.Delete(d => d.LandNumber == oldlandNumber);
                        }
                        else
                        {
                            var landCode = s.GetPropertyValue("LandCode").ToString();
                            map.Dispatcher.Invoke(new Action(() => index = dlg.dg.SelectedIndex));
                            var editor = new GeometryTopologyUnion(map, e.Layer, e.Graphics, index);
                            var graphicNew = editor.Do() as Graphic;
                            var graphicOld = items.Where(x => x.Graphic != graphicNew).FirstOrDefault().Graphic;
                            var landNewId = Guid.Parse(graphicNew.Object.Object.GetPropertyValue("ID").ToString());
                            var landOldId = Guid.Parse(graphicOld.Object.Object.GetPropertyValue("ID").ToString());
                            var entityNew = landBus.GetLandById(landNewId);
                            var entityOld = landBus.GetLandById(landOldId);
                            entityOld.LandNumber = entityNew.LandNumber;
                            entityNew.LandNumber = entityNew.ZoneCode + landCode;
                            landStation.Update(entityOld);
                            landStation.Update(entityNew);
                        }
                        //map.Refresh();
                        var args = new MapMessageEventArgs("RefreshMapContrl_UIdata");
                        map.Message.Send(this, args);
                    }));
                };
                Workpage.Page.ShowDialog(dlg, (b, r) => { dlg.Uninstall(); });
            }));
        }
    }
}