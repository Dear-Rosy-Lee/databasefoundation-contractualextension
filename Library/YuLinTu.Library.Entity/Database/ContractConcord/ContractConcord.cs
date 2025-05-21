// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包合同
    /// </summary>
    [Serializable]
    [DataTable("CBJYQ_HT")]
    public class ContractConcord : NotifyCDObject
    {
        #region Ctor

        private Guid id;
        private string concordNumber;
        private Guid senderId;
        private string senderName;
        private string zoneCode;
        private string arableLandType;
        private DateTime? arableLandStartTime;
        private DateTime? arableLandEndTime;
        private string badlandType;
        private DateTime? badlandStartTime;
        private DateTime? badlandEndTime;
        private string landPurpose;
        private string badlandPurpose;
        private double? totalTableArea;
        private string managementTime;
        private DateTime? senderDate;
        private DateTime? contractDate;
        private DateTime? checkAgencyDate;
        private string contractPath;
        private eStatus status;
        private string contractCredentialNumber;
        private Guid? contracterId;
        private string contracterName;
        private string secondContracterName;
        private string secondContracterLocated;
        private double personAvgArea;
        private string contracterIdentifyNumber;
        private string contracterRepresentName;
        private string contracterRepresentType;
        private string contracterRepresentNumber;
        private string contracterRepresentTelphone;
        private string agentName;
        private string agentCrdentialType;
        private string agentCrdentialNumber;
        private string agentTelphone;
        private string contracerType;
        private bool flag;
        private double? privateArea;
        private double? contractMoney;
        private Guid? requireBookId;
        private string founder;
        private DateTime? creationTime;
        private string modifier;
        private DateTime? modifiedTime;
        private string comment;
        private double countActualArea;
        private double countAwareArea;
        private double countMotorizeLandArea;
        private bool isValid;
        private string extendA;
        private string extendB;
        private string extendC;
        private string publicityChronicle;
        private string publicityChroniclePerson;
        private DateTime? publicityDate;
        private string publicityResult;
        private string publicityContractor;
        private DateTime? publicityResultDate;
        private string publicityCheckOpinion;
        private string publicityCheckPerson;
        private DateTime? publicityCheckDate;

        #endregion

        #region Properties

        /// <summary>
        ///承包合同唯一标识
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }

        /// <summary>
        ///农村土地承包合同编号
        /// </summary>
        [DataColumn("CBHTBM", Nullable = false)]
        public string ConcordNumber
        {
            get { return concordNumber; }
            set
            {
                concordNumber = value;
                NotifyPropertyChanged("ConcordNumber");
                if (string.IsNullOrEmpty(concordNumber))
                    return;
                concordNumber = concordNumber.Trim();

            }
        }

        /// <summary>
        ///发包方Id
        /// </summary>
        [DataColumn("FBFBS", Nullable = false)]
        public Guid SenderId
        {
            get { return senderId; }
            set { senderId = value; NotifyPropertyChanged("SenderId"); }
        }

        /// <summary>
        ///发包方名称
        /// </summary>
        [DataColumn("FBFMC", Nullable = false)]
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value.TrimSafe(); NotifyPropertyChanged("SenderName"); }
        }

        /// <summary>
        ///地域代码
        /// </summary>
        [DataColumn("DYBM", Nullable = false)]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value; 
                NotifyPropertyChanged("ZoneCode");
                if (string.IsNullOrEmpty(zoneCode))
                    return;
                zoneCode = zoneCode.Trim();
            }
        }

        /// <summary>
        ///耕地承包方式
        /// </summary>
        [DataColumn("CBFS", Nullable = false)]
        public string ArableLandType
        {
            get { return arableLandType; }
            set { arableLandType = value.TrimSafe(); NotifyPropertyChanged("ArableLandType"); }
        }

        /// <summary>
        ///耕地承包起始时间
        /// </summary>
        [DataColumn("CBQXQ")]
        public DateTime? ArableLandStartTime
        {
            get { return arableLandStartTime; }
            set { arableLandStartTime = value; NotifyPropertyChanged("ArableLandStartTime"); }
        }

        /// <summary>
        ///耕地承包结束时间
        /// </summary>
        [DataColumn("CBQXZ")]
        public DateTime? ArableLandEndTime
        {
            get { return arableLandEndTime; }
            set { arableLandEndTime = value; NotifyPropertyChanged("ArableLandEndTime"); }
        }

        /// <summary>
        ///四荒地承包方式
        /// </summary>
        [DataColumn("SHDCBFS")]
        public string BadlandType
        {
            get { return badlandType; }
            set { badlandType = value.TrimSafe(); NotifyPropertyChanged("BadlandType"); }
        }

        /// <summary>
        ///四荒地承包起始时间
        /// </summary>
        [DataColumn("SHDCBQXQ")]
        public DateTime? BadlandStartTime
        {
            get { return badlandStartTime; }
            set { badlandStartTime = value; NotifyPropertyChanged("BadlandStartTime"); }
        }

        /// <summary>
        ///四荒地承包结束时间
        /// </summary>
        [DataColumn("SHDCBQXZ")]
        public DateTime? BadlandEndTime
        {
            get { return badlandEndTime; }
            set { badlandEndTime = value; NotifyPropertyChanged("BadlandEndTime"); }
        }

        /// <summary>
        ///耕地承包用途
        /// </summary>
        [DataColumn("CBYT", Nullable = false)]
        public string LandPurpose
        {
            get { return landPurpose; }
            set { landPurpose = value.TrimSafe(); NotifyPropertyChanged("LandPurpose"); }
        }

        /// <summary>
        ///四荒地承包用途
        /// </summary>
        [DataColumn("SHDCBYT")]
        public string BadlandPurpose
        {
            get { return badlandPurpose; }
            set { badlandPurpose = value.TrimSafe(); NotifyPropertyChanged("BadlandPurpose"); }
        }

        /// <summary>
        ///二轮承包台账面积
        /// </summary>
        [DataColumn("ELCBTZMJ")]
        public double? TotalTableArea
        {
            get { return totalTableArea; }
            set { totalTableArea = value; NotifyPropertyChanged("TotalTableArea"); }
        }

        /// <summary>
        ///承包期限
        /// </summary>
        [DataColumn("JYQX")]
        public string ManagementTime
        {
            get { return managementTime; }
            set { managementTime = value.TrimSafe(); NotifyPropertyChanged("ManagementTime"); }
        }

        /// <summary>
        ///发包方签订日期
        /// </summary>
        [DataColumn("FBFQDSJ")]
        public DateTime? SenderDate
        {
            get { return senderDate; }
            set { senderDate = value; NotifyPropertyChanged("SenderDate"); }
        }

        /// <summary>
        ///承包方签订日期
        /// </summary>
        [DataColumn("CBFQDSJ")]
        public DateTime? ContractDate
        {
            get { return contractDate; }
            set { contractDate = value; NotifyPropertyChanged("ContractDate"); }
        }

        /// <summary>
        ///鉴证机构日期
        /// </summary>
        [DataColumn("JZJGRQ")]
        public DateTime? CheckAgencyDate
        {
            get { return checkAgencyDate; }
            set { checkAgencyDate = value; NotifyPropertyChanged("CheckAgencyDate"); }
        }

        /// <summary>
        ///农村土地承包合同附件路径
        /// </summary>
        [DataColumn("CBHTFJ")]
        public string ContractPath
        {
            get { return contractPath; }
            set { contractPath = value.TrimSafe(); NotifyPropertyChanged("ContractPath"); }
        }

        /// <summary>
        ///状态
        /// </summary>
        [DataColumn("ZT")]
        public eStatus Status
        {
            get { return status; }
            set { status = value; NotifyPropertyChanged("Status"); }
        }

        /// <summary>
        ///农村土地承包经营权证号
        /// </summary>
        [DataColumn("CBJYQZH")]
        public string ContractCredentialNumber
        {
            get { return contractCredentialNumber; }
            set
            {
                contractCredentialNumber = value; NotifyPropertyChanged("Status");
                if (string.IsNullOrEmpty(contractCredentialNumber))
                    return;
                contractCredentialNumber = contractCredentialNumber.Trim();
            }
        }

        /// <summary>
        ///承包方Id
        /// </summary>
        [DataColumn("CBFBS")]
        public Guid? ContracterId
        {
            get { return contracterId; }
            set { contracterId = value; NotifyPropertyChanged("ContracterId"); }
        }

        /// <summary>
        ///承包方姓名
        /// </summary>
        [DataColumn("CBFMC", Nullable = false)]
        public string ContracterName
        {
            get { return contracterName; }
            set { contracterName = value.TrimSafe(); NotifyPropertyChanged("ContracterName"); }
        }

        /// <summary>
        /// 第二承包方名称
        /// </summary>
        [DataColumn("DECBFMC")]
        public string SecondContracterName
        {
            get { return secondContracterName; }
            set { secondContracterName = value.TrimSafe(); NotifyPropertyChanged("SecondContracterName"); }
        }

        /// <summary>
        /// 第二承包方住址
        /// </summary>
        [DataColumn("DECBFZZ")]
        public string SecondContracterLocated
        {
            get { return secondContracterLocated; }
            set { secondContracterLocated = value.TrimSafe(); NotifyPropertyChanged("SecondContracterLocated"); }
        }

        /// <summary>
        /// 人均面积
        /// </summary>
        [DataColumn("RJMJ")]
        public double PersonAvgArea
        {
            get { return personAvgArea; }
            set { personAvgArea = value; NotifyPropertyChanged("PersonAvgArea"); }
        }

        /// <summary>
        ///承包方证件号
        /// </summary>
        [DataColumn("CBFZJHM")]
        public string ContracterIdentifyNumber
        {
            get { return contracterIdentifyNumber; }
            set { contracterIdentifyNumber = value.TrimSafe(); NotifyPropertyChanged("ContracterIdentifyNumber"); }
        }

        /// <summary>
        ///承包方法人代表姓名
        /// </summary>
        [DataColumn("CBFFRDBXM")]
        public string ContracterRepresentName
        {
            get { return contracterRepresentName; }
            set { contracterRepresentName = value.TrimSafe(); NotifyPropertyChanged("ContracterRepresentName"); }
        }

        /// <summary>
        ///承包方法人代表证件类型
        /// </summary>
        [DataColumn("CBFFRDBZJLX")]
        public string ContracterRepresentType
        {
            get { return contracterRepresentType; }
            set { contracterRepresentType = value.TrimSafe(); NotifyPropertyChanged("ContracterRepresentType"); }
        }

        /// <summary>
        ///承包方法人代表证件号
        /// </summary>
        [DataColumn("CBFFRDBZJHM")]
        public string ContracterRepresentNumber
        {
            get { return contracterRepresentNumber; }
            set { contracterRepresentNumber = value.TrimSafe(); NotifyPropertyChanged("ContracterRepresentNumber"); }
        }

        /// <summary>
        ///承包方法人代表电话号码
        /// </summary>
        [DataColumn("CBFFRDBDHHM")]
        public string ContracterRepresentTelphone
        {
            get { return contracterRepresentTelphone; }
            set { contracterRepresentTelphone = value.TrimSafe(); NotifyPropertyChanged("ContracterRepresentTelphone"); }
        }

        /// <summary>
        ///代理人姓名
        /// </summary>
        [DataColumn("DLRXM")]
        public string AgentName
        {
            get { return agentName; }
            set { agentName = value.TrimSafe(); NotifyPropertyChanged("AgentName"); }
        }

        /// <summary>
        ///代理人证件类型
        /// </summary>
        [DataColumn("DLRZJLX")]
        public string AgentCrdentialType
        {
            get { return agentCrdentialType; }
            set { agentCrdentialType = value.TrimSafe(); NotifyPropertyChanged("AgentCrdentialType"); }
        }

        /// <summary>
        ///代理人证件号码
        /// </summary>
        [DataColumn("DLRZJHM")]
        public string AgentCrdentialNumber
        {
            get { return agentCrdentialNumber; }
            set { agentCrdentialNumber = value.TrimSafe(); NotifyPropertyChanged("AgentCrdentialNumber"); }
        }

        /// <summary>
        ///代理人电话号码
        /// </summary>
        [DataColumn("DLRDHHM")]
        public string AgentTelphone
        {
            get { return agentTelphone; }
            set { agentTelphone = value.TrimSafe(); NotifyPropertyChanged("AgentTelphone"); }
        }

        /// <summary>
        ///承包方类型
        /// </summary>
        [DataColumn("CBFLX")]
        public string ContracerType
        {
            get { return contracerType; }
            set { contracerType = value.TrimSafe(); NotifyPropertyChanged("ContracerType"); }
        }

        /// <summary>
        ///长久标志
        /// </summary>
        [DataColumn("CJBZ")]
        public bool Flag
        {
            get { return flag; }
            set { flag = value; NotifyPropertyChanged("Flag"); }
        }

        /// <summary>
        ///自留地面积
        /// </summary>
        [DataColumn("ZLDMJ")]
        public double? PrivateArea
        {
            get { return privateArea; }
            set { privateArea = value; NotifyPropertyChanged("PrivateArea"); }
        }

        /// <summary>
        /// 承包金额
        /// </summary>
        [DataColumn("CBJE")]
        public double? ContractMoney
        {
            get { return contractMoney; }
            set { contractMoney = value; NotifyPropertyChanged("ContractMoney"); }
        }

        /// <summary>
        ///农村土地承包经营权申请书ID
        /// </summary>
        [DataColumn("QZSQSBZ")]
        public Guid? RequireBookId
        {
            get { return requireBookId; }
            set { requireBookId = value; NotifyPropertyChanged("RequireBookId"); }
        }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("CJZ")]
        public string Founder
        {
            get { return founder; }
            set { founder = value.TrimSafe(); NotifyPropertyChanged("Founder"); }
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataColumn("CJSJ")]
        public DateTime? CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; NotifyPropertyChanged("CreationTime"); }
        }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("ZHXGZ")]
        public string Modifier
        {
            get { return modifier; }
            set { modifier = value.TrimSafe(); NotifyPropertyChanged("Modifier"); }
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataColumn("ZHXGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set { modifiedTime = value; NotifyPropertyChanged("ModifiedTime"); }
        }

        /// <summary>
        ///备注
        /// </summary>
        [DataColumn("BZXX")]
        public string Comment
        {
            get { return comment; }
            set { comment = value.TrimSafe(); NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        ///实测面积
        /// </summary>
        [DataColumn("SCZMJ")]
        public double CountActualArea
        {
            get { return countActualArea; }
            set { countActualArea = value; NotifyPropertyChanged("CountActualArea"); }
        }

        /// <summary>
        ///确权面积
        /// </summary>
        [DataColumn("QQZMJ")]
        public double CountAwareArea
        {
            get { return countAwareArea; }
            set { countAwareArea = value; NotifyPropertyChanged("CountAwareArea"); }
        }

        /// <summary>
        ///机动地总面积
        /// </summary>
        [DataColumn("JDDZMJ")]
        public double CountMotorizeLandArea
        {
            get { return countMotorizeLandArea; }
            set { countMotorizeLandArea = value; NotifyPropertyChanged("CountMotorizeLandArea"); }
        }

        /// <summary>
        ///当前合同是否可用
        /// </summary>
        [DataColumn("HTSFKY")]
        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; NotifyPropertyChanged("IsValid"); }
        }

        /// <summary>
        /// 预留A
        /// </summary>
        [DataColumn("YLA")]
        public string ExtendA
        {
            get { return extendA; }
            set { extendA = value.TrimSafe(); NotifyPropertyChanged("ExtendA"); }
        }

        /// <summary>
        ///预留B
        /// </summary>
        [DataColumn("YLB")]
        public string ExtendB
        {
            get { return extendB; }
            set { extendB = value.TrimSafe(); NotifyPropertyChanged("ExtendB"); }
        }

        /// <summary>
        ///预留C
        /// </summary>
        [DataColumn("YLC")]
        public string ExtendC
        {
            get { return extendC; }
            set { extendC = value.TrimSafe(); NotifyPropertyChanged("ExtendC"); }
        }

        /// <summary>
        /// 公示记事
        /// </summary>
        [DataColumn("GSJS")]
        public string PublicityChronicle
        {
            get { return publicityChronicle; }
            set { publicityChronicle = value.TrimSafe(); NotifyPropertyChanged("PublicityChronicle"); }
        }

        /// <summary>
        /// 公示记事人
        /// </summary>
        [DataColumn("GSJSR")]
        public string PublicityChroniclePerson
        {
            get { return publicityChroniclePerson; }
            set { publicityChroniclePerson = value.TrimSafe(); NotifyPropertyChanged("PublicityChroniclePerson"); }
        }

        /// <summary>
        /// 公示日期
        /// </summary>
        [DataColumn("GSRQ")]
        public DateTime? PublicityDate
        {
            get { return publicityDate; }
            set { publicityDate = value; NotifyPropertyChanged("PublicityDate"); }
        }

        /// <summary>
        /// 公示结果意见
        /// </summary>
        [DataColumn("GSJGYJ")]
        public string PublicityResult
        {
            get { return publicityResult; }
            set { publicityResult = value.TrimSafe(); NotifyPropertyChanged("PublicityResult"); }
        }

        /// <summary>
        /// 承包方代表
        /// </summary>
        [DataColumn("CBFDB")]
        public string PublicityContractor
        {
            get { return publicityContractor; }
            set { publicityContractor = value.TrimSafe(); NotifyPropertyChanged("PublicityContractor"); }
        }

        /// <summary>
        /// 公示结果日期
        /// </summary>
        [DataColumn("GSJGRQ")]
        public DateTime? PublicityResultDate
        {
            get { return publicityResultDate; }
            set { publicityResultDate = value; NotifyPropertyChanged("PublicityResultDate"); }
        }

        /// <summary>
        /// 公示审核意见
        /// </summary>
        [DataColumn("GSSHYJ")]
        public string PublicityCheckOpinion
        {
            get { return publicityCheckOpinion; }
            set { publicityCheckOpinion = value.TrimSafe(); NotifyPropertyChanged("PublicityCheckOpinion"); }
        }

        /// <summary>
        /// 公示审核人
        /// </summary>
        [DataColumn("GSSHR")]
        public string PublicityCheckPerson
        {
            get { return publicityCheckPerson; }
            set { publicityCheckPerson = value.TrimSafe(); NotifyPropertyChanged("PublicityCheckPerson"); }
        }

        /// <summary>
        /// 公示审核日期
        /// </summary>
        [DataColumn("GSSHRQ")]
        public DateTime? PublicityCheckDate
        {
            get { return publicityCheckDate; }
            set { publicityCheckDate = value; NotifyPropertyChanged("PublicityCheckDate"); }
        }

        #endregion

        #region Ctor

        public ContractConcord()
        {
            id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            SenderDate = DateTime.Now;
            ContractDate = DateTime.Now;
            CheckAgencyDate = DateTime.Now;
            ArableLandStartTime = DateTime.Now;
            ArableLandEndTime = DateTime.Now;
            BadlandStartTime = DateTime.Now;
            BadlandEndTime = DateTime.Now;
            Status = eStatus.UnKnown;
            IsValid = true;
        }

        #endregion
    }
}