// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data.SqlClient;
using YuLinTu.Data;
using YuLinTu;
using YuLinTu.Data.SQLite;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 数据源
    /// </summary>
    public class DataBaseSourceWork
    {
        /// <summary>
        /// 连接数据库失败
        /// </summary>
        public const string ConnectionError = "连接数据库失败,请检查数据库连接路径是否有效!";

        /// <summary>
        /// 获得当前数据源
        /// </summary>
        public static IDbContext GetDataBaseSource()
        {
            IDbContext dsNew = null;
            try
            {
                dsNew = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException("", "创建数据库实例失败!", ex.Message + ex.StackTrace);
                dsNew = null;
            }
            return dsNew;
        }

        /// <summary>
        /// 获得当前数据源
        /// </summary>
        public static IDbContext GetDataBaseSource(string dataSourceName)
        {
            IDbContext dsNew = null;
            try
            {
                dsNew = DataSource.Create<IDbContext>(dataSourceName);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException("", "创建数据库实例失败!", ex.Message + ex.StackTrace);
                dsNew = null;
            }
            return dsNew;
        }

        /// <summary>
        /// 获得当前数据源
        /// </summary>
        public static IDbContext GetDataBaseSourceByPath(string fileName)
        {
            IDbContext dsNew = null;
            try
            {
                dsNew = ProviderDbCSQLite.CreateDataSourceByFileName(fileName) as IDbContext;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException("", "创建数据库实例失败!", ex.Message + ex.StackTrace);
                dsNew = null;
                throw ex;
            }
            return dsNew;
        }
    }
}
