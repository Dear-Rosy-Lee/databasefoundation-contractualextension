/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 登记簿信息
    /// </summary>
    [Serializable]
    public class ContractRegeditBookWork : AgricultureWordBook
    {
        #region Fields

        private bool useDefaultDirection;//使用默认四至方向

        protected ContractRegeditBookPrinterData bookPrinterData;

        public static ContractRegeditBookSettingDefine WarrantDefine = ContractRegeditBookSettingDefine.GetIntence();

        #endregion Fields

        /// <summary>
        /// 承包地块业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        #region Ctor

        public ContractRegeditBookWork()
        {
            string value = ToolConfiguration.GetSpecialAppSettingValue("UseDefaultDirection", "true");
            Boolean.TryParse(value, out useDefaultDirection);
            useDefaultDirection = AgricultureSetting.SystemTableLandNeighborDirectory;
            base.TemplateName = "登记簿";
        }

        #endregion Ctor

        #region Methods

        #region Override

        /// <summary>
        /// 填写信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            InitalizeDataInformation(data);
            InitalizeTempateInfo();

            base.OnSetParamValue(data);
            WriteInformation(data);
            //base.Destroyed();
            return true;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual void InitalizeDataInformation(object data)
        {
            bookPrinterData = data as ContractRegeditBookPrinterData;
            Concord = bookPrinterData.Concord.Clone() as ContractConcord;
            Contractor = bookPrinterData.FamilyEntry.Clone() as VirtualPerson;
            CurrentZone = bookPrinterData.CurrentZone.Clone() as Zone;
            Book = bookPrinterData.Book.Clone() as YuLinTu.Library.Entity.ContractRegeditBook;
            Tissue = bookPrinterData.Tissue.Clone() as CollectivityTissue;
            LandCollection = bookPrinterData.ContractLands.Clone() as List<ContractLand>;
            if (Contractor.IsStockFarmer)
            {
                var stockLandsvp = DbContext.CreateBelongRelationWorkStation().GetLandByPerson(Contractor.ID, CurrentZone.FullCode);
                if (stockLandsvp.Count > 0)
                    LandCollection.AddRange(stockLandsvp);
            }
            AccountLandBusiness = bookPrinterData.AccountLandBusiness;
            ListLandDots = bookPrinterData.ListLandDots;
        }

        /// <summary>
        /// 初始化模板
        /// </summary>
        protected virtual void InitalizeTempateInfo()
        {
            // 删除附图页
            if (WarrantDefine.IsDeleteDrawingPage)
            {
                // 删除第5节
                DeleteSection(4);
                DeleteParagraph();
            }

            // 删除登簿人页
            if (WarrantDefine.IsDeleteRegistratorPage)
            {
                // 删除第4节
                DeleteSection(3);
            }

            // 删除首页
            if (WarrantDefine.IsDeleteIndexPage)
            {
                // 删除第1节
                DeleteSection(0);
            }
        }

        #endregion Override

        #region Family

        /// <summary>
        /// 设置共有人值
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void SetSharePersonValue(ContractRegeditBookPrinterData dt)
        {
            var personSection = 0;

            List<Person> person = dt.SharePersons;
            if (SystemSet.PersonTable)
                person.Remove(person.Find(c => c.IsSharedLand.Equals("否")));
            int row = 10;
            if (person.Count - 10 > 0)
            {
                InsertTableRow(0, row, person.Count - 10);
            }
            for (int i = 0; i < person.Count; i++)
            {
                string name = InitalizeFamilyName(person[i].Name);
                string sex = "";

                if (person[i].Gender.ToString() == "Male")
                    sex = "男";
                if (person[i].Gender.ToString() == "Female")
                    sex = "女";
                if (person[i].Gender.ToString() == "Unknow")
                    sex = "";

                string Relationship = person[i].Relationship;
                string icn = person[i].ICN;
                if (person[i].CardType != eCredentialsType.IdentifyCard)
                {
                    icn = "";
                }
                string comment = person[i].Comment;

                SetTableCellValue(personSection, 0, row, 0, InitalizeFamilyName(name));
                SetTableCellValue(personSection, 0, row, 1, Relationship);
                SetTableCellValue(personSection, 0, row, 2, icn);
                SetTableCellValue(personSection, 0, row, 3, string.IsNullOrEmpty(comment) ? "" : comment);
                row++;
            }
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        public virtual void WriteSharePersonValue(ContractRegeditBookPrinterData dt)
        {
            List<Person> sharePersons = dt.SharePersons;//排序共有人，并返回人口集合
            int index = 1;
            foreach (Person person in sharePersons)
            {
                string name = "bmSharePersonName" + index.ToString();
                SetBookmarkValue(name, InitalizeFamilyName(person.Name));//姓名
                string gender = "bmSharePersonGender" + index.ToString();
                string sex = person.Gender == eGender.Female ? "女" : (person.Gender == eGender.Male ? "男" : "");
                SetBookmarkValue(gender, sex);//性别
                string ageString = "bmSharePersonAge" + index.ToString();
                SetBookmarkValue(ageString, GetPersonBirthday(person, true));//年龄
                string relationString = "bmSharePersonRelation" + index.ToString();
                SetBookmarkValue(relationString, person.Relationship);//家庭关系
                string icnNumber = "bmSharePersonNumber" + index.ToString();
                SetBookmarkValue(icnNumber, person.ICN);//身份证号码
                string comment = "bmSharePersonComment" + index.ToString();
                if (!string.IsNullOrEmpty(person.Comment))
                {
                    SetBookmarkValue(comment, person.Comment);//备注
                }
                index++;
            }
            index = 0;
        }

        /// <summary>
        /// 获取人的年龄
        /// </summary>
        /// <returns></returns>
        private string GetPersonBirthday(Person person, bool showBirthday)
        {
            if (!person.Birthday.HasValue || string.IsNullOrEmpty(person.ICN))
            {
                return string.Empty;
            }
            if (ToolICN.Check(person.ICN))
            {
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            if (!showBirthday && person.Birthday.HasValue)
            {
                int age = DateTime.Now.Year - person.Birthday.Value.Year;
                return age > 0 ? age.ToString() : "";
            }
            if (ICNHelper.Check(person.ICN))
            {
                DateTime birthDay = ICNHelper.GetBirthday(person.ICN);
                return birthDay.Year.ToString() + "." + birthDay.Month.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        private void WriteFamilyInformation(ContractRegeditBookPrinterData dt)
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("FamilyName", InitalizeFamilyName(dt.ContractorName));
                SetBookmarkValue("FamilyCount", dt.SharePersons.Count.ToString());
                SetBookmarkValue("FamilyName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(dt.ContractorName));
                string gender = GetGender(dt);
                SetBookmarkValue("Gender" + (i == 0 ? "" : i.ToString()), gender);
                string age = GetAge(dt);
                SetBookmarkValue("Age" + (i == 0 ? "" : i.ToString()), age);
                SetBookmarkValue("IdentifyNumber" + (i == 0 ? "" : i.ToString()), dt.IcnNumber);
                SetBookmarkValue("Comment" + (i == 0 ? "" : i.ToString()), dt.ContractorComment);
                string year = DateTime.Now.Year.ToString();
                SetBookmarkValue("NowYear" + (i == 0 ? "" : i.ToString()), year);
                string month = DateTime.Now.Month.ToString();
                SetBookmarkValue("NowMonth" + (i == 0 ? "" : i.ToString()), month);
                string day = DateTime.Now.Day.ToString();
                SetBookmarkValue("NowDay" + (i == 0 ? "" : i.ToString()), day);
                string fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullDate" + (i == 0 ? "" : i.ToString()), fullDate);
                year = ToolMath.GetChineseLowNimeric(DateTime.Now.Year.ToString());
                SetBookmarkValue("ChineseYear" + (i == 0 ? "" : i.ToString()), year);
                month = ToolMath.GetChineseLowNumber(DateTime.Now.Month.ToString());
                SetBookmarkValue("ChineseMonth" + (i == 0 ? "" : i.ToString()), month);
                day = ToolMath.GetChineseLowNumber(DateTime.Now.Day.ToString());
                SetBookmarkValue("ChineseDay" + (i == 0 ? "" : i.ToString()), day);
                fullDate = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("FullChineseDate" + (i == 0 ? "" : i.ToString()), fullDate);
            }
        }

        /// <summary>
        /// 获取承包方性别
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string GetGender(ContractRegeditBookPrinterData dt)
        {
            if (dt.SharePersons == null || dt.SharePersons.Count == 0)
            {
                return "";
            }
            string value = EnumNameAttribute.GetDescription(dt.SharePersons[0].Gender);
            string sex = value == EnumNameAttribute.GetDescription(eGender.Unknow) ? "" : value;
            return " " + sex + " ";
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string GetAge(ContractRegeditBookPrinterData dt)
        {
            if (dt.SharePersons == null || dt.SharePersons.Count == 0)
            {
                return "";
            }
            Person person = dt.SharePersons[0].Clone() as Person;
            if (person.Birthday != null && person.Birthday.HasValue && person.Birthday.Value.Date == DateTime.Today.Date)
            {
                person.Birthday = ToolICN.GetBirthday(person.ICN);
            }
            int age = person.GetAge();
            person = null;
            if (age < 1 || age > 200)
            {
                return "     ";
            }
            else
            {
                return age.ToString();
            }
        }

        #endregion Family

        #region ContractLand

        /// <summary>
        /// 设置承包地信息
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void SetContractLandValue(ContractRegeditBookPrinterData dt)
        {
            var landSection = 0;

            bool ActualAreaColumnWriteActualAreaWithLandBook = true; // 这个设置还需要探讨
            bool useNeighbor = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("LandBookNeighborSetting", "true");
            Boolean.TryParse(value, out useNeighbor);
            bool exportLandComment = true;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExportLandBookCommentSetting", "false");
            Boolean.TryParse(value, out exportLandComment);

            int tableIndex = 1;
            int startRow = 2;
            LandCollection.Sort("IsStockLand", eOrder.Ascending);
            if (LandCollection != null && LandCollection.Count > 0)
            {
                var addRows = LandCollection.Count - 1;
                if (addRows > 0)
                {
                    InsertTableRow(1, startRow, addRows);
                }

                for (int i = 0; i < LandCollection.Count; i++)
                {
                    string name = LandCollection[i].Name;
                    string code = LandCollection[i].LandNumber;  //InitalizeLandNumberValue(LandCollection[i]);
                    string tableArea = (dt.NoWriteLandTableArea || LandCollection[i].TableArea == 0.0) ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].TableArea.ToString(), 2);
                    string awareArea = LandCollection[i].AwareArea == 0.0 ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].AwareArea.ToString(), 2);
                    string actualArea = LandCollection[i].ActualArea == 0.0 ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].ActualArea.ToString(), 2);
                    string area = ActualAreaColumnWriteActualAreaWithLandBook ? actualArea : awareArea; // AgricultureWarrantSetting.ActualAreaColumnWriteActualAreaWithLandBook

                    if (LandCollection[i].IsStockLand)//如果是确股地，确权（合同）面积输出量化户面积(陆丰确股需求)
                    {
                        var relation = DataBaseSource.GetDataBaseSource().CreateBelongRelationWorkStation().Get(o =>
                            o.VirtualPersonID == Contractor.ID && o.LandID == LandCollection[i].ID).FirstOrDefault();
                        awareArea = relation?.QuanficationArea.AreaFormat();
                    }

                    var dictdkdj = DictList.Find(c => c.Code == LandCollection[i].LandLevel && c.GroupCode == DictionaryTypeInfo.DLDJ);
                    string levelString = dictdkdj == null ? "" : dictdkdj.Name;

                    string comment = LandCollection[i].Comment != null ? LandCollection[i].Comment : "";

                    List<string> locationXY = new List<string>();
                    var currentLandDots = ListLandDots.FindAll(d => d.LandID == LandCollection[i].ID);
                    if (currentLandDots != null && currentLandDots.Count >= 1)
                    {
                        var currentLandValidDots = currentLandDots.FindAll(m => m.IsValid == true);
                        currentLandValidDots = currentLandValidDots.Count == 0 ? currentLandDots : currentLandValidDots;
                        foreach (var currentLandDot in currentLandValidDots)
                        {
                            if (currentLandDot.Shape != null)
                            {
                                var currentLandDotCdts = currentLandDot.Shape.ToCoordinates();
                                locationXY.Add(currentLandDot.DotNumber + "(" + ToolMath.CutNumericFormat(currentLandDotCdts[0].Y, 3) + "," + ToolMath.CutNumericFormat(currentLandDotCdts[0].X, 3) + ")");
                            }
                        }
                    }
                    int currentRowHright = 0;
                    if (locationXY.Count - 3 > 0)
                    {
                        currentRowHright = locationXY.Count * 11;
                    }
                    else
                    {
                        currentRowHright = 33;
                    }

                    var locationStr = locationXY.Count > 0 ? string.Join("\n", locationXY) : string.Empty;

                    string isFarmerLand = (LandCollection[i].IsFarmerLand != null && LandCollection[i].IsFarmerLand.HasValue) ? (LandCollection[i].IsFarmerLand.Value ? "是" : "否") : "";

                    string landName = LandCollection[i].LandName;
                    if (string.IsNullOrEmpty(landName))
                    {
                        var landNamedic = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX).Find(c => c.Code == LandCollection[i].LandCode);
                        landName = landNamedic == null ? "" : landNamedic.Name;
                    }
                    if (landName == "未知")
                    {
                        landName = string.Empty;
                    }

                    var dictTDYT = DictList.Find(c => c.Code == LandCollection[i].Purpose && c.GroupCode == DictionaryTypeInfo.TDYT);
                    string landpurpose = dictTDYT == null ? "" : dictTDYT.Name;

                    string north = string.IsNullOrEmpty(LandCollection[i].NeighborNorth) ? "" : LandCollection[i].NeighborNorth;
                    string south = string.IsNullOrEmpty(LandCollection[i].NeighborSouth) ? "" : LandCollection[i].NeighborSouth;
                    string west = string.IsNullOrEmpty(LandCollection[i].NeighborWest) ? "" : LandCollection[i].NeighborWest;
                    string east = string.IsNullOrEmpty(LandCollection[i].NeighborEast) ? "" : LandCollection[i].NeighborEast;

                    SetTableCellValue(landSection, tableIndex, startRow, 1, name);
                    SetTableCellValue(landSection, tableIndex, startRow, 2, code);
                    SetTableCellValue(landSection, tableIndex, startRow, 3, tableArea);
                    SetTableCellValue(landSection, tableIndex, startRow, 4, actualArea);
                    SetTableCellValue(landSection, tableIndex, startRow, 5, isFarmerLand);
                    SetTableCellValue(landSection, tableIndex, startRow, 6, "东：" + east + "\n" + "南：" + south + "\n" + "西：" + west + "\n" + "北：" + east);

                    startRow++;
                }
            }
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteContractLandValue(ContractRegeditBookPrinterData dt)
        {
            int index = 1;
            foreach (ContractLand land in dt.ContractLands)
            {
                string landName = ExAgricultureString.BOOKLANDNAME + index.ToString();
                SetBookmarkValue(landName, land.Name);//地块名称
                InitalizeLandNumberValue(index, land);//地块编号
                string landArea = ExAgricultureString.BOOKLANDAREA + index.ToString();
                SetBookmarkValue(landArea, land.ActualArea.AreaFormat());//实测面积
                string awareArea = ExAgricultureString.BOOKLANDAWAREAREA + index.ToString();
                SetBookmarkValue(awareArea, land.AwareArea.AreaFormat());//确权面积
                string tabArae = ExAgricultureString.BOOKLANDTABLEAREA + index.ToString();
                SetBookmarkValue(tabArae, land.TableArea.AreaFormat());//二轮台帐面积
                SetLandInformation(land, index, dt);//设置地块信息
                index++;
            }
        }

        /// <summary>
        /// 设置地块编码值
        /// </summary>
        /// <returns></returns>
        private void InitalizeLandNumberValue(int index, ContractLand land)
        {
            string landNumber = ExAgricultureString.BOOKLANDNUMBER + index.ToString();
            string landValue = ContractLand.GetLandNumber(land.CadastralNumber);
            if (landValue.Length > AgricultureSetting.AgricultureLandNumberMedian)
            {
                landValue = landValue.Substring(AgricultureSetting.AgricultureLandNumberMedian);
            }
            SetBookmarkValue(landNumber, landValue);//地块编号
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="printSetting"></param>
        /// <param name="index"></param>
        private void SetLandInformation(ContractLand land, int index, ContractRegeditBookPrinterData dt)
        {
            string landLevel = ExAgricultureString.BOOKLANDLEVEL + index.ToString();
            List<Dictionary> landLevelDicts = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            string levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? "" : EnumNameAttribute.GetDescription(land.LandLevel);
            levelString = levelString == "未知" ? "" : levelString;
            string level = AgricultureSetting.UseSystemLandLevelDescription ? levelString : landLevelDicts.Find(c => c.Code == land.LandLevel).Name;  //地块等级设置
            SetBookmarkValue(landLevel, level);//耕保等级
            string landType = ExAgricultureString.BOOKLANDTYPE + index.ToString();
            string landCodeName = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX).Find(c => c.Code == land.LandCode).Name;
            SetBookmarkValue(landType, landCodeName);//地类
            bool useNeighbor = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("LandBookNeighborSetting", "true");
            Boolean.TryParse(value, out useNeighbor);
            if (useNeighbor)
            {
                string landNeighbor = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                WriteLandNeighbor(land, landNeighbor);
            }
            else
            {
                string landFigure = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                SetBookmarkValue(landFigure, "见附图");//附图
            }
            string isFarmerLand = ExAgricultureString.BOOKISFARMERLAN + index.ToString();
            SetBookmarkValue(isFarmerLand, ContractLand.GetFarmerLand(land.IsFarmerLand));
            string comment = ExAgricultureString.BOOKLANDCOMMENT + index.ToString();
            SetBookmarkValue(comment, land.Comment);
        }

        /// <summary>
        /// 打印土地四至
        /// </summary>
        private void WriteLandNeighbor(ContractLand land, string neighborIndex)
        {
            string column = neighborIndex + "1";
            if (land.NeighborEast != null)
            {
                SetBookmarkValue(column, land.NeighborEast);//四至东
            }
            column = neighborIndex + "2";
            if (land.NeighborWest != null)
            {
                SetBookmarkValue(column, land.NeighborWest);//四至西
            }
            column = neighborIndex + "3";
            if (land.NeighborSouth != null)
            {
                SetBookmarkValue(column, land.NeighborSouth);//四至南
            }
            column = neighborIndex + "4";
            if (land.NeighborNorth != null)
            {
                SetBookmarkValue(column, land.NeighborNorth);//四至北
            }
        }

        /// <summary>
        /// 写开垦地信息
        /// </summary>
        private void WriteReclamationInformation(List<ContractLand> lands)
        {
            List<ContractLand> landCollection = lands.Clone() as List<ContractLand>;
            List<ContractLand> landArray = landCollection.FindAll(ld => (!string.IsNullOrEmpty(ld.Comment) && ld.Comment.IndexOf("开垦地") >= 0));
            double reclamationTableArea = 0.0;//开垦地台帐面积
            double reclamationActualArea = 0.0;//开垦地实测面积
            double reclamationAwareArea = 0.0;//开垦地确权面积
            foreach (ContractLand land in landArray)
            {
                reclamationTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                reclamationActualArea += land.ActualArea;
                reclamationAwareArea += land.AwareArea;
                landCollection.Remove(land);
            }
            double retainTableArea = 0.0;
            double retainActualArea = 0.0;
            double retainAwareArea = 0.0;
            foreach (ContractLand land in landCollection)
            {
                retainTableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                retainActualArea += land.ActualArea;
                retainAwareArea += land.AwareArea;
            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("ReclamationTableArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(reclamationTableArea.ToString(), 2));//台帐面积
                SetBookmarkValue("RetainTableArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(retainTableArea.ToString(), 2));//台帐面积
                SetBookmarkValue("ReclamationActualArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(reclamationActualArea.ToString(), 2));//实测面积
                SetBookmarkValue("RetainActualArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(retainActualArea.ToString(), 2));//实测面积
                SetBookmarkValue("ReclamationAwareArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(reclamationAwareArea.ToString(), 2));//确权面积
                SetBookmarkValue("RetainAwareArea" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(retainAwareArea.ToString(), 2));//确权面积
            }
        }

        #endregion ContractLand

        #region OtherInformation

        /// <summary>
        /// 填写信息
        /// </summary>
        protected virtual bool WriteInformation(object data)
        {
            List<Dictionary> landTypeDicts = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);

            //bool WriteContentsForTableWithLandBook = true; //设置还要探讨    登记薄中填写表格内容
            try
            {
                string number = ToolString.GetAllNumberWithInString(bookPrinterData.ContractorAddress);
                string address = bookPrinterData.isWriteAddress ? bookPrinterData.ContractorAddress : (string.IsNullOrEmpty(number) ? bookPrinterData.ContractorAddress : bookPrinterData.ContractorAddress.Replace(number, ToolMath.GetChineseLowNumber(number)));
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("bmPriviceAndCounty" + (i == 0 ? "" : i.ToString()), bookPrinterData.CityAndCountyName);
                    SetBookmarkValue("bmICNNumber" + (i == 0 ? "" : i.ToString()), bookPrinterData.IcnNumber.GetSettingEmptyReplacement());
                    SetBookmarkValue("bmPosterNumber" + (i == 0 ? "" : i.ToString()), bookPrinterData.PosterNumber);
                    SetBookmarkValue("bmAreaConcordTable" + (i == 0 ? "" : i.ToString()), bookPrinterData.TableArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(bookPrinterData.TableArea, 2));
                    SetBookmarkValue("bmBookNumber" + (i == 0 ? "" : i.ToString()), bookPrinterData.BookNumber);
                    SetBookmarkValue("bmRegeditNumber" + (i == 0 ? "" : i.ToString()), bookPrinterData.RegeditNumber);
                    SetBookmarkValue("bmSenderName" + (i == 0 ? "" : i.ToString()), bookPrinterData.SenderName);
                    SetBookmarkValue("bmContractorName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(bookPrinterData.ContractorName));
                    SetBookmarkValue("bmContractorAddress" + (i == 0 ? "" : i.ToString()), address);
                    SetBookmarkValue("bmContractorTelephone" + (i == 0 ? "" : i.ToString()), bookPrinterData.ContractorTelephone);
                    SetBookmarkValue("bmConcordNumber" + (i == 0 ? "" : i.ToString()), bookPrinterData.ConcordNumber);
                    SetBookmarkValue("bmStartTime" + (i == 0 ? "" : i.ToString()), bookPrinterData.StartTime);
                    SetBookmarkValue("bmEndTime" + (i == 0 ? "" : i.ToString()), bookPrinterData.EndTime);
                    SetBookmarkValue("bmContractType" + (i == 0 ? "" : i.ToString()), landTypeDicts.Find(c => c.Code == bookPrinterData.ContractType).Name);
                    SetBookmarkValue("bmUseType" + (i == 0 ? "" : i.ToString()), bookPrinterData.UseType);
                    SetBookmarkValue("bmAreaLand" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(bookPrinterData.AreaLand, 2));
                    SetBookmarkValue("bmCountLand" + (i == 0 ? "" : i.ToString()), bookPrinterData.CountLand);
                    SetBookmarkValue("bmLandTableArea" + (i == 0 ? "" : i.ToString()), bookPrinterData.TableArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(bookPrinterData.TableArea, 2));
                    SetBookmarkValue("bmLandActualArea" + (i == 0 ? "" : i.ToString()), bookPrinterData.ActualArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(bookPrinterData.ActualArea, 2));
                    SetBookmarkValue("bmLandAwareArea" + (i == 0 ? "" : i.ToString()), bookPrinterData.AwareArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(bookPrinterData.AwareArea, 2));
                }
                /* 修改于2016/09/22 权证流水号应该是6位 */
                SetBookmarkValue(AgricultureBookMark.BookSerialNumber, string.IsNullOrEmpty(Book.SerialNumber) ? "" : Book.SerialNumber.PadLeft(6, '0'));
                //if (WriteContentsForTableWithLandBook)  //AgricultureWarrantSetting.WriteContentsForTableWithLandBook
                //{
                SetSharePersonValue(bookPrinterData);
                SetContractLandValue(bookPrinterData);
                //}
                //else
                //{
                //    WriteSharePersonValue(dt);
                //    WriteContractLandValue(dt);
                //}
                //WriteReclamationInformation(dt.ContractLands);
                SetRequireDate();
                //WriteZoneExpressBookMark(dt);
                WriteFamilyInformation(bookPrinterData);
                WriteStartAndEnd(bookPrinterData);
                //WriteOtherInfo();
                bookPrinterData.Disponse();
                return true;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 填写承包开始结束日期
        /// </summary>
        private void WriteStartAndEnd(ContractRegeditBookPrinterData dt)
        {
            string startYear = (dt.Concord.ArableLandStartTime == null || !dt.Concord.ArableLandStartTime.HasValue) ? "" : dt.Concord.ArableLandStartTime.Value.Year.ToString();
            string startMonth = (dt.Concord.ArableLandStartTime == null || !dt.Concord.ArableLandStartTime.HasValue) ? "" : dt.Concord.ArableLandStartTime.Value.Month.ToString();
            string startDay = (dt.Concord.ArableLandStartTime == null || !dt.Concord.ArableLandStartTime.HasValue) ? "" : dt.Concord.ArableLandStartTime.Value.Day.ToString();
            string endYear = (dt.Concord.ArableLandEndTime == null || !dt.Concord.ArableLandEndTime.HasValue) ? "" : dt.Concord.ArableLandEndTime.Value.Year.ToString();
            string endMonth = (dt.Concord.ArableLandEndTime == null || !dt.Concord.ArableLandEndTime.HasValue) ? "" : dt.Concord.ArableLandEndTime.Value.Month.ToString();
            string endDay = (dt.Concord.ArableLandEndTime == null || !dt.Concord.ArableLandEndTime.HasValue) ? "" : dt.Concord.ArableLandEndTime.Value.Day.ToString();
            string date = "自" + string.Format("{0}年{1}月{2}日", dt.Concord.ArableLandStartTime.Value.Year, dt.Concord.ArableLandStartTime.Value.Month, dt.Concord.ArableLandStartTime.Value.Day) + "起至"
                          + string.Format("{0}年{1}月{2}日", dt.Concord.ArableLandEndTime.Value.Year, dt.Concord.ArableLandEndTime.Value.Month, dt.Concord.ArableLandEndTime.Value.Day) + "止";
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("SYear" + (i == 0 ? "" : i.ToString()), startYear);//起始时间-年
                SetBookmarkValue("SMonth" + (i == 0 ? "" : i.ToString()), startMonth);//起始时间-月
                SetBookmarkValue("SDay" + (i == 0 ? "" : i.ToString()), startDay);//起始时间-日
                SetBookmarkValue("EYear" + (i == 0 ? "" : i.ToString()), endYear);//起始时间-年
                SetBookmarkValue("EMonth" + (i == 0 ? "" : i.ToString()), endMonth);//起始时间-月
                SetBookmarkValue("EDay" + (i == 0 ? "" : i.ToString()), endDay);//起始时间-日
                SetBookmarkValue("ManagementTime" + (i == 0 ? "" : i.ToString()), date);//结束时间-日
                if (dt.Concord.Flag)
                {
                    SetBookmarkValue("bmLongTime" + (i == 0 ? "" : i.ToString()), "长久");//承包期限长久
                }
            }
        }

        /// <summary>
        /// 设置申请日期
        /// </summary>
        private void SetRequireDate()
        {
            DateTime date = DateTime.Now;
            int value = date != null ? date.Year : 0;
            string valueString = value != 0 ? ToolMath.GetChineseLowNimeric(value.ToString()) : "";
            for (int i = 1; i < 5; i++)
            {
                string dtp = "bmYear" + i.ToString();
                SetBookmarkValue(dtp, valueString);
            }
            value = date != null ? date.Month : 0;
            valueString = GetValue(value);
            if (string.IsNullOrEmpty(valueString))
            {
                valueString = value != 0 ? ToolMath.GetChineseLowNumber(value.ToString()) : "";
            }
            for (int i = 1; i < 5; i++)
            {
                string dtp = "bmMonth" + i.ToString();
                SetBookmarkValue(dtp, valueString);
            }
            value = date != null ? date.Day : 0;
            valueString = GetValue(value);
            if (string.IsNullOrEmpty(valueString))
            {
                valueString = value != 0 ? ToolMath.GetChineseLowNumber(value.ToString()) : "";
            }
            for (int i = 1; i < 5; i++)
            {
                string dtp = "bmDay" + i.ToString();
                SetBookmarkValue(dtp, valueString);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValue(int value)
        {
            if (value == 10)
            {
                return "十";
            }
            if (value == 20)
            {
                return "二十";
            }
            if (value == 30)
            {
                return "三十";
            }
            return string.Empty;
        }

        #endregion OtherInformation

        #endregion Methods
    }
}