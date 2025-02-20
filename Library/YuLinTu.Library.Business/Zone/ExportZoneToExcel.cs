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
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出行政区域调查表
    /// </summary>
    [Serializable]
    public class ExportZoneToExcel : ExportExcelBase
    {
        #region Fields

        private Zone currentZone;
        private ToolProgress toolProgress;

        #endregion

        #region Property

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 是14位编码/否16位编码
        /// </summary>
        public bool IsStandCode { get; set; }

        /// <summary>
        /// 保存目录
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 行政地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        #endregion

        #region Ctor

        public ExportZoneToExcel()
        {
            SaveFilePath = string.Empty;
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

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion

        #region Methods

        #region 开始生成Excel操作

        /// <summary>
        /// 开始导出数据
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="zone">地域</param>
        public void BeginExcel()
        {
            PostProgress(1, "开始");
            Write();//写数据
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                PostProgress(5, "正在获取地域数据");
                Open("");
                WriteZoneInformation();
                PostProgress(100, "完成");
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }

        #endregion

        #region 开始往Excel中添加值

        /// <summary>
        /// 写地域信息到Excel表中
        /// </summary>
        private void WriteZoneInformation()
        {
            if (ZoneList == null || ZoneList.Count == 0)
            {
                return;
            }
            ZoneList.RemoveAll(t => t.Level == eZoneLevel.State);
            var orders = ZoneList.OrderBy(ze => ze.FullCode);
            SetRangeWidthAndHeight("A1", "A1", "地域编码", 20, 20);
            SetRangeWidthAndHeight("B1", "B1", "地域名称", 20, 20);
            int i = 1;
            toolProgress.InitializationPercent(ZoneList.Count, 95, 5);
            foreach (Zone zone in orders)
            {
                if (zone.Level == eZoneLevel.State)
                {
                    continue;
                }
                i++;
                if (!IsStandCode && zone.Level == eZoneLevel.Group && zone.FullCode.Length == 14)
                {
                    zone.FullCode = zone.FullCode.Substring(0, 12) + "00" + zone.FullCode.Substring(12, 2);
                }
                SetRange("A" + i.ToString(), "A" + i.ToString(), zone.FullCode, 13, false);
                SetRange("B" + i.ToString(), "B" + i.ToString(), zone.Name, 13, false);
                toolProgress.DynamicProgress(ZoneDesc);
            }
            i++;
            SetRange("A" + i.ToString(), "A" + i.ToString(), "合计", 13, false);
            SetRange("B" + i.ToString(), "B" + i.ToString(), ZoneList.Count.ToString(), 13, false);
            SetLineType("B" + i.ToString());
            SetRangeAlignment("A2", "A" + i.ToString(), 1, 2);
        }

        #endregion

        #endregion
    }
}
