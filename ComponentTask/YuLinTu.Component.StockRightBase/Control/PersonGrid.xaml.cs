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
using YuLinTu.Component.StockRightBase.Entity;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// VirtualPersonGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PersonGrid : UserControl
    {
        public ObservableCollection<PersonGridModel> Items { get; set; } = new ObservableCollection<PersonGridModel>();

        public Action SelectChangedAction { get; set; }


        public PersonGridModel SelectedItem
        {
            get { return view.SelectedItem as PersonGridModel; }
        }

        public PersonGrid()
        {
            InitializeComponent();
            view.Roots = Items; 
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectChangedAction?.Invoke();
        }

        private void miEdit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
