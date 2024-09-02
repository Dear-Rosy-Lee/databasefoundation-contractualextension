/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 预览单户申请表
    /// </summary>
    public class ExportLandRequireBook : AgricultureWordBook
    {
        #region Fields

        private VirtualPerson currentFamily;
        private ContractConcord concord;
        private List<Person> persons;
        private List<ContractLand> lands;
        private List<ContractConcord> concords;

        private bool useActualArea;

        #endregion

        #region Properties

        /// <summary>
        /// 承包合同集合
        /// </summary>
        public List<ContractConcord> Concords
        {
            get { return concords; }
            set { concords = value; }
        }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> Lands
        {
            get { return lands; }
            set { lands = value; }
        }

        /// <summary>
        /// 根据合同ID获取地块
        /// </summary>
        public List<ContractLand> ConcordLands
        { get; set; }

        /// <summary>
        /// 共有人集合
        /// </summary>
        public List<Person> Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        /// <summary>
        /// 地域乡
        /// </summary>
        public Zone CurrentZoneCounty { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZoneProvince { get; set; }

        /// <summary>
        /// 地域市
        /// </summary>
        public Zone CurrentZoneCity { get; set; }

        /// <summary>
        /// 地域镇
        /// </summary>
        public Zone CurrentZoneTown { get; set; }

        /// <summary>
        /// 地域组
        /// </summary>
        public Zone CurrentZoneGroup { get; set; }

        /// <summary>
        /// 地域村
        /// </summary>
        public Zone CurrentZoneVillage { get; set; }

        /// <summary>
        /// 打印模式（不输出承包方，让现实承包方手填）
        /// </summary>
        public bool IsPrint { get; set; }

        public DateTime? RequireDate { get; set; }//申请日期

        public DateTime? CheckDate { get; set; }//审核日期

        public eConstructMode ConstructMode { get; set; }//土地承包方式

        /// <summary>
        /// 当前家庭
        /// </summary>
        public VirtualPerson CurrentFamily
        {
            get { return currentFamily; }
            set
            {
                if(currentFamily != value)
                {
                    currentFamily = value;
                }
            }
        }

        #endregion

        #region Ctor

        public ExportLandRequireBook(VirtualPerson family)
        {
            if (family == null || family == currentFamily)
            {
                return;
            }
            currentFamily = family;
            List<Person> spersons = new List<Person>();//得到户对应的共有人
            spersons = currentFamily.SharePersonList;
            persons = SortSharePerson(spersons, currentFamily.Name); //排序共有人，并返回人口集合
            spersons.Clear();
            useActualArea = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
            Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积
            base.TemplateName = "单户登记申请书";
            base.Tags = new object[1];
            base.Tags[0] = family;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 初始化合同信息
        /// </summary>
        public ContractConcord InitalizeConcord(ContractConcord concordNow)
        {
            if (currentFamily == null)
            {
                concordNow = null;
                return concordNow;
            }
            //承包合同  
            if (concords == null || concords.Count == 0)
            {
                concordNow = null;
                return concordNow;
            }
            if (ConstructMode == eConstructMode.Family)
            {
                string code = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
                concord = concords.Find(cd => cd.ArableLandType == code);
            }
            else
            {
                string code = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
                concord = concords.Find(cd => cd.ArableLandType != code);
            }

            // 一旦清除掉，下次再调用此函数，就取不到合同信息了
            // concords.Clear();
            concordNow = concord;
            GC.Collect();
            return concordNow;
        }

        #endregion

        #region Priavte

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns></returns>
        private string GetGender()
        {
            string value = EnumNameAttribute.GetDescription(persons[0].Gender);
            string sex = value == EnumNameAttribute.GetDescription(eGender.Unknow) ? "" : value;
            return " " + sex + " ";
        }

        private string AddEmpty(string str, int Emptycount, string[] parms)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            str = GetName(str, parms);
            for (int i = 0; i < Emptycount - str.Length; i++)
            {
                if (i % 2 == 0)
                    str = "" + str;
                else
                    str = str + "";
            }
            return str;
        }

        private string AddEmpty(string str, int Emptycount)
        {
            if (string.IsNullOrEmpty(str))
                str = "";
            for (int i = 0; i < Emptycount - str.Length; i++)
            {
                if (i % 2 == 0)
                    str = "" + str;
                else
                    str = str + "";
            }
            return str;
        }

        private string GetName(string str, string[] parms)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string temp = str.Substring(str.Length - 1);
                foreach (string item in parms)
                {
                    if (temp == item)
                    {
                        return str.Substring(0, str.Length - 1);
                    }
                }
            }
            return str;
        }

        #endregion

        #region Override

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <returns></returns>
        public bool CheckLandValue()
        {
            bool isRight = false;
            if (lands == null)  //根据当前承包方获取地块集合
            {
                return false;
            }
            // string code = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
            foreach (ContractLand land in lands)
            {
                if (land.ConstructMode == ((int)ConstructMode).ToString())
                {
                    isRight = true;
                    break;
                }
            }
            return isRight;
        }

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            InitalizeDataInformation();
            LandCollection = concord != null ? ConcordLands : Lands;
            //LandCollection = concord != null ? LandCollection : SpliteLandCollection(LandCollection);
            base.OnSetParamValue(data);
            //Zone countyZone = this.CurrentZoneCounty;    //(currentFamily.SenderCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));   //得到村、镇、组地域
            //Zone townZone = this.CurrentZoneTown;       //(currentFamily.SenderCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
            //Zone zone = this.CurrentZoneVillage;         //currentFamily.SenderCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
            //Zone groupZone = this.CurrentZoneGroup;     //DB.Zone.Get(currentFamily.SenderCode);
            double landArea = 0.0;
            foreach (ContractLand land in LandCollection)
            {
                landArea += useActualArea ? land.ActualArea : land.AwareArea;
            }
            SetBookmarkValue(AgricultureBookMark.SenderName, concord != null ? concord.SenderName : "");
            CollectivityTissue tissue = GetSenderById(concord.SenderId);
            SetBookmarkValue(AgricultureBookMark.SenderLawyerName, tissue != null ? tissue.LawyerName : "");
            SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone, tissue != null ? tissue.LawyerTelephone : "");
            SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber, tissue != null ? tissue.LawyerCartNumber : "");
            for (int i = 0; i < 6; i++)
            {
                //string villageName = zone != null ? zone.Name.Substring(0, zone.Name.Length) : "";
                //SetBookmarkValue("bmNameVillage" + (i == 0 ? "" : i.ToString()), villageName);
                //string groupNamne = groupZone != null ? ToolString.GetLeftNumberWithInString(groupZone.Name) : "";
                //if (string.IsNullOrEmpty(groupNamne))
                //{
                //    groupNamne = ToolMath.GetChineseLowNumber(zone.Code);
                //}
                //SetBookmarkValue("bmNameGroup" + (i == 0 ? "" : i.ToString()), groupNamne);
                SetBookmarkValue("bmConcordNumber" + (i == 0 ? "" : i.ToString()), concord != null ? concord.ConcordNumber : "");
                SetBookmarkValue("bmLandCount" + (i == 0 ? "" : i.ToString()), LandCollection != null && LandCollection.Count > 0 ? LandCollection.Count.ToString() : "");
                SetBookmarkValue("bmLandArea" + (i == 0 ? "" : i.ToString()), landArea.ToString("F2"));
                //SetBookmarkValue("bmCountyName" + (i == 0 ? "" : i.ToString()), countyZone != null ? countyZone.Name : "");
                //SetBookmarkValue("bmNameTown" + (i == 0 ? "" : i.ToString()), townZone != null ? townZone.Name : "");
                SetBookmarkValue("bmContractMode" + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(ConstructMode));
            }
            WriteFamilyInformation();
            WriteDateInformation();
            WriteZoneExpressInformation();
            //countyZone = null;
            //townZone = null;
            //zone = null;
            //groupZone = null;
            concord = null;
            persons = null;
            base.Destroyed();
            GC.Collect();
            return true;
        }
        public CollectivityTissue GetSenderById(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = id;
            arg.Name = SenderMessage.SENDER_GET_ID;
            TheBns.Current.Message.Send(this, arg);
            CollectivityTissue tissue = arg.ReturnValue as CollectivityTissue;
            return tissue;
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeDataInformation()
        {
            if (currentFamily == null)
            {
                return;
            }

            Contractor = currentFamily.Clone() as VirtualPerson;
            if (Contractor == null)
            {
                return;
            }

            Concord = concord;
            // Tissue = Concord != null ? DataInstance.CollectivityTissue.Get(Concord.SenderId) : DataInstance.CollectivityTissue.Get(CurrentZone.ID);
            // Book = Concord != null ? DataInstance.ContractRegeditBook.Get(Concord.ID) : null;
            DateValue = RequireDate;
            DateChecked = CheckDate;
        }

        /// <summary>
        /// 分离地块信息
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SpliteLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            List<ContractLand> landCollection = new List<ContractLand>();
            string code = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
            string otherCode = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "其他").Code;
            foreach (ContractLand land in lands)
            {
                if (land.ActualArea == 0 && land.AwareArea == 0)
                {
                    continue;
                }
                if (land.ConstructMode == code)
                {
                    landCollection.Add(land);
                    continue;
                }

                if (ConstructMode == eConstructMode.Other || land.ConstructMode != code)
                {
                    landCollection.Add(land);
                }
            }
            return landCollection;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValue(int value)
        {
            if (value == 10)
            {
                return "十";
            }
            if (value == 20)
            {
                return "二十";
            }
            if (value == 30)
            {
                return "三十";
            }
            return string.Empty;
        }

        /// <summary>
        /// 填写行政区域信息
        /// </summary>
        private void WriteZoneExpressInformation()
        {
            //if (CurrentZone == null)   //当前地域
            //{
            //    return;
            //}
            //string zoneName = this.CurrentZoneCounty.Name;
            //SetBookmarkValue("bmCountryName", zoneName);
            //for (int i = 0; i < 6; i++)
            //{
            //    SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), zoneName);
            //    SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
            //    SetBookmarkValue("CityTownVillageGroupName" + (i == 0 ? "" : i.ToString()), this.CurrentZoneCounty.Name);

            //}
            //zoneName = this.CurrentZoneProvince.Name;
            //for (int i = 0; i < 6; i++)
            //{
            //    SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), zoneName);
            //    SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

            //}
            //zoneName = this.CurrentZoneCity.Name;
            //for (int i = 0; i < 6; i++)
            //{
            //    SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), zoneName);
            //    SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
            //}
            //zoneName = this.CurrentZoneTown.Name;
            //for (int i = 0; i < 6; i++)
            //{
            //    SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), zoneName);
            //    SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
            //    SetBookmarkValue("CountyTownVillageGroupName" + (i == 0 ? "" : i.ToString()), this.CurrentZoneCounty.Name + this.CurrentZoneTown.Name);

            //}
            //if (CurrentZone.Level >= eZoneLevel.Group)
            //{
            //    zoneName = this.CurrentZoneVillage.Name;
            //    for (int i = 0; i < 6; i++)
            //    {
            //        SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), zoneName);
            //        SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", ""));
            //        SetBookmarkValue("CountyTownVillageGroupName" + (i == 0 ? "" : i.ToString()), this.CurrentZoneCounty.Name + this.CurrentZoneTown.Name + this.CurrentZoneVillage.Name);
            //    }
            //}
            //if (CurrentZone.Level == eZoneLevel.Group)
            //{
            //    zoneName = this.CurrentZoneGroup.Name;
            //    for (int i = 0; i < 6; i++)
            //    {
            //        SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), zoneName);
            //        string number = ToolString.GetLeftNumberWithInString(zoneName);
            //        string groupName = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
            //        SetBookmarkValue("ZoneName" + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);//座落
            //        groupName = string.IsNullOrEmpty(number) ? zoneName : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
            //        SetBookmarkValue("GroupName" + (i == 0 ? "" : i.ToString()), groupName);
            //        SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

            //        SetBookmarkValue("CountyTownVillageGroupName" + (i == 0 ? "" : i.ToString()), this.CurrentZoneCounty.Name + this.CurrentZoneTown.Name + this.CurrentZoneVillage.Name + this.CurrentZoneGroup.Name);
            //    }
            //}
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        private void WriteFamilyInformation()
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("bmFamilyName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(currentFamily.Name));
                SetBookmarkValue("bmFamilyCount" + (i == 0 ? "" : i.ToString()), persons.Count.ToString());
                SetBookmarkValue("FamilyName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(currentFamily.Name));
                string gender = GetGender();
                SetBookmarkValue("Gender" + (i == 0 ? "" : i.ToString()), gender);
                string age = GetAge();
                SetBookmarkValue("Age" + (i == 0 ? "" : i.ToString()), age);
                SetBookmarkValue("IdentifyNumber" + (i == 0 ? "" : i.ToString()), currentFamily.Number);
                SetBookmarkValue("Comment" + (i == 0 ? "" : i.ToString()), currentFamily.Comment);
                string year = DateTime.Now.Year.ToString();
                SetBookmarkValue("NowYear" + (i == 0 ? "" : i.ToString()), year);
                string month = DateTime.Now.Month.ToString();
                SetBookmarkValue("NowMonth" + (i == 0 ? "" : i.ToString()), month);
                string day = DateTime.Now.Day.ToString();
                SetBookmarkValue("NowDay" + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullDate" + (i == 0 ? "" : i.ToString()), fullDate);
                year = ToolMath.GetChineseLowNimeric(DateTime.Now.Year.ToString());
                SetBookmarkValue("ChineseYear" + (i == 0 ? "" : i.ToString()), year);
                month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue("ChineseMonth" + (i == 0 ? "" : i.ToString()), month);
                day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue("ChineseDay" + (i == 0 ? "" : i.ToString()), day);
                fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullChineseDate" + (i == 0 ? "" : i.ToString()), fullDate);

                SetBookmarkValue("SecondConcordTotalArea" + (i == 0 ? "" : i.ToString()), currentFamily.FamilyExpand.SecondConcordTotalArea == 0 ? "/" : currentFamily.FamilyExpand.SecondConcordTotalArea.ToString("0.00"));
                SetBookmarkValue("SecondConcordTotalLandCount" + (i == 0 ? "" : i.ToString()), currentFamily.FamilyExpand.SecondConcordTotalLandCount == 0 ? "/" : currentFamily.FamilyExpand.SecondConcordTotalLandCount.ToString());

            }

            int personcount = 0;
            foreach (Person item in persons)
            {
                if (SystemSet.StatisticsDeadPersonInfo == false && item.Comment.IsNullOrEmpty()== false && item.Comment.Contains("去世"))
                {
                    continue;
                }
               
                personcount++;
            }          
            SetBookmarkValue("ContractorCount", personcount.ToString());
        }

        /// <summary>
        /// 填写日期信息
        /// </summary>
        private void WriteDateInformation()
        {
            int value = (RequireDate != null && RequireDate.HasValue) ? RequireDate.Value.Year : 0;
            string valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmYear1", valueString);
            value = (RequireDate != null && RequireDate.HasValue) ? RequireDate.Value.Month : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmMonth1", valueString);
            value = (RequireDate != null && RequireDate.HasValue) ? RequireDate.Value.Day : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmDay1", valueString);

            value = (CheckDate != null && CheckDate.HasValue) ? CheckDate.Value.Year : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmYear2", valueString);
            value = (CheckDate != null && CheckDate.HasValue) ? CheckDate.Value.Month : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmMonth2", valueString);
            value = (CheckDate != null && CheckDate.HasValue) ? CheckDate.Value.Day : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmDay2", valueString);

            value = (RequireDate != null && RequireDate.HasValue) ? RequireDate.Value.Year : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmYear3", valueString);
            value = (CheckDate != null && CheckDate.HasValue) ? CheckDate.Value.Month : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmMonth3", valueString);
            value = (CheckDate != null && CheckDate.HasValue) ? CheckDate.Value.Day : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmDay3", valueString);
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        private string GetAge()
        {
            if (persons.Count == 0)
            {
                return "";
            }
            Person person = persons[0].Clone() as Person;
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

        #endregion
    }
}
