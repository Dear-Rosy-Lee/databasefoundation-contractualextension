using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// LandGrid.xaml 的交互逻辑
    /// </summary>
    public partial class LandGrid : UserControl
    {

        public Action SelectChangedAction { get; set; }

        public Action ItemDoubleClick { get; set; }

        public ObservableCollection<ContractLand> Items { get; set; } = new ObservableCollection<ContractLand>();

        public ContractLand SelectedItem
        {
            get { return view.SelectedItem as ContractLand; }
        }

        public LandGrid()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectChangedAction?.Invoke();
        }

        private void view_ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClick?.Invoke();
        }

    }
}
