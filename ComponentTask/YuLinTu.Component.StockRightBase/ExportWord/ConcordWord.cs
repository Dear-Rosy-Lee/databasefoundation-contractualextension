/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.StockRightBase
{
    /// <summary>
    /// 合同
    /// </summary>
    public class ConcordWord : AgricultureWordBook
    {

        #region 泰宁导出逻辑

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
                WriteLand();
                string concordTrem = Concord.Flag ? concordTrem = "长久" : Concord.ManagementTime + "年";
                var concordDate = "自" + ((DateTime)Concord.ArableLandStartTime).ToString("yyyy年MM月dd日") + "起至" + 
                    ((DateTime)Concord.ArableLandEndTime).ToString("yyyy年MM月dd日" + "止");
                SetBookmarkValue("ConcordDate", concordDate);
                SetBookmarkValue("ConcordTrem", concordTrem);
                SetBookmarkValue("ConcordMode", Concord.ArableLandType == "110" ? "家庭承包方式" : "其他承包方式");
                SetBookmarkValue("ContractorNumber", Contractor.Number);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLand()
        {
            int rowCount = StockLands.Count - 3;
            int tableIndex = 0;
            if (rowCount > 0)
            {
                InsertTableRow(tableIndex, 2, rowCount);
            }
            int startRow = 2;
            int baseStartRow = startRow;

            var specialQuaAreaTotal = DataHelper.GetSpecialQuantificationAreaTotal(StockLands, CurrentZone, Contractor).FormatArea();
            SetBookmarkValue(SpecialBookmarks.TotalTableArea, specialQuaAreaTotal);
            var ratio = Contractor.GetFamilySendAreaRatio(CurrentZone, DbContext).FormatArea();
            SetBookmarkValue(SpecialBookmarks.TotalTableAreaRatio, ratio);
            foreach (var item in StockLands)
            {
                var startColumn = 0;
                SetTableCellValue(tableIndex, startRow, startColumn, item.Name);
                SetTableCellValue(tableIndex, startRow, ++startColumn, DataHelper.GetLandNumber(item));
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.NeighborEast);
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.NeighborWest);
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.NeighborSouth);
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.NeighborNorth);
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.GetShareLandArea().FormatArea());
                ++startColumn;    
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.GetLandLevel());
                SetTableCellValue(tableIndex, startRow, ++startColumn, item.Comment);
                startRow++;
            }

            VerticalMergeTable(tableIndex, baseStartRow, startRow-1,7);
            var shareAreaTotal = DataHelper.GetShareLandAreaTotal(StockLands,CurrentZone,Contractor).FormatArea();
            SetTableCellValue(tableIndex, baseStartRow, 7, shareAreaTotal);
        }

        #endregion
    }
}
