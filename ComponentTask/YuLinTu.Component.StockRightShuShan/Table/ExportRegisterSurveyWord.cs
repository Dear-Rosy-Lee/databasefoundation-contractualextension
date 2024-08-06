using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightShuShan
{
    public class ExportRegisterSurveyWord : Table.AgricultureWordBook
    {
        int _row = 9;
        private Dictionary<string, double> m_ratio;

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            try
            {
                var concord = DataHelper.GetConcord(LandCollection, Contractor, DbContext);//获取确股合同
                this.Concord = concord != null ? concord : Concord;
                m_ratio = DataHelper.GetRatioDic(CurrentZone);
                base.OnSetParamValue(data);
                WriteOtherInfo();
                //WritLandInfo();//先写地块再写人
                WritSharePerson();
                Disponse();
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
        /// 填写其他信息
        /// </summary>
        private void WriteOtherInfo()
        {
            if (LandCollection == null || LandCollection.Count == 0)
            {
                return;
            }

            VirtualPersonExpand expand = Contractor.FamilyExpand;

            SetBookmarkValue("ContractorIdentifyNumber", Contractor.Number);//承包方证件号码
            SetBookmarkValue("SenderLawyerName", Tissue.LawyerName);//发包方负责人
            SetBookmarkValue("ContractorTelephone", Contractor.Telephone);//联系电话
            SetBookmarkValue("PostNumber", Contractor.PostalNumber);//邮政编码
            //从承包方中获取
            SetBookmarkValue("ConcordNumber", string.IsNullOrEmpty(expand?.ConcordNumber) ? Concord.ConcordNumber : expand.ConcordNumber);//合同编号
            SetBookmarkValue("BookWarrantNumber", string.IsNullOrEmpty(expand?.WarrantNumber) ? Concord.ConcordNumber : expand.WarrantNumber);//权证编号
            var startAndEndDate = DataHelper.GetContractDate(Contractor);
            if (string.IsNullOrEmpty(startAndEndDate)) startAndEndDate = DataHelper.GetContractDate(Concord);
            SetBookmarkValue("ConcordDate", startAndEndDate);    // 承包起止日期


            SetBookmarkValue("PersonCount", Contractor.SharePersonList.Count.ToString());//家庭成员总数

            bool isAllStock = LandCollection.FindAll(s => s.IsStockLand).Count == LandCollection.Count ? true : false;//是否全是确股地
            bool isAllNotStock = LandCollection.FindAll(s => s.IsStockLand).Count == 0 ? true : false;//是否全是确权地

            // 取得承包方式
            eConstructMode mode = (eConstructMode)expand.ConstructMode;
            if (isAllStock)//全是确股地时只勾选“其他确权确股不确地”
            {
                SetBookmarkValue("OtherStock", "R");//其他确权确股不确地
            }
            else if (isAllNotStock)//全是确权地时只勾选“其他确权确股不确地”以外的选项
            {
                switch (mode)
                {
                    case eConstructMode.Consensus:
                        SetBookmarkValue(AgricultureBookMark.ConsensusContract + "SP", "R");//公开协商
                        break;
                    case eConstructMode.Exchange:
                        SetBookmarkValue(AgricultureBookMark.ExchangeContract + "SP", "R");//互换
                        break;
                    case eConstructMode.Family:
                        SetBookmarkValue(AgricultureBookMark.FamilyContract + "SP", "R");//家庭承包
                        break;
                    case eConstructMode.Other:
                    case eConstructMode.OtherContract:
                        SetBookmarkValue(AgricultureBookMark.OtherContract + "SP", "R");//其他确权确股不确地
                        break;
                    case eConstructMode.Tenderee:
                        SetBookmarkValue(AgricultureBookMark.TendereeContract + "SP", "R");//招标
                        break;
                    case eConstructMode.Transfer:
                        SetBookmarkValue(AgricultureBookMark.TransferContract + "SP", "R");//转让
                        break;
                    case eConstructMode.Vendue:
                        SetBookmarkValue(AgricultureBookMark.VendueContract + "SP", "R");//拍卖
                        break;
                    default:
                        break;
                }
            }
            else
            {
                SetBookmarkValue("OtherStock", "R");//其他确权确股不确地
                switch (mode)
                {
                    case eConstructMode.Consensus:
                        SetBookmarkValue(AgricultureBookMark.ConsensusContract + "SP", "R");//公开协商
                        break;
                    case eConstructMode.Exchange:
                        SetBookmarkValue(AgricultureBookMark.ExchangeContract + "SP", "R");//互换
                        break;
                    case eConstructMode.Family:
                        SetBookmarkValue(AgricultureBookMark.FamilyContract + "SP", "R");//家庭承包
                        break;
                    case eConstructMode.Other:
                    case eConstructMode.OtherContract:
                        SetBookmarkValue(AgricultureBookMark.OtherContract + "SP", "R");//其他确权确股不确地
                        break;
                    case eConstructMode.Tenderee:
                        SetBookmarkValue(AgricultureBookMark.TendereeContract + "SP", "R");//招标
                        break;
                    case eConstructMode.Transfer:
                        SetBookmarkValue(AgricultureBookMark.TransferContract + "SP", "R");//转让
                        break;
                    case eConstructMode.Vendue:
                        SetBookmarkValue(AgricultureBookMark.VendueContract + "SP", "R");//拍卖
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            Contractor = null;
            CurrentZone = null;
            LandCollection = null;
            _row = 9;
            GC.Collect();
        }

        /// <summary>
        /// 写共有人
        /// </summary>
        private void WritSharePerson()
        {
            _row += 2;
            //添加行
            if (Contractor.SharePersonList.Count - 1 > 0)
            {
                InsertTableRow(0, _row, Contractor.SharePersonList.Count - 1);
            }
            //SetTableCellValue(0, _row,4,GetComment(Contractor));//成员备注只填写第一行=>9.8取消备注
            foreach (var item in Contractor.SharePersonList)
            {
                int colum = 0;
                SetTableCellValue(0, _row, colum++, item.Name);
                SetTableCellValue(0, _row, colum++, DataHelper.GetGender(item));
                SetTableCellValue(0, _row, colum++, item.Relationship);
                SetTableCellValue(0, _row, colum++, item.ICN);
                SetTableCellValue(0, _row, colum++, item.Comment);
                _row++;
            }
            SetBookmarkValue("FamilyCount", Contractor.SharePersonList?.Count.ToString());
        }

        /// <summary>
        /// 写地块
        /// </summary>
        protected override void WriteLandInformation()
        {
            //添加行
            if (LandCollection.Count - 1 > 0)
            {
                InsertTableRow(0, _row, LandCollection.Count - 1);
            }
            double quaAreaTotal = 0;
            foreach (var item in LandCollection)
            {
                var quaArea = DataHelper.GetQuantificationArea(Contractor, item, CurrentZone, DbContext);
                quaAreaTotal += quaArea;
                int colum = 0;
                SetTableCellValue(0, _row, colum++, item.Name);
                SetTableCellValue(0, _row, colum++, DataHelper.GetLandNumber(item));
                SetTableCellValue(0, _row, colum++, DataHelper.GetNeighbor(item));
                var tableArea = DataHelper.GetTableArea(Contractor, item, CurrentZone, DbContext);
                SetTableCellValue(0, _row, colum++, quaArea.AreaFormat(2));
                SetTableCellValue(0, _row, colum++, DataHelper.GetLandPurpose(item));
                SetTableCellValue(0, _row, colum++, item.LandName);
                SetTableCellValue(0, _row, colum++, DataHelper.GetLandLevel(item));
                SetTableCellValue(0, _row, colum++, DataHelper.IsFarmLand(item));
                SetTableCellValue(0, _row, colum++, $"确权确股不确地，本户所占份额面积{quaArea.ToString("0.00")}亩");
                _row++;
            }
            SetBookmarkValue("AreaTotal", quaAreaTotal.AreaFormat(2));
            SetBookmarkValue("LandCount", LandCollection.Count.ToString());
            SetBookmarkValue("LandAllArea", LandCollection.Sum(o => o.TableArea)?.AreaFormat(2));
        }

        #region Helper

        /// <summary>
        /// 获取定制的成员备注信息
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private string GetComment(VirtualPerson family)
        {
            if (family == null || family.SharePersonList == null)
            {
                return string.Empty;
            }

            var ratio = m_ratio.ContainsKey(family.FamilyNumber) ? (m_ratio[family.FamilyNumber] * 100).ToString() + "%" : string.Empty;

            return string.Format("{0}口人，{1}比例", family.SharePersonList.Count.ToString(), ratio);
        }

        #endregion
    }
}
