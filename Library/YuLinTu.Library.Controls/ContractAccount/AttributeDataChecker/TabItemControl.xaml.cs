using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// TabItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class TabItemControl : UserControl
    {
        public TabItemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        public void InistallControl(Dictionary<TermParamCondition, List<TermParamCondition>> tcList)
        {
            if (tcList == null || tcList.Count == 0)
                return;
            List<TabAutoItemModel> timList = new List<TabAutoItemModel>();
            foreach (var item in tcList)
            {
                var aif = item.Key;
                TabAutoItemModel tm = new TabAutoItemModel()
                {
                    Category = aif.Category,
                    Count = item.Value.Count() + "类",
                    Group = aif.Group,
                    Img = aif.Img,
                    Enable = aif.IsEnable
                };
                item.Value.ForEach(t =>
                {
                    tm.Children.Add(t);
                });
                timList.Add(tm);
            }
            ic.ItemsSource = timList;
        }

        /// <summary>
        /// 设置子项
        /// </summary>
        private void CheckChildControl(StackPanel parentPanel, bool isCheck)
        {
            Grid grid = parentPanel.Children[0] as Grid;
            if (grid == null)
            {
                return;
            }
            foreach (var item in grid.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    if (cb == null)
                        continue;
                    cb.IsChecked = isCheck;
                }
            }
        }

        private void childCBClick(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
            {
                return;
            }
            Grid grid = null;
            if (cb.Parent is Grid)
            {
                grid = cb.Parent as Grid;
            }
            if (grid == null)
            {
                return;
            }
            bool? result = AllCheckStatus(grid);
            SetParentControl(grid, result);
        }

        /// <summary>
        /// 全选状态
        /// </summary>
        private bool? AllCheckStatus(Grid grid)
        {
            bool allCheckFalse = true;//全部未勾选
            bool allCheckTrue = true;//全部勾选
            foreach (var item in grid.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cbox = item as CheckBox;
                    if (cbox == null)
                        continue;
                    if ((bool)cbox.IsChecked)
                    {
                        allCheckFalse = false;
                    }
                    else
                    {
                        allCheckTrue = false;
                    }
                }
            }
            if (allCheckFalse)
            {
                return false;
            }
            else if (allCheckTrue)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置复选项是否选中
        /// </summary>
        private void SetParentControl(Grid grid, bool? isChecked = null)
        {
            StackPanel sp = grid.Parent as StackPanel;
            if (sp == null)
            {
                return;
            }
            MetroExpander mexpander = sp.Parent as MetroExpander;
            if (mexpander == null)
            {
                return;
            }
            int childcount = VisualTreeHelper.GetChildrenCount((DependencyObject)mexpander.Header);
            for (int i = 0; i < childcount; i++)
            {
                var control = VisualTreeHelper.GetChild((DependencyObject)mexpander.Header, i);
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    if (cb != null)
                    {
                        cb.IsChecked = isChecked;
                        break;
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CheckBox cb = sender as CheckBox;
            if (cb == null)
            {
                return;
            }
        }
    }
}
