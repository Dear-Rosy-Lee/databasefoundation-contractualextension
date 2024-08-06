using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.StockRightShuShan
{
    public class ExportConcordWord : Table.AgricultureWordBook
    {
        private static Dictionary<string, double> m_ratioDic;    //户号和该户确权确股比例的字典

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                m_ratioDic = DataHelper.GetRatioDic(CurrentZone);
                base.OnSetParamValue(data);
                string concordTrem = Concord.Flag ? concordTrem = "长久" : Concord.ManagementTime + "年";
                var concordDate = "自" + ((DateTime)Concord.ArableLandStartTime).ToString("yyyy年MM月dd日") + "起至" + ((DateTime)Concord.ArableLandEndTime).ToString("yyyy年MM月dd日" + "止");
                SetBookmarkValue("ConcordDate", concordTrem != "长久" ? concordDate : string.Empty);
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
        protected override void WriteLandInformation()
        {
            int rowCount = LandCollection.Count - 2;
            if (rowCount > 0)
            {
                InsertTableRow(0, 2, rowCount);
            }
            int startRow = 2;
            double quaAreaTotal = 0;
            foreach (var item in LandCollection)
            {
                var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, item.ID);
                var quaArea = DataHelper.GetQuantificationArea(Contractor, item, CurrentZone, DbContext);
                quaAreaTotal += quaArea;
                SetTableCellValue(0, startRow, 0, item.Name);
                SetTableCellValue(0, startRow, 1, DataHelper.GetLandNumber(item));
                SetTableCellValue(0, startRow, 2, item.NeighborEast);
                SetTableCellValue(0, startRow, 3, item.NeighborWest);
                SetTableCellValue(0, startRow, 4, item.NeighborSouth);
                SetTableCellValue(0, startRow, 5, item.NeighborNorth);
                //var area = item.IsStockLand ? relation?.QuanficationArea : DataHelper.GetSetArea(item);

                SetTableCellValue(0, startRow, 6, quaArea.AreaFormat(2));
                SetTableCellValue(0, startRow, 7, DictList.Find(o => o.GroupCode == "C8" && o.Code == item.LandLevel).Name);
                //SetTableCellValue(0, startRow, 8, item.Comment);
                SetTableCellValue(0, startRow, 8, $"确权确股不确地，本户所占份额面积{quaArea.ToString("0.00")}亩");
                startRow++;
            }
            SetTableCellValue(0, startRow, 6, quaAreaTotal.AreaFormat(2));
            SetTableCellValue(0, startRow, 8, $"确权确股不确地，本户所占份额面积{quaAreaTotal.ToString("0.00")}亩");
        }

        private void WritSharePerson()
        {
            int startRow = 2;
            //添加行
            if (Contractor.SharePersonList.Count - 1 > 0)
            {
                InsertTableRow(1, startRow, Contractor.SharePersonList.Count - 1);
            }
            //SetTableCellValue(0, _row,4,GetComment(Contractor));//成员备注只填写第一行=>9.8取消备注
            foreach (var item in Contractor.SharePersonList)
            {
                int colum = 0;
                SetTableCellValue(0, startRow, colum++, item.Name);
                SetTableCellValue(0, startRow, colum++, DataHelper.GetGender(item));
                SetTableCellValue(0, startRow, colum++, item.Relationship);
                SetTableCellValue(0, startRow, colum++, item.ICN);
                SetTableCellValue(0, startRow, colum++, item.Comment);
                startRow++;
            }

            SetBookmarkValue("FamilyCount", Contractor.SharePersonList?.Count.ToString());
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
            var area = land.IsStockLand ? DataHelper.GetSetArea(land) : 0.00;
            var areaRatio = familyRatio * area;
            areaRatioTotal += areaRatio;
            return string.Format("确权确股不确地，本户所占份额面积{0}亩", areaRatio.ToString("0.00"));
        }
    }
}
