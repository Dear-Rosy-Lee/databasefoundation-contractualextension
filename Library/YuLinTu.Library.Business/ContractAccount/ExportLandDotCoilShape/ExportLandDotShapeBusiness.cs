/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地域模块业务处理
    /// </summary>
    public class ExportLandDotShapeBusiness : Task
    {
        #region Fildes

        private IDbContext db;

        #endregion

        #region Properties

        /// <summary>
        /// 地域表操作接口
        /// </summary>
        public IZoneWorkStation Station { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext
        {
            get { return db; }
            set { db = value; }
        }



        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandDotShapeBusiness(IDbContext db)
        {
            this.db = db;
            this.Station = db == null ? null : db.CreateZoneWorkStation();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandDotShapeBusiness()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出地域界址点图斑
        /// </summary>
        public bool ExportZoneShape(Zone zone, string fileName)
        {
            bool reslut = true;
            try
            {
                TaskExportLandDotShapeArgument metadata = Argument as TaskExportLandDotShapeArgument;
                IBuildLandBoundaryAddressDotWorkStation jzdStation = DbContext.CreateBoundaryAddressDotWorkStation();
                string markDes = GetZoneName(zone);
                List<BuildLandBoundaryAddressDot> LandDotList = jzdStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);

                List<BuildLandBoundaryAddressDot> listGeoLandDot = LandDotList.FindAll(c => c.Shape != null);
                if (listGeoLandDot == null || listGeoLandDot.Count == 0)
                {
                    this.ReportWarn("未获取到地块界址点空间数据!");
                    this.ReportProgress(100);
                    return false;
                }
                var landStation = DbContext.CreateAgriculturalLandWorkstation();
                var landList = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                var dic = new Dictionary<Guid, string>();
                landList.ForEach(t =>
                {
                    if (!dic.ContainsKey(t.ID))
                        dic[t.ID] = t.LandNumber;
                });
                using (var export = new ExportLandDotGeoToShape())
                {
                    fileName = Path.Combine(fileName, markDes + "界址点.shp");
                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    export.ZoneDesc = markDes;
                    export.FileName = fileName;
                    export.CurrentZone = zone;
                    export.LandIdDictionary = dic;
                    export.ListGeoLandDot = listGeoLandDot;
                    export.DictList = metadata.DictList;
                    export.Lang = eLanguage.CN;
                    export.ProgressChanged += ReportPercent;
                    export.Alert += ReportInfo;
                    export.ExportToShape();
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导出地域界址点图斑失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地块界址点shape)", ex.Message + ex.StackTrace);
                reslut = false;
            }
            return reslut;
        }



        #region 辅助方法

        /// <summary>
        /// 获取地域名称
        /// </summary>
        public string GetZoneName(Zone zone)
        {
            string name = zone.Name;
            try
            {
                Zone tempZone = Station.Get(zone.UpLevelCode);
                while (tempZone.Level <= eZoneLevel.Village)
                {
                    name = tempZone.Name + name;
                    tempZone = Station.Get(tempZone.UpLevelCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetZoneName(获取地域名称)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域名称出错," + ex.Message);
            }
            return name;
        }

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        /// <param name="progress"></param>
        private void export_PostProgressEvent(int progress, object userData)
        {
            this.ReportProgress(progress, userData);
        }

        #endregion


        #endregion
    }
}
