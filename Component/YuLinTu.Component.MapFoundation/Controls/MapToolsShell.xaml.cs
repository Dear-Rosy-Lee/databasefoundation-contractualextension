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

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 下拉菜单控件
    /// </summary>
    public partial class MapToolsShell : UserControl
    {
        public System.Collections.ObjectModel.ObservableCollection<object> Tools { get; private set; }

        public MapToolsShell()
        {
            InitializeComponent();

            DataContext = this;
            Tools = new System.Collections.ObjectModel.ObservableCollection<object>();
        }
    }
}
