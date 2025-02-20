/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Spatial;
using YuLinTu.Appwork;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.tGIS.Client;
using YuLinTu.Components.tGIS;
using Xceed.Wpf.Toolkit;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS.Data;
using YuLinTu;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Repository;
using System.Threading;
using YuLinTu.Data.Dynamic;
using System.Windows.Input;
using System.Collections;
using YuLinTu.Data.Shapefile;
using YuLinTu.Diagrams;

namespace YuLinTu.Component.DiagramFoundation
{
    internal class MapPageContext : MapPageContextBase
    {
        #region Fields

        //private MetroButton btnToDiagramsView = null;
        private InvestigationVisioMapPanel visioMapPanel = null;
        #endregion

        #region Ctor

        public MapPageContext(IWorkpage workpage)
            : base(workpage)
        {
            visioMapPanel = new InvestigationVisioMapPanel();
        }

        #endregion

        #region Methods

        #region Methods - Message Handler

        /// <summary>
        /// 初始化添加出图设置模板
        /// </summary>        
        protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        {
            if (Navigator == null)
            {
                return;
            }
            Navigator.RootItemAutoExpand = false;

            visioMapPanel.TheWorkPage = Workpage;
            visioMapPanel.CurrentMapControl = MapControl;

            var binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToSelectPolyGonButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            visioMapPanel.mtbPolyGonSelectExport.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToSelectRectangleButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            visioMapPanel.mtbRectangleSelectExport.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            e.Items.Add(new MetroListTabItem()
            {
                Name = "tbDrawDiagram",
                Header = new ImageTextItem()
                {
                    ImagePosition = eDirection.Top,
                    Text = "制图",
                    ToolTip = "制图设置",
                    //Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/map.png"))
                    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/ViewWebLayoutView.png"))
                },
                Content = visioMapPanel,
            });
        }

        protected override void OnUninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            //btnToDiagramsView.Click -= btnToDiagramsView_Click;
        }
        
        #endregion

        #endregion
    }
}
