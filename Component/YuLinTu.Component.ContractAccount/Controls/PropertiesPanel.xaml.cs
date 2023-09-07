using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    public partial class PropertiesPanel : UserControl, IDisposable
    {
        #region Properties

        #endregion

        #region Fields

        private Dictionary<Type, PropertyGrid> dicPropertyGrids = new Dictionary<Type, PropertyGrid>();

        #endregion

        #region Ctor

        public PropertiesPanel()
        {
            InitializeComponent();

            DataContext = this;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void SetObject(object obj)
        {
            if (obj == null)
            {
                Clear();
                return;
            }

            var pg = GetPropertyGrid(obj);
            pg.Object = obj;
            pg.Visibility = System.Windows.Visibility.Visible;

            foreach (var item in dicPropertyGrids)
            {
                if (item.Value == pg)
                    continue;

                item.Value.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public void Clear()
        {
            foreach (var item in dicPropertyGrids)
                item.Value.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Dispose()
        {
            foreach (var item in dicPropertyGrids)
                if(item.Value!= null) item.Value.Object = null;

            dicPropertyGrids.Clear();
            container.Children.Clear();
        }

        #endregion

        #region Methods - Private

        private PropertyGrid GetPropertyGrid(object obj)
        {
            var type = obj.GetType();
            if (!dicPropertyGrids.ContainsKey(type))
            {
                var pg = new PropertyGrid()
                {
                    Margin = new Thickness(10, 0, 10, 0),
                    BroadcastPropertyChanged = false,
                    IsGroupingEnabled = true
                };

                container.Children.Add(pg);
                dicPropertyGrids[type] = pg;
            }

            return dicPropertyGrids[type];
        }

        #endregion

        #endregion
    }
}
