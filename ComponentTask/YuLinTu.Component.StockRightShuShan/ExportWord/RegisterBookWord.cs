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
using YuLinTu.Component.StockRightShuShan.Helper;
using YuLinTu.Component.StockRightShuShan.Model;

namespace YuLinTu.Component.StockRightShuShan.ExportWord
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
            return true;
        }

        private void WritePersonInfo()
        {
            int rowIndex = 17;
            if (Contractor.SharePersonList.Count > 10)
            {
                InsertTableRow(0, rowIndex, Contractor.SharePersonList.Count - 10);
            }
            foreach (var item in Contractor.SharePersonList)
            {
                SetTableCellValue(0, rowIndex, 0, item.Name);
                SetTableCellValue(0, rowIndex, 1, item.Gender.EnumToString());
                SetTableCellValue(0, rowIndex, 2, item.Relationship);
                SetTableCellValue(0, rowIndex, 3, item.ICN);
                SetTableCellValue(0, rowIndex, 4, item.Comment);
                rowIndex++;
            }
        }


        private void WriteLandInfo()
        {
            int tableIndex = 1;
            int rowIndex = 2;
            if (StockLands.Count > 1)
            {
                InsertTableRow(tableIndex, rowIndex, StockLands.Count-1);//根据地块数量插入行
            }

            foreach (var item in StockLands)
            {

                List<string> locationXY = new List<string>();
                var currentLandDots = ListLandDots.FindAll(d => d.LandID == item.ID);
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

                var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, item.ID);
                int colIndex = 0;
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.LandNumber);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.Name);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborEast);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborSouth);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborWest);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.NeighborNorth);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, coordinatesStr);
                var tableArea = DataHelper.GetTableArea(Contractor,item,CurrentZone,DbContext);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++,tableArea!=0? tableArea.AreaFormat(2):"0.00");
               
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.ActualArea.AreaFormat(2));
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, relation?.QuanficationArea.AreaFormat(2));
                awarArea += relation?.QuanficationArea;
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, DictList.Find(o=>o.GroupCode== DictionaryTypeInfo.TDYT&&o.Code==item.Purpose)?.Name);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, DictList.Find(o => o.GroupCode == DictionaryTypeInfo.DLDJ && o.Code == item.LandLevel)?.Name);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, item.LandName);
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, (bool)item.IsFarmerLand ? "是" :"否");
                SetTableCellValueDHDZ(tableIndex, rowIndex, colIndex++, "确权确股不确地");
                rowIndex++;
            }
        }


    }
}
