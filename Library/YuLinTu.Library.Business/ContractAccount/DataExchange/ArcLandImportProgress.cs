/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using YuLinTu.Data;
using YuLinTu.Business.ContractLand.Exchange2;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入土地压缩包
    /// </summary>
    public partial class ArcLandImportProgress : Task
    {
        #region Fields

        private IDbContext database;//属性库
        //private int currentIndex;//进度值
        private Zone currentZone;//当前地域
        //private AgriLandEntity landEntity;//农用地实体

        private DownloadSettingDefine DownloadSettingDefine = DownloadSettingDefine.GetIntence();

        #endregion

        #region Properties

        /// <summary>
        /// 土地类型
        /// </summary>
        // public LandOperatorEventArgs LandOperator { get; set; }

        public LanderType LanderType { get; set; }
        #endregion

        #region Delegate

        //public event EventHandler ImportCompleted;

        #endregion

        #region Ctor

        public ArcLandImportProgress()
        {

        }

        #endregion

        #region Override

        protected override void OnGo()
        {
            ArcLandImporArgument metadata = Argument as ArcLandImporArgument;
            bool canContinue = InitalizeData(metadata);
            if (!canContinue)
            {
                return;
            }
            try
            {
                bool success = ImportAgricultureLandData(metadata);
                if (success)
                    this.ReportProgress(100, "完成");
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "导入压缩包失败!", ex.Message + ex.StackTrace);
                this.ReportException(ex, "数据处理失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private bool InitalizeData(ArcLandImporArgument argument)
        {
            if (argument == null || (argument.OpratorName != "DownLoad" && argument.FileName == null))
            {
                this.ReportException(null, "参数错误!");
                return false;
            }
            currentZone = argument.CurrentZone;
            database = argument.Database;
            if (database == null)
            {
                this.ReportException(null, "数据库参数错误!");
                return false;
            }
            //currentIndex = 1;
            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 检查导入
        /// </summary>
        /// <returns></returns>
        private bool CheckImportAgrilatureDataValide(YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection zones)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExZone zone = zones.Find(ze => ze.FullCode == currentZone.FullCode);
            return zone != null;
        }

        /// <summary>
        /// 检查导入数据操作方式
        /// </summary>
        private bool CheckImportAgrilatureDataOperation()
        {
            return database.CreateContractLandWorkstation().Any(t => t.ZoneCode == currentZone.FullCode);
        }

        /// <summary>
        /// 解压压缩包
        /// </summary>
        /// <param name="filePath">压缩包路径</param>
        /// <returns></returns>
        private string ExtractFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }
            if (LanderType == LanderType.AgricultureLand)
            {
                return YuLinTu.Business.ContractLand.Exchange2.ZipOperation.ExtractZip(filePath, string.Empty);
            }
            return ZipOperation.ExtractZip(filePath, string.Empty);
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="xmlFileName"></param>
        private static void DeleteExtractFile(string xmlFileName)
        {
            if (!System.IO.File.Exists(xmlFileName))
            {
                return;
            }
            string directory = System.IO.Path.GetDirectoryName(xmlFileName);
            if (string.IsNullOrEmpty(directory) || !System.IO.Directory.Exists(directory))
            {
                return;
            }
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(xmlFileName);
                fi.Attributes = System.IO.FileAttributes.Normal;
                fi.Delete();
                System.IO.Directory.Delete(directory, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 地域信息映射
        /// </summary>
        private void ImportAgriLandZoneMapping(YuLinTu.Business.ContractLand.Exchange2.ExZoneCollection zones)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            var zoneStation = database.CreateZoneWorkStation();
            foreach (YuLinTu.Business.ContractLand.Exchange2.ExZone exZone in zones)
            {
                try
                {
                    Zone zone = AgriZoneMapping(exZone);
                    var dbZone = zoneStation.Get(zone.FullCode);
                    if (dbZone != null)
                    {
                        zone.ID = dbZone.ID;
                        zoneStation.Update(zone);
                    }
                    else
                    {
                        zoneStation.Add(zone);
                    }
                }
                catch (Exception ex)
                {
                    this.ReportError("更新地域数据发生错误!");
                    Log.Log.WriteError(this, "ImportAgriLandZoneMapping(地域信息映射)", ex.Message + ex.StackTrace);
                    break;
                }
            }
        }

        /// <summary>
        /// 地域映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <returns></returns>
        private Zone AgriZoneMapping(YuLinTu.Business.ContractLand.Exchange2.ExZone exZone)
        {
            Zone zone = new Zone();
            zone.ID = exZone.ID;
            zone.Code = string.IsNullOrEmpty(exZone.Code) ? exZone.Code : zone.Code;
            zone.Comment = string.IsNullOrEmpty(exZone.Comment) ? zone.Comment : exZone.Comment;
            if (exZone.CreateTime == null)
            {
                zone.CreateTime = DateTime.Now;
            }
            else
            {
                zone.CreateTime = exZone.CreateTime;
            }
            zone.CreateUser = string.IsNullOrEmpty(exZone.CreateUser) ? zone.CreateUser : exZone.CreateUser;
            zone.FullCode = string.IsNullOrEmpty(exZone.FullCode) ? zone.FullCode : exZone.FullCode;
            zone.FullName = string.IsNullOrEmpty(exZone.FullName) ? zone.FullName : exZone.FullName;
            zone.Level = exZone.Level <= 0 ? zone.Level : (eZoneLevel)Enum.Parse(typeof(eZoneLevel), exZone.Level.ToString());
            if (exZone.ModifyTime == null)
            {
                zone.LastModifyTime = DateTime.Now;
            }
            else
            {
                zone.LastModifyTime = exZone.ModifyTime;
            }
            zone.LastModifyUser = string.IsNullOrEmpty(exZone.ModifyUser) ? zone.LastModifyUser : exZone.ModifyUser;
            zone.Name = string.IsNullOrEmpty(exZone.Name) ? zone.Name : exZone.Name;
            zone.UpLevelCode = string.IsNullOrEmpty(exZone.ParentCode) ? zone.UpLevelCode : exZone.ParentCode;
            zone.UpLevelName = string.IsNullOrEmpty(exZone.ParentName) ? zone.UpLevelName : exZone.ParentName;
            zone.Shape = YuLinTu.Spatial.Geometry.FromBytes(exZone.Shape.WellKnownBinary, 0);// exZone.Shape.Srid);
            return zone;
        }

        #endregion

    }
}
