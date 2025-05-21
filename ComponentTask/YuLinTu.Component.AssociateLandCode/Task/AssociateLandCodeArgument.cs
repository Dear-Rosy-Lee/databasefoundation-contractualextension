using YuLinTu.Component.Common;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.AssociateLandCode
{
    public class AssociateLandCodeArgument : TaskArgument
    {
        #region Fields

       
        private string databaseFilePath;   //SQLite数据库路径
        private string olddatabaseFilePath;   //SQLite数据库保存路径
        private string resultFilePath;   //结果文件保存路径

        #endregion Fields

        #region Properties

        [DisplayLanguage("新编码SQLite数据库")]
        [DescriptionLanguage("请选择需要关联原编码SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string DatabaseFilePath
        {
            get { return databaseFilePath; }
            set { databaseFilePath = value; NotifyPropertyChanged("DatabaseFilePath"); }
        }

        [DisplayLanguage("原编码SQLite数据库")]
        [DescriptionLanguage("请选择原编码SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string OldDatabaseFilePath
        {
            get { return olddatabaseFilePath; }
            set { olddatabaseFilePath = value; NotifyPropertyChanged("OldDatabaseFilePath"); }
        }

        [DisplayLanguage("挂接结果存放路径", IsLanguageName = false)]
        [DescriptionLanguage("挂接结果存放路径的路径", IsLanguageName = false)]
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
    }
}
