using System;
using System.Collections.Generic;
using System.Net;
using YuLinTu.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 摸底调查表
    /// </summary>
    [Serializable]
    [DataTable("TZ_MDDCB")]
    public class SurveyForm : NotifyCDObject
    {
        #region Ctor
        /// <summary>
        /// 构造方法
        /// </summary>
        public SurveyForm()
        {
            id = Guid.NewGuid();
        }
        #endregion Ctor
        #region Field
        private Guid id;
        private Guid? ownerId;
        private string ownerName;
        private string ownerNumber;
        private List<Person> sharePersonList;
        private string sharePerson;
        private string ownerAddress;
        private string zoneCode;
        private eVirtualPersonType virtualType;
        private eCredentialsType cardType;
        private string telephone;
        private string lawyerPosterNumber;
        private string familyNumber;
        private string personCount;
        private string otherInfomation;
        private eBHQK changeSituation;
        private List<ContractLand> contractLandList;
        private string contractLand;
        private string oldZoneName;
        private double exchangeLandArea;
        #endregion Field

        /// <summary>
        /// ID
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }

        /// <summary>
        /// 承包方标识
        /// </summary>
        [DataColumn("CBFID")]
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
        /// 承包方名称
        /// </summary>
        [DataColumn("CBFMC")]
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
        ///承包方(代表)证件号码
        /// </summary>
        [DataColumn("CBFZJHM")]
        public string OwnerNumber
        {
            get { return ownerNumber; }
            set
            {
                ownerNumber = value;
                if (string.IsNullOrEmpty(ownerNumber))
                    return;
                ownerNumber = ownerNumber.Trim();
                NotifyPropertyChanged("OwnerNumber");
            }
        }

        /// <summary>
        /// 共有人集合
        /// </summary>
        [DataColumn(Enabled = false)]
        public List<Person> SharePersonList
        {
            get
            {
                if (sharePersonList == null)
                {
                    sharePersonList = ToolSerialize.DeserializeXmlString<List<Person>>(this.sharePerson);
                }
                //List<Person> list = ToolSerialize.DeserializeXmlString<List<Person>>(this.sharePerson);
                if (sharePersonList == null)
                {
                    sharePersonList = new List<Person>();
                }
                //由于历史原因，外部代码中调用了Clear方法
                return new List<Person>(sharePersonList);
            }
            set
            {
                sharePersonList = value;
                sharePerson = ToolSerialize.SerializeXmlString<List<Person>>(value);
                NotifyPropertyChanged("SharePerson");
            }
        }

        /// <summary>
        /// 自身其他信息及共有人信息
        /// </summary>
        [DataColumn("GYRXX")]
        public string SharePerson
        {
            get { return sharePerson; }
            set
            {
                sharePerson = value;
                sharePersonList = null;
                NotifyPropertyChanged("SharePerson");
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        [DataColumn("CBFDZ")]
        public string OwnerAddress
        {
            get { return ownerAddress; }
            set { ownerAddress = value.TrimSafe(); NotifyPropertyChanged("OwnerAddress"); }
        }

        /// <summary>
        ///地域编码
        /// </summary>
        [DataColumn("DYBM", Nullable = false)]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                if (string.IsNullOrEmpty(zoneCode))
                    return;
                zoneCode = zoneCode.Trim();
                NotifyPropertyChanged("ZoneCode");
            }
        }

        /// <summary>
        /// 承包方类型
        /// </summary>
        [DataColumn("CBFLX", Nullable = false)]
        public eVirtualPersonType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; NotifyPropertyChanged("VirtualType"); }
        }

        /// <summary>
        /// 证件类型
        /// </summary>
        [DataColumn("CBFZJLX")]
        public eCredentialsType CardType
        {
            get { return cardType; }
            set { cardType = value; NotifyPropertyChanged("CardType"); }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        [DataColumn("LXDH")]
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value.TrimSafe(); NotifyPropertyChanged("Telephone"); }
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataColumn("YZBM")]
        public string LawyerPosterNumber
        {
            get { return lawyerPosterNumber; }
            set
            {
                lawyerPosterNumber = value.TrimSafe(); NotifyPropertyChanged("LawyerPosterNumber");
            }
        }

        /// <summary>
        /// 户号
        /// </summary>
        [DataColumn("HH")]
        public string FamilyNumber
        {
            get { return familyNumber; }
            set { familyNumber = value.TrimSafe(); NotifyPropertyChanged("familyNumber"); }
        }

        /// <summary>
        /// 人口数
        /// </summary>
        [DataColumn("RKS")]
        public string PersonCount
        {
            get { return personCount; }
            set
            {
                personCount = value.TrimSafe();
                if (personCount == "0" || string.IsNullOrWhiteSpace(personCount))
                {
                    personCount = SharePersonList == null ? "1" : SharePersonList.Count.ToString();
                }
                NotifyPropertyChanged("PersonCount");
            }
        }

        /// <summary>
        /// 其它信息
        /// </summary>
        [DataColumn("QTXX")]
        public string OtherInfomation
        {
            get { return otherInfomation; }
            set { otherInfomation = value.TrimSafe(); NotifyPropertyChanged("OtherInfomation"); }
        }

        /// <summary>
        ///变化情况
        /// </summary>
        [DataColumn("BHQK")]
        public eBHQK ChangeSituation
        {
            get { return changeSituation; }
            set { changeSituation = value; NotifyPropertyChanged("ChangeSituation"); }
        }

        /// <summary>
        /// 地块集合
        /// </summary>
        [DataColumn(Enabled = false)]
        public List<ContractLand> ContractLandList
        {
            get
            {
                if (contractLandList == null)
                {
                    contractLandList = ToolSerialize.DeserializeXmlString<List<ContractLand>>(this.contractLand);
                }
                if (contractLandList == null)
                {
                    contractLandList = new List<ContractLand>();
                }
                //由于历史原因，外部代码中调用了Clear方法
                return new List<ContractLand>(contractLandList);
            }
            set
            {
                contractLandList = value;
                contractLand = ToolSerialize.SerializeXmlString<List<ContractLand>>(value);
                NotifyPropertyChanged("ContractLand");
            }
        }

        /// <summary>
        /// 地块信息
        /// </summary>
        [DataColumn("DKXX")]
        public string ContractLand
        {
            get { return contractLand; }
            set
            {
                contractLand = value;
                NotifyPropertyChanged("ContractLand");
            }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        [DataColumn("YDYMC")]
        public string OldZoneName
        {
            get { return oldZoneName; }
            set { oldZoneName = value.TrimSafe(); NotifyPropertyChanged("OldZoneName"); }
        }

        /// <summary>
        /// 互换面积变化
        /// </summary>
        public double ExchangeLandArea
        {
            get { return exchangeLandArea; }
            set { exchangeLandArea = value; NotifyPropertyChanged("ExchangeLandArea"); }
        }
    }
}
