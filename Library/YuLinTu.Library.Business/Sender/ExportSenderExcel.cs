/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出发包方调查表
    /// </summary>
    [Serializable]
    public class ExportSenderSurveyTable : ExportExcelBase
    {
        #region Fields

        private int index;//下标

        private string templatePath;

        private ToolProgress toolProgress;

        #endregion

        #region Propertys

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool ShowValue { get; set; }

        /// <summary>
        /// 保存文件名
        /// </summary>
        public string SaveFileName { get; set; }

        /// <summary>
        /// 集体经济组织集合
        /// </summary>
        public List<CollectivityTissue> TissueCollection { get; set; }
        public string UnitName { get; set; }

        #endregion

        #region Ctor

        public ExportSenderSurveyTable()
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
        public void BeginToZone(string templatePath)
        {
            RePostProgress(1);
            if (TissueCollection == null || TissueCollection.Count == 0)
            {
                return;
            }
            if (!File.Exists(templatePath))
            {
                PostErrorInfo("模板路径不存在！");
                return;
            }
            this.templatePath = templatePath;
            Write();//写数据
        }

        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                RePostProgress(5);
                OpenExcelFile();
                RePostProgress(15);
                if (!SetValue())
                    return;
                BeginWrite();
                WriteCountInformation();
                RePostProgress(100);
                //if (ShowValue)
                //{                   
                //    PrintView(SaveFileName);
                //}
                //else
                //{
                    SaveAs(SaveFileName);
                //}
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose(); ;
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
            RePostProgress(5);
            index = 3;
            return true;
        }

        #endregion

        #region 开始生成Excel

        private bool BeginWrite()
        {
            //if (TissueCollection.Count > 31)
            //{
            //    int rowCount = TissueCollection.Count - 31;
            //    for (int i = 0; i < rowCount; i++)
            //    {
            //        InsertRowCell(8);
            //    }
            //}
            string titleName = UnitName + "农村土地承包经营权发包方调查表";
            SetRange("A1", "P1", titleName);
            var order = TissueCollection.OrderBy(te =>
            {
                int num = 0;
                string code = te.Code.Length > Zone.ZONE_VILLAGE_LENGTH ? te.Code.Substring(Zone.ZONE_VILLAGE_LENGTH) : te.Code.Substring(Zone.ZONE_TOWN_LENGTH);
                Int32.TryParse(code, out num);
                return num;
            });
            order = order.OrderBy(te => te.ZoneCode);
            foreach (CollectivityTissue tissue in order)
            {
                string tissueCode = tissue.Code;
                if (tissueCode.Length > 14)
                {
                    //tissueCode = tissue.Code.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + tissue.Code.Substring(Zone.ZONE_VILLAGE_LENGTH + 2, 2);
                    tissueCode = tissue.Code.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + tissue.Code.Substring(Zone.ZONE_VILLAGE_LENGTH, 2);
                }
                SetRange("A" + index, "A" + index, tissueCode);
                SetRange("B" + index, "B" + index, tissue.Name);
                SetRange("C" + index, "C" + index, tissue.LawyerName);
                SetRange("D" + index, "D" + index, tissue.LawyerTelephone);
                SetRange("E" + index, "E" + index, tissue.LawyerAddress);
                SetRange("F" + index, "F" + index, tissue.LawyerPosterNumber);
                if (tissue.LawyerCredentType != 0 && !ToolMath.MatchEntiretyNumber(tissue.LawyerCredentType.ToString()))
                {
                    SetRange("G" + index, "G" + index, EnumNameAttribute.GetDescription(tissue.LawyerCredentType));
                }
                SetRange("H" + index, "H" + index, tissue.LawyerCartNumber);
                SetRange("I" + index, "I" + index, tissue.SurveyPerson);
                SetRange("J" + index, "J" + index, (tissue.SurveyDate != null && tissue.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(tissue.SurveyDate.Value) : "");
                SetRange("K" + index, "K" + index, tissue.SurveyChronicle);
                SetRange("L" + index, "L" + index, tissue.CheckPerson);
                SetRange("M" + index, "M" + index, (tissue.CheckDate != null && tissue.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(tissue.CheckDate.Value) : "");
                SetRange("N" + index, "N" + index, tissue.CheckOpinion);
                SetRange("O" + index, "O" + index, tissue.Comment);
                SetRange("P" + index, "P" + index, tissue.SocialCode);
                index++;
            }
            //for (int row = index; row < 34; row++)
            //{
            //    SetRange("G" + row, "G" + row, "");
            //}
            return true;
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCountInformation()
        {
            SetRange("A" + index, "A" + index, "合计");
            SetRange("B" + index, "B" + index, "\\");
            SetRange("C" + index, "C" + index, "\\");
            SetRange("D" + index, "D" + index, "\\");
            SetRange("E" + index, "E" + index, "\\");
            SetRange("F" + index, "F" + index, "\\");
            SetRange("G" + index, "G" + index, "\\");
            SetRange("H" + index, "H" + index, "\\");
            SetRange("I" + index, "I" + index, "\\");
            SetRange("J" + index, "J" + index, "\\");
            SetRange("K" + index, "K" + index, "\\");
            SetRange("L" + index, "L" + index, "\\");
            SetRange("M" + index, "M" + index, "\\");
            SetRange("N" + index, "N" + index, "\\");
            SetRange("O" + index, "O" + index, "\\");
            SetRange("P" + index, "P" + index, "\\");
            SetLineType("A1", "P" + index, true);
            //index++;
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
