/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// Word文件处理基类
    /// </summary>
    public class AgricultureWordBookWork : WordBase
    {
        #region Property

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
        public SystemSetDefineWork SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefineWork>();
                var section = profile.GetSection<SystemSetDefineWork>();
                var config = section.Settings as SystemSetDefineWork;
                return config;
            }
            set
            {
                
            }
        }

        #endregion

        #region Methods

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public AgricultureWordBookWork()
        {
            BookMarkCount = AgricultureSettingWork.SystemBookMarkNumber;
            ConstructType = "";

            //string fileName = System.Windows.Forms.Application.StartupPath + @"\Config\LandType.xml";
            //landTypeCollection = ToolSerialization.DeserializeXml(fileName, typeof(LandTypeCollection)) as LandTypeCollection;
            //BookMarkCount = AgricultureSettingWork.SystemBookMarkNumber;
            DateValue = DateTime.Now;
            DateChecked = DateTime.Now;


        }

        #endregion

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
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
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitialEntity(object data)
        {
            if (data == null)
            {
                return;
            }
            if (data is VirtualPerson)
            {
                Contractor = data as VirtualPerson;
            }
            if (data is Zone)
            {
                CurrentZone = data as Zone;
            }
            if (data is CollectivityTissue)
            {
                Tissue = data as CollectivityTissue;
            }
            if (data is ContractConcord)
            {
                Concord = data as ContractConcord;
            }
            if (data is ContractRegeditBook)
            {
                Book = data as ContractRegeditBook;
            }
            if (data is List<ContractLand>)
            {
                LandCollection = data as List<ContractLand>;
            }
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
            GC.Collect();
        }

        #endregion

        #region Contractor

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        private void WriteContractorInformaion()
        {
            if (Contractor == null)
            {
                return;
            }
            string townName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Contractor.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            int familyNumber = 0;
            if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
            {
                Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
            }
            string familyString = familyNumber > 0 ? string.Format("{0:D4}", familyNumber) : "";
            string familyAllString = Contractor.ZoneCode;
            familyAllString = familyAllString.PadRight(14, '0') + familyString;
            if (AgricultureSettingWork.AgricultureLandWordFamilyNumber)
            {
                familyString = familyAllString;
            }
            VirtualPersonExpand expand = Contractor.FamilyExpand;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.ContractorName + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(Contractor.Name));//承包方姓名
                SetBookmarkValue(AgricultureBookMarkWork.ContractorNumber + (i == 0 ? "" : i.ToString()), familyString);//承包方户号
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAllNumber + (i == 0 ? "" : i.ToString()), familyAllString);//承包方全户号
                //SetBookmarkValue(AgricultureBookMarkWork.ContractorIdentifyNumber + (i == 0 ? "" : i.ToString()), Contractor.Number);//承包方身份证号码
                if (Contractor.CardType == eCredentialsType.IdentifyCard)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.ContractorIdentifyNumber + (i == 0 ? "" : i.ToString()), Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码
                }
                else
                {
                    SetBookmarkValue(AgricultureBookMarkWork.ContractorOtherCardNumber + (i == 0 ? "" : i.ToString()), Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);  //其他证件号码
                }
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAllocationPerson + (i == 0 ? "" : i.ToString()), expand.AllocationPerson);//承包方实际分配人数
                SetBookmarkValue(AgricultureBookMarkWork.ContractorComment + (i == 0 ? "" : i.ToString()), Contractor.Comment);//承包方备注
                SetBookmarkValue(AgricultureBookMarkWork.ContractorLocation + (i == 0 ? "" : i.ToString()), Contractor.Address.IsNullOrEmpty() ? "/" : Contractor.Address);//坐落地域
                SetBookmarkValue(AgricultureBookMarkWork.ContractorCommunicate + (i == 0 ? "" : i.ToString()), Contractor.Address.IsNullOrEmpty()?"/": Contractor.Address);//通信地址
                SetBookmarkValue(AgricultureBookMarkWork.ContractorTelephone + (i == 0 ? "" : i.ToString()), Contractor.Telephone.IsNullOrEmpty() ? "/" : Contractor.Telephone);//承包方电话号码
                SetBookmarkValue(AgricultureBookMarkWork.ContractorPostNumber + (i == 0 ? "" : i.ToString()), Contractor.PostalNumber.IsNullOrEmpty() ? "/" : Contractor.PostalNumber);//承包方邮政编码
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAddress + (i == 0 ? "" : i.ToString()), Contractor.Address.IsNullOrEmpty() ? "/" : Contractor.Address);//承包方地址
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAddressTown + (i == 0 ? "" : i.ToString()), townName);//承包方地址到镇
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAddressVillage + (i == 0 ? "" : i.ToString()), villageName);//承包方地址到村
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAddressGroup + (i == 0 ? "" : i.ToString()), groupName);//承包方地址到组
                SetBookmarkValue(AgricultureBookMarkWork.ContractorSurveyPerson + (i == 0 ? "" : i.ToString()), expand.SurveyPerson);//承包方调查员
                SetBookmarkValue(AgricultureBookMarkWork.ContractorSurveyDate + (i == 0 ? "" : i.ToString()), (expand.SurveyDate != null && expand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(expand.SurveyDate.Value) : "");//承包方调查员
                SetBookmarkValue(AgricultureBookMarkWork.ContractorSurveyChronicle + (i == 0 ? "" : i.ToString()), expand.SurveyChronicle);//承包方调查记事
                SetBookmarkValue(AgricultureBookMarkWork.ContractorCheckPerson + (i == 0 ? "" : i.ToString()), expand.CheckPerson);//承包方审核员
                SetBookmarkValue(AgricultureBookMarkWork.ContractorCheckDate + (i == 0 ? "" : i.ToString()), (expand.CheckDate != null && expand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(expand.CheckDate.Value) : "");//承包方审核日期
                SetBookmarkValue(AgricultureBookMarkWork.ContractorCheckOpinion + (i == 0 ? "" : i.ToString()), expand.CheckOpinion);//承包方审核意见
            }
            WriteCredentialsInformation();
            WriteSharePersonInformation();
        }

        /// <summary>
        /// 设置证件类型
        /// </summary>
        private void WriteCredentialsInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            eCredentialsType type = Contractor.CardType;
            switch (type)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue(AgricultureBookMarkWork.IdentifyCard, "R");//证件号码
                    break;
                case eCredentialsType.AgentCard:
                    SetBookmarkValue(AgricultureBookMarkWork.AgentCard, "R");//证件号码
                    break;
                case eCredentialsType.OfficerCard:
                    SetBookmarkValue(AgricultureBookMarkWork.OfficerCard, "R");//证件号码
                    break;
                case eCredentialsType.Other:
                    SetBookmarkValue(AgricultureBookMarkWork.CredentialOther, "R");//证件号码
                    break;
                case eCredentialsType.Passport:
                    SetBookmarkValue(AgricultureBookMarkWork.Passport, "R");//证件号码
                    break;
                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue(AgricultureBookMarkWork.ResidenceBooklet, "R");//证件号码
                    break;
                default:
                    SetBookmarkValue(AgricultureBookMarkWork.CredentialOther, "R");//其它证件号码
                    break;
            }
        }

        /// <summary>
        /// 设置承包类型
        /// </summary>
        private void WriteConcordModeInformation()
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
                    SetBookmarkValue(AgricultureBookMarkWork.ConsensusContract, "R");//公开协商
                    break;
                case eConstructMode.Exchange:
                    SetBookmarkValue(AgricultureBookMarkWork.ExchangeContract, "R");//互换
                    break;
                case eConstructMode.Family:
                    SetBookmarkValue(AgricultureBookMarkWork.FamilyContract, "R");//家庭承包
                    break;
                case eConstructMode.Other:
                    SetBookmarkValue(AgricultureBookMarkWork.OtherContract, "R");//其他
                    break;
                case eConstructMode.Tenderee:
                    SetBookmarkValue(AgricultureBookMarkWork.TendereeContract, "R");//招标
                    break;
                case eConstructMode.Transfer:
                    SetBookmarkValue(AgricultureBookMarkWork.TransferContract, "R");//转让
                    break;
                case eConstructMode.Vendue:
                    SetBookmarkValue(AgricultureBookMarkWork.VendueContract, "R");//拍卖
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonInformation()
        {
            if (Contractor == null)
            {
                return;
            }
            List<Person> persons = SortSharePerson(Contractor.SharePersonList, Contractor.Name);
            string nameList = "";
            foreach (var item in persons)
            {
                nameList += (item.Name == Contractor.Name) ? InitalizeFamilyName(item.Name) : item.Name;
                nameList += "、";
            }
            string cutNameList = string.IsNullOrEmpty(nameList) ? "" : nameList.Substring(0, nameList.Length - 1);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.ContractorCount + (i == 0 ? "" : i.ToString()), persons.Count.ToString());//承包方家庭成员个数
                SetBookmarkValue(AgricultureBookMarkWork.SharePersonCount + (i == 0 ? "" : i.ToString()), persons.Count.ToString());//承包方家庭成员个数 
                SetBookmarkValue(AgricultureBookMarkWork.SharePersonString + (i == 0 ? "" : i.ToString()), cutNameList);//共有人字符串
            }
            Person familyPerson = null;
            int index = 1;
            foreach (Person person in persons)
            {
                if (person.Name == Contractor.Name && person.ICN == Contractor.Number)
                {
                    familyPerson = person.Clone() as Person;
                }
                string name = AgricultureBookMarkWork.SharePersonName + index.ToString();
                SetBookmarkValue(name, person.Name == Contractor.Name ? InitalizeFamilyName(person.Name) : person.Name);//共有人姓名
                string gender = AgricultureBookMarkWork.SharePersonGender + index.ToString();
                string sex = person.Gender == eGender.Female ? "女" : (person.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(gender, sex);//共有人性别
                string ageString = AgricultureBookMarkWork.SharePersonAge + index.ToString();
                SetBookmarkValue(ageString, GetAge(person));//共有人年龄
                string birthString = AgricultureBookMarkWork.SharePersonBirthday + index.ToString();
                SetBookmarkValue(birthString, GetPersonBirthday(person, 0));//共有人出生日期
                string birthDayString = AgricultureBookMarkWork.SharePersonBirthMonthDay + index.ToString();
                SetBookmarkValue(birthDayString, GetPersonBirthday(person, 2));//共有人出生日期
                string birthAllString = AgricultureBookMarkWork.SharePersonAllBirthday + index.ToString();
                SetBookmarkValue(birthAllString, GetPersonBirthday(person, 1));//共有人全出生日期
                string relationString = AgricultureBookMarkWork.SharePersonRelation + index.ToString();
                SetBookmarkValue(relationString, person.Relationship);//家庭关系
                string icnNumber = AgricultureBookMarkWork.SharePersonNumber + index.ToString();
                SetBookmarkValue(icnNumber, person.ICN);//身份证号码
                string nation = AgricultureBookMarkWork.SharePersonNation + index.ToString();
                string nationString = EnumNameAttribute.GetDescription(person.Nation);
                SetBookmarkValue(nation, nationString == "未知" ? "" : nationString);//共有人民族
                string nature = AgricultureBookMarkWork.SharePersonAccountNature + index.ToString();
                SetBookmarkValue(nature, person.AccountNature);//共有人性质
                string comment = AgricultureBookMarkWork.SharePersonComment + index.ToString();
                SetBookmarkValue(comment, person.Comment);//备注
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
                SetBookmarkValue(AgricultureBookMarkWork.ContractorGender + (i == 0 ? "" : i.ToString()), sex);//承包方性别
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAge + (i == 0 ? "" : i.ToString()), GetAge(familyPerson));//承包方年龄
                SetBookmarkValue(AgricultureBookMarkWork.ContractorBirthday + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 0));//承包方初始日期
                SetBookmarkValue(AgricultureBookMarkWork.ContractorAllBirthday + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 1));//承包方初始日期
                SetBookmarkValue(AgricultureBookMarkWork.ContractorBirthMonthDay + (i == 0 ? "" : i.ToString()), GetPersonBirthday(familyPerson, 2));//承包方初始日期
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

        #endregion

        #region Contractland

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLandInformation()
        {
            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }
            int index = 1;
            LandCollection = SortLandCollection(LandCollection);
            foreach (var land in LandCollection)
            {
                AgricultureLandExpand expand = land.LandExpand;
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureName + index, land.Name);//地块名称
                string landNumber = ContractLand.GetLandNumber(land.CadastralNumber);
                if (landNumber.Length > AgricultureSettingWork.AgricultureLandNumberMedian)
                {
                    landNumber = landNumber.Substring(AgricultureSettingWork.AgricultureLandNumberMedian);
                }
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureNumber + index, landNumber);//地块编码
                string actualAreaString = land.ActualArea > 0.0 ? ToolMath.SetNumbericFormat(InitalizeArea(land.ActualArea).ToString(), 2) : "";
                actualAreaString = land.ActualArea == 0.0 ? AgricultureSettingWork.InitalizeAreaString() : actualAreaString;
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureActualArea + index, actualAreaString);//实测面积
                string awareAreaString = land.AwareArea > 0.0 ? ToolMath.SetNumbericFormat(InitalizeArea(land.AwareArea).ToString(), 2) : "";
                awareAreaString = land.AwareArea == 0.0 ? AgricultureSettingWork.InitalizeAreaString() : awareAreaString;
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureAwareArea + index, awareAreaString);//确权面积
                string tableAreaString = (land.TableArea != null && land.TableArea.HasValue) ? (land.TableArea.Value > 0.0 ? ToolMath.SetNumbericFormat(InitalizeArea(land.TableArea.Value).ToString(), 2) : "") : "";
                tableAreaString = (land.TableArea != null && land.TableArea.HasValue && land.TableArea.Value == 0.0) ? AgricultureSettingWork.InitalizeAreaString() : tableAreaString;
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureTableArea + index, tableAreaString);//台帐面积
                string modoAreaString = (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue) ? (land.MotorizeLandArea.Value > 0.0 ? ToolMath.SetNumbericFormat(InitalizeArea(land.TableArea.Value).ToString(), 2) : "") : "";
                modoAreaString = (land.MotorizeLandArea != null && land.MotorizeLandArea.HasValue && land.MotorizeLandArea.Value == 0.0) ? AgricultureSettingWork.InitalizeAreaString() : modoAreaString;
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureModoArea + index, modoAreaString);//地块机动地面积
                InitalizeSmallNumber(index, ContractLand.GetLandNumber(land.CadastralNumber));
                string levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? "" : EnumNameAttribute.GetDescription(land.LandLevel);
                levelString = levelString == "未知" ? "" : levelString;
                levelString = AgricultureSettingWork.UseSystemLandLevelDescription ? levelString : InitalizeLandLevel(land.LandLevel);
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandLevel + index, levelString);//等级
                string landName = !string.IsNullOrEmpty(land.LandName) ? land.LandName : "";
                if (string.IsNullOrEmpty(landName) && DictList != null)
                {
                    Dictionary lt = DictList.Find(ld => ld.Code == land.LandCode);
                    landName = lt != null ? lt.Name : "";
                }
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandType + index, landName == "未知" ? "" : landName);//地类
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureIsFarmarLand + index, (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue) ? "" : (land.IsFarmerLand.Value ? "是" : "否"));//是否基本农田
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureEast + index, land.NeighborEast);//东
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureEastName + index, "东:" + land.NeighborEast);//东
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureSouth + index, land.NeighborSouth);//南
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureSouthName + index, "南:" + land.NeighborSouth);//南
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureWest + index, land.NeighborWest);//西
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureWestName + index, "西:" + land.NeighborWest);//西
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureNorth + index, land.NeighborNorth);//北
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureNorthName + index, "北:" + land.NeighborNorth);//北
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureNeighbor + index, SystemSet.NergionbourSet ? InitalizeLandNeightor(land) : "见附图");//四至
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureNeighborFigure + index, "见附图");//四至见附图
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureComment + index, land.Comment);//地块备注
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureConstructMode + index, EnumNameAttribute.GetDescription(land.ConstructMode));//承包方式
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureConstractType + index, EnumNameAttribute.GetDescription(land.LandCategory));//地块类别
                SetBookmarkValue(AgricultureBookMarkWork.AgriculturePlotNumber + index, land.PlotNumber);//地块畦数
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureManagerType + index, EnumNameAttribute.GetDescription(land.ManagementType));//地块经营方式
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureSourceFamilyName + index, land.FormerPerson);//原户主姓名
                string plantType = EnumNameAttribute.GetDescription(land.PlantType);
                SetBookmarkValue(AgricultureBookMarkWork.AgriculturePlantType + index, plantType == "未知" ? "" : plantType);//耕保类型
                if (land.IsTransfer)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.AgricultureTransterMode + index, EnumNameAttribute.GetDescription(land.TransferType));//流转方式
                    SetBookmarkValue(AgricultureBookMarkWork.AgricultureTransterTerm + index, land.TransferTime);//流转期限
                    SetBookmarkValue(AgricultureBookMarkWork.AgricultureTransterArea + index, land.PertainToArea > 0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : "");//流转面积
                }
                string platType = EnumNameAttribute.GetDescription(land.PlatType);
                SetBookmarkValue(AgricultureBookMarkWork.AgriculturePlatType + index, platType == "未知" ? "" : platType);//种植类型
                string landPurpose = EnumNameAttribute.GetDescription(land.Purpose);
                SetBookmarkValue(AgricultureBookMarkWork.AgriculturePurpose + index, (string.IsNullOrEmpty(landPurpose) || ToolMath.MatchEntiretyNumber(landPurpose)) ? "种植业" : landPurpose);//土地用途
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureUseSituation + index, expand.UseSituation);//土地利用情况
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureYield + index, expand.Yield);//土地产量情况
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOutputValue + index, expand.OutputValue);//土地产值情况
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureIncomeSituation + index, expand.IncomeSituation);//土地收益情况
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureElevation + index, expand.Elevation.ToString());//高程
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureSurveyPerson + index, expand.SurveyPerson);//地块调查员
                if (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.AgricultureSurveyDate + index, ToolDateTime.GetLongDateString(expand.SurveyDate.Value));//地块调查日期
                }
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureSurveyChronicle + index, expand.SurveyChronicle);//地块调查记事
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureCheckPerson + index, expand.CheckPerson);//地块审核员
                if (expand.CheckDate != null && expand.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.AgricultureCheckDate + index, ToolDateTime.GetLongDateString(expand.CheckDate.Value));//地块审核日期
                }
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureCheckOpinion + index, expand.CheckOpinion);//地块审核意见
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureImageNumber + index, expand.ImageNumber);//地块图幅号
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureFefer + index, expand.ReferPerson);//地块指界人
                index++;
            }
            WriteLandCalInformation();
            WriteReclamationInformation();
        }

        /// <summary>
        /// 初始化四至
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        private string InitalizeLandNeightor(ContractLand land)
        {
            string neighbor = string.Format("东：{0}\n南：{1}\n西：{2} \n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("东：{0}\n西：{1}\n南：{2} \n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
            }
            return neighbor;
        }

        /// <summary>
        /// 初始化角码
        /// </summary>
        private void InitalizeSmallNumber(int order, string landNumber)
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
            SetBookmarkValue(AgricultureBookMarkWork.AgricultureSmallNumber + order, number);
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
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
        private void WriteLandCalInformation()
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
            for (int j = 0; j < BookMarkCount; j++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureCount + (j == 0 ? "" : j.ToString()), LandCollection.Count < 1 ? "  " : LandCollection.Count.ToString());//地块总数
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureContractLandCount + (j == 0 ? "" : j.ToString()), (lands != null && lands.Count > 0) ? lands.Count.ToString() : "");//承包地块总数
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOtherCount + (j == 0 ? "" : j.ToString()), (otherLands != null && otherLands.Count > 0) ? otherLands.Count.ToString() : AgricultureSettingWork.InitalizeAreaString());//非承包地块总数
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureActualAreaCount + (j == 0 ? "" : j.ToString()), actualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(actualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总实测面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureAwareAreaCount + (j == 0 ? "" : j.ToString()), awareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(awareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总确权面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureTableAreaCount + (j == 0 ? "" : j.ToString()), tableArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(tableArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureModoAreaCount + (j == 0 ? "" : j.ToString()), modoArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(modoArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总机动地面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureContractLandActualAreaCount + (j == 0 ? "" : j.ToString()), conLandActualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(conLandActualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总实测面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureContractLandAwareAreaCount + (j == 0 ? "" : j.ToString()), conLandawareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(conLandawareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总确权面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureContractLandTableAreaCount + (j == 0 ? "" : j.ToString()), conLandtableArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(conLandtableArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureContractLandModoAreaCount + (j == 0 ? "" : j.ToString()), conLandmodoArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(conLandmodoArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总机动地面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOtherLandActualAreaCount + (j == 0 ? "" : j.ToString()), othLandActualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(othLandActualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总实测面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOtherLandAwareAreaCount + (j == 0 ? "" : j.ToString()), othLandawareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(othLandawareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总确权面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOtherLandTableAreaCount + (j == 0 ? "" : j.ToString()), othLandtableArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(othLandtableArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总二轮台账面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureOtherLandModoAreaCount + (j == 0 ? "" : j.ToString()), othLandmodoArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(othLandmodoArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//地块总机动地面积
            }
        }

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        private void WriteReclamationInformation()
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
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureReclationTableArea + (i == 0 ? "" : i.ToString()), reclamationTableArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(reclamationTableArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//台帐面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureRetainTableArea + (i == 0 ? "" : i.ToString()), retainTableArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(retainTableArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//台帐面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureReclationActualArea + (i == 0 ? "" : i.ToString()), reclamationActualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(reclamationActualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//实测面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureRetainActualArea + (i == 0 ? "" : i.ToString()), retainActualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(retainActualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//实测面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureReclationAwareArea + (i == 0 ? "" : i.ToString()), reclamationAwareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(reclamationAwareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//确权面积
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureRetainAwareArea + (i == 0 ? "" : i.ToString()), retainAwareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(retainAwareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//确权面积
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

        #endregion

        #region Sender

        /// <summary>
        /// 书写发包方信息
        /// </summary>
        private void WriteSenderInformation()
        {
            if (Tissue == null)
            {
                return;
            }
            string senderNameExpress = InitalizeSenderExpress();//发包方名称扩展如(第一村民小组)。
            string townName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_TOWN_LENGTH);
            string villageName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_VILLAGE_LENGTH);
            string groupName = InitalizeZoneName(Tissue.ZoneCode, Zone.ZONE_GROUP_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.SenderName + (i == 0 ? "" : i.ToString()), Tissue.Name);//发包方名称
                SetBookmarkValue(AgricultureBookMarkWork.SenderNameExpress + (i == 0 ? "" : i.ToString()), senderNameExpress);//发包方名称扩展如(第一村民小组)
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerName + (i == 0 ? "" : i.ToString()), Tissue.LawyerName);//发包方法人名称
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerTelephone + (i == 0 ? "" : i.ToString()), Tissue.LawyerTelephone);//发包方法人联系方式
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerAddress + (i == 0 ? "" : i.ToString()), Tissue.LawyerAddress);//发包方法人地址
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerPostNumber + (i == 0 ? "" : i.ToString()), Tissue.LawyerPosterNumber);//发包方法人邮政编码
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerCredentType + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(Tissue.LawyerCredentType));//发包方法人证件类型
                SetBookmarkValue(AgricultureBookMarkWork.SenderLawyerCredentNumber + (i == 0 ? "" : i.ToString()), Tissue.LawyerCartNumber);//发包方法人证件号码
                SetBookmarkValue(AgricultureBookMarkWork.SenderCode + (i == 0 ? "" : i.ToString()), Tissue.Code);//发包方代码
                SetBookmarkValue(AgricultureBookMarkWork.SenderTownName + (i == 0 ? "" : i.ToString()), townName);//发包方到镇
                SetBookmarkValue(AgricultureBookMarkWork.SenderVillageName + (i == 0 ? "" : i.ToString()), villageName);//发包方到村
                SetBookmarkValue(AgricultureBookMarkWork.SenderGroupName + (i == 0 ? "" : i.ToString()), groupName);//发包方到组
                SetBookmarkValue(AgricultureBookMarkWork.SenderSurveyChronicle + (i == 0 ? "" : i.ToString()), Tissue.SurveyChronicle);//调查记事
                SetBookmarkValue(AgricultureBookMarkWork.SenderSurveyPerson + (i == 0 ? "" : i.ToString()), Tissue.SurveyPerson);//调查员
                if (Tissue.SurveyDate != null && Tissue.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.SenderSurveyDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Tissue.SurveyDate.Value));//调查日期
                }
                SetBookmarkValue(AgricultureBookMarkWork.SenderCheckPerson + (i == 0 ? "" : i.ToString()), Tissue.CheckPerson);//审核员
                if (Tissue.CheckDate != null && Tissue.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.SenderCheckDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Tissue.CheckDate.Value));//审核日期
                }
                SetBookmarkValue(AgricultureBookMarkWork.SenderChenkOpinion + (i == 0 ? "" : i.ToString()), Tissue.CheckOpinion);//审核意见
            }
        }

        #endregion

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        private void WriteConcordInformation()
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
            string landPurpose = EnumNameAttribute.GetDescription(Concord.LandPurpose);
            if (!string.IsNullOrEmpty(landPurpose) && ToolMath.MatchEntiretyNumber(landPurpose))
            {
                landPurpose = "种植业";
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.ConcordNumber + (i == 0 ? "" : i.ToString()), Concord.ConcordNumber);//合同编号
                SetBookmarkValue(AgricultureBookMarkWork.ConcordTrem + (i == 0 ? "" : i.ToString()), Concord.ManagementTime);//合同期限
                SetBookmarkValue(AgricultureBookMarkWork.ConcordMode + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(Concord.ArableLandType));//合同承包方式
                SetBookmarkValue(AgricultureBookMarkWork.ConcordPurpose + (i == 0 ? "" : i.ToString()), landPurpose);//合同土地用途
                SetBookmarkValue(AgricultureBookMarkWork.ConcordLandCount + (i == 0 ? "" : i.ToString()), landCount.ToString());//合同中地块总数
                SetBookmarkValue(AgricultureBookMarkWork.ConcordActualAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountActualArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(Concord.CountActualArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//合同总实测面积
                SetBookmarkValue(AgricultureBookMarkWork.ConcordAwareAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountAwareArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(Concord.CountAwareArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//合同总确权面积
                SetBookmarkValue(AgricultureBookMarkWork.ConcordTableAreaCount + (i == 0 ? "" : i.ToString()), (Concord.TotalTableArea != null && Concord.TotalTableArea.HasValue && Concord.TotalTableArea.Value > 0) ? ToolMath.SetNumbericFormat(InitalizeArea(Concord.TotalTableArea.Value).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//合同块总二轮台账面积
                SetBookmarkValue(AgricultureBookMarkWork.ConcordModoAreaCount + (i == 0 ? "" : i.ToString()), Concord.CountMotorizeLandArea > 0 ? ToolMath.SetNumbericFormat(InitalizeArea(Concord.CountMotorizeLandArea).ToString(), 2) : AgricultureSettingWork.InitalizeAreaString());//合同总机动地面积
                SetBookmarkValue(AgricultureBookMarkWork.ConcordAddress + (i == 0 ? "" : i.ToString()), Concord.SecondContracterLocated);//合同中承包方地址

            }
            WriteConcordModeInformation();
            WriteConcordStartAndEndTime();
        }

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteConcordStartAndEndTime()
        {
            if (Concord == null)
            {
                return;
            }
            string startYear = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue) ? "" : Concord.ArableLandStartTime.Value.Year.ToString();
            string startMonth = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue) ? "" : Concord.ArableLandStartTime.Value.Month.ToString();
            string startDay = (Concord.ArableLandStartTime == null || !Concord.ArableLandStartTime.HasValue) ? "" : Concord.ArableLandStartTime.Value.Day.ToString();
            string endYear = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue) ? "" : Concord.ArableLandEndTime.Value.Year.ToString();
            string endMonth = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue) ? "" : Concord.ArableLandEndTime.Value.Month.ToString();
            string endDay = (Concord.ArableLandEndTime == null || !Concord.ArableLandEndTime.HasValue) ? "" : Concord.ArableLandEndTime.Value.Day.ToString();
            string date = "";
            if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue && Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
            {
                date = "自" + string.Format("{0}年{1}月{2}日", Concord.ArableLandStartTime.Value.Year, Concord.ArableLandStartTime.Value.Month, Concord.ArableLandStartTime.Value.Day) + "起至"
                              + string.Format("{0}年{1}月{2}日", Concord.ArableLandEndTime.Value.Year, Concord.ArableLandEndTime.Value.Month, Concord.ArableLandEndTime.Value.Day) + "止";
            }
            for (int i = 0; i < BookMarkCount; i++)
            {
                if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.ConcordStartDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Concord.ArableLandStartTime.Value));//起始时间
                }
                SetBookmarkValue(AgricultureBookMarkWork.ConcordStartYearDate + (i == 0 ? "" : i.ToString()), startYear);//起始时间-年
                SetBookmarkValue(AgricultureBookMarkWork.ConcordStartMonthDate + (i == 0 ? "" : i.ToString()), startMonth);//起始时间-月
                SetBookmarkValue(AgricultureBookMarkWork.ConcordStartDayDate + (i == 0 ? "" : i.ToString()), startDay);//起始时间-日
                if (Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.ConcordEndDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(Concord.ArableLandEndTime.Value));//结束时间
                }
                SetBookmarkValue(AgricultureBookMarkWork.ConcordEndYearDate + (i == 0 ? "" : i.ToString()), endYear);//起始时间-年
                SetBookmarkValue(AgricultureBookMarkWork.ConcordEndMonthDate + (i == 0 ? "" : i.ToString()), endMonth);//起始时间-月
                SetBookmarkValue(AgricultureBookMarkWork.ConcordEndDayDate + (i == 0 ? "" : i.ToString()), endDay);//起始时间-日
                SetBookmarkValue(AgricultureBookMarkWork.ConcordDate + (i == 0 ? "" : i.ToString()), date);//承包时间
                SetBookmarkValue(AgricultureBookMarkWork.ConcordTrem + (i == 0 ? "" : i.ToString()), Concord.Flag ? "长久" : Concord.ManagementTime);//合同期限
                SetBookmarkValue(AgricultureBookMarkWork.ConcordLongTime + (i == 0 ? "" : i.ToString()), "长久");//合同中长久日期
            }
        }

        #endregion

        #region Book

        /// <summary>
        /// 书写权证信息
        /// </summary>
        private void WriteBookInformation()
        {
            if (Book == null || string.IsNullOrEmpty(Book.RegeditNumber))
            {
                return;
            }
            string number = (Book != null && AgriculturePrintSettingWork.BookNumberPrintMedian > 0 && AgriculturePrintSettingWork.BookNumberPrintMedian <= Book.RegeditNumber.Length) ? Book.RegeditNumber.Substring(0, AgriculturePrintSettingWork.BookNumberPrintMedian) : Book.RegeditNumber.Substring(0, Book.RegeditNumber.Length - 1);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.BookNumber + (i == 0 ? "" : i.ToString()), number);//编号
                SetBookmarkValue(AgricultureBookMarkWork.BookOrganName + (i == 0 ? "" : i.ToString()), Book.SendOrganization);//发证机关名称
                SetBookmarkValue(AgricultureBookMarkWork.BookYear + (i == 0 ? "" : i.ToString()), Book.Year);//年号
                SetBookmarkValue(AgricultureBookMarkWork.BookWarrantNumber + (i == 0 ? "" : i.ToString()),Book.RegeditNumber);//权证编号
                SetBookmarkValue(AgricultureBookMarkWork.BookAllNumber + (i == 0 ? "" : i.ToString()), Book.SendOrganization + "农地承包权(" + Book.Year + ")第" + Book.RegeditNumber + "号");
                SetBookmarkValue(AgricultureBookMarkWork.BookSerialNumber + (i == 0 ? "" : i.ToString()), Book.Number);//六位流水号
                SetBookmarkValue(AgricultureBookMarkWork.BookFullSerialNumber + (i == 0 ? "" : i.ToString()), Book.SendOrganization + "农地承包权(" + Book.Year + ")第" + Book.RegeditNumber + "号");//所有权证编号(包括发证机关、年号、流水号)
            }
            WriteAwareDateInformation();
            WriteSenderDateInformation();
        }

        /// <summary>
        /// 填写颁证日期信息
        /// </summary>
        private void WriteAwareDateInformation()
        {
            string year = Book.PrintDate != null ? Book.PrintDate.Year.ToString() : "";
            string awareYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.ToString()) : year;
            string oneYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(2, 1)) : "";
            string twoYear = !string.IsNullOrEmpty(year) ? ToolMath.GetChineseLowNimeric(year.Substring(3, 1)) : "";
            string month = Book.PrintDate != null ? Book.PrintDate.Month.ToString() : "";
            string awareMonth = !string.IsNullOrEmpty(month) ? ToolMath.GetChineseLowNumber(month.ToString()) : month;
            if (awareMonth.Equals("一十"))
            {
                awareMonth = "十";
            }
            string day = Book.PrintDate != null ? Book.PrintDate.Day.ToString() : "";
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
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareYear + (i == 0 ? "" : i.ToString()), awareYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareFirstYear + (i == 0 ? "" : i.ToString()), firstYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareSecondYear + (i == 0 ? "" : i.ToString()), secondYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareThreeYear + (i == 0 ? "" : i.ToString()), threeYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareFourYear + (i == 0 ? "" : i.ToString()), fourYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareShortYear + (i == 0 ? "" : i.ToString()), shortYear);//打印日期到年后2位
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareOneYear + (i == 0 ? "" : i.ToString()), oneYear);//打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareLastYear + (i == 0 ? "" : i.ToString()), twoYear);//打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareMonth + (i == 0 ? "" : i.ToString()), awareMonth);//打印日期到月
                SetBookmarkValue(AgricultureBookMarkWork.BookAwareDay + (i == 0 ? "" : i.ToString()), awareday);//打印日期到日
                SetBookmarkValue(AgricultureBookMarkWork.BookAllAwareDate + (i == 0 ? "" : i.ToString()), allAwareString);//打印所有颁证日期
            }
        }

        /// <summary>
        /// 填写填证日期信息
        /// </summary>
        private void WriteSenderDateInformation()
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
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteYear + (i == 0 ? "" : i.ToString()), awareYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteFirstYear + (i == 0 ? "" : i.ToString()), firstYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteSecondYear + (i == 0 ? "" : i.ToString()), secondYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteThreeYear + (i == 0 ? "" : i.ToString()), threeYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteFourYear + (i == 0 ? "" : i.ToString()), fourYear);//打印日期到年
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteShortYear + (i == 0 ? "" : i.ToString()), shortYear);//打印日期到年后2位
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteOneYear + (i == 0 ? "" : i.ToString()), oneYear);//打印日期到年倒数第二位
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteLastYear + (i == 0 ? "" : i.ToString()), twoYear);//打印日期到年最后一位
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteMonth + (i == 0 ? "" : i.ToString()), awareMonth);//打印日期到月
                SetBookmarkValue(AgricultureBookMarkWork.BookWriteDay + (i == 0 ? "" : i.ToString()), awareday);//打印日期到日
                SetBookmarkValue(AgricultureBookMarkWork.BookAllWriteDate + (i == 0 ? "" : i.ToString()), allAwareString);//打印所有颁证日期
            }
        }

        #endregion

        #region Zone
        /// <summary>
        /// 创建发包方扩展
        /// </summary>
        /// <returns></returns>
        private string InitalizeSenderExpress()
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
        private void WriteZoneInformation()
        {
            if (CurrentZone == null)
            {
                return;
            }
            string countyCode = CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH);
            Zone county = CurrentZone;
            string unitName = county != null ? CurrentZone.FullName.Replace(county.FullName, "") : CurrentZone.FullName;
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.ZoneName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMarkWork.LocationName + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);
                SetBookmarkValue(AgricultureBookMarkWork.TownUnitName + (i == 0 ? "" : i.ToString()), unitName);
                SetBookmarkValue(AgricultureBookMarkWork.CountyUnitName + (i == 0 ? "" : i.ToString()), county != null ? (county.Name + unitName) : unitName);
            }
            string zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_COUNTY_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.CountyName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMarkWork.SmallCountyName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_PROVICE_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.ProviceName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMarkWork.SmallProviceName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");

            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_CITY_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.CityName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMarkWork.SmallCityName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_TOWN_LENGTH);
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.TownName + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue(AgricultureBookMarkWork.SmallTownName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            if (CurrentZone.Level >= eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_VILLAGE_LENGTH);
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.VillageName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMarkWork.SmallVillageName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", "") : "");
                    SetBookmarkValue(AgricultureBookMarkWork.VillageUnitName + (i == 0 ? "" : i.ToString()), zoneName + CurrentZone.Name);
                }
            }
            if (CurrentZone.Level == eZoneLevel.Group)
            {
                zoneName = InitalizeZoneName(CurrentZone.FullCode, Zone.ZONE_GROUP_LENGTH);
                string number = ToolString.GetLeftNumberWithInString(zoneName);
                string groupName = string.IsNullOrEmpty(number) ? zoneName : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
                string smallGroup = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
                for (int i = 0; i < BookMarkCount; i++)
                {
                    SetBookmarkValue(AgricultureBookMarkWork.GroupName + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue(AgricultureBookMarkWork.ChineseGroupName + (i == 0 ? "" : i.ToString()), groupName);
                    SetBookmarkValue(AgricultureBookMarkWork.SmallChineseGroupName + (i == 0 ? "" : i.ToString()), smallGroup);
                    SetBookmarkValue(AgricultureBookMarkWork.SmallGroupName + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(zoneName) ? zoneName.Substring(0, zoneName.Length - 1) : "");
                }
            }
        }

        #endregion

        #region DateTime

        /// <summary>
        /// 填写日期扩展信息
        /// </summary>
        private void WriteDateTimeInformation()
        {
            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue(AgricultureBookMarkWork.NowYear + (i == 0 ? "" : i.ToString()), DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.YearName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMarkWork.CheckYearName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Year.ToString() : "");
                SetBookmarkValue(AgricultureBookMarkWork.NowMonth + (i == 0 ? "" : i.ToString()), DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.MonthName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMarkWork.CheckMonthName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Month.ToString() : "");
                SetBookmarkValue(AgricultureBookMarkWork.NowDay + (i == 0 ? "" : i.ToString()), DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.DayName + (i == 0 ? "" : i.ToString()), (DateValue != null && DateValue.HasValue) ? DateValue.Value.Day.ToString() : "");
                SetBookmarkValue(AgricultureBookMarkWork.CheckDayName + (i == 0 ? "" : i.ToString()), (DateChecked != null && DateChecked.HasValue) ? DateChecked.Value.Day.ToString() : "");
                //SetBookmarkValue(AgricultureBookMarkWork.FullDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString(DateTime.Now));
                SetBookmarkValue(AgricultureBookMarkWork.FullDate + (i == 0 ? "" : i.ToString()), ToolDateTime.GetLongDateString((DateTime)DateValue));
                string year = ToolMath.GetChineseLowNumber(DateTime.Now.Year.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.ChineseYearName + (i == 0 ? "" : i.ToString()), year);
                string month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.ChineseMonthName + (i == 0 ? "" : i.ToString()), month);
                string day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue(AgricultureBookMarkWork.ChineseDayName + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue(AgricultureBookMarkWork.FullChineseDate + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        #endregion

        #region Parcel

        /// <summary>
        /// 书写地块示意图信息
        /// </summary>
        private void WriteParcelInformation()
        {
            string drawPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSettingWork.DRAWPERSON, "");
            string checkPerson = ToolConfiguration.GetSpecialAppSettingValue(AgricultureSettingWork.VERIFYPERSON, "");
            if (!string.IsNullOrEmpty(drawPerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandDrawPerson, drawPerson);//制图人
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
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandDrawDate, ToolDateTime.GetLongDateString(date));//制图日期
            }
            if (!string.IsNullOrEmpty(checkPerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandCheckPerson, checkPerson);//审核人
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
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandCheckDate, ToolDateTime.GetLongDateString(date));//审核日期
            }
        }

        #endregion

        #region Other

        /// <summary>
        /// 书写其他信息
        /// </summary>
        private void WriteOtherInformation()
        {
            if (!string.IsNullOrEmpty(ParcelDrawPerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandDrawPerson, ParcelDrawPerson);//制图人
            }
            if (ParcelDrawDate != null && ParcelDrawDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandDrawDate, ToolDateTime.GetLongDateString(ParcelDrawDate.Value));//制图日期
            }
            if (!string.IsNullOrEmpty(ParcelCheckPerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandCheckPerson, ParcelCheckPerson);//审核人
            }
            if (ParcelCheckDate != null && ParcelCheckDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureLandCheckDate, ToolDateTime.GetLongDateString(ParcelCheckDate.Value));//审核日期
            }
            if (!string.IsNullOrEmpty(DrawTablePerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureDrawTablePerson, DrawTablePerson);//制表人
            }
            if (DrawTableDate != null && DrawTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureDrawTableDate, ToolDateTime.GetLongDateString(DrawTableDate.Value));//制表日期
            }
            if (!string.IsNullOrEmpty(CheckTablePerson))
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureCheckTablePerson, CheckTablePerson);//检查人
            }
            if (CheckTableDate != null && CheckTableDate.HasValue)
            {
                SetBookmarkValue(AgricultureBookMarkWork.AgricultureCheckTableDate, ToolDateTime.GetLongDateString(CheckTableDate.Value));//制表日期
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// 初始化地域名称
        /// </summary>
        private string InitalizeZoneName(string zoneCode, int length)
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
                return "/";
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

        /// <summary>
        /// 初始化面积
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private double InitalizeArea(double area, int dicimal = 4)
        {
            double ae = Math.Round(area, dicimal);
            return ae;
        }

        /// <summary>
        /// 初始化面积
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private string InitalizeAreaString(string area, int dicimal = 4)
        {
            if (string.IsNullOrEmpty(area))
            {
                return area;
            }
            double ae = 0.0;
            double.TryParse(area, out ae);
            ae = InitalizeArea(ae, dicimal);
            return ae.ToString();
        }

        #endregion

        #endregion
    }
}
