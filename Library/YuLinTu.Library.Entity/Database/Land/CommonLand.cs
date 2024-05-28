/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地块基类实体(定义公有字段属性)
    /// </summary>
    [Serializable]
    public class CommonLand : NotifyCDObject
    {
        #region Fields

        /// <summary>
        /// 地块名称
        /// </summary>
        private string name;

        /// <summary>
        /// 调查编码
        /// </summary>
        private string surveyNumber;

        /// <summary>
        /// 地块编码
        /// </summary>
        private string landNumber;

        /// <summary>
        /// 宗地编码
        /// </summary>
        private string parcelNumber;

        /// <summary>
        /// 地籍编码
        /// </summary>
        private string cadastralNumber;

        /// <summary>
        /// 坐落代码
        /// </summary>
        private string zoneCode;

        /// <summary>
        /// 坐落名称
        /// </summary>
        private string zoneName;

        /// <summary>
        /// 权属单位代码
        /// </summary>
        private string senderCode;

        /// <summary>
        /// 权属单位名称
        /// </summary>
        private string senderName;

        /// <summary>
        /// 承包方名称
        /// </summary>
        private string ownerName;

        /// <summary>
        /// 承包方标识
        /// </summary>
        private Guid? ownerId;

        /// <summary>
        /// 土地利用类型
        /// </summary>
        private string landCode;

        /// <summary>
        /// 土地利用类型名称
        /// </summary>
        private string landName;

        /// <summary>
        /// 实测面积
        /// </summary>
        private double actualArea;

        /// <summary>
        /// 颁证面积
        /// </summary>
        private double awareArea;

        //private double contractDelayArea;

        /// <summary>
        /// 地块备注信息
        /// </summary>
        private string comment;

        /// <summary>
        /// 源编码
        /// </summary>
        private string sourceNumber;

        /// <summary>
        /// 四至东
        /// </summary>
        private string neighborEast;

        /// <summary>
        /// 四至西
        /// </summary>
        private string neighborWest;

        /// <summary>
        /// 四至南
        /// </summary>
        private string neighborSouth;

        /// <summary>
        /// 四至北
        /// </summary>
        private string neighborNorth;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 地块名称
        /// </summary>
        [DataColumn("DKMC")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 调查编码
        /// </summary>
        [DataColumn("DCBM")]
        public string SurveyNumber
        {
            get { return surveyNumber; }
            set
            {
                surveyNumber = value;
                NotifyPropertyChanged("SurveyNumber");
            }
        }

        /// <summary>
        /// 地块编码
        /// </summary>
        [DataColumn("DKBM")]
        public string LandNumber
        {
            get { return landNumber; }
            set
            {
                landNumber = value;
                NotifyPropertyChanged("LandNumber");
            }
        }

        /// <summary>
        /// 宗地编码
        /// </summary>
        [DataColumn("ZDBM")]
        public string ParcelNumber
        {
            get { return parcelNumber; }
            set
            {
                parcelNumber = value;
                NotifyPropertyChanged("ParcelNumber");
            }
        }

        /// <summary>
        /// 地籍编码
        /// </summary>
        [DataColumn("DJBM")]
        public string CadastralNumber
        {
            get { return cadastralNumber; }
            set
            {
                cadastralNumber = value;
                NotifyPropertyChanged("CadastralNumber");
            }
        }

        /// <summary>
        /// 坐落代码
        /// </summary>
        [DataColumn("ZLDM")]
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
        /// 坐落名称
        /// </summary>
        [DataColumn("ZLMC")]
        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                NotifyPropertyChanged("ZoneName");
            }
        }

        /// <summary>
        /// 权属单位代码
        /// </summary>
        [DataColumn("QSDWDM")]
        public string SenderCode
        {
            get { return senderCode; }
            set
            {
                senderCode = value;
                NotifyPropertyChanged("SenderCode");
            }
        }

        /// <summary>
        /// 权属单位名称
        /// </summary>
        [DataColumn("QSDWMC")]
        public string SenderName
        {
            get { return senderName; }
            set
            {
                senderName = value;
                NotifyPropertyChanged("SenderName");
            }
        }

        /// <summary>
        /// 承包方名称
        /// </summary>
        [DataColumn("QLRMC")]
        public string OwnerName
        {
            get { return ownerName; }
            set
            {
                ownerName = value;
                NotifyPropertyChanged("OwnerName");
            }
        }

        /// <summary>
        /// 承包方标识
        /// </summary>
        [DataColumn("QLRBS")]
        public Guid? OwnerId
        {
            get { return ownerId; }
            set
            {
                ownerId = value;
                NotifyPropertyChanged("OwnerId");
            }
        }

        /// <summary>
        /// 土地利用类型
        /// </summary>
        [DataColumn("TDLYLX")]
        public string LandCode
        {
            get { return landCode; }
            set
            {
                landCode = value;
                NotifyPropertyChanged("LandCode");
            }
        }

        /// <summary>
        /// 土地利用类型名称
        /// </summary>
        [DataColumn("TDLYLXMC")]
        public string LandName
        {
            get { return landName; }
            set
            {
                landName = value;
                NotifyPropertyChanged("LandName");
            }
        }

        /// <summary>
        /// 实测面积
        /// </summary>
        [DataColumn("SCMJ")]
        public double ActualArea
        {
            get { return actualArea; }
            set
            {
                actualArea = value;
                NotifyPropertyChanged("ActualArea");
            }
        }

        /// <summary>
        /// 颁证面积
        /// </summary>
        [DataColumn("BZMJ")]
        public double AwareArea
        {
            get { return awareArea; }
            set
            {
                awareArea = value;
                NotifyPropertyChanged("AwareArea");
            }
        }

        ///// <summary>
        /////延包面积
        ///// </summary>
        //[DataColumn("YBMJ")]
        //public double ContractDelayArea
        //{
        //    get { return contractDelayArea; }
        //    set
        //    {
        //        contractDelayArea = value;
        //        NotifyPropertyChanged("ContractDelayArea");
        //    }
        //}

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("DKBZXX")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
            }
        }

        /// <summary>
        /// 源编码
        /// </summary>
        [DataColumn("YBM")]
        public string SourceNumber
        {
            get { return sourceNumber; }
            set
            {
                sourceNumber = value;
                NotifyPropertyChanged("SourceNumber");
            }
        }

        /// <summary>
        /// 四至东(右)
        /// </summary>
        [DataColumn("DKDZ")]
        public string NeighborEast
        {
            get { return neighborEast; }
            set
            {
                neighborEast = value;
                NotifyPropertyChanged("NeighborEast");
            }
        }

        /// <summary>
        /// 四至西(左)
        /// </summary>
        [DataColumn("DKXZ")]
        public string NeighborWest
        {
            get { return neighborWest; }
            set
            {
                neighborWest = value;
                NotifyPropertyChanged("NeighborWest");
            }
        }

        /// <summary>
        /// 四至南(下)
        /// </summary>
        [DataColumn("DKNZ")]
        public string NeighborSouth
        {
            get { return neighborSouth; }
            set
            {
                neighborSouth = value;
                NotifyPropertyChanged("NeighborSouth");
            }
        }

        /// <summary>
        /// 四至北(上)
        /// </summary>
        [DataColumn("DKBZ")]
        public string NeighborNorth
        {
            get { return neighborNorth; }
            set
            {
                neighborNorth = value;
                NotifyPropertyChanged("NeighborNorth");
            }
        }

        #endregion Properties
    }
}