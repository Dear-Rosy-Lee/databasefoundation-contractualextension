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
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 调查信息公示表
    /// </summary>
    [Serializable]
    public class ExportLandInfomationExcelTable : ExportExcelBase
    {
        #region Fields

        private ToolProgress toolProgress;//进度条
        private int index;//下标
        private int cindex;
        private string templatePath;
        private string contracteeName;
        private bool showContractee;
        private int familyCount;//户数
        private int tableCount;//总地块数
        private int landCount;//总地块数
        private double tableArea;//合同面积
        private double actualArea;//实测面积
        private bool useTableArea;//使用台账面积

        #endregion

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
        /// 台账常规设置
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

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

        #endregion

        #region Ctor

        public ExportLandInfomationExcelTable()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "调查信息公示表";
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
            index = 5;
            return true;
        }

        #endregion

        #region 开始生成Excel

        private bool BeginWrite()
        {
            if (CurrentZone == null)
            {
                return false;
            }
            //string staticsFamily = ToolConfiguration.GetSpecialAppSettingValue("StaticsInformationByLandFamily", "true");
            bool yes = (bool)SettingDefine.DisplayCollectUsingCBdata;
            //Boolean.TryParse(staticsFamily, out yes);
            useTableArea = SettingDefine.SurvyInfoTableLandDisplay;   //设置调查信息公示表中的地块数显示
            if (AccountLandFamily == null || AccountLandFamily.Count == 0)
                return false;

            AccountLandFamily.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.CurrentFamily.FamilyNumber);
                long bNumber = Convert.ToInt64(b.CurrentFamily.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });
            if (yes)
            {
            //    //List<ContractAccountLandFamily> vpList = AccountLandFamily.FindAll(fm => (fm.CurrentFamily.Name.IndexOf("机动地") >= 0 || fm.CurrentFamily.Name.IndexOf("集体") >= 0));
            //    //foreach (ContractAccountLandFamily vpn in vpList)
            //    //{
            //    //    vpList.Remove(vpn);
            //    //}
            //    //vpList.Clear();
                //AccountLandFamily.RemoveAll(fm => (fm.CurrentFamily.Name.IndexOf("机动地") >= 0 || fm.CurrentFamily.Name.IndexOf("集体") >= 0));
                AccountLandFamily.RemoveAll(c => c.CurrentFamily.FamilyExpand.ContractorType != eContractorType.Farmer);
            }
            showContractee = false;
            familyCount = AccountLandFamily.Count;
            toolProgress.InitializationPercent(AccountLandFamily.Count, 99, 1);

            foreach (ContractAccountLandFamily landFamily in AccountLandFamily)
            {
                cindex++;
                toolProgress.DynamicProgress(ZoneDesc + landFamily.CurrentFamily.Name);
                WriteInformation(landFamily);
            }
            WriteTempLate();
            SetLineType("A5", "N" + (index - 1), false);
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
            string landTypeName = string.Empty;
            double landAreaCount = 0;
            double landtableCount = 0;
            int tableLandCount = 0;
            int actualLandCount = 0;
            List<ContractLand> lands = landFamily.LandCollection;
            contracteeName = SystemDefine.CountryTableHead ? (string.IsNullOrEmpty(SenderNameVillage) ? "" : SenderNameVillage) : (string.IsNullOrEmpty(SenderName) ? "" : SenderName);
            showContractee = !string.IsNullOrEmpty(contracteeName);
            int startrow = index;
            lands.Sort("IsStockLand", eOrder.Ascending);
            foreach (ContractLand land in lands)
            {
                landAreaCount += land.ActualArea;
                landtableCount += land.AwareArea;
                WriteCurrentZoneInformation(land, index);
                tableLandCount += (land.AwareArea  > 0) ? 1 : 0;
                actualLandCount+= (land.ActualArea > 0) ? 1 : 0;
                index++;
            }
            landCount += useTableArea ? actualLandCount : lands.Count;
            tableCount += useTableArea ? tableLandCount : lands.Count;
            if (lands.Count == 0)
            {
                index++;
            }
            tableArea += landtableCount;
            actualArea += landAreaCount;
            InitalizeRangeValue("A" + startrow, "A" + (index - 1), cindex);
            InitalizeRangeValue("B" + startrow, "B" + (index - 1), landFamily.CurrentFamily.Name.InitalizeFamilyName(SystemDefine.KeepRepeatFlag));
            InitalizeRangeValue("N" + startrow, "N" + (index - 1), "");
            string htareainfostring = string.Empty;
            string scareainfostring = string.Empty;

            if (lands.Count > 1)
            {
                htareainfostring = string.Format("合计:\n   {0}块\n{1}亩", (useTableArea ? actualLandCount : lands.Count), ToolMath.SetNumbericFormat(landAreaCount.ToString(), 2));
                scareainfostring = string.Format("合计:\n   {0}块\n{1}亩", (useTableArea ? tableLandCount : lands.Count), ToolMath.SetNumbericFormat(landtableCount.ToString(), 2));

                int landcountstringlength = lands.Count.ToString().Length;
                int htareastringlength = htareainfostring.Length - landcountstringlength - 10;
                //SetCellValueUnderline(2, 7, landcountstringlength);
                //SetCellValueUnderline(2, 9 + landcountstringlength, htareastringlength);

                int tableareastringlength = tableLandCount.ToString().Length;
                int uselandcount;
                if (useTableArea)
                {
                    uselandcount = tableareastringlength;
                }
                else
                {
                    uselandcount = landcountstringlength;
                }
                int scareastringlength = scareainfostring.Length - uselandcount - 10;
                //SetCellValueUnderline(3, 7, uselandcount);
                //SetCellValueUnderline(3, 9 + uselandcount, scareastringlength);

                InitalizeRangeValuePublishExcelUse("D" + startrow, "D" + (index - 1), htareainfostring, 7, landcountstringlength, 9 + landcountstringlength, htareastringlength);
                InitalizeRangeValuePublishExcelUse("C" + startrow, "C" + (index - 1), scareainfostring, 7, uselandcount, 9 + uselandcount, scareastringlength);


            }
            else
            {
                htareainfostring = string.Format("合计:\n   {0}块\n{1}亩", (useTableArea ? actualLandCount : lands.Count), ToolMath.SetNumbericFormat(landAreaCount.ToString(), 2));
                scareainfostring = string.Format("合计:\n   {0}块\n{1}亩", (useTableArea ? tableLandCount : lands.Count), ToolMath.SetNumbericFormat(landtableCount.ToString(), 2));

                int landcountstringlength = lands.Count.ToString().Length;
                int htareastringlength = htareainfostring.Length - landcountstringlength - 10;

                int tableareastringlength = tableLandCount.ToString().Length;
                int uselandcount;
                if (useTableArea)
                {
                    uselandcount = tableareastringlength;
                }
                else
                {
                    uselandcount = landcountstringlength;
                }
                int scareastringlength = scareainfostring.Length - uselandcount -10;

                SetRangPublishExcelUse("D" + startrow, "D" + (index - 1), 42.25, htareainfostring, 7, landcountstringlength, 9 + landcountstringlength, htareastringlength);
                SetRangPublishExcelUse("C" + startrow, "C" + (index - 1), 42.25, scareainfostring, 7, uselandcount, 9 + uselandcount, scareastringlength);
                
            }

            lands.Clear();
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary> 
        private void WriteCurrentZoneInformation(ContractLand land, int index)
        {
            //string landNumber = ContractLand.GetLandNumber(land.LandNumber);
            //string landNumber = ContractLand.GetLandNumber(land.CadastralNumber);
            //if (landNumber.Length > YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian)
            //{
            //    landNumber = landNumber.Substring(YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian);
            //}
            InitalizeRangeValue("E" + index, "E" + index, land.Name.IsNullOrEmpty() ? "/" : land.Name);
            InitalizeRangeValue("F" + index, "F" + index, land.LandNumber.IsNullOrEmpty() ? "/" : land.LandNumber);
            InitalizeRangeValue("G" + index, "G" + index, land.NeighborEast != null ? land.NeighborEast : "/");
            InitalizeRangeValue("H" + index, "H" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
            InitalizeRangeValue("I" + index, "I" + index, land.NeighborWest != null ? land.NeighborWest : "/");
            InitalizeRangeValue("J" + index, "J" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
            InitalizeRangeValue("K" + index, "K" + index, land.AwareArea == 0 ? SystemDefine.InitalizeAreaString() : ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2));
            InitalizeRangeValue("L" + index, "L" + index, land.ActualArea == 0 ? SystemDefine.InitalizeAreaString() : ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2));
            InitalizeRangeValue("M" + index, "M" + index, land.LandExpand.PublicityComment);
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        private void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "M1").ToString();
            title = SystemDefine.GetTableHeaderStr(CurrentZone) + title;
            InitalizeRangeValue("A" + 1, "M" + 1, title);
            if (showContractee)
            {
                InitalizeRangeValue("A" + 2, "E" + 2, string.Format("发包方:{0}", contracteeName));
            }
            if (StartDate != null && StartDate.HasValue && EndDate != null && EndDate.HasValue)
            {
                int days = ((TimeSpan)(EndDate - StartDate)).Days+1;
                //int year = EndDate.Value.Year - StartDate.Value.Year;
                //int month = EndDate.Value.Month - StartDate.Value.Month;
                //int day = EndDate.Value.Day - StartDate.Value.Day + 1;
                string dateString = string.Format("公示日期:{0}  至  {1}", StartDate.Value.ToString("yyyy 年 MM 月 dd 日 "), EndDate.Value.ToString("yyyy 年 MM 月 dd 日 "));
                //year = year * 365;
                //month = month * 30;
                //day = year + month + day;
                dateString += "(共";
                dateString += days.ToString();
                dateString += "天)";
                InitalizeRangeValue("F" + 2, "M" + 2, dateString);
            }
            WriteCount();
            index++;
            string information = "制表人:";
            information += DrawPerson;
            information += "                    制表日期:";
            information += (DrawDate != null && DrawDate.HasValue) ? ToolDateTime.GetLongDateString(DrawDate.Value) : "";
            information += "                    审核人:";
            information += CheckPerson;
            information += "                    审核日期:";
            information += (CheckDate != null && CheckDate.HasValue) ? ToolDateTime.GetLongDateString(CheckDate.Value) : "";
            InitalizeRangeValue("A" + index, "M" + index, information);
            SetRangeAlignment("A" + index, "M" + index, 1, 2);
            //InitalizeRangeValue("B" + index, "C" + index, DrawPerson);
            //InitalizeRangeValue("D" + index, "D" + index, "制表日期");
            //InitalizeRangeValue("E" + index, "F" + index, (DrawDate != null && DrawDate.HasValue) ? ToolDateTime.GetLongDateString(DrawDate.Value) : "");
            //InitalizeRangeValue("G" + index, "G" + index, "审核人");
            //InitalizeRangeValue("H" + index, "I" + index, CheckPerson);
            //InitalizeRangeValue("J" + index, "J" + index, "审核日期");
            //InitalizeRangeValue("K" + index, "M" + index, (CheckDate != null && CheckDate.HasValue) ? ToolDateTime.GetLongDateString(CheckDate.Value) : "");
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 42.25, "合计");
            InitalizeRangeValue("B" + index, "B" + index, familyCount > 0 ? familyCount.ToString() : "");
            string areaString = string.Format("合计:\n   {0}块\n{1}亩", tableCount, ToolMath.SetNumbericFormat(tableArea.ToString(), 2));
            InitalizeRangeValue("C" + index, "C" + index, areaString);
            areaString = string.Format("合计:\n   {0}块\n{1}亩", landCount, ToolMath.SetNumbericFormat(actualArea.ToString(), 2));
            InitalizeRangeValue("D" + index, "D" + index, areaString);
            InitalizeRangeValue("E" + index, "E" + index, "\\");
            InitalizeRangeValue("F" + index, "F" + index, "\\");
            InitalizeRangeValue("G" + index, "G" + index, "\\");
            InitalizeRangeValue("H" + index, "H" + index, "\\");
            InitalizeRangeValue("I" + index, "I" + index, "\\");
            InitalizeRangeValue("J" + index, "J" + index, "\\");
            InitalizeRangeValue("K" + index, "K" + index, tableArea > 0 ? ToolMath.SetNumbericFormat(tableArea.ToString(), 2) : "\\");
            InitalizeRangeValue("L" + index, "L" + index, actualArea > 0 ? ToolMath.SetNumbericFormat(actualArea.ToString(), 2) : "\\");
            InitalizeRangeValue("M" + index, "M" + index, "\\");
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
