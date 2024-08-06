using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightShuShan.Entity;
using YuLinTu.Component.StockRightShuShan.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.StockRightShuShan.Bussiness
{
    /// <summary>
    /// 数据入库
    /// </summary>
    public class ImportLandBussness : BussinessData
    {
        private int m_contractorCount = 0;//承包方数
        private int m_familyPersonCount = 0;//共有人数
        private Dictionary<Guid, string> shareAreaDic = new Dictionary<Guid, string>();//地块共用面积的字典，屏蔽删除权属关系时将shareArea置为0的影响

        public ImportLandBussness(IDbContext dbContext, Zone currentZone) : base(dbContext, currentZone)
        {
        }

        #region 导入地块实体
        public override string ImportLansEntities(List<ContractLand> landList)
        {
            DeleteLand();//清除确股地块
            try
            {
                foreach (var land in landList)
                {
                    if (!shareAreaDic.ContainsKey(land.ID))
                    {
                        shareAreaDic.Add(land.ID, land.ShareArea);
                    }
                    var i = m_landStation.Add(land);
                }
            }
            catch (Exception ex)
            {
                return string.Format("导入地块时发生异常，请检查填写的地块数据是否正确！\n{0}",ex.Message+ex.StackTrace);
            }

            return null;
        }

        #endregion

        #region 导入权属关系
        public override string ImportRelationship(List<ConvertEntity> excelData)
        {
            if (excelData == null || excelData.Count == 0)
            {
                var errorInfo= string.Format("导入权属时发生异常，请检查填写的导入数据是否正确！");
                return errorInfo;
            }
            try
            {
                DeleteRelation();
                foreach (var entity in excelData)
                {
                    foreach (var land in entity.LandList)
                    {
                        var belongRelation = new BelongRelation();
                        belongRelation.LandID = land.ID;
                        belongRelation.VirtualPersonID = entity.Contractor.ID;
                        belongRelation.ZoneCode = m_currentZone.FullCode;
                        belongRelation.QuanficationArea =Math.Round( land.QuantificicationArea,2);
                        double tableArea =(land.TableArea.HasValue ? land.TableArea.Value : 0);//台账面积
                        double formatArea = Math.Round(tableArea, 2);
                        belongRelation.TableArea = tableArea;
                        m_personStation.AddBelongRelation(belongRelation);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorInfo = string.Format("导入权属是发生异常，{0}\n请检查填写的导入数据是否正确！",ex.Message);
                return errorInfo;
            }

            //更新地块共用面积
            var tatalData= GetBussinessObject(m_currentZone);
            foreach (var land in tatalData.ContractLands)
            {
                if (shareAreaDic.ContainsKey(land.ID))
                {
                    var shareArea = shareAreaDic[land.ID];
                    land.ShareArea = shareArea;
                    m_landStation.Update(land);
                }
            }

            return null;
        }
        #endregion

        #region 导入承包方实体
        /// <summary>
        /// 导入承包方实体
        /// </summary>
        /// <returns></returns>
        public override string ImprotContractorEntitys(List<VirtualPerson> contractors)
        {
            try
            {
                m_dbContext.BeginTransaction();
                ClearData();
                var isSynchronous = AgricultureSetting.SystemVirtualPersonSynchronous;
                ContainerFactory factory = new ContainerFactory(m_dbContext);
                IVirtualPersonWorkStation<LandVirtualPerson> landStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
                IVirtualPersonWorkStation<YardVirtualPerson> yardStation = factory.CreateVirtualPersonStation<YardVirtualPerson>();
                IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = factory.CreateVirtualPersonStation<HouseVirtualPerson>();
                IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = factory.CreateVirtualPersonStation<WoodVirtualPerson>();
                IVirtualPersonWorkStation<TableVirtualPerson> tableStation = factory.CreateVirtualPersonStation<TableVirtualPerson>();
                IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = factory.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
                foreach (VirtualPerson vp in contractors)
                {
                    if (!isSynchronous)
                    {
                        ImportSingleTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                    }
                    else
                    {
                        bool result = ImportSingleTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                        if (!result)
                        {
                            continue;
                        }
                        ImportMultiTable(vp, landStation, yardStation, houseStation, woodStation, colleStation);
                    }
                }
                m_dbContext.CommitTransaction();
                //if (m_contractorCount == contractors.Count)
                //{
                //    _errorInfo.Append(string.Format("表中共有{0}户数据,成功导入{1}户、{2}条共有人记录", 
                //        contractors.Count, contractors.Count, m_contractorCount));
                //}
                //else
                //{
                //    _errorInfo.Append(string.Format("表中共有{0}户数据,成功导入{1}户、{2}条共有人记录,其中{3}户数据被锁定!",
                //        contractors.Count, m_contractorCount, m_personStation, contractors.Count - m_familyPersonCount));
                //}
            }
            catch (Exception ex)
            {
                m_dbContext.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteError(this, "ImprotContractorEntitys(导入承包方实体)", ex.Message + ex.StackTrace);
                var errorInfo=string.Format("导入承包方实体发生错误，请检查导入模板是否填写正确！\n{0}", ex.Message + ex.StackTrace);
                return errorInfo;
            }
            return null;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <returns></returns>
        private bool ClearData(eVirtualType virtualType=eVirtualType.Land)
        {
            ContainerFactory factory = new ContainerFactory(m_dbContext);
            IVirtualPersonWorkStation<LandVirtualPerson> landStation = factory.CreateVirtualPersonStation<LandVirtualPerson>();
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation = factory.CreateVirtualPersonStation<YardVirtualPerson>();
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = factory.CreateVirtualPersonStation<HouseVirtualPerson>();
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = factory.CreateVirtualPersonStation<WoodVirtualPerson>();
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = factory.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
            List<VirtualPerson> familys = new List<VirtualPerson>();
            switch (virtualType)
            {
                case eVirtualType.Land:
                    familys = landStation.GetByZoneCode(m_currentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        //landStation.DeleteByZoneCode(CurrentZone.FullCode);
                        landStation.ClearZoneVirtualPersonALLData(m_currentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<LandVirtualPerson>(landStation, familys);
                        //DeleteVirtualPerson<VirtualPerson>(factory, familys);
                    }
                    break;
                case eVirtualType.Yard:
                    familys = yardStation.GetByZoneCode(m_currentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        yardStation.DeleteByZoneCode(m_currentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<YardVirtualPerson>(yardStation, familys);
                    }
                    break;
                case eVirtualType.House:
                    familys = houseStation.GetByZoneCode(m_currentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        houseStation.DeleteByZoneCode(m_currentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<HouseVirtualPerson>(houseStation, familys);
                    }
                    break;
                case eVirtualType.Wood:
                    familys = woodStation.GetByZoneCode(m_currentZone.FullCode, eVirtualPersonStatus.Lock, eLevelOption.Self);
                    if (familys != null && familys.Count == 0)
                    {
                        woodStation.DeleteByZoneCode(m_currentZone.FullCode);
                    }
                    else
                    {
                        DeleteVirtualPerson<WoodVirtualPerson>(woodStation, familys);
                    }
                    break;
            }
            ClearTableVirtualPerson();
            familys = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 清空二轮台账信息
        /// </summary>
        private void ClearTableVirtualPerson()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config\" + "FamilyImportDefine.xml";
            if (!System.IO.File.Exists(filePath))
            {
                return;
            }
            FamilyImportDefine landDefine = YuLinTu.Library.Business.ToolSerialization.DeserializeXml(filePath, typeof(FamilyImportDefine)) as FamilyImportDefine;
            if (landDefine != null && (landDefine.ExPackageNameIndex > 0 || landDefine.ExPackageNumberIndex > 0
                || landDefine.IsDeadedIndex > 0 || landDefine.LocalMarriedRetreatLandIndex > 0
                || landDefine.PeasantsRetreatLandIndex > 0 || landDefine.ForeignMarriedRetreatLandIndex > 0))
            {
                IVirtualPersonWorkStation<TableVirtualPerson> tableStation = m_dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                tableStation.DeleteByZoneCode(m_currentZone.FullCode);
            }
        }

        /// <summary>
        /// 删除承包方信息
        /// </summary>
        private void DeleteVirtualPerson<T>(IVirtualPersonWorkStation<T> station, List<VirtualPerson> lockfamilys) where T : VirtualPerson
        {
            List<VirtualPerson> familys = InitalizeDeleteVirtualPerson(lockfamilys);
            foreach (VirtualPerson vp in familys)
            {
                station.Delete(vp.ID);
            }
            familys.Clear();
        }

        /// <summary>
        /// 初始化删除信息
        /// </summary>
        /// <param name="lockfamilys"></param>
        /// <returns></returns>
        private List<T> InitalizeDeleteVirtualPerson<T>(List<T> lockfamilys, eVirtualType virtualType = eVirtualType.House) where T : VirtualPerson
        {
            IVirtualPersonWorkStation<LandVirtualPerson> landStation = m_dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation = m_dbContext.CreateVirtualPersonStation<YardVirtualPerson>();
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation = m_dbContext.CreateVirtualPersonStation<HouseVirtualPerson>();
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation = m_dbContext.CreateVirtualPersonStation<WoodVirtualPerson>();
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation = m_dbContext.CreateVirtualPersonStation<CollectiveLandVirtualPerson>();
            List<VirtualPerson> familys = new List<VirtualPerson>();
            switch (virtualType)
            {
                case eVirtualType.Land:
                    familys = landStation.GetByZoneCode(m_currentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.Yard:
                    familys = yardStation.GetByZoneCode(m_currentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.House:
                    familys = houseStation.GetByZoneCode(m_currentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
                case eVirtualType.Wood:
                    familys = woodStation.GetByZoneCode(m_currentZone.FullCode, eLevelOption.Self);
                    foreach (VirtualPerson vp in lockfamilys)
                    {
                        familys.Remove(familys.Find(fam => fam.ID == vp.ID));
                    }
                    break;
            }
            return familys as List<T>;
        }

        /// <summary>
        /// 插入单张表数据
        /// </summary>
        private bool ImportSingleTable(VirtualPerson vp, IVirtualPersonWorkStation<LandVirtualPerson> landStation,
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation,
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation,
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation,
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation, eVirtualType virtualType = eVirtualType.Land)
        {
            switch (virtualType)
            {
                case eVirtualType.Land:
                    if (!landStation.ExistsLockByZoneCodeAndName(m_currentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = landStation.GetFamilyNumber(vp.FamilyNumber, m_currentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            //_errorInfo.Append(string.Format("承包方:{0}的户号{1}已经被{2}使用,该户将不会被导入!\n", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        landStation.Add(vp);
                    }
                    break;
                case eVirtualType.Yard:
                    if (!yardStation.ExistsLockByZoneCodeAndName(m_currentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = yardStation.GetFamilyNumber(vp.FamilyNumber, m_currentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            //_errorInfo.Append(string.Format("承包方:{0}的户号{1}已经被{2}使用\n", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        yardStation.Add(vp);
                    }
                    break;
                case eVirtualType.House:
                    if (!houseStation.ExistsLockByZoneCodeAndName(m_currentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = houseStation.GetFamilyNumber(vp.FamilyNumber, m_currentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            //_errorInfo.Append(string.Format("承包方:{0}的户号{1}已经被{2}使用\n", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        houseStation.Add(vp);
                    }
                    break;
                case eVirtualType.Wood:
                    if (!woodStation.ExistsLockByZoneCodeAndName(m_currentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = woodStation.GetFamilyNumber(vp.FamilyNumber, m_currentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            //_errorInfo.Append(string.Format("承包方:{0}的户号{1}已经被{2}使用\n", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        woodStation.Add(vp);
                    }
                    break;
                case eVirtualType.CollectiveLand:
                    if (!colleStation.ExistsLockByZoneCodeAndName(m_currentZone.FullCode, vp.Name))
                    {
                        VirtualPerson virp = colleStation.GetFamilyNumber(vp.FamilyNumber, m_currentZone.FullCode);
                        if (virp != null)
                        {
                            int familyNumber = 0;
                            Int32.TryParse(vp.FamilyNumber, out familyNumber);
                            //_errorInfo.Append(string.Format("承包方:{0}的户号{1}已经被{2}使用\n", vp.Name, familyNumber, virp.Name));
                            return false;
                        }
                        colleStation.Add(vp);
                    }
                    break;
                default:
                    break;
            }

            m_contractorCount++;
            m_familyPersonCount += vp.SharePersonList.Count;
            return true;
        }

        /// <summary>
        /// 插入多张表数据
        /// </summary>
        private void ImportMultiTable(VirtualPerson vp, IVirtualPersonWorkStation<LandVirtualPerson> landStation,
            IVirtualPersonWorkStation<YardVirtualPerson> yardStation,
            IVirtualPersonWorkStation<HouseVirtualPerson> houseStation,
            IVirtualPersonWorkStation<WoodVirtualPerson> woodStation,
            IVirtualPersonWorkStation<CollectiveLandVirtualPerson> colleStation,eVirtualType virtualType=eVirtualType.Land)
        {
            switch (virtualType)
            {
                case eVirtualType.Land:
                    if (virtualType == eVirtualType.Land)
                        return;
                    landStation.Add(vp);
                    break;
                case eVirtualType.Yard:
                    if (virtualType == eVirtualType.Yard)
                        return;
                    yardStation.Add(vp);
                    break;
                case eVirtualType.House:
                    if (virtualType == eVirtualType.House)
                        return;
                    houseStation.Add(vp);
                    break;
                case eVirtualType.Wood:
                    if (virtualType == eVirtualType.Wood)
                        return;
                    woodStation.Add(vp);
                    break;
                case eVirtualType.CollectiveLand:
                    if (virtualType == eVirtualType.CollectiveLand)
                        return;
                    colleStation.Add(vp);
                    break;
                default:
                    break;
            }
        }

        #endregion
        
    }
}
