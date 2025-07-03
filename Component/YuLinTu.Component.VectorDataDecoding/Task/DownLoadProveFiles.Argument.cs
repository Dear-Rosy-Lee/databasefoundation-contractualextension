using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class DownLoadProveFilesArgument : TaskArgument
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


        [DisplayLanguage("证明文件存放路径", IsLanguageName = false)]
        [DescriptionLanguage("证明文件存放路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
     UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("请选择证明文件存放路径")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                //qualityCompressionDataSetDefine.ResultFilePath = ResultFilePath;
                if (resultFilePath.Length > 3)
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
        public bool checkPass;
        #endregion

        #region Ctor

        public DownLoadProveFilesArgument()
        {
        }

        #endregion
    }
}
