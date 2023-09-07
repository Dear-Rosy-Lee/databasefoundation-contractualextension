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
    /// 集体建设用地使用权界址线的业务逻辑层接口
    /// </summary>
    public interface IBuildLandBoundaryAddressCoilWorkStation : IWorkstation<BuildLandBoundaryAddressCoil>
    {
        #region Methods

        /// <summary>
        /// 获取界址线对象
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>对象</returns>
        BuildLandBoundaryAddressCoil Get(Guid id);

        /// <summary>
        /// 检查是否存在指定id的权证对象
        /// </summary>
        /// <param name="id">id</param>
        bool Exist(Guid id);

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象集合
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>对象集合</returns>
        List<BuildLandBoundaryAddressCoil> GetByLandID(Guid landID);

        /// <summary>
        /// 根据地域获取数据
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eSearchOption searchOption, eLandPropertyRightType landType);

        /// <summary>
        /// 根据地域获取数据
        /// </summary>
        List<BuildLandBoundaryAddressCoil> GetByZoneCode(string zoneCode, eLevelOption levelOption = eLevelOption.Self);

        /// <summary>
        /// 根据地块标识获取界址线集合
        /// </summary>
        List<BuildLandBoundaryAddressCoil> GetByLandId(Guid id);

        /// <summary>
        /// 通过集体建设用地使用权ID获取权证对象数量
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        int CountByLandID(Guid landID);

        /// <summary>
        /// 更新权证
        /// </summary>
        /// <param name="coil">新的值</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        int Update(BuildLandBoundaryAddressCoil coil);

        /// <summary>
        /// 鹿泉的更新逻辑
        /// </summary>
        /// <param name="coil"></param>
        /// <param name="isInitQlrAndJzxWZ"></param>
        /// <returns></returns>
        int UpdateLuYi(BuildLandBoundaryAddressCoil coil, bool isInitQlrAndJzxWZ = false);

        /// <summary>
        /// 删除权证对象
        /// </summary>
        /// <param name="ID">权证对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 通过属性ID删除权证对象
        /// </summary>
        /// <param name="ID">对象ID</param>
        /// <returns>-1(错误)/0(失败)/1(成功)</returns>
        int DeleteByProperty(Guid propertyID);

        /// <summary>
        /// 根据地域代码删除数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption, string landType);

        /// <summary>
        /// 根据地域代码删除集体建设用地使用权界址线对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址线数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus);

        /// <summary>
        /// 根据地块ID删除界址线信息
        /// </summary>
        int DeleteByLandIds(Guid[] ids);

        /// <summary>
        /// 根据地域代码删除指定承包方状态的界址线数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="virtualStatus">对应承包方状态</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功))</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="landType">土地权属类型</param>
        /// <returns>-1(参数错误)/int(数量))</returns>
        int Count(string zoneCode, eLevelOption searchOption);


        /// <summary>
        /// 批量添加界址线数据
        /// </summary>
        /// <param name="listCoil">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<BuildLandBoundaryAddressCoil> listCoil);

        /// <summary>
        /// 批量更新界址线数据
        /// </summary>
        /// <param name="listDot">界址点对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        int UpdateRange(List<BuildLandBoundaryAddressCoil> listCoil);

        /// <summary>
        /// 批量添加界址点线
        /// </summary>
        /// <param name="coils"></param>
        /// <param name="dots"></param>
        /// <returns></returns>
        int AddDotCoilList(List<BuildLandBoundaryAddressCoil> coils, List<BuildLandBoundaryAddressDot> dots);
        
        /// <summary>
        /// 按照SQL语句在数据访问层中插入批量的界址数据
        /// </summary>
        /// <param name="coils">待插入保存的线集</param>
        /// <param name="srid">点集的空间参考索引</param>
        void SQLaddCoilsIntoSqilite(List<BuildLandBoundaryAddressCoil> coils, int srid);

        /// <summary>
        /// 批量修改界址线类别
        /// </summary>      
        int UpdateCoilCollectionType(List<Guid> coilids, string coilType);

        #endregion
    }
}
