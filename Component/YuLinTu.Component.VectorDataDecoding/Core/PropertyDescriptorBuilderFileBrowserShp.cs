using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding
{
    internal class PropertyDescriptorBuilderFileBrowserShp : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.ValidatesOnExceptions = true;
                b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                var designer = new FileBrowserTextBox();
                designer.Filter = "文件类型(*.shp)|*.shp";
                designer.Multiselect = false;
                designer.Watermask = defaultValue.Watermask;
                designer.SetBinding(TextBox.TextProperty, b);
                designer.SetBinding(TextBox.ToolTipProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(TextBox.TextProperty);
            }));

            return defaultValue;
        }

        #endregion
    }

    internal class PropertyDescriptorBuilderFolderBrowserExtsion : PropertyDescriptorBuilder
    {

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke((Action)delegate
            {
                Binding binding = new Binding("Value")
                {
                    Source = defaultValue,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                };

                FolderBrowserTextBox folderBrowserTextBox = new FolderBrowserTextBox();
                folderBrowserTextBox.Watermask = defaultValue.Watermask;
                folderBrowserTextBox.Description = defaultValue.Description;
                folderBrowserTextBox.SetBinding(TextBox.TextProperty, binding);
                folderBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, binding);
                defaultValue.Designer = folderBrowserTextBox;

                defaultValue.BindingExpression = folderBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }, new object[0]);
            return defaultValue;


        }
    }

    internal class PropertyBuilderCheckCardBoolean : PropertyDescriptorBuilder
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
                DockPanel.SetDock(checkBox, Dock.Right);           //调整控件样式

                var cb = new MetroComboBox();
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);
                cb.SelectedValuePath = "Value";
                cb.DisplayMemberPath = "Key";
                cb.ItemsSource = items;

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

        #endregion Methods
    }
}
