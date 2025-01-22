/*
 * (C) 2016 公司版权所有,保留所有权利
*/
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 序列化
    /// </summary>
    public class ToolSerialization
    {
        #region Methods - Serialization

        public static void SerializeBinary(string fileName, object obj)
        {
            FileStream stream = File.Create(fileName);

            BinaryFormatter binFmt = new BinaryFormatter();
            binFmt.Serialize(stream, obj);
            stream.Close();
        }

        public static object DeserializeBinary(string fileName, Type type)
        {
            FileStream stream = File.OpenRead(fileName);

            BinaryFormatter binFmt = new BinaryFormatter();
            object obj = binFmt.Deserialize(stream);
            stream.Close();

            return obj;
        }

        public static object DeserializeBinary(string fileName, Type type, SerializationBinder binder)
        {
            FileStream stream = File.OpenRead(fileName);

            BinaryFormatter binFmt = new BinaryFormatter();
            binFmt.Binder = binder;

            object obj = binFmt.Deserialize(stream);
            stream.Close();

            return obj;
        }

        public static void SerializeXml(string fileName, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static void SerializeXml(string fileName, object obj, Type[] types)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), types);
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static void SerializeXml(string fileName, object obj, XmlAttributeOverrides over)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), over);
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public static object DeserializeXml(string fileName, Type type)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(type);
            using (var reader = new StreamReader(fileName))
            {
                object obj = serializer.Deserialize(reader);
                return obj;
            }
        }

        public static object DeserializeXml(string fileName, Type type, Type[] types)
        {
            XmlSerializer serializer = new XmlSerializer(type, types);
            using (var reader = new StreamReader(fileName))
            {
                object obj = serializer.Deserialize(reader);
                return obj;
            }
        }

        public static string SerializeXmlString(object obj)
        {
            string xml = null;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
            }
            return xml;
        }

        public static string SerializeXmlString(object obj, Type[] types)
        {
            string xml = null; XmlSerializer serializer = new XmlSerializer(obj.GetType(), types);
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
            }
            return xml;
        }

        public static string SerializeXmlString(object obj, XmlAttributeOverrides over)
        {
            string xml = null;
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), over);
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xml = writer.ToString();
            }
            return xml;
        }

        public static object DeserializeXmlString(string objXml, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            using (var reader = new StringReader(objXml))
            {
                object obj = serializer.Deserialize(reader);
                return obj;
            }
        }

        #endregion
    }
}
