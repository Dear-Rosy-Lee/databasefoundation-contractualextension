/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 区域界线
    /// </summary>
    [Serializable]
    [DataTable("QYJX")]
    public class ZoneBoundary : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private int? code;  //标识码
        private string featureCode;  //要素代码
        private string boundaryLineType;    //界线类型
        private string boundaryLineNature;  //界线性质
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
        /// 界线类型
        /// </summary>
        [DataColumn("JXLX", Nullable = false)]
        public string BoundaryLineType
        {
            get { return boundaryLineType; }
            set
            {
                boundaryLineType = value.TrimSafe();
                NotifyPropertyChanged("BoundaryLineType");
            }
        }

        /// <summary>
        /// 界线性质
        /// </summary>
        [DataColumn("JXXZ", Nullable = false)]
        public string BoundaryLineNature
        {
            get { return boundaryLineNature; }
            set
            {
                boundaryLineNature = value.TrimSafe();
                NotifyPropertyChanged("BoundaryLineNature");
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
        public ZoneBoundary()
        {
            ID = Guid.NewGuid();
        }

        #endregion
    }
}
