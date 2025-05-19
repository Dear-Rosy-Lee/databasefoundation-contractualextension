/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.ObjectModel;
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方绑定实体
    /// </summary>
    public class VirtualPersonItem : NotifyCDObject
    {
        #region Fields

        private Visibility visibility;
        private string name;
        private string iCN;
        private eImage img;
        private string houseHolderName;
        private eVirtualPersonStatus status;
        private string contractorNumber;
        private double? totalTableArea;
        private string oldVirtualCode;

        #endregion

        #region Property

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string ICN
        {
            get { return iCN; }
            set { iCN = value; NotifyPropertyChanged("ICN"); }
        }

        /// <summary>
        /// 原承包方编码
        /// </summary>
        public string OldVirtualCode
        {
            get { return oldVirtualCode; }
            set { oldVirtualCode = value; NotifyPropertyChanged("OldVirtualCode"); }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string HouseHolderName
        {
            get { return houseHolderName; }
            set { houseHolderName = value; NotifyPropertyChanged("HouseHolderName"); }
        }

        /// <summary>
        /// 共有人
        /// </summary>
        public ObservableCollection<BindPerson> Children
        {
            get;
            private set;
        }

        /// <summary>
        /// 现承包人数
        /// </summary>
        public string ContractorNumber
        {
            get { return contractorNumber; }
            set { contractorNumber = value; NotifyPropertyChanged("ContractorNumber"); }
        }

        /// <summary>
        /// 总台账面积
        /// </summary>
        public double? TotalTableArea
        {
            get { return totalTableArea; }
            set { totalTableArea = value; NotifyPropertyChanged("TotalTableArea"); }
        }

        /// <summary>
        /// 股份数
        /// </summary>
        public double EquityValue { get; set; }

        /// <summary>
        /// 入股面积
        /// </summary>
        public double EquityArea { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Tag { get; set; }

        /// <summary>
        /// 地域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 户编号
        /// </summary>
        public string FamilyNumber { get; set; }

        /// <summary>
        /// 承包方状态0正常,1锁定
        /// </summary>
        public eVirtualPersonStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public eImage Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public VirtualPersonItem()
        {
            Children = new ObservableCollection<BindPerson>();
            Img = eImage.Family;
        }

        #endregion

        #region Methods

        #endregion
    }

    /// <summary>
    /// 人员绑定实体
    /// </summary>
    public class BindPerson : NotifyCDObject
    {
        #region Filds

        private Visibility visibility;
        private string name;
        private string iCN;
        private eImage img;
        private string relationShip;
        private string age;
        private string comment;
        private eGender gender;
        private Guid id;
        private Guid fid;
        private string oldVirtualCode = "1";

        #endregion

        #region  Propertys

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                if (Tag != null)
                    Tag.ID = id;
            }
        }

        /// <summary>
        /// 承包方ID
        /// </summary>
        public Guid FamilyID
        {
            get
            {
                return fid;
            }
            set
            {
                fid = value;
                if (Tag != null)
                    Tag.FamilyID = fid;
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
                if (Tag != null)
                    Tag.Name = name;
            }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string ICN
        {
            get { return iCN; }
            set
            {
                iCN = value;
                NotifyPropertyChanged("ICN");
                if (Tag != null)
                    Tag.ICN = iCN;
            }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string OldVirtualCode
        {
            get { return oldVirtualCode; }
            set
            {
                oldVirtualCode = value;
                NotifyPropertyChanged("OldVirtualCode");

            }
        }
        /// <summary>
        /// 关系
        /// </summary>
        public string Relationship
        {
            get { return relationShip; }
            set
            {
                relationShip = value;
                NotifyPropertyChanged("RelationShip");
                if (Tag != null)
                    Tag.Relationship = relationShip;
            }
        }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age
        {
            get { return age; }
            set
            {
                age = value;
                NotifyPropertyChanged("Age");
                if (Tag != null)
                    Tag.Age = age;
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                NotifyPropertyChanged("Comment");
                if (Tag != null)
                    Tag.Comment = comment;
            }
        }

        /// <summary>
        /// 共有人
        /// </summary>
        public Person Tag { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public eImage Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public eGender Gender
        {
            get { return gender; }
            set { gender = value; NotifyPropertyChanged("Gender"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public BindPerson(Person p)
        {
            ICN = p.ICN;
            ID = p.ID;
            FamilyID = (Guid)p.FamilyID;
            Name = p.Name;
            Relationship = p.Relationship;
            Comment = p.Opinion;
            Gender = p.Gender;
            Tag = p;
            Img = VirtualPersonItemHelper.ChangeByGender(p.Gender);
            if (p.CardType == eCredentialsType.IdentifyCard)
            {
                int currentAge = p.GetAge();
                Age = currentAge > 0 ? currentAge.ToString() : "";
            }
            else
            {
                Age = p.Age;
                if (string.IsNullOrEmpty(Age))
                {
                    int age = ToolDateTime.GetAge(p.Birthday);
                    if (age != -1)
                        Age = age.ToString();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 类型
    /// </summary>
    public enum eImage
    {
        /// <summary>
        /// 家庭
        /// </summary>
        Family = 0,

        /// <summary>
        /// 男
        /// </summary>
        Man = 1,

        /// <summary>
        /// 女
        /// </summary>
        Woman = 2,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 3,

        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 4,
    }
}
