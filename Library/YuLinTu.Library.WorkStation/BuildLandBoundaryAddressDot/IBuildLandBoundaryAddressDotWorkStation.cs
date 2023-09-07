/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 集体建设用地使用权界址点的业务逻辑层接口
    /// </summary>
    public interface IBuildLandBoundaryAddressDotWorkStation : IWorkstation<BuildLandBoundaryAddressDot>
    {
        #region Methods

        /// <summary>
        /// 根据标识码获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权界址点对象</returns>
        BuildLandBoundaryAddressDot Get(Guid id);

        /// <summary>
        /// 判断标识码对象是否存在？
        /// </summary>
        /// <param name="id">标识码</param>
        bool Exist(Guid id);

        /// <summary>
        /// 根据集体建设用地使用权ID获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID);

        /// <summary>
        /// 根据承包地块ID获取有效的界址点对象集合
        /// </summary>
        /// <param name="landID">承包地块ID</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID, bool isValid);

        /// <summary>
        /// 根据集体建设用地使用权ID统计集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int CountByLandID(Guid landID);

        /// <summary>
        /// 更新集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权界址点对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(BuildLandBoundaryAddressDot dot);

        /// <summary>
        /// 根据标识码删除集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 根据集体建设用地使用权ID删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="propertyID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByProperty(Guid propertyID);

        /// <summary>
        /// 根据地域代码、土地权属类型获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eSearchOption searchOption, string landType);

        /// <summary>
        /// 根据地域代码及地域匹配级别获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self);

        /// <summary>
        /// 根据地块标识获取界址点集合
        /// </summary>
        List<BuildLandBoundaryAddressDot> GetByLandId(Guid id);

        /// <summary>
        /// 根据地域代码、土地权属类型删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption, string landType);

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus);

        /// <summary>
        /// 根据地块ID删除界址点信息
        /// </summary>
        int DeleteByLandIds(Guid[] ids);

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 按地域、土地权属类型统计集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        ///// <summary>
        ///// 添加界址点
        ///// </summary>  
        //int Add(BuildLandBoundaryAddressDot dot);

        /// <summary>
        /// 批量添加界址点数据
        /// </summary>
        /// <param name="listDot">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<BuildLandBoundaryAddressDot> listDot);

        /// <summary>
        /// 批量更新界址点数据
        /// </summary>
        /// <param name="listDot">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        int UpdateRange(List<BuildLandBoundaryAddressDot> listDot);



        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="dots">待插入保存的点集</param>
        /// <param name="srid">点集的空间参考索引</param>
        void SQLaddDotsIntoSqilite(List<BuildLandBoundaryAddressDot> dots, int srid);

        #endregion
    }
}
