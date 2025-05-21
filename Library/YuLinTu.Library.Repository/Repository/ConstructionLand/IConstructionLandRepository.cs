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
    /// 集体建设用地使用权表的数据访问接口
    /// </summary>
    public interface IConstructionLandRepository :   IRepository<ConstructionLand>
    {
        #region Methods

        /// <summary>
        /// 根据地域全代码获取对象
        /// </summary>
        /// <param name="fullCode">地域全代码</param>
        /// <returns></returns> 
        List<ContractLand> Get(string fullCode);

        /// <summary>
        /// 统计集体建设用地使用权对象数量
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int CountByZoneCode(string zoneCode);

        /// <summary>
        /// 根据集体建设用地使用权ID获取数据
        /// </summary>
        /// <param name="buildLandPropertyID"></param>
        /// <returns>集体建设用地使用权集合</returns>
        ConstructionLand Get(Guid buildLandPropertyID);

        /// <summary>
        /// 根据地籍号（不含地域全编码及"100"）获取对象
        /// </summary>
        /// <returns></returns> 
        ConstructionLand GetByNumber(string number);

        /// <summary>
        /// 根据权属单位代码获取数据
        /// </summary>
        /// <param name="codeZone">权属单位代码</param>
        /// <returns>目标对象集合</returns>
        List<ContractLand> GetByOwnUnitCode(string ownUnitCode);

        /// <summary>
        /// 根据权属单位代码、获取数据
        /// </summary>
        /// <param name="ownUnitCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>目标对象集合</returns>
        List<ContractLand> GetByOwnUnitCode(string ownUnitCode, eLevelOption levelOption, bool isValid);

        /// <summary>
        /// 根据承包方ID获取集体建设用地使用权集合
        /// </summary>
        /// <param name="familyID">承包方ID</param>
        /// <returns>集体建设用地使用权集合</returns>
        List<ContractLand> GetByFamilyID(Guid familyID);

        /// <summary>
        /// 根据地籍号获取数据
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>目标对象集合</returns>
        List<ContractLand> GetByCadastralNumber(string cadastralNumber);

        /// <summary>
        /// 判断宅基地是否存在
        /// </summary>
        /// <param name="guid">唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        bool Exists(Guid guid);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="yard">宅基地</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        int Update(ConstructionLand yard);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        int Update(Guid houseHolderID, string houseHolderName);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="yard">宅基地</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        int UpdateDataIsNotValidByZoneCode(string zoneCode);

        /// <summary>
        /// 删除宅基地
        /// </summary>
        /// <param name="guid">唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        int Delete(Guid guid);

        /// <summary>
        /// 删除地域代码下所有宅基地
        /// </summary>
        /// <param name="fullCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns> 
        int DeleteByZoneCode(string fullCode);

        /// <summary>
        /// 宗地号是否存在
        /// </summary> 
        bool ExistsToLandNumber(Guid id, string landNumber);

        /// <summary>
        /// 宗地号（不含地域全编码及"100"）是否存在
        /// </summary> 
        bool ExistsByNumber(string number);

        /// <summary>
        /// 根据地籍号获得集体建设用地使用权集合
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>集体建设用地使用权集合</returns>
        List<ContractLand> SearchByNumber(string number, eSearchOption searchOption);

        /// <summary>
        /// 通过承包方名称获得集体建设用地使用权集合
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>集体建设用地使用权集合</returns>
        List<ContractLand> SearchByVirtualPersonName(string name, eSearchOption searchOption);

        /// <summary>
        /// 通过地籍号、区域代码获得集体建设用地使用权集合
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>集体建设用地使用权集合</returns>
        List<ContractLand> SearchByNumber(string number, eSearchOption searchOption, string zoneCode);

        /// <summary>
        /// 通过承包方名称、区域代码获得集体建设用地使用权集合
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>集体建设用地使用权集合</returns>
        List<ContractLand> SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode);
        
        /// <summary>
        /// 获取集体建设用地使用权确权审批表编号最大值
        /// </summary>
        /// <returns>-1（不存在）/int 表中集体建设用地使用权确权审批表编号最大值</returns>
        int GetMaxNumber();

        #endregion
    }
}