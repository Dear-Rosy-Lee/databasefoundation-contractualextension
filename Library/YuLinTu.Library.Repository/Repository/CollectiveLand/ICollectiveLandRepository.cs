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
    /// 集体土地所有权的数据访问接口
    /// </summary>
    public interface ICollectiveLandRepository :   IRepository<CollectiveLand>
    {

        /// <summary>
        /// 删除与目标标识码相同的集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLandID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid collectiveLandID);

        /// <summary>
        /// 删除与目标权属单位代码相同的集体土地所有权对象
        /// </summary>
        /// <param name="ownUnitCode">权属单位代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 更新集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLand">集体土地所有权对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(CollectiveLand collectiveLand);

        /// <summary>
        /// 根据承包方ID更新其土地所有权人名称
        /// </summary>
        /// <param name="houseHolderID">承包方ID</param>
        /// <param name="houseHolderName">土地所有权人</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Guid houseHolderID, string houseHolderName);

        /// <summary>
        /// 获得与目标标识码相同的集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLandID">标识码</param>
        /// <returns>集体土地所有权</returns>
        CollectiveLand Get(Guid collectiveLandID);

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>集体土地所有权对象集合</returns>
        CollectiveLand GetByZoneCode(string zoneCode);

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>以权属单位代码排序的集体土地所有权对象集合</returns>
        List<CollectiveLand> GetByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据地号获得集体土地所有权对象
        /// </summary>
        /// <param name="landNumber">地号</param>
        /// <returns>以地号排序的集体土地所有权对象集合</returns>
        List<CollectiveLand> GetByLandNumber(string landNumber);

        /// <summary>
        /// 根据图号获得集体土地所有权对象
        /// </summary>
        /// <param name="imageNumber">图号</param>
        /// <returns>集体土地所有权对象集合</returns>
        List<CollectiveLand> GetByImageNumber(string ImageNumber);

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象的统计信息
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>统计信息</returns>
        CollectiveLandAreaInfo CountAreaByZoneCode(string zoneCode, eLevelOption levelOption);
    }
}
