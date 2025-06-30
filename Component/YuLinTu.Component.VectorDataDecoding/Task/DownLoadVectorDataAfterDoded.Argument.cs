using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class DownLoadVectorDataAfterDodedArgument : TaskArgument
    {
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

        [DisplayLanguage("文件夹路径", IsLanguageName = false)]
        [DescriptionLanguage("选择存放已处理完成原始数据文件夹的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("选择存放已处理完成原始数据文件夹的路径")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                if (!resultFilePath.IsNullOrEmpty())
                {
                    ConfirmEnabled = true;
                }
                NotifyPropertyChanged("ResultFilePath");
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

        public DownLoadVectorDataAfterDodedArgument()
        {
        }

        #endregion
    }
}
