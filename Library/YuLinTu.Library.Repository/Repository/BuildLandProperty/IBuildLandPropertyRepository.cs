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
    public interface IBuildLandPropertyRepository :   IRepository<BuildLandProperty>
    {
        #region Methods

        /// <summary>
        /// 根据地域全代码获取对象
        /// </summary>
        /// <param name="fullCode">地域全代码</param>
        /// <returns></returns>
        BuildLandPropertyCollection Get(string fullCode);

        int CountByZoneCode(string zoneCode);

        /// <summary>
        /// 根据ID获取对象
        /// </summary>
        /// <param name="buildLandPropertyID">ID</param>
        /// <returns></returns>
        BuildLandProperty Get(Guid buildLandPropertyID);

        /// <summary>
        /// 根据地籍号（不含地域全编码及"100"）获取对象
        /// </summary>
        /// <returns></returns>
        BuildLandProperty GetByNumber(string number);

        /// <summary>
        /// 根据权属单位代码获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="codeZone">权属单位代码</param>
        /// <returns>根据承包方名称排序的集体建设用地使用权对象</returns>
        BuildLandPropertyCollection GetByOwnUnitCode(string ownUnitCode);

        /// <summary>
        /// 根据权属单位代码、是否可用获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="ownUnitCode">根据权属单位代码</param>
        /// <param name="levelOption">匹配级别</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        BuildLandPropertyCollection GetByOwnUnitCode(string ownUnitCode, eLevelOption levelOption, bool isValid);

        /// <summary>
        /// 根据承包方ID获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="familyID">承包方ID</param>
        /// <returns>目标对象集合</returns>
        BuildLandPropertyCollection GetByFamilyID(Guid familyID);

        /// <summary>
        /// 根据权属单位代码、户主名称获取地块
        /// </summary>
        ///<param name="zoneCode">根据权属单位代码</param>
        /// <param name="householderName">承包方名称</param>
        /// <returns>根据地籍号排序的目标对象集合</returns>
        BuildLandPropertyCollection GetByHouseholderName(string zoneCode, string householderName);

        /// <summary>
        /// 根据地籍号获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>目标对象集合</returns>
        BuildLandPropertyCollection GetByCadastralNumber(string cadastralNumber);

        /// <summary>
        /// 判断宅基地是否存在
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(Guid guid);

        /// <summary>
        /// 更新集体建设用地使用权对象数据
        /// </summary>
        /// <param name="yard">集体建设用地使用权对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(BuildLandProperty yard);

        /// <summary>
        /// 更新户主名称
        /// </summary>
        /// <param name="houseHolderID">承包方ID</param>
        /// <param name="houseHolderName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Guid houseHolderID, string houseHolderName);

        /// <summary>
        /// 根据权属单位代码更新集体建设用地使用权对象数据
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int UpdateDataIsNotValidByZoneCode(string zoneCode);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 删除权属单位代码下所有集体建设用地使用权对象
        /// </summary>
        /// <param name="fullCode">权属单位代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string fullCode);

        /// <summary>
        /// 根据标识码与宗地号判断是否存在集体建设用地使用权对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <param name="landNumber">宗地号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistsToLandNumber(Guid id, string landNumber);

        /// <summary>
        /// 是否存在以末尾包含目标地籍号的集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistsByNumber(string number);

        /// <summary>
        /// 通过地籍号查找集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        BuildLandPropertyCollection SearchByNumber(string number, eSearchOption searchOption);

        /// <summary>
        /// 通过承包方名称查找集体建设用地使用权对象
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        BuildLandPropertyCollection SearchByVirtualPersonName(string name, eSearchOption searchOption);

        /// <summary>
        /// 通过地籍号、权属单位代码查找集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchType">查找类型（精确：地籍号右相似、权属单位代码左相似；模糊：地籍号包含、权属单位代码左相似）</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        BuildLandPropertyCollection SearchByNumber(string number, eSearchOption searchOption, string zoneCode);

        /// <summary>
        /// 通过承包方名称、权属单位代码查找集体建设用地使用权对象
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchType">查找类型（精确：承包方名称相同、权属单位代码左相似；模糊：承包方名称包含、权属单位代码左相似）</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        BuildLandPropertyCollection SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode);

        /// <summary>
        /// 获取集体建设用地使用权确权审批表编号的最大值
        /// </summary>
        /// <returns>编号的最大值</returns>       
        int GetMaxNumber();

        #endregion
    }
}