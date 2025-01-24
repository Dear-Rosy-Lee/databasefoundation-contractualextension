using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 更改坐标系
    /// </summary>
    public class CoordShiftingArgument : TaskArgument
    {
        private SettingsProfileCenter center;
        private DataSave dataSave;
        public CoordShiftingArgument()
        {
            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<DataSave>();
            var section = profile.GetSection<DataSave>();
            dataSave = (section.Settings as DataSave);

            NewShapePath = dataSave.NewShapePath;
            if (dataSave.SingleOldShapePath != null)
                OldShapePath = new List<string>() { dataSave.SingleOldShapePath };
        }

        #region 数据存储

        [DisplayLanguage("原Shape文件：")]
        [DescriptionLanguage("原Shape文件")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderList),
            Trigger = typeof(ArgumentPropertyTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public List<string> OldShapePath
        {
            get { return _SourcesShps; }
            set
            {
                _SourcesShps = value;
                NotifyPropertyChanged(nameof(OldShapePath));
                if (_SourcesShps == null || _SourcesShps.Count == 0)
                {
                    OldShapePrj = null;
                    return;
                }

                string shpfile = _SourcesShps[0];
                string filepath = System.IO.Path.GetDirectoryName(shpfile);
                string filename = System.IO.Path.GetFileNameWithoutExtension(shpfile);
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var provider = ds.DataSource as IProviderShapefile;
                OldShapePrj = provider.GetSpatialReference(filename);
            }
        }
        private List<string> _SourcesShps;

        [DisplayLanguage("原Shape文件坐标系：")]
        [DescriptionLanguage("原Shape文件坐标系,多个文件时，显示第一个文件的坐标系")]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderLable),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-horizontal.png")]
        public SpatialReference OldShapePrj
        {
            get { return _OldShapePrjName; }
            set
            {
                _OldShapePrjName = value;
                NotifyPropertyChanged("OldShapePrj");
            }
        }
        private SpatialReference _OldShapePrjName;


        [DisplayLanguage("是否纠偏数据")]
        [DescriptionLanguage("是否纠偏数据")]
        [PropertyDescriptor(
        Builder = typeof(PropertyBuilderCheckCardBoolean),
            Trigger = typeof(ArchiveControlTrigger),
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-horizontal.png")]
        public bool IsReChangeCoord
        {
            get { return _IsReChangeCoord; }
            set
            {
                _IsReChangeCoord = value;
                NotifyPropertyChanged("IsReChangeCoord");
            }
        }
        private bool _IsReChangeCoord;

        [DisplayLanguage("目标坐标系：")]
        [DescriptionLanguage("目标坐标系")]
        [PropertyDescriptor(
        Builder = typeof(PropertyDescriptorBuilderSelectedSpatialReference),
            Trigger = typeof(ArchiveControlTrigger),
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-horizontal.png")]
        public SpatialReference ChangeDestinationPrj
        {
            get { return _DestinationPrj; }
            set
            {
                _DestinationPrj = value;
                NotifyPropertyChanged("ChangeDestinationPrj");
            }
        }
        private SpatialReference _DestinationPrj;

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
