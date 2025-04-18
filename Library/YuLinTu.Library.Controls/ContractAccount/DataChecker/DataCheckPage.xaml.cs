using CSScriptLibrary;
using DevComponents.DotNetBar;
using DotSpatial.Projections.Transforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{

    /// <summary>
    /// DataCheckPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataCheckPage : TabMessageBox
    {
        private Dictionary<string, object> _dataObjects = new Dictionary<string, object>(); 
        private List<KeyValue<string,CheckItemConfig>> checkItemList = new List<KeyValue<string, CheckItemConfig>>(); 
        public TaskAttributeDataCheckerArgument TaskArgument;
        private object _currentObject; // 当前显示的类别对象
        public IWorkpage Page { get; set; }

        public DataCheckPage()
        {
            InitializeComponent();
            LoadNavigationItems();
        }

        public void LoadNavigationItems()
        {
            var assembly = Assembly.Load("YuLinTu.Library.Business");
            var types = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<NavigationItemAttribute>() != null);
            var savedConfigs = CheckItemConfigHelper.Load();
            savedConfigs.ForEach(x =>
            {
                checkItemList.Add(new KeyValue<string, CheckItemConfig>(x.DisplayName, x));
            });
            
            
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<NavigationItemAttribute>();
                var obj = Activator.CreateInstance(type);  // 创建数据对象
                _dataObjects[attr.Title] = obj; // 存储对象
                

                var item = new ListBoxItem { Content = attr.Title, Tag = obj };
                NavListBox.Items.Add(item);
            }

            if (NavListBox.Items.Count > 0)
            {
                NavListBox.SelectedIndex = 0; // 默认选中第一项
            }
        }



        /// <summary>
        /// 切换导航栏时加载对应的检查项
        /// </summary>
        private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavListBox.SelectedItem is ListBoxItem item && item.Tag is object selectedObject)
            {
                _currentObject = selectedObject; // 更新当前对象
                LoadCheckItems(selectedObject);
            }
        }

        private void LoadCheckItems(object dataObject)
        {
            CheckItemPanel.Children.Clear();
            var properties = dataObject.GetType().GetProperties()
              .Where(p => p.GetCustomAttribute<CheckItemAttribute>() != null);
            var savedConfigs = CheckItemConfigHelper.Load();
            
            
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<CheckItemAttribute>();
                if (attr != null)
                {
                    var target = checkItemList.FirstOrDefault(kv => kv.Key == prop.Name);
                    dataObject.SetPropertyValue(prop.Name, target.Value.IsChecked);
                    var grid = new Grid
                    {
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    // 定义三列（CheckBox、Label、Description）
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    // 复选框
                    var checkBox = new System.Windows.Controls.CheckBox
                    {
                        IsChecked = (bool)prop.GetValue(dataObject),
                        Tag = prop,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 0),
                        FontSize = 13
                    };
                    checkBox.Checked += CheckBox_Changed;
                    checkBox.Unchecked += CheckBox_Changed;
                    Grid.SetColumn(checkBox, 0);

                    // 标题（Label）
                    var label = new TextBlock
                    {
                        Text = attr.Label,
                        FontWeight = FontWeights.Bold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(-5, 0, 5, 0)
                    };
                    Grid.SetColumn(label, 1);

                    // 描述（Description）
                    var description = new TextBlock
                    {
                        Text = attr.Description,
                        FontSize = 12,
                        Foreground = System.Windows.Media.Brushes.Gray,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(50, 0, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    Grid.SetColumn(description, 2);

                    

                    // 横向分割线
                    var separator = new System.Windows.Controls.Separator
                    {
                        Margin = new Thickness(0, 5, 0, 5),
                        Background = System.Windows.Media.Brushes.LightGray,
                        Height = 1
                    };

                    // 添加控件到 Grid
                    grid.Children.Add(checkBox);
                    grid.Children.Add(label);
                    grid.Children.Add(description);
                    // 将 Grid 添加到 CheckItemPanel
                    CheckItemPanel.Children.Add(grid);
                    CheckItemPanel.Children.Add(separator);
                }
            }
        }
        /// <summary>
        /// 复选框状态变化事件
        /// </summary>
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.Tag is PropertyInfo property && _currentObject != null)
            {
                var target = checkItemList.FirstOrDefault(kv => kv.Key == property.Name);
                if (target != null && checkBox.IsChecked is bool isChecked)
                {
                    target.Value.IsChecked = isChecked;
                }
                var configList = checkItemList.Select(kv => kv.Value).ToList();
                CheckItemConfigHelper.Save(configList);
                property.SetValue(_currentObject, checkBox.IsChecked);
            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(true);
            // 取消对“全不选”的选中状态，防止同时勾选
            SelectNoneCheckBox.IsChecked = false;
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void SelectNoneCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(false);
            // 取消对“全选”的选中状态
            SelectAllCheckBox.IsChecked = false;
        }
        private void SetAllCheckBoxes(bool isChecked)
        {
            foreach (var grid in CheckItemPanel.Children.OfType<Grid>())
            {
                var target = checkItemList.FirstOrDefault(kv => kv.Key == grid.Name);
                if (target != null)
                {
                    target.Value.IsChecked = isChecked;
                }
                var configList = checkItemList.Select(kv => kv.Value).ToList();
                CheckItemConfigHelper.Save(configList);
                var checkBox = grid.Children.OfType<System.Windows.Controls.CheckBox>().FirstOrDefault();
                if (checkBox != null)
                {
                    checkBox.IsChecked = isChecked;
                }
            }
        }
        private void SelectNoneCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var mandatoryField = MapCheckItems<MandatoryField>();
            var dataCorrectness = MapCheckItems<DataCorrectness>();
            var ruleOfIDCheck = MapCheckItems<RuleOfIDCheck>();
            var dataLogic = MapCheckItems<DataLogic>();
            var dataRepeatability = MapCheckItems<DataRepeataBility>();
            var uniqueness = MapCheckItems<Uniqueness>();
            TaskArgument = new TaskAttributeDataCheckerArgument();
            TaskArgument.MandatoryField = mandatoryField;
            TaskArgument.DataCorrectness = dataCorrectness;
            TaskArgument.RuleOfIDCheck = ruleOfIDCheck;
            TaskArgument.DataLogic = dataLogic;
            TaskArgument.DataRepeataBility = dataRepeatability;
            TaskArgument.Uniqueness = uniqueness;
        }

        private T MapCheckItems<T>() where T : new()
        {
            var targetObject = new T();
            var targetProperties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<CheckItemAttribute>() != null)
                .ToDictionary(p => p.Name, p => p); // 转换为字典，方便查找

            foreach (var entry in checkItemList)
            {
                foreach (var prop in targetProperties)
                {
                    if (targetProperties.ContainsKey(entry.Key))
                    {
                        var target = targetProperties.FirstOrDefault(kv => kv.Key == entry.Key);
                        target.Value.SetValue(targetObject, entry.Value.IsChecked);
                    }
                }
            }

            return targetObject;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var configList = checkItemList.Select(kv => kv.Value).ToList();
            CheckItemConfigHelper.Save(configList);
        }
    }
}
