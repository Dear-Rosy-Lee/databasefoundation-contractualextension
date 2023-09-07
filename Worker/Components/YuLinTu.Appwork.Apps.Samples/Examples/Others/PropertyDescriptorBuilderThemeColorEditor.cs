using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class PropertyDescriptorBuilderThemeColorEditor : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;

                var designer = new ThemeColorSelectorComboBox();
                designer.Padding = new Thickness(6, 5, 6, 4);
                designer.SetBinding(ThemeColorSelectorComboBox.SelectedValueProperty, b);

                defaultValue.Designer = designer;
            }));

            return defaultValue;
        }

        #endregion
    }
}
