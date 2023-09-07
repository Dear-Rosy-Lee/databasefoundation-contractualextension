using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Data;

namespace YuLinTu.Component.MapFoundation
{
    public class TopologyCommands
    {
        #region TopologyZoneLandCommand

        public static RoutedCommand TopologyZoneLandCommandCommand
        {
            get
            {
                if (cmdTopologyZoneLandCommand == null)
                    cmdTopologyZoneLandCommand = new RoutedCommand("TopologyZoneLandCommand", typeof(TopologyCommands));
                return cmdTopologyZoneLandCommand;
            }
        }
        private static RoutedCommand cmdTopologyZoneLandCommand;

        public static CommandBinding TopologyZoneLandCommandCommandBinding
        {
            get
            {
                if (cmdBindingTopologyZoneLandCommand == null)
                    cmdBindingTopologyZoneLandCommand = new CommandBinding(TopologyZoneLandCommandCommand, OnTopologyZoneLandCommandCommandExecuted, OnTopologyZoneLandCommandCommandCanExecuted);
                return cmdBindingTopologyZoneLandCommand;
            }
        }
        private static CommandBinding cmdBindingTopologyZoneLandCommand;

        private static void OnTopologyZoneLandCommandCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var c = e.Parameter as MapPageContextTopology;
            if (c == null)
                throw new ArgumentNullException("CommandParameter");

            var codeZonePage = c.Workpage.Properties.TryGetValue<string>("CurrentZoneCode", null);
            var zone = c.Workpage.Properties.TryGetValue<Library.Entity.Zone>("CurrentZone", null);
            var root = new TaskGroup();
            root.Argument = new TaskArgument();

            var dlg = new LandTopologyDialog();
            dlg.Args.ZoneCode = codeZonePage;
            dlg.Args.ZoneFullName = zone.FullName;
            dlg.Args.DataSource = DataSource.Create(TheBns.Current.GetDataSourceName());
            dlg.Args.LayerName = "承包地";

            dlg.ConfirmStart += (s, a) =>
            {
                root.Name = "拓扑检查";
                root.Description = string.Format("对地域编码为 {0} 的地域进行拓扑检查", codeZonePage);

                root.Add(dlg.CreateTask());
            };
            dlg.ConfirmCompleted += (s, a) =>
            {
                c.Workpage.TaskCenter.Add(root);
                root.StartAsync();
                c.Workpage.Message.Send(dlg, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };

            c.Workpage.Page.ShowDialog(dlg);
        }

        private static void OnTopologyZoneLandCommandCommandCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            var c = e.Parameter as MapPageContextTopology;
            if (c == null)
                throw new ArgumentNullException("CommandParameter");

            var codeZonePage = c.Workpage.Properties.TryGetValue<string>("CurrentZoneCode", null);
            e.CanExecute = !(codeZonePage.IsNullOrBlank() || codeZonePage == "86");
        }

        #endregion
    }
}
