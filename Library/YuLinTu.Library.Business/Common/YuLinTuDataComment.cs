/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据
    /// </summary>
    public class YuLinTuDataComment
    {
        #region Propertys

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliseName { get; set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        public string LayerName { get; set; }

        /// <summary>
        /// 映射信息
        /// </summary>
        public Dictionary<string, string> Mapping { get; set; }

        /// <summary>
        /// 当前对象集合
        /// </summary>
        public Object CurrentObject { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        public int RowValue { get; set; }

        #endregion

        #region Ctor

        public YuLinTuDataComment()
        {
            Mapping = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 系列化成Xml
        /// </summary>
        public static void SerializeXml(YuLinTuDataCommentCollection dataCollection)
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config";
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            string fileName = filePath + @"\DataConfigMapping.xml";
            if (dataCollection == null)
            {
                return;
            }
            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))//创建写入流
            {
                writer.Formatting = Formatting.Indented;//格式
                writer.WriteStartDocument();//写文档
                writer.WriteStartElement("ArrayOf" + "YuLinTuDataComment", "http://www.w3.org/2001/XMLSchema-instance");
                foreach (YuLinTuDataComment data in dataCollection)
                {
                    writer.WriteStartElement("YuLinTuDataComment");//开始属性节点
                    writer.WriteElementString("IsSelect", data.Checked.ToString());
                    writer.WriteElementString("Name", data.Name);
                    writer.WriteElementString("AliseName", data.AliseName);
                    writer.WriteElementString("LayerName", data.LayerName);
                    writer.WriteElementString("RowValue", data.RowValue.ToString());
                    writer.WriteStartElement("Mapping");//开始属性节点
                    foreach (KeyValuePair<string, string> keyValue in data.Mapping)
                    {
                        writer.WriteElementString(keyValue.Key, keyValue.Value);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();//结束文档
                writer.Flush();//刷新
                writer.Close();//关闭
            }
        }

        /// <summary>
        /// 系列化数据
        /// </summary>
        /// <param name="reader"></param>
        public static YuLinTuDataCommentCollection DeserializeXml()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + @"\Config";
            if (!System.IO.Directory.Exists(filePath))
            {
                return new YuLinTuDataCommentCollection();
            }
            string fileName = filePath + @"\DataConfigMapping.xml";
            if (!System.IO.File.Exists(fileName))
            {
                return new YuLinTuDataCommentCollection();
            }
            YuLinTuDataCommentCollection dataCollection = DeserializeXml(fileName);
            return dataCollection;
        }

        /// <summary>
        /// 系列化数据
        /// </summary>
        /// <param name="reader"></param>
        private static YuLinTuDataCommentCollection DeserializeXml(string xmlFileName)
        {
            YuLinTuDataCommentCollection dataCollection = new YuLinTuDataCommentCollection();
            if (string.IsNullOrEmpty(xmlFileName))
            {
                return dataCollection;
            }
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(xmlFileName);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return new YuLinTuDataCommentCollection();
            }
            XmlElement element = xmlDocument.DocumentElement;
            XmlNodeList nodeList = element.ChildNodes;
            for (int index = 0; index < nodeList.Count; index++)
            {
                XmlNode xmlNode = nodeList.Item(index);
                CreateEntityByXmlNode(xmlNode, dataCollection);
            }
            return dataCollection;
        }

        /// <summary>
        /// 获取正确的Xml节点
        /// </summary>
        /// <param name="xmlNode">xml节点</param>
        /// <returns></returns>
        private static void CreateEntityByXmlNode(XmlNode xmlNode, YuLinTuDataCommentCollection dataCollection)
        {
            XmlNode node = xmlNode;
            if (node != null && !string.IsNullOrEmpty(node.Name) && node.Name == "YuLinTuDataComment")
            {
                YuLinTuDataComment data = LoadXmlData(node.ChildNodes);
                if (data != null)
                {
                    dataCollection.Add(data);
                }
                return;
            }
            if (node.HasChildNodes)
            {
                XmlNodeList nodeList = node.ChildNodes;
                for (int index = 0; index < nodeList.Count; index++)
                {
                    XmlNode nextNode = nodeList.Item(index);
                    CreateEntityByXmlNode(nextNode, dataCollection);
                }
            }
        }

        /// <summary>
        /// 加载Xml数据
        /// </summary>
        /// <param name="nodeList">节点列表</param>
        /// <returns></returns>
        private static YuLinTuDataComment LoadXmlData(XmlNodeList nodeList)
        {
            YuLinTuDataComment data = new YuLinTuDataComment();
            string value = string.Empty;
            for (int index = 0; index < nodeList.Count; index++)
            {
                XmlNode xmlNode = nodeList.Item(index);
                if (xmlNode.Name == "Mapping")
                {
                    for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                    {
                        XmlNode node = xmlNode.ChildNodes[i];
                        data.Mapping.Add(node.Name, GetXmlNodeValue(node));
                    }
                    continue;
                }
                if (xmlNode.Name == "IsSelect")
                {
                    data.Checked = System.Convert.ToBoolean(GetXmlNodeValue(xmlNode));
                    continue;
                }
                if (xmlNode.Name == "Name")
                {
                    data.Name = GetXmlNodeValue(xmlNode);
                    continue;
                }
                if (xmlNode.Name == "AliseName")
                {
                    data.AliseName = GetXmlNodeValue(xmlNode);
                    continue;
                }
                if (xmlNode.Name == "LayerName")
                {
                    data.LayerName = GetXmlNodeValue(xmlNode);
                }
                if (xmlNode.Name == "RowValue")
                {
                    data.RowValue = Convert.ToInt32(GetXmlNodeValue(xmlNode));
                }
            }
            return data;
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string GetXmlNodeValue(XmlNode node)
        {
            if (node.FirstChild == null)
            {
                return node.Value;
            }
            return node.FirstChild.Value;
        }

        #endregion
    }

    /// <summary>
    /// 数据信息集合
    /// </summary>
    public class YuLinTuDataCommentCollection : List<YuLinTuDataComment>
    {
    }
}