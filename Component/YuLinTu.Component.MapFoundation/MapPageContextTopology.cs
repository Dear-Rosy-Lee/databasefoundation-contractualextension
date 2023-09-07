using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using YuLinTu.tGIS.Client;
using YuLinTu.Appwork;
using YuLinTu.Data;
using System.Windows.Data;

using YuLinTu.Components.tGIS;
using YuLinTu.Library.Entity;
using YuLinTu.Component.MapFoundation.Configuration;


namespace YuLinTu.Component.MapFoundation
{
    [Workpage(typeof(YuLinTuMapFoundation))]
    internal class MapPageContextTopology : TheWorkpageMessageHandler,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent
    {
        #region Fields

        private MenuItem menuTopologyZone = null;

        private List<ProTable> tableList = PropertyTableDefine.DeserializeProTable();
        private ProTable table = null;      

        #endregion

        #region Ctor

        public MapPageContextTopology(IWorkpage workpage)
            : base(workpage)
        {
           
        }

        #endregion

        #region Methods

        #region Methods - Message Handler

        /// <summary>
        /// 模块配置
        /// </summary>
        [MessageHandler(ID = EdCore.langInstallWorkpageOptionsEditor)]
        private void langInstallWorkpageOptionsEditor(object sender, InstallWorkpageOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "自定义设置",
                    Editor = new MapFoundationUserConfigPage(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "公示图设置",
                    Editor = new DiagramFoundationCommonConfigPage(Workpage),
                });
            }));
        }


        [MessageHandler(ID = IDMap.InstallToobarInQueryContainer)]
        private void InstallToobarInQueryContainer(object sender, InstallUIElementsEventArgs e)
        {

        }

        [MessageHandler(ID = IDMap.InstallCreateTopologyTaskMenu)]
        private void InstallCreateTopologyTaskMenu(object sender, InstallUIElementsEventArgs e)
        {
            var shell = new TopologyMenuShell();
            menuTopologyZone = shell.Content as MenuItem;
            shell.Content = null;

            e.Items.Add(menuTopologyZone);
            menuTopologyZone.Click += MenuTopologyZone_Click;
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            if (menuTopologyZone != null)
            {
                menuTopologyZone.Click -= MenuTopologyZone_Click;
                menuTopologyZone = null;
            }
        }

        #endregion

        #region Methods - Events

        private void MenuTopologyZone_Click(object sender, RoutedEventArgs e)
        {
            var zone = Workpage.Workspace.Properties.TryGetValue<Zone>("CurrentZone", null);
            if (zone == null)
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    Message = "未选择行政地域，无法进行拓扑检查。",
                    Header = "拓扑检查"
                });

                return;
            }
            if (zone.Shape == null || zone.Shape.IsEmpty())
            {
                Workpage.Page.ShowDialog(new MessageDialog()
                {
                    Message = string.Format("{0} 不存在空间数据，无法进行拓扑检查。", zone.FullName),
                    Header = "拓扑检查"
                });

                return;
            }

            Workpage.Message.Send(this, new MsgEventArgs<
                YuLinTu.Spatial.Geometry>(IDMap.RequestCreateTopologyTask)
            { Parameter = zone.Shape });
        }


        #endregion

        #region Methods - Private - 属性编辑框修改列名为中文名

        /// <summary>
        /// 编辑前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.RequestInstallOldGraphicPropertyEditor)]
        private void RequestInstallOldGraphicPropertyEditor(object sender, MsgEventArgs<InitializePropertyDescriptorEventArgs> e)
        {
            var graphic = e.Tag as Graphic;
            if (graphic == null)
                return;
            var vl = graphic.Layer as VectorLayer;
            if (vl == null || vl.DataSource == null)
                return;
            var layername = vl.DataSource.ElementName;
            EditPropertyName(layername, e);
        }

        /// <summary>
        /// 绘制前
        /// </summary>
        [MessageHandler(ID = IDMap.RequestInstallNewGraphicPropertyEditor)]
        private void RequestInstallNewGraphicPropertyEditor(object sender, MsgEventArgs<InitializePropertyDescriptorEventArgs> e)
        {
            var vl = e.Tag as VectorLayer;
            if (vl == null || vl.DataSource == null)
                return;
            var layername = vl.DataSource.ElementName;
            EditPropertyName(layername, e);
        }

        /// <summary>
        /// 编辑列名
        /// </summary>
        /// <param name="e"></param>
        private void EditPropertyName(string layername, MsgEventArgs<InitializePropertyDescriptorEventArgs> e)
        {
            string tableName = layername;
            if (table == null || !tableName.Equals(table.TableName))
                table = tableList.Find(c => c.TableName.Equals(tableName));
            if (table == null)
                return;
           
            string columnName = e.Parameter.PropertyDescriptor.Name;
            SetField field = table.FieldList.Find(c => c.FieldName.Equals(columnName));

            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                if (field == null)
                {
                    e.Parameter.PropertyDescriptor.Visibility = Visibility.Collapsed;
                    return;
                }
                e.Parameter.PropertyDescriptor.AliasName = field.AliseName;
                             
                if (field.IsVisible)
                    e.Parameter.PropertyDescriptor.Visibility = Visibility.Visible;
                else
                    e.Parameter.PropertyDescriptor.Visibility = Visibility.Collapsed;

                if (!field.IsEdit)
                {
                    var b = new Binding("Value");
                    b.Source = e.Parameter.PropertyDescriptor;
                    b.Mode = BindingMode.TwoWay;
                    b.Converter = new SuperTypeConverter() { ConvertType = typeof(string), ConvertBackType = typeof(int) };
                    b.ValidatesOnExceptions = true;
                    b.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;

                    var tb = new MetroTextBox();
                    tb.IsReadOnly = true;
                    tb.PaddingWatermask = new Thickness(5, 0, 0, 0);
                    tb.VerticalContentAlignment = VerticalAlignment.Center;
                    tb.SetBinding(TextBox.ToolTipProperty, b);
                    tb.SetBinding(TextBox.TextProperty, b);

                    e.Parameter.PropertyDescriptor.Editable = false;
                    e.Parameter.PropertyDescriptor.Designer = tb;
                    e.Parameter.PropertyDescriptor.BindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
                }
            }));
        }

        #endregion

        #region Methods - Private - 属性表字段设置
        /// <summary>
        /// 陈泽林 20170110 通过读取配置文件进行设置属性表的相关属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = IDMap.InstallLayerPropertiesDataGridColumn)]
        private void InstallLayerPropertiesDataGridColumn(object sender, MsgEventArgs<InitializeDataGridColumnEventArgs> e)
        {
            var vl = e.Tag as VectorLayer;
            if (vl == null || vl.DataSource == null)
                return;
            //List<ProTable> tableList = PropertyTableDefine.DeserializeProTable();
            if (tableList == null)
                return;

            string tableName = vl.DataSource.ElementName;
            if (table == null || !tableName.Equals(table.TableName))
                table = tableList.Find(c => c.TableName.Equals(tableName));
            if (table == null)
                return;

            string columnName = e.Parameter.Property.ColumnName;
            SetField field = table.FieldList.Find(c => c.FieldName.Equals(columnName));
            //if (field == null)
            //    return;

            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                if (field == null)
                {
                    e.Parameter.Column.Visibility = Visibility.Collapsed;
                    return;
                }
                e.Parameter.Column.Header = field.AliseName;
                e.Parameter.Column.IsReadOnly = !field.IsEdit;
                if (field.IsVisible)
                    e.Parameter.Column.Visibility = Visibility.Visible;
                else
                    e.Parameter.Column.Visibility = Visibility.Collapsed;
            }));
        }

        #endregion

        #endregion
    }
}
