/*
 * (C) 2014-2017 鱼鳞图公司版权所有，保留所有权利
*/
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using Quality.Business.TaskBasic.GDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using YuLinTu;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 空间数据处理
    /// </summary>
    public class ArcSpaceDataProgress : Task
    {
        #region Fields

        #endregion

        #region Propertys

        /// <summary>
        /// 保存路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 矢量保存路径
        /// </summary>
        public string ShapeFilePath { get; set; }

        /// <summary>
        /// 坐标系
        /// </summary>
        public string SpatialText { get; set; }

        /// <summary>
        /// 控制点集合
        /// </summary>
        public List<KZD> KZDS { get; set; }

        /// <summary>
        /// 县级行政区集合
        /// </summary>
        public List<XJXZQ> XAJXZQS { get; set; }

        /// <summary>
        /// 乡级行政区集合
        /// </summary>
        public List<XJQY> XGJXZQS { get; set; }

        /// <summary>
        /// 村级行政区集合
        /// </summary>
        public List<CJQY> CJXZQS { get; set; }

        /// <summary>
        /// 组级行政区集合
        /// </summary>
        public List<ZJQY> ZJXZQS { get; set; }

        /// <summary>
        /// 区域界线
        /// </summary>
        public List<QYJX> QYJXS { get; set; }

        /// <summary>
        /// 界址点集合
        /// </summary>
        public List<SqliteJZD> JZDS { get; set; }

        /// <summary>
        /// 界址线集合
        /// </summary>
        public List<SqliteJZX> JZXS { get; set; }

        /// <summary>
        /// 基本农田保护区集合
        /// </summary>
        public List<JBNTBHQ> JBNTBHQS { get; set; }

        /// <summary>
        /// 点状地物集合
        /// </summary>
        public List<DZDW> DZDWS { get; set; }

        /// <summary>
        /// 线状地物集合
        /// </summary>
        public List<XZDW> XZDWS { get; set; }

        /// <summary>
        /// 面状地物集合
        /// </summary>
        public List<MZDW> MZDWS { get; set; }

        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// 拓展名称
        /// </summary>
        public string ExtentName { get; set; }

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ExportFileEntity ExportFile { get; set; }

        #endregion

        #region Ctor

        public ArcSpaceDataProgress()
        {
            InitiallCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出农业部2016最新元数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataInfo">内容</param>
        public bool ExportMeataData(string filePath, string name, VictorData dataInfo)
        {
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            if (!ExportFile.VictorMeata.IsExport)
                return true;
            if (!Directory.Exists(filePath))
            {
                return false;
            }
            try
            {
                string exportfileName = Path.Combine(filePath, string.Format("{0}.xml", name));
                if (File.Exists(exportfileName))
                {
                    File.Delete(exportfileName);
                }
                if (!string.IsNullOrEmpty(SpatialText))
                {
                    SetMetadataInfo(dataInfo, SpatialText);
                }
                string tempFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\MeataTemplate.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(tempFileName);    //加载Xml文件   
                XmlElement rootElem = doc.DocumentElement;   //获取根节点
                FixNodeValue(rootElem, "date", DateTime.Now.ToString("yyyyMMdd"));
                FixNodeValue(rootElem, "title", dataInfo.Data_Info.Title);
                FixNodeValue(rootElem, "geoID", dataInfo.Data_Info.GeoID);
                FixNodeValue(rootElem, "dataEdition", dataInfo.Data_Info.DataEdition);
                FixNodeValue(rootElem, "ending", dataInfo.Data_Info.Ending);
                FixNodeValue(rootElem, "rpOrgName", dataInfo.Data_Info.RpOrgName);
                FixNodeValue(rootElem, "rpCnt", dataInfo.Data_Info.RpCnt);
                FixNodeValue(rootElem, "idAbs", dataInfo.Data_Info.IdAbs);
                FixNodeValue(rootElem, "voiceNum", dataInfo.Data_Info.VoiceNum);
                FixNodeValue(rootElem, "faxNum", dataInfo.Data_Info.FaxNum);
                FixNodeValue(rootElem, "cntAddress", dataInfo.Data_Info.CntAddress);
                FixNodeValue(rootElem, "cntCode", dataInfo.Data_Info.CntCode);
                FixNodeValue(rootElem, "cntEmail", dataInfo.Data_Info.CntEmail);

                FixNodeValue(rootElem, "coorRSID", dataInfo.Sys_Info.CoorRSID);
                FixNodeValue(rootElem, "centralMer", dataInfo.Sys_Info.CentralMer);
                FixNodeValue(rootElem, "eastFAL", dataInfo.Sys_Info.EastFAL);
                FixNodeValue(rootElem, "northFAL", dataInfo.Sys_Info.NorthFAL);
                FixNodeValue(rootElem, "coorFDKD", dataInfo.Sys_Info.CoorFDKD);

                FixNodeValue(rootElem, "dqStatement", dataInfo.DQ_Info.DqStatement);
                FixNodeValue(rootElem, "dqLineage", dataInfo.DQ_Info.DqLineage);
                //doc.SelectSingleNode("Question/Question[@name=1]").Attributes["sound"].InnerXml = "2"; //将name=1记录下的sound值设为2
                doc.Save(exportfileName);
                return true;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 修改节点内容
        /// </summary>
        private void FixNodeValue(XmlElement rootElem, string nodename, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;
            XmlNodeList personNodes = rootElem.GetElementsByTagName(nodename); //获取person子节点集合  
            if (personNodes != null && personNodes.Count > 0)
            {
                foreach (XmlNode item in personNodes)
                {
                    item.InnerText = value;
                }
            }
        }

        /// <summary>
        /// 导出地域空间数据
        /// </summary>
        /// <returns></returns>
        public bool ExportSpaceZone()
        {
            int index = 0;
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            try
            {
                if (ExportFile.VictorXJXZQ.IsExport && XAJXZQS != null)
                {
                    List<XJXZQ> xjxzqList = GetXJXZQData(XAJXZQS);
                    xjxzqList.ForEach(t =>
                    {
                        t.BSM = QuantityValue.County + index;
                        index++;
                    });
                    ExportSpaceInfo<XJXZQ>(xjxzqList);
                }
                if (XGJXZQS != null && ExportFile.VictorXJQY.IsExport)
                {
                    index = 0;
                    List<XJQY> xjqyList = GetXJQYData(XGJXZQS);
                    xjqyList.ForEach(t =>
                    {
                        t.BSM = QuantityValue.Town + index;
                        index++;
                    });
                    ExportSpaceInfo<XJQY>(xjqyList);
                }
                if (CJXZQS != null && ExportFile.VictorCJQY.IsExport)
                {
                    index = 0;
                    List<CJQY> cjqyList = GetCJQYData(CJXZQS);
                    cjqyList.ForEach(t =>
                    {
                        t.BSM = QuantityValue.Village + index;
                        index++;
                    });
                    ExportSpaceInfo<CJQY>(cjqyList);
                }
                if (ZJXZQS != null && ExportFile.VictorZJQY.IsExport)
                {
                    index = 0;
                    List<ZJQY> zjqyList = GetZJQYData(ZJXZQS);
                    zjqyList.ForEach(t =>
                    {
                        t.BSM = QuantityValue.Group + index;
                        index++;
                    });
                    ExportSpaceInfo<ZJQY>(zjqyList);
                }
            }
            catch (Exception ex)
            {
                string errorInfo = "导出地域空间数据时发生错误,无法继续导出";
                this.ReportError(errorInfo);
                LogWrite.WriteErrorLog(errorInfo + ex.Message, ex);
            }
            Clear();
            return true;
        }

        /// <summary>
        /// 导出其它空间数据
        /// </summary>
        /// <returns></returns>
        public bool ExportSpaceData(ComplexSpaceEntity spaceEntity)
        {
            if (spaceEntity == null)
            {
                return false;
            }
            try
            {
                if (ExportFile.VictorKZD.IsExport)
                {
                    InitialControlData(spaceEntity.KZD, new FilePathManager().OtherName, "点之记");
                    ExportSpaceInfo<KZD>(spaceEntity.KZD);
                }
                if (ExportFile.VictorMZDW.IsExport)
                    ExportSpaceInfo<MZDW>(spaceEntity.MZDW);

                if (ExportFile.VictorQYJX.IsExport)
                    ExportSpaceInfo<QYJX>(spaceEntity.QYJX);

                if (ExportFile.VictorXZDW.IsExport)
                    ExportSpaceInfo<XZDW>(spaceEntity.XZDW);

                if (ExportFile.VictorDZDW.IsExport)
                    ExportSpaceInfo<DZDW>(spaceEntity.DZDW);

                if (ExportFile.VictorJBNTBHQ.IsExport)
                    ExportSpaceInfo<JBNTBHQ>(spaceEntity.JBNTBHQ);

                if (ExportFile.VictorZJ.IsExport)
                    ExportSpaceInfo<ZJ>(spaceEntity.ZJ);
            }
            catch (Exception ex)
            {
                string errorInfo = "导出地域空间数据时发生错误,无法继续导出";
                this.ReportError(errorInfo);
                LogWrite.WriteErrorLog(errorInfo + ex.Message, ex);
            }
            Clear();
            return true;
        }

        /// <summary>
        /// 初始化控制点数据
        /// </summary>
        private void InitialControlData(List<KZD> kzdList, string otherPath, string folderName)
        {
            if (kzdList == null || kzdList.Count == 0)
                return;
            foreach (var kzd in kzdList)
            {
                if (string.IsNullOrEmpty(kzd.DZJ))
                    continue;
                string filename = kzd.DZJ.Substring(kzd.DZJ.LastIndexOf("\\") + 1);
                if (string.IsNullOrEmpty(filename))
                    continue;
                kzd.DZJ = Path.Combine(otherPath, folderName, filename);
            }
        }

        /// <summary>
        /// 导出Sqlite数据库中的矢量数据
        /// </summary>
        public void ExportSpaceDataBase(SqliteManager manager, string zoneCode, bool newStand = false)
        {
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            if (manager == null)
            {
                return;
            }
            string tempPath = Path.GetTempPath();
            if (ExportFile.VictorDK.IsExport)
            {
                var fieldList = new ShapeFileInfo().DKZD;
                ProcessExportShape<SqliteDK>("导出地块时发生错误", DK.TableName, tempPath,
                    OSGeo.OGR.wkbGeometryType.wkbPolygon, fieldList, manager, QuantityValue.Land);
                var shapName = JZD.TableName + ExtentName + ".shp";
                string pointPath = Path.Combine(ShapeFilePath, shapName);

                InitalzeGeometryCoordinate(pointPath);

                shapName = JZX.TableName + ExtentName + ".shp";
                var linePath = Path.Combine(ShapeFilePath, shapName);

                InitalzeGeometryCoordinate(linePath);
            }
        }

        /// <summary>
        /// 导出shape文件
        /// </summary>
        public void ProcessExportShape<T>(string errorInfo, string name, string tempPath, OSGeo.OGR.wkbGeometryType geoType, List<FieldInformation> fieldList,
            SqliteManager manager, int startIndex, Func<string, IEnumerable<T>> fucNum = null)
        {
            try
            {
                ProcessExport<T>(name, tempPath, geoType, fieldList, manager, startIndex, fucNum);
            }
            catch (Exception ex)
            {
                this.ReportError(errorInfo + "，详情请查看日志");
                LogWrite.WriteErrorLog(errorInfo + ex.Message, ex);
            }
        }


        /// <summary>
        /// 处理文件导出
        /// </summary>
        public void ProcessExport<T>(string layerName, string tempPath, OSGeo.OGR.wkbGeometryType geoType, List<FieldInformation> fieldList,
             SqliteManager manager, int startIndex, Func<string, IEnumerable<T>> fucNum = null)
        {
            var shapName = layerName + ExtentName + ".shp";
            string newfileName = Path.Combine(tempPath, shapName);
            if (DelteShapeFile(newfileName))
                newfileName = Path.Combine(tempPath, layerName + ExtentName + "1.shp");
            GDALShapeFileWriter<T> writer = new GDALShapeFileWriter<T>(OSGeo.OGR.wkbGeometryType.wkbPoint, tempPath);
            var layer = writer.InitiallLayer(newfileName, layerName, geoType, fieldList);
            string fileName = Path.Combine(ShapeFilePath, shapName);
            manager.GetDataToMethod<T>(new Action<List<T>>((list) =>
            {
                lock (ExportFile)
                {
                    BsmValueSet(list, ref startIndex);
                    writer.WriteVectorFile(layer, list);
                    list.Clear();
                }
            }));
            layer.Dispose();
            writer.Dispose();
            DelteShapeFile(fileName);
            CopyShapeFile(newfileName, fileName);
            DelteShapeFile(newfileName);
            InitalzeGeometryCoordinate(fileName);
        }

        /// <summary>
        /// 设置标识码值
        /// </summary>
        private void BsmValueSet<T>(List<T> list, ref int startIndex)
        {
            bool lineType = false;
            if (typeof(T).Name == typeof(SqliteJZX).Name)
                lineType = true;
            foreach (var item in list)
            {
                startIndex++;
                ObjectExtension.SetPropertyValue(item, "BSM", startIndex);
                if (lineType)
                {
                    SqliteJZX t = item as SqliteJZX;
                    if (t.JZXWZ != null && t.JZXWZ.Length == 2)
                    {
                        t.JZXWZ = t.JZXWZ.Substring(1);
                    }
                }
            }
        }

        /// <summary>
        /// 删除矢量文件
        /// </summary>
        private bool DelteShapeFile(string shpPath)
        {
            try
            {
                if (File.Exists(shpPath))
                    File.Delete(shpPath);
                var dbfPath = Path.ChangeExtension(shpPath, ".dbf");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".shx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbn");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".shp.xml");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("删除临时文件:" + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 复制矢量文件
        /// </summary>
        private bool CopyShapeFile(string shpPath, string destPath)
        {
            try
            {
                if (File.Exists(shpPath))
                    File.Copy(shpPath, destPath, true);
                var dbfPath = Path.ChangeExtension(shpPath, ".dbf");
                if (File.Exists(dbfPath))
                    File.Copy(dbfPath, Path.ChangeExtension(destPath, ".dbf"), true);
                dbfPath = Path.ChangeExtension(shpPath, ".shx");
                if (File.Exists(dbfPath))
                    File.Copy(dbfPath, Path.ChangeExtension(destPath, ".shx"), true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 导出空间数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableNameCN">中文名</param>
        /// <param name="xzqy">是否行政区域</param>
        /// <returns></returns>
        public bool ExportSpaceInfo<T>(List<T> list) where T : class
        {

            string tableName = string.Empty;
            string tableNameCN = string.Empty;
            bool result = true;
            try
            {
                FieldInfo[] fieldList = typeof(T).GetFields();
                for (int i = 0; i < fieldList.Length; i++)
                {
                    FieldInfo info = fieldList[i];
                    if (info.Name == "TableName")
                    {
                        tableName = info.GetValue(null).ToString();
                    }
                    if (info.Name == "TableNameCN")
                    {
                        tableNameCN = info.GetValue(null).ToString();
                    }
                }
                string fileName = Path.Combine(ShapeFilePath, tableName + ExtentName);
                List<IFeature> featureCollection = new List<IFeature>();
                if (list != null && list.Count > 0)
                {
                    foreach (var en in list)
                    {
                        IFeature feautre = InitalizeFeature<T>(en);
                        if (feautre != null)
                        {
                            featureCollection.Add(feautre);
                        }
                    }
                }
                IList<IFeature> featureList = featureCollection as IList<IFeature>;
                if (featureList.Count > 0)
                {
                    ShapefileDataWriter dataWriter = new ShapefileDataWriter(fileName, GeometryFactory.Default, Encoding.Default);
                    dataWriter.Header = InitalizeHeader(tableName);
                    dataWriter.Header.NumRecords = featureCollection.Count;
                    dataWriter.Write(featureList);
                    dataWriter = null;
                    this.ReportInfomation("成功导出" + tableNameCN + "数据" + featureCollection.Count + "条");
                }
                else
                {
                    result = false;
                }
                InitalzeGeometryCoordinate(fileName);
                featureCollection.Clear();
            }
            catch (SystemException ex)
            {
                LogWrite.WriteErrorLog("导出" + tableNameCN + "发生错误" + ex.Message + ex.StackTrace);
                this.ReportError("导出" + tableNameCN + "发生错误");
                result = false;
            }
            finally
            {
                list = null;
            }
            return result;
        }

        /// <summary>
        /// 初始化表头
        /// </summary>
        /// <returns></returns>
        private DbaseFileHeader InitalizeHeader(string name)
        {
            var propInfo = typeof(ShapeFileInfo).GetProperties();
            var pi = propInfo.First(t => t.Name == (name + "ZD"));
            if (pi == null)
            {
                return null;
            }
            var si = new ShapeFileInfo();
            var header = new DbaseFileHeader();
            var columns = pi.GetValue(si, null) as List<FieldInformation>;
            foreach (var item in columns)
            {
                var typechar = 'C';
                if (item.CharType == "Int")
                    typechar = 'N';
                else if (item.CharType == "Double" || item.CharType == "Float")
                    typechar = 'F';
                header.AddColumn(item.FieldName, typechar, item.FieldLength, item.FieldPrecision);
            }
            return header;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="record">实体</param>
        /// <param name="index">序号</param>
        /// <param name="geometry">图形</param>
        /// <param name="xzqy">是否是行政区域</param>
        /// <returns></returns>
        private AttributesTable InitalizeReconrd<T>(T record, ref object geometry) where T : class
        {
            PropertyInfo[] infoList = typeof(T).GetProperties();
            AttributesTable attributes = new AttributesTable();
            for (int i = 0; i < infoList.Length; i++)
            {
                PropertyInfo info = infoList[i];
                if (info.Name == "Shape")
                {
                    geometry = info.GetValue(record, null);
                    continue;
                }
                attributes.AddAttribute(info.Name, info.GetValue(record, null));
            }
            return attributes;
        }

        /// <summary>
        /// 初始化记录
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="record">实体</param>
        /// <param name="index">序号</param>
        /// <param name="geometry">图形</param>
        /// <param name="xzqy">是否是行政区域</param>
        /// <returns></returns>
        private Feature InitalizeFeature<T>(T record) where T : class
        {
            object shape = null;
            AttributesTable attributes = InitalizeReconrd<T>(record, ref shape);
            if (shape == null)
            {
                return null;
            }
            IGeometry gometry = null;
            if (shape is IGeometry)
            {
                gometry = shape as IGeometry;
            }
            if (shape is YuLinTu.Spatial.Geometry)
            {
                gometry = (shape as YuLinTu.Spatial.Geometry).Instance;
            }
            if (gometry == null)
            {
                return null;
            }
            return new Feature(gometry, attributes);
        }

        /// <summary>
        /// 初始化集合数据
        /// </summary>
        public void InitiallCollection()
        {
            KZDS = new List<KZD>();
            XAJXZQS = new List<XJXZQ>();
            XGJXZQS = new List<XJQY>();
            CJXZQS = new List<CJQY>();
            ZJXZQS = new List<ZJQY>();
            JZXS = new List<SqliteJZX>();
            JZDS = new List<SqliteJZD>();
            QYJXS = new List<QYJX>();
            JBNTBHQS = new List<JBNTBHQ>();
            DZDWS = new List<DZDW>();
            XZDWS = new List<XZDW>();
            MZDWS = new List<MZDW>();
        }

        /// <summary>
        /// 初始化坐标系
        /// </summary>
        public void InitalzeGeometryCoordinate(string fileName)
        {
            if (string.IsNullOrEmpty(SpatialText))
            {
                return;
            }
            string prjFile = fileName;
            if (Path.GetExtension(fileName) == ".shp")
            {
                prjFile = Path.ChangeExtension(fileName, ".prj");
            }
            else
            {
                prjFile = fileName + ".prj";
            }
            System.IO.File.WriteAllText(prjFile, SpatialText);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        private void Clear()
        {
            if (KZDS != null)
                KZDS.Clear();
            if (XAJXZQS != null)
                XAJXZQS.Clear();
            if (XGJXZQS != null)
                XGJXZQS.Clear();
            if (CJXZQS != null)
                CJXZQS.Clear();
            if (ZJXZQS != null)
                ZJXZQS.Clear();
            if (JZXS != null)
                JZXS.Clear();
            if (JZDS != null)
                JZDS.Clear();
            if (QYJXS != null)
                QYJXS.Clear();
            if (JBNTBHQS != null)
                JBNTBHQS.Clear();
            if (DZDWS != null)
                DZDWS.Clear();
            if (XZDWS != null)
                XZDWS.Clear();
            if (MZDWS != null)
                MZDWS.Clear();
        }

        /// <summary>
        /// 元数据内容
        /// </summary>
        private void SetMetadataInfo(VictorData dataInfo, string infoString)
        {
            if (string.IsNullOrEmpty(infoString) || dataInfo == null)
            {
                return;
            }
            if (infoString.Contains("Xian_1980"))
            {
                dataInfo.Sys_Info.CoorRSID = "002";
            }
            else
            {
                dataInfo.Sys_Info.CoorRSID = "001";
            }
            int index = infoString.LastIndexOf("PARAMETER[\"False_Easting");
            if (index > 0)
            {
                string estString = infoString.Substring(index + 26);
                index = estString.IndexOf("]");
                dataInfo.Sys_Info.EastFAL = index > 0 ? estString.Substring(0, index) : "";
            }
            index = infoString.LastIndexOf("PARAMETER[\"False_Northing");
            if (index > 0)
            {
                string estString = infoString.Substring(index + 27);
                index = estString.IndexOf("]");
                dataInfo.Sys_Info.NorthFAL = index > 0 ? estString.Substring(0, index) : "";
            }
            index = infoString.LastIndexOf("PARAMETER[\"Central_Meridian");
            if (index > 0)
            {
                string estString = infoString.Substring(index + 29);
                index = estString.IndexOf("]");
                dataInfo.Sys_Info.CentralMer = index > 0 ? estString.Substring(0, index) : "";
            }
        }

        #region Shape文件数据读取

        /// <summary>
        /// 获取组级区域数据
        /// </summary>
        /// <returns></returns>
        private List<ZJQY> GetZJQYData(List<ZJQY> dataList)
        {
            string fileName = Path.Combine(ShapeFilePath, "ZJQY" + ExtentName + ".shp");
            if (!File.Exists(fileName) || ExportFile.IsAllExport)
            {
                return dataList;
            }
            List<ZJQY> list = new List<ZJQY>();
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                while (dataReader.Read())
                {
                    ZJQY en = new ZJQY();
                    en.BSM = GetFiledIntValue(dataReader, ZJQY.CBSM);
                    en.ZJQYMC = GetFiledValue(dataReader, ZJQY.CZJQYMC, true, false);
                    en.YSDM = GetFiledValue(dataReader, ZJQY.CYSDM, true, false);
                    en.ZJQYDM = GetFiledValue(dataReader, ZJQY.CZJQYDM, true, false);
                    en.Shape = YuLinTu.Spatial.Geometry.FromInstance(dataReader.Geometry);
                    list.Add(en);
                }
                dataList.ForEach(t =>
                {
                    list.RemoveAll(l => l.ZJQYDM == t.ZJQYDM);
                });
                list.ForEach(t => dataList.Add(t));
            }
            return dataList;
        }

        /// <summary>
        /// 获取村级区域数据
        /// </summary>
        private List<CJQY> GetCJQYData(List<CJQY> dataList)
        {
            string fileName = Path.Combine(ShapeFilePath, "CJQY" + ExtentName + ".shp");
            if (!File.Exists(fileName) || ExportFile.IsAllExport)
            {
                return dataList;
            }
            List<CJQY> list = new List<CJQY>();
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                while (dataReader.Read())
                {
                    CJQY en = new CJQY();
                    en.BSM = GetFiledIntValue(dataReader, CJQY.CBSM);
                    en.YSDM = GetFiledValue(dataReader, CJQY.CYSDM, true, false);
                    en.CJQYMC = GetFiledValue(dataReader, CJQY.CCJQYMC, true, false);
                    en.CJQYDM = GetFiledValue(dataReader, CJQY.CCJQYDM, true, false);
                    en.Shape = YuLinTu.Spatial.Geometry.FromInstance(dataReader.Geometry);
                    list.Add(en);
                }
            }
            dataList.ForEach(t =>
            {
                list.RemoveAll(l => l.CJQYDM == t.CJQYDM);
            });
            list.ForEach(t => dataList.Add(t));
            return dataList;
        }

        /// <summary>
        /// 获取乡级区域数据
        /// </summary>
        private List<XJQY> GetXJQYData(List<XJQY> dataList)
        {
            string fileName = Path.Combine(ShapeFilePath, "XJQY" + ExtentName + ".shp");
            if (!File.Exists(fileName) || ExportFile.IsAllExport)
            {
                return dataList;
            }
            List<XJQY> list = new List<XJQY>();
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                while (dataReader.Read())
                {
                    XJQY en = new XJQY();
                    en.BSM = GetFiledIntValue(dataReader, XJQY.CBSM);
                    en.YSDM = GetFiledValue(dataReader, XJQY.CYSDM, true, false);
                    en.XJQYMC = GetFiledValue(dataReader, XJQY.CXJQYMC, true, false);
                    en.XJQYDM = GetFiledValue(dataReader, XJQY.CXJQYDM, true, false);
                    en.Shape = YuLinTu.Spatial.Geometry.FromInstance(dataReader.Geometry);
                    list.Add(en);
                }
            }
            dataList.ForEach(t =>
            {
                list.RemoveAll(l => l.XJQYDM == t.XJQYDM);
            });
            list.ForEach(t => dataList.Add(t));
            return dataList;
        }

        /// <summary>
        /// 获取县级行政区数据
        /// </summary>
        private List<XJXZQ> GetXJXZQData(List<XJXZQ> dataList)
        {
            string fileName = Path.Combine(ShapeFilePath, "XJXZQ" + ExtentName + ".shp");
            if (!File.Exists(fileName) || ExportFile.IsAllExport)
            {
                return dataList;
            }
            List<XJXZQ> list = new List<XJXZQ>();
            using (ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                while (dataReader.Read())
                {
                    XJXZQ en = new XJXZQ();
                    en.BSM = GetFiledIntValue(dataReader, XJXZQ.CBSM);
                    en.YSDM = GetFiledValue(dataReader, XJXZQ.CYSDM, true, false);
                    en.XZQMC = GetFiledValue(dataReader, XJXZQ.CXZQMC, true, false);
                    en.XZQDM = GetFiledValue(dataReader, XJXZQ.CXZQDM, true, false);
                    en.Shape = YuLinTu.Spatial.Geometry.FromInstance(dataReader.Geometry);
                    list.Add(en);
                }
            }
            dataList.ForEach(t =>
            {
                list.RemoveAll(l => l.XZQDM == t.XZQDM);
            });
            list.ForEach(t => dataList.Add(t));
            return dataList;
        }

        /// <summary>
        /// 获取子段值
        /// </summary>
        private string GetFiledValue(ShapefileDataReader dataReader, string fieldName, bool isNecessary = true, bool isNumber = true)
        {
            int fieldIndex = ShapeFileInfo.GetFiledIndex(dataReader, fieldName.Trim());
            if (fieldIndex == -1)
            {
                return "";
            }
            object value = dataReader.GetValue(fieldIndex);
            if (value == null)
            {
                return "";
            }
            else
            {
                string returnVaule = value.ToString().Replace("\0", "");
                returnVaule = string.IsNullOrEmpty(returnVaule) ? (isNumber ? "0" : returnVaule) : returnVaule;
                return returnVaule.Trim();
            }
        }

        /// <summary>
        /// 获取子段值
        /// </summary>
        private int GetFiledIntValue(ShapefileDataReader dataReader, string fieldName, bool isNecessary = true)
        {
            int fieldIndex = ShapeFileInfo.GetFiledIndex(dataReader, fieldName.Trim());
            if (fieldIndex == -1)
            {
                return 0;
            }
            object value = dataReader.GetValue(fieldIndex);
            if (value == null)
            {
                return 0;
            }
            else
            {
                string returnVaule = value.ToString().Replace("\0", "");
                returnVaule = string.IsNullOrEmpty(returnVaule) ? "0" : returnVaule;
                int fvalue = 0;
                int.TryParse(returnVaule, out fvalue);
                return fvalue;
            }
        }

        #endregion

        #endregion
    }
}
