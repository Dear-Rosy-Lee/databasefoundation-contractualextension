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
    /// 集体建设用地使用权界址点的业务逻辑层实现
    /// </summary>
    public class BuildLandBoundaryAddressDotWorkStation : Workstation<BuildLandBoundaryAddressDot>, IBuildLandBoundaryAddressDotWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口
        /// </summary>
        public new IBuildLandBoundaryAddressDotRepository DefaultRepository
        {
            get { return base.DefaultRepository as IBuildLandBoundaryAddressDotRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public BuildLandBoundaryAddressDotWorkStation(IBuildLandBoundaryAddressDotRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据标识码获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权界址点对象</returns>
        public BuildLandBoundaryAddressDot Get(Guid id)
        {
            return DefaultRepository.Get(id);
        }

        /// <summary>
        /// 判断标识码对象是否存在？
        /// </summary>
        /// <param name="id">标识码</param>
        public bool Exist(Guid id)
        {
            return DefaultRepository.Exist(id);
        }

        /// <summary>
        /// 根据集体建设用地使用权ID获取集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID)
        {
            return DefaultRepository.GetByLandID(landID);
        }

        /// <summary>
        /// 根据承包地块ID获取有效的界址点对象集合
        /// </summary>
        /// <param name="landID">承包地块ID</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByLandID(Guid landID, bool isValid)
        {
            return DefaultRepository.GetByLandID(landID, isValid);
        }

        /// <summary>
        /// 根据集体建设用地使用权ID统计集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByLandID(Guid landID)
        {
            return DefaultRepository.CountByLandID(landID);
        }

        /// <summary>
        /// 更新集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权界址点对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(BuildLandBoundaryAddressDot dot)
        {
            DefaultRepository.Update(dot);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据标识码删除集体建设用地使用权界址点对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据集体建设用地使用权ID删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="propertyID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByProperty(Guid propertyID)
        {
            DefaultRepository.DeleteByProperty(propertyID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码、土地权属类型获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eSearchOption searchOption, string landType)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, searchOption, landType);
        }

        /// <summary>
        /// 根据地域代码及地域匹配级别获取集体建设用地使用权界址点对象集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>集体建设用地使用权界址点对象集合</returns>
        public List<BuildLandBoundaryAddressDot> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, levelOption);
        }

        /// <summary>
        /// 根据地块标识获取界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> GetByLandId(Guid id)
        {
            return DefaultRepository.GetByLandId(id);
        }

        /// <summary>
        /// 根据地域代码、土地权属类型删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption, string landType)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption, landType);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, levelOption, virtualStatus);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地块ID删除界址点信息
        /// </summary>
        public int DeleteByLandIds(Guid[] ids)
        {
            DefaultRepository.DeleteByLandIds(ids);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址点数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按地域、土地权属类型统计集体建设用地使用权界址点对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        ///// <summary>
        ///// 添加界址点
        ///// </summary>
        //public int Add(BuildLandBoundaryAddressDot dot)
        //{
        //    DefaultRepository.Add(dot);
        //    return TrySaveChanges(DefaultRepository);
        //}

        /// <summary>
        /// 批量添加界址点数据
        /// </summary>
        /// <param name="listDot">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<BuildLandBoundaryAddressDot> listDot)
        {
            foreach (var dot in listDot)
            {
                DefaultRepository.Add(dot);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量更新界址点数据
        /// </summary>
        /// <param name="listDot">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        public int UpdateRange(List<BuildLandBoundaryAddressDot> listDot)
        {
            foreach (var dot in listDot)
            {
                DefaultRepository.Update(dot);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="dots">待插入保存的点集</param>
        /// <param name="srid">点集的空间参考索引</param>
        public void SQLaddDotsIntoSqilite(List<BuildLandBoundaryAddressDot> dots, int srid)
        {
            DefaultRepository.SQLaddDotsIntoSqilite(dots,srid);
        }


        #endregion

    }
}
