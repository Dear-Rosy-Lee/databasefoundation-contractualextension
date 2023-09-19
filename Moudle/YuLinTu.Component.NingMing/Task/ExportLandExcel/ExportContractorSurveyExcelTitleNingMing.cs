using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.NingMing
{
    /// <summary>
    /// 撰写表头信息
    /// </summary>
    public partial class ExportContractorSurveyExcelNingMing
    {
        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            string titleName = "宁明县第二轮土地承包到期后再延长三十年农户共有人信息和耕作地块摸底表";

            if (!string.IsNullOrEmpty(titleName))
            {
                SetRange("A1", "P1", 32.25, 18, true, titleName);
            }
            InitalizeRangeValue("A3", "A5", "承包方编号");
            SetRange("A3", "A5", "承包方编号");
            InitalizeRangeValue("B3", "B5", "承包方名称");
            SetRange("B3", "B5", "承包方名称");
            InitalizeRangeValue("C3", "C5", "证书编号");
            SetRange("C3", "C5", "证书编号");
            SetRange("E2", "P2", 21.75, 11, false, 3, 2, "日期:" + GetDate() + "               ");
            SetRange("A2", "D2", "单位:  " + ExcelName);
            columnIndex = 3;
            columnIndex = WriteContractorTitle(columnIndex);//撰写承包方表头信息
            columnIndex = WriterAgricultureLandTitle(columnIndex);//撰写承包地块信息
        }

        /// <summary>
        /// 撰写承包方表头信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteContractorTitle(int columnIndex)
        {
            if (contractLandOutputSurveyDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
            }

            if (contractLandOutputSurveyDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证件号码", 20, true);
            }
            if (contractLandOutputSurveyDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
            }

            if (columnIndex > 3)
            {
                SetRange("D3", "F3", "家庭成员情况（含户主）", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写承包地块信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriterAgricultureLandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.LandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块名称", true);
            }
            if (contractLandOutputSurveyDefine.CadastralNumberValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块编码", 23.25, true);
            }

            if (contractLandOutputSurveyDefine.ActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测总面积", true);
            }

            if (contractLandOutputSurveyDefine.LandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "东", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "南", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "西", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "北", true);
                SetRange("K4", "N4", "四至", true);
            }
            if(contractLandOutputSurveyDefine.LandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地利用类型", true);
            }
            if (contractLandOutputSurveyDefine.CommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是（否）有错漏。附申请材料", true);
            }

            if (columnIndex > 3)
            {
                SetRange("G3", "P3", "农村土地承包经营权承包地块详细信息", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 初始化标题名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeTitleName(string titleName)
        {
            if (AgricultureSetting.UseTemplateTitle)
            {
                return InitalizeTitle() + titleName;
            }
            if (AgricultureSetting.UseTableSourceTitle)
            {
                return GetTitle() + "区、县（市）" + titleName;
            }
            return AgricultureSetting.InitalizeTitle(currentZone) + titleName;
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        private string GetTitle()
        {
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                //Zone county = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                //Zone city = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));

                AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
                if (currentZone.Level == eZoneLevel.Group)
                {
                    Zone group = alb.GetParent(currentZone);
                    Zone village = alb.GetParent(group);
                    Zone town = alb.GetParent(village);
                    Zone county = alb.GetParent(town);
                    Zone city = alb.GetParent(county);

                    if (city != null && county != null)
                    {
                        string zoneName = county.FullName.Replace(city.FullName, "");
                        return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                    }
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        private string InitalizeTitle()
        {
            AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
            Zone group = new Zone();
            Zone village = new Zone();
            Zone town = new Zone();
            Zone county = new Zone();
            Zone city = new Zone();

            if (currentZone.Level == eZoneLevel.Group)
            {
                group = currentZone;
                village = alb.GetParent(group);
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                village = currentZone;
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Town)
            {
                town = currentZone;
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone != null && currentZone.FullCode.Length > Zone.ZONE_COUNTY_LENGTH)
            {
                //Zone county = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                return county != null ? county.Name : "";
            }
            return "";
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        /// <returns></returns>
        private string GetUnitName()
        {
            AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
            Zone group = new Zone();
            Zone village = new Zone();
            Zone town = new Zone();
            Zone county = new Zone();
            Zone city = new Zone();

            if (currentZone.Level == eZoneLevel.Group)
            {
                group = currentZone;
                village = alb.GetParent(group);
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                village = currentZone;
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Town)
            {
                town = currentZone;
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                //Zone city = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                if (city != null)
                {
                    return currentZone.FullName.Replace(city.FullName, "");
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        /// <param name="code"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        //private string GetZoneName(string code, eZoneLevel level)
        //{
        //    Zone tempZone = DB.Zone.Get(code);
        //    if (tempZone.Level != level)
        //    {
        //        return GetZoneName(tempZone.UpLevelCode, level);
        //    }
        //    return tempZone.Name;
        //}

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
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
    }
}