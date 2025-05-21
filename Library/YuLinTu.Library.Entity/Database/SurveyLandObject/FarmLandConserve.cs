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
    /// 基本农田保护区
    /// </summary>
    [Serializable]
    [DataTable("JBNTBHQ")]
    public class FarmLandConserve : NotifyCDObject
    {
        #region Fields

        private Guid id;
        private int? code;  //标识码
        private string featureCode;  //要素代码
        private string conserveNumber;    //保护区编号a
        private double? farmLandArea;  //基本农田面积
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
        /// 保护区编号a
        /// </summary>
        [DataColumn("BHQBH")]
        public string ConserveNumber
        {
            get { return conserveNumber; }
            set
            {
                conserveNumber = value.TrimSafe();
                NotifyPropertyChanged("ConserveNumber");
            }
        }

        /// <summary>
        /// 基本农田面积
        /// </summary>
        [DataColumn("JBNTMJ")]
        public double? FarmLandArea
        {
            get { return farmLandArea; }
            set
            {
                farmLandArea = value;
                NotifyPropertyChanged("FarmLandArea");
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
        public FarmLandConserve()
        {
            ID = Guid.NewGuid();
        }

        #endregion
    }
}
