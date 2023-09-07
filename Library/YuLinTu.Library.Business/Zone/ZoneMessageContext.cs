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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地域模块消息
    /// </summary>
    public class ZoneMessageContext : WorkstationContextBase
    {
        #region Fildes

        #endregion

        #region Methods

        /// <summary>
        /// 添加地域数据
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_ADD)]
        private void OnAddZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                ZoneDataBusiness business = CreateBusiness(db);
                e.ReturnValue = business.Add(e.Parameter as Zone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnAddZone(添加地域数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 更新地域数据
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_UPDATE)]
        private void OnUpdateZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone curZone = e.Parameter as Zone;
                IDbContext db = e.Datasource;
                ZoneDataBusiness business = CreateBusiness(db);
                e.ReturnValue = business.UpdateZone(curZone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnUpdateZone(更新地域数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除地域数据
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_DELETE)]
        private void OnDeleteZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                ZoneDataBusiness business = CreateBusiness(db);
                Zone zone = e.Parameter as Zone;
                if (zone != null)
                {
                    e.ReturnValue = business.UpdateZone(zone);
                }
                else
                {
                    e.ReturnValue = false;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteZone(删除地域数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 清除地域数据
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_CLEAR)]
        private void OnClearZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                ZoneDataBusiness business = CreateBusiness(db);
                e.ReturnValue = business.ClearZone();
            }
            catch (NullReferenceException)
            {
                throw new Exception("数据库可能正在使用,无法完成清空操作！");
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 导出地域压缩包
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_EXPORTPACKAGE)]
        private void OnExportZonePackage(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导出地域图斑
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_EXPORTSHAPE)]
        private void OnExportZoneShape(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导出地域调查表
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_EXPORTTABLE)]
        private void OnExportZoneTable(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_GETALLDATA)]
        private void OnGetAllZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                ZoneDataBusiness business = CreateBusiness(db);
                e.ReturnValue = business.GetAllZone();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGetAllZone(获取所有数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 根据地域编码获取地域集合
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_GETDATA)]
        private void OnZonesByCode(object sender, ModuleMsgArgs e)
        {
            try
            {
                string parameter = e.Parameter.ToString();
                if (string.IsNullOrEmpty(parameter))
                {
                    e.ReturnValue = null;
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    e.ReturnValue = business.ZonesByCode(parameter);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnZonesByCode(获取地域数据集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取指定地域编码的地域
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_GET)]
        private void OnZoneByCode(object sender, ModuleMsgArgs e)
        {
            try
            {
                string parameter = e.Parameter.ToString();
                if (string.IsNullOrEmpty(parameter))
                {
                    e.ReturnValue = null;
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    e.ReturnValue = business.ZoneByCode(parameter);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnZonesByCode(获取地域数据集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导入地域图斑
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_IMPORTSHAPE)]
        private void OnImportZoneShape(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导入地域调查表
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_IMPORTTABLE)]
        private void OnImportZoneTable(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 创建业务逻辑
        /// </summary>
        private ZoneDataBusiness CreateBusiness(IDbContext db)
        {
            ZoneDataBusiness business = new ZoneDataBusiness();
            business.DbContext = db;
            business.Station = db.CreateZoneWorkStation();
            return business;
        }

        /// <summary>
        /// 获取父级地域集合(县级)
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_PARENTS_ZONE)]
        private void VirtualGetParent(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                if (zone == null || (zone != null && zone.Level >= eZoneLevel.State))
                {
                    e.ReturnValue = null;
                    return;
                }
                IDbContext db = e.Datasource;
                List<Zone> zoneList = new List<Zone>();
                ZoneDataBusiness business = CreateBusiness(db);
                Zone temZone = business.Get(zone.UpLevelCode);
                zoneList.Add(temZone);
                while (temZone.Level < eZoneLevel.State)
                {
                    temZone = business.Get(temZone.UpLevelCode);
                    zoneList.Add(temZone);
                }
                e.ReturnValue = zoneList;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetParent(获取父级地域集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取父级地域集合(省级)
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE)]
        private void VirtualGetToProvinceParent(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                if (zone == null || (zone != null && zone.Level >= eZoneLevel.County))
                {
                    e.ReturnValue = null;
                    return;
                }
                IDbContext db = e.Datasource;
                List<Zone> zoneList = new List<Zone>();
                ZoneDataBusiness business = CreateBusiness(db);
                Zone temZone = business.Get(zone.UpLevelCode);
                zoneList.Add(temZone);
                while (temZone.Level <= eZoneLevel.Province)
                {
                    temZone = business.Get(temZone.UpLevelCode);
                    zoneList.Add(temZone);
                }
                e.ReturnValue = zoneList;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetParent(获取父级地域集合)", ex.Message + ex.StackTrace);
            }
        }


        /// <summary>
        /// 获取父级地域集合(镇级)
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_PARENTSTOTOWN_ZONE)]
        private void VirtualGetParentsToTown(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                if (zone == null || (zone != null && zone.Level >= eZoneLevel.Town))
                {
                    e.ReturnValue = null;
                    return;
                }
                IDbContext db = e.Datasource;
                List<Zone> zoneList = new List<Zone>();
                ZoneDataBusiness business = CreateBusiness(db);
                Zone temZone = business.Get(zone.UpLevelCode);
                zoneList.Add(temZone);
                while (temZone.Level < eZoneLevel.Town)
                {
                    temZone = business.Get(temZone.UpLevelCode);
                    zoneList.Add(temZone);
                }
                e.ReturnValue = zoneList;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetParent(获取父级地域集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取子级地域集合
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_CHILDREN_ZONE)]
        private void VirtualGetChildren(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                IDbContext dbContext = e.Datasource;
                ZoneDataBusiness zoneBusiness = CreateBusiness(dbContext);
                List<Zone> zoneAll = zoneBusiness.GetAllZone();
                List<Zone> zonesGroup = zoneAll.FindAll(c => c.UpLevelCode == zone.FullCode);
                e.ReturnValue = zonesGroup;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetChildren(获取子级地域集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取子级地域集合
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_ALLCHILDREN_ZONE)]
        private void VirtualGetAllChildren(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                IDbContext dbContext = e.Datasource;
                ZoneDataBusiness zoneBusiness = CreateBusiness(dbContext);
                List<Zone> zoneAll = zoneBusiness.ZonesByCode(zone.FullCode);
                e.ReturnValue = zoneAll;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetAllChildren(获取子级地域集合)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取当前地域之上级地域
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_PARENT_ZONE)]
        public void GetParent(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                IDbContext dbContext = e.Datasource;
                ZoneDataBusiness zoneBusiness = CreateBusiness(dbContext);
                Zone parent = zoneBusiness.Get(zone.UpLevelCode);
                e.ReturnValue = parent;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取上级地域)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_ZONENAME)]
        private void VirtualGetZoneName(object sender, ModuleMsgArgs e)
        {
            try
            {
                string parameter = e.Parameter as string;
                if (string.IsNullOrEmpty(parameter))
                {
                    e.ReturnValue = "";
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    Zone cZone = business.Station.Get(parameter);
                    e.ReturnValue = cZone == null ? "" : cZone.Name;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetZoneName(获取地域名称)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取地域
        /// </summary>
        [MessageHandler(Name = ZoneMessage.VIRTUALPERSON_ZONE)]
        private void VirtualGetZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                string parameter = e.Parameter as string;
                if (string.IsNullOrEmpty(parameter))
                {
                    e.ReturnValue = "";
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    Zone cZone = business.Station.Get(parameter);
                    e.ReturnValue = cZone;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetZoneName(获取地域名称)", ex.Message + ex.StackTrace);
            }
        }


        /// <summary>
        /// 获取单位名称
        /// </summary>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME)]
        private void VirtualGetUnitName(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone parameter = e.Parameter as Zone;
                if (parameter == null)
                {
                    e.ReturnValue = "";
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    e.ReturnValue = business.GetZoneName(parameter);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetUnitName(获取单位名称)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 当前二轮台账导出表标题
        /// </summary>
        [MessageHandler(Name = SecondTableLandMessage.CURRENTZONE_UNITNAME)]
        public void OnGetTitleNameComplate(object sender, ModuleMsgArgs e)
        {
            try
            {
                string parameter = e.Parameter as string;
                if (parameter == null)
                {
                    e.ReturnValue = "";
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ZoneDataBusiness business = CreateBusiness(db);
                    Zone county = business.Get(parameter.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                    Zone privice = business.Get(parameter.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                    string ret = "";
                    if (privice != null && county != null)
                    {
                        string zoneName = county.FullName.Replace(privice.FullName, "");
                        ret = zoneName.Substring(0, zoneName.Length - 1);
                    }
                    e.ReturnValue = ret;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VirtualGetUnitName(获取单位名称)", ex.Message + ex.StackTrace);
            }
        }


        #endregion
    }
}
