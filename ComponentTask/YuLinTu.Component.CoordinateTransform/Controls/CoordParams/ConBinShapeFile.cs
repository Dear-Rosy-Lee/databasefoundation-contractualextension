using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShapeProcess
{
    /// <summary>
    /// 合并文件
    /// </summary>
    public class ConBinShapeFile
    {
        public delegate void ReportInfo(string msg);
        public delegate void ProcessDelegate(int p, string msg);

        public ReportInfo InfoDelegate { get; set; }
        public ProcessDelegate Process { get; set; }

        public string extName;

        public ConBinShapeFile()
        {
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="list">待合并文件路径</param>
        /// <param name="tableName">文件名称</param>
        /// <param name="destinPath">导出路径</param> 
        public bool ConbinFile(List<string> list, string destinPath)
        {
            var tableName = Path.GetFileNameWithoutExtension(list[0]);
            bool result = AddConbin(list, destinPath);
            return result;
        }

        /// <summary>
        /// 增加字段的合并
        /// </summary>
        private bool AddConbin(List<string> list, string destinPath)
        {
            bool result = true;
            var errorinfo = "";
            int errCount = 0;
            try
            {
                //Process(1, string.Format("正在预处理{0}矢量数据", tableName));
                var shpPath = destinPath;
                var tableName = Path.GetFileNameWithoutExtension(shpPath);

                var flist = Directory.GetFiles(destinPath);
                for (int i = 0; i < flist.Length; i++)
                {
                    var name = Path.GetFileNameWithoutExtension(flist[i]);
                    if (name == tableName)
                        File.Delete(flist[i]);
                }
                Layer newLayer = null;
                string strDriverName = "ESRI Shapefile";
                OSGeo.OGR.Driver driver = Ogr.GetDriverByName(strDriverName);
                OSGeo.OGR.DataSource newsource = null;
                OSGeo.OGR.DataSource oldsource = null;
                if (driver == null)
                {
                    return false;
                }

                oldsource = driver.Open(list[0], 1);
                if (oldsource == null)
                    return false;
                var name0 = Path.GetFileNameWithoutExtension(list[0]);
                var layer0 = oldsource.GetLayerByName(name0);
                var dir = Path.GetDirectoryName(shpPath);
                newsource = driver.CreateDataSource(shpPath, null); // 创建数据源
                if (newsource == null)
                {
                    return false;
                }
                newLayer = newsource.CreateLayer(shpPath, layer0.GetSpatialRef(), layer0.GetGeomType(), null);

                var oDefn = layer0.GetLayerDefn();
                int iFieldCount = oDefn.GetFieldCount();

                for (int iField = 0; iField < iFieldCount; iField++)
                {
                    var oFieldDefn = oDefn.GetFieldDefn(iField);
                    newLayer.CreateField(oFieldDefn, 1);
                }
                var index = newLayer.FindFieldIndex("TCMCBH", 1);// oDefn.GetGeomFieldIndex("TCMCBH");
                if (index < 0)
                {
                    var oField = new FieldDefn("TCMCBH", FieldType.OFTString);
                    oField.SetWidth(3);
                    newLayer.CreateField(oField, 1);
                }
                layer0.Dispose();
                oldsource.Dispose();
                foreach (var item in list)
                {
                    var name = Path.GetFileNameWithoutExtension(item);
                    var pna = tableName + extName + "-";
                    var nameindex = name.Replace(pna, "");

                    var dsource = driver.Open(item, 1); // 创建数据源
                    if (dsource == null)
                    {
                        return false;
                    }
                    var flayer = dsource.GetLayerByName(name);
                    if (flayer == null)
                    {
                        return false;
                    }
                    var count = flayer.GetFeatureCount(1);
                    errorinfo = Path.GetFileName(item);
                    var fieldDefen = newLayer.GetLayerDefn();
                    for (int i = 0; i < count; i++)
                    {
                        errCount = i;
                        var feature = flayer.GetFeature(i);
                        var newFeature = new Feature(fieldDefen);
                        newFeature.SetFrom(feature, 1);
                        newFeature.SetField("TCMCBH", nameindex);
                        newLayer.CreateFeature(newFeature);
                    }
                    flayer.Dispose();
                    dsource.Dispose();
                }
                newLayer.Dispose();
                newsource.Dispose();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 检查数据属性
        /// </summary>
        /// <returns></returns>
        private bool CheckDataProperties(List<string> list)
        {
            bool result = true;
            var path = list[0];
            DbaseFieldDescriptor[] dfds = null;
            var dic = new Dictionary<string, DbaseFieldDescriptor>();
            try
            {
                using (var sdr = new ShapefileDataReader(path, GeometryFactory.Default))
                {
                    dfds = sdr.DbaseHeader.Fields;
                    for (int i = 0; i < dfds.Length; i++)
                    {
                        dic.Add(dfds[i].Name, dfds[i]);
                    }
                    for (int i = 1; i < list.Count; i++)
                    {
                        var item = list[i];
                        using (var reder = new ShapefileDataReader(item, GeometryFactory.Default))
                        {
                            var fds = reder.DbaseHeader.Fields;
                            for (int j = 0; j < fds.Length; j++)
                            {
                                var field = fds[j];
                                if (dic.ContainsKey(field.Name))
                                {
                                    var dicField = dic[field.Name];
                                    if (field.Length != dicField.Length || field.Type != dicField.Type || field.DecimalCount != dicField.DecimalCount)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
    }
}
