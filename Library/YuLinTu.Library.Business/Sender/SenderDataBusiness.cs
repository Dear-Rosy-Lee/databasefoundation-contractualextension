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
using Microsoft.Practices.Unity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方业务处理类
    /// </summary>
    public class SenderDataBusiness : Task
    {
        #region Fildes

        private BackgroundWorker worker;
        private IDbContext db;

        #endregion

        #region Properties

        /// <summary>
        /// 地域表操作接口
        /// </summary>
        public ISenderWorkStation Station { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 使用行政地域组织为发包方名称
        /// </summary>
        public bool UseAliasName { get; set; }

        #endregion

        #region Ctor

        public SenderDataBusiness()
        {
        }

        public SenderDataBusiness(IDbContext db)
        {
            this.db = db;
            this.Station = db == null ? null : db.CreateSenderWorkStation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 添加发包方数据
        /// </summary>
        public bool AddSender(CollectivityTissue tissue)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                int num = Station.Add(tissue);
                result = (num == 1) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "AddSender(添加发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加发包方出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 更新发包方数据
        /// </summary>
        public bool UpdateSender(CollectivityTissue tissue)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                int num = Station.Update(tissue);
                result = (num == 1) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateSender(更新发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("更新发包方出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除发包方数据
        /// </summary>
        public bool DeleteSender(CollectivityTissue tissue)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                int num = Station.Delete(tissue);
                result = (num > 0) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteSender(删除发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除发包方出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除发包方数据
        /// </summary>
        public bool DeleteSender(string tissueCode)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                int num = Station.Delete(t => t.Code == tissueCode);
                result = (num > 0) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteSender(删除发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除发包方出错," + ex.Message);
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 删除发包方数据
        /// </summary>
        public bool DeleteSenders(List<string> tissueCodes)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                int num = Station.Delete(t => tissueCodes.Contains(t.Code));
                result = (num > 0) ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteSender(删除发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除发包方出错," + ex.Message);
                result = false;
            }
            return result;
        }



        /// <summary>
        /// 获取指定地域编码下的发包方
        /// </summary>
        public List<CollectivityTissue> SendersByCode(string zoneCode)
        {

            List<CollectivityTissue> list = null;
            if (!CanContinue())
            {
                return list;
            }
            if (zoneCode == null)
                return list;
            try
            {
                list = Station.GetTissues(zoneCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "SendersByCode(地域编码下发包方)", ex.Message + ex.StackTrace);
                this.ReportError("获取指定地域编码下的发包方出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 获取指定地域及子地域下的发包方
        /// </summary>
        public List<CollectivityTissue> SenderSubsByCode(string zoneCode)
        {
            List<CollectivityTissue> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                list = Station.GetTissues(zoneCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "SenderListByCode(地域编码下发包方)", ex.Message + ex.StackTrace);
                this.ReportError("获取指定地域及子地域下的发包方出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据Id获取发包方
        /// </summary>
        public CollectivityTissue GetSenderById(Guid id)
        {
            CollectivityTissue tissue = null;
            if (!CanContinue() || id == null)
            {
                return null;
            }
            try
            {
                tissue = Station.Get(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetSenderCode(获取新的发包方编码)", ex.Message + ex.StackTrace);
                this.ReportError("获取新的发包方编码出错," + ex.Message);
            }
            return tissue;
        }

        /// <summary>
        /// 获取新的发包方编码
        /// </summary>
        public string GetSenderCode(Zone zone, CollectivityTissue tissue)
        {
            string tissucCode = string.Empty;
            if (!CanContinue())
            {
                return tissucCode;
            }
            try
            {
                tissucCode = CreatSenderCode(zone, tissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetSenderCode(获取新的发包方编码)", ex.Message + ex.StackTrace);
                this.ReportError("获取新的发包方编码出错," + ex.Message);
            }
            return tissucCode;
        }

        /// <summary>
        /// 创建发包方编码
        /// </summary>
        public string CreatSenderCode(Zone currentZone, CollectivityTissue currentTissue)
        {
            string tissucCode = string.Empty;
            if (!CanContinue())
            {
                return tissucCode;
            }
            int count = Station.Count(currentZone.FullCode, eLevelOption.Self);
            if (count == 0 && currentTissue != null)
            {
                currentTissue.ID = currentZone.ID;
            }
            tissucCode = InitalizeTissueCodeValue(currentZone.FullCode, currentZone.Level) + string.Format("{0:D2}", count);
            while (Station.CodeExists(tissucCode))
            {
                ++count;
                tissucCode = InitalizeTissueCodeValue(currentZone.FullCode, currentZone.Level) + string.Format("{0:D2}", count);
            }
            return tissucCode;
        }

        /// <summary>
        /// 初始化发包方代码
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private string InitalizeTissueCodeValue(string code, eZoneLevel level)
        {
            switch (level)
            {
                case eZoneLevel.Town:
                    code = code + "000";
                    break;
                case eZoneLevel.Village:
                    break;
                case eZoneLevel.Group:
                    code = code.Substring(0, 12);
                    break;
                default:
                    break;
            }
            return code;
        }

        /// <summary>
        /// 发包方名称是否重复
        /// </summary>
        public bool SenderNameRepeat(CollectivityTissue currentTissue)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                result = Station.Any(t => t.Name == currentTissue.Name && t.Code != currentTissue.Code);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "SenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
                this.ReportError("查询发包方名称是否重复出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 发包方是否默认
        /// </summary>
        public bool IsDefaultSender(CollectivityTissue currentTissue)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                result = Station.IsDefaultTissue(currentTissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsDefaultSender(发包方是否默认)", ex.Message + ex.StackTrace);
                this.ReportError("查询发包方是否默认出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 地域下是否存在发包方
        /// </summary>
        public bool SenderExitsInZone(string zoneCode)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                result = Station.Exists(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "SenderExitsInZone(地域下是否存在发包方)", ex.Message + ex.StackTrace);
                this.ReportError("查询地域下是否存在发包方出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 清空地域下的发包方
        /// </summary>
        public bool ClearByCode(string zoneCode)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                int num = Station.DeleteByZoneCode(zoneCode, eLevelOption.Self);
                result = num > 0 ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearByCode(清空地域下的发包方)", ex.Message + ex.StackTrace);
                this.ReportError("清空地域下的发包方出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 删除地域下发包方
        /// </summary>
        public bool DelZoneTissue(string zoneCode)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                int num = Station.DeleteByZoneCode(zoneCode, eLevelOption.Self);
                result = num > 0 ? true : false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DelZoneComplate(删除地域下发包方)", ex.Message + ex.StackTrace);
                this.ReportError("删除地域下发包方出错," + ex.Message);
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
                this.ReportError("尚未初始化发包方表的访问接口");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 导入地域完成(适用于导入行政地域)
        /// </summary>
        /// <param name="arg">参数</param>
        /// <param name="reportProgress">
        /// 添加于2016/10/15
        /// 为了满足初始化发包方外部报告进度而使用</param>
        public void ProcessZoneComplateForImport(MultiObjectArg arg, Action<int, string> reportProgress = null)
        {
            if (!CanContinue())
                return;
            MultiObjectArg multiarg = arg;
            if (multiarg == null)
            {
                return;
            }
            List<Zone> zones = multiarg.ParameterA as List<Zone>;//新增地域
            List<Zone> dbzoneList = multiarg.ParameterB as List<Zone>;//原有地域
            ZoneDefine define = multiarg.ParameterD as ZoneDefine;//地域配置信息
            UseAliasName = define == null ? false : define.IntiallData;
            bool modify = multiarg.ParameterC == null ? true : (bool)multiarg.ParameterC;//修改名称
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            if (dbzoneList == null)
            {
                dbzoneList = new List<Zone>();
            }
            try
            {
                DbContext.OpenConnection();
                DbContext.BeginTransaction();
                UpdateTissue(Station, zones, dbzoneList, modify, reportProgress);
                DbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                DbContext.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "后台线程更新发包方", ex.Message + ex.StackTrace);
            }
            finally
            {
                zones = null;
                dbzoneList = null;
                DbContext.CloseConnection();
            }
        }

        /// <summary>
        /// 导入地域完成
        /// </summary>
        public void ProcessZoneComplate(MultiObjectArg arg)
        {
            if (!CanContinue())
            {
                return;
            }
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync(arg);
        }

        /// <summary>
        /// 后台线程更新发包方
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MultiObjectArg multiarg = e.Argument as MultiObjectArg;
            if (multiarg == null)
            {
                return;
            }
            List<Zone> zones = multiarg.ParameterA as List<Zone>;//新增地域
            List<Zone> dbzoneList = multiarg.ParameterB as List<Zone>;//原有地域
            ZoneDefine define = multiarg.ParameterD as ZoneDefine;//地域配置信息
            UseAliasName = define == null ? false : define.IntiallData;
            bool modify = multiarg.ParameterC == null ? true : (bool)multiarg.ParameterC;//修改名称
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            if (dbzoneList == null)
            {
                dbzoneList = new List<Zone>();
            }
            try
            {
                DbContext.OpenConnection();
                DbContext.BeginTransaction();
                UpdateTissue(Station, zones, dbzoneList, modify);
                DbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                DbContext.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "后台线程更新发包方", ex.Message + ex.StackTrace);
            }
            finally
            {
                zones = null;
                dbzoneList = null;
                DbContext.CloseConnection();
            }
        }

        /// <summary>
        /// 更新发包方
        /// </summary>
        /// <param name="reportProgress">添加于2016/10/15
        /// 为了满足初始化发包方外部报告进度而使用</param>
        private void UpdateTissue(ISenderWorkStation station, List<Zone> zones, List<Zone> dbZones, bool fixName, Action<int, string> reportProgress = null)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            string exceptCode = string.Empty;
            string exceptName = string.Empty;

            if (reportProgress != null)
                reportProgress(1, "开始初始化发包方。");
            int index = 0;
            double percent = 99 / (double)zones.Count;

            foreach (Zone zone in zones)
            {
                if (zone.FullCode.Length < Zone.ZONE_TOWN_LENGTH)//镇村组才有集体经济组织
                {
                    continue;
                }
                if (zone.FullCode.Substring(0, 4) != exceptCode)
                {
                    exceptCode = zone.FullCode.Substring(0, 4);
                    exceptName = station.GetZone(exceptCode).FullName;
                }
                Zone dbZone = dbZones.Find(t => t.ID == zone.ID);
                if (dbZone != null)//原来地域存在,则对发包方进行修改.
                {
                    List<CollectivityTissue> list = station.GetTissues(dbZone.FullCode);
                    if (list == null || list.Count == 0)
                    {
                        CollectivityTissue tissue = CreateTissue(zone, exceptName);
                        station.Add(tissue);
                    }

                    foreach (CollectivityTissue ct in list)
                    {
                        if (ct.ZoneCode != zone.FullCode)
                        {
                            ct.ZoneCode = zone.FullCode;
                            ct.Code = InitalizeTissueCode(zone);
                        }
                        if (zone.FullName != dbZone.FullName && ct.ID == zone.ID && fixName)//前后地域名称，则修改默认发包方名称
                        {
                            ct.Name = GetCollectivityTissueName(zone, exceptName);
                        }
                        station.Update(ct);
                    }
                }
                else
                {
                    CollectivityTissue tissue = CreateTissue(zone, exceptName);
                    station.Add(tissue);
                }

                if (reportProgress != null)
                    reportProgress((int)(1 + percent * (++index)), zone.FullName);
            }
        }

        /// <summary>
        /// 创建发包方
        /// </summary>
        private CollectivityTissue CreateTissue(Zone zone, string exceptName)
        {
            CollectivityTissue tissue = new CollectivityTissue();
            tissue.Name = GetCollectivityTissueName(zone, exceptName);
            if (string.IsNullOrEmpty(tissue.Name))
            {
                return null;
            }
            tissue.ID = zone.ID;
            tissue.Type = eTissueType.General;
            tissue.Status = eStatus.Register;
            tissue.Founder = "Admin";//创建人
            tissue.Modifier = "Admin";//修改人
            tissue.ZoneCode = zone.FullCode;//地域代码
            tissue.Code = InitalizeTissueCode(zone);
            tissue.CreationTime = DateTime.Now;
            tissue.ModifiedTime = DateTime.Now;
            return tissue;
        }

        /// <summary>
        /// 初始化发包方代码
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private string InitalizeTissueCode(Zone zone)
        {
            string code = zone.FullCode;
            switch (zone.Level)
            {
                case eZoneLevel.Town:
                    code = zone.FullCode.PadRight(14, '0');
                    break;
                case eZoneLevel.Village:
                    code = zone.FullCode.PadRight(14, '0');
                    break;
                case eZoneLevel.Group:
                    if (zone.FullCode.Length == 16)
                    {
                        code = ZoneHelper.ChangeCodeShort(zone.FullCode);
                    }
                    else
                    {
                        code = zone.FullCode;
                    }
                    break;
                default:
                    break;
            }
            return code;
        }

        /// <summary>
        /// 获取集体经济组织名称
        /// </summary>
        /// <returns></returns>
        private string GetCollectivityTissueName(Zone zone, string exceptName)
        {
            if (string.IsNullOrEmpty(exceptName))
            {
                return "";
            }
            string tissueName = string.Empty;
            tissueName = zone.FullName.Replace(exceptName, "");
            //switch (zone.Level)
            //{
            //    case eZoneLevel.Town:
            //        tissueName = zone.FullName.Replace(exceptName, "") + (UseAliasName ? "" : "集体资产管理委员会");
            //        break;
            //    case eZoneLevel.Village:
            //        tissueName = zone.FullName.Replace(exceptName, "") + (UseAliasName ? "" : "农业合作联社");
            //        break;
            //    case eZoneLevel.Group:
            //        tissueName = zone.FullName.Replace(exceptName, "") + (UseAliasName ? "" : "第" + Convert.ToInt16(zone.Code).ToString() + "农业合作社");
            //        break;
            //    default:
            //        break;
            //}
            return tissueName;
        }

        /// <summary>
        /// 清除发包方数据
        /// </summary>
        public bool ClearSender()
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                result = Station.Clear();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearSender(清除发包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("清除发包方数据出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 根据地域添加默认发包方
        /// </summary>
        public bool AddSenderByZone(Zone zone)
        {
            bool result = false;
            string exceptCode = zone.FullCode.Substring(0, 4);
            string exceptName = Station.GetZone(exceptCode).FullName;
            CollectivityTissue tissue = CreateTissue(zone, exceptName);
            int num = Station.Add(tissue);
            result = num > 0 ? true : false;
            return result;
        }

        #endregion
    }
}
