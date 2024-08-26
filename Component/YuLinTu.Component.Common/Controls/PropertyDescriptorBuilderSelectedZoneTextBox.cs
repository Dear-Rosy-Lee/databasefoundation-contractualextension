/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
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
using YuLinTu.Library.Business;
using YuLinTu.Data.SQLite;
using YuLinTu.Data;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// 自定义控件(设计器)
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedZoneTextBox : PropertyDescriptorBuilder
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

                var zoneTextBox = new SelectedZoneTextBox();
                zoneTextBox.IsReadOnly = true;
                zoneTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                //zoneTextBox.Filter = LanguageAttribute.GetLanguage("lang11034");
                zoneTextBox.SetBinding(SelectedZoneTextBox.FullZoneNameAndCodeProperty, bind);

                defaultValue.Designer = zoneTextBox;
            }));
            return defaultValue;
        }

        #endregion Method
    }

    /// <summary>
    /// 自定义控件(设计器)选地域和人
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedZoneAndVPTextBox : PropertyDescriptorBuilder
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
                var dockPanel = new DockPanel();

                var bind = new Binding("Value");
                bind.Source = defaultValue;
                bind.Mode = BindingMode.TwoWay;

                var zoneTextBox = new SelectedSummaryExportZoneTB();
                zoneTextBox.IsReadOnly = true;
                zoneTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                zoneTextBox.SetBinding(SelectedSummaryExportZoneTB.SelectZoneAndPersonInfoProperty, bind);
                zoneTextBox.Visibility = Visibility.Collapsed;

                //var vpTextBox = new SelectedVirtualPersonsTextBox();
                //vpTextBox.IsReadOnly = true;
                //vpTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                //vpTextBox.SetBinding(SelectedVirtualPersonsTextBox.SelectZoneAndPersonInfoProperty, bind);
                //vpTextBox.Visibility = Visibility.Collapsed;

                //var grid = new Grid();
                //grid.Children.Add(zoneTextBox);
                //grid.Children.Add(vpTextBox);
                //MetroTextBox mb = new MetroTextBox();
                //mb.ContentRight = grid;

                defaultValue.Designer = zoneTextBox;
            }));
            return defaultValue;
        }

        #endregion Method
    }

    /// <summary>
    /// 自定义控件
    /// 对应对象实体类属性为bool型
    /// </summary>
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

    /// <summary>
    /// 自定义控件(设计器)
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedFileTextBoxShp : PropertyDescriptorBuilder
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

                var fileTextBox = new SelectedFileTextBox() { Filter = "文件类型(*.shp)|*.shp" };
                fileTextBox.IsReadOnly = true;
                fileTextBox.Visibility = Visibility.Visible;
                fileTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                fileTextBox.SetBinding(SelectedFileTextBox.FileNameProperty, bind);
                var grid = new Grid();
                grid.Children.Add(fileTextBox);

                defaultValue.Designer = grid;
            }));
            return defaultValue;
        }

        #endregion Method
    }


    /// <summary>
    /// 自定义控件(设计器)
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedFileTextBoxSqlite : PropertyDescriptorBuilder
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

                var fileTextBox = new SelectedFileTextBox() { Filter = "文件类型(*.sqlite)|*.sqlite" };
                fileTextBox.IsReadOnly = true;
                fileTextBox.Visibility = Visibility.Visible;
                fileTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                fileTextBox.SetBinding(SelectedFileTextBox.FileNameProperty, bind);
                var grid = new Grid();
                grid.Children.Add(fileTextBox);

                defaultValue.Designer = grid;
            }));
            return defaultValue;
        }

        #endregion Method
    }


    /// <summary>
    /// 自定义控件(设计器)
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedMdb : PropertyDescriptorBuilder
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

                var fileTextBox = new SelectedFileTextBox() { Filter = "文件类型(*.mdb)|*.mdb" };
                fileTextBox.IsReadOnly = true;
                fileTextBox.Visibility = Visibility.Visible;
                fileTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                fileTextBox.SetBinding(SelectedFileTextBox.FileNameProperty, bind);
                var grid = new Grid();
                grid.Children.Add(fileTextBox);

                defaultValue.Designer = grid;
            }));
            return defaultValue;
        }

        #endregion Method
    }

    /// <summary>
    /// 自定义控件(设计器)
    /// 对应对象实体类属性为 string 型
    /// </summary>
    public class PropertyDescriptorBuilderSelectedFolderTextBox : PropertyDescriptorBuilder
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

                var folderTextBox = new SelectedFolderTextBox();
                folderTextBox.IsReadOnly = true;
                folderTextBox.Visibility = Visibility.Visible;
                folderTextBox.WorkPage = defaultValue.PropertyGrid.Properties["Workpage"] as IWorkpage;
                folderTextBox.SetBinding(SelectedFolderTextBox.FolderNameProperty, bind);

                defaultValue.Designer = folderTextBox;
            }));
            return defaultValue;
        }

        #endregion Method
    }

    /// <summary>
    /// 行政地域触发器
    /// </summary>
    public class PropertyTriggerZone : PropertyTrigger
    {
        #region Methods

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "DatabaseFilePath")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var tb = pd.Designer as SelectedZoneTextBox;
                    if (tb == null)
                        return;
                    try
                    {
                        var path = pd.Object.GetPropertyValue<string>("DatabaseFilePath");
                        if (path.IsNullOrEmpty())
                            return;

                        tb.DbContext = DataBaseSource.GetDataBaseSourceByPath(path.ToString());
                    }
                    catch { }
                }));
            }
            else if (propertyName == "IsBatchCombination")
            {
                var isBatch = pd.Object.GetPropertyValue<bool>("IsBatchCombination");
                pd.Visibility = isBatch ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// 参数值改变
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="propertyName"></param>
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "DatabaseFilePath")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var tb = pd.Designer as SelectedZoneTextBox;
                    if (tb == null)
                        return;
                    try
                    {
                        var path = pd.Object.GetPropertyValue<string>("DatabaseFilePath");
                        tb.DbContext = DataBaseSource.GetDataBaseSourceByPath(path.ToString());
                    }
                    catch { }
                }));
            }
            else if (propertyName == "IsBatchCombination")
            {
                var isBatch = pd.Object.GetPropertyValue<bool>("IsBatchCombination");
                pd.Visibility = isBatch ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// 文件路径触发器
    /// </summary>
    public class PropertyTriggerFile : PropertyTrigger
    {
        #region Methods

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "IsBatchCombination")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var grid = pd.Designer as Grid;
                    if (grid == null)
                        return;
                    try
                    {
                        var isBatch = pd.Object.GetPropertyValue<bool>("IsBatchCombination");
                        /*if (isBatch) 2024年8月23  这里不清楚有什么左右，又引起了界面上bug 先注释了if (isBatch)
                        {
                            grid.Children[0].Visibility = Visibility.Collapsed;
                            grid.Children[1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            grid.Children[0].Visibility = Visibility.Visible;
                            grid.Children[1].Visibility = Visibility.Collapsed;
                        }*/
                        //var sft0 = grid.Children[0] as SelectedFileTextBox;
                        //sft0.Text = "";
                        //var sft1 = grid.Children[1] as SelectedFolderTextBox;
                        //sft1.Text = "";
                    }
                    catch { }
                }));
            }
        }

        /// <summary>
        /// 参数值改变
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="propertyName"></param>
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "IsBatchCombination")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var grid = pd.Designer as Grid;
                    if (grid == null)
                        return;
                    try
                    {
                        var isBatch = pd.Object.GetPropertyValue<bool>("IsBatchCombination");
                        /*if (isBatch) 2024年8月23  这里不清楚有什么左右，又引起了界面上bug 先注释了if (isBatch)
                        {
                            grid.Children[0].Visibility = Visibility.Collapsed;
                            grid.Children[1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            grid.Children[0].Visibility = Visibility.Visible;
                            grid.Children[1].Visibility = Visibility.Collapsed;
                        }*/
                        var sft0 = grid.Children[0] as SelectedFileTextBox;
                        sft0.Text = "";
                        //var sft1 = grid.Children[1] as SelectedFolderTextBox;
                        //sft1.Text = "";
                    }
                    catch { }
                }));
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// 选择地域相应选择承包方触发器
    /// </summary>
    public class PropertyTriggerZoneAndVP : PropertyTrigger
    {
        #region Methods

        public override void OnPropertyValueInstalled(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "SelectZoneAndPersonInfo")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var grid = pd.Designer as Grid;
                    if (grid == null)
                        return;
                    try
                    {
                        var szvpinfo = pd.Object.GetPropertyValue<SelectedZoneAndPersonInfo>("SelectZoneAndPersonInfo");
                        string[] nameAndCode = szvpinfo.ZoneNameAndCode.IsNullOrBlank() ? null : szvpinfo.ZoneNameAndCode.Split('#');
                        var zonecode = nameAndCode == null ? string.Empty : nameAndCode[1];
                        if (string.IsNullOrEmpty(zonecode)) return;
                        var dbcontext = DataBaseSource.GetDataBaseSource();
                        var zoneStation = dbcontext.CreateZoneWorkStation();
                        var zone = zoneStation.Get(zonecode);
                        if (zone.Level == Library.Entity.eZoneLevel.Group)
                        {
                            grid.Children[0].Visibility = Visibility.Collapsed;
                            grid.Children[1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            grid.Children[0].Visibility = Visibility.Visible;
                            grid.Children[1].Visibility = Visibility.Collapsed;
                        }
                    }
                    catch { }
                }));
            }
        }

        /// <summary>
        /// 参数值改变
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="propertyName"></param>
        public override void OnPropertyValueChanged(PropertyDescriptor pd, string propertyName)
        {
            if (propertyName == "SelectZoneAndPersonInfo")
            {
                pd.Designer.Dispatcher.Invoke(new Action(() =>
                {
                    var grid = pd.Designer as Grid;
                    if (grid == null)
                        return;
                    try
                    {
                        var szvpinfo = pd.Object.GetPropertyValue<SelectedZoneAndPersonInfo>("SelectZoneAndPersonInfo");
                        string[] nameAndCode = szvpinfo.ZoneNameAndCode.IsNullOrBlank() ? null : szvpinfo.ZoneNameAndCode.Split('#');
                        var zonecode = nameAndCode == null ? string.Empty : nameAndCode[1];
                        if (string.IsNullOrEmpty(zonecode)) return;
                        var dbcontext = DataBaseSource.GetDataBaseSource();
                        var zoneStation = dbcontext.CreateZoneWorkStation();
                        var zone = zoneStation.Get(zonecode);
                        if (zone.Level == Library.Entity.eZoneLevel.Group)
                        {
                            grid.Children[0].Visibility = Visibility.Collapsed;
                            grid.Children[1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            grid.Children[0].Visibility = Visibility.Visible;
                            grid.Children[1].Visibility = Visibility.Collapsed;
                        }
                    }
                    catch { }
                }));
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// 选定地域及对应承包方集合信息
    /// </summary>
    public class SelectedZoneAndPersonInfo
    {
        /// <summary>
        /// 地域信息-地域名称及代码
        /// </summary>
        public string ZoneNameAndCode { set; get; }

        /// <summary>
        /// 选定人
        /// </summary>
        public List<PersonSelectedInfo> SelectedPersons { set; get; }
    }
}