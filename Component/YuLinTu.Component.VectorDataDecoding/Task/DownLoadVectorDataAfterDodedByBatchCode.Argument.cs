using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class DownLoadVectorDataAfterDodedByBatchCodeArgument : TaskArgument
    {
 
        [DisplayLanguage("批次编码", IsLanguageName = false)]
        [DescriptionLanguage("批次编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string BatchCode
        {
            get { return batchCode; }
            set
            {
                batchCode = value;
                NotifyPropertyChanged("BatchCode");
            }
        }
        private string batchCode;

        [DisplayLanguage("批次名称", IsLanguageName = false)]
        [DescriptionLanguage("批次名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
       UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
      
      
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

        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("所在地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
       UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]

        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;

                NotifyPropertyChanged("ZoneCode");
            }
        }

        private string zoneCode;

        [DisplayLanguage("数据存放路径", IsLanguageName = false)]
        [DescriptionLanguage("数据存放文件夹路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
     UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("数据存放文件夹")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                //qualityCompressionDataSetDefine.ResultFilePath = ResultFilePath;
                if(resultFilePath?.Length>2)
                {
                    ConfirmEnabled = true;
                }
                NotifyPropertyChanged("ResultFilePath");
            }
        }
        private string resultFilePath;
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
        #region Ctor

        public DownLoadVectorDataAfterDodedByBatchCodeArgument()
        {
        }

        #endregion
    }
}
