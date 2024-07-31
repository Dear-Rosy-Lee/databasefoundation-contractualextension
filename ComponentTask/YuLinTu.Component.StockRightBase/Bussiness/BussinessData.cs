using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightBase.Entity;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.StockRightBase.Bussiness
{
    /// <summary>
    /// 业务操作类
    /// </summary>
    public class BussinessData
    {
        protected IDbContext m_dbContext;
        protected Zone m_currentZone;
        protected IVirtualPersonWorkStation<LandVirtualPerson> m_personStation;
        protected IBelongRelationWorkStation m_belongStation;
        protected IContractLandWorkStation m_landStation;
        protected IDictionaryWorkStation m_dicStation;

        public BussinessData(IDbContext dbContext,Zone currentZone)
        {
            m_dbContext = dbContext;
            m_currentZone = currentZone;
            m_personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            m_belongStation = dbContext.CreateBelongRelationWorkStation();
            m_landStation = dbContext.CreateContractLandWorkstation();
            m_dicStation = dbContext.CreateDictWorkStation();
        }


        /// <summary>
        /// 获取确股对象集合
        /// </summary>
        /// <param name="currentZone"></param>
        /// <returns></returns>
        public StockRightBussinessObject GetBussinessObject(Zone currentZone)
        {
            var bussnessObject = new StockRightBussinessObject();
            var dicList= m_dicStation.Get();

            bussnessObject.CurrentZone = currentZone;
            bussnessObject.BelongRelations = new List<BelongRelation>();
            bussnessObject.DictList = dicList;
            bussnessObject.VirtualPersonsAll = new ObservableCollection<VirtualPerson>(m_personStation.GetByZoneCode(currentZone.FullCode));
            bussnessObject.VirtualPersons = new ObservableCollection<VirtualPerson>(m_personStation.GetByZoneCode(currentZone.FullCode).Where(o => o.IsStockFarmer));
            bussnessObject.ContractLandsAll = new ObservableCollection<ContractLand>(m_landStation.GetCollection(currentZone.FullCode, eLevelOption.Self));
            bussnessObject.ContractLands = new ObservableCollection<ContractLand>(m_landStation.GetCollection(currentZone.FullCode, eLevelOption.Self).Where(o => o.IsStockLand));
            
            return bussnessObject;
        }

        /// <summary>
        /// 删除地域下所有股农 联动股权关系表也要删除
        /// </summary>
        /// <returns></returns>
        public bool DeleteStockPerson()
        {
            var isSuccess = true;
            try
            {
                var bussnessObject = GetBussinessObject(m_currentZone);
                bussnessObject.VirtualPersonsAll.ToList().ForEach(o =>
                {
                    o.IsStockFarmer = false;
                });
                m_personStation.UpdatePersonList(bussnessObject.VirtualPersonsAll.ToList());
            }
            catch (Exception ex)
            {
                isSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "清除股农数据失败", ex.Message + ex.StackTrace);
                throw;
            }
            return isSuccess;
        }



        /// <summary>
        /// 删除地域下所有确股地块联动股权关系也要删除
        /// </summary>
        /// <returns></returns>
        public bool DeleteLand()
        {
            var isSuccess = true;
            try
            {
                m_dbContext.CreateBelongRelationWorkStation().Deleteland(m_currentZone.FullCode);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "清除确股地块数据失败", ex.Message + ex.StackTrace);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除地域下所有确股地块联动股权关系也要删除
        /// </summary>
        /// <returns></returns>
        public bool DeleteLands(List<ContractLand> lands)
        {
            var isSuccess = true;
            try
            {
                m_dbContext.CreateBelongRelationWorkStation().DeleteLands(lands);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "清除确股地块数据失败", ex.Message + ex.StackTrace);
                throw;
            }
            return isSuccess;
        }


        /// <summary>
        /// 删除地域下所有股权关系，同时地块的共用面积归0（共用面积及地块所有关系中量化户面积之和）
        /// </summary>
        /// <returns></returns>
        public bool DeleteRelation()
        {
            var isSuccess = true;
            try
            {
               m_belongStation.DeleteRelations(m_currentZone.FullCode);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "清除股权关系数据失败", ex.Message + ex.StackTrace);
                throw;
            }
            return isSuccess;
        }

        /// <summary>
        /// 导入地块数据供子类重写使用
        /// </summary>
        /// <param name="landContrctors"></param>
        /// <returns></returns>
        public virtual string ImportLansEntities(List<ContractLand> landList)
        {
            return null;
        }
        
        /// <summary>
        /// 导入承包方数据供子类重写使用
        /// </summary>
        /// <param name="landContrctors"></param>
        /// <returns></returns>
        public virtual string ImprotContractorEntitys(List<VirtualPerson> vpList)
        {
            return null;
        }

        /// <summary>
        /// 导入权属关系
        /// </summary>
        /// <param name="landContrctors"></param>
        /// <returns></returns>
        public virtual string ImportRelationship(List<ConvertEntity> excelData)
        {
            return null;
        }
    }
}
