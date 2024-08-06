using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan
{
    /// <summary>
    /// 承包台账
    /// </summary>
    public class ExportLandAccountWord : Table.AgricultureWordBook
    {

        protected override bool OnSetParamValue(object data)
        {
            try
            {
                SetBookmarkValue("SenderName", Tissue.Name);
                SetBookmarkValue("SenderCode", Tissue.Code);
                WritLandInfo();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "导出表失败", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        private void WritLandInfo()
        {
            int row = 4;
            var relationStation = DbContext.CreateBelongRelationWorkStation();
            Contractors = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetByZoneCode(CurrentZone.FullCode);
            LandCollectionAll = DbContext.CreateContractLandWorkstation().Get(o => o.ZoneCode == CurrentZone.FullCode);
            AccountLandFamily = new List<ContractAccountLandFamily>();
            foreach (var vp in Contractors)
            {
                var landCollection = LandCollectionAll == null ? new List<ContractLand>() : LandCollectionAll.FindAll(c => c.OwnerId == vp.ID);
                ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                accountLandFamily.CurrentFamily = vp;
                accountLandFamily.Persons = vp.SharePersonList;
                if (vp.IsStockFarmer)
                {
                    var stockLands = relationStation.GetLandByPerson(vp.ID, CurrentZone.FullCode);
                    if (stockLands.Count > 0)
                        landCollection.AddRange(stockLands);
                }
                if (landCollection.Count == 0) continue;
                accountLandFamily.LandCollection = landCollection;
                AccountLandFamily.Add(accountLandFamily);
            }

            InsertTableRow(0, row, AccountLandFamily.Count + AccountLandFamily.Sum(o => o.LandCollection.Count) - 1);
            double TotalAuaArea = 0;
            foreach (var item in AccountLandFamily)
            {
                if (item.LandCollection.Count == 0) continue;

                SetTableCellValue(0, row, 0, InitalizeContractorCode(item.CurrentFamily, CurrentZone).GetLastString(4));//承包方编码
                SetTableCellValue(0, row, 1, item.CurrentFamily?.Name);//承包方代表
                VerticalMergeTable(0, row, row + item.LandCollection.Count, 0);
                VerticalMergeTable(0, row, row + item.LandCollection.Count, 1);
                double quaAreaTotal = 0;
                foreach (var land in item.LandCollection)
                {
                    var quaArea = DataHelper.GetQuantificationArea(item.CurrentFamily, land, CurrentZone, DbContext);
                    quaAreaTotal += quaArea;
                    TotalAuaArea += quaArea;
                    SetTableCellValue(0, row, 2, land.Name);
                    SetTableCellValue(0, row, 3, DataHelper.GetLandNumber(land));
                    SetTableCellValue(0, row, 4, quaArea.AreaFormat(2));
                    SetTableCellValue(0, row, 5, quaArea.AreaFormat(2));
                    SetTableCellValue(0, row, 6, land.NeighborEast);
                    SetTableCellValue(0, row, 7, land.NeighborSouth);
                    SetTableCellValue(0, row, 8, land.NeighborWest);
                    SetTableCellValue(0, row, 9, land.NeighborNorth);
                    row++;
                }
                SetTableCellValue(0, row, 2, "合计：");
                HorizontalMergeTable(0, row, 2, 3);
                SetTableCellValue(0, row, 4, quaAreaTotal.AreaFormat(2));
                SetTableCellValue(0, row, 5, quaAreaTotal.AreaFormat(2));
                SetTableCellValue(0, row, 6, "地块数：");
                SetTableCellValue(0, row, 7, item.LandCollection.Count.ToString());
                HorizontalMergeTable(0, row, 7, 9);
                row++;
            }
            SetBookmarkValue("ConcordAreaSum", TotalAuaArea.AreaFormat(2));
            SetBookmarkValue("ActuralAreaSum", TotalAuaArea.AreaFormat(2));
            SetBookmarkValue("LandCount", LandCollectionAll.Count.ToString());
            SetBookmarkValue("contractorCount", AccountLandFamily.Count.ToString());
        }

        /// <summary>
        /// 水平合并表中单元格
        /// </summary>
        protected new void HorizontalMergeTable(int tableIndex, int rowIndex, int startColumnIndex, int endColumnIndex)
        {
            if (doc == null || builder == null)
            {
                return;
            }
            Aspose.Words.NodeCollection tables = doc.GetChildNodes(Aspose.Words.NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Aspose.Words.Tables.Table table = tables[tableIndex] as Aspose.Words.Tables.Table;
            if (table == null)
            {
                return;
            }
            try
            {
                if (rowIndex >= table.Rows.Count)
                {
                    return;
                }
                if (endColumnIndex <= startColumnIndex)
                {
                    return;
                }
                Row row = table.Rows[rowIndex];
                if (endColumnIndex > row.Cells.Count)
                {
                    return;
                }
                Cell cellStart = row.Cells[startColumnIndex];
                if (cellStart == null)
                {
                    return;
                }
                Cell cellEnd = row.Cells[endColumnIndex];
                if (cellEnd == null)
                {
                    return;
                }
                MergeCells(cellStart, cellEnd);
                //for (int index = startColumnIndex; index < endColumnIndex; index++)
                //{
                //    cell = row.Cells[index];
                //    cell.CellFormat.HorizontalMerge = CellMerge.Previous;
                //}
                cellEnd = null;
                cellStart = null;
                row = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
