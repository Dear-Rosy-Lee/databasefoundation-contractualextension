using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// SplitLandEdit.xaml 的交互逻辑
    /// </summary>
    public partial class SplitLandEdit : MetroDialog
    {
        #region Fields

        private Layer layer;
        private List<Graphic> graphics;
        private MapControl map;
        private GraphicsLayer layerHover;

        #endregion Fields

        #region Property

        public IDbContext DbContext { get; set; }

        public Zone CurrentZone { get; set; }

        public List<string> ListLandCode { get; set; }

        public List<SplitItem> SplitItems { get; set; }

        public bool Flag { get; set; }

        public string LandCode { get; set; }

        #endregion Property

        public SplitLandEdit(MapControl map, Layer layer, List<Graphic> graphics, IDbContext dbContext, Zone zone)
        {
            this.graphics = graphics;
            this.map = map;
            this.layer = layer;
            InitializeComponent();
            DataContext = this;
            DbContext = dbContext;
            CurrentZone = zone;
        }

        #region Methods

        #region Methods - Public

        public void Uninstall()
        {
            layerHover.Graphics.Clear();
            map.Layers.Remove(layerHover);
            layerHover = null;
            map = null;
            graphics = null;
            layer = null;
        }

        #endregion Methods - Public

        #region Methods - Override

        protected override void OnInitializeGo()
        {
        }

        protected override void OnInitializeCompleted()
        {
            var vectorLayer = layer as VectorLayer;
            var labeler = vectorLayer != null ? vectorLayer.Labeler : null;
            var dataSource = vectorLayer != null ? vectorLayer.DataSource : null;
            LandCode = string.Empty;
            SplitItems = new List<SplitItem>();
            graphics.ForEach(c =>
            {
                var item = new SplitItem();
                item.Graphic = c;
                item.Text = labeler != null ? string.Format("{0}", labeler.GetLabelText(c.Object.Object)) : null;

                if (item.Text.TrimSafe().IsNullOrBlank())
                    item.Text = GetDisplayText(c, dataSource);
                var res = c.Object.Object.GetPropertyValue("DKBM").ToString();
                item.SurveyNumber = res.Substring(res.Length - 5);
                item.Flag = Visibility.Visible;
                SplitItems.Add(item);
            });
            LandCode = SplitItems.Select(x => x.SurveyNumber).ToList().Min().ToString();
            dg.ItemsSource = SplitItems;
            TextBlock.Text = $"请选择一个地块,使其地块编码为:{LandCode}";
            Flag = true;
            layerHover = new GraphicsLayer();
            map.InternalLayers.Add(layerHover);
        }

        protected override void OnConfirmStarted()
        {
        }

        protected override void OnConfirmEnded()
        {
        }

        #endregion Methods - Override

        #region Methods - Events

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            dg.ItemsSource = null;
            if (CheckBox.IsChecked == false)
            {
                SplitItems.ForEach(c =>
                {
                    c.Flag = Visibility.Hidden;
                    TextBlock.Text = $"请选择一个地块,使其地块编码为:{LandCode}";
                    TextBlock.Visibility = Visibility.Visible;
                });
                Flag = false;
            }
            else
            {
                SplitItems.ForEach(c =>
                {
                    TextBlock.Visibility = Visibility.Collapsed;
                    c.Flag = Visibility.Visible;
                });
                Flag = true;
            }

            dg.ItemsSource = SplitItems;
        }

        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.ShowDialog(new MessageDialog()
            {
                Message = "是否将原地块编码选择到指定的图形中？",
                Header = "地块编码"
            }, (b, r) =>
            {
                if (b.HasValue && b.Value)
                    ConfirmAsync();
            });
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanConfirm = dg.SelectedIndex >= 0;

            layerHover.Graphics.Clear();

            var item = dg.SelectedItem as SplitItem;
            if (item == null)
                return;

            var graphic = item.Graphic.Clone() as Graphic;

            switch (graphic.Geometry.GeometryType)
            {
                case YuLinTu.Spatial.eGeometryType.Point:
                case YuLinTu.Spatial.eGeometryType.MultiPoint:
                    graphic.Symbol = Application.Current.TryFindResource("DefaultUISymbol_Mark_Union_Highlight") as UISymbol;
                    break;

                case YuLinTu.Spatial.eGeometryType.Polyline:
                case YuLinTu.Spatial.eGeometryType.MultiPolyline:
                    graphic.Symbol = Application.Current.TryFindResource("DefaultUISymbol_Line_Union_Highlight") as UISymbol;
                    break;

                case YuLinTu.Spatial.eGeometryType.Polygon:
                case YuLinTu.Spatial.eGeometryType.MultiPolygon:
                    graphic.Symbol = Application.Current.TryFindResource("DefaultUISymbol_Fill_Union_Highlight") as UISymbol;
                    break;

                case YuLinTu.Spatial.eGeometryType.GeometryCollection:
                case YuLinTu.Spatial.eGeometryType.Unknown:
                default:
                    break;
            }

            layerHover.Graphics.Add(graphic);
        }

        private void mbtnGetLandCode_Click(object sender, RoutedEventArgs e)
        {
            var dc = e.Source.GetPropertyValue("DataContext").GetPropertyValue("SurveyNumber");
            if (dc != null)
            {
                var landBusiness = new AccountLandBusiness(DbContext);
                if (landBusiness == null)
                    landBusiness = new AccountLandBusiness(DbContext);
                string number = landBusiness.GetNewLandNumber(CurrentZone.FullCode);
                SplitItems.Where(x => x.SurveyNumber == dc.ToString()).FirstOrDefault().SurveyNumber = number.Substring(number.Length - 5);
                dg.ItemsSource = null;
                dg.ItemsSource = SplitItems;
            }
        }

        private void dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = dg.SelectedItem as SplitItem;
            if (item == null)
                return;

            if (item.Graphic.Geometry.GeometryType == Spatial.eGeometryType.Point)
                map.PanTo(item.Graphic.Geometry);
            else
                map.ZoomTo(item.Graphic.Geometry);
        }

        private void MetroButton_Click_1(object sender, RoutedEventArgs e)
        {
            Envelope env = null;
            graphics.ForEach(c =>
            {
                if (env == null)
                    env = c.Geometry.GetEnvelope();
                else
                    env.Union(c.Geometry.GetEnvelope());
            });

            map.ZoomTo(env);
        }

        #endregion Methods - Events

        #region Methods - Private

        private string GetDisplayText(Graphic c, YuLinTu.tGIS.Data.IGeoSource dataSource)
        {
            return c.Object != null ? (c.Object.Object != null ? c.Object.Object.ToString() : null) : null;
        }

        #endregion Methods - Private

        #endregion Methods
    }
}