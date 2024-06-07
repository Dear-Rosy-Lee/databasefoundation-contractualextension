/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Component.Common;
using YuLinTu.Component.Setting;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.SeparateDataBaseTask
{
    /// <summary>
    /// 合并SQLite数据库任务参数
    /// </summary>
    public class TaskSQLiteSeparateArgument : TaskArgument
    {
        #region Fields

        private string zoneNameAndCode;   //当前地域名称+编码
        private string databaseFilePath;   //SQLite数据库路径
        private string databaseSavePath;   //SQLite数据库保存路径

        #endregion

        #region Properties

        [DisplayLanguage("SQLite数据库路径")]
        [DescriptionLanguage("请选择分离SQLite数据库所在路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedFileTextBoxSqlite),
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

        [DisplayLanguage("分离数据库保存路径")]
        [DescriptionLanguage("请选择分离数据库保存路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSaveFileBrowserSQLite),
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
        public TaskSQLiteSeparateArgument()
        { }

        #endregion
    }
}
