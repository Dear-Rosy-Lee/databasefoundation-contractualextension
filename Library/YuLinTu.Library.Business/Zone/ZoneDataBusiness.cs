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
    public class ZoneDataBusiness : Task
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
        /// 地域设置
        /// </summary>
        public ZoneDefine Define { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoneDataBusiness(IDbContext db)
        {
            this.db = db;
            this.Station = db == null ? null : db.CreateZoneWorkStation();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoneDataBusiness()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取地域
        /// </summary>
        public Zone Get(string zoneCode)
        {
            Zone zone = null;
            if (!CanContinue())
            {
                return zone;
            }
            try
            {
                zone = Station.Get(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取地域)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域," + ex.Message);
            }
            return zone;
        }

        /// <summary>
        /// 添加地域数据
        /// </summary>
        public bool Add(Zone zone)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                Station.Add(zone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Add(添加地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加地域出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 是否可继续
        /// </summary>
        private bool CanContinue()
        {
            if (Station == null)
            {
                this.ReportError("尚未初始化地域表的访问接口");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 地域编码是否存在
        /// </summary>
        public bool IsZoneCodeExist(string zoneCode)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                result = Station.Exists(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsZoneCodeExist(地域编码是否存在)", ex.Message + ex.StackTrace);
                this.ReportError("查询地域编码是否存在出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 地域编码是否存在(ID不同)
        /// </summary>
        public bool IsZoneCodeExist(Zone zone)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                result = Station.Any(t => t.FullCode == zone.FullCode && t.ID != zone.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsZoneCodeExist(地域编码是否存在)", ex.Message + ex.StackTrace);
                this.ReportError("查询地域编码是否存在出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 更新地域数据
        /// </summary>
        public bool UpdateZone(Zone zone)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                result = (Station.Update(zone) == 1) ? true : false; ;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateZone(更新地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("更新地域出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 更新地域数据
        /// </summary>
        public bool UpdateZoneCodeName(Zone zone)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                result = (Station.UpdateCodeName(zone) == 1) ? true : false; ;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateZone(更新地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("更新地域出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 上传地域数据到服务
        /// </summary>
        /// <param name="cZone">当前地域</param>
        /// <param name="isEncrypt">是否加密</param>
        /// <param name="businessDataService">服务地址</param>
        /// <returns></returns>
        public bool UpdateToService(Zone cZone, bool isEncrypt, string businessDataService, string user, string sessionCode)
        {
            bool result = false;
            this.ReportProgress(1);
            List<Zone> zones = GetAllZone();
            this.ReportProgress(10);
            if (zones == null || zones.Count == 0)
            {
                return result;
            }
            List<Zone> list = ZoneListFromCollection(zones, cZone.FullCode);
            if (list.Count == 0)
            {
                this.ReportProgress(100, "完成");
                return result;
            }
            this.ReportProgress(15, "获取数据");
            IList<YuLinTu.PropertyRight.Registration.EX_XZDY> zoneCollection = WebZoneMapping(list);
            if (zoneCollection == null || zoneCollection.Count == 0)
            {
                return result;
            }
            try
            {
                this.ReportProgress(20, "正在上传数据");
                YuLinTu.PropertyRight.Services.Client.ContractLandRegistrationServiceClient landService = ServiceHelper.InitazlieServerData(user, sessionCode, isEncrypt, businessDataService);
                landService.ImportZones(zoneCollection);
                this.ReportInfomation(string.Format("{0}下上传了{1}条数据", cZone.FullName, list.Count));
                this.ReportProgress(100, "完成上传");
                result = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateToService(上传地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("上传地域数据出错,详细信息:" + ex.Message);
                result = false;
            }
            finally
            {
                this.ReportProgress(100, "完成");
                zones = null;
                list = null;
                zoneCollection = null;
            }
            return result;
        }

        /// <summary>
        /// 行政区域数据映射
        /// </summary>
        private IList<YuLinTu.PropertyRight.Registration.EX_XZDY> WebZoneMapping(List<Zone> zones)
        {
            IList<YuLinTu.PropertyRight.Registration.EX_XZDY> zoneCollection = new List<YuLinTu.PropertyRight.Registration.EX_XZDY>();
            foreach (Zone exZone in zones)
            {
                YuLinTu.PropertyRight.Registration.EX_XZDY zone = new PropertyRight.Registration.EX_XZDY();
                zone.BM = exZone.Code;
                zone.ID = exZone.ID;
                zone.JB = (PropertyRight.eZoneLevel)exZone.Level;
                zone.MC = exZone.Name;
                zone.QBM = exZone.FullCode;
                if (!Define.UseStandCode && exZone.FullCode.Length == 14)
                {
                    string newZoneCode = exZone.FullCode.Substring(0, 12) + "00" + exZone.FullCode.Substring(12, 2);
                    zone.QBM = newZoneCode;
                    if (!string.IsNullOrEmpty(exZone.Code))
                        zone.BM = exZone.Code.PadLeft(4, '0');
                }
                zone.QMC = exZone.FullName;
                zone.Shape = exZone.Shape;
                zone.SJQBM = exZone.UpLevelCode;
                zone.SJQC = exZone.UpLevelName;
                zoneCollection.Add(zone);
            }
            return zoneCollection;
        }

        /// <summary>
        /// 删除地域数据
        /// </summary>
        public bool DeleteZone(string zoneCode)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                int num = Station.DeleteCollection(zoneCode, eLevelOption.SelfAndSubs);
                result = (num > 0) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteZone(删除地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除地域出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 清除地域数据
        /// </summary>
        public bool ClearZone()
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                int clearResult = Station.Delete();
                int addResult = 1;
                Zone china = ZoneHelper.China;
                china.ID = Guid.NewGuid();
                Station.Add(china);
                result = (clearResult > 0 && addResult > 0) ? true : false;
            }
            catch (NullReferenceException)
            {
                throw new Exception("数据库可能正在使用,无法完成清空操作！");
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除地域出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        public List<Zone> GetAllZone()
        {
            List<Zone> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                list = Station.GetAll();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetAllZone(获取所有数据)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据地域编码获取地域集合
        /// </summary>
        public List<Zone> ZonesByCode(string zoneCode, eLevelOption level = eLevelOption.SelfAndSubs)
        {
            List<Zone> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                if (!string.IsNullOrEmpty(zoneCode))
                {
                    list = Station.GetChildren(zoneCode, level);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取地域数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 获取指定地域编码的地域
        /// </summary>
        public Zone ZoneByCode(string zoneCode)
        {
            Zone z = null;
            if (!CanContinue())
            {
                return z;
            }
            try
            {
                if (!string.IsNullOrEmpty(zoneCode))
                {
                    z = Station.Get(zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZoneByCode(获取地域数据)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域数据出错," + ex.Message);
            }
            return z;
        }

        /// <summary>
        /// 导出地域表
        /// </summary>
        public bool ExportZoneTable(Zone zone, string fileName)
        {
            bool reslut = true;
            if (!CanContinue() || zone == null)
            {
                return false;
            }
            try
            {
                List<Zone> zones = ZonesByCode(zone.FullCode);
                var listZones = Station.GetAllZonesToProvince(zone);
                string markDes = GetZoneName(zone);
                if (zones == null || zones.Count == 0)
                {
                    this.ReportError("未获取到地域数据!");
                    return false;
                }
                using (ExportZoneToExcel export = new ExportZoneToExcel())
                {
                    export.ZoneDesc = markDes;
                    export.SaveFilePath = fileName + @"\" + "地域信息表.xls";
                    export.CurrentZone = zone;
                    export.ZoneList = listZones;
                    export.IsStandCode = Define == null ? true : Define.UseStandCode;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.BeginExcel();
                    export.SaveAs(export.SaveFilePath);
                    //System.Diagnostics.Process.Start(export.SaveFilePath);
                }
                this.ReportInfomation(string.Format("成功导出{0}条地域数据!", listZones.Count));
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地域表)", ex.Message + ex.StackTrace);
                reslut = false;
            }
            return reslut;
        }

        /// <summary>
        /// 导出地域图斑
        /// </summary>
        public bool ExportZoneShape(Zone zone, string fileName)
        {
            bool reslut = true;
            if (!CanContinue() || zone == null)
            {
                return false;
            }
            try
            {
                List<Zone> zones = ZonesByCode(zone.FullCode);
                string markDes = GetZoneName(zone);
                if (zones == null || zones.Count == 0)
                {
                    this.ReportError("未获取到地域数据!");
                    return false;
                }
                using (ExportZoneToShape export = new ExportZoneToShape())
                {
                    fileName = Path.Combine(fileName, zone.Name + ".shp");
                    export.ZoneDesc = markDes;
                    export.FileName = fileName;
                    export.CurrentZone = zone;
                    export.ZoneList = zones;
                    export.IsStandCode = Define == null ? true : Define.UseStandCode;
                    export.Lang = HeaderLanguage.GetLanguage();
                    export.ProgressChanged += ReportPercent;
                    export.Alert += ReportInfo;
                    export.ExportToShape();
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地域图斑)", ex.Message + ex.StackTrace);
                reslut = false;
            }
            return reslut;
        }

        /// <summary>
        /// 导出地域压缩包
        /// </summary>
        public bool ExportZonePackage(Zone zone, string fileName)
        {
            bool reslut = true;
            if (!CanContinue() || zone == null)
            {
                return false;
            }
            try
            {
                List<Zone> zones = ZonesByCode(zone.FullCode);
                string markDes = GetZoneName(zone);
                if (zones == null || zones.Count == 0)
                {
                    this.ReportError("未获取到地域数据!");
                    return false;
                }
                zones.RemoveAll(t => t.Level == eZoneLevel.State);
                string zoneName = SpliteZoneName(zone);
                using (ExportZonePackage export = new ExportZonePackage())
                {
                    export.ZoneDesc = markDes;
                    export.FileName = fileName;
                    export.CurrentZone = zone;
                    export.IsStandCode = Define == null ? true : Define.UseStandCode;
                    export.ProgressChanged += ReportPercent;
                    export.Alert += ReportInfo;
                    export.ExportPackage(zones, zoneName);
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导出地域压缩包)", ex.Message + ex.StackTrace);
                reslut = false;
            }
            return reslut;
        }

        /// <summary>
        /// 截取地域名称
        /// </summary>
        private string SpliteZoneName(Zone zone)
        {
            if (zone == null)
            {
                return string.Empty;
            }
            //if (zone.Level > eZoneLevel.County)
            //{
            //    return zone.FullName;
            //}            
            //zone = Station.Get(zone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            //string zoneName = zone != null ? zone.FullName.Replace(zone.FullName, "") : zone.FullName;

            return zone.FullName;
        }

        /// <summary>
        /// 导入地域数据
        /// </summary>
        public object ImportZoneData(string fileName, bool isClear)
        {
            object result = null;
            try
            {
                if (isClear)
                {
                    ClearZone();
                }
                using (ImportZoneData zoneImport = new ImportZoneData())
                {
                    zoneImport.ProgressChanged += ReportPercent;
                    zoneImport.Alert += ReportInfo;
                    zoneImport.DataInstance = DbContext;
                    zoneImport.Station = Station;
                    this.ReportProgress(1, "开始读取数据");
                    zoneImport.ReadZoneInformation(fileName);
                    zoneImport.InitalizeZoneList();
                    this.ReportProgress(10, "开始检查数据");
                    bool canImport = zoneImport.ZoneInformtionCheck();
                    if (canImport)
                    {
                        this.ReportProgress(20, "开始处理数据");
                        zoneImport.InportZoneEntity();
                        this.ReportProgress(100, "完成");
                        zoneImport.MultiArg.ParameterD = Define;
                        result = zoneImport.MultiArg;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportZoneTable(导入地域数据)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 导入地域图斑数据
        /// </summary>
        public bool ImportZoneShape(string fileName, bool isCleart)
        {
            bool result = true;
            try
            {
                using (ImportZoneShape import = new ImportZoneShape())
                {
                    import.ProgressChanged += ReportPercent;
                    import.Alert += ReportInfo;
                    import.DataInstance = DbContext;
                    import.Station = Station;
                    import.IsShapeCombine = Define == null ? false : Define.ShapeToBase;
                    import.IsClear = isCleart;
                    this.ReportProgress(1, "开始读取数据");
                    import.ReadZoneInformation(fileName);
                    import.InitalizeZoneList();
                    this.ReportProgress(10, "开始处理数据");
                    import.InportZoneShape();
                    this.ReportProgress(100, "完成");
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导入数据发生错误," + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportZoneShape(导入地域图斑数据)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        public string GetZoneName(Zone zone)
        {
            string name = zone.Name;
            if (!CanContinue() || zone.FullCode.Length <= 6)
            {
                return name;
            }
            try
            {
                Zone tempZone = Station.Get(zone.UpLevelCode);
                while (tempZone.Level <= eZoneLevel.County)
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
        /// 查询地域下是否存在业务
        /// </summary>
        public bool HasBusinessInZone(string zoneCode)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = this.DbContext;
            arg.Parameter = zoneCode;
            arg.Name = ZoneMessage.ZONE_EXIST_VIRTUALPERSON;
            TheBns.Current.Message.Send(this, arg);
            bool result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            arg.Name = ZoneMessage.ZONE_EXIST_AGRICULTURELAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }

            arg.Name = ZoneMessage.ZONE_EXIST_COLLECTIVELAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            arg.Name = ZoneMessage.ZONE_EXIST_CONSTRUCTIONLAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }

            arg.Name = ZoneMessage.ZONE_EXIST_HOMESTEADLAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// 查询是否存在业务
        /// </summary>
        public bool HasBusiness()
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = this.DbContext;
            arg.Name = ZoneMessage.ZONE_EXIST_VIRTUALPERSON;
            TheBns.Current.Message.Send(this, arg);
            bool result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            arg.Name = ZoneMessage.ZONE_EXIST_AGRICULTURELAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }

            arg.Name = ZoneMessage.ZONE_EXIST_COLLECTIVELAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            arg.Name = ZoneMessage.ZONE_EXIST_CONSTRUCTIONLAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }

            arg.Name = ZoneMessage.ZONE_EXIST_HOMESTEADLAND;
            TheBns.Current.Message.Send(this, arg);
            result = (bool)arg.ReturnValue;
            if (result)
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// 查找父级及子集地域
        /// </summary>
        public List<Zone> ZoneListFromCollection(List<Zone> zones, string zoneCode)
        {
            List<Zone> list = new List<Zone>();
            if (zones == null || zones.Count == 0)
            {
                return list;
            }
            if (zoneCode == "86")
            {
                this.ReportWarn("服务器已有中国的行政地域，请选择省级以下！");
                return list;
            }
            Zone cZone = zones.Find(t => t.FullCode == zoneCode);
            if (cZone == null)
            {
                return list;
            }
            var collection = zones.FindAll(t => t.FullCode != null && t.FullCode.StartsWith(zoneCode));
            collection.ForEach(t => list.Add(t));
            Zone upZone = zones.Find(t => t.FullCode == cZone.UpLevelCode);
            while (upZone != null && (upZone.FullCode != ""))
            {
                collection.Add(upZone);
                upZone = zones.Find(t => t.FullCode == upZone.UpLevelCode);
            }
            return collection;
        }

        #endregion

        #region 辅助方法

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
    }
}
