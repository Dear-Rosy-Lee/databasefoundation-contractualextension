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
    /// 集体建设用地面状地物的数据访问接口
    /// </summary>
    public interface IMZDWRepository : IRepository<MZDW>
    {
        /// <summary>
        /// 根据区域代码获取以标识码排序的面状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>点状地物集合</returns>
        List<MZDW> GetByZoneCode(string zoneCode);
    }
}
