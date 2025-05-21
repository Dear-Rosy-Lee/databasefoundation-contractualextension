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

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public class ToolSerialization
    {
        #region Methods - Serialization

        /// <summary>
        /// 序列化到文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="obj">内容</param>
        public static void SerializeBinary(string fileName, object obj)
        {
            FileStream stream = File.Create(fileName);
            BinaryFormatter binFmt = new BinaryFormatter();
            binFmt.Binder = new NonVersionedSerializationBinder();
            binFmt.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        /// <param name="fileName">需反序列化的文件路径</param>
        /// <param name="type">需反序列化成的类型</param>
        /// <returns></returns>
        public static object DeserializeBinary(string fileName, Type type)
        {
            FileStream stream = File.OpenRead(fileName);

            BinaryFormatter binFmt = new BinaryFormatter();
            object obj = binFmt.Deserialize(stream);
            stream.Close();

            return obj;
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        public static object DeserializeBinary(string fileName, Type type, SerializationBinder binder)
        {
            FileStream stream = File.OpenRead(fileName);

            BinaryFormatter binFmt = new BinaryFormatter();
            binFmt.Binder = binder;

            object obj = binFmt.Deserialize(stream);
            stream.Close();

            return obj;
        }

        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static void SerializeXml(string fileName, object obj)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                writer = new StreamWriter(fileName, false, Encoding.UTF8);
                serializer.Serialize(writer, obj);
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                throw ex;
            }
        }

        ///<summary>
        /// 序列化到文件
        /// </summary>
        public static void SerializeXml(string fileName, object obj, Type[] types)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType(), types);
                writer = new StreamWriter(fileName, false, Encoding.UTF8);
                serializer.Serialize(writer, obj);
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                throw ex;
            }
        }

        ///<summary>
        /// 序列化到文件
        /// </summary>
        public static void SerializeXml(string fileName, object obj, XmlAttributeOverrides over)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType(), over);
                writer = new StreamWriter(fileName, false, Encoding.UTF8);
                serializer.Serialize(writer, obj);
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                throw ex;
            }
        }

        ///<summary>
        /// 反序列化文件
        /// </summary>
        public static object DeserializeXml(string fileName, Type type)
        {
            StreamReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                reader = new StreamReader(fileName);
                object obj = serializer.Deserialize(reader);
                reader.Close();

                return obj;
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();

                throw ex;
            }
        }

        ///<summary>
        /// 反序列化文件
        /// </summary>
        public static object DeserializeXml(string fileName, Type type, Type[] types)
        {
            StreamReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(type, types);
                reader = new StreamReader(fileName);
                object obj = serializer.Deserialize(reader);
                reader.Close();

                return obj;
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();

                throw ex;
            }
        }

        /// <summary>
        /// 序列化文件成xml字符串
        /// </summary>
        public static string SerializeXmlString(object obj)
        {
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
                if (writer != null)
                    writer.Close();

                throw ex;
            }

            return xml;
        }

        /// <summary>
        /// 序列化文件成xml字符串
        /// </summary>
        public static string SerializeXmlString(object obj, Type[] types)
        {
            string xml = null;
            StringWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType(), types);
                writer = new StringWriter();
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                throw ex;
            }

            return xml;
        }

        /// <summary>
        /// 序列化文件成xml字符串
        /// </summary>
        public static string SerializeXmlString(object obj, XmlAttributeOverrides over)
        {
            string xml = null;
            StringWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType(), over);
                writer = new StringWriter();
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                throw ex;
            }

            return xml;
        }

        /// <summary>
        /// 反序列化xml字符串为指定类型
        /// </summary>
        public static object DeserializeXmlString(string objXml, Type type)
        {
            StringReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                reader = new StringReader(objXml);
                object obj = serializer.Deserialize(reader);
                reader.Close();

                return obj;
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();

                throw ex;
            }
        }

        #endregion
    }
    class NonVersionedSerializationBinder : SerializationBinder
    {
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            if (serializedType.FullName.StartsWith("YuLinTu"))
            {
                assemblyName = serializedType.Assembly.GetName().Name;
                typeName = serializedType.FullName;

            }
            else
                base.BindToName(serializedType, out assemblyName, out typeName);
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return null;
        }
    }
}
