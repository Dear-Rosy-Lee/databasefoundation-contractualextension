using System;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Spatial;


namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    /// <summary>
    /// Reference帮助类
    /// </summary>
    public class ReferenceHelper
    {
        /// <summary>
        /// 获取指定表的坐标信息
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="tableName">表名称</param>
        /// <param name="shapFiled">字段名称</param>
        /// <returns></returns>
        public static string GetDbProjectInfo<T>(IDbContext db, string tableName, string shapFiled)
        {
            string sqlText = string.Format("SELECT srtext FROM geom_cols_ref_sys WHERE Lower(f_table_name) = Lower('{0}') AND Lower(f_geometry_column) = Lower('{1}')", tableName, shapFiled);
            var qc = db.CreateQuery<T>();
            qc.CommandContext.CommandText.Clear();
            qc.CommandContext.CommandText.Append(sqlText);
            var list = qc.GetTable();
            var srt = list.Rows[0][0] as string;
            return srt;
        }

        /// <summary>
        /// 获取指定表的Reference
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="tableName">表名称</param>
        /// <param name="shapFiled">字段名称</param>
        /// <returns></returns>
        public static SpatialReference GetDbReference<T>(IDbContext db, string tableName, string shapFiled)
        {
            SpatialReference sr = null;
            try
            {
                var type = db.DataSource.ToString();
                if (type == "Common.SQLite")
                {
                    string sqlText = string.Format("SELECT * FROM geom_cols_ref_sys WHERE Lower(f_table_name) = Lower('{0}') AND Lower(f_geometry_column) = Lower('{1}')", tableName, shapFiled);
                    var qc = db.CreateQuery<T>();
                    qc.CommandContext.CommandText.Clear();
                    qc.CommandContext.CommandText.Append(sqlText);
                    var list = qc.GetTable();
                    var srid = (long)list.Rows[0]["srid"];
                    // var srtext = (string)list.Rows[0]["srtext"];
                    sr = new Spatial.SpatialReference((int)srid);//, srtext);
                }
            }
            catch
            {
                throw new Exception("获取数据库表{0}中字段{1}的空间坐标信息发生错误!");
            }
            return sr;
        }
        /// <summary>
        /// 获取图形Reference
        /// </summary>
        public static SpatialReference GetShapeReference(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            string prjfileName = Path.ChangeExtension(fileName, ".prj");
            if (!File.Exists(prjfileName))
            {
                return null;
            }
            StreamReader sReader = new StreamReader(prjfileName, System.Text.Encoding.GetEncoding("GB2312"));
            string sridString = sReader.ReadToEnd();
            return new SpatialReference(sridString);
        }
    }
}
