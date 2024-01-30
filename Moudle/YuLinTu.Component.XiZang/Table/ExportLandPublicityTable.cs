using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using Aspose.Words;
using Aspose.Words.Tables;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出公示结果归户表(word)
    /// </summary>
    [Serializable]
    public class ExportLandPublicityTable : AgricultureWordBook
    {
        #region 字段
        private string fileName = string.Empty;//文件名称
        private string errorInformation = string.Empty;//错误信息       
        private GetDictionary dic;//藏文字典
        private int landStartRow = 8;//开始书写地块信息的行位置

        #endregion

        #region 属性
        #endregion

        #region Ctor

        public ExportLandPublicityTable(string fileName, string dictoryName)
        {
            this.fileName = fileName;
            dic = new GetDictionary(dictoryName);
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

                WriteLandInfo();//写地块信息
                WritePersonInfo();//写共有人信息
                WriteOtherInfo();//写其他信息
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return true;
            }
            return true;
        }

        /// <summary>
        /// 写其他信息
        /// </summary>
        private void WriteOtherInfo()
        {
            string concordStartDate = string.Empty;
            string concordEndDate = string.Empty;
            concordStartDate = Contractor.FamilyExpand.ConcordStartTime == null ? "" : ((DateTime)Contractor.FamilyExpand.ConcordStartTime.Value).ToLongDateString();
            concordEndDate = Contractor.FamilyExpand.ConcordEndTime == null ? "" : ((DateTime)Contractor.FamilyExpand.ConcordEndTime.Value).ToLongDateString();

            if (!string.IsNullOrEmpty(concordStartDate))
            {
                SetBookmarkValue("ContractStartTime", concordStartDate);//承包起始日期
            }
            if (!string.IsNullOrEmpty(concordEndDate))
            {
                SetBookmarkValue("ContractEndTime", !Concord.Flag ? concordEndDate : "9999年1月1日");//承包终止日期
            }
            string number = "/";
            if (Contractor.FamilyExpand.ConcordNumber.IsNullOrEmpty() == false)
            {
                number = Contractor.FamilyExpand.ConcordNumber;
            }
            if (Contractor.FamilyExpand.WarrantNumber.IsNullOrEmpty() == false)
            {
                number = Contractor.FamilyExpand.WarrantNumber;
            }            
            SetBookmarkValue("ConcordNumber", number);

            SetBookmarkValue("SenderName", Tissue != null ? Tissue.Name : Concord.SenderName);
            SetBookmarkValue("SenderLawyerName", Tissue != null ? Tissue.LawyerName : "/");

            // 取得承包方式
            eConstructMode mode = (eConstructMode)Contractor.FamilyExpand.ConstructMode;
            switch (mode)
            {
                case eConstructMode.Consensus:
                    SetBookmarkValue(AgricultureBookMark.ConsensusContract, "R");//公开协商
                    SetBookmarkValue(AgricultureBookMark.ConsensusContract + "1", "R");//藏文
                    break;
                case eConstructMode.Exchange:
                    SetBookmarkValue(AgricultureBookMark.ExchangeContract, "R");//互换
                    SetBookmarkValue(AgricultureBookMark.ExchangeContract + "1", "R");//藏文
                    break;
                case eConstructMode.Family:
                    SetBookmarkValue(AgricultureBookMark.FamilyContract, "R");//家庭承包
                    SetBookmarkValue(AgricultureBookMark.FamilyContract + "1", "R");//藏文
                    break;
                case eConstructMode.Other:
                case eConstructMode.OtherContract:
                    SetBookmarkValue(AgricultureBookMark.OtherContract, "R");//其他
                    SetBookmarkValue(AgricultureBookMark.OtherContract + "1", "R");//藏文
                    break;
                case eConstructMode.Tenderee:
                    SetBookmarkValue(AgricultureBookMark.TendereeContract, "R");//招标
                    SetBookmarkValue(AgricultureBookMark.TendereeContract + "1", "R");//藏文
                    break;
                case eConstructMode.Transfer:
                    SetBookmarkValue(AgricultureBookMark.TransferContract, "R");//转让
                    SetBookmarkValue(AgricultureBookMark.TransferContract + "1", "R");//藏文
                    break;
                case eConstructMode.Vendue:
                    SetBookmarkValue(AgricultureBookMark.VendueContract, "R");//拍卖
                    SetBookmarkValue(AgricultureBookMark.VendueContract + "1", "R");//藏文
                    break;
                default:
                    break;
            }
            //    IdentifyCard = 1,
            //OfficerCard = 2,
            //AgentCard = 3,
            //ResidenceBooklet = 4,
            //Passport = 5,
            //Other = 9
            eCredentialsType cardType = (eCredentialsType)Contractor.CardType;
            switch (cardType)
            {
                case eCredentialsType.IdentifyCard:
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.IdentifyCard + "1", "R");//藏文
                    break;
                case eCredentialsType.OfficerCard:
                    SetBookmarkValue(AgricultureBookMark.OfficerCard, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.OfficerCard + "1", "R");//藏文
                    break;
                case eCredentialsType.Other:
                case eCredentialsType.AgentCard:
                    SetBookmarkValue(AgricultureBookMark.CredentialOther, "R");//
                    SetBookmarkValue(AgricultureBookMark.CredentialOther + "1", "R");//藏文
                    break;
                case eCredentialsType.Passport:
                    SetBookmarkValue(AgricultureBookMark.Passport, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.Passport + "1", "R");//藏文
                    break;
                case eCredentialsType.ResidenceBooklet:
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet, "R");//证件类型
                    SetBookmarkValue(AgricultureBookMark.ResidenceBooklet + "1", "R");//藏文
                    break;
                default:
                    break;
            }


            WritePublicyInformation();//写调查信息
        }

        /// <summary>
        /// 书写调查信息
        /// </summary>
        private void WritePublicyInformation()
        {
            if (Concord == null)
            {
                return;
            }
            string surveyDate = (Concord.PublicityDate != null) ? ToolDateTime.GetLongDateString(Concord.PublicityDate.Value) : "";
            string checkDate = (Concord.PublicityResultDate != null) ? ToolDateTime.GetLongDateString(Concord.PublicityResultDate.Value) : "";
            string publicDatbe = (Concord.PublicityCheckDate != null) ? ToolDateTime.GetLongDateString(Concord.PublicityCheckDate.Value) : "";
            SetBookmarkValue("PublicityChroniclePerson", Concord.PublicityChroniclePerson);//公示记事人
            SetBookmarkValue("PublicityDate", surveyDate);//公示记事日期
            SetBookmarkValue("PublicityChronicle", Concord.PublicityChronicle);//公示记事
            SetBookmarkValue("PublicityContractor", Concord.PublicityContractor);//承包方代表
            SetBookmarkValue("PublicityResultDate", checkDate);//地块审核日期
            SetBookmarkValue("PublicityResult", Concord.PublicityResult);//地块审核意见
            SetBookmarkValue("PublicityCheckPerson", Concord.PublicityCheckPerson);//公示结果审核人
            SetBookmarkValue("PublicityCheckDate", publicDatbe);//公示结果审核日期
            SetBookmarkValue("PublicityCheckOpinion", Concord.PublicityCheckOpinion);//公示结果审核意见
        }

        /// <summary>
        /// 写地块信息
        /// </summary>
        private void WriteLandInfo()
        {
            // 只输出承包地块的信息，排除“非承包地块”信息，必须在第一行写值，因为合格了单元格。
            //List<ContractLand> cLandList = LandCollection.FindAll(l => l.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            List<ContractLand> cLandList = LandCollection;
            if (cLandList.Count - 1 > 0)
            {
                InsertTableRowCloneByCount1(0, landStartRow, landStartRow + 3, (cLandList.Count - 1));

                //for (int i = 0; i < cLandList.Count; i++)
                //{
                //    var starrow = landStartRow + i * 3;
                //    var endrow = starrow + 5;
                //    VerticalMergeTable(0, starrow, endrow, 2);
                //}
                //    VerticalMergeTable(0, starrow, endrow, 3);
                //    VerticalMergeTable(0, starrow, endrow, 4);
                //    VerticalMergeTable(0, starrow, endrow, 5);
                //    VerticalMergeTable(0, starrow, endrow, 6);
                //    VerticalMergeTable(0, starrow, endrow, 7);
                //    VerticalMergeTable(0, starrow, endrow, 8);
                //}                
            }
            double cArea = 0;
            cLandList.ForEach(l => cArea += l.ActualArea);
            string alloctioonPerson = Contractor.FamilyExpand != null ? Contractor.FamilyExpand.AllocationPerson : "";

            SetBookmarkValue("LandCount", cLandList.Count > 0 ? cLandList.Count.ToString() : "");
            SetBookmarkValue("LandCount1", cLandList.Count > 0 ? cLandList.Count.ToString() : "");
            SetBookmarkValue("LandAreaCount", cArea.AreaFormat());
            SetBookmarkValue("LandAreaCount1", cArea.AreaFormat());
            for (int i = 0; i < cLandList.Count; i++)
            {
                int colBase = 0;
                string landNumber = cLandList[i].LandNumber;
                var systemSetting = SystemSetDefine.GetIntence();
                if (systemSetting.LandNumericFormatSet && landNumber.Length > systemSetting.LandNumericFormatValueSet)
                {
                    landNumber = landNumber.Substring(systemSetting.LandNumericFormatValueSet);
                }
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(cLandList[i].Name) + "(" + ConvertNull(landNumber) + ")");//地块名称和地块编码

                colBase++;
                SetTableCellValue(0, landStartRow++, colBase, cLandList[i].NeighborEast);
                SetTableCellValue(0, landStartRow++, colBase, cLandList[i].NeighborSouth);
                SetTableCellValue(0, landStartRow++, colBase, cLandList[i].NeighborWest);
                SetTableCellValue(0, landStartRow, colBase, cLandList[i].NeighborNorth);
                colBase++;
                landStartRow = landStartRow - 3;              
                string tableArea = ConvertNull(cLandList[i].TableArea.Value.ToString("f2"));
                if (tableArea.Equals(0.00) || cLandList[i].TableArea.Value==0)
                    tableArea = "/";
                SetTableCellValue(0, landStartRow, colBase++, tableArea);//合同面积
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(cLandList[i].ActualArea.ToString("f2")));//实测面积
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(GetEnumDesp<eLandPurposeType>(cLandList[i].Purpose)));//土地用途
                string landLevel = ConvertNull(GetEnumDesp<eContractLandLevel>(cLandList[i].LandLevel));
                if (landLevel.Equals("未知"))
                    landLevel = "/";
                SetTableCellValue(0, landStartRow, colBase++, landLevel);//地力等级
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(cLandList[i].LandName));//土地利用类型
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(cLandList[i].Comment));//备注

                landStartRow = landStartRow + 4;
            }
        }

        /// <summary>
        /// 写共有人信息
        /// </summary>
        private void WritePersonInfo()
        {
            if (LandCollection.Count >= 1)
            {
                landStartRow = 10 + LandCollection.Count * 4;
            }
            else
            {
                landStartRow = 11;
            }

            if (Contractor.FamilyExpand != null && ((int)Contractor.FamilyExpand.ConstructMode).ToString() != "110")
            {
                return;
            }
            List<Person> persons = SortSharePerson(Contractor.SharePersonList, Contractor.Name);

            SetBookmarkValue("PersonCount", persons.Count.ToString());//共多少人
            if (persons.Count - 1 > 0)
            {
                InsertTableRow(0, landStartRow, persons.Count - 1);
            }
            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                int colBase = 0;
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(InitalizeFamilyName(person.Name)));//姓名
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(person.Relationship));//家庭关系 
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(person.ICN));//身份证号码 
                SetTableCellValue(0, landStartRow, colBase++, ConvertNull(person.Comment));//备注 
                landStartRow++;
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// 如果要判断的字符串为空或空白，则用“/”代替
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        private string ConvertNull(string originalStr)
        {
            return String.IsNullOrEmpty(originalStr) ? "/" : originalStr;
        }

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValueStr">枚举值字符串</param>
        /// <returns></returns>
        private string GetEnumDesp<TEnum>(string enumValueStr) where TEnum : struct
        {
            TEnum tEnum = new TEnum();
            Enum.TryParse<TEnum>(enumValueStr, out tEnum);

            return EnumNameAttribute.GetDescription(tEnum);
        }

        /// <summary>
        /// 向指定的表中复制对应行数的单元格并追加到表尾
        /// </summary>
        /// <param name="tableIndex">表号</param>
        /// <param name="startRow">开始插入行</param>
        ///  <param name="endRow">结束插入行</param>
        /// <param name="Count">复制几次</param>
        protected void InsertTableRowCloneByCount1(int tableIndex, int startRow, int endRow, int Count = 1)
        {
            if (base.doc == null)
            {
                return;
            }
            NodeCollection tables = base.doc.GetChildNodes(NodeType.Table, true);
            if (tables == null || tables.Count == 0 || tableIndex >= tables.Count)
            {
                return;
            }
            Table table = tables[tableIndex] as Table;
            if (table == null)
            {
                return;
            }
            if (startRow >= table.Rows.Count)
            {
                return;
            }
            endRow++;//因为插入是插入到当前的前一行
            var addrowcount = endRow - startRow;//复制的行
            try
            {
                Node node = null;
                int addindex = endRow;
                for (int i = 0; i < Count; i++)
                {
                    for (int index = startRow; index < endRow; index++)
                    {
                        node = table.Rows[index].Clone(true);
                        table.Rows.Insert(addindex, node);
                        addindex++;
                    }
                }
                node = null;
                table = null;
                tables = null;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}
