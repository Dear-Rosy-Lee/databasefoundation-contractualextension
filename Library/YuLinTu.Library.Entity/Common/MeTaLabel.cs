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
    [Serializable]
    [DataTable("ZD_MeTaLabel")]
    public class MeTaLabel : NotifyCDObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static MeTaLabel()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandMarkType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryPointType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MeTaLabel()
        {
            ID = Guid.NewGuid();          
        }

        /// <summary>
        ///标识码
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
        private Guid id;


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

        /// <summary>
        /// 空间字段
        /// </summary>
        private Geometry shape;


        /// <summary>
        /// BSM
        /// </summary>
        [DataColumn("BSM")]
        public int BSM
        {
            get { return bsm; }
            set
            {
                bsm = value;
                NotifyPropertyChanged("BSM");
            }
        }
        /// <summary>
        /// BSM
        /// </summary>
        private int bsm;

        /// <summary>
        /// YSDM
        /// </summary>
        [DataColumn("YSDM")]
        public string YSDM
        {
            get { return ysdm; }
            set
            {
                ysdm = value;
                NotifyPropertyChanged("YSDM");
            }
        }
        /// <summary>
        /// YSDM
        /// </summary>
        private string ysdm;


        /// <summary>
        /// ZJNR
        /// </summary>
        [DataColumn("ZJNR")]
        public string ZJNR
        {
            get { return zjnr; }
            set
            {
                zjnr = value;
                NotifyPropertyChanged("ZJNR");
            }
        }
        /// <summary>
        /// ZJNR
        /// </summary>
        private string zjnr;


        /// <summary>
        /// ZT
        /// </summary>
        [DataColumn("ZT")]
        public string ZT
        {
            get { return zt; }
            set
            {
                zt = value;
                NotifyPropertyChanged("ZT");
            }
        }
        /// <summary>
        /// ZT
        /// </summary>
        private string zt;

        /// <summary>
        /// YC
        /// </summary>
        [DataColumn("YC")]
        public string YC
        {
            get { return yc; }
            set
            {
                yc = value;
                NotifyPropertyChanged("YC");
            }
        }
        /// <summary>
        /// YC
        /// </summary>
        private string yc;

        /// <summary>
        /// BS
        /// </summary>
        [DataColumn("BS")]
        public int BS
        {
            get { return bs; }
            set
            {
                bs = value;
                NotifyPropertyChanged("BS");
            }
        }
        /// <summary>
        /// BS
        /// </summary>
        private int bs;

        /// <summary>
        /// XZ
        /// </summary>
        [DataColumn("XZ")]
        public string XZ
        {
            get { return xz; }
            set
            {
                xz = value;
                NotifyPropertyChanged("XZ");
            }
        }
        /// <summary>
        /// XZ
        /// </summary>
        private string xz;

        /// <summary>
        /// XHX
        /// </summary>
        [DataColumn("XHX")]
        public string XHX
        {
            get { return xhx; }
            set
            {
                xhx = value;
                NotifyPropertyChanged("XHX");
            }
        }
        /// <summary>
        /// XHX
        /// </summary>
        private string xhx;

        /// <summary>
        /// KD
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
        /// KD
        /// </summary>
        private double kd;


        /// <summary>
        /// GD
        /// </summary>
        [DataColumn("GD")]
        public double GD
        {
            get { return gd; }
            set
            {
                gd = value;
                NotifyPropertyChanged("GD");
            }
        }
        /// <summary>
        /// GD
        /// </summary>
        private double gd;

        /// <summary>
        /// JG
        /// </summary>
        [DataColumn("JG")]
        public double JG
        {
            get { return jg; }
            set
            {
                jg = value;
                NotifyPropertyChanged("JG");
            }
        }
        /// <summary>
        /// JG
        /// </summary>
        private double jg;

        /// <summary>
        /// ZJDZXJXZB
        /// </summary>
        [DataColumn("ZJDZXJXZB")]
        public double ZJDZXJXZB
        {
            get { return zjdzxjxzb; }
            set
            {
                zjdzxjxzb = value;
                NotifyPropertyChanged("ZJDZXJXZB");
            }
        }
        /// <summary>
        /// ZJDZXJXZB
        /// </summary>
        private double zjdzxjxzb;

        /// <summary>
        /// ZJDZXJYZB
        /// </summary>
        [DataColumn("ZJDZXJYZB")]
        public double ZJDZXJYZB
        {
            get { return zjdzxjyzb; }
            set
            {
                zjdzxjyzb = value;
                NotifyPropertyChanged("ZJDZXJYZB");
            }
        }
        /// <summary>
        /// ZJDZXJYZB
        /// </summary>
        private double zjdzxjyzb;

        /// <summary>
        /// ZJFX
        /// </summary>
        [DataColumn("ZJFX")]
        public double ZJFX
        {
            get { return zjfx; }
            set
            {
                zjfx = value;
                NotifyPropertyChanged("ZJFX");
            }
        }
        /// <summary>
        /// ZJFX
        /// </summary>
        private double zjfx;
        
        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYBM")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                NotifyPropertyChanged("ZoneCode");
            }
        }
        /// <summary>
        /// DYBM
        /// </summary>
        private string zoneCode;
               
    }
}
