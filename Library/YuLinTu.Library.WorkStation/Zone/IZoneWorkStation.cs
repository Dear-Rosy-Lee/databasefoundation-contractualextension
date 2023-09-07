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
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 地域模块接口定义
    /// </summary>
    public interface IZoneWorkStation : IWorkstation<Zone>
    {
        #region Methods

        /// <summary>
        /// 批量添加地域对象
        /// </summary>
        /// <param name="listZone">地域对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        int Add(List<Zone> listZone, bool overwrite, Action<Zone, int, int> action);

        /// <summary>
        /// 根据地域全编码获得地域
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns>地域</returns>
        Zone Get(string codeZone);

        /// <summary>
        /// 根据全部地域
        /// </summary>
        List<Zone> GetAll();

        /// <summary>
        /// 根据地域ID获得地域
        /// </summary>
        /// <param name="idZone">地域ID</param>
        /// <returns>地域</returns>
        Zone Get(Guid idZone);

        /// <summary>
        /// 获取指定区域的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        List<Zone> GetChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域全编码统计地域数量
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        int Count(string codeZone);

        /// <summary>
        /// 统计指定区域下所有二级子地域数量
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据地域id删除对象
        /// </summary>
        /// <param name="idZone">地域id</param>
        /// <returns></returns>
        int Delete(Guid idZone);

        /// <summary>
        /// 根据地域全编码删除对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns></returns>
        int Delete(string codeZone);

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int DeleteChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域全编码删除地域对象
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int DeleteCollection(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 更新地域对象
        /// </summary>
        /// <param name="zone">地域</param>
        /// <returns></returns>
        int Update(Zone zone);

        /// <summary>
        /// 获取指定区域下指定地域等级的地域
        /// </summary>
        /// <param name="codeZone">地域代码</param>
        /// <param name="levelOption">地域等级</param>
        /// <returns>地域</returns>
        List<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel);

        /// <summary>
        /// 根据指定的地域编码判断地域是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(string zoneCode);

        /// <summary>
        /// 根据地域级别获得地域对象
        /// </summary>
        /// <param name="level">地域级别</param>
        /// <returns>地域对象</returns>
        List<Zone> GetByZoneLevel(eZoneLevel level);

        /// <summary>
        /// 根据指定的上级地域编码判断当前加入的地域名称是否存在
        /// </summary>
        /// <param name="upLevelCode">上级地域编码</param>
        /// <param name="zone">当前添加地域</param>
        bool ExistName(string upLevelCode, Zone zone);

        /// <summary>
        /// 清空所有地域图斑
        /// </summary>
        int ClearZoneShape();

        /// <summary>
        /// 获取地域名称(到镇)
        /// </summary>
        string GetTownZoneName(Zone zone);

        /// <summary>
        /// 获取村发包时村级地域名称(到镇)
        /// </summary>
        string GetTownVillageName(Zone zone);

        /// <summary>
        /// 获取地域名称
        /// </summary>
        string GetZoneName(Zone zone);

        /// <summary>
        /// 获取村发包时村级地域名称
        /// </summary>
        string GetVillageName(Zone zone);

        /// <summary>
        /// 获取全部的地域(包括镇、村、组三级地域)
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <returns>如传入地域为村，则返回的地域集合包括镇、村、组</returns>
        List<Zone> GetAllZones(Zone zone);

        /// <summary>
        /// 获取到镇的父级地域
        /// </summary>
        /// <param name="zone">传入的地域</param>
        /// <returns>父级地域集合</returns>
        List<Zone> GetParents(Zone zone);

        /// <summary>
        /// 获取全部的地域(直到省)
        /// </summary>
        /// <param name="zone">当前地域</param>
        List<Zone> GetAllZonesToProvince(Zone zone);

        /// <summary>
        /// 获取到省的父级地域
        /// </summary>
        /// <param name="zone">传入的地域</param>
        /// <returns>父级地域集合</returns>
        List<Zone> GetParentsToProvince(Zone zone);

        /// <summary>
        /// 获取最大行政地域
        /// </summary>
        List<Zone> GetMaxLevelZone();

        string GetZoneNameByLevel(string zoneCode, eZoneLevel level);

        /// <summary>
        /// 获取与目标图形相交的最大面积的地域
        /// </summary>
        /// <returns></returns>
        Zone GetGeoIntersectZoneOFMaxAea(YuLinTu.Spatial.Geometry targetGeo);

        #endregion
    }
}
