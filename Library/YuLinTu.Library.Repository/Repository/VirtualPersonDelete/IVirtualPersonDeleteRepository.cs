using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public interface IVirtualPersonDeleteRepository : IRepository<VirtualPerson_Del>
    {
        /// <summary>
        /// 删除当前地域下所有数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据地域代码及其查找
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        List<VirtualPerson_Del> GetByZoneCode(string zoneCode, eSearchOption searchOption);
    }
}
