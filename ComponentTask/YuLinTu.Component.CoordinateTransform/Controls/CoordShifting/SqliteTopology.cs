/*
 * (C)2016 -2015 公司版权所有,保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 拓扑数据库管理
    /// </summary>
    public class SqliteTopology
    {
        #region Property

        /// <summary>
        /// 文件目录
        /// </summary>
        public string dbPath { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public string dataSource;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqliteTopology(string filePath = "")
        {
            dbPath = Path.Combine((!string.IsNullOrEmpty(filePath) ? filePath :
                AppDomain.CurrentDomain.BaseDirectory), @"changedb.sqlite"); //Path.Combine(filePath, @".sqlite");
            dataSource = @"Data Source=" + dbPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 创建拓扑db
        /// </summary>
        public static string CreateTopoDb(string folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath);
                string dbPath = Path.Combine(folderPath, "topodb.sqlite");
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Template\\topodb.sqlite", dbPath, true);
                return "";
            }
            catch (Exception ex)
            {
                return "检查信息路径设置不正确！";
            }
        }

        /// <summary>
        /// 插入拓扑数据
        /// </summary>
        public void InsertTopoValue<T>(List<T> sqlList, string tableName = "", Func<T, string> creatSql = null)
        {
            if (!File.Exists(dbPath) || sqlList == null || (sqlList != null && sqlList.Count == 0))
                return;
            if (tableName == "")
                tableName = typeof(T).Name;
            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                conn.Open();
                string dataString = string.Empty;
                using (DbTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                            command.ExecuteScalar();
                        }
                        int srid = 0;
                        //创建表 
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            for (int i = 0; i < sqlList.Count; i++)
                            {
                                T t = sqlList[i];
                                if (creatSql == null)
                                    continue;
                                var sql = creatSql(t);
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                        sqlList.Clear();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //LogWrite.WriteErrorLog("保存拓扑错误数据时发生错误:" + dataString + ex.Message + ex.StackTrace);
                        trans.Rollback();
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 插入矢量数据
        /// </summary>
        public void InsertCodeData(List<CodeData> dataList, string tableName, int srid)
        {
            InsertTopoValue<CodeData>(dataList, "", (t) =>
            {
                string geoString = t.Shape.AsText();
                if (t.Shape.Instance.Coordinates.Length > 0 && t.Shape.Instance.Coordinates[0].Z != double.NaN)
                {
                    geoString = geoString.Replace(" 0,", ",");
                    geoString = geoString.Replace(" 0))", "))");
                    if (t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.Point ||
                        t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPoint ||
                        t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPolygon)
                        geoString = geoString.Replace(" 0)", ")");
                }
                string sqlValue = string.Format("'{0}','{1}','{2}','{3}'", t.EnId, t.Code, t.IsBase, t.RowNumber);
                var dataString = string.Format("INSERT INTO {0} VALUES({1},{2})", tableName, sqlValue,
                    t.Shape == null ? "null" : string.Format("GeomFromText('{0}',{1})", geoString, srid));
                return dataString;
            });
        }

        /// <summary>
        /// 插入带参数的范围数据
        /// </summary>
        public void InsertEnData(List<EnData> dataList, string tableName, int srid)
        {
            InsertTopoValue<EnData>(dataList, "", (t) =>
            {
                string geoString = t.Shape.AsText();
                if (t.Shape.Instance.Coordinates.Length > 0 && t.Shape.Instance.Coordinates[0].Z != double.NaN)
                {
                    geoString = geoString.Replace(" 0,", ",");
                    geoString = geoString.Replace(" 0))", "))");
                    if (t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.Point ||
                        t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPoint ||
                        t.Shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPolygon)
                        geoString = geoString.Replace(" 0)", ")");
                }
                string sqlValue = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'",
                    t.ID, t.ZoneCode, t.A0, t.A1, t.A2, t.B0, t.B1, t.B2, t.XIndex, t.YIndex);
                var dataString = string.Format("INSERT INTO {0} VALUES({1},{2})", tableName, sqlValue,
                    t.Shape == null ? "null" : string.Format("GeomFromText('{0}',{1})", geoString, srid));
                return dataString;
            });
        }

        /// <summary>
        /// 插入范围数据
        /// </summary>
        public void InsertEnCenterData(List<EnCenterData> dataList, string tableName, int srid)
        {
            InsertTopoValue<EnCenterData>(dataList, "", (t) =>
            {
                string geoString = t.EnvelopShape.AsText();
                if (t.EnvelopShape.Instance.Coordinates.Length > 0 && t.EnvelopShape.Instance.Coordinates[0].Z != double.NaN)
                {
                    geoString = geoString.Replace(" 0,", ",");
                    geoString = geoString.Replace(" 0))", "))");
                    if (t.EnvelopShape.GeometryType == YuLinTu.Spatial.eGeometryType.Point ||
                        t.EnvelopShape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPoint ||
                        t.EnvelopShape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPolygon)
                        geoString = geoString.Replace(" 0)", ")");
                }
                string sqlValue = string.Format("'{0}','{1}'", t.ID, t.ZoneCode);
                var dataString = string.Format("INSERT INTO {0} VALUES({1},{2})", tableName, sqlValue,
                    t.EnvelopShape == null ? "null" : string.Format("GeomFromText('{0}',{1})", geoString, srid));
                return dataString;
            });
        }

        /// <summary>
        /// 根据地域变化获取范围
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public List<EnCenterData> GetEnCenterDatas(string zoneCode, int srid)
        {
            List<EnCenterData> list = new List<EnCenterData>();
            GetDataConnection((conn, cmd) =>
            {
                var dataString = $"Select ID,ZoneCode, AsText(Shape) from EnCenterData where ZoneCode='{zoneCode}'";
                cmd.CommandText = dataString;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EnCenterData data = new EnCenterData();
                    data.ID = reader[0] == null ? Guid.NewGuid() : Guid.Parse(reader[0].ToString());
                    data.ZoneCode = reader[1].ToString();
                    var shapewkt = $"{reader[2] as string}#{srid}";
                    data.EnvelopShape = YuLinTu.Spatial.Geometry.FromString(shapewkt);
                    list.Add(data);
                }
            });
            return list;
        }

        /// <summary>
        /// 根据地域变化获取范围
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public List<EnData> GetEnDatas(string id, int srid)
        {
            List<EnData> list = new List<EnData>();
            GetDataConnection((conn, cmd) =>
            {
                var dataString = $"Select ID,ZoneCode,A0,A1,A2,B0,B1,B2,XIndex,YIndex,AsText(Shape) from EnData where ID='{id}'";
                cmd.CommandText = dataString;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var data = new EnData();
                    data.ID = reader[0] == null ? Guid.NewGuid() : Guid.Parse(reader[0].ToString());
                    data.ZoneCode = reader[1].ToString();
                    data.A0 = double.Parse(reader[2].ToString());
                    data.A1 = double.Parse(reader[3].ToString());
                    data.A2 = double.Parse(reader[4].ToString());
                    data.B0 = double.Parse(reader[5].ToString());
                    data.B1 = double.Parse(reader[6].ToString());
                    data.B2 = double.Parse(reader[7].ToString());
                    data.XIndex = int.Parse(reader[8].ToString());
                    data.YIndex = int.Parse(reader[9].ToString());
                    var shapewkt = $"{reader[10] as string}#{srid}";
                    data.Shape = YuLinTu.Spatial.Geometry.FromString(shapewkt);
                    list.Add(data);
                }
            });
            return list;
        }

        /// <summary>
        /// 根据地域变化获取范围
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <returns></returns>
        public List<EnData> GetByShapeinserts(string zonecode, Geometry shape)
        {
            List<EnData> list = new List<EnData>();
            GetDataConnection((conn, cmd) =>
            {
                var dataString = "Select ID,ZoneCode,A0,A1,A2,B0,B1,B2,XIndex,YIndex,AsText(Shape) from EnData where ZoneCode='{0}' and ST_Intersects(shape,{1})";
                string geoString = shape.AsText();
                if (shape.Instance.Coordinates.Length > 0 && shape.Instance.Coordinates[0].Z != double.NaN)
                {
                    geoString = geoString.Replace(" 0,", ",");
                    geoString = geoString.Replace(" 0))", "))");
                    if (shape.GeometryType == YuLinTu.Spatial.eGeometryType.Point ||
                        shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPoint ||
                        shape.GeometryType == YuLinTu.Spatial.eGeometryType.MultiPolygon)
                        geoString = geoString.Replace(" 0)", ")");
                }
                var shapstring = string.Format("GeomFromText('{0}',{1})", geoString, shape.Srid);
                dataString = string.Format(dataString,zonecode, shapstring);
                 
                cmd.CommandText = dataString;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var data = new EnData();
                    data.ID = reader[0] == null ? Guid.NewGuid() : Guid.Parse(reader[0].ToString());
                    data.ZoneCode = reader[1].ToString();
                    data.A0 = double.Parse(reader[2].ToString());
                    data.A1 = double.Parse(reader[3].ToString());
                    data.A2 = double.Parse(reader[4].ToString());
                    data.B0 = double.Parse(reader[5].ToString());
                    data.B1 = double.Parse(reader[6].ToString());
                    data.B2 = double.Parse(reader[7].ToString());
                    data.XIndex = int.Parse(reader[8].ToString());
                    data.YIndex = int.Parse(reader[9].ToString());
                    var shapewkt = $"{reader[10] as string}#{shape.Srid}";
                    data.Shape = YuLinTu.Spatial.Geometry.FromString(shapewkt);
                    list.Add(data);
                }
            });
            return list;
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int deleteDataByzoneCode(string sql)
        {
            int result = -1;
            GetDataConnection((conn, cmd) =>
            {
                cmd.CommandText = sql;
                result = cmd.ExecuteNonQuery();
            });
            return result;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="action"></param>
        public void GetDataConnection(Action<SQLiteConnection, SQLiteCommand> action)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dataSource))
            {
                conn.Open();
                string dataString = string.Empty;
                try
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT load_extension('libspatialite-4.dll');";
                        command.ExecuteScalar();
                    }
                    //创建表 
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        action(conn, cmd);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                }
            }
        }

        #endregion
    }
}
