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
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightShuShan.Control
{
    /// <summary>
    /// 股权量化
    /// </summary>
    public partial class StockQuanlification : ClassicWindowDialog
    {
        public Zone CurrentZone { get; set; }

        public StockQuanlification()
        {
            InitializeComponent();
            Model = new StockQuanlificationModel();
            DataContext = Model;

        }

        public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(StockQuanlificationModel), typeof(StockQuanlification));

        public StockQuanlificationModel Model
        {
            get { return (StockQuanlificationModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }






    }
}
