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
    /// 承包方模块消息
    /// </summary>
    public class VirtualPersonMessageContext : WorkstationContextBase
    {
        #region Fildes

        #endregion

        #region Methods

        /// <summary>
        /// 查询地域下是否存在发包方
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_EXIST_VIRTUALPERSON)]
        private void OnPersonExitInZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                string zoneCode = e.Parameter as string;
                IDbContext db = e.Datasource;
                VirtualPersonBusiness business = CreateBusiness(e.Datasource);
                business.VirtualType = eVirtualType.Land;
                bool exitLand = business.ExitInZone(zoneCode);
                if (exitLand)
                {
                    e.ReturnValue = true;
                    return;
                }
                //business.VirtualType = eVirtualType.CollectiveLand;
                //bool exitColle = business.ExitInZone(zoneCode);
                //if (exitColle)
                //{
                //    e.ReturnValue = true;
                //    return;
                //}

                //business.VirtualType = eVirtualType.Wood;
                //bool exitWood = business.ExitInZone(zoneCode);
                //if (exitWood)
                //{
                //    e.ReturnValue = true;
                //    return;
                //}

                //business.VirtualType = eVirtualType.Yard;
                //bool exitYard = business.ExitInZone(zoneCode);
                //if (exitYard)
                //{
                //    e.ReturnValue = true;
                //    return;
                //}

                //business.VirtualType = eVirtualType.House;
                //bool exitHouse = business.ExitInZone(zoneCode);
                //if (exitHouse)
                //{
                //    e.ReturnValue = true;
                //    return;
                //}
                e.ReturnValue = false;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnPersonExitInZone(查询地域下是否存在发包方)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        /// <summary>
        /// 清空合同时清空人下对应信息(当合同被清空时接收消息)
        /// </summary>
        //[MessageHandler(Name = ConcordMessage.CONCORD_CLEAR_COMPLATE)]
        //private void OnClearConcordVPINfor(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        string zoneCode = e.SenderCode as string;
        //        if (zoneCode == null)
        //        {
        //            e.ReturnValue = null;
        //        }
        //        else
        //        {
        //            IDbContext db = e.Datasource;               
        //            var vpStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
        //            var vps = vpStation.GetByZoneCode(zoneCode, eLevelOption.Self);
        //            foreach (var item in vps)
        //            {
        //                VirtualPersonExpand vpitemexpand = item.FamilyExpand;
        //                vpitemexpand.ConcordEndTime = null;
        //                vpitemexpand.ConcordNumber = null;
        //                vpitemexpand.ConcordStartTime = null;
        //                vpitemexpand.WarrantNumber = null;
        //                item.FamilyExpand = vpitemexpand;
        //                vpStation.Update(item);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearConcordVPINfor(清空而合同对应承包方下数据)", ex.Message + ex.StackTrace);
        //    }
        //}


        /// <summary>
        /// 导出承包方调查表Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_EXPORT_EXCEL)]
        private void OnExportPersonExcel(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = CreateDb();
                MessageParameter mp = e.Parameter as MessageParameter;
                TaskVirtualPersonArgument meta = new TaskVirtualPersonArgument();
                meta.IsClear = false;
                meta.FileName = mp.FileName;
                meta.ArgType = ePersonArgType.ExportExcel;
                meta.Database = dbContext;
                meta.CurrentZone = mp.CurrentZone as Zone;
                meta.virtualType = eVirtualType.Land;
                meta.DateValue = null;
                meta.PubDateValue = null;
                TaskVirtualPersonOperation import = new TaskVirtualPersonOperation();
                import.Argument = meta;
                import.FamilyOutputSet = mp.FamilyOutputSet;
                import.FamilyOtherSet = mp.FamilyOtherSet;
                import.IsBatch = mp.IsBatch;
                import.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                import.StartAsync();
                e.ReturnValue = "";
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnExportSender(导出发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 导出承包方调查表Word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_EXPORT_WORD)]
        private void OnExportPersonWord(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = CreateDb();
                MessageParameter mp = e.Parameter as MessageParameter;
                TaskVirtualPersonArgument meta = new TaskVirtualPersonArgument();
                meta.IsClear = false;
                meta.FileName = mp.FileName;
                meta.ArgType = ePersonArgType.ExportExcel;
                meta.Database = dbContext;
                meta.CurrentZone = mp.CurrentZone as Zone;
                meta.virtualType = eVirtualType.Land;
                meta.DateValue = null;
                meta.PubDateValue = null;
                TaskVirtualPersonOperation import = new TaskVirtualPersonOperation();
                import.Argument = meta;
                import.FamilyOutputSet = mp.FamilyOutputSet;
                import.FamilyOtherSet = mp.FamilyOtherSet;
                import.IsBatch = mp.IsBatch;
                import.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                import.StartAsync();
                e.ReturnValue = "";
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnExportSender(导出发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 获取地域下承包方
        /// </summary>
        [MessageHandler(Name = VirtualPersonMessage.VIRTUALPERSON_GEEBYZONE)]
        private void OnZonePersonGet(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = CreateDb();
                string zoneCode = e.Parameter as string;
                eVirtualType virtualType = (eVirtualType)e.Tag;
                VirtualPersonBusiness bus = new VirtualPersonBusiness(dbContext);
                bus.VirtualType = virtualType;
                e.ReturnValue = bus.GetByZone(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnZonePersonGet(获取地域下承包方)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 清空地域下未被锁定的承包方
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE)]
        private void OnClearVirtualPerson(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = e.Datasource;
                string zoneCode = e.ZoneCode as string;
                var personStaion = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                e.ReturnValue = personStaion.DeleteByZoneCode(zoneCode, eVirtualPersonStatus.Right, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearVirtualPerson(清空地域下未被锁定的承包方)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 创建业务类
        /// </summary>
        private VirtualPersonBusiness CreateBusiness(IDbContext db)
        {
            VirtualPersonBusiness business = new VirtualPersonBusiness(db);
            return business;
        }

        #endregion
    }
}
