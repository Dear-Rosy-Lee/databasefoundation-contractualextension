using GeoAPI.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.DF.Enums;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class CreateVectorDecBatchTaskArgument : TaskArgument
    {
        #region Properties

        private string zoneName;

        [DisplayLanguage("地域名称", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        

        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                NotifyPropertyChanged("ZoneName");
            }
        }



        private string zoneCode;

        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
            
                NotifyPropertyChanged("ZoneCode");
            }
        }

        [DisplayLanguage("批次名称", IsLanguageName = false)]
        [DescriptionLanguage("待处理批次名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderTextBoxCustom),
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        [Required]
        [WatermaskLanguage("请命名批次名称，如XXX村承包地")]
        public string BatchName
        {
            get { return _BatchName; }
            set
            {
                _BatchName = value;
                if (_BatchName.IsNotNullOrEmpty() && _BatchName.Length > 3)
                {
                    ConfirmEnabled = true;
                }
                else
                {
                    ConfirmEnabled = false;
                }
                    NotifyPropertyChanged("BatchName");
            }
        }
        private string _BatchName;

        [Enabled(false)]
        public bool ConfirmEnabled
        {
            get { return checkPass; }
            set
            {
                checkPass = value;
                NotifyPropertyChanged("ConfirmEnabled");
            }
        }
        public bool checkPass = false;

        #endregion

        #region Ctor

        public CreateVectorDecBatchTaskArgument()
        {
            
        }

        #endregion
    }
}
