using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan
{
    public class ExportCurrentSituationSurveyWord : Table.AgricultureWordBook
    {
        /// <summary>
        /// true 不确地 false 不确界
        /// </summary>
        public bool IsNotLand
        {
            get; set;
        }


        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                //WritLandInfo();
                base.OnSetParamValue(data);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "导出表失败", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 设置地块
        /// </summary>
        protected override void WriteLandInformation()
        {
            int row = 3;
            //CopyTable(0, row, 10, 10,10);
            //添加行
            //if (LandCollection.Count - 1 > 0)
            //{
            if (LandCollection.Count > 1)
                InsertTableRow(0, row, LandCollection.Count - 3);
            //}

            foreach (var item in LandCollection)
            {
                //if (_titleInRowList.Where(o => o == row).Count() > 0)//跳过表头行
                //{
                //    row++;
                //    continue;
                //}
                //var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, item.ID);
                var quaArea = DataHelper.GetQuantificationArea(Contractor, item, CurrentZone, DbContext);
                var tableArea = DataHelper.GetTableArea(Contractor, item, CurrentZone, DbContext);
                int colBase = 0;
                SetTableCellValue(0, row, colBase++, item.Name);
                SetTableCellValue(0, row, colBase++, item.LandNumber.GetLastString(5));
                SetTableCellValue(0, row, colBase++, item.NeighborEast);
                SetTableCellValue(0, row, colBase++, item.NeighborSouth);
                SetTableCellValue(0, row, colBase++, item.NeighborWest);
                SetTableCellValue(0, row, colBase++, item.NeighborNorth);
                SetTableCellValue(0, row, colBase++, item.ActualArea.AreaFormat(2));
                SetTableCellValue(0, row, colBase++, tableArea.AreaFormat(2));
                SetTableCellValue(0, row, colBase++, quaArea.AreaFormat(2));
                SetTableCellValue(0, row, colBase++, TDYT.Find(o => o.Code == item.Purpose)?.Name);
                SetTableCellValue(0, row, colBase++, DLDJ.Find(o => o.Code == item.LandLevel)?.Name);
                SetTableCellValue(0, row, colBase++, item.LandName);
                SetTableCellValue(0, row, colBase++, item.IsFarmerLand != null && (bool)item.IsFarmerLand ? "是" : "否");
                SetTableCellValue(0, row, colBase, "");
                row++;
            }
            VerticalMergeTable(0, 3, row - 1, 14);
            _titleInRowList.Clear();
        }
    }
}
