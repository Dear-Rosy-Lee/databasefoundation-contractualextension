using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu;
namespace YuLinTu.Component.Setting
{
    public class SpatialReferenceSetDialogArgument : TaskArgument
    {
        #region Properties      

        [DisplayLanguage("数据库路径")]
        [DescriptionLanguage("数据库路径")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderSaveFileBrowserSQLite),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SaveFileName
        {
            get { return _SaveFileName; }
            set { _SaveFileName = value; NotifyPropertyChanged("SaveFileName"); }
        }
        private string _SaveFileName;

        [DisplayLanguage("坐标系")]
        [DescriptionLanguage("坐标系")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderSelectedSRBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public Spatial.SpatialReference SelectSRName
        {
            get { return _SelectSRName; }
            set { _SelectSRName = value; NotifyPropertyChanged("SelectSRName"); }
        }
        private Spatial.SpatialReference _SelectSRName;


        #endregion

    }
}
