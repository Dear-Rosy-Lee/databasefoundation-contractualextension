/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出村组公示公告Word
    /// </summary>
    public class ExportAnnouncementWord : AgricultureWordBook
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportAnnouncementWord(IDbContext db)
        {
            dbContext = db;
        }

        #endregion

        #region Fields

        private double awareArea;//确权面积
        private double actualArea;//实测面积
        private double tableArea;//二轮总面积
        private Zone currentZone; //当前地域
        private IDbContext dbContext;
        ZoneDataBusiness zoneBusiness;

        #endregion

        #region Properties

        /// <summary>
        /// 设置村组公示公告表的时间和单位名称等信息
        /// </summary>
        public DateSetting DateSettingForAnnoucementWord { get; set; }

        /// <summary>
        /// 当前地域下的所有承包方信息
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 当前地域下的所有地块信息
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        #endregion

        #region Method

        #region Method-Override

        /// <summary>
        /// 设置参数值
        /// </summary>    
        protected override bool OnSetParamValue(object data)
        {
            if (data == null || dbContext == null)
            {
                return false;
            }
            //检查地域数据
            currentZone = data as Zone;
            if (currentZone == null)
            {
                return false;
            }
            if (ListPerson == null)
            {
                ListPerson = new List<VirtualPerson>();
            }

            //此处添加根据配置文件确定是否导出集体户信息

            //string value = ToolConfiguration.GetSpecialAppSettingValue("StaticsInformationByLandFamily", "true");
            //bool statics = true;
            //Boolean.TryParse(value, out statics);
            //if (statics)
            //{
            //    List<VirtualPerson> persons = virtualPersons.FindAll(family => (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0));
            //    if (persons != null && persons.Count > 0)
            //    {
            //        foreach (VirtualPerson person in persons)
            //        {
            //            virtualPersons.Remove(person);
            //        }
            //    }
            //}

            if (ListLand == null)
            {
                ListLand = new List<ContractLand>();
            }

            //if (statics)
            //{
            //    List<ContractLand> landArray = lands.FindAll(ld => (ld.HouseHolderName.IndexOf("机动地") >= 0 || ld.HouseHolderName.IndexOf("集体") >= 0));
            //    if (landArray != null && landArray.Count > 0)
            //    {
            //        foreach (ContractLand land in landArray)
            //        {
            //            lands.Remove(land);
            //        }
            //    }
            //}
            WriteSenderInformation();
            WriteDate();
            WriteZone();
            WriteOtherInformations();
            WriteZoneExpressBookMark();
            WriteDateInformation();
            return true;
        }

        #endregion

        #region Method-Private

        /// <summary>
        /// 写日期
        /// </summary>
        private void WriteDate()
        {
            SetBookmarkValue("CurYear",
                (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNimeric(DateSettingForAnnoucementWord.PublishStartDate.Value.Year.ToString()) : "    ");
            SetBookmarkValue("CurMonth",
                (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNumber(DateSettingForAnnoucementWord.PublishStartDate.Value.Month.ToString()) : "    ");
            SetBookmarkValue("CurDay",
                (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNumber(DateSettingForAnnoucementWord.PublishStartDate.Value.Day.ToString()) : "    ");

            SetBookmarkValue("CurYearNum",
               (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Year.ToString() : "    ");
            SetBookmarkValue("CurMonthNum",
                (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Month.ToString() : "    ");
            SetBookmarkValue("CurDayNum",
                (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Day.ToString() : "    ");



            SetBookmarkValue("EndYear",
                (DateSettingForAnnoucementWord.PublishEndDate != null && DateSettingForAnnoucementWord.PublishEndDate.HasValue) ? DateSettingForAnnoucementWord.PublishEndDate.Value.Year.ToString() : "    ");
            SetBookmarkValue("EndMonth",
                (DateSettingForAnnoucementWord.PublishEndDate != null && DateSettingForAnnoucementWord.PublishEndDate.HasValue) ? DateSettingForAnnoucementWord.PublishEndDate.Value.Month.ToString() : "    ");
            SetBookmarkValue("EndDay",
                (DateSettingForAnnoucementWord.PublishEndDate != null && DateSettingForAnnoucementWord.PublishEndDate.HasValue) ? DateSettingForAnnoucementWord.PublishEndDate.Value.Day.ToString() : "    ");
        }

        /// <summary>
        /// 写行政地域
        /// </summary>
        private void WriteZone()
        {
            string zoneName = currentZone.FullName.Replace("四川省", "");
            var index = zoneName.IndexOf("市");
            if (index > 0)
                zoneName = zoneName.Substring(index + 1);
            SetBookmarkValue("ZoneName", zoneName);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("ZoneName" + i, zoneName);
            }


            string townName = zoneName.Replace("成都市温江区", "");
            index = zoneName.IndexOf("县");
            if (index > 0)
            {
                townName = zoneName.Substring(index + 1);
            }
            SetBookmarkValue("TwonToGroupName", townName);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("TwonToGroupName" + i, townName);
            }

            string villageName = zoneName.Replace("成都市温江区万春镇", "");
            index = zoneName.IndexOf("镇");
            if (index < 0)
            {
                index = zoneName.IndexOf("乡");
            }
            if (index > 0)
            {
                villageName = zoneName.Substring(index + 1);
            }
            SetBookmarkValue("VillageAndGroupName", villageName);
        }

        /// <summary>
        /// 写其它信息
        /// </summary>
        private void WriteOtherInformations()
        {

            actualArea = Business.ToolMath.SetNumericFormat(actualArea, 4, 1);
            awareArea = Business.ToolMath.SetNumericFormat(awareArea, 4, 1);
            tableArea = Business.ToolMath.SetNumericFormat(tableArea, 4, 1);
            SetBookmarkValue("FamilyNumber", (ListPerson != null && ListPerson.Count > 0) ? ListPerson.Count.ToString() : "   ");
            SetBookmarkValue("LandNumber", (ListLand != null && ListLand.Count > 0) ? ListLand.Count.ToString() : "   ");
            double area = CalArea();
            area = ToolMath.SetNumericFormat(area, 4, 1);
            SetBookmarkValue("TotalAwareArea", area > 0 ? ToolMath.SetNumbericFormat(area.ToString(), 2) : "    ");
            SetBookmarkValue(AgricultureBookMark.AgricultureActualAreaCount, actualArea > 0 ? actualArea.ToString("0.00") : "    ");
            SetBookmarkValue(AgricultureBookMark.AgricultureAwareAreaCount, awareArea > 0 ? ToolMath.SetNumbericFormat(awareArea.ToString(), 2) : "    ");
            SetBookmarkValue(AgricultureBookMark.AgricultureTableAreaCount, tableArea > 0 ? ToolMath.SetNumbericFormat(tableArea.ToString(), 2) : "    ");
            SetBookmarkValue("RequireNameAddAddrress", DateSettingForAnnoucementWord.Address);
            SetBookmarkValue("RequireName", DateSettingForAnnoucementWord.StampUnit);
        }

        /// <summary>
        /// 计算面积
        /// </summary>
        /// <returns></returns>
        private double CalArea()
        {
            bool useActualArea = false;
            //string areaValue = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "false");
            //Boolean.TryParse(areaValue, out useActualArea);//使用实测面积作为颁证面积
            double area = 0.0;
            foreach (ContractLand land in ListLand)
            {
                area += useActualArea ? land.ActualArea : land.AwareArea;
                actualArea += land.ActualArea;
                awareArea += land.AwareArea;
                tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
            }
            return area;
        }

        /// <summary>
        /// 写地域扩展书签
        /// </summary>
        private void WriteZoneExpressBookMark()
        {
            if (currentZone == null)
            {
                return;
            }
            string zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.County);
            SetBookmarkValue("bmCountryName", zoneName);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

            }
            zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Province);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

            }
            zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.City);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), zoneName.Length > 1 ? zoneName.Substring(0, zoneName.Length - 1) : "");
            }
            zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Town);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), zoneName);
                SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
            }
            if (currentZone.Level >= eZoneLevel.Group)
            {
                zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Village);
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), zoneName);
                    SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", ""));
                }
            }
            if (currentZone.Level == eZoneLevel.Group)
            {
                zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Group);
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), zoneName);
                    string number = ToolString.GetLeftNumberWithInString(zoneName);
                    string groupName = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
                    SetBookmarkValue("LocationName" + (i == 0 ? "" : i.ToString()), currentZone.FullName);//座落
                    groupName = string.IsNullOrEmpty(number) ? zoneName : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
                    SetBookmarkValue("GroupName" + (i == 0 ? "" : i.ToString()), groupName);
                    SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
                }
            }
        }

        /// <summary>
        /// 根据地域级别获取地域名称
        /// </summary>
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level)
        {
            if (zoneBusiness == null)
            {
                zoneBusiness = new ZoneDataBusiness();
                zoneBusiness.DbContext = dbContext;
                zoneBusiness.Station = dbContext.CreateZoneWorkStation();
            }
            Zone temp = zoneBusiness.Get(zoneCode);
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level);
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        private void WriteDateInformation()
        {
            for (int i = 0; i < 6; i++)
            {
                string year = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Year.ToString() : "   ";
                SetBookmarkValue("Year" + (i == 0 ? "" : i.ToString()), year);
                string month = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Month.ToString() : "   ";
                SetBookmarkValue("Month" + (i == 0 ? "" : i.ToString()), month);
                string day = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? DateSettingForAnnoucementWord.PublishStartDate.Value.Day.ToString() : "   ";
                SetBookmarkValue("Day" + (i == 0 ? "" : i.ToString()), day);
                string fullDate = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? year + "年" + month + "月" + day + "日" : "   ";
                SetBookmarkValue("FullDate" + (i == 0 ? "" : i.ToString()), fullDate);
                year = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNimeric(DateSettingForAnnoucementWord.PublishStartDate.Value.Year.ToString()) : "   ";
                SetBookmarkValue("ChineseYear" + (i == 0 ? "" : i.ToString()), year);
                month = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNumber(DateSettingForAnnoucementWord.PublishStartDate.Value.Month.ToString()) : "   ";
                SetBookmarkValue("ChineseMonth" + (i == 0 ? "" : i.ToString()), month);
                day = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? ToolMath.GetChineseLowNumber(DateSettingForAnnoucementWord.PublishStartDate.Value.Day.ToString()) : "   ";
                SetBookmarkValue("ChineseDay" + (i == 0 ? "" : i.ToString()), day);
                fullDate = (DateSettingForAnnoucementWord.PublishStartDate != null && DateSettingForAnnoucementWord.PublishStartDate.HasValue) ? year + "年" + month + "月" + day + "日" : "   ";
                SetBookmarkValue("FullChineseDate" + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        #endregion

        #endregion
    }
}
