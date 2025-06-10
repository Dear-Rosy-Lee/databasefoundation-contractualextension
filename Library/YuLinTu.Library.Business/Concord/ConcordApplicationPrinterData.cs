using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 农村土地承包经营权集体登记申请书
    /// </summary>
    public class ConcordApplicationPrinterData
    {
        #region Fields

        private IDbContext db;
        private CollectivityTissue tissue;
        private Zone currentZone;//当前地域

        private string nameProvice;
        private string nameCity;
        private string nameTown;
        private string nameCounty;
        private string nameVillage;
        private string nameGroup;
        private string year;
        private string requireNumber;
        private string nameTissue;
        private string areaContract;
        private string areaFarm;
        private string areaOther;
        private string cntContractor;
        private bool useActualArea;

        #endregion

        #region Properties

        /// <summary>
        /// 省区域名称
        /// </summary>
        public string NameProvice
        {
            set { nameProvice = value; }
            get { return nameProvice; }
        }

        /// <summary>
        /// 市区域名称
        /// </summary>
        public string NameCity
        {
            set { nameCity = value; }
            get { return nameCity; }
        }

        /// <summary>
        /// 镇区域名称
        /// </summary>
        public string NameTown
        {
            set { nameTown = value; }
            get { return nameTown; }
        }

        /// <summary>
        /// 乡区域名称
        /// </summary>
        public string NameCounty
        {
            set { nameCounty = value; }
            get { return nameCounty; }
        }

        /// <summary>
        /// 村区域名称
        /// </summary>
        public string NameVillage
        {
            set { nameVillage = value; }
            get { return nameVillage; }
        }

        /// <summary>
        /// 组区域名称
        /// </summary>
        public string NameGroup
        {
            set { nameGroup = value; }
            get { return nameGroup; }
        }

        public string Year
        {
            get { return year; }
        }

        public string RequireNumber
        {
            get { return requireNumber; }
        }

        public string NameTissue
        {
            get { return nameTissue; }
        }

        public string AreaContract
        {
            get { return areaContract; }
        }

        public string AreaFarm
        {
            get { return areaFarm; }
        }


        public string AreaOther
        {
            get { return areaOther; }
        }

        /// <summary>
        /// 合同数目
        /// </summary>
        public string CountContractor
        {
            get { return cntContractor; }
        }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 地块总数
        /// </summary>
        public string LandCount { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public string ContractMode { get; set; }

        /// <summary>
        /// 承包期限
        /// </summary>
        public string ContractTerm { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime? RequireDate { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DataInstance
        {
            get
            {
                return db;
            }
        }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get
            {
                return currentZone;
            }
        }

        /// <summary>
        /// 集体经济组织
        /// </summary>
        public CollectivityTissue Tissue
        {
            get
            {
                return tissue;
            }
        }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandCollection
        { get; set; }

        /// <summary>
        /// 合同集合
        /// </summary>
        public List<ContractConcord> ConcordCollection
        { get; set; }

        /// <summary>
        /// 农村土地承包经营申请登记表
        /// </summary>
        public ContractRequireTable RequireTable
        { get; set; }


        public int DicemallNumber { get; set; }
        
        #endregion

        #region Ctor

        public ConcordApplicationPrinterData(IDbContext db, CollectivityTissue tissue, Zone zone, string year = "", string number = "")
        {
            this.db = db;
            this.tissue = tissue;
            this.currentZone = zone;
            this.year = year;
            this.requireNumber = number;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 记录
        /// </summary>
        /// <returns></returns>
        public ContractRequireTable Record()
        {
            ContractRequireTable tab = this.RequireTable;
            if (tab != null)
            {
                tab.ZoneCode = currentZone.FullCode;
                return tab;
            }
            tab = new ContractRequireTable();
            tab.ID = Guid.NewGuid();
            tab.Year = Year;
            tab.Number = RequireNumber;
            tab.Path = tissue.Code;
            tab.CreationTime = DateTime.Now;
            tab.ModifiedTime = DateTime.Now;
            //if (RequireDate != null && RequireDate.HasValue)
            //{
            //    tab.Date = RequireDate.Value;
            //}
            tab.Date = RequireDate;
            tab.ZoneCode = currentZone.FullCode;

            return tab;
        }

        /// <summary>
        /// 初始化内部数据
        /// </summary>
        public void InitializeInnerData()
        {
            useActualArea = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
            Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积    
            InitializeTissueInfo(tissue);
            //nameGroup = "";
        }

        /// <summary>
        /// 初始化集体经济组织
        /// </summary>
        /// <param name="tissue"></param>
        private void InitializeTissueInfo(CollectivityTissue tissue)
        {
            nameTissue = tissue.Name;
            ContractMode = EnumNameAttribute.GetDescription(eConstructMode.Family);
            List<ContractLand> landCollection = this.LandCollection;
            List<ContractConcord> listConcords = this.ConcordCollection;
            double area = 0;
            int landCount = 0;
            string code = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
            bool isFlag = false;
            if (listConcords != null && listConcords.Count > 0)
            {
                isFlag = listConcords[0].Flag;
                StartTime = listConcords[0].ArableLandStartTime.HasValue ? listConcords[0].ArableLandStartTime : null;
                EndTime = listConcords[0].ArableLandEndTime.HasValue ? listConcords[0].ArableLandEndTime : null;
            }
            if (isFlag)
            {
                StartTime = null;
                EndTime = null;
            }
            foreach (ContractConcord concord in listConcords)
            {
                if (concord.ArableLandType != code)
                {
                    continue;
                }
                //if (StartTime == null)
                //{
                //    StartTime = concord.ArableLandStartTime.HasValue ? concord.ArableLandStartTime : null;
                //}
                //if (EndTime == null)
                //{
                //    EndTime = concord.ArableLandEndTime.HasValue ? concord.ArableLandEndTime : null;
                //}
                area += useActualArea ? concord.CountActualArea : concord.CountAwareArea;
                landCount += landCollection.Count(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == concord.ID);
            }
            LandCount = landCount.ToString();
            if (isFlag)
            {
                ContractTerm = ":长久";
            }
            else
            {
                if (StartTime != null && StartTime.HasValue && EndTime != null && EndTime.HasValue)
                {
                    //ContractTerm = ToolDateTime.CalcateTerm(StartTime, EndTime);
                    string stYear = (StartTime != null && StartTime.HasValue) ? StartTime.Value.Year.ToString() : "";
                    string stMonth = (StartTime != null && StartTime.HasValue) ? StartTime.Value.Month.ToString() : "";
                    string stDay = (StartTime != null && StartTime.HasValue) ? StartTime.Value.Day.ToString() : "";

                    string edYear = (StartTime != null && EndTime.HasValue) ? EndTime.Value.Year.ToString() : "";
                    string edMonth = (StartTime != null && EndTime.HasValue) ? EndTime.Value.Month.ToString() : "";
                    string edDay = (StartTime != null && EndTime.HasValue) ? EndTime.Value.Day.ToString() : "";

                    string term = "自" + stYear + "年" + stMonth + "月" + stDay + " 日" + "起至" + edYear + "年" + edMonth + "月" + edDay + "日" + "止";
                    ContractTerm = term;
                }
            }
            areaContract = area.AreaFormat(DicemallNumber);
            areaFarm = areaContract;
            areaOther = " ";
            cntContractor = listConcords.FindAll(c => c.ArableLandType == code).Count.ToString();
            listConcords.Clear();
            landCollection = null;
            GC.Collect();
        }

        #endregion
    }
}
