// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using System.Text.RegularExpressions;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 集体建设用地
    /// </summary>
    [DataTable("ConstructionLand")]
    [Serializable]
    public class ConstructionLand : CommonLand
    {
        #region Ctor

        static ConstructionLand()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandUseRightType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandLevel);
        }

        #endregion

        #region Fields

        /// <summary>
        ///标识码
        /// </summary>
        private Guid id;

        /// <summary>
        ///宗地四至
        /// </summary>
        private string landNeighbor;

        /// <summary>
        ///通讯地址
        /// </summary>
        private string address;

        /// <summary>
        ///权属性质
        /// </summary>
        private eLandPropertyType ownRightType;

        /// <summary>
        ///使用权类型
        /// </summary>
        private LandUseRightType useRightType;

        /// <summary>
        ///土地用途
        /// </summary>
        private string landPurpose;

        ///// <summary>
        /////实测面积，已废弃
        /////请使用 SelfArea
        ///// </summary>
        //private double actualArea;

        ///// <summary>
        /////发证面积，已废弃
        /////请使用privateArea
        ///// </summary>
        //private double? awareArea;

        /// <summary>
        ///合计面积
        /// </summary>
        private double? countArea;

        /// <summary>
        ///批准农村宅基地面积
        /// </summary>
        private double? approveLandArea;

        /// <summary>
        ///建房日期
        /// </summary>
        private DateTime? buildHouseDate;

        /// <summary>
        ///建筑容积率
        /// </summary>
        private double? frameCapacityRatio;

        /// <summary>
        ///建筑密度
        /// </summary>
        private double? frameDensity;

        /// <summary>
        ///土地级别
        /// </summary>
        private eLandLevel landLevel;

        /// <summary>
        ///申报地价
        /// </summary>
        private double? declareLandPrice;

        /// <summary>
        ///取得价格
        /// </summary>
        private double? realityPrice;

        /// <summary>
        ///集体建设用地使用权附件路径
        /// </summary>
        private string path;

        /// <summary>
        ///开始审核
        /// </summary>
        private bool beginCheck;

        /// <summary>
        ///审核完成
        /// </summary>
        private bool endCheck;

        /// <summary>
        ///是否打证
        /// </summary>
        private bool isPrint;

        /// <summary>
        ///状态 登记10、审核20、核发权证30
        /// </summary>
        private int status;

        /// <summary>
        ///扩展属性A 备用
        ///疑似B.S中已使用
        /// </summary>
        private string extendA;

        /// <summary>
        ///扩展属性B 备用
        ///目前已用户存储 集体建设用地使用权确权审批表编号。
        /// </summary>
        private string extendB;

        /// <summary>
        ///扩展属性C 备用
        ///目前已用户存储 集体建设用地使用权确权审批表年号。
        /// </summary>
        private string extendC;

        /// <summary>
        ///创建者
        /// </summary>
        private string founder;

        /// <summary>
        ///创建时间
        /// </summary>
        private DateTime? creationTime;

        /// <summary>
        ///最后修改者
        /// </summary>
        private string modifier;

        /// <summary>
        ///最后修改时间
        /// </summary>
        private DateTime? modifiedTime;

        ///// <summary>
        /////备注
        ///// </summary>
        //private string comment;

        /// <summary>
        ///实测面积
        /// </summary>
        private double selfArea;

        /// <summary>
        ///确权面积
        /// </summary>
        private double privateArea;

        /// <summary>
        ///违法用地面积
        /// </summary>
        private double illegalArea;

        /// <summary>
        ///超占面积
        /// </summary>
        private double exceedArea;

        /// <summary>
        ///地类名称
        /// </summary>
        private string landTypeName;

        /// <summary>
        ///是否可用
        /// </summary>
        private bool isValid;

        /// <summary>
        /// 地籍区编号
        /// </summary>
        private string cadastralZoneCode;

        #endregion

        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", ColumnType = eDataType.Guid)]
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
        ///宗地四至
        /// </summary>
        [DataColumn("LandNeighbor", ColumnType = eDataType.String)]
        public string LandNeighbor 
        { 
            get { return landNeighbor; } 
            set 
            {
                landNeighbor = value;
                NotifyPropertyChanged("LandNeighbor");
            }
        }

        /// <summary>
        ///通讯地址
        /// </summary>
        [DataColumn("Address", ColumnType = eDataType.String)]
        public string Address 
        { 
            get { return address; } 
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        /// <summary>
        ///权属性质
        /// </summary>
        [DataColumn("OwnRightType", ColumnType = eDataType.Int32)]
        private eLandPropertyType OwnRightType
        {
            get { return ownRightType; }
            set
            {
                ownRightType = value;
                NotifyPropertyChanged("OwnRightType");
            }
        }

        /// <summary>
        ///使用权类型
        /// </summary>
        [DataColumn("UseRightType", ColumnType = eDataType.Int32)]
        public LandUseRightType UseRightType
        {
            get { return useRightType; }
            set 
            {
                useRightType = value;
                NotifyPropertyChanged("UseRightType");
            }
        }

        /// <summary>
        ///土地用途
        /// </summary>
        [DataColumn("LandPurpose", ColumnType = eDataType.String)]
        public string LandPurpose
        {
            get { return landPurpose; }
            set
            {
                landPurpose = value;
                NotifyPropertyChanged("LandPurpose");
            }
        }

        ///// <summary>
        /////实测面积，已废弃
        /////请使用 SelfArea
        ///// </summary>
        //[DataColumn("ActualArea", ColumnType = eDataType.Decimal)]
        //public double ActualArea
        //{
        //    get { return actualArea; }
        //    set
        //    {
        //        actualArea = value;
        //        NotifyPropertyChanged("ActualArea");
        //    }
        //}

        ///// <summary>
        /////发证面积，已废弃
        /////请使用PrivateArea
        ///// </summary>
        //[DataColumn("AwareArea", ColumnType = eDataType.Decimal)]
        //public double? AwareArea
        //{
        //    get { return awareArea; }
        //    set
        //    {
        //        awareArea = value;
        //        NotifyPropertyChanged("AwareArea");
        //    }
        //}

        /// <summary>
        ///合计面积
        /// </summary>
        [DataColumn("CountArea", ColumnType = eDataType.Decimal)]
        public double? CountArea
        {
            get { return countArea; }
            set
            {
                countArea = value;
                NotifyPropertyChanged("CountArea");
            }
        }

        /// <summary>
        ///批准农村宅基地面积
        /// </summary>
        [DataColumn("ApproveLandArea", ColumnType = eDataType.Decimal)]
        public double? ApproveLandArea
        {
            get { return approveLandArea; }
            set
            {
                approveLandArea = value;
                NotifyPropertyChanged("ApproveLandArea");
            }
        }

        /// <summary>
        ///建房日期
        /// </summary>
        [DataColumn("BuildHouseDate", ColumnType = eDataType.DateTime)]
        public DateTime? BuildHouseDate
        {
            get { return buildHouseDate; }
            set
            {
                buildHouseDate = value;
                NotifyPropertyChanged("BuildHouseDate");
            }
        }

        /// <summary>
        ///建筑容积率
        /// </summary>
        [DataColumn("FrameCapacityRatio", ColumnType = eDataType.Decimal)]
        public double? FrameCapacityRatio
        {
            get { return frameCapacityRatio; }
            set
            {
                frameCapacityRatio = value;
                NotifyPropertyChanged("FrameCapacityRatio");
            }
        }

        /// <summary>
        ///建筑密度
        /// </summary>
        [DataColumn("FrameDensity", ColumnType = eDataType.Decimal)]
        public double? FrameDensity
        {
            get { return frameDensity; }
            set
            {
                frameDensity = value;
                NotifyPropertyChanged("FrameDensity");
            }
        }

        /// <summary>
        ///土地级别
        /// </summary>
        [DataColumn("LandLevel", ColumnType = eDataType.Int32)]
        public eLandLevel LandLevel
        {
            get { return landLevel; }
            set
            {
                landLevel = value;
                NotifyPropertyChanged("LandLevel");
            }
        }

        /// <summary>
        ///申报地价
        /// </summary>
        [DataColumn("DeclareLandPrice", ColumnType = eDataType.Decimal)]
        public double? DeclareLandPrice
        {
            get { return declareLandPrice; }
            set
            {
                declareLandPrice = value;
                NotifyPropertyChanged("DeclareLandPrice");
            }
        }

        /// <summary>
        ///取得价格
        /// </summary>
        [DataColumn("RealityPrice", ColumnType = eDataType.Decimal)]
        public double? RealityPrice
        {
            get { return realityPrice; }
            set
            {
                realityPrice = value;
                NotifyPropertyChanged("RealityPrice");
            }
        }

        /// <summary>
        ///集体建设用地使用权附件路径
        /// </summary>
        [DataColumn("Path", ColumnType = eDataType.String)]
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                NotifyPropertyChanged("Path");
            }
        }

        /// <summary>
        ///开始审核
        /// </summary>
        [DataColumn("BeginCheck", ColumnType = eDataType.Boolean)]
        public bool BeginCheck
        {
            get { return beginCheck; }
            set
            {
                beginCheck = value;
                NotifyPropertyChanged("BeginCheck");
            }
        }

        /// <summary>
        ///审核完成
        /// </summary>
        [DataColumn("EndCheck", ColumnType = eDataType.Boolean)]
        public bool EndCheck
        {
            get { return endCheck; }
            set
            {
                endCheck = value;
                NotifyPropertyChanged("EndCheck");
            }
        }

        /// <summary>
        ///是否打证
        /// </summary>
        [DataColumn("IsPrint", ColumnType = eDataType.Boolean)]
        public bool IsPrint
        {
            get { return isPrint; }
            set
            {
                isPrint = value;
                NotifyPropertyChanged("IsPrint");
            }
        }

        /// <summary>
        ///状态 登记10、审核20、核发权证30
        /// </summary>
        [DataColumn("Status", ColumnType = eDataType.Int32)]
        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        ///扩展属性A 备用
        ///疑似B.S中已使用
        /// </summary>
        [DataColumn("ExtendA", ColumnType = eDataType.String)]
        public string ExtendA
        {
            get { return extendA; }
            set
            {
                extendA = value;
                NotifyPropertyChanged("ExtendA");
            }
        }

        /// <summary>
        ///扩展属性B 备用
        ///目前已用户存储 集体建设用地使用权确权审批表编号。
        /// </summary>
        [DataColumn("ExtendB", ColumnType = eDataType.String)]
        public string ExtendB
        {
            get { return extendB; }
            set
            {
                extendB = value;
                NotifyPropertyChanged("ExtendB");
            }
        }

        /// <summary>
        ///扩展属性C 备用
        ///目前已用户存储 集体建设用地使用权确权审批表年号。
        /// </summary>
        [DataColumn("ExtendC", ColumnType = eDataType.String)]
        public string ExtendC
        {
            get { return extendC; }
            set
            {
                extendC = value;
                NotifyPropertyChanged("ExtendC");
            }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("Founder", ColumnType = eDataType.String)]
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
        [DataColumn("CreationTime", ColumnType = eDataType.DateTime)]
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
        [DataColumn("Modifier", ColumnType = eDataType.String)]
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
        [DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set
            {
                modifiedTime = value;
                NotifyPropertyChanged("ModifiedTime");
            }
        }

        ///// <summary>
        /////备注
        ///// </summary>
        //[DataColumn("Comment", ColumnType = eDataType.String)]
        //public string Comment
        //{
        //    get { return comment; }
        //    set
        //    {
        //        comment = value;
        //        NotifyPropertyChanged("Comment");
        //    }
        //}

        /// <summary>
        ///实测面积
        /// </summary>
        [DataColumn("SelfArea", ColumnType = eDataType.Decimal)]
        public double SelfArea
        {
            get { return selfArea; }
            set
            {
                selfArea = value;
                NotifyPropertyChanged("SelfArea");
            }
        }

        /// <summary>
        ///确权面积
        /// </summary>
        [DataColumn("PublicArea", ColumnType = eDataType.Decimal)]
        public double PrivateArea
        {
            get { return privateArea; }
            set
            {
                privateArea = value;
                NotifyPropertyChanged("PrivateArea");
            }
        }

        /// <summary>
        ///违法用地面积
        /// </summary>
        [DataColumn("IllegalArea", ColumnType = eDataType.Decimal)]
        public double IllegalArea
        {
            get { return illegalArea; }
            set
            {
                illegalArea = value;
                NotifyPropertyChanged("IllegalArea");
            }
        }

        /// <summary>
        ///超占面积
        /// </summary>
        [DataColumn("ExceedArea", ColumnType = eDataType.Decimal)]
        public double ExceedArea
        {
            get { return exceedArea; }
            set
            {
                exceedArea = value;
                NotifyPropertyChanged("ExceedArea");
            }
        }

        /// <summary>
        ///地类名称
        /// </summary>
        [DataColumn("LandTypeName", ColumnType = eDataType.String)]
        public string LandTypeName
        {
            get { return landTypeName; }
            set
            {
                landTypeName = value;
                NotifyPropertyChanged("LandTypeName");
            }
        }

        /// <summary>
        ///是否可用
        /// </summary>
        [DataColumn("IsValid", ColumnType = eDataType.Boolean)]
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                NotifyPropertyChanged("IsValid");
            }
        }

        /// <summary>
        /// 地籍区编号
        /// </summary>
        [DataColumn("CadastralZoneCode", ColumnType = eDataType.String)]
        public string CadastralZoneCode
        {
            get { return cadastralZoneCode; }
            set
            {
                cadastralZoneCode = value;
                NotifyPropertyChanged("CadastralZoneCode");
            }
        }

        #endregion

        #region Ctor

        public ConstructionLand()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            OwnRightType = eLandPropertyType.Collectived;
            UseRightType = LandUseRightType.AssignHouse;
            LandPurpose = "072";
            LandLevel = eLandLevel.UnKnow;
            BuildHouseDate = DateTime.Now;
            LandTypeName = "农村宅基地";
        }

        #endregion

        #region Methods

        public string[] GetLandNeighbors()
        {
            if (LandNeighbor == null || LandNeighbor.Length < 4)
                return new string[] { "", "", "", "" };

            string[] neighbors = LandNeighbor.Split(new char[] { '\r' });
            if (neighbors == null || neighbors.Length != 4)
            {
                neighbors = LandNeighbor.Split(new char[] { '\n' });//xml中会变成\n
                if (neighbors == null || neighbors.Length != 4)
                    return new string[] { "", "", "", "" };
            }

            return neighbors;
        }

        #endregion
    }
}