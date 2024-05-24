//(C) 2013 鱼鳞图公司版权所有，保留所有权利。
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DatabaseOperator
    {
        #region Progress

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <returns></returns>
        public static bool UpdateDatabase()
        {
            try
            {
                AddDatabaseTable();
                UpdateTableField();
                return true;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 添加数据库表
        /// </summary>
        /// <returns></returns>
        public static bool AddDatabaseTable()
        {
            //string connectionName = InitalizeConnectionName();
            //List<System.Configuration.ConnectionStringSettings> list = YuLinTu.Library.Basic.ToolConfiguration.GetConnectionStringSettingsList();
            //foreach (System.Configuration.ConnectionStringSettings conString in list)
            //{
            //    if (conString.Name != connectionName)
            //    {
            //        continue;
            //    }
            //    switch (conString.ProviderName)
            //    {
            //        case "Common.Access":
            //            AccessTableProgress(conString.ConnectionString);
            //            break;
            //        case "Common.SqlServer":
            //            SqlServerTableProgress(conString.ConnectionString);
            //            break;
            //        case "ArcGIS.FileGDB":
            //            break;
            //    }
            //    break;
            //}
            //return true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新数据库字段
        /// </summary>
        public static bool UpdateTableField()
        {
            //string connectionName = InitalizeConnectionName();
            //List<System.Configuration.ConnectionStringSettings> list = YuLinTu.Library.Basic.ToolConfiguration.GetConnectionStringSettingsList();
            //foreach (System.Configuration.ConnectionStringSettings conString in list)
            //{
            //    if (conString.Name != connectionName)
            //    {
            //        continue;
            //    }
            //    switch (conString.ProviderName)
            //    {
            //        case "Common.Access":
            //            UpdateAccessField(conString.ConnectionString);
            //            break;
            //        case "Common.SqlServer":
            //            UpdateSqlServerField(conString.ConnectionString);
            //            break;
            //        case "ArcGIS.FileGDB":
            //            break;
            //    }
            //    break;
            //}
            //return true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化连接字符串名称
        /// </summary>
        /// <returns></returns>
        private static string InitalizeConnectionName()
        {
            string connectionString = string.Empty;
            string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath, ConfigurationManager.AppSettings["ProfileDirectoryName"]);
            fileName = Path.Combine(fileName, ConfigurationManager.AppSettings["BnsFileName"]);
            if (!System.IO.File.Exists(fileName))
            {
                return string.Empty;
            }
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(fileName);
                XmlElement element = xmlDocument.DocumentElement;
                XmlNodeList nodeList = element.ChildNodes;
                for (int index = 0; index < nodeList.Count; index++)
                {
                    XmlNode xmlNode = nodeList.Item(index);
                    if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.Name) && xmlNode.Name == "CommonDataSource")
                    {
                        if (!string.IsNullOrEmpty(xmlNode.InnerText))
                        {
                            connectionString = xmlNode.InnerText;
                        }
                        else if (xmlNode.FirstChild != null)
                        {
                            connectionString = xmlNode.FirstChild.Value;
                        }
                        else { }
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return connectionString;
        }

        #endregion

        #region Access

        /// <summary>
        /// Access表处理
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static bool AccessTableProgress(string connectionString)
        {
            bool success = false;
            try
            {
                success = AddTableVirtualPerson(connectionString);
                success = AddSecondTableLand(connectionString);
                success = AddAgricultureAffair(connectionString);
                AddTableField(connectionString);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return success;
        }

        /// <summary>
        /// 更新Access字段
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool UpdateAccessField(string connectionString)
        {
            try
            {
                using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))//更新表中字段类型值
                {
                    connection.Open();
                    string cmdString = "ALTER TABLE TableVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE LandVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE YardVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE WoodVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE HouseVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE CollectiveLandVirtualPerson ALTER COLUMN [OtherInfomation] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE ContractLand ALTER COLUMN [ArableLandTime] Memo";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE BuildLandBoundaryAddressDot ALTER COLUMN [XCoordinate] Double";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE BuildLandBoundaryAddressDot ALTER COLUMN [YCoordinate] Double";
                    cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return true;
        }

        /// <summary>
        /// 添加表字段
        /// </summary>
        /// <returns></returns>
        private static void AddTableField(string connectionString)
        {
            try
            {
                //添加承包方表字段
                using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
                {
                    connection.Open();
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray == null || row.ItemArray.Length <= 2
                            || row.ItemArray[2] == null)
                        {
                            continue;
                        }
                        switch (row.ItemArray[2].ToString())
                        {
                            case "LandVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "YardVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "TableVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "WoodVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "HouseVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "CollectiveLandVirtualPerson":
                                AddVirtualPersonField(connection, row);
                                break;
                            case "ContractConcord":
                                AddContractConcordField(connection, row);
                                break;
                            case "ContractLand":
                                AddContractLandField(connection, row);
                                break;
                            case "Person":
                                AddPersonField(connection, row);
                                break;
                            case "CollectivityTissue":
                                AddCollectivityTissueField(connection, row);
                                break;
                            case "BuildLandBoundaryAddressDot":
                                AddDotTableField(connection, row);
                                break;
                            case "BuildLandBoundaryAddressCoil":
                                AddLineTableField(connection, row);
                                break;
                            case "ContractRegeditBook":
                                AddContractRegeditBookField(connection, row);
                                break;
                            case "ContractRequireTable":
                                AddContractRequireBookField(connection, row);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddVirtualPersonField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "Telephone");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [Telephone] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PersonCount");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PersonCount] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "TotalArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [TotalArea] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PostalNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PostalNumber] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "FamilyNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [FamilyNumber] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "TotalActualArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [TotalActualArea] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "TotalAwareArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [TotalAwareArea] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "TotalModoArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [TotalModoArea] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "TotalTableArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [TotalTableArea] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "OtherInfomation");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [OtherInfomation] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendA] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendB] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendC] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "CredentialsType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [CredentialsType] Int";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddContractLandField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "LandName");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LandName] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PlatType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PlatType] Int32";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ConstructMode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ConstructMode] Int32";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ArableLandTime");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ArableLandTime] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "CadastralZoneCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [CadastralZoneCode] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PertainToArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PertainToArea] LONG";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "WidthHeight");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [WidthHeight] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameA] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameB] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameC] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameD");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameD] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameE");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameE] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "AliasNameF");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [AliasNameF] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddContractConcordField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendA] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendB] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendC] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PersonAvgArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PersonAvgArea] LONG";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SecondContracterLocated");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SecondContracterLocated] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SecondContracterName");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SecondContracterName] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityChronicle");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityChronicle] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityChroniclePerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityChroniclePerson] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityDate] DateTime";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityResult");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityResult] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityContractor");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityContractor] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityResultDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityResultDate] DateTime";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityCheckOpinion");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityCheckOpinion] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityCheckPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityCheckPerson] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PublicityCheckDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PublicityCheckDate] DateTime";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddPersonField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "IsFarmer");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [IsFarmer] Int";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtensionPackageNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtensionPackageNumber] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "IsDeaded");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [IsDeaded] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LocalMarriedRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LocalMarriedRetreatLand] LONG";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "PeasantsRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [PeasantsRetreatLand] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ForeignMarriedRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ForeignMarriedRetreatLand] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SharePerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SharePerson] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "IsSharedLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [IsSharedLand] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendA] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendB] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendC] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendD");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendD] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendE");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendE] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ExtendF");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ExtendF] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加集体经济组织字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddCollectivityTissueField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "LawyerTelephone");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LawyerTelephone] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LawyerAddress");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LawyerAddress] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LawyerPosterNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LawyerPosterNumber] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LawyerCredentType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LawyerCredentType] Int";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LawyerCartNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LawyerCartNumber] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SurveyPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SurveyPerson] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SurveyDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SurveyDate] DateTime";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SurveyChronicle");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SurveyChronicle] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ChenkPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ChenkPerson] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ChenkDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ChenkDate] DateTime";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "ChenkOpinion");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [ChenkOpinion] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址点表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddDotTableField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "Description");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [Description] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SenderCode] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LandType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LandType] Int";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址线表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddLineTableField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "Description");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [Description] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SenderCode] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "NeighborPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [NeighborPerson] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "NeighborFefer");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [NeighborFefer] Text";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeFieldInformation(dataReader, "LandType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [LandType] Int";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址线表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddContractRegeditBookField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SenderCode] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加申请表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddContractRequireBookField(System.Data.OleDb.OleDbConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            System.Data.OleDb.OleDbCommand readCommand = new System.Data.OleDb.OleDbCommand(cmdString, connection);
            System.Data.OleDb.OleDbDataReader dataReader = readCommand.ExecuteReader();
            System.Data.OleDb.OleDbCommand cmd = null;
            bool isExist = InitalizeFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD COLUMN [SenderCode] Memo";
                cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 判断字段信息
        /// </summary>
        /// <returns></returns>
        private static bool InitalizeFieldInformation(System.Data.OleDb.OleDbDataReader dataReader, string fieldName)
        {
            bool exist = false;
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                if (dataReader.GetName(index) == fieldName)
                {
                    exist = true;
                    break;
                }
            }
            if (!dataReader.IsClosed)
            {
                dataReader.Close();
            }
            return exist;
        }

        /// <summary>
        /// 添加二轮地块表
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool AddTableVirtualPerson(string connectionString)
        {
            bool success = false;
            using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "TableVirtualPerson")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE TableVirtualPerson ([ID] GUID, [Name] Memo, [Number] Memo, [SharePerson] Memo, ";
                    cmdString += "[LandLocated] Memo, [SenderCode] Memo, [VirtualType] Int, [SourceID] GUID, [Founder] Memo, ";
                    cmdString += "[CreationTime] DateTime, [Modifier] Memo, [ModifiedTime] DateTime, [Comment] Memo, ";
                    cmdString += "[Status] Int, [Telephone] Memo, [PersonCount] Memo, [TotalArea] Memo, [PostalNumber] Memo, ";
                    cmdString += "[FamilyNumber] Memo, [TotalActualArea] Memo, [TotalAwareArea] Memo, [TotalModoArea] Memo, ";
                    cmdString += "[TotalTableArea] Memo, [OtherInfomation] Memo, [ExtendA] Memo, [ExtendB] Memo,[ExtendC] Memo)";
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        /// <summary>
        /// 添加二轮承包方表
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool AddSecondTableLand(string connectionString)
        {
            bool success = false;
            using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "SecondTableLand")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE SecondTableLand ([ID] GUID, [CadastralNumber] Memo, [Name] Memo, ";
                    cmdString += "[LandNumber] Memo, [HouseHolderName] Memo, [HouseHolderId] GUID, [LandCode] Memo, [ContractType] Int, ";
                    cmdString += "[LandLevel] Int, [PlantType] Int, [LandScopeLevel] Int, [LandNeighbor] Memo, ";
                    cmdString += "[OwnRightType] Int, [OwnRightCode] Text, [OwnRightName] Text, [ZoneCode] Text, [ZoneName] Text, ";
                    cmdString += "[LineArea] LONG, [TableArea] LONG, [ActualArea] LONG, [AwareArea] LONG, ";
                    cmdString += "[IsFarmerLand] Int, [Purpose] Int, [ManagementType] LONG, [Soiltype] Memo, ";
                    cmdString += "[IsFlyLand] Int, [ConcordId] LONG, [Founder] Memo, [CreationTime] DateTime, ";
                    cmdString += "[Modifier] Memo, [ModifiedTime] DateTime, [Comment] Memo, [Status] Int, ";
                    cmdString += "[FormerPerson] Memo, [PlotNumber] Memo, [MotorizeLandArea] LONG, [IsTransfer] Int, ";
                    cmdString += "[TransferType] Int, [TransferTime] Memo, [TransferWhere] Memo, [ExtendA] Memo, ";
                    cmdString += "[ExtendB] Memo, [ExtendC] Memo, [WidthHeight] Memo, [ArableLandTime] Memo, ";
                    cmdString += "[PertainToArea] LONG, [CadastralZoneCode] Text, [LandName] Text, [PlatType] Int, ";
                    cmdString += "[ConstructMode] Int, [AliasNameA] Text, [AliasNameB] Text, [AliasNameC] Text, ";
                    cmdString += "[AliasNameD] Text, [AliasNameE] Text, [AliasNameF] Text)";
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        /// <summary>
        /// 添加业务表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static bool AddAgricultureAffair(string connectionString)
        {
            bool success = false;
            using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "AgricultureAffair")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE AgricultureAffair ([ID] GUID, [Name] Memo, [Category] Int, [OwnershipCategory] Int, ";
                    cmdString += "[Description] Memo, [WorkAgrument] Memo, [Status] Int, [SenderCode]  Memo,[SenderName]  Memo, [Creater] Memo, ";
                    cmdString += "[CreaterTime] DateTime, [Modifyer] Memo, [ModifyerTime] DateTime, [Comment] Memo)";
                    System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        #endregion

        #region SqlServer

        /// <summary>
        /// 更新Access字段
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool UpdateSqlServerField(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))//更新表中字段类型值
                {
                    connection.Open();
                    string cmdString = "ALTER TABLE TableVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE LandVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE YardVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE WoodVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE HouseVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE CollectiveLandVirtualPerson ALTER COLUMN [OtherInfomation] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                    cmdString = "ALTER TABLE ContractLand ALTER COLUMN [ArableLandTime] nText";
                    cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return true;
        }

        /// <summary>
        /// SqlServer表处理
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static bool SqlServerTableProgress(string connectionString)
        {
            bool success = false;
            try
            {
                success = AddSqlServerTableVirtualPerson(connectionString);
                success = AddSqlServerSecondTableLand(connectionString);
                success = AddSqlServerAgricultureAffair(connectionString);
                AddSqlServerTableField(connectionString);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return success;
        }

        /// <summary>
        /// 添加二轮地块表
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool AddSqlServerTableVirtualPerson(string connectionString)
        {
            bool success = false;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "TableVirtualPerson")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE TableVirtualPerson ([ID] [uniqueidentifier] NOT NULL, [Name] [nvarchar](150) NULL, [Number] [nvarchar](150) NULL, [SharePerson] [xml] NULL, ";
                    cmdString += "[LandLocated] [nvarchar](200) NULL, [SenderCode] [nvarchar](50) NOT NULL, [VirtualType] [int] NOT NULL, [SourceID] [uniqueidentifier] NULL, [Founder] [nvarchar](50) NULL, ";
                    cmdString += "[CreationTime] [datetime] NULL, [Modifier] [nvarchar](50) NULL, [ModifiedTime] [datetime] NULL, [Comment] [ntext] NULL, ";
                    cmdString += "[Status] [int] NOT NULL, [Telephone] [nvarchar](50) NULL, [PersonCount] [nvarchar](20) NULL, [TotalArea] [nvarchar](20) NULL, [PostalNumber] [nvarchar](30) NULL, ";
                    cmdString += "[FamilyNumber] [nvarchar](30) NULL, [TotalActualArea] [nvarchar](20) NULL, [TotalAwareArea] [nvarchar](20) NULL, [TotalModoArea] [nvarchar](20) NULL, ";
                    cmdString += "[TotalTableArea] [nvarchar](20) NULL, [OtherInfomation] nText, [ExtendA] [nvarchar](50) NULL, [ExtendB] [nvarchar](50) NULL,[ExtendC] [nvarchar](50) NULL)";
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        /// <summary>
        /// 添加二轮承包方表
        /// </summary>
        /// <param name="connectionString"></param>
        private static bool AddSqlServerSecondTableLand(string connectionString)
        {
            bool success = false;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "SecondTableLand")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE SecondTableLand ([ID] [uniqueidentifier] NOT NULL, [CadastralNumber] [nvarchar](50) NOT NULL, [Name] [nvarchar](50) NULL, ";
                    cmdString += "[LandNumber] [nvarchar](50) NOT NULL, [HouseHolderName] [nvarchar](50) NOT NULL, [HouseHolderId] [uniqueidentifier] NULL, [LandCode] [nvarchar](4) NOT NULL, [ContractType] [int] NOT NULL, ";
                    cmdString += "[LandLevel] [int] NOT NULL, [PlantType] [int] NOT NULL, [LandScopeLevel] [int] NOT NULL, [LandNeighbor] [ntext] NULL, ";
                    cmdString += "[OwnRightType] [int] NOT NULL, [OwnRightCode] [nvarchar](50) NOT NULL, [OwnRightName] [nvarchar](150) NOT NULL, [ZoneCode] [nvarchar](50) NOT NULL, [ZoneName] [nvarchar](150) NOT NULL, ";
                    cmdString += "[LineArea] [decimal](20, 8) NULL, [TableArea] [decimal](20, 8) NULL, [ActualArea] [decimal](20, 8) NOT NULL, [AwareArea] [decimal](20, 8) NOT NULL, ";
                    cmdString += "[IsFarmerLand] [bit] NULL, [Purpose] [int] NOT NULL, [ManagementType] [int] NOT NULL, [Soiltype] [nvarchar](50) NULL, ";
                    cmdString += "[IsFlyLand] [bit] NOT NULL, [ConcordId] [uniqueidentifier] NULL, [Founder] [nvarchar](50) NULL, [CreationTime] [datetime] NULL, ";
                    cmdString += "[Modifier] [nvarchar](50) NULL, [ModifiedTime] [datetime] NULL, [Comment] [ntext] NULL, [Status] [int] NOT NULL, ";
                    cmdString += "[FormerPerson] [nvarchar](50) NULL, [PlotNumber] [nvarchar](10) NULL, [MotorizeLandArea] [decimal](20, 8) NULL, [IsTransfer] [bit] NOT NULL, ";
                    cmdString += "[TransferType] [int] NULL, [TransferTime] [nvarchar](250) NULL, [TransferWhere] [nvarchar](150) NULL, [ExtendA] [nvarchar](max) NULL, ";
                    cmdString += "[ExtendB] [nvarchar](max) NULL, [ExtendC] [nvarchar](max) NULL, [WidthHeight] [nvarchar](50) NOT NULL, [ArableLandTime] nText NULL, ";
                    cmdString += "[PertainToArea] [decimal](20, 8) NOT NULL, [CadastralZoneCode] [nvarchar](50) NULL, [LandName] [nvarchar](50) NULL, [PlatType] [int] NOT NULL, ";
                    cmdString += "[ConstructMode] [int] NULL, [AliasNameA] Text NULL, [AliasNameB] Text NULL, [AliasNameC] Text NULL, ";
                    cmdString += "[AliasNameD] Text NULL, [AliasNameE] Text NULL, [AliasNameF] Text NULL)";
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        /// <summary>
        /// 添加业务表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static bool AddSqlServerAgricultureAffair(string connectionString)
        {
            bool success = false;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                bool exist = false;
                try
                {
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray != null && row.ItemArray.Length > 2
                            && row.ItemArray[2] != null && row.ItemArray[2].ToString() == "AgricultureAffair")
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist)
                    {
                        return true;
                    }
                    string cmdString = "CREATE TABLE AgricultureAffair ([ID] [uniqueidentifier] NOT NULL, [Name] [nvarchar](50) NULL, [Category] [int] NOT NULL, [OwnershipCategory] [tinyint] NOT NULL, ";
                    cmdString += "[Description] [nvarchar](500) NULL, [WorkAgrument] [xml] NULL, [Status] [int] NOT NULL, [SenderCode]  [nvarchar](19) NOT NULL,[SenderName]  [nvarchar](50) NOT NULL, [Creater] [nvarchar](50) NULL, ";
                    cmdString += "[CreaterTime] [datetime] NULL, [Modifyer] [nvarchar](50) NULL, [ModifyerTime] [datetime] NULL, [Comment] [nvarchar](500) NULL)";
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return success;
        }

        /// <summary>
        /// 添加表字段
        /// </summary>
        /// <returns></returns>
        private static void AddSqlServerTableField(string connectionString)
        {
            try
            {
                //添加承包方表字段
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    System.Data.DataTable table = connection.GetSchema("Tables");
                    foreach (System.Data.DataRow row in table.Rows)
                    {
                        if (row.ItemArray == null || row.ItemArray.Length <= 2
                            || row.ItemArray[2] == null)
                        {
                            continue;
                        }
                        switch (row.ItemArray[2].ToString())
                        {
                            case "LandVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "YardVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "TableVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "WoodVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "HouseVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "CollectiveLandVirtualPerson":
                                AddSqlServerVirtualPersonField(connection, row);
                                break;
                            case "ContractConcord":
                                AddSqlServerContractConcordField(connection, row);
                                break;
                            case "ContractLand":
                                AddSqlServerContractLandField(connection, row);
                                break;
                            case "Person":
                                AddSqlServerPersonField(connection, row);
                                break;
                            case "CollectivityTissue":
                                AddSqlServerCollectivityTissueField(connection, row);
                                break;
                            case "BuildLandBoundaryAddressDot":
                                AddSqlServerDotTableField(connection, row);
                                break;
                            case "BuildLandBoundaryAddressCoil":
                                AddSqlServerLineTableField(connection, row);
                                break;
                            case "ContractRegeditBook":
                                AddSqlServerContractRegeditBookField(connection, row);
                                break;
                            case "ContractRequireTable":
                                AddSqlServerContractRequireBookField(connection, row);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerVirtualPersonField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "Telephone");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [Telephone] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PersonCount");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PersonCount] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "TotalArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [TotalArea] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PostalNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PostalNumber] [nvarchar](30) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "FamilyNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [FamilyNumber] [nvarchar](30) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "TotalActualArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [TotalActualArea] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "TotalAwareArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [TotalAwareArea] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "TotalModoArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [TotalModoArea] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "TotalTableArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [TotalTableArea] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "OtherInfomation");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [OtherInfomation] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendA] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendB] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendC] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "CredentialsType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [CredentialsType] int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerContractLandField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "LandName");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LandName] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PlatType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PlatType] int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ConstructMode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ConstructMode] int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ArableLandTime");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ArableLandTime] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "CadastralZoneCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [CadastralZoneCode] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PertainToArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PertainToArea] [decimal](20, 8) NOT NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "WidthHeight");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [WidthHeight] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameA] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameB] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameC] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameD");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameD] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameE");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameE] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "AliasNameF");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [AliasNameF] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerContractConcordField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendA] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendB] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendC] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PersonAvgArea");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PersonAvgArea] [decimal](20, 8) NOT NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SecondContracterLocated");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SecondContracterLocated] [nvarchar](150) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SecondContracterName");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SecondContracterName] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityChronicle");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityChronicle] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityChroniclePerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityChroniclePerson] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityDate] DateTime NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityResult");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityResult] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityContractor");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityContractor] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityResultDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityResultDate] DateTime NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityCheckOpinion");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityCheckOpinion] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityCheckPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityCheckPerson] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PublicityCheckDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PublicityCheckDate] DateTime NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加承包方字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerPersonField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "IsFarmer");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [IsFarmer] Int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtensionPackageNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtensionPackageNumber] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "IsDeaded");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [IsDeaded] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LocalMarriedRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LocalMarriedRetreatLand] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "PeasantsRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [PeasantsRetreatLand] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ForeignMarriedRetreatLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ForeignMarriedRetreatLand] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SharePerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SharePerson] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "IsSharedLand");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [IsSharedLand] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendA");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendA] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendB");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendB] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendC");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendC] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendD");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendD] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendE");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendE] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ExtendF");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ExtendF] [nvarchar](100) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加集体经济组织字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerCollectivityTissueField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "LawyerTelephone");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LawyerTelephone] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LawyerAddress");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LawyerAddress] [nvarchar](150) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LawyerPosterNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LawyerPosterNumber] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LawyerCredentType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LawyerCredentType] Int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LawyerCartNumber");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LawyerCartNumber] [nvarchar](30) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SurveyPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SurveyPerson] [nvarchar](20) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SurveyDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SurveyDate] DateTime NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SurveyChronicle");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SurveyChronicle] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ChenkPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ChenkPerson] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ChenkDate");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ChenkDate] DateTime NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "ChenkOpinion");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [ChenkOpinion] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址点表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerDotTableField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "Description");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [Description] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SenderCode] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LandType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LandType] Int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址线表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerLineTableField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "Description");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [Description] nText NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SenderCode] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "NeighborPerson");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [NeighborPerson] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "NeighborFefer");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [NeighborFefer] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
            dataReader = readCommand.ExecuteReader();
            isExist = InitalizeSqlServerFieldInformation(dataReader, "LandType");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [LandType] Int NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加界址线表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerContractRegeditBookField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SenderCode] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 添加申请表字段
        /// </summary>
        /// <param name="column"></param>
        private static void AddSqlServerContractRequireBookField(SqlConnection connection, System.Data.DataRow row)
        {
            string tableName = row.ItemArray[2].ToString();
            if (string.IsNullOrEmpty(tableName))
            {
                return;
            }
            string cmdString = "select Top 1 * from " + tableName;
            SqlCommand readCommand = new SqlCommand(cmdString, connection);
            SqlDataReader dataReader = readCommand.ExecuteReader();
            SqlCommand cmd = null;
            bool isExist = InitalizeSqlServerFieldInformation(dataReader, "SenderCode");
            if (!isExist)
            {
                cmdString = "ALTER TABLE " + tableName + " ADD [SenderCode] [nvarchar](50) NULL";
                cmd = new SqlCommand(cmdString, connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 判断字段信息
        /// </summary>
        /// <returns></returns>
        private static bool InitalizeSqlServerFieldInformation(SqlDataReader dataReader, string fieldName)
        {
            bool exist = false;
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                if (dataReader.GetName(index) == fieldName)
                {
                    exist = true;
                    break;
                }
            }
            if (!dataReader.IsClosed)
            {
                dataReader.Close();
            }
            return exist;
        }

        #endregion

    }
}
