/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 填写地块相关信息
    /// </summary>
    partial class ContractWarrantPrinter
    {
        /// <summary>
        /// 设置地块信息
        /// </summary>
        protected virtual void SetContractLandValue(List<ContractLand> landCollection)
        {
            if (regBook != null)
                WriteFamilyLandCalInformation(landCollection);
            if (otherBook != null)
                WriteOtherLandCalInformation(landCollection);
        }

        /// <summary>
        /// 书写家庭承包方式地块统计信息
        /// </summary>
        protected virtual void WriteFamilyLandCalInformation(List<ContractLand> landCollection)
        {
            List<ContractLand> lands = landCollection.FindAll(ld => ld.ConstructMode == ((int)eConstructMode.Family).ToString()); //家庭承包方式
            double conLandActualArea = 0.0;
            double conLandawareArea = 0.0;
            lands.ForEach(l =>
            {
                conLandActualArea += l.ActualArea;
                conLandawareArea += l.AwareArea;
            });
            SetBookmarkValue("bmLandCount", lands.Count < 1 ? "  " : lands.Count.ToString());
            SetBookmarkValue("bmLandArea", conLandawareArea.ToString("0.00"));
            int index = 1;
            foreach (ContractLand land in lands)
            {
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
                SetBookmarkValue("bmLandNumber" + index.ToString(), land.LandNumber.IsNullOrBlank() ? "" : land.LandNumber);
                SetBookmarkValue("bmLandNeighbor" + index.ToString(), neighbor);

                SetBookmarkValue("EastName" + index.ToString(), land.NeighborEast);
                SetBookmarkValue("SouthName" + index.ToString(), land.NeighborSouth);
                SetBookmarkValue("WestName" + index.ToString(), land.NeighborWest);
                SetBookmarkValue("NorthName" + index.ToString(), land.NeighborNorth);

                SetBookmarkValue("bmLandArea" + index.ToString(), land.AwareArea.ToString("0.00"));
                SetBookmarkValue("bmLandIsFarmerLand" + index.ToString(), land.IsFarmerLand == null ? "" : (land.IsFarmerLand.Value ? "是" : "否"));
                SetBookmarkValue("bmLandComment" + index.ToString(), land.Comment.IsNullOrBlank() ? "" : land.Comment);

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
        protected virtual void WriteOtherLandCalInformation(List<ContractLand> landCollection)
        {
            SetBookmarkValue("bmOtherBookNumber", otherBook.Number);
            SetBookmarkValue("bmSenderName2", (Tissue == null || Tissue.Name.IsNullOrEmpty()) ? "/" : Tissue.Name);//发包方名称
            SetBookmarkValue("bmContracter2", InitalizeFamilyName(Contractor.Name));//承包方姓名
            SetBookmarkValue("bmIdentifyNumber2", Contractor.Number.IsNullOrEmpty() ? "/" : Contractor.Number);//承包方身份证号码
            if (otherConcord != null)
            {
                SetBookmarkValue("bmOtherConcordNumber", otherConcord.ConcordNumber);//其他承包方式合同编号
                string date = "";
                if (otherConcord.ArableLandStartTime != null && otherConcord.ArableLandStartTime.HasValue && otherConcord.ArableLandEndTime != null && otherConcord.ArableLandEndTime.HasValue)
                {
                    date = string.Format("{0}年{1}月{2}日", otherConcord.ArableLandStartTime.Value.Year, otherConcord.ArableLandStartTime.Value.Month, otherConcord.ArableLandStartTime.Value.Day) + "至"
                         + string.Format("{0}年{1}月{2}日", otherConcord.ArableLandEndTime.Value.Year, otherConcord.ArableLandEndTime.Value.Month, otherConcord.ArableLandEndTime.Value.Day) + "止";
                }
                SetBookmarkValue("bmOtherStartAndEndTime", otherConcord.Flag ? "长久" : date);//承包时间
            }

            List<ContractLand> otherLands = landCollection.FindAll(ld => ld.ConstructMode != ((int)eConstructMode.Family).ToString()); //其他方式承包
            double othLandActualArea = 0.0;
            double othLandawareArea = 0.0;
            foreach (ContractLand land in otherLands)
            {
                othLandActualArea += land.ActualArea;
                othLandawareArea += land.AwareArea;
            }
            SetBookmarkValue("bmOtherLandArea", othLandawareArea.ToString("0.00"));
            SetBookmarkValue("bmOtherLandCount", otherLands.Count < 1 ? "  " : otherLands.Count.ToString());

            int index = 1;
            foreach (ContractLand land in otherLands)
            {
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
                SetBookmarkValue("bmOtherLandNumber" + index.ToString(), land.LandNumber.IsNullOrBlank() ? "" : land.LandNumber);
                SetBookmarkValue("bmOtherLandNeighbor" + index.ToString(), neighbor);
                SetBookmarkValue("OtherEastName" + index.ToString(), land.NeighborEast);
                SetBookmarkValue("OtherSouthName" + index.ToString(), land.NeighborSouth);
                SetBookmarkValue("OtherWestName" + index.ToString(), land.NeighborWest);
                SetBookmarkValue("OtherNorthName" + index.ToString(), land.NeighborNorth);

                SetBookmarkValue("bmOtherLandArea" + index.ToString(), land.AwareArea.ToString("0.00"));
                SetBookmarkValue("bmOtherLandIsFarmerLand" + index.ToString(), land.IsFarmerLand == null ? "" : (land.IsFarmerLand.Value ? "是" : "否"));
                SetBookmarkValue("bmOtherLandComment" + index.ToString(), land.Comment.IsNullOrBlank() ? "" : land.Comment);

                index++;
                if (BookLandNum != null && BookLandNum.Value > 0 && index > BookLandNum.Value)
                {
                    break;
                }
            }
            if (otherLands != null && BookLandNum != null && otherLands.Count > BookLandNum.Value)
            {
                List<ContractLand> othLands = new List<ContractLand>();
                for (int indexStart = BookLandNum.Value; indexStart < otherLands.Count; indexStart++)
                {
                    othLands.Add(otherLands[indexStart]);
                }
                if (!UseExcel)
                {
                    AgriLandExporeeProgress(othLands, false);
                }
                else
                {
                    AgriLandExporessExcelProgress(othLands);
                }
            }
        }

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
        /// 设置地块编码值
        /// </summary>
        /// <returns></returns>
        private void SetLandNumberValue(int index, ContractLand land)
        {
            string landNumber = ExAgricultureString.BOOKLANDNUMBER + index.ToString();
            //string landValue = ContractLand.GetLandNumber(land.LandNumber);
            //if (landValue.Length > AgricultureSetting.AgricultureLandNumberMedian)
            //{
            //    landValue = landValue.Substring(AgricultureSetting.AgricultureLandNumberMedian);
            //}
            SetBookmarkValue(landNumber, land.LandNumber);//地块编号
        }

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
            return landCollection;
        }

        /// <summary>
        /// 设置角码
        /// </summary>
        private string SetSmallNumber(string smallNumber, string landNumber)
        {
            string landName = landNumber;
            int index = landNumber.IndexOf("-");
            if (index < 0)
            {
                index = landNumber.IndexOf("－");
            }
            if (index < 0)
            {
                return landName;
            }
            string surfix = landNumber.Substring(0, index);
            string number = ToolString.GetAllNumberWithInString(surfix);
            if (string.IsNullOrEmpty(number))
            {
                return landNumber;
            }
            SetBookmarkValue(smallNumber, number);
            landName = surfix.Replace(number, "") + "　" + landNumber.Substring(index, 3) + "\n" + landNumber.Substring(index + 3);
            return landName;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        /// <param name="printSetting"></param>
        /// <param name="index"></param>
        private void SetLandInformation(ContractLand land, int index)
        {
            //if (printSetting.ShowLevel)
            //{
            //string landLevel = ExAgricultureString.BOOKLANDLEVEL + index.ToString();
            //string levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? "" : EnumNameAttribute.GetDescription(land.LandLevel);
            //levelString = levelString == "未知" ? "" : levelString;
            //levelString = AgricultureSetting.UseSystemLandLevelDescription ? levelString : land.LandLevel;
            //SetBookmarkValue(landLevel, levelString);//耕保等级
            //}
            //if (printSetting.ShowLandType)
            //{
            //string landType = ExAgricultureString.BOOKLANDTYPE + index.ToString();
            //SetBookmarkValue(landType, string.IsNullOrEmpty(land.LandName) ? land.LandCategory : land.LandName);//地类
            //}
            //if (printSetting.ShowLandFigure)
            //{
            if (!SystemSet.NergionbourSet)
            {
                string landFigure = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                SetBookmarkValue(landFigure, "见附图");//附图
            }
            else
            {
                string landNeighbor = ExAgricultureString.BOOKLANDNEIGHBOR + index.ToString();
                WriteLandNeighbor(land, landNeighbor);
            }
            //string isFarmerLand = ExAgricultureString.BOOKISFARMERLAN + index.ToString();
            //SetBookmarkValue(isFarmerLand, ContractLand.GetFarmerLand(land.IsFarmerLand));
            //}
        }

        /// <summary>
        /// 打印土地四至
        /// </summary>
        private void WriteLandNeighbor(ContractLand land, string neighborIndex)
        {
            //if (string.IsNullOrEmpty(landNeighbor.Trim()))
            //{
            //    return;
            //}
            //string[] array = landNeighbor;
            //if (array.Length == 1)
            //{
            //    array = landNeighbor.Split('\n');
            //}
            //string column = neighborIndex + "1";
            //if (array.Length >= 1)
            //{
            //    SetBookmarkValue(column, showLandNeighborDirection ? "东:" + array[0] : array[0]);//四至东
            //}
            //column = neighborIndex + "2";
            //if (array.Length >= 3)
            //{
            //    SetBookmarkValue(column, showLandNeighborDirection ? "西:" + array[2] : array[2]);//四至西
            //}
            //column = neighborIndex + "3";
            //if (array.Length >= 2)
            //{
            //    SetBookmarkValue(column, showLandNeighborDirection ? "南:" + array[1] : array[1]);//四至南
            //}
            //column = neighborIndex + "4";
            //if (array.Length >= 4)
            //{
            //    SetBookmarkValue(column, showLandNeighborDirection ? "北:" + array[3] : array[3]);//四至北
            //}

            string neighbor = string.Format("东：{0}\n南：{1}\n西：{2} \n北：{3}", land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth);
            if (!SystemSet.NergionbourSortSet)
            {
                neighbor = string.Format("东：{0}\n西：{1}\n南：{2} \n北：{3}", land.NeighborEast, land.NeighborWest, land.NeighborSouth, land.NeighborNorth);
            }
            SetBookmarkValue(neighborIndex, neighbor);//四至
        }

        /// <summary>
        /// 获取地类
        /// </summary>
        /// <param name="landCode"></param>
        /// <returns></returns>
        //private string GetLandType(string landCode)
        //{
        //    LandType landType = db.LandType.SelectByCode(landCode);
        //    if (landType == null || landType.Name == "未知")
        //    {
        //        return "";
        //    }
        //    return landType.Name;
        //}

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        protected virtual void WriteSharePersonValue()
        {
            WriteSharePersonTitleInfo();

            if (Contractor == null)
            {
                return;
            }
            List<Person> sharePersons = Contractor.SharePersonList;
            sharePersons = RelationShipMapping.SortPersonListByRelation(sharePersons);
            int personCountIndex = 1;
            foreach (Person person in sharePersons)
            {
                string name = person.Name.IsNullOrEmpty() ? "" : person.Name;
                string relation = person.Relationship.IsNullOrEmpty() ? "" : person.Relationship;
                string comment = person.Comment.IsNullOrEmpty() ? "" : person.Comment;

                SetBookmarkValue("bmSharePersonName" + personCountIndex.ToString(), InitalizeFamilyName(name));
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

        protected virtual void WriteSharePersonTitleInfo()
        {
            if (regBook == null)
                return;
            SetBookmarkValue("bmFamilyBookNumber", regBook.Number);
            SetBookmarkValue("bmSenderName1", Tissue?.Name.GetSettingEmptyReplacement());//发包方名称
            SetBookmarkValue("bmContracter1", InitalizeFamilyName(Contractor.Name));//承包方姓名
            SetBookmarkValue("bmIdentifyNumber1", Contractor.Number.GetSettingEmptyReplacement());//承包方身份证号码
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
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection)
        {
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in personCollection)
            {
                if (person.Name == Concord.ContracterName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != Concord.ContracterName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
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
        /// 共有人数据处理
        /// </summary>
        /// <param name="sharePersons"></param>
        protected void AgriSharePersonExpress(List<Person> sharePersons)
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
        protected void AgriSharePersonExcelExpress(List<Person> sharePersons)
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

    /// <summary>
    /// 打印设置
    /// </summary>
    //public class PrintSetting
    //{
    //    /// <summary>
    //    /// 显示地块编号
    //    /// </summary>
    //    public bool ShowLandNumber
    //    {
    //        get { return AgriculturePrintSetting.IsPrintLandNumber; }
    //    }

    //    /// <summary>
    //    /// 显示等级
    //    /// </summary>
    //    public bool ShowLevel
    //    {
    //        get { return AgriculturePrintSetting.IsShowLandLevel; }
    //    }

    //    /// <summary>
    //    /// 显示地类
    //    /// </summary>
    //    public bool ShowLandType
    //    {
    //        get { return AgriculturePrintSetting.IsShowLandType; }
    //    }

    //    /// <summary>
    //    /// 打印附图
    //    /// </summary>
    //    public bool ShowLandFigure
    //    {
    //        get { return AgriculturePrintSetting.IsShowLandFigure; }
    //    }

    //    /// <summary>
    //    /// 打印四至
    //    /// </summary>
    //    public bool ShowLandNeighbor
    //    {
    //        get { return AgriculturePrintSetting.IsShowLandNeighbor; }
    //    }

    //    /// <summary>
    //    /// 打印基本农田
    //    /// </summary>
    //    public bool ShowIsFarmerLand
    //    {
    //        get { return AgriculturePrintSetting.IsShowBasicFarmerLand; }
    //    }

    //    /// <summary>
    //    /// 打印二轮台帐面积
    //    /// </summary>
    //    public bool ShowTabArea
    //    {
    //        get { return AgriculturePrintSetting.IsShowTabArea; }
    //    }

    //    public PrintSetting()
    //    {
    //    }
    //}
}