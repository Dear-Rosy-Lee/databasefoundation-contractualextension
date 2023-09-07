using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Data;

namespace YuLinTu.Library.WorkStation
{
    public class StockWarrantWorkStation : Workstation<StockWarrant>, IStockWarrantWorkStation
    {
        public new IStockWarrantRespository DefaultRepository
        {
            get { return base.DefaultRepository as IStockWarrantRespository; }
            set { base.DefaultRepository = value; }
        }

        public StockWarrantWorkStation(IStockWarrantRespository rep)
        {
            DefaultRepository = rep;
        }

        public int Add(List<StockWarrant> warrants)
        {
            Delete(o => o.ZoneCode == warrants.FirstOrDefault().ZoneCode);
            foreach (var w in warrants)
            {
                Add(w);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据行政区获取数据
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public List<StockWarrant> GetByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            List<StockWarrant> entity = new List<StockWarrant>();
            if (searchOption == eLevelOption.Self)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode.Equals(zoneCode));
            }
            else if (searchOption == eLevelOption.Subs)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode));
            }
            else if (searchOption == eLevelOption.SelfAndSubs)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode.StartsWith(zoneCode));
            }

            return entity;
        }

        /// <summary>
        /// 删除行政区下数据
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public int DeleteByZone(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            if (searchOption == eLevelOption.Self)
                DefaultRepository.Delete(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                DefaultRepository.Delete(c => c.ZoneCode.StartsWith(zoneCode));
            else
                DefaultRepository.Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量添加权属关系数据
        /// </summary>
        /// <param name="listRelation"></param>
        /// <returns></returns>
        public int AddRang(List<StockWarrant> listRelation)
        {
            if (listRelation == null || listRelation.Count == 0)
                return 0;
            listRelation.ForEach(t => DefaultRepository.Add(t));
            return TrySaveChanges(DefaultRepository);
        }

        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        public int Update(StockWarrant entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }
    }
}