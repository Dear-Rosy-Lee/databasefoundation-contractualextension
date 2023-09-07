using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace YuLinTu.Library.Basic
{
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
}
