using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    public class BelongRelationWorkStation :Workstation<BelongRelation>,IBelongRelationWorkStation
    {
        public new IBelongRelationRespository DefaultRepository
        {
            get { return base.DefaultRepository as IBelongRelationRespository; }
            set { base.DefaultRepository = value; }
        }
        public IContractLandRepository LandRepository { get; set; }

        public IVirtualPersonRepository<LandVirtualPerson> PersonRepository { get; set; }

        public BelongRelationWorkStation(IBelongRelationRespository rep, IContractLandRepository landRep, IVirtualPersonRepository<LandVirtualPerson> personRep)
        {
            DefaultRepository = rep;
            LandRepository = landRep;
            PersonRepository = personRep;
        }
        /// <summary>
        /// 得到行政区下数据
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public List<BelongRelation> GetdDataByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            List<BelongRelation> relation = new List<BelongRelation>();
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return relation;
            if (searchOption == eLevelOption.Self)
                relation = DefaultRepository.Get(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                relation = DefaultRepository.Get(c => c.ZoneCode.StartsWith(zoneCode));
            else
                relation = DefaultRepository.Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return relation;
        }

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
        public int AddRang(List<BelongRelation> listRelation)
        {
            if (listRelation == null || listRelation.Count == 0)
                return 0;
            listRelation.ForEach(t => DefaultRepository.Add(t));
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除地域下所有股权关系，并且确股地块的共用面积归0
        /// </summary>
        /// <param name="relations"></param>
        /// <returns></returns>
        public int DeleteRelations(string zoneCode)
        {
            Delete(o => o.ZoneCode == zoneCode);
            var lands = LandRepository.Get(o => o.ZoneCode == zoneCode && o.IsStockLand);
            lands.ForEach(o =>
            {
                o.ShareArea = "0";
                LandRepository.Update(o);
            });
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除确股地，并联动删除权属关系
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public int Deleteland(string zoneCode)
        {
            var lands = LandRepository.Get(o => o.ZoneCode == zoneCode && o.IsStockLand);
            lands.ForEach(o => 
            {
                LandRepository.Delete(o.ID);
                Delete(r => r.LandID == o.ID);
            });
            return TrySaveChanges(DefaultRepository);
            
        }

        /// <summary>
        /// 删除指定的确股地，并联动删除权属关系
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        public int DeleteLands(List<ContractLand> lands)
        {
            List<ContractLand> delLands = new List<ContractLand>();
            ContractLand landTemp = null;
            foreach (var land in lands)
            {
                landTemp = LandRepository.Get(o => o.LandNumber == land.LandNumber).FirstOrDefault();
                if (landTemp != null)
                {
                    delLands.Add(landTemp);
                }

            }
            if (delLands.Count != 0)
            {
                delLands.ForEach(o =>
                {
                    LandRepository.Delete(o.ID);
                    Delete(r => r.LandID == o.ID);
                });
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据人获取当前人下的地
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public List<ContractLand> GetLandByPerson(Guid personId,string zoneCode)
        {
            List<ContractLand> lands=new List<ContractLand>();
            var relations = Get(o => o.VirtualPersonID == personId);
            relations.ForEach(o => 
            {
                var land = LandRepository.Get(o.LandID);
                if (land != null)
                {
                    land.Comment ="共有宗地总面积:"+ LandRepository.Get(l => l.ZoneCode == zoneCode && l.IsStockLand).Sum(landEx=> landEx.ActualArea);
                    lands.Add(land);
                }
            });
            return lands;
        }

        /// <summary>
        /// 根据人获取当前人下的地，并且不修改备注
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public List<ContractLand> GetNotCommentLandByPerson(Guid personId, string zoneCode)
        {
            List<ContractLand> lands = new List<ContractLand>();
            var relations = Get(o => o.VirtualPersonID == personId);
            relations.ForEach(o =>
            {
                var land = LandRepository.Get(o.LandID);
                if (land != null)
                {
                    lands.Add(land);
                }
            });
            return lands;
        }

        public List<PersonStockLand> GetPersonStockLand(string zoneCode)
        {
            var personStockLands = new List<PersonStockLand>();
            var stockPersons=PersonRepository.Get(o => o.ZoneCode == zoneCode && o.IsStockFarmer);
            foreach (var p in stockPersons)
            {
                var stockLands = GetLandByPerson(p.ID, zoneCode);
                if (stockLands.Count > 0)
                {
                    var personStockLand = new PersonStockLand();
                    personStockLand.Contractor = p;
                    personStockLand.StockLands = stockLands;
                    personStockLands.Add(personStockLand);
                }
            }
            return personStockLands;
        }


        public void DataBaseExchange(string zoneCode)
        {
            SetPerson(zoneCode);
            SetRelation(zoneCode);
            CaculateQuanfication(zoneCode);
            AddNewLand(zoneCode);
            DeleteOldLand(zoneCode);
            TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 1 将人设置为股农
        /// </summary>
        private void SetPerson(string zoneCode)
        {
            var personIds=LandRepository.Get(o => o.IsStockLand&&o.ZoneCode==zoneCode).Select(o=>o.OwnerId).Distinct();
            foreach (var id in personIds)
            {
                var p = PersonRepository.Get(o => o.ID == id).FirstOrDefault();
                p.IsStockFarmer = true;
                PersonRepository.Update(p);
            }
        }

        /// <summary>
        /// 2 设置关系
        /// </summary>
        /// <param name="zoneCode"></param>
        private void SetRelation(string zoneCode)
        {
            var stockLands = LandRepository.Get(o => o.IsStockLand && o.ZoneCode == zoneCode);
            stockLands.ForEach(o =>
            {
                var relation = new BelongRelation();
                relation.LandID = o.ID;
                relation.VirtualPersonID = (Guid)o.OwnerId;
                relation.QuanficationArea = Convert.ToDouble(o.QuantificatAreaByStock);
                relation.ZoneCode = zoneCode;
                Add(relation);
            });
        }


        /// <summary>
        /// 3 计算地跨编码相同的确股地的量化户面积之和
        /// </summary>
        /// <param name="zoneCode"></param>
        private void CaculateQuanfication(string zoneCode)
        {

            var dic = GetLandGroup(zoneCode);
            foreach (var d in dic)
            {
                foreach (var land in d.Value)
                {
                    land.ShareArea = d.Value.Sum(o => Convert.ToDouble(o.QuantificatAreaByStock)).ToString();
                    LandRepository.Update(land);
                }
            }
        }

        /// <summary>
        /// 4 添加新的确股地
        /// </summary>
        private void AddNewLand(string zoneCode)
        {
            var dic = GetLandGroup(zoneCode);
            foreach (var d in dic)
            {
                var land = new ContractLand();
                land = (ContractLand)d.Value.FirstOrDefault().Clone();
                land.QuantificatAreaByStock = null;
                land.ID = Guid.NewGuid();
                land.OwnerId = null;
                LandRepository.Add(land);
                foreach (var oldLand in d.Value)
                {
                    var relations = Get(o => o.LandID == oldLand.ID);
                    foreach (var r in relations)
                    {
                        r.LandID = land.ID;
                        Update(r,o=>o.VirtualPersonID== oldLand.OwnerId&&o.LandID==oldLand.ID);
                    }
                }

            }
        }

        /// <summary>
        /// 5 删除老确股地块数据
        /// </summary>
        /// <param name="zoneCode"></param>
        private void DeleteOldLand(string zoneCode)
        {
            var stockLands = LandRepository.Get(o => o.IsStockLand && o.ZoneCode == zoneCode);
            stockLands.ForEach(o =>
            {
                if (!string.IsNullOrWhiteSpace(o.QuantificatAreaByStock))
                {
                    LandRepository.Delete(o.ID);
                }
            });
        }


        /// <summary>
        /// 将老数据的确股地中，地块编码相同的地块整合分组存到Dictionary里面
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ContractLand>> GetLandGroup(string zoneCode)
        {
            var stockLands = LandRepository.Get(o => o.IsStockLand && o.ZoneCode == zoneCode);
            Dictionary<string, List<ContractLand>> dic = new Dictionary<string, List<ContractLand>>();
            stockLands.ForEach(o =>
            {
                if (dic.ContainsKey(o.LandNumber))
                {
                    dic.FirstOrDefault(d => d.Key == o.LandNumber).Value.Add(o);
                }
                else
                {
                    var landList = new List<ContractLand>();
                    landList.Add(o);
                    dic.Add(o.LandNumber, landList);
                }
            });
            return dic;
        }

     
    }
}
