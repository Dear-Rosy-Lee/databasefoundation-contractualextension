/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
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
    /// 二轮台账地块的数据访问接口
    /// </summary>
    public interface ISecondTableLandRepository : IAgricultureLandRepository<SecondTableLand>
    {
    }
}