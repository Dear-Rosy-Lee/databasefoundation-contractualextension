/*
 * (C) 2012- 2014 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using System.Linq;
using YuLinTu.Common.Office;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 承包方调查表
    /// </summary>
    [Serializable]
    public class ExportContractorTable : AgricultureWordBook
    {
        #region Fields

        private GetDictionary dic;

        #endregion

        #region Properties
        #endregion

        #region Ctor

        public ExportContractorTable(string dictoryname)
        {
            dic = new GetDictionary(dictoryname);
            dic.Read();
        }

        #endregion

        #region Methods

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
                WriteSenderInformation(Contractor.ZoneCode);
                WriteSharePersonInformation(Contractor);
                WriteOtherInfo();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return true;
            }
            return true;
        }

        /// <summary>
        /// 书写发包方信息
        /// </summary>
        private void WriteSenderInformation(string zoneCode)
        {
            SetBookmarkValue("SenderCode", zoneCode.GetSenderCode());//发包方名称
        }

        /// <summary>
        /// 书写基类未处理的书签
        /// </summary>
        private void WriteOtherInfo()
        {
            if (base.Contractor == null) return;
            VirtualPersonExpand expand = base.Contractor.FamilyExpand;
            string familyString = Contractor.FamilyNumber;
            if (Contractor.FamilyNumber != null && Contractor.FamilyNumber.Length > 0 && Contractor.FamilyNumber.Length < 4)
            {
                familyString = Contractor.FamilyNumber != null && Contractor.FamilyNumber.Length > 0 ? Contractor.FamilyNumber.PadLeft(4, '0') : "";
            }           

            SetBookmarkValue("ContractorNumber", familyString);

            // 承包合同编号
            if (expand != null && !string.IsNullOrEmpty(expand.ConcordNumber))
            {
                SetBookmarkValue("ConcordHave", "R");
                SetBookmarkValue(AgricultureBookMark.ConcordNumber, expand.ConcordNumber);
            }
            else
            {
                SetBookmarkValue("ConcordNothing", "R");
                SetBookmarkValue(AgricultureBookMark.ConcordNumber, "/");
            }

            // 经营权证编号
            if (expand != null && !string.IsNullOrEmpty(expand.WarrantNumber))
            {
                SetBookmarkValue("WarrantHave", "R");
                SetBookmarkValue("BookWarrantNumber", expand.WarrantNumber);
            }
            else
            {
                SetBookmarkValue("WarrantNothing", "R");
                SetBookmarkValue("BookWarrantNumber", "/");
            }

            if (expand.CheckOpinion.IsNotNullOrEmpty())
                SetBookmarkValue("ContractorCheckOpinion", expand.CheckOpinion);
            if (expand.SurveyChronicle.IsNotNullOrEmpty())
                SetBookmarkValue("ContractorSurveyChronicle", expand.SurveyChronicle);

            string contractStartDate = String.Empty, contractEndDate = string.Empty, term = string.Empty;

            if (expand != null && expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue && expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue
                && expand.ConcordStartTime.Value.Year > 1753 && expand.ConcordEndTime.Value.Year > 1753)
            {
                contractStartDate = string.Format("{0}年{1}月{2}日", expand.ConcordStartTime.Value.Year, expand.ConcordStartTime.Value.Month, expand.ConcordStartTime.Value.Day);
                contractEndDate = string.Format("{0}年{1}月{2}日", expand.ConcordEndTime.Value.Year, expand.ConcordEndTime.Value.Month, expand.ConcordEndTime.Value.Day);
                term = ToolDateTime.CalcateTerm(expand.ConcordStartTime.Value, expand.ConcordEndTime.Value)+"年";
            }

            if (contractStartDate.IsNotNullOrEmpty())
                SetBookmarkValue("ContractStartDate", contractStartDate);       // 承包起始日期
            if (contractEndDate.IsNotNullOrEmpty())
            {
                SetBookmarkValue("ContractEndDate", contractEndDate);    // 承包终止日期
                term = !contractEndDate.Contains("9999") ? term : "长久";
            }        
            if (term.IsNotNullOrEmpty())
                SetBookmarkValue("ContractDueTime", term);                     // 合同年限

            // 取得承包方式
            eConstructMode mode = (eConstructMode)expand.ConstructMode;
            switch (mode)
            {
                case eConstructMode.Consensus:
                    SetBookmarkValue(AgricultureBookMark.ConsensusContract, "R");//公开协商
                    break;
                case eConstructMode.Exchange:
                    SetBookmarkValue(AgricultureBookMark.ExchangeContract, "R");//互换
                    break;
                case eConstructMode.Family:
                    SetBookmarkValue(AgricultureBookMark.FamilyContract, "R");//家庭承包
                    break;
                case eConstructMode.Other:
                case eConstructMode.OtherContract:
                    SetBookmarkValue(AgricultureBookMark.OtherContract, "R");//其他
                    break;
                case eConstructMode.Tenderee:
                    SetBookmarkValue(AgricultureBookMark.TendereeContract, "R");//招标
                    break;
                case eConstructMode.Transfer:
                    SetBookmarkValue(AgricultureBookMark.TransferContract, "R");//转让
                    break;
                case eConstructMode.Vendue:
                    SetBookmarkValue(AgricultureBookMark.VendueContract, "R");//拍卖
                    break;
                default:
                    break;
            }

            // 当证件类型不是身份证时，需要获取证件号码（主版本里当证件类型不是身份证时，是用的另外一个书签——ContractorOtherCardNumber，而西藏插件没有）
            if (Contractor.CardType != eCredentialsType.IdentifyCard)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorIdentifyNumber, Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码
            }
        }
        /// <summary>
        /// 书写共有人信息
        /// </summary>
        /// <param name="family"></param>
        private void WriteSharePersonInformation(VirtualPerson family)
        {
            if (family.FamilyExpand != null && family.FamilyExpand.ConstructMode != eConstructMode.Family)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorCount, "     ");
                return;
            }
            // 模板里，家庭成员信息是从第9行开始
            int row = 9;
            var persons = SortSharePerson(family.SharePersonList, family.Name);

            // 模板里家庭成员只有5行，当家庭成员超过5个时，得插入多出来的
            int rowNumber = persons.Count - 5;
            if (rowNumber > 0)
            {
                InsertTableRow(0, row + 2, rowNumber);
            }

            int index = 0;
            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                SetTableCellValue(index, row, index, InitalizeFamilyName(person.Name));
                SetTableCellValue(index, row, index + 1, person.Relationship);
                SetTableCellValue(index, row, index + 2, person.ICN);
                SetTableCellValue(index, row, index + 3, string.IsNullOrEmpty(person.Comment) ? "" : person.Comment);
                row++;
            }
        }

        #endregion
    }
}
