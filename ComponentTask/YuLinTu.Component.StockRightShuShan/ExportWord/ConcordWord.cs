/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.StockRightShuShan.ExportWord
{
    /// <summary>
    /// 合同
    /// </summary>
    public class ConcordWord : AgricultureWordBook
    {

        #region 蜀山导出逻辑

        private static Dictionary<string, double> m_ratioDic;    //户号和该户确权确股比例的字典

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                var concord = DataHelper.GetConcord(Contractor, DbContext);//获取确股合同
                this.Concord = concord != null ? concord : Concord;
                base.OnSetParamValue(data);
                m_ratioDic = DataHelper.GetRatioDic(CurrentZone);
                WriteLand();
                string concordTrem = Concord.Flag ? concordTrem = "长久" : Concord.ManagementTime + "年";
                var concordDate = "自" + ((DateTime)Concord.ArableLandStartTime).ToString("yyyy年MM月dd日") + "起至" + 
                    ((DateTime)Concord.ArableLandEndTime).ToString("yyyy年MM月dd日" + "止");
                SetBookmarkValue("ConcordDate", concordDate);
                SetBookmarkValue("ConcordTrem", concordTrem);
                SetBookmarkValue("ConcordMode", Concord.ArableLandType == "110" ? "家庭承包方式" : "其他承包方式");
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                m_ratioDic.Clear();
            }
            return true;
        }

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLand()
        {
            int rowCount = StockLands.Count - 2;
            if (rowCount > 0)
            {
                InsertTableRow(0, 2, rowCount);
            }
            int startRow = 2;
            double? areaAll = 0d;
            double? areaRatioTotal = 0;
            foreach (var item in StockLands)
            {
                var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, item.ID);
                SetTableCellValue(0, startRow, 0, item.Name);
                SetTableCellValue(0, startRow, 1, DataHelper.GetLandNumber(item));
                SetTableCellValue(0, startRow, 2, item.NeighborEast);
                SetTableCellValue(0, startRow, 3, item.NeighborWest);
                SetTableCellValue(0, startRow, 4, item.NeighborSouth);
                SetTableCellValue(0, startRow, 5, item.NeighborNorth);
                var area = GetQuaArea(item);
                areaAll += area;
                SetTableCellValue(0, startRow, 6, area != 0 ? area.AreaFormat() : "0.00");
                SetTableCellValue(0, startRow, 7, DictList.Find(o => o.GroupCode == "C8" && o.Code == item.LandLevel)?.Name);
                SetTableCellValue(0, startRow, 8, GetComment(item, ref areaRatioTotal));
                startRow++;
            }
            SetTableCellValue(0, startRow, 6, areaAll!=0?areaAll.AreaFormat():"0.00");
            var areaRatioStr = string.Format("确权确股不确地，本户所占份额面积{0}亩", areaRatioTotal.AreaFormat());
            SetTableCellValue(0, startRow, 8, areaRatioStr);
        }

        /// <summary>
        /// 获取定制化的备注信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetComment(ContractLand land, ref double? areaRatioTotal)
        {
            var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, land.ID);
            var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landStation = DbContext.CreateContractLandWorkstation();

            var vp = vpStation.Get(Concord.ContracterId.HasValue ? Concord.ContracterId.Value : new Guid());
            var familyRatio = m_ratioDic.ContainsKey(vp.FamilyNumber) ? m_ratioDic[vp.FamilyNumber] : 0.00;
            var areaRatio = familyRatio * DataHelper.GetSetArea(land);
            areaRatioTotal += areaRatio;
            return string.Format("确权确股不确地，本户所占份额面积{0}亩", areaRatio.ToString("0.00"));
        }

        /// <summary>
        /// 获取定制化的备注信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private double GetQuaArea(ContractLand land)
        {
            var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, land.ID);
            var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landStation = DbContext.CreateContractLandWorkstation();

            var vp = vpStation.Get(Concord.ContracterId.HasValue ? Concord.ContracterId.Value : new Guid());
            var familyRatio = m_ratioDic.ContainsKey(vp.FamilyNumber) ? m_ratioDic[vp.FamilyNumber] : 0.00;
            var areaRatio = familyRatio * DataHelper.GetSetArea(land);

            return areaRatio;
        }


        #endregion
    }
}
