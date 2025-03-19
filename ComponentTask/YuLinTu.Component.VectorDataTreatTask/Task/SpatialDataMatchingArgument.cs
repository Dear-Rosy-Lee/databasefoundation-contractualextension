using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataTreatTask
{
    public class SpatialDataMatchingArgument : TaskArgument
    {
        #region Fields

        private string checkFilePath;
        private string resultFilePath;

        #endregion Fields

        #region Properties

        [DisplayLanguage("待处理数据路径", IsLanguageName = false)]
        [DescriptionLanguage("待处理数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string CheckFilePath
        {
            get { return checkFilePath; }
            set
            {
                checkFilePath = value;
                //qualityCompressionDataSetDefine.CheckFilePath = CheckFilePath;
                NotifyPropertyChanged("CheckFilePath");
            }
        }

        [DisplayLanguage("处理结果文件路径", IsLanguageName = false)]
        [DescriptionLanguage("存放处理文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
             UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                //qualityCompressionDataSetDefine.ResultFilePath = ResultFilePath;
                NotifyPropertyChanged("ResultFilePath");
            }
        }

        [DisplayLanguage("自动压缩文件", IsLanguageName = false)]
        [DescriptionLanguage("处理完成自动压缩文件", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
             UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool AutoComprass
        {
            get { return autoComprass; }
            set
            {
                autoComprass = value;
                NotifyPropertyChanged("AutoComprass");
            }
        }

        private bool autoComprass;

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SpatialDataMatchingArgument()
        {
            checkFilePath = "";
            resultFilePath = "";
        }

        #endregion Ctor
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

}