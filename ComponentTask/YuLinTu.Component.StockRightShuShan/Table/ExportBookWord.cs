using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan
{
    public class ExportBookWord : Table.AgricultureWordBook
    {

        private const int StockLandCapcity = 5;


        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                base.OnSetParamValue(data);
                if (!string.IsNullOrEmpty(Book?.Number))
                    SetBookmarkValue(AgricultureBookMark.BookSerialNumber, Book?.Number.Substring(6, 6) + Environment.NewLine + Book?.Number.Substring(12, 6));
                var zoneNameCounty = GetZoneNameByLevel(CurrentZone.FullCode, YuLinTu.Library.Entity.eZoneLevel.County, YuLinTu.Library.Business.DataBaseSource.GetDataBaseSource().CreateZoneWorkStation());

                SetBookmarkValue("Country", zoneNameCounty);
                SetBookmarkValue("Country1", zoneNameCounty);
                //WriteStockLandInformation();

                // 该户量化面积之和
                var quaAreaCount = DataHelper.GetTotalQuantificationArea(Contractor, LandCollection, CurrentZone, DbContext);

                SetBookmarkValue("ActualAreaCount", LandCollection?.Sum(o => o.ActualArea).AreaFormat(2));
                SetBookmarkValue("QuantificationAreaCount", quaAreaCount.AreaFormat(2));
                SetBookmarkValue("AllLandCount", LandCollection?.Count.ToString());

                var lands = LandCollection?.Where(o => o.IsStockLand == false).ToList();//普通地块
                SetBookmarkValue("LandArea", lands?.Sum(o => o.ActualArea).AreaFormat(2));
                SetBookmarkValue("LandCount", lands?.Count.ToString());

                var stockLand = LandCollection?.Where(o => o.IsStockLand == true).ToList();//确股地块
                SetBookmarkValue("StockLandArea", quaAreaCount.AreaFormat(2));
                SetBookmarkValue("StockLandCount", stockLand?.Count.ToString());
                ExportExtendLands(lands, 20, false);
                ExportExtendLands(stockLand, StockLandCapcity, true);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "导出表失败", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        private void ExportExtendLands(List<ContractLand> lands, int capacity, bool isStockLand)
        {
            if (lands != null && lands.Count > capacity)
            {
                List<ContractLand> extendLands = new List<ContractLand>();
                for (int indexStart = capacity; indexStart < lands.Count; indexStart++)
                {
                    extendLands.Add(lands[indexStart]);
                }
                if (isStockLand) StockLandExtend(extendLands);
                else LandExtend(extendLands);
            }
        }


        /// <summary>
        /// 地块信息扩展处理（超出20条确权地块数据另外导出或预览）
        /// </summary>
        private void LandExtend(List<ContractLand> landCollection)
        {
            var regeditExtend = new RegeditBookLandExtend();
            regeditExtend.LandCollection = landCollection;
            regeditExtend.VirtualPerson = Contractor;
            
            regeditExtend.ProcessLandExtend(System.IO.Path.GetDirectoryName(SavedFileName));
        }

        /// <summary>
        /// 确股地块信息扩展处理
        /// </summary>
        private void StockLandExtend(List<ContractLand> landCollection)
        {
            StockLandExpressProgress landExpress = new StockLandExpressProgress();
            landExpress.CurrentZone = CurrentZone;
            landExpress.DbContext = DbContext;
            landExpress.IsFamilyMode = true;
            landExpress.BatchExport = true;
            landExpress.Contractor = Contractor;
            landExpress.LandCollection = landCollection;
            landExpress.DictList = DictList;
            landExpress.BookLandNum = StockLandCapcity;
            landExpress.BookPersonNum = 9;
            landExpress.SystemSet = SystemSet;
            landExpress.IsDataSummaryExport = false;
            landExpress.InitalizeAgriLandExpress(false, System.IO.Path.GetDirectoryName(SavedFileName));

            landCollection.Clear();
            GC.Collect();
        }

        protected override void WriteLandInformation()
        {
            WriteLandInfo(true);
        }


        private void WriteLandInfo(bool isStockLand)
        {
            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }
            var index = isStockLand ? 21 : 1;
            LandCollection = SortLandCollection(LandCollection);
            var lands = LandCollection.Where(o => o.IsStockLand == isStockLand).ToList();
            if (!isStockLand)//如果是确权普通地块形式则只打印前20条,其余的另外打印
            {
                lands = lands.Take(20).ToList();
            }
            else
            {
                lands = lands.Take(StockLandCapcity).ToList();
            }
            var landBusiness = new AccountLandBusiness(DbContext);
            var landStocks = landBusiness.GetLandCollection(CurrentZone.FullCode).Where(o => o.IsStockLand).ToList();//获取确股地块
            foreach (var land in lands)
            {
                var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, land.ID);
                SetBookmarkValue("StockArea" + index, relation?.QuanficationArea.AreaFormat(2));
                SetBookmarkValue("ShareArea" + index, land.ShareArea);
                AgricultureLandExpand expand = land.LandExpand;
                SetBookmarkValue(AgricultureBookMark.AgricultureName + index, land.Name); //地块名称
                SetBookmarkValue(AgricultureBookMark.AgricultureNumber + index, DataHelper.GetLandNumber(land)); //地块编码
                if (isStockLand)
                    SetBookmarkValue(AgricultureBookMark.AgricultureActualArea + index, landStocks.Sum(o => o.ActualArea).AreaFormat()); //实测面积
                else
                    SetBookmarkValue(AgricultureBookMark.AgricultureActualArea + index, land.ActualArea.AreaFormat());
                SetBookmarkValue(AgricultureBookMark.AgricultureAwareArea + index, land.AwareArea.AreaFormat()); //确权面积
                SetBookmarkValue(AgricultureBookMark.AgricultureTableArea + index, land.TableArea.AreaFormat()); //台帐面积
                SetBookmarkValue(AgricultureBookMark.AgricultureModoArea + index, land.MotorizeLandArea.AreaFormat()); //地块机动地面积
                InitalizeSmallNumber(index, ContractLand.GetLandNumber(land.CadastralNumber));
                var level = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DLDJ && d.Code == land.LandLevel);
                string levelString = level != null ? level.Name : "";
                levelString = levelString == "未知" ? "" : levelString;
                levelString = AgricultureSetting.UseSystemLandLevelDescription
                    ? levelString
                    : InitalizeLandLevel(land.LandLevel);
                SetBookmarkValue(AgricultureBookMark.AgricultureLandLevel + index, levelString); //等级
                string landName = !string.IsNullOrEmpty(land.LandName) ? land.LandName : "";
                if (string.IsNullOrEmpty(landName) && DictList != null)
                {
                    Dictionary lt = DictList.Find(ld => ld.Code == land.LandCode);
                    landName = lt != null ? lt.Name : "";
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureLandType + index, landName == "未知" ? "" : landName);
                //地类
                SetBookmarkValue(AgricultureBookMark.AgricultureIsFarmarLand + index,
                    (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue)
                        ? ""
                        : (land.IsFarmerLand.Value ? "是" : "否")); //是否基本农田
                SetBookmarkValue(AgricultureBookMark.AgricultureEast + index, land.NeighborEast); //东
                SetBookmarkValue(AgricultureBookMark.AgricultureEastName + index, "东:" + land.NeighborEast); //东
                SetBookmarkValue(AgricultureBookMark.AgricultureSouth + index, land.NeighborSouth); //南
                SetBookmarkValue(AgricultureBookMark.AgricultureSouthName + index, "南:" + land.NeighborSouth); //南
                SetBookmarkValue(AgricultureBookMark.AgricultureWest + index, land.NeighborWest); //西
                SetBookmarkValue(AgricultureBookMark.AgricultureWestName + index, "西:" + land.NeighborWest); //西
                SetBookmarkValue(AgricultureBookMark.AgricultureNorth + index, land.NeighborNorth); //北
                SetBookmarkValue(AgricultureBookMark.AgricultureNorthName + index, "北:" + land.NeighborNorth); //北
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighbor + index,
                    SystemSet.NergionbourSet ? InitalizeLandNeightor(land) : "见附图"); //四至
                SetBookmarkValue(AgricultureBookMark.AgricultureNeighborFigure + index, "见附图"); //四至见附图
                SetBookmarkValue(AgricultureBookMark.AgricultureComment + index, land.Comment); //地块备注
                var mode =
                    DictList.Find(d => d.GroupCode == DictionaryTypeInfo.CBJYQQDFS && d.Code == land.ConstructMode);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstructMode + index, mode != null ? mode.Name : "");
                //承包方式
                var callog = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.DKLB && d.Code == land.LandCategory);
                SetBookmarkValue(AgricultureBookMark.AgricultureConstractType + index, callog != null ? callog.Name : "");
                //地块类别
                SetBookmarkValue(AgricultureBookMark.AgriculturePlotNumber + index, land.PlotNumber); //地块畦数
                var manager = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.ManagementType);
                SetBookmarkValue(AgricultureBookMark.AgricultureManagerType + index, manager != null ? manager.Name : "");
                //地块经营方式
                SetBookmarkValue(AgricultureBookMark.AgricultureSourceFamilyName + index, land.FormerPerson); //原户主姓名
                var plant = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.GBZL && d.Code == land.PlantType);
                string plantType = plant != null ? plant.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlantType + index, plantType == "未知" ? "" : plantType);
                //耕保类型
                if (land.IsTransfer)
                {
                    var transMode =
                        DictList.Find(d => d.GroupCode == DictionaryTypeInfo.JYFS && d.Code == land.TransferType);
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterMode + index,
                        transMode != null ? transMode.Name : ""); //流转方式
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterTerm + index, land.TransferTime); //流转期限
                    SetBookmarkValue(AgricultureBookMark.AgricultureTransterArea + index,
                        land.PertainToArea > 0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : "");
                    //流转面积
                }
                var plat = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.ZZLX && d.Code == land.PlatType);
                string platType = plat != null ? plat.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePlatType + index, platType == "未知" ? "" : platType);
                //种植类型
                var purpose = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.TDYT && d.Code == land.Purpose);
                string landPurpose = purpose != null ? purpose.Name : "";
                SetBookmarkValue(AgricultureBookMark.AgriculturePurpose + index,
                    (string.IsNullOrEmpty(landPurpose) || ToolMath.MatchEntiretyNumber(landPurpose))
                        ? "种植业"
                        : landPurpose); //土地用途
                SetBookmarkValue(AgricultureBookMark.AgricultureUseSituation + index, expand.UseSituation); //土地利用情况
                SetBookmarkValue(AgricultureBookMark.AgricultureYield + index, expand.Yield); //土地产量情况
                SetBookmarkValue(AgricultureBookMark.AgricultureOutputValue + index, expand.OutputValue); //土地产值情况
                SetBookmarkValue(AgricultureBookMark.AgricultureIncomeSituation + index, expand.IncomeSituation);
                //土地收益情况
                SetBookmarkValue(AgricultureBookMark.AgricultureElevation + index, expand.Elevation.ToString()); //高程
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyPerson + index, expand.SurveyPerson); //地块调查员
                if (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureSurveyDate + index,
                        ToolDateTime.GetLongDateString(expand.SurveyDate.Value)); //地块调查日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureSurveyChronicle + index, expand.SurveyChronicle);
                //地块调查记事
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckPerson + index, expand.CheckPerson); //地块审核员
                if (expand.CheckDate != null && expand.CheckDate.HasValue)
                {
                    SetBookmarkValue(AgricultureBookMark.AgricultureCheckDate + index,
                        ToolDateTime.GetLongDateString(expand.CheckDate.Value)); //地块审核日期
                }
                SetBookmarkValue(AgricultureBookMark.AgricultureCheckOpinion + index, expand.CheckOpinion); //地块审核意见
                SetBookmarkValue(AgricultureBookMark.AgricultureImageNumber + index, expand.ImageNumber); //地块图幅号
                SetBookmarkValue(AgricultureBookMark.AgricultureFefer + index, expand.ReferPerson); //地块指界人
                index++;
            }
           
        }

        /// <summary>
        /// 填写确股地块信息
        /// </summary>
        private void WriteStockLandInformation()
        {
            WriteLandInfo(true);
        }
    }
}
