using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.ExportWord
{
    /// <summary>
    /// 权证
    /// </summary>
    public class WarrantWord : AgricultureWordBook
    {
        #region Fields
        private string filePath;//保存文件路径
        private string proviceName;//省名称
        private string cityName;//市名称
        private string countryName;//区县名称
        private string countyCode;//区县编码
        private string townName = string.Empty;//镇名称
        private string villageName = string.Empty;//村名称
        private string groupName = string.Empty;//组名称
        private string fullName = string.Empty;//地域全称
        private bool isPrint;


        #endregion

        #region Propertys

        /// <summary>
        /// 批量导出
        /// </summary>
        public bool BatchExport { get; set; }

        /// <summary>
        /// 是否承包方汇总导出
        /// </summary>
        public bool IsDataSummaryExport { get; set; }

        /// <summary>
        /// 扩展人和地使用Excel文件
        /// </summary>
        public bool UseExcel { get; set; }

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum;

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum;

        /// <summary>
        /// 直到省的父级地域
        /// </summary>
        public List<Zone> ParentsToProvince { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="Concord">合同</param>
        public WarrantWord()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 打印承包经营权证
        /// </summary>
        public void PrintContractLand(bool isPrint)
        {
            this.isPrint = isPrint;
            if (Concord == null || Book == null || !GetTemplateFilePath())
            {
                return;
            }
            InitalizeData();
            try
            {
                if (!isPrint)
                {
                    int familyNumber = 0;
                    if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
                    {
                        Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
                    }
                    else
                    {
                        string landNumber = Concord != null ? Concord.ConcordNumber : "";
                        landNumber = (!string.IsNullOrEmpty(landNumber) && landNumber.Length >= 18) ? landNumber.Substring(14, 3) : "";
                        if (!string.IsNullOrEmpty(landNumber))
                        {
                            Int32.TryParse(landNumber, out familyNumber);
                        }
                    }
                    string familyName = (familyNumber == 0 ? Contractor.Name : (familyNumber + "-" + Contractor.Name));
                    var savePath = SystemSet.DefaultPath + @"" + familyName + " - 农村土地承包经营权证书";
                    this.PrintPreview(this, savePath);
                }
                else
                {
                    this.Print(this);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 导出承包经营权证
        /// </summary>
        public void ExportContractLand(string fileName)
        {
            filePath = fileName;
            CurrentZone = CurrentZone;
            if (Concord == null || Book == null || !GetTemplateFilePath())
            {
                return;
            }
            InitalizeData();
            try
            {
                int familyNumber = 0;
                if (!string.IsNullOrEmpty(Contractor.FamilyNumber))
                {
                    Int32.TryParse(Contractor.FamilyNumber, out familyNumber);
                }
                else
                {
                    string landNumber = Concord != null ? Concord.ConcordNumber : "";
                    landNumber = (!string.IsNullOrEmpty(landNumber) && landNumber.Length >= 18) ? landNumber.Substring(14, 3) : "";
                    if (!string.IsNullOrEmpty(landNumber))
                    {
                        Int32.TryParse(landNumber, out familyNumber);
                    }
                }
                string familyName = (familyNumber == 0 ? Contractor.Name : (familyNumber + "-" + Contractor.Name));
                string file = fileName;
                if (BatchExport)
                {
                    file += @"\农村土地承包经营权证书";
                }
                if (!Directory.Exists(file))
                {
                    Directory.CreateDirectory(file);
                }
                file += @"\";
                file += familyName;
                file += "-农村土地承包经营权证书";
                if (IsDataSummaryExport) file = filePath + @"\承包权证";
                if (Concord != null && Concord.ArableLandType != ((int)eConstructMode.Family).ToString())
                {
                    file += "(其它承包方式)";
                }

                SaveAs(this, file, true);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        private bool GetTemplateFilePath()
        {
            var filePath = TemplateHelper.WordTemplate(TemplateName);
            if (!File.Exists(filePath))//判断文件是否存在
            {
                return false;
            }
            if (!OpenTemplate(filePath))//打开文件
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitalizeData()
        {
            var zoneStation = DbContext.CreateZoneWorkStation();
            var ParentsToProvince = zoneStation.GetParentsToProvince(CurrentZone);
            LandCollection = StockLands;
            if (ParentsToProvince == null || ParentsToProvince.Count == 0)
                return;
            var province = ParentsToProvince.Find(c => c.Level == eZoneLevel.Province);
            var city = ParentsToProvince.Find(c => c.Level == eZoneLevel.City);
            var county = ParentsToProvince.Find(c => c.Level == eZoneLevel.County);
            var town = ParentsToProvince.Find(c => c.Level == eZoneLevel.Town);
            var village = ParentsToProvince.Find(c => c.Level == eZoneLevel.Village);
            var group = ParentsToProvince.Find(c => c.Level == eZoneLevel.Group);

            proviceName = province == null ? null : province.Name;
            cityName = city == null ? null : city.Name;
            countryName = county == null ? null : county.Name;
            countyCode = county == null ? string.Empty : county.FullCode;
            townName = town == null ? null : town.Name;
            villageName = village == null ? null : village.Name;
            groupName = group == null ? null : group.Name;
            fullName = CurrentZone.FullName == null ? null : CurrentZone.FullName.Replace(city.FullName, "");
        }

        /// <summary>
        /// 设置书签值
        /// </summary>
        protected override bool OnSetParamValue(object data)
        {
            return WriteBookInformation(data);
        }

        /// <summary>
        /// 填写信息
        /// </summary>
        /// <param name="data"></param>
        private bool WriteBookInformation(object data)
        {
            try
            {
                WriteZoneExpressBookMark();//设置其它书签
                List<ContractLand> landCollection = SortLandCollection(LandCollection);//获取地块集合
                WriteSharePersonValue();//设置共有人信息
                SetContractLandValue(landCollection);//设置地块值
                SetBookmarkValue("BookSerialNumber", Contractor.FamilyNumber?.PadLeft(6, '0'));
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                if (Book != null)
                {
                    year = Book.SendDate != null ? Book.SendDate.Year.ToString() : "";
                    month = Book.SendDate != null ? Book.SendDate.Month.ToString() : "";
                    day = Book.SendDate != null ? Book.SendDate.Day.ToString() : "";
                }
                string allAwareString = year + "年" + month + "月" + day + "日";
                SetBookmarkValue("BookAllAwareDate", allAwareString);
                landCollection = null;
                Disponse();//注销
                return true;
            }
            catch
            {
                this.Close();
                return false;
            }
        }

        /// <summary>
        /// 写地域扩展书签
        /// </summary>
        private void WriteZoneExpressBookMark()
        {
            var simpleProvinceNamesDics = InitalizeSimpleProvice();
            var simplenamedic = simpleProvinceNamesDics.Where(s => s.Key.Contains(proviceName)).FirstOrDefault();
            var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
            SetBookmarkValue(AgricultureBookMark.SimpleProviceName, simplename);
            string bookYear = Book == null ? "" : Book.Year;
            SetBookmarkValue(AgricultureBookMark.BookYear, "(" + bookYear + ")");
            SetBookmarkValue("County", countryName);
            string serialNumber = string.Empty;
            if (Book != null)
                serialNumber = string.IsNullOrEmpty(Book.SerialNumber) ? "" : Book.SerialNumber.PadLeft(6, '0');
            //else if (otherBook != null)
            //    serialNumber = string.IsNullOrEmpty(otherBook.SerialNumber) ? "" : otherBook.SerialNumber.PadLeft(6, '0');
            SetBookmarkValue(AgricultureBookMark.BookSerialNumber, serialNumber);


            string number = string.Empty;
            if (Book != null)
                number = "NO.J" + countyCode + (string.IsNullOrEmpty(Book.SerialNumber) ? "" : Book.SerialNumber.PadLeft(7, '0'));
            //else if (otherBook != null)
            //    number = "NO.J" + countyCode + (string.IsNullOrEmpty(otherBook.SerialNumber) ? "" : otherBook.SerialNumber.PadLeft(7, '0'));
            SetBookmarkValue(AgricultureBookMark.BookWarrantNumber, number);//权证编号

        }

        /// <summary>
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
        /// 注销
        /// </summary>
        private void Disponse()
        {
            Book = null;//承包权证
            Contractor = null;//承包方
        }

        #endregion

        #region 地块信息处理
        /// <summary>
        /// 设置地块信息
        /// </summary>
        private void SetContractLandValue(List<ContractLand> landCollection)
        {
            if (Book != null)
                WriteFamilyLandCalInformation(landCollection);
            //if (otherBook != null)
            //    WriteOtherLandCalInformation(landCollection);
        }

        /// <summary>
        /// 书写家庭承包方式地块统计信息
        /// </summary>
        private void WriteFamilyLandCalInformation(List<ContractLand> landCollection)
        {
            List<ContractLand> lands = landCollection.FindAll(ld => ld.ConstructMode == ((int)eConstructMode.Family).ToString()); //家庭承包方式
            double conLandActualArea = 0.0;
            double conLandawareArea = 0.0;
            lands.ForEach(l =>
            {
                conLandActualArea += l.ActualArea;
                conLandawareArea += DataHelper.GetQuantificationArea(Contractor,l,CurrentZone,DbContext);
            });
            SetBookmarkValue("bmLandCount", lands.Count < 1 ? "  " : lands.Count.ToString());
            SetBookmarkValue("bmLandArea", conLandawareArea.ToString("0.00"));
            int index = 1;
            foreach (ContractLand land in lands)
            {
                string numberName= land.LandNumber;
                if (SystemSet.LandNumericFormatSet)
                {
                    numberName = land.LandNumber.SimpleLandNumberFormatValue(SystemSet) + "\n" + land.Name;
                }
                else if (SystemSet.LandNumericFormatSet == false && land.LandNumber.IsNullOrEmpty() == false && land.LandNumber.Length == 19)
                {
                    numberName = land.LandNumber.Substring(0, 12) + "\n" + land.LandNumber.Substring(12, 7) + "\n" + land.Name;
                }
                else
                {
                    numberName = land.LandNumber + "\n" + land.Name;
                }               

                string neighbor = string.Empty;
                if (!SystemSet.NergionbourSet)
                {
                    neighbor = "见附图";
                }
                else
                {
                    neighbor = string.Format("东：{0}\n南：{1}\n西：{2}\n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
                    if (!SystemSet.NergionbourSortSet)
                    {
                        neighbor = string.Format("东：{0}\n西：{1}\n南：{2}\n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
                    }
                }
                SetBookmarkValue("bmLandNumber" + index.ToString(), numberName);
                SetBookmarkValue("bmLandNeighbor" + index.ToString(), neighbor);

                SetBookmarkValue("EastName" + index.ToString(), land.NeighborEast);
                SetBookmarkValue("SouthName" + index.ToString(), land.NeighborSouth);
                SetBookmarkValue("WestName" + index.ToString(), land.NeighborWest);
                SetBookmarkValue("NorthName" + index.ToString(), land.NeighborNorth);

                var quaArea = DataHelper.GetQuantificationArea(Contractor, land, CurrentZone, DbContext);
                var quaAreastr = "确份" + (quaArea != 0 ? quaArea.AreaFormat() : "0.00");
                SetBookmarkValue("bmLandArea" + index.ToString(), quaAreastr);
                SetBookmarkValue("bmLandIsFarmerLand" + index.ToString(), land.IsFarmerLand == null ? "" : (land.IsFarmerLand.Value ? "是" : "否"));
                //SetBookmarkValue("bmLandComment" + index.ToString(), land.Comment.IsNullOrBlank() ? "" : land.Comment);
                SetBookmarkValue("bmLandComment" + index.ToString(), "确权确股比\n例为:"+ Contractor.FamilyExpand.ExtendName??"");

                index++;
                if (BookLandNum != null && BookLandNum.Value > 0 && index > BookLandNum.Value)
                {
                    break;
                }
            }
            if (lands != null && BookLandNum != null && lands.Count > BookLandNum.Value)
            {
                List<ContractLand> familyLands = new List<ContractLand>();
                for (int indexStart = BookLandNum.Value; indexStart < lands.Count; indexStart++)
                {
                    familyLands.Add(lands[indexStart]);
                }
                if (!UseExcel)
                {
                    AgriLandExporeeProgress(familyLands, true);
                }
                else
                {
                    AgriLandExporessExcelProgress(familyLands);
                }
            }
        }

        /// <summary>
        /// 书写其他承包方式地块统计信息
        /// </summary>
        //private void WriteOtherLandCalInformation(List<ContractLand> landCollection)
        //{
        //    SetBookmarkValue("bmOtherBookNumber", otherBook.Number);
        //    SetBookmarkValue("bmSenderName2", (Tissue == null || Tissue.Name.IsNullOrEmpty()) ? "/" : Tissue.Name);//发包方名称
        //    SetBookmarkValue("bmContracter2", InitalizeFamilyName(Contractor.Name));//承包方姓名
        //    SetBookmarkValue("bmIdentifyNumber2", Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码
        //    if (otherConcord != null)
        //    {
        //        SetBookmarkValue("bmOtherConcordNumber", otherConcord.ConcordNumber);//其他承包方式合同编号
        //        string date = "";
        //        if (otherConcord.ArableLandStartTime != null && otherConcord.ArableLandStartTime.HasValue && otherConcord.ArableLandEndTime != null && otherConcord.ArableLandEndTime.HasValue)
        //        {
        //            date = string.Format("{0}年{1}月{2}日", otherConcord.ArableLandStartTime.Value.Year, otherConcord.ArableLandStartTime.Value.Month, otherConcord.ArableLandStartTime.Value.Day) + "至"
        //                 + string.Format("{0}年{1}月{2}日", otherConcord.ArableLandEndTime.Value.Year, otherConcord.ArableLandEndTime.Value.Month, otherConcord.ArableLandEndTime.Value.Day) + "止";
        //        }
        //        SetBookmarkValue("bmOtherStartAndEndTime", otherConcord.Flag ? "长久" : date);//承包时间
        //    }

        //    List<ContractLand> otherLands = landCollection.FindAll(ld => ld.ConstructMode != ((int)eConstructMode.Family).ToString()); //其他方式承包
        //    double othLandActualArea = 0.0;
        //    double othLandawareArea = 0.0;
        //    foreach (ContractLand land in otherLands)
        //    {
        //        othLandActualArea += land.ActualArea;
        //        othLandawareArea += land.AwareArea;
        //    }
        //    SetBookmarkValue("bmOtherLandArea", othLandawareArea.ToString("0.00"));
        //    SetBookmarkValue("bmOtherLandCount", otherLands.Count < 1 ? "  " : otherLands.Count.ToString());

        //    int index = 1;
        //    foreach (ContractLand land in otherLands)
        //    {
        //        string neighbor = string.Empty;
        //        if (!SystemSet.NergionbourSet)
        //        {
        //            neighbor = "见附图";
        //        }
        //        else
        //        {
        //            neighbor = string.Format("东：{0}\n南：{1}\n西：{2}\n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
        //            if (!SystemSet.NergionbourSortSet)
        //            {
        //                neighbor = string.Format("东：{0}\n西：{1}\n南：{2}\n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
        //            }
        //        }
        //        SetBookmarkValue("bmOtherLandNumber" + index.ToString(), land.LandNumber.IsNullOrBlank() ? "" : land.LandNumber);
        //        SetBookmarkValue("bmOtherLandNeighbor" + index.ToString(), neighbor);
        //        SetBookmarkValue("OtherEastName" + index.ToString(), land.NeighborEast);
        //        SetBookmarkValue("OtherSouthName" + index.ToString(), land.NeighborSouth);
        //        SetBookmarkValue("OtherWestName" + index.ToString(), land.NeighborWest);
        //        SetBookmarkValue("OtherNorthName" + index.ToString(), land.NeighborNorth);

        //        SetBookmarkValue("bmOtherLandArea" + index.ToString(), land.AwareArea.ToString("0.00"));
        //        SetBookmarkValue("bmOtherLandIsFarmerLand" + index.ToString(), land.IsFarmerLand == null ? "" : (land.IsFarmerLand.Value ? "是" : "否"));
        //        SetBookmarkValue("bmOtherLandComment" + index.ToString(), land.Comment.IsNullOrBlank() ? "" : land.Comment);

        //        index++;
        //        if (BookLandNum != null && BookLandNum.Value > 0 && index > BookLandNum.Value)
        //        {
        //            break;
        //        }
        //    }
        //    if (otherLands != null && BookLandNum != null && otherLands.Count > BookLandNum.Value)
        //    {
        //        List<ContractLand> othLands = new List<ContractLand>();
        //        for (int indexStart = BookLandNum.Value; indexStart < otherLands.Count; indexStart++)
        //        {
        //            othLands.Add(otherLands[indexStart]);
        //        }
        //        if (!UseExcel)
        //        {
        //            AgriLandExporeeProgress(othLands, false);
        //        }
        //        else
        //        {
        //            AgriLandExporessExcelProgress(othLands);
        //        }
        //    }
        //}

        /// <summary>
        /// 地块信息扩展处理
        /// </summary>
        private void AgriLandExporeeProgress(List<ContractLand> landCollection, bool isFamilyMode = true)
        {
            AgriLandExpressProgress landExpress = new AgriLandExpressProgress();
            landExpress.IsFamilyMode = isFamilyMode;
            landExpress.BatchExport = BatchExport;
            landExpress.Contractor = Contractor;
            landExpress.LandCollection = landCollection;
            landExpress.DictList = DictList;
            landExpress.BookLandNum = BookLandNum;
            landExpress.BookPersonNum = BookPersonNum;
            landExpress.SystemSet = SystemSet;
            landExpress.CurrentZone = CurrentZone;
            landExpress.DbContext = DbContext;
            landExpress.IsDataSummaryExport = IsDataSummaryExport;
            landExpress.InitalizeAgriLandExpress(isPrint, filePath);

            landCollection.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 地块信息扩展处理
        /// </summary>
        private void AgriLandExporessExcelProgress(List<ContractLand> landCollection)
        {
            AgriLandExpressExcelProgress landExpress = new AgriLandExpressExcelProgress();
            landExpress.BatchExport = BatchExport;
            landExpress.Contractor = Contractor;
            landExpress.LandCollection = landCollection;
            landExpress.DictList = DictList;
            landExpress.BookLandNum = BookLandNum;
            landExpress.BookPersonNum = BookPersonNum;
            landExpress.IsDataSummaryExport = IsDataSummaryExport;
            landExpress.InitalizeAgriLandExpress(isPrint, filePath);

            landCollection.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
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
            return landCollection;
        }

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonValue()
        {
            if (Book == null)
                return;
            SetBookmarkValue("bmFamilyBookNumber", Book.Number);
            SetBookmarkValue("bmSenderName1", (Tissue == null || Tissue.Name.IsNullOrEmpty()) ? "/" : Tissue.Name);//发包方名称
            SetBookmarkValue("bmContracter1", InitalizeFamilyName(Contractor.Name));//承包方姓名
            SetBookmarkValue("bmIdentifyNumber1", Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码
            if (Concord != null)
            {
                SetBookmarkValue("bmFamilyConcordNumber", Concord.ConcordNumber);//合同编号
                string date = "";
                if (Concord.ArableLandStartTime != null && Concord.ArableLandStartTime.HasValue && Concord.ArableLandEndTime != null && Concord.ArableLandEndTime.HasValue)
                {
                    date = Concord.ArableLandStartTime.Value.ToString("yyyy年MM月dd日") + "至" + Concord.ArableLandEndTime.Value.ToString("yyyy年MM月dd日") + "止";
                }
                SetBookmarkValue("bmFamilyStartAndEndTime", Concord.Flag ? "长久" : date);//承包时间
            }
            if (Contractor == null)
            {
                return;
            }
            List<Person> sharePersons = Contractor.SharePersonList;
            int personCountIndex = 1;
            foreach (Person person in sharePersons)
            {
                string name = person.Name.IsNullOrEmpty() ? "" : person.Name;
                string relation = person.Relationship.IsNullOrEmpty() ? "" : person.Relationship;
                string comment = person.Comment.IsNullOrEmpty() ? "" : person.Comment;

                SetBookmarkValue("bmSharePersonName" + personCountIndex.ToString(), name);
                SetBookmarkValue("bmSharePersonRelation" + personCountIndex.ToString(), relation);
                SetBookmarkValue("bmSharePersonComment" + personCountIndex.ToString(), comment);

                personCountIndex++;
                if ((BookPersonNum != null && BookPersonNum.Value > 0)
                     && personCountIndex > BookPersonNum.Value)
                {
                    break;
                }
            }
            if (sharePersons != null && BookPersonNum != null && sharePersons.Count > BookPersonNum.Value)
            {
                sharePersons.RemoveRange(0, BookPersonNum.Value);
                if (!UseExcel)
                {
                    AgriSharePersonExpress(sharePersons);
                }
                else
                {
                    AgriSharePersonExcelExpress(sharePersons);
                }
            }
            sharePersons.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 共有人数据处理
        /// </summary>
        /// <param name="sharePersons"></param>
        private void AgriSharePersonExpress(List<Person> sharePersons)
        {
            AgriLandExpressProgress landExpress = new AgriLandExpressProgress();
            landExpress.BatchExport = BatchExport;
            landExpress.Contractor = Contractor;
            landExpress.SharePersonCollection = sharePersons;
            landExpress.Concord = Concord;
            landExpress.BookLandNum = BookLandNum;
            landExpress.BookPersonNum = BookPersonNum;
            landExpress.SystemSet = SystemSet;
            landExpress.IsDataSummaryExport = IsDataSummaryExport;
            landExpress.InitalizeSharePersonExpress(isPrint, filePath);

            sharePersons.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 共有人数据处理
        /// </summary>
        /// <param name="sharePersons"></param>
        private void AgriSharePersonExcelExpress(List<Person> sharePersons)
        {
            AgriLandExpressExcelProgress landExpress = new AgriLandExpressExcelProgress();
            landExpress.BatchExport = BatchExport;
            landExpress.Contractor = Contractor;
            landExpress.SharePersonCollection = sharePersons;
            landExpress.Concord = Concord;
            landExpress.BookLandNum = BookLandNum;
            landExpress.BookPersonNum = BookPersonNum;
            landExpress.IsDataSummaryExport = IsDataSummaryExport;
            landExpress.InitalizeSharePersonExpress(isPrint, filePath);

            sharePersons.Clear();
            GC.Collect();
        }
    }
    #endregion
}
