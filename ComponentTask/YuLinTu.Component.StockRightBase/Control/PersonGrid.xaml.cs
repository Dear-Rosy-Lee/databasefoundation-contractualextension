using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// VirtualPersonGrid.xaml 的交互逻辑
    /// </summary>
    public partial class PersonGrid : UserControl
    {
        public ITheWorkpage TheWorkpage { get; set; }
        public ObservableCollection<PersonGridModel> Items { get; set; } = new ObservableCollection<PersonGridModel>();

        public Action SelectChangedAction { get; set; }

        public IDbContext DataSource { get; set; }
        public Zone CurrentZone { get; set; }

        public VirtualPerson VirPerson { get; set; }

        public PersonGridModel SelectedItem
        {
            get { return view.SelectedItem as PersonGridModel; }
        }

        public List<BelongRelation> PersonbelongRelations { get; set; }

        public List<ContractLand> LandList { get; set; }

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
            StockLandPage page = new StockLandPage();
            page.SelectedItem = SelectedItem;
            page.PersonbelongRelations = this.PersonbelongRelations;
            page.DataSource = this.DataSource;
            page.LandList = this.LandList;
            page.Workpage = TheWorkpage;
            page.CurrentZone = this.CurrentZone;
            page.VirtualItem = VirPerson;
            TheWorkpage.Page.ShowDialog(page, (ee, aa) =>
            {
                if (!(bool)ee)
                    return;
                Updata();
            });
        }

        private void Updata()
        {

        }
    }
}
