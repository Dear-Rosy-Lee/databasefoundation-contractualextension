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
using YuLinTu.Spatial;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包台账地块接口定义
    /// </summary>
    public interface IContractLandMarkWorkStation : IWorkstation<ContractLandMark>
    {
        #region Methods

        /// <summary>
        /// 获取对象
        /// </summary>
        ContractLand Get(Guid guid);   

        /// <summary>
        /// 批量添加承包地块数据
        /// </summary>
        /// <param name="listLand">承包地块对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<ContractLandMark> listLand);
        /// <summary>
        /// 按地域获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param na承包台账me="searchOption">匹配等级</param>
        /// <returns>承包台账地块集合</returns>
        List<ContractLandMark> GetCollection(string zoneCode, eLevelOption searchOption);
        int Update(ContractLandMark entity);
        int DeleteByZoneCode(string zoneCode);
        #endregion

    }
}
