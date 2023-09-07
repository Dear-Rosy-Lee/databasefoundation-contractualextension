// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址线
    /// </summary>   
    [Serializable]
    [DataTable("JZX")]
    public class BuildLandBoundaryAddressCoil : NotifyCDObject, IComparable
    {
        #region Fields

        private Guid id;
        private double coilLength;
        private string lineType;
        private string coilType;
        private string position;
        private string agreementNumber;
        private string agreementBook;
        private string controversyNumber;
        private string controversyBook;
        private short orderID;
        private Guid startPointID;
        private Guid endPointID;
        private Guid landID;
        private string founder;
        private DateTime? creationTime;
        private string modifier;
        private DateTime? modifiedTime;
        private string description;
        private string neighborFefer;
        private string neighborPerson;
        private string zoneCode;
        private string landType;
        private string comment;
        private string landNumber;
        private string startNumber;
        private string endNumber;
        private Geometry shape;

        #endregion

        #region  Ctor

        static BuildLandBoundaryAddressCoil()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryNatrueType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryLineCategory);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryLinePosition);
        }

        #endregion

        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public virtual Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        /// <summary>
        ///界址线长度
        /// </summary>
        [DataColumn("JZXCD")]
        public double CoilLength
        {
            get { return coilLength; }
            set
            {
                coilLength = value;
                NotifyPropertyChanged("CoilLength");
            }
        }

        /// <summary>
        ///界线性质(见城镇地籍表29)
        /// </summary>
        [DataColumn("JXXZ")]
        public string LineType
        {
            get { return lineType; }
            set
            {
                lineType = value;
                NotifyPropertyChanged("LineType");
            }
        }

        /// <summary>
        ///界址线类别(围墙、墙壁、红线、界线)
        /// </summary>
        [DataColumn("JZXLB")]
        public string CoilType
        {
            get { return coilType; }
            set
            {
                coilType = value;
                NotifyPropertyChanged("CoilType");
            }
        }

        /// <summary>
        ///界址线位置(内、中、外)
        /// </summary>
        [DataColumn("JZXWZ")]
        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
        }

        /// <summary>
        ///权属界线协议书编号
        /// </summary>
        [DataColumn("XYSBH")]
        public string AgreementNumber
        {
            get { return agreementNumber; }
            set
            {
                agreementNumber = value;
                NotifyPropertyChanged("AgreementNumber");
            }
        }

        /// <summary>
        ///权属界线协议书
        /// </summary>
        [DataColumn("XYS")]
        public string AgreementBook
        {
            get { return agreementBook; }
            set
            {
                agreementBook = value;
                NotifyPropertyChanged("AgreementBook");
            }
        }

        /// <summary>
        ///权属争议缘由书编号
        /// </summary>
        [DataColumn("ZYYYSBH")]
        public string ControversyNumber
        {
            get { return controversyNumber; }
            set
            {
                controversyNumber = value;
                NotifyPropertyChanged("ControversyNumber");
            }
        }

        /// <summary>
        ///权属争议缘由书
        /// </summary>
        [DataColumn("ZYYYS")]
        public string ControversyBook
        {
            get { return controversyBook; }
            set
            {
                controversyBook = value;
                NotifyPropertyChanged("ControversyBook");
            }
        }

        /// <summary>
        ///界址线顺序号
        /// </summary>
        [DataColumn("JZXSXH")]
        public short OrderID
        {
            get { return orderID; }
            set
            {
                orderID = value;
                NotifyPropertyChanged("OrderID");
            }
        }

        /// <summary>
        ///起点
        /// </summary>
        [DataColumn("JZXQD")]
        public Guid StartPointID
        {
            get { return startPointID; }
            set
            {
                startPointID = value;
                NotifyPropertyChanged("StartPointID");
            }
        }

        /// <summary>
        ///终点
        /// </summary>
        [DataColumn("JZXZD")]
        public Guid EndPointID
        {
            get { return endPointID; }
            set
            {
                endPointID = value;
                NotifyPropertyChanged("EndPointID");
            }
        }

        /// <summary>
        ///集体建设用地使用权ID
        /// </summary>
        [DataColumn("DKBS")]
        public Guid LandID
        {
            get { return landID; }
            set
            {
                landID = value;
                NotifyPropertyChanged("LandID");
            }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("CJZ")]
        public string Founder
        {
            get { return founder; }
            set
            {
                founder = value;
                NotifyPropertyChanged("Founder");
            }
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreationTime
        {
            get { return creationTime; }
            set
            {
                creationTime = value;
                NotifyPropertyChanged("CreationTime");
            }
        }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("ZHXGZ")]
        public string Modifier
        {
            get { return modifier; }
            set
            {
                modifier = value;
                NotifyPropertyChanged("Modifier");
            }
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataColumn("ZHXGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set
            {
                modifiedTime = value;
                NotifyPropertyChanged("ModifiedTime");
            }
        }

        /// <summary>
        ///说明
        /// </summary>
        [DataColumn("JZXSM")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        /// <summary>
        ///毗邻地块指界人
        /// </summary>
        [DataColumn("PLDWZJR")]
        public string NeighborFefer
        {
            get { return neighborFefer; }
            set
            {
                neighborFefer = value;
                NotifyPropertyChanged("NeighborFefer");
            }
        }

        /// <summary>
        ///毗邻地物权利人
        /// </summary>
        [DataColumn("PLDWQLR")]
        public string NeighborPerson
        {
            get { return neighborPerson; }
            set
            {
                neighborPerson = value;
                NotifyPropertyChanged("NeighborPerson");
            }
        }

        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYDM")]
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
        ///界址线所属权利类型
        /// </summary>
        [DataColumn("TDQSLX")]
        public string LandType
        {
            get { return landType; }
            set
            {
                landType = value;
                NotifyPropertyChanged("LandType");
            }
        }

        /// <summary>
        ///备注
        /// </summary>
        [DataColumn("BZXX")]
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
        /// 起点号
        /// </summary>
        [DataColumn("JZXQDH")]
        public string StartNumber
        {
            get { return startNumber; }
            set
            {
                startNumber = value;
                NotifyPropertyChanged("StartNumber");
            }
        }

        /// <summary>
        /// 终点号
        /// </summary>
        [DataColumn("JZXZDH")]
        public string EndNumber
        {
            get { return endNumber; }
            set
            {
                endNumber = value;
                NotifyPropertyChanged("EndNumber");
            }
        }

        /// <summary>
        /// 图形
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

        #endregion

        #region Ctor

        public BuildLandBoundaryAddressCoil()
        {
            ID = Guid.NewGuid();
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            LineType = ((int)eBoundaryNatureType.Other).ToString();
            CoilType = ((int)eBoundaryLineCategory.Other).ToString();
            Position = ((int)eBoundaryLinePosition.Left).ToString();
            LandType = ((int)eLandPropertyRightType.AgricultureLand).ToString();
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 升序
        /// </summary>
        /// <param name="obj">需要比较的实体</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            BuildLandBoundaryAddressCoil dot = obj as BuildLandBoundaryAddressCoil;

            if (this.OrderID > dot.OrderID)
                return 1;
            else if (this.OrderID < dot.OrderID)
                return -1;

            return 0;
        }

        #endregion
    }

    [Serializable]
    [DataTable("JZX")]
    public class TempBLBAC : BuildLandBoundaryAddressCoil
    {
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public override Guid ID
        {
            get
            {
                return base.ID;
            }

            set
            {
                base.ID = value;
            }
        }
    }
}