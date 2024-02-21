/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 发包方模块接口定义实现
    /// </summary>
    public sealed class VirtualPersonWorkStation<T> : Workstation<T>, IVirtualPersonWorkStation<T> where T : VirtualPerson
    {
        #region Properties

        public new IVirtualPersonRepository<T> DefaultRepository
        {
            get { return base.DefaultRepository as IVirtualPersonRepository<T>; }
            set { base.DefaultRepository = value; }
        }

        public IBelongRelationRespository BelongRalationRespository { get; set; }

        #endregion Properties

        #region Ctor

        public VirtualPersonWorkStation(IVirtualPersonRepository<T> rep, IBelongRelationRespository belongRep = null)
        {
            DefaultRepository = rep;
            BelongRalationRespository = belongRep;
        }

        #endregion Ctor

        #region 方法实现

        /// <summary>
        /// 根据Id获取承包方
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VirtualPerson Get(Guid id)
        {
            return DefaultRepository.Get(id);
        }

        /// <summary>
        /// 根据名称及地域编码获取承包方
        /// </summary>
        public VirtualPerson Get(string name, string code)
        {
            return DefaultRepository.Get(name, code);
        }

        /// <summary>
        /// 根据承包方证件号码及所在地域获取承包方
        /// </summary>
        /// <param name="number"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public VirtualPerson GetByNumber(string number, string code)
        {
            return DefaultRepository.GetByNumber(number, code);
        }

        /// <summary>
        /// 根据承包方编码及所在地域获取承包方
        /// </summary>
        /// <param name="familyNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public VirtualPerson GetFamilyNumber(string familyNumber, string code)
        {
            return DefaultRepository.Get(familyNumber, code);
        }

        public VirtualPerson GetByHH(string hh, string code)
        {
            return DefaultRepository.GetByHH(hh, code);
        }

        /// <summary>
        /// 根据承包方名称，证件号，所在地域获取承包方
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public VirtualPerson Get(string name, string number, string code)
        {
            return DefaultRepository.Get(name, number, code);
        }

        /// <summary>
        /// 根据承包方名称，所在地域获取指定类型的承包方
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public VirtualPerson Get(string name, string zoneCode, eVirtualPersonType virtualPersonType)
        {
            return DefaultRepository.Get(name, zoneCode, virtualPersonType);
        }

        /// <summary>
        /// 统计地域下名称为XX的承包方个数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int Count(string name, string code)
        {
            return DefaultRepository.Count(name, code);
        }

        /// <summary>
        /// 统计地域下的承包方个数,包括子级地域
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int CountByZone(string code)
        {
            return DefaultRepository.CountByZone(code);
        }

        /// <summary>
        /// 获取地域下承包方
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetByZoneCode(string code)
        {
            List<VirtualPerson> list = DefaultRepository.GetByZoneCode(code);
            if (list != null && list.Count > 0)
            {
                list.Sort((a, b) =>
                {
                    long aNumber = 0;
                    long bNumber = 0;
                    long.TryParse(a.FamilyNumber, out aNumber);
                    long.TryParse(b.FamilyNumber, out bNumber);
                    return aNumber.CompareTo(bNumber);
                });
            }
            return list;
        }

        /// <summary>
        /// 获取地域下承包方
        /// </summary>
        /// <param name="code"></param>
        /// <param name="levelOption"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetByZoneCode(string code, eLevelOption levelOption)
        {
            return DefaultRepository.GetByZoneCode(code, levelOption);
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        /// <param name="code"></param>
        /// <param name="status"></param>
        /// <param name="levelOption"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetByZoneCode(string code, eVirtualPersonStatus status, eLevelOption levelOption)
        {
            return DefaultRepository.GetByZoneCode(code, status, levelOption);
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        /// <param name="code"></param>
        /// <param name="virtualType"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType)
        {
            return DefaultRepository.GetCollection(code, virtualType);
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        /// <param name="code"></param>
        /// <param name="virtualType"></param>
        /// <param name="levelOption"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetCollection(string code, eVirtualPersonType virtualType, eLevelOption levelOption)
        {
            return DefaultRepository.GetCollection(code, virtualType, levelOption);
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public List<VirtualPerson> GetCollection(string code, string Name)
        {
            return DefaultRepository.GetCollection(code, Name);
        }

        /// <summary>
        /// 更新承包方
        /// </summary>
        /// <param name="virtualPerson"></param>
        /// <returns></returns>
        public int Update(VirtualPerson virtualPerson)
        {
            DefaultRepository.Update(virtualPerson);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量更新承包方
        /// </summary>
        /// <returns></returns>
        public int UpdatePersonList(List<VirtualPerson> persons)
        {
            foreach (var p in persons)
            {
                DefaultRepository.Update(p);
                if (!p.IsStockFarmer)//如果是删除股农，则联动清除股农量化权属关系
                {
                    BelongRalationRespository.Delete(o => o.VirtualPersonID == p.ID);
                }
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int Delete(Guid ID)
        {
            DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包方身份证号删除承包方信息
        /// </summary>
        /// <param name="ID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(string cardId)
        {
            DefaultRepository.Delete(cardId);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int DeleteByZoneCode(string code)
        {
            DefaultRepository.DeleteByZoneCode(code);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        public int DeleteByZoneCode(string code, eVirtualPersonType virtualType)
        {
            DefaultRepository.DeleteByZoneCode(code, virtualType);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        public int DeleteByZoneCode(string code, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(code, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        public int DeleteByZoneCode(string code, eVirtualPersonType virtualType, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(code, virtualType, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除指定地域下指定的承包方状态的所有承包方对象
        /// </summary>
        /// <param name="code">地域编码</param>
        /// <param name="virtualType">承包方状态</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        public int DeleteByZoneCode(string code, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(code, virtualStatus, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ExistsByZoneCodeAndName(string code, string Name)
        {
            return DefaultRepository.ExistsByZoneCodeAndName(code, Name);
        }

        /// <summary>
        /// 是否锁定
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ExistsLockByZoneCodeAndName(string code, string Name)
        {
            return DefaultRepository.ExistsLockByZoneCodeAndName(code, Name);
        }

        /// <summary>
        /// 创建承包方编码
        /// </summary>
        public string CreateVirtualPersonNum(string zoneCode, eContractorType contractorType = eContractorType.Farmer)
        {
            string num = string.Empty;
            if (string.IsNullOrEmpty((zoneCode)))
            {
                return num;
            }
            List<VirtualPerson> vps = DefaultRepository.GetByZoneCode(zoneCode);
            List<VirtualPerson> vpsFilter = vps == null ? null : vps.FindAll(c => c.FamilyExpand.ContractorType == contractorType);
            if (vpsFilter != null && vpsFilter.Count > 0)
            {
                int maxNum = vpsFilter.Max(t =>
                {
                    int fNumber = 0;
                    int.TryParse(t.FamilyNumber, out fNumber);
                    return fNumber;
                });
                num = (maxNum + 1).ToString();//.PadLeft(4, '0');
            }
            else
            {
                if (contractorType == eContractorType.Farmer)
                    num = "1";
                else if (contractorType == eContractorType.Personal)
                    num = "8001";
                else if (contractorType == eContractorType.Unit)
                    num = "9001";
            }
            return num;
        }

        /// <summary>
        /// 存在承包方数据的地域集合
        /// </summary>
        /// <param name="zoneList">地域集合</param>
        public List<Zone> ExistZones(List<Zone> zoneList)
        {
            return DefaultRepository.ExistZones(zoneList);
        }

        /// <summary>
        /// 批量添加承包方数据
        /// </summary>
        /// <param name="listPerson">承包方对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<VirtualPerson> listPerson)
        {
            int addCount = 0;
            if (listPerson == null || listPerson.Count == 0)
            {
                return addCount;
            }
            foreach (var person in listPerson)
            {
                DefaultRepository.Add(person);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        /// <summary>
        /// 根据地域删除下面承包方所有数据
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public void ClearZoneVirtualPersonALLData(string zoneCode)
        {
            //删除包括子集地域信息及联动数据 江宇修改2016.8.30
            System.Threading.Tasks.Task.Factory.StartNew((() =>
            {
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.ContractRequireTable>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.ContractRegeditBook>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.ContractConcord>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.ContractLand>(c => c.LocationCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.SecondTableLand>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.Delete<YuLinTu.Library.Entity.LandVirtualPerson>(c => c.ZoneCode.StartsWith(zoneCode));
                base.DefaultRepository.SaveChanges();
            }));
        }

        /// <summary>
        /// 根据承包方id集合删除承包方关联数据
        /// </summary>
        public int DeleteRelationDataByVps(List<Guid> vpIds)
        {
            DefaultRepository.DeleteRelationDataByVps(vpIds);
            return TrySaveChanges(DefaultRepository);
        }

        public List<LandVirtualPerson> GetVirtualPersonsByLand(Guid landId)
        {
            return DefaultRepository.GetVirtualPersonsByLand(landId);
            //throw new NotImplementedException();
        }

        public int AddBelongRelation(BelongRelation belongRelation)
        {
            return DefaultRepository.AddBelongRelation(belongRelation);
        }

        public int DeleteRelationDataByZone(string zoneCode)
        {
            return DefaultRepository.DeleteRelationDataByZone(zoneCode);
        }

        public BelongRelation GetRelationByID(Guid personId, Guid landId)
        {
            return DefaultRepository.GetRelationByID(personId, landId);
        }

        public List<BelongRelation> GetRelationsByVpID(Guid personID)
        {
            return DefaultRepository.GetRelationsByVpID(personID);
        }

        public List<BelongRelation> GetRelationByZone(string zoneCode)
        {
            return DefaultRepository.GetRelationByZone(zoneCode);
        }

        #endregion 方法实现
    }
}