using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    public class LandTopologyTaskArgument : TopologyTaskArgument
    {
        #region Methods

        public string ZoneFullName
        {
            get { return _ZoneFullName; }
            set { _ZoneFullName = value; NotifyPropertyChanged("ZoneFullName"); }
        }
        private string _ZoneFullName;

        public string ZoneCode
        {
            get { return _ZoneCode; }
            set { _ZoneCode = value; NotifyPropertyChanged("ZoneCode"); }
        }
        private string _ZoneCode;

        public string LayerName
        {
            get { return _LayerName; }
            set { _LayerName = value; NotifyPropertyChanged("LayerName"); }
        }
        private string _LayerName;

        [System.Xml.Serialization.XmlIgnore]
        public IDataSource DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; NotifyPropertyChanged("DataSource"); }
        }
        private IDataSource _DataSource;

        [System.Xml.Serialization.XmlIgnore]
        public Geometry Geometry
        {
            get { return _Geometry; }
            set { _Geometry = value; NotifyPropertyChanged("Geometry"); }
        }
        private Geometry _Geometry;

        public bool ClearAll
        {
            get { return _ClearAll; }
            set { _ClearAll = value; NotifyPropertyChanged("ClearAll"); }
        }
        private bool _ClearAll = true;

        public bool DoOverlaps
        {
            get { return _DoOverlaps; }
            set { _DoOverlaps = value; NotifyPropertyChanged("DoOverlaps"); }
        }
        private bool _DoOverlaps = true;

        public double OverlapsTolerance
        {
            get { return _OverlapsTolerance; }
            set { _OverlapsTolerance = value; NotifyPropertyChanged("OverlapsTolerance"); }
        }
        private double _OverlapsTolerance = 0.001;

        public bool DoNodeRepeat
        {
            get { return _DoNodeRepeat; }
            set { _DoNodeRepeat = value; NotifyPropertyChanged("DoNodeRepeat"); }
        }
        private bool _DoNodeRepeat = true;

        public double NodeRepeatTolerance1
        {
            get { return _NodeRepeatTolerance1; }
            set { _NodeRepeatTolerance1 = value; NotifyPropertyChanged("NodeRepeatTolerance1"); }
        }
        private double _NodeRepeatTolerance1 = 0.0001;

        public double NodeRepeatTolerance2
        {
            get { return _NodeRepeatTolerance2; }
            set { _NodeRepeatTolerance2 = value; NotifyPropertyChanged("NodeRepeatTolerance2"); }
        }
        private double _NodeRepeatTolerance2 = 0.05;

        public bool DoArea
        {
            get { return _DoArea; }
            set { _DoArea = value; NotifyPropertyChanged("DoArea"); }
        }
        private bool _DoArea = true;

        public double AreaTolerance
        {
            get { return _AreaTolerance; }
            set { _AreaTolerance = value; NotifyPropertyChanged("AreaTolerance"); }
        }
        private double _AreaTolerance = 1;


        public bool DoAngle
        {
            get { return _DoAngle; }
            set { _DoAngle = value; NotifyPropertyChanged("DoAngle"); }
        }
        private bool _DoAngle = true;

        public double AngleTolerance
        {
            get { return _AngleTolerance; }
            set { _AngleTolerance = value; NotifyPropertyChanged("AngleTolerance"); }
        }
        private double _AngleTolerance = 5;

        public bool DoSelfOverlaps
        {
            get { return _DoSelfOverlaps; }
            set { _DoSelfOverlaps = value; NotifyPropertyChanged("DoSelfOverlaps"); }
        }
        private bool _DoSelfOverlaps = true;

        public double SelfOverlapsDistanceTolerance
        {
            get { return _SelfOverlapsDistanceTolerance; }
            set { _SelfOverlapsDistanceTolerance = value; NotifyPropertyChanged("SelfOverlapsDistanceTolerance"); }
        }
        private double _SelfOverlapsDistanceTolerance = 0.1;

        public double SelfOverlapsLengthTolerance
        {
            get { return _SelfOverlapsLengthTolerance; }
            set { _SelfOverlapsLengthTolerance = value; NotifyPropertyChanged("SelfOverlapsLengthTolerance"); }
        }
        private double _SelfOverlapsLengthTolerance = 0.05;

        public bool DoNearNodeRepeat
        {
            get { return _DoNearNodeRepeat; }
            set { _DoNearNodeRepeat = value; NotifyPropertyChanged("DoNearNodeRepeat"); }
        }
        private bool _DoNearNodeRepeat = true;

        public double NearNodeRepeatTolerance
        {
            get { return _NearNodeRepeatTolerance; }
            set { _NearNodeRepeatTolerance = value; NotifyPropertyChanged("NearNodeRepeatTolerance"); }
        }
        private double _NearNodeRepeatTolerance = 0.05;

        public bool DoNodeOnEdge
        {
            get { return _DoDoNodeOnEdge; }
            set { _DoDoNodeOnEdge = value; NotifyPropertyChanged("DoNodeOnEdge"); }
        }
        private bool _DoDoNodeOnEdge = true;

        public double NodeOnEdgeTolerance
        {
            get { return _NodeOnEdgeTolerance; }
            set { _NodeOnEdgeTolerance = value; NotifyPropertyChanged("NodeOnEdgeTolerance"); }
        }
        private double _NodeOnEdgeTolerance = 0.05;

        #endregion
    }
}
