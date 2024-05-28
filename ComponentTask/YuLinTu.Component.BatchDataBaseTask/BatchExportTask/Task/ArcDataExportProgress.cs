/*
 * (C) 2015 - 2016 鱼鳞图公司版权所有,保留所有权利
*/
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 导出数据库
    /// </summary>
    public class ArcDataExportProgress : Task
    {
        #region Fields
        private bool canExport;//是否可以导出
        private bool showInformation = false;//是否显示信息
        private IZoneWorkStation zoneStation;
        private IVirtualPersonWorkStation<LandVirtualPerson> VirtualPersonStation;//承包台账(承包方)Station

        #endregion

        #region Propertys
        /// <summary>
        /// 目标节点地域
        /// </summary>
        public YuLinTu.Library.Entity.Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据源上下文
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 目标文件夹
        /// </summary>
        public string Folder { get; set; }
        /// <summary>
        /// 包含界址点、界址线
        /// </summary>
        public bool ContainDotLine { get; set; }

        /// <summary>
        /// 重复点容差
        /// </summary>
        public double RepeatValue { get; set; }

        /// <summary>
        /// 是否检查
        /// </summary>
        public bool CanChecker { get; set; }
        #endregion

        #region Ctor

        public ArcDataExportProgress(IDbContext db)
        {
            this.Name = "批量导出数据库";
            DbContext = db;
            zoneStation = DbContext.CreateZoneWorkStation();
            VirtualPersonStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();

            RepeatValue = 0.05;
        }

        #endregion

        #region Override

        /// <summary>
        /// 导出数据成果
        /// </summary>
        public void Export()
        {
            this.ReportProgress(0, "开始");
            var metadata = Argument as TaskBuildExportResultDataBaseArgument;

            bool canContinue = InitalizeData(metadata);
            if (!canContinue)
            {
                return;
            }
            try
            {
                if (CurrentZone.Level > YuLinTu.Library.Entity.eZoneLevel.County)
                {
                    this.ReportError("选择行政区域高于区县级行政区域!");
                    return;
                }
                string dbName = System.Windows.Forms.Application.StartupPath + @"\Template\Data.sqlite";
                if (!System.IO.File.Exists(dbName))
                {
                    this.ReportError("系统中缺少数据库文件!");
                    return;
                }
                else
                {
                    System.IO.File.Copy(dbName, System.Windows.Forms.Application.StartupPath + @"\Data.sqlite", true);
                }
                ArcDataProgress();
            }
            catch (System.Exception ex)
            {
                if (ex.GetType().Name == "TaskStopException")
                    return;
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);

                this.ReportError("导出数据库出错，详细信息请查看日志!");
                this.ReportProgress(100, "完成");
                return;
            }
            this.ReportProgress(100, "完成");
        }

        #endregion

        #region 数据检查

        /// <summary>
        /// 初始化数据
        /// </summary>
        private bool InitalizeData(TaskBuildExportResultDataBaseArgument metadata)
        {
            if (metadata == null)
            {
                this.ReportError("选择行政区域无效!");
                return false;
            }

            if (CurrentZone == null)
            {
                this.ReportError("选择行政区域无效!");
                return false;
            }
            canExport = true;
            InitalizeDirectory();
            return true;

        }

        /// <summary>
        /// 数据处理
        /// </summary>
        private void ArcDataProgress()
        {
            var zones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (zones == null)
            {
                this.ReportError(string.Format("地域{0}下无地域数据", CurrentZone.FullName));
                return;
            }
            bool canContinue = CanContinue();
            if (!canContinue)
            {
                return;
            }
            ArcSpaceDataProgress spaceProgress = new ArcSpaceDataProgress(DbContext);
            HashSet<string> extendSet = new HashSet<string>();
            spaceProgress.Alert += (s, e) => { this.ReportAlert(e.Grade, e.UserState, e.Description); };
            spaceProgress.ProgressChanged += (s, e) => { this.ReportProgress(e.Percent, e.UserState); };
            spaceProgress.ShapeFilePath = Folder;
            spaceProgress.CurrentZone = CurrentZone;
            InitalizeSpatialCoordianteSystem(spaceProgress);//初始化空间坐标系
            SqliteManager sqliteManager = new SqliteManager();
            this.ReportProgress(1, string.Format("正在获取{0}数据", CurrentZone.FullName));
            if (!CanChecker || canExport)
            {
                ExportShape(spaceProgress, sqliteManager, zones);
            }
            else
            {
                ShowChecker(Folder);
            }
            zones = null;
            GC.Collect();
        }

        /// <summary>
        /// 导出shape文件
        /// </summary>
        private void ExportShape(ArcSpaceDataProgress spaceProgress,
            SqliteManager manager, List<YuLinTu.Library.Entity.Zone> zones)
        {
            try
            {
                this.ReportProgress(2, string.Format("正在处理矢量数据"));
                if (!ContainDotLine)
                {
                    spaceProgress.ExportSpaceDataBase(zones, manager, true);
                }
            }
            catch (Exception ex)
            {
                this.ReportProgress(100, "完成");
                this.ReportAlert(eMessageGrade.Exception, null, ex.Message + "详细信息请看日志");
            }
            GC.Collect();
        }

        /// <summary>
        /// 显示检查信息
        /// </summary>
        private void ShowChecker(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                System.IO.DirectoryInfo directory = Directory.GetParent(filePath);
                directory.Delete(true);
            }
            if (!showInformation)
            {
                string dataRecord = Application.StartupPath + @"\Error\DataChecker.txt";
                if (CanChecker)
                {
                    if (!canExport)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, "导出" + CurrentZone.FullName + "下数据存在问题,请在" + dataRecord + "中查看详细信息...");
                    }
                }
                if (System.IO.File.Exists(dataRecord))
                {
                    System.Diagnostics.Process.Start(dataRecord);
                }
            }
            this.ReportProgress(100, "完成");
        }

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        private List<string> InitalizeAllRelation()
        {
            var list = FamilyRelationShip.AllRelation();
            return list;
        }

        /// <summary>
        /// 初始化空间坐标系
        /// </summary>
        private void InitalizeSpatialCoordianteSystem(ArcSpaceDataProgress spaceProgress)
        {
            if (spaceProgress == null)
            {
                return;
            }
            var targetSpatialReference = DbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).Schema,
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).TableName);

            var info = targetSpatialReference.ToEsriString();
            spaceProgress.SpatialText = info;
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue()
        {
            bool canContinue = true;
            int spotscount = VirtualPersonStation.CountByZone(CurrentZone.FullCode);
            if (spotscount <= 0)
            {
                this.ReportAlert(eMessageGrade.Warn, null, "选择地域下不存在承包方与地块数据,无法执行导出操作!");
                canContinue = false;
            }
            string fileName = Application.StartupPath + @"\ShapeTemplate";
            if (!Directory.Exists(fileName))
            {
                this.ReportAlert(eMessageGrade.Error, null, "矢量数据文件模板不存在!");
                canContinue = false;
            }
            fileName = Application.StartupPath + @"\Template\Data.sqlite";
            if (!File.Exists(fileName))
            {
                this.ReportAlert(eMessageGrade.Error, null, "临时数据库文件模板不存在!");
                canContinue = false;
            }
            return canContinue;
        }
        #endregion

        #region 数据转换
        /// <summary>
        /// 点
        /// </summary>
        private class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Dist
            {
                get
                {
                    return Math.Sqrt(X * X + Y * Y);
                }
            }
            public Point(double x, double y)
            {
                this.X = Math.Round(x, 4);
                this.Y = Math.Round(y, 4);
            }

            public override bool Equals(object obj)
            {
                Point y = obj as Point;
                if (null == y)
                    return false;

                if (X == y.X && Y == y.Y)
                    return true;

                return Dist == y.Dist;
            }

            public override int GetHashCode()
            {
                return Dist.GetHashCode();
            }
        }

        /// <summary>
        /// 线
        /// </summary>
        private class Line
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public Line()
            {

            }
            public Line(Point start, Point end)
            {
                this.Start = start;
                this.End = end;
            }

            public override bool Equals(object obj)
            {
                var y = obj as Line;
                if (null == y)
                    return false;

                if (Start.Equals(y.Start) && End.Equals(y.End))
                    return true;

                return Start.Equals(y.End) && End.Equals(y.Start);
            }

            public override int GetHashCode()
            {
                return Start.GetHashCode() ^ End.GetHashCode();
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// 初始化错误记录文件目录
        /// </summary>
        private void InitalizeDirectory()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Error"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Error");
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataChecker.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                writer.WriteLine(System.DateTime.Now.ToString());
            }
        }

        #endregion
    }
}
