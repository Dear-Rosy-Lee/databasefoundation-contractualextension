using System;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataLinkageTask
{
    public class PropertyDescriptorBuilderFileBrowserShp : PropertyDescriptorBuilder
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
}
