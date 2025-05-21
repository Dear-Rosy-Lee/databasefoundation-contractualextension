using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using YuLinTu.Component.CoordinateTransformTask.Controls;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 配置项
    /// </summary>
    public class PropertyDescriptorBuilderComboBox : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;

                var designer = new MetroComboBox();
                designer.ItemsSource = new List<TypeDescrip>()
                {
                    new TypeDescrip(){Message="添加带号",Type=2},
                    new TypeDescrip(){Message= "删除带号",Type=1}
                };
                designer.DisplayMemberPath = "Message";
                designer.SelectedValuePath = "Type";
                designer.SetBinding(ComboBox.SelectedValueProperty, b);
                if ((int)defaultValue.Value == 0)
                {
                    designer.SelectedIndex = 0;
                }

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(MetroComboBox.SelectedValueProperty);
            }));

            return defaultValue;
        }

        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyDescriptorBuilderDoubleUpDown : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;
                var designer = new DoubleUpDown();
                designer.ToolTip = "";
                designer.TextAlignment = TextAlignment.Left;
                designer.SetBinding(DoubleUpDown.ValueProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(DoubleUpDown.ValueProperty);
            }));

            return defaultValue;
        }

        #endregion
    }

    public class PropertyDescriptorBuilderLable : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;
                var designer = new TextBlock();
                //designer.ToolTip = "";
                designer.HorizontalAlignment = HorizontalAlignment.Stretch;
                designer.VerticalAlignment = VerticalAlignment.Center;
                //designer.VerticalContentAlignment = VerticalAlignment.Center;
                //designer.HorizontalContentAlignment = HorizontalAlignment.Left;
                designer.SetBinding(TextBlock.TextProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(TextBlock.TextProperty);
            }));

            return defaultValue;
        }

        #endregion
    }

    public class PropertyDescriptorBuilderSelectedSpatialReference : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;
                var designer = new SelectedSpatialReference();
                designer.SetBinding(SelectedSpatialReference.SetReferenceProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(SelectedSpatialReference.SetReferenceProperty);

            }));

            return defaultValue;
        }

        #endregion 
    }

    public class PropertyDescriptorBuilderTextBox : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;
                var designer = new TextBox();
                //designer.ToolTip = "";
                designer.HorizontalAlignment = HorizontalAlignment.Stretch;
                designer.VerticalAlignment = VerticalAlignment.Center;
                designer.Height = 30;
                designer.VerticalContentAlignment = VerticalAlignment.Center;
                designer.HorizontalContentAlignment = HorizontalAlignment.Left;
                designer.SetBinding(TextBox.TextProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(TextBox.TextProperty);
            }));

            return defaultValue;
        }

        #endregion
    }


    public class PropertyDescriptorBuilderCheckbox : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;
                var designer = new CheckBox();
                //designer.ToolTip = "";
                designer.HorizontalAlignment = HorizontalAlignment.Stretch;
                designer.VerticalAlignment = VerticalAlignment.Center;
                designer.VerticalContentAlignment = VerticalAlignment.Center;
                designer.HorizontalContentAlignment = HorizontalAlignment.Left;
                designer.SetBinding(CheckBox.IsCheckedProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(CheckBox.IsCheckedProperty);
            }));

            return defaultValue;
        }

        #endregion
    }

    //public class PropertyDescriptorBuilderSpatialReference : PropertyDescriptorBuilder
    //{
    //    #region Methods

    //    public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
    //    {
    //        defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
    //        {
    //            var b = new Binding("Value");
    //            b.Source = defaultValue;
    //            b.Mode = BindingMode.TwoWay;
    //            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
    //            b.ValidatesOnDataErrors = true;
    //            var designer = new TextBlock();
    //            //designer.ToolTip = "";
    //            designer.HorizontalAlignment = HorizontalAlignment.Left;
    //            designer.VerticalAlignment = VerticalAlignment.Center;
    //            designer.SetBinding(TextBlock.TextProperty, b);

    //            defaultValue.Designer = designer;
    //            defaultValue.BindingExpression = designer.GetBindingExpression(TextBlock.TextProperty);
    //        }));

    //        return defaultValue;
    //    }

    //    #endregion
    //}

    //public class PropertyDescriptorBuilderMutilFileBrowser : PropertyDescriptorBuilder
    //{
    //    #region Methods

    //    public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
    //    {
    //        defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
    //        {
    //            var b = new Binding("Value");
    //            b.Source = defaultValue;
    //            b.Mode = BindingMode.TwoWay;
    //            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
    //            b.ValidatesOnDataErrors = true;

    //            var designer = new FileSelectTestBox();
    //            designer.Filter = "矢量文件(*.shp)|*.shp";
    //            designer.Multiselect = true;
    //            designer.IsReadOnly = true;
    //            designer.SetBinding(TextBox.TagProperty, b);

    //            defaultValue.Designer = designer;
    //        }));

    //        return defaultValue;
    //    }

    //    #endregion
    //}


    public class PropertyDescriptorBuilderList : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.ValidatesOnDataErrors = true;

                SplitFileBrowserTextBox sfbtb = new SplitFileBrowserTextBox() { IsReadOnly = true };
                List<string> fileList = defaultValue.Value as List<string>;
                if (fileList != null && fileList.Count > 0)
                {
                    sfbtb.SetFiles(fileList);
                }
                var designer = sfbtb;
                designer.SetBinding(SplitFileBrowserTextBox.SelectdFilesProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(SplitFileBrowserTextBox.SelectdFilesProperty);
            }));

            return defaultValue;
        }

        #endregion
    }
    public class PropertyBuilderCheckCardBoolean : PropertyDescriptorBuilder
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
                items["是"] = false;
                items["否"] = true;

                var dockPanel = new DockPanel();
                var cb = new MetroComboBox();
                cb.Padding = new Thickness(6, 4, 6, 5);
                cb.BorderThickness = new Thickness(1);
                cb.SelectedValuePath = "Value";
                cb.DisplayMemberPath = "Key";
                cb.ItemsSource = items;
                cb.SelectedIndex = 0;
                var b1 = new Binding("Value");
                b1.Source = defaultValue;
                cb.SetBinding(ComboBox.SelectedValueProperty, b1);

                dockPanel.Children.Add(cb);

                defaultValue.Designer = dockPanel;
            }));
            return defaultValue;
        }

        #endregion
    }


    /// <summary>
    /// 信息描述
    /// </summary>
    public class TypeDescrip
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Message { get; set; }
    }
}
