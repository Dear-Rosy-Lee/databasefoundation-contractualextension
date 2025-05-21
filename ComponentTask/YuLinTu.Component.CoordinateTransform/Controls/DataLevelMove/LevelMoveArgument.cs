using System.Collections.Generic;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 数据平移
    /// </summary>
    public class LevelMoveArgument : TaskArgument
    {
        [DisplayLanguage("X平移量：")]
        [DescriptionLanguage("X坐标平移值(米)")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Producte.png")]
        public double XMoveNumber
        {
            get { return _XMoveNumber; }
            set
            {
                _XMoveNumber = value; NotifyPropertyChanged("XMoveNumber");
            }
        }
        private double _XMoveNumber;

        [DisplayLanguage("Y平移量：")]
        [DescriptionLanguage("Y坐标平移值(米)")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Producte.png")]
        public double YMoveNumber
        {
            get { return _YMoveNumber; }
            set
            {
                _YMoveNumber = value;
                NotifyPropertyChanged("YMoveNumber");
            }
        }

        private double _YMoveNumber;

        #region 数据存储

        [DisplayLanguage("原Shape文件：")]
        [DescriptionLanguage("原Shape文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderList),
            Trigger = typeof(ArgumentPropertyTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public List<string> OldShapePath
        {
            get { return _OldShapePath; }
            set
            {
                _OldShapePath = value;
                NotifyPropertyChanged(nameof(OldShapePath));
            }
        }
        private List<string> _OldShapePath;


        [DisplayLanguage("新Shape文件存储位置：")]
        [DescriptionLanguage("新Shape文件存储位置")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string NewShapePath
        {
            get { return _NewShapePath; }
            set
            {
                _NewShapePath = value;
                NotifyPropertyChanged("NewShapePath");
            }
        }
        private string _NewShapePath;
        #endregion
    }
}
