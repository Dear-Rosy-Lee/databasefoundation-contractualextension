/*
 * (C) 2014-2017 鱼鳞图公司版权所有，保留所有权利
*/
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.SQLite;
using System.Collections.Generic;
using System.Data;
using YuLinTuQuality.Business.TaskBasic;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// Sqlite数据库管理类
    /// </summary>
    public class SqliteManager
    {
        #region Fields

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext dbContext;

        /// <summary>
        /// 数据源
        /// </summary>
        public string dataSource;

        /// <summary>
        /// 文件路径
        /// </summary>
        private string filePath;

        #endregion

        #region Property

        /// <summary>
        /// 当前数据库路径
        /// </summary>
        public string DbFilePath { get { return filePath; } }

        #endregion

        #region Ctor

        public SqliteManager(string idString = "")
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string.IsNullOrEmpty(idString) ? "Data" : idString) + ".sqlite");
            dataSource = @"Data Source=" + filePath;
        }

        #endregion

        #region Methods

        #region 写人数据

        /// <summary>
        /// 写入数据库库数据
        /// </summary>
        public int InsertData<T>(List<T> list, int srid) where T : new()
        {
            if (list == null || list.Count == 0)
                return 0;
            using (IDbContext db = ProviderDbCSQLite.CreateDataSource(dataSource) as IDbContext)
            {
                using (IDbSchema schema = db.DataSource.CreateSchema())
                {
                    var dataBaseNames = schema.GetElements();
                    FieldInfo info = typeof(T).GetField("TableName");
                    string name = info.GetValue(null).ToString();
                    if (!dataBaseNames.Any(t => t.TableName == name))
                    {
                        schema.Export(typeof(T), srid);
                    }
                    var qc = db.CreateQuery<T>();

                    qc.AddRange(list.Cast<object>().ToArray()).Save();

                    //int addCount = 0;
                    //try
                    //{
                    //    db.BeginTransaction();
                    //    foreach (var item in list)
                    //    {
                    //        addCount++;
                    //        qc.Add(item).Save();
                    //        if (addCount == 10000)
                    //        {
                    //            db.CommitTransaction();
                    //            db.BeginTransaction();
                    //            addCount = 0;
                    //        }
                    //    }
                    //    if (addCount > 0)
                    //    {
                    //        db.CommitTransaction();
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    db.RollbackTransaction();
                    //    throw ex;
                    //}
                }
            }
            return list.Count;
        }

        #endregion

        #region 获取数据

        /// <summary>
        /// 分批次获取所有数据
        /// </summary>
        public void GetDataToMethod<T>(Action<List<T>> fuc)
        {
            List<T> list = new List<T>();
            using (IDbContext db = ProviderDbCSQLite.CreateDataSource(dataSource) as IDbContext)
            {
                IDbSchema schema = db.DataSource.CreateSchema();
                var dataBaseNames = schema.GetElements();
                FieldInfo info = typeof(T).GetField("TableName");
                string name = info.GetValue(null).ToString();
                if (dataBaseNames.Any(t => t.TableName == name))
                {
                    var qc = db.CreateQuery<T>();
                    qc.ForEach((index, cnt, obj) =>
                    {
                        list.Add(obj);
                        if (list.Count == 30000)
                        {
                            fuc(list);
                            list.Clear();
                        }
                        return true;
                    });
                    if (list.Count > 0)
                    {
                        fuc(list);
                    }
                }
            }
        }

        #endregion

        #region 业务方法

        public int GetDataCount<T>()
        {
            using (IDbContext db = ProviderDbCSQLite.CreateDataSource(dataSource) as IDbContext)
            {
                IDbSchema schema = db.DataSource.CreateSchema();
                var dataBaseNames = schema.GetElements();
                FieldInfo info = typeof(T).GetField("TableName");
                string name = info.GetValue(null).ToString();
                if (dataBaseNames.Any(t => t.TableName == name))
                {
                    var qc = db.CreateQuery<T>();
                    return qc.Count();
                }
            }
            return 0;
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        public void DeleteFile()
        {
            if (dbContext != null)
            {
                dbContext.CloseConnection();
                dbContext.ClearCache();
                dbContext = null;
                GC.Collect();
            }
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("删除临时文件失败!" + ex.Message);
            }
        }

        /// <summary>
        /// 拷贝新的数据库
        /// </summary>
        public void CopyNewDatabase()
        {
            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\Template\\Data.sqlite", filePath, true);
        }

        #endregion

        #endregion
    }
}
