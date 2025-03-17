using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Cells;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 摸底调查核实表
    /// </summary>
    [Serializable]
    public class ExportLandVerifyExcelTable : ExportExcelBase
    {
        #region Fields

        protected ToolProgress toolProgress;//进度条
        protected int cindex;
        protected int index;//下标
        protected string templatePath;
        protected int familyCount;//户数
        protected int peopleCount;//家人数
        protected int landCount;//总地块数
        protected double AwareArea;//颁证面积
        protected double TableArea = 0;//合同面积
        protected double ActualArea;//实测面积
        protected List<Dictionary> dictCBFLX;
        protected List<Dictionary> dictXB;
        protected List<Dictionary> dictZJLX;
        protected List<Dictionary> dictTDYT;
        protected List<Dictionary> dictDLDJ;
        protected List<Dictionary> dicSF;
        protected List<Dictionary> dictSYQXZ;
        protected List<Dictionary> dictDKLB;
        protected List<Dictionary> dictTDLYLX;
        protected List<EnumStore<eBHQK>> bhqkList;//变化情况
        protected List<string> jtmcs;

        #endregion Fields

        #region Properties

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

        public Worksheet Worksheet2 { get; set; }

        #endregion Properties

        #region Ctor

        public ExportLandVerifyExcelTable()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "摸底调查核实表";
            jtmcs = new List<string>() { "村集体", "社集体", "集体", "集体地", "组集体", "共有", "争议地" };
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
            index = 7;
            return true;
        }

        #endregion 开始生成Excel之前的一系列操作

        #region 开始生成Excel

        public virtual bool BeginWrite()
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

            if (AccountLandFamily == null || AccountLandFamily.Count == 0)
                return false;

            AccountLandFamily.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.CurrentFamily.FamilyNumber);
                long bNumber = Convert.ToInt64(b.CurrentFamily.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });

            if (!string.IsNullOrEmpty(AccountLandFamily[0].CurrentFamily.OldVirtualCode))
            {
                //AccountLandFamily.GroupBy(g=>g.CurrentFamily.)
                //var query = from d in AccountLandFamily
                //            orderby d.CurrentFamily.oldVirtualCode, d.CurrentFamily.FamilyNumber
                //            select d;
                //AccountLandFamily = query.ToList();
            }
            familyCount = AccountLandFamily.Count;
            toolProgress.InitializationPercent(AccountLandFamily.Count, 99, 1);
            //Worksheet2 = workbook.Worksheets.Add("ysb");
            //Worksheet2.Cells.SetColumnWidth(0, 21);
            //Worksheet2.Cells.SetColumnWidth(1, 21);
            //Worksheet2.Cells.SetColumnWidth(2, 25);
            //Worksheet2.Cells.SetColumnWidth(3, 25);
            bhqkList = EnumStore<eBHQK>.GetListByType();
            var delvps = AccountLandFamily.FindAll(t => t.CurrentFamily.Status == eVirtualPersonStatus.Bad);

            //HashSet<Guid> delset = new HashSet<Guid>();
            //foreach (var landFamily in AccountLandFamily)
            //{
            //    if (delset.Contains(landFamily.CurrentFamily.ID) || landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad)
            //        continue;
            //    var delvp = delvps.Find(t => (t.CurrentFamily.ZoneCode.PadRight(14, '0') + t.CurrentFamily.FamilyNumber.PadLeft(4, '0')) == landFamily.CurrentFamily.OldVirtualCode);
            //    if (delvp != null)
            //    {
            //        delset.Add(delvp.CurrentFamily.ID);
            //        foreach (var item in delvp.LandDelCollection)
            //        {
            //            if (landFamily.LandCollection.Any(t => t.LandNumber == item.DKBM))
            //                continue;
            //            landFamily.LandDelCollection.Add(item);
            //        }
            //    }
            //}
            //AccountLandFamily.RemoveAll(t => delset.Contains(t.CurrentFamily.ID));
            foreach (var landFamily in AccountLandFamily)
            {
                cindex++;
                toolProgress.DynamicProgress(ZoneDesc + landFamily.CurrentFamily.Name);
                WriteInformation(landFamily);
            }
            WriteTempLate();
            SetLineType("A7", "AI" + index, false);
            //Worksheet2.IsVisible = false;
            this.Information = string.Format("{0}导出{1}条承包台账数据!", ZoneDesc, AccountLandFamily.Count);
            AccountLandFamily = null;
            return true;
        }

        /// <summary>
        /// 书写每个承包户信息
        /// </summary>
        /// <param name="virtualPerson"></param>
        public virtual void WriteInformation(ContractAccountLandFamily landFamily)
        {
            if (landFamily == null)
                return;

            double TotalLandAware = 0;
            double TotalLandActual = 0;
            double TotalLandTable = 0;
            int aindex = index;
            int bindex = index;

            List<ContractLand> lands = landFamily.LandCollection;
            List<ContractLand_Del> landDels = landFamily.LandDelCollection;
            List<Person> peoples = landFamily.Persons;

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
                SetRowHeight(bindex - 1, 27.75);
                WritePersonInformation(person, bindex, flag);
                SetRowHeight(bindex, 27.75);
                bindex++;
            }
            HashSet<string> setlandnumber = new HashSet<string>();//手动设置关联的地块编码
            var familystr = ((int)eLandCategoryType.ContractLand).ToString();
            foreach (ContractLand land in lands)
            {
                double tableArea = land.TableArea ?? 0;
                TotalLandTable += tableArea;
                if (land.LandCategory == familystr)
                {
                    TotalLandAware += land.AwareArea;
                }
                TotalLandActual += land.ActualArea;
                WriteLandInformation(landFamily.CurrentFamily, land, aindex, land.LandCategory == familystr);
                SetRowHeight(aindex, 27.75);
                if (!string.IsNullOrEmpty(land.OldLandNumber) && !setlandnumber.Contains(land.OldLandNumber))
                {
                    setlandnumber.Add(land.OldLandNumber);
                }
                aindex++;
            }
            landDels.RemoveAll(t => setlandnumber.Contains(t.DKBM));
            foreach (ContractLand_Del landDel in landDels)
            {
                SetRowHeight(aindex, 27.75);
                WriteDelLandInformation(landFamily.CurrentFamily, landDel, aindex);
                aindex++;
            }
            int height = 0;
            if ((landFamily.LandCollection.Count + landFamily.LandDelCollection.Count) > landFamily.Persons.Count)
            {
                height = landFamily.LandCollection.Count + landFamily.LandDelCollection.Count;
            }
            else
            {
                height = landFamily.Persons.Count;
            }
            landCount += lands.Count + landDels.Count;
            height = height == 0 ? 1 : height;
            AwareArea += TotalLandAware;
            ActualArea += TotalLandActual;
            TableArea += TotalLandTable;
            int getcode = GetCBFLXNumber(landFamily.CurrentFamily.FamilyExpand.ContractorType);
            Dictionary cardtype = dictCBFLX.Find(c => c.Code.Equals(getcode.ToString()));
            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            string virtualpersonCode = $"{landFamily.CurrentFamily.ZoneCode.PadRight(14, '0')}{result}";
            if (landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad)
            {
                virtualpersonCode = landFamily.CurrentFamily.OldVirtualCode;
            }
            bool sfjt = jtmcs.Contains(landFamily.CurrentFamily.Name);
            string oldvpcode = string.IsNullOrEmpty(landFamily.CurrentFamily.OldVirtualCode) && landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad ? virtualpersonCode : landFamily.CurrentFamily.OldVirtualCode;
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), cindex);
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name);
            InitalizeRangeValue("C" + index, "C" + (index + height - 1), sfjt ? "" : cardtype.Name);
            InitalizeRangeValue("D" + index, "D" + (index + height - 1), sfjt ? "" : virtualpersonCode);
            //InitalizeSheet2RangeValue("A" + 1, "A" + 1, "c1", Worksheet2);
            //InitalizeSheet2RangeValue("A" + (index - 5), "A" + (index + height - 1 - 5), sfjt ? "" : virtualpersonCode, Worksheet2);
            //InitalizeSheet2RangeValue("B" + 1, "B" + 1, "c2", Worksheet2);
            //InitalizeSheet2RangeValue("B" + (index - 5), "B" + (index + height - 1 - 5), sfjt ? "" : oldvpcode, Worksheet2);
            InitalizeRangeValue("E" + index, "E" + (index + height - 1), landFamily.CurrentFamily.Telephone);
            InitalizeRangeValue("F" + index, "F" + (index + height - 1), landFamily.CurrentFamily.Address);
            InitalizeRangeValue("G" + index, "G" + (index + height - 1), sfjt ? 0 : landFamily.Persons.Count);
            InitalizeRangeValue("X" + index, "X" + (index + height - 1), TotalLandTable);
            InitalizeRangeValue("Z" + index, "Z" + (index + height - 1), TotalLandAware);
            InitalizeRangeValue("AB" + index, "AB" + (index + height - 1), TotalLandActual);
            if (landFamily.CurrentFamily.Status == eVirtualPersonStatus.Bad)
            {
                InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), "合同注销");
            }
            else
            {
                var bhqk = bhqkList.Find(t => t.Value == landFamily.CurrentFamily.ChangeSituation);
                if (bhqk != null)
                    InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), bhqk.DisplayName);
                else
                {
                    InitalizeRangeValue("AI" + index, "AI" + (index + height - 1), "其他直接顺延");
                }
            }
            index += height;
            //workbook.Worksheets[0].HorizontalPageBreaks.Add("A" + index);
            //workbook.Worksheets[0].VerticalPageBreaks.Add("A" + index);
            lands.Clear();
        }

        //public virtual void WriteCurrentZoneInformation(ContractLand_Del landDel, int index)
        //{
        //    InitalizeRangeValue("P" + index, "P" + index, landDel.DKMC.IsNullOrEmpty() ? "/" : landDel.DKMC);
        //    InitalizeRangeValue("Q" + index, "Q" + index, landDel.DKBM.IsNullOrEmpty() ? "/" : landDel.DKBM);
        //    InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
        //    InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM, Worksheet2);
        //    InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
        //    InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), landDel.QQDKBM.IsNullOrEmpty() ? "" : landDel.QQDKBM, Worksheet2);
        //    InitalizeRangeValue("Y" + index, "Y" + index, (landDel.QQMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.QQMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
        //    InitalizeRangeValue("AA" + index, "AA" + index, (landDel.SCMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.SCMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
        //    InitalizeRangeValue("AC" + index, "AC" + index, landDel.DKDZ != null ? landDel.DKDZ : "/");
        //    InitalizeRangeValue("AD" + index, "AD" + index, landDel.DKNZ != null ? landDel.DKNZ : "/");
        //    InitalizeRangeValue("AE" + index, "AE" + index, landDel.DKXZ != null ? landDel.DKXZ : "/");
        //    InitalizeRangeValue("AF" + index, "AF" + index, landDel.DKBZ != null ? landDel.DKBZ : "/");
        //    InitalizeRangeValue("AG" + index, "AH" + index, "删除");
        //}

        /// <summary>
        /// 书写当前地域信息
        /// </summary>
        //public virtual void WriteCurrentZoneInformation(ContractLand land, int index)
        //{
        //    Dictionary syqxz = dictSYQXZ.Find(c => c.Name.Equals(land.OwnRightType) || c.Code.Equals(land.OwnRightType));
        //    Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(land.LandCategory) || c.Code.Equals(land.LandCategory));
        //    Dictionary tdlylx = dictTDLYLX.Find(c => c.Name.Equals(land.LandCode) || c.Code.Equals(land.LandCode));
        //    Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(land.Purpose) || c.Code.Equals(land.Purpose));
        //    Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(land.LandLevel) || c.Code.Equals(land.LandLevel));
        //    InitalizeRangeValue("P" + index, "P" + index, land.Name.IsNullOrEmpty() ? "" : land.Name);
        //    InitalizeRangeValue("Q" + index, "Q" + index, land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber);
        //    InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
        //    InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber, Worksheet2);
        //    InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
        //    InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), land.OldLandNumber.IsNullOrEmpty() ? "" : land.OldLandNumber, Worksheet2);
        //    if (syqxz != null)
        //        InitalizeRangeValue("R" + index, "R" + index, syqxz.Name);
        //    if (dklb != null)
        //        InitalizeRangeValue("S" + index, "S" + index, dklb.Name);
        //    if (tdlylx != null)
        //        InitalizeRangeValue("T" + index, "T" + index, tdlylx.Name);
        //    if (dldj != null)
        //        InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
        //    if (tdyt != null)
        //        InitalizeRangeValue("V" + index, "V" + index, tdyt.Name);

        //    if (land.IsFarmerLand != null)
        //    {
        //        Dictionary SF = dicSF.Find(c => c.Code.Equals(land.IsFarmerLand == true ? "1" : "2"));
        //        InitalizeRangeValue("W" + index, "W" + index, SF.Name);
        //    }

        //    InitalizeRangeValue("Y" + index, "Y" + index, Math.Round(land.AwareArea, 2));
        //    InitalizeRangeValue("AA" + index, "AA" + index, Math.Round(land.ActualArea, 2));
        //    InitalizeRangeValue("AC" + index, "AC" + index, land.NeighborEast != null ? land.NeighborEast : "/");
        //    InitalizeRangeValue("AD" + index, "AD" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
        //    InitalizeRangeValue("AE" + index, "AE" + index, land.NeighborWest != null ? land.NeighborWest : "/");
        //    InitalizeRangeValue("AF" + index, "AF" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
        //    InitalizeRangeValue("AG" + index, "AH" + index, land.Comment);
        //}

        public virtual void WritePersonInformation(Person person, int index, bool flag)
        {
            Dictionary gender = dictXB.Find(c => c.Code.Equals(person.Gender == eGender.Male ? "1" : "2"));
            var code = GetCardTypeNumber(person.CardType);
            Dictionary cardtype = dictZJLX.Find(c => c.Code.Equals(code.ToString()));

            InitalizeRangeValue("H" + index, "H" + index, person.Name.IsNullOrEmpty() ? "/" : person.Name);
            if (gender != null)
            {
                InitalizeRangeValue("I" + index, "I" + index, gender.Name + "性");
            }
            else
            {
                InitalizeRangeValue("I" + index, "I" + index, "");
            }
            InitalizeRangeValue("J" + index, "J" + index, person.Telephone);
            InitalizeRangeValue("K" + index, "K" + index, cardtype.Name);
            InitalizeRangeValue("L" + index, "L" + index, person.ICN);
            InitalizeRangeValue("M" + index, "M" + index, person.Relationship);
            InitalizeRangeValue("N" + index, "N" + index, person.Comment);
            InitalizeRangeValue("O" + index, "O" + index, person.Opinion);
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        public virtual void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "AI2").ToString();
            title = $"{ZoneDesc}{title}";
            InitalizeRangeValue("A" + 1, "AI" + 2, title);
            if (Tissue == null)
                return;
            InitalizeRangeValue("C" + 3, "E" + 3, Tissue.Name);
            InitalizeRangeValue("G" + 3, "G" + 3, Tissue.Code);
            InitalizeRangeValue("J" + 3, "K" + 3, Tissue.LawyerName);
            var code = GetCardTypeNumber(Tissue.LawyerCredentType);
            Dictionary cardtype = dictZJLX.Find(c => c.Code.Equals(code.ToString()));
            InitalizeRangeValue("M" + 3, "N" + 3, cardtype.Name);
            InitalizeRangeValue("P" + 3, "S" + 3, Tissue.LawyerCartNumber);
            InitalizeRangeValue("U" + 3, "V" + 3, Tissue.LawyerTelephone);
            InitalizeRangeValue("Z" + 3, "AB" + 3, Tissue.LawyerAddress);
            InitalizeRangeValue("AD" + 3, "AD" + 3, Tissue.LawyerPosterNumber);
            InitalizeRangeValue("AF" + 3, "AG" + 3, Tissue.SurveyPerson);
            DateTime surveyDate = DateTime.Now;
            if (Tissue.SurveyDate != null)
            {
                surveyDate = Convert.ToDateTime(Tissue.SurveyDate);
            }
            InitalizeRangeValue("AI" + 3, "AI" + 3, surveyDate);

            WriteCount();
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        public virtual void WriteCount()
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

        protected virtual int GetCBFLXNumber(eContractorType type)
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
        /// 书写地块信息
        /// </summary>
        public virtual void WriteLandInformation(VirtualPerson vp, ContractLand land, int index, bool familycontract)
        {
            Dictionary syqxz = dictSYQXZ.Find(c => c.Name.Equals(land.OwnRightType) || c.Code.Equals(land.OwnRightType));
            Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(land.LandCategory) || c.Code.Equals(land.LandCategory));
            Dictionary tdlylx = dictTDLYLX.Find(c => c.Name.Equals(land.LandCode) || c.Code.Equals(land.LandCode));
            Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(land.Purpose) || c.Code.Equals(land.Purpose));
            Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(land.LandLevel) || c.Code.Equals(land.LandLevel));
            InitalizeRangeValue("P" + index, "P" + index, land.Name.IsNullOrEmpty() ? "" : land.Name);
            InitalizeRangeValue("Q" + index, "Q" + index, land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber);
            //InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
            //InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), land.LandNumber.IsNullOrEmpty() ? "" : land.LandNumber, Worksheet2);
            //InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
            //InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), land.OldLandNumber.IsNullOrEmpty() ? "" : land.OldLandNumber, Worksheet2);
            if (syqxz != null)
                InitalizeRangeValue("R" + index, "R" + index, syqxz.Name);
            if (dklb != null)
                InitalizeRangeValue("S" + index, "S" + index, dklb.Name);
            if (tdlylx != null)
                InitalizeRangeValue("T" + index, "T" + index, tdlylx.Name);
            if (dldj != null)
                InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
            if (tdyt != null)
                InitalizeRangeValue("V" + index, "V" + index, tdyt.Name);

            if (land.IsFarmerLand != null)
            {
                Dictionary SF = dicSF.Find(c => c.Code.Equals(land.IsFarmerLand == true ? "1" : "2"));
                InitalizeRangeValue("W" + index, "W" + index, SF.Name);
            }

            InitalizeRangeValue("Y" + index, "Y" + index, familycontract ? Math.Round(land.AwareArea, 2) : 0);
            InitalizeRangeValue("AA" + index, "AA" + index, Math.Round(land.ActualArea, 2));
            InitalizeRangeValue("AC" + index, "AC" + index, land.NeighborEast != null ? land.NeighborEast : "");
            InitalizeRangeValue("AD" + index, "AD" + index, land.NeighborSouth != null ? land.NeighborSouth : "");
            InitalizeRangeValue("AE" + index, "AE" + index, land.NeighborWest != null ? land.NeighborWest : "");
            InitalizeRangeValue("AF" + index, "AF" + index, land.NeighborNorth != null ? land.NeighborNorth : "");
            InitalizeRangeValue("AG" + index, "AH" + index, land.Comment);
        }

        /// <summary>
        /// 书写删除地块信息
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="landDel"></param>
        /// <param name="index"></param>
        public virtual void WriteDelLandInformation(VirtualPerson vp, ContractLand_Del landDel, int index)
        {
            InitalizeRangeValue("P" + index, "P" + index, landDel.DKMC.IsNullOrEmpty() ? "" : landDel.DKMC);
            InitalizeRangeValue("Q" + index, "Q" + index, landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM);
            //InitalizeSheet2RangeValue("C" + 1, "C" + 1, "d1", Worksheet2);
            //InitalizeSheet2RangeValue("C" + (index - 5), "C" + (index - 5), landDel.DKBM.IsNullOrEmpty() ? "" : landDel.DKBM, Worksheet2);
            //InitalizeSheet2RangeValue("D" + 1, "D" + 1, "d2", Worksheet2);
            //InitalizeSheet2RangeValue("D" + (index - 5), "D" + (index - 5), landDel.QQDKBM.IsNullOrEmpty() ? landDel.DKBM : landDel.QQDKBM, Worksheet2);
            InitalizeRangeValue("Y" + index, "Y" + index, (landDel.QQMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.QQMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AA" + index, "AA" + index, (landDel.SCMJ > 0.0) ? ToolMath.SetNumbericFormat(landDel.SCMJ.ToString(), 2) : SystemDefine.InitalizeAreaString());
            InitalizeRangeValue("AC" + index, "AC" + index, landDel.DKDZ != null ? landDel.DKDZ : "");
            InitalizeRangeValue("AD" + index, "AD" + index, landDel.DKNZ != null ? landDel.DKNZ : "");
            InitalizeRangeValue("AE" + index, "AE" + index, landDel.DKXZ != null ? landDel.DKXZ : "");
            InitalizeRangeValue("AF" + index, "AF" + index, landDel.DKBZ != null ? landDel.DKBZ : "");
            InitalizeRangeValue("AG" + index, "AH" + index, "删除");

            Dictionary syqxz = dictSYQXZ.Find(c => c.Name.Equals(landDel.SYQXZ) || c.Code.Equals(landDel.SYQXZ));
            Dictionary dklb = dictDKLB.Find(c => c.Name.Equals(landDel.DKLB) || c.Code.Equals(landDel.DKLB));
            Dictionary tdlylx = dictTDLYLX.Find(c => c.Name.Equals(landDel.TDLYLX) || c.Code.Equals(landDel.TDLYLX));
            Dictionary tdyt = dictTDYT.Find(c => c.Name.Equals(landDel.TDYT) || c.Code.Equals(landDel.TDYT));
            Dictionary dldj = dictDLDJ.Find(c => c.Name.Equals(landDel.DLDJ) || c.Code.Equals(landDel.DLDJ));


            if (syqxz != null)
                InitalizeRangeValue("R" + index, "R" + index, syqxz.Name);
            if (dklb != null)
                InitalizeRangeValue("S" + index, "S" + index, dklb.Name);
            if (tdlylx != null)
                InitalizeRangeValue("T" + index, "T" + index, tdlylx.Name);
            if (dldj != null)
                InitalizeRangeValue("U" + index, "U" + index, dldj.Name);
            if (tdyt != null)
                InitalizeRangeValue("V" + index, "V" + index, tdyt.Name);

            if (!string.IsNullOrEmpty(landDel.SFJBNT))
            {
                Dictionary SF = dicSF.Find(c => c.Code.Equals(landDel.SFJBNT));
                if (SF != null)
                    InitalizeRangeValue("W" + index, "W" + index, SF.Name);
            }

        }

        #endregion 开始生成Excel

        #endregion Methods
    }
}