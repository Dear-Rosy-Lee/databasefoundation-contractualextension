using Aspose.Cells;
using Aspose.Cells.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 摸底调查核实表
    /// </summary>
    [Serializable]
    public class ExportLandVerifySingleExcelTable : ExportExcelBase
    {
        #region Fields

        private ToolProgress toolProgress;//进度条
        private int index;//下标
        private string templatePath;
        private int familyCount;//户数
        private int peopleCount;//家人数
        private int landCount;//总地块数
        private double AwareArea;//颁证面积
        private double TableArea = 0;//合同面积
        private double ActualArea;//实测面积
        private List<Dictionary> dictCBFLX;
        private List<Dictionary> dictXB;
        private List<Dictionary> dictZJLX;
        private List<Dictionary> dictTDYT;
        private List<Dictionary> dictDLDJ;
        private List<Dictionary> dicSF;
        private List<Dictionary> dictSYQXZ;
        private List<Dictionary> dictDKLB;
        private List<Dictionary> dictTDLYLX;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Tissue { get; set; }

        /// <summary>
        /// 承包方家庭信息
        /// </summary>
        public ContractAccountLandFamily AccountLandFamily { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine = SystemSetDefine.GetIntence();

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string TemplateFile { get; set; }

        public string Information { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        public IDbContext DbContext { get; set; }

        #endregion Properties

        #region Ctor

        public ExportLandVerifySingleExcelTable()
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
                dicSF = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.SF);
                dictCBFLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBFLX);
                dictXB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.XB);
                dictZJLX = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.ZJLX);
                dictTDYT = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDYT);
                dictDLDJ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DLDJ);
                dictSYQXZ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.SYQXZ);
                dictDKLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.DKLB);
                dictTDLYLX = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.TDLYLX);
            }
            if (CurrentZone == null)
            {
                return false;
            }

            if (AccountLandFamily == null)
                return false;

            toolProgress.DynamicProgress(ZoneDesc + AccountLandFamily.CurrentFamily.Name);
            WriteInformation(AccountLandFamily);
            InitalizeRangeValue("A" + 2, "E" + 2, $"单位:{Tissue.Name}");
            //WriteTempLate();
            SetLineType("A7", "P" + index, false);
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
            double TotalLandAware = 0;
            int aindex = index;
            int bindex = index;

            List<ContractLand> lands = landFamily.LandCollection;
            List<Person> peoples = landFamily.Persons;

            int rowcount = lands.Count > peoples.Count ? lands.Count : peoples.Count;
            Cells cells = workSheet.Cells;
            for (int i = 0; i < rowcount - 1; i++)
            {
                cells.InsertRow(6);
            }

            var hz = peoples.FirstOrDefault(f => f.Relationship == "01" || f.Relationship == "02"
            || f.Relationship == "户主" || f.Relationship == "本人");
            if (hz != null)
            {
                peoples.Remove(hz);
                peoples.Insert(0, hz);
            }

            peopleCount += peoples.Count;
            lands.Sort("IsStockLand", eOrder.Ascending);
            bool flag = false;
            if (peoples.Count > lands.Count)
                flag = true;
            foreach (Person person in peoples)
            {
                WritePersonInformation(person, bindex, flag);
                bindex++;
            }

            foreach (ContractLand land in lands)
            {
                TotalLandAware += land.AwareArea;
                WriteCurrentZoneInformation(land, aindex);
                aindex++;
            }

            landCount += lands.Count;
            if (lands.Count == 0)
            {
                //index++;
            }
            string landinfo = $"合计：{lands.Count}块\r\n总面积{TotalLandAware}亩";
            AwareArea += TotalLandAware;
            int getcode = GetCBFLXNumber(landFamily.CurrentFamily.FamilyExpand.ContractorType);
            Dictionary cardtype = dictCBFLX.Find(c => c.Code.Equals(getcode.ToString()));
            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), $"{landFamily.CurrentFamily.FamilyNumber}");
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name);
            InitalizeRangeValue("G" + index, "G" + (index + height - 1), landinfo);
            InitalizeRangeValue("O" + index, "O" + (index + height - 1), "");
            InitalizeRangeValue("P" + index, "P" + (index + height - 1), "");
            index += height;
            //workbook.Worksheets[0].HorizontalPageBreaks.Add("A" + index);
            //workbook.Worksheets[0].VerticalPageBreaks.Add("A" + index);
            lands.Clear();
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary>
        private void WriteCurrentZoneInformation(ContractLand land, int index)
        {
            InitalizeRangeValue("H" + index, "H" + index, land.Name.IsNullOrEmpty() ? "/" : land.Name);
            InitalizeRangeValue("I" + index, "I" + index, land.LandNumber.IsNullOrEmpty() ? "/" : land.LandNumber);
            InitalizeRangeValue("J" + index, "J" + index, (land.AwareArea > 0.0) ? ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("K" + index, "K" + index, land.NeighborEast != null ? land.NeighborEast : "/");
            InitalizeRangeValue("L" + index, "L" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
            InitalizeRangeValue("M" + index, "M" + index, land.NeighborWest != null ? land.NeighborWest : "/");
            InitalizeRangeValue("N" + index, "N" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
        }

        private void WritePersonInformation(Person person, int index, bool flag)
        {
            Dictionary gender = dictXB.Find(c => c.Code.Equals(person.Gender == eGender.Male ? "1" : "2"));
            InitalizeRangeValue("C" + index, "C" + index, person.Name.IsNullOrEmpty() ? "/" : person.Name);
            if (gender != null)
            {
                InitalizeRangeValue("D" + index, "D" + index, gender.Name + "性");
            }
            else
            {
                InitalizeRangeValue("D" + index, "D" + index, "");
            }
            InitalizeRangeValue("E" + index, "E" + index, person.ICN);
            InitalizeRangeValue("F" + index, "F" + index, person.Relationship);
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        private void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "AH2").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "AH" + 2, title);
            InitalizeRangeValue("C" + 3, "D" + 3, Tissue.Name);
            InitalizeRangeValue("F" + 3, "F" + 3, Tissue.Code);
            InitalizeRangeValue("I" + 3, "J" + 3, Tissue.LawyerName);
            var code = GetCardTypeNumber(Tissue.LawyerCredentType);
            Dictionary cardtype = dictZJLX.Find(c => c.Code.Equals(code.ToString()));
            InitalizeRangeValue("L" + 3, "L" + 3, cardtype.Name);
            InitalizeRangeValue("N" + 3, "P" + 3, Tissue.LawyerCartNumber);
            InitalizeRangeValue("R" + 3, "S" + 3, Tissue.LawyerTelephone);
            InitalizeRangeValue("W" + 3, "Z" + 3, Tissue.LawyerAddress);
            InitalizeRangeValue("AB" + 3, "AB" + 3, Tissue.LawyerPosterNumber);
            InitalizeRangeValue("AD" + 3, "AE" + 3, Tissue.SurveyPerson);
            DateTime surveyDate = new DateTime();
            if (Tissue.SurveyDate != null)
            {
                surveyDate = Convert.ToDateTime(Tissue.SurveyDate);
            }
            InitalizeRangeValue("AG" + 3, "AH" + 3, surveyDate.ToString("yyyy年MM月dd日"));

            WriteCount();
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 42.25, "合计");
            InitalizeRangeValue("B" + index, "F" + index, $"{familyCount} 户");
            InitalizeRangeValue("G" + index, "N" + index, $"{peopleCount} 人");
            InitalizeRangeValue("P" + index, "W" + index, $"{landCount} 块");
            InitalizeRangeValue("X" + index, "X" + index, $"{TableArea}亩");
            InitalizeRangeValue("Y" + index, "Y" + index, $"{AwareArea}亩");
            InitalizeRangeValue("Z" + index, "Z" + index, $"{AwareArea}亩");
            InitalizeRangeValue("AA" + index, "AA" + index, $"{ActualArea}亩");
            InitalizeRangeValue("AB" + index, "AB" + index, $"{ActualArea}亩");
            InitalizeRangeValue("AC" + index, "AC" + index, "\\");
            InitalizeRangeValue("AD" + index, "AD" + index, "\\");
            InitalizeRangeValue("AE" + index, "AE" + index, "\\");
            InitalizeRangeValue("AF" + index, "AF" + index, "\\");
            InitalizeRangeValue("AG" + index, "AG" + index, "\\");
            InitalizeRangeValue("AH" + index, "AH" + index, "\\");
            InitalizeRangeValue("AI" + index, "AI" + index, "\\");
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

        /// <summary>
        ///  获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>

        #endregion 开始生成Excel

        #endregion Methods
    }
}