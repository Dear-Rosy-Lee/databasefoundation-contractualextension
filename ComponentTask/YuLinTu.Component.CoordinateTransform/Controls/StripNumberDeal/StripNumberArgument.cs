using System.Collections.Generic;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 带号处理
    /// </summary>
    public class StripNumberArgument : TaskArgument
    {
        [DisplayLanguage("操作：")]
        [DescriptionLanguage("带号操作类型")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderComboBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Producte.png")]
        public int OperateStripNumber
        {
            get { return _OperateStripNumber; }
            set
            {
                _OperateStripNumber = value; NotifyPropertyChanged("OperateStripNumber");
            }
        }
        private int _OperateStripNumber;

        #region 数据存储

        [DisplayLanguage("原Shape文件：")]
        [DescriptionLanguage("原Shape文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderList),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public List<string> OldShapePath
        {
            get { return _OldShapePath; }
            set
            {
                _OldShapePath = value;
                NotifyPropertyChanged("OldShapePath");
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

                //ExportLastResSettingDefine.SaveFolderName = _SaveFolderName;
                NotifyPropertyChanged("NewShapePath");
            }
        }
        private string _NewShapePath;
        #endregion
    }
}
