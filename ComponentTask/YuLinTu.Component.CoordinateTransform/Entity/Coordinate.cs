using System.Collections.Generic;
using System.ComponentModel;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 二维的点
    /// </summary>
    public struct Point2d
    {
        public double X;
        public double Y;
    }

    /// <summary>
    /// 三维的点
    /// </summary>
    public struct Point3d
    {
        public double X;
        public double Y;
        public double Z;
    }

    /// <summary>
    /// 4参数法
    /// </summary>
    public class Param4 : NotifyCDObject
    {
        /// <summary>
        /// X平移
        /// </summary>
        [DisplayName("X平移")]
        public double Px
        {
            get { return _Px; }
            set { _Px = value; NotifyPropertyChanged("Px"); }
        }
        private double _Px;

        /// <summary>
        /// Y平移
        /// </summary>
        [DisplayName("Y平移")]
        public double Py
        {
            get { return _Py; }
            set { _Py = value; NotifyPropertyChanged("Py"); }
        }
        private double _Py;

        /// <summary>
        /// 旋转角度T
        /// </summary>
        [DisplayName("旋转角度T")]
        public double RotateAngleT
        {
            get { return _RotateAngleT; }
            set { _RotateAngleT = value; NotifyPropertyChanged("RotateAngleT"); }
        }
        private double _RotateAngleT;

        /// <summary>
        /// 尺度K
        /// </summary>
        [DisplayName("尺度K")]
        public double ScaleK
        {
            get { return _ScaleK; }
            set { _ScaleK = value; NotifyPropertyChanged("ScaleK"); }
        }
        private double _ScaleK;
    }

    /// <summary>
    /// 7参数法
    /// </summary>
    public class Param7 : NotifyCDObject
    {
        /// <summary>
        /// X平移
        /// </summary>
        [DisplayName("X平移")]
        public double Px
        {
            get { return _Px; }
            set { _Px = value; NotifyPropertyChanged("Px"); }
        }
        private double _Px;

        /// <summary>
        /// Y平移
        /// </summary>
        [DisplayName("Y平移")]
        public double Py
        {
            get { return _Py; }
            set { _Py = value; NotifyPropertyChanged("Py"); }
        }
        private double _Py;

        /// <summary>
        /// Z平移
        /// </summary>
        [DisplayName("Z平移")]
        public double Pz
        {
            get { return _Pz; }
            set { _Pz = value; NotifyPropertyChanged("Pz"); }
        }
        private double _Pz;

        /// <summary>
        /// X轴旋转角度
        /// </summary>
        [DisplayName("X轴旋转角度")]
        public double XAxisRotateAngle
        {
            get { return _XAxisRotateAngle; }
            set { _XAxisRotateAngle = value; NotifyPropertyChanged("XAxisRotateAngle"); }
        }
        private double _XAxisRotateAngle;

        /// <summary>
        /// Y轴旋转角度
        /// </summary>
        [DisplayName("Y轴旋转角度")]
        public double YAxisRotateAngle
        {
            get { return _YAxisRotateAngle; }
            set { _YAxisRotateAngle = value; NotifyPropertyChanged("YAxisRotateAngle"); }
        }
        private double _YAxisRotateAngle;

        /// <summary>
        /// Z轴旋转角度
        /// </summary>
        [DisplayName("Z轴旋转角度")]
        public double ZAxisRotateAngle
        {
            get { return _ZAxisRotateAngle; }
            set { _ZAxisRotateAngle = value; NotifyPropertyChanged("ZAxisRotateAngle"); }
        }
        private double _ZAxisRotateAngle;

        /// <summary>
        /// 尺度K
        /// </summary>
        [DisplayName("尺度K")]
        public double ScaleK
        {
            get { return _ScaleK; }
            set { _ScaleK = value; NotifyPropertyChanged("ScaleK"); }
        }
        private double _ScaleK;
    }

    /// <summary>
    /// 数据存储
    /// </summary>
    public class DataSave : NotifyCDObject
    {
        /// <summary>
        /// 0：4参数法
        /// 1：7参数法
        /// </summary>
        public int TestMethod { get; set; }

        /// <summary>
        /// 原shape文件位置
        /// </summary>
        [DisplayName("原Shape文件位置")]
        public List<string> OldShapePath
        {
            get { return _OldShapePath; }
            set { _OldShapePath = value; NotifyPropertyChanged("OldShapePath"); }
        }
        private List<string> _OldShapePath;

        /// <summary>
        /// 原shape文件位置
        /// </summary>
        [DisplayName("原Shape文件位置")]
        public string SingleOldShapePath
        {
            get { return _SingleOldShapePath; }
            set { _SingleOldShapePath = value; NotifyPropertyChanged("SingleOldShapePath"); }
        }
        private string _SingleOldShapePath;

        /// <summary>
        /// 新Shape文件的存放位置
        /// </summary>
        [DisplayName("新Shape文件的存放位置")]
        public string NewShapePath
        {
            get { return _NewShapePath; }
            set { _NewShapePath = value; NotifyPropertyChanged("NewShapePath"); }
        }
        private string _NewShapePath;

        public string DestProj { set; get; }
    }
}
