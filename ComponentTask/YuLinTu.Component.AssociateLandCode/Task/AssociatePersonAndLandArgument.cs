using System;
using System.IO;
using YuLinTu.Component.Common;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.AssociateLandCode
{
    public class AssociatePersonAndLandArgument : TaskArgument
    {
        #region Fields


        private string databaseFilePath;   //SQLite数据库路径
        private string olddatabaseFilePath;   //SQLite数据库保存路径
        private string resultFilePath;   //结果文件保存路径
        private string relationExcelFilePath;
        private bool searchInSharePerson;
        private bool searchInvpcode;
        private bool searchInshape;
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

        [DisplayLanguage("权属单位变化情况表")]
        [DescriptionLanguage("新旧权属单位变化情况表")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedExcel),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string RelationExcelFilePath
        {
            get { return relationExcelFilePath; }
            set { relationExcelFilePath = value; NotifyPropertyChanged("RelationExcelFilePath"); }
        }

        [DisplayLanguage("以图形相似性进行关联")]
        [DescriptionLanguage("在发包方内未在属性上关联的地块按图形相似性进行查找关联")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool SearchInShape
        {
            get { return searchInshape; }
            set { searchInshape = value; NotifyPropertyChanged("SearchInShape"); }
        }

        [DisplayLanguage("承包方编码后四位与姓名查找")]
        [DescriptionLanguage("承包方编码后四位与姓名查找关联")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool SearchInvpcode
        {
            get { return searchInvpcode; }
            set { searchInvpcode = value; NotifyPropertyChanged("SearchInvpcode"); }
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

        public AssociatePersonAndLandArgument() 
        {
            resultFilePath= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
