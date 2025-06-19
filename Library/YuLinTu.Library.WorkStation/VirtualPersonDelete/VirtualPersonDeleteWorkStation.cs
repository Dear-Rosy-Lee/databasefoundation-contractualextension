using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    public class VirtualPersonDeleteWorkStation : Workstation<VirtualPerson_Del>, IVirtualPersonDeleteWorkStation
    {
        #region Properties

        public new IVirtualPersonDeleteRepository DefaultRepository
        {
            get { return base.DefaultRepository as IVirtualPersonDeleteRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion Properties

        #region Ctor

        public VirtualPersonDeleteWorkStation(IVirtualPersonDeleteRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion Ctor

        #region Methods
        /// <summary>
        /// 删除当前地域下所有数据
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域代码及其查找
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        public List<VirtualPerson_Del> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, searchOption);
        }
        #endregion Methods
    }
}
