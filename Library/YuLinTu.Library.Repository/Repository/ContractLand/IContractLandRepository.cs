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
    public interface IContractLandRepository :  IAgricultureLandRepository<ContractLand>
    {
        /// <summary>
        /// 跟新地块Shape数据
        /// </summary>
        /// <param name="entity">地块实体对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int UpdateShape(ContractLand entity);
    }
}