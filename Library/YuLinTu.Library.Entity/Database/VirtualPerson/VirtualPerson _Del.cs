/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 用于操作其他5个承包方表
    /// </summary>
    [DataTable("TZ_SCCBF")]
    [Serializable]
    public class VirtualPerson_Del : NotifyInfoCDObject  //(修改前)YltEntityIDName
    {
        #region Fivps

        private Guid id;
        private string name;
        private string number;
        private string sharePerson;
        private string address;
        private string zoneCode;
        private eVirtualPersonType virtualType;
        private eCredentialsType cardType; 
        private string telephone;
        private string postalNumber;
        private string familyNumber;
        private string personCount;
        private string otherInfomation; 
        private string comment;
        private List<Person> sharePersonList;
        private string oldVirtualCode;
        private VirtualPersonExpand expand;

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

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }

        /// <summary>
        ///名称
        /// </summary>
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Gallery = "基本信息", Catalog = "基本信息",
          UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/HeaderInsertGallery.png")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "名称不能为空")]
        [StringLength(10)]
        [Display(Name = "名称")]
        [DataColumn("CBFMC", Nullable = false)]
        public string Name
        {
            get { return name; }
            set { name = value.TrimSafe(); NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        ///承包方(代表)证件号码
        /// </summary>
        [DataColumn("CBFZJHM")]
        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                if (string.IsNullOrEmpty(number))
                    return;
                number = number.Trim();
                NotifyPropertyChanged("Number");
            }
        }

        /// <summary>
        ///自身其他信息及共有人信息
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
        ///地址
        /// </summary>
        [DataColumn("CBFDZ")]
        public string Address
        {
            get { return address; }
            set { address = value.TrimSafe(); NotifyPropertyChanged("Address"); }
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
        ///承包方类型
        /// </summary>
        [DataColumn("CBFLX", Nullable = false)]
        public eVirtualPersonType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; NotifyPropertyChanged("VirtualType"); }
        }

        /// <summary>
        ///证件类型
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
        public string PostalNumber
        {
            get { return postalNumber; }
            set { postalNumber = value.TrimSafe(); NotifyPropertyChanged("PostalNumber"); }
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
        ///备注
        /// </summary>
        [DataColumn("BZXX")]
        public string Comment
        {
            get { return comment; }
            set { comment = value.TrimSafe(); NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        ///原承包方编码
        /// </summary>
        [DataColumn("YCBFBM")]
        public string OldVirtualCode
        {
            get { return oldVirtualCode; }
            set { oldVirtualCode = value; NotifyPropertyChanged("OvpVirtualCode"); }
        }
        

        #endregion
        

        /// <summary>
        /// 扩展实体
        /// </summary>
        [DataColumn(Enabled = false)]
        public VirtualPersonExpand FamilyExpand
        {
            get
            {
                expand = ToolSerialize.DeserializeXmlString<VirtualPersonExpand>(this.otherInfomation);

                return expand == null ? new VirtualPersonExpand() : expand;
            }
            set
            {
                var xml = ToolSerialize.SerializeXmlString<VirtualPersonExpand>(value);
                OtherInfomation = xml;
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
        #region Ctor

        static VirtualPerson_Del()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eVirtualPersonType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eVirtualPersonStatus);
        }

        public VirtualPerson_Del()
        {
            id = Guid.NewGuid(); 
            cardType = eCredentialsType.IdentifyCard;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 添加共有人
        /// </summary>
        public void AddSharePerson(Person p)
        {
            List<Person> list = this.SharePersonList;
            if (list == null)
                list = new List<Person>();
            list.Add(p);
            SharePersonList = list;
        }

        /// <summary>
        /// 设置人口信息
        /// </summary>
        public void SetFamilyPersonInfo()
        {
            List<Person> list = SharePersonList;
            foreach (Person person in list)
            {
                person.ZoneCode = zoneCode;
                person.FamilyID = id;
                person.FamilyNumber = familyNumber;
                if (person.Name != name)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(person.ICN) && !string.IsNullOrEmpty(number))
                {
                    person.ICN = number;
                }
                else if (string.IsNullOrEmpty(number) && !string.IsNullOrEmpty(person.ICN))
                {
                    number = person.ICN;
                }
            }
            SharePersonList = list;
        }

        public override string ToString()
        {
            return string.Concat(Name, Number, ZoneCode);
        }

        /// <summary>
        /// 转换对象
        /// </summary> 
        public static VirtualPerson_Del ChangeDataEntity(string zoneCode, VirtualPerson vp, string fnum = "")
        {
            var data = new VirtualPerson_Del()
            {
                ID = vp.ID,
                Name = vp.Name,
                Number = vp.Number,
                SharePerson = vp.SharePerson,
                Address = vp.Address,
                ZoneCode = vp.ZoneCode,
                VirtualType = vp.VirtualType,
                CardType = vp.CardType,
                Telephone = vp.Telephone,
                PostalNumber = vp.PostalNumber,
                FamilyNumber = vp.FamilyNumber,
                PersonCount = vp.PersonCount,
                OtherInfomation = vp.OtherInfomation,
                Comment = vp.Comment,
                SharePersonList = vp.SharePersonList,
                OldVirtualCode = vp.OldVirtualCode,
                FamilyExpand = vp.FamilyExpand
            };
            return data;
        }

        #endregion
    }
}
