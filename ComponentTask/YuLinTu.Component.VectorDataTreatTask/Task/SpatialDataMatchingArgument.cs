using Microsoft.Scripting.Actions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Component.Account.Models;
using YuLinTu.Windows.Wpf.Metro.Components;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace YuLinTu.Component.VectorDataTreatTask
{
    public class SpatialDataMatchingArgument : TaskArgument
    {
        #region Fields

        private string checkFilePath;
        private string resultFilePath;
        private string userName;
        private string zoneCode;
        #endregion Fields

        #region Properties

        [DisplayLanguage("用户名", IsLanguageName = false)]
        [DescriptionLanguage("鱼鳞图云用户", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderText), Editable = false,
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/account.png")]
        [WatermaskLanguage("请先登录鱼鳞图云账号")]
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                NotifyPropertyChanged("UserName");
            }
        }

        [DisplayLanguage("待处理数据路径", IsLanguageName = false)]
        [DescriptionLanguage("待处理数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        [WatermaskLanguage("请选择.shp矢量文件路径")]
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

        [DisplayLanguage("处理结果文件夹路径", IsLanguageName = false)]
        [DescriptionLanguage("存放处理文件夹的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
             UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("请在线加密数据存放文件夹")]
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

        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderText),Editable = true, 
     UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]

        [Required(AllowEmptyStrings = false, ErrorMessage = "地域代码必填")]
        [RegularExpression("^[1-9]\\d{11}$", ErrorMessage = "格式错误，请填写12位村级地域编码")]
        [WatermaskLanguage("请先登录鱼鳞图云账号")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                NotifyPropertyChanged("ZoneCode");
            }
        }

        [DisplayLanguage("自动压缩文件", IsLanguageName = false)]
        [DescriptionLanguage("处理完成自动压缩文件", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
             UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-zipper.png")]
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
            ZoneCode = AppGlobalSettings.Current.TryGetValue(Parameters.RegionName, "");// Parameters.Region.ToString();
            UserName = AppGlobalSettings.Current.TryGetValue(Parameters.UserName, "");

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
    public class PropertyDescriptorBuilderFolderBrowserExtsion : PropertyDescriptorBuilder
    {
      
      public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke((Action)delegate
            {
                Binding binding = new Binding("Value")
                {
                    Source = defaultValue,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                };
           
                FolderBrowserTextBox folderBrowserTextBox = new FolderBrowserTextBox();
                folderBrowserTextBox.Watermask = defaultValue.Watermask;
                folderBrowserTextBox.Description = defaultValue.Description;
                folderBrowserTextBox.SetBinding(TextBox.TextProperty, binding);
                folderBrowserTextBox.SetBinding(FrameworkElement.ToolTipProperty, binding);
                defaultValue.Designer = folderBrowserTextBox;
       
                defaultValue.BindingExpression = folderBrowserTextBox.GetBindingExpression(TextBox.TextProperty);
            }, new object[0]);
            return defaultValue;
        

        }
    }

    public class PropertyDescriptorBuilderTextBoxReadOnly : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var pi = defaultValue.Object.GetType().GetProperty(defaultValue.Name);

                var b = new Binding("Value")
                {
                    Source = defaultValue,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true
                };
                var designer = new MetroTextBox
                {
                    IsReadOnly =true,
                };

                var pda = pi.GetAttribute<PropertyDescriptorAttribute>();
                if (pda != null && pda.Converter != null)
                {
                    b.Converter = Activator.CreateInstance(pda.Converter) as IValueConverter;
                    b.ConverterParameter = new PropertyGridConverterParameterPair(defaultValue.PropertyGrid, pda.ConverterParameter);
                }
                designer.Watermask = defaultValue.Watermask;
                designer.SetBinding(MetroTextBox.TextProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(MetroTextBox.TextProperty);
            }));

            return defaultValue;
        }
    }

    public class PropertyDescriptorBuilderText : PropertyDescriptorBuilder
    {
        public override PropertyDescriptor Build(PropertyDescriptor defaultValue)
        {
            defaultValue.Designer.Dispatcher.Invoke(new Action(() =>
            {
                var pi = defaultValue.Object.GetType().GetProperty(defaultValue.Name);

                var b = new Binding("Value")
                {
                    Source = defaultValue,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true
                };
                var designer = new MetroTextBox
                {
                    IsReadOnly = !defaultValue.Editable,
                };

                var pda = pi.GetAttribute<PropertyDescriptorAttribute>();
                if (pda != null && pda.Converter != null)
                {
                    b.Converter = Activator.CreateInstance(pda.Converter) as IValueConverter;
                    b.ConverterParameter = new PropertyGridConverterParameterPair(defaultValue.PropertyGrid, pda.ConverterParameter);
                }
                designer.Watermask = defaultValue.Watermask;
                designer.SetBinding(MetroTextBox.TextProperty, b);

                defaultValue.Designer = designer;
                defaultValue.BindingExpression = designer.GetBindingExpression(MetroTextBox.TextProperty);
            }));

            return defaultValue;
        }
    }

}