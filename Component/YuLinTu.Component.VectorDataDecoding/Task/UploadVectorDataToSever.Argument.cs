using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.DF.Enums;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class UploadVectorDataToSeverArgument : TaskArgument
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

        [DisplayLanguage("地块数量", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据地块数量", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
              UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public int? LandCount
        {
            get { return landCount; }
            set
            {
                landCount = value;
                NotifyPropertyChanged("LandCount");
            }
        }
        private int? landCount;
        #endregion

        #region Ctor

        public UploadVectorDataToSeverArgument()
        {
        }

        #endregion
    }
}
