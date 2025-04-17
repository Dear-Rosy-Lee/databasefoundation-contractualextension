using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataTreatTask
{
    public class VectorDataUpdateArgument : TaskArgument
    {
        #region Fields

        private string checkFilePath;
        private string resultFilePath;

        #endregion Fields

        #region Properties

        [DisplayLanguage("地块数据路径", IsLanguageName = false)]
        [DescriptionLanguage("待处理地块数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
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

        [DisplayLanguage("使用确权地块编码挂接", IsLanguageName = false)]
        [DescriptionLanguage("使用确权地块编码挂接", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool UseOldNumber
        {
            get { return useOldNumber; }
            set
            {
                useOldNumber = value;
                NotifyPropertyChanged("UseOldNumber");
            }
        }
        private bool useOldNumber;

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public VectorDataUpdateArgument()
        {
            checkFilePath = "";
            resultFilePath = "";
        }

        #endregion Ctor
    }
}