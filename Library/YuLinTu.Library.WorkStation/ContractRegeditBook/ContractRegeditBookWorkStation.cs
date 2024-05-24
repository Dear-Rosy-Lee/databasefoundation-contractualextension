/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using static YuLinTu.tGISCNet.PointAreaRelationCheck;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包权证业务逻辑层接口实现
    /// </summary>
    public class ContractRegeditBookWorkStation : Workstation<ContractRegeditBook>, IContractRegeditBookWorkStation
    {
        #region Property

        /// <summary>
        /// 默认数据访问接口属性
        /// </summary>
        public new IContractRegeditBookRepository DefaultRepository
        {
            get { return base.DefaultRepository as IContractRegeditBookRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rep"></param>
        public ContractRegeditBookWorkStation(IContractRegeditBookRepository rep)
        {
            this.DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 添加承包权证
        /// </summary>
        /// <param name="book">权证对象</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功)</returns>
        public int AddRegeditBook(ContractRegeditBook book)
        {
            DefaultRepository.Add(book);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量添加承包权证数据
        /// </summary>
        /// <param name="listBook">承包权证对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ContractRegeditBook> listBook)
        {
            int addCount = 0;
            if (listBook == null || listBook.Count == 0)
            {
                return addCount;
            }
            foreach (var book in listBook)
            {
                DefaultRepository.Add(book);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="guid">承包权证ID号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            DefaultRepository.Delete(guid);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据权证ID集合删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="lstGuids">权证(合同)ID标识集合</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(List<Guid> lstGuids)
        {
            foreach (var id in lstGuids)
            {
                DefaultRepository.Delete(id);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByRegeditNumber(string number)
        {
            DefaultRepository.DeleteByRegeditNumber(number);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 更新农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="entity">权证对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractRegeditBook entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }

        public int UpdataList(List<ContractRegeditBook> listBook)
        {
            foreach (var item in listBook)
            {
                DefaultRepository.Update(item);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据ID获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="guid">承包权证ID号</param>
        /// <returns>承包权证对象</returns>
        public ContractRegeditBook Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 根据合同id集合获取权证集合
        /// </summary>
        public List<ContractRegeditBook> GetByConcordIds(Guid[] ids)
        {
            return DefaultRepository.GetByConcordIds(ids);
        }

        /// <summary>
        /// 根据ID判断农村土地承包经营权登记薄对象是否存在
        /// </summary>
        /// <param name="guid">ID</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            return DefaultRepository.Exists(guid);
        }

        /// <summary>
        /// 通过登记薄编号查看是否存在有权证号相同但Guid不同的存在。
        /// </summary>
        /// <param name="concordNumber">登记薄编号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(string regeditNumber)
        {
            return DefaultRepository.Exists(regeditNumber);
        }

        /// <summary>
        /// 通过登记薄编号获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="regeditNumber">登记薄编号</param>
        /// <returns>权证对象</returns>
        public ContractRegeditBook Get(string regeditNumber)
        {
            return DefaultRepository.Get(regeditNumber);
        }

        /// <summary>
        /// 根据权证流水号及其查找类型获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证流水号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营权登记薄对象</returns>
        public ContractRegeditBook GetByNumber(string number, eSearchOption searchOption)
        {
            return DefaultRepository.GetByNumber(number, searchOption);
        }

        /// <summary>
        /// 根据地域代码及其查找类型获取权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        public List<ContractRegeditBook> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            return DefaultRepository.GetByZoneCode(zoneCode, searchOption);
        }

        /// <summary>
        /// 根据权证号获取及其查找类型获取权证
        /// </summary>
        /// <param name="number">权证号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        public List<ContractRegeditBook> GetCollection(string number, eSearchOption searchOption)
        {
            return DefaultRepository.GetCollection(number, searchOption);
        }

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包权证集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">权证级别</param>
        /// <returns>农村土地承包权证集合</returns>
        public List<ContractRegeditBook> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetContractsByZoneCode(zoneCode, searchOption);
        }

        /// <summary>
        /// 根据地域代码及其查找类型删除权证
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
        /// 删除当前地域下所有指定承包方状态的权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, levelOption, virtualStatus);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的权证
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
        /// 按地域及其匹配类型统计权证数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配类型</param>
        /// <returns>-1（参数错误）/int 权证数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        /// <summary>
        /// 存在权证的地域集合
        /// </summary>
        /// <param name="zoneList">地域集合</param>
        public List<Zone> ExistZones(List<Zone> zoneList)
        {
            return DefaultRepository.ExistZones(zoneList);
        }

        /// <summary>
        /// 获取最新的权证流水号
        /// </summary>
        public string GetNewSerialNumber(int minNumebr, int maxNumber)
        {
            return DefaultRepository.GetNewSerialNumber(minNumebr, maxNumber);
        }
        /// <summary>
        /// 获取最大流水号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSerialNumber()
        {
            return DefaultRepository.GetMaxSerialNumber();
        }

        /// <summary>
        /// 在整库中获取最大流水号
        /// </summary>
        public int GetMaxSerialNumber1()
        {
            return DefaultRepository.GetMaxSerialNumber1();
        }

        #endregion
    }
}
