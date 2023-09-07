/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using Microsoft.Practices.Unity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 接收消息
    /// </summary>
    public class AccountLandMessageContext : WorkstationContextBase
    {
        #region Fields

        #endregion

        #region Methods

        #region 数据处理

        /// <summary>
        /// 根据承包方标识删除下属地块-合同-权证
        /// </summary>
        //[MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE)]
        //public void OnDeleteLandByPersonIDComplate(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        VirtualPerson virtualPerson = e.Parameter as VirtualPerson;
        //        if (virtualPerson == null)
        //        {
        //            return;
        //        }
        //        IDbContext db = e.Datasource;             
        //        var landStation = db.CreateContractLandWorkstation();
        //        var lands = landStation.GetCollection(virtualPerson.ID);
        //        e.ReturnValue = landStation.DeleteLandByPersonID(virtualPerson.ID);
        //        var concordStation = db.CreateConcordStation();
        //        var regeditStation = db.CreateRegeditBookStation();
        //        var concords = concordStation.GetContractsByFamilyID(virtualPerson.ID);
        //        var coilStation = db.CreateBoundaryAddressCoilWorkStation();
        //        var dotStation = db.CreateBoundaryAddressDotWorkStation();
        //        if (lands != null && lands.Count > 0)
        //        {
        //            foreach (var land in lands)
        //            {
        //                coilStation.Delete(c => c.LandID == land.ID);
        //                dotStation.Delete(c => c.LandID == land.ID);
        //            }
        //        }

        //        if (concords != null && concords.Count > 0)
        //        {                   
        //            foreach (var concord in concords)
        //            {
        //                concordStation.Delete(concord.ID);
        //                regeditStation.Delete(concord.ID);
        //            }                
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteLandByPersonIDComplate(根据承包方标识删除下属地块)", ex.Message + ex.StackTrace);
        //    }
        //}

        /// <summary>
        /// 清空地块(承包方被清空)
        /// </summary>
        //[MessageHandler(Name = VirtualPersonMessage.CLEAR_COMPLATE)]
        //public void OnClearContractLand(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        var dbContext = e.Datasource;
        //        var zoneCode = e.ZoneCode;
        //        if (dbContext == null || string.IsNullOrEmpty(zoneCode))
        //            return;
        //        var landStation = dbContext.CreateContractLandWorkstation();
        //        e.ReturnValue = landStation.DeleteOtherByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearContractLand(清空地块)", ex.Message + ex.StackTrace);
        //    }
        //}

        ///<summary>
        /// 修改承包地块信息
        ///<summary>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_EDIT_COMPLATE)]
        public void OnEditContractLand(object sender, ModuleMsgArgs e)
        {
            try
            {
                VirtualPerson editVp = e.Parameter as VirtualPerson;
                if (editVp == null)
                    return;
                var db = e.Datasource;
                if (db == null)
                    db = DataBaseSource.GetDataBaseSource();
                var landStaion = db.CreateContractLandWorkstation();
                List<ContractLand> lands = landStaion.GetLandsByObligeeIds(new Guid[] { editVp.ID });
                if (lands == null || lands.Count == 0)
                    return;
                lands.ForEach(c => c.OwnerName = editVp.Name);
                landStaion.UpdateRange(lands);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnEditContractLand(编辑承包地快信息)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 编辑图形
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_EDITGEOMETRY_COMPLETE)]
        public void OnEditGeometry(object sender, ModuleMsgArgs e)
        {
            try
            {
                Guid landId;
                Guid.TryParse((string)e.Parameter, out landId);
                var area = (double)e.Tag;
                var geo = e.ReturnValue as YuLinTu.Spatial.Geometry;
                var db = DataBaseSource.GetDataBaseSource();
                var landStation = db.CreateContractLandWorkstation();
                var curLand = landStation.Get(landId);
                if (curLand == null)
                    return;
                curLand.ActualArea = area;
                //curLand.AwareArea = area;
                curLand.Shape = geo;
                landStation.Update(curLand);
            }
            catch (Exception ex)
            {
                Log.Log.WriteException(this, "OnEditGeometry(编辑图形)", ex.Message + ex.StackTrace);
                throw new YltException(string.Format("编辑图形保存失败：{0}", ex.Message + ex.StackTrace));
            }
        }

        /// <summary>
        /// 清空合同时清空人下对应信息(当合同被清空时接收消息)
        /// </summary>
        //[MessageHandler(Name = ConcordMessage.CONCORD_CLEAR_COMPLATE)]
        //private void OnClearConcordlandINfor(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        string zoneCode = e.ZoneCode as string;
        //        if (zoneCode == null)
        //        {
        //            e.ReturnValue = null;
        //        }
        //        else
        //        {
        //            IDbContext db = e.Datasource;
        //            var landStation = db.CreateContractLandWorkstation();
        //            var lands = landStation.GetCollection(zoneCode, eLevelOption.Self);
        //            foreach (var item in lands)
        //            {
        //                item.ConcordArea = "";
        //                item.ConcordId = null;                      
        //                landStation.Update(item);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearConcordVPINfor(清空而合同对应承包方下数据)", ex.Message + ex.StackTrace);
        //    }
        //}


        #endregion

        #endregion
    }
}
