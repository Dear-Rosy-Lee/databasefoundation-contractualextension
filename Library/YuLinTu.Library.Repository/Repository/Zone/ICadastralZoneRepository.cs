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
    /// <summary>
    /// 地籍区的数据访问接口
    /// </summary>
    public interface ICadastralZoneRepository :  IRepository<CadastralZone>
    {
        #region Methods - Stable

        /// <summary>
        /// 添加地籍区对象
        /// </summary>
        /// <param name="zone">地籍区对象实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int S_Add(CadastralZone zone);

        /// <summary>
        /// 批量添加地籍区对象
        /// </summary>
        /// <param name="listZone">地籍区对象实体集合</param>
        /// <param name="overwrite">存在已有对象是否覆盖</param>
        /// <param name="action"></param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int S_Add(CadastralList<Zone> listZone, bool overwrite, Action<CadastralZone, int, int> action);

        /// <summary>
        /// 得到指定区域下的所有有地籍区
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns>地籍区对象集合</returns>
        CadastralZone S_Get(string codeZone);

       
        /// <summary>
        /// 根据标识码获得对象
        /// </summary>
        /// <param name="idZone"></param>
        /// <returns></returns>
        CadastralZone S_Get(Guid idZone);

      
        /// <summary>
        /// 获得指定区域的地籍区域
        /// </summary>
        /// <param name="codeZone">地籍区代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        CadastralList<Zone> S_GetChildren(string codeZone, eLevelOption levelOption);

       
        /// <summary>
        /// 获得指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        CadastralList<Zone> S_GetCollection(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 统计指定区域的地籍区数量
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns></returns>
        int S_Count(string codeZone);

        /// <summary>
        /// 根据标识码删除地籍区对象
        /// </summary>
        /// <param name="idZone"></param>
        /// <returns></returns>
        int S_Delete(Guid idZone);

        /// <summary>
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns></returns>
        int S_Delete(string codeZone);

        /// <summary>
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int S_DeleteChildren(string codeZone, eLevelOption levelOption);


        /// <summary>
        /// 删除指定区域的地籍区对象
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns></returns>
        int S_DeleteCollection(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 更新地籍区对象
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        int S_Update(CadastralZone zone);

        #endregion

        #region Methods - Obsolete

        #region Methods


        /// <summary>
        /// 根据上级地域代码及其匹配等级统计区域等级为2的各类地域的数量
        /// </summary>
        /// <param name="zoneFullCodd">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>键（地域级别）值（地域数量）对</returns>
        SortedList<eZoneLevel, int> StatZoneLevelCountByFullCode(string zoneFullCodd, eLevelOption levelOption);

        /// <summary>
        /// 获得指定区域类指定地域级别的地域数量
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <param name="level">地域级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string codeZone, eLevelOption levelOption, eZoneLevel level);

        /// <summary>
        /// 统计指定区域类地域数量
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string codeZone);

        /// <summary>
        /// 根据地域编码获得地籍区对象
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <returns>地籍区对象</returns>
        CadastralZone Get(string codeZone);

        /// <summary>
        /// 根据地域标识码获得地籍区对象
        /// </summary>
        /// <param name="idZone">地域标识码</param>
        /// <returns>地籍区对象</returns>
        CadastralZone Get(Guid idZone);

        /// <summary>
        /// 根据地域等级获得地籍区对象
        /// </summary>
        /// <param name="level">地域等级</param>
        /// <returns>地籍区对象</returns>
        CadastralList<Zone> GetByZoneLevel(eZoneLevel level);

        /// <summary>
        /// 获得指定区域下所有二级子区域
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>地籍区集合对象</returns>
        CadastralList<Zone> GetChildren(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 统计指定区域下所有二级子区域的数量
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域ID删除地籍区对象
        /// </summary>
        /// <param name="zoneID">ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid zoneID);

        /// <summary>
        /// 根据地域编码删除地籍区对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByCode(string zoneCode);

        /// <summary>
        /// 根据地域编码删除二级子地籍区对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 更新地籍区对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(CadastralZone zone);

        /// <summary>
        /// 根据地域编码判断地籍区对象是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(string zoneCode);

        /// <summary>
        /// 根据地域id判断地籍区对象是否存在
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(Guid zoneID);

        /// <summary>
        /// 获得指定区域下所有指定地域级别的二级地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="zoneLevel">地域级别</param>
        /// <returns>地域集合</returns>
        CadastralList<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel);

        /// <summary>
        /// 根据地域编码获得二级子地籍区
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>二级子地籍区</returns>
        CadastralList<Zone> GetZones(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 得到包含指定区域代码的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>地域集合</returns>
        CadastralList<Zone> GetSubZones(string codeZone, eLevelOption levelOption);

        /// <summary>
        /// 根据地域名称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域集合</returns>
        CadastralList<Zone> SearchByName(string zoneName, eSearchOption searchType);

        /// <summary>
        /// 在指定地域下根据地域名称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchType">地域名称查找类型</param>
        /// <returns>地域集合</returns>
        CadastralList<Zone> SearchByName(string zoneName, string zoneCode, eSearchOption searchType);

        /// <summary>
        /// 根据地域全称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域全称</param>
        /// <param name="searchType">地域全称查找类型</param>
        /// <returns>地域集合</returns>
        CadastralList<Zone> SearchByFullName(string zoneName, eSearchOption searchType);

        /// <summary>
        /// 根据区域代码获得地域对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchType">区域代码查找类型</param>
        /// <returns>地域对象</returns>
        CadastralList<Zone> SearchByCode(string zoneCode, eSearchOption searchType);

        /// <summary>
        /// 根据地域全编码获得地域对象
        /// </summary>
        /// <param name="zoneFullCode">地域全编码</param>
        /// <param name="searchType">地域全编码查找类型</param>
        /// <returns>地域对象</returns>
        CadastralList<Zone> SearchByFullCode(string zoneFullCode, eSearchOption searchType);

        #endregion
        #endregion
    }
}
