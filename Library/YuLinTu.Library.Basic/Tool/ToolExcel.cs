using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;
using System.Data;

namespace YuLinTu.Library.Basic
{
    public class ToolExcel
    {
        #region OleDb_ReadExcel

        public static object[,] GetExcelData(OleDbConnection OleDbConn, string filePath, int cellCount)
        {
            return ReadeExcel(OleDbConn, filePath, cellCount);
        }

        private static object[,] ReadeExcel(OleDbConnection OleDbConn, string filePath, int cellCount)
        {
            string fileName = Path.GetFileName(filePath);
            string conn = string.Concat("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= ", filePath, ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'"); ;
            if (fileName.EndsWith(".xlsx"))
            {
                //excel2007 连接字符串
                conn = string.Concat("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", filePath, ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'");
            }

            OleDbConn.ConnectionString = conn;
            OleDbConn.Open();
            DataTable dt = OleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });//获取excel工作表信息
            string sheetNane;
            DataSet ds = new DataSet();
            //加载Excel中的数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sheetNane = dt.Rows[i]["TABLE_NAME"].ToString().Trim();
                if (IsTable(sheetNane))
                {
                    //SheetName为excel中表的名字
                    string sql = string.Concat("select * from [", sheetNane, "]");
                    OleDbCommand cmd = new OleDbCommand(sql, OleDbConn);
                    OleDbDataAdapter ad = new OleDbDataAdapter(cmd);
                    try
                    {
                        ad.Fill(ds, sheetNane);
                    }
                    catch
                    {
                    }
                }
            }
            //寻找符合标准的表
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (ds.Tables[i].Columns.Count < cellCount)
                {
                    ds.Tables.Remove(ds.Tables[i].TableName);
                    i--;
                }
            }
            OleDbConn.Close();

            if (ds.Tables.Count < 1)
            {
                return null;
            }

            //因为用OleDB方式读取出来的行数比Excel中少了1行
            object[,] allItem = new object[ds.Tables[0].Rows.Count + 1, ds.Tables[0].Columns.Count];
            int rangeCount = ds.Tables[0].Rows.Count + 1;

            //给object【，】数组赋值，方便使用，提高效率
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    allItem[i, j] = ds.Tables[0].Rows[i][j];
                }
            }
            return allItem;
        }

        private static bool IsTable(string tableName)
        {
            bool blIsTable = false;
            if (tableName != string.Empty)
            {
                int nIndex = tableName.LastIndexOf("$");
                if (nIndex > 0
                    && !tableName.ToLower().Contains("_filterdatabase")
                    && !tableName.ToLower().Contains("'$_")
                    && !tableName.ToLower().Contains("$'_"))
                {
                    blIsTable = true;
                }
            }
            return blIsTable;
        }

        #endregion
    }
}
