// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;
using YuLinTu.Library.Entity.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包方
    /// </summary>
    [DataTable("Person")]
    [Serializable]
    public class Person : NotifyInfoCDObject//CommonPerson, INotifyPropertyChanged
    {
        #region Fields
       
        private string _icn;
        private string birthdayStr;
        private DateTime? birthday;
        #endregion

        #region Properties
        [DataColumn("Error", Enabled = false)]
        public override string Error
        {
            get
            {
                return base.Error;
            }
        }

        [DataColumn("this", Enabled = false)]
        public override string this[string columnName]
        {
            get
            {
                return base[columnName];
            }
        }

        [DataColumn("ID", ColumnType = eDataType.Guid)]
        public Guid ID
        {
            get;
            set;
        }
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Gallery = "基本信息", Catalog = "基本信息",
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/HeaderInsertGallery.png")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "姓名不能为空")]
        [StringLength(10, ErrorMessage = "姓名超出长度")]
        [Display(Name = "名称")]
        [DataColumn("Name", ColumnType = eDataType.String)]
        public string Name
        {
            get;
            set;
        }

        [DataColumn("ICN", ColumnType = eDataType.String)]
        public string ICN
        {
            get { return _icn; }
            set { _icn = value; if (_icn != null) _icn = _icn.Trim(); }
        }

        [DataColumn("Birthday", ColumnType = eDataType.DateTime)]
        [XmlIgnore]
        public DateTime? Birthday
        {
            get
            {
                if (!birthday.HasValue)
                {
                    DateTime date;
                    birthday = DateTime.TryParse(birthdayStr, out date) ? date : null as DateTime?;
                }
                return birthday;
            }
            set
            {
                birthdayStr = null;
                birthday = value;
            }
        }

        [XmlElement(ElementName = "Birthday")]
        [DataColumn(Enabled = false)]
        public string BirthdayString
        {
            get
            {
                if (birthdayStr == null)
                {
                    birthdayStr = Birthday.HasValue ? Birthday.Value.ToString("yyyy-MM-dd") : null;
                }
                return birthdayStr;
            }
            set
            {
                birthday = null;
                birthdayStr = value;
            }
        }

        [DataColumn("Gender", ColumnType = eDataType.Int32)]
        public eGender Gender { get; set; }

        [DataColumn("FamilyID", ColumnType = eDataType.Guid)]
        public Guid? FamilyID { get; set; }

        [DataColumn("FamilyNumber", ColumnType = eDataType.String)]
        public string FamilyNumber { get; set; }

        [DataColumn("Nation", ColumnType = eDataType.Int32)]
        public eNation Nation { get; set; }


        private string _relationship;
        [DataColumn("Relationship", ColumnType = eDataType.String)]
        public string Relationship
        {
            get { return _relationship; }
            set
            {
                _relationship = value;
                NotifyPropertyChanged("Relationship");
            }
        }

        [DataColumn("ZoneID", ColumnType = eDataType.String)]
        public string ZoneCode { get; set; }

        [DataColumn("IsFarmer", ColumnType = eDataType.Boolean)]
        public bool? IsFarmer { get; set; }



        [DataColumn("QGS", ColumnType = eDataType.Int32)]
        public double StockQuantity { get; set; }


        [DataColumn("QGMJ", ColumnType = eDataType.Int32)]
        public double StockArea { get; set; }


        /// <summary>
        /// 延包土地份数
        /// </summary>
        public string ExtensionPackageNumber { get; set; }

        /// <summary>
        /// 已死亡人员
        /// </summary>
        public string IsDeaded { get; set; }

        /// <summary>
        /// 出嫁后未退承包地人员
        /// </summary>
        public string LocalMarriedRetreatLand { get; set; }

        /// <summary>
        /// 农转非后未退承包地人员
        /// </summary>
        public string PeasantsRetreatLand { get; set; }

        /// <summary>
        /// 婚进但在非出地未退承包地人员
        /// </summary>
        public string ForeignMarriedRetreatLand { get; set; }

        /// <summary>
        /// 承包地共有人
        /// </summary>
        public string SharePerson { get; set; }

        /// <summary>
        /// 是否享有承包地
        /// </summary>
        public string IsSharedLand { get; set; }

        /*2013-11-19日增加*/

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public string AccountNature { get; set; }

        /// <summary>
        /// 从何处迁入
        /// </summary>
        public string SourceMove { get; set; }

        /// <summary>
        /// 迁入时间
        /// </summary>
        public string MoveTime { get; set; }

        /// <summary>
        /// 是否为99年共有人
        /// </summary>
        public string IsNinetyNineSharePerson { get; set; }

        /// <summary>
        /// 户籍备注
        /// </summary>
        public string CencueComment { get; set; }

        /// <summary>
        /// 扩展A
        /// </summary>
        public string ExtendA { get; set; }

        /// <summary>
        /// 扩展B
        /// </summary>
        public string ExtendB { get; set; }

        /// <summary>
        /// 扩展C
        /// </summary>
        public string ExtendC { get; set; }

        /// <summary>
        /// 扩展D
        /// </summary>
        public string ExtendD { get; set; }

        /// <summary>
        /// 扩展E
        /// </summary>
        public string ExtendE { get; set; }

        /// <summary>
        /// 扩展F
        /// </summary>
        public string ExtendF { get; set; }

        [DataColumn("Comment", ColumnType = eDataType.String)]
        public string Comment { get; set; }

        [DataColumn("Founder", ColumnType = eDataType.String)]
        public string CreateUser { get; set; }

        [DataColumn("Modifier", ColumnType = eDataType.String)]
        public string LastModifyUser { get; set; }

        [DataColumn("CreationTime", ColumnType = eDataType.DateTime)]
        public DateTime? CreateTime { get; set; }

        [DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 共有人状态
        /// </summary>
        public eEntityStatus EntityStatus { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public eCredentialsType CardType { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostNumber { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否自动识别
        /// </summary>
        public bool AutoIdentify { get; set; }

        #endregion

        #region Ctor

        static Person()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eGender);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eNation);
        }

        public Person()
        {
            ID = Guid.NewGuid();
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
            Gender = eGender.Unknow;
            Nation = eNation.UnKnown;
            CardType = eCredentialsType.IdentifyCard;
            EntityStatus = eEntityStatus.Normal;
            IsFarmer = true;
            IsSharedLand = "是";
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        public int GetAge()
        {
            if (CardType != eCredentialsType.IdentifyCard)
            {
                return -1;
            }
            if (!string.IsNullOrEmpty(ICN))
            {
                Birthday = GetBirthday(ICN);
            }
            if (!Birthday.HasValue)
            {
                return -1;
            }
            DateTime birthday = Birthday.Value;
            DateTime now = DateTime.Now;

            int year = birthday.Year;
            int nowYear = now.Year;

            int month = birthday.Month;
            int nowMonth = now.Month;

            int day = birthday.Day;
            int nowDay = now.Day;

            int age = nowYear - year;
            if (age < 1)
            {
                return -1;
            }
            return nowMonth > month || (nowMonth == month && nowDay >= day) ? age : --age;
            //if (nowYear >= year)
            //{


            //    if (month > nowMonth)
            //        return (year - 1) < 200 ? (year - 1) : -1;
            //    if (month < nowMonth)
            //        return year < 200 ? year : -1;
            //    if (month == nowMonth)
            //    {
            //        if (day < nowDay)
            //            return year < 200 ? year : -1;
            //        if (day > nowDay)
            //            return (year - 1) < 200 ? (year - 1) : -1;
            //        if (day == nowDay)
            //            return year < 200 ? year : -1;
            //    }
            //}
            //return -1;
        }

        /// <summary>
        /// 覆写
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(Name, ICN, ZoneCode);
        }

        /// <summary>
        /// 提取身份证号码出生日期
        /// </summary>
        private DateTime GetBirthday(string icn)
        {
            DateTime val = DateTime.MinValue;
            int year = 0, month = 0, day = 0;
            bool flag = false;
            if (icn.Length == 15)
            {
                flag = int.TryParse("19" + icn.Substring(6, 2), out year)
                    && int.TryParse(icn.Substring(8, 2), out month)
                    && int.TryParse(icn.Substring(10, 2), out day);
            }
            else if (icn.Length == 18)
            {
                flag = int.TryParse(icn.Substring(6, 4), out year)
                    && int.TryParse(icn.Substring(10, 2), out month)
                    && int.TryParse(icn.Substring(12, 2), out day);
            }
            //2月30号依然异常
            //flag = flag && year > 0 && year < 10000 && month > 0 && month < 13 && day > 0 && day < 32;
            if (flag)
                try
                {
                    val = new DateTime(year, month, day);
                }
                catch { }
            return val;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        public bool Equal(Person person)
        {
            bool isEqual = this.Name == person.Name
                        && this.CardType == person.CardType
                        && this.ICN == person.ICN
                        && this.Gender == person.Gender
                        && this.Nation == person.Nation
                        && this.IsSharedLand == person.IsSharedLand
                        && this.Relationship == person.Relationship;
            return isEqual;
        }

        #endregion
    }
}