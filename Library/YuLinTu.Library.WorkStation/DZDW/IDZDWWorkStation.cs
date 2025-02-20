/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 集体建设用地点状地物的业务逻辑层接口
    /// </summary>
    public interface IDZDWWorkStation : IWorkstation<DZDW>
    {
        /// <summary>
        /// 根据区域代码获取以标识码排序的点状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>点状地物集合</returns>
        List<DZDW> GetByZoneCode(string zoneCode);

        /// <summary>
        /// 批量添加点状地物数据
        /// </summary>
        /// <param name="listPoint">点状地物对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<DZDW> listPoint);
    }
}
