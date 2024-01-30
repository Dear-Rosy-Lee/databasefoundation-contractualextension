using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.tGIS.Client;
using YuLinTu.Spatial;
using System.Windows.Data;
using System.Threading;
using System.Collections.Specialized;
using Xceed.Wpf.Toolkit;
using YuLinTu.Components.tGIS;

namespace YuLinTu.Component.MapFoundation
{
    public class MapPageContextsTopology : MapPageContextBase
    {
        public MapPageContextsTopology(IWorkpage workpage)
          : base(workpage)
        {
        }

        [MessageHandler(ID = EditGISClient.tGIS_LandCodeEdit_Geometry_Begin)]
        private void tGIS_SplitLand_Geometry_Install(object sender, MessageUnionGeometryInstallEventArgs e)
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
                    map.Dispatcher.Invoke(new Action(() => index = dlg.dg.SelectedIndex));

                    var editor = new GeometryTopologyUnion(map, e.Layer, e.Graphics, index);
                    var graphic = editor.Do() as Graphic;
                    var listDeletes = e.Graphics.Where(c => c != graphic).ToList();

                    var args = new InstallUnionGeometryResultEventArgs(e.Layer, listDeletes, graphic);
                    Workpage.Message.Send(this, args);
                    if (args.IsCancel)
                        return;

                    map.Dispatcher.Invoke(new Action(() =>
                    {
                        //map.SaveAsync(null, new Graphic[] { graphic }, listDeletes.ToArray(), () =>
                        //{
                        //    e.Layer.Refresh();
                        //    map.SelectedItems.Clear();
                        //    map.SelectedItems.Add(graphic);
                        //    are.Set();
                        //}, error =>
                        //{
                        //    e.Parameter = false;
                        //    are.Set();

                        //    Workpage.Page.ShowDialog(new MessageDialog()
                        //    {
                        //        MessageGrade = eMessageGrade.Error,
                        //        Message = "分割图形的过程中发生了一个未知错误。",
                        //        Header = "分割"
                        //    });
                        //});
                    }));

                    are.WaitOne();
                };

                Workpage.Page.ShowDialog(dlg, (b, r) => { dlg.Uninstall(); });
            }));
        }
    }
}