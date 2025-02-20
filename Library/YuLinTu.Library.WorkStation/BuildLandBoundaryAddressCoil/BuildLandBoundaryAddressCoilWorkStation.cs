/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 集体建设用地使用权界址线的业务逻辑层实现
    /// </summary>
    public class BuildLandBoundaryAddressCoilWorkStation : Workstation<BuildLandBoundaryAddressCoil>, IBuildLandBoundaryAddressCoilWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口
        /// </summary>
        public new IBuildLandBoundaryAddressCoilRepository DefaultRepository
        {
            get { return base.DefaultRepository as IBuildLandBoundaryAddressCoilRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public BuildLandBoundaryAddressCoilWorkStation(IBuildLandBoundaryAddressCoilRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取权证对象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>对象</returns>
        public BuildLandBoundaryAddressCoil Get(Guid id)
        {
            return DefaultRepository.Get(id);
        }

        /// <summary>
        /// 检查是否存在指定id的权证对象
        /// </summary>
        /// <param name="id">id</param>
        public bool Exist(Guid id)
        {
            return DefaultRepository.Exist(id);
        }

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象集合
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>对象集合</returns>
        public List<BuildLandBoundaryAddressCoil> GetByLandID(Guid landID)
        {
            return DefaultRepository.GetByLandID(landID);
        }

        /// <summary>
        /// 根据地域获取数据
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eSearchOption searchOption, eLandPropertyRightType landType)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, searchOption, landType);
        }

        /// <summary>
        /// 根据地域获取数据
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, levelOption);
        }

        /// <summary>
        /// 根据地块标识获取界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> GetByLandId(Guid id)
        {
            return DefaultRepository.GetByLandId(id);
        }

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        public int CountByLandID(Guid landID)
        {
            return DefaultRepository.CountByLandID(landID);
        }

        /// <summary>
        /// 更新权证
        /// </summary>
        /// <param name="coil">新的值</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int Update(BuildLandBoundaryAddressCoil coil)
        {
            DefaultRepository.Update(coil);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 鹿泉的更新逻辑
        /// </summary>
        /// <param name="coil"></param>
        /// <param name="isInitQlrAndJzxWZ"></param>
        /// <returns></returns>
        public int UpdateLuYi(BuildLandBoundaryAddressCoil coil, bool isInitQlrAndJzxWZ = false)
        {
            DefaultRepository.UpdateLuYi(coil, isInitQlrAndJzxWZ);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除权证对象
        /// </summary>
        /// <param name="ID">权证对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int Delete(Guid ID)
        {
            DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 通过属性ID删除权证对象
        /// </summary>
        /// <param name="ID">对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        public int DeleteByProperty(Guid propertyID)
        {
            DefaultRepository.DeleteByProperty(propertyID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption, string landType)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption, landType);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址线对象
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
        /// 根据地域代码删除指定承包方状态的界址线数据
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
        /// 根据地块ID删除界址线信息
        /// </summary>
        public int DeleteByLandIds(Guid[] ids)
        {
            DefaultRepository.DeleteByLandIds(ids);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址线数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        /// <summary>
        /// 批量添加界址线数据
        /// </summary>
        /// <param name="listCoil">界址线对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<BuildLandBoundaryAddressCoil> listCoil)
        {
            foreach (var coil in listCoil)
            {
                DefaultRepository.Add(coil);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量更新界址线数据
        /// </summary>
        /// <param name="listDot">界址线对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        public int UpdateRange(List<BuildLandBoundaryAddressCoil> listCoil)
        {
            foreach (var coil in listCoil)
            {
                DefaultRepository.Update(coil);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量添加界址点线
        /// </summary>
        /// <param name="coils"></param>
        /// <param name="dots"></param>
        /// <returns></returns>
        public int AddDotCoilList(List<BuildLandBoundaryAddressCoil> coils, List<BuildLandBoundaryAddressDot> dots)
        {
            DefaultRepository.AddDotCoilList(coils, dots);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="coils">待插入保存的线集</param>
        /// <param name="srid">点集的空间参考索引</param>
        public void SQLaddCoilsIntoSqilite(List<BuildLandBoundaryAddressCoil> coils, int srid)
        {
            DefaultRepository.SQLaddCoilsIntoSqilite(coils,srid);
        }

        /// <summary>
        /// 批量修改界址线类别
        /// </summary>
        public int UpdateCoilCollectionType(List<Guid> coilids, string coilType)
        {
            DefaultRepository.UpdateCoilCollectionType(coilids, coilType);
            return TrySaveChanges(DefaultRepository);
        }

        #endregion
    }
}
