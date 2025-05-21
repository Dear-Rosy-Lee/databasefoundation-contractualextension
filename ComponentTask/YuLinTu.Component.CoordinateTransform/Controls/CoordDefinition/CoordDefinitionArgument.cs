using System.Collections.Generic;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 定义坐标系
    /// </summary>
    public class CoordDefinitionArgument : TaskArgument
    {
        private SettingsProfileCenter center;
        private DataSave dataSave;
        public CoordDefinitionArgument()
        {
            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<DataSave>();
            var section = profile.GetSection<DataSave>();
            dataSave = (section.Settings as DataSave);

            DefinitionShape = new List<string>();
            if (!string.IsNullOrEmpty(dataSave.NewShapePath))
                DefinitionShape = new List<string>() { dataSave.NewShapePath };
            TargetPrjPath = dataSave.SingleOldShapePath;
        }

        #region 数据存储
        [DisplayLanguage("目标定义Shape存储位置：")]
        [DescriptionLanguage("目标定义Shape存储位置")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderList),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public List<string> DefinitionShape
        {
            get { return _DefinitionShape; }
            set
            {
                _DefinitionShape = value;
                NotifyPropertyChanged("DefinitionShape");
            }
        }
        private List<string> _DefinitionShape;

        [DisplayLanguage("选择所需定义的坐标系文件：")]
        [DescriptionLanguage("选择所需定义的坐标系文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorSelectPrjFile),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string TargetPrjPath
        {
            get { return _TargetPrjPath; }
            set
            {
                _TargetPrjPath = value;
                NotifyPropertyChanged("TargetPrjPath");
            }
        }
        private string _TargetPrjPath;
        #endregion
    }
}
