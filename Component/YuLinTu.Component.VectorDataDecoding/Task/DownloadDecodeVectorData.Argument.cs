using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class DownloadDecodeVectorDataByBatchCodeArgument : TaskArgument
    {
      

        #region Properties
        [DisplayLanguage("处理批次编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理批次号", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
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
        private string resultFilePath;

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
        #endregion

        #region Ctor

        public DownloadDecodeVectorDataByBatchCodeArgument()
        {
        }

        #endregion
    }
}
