/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    public class ExportLandShapeBusiness : Task
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



        /// <summary>
        /// 导出设置
        /// </summary>
        public ExportContractLandShapeDefine ExportContractLandShapeDefine = ExportContractLandShapeDefine.GetIntence();


        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandShapeBusiness(IDbContext db)
        {
            this.db = db;
            this.Station = db == null ? null : db.CreateZoneWorkStation();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandShapeBusiness()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出地域图斑
        /// </summary>
        public bool ExportZoneShape(Zone zone, string fileName)
        {
            bool reslut = true;
            try
            {
                TaskExportLandShapeArgument metadata = Argument as TaskExportLandShapeArgument;
                AccountLandBusiness ContractAccountBusiness = new AccountLandBusiness(DbContext);
                string markDes = GetZoneName(zone);
                List<ContractLand> initLandList = ContractAccountBusiness.GetCollection(zone.FullCode, eLevelOption.Self);
                List<ContractLand> listGeoLand = initLandList.FindAll(c => c.Shape != null);
                if (listGeoLand == null || listGeoLand.Count == 0)
                {
                    this.ReportProgress(0, string.Format("{0}", markDes));
                    this.ReportProgress(100, null);
                    this.ReportWarn("未获取到地块空间数据!");
                    return false;
                }
                //listGeoLand.LandNumberFormat(SystemSet);   //TODO 有问题
                //陈泽林 20161227 
                //int getlandnumcount = ExportContractLandShapeDefine.LandNumberGetCount;
                //foreach (var oldNumberItem in listGeoLand)
                //{
                //    if (string.IsNullOrEmpty(oldNumberItem.LandNumber))
                //        continue;
                //    int length = oldNumberItem.LandNumber.Length;
                //    if (length > getlandnumcount)
                //    {
                //        oldNumberItem.LandNumber = oldNumberItem.LandNumber.Substring(getlandnumcount, length - getlandnumcount);
                //    }
                //}
                using (ExportLandGeoToShape export = new ExportLandGeoToShape(ExportContractLandShapeDefine))
                {
                    fileName = Path.Combine(fileName, markDes + "地块.shp");
                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    export.ZoneDesc = markDes;
                    export.FileName = fileName;
                    export.CurrentZone = zone;
                    export.ListVp = metadata.vps;
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
                this.ReportError("导出地域图斑失败!" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地块图斑)", ex.Message + ex.StackTrace);
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
