/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Component.ZoneInvestigation
{
    public class MapPageContext : MapPageContextBase
    {
        #region Properties

        #endregion

        #region Fields
        /// <summary>
        /// 导航类
        /// </summary>       
        private VirtualPersonNavigator pnav = new VirtualPersonNavigator();
        private Zone currentZone;

        private MetroToggleButton btnMapQueryBusiness;
        private MapCoordinateStatusbarShell shellMapCoordinateStatusbar;

        private MetroToggleButton btnShowLayer = null;
        //private PopulationPanel panel = null;
        #endregion

        #region Ctor

        public MapPageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        #region Methods

        #region Methods - Override

        /// <summary>
        /// 初始化添加查询坐标节点
        /// </summary>        
        [MessageHandler(ID = IDMap.InstallToobarInQueryContainer)]
        private void InstallToobarInQueryContainer(object sender, InstallUIElementsEventArgs e)
        {
            btnMapQueryBusiness = new MetroToggleButton();
            btnMapQueryBusiness.ToolTip = "查询节点";
            btnMapQueryBusiness.Padding = new Thickness(4, 2, 4, 2);
            btnMapQueryBusiness.MaxWidth = 45;
            btnMapQueryBusiness.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnMapQueryBusiness.Content = new ImageTextItem() { ImagePosition = eDirection.Top, Text = "查询节点", Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/List.png")) };
            e.Items.Add(btnMapQueryBusiness);

            var binding4 = new Binding("Action");
            binding4.Source = MapControl;
            binding4.Converter = new MapActionToQueryGeoPointButtonIsCheckedConverter();
            binding4.Mode = BindingMode.TwoWay;
            btnMapQueryBusiness.SetBinding(MetroToggleButton.IsCheckedProperty, binding4);
        }

        /// <summary>
        /// 初始化添加查询坐标节点
        /// </summary>        

        protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        {
            e.Items.Add(new MetroListTabItem()
            {
                Name = "PopulationItem",
                Header = new ImageTextItem()
                {
                    ImagePosition = eDirection.Top,
                    Text = "权属",
                    ToolTip = "权属",  //LanguageAttribute.GetLanguage("lang3070005")
                    Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/Population32.png"))
                },
                Content = new TextBox(),
            });

        }


        /// <summary>
        /// 初始化状态栏时加载坐标显示及跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnInstalStatusbar(object sender, InstallUIElementsEventArgs e)
        {
            shellMapCoordinateStatusbar = new MapCoordinateStatusbarShell(MapControl);
            shellMapCoordinateStatusbar.OnInstallMouseMoveDisplayCoordinate();
            DockPanel.SetDock(shellMapCoordinateStatusbar, Dock.Right);
            e.Items.Add(shellMapCoordinateStatusbar);

            var sp = new YuLinTu.Windows.Wpf.Metro.Components.Separator();
            DockPanel.SetDock(sp, Dock.Right);
            e.Items.Add(sp);
        }

        /// <summary>
        /// 注册导航模板，设定导航样式 
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Navigation/Res.xaml") };
            var key = new DataTemplateKey(typeof(NavigateZoneItem));
            if (Navigator != null)
            {
                Navigator.RegisterItemTemplate(typeof(NavigateZoneItem), dic[key] as DataTemplate);
            }

        }

        protected override void OnUninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            pnav = null;
        }

        #endregion

        #region Methods - Events

        #endregion

        #region Methods-Message

        /// <summary>
        /// 初始化安装账户
        /// </summary>     
        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            base.OnInstallAccountData(sender, e);
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langInstallNavigateItems)]
        private void OnInstallNavigateItems(object sender, InstallNavigateItemMsgEventArgs e)
        {
            e.Instance.Items.AddRange(pnav.GetChildren(e.Instance.Root));
        }

        /// <summary>
        /// 导航到对应点
        /// </summary>      
        [MessageHandler(ID = EdCore.langNavigateTo)]
        private void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            VirtualPerson selectPerson = null;
            if (e.Object == null)
                return;
            if (e.Object.Object is Zone)
            {
                currentZone = e.Object.Object as Zone;
                var geo = currentZone.Shape;
                if (geo == null)
                    return;
                geo = geo.Envelope().Buffer(10);
                MapControl.ZoomTo(geo);
            }
            if (e.Object.Object is VirtualPerson)
            {
                selectPerson = e.Object.Object as VirtualPerson;
                var db = DataSource.Create<DbContext>(TheBns.Current.GetDataSourceName());

                //获取到组别下的权利人下地块集合
                var workstation = new ContainerFactory(db).CreateWorkstation<ISecondTableLandWorkStation, ISecondTableLandRepository>();
                var getPersonLand = workstation.GetCollection(selectPerson.ID);
                if (getPersonLand == null) return;
                YuLinTu.Spatial.Geometry personLandGeo = null;
                if (getPersonLand.Count == 1)
                {
                    if (getPersonLand[0].Shape == null) return;
                }
                else if (getPersonLand.Count > 1)
                {
                    for (int i = 1; i < getPersonLand.Count; i++)
                    {
                        if (getPersonLand[i].Shape == null) continue;
                        personLandGeo = (getPersonLand[0].Shape as YuLinTu.Spatial.Geometry).Union(getPersonLand[i].Shape as YuLinTu.Spatial.Geometry);
                    }
                }
                if (personLandGeo == null) return;
                personLandGeo = personLandGeo.Envelope().Buffer(10);
                MapControl.ZoomTo(personLandGeo);
            }

        }



        #endregion

        #endregion
    }
}
