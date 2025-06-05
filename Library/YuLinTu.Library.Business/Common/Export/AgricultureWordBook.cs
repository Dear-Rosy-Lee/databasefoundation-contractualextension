/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// Word文件处理基类
    /// </summary>
    public class AgricultureWordBook : WordBase
    {
        #region Property

        /// <summary>
        /// by 江宇20161115 安徽插件使用
        /// </summary>
        public IDbContext DbContext { get; set; }

        private List<ContractLand> _stockLands = new List<ContractLand>();

        public List<ContractLand> StockLands
        {
            get { return _stockLands; }
            set
            {
                _stockLands = value;
            }
        }

        /// <summary>
        /// by 江宇 20161130确权确股使用，不要删除
        /// 界址点线
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListLandDots { get; set; }

        /// <summary>
        /// 是否确股——true为确股，false为确权，空为确权确股
        /// </summary>
        public bool? IsStockLand { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 地域集合(地域的父级地域集合,到县级)
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 地块类别
        /// </summary>
        public string ConstructType { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }

        /// <summary>
        /// 地类集合
        /// </summary>
        //public List<LandType> LandTypeCollection { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public ContractConcord Concord { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Tissue { get; set; }

        /// <summary>
        /// 权证
        /// </summary>
        public ContractRegeditBook Book { get; set; }

        /// <summary>
        /// 制图人
        /// </summary>
        public string ParcelDrawPerson { get; set; }

        /// <summary>
        /// 制图日期
        /// </summary>
        public DateTime? ParcelDrawDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string ParcelCheckPerson { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? ParcelCheckDate { get; set; }

        /// <summary>
        /// 制表人
        /// </summary>
        public string DrawTablePerson { get; set; }

        /// <summary>
        /// 制表日期
        /// </summary>
        public DateTime? DrawTableDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckTablePerson { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? CheckTableDate { get; set; }

        /// <summary>
        /// 书签数量
        /// </summary>
        public int BookMarkCount { get; set; }

        /// <summary>
        /// 日期值
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// 审核日期值
        /// </summary>
        public DateTime? DateChecked { get; set; }

        /// <summary>
        /// 是否填写空面积
        /// </summary>
        public bool WriteNullArea { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                var config = section.Settings as SystemSetDefine;
                base.EmptyReplacement = config == null ? "" : (config.EmptyReplacement == null ? "" : config.EmptyReplacement);
                return config;
            }
        }

        private string seriseNumber = "";
        #endregion Property

        #region Methods

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public AgricultureWordBook()
        {
            BookMarkCount = AgricultureSetting.SystemBookMarkNumber;
            ConstructType = "10";
            DateValue = DateTime.Now;
            DateChecked = DateTime.Now;
        }

        #endregion Ctor

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            InitialEntity(data);
            WriteZoneInformation();
            WriteContractorInformaion();
            WriteLandInformation();
            WriteSenderInformation();
            WriteConcordInformation();
            WriteBookInformation();
            WriteDateTimeInformation();
            //WriteParcelInformation();
            WriteOtherInformation();
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected virtual void InitialEntity(object data)
        {
            if (data == null)
                return;
            if (data is VirtualPerson)
                Contractor = data as VirtualPerson;
            if (data is Zone)
                CurrentZone = data as Zone;
            if (data is CollectivityTissue)
                Tissue = data as CollectivityTissue;
            if (data is ContractConcord)
                Concord = data as ContractConcord;
            if (data is ContractRegeditBook)
                Book = data as ContractRegeditBook;
            if (data is List<ContractLand>)
                LandCollection = data as List<ContractLand>;
        }

        /// <summary>
        /// 注销
        /// </summary>
        protected virtual void Destroyed()
        {
            CurrentZone = null;
            Contractor = null;
            LandCollection = null;
            Concord = null;
            Tissue = null;
            Book = null;
            DateValue = null;
            //GC.Collect();
        }

        #endregion Override

        #region Contractor

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        protected virtual void WriteContractorInformaion(bool writeperson = true)
        {
            if (Contractor == null)
                return;
            string townName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            int familyNumber = 0;
            if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
            {
                Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
            }
            string familyString = familyNumber > 0 ? string.Format("{0:D4}", familyNumber) : "";
            string familyAllString = Contractor.ZoneCode;//== null ? "" : Contractor.SenderCode;
            familyAllString = familyAllString.PadRight(14, '0') + familyString;
            if (AgricultureSetting.AgricultureLandWordFamilyNumber)
            {
                familyString = familyAllString;
            }
            VirtualPersonExpand expand = Contractor.FamilyExpand;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorName + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(Contractor.Name));//承包方姓名
                SetBookmarkValue(AgricultureBookMark.ContractorNumber + (i == 0 ? "" : i.ToString()), familyString);//承包方户号
                SetBookmarkValue(AgricultureBookMark.ContractorAllNumber + (i == 0 ? "" : i.ToString()), familyAllString);//承包方全户号

                if (Contractor.CardType == eCredentialsType.IdentifyCard)
                {
                    SetBookmarkValue(AgricultureBookMark.ContractorIdentifyNumber + (i == 0 ? "" : i.ToString()), Contractor.Number.GetSettingEmptyReplacement());//承包方身份证号码
                }
                else
                {
                    SetBookmarkValue(AgricultureBookMark.ContractorOtherCardNumber + (i == 0 ? "" : i.ToString()), Contractor.Number.GetSettingEmptyReplacement());  //其他证件号码
                }

                SetBookmarkValue(AgricultureBookMark.ContractorAllocationPerson + (i == 0 ? "" : i.ToString()), expand.AllocationPerson);//承包方实际分配人数
                SetBookmarkValue(AgricultureBookMark.ContractorComment + (i == 0 ? "" : i.ToString()), Contractor.Comment);//承包方备注
                SetBookmarkValue(AgricultureBookMark.ContractorLocation + (i == 0 ? "" : i.ToString()), Contractor.Address.GetSettingEmptyReplacement());//坐落地域
                SetBookmarkValue(AgricultureBookMark.ContractorCommunicate + (i == 0 ? "" : i.ToString()), Contractor.Address.GetSettingEmptyReplacement());//通信地址
                SetBookmarkValue(AgricultureBookMark.ContractorTelephone + (i == 0 ? "" : i.ToString()), Contractor.Telephone.GetSettingEmptyReplacement());//承包方电话号码
                SetBookmarkValue(AgricultureBookMark.ContractorPostNumber + (i == 0 ? "" : i.ToString()), Contractor.PostalNumber.GetSettingEmptyReplacement());//承包方邮政编码
                SetBookmarkValue(AgricultureBookMark.ContractorAddress + (i == 0 ? "" : i.ToString()), Contractor.Address.GetSettingEmptyReplacement());//承包方地址
                SetBookmarkValue(AgricultureBookMark.ContractorAddressTown + (i == 0 ? "" : i.ToString()), townName);//承包方地址到镇
                SetBookmarkValue(AgricultureBookMark.ContractorAddressVillage + (i == 0 ? "" : i.ToString()), villageName);//承包方地址到村
                SetBookmarkValue(AgricultureBookMark.ContractorAddressGroup + (i == 0 ? "" : i.ToString()), groupName);//承包方地址到组
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyPerson + (i == 0 ? "" : i.ToString()), expand.SurveyPerson);//承包方调查员
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyDate + (i == 0 ? "" : i.ToString()), (expand.SurveyDate != null && expand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(expand.SurveyDate.Value) : "");//承包方调查员
                SetBookmarkValue(AgricultureBookMark.ContractorSurveyChronicle + (i == 0 ? "" : i.ToString()), expand.SurveyChronicle.GetSettingEmptyReplacement());//承包方调查记事
                SetBookmarkValue(AgricultureBookMark.ContractorCheckPerson + (i == 0 ? "" : i.ToString()), expand.CheckPerson.GetSettingEmptyReplacement());//承包方审核员
                SetBookmarkValue(AgricultureBookMark.ContractorCheckDate + (i == 0 ? "" : i.ToString()),
                    ToolDateTime.GetLongDateString(expand.CheckDate.GetValueOrDefault()).GetSettingEmptyReplacement());//承包方审核日期
                SetBookmarkValue(AgricultureBookMark.ContractorCheckOpinion + (i == 0 ? "" : i.ToString()), expand.CheckOpinion.GetSettingEmptyReplacement());//承包方审核意见
                SetBookmarkValue(AgricultureBookMark.ContractorPublicityPerson + (i == 0 ? "" : i.ToString()), expand.PublicityChroniclePerson.GetSettingEmptyReplacement());//承包方公示记事人
                SetBookmarkValue(AgricultureBookMark.ContractorPublicityDate + (i == 0 ? "" : i.ToString()),
                    ToolDateTime.GetLongDateString(expand.PublicityDate.GetValueOrDefault()).GetSettingEmptyReplacement());//承包方公式日期
                SetBookmarkValue(AgricultureBookMark.ContractorPublicityChronicle + (i == 0 ? "" : i.ToString()), expand.PublicityChronicle.GetSettingEmptyReplacement());//承包方公示记事
                SetBookmarkValue(AgricultureBookMark.ContractorConcordNumber + (i == 0 ? "" : i.ToString()), expand.ConcordNumber);//承包方上的合同编码

                DateTime? startTime = expand.ConcordStartTime;
                DateTime? endTime = expand.ConcordEndTime;

                if (startTime != null)
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordStartYear + (i == 0 ? "" : i.ToString()), startTime.Value.Year.ToString());//开始时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordStartMonth + (i == 0 ? "" : i.ToString()), startTime.Value.Month.ToString());//开始时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordStartDay + (i == 0 ? "" : i.ToString()), startTime.Value.Day.ToString());//开始时间-日
                }
                else
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordStartYear + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//开始时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordStartMonth + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//开始时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordStartDay + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//开始时间-日
                }
                if (endTime != null)
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordEndYear + (i == 0 ? "" : i.ToString()), endTime.Value.Year.ToString());//结束时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordEndMonth + (i == 0 ? "" : i.ToString()), endTime.Value.Month.ToString());//结束时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordEndDay + (i == 0 ? "" : i.ToString()), endTime.Value.Day.ToString());//结束时间-日
                }
                else
                {
                    SetBookmarkValue(AgricultureBookMark.ConcordEndYear + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//结束时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordEndMonth + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//结束时间-日
                    SetBookmarkValue(AgricultureBookMark.ConcordEndDay + (i == 0 ? "" : i.ToString()), "".GetSettingEmptyReplacement());//结束时间-日
                }
            }
            WriteCredentialsInformation();
            if (writeperson)
                WriteSharePersonInformation();
        }

        /// <summary>
        /// 设置证件类型
        /// </summary>
        protected virtual void WriteCredentialsInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            eCredentialsType type = Contractor.CardType;
            string CardTypeText = string.Empty;
            switch (type)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard, "R");//证件号码
                    CardTypeText = "居民身份证";//证件号码
                    break;

                case eCredentialsType.AgentCard:
                    SetBookmarkValue(AgricultureBookMark.AgentCard, "R");//证件号码
                    CardTypeText = "行政、企事业单位机构代码证或法人代码证";//证件号码
                    break;

                case eCredentialsType.OfficerCard:
                    SetBookmarkValue(AgricultureBookMark.OfficerCard, "R");//证件号码
                    CardTypeText = "军官证";//证件号码
                    break;

                case eCredentialsType.Other:
                    SetBookmarkValue(AgricultureBookMark.CredentialOther, "R");//证件号码
                    CardTypeText = "其他";//证件号码
                    break;

                case eCredentialsType.Passport:
                    SetBookmarkValue(AgricultureBookMark.Passport, "R");//证件号码
                    CardTypeText = "护照";//证件号码
                    break;

                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet, "R");//证件号码
                    CardTypeText = "户口簿";//证件号码
                    break;

                default:
                    break;
            }
            SetBookmarkValue("ContractorCardType", CardTypeText);//证件类型
        }

        /// <summary>
        /// 设置承包类型
        /// </summary>
        protected virtual void WriteConcordModeInformation()
        {
            if (Concord == null && DictList != null)
            {
                return;
            }
            int number = 0;
            int.TryParse(Concord.ArableLandType, out number);
            if (number <= 0)
                return;
            eConstructMode mode = (eConstructMode)number;
            switch (mode)
            {
                case eConstructMode.Consensus:
                    SetBookmarkValue(AgricultureBookMark.ConsensusContract, "R");//公开协商
                    break;

                case eConstructMode.Exchange:
                    SetBookmarkValue(AgricultureBookMark.ExchangeContract, "R");//互换
                    break;

                case eConstructMode.Family:
                    SetBookmarkValue(AgricultureBookMark.FamilyContract, "R");//家庭承包
                    break;

                case eConstructMode.Other:
                    SetBookmarkValue(AgricultureBookMark.OtherContract, "R");//其他
                    break;

                case eConstructMode.Tenderee:
                    SetBookmarkValue(AgricultureBookMark.TendereeContract, "R");//招标
                    break;

                case eConstructMode.Transfer:
                    SetBookmarkValue(AgricultureBookMark.TransferContract, "R");//转让
                    break;

                case eConstructMode.Vendue:
                    SetBookmarkValue(AgricultureBookMark.VendueContract, "R");//拍卖
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void WriteSharePersonInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            List<Person> persons = SortSharePerson(Contractor.SharePersonList, Contractor.Name);
            if (SystemSet.PersonTable)
            {
                persons = persons.FindAll(c => c.IsSharedLand == "是");
            }
            string nameList = "";

            int personcount = 0;
            foreach (var item in persons)
            {
                if (SystemSet.StatisticsDeadPersonInfo == false && item.Comment.IsNullOrEmpty() == false && item.Comment.Contains("去世"))
                {
                    continue;
                }
                nameList += (item.Name == Contractor.Name) ? InitalizeFamilyName(item.Name) : item.Name;
                nameList += "、";
                personcount++;
            }

            int mainpersoncnt = 0;
            foreach (var item in persons)
            {
                if (item.Comment.IsNullOrEmpty() == false && item.Comment.Contains("去世")) continue;
                if (item.Comment.IsNullOrEmpty() == false && item.Comment.Contains("外嫁")) continue;
                if (item.Comment.IsNullOrEmpty() == false && item.Comment.Contains("迁出")) continue;
                mainpersoncnt++;
            }

            string cutNameList = string.IsNullOrEmpty(nameList) ? "" : nameList.Substring(0, nameList.Length - 1);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorCount + (i == 0 ? "" : i.ToString()), personcount.ToString());//承包方家庭成员个数
                SetBookmarkValue(AgricultureBookMark.ContractorMainCount + (i == 0 ? "" : i.ToString()), mainpersoncnt.ToString());//承包方家庭成员个数不包括：去世、外嫁、迁出  人数
                SetBookmarkValue(AgricultureBookMark.SharePersonCount + (i == 0 ? "" : i.ToString()), personcount.ToString());//承包方家庭成员个数
                SetBookmarkValue(AgricultureBookMark.SharePersonString + (i == 0 ? "" : i.ToString()), cutNameList);//共有人字符串
            }
            Person familyPerson = null;
            int index = 1;
            foreach (Person person in persons)
            {
                if (SystemSet.StatisticsDeadPersonInfo == false && person.Comment.IsNullOrEmpty() == false && person.Comment.Contains("去世"))
                {
                    continue;
                }
                if (person.Name == Contractor.Name && person.ICN == Contractor.Number)
                {
                    familyPerson = person.Clone() as Person;
                }
                string name = AgricultureBookMark.SharePersonName + index.ToString();
                SetBookmarkValue(name, person.Name == Contractor.Name ? InitalizeFamilyName(person.Name) : person.Name);//共有人姓名
                string gender = AgricultureBookMark.SharePersonGender + index.ToString();
                string sex = person.Gender == eGender.Female ? "女" : (person.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(gender, sex);//共有人性别
                string ageString = AgricultureBookMark.SharePersonAge + index.ToString();
                SetBookmarkValue(ageString, GetAge(person));//共有人年龄
                string birthString = AgricultureBookMark.SharePersonBirthday + index.ToString();
                SetBookmarkValue(birthString, GetPersonBirthday(person, 0));//共有人出生日期
                string birthDayString = AgricultureBookMark.SharePersonBirthMonthDay + index.ToString();
                SetBookmarkValue(birthDayString, GetPersonBirthday(person, 2));//共有人出生日期
                string birthAllString = AgricultureBookMark.SharePersonAllBirthday + index.ToString();
                SetBookmarkValue(birthAllString, GetPersonBirthday(person, 1));//共有人全出生日期
                string relationString = AgricultureBookMark.SharePersonRelation + index.ToString();
                SetBookmarkValue(relationString, person.Relationship);//家庭关系
                string icnNumber = AgricultureBookMark.SharePersonNumber + index.ToString();
                SetBookmarkValue(icnNumber, person.ICN);//身份证号码
                string nation = AgricultureBookMark.SharePersonNation + index.ToString();
                string nationString = EnumNameAttribute.GetDescription(person.Nation);
                SetBookmarkValue(nation, nationString == "未知" ? "" : nationString);//共有人民族
                string nature = AgricultureBookMark.SharePersonAccountNature + index.ToString();
                SetBookmarkValue(nature, person.AccountNature);//共有人性质
                string comment = AgricultureBookMark.SharePersonComment + index.ToString();
                SetBookmarkValue(comment, person.Comment);//备注

                string sfcomment = "SFGYR" + index.ToString();
                SetBookmarkValue(sfcomment, person.IsSharedLand);//备注

                index++;
            }
            persons = null;
            if (familyPerson == null)
            {
                return;
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                string sex = familyPerson.Gender == eGender.Female ? "女" : (familyPerson.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(AgricultureBookMark.ContractorGender + (i == 0 ? "" : i.ToString()), sex);//承包方性别
                SetBookmarkValue(AgricultureBookMark.ContractorAge + (i == 0 ? "" : i.ToString()), GetAge(familyPerson));//承包方年龄
                SetBookmarkValue(AgricultureBookMark.ContractorBirthday + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 0));//承包方初始日期
                SetBookmarkValue(AgricultureBookMark.ContractorAllBirthday + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 1));//承包方初始日期
                SetBookmarkValue(AgricultureBookMark.ContractorBirthMonthDay + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 2));//承包方初始日期
            }
        }

        /// <summary>
        /// 获取人的年龄
        /// </summary>
        /// <returns></returns>
        protected string GetPersonBirthday(Person person, int birth)
        {
            if (person.Birthday == null || !person.Birthday.HasValue)
            {
                if (!string.IsNullOrEmpty(person.ICN))
                {
                    person.Birthday = ToolICN.GetBirthdayInNotCheck(person.ICN);
                }
            }
            if (person.Birthday == null || !person.Birthday.HasValue)
            {
                return "";
            }
            string birthString = "";
            DateTime birthDay = person.Birthday.Value;
            switch (birth)
            {
                case 1:
                    birthString = birthDay.Year.ToString() + "." + birthDay.Month.ToString() + "." + birthDay.Day.ToString();
                    break;

                case 2:
                    birthString = birthDay.Year.ToString() + "." + birthDay.Month.ToString();
                    break;

                default:
                    birthString = ToolDateTime.GetLongDateString(birthDay);
                    break;
            }
            return birthString;
        }

        /// <summary>
        /// 对共有人排序(户主最前面)
        /// </summary>
        protected List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            if (personCollection == null || personCollection.Count == 0)
                return sharePersonCollection;
            personCollection.ForEach(c =>
            {
                if (c == null || string.IsNullOrEmpty(c.Name))
                    return;
                if (c.Name.Contains(" "))
                    c.Name = c.Name.Trim();
            });
            Person p = personCollection.Find(t => t.Name == houseName);
            if (p != null)
            {
                sharePersonCollection.Add(p);
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        protected string GetAge(Person person)
        {
            if (person.Birthday != null && person.Birthday.HasValue && person.Birthday.Value.Date == DateTime.Today.Date)
            {
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            int age = person.GetAge();
            person = null;
            if (age < 1 || age > 200)
            {
                return "     ";
            }
            else
            {
                return age.ToString();
            }
        }

        #endregion Contractor

        #region Contractland

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        protected virtual void WriteLandInformation()
        {
            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }
            var dldjdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.DLDJ).ToDictionary(d => d.Code);
            var qdfsdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS).ToDictionary(d => d.Code);
            var dklbdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.DKLB).ToDictionary(d => d.Code);
            var jyfsdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.JYFS).ToDictionary(d => d.Code);
            var tdytdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.TDYT).ToDictionary(d => d.Code);
            var zzlxdic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.ZZLX).ToDictionary(d => d.Code);
            var gbzldic = DictList.FindAll(d => d.GroupCode == DictionaryTypeInfo.GBZL).ToDictionary(d => d.Code);

            int index = 1;
            LandCollection = SortLandCollection(LandCollection);
            foreach (var land in LandCollection)
            {
                AgricultureLandExpand expand = land.LandExpand;
                SetBookmarkValue(AgricultureBookMark.AgricultureName + index, land.Name);//地块名称
                string landNumber = land.LandNumber;
                SetBookmarkValue(AgricultureBookMark.AgricultureNumber + index, landNumber);//地块编码
                SetBookmarkValue(AgricultureBookMark.AgricultureActualArea + index, land.ActualArea.AreaFormat(SystemSet.DecimalPlaces, true));//实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureAwareArea + index, land.AwareArea.AreaFormat(SystemSet.DecimalPlaces, true));//确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureTableArea + index, land.TableArea.AreaFormat(SystemSet.DecimalPlaces, true));//台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureModoArea + index, land.MotorizeLandArea.AreaFormat(SystemSet.DecimalPlaces));//地块机动地面积
                InitalizeSmallNumber(index, ContractLand.GetLandNumber(land.CadastralNumber));

                Dictionary level, mode, callog, manager, plant, plat, purpose = null;
                dldjdic.TryGetValue(land.LandLevel == null ? "" : land.LandLevel, out level);
                qdfsdic.TryGetValue(land.ConstructMode == null ? "" : land.ConstructMode, out mode);
                dklbdic.TryGetValue(land.LandCategory == null ? "" : land.LandCategory, out callog);
                jyfsdic.TryGetValue(land.ManagementType == null ? "" : land.ManagementType, out manager);
                gbzldic.TryGetValue(land.PlantType == null ? "" : land.PlantType, out plant);
                zzlxdic.TryGetValue(land.PlatType == null ? "" : land.PlatType, out plat);
                tdytdic.TryGetValue(land.Purpose == null ? "" : land.Purpose, out purpose);

                //land.LandLevel.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DLDJ && d.Code == land.LandLevel);
                string levelString = level != null ? level.Name : "";
                levelString = levelString == "未知" ? "" : levelString;
                levelString = AgricultureSetting.UseSystemLandLevelDescription ? levelString : InitalizeLandLevel(land.LandLevel);
                SetBookmarkValue(AgricultureBookMark.AgricultureLandLevel + index, levelString);//等级
                string landName = !string.IsNullOrEmpty(land.LandName) ? land.LandName : "";
                if (string.IsNullOrEmpty(landName) && DictList != null && land.LandCode != null)
                {
                    Dictionary lt = DictList.Find(ld => ld.Code == land.LandCode);
                    landName = lt != null ? lt.Name : "";
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureLandType + index, landName == "未知" ? "" : landName);//地类
                SetBookmarkValue(AgricultureBookMark.AgricultureIsFarmarLand + index, (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue) ? "" : (land.IsFarmerLand.Value ? "是" : "否"));//是否基本农田
                SetBookmarkValue(AgricultureBookMark.AgricultureEast + index, land.NeighborEast);//东
                SetBookmarkValue(AgricultureBookMark.AgricultureEastName + index, "东:" + land.NeighborEast);//东
                SetBookmarkValue(AgricultureBookMark.AgricultureSouth + index, land.NeighborSouth);//南
                SetBookmarkValue(AgricultureBookMark.AgricultureSouthName + index, "南:" + land.NeighborSouth);//南
                SetBookmarkValue(AgricultureBookMark.AgricultureWest + index, land.NeighborWest);//西
                SetBookmarkValue(AgricultureBookMark.AgricultureWestName + index, "西:" + land.NeighborWest);//西
                SetBookmarkValue(AgricultureBookMark.AgricultureNorth + index, land.NeighborNorth);//北
                SetBookmarkValue(AgricultureBookMark.AgricultureNorthName + index, "北:" + land.NeighborNorth);//北
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighbor + index, SystemSet.NergionbourSet ? InitalizeLandNeightor(land) : "见附图");//四至
                SetBookmarkValue(AgricultureBookMark.AgricultureNoPreNeighbor + index, SystemSet.NergionbourSet ? InitalizeLandNeightor1(land) : "见附图");//四至
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighborFigure + index, "见附图");//四至见附图
                SetBookmarkValue(AgricultureBookMark.AgricultureComment + index, land.Comment);//地块备注
                //Dictionary mode = land.ConstructMode.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS && d.Code == land.ConstructMode);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstructMode + index, mode != null ? mode.Name : "");//承包方式
                //Dictionary callog = land.LandCategory.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DKLB && d.Code == land.LandCategory);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstractType + index, callog != null ? callog.Name : "");//地块类别
                SetBookmarkValue(AgricultureBookMark.AgriculturePlotNumber + index, land.PlotNumber);//地块畦数
                //Dictionary manager = land.ManagementType.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.ManagementType);
                SetBookmarkValue(AgricultureBookMark.AgricultureManagerType + index, manager != null ? manager.Name : "");//地块经营方式
                SetBookmarkValue(AgricultureBookMark.AgricultureSourceFamilyName + index, land.FormerPerson);//原户主姓名
                //Dictionary plant = land.PlantType.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.GBZL && d.Code == land.PlantType);
                string plantType = plant != null ? plant.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlantType + index, plantType == "未知" ? "" : plantType);//耕保类型
                if (land.IsTransfer)
                {
                    var transMode = land.TransferType.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.TransferType);
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterMode + index, transMode != null ? transMode.Name : "");//流转方式
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterTerm + index, land.TransferTime);//流转期限
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterArea + index, land.PertainToArea.AreaFormat());//流转面积
                }
                //Dictionary plat = land.PlatType.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.ZZLX && d.Code == land.PlatType);
                string platType = plat != null ? plat.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlatType + index, platType == "未知" ? "" : platType);//种植类型
                //Dictionary purpose = land.Purpose.IsNullOrEmpty() ? null : DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == land.Purpose);
                string landPurpose = purpose != null ? purpose.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePurpose + index, (string.IsNullOrEmpty(landPurpose) || ToolMath.MatchEntiretyNumber(landPurpose)) ? "种植业" : landPurpose);//土地用途
                SetBookmarkValue(AgricultureBookMark.AgricultureUseSituation + index, expand.UseSituation);//土地利用情况
                SetBookmarkValue(AgricultureBookMark.AgricultureYield + index, expand.Yield);//土地产量情况
                SetBookmarkValue(AgricultureBookMark.AgricultureOutputValue + index, expand.OutputValue);//土地产值情况
                SetBookmarkValue(AgricultureBookMark.AgricultureIncomeSituation + index, expand.IncomeSituation);//土地收益情况
                SetBookmarkValue(AgricultureBookMark.AgricultureElevation + index, expand.Elevation.ToString());//高程
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyPerson + index, expand.SurveyPerson);//地块调查员
                if (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureSurveyDate + index, ToolDateTime.GetLongDateString(expand.SurveyDate.Value));//地块调查日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyChronicle + index, expand.SurveyChronicle);//地块调查记事
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckPerson + index, expand.CheckPerson);//地块审核员
                if (expand.CheckDate != null && expand.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureCheckDate + index, ToolDateTime.GetLongDateString(expand.CheckDate.Value));//地块审核日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckOpinion + index, expand.CheckOpinion);//地块审核意见
                SetBookmarkValue(AgricultureBookMark.AgricultureImageNumber + index, expand.ImageNumber);//地块图幅号
                SetBookmarkValue(AgricultureBookMark.AgricultureFefer + index, expand.ReferPerson);//地块指界人
                index++;
            }
            WriteLandCalInformation();
            WriteReclamationInformation();
            SetBookmarkValue(AgricultureBookMark.AgricultureAllLandsNeighbor, InitalizeLandNeightor1(LandCollection));//所有四至
        }

        /// <summary>
        /// 获取地块编码
        /// </summary>
        /// <param name="vaule"></param>
        /// <returns></returns>
        protected virtual string GetLandNumber(string vaule)
        {
            string landNumber = vaule;
            if (SystemSet.LandNumericFormatSet)
            {
                int getlandnumcount = SystemSet.LandNumericFormatValueSet;
                int length = landNumber.Length;
                if (length > getlandnumcount)
                {
                    landNumber = landNumber.Substring(getlandnumcount, length - getlandnumcount);
                }
            }
            return landNumber;
        }

        /// <summary>
        /// 初始化四至
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        protected virtual string InitalizeLandNeightor(ContractLand land)
        {
            string neighbor = string.Format("东：{0}\n南：{1}\n西：{2} \n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("东：{0}\n西：{1}\n南：{2} \n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
            }
            return neighbor;
        }

        /// <summary>
        /// 初始化四至
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        protected virtual string InitalizeLandNeightor1(ContractLand land)
        {
            string neighbor = string.Format("{0}\n{1}\n{2} \n{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("{0}\n{1}\n{2}\n{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
            }
            return neighbor;
        }

        /// <summary>
        /// 初始化四至-只要人名，不要道路 沟渠 田埂
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        protected virtual string InitalizeLandNeightor1(List<ContractLand> lands)
        {
            string allneighbor = "";
            foreach (var item in lands)
            {
                if (item.NeighborEast.IsNotNullOrEmpty())
                {
                    allneighbor += item.NeighborEast + " ";
                }
                if (item.NeighborSouth.IsNotNullOrEmpty())
                {
                    allneighbor += item.NeighborSouth + " ";
                }
                if (item.NeighborWest.IsNotNullOrEmpty())
                {
                    allneighbor += item.NeighborWest + " ";
                }
                if (item.NeighborNorth.IsNotNullOrEmpty())
                {
                    allneighbor += item.NeighborNorth + " ";
                }
            }
            allneighbor.Replace("道路", "").Replace("沟渠", "").Replace("田埂", "");
            return allneighbor;
        }

        /// <summary>
        /// 初始化角码
        /// </summary>
        protected virtual void InitalizeSmallNumber(int order, string landNumber)
        {
            string landName = landNumber;
            int index = landNumber.IndexOf("-");
            if (index < 0)
            {
                index = landNumber.IndexOf("－");
            }
            if (index < 0)
            {
                return;
            }
            string surfix = landNumber.Substring(0, index);
            string number = ToolString.GetAllNumberWithInString(surfix);
            if (string.IsNullOrEmpty(number))
            {
                return;
            }
            SetBookmarkValue(AgricultureBookMark.AgricultureSmallNumber + order, number);
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        protected virtual List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                if (landNumber.Length > 14)
                {
                    landNumber = landNumber.Substring(14);
                }
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        /// <summary>
        /// 书写地块计算信息
        /// </summary>
        protected virtual void WriteLandCalInformation()
        {
            List<ContractLand> lands = LandCollection.FindAll(ld => ld.LandCategory == ConstructType);
            List<ContractLand> otherLands = LandCollection.FindAll(ld => ld.LandCategory != ConstructType);
            double actualArea = 0.0;
            double conLandActualArea = 0.0;
            double othLandActualArea = 0.0;
            double awareArea = 0.0;
            double conLandawareArea = 0.0;
            double othLandawareArea = 0.0;
            double tableArea = 0.0;
            double conLandtableArea = 0.0;
            double othLandtableArea = 0.0;
            double modoArea = 0.0;
            double conLandmodoArea = 0.0;
            double othLandmodoArea = 0.0;
            foreach (ContractLand land in LandCollection)
            {
                actualArea += land.ActualArea;
                awareArea += land.AwareArea;
                tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                modoArea += (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue) ? land.MotorizeLandArea.Value : 0.0;
            }
            foreach (ContractLand land in lands)
            {
                conLandActualArea += land.ActualArea;
                conLandawareArea += land.AwareArea;
                conLandtableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                conLandmodoArea += (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue) ? land.MotorizeLandArea.Value : 0.0;
            }
            othLandActualArea = actualArea - conLandActualArea;
            othLandawareArea = awareArea - conLandawareArea;
            othLandtableArea = tableArea - conLandtableArea;
            othLandmodoArea = modoArea - conLandmodoArea;
            var sumarea = LandCollection.Sum(o => Convert.ToDouble(o.ShareArea)).AreaFormat();
            for (int j = 0; j < BookMarkCount; j++)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCount + (j == 0 ? "" : j.ToString()), LandCollection.Count > 0 ? LandCollection.Count.ToString() : string.Empty);//地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandCount + (j == 0 ? "" : j.ToString()), lands.Count > 0 ? lands.Count.ToString() : string.Empty);//承包地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherCount + (j == 0 ? "" : j.ToString()), otherLands.Count > 0 ? otherLands.Count.ToString() : string.Empty);//非承包地块总数
                SetBookmarkValue(AgricultureBookMark.AgricultureActualAreaCount + (j == 0 ? "" : j.ToString()), actualArea.AreaFormat(SystemSet.DecimalPlaces));//地块总实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureAwareAreaCount + (j == 0 ? "" : j.ToString()), awareArea.AreaFormat(SystemSet.DecimalPlaces));//地块总确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureTableAreaCount + (j == 0 ? "" : j.ToString()), tableArea.AreaFormat(SystemSet.DecimalPlaces));//地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.AgricultureModoAreaCount + (j == 0 ? "" : j.ToString()), modoArea.AreaFormat(SystemSet.DecimalPlaces));//地块总机动地面积
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandActualAreaCount + (j == 0 ? "" : j.ToString()), conLandActualArea.AreaFormat(SystemSet.DecimalPlaces));//承包地块总实测面积
                if (IsStockLand != null && (bool)IsStockLand)
                    SetBookmarkValue("ContractLandActualAreaCount", sumarea);
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandAwareAreaCount + (j == 0 ? "" : j.ToString()), conLandawareArea.AreaFormat(SystemSet.DecimalPlaces));//承包地块总确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandTableAreaCount + (j == 0 ? "" : j.ToString()), conLandtableArea.AreaFormat(SystemSet.DecimalPlaces));//承包地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.AgricultureContractLandModoAreaCount + (j == 0 ? "" : j.ToString()), conLandmodoArea.AreaFormat(SystemSet.DecimalPlaces));//承包地块总机动地面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandActualAreaCount + (j == 0 ? "" : j.ToString()), othLandActualArea.AreaFormat(SystemSet.DecimalPlaces));//地块总实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandAwareAreaCount + (j == 0 ? "" : j.ToString()), othLandawareArea.AreaFormat(SystemSet.DecimalPlaces));//地块总确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandTableAreaCount + (j == 0 ? "" : j.ToString()), othLandtableArea.AreaFormat(SystemSet.DecimalPlaces));//地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.AgricultureOtherLandModoAreaCount + (j == 0 ? "" : j.ToString()), othLandmodoArea.AreaFormat(SystemSet.DecimalPlaces));//地块总机动地面积
            }
        }

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        protected virtual void WriteReclamationInformation()
        {
            List<ContractLand> landCollection = LandCollection.Clone() as List<ContractLand>;
            List<ContractLand> landArray = landCollection.FindAll(ld => (!string.IsNullOrEmpty(ld.Comment) && ld.Comment.IndexOf("开垦地") >= 0));
            double reclamationTableArea = 0.0;//开垦地台帐面积
            double reclamationActualArea = 0.0;//开垦地实测面积
            double reclamationAwareArea = 0.0;//开垦地确权面积
            foreach (ContractLand land in landArray)
            {
                reclamationTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                reclamationActualArea += land.ActualArea;
                reclamationAwareArea += land.AwareArea;
                landCollection.Remove(land);
            }
            double retainTableArea = 0.0;
            double retainActualArea = 0.0;
            double retainAwareArea = 0.0;
            foreach (ContractLand land in landCollection)
            {
                retainTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                retainActualArea += land.ActualArea;
                retainAwareArea += land.AwareArea;
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationTableArea + (i == 0 ? "" : i.ToString()), reclamationTableArea.AreaFormat(SystemSet.DecimalPlaces));//台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainTableArea + (i == 0 ? "" : i.ToString()), retainTableArea.AreaFormat(SystemSet.DecimalPlaces));//台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationActualArea + (i == 0 ? "" : i.ToString()), reclamationActualArea.AreaFormat(SystemSet.DecimalPlaces));//实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainActualArea + (i == 0 ? "" : i.ToString()), retainActualArea.AreaFormat(SystemSet.DecimalPlaces));//实测面积
                SetBookmarkValue(AgricultureBookMark.AgricultureReclationAwareArea + (i == 0 ? "" : i.ToString()), reclamationAwareArea.AreaFormat(SystemSet.DecimalPlaces));//确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureRetainAwareArea + (i == 0 ? "" : i.ToString()), retainAwareArea.AreaFormat(SystemSet.DecimalPlaces));//确权面积
            }
            landCollection.Clear();
            landArray.Clear();
        }

        /// <summary>
        /// 获取等级
        /// </summary>
        protected string InitalizeLandLevel(string landLevel)
        {
            if (DictList == null)
            {
                return "";
            }
            Dictionary dic = DictList.Find(t => t.Code == landLevel);
            if (dic == null)
            {
                return "";
            }
            return dic.Name;
        }

        #endregion Contractland

        #region Sender

        /// <summary>
        /// 书写发包方信息
        /// </summary>
        protected virtual void WriteSenderInformation()
        {
            if (Tissue == null)
            {
                return;
            }
            string senderNameExpress = InitalizeSenderExpress();//发包方名称扩展如(第一村民小组)。
            var countyName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_COUNTY_LENGTH);
            string townName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            var zjlxinfo = Tissue.LawyerCredentType;
            var ZJLX = (int)zjlxinfo;
            Dictionary layerCard;
            layerCard = (DictList == null ? new List<Dictionary>() : DictList).Find(d => d.GroupCode == DictionaryTypeInfo.ZJLX && d.Code == ZJLX.ToString());
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.SenderName + (i == 0 ? "" : i.ToString()), Tissue.Name.GetSettingEmptyReplacement());//发包方名称
                SetBookmarkValue("SocialCode" + (i == 0 ? "" : i.ToString()), Tissue.SocialCode);//社会信用代码
                SetBookmarkValue(AgricultureBookMark.SenderNameExpress + (i == 0 ? "" : i.ToString()), senderNameExpress);//发包方名称扩展如(第一村民小组)
                SetBookmarkValue(AgricultureBookMark.SenderLawyerName + (i == 0 ? "" : i.ToString()), Tissue.LawyerName);//发包方法人名称
                SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone + (i == 0 ? "" : i.ToString()), Tissue.LawyerTelephone);//发包方法人联系方式
                SetBookmarkValue(AgricultureBookMark.SenderLawyerAddress + (i == 0 ? "" : i.ToString()), Tissue.LawyerAddress);//发包方法人地址
                SetBookmarkValue(AgricultureBookMark.SenderLawyerPostNumber + (i == 0 ? "" : i.ToString()), Tissue.LawyerPosterNumber);//发包方法人邮政编码
                SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentType + (i == 0 ? "" : i.ToString()), layerCard != null ? layerCard.Name : "");//发包方法人证件类型
                SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber + (i == 0 ? "" : i.ToString()), Tissue.LawyerCartNumber);//发包方法人证件号码
                SetBookmarkValue(AgricultureBookMark.SenderCode + (i == 0 ? "" : i.ToString()), Tissue.Code);//发包方代码
                SetBookmarkValue(AgricultureBookMark.SenderCountyName + (i == 0 ? "" : i.ToString()), countyName.GetSettingEmptyReplacement());//发包方到县
                SetBookmarkValue(AgricultureBookMark.SenderTownName + (i == 0 ? "" : i.ToString()), townName.GetSettingEmptyReplacement());//发包方到镇
                SetBookmarkValue(AgricultureBookMark.SenderVillageName + (i == 0 ? "" : i.ToString()), villageName.GetSettingEmptyReplacement());//发包方到村
                SetBookmarkValue(AgricultureBookMark.SenderGroupName + (i == 0 ? "" : i.ToString()), groupName.GetSettingEmptyReplacement());//发包方到组
                SetBookmarkValue(AgricultureBookMark.SenderSurveyChronicle + (i == 0 ? "" : i.ToString()), Tissue.SurveyChronicle.GetSettingEmptyReplacement());//调查记事
                SetBookmarkValue(AgricultureBookMark.SenderSurveyPerson + (i == 0 ? "" : i.ToString()), Tissue.SurveyPerson.GetSettingEmptyReplacement());//调查员
                if (Tissue.SurveyDate != null && Tissue.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.SenderSurveyDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Tissue.SurveyDate.Value).GetSettingEmptyReplacement());//调查日期
                }
                SetBookmarkValue(AgricultureBookMark.SenderCheckPerson + (i == 0 ? "" : i.ToString()), Tissue.CheckPerson.GetSettingEmptyReplacement());//审核员
                if (Tissue.CheckDate != null && Tissue.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.SenderCheckDate + (i == 0 ? "" : i.ToString()),
                        ToolDateTime.GetLongDateString(Tissue.CheckDate.Value).GetSettingEmptyReplacement());//审核日期
                }
                SetBookmarkValue(AgricultureBookMark.SenderChenkOpinion + (i == 0 ? "" : i.ToString()), Tissue.CheckOpinion.GetSettingEmptyReplacement());//审核意见
            }
        }

        #endregion Sender

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        protected virtual void WriteConcordInformation()
        {
            if (Concord == null || string.IsNullOrEmpty(Concord.ConcordNumber))
            {
                return;
            }
            string townName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Concord.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            int landCount = 0;
            if (landCount == 0 && LandCollection != null)
            {
                landCount = LandCollection.Count(ld => ld.ConcordId == Concord.ID);
            }
            var purpose = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == Concord.LandPurpose);
            string landPurpose = null;
            if (purpose != null)
                landPurpose = purpose.Name;
            if (!string.IsNullOrEmpty(landPurpose) && ToolMath.MatchEntiretyNumber(landPurpose))
            {
                landPurpose = "种植业";
            }
            var dic = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS && d.Code == Concord.ArableLandType);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ConcordNumber + (i == 0 ? "" : i.ToString()), Concord.ConcordNumber.GetSettingEmptyReplacement());//合同编号
                SetBookmarkValue(AgricultureBookMark.ConcordTrem + (i == 0 ? "" : i.ToString()), Concord.ManagementTime);//合同期限
                SetBookmarkValue(AgricultureBookMark.ConcordMode + (i == 0 ? "" : i.ToString()), dic != null ? dic.Name : SystemSet.EmptyReplacement);//合同承包方式
                SetBookmarkValue(AgricultureBookMark.ConcordPurpose + (i == 0 ? "" : i.ToString()), landPurpose);//合同土地用途
                SetBookmarkValue(AgricultureBookMark.ConcordLandCount + (i == 0 ? "" : i.ToString()), landCount.ToString());//合同中地块总数
                SetBookmarkValue(AgricultureBookMark.ConcordActualAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountActualArea.AreaFormat());//合同总实测面积
                SetBookmarkValue(AgricultureBookMark.ConcordAwareAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountAwareArea.AreaFormat());//合同总确权面积
                SetBookmarkValue(AgricultureBookMark.ConcordTableAreaCount + (i == 0 ? "" : i.ToString()), Concord.TotalTableArea.AreaFormat());//合同块总二轮台账面积
                SetBookmarkValue(AgricultureBookMark.ConcordModoAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountMotorizeLandArea.AreaFormat());//合同总机动地面积
                SetBookmarkValue(AgricultureBookMark.ConcordAddress + (i == 0 ? "" : i.ToString()), Concord.SecondContracterLocated);//合同中承包方地址
            }
            WriteConcordModeInformation();
            WriteConcordStartAndEndTime();
        }

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        protected virtual void WriteConcordStartAndEndTime()
        {
            if (Concord == null)
                return;
            string startYear = (Concord.ArableLandStartTime == null) ? "" : Concord.ArableLandStartTime.Value.Year.ToString();
            string startMonth = (Concord.ArableLandStartTime == null) ? "" : Concord.ArableLandStartTime.Value.Month.ToString();
            string startDay = (Concord.ArableLandStartTime == null) ? "" : Concord.ArableLandStartTime.Value.Day.ToString();
            string endYear = (Concord.ArableLandEndTime == null) ? "" : Concord.ArableLandEndTime.Value.Year.ToString();
            string endMonth = (Concord.ArableLandEndTime == null) ? "" : Concord.ArableLandEndTime.Value.Month.ToString();
            string endDay = (Concord.ArableLandEndTime == null) ? "" : Concord.ArableLandEndTime.Value.Day.ToString();
            string date = "";
            if (Concord.ArableLandStartTime.HasValue && Concord.ArableLandEndTime.HasValue)
                date = Concord.ArableLandStartTime.Value.ToString("yyyy年MM月dd日") + "至" + Concord.ArableLandEndTime.Value.ToString("yyyy年MM月dd日") + "止";
            for (int i = 0; i < BookMarkCount; i++)
            {
                if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue)
                    SetBookmarkValue(AgricultureBookMark.ConcordStartDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Concord.ArableLandStartTime.Value));//起始时间
                SetBookmarkValue(AgricultureBookMark.ConcordStartYearDate + (i == 0 ? "" : i.ToString()), startYear);//起始时间-年
                SetBookmarkValue(AgricultureBookMark.ConcordStartMonthDate + (i == 0 ? "" : i.ToString()), startMonth);//起始时间-月
                SetBookmarkValue(AgricultureBookMark.ConcordStartDayDate + (i == 0 ? "" : i.ToString()), startDay);//起始时间-日
                if (Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
                    SetBookmarkValue(AgricultureBookMark.ConcordEndDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Concord.ArableLandEndTime.Value));//结束时间
                SetBookmarkValue(AgricultureBookMark.ConcordEndYearDate + (i == 0 ? "" : i.ToString()), endYear);//起始时间-年
                SetBookmarkValue(AgricultureBookMark.ConcordEndMonthDate + (i == 0 ? "" : i.ToString()), endMonth);//起始时间-月
                SetBookmarkValue(AgricultureBookMark.ConcordEndDayDate + (i == 0 ? "" : i.ToString()), endDay);//起始时间-日
                SetBookmarkValue(AgricultureBookMark.ConcordTrem + (i == 0 ? "" : i.ToString()), Concord.Flag ? "长久" : Concord.ManagementTime);//合同期限
                SetBookmarkValue(AgricultureBookMark.ConcordDate + (i == 0 ? "" : i.ToString()), Concord.Flag ? "长久" : date);//承包时间
                SetBookmarkValue(AgricultureBookMark.ConcordLongTime + (i == 0 ? "" : i.ToString()), "长久");//合同中长久日期
            }
        }

        #endregion Concord

        #region Book

        /// <summary>
        /// 书写权证信息
        /// </summary>
        protected virtual void WriteBookInformation()
        {
            if (Book == null || string.IsNullOrEmpty(Book.RegeditNumber))
                return;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookNumber + (i == 0 ? "" : i.ToString()), Book.Number);//编号
                SetBookmarkValue(AgricultureBookMark.BookOrganName + (i == 0 ? "" : i.ToString()), Book.SendOrganization);//发证机关名称
                SetBookmarkValue(AgricultureBookMark.BookYear + (i == 0 ? "" : i.ToString()), Book.Year);//年号
                SetBookmarkValue(AgricultureBookMark.BookWarrantNumber + (i == 0 ? "" : i.ToString()), Book.Number);//权证编号
                SetBookmarkValue(AgricultureBookMark.BookAllNumber + (i == 0 ? "" : i.ToString()), Book.SendOrganization + "农地承包权(" + Book.Year + ")第" + Book.RegeditNumber + "号");
                SetBookmarkValue(AgricultureBookMark.BookSerialNumber + (i == 0 ? "" : i.ToString()), Book.SerialNumber);//六位流水号
                SetBookmarkValue(AgricultureBookMark.BookFullSerialNumber + (i == 0 ? "" : i.ToString()), $"{seriseNumber}{Book.SerialNumber.PadLeft(6, '0')}号");//所有权证编号(包括发证机关、年号、流水号)
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookexcursus + (i == 0 ? "" : i.ToString()), Book.ContractRegeditBookExcursus);//附记
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookPerson + (i == 0 ? "" : i.ToString()), Book.ContractRegeditBookPerson);//登簿人
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookTime + (i == 0 ? "" : i.ToString()), Book.ContractRegeditBookTime.HasValue ? Book.ContractRegeditBookTime.Value.ToString("yyyy年MM月dd日") : string.Empty);//打印所有颁证日期
            }
            WriteAwareDateInformation();
            WriteSenderDateInformation();
            if (!string.IsNullOrEmpty(Book.Number))
                SetBookmarkValue("BookSerialNumber12bit", Book.Number.Substring(6, 6) + Environment.NewLine + Book.Number.Substring(12, 6));
        }

        /// <summary>
        /// 填写颁证日期信息
        /// </summary>
        protected virtual void WriteAwareDateInformation()
        {
            string year = Book.SendDate != null ? Book.SendDate.Year.ToString() : "";
            string awareYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.ToString()) : year;
            string oneYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : "";
            string twoYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : "";
            string month = Book.SendDate != null ? Book.SendDate.Month.ToString() : "";
            string awareMonth = !string.IsNullOrEmpty(month) ? ToolMath.GetChineseLowNumber(month.ToString()) : month;
            if (awareMonth.Equals("一十"))
                awareMonth = "十";
            string day = Book.SendDate != null ? Book.SendDate.Day.ToString() : "";
            string awareday = !string.IsNullOrEmpty(day) ? ToolMath.GetChineseLowNumber(day.ToString()) : day;
            if (awareday.Equals("一十"))
            {
                awareday = "十";
            }
            string allAwareString = year + "年" + month + "月" + day + "日";
            string shortYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? year.Substring(2) : year;
            string firstYear = (!string.IsNullOrEmpty(year) && year.Length > 0) ? ToolMath.GetChineseLowNimeric(year.Substring(0, 1)) : year;
            string secondYear = (!string.IsNullOrEmpty(year) && year.Length > 1) ? ToolMath.GetChineseLowNimeric(year.Substring(1, 1)) : year;
            string threeYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : year;
            string fourYear = (!string.IsNullOrEmpty(year) && year.Length > 3) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : year;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookAwareYear + (i == 0 ? "" : i.ToString()), awareYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareFirstYear + (i == 0 ? "" : i.ToString()), firstYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareSecondYear + (i == 0 ? "" : i.ToString()), secondYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareThreeYear + (i == 0 ? "" : i.ToString()), threeYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareFourYear + (i == 0 ? "" : i.ToString()), fourYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookAwareShortYear + (i == 0 ? "" : i.ToString()), shortYear);//打印日期到年后2位
                SetBookmarkValue(AgricultureBookMark.BookAwareOneYear + (i == 0 ? "" : i.ToString()), oneYear);//打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMark.BookAwareLastYear + (i == 0 ? "" : i.ToString()), twoYear);//打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMark.BookAwareMonth + (i == 0 ? "" : i.ToString()), awareMonth);//打印日期到月
                SetBookmarkValue(AgricultureBookMark.BookAwareDay + (i == 0 ? "" : i.ToString()), awareday);//打印日期到日
                SetBookmarkValue(AgricultureBookMark.BookAllAwareDate + (i == 0 ? "" : i.ToString()), allAwareString);//打印所有颁证日期
                SetBookmarkValue(AgricultureBookMark.BookAllAwareFullDate + (i == 0 ? "" : i.ToString()), Book.SendDate != null ? Book.SendDate.ToString("yyyy   MM   dd") : "");//打印所有颁证日期
            }
        }

        /// <summary>
        /// 填写填证日期信息
        /// </summary>
        protected virtual void WriteSenderDateInformation()
        {
            string year = Book.WriteDate != null ? Book.WriteDate.Year.ToString() : "";
            string awareYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.ToString()) : year;
            string oneYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : "";
            string twoYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : "";
            string month = Book.WriteDate != null ? Book.WriteDate.Month.ToString() : "";
            string awareMonth = !string.IsNullOrEmpty(month) ? ToolMath.GetChineseLowNumber(month.ToString()) : month;
            if (awareMonth.Equals("一十"))
            {
                awareMonth = "十";
            }
            string day = Book.WriteDate != null ? Book.WriteDate.Day.ToString() : "";
            string awareday = !string.IsNullOrEmpty(day) ? ToolMath.GetChineseLowNumber(day.ToString()) : day;
            if (awareday.Equals("一十"))
            {
                awareday = "十";
            }
            string allAwareString = year + "年" + month + "月" + day + "日";
            string shortYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? year.Substring(2) : year;
            string firstYear = (!string.IsNullOrEmpty(year) && year.Length > 0) ? ToolMath.GetChineseLowNimeric(year.Substring(0, 1)) : year;
            string secondYear = (!string.IsNullOrEmpty(year) && year.Length > 1) ? ToolMath.GetChineseLowNimeric(year.Substring(1, 1)) : year;
            string threeYear = (!string.IsNullOrEmpty(year) && year.Length > 2) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : year;
            string fourYear = (!string.IsNullOrEmpty(year) && year.Length > 3) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : year;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.BookWriteYear + (i == 0 ? "" : i.ToString()), awareYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteFirstYear + (i == 0 ? "" : i.ToString()), firstYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteSecondYear + (i == 0 ? "" : i.ToString()), secondYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteThreeYear + (i == 0 ? "" : i.ToString()), threeYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteFourYear + (i == 0 ? "" : i.ToString()), fourYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMark.BookWriteShortYear + (i == 0 ? "" : i.ToString()), shortYear);//打印日期到年后2位
                SetBookmarkValue(AgricultureBookMark.BookWriteOneYear + (i == 0 ? "" : i.ToString()), oneYear);//打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMark.BookWriteLastYear + (i == 0 ? "" : i.ToString()), twoYear);//打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMark.BookWriteMonth + (i == 0 ? "" : i.ToString()), awareMonth);//打印日期到月
                SetBookmarkValue(AgricultureBookMark.BookWriteDay + (i == 0 ? "" : i.ToString()), awareday);//打印日期到日
                SetBookmarkValue(AgricultureBookMark.BookAllWriteDate + (i == 0 ? "" : i.ToString()), allAwareString);//打印所有颁证日期
            }
        }

        #endregion Book

        #region Zone

        /// <summary>
        /// 创建发包方扩展
        /// </summary>
        /// <returns></returns>
        protected virtual string InitalizeSenderExpress()
        {
            if (CurrentZone == null)
            {
                return string.Empty;
            }
            string number = ToolString.GetLeftNumberWithInString(CurrentZone.Name);
            if (string.IsNullOrEmpty(number))
            {
                number = CurrentZone.Code;
            }
            return "(第" + ToolMath.GetChineseLowNumber(number) + "村民小组)";
        }

        /// <summary>
        /// 填写行政区域信息
        /// </summary>
        protected virtual void WriteZoneInformation()
        {
            if (CurrentZone == null)
            {
                return;
            }

            string countyCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            string townCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_TOWN_LENGTH);
            string viliiageCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH);
            //Zone county = CurrentZone;
            //陈泽林 20161123
            Zone county = ZoneList != null ? ZoneList.Find(t => t.FullCode == countyCode) : null;
            Zone town = ZoneList != null ? ZoneList.Find(t => t.FullCode == townCode) : null;
            Zone village = ZoneList != null ? ZoneList.Find(t => t.FullCode == viliiageCode) : null;
            string unitName = string.Empty;
            unitName = county != null ? (CurrentZone.FullName != null ? CurrentZone.FullName.Replace(county.FullName, "") : CurrentZone.FullName) : "";
            var cvillageName = "";
            if (town != null && village != null)
                cvillageName = $"{town.Name}{village.Name}";
            if (county != null)
                cvillageName = county.Name + cvillageName;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ZoneName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMark.LocationName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMark.TownUnitName + (i == 0 ? "" : i.ToString()), unitName);
                SetBookmarkValue(AgricultureBookMark.CountyUnitName + (i == 0 ? "" : i.ToString()), county != null ? (county.Name + unitName) : unitName);
                SetBookmarkValue(AgricultureBookMark.CountyVillageName + (i == 0 ? "" : i.ToString()), cvillageName);
            }
            string zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_COUNTY_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.CountyName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallCountyName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_PROVICE_LENGTH);
            var simpleProvinceNamesDics = InitalizeSimpleProvice();
            var simplenamedic = simpleProvinceNamesDics.Where(s => s.Key.Contains(zoneName)).FirstOrDefault();
            var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
            if (Book != null)
                seriseNumber = $"{simplename}（{Book.Year}）{county.Name}农村土地承包经营权证第";
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.ProviceName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallProviceName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
                SetBookmarkValue(AgricultureBookMark.SimpleProviceName + (i == 0 ? "" : i.ToString()), simplename);
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_CITY_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.CityName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallCityName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_TOWN_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.TownName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMark.SmallTownName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            if (CurrentZone.Level >= eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_VILLAGE_LENGTH);
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMark.VillageName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMark.SmallVillageName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", "").Replace("村委", "") : "");
                    SetBookmarkValue(AgricultureBookMark.VillageUnitName + (i == 0 ? "" : i.ToString()), zoneName + CurrentZone.Name);
                }
            }
            if (CurrentZone.Level == eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_GROUP_LENGTH);
                string number = ToolString.GetLeftNumberWithInString(zoneName);
                string groupName = string.IsNullOrEmpty(number) ? zoneName : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
                string smallGroup = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
                string smallnoZoneGroup = smallGroup.IsNullOrEmpty() ? "" : smallGroup.Substring(0, smallGroup.Length - 1).Replace("社", "").Replace("组", "").Replace("屯", "");
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMark.GroupName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMark.ChineseGroupName + (i == 0 ? "" : i.ToString()), groupName);
                    SetBookmarkValue(AgricultureBookMark.SmallChineseGroupName + (i == 0 ? "" : i.ToString()), smallGroup);
                    SetBookmarkValue(AgricultureBookMark.SmallChineseGroupNoZoneName + (i == 0 ? "" : i.ToString()), smallnoZoneGroup);
                    SetBookmarkValue(AgricultureBookMark.SmallGroupName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
                }
            }

            //取特殊组名
            if (Contractor != null && Contractor.Address.IsNotNullOrEmpty())
            {
                string temp = Contractor.Address.GetAfter("村");//获取村后的字符串
                if (Contractor.Address.Contains("村村"))
                {
                    temp = "村" + temp;
                }
                SetBookmarkValue("SpecialGroup", temp);
            }
        }

        #endregion Zone

        #region DateTime

        /// <summary>
        /// 填写日期扩展信息
        /// </summary>
        protected virtual void WriteDateTimeInformation()
        {
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMark.NowYear + (i == 0 ? "" : i.ToString()), DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMark.YearName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckYearName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.NowMonth + (i == 0 ? "" : i.ToString()), DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMark.MonthName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckMonthName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.NowDay + (i == 0 ? "" : i.ToString()), DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMark.DayName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Day.ToString() : "");
                SetBookmarkValue(AgricultureBookMark.CheckDayName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Day.ToString() : "");
                //SetBookmarkValue(AgricultureBookMark.FullDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(DateTime.Now));
                SetBookmarkValue(AgricultureBookMark.FullDate + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? ToolDateTime.GetLongDateString((DateTime)DateValue) : "");
                string year = ToolMath.GetChineseLowNumber(DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseYearName + (i == 0 ? "" : i.ToString()), year);
                string month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseMonthName + (i == 0 ? "" : i.ToString()), month);
                string day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMark.ChineseDayName + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue(AgricultureBookMark.FullChineseDate + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        #endregion DateTime

        #region Parcel

        /// <summary>
        /// 书写地块示意图信息
        /// </summary>
        protected virtual void WriteParcelInformation()
        {
            string drawPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSetting.DRAWPERSON, "");
            string checkPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSetting.VERIFYPERSON, "");
            if (!string.IsNullOrEmpty(drawPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawPerson, drawPerson);//制图人
            }
            DateTime date = DateTime.Now;
            string value = ToolConfiguration.GetSpecialAppSettingValue("GraphDateTime", "");
            try
            {
                value = !string.IsNullOrEmpty(value) ? value.Replace(",", "") : value;
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime.TryParse(value, out date);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (date != null)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawDate, ToolDateTime.GetLongDateString(date));//制图日期
            }
            if (!string.IsNullOrEmpty(checkPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckPerson, checkPerson);//审核人
            }
            value = ToolConfiguration.GetSpecialAppSettingValue("CheckDateTime", "");
            try
            {
                value = !string.IsNullOrEmpty(value) ? value.Replace(",", "") : value;
                if (!string.IsNullOrEmpty(value))
                {
                    DateTime.TryParse(value, out date);
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (date != null)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckDate, ToolDateTime.GetLongDateString(date));//审核日期
            }
        }

        #endregion Parcel

        #region Other

        /// <summary>
        /// 书写其他信息
        /// </summary>
        protected virtual void WriteOtherInformation()
        {
            if (!string.IsNullOrEmpty(ParcelDrawPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawPerson, ParcelDrawPerson);//制图人
            }
            if (ParcelDrawDate != null && ParcelDrawDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandDrawDate, ToolDateTime.GetLongDateString(ParcelDrawDate.Value));//制图日期
            }
            if (!string.IsNullOrEmpty(ParcelCheckPerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckPerson, ParcelCheckPerson);//审核人
            }
            if (ParcelCheckDate != null && ParcelCheckDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureLandCheckDate, ToolDateTime.GetLongDateString(ParcelCheckDate.Value));//审核日期
            }
            if (!string.IsNullOrEmpty(DrawTablePerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureDrawTablePerson, DrawTablePerson);//制表人
            }
            if (DrawTableDate != null && DrawTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureDrawTableDate, ToolDateTime.GetLongDateString(DrawTableDate.Value));//制表日期
            }
            if (!string.IsNullOrEmpty(CheckTablePerson))
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckTablePerson, CheckTablePerson);//检查人
            }
            if (CheckTableDate != null && CheckTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckTableDate, ToolDateTime.GetLongDateString(CheckTableDate.Value));//制表日期
            }
        }

        #endregion Other

        #region Helper

        /// <summary>
        /// 初始化地域名称
        /// </summary>
        protected virtual string InitalizeZoneName(string zoneCode, int length)
        {
            string zoneName = string.Empty;
            if (ZoneList == null || ZoneList.Count == 0 || zoneCode.Length < length)
            {
                return zoneName;
            }
            string code = zoneCode.Substring(0, length);
            Zone zone = ZoneList.Find(t => t.FullCode == code);
            if (zone != null)
            {
                zoneName = zone.Name;
            }
            return zoneName;
        }

        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public string InitalizeFamilyName(string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return SystemSetDefine.GetIntence()?.EmptyReplacement;
            }
            if (!SystemSet.KeepRepeatFlag)
            {
                return familyName;
            }
            string number = ToolString.GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }

        /// 初始化省市简写
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, string> InitalizeSimpleProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "京");
            dic.Add("天津市", "津");
            dic.Add("河北省", "冀");
            dic.Add("山西省", "晋");
            dic.Add("内蒙古自治区", "内蒙古");
            dic.Add("辽宁省", "辽");
            dic.Add("吉林省", "吉");
            dic.Add("黑龙江省", "黑");
            dic.Add("上海市", "沪");
            dic.Add("江苏省", "苏");
            dic.Add("浙江省", "浙");
            dic.Add("安徽省", "皖");
            dic.Add("福建省", "闽");
            dic.Add("江西省", "赣");
            dic.Add("山东省", "鲁");
            dic.Add("河南省", "豫");
            dic.Add("湖北省", "鄂");
            dic.Add("湖南省", "湘");
            dic.Add("广东省", "粤");
            dic.Add("广西壮族自治区", "桂");
            dic.Add("海南省", "琼");
            dic.Add("重庆市", "渝");
            dic.Add("四川省", "川");
            dic.Add("贵州省", "黔");
            dic.Add("云南省", "云");
            dic.Add("西藏自治区", "藏");
            dic.Add("陕西省", "陕");
            dic.Add("甘肃省", "甘");
            dic.Add("青海省", "青");
            dic.Add("宁夏回族自治区", "宁");
            dic.Add("新疆维吾尔自治区", "新");
            dic.Add("香港特别行政区", "港");
            dic.Add("澳门特别行政区", "澳");
            dic.Add("台湾省", "台");
            return dic;
        }

        #endregion Helper

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        protected string GetZoneNameByLevel(string zoneCode, YuLinTu.Library.Entity.eZoneLevel level, IZoneWorkStation zoneStation)
        {
            YuLinTu.Library.Entity.Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            return GetZoneNameByLevel(temp.UpLevelCode, level, zoneStation);
        }

        #endregion Methods
    }
}