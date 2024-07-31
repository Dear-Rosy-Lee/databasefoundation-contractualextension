using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase
{
    /// <summary>
    /// 数据帮助类
    /// </summary>
    public static  class DataHelper
    {
        #region fields

        /// <summary>
        /// 数据字典集合
        /// </summary>
        private static List<Dictionary> m_dictList= DataBaseSource.GetDataBaseSource().CreateDictWorkStation().Get();
        /// <summary>
        /// 系统设置
        /// </summary>
        private static SystemSetDefine m_systemSet = SystemSetDefine.GetIntence();
        private static IDbContext m_dbContext = DataBaseSource.GetDataBaseSource();
        /// <summary>
        /// 共用面积所取面积类型
        /// </summary>
        private static eAreaType m_shareAreaType = eAreaType.ActualArea;
        /// <summary>
        /// 量化面积所取面积类型
        /// </summary>
        private static eAreaType m_QuantificationAreaType = eAreaType.TableArea;

        #endregion

        #region 家庭成员信息处理

        /// <summary>
        /// 获取家庭成员性别
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static string GetGender(Person person)
        {
            string gender = string.Empty;

            switch (person.Gender)
            {
                case eGender.Male:
                    gender= "男";
                    break;
                case eGender.Female:
                    gender= "女";
                    break;
                default:
                    gender= "未知";
                    break;
            }

            return gender;
        }

        /// <summary>
        /// 求比例，保留两位小数
        /// </summary>
        /// <param name="families">分子</param>
        /// <param name="groupPerson">分母</param>
        /// <returns>保留两位小数</returns>
        public static double GetRatio(int molecule, int denominator)
        {
            if (denominator == 0)
            {
                return 0.00;
            }

            return Math.Round((double)molecule / denominator, 2);
        }

        /// <summary>
        /// 求家庭成员数的比例的保留两位小数的字符串表示，含%
        /// </summary>
        /// <param name="families"></param>
        /// <param name="groupPerson"></param>
        /// <returns></returns>
        public static string GetRatioStr(int molecule, int denominator)
        {
            if (denominator == 0)
            {
                return "0.00";
            }

            return Math.Round((double)molecule / denominator, 2).ToString() + "%";
        }

        /// <summary>
        /// 求家庭成员数的比例
        /// </summary>
        /// <param name="families">家庭成员数</param>
        /// <param name="groupPerson">组人口数</param>
        /// <returns>保留digits位小数</returns>
        public static double GetRatio(int families, int groupPerson,int digits)
        {
            if (groupPerson == 0)
            {
                return 0.00;
            }

            return Math.Round((double)families / groupPerson, digits);
        }

        #endregion

        #region 字符数据处理

        /// <summary>
        /// 单srcStr为空或为空白时，用replacement代替
        /// </summary>
        /// <param name="replacement"></param>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        public static string Replace(string replacement, string srcStr)
        {
            if (srcStr.IsNullOrEmpty())
            {
                return replacement;
            }

            return srcStr;
        }

        /// <summary>
        /// 匹配给定的字符串是否在相应的字符串数组中
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="targetList"></param>
        /// <returns></returns>
        public static bool Match(string sourceStr, params string[] targetList)
        {
            foreach (var item in targetList)
            {
                if (item.Equals(sourceStr))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 将字符串转化为整形，转换失败时返回0
        /// </summary>
        /// <param name="intStr"></param>
        /// <returns></returns>
        public static int GetInt(string intStr)
        {
            int result = 0;

            int.TryParse(intStr, out result);

            return result;
        }

        /// <summary>
        /// 将字符串转化为double
        /// </summary>
        /// <param name="intStr"></param>
        /// <returns></returns>
        public static double? GetDouble(string doubleStr)
        {
            double? result=double.NaN;
            double temp;

            if (double.TryParse(doubleStr, out temp))
            {
                result = temp;
            }

            return result;
        }

        /// <summary>
        /// 获取格式化的面积，默认保留两位小数
        /// </summary>
        /// <param name="area">面积</param>
        /// <param name="precision">精度</param>
        /// <returns></returns>
        public static string FormatArea(this double area, int precision = 2)
        {
            return area != 0 ? area.AreaFormat(precision) :
                "0." + "0".PadRight(precision, '0');
        }

        #endregion

        #region 地块数据处理

        /// <summary>
        /// 获取定制化的备注信息,通过合同获取承包方
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetComment(ContractLand land, Dictionary<string, double> ratioDic, ContractConcord concord)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landStation = dbContext.CreateContractLandWorkstation();

            var vp = vpStation.Get(concord.ContracterId.HasValue ? concord.ContracterId.Value : new Guid());
            var familyRatio = ratioDic.ContainsKey(vp.FamilyNumber) ? ratioDic[vp.FamilyNumber] : 0.00;
            var area = land.IsStockLand?DataHelper.GetSetArea(land):0.00;
            var areaRatio = familyRatio * area;
            return string.Format("确股不确地，本户所占份额面积{0}亩", areaRatio.ToString("0.00"));
        }

        /// <summary>
        /// 获取定制化的备注信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetComment(ContractLand land, Dictionary<string, double> ratioDic, VirtualPerson contractor)
        {
            var familyRatio = ratioDic.ContainsKey(contractor.FamilyNumber) ? ratioDic[contractor.FamilyNumber] : 0.00;
            var area = land.IsStockLand ? DataHelper.GetSetArea(land) : 0.00;
            var areaRatio = familyRatio * area;
            return string.Format("确股不确地，本户所占份额面积{0}亩", areaRatio.ToString("0.00"));
        }

        /// <summary>
        /// 获取土地用途
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        public static string GetLandPurpose(this ContractLand land)
        {
            if (m_dictList == null || m_dictList.Count == 0 || land.Purpose.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var purpose = m_dictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == land.Purpose);
            string landPurpose = purpose != null ? purpose.Name : "";

            return Replace("", landPurpose);
        }

        public static double GetSetArea(ContractLand land)
        {
            switch (m_systemSet.ChooseArea)
            {
                case 0:
                    return land.TableArea.HasValue ? land.TableArea.Value : 0;//二轮面积
                case 1:
                    return land.ActualArea;//实测面积
                case 2:
                    return land.AwareArea;//确权面积
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取定制化的地块四至
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetNeighbor(ContractLand land)
        {
            string neighborEmpty = "东至：\n南至：\n西至：\n北至：";

            if (land != null)
            {
                neighborEmpty = string.Format("东至：{0}\n南至：{1}\n西至：{2}\n北至：{3}",
                    land.NeighborEast, land.NeighborSouth,
                    land.NeighborWest, land.NeighborNorth
                    );
            }

            return neighborEmpty;
        }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        public static string IsFarmLand(this ContractLand land,string replacement="")
        {
            return land.IsFarmerLand.HasValue? (land.IsFarmerLand.Value ? "是" : "否"): replacement;
        }

        /// <summary>
        /// 地力等级
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        public static string GetLandLevel(this ContractLand land)
        {
            if (land == null || land.LandLevel == null)
            {
                return string.Empty;
            }

            return GetEnumDesp<eContractLandLevel>(land.LandLevel);
        }

        /// <summary>
        /// 根据系统设置获取单户地块总面积，结果保留两位小数
        /// </summary>
        /// <param name="landCollection"></param>
        /// <returns></returns>
        public static string GetSetAreaTotal(List<ContractLand> landCollection)
        {
            double totalArea = 0;
            landCollection.ForEach(s => totalArea += DataHelper.GetSetArea(s));//根据系统设置获取相应的地块总面积

            return totalArea.ToString("0.00");
        }

        /// <summary>
        /// 根据系统配置获取截取后的地块编码
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        public static string GetLandNumber(ContractLand land)
        {
            string landNumber = land.LandNumber;
            int subStartIndex = m_systemSet.LandNumericFormatValueSet;
            bool canSubstring = m_systemSet.LandNumericFormatSet;
            if (canSubstring && landNumber.Length > subStartIndex)
            {
                landNumber = landNumber.Substring(subStartIndex);
            }

            return landNumber;
        }

        #endregion

        #region 其他数据处理

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValueStr">枚举值字符串</param>
        /// <returns></returns>
        public static string GetEnumDesp<TEnum>(string enumValueStr) where TEnum : struct
        {
            TEnum tEnum = new TEnum();
            System.Enum.TryParse<TEnum>(enumValueStr, out tEnum);

            return EnumNameAttribute.GetDescription(tEnum);
        }

        public static string GetContractDate(VirtualPerson contractor)
        {
            var contractDate = string.Empty;
            var contractStartDate = string.Empty;
            var contractEndDate = string.Empty;
            VirtualPersonExpand expand = contractor.FamilyExpand;

            if (expand != null && expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue && expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue
                && expand.ConcordStartTime.Value.Year > 1753 && expand.ConcordEndTime.Value.Year > 1753)
            {
                contractStartDate = string.Format("{0}年{1}月{2}日", expand.ConcordStartTime.Value.Year, expand.ConcordStartTime.Value.Month, expand.ConcordStartTime.Value.Day);
                contractEndDate = string.Format("{0}年{1}月{2}日", expand.ConcordEndTime.Value.Year, expand.ConcordEndTime.Value.Month, expand.ConcordEndTime.Value.Day);
            }
            if (!string.IsNullOrEmpty(contractStartDate) && !string.IsNullOrEmpty(contractEndDate))
            {
                contractDate= string.Format("{0} 至 {1}", contractStartDate, contractEndDate);    // 承包起止日期
            }

            return contractDate;
        }

        /// <summary>
        /// 根据当前地域获取地域下家庭编码和家庭占比的字典
        /// </summary>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        public static Dictionary<string, double> GetRatioDic(Zone currentZone)
        {
            Dictionary<string, double> ratioDic = new Dictionary<string, double>();

            if (currentZone != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var familise = vpStation.GetByZoneCode(currentZone.FullCode);

                if (familise != null && familise.Count > 0)
                {
                    var groupPeople = 0;
                    familise.ForEach(s => groupPeople += s.SharePersonList.Count);
                    if (groupPeople != 0)
                    {
                        familise.ForEach(s =>
                        {
                            var ratio = DataHelper.GetRatio(s.SharePersonList.Count, groupPeople, 4);
                            ratioDic.Add(s.FamilyNumber, ratio);
                        });
                    }
                }
            }

            return ratioDic;
        }

        /// <summary>
        /// 如果全是确股地获取确股合同
        /// </summary>
        /// <param name="landCollection"></param>
        /// <param name="contractor"></param>
        /// <returns></returns>
        public static StockConcord GetConcord(List<ContractLand> landList, VirtualPerson contractor, IDbContext dbContext)
        {
            if (!landList.IsNullOrEmpty() && contractor != null)
            {
                var notStockLand = landList.FindAll(s => !s.IsStockLand);//非确股地的集合

                if (notStockLand.IsNullOrEmpty())
                {
                    var stockConcordStation = dbContext.CreateStockConcordWorkStation();
                    var concordList = stockConcordStation != null ? stockConcordStation.Get() : new List<StockConcord>();
                    StockConcord concord = !concordList.IsNullOrEmpty() ? concordList.FirstOrDefault(s => s.ContracterId == contractor.ID) : null;

                    return concord;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取确股合同
        /// </summary>
        /// <param name="landCollection"></param>
        /// <param name="contractor"></param>
        /// <returns></returns>
        public static StockConcord GetConcord(VirtualPerson contractor, IDbContext dbContext)
        {
            if (contractor != null)
            {
                var stockConcordStation = dbContext.CreateStockConcordWorkStation();
                var concordList = stockConcordStation != null ? stockConcordStation.Get() : new List<StockConcord>();
                StockConcord concord = !concordList.IsNullOrEmpty() ? concordList.FirstOrDefault(s => s.ContracterId == contractor.ID) : null;

                return concord;
            }

            return null;
        }
        #endregion

        #region 确股相关逻辑

        /// <summary>
        /// 根据承包方和对应地块获取相应的量化面积
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetQuantificationArea(VirtualPerson contractor, ContractLand land, Zone currentZone, IDbContext dbContext)
        {
            if (contractor == null || land == null || currentZone == null || dbContext == null)
            {
                return 0;
            }

            var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
            var relation = belongWorkstation?.Get(s =>
                s.VirtualPersonID == contractor.ID &&
                s.LandID == land.ID &&
                s.ZoneCode == currentZone.FullCode
            ).FirstOrDefault();
            var area = relation?.QuanficationArea;

            return area ?? 0;

        }

        /// <summary>
        /// 根据承包方和对应地块获取相应的台账面积
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetTableArea(VirtualPerson contractor, ContractLand land, Zone currentZone, IDbContext dbContext)
        {
            if (contractor == null || land == null || currentZone == null || dbContext == null)
            {
                return 0;
            }

            var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
            var relation = belongWorkstation?.Get(s =>
                s.VirtualPersonID == contractor.ID &&
                s.LandID == land.ID &&
                s.ZoneCode == currentZone.FullCode
            ).FirstOrDefault();
            var area = relation?.TableArea;

            return area ?? 0;

        }

        /// <summary>
        /// 根据承包方和当前地域获取承包方的实测面积之和
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetActualAreaTotal(this VirtualPerson contractor, Zone currentZone, IDbContext dbContext)
        {
            if (contractor == null || currentZone == null || dbContext == null)
            {
                return 0;
            }

            var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
            var landStation = dbContext.CreateContractLandWorkstation();
            var relations = belongWorkstation?.Get(s =>
                s.VirtualPersonID == contractor.ID &&
                s.ZoneCode == currentZone.FullCode
            );
            var areaTotal = relations?.Sum(re => landStation.Get(re.LandID)?.ActualArea);

            return areaTotal ?? 0;

        }

        /// <summary>
        /// 获取共用地块涉及农户总数
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="currentZone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static int GetShareLandPeron(this VirtualPerson contractor, Zone currentZone, IDbContext dbContext)
        {
            if (contractor == null || currentZone == null || dbContext == null)
            {
                return 0;
            }

            var vpStockLand = contractor.GetAllStockLand(currentZone, dbContext);
            var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
            List<BelongRelation> relations = new List<BelongRelation>();
            if (vpStockLand != null && vpStockLand.Count > 0)
            {
                vpStockLand.ForEach(l => 
                relations.AddRange(
                    belongWorkstation.Get(s => s.LandID == l.ID && s.ZoneCode == l.ZoneCode)
                    ));
            }

            return relations.GroupBy(r => r.VirtualPersonID).Count();
        }

        /// <summary>
        /// 根据承包方和当前地域获取承包方的台账面积之和
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetTableAreaTotal(this VirtualPerson contractor, Zone currentZone, IDbContext dbContext)
        {
            if (contractor == null || currentZone == null || dbContext == null)
            {
                return 0;
            }

            var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
            var relations = belongWorkstation?.Get(s =>
                s.VirtualPersonID == contractor.ID &&
                s.ZoneCode == currentZone.FullCode
            );
            var areaTotal = relations?.Sum(re => re.TableArea);

            return areaTotal ?? 0;

        }

        /// <summary>
        /// 获取发包的确股地的实测面积之和
        /// </summary>
        /// <returns></returns>
        public static double GetSendActualAreaTotal(this Zone currentZone, IDbContext dbContext)
        {
            List<ContractLand> allSendStockLand = currentZone.GetAllStockLand(dbContext);

            return allSendStockLand.Sum(s => s.ActualArea);

        }

        /// <summary>
        /// 获取当前地域下的所有取股地
        /// </summary>
        /// <param name="currentZone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static List<ContractLand> GetAllStockLand(this Zone currentZone, IDbContext dbContext)
        {
            var landStation = dbContext.CreateContractLandWorkstation();
            var landList = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);

            return landList != null ? landList.FindAll(l => l.IsStockLand) : new List<ContractLand>();
        }

        /// <summary>
        /// 承包地面积（股份）总计中面积值除以发包方总面积*100%
        /// </summary>
        /// <param name=""></param>
        /// <param name="currentZone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static double GetFamilySendAreaRatio(this VirtualPerson contractor, Zone currentZone, IDbContext dbContext)
        {
            double vpAreaTotal = contractor.GetTableAreaTotal(currentZone, dbContext);
            //double sendTableAreaTotal = currentZone.GetSendTableAreaTotal(dbContext);
            double sendActualAreaTotal = currentZone.GetSendActualAreaTotal(dbContext);

            return sendActualAreaTotal!=0?(vpAreaTotal / sendActualAreaTotal) * 100:0;
        }

        /// <summary>
        /// 根据承包方和当前地域获取承包方的所有确股地块
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static List<ContractLand> GetAllStockLand(this VirtualPerson contractor, Zone currentZone, IDbContext dbContext)
        {
            List<ContractLand> stockLandList = new List<ContractLand>();
            var landStation = dbContext.CreateContractLandWorkstation();

            if (contractor != null && currentZone != null && dbContext != null)
            {
                var belongWorkstation = dbContext.CreateBelongRelationWorkStation();
                var relations = belongWorkstation?.Get(s =>
                    s.VirtualPersonID == contractor.ID &&
                    s.ZoneCode == currentZone.FullCode
                );

                relations?.ForEach(re => stockLandList.Add(landStation.Get(re.LandID)));

            }

            return stockLandList;
        }
        #endregion

        #region 南安逻辑

        /// <summary>
        /// 根据地块获取相应的量化面积,不提供承包方信息，此情况要求所有承包方相同地块的量化面积也相同
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetQuantificationArea(this ContractLand land, Zone currentZone)
        {
            if (land == null || currentZone == null)
            {
                return 0;
            }

            var belongWorkstation = m_dbContext.CreateBelongRelationWorkStation();
            var relation = belongWorkstation?.Get(s =>
                s.LandID == land.ID &&
                s.ZoneCode == currentZone.FullCode
            ).FirstOrDefault();
            var area = relation?.QuanficationArea;

            return area ?? 0;

        }

        public static double GetTableArea(this ContractLand land)
        {
            if (land != null && land.TableArea.HasValue)
            {
                return land.TableArea.Value;
            }

            return 0;
        }

        /// <summary>
        /// 根据承包方和对应地块获取相应的确股台账面积
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="land"></param>
        /// <returns></returns>
        public static double GetStockTableArea(this ContractLand land, VirtualPerson contractor, Zone currentZone)
        {
            if (contractor == null || land == null || currentZone == null || m_dbContext == null)
            {
                return 0;
            }

            var belongWorkstation = m_dbContext.CreateBelongRelationWorkStation();
            var relation = belongWorkstation?.Get(s =>
                s.VirtualPersonID == contractor.ID &&
                s.LandID == land.ID &&
                s.ZoneCode == currentZone.FullCode
            ).FirstOrDefault();
            var area = relation?.TableArea;

            return area ?? 0;

        }

        /// <summary>
        /// 获取共用地块总面积
        /// </summary>
        /// <param name="stockLandList"></param>
        /// <returns></returns>
        public static double GetShareLandAreaTotal(List<ContractLand> stockLandList, Zone currentZone = null, VirtualPerson contractor = null)
        {
            if (stockLandList == null || stockLandList.Count == 0 || currentZone == null)
            {
                return 0;
            }

            switch (m_shareAreaType)
            {
                case eAreaType.ActualArea:
                    return stockLandList.Sum(s => s.ActualArea);
                case eAreaType.QuantificationArea:
                    return stockLandList.Sum(s => s.GetQuantificationArea(currentZone));
                case eAreaType.TableArea:
                    return stockLandList.Sum(s => s.GetTableArea());
                case eAreaType.StockTableArea:
                    return stockLandList.Sum(s => s.GetStockTableArea(contractor, currentZone));
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取承包方地块量化总面积
        /// </summary>
        /// <param name="stockLandList"></param>
        /// <returns></returns>
        public static double GetSpecialQuantificationAreaTotal(List<ContractLand> stockLandList, Zone currentZone = null, VirtualPerson contractor = null)
        {
            if (stockLandList == null || stockLandList.Count == 0 || currentZone == null)
            {
                return 0;
            }

            switch (m_QuantificationAreaType)
            {
                case eAreaType.ActualArea:
                    return stockLandList.Sum(s => s.ActualArea);
                case eAreaType.QuantificationArea:
                    return stockLandList.Sum(s => s.GetQuantificationArea(currentZone));
                case eAreaType.TableArea:
                    return stockLandList.Sum(s => s.GetTableArea());
                case eAreaType.StockTableArea:
                    return stockLandList.Sum(s => s.GetStockTableArea(contractor, currentZone));
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取地块的共用面积
        /// </summary>
        /// <param name="land"></param>
        /// <param name="currentZone"></param>
        /// <param name="contractor"></param>
        /// <returns></returns>
        public static double GetShareLandArea(this ContractLand land, VirtualPerson contractor = null, Zone currentZone = null)
        {
            if (land == null)
            {
                return 0;
            }

            switch (m_shareAreaType)
            {
                case eAreaType.ActualArea:
                    return land.ActualArea;
                case eAreaType.QuantificationArea:
                    return land.GetQuantificationArea(currentZone);
                case eAreaType.TableArea:
                    return land.GetTableArea();
                case eAreaType.StockTableArea:
                    return land.GetStockTableArea(contractor, currentZone);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取特殊的量化面积
        /// </summary>
        /// <param name="land"></param>
        /// <param name="contractor"></param>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        public static double GetSpecialQuantificationArea(this ContractLand land, VirtualPerson contractor = null, Zone currentZone = null)
        {
            switch (m_QuantificationAreaType)
            {
                case eAreaType.ActualArea:
                    return land.ActualArea;
                case eAreaType.QuantificationArea:
                    return land.GetQuantificationArea(currentZone);
                case eAreaType.TableArea:
                    return land.GetTableArea();
                case eAreaType.StockTableArea:
                    return land.GetStockTableArea(contractor, currentZone);
                default:
                    return 0;
            }
        }

        private enum eAreaType
        {
            /// <summary>
            /// 台账面积
            /// </summary>
            TableArea,
            /// <summary>
            /// 实测面积
            /// </summary>
            ActualArea,
            /// <summary>
            /// 确股台账面积
            /// </summary>
            StockTableArea,
            /// <summary>
            /// 量化面积
            /// </summary>
            QuantificationArea,
        }
        #endregion
    }
}
