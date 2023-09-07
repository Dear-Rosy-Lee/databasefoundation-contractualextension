using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.MapFoundation.Configuration
{
    /// <summary>
    /// 鱼鳞图-打开属性表字段属性设置
    /// </summary>
    public class PropertyTableDefine
    {
        #region Properties

        #endregion

        #region 获取数据库表
        /// <summary>
        /// 反序列化
        /// </summary>
        public static List<ProTable> DeserializeProTable()
        {
            try
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Template\数据库字典.doc";
                if (!File.Exists(fileName))
                {
                    return new List<ProTable>();
                }
                //List<ProTable> tableList = ToolSerialization.DeserializeXml(fileName, typeof(List<ProTable>)) as List<ProTable>;
                LoadPropertyTable load = new LoadPropertyTable();
                load.OpenTemplate(fileName);
                List<ProTable> tableList = new List<ProTable>();
                tableList = load.Read();
                if (tableList == null)
                {
                    return new List<ProTable>();
                }
                else
                {
                    return tableList;
                }
            }
            catch
            {
                return new List<ProTable>();
            }
        }

        #endregion
    }

    /// <summary>
    /// 数据库表
    /// </summary>
    public class ProTable
    {
        #region Filds

        #endregion

        #region Property

        /// <summary>
        /// 表格名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<SetField> FieldList { get; set; }

        #endregion
    }

    /// <summary>
    /// 数据库字段
    /// </summary>
    public class SetField
    {
        #region Properties

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段别名
        /// </summary>
        public string AliseName { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEdit { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible { get; set; }      

        #endregion
    }
}
