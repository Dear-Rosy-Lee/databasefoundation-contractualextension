using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace YuLinTu.Library.Basic
{
    /// <summary>
    /// 鱼鳞图类别配置
    /// </summary>
    public class ToolCategory
    {
        #region Methods

        /// <summary>
        /// 系列化成Xml
        /// </summary>
        public static void SerializeXml(LayoutValueCollection dataCollection)
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config";
            if (!System.IO.Directory.Exists(filePath))
            {
                return;
            }
            string fileName = filePath + @"\LayoutCategoryConfig.xml";
            if (dataCollection == null || dataCollection.Count == 0)
            {
                return;
            }
            YuLinTu.Library.Basic.ToolSerialization.SerializeXml(fileName, dataCollection);
        }

        /// <summary>
        /// 反系列化数据
        /// </summary>
        /// <param name="reader"></param>
        public static LayoutValueCollection DeserializeXml()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config";
            if (!System.IO.Directory.Exists(filePath))
            {
                return new LayoutValueCollection();
            }
            string fileName = filePath + @"\LayoutCategoryConfig.xml";
            if (!System.IO.File.Exists(fileName))
            {
                return new LayoutValueCollection();
            }
            LayoutValueCollection dataCollection = YuLinTu.Library.Basic.ToolSerialization.DeserializeXml(fileName, typeof(LayoutValueCollection)) as LayoutValueCollection;
            return dataCollection;
        }

        #endregion
    }

    /// <summary>
    /// 布局类型值
    /// </summary>
    public class LayoutValue
    {
        #region Propertys

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 是否可高亮
        /// </summary>
        public bool Falsh { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliseName { get; set; }

        /// <summary>
        /// 当前对象
        /// </summary>
        public Object CurrentObject { get; set; }

        /// <summary>
        /// 标签项索引值
        /// </summary>
        public int TabItemIndex { get; set; }

        #endregion

        #region Ctor

        public LayoutValue()
        {

        }

        #endregion
    }

    /// <summary>
    /// 布局类型集合
    /// </summary>
    public class LayoutValueCollection : List<LayoutValue>
    {
    }
}
