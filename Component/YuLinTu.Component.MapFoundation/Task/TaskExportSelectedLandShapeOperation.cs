using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskExportSelectedLandShapeOperation : Task
    {
        #region Fields

        private readonly IZoneWorkStation zoneStation;

        private bool returnValue = false;

        #endregion Fields

        #region Propertys

        public Zone CurrentZone { get; set; }

        public IDbContext DbContext { get; set; }

        public string Folder { get; set; }

        public ExportContractLandShapeDefine ExportContractLandShapeDefine = ExportContractLandShapeDefine.GetIntence();

        #endregion Propertys

        #region ctor

        public TaskExportSelectedLandShapeOperation(IDbContext dbContext)
        {
            DbContext = dbContext;
            zoneStation = dbContext.CreateZoneWorkStation();
        }

        #endregion ctor

        protected override void OnGo()
        {
            this.ReportProgress(0, "开始");
            var metadata = Argument as TaskExportSelectedLandShapeArgument;
            Folder = metadata.SaveFilePath;
            CurrentZone = metadata.CurrentZone;
            ExportLandShapeBusiness business = new ExportLandShapeBusiness();
            ExportZoneShape(CurrentZone, Folder);
            if (returnValue)
            {
                CanOpenResult = true;
            }
            this.ReportProgress(100, "完成");
        }

        public void ExportZoneShape(Zone zone, string fileName)
        {
            try
            {
                TaskExportSelectedLandShapeArgument metadata = Argument as TaskExportSelectedLandShapeArgument;
                AccountLandBusiness ContractAccountBusiness = new AccountLandBusiness(DbContext);
                string markDes = GetZoneName(zone);
                List<ContractLand> listGeoLand = metadata.Lands;
                if (listGeoLand == null || listGeoLand.Count == 0)
                {
                    this.ReportProgress(0, null);
                    this.ReportProgress(100, null);
                    this.ReportWarn("未获取到地块空间数据!");
                }

                using (ExportLandGeoToShape export = new ExportLandGeoToShape(ExportContractLandShapeDefine))
                {
                    fileName = Path.Combine(fileName, markDes + "地块.shp");
                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    export.FileName = fileName;
                    export.CurrentZone = zone;
                    export.ListVp = metadata.VPS;
                    export.ListGeoLand = listGeoLand;
                    export.DictList = metadata.DictList;
                    export.Lang = eLanguage.CN;
                    export.ProgressChanged += ReportPercent;
                    export.Alert += ReportInfo;
                    export.ExportToShape();
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导出地域图斑失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地块图斑)", ex.Message + ex.StackTrace);
            }
        }

        public string GetZoneName(Zone zone)
        {
            string name = zone.Name;
            try
            {
                Zone tempZone = zoneStation.Get(zone.UpLevelCode);
                while (tempZone.Level <= eZoneLevel.Village)
                {
                    name = tempZone.Name + name;
                    tempZone = zoneStation.Get(tempZone.UpLevelCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetZoneName(获取地域名称)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域名称出错," + ex.Message);
            }
            return name;
        }

        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(Folder);
            base.OpenResult();
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
    }
}