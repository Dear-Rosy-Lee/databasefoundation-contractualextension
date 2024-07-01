/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据处理类
    /// 1.从指定数据库中读取数据
    /// 2.将读取的数据写入指定数据库中
    /// </summary>
    public class DataProcess
    {
        #region Delegate

        /// <summary>
        /// 报告信息委托方法声明
        /// </summary>
        public delegate void ReportInformationDelegate(eMessageGrade errortype, string msg);

        /// <summary>
        /// 报告信息委托属性
        /// </summary>
        public ReportInformationDelegate ReportInfo { get; set; }

        #endregion Delegate

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataProcess()
        {
        }

        #endregion Ctor



        #region Method

        /// <summary>
        /// 行政地域数据处理
        /// </summary>
        /// <param name="zones">待处理的行政地域集合(读取出来的数据)</param>
        /// <param name="allZonesLocal">对比行政地域集合(待写入数据库中已存在的数据)</param>
        /// <param name="createWsLocal">待写入数据业务逻辑</param>
        /// <returns></returns>
        public bool ZoneDataProcess(List<Zone> zones, List<Zone> allZonesLocal, IDbContext dbContextLocal, CreateWorkStation createWsLocal)
        {
            try
            {
                dbContextLocal.BeginTransaction();
                foreach (var zone in zones)
                {
                    if (allZonesLocal.Any(c => !string.IsNullOrEmpty(c.FullCode) && c.FullCode == zone.FullCode))
                        continue;
                    createWsLocal.ZoneStation.Add(zone);
                }
                dbContextLocal.CommitTransaction();
            }
            catch
            {
                dbContextLocal.RollbackTransaction();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 调查宗地数据处理
        /// </summary>
        /// <param name="createWsTarget">待读取数据业务逻辑</param>
        /// <param name="createWsLocal">待写入数据业务逻辑</param>
        /// <returns></returns>
        public bool SurveyLandDataProcess(CreateWorkStation createWsTarget,
            CreateWorkStation createWsLocal, bool isCoverDataByZoneLevel, bool isCombination = true)
        {
            try
            {
                int count = -1;
                var listSurveyLandTarget = createWsTarget.SurveyLandStation.Get();
                if (listSurveyLandTarget != null && listSurveyLandTarget.Count > 0)
                {
                    var listSurveyLandLocal = createWsLocal.SurveyLandStation.Get();
                    if (listSurveyLandLocal == null || listSurveyLandLocal.Count == 0 || (listSurveyLandLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.SurveyLandStation.AddRange(listSurveyLandLocal);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("调查宗地数据合并失败") : string.Format("调查宗地数据分离失败");
                        if (ReportInfo != null)
                            ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("共合并{0}条调查宗地数据", listSurveyLandTarget.Count) :
                            string.Format("共分离{0}条调查宗地数据", listSurveyLandTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  除行政地域、调查宗地以外的其他业务数据处理
        /// </summary>
        /// <param name="zone">待处理行政地域</param>
        /// <param name="createWsTarget">待读取数据业务逻辑</param>
        /// <param name="createWsLocal">待写入数据业务逻辑</param>
        /// <returns></returns>
        public bool OtherDataProcessOld(Zone zone, CreateWorkStation createWsTarget,
            CreateWorkStation createWsLocal, bool isCoverDataByZoneLevel, bool isCombination = true)
        {
            try
            {
                int count = -1;
                var listTissueTarget = createWsTarget.SenderStation.GetTissues(zone.FullCode, eLevelOption.Self);
                if (listTissueTarget != null && listTissueTarget.Count > 0)
                {
                    var listTissueLocal = createWsLocal.SenderStation.GetTissues(zone.FullCode, eLevelOption.Self);
                    if (listTissueLocal.Count > 0)
                    {
                        if (isCoverDataByZoneLevel)
                        {
                            createWsLocal.SenderStation.DeleteByZoneCode(zone.FullCode, eLevelOption.Self);
                        }
                        else
                        {
                        }
                    }
                    if (listTissueLocal == null || listTissueLocal.Count == 0)
                    {
                        count = createWsLocal.SenderStation.AddRange(listTissueTarget);
                    }
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}发包方数据合并失败", zone.FullName) :
                                                      string.Format("{0}发包方数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条发包方数据", zone.FullName, listTissueTarget.Count) :
                                                      string.Format("{0}共分离{1}条发包方数据", zone.FullName, listTissueTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }
                else
                {
                    string info = isCombination ? string.Format("{0}未获取发包方业务数据,忽略合并", zone.FullName) :
                                                  string.Format("{0}未获取发包方业务数据,忽略分离", zone.FullName);
                    ReportInfo(eMessageGrade.Warn, info);
                    return true;
                }

                var listPersonTarget = createWsTarget.LandVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listPersonTarget != null && listPersonTarget.Count > 0)
                {
                    var listPersonLocal = createWsLocal.LandVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                    if (listPersonLocal == null || listPersonLocal.Count == 0 || (listPersonLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.LandVirtualPersonStation.AddRange(listPersonTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}承包方数据合并失败", zone.FullName) :
                                                      string.Format("{0}承包方数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条承包方数据", zone.FullName, listPersonTarget.Count) :
                                                      string.Format("{0}共分离{1}条承包方数据", zone.FullName, listPersonTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }
                else
                {
                    string info = isCombination ? string.Format("{0}未获取承包方业务数据,忽略合并", zone.FullName) :
                                                  string.Format("{0}未获取承包方业务数据,忽略分离", zone.FullName);
                    ReportInfo(eMessageGrade.Warn, info);
                    return true;
                }

                var listLandTarget = createWsTarget.ContractLandStation.GetCollection(zone.FullCode, eLevelOption.Self);
                if (listLandTarget != null && listLandTarget.Count > 0)
                {
                    var listLandLocal = createWsLocal.ContractLandStation.GetCollection(zone.FullCode, eLevelOption.Self);
                    if (listLandLocal == null || listLandLocal.Count == 0 || (listLandLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.ContractLandStation.AddRange(listLandTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}承包地块数据合并失败", zone.FullName) :
                                                       string.Format("{0}承包地块数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条承包地块数据", zone.FullName, listLandTarget.Count) :
                                                      string.Format("{0}共分离{1}条承包地块数据", zone.FullName, listLandTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listConcordTarget = createWsTarget.ConcordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listConcordTarget != null && listConcordTarget.Count > 0)
                {
                    var listConcordLocal = createWsLocal.ConcordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                    if (listConcordLocal == null || listConcordLocal.Count == 0 || (listConcordLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.ConcordStation.AddRange(listConcordTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}承包合同数据合并失败", zone.FullName) :
                                                       string.Format("{0}承包合同数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条承包合同数据", zone.FullName, listConcordTarget.Count) :
                                                       string.Format("{0}共分离{1}条承包合同数据", zone.FullName, listConcordTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listBookTarget = createWsTarget.BookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listBookTarget != null && listBookTarget.Count > 0)
                {
                    var listBookLocal = createWsLocal.BookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                    if (listBookLocal == null || listBookLocal.Count == 0 || (listBookLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.BookStation.AddRange(listBookTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}承包权证数据合并失败", zone.FullName) :
                                                      string.Format("{0}承包权证数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条承包权证数据", zone.FullName, listBookTarget.Count) :
                                                      string.Format("{0}共分离{1}条承包权证数据", zone.FullName, listBookTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listSecondTableTarget = createWsTarget.SecondTableStation.GetCollection(zone.FullCode, eLevelOption.Self);
                if (listSecondTableTarget != null && listSecondTableTarget.Count > 0)
                {
                    var listSecondTableLocal = createWsLocal.SecondTableStation.GetCollection(zone.FullCode, eLevelOption.Self);
                    if (listSecondTableLocal == null || listSecondTableLocal.Count == 0 || (listSecondTableLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.SecondTableStation.AddRange(listSecondTableTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}二轮台账地块数据合并失败", zone.FullName) :
                                                       string.Format("{0}二轮台账地块数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条二轮台账地块数据", zone.FullName, listSecondTableTarget.Count) :
                                                       string.Format("{0}共分离{1}条二轮台账地块数据", zone.FullName, listSecondTableTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listSecondPersonTarget = createWsTarget.SecondPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listSecondPersonTarget != null && listSecondPersonTarget.Count > 0)
                {
                    var listSecondPersonLocal = createWsLocal.SecondPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                    if (listSecondPersonLocal == null || listSecondPersonLocal.Count == 0 || (listSecondPersonLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.SecondPersonStation.AddRange(listSecondPersonTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}二轮台账承包方数据合并失败", zone.FullName) :
                                                       string.Format("{0}二轮台账承包方数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条二轮台账承包方数据", zone.FullName, listSecondPersonTarget.Count) :
                                                       string.Format("{0}共分离{1}条二轮台账承包方数据", zone.FullName, listSecondPersonTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listDotTarget = createWsTarget.DotStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                if (listDotTarget != null && listDotTarget.Count > 0)
                {
                    var listDotLocal = createWsLocal.DotStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                    if (listDotLocal == null || listDotLocal.Count == 0 || (listDotLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.DotStation.AddRange(listDotTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}界址点数据合并失败", zone.FullName) :
                                                       string.Format("{0}界址点数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条界址点数据", zone.FullName, listDotTarget.Count) :
                                                       string.Format("{0}共分离{1}条界址点数据", zone.FullName, listDotTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listCoilTarget = createWsTarget.CoilStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                if (listCoilTarget != null && listCoilTarget.Count > 0)
                {
                    var listCoilLocal = createWsLocal.CoilStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                    if (listCoilLocal == null || listCoilLocal.Count == 0 || (listCoilLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.CoilStation.AddRange(listCoilTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}界址线数据合并失败", zone.FullName) :
                                                      string.Format("{0}界址线数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条界址线数据", zone.FullName, listCoilTarget.Count) :
                                                       string.Format("{0}共分离{1}条界址线数据", zone.FullName, listCoilTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listRequireTableTarget = createWsTarget.RequireTableStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                if (listRequireTableTarget != null && listRequireTableTarget.Count > 0)
                {
                    var listRequireTableLocal = createWsLocal.RequireTableStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                    if (listRequireTableLocal == null || listRequireTableLocal.Count == 0 || (listRequireTableLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.RequireTableStation.AddRange(listRequireTableTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}申请登记簿数据合并失败", zone.FullName) :
                                                       string.Format("{0}申请登记簿数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条申请登记簿数据", zone.FullName, listRequireTableTarget.Count) :
                                                       string.Format("{0}共分离{1}条申请登记簿数据", zone.FullName, listRequireTableTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listFarmLandConserveTarget = createWsTarget.FarmLandConserveStation.GetByZoneCode(zone.FullCode);
                if (listFarmLandConserveTarget != null && listFarmLandConserveTarget.Count > 0)
                {
                    var listFarmLandConserveLocal = createWsLocal.FarmLandConserveStation.GetByZoneCode(zone.FullCode);
                    if (listFarmLandConserveLocal == null || listFarmLandConserveLocal.Count == 0 || (listFarmLandConserveLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.FarmLandConserveStation.AddRange(listFarmLandConserveTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}基本农田保护区数据合并失败", zone.FullName) :
                                                       string.Format("{0}基本农田保护区数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条基本农田保护区数据", zone.FullName, listFarmLandConserveTarget.Count) :
                                                      string.Format("{0}共分离{1}条基本农田保护区数据", zone.FullName, listFarmLandConserveTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listPointTarget = createWsTarget.PointStation.GetByZoneCode(zone.FullCode);
                if (listPointTarget != null && listPointTarget.Count > 0)
                {
                    var listPointLocal = createWsLocal.PointStation.GetByZoneCode(zone.FullCode);
                    if (listPointLocal == null || listPointLocal.Count == 0 || (listPointLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.PointStation.AddRange(listPointTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}点状地物数据合并失败", zone.FullName) :
                                                      string.Format("{0}点状地物数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条点状地物数据", zone.FullName, listPointTarget.Count) :
                                                       string.Format("{0}共分离{1}条点状地物数据", zone.FullName, listPointTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listLineTarget = createWsTarget.LineStation.GetByZoneCode(zone.FullCode);
                if (listLineTarget != null && listLineTarget.Count > 0)
                {
                    var listLineLocal = createWsLocal.LineStation.GetByZoneCode(zone.FullCode);
                    if (listLineLocal == null || listLineLocal.Count == 0 || (listLineLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.LineStation.AddRange(listLineTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}线状地物数据合并失败", zone.FullName) :
                                                        string.Format("{0}线状地物数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条线状地物数据", zone.FullName, listLineTarget.Count) :
                                                       string.Format("{0}共分离{1}条线状地物数据", zone.FullName, listLineTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listPolygonTarget = createWsTarget.PolygonStation.GetByZoneCode(zone.FullCode);
                if (listPolygonTarget != null && listPolygonTarget.Count > 0)
                {
                    var listPolygonLocal = createWsLocal.PolygonStation.GetByZoneCode(zone.FullCode);
                    if (listPolygonLocal == null || listPolygonLocal.Count == 0 || (listPolygonLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.PolygonStation.AddRange(listPolygonTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}面状地物数据合并失败", zone.FullName) :
                                                     string.Format("{0}面状地物数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条面状地物数据", zone.FullName, listPolygonTarget.Count) :
                                                     string.Format("{0}共分离{1}条面状地物数据", zone.FullName, listPolygonTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listControlPointTarget = createWsTarget.ControlPointStation.GetByZoneCode(zone.FullCode);
                if (listControlPointTarget != null && listControlPointTarget.Count > 0)
                {
                    var listControlPointLocal = createWsLocal.ControlPointStation.GetByZoneCode(zone.FullCode);
                    if (listControlPointLocal == null || listControlPointLocal.Count == 0 || (listControlPointLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.ControlPointStation.AddRange(listControlPointTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}控制点数据合并失败", zone.FullName) :
                                                       string.Format("{0}控制点数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条控制点数据", zone.FullName, listControlPointTarget.Count) :
                                                       string.Format("{0}共分离{1}条控制点数据", zone.FullName, listControlPointTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }

                var listZoneBoundaryTarget = createWsTarget.ZoneBoundaryStation.GetByZoneCode(zone.FullCode);
                if (listZoneBoundaryTarget != null && listZoneBoundaryTarget.Count > 0)
                {
                    var listZoneBoundaryLocal = createWsLocal.ZoneBoundaryStation.GetByZoneCode(zone.FullCode);
                    if (listZoneBoundaryLocal == null || listZoneBoundaryLocal.Count == 0 || (listZoneBoundaryLocal.Count > 0 && isCoverDataByZoneLevel))
                        count = createWsLocal.ZoneBoundaryStation.AddRange(listZoneBoundaryTarget);
                    if (count < 0)
                    {
                        string info = isCombination ? string.Format("{0}区域界线数据合并失败", zone.FullName) :
                                                        string.Format("{0}区域界线数据分离失败", zone.FullName);
                        ReportInfo(eMessageGrade.Warn, info);
                    }
                    else
                    {
                        string info = isCombination ? string.Format("{0}共合并{1}条区域界线数据", zone.FullName, listZoneBoundaryTarget.Count) :
                                                       string.Format("{0}共分离{1}条区域界线数据", zone.FullName, listZoneBoundaryTarget.Count);
                        ReportInfo(eMessageGrade.Infomation, info);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  除行政地域、调查宗地以外的其他业务数据处理
        /// </summary>
        /// <param name="zone">待处理行政地域</param>
        /// <param name="createWsTarget">待读取数据业务逻辑</param>
        /// <param name="createWsLocal">待写入数据业务逻辑</param>
        /// <returns></returns>
        public bool OtherDataProcess(Zone zone, CreateWorkStation createWsTarget,
            CreateWorkStation createWsLocal, bool isCoverDataByZoneLevel, bool isCombination = true)
        {
            try
            {
                int result;
                result = BatchImportData<CollectivityTissue>("发包方", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.SenderStation.GetTissues(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.SenderStation.GetTissues(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.SenderStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.SenderStation.AddRange(list); });
                if (result == 0)
                    return true;

                result = BatchImportData<VirtualPerson>("承包方", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.LandVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.LandVirtualPersonStation.GetByZoneCode(zone.FullCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.LandVirtualPersonStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.LandVirtualPersonStation.AddRange(list); });
                if (result == 0)
                    return true;

                BatchImportData<ContractLand>("承包地块", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.ContractLandStation.GetCollection(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ContractLandStation.GetCollection(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ContractLandStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.ContractLandStation.AddRange(list); });

                BatchImportData<BelongRelation>("权属关系", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                   (zoneCode) => { return createWsTarget.BelongRelationStation.GetdDataByZoneCode(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.BelongRelationStation.GetdDataByZoneCode(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.BelongRelationStation.DeleteByZone(zoneCode, eLevelOption.Self); },
                   (list) => { return createWsLocal.BelongRelationStation.AddRang(list); });

                BatchImportData<ContractConcord>("承包合同", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.ConcordStation.GetContractsByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ConcordStation.GetContractsByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ConcordStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.ConcordStation.AddRange(list); });

                BatchImportData<ContractRegeditBook>("承包权证", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.BookStation.GetContractsByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.BookStation.GetContractsByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.BookStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.BookStation.AddRange(list); });

                BatchImportData<StockConcord>("确股合同", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.StockConcordWorkStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.StockConcordWorkStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.StockConcordWorkStation.DeleteByZone(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.StockConcordWorkStation.Add(list); });

                BatchImportData<StockWarrant>("确股权证", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.StockWarrantWorkStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.StockWarrantWorkStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.StockWarrantWorkStation.DeleteByZone(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.StockWarrantWorkStation.Add(list); });

                //BatchImportData<SecondTableLand>("二轮台账地块", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                //    (zoneCode) => { return createWsTarget.SecondTableStation.GetCollection(zoneCode, eLevelOption.Self); },
                //    (zoneCode) => { return createWsLocal.SecondTableStation.GetCollection(zoneCode, eLevelOption.Self); },
                //    (zoneCode) => { return createWsLocal.SecondTableStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                //    (list) => { return createWsLocal.SecondTableStation.AddRange(list); }, false);

                //BatchImportData<VirtualPerson>("二轮台账承包方", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                //    (zoneCode) => { return createWsTarget.SecondPersonStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                //    (zoneCode) => { return createWsLocal.SecondPersonStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                //    (zoneCode) => { return createWsLocal.SecondPersonStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                //    (list) => { return createWsLocal.SecondPersonStation.AddRange(list); }, false);

                BatchImportData<BuildLandBoundaryAddressDot>("界址点", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.DotStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.DotStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.DotStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.DotStation.AddRange(list); }, false);

                BatchImportData<BuildLandBoundaryAddressCoil>("界址线", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.CoilStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.CoilStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.CoilStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.CoilStation.AddRange(list); }, false);

                BatchImportData<ContractRequireTable>("申请登记簿", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.RequireTableStation.GetByZoneCode(zoneCode, eSearchOption.Precision); },
                    (zoneCode) => { return createWsLocal.RequireTableStation.GetByZoneCode(zoneCode, eSearchOption.Precision); },
                    (zoneCode) => { return createWsLocal.RequireTableStation.DeleteByZoneCode(zoneCode, eSearchOption.Precision); },
                    (list) => { return createWsLocal.RequireTableStation.AddRange(list); }, false);

                BatchImportData<FarmLandConserve>("基本农田保护区", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.FarmLandConserveStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.FarmLandConserveStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.FarmLandConserveStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.FarmLandConserveStation.AddRange(list); }, false);

                BatchImportData<DZDW>("点状地物", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.PointStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.PointStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.PointStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.PointStation.AddRange(list); }, false);

                BatchImportData<XZDW>("线状地物", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.LineStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.LineStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.LineStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.LineStation.AddRange(list); }, false);

                BatchImportData<MZDW>("面状地物", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.PolygonStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.PolygonStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.PolygonStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.PolygonStation.AddRange(list); }, false);

                BatchImportData<ControlPoint>("控制点", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.ControlPointStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.ControlPointStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.ControlPointStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.ControlPointStation.AddRange(list); }, false);

                BatchImportData<ZoneBoundary>("区域界线", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.ZoneBoundaryStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.ZoneBoundaryStation.GetByZoneCode(zoneCode); },
                    (zoneCode) => { return createWsLocal.ZoneBoundaryStation.Delete(c => c.ZoneCode == zoneCode); },
                    (list) => { return createWsLocal.ZoneBoundaryStation.AddRange(list); }, false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 批量处理数据(读取数据和写入数据)
        /// </summary>
        /// <param name="isKeyBus">是否是主要业务（承包方、承包台账、承包合同、承包权证）</param>
        public int BatchImportData<T>(string strType, string zoneCode, string zoneName, bool isCoverDataByZoneLevel, bool isCombination,
            Func<string, List<T>> getsourceData, Func<string, List<T>> getLocalData, Func<string, int> deleteData, Func<List<T>, int> addData, bool isKeyBus = true)
        {
            int count = -1;
            int delCount = 0;
            List<T> soureList = getsourceData(zoneCode);
            if (soureList != null && soureList.Count > 0)
            {
                var listTissueLocal = getLocalData(zoneCode);
                if (listTissueLocal.Count > 0)
                {
                    if (isCoverDataByZoneLevel)
                    {
                        delCount = deleteData(zoneCode);
                    }
                    else
                    {
                        string info = string.Format("{0}存在{1}数据且不需要覆盖,忽略合并", zoneName, strType);
                        ReportInfo(eMessageGrade.Warn, info);
                        return 1;
                    }
                }
                if (listTissueLocal == null || listTissueLocal.Count == 0 || delCount > 0)
                {
                    count = addData(soureList);
                }
                if (count < 0)
                {
                    string info = isCombination ? string.Format("{0{1}数据合并失败", zoneName, strType) :
                                                  string.Format("{0}{1}数据分离失败", zoneName, strType);
                    ReportInfo(eMessageGrade.Warn, info);
                    return -1;
                }
                else
                {
                    string info = isCombination ? string.Format("{0}共合并{1}条{2}数据", zoneName, soureList.Count, strType) :
                                                  string.Format("{0}共分离{1}条{2}数据", zoneName, soureList.Count, strType);
                    ReportInfo(eMessageGrade.Infomation, info);
                    return 1;
                }
            }
            else
            {
                string info = isCombination ? string.Format("{0}未获取{1}业务数据,忽略合并", zoneName, strType) :
                                              string.Format("{0}未获取{1}业务数据,忽略分离", zoneName, strType);
                if (isKeyBus)
                    ReportInfo(eMessageGrade.Warn, info);
                else
                    ReportInfo(eMessageGrade.Infomation, info);
                return 0;
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <returns></returns>
        public bool ImportData<T>(string zoneCode, List<T> enList, IWorkstation station, IDbContext db)
        {
            if (db == null)
                return false;
            if (enList == null || enList.Count == 0)
                return true;
            try
            {
                db.BeginTransaction();
                foreach (var item in enList)
                {
                    station.Add(item);
                }
                db.CommitTransaction();
                return true;
            }
            catch
            {
                db.RollbackTransaction();
                return false;
            }
        }

        #endregion Method
    }
}