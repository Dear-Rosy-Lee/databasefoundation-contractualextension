using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据源
    /// </summary>
    public class Db
    {
        /// <summary>
        /// The connection error
        /// </summary>
        //public const string ConnectionError = "连接数据库失败,请检查数据库连接路径是否有效!";

        private static IDbContext db = null;

        public static IDbContext GetInstance()
        {
            if (db is null || db.DataSource is null)
            {
                db = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
                return db;
            }
            var newdb = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            if (newdb.ConnectionString.Equals(db.ConnectionString))
            {
                return db;
            }
            db = newdb;
            return db;
        }
    }
}