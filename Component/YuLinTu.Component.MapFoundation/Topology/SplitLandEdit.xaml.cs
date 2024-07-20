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
            if (layerHover != null)
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

            string oldNumber;
            var gettype = graphics[0].Object.Object.GetType().ToString();
            if (gettype == "YuLinTu.Library.Entity.ContractLand")
            {
                var ep = graphics[0].Object.Object.GetPropertyValue("OldLandNumber");
                oldNumber = ep == null ? "" : ep.ToString();
            }
            else
            {
                oldNumber = graphics[0].Object.Object.GetPropertyValue("DKBM").ToString();
            }
            oldNumber = oldNumber.Length > 5 ? oldNumber.Substring(oldNumber.Length - 5) : "";
            graphics.ForEach(c =>
            {
                var item = new SplitItem();
                item.Graphic = c;
                var tpye = c.Object.Object.GetType().ToString();
                if (tpye == "YuLinTu.Library.Entity.ContractLand")
                {
                    var land = c.Object.Object as ContractLand;
                    item.Land = land;
                    item.Text = land.OwnerName;
                    item.OldNumber = oldNumber;
                    item.SurveyNumber = land.SurveyNumber;// landNumber.ToString().PadLeft(5, '0');
                    item.NewNumber = item.SurveyNumber;
                    item.Flag = Visibility.Visible;
                    item.DKMJ = land.AwareArea.ToString();
                    SplitItems.Add(item);
                }
                else
                {
                    item.Text = labeler != null ? string.Format("{0}", labeler.GetLabelText(c.Object.Object)) : null;

                    if (item.Text.TrimSafe().IsNullOrBlank())
                        item.Text = GetDisplayText(c, dataSource);
                    var res = c.Object.Object.GetPropertyValue("DKBM").ToString();
                    if (oldNumber.Length > 5)
                        item.OldNumber = oldNumber.Substring(res.Length - 5);
                    var landNumber = int.Parse(res.Substring(res.Length - 5)) + 1;
                    item.SurveyNumber = landNumber.ToString().PadLeft(5, '0');
                    item.NewNumber = item.SurveyNumber;
                    item.Flag = Visibility.Visible;
                    item.DKMJ = c.Object.Object.GetPropertyValue("BZMJ").ToString();
                    SplitItems.Add(item);
                }
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

        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner == null)
            {
                this.Close();
                return;
            }
            Owner.ShowDialog(new MessageDialog()
            {
                Message = "是否确定地块编码编辑完成并保存？",
                Header = "地块编码"
            }, (b, r) =>
            {
                if (b.HasValue && b.Value)
                    ConfirmAsync();
            });
        }

        #endregion Methods - Override

        #region Methods - Events

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var data = sender.GetPropertyValue("DataContext") as SplitItem;
            foreach (var item in SplitItems)
            {
                if (item.NewNumber == data.NewNumber)
                {
                    item.IsChecked = true;
                    item.SurveyNumber = item.OldNumber;
                }
                else
                {
                    item.IsChecked = false;
                    item.SurveyNumber = item.NewNumber;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            {
                foreach (var item in SplitItems)
                {
                    item.IsChecked = false;
                    item.SurveyNumber = item.NewNumber;
                }
            }
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
                foreach (var item in SplitItems)
                {
                    if (item.SurveyNumber == dc.ToString())
                    {
                        var maxNumber = SplitItems.Select(x => x.SurveyNumber).Max();
                        var number = int.Parse(maxNumber) + 1;
                        item.SurveyNumber = number.ToString().PadLeft(5, '0');
                    }
                }

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