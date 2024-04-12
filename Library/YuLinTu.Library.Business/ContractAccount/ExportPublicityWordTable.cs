/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 公示结果归户表
    /// </summary>
    [Serializable]
    public class ExportPublicityWordTable : AgricultureWordBook
    {
        #region Properties

        /// <summary>
        /// 年
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// 日
        /// </summary>
        public string Day { get; set; }

        #endregion Properties

        #region Ctor

        public ExportPublicityWordTable()
        {
            base.TemplateName = "公示结果归户表";
        }

        #endregion Ctor

        #region Methods

        #region Override

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
                if (!CheckDataInformation(data))
                {
                    return false;
                }
                WriteTitleInformation();
                WriteConcordInformations();
                WritePublicyInformation();
                //Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            Contractor = null;
            CurrentZone = null;
            LandCollection = null;
            GC.Collect();
        }

        #endregion Override

        #region Family

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        private void WitePersonInformaion()
        {
            List<Person> persons = new List<Person>();//得到户对应的共有人
            persons = Contractor.SharePersonList;
            if (SystemSet.PersonTable)
            {
                persons = persons.FindAll(c => c.IsSharedLand.Equals("是"));
            }
            List<Person> sortPersons = SortSharePerson(persons, Contractor.Name);//排序共有人，并返回人口集合
            string name = "";
            foreach (var item in sortPersons)
            {
                name += item.Name;
                name += "、";
            }
            SetBookmarkValue("ContractorName", InitalizeFamilyName(Contractor.Name));//承包方姓名
            SetBookmarkValue("ContractorTelephone", Contractor.Telephone.GetSettingEmptyReplacement());//联系电话
            SetBookmarkValue("ContractorIdentifyNumber", string.IsNullOrEmpty(Contractor.Number) ? "/" : Contractor.Number);//证件号码
            SetBookmarkValue("ContractorAddress", string.IsNullOrEmpty(Contractor.Address) ? "/" : Contractor.Address);//地址
            SetBookmarkValue("ContractorPostNumber", string.IsNullOrEmpty(Contractor.PostalNumber) ? "/" : Contractor.PostalNumber);//邮政编码
            string alloctioonPerson = Contractor.FamilyExpand != null ? Contractor.FamilyExpand.AllocationPerson : "";
            if (alloctioonPerson == Contractor.PersonCount)
            {
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("AlloctionPersonList" + (i == 0 ? "" : i.ToString()), !string.IsNullOrEmpty(name) ? name.Substring(0, name.Length - 1) : "");
                    //共有人
                }
            }
            WriteSharePersonValue();
            persons.Clear();
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonValue()
        {
            List<Person> persons = new List<Person>();//得到户对应的共有人
            persons = Contractor.SharePersonList;
            if (SystemSet.PersonTable)
            {
                persons = persons.FindAll(c => c.IsSharedLand.Equals("是"));
            }
            List<Person> sortPersons = SortSharePerson(persons, Contractor.Name);//排序共有人，并返回人口集合
            int index = 1;
            foreach (Person person in sortPersons)
            {
                string name = "bmSharePersonName" + index.ToString();
                SetBookmarkValue(name, InitalizeFamilyName(person.Name));//姓名
                string gender = "bmSharePersonGender" + index.ToString();
                string relationString = "bmSharePersonRelation" + index.ToString();
                SetBookmarkValue(relationString, person.Relationship);//家庭关系
                string icnNumber = "bmSharePersonNumber" + index.ToString();
                string number = person.CardType == eCredentialsType.IdentifyCard ? person.ICN : "";
                SetBookmarkValue(icnNumber, number);//身份证号码
                string comment = "bmSharePersonComment" + index.ToString();
                if (!string.IsNullOrEmpty(person.Comment))
                {
                    SetBookmarkValue(comment, person.Comment);//备注
                }
                index++;
            }
        }

        #endregion Family

        #region Contractland

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollections(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        #endregion Contractland

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        private void WriteConcordInformations()
        {
            WitePersonInformaion();

            WriteStartAndEnd();
            WriteSecondStartAndEnd();

            WritePrintDate();

            try
            {
                if (Concord != null)
                {
                    string mode = GetConstructMode();
                    if (Contractor != null && !string.IsNullOrEmpty(mode))
                    {
                        object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), mode);
                        string cardType = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eConstructMode.Family.ToString() : ((eConstructMode)obj).ToString();
                        if (cardType == "OtherContract")
                            cardType = "Other";
                        SetBookmarkValue(cardType, "R");
                    }
                    else
                    {
                        SetBookmarkValue("Family", "R");
                    }
                }
                string secondmode = GetConstructMode(((int)Contractor.FamilyExpand.ConstructMode).ToString());
                if (Contractor.FamilyExpand != null && !string.IsNullOrEmpty(secondmode))
                {
                    object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), secondmode);
                    string cardType = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eConstructMode.Family.ToString() : ((eConstructMode)obj).ToString();
                    if (cardType == "OtherContract")
                        cardType = "Other";
                    SetBookmarkValue("Second" + cardType, "R");
                }
                else
                {
                    SetBookmarkValue("Family", "R");
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
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
            string surveyDate = (Concord.PublicityDate != null && Concord.PublicityDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityDate.Value) : "";
            string checkDate = (Concord.PublicityResultDate != null && Concord.PublicityResultDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityResultDate.Value) : "";
            string publicDatbe = (Concord.PublicityCheckDate != null && Concord.PublicityCheckDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityCheckDate.Value) : "";
            SetBookmarkValue("PublicityChroniclePerson", Concord.PublicityChroniclePerson.GetSettingEmptyReplacement());//地块调查员
            SetBookmarkValue("PublicityDate", surveyDate.GetSettingEmptyReplacement());//地块调查日期
            SetBookmarkValue("PublicityChronicle", Concord.PublicityChronicle.GetSettingEmptyReplacement());//地块调查记事
            SetBookmarkValue("PublicityContractor", Concord.PublicityContractor.GetSettingEmptyReplacement());//地块审核员
            SetBookmarkValue("PublicityResultDate", checkDate.GetSettingEmptyReplacement());//地块审核日期
            SetBookmarkValue("PublicityResult", Concord.PublicityResult.GetSettingEmptyReplacement());//地块审核意见
            SetBookmarkValue("PublicityCheckPerson", Concord.PublicityCheckPerson.GetSettingEmptyReplacement());//地块审核员
            SetBookmarkValue("PublicityCheckDate", publicDatbe.GetSettingEmptyReplacement());//地块审核日期
            SetBookmarkValue("PublicityCheckOpinion", Concord.PublicityCheckOpinion.GetSettingEmptyReplacement());//地块审核意见
        }

        #endregion Concord

        #region OtherInformation

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd()
        {
            if (Concord == null)
            {
                SetBookmarkValue("ConcordDate", "/");//结束时间-日
                return;
            }
            //DateTime? startTime = Concord.ArableLandStartTime;
            //DateTime? endTime = Concord.ArableLandEndTime;
            DateTime? startTime = Contractor.FamilyExpand.ConcordStartTime;
            DateTime? endTime = Contractor.FamilyExpand.ConcordEndTime;
            string date = "/";
            if (startTime != null && startTime.HasValue && startTime.Value.Year > 1753)
            {
                date = string.Format("{0}年{1}月{2}日", startTime.Value.Year, startTime.Value.Month, startTime.Value.Day) + "至";
            }
            if (date != "/")
            {
                if (endTime != null && endTime.HasValue && endTime.Value.Year > 1753)
                {
                    date += string.Format("{0}年{1}月{2}日", endTime.Value.Year, endTime.Value.Month, endTime.Value.Day);
                }
                else
                {
                    date += "     年   月   日";
                }
            }
            SetBookmarkValue("ConcordDate", date);//结束时间-日
            if (Concord.ManagementTime == "长久")
            {
                SetBookmarkValue("ConcordDate", "长久");//结束时间-日
            }
        }

        /// <summary>
        /// 填写二轮承包开始结束日期
        /// </summary>
        private void WriteSecondStartAndEnd()
        {
            if (Contractor.FamilyExpand == null)
            {
                SetBookmarkValue("SecondConcordDate", "/");//结束时间-日
                return;
            }
            DateTime? startTime = Contractor.FamilyExpand.ConcordStartTime;
            DateTime? endTime = Contractor.FamilyExpand.ConcordEndTime;
            //DateTime? startTime = Contractor.FamilyExpand.ConcordStartTime;
            //DateTime? endTime = Contractor.FamilyExpand.ConcordEndTime;
            string date = "/";
            if (startTime != null && startTime.HasValue && startTime.Value.Year > 1753)
            {
                date = string.Format("{0}年{1}月{2}日", startTime.Value.Year, startTime.Value.Month, startTime.Value.Day) + "至";
            }
            if (date != "/")
            {
                if (endTime != null && endTime.HasValue && endTime.Value.Year > 1753)
                {
                    date += string.Format("{0}年{1}月{2}日", endTime.Value.Year, endTime.Value.Month, endTime.Value.Day);
                }
                else
                {
                    date += "     年   月   日";
                }
            }
            SetBookmarkValue("SecondConcordDate", date);//结束时间-日
            if (Concord != null)
            {
                if (Concord.ManagementTime == "长久")
                {
                    SetBookmarkValue("SecondConcordDate", "长久");//结束时间-日
                }
            }
        }

        /// <summary>
        /// 填写签约日期
        /// </summary>
        private void WritePrintDate()
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Year" + (i == 0 ? "" : i.ToString()), Year);//签约日期-年
                SetBookmarkValue("Month" + (i == 0 ? "" : i.ToString()), Month);//签约日期-月
                SetBookmarkValue("Day" + (i == 0 ? "" : i.ToString()), Day);//签约日期-日
            }
        }

        /// <summary>
        /// 写表头信息
        /// </summary>
        private void WriteTitleInformation()
        {
            int familyNumber = 0;
            Int32.TryParse(Contractor.FamilyNumber, out familyNumber);

            List<ContractLand> cLandList = LandCollection.FindAll(l => l.LandCategory == ((int)eLandCategoryType.ContractLand).ToString() || l.IsStockLand);

            double cArea = 0;
            cLandList.ForEach(l => cArea += l.ActualArea);
            double tArea = 0.0;
            cLandList.ForEach(l => tArea += (l.TableArea != null && l.TableArea.HasValue) ? l.TableArea.Value : 0.0);
            double otherArea = 0.0;
            List<ContractLand> otherLandList = LandCollection.FindAll(l => l.LandCategory != ((int)eLandCategoryType.ContractLand).ToString());
            otherLandList.ForEach(l => otherArea += l.ActualArea);
            string alloctioonPerson = Contractor.FamilyExpand != null ? Contractor.FamilyExpand.AllocationPerson : "";
            VirtualPersonExpand expand = Contractor.FamilyExpand;

            var concordNumber = string.IsNullOrEmpty(expand.ConcordNumber) ? "/" : expand.ConcordNumber;
            var warrantNumber = string.IsNullOrEmpty(expand.WarrantNumber) ? "/" : expand.WarrantNumber;

            if (string.IsNullOrEmpty(expand.ConcordNumber) == false && string.IsNullOrEmpty(expand.WarrantNumber) == false)
            {
                concordNumber = warrantNumber;
            }
            else if (string.IsNullOrEmpty(expand.ConcordNumber) && string.IsNullOrEmpty(expand.WarrantNumber))
            {
                concordNumber = "/";
            }
            else if (string.IsNullOrEmpty(expand.ConcordNumber) && string.IsNullOrEmpty(expand.WarrantNumber) == false)
            {
                concordNumber = warrantNumber;
            }

            SetBookmarkValue("ConcordNumber", concordNumber);//合同编号
            SetBookmarkValue("LandCount", cLandList.Count > 0 ? cLandList.Count.ToString() : "");
            SetBookmarkValue("ActualAreaCount", cArea.AreaFormat());
            SetBookmarkValue("TableAreaCount", tArea.AreaFormat());
            SetBookmarkValue("OtherLandCount", otherLandList.Count > 0 ? otherLandList.Count.ToString() : " ");
            SetBookmarkValue("OtherAreaCount", otherArea > 0 ? ToolMath.SetNumbericFormat(otherArea.ToString(), 2) : "  ");

            List<Person> persons = new List<Person>();  //得到户对应的共有人
            persons = Contractor.SharePersonList;
            if (SystemSet.PersonTable)
                persons = persons.FindAll(c => c.IsSharedLand.Equals("是"));
            SetBookmarkValue("SenderName", Tissue != null ? Tissue.Name : Concord.SenderName);
            SetBookmarkValue("SenderLawyerName", Tissue != null ? Tissue.LawyerName : "/");
            WritLandInfo(LandCollection);

            bool isErlun = IsHaveBookmark("SecondFamily");
            if (isErlun)
            {
                string mode = GetConstructMode(((int)Contractor.FamilyExpand.ConstructMode).ToString());
                if (Contractor.FamilyExpand == null || string.IsNullOrEmpty(mode) || Contractor.FamilyExpand.ConstructMode == eConstructMode.Family)
                {
                    SetBookmarkValue("ContractorCount", persons.Count.ToString());
                    WritPersonInfo(persons);
                }
                else
                    SetBookmarkValue(AgricultureBookMark.ContractorCount, "     ");
            }
            else
            {
                string mode = Concord != null ? GetConstructMode(Concord.ArableLandType) : "";
                //if (Contractor.FamilyExpand == null || string.IsNullOrEmpty(mode) || Contractor.FamilyExpand.ConstructMode == eConstructMode.Family)
                if (Concord == null || string.IsNullOrEmpty(mode) || Concord.ArableLandType == ((int)eConstructMode.Family).ToString())
                {
                    SetBookmarkValue("ContractorCount", persons.Count.ToString());
                    WritPersonInfo(persons);
                }
                else
                    SetBookmarkValue(AgricultureBookMark.ContractorCount, "     ");
            }

            object obj = Enum.Parse(typeof(eCredentialsType), Contractor.CardType.ToString());
            string cardType = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eCredentialsType.IdentifyCard.ToString() : ((eCredentialsType)obj).ToString();
            if (cardType == "Other")
            {
                cardType = "CredentialOther";
            }
            SetBookmarkValue(cardType, "R");

            //二轮合同信息
            var number = Contractor.FamilyExpand != null ? (!string.IsNullOrEmpty(Contractor.FamilyExpand.ConcordNumber) ? Contractor.FamilyExpand.ConcordNumber : "") : "/";
            SetBookmarkValue("SecondConcordNumber", number);
        }

        /// <summary>
        /// 设置地块
        /// </summary>
        private int WritLandInfo(List<ContractLand> landCollection)
        {
            List<Dictionary> listDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            List<Dictionary> listTDYT = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
            List<Dictionary> listDKLB = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            int row = 8;//开始插入地块行数
            int addrow = landCollection.Count - 8;
            int rowCount = addrow / 12;//12为填满一页地块数
            rowCount = addrow % 12 == 0 ? rowCount : ++rowCount;
            if (addrow > 0)
            {
                if (rowCount == 0)
                {
                    rowCount = 1;
                }
                InsertTableRow(0, row + 2, rowCount * 13);
            }
            int rowValue = 16;//16为第一个插入表头位置
            int increment = 13;//13为增量行数
            landCollection.Sort("IsStockLand", eOrder.Ascending);
            for (int i = 0; i < landCollection.Count; i++)
            {
                if (row >= rowValue && (row - rowValue) % increment == 0)
                {
                    InsertTableTitle(0, row, 0);
                    SetTableRowHeight(0, row, 45);
                    row++;
                    --i;
                    continue;
                }
                bool showTableArea = false;
                if (CurrentZone != null)
                {
                    showTableArea = CurrentZone.ID == landCollection[i].ConcordId ? true : false;
                }
                string name = landCollection[i].Name;
                string canumber = SysSubLandNumber(landCollection[i].LandNumber);

                double? tableArea = landCollection[i].TableArea;
                double actualArea = landCollection[i].ActualArea;
                string purpose = landCollection[i].Purpose;

                if (!ToolMath.MatchEntiretyNumber(purpose.ToString()))
                {
                    purpose = listTDYT.Find(c => c.Name == "种植业").Code; //eLandPurposeType.Planting.ToString();
                }
                string comment = landCollection[i].Comment;
                string landLevel = landCollection[i].LandLevel;
                string levelString = "";
                if (landLevel.IsNullOrEmpty())
                {
                    levelString = "";
                }
                else
                {
                    var LandLevelName = listDLDJ.Find(c => c.Code == landLevel);
                    levelString = ToolMath.MatchEntiretyNumber(landLevel.ToString()) ? (LandLevelName == null ? "" : LandLevelName.Name) : "";
                    levelString = levelString == "未知" ? "" : levelString;
                }
                var dklbdic = listDKLB.Find(c => c.Code == landCollection[i].LandCategory);
                string dklb = dklbdic != null ? dklbdic.Name : "";
                dklb = dklb == "未知" ? "" : dklb;

                int colBase = 0;
                string sfcbd = (dklb == "承包地块") ? "是" : "否";
                SetTableCellValue(0, row, colBase, name);
                SetTableCellValue(0, row, colBase + 1, canumber);

                SetTableCellValue(0, row, colBase + 2, InitalizeLandNeightors(LandCollection[i]));// string.Format("东:{0}\n南:{1}\n西:{2}\n北:{3}",
                                                                                                 //landCollection[i].NeighborEast != null ? landCollection[i].NeighborEast : "",
                                                                                                 //landCollection[i].NeighborSouth != null ? landCollection[i].NeighborSouth : "",
                                                                                                 //landCollection[i].NeighborWest != null ? landCollection[i].NeighborWest : "",
                                                                                                 //landCollection[i].NeighborNorth != null ? landCollection[i].NeighborNorth : ""));

                SetTableCellValue(0, row, colBase + 3, (tableArea == null || !tableArea.HasValue || tableArea.Value <= 0.0) ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(tableArea.Value.ToString(), 2));
                SetTableCellValue(0, row, colBase + 4, actualArea == 0 ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(actualArea.ToString(), 2));
                SetTableCellValue(0, row, colBase + 5, listTDYT.Find(c => c.Code == purpose).Name);
                SetTableCellValue(0, row, colBase + 6, levelString);
                if (landCollection[i].LandCategory != ((int)eLandCategoryType.ContractLand).ToString())
                {
                    comment = string.IsNullOrEmpty(comment) ? "" + dklb : comment + "(" + dklb + ")";
                }
                else
                {
                    comment = string.IsNullOrEmpty(comment) ? "" : comment;
                }
                if (landCollection[i].IsStockLand)
                {
                    comment = landCollection[i].Comment;
                }
                SetTableCellValue(0, row, colBase + 7, sfcbd);
                SetTableCellValue(0, row, colBase + 8, comment);
                row++;
            }
            return addrow > 0 ? addrow : 0;
        }

        /// <summary>
        /// 初始化四至
        /// </summary>
        /// <param name="neighbor"></param>
        /// <returns></returns>
        private string InitalizeLandNeightors(ContractLand land)
        {
            string neighbor = string.Format("东：{0}\n南：{1}\n西：{2} \n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("东：{0}\n西：{1}\n南：{2} \n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
            }
            return neighbor;
        }

        /// <summary>
        /// 插入表头
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="row"></param>
        private void InsertTableTitle(int tableIndex, int row, int column)
        {
            SetTableCellValue(tableIndex, row, column, "地块名称");
            SetTableCellValue(tableIndex, row, column + 1, "地块编码");
            SetTableCellValue(tableIndex, row, column + 2, "       地块四至");
            SetTableCellValue(tableIndex, row, column + 3, "合同\r\n面积");
            SetTableCellValue(tableIndex, row, column + 4, "实测\r\n面积");
            SetTableCellValue(tableIndex, row, column + 5, "土地\r\n用途");
            SetTableCellValue(tableIndex, row, column + 6, "地力\r\n等级");
            SetTableCellValue(tableIndex, row, column + 7, "是否\r\n承包地");
            SetTableCellValue(tableIndex, row, column + 8, "    地块备注");
        }

        /// <summary>
        /// 设置共有人值
        /// </summary>
        public virtual void WritPersonInfo(List<Person> personList)
        {
            if (personList == null || personList.Count() == 0)
            {
                return;
            }
            int row = 2;
            if (personList.Count - 15 > 0)
            {
                InsertTableRow(1, 3, personList.Count - 15);
            }
            for (int i = 0; i < personList.Count; i++)
            {
                string name = personList[i].Name;
                string relationship = personList[i].Relationship;
                string icn = personList[i].CardType != eCredentialsType.IdentifyCard ? "" : personList[i].ICN;
                string comment = personList[i].Comment;
                int colBase = 0;
                SetTableCellValue(1, row, colBase, InitalizeFamilyName(name));
                SetTableCellValue(1, row, colBase + 1, relationship);
                SetTableCellValue(1, row, colBase + 2, icn);
                SetTableCellValue(1, row, colBase + 3, SetReplacement(comment));
                row++;
            }
        }

        /// <summary>
        /// 根据系统设置替换空字符串或为0的数据
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        private string SetReplacement(string srcStr, bool isZeroReplaced = false)
        {
            if (string.IsNullOrEmpty(srcStr))
            {
                return SystemSetDefine.GetIntence().EmptyReplacement;
            }
            else if (isZeroReplaced && (srcStr.Trim().Equals("0") || srcStr.Trim().Equals("0.00")))
            {
                return SystemSetDefine.GetIntence().EmptyReplacement;
            }

            return srcStr;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckDataInformation(object data)
        {
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
                return false;
            if (data is ContractConcord)
            {
                Concord = data as ContractConcord;
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var contractor = personStation.Get(c => c.ID == Concord.ContracterId.Value).FirstOrDefault();
                Contractor = (Concord.ContracterId != null && Concord.ContracterId.HasValue) ? contractor : null;
            }
            else
            {
                Contractor = data as VirtualPerson;
                var concordStation = dbContext.CreateConcordStation();
                var concords = concordStation.GetAllConcordByFamilyID(Contractor.ID);
                if (concords.Count > 1)
                {
                    string familyMode = ((int)eConstructMode.Family).ToString();
                    Concord = concords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType == familyMode);
                }
                else if (concords.Count == 1)
                    Concord = concords[0];
            }
            if (Contractor == null)
            {
                Contractor = new VirtualPerson() { Name = "   " };
            }

            //检查地域数据
            if (CurrentZone == null)
            {
                return false;
            }
            if (LandCollection == null)
            {
                var landStation = dbContext.CreateContractLandWorkstation();
                var landCollection = landStation.GetCollection(Contractor.ID);
                LandCollection = Contractor != null ? landCollection : new List<ContractLand>();
            }
            LandCollection = SortLandCollections(LandCollection);
            return true;
        }

        #endregion OtherInformation

        #region Helper

        /// <summary>
        /// 获取承包方式
        /// </summary>
        /// <returns></returns>
        private string GetConstructMode(string type)
        {
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
                return "";
            if (Concord == null)
                return "";
            var dictStation = dbContext.CreateDictWorkStation();
            var listDict = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.CBJYQQDFS);
            string mode = string.Empty;
            if (listDict != null && listDict.Count > 0)
            {
                //int modeCode = (int)(Contractor.FamilyExpand.ConstructMode);
                //Concord.ArableLandType
                int modeCode = int.Parse(type);
                var dict = listDict.Find(c => !string.IsNullOrEmpty(c.Code) && c.Code.Equals(modeCode.ToString()));
                mode = dict == null ? "" : dict.Name;
            }
            return mode;
        }

        /// <summary>
        /// 根据系统配置获取截取后的地块编码
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        public string SysSubLandNumber(string landNumber)
        {
            int subStartIndex = SystemSet.LandNumericFormatValueSet;
            bool canSubstring = SystemSet.LandNumericFormatSet;
            if (canSubstring && landNumber.Length > subStartIndex)
            {
                landNumber = landNumber.Substring(subStartIndex);
            }

            return landNumber;
        }

        /// <summary>
        /// 获取承包方式
        /// </summary>
        /// <returns></returns>
        private string GetConstructMode()
        {
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
                return "";
            var dictStation = dbContext.CreateDictWorkStation();
            var listDict = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.CBJYQQDFS);
            string mode = string.Empty;
            if (listDict != null && listDict.Count > 0)
            {
                int modeCode = (int)(Contractor.FamilyExpand.ConstructMode);
                var dict = listDict.Find(c => !string.IsNullOrEmpty(c.Code) && c.Code.Equals(modeCode.ToString()));
                mode = dict == null ? "" : dict.Name;
            }
            return mode;
        }

        #endregion Helper

        #endregion Methods
    }
}