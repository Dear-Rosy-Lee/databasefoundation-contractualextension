/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Library.WorkStation;
using static YuLinTu.tGISCNet.AreaLineOverlapCheck;
using Aspose.Cells;
using System.Drawing;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 南宁专用-调查信息公示表导出
    /// </summary>
    public class ExportLandInfomationExcelTableNanning : ExportExcelBase
    {
        #region Fields

        protected ToolProgress toolProgress;//进度条
        protected int index;//下标
        protected int cindex;
        protected string templatePath;
        protected string contracteeName;
        protected bool showContractee;
        protected int familyCount;//户数
        protected int tableCount;//总地块数
        protected int landCount;//总地块数
        protected double tableArea;//合同面积
        protected double actualArea;//实测面积
        protected bool useTableArea;//使用台账面积
        protected int peopleCount;//家人数
        protected double AwareArea;//颁证面积
        protected double TableArea = 0;//合同面积
        protected double ActualArea;//实测面积

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


        public ExportLandInfomationExcelTableNanning()
        {
            // 可根据需要指定南宁专用模板名
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            
            base.TemplateName = "调查信息公示表(南宁)";
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }
        #region 开始生成前的操作


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


        #region 生成excel逻辑
        private bool BeginWrite()
        {
            if (CurrentZone == null)
            {
                return false;
            }
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
                WriteInformationNanning(landFamily);
            }
            WriteTempLate();
            SetLineType("A5", "P" + (index - 1), false);
            this.Information = string.Format("{0}导出{1}条承包台账数据!", ZoneDesc, AccountLandFamily.Count);
            AccountLandFamily = null;
            return true;
        }
        /// <summary>
        /// 书写每个承包户信息
        /// </summary>
        /// <param name="virtualPerson"></param>
        private void WriteInformationNanning(ContractAccountLandFamily landFamily)
        {
            if (landFamily == null)
                return;

            int height = (landFamily.LandCollection.Count > landFamily.Persons.Count) ? landFamily.LandCollection.Count : landFamily.Persons.Count;
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
                WritePersonInformationNanning(person, bindex, flag);
                SetRowHeight(bindex, 27.75);
                bindex++;
            }

            foreach (ContractLand land in lands)
            {
                double tableArea = land.TableArea ?? 0;
                TotalLandTable += tableArea;
                TotalLandAware += land.AwareArea;
                TotalLandActual += land.ActualArea;
                WriteCurrentZoneInformation(land, aindex);
                SetRowHeight(aindex, 27.75);
                aindex++;
            }


            landCount += lands.Count;
            if (lands.Count == 0)
            {
                //index++;
            }
            AwareArea += TotalLandAware;
            ActualArea += TotalLandActual;
            TableArea += TotalLandTable;

            string result = landFamily.CurrentFamily.FamilyNumber.PadLeft(4, '0');
            InitalizeRangeValue("A" + index, "A" + (index + height - 1), cindex);
            InitalizeRangeValue("B" + index, "B" + (index + height - 1), landFamily.CurrentFamily.Name);
            //InitalizeRangeValue("C" + index, "C" + (index + height - 1), result);
            //var tel = landFamily.CurrentFamily.Telephone;
            //InitalizeRangeValue("D" + index, "D" + (index + height - 1), tel);
            //if (tel.IsNotNullOrEmpty() && tel.Length >= 11)
            //    InitalizeRangeValue("D" + index, "D" + (index + height - 1), $"{tel.Substring(0, 3)}****{tel.Substring(tel.Length - 4)}");
            //InitalizeRangeValue("E" + index, "E" + (index + height - 1), landFamily.Persons.Count);
            index += height;
            //workbook.Worksheets[0].HorizontalPageBreaks.Add("A" + index);
            //workbook.Worksheets[0].VerticalPageBreaks.Add("A" + index);
            lands.Clear();
        }

        #endregion
        private void WritePersonInformationNanning(Person person, int index, bool flag)
        {

            InitalizeRangeValue("M" + index, "M" + index, person.Name.IsNullOrEmpty() ? "/" : person.Name);
            InitalizeRangeValue("N" + index, "N" + index, person.ICN);
            InitalizeRangeValue("O" + index, "O" + index, person.Relationship);
            InitalizeRangeValue("P" + index, "P" + index, person.Comment);
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary> 
        private void WriteCurrentZoneInformation(ContractLand land, int index)
        {
            InitalizeRangeValue("D" + index, "D" + index, land.ActualArea == 0 ? SystemDefine.InitalizeAreaString() : ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2));
            InitalizeRangeValue("E" + index, "E" + index, land.Name.IsNullOrEmpty() ? "/" : land.Name);
            InitalizeRangeValue("F" + index, "F" + index, land.LandNumber.IsNullOrEmpty() ? "/" : land.LandNumber.Substring(land.LandNumber.Length - 5));
            InitalizeRangeValue("G" + index, "G" + index, land.NeighborEast != null ? land.NeighborEast : "/");
            InitalizeRangeValue("H" + index, "H" + index, land.NeighborSouth != null ? land.NeighborSouth : "/");
            InitalizeRangeValue("I" + index, "I" + index, land.NeighborWest != null ? land.NeighborWest : "/");
            InitalizeRangeValue("J" + index, "J" + index, land.NeighborNorth != null ? land.NeighborNorth : "/");
            InitalizeRangeValue("K" + index, "K" + index, land.ActualArea == 0 ? SystemDefine.InitalizeAreaString() : ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2));
            InitalizeRangeValue("L" + index, "L" + index, land.Comment);
        }
        /// <summary>
        /// 填写模板
        /// </summary>
        private void WriteTempLate()
        {
            string title = GetRangeToValue("A1", "Q1").ToString();
            //title = SystemDefine.GetTableHeaderStr(CurrentZone) + title;
            InitalizeRangeValue("A" + 1, "P" + 1, title);
            InitalizeRangeValue("A" + 2, "E" + 2, string.Format("发包方名称:{0}", SenderName));
            //InitalizeRangeValue("G" + 2, "I" + 2, string.Format("发包方编码:{0}", Tissue.Code));
            //InitalizeRangeValue("J" + 2, "M" + 2, string.Format("发包方负责人姓名:{0}", Tissue.LawyerName));
            //if (StartDate != null && StartDate.HasValue && EndDate != null && EndDate.HasValue)
            //{
            //    int days = ((TimeSpan)(EndDate - StartDate)).Days + 1;
            //    //int year = EndDate.Value.Year - StartDate.Value.Year;
            //    //int month = EndDate.Value.Month - StartDate.Value.Month;
            //    //int day = EndDate.Value.Day - StartDate.Value.Day + 1;
            //    string dateString = string.Format("{0} - {1}", StartDate.Value.ToString("yyyy年M月dd日"), EndDate.Value.ToString("yyyy年M月dd日"));
            //    //year = year * 365;
            //    //month = month * 30;
            //    //day = year + month + day;

            //    InitalizeRangeValue("N" + 2, "R" + 2, string.Format("日期：{0}", dateString));
            //}
            WriteCount();
            index++;

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
            InitalizeRangeValue("B" + index, "C" + index, familyCount > 0 ? familyCount.ToString() : "");
            InitalizeRangeValue("D" + index, "D" + index, AwareArea > 0 ? ToolMath.SetNumbericFormat(AwareArea.ToString(), 2) : "\\");
            
            InitalizeRangeValue("E" + index, "F" + index, landCount);
            InitalizeRangeValue("G" + index, "G" + index, "\\");
            InitalizeRangeValue("H" + index, "H" + index, "\\");
            InitalizeRangeValue("I" + index, "I" + index, "\\");
            InitalizeRangeValue("J" + index, "J" + index, "\\");
            
            InitalizeRangeValue("K" + index, "K" + index, ActualArea > 0 ? ToolMath.SetNumbericFormat(ActualArea.ToString(), 2) : "\\");
            InitalizeRangeValue("L" + index, "L" + index, "\\");
            InitalizeRangeValue("M" + index, "P" + index, peopleCount);
        }
    }
}
