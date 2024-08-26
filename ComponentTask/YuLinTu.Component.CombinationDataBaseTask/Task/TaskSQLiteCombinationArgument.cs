/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Component.Common;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CombinationDataBaseTask
{
    /// <summary>
    /// 合并SQLite数据库任务参数
    /// </summary>
    public class TaskSQLiteCombinationArgument : TaskArgument
    {
        #region Fields

        private string zoneNameAndCode;   //当前地域名称+编码
        private string databaseFilePath;   //SQLite数据库路径
        private bool isCoverDataByZoneLevel;   //是否覆盖数据库中相同地域下的数据
        private bool isBatchCombination;   //是否批量合并数据库

        #endregion

        #region Properties

        [DisplayLanguage("批量合并数据库")]
        [DescriptionLanguage("是否批量合并数据库")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
          UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsBatchCombination
        {
            get { return isBatchCombination; }
            set { isBatchCombination = value; NotifyPropertyChanged("IsBatchCombination"); }
        }
        

        [DisplayLanguage("SQLite数据库路径")]
        [DescriptionLanguage("请选择合并SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
            Trigger = typeof(PropertyTriggerFile),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string DatabaseFilePath
        {
            get { return databaseFilePath; }
            set { databaseFilePath = value; NotifyPropertyChanged("DatabaseFilePath"); }
        }

        [DisplayLanguage("行政地域")]
        [DescriptionLanguage("请选择行政地域编码")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedZoneTextBox),
            Trigger = typeof(PropertyTriggerZone),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string ZoneNameAndCode
        {
            get { return zoneNameAndCode; }
            set { zoneNameAndCode = value; NotifyPropertyChanged("ZoneNameAndCode"); }
        }

        [DisplayLanguage("覆盖数据库数据")]
        [DescriptionLanguage("是否根据地域级别匹配覆盖本地数据库数据")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCoverDataByZoneLevel
        {
            get { return isCoverDataByZoneLevel; }
            set { isCoverDataByZoneLevel = value; NotifyPropertyChanged("IsCoverDataByZoneLevel"); }
        }

      

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskSQLiteCombinationArgument()
        { }

        #endregion
    }
}
