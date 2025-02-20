// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    public interface IZoneRepository : IRepository<Zone>
    {
        #region Methods - Stable

        /// <summary>
        /// 添加地域对象
        /// </summary>
        /// <param name="zone">地域对象实体</param>
        /// <returns></returns>
        int S_Add(Zone zone);

        /// <summary>
        /// 批量添加地域对象
        /// </summary>
        /// <param name="listZone">地域对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        int S_Add(List<Zone> listZone, bool overwrite, Action<Zone, int, int> action);

        /// <summary>
        /// 根据地域全编码获得地域
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns>地域</returns>
        Zone S_Get(string codeZone);

        /// <summary>
        /// 根据地域ID获得地域
        /// </summary>
        /// <param name="idZone">地域ID</param>
        /// <returns>地域</returns>
        Zone S_Get(Guid idZone);

        /// <summary>
        /// 获取指定区域的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        List<Zone> S_GetChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 获取指定区域的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        List<Zone> S_GetCollection(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域全编码统计地域数量
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        int S_Count(string codeZone);

        /// <summary>
        /// 根据地域id删除对象
        /// </summary>
        /// <param name="idZone">地域id</param>
        /// <returns></returns>
        int S_Delete(Guid idZone);

        /// <summary>
        /// 根据地域全编码删除对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        int S_Delete(string codeZone);

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int S_DeleteChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int S_DeleteCollection(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 更新地域对象
        /// </summary>
        /// <param name="zone">地域</param>
        /// <returns></returns>
        int S_Update(Zone zone);

        #endregion

        #region Methods - Obsolete

        #region Methods

        /// <summary>
        /// 统计指定地域下二级地域各地域等级的地域数量
        /// </summary>
        /// <param name="zoneFullCodd">上级地域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>键（地域级别）值（地域数量）对</returns>
        SortedList<eZoneLevel, int> StatZoneLevelCountByFullCode(string zoneFullCodd, eLevelOption levelOption);

        /// <summary>
        /// 统计指定区域指定地域级别的地域数量
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">地域全编码匹配等级</param>
        /// <param name="level">地域等级</param>
        /// <returns>-1（参数错误）/int 地域数量</returns>
        int Count(string codeZone, eLevelOption levelOption, eZoneLevel level);

        /// <summary>
        /// 根据地域全编码获得地域
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns>地域</returns>
        Zone Get(string codeZone);

        /// <summary>
        /// 根据地域id获得地域
        /// </summary>
        /// <param name="idZone">id</param>
        /// <returns>地域</returns>
        Zone Get(Guid idZone);

        /// <summary>
        /// 根据地域级别获得地域对象
        /// </summary>
        /// <param name="level">地域级别</param>
        /// <returns>地域对象</returns>
        List<Zone> GetByZoneLevel(eZoneLevel level);

        /// <summary>
        /// 得到指定区域下所有二级子地域集合
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>地域集合</returns>
        List<Zone> GetChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 统计指定区域下所有二级子地域数量
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域编码获得指定地域级别的上级地域
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="level">地域级别</param>
        /// <returns>上级地域</returns>
        Zone GetUpZone(string codeZone, eZoneLevel level);

        /// <summary>
        /// 根据区域id删除区域对象
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid zoneID);

        /// <summary>
        /// 根据地域全编码删除地域
        /// </summary>
        /// <param name="zoneCode">地域全编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByCode(string zoneCode);

        /// <summary>
        /// 根据地域编码删除其二级子地域
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">地域编码匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据id更新地域对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Zone zone);

        int UpdateCodeName(Zone zone);

        /// <summary>
        /// 根据地域全编码更新地域对象
        /// </summary>
        /// <param name="zone">地域实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int UpdateByFullCode(Zone zone);

        /// <summary>
        /// 根据指定的地域编码判断地域是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(string zoneCode);

        /// <summary>
        /// 根据区域id确定地域对象是否存在
        /// </summary>
        /// <param name="zoneID">区域id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(Guid zoneID);

        /// <summary>
        /// 获取指定区域下指定地域等级的地域
        /// </summary>
        /// <param name="codeZone">地域代码</param>
        /// <param name="levelOption">地域等级</param>
        /// <returns>地域</returns>
        List<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel);

        /// <summary>
        /// 得到指定区域所有二级子区域
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>区域对象</returns>
        List<Zone> GetZones(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据地域编码获取地域
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>地域对象</returns>
        List<Zone> GetSubZones(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域名称及其查找类型获取地域对象集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象集合</returns>
        List<Zone> SearchByName(string zoneName, eSearchOption searchType);

        /// <summary>
        /// 根据指定的地域名称、地域代码及其查找类型获得区域对象集合
        /// </summary>
        /// <param name="zoneName">区域名称</param>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>区域对象集合</returns>
        List<Zone> SearchByName(string zoneName, string zoneCode, eSearchOption searchType);

        /// <summary>
        /// 根据地域全称及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneName">地域全称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        List<Zone> SearchByFullName(string zoneName, eSearchOption searchType);

        /// <summary>
        /// 根据地域编码及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        List<Zone> SearchByCode(string zoneCode, eSearchOption searchType);

        /// <summary>
        /// 根据地域全编码及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneFullCode">地域全编码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        List<Zone> SearchByFullCode(string zoneFullCode, eSearchOption searchType);

        /// <summary>
        /// 根据指定的上级地域编码判断当前加入的地域名称是否存在
        /// </summary>
        /// <param name="upLevelCode">上级地域编码</param>
        /// <param name="zone">当前添加地域</param>
        bool ExistName(string upLevelCode, Zone zone);

        /// <summary>
        /// 获取最大行政地域
        /// </summary>
        /// <returns></returns>
        List<Zone> GetMaxLevelZone();

        /// <summary>
        /// 获取与目标图形相交的最大面积的地域
        /// </summary>
        /// <returns></returns>
        Zone GetGeoIntersectZoneOFMaxAea(YuLinTu.Spatial.Geometry targetGeo);

        #endregion

        #endregion
    }
}
