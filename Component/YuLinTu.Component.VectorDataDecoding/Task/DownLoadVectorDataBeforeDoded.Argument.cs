using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class DownLoadVectorDataBeforeDodedArgument : TaskArgument
    {
        #region Properties
        private string zoneName;

        [DisplayLanguage("地域名称", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]

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

        [DisplayLanguage("待脱密数据存放文件夹", IsLanguageName = false)]
        [DescriptionLanguage("存放待脱密原始数据文件夹的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("请选择待脱密数据存放文件夹")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                if(!resultFilePath.IsNullOrEmpty())
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
        #endregion

        #region Ctor

        public DownLoadVectorDataBeforeDodedArgument()
        {
        }

        #endregion
    }
}
