/******************************************************
* 创建:颜学铭 2013/3/11 10:36:20 
* 修改:
******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// Sqlite数据库访问辅助类
    /// </summary>
    ///<remark>
    /// 编号        作者        日期                    操作
    /// 001         颜学铭     2010-01-05 16:27   编写源码
    ///</remark>
    public class SQLiteHelper : IDisposable
    {
        #region Private Fields
        private SQLiteConnection _con;
        #endregion

        #region Public Properties And Methods
        /// <summary>
        /// 创建一个新的SQLite数据库
        /// </summary>
        /// <param name="SQLiteName">传入的数据库名称，如：c:/test.db</param>
        public static void CreatNewSQLite(string SQLiteName)
        {
            SQLiteConnection.CreateFile(SQLiteName);
        }
        /// <summary>
        /// 建立数据库连接
        /// 连接串格式示例：
        ///     strFileName:@"e:\test.db3"
        /// </summary>
        /// <param name="strFileName"></param>
        public void Connection(string strFileName)
        {
            Close();
            _con = new SQLiteConnection("Data Source=" + strFileName);//"Data Source=e:\\test.db3"
            _con.Open();
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            //_con.
            Dispose();
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public SQLiteTransaction BeginTransaction()
        {
            return _con.BeginTransaction();
        }
        /// <summary>
        /// insert示例：
        ///     sql:insert into Book values(@ID,@BookName,@Price);
        ///     lstParameters:{["ID",1]、["BookName","语文"]、["Price",35]}
        /// update示例：
        ///     sql:update Book set BookName=@BookName,Price=@Price where ID=@ID;"
        ///     lstParameters:{["ID",1]、["BookName","语文"]、["Price",35]}
        /// delete示例：
        ///     sql:delete from Book where ID=@ID;
        ///     lstParameters:{["ID",1]}
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="lstParameters"></param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string sql, IEnumerable<SQLiteParam> lstParameters)
        {
            SQLiteCommand cmd = _con.CreateCommand();

            cmd.CommandText = sql;// "insert into Book values(@ID,@BookName,@Price);";
            if (lstParameters != null)
            {
                foreach (SQLiteParam kv in lstParameters)
                {
                    cmd.Parameters.Add(new SQLiteParameter(kv.ParamName, kv.ParamValue));
                }
            }
            int i = cmd.ExecuteNonQuery();
            return i == 1;
        }
        public SQLiteCommand CreateCommand(string sql)
        {
            SQLiteCommand cmd= _con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = _con;
            return cmd;
        }
        /// <summary>
        /// select示例：
        ///     sql:"select * from Book where ID=@ID;";
        ///     lstParameters:{["ID",1]}
        /// 结果集读取示例：
        ///       while(dr.Read())
        ///       {
        ///           Book book = new Book();
        ///           book.ID = dr.GetInt32(0);
        ///           book.BookName = dr.GetString(1);
        ///           book.Price = dr.GetDecimal(2);
        ///       }
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="lstParameters"></param>
        /// <returns></returns>
        public SQLiteDataReader Query(string sql, IEnumerable<SQLiteParam> lstParameters=null)
        {
            //_con.Open();

            SQLiteCommand cmd = _con.CreateCommand();

            cmd.CommandText = sql;// "select * from Book where ID=@ID;";
            if (lstParameters != null)
            {
                foreach (SQLiteParam kv in lstParameters)
                {
                    cmd.Parameters.Add(new SQLiteParameter(kv.ParamName, kv.ParamValue));//cmd.Parameters.Add(new SQLiteParameter("ID", ID));
                }
            }
            SQLiteDataReader dr = cmd.ExecuteReader();
            return dr;
        }
        #region IDisposable
        public void Dispose()
        {
            if (_con != null && !string.IsNullOrEmpty(_con.ConnectionString))
            {
                _con.Dispose();
                _con = null;
            }
        }
        #endregion
        #endregion
    }
    public class SQLiteParam
    {
        public string ParamName;
        public object ParamValue;
    }
}
