// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 农村土地承包地的数据访问接口
    /// </summary>
    public interface ISecondTebleLandRepository :   IRepository<SecondTebleLand>
    {
        #region Methods


        /// <summary>
        /// 添加对象
        /// </summary>
        new  string Add(object entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        int Delete(Guid guid);

        /// <summary>
        /// 更新对象
        /// </summary>
        int Update(SecondTebleLand entity);

        /// <summary>
        /// 获取对象
        /// </summary>
        SecondTebleLand Get(Guid guid);

        /// <summary>
        /// 根据标识码判断土地承包地是否存在
        /// </summary>
        bool Exists(Guid guid);
        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据地籍号判断土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns></returns>
        bool ExistByCadastralNumber(string cadastralNumber);

        /// <summary>
        /// 根据权利人id获取土地承包地
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns>土地承包地对象</returns>
        SecondTebleLandCollection GetCollection(Guid familyID);

        /// <summary>
        /// 根据土地是否流转获取土地承包地
        /// </summary>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>土地承包地对象</returns>
        SecondTebleLandCollection GetByIsTransfer(bool isTransfer);

        /// <summary>
        /// 根据土地是否流转获取指定区域下的土地承包地
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>土地承包地；如果区域代码为null，返回所有区域下符合要求的对象</returns>
        SecondTebleLandCollection GetByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据土地是否流转统计指定区域下的土地承包地
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>-1(参数错误)/土地承包地数量</returns>
        int CountByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据流转类型获取承包地对象
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包地对象</returns>
        SecondTebleLandCollection GetByTransferType(eTransferType transferType);

        /// <summary>
        /// 根据流转类型获取指定区域下的承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包地对象</returns>
        SecondTebleLandCollection GetByTransferType(string zoneCode, eTransferType transferType);

        /// <summary>
        /// 根据户获取地块
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns>承包地块</returns>
        SecondTebleLandCollection GetCollection(Guid familyID, eConstructType constructType);

        /// <summary>
        /// 获取地域下以坐落单位代码排序的地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>以坐落单位代码排序的地块</returns>
        SecondTebleLandCollection GetCollection(string zoneCode);

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <returns>土地承包地对象</returns>
        SecondTebleLand Get(string number);

        /// <summary>
        /// 根据地籍号判断土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByNumber(string cadastralNumber);

        /// <summary>
        /// 根据地籍号获取土地承包地对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>土地承包地对象</returns>
        SecondTebleLand GetByNumber(string number);

        /// <summary>
        /// 根据户主名称获取地块
        /// </summary>
        /// <param name="householderName">权利人名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地块对象</returns>
        SecondTebleLandCollection GetByHouseholderName(string zoneCode, string householderName);

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>地块信息</returns>
        SecondTebleLandCollection GetByConcordId(Guid concordId);

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 删除当前地域下指定承包经营权类型所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eConstructType constructType);

        /// <summary>
        /// 获取合同下地块面积
        /// </summary>
        /// <param name="guid">合同ID</param>
        /// <returns>地块面积</returns>
        double GetLandAreaByConcordID(Guid guid);

        /// <summary>
        /// 根据区域代码匹配地籍号来获取土地承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>土地承包地对象</returns>
        SecondTebleLandCollection GetLandsByZoneCodeInCadastralNumber(string zoneCode);

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1(参数错误)/地块数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域统计面积
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>统计面积</returns>
        double CountArea(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 统计地域下实测面积
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>实测面积</returns>
        double CountActualArea(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 统计区域下没有合同的地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1(参数错误)/没有合同的地块数量</returns>
        int CountNoConcord(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域获取地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption"><匹配等级/param>
        /// <returns>地块</returns>
        SecondTebleLandCollection GetCollection(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按户统计地块
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns></returns>
        int Count(Guid familyID);


        /// <summary>
        /// 根据地籍号获得以权利人名称排序的承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>承包地对象</returns>
        SecondTebleLandCollection SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType);

        /// <summary>
        /// 根据权利人名称获得以权利人名称排序的承包地对象
        /// </summary>
        /// <param name="name">权利人名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>承包地对象</returns>
        SecondTebleLandCollection SearchByVirtualPersonName(string name, eSearchOption searchType);

        /// <summary>
        /// 根据地籍号与权属单位代码获取以权利人姓名排序的土地承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">地籍号查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以权利人姓名排序的土地承包地对象</returns>
        SecondTebleLandCollection SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 根据地籍号与权属单位代码获取以权利人姓名排序的土地承包地对象
        /// </summary>
        /// <param name="name">权利人名称</param>
        /// <param name="searchType">权利人名称匹配类型</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>土地承包对象</returns>
        SecondTebleLandCollection SearchByVirtualPersonName(string name, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 按土地类型搜索
        /// </summary>
        /// <returns></returns>
        SecondTebleLandCollection SearchByLandType(string zoneCode, eConstructType constructType);

        /// <summary>
        /// 按承包地类型搜索指定区域下的承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">经营权类型</param>
        /// <returns>承包地对象</returns>
        SecondTebleLandCollection SearchByLandTypeAndCadaZone(string zoneCode, eConstructType constructType);

        #endregion
    }
}