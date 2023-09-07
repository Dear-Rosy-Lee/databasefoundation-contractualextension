using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public class PropertyDescriptorBuilderSaveFileBrowserSQLite : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;

                var designer = new SaveFileBrowserTextBox();
                designer.Filter = LanguageAttribute.GetLanguage("lang11034");
                designer.SetBinding(TextBox.TextProperty, b);
                designer.SetBinding(TextBox.ToolTipProperty, b);

                defaultValue.Designer = designer;
            }));

            return defaultValue;
        }

        #endregion
    }
}
