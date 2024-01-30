using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using System.Globalization;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 承包合同
    /// </summary>
    public class ExportAgricultureConcord : AgricultureWordBook
    {
        #region Fields

        private bool useActualArea;//是否使用实测面积
        private GetDictionary dic;

        #endregion

        #region Properties
        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }
        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson VirtualPerson { get; set; }
        /// <summary>
        /// 县级地域名称
        /// </summary>
        public string ZoneNameCounty { get; set; }

        /// <summary>
        /// 镇级地域名称
        /// </summary>
        public string ZoneNameTown { get; set; }

        /// <summary>
        /// 村级地域名称
        /// </summary>
        public string ZoneNameVillage { get; set; }

        /// <summary>
        /// 组级地域名称
        /// </summary>
        public string ZoneNameGroup { get; set; }
        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine Systemset { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> ListDict { get; set; }

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
        public IDbContext dbContext { get; set; }

        #endregion

        #region Ctor

        public ExportAgricultureConcord(string dictoryName)
        {
            dic = new GetDictionary(dictoryName);
            dic.Read();
        }

        #endregion

        #region Methods

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
                if (!CheckDataInformation(data))
                {
                    return false;
                }
                InitalizeDataInformation(data);
                base.OnSetParamValue(data);
                useActualArea = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
                Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积

                WriteTitleInformation();
                WriteConcordInformation();
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
        /// <param name="data"></param>
        /// <returns></returns>
        private void InitalizeDataInformation(object data)
        {
            base.Concord = Concord.Clone() as ContractConcord;
            Contractor = VirtualPerson.Clone() as VirtualPerson;
            var senderStation = dbContext.CreateSenderWorkStation();
            Tissue = senderStation.Get(Concord.SenderId);
            base.CurrentZone = CurrentZone.Clone() as Zone;
            LandCollection = ListLand.Clone() as List<ContractLand>;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            base.Destroyed();
            VirtualPerson = null;
            CurrentZone = null;
            ListLand = null;
            GC.Collect();
        }

        #endregion

        #region Contractland

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLandInformation()
        {
            if (ListLand == null || ListLand.Count < 1)
            {
                return;
            }

            int rowCount = ListLand.Count - 2;
            if (rowCount > 0)
            {
                InsertTableRow(0, 2, rowCount);
            }
            int tableIndex = 0;
            int startRow = 2;
            double allArea = 0.0;
            foreach (var item in ListLand)
            {
                SetTableCellValue(tableIndex, startRow, 0, item.Name);
                string landNumber = item.LandNumber;
                var systemSetting = SystemSetDefine.GetIntence();
                if (systemSetting.LandNumericFormatSet && landNumber.Length > systemSetting.LandNumericFormatValueSet)
                {
                    landNumber = landNumber.Substring(systemSetting.LandNumericFormatValueSet);
                }
                SetTableCellValue(tableIndex, startRow, 1, landNumber);
                SetTableCellValue(tableIndex, startRow, 2, item.NeighborEast);
                SetTableCellValue(tableIndex, startRow, 3, item.NeighborWest);
                SetTableCellValue(tableIndex, startRow, 4, item.NeighborSouth);
                SetTableCellValue(tableIndex, startRow, 5, item.NeighborNorth);
                double useArea = ChooseArea(item, WorkspacePageContext.concordSetting.ChooseArea);
                allArea += useArea;
                if (useActualArea)
                {
                    SetTableCellValue(tableIndex, startRow, 6, useArea > 0.0 ? useArea.ToString("f2") : AgricultureSetting.InitalizeAreaString());
                }
                else
                {
                    SetTableCellValue(tableIndex, startRow, 6, useArea > 0.0 ? useArea.ToString("f2") : AgricultureSetting.InitalizeAreaString());
                }
                try
                {
                    string levelString = EnumNameAttribute.GetDescription(item.LandLevel);
                    levelString = levelString == "未知" ? "" : levelString;
                    if (!ToolMath.MatchAllNumber(levelString))
                    {
                        levelString = "";
                    }
                    levelString = GetEnumDesp<eContractLandLevel>(item.LandLevel);
                    SetTableCellValue(tableIndex, startRow, 7, levelString);
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
                SetTableCellValue(tableIndex, startRow, 8, item.Comment);
                startRow++;
            }
            if (startRow < 4)
            {
                startRow = 4;
            }
            SetTableCellValue(tableIndex, startRow, 6, allArea.ToString("f2"));
        }

        /// <summary>
        /// 根据设置的合同面积输出“面积”
        /// </summary>
        /// <param name="land"></param>
        /// <param name="chooseAreaSetting"></param>
        /// <returns></returns>
        private double ChooseArea(ContractLand land, int chooseAreaSetting)
        {
            double area = 0.0;
            // 0 - 二轮合同面积，1 - 实测面积，2 - 确权面积
            switch (chooseAreaSetting)
            {
                case 0:
                    area = (double)land.TableArea;
                    break;
                case 1:
                    area = land.ActualArea;
                    break;
                case 2:
                    area = land.AwareArea;
                    break;
                default:
                    break;
            }
            return area;
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
            lands.Clear();
            return landCollection;
        }


        #endregion

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        private void WriteConcordInformation()
        {
            WriteLandInformation();
            WriteStartAndEnd();
        }

        #endregion

        #region OtherInformation

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd()
        {
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
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckDataInformation(object data)
        {
            //检查合同数据
            Concord = data as ContractConcord;
            if (Concord == null)
            {
                return false;
            }
            if (ListLand == null)
            {
                ListLand = new List<ContractLand>();
            }
            ListLand = SortLandCollection(ListLand);
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
            SetBookmarkValue(AgricultureBookMark.ConcordNumber, Concord.ConcordNumber.IsNullOrEmpty() ? "/" : Concord.ConcordNumber);   // 合同编号
            SetBookmarkValue("SenderName", Concord.SenderName);
            SetBookmarkValue("SenderName1", Concord.SenderName);
            SetBookmarkValue("ContractorName", InitalizeFamilyName(VirtualPerson.Name));
            SetBookmarkValue("ContractorName1", InitalizeFamilyName(VirtualPerson.Name));
            SetBookmarkValue("ContractorAddress", VirtualPerson.Address);            
            SetBookmarkValue("SenderLawyerName", Tissue.LawyerName);    //发包方负责人
            SetBookmarkValue("SenderLawyerName1", Tissue.LawyerName);
            SetBookmarkValue("SenderLawyerTelephone", Tissue.LawyerTelephone);
            SetBookmarkValue("SenderLawyerCredentNumber", Tissue.LawyerCartNumber);
          
            string vpName = VirtualPerson.Name;
            if (vpName == null || vpName == "")
                vpName = dic.translante(VirtualPerson.Name);
            SetBookmarkValue("QDDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
            int familyNumber = 0;
            Int32.TryParse(VirtualPerson.FamilyNumber, out familyNumber);
            string alloctioonPerson = VirtualPerson.FamilyExpand.AllocationPerson;
            alloctioonPerson = string.IsNullOrEmpty(alloctioonPerson) ? "  " : alloctioonPerson;

            string address = GetLandLocation();
            var date = "";
            if (Concord.ArableLandStartTime != null && Concord.ArableLandEndTime != null)
            {
                date = ((DateTime)Concord.ArableLandStartTime).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + "至"
                    + ((DateTime)Concord.ArableLandEndTime).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + "。";
            }
            SetBookmarkValue(AgricultureBookMark.ConcordTrem, Concord.Flag ? "长久" : Concord.ManagementTime + "年:");//合同期限
            SetBookmarkValue(AgricultureBookMark.ConcordDate, Concord.Flag ? "" : date);//承包时间
            SetBookmarkValue("LandPurpose", ConvertLandPurpose(GetEnumDesp<eLandPurposeType>(Concord.LandPurpose)));// 土地利用类型
            SetBookmarkValue("bmAccepterAddress", string.IsNullOrEmpty(Concord.SecondContracterLocated) ? address : Concord.SecondContracterLocated);
            SetBookmarkValue("bmLandAddress", string.IsNullOrEmpty(Concord.SecondContracterLocated) ? address : Concord.SecondContracterLocated);

            string concordStartDate = string.Empty;           
            concordStartDate = Concord.ArableLandStartTime == null ? "" : ((DateTime)Concord.ArableLandStartTime).ToLongDateString();         

            if (!string.IsNullOrEmpty(concordStartDate))
            {
                SetBookmarkValue("ContractStartTime", concordStartDate);//承包起始日期
            }
        }

        private string ConvertLandPurpose(string landPurpose)
        {
            string defaultPurpse = string.Empty;
            switch (landPurpose)
            {
                case "种植业":
                case "林业":
                case "畜牧业":
                case "渔业":
                    defaultPurpse = "农业用途";
                    break;
                case "非农业用途":
                    defaultPurpse = "非农业用途";
                    break;
                default:
                    break;
            }
            return defaultPurpse;
        }
        /// <summary>
        /// 获取土地地址
        /// </summary>
        /// <returns></returns>
        private string GetLandLocation()
        {
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone city = zoneStation.Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            if (city == null)
            {
                return CurrentZone.FullName;
            }
            string location = CurrentZone.FullName.Replace(city.FullName, "");
            city = null;
            return location;
        }

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValueStr">枚举值字符串</param>
        /// <returns></returns>
        private string GetEnumDesp<TEnum>(string enumValueStr) where TEnum : struct
        {
            TEnum tEnum = new TEnum();
            Enum.TryParse<TEnum>(enumValueStr, out tEnum);

            return EnumNameAttribute.GetDescription(tEnum);
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level)
        {
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone temp = zoneStation.Get(zoneCode);
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level);
        }

        #endregion

        #endregion
    }
}
