/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    [Serializable]
    public class ExportContractorLandSingleSurveyTable : ExportExcelBase
    {
        #region Fields

        private bool result = true;
        private VirtualPerson vp;
        private int index;//下标
        private Zone currentZone;
        //private List<VirtualPerson> familys;//承包方集合
        private string templatePath;
        private List<VirtualPerson> tablefamilys;//承包方集合
        private ToolProgress toolProgress;
        private PublicityConfirmDefine contractLandOutputSurveyDefine = PublicityConfirmDefine.GetIntence();
        private SingleFamilySurveyDefine singleFamilySurveySetting = SingleFamilySurveyDefine.GetIntence();
        private SystemSetDefine SystemSet = SystemSetDefine.GetIntence();
        #endregion

        #region Propertys

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool ShowValue { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        ///// <summary>
        ///// 日期
        ///// </summary>
        //public DateTime? Date { get; set; }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor
        {
            get { return vp; }
            set { vp = value; }
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 二轮承包方集合
        /// </summary>
        public List<VirtualPerson> TableFamilys
        {
            get { return tablefamilys; }
            set { tablefamilys = value; }
        }

        /// <summary>
        /// 字典内容
        /// </summary>
        public List<Dictionary> DictionList { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandArrays { get; set; }

        /// <summary>
        /// 二轮地块集合
        /// </summary>
        public List<SecondTableLand> TableLandArrays { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public List<ContractConcord> ConcordCollection { get; set; }

        /// <summary>
        /// 登记簿集合
        /// </summary>
        public List<ContractRegeditBook> BookColletion { get; set; }

        #endregion

        #region Ctor

        public ExportContractorLandSingleSurveyTable()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="templatePath"></param>
        public bool BeginToVirtualPerson(VirtualPerson vp, string templatePath)
        {
            result = true;
            //PostProgress(1);

            if (!File.Exists(templatePath))
                PostErrorInfo("模板路径不存在！");

            if (vp == null)
                PostErrorInfo("承包方信息无效。");



            this.templatePath = templatePath;
            this.vp = vp;

            Write();//写数据
            //PostProgress(100);           
            return result;
        }

        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                //PostProgress(5);
                OpenExcelFile();
                //PostProgress(15);

                if (!SetValue())
                    return;
                BeginWrite();

                if (ShowValue)
                {
                    Show();
                }
                else
                {
                    SaveAs(SaveFilePath);
                    Dispose();
                }

            }
            catch (System.Exception e)
            {

                PostErrorInfo(e.Message.ToString());
                Dispose();

            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
        }

        private void InitizeValue()
        {
            index = 1;
        }

        private bool SetValue()
        {
            InitizeValue();

            //PostProgress(5);

            //PostProgress(10);

            return true;
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in personCollection)
            {
                if (person.Name == houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        #endregion

        #region 开始生成Excel

        private bool BeginWrite()
        {
            WriteTitle();
            int length = 0;
            //double height = 19.5;
            int currentIndex = 0;

            #region 打印信息

            try
            {
                bool useActualArea = true;
                string areaValue = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
                Boolean.TryParse(areaValue, out useActualArea);//使用实测面积作为颁证面积

                toolProgress.InitializationPercent(100, 90);
                List<Person> fsp = vp.SharePersonList;
                List<Person> persons = SortSharePerson(fsp, vp.Name);
                if (SystemSet.PersonTable)
                    persons=persons.FindAll(c=>c.IsSharedLand.Equals("是"));

                List<ContractLand> landCollection = LandArrays;
                List<SecondTableLand> lands = TableLandArrays;
                int count = 0;
                if (lands != null)
                {
                    count = persons != null && lands != null && persons.Count > lands.Count ? persons.Count : lands.Count;
                }
                if ((lands == null || lands.Count == 0) && landCollection != null && landCollection.Count > 0)
                {
                    count = persons != null && lands != null && persons.Count > landCollection.Count ? persons.Count : landCollection.Count;
                }
                int landCount = count;
                if ((lands == null || lands.Count == 0) && landCollection.Count > 0)
                {
                    landCount = FilterLandCollection(landCollection);
                    landCount = landCount < landCollection.Count ? landCount : landCollection.Count;
                }
                count = persons != null && lands != null && persons.Count > landCount ? persons.Count : landCount;

                int increment = count >= 9 ? count - 9 : 0;
                for (int i = 0; i < increment; i++)
                {
                    InsertRowCell(8);
                }
                #region 打印户信息

                //打印户信息
                int number = 0;
                if (!string.IsNullOrEmpty(vp.FamilyNumber))
                {
                    Int32.TryParse(vp.FamilyNumber, out number);
                }
                SetRange("A6", "A" + (6 + count - 1), number > 0 ? number.ToString() : "");
                SetRange("B6", "B" + (6 + count - 1), vp.Name.InitalizeFamilyName(SystemSet.KeepRepeatFlag));
                SetRange("C6", "C" + (6 + count - 1), persons.Count.ToString());
                #endregion

                #region 打印人口信息

                //打印人口信息
                currentIndex = 6;
                foreach (Person person in persons)
                {
                    person.Name = InitalizeFamilyName(person.Name, SystemSet.KeepRepeatFlag);
                    SetRange("D" + currentIndex, "D" + currentIndex, person.Name == vp.Name ? person.Name : person.Name);
                    SetRange("E" + currentIndex, "E" + currentIndex, GetGender(person.Gender));
                    SetRange("F" + currentIndex, "F" + currentIndex, person.Age.IsNullOrEmpty()?( person.GetAge() < 1 ? "" : person.GetAge().ToString()):person.Age);
                    SetRange("G" + currentIndex, "G" + currentIndex, person.ICN);
                    if (person.Name == vp.Name && person.ICN == vp.Number)
                    {
                        person.Relationship = "户主";
                    }
                    SetRange("H" + currentIndex, "H" + currentIndex, person.Relationship);
                    currentIndex++;
                }
                #endregion

                #region 打印二轮承包台账

                string rangeValue = GetExcelRangeValue("N5", "N5").ToString();
                bool isLandNeighbor = rangeValue == "东" ? true : false;
                currentIndex = 6;
                if (lands != null)
                {
                    foreach (SecondTableLand land in lands)
                    {
                        if (land.TableArea == null || !land.TableArea.HasValue || land.TableArea.Value == 0.0)
                        {
                            continue;
                        }
                        SetRange("K" + currentIndex, "K" + currentIndex, ToolMath.SetNumbericFormat(land.TableArea.ToString(), 2));
                        SetRange("L" + currentIndex, "L" + currentIndex, singleFamilySurveySetting.SecondLandType ? land.LandName : "");
                        SetRange("M" + currentIndex, "M" + currentIndex, singleFamilySurveySetting.SecondLandName ? land.Name : "");
                        if (singleFamilySurveySetting.SecondLandNeighbor)
                        {
                            SetRange("N" + currentIndex, "N" + currentIndex, land.NeighborEast);//四至东
                            SetRange("O" + currentIndex, "O" + currentIndex, land.NeighborSouth);//四至南
                            SetRange("P" + currentIndex, "P" + currentIndex, land.NeighborWest);//四至西
                            SetRange("Q" + currentIndex, "Q" + currentIndex, land.NeighborNorth);//四至北
                        }
                        SetRange("R" + currentIndex, "R" + currentIndex, land.Comment);
                        currentIndex++;
                    }
                }
                VirtualPerson svp = TableFamilys == null ? null : TableFamilys.Find(c => c.Name == vp.Name && c.ZoneCode == vp.ZoneCode);
                if (svp != null)
                {
                    SetRange("I6", "I" + (currentIndex == 6 ? currentIndex : (currentIndex - 1)), svp != null ? svp.PersonCount : "");
                    SetRange("J6", "J" + +(currentIndex == 6 ? currentIndex : (currentIndex - 1)), svp != null ? svp.TotalArea : SystemSet.InitalizeAreaString());
                }
                double totalArea = 0.0;
                if ((lands == null || lands.Count == 0) && currentZone.FullCode.Substring(0, Zone.ZONE_PROVICE_LENGTH) != "52")
                {
                    foreach (ContractLand land in landCollection)
                    {
                        if (land.TableArea == null || !land.TableArea.HasValue || land.TableArea.Value == 0.0)
                        {
                            continue;
                        }
                        SetRange("K" + currentIndex, "K" + currentIndex, land.TableArea > 0 ? ToolMath.SetNumbericFormat(land.TableArea.ToString(), 2) : SystemSet.InitalizeAreaString());
                        SetRange("L" + currentIndex, "L" + currentIndex, singleFamilySurveySetting.SecondLandType ? land.LandName : "");
                        SetRange("M" + currentIndex, "M" + currentIndex, singleFamilySurveySetting.SecondLandName ? land.Name : "");
                        if (singleFamilySurveySetting.SecondLandNeighbor)
                        {
                            SetRange("N" + currentIndex, "N" + currentIndex, land.NeighborEast);//四至东
                            SetRange("O" + currentIndex, "O" + currentIndex, land.NeighborSouth);//四至南
                            SetRange("P" + currentIndex, "P" + currentIndex, land.NeighborWest);//四至西
                            SetRange("Q" + currentIndex, "Q" + currentIndex, land.NeighborNorth);//四至北
                        }
                        totalArea += land.TableArea.HasValue ? land.TableArea.Value : 0.0;
                        SetRange("R" + currentIndex, "R" + currentIndex, land.Comment);
                        currentIndex++;
                    }
                }
                if (svp == null && totalArea > 0)
                {
                    InitalizeRangeValue("J6", "J" + +(currentIndex == 6 ? currentIndex : (currentIndex - 1)), totalArea > 0 ? ToolMath.SetNumbericFormat(totalArea.ToString(), 2) : SystemSet.InitalizeAreaString());
                }
                if (svp == null && totalArea <= 0)
                {
                    InitalizeRangeValue("J6", "J" + +(currentIndex == 6 ? currentIndex : (currentIndex - 1)), "");
                }

                #endregion

                #region 打印承包台账

                if (!useActualArea)
                {
                    SetRange("C" + (16 + increment), "C" + (17 + increment), "确权总面积");
                    SetRange("D" + (16 + increment), "D" + (17 + increment), "确权面积");
                }
                int increIndex = landCollection.Count >= 9 ? landCollection.Count - 9 : 0;
                for (int i = 0; i < increIndex; i++)
                {
                    InsertRowCell(18 + increment + 3);
                }
                currentIndex = 18 + increment;
                totalArea = 0.0;
                foreach (ContractLand land in landCollection)
                {
                    //string landNumber = ContractLand.GetLandNumber(land.CadastralNumber);
                    //if (landNumber.Length > YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian)
                    //{
                    //    landNumber = landNumber.Substring(YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian);
                    //}
                    string landNumber = land.LandNumber;
                    SetRange("A" + currentIndex, "A" + currentIndex, landNumber);
                    SetRange("B" + currentIndex, "B" + currentIndex, land.Name);
                    totalArea += useActualArea ? land.ActualArea : land.AwareArea;
                    string areaString = useActualArea ? ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2) : ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2);
                    areaString = areaString == "0.00" ? SystemSet.InitalizeAreaString() : areaString;
                    SetRange("D" + currentIndex, "D" + currentIndex, areaString);
                    SetRange("E" + currentIndex, "E" + currentIndex, singleFamilySurveySetting.LandType ? land.LandName : "");
                    if (singleFamilySurveySetting.LandNeighbor)
                    {
                        SetRange("F" + currentIndex, "F" + currentIndex, land.NeighborEast);//四至东
                        SetRange("G" + currentIndex, "G" + currentIndex, land.NeighborSouth);//四至南
                        SetRange("H" + currentIndex, "H" + currentIndex, land.NeighborWest);//四至西
                        SetRange("I" + currentIndex, "I" + currentIndex, land.NeighborNorth);//四至北
                    }
                    if (land.ConstructMode == string.Empty)
                    {
                        land.ConstructMode = ((int)eConstructMode.Family).ToString();
                    }
                    if(land.ConstructMode!= ((int)eConstructMode.Family).ToString())
                        land.ConstructMode= ((int)eConstructMode.OtherContract).ToString();
                    string value = land.ConstructMode;
                    Dictionary dic = null;
                    if (DictionList != null)
                    {
                        dic = DictionList.Find(t => t.GroupCode == DictionaryTypeInfo.CBJYQQDFS && t.Code == value);
                    }
                    if (dic != null)
                    {
                        value = dic.Name;
                    }
                    SetRange("J" + currentIndex, "J" + currentIndex, singleFamilySurveySetting.LandContractMode ? value : "");
                    SetRange("K" + currentIndex, "K" + currentIndex, singleFamilySurveySetting.IsTransfer ? (land.IsTransfer ? "是" : "否") : "");
                    if (land.IsTransfer)
                    {
                        value = land.TransferType;
                        if (DictionList != null)
                        {
                            dic = DictionList.Find(t => t.GroupCode == DictionaryTypeInfo.LZLX && t.Code == value);
                        }
                        if (dic != null)
                        {
                            value = dic.Name;
                        }
                        SetRange("L" + currentIndex, "L" + currentIndex, singleFamilySurveySetting.TransferMode ? (value == "未知" ? "" : value) : "");
                        SetRange("M" + currentIndex, "M" + currentIndex, singleFamilySurveySetting.TransferTerm ? land.TransferTime : "");
                    }
                    value = land.PlatType;
                    if (DictionList != null)
                    {
                        dic = DictionList.Find(t => t.GroupCode == DictionaryTypeInfo.ZZLX && t.Code == value);
                    }
                    if (dic != null)
                    {
                        value = dic.Name;
                    }
                    SetRange("N" + currentIndex, "N" + currentIndex, singleFamilySurveySetting.LandPlatType ? ((value == "Other") ? "" : value) : "");
                    SetRange("O" + currentIndex, "R" + currentIndex, singleFamilySurveySetting.LandComment ? (!string.IsNullOrEmpty(land.Comment) ? land.Comment : "") : "");
                    currentIndex++;
                }
                string mergeArea = (landCollection != null && landCollection.Count > 0) ? (totalArea > 0 ? ToolMath.SetNumbericFormat(totalArea.ToString(), 2) : SystemSet.InitalizeAreaString()) : "";
                SetRange("C" + (18 + increment), "C" + (currentIndex == (18 + increment) ? currentIndex : (currentIndex - 1)), mergeArea);
                #endregion

                index += length;
                toolProgress.DynamicProgress();
                fsp = null;
                persons = null;
                landCollection = null;
                lands = null;
                svp = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            #endregion
            return true;
        }

        /// <summary>
        /// 过滤地块数据
        /// </summary>
        /// <returns></returns>
        private int FilterLandCollection(List<ContractLand> landCollection)
        {
            int count = 0;
            foreach (ContractLand land in landCollection)
            {
                if (land.TableArea == null || !land.TableArea.HasValue || land.TableArea.Value == 0.0)
                {
                    continue;
                }
                count++;
            }
            return count;
        }

        private string GetLandUsePersonType(eLandUsePersonType typeName)
        {
            switch (typeName)
            {
                case eLandUsePersonType.Individual:
                    return "个人";
                case eLandUsePersonType.Family:
                    return "户";
            }

            return "个人";
        }

        private string GetGender(eGender gender)
        {
            switch (gender)
            {
                case eGender.Male:
                    return "男";
                case eGender.Female:
                    return "女";
            }
            return "";
        }

        private void WriteTitle()
        {
            string titleName = InitalizeTitleName("农村土地承包经营权勘界确权调查确认表");
            if (!string.IsNullOrEmpty(titleName))
            {
                SetRange("A" + index, "R" + index, titleName);
            }
            index++;
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                string zoneStr = "单位：" + SystemSet.GetTBDWStr(CurrentZone);//UnitName;// +"乡、镇(社区服务中心)";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Village) + "村";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Group) + "组";
                SetRange("A" + index, "G" + index, zoneStr);
            }
            else
            {
                SetRange("A" + index, "G" + index, "单位:       乡、镇(社区服务中心）     村     组");
            }
            SetRange("H" + index, "L" + index, "户主电话号码： " + vp.Telephone);
            if (AgricultureSetting.ShowSecondTableDataNeighborWithDirection)
            {
                SetRange("N5", "N5", "东");
                SetRange("O5", "O5", "南");
                SetRange("P5", "P5", "西");
                SetRange("Q5", "Q5", "北");
            }
            else
            {
                SetRange("N5", "N5", "上");
                SetRange("O5", "O5", "下");
                SetRange("P5", "P5", "左");
                SetRange("Q5", "Q5", "右");
            }
            index += 4;
        }

        /// <summary>
        /// 初始化标题名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeTitleName(string titleName)
        {
            return SystemSet.GetTableHeaderStr(CurrentZone) + titleName;
            //if (AgricultureSetting.UseTemplateTitle)
            //{
            //    return "";
            //}
            //if (AgricultureSetting.UseTableSourceTitle)
            //{
            //    return GetTitle() + "区、县（市）" + titleName;
            //}
            //return AgricultureSetting.InitalizeTitle(currentZone) + titleName;
        }

        private string GetTitle()
        {
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {

                AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
                Zone group = new Zone();
                Zone village = new Zone();
                Zone town = new Zone();
                Zone county = new Zone();
                Zone city = new Zone();

                if (currentZone.Level == eZoneLevel.Group)
                {
                    group = currentZone;
                    village = alb.GetParent(group);
                    town = alb.GetParent(village);
                    county = alb.GetParent(town);
                    city = alb.GetParent(county);
                }
                if (currentZone.Level == eZoneLevel.Village)
                {
                    village = currentZone;
                    town = alb.GetParent(village);
                    county = alb.GetParent(town);
                    city = alb.GetParent(county);
                }
                if (currentZone.Level == eZoneLevel.Town)
                {
                    town = currentZone;
                    county = alb.GetParent(town);
                    city = alb.GetParent(county);
                }

                if (city != null && county != null)
                {
                    string zoneName = county.FullName.Replace(city.FullName, "");
                    return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                }
                county = null;
                city = null;
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        //private string GetZoneName(Zone zone)
        //{
        //    Zone county = DB.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
        //    if (county == null)
        //    {
        //        return zone.FullName;
        //    }
        //    string name = zone.FullName.Replace(county.FullName, "");
        //    county = null;
        //    return name;
        //}

        //private string GetZoneName(string code, eZoneLevel level)
        //{
        //    Zone tempZone = DB.Zone.Get(code);
        //    AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
        //    Zone tempZone = alb.get
        //    if (tempZone.Level < level)
        //    {
        //        return GetZoneName(tempZone.UpLevelCode, level);
        //    }

        //    return tempZone.Name.Substring(0, tempZone.Name.Length - 1); ;
        //}

        private string GetName(string str, string[] parms)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string temp = str.Substring(str.Length - 1);
                foreach (string item in parms)
                {
                    if (temp == item)
                    {
                        return str.Substring(0, str.Length - 1);
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }
        #endregion

        #endregion
    }
}
