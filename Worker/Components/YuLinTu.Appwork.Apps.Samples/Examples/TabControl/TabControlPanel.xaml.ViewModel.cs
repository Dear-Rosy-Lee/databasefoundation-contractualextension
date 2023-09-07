using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(TabControlPanel))]
    public partial class TabControlPanelViewModel : ViewModelObject
    {
        #region Properties

        #region Properties - Common

        public TabControlConfiguration Config
        {
            get { return _Config; }
            set { _Config = value; NotifyPropertyChanged(() => Config); }
        }
        private TabControlConfiguration _Config = new TabControlConfiguration();

        public System.Collections.ObjectModel.ObservableCollection<string> Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; NotifyPropertyChanged(() => Tabs); }
        }
        private System.Collections.ObjectModel.ObservableCollection<string> _Tabs = new System.Collections.ObjectModel.ObservableCollection<string>() { };


        #endregion

        #region Properties - System

        #endregion

        #endregion

        #region Commands

        #region Commands - AddTab

        public DelegateCommand CommandAddTab { get { return _CommandAddTab ?? (_CommandAddTab = new DelegateCommand(args => OnAddTab(args), args => OnCanAddTab(args))); } }
        private DelegateCommand _CommandAddTab;

        private bool OnCanAddTab(object args)
        {
            return true;
        }

        private void OnAddTab(object args)
        {
            Tabs.Add($"Tab{++tabCounter}");
            Config.SelectedTabIndex = Tabs.Count - 1;
        }

        #endregion

        #region Commands - Insert

        public DelegateCommand CommandInsert { get { return _CommandInsert ?? (_CommandInsert = new DelegateCommand(args => OnInsert(args), args => OnCanInsert(args))); } }
        private DelegateCommand _CommandInsert;

        private bool OnCanInsert(object args)
        {
            return true;
        }

        private void OnInsert(object args)
        {
            var index = Config.SelectedTabIndex < 0 ? 0 : Config.SelectedTabIndex;
            Tabs.Insert(index, $"Tab{++tabCounter}");
            Config.SelectedTabIndex = index;
        }

        #endregion


        #region Commands - RemoveTab

        public DelegateCommand CommandRemoveTab { get { return _CommandRemoveTab ?? (_CommandRemoveTab = new DelegateCommand(args => OnRemoveTab(args), args => OnCanRemoveTab(args))); } }
        private DelegateCommand _CommandRemoveTab;

        private bool OnCanRemoveTab(object args)
        {
            return Config.SelectedTabIndex >= 0;
        }

        private void OnRemoveTab(object args)
        {
            Tabs.RemoveAt(Config.SelectedTabIndex);
        }

        #endregion

        #region Commands - ClearTab

        public DelegateCommand CommandClearTab { get { return _CommandClearTab ?? (_CommandClearTab = new DelegateCommand(args => OnClearTab(args), args => OnCanClearTab(args))); } }
        private DelegateCommand _CommandClearTab;

        private bool OnCanClearTab(object args)
        {
            return Tabs.Count > 0;
        }

        private void OnClearTab(object args)
        {
            Tabs.Clear();
        }

        #endregion

        #region Fields

        private int tabCounter = 0;

        #endregion

        #endregion

        #region Ctor

        public TabControlPanelViewModel()
        {
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        #endregion

        #region Methods - System

        #endregion

        #endregion
    }
}
