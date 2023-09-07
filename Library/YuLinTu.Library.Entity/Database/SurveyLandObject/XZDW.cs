// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 线状地物
    /// </summary>
    [Serializable]
    [DataTable("XZDW")]
    public class XZDW : NotifyCDObject
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
        /// 长度
        /// </summary>
        private double cd;

        /// <summary>
        /// 宽度
        /// </summary>
        private double kd;

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
        /// 长度
        /// </summary>
        [DataColumn("CD")]
        public double CD
        {
            get { return cd; }
            set
            {
                cd = value;
                NotifyPropertyChanged("CD");
            }
        }


        /// <summary>
        /// 宽度
        /// </summary>
        [DataColumn("KD")]
        public double KD
        {
            get { return kd; }
            set
            {
                kd = value;
                NotifyPropertyChanged("KD");
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

        public XZDW()
        {
            ID = Guid.NewGuid();
        }

    }
}
