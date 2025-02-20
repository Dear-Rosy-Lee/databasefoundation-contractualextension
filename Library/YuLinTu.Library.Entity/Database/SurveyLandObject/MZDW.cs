// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 面状地物
    /// </summary>
    [Serializable]
    [DataTable("MZDW")]
    public class MZDW : NotifyCDObject
    {

        /// <summary>
        /// 标识
        /// </summary>
        private Guid id;
        /// <summary>
        /// 空间字段
        /// </summary>
        private Geometry shape;

        /// <summary>
        /// 标识码
        /// </summary>
        private int? bsm;

        /// <summary>
        /// 要素代码
        /// </summary>
        private string ysdm;

        /// <summary>
        /// 地物名称
        /// </summary>
        private string dwmc;

        /// <summary>
        /// 面积
        /// </summary>
        private double area;

        /// <summary>
        /// 地域名称
        /// </summary>
        private string zonename;

        /// <summary>
        /// 地域编码
        /// </summary>
        private string zonecode;


        /// <summary>
        /// 备注
        /// </summary>
        private string comment;

        /// <summary>
        /// 标识码
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
        [DataColumn("BSM")]
        public int? BSM
        {
            get { return bsm; }
            set
            {
                bsm = value;
                NotifyPropertyChanged("BSM");
            }
        }

        /// <summary>
        /// 要素代码
        /// </summary>
        [DataColumn("YSDM")]
        public string YSDM
        {
            get { return ysdm; }
            set
            {
                ysdm = value.TrimSafe();
                NotifyPropertyChanged("YSDM");
            }
        }

        /// <summary>
        /// 地物名称
        /// </summary>
        [DataColumn("DWMC")]
        public string DWMC
        {
            get { return dwmc; }
            set
            {
                dwmc = value.TrimSafe();
                NotifyPropertyChanged("DWMC");
            }
        }


        /// <summary>
        /// 面积
        /// </summary>
        [DataColumn("MJ")]
        public double Area
        {
            get { return area; }
            set
            {
                area = value;
                NotifyPropertyChanged("Area");
            }
        }

        /// <summary>
        /// 地域名称
        /// </summary>
        [DataColumn("zonename")]
        public string ZoneName
        {
            get { return zonename; }
            set
            {
                zonename = value.TrimSafe();
                NotifyPropertyChanged("ZoneName");
            }
        }


        /// <summary>
        /// 地域编码
        /// </summary>
        [DataColumn("zonecode")]
        public string ZoneCode
        {
            get { return zonecode; }
            set
            {
                zonecode = value.TrimSafe();
                NotifyPropertyChanged("ZoneCode");
            }
        }




        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("BZ")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value.TrimSafe();
                NotifyPropertyChanged("Comment");
            }
        }



        /// <summary>
        /// 空间字段
        /// </summary>
        public Geometry Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                NotifyPropertyChanged("Shape");
            }
        }
        public MZDW()
        {
            ID = Guid.NewGuid();
        }
    }
}
