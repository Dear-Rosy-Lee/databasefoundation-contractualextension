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
using YuLinTu.Spatial;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包台账地块类定义
    /// </summary>
    public class ContractLandMarkWorkStation : Workstation<ContractLandMark>, IContractLandMarkWorkStation
    {
        #region Properties

        /// <summary>
        /// 承包地数据访问层
        /// </summary>
        public new IContractLandMarkRepository DefaultRepository
        {
            get { return base.DefaultRepository as IContractLandMarkRepository; }
            set { base.DefaultRepository = value; }
        }
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandMarkWorkStation(IContractLandMarkRepository rep)
        {
            DefaultRepository = rep;           
        }

        #endregion

        #region Methods 

        /// <summary>
        /// 获取对象
        /// </summary>
        public ContractLand Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 批量添加承包地块数据
        /// </summary>
        /// <param name="listLand">承包地块对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ContractLandMark> listLand)
        {
            int addCount = 0;
            if (listLand == null || listLand.Count == 0)
            {
                return addCount;
            }
            foreach (var land in listLand)
            {
                DefaultRepository.Add(land);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }
        /// <summary>
        /// 按地域获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param na承包台账me="searchOption">匹配等级</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLandMark> GetCollection(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetCollection(zoneCode, searchOption);
        }
        /// <summary>
        /// 更新承包台账地块对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractLandMark entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }
        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode);
            return TrySaveChanges(DefaultRepository);
        }
        #endregion   
    }
}
