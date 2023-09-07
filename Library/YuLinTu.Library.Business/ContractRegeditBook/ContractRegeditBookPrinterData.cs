/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 打印登记簿初始化数据
    /// </summary>
    public class ContractRegeditBookPrinterData
    {
        #region Fields

        private ContractConcord concord;
        private ContractRegeditBook book;
        private Zone contractorZone;
        private List<Person> listSharePerson;
        private List<ContractLand> listLands;
        private double areaAll;
        private double tableArea;
        private double awareArea;
        private double actualArea;
        private string contractorAddress;
        private string cityAndCountyName;
        private string proviceName;
        private string cityName;
        private string countryName;
        private string townName;
        private string villageName;
        private string groupName;
        private string posterNumer;
        private string telephone;
        private string icnNumber;
        private string familyComment;
        private bool useActualArea;
        private bool exportAllLandType;

        private IVirtualPersonWorkStation<LandVirtualPerson> personStation;
        private IContractLandWorkStation landStation;
        private IDictionaryWorkStation dictStation;
        private IConcordWorkStation concordStation;
        private IContractRegeditBookWorkStation bookStation;
        private ISenderWorkStation senderStation;
        private IZoneWorkStation zoneStation;
        private IBuildLandBoundaryAddressDotWorkStation dotWorkStation;

        #endregion

        #region Properties

        /// <summary>
        /// 承包地块业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        /// <summary>
        /// 合同业务
        /// </summary>
        public ConcordBusiness ConcordBusiness { get; set; }

        /// <summary>
        /// 数据字典业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 承包方业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine { get; set; }

        /// <summary>
        /// 字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        public string CityAndCountyName
        {
            get { return cityAndCountyName; }
        }

        public string PriviceName
        {
            get { return proviceName; }
        }

        public string CityName
        {
            get { return cityName; }
        }

        public string CountyName
        {
            get { return countryName; }
        }

        public string TownName
        {
            get { return townName; }
        }

        public string VillageName
        {
            get { return villageName; }
        }

        public string GroupName
        {
            get { return groupName; }
        }

        public string BookNumber
        {
            get { return book.Number; }
        }

        public string RegeditNumber
        {
            get { return book.RegeditNumber; }
        }

        public string SenderName
        {
            get { return concord.SenderName; }
        }

        public string ContractorName
        {
            get { return concord.ContracterName; }
        }

        public string IcnNumber
        {
            get { return icnNumber; }
        }

        public string ContractorComment
        {
            get
            {
                return familyComment;
            }
        }

        public string PosterNumber
        {
            get { return posterNumer; }
        }

        public string ContractorAddress
        {
            get { return contractorAddress; }
        }

        public bool isWriteAddress
        {
            get
            {
                return (concord != null && !string.IsNullOrEmpty(concord.SecondContracterLocated)) ? true : false;
            }
        }

        public string ContractorTelephone
        {
            get { return string.IsNullOrEmpty(telephone) ? concord.ContracterRepresentTelphone : telephone; }
        }

        public string TableArea
        {
            get
            {
                return tableArea.ToString();
            }
        }

        public string AwareArea
        {
            get { return awareArea.ToString(); }
        }

        public string ActualArea
        {
            get { return actualArea.ToString(); }
        }

        public string ConcordNumber
        {
            get { return concord.ConcordNumber; }
        }

        public ContractConcord Concord
        {
            get
            {
                return concord;
            }
        }

        public string StartTime
        {
            get
            {
                DateTime dt = concord.ArableLandStartTime.Value;
                return string.Format("{0}年{1}月{2}日", dt.Year, dt.Month, dt.Day);
            }
        }

        public string EndTime
        {
            get
            {
                if (concord.Flag)
                    return "\\";
                DateTime dt = concord.ArableLandEndTime.Value;
                return string.Format("{0}年{1}月{2}日", dt.Year, dt.Month, dt.Day);
            }
        }

        public string ContractType
        {
            get { return EnumNameAttribute.GetDescription(concord.ArableLandType); }
        }

        public string UseType
        {
            get
            {
                string val = EnumNameAttribute.GetDescription(concord.LandPurpose);
                val = (string.IsNullOrEmpty(val) || ToolMath.MatchEntiretyNumber(val)) ? "种植业" : val;
                return val;
            }
        }

        public List<Person> SharePersons
        {
            get { return listSharePerson; }
        }

        public string AreaLand
        {
            get { return areaAll.ToString(); }
        }

        public string CountLand
        {
            get { return listLands.Count.ToString(); }
        }

        public List<ContractLand> ContractLands
        {
            get { return listLands; }
        }

        /// <summary>
        /// 有效界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListLandDots { get; set; }

        /// <summary>
        /// 是否使用实测面积作为确权面积
        /// </summary>
        public bool UseActualArea
        {
            get { return useActualArea; }
        }

        /// <summary>
        /// 显示共有人家庭关系
        /// </summary>
        public bool ShowPersonRealation { get; set; }

        /// <summary>
        /// 显示共有人出生年月
        /// </summary>
        public bool ShowPersonBirthday { get; set; }

        /// <summary>
        /// 去掉二轮台账列
        /// </summary>
        public bool ExceptTableAreaColumn { get; set; }

        /// <summary>
        /// 设置四列四至
        /// </summary>
        public bool SetLandNeighborFourColumn { get; set; }

        /// <summary>
        /// 确权面积代替合同面积
        /// </summary>
        public bool TabaleAreaReplaceByAwareArea { get; set; }

        /// <summary>
        /// 去掉是否基本农田列
        /// </summary>
        public bool ExceptIsFarmerLandColumn { get; set; }

        /// <summary>
        /// 不填写地块二轮合同面积
        /// </summary>
        public bool NoWriteLandTableArea { get; set; }

        /// <summary>
        /// 填写地类信息
        /// </summary>
        public bool WriteLandTypeInformation { get; set; }

        /// <summary>
        /// 权证
        /// </summary>
        public ContractRegeditBook Book
        {
            get
            {
                return book;
            }
        }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return contractorZone; }
            set { contractorZone = value; }
        }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson FamilyEntry { get; set; }

        /// <summary>
        /// 集体经济组织
        /// </summary>
        public CollectivityTissue Tissue { get; set; }

        public IDbContext DbContext { get; set; }

        #endregion

        #region Ctor

        public ContractRegeditBookPrinterData(ContractConcord concord)
        {
            this.concord = concord;
        }

        #endregion

        #region Methods

        public bool InitializeInnerData()
        {
            bool result = true;
            try
            {
                personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                landStation = DbContext.CreateContractLandWorkstation();
                concordStation = DbContext.CreateConcordStation();
                bookStation = DbContext.CreateRegeditBookStation();
                dictStation = DbContext.CreateDictWorkStation();
                senderStation = DbContext.CreateSenderWorkStation();
                zoneStation = DbContext.CreateZoneWorkStation();
                dotWorkStation = DbContext.CreateBoundaryAddressDotWorkStation();

                InitializeCommonData();
                InitializeSharePerson();
                InitializeLands();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void InitializeLands()
        {
            bool ActualAreaColumnWriteActualAreaWithLandBook = true;
            useActualArea = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
            Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积
            exportAllLandType = false;
            string landValue = ToolConfiguration.GetSpecialAppSettingValue("ExportAllLandTypeSetting", "false");
            Boolean.TryParse(landValue, out exportAllLandType);
            listLands = exportAllLandType ?
                landStation.GetCollection(concord.ContracterId != null ? concord.ContracterId.Value : Guid.Empty) : landStation.GetByConcordId(concord.ID);
            listLands = SortLandCollection(listLands);
            listLands.LandNumberFormat(SystemDefine);
            areaAll = 0;
            foreach (ContractLand land in listLands)
            {
                areaAll += ActualAreaColumnWriteActualAreaWithLandBook ? land.ActualArea : land.AwareArea;    //AgricultureWarrantSetting.ActualAreaColumnWriteActualAreaWithLandBook  设置问题
                tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                awareArea += land.AwareArea;
                actualArea += land.ActualArea;
            }
            tableArea = Math.Round(tableArea, 4);
            awareArea = Math.Round(awareArea, 4);
            actualArea = Math.Round(actualArea, 4);
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
            return landCollection;
        }

        private List<ContractLand> SpliteLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            List<ContractLand> landCollection = new List<ContractLand>();

            string landCategory = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB).Find(c => c.Name == "承包地块").Code;
            foreach (ContractLand land in lands)
            {
                if (land.LandCategory != landCategory || land.ActualArea == 0)
                {
                    continue;
                }
                landCollection.Add(land);
            }
            return landCollection;
        }

        private void InitializeSharePerson()
        {
            VirtualPerson vp = personStation.Get(concord.ContracterId != null ? concord.ContracterId.Value : Guid.Empty);
            FamilyEntry = vp == null ? new VirtualPerson() { Name = "" } : vp.Clone() as VirtualPerson;
            List<Person> persons = vp.SharePersonList;
            listSharePerson = SortSharePerson(persons);
            bool showPersonRelation = false;
            string value = ToolConfiguration.GetSpecialAppSettingValue("ExportSharePersonRelationInformation", "false");
            Boolean.TryParse(value, out showPersonRelation);
            ShowPersonRealation = showPersonRelation;
            bool showPersonBirthday = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExportSharePersonBirthdayInformation", "false");
            Boolean.TryParse(value, out showPersonBirthday);
            ShowPersonBirthday = showPersonBirthday;
            bool exceptTabAreaColumn = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExceptTableAreaColumnSetting", "false");
            Boolean.TryParse(value, out exceptTabAreaColumn);
            ExceptTableAreaColumn = exceptTabAreaColumn;
            bool setLandNeighborFourColumn = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("SetLandNeighborFourColumnInformation", "false");
            Boolean.TryParse(value, out setLandNeighborFourColumn);
            SetLandNeighborFourColumn = setLandNeighborFourColumn;
            bool tableAreaReplateByAwareArea = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("TableAreaColumnReplaceAwareArea", "false");
            Boolean.TryParse(value, out tableAreaReplateByAwareArea);
            TabaleAreaReplaceByAwareArea = tableAreaReplateByAwareArea;
            bool exceptIsFarmerLandColumn = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExceptIsFarmerLandColumnSetting", "false");
            Boolean.TryParse(value, out exceptIsFarmerLandColumn);
            ExceptIsFarmerLandColumn = exceptIsFarmerLandColumn;
            bool noWriteLandTableArea = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("NoWriteLandTableAreaSetting", "false");
            Boolean.TryParse(value, out noWriteLandTableArea);
            NoWriteLandTableArea = noWriteLandTableArea;
            bool writeLandTypeInformation = false;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExportLandTypeInformationSetting", "false");
            Boolean.TryParse(value, out writeLandTypeInformation);
            WriteLandTypeInformation = writeLandTypeInformation;
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection)
        {
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in personCollection)
            {
                if (person.Name == concord.ContracterName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != concord.ContracterName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        private void InitializeCommonData()
        {
            Tissue = concord != null ? senderStation.Get(concord.SenderId) : null;
            if (Tissue == null)
            {
                Tissue = senderStation.Get(CurrentZone.ID);
            }
            if (Tissue == null)
            {
                var senders = senderStation.Get(s => s.ZoneCode == CurrentZone.FullCode);
                if (senders != null && senders.Count >= 1)
                    Tissue = senders[0];
            }
            book = concord != null ? bookStation.Get(concord.ID) : null;
            if (book == null)
            {
                return;// (ExAgricultureString.CONTRACT_CONCORD_NO_BOOK);
            }

            var listLandDots = dotWorkStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.Self);
            ListLandDots = listLandDots == null ? new List<BuildLandBoundaryAddressDot>() : listLandDots;

            VirtualPerson family = personStation.Get(concord.ContracterId.Value);
            if (family == null)
            {
                family = new VirtualPerson();
                family.Name = concord.ContracterName;
                family.ZoneCode = concord.ZoneCode;
            }
            contractorZone = zoneStation.Get(concord.ZoneCode);
            contractorAddress = GetZoneNameWithoutCityName(contractorZone.FullCode);
            posterNumer = family.PostalNumber;
            telephone = family.Telephone;
            icnNumber = family.Number;
            familyComment = family.Comment;
            Zone provice = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
            proviceName = provice != null ? provice.Name : "";
            Zone city = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            cityName = city != null ? city.Name : "";
            Zone counry = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
            countryName = counry != null ? counry.Name : "";
            cityAndCountyName = city.Name + counry.Name;
            Zone town = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
            townName = town != null ? town.Name : "";
            Zone village = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
            villageName = village != null ? village.Name : " ";
            if (family.ZoneCode.Length >= Zone.ZONE_GROUP_LENGTH)
            {
                Zone group = zoneStation.Get(family.ZoneCode.Substring(0, Zone.ZONE_GROUP_LENGTH));
                groupName = group != null ? group.Name : "";
            }
        }

        private string GetZoneNameWithoutCityName(string codeZone)
        {
            if (string.IsNullOrEmpty(codeZone))
                return string.Empty;

            if (codeZone.Length <= Zone.ZONE_CITY_LENGTH)
                return string.Empty;

            string name = string.Empty;
            string cityCode = codeZone.Substring(0, Zone.ZONE_CITY_LENGTH);
            Zone cityZone = zoneStation.Get(cityCode);
            Zone zone = zoneStation.Get(codeZone);
            name = zone.FullName.IsNullOrEmpty() ? "" : zone.FullName.Replace(cityZone.FullName, string.Empty);
            if (name == null)
            {
                name = string.IsNullOrEmpty(concord.SecondContracterLocated) ? name : concord.SecondContracterLocated;
            }
            return name;
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Disponse()
        {
            book = null;
            contractorZone = null;
            listSharePerson = null;
            listLands = null;
            GC.Collect();
        }

        #endregion

    }
}
