using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Data;

namespace YuLinTu.Library.WorkStation
{
    public  class StockConcordWorkStation :Workstation<StockConcord>,IStockConcordWorkStation
    {
        public new IStockConcordRespository DefaultRepository
        {
            get { return base.DefaultRepository as IStockConcordRespository; }
            set { base.DefaultRepository = value; }
        }

        public IStockWarrantRespository WarrantRepository { get; set; }


        public StockConcordWorkStation(IStockConcordRespository rep, IStockWarrantRespository warrantRep)
        {
            DefaultRepository = rep;
            WarrantRepository = warrantRep;
        }

        public  int Add(List<StockConcord> concords)
        {
            Delete(o => o.ZoneCode == concords.FirstOrDefault().ZoneCode);
            WarrantRepository.Delete(o => o.ZoneCode == concords.FirstOrDefault().ZoneCode);
            foreach (var c in concords)
            {
                Add(c);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据行政区获取数据
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public List<StockConcord> GetByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            List<StockConcord> entity = new List<StockConcord>();
            if (searchOption == eLevelOption.Self)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
            }
            else if (searchOption == eLevelOption.Subs)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
            }
            else if (searchOption == eLevelOption.SelfAndSubs)
            {
                entity = DefaultRepository.Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
            }

            return entity;
        }

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<StockConcord> GetStockByFamilyID(Guid guid)
        {
            if (guid == null || guid == Guid.Empty)
                return null;

            object entity = Get(c => c.ContracterId.Equals(guid));
            return entity as List<StockConcord>;
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
        public int AddRang(List<StockConcord> listRelation)
        {
            if (listRelation == null || listRelation.Count == 0)
                return 0;
            listRelation.ForEach(t => DefaultRepository.Add(t));
            return TrySaveChanges(DefaultRepository);
        }
    }
}
