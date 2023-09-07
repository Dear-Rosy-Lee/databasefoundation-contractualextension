using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Tasks;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// PolygonTopologyByUserSetting.xaml 的交互逻辑
    /// </summary>
    public partial class PolygonTopologyByUserSetting : Dialog
    {
        #region Properties

        /// <summary>
        /// 相交地物的ID及空间地块列表
        /// </summary>
        public Dictionary<string, YuLinTu.Spatial.Geometry> intersectGeoIDs { get; set; }

        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public MapFoundationUserSettingDefine OtherDefine
        {
            get { return mpfdtusdsetting; }
            set
            {
                mpfdtusdsetting = value;
                this.DataContext = OtherDefine;
            }
        }

        #endregion

        #region Fields
        internal MapFoundationUserSettingDefine config;
        internal SettingsProfileCenter systemCenter;
        private Layer layer;
        private YuLinTu.Spatial.Geometry drawGeometry;
        private MapFoundationUserSettingDefine mpfdtusdsetting;

        #endregion

        #region Ctor
        public PolygonTopologyByUserSetting(Layer layer, YuLinTu.Spatial.Geometry drawGeometry)
        {
            this.layer = layer;
            this.drawGeometry = drawGeometry;
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Uninstall()
        {

        }

        #endregion

        #region Methods - Override

        protected override void OnInitializeGo()
        {
            var targetLayer = layer as VectorLayer;
            if (targetLayer == null || targetLayer.DataSource == null) return;

            intersectGeoIDs = new Dictionary<string, Spatial.Geometry>();
            targetLayer.DataSource.GetByIntersects(drawGeometry).ForEach(s => intersectGeoIDs.Add(s.Object.GetPropertyValue("ID").ToString(), s.Geometry));

            CanConfirm = false;
        }

        protected override void OnInitializeStarted()
        {
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<MapFoundationUserSettingDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<MapFoundationUserSettingDefine>();
            config = (section.Settings as MapFoundationUserSettingDefine);   //得到经反序列化后的对象
            OtherDefine = config.Clone() as MapFoundationUserSettingDefine;
        }

        protected override void OnInitializeCompleted()
        {
            CanConfirm = true;
        }


        #endregion

        #region Methods - Events
               

        #endregion

        #region Methods - Private


        #endregion

        #endregion
    }
}
