using System;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 坐标系转换
    /// </summary>
    public class CoordTransArgument : TaskArgument
    {
        public const string ZBXCWTS = "选择的多个Shape文件坐标系不一致，请重新选择！";
        private SettingsProfileCenter center;
        private DataSave dataSave;
        public CoordTransArgument()
        {
            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<DataSave>();
            var section = profile.GetSection<DataSave>();
            dataSave = (section.Settings as DataSave);

            NewShapePath = dataSave.NewShapePath;
            OldShapePath = dataSave.OldShapePath;
            DestinationPrj = dataSave.DestProj;
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
                    OldShapePrjName = null;
                    return;
                }

                string shpfile = _SourcesShps[0];
                string filepath = System.IO.Path.GetDirectoryName(shpfile);
                string filename = System.IO.Path.GetFileNameWithoutExtension(shpfile);
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var provider = ds.DataSource as IProviderShapefile;
                var cpji = provider.GetSpatialReference(filename);
                OldShapePrjName = cpji.CreateProjectionInfo().Name == null ? cpji.WKID.ToString() : cpji.CreateProjectionInfo().Name;

                if (_SourcesShps.Count > 1)
                {
                    foreach (var item in _SourcesShps)
                    {
                        shpfile = item;
                        filepath = System.IO.Path.GetDirectoryName(shpfile);
                        filename = System.IO.Path.GetFileNameWithoutExtension(shpfile);
                        ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                        provider = ds.DataSource as IProviderShapefile;

                        var cpjitemp = provider.GetSpatialReference(filename);
                        var temp = cpji.CreateProjectionInfo().Name == null ? cpjitemp.WKID.ToString() : cpjitemp.CreateProjectionInfo().Name;

                        //string temp = provider.GetSpatialReference(filename).CreateProjectionInfo().Name;
                        if (!OldShapePrjName.Equals(temp, StringComparison.CurrentCultureIgnoreCase))
                        {
                            OldShapePrjName = ZBXCWTS;
                            return;
                        }
                    }
                }
            }
        }
        private List<string> _SourcesShps;

        [DisplayLanguage("原Shape文件坐标系：")]
        [DescriptionLanguage("原Shape文件坐标系")]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderLable),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-horizontal.png")]
        public string OldShapePrjName
        {
            get { return _OldShapePrjName; }
            set
            {
                _OldShapePrjName = value;
                NotifyPropertyChanged("OldShapePrjName");
            }
        }
        private string _OldShapePrjName;

        [DisplayLanguage("目标坐标系：")]
        [DescriptionLanguage("目标坐标系")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/folder-horizontal.png")]
        public string DestinationPrj
        {
            get { return _DestinationPrj; }
            set
            {
                _DestinationPrj = value;
                NotifyPropertyChanged("DestinationPrj");
            }
        }
        private string _DestinationPrj;

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

        #region 参数配置
        [DisplayLanguage("启用自定义参数：")]
        [DescriptionLanguage("启用自定义参数")]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderCheckbox),
           Trigger = typeof(ChangeWayTrigger),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool EnabledCustomArgs
        {
            get { return _EnabledCustomArgs; }
            set
            {
                _EnabledCustomArgs = value; NotifyPropertyChanged("EnabledCustomArgs");
            }
        }
        private bool _EnabledCustomArgs;

        [DisplayLanguage("X 平移(米) =")]
        [DescriptionLanguage("X 平移(米)")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            Trigger = typeof(ChangeWayTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public double PyX
        {
            get { return _PyX; }
            set
            {
                _PyX = value; NotifyPropertyChanged("PyX");
            }
        }
        private double _PyX;

        [DisplayLanguage("Y 平移(米) =")]
        [DescriptionLanguage("Y 平移(米)")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            Trigger = typeof(ChangeWayTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/inbox-document-text.png")]
        public double PyY
        {
            get { return _PyY; }
            set
            {
                _PyY = value; NotifyPropertyChanged("PyY");
            }
        }
        private double _PyY;

        [DisplayLanguage("旋转角度T =")]
        [DescriptionLanguage("旋转角度T")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            Trigger = typeof(ChangeWayTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Producte.png")]
        public double RotateAngleT
        {
            get { return _RotateAngleT; }
            set
            {
                _RotateAngleT = value; NotifyPropertyChanged("RotateAngleT");
            }
        }
        private double _RotateAngleT;

        [DisplayLanguage("尺度 K=")]
        [DescriptionLanguage("尺度 K")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderDoubleUpDown),
            Trigger = typeof(ChangeWayTrigger),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/property.png")]
        public double Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value; NotifyPropertyChanged("Scale");
            }
        }
        private double _Scale;

        #endregion

    }
}
