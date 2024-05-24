/*
 * (C) 2014  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    public class PropertyDescriptorBuilderTest : PropertyDescriptorBuilder
    {
        #region Methods

        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var b = new Binding("Value");
                b.Source = defaultValue;
                b.Mode = BindingMode.TwoWay;
                var designer = new FileChoiceControl();
                designer.SetBinding(FileChoiceControl.MetaDataProperty, b);

                var address = new Binding("ImportFilePath");
                address.Source = defaultValue.Object;
                address.Mode = BindingMode.TwoWay;
                designer.SetBinding(FileChoiceControl.ImportFilePathProperty, address);

                defaultValue.Designer = designer;
            }));

            return defaultValue;
        }

        #endregion
    }
}
