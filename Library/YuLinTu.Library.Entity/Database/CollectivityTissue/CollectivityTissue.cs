// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 集体经济组织
    /// </summary>
    [Serializable]
    [DataTable("JCSJ_FBF")]
    public class CollectivityTissue : NotifyCDObject  //(修改前)YltEntityIDName
    {
        #region Ctor

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static CollectivityTissue()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTissueType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eStatus);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public CollectivityTissue()
        {
            id = Guid.NewGuid();
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            Type = eTissueType.UnKnown;
            Status = eStatus.UnKnown;
            LawyerCredentType = eCredentialsType.IdentifyCard;
        }

        #endregion

        #region Field

        private string zoneCode = string.Empty;
        private Guid id;
        private string socialCode;
        private string name;
        private string lawyerName;
        private string lawyerTelephone;
        private string lawyerAddress;
        private string lawyerPosterNumber;
        private eCredentialsType lawyerCredentType;
        private string lawyerCartNumber;
        private eTissueType type;
        private eStatus status;
        private string code;
        private string founder;
        private DateTime? creationTime;
        private string modifier;
        private DateTime? modifiedTime;
        private string surveyPerson;
        private DateTime? surveyDate;
        private string surveyChronicle;
        private string checkPerson;
        private DateTime? checkDate;
        private string checkOpinion;
        private string comment;
        private eLandPropertyType ownRightType = eLandPropertyType.Collectived;    // 所有权性质，湖南工单特有
        private eGender gender = eGender.Unknow;   // 性别，湖南工单特有

        #endregion

        #region Properties

        /// <summary>
        ///集体经济组织编码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = true)]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value; NotifyPropertyChanged("ID");
            }
        }
        /// <summary>
        ///社会信用代码
        /// </summary>
        [DataColumn("SHXYDM", Nullable = false, PrimaryKey = true)]
        public string SocialCode
        {
            get { return socialCode; }
            set
            {
                socialCode = value.TrimSafe(); NotifyPropertyChanged("SocialCode");
            }
        }

        /// <summary>
        ///发包方名称
        /// </summary>
        [DataColumn("FBFMC", Nullable = false)]
        public string Name
        {
            get { return name; }
            set
            {
                name = value.TrimSafe(); NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        ///发包方负责人姓名
        /// </summary>
        [DataColumn("FBFFZRXM")]
        public string LawyerName
        {
            get { return lawyerName; }
            set
            {
                lawyerName = value.TrimSafe(); NotifyPropertyChanged("LawyerName");
            }
        }

        /// <summary>
        ///联系电话
        /// </summary>
        [DataColumn("LXDH")]
        public string LawyerTelephone
        {
            get { return lawyerTelephone; }
            set
            {
                lawyerTelephone = value.TrimSafe();
                NotifyPropertyChanged("LawyerTelephone");
            }
        }

        /// <summary>
        ///发包方地址
        /// </summary>
        [DataColumn("FBFDZ")]
        public string LawyerAddress
        {
            get { return lawyerAddress; }
            set
            {
                lawyerAddress = value.TrimSafe(); NotifyPropertyChanged("LawyerAddress");
            }
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
        /// 负责人证件类型
        /// </summary>
        [DataColumn("FZRZJLX")]
        public eCredentialsType LawyerCredentType
        {
            get { return lawyerCredentType; }
            set
            {
                lawyerCredentType = value; NotifyPropertyChanged("LawyerCredentType");
            }
        }

        /// <summary>
        ///负责人证件号码
        /// </summary>
        [DataColumn("FZRZJHM")]
        public string LawyerCartNumber
        {
            get { return lawyerCartNumber; }
            set
            {
                lawyerCartNumber = value.TrimSafe(); NotifyPropertyChanged("LawyerCartNumber");
            }
        }

        /// <summary>
        ///集体经济组织类型
        /// </summary>
        [DataColumn("LX", Nullable = false)]
        public eTissueType Type
        {
            get { return type; }
            set
            {
                type = value; NotifyPropertyChanged("Type");
            }
        }

        /// <summary>
        ///行政地域编码
        /// </summary>
        [DataColumn("XZDYBM", Nullable = false)]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                if (!string.IsNullOrEmpty(zoneCode))
                    zoneCode = zoneCode.Trim();
                NotifyPropertyChanged("ZoneCode");
            }
        }

        /// <summary>
        ///状态
        /// </summary>
        [DataColumn("ZT")]
        public eStatus Status
        {
            get { return status; }
            set
            {
                status = value; NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        ///发包方编码
        /// </summary>
        [DataColumn("FBFBM")]
        public string Code
        {
            get { return code; }
            set
            {
                code = value.TrimSafe(); NotifyPropertyChanged("Code");
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
                founder = value.TrimSafe(); NotifyPropertyChanged("Founder");
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
                creationTime = value; NotifyPropertyChanged("CreationTime");
            }
        }

        /// <summary>
        ///修改者
        /// </summary>
        [DataColumn("XGZ")]
        public string Modifier
        {
            get { return modifier; }
            set
            {
                modifier = value.TrimSafe(); NotifyPropertyChanged("Modifier");
            }
        }

        /// <summary>
        ///修改时间
        /// </summary>
        [DataColumn("XGSJ")]
        public DateTime? ModifiedTime
        {
            get { return modifiedTime; }
            set
            {
                modifiedTime = value; NotifyPropertyChanged("ModifiedTime");
            }
        }

        /// <summary>
        /// 发包方调查员
        /// </summary>
        [DataColumn("FBFDCY")]
        public string SurveyPerson
        {
            get { return surveyPerson; }
            set
            {
                surveyPerson = value.TrimSafe(); NotifyPropertyChanged("SurveyPerson");
            }
        }

        /// <summary>
        /// 发包方调查日期
        /// </summary>
        [DataColumn("FBFDCRQ")]
        public DateTime? SurveyDate
        {
            get { return surveyDate; }
            set
            {
                surveyDate = value; NotifyPropertyChanged("SurveyDate");
            }
        }

        /// <summary>
        /// 发包方调查记事
        /// </summary>
        [DataColumn("FBFDCJS")]
        public string SurveyChronicle
        {
            get { return surveyChronicle; }
            set
            {
                surveyChronicle = value.TrimSafe(); NotifyPropertyChanged("SurveyChronicle");
            }
        }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataColumn("SHR")]
        public string CheckPerson
        {
            get { return checkPerson; }
            set
            {
                checkPerson = value.TrimSafe(); NotifyPropertyChanged("ChenkPerson");
            }
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        [DataColumn("SHRQ")]
        public DateTime? CheckDate
        {
            get { return checkDate; }
            set
            {
                checkDate = value; NotifyPropertyChanged("ChenkDate");
            }
        }

        /// <summary>
        /// 审核意见
        /// </summary>
        [DataColumn("SHYJ")]
        public string CheckOpinion
        {
            get { return checkOpinion; }
            set
            {
                checkOpinion = value.TrimSafe(); NotifyPropertyChanged("ChenkOpinion");
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataColumn("BZXX")]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value.TrimSafe(); NotifyPropertyChanged("Comment");
            }
        }

        /// <summary>
        ///权属性质
        /// </summary>
        [DataColumn("QSXZ")]
        public eLandPropertyType OwnRightType
        {
            get { return ownRightType; }
            set
            {
                if (value != 0)
                {
                    ownRightType = value;
                    NotifyPropertyChanged("OwnRightType");
                }
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        [DataColumn("Gender")]
        public eGender Gender
        {
            get { return gender; }
            set
            {
                if (value != 0)
                {
                    gender = value;
                    NotifyPropertyChanged("Gender");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 格式化地域编码
        /// </summary>
        /// <param name="codeZone"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string FormatCode(string codeZone, int number)
        {
            if (string.IsNullOrEmpty(codeZone))
                return string.Empty;

            if (codeZone.Length > YuLinTu.Library.Entity.Zone.ZONE_GROUP_LENGTH)
                return string.Empty;

            if (number < 0)
                return string.Empty;

            string code = codeZone;

            int cnt = YuLinTu.Library.Entity.Zone.ZONE_GROUP_LENGTH - codeZone.Length;
            for (int i = 0; i < cnt; i++)
                code += "0";

            code = string.Format("{0}{1:D5}", code, number);

            return code;
        }

        #endregion
    }
}