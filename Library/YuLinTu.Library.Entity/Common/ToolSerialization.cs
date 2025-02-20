/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public class ToolSerialize
    {
        #region Methods - Serialization

        /// <summary>
        /// 序列化文件成xml字符串
        /// </summary>
        public static string SerializeXmlString<T>(T obj) where T : class
        {
            if (obj == null)
            {
                return string.Empty;
            }
            string xml = null;
            StringWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                writer = new StringWriter();
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
                writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            { 
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return xml;
        }

        /// <summary>
        /// 反序列化xml字符串为指定类型
        /// </summary>
        public static T DeserializeXmlString<T>(string objXml) where T : class,new()
        {
            if (string.IsNullOrEmpty(objXml))
            {
                return null;
            }
            StringReader reader = null;
            try
            {
                Type type = typeof(T);
                XmlSerializer serializer = new XmlSerializer(type);
                reader = new StringReader(objXml);
                object obj = serializer.Deserialize(reader);
                reader.Close();
                return obj as T;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        #endregion
    }
}
