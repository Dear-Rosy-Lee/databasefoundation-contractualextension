using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public class VirtualPersonDeleteRepository : RepositoryDbContext<VirtualPerson_Del>, IVirtualPersonDeleteRepository
    {
        private IDataSourceSchema m_DSSchema = null;

        public VirtualPersonDeleteRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        /// <summary>
        /// 删除当前地域下所有数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string code, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return -1;
            int cnt = 0;
            if (levelOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(code));
            else if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.ZoneCode.Equals(code));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(code) && c.ZoneCode != code);
            return cnt;
        }

        /// <summary>
        /// 根据地域代码及其查找
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        public List<VirtualPerson_Del> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {

            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }
            List<VirtualPerson_Del> entity = null;
            if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.ZoneCode.Contains(zoneCode));
            else
                entity = Get(c => c.ZoneCode.Equals(zoneCode));
            return entity;
        }
    }
}
