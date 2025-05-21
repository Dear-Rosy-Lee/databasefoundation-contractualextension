/*
* (C) 2025  鱼鳞图公司版权所有,保留所有权利 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using YuLinTu.Library.Entity;
using YuLinTu.Business.ContractLand.Exchange2;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据处理
    /// </summary>
    public class ExportZonePackage : Task
    {
        #region Fields

        private ToolProgress toolProgress;

        #endregion

        #region Propertys

        /// <summary>
        /// 文件目录
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperateType { get; set; }

        /// <summary>
        /// 是14位编码/否16位编码
        /// </summary>
        public bool IsStandCode { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        #endregion

        #region Ctor

        public ExportZonePackage()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出数据压缩包
        /// </summary>
        public void ExportPackage(List<Zone> zones, string zoneName)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            this.ReportProgress(1, "完成");
            ExZoneCollection zoneCollection = new ExZoneCollection();
            bool showPersent = zones.Count != 0 && zones.Count > 100 ? false : true;
            string desctiption = !showPersent ? "(" + zones.Count.ToString() + ")" : "";
            try
            {
                toolProgress.InitializationPercent(zones.Count, 99, 1);
                foreach (Zone zone in zones)
                {
                    YuLinTu.Business.ContractLand.Exchange2.ExZone exZone = new YuLinTu.Business.ContractLand.Exchange2.ExZone();
                    AgriLandZoneMapping(exZone, zone);
                    zoneCollection.Add(exZone);
                    toolProgress.DynamicProgress(ZoneDesc);
                }
                ToolSerialization.SerializeBinary(FileName, zoneCollection);
                AgriFileProgresss(FileName, zoneName);
                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("{0}成功导出压缩包", ZoneDesc));
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                YuLinTu.Library.Log.Log.WriteException(this, "AgriFileProgresss", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 文件处理
        /// </summary>
        private string AgriFileProgresss(string fileName, string zoneName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            Environment.CurrentDirectory = Path.GetDirectoryName(fileName);
            ArrayList fileList = new ArrayList();
            fileList.Add(Path.GetFileName(fileName));
            try
            {
                ZipOperation.Compress(Path.GetDirectoryName(fileName), zoneName + "(行政区域)", fileList, "");
                return Path.GetDirectoryName(fileName) + @"\" + zoneName + "(行政区域).zip";
            }
            catch (SystemException ex)
            {
                SharpZipOperation.CompressContractLand(Path.GetDirectoryName(fileName), zoneName + "(行政区域)", fileList, "");
                this.ReportError("导出" + zoneName + "压缩包失败");
                YuLinTu.Library.Log.Log.WriteException(this, "AgriFileProgresss", ex.Message + ex.StackTrace);
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                    File.Delete(fileName);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 地域信息映射
        /// </summary>
        private void AgriLandZoneMapping(YuLinTu.Business.ContractLand.Exchange2.ExZone exZone, Zone zone)
        {
            string zoneCode = zone.FullCode;
            string code = zone.Code;
            if (zone.Level == eZoneLevel.Group && !IsStandCode && zoneCode.Length == 14)
            {
                code = "00" + code;
                zoneCode = zoneCode.Substring(0, 12) + "00" + zoneCode.Substring(12, 2);
            }
            exZone.ID = zone.ID;
            exZone.Code = zone.Code;
            exZone.Comment = zone.Comment;
            exZone.CreateTime = DateTime.Now;
            exZone.CreateUser = zone.CreateUser;
            exZone.FullCode = zoneCode;
            exZone.FullName = zone.FullName;
            exZone.Level = (int)zone.Level;
            exZone.ModifyTime = DateTime.Now;
            exZone.ModifyUser = zone.LastModifyUser;
            exZone.Name = zone.Name;
            exZone.ParentCode = zone.UpLevelCode;
            exZone.ParentName = zone.UpLevelName;
            if (zone.Shape == null)
            {
                return;
            }
            YuLinTu.Spatial.Geometry geometry = zone.Shape as YuLinTu.Spatial.Geometry;
            if (geometry == null)
            {
                return;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExGeometry exGeometry = new YuLinTu.Business.ContractLand.Exchange2.ExGeometry();
            exGeometry.WellKnownBinary = geometry.ToBytes();
            exGeometry.Srid = geometry.Srid;
            exZone.Shape = exGeometry;
        }

        /// <summary>
        /// 进度
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion

    }
}
