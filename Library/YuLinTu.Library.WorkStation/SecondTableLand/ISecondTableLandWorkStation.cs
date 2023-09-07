/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 二轮台账地块接口定义
    /// </summary>
    public interface ISecondTableLandWorkStation : IWorkstation<SecondTableLand>
    {
        #region Methods

        /// <summary>
        /// 根据标识码删除二轮台账地块对象
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 更新二轮台账地块对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(SecondTableLand entity);

        /// <summary>
        ///  根据承包方id更新承包方名称
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="ownerName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Guid ownerId, string ownerName);

        /// <summary>
        /// 获取对象
        /// </summary>
        SecondTableLand Get(Guid guid);

        /// <summary>
        /// 根据标识码判断二轮台账地块对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        /// <summary>
        /// 批量添加二轮承包地块数据
        /// </summary>
        /// <param name="listSecondLand">二轮承包地块对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<SecondTableLand> listSecondLand);

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据地籍号精确判断农村土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByCadastralNumberP(string cadastralNumber);

        /// <summary>
        /// 根据承包方id获取二轮台账地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetCollection(Guid ownerId);

        /// <summary>
        /// 根据土地是否流转获取二轮台账地块集合
        /// </summary>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetByIsTransfer(bool isTransfer);

        /// <summary>
        /// 根据土地是否流转获取指定区域下的二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>二轮台账地块集合；指定区域为null，返回所有区域符合土地是否流转参数的土地对象</returns>
        List<SecondTableLand> GetByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据土地是否流转统计指定区域下的二轮台账地块数量
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int CountByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据流转类型获取二轮台账地块集合
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetByTransferType(string transferType);

        /// <summary>
        /// 根据流转类型获取指定区域下的二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>二轮台账地块集合；指定区域为null，返回所有区域符合流转类型参数的土地对象</returns>
        List<SecondTableLand> GetByTransferType(string zoneCode, string transferType);

        /// <summary>
        /// 根据承包方id获取地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>地块集合</returns>
        List<SecondTableLand> GetCollection(Guid ownerId, string constructType);

        /// <summary>
        /// 获取地域下二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetCollection(string zoneCode);

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>二轮台账地块对象</returns>
        SecondTableLand Get(string cadastralNumber);

        /// <summary>
        /// 根据地籍号模糊判断对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByCadastralNumberF(string cadastralNumber);

        /// <summary>
        /// 根据地籍号获取二轮台账地块对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>二轮台账地块对象</returns>
        SecondTableLand GetByCadastralNumber(string cadastralNumber);

        /// <summary>
        /// 获取指定区域下指定承包方姓名的二轮台账地块对象集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetByOwnerName(string zoneCode, string ownerName);

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetByConcordId(Guid concordId);

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 删除当前地域下指定承包经营权类型的所有二轮台账地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, string constructType);

        /// <summary>
        /// 根据承包方ID删除下属地块信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteLandByPersonID(Guid guid);

        /// <summary>
        /// 获取合同id下的二轮台账地块面积
        /// </summary>
        /// <param name="concordId">合同id</param>
        /// <returns>地块面积</returns>
        double GetLandAreaByConcordID(Guid concordId);

        /// <summary>
        /// 根据区域代码获得所有该区域下的并且以地籍号排序的二轮台账土地对象集合【CadastralNumber.Contains(zoneCode)】
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以地籍号排序的二轮台账地块</returns>
        List<SecondTableLand> GetLandsByZoneCodeInCadastralNumber(string zoneCode);

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域统计面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>面积</returns>
        double CountArea(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 统计地域下实测面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>实测面积</returns>
        double CountActualArea(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 统计区域下没有合同的地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        int CountNoConcord(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域获取二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param na二轮台账me="searchOption">匹配等级</param>
        /// <returns>二轮台账地块集合</returns>
        List<SecondTableLand> GetCollection(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按承包方ID统计地块
        /// </summary>
        /// <param name="ownerId">承包方ID</param>
        /// <returns>地块数量</returns>
        int Count(Guid ownerId);

        /// <summary>
        /// 根据地籍号获取以承包方名称排序的二轮台账地块集合
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的二轮台账地块对象集合</returns>
        List<SecondTableLand> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType);

        /// <summary>
        /// 根据承包方名称获取以承包方名称排序的二轮台账地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的二轮台账地块集合</returns>
        List<SecondTableLand> SearchByVirtualPersonName(string ownerName, eSearchOption searchType);

        /// <summary>
        /// 根据地籍号获取指定区域下以承包方名称排序的二轮台账地块对象集合
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的二轮台账地块集合;区域代码为空时，返回所有满足指定地籍号的并以承包方名称排序的地块集合</returns>
        List<SecondTableLand> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 根据承包方名称获取指定区域下以承包方名称排序的二轮台账地块对象集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的二轮台账地块对象集合;区域代码为空时，返回所有满足指定承包方名称的并以承包方名称排序的地块集合</returns>
        List<SecondTableLand> SearchByVirtualPersonName(string ownerName, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 按承包经营权类型搜索指定区域的二轮台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>二轮台账地块集合；如果区域代码为null，返回空的二轮台账地块对象集合</returns>
        List<SecondTableLand> SearchByLandType(string zoneCode, string constructType);

        /// <summary>
        /// 按承包经营权类型搜索指定承包地地籍区编号的二轮台账地块集合
        /// </summary>
        /// <param name="cadastralZoneCode">地籍区编号</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>二轮台账地块集合；如果区域代码为null，返回空的承包地集合对象</returns>
        List<SecondTableLand> SearchByLandTypeAndCadaZone(string cadastralZoneCode, string constructType);

        /// <summary>
        /// 按区域搜索指定地类名称的二轮台账地块信息
        /// </summary>
        /// <param name="landName">地类名称</param>
        /// <returns>二轮台账地块对象</returns>
        SecondTableLand SearchByLandNumberAndZoneCode(string landName, string zoneCode);

        #endregion
    }
}
