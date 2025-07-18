using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Data;
using YuLinTu.DF.Controls;
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
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
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

    internal class PropertyDescriptorBuilderProveFileBrowserShp : PropertyDescriptorBuilder
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
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                var designer = new FileBrowserTextBox();
                designer.Filter = "文档文件(*.pdf;) | *.pdf;| 图片文件(*.jpg; *.png)| *.jpg;*.png";
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
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
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
                b1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
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
    public class PropertyDescriptorBuilderTextBoxCustom : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke((Action)delegate
            {
                int maxLength = int.MaxValue;
                PropertyInfo property = defaultValue.Object.GetType().GetProperty(defaultValue.Name);
                DataColumnAttribute attribute = property.GetAttribute<DataColumnAttribute>();
                if (attribute != null)
                {
                    maxLength = ((attribute.Size <= 0) ? int.MaxValue : (attribute.Size / 2));
                }

                Binding binding = new Binding("Value")
                {
                    Source = defaultValue,
                    ValidatesOnDataErrors = true,
                    Mode= BindingMode.TwoWay,
                    UpdateSourceTrigger= UpdateSourceTrigger.PropertyChanged
                };
                MetroTextBox metroTextBox = new MetroTextBox();
                metroTextBox.MaxLength = maxLength;
                metroTextBox.Watermask = defaultValue.Watermask;
                metroTextBox.PaddingWatermask = new Thickness(5.0, 0.0, 0.0, 0.0);
                metroTextBox.IsReadOnly = false;
                metroTextBox.BorderThickness = new Thickness(1.0);
                metroTextBox.SetBinding(TextBox.TextProperty, binding);
                metroTextBox.SetBinding(FrameworkElement.ToolTipProperty, binding);
                defaultValue.Designer = metroTextBox;
            }, new object[0]);
            return defaultValue;
        }
    }
    public class PropertyDescriptorBuilderReadOnlyTextBoxCustom : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke((Action)delegate
            {
                int maxLength = int.MaxValue;
                PropertyInfo property = defaultValue.Object.GetType().GetProperty(defaultValue.Name);
                DataColumnAttribute attribute = property.GetAttribute<DataColumnAttribute>();
                if (attribute != null)
                {
                    maxLength = ((attribute.Size <= 0) ? int.MaxValue : (attribute.Size / 2));
                }

                Binding binding = new Binding("Value")
                {
                    Source = defaultValue,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                MetroTextBox metroTextBox = new MetroTextBox();
                metroTextBox.MaxLength = maxLength;
                metroTextBox.Watermask = defaultValue.Watermask;
                metroTextBox.PaddingWatermask = new Thickness(5.0, 0.0, 0.0, 0.0);
                metroTextBox.IsReadOnly = true;
                metroTextBox.BorderThickness = new Thickness(1.0);
                metroTextBox.SetBinding(TextBox.TextProperty, binding);
                metroTextBox.SetBinding(FrameworkElement.ToolTipProperty, binding);
                defaultValue.Designer = metroTextBox;
            }, new object[0]);
            return defaultValue;
        }
    }

    public class PropertyDescriptorBuilderMultiLineReadOnlyTextBoxCustom : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke((Action)delegate
            {
                int maxLength = int.MaxValue;
                PropertyInfo property = defaultValue.Object.GetType().GetProperty(defaultValue.Name);
                DataColumnAttribute attribute = property.GetAttribute<DataColumnAttribute>();
                if (attribute != null)
                {
                    maxLength = ((attribute.Size <= 0) ? int.MaxValue : (attribute.Size / 2));
                }

                Binding binding = new Binding("Value")
                {
                    Source = defaultValue,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                MetroTextBox metroTextBox = new MetroTextBox();
                metroTextBox.AcceptsReturn = true;
                metroTextBox.MinHeight = 100.0;
                metroTextBox.MaxHeight = 300.0;
                metroTextBox.MaxLength = maxLength;
                metroTextBox.Watermask = defaultValue.Watermask;
                metroTextBox.PaddingWatermask = new Thickness(5.0, 0.0, 0.0, 0.0);
                metroTextBox.VerticalContentAlignment = VerticalAlignment.Stretch;
                metroTextBox.IsReadOnly = true;
                metroTextBox.BorderThickness = new Thickness(1.0);
                metroTextBox.SetBinding(TextBox.TextProperty, binding);
                metroTextBox.SetBinding(FrameworkElement.ToolTipProperty, binding);
                defaultValue.Designer = metroTextBox;
            }, new object[0]);
            return defaultValue;
        }
    }

    public class PropertyDescriptorBuilderEnum : PropertyDescriptorBuilder
    {
        public ObservableKeyValueList<object, string> Items { get; private set; }

        public PropertyDescriptorBuilderEnum()
        {
        }

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(() =>
            {
                var type = defaultValue.Object.GetType();
                var pi = type.GetProperty(defaultValue.Name);
                var enumType = pi.PropertyType;
                //var filterMethod = pi.GetAttribute<EnumFilterAttribute>()?.FilterMethod;

                Items = new ObservableKeyValueList<object, string>();
                var fields = enumType.GetFields();
                foreach (var field in fields)
                {
                    if (!field.FieldType.IsEnum)
                        continue;

                    var enumValue = field.GetValue(null);

                    //if (filterMethod != null && !((bool)type.GetMethod(filterMethod)
                    //    .Invoke(defaultValue.Object, new object[] { enumValue })))
                    //    continue;

                    Items[enumValue] = EnumNameAttribute.GetDescription(enumValue);
                }

                var cb = new ClearableComboBox
                {
                    Padding = new Thickness(6, 4, 6, 5),
                    BorderThickness = new Thickness(1),
                    SelectedValuePath = "Key",
                    DisplayMemberPath = "Value",
                    ItemsSource = Items,
                    SelectedIndex = -1
                };
                cb.Style = (Style)cb.FindResource("Metro_ComboBox_Style");

                var b1 = new Binding("Value")
                {
                    Source = defaultValue,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                var pda = pi.GetAttribute<PropertyDescriptorAttribute>();
                if (pda != null && pda.Converter != null)
                {
                    b1.Converter = Activator.CreateInstance(pda.Converter) as IValueConverter;
                    b1.ConverterParameter = new PropertyGridConverterParameterPair(defaultValue.PropertyGrid, pda.ConverterParameter);
                }

                cb.SetBinding(MetroComboBox.SelectedValueProperty, b1);

                defaultValue.Designer = cb;
                defaultValue.BindingExpression = cb.GetBindingExpression(ClearableComboBox.SelectedValueProperty);
            });

            return defaultValue;
        }
    }

    public class PropertyDescriptorBuilderComboBoxEnum : PropertyDescriptorBuilder
    {
        public ObservableKeyValueList<object, string> Items { get; private set; }

        public PropertyDescriptorBuilderComboBoxEnum()
        {
        }

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(() =>
            {
                var type = defaultValue.Object.GetType();
                var pi = type.GetProperty(defaultValue.Name);
                var enumType = pi.PropertyType;
                //var filterMethod = pi.GetAttribute<EnumFilterAttribute>()?.FilterMethod;

                Items = new ObservableKeyValueList<object, string>();
                var fields = enumType.GetFields();
                foreach (var field in fields)
                {
                    if (!field.FieldType.IsEnum)
                        continue;

                    var enumValue = field.GetValue(null);

                    //if (filterMethod != null && !((bool)type.GetMethod(filterMethod)
                    //    .Invoke(defaultValue.Object, new object[] { enumValue })))
                    //    continue;

                    Items[enumValue] =EnumNameAttribute.GetDescription(enumValue);
                }

                var cb = new MetroComboBox
                {
                    Padding = new Thickness(6, 4, 6, 5),
                    BorderThickness = new Thickness(1),
                    SelectedValuePath = "Key",
                    DisplayMemberPath = "Value",
                    ItemsSource = Items,
                    SelectedIndex = -1
                };
                cb.Style = (Style)cb.FindResource("Metro_ComboBox_Style");

                var b1 = new Binding("Value")
                {
                    Source = defaultValue,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                var pda = pi.GetAttribute<PropertyDescriptorAttribute>();
                if (pda != null && pda.Converter != null)
                {
                    b1.Converter = Activator.CreateInstance(pda.Converter) as IValueConverter;
                    b1.ConverterParameter = new PropertyGridConverterParameterPair(defaultValue.PropertyGrid, pda.ConverterParameter);
                }

                cb.SetBinding(MetroComboBox.SelectedValueProperty, b1);

                defaultValue.Designer = cb;
                defaultValue.BindingExpression = cb.GetBindingExpression(ClearableComboBox.SelectedValueProperty);
            });

            return defaultValue;
        }
    }

 
}
