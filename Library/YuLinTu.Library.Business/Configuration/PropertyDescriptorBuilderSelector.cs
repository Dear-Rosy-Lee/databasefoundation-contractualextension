/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows.Data;
using System.Windows.Controls;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 自定义控件
    /// 对应对象实体类属性为 int 型
    /// </summary>
    public class PropertyDescriptorBuilderSelector : PropertyDescriptorBuilder
    {
        #region Methods

        /// <summary>
        /// 自定义combox控件，通过defaultValue绑定控件，combox被选中的值是绑定的属性Value 
        /// 其实这个Value就是对象的某个属性值
        /// </summary>
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var list = defaultValue.PropertyGrid.Properties["index"] as List<KeyValue<int, string>>;
                var cb = new MetroComboBox();
                cb.ItemsSource = list;
                cb.DisplayMemberPath = "Value";
                cb.SelectedValuePath = "Key";
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);

                //defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;

                var b1 = new Binding("Value");
                b1.Source = defaultValue;
                cb.SetBinding(ComboBox.SelectedValueProperty, b1);
                defaultValue.Designer = cb;

            }));
            return defaultValue;
        }

        #endregion
    }

    /// <summary>
    /// 自定义控件
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderString : PropertyDescriptorBuilder
    {
        #region Methods

        /// <summary>
        /// 自定义combox控件，通过defaultValue绑定控件，combox被选中的值是绑定的属性Value 
        /// 其实这个Value就是对象的某个属性值
        /// </summary>
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var list = defaultValue.PropertyGrid.Properties["index"] as List<KeyValue<int, string>>;
                var cb = new MetroComboBox();
                cb.ItemsSource = list;
                cb.DisplayMemberPath = "Value";
                cb.SelectedValuePath = "Value";              
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);
              
                var b1 = new Binding("Value");
                b1.Source = defaultValue;
                cb.SetBinding(ComboBox.SelectedValueProperty, b1);
                defaultValue.Designer = cb;

            }));
            return defaultValue;
        }

        #endregion
    }

    /// <summary>
    /// 自定义控件
    /// 对应对象实体类属性为bool型
    /// </summary>
    public class PropertyDescriptorBoolean : PropertyDescriptorBuilder
    {
        #region Methods

        /// <summary>
        /// 自定义combox控件，通过defaultValue绑定控件，combox被选中的值是绑定的属性Value 
        /// 其实这个Value就是对象的某个属性值，checkbox和combox绑定同一个值
        /// </summary>
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var items = new KeyValueList<string, bool>();
                items["是"] = true;
                items["否"] = false;

                var dockPanel = new DockPanel();
                var checkBox = new CheckBox();
                checkBox.Margin = new Thickness(4);
                checkBox.VerticalAlignment = VerticalAlignment.Center;
                if (defaultValue.AliasName.Equals("承包方名称"))
                    checkBox.IsEnabled = false;
                DockPanel.SetDock(checkBox, Dock.Right);           //调整控件样式

                var cb = new MetroComboBox();
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);
                cb.SelectedValuePath = "Value";
                cb.DisplayMemberPath = "Key";
                cb.ItemsSource = items;
                if(defaultValue.AliasName.Equals("承包方名称"))
                    cb.IsEnabled = false;

                var b1 = new Binding("Value");
                b1.Source = defaultValue;
                cb.SetBinding(ComboBox.SelectedValueProperty, b1);
                checkBox.SetBinding(CheckBox.IsCheckedProperty, b1);

                dockPanel.Children.Add(checkBox);
                dockPanel.Children.Add(cb);
                defaultValue.Designer = dockPanel;
            }));
            return defaultValue;
        }

        #endregion
    }

    /// <summary>
    /// 自定义控件
    /// 对应对象实体类属性为bool型
    /// </summary>
    public class PropertyDescriptorBool : PropertyDescriptorBuilder
    {
        #region Methods

        /// <summary>
        /// 自定义combox控件，通过defaultValue绑定控件，combox被选中的值是绑定的属性Value 
        /// 其实这个Value就是对象的某个属性值，checkbox和combox绑定同一个值
        /// </summary>
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var items = new KeyValueList<string, bool>();
                items["是"] = true;
                items["否"] = false;

                var dockPanel = new DockPanel();
                //var checkBox = new CheckBox();
                //checkBox.Margin = new Thickness(4);
                //checkBox.VerticalAlignment = VerticalAlignment.Center;
               // DockPanel.SetDock(checkBox, Dock.Right);           //调整控件样式

                var cb = new MetroComboBox();
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);
                cb.SelectedValuePath = "Value";
                cb.DisplayMemberPath = "Key";
                cb.ItemsSource = items;

                var b1 = new Binding("Value");
                b1.Source = defaultValue;
                cb.SetBinding(ComboBox.SelectedValueProperty, b1);
               // checkBox.SetBinding(CheckBox.IsCheckedProperty, b1);

               // dockPanel.Children.Add(checkBox);
                dockPanel.Children.Add(cb);
                defaultValue.Designer = dockPanel;
            }));
            return defaultValue;
        }

        #endregion
    }
}
