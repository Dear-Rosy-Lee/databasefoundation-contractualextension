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
    /// 集体建设用地调查宗地的业务逻辑层接口
    /// </summary>
    public interface IXZDWWorkStation : IWorkstation<XZDW>
    {
        /// <summary>
        /// 根据区域代码获取以标识码排序的线状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>线状地物集合</returns>
        List<XZDW> GetByZoneCode(string zoneCode);

        /// <summary>
        /// 批量添加线状地物数据
        /// </summary>
        /// <param name="listLine">线状地物对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<XZDW> listLine);
    }
}
