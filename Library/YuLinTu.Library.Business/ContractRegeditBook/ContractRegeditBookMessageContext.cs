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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 权证模块消息
    /// </summary>
    public class ContractRegeditBookMessageContext : WorkstationContextBase
    {
        #region Fildes

        #endregion

        #region Methods

        /// <summary>
        /// 删除权证(当合同被删除时接收消息)
        /// </summary>
        [MessageHandler(Name = ConcordMessage.CONCORD_DELETE_COMPLATE)]
        private void OnDeleteRegeditBookByNumber(object sender, ModuleMsgArgs e)
        {
            try
            {
                ContractConcord concord = e.Parameter as ContractConcord;
                if (concord == null)
                {
                    e.ReturnValue = null;
                }
                else
                {
                    IDbContext db = e.Datasource;
                    ContractRegeditBookBusiness business = CreateBusiness(db);
                    e.ReturnValue = business.DeleteByRegeditNumber(concord.ConcordNumber);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteRegeditBookByNumber(删除权证数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空权证(当合同被清空时接收消息)
        /// </summary>
        //[MessageHandler(Name = ConcordMessage.CONCORD_CLEAR_COMPLATE)]
        //private void OnClearRegeditBook(object sender, ModuleMsgArgs e)
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
        //            var bookStaion = db.CreateRegeditBookStation();
        //            bookStaion.DeleteByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearRegeditBook(清空权证数据)", ex.Message + ex.StackTrace);
        //    }
        //}

        /// <summary>
        /// 清空权证(承包方和承包地块被清空)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE)]
        public void OnClearAllRegeditBooks(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                IDbContext db = e.Datasource;
                var bookStation = db.CreateRegeditBookStation();
                bookStation.DeleteByZoneCode(currentZoneCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearAllRegeditBooks(清空权证数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空权证(承包地被清空)
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTACCOUNT_CLEAR_COMPLETE)]
        public void OnClearRegeditBooks(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                IDbContext db = e.Datasource;
                var bookStation = db.CreateRegeditBookStation();
                bookStation.DeleteByZoneCode(currentZoneCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnClearRegeditBooks(清空权证数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空权证(承包方被清空)
        /// </summary>
        //[MessageHandler(Name = VirtualPersonMessage.CLEAR_COMPLATE)]
        //public void OnClearContractRegeditBook(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        var dbContext = e.Datasource;
        //        var zoneCode = e.ZoneCode;
        //        if (dbContext == null || string.IsNullOrEmpty(zoneCode))
        //            return;
        //        var bookStation = dbContext.CreateRegeditBookStation();
        //        e.ReturnValue = bookStation.DeleteByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearContractRegeditBook(清空权证)", ex.Message + ex.StackTrace);
        //    }
        //}

        /// <summary>
        /// 删除权证
        /// </summary>
        [MessageHandler(Name = ContractAccountMessage.CONTRACTLAND_DELETE_COMPLETE)]
        public void OnDeleteRegeditBook(object sender, ModuleMsgArgs e)
        {
            try
            {
                string currentZoneCode = e.ZoneCode;
                ContractLand land = e.Parameter as ContractLand;
                if (land.ConcordId == null)
                    return;
                IDbContext db = e.Datasource;
                var bookStation = db.CreateRegeditBookStation();
                var concordStaion = db.CreateConcordStation();
                if (concordStaion.Get(c => c.ID == land.ConcordId).FirstOrDefault() == null)
                {
                    bookStation.Delete((Guid)land.ConcordId);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteRegeditBook(删除权证)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 创建业务逻辑
        /// </summary>
        private ContractRegeditBookBusiness CreateBusiness(IDbContext db)
        {
            ContractRegeditBookBusiness business = new ContractRegeditBookBusiness(db);
            return business;
        }

        #endregion
    }
}
