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

namespace YuLinTu.Appwork.Apps.Samples
{
    [View(typeof(ThemeColorViewerPanel))]
    public partial class ThemeColorViewerPanelViewModel : ViewModelObject
    {
        #region Properties

        #region Properties - Common

        public ThemeColorViewerItem SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(() => SelectedItem); }
        }
        private ThemeColorViewerItem _SelectedItem;

        public System.Collections.ObjectModel.ObservableCollection<ThemeColorViewerItem> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private System.Collections.ObjectModel.ObservableCollection<ThemeColorViewerItem> _Items = new System.Collections.ObjectModel.ObservableCollection<ThemeColorViewerItem>()
        {
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.TransparentBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.TransparentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.TransparentBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.TransparentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BorderBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BorderBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundLighterLighterBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BorderBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BackgroundBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundLighterLighterBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemHoverBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemHoverBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemHoverBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ItemActiveBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeHoverBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeHoverBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeHoverBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeActiveBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BackgroundDarkerBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.BackgroundDarkerBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundLighterLighterBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundLighterLighterBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundLighterBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ForegroundBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.InformationBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.InformationBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.WarnBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.WarnBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ErrorBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ErrorBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ExceptionBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ExceptionBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.SuccessBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ContentBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.SuccessBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.InformationBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.InformationBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.WarnBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.WarnBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ErrorBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ErrorBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ExceptionBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ExceptionBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
            new ThemeColorViewerItem()
            {
                BorderBrushKey = ThemeColorItem.GetByName(nameof(ResourceKeys.SuccessBrushKey)),
                BackgroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.SuccessBrushKey)),
                ForegroundKey = ThemeColorItem.GetByName(nameof(ResourceKeys.ThemeForegroundBrushKey)),
            },
        };

        #endregion

        #region Properties - System

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public ThemeColorViewerPanelViewModel()
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
