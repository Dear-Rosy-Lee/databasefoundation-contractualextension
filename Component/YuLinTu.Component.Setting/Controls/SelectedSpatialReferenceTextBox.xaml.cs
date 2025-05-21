/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
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
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.tGIS.Client.Controls;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 选择坐标系界面
    /// </summary>
    public partial class SelectedSpatialReferenceTextBox : MetroTextBox
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectedSpatialReferenceTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        /// 依赖属性地域全名称
        /// </summary>
        public Spatial.SpatialReference NowSpatialReference
        {
            get { return (Spatial.SpatialReference)GetValue(NowSpatialReferenceProperty); }
            set { SetValue(NowSpatialReferenceProperty, value); }
        }

        public static readonly DependencyProperty NowSpatialReferenceProperty = DependencyProperty.Register("NowSpatialReference", typeof(Spatial.SpatialReference), typeof(SelectedSpatialReferenceTextBox), new PropertyMetadata((s, a) =>
           {
               var sr = a.NewValue as Spatial.SpatialReference;
               if (sr == null) (s as MetroTextBox).Text = "";
               if (sr != null)
               {
                   try
                   {
                       DotSpatial.Projections.ProjectionInfo cpi = sr.CreateProjectionInfo();
                       if (sr.WKID == 0)
                       {
                           (s as MetroTextBox).Text = "Unknown";
                       }
                       else
                       {
                           if (sr.IsGEOGCS())
                           {
                               (s as MetroTextBox).Text = cpi.GeographicInfo.Name;
                           }
                           else if (sr.IsPROJCS())
                           {
                            //大地坐标系
                            (s as MetroTextBox).Text = cpi.Name;
                           }
                       }
                   }
                   catch
                   {
                       (s as MetroTextBox).Text = "";
                   }
               }
           }));

        /// <summary>
        /// 工作空间属性
        /// </summary>
        public IWorkspace WorkSpace { get; set; }

        #endregion Properties

        #region Event

        /// <summary>
        /// 地域选择按钮选择
        /// </summary>
        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            SpatialReferenceSelectWindow SRSelectPage = new SpatialReferenceSelectWindow();
            SRSelectPage.RootDirectory = System.IO.Path.Combine(TheApp.Current.GetDataPath(), "SpatialReferences");
            SRSelectPage.WindowTitle = null;
            SRSelectPage.Topmost = true;
            SRSelectPage.WindowCaption = "选择坐标系";
            SRSelectPage.WindowDescription = "从下面的列表中选择一个新坐标系,作为新数据库的坐标系";
            var settings = TheApp.Current.TryLoadUserSection("Guest@yulintu.com");
            var index = settings.GetSection("ThemeColor").TryGetValue("ThemeColor", 1);
            SRSelectPage.SetTheme(new ResourceDictionary() { Source = new Uri(string.Format("pack://application:,,,/YuLinTu.Windows.Wpf.Metro;component/Themes/{0:D2}/WindowColor.xaml", index)) });
            SRSelectPage.ShowDialog();

            var currentSR = SRSelectPage.GetSelectedSpatialReference();
            if (currentSR == null)
            {
                NowSpatialReference = null;
            }
            else
            {
                NowSpatialReference = currentSR;
            }
        }

        #endregion Event
    }
}