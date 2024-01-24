/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Business;
using YuLinTu.Library.Office;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 调查信息公示表
    /// </summary>
    [Serializable]
    public class ExportLandInfomationExcelTable : ExportExcelBase
    {
        #region Fields

        private Library.Business.ToolProgress toolProgress;//进度条
        private const int defaultStartIndex = 9;
        private int index;//下标
        private string templatePath;
        private string contracteeName;
        private bool showContractee;
        private int familyCount;//户数
        private int tableCount;//总地块数
        private int landCount;//总地块数
        private double tableArea;//合同面积
        private double awareArea;//实测面积
        private bool useTableArea;//使用台账面积
        private SystemSetDefine SystemSet = SystemSetDefine.GetIntence();
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
        /// 承包方家庭信息集合-承包地块的
        /// </summary>
        public List<ContractAccountLandFamily> AccountLandFamily { get; set; }


        /// <summary>
        /// 承包方家庭信息集合-非承包地块的
        /// </summary>
        public List<ContractAccountLandFamily> AccountLandFamilyOthers { get; set; }


        /// <summary>
        /// 台账常规设置
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine { get; set; }

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
            toolProgress = new Library.Business.ToolProgress();
            toolProgress.OnPostProgress += new Library.Business.ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
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
        public bool BeginToZone(string templatePath)
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

                OpenExcelFile();
                if (!SetValue())
                    return;
                BeginWrite();
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
        private void OpenExcelFile(int workbookSheetIndex = 0)
        {
            Open(templatePath, workbookSheetIndex);
        }

        /// <summary>
        /// 初始值
        /// </summary> 
        private bool SetValue()
        {
            index = defaultStartIndex;
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
            //2017.10.20修改，没有地的承包方仍然显示到调查信息公示表中
            //AccountLandFamily.RemoveAll(v => v.LandCollection.Count == 0);
            AccountLandFamily.Sort((a, b) =>
            {
                long aNumber = Convert.ToInt64(a.CurrentFamily.FamilyNumber);
                long bNumber = Convert.ToInt64(b.CurrentFamily.FamilyNumber);
                return aNumber.CompareTo(bNumber);
            });
            if (yes)
            {
                AccountLandFamily.RemoveAll(fm =>fm.CurrentFamily.Name.IndexOf("集体") >= 0);
            }
            showContractee = false;
            familyCount = AccountLandFamily.Count;
            toolProgress.InitializationPercent(AccountLandFamily.Count, 99, 1);
            int vpIndex = 1;
            var insertRows = AccountLandFamily.Sum(f => f.LandCollection.Count) +
                AccountLandFamily.Count(f => f.LandCollection.Count == 0) + 1;//要插入的行数，包括总合计行

            InsertRowCell(index - 1, insertRows);//插入行
            foreach (ContractAccountLandFamily landFamily in AccountLandFamily)
            {
                toolProgress.DynamicProgress(ZoneDesc + landFamily.CurrentFamily.Name);
                WriteInformation(landFamily, vpIndex++);
            }
            WriteTempLate();
            SetLineType("A9", "M" + (index - 1), false);
            this.Information = string.Format("{0}导出{1}条承包台账数据!", ZoneDesc, AccountLandFamily.Count);
            AccountLandFamily = null;
            return true;
        }

        /// <summary>
        /// 书写每个承包户信息
        /// </summary>
        /// <param name="virtualPerson"></param>
        private void WriteInformation(ContractAccountLandFamily landFamily, int vpIndex)
        {
            int startrow = index;
            List<ContractLand> lands = landFamily.LandCollection;

            if (landFamily == null || lands == null || lands.Count == 0)
            {
                var vpname = landFamily.CurrentFamily.FamilyExpand != null ? landFamily.CurrentFamily.FamilyExpand.ExtendName : "";
                vpname = vpname.IsNullOrEmpty() ? "" : vpname;
                //2017.10.20修改，序号显示4位家庭编号
                InitalizeRangeValue("A" + startrow, "A" + index, landFamily.CurrentFamily.FamilyNumber?.PadLeft(4, '0'));
                InitalizeRangeValue("B" + startrow, "B" + index, vpname.InitalizeFamilyName(SystemSet.KeepRepeatFlag) + "\n"+ landFamily.CurrentFamily.Name.InitalizeFamilyName(SystemSet.KeepRepeatFlag));
                index++;
                return;
            }
            string landTypeName = string.Empty;
            // 实测面积
            double landAreaCount = 0;
            // 二轮承包合同面积
            double landtableCount = 0;
            int tableLandCount = 0;

            //contracteeName = SystemDefine.CountryTableHead ? (string.IsNullOrEmpty(SenderNameVillage) ? "" : SenderNameVillage) : (string.IsNullOrEmpty(SenderName) ? "" : SenderName);
            contracteeName = string.IsNullOrEmpty(SenderName) ? "" : SenderName;
            showContractee = !string.IsNullOrEmpty(contracteeName);

            foreach (ContractLand land in lands)
            {
                //2017.10.20修改，序号显示4位家庭编号
                InitalizeRangeValue("A" + startrow, "A" + index, landFamily.CurrentFamily.FamilyNumber?.PadLeft(4, '0'));
                string str = landFamily.CurrentFamily.FamilyExpand.ExtendName.InitalizeFamilyName(SystemSet.KeepRepeatFlag);
                str += "\n";
                str += landFamily.CurrentFamily.Name.InitalizeFamilyName(SystemSet.KeepRepeatFlag);
                InitalizeRangeValue("B" + startrow, "B" + index, str);
                landAreaCount += land.ActualArea;
                landtableCount += (land.TableArea != null && land.TableArea.HasValue && land.TableArea.Value > 0) ? land.TableArea.Value : 0;
                WriteCurrentZoneInformation(land, index);
                tableLandCount += (land.TableArea != null && land.TableArea.HasValue && land.TableArea.Value > 0) ? 1 : 0;
                index++;
            }
            landCount += lands.Count;
            tableCount += useTableArea ? tableLandCount : lands.Count;

            tableArea += landtableCount;
            awareArea += landAreaCount;

            var landAreaCountstr = landAreaCount == 0 ? "/" : Library.Business.ToolMath.SetNumbericFormat(landAreaCount.ToString(), 2);
            var landtableCountstr = landtableCount == 0 ? "/" : Library.Business.ToolMath.SetNumbericFormat(landAreaCount.ToString(), 2);

            if (lands.Count > 1)
            {
                InitalizeRangeValue("D" + startrow, "D" + (index - 1), string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", lands.Count, landAreaCountstr, lands.Count, landAreaCountstr)); // 实测总面积
                InitalizeRangeValue("C" + startrow, "C" + (index - 1), string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", (useTableArea ? tableLandCount : lands.Count), landtableCountstr, (useTableArea ? tableLandCount : lands.Count), landtableCountstr)); // 二轮承包合同面积
            }
            else
            {
                SetRange("D" + startrow, "D" + (index - 1), 42.25, string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", lands.Count, landAreaCountstr, lands.Count, landAreaCountstr)); // 实测总面积
                SetRange("C" + startrow, "C" + (index - 1), 42.25, string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", (useTableArea ? tableLandCount : lands.Count), landtableCountstr, (useTableArea ? tableLandCount : lands.Count), landtableCountstr)); // 二轮承包合同面积
            }
            lands.Clear();
        }

        /// <summary>
        /// 书写当前地块信息
        /// </summary> 
        private void WriteCurrentZoneInformation(ContractLand land, int index)
        {
            InitalizeRangeValue("E" + index, "E" + index, land.Name);
            InitalizeRangeValue("F" + index, "F" + index, land.LandNumber);
            InitalizeRangeValue("G" + index, "G" + index, land.NeighborEast != null ? land.NeighborEast : "");
            InitalizeRangeValue("H" + index, "H" + index, land.NeighborSouth != null ? land.NeighborSouth : "");
            InitalizeRangeValue("I" + index, "I" + index, land.NeighborWest != null ? land.NeighborWest : "");
            InitalizeRangeValue("J" + index, "J" + index, land.NeighborNorth != null ? land.NeighborNorth : "");
            InitalizeRangeValue("K" + index, "K" + index, land.TableArea.HasValue && land.TableArea.Value != 0 ? land.TableArea.Value.AreaFormat(2) : "/");//二轮合同面积
            InitalizeRangeValue("L" + index, "L" + index, land.ActualArea == 0 ? "/" : land.ActualArea.AreaFormat(2));   // 实测面积
            InitalizeRangeValue("M" + index, "M" + index, land.Comment != null ? land.Comment : "");
            SetRangeHeight(index, index, 30);
        }

        /// <summary>
        /// 填写模板
        /// </summary>
        private void WriteTempLate()
        {
            if (showContractee)
            {
                InitalizeRangeValue("A" + 4, "E" + 4, string.Format("发包方:{0}", contracteeName));
            }
            if (StartDate != null && StartDate.HasValue && EndDate != null && EndDate.HasValue)
            {
                int year = EndDate.Value.Year - StartDate.Value.Year;
                int month = EndDate.Value.Month - StartDate.Value.Month;
                int day = EndDate.Value.Day - StartDate.Value.Day + 1;
                string dateString = string.Format("公示日期:{0}  至  {1}", StartDate.Value.ToString("yyyy 年 MM 月 dd 日 "), EndDate.Value.ToString("yyyy 年 MM 月 dd 日 "));
                year = year * 365;
                month = month * 30;
                day = year + month + day;
                dateString += "(共";
                dateString += day.ToString();
                dateString += "天)";
                InitalizeRangeValue("F" + 4, "M" + 4, dateString);
            }
            WriteCount();
            int tempIndex = index + 2;
            string info = "制表人:  ";
            if (!String.IsNullOrEmpty(DrawPerson))
            {
                InitalizeRangeValue("A" + tempIndex, "C" + tempIndex, info + DrawPerson);
            }
            info = "制表日期:  ";
            if ((DrawDate != null && DrawDate.HasValue))
                InitalizeRangeValue("D" + tempIndex, "F" + tempIndex, info + Library.Business.ToolDateTime.GetLongDateString(DrawDate.Value));
            info = "审核人:  ";
            if (!String.IsNullOrEmpty(CheckPerson))
                InitalizeRangeValue("H" + tempIndex, "J" + tempIndex, info + CheckPerson);
            info = "审核日期:  ";
            if ((CheckDate != null && CheckDate.HasValue))
                InitalizeRangeValue("K" + tempIndex, "M" + tempIndex, info + Library.Business.ToolDateTime.GetLongDateString(CheckDate.Value));
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 42.25, "合计");
            InitalizeRangeValue("B" + index, "B" + index, familyCount > 0 ? familyCount.ToString() : "");
            string areaString = string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", tableCount, Library.Business.ToolMath.SetNumbericFormat(tableArea.ToString(), 2), tableCount, Library.Business.ToolMath.SetNumbericFormat(tableArea.ToString(), 2));
            InitalizeRangeValue("C" + index, "C" + index, areaString);
            areaString = string.Format("བསྡོམས་རྩིས་\n   {0}དུམ་\n{1}མུའུ་\n合计:\n   {2}块\n{3}亩", landCount, Library.Business.ToolMath.SetNumbericFormat(awareArea.ToString(), 2), landCount, Library.Business.ToolMath.SetNumbericFormat(awareArea.ToString(), 2));
            InitalizeRangeValue("D" + index, "D" + index, areaString);
            InitalizeRangeValue("E" + index, "E" + index, "\\");
            InitalizeRangeValue("F" + index, "F" + index, "\\");
            InitalizeRangeValue("G" + index, "G" + index, "\\");
            InitalizeRangeValue("H" + index, "H" + index, "\\");
            InitalizeRangeValue("I" + index, "I" + index, "\\");
            InitalizeRangeValue("J" + index, "J" + index, "\\");
            InitalizeRangeValue("K" + index, "K" + index, tableArea > 0 ? Library.Business.ToolMath.SetNumbericFormat(tableArea.ToString(), 2) : "\\");
            InitalizeRangeValue("L" + index, "L" + index, awareArea > 0 ? Library.Business.ToolMath.SetNumbericFormat(awareArea.ToString(), 2) : "\\");
            InitalizeRangeValue("M" + index, "M" + index, "\\");
            SetLineType("A" + index, "M" + index, false);
        }
        #endregion

        #endregion
    }
}
