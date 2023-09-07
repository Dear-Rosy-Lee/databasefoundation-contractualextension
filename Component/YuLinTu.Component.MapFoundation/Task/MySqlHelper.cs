using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.MapFoundation
{
    class MySqlHelper
    {
        public static bool IsTableExists(SQLiteHelper db,string tableName)
        {
            int n = queryOneInt(db, "select count(*) from sqlite_master where type='table' and lower(tbl_name)='" + tableName.ToLower() + "'");
            return n == 1;
        }
        public static SQLiteHelper OpenDatabase(string testDB)
        {
            SQLiteHelper db = new SQLiteHelper();
            db.Connection(testDB);
            return db;
        }
        public static SQLiteHelper CreateDatabase(string testDB)
        {
            if (System.IO.File.Exists(testDB))
                System.IO.File.Delete(testDB);
            SQLiteHelper.CreatNewSQLite(testDB);
            SQLiteHelper db = new SQLiteHelper();
            db.Connection(testDB);
            //string sql = "CREATE TABLE [tiles] ([zoom] INT, [row] INT, [col] INT, [img] BLOB,  CONSTRAINT [sqlite_autoindex_tiles_1] PRIMARY KEY ([zoom], [row], [col]));";
            //db.ExecuteNonQuery(sql, null);
            //String sql = "CREATE TABLE [metadata] ([key] INT, [strVal] VARCHAR(2048));";
            //String sql = "CREATE TABLE [metadata] ([key] INT, [strVal] VARCHAR(2048), CONSTRAINT[] PRIMARY KEY([key]));";
            //db.ExecuteNonQuery(sql, null);
            //sql = "CREATE TABLE [layers] ([layerName] VARCHAR(255), [layerInfo] VARCHAR(1024));";
            CreateTableTileInfo(db);
            return db;
        }
        public static void CreateTableTileInfo(SQLiteHelper db)
        {
            string sql = "CREATE TABLE [tileInfo] ([id] INTEGER PRIMARY KEY AUTOINCREMENT, [info] VARCHAR(1024));";
            db.ExecuteNonQuery(sql, null);
        }
        public static void CreateTableTiles(SQLiteHelper db,String tbName)
        {
            string sql = "CREATE TABLE ["+tbName+"] ([zoom] INT, [row] INT, [col] INT, [img] BLOB,  CONSTRAINT [sqlite_autoindex_tiles_1] PRIMARY KEY ([zoom], [row], [col]));";
            db.ExecuteNonQuery(sql, null);
        }
        public static int GetNextObjectOID(SQLiteHelper db, String tbName)
        {
            string sql = "select max(rowid) from " + tbName;
            var dr=db.Query(sql);
            if (dr.Read())
            {
                if (dr.IsDBNull(0))
                    return 1;
                return dr.GetInt32(0)+1;
            }
            return 0;
        }
        public static int queryOneInt(SQLiteHelper db,String sql)
        {
            var dr = db.Query(sql);
            if (dr.Read())
            {
                if (dr.IsDBNull(0))
                    return 0;
                return dr.GetInt32(0);
            }
            return 0;
        }
    }
}
