using YuLinTu.Component.Common;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.AssociateLandCode
{
    public class AssociateLandCodeArgument : TaskArgument
    {
        #region Fields

       
        private string databaseFilePath;   //SQLite数据库路径
        private string olddatabaseFilePath;   //SQLite数据库保存路径

        #endregion Fields

        #region Properties

        [DisplayLanguage("需要关联原地块编码SQLite数据库")]
        [DescriptionLanguage("请选择需要关联原地块编码SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string DatabaseFilePath
        {
            get { return databaseFilePath; }
            set { databaseFilePath = value; NotifyPropertyChanged("DatabaseFilePath"); }
        }

        [DisplayLanguage("原地块编码SQLite数据库")]
        [DescriptionLanguage("请选择原地块编码SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string OldDatabaseFilePath
        {
            get { return olddatabaseFilePath; }
            set { olddatabaseFilePath = value; NotifyPropertyChanged("OldDatabaseFilePath"); }
        }

        #endregion Properties
    }
}
