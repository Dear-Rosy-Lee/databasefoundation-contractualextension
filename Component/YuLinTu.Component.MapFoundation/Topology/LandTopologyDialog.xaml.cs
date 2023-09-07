using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public partial class LandTopologyDialog : Dialog
    {
        #region Properties

        public LandTopologyTaskArgument Args { get; private set; }

        #endregion

        #region Fields


        #endregion

        #region Ctor

        public LandTopologyDialog()
        {
            InitializeComponent();

            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<LandTopologyTaskArgument>();
            Args = profile.GetSection<LandTopologyTaskArgument>().Settings;

            DataContext = Args;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public TaskGroup CreateTask()
        {
            var task = new LandTopologyTaskGroup();
            task.Argument = Args;
            task.Name = "承包地";
            task.Description = "对承包地进行拓扑检查";

            TryCreateOverlaps(task);
            TryCreateNodeRepeat(task);
            TryCreateArea(task);
            TryCreateAngle(task);
            TryCreateSelfOverlaps(task);
            TryCreateLandNearNodeRepeat(task);
            TryCreateNodeOnEdge(task);

            return task;
        }

        #endregion

        #region Methods - Override

        protected override bool OnConfirmGo()
        {
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            center.Save<LandTopologyTaskArgument>();

            return true;
        }

        protected override void OnConfirmStarted()
        {
        }

        #endregion

        #region Methods - Private

        private void TryCreateOverlaps(TaskGroup root)
        {
            if (!Args.DoOverlaps)
                return;

            var task = new LandOverlapsTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateNodeRepeat(TaskGroup root)
        {
            if (!Args.DoNodeRepeat)
                return;

            var task = new LandNodeRepeatTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateArea(TaskGroup root)
        {
            if (!Args.DoArea)
                return;

            var task = new LandAreaTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateAngle(TaskGroup root)
        {
            if (!Args.DoAngle)
                return;

            var task = new LandAngleTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateSelfOverlaps(TaskGroup root)
        {
            if (!Args.DoSelfOverlaps)
                return;

            var task = new LandSelfOverlapsTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateLandNearNodeRepeat(TaskGroup root)
        {
            if (!Args.DoNearNodeRepeat)
                return;

            var task = new LandNearNodeRepeatTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        private void TryCreateNodeOnEdge(TaskGroup root)
        {
            if (!Args.DoNodeOnEdge)
                return;

            var task = new LandNodeOnEdgeTopologyTask();
            task.Argument = Args;

            root.Add(task);
        }

        #endregion

        #endregion
    }
}
