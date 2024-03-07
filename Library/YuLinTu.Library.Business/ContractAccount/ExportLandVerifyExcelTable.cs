using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Library.WorkStation;
using YuLinTu.Data;
using System.Diagnostics;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 摸底调查核实表
    /// </summary>
    [Serializable]
    public class ExportLandVerifyExcelTable : ExportExcelBase
    {
        #region Fields

        private ToolProgress toolProgress;//进度条
        private int index;//下标
        private string templatePath;
        private string contracteeName;
        private bool showContractee;
        private int familyCount;//户数
        private int peopleCount;//家人数
        private int landCount;//总地块数
        private double AwareArea;//颁证面积
        private List<Dictionary> dictCBFLX;
        private List<Dictionary> dictXB;
        private List<Dictionary> dictZJLX;
        private List<Dictionary> dictTDYT;
        private List<Dictionary> dictDLDJ;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 发包方名称(按村发包)
        /// </summary>
        public string SenderNameVillage { get; set; }

        /// <summary>
        /// 发包方名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Tissue { get; set; }

        /// <summary>
        /// 承包方家庭信息集合
        /// </summary>
        public List<ContractAccountLandFamily> AccountLandFamily { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine = SystemSetDefine.GetIntence();

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        public List<ContractConcord> concords { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string TemplateFile { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 制表人
        /// </summary>
        public string DrawPerson { get; set; }

        /// <summary>
        /// 制表日期
        /// </summary>
        public DateTime? DrawDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckPerson { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? CheckDate { get; set; }

        public string Information { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前进度百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        #endregion Properties

        #region Ctor

        public ExportLandVerifyExcelTable()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "摸底调查核实表";
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion Ctor

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="templatePath"></param>
        public virtual bool BeginToZone(string templatePath)
        {
            //RePostProgress(1);
            if (!File.Exists(templatePath))
            {
                PostErrorInfo("模板路径不存在！");
                return false;
            }
            this.templatePath = templatePath;
            Write();//写数据
            return true;
        }

        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                //RePostProgress(5);
                OpenExcelFile();
                //RePostProgress(15);
                if (!SetValue())
                    return;
                BeginWrite();
                //RePostProgress(100);
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 非批量进度提示
        /// </summary>
        /// <param name="persent"></param>
        private void RePostProgress(int persent)
        {
            PostProgress(persent);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
        }

        /// <summary>
        /// 初始值
        /// </summary>
        private bool SetValue()
        {
            //RePostProgress(5);
            index = 6;
            return true;
        }

        #endregion 开始生成Excel之前的一系列操作

        #region 开始生成Excel

        private bool BeginWrite()
        {
            var dictStation = DbContext.CreateDictWorkStation();
            var DictList = dictStation.Get();
            if (DictList != null && DictList.Count > 0)
            {
                dictCBFLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBFLX);
                dictXB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.XB);
                dictZJLX = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.ZJLX);
                dictTDYT = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
                dictDLDJ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DLDJ);
            }
            if (CurrentZone == null)
            {
                return false;
            }

            if (AccountLandFamily == null || AccountLandFamily.Count == 0)
                return false;

            AccountLandFamily.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.CurrentFamily.FamilyNumber);
                long bNumber = Convert.ToInt64(b.CurrentFamily.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });

            showContractee = false;
            familyCount = AccountLandFamily.Count;
            toolProgress.InitializationPercent(AccountLandFamily.Count, 99, 1);

            foreach (ContractAccountLandFamily landFamily in AccountLandFamily)
            {
                toolProgress.DynamicProgress(ZoneDesc + landFamily.CurrentFamily.Name);
                WriteInformation(landFamily);
            }
            WriteTempLate();
            SetLineType("A6", "X" + index, false);
            this.Information = string.Format("{0}导出{1}条承包台账数据!", ZoneDesc, AccountLandFamily.Count);
            AccountLandFamily = null;
            return true;
        }

        /// <summary>
        /// 书写每个承包户信息
        /// </summary>
        /// <param name="virtualPerson"></param>
        private void WriteInformation(ContractAccountLandFamily landFamily)
        {
            if (landFamily == null)
                return;

            int height = (landFamily.LandCollection.Count > landFamily.Persons.Count) ? landFamily.LandCollection.Count : landFamily.Persons.Count;
            double TotallandAware = 0;
            int aindex = index;
            int bindex = index;

            List<ContractLand> lands = landFamily.LandCollection;
            List<Person> peoples = landFamily.Persons;
            peopleCount += peoples.Count;
            contracteeName = SystemDefine.CountryTableHead ? (string.IsNullOrEmpty(SenderNameVillage) ? "" : SenderNameVillage) : (string.IsNullOrEmpty(SenderName) ? "" : SenderName);
            showContractee = !string.IsNullOrEmpty(contracteeName);

            lands.Sort("IsStockLand", eOrder.Ascending);
        
            foreach (Person person in peoples)
            {
                WritePersonInformation(person, bindex);
                bindex++;
            }

            foreach (ContractLand land in lands)
            {
                TotallandAware += land.AwareArea;
                WriteCurrentZoneInformation(land, aindex);
                aindex++;
            }
         
            landCount += lands.Count;
            if (lands.Count == 0)
            {
                index++;
            }
            AwareArea += TotallandAware;
            int getcode = GetCBFLXNumber(landFamily.CurrentFamily.FamilyExpand.ContractorType);
            Dictionary cardtype = dictCBFLX.Find(c => c.Code.Equals(getcode.ToString()));
            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(5, '0');
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), $"{landFamily.CurrentFamily.ZoneCode}{result}");
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name.InitalizeFamilyName(SystemDefine.KeepRepeatFlag));
            InitalizeRangeValue("C" + index, "C" + (index + height - 1), cardtype.Name);
            InitalizeRangeValue("D" + index, "D" + (index + height - 1), peoples.Count);
            InitalizeRangeValue("O" + index, "O" + (index + height - 1), TotallandAware);
            InitalizeRangeValue("X" + index, "X" + (index + height - 1), "");
            ContractConcord concord = concords.Where(x => x.ContracterId == landFamily.CurrentFamily.ID).FirstOrDefault();
            if (concords.Count != 0)
            {
                InitalizeRangeValue("V" + index, "V" + (index + height - 1), concord.ConcordNumber);
            }
            index += height;
            lands.Clear();
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary>
        private void WriteCurrentZoneInformation(ContractLand land, int index)
        {
            InitalizeRangeValue("L" + index, "L" + index, land.Name.IsNullOrEmpty() ? "/" : land.Name);
            InitalizeRangeValue("M" + index, "M" + index, land.LandNumber.IsNullOrEmpty() ? "/" : land.LandNumber);
            InitalizeRangeValue("N" + index, "N" + index, (land.AwareArea > 0.0) ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("P" + index, "P" + index, land.NeighborEast != null ? land.NeighborEast : "/");
            InitalizeRangeValue("Q" + index, "Q" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
            InitalizeRangeValue("R" + index, "R" + index, land.NeighborWest != null ? land.NeighborWest : "/");
            InitalizeRangeValue("S" + index, "S" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
            Dictionary tdyt = dictTDYT.Find(c => c.Code.Equals(land.Purpose));
            Dictionary dldj = dictDLDJ.Find(c => c.Code.Equals(land.LandLevel));
            InitalizeRangeValue("T" + index, "T" + index, tdyt.Name);
            InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
            InitalizeRangeValue("W" + index, "W" + index, land.LandExpand.PublicityComment);
        }

        private void WritePersonInformation(Person person, int index)
        {
            Dictionary gender = dictXB.Find(c => c.Code.Equals(person.Gender == eGender.Male ? "1" : "2"));
            var code = GetCardTypeNumber(person.CardType);
            Dictionary cardtype = dictZJLX.Find(c => c.Code.Equals(code.ToString()));
            InitalizeRangeValue("E" + index, "E" + index, person.Name.IsNullOrEmpty() ? "/" : person.Name);
            InitalizeRangeValue("F" + index, "F" + index, gender.Name);
            InitalizeRangeValue("G" + index, "G" + index, cardtype.Name);
            InitalizeRangeValue("H" + index, "H" + index, person.ICN);
            InitalizeRangeValue("I" + index, "I" + index, person.Relationship);
            InitalizeRangeValue("J" + index, "J" + index, person.IsSharedLand);
            InitalizeRangeValue("K" + index, "K" + index, "");
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        private void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "X1").ToString();
            //var townName = GetParent(CurrentZone);
            title = $"{title}";
            InitalizeRangeValue("A" + 1, "X" + 1, title);
            if (showContractee)
            {
                InitalizeRangeValue("A" + 2, "D" + 2, string.Format("发包方:{0}", contracteeName));
            }

            var datatime = DateTime.Now.ToString("D");
            InitalizeRangeValue("E" + 2, "X" + 2, $"日期：{datatime}");

            WriteCount();
            index++;
            string information = "审核人";
            InitalizeRangeValue("W" + index, "W" + index, information);
            InitalizeRangeValue("X" + index, "X" + index, "");
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 42.25, "合计");
            InitalizeRangeValue("B" + index, "B" + index, familyCount > 0 ? familyCount.ToString() : "");
            string areaString = string.Format("合计:\n   {0}块", landCount);
            InitalizeRangeValue("C" + index, "C" + index, "\\");
            InitalizeRangeValue("D" + index, "D" + index, peopleCount);
            InitalizeRangeValue("E" + index, "E" + index, "\\");
            InitalizeRangeValue("F" + index, "F" + index, "\\");
            InitalizeRangeValue("G" + index, "G" + index, "\\");
            InitalizeRangeValue("H" + index, "H" + index, "\\");
            InitalizeRangeValue("I" + index, "I" + index, "\\");
            InitalizeRangeValue("J" + index, "J" + index, "\\");
            InitalizeRangeValue("K" + index, "K" + index, "\\");
            InitalizeRangeValue("L" + index, "L" + index, "\\");
            InitalizeRangeValue("M" + index, "M" + index, areaString);
            InitalizeRangeValue("N" + index, "N" + index, AwareArea);
            InitalizeRangeValue("P" + index, "P" + index, AwareArea);
            InitalizeRangeValue("Q" + index, "Q" + index, "\\");
            InitalizeRangeValue("R" + index, "R" + index, "\\");
            InitalizeRangeValue("S" + index, "S" + index, "\\");
            InitalizeRangeValue("T" + index, "T" + index, "\\");
            InitalizeRangeValue("U" + index, "U" + index, "\\");
            InitalizeRangeValue("V" + index, "V" + index, "\\");
            InitalizeRangeValue("W" + index, "W" + index, "\\");
            InitalizeRangeValue("X" + index, "X" + index, "\\");
        }

        private int GetCardTypeNumber(eCredentialsType type)
        {
            switch (type)
            {
                case eCredentialsType.IdentifyCard:
                    return 1;

                case eCredentialsType.AgentCard:
                    return 3;

                case eCredentialsType.OfficerCard:
                    return 2;

                case eCredentialsType.Other:
                    return 9;

                case eCredentialsType.Passport:
                    return 5;

                case eCredentialsType.ResidenceBooklet:
                    return 4;
            }
            return 0;
        }

        private int GetCBFLXNumber(eContractorType type)
        {
            switch (type)
            {
                case eContractorType.Farmer:
                    return 1;

                case eContractorType.Personal:
                    return 2;

                case eContractorType.Unit:
                    return 3;
            }
            return 0;
        }

        #endregion 开始生成Excel

        #endregion Methods
    }
}