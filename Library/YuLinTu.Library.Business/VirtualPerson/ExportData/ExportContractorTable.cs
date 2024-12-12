/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方调查表
    /// </summary>
    public class ExportContractorTable : AgricultureWordBook
    {
        #region Field

        private ContractConcord concord; //合同

        #endregion

        #region Properties

        /// <summary>
        /// 合同编码
        /// </summary>
        public string ConcordNumber { set; get; }
        /// <summary>
        /// 镇村组描述
        /// </summary>
        public string MarkDesc { set; get; }
        /// <summary>
        /// 系统设置
        /// </summary>
        #endregion

        #region Ctor

        public ExportContractorTable()
        {
            base.TemplateName = "承包方调查表";
        }

        #endregion

        #region Methods

        /// <summary>
        /// 填写数据
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            VirtualPerson family = (data as VirtualPerson).Clone<VirtualPerson>();
            if (data == null || family == null)
            {
                return false;
            }
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            ConcordBusiness cordBusiness = new ConcordBusiness(dbContext);
            ContractRegeditBookBusiness bookBusiness = new ContractRegeditBookBusiness(dbContext);
            List<ContractConcord> concordCollection = cordBusiness.GetCollection(family.ID);
            if (concordCollection != null && concordCollection.Count > 0)
            {
                concord = concordCollection[0];
                Book = bookBusiness.Get(concord.ID);
            }
            try
            {
                if (SystemSet.PersonTable)
                {
                    //family.SharePersonList.Remove(family.SharePersonList.Find(c => c.IsSharedLand == "否"));

                    List<Person> person = family.SharePersonList;
                    person = person.FindAll(c => c.IsSharedLand.Equals("是"));
                    family.SharePersonList = person;
                }

                base.OnSetParamValue(family);
                //WriteSenderInformation(family.SenderCode);   //书写发包方信息
                WriteContractorInformtaion(family);   //书写承包方信息
                WriteSharePersonInformation(family);  //书写共有人信息
                WriteSecondConcordInformation(family); //书写合同信息
                WriteOtherInformation(family);  //书写其他信息
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
        /// 书写发包方信息
        /// </summary>
        private void WriteSenderInformation(string zoneCode)
        {
            string code = zoneCode;
            if (concord != null && !string.IsNullOrEmpty(concord.ConcordNumber))
            {
                var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());

                var senderStation = dbContext.CreateSenderWorkStation();
                CollectivityTissue tissue = senderStation.Get(CurrentZone.ID);
                if (tissue is null)
                {
                    tissue = senderStation.GetTissues(CurrentZone.FullCode, eLevelOption.Self).FirstOrDefault();
                }
                code = tissue.Code;
                if (code.Length > 16)
                {
                    code = zoneCode;
                }
            }
            if (code.Length > 14)
            {
                code = code.Substring(0, 12) + code.Substring(14, 2);
            }
            else
            {
                code = code.PadRight(14, '0');
            }
            SetBookmarkValue("SenderCode", code);//发包方名称
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
            SetBookmarkValue("ContractorNumber", number.IsNullOrEmpty() ? "".GetSettingEmptyReplacement() : number);//承包方编码
            SetBookmarkValue("ContractorName", InitalizeFamilyName(family.Name));//承包方名称
            SetBookmarkValue("ContractorTelephone", family.Telephone.GetSettingEmptyReplacement());//承包方联系电话
            SetBookmarkValue("ContractorAddress", family.Address.IsNullOrEmpty() ? "".GetSettingEmptyReplacement() : family.Address);//承包方地址
            SetBookmarkValue("ContractorPostNumber", family.PostalNumber.IsNullOrEmpty() ? "".GetSettingEmptyReplacement() : family.PostalNumber);//承包方邮政编码
            SetBookmarkValue("ContractorIdentifyNumber", family.Number.IsNullOrEmpty() ? "".GetSettingEmptyReplacement() : family.Number);//承包方证件号码
            SetBookmarkValue("ContractorCount", family.SharePersonList.Count.ToString());//承包方共有人数
            string name = (family != null && (int)family.CardType != 0) ? family.CardType.ToString() : eCredentialsType.IdentifyCard.ToString();
            if (name == "Other")
            {
                name = "CredentialOther";
            }
            SetBookmarkValue(name, "R");
        }

        /// <summary>
        /// 填写共有人信息
        /// </summary>
        public virtual void WriteSharePersonInformation(VirtualPerson family)
        {
            if (family == null)
            {
                return;
            }
            int mode = (int)family.FamilyExpand.ConstructMode;
            if (family.FamilyExpand != null && !string.IsNullOrEmpty(mode.ToString()) && family.FamilyExpand.ConstructMode != eConstructMode.Family)
            {
                SetBookmarkValue(AgricultureBookMark.ContractorCount, "     ");
                return;
            }
            int row = 9;
            List<Person> persons = SortSharePerson(family.SharePersonList, family.Name);
            int rowNumber = persons.Count - 14;
            if (rowNumber > 0)
            {
                InsertTableRow(0, row + 2, rowNumber);
            }
            string text = GetTableCellValue(0, 8, 3);
            int diedPersonCount = 0;
            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                SetTableCellValue(0, row, 0, InitalizeFamilyName(person.Name));
                SetTableCellValue(0, row, 1, person.Relationship);
                SetTableCellValue(0, row, 2, person.ICN);
                if (text.IndexOf("成员备注") != 0)
                {
                    SetTableCellValue(0, row, 3, string.IsNullOrEmpty(person.CencueComment) ? "" : person.CencueComment);
                    SetTableCellValue(0, row, 4, string.IsNullOrEmpty(person.Comment) ? "" : person.Comment);
                }
                else
                {
                    SetTableCellValue(0, row, 3, string.IsNullOrEmpty(person.Comment) ? "" : person.Comment);
                }
                row++;
                if (person.Comment != null)
                {
                    if (person.Comment.Contains("已故") || person.Comment.Contains("去世") || person.Comment.Contains("死亡") || person.Comment.Contains("逝世"))
                    {
                        diedPersonCount++;
                    }
                }
            }
            if (SystemSet.ExportVPTableCountContainsDiedPerson == false)
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
            //SetBookmarkValue("ConcordNumber", expand != null ? (string.IsNullOrEmpty(expand.ConcordNumber) ? "/" : expand.ConcordNumber) : "/");//合同编号
            //SetBookmarkValue("BookWarrantNumber", expand != null ? (string.IsNullOrEmpty(expand.WarrantNumber) ? "/" : expand.WarrantNumber) : "/");//权证编号

            string date = "";
            string term = "";
            if (expand != null && expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue && expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue
                && expand.ConcordStartTime.Value.Year > 1753 && expand.ConcordEndTime.Value.Year > 1753)
            {
                date = string.Format("{0}年{1}月{2}日", expand.ConcordStartTime.Value.Year, expand.ConcordStartTime.Value.Month, expand.ConcordStartTime.Value.Day) + "至"
                              + string.Format("{0}年{1}月{2}日", expand.ConcordEndTime.Value.Year, expand.ConcordEndTime.Value.Month, expand.ConcordEndTime.Value.Day);
                term = ToolDateTime.CalcateTerm(expand.ConcordStartTime, expand.ConcordEndTime);
            }
            SetBookmarkValue("ConcordDate", date.IsNullOrEmpty() ? "".GetSettingEmptyReplacement() : date);//合同日期
            SetBookmarkValue("ConcordTrem", !string.IsNullOrEmpty(term) ? term : "".GetSettingEmptyReplacement());//合同年限

            if (expand != null && !string.IsNullOrEmpty(expand.ConcordNumber)) //(concord != null && !string.IsNullOrEmpty(concord.ConcordNumber))
            {
                SetBookmarkValue("ConcordHave", "R");//证件号码
                SetBookmarkValue("ConcordNumber", expand.ConcordNumber);//合同编号
                //SetBookmarkValue("ConcordNumber", concord != null ? (string.IsNullOrEmpty(concord.ConcordNumber) ? "/" : concord.ConcordNumber) : "/");//合同编号
            }
            else
            {
                SetBookmarkValue("ConcordNothing", "R");//证件号码
                SetBookmarkValue("ConcordNumber", "".GetSettingEmptyReplacement());
            }
            if (expand != null && !string.IsNullOrEmpty(expand.WarrantNumber))//(Book != null && !string.IsNullOrEmpty(Book.Number)) 
            {
                SetBookmarkValue("WarrantHave", "R");//证件号码
                SetBookmarkValue("BookNumber", expand.WarrantNumber.GetSettingEmptyReplacement());//权证编号
                //SetBookmarkValue("BookNumber", Book != null ? (string.IsNullOrEmpty(Book.Number) ? "/" : Book.Number) : "/");//权证编号
            }
            else
            {
                SetBookmarkValue("WarrantNothing", "R");//证件号码
                SetBookmarkValue("BookNumber", "".GetSettingEmptyReplacement());
            }
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            var listDict = dictBusiness.GetByGroupCodeDict(DictionaryTypeInfo.CBJYQQDFS);
            string code = ((int)expand.ConstructMode).ToString();
            var dict = listDict.Find(c => c.Code == code);
            if (dict != null)
            {
                object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), dict.Name);
                eConstructMode mode = eConstructMode.Family;
                if (obj != null)
                    mode = (eConstructMode)obj;
                string name = mode.ToString();
                if (name == "OtherContract")
                    name = "Other";
                if (!string.IsNullOrEmpty(name))
                {
                    SetBookmarkValue(name, "R");//承包方式
                }
                else
                {
                    SetBookmarkValue("Family", "R");//承包方式
                }
            }
            else
            {
                SetBookmarkValue("Family", "R");//承包方式
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
            ////承包方地址镇村组
            //SetBookmarkValue("ContractorAddress", MarkDesc);
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            GC.Collect();
        }

        #endregion
    }
}
