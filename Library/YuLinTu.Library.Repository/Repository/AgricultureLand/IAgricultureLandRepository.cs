/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// (ContractLand、SeondTableLand)地块的数据访问接口
    /// </summary>
    public interface IAgricultureLandRepository<T> : IRepository<T> where T : ContractLand
    {
        #region Methods

        /// <summary>
        /// 添加农村土地承包地对象
        /// </summary>
        new string Add(T entity);

        /// <summary>
        /// 根据标识码删除农村土地承包地对象
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 根据承包方ID删除下属地块信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteLandByPersonID(Guid guid);

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(T entity);

        int UpdateOldLandsCode(T entity);

        /// <summary>
        /// 根据承包方id更新承包方名称
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="ownerName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Guid ownerId, string ownerName);

        /// <summary>
        /// 根据标识码获取对象
        /// </summary>
        T Get(Guid guid);

        /// <summary>
        /// 根据标识码判断对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据地籍号精确判断农村土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByCadastralNumberP(string cadastralNumber);

        /// <summary>
        /// 根据承包方id获取地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>地块集合</returns>
        List<T> GetCollection(Guid ownerId);

        /// <summary>
        /// 根据土地是否流转获取农村土地承包地对象集合
        /// </summary>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>农村土地承包地对象集合</returns>
        List<T> GetByIsTransfer(bool isTransfer);

        /// <summary>
        /// 根据土地是否流转获取指定区域下的农村土地承包地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>农村土地承包地对象集合；指定区域为null，返回所有区域符合土地是否流转参数的土地对象</returns>
        List<T> GetByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据土地是否流转统计指定区域下的农村土地承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int CountByIsTransfer(string zoneCode, bool isTransfer);

        /// <summary>
        /// 根据流转类型获取土地承包地对象集合
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>农村土地承包地对象集合</returns>
        List<T> GetByTransferType(string transferType);

        /// <summary>
        /// 根据流转类型获取指定区域下的土地承包地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>土地承包地对象集合；指定区域为null，返回所有区域符合流转类型参数的土地对象</returns>
        List<T> GetByTransferType(string zoneCode, string transferType);

        /// <summary>
        /// 根据地块类别获取指定承包方id下的地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructType">地块类别</param>
        /// <returns>地块集合</returns>
        List<T> GetCollection(Guid ownerId, string constructType);

        /// <summary>
        /// 根据承包方式获取指定承包方id下的地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructMode">承包方式</param>
        /// <returns>地块集合</returns>
        List<T> GetCollectionByConstructMode(Guid ownerId, string constructMode);

        /// <summary>
        /// 获取指定地域下的地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>土地对象集合</returns>
        List<T> GetCollection(string zoneCode);

        /// <summary>
        /// 根据地籍号获取地块对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>土地承包地对象</returns>
        T Get(string cadastralNumber);

        T GetByLandNumber(string landNumber);

        /// <summary>
        /// 根据地籍号模糊判断对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByCadastralNumberF(string cadastralNumber);

        /// <summary>
        /// 根据地籍号获取土地承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>土地承包地对象</returns>
        T GetByCadastralNumber(string cadastralNumber);

        /// <summary>
        /// 获取指定区域下指定承包方姓名的地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地块集合</returns>
        List<T> GetByOwnerName(string zoneCode, string ownerName);

        /// <summary>
        /// 根据合同Id获取地块集合
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>地块集合</returns>
        List<T> GetByConcordId(Guid concordId);

        /// <summary>
        /// 根据合同Id集合获取地块信息集合
        /// </summary>
        List<T> GetByConcordIds(Guid[] ids);

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 删除当前地域下指定承包经营权类型的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, string constructType);

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption);

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteOtherByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据目标地块，返回相交地块集合
        /// </summary>
        /// <param name="tagetLand"></param>
        /// <returns></returns>
        List<ContractLand> GetIntersectLands(ContractLand tagetLand, Geometry tagetLandShape);

        /// <summary>
        /// 获取指定合同Id下的地块面积
        /// </summary>
        /// <param name="concordId">合同id</param>
        /// <returns>地块面积</returns>
        double GetLandAreaByConcordID(Guid concordId);

        /// <summary>
        /// 根据区域代码获得所有该区域下的并且以地籍号排序的土地对象集合【CadastralNumber.Contains(zoneCode)】
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以地籍号排序的承包地对象集合</returns>
        List<T> GetLandsByZoneCodeInCadastralNumber(string zoneCode);

        int CountByConcordId(Guid concordId);

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
        /// 根据地块ID集合删除对应界址点线
        /// </summary>
        /// <param name="landIDs"></param>
        void DeleteCoilDotByLandIDs(List<Guid> landIDs);

        /// <summary>
        /// 按地域获取地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块集合</returns>
        List<T> GetCollection(string zoneCode, eLevelOption searchOption);
         
        /// <summary>
        /// 获取指定地域下的所有空间地块集合
        /// </summary>
        /// <param name="zoneCode">指定地域编码</param>
        /// <param name="levelOption">查找地域级别</param>
        /// <returns>空间地块集合</returns>
        List<T> GetShapeCollection(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 按地域匹配等级获取指定承包方状态的地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>地块集合</returns>
        List<T> GetCollection(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption);

        /// <summary>
        /// 根据地域及地域级别查询指定承包方状态下没有界址信息地块
        /// </summary>
        /// <param name="zoneCode">地域信息</param>
        /// <param name="option">地域级别</param>
        /// <returns>查询结果</returns>
        List<T> GetLandsWithoutSiteInfoByZone(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption option = eLevelOption.Self);

        /// <summary>
        /// 根据承包方标识集合获取地块集合
        /// </summary>
        /// <param name="obligeeIds">承包方标识集合</param>
        /// <returns>查询地块集合</returns>
        List<T> GetLandsByObligeeIds(Guid[] obligeeIds);

        /// <summary>
        /// 按承包方ID统计地块
        /// </summary>
        /// <param name="ownerId">承包方ID</param>
        /// <returns>地块数量</returns>
        int Count(Guid ownerId);

        /// <summary>
        /// 根据地籍号获取以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包地地块集合对象</returns>
        List<T> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType);

        /// <summary>
        /// 根据承包方名称获取承包地地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包地地块集合对象</returns>
        List<T> SearchByVirtualPersonName(string ownerName, eSearchOption searchType);

        /// <summary>
        /// 根据地籍号获取指定区域下以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包地地块集合对象；区域代码为空时，返回所有满足指定地籍号的并以承包方名称排序的地块集合</returns>
        List<T> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 根据承包方名称获取指定区域下以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包地地块集合对象；区域代码为空时，返回所有满足指定承包方名称的并以承包方名称排序的地块集合</returns>
        List<T> SearchByVirtualPersonName(string ownerName, eSearchOption searchType, string zoneCode);

        /// <summary>
        /// 按承包经营权类型搜索指定区域的土地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>土地对象集合；如果区域代码为null，返回空的承包地集合对象</returns>
        List<T> SearchByLandType(string zoneCode, string constructType);

        /// <summary>
        /// 按承包经营权类型搜索指定承包地地籍区编号的土地对象集合
        /// </summary>
        /// <param name="cadastralZoneCode">地籍区编号</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>土地对象集合；如果区域代码为null，返回空的承包地集合对象</returns>
        List<T> SearchByLandTypeAndCadaZone(string cadastralZoneCode, string constructType);

        /// <summary>
        /// 按区域搜索指定地类名称的承包地块对象
        /// </summary>
        ///<param name="landName">地类名称</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>承包地对象</returns>
        T SearchByLandNameAndZoneCode(string landName, string zoneCode);

        /// <summary>
        /// 将数据库中承包地承包方式设置为家庭承包
        /// </summary>
        void SetDataBaseCBDFamilyCBFS();

        /// <summary>
        /// 删除选定承包方所有数据
        /// </summary>
        void DeleteSelectVirtualPersonAllData(Guid ID);

        /// <summary>
        /// 根据地块标识集合删除和更新相关业务数据
        /// </summary>
        /// <param name="ids">地块标识集合</param>
        int DeleteRelationDataByLand(Guid[] ids);
        /// <summary>
        /// 根据图形获取相交和包含的承包地块
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        List<T> GetLandsBYGraph(Geometry graph);

        #endregion
    }
}
