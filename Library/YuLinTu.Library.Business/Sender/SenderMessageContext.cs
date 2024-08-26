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
    /// 发包方模块消息
    /// </summary>
    public class SenderMessageContext : WorkstationContextBase
    {
        #region Fildes

        #endregion

        #region Methods

        #region 数据处理

        /// <summary>
        /// 添加发包方数据
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_ADD)]
        private void OnAddSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue tissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.AddSender(tissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnAddSender(添加发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 合并发包方数据
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_COMBINE)]
        private void OnComBineSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue tissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.AddSender(tissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnAddSender(添加发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        /// <summary>
        /// 更新发包方数据
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_UPDATE)]
        private void OnUpdateSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue tissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.UpdateSender(tissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnUpdateSender(更新发包方数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除发包方数据
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_DELETE)]
        private void OnDeleteSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue tissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.DeleteSender(tissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDeleteSender(删除发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 导入发包方调查表
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_IMPORTTABLE)]
        private void OnImportTalbe(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导出发包方Excel模板
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_EXCELTEMPLATE)]
        private void OnExcelTemplate(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导出发包方Word模板
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_WORDTEMPLATE)]
        private void OnWordTemplate(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导入发包方为Excel
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_EXPORTEXCEL)]
        private void OnExportExcel(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 导出发包方为Word
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_EXPORTWORD)]
        private void OnExportWord(object sender, ModuleMsgArgs e)
        {
        }

        /// <summary>
        /// 获取指定地域编码下的发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_GETDATA)]
        private void OnSendersByCode(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.SendersByCode(e.Parameter.ToString());
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSendersByCode(地域编码下发包方)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取指定地域及子地域下的发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_GETCHILDRENDATA)]
        private void OnSenderListByCode(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.SenderSubsByCode(e.Parameter.ToString());
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSendersByCode(地域编码下发包方)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取新的发包方编码
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_CREATCODE)]
        private string OnGetSenderCode(object sender, ModuleMsgArgs e)
        {
            string tissucCode = string.Empty;
            try
            {
                Zone currentZone = e.Parameter as Zone;
                CollectivityTissue currentTissue = e.Tag as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.CreatSenderCode(currentZone, currentTissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGetSenderCode(获取新的发包方编码)", ex.Message + ex.StackTrace);
            }
            return tissucCode;
        }

        /// <summary>
        /// 发包方名称是否重复
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_NAMEEXIT)]
        private void OnSenderNameRepeat(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue currentTissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.SenderNameRepeat(currentTissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 发包方名称是否重复
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_NAMECODEEXIT)]
        private void OnSenderNameOrCodeRepeat(object sender, ModuleMsgArgs e)
        {
            CollectivityTissue currentTissue = e.Parameter as CollectivityTissue;
            IDbContext db = e.Datasource;
            SenderDataBusiness business = CreateBusiness(e.Datasource);
            e.ReturnValue = business.SenderNameOrCodeRepeat(currentTissue);
        }

        /// <summary>
        /// 是否是默认发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_ISDEFAULT)]
        private void OnIsDefaultSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                CollectivityTissue currentTissue = e.Parameter as CollectivityTissue;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.IsDefaultSender(currentTissue);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnIsDefaultSender(是否是默认发包方)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 地域下是否存在发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_EXITS)]
        public void OnSenderExitsInZone(object sender, ModuleMsgArgs e)
        {
            try
            {
                string zoneCode = e.Parameter as string;
                ContainerFactory factroy = new ContainerFactory(e.Datasource);
                ISenderWorkStation station = factroy.CreateSenderWorkStation();
                bool exit = station.Exists(zoneCode);
                e.ReturnValue = exit;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清空地域下的发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_CLEARBYZONECODE)]
        public void OnClearByCode(object sender, ModuleMsgArgs e)
        {
            try
            {
                string zoneCode = e.Parameter as string;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.DeleteSender(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除地域完成
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_DELETE_COMPLETE)]
        public void OnDelZoneComplate(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                IDbContext db = e.Datasource;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.DeleteSender(zone.FullCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnDelZoneComplate(删除地域完成)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 根据地域添加默认发包方
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_ADD_COMPLETE)]
        public void OnAddZoneComplate(object sender, ModuleMsgArgs e)
        {
            try
            {
                Zone zone = e.Parameter as Zone;
                if (zone.FullCode.Length < Zone.ZONE_TOWN_LENGTH)//镇村组才有集体经济组织
                {
                    return;
                }
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.AddSenderByZone(zone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnAddZoneComplate(根据地域添加默认发包方)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 根据获取发包方
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_GET_ID)]
        public void OnGetSenderById(object sender, ModuleMsgArgs e)
        {
            try
            {
                Guid id = (Guid)e.Parameter;
                SenderDataBusiness business = CreateBusiness(e.Datasource);
                e.ReturnValue = business.GetSenderById(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnAddZoneComplate(根据地域添加默认发包方)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region 地域导入后处理发包方

        /// <summary>
        /// 导入地域完成
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_IMPORTTABLE_COMPLETE)]
        public void OnImportZoneComplate(object sender, ModuleMsgArgs e)
        {
            MultiObjectArg multiarg = e.Parameter as MultiObjectArg;
            if (multiarg == null)
            {
                return;
            }
            IDbContext db = e.Datasource;
            SenderDataBusiness business = CreateBusiness(e.Datasource);
            business.ProcessZoneComplateForImport(multiarg);
        }

        /// <summary>
        /// 更新地域完成
        /// </summary>
        [MessageHandler(Name = SenderMessage.SENDER_UPDATEZONE)]
        public void OnUpdateZoneComplate(object sender, ModuleMsgArgs e)
        {
            MultiObjectArg multiarg = e.Parameter as MultiObjectArg;
            if (multiarg == null || e.Datasource == null)
            {
                return;
            }
            IDbContext db = e.Datasource;
            SenderDataBusiness business = CreateBusiness(e.Datasource);
            business.ProcessZoneComplate(multiarg);
        }

        /// <summary>
        /// 创建业务逻辑
        /// </summary>
        private SenderDataBusiness CreateBusiness(IDbContext db)
        {
            SenderDataBusiness business = new SenderDataBusiness();
            business.DbContext = db;
            business.Station = db.CreateSenderWorkStation();
            return business;
        }

        #endregion

        #region 地域清空后发包方处理

        /// <summary>
        /// 清除发包方数据
        /// </summary>
        //[MessageHandler(Name = ZoneMessage.ZONE_CLEAR_COMPLETE)]
        //private void OnClearSender(object sender, ModuleMsgArgs e)
        //{
        //    try
        //    {
        //        IDbContext db = e.Datasource;
        //        SenderDataBusiness business = CreateBusiness(e.Datasource);
        //        e.ReturnValue = business.ClearSender();
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearSender(清除发包方数据)", ex.Message + ex.StackTrace);
        //        throw ex;
        //    }
        //}

        #endregion

        #region 导出发包方调查表

        /// <summary>
        /// 导出发包方调查表Execl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(Name = SenderMessage.SENDER_EXPORT_COM)]
        private void OnExportExcelSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = CreateDb();
                TaskSenderArgument meta = new TaskSenderArgument();
                meta.Database = dbContext;
                meta.CurrentZone = e.Parameter as Zone;
                meta.ArgType = eSenderArgType.ExportExcel;
                TaskSenderOperation taskSender = new TaskSenderOperation();
                taskSender.Argument = meta;
                taskSender.Name = "导出发包方数据";
                taskSender.Completed += new TaskCompletedEventHandler((o, b) =>
                {
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTEXCEL_COMPLETE, true));
                });
                taskSender.Terminated += new TaskTerminatedEventHandler((o, b) =>
                {
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTEXCEL_COMPLETE, false));
                });
                taskSender.StartAsync();
                e.ReturnValue = "";
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnExportSender(导出发包方数据)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 导出发包方调查表Word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(Name = SenderMessage.SENDER_EXPORT_WORD)]
        private void OnExportWordSender(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext dbContext = CreateDb();
                TaskSenderArgument meta = new TaskSenderArgument();
                meta.Database = dbContext;
                meta.CurrentZone = e.Parameter as Zone;
                meta.FileName = e.Tag as string;
                meta.ArgType = eSenderArgType.ExportWord;
                TaskSenderOperation senderTask = new TaskSenderOperation();
                senderTask.Argument = meta;
                senderTask.Name = "导出发包方数据";

                senderTask.Completed += new TaskCompletedEventHandler((o, b) =>
                {
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTWORD_COMPLETE, true));
                });
                senderTask.Terminated += new TaskTerminatedEventHandler((o, b) =>
                {
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTWORD_COMPLETE, false));
                });
                senderTask.StartAsync();
                e.ReturnValue = "";
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnExportSender(导出发包方数据)", ex.Message + ex.StackTrace);
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

        #endregion

        #endregion
    }
}
