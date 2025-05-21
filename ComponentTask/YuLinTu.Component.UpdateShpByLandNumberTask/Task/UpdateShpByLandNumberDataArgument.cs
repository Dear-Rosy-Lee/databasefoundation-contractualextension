using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    public class UpdateShpByLandNumberDataArgument : TaskArgument
    {
        #region Fields

        private string updateFilePath;
        private string resultFilePath;

        #endregion Fields

        #region Properties

        [DisplayLanguage("上传更新图斑", IsLanguageName = false)]
        [DescriptionLanguage("上传更新图斑文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string UpdateFilePath
        {
            get { return updateFilePath; }
            set
            {
                updateFilePath = value;
                NotifyPropertyChanged("UpdateFilePath");
            }
        }

        [DisplayLanguage("更新结果存放路径", IsLanguageName = false)]
        [DescriptionLanguage("更新结果存放的路径", IsLanguageName = false)]
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
        public UpdateShpByLandNumberDataArgument()
        {
            updateFilePath = "";
            resultFilePath = "";
        }
        #endregion Ctor

    }

}