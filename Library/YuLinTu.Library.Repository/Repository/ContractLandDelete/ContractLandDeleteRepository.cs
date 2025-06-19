using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public class ContractLandDeleteRepository : RepositoryDbContext<ContractLand_Del>, IContractLandDeleteRepository
    {
  

        private IDataSourceSchema m_DSSchema = null;

        public ContractLandDeleteRepository(IDataSource ds)
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
                cnt = Delete(c => c.DYBM.StartsWith(code));
            else if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.DYBM.Equals(code));
            else
                cnt = Delete(c => c.DYBM.StartsWith(code) && c.DYBM != code);
            return cnt;
        }

        public List<ContractLand_Del> GetDelLandByZone(string zoneCode)
        {
            var entity = Get(c => c.DYBM.StartsWith(zoneCode));
            return entity.ToList();
        }

    }
}
