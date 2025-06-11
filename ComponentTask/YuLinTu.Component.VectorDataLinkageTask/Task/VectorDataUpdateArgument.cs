using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataLinkageTask
{
    public class VectorDataLinkageArgument : TaskArgument
    {
        #region Fields

        private string checkFilePath;
        private string resultFilePath;

        #endregion Fields

        #region Properties

        [DisplayLanguage("Shape文件路径", IsLanguageName = false)]
        [DescriptionLanguage("待上传的地块数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string CheckFilePath
        {
            get { return checkFilePath; }
            set
            {
                checkFilePath = value;
                NotifyPropertyChanged("CheckFilePath");
            }
        }

        [DisplayLanguage("日志记录", IsLanguageName = false)]
        [DescriptionLanguage("存放上传日志的目录", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                NotifyPropertyChanged("ResultFilePath");
            }
        }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public VectorDataLinkageArgument()
        {
            checkFilePath = "";
            resultFilePath = "";
        }

        #endregion Ctor
    }
}