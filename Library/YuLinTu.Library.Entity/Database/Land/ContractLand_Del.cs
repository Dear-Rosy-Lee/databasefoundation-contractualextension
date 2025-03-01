using System;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包地块_DEL
    /// </summary>
    [Serializable]
    [DataTable("TZ_SCDKB")]
    public class ContractLand_Del : NotifyCDObject
    {
        #region Fields
        private Guid id;
        private string dkmc;
        private string dkbm;
        private double bzmj;
        private double scmj;
        private Guid? cbfid;
        private string dkdz;
        private string dknz;
        private string dkxz;
        private string dkbz;
        private string bzxx;
        private string dybm;
        #endregion Fields

        #region Properties
        [DataColumn("ID")]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }
        [DataColumn("DKMC")]
        public string DKMC
        {
            get { return dkmc; }
            set
            {
                dkmc = value;
                NotifyPropertyChanged("DKMC");
            }
        }
        [DataColumn("DKBM")]
        public string DKBM
        {
            get { return dkbm; }
            set
            {
                dkbm = value;
                NotifyPropertyChanged("DKBM");
            }
        }
        [DataColumn("QQDKBM")]
        public string QQDKBM
        {
            get { return qqdkbm; }
            set
            {
                qqdkbm = value;
                NotifyPropertyChanged("DKBM");
            }
        }
        private string qqdkbm;
        /// <summary>
        /// 实测面积
        /// </summary>
        [DataColumn("SCMJ")]
        public double SCMJ
        {
            get { return scmj; }
            set
            {
                scmj = value;
                NotifyPropertyChanged("SCMJ");
            }
        }

        /// <summary>
        /// 颁证面积
        /// </summary>
        [DataColumn("QQMJ")]
        public double QQMJ
        {
            get { return bzmj; }
            set
            {
                bzmj = value;
                NotifyPropertyChanged("QQMJ");
            }
        }

        /// <summary>
        /// 承包方id
        /// </summary>
        [DataColumn("CBFID")]
        public Guid? CBFID
        {
            get { return cbfid; }
            set
            {
                cbfid = value;
                NotifyPropertyChanged("CBFID");
            }
        }


        [DataColumn("DKDZ")]
        public string DKDZ
        {
            get { return dkdz; }
            set
            {
                dkdz = value;
                NotifyPropertyChanged("DKDZ");
            }
        }
        [DataColumn("DKNZ")]
        public string DKNZ
        {
            get { return dknz; }
            set
            {
                dknz = value;
                NotifyPropertyChanged("DKNZ");
            }
        }
        [DataColumn("DKXZ")]
        public string DKXZ
        {
            get { return dkxz; }
            set
            {
                dkxz = value;
                NotifyPropertyChanged("DKXZ");
            }
        }
        [DataColumn("DKBZ")]
        public string DKBZ
        {
            get { return dkbz; }
            set
            {
                dkbz = value;
                NotifyPropertyChanged("DKBZ");
            }
        }
        [DataColumn("BZXX")]
        public string BZXX
        {
            get { return bzxx; }
            set
            {
                bzxx = value;
                NotifyPropertyChanged("BZXX");
            }
        }
        [DataColumn("DYBM")]
        public string DYBM
        {
            get { return dybm; }
            set
            {
                dybm = value;
                NotifyPropertyChanged("DYBM");
            }
        }

        [DataColumn("SYQXZ")]
        public string SYQXZ
        {
            get; set;
        }

        [DataColumn("DKLB")]
        public string DKLB
        {
            get; set;
        }

        [DataColumn("TDLYLX")]
        public string TDLYLX
        {
            get; set;
        }

        [DataColumn("TDYT")]
        public string TDYT
        {
            get; set;
        }

        [DataColumn("SFJBNT")]
        public string SFJBNT
        {
            get; set;
        }

        [DataColumn("DLDJ")]
        public string DLDJ
        {
            get; set;
        }

        #endregion Properties

        /// <summary>
        /// 转换对象
        /// </summary> 
        public static ContractLand_Del ChangeDataEntity(string zoneCode, ContractLand ld)
        {
            var data = new ContractLand_Del()
            {
                BZXX = ld.Comment,
                DKBM = ld.LandNumber,
                DKBZ = ld.NeighborNorth,
                DKXZ = ld.NeighborWest,
                DKNZ = ld.NeighborSouth,
                DKDZ = ld.NeighborEast,
                DKMC = ld.Name,
                DYBM = zoneCode,
                ID = ld.ID,
                QQDKBM = ld.OldLandNumber,
                QQMJ = ld.AwareArea,
                SCMJ = ld.ActualArea,
                CBFID = ld.OwnerId,
                TDYT = ld.Purpose,
                TDLYLX = ld.LandCode,
                SYQXZ = ld.OwnRightType,
                DLDJ = ld.LandLevel,
                DKLB = ld.LandCategory,
                SFJBNT = ld.IsFarmerLand == null ? "" : ld.IsFarmerLand + ""
            };
            return data;
        }
    }
}
