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
using YuLinTu.Component.StockRightBase.Entity;
using YuLinTu.Component.StockRightBase.Enum;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// 股权分配
    /// </summary>
    public partial class StockDistribution : InfoPageBase
    {

        public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(StockDistributionModel), typeof(StockDistribution));

        public StockDistributionModel Model
        {
            get { return (StockDistributionModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }


        public StockDistribution()
        {
            InitializeComponent();
            Model = new StockDistributionModel();
            DataContext = Model;
            radioButtonPerson.IsChecked = true;
            base.Confirm += Confirm;

        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var hasError = this.Validate();
            if (hasError)
                return;
            ConfirmAsync();
            Close();
        }

        private new void Confirm(object sender, MsgEventArgs<bool> e)
        {
            e.Parameter = true;
        }
    }
}
