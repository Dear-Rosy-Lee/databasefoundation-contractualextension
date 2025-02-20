/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 单户确认表
    /// </summary>
    [Serializable]
    public class ExportSingleFamilyConfirmTable : ExportExcelBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportSingleFamilyConfirmTable(IDbContext db)
        {
            if (db == null)
                return;
            dbContext = db;
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

        #region Fields

        private IDbContext dbContext; //数据库业务
        private ToolProgress toolProgress;
        private string templatePath; //模板路径
        private VirtualPerson virtualPerson;   //当前承包方
        private int index; //下标

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatuDes { get; set; }

        /// <summary>
        /// 标题名称
        /// </summary>
        public string TitleName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 行政地域业务
        /// </summary>
        public ZoneDataBusiness ZoneBusiness { get; set; }

        /// <summary>
        /// 合同集合
        /// </summary>
        public List<ContractConcord> ListConcord { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 权证集合
        /// </summary>
        public List<ContractRegeditBook> ListRegeditBook { get; set; }
        private SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>  
        public void BeginToVirtualPerson(VirtualPerson vp, string templatePath)
        {
            PostProgress(1, StatuDes);

            if (!File.Exists(templatePath))
                PostErrorInfo("模板路径不存在！");

            if (vp == null)
                PostErrorInfo("承包方信息无效。");

            if (dbContext == null)
                PostErrorInfo("数据源错误！");

            this.templatePath = templatePath;
            this.virtualPerson = vp;

            Write();//写数据
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                PostProgress(5, StatuDes);
                OpenExcelFile();
                PostProgress(15, StatuDes);
                dbContext.OpenConnection();
                if (!SetValue())
                    return;

                BeginWrite();   //开始写数据

                dbContext.CloseConnection();
                toolProgress.DynamicProgress();
                SaveAs(SaveFilePath);    //保存文件
                PostProgress(100, StatuDes);
            }
            catch (Exception ex)
            {
                dbContext.CloseConnection();
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 打开模板文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
        }

        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <returns></returns>
        private bool SetValue()
        {
            index = 1;

            PostProgress(5);

            PostProgress(10);

            return true;
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
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

        /// <summary>
        /// 宗地排序
        /// </summary>    
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

        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始写数据
        /// </summary>
        /// <returns></returns>
        private bool BeginWrite()
        {
            WriteTitle();
            int length = 0;
            double height = 19.5;
            int currentIndex = 0;

            #region 打印信息

            try
            {
                toolProgress.InitializationPercent(100, 90);
                List<Person> listPerson = virtualPerson.SharePersonList;
                if (SystemSet.PersonTable)
                    listPerson=listPerson.FindAll(c=>c.IsSharedLand.Equals("是"));
                List<Person> persons = SortSharePerson(listPerson, virtualPerson.Name);  //对共有人进行排序
                List<ContractConcord> concords = ListConcord.FindAll(c => c.ContracterId != null && (c.ContracterId == virtualPerson.ID));
                if (concords == null)
                {
                    concords = new List<ContractConcord>();
                }
                List<ContractLand> landCollection = ListLand.FindAll(c => c.OwnerId != null && (c.OwnerId == virtualPerson.ID));
                landCollection = SortLandCollection(landCollection);
                int count = persons != null && persons.Count > landCollection.Count ? persons.Count : landCollection.Count;
                int increment = count >= 20 ? count - 20 : 0;
                for (int i = 0; i < increment; i++)
                {
                    InsertRowCell(10);
                }

                #region 打印户信息

                //打印户信息
                SetRange("A6", "A" + (count > 0 ? (6 + count - 1) : 6), VirtualPersonOperator.InitalizeFamilyName(virtualPerson.Name, SystemSet.KeepRepeatFlag));
                SetRange("B6", "B" + (count > 0 ? (6 + count - 1) : 6), listPerson.Count.ToString());

                #endregion

                #region 打印人口信息

                //打印人口信息
                currentIndex = 6;
                foreach (Person person in persons)
                {
                    SetRange("C" + currentIndex, "C" + currentIndex, person.Name == virtualPerson.Name ? VirtualPersonOperator.InitalizeFamilyName(person.Name, SystemSet.KeepRepeatFlag) : person.Name);
                    SetRange("D" + currentIndex, "D" + currentIndex, GetGender(person.Gender));
                    SetRange("E" + currentIndex, "E" + currentIndex, person.GetAge() < 1 ? "" : person.GetAge().ToString());
                    SetRange("F" + currentIndex, "F" + currentIndex, person.ICN);
                    if (person.Name == virtualPerson.Name && person.ICN == virtualPerson.Number)
                    {
                        person.Relationship = "户主";
                    }
                    SetRange("G" + currentIndex, "G" + currentIndex, person.Relationship);
                    currentIndex++;
                }

                #endregion

                #region 打印承包台账

                bool useActualArea = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
                Boolean.TryParse(value, out useActualArea);//使用实测面积作为颁证面积
                double totalArea = landCollection.Sum(ld => useActualArea ? ld.ActualArea : ld.AwareArea);
                currentIndex = 6;
                int curIndex = currentIndex;
                foreach (ContractConcord concord in concords)
                {
                    currentIndex = curIndex;
                    List<ContractLand> landArray = landCollection.FindAll(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId == concord.ID);
                    List<ContractLand> lands = new List<ContractLand>();
                    foreach (ContractLand land in landArray)
                    {
                        lands.Add(land);
                        landCollection.Remove(land);
                    }
                    ContractRegeditBook book = ListRegeditBook.Find(c => c.ID == concord.ID);
                    WriteLandInformation(currentIndex, lands, height, useActualArea, increment, concord.ConcordNumber, book == null ? "" : book.RegeditNumber, CalLandCollectionArea(lands, useActualArea), lands.Count > 1 ? true : false);
                    curIndex += lands.Count;
                }
                currentIndex = curIndex;
                if (landCollection.Count > 0)
                {
                    WriteLandInformation(currentIndex, landCollection, height, useActualArea, increment, "", "", CalLandCollectionArea(landCollection, useActualArea), landCollection.Count > 1 ? true : false);
                }
                SetRange("M" + (increment > 0 ? (26 + increment) : 26), "M" + (increment > 0 ? (26 + increment) : 26), totalArea.AreaFormat());

                #endregion

                index += length;
                toolProgress.DynamicProgress();
            }
            catch (Exception ex)
            {
                dbContext.CloseConnection();
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 填写地块信息
        /// </summary>    
        private void WriteLandInformation(int currentIndex, List<ContractLand> lands, double height, bool useActualArea, int increment, string concordNumber, string bookNumber, double allArea, bool merage)
        {
            double totalArea = 0.0;
            SetRange("H" + currentIndex, "H" + (lands.Count > 0 ? ((lands.Count == 1 && merage) ? (currentIndex + 1) : (currentIndex + lands.Count - 1)) : currentIndex), concordNumber);
            SetRange("I" + currentIndex, "I" + (lands.Count > 0 ? ((lands.Count == 1 && merage) ? (currentIndex + 1) : (currentIndex + lands.Count - 1)) : currentIndex), bookNumber);
            SetRange("J" + currentIndex, "J" + (lands.Count > 0 ? (currentIndex + lands.Count - 1) : currentIndex), allArea > 0.0 ? ToolMath.SetNumbericFormat(allArea.ToString(), 2) : SystemSet.InitalizeAreaString());
            foreach (ContractLand land in lands)
            {
                totalArea += useActualArea ? land.ActualArea : land.AwareArea;
                string landNumber = land.LandNumber;  
                SetRange("K" + currentIndex, "K" + currentIndex, landNumber);
                SetRange("L" + currentIndex, "L" + currentIndex, land.Name);
                SetRange("M" + currentIndex, "M" + currentIndex, useActualArea ? (land.ActualArea > 0.0 ? ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2) : SystemSet.InitalizeAreaString())
                : (land.AwareArea > 0.0 ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : SystemSet.InitalizeAreaString()));
                SetRange("N" + currentIndex, "N" + currentIndex, land.NeighborEast);//四至东
                SetRange("O" + currentIndex, "O" + currentIndex, land.NeighborSouth);//四至南
                SetRange("P" + currentIndex, "P" + currentIndex, land.NeighborWest);//四至西
                SetRange("Q" + currentIndex, "Q" + currentIndex, land.NeighborNorth);//四至北
                SetRange("R" + currentIndex, "R" + currentIndex, land.LandName);
                currentIndex++;
            }
        }

        /// <summary>
        /// 计算地块面积
        /// </summary>
        private double CalLandCollectionArea(List<ContractLand> lands, bool useActualArea)
        {
            double totalArea = 0.0;
            foreach (ContractLand land in lands)
            {
                totalArea += useActualArea ? land.ActualArea : land.AwareArea;
            }
            return totalArea;
        }

        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            string titleName = InitalizeTitleName("农村土地承包经营权确权登记单户确认表");
            if (!string.IsNullOrEmpty(titleName))
            {
                SetRange("A" + index, "R" + index, InitalizeTitleName("农村土地承包经营权确权登记单户确认表"));
            }
            index++;
            if (CurrentZone != null && CurrentZone.FullCode.Length > 0)
            {
                string zoneStr = "单位：" + TitleName;
                SetRange("A" + index, "G" + index,  zoneStr);
            }
            else
            {
                SetRange("A" + index, "G" + index,  "单位:       乡、镇(社区服务中心）     村     组");
            }
            SetRange("H" + index, "K" + index,  "日期： " + string.Format("{0: yyyy 年 MM 月 dd 日}", DateTime.Now));
            int number = 0;
            if (!string.IsNullOrEmpty(virtualPerson.FamilyNumber))
            {
                Int32.TryParse(virtualPerson.FamilyNumber, out number);
            }
            string format = AgricultureSetting.AgricultureLandEncodingRule == 0 ? "{0:D3}" : "{0:D4}";
            SetRange("L" + index, "R" + index,  "户号： " + string.Format(format, number));
            index += 4;
        }

        /// <summary>
        /// 初始化标题名称
        /// </summary>
        private string InitalizeTitleName(string titleName)
        {
            return UnitName + titleName;
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        private string GetTitle()
        {
            if (CurrentZone != null && CurrentZone.FullCode.Length > 0)
            {
                Zone county = ZoneBusiness.Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                Zone city = ZoneBusiness.Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                if (city != null && county != null)
                {
                    string zoneName = county.FullName.Replace(city.FullName, "");
                    return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                }
                return CurrentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>   
        private string GetZoneName(Zone zone)
        {
            Zone county = ZoneBusiness.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
            if (county == null)
            {
                return zone.FullName;
            }
            return zone.FullName.Replace(county.FullName, "");
        }

        /// <summary>
        /// 获取性别
        /// </summary>  
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
