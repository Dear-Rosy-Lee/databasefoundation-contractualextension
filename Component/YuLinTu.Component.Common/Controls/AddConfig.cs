/*
 * (C) 2015 鱼鳞图公司版权所有，保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace YuLinTu.Component.Common
{
   public class AddConfig
    {
        /// <summary>
        /// 配置文件节点填写
        /// </summary>
        /// <returns>成功：true/ 错误：false</returns>
        public static bool WriteAppConfigByXML()
        {
            bool isOk = false;
            try
            {
                StringBuilder stringbuilder = new StringBuilder();

                XmlDocument xml = new XmlDocument();
                xml.Load(Application.ExecutablePath + ".config");

                //	<providerSettings>
                XmlNode node = xml.SelectSingleNode("configuration//providerSettings//providers");
                if (!CheckConfig(node))
                {
                    string innerXml = node.InnerXml;
                    stringbuilder.Append(innerXml);
                    stringbuilder.Append("<add name=\"Common.OleDb\"  type =\"YuLinTu.Data.OleDb.ProviderDbCOle, YuLinTu.Data.OleDb\"></add><add name=\"Common.Access\"  type =\"YuLinTu.Data.OleDb.ProviderDbCAccess, YuLinTu.Data.OleDb\"></add><add name=\"Common.SQLite\" type =\"YuLinTu.Data.SQLite.ProviderDbCSQLite, YuLinTu.Data.SQLite\"></add>");
                    node.InnerXml = stringbuilder.ToString();
                }
                stringbuilder.Clear();

                //	<dataSourceSettings>
                XmlNode node2 = xml.SelectSingleNode("configuration//dataSourceSettings//dataSources");
                if (!CheckConfig(node2))
                {
                    string innerXml2 = node2.InnerXml;
                    stringbuilder.Append(innerXml2);
                    stringbuilder.Append("<add name=\"Common.OleDb\" type =\"YuLinTu.Data.DbContext, YuLinTu.Data\"></add><add name=\"Common.Access\"  type =\"YuLinTu.Data.DbContext, YuLinTu.Data\"></add><add name=\"Common.SQLite\"  type =\"YuLinTu.Data.DbContext, YuLinTu.Data\"></add>");
                    node2.InnerXml = stringbuilder.ToString();
                }

                xml.Save(Application.ExecutablePath + ".config");
                isOk = true;

                return isOk;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 检查节点是否添加
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>成功：true/ 错误：false</returns>
        private static bool CheckConfig(XmlNode node)
        {
            bool isOK = false;
            List<string> lst = new List<string>() { "Common.OleDb", "Common.Access", "Common.SQLite" };
            var childnodes = node.ChildNodes;
            foreach (var item in childnodes)
            {
                XmlElement element = item as XmlElement;
                if (element != null)
                {
                    var attribute = element.Attributes[0];
                    bool result = lst.Contains(attribute.InnerText);
                    if (result)
                    {
                        isOK = true;
                        break;
                    }
                }
            }
            return isOK;
        }
    }
}
