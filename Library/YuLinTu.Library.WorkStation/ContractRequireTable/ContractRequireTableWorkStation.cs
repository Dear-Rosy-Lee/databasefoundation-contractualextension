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

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 农村土地承包经营申请登记表业务逻辑层接口实现
    /// </summary>
    public class ContractRequireTableWorkStation : Workstation<ContractRequireTable>, IContractRequireTableWorkStation
    {
        #region Property

        /// <summary>
        /// 默认数据访问接口属性
        /// </summary>
        public new IContractRequireTableRepository DefaultRepository
        {
            get { return base.DefaultRepository as IContractRequireTableRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rep"></param>
        public ContractRequireTableWorkStation(IContractRequireTableRepository rep)
        {
            this.DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据ID删除农村土地承包经营申请登记表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            DefaultRepository.Delete(guid);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 更新农村土地承包经营申请登记表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractRequireTable entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据ID获取农村土地承包经营申请登记表对象
        /// </summary>
        public ContractRequireTable Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 根据ID判断农村土地承包经营申请登记表对象是否存在
        /// </summary>
        public bool Exists(Guid guid)
        {
            return DefaultRepository.Exists(guid);
        }

        /// <summary>
        /// 根据申请书编号获取农村土地承包经营申请登记表对象
        /// </summary>
        /// <param name="tabNumber">申请书编号</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        public ContractRequireTable Get(string tabNumber)
        {
            return DefaultRepository.Get(tabNumber);
        }

        /// <summary>
        /// 根据组织编码获得以申请书编号排序农村土地承包经营申请登记表
        /// </summary>
        /// <param name="tissueCode">组织编码</param>
        /// <returns>申请登记表</returns>
        public List<ContractRequireTable> GetTissueRequireTable(string tissueCode)
        {
            return DefaultRepository.GetTissueRequireTable(tissueCode);
        }


        /// <summary>
        /// 根据地域代码获取农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        public List<ContractRequireTable> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, searchOption);
        }

        /// <summary>
        /// 根据地域代码删除农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按地域统计农村土地承包经营申请登记表的数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        /// <summary>
        /// 批量添加农村土地承包经营申请登记表数据
        /// </summary>
        /// <param name="listRequireTable">农村土地承包经营申请登记表对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ContractRequireTable> listRequireTable)
        {
            int addCount = 0;
            if (listRequireTable == null || listRequireTable.Count == 0)
            {
                return addCount;
            }
            foreach (var reauireTable in listRequireTable)
            {
                DefaultRepository.Add(reauireTable);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumber()
        {
            return DefaultRepository.GetMaxNumber();
        }

        #endregion
    }
}
