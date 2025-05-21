/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Entity;
using System;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据字典模块业务处理
    /// </summary>
    public class DictionaryBusiness : Task
    {
        #region Fileds

        private IDbContext dbContext;

        #endregion

        #region Properties

        /// <summary>
        /// 数据字典业务逻辑层接口
        /// </summary>
        public IDictionaryWorkStation Station { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext
        {
            get { return dbContext; }
            set
            {
                dbContext = value;
                if (dbContext != null && Station == null)
                {
                    Station = dbContext.CreateDictWorkStation();
                }
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryBusiness(IDbContext db = null)
        {
            if (db != null)
            {
                DbContext = db;
                Station = dbContext.CreateDictWorkStation();
            }
        }

        #endregion

        #region Method 数据操作

        /// <summary>
        /// 根据分组码获得该组内所有类型信息
        /// </summary>
        /// <param name="groupcode">分组码</param>
        public List<Dictionary> GetByGroupCodeDict(string groupcode)
        {
            List<Dictionary> list = null;
            if (!CanContinue())
            {
                return list;
            }
            try
            {
                if (!string.IsNullOrEmpty(groupcode))
                {
                    list = Station.GetByGroupCodeWork(groupcode);
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByGroupCodeDict(获取属性组内的所有属性信息)", ex.Message + ex.StackTrace);
                this.ReportError("获取属性组下的属性信息失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 逐条添加数据
        /// </summary>
        /// <param name="dict">实体对象</param>
        public int AddDict(Dictionary dict)
        {
            int dictCount = 0;
            if (!CanContinue() || dict == null)
            {
                return dictCount;
            }
            try
            {
                dictCount = Station.AddWork(dict);
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "AddDict(添加属性数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加属性数据失败," + ex.Message);
            }
            return dictCount;
        }

        /// <summary>
        /// 根据属性名称获得该类型信息
        /// </summary>
        /// <param name="name">属性名称</param>
        public Dictionary GetByNameDict(string name)
        {
            Dictionary dict = null;
            if (!CanContinue())
            {
                return dict;
            }
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    dict = Station.GetByNameWork(name);
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByNameDict(属性信息)", ex.Message + ex.StackTrace);
                this.ReportError("获取属性数据失败," + ex.Message);
            }
            return dict;
        }

        /// <summary>
        /// 修改字典中指定的实体对象
        /// </summary>
        /// <param name="dict">实体对象</param>
        public int ModifyDict(Dictionary dict)
        {
            int dictModifyCount = 0;
            if (!CanContinue() || dict == null)
            {
                return dictModifyCount;
            }
            try
            {
                dictModifyCount = Station.ModifyWork(dict);
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ModifyDict(修改属性数据)", ex.Message + ex.StackTrace);
                this.ReportError("修改属性数据失败," + ex.Message);
            }
            return dictModifyCount;
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="values"></param>
        public int AddRangeToDictionary(List<Dictionary> values)
        {
            int listDictCounts = 0;
            if (!CanContinue())
            {
                return listDictCounts;
            }
            try
            {
                if (values != null && values.Count > 0)
                {
                    listDictCounts = Station.AddRangeToDictionary(values);
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "AddRangeToDictionary(批量添加数据)", ex.Message + ex.StackTrace);
                this.ReportError("批量添加数据失败," + ex.Message);
            }
            return listDictCounts;
        }

        /// <summary>
        /// 逐条删除属性信息
        /// </summary>
        public int DelDict(Dictionary value)
        {
            int count = 0;
            if (!CanContinue() || value == null)
            {
                return count;
            }
            try
            {
                count = Station.DelWork(value);
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DelDict(删除属性数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除属性数据失败," + ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 批量删除属性数据
        /// </summary>
        public int DelRangeFromDict(List<Dictionary> values)
        {
            int delCounts = 0;
            if (!CanContinue())
            {
                return delCounts;
            }
            try
            {
                if (values != null && values.Count > 0)
                {
                    delCounts = Station.DeleteRangeFromDict(values);
                }
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DelRangeFromDict(批量删除属性数据)", ex.Message + ex.StackTrace);
                this.ReportError("批量删除属性数据失败," + ex.Message);
            }
            return delCounts;
        }

        /// <summary>
        /// 获得所有字典数据
        /// </summary>
        /// <returns></returns>
        public List<Dictionary> GetAll()
        {
            List<Dictionary> dictList = null;
            if (!CanContinue())
            {
                return dictList;
            }
            try
            {
                dictList = Station.Get();
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DelRangeFromDict(批量删除属性数据)", ex.Message + ex.StackTrace);
                this.ReportError("批量删除属性数据失败," + ex.Message);
            }
            return dictList;
        }

        /// <summary>
        /// 检查分组码是否重复
        /// </summary>
        public bool IsGroupCodeReapet(string groupCode, Guid id)
        {
            if (!CanContinue())
            {
                return false;
            }
            if (string.IsNullOrEmpty(groupCode))
            {
                return false;
            }
            bool result = false;
            try
            {
                if (id == null)
                {
                    result = Station.Any(t => t.GroupCode == groupCode);
                }
                else
                {
                    result = Station.Any(t => t.ID != id && t.GroupCode == groupCode && t.Code == "");
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsGroupCodeReapet(判断分组码是否存在)", ex.Message + ex.StackTrace);
                this.ReportError("判断分组码是否存在失败," + ex.Message);
            }
            return result;
        }

        #endregion

        #region Method 自定义辅助

        public bool CanContinue()
        {
            if (Station == null)
            {
                this.ReportError("尚未初始化数据字典的访问接口");
                return false;
            }
            return true;
        }

        #endregion
    }
}
