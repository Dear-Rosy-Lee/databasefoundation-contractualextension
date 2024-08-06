/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.StockRightShuShan.ExportTask
{
    /// <summary>
    /// 批量导出承包地块调查表任务类 支持整组导出需要后续扩展
    /// </summary>
    public class ExportWordGroupTask : TaskGroup
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        public string TempName { get; set; }

        public AgricultureWordBook Book { get; set; }

        public string FileName { get; set; }

        public ExportWordTask Task { get; set; }


        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            try
            {
                Clear();
                List<VirtualPerson> selfAndSubsPersons = new List<VirtualPerson>();
                var zoneStation = DbContext.CreateZoneWorkStation();
                var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                selfAndSubsPersons = personStation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                var selfAndSubsZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                var allZones = zoneStation.GetAllZones(CurrentZone);
                foreach (var zone in selfAndSubsZones)
                {
                    var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode && c.IsStockFarmer);
                    string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                    //ExportWordTask Task=new  ExportWordTask();
                    Task.DbContext = DbContext;
                    Task.CurrentZone = zone;
                    Task.FileName = FileName + @"\" + savePath;
                    Task.SelectedPersons = listPersons;
                    Task.Name = "批量" + Name;
                    Task.TempName = TempName;
                    Task.Book = Book;
                    Task.Description = zone.FullName;
                    Add(Task);
                }
                base.OnGo();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportPublishWordOperation(导出" + Name + "任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出" + Name + "出现异常!");
            }
        }


        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(FileName);
            base.OpenResult();
        }
    }
}
