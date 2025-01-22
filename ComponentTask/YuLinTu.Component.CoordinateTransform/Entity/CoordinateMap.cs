namespace YuLinTu.Component.CoordinateTransformTask
{
    public class CoordinateMap : NotifyCDObject
    {
        [DisplayLanguage("EPSG")]
        public int EPSG
        {
            get { return _EPSG; }
            set { _EPSG = value; NotifyPropertyChanged("EPSG"); }
        }

        private int _EPSG;

        [DisplayLanguage("坐标系名称")]
        public string CoordinateName
        {
            get { return _CoordinateName; }
            set { _CoordinateName = value; NotifyPropertyChanged("CoordinateName"); }
        }

        private string _CoordinateName;

        [DisplayLanguage("经度最小")]
        public decimal MinLongitude
        {
            get { return _MinLongitude; }
            set { _MinLongitude = value; NotifyPropertyChanged("MinLongitude"); }
        }

        private decimal _MinLongitude;

        [DisplayLanguage("经度最大")]
        public decimal MaxLongitude
        {
            get { return _MaxLongitude; }
            set { _MaxLongitude = value; NotifyPropertyChanged("MaxLongitude"); }
        }

        private decimal _MaxLongitude;

        [DisplayLanguage("中央经线")]
        public decimal CentralMeridian
        {
            get { return _CentralMeridian; }
            set { _CentralMeridian = value; NotifyPropertyChanged("CentralMeridian"); }
        }

        private decimal _CentralMeridian;

        [DisplayLanguage("备注")]
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; NotifyPropertyChanged("Remark"); }
        }

        private string _Remark;
    }
}
