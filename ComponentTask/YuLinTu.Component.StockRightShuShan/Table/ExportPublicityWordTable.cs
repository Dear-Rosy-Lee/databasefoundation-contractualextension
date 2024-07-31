using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu;
using YuLinTu.Common.Office;
using YuLinTu.Component.StockRightShuShan.Table;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan
{
    /// <summary>
    /// 公示结果归户表
    /// </summary>
    [Serializable]
    public class ExportPublicityWordTable : Table.AgricultureWordBook
    {
        #region Fields
        #endregion

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

        #endregion

        #region Ctor

        public ExportPublicityWordTable()
        {
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
            try
            {
                base.OnSetParamValue(data);
                if (!CheckDataInformation(data))
                {
                    return false;
                }
                WriteTitleInformation();
                //WriteConcordInformation();
                WritePublicyInformation();
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
        /// 注销
        /// </summary>
        private void Disponse()
        {
            Contractor = null;
            CurrentZone = null;
            LandCollection = null;
            GC.Collect();
        }

        #endregion

        #region Family

        /// <summary>
        /// 填写承包方信息
        /// </summary>
        private void WitePersonInformaion()
        {
            List<Person> persons = new List<Person>();//得到户对应的共有人
            persons = Contractor.SharePersonList;
            List<Person> sortPersons = SortSharePerson(persons, Contractor.Name);//排序共有人，并返回人口集合
            string name = "";
            foreach (var item in sortPersons)
            {
                name += item.Name;
                name += "、";
            }
            SetBookmarkValue("ContractorName", InitalizeFamilyName(Contractor.Name).GetExportString());//承包方姓名
            SetBookmarkValue("ContractorTelephone", Contractor.Telephone.GetExportString());//联系电话
            SetBookmarkValue("ContractorIdentifyNumber", Contractor.Number.GetExportString());//证件号码
            SetBookmarkValue("ContractorAddress", Contractor.Address.GetExportString());//地址
            SetBookmarkValue("ContractorPostNumber", string.IsNullOrEmpty(Contractor.PostalNumber) ? "" : Contractor.PostalNumber);//邮政编码
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
            List<Person> sortPersons = SortSharePerson(persons, Contractor.Name);//排序共有人，并返回人口集合
            int index = 1;
            foreach (Person person in sortPersons)
            {
                string name = "bmSharePersonName" + index.ToString();
                SetBookmarkValue(name, person.Name);//姓名
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

        #endregion

        #region Contractland


        #endregion

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        protected override void WriteConcordInformation()
        {
            WitePersonInformaion();

            //WritLandInfo(LandCollection);

            WriteStartAndEnd();

            WritePrintDate();

            try
            {
                //string mode = GetConstructMode();
                //if (Contractor != null && !string.IsNullOrEmpty(mode))
                //{
                //    object obj = EnumNameAttribute.GetValue(typeof(eConstructMode), mode);
                //    string cardType = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eConstructMode.Family.ToString() : ((eConstructMode)obj).ToString();
                //    SetBookmarkValue(cardType, "R");
                //}
                //else
                //{
                //    SetBookmarkValue("Family", "R");
                //}

                VirtualPersonExpand expand = Contractor.FamilyExpand;
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

                var concordNumber = Concord != null && Concord.ConcordNumber.IsNotNullOrEmpty() ? Concord.ConcordNumber : "";
                SetBookmarkValue("ConcordNumberSP", concordNumber);
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
            SetBookmarkValue("PublicityChroniclePerson", Concord.PublicityChroniclePerson);//地块调查员
            SetBookmarkValue("PublicityDate", surveyDate);//地块调查日期
            SetBookmarkValue("PublicityChronicle", Concord.PublicityChronicle);//地块调查记事
            SetBookmarkValue("PublicityContractor", Concord.PublicityContractor);//地块审核员
            SetBookmarkValue("PublicityResultDate", checkDate);//地块审核日期
            SetBookmarkValue("PublicityResult", Concord.PublicityResult);//地块审核意见
            SetBookmarkValue("PublicityCheckPerson", Concord.PublicityCheckPerson);//地块审核员
            SetBookmarkValue("PublicityCheckDate", publicDatbe);//地块审核日期
            SetBookmarkValue("PublicityCheckOpinion", Concord.PublicityCheckOpinion);//地块审核意见
        }

        #endregion

        #region OtherInformation

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd()
        {
            if (Contractor == null)
            {
                SetBookmarkValue("ConcordDate", "/");//结束时间-日
                return;
            }
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
            if (date == "/" && Concord.ArableLandStartTime != null && Concord.ArableLandEndTime != null)
            {
                date = Concord.ArableLandStartTime.Value.ToLongDateString().ToString() + "至" + Concord.ArableLandEndTime.Value.ToLongDateString().ToString();
            }
            SetBookmarkValue("ConcordDate", date);//结束时间-日
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
            string landCategory = string.Empty;
            var dictDKLB = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            if (dictDKLB != null && dictDKLB.Count > 0)
            {
                string desc = EnumNameAttribute.GetDescription(eLandCategoryType.ContractLand);
                landCategory = dictDKLB.Find(c => c.Name.Equals(desc)) == null ? "" : dictDKLB.Find(c => c.Name.Equals(desc)).Code;
            }
            List<ContractLand> cLandList = LandCollection.FindAll(l => l.LandCategory == landCategory || l.IsStockLand);
            double cArea = 0;
            cLandList.ForEach(l => cArea += l.ActualArea);
            double tArea = 0.0;
            cLandList.ForEach(l => tArea += l.TableArea ?? 0.0);
            double otherArea = 0.0;
            List<ContractLand> otherLandList = LandCollection.FindAll(l => l.LandCategory != landCategory);
            otherLandList.ForEach(l => otherArea += l.ActualArea);
            string alloctioonPerson = Contractor.FamilyExpand != null ? Contractor.FamilyExpand.AllocationPerson : "";
            string number = Contractor != null ? (!string.IsNullOrEmpty(Contractor.FamilyExpand?.WarrantNumber) ? Contractor.FamilyExpand.WarrantNumber : "") : "/";
            number = (Contractor != null && !string.IsNullOrEmpty(number)) ? number : (!string.IsNullOrEmpty(Contractor?.FamilyExpand?.ConcordNumber) ? Contractor.FamilyExpand.ConcordNumber : "/");
            if (number == "/" || number.IsNullOrEmpty())
            {
                number = Concord.ConcordNumber;
            }

            var stockLandList = CurrentZone.GetAllStockLand();
            var stockLandCount = stockLandList.Count;
            var stockLandTableAreaTotal = DataHelper.GetShareLandAreaTotal(stockLandList, CurrentZone);
            SetBookmarkValue("StockLandCount", stockLandCount > 0 ? stockLandCount.ToString() : "/");
            SetBookmarkValue("StockLandAreaCount", stockLandTableAreaTotal.AreaFormat(2));

            SetBookmarkValue("ConcordNumber", number);
            SetBookmarkValue("LandCount", cLandList.Count > 0 ? cLandList.Count.ToString() : "/");
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            SetBookmarkValue("ActualAreaCount", cArea.AreaFormat());
            SetBookmarkValue("TableAreaCount", tArea.AreaFormat());
            SetBookmarkValue("OtherLandCount", otherLandList.Count > 0 ? otherLandList.Count.ToString() : " ");
            SetBookmarkValue("OtherAreaCount", otherArea.AreaFormat());
            List<Person> persons = new List<Person>();  //得到户对应的共有人

            persons = Contractor.SharePersonList;
            SetBookmarkValue("SenderName", (Tissue != null ? Tissue.Name : Concord.SenderName).GetExportString());
            SetBookmarkValue("SenderLawyerName", Tissue?.LawyerName.GetExportString());

            string mode = GetConstructMode();
            if (Contractor.FamilyExpand == null || string.IsNullOrEmpty(mode) || Contractor.FamilyExpand.ConstructMode == eConstructMode.Family)
            {
                SetBookmarkValue("ContractorCount", (!string.IsNullOrEmpty(Contractor.PersonCount) ? Contractor.PersonCount : persons.Count.ToString().GetExportString()));
                WritPersonInfo(persons);
            }
            else
            {
                SetBookmarkValue(AgricultureBookMark.ContractorCount, "/");
            }
            object obj = System.Enum.Parse(typeof(eCredentialsType), Contractor.CardType.ToString());
            string cardType = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eCredentialsType.IdentifyCard.ToString() : ((eCredentialsType)obj).ToString();
            if (cardType == "Other")
            {
                cardType = "CredentialOther";
            }
            SetBookmarkValue(cardType, "R");
        }

        /// <summary>
        /// 初始化发包方名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeSenderName()
        {
            if (CurrentZone.FullCode.Length < Zone.ZONE_COUNTY_LENGTH)
            {
                return CurrentZone.Name;
            }
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
                return "";
            var zoneStation = dbContext.CreateZoneWorkStation();
            var zone = zoneStation.Get(c => c.FullCode.Equals(CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH))).FirstOrDefault();
            if (zone == null)
            {
                return CurrentZone.Name;
            }
            return CurrentZone.FullName.Replace(zone.FullName, "");
        }




        /// <summary>
        /// 设置地块
        /// </summary>
        //protected override void WriteLandInformation()
        //{
        //    //landCollection.Sort("IsStockLand", eOrder.Ascending);
        //    int row = 8;
        //    CopyTable(7, 13, 8);
        //    double quaAreaTotal = 0;
        //    foreach (var item in LandCollection)
        //    {
        //        var relation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByID(Contractor.ID, item.ID);
        //        if (_titleInRowList.Any(o => o == row))//跳过表头行
        //        {
        //            //landCollection.Insert(LandCollection.IndexOf(item), item);
        //            row++;
        //            continue;
        //        }
        //        int colBase = 0;
        //        var neighbour = @"东：" + item.NeighborEast + "\n" +
        //                        "南：" + item.NeighborSouth + "\n" +
        //                        "西：" + item.NeighborWest + "\n" +
        //                        "北：" + item.NeighborNorth;
        //        var quaArea = DataHelper.GetQuantificationArea(Contractor, item, CurrentZone, DbContext);
        //        quaAreaTotal += quaArea;
        //        SetTableCellValue(0, row, colBase++, item.Name + Environment.NewLine + item.LandNumber.GetLastString(5));
        //        SetTableCellValue(0, row, colBase++, neighbour);
        //        SetTableCellValue(0, row, colBase++, item.ActualArea.AreaFormat(2));
        //        SetTableCellValue(0, row, colBase++, quaArea.AreaFormat(2));
        //        SetTableCellValue(0, row, colBase++, TDYT.Find(o => o.Code == item.Purpose)?.Name);
        //        SetTableCellValue(0, row, colBase++, DLDJ.Find(o => o.Code == item.LandLevel)?.Name);
        //        SetTableCellValue(0, row, colBase++, item.LandName);
        //        SetTableCellValue(0, row, colBase, "");
        //        row++;
        //    }
        //    SetBookmarkValue("SurvFamilyAreaRatio", quaAreaTotal.AreaFormat(2));
        //    _titleInRowList.Clear();
        //}

        protected override void WriteLandInformation()
        {
            List<Dictionary> listDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            List<Dictionary> listTDYT = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT);
            List<Dictionary> listDKLB = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB);
            int row = 8;//开始插入地块行数
            int addrow = LandCollection.Count - 8;
            int rowCount = addrow / 11;//12为填满一页地块数
            rowCount = addrow % 11 == 0 ? rowCount : ++rowCount;
            if (addrow > 0)
            {
                if (rowCount == 0)
                {
                    rowCount = 1;
                }
                InsertTableRow(0, row + 2, rowCount * 11);
            }
            int rowValue = 13;//16为第一个插入表头位置
            int increment = 11;//13为增量行数
            double quaAreaTotal = 0;
            for (int i = 0; i < LandCollection.Count; i++)
            {
                if (row >= rowValue && (row - rowValue) % increment == 0)
                {
                    InsertTableTitle(0, row, 0);
                    SetTableRowHeight(0, row, 45);
                    row++;
                    --i;
                    continue;
                }

                int colBase = 0;
            
                var quaArea = DataHelper.GetQuantificationArea(Contractor, LandCollection[i], CurrentZone, DbContext);
                quaAreaTotal += quaArea;
                SetTableCellValue(0, row, colBase++, LandCollection[i].Name + Environment.NewLine + LandCollection[i].LandNumber.GetLastString(5));
                SetTableCellValue(0, row, colBase++, DataHelper.GetNeighbor(LandCollection[i]));
                SetTableCellValue(0, row, colBase++, LandCollection[i].ActualArea.AreaFormat(2));
                SetTableCellValue(0, row, colBase++, quaArea.AreaFormat(2));
                SetTableCellValue(0, row, colBase++, TDYT.Find(o => o.Code == LandCollection[i].Purpose)?.Name);
                SetTableCellValue(0, row, colBase++, DLDJ.Find(o => o.Code == LandCollection[i].LandLevel)?.Name);
                SetTableCellValue(0, row, colBase++, LandCollection[i].LandName);
                SetTableCellValue(0, row, colBase, "");
                row++;
            }
            SetBookmarkValue("SurvFamilyAreaRatio", quaAreaTotal.AreaFormat(2));
            _titleInRowList.Clear();
        }

        /// <summary>
        /// 插入表头
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="row"></param>
        private void InsertTableTitle(int tableIndex, int row, int column)
        {
            SetTableCellValue(tableIndex, row, column, "公用地块名称（地块编码）");
            SetTableCellValue(tableIndex, row, column + 1, "    地块四至");
            SetTableCellValue(tableIndex, row, column + 2, "共用地块实测面积");
            SetTableCellValue(tableIndex, row, column + 3, "农户承包合同面积");
            SetTableCellValue(tableIndex, row, column + 4, "土地用途");
            SetTableCellValue(tableIndex, row, column + 5, "地力等级");
            SetTableCellValue(tableIndex, row, column + 6, "土地利用类型");
            SetTableCellValue(tableIndex, row, column + 7, "地块备注");
        }


        /// <summary>
        /// 设置共有人值
        /// </summary>
        private void WritPersonInfo(List<Person> personList)
        {
            if (personList == null || personList.Count() == 0)
            {
                return;
            }
            int row = 2;
            for (int i = 0; i < personList.Count; i++)
            {
                string name = personList[i].Name;
                string relationship = personList[i].Relationship;
                string icn = personList[i].CardType != eCredentialsType.IdentifyCard ? "" : personList[i].ICN;
                string comment = personList[i].Comment;
                int colBase = 0;
                SetTableCellValue(1, row, colBase, name);
                SetTableCellValue(1, row, colBase + 1, relationship);
                SetTableCellValue(1, row, colBase + 2, icn);

                row++;
            }
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
            }
            if (Contractor == null)
            {
                Contractor = new VirtualPerson() { Name = "   " };
            }
            //检查合同数据
            var concordStation = dbContext.CreateConcordStation();
            var concord = concordStation.GetContractsByFamilyID(Contractor.ID).FirstOrDefault();
            Concord = Concord == null ? concord : Concord;
            if (Concord == null)
            {
                var tissueStation = dbContext.CreateSenderWorkStation();
                var tissue = tissueStation.Get(c => c.ID == CurrentZone.ID).FirstOrDefault();
                Concord = new ContractConcord();
                Concord.SenderName = tissue != null ? tissue.Name : "";
                Concord.SenderId = tissue != null ? tissue.ID : CurrentZone.ID;
            }
            //检查地域数据
            var zoneStation = dbContext.CreateZoneWorkStation();
            var zoneByContractor = zoneStation.Get(c => c.FullCode.Equals(Contractor.ZoneCode)).FirstOrDefault();
            var zoneByConcord = zoneStation.Get(c => c.FullCode.Equals(Concord.ZoneCode)).FirstOrDefault();
            CurrentZone = Contractor != null ? zoneByContractor : (Concord != null ? zoneByConcord : null);
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
            return true;
        }

        #endregion

        #region Helper

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
        #endregion

        #endregion
    }
}
