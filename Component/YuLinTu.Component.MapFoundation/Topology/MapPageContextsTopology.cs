using System;
using System.Linq;
using System.Windows.Media;
using YuLinTu.Windows;
using YuLinTu.tGIS.Client;
using System.Threading;
using YuLinTu.Components.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

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
                var dlg = new SplitLandEdit(MapControl, e.Layer, e.Graphics);
                dlg.Background = Brushes.Transparent;
                dlg.Confirm += (s, a) =>
                {
                    var are = new AutoResetEvent(false);

                    var index = 0;
                    map.Dispatcher.Invoke(new Action(() =>
                    {
                        //var db = e.DbContext;
                        //AccountLandBusiness landBus = new AccountLandBusiness(db);
                        //var landStation = db.CreateContractLandWorkstation();
                        //index = dlg.dg.SelectedIndex;
                        //var editor = new GeometryTopologyUnion(map, e.Layer, e.Graphics, index);
                        //var graphic1 = editor.Do() as Graphic;
                        //var landid1 = graphic1.Object.Object.GetPropertyValue("ID");
                        //Guid landId1 = Guid.Parse(landid1.ToString());
                        //var graphic2 = e.Graphics.Where(c => c != graphic1).FirstOrDefault();
                        //var land2 = graphic2.Object.Object.GetPropertyValue("ID");
                        //Guid landId2 = Guid.Parse(landid1.ToString());
                        //var entity1 = landBus.GetLandById(landId1);
                        //var entity2 = landBus.GetLandById(landId2);
                        //var landCode1 = graphic1.Object.Object.GetPropertyValue("DKBM");
                        //var landCode2 = graphic1.Object.Object.GetPropertyValue("DKBM");
                    }));
                };

                Workpage.Page.ShowDialog(dlg, (b, r) => { dlg.Uninstall(); });
            }));
        }
    }
}