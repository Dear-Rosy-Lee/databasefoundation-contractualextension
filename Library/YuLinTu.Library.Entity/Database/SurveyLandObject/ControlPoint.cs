/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 控制点
    /// </summary>
    [Serializable]
    [DataTable("KZD")]
    public class ControlPoint : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private int? code;  //标识码
        private string featureCode;  //要素代码
        private string pointName;   //控制点名称
        private string pointNumber;  //控制点点号
        private string pointType;    //控制点类型
        private string pointRank;    //控制点等级
        private string bsType;      //标石类型
        private string bzType;      //标志类型
        private string pointState;  //控制点状态
        private string dzj;   //点之记
        private double? x2000;   //X_2000a
        private double? y2000;  //Y_2000a
        private double? x80;   //X(E)_XA80a
        private double? y80;   //Y(E)_XA80a
        private string zoneCode;  //地域编码
        private string zoneName;  //地域名称
        private Geometry shape;  //空间信息

        #endregion

        #region Properties

        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = true)]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        /// <summary>
        /// 标识码
        /// </summary>
        [DataColumn("BSM", Nullable = false)]
        public int? Code
        {
            get { return code; }
            set
            {
                code = value;
                NotifyPropertyChanged("Code");
            }
        }

        /// <summary>
        /// 要素代码
        /// </summary>
        [DataColumn("YSDM", Nullable = false)]
        public string FeatureCode
        {
            get { return featureCode; }
            set
            {
                featureCode = value.TrimSafe();
                NotifyPropertyChanged("FeatureCode");
            }
        }

        /// <summary>
        /// 控制点名称
        /// </summary>
        [DataColumn("KZDMC")]
        public string PointName
        {
            get { return pointName; }
            set
            {
                pointName = value.TrimSafe();
                NotifyPropertyChanged("PointName");
            }
        }

        /// <summary>
        /// 控制点点号
        /// </summary>
        [DataColumn("KZDDH")]
        public string PointNumber
        {
            get { return pointNumber; }
            set
            {
                pointNumber = value.TrimSafe();
                NotifyPropertyChanged("PointNumber");
            }
        }

        /// <summary>
        /// 控制点类型
        /// </summary>
        [DataColumn("KZDLX", Nullable = false)]
        public string PointType
        {
            get { return pointType; }
            set
            {
                pointType = value.TrimSafe();
                NotifyPropertyChanged("pointType");
            }
        }

        /// <summary>
        /// 控制点等级
        /// </summary>
        [DataColumn("KZDDJ", Nullable = false)]
        public string PointRank
        {
            get { return pointRank; }
            set
            {
                pointRank = value.TrimSafe();
                NotifyPropertyChanged("pointRank");
            }
        }

        /// <summary>
        /// 标石类型
        /// </summary>
        [DataColumn("BSLX", Nullable = false)]
        public string BsType
        {
            get { return bsType; }
            set
            {
                bsType = value.TrimSafe();
                NotifyPropertyChanged("BsType");
            }
        }

        /// <summary>
        /// 标志类型
        /// </summary>
        [DataColumn("BZLX", Nullable = false)]
        public string BzType
        {
            get { return bzType; }
            set
            {
                bzType = value.TrimSafe();
                NotifyPropertyChanged("BzType");
            }
        }

        /// <summary>
        /// 控制点状态
        /// </summary>
        [DataColumn("KZDZT")]
        public string PointState
        {
            get { return pointState; }
            set
            {
                pointState = value.TrimSafe();
                NotifyPropertyChanged("PointState");
            }
        }

        /// <summary>
        /// 点之记
        /// </summary>
        [DataColumn("DZJ", Nullable = false)]
        public string Dzj
        {
            get { return dzj; }
            set
            {
                dzj = value.TrimSafe();
                NotifyPropertyChanged("Dzj");
            }
        }

        /// <summary>
        /// X_2000a
        /// </summary>
        [DataColumn("X2000", Nullable = false)]
        public double? X2000
        {
            get { return x2000; }
            set
            {
                x2000 = value;
                NotifyPropertyChanged("X2000");
            }
        }

        /// <summary>
        /// Y_2000a
        /// </summary>
        [DataColumn("Y2000", Nullable = false)]
        public double? Y2000
        {
            get { return y2000; }
            set
            {
                y2000 = value;
                NotifyPropertyChanged("Y2000");
            }
        }

        /// <summary>
        /// X(E)_XA80a
        /// </summary>
        [DataColumn("X80")]
        public double? X80
        {
            get { return x80; }
            set
            {
                x80 = value;
                NotifyPropertyChanged("X80");
            }
        }

        /// <summary>
        /// Y(E)_XA80a
        /// </summary>
        [DataColumn("Y80")]
        public double? Y80
        {
            get { return y80; }
            set
            {
                y80 = value;
                NotifyPropertyChanged("Y80");
            }
        }

        /// <summary>
        /// 地域编码
        /// </summary>
        [DataColumn("DYDM")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value.TrimSafe();
                NotifyPropertyChanged("ZoneCode");
            }
        }

        /// <summary>
        /// 地域名称
        /// </summary>
        [DataColumn("DYMC")]
        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value.TrimSafe();
                NotifyPropertyChanged("ZoneName");
            }
        }

        /// <summary>
        /// 空间信息
        /// </summary>
        [DataColumn("Shape")]
        public Geometry Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                NotifyPropertyChanged("Shape");
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlPoint()
        {
            ID = Guid.NewGuid();
        }

        #endregion
    }
}
