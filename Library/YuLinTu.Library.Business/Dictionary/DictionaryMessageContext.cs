/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据字典模块接收消息
    /// </summary>
    public class DictionaryMessageContext : WorkstationContextBase
    {
        #region Method 接收消息

        /// <summary>
        /// 从数据字典中获取属性对象
        /// </summary>
        [MessageHandler(Name=SecondTableLandMessage.SECONDLAND_GET_DICTIONARY)]
        public void OnGetDictionaryValue(object sender, ModuleMsgArgs e)
        {
            try
            {
                IDbContext db = e.Datasource;
                DictionaryBusiness business = CreateBusiness(db);
                e.ReturnValue = business.GetAll();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGetDictionaryValue(获取字典属性对象)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 从数据字典中获取属性对象
        /// </summary>
        [MessageHandler(Name = ConcordMessage.CONCORD_GET_DICTIONARY)]
        public void OnGetDictionary(object sender, ModuleMsgArgs e)
        {
            OnGetDictionaryValue(sender, e);
        }
        #endregion

        #region Method 辅助方法

        /// <summary>
        /// 创建业务
        /// </summary>
        private DictionaryBusiness CreateBusiness(IDbContext db)
        {
            DictionaryBusiness business = new DictionaryBusiness();
            business.Station = db.CreateDictWorkStation();
            business.DbContext = db;
            return business;
        }

        #endregion
    }
}
