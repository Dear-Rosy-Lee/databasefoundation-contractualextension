/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Component.Common;
using YuLinTu.Component.Setting;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractedLand.BoundaryCalculateTask
{
    /// <summary>
    /// 任务参数
    /// </summary>
    public class TaskBoundaryCalculateArgument : TaskArgument
    {
        #region Fields

        private string shapfilePath;   //当前地域名称+编码
        private string databaseFilePath;   //SQLite数据库路径
        private string databaseSavePath;   //SQLite数据库保存路径

        #endregion

        #region Properties

        [DisplayLanguage("矢量地块目录")]
        [DescriptionLanguage("请选择地块矢量文件，可以多选")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/border-all.png")]
        public string ShapeFilePath
        {
            get { return shapfilePath; }
            set { shapfilePath = value; NotifyPropertyChanged("ShapeFilePath"); }
        }

        [DisplayLanguage("权属MDB数据路")]
        [DescriptionLanguage("请选权属MDB数据路文件")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedMdb),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string DatabaseFilePath
        {
            get { return databaseFilePath; }
            set { databaseFilePath = value; NotifyPropertyChanged("DatabaseFilePath"); }
        }

        [DisplayLanguage("文件保存路径")]
        [DescriptionLanguage("请选择生成文件保存路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string DatabaseSavePath
        {
            get { return databaseSavePath; }
            set { databaseSavePath = value; NotifyPropertyChanged("DatabaseSavePath"); }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskBoundaryCalculateArgument()
        { }

        #endregion
    }
}
