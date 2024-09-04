/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方模块业务处理
    /// </summary>
    public class VirtualPersonBusiness : Task
    {
        #region Fildes

        /// <summary>
        /// 是否有错
        /// </summary>
        private bool isErrorRecord = false;

        /// <summary>
        /// 是否可以导入
        /// </summary>
        private bool canImport = false;

        /// <summary>
        /// 用于记录错误信息
        /// </summary>
        private string errorMesssage;

        /// <summary>
        /// 承包方类型
        /// </summary>
        private eVirtualType virtualType;

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext dbContext;

        /// <summary>
        /// 承包经营Station
        /// </summary>
        private IVirtualPersonWorkStation<LandVirtualPerson> landStation;

        /// <summary>
        /// 林权Station
        /// </summary>
        private IVirtualPersonWorkStation<WoodVirtualPerson> woodStation;

        /// <summary>
        /// 集体建设用地Station
        /// </summary>
        private IVirtualPersonWorkStation<YardVirtualPerson> yardStation;

        /// <summary>
        /// 房屋所有Station
        /// </summary>
        private IVirtualPersonWorkStation<HouseVirtualPerson> houseStation;

        /// <summary>
        /// 集体Station
        /// </summary>
        private IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation;

        /// <summary>
        /// 二轮台账(承包方)Station
        /// </summary>
        private IVirtualPersonWorkStation<TableVirtualPerson> tableStation;

        /// <summary>
        /// 承包地块Station
        /// </summary>
        private IContractLandWorkStation contractLandStation;

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        private FamilyImportDefine familyImportSet;

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        private FamilyOutputDefine familyOutputSet;

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        private FamilyOtherDefine familyOtherSet;

        #endregion

        #region Properties

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; }
        }

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        public FamilyImportDefine FamilyImportSet
        {
            get { return familyImportSet; }
            set { familyImportSet = value; }
        }

        /// <summary>
        /// 承包方导出配置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet
        {
            get { return familyOutputSet; }
            set { familyOutputSet = value; }
        }

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet
        {
            get { return familyOtherSet; }
            set { familyOtherSet = value; }
        }
        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();
        #endregion

        #region Ctor

        public VirtualPersonBusiness(IDbContext db)
        {
            dbContext = db;
            landStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
            woodStation = db == null ? null : db.CreateVirtualPersonStation<WoodVirtualPerson>();
            yardStation = db == null ? null : db.CreateVirtualPersonStation<YardVirtualPerson>();
            houseStation = db == null ? null : db.CreateVirtualPersonStation<HouseVirtualPerson>();
            colleStation = db == null ? null : db.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
            tableStation = db == null ? null : db.CreateVirtualPersonStation<TableVirtualPerson>();
            contractLandStation = db == null ? null : dbContext.CreateContractLandWorkstation();
            virtualType = eVirtualType.Land;
        }

        #endregion

        #region Methods

        #region 业务

        /// <summary>
        /// 开启事物
        /// </summary>
        public void BeganTranscation()
        {
            if (dbContext != null)
                dbContext.BeginTransaction();
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        public void Commit()
        {
            if (dbContext != null)
                dbContext.CommitTransaction();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBack()
        {
            if (dbContext != null)
                dbContext.RollbackTransaction();
        }

        #endregion

        #region 数据处理

        public VirtualPerson GetVirtualPersonByID(Guid id)
        {
            VirtualPerson vp = null;
            if (!CanContinue())
            {
                return vp;
            }
            try
            {
                vp = landStation.Get(id);

            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "获取承包方", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方出错," + ex.Message);

            }
            return vp;
        }

        /// <summary>
        /// 获取承包方数据
        /// </summary>
        public VirtualPerson Get(Guid id)
        {
            VirtualPerson person = null;
            if (!CanContinue())
            {
                return person;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        person = landStation.Get(id);
                        break;
                    case eVirtualType.Yard:
                        person = yardStation.Get(id);
                        break;
                    case eVirtualType.House:
                        person = houseStation.Get(id);
                        break;
                    case eVirtualType.Wood:
                        person = woodStation.Get(id);
                        break;
                    case eVirtualType.CollectiveLand:
                        person = colleStation.Get(id);
                        break;
                    case eVirtualType.SecondTable:
                        person = tableStation.Get(id);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return person;
        }

        /// <summary>
        /// 根据名称与地域编码或得承包方信息
        /// </summary>
        public VirtualPerson Get(string name, string code)
        {
            VirtualPerson person = null;
            if (!CanContinue())
            {
                return person;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        person = landStation.Get(name, code);
                        break;
                    case eVirtualType.Yard:
                        person = yardStation.Get(name, code);
                        break;
                    case eVirtualType.House:
                        person = houseStation.Get(name, code);
                        break;
                    case eVirtualType.Wood:
                        person = woodStation.Get(name, code);
                        break;
                    case eVirtualType.CollectiveLand:
                        person = colleStation.Get(name, code);
                        break;
                    case eVirtualType.SecondTable:
                        person = tableStation.Get(name, code);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return person;
        }

        /// <summary>
        /// 更新发包方编码
        /// </summary>
        public void UpdataSenderCode(string oldsenderCode, CollectivityTissue tissue)
        {
            var vps = landStation.GetByZoneCode(oldsenderCode);
            if (vps.Count == 0)
                return;
            foreach (var vp in vps)
            {
                vp.OldVirtualCode = vp.ZoneCode.PadRight(14, '0') + vp.FamilyNumber.PadLeft(4, '0');
                vp.ZoneCode = tissue.ZoneCode;
            }
            landStation.UpdatePersonList(vps);
        }

        /// <summary>
        /// 根据承包方的名称获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        public List<VirtualPerson> GetCollection(string zoneCode, string Name)
        {
            List<VirtualPerson> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        list = landStation.GetCollection(zoneCode, Name);
                        break;
                    case eVirtualType.Yard:
                        list = yardStation.GetCollection(zoneCode, Name);
                        break;
                    case eVirtualType.House:
                        list = houseStation.GetCollection(zoneCode, Name);
                        break;
                    case eVirtualType.Wood:
                        list = woodStation.GetCollection(zoneCode, Name);
                        break;
                    case eVirtualType.CollectiveLand:
                        list = colleStation.GetCollection(zoneCode, Name);
                        break;
                    case eVirtualType.SecondTable:
                        list = tableStation.GetCollection(zoneCode, Name);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 获取新的户号
        /// </summary>
        public string GetFamilyNumber(string zoneCode, eContractorType contractorType = eContractorType.Farmer)
        {
            string number = string.Empty;
            if (!CanContinue())
            {
                return number;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        number = landStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                    case eVirtualType.Yard:
                        number = yardStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                    case eVirtualType.House:
                        number = houseStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                    case eVirtualType.Wood:
                        number = woodStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                    case eVirtualType.CollectiveLand:
                        number = colleStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                    case eVirtualType.SecondTable:
                        number = tableStation.CreateVirtualPersonNum(zoneCode, contractorType);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetFamilyNumber(获取新的户号)", ex.Message + ex.StackTrace);
                this.ReportError("获取新的户号出错," + ex.Message);
            }
            return number;
        }

        /// <summary>
        /// 添加承包方数据
        /// </summary>
        public bool Add(VirtualPerson vp)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        landStation.Add(vp);
                        break;
                    case eVirtualType.Yard:
                        yardStation.Add(vp);
                        break;
                    case eVirtualType.House:
                        houseStation.Add(vp);
                        break;
                    case eVirtualType.Wood:
                        woodStation.Add(vp);
                        break;
                    case eVirtualType.CollectiveLand:
                        colleStation.Add(vp);
                        break;
                    case eVirtualType.SecondTable:
                        tableStation.Add(vp);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Add(添加承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加承包方出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 是否可继续
        /// </summary>
        private bool CanContinue()
        {
            if (landStation == null && woodStation == null && yardStation == null &&
            houseStation == null && colleStation == null && tableStation == null)
            {
                this.ReportError("尚未初始化承包方表的访问接口");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新承包方数据
        /// </summary>
        public bool Update(VirtualPerson vp)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        result = (landStation.Update(vp) == 1) ? true : false;
                        break;
                    case eVirtualType.Yard:
                        result = (yardStation.Update(vp) == 1) ? true : false;
                        break;
                    case eVirtualType.House:
                        result = (houseStation.Update(vp) == 1) ? true : false;
                        break;
                    case eVirtualType.Wood:
                        result = (woodStation.Update(vp) == 1) ? true : false;
                        break;
                    case eVirtualType.CollectiveLand:
                        result = (colleStation.Update(vp) == 1) ? true : false;
                        break;
                    case eVirtualType.SecondTable:
                        result = (tableStation.Update(vp) == 1) ? true : false;
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateZone(更新承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("更新承包方出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 删除承包方数据
        /// </summary>
        public bool Delete(Guid id)
        {
            if (!CanContinue())
            {
                return false;
            }
            bool result = true;
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        result = (landStation.Delete(id) > 0) ? true : false;
                        break;
                    case eVirtualType.Yard:
                        result = (yardStation.Delete(id) > 0) ? true : false;
                        break;
                    case eVirtualType.House:
                        result = (houseStation.Delete(id) > 0) ? true : false;
                        break;
                    case eVirtualType.Wood:
                        result = (woodStation.Delete(id) > 0) ? true : false;
                        break;
                    case eVirtualType.CollectiveLand:
                        result = (colleStation.Delete(id) > 0) ? true : false;
                        break;
                    case eVirtualType.SecondTable:
                        result = (tableStation.Delete(id) > 0) ? true : false;
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包方出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 清除承包方数据
        /// </summary>
        public bool ClearZoneData(string zoneCode)
        {
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return false;
            }
            bool result = true;
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        result = (landStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                    case eVirtualType.Yard:
                        result = (yardStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                    case eVirtualType.House:
                        result = (houseStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                    case eVirtualType.Wood:
                        result = (woodStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                    case eVirtualType.CollectiveLand:
                        result = (colleStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                    case eVirtualType.SecondTable:
                        result = (tableStation.DeleteByZoneCode(zoneCode) > 0) ? true : false;
                        break;
                }
            }
            catch (NullReferenceException)
            {
                throw new Exception("数据库可能正在使用,无法完成清空操作！");
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除承包方数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包方出错," + ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据承包方状态获取指定的地域中以名称排序的“承包方”的集合
        /// </summary>
        /// <param name="code"></param>
        /// <param name="status"></param>
        /// <param name="levelOption"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetByZoneCode(string zoneCode, eVirtualPersonStatus status, eLevelOption levelOption)
        {
            List<VirtualPerson> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        list = landStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                    case eVirtualType.Yard:
                        list = yardStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                    case eVirtualType.House:
                        list = houseStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                    case eVirtualType.Wood:
                        list = woodStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                    case eVirtualType.CollectiveLand:
                        list = colleStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                    case eVirtualType.SecondTable:
                        list = tableStation.GetByZoneCode(zoneCode, status, levelOption);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据地域编码获取承包方
        /// </summary>
        public List<VirtualPerson> GetByZone(string zoneCode)
        {
            List<VirtualPerson> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            switch (virtualType)
            {
                case eVirtualType.Land:
                    list = landStation.GetByZoneCode(zoneCode);
                    break;
                case eVirtualType.Yard:
                    list = yardStation.GetByZoneCode(zoneCode);
                    break;
                case eVirtualType.House:
                    list = houseStation.GetByZoneCode(zoneCode);
                    break;
                case eVirtualType.Wood:
                    list = woodStation.GetByZoneCode(zoneCode);
                    break;
                case eVirtualType.CollectiveLand:
                    list = colleStation.GetByZoneCode(zoneCode);
                    break;
                case eVirtualType.SecondTable:
                    list = tableStation.GetByZoneCode(zoneCode);
                    break;
            }
            return list;
        }

        /// <summary>
        /// 根据地域编码获取二轮承包方
        /// </summary>
        public List<VirtualPerson> GetTableByZone(string zoneCode)
        {
            List<VirtualPerson> list = null;
            IVirtualPersonWorkStation<TableVirtualPerson> tbStation = dbContext == null ? null : dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
            if (tbStation == null || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = tbStation.GetByZoneCode(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetTableByZone(获取二轮承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取二轮承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 统计指定的地域中“承包方”的数量
        /// </summary>
        public int CountByZone(string zoneCode)
        {
            int count = -1;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return count;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        count = landStation.CountByZone(zoneCode);
                        break;
                    case eVirtualType.Yard:
                        count = yardStation.CountByZone(zoneCode);
                        break;
                    case eVirtualType.House:
                        count = houseStation.CountByZone(zoneCode);
                        break;
                    case eVirtualType.Wood:
                        count = woodStation.CountByZone(zoneCode);
                        break;
                    case eVirtualType.CollectiveLand:
                        count = colleStation.CountByZone(zoneCode);
                        break;
                    case eVirtualType.SecondTable:
                        count = tableStation.CountByZone(zoneCode);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Count(地域下的承包方)", ex.Message + ex.StackTrace);
                this.ReportError("统计承包权证失败" + ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 获取存在承包方数据的地域集合
        /// </summary>
        /// <param name="listZone">地域集合</param>
        public List<Zone> GetExsitZones(List<Zone> listZone)
        {
            List<Zone> zones = null;
            if (!CanContinue() || listZone == null || listZone.Count == 0)
            {
                return zones;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        zones = landStation.ExistZones(listZone);
                        break;
                    case eVirtualType.Yard:
                        zones = yardStation.ExistZones(listZone);
                        break;
                    case eVirtualType.House:
                        zones = houseStation.ExistZones(listZone);
                        break;
                    case eVirtualType.Wood:
                        zones = woodStation.ExistZones(listZone);
                        break;
                    case eVirtualType.CollectiveLand:
                        zones = colleStation.ExistZones(listZone);
                        break;
                    case eVirtualType.SecondTable:
                        zones = tableStation.ExistZones(listZone);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetExsitZones(获取存在承包方数据的地域集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取存在承包方数据的地域集合出错," + ex.Message);
            }
            return zones;
        }

        /// <summary>
        /// 地域下是否存在承包方
        /// </summary>
        public bool ExitInZone(string zoneCode)
        {
            bool result = false;
            if (!CanContinue())
            {
                return result;
            }
            try
            {
                switch (virtualType)
                {
                    case eVirtualType.Land:
                        result = string.IsNullOrEmpty(zoneCode) ? landStation.Any() : landStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                    case eVirtualType.Yard:
                        result = string.IsNullOrEmpty(zoneCode) ? yardStation.Any() : yardStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                    case eVirtualType.House:
                        result = string.IsNullOrEmpty(zoneCode) ? houseStation.Any() : houseStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                    case eVirtualType.Wood:
                        result = string.IsNullOrEmpty(zoneCode) ? woodStation.Any() : woodStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                    case eVirtualType.CollectiveLand:
                        result = string.IsNullOrEmpty(zoneCode) ? colleStation.Any() : colleStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                    case eVirtualType.SecondTable:
                        result = string.IsNullOrEmpty(zoneCode) ? tableStation.Any() : tableStation.Any(t => t.ZoneCode == zoneCode);
                        break;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExitInZone(地域下是否存在承包方)", ex.Message + ex.StackTrace);
                this.ReportError("查询地域下是否存在承包方出错," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 设置户主及对应的业务信息更新
        /// </summary>
        public bool SetMainPerson(VirtualPerson vp, Person p)
        {
            bool result = true;
            if (vp == null || p == null)
            {
                return false;
            }
            List<Person> list = vp.SharePersonList;
            Person selectPerson = list.Find(t => t.ID == p.ID);      //选中共有人            
            Person mainPerson = list.Find(t => t.Name == vp.Name);   //原户主  
            if (selectPerson == null || mainPerson == null)
            {
                return false;
            }

            //更新地块信息
            var vplandstation = dbContext.CreateContractLandWorkstation();
            //var vplands = vplandstation.GetCollection(vp.ID);

            //foreach (var item in vplands)
            //{
            //    item.OwnerName = selectPerson.Name;
            //}
            //vplandstation.UpdateRange(vplands);

            //var concordStation = dbContext.CreateConcordStation();
            //var concords = concordStation.GetContractsByFamilyID(vp.ID);
            //foreach (var item in concords)
            //{
            //    item.ContracterName = selectPerson.Name;
            //    item.ContracterIdentifyNumber = selectPerson.ICN;
            //    concordStation.Update(item);
            //}

            selectPerson.ID = vp.ID;
            selectPerson.Relationship = "户主";
            selectPerson.IsSharedLand = "是";
            mainPerson.Relationship = "";
            mainPerson.ID = Guid.NewGuid();

            vp.Name = selectPerson.Name;
            vp.Number = selectPerson.ICN;
            vp.CardType = selectPerson.CardType;
            var upvp = vp.Clone() as VirtualPerson;
            //排序
            vp.SharePersonList = SortSharePerson(list, vp.Name);

            //upvp.SharePersonList = SortSharePerson(list, vp.Name);
            try
            {
                vplandstation.UpdateRelationData(vp.ID, selectPerson.Name, selectPerson.ICN, vp);
                //    switch (virtualType)
                //    {
                //        case eVirtualType.Land:
                //            landStation.Update(vp);
                //            break;
                //        case eVirtualType.Yard:
                //            yardStation.Update(vp);
                //            break;
                //        case eVirtualType.House:
                //            houseStation.Update(vp);
                //            break;
                //        case eVirtualType.Wood:
                //            woodStation.Update(vp);
                //            break;
                //        case eVirtualType.CollectiveLand:
                //            colleStation.Update(vp);
                //            break;
                //        case eVirtualType.SecondTable:
                //            tableStation.Update(vp);
                //            break;
                //   }

            }
            catch (Exception ex)
            {
                result = false;
                YuLinTu.Library.Log.Log.WriteException(this, "SetMainPerson(设置户主)", ex.Message + ex.StackTrace);
                this.ReportError("设置户主," + ex.Message);
            }
            return result;
        }

        #endregion

        #region 导入导出

        /// <summary>
        /// 导入数据
        /// </summary>
        public bool ImportData(Zone zone, string fileName, bool isStockRight)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportProgress(100, null);
                this.ReportWarn("未获取" + zone.Name + "下承包方调查表,或选择表格路径不正确,请检查再执行导入操作!");
                return false;
            }
            bool isSuccess = false;
            try
            {
                isErrorRecord = false;
                canImport = false;
                errorMesssage = "";
                using (ImportContractorSurveyTable familyImport = new ImportContractorSurveyTable())
                {
                    string zoneNameDesc = GetMarkDesc(zone);
                    string tabelName = Path.GetFileNameWithoutExtension(fileName);
                    familyImport.ProgressChanged += ReportPercent;
                    familyImport.Alert += (s, e) =>
                    {
                        //string msg = tabelName + e.Description;
                        string msg = e.Description;
                        this.ReportAlert(e.Grade, e.UserState, msg);
                        if (e.Grade == eMessageGrade.Error)
                            isErrorRecord = true;
                    };
                    familyImport.DataInstance = this.dbContext;
                    familyImport.CurrentZone = zone;
                    familyImport.TableType = 1;
                    familyImport.VirtualType = virtualType;
                    familyImport.FamilyImportSet = FamilyImportSet;
                    familyImport.FamilyOtherSet = FamilyOtherSet;
                    familyImport.ZoneDesc = zoneNameDesc;
                    familyImport.TableName = tabelName;
                    this.ReportProgress(1, "开始读取数据");
                    familyImport.ReadContractorInformation(fileName, isStockRight);
                    familyImport.InitalizeVirtualPersonInformatiion();
                    this.ReportProgress(5, "开始检查数据");
                    canImport = familyImport.VerifyCensusInfo();
                    if (canImport && !isErrorRecord)
                    {
                        this.ReportProgress(10, "开始处理数据");
                        familyImport.Percent = 90;
                        familyImport.CurrentPercent = 10;
                        familyImport.ImprotEntitys();
                        this.ReportProgress(100, "完成");
                        isSuccess = true;
                    }
                    familyImport.Disponse();
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导入承包方调查表数据失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ImportData(导入承包方数据)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量导入承包方调查表
        /// </summary>
        /// <param name="zone">行政区域</param>
        /// <param name="fileName">文件夹路径</param>
        /// <param name="isClear">是否清空</param>
        public void VolumnImport(Zone zone, string fileName, bool isClear)
        {
            var tableFiles = Directory.GetFiles(fileName);   //获得文件夹下的所有表格文件
            if (tableFiles.Count() == 0)
            {
                return;
            }
            this.ReportProgress(1, "开始批量导入");
            List<Zone> zones = GetChildZone(zone);     //获取当期村级地域下的所有子级组地域集合  
            int count = 0;   //计数器
            int index = 0;
            double percent = 99 / (double)tableFiles.Count();
            foreach (var tableFile in tableFiles)
            {
                foreach (var zoneItem in zones)
                {
                    string zoneName = zoneItem.FullName;
                    string tableName = Path.GetFileNameWithoutExtension(tableFile);
                    if (zoneName.Contains(tableName))
                    {
                        double percentCount = 1 + index * percent;
                        this.ReportProgress((int)percentCount, tableName);
                        bool isSuccess = true;   //ImportData(zoneItem, tableFile, isClear, false, percent, percentCount);
                        index++;
                        if (isSuccess)
                        {
                            count++;
                        }
                    }
                }
            }
            this.ReportProgress(100, null);
            this.ReportInfomation(string.Format("导入成功{0}个表格,失败{1}个表格", count, tableFiles.Count() - count));
        }

        /// <summary>
        /// 导出数据到Excel表
        /// </summary>
        public void ExportDataExcel(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                List<VirtualPerson> tbVps = GetTableByZone(zone.FullCode);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.VirtualPersonSurveyExcel);
                string zoneName = GetUinitName(zone);
                var ownerIds = new List<Guid>();
                vps.ForEach(c => ownerIds.Add(c.ID));
                var landList = contractLandStation.GetCollection(ownerIds);
                using (ExportContractorExcel export = new ExportContractorExcel())
                {
                    export.SaveFilePath = fileName + @"\" + excelName + TemplateFile.VirtualPersonSurveyExcel + ".xls";
                    export.CurrentZone = zone;
                    export.FamilyList = vps;
                    export.TableFamilyList = tbVps;
                    export.LandList = landList;
                    export.UnitName = zoneName;
                    export.Percent = averagePercent;
                    export.CurrentPercent = currentPercent;
                    export.ZoneDesc = excelName;
                    export.PersonType = VirtualType;
                    //export.TemplateFile = tempPath;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = export.BeginExcel(tempPath);
                }
                vps = null;
                tbVps = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Word
        /// </summary>
        /// <param name="zone">当前导出地域</param>
        /// <param name="listPerson">当前导出承包方列表</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="averagePercent">每个地域所占平均百分比</param>
        /// <param name="percent">当前百分比进度</param>
        public void ExportDataWord(Zone zone, List<VirtualPerson> listPerson, string filePath, CollectivityTissue sender, double averagePercent = 0.0, double percent = 0.0, bool isShow = false)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string info = string.Format("{0}导出{1}张承包方Word调查表", markDesc, listPerson.Count);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                string zoneName = GetUinitName(zone);
                int index = 1;
                double vpPercent = averagePercent / (double)listPerson.Count;   //每个承包方所占的百分比
                foreach (VirtualPerson family in listPerson)
                {
                    //是否显示集体户信息
                    if (FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }
                    ExportContractorTable export = new ExportContractorTable();
                    export.MarkDesc = markDesc;
                    export.OpenTemplate(tempPath);
                    export.CurrentZone = zone;
                    export.Tissue = sender;
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    export.SaveAs(family, filePath + @"\" + familyNuber + "-" + family.Name + "-农村土地承包经营权承包方调查表.doc");
                    this.ReportProgress((int)(percent + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                if (isShow)
                {
                    this.ReportInfomation(info);
                    this.ReportProgress(100, "完成");
                }
                listPerson = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出单个数据到Word
        /// </summary>
        public void ExportDataWord(Zone zone, VirtualPerson vp, string MarkDesc, string ConcordNumber, CollectivityTissue sender, List<Dictionary> DictList)
        {
            try
            {
                if (vp == null)
                {
                    return;
                }
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                string zoneName = GetUinitName(zone);
                ExportContractorTable export = new ExportContractorTable();

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorTable)
                        export = (ExportContractorTable)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion
                export.ZoneList = dbContext.CreateZoneWorkStation().Get();
                export.MarkDesc = MarkDesc;
                export.CurrentZone = zone;
                export.DictList = DictList;
                export.Tissue = sender;
                export.ConcordNumber = ConcordNumber;
                export.OpenTemplate(tempPath);
                export.PrintPreview(vp, SystemSet.DefaultPath + @"\" + vp.Number + "-" + vp.Name + "-农村土地承包经营权承包方调查表");
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出户主声明书处理
        /// </summary>
        /// <param name="zone">当前导出地域</param>
        /// <param name="listPerson">当前导出承包方列表</param>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="time">时间</param>
        /// <param name="averagePercent">每个地域所占平均百分比</param>
        /// <param name="percent">当前百分比进度</param>   
        public void ExportApplyBook(Zone zone, List<VirtualPerson> listPerson, string fileName, DateTime? time,
            double averagePercent = 0.0, double percent = 0.0, bool isShow = false)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string info = string.Format("{0}导出{1}张户主声明书", markDesc, listPerson.Count);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonApplyBook);
                string zoneName = GetUinitName(zone);
                int index = 1;
                var zonelist = GetParentZone(zone);
                double vpPercent = averagePercent / (double)listPerson.Count;
                foreach (VirtualPerson family in listPerson)
                {
                    //确定是否导出集体户信息(利用配置文件)
                    if (FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    ExportApplyBook exportFamily = new ExportApplyBook(family);

                    #region 通过反射等机制定制化具体的业务处理类
                    var temp = WorksheetConfigHelper.GetInstance(exportFamily);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportApplyBook)
                            exportFamily = (ExportApplyBook)temp;
                        templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }
                    #endregion

                    exportFamily.Date = time;       //获取日期
                    exportFamily.ZoneName = GetUinitName(zone);
                    exportFamily.CurrentZone = zone;
                    exportFamily.ZoneList = zonelist;
                    exportFamily.OpenTemplate(templatePath);
                    exportFamily.RightName = InitalizeRightType();
                    exportFamily.SaveAs(family, fileName + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.VirtualPersonApplyBook + ".doc");
                    this.ReportProgress((int)(percent + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                if (isShow)
                {
                    this.ReportInfomation(info);
                    this.ReportProgress(100, "完成");
                }
                listPerson = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ApplyBookWord(户主声明书处理)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 户主声明书处理
        /// </summary>
        public bool ApplyBookWord(Zone zone, VirtualPerson vp, DateTime? time, bool save = false, string savePath = "")
        {
            bool isSuccess = true;   //导出(预览)是否成功
            try
            {
                string familyNuber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonApplyBook);
                var zonelist = GetParentZone(zone);
                ExportApplyBook exportFamily = new ExportApplyBook(vp);

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(exportFamily);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportApplyBook)
                        exportFamily = (ExportApplyBook)temp;
                    templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                exportFamily.Date = time;
                exportFamily.ZoneName = GetUinitName(zone);
                exportFamily.CurrentZone = zone;
                exportFamily.ZoneList = zonelist;
                exportFamily.OpenTemplate(templatePath);
                exportFamily.RightName = InitalizeRightType();
                if (!save)
                {
                    exportFamily.PrintPreview(vp, SystemSet.DefaultPath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonApplyBook);
                }
                else
                {
                    exportFamily.SaveAs(vp, savePath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonApplyBook + ".doc");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ApplyBookWord(户主声明书处理)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 导出委托声明书
        /// </summary>
        /// <param name="zone">当前导出地域</param>
        /// <param name="listPerson">当前导出承包方列表</param>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="time">时间</param>
        /// <param name="averagePercent">每个地域所占平均百分比</param>
        /// <param name="percent">当前百分比进度</param> 
        public void ExportDelegateBook(Zone zone, List<VirtualPerson> listPerson, string fileName, DateTime? time,
            double averagePercent = 0.0, double percent = 0.0, bool isShow = false)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string info = string.Format("{0}导出{1}张委托声明书", markDesc, listPerson.Count);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonDelegateBook);
                string zoneName = GetUinitName(zone);
                List<Zone> zones = GetParentZone(zone);
                int index = 1;
                double vpPercent = averagePercent / (double)listPerson.Count;
                foreach (VirtualPerson family in listPerson)
                {
                    //是否显示集体户信息
                    if (FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }

                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    ExportDelegateBook exportFamily = new ExportDelegateBook(family);
                    exportFamily.DateValue = time;
                    exportFamily.ZoneList = zones;
                    exportFamily.CurrentZone = zone;
                    exportFamily.OpenTemplate(templatePath);
                    exportFamily.RightName = InitalizeRightType();
                    exportFamily.SaveAs(family, fileName + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.VirtualPersonDelegateBook + ".doc");
                    this.ReportProgress((int)(percent + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                if (isShow)
                {
                    this.ReportInfomation(info);
                    this.ReportProgress(100, "完成");
                }
                listPerson = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDelegateBook(导出委托声明书)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 委托声明书处理
        /// </summary>
        public bool DelegateBookWord(Zone zone, VirtualPerson vp, DateTime? time, bool save = false, string savePath = "")
        {
            bool isSuccess = true;
            try
            {
                string familyNuber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonDelegateBook);
                ExportDelegateBook exportFamily = new ExportDelegateBook(vp);
                exportFamily.DateValue = time;
                exportFamily.ZoneList = GetParentZone(zone);
                exportFamily.CurrentZone = zone;
                exportFamily.OpenTemplate(templatePath);
                exportFamily.RightName = InitalizeRightType();
                if (!save)
                {
                    exportFamily.PrintPreview(vp, SystemSet.DefaultPath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonDelegateBook);
                }
                else
                {
                    exportFamily.SaveAs(vp, savePath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonDelegateBook + ".doc");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "DelegateBookWord(委托声明书处理)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 导出无异议声明书
        /// </summary>
        public void ExportIdeaBook(Zone zone, List<VirtualPerson> listPerson, string fileName, DateTime? time, DateTime? pubTime,
            double averagePercent = 0.0, double percent = 0.0, bool isShow = false)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string info = string.Format("{0}导出{1}张无异议声明书", markDesc, listPerson.Count);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonIdeaBook);
                string zoneName = GetUinitName(zone);
                List<Zone> zones = GetParentZone(zone);
                int index = 1;
                double vpPercent = averagePercent / (double)listPerson.Count;
                foreach (VirtualPerson family in listPerson)
                {
                    //是否显示集体户信息
                    if (FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    ExportIdeaBook exportDelegate = new ExportIdeaBook(family);
                    exportDelegate.Date = time;
                    exportDelegate.PubDate = pubTime;
                    exportDelegate.ZoneName = zoneName;
                    exportDelegate.RightName = InitalizeRightType();
                    exportDelegate.LandAliseName = InitalizeLandRightName();
                    exportDelegate.OpenTemplate(templatePath);
                    exportDelegate.SaveAs(family, fileName + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.VirtualPersonIdeaBook + ".doc");
                    this.ReportProgress((int)(percent + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                if (isShow)
                {
                    this.ReportInfomation(info);
                    this.ReportProgress(100, "完成");
                }
                listPerson = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportIdeaBook(导出无异议声明书)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 无异议声明书处理
        /// </summary>
        public bool IdeaBookWord(Zone zone, VirtualPerson vp, DateTime? time, DateTime? pubTime, bool save = false, string savePath = "")
        {
            bool isSuccess = true;
            try
            {
                string familyNuber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonIdeaBook);
                ExportIdeaBook exportDelegate = new ExportIdeaBook(vp);
                exportDelegate.ZoneList = dbContext.CreateZoneWorkStation().Get();
                exportDelegate.Date = time;       //声明日期
                exportDelegate.PubDate = pubTime;  //公示日期
                exportDelegate.CurrentZone = zone;
                exportDelegate.ZoneName = GetUinitName(zone);
                exportDelegate.RightName = InitalizeRightType();
                exportDelegate.LandAliseName = InitalizeLandRightName();
                exportDelegate.OpenTemplate(templatePath);
                if (!save)
                {
                    exportDelegate.PrintPreview(vp, SystemSet.DefaultPath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonIdeaBook);
                }
                else
                {
                    exportDelegate.SaveAs(vp, savePath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonIdeaBook + ".doc");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "IdeaBookWord(无异议声明书处理)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 导出测绘申请书
        /// </summary>
        public void ExportSurveyBook(Zone zone, List<VirtualPerson> listPerson, string fileName, DateTime? time,
             double averagePercent = 0.0, double percent = 0.0, bool isShow = false)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string info = string.Format("{0}导出{1}张测绘申请书", markDesc, listPerson.Count);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyBook);
                List<Zone> zonelist = GetParentZone(zone);
                int index = 1;
                double vpPercent = averagePercent / (double)listPerson.Count;
                foreach (VirtualPerson family in listPerson)
                {
                    if (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0)
                    {
                        continue;
                    }
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    ExportSurveyBook export = new ExportSurveyBook();
                    export.Date = time;
                    export.VirtualPerson = family;
                    export.CurrentZone = zone;
                    export.ZoneList = zonelist;
                    export.OpenTemplate(templatePath);
                    export.SaveAs(family, fileName + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.VirtualPersonSurveyBook + ".doc");
                    this.ReportProgress((int)(percent + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                if (isShow)
                {
                    this.ReportInfomation(info);
                    this.ReportProgress(100, "完成");
                }
                listPerson = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportIdeaBook(导出无异议声明书)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 测绘申请书处理
        /// </summary>
        public bool SurveyBookWord(Zone zone, VirtualPerson vp, DateTime? time, bool save = false, string savePath = "")
        {
            bool isSuccess = true;
            try
            {
                List<Zone> zonelist = GetParentZone(zone);
                string familyNuber = ToolString.ExceptSpaceString(vp.FamilyNumber);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyBook);
                ExportSurveyBook export = new ExportSurveyBook();
                export.Date = time;
                export.VirtualPerson = vp;
                export.CurrentZone = zone;
                export.ZoneList = zonelist;
                export.OpenTemplate(templatePath);
                if (!save)
                {
                    export.PrintPreview(vp, SystemSet.DefaultPath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonSurveyBook);
                }
                else
                {
                    export.SaveAs(vp, savePath + @"\" + familyNuber + "-" + vp.Name + "-" + TemplateFile.VirtualPersonSurveyBook + ".doc");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "SurveyBookWord(测绘申请书处理)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        #endregion

        #region 工具

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <param name="ListPerson">待修改的承包方集合(已经根据设置决定是否去除集体户信息)</param>
        /// <param name="currentZone">当前地域</param>
        public void InitialVirtualPersonInfo(TaskInitialVirtualPersonArgument argument, List<VirtualPerson> listPersons, Zone currentZone)
        {
            try
            {
                string markDesc = GetMarkDesc(currentZone);
                if (dbContext == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                if (listPersons == null || listPersons.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方信息", markDesc));
                    return;
                }
                this.ReportProgress(1, "开始");
                var farmPersons = listPersons.FindAll(c => c.FamilyExpand.ContractorType == eContractorType.Farmer && c.Status != eVirtualPersonStatus.Lock);
                var personalPersons = listPersons.FindAll(c => c.FamilyExpand.ContractorType == eContractorType.Personal && c.Status != eVirtualPersonStatus.Lock);
                var unitPersons = listPersons.FindAll(c => c.FamilyExpand.ContractorType == eContractorType.Unit && c.Status != eVirtualPersonStatus.Lock);
                double vpPercent = 99 / (double)listPersons.Count;
                int currentPercent = 1;
                int lockPersonCount = listPersons.Count(c => c.Status == eVirtualPersonStatus.Lock);  //统计锁定的人
                var landStation = dbContext.CreateContractLandWorkstation();   //注意:业务统一到了承包台账里面

                dbContext.BeginTransaction();
                int farmSuccessCount = SetPersonValue(farmPersons, argument, landStation, argument.FarmerFamilyNumberIndex, 1, 0, vpPercent, currentPercent, markDesc);   //农户
                int personaSuccessCount = SetPersonValue(personalPersons, argument, landStation, argument.PersonalFamilyNumberIndex, 8001, farmPersons.Count, vpPercent, currentPercent, markDesc);  //个人
                int unitSuccessCount = SetPersonValue(unitPersons, argument, landStation, argument.UnitFamilyNumberIndex, 9001, listPersons.Count - unitPersons.Count, vpPercent, currentPercent, markDesc);    //单位
                dbContext.CommitTransaction();

                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("{0}共{1}个承包方,其中{2}个承包方被锁定,成功初始化{3}个承包方信息", markDesc, listPersons.Count,
                  lockPersonCount, farmSuccessCount + personaSuccessCount + unitSuccessCount));

                farmPersons.Clear();
                personalPersons.Clear();
                unitPersons.Clear();
                farmPersons = null;
                personalPersons = null;
                unitPersons = null;
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError("初始化承包方基本信息失败!");
                YuLinTu.Library.Log.Log.WriteError(this, "InitialVirtualPersonInfo(提交更新数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 设置承包方值
        /// </summary>
        /// <param name="vps">承包方集合（农户、个人、单位）</param>
        /// <param name="argument">任务参数</param>
        /// <param name="index">初始户号</param>
        /// <param name="formatnumber">已初始化数量</param>
        /// <param name="vpPercent">平均百分比</param>
        /// <param name="currentPercent">当前百分比</param>
        /// <param name="markDesc">任务描述信息</param>
        private int SetPersonValue(List<VirtualPerson> vps, TaskInitialVirtualPersonArgument argument, IContractLandWorkStation landStation,
            int[] familyIndex, int index, int formatnumber, double vpPercent = 0.0, double currentPercent = 0.0, string markDesc = "")
        {
            int successCount = 0;
            if (vps == null || vps.Count == 0 || landStation == null)
                return successCount;
            //vps.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
            bool isNULL = argument.InitialNull;
            foreach (VirtualPerson vpi in vps)
            {
                string Number = argument.CurrentZone.FullCode.PadRight(14, '0') + vpi.FamilyNumber.PadLeft(4, '0');
                if (vpi.FamilyExpand.ConstructMode == eConstructMode.Family)
                    Number += "J";
                else
                    Number += "Q";
                //20160929 陈泽林 不管什么状态统一初始化
                if (!isNULL || (isNULL && (vpi.FamilyNumber == null || vpi.FamilyNumber.Trim() == string.Empty)))
                {
                    if (argument.InitiallNumber)
                    {
                        if (argument.VillageInlitialSet)
                            vpi.FamilyNumber = familyIndex[0].ToString().PadLeft(4, '0');
                        else
                            vpi.FamilyNumber = index.ToString().PadLeft(4, '0');
                    }
                }
                //if (vpi.Status == eVirtualPersonStatus.Lock)   //去除被锁定的承包方
                //{
                //    formatnumber++;
                //    //index++;
                //    this.ReportInfomation(string.Format("{0}被锁定,未初始化信息", vpi.Name));
                //    continue;
                //}
                if (vpi.Status == eVirtualPersonStatus.Right)
                {
                    if (!isNULL || (isNULL && (vpi.PostalNumber == null || vpi.PostalNumber.Trim() == string.Empty)))
                    {
                        if (argument.InitiallZip)
                        {
                            vpi.PostalNumber = argument.ZipCode;
                        }
                    }
                    if (!isNULL || (isNULL && (vpi.SharePersonList == null || vpi.SharePersonList.Count == 0)))
                    {
                        if (argument.InitiallNation)
                        {
                            List<Person> list = vpi.SharePersonList;
                            if (list != null)
                            {
                                list.ForEach(t => t.Nation = argument.CNation);
                            }
                            vpi.SharePersonList = list;
                        }
                        if (argument.InitiallSex)
                        {
                            List<Person> list = vpi.SharePersonList;
                            if (list != null)
                            {
                                list.ForEach(t =>
                                {
                                    if (!string.IsNullOrEmpty(t.ICN) && t.ICN.Length == 18)
                                    {
                                        t.Gender = ToolICN.GetAllGenderNoCheck(t.ICN);
                                    }
                                });
                            }
                            vpi.SharePersonList = list;
                        }

                    }
                    if (!isNULL || (isNULL && (vpi.Address == null || vpi.Address.Trim() == string.Empty)))
                    {
                        if (argument.InitiallVpAddress)
                            vpi.Address = argument.Address;
                    }
                    VirtualPersonExpand pand = vpi.FamilyExpand;
                    if (isNULL)
                    {
                        if (argument.InitiallSurveyPerson && (pand.SurveyPerson == null || pand.SurveyPerson.Trim().Equals("")))
                            pand.SurveyPerson = argument.Expand.SurveyPerson;
                        if (argument.InitiallSurveyDate && pand.SurveyDate == null)
                            pand.SurveyDate = argument.Expand.SurveyDate;
                        if (argument.InitiallSurveyAccount && (pand.SurveyChronicle == null || pand.SurveyChronicle.Trim().Equals("")))
                            pand.SurveyChronicle = argument.Expand.SurveyChronicle;
                        if (argument.InitiallPublishCheckPerson && (pand.PublicityCheckPerson == null || pand.PublicityCheckPerson.Trim().Equals("")))
                            pand.PublicityCheckPerson = argument.Expand.PublicityCheckPerson;
                        if (argument.InitiallcbPublishAccount && (pand.PublicityChronicle == null || pand.PublicityChronicle.Trim().Equals("")))
                            pand.PublicityChronicle = argument.Expand.PublicityChronicle;
                        if (argument.InitiallPublishAccountPerson && (pand.PublicityChroniclePerson == null || pand.PublicityChroniclePerson.Trim().Equals("")))
                            pand.PublicityChroniclePerson = argument.Expand.PublicityChroniclePerson;
                        if (argument.InitiallPublishDate && pand.PublicityDate == null)
                            pand.PublicityDate = argument.Expand.PublicityDate;
                        if (argument.InitiallCheckDate && pand.CheckDate == null)
                            pand.CheckDate = argument.Expand.CheckDate;
                        if (argument.InitiallCheckPerson && (pand.CheckPerson == null || pand.CheckPerson.Trim().Equals("")))
                            pand.CheckPerson = argument.Expand.CheckPerson;
                        if (argument.InitiallCheckOpinion && (pand.CheckOpinion == null || pand.CheckOpinion.Trim().Equals("")))
                            pand.CheckOpinion = argument.Expand.CheckOpinion;
                        if (argument.InitialContractWay && (pand.ConstructMode == eConstructMode.Other))
                            pand.ConstructMode = argument.Expand.ConstructMode;
                        if (argument.InitStartTime && (pand.ConcordStartTime == null))
                            pand.ConcordStartTime = argument.Expand.ConcordStartTime;
                        if (argument.InitEndTime && (pand.ConcordEndTime == null))
                            pand.ConcordEndTime = argument.Expand.ConcordEndTime;
                        if (argument.InitConcordNumber && (pand.ConcordNumber == null || pand.ConcordNumber == ""))
                            pand.ConcordNumber = Number;
                        if (argument.InitWarrentNumber && (pand.WarrantNumber == null || pand.WarrantNumber == ""))
                            pand.WarrantNumber = Number;
                        if (argument.InitPersonComment && vpi.SharePersonList != null && vpi.SharePersonList.Count > 0)
                        {
                            List<Person> listPerson = vpi.SharePersonList;
                            listPerson.Where(person => person.Comment.IsNullOrEmpty()).ForEach(person => person.Comment = argument.PersonComment);
                            vpi.SharePersonList = listPerson;
                        }
                    }
                    else
                    {
                        if (argument.InitPersonComment && vpi.SharePersonList != null && vpi.SharePersonList.Count > 0)
                        {
                            List<Person> listPerson = vpi.SharePersonList;
                            listPerson.ForEach(person => person.Comment = argument.PersonComment);
                            vpi.SharePersonList = listPerson;
                        }
                        if (argument.InitiallSurveyPerson)
                            pand.SurveyPerson = argument.Expand.SurveyPerson;
                        if (argument.InitiallSurveyDate)
                            pand.SurveyDate = argument.Expand.SurveyDate;
                        if (argument.InitiallSurveyAccount)
                            pand.SurveyChronicle = argument.Expand.SurveyChronicle;
                        if (argument.InitiallPublishCheckPerson)
                            pand.PublicityCheckPerson = argument.Expand.PublicityCheckPerson;
                        if (argument.InitiallcbPublishAccount)
                            pand.PublicityChronicle = argument.Expand.PublicityChronicle;
                        if (argument.InitiallPublishAccountPerson)
                            pand.PublicityChroniclePerson = argument.Expand.PublicityChroniclePerson;
                        if (argument.InitiallPublishDate)
                            pand.PublicityDate = argument.Expand.PublicityDate;
                        if (argument.InitiallCheckDate)
                            pand.CheckDate = argument.Expand.CheckDate;
                        if (argument.InitiallCheckPerson)
                            pand.CheckPerson = argument.Expand.CheckPerson;
                        if (argument.InitiallCheckOpinion)
                            pand.CheckOpinion = argument.Expand.CheckOpinion;
                        if (argument.InitialContractWay)
                            pand.ConstructMode = argument.Expand.ConstructMode;
                        if (argument.InitStartTime)
                            pand.ConcordStartTime = argument.Expand.ConcordStartTime;
                        if (argument.InitEndTime)
                            pand.ConcordEndTime = argument.Expand.ConcordEndTime;
                        if (argument.InitConcordNumber)
                            pand.ConcordNumber = Number;
                        if (argument.InitWarrentNumber)
                            pand.WarrantNumber = Number;
                    }
                    vpi.FamilyExpand = pand;
                }

                if (argument.InitSharePersonComment)
                {
                    InitSharePersonComment(vpi);
                }
                //bool isSuccess = Update(vpi);
                int upCount = landStation.UpdateDataForInitialVirtualPerson(vpi);
                this.ReportProgress((int)(currentPercent + vpPercent * formatnumber), string.Format("{0}", markDesc + vpi.Name));
                if (upCount > 0)
                {
                    successCount++;
                    formatnumber++;
                    index++;
                    familyIndex[0]++;
                }
            }
            return successCount;
        }


        /// <summary>
        /// 后期添加的初始化共有人备注信息（从Description赋值到Comment。）
        /// </summary>
        private void InitSharePersonComment(VirtualPerson vpi)
        {
            foreach (var person in vpi.SharePersonList)
            {
                person.Comment = person.Description;
            }
            //设置sharePerson序列化字符串，并且通知界面
            vpi.SharePersonList = vpi.SharePersonList;
        }

        #endregion

        #endregion

        #region 辅助方法

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        public string GetMarkDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 初始化权属地块类型
        /// </summary>
        private string InitalizeLandRightName()
        {
            string templateName = "承包地田块";
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    templateName = "承包地田块";
                    break;
                case eVirtualType.Yard:
                    templateName = "建设用地";
                    break;
                case eVirtualType.House:
                    templateName = "房屋";
                    break;
                case eVirtualType.Wood:
                    templateName = "林地";
                    break;
                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 初始化权属类型
        /// </summary>
        private string InitalizeRightType()
        {
            string templateName = "农村土地承包经营权";
            switch (VirtualType)
            {
                case eVirtualType.Land:
                    templateName = "农村土地承包经营权";
                    break;
                case eVirtualType.Yard:
                    templateName = "集体建设用地使用权";
                    break;
                case eVirtualType.House:
                    templateName = "房屋所有权";
                    break;
                case eVirtualType.Wood:
                    templateName = "林权";
                    break;
                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public FamilyOutputDefine InitalizeFamilyDefine()
        {
            FamilyOutputDefine define = null;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + "FamilyOutputDefine.xml";
            if (File.Exists(filePath))
            {
                define = ToolSerialization.DeserializeXml(filePath, typeof(FamilyOutputDefine)) as FamilyOutputDefine;
            }
            if (define == null)
            {
                define = new FamilyOutputDefine();
            }
            if (!define.ContractorTypeValue)
            {
                define.ColumnCount -= 1;
            }
            return define;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取地域下所有子级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public List<Zone> GetChildZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_CHILDREN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取发包方
        /// </summary>
        public CollectivityTissue GetTissue(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = this.dbContext;
            arg.Name = SenderMessage.SENDER_GET_ID;
            TheBns.Current.Message.Send(this, arg);
            CollectivityTissue tissue = arg.ReturnValue as CollectivityTissue;
            return tissue;
        }

        /// <summary>
        ///  错误信息报告
        /// </summary>
        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }

        /// <summary>
        /// 错误信息报告
        /// </summary>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                errorMesssage += e.Description;
                this.ReportAlert(e.Grade, e.UserState, e.Description);
                if (e.Grade == eMessageGrade.Error)
                    isErrorRecord = true;
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
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
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        /// 对共有人排序(户主最前面)
        /// </summary>
        public List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            Person p = personCollection.Find(t => t.Name == houseName);
            if (p != null)
            {
                if (personCollection.Count > 1)
                {
                    p.Relationship = "户主";
                }
                else
                    p.Relationship = "本人";
                p.IsSharedLand = "是";
                sharePersonCollection.Add(p);
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        #endregion




    }
}
