/*
 * (C) 2016  公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.IO;
using System.Windows;
using YuLinTu.Windows;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 文件选择框
    /// </summary>
    public class PropertyDescriptorBuilderFileBrowser : PropertyDescriptorBuilder
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

                var fileTextBox = new SelectedFileTextBox();
                fileTextBox.IsReadOnly = true;
                fileTextBox.Visibility = Visibility.Visible;
                fileTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                fileTextBox.SetBinding(SelectedFileTextBox.FileNameProperty, b);

                defaultValue.Designer = fileTextBox;
            }));

            return defaultValue;
        }

        #endregion
    }

}
