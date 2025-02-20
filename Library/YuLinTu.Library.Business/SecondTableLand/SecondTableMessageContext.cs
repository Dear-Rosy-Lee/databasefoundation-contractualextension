/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 接受消息方
    /// </summary>
    public class SecondTableMessageContext : WorkstationContextBase
    {
        #region Fields

        #endregion

        #region Methods

        #region 数据处理

        /// <summary>
        /// 根据承包方标识删除下属地块
        /// </summary>
        [MessageHandler(Name = SecondTableLandMessage.SECONDPERSON_DELT_COMPLETE)]
        public void OnDeleteLandByPersonIDComplate(object sender, ModuleMsgArgs e)
        {
            try
            {
                VirtualPerson virtualPerson = e.Parameter as VirtualPerson;
                if (virtualPerson == null)
                {
                    return;
                }
                IDbContext db = e.Datasource;
                SecondTableLandBusiness secondLandBussiness = CreateBusiness(db);
                e.ReturnValue = secondLandBussiness.DeleteLandByPersonID(virtualPerson.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 更新地块的承包方名称
        /// </summary>
        [MessageHandler(Name = SecondTableLandMessage.SECONDPERSON_SET_COMPLATE)]
        public void OnUpdateLandOwnerNameComplate(object sender, ModuleMsgArgs e)
        {
            try
            {
                VirtualPerson virtualPerson = e.Parameter as VirtualPerson;
                if (virtualPerson == null)
                {
                    return;
                }
                IDbContext db = e.Datasource;
                SecondTableLandBusiness secondLandBussiness = CreateBusiness(db);
                e.ReturnValue = secondLandBussiness.Update(virtualPerson.ID,virtualPerson.Name);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSenderNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
        }

        private SecondTableLandBusiness CreateBusiness(IDbContext db)
        {
            SecondTableLandBusiness business = new SecondTableLandBusiness(db);
            return business;
        }
        #endregion

        #endregion

    }
}
