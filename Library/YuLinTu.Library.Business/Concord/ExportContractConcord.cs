/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 预览(导出)合同处理类
    /// </summary>
    public class ExportContractConcord : AgricultureWordBook
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportContractConcord(IDbContext dbContext)
        {
            base.TemplateName = "承包合同";
            base.Tags = new object[1];
            base.Tags[0] = dbContext;

            db = dbContext;
            concordSetting = ContractConcordSettingDefine.GetIntence();
            senderStation = dbContext.CreateSenderWorkStation();
            zoneBusiness = new ZoneDataBusiness();
            zoneBusiness.DbContext = dbContext;
            zoneBusiness.Station = dbContext.CreateZoneWorkStation();
        }

        #endregion Ctor

        #region Fields

        protected readonly IDbContext db;
        protected readonly ISenderWorkStation senderStation;
        protected readonly ZoneDataBusiness zoneBusiness; //行政地域业务
        protected readonly ContractConcordSettingDefine concordSetting;

        protected VirtualPerson virtualPerson;//承包方
        protected ContractConcord concord; //承包合同
        protected SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        //private ContractRegeditBookBusiness bookBusiness;   //权证业务

        #endregion Fields

        #region Properties

        /// <summary>
        /// 指定合同下的地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 承包方数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 合同面积类型
        /// </summary>
        public int AreaType { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson VirtualPerson
        {
            get { return virtualPerson; }
            set { virtualPerson = value; }
        }

        #endregion Properties

        #region Methods

        #region Methods - Override

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null || db == null)
            {
                return false;
            }
            try
            {
                if (!CheckDataInformation(data))
                {
                    return false;
                }
                InitalizeDataInformation(data);

                base.OnSetParamValue(data);
                //WriteTitleInformation();
                WriteConcordInfo();
                Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        protected virtual void InitalizeDataInformation(object data)
        {
            Concord = concord.Clone() as ContractConcord;
            Contractor = virtualPerson.Clone() as VirtualPerson;
            CurrentZone = CurrentZone.Clone() as Zone;
            //Book = Concord != null ? bookBusiness.Get(Concord.ID) : null;  //修改获取权证方式
            //陈泽林 20161219
            Tissue = Concord != null ? senderStation.Get(Concord.SenderId) : null;

            if (Tissue == null)
            {
                Tissue = senderStation.Get(CurrentZone.ID);
            }
            if (Tissue == null)
            {
                Tissue = senderStation.GetTissues(CurrentZone.FullCode, eLevelOption.Self).FirstOrDefault();
            }
            LandCollection = ListLand.Clone() as List<ContractLand>;
            ZoneList = zoneBusiness.GetAllZone();
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            base.Destroyed();
            virtualPerson = null;
            CurrentZone = null;
            ListLand = null;
            GC.Collect();
        }

        #endregion Methods - Override

        #region Methods - Family

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        private void WitePersonInformaion()
        {
            List<Person> persons = virtualPerson.SharePersonList;    //得到户对应的共有人
            List<Person> sharePersons = SortSharePerson(persons, virtualPerson.Name); //排序共有人，并返回人口集合
            string name = "";
            foreach (var item in persons)
            {
                name += (item.Name == virtualPerson.Name) ? InitalizeFamilyName(item.Name) : item.Name;
                name += "、";
            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("ConcordNumber" + (i == 0 ? "" : i.ToString()), concord.ConcordNumber);//合同编号
                SetBookmarkValue("bmContractNumber" + (i == 0 ? "" : i.ToString()), concord.ConcordNumber);//合同编号
                SetBookmarkValue("PersonCount" + (i == 0 ? "" : i.ToString()), persons.Count < 1 ? "" : (persons.Count < 1 ? "" : persons.Count.ToString()));//共有人数量
                SetBookmarkValue("PersonList" + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(name) ? name.Substring(0, name.Length - 1) : "");//共有人
            }
            string alloctioonPerson = virtualPerson.FamilyExpand.AllocationPerson;
            if (alloctioonPerson == virtualPerson.PersonCount || alloctioonPerson == persons.Count.ToString())
            {
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("AlloctionPersonList" + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(name) ? name.Substring(0, name.Length - 1) : "");//共有人
                }
            }
            WriteSharePersonValue();
            persons.Clear();
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonValue()
        {
            List<Person> persons = virtualPerson.SharePersonList;    //得到户对应的共有人
            List<Person> sharePersons = SortSharePerson(persons, virtualPerson.Name); //排序共有人，并返回人口集合
            int rowCount = persons.Count;
            if (rowCount > 1)
            {
                InsertTableRow(1, 1, rowCount-1);
            }
            int tableIndex = 1;
            int startRow = 1;
            foreach (Person person in sharePersons)
            {
                SetTableCellValue(tableIndex, startRow, 0, person.Name);
                SetTableCellValue(tableIndex, startRow, 1, person.Relationship);
                SetTableCellValue(tableIndex, startRow, 2, person.ICN);
                SetTableCellValue(tableIndex, startRow, 3, person.Comment);
                startRow++;
            }
    
        }

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        protected virtual void WriteLandInfo()
        {
            int rowCount = ListLand.Count;
            if (rowCount > 1)
            {
                InsertTableRow(0, 2, rowCount-1);
            }
            var totleActualArea = 0.00;
            int tableIndex = 0;
            int startRow = 2;
            foreach (var item in ListLand)
            {
                SetTableCellValue(tableIndex, startRow, 0, item.Name);
                SetTableCellValue(tableIndex, startRow, 1, item.CadastralNumber);
                SetTableCellValue(tableIndex, startRow, 2, item.NeighborEast.IsNullOrEmpty() ? "" : item.NeighborEast);
                SetTableCellValue(tableIndex, startRow, 3, item.NeighborWest.IsNullOrEmpty() ? "" : item.NeighborWest);
                SetTableCellValue(tableIndex, startRow, 4, item.NeighborSouth.IsNullOrEmpty() ? "" : item.NeighborSouth);
                SetTableCellValue(tableIndex, startRow, 5, item.NeighborNorth.IsNullOrEmpty() ? "" : item.NeighborNorth);
                SetTableCellValue(tableIndex, startRow, 6, item.ActualArea.ToString());
                totleActualArea += item.ActualArea;
                SetTableCellValue(tableIndex, startRow, 7, item.LandLevel);
                SetTableCellValue(tableIndex, startRow, 8, item.Comment);
                startRow++;
            }
            SetTableCellValue(tableIndex, startRow, 6, totleActualArea.ToString());
        }

        /// <summary>
        /// 获取人的年龄
        /// </summary>
        /// <returns></returns>
        private string GetPersonBirthday(Person person, bool showBirthday)
        {
            if (!person.Birthday.HasValue || string.IsNullOrEmpty(person.ICN))
            {
                return string.Empty;
            }
            if (!showBirthday && person.Birthday.HasValue)
            {
                int age = DateTime.Now.Year - person.Birthday.Value.Year;
                return age.ToString();
            }
            if (ToolICN.Check(person.ICN))
            {
                DateTime birthDay = ToolICN.GetBirthday(person.ICN);
                return birthDay.Year.ToString() + "." + birthDay.Month.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns></returns>
        private string GetGender()
        {
            List<Person> persons = virtualPerson.SharePersonList;   //得到户对应的共有人
            List<Person> sharePersons = SortSharePerson(persons, virtualPerson.Name);//排序共有人，并返回人口集合
            string value = sharePersons.Count > 0 ? EnumNameAttribute.GetDescription(sharePersons[0].Gender) : "";
            string sex = value == EnumNameAttribute.GetDescription(eGender.Unknow) ? "" : value;
            return " " + sex + " ";
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns></returns>
        private string GetAge()
        {
            List<Person> persons = virtualPerson.SharePersonList;   //得到户对应的共有人
            List<Person> sharePersons = SortSharePerson(persons, virtualPerson.Name);//排序共有人，并返回人口集合
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

        #endregion Methods - Family

        #region Methods - ContractLand

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        protected override void WriteReclamationInformation()
        {
            List<ContractLand> landCollection = ListLand.Clone() as List<ContractLand>;
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
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("ReclamationTableArea" + (i == 0 ? "" : i.ToString()), reclamationTableArea.AreaFormat());//台帐面积
                SetBookmarkValue("RetainTableArea" + (i == 0 ? "" : i.ToString()), retainTableArea.AreaFormat());
                SetBookmarkValue("ReclamationActualArea" + (i == 0 ? "" : i.ToString()), reclamationActualArea.AreaFormat());
                SetBookmarkValue("RetainActualArea" + (i == 0 ? "" : i.ToString()), retainActualArea.AreaFormat());
                SetBookmarkValue("ReclamationAwareArea" + (i == 0 ? "" : i.ToString()), reclamationAwareArea.AreaFormat());
                SetBookmarkValue("RetainAwareArea" + (i == 0 ? "" : i.ToString()), retainAwareArea.AreaFormat());
            }
            landCollection.Clear();
            landArray.Clear();
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        protected override List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
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

        #endregion Methods - ContractLand

        #region Methods - Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        protected virtual void WriteConcordInfo()
        {
            //WitePersonInformaion();
            WriteSharePersonValue();

            WriteLandInfo();

            //WriteReclamationInformation();

            //WriteStartAndEnd();

            //WritePrintDate();
            var date = "";
            if (Concord.ArableLandType == "110")
                SetBookmarkValue("LandType", "家庭承包方式");
            else
                SetBookmarkValue("LandType", "其他承包方式");
            if (Concord.ArableLandStartTime != null && Concord.ArableLandEndTime != null)
            {
                date = ((DateTime)Concord.ArableLandStartTime).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + "至"
                    + ((DateTime)Concord.ArableLandEndTime).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + "。";
            }
            SetBookmarkValue(AgricultureBookMark.ConcordTrem, Concord.Flag ? "长久" : Concord.ManagementTime + "年:");  // 合同期限
            SetBookmarkValue(AgricultureBookMark.ConcordDate, Concord.Flag ? "" : date);                                // 承包时间
            SetBookmarkValue(AgricultureBookMark.ConcordNumber, concord.ConcordNumber.IsNullOrEmpty() ? string.Empty : concord.ConcordNumber);  // 合同编码
            var senderName = GetSettingSenderName(concord.ZoneCode) ?? concord.SenderName;
            var contractorAddress = GetSettingContractorName(concord.ZoneCode) ?? virtualPerson.Address;
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue(AgricultureBookMark.SenderName + (i == 0 ? "" : i.ToString()), senderName);     // 发包方名称
                SetBookmarkValue("ContractorAddress" + (i == 0 ? "" : i.ToString()), contractorAddress);     // 承包方地址
            }

            SetBookmarkValue(AgricultureBookMark.SenderLawyerName, Tissue.LawyerName.IsNullOrEmpty() ? string.Empty : Tissue.LawyerName);     // 发包方法人名称（即发包方负责人）
            SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber, Tissue.LawyerCartNumber);//发包方法人证件号码
            SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone, Tissue.LawyerTelephone);//发包方法人联系方式
            SetBookmarkValue(AgricultureBookMark.ContractorTelephone, Contractor.Telephone.IsNullOrEmpty() ? "/" : Contractor.Telephone);//承包方电话号码
            SetBookmarkValue(AgricultureBookMark.ContractorIdentifyNumber, Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码

            // 以下代码注释，当主版本需要时再启用，现在实际情况，手签名和盖章比较好，故暂时隐藏
            //SetBookmarkValue("SenderName1", concord.SenderName.IsNullOrEmpty() ? string.Empty : concord.SenderName);     // 发包方名称（章）
            //SetBookmarkValue("SenderLawyerName1", Tissue.LawyerName.IsNullOrEmpty() ? string.Empty : Tissue.LawyerName); // 发包方法人名称（即发包方负责人）（章）
            //SetBookmarkValue(AgricultureBookMark.SenderLawyerTelephone, Tissue.LawyerTelephone.IsNullOrEmpty() ? string.Empty : Tissue.LawyerTelephone);  // 发包方法人联系方式
            //SetBookmarkValue(AgricultureBookMark.SenderLawyerCredentNumber, Tissue.LawyerCartNumber.IsNullOrEmpty() ? string.Empty : Tissue.LawyerCartNumber);    // 发包方法人证件号码
        }

        #endregion Methods - Concord

        #region Methods - OtherInfomation

        /// <summary>
        /// 检查数据
        /// </summary>
        protected virtual bool CheckDataInformation(object data)
        {
            //检查合同数据
            concord = data as ContractConcord;
            if (concord == null)
            {
                return false;
            }
            //得到地块集合
            if (ListLand == null)
            {
                ListLand = new List<ContractLand>();
            }
            //地块排序
            ListLand = SortLandCollection(ListLand);
            //检查地域数据
            if (CurrentZone == null)
            {
                return false;
            }
            if (VirtualPerson == null)
            {
                VirtualPerson = new VirtualPerson() { Name = "   " };
            }
            return true;
        }

        /// <summary>
        /// 写表头信息
        /// </summary>
        private void WriteTitleInformation()
        {
            int familyNumber = 0;
            Int32.TryParse(virtualPerson.FamilyNumber, out familyNumber);
            string alloctioonPerson = virtualPerson.FamilyExpand.AllocationPerson;
            alloctioonPerson = string.IsNullOrEmpty(alloctioonPerson) ? "  " : alloctioonPerson;
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("ContracterName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(virtualPerson.Name));
                SetBookmarkValue("HouseHolderName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(virtualPerson.Name));
                SetBookmarkValue("bmAccepterName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(virtualPerson.Name));
                SetBookmarkValue("bmIdentifyNumber" + (i == 0 ? "" : i.ToString()), virtualPerson.Number);
                SetBookmarkValue("bmIcnNumber" + (i == 0 ? "" : i.ToString()), virtualPerson.Number);
                SetBookmarkValue("bmHouseHolderNumber" + (i == 0 ? "" : i.ToString()), virtualPerson.Number);
                SetBookmarkValue("bmContractorNumber" + (i == 0 ? "" : i.ToString()), virtualPerson.Number);
                SetBookmarkValue("bmFamilyNumber" + (i == 0 ? "" : i.ToString()), AgricultureSetting.AgricultureLandEncodingRule == 0 ? string.Format("{0:D3}", familyNumber) : string.Format("{0:D4}", familyNumber));
                SetBookmarkValue("bmAlloctionPerson" + (i == 0 ? "" : i.ToString()), alloctioonPerson);
            }
            //WriteZoneInformation();

            /* 修改于2016/8/27 改变获取地域的方式 */
            WriteZoneInfo();

            WriteDateExpressInformation();
            //CollectivityTissue tissue = senderBusiness.GetSenderById(concord.SenderId);
            string address = GetLandLocation();
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("bmSender" + (i == 0 ? "" : i.ToString()), concord.SenderName);
                SetBookmarkValue("bmSenderName" + (i == 0 ? "" : i.ToString()), concord.SenderName);
                SetBookmarkValue("bmSenderNameExpress" + (i == 0 ? "" : i.ToString()), CreateSenderExpress());
                SetBookmarkValue("LandTerm" + (i == 0 ? "" : i.ToString()), concord.ManagementTime);
                SetBookmarkValue("LandAddress" + (i == 0 ? "" : i.ToString()), string.IsNullOrEmpty(concord.SecondContracterLocated) ? address : concord.SecondContracterLocated);
                SetBookmarkValue("bmLandPurpose" + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(concord.LandPurpose));
                SetBookmarkValue("ContractMode" + (i == 0 ? "" : i.ToString()), EnumNameAttribute.GetDescription(concord.ArableLandType));
            }
            SetBookmarkValue("bmAccepterAddress", string.IsNullOrEmpty(concord.SecondContracterLocated) ? address : concord.SecondContracterLocated);
            SetBookmarkValue("bmLandAddress", string.IsNullOrEmpty(concord.SecondContracterLocated) ? address : concord.SecondContracterLocated);
        }

        /// <summary>
        /// 填写承包方地址
        /// </summary>
        private void WriteZoneInfo()
        {
            if (CurrentZone == null)
            {
                return;
            }
            List<Zone> zonesToProvince;
            try
            {
                var zoneStation = db.CreateZoneWorkStation();
                zonesToProvince = zoneStation.GetAllZonesToProvince(CurrentZone);

                var province = zonesToProvince.Find(c => c.Level == eZoneLevel.Province);
                var city = zonesToProvince.Find(c => c.Level == eZoneLevel.City);
                var county = zonesToProvince.Find(c => c.Level == eZoneLevel.County);
                var town = zonesToProvince.Find(c => c.Level == eZoneLevel.Town);
                var village = zonesToProvince.Find(c => c.Level == eZoneLevel.Village);
                var group = zonesToProvince.Find(c => c.Level == eZoneLevel.Group);

                string zoneName = county == null ? "" : county.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("bmCountryName" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
                zoneName = province == null ? "" : province.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
                zoneName = city == null ? "" : city.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
                zoneName = town == null ? "" : town.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
                zoneName = village == null ? "" : village.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", ""));
                }
                zoneName = group == null ? "" : group.Name;
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        private string GetSettingSenderName(string zoneCode)
        {
            switch (concordSetting.SendNamePrefixLevel)
            {
                case 0:
                    return GetZoneNameByLevel(zoneCode, eZoneLevel.Town);

                case 1:
                    return GetZoneNameByLevel(zoneCode, eZoneLevel.County);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        private string GetSettingContractorName(string zoneCode)
        {
            switch (concordSetting.ContractorNamePrefixLevel)
            {
                case 0:
                    return GetZoneNameByLevel(zoneCode, eZoneLevel.Town);

                case 1:
                    return GetZoneNameByLevel(zoneCode, eZoneLevel.County);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel zoneLevel)
        {
            var zoneName = string.Empty;
            Zone zone = zoneBusiness.Get(zoneCode);
            if (zone != null)
            {
                for (eZoneLevel level = zoneLevel; level >= zone.Level && level <= zoneLevel; level--)
                {
                    zoneName += GetZoneName(zoneCode, level);
                }
            }

            return zoneName;
        }

        /// <summary>
        /// 通过地域全编码和行政区级别获取相应的行政区名称
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetZoneName(string zoneCode, eZoneLevel level)
        {
            var zoneStation = DataBaseSource.GetDataBaseSource().CreateZoneWorkStation();
            Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();

            if (temp == null)
            {
                return string.Empty;
            }
            if (temp.Level == level)
            {
                return temp.Name;
            }

            return GetZoneName(temp.UpLevelCode, level);
        }

        /// <summary>
        /// 填写日期扩展信息
        /// </summary>
        private void WriteDateExpressInformation()
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("FamilyName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(virtualPerson.Name));
                string gender = GetGender();
                SetBookmarkValue("Gender" + (i == 0 ? "" : i.ToString()), gender);
                string age = GetAge();
                SetBookmarkValue("Age" + (i == 0 ? "" : i.ToString()), age);
                SetBookmarkValue("IdentifyNumber" + (i == 0 ? "" : i.ToString()), virtualPerson.Number);
                SetBookmarkValue("Comment" + (i == 0 ? "" : i.ToString()), virtualPerson.Comment);
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
            }
        }

        /// <summary>
        /// 获取土地地址
        /// </summary>
        private string GetLandLocation()
        {
            Zone city = zoneBusiness.Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            if (city == null)
            {
                return CurrentZone.FullName;
            }
            string location = CurrentZone.FullName.Replace(city.FullName, "");
            city = null;
            return location;
        }

        /// <summary>
        /// 创建发包方扩展
        /// </summary>
        /// <returns></returns>
        private string CreateSenderExpress()
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
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd()
        {
            string startYear = (concord.ArableLandStartTime == null || !concord.ArableLandStartTime.HasValue) ? "" : concord.ArableLandStartTime.Value.Year.ToString();
            string startMonth = (concord.ArableLandStartTime == null || !concord.ArableLandStartTime.HasValue) ? "" : concord.ArableLandStartTime.Value.Month.ToString();
            string startDay = (concord.ArableLandStartTime == null || !concord.ArableLandStartTime.HasValue) ? "" : concord.ArableLandStartTime.Value.Day.ToString();
            string endYear = (concord.ArableLandEndTime == null || !concord.ArableLandEndTime.HasValue) ? "" : concord.ArableLandEndTime.Value.Year.ToString();
            string endMonth = (concord.ArableLandEndTime == null || !concord.ArableLandEndTime.HasValue) ? "" : concord.ArableLandEndTime.Value.Month.ToString();
            string endDay = (concord.ArableLandEndTime == null || !concord.ArableLandEndTime.HasValue) ? "" : concord.ArableLandEndTime.Value.Day.ToString();
            string date = "";
            if (concord.ArableLandStartTime != null && concord.ArableLandStartTime.HasValue && concord.ArableLandEndTime != null && concord.ArableLandEndTime.HasValue)
            {
                date = "自" + string.Format("{0}年{1}月{2}日", concord.ArableLandStartTime.Value.Year, concord.ArableLandStartTime.Value.Month, concord.ArableLandStartTime.Value.Day) + "起至"
                              + string.Format("{0}年{1}月{2}日", concord.ArableLandEndTime.Value.Year, concord.ArableLandEndTime.Value.Month, concord.ArableLandEndTime.Value.Day) + "止";
            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("SYear" + (i == 0 ? "" : i.ToString()), startYear);//起始时间-年
                SetBookmarkValue("SMonth" + (i == 0 ? "" : i.ToString()), startMonth);//起始时间-月
                SetBookmarkValue("SDay" + (i == 0 ? "" : i.ToString()), startDay);//起始时间-日
                SetBookmarkValue("EYear" + (i == 0 ? "" : i.ToString()), endYear);//起始时间-年
                SetBookmarkValue("EMonth" + (i == 0 ? "" : i.ToString()), endMonth);//起始时间-月
                SetBookmarkValue("EDay" + (i == 0 ? "" : i.ToString()), endDay);//起始时间-日
                SetBookmarkValue("ManagementTime" + (i == 0 ? "" : i.ToString()), concord.Flag ? "长久" : date);//结束时间-日
                if (concord.Flag)
                {
                    SetBookmarkValue("bmLongTime" + (i == 0 ? "" : i.ToString()), "长久");//承包期限长久
                }
            }
        }

        /// <summary>
        /// 填写签约日期
        /// </summary>
        private void WritePrintDate()
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Year" + (i == 0 ? "" : i.ToString()), string.IsNullOrEmpty(Year) ? DateTime.Now.Year.ToString() : Year);//签约日期-年
                SetBookmarkValue("Month" + (i == 0 ? "" : i.ToString()), string.IsNullOrEmpty(Month) ? DateTime.Now.Month.ToString() : Month);//签约日期-月
                SetBookmarkValue("Day" + (i == 0 ? "" : i.ToString()), string.IsNullOrEmpty(Day) ? DateTime.Now.Day.ToString() : Day);//签约日期-日
            }
        }

        #endregion Methods - OtherInfomation

        //#region Methods - 辅助方法

        ///// <summary>
        ///// 初始化户主名称
        ///// </summary>
        //private string InitalizeFamilyName(string familyName)
        //{
        //    if (string.IsNullOrEmpty(familyName))
        //    {
        //        return "";
        //    }
        //    string number = ToolString.GetAllNumberWithInString(familyName);
        //    if (!string.IsNullOrEmpty(number))
        //    {
        //        return familyName.Replace(number, "");
        //    }
        //    int index = familyName.IndexOf("(");
        //    if (index > 0)
        //    {
        //        return familyName.Substring(0, index);
        //    }
        //    return familyName;
        //}

        //#endregion

        #endregion Methods
    }
}