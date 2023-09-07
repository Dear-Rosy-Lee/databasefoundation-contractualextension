/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using System.Collections;
using YuLinTu.Library.Office;
using System.Text.RegularExpressions;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入发包方数据类
    /// </summary>
    [Serializable]
    public class ImportSenderData : Task
    {
        #region Fields

        private ToolProgress toolProgress;//进度工具
        private List<CollectivityTissue> tissueCollection;//发包方集合
        private List<string> errorList;//错误列表
        private List<string> warnList; //警告列表
        private int addCount;//添加数
        private int updateCount;//更新数
        private ISenderWorkStation station;//表查询
        public IDbContext dataInstance;//数据库
        private string ExcelError = "Excel数据错误,其中没有信息";

        #endregion

        #region Propertys

        /// <summary>
        /// 数据库实例
        /// </summary>
        public IDbContext DataInstance
        {
            get { return dataInstance; }
            set
            {
                dataInstance = value;
                if (value != null)
                {
                    ContainerFactory factroy = new ContainerFactory(dataInstance);
                    station = factroy.CreateSenderWorkStation();
                }
            }
        }

        /// <summary>
        /// 是否清空数据
        /// </summary>
        public bool IsClear { get; set; }

        #endregion

        #region Ctor

        public ImportSenderData()
        {
            toolProgress = new ToolProgress();
        }

        #endregion

        #region Methods - ReadInformation

        /// <summary>
        /// 读取发包方信息
        /// </summary>
        public void ReadZoneInformation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            ExcelReaderSender reader = new ExcelReaderSender();
            try
            {
                reader.Open(fileName);
                reader.ReadInformation();
                tissueCollection = reader.TissueCollection;
                errorList = reader.ErrorCollection;
                warnList = reader.WarnCollection;
            }
            catch (Exception e)
            {
                this.ReportError(string.Format("{0}", e.Message.ToString()));
                reader.Dispose();
                return;
            }
            finally
            {
                reader.Dispose();
            }
            if (warnList != null && warnList.Count > 0)
            {
                foreach (string warn in warnList)
                {
                    this.ReportWarn(warn);
                }
            }
            if (errorList != null && errorList.Count > 0)
            {
                foreach (string error in errorList)
                {
                    this.ReportError(error);
                }
            }
            if ((tissueCollection == null || tissueCollection.Count < 1) && (errorList == null || errorList.Count == 0))
            {
                this.ReportError(ExcelError);
            }
        }

        #endregion

        #region Methods - Import

        /// <summary>
        /// 导入发包方实体
        /// </summary>
        public void InportSenderEntity()
        {
            IDbContext db = DataInstance as IDbContext;
            toolProgress.InitializationPercent(tissueCollection.Count, 90);
            try
            {
                db.OpenConnection();
                db.BeginTransaction();
                InportZoneToDatabase(tissueCollection);
                db.CommitTransaction();
            }
            catch (SystemException ex)
            {
                db.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "InportSenderEntity(导入发包方)", ex.Message + ex.StackTrace);
                this.ReportProgress(5, "数据表加载完成");
            }
            finally
            {
                db.CloseConnection();
            }
            this.ReportInfomation(string.Format("当前表中共有{0}条数据,成功导入{1}条记录,成功更新{2}条记录", tissueCollection.Count, addCount, updateCount));
        }

        /// <summary>
        /// 导入发包方数据到数据库中
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private bool InportZoneToDatabase(List<CollectivityTissue> tissueList)
        {
            try
            {
                addCount = 0;
                updateCount = 0;
                ContainerFactory factroy = new ContainerFactory(DataInstance);
                ISenderWorkStation station = factroy.CreateSenderWorkStation();
                int index = 1;
                double step = 80 / (double)tissueList.Count;

                foreach (CollectivityTissue zone in tissueList)
                {
                    if (Add(zone, station))
                    {
                        this.ReportProgress(20 + (int)(step * index), "导入" + zone.Name);
                        index++;
                    }
                }
                tissueList = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
            }
            return true;
        }

        /// <summary>
        /// 添加发包方信息
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private bool Add(CollectivityTissue tissue, ISenderWorkStation station)
        {
            string townCode = tissue.Code.Substring(0, Zone.ZONE_TOWN_LENGTH);
            string villageCode = tissue.Code.Substring(0, Zone.ZONE_VILLAGE_LENGTH);
            string groupCode = string.Empty;
            if (!string.IsNullOrEmpty(tissue.Code) && tissue.Code.Length == 14)
                groupCode = tissue.Code.Substring(0, 12) + tissue.Code.Substring(12);
            else if (!string.IsNullOrEmpty(tissue.Code) && tissue.Code.Length == 16)
                groupCode = tissue.Code.Substring(0, 12) + tissue.Code.Substring(14);
            Zone town = GetZoneByCode(townCode);
            if (town == null)
            {
                this.ReportError(string.Format("系统中不存在乡镇级行政区域代码{0}!", townCode));
                return false;
            }
            Zone village = GetZoneByCode(villageCode);
            if (village == null)
            {
                if (town != null && tissue.Code.Length == 14)
                {
                    tissue.ZoneCode = town.FullCode;
                    HandleData(tissue, station);
                    return true;
                }
                else
                {
                    this.ReportError(string.Format("系统中不存在村级行政区域代码{0}!", villageCode));
                    return false;
                }
            }
            Zone group = GetZoneByCode(groupCode);
            if (group == null)
            {
                if (village != null && tissue.Code.Length == 14)
                {
                    tissue.ZoneCode = village.FullCode;
                    HandleData(tissue, station);
                    return true;
                }
                else
                {
                    this.ReportError(string.Format("系统中不存在组级行政区域代码{0}!", groupCode));
                    return false;
                }
            }
            tissue.ZoneCode = groupCode;
            HandleData(tissue, station);
            return true;
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        private void HandleData(CollectivityTissue tissue, ISenderWorkStation station)
        {
            CollectivityTissue ct = station.GetByCode(tissue.Code);
            if (ct == null)
            {
                addCount++;
                station.Add(tissue);
            }
            else
            {
                updateCount++;
                tissue.ID = ct.ID;
                station.Update(tissue);
            }
        }

        /// <summary>
        /// 行政地域获取
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        private Zone GetZoneByCode(string zoneCode)
        {
            ModuleMsgArgs argsGet = new ModuleMsgArgs();
            argsGet.Name = ZoneMessage.ZONE_GET;
            argsGet.Parameter = zoneCode;
            argsGet.Datasource = DataBaseSource.GetDataBaseSource();
            TheBns.Current.Message.Send(this, argsGet);
            Zone zone = argsGet.ReturnValue as Zone;
            return zone;
        }

        #region Methods - HelperTissue

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
                    code = zone.FullCode + "00000";
                    break;
                case eZoneLevel.Village:
                    code = zone.FullCode + "00";
                    break;
                case eZoneLevel.Group:
                    code = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14);
                    break;
                default:
                    break;
            }
            return code;
        }

        #endregion

        #endregion
    }
}
