/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows.Data;
using System.Windows.Controls;
using YuLinTu.Windows;
using System.Windows;

namespace YuLinTu.Component.Setting
{  
        /// <summary>
        /// 自定义控件(设计器)
        /// 对应对象实体类属性为 string 型
        /// </summary>
        public class PropertyDescriptorBuilderSelectedSRBox : PropertyDescriptorBuilder
        {
            #region Method

            /// <summary>
            /// 自定义textbox控件，通过defaultValue绑定控件，textbox的值是绑定的属性Value 
            /// 其实这个Value就是对象的某个属性值
            /// </summary>
            public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
            {
                defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var bind = new Binding("Value");
                    bind.Source = defaultValue;
                    bind.Mode = BindingMode.TwoWay;

                    var srTextBox = new SelectedSpatialReferenceTextBox();
                    srTextBox.IsReadOnly = true;
                    srTextBox.WorkSpace = defaultValue.PropertyGrid.Properties["workSpace"] as IWorkspace;

                    srTextBox.SetBinding(SelectedSpatialReferenceTextBox.NowSpatialReferenceProperty, bind);
                    defaultValue.Designer = srTextBox;
                }));
                return defaultValue;
            }

            #endregion
        }

}
