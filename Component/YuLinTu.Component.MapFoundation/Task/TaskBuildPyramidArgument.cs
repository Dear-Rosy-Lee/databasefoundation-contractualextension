using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskBuildPyramidArgument : TaskArgument
    {
        #region Properties

        [DisplayLanguage("影像文件")]
        [DescriptionLanguage("影像文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderFileBrowserRaster),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string RasterFiles
        {
            get { return _RasterFiles; }
            set { _RasterFiles = value.TrimSafe(); NotifyPropertyChanged("RasterFiles"); }
        }
        private string _RasterFiles;

        [DisplayLanguage("SQLite 数据库文件")]
        [DescriptionLanguage("SQLite 数据库文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderSaveFileBrowserSQLite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SQLiteFileName
        {
            get { return _SQLiteFileName; }
            set { _SQLiteFileName = value; NotifyPropertyChanged("SQLiteFileName"); }
        }
        private string _SQLiteFileName;

        #endregion

        #region Ctor

        public TaskBuildPyramidArgument()
        {
        }

        #endregion

        #region Methods

        #endregion
    }
}
