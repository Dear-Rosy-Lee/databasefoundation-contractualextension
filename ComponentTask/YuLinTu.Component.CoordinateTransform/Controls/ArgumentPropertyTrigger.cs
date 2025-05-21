using System;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class ArgumentPropertyTrigger : PropertyTrigger
    {
        /// <summary>
        /// 参数值改变
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="propertyName"></param>
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
        }
    }

    public class ChangeWayTrigger : PropertyTrigger
    {
        private System.Windows.Visibility vis = System.Windows.Visibility.Visible;
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "EnabledCustomArgs")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();
                    vis = Convert.ToBoolean(value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

                    if (pd.Name == "PyX" || pd.Name == "PyY" || pd.Name == "RotateAngleT" || pd.Name == "Scale")
                        pd.Visibility = vis;
                }));
            }
        }

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName != "EnabledCustomArgs")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var v = pd.Object.GetPropertyValue(propertyName);
                    if (v == null)
                        return;
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();

                    if (pd.Name == "PyX" || pd.Name == "PyY" || pd.Name == "RotateAngleT" || pd.Name == "Scale")
                        pd.Visibility = vis;
                }));
            }
            else
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();
                    vis = Convert.ToBoolean(value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }));
            }
        }
    }

    public class ArchiveControlTrigger : PropertyTrigger
    {
        private System.Windows.Visibility vis = System.Windows.Visibility.Collapsed;
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "IsReChangeCoord")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();
                    vis = Convert.ToBoolean(value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

                    if (pd.Name == "ChangeDestinationPrj")
                        pd.Visibility = vis;
                }));
            }
        }

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "IsReChangeCoord")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var v = pd.Object.GetPropertyValue(propertyName);
                    if (v == null)
                        return;
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();

                    if (pd.Name == "ChangeDestinationPrj")
                        pd.Visibility = vis;
                }));
            }
            else
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var v = pd.Object.GetPropertyValue(propertyName);
                    if (v == null)
                        return;
                    var value = pd.Object.GetPropertyValue(propertyName).ToString();
                    vis = Convert.ToBoolean(value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }));
            }
        }
    }
}
