using System.Windows;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask.Controls
{
    /// <summary>
    /// SelectedSpatialReference.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedSpatialReference : MetroTextBox
    {
        public SelectedSpatialReference()
        {
            InitializeComponent();
            this.IsReadOnly = true;
        }

        public SpatialReference SetReference
        {
            get { return (SpatialReference)GetValue(SetReferenceProperty); }
            set
            {
                SetValue(SetReferenceProperty, value);
            }
        }

        public static readonly DependencyProperty SetReferenceProperty =
            DependencyProperty.Register("SetReference", typeof(SpatialReference), typeof(SelectedSpatialReference));

        private static void CurrentReferenceChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs args)
        {
            SelectedSpatialReference reference = dependency as SelectedSpatialReference;
            if (reference != null)
            {
                SpatialReference spatial = args.NewValue as SpatialReference;
                if (spatial == null)
                {
                    reference.Text = null;
                }
                else
                {
                    var name = spatial.CreateProjectionInfo().Name;
                    reference.Text = string.IsNullOrEmpty(name) ? spatial.WKID.ToString() : name;
                }
            }
        }
        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            SpatialReference sr = OpenSelectSpatialReference();
            if (sr == null)
            {
                Text = null;
                return;
            }
            else
            {
                var name = sr.CreateProjectionInfo().Name;
                Text = string.IsNullOrEmpty(name) ? sr.WKID.ToString() : name;
            }

            SetReference = sr;
            this.Focus();
        }

        private SpatialReference OpenSelectSpatialReference()
        {
            // 加载地理坐标系信息
            var dlg = new SpatialReferenceSelectWindow()
            {
                WindowTitle = null,
                CloseCaptionButtonVisibility = Visibility.Visible,
                RootDirectory = System.IO.Path.Combine(TheApp.Current.GetDataPath(), "SpatialReferences")
            };

            dlg.ShowDialog();

            return dlg.GetSelectedSpatialReference();
        }


    }
}
