/*
 * (C)2016 公司版权所有,保留所有权利
*/
using OSGeo.OGR;
using Quality.Business.TaskBasic;
using Quality.Business.TaskBasic.GDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Spatial;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// GDAL Shape文件类
    /// </summary>
    public class ShapeFileWriter<T>
    {
        #region Fields

        private HashSet<KeyName> knSet;

        private DataSource oDS = null;
        private Driver oDriver = null;
        #endregion

        #region Property

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> FeatrueList { get; set; }

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<FieldInformation> FieldDefnList { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 矢量数据类型
        /// </summary>
        public wkbGeometryType GeometryType { get; set; }

        #endregion

        #region Ctor

        public ShapeFileWriter()
        {
        }

        public ShapeFileWriter(wkbGeometryType geoType)
        {
            GeometryType = geoType;
        }

        public ShapeFileWriter(wkbGeometryType geoType, string folderName)
        {
            FilePath = folderName;
            GeometryType = geoType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 注册Ogr驱动
        /// </summary>
        public static bool Registerdll()
        {
            try
            {
                Ogr.RegisterAll();
                // 为了支持中文路径，请添加下面这句代码 
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
                // 为了使属性表字段支持中文，请添加下面这句 
                OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "CP936");
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("注册Gdal驱动失败,无法进行相关的shape文件导出:" + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化新的图层
        /// </summary>
        /// <param name="fileName">shape文件的完整路径</param>
        /// <param name="layerName">图层名称</param>
        /// <param name="geoType">图层类型</param>
        public Layer InitiallLayer(string fileName, string layerName, wkbGeometryType geoType, List<FieldInformation> fieldList = null)
        {
            try
            {
                Layer pLayer = null;
                string strDriverName = "ESRI Shapefile"; //创建数据，这里以创建ESRI的shp文件为例
                oDriver = Ogr.GetDriverByName(strDriverName);
                if (oDriver == null)
                {
                    return pLayer;
                }
                if (File.Exists(fileName))
                {
                    oDS = oDriver.Open(fileName, 1);
                }
                else
                {
                    oDS = oDriver.CreateDataSource(fileName, null); // 创建数据源
                }
                if (oDS == null)
                {
                    return pLayer;
                }
                pLayer = oDS.CreateLayer(layerName, null, geoType, null);
                CreateFeildInfo(pLayer, fieldList);
                return pLayer;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 初始化新的图层
        /// </summary>
        /// <param name="fileName">shape文件的完整路径</param>
        public Layer OpenLayer(string fileName)
        {
            try
            {
                string layerName = Path.GetFileNameWithoutExtension(fileName);
                Layer pLayer = null;
                string strDriverName = "ESRI Shapefile"; //创建数据，这里以创建ESRI的shp文件为例
                using (var oDriver = Ogr.GetDriverByName(strDriverName))
                {
                    if (oDriver == null)
                    {
                        return pLayer;
                    }
                    if (File.Exists(fileName))
                    {
                        oDS = oDriver.Open(fileName, 1);
                    }
                    if (oDS == null)
                    {
                        return pLayer;
                    }
                    pLayer = oDS.GetLayerByName(layerName);
                }
                return pLayer;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 初始化新的图层
        /// </summary>
        /// <param name="fileName">shape文件的完整路径</param>
        public Layer OpenLayerNew(string fileName)
        {
            try
            {
                string layerName = Path.GetFileNameWithoutExtension(fileName);
                Layer pLayer = null;
                string strDriverName = "ESRI Shapefile"; //创建数据，这里以创建ESRI的shp文件为例
                using (var odriver = Ogr.GetDriverByName(strDriverName))
                {
                    if (odriver == null)
                    {
                        return pLayer;
                    }
                    if (File.Exists(fileName))
                    {
                        var ods = odriver.Open(fileName, 1);
                        if (ods != null)
                        {
                            pLayer = ods.GetLayerByName(layerName);
                        }
                    }
                }
                return pLayer;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 创建Shape文件
        /// </summary>
        /// <param name="layer">根据已有图层创建</param>
        /// <param name="dataSource">数据集合</param>
        /// <returns></returns>
        public string WriteVectorFile(Layer layer, List<T> dataSource)
        {
            if (knSet == null || knSet.Count() == 0)
                return "未建立字段字典";
            FeatureDefn oDefn = layer.GetLayerDefn();
            return WriteShapeLayer(dataSource, knSet, oDefn, layer);
            //if (!string.IsNullOrEmpty(err))
            //{
            //    LogWrite.WriteErrorLog(err);
            //    return false;
            //}
            //return true;
        }

        /// <summary>
        /// 创建Shape文件内容
        /// </summary>
        private string WriteShapeLayer(List<T> list, HashSet<KeyName> dic, FeatureDefn oDefn, Layer oLayer)
        {
            string error = string.Empty;
            try
            {
                foreach (var item in list)
                {
                    OSGeo.OGR.Feature featrueEn = new OSGeo.OGR.Feature(oDefn);
                    foreach (var ditem in dic)
                    {
                        var objValue = ObjectExtension.GetPropertyValue(item, ditem.Name);
                        switch (ditem.FiledType)
                        {
                            case "Int64":
                                featrueEn.SetField(ditem.Key, objValue == null ? 0 : Convert.ToInt64(objValue));
                                break;
                            case "Int32":
                            case "Int":
                                featrueEn.SetField(ditem.Key, objValue == null ? 0 : Convert.ToInt32(objValue));
                                break;
                            case "number":
                                featrueEn.SetField(ditem.Key, objValue == null ? 0 : Convert.ToInt32(objValue));
                                break;
                            case "Double":
                                featrueEn.SetField(ditem.Key, objValue == null ? 0 : Convert.ToDouble(objValue));
                                break;
                            case "String":
                                featrueEn.SetField(ditem.Key, objValue == null ? "" : objValue.ToString());
                                break;
                            case "DateTime":
                                if (objValue != null)
                                {
                                    var dt = (DateTime)objValue;
                                    featrueEn.SetField(ditem.Key, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 1);
                                }
                                break;
                            case "Guid":
                                featrueEn.SetField(ditem.Key, objValue == null ? "" : objValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    var geo = item.GetPropertyValue("Shape") as YuLinTu.Spatial.Geometry;
                    if (geo == null)
                        continue;
                    if (geo.GeometryType == eGeometryType.MultiPoint)
                    {
                        YuLinTu.Spatial.Geometry[] geoArray = geo.ToSingleGeometries();
                        var ogrWkb = OSGeo.OGR.Geometry.CreateFromWkb(geoArray[0].ToBytes());
                        featrueEn.SetGeometry(ogrWkb);
                        oLayer.CreateFeature(featrueEn);
                    }
                    else
                    {
                        var ogrWkb = OSGeo.OGR.Geometry.CreateFromWkb(geo.ToBytes());
                        featrueEn.SetGeometry(ogrWkb);
                        oLayer.CreateFeature(featrueEn);
                    }
                }
            }
            catch (Exception ex)
            {
                error += ex.Message + ex.StackTrace;
            }
            return error;
        }

        public void Dispose()
        {
            if (oDS != null)
            {
                oDS.Dispose();
            }
            if (oDriver != null)
            {
                oDriver.Dispose();
            }
        }

        /// <summary>
        /// 创建字段信息
        /// </summary>
        private void CreateFeildInfo(Layer oLayer, List<FieldInformation> fieldList)
        {
            knSet = new HashSet<KeyName>();
            if (fieldList == null)
            {
                FieldDefnList = new List<FieldInformation>();
                var infoList = typeof(T).GetProperties();
                for (int i = 0; i < infoList.Length; i++)
                {
                    var info = infoList[i];
                    string name = info.Name;
                    if (name == "Shape")
                        continue;
                    FieldDefn oFieldName = new FieldDefn(name, ChangeType(info.PropertyType.Name));
                    //oFieldName.SetWidth(6);
                    knSet.Add(new KeyName() { Name = name, FiledType = info.PropertyType.Name, Key = i });
                    oLayer.CreateField(oFieldName, 1);
                }
                return;
            }
            else
            {
                foreach (var item in fieldList)
                {
                    if (item.FieldName == "Shape")
                        continue;
                    FieldDefn oFieldName = new FieldDefn(item.FieldName, ChangeType(item.FieldType));
                    oFieldName.SetWidth(item.FieldLength);
                    knSet.Add(new KeyName() { Name = item.FieldName, FiledType = item.FieldType });
                    oLayer.CreateField(oFieldName, 1);
                }
                FeatureDefn oDefn = oLayer.GetLayerDefn();
                int iFieldCount = oDefn.GetFieldCount();
                for (int iField = 0; iField < iFieldCount; iField++)
                {
                    FieldDefn oFieldDefn = oDefn.GetFieldDefn(iField);
                    string filedName = oFieldDefn.GetName();
                    var d = knSet.FirstOrDefault(t => t.Name == filedName);
                    if (d != null)
                        d.Key = iField;
                }
            }
            //else
            //{
            //    FeatureDefn oDefn = oLayer.GetLayerDefn();
            //    int iFieldCount = oDefn.GetFieldCount();
            //    for (int iField = 0; iField < iFieldCount; iField++)
            //    {
            //        FieldDefn oFieldDefn = oDefn.GetFieldDefn(iField);
            //        string filedName = oFieldDefn.GetName();
            //        var type = oFieldDefn.GetTypeName();
            //        knSet.Add(new KeyName() { Name = filedName, FiledType = type, Key = iField });
            //    }
            //}
        }

        /// <summary>
        /// 转换字段类型
        /// </summary>
        private FieldType ChangeType(string typename)
        {
            FieldType ft = FieldType.OFTString;
            switch (typename)
            {
                case "number":
                    ft = FieldType.OFTInteger;
                    break;
                case "int":
                    ft = FieldType.OFTInteger;
                    break;
                case "double":
                    ft = FieldType.OFTReal;
                    break;
                case "string":
                    ft = FieldType.OFTString;
                    break;
                case "date":
                    ft = FieldType.OFTTime;
                    break;
                case "dateTime":
                    ft = FieldType.OFTDateTime;
                    break;
                case "object":
                    ft = FieldType.OFTBinary;
                    break;
                case "float":
                    ft = FieldType.OFTReal;
                    break;

                case "Number":
                    ft = FieldType.OFTInteger;
                    break;
                case "Int":
                    ft = FieldType.OFTInteger;
                    break;
                case "Int32":
                    ft = FieldType.OFTInteger;
                    break;
                case "Int64":
                    ft = FieldType.OFTInteger;
                    break;
                case "Double":
                    ft = FieldType.OFTReal;
                    break;
                case "String":
                    ft = FieldType.OFTString;
                    break;
                case "Date":
                    ft = FieldType.OFTTime;
                    break;
                case "DateTime":
                    ft = FieldType.OFTDateTime;
                    break;
                case "Object":
                    ft = FieldType.OFTBinary;
                    break;
                case "Float":
                    ft = FieldType.OFTReal;
                    break;
            }
            return ft;
        }

        /// <summary>
        /// 初始化坐标系
        /// </summary>
        public void InitalzeGeometryCoordinate(string spatialText, string path, string name)
        {
            if (string.IsNullOrEmpty(spatialText))
            {
                return;
            }
            string prjFile = path + @"\" + name + ".prj";
            System.IO.File.WriteAllText(prjFile, spatialText);
        }

        #endregion

    }
}
