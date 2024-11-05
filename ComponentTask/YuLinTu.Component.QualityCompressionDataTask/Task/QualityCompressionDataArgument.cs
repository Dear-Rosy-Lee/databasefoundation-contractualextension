using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    public class QualityCompressionDataArgument : TaskArgument
    {
        #region Fields

        private string checkFilePath;
        private string resultFilePath;

        #endregion Fields

        #region Properties

        [DisplayLanguage("待检数据路径", IsLanguageName = false)]
        [DescriptionLanguage("待检查数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
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

        [DisplayLanguage("压缩文件路径", IsLanguageName = false)]
        [DescriptionLanguage("存放压缩文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
             UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
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

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public QualityCompressionDataArgument()
        {
            checkFilePath = "";
            resultFilePath = "";
            //读出配置
            //center = TheApp.Current.GetSystemSettingsProfileCenter();
            //var profile = center.GetProfile<QualityCompressionDataSetDefine>();
            //var section = profile.GetSection<QualityCompressionDataSetDefine>();
            //CheckFilePath = qualityCompressionDataSetDefine.CheckFilePath;
            //ResultFilePath = qualityCompressionDataSetDefine.ResultFilePath;
        }

        #endregion Ctor
    }
}