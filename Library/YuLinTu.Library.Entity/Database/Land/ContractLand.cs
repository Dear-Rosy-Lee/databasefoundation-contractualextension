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
    /// 农村土地承包地块
    /// </summary>
    [Serializable]
    [DataTable("ZD_CBD")]
    public class ContractLand : CommonLand
    {
        #region Fields

        /// <summary>
        /// 标识码
        /// </summary>
        private Guid id;

        /// <summary>
        /// 地块类别
        /// </summary>
        private string landcategory;

        /// <summary>
        /// 土地等级
        /// </summary>
        private string landLevel;

        /// <summary>
        /// 耕保类型
        /// </summary>
        private string plantType;

        /// <summary>
        /// 耕地坡度级
        /// </summary>
        private string landScopeLevel;

        /// <summary>
        /// 权属性质
        /// </summary>
        private string ownRightType;

        /// <summary>
        /// 线状地物面积
        /// </summary>
        private double? lineArea;

        /// <summary>
        /// 台账面积
        /// </summary>
        private double? tableArea;

        private double contractDelayArea;

        /// <summary>
        /// 是否基本农田
        /// </summary>
        private bool? isFarmerLand;

        /// <summary>
        /// 土地用途
        /// </summary>
        private string purpose;

        /// <summary>
        /// 经营方式
        /// </summary>
        private string managementType;

        /// <summary>
        /// 原户主姓名（曾经耕种）
        /// </summary>
        private string formerPerson;

        /// <summary>
        /// 土地肥沃力
        /// </summary>
        private string soiltype;

        /// <summary>
        /// 是否飞地
        /// </summary>
        private bool isFlyLand;

        /// <summary>
        /// 合同ID
        /// </summary>
        private Guid? concordID;

        /// <summary>
        /// 创建者
        /// </summary>
        private string founder;

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime? creationTime;

        /// <summary>
        /// 最后修改者
        /// </summary>
        private string modifier;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        private DateTime? modifiedTime;

        /// <summary>
        /// 是否流转
        /// </summary>
        private bool isTransfer;

        /// <summary>
        /// 流转类型
        /// </summary>
        private string transferType;

        /// <summary>
        /// 流转期限
        /// </summary>
        private string transferTime;

        /// <summary>
        /// 流转给谁
        /// </summary>
        private string transferWhere;

        /// <summary>
        /// 畦数
        /// </summary>
        private string plotNumber;

        /// <summary>
        /// 机动地面积
        /// </summary>
        private double? motorizeLandArea;

        /// <summary>
        ///备用字段A 已用于
        ///1、宗地座落方位描述
        /// </summary>
        private string extendA;

        /// <summary>
        /// 备用字段B(宗地统一编码)
        /// </summary>
        private string extendB;

        /// <summary>
        /// 备用字段C
        /// </summary>
        private string extendC;

        /// <summary>
        /// 状态 登记10、审核20
        /// </summary>
        private string status;

        /// <summary>
        /// 附属地面积(流转面积)
        /// </summary>
        private double pertainToArea;

        /// <summary>
        /// 承包起始日期
        /// </summary>
        private string arableLandTime;

        /// <summary>
        /// 长*宽
        /// </summary>
        private string widthHeight;

        /// <summary>
        /// 地籍区编号
        /// </summary>
        private string cadastralZoneCode;

        /// <summary>
        /// 种植类型
        /// </summary>
        private string platType;

        /// <summary>
        /// 承包方式
        /// </summary>
        private string constructMode;

        /// <summary>
        /// 预留A
        /// </summary>
        private string aliasNameA;

        /// <summary>
        /// 预留B
        /// </summary>
        private string aliasNameB;

        /// <summary>
        /// 预留C
        /// </summary>
        private string aliasNameC;

        /// <summary>
        /// 预留D
        /// </summary>
        private string aliasNameD;

        /// <summary>
        /// 预留E
        /// </summary>
        private string aliasNameE;

        /// <summary>
        /// 预留F
        /// </summary>
        private string aliasNameF;

        /// <summary>
        /// 扩展信息contract
        /// </summary>
        private string landExpand;

        /// <summary>
        /// 空间字段
        /// </summary>
        private Geometry shape;

        private string shareArea;

        private string concordArea;

        private double stockQuantity;

        private string stockQuantityAdv;

        private string quantificatAreaByLand;

        private string quantificatAreaByStock;

        private string modulus;

        private string opinion;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 共用面积
        /// </summary>
        [DataColumn("GYMJ")]
        public string ShareArea
        {
            get { return shareArea; }
            set { shareArea = value; NotifyPropertyChanged("ShareArea"); }
        }

        /// <summary>
        /// 合同面积
        /// </summary>
        [DataColumn("HTMJ")]
        public string ConcordArea
        {
            get
            {
                return concordArea;
            }
            set
            {
                concordArea = value;
                NotifyPropertyChanged("ConcordArea");
            }
        }

        /// <summary>
        /// 股权数量
        /// </summary>
        [DataColumn("GQSL")]
        public double StockQuantity
        {
            get { return stockQuantity; }
            set { stockQuantity = value; NotifyPropertyChanged("StockQuantity"); }
        }

        /// <summary>
        /// 人均股权数量
        /// </summary>
        [DataColumn("RJGQSL")]
        public string StockQuantityAdv
        {
            get { return stockQuantityAdv; }
            set { stockQuantityAdv = value; NotifyPropertyChanged("StockQuantityAdv"); }
        }

        /// <summary>
        /// 量化户面积（确地）
        /// </summary>
        [DataColumn("LHHMJD")]
        public string QuantificatAreaByLand
        {
            get { return quantificatAreaByLand; }
            set { quantificatAreaByLand = value; NotifyPropertyChanged("QuantificatAreaByLand"); }
        }

        /// <summary>
        /// 量化户面积（确股）
        /// </summary>
        [DataColumn("LHHMJG")]
        public string QuantificatAreaByStock
        {
            get { return quantificatAreaByStock; }
            set { quantificatAreaByStock = value; NotifyPropertyChanged("QuantificatAreaByStock"); }
        }

        /// <summary>
        /// 系数
        /// </summary>
        [DataColumn("XS")]
        public string Modulus
        {
            get { return modulus; }
            set { modulus = value; NotifyPropertyChanged("Modulus"); }
        }

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
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
        ///地块类别
        /// </summary>
        [DataColumn("DKLB")]
        public string LandCategory
        {
            get { return landcategory; }
            set
            {
                landcategory = value.TrimSafe();
                NotifyPropertyChanged("LandCategory");
            }
        }

        /// <summary>
        ///地力等级
        /// </summary>
        [DataColumn("DLDJ")]
        public string LandLevel
        {
            get { return landLevel; }
            set
            {
                landLevel = value.TrimSafe();
                NotifyPropertyChanged("LandLevel");
            }
        }

        /// <summary>
        ///耕保类型
        /// </summary>
        [DataColumn("GBLX")]
        public string PlantType
        {
            get { return plantType; }
            set
            {
                plantType = value.TrimSafe();
                NotifyPropertyChanged("PlantType");
            }
        }

        /// <summary>
        ///耕地坡度级
        /// </summary>
        [DataColumn("GDPDJ")]
        public string LandScopeLevel
        {
            get { return landScopeLevel; }
            set
            {
                landScopeLevel = value.TrimSafe();
                NotifyPropertyChanged("LandScopeLevel");
            }
        }

        /// <summary>
        ///权属性质
        /// </summary>
        [DataColumn("QSXZ")]
        public string OwnRightType
        {
            get { return ownRightType; }
            set
            {
                ownRightType = value.TrimSafe();
                NotifyPropertyChanged("OwnRightType");
            }
        }

        /// <summary>
        ///线状地物面积
        /// </summary>
        [DataColumn("XZDWMJ")]
        public double? LineArea
        {
            get { return lineArea; }
            set
            {
                lineArea = value;
                NotifyPropertyChanged("LineArea");
            }
        }

        /// <summary>
        ///台账面积
        /// </summary>
        [DataColumn("TZMJ")]
        public double? TableArea
        {
            get { return tableArea; }
            set
            {
                tableArea = value;
                NotifyPropertyChanged("TableArea");
            }
        }

        /// <summary>
        ///延包面积
        /// </summary>
        [DataColumn("YBMJ")]
        public double ContractDelayArea
        {
            get { return contractDelayArea; }
            set
            {
                contractDelayArea = value;
                NotifyPropertyChanged("ContractDelayArea");
            }
        }

        /// <summary>
        /// 地块信息修改意见
        /// </summary>
        [DataColumn("DKXXXGYJ")]
        public string Opinion
        {
            get { return opinion; }
            set
            {
                opinion = value;
                NotifyPropertyChanged("Opinion");
            }
        }

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DataColumn("SFJBNT")]
        public bool? IsFarmerLand
        {
            get { return isFarmerLand; }
            set
            {
                isFarmerLand = value;
                NotifyPropertyChanged("IsFarmerLand");
            }
        }

        /// <summary>
        ///土地用途
        /// </summary>
        [DataColumn("TDYT")]
        public string Purpose
        {
            get { return purpose; }
            set
            {
                purpose = value.TrimSafe();
                NotifyPropertyChanged("Purpose");
            }
        }

        /// <summary>
        ///经营方式
        /// </summary>
        [DataColumn("JYFS")]
        public string ManagementType
        {
            get { return managementType; }
            set
            {
                managementType = value.TrimSafe();
                NotifyPropertyChanged("ManagementType");
            }
        }

        /// <summary>
        ///原户主姓名（曾经耕种）
        /// </summary>
        [DataColumn("YHZXM")]
        public string FormerPerson
        {
            get { return formerPerson; }
            set
            {
                formerPerson = value.TrimSafe();
                NotifyPropertyChanged("FormerPerson");
            }
        }

        /// <summary>
        ///土地肥沃力
        /// </summary>
        [DataColumn("TDFWL")]
        public string Soiltype
        {
            get { return soiltype; }
            set
            {
                soiltype = value.TrimSafe();
                NotifyPropertyChanged("Soiltype");
            }
        }

        /// <summary>
        ///是否飞地
        /// </summary>
        [DataColumn("SFFD")]
        public bool IsFlyLand
        {
            get { return isFlyLand; }
            set
            {
                isFlyLand = value;
                NotifyPropertyChanged("IsFlyLand");
            }
        }

        /// <summary>
        ///合同ID
        /// </summary>
        [DataColumn("HTID")]
        public Guid? ConcordId
        {
            get { return concordID; }
            set
            {
                concordID = value;
                NotifyPropertyChanged("ConcordId");
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
                founder = value.TrimSafe();
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
                modifier = value.TrimSafe();
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
        ///是否流转
        /// </summary>
        [DataColumn("SFLZ")]
        public bool IsTransfer
        {
            get { return isTransfer; }
            set
            {
                isTransfer = value;
                NotifyPropertyChanged("IsTransfer");
            }
        }

        /// <summary>
        ///流转类型
        /// </summary>
        [DataColumn("LZLX")]
        public string TransferType
        {
            get { return transferType; }
            set
            {
                transferType = value.TrimSafe();
                NotifyPropertyChanged("TransferType");
            }
        }

        /// <summary>
        ///流转期限
        /// </summary>
        [DataColumn("LZQX")]
        public string TransferTime
        {
            get { return transferTime; }
            set
            {
                transferTime = value.TrimSafe();
                NotifyPropertyChanged("TransferTime");
            }
        }

        /// <summary>
        ///流转给谁
        /// </summary>
        [DataColumn("LZGS")]
        public string TransferWhere
        {
            get { return transferWhere; }
            set
            {
                transferWhere = value.TrimSafe();
                NotifyPropertyChanged("TransferWhere");
            }
        }

        /// <summary>
        ///畦数
        /// </summary>
        [DataColumn("QS")]
        public string PlotNumber
        {
            get { return plotNumber; }
            set
            {
                plotNumber = value.TrimSafe();
                NotifyPropertyChanged("PlotNumber");
            }
        }

        /// <summary>
        ///机动地面积
        /// </summary>
        [DataColumn("JDDMJ")]
        public double? MotorizeLandArea
        {
            get { return motorizeLandArea; }
            set
            {
                motorizeLandArea = value;
                NotifyPropertyChanged("MotorizeLandArea");
            }
        }

        /// <summary>
        ///备用字段A 已用于
        ///1、宗地座落方位描述
        /// </summary>
        [DataColumn("BYZDA")]
        public string ExtendA
        {
            get { return extendA; }
            set
            {
                extendA = value.TrimSafe();
                NotifyPropertyChanged("ExtendA");
            }
        }

        /// <summary>
        ///备用字段B(宗地统一编码)
        /// </summary>
        [DataColumn("BYZDB")]
        public string ExtendB
        {
            get { return extendB; }
            set
            {
                extendB = value.TrimSafe();
                NotifyPropertyChanged("ExtendB");
            }
        }

        /// <summary>
        ///备用字段C
        /// </summary>
        [DataColumn("BYZDC")]
        public string ExtendC
        {
            get { return extendC; }
            set
            {
                extendC = value.TrimSafe();
                NotifyPropertyChanged("ExtendC");
            }
        }

        /// <summary>
        ///状态 登记10、审核20
        /// </summary>
        [DataColumn("ZT")]
        public string Status
        {
            get { return status; }
            set
            {
                status = value.TrimSafe();
                NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        ///附属地面积(流转面积)
        /// </summary>
        [DataColumn("FSDMJ")]
        public double PertainToArea
        {
            get { return pertainToArea; }
            set
            {
                pertainToArea = value;
                NotifyPropertyChanged("PertainToArea");
            }
        }

        /// <summary>
        ///承包起始日期
        /// </summary>
        [DataColumn("CBQSRQ")]
        public string ArableLandTime
        {
            get { return arableLandTime; }
            set
            {
                arableLandTime = value.TrimSafe();
                NotifyPropertyChanged("ArableLandTime");
            }
        }

        /// <summary>
        ///长*宽
        /// </summary>
        [DataColumn("ZK")]
        public string WidthHeight
        {
            get { return widthHeight; }
            set
            {
                widthHeight = value.TrimSafe();
                NotifyPropertyChanged("WidthHeight");
            }
        }

        /// <summary>
        /// 地籍区编号
        /// </summary>
        [DataColumn("DJQBM")]
        public string CadastralZoneCode
        {
            get { return cadastralZoneCode; }
            set
            {
                cadastralZoneCode = value.TrimSafe();
                NotifyPropertyChanged("CadastralZoneCode");
            }
        }

        /// <summary>
        /// 种植类型
        /// </summary>
        [DataColumn("ZZLX")]
        public string PlatType
        {
            get { return platType; }
            set
            {
                platType = value.TrimSafe();
                NotifyPropertyChanged("PlatType");
            }
        }

        /// <summary>
        /// 承包方式
        /// </summary>
        [DataColumn("CBFS")]
        public string ConstructMode
        {
            get { return constructMode; }
            set
            {
                constructMode = value.TrimSafe();
                NotifyPropertyChanged("ConstructMode");
            }
        }

        /// <summary>
        /// 预留A(融安 是否登记已占用)
        /// </summary>
        [DataColumn("YLA")]
        public string AliasNameA
        {
            get { return aliasNameA; }
            set
            {
                aliasNameA = value.TrimSafe();
                NotifyPropertyChanged("AliasNameA");
            }
        }

        /// <summary>
        /// 预留B(查找四至预留初步结果信息)
        /// </summary>
        [DataColumn("YLB")]
        public string AliasNameB
        {
            get { return aliasNameB; }
            set
            {
                aliasNameB = value.TrimSafe();
                NotifyPropertyChanged("AliasNameB");
            }
        }

        /// <summary>
        /// 预留C
        /// </summary>
        [DataColumn("YLC")]
        public string AliasNameC
        {
            get { return aliasNameC; }
            set
            {
                aliasNameC = value.TrimSafe();
                NotifyPropertyChanged("AliasNameC");
            }
        }

        /// <summary>
        /// 预留D //用于大数据量初始化地块周边地块集合
        /// </summary>
        [DataColumn("YLD")]
        public string AliasNameD
        {
            get { return aliasNameD; }
            set
            {
                aliasNameD = value.TrimSafe();
                NotifyPropertyChanged("AliasNameD");
            }
        }

        /// <summary>
        /// 预留E
        /// </summary>
        [DataColumn("YLE")]
        public string AliasNameE
        {
            get { return aliasNameE; }
            set
            {
                aliasNameE = value.TrimSafe();
                NotifyPropertyChanged("AliasNameE");
            }
        }

        /// <summary>
        /// 预留F
        /// </summary>
        [DataColumn("YLF")]
        public string AliasNameF
        {
            get { return aliasNameF; }
            set
            {
                aliasNameF = value.TrimSafe();
                NotifyPropertyChanged("AliasNameF");
            }
        }

        /// <summary>
        /// 扩展信息
        /// </summary>
        [DataColumn("DKKZXX")]
        public string ExpandInfo
        {
            get { return landExpand; }
            set
            {
                landExpand = value.TrimSafe();
                NotifyPropertyChanged("LandExpand");
            }
        }

        #region 确权确股

        private double quantificicationStockQuantity;

        /// <summary>
        /// 量化股数
        /// </summary>
        [DataColumn("LHGS")]
        public double QuantificicationStockQuantity
        {
            get { return quantificicationStockQuantity; }
            set
            {
                quantificicationStockQuantity = value;
                NotifyPropertyChanged("QuantificicationStockQuantity");
            }
        }

        private double obligateStockQuantity;

        /// <summary>
        /// 预留股数
        /// </summary>
        [DataColumn("YLGS")]
        public double ObligateStockQuantity
        {
            get { return obligateStockQuantity; }
            set
            {
                obligateStockQuantity = value;
                NotifyPropertyChanged("ObligateStockQuantity");
            }
        }

        private double quantificitionArea;

        /// <summary>
        /// 量化面积
        /// </summary>
        [DataColumn("LHMJ")]
        public double QuantificicationArea
        {
            get { return quantificitionArea; }
            set
            {
                quantificitionArea = value;
                NotifyPropertyChanged("QuantificitionArea");
            }
        }

        private double obligateArea;

        /// <summary>
        /// 预留面积
        /// </summary>
        [DataColumn("YLMJ")]
        public double ObligateArea
        {
            get { return obligateArea; }
            set
            {
                obligateArea = value;
                NotifyPropertyChanged("ObligateArea");
            }
        }

        private bool isStockLand;

        /// <summary>
        /// 是否确股地块
        /// </summary>
        [DataColumn("SFQGDK")]
        public bool IsStockLand
        {
            get { return isStockLand; }
            set { isStockLand = value; NotifyPropertyChanged("IsStockLand"); }
        }

        #endregion 确权确股

        //[DataColumn(Enabled = false)]
        //public int Img { get { return shape != null ? 1 : 2; } }

        /// <summary>
        /// 地块图片
        /// </summary>
        [DataColumn(Enabled = false)]
        public int Img
        {
            get
            {
                return img == 0 ? (shape != null ? 1 : 2) : img;
            }
            set
            {
                img = value;
                NotifyPropertyChanged("Img");
            }
        }

        private int img;

        /// <summary>
        /// 地块扩展
        /// </summary>
        [DataColumn(Enabled = false)]
        public AgricultureLandExpand LandExpand
        {
            get
            {
                //return string.IsNullOrEmpty(ExpandInfo) ? new AgricultureLandExpand() { ID = this.ID, Name = this.Name, HouseHolderName = this.Name } :
                //    ToolSerialize.DeserializeXmlString<AgricultureLandExpand>(this.landExpand);
                AgricultureLandExpand expand = ToolSerialize.DeserializeXmlString<AgricultureLandExpand>(this.landExpand);
                return expand == null ? new AgricultureLandExpand() { ID = this.ID, Name = this.Name, HouseHolderName = this.Name } : expand;
            }
            set
            {
                ExpandInfo = ToolSerialize.SerializeXmlString<AgricultureLandExpand>(value);
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

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static ContractLand()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eConstructType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_ePlantProtectType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandSlopelLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPurposeType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eManageType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTransferType);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ContractLand()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            ConstructMode = "110";
            landLevel = "900";
            OwnRightType = "30";
            Purpose = "1";
            LandCategory = "10";
            PlatType = "0";
            PlantType = "3";
            LandCode = "121";
            LandName = "空闲地";
            ManagementType = "9";
            IsTransfer = false;
            isFarmerLand = null;
            Modifier = "Admin";
            Founder = "Admin";
            ID = Guid.NewGuid();
            IsFlyLand = false;
            ActualArea = 0.0;
            AwareArea = 0.0;
            TableArea = 0.0;
            ContractDelayArea = 0.0;
            LineArea = 0.0;
            MotorizeLandArea = 0.0;
            PertainToArea = 0.0;
            LandName = "";
        }

        #endregion Ctor

        #region Methods

        public static string[] GetLandNeighbor(string landNeighbor)
        {
            if (landNeighbor == null || landNeighbor.Length < 4)
                return new string[] { "", "", "", "" };

            string[] neighbors = landNeighbor.Split(new char[] { '\r' });
            if (neighbors == null || neighbors.Length != 4)
            {
                neighbors = landNeighbor.Split(new char[] { '\n' });//xml中会变成\n
                if (neighbors == null || neighbors.Length != 4)
                    return new string[] { "", "", "", "" };
            }

            return neighbors;
        }

        //public string[] GetLandNeighbor()
        //{
        //    if (LandNeighbor == null || LandNeighbor.Length < 4)
        //        return new string[] { "", "", "", "" };

        //    string[] neighbors = LandNeighbor.Split(new char[] { '\r' });
        //    if (neighbors == null || neighbors.Length != 4)
        //    {
        //        neighbors = LandNeighbor.Split(new char[] { '\n' });//xml中会变成\n
        //        if (neighbors == null || neighbors.Length != 4)
        //            return new string[] { "", "", "", "" };
        //    }

        //    return neighbors;
        //}

        /// <summary>
        /// 获取耕保类型
        /// </summary>
        /// <param name="plantType"></param>
        /// <returns></returns>
        public static string GetLandPlantType(ePlantProtectType plantType)
        {
            if (plantType == ePlantProtectType.FirstGrade)
            {
                return "一类";
            }
            else if (plantType == ePlantProtectType.SecondGrade)
            {
                return "二类";
            }
            else { }
            return "";
        }

        /// <summary>
        /// 获取耕保类型
        /// </summary>
        /// <param name="plantType"></param>
        /// <returns></returns>
        public static ePlantProtectType GetPlantProtectType(string plantType)
        {
            if (plantType == "一" || plantType == "一类")
            {
                return ePlantProtectType.FirstGrade;
            }
            if (plantType == "二" || plantType == "二类")
            {
                return ePlantProtectType.SecondGrade;
            }
            return ePlantProtectType.UnKnown;
        }

        /// <summary>
        /// 获取是否基本农田
        /// </summary>
        /// <param name="isFarmerLand"></param>
        /// <returns></returns>
        public static bool? GetFarmerLand(string isFarmerLand)
        {
            if (isFarmerLand == "是")
            {
                return true;
            }
            else if (isFarmerLand == "否")
            {
                return false;
            }
            else { }
            return null;
        }

        /// <summary>
        /// 获取是否基本农田
        /// </summary>
        /// <param name="isFarmerLand"></param>
        /// <returns></returns>
        public static string GetFarmerLand(bool? isFarmerLand)
        {
            if (isFarmerLand.HasValue)
            {
                return isFarmerLand.Value ? "是" : "否";
            }
            return "";
        }

        /// <summary>
        /// 获取地块编号
        /// </summary>
        /// <param name="cardicalNumber"></param>
        /// <returns></returns>
        public static string GetLandNumber(string cardicalNumber)
        {
            if (string.IsNullOrEmpty(cardicalNumber))
            {
                return "";
            }
            string landNumber = cardicalNumber;
            if (cardicalNumber.Length > Zone.ZONE_FULL_LENGTH)
            {
                landNumber = cardicalNumber.Substring(Zone.ZONE_FULL_LENGTH);
                return landNumber.Trim();
            }
            if (cardicalNumber.Length > Zone.ZONE_FULLGROUP_LENGTH)
            {
                landNumber = cardicalNumber.Substring(Zone.ZONE_FULLGROUP_LENGTH);
                return landNumber.Trim();
            }
            else if (cardicalNumber.Length == Zone.ZONE_FULLGROUP_LENGTH)
            {
                return "";
            }
            return cardicalNumber.Trim();
        }

        /// <summary>
        /// 获取缩略地块编号
        /// </summary>
        /// <param name="cardicalNumber"></param>
        /// <returns></returns>
        public static string GetShortLandNumber(string landNumber)
        {
            if (string.IsNullOrEmpty(landNumber))
            {
                return "";
            }
            string number = GetLandNumber(landNumber);
            if (number.Length > 5)
            {
                return number.Substring(number.Length - 5);
            }
            return number;
        }

        /// <summary>
        /// 获取地块编号
        /// </summary>
        /// <param name="cardicalNumber"></param>
        /// <returns></returns>
        private static string GetTownLandNumber(string cardicalNumber)
        {
            if (string.IsNullOrEmpty(cardicalNumber))
            {
                return "";
            }
            if (cardicalNumber.Length > Zone.ZONE_TOWN_LENGTH)
            {
                string flag = cardicalNumber.Substring(Zone.ZONE_TOWN_LENGTH, 3);
                if (flag == "000")
                {
                    string landNumber = cardicalNumber.Substring(Zone.ZONE_TOWN_LENGTH + 3);
                    return landNumber.Trim();
                }
            }
            else if (cardicalNumber.Length == Zone.ZONE_TOWN_LENGTH)
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// 获取地块编号
        /// </summary>
        /// <param name="cardicalNumber"></param>
        /// <returns></returns>
        private static string GetVillageLandNumber(string cardicalNumber)
        {
            if (string.IsNullOrEmpty(cardicalNumber))
            {
                return "";
            }
            if (cardicalNumber.Length > Zone.ZONE_VILLAGE_LENGTH)
            {
                string flag = cardicalNumber.Substring(Zone.ZONE_VILLAGE_LENGTH, 3);
                if (flag == "888")
                {
                    string landNumber = cardicalNumber.Substring(Zone.ZONE_VILLAGE_LENGTH + 3);
                    return landNumber.Trim();
                }
            }
            else if (cardicalNumber.Length == Zone.ZONE_VILLAGE_LENGTH)
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="sourceLand"></param>
        /// <returns></returns>
        public string Compare(ContractLand land)
        {
            string information = "地块编码为" + ContractLand.GetLandNumber(this.CadastralNumber) + "的地块";
            if (ExceptSpaceString(this.Name) != ExceptSpaceString(land.Name))
            {
                information += "地块名称由" + ExceptSpaceString(this.Name) + "变更为" + ExceptSpaceString(land.Name) + ";";
            }
            if (this.ActualArea != land.ActualArea)
            {
                information += "实测面积由" + this.ActualArea + "变更为" + land.ActualArea + ";";
            }
            if (this.AwareArea != land.AwareArea)
            {
                information += "确权面积由" + this.AwareArea + "变更为" + land.AwareArea + ";";
            }
            if (this.CadastralNumber != land.CadastralNumber)
            {
                information += "地块编码由" + ContractLand.GetLandNumber(this.CadastralNumber) + "变更为" + ContractLand.GetLandNumber(land.CadastralNumber) + ";";
            }
            if (this.ConstructMode != land.ConstructMode)
            {
                information += "承包方式由" + EnumNameAttribute.GetDescription(this.ConstructMode) + "变更为" + EnumNameAttribute.GetDescription(land.ConstructMode) + ";";
            }
            if (this.LandCategory != land.LandCategory)
            {
                information += "地块类别由" + EnumNameAttribute.GetDescription(this.LandCategory) + "变更为" + EnumNameAttribute.GetDescription(land.LandCategory) + ";";
            }
            if (this.Purpose != land.Purpose)
            {
                information += "土地用途由" + EnumNameAttribute.GetDescription(this.Purpose) + "变更为" + EnumNameAttribute.GetDescription(land.Purpose) + ";";
            }
            if (this.OwnerName != land.OwnerName)
            {
                information += "权属由" + this.OwnerName + "变更为" + land.OwnerName + ";";
            }
            if (this.IsFarmerLand != land.IsFarmerLand)
            {
                information += "是否基本农田由" + ((this.IsFarmerLand != null && this.IsFarmerLand.HasValue) ? (this.IsFarmerLand.Value ? "是" : "否") : "") + "变更为" + ((land.IsFarmerLand != null && land.IsFarmerLand.HasValue) ? (land.IsFarmerLand.Value ? "是" : "否") : "") + ";";
            }
            if (ExceptSpaceString(this.LandName) != ExceptSpaceString(land.LandName))
            {
                information += "地类由" + this.LandName + "变更为" + land.LandName + ";";
            }
            AgricultureLandExpand sourceExpand = this.LandExpand;
            AgricultureLandExpand targetExpand = land.LandExpand;
            if (ExceptSpaceString(sourceExpand.ImageNumber) != ExceptSpaceString(targetExpand.ImageNumber))
            {
                information += "图幅编号由" + sourceExpand.ImageNumber + "变更为" + targetExpand.ImageNumber + ";";
            }
            if (sourceExpand.Elevation != targetExpand.Elevation && sourceExpand.Elevation != 0 && targetExpand.Elevation != 0)
            {
                information += "高程由" + sourceExpand.Elevation + "变更为" + targetExpand.Elevation + ";";
            }
            if (ExceptSpaceString(this.PlotNumber) != ExceptSpaceString(land.PlotNumber))
            {
                information += "畦数由" + this.PlotNumber + "变更为" + land.PlotNumber + ";";
            }
            if (ExceptSpaceString(this.NeighborEast) != ExceptSpaceString(land.NeighborEast))
            {
                information += "四至东由" + this.NeighborEast + "变更为" + land.NeighborEast + ";";
            }
            if (ExceptSpaceString(this.NeighborSouth) != ExceptSpaceString(land.NeighborSouth))
            {
                information += "四至南由" + this.NeighborEast + "变更为" + land.NeighborEast + ";";
            }
            if (ExceptSpaceString(this.NeighborWest) != ExceptSpaceString(land.NeighborWest))
            {
                information += "四至西由" + this.NeighborWest + "变更为" + land.NeighborWest + ";";
            }
            if (ExceptSpaceString(this.NeighborNorth) != ExceptSpaceString(land.NeighborNorth))
            {
                information += "四至北由" + this.NeighborNorth + "变更为" + land.NeighborNorth + ";";
            }
            if (this.TableArea != null && this.TableArea.HasValue && this.TableArea.Value != land.TableArea.Value)
            {
                information += "台账面积由" + this.TableArea.Value + "变更为" + land.TableArea.Value + ";";
            }
            if (this.LandLevel != land.LandLevel)
            {
                information += "地力等级由" + EnumNameAttribute.GetDescription(this.LandLevel) + "变更为" + EnumNameAttribute.GetDescription(land.LandLevel) + ";";
            }
            if (this.PlatType != land.PlatType)
            {
                information += "种植类型由" + EnumNameAttribute.GetDescription(this.PlatType) + "变更为" + EnumNameAttribute.GetDescription(land.PlatType) + ";";
            }
            if (this.PlantType != land.PlantType)
            {
                information += "耕保类型由" + EnumNameAttribute.GetDescription(this.PlantType) + "变更为" + EnumNameAttribute.GetDescription(land.PlantType) + ";";
            }
            if (this.ManagementType != land.ManagementType)
            {
                information += "经营方式由" + EnumNameAttribute.GetDescription(this.ManagementType) + "变更为" + EnumNameAttribute.GetDescription(land.ManagementType) + ";";
            }
            if (this.IsFlyLand != land.IsFlyLand)
            {
                information += "是否飞地由" + (this.IsFlyLand ? "是" : "否") + "变更为" + (land.IsFlyLand ? "是" : "否") + ";";
            }
            if (ExceptSpaceString(sourceExpand.UseSituation) != ExceptSpaceString(targetExpand.UseSituation))
            {
                information += "利用情况由" + ExceptSpaceString(sourceExpand.UseSituation) + "变更为" + ExceptSpaceString(targetExpand.UseSituation) + ";";
            }
            if (ExceptSpaceString(sourceExpand.Yield) != ExceptSpaceString(targetExpand.Yield))
            {
                information += "产量由" + ExceptSpaceString(sourceExpand.Yield) + "变更为" + ExceptSpaceString(targetExpand.Yield) + ";";
            }
            if (ExceptSpaceString(sourceExpand.OutputValue) != ExceptSpaceString(targetExpand.OutputValue))
            {
                information += "产值由" + ExceptSpaceString(sourceExpand.OutputValue) + "变更为" + ExceptSpaceString(targetExpand.OutputValue) + ";";
            }
            if (ExceptSpaceString(sourceExpand.IncomeSituation) != ExceptSpaceString(targetExpand.IncomeSituation))
            {
                information += "收益情况由" + ExceptSpaceString(sourceExpand.IncomeSituation) + "变更为" + ExceptSpaceString(targetExpand.IncomeSituation) + ";";
            }
            if (this.IsTransfer != land.IsTransfer)
            {
                information += "是否流转由" + (this.IsTransfer ? "是" : "否") + "变更为" + (land.IsTransfer ? "是" : "否") + ";";
            }
            if (this.TransferType != land.TransferType)
            {
                information += "流转方式由" + EnumNameAttribute.GetDescription(this.TransferType) + "变更为" + EnumNameAttribute.GetDescription(land.TransferType) + ";";
            }
            if (this.PertainToArea != land.PertainToArea)
            {
                information += "流转面积由" + this.PertainToArea.ToString() + "变更为" + land.PertainToArea.ToString() + ";";
            }
            if (ExceptSpaceString(this.TransferTime) != ExceptSpaceString(land.TransferTime))
            {
                information += "流转面积由" + this.TransferTime + "变更为" + land.TransferTime + ";";
            }
            if (ExceptSpaceString(this.Comment) != ExceptSpaceString(land.Comment))
            {
                information += "备注由" + ExceptSpaceString(this.Comment) + "变更为" + ExceptSpaceString(land.Comment) + ";";
            }
            if (ExceptSpaceString(sourceExpand.ReferPerson) != ExceptSpaceString(targetExpand.ReferPerson))
            {
                information += "调查员由" + ExceptSpaceString(sourceExpand.ReferPerson) + "变更为" + ExceptSpaceString(targetExpand.ReferPerson) + ";";
            }
            if (ExceptSpaceString(sourceExpand.SurveyPerson) != ExceptSpaceString(targetExpand.SurveyPerson))
            {
                information += "调查员由" + ExceptSpaceString(sourceExpand.SurveyPerson) + "变更为" + ExceptSpaceString(targetExpand.SurveyPerson) + ";";
            }
            if (sourceExpand.SurveyDate != targetExpand.SurveyDate)
            {
                information += "调查日期由" + ((sourceExpand.SurveyDate == null || !sourceExpand.SurveyDate.HasValue) ? "" : sourceExpand.SurveyDate.Value.ToString("yyyy年MM月dd日"))
                            + "变更为" + ((targetExpand.SurveyDate == null || !targetExpand.SurveyDate.HasValue) ? "" : targetExpand.SurveyDate.Value.ToString("yyyy年MM月dd日")) + ";";
            }
            if (ExceptSpaceString(sourceExpand.SurveyChronicle) != ExceptSpaceString(targetExpand.SurveyChronicle))
            {
                information += "调查记事由" + ExceptSpaceString(sourceExpand.SurveyChronicle) + "变更为" + ExceptSpaceString(targetExpand.SurveyChronicle) + ";";
            }
            if (ExceptSpaceString(sourceExpand.CheckPerson) != ExceptSpaceString(targetExpand.CheckPerson))
            {
                information += "审核人由" + ExceptSpaceString(sourceExpand.CheckPerson) + "变更为" + ExceptSpaceString(targetExpand.CheckPerson) + ";";
            }
            if (sourceExpand.CheckDate != targetExpand.CheckDate)
            {
                information += "审核日期由" + ((sourceExpand.CheckDate == null || !sourceExpand.CheckDate.HasValue) ? "" : sourceExpand.CheckDate.Value.ToString("yyyy年MM月dd日"))
                            + "变更为" + ((targetExpand.CheckDate == null || !targetExpand.CheckDate.HasValue) ? "" : targetExpand.CheckDate.Value.ToString("yyyy年MM月dd日")) + ";";
            }
            if (ExceptSpaceString(sourceExpand.CheckOpinion) != ExceptSpaceString(targetExpand.CheckOpinion))
            {
                information += "审核意见由" + ExceptSpaceString(sourceExpand.CheckOpinion) + "变更为" + ExceptSpaceString(targetExpand.CheckOpinion) + ";";
            }
            if (information == "地块编码为" + ContractLand.GetLandNumber(this.CadastralNumber) + "的地块")
            {
                information = "";
            }
            return information;
        }

        /// <summary>
        /// 去除空字符串
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        private string ExceptSpaceString(string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            return sourceString.Trim().Replace(" ", "").Replace("\0", "");
        }

        #endregion Methods
    }

    ///// <summary>
    ///// 农村土地承包地块集合
    ///// </summary>
    //[Serializable]
    //public class ContractLandCollection : List<ContractLand>
    //{
    //}
}