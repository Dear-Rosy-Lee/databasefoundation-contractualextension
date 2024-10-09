/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 地域模块接口定义实现
    /// </summary>
    public sealed class ZoneWorkStation : Workstation<Zone>, IZoneWorkStation
    {
        #region Properties

        public new IZoneRepository DefaultRepository
        {
            get { return base.DefaultRepository as IZoneRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        public ZoneWorkStation(IZoneRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 批量添加地域对象
        /// </summary>
        /// <param name="listZone">地域对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int Add(List<Zone> listZone, bool overwrite, Action<Zone, int, int> action)
        {
            DefaultRepository.S_Add(listZone, overwrite, action);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域前编码获取数据
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <returns></returns>
        public Zone Get(string codeZone)
        {
            return DefaultRepository.Get(codeZone);
        }

        /// <summary>
        /// 根据地域id获取地域
        /// </summary>
        /// <param name="idZone">地域ID</param>
        /// <returns></returns>
        public Zone Get(Guid idZone)
        {
            return DefaultRepository.Get(idZone);
        }

        /// <summary>
        /// 得到指定区域下所有二级子地域集合
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>地域集合</returns>
        public List<Zone> GetChildren(string codeZone, eLevelOption levelOption)
        {
            if (codeZone == "86")
            {
                return GetAll();
            }
            return DefaultRepository.GetChildren(codeZone, levelOption);
        }

        /// <summary>
        /// 统计地域编码下数据个数
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns></returns>
        public int Count(string codeZone)
        {
            return DefaultRepository.Count(codeZone);
        }

        /// <summary>
        /// 统计指定区域下所有二级子地域数量
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption levelOption)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                throw new YltException("获取地域编码参数失败!");
            }
            return DefaultRepository.Count(zoneCode, levelOption);
        }

        /// <summary>
        /// 根据区域id删除区域对象
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid idZone)
        {
            DefaultRepository.Delete(idZone);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据区域编码删除区域对象
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(string codeZone)
        {
            DefaultRepository.DeleteByCode(codeZone);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public int DeleteChildren(string codeZone, eLevelOption levelOption)
        {
            DefaultRepository.S_DeleteChildren(codeZone, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        public int DeleteCollection(string codeZone, eLevelOption levelOption)
        {
            DefaultRepository.S_DeleteCollection(codeZone, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据id更新地域对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Zone zone)
        {
            DefaultRepository.Update(zone);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 获取全部地域
        /// </summary>
        /// <returns></returns>
        public List<Zone> GetAll()
        {
            List<Zone> collection = new List<Zone>();
            List<Zone> list = DefaultRepository.Get();
            if (list != null && list.Count > 0)
            {
                list.ForEach(t => collection.Add(t));
            }
            return collection;
        }

        /// <summary>
        /// 获取指定区域下指定地域等级的地域
        /// </summary>
        /// <param name="codeZone">地域代码</param>
        /// <param name="levelOption">地域等级</param>
        /// <returns>地域</returns>
        public List<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel)
        {
            return DefaultRepository.GetByChildLevel(codeZone, zoneLevel);
        }

        /// <summary>
        /// 根据指定的地域编码判断地域是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(string zoneCode)
        {
            return DefaultRepository.Exists(zoneCode);
        }

        /// <summary>
        /// 根据地域级别获得地域对象
        /// </summary>
        /// <param name="level">地域级别</param>
        /// <returns>地域对象</returns>
        public List<Zone> GetByZoneLevel(eZoneLevel level)
        {
            return DefaultRepository.GetByZoneLevel(level);
        }

        /// <summary>
        /// 根据指定的上级地域编码判断当前加入的地域名称是否存在
        /// </summary>
        /// <param name="upLevelCode">上级地域编码</param>
        /// <param name="zone">当前添加地域</param>
        public bool ExistName(string upLevelCode, Zone zone)
        {
            return DefaultRepository.ExistName(upLevelCode, zone);
        }

        /// <summary>
        /// 清空所有地域图斑
        /// </summary>
        public int ClearZoneShape()
        {
            int cnt = 0;
            var allZones = DefaultRepository.Get();
            if (allZones == null || allZones.Count == 0)
            {
                return -1;
                throw new YltException("获取行政地域数据失败!");
            }
            foreach (var zone in allZones)
            {
                if (zone.Shape != null)
                {
                    zone.Shape = null;
                    cnt = DefaultRepository.Update(zone);
                }
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 获取地域名称(到镇)
        /// </summary>
        public string GetTownZoneName(Zone zone)
        {
            if (zone == null)
            {
                return "";
                throw new YltException("获取行政地域数据失败!");
            }
            string name = zone.Name;
            if (zone.FullCode.Length <= 6)
            {
                return name;
            }
            Zone tempZone = Get(zone.UpLevelCode);
            while (tempZone.Level < eZoneLevel.County)
            {
                name = tempZone.Name + name;
                tempZone = Get(tempZone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 获取村发包时村级地域名称(到镇)
        /// </summary>
        public string GetTownVillageName(Zone zone)
        {
            if (zone == null)
            {
                return "";
                throw new YltException("获取行政地域数据失败!");
            }
            string name = zone.Level == eZoneLevel.Group ? "" : zone.Name;
            if (zone.FullCode.Length <= 6)
            {
                return zone.Name;
            }
            Zone tempZone = Get(zone.UpLevelCode);
            while (tempZone.Level < eZoneLevel.County)
            {
                name = tempZone.Name + name;
                tempZone = Get(tempZone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        public string GetZoneName(Zone zone)
        {
            if (zone == null)
            {
                return "";
                throw new YltException("获取行政地域数据失败!");
            }
            string name = zone.Name;
            if (zone.FullCode.Length <= 6)
            {
                return name;
            }
            Zone tempZone = Get(zone.UpLevelCode);
            while (tempZone.Level <= eZoneLevel.County)
            {
                name = tempZone.Name + name;
                tempZone = Get(tempZone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 获取村发包时村级地域名称
        /// </summary>
        public string GetVillageName(Zone zone)
        {
            if (zone == null)
            {
                return "";
                throw new YltException("获取行政地域数据失败!");
            }
            string name = zone.Level == eZoneLevel.Group ? "" : zone.Name;
            if (zone.FullCode.Length <= 6)
            {
                return zone.Name;
            }
            Zone tempZone = Get(zone.UpLevelCode);
            while (tempZone.Level <= eZoneLevel.County)
            {
                name = tempZone.Name + name;
                tempZone = Get(tempZone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 获取全部的地域(包括镇、村、组三级地域)
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <returns>如传入地域为村，则返回的地域集合包括镇、村、组</returns>
        public List<Zone> GetAllZones(Zone zone)
        {
            List<Zone> allZones = new List<Zone>();
            if (zone == null)
            {
                return allZones;
                throw new YltException("获取行政地域数据失败!");
            }
            List<Zone> childrenZones = GetChildren(zone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> parentZone = GetParents(zone);
            if (zone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(zone);
                if (parentZone != null)
                    allZones.AddRange(parentZone);
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                //选择为村
                if (parentZone != null)
                    allZones.AddRange(parentZone);
                if (childrenZones != null)
                    allZones.AddRange(childrenZones);
            }
            else if (zone.Level >= eZoneLevel.Town)
            {
                //选择为镇
                if (childrenZones != null)
                    allZones.AddRange(childrenZones);
            }
            return allZones;
        }

        /// <summary>
        /// 获取到镇的父级地域
        /// </summary>
        /// <param name="zone">传入的地域</param>
        /// <returns>父级地域集合</returns>
        public List<Zone> GetParents(Zone zone)
        {
            List<Zone> parentsZones = new List<Zone>();
            if (zone == null || (zone != null && zone.Level >= eZoneLevel.Town))
            {
                return parentsZones;
                throw new YltException("获取行政地域数据失败!");
            }
            Zone temZone = Get(zone.UpLevelCode);
            parentsZones.Add(temZone);
            while (temZone.Level < eZoneLevel.Town)
            {
                temZone = Get(temZone.UpLevelCode);
                parentsZones.Add(temZone);
            }
            return parentsZones;
        }

        /// <summary>
        /// 获取全部的地域(直到省)
        /// </summary>
        /// <param name="zone">当前地域</param>
        public List<Zone> GetAllZonesToProvince(Zone zone)
        {
            List<Zone> allZones = new List<Zone>();
            if (zone == null)
            {
                return allZones;
                throw new YltException("获取行政地域数据失败!");
            }
            List<Zone> childrenZones = GetChildren(zone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> parentZone = GetParentsToProvince(zone);
            if (zone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(zone);
                if (parentZone != null)
                    allZones.AddRange(parentZone);
            }
            else
            {
                //选择为村
                if (parentZone != null)
                    allZones.AddRange(parentZone);
                if (childrenZones != null)
                    allZones.AddRange(childrenZones);
            }
            return allZones;
        }

        /// <summary>
        /// 获取到省的父级地域
        /// </summary>
        /// <param name="zone">传入的地域</param>
        /// <returns>父级地域集合</returns>
        public List<Zone> GetParentsToProvince(Zone zone)
        {
            List<Zone> parentsZones = new List<Zone>();
            if (zone == null)
            {
                return parentsZones;
            }
            Zone temZone = Get(zone.UpLevelCode);
            if (temZone != null)
            {
                parentsZones.Add(temZone);
                while (temZone.Level < eZoneLevel.Province)
                {
                    temZone = Get(temZone.UpLevelCode);
                    parentsZones.Add(temZone);
                }
            }
            return parentsZones;
        }

        /// <summary>
        /// 获取最大行政地域
        /// </summary>
        public List<Zone> GetMaxLevelZone()
        {
            return DefaultRepository.GetMaxLevelZone();
        }


        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary> 
        public string GetZoneNameByLevel(string zoneCode, eZoneLevel level)
        {
            Zone temp = Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            return GetZoneNameByLevel(temp.UpLevelCode, level);
        }

        /// <summary>
        /// 获取与目标图形相交的最大面积的地域
        /// </summary>
        /// <returns></returns>
        public Zone GetGeoIntersectZoneOFMaxAea(YuLinTu.Spatial.Geometry targetGeo)
        {
            return DefaultRepository.GetGeoIntersectZoneOFMaxAea(targetGeo);
        }

        #endregion
    }
}
