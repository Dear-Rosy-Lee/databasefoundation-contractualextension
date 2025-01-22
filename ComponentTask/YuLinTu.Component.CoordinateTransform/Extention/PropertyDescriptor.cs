using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class PropertyDescriptorSelectShapeFile : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                Binding binding = new Binding("Value");
                binding.Source = (object)defaultValue;
                binding.Mode = BindingMode.TwoWay;
                binding.ValidatesOnExceptions = true;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                FileBrowserTextBox fileBrowserTextBox = new FileBrowserTextBox();
                fileBrowserTextBox.Filter = "Shp文件|*.shp";
                fileBrowserTextBox.SetBinding(TextBox.TextProperty, (BindingBase)binding);
                fileBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, (BindingBase)binding);
                defaultValue.Designer = (UIElement)fileBrowserTextBox;
                defaultValue.BindingExpression = (BindingExpressionBase)fileBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }));
            return defaultValue;
        }
    }

    public class PropertyDescriptorSelectPrjFile : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                Binding binding = new Binding("Value");
                binding.Source = (object)defaultValue;
                binding.Mode = BindingMode.TwoWay;
                binding.ValidatesOnExceptions = true;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                FileBrowserTextBox fileBrowserTextBox = new FileBrowserTextBox();
                fileBrowserTextBox.Filter = "Prj文件|*.prj";
                fileBrowserTextBox.SetBinding(TextBox.TextProperty, (BindingBase)binding);
                fileBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, (BindingBase)binding);
                defaultValue.Designer = (UIElement)fileBrowserTextBox;
                defaultValue.BindingExpression = (BindingExpressionBase)fileBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }));
            return defaultValue;
        }
    }

    public class PropertyDescriptorSelectRasterDataFile : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                Binding binding = new Binding("Value");
                binding.Source = (object)defaultValue;
                binding.Mode = BindingMode.TwoWay;
                binding.ValidatesOnExceptions = true;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                FileBrowserTextBox fileBrowserTextBox = new FileBrowserTextBox();
                fileBrowserTextBox.Filter = "Tif文件|*.tif";
                fileBrowserTextBox.SetBinding(TextBox.TextProperty, (BindingBase)binding);
                fileBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, (BindingBase)binding);
                defaultValue.Designer = (UIElement)fileBrowserTextBox;
                defaultValue.BindingExpression = (BindingExpressionBase)fileBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }));
            return defaultValue;
        }
    }

    public class PropertyDescriptorSaveRasterDataFile : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                Binding binding = new Binding("Value");
                binding.Source = (object)defaultValue;
                binding.Mode = BindingMode.TwoWay;
                binding.ValidatesOnExceptions = true;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                SaveFileBrowserTextBox fileBrowserTextBox = new SaveFileBrowserTextBox();
                fileBrowserTextBox.Filter = "Tif文件|*.tif";
                fileBrowserTextBox.SetBinding(TextBox.TextProperty, (BindingBase)binding);
                fileBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, (BindingBase)binding);
                defaultValue.Designer = (UIElement)fileBrowserTextBox;
                defaultValue.BindingExpression = (BindingExpressionBase)fileBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }));
            return defaultValue;
        }
    }
}
