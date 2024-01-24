/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Data;
using YuLinTu.Diagrams;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 登记簿信息
    /// </summary>
    [Serializable]
    public class ContractRegeditBookWork : AgricultureWordBook
    {
        #region Fields

        private bool useDefaultDirection;//使用默认四至方向
        private YuLinTu.Library.WorkStation.IContractLandWorkStation landStation;
        private List<Dictionary> dictDKLB;    //地块类别数据字典集合
        private List<ContractLand> geoLandCollection;  //空间地块集合-用户的地块集合
        private ContractBusinessParcelWordSettingDefine ParcelSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
        private ExportLandParcelMainOperation exportLandParcelMainOperation;
        private SpatialReference spatialReference;
        #endregion

        #region Properties

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 所有界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> DotData { get; set; }

        /// <summary>
        /// 数据字典集合(所有)
        /// </summary>
        public List<Dictionary> DictDKLB
        {
            get { return dictDKLB; }
            set
            {
                dictDKLB = value;
            }
        }


        public DiagramsView ViewOfAllMultiParcel { get; set; }

        /// <summary>
        /// 村级地域
        /// </summary>
        public Zone VillageZone { get; set; }

        /// <summary>
        /// 空间地块集合(当前地域)
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

        /// <summary>
        /// 线状地物(当前地域)
        /// </summary>
        public List<XZDW> ListLineFeature { get; set; }

        /// <summary>
        /// 点状地物(当前地域)
        /// </summary>
        public List<DZDW> ListPointFeature { get; set; }

        /// <summary>
        /// 面状地物(当前地域)
        /// </summary>
        public List<MZDW> ListPolygonFeature { get; set; }

        /// <summary>
        /// 宗地图文件保存路径
        /// </summary>
        public string SavePathOfImage { get; set; }

        #endregion

        /// <summary>
        /// 承包地块业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        #region Ctor

        public ContractRegeditBookWork(IDbContext dbContext)
        {
            string value = ToolConfiguration.GetSpecialAppSettingValue("UseDefaultDirection", "true");
            Boolean.TryParse(value, out useDefaultDirection);
            useDefaultDirection = AgricultureSetting.SystemTableLandNeighborDirectory;
            spatialReference = dbContext.CreateSchema().GetElementSpatialReference(ObjectContext.Create(typeof(ContractLand)).Schema, ObjectContext.Create(typeof(ContractLand)).TableName);
            exportLandParcelMainOperation = new ExportLandParcelMainOperation();
        }

        #endregion

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
            base.OnSetParamValue(data);
            //GetAllMultiParcel();
            WriteInformation(data);
            base.Destroyed();
            return true;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void InitalizeDataInformation(object data)
        {
            ContractRegeditBookPrinterData dt = data as ContractRegeditBookPrinterData;
            Concord = dt.Concord.Clone() as ContractConcord;
            Contractor = dt.FamilyEntry.Clone() as VirtualPerson;
            CurrentZone = dt.CurrentZone.Clone() as Zone;
            ConstructType = ((int)eConstructMode.Family).ToString();
            Book = dt.Book.Clone() as YuLinTu.Library.Entity.ContractRegeditBook;
            Tissue = dt.Tissue.Clone() as CollectivityTissue;
            LandCollection = dt.ContractLands.Clone() as List<ContractLand>;
            if (Contractor.IsStockFarmer)
            {
                var stockLandsvp = DbContext.CreateBelongRelationWorkStation().GetLandByPerson(Contractor.ID, CurrentZone.FullCode);
                if (stockLandsvp.Count > 0)
                    LandCollection.AddRange(stockLandsvp);
            }
            AccountLandBusiness = dt.AccountLandBusiness;
            landStation = dt.DbContext.CreateContractLandWorkstation();
            ListLandDots = dt.ListLandDots;
            if (IsStockLand != null)
            {
                if ((bool)IsStockLand)
                {
                    geoLandCollection = ListGeoLand;
                }
                else
                {
                    geoLandCollection = Contractor != null ? ListGeoLand.FindAll(c => c.OwnerId == Contractor.ID) : new List<ContractLand>();//得到地块集合
                }
            }
            List<ContractLand> lands = new List<ContractLand>();
            if (IsStockLand != null)
            {
                if (!(bool)IsStockLand)
                {
                    foreach (ContractLand land in geoLandCollection)
                    {
                        if (dictDKLB.Any(c => !string.IsNullOrEmpty(c.Code) && c.Code.Equals(land.LandCategory)))
                        {
                            continue;
                        }
                        if (land.LandCategory == null)
                        {
                            YuLinTu.Library.Log.Log.WriteWarn(land, "地块示意图错误", "当前地块" + land.LandNumber + "地块类别为空");
                            throw new ArgumentNullException("地块示意图错误:" + "当前地块" + land.LandNumber + "地块类别为空");
                        }
                        lands.Add(land);
                    }
                }
                else
                {
                    lands = ListGeoLand.Clone();
                }
            }
            foreach (ContractLand land in lands)
            {
                geoLandCollection.Remove(land);
            }
        }

        #endregion

        #region Family

        /// <summary>
        /// 设置共有人值
        /// </summary>
        /// <param name="dt"></param>
        private void SetSharePersonValue(ContractRegeditBookPrinterData dt)
        {
            List<Person> person = dt.SharePersons;
            if (SystemSetDefine.GetIntence().PersonTable)
                person.Remove(person.Find(c => c.IsSharedLand.Equals("否")));
            int row = 17;
            if (person.Count - 10 > 0)
            {
                InsertTableRow(0, 17, person.Count - 10);
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
                string comment = person[i].Comment;

                SetTableCellValue(0, row, 0, InitalizeFamilyName(name));
                SetTableCellValue(0, row, 1, sex);
                SetTableCellValue(0, row, 2, Relationship);
                SetTableCellValue(0, row, 3, icn);
                SetTableCellValue(0, row, 4, string.IsNullOrEmpty(comment) ? "" : comment);
                row++;
            }
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        private void WriteFamilyInformation(ContractRegeditBookPrinterData dt)
        {
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("bmFamilyName", dt.ContractorName);
                SetBookmarkValue("bmFamilyCount", dt.SharePersons.Count.ToString());
                SetBookmarkValue("FamilyName" + (i == 0 ? "" : i.ToString()), dt.ContractorName);
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

        #endregion

        #region ContractLand

        private void SetContractLandValue(ContractRegeditBookPrinterData dt)
        {
            bool ActualAreaColumnWriteActualAreaWithLandBook = true; // 这个设置还需要探讨                   
            bool useNeighbor = true;
            string value = ToolConfiguration.GetSpecialAppSettingValue("LandBookNeighborSetting", "true");
            Boolean.TryParse(value, out useNeighbor);
            bool exportLandComment = true;
            value = ToolConfiguration.GetSpecialAppSettingValue("ExportLandBookCommentSetting", "false");
            Boolean.TryParse(value, out exportLandComment);

            int tableindex = 1;     //开始书写的表格位置   
            int rowindex = 2;    //开始书写的表格行位置
            LandCollection.Sort("IsStockLand", eOrder.Ascending);
            if (LandCollection != null || LandCollection.Count > 0)
            {
                var addRows = LandCollection.Count - 1;
                if (addRows > 0)
                {
                    InsertTableRow(tableindex, rowindex, addRows);
                }
                for (int i = 0; i < LandCollection.Count; i++)
                {
                    string name = LandCollection[i].Name;
                    string code = LandCollection[i].LandNumber;
                    string tableArea = (dt.NoWriteLandTableArea || LandCollection[i].TableArea == 0.0) ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].TableArea.ToString(), 2);
                    string awareArea = LandCollection[i].AwareArea == 0.0 ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].AwareArea.ToString(), 2);
                    string actualArea = LandCollection[i].ActualArea == 0.0 ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(LandCollection[i].ActualArea.ToString(), 2);
                    string area = ActualAreaColumnWriteActualAreaWithLandBook ? actualArea : awareArea; // AgricultureWarrantSetting.ActualAreaColumnWriteActualAreaWithLandBook

                    var dictdkdj = DictList.Find(c => c.Code == LandCollection[i].LandLevel && c.GroupCode == DictionaryTypeInfo.DLDJ);
                    string levelString = dictdkdj == null ? "" : dictdkdj.Name;

                    string comment = LandCollection[i].Comment != null ? LandCollection[i].Comment : "";

                    string locationXY = "";
                    var currentLandDots = ListLandDots.FindAll(d => d.LandID == LandCollection[i].ID);
                    if (currentLandDots != null && currentLandDots.Count >= 1)
                    {
                        //空间坐标的秭归导出表示法
                        //var currentLandValidDots = currentLandDots.FindAll(m => m.IsValid == true);
                        //currentLandValidDots = currentLandValidDots.Count == 0 ? currentLandDots : currentLandValidDots;
                        //var landDots = ResetUnitDotNumber(currentLandValidDots);
                        //foreach (var currentLandDot in currentLandValidDots)
                        //{
                        //    if (currentLandDot.Shape != null)
                        //    {
                        //        var currentLandDotCdts = currentLandDot.Shape.ToCoordinates();
                        //        locationXY += string.Format("{0}(X:{1},Y:{2})\r\n",
                        //            currentLandDot.UniteDotNumber,
                        //            ToolMath.CutNumericFormat(currentLandDotCdts[0].Y, 3),
                        //            ToolMath.CutNumericFormat(currentLandDotCdts[0].X, 3));
                        //    }
                        //}
                        ////末尾加上起始坐标
                        //var currentLandDotCdtsTemp = currentLandValidDots[0].Shape.ToCoordinates();
                        //locationXY += string.Format("{0}(X:{1},Y:{2})\r\n",
                        //    currentLandValidDots[0].UniteDotNumber,
                        //    ToolMath.CutNumericFormat(currentLandDotCdtsTemp[0].Y, 3),
                        //    ToolMath.CutNumericFormat(currentLandDotCdtsTemp[0].X, 3));

                        var currentLandValidDots = currentLandDots.FindAll(m => m.IsValid == true);
                        currentLandValidDots = currentLandValidDots.Count == 0 ? currentLandDots : currentLandValidDots;
                        foreach (var currentLandDot in currentLandValidDots)
                        {
                            if (currentLandDot.Shape != null)
                            {
                                var currentLandDotCdts = currentLandDot.Shape.ToCoordinates();
                                locationXY += string.Format("{0}({1},{2})\r\n",
                                    currentLandDot.DotNumber,
                                    ToolMath.CutNumericFormat(currentLandDotCdts[0].X, 3),
                                    ToolMath.CutNumericFormat(currentLandDotCdts[0].Y, 3));
                            }
                        }
                    }
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

                    SetTableCellValueDHDZ(tableindex, rowindex, 0, code);
                    SetTableCellValueDHDZ(tableindex, rowindex, 1, name);
                    SetTableCellValueDHDZ(tableindex, rowindex, 2, east);
                    SetTableCellValueDHDZ(tableindex, rowindex, 3, south);
                    SetTableCellValueDHDZ(tableindex, rowindex, 4, west);
                    SetTableCellValueDHDZ(tableindex, rowindex, 5, north);
                    SetTableCellValueDHDZ(tableindex, rowindex, 6, locationXY);
                    SetTableCellValueDHDZ(tableindex, rowindex, 7, tableArea);
                    SetTableCellValueDHDZ(tableindex, rowindex, 8, actualArea);
                    SetTableCellValueDHDZ(tableindex, rowindex, 9, awareArea);

                    SetTableCellValueDHDZ(tableindex, rowindex, 10, landpurpose);
                    SetTableCellValueDHDZ(tableindex, rowindex, 11, levelString);
                    SetTableCellValueDHDZ(tableindex, rowindex, 12, landName);
                    SetTableCellValueDHDZ(tableindex, rowindex, 13, isFarmerLand);
                    SetTableCellValueDHDZ(tableindex, rowindex, 14, comment);
                    rowindex++;
                }

            }

        }


        #endregion

        #region OtherInformation

        /// <summary>
        /// 填写信息
        /// </summary>
        private bool WriteInformation(object data)
        {
            List<Dictionary> landTypeDicts = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            ContractRegeditBookPrinterData dt = data as ContractRegeditBookPrinterData;
            SetBookmarkValue("County1", dt.CountyName);
            SetBookmarkValue("County", dt.CountyName);
            bool WriteContentsForTableWithLandBook = true; //设置还要探讨    登记薄中填写表格内容
            try
            {
                string number = ToolString.GetAllNumberWithInString(dt.ContractorAddress);
                string address = dt.isWriteAddress ? dt.ContractorAddress : (string.IsNullOrEmpty(number) ? dt.ContractorAddress : dt.ContractorAddress.Replace(number, ToolMath.GetChineseLowNumber(number)));
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("bmPriviceAndCounty" + (i == 0 ? "" : i.ToString()), dt.CityAndCountyName);
                    SetBookmarkValue("bmICNNumber" + (i == 0 ? "" : i.ToString()), dt.IcnNumber);
                    SetBookmarkValue("bmPosterNumber" + (i == 0 ? "" : i.ToString()), dt.PosterNumber);
                    SetBookmarkValue("bmAreaConcordTable" + (i == 0 ? "" : i.ToString()), dt.TableArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(dt.TableArea, 2));
                    SetBookmarkValue("bmBookNumber" + (i == 0 ? "" : i.ToString()), dt.BookNumber);
                    SetBookmarkValue("bmRegeditNumber" + (i == 0 ? "" : i.ToString()), dt.RegeditNumber);
                    SetBookmarkValue("bmSenderName" + (i == 0 ? "" : i.ToString()), dt.SenderName);
                    SetBookmarkValue("bmContractorName" + (i == 0 ? "" : i.ToString()), InitalizeFamilyName(dt.ContractorName));
                    SetBookmarkValue("bmContractorAddress" + (i == 0 ? "" : i.ToString()), address);
                    SetBookmarkValue("bmContractorTelephone" + (i == 0 ? "" : i.ToString()), dt.ContractorTelephone);
                    SetBookmarkValue("bmConcordNumber" + (i == 0 ? "" : i.ToString()), dt.ConcordNumber);
                    SetBookmarkValue("bmStartTime" + (i == 0 ? "" : i.ToString()), dt.StartTime);
                    SetBookmarkValue("bmEndTime" + (i == 0 ? "" : i.ToString()), dt.EndTime);
                    SetBookmarkValue("bmContractType" + (i == 0 ? "" : i.ToString()), landTypeDicts.Find(c => c.Code == dt.ContractType).Name);
                    SetBookmarkValue("bmUseType" + (i == 0 ? "" : i.ToString()), dt.UseType);
                    SetBookmarkValue("bmAreaLand" + (i == 0 ? "" : i.ToString()), ToolMath.SetNumbericFormat(dt.AreaLand, 2));
                    SetBookmarkValue("bmCountLand" + (i == 0 ? "" : i.ToString()), dt.CountLand);
                    SetBookmarkValue("bmLandTableArea" + (i == 0 ? "" : i.ToString()), dt.TableArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(dt.TableArea, 2));
                    SetBookmarkValue("bmLandActualArea" + (i == 0 ? "" : i.ToString()), dt.ActualArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(dt.ActualArea, 2));
                    SetBookmarkValue("bmLandAwareArea" + (i == 0 ? "" : i.ToString()), dt.AwareArea == "0" ? SystemSet.InitalizeAreaString() : ToolMath.SetNumbericFormat(dt.AwareArea, 2));
                }
                SetBookmarkValue("BookWarrantNumber", Book.Number);//登记簿代码
                SetBookmarkValue("SenderName", Tissue.Name);//发包方名称
                SetBookmarkValue("SenderLawyerName", Tissue.LawyerName);//发包方负责人
                SetBookmarkValue("SenderCode", Tissue.Code);//发包方代码
                SetBookmarkValue("ConcordMode", GetEnumDesp<eConstructMode>(Concord.ArableLandType));//合同承包方式
                SetBookmarkValue("ConcordNumber", Concord.ConcordNumber);//合同编号
                SetBookmarkValue(AgricultureBookMark.BookYear, Book.Year);//年号
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookexcursus, Book.ContractRegeditBookExcursus);//附记
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookPerson, Book.ContractRegeditBookPerson);//登簿人               
                SetBookmarkValue(AgricultureBookMark.BookContractRegeditBookTime, Book.ContractRegeditBookTime.HasValue ? Book.ContractRegeditBookTime.Value.ToString("yyyy年MM月dd日") : string.Empty);//打印所有颁证日期
                string date = "";
                if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue && Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
                {
                    var startDate = string.Format("{0}年{1}月{2}日", Concord.ArableLandStartTime.Value.Year, Concord.ArableLandStartTime.Value.Month, Concord.ArableLandStartTime.Value.Day);
                    var endDate = string.Format("{0}年{1}月{2}日", Concord.ArableLandEndTime.Value.Year, Concord.ArableLandEndTime.Value.Month, Concord.ArableLandEndTime.Value.Day);
                    endDate = !Concord.Flag ? endDate : "9999年1月1日";
                    date = string.Format("自{0}起至{1}止",startDate,endDate);
                }
                SetBookmarkValue("ConcordDate", date);//承包时间
                /* 修改于2016/09/22 权证流水号应该是6位 */
                SetBookmarkValue(AgricultureBookMark.BookSerialNumber, string.IsNullOrEmpty(Book.SerialNumber) ? "" : Book.SerialNumber.PadLeft(6, '0'));
                var simpleProvinceNamesDics = InitalizeSimpleProvice();
                string zoneName = CurrentZone.FullName;
                var simplenamedic = simpleProvinceNamesDics.Where(s => zoneName.Contains(s.Key)).FirstOrDefault();
                var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
                if (CurrentZone.FullName.Contains("西藏"))
                {
                    simplename = "藏";
                }
                SetBookmarkValue(AgricultureBookMark.SimpleProviceName, simplename);
                SetSharePersonValue(dt);
                InitalizeAllEngleView();//附图
                SetContractLandValue(dt);
                SetRequireDate();
                WriteFamilyInformation(dt);
                WriteStartAndEnd(dt);
                dt.Disponse();
                return true;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 显示所有鹰眼图
        /// </summary>
        private void InitalizeAllEngleView()
        {
            string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
            if (System.IO.File.Exists(fileName))
            {
                if (ParcelSettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                {
                    InsertImageCellWithoutPading(AgricultureBookMark.AgricultureAllShape, fileName, 320, 360);
                }
                else
                {
                    InsertImageCellWithoutPading(AgricultureBookMark.AgricultureAllShape, fileName, ParcelSettingDefine.ViewOfAllMultiParcelWitdh, ParcelSettingDefine.ViewOfAllMultiParcelHeight);
                }
            }
            System.IO.File.Delete(fileName);

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

        /// <summary>
        /// 写地域扩展书签
        /// </summary>
        private void WriteZoneExpressBookMark(ContractRegeditBookPrinterData dt)
        {
            SetBookmarkValue("bmCountryName", dt.CountyName);
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), dt.CountyName);
                SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), dt.CountyName.Substring(0, dt.CountyName.Length - 1));

            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), dt.PriviceName);
                SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), dt.PriviceName.Substring(0, dt.PriviceName.Length - 1));

            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), dt.CityName);
                SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), dt.CityName.Substring(0, dt.CityName.Length - 1));
            }
            for (int i = 0; i < 6; i++)
            {
                SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), dt.TownName);
                SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), dt.TownName.Substring(0, dt.TownName.Length - 1));
            }
            if (!string.IsNullOrEmpty(dt.VillageName))
            {
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), dt.VillageName);
                    SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), dt.VillageName.Substring(0, dt.VillageName.Length - 1).Replace("社区", "").Replace("街道办事处", ""));
                }
            }
            if (!string.IsNullOrEmpty(dt.GroupName))
            {
                for (int i = 0; i < 6; i++)
                {
                    SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), dt.GroupName);
                    string number = ToolString.GetLeftNumberWithInString(dt.GroupName);
                    string groupName = string.IsNullOrEmpty(number) ? dt.GroupName : ToolMath.GetChineseLowNumber(number);
                    SetBookmarkValue("LocationName" + (i == 0 ? "" : i.ToString()), CurrentZone.FullName);//座落
                    groupName = string.IsNullOrEmpty(number) ? dt.GroupName : dt.GroupName.Replace(number, ToolMath.GetChineseLowNumber(number));
                    SetBookmarkValue("GroupName" + (i == 0 ? "" : i.ToString()), groupName);
                    SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), dt.GroupName.Substring(0, dt.GroupName.Length - 1));
                }
            }
        }

        #endregion

        #region 附图

        #region Parcel

        /// <summary>
        /// 获取全地块示意图-本宗设置
        /// </summary>
        public void GetAllMultiParcel()
        {
            try
            {
                MapShapeUI map = null;
                BitmapSource image = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ParcelSettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                    {
                        ViewOfAllMultiParcel.Paper.Model.Width = 336;
                        ViewOfAllMultiParcel.Paper.Model.Height = 357;
                    }
                    else
                    {
                        ViewOfAllMultiParcel.Paper.Model.Width = ParcelSettingDefine.ViewOfAllMultiParcelWitdh;
                        ViewOfAllMultiParcel.Paper.Model.Height = ParcelSettingDefine.ViewOfAllMultiParcelHeight;
                    }
                }));
                List<FeatureObject> listAllFeature = new List<FeatureObject>();
                List<FeatureObject> listOwenrFeature = new List<FeatureObject>();
                List<FeatureObject> listdzdwFeature = new List<FeatureObject>();
                List<FeatureObject> listxzdwFeature = new List<FeatureObject>();
                List<FeatureObject> listmzdwFeature = new List<FeatureObject>();
                List<FeatureObject> listgroupZoneFeature = new List<FeatureObject>();
                List<FeatureObject> listvillageZoneFeature = new List<FeatureObject>();

                //当前地域下的所有空间地块图形导出
                DiagramBase diagram = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (ViewOfAllMultiParcel.Items.Count == 0)
                    {
                        diagram = new MapShape() { }.CreateDiagram();
                        ViewOfAllMultiParcel.Items.Add(diagram);
                        if (ParcelSettingDefine.IsFixedViewOfAllLandGeoWordExtend)
                        {
                            diagram.Model.Width = 300;
                            diagram.Model.Height = 300;
                        }
                        else
                        {
                            diagram.Model.Width = ParcelSettingDefine.ViewOfAllMultiParcelWitdh > 36 ? ParcelSettingDefine.ViewOfAllMultiParcelWitdh - 36 : ParcelSettingDefine.ViewOfAllMultiParcelWitdh;
                            diagram.Model.Height = ParcelSettingDefine.ViewOfAllMultiParcelHeight > 57 ? ParcelSettingDefine.ViewOfAllMultiParcelHeight - 57 : ParcelSettingDefine.ViewOfAllMultiParcelHeight;
                        }
                        diagram.Model.BorderWidth = 0;
                        diagram.Model.X = 15;
                        diagram.Model.Y = 10;
                    }
                    else
                    {
                        diagram = ViewOfAllMultiParcel.Items[0];
                    }
                    map = diagram.Content as MapShapeUI;
                    map.MapControl.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                }));
                int layerCountIndex = 0;
                DynamicGeoSource allGeos = null;
                DynamicGeoSource ownerGeos = null;
                if (ParcelSettingDefine.IsShowOtherVPallLands)
                    allGeos = GetSetALLOtherlands(listAllFeature, map, layerCountIndex++);
                if (ParcelSettingDefine.IsShowVPallLands)
                    ownerGeos = GetSetALLOwnerlands(listOwenrFeature, map, layerCountIndex++);
                if (ParcelSettingDefine.ShowdxmzdwHandle)
                {
                    GetSetALLdzdwGeos(listdzdwFeature, map, layerCountIndex++);
                    GetSetALLxzdwGeos(listxzdwFeature, map, layerCountIndex++);
                    GetSetALLmzdwGeos(listmzdwFeature, map, layerCountIndex++);
                }
                if (ParcelSettingDefine.IsShowGroupZoneBoundary)
                    GetSetGroupZoneGeos(listgroupZoneFeature, map, layerCountIndex++);
                if (ParcelSettingDefine.IsShowVillageZoneBoundary)
                    GetSetVillageZoneGeos(listvillageZoneFeature, map, layerCountIndex++);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    map.MapControl.SpatialReference = spatialReference;
                    Envelope geosFullExtend = null;
                    if (allGeos != null && ParcelSettingDefine.IsShowOtherVPallLands)
                    {
                        geosFullExtend = ownerGeos.FullExtend.ToGeometry().Union(allGeos.FullExtend.ToGeometry()).Buffer(10).GetEnvelope();
                    }
                    if ((ownerGeos != null && ParcelSettingDefine.IsShowOtherVPallLands == false) || (listAllFeature.Count == 0 && listOwenrFeature.Count > 0))
                    {
                        geosFullExtend = ownerGeos.FullExtend.ToGeometry().Buffer(10).GetEnvelope();
                    }
                    if (geosFullExtend != null)
                    {
                        map.MapControl.Extend = geosFullExtend;
                        map.MapControl.NavigateTo(geosFullExtend);
                        map.MapControl.Extend = geosFullExtend;
                    }
                }));

                for (int mi = layerCountIndex; mi < map.MapControl.Layers.Count; mi++)
                {
                    var pointLyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    var geoSource = pointLyer.DataSource as DynamicGeoSource;
                    geoSource.Clear();
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var env = map.MapControl.Extend;
                    int itemindex = 1;
                    //exportLandParcelMainOperation.AddMainCompass(ViewOfAllMultiParcel, diagram, itemindex++);    //添加主视图指北针
                    //if (ParcelSettingDefine.IsShowViewOfAllScale)
                    //{
                    //    exportLandParcelMainOperation.AddScaleText(ViewOfAllMultiParcel, diagram, env, itemindex++, ParcelSettingDefine.ViewOfAllScaleWH, true);    //添加比例尺文本标注                       
                    //}

                    //保存为图片
                    image = ViewOfAllMultiParcel.SaveToImage(3);
                    if (image == null)
                        return;
                    string fileName = SavePathOfImage + @"\" + Contractor.ZoneCode + "-" + Contractor.Name + ".jpg";
                    image.SaveToJpgFile(fileName);

                }));
                listAllFeature.Clear();
                listAllFeature = null;
                listOwenrFeature.Clear();
                listOwenrFeature = null;
                listdzdwFeature.Clear();
                listdzdwFeature = null;
                listxzdwFeature.Clear();
                listxzdwFeature = null;
                listmzdwFeature.Clear();
                listmzdwFeature = null;
            }
            catch (Exception ex)
            {
                throw new YltException("获取全地块示意图失败!");
            }
        }

        #endregion

        #region 全域所有信息获取与设置

        /// <summary>
        ///  全域获取设置临宗地块
        /// </summary>
        private DynamicGeoSource GetSetALLOtherlands(List<FeatureObject> listAllFeature, MapShapeUI map, int layerCountIndex)
        {
            //首先创建邻宗图层
            foreach (var land in ListGeoLand)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = land;
                fo.Geometry = land.Shape;
                fo.GeometryPropertyName = "Shape";
                if (geoLandCollection.Contains(land) == false) listAllFeature.Add(fo);
            }

            DynamicGeoSource allGeo = null;  //邻宗数据源 
            VectorLayer lyer = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyer = new VectorLayer();  //创建邻宗矢量图层
                    lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 255, 255, 255),
                        BorderStrokeColor = Color.FromArgb(255, 0, 255, 64),
                        BorderThickness = 1
                    });

                    allGeo = new DynamicGeoSource();
                    allGeo.AddRange(listAllFeature.ToArray());
                    lyer.DataSource = allGeo;
                    map.MapControl.Layers.Add(lyer);
                }
                else
                {
                    lyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    allGeo = lyer.DataSource as DynamicGeoSource;
                    allGeo.Clear();
                    allGeo.AddRange(listAllFeature.ToArray());
                }
                 ((lyer.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = ParcelSettingDefine.ViewOfAllOthervpLandColor;
                ((lyer.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = ParcelSettingDefine.ViewOfAllOthervpLandBorderWidth;
            }));
            return allGeo;
        }

        /// <summary>
        /// 本宗大区获取设置本宗地块
        /// </summary>
        /// <param name="listAllFeature"></param>
        /// <param name="map"></param>
        private DynamicGeoSource GetSetALLOwnerlands(List<FeatureObject> listOwenrFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建本宗图层
            foreach (var child in geoLandCollection)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listOwenrFeature.Add(fo);
            }

            DynamicGeoSource ownerGeo = null;  //本宗数据源
            VectorLayer lyerOwnerExtent = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerOwnerExtent = new VectorLayer();   //创建本宗矢量图层
                    lyerOwnerExtent.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerOwnerExtent.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 255, 62, 62),
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        BorderThickness = 1
                    });
                    lyerOwnerExtent.Labeler = new SimpleExpressionLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 13,
                        AllowOverlapping = true,
                        AllowTextOverflow = true,
                    })
                    {
                        LabelExpression = "LandNumber == null? LandNumber: (LandNumber.Length>5? Int32.Parse(LandNumber.Substring(LandNumber.Length-5,5)).ToString() : Int32.Parse(LandNumber).ToString() )",
                    };
                    ownerGeo = new DynamicGeoSource();
                    ownerGeo.AddRange(listOwenrFeature.ToArray());
                    lyerOwnerExtent.DataSource = ownerGeo;
                    map.MapControl.Layers.Add(lyerOwnerExtent);
                }
                else
                {
                    lyerOwnerExtent = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    ownerGeo = lyerOwnerExtent.DataSource as DynamicGeoSource;
                    ownerGeo.Clear();
                    ownerGeo.AddRange(listOwenrFeature.ToArray());
                }

                ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontFamily = ParcelSettingDefine.ViewOfAllLabelFontSet;
                ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontSize = ParcelSettingDefine.ViewOfAllLabelFontSize;
                var nowFontStyle = ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontStyle;

                if (ParcelSettingDefine.ViewOfAllLabelBold)
                    nowFontStyle = nowFontStyle | eFontStyle.Bold;
                if (ParcelSettingDefine.ViewOfAllLabelUnderLine)
                    nowFontStyle = nowFontStyle | eFontStyle.Underline;
                if (ParcelSettingDefine.ViewOfAllLabeltiltLine)
                    nowFontStyle = nowFontStyle | eFontStyle.Italic;
                if (ParcelSettingDefine.ViewOfAllLabelStrikeLine)
                    nowFontStyle = nowFontStyle | eFontStyle.Strikeout;

                ((lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Symbol as SimpleTextPolygonSymbolPerFeature).FontStyle = nowFontStyle;
                (lyerOwnerExtent.Labeler as SimpleExpressionLabeler).Enabled = ParcelSettingDefine.IsShowViewOfAllLabel;

                ((lyerOwnerExtent.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = ParcelSettingDefine.ViewOfAllvpLandColor;
                ((lyerOwnerExtent.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = ParcelSettingDefine.ViewOfAllvpLandBorderWidth;
            }));
            return ownerGeo;
        }

        /// <summary>
        /// 本宗大区获取设置点状要素
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetALLdzdwGeos(List<FeatureObject> listdzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建点状地物图层
            foreach (var child in ListPointFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listdzdwFeature.Add(fo);
            }
            DynamicGeoSource dzdwGeo = null;  //点状地物数据源
            VectorLayer lyerdzdw = null;
            //点状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerdzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyerdzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerdzdw.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 204, 225, 160),
                        BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderThickness = 0.3,
                    });
                    lyerdzdw.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 10
                    })
                    {
                        Enabled = false,
                        LabelProperty = "OwnerName"
                    };
                    dzdwGeo = new DynamicGeoSource();
                    dzdwGeo.AddRange(listdzdwFeature.ToArray());
                    lyerdzdw.DataSource = dzdwGeo;
                    map.MapControl.Layers.Add(lyerdzdw);
                }
                else
                {
                    lyerdzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    dzdwGeo = lyerdzdw.DataSource as DynamicGeoSource;
                    dzdwGeo.Clear();
                    dzdwGeo.AddRange(listdzdwFeature.ToArray());
                }
            }));
        }
        private void GetSetALLxzdwGeos(List<FeatureObject> listxzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建线状地物图层
            foreach (var child in ListLineFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listxzdwFeature.Add(fo);
            }

            DynamicGeoSource xzdwGeo = null;  //线状地物数据源
            VectorLayer lyerxzdw = null;
            //线状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerxzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyerxzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerxzdw.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 158, 148, 112),
                        StrokeThickness = 1,
                    });
                    lyerxzdw.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeature()
                    {
                        FontSize = 10
                    })
                    {
                        Enabled = false,
                        LabelProperty = "OwnerName"
                    };
                    xzdwGeo = new DynamicGeoSource();
                    xzdwGeo.AddRange(listxzdwFeature.ToArray());
                    lyerxzdw.DataSource = xzdwGeo;
                    map.MapControl.Layers.Add(lyerxzdw);
                }
                else
                {
                    lyerxzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    xzdwGeo = lyerxzdw.DataSource as DynamicGeoSource;
                    xzdwGeo.Clear();
                    xzdwGeo.AddRange(listxzdwFeature.ToArray());
                }
            }));
        }
        private void GetSetALLmzdwGeos(List<FeatureObject> listmzdwFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建面状地物图层
            foreach (var child in ListPolygonFeature)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = child;
                fo.Geometry = child.Shape;
                fo.GeometryPropertyName = "Shape";
                listmzdwFeature.Add(fo);
            }
            DynamicGeoSource mzdwGeo = null;  //面状地物数据源
            VectorLayer lyermzdw = null;
            //面状地物
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyermzdw = new VectorLayer();  //创建邻宗矢量图层
                    lyermzdw.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyermzdw.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Color.FromArgb(255, 239, 235, 219),
                        BorderStrokeColor = Color.FromArgb(255, 196, 191, 189),
                        BorderThickness = 1,
                    });
                    mzdwGeo = new DynamicGeoSource();
                    mzdwGeo.AddRange(listmzdwFeature.ToArray());
                    lyermzdw.DataSource = mzdwGeo;
                    map.MapControl.Layers.Add(lyermzdw);
                }
                else
                {
                    lyermzdw = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    mzdwGeo = lyermzdw.DataSource as DynamicGeoSource;
                    mzdwGeo.Clear();
                    mzdwGeo.AddRange(listmzdwFeature.ToArray());
                }
            }));

        }

        /// <summary>
        /// 本宗大区获取设置组级地域边界
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetGroupZoneGeos(List<FeatureObject> listgroupZoneFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建组级地物图层
            FeatureObject fo = new FeatureObject();
            fo.Object = CurrentZone;
            fo.Geometry = CurrentZone.Shape;
            fo.GeometryPropertyName = "Shape";
            listgroupZoneFeature.Add(fo);

            DynamicGeoSource zjdyGeo = null;  //组级地域数据源
            VectorLayer lyerzjdy = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerzjdy = new VectorLayer();  //创建邻宗矢量图层
                    lyerzjdy.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerzjdy.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        //BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderStrokeColor = Colors.Red,
                        BorderThickness = 2,
                    });
                    zjdyGeo = new DynamicGeoSource();
                    zjdyGeo.AddRange(listgroupZoneFeature.ToArray());
                    lyerzjdy.DataSource = zjdyGeo;
                    map.MapControl.Layers.Add(lyerzjdy);
                }
                else
                {
                    lyerzjdy = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    zjdyGeo = lyerzjdy.DataSource as DynamicGeoSource;
                    zjdyGeo.Clear();
                    zjdyGeo.AddRange(listgroupZoneFeature.ToArray());
                }
                ((lyerzjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = ParcelSettingDefine.GroupBoundaryBorderColor;
                ((lyerzjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = ParcelSettingDefine.GroupZoneBoundaryBorderWidth;
            }));
        }
        /// <summary>
        /// 本宗大区获取设置村级地域边界
        /// </summary>
        /// <param name="listdzdwFeature"></param>
        /// <param name="map"></param>
        private void GetSetVillageZoneGeos(List<FeatureObject> listvillageZoneFeature, MapShapeUI map, int layerCountIndex)
        {
            //创建村级地物图层
            FeatureObject fo = new FeatureObject();
            fo.Object = VillageZone;
            fo.Geometry = VillageZone.Shape;
            fo.GeometryPropertyName = "Shape";
            listvillageZoneFeature.Add(fo);

            DynamicGeoSource cjdyGeo = null;  //村级地域数据源
            VectorLayer lyercjdy = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyercjdy = new VectorLayer();  //创建邻宗矢量图层
                    lyercjdy.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyercjdy.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        //BorderStrokeColor = Color.FromArgb(255, 114, 137, 68),
                        BorderStrokeColor = Colors.Blue,
                        BorderThickness = 2,
                    });
                    cjdyGeo = new DynamicGeoSource();
                    cjdyGeo.AddRange(listvillageZoneFeature.ToArray());
                    lyercjdy.DataSource = cjdyGeo;
                    map.MapControl.Layers.Add(lyercjdy);
                }
                else
                {
                    lyercjdy = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    cjdyGeo = lyercjdy.DataSource as DynamicGeoSource;
                    cjdyGeo.Clear();
                    cjdyGeo.AddRange(listvillageZoneFeature.ToArray());
                }
                ((lyercjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = ParcelSettingDefine.VillageZoneBoundaryBorderColor;
                ((lyercjdy.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = ParcelSettingDefine.VillageZoneBoundaryBorderWidth;
            }));
        }
        #endregion

        #endregion

        #region helper

        /// <summary>
        /// 设置统编号连续，并在末尾添加起始点
        /// </summary>
        /// <param name="currentLandValidDots"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressDot> ResetUnitDotNumber(List<BuildLandBoundaryAddressDot> currentLandValidDots)
        {
            List<BuildLandBoundaryAddressDot> resultList = new List<BuildLandBoundaryAddressDot>();
            int startIndex = 0;
            if (currentLandValidDots != null && currentLandValidDots.Count != 0)
            {
                var firstDot = currentLandValidDots.FirstOrDefault();
                resultList.Add(firstDot);
                var firstUnitNumber = firstDot.UniteDotNumber;
                if (firstDot != null && firstUnitNumber.IsNotNullOrEmpty() && firstUnitNumber.Length > 1)
                {
                    int.TryParse(firstDot.UniteDotNumber.Substring(1), out startIndex);
                    var tag = firstDot.UniteDotNumber.Substring(0, 1);
                    for (int i = 1; i < currentLandValidDots.Count; i++)
                    {
                        currentLandValidDots[i].UniteDotNumber = tag + (++startIndex);
                        resultList.Add(currentLandValidDots[i]);
                    }
                }
            }

            return resultList;
        }

        /// 初始化省市简写
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> InitalizeSimpleProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "京");
            dic.Add("天津市", "津");
            dic.Add("河北省", "冀");
            dic.Add("山西省", "晋");
            dic.Add("内蒙古自治区", "内蒙古");
            dic.Add("辽宁省", "辽");
            dic.Add("吉林省", "吉");
            dic.Add("黑龙江省", "黑");
            dic.Add("上海市", "沪");
            dic.Add("江苏省", "苏");
            dic.Add("浙江省", "浙");
            dic.Add("安徽省", "皖");
            dic.Add("福建省", "闽");
            dic.Add("江西省", "赣");
            dic.Add("山东省", "鲁");
            dic.Add("河南省", "豫");
            dic.Add("湖北省", "鄂");
            dic.Add("湖南省", "湘");
            dic.Add("广东省", "粤");
            dic.Add("广西壮族自治区", "桂");
            dic.Add("海南省", "琼");
            dic.Add("重庆市", "渝");
            dic.Add("四川省", "川");
            dic.Add("贵州省", "贵");
            dic.Add("云南省", "云");
            dic.Add("西藏自治区", "藏");
            dic.Add("陕西省", "陕");
            dic.Add("甘肃省", "甘");
            dic.Add("青海省", "青");
            dic.Add("宁夏回族自治区", "宁");
            dic.Add("新疆维吾尔自治区", "新");
            dic.Add("香港特别行政区", "港");
            dic.Add("澳门特别行政区", "澳");
            dic.Add("台湾省", "台");
            return dic;
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
        #endregion

        #endregion
    }
}
