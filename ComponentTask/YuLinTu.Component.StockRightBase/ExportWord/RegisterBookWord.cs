/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.Component.StockRightBase.Helper;
using YuLinTu.Component.StockRightBase.Model;

namespace YuLinTu.Component.StockRightBase
{
    /// <summary>
    /// 登记簿信息
    /// </summary>
    public class RegisterBookWord : AgricultureWordBook
    {
        double? awarArea = 0d;

        protected override bool OnSetParamValue(object data)
        {
            base.OnSetParamValue(data);
            WritePersonInfo();
            WriteLandInfo();
            SetBookmarkValue("BookSerialNumber", Contractor.FamilyNumber?.PadLeft(6, '0'));
            SetBookmarkValue("ConcordMode", "家庭承包");
            SetBookmarkValue("AwareAreaCount", awarArea?.AreaFormat(2));
            SetBookmarkValue("LandCount", StockLands.Count.ToString());
            SetBookmarkValue(SpecialBookmarks.TotalTableArea, DataHelper.GetSpecialQuantificationAreaTotal(StockLands, CurrentZone, Contractor).FormatArea());
            SetBookmarkValue(SpecialBookmarks.TotalTableAreaRatio, Contractor.GetFamilySendAreaRatio(CurrentZone, DbContext).FormatArea());
            var stockLandCount = Contractor.GetAllStockLand(CurrentZone, DbContext).Count.ToString();
            SetBookmarkValue(SpecialBookmarks.StockLandCount, stockLandCount);//共用地块数量（块）
            var actualAreaTotal = DataHelper.GetShareLandAreaTotal(StockLands,CurrentZone,Contractor).FormatArea();
            SetBookmarkValue("StockLandAreaTotal", actualAreaTotal);//共用地块总面积（亩）
            var shareLandPeoples = Contractor.GetShareLandPeron(CurrentZone, DbContext).ToString();
            SetBookmarkValue("StockLandRelationPeoples", shareLandPeoples);//共用地块涉及农户总数
            SetBookmarkValue("ContractorNumber", Contractor.Number);

            return true;
        }

        private void WritePersonInfo()
        {
            int rowIndex = 20;
            int tableIndex = 0;
            if (Contractor.SharePersonList.Count > 2)
            {
                InsertTableRow(tableIndex, rowIndex, Contractor.SharePersonList.Count - 2);
            }
            foreach (var item in Contractor.SharePersonList)
            {
                SetTableCellValue(tableIndex, rowIndex, 0, item.Name);
                SetTableCellValue(tableIndex, rowIndex, 1, item.Gender.EnumToString());
                SetTableCellValue(tableIndex, rowIndex, 2, item.Relationship);
                SetTableCellValue(tableIndex, rowIndex, 3, item.ICN);
                SetTableCellValue(tableIndex, rowIndex, 4, item.Comment);
                rowIndex++;
            }
        }


        private void WriteLandInfo()
        {
            int tableIndex = 1;
            int rowIndex = 2;
            int startRow = rowIndex;
            if (StockLands.Count > 1)
            {
                InsertTableRow(tableIndex, rowIndex, StockLands.Count-1);//根据地块数量插入行
            }

            foreach (var item in StockLands)
            {
                List<string> locationXY = new List<string>();

                var dotStation = DbContext.CreateBoundaryAddressDotWorkStation();
                var currentLandDots = dotStation.GetByLandId(item.ID);
                if (currentLandDots != null && currentLandDots.Count >= 1)
                {
                    var currentLandValidDots = currentLandDots.FindAll(m => m.IsValid == true);
                    currentLandValidDots = currentLandValidDots.Count == 0 ? currentLandDots : currentLandValidDots;
                    foreach (var currentLandDot in currentLandValidDots)
                    {
                        if (currentLandDot.Shape != null)
                        {
                            var currentLandDotCdts = currentLandDot.Shape.ToCoordinates();
                            locationXY.Add( currentLandDot.DotNumber + "(" + ToolMath.CutNumericFormat(currentLandDotCdts[0].Y, 3) + "," + ToolMath.CutNumericFormat(currentLandDotCdts[0].X, 3) + ")");
                        }
                    }
                }
                var coordinatesStr = string.Join("\n", locationXY);

                int colIndex = 0;
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.LandNumber);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.Name);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborEast);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborSouth);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborWest);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborNorth);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, coordinatesStr);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.GetShareLandArea().FormatArea());//共用地块面积

                colIndex += 3;

                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, "");//量化到户股份
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.GetLandPurpose());
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.GetLandLevel());
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.LandName);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.IsFarmLand());
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.Comment);
                rowIndex++;
            }

            VerticalMergeTable(tableIndex, startRow, rowIndex - 1, 8);
            VerticalMergeTable(tableIndex, startRow, rowIndex - 1, 9);
            VerticalMergeTable(tableIndex, startRow, rowIndex - 1, 10);

            var areaTotal = DataHelper.GetShareLandAreaTotal(StockLands, CurrentZone, Contractor).FormatArea();
            SetTableCellValueDHDZ(tableIndex, startRow, 8, areaTotal);//共用地块总面积
            var quaAreaTotal = DataHelper.GetSpecialQuantificationAreaTotal(StockLands, CurrentZone, Contractor).FormatArea();
            SetTableCellValueDHDZ(tableIndex, startRow, 9, quaAreaTotal);
            var ratio = DataHelper.GetFamilySendAreaRatio(Contractor, CurrentZone, DbContext).FormatArea();
            SetTableCellValueDHDZ(tableIndex, startRow, 10, ratio);//所占比例
        }


    }
}
