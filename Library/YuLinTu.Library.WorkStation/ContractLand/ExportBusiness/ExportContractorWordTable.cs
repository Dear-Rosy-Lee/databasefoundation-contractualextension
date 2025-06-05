/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包方调查表
    /// </summary>
    [Serializable]
    public class ExportContractorWordTable : AgricultureWordBookWork
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 合同编码
        /// </summary>
        public string ConcordNumber { set; get; }
        /// <summary>
        /// 权证编码
        /// </summary>
        public string WarrentNumber { set; get; }
        public bool IsShare { get; set; }

        /// <summary>
        /// 镇村组描述
        /// </summary>
        public string MarkDesc { set; get; }

        public bool ExportVPTableCountContainsDiedPerson { set; get; }

        #endregion

        #region Ctor

        public ExportContractorWordTable()
        {
            base.TemplateName = "承包方调查表";
        }

        #endregion

        #region Methods

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
            {
                return false;
            }
            VirtualPerson family = data as VirtualPerson;
            if (family == null)
            {
                return false;
            }
            try
            {
                base.OnSetParamValue(data);
                WriteContractorInformtaion(family);
                WriteSharePersonInformation(family);   //填写共有人  
                WriteSecondConcordInformation(family);
                WriteOtherInformation(family);
                Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return true;
            }
            return true;
        }
        /// <summary>
        /// 书写承包方信息
        /// </summary>
        private void WriteContractorInformtaion(VirtualPerson family)
        {
            int familyNumber = 0;
            Int32.TryParse(family.FamilyNumber, out familyNumber);
            string zoneCode = family.ZoneCode;
            if (family.ZoneCode.Length == Zone.ZONE_GROUP_LENGTH)
            {
                zoneCode = family.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + family.ZoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
            }
            string number = string.Format("{0:D4}", familyNumber);
            SetBookmarkValue("ContractorNumber", number.IsNullOrEmpty() ? "/" : number);//承包方编码
            SetBookmarkValue("ContractorName", InitalizeFamilyName(family.Name));//承包方名称
            SetBookmarkValue("ContractorTelephone", family.Telephone.IsNullOrEmpty() ? "/" : family.Telephone);//承包方联系电话
            SetBookmarkValue("ContractorAddress", family.Address.IsNullOrEmpty() ? "/" : family.Address);//承包方地址
            SetBookmarkValue("ContractorPostNumber", family.PostalNumber.IsNullOrEmpty() ? "/" : family.PostalNumber);//承包方邮政编码
            SetBookmarkValue("ContractorIdentifyNumber", family.Number.IsNullOrEmpty() ? "/" : family.Number);//承包方证件号码
            SetBookmarkValue("ContractorCount", family.SharePersonList.Count.ToString());//承包方共有人数
            string name = (family != null && (int)family.CardType != 0) ? family.CardType.ToString() : eCredentialsType.IdentifyCard.ToString();
            if (name == "Other")
            {
                name = "CredentialOther";
            }
            SetBookmarkValue(name, "R");
        }
        /// <summary>
        /// 书写证件书签
        /// </summary>
        private void WriteCredentialsInformation(VirtualPerson family)
        {
            switch (family.CardType)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue("IdentifyCard", "R");//证件号码
                    break;
                case eCredentialsType.AgentCard:
                    SetBookmarkValue("AgentCard", "R");//证件号码
                    break;
                case eCredentialsType.OfficerCard:
                    SetBookmarkValue("OfficerCard", "R");//证件号码
                    break;
                case eCredentialsType.Other:
                    SetBookmarkValue("CredentialOther", "R");//证件号码
                    break;
                case eCredentialsType.Passport:
                    SetBookmarkValue("Passport", "R");//证件号码
                    break;
                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue("ResidenceBooklet", "R");//证件号码
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 书写共有人信息
        /// </summary>
        /// <param name="family"></param>
        public virtual void WriteSharePersonInformation(VirtualPerson family)
        {
            if (family == null)
            {
                return;
            }
            int mode = (int)family.FamilyExpand.ConstructMode;
            if (family.FamilyExpand != null && !string.IsNullOrEmpty(mode.ToString()) && family.FamilyExpand.ConstructMode != eConstructMode.Family)
            {
                SetBookmarkValue("ContractorCount", "     ");
                return;
            }
            int row = 9;
            List<Person> persons = family.SharePersonList;
            if (IsShare)
                persons = persons.FindAll(c => c.IsSharedLand.Equals("是"));

            int rowNumber = persons.Count - 14;
            if (rowNumber > 0)
            {
                InsertTableRow(0, row + 2, rowNumber);
            }
            int diedPersonCount = 0;
            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                SetTableCellValue(0, row, 0, InitalizeFamilyName(person.Name), 0, 2);
                SetTableCellValue(0, row, 1, person.Relationship, 0, 2);
                SetTableCellValue(0, row, 2, person.CardType == eCredentialsType.IdentifyCard ? person.ICN : "", 0, 2);
                SetTableCellValue(0, row, 3, string.IsNullOrEmpty(person.Comment) ? "" : person.Comment, 0, 2);
                row++;
                if (person.Comment != null)
                {
                    if (person.Comment.Contains("已故") || person.Comment.Contains("去世") || person.Comment.Contains("死亡") || person.Comment.Contains("逝世"))
                    {
                        diedPersonCount++;
                    }
                }
            }

            if (ExportVPTableCountContainsDiedPerson == false)
            {
                SetBookmarkValue("ContractorCount", (persons.Count - diedPersonCount).ToString());//合同编号
            }
        }

        /// <summary>
        /// 书写合同信息
        /// </summary>
        private void WriteSecondConcordInformation(VirtualPerson family)
        {
            VirtualPersonExpand expand = family.FamilyExpand;
            SetBookmarkValue("ConcordNumber", expand != null ? (string.IsNullOrEmpty(expand.ConcordNumber) ? "/" : expand.ConcordNumber) : "/");//合同编号
            SetBookmarkValue("BookNumber", expand != null ? (string.IsNullOrEmpty(expand.WarrantNumber) ? "/" : expand.WarrantNumber) : "/");//权证编号

            //SetBookmarkValue("ConcordNumber", string.IsNullOrEmpty(ConcordNumber) ? "/" : ConcordNumber);//合同编号
            //SetBookmarkValue("BookNumber", string.IsNullOrEmpty(WarrentNumber) ? "/" : WarrentNumber);//权证编号
            if (expand != null && expand.ConcordNumber != null && !string.IsNullOrEmpty(expand.ConcordNumber))
            {
                SetBookmarkValue("ConcordHave", "R");//证件号码
            }
            else
            {
                SetBookmarkValue("ConcordNothing", "R");//证件号码
            }
            if (expand != null && expand.WarrantNumber != null && !string.IsNullOrEmpty(expand.WarrantNumber))
            {
                SetBookmarkValue("WarrantHave", "R");//证件号码
            }
            else
            {
                SetBookmarkValue("WarrantNothing", "R");//证件号码
            }
            string date = "";
            string term = "";
            if (expand != null && expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue && expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue
                && expand.ConcordStartTime.Value.Year > 1753 && expand.ConcordEndTime.Value.Year > 1753)
            {
                date = string.Format("{0}年{1}月{2}日", expand.ConcordStartTime.Value.Year, expand.ConcordStartTime.Value.Month, expand.ConcordStartTime.Value.Day) + "至"
                              + string.Format("{0}年{1}月{2}日", expand.ConcordEndTime.Value.Year, expand.ConcordEndTime.Value.Month, expand.ConcordEndTime.Value.Day);
                term = ToolDateTime.CalcateTerm(expand.ConcordStartTime.Value, expand.ConcordEndTime.Value);
            }
            SetBookmarkValue("ConcordDate", date.IsNullOrEmpty() ? "/" : date);//合同日期
            SetBookmarkValue("ConcordTrem", !string.IsNullOrEmpty(term) ? term : "/");//合同年限
            //if (expand != null && !string.IsNullOrEmpty(expand.ConcordNumber))
            //{
            //    SetBookmarkValue("ConcordHave", "R");//证件号码
            //}
            //else
            //{
            //    SetBookmarkValue("ConcordNothing", "R");//证件号码
            //}
            //if (expand != null && !string.IsNullOrEmpty(expand.WarrantNumber))
            //{
            //    SetBookmarkValue("WarrantHave", "R");//证件号码
            //}
            //else
            //{
            //    SetBookmarkValue("WarrantNothing", "R");//证件号码
            //}
            bool isExsit = EnumNameAttribute.GetAttributes(typeof(eConstructMode)).Any(c => (eConstructMode)c.Value == expand.ConstructMode);
            //object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), expand.ConstructMode.ToString());
            //EnumNameAttribute.GetValue()
            if (isExsit)
            {
                string name = expand.ConstructMode.ToString();
                if (name == "OtherContract")
                    name = "Other";
                if (!string.IsNullOrEmpty(name))
                {
                    SetBookmarkValue(name, "R");//承包方式
                }
            }
        }

        /// <summary>
        /// 书写其他信息
        /// </summary>
        private void WriteOtherInformation(VirtualPerson family)
        {
            string surveyDate = (family.FamilyExpand.SurveyDate != null && family.FamilyExpand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(family.FamilyExpand.SurveyDate.Value) : "";
            string checkDate = (family.FamilyExpand.CheckDate != null && family.FamilyExpand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(family.FamilyExpand.CheckDate.Value) : "";
            SetBookmarkValue("ContractorSurveyChronicle", family.FamilyExpand.SurveyChronicle);//承包方调查记事
            SetBookmarkValue("ContractorSurveyPerson", family.FamilyExpand.SurveyPerson);//承包方调查员
            if (!string.IsNullOrEmpty(surveyDate))
            {
                SetBookmarkValue("ContractorSurveyDate", surveyDate);//发包方调查日期
            }
            SetBookmarkValue("ContractorCheckOpinion", family.FamilyExpand.CheckOpinion);//发包方审核记事
            SetBookmarkValue("ContractorCheckPerson", family.FamilyExpand.CheckPerson);//发包方审核员
            if (!string.IsNullOrEmpty(checkDate))
            {
                SetBookmarkValue("ContractorCheckDate", checkDate);//发包方审核日期
            }

        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            GC.Collect();
        }

        #endregion

        #endregion
    }
}
