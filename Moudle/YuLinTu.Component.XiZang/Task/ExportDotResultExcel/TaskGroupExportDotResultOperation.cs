/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出界址点成果表数据操作任务类
    /// </summary>
    public class TaskGroupExportDotResultOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportDotResultOperation()
        {
        }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportDotResultArgument groupMeta = Argument as TaskGroupExportDotResultArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            AccountLandBusiness landBuiness = new AccountLandBusiness(dbContext);
            List<Zone> childrenZone = landBuiness.GetChildrenZone(currentZone);
            Zone parentZone = landBuiness.GetParent(currentZone);
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, landBuiness);
            string folderString = CreateDirectory(allZones, currentZone);
            string fileName = groupMeta.FileName + @"\" + folderString;
            List<ContractLand> listGeoLand = groupMeta.ListGeoLand;
            List<VirtualPerson> listPerson = groupMeta.ListPerson;
            string markDesc = ExportZoneListDir(currentZone, allZones);
            if (listPerson.Count == 0)
            {
                this.ReportProgress(0, string.Format("{0}界址点成果表", markDesc));
                this.ReportProgress(100, null);
                this.ReportWarn(string.Format("{0}未获取承包方信息", markDesc));
                return;
            }
            foreach (var person in listPerson)
            {
                List<ContractLand> currentListLand = listGeoLand.FindAll(c => c.OwnerId == person.ID);
                string savePath = fileName + @"\" + person.Name;
                if (!System.IO.Directory.Exists(savePath) && currentListLand != null && currentListLand.Count > 0)
                    System.IO.Directory.CreateDirectory(savePath);
                TaskExportDotResultArgument meta = new TaskExportDotResultArgument();
                meta.CurrentZone = currentZone;
                meta.FileName = savePath;
                meta.Database = dbContext;
                meta.VirtualType = groupMeta.VirtualType;
                meta.CurrentPerson = person;
                meta.ListGeoLand = currentListLand;
                TaskExportDotResultOperation import = new TaskExportDotResultOperation();
                import.Argument = meta;
                import.Description = markDesc + person.Name;
                import.Name = "导出界址点成果表";
                Add(import);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion

        #region Method—Helper

        /// <summary>
        /// 获取全部的地域
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="fileName">保存路径</param>
        /// <param name="business">合同业务</param>
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, AccountLandBusiness business)
        {
            List<Zone> allZones = new List<Zone>();
            allZones.Add(currentZone);
            if (currentZone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(parentZone);
                allZones.Add(business.GetParent(parentZone));
            }
            else if (currentZone.Level == eZoneLevel.Village)
            {
                //选择为村
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                }
                allZones.Add(parentZone);
            }
            else if (currentZone.Level == eZoneLevel.Town)
            {
                //选择为镇
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                    List<Zone> zones = business.GetChildrenZone(child);
                    foreach (var zone in zones)
                    {
                        allZones.Add(zone);
                    }
                }
            }
            return allZones;
        }

        /// <summary>
        /// 创建文件目录(可以创建至组)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectory(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        /// <summary>
        /// 创建文件目录(仅创建至村)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectoryByVilliage(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Level == eZoneLevel.Group ? "" : cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary>       
        private string ExportZoneListDir(Zone zone, List<Zone> allZones)
        {
            string exportzonedir = string.Empty;
            if (zone.Level == eZoneLevel.Group)
            {
                Zone vzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                Zone tzone = allZones.Find(t => t.FullCode == vzone.UpLevelCode);
                exportzonedir = tzone.Name + vzone.Name + zone.Name;
            }
            if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            return exportzonedir;
        }

        #endregion
    }
}
