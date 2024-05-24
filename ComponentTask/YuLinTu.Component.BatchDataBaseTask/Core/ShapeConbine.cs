/*
 * (C)2016 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Quality.Business.TaskBasic;
using System.IO;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using OSGeo.OGR;
using YuLinTu.tGISCNet;
using YuLinTu.Spatial;
using Quality.Business.TaskBasic.GDAL;

namespace YuLinTu.Component.MergeTask
{
    /// <summary>
    /// 功能：shape文件合并
    /// </summary>
    public class ShapeConbine : IDisposable
    {
        #region Fields

        private HashSet<KeyName> knSet;

        private string mainShapePath;

        private string mergShapePath;

        private GDALShapeFileWriter<string> writer;

        public Action<string> ReportErr { get; set; }

        #endregion

        #region Properties

        public string MainShapePath { get { return mainShapePath; } set { mainShapePath = value; } }

        public string MergShapePath { get { return mergShapePath; } set { mergShapePath = value; } }

        public GDALShapeFileWriter<string> Writer { get { return writer; } set { writer = value; } }

        #endregion

        #region Ctor

        public ShapeConbine()
        {
            knSet = new HashSet<KeyName>();
        }

        public ShapeConbine(string mainPath, string mergePath)
        {
            knSet = new HashSet<KeyName>();
            mainShapePath = mainPath;
            mergShapePath = mergePath;
        }

        #endregion

        #region Methods

        public string MergFile()
        {
            var err = "";
            return err;
        }

        public string GDALMergFile(Layer inmainlayer = null, int bsmValue = -1, string mainField = "")
        {
            var codeSet = new HashSet<string>();
            if (mainField != "")
            {
                using (var shp = new ShapeFile())
                {
                    var shperr = shp.Open(mainShapePath);
                    if (!string.IsNullOrEmpty(shperr))
                    {
                        var fIndex = shp.FindField(mainField);
                        if (fIndex > -1)
                        {
                            for (int i = 0; i < shp.GetRecordCount(); i++)
                            {
                                var v = shp.GetFieldString(i, fIndex);
                                if (!string.IsNullOrEmpty(v))
                                {
                                    codeSet.Add(v);
                                }
                            }
                        }
                        else
                        {
                            return string.Format("矢量文件中缺少关键字{0}，无法合并文件", Path.GetFileName(mainShapePath));
                        }
                    }
                }
            }
            var err = "";
            if (writer == null)
                writer = new GDALShapeFileWriter<string>();
            Layer mainlayer = inmainlayer;
            if (mainlayer == null)
                mainlayer = writer.OpenLayer(mainShapePath);
            var mergelayer = writer.OpenLayerNew(mergShapePath);
            if (mainlayer == null)
                return string.Format("打开合并文件{0}出错", Path.GetFileName(mainShapePath));
            if (mergelayer == null)
                return string.Format("打开待合并文件{0}出错", Path.GetFileName(mergShapePath));

            try
            {
                var allFilds = GetFieldList(mainlayer);
                var mergFilds = GetFieldList(mergelayer);
                mergelayer.Dispose();
                if (allFilds == null)
                {
                    return string.Format("矢量文件{0}获取字段信息失败,无法进行数据合并", Path.GetFileName(mainShapePath));
                }
                if (mergFilds == null)
                {
                    return string.Format("矢量文件{0}获取字段信息失败,无法进行数据合并", Path.GetFileName(mergShapePath));
                }
                var sameFilds = new List<string>();
                for (var i = 0; i < allFilds.Count; i++)
                {
                    var item = allFilds[i];
                    knSet.Add(new KeyName() { Name = item.GetName(), Type = item.GetFieldType(), Key = i });
                    var useField = allFilds.Find(t => t.GetName() == item.GetName());
                    if (useField == null)
                        continue;
                    if (useField.GetFieldType() == item.GetFieldType())
                        sameFilds.Add(item.GetName());
                }
                if (sameFilds.Count == 0)
                {
                    return string.Format("矢量文件{0}不存在相同字段,无法合并", Path.GetFileName(mergShapePath));
                }
                MergeShapeData(mainlayer, sameFilds, bsmValue, mainField, codeSet);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (inmainlayer == null)
                    mainlayer.Dispose();
                mergelayer.Dispose();
            }
            return err;
        }

        public void Dispose()
        {
            writer.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// 合并数据
        /// </summary>
        /// <param name="layer">主图层</param>
        /// <param name="mergLayer">待合并图层</param>
        /// <param name="sameFields">相同字段名称</param>
        /// <param name="bsmValue">标识码初始值</param>
        public void MergeShapeData(Layer layer, List<string> sameFieldName, int bsmValue, string mainFiled = "", HashSet<string> codeSet = null)
        {
            var layerDefn = layer.GetLayerDefn();
            var ksBSM = knSet.FirstOrDefault(t => t.Name == "BSM");
            int bsmIndex = -1;
            if (ksBSM != null)
                bsmIndex = ksBSM.Key;
            int row = 0;
            var layerName = Path.GetFileName(mainShapePath);
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(mergShapePath);
                if (!string.IsNullOrEmpty(err))
                    return;
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (var item in sameFieldName)
                {
                    var index = shp.FindField(item);
                    if (index > -1)
                    {
                        dic.Add(item, index);
                    }
                }
                for (int i = 0; i < shp.GetRecordCount(); i++)
                {
                    bool canAdd = true;
                    Feature freature = new Feature(layerDefn);
                    foreach (var item in knSet)
                    {
                        if (!dic.ContainsKey(item.Name))
                            continue;
                        if (bsmValue > -1 && item.Name == "BSM" && row != -1)
                        {
                            freature.SetField(bsmIndex, bsmValue + row);
                            continue;
                        }
                        if (mainFiled != "" && item.Name == mainFiled)
                        {
                            var valstr = shp.GetFieldString(i, dic[item.Name]);
                            if (codeSet.Contains(valstr))
                                canAdd = false;
                            break;
                        }
                        switch (item.Type)
                        {
                            case FieldType.OFTBinary:
                                break;
                            case FieldType.OFTDate:
                                break;
                            case FieldType.OFTDateTime:
                                break;
                            case FieldType.OFTInteger:
                                var valint = shp.GetFieldInt(i, dic[item.Name]);
                                freature.SetField(item.Key, valint == null ? 0 : valint.Value);
                                break;
                            case FieldType.OFTIntegerList:
                                break;
                            case FieldType.OFTReal:
                                var valdou = shp.GetFieldDouble(i, dic[item.Name]);
                                freature.SetField(item.Key, valdou == null ? 0 : valdou.Value);
                                break;
                            case FieldType.OFTRealList:
                                break;
                            case FieldType.OFTString:
                                var valstr = shp.GetFieldString(i, dic[item.Name]);
                                freature.SetField(item.Key, valstr);
                                break;
                            case FieldType.OFTStringList:
                                break;
                            case FieldType.OFTTime:
                                break;
                            case FieldType.OFTWideString:
                                break;
                            case FieldType.OFTWideStringList:
                                break;
                            default:
                                break;
                        }
                    }
                    if (!canAdd)
                        continue;
                    var geo = shp.GetGeometry(i);
                    if (geo == null)
                    {
                        if (ReportErr != null)
                        {
                            ReportErr(string.Format("矢量文件{0}第{1}行数据的空间数据异常，不能合并该条记录", layerName, i));
                        }
                        break;
                    }
                    YuLinTu.Spatial.Geometry[] geoArray = geo.ToSingleGeometries();
                    var ogrWkb = OSGeo.OGR.Geometry.CreateFromWkb(geoArray[0].ToBytes());
                    freature.SetGeometry(ogrWkb);
                    layer.CreateFeature(freature);
                    row++;
                }
            }
        }

        /// <summary>
        /// 获取文件字段
        /// </summary>
        /// <returns></returns>
        protected List<FieldDefn> GetFieldList(Layer oLayer)
        {
            try
            {
                var list = new List<FieldDefn>();
                var oDefn = oLayer.GetLayerDefn();
                int iFieldCount = oDefn.GetFieldCount();
                for (int iField = 0; iField < iFieldCount; iField++)
                {
                    var oFieldDefn = oDefn.GetFieldDefn(iField);
                    list.Add(oFieldDefn);
                }
                return list;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 获取文件字段
        /// </summary>
        /// <returns></returns>
        protected List<DbaseFieldDescriptor> GetFieldList(string shapePath)
        {
            try
            {
                List<DbaseFieldDescriptor> dfd = new List<DbaseFieldDescriptor>();
                using (ShapefileDataReader maindr = new ShapefileDataReader(shapePath, GeometryFactory.Default))
                {
                    DbaseFileHeader maindfh = maindr.DbaseHeader;
                    for (int i = 0; i < maindfh.Fields.Length; i++)
                    {
                        DbaseFieldDescriptor field = maindfh.Fields[i];
                        dfd.Add(field);
                    }
                }
                return dfd;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最大标识码
        /// </summary>
        public static int MaxBSM(string shpFile)
        {
            int maxbsm = 0;
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(shpFile);
                if (!string.IsNullOrEmpty(err))
                    return maxbsm;
                int bsmId = shp.FindField("BSM");
                if (bsmId < 0)
                    return maxbsm;
                for (int i = 0; i < shp.GetRecordCount(); i++)
                {
                    var cbsm = shp.GetFieldInt(i, bsmId);
                    if (cbsm != null && cbsm > maxbsm)
                        maxbsm = cbsm.Value;
                }
            }
            return maxbsm;
        }

        #endregion
    }

    /// <summary>
    /// 标识码实体
    /// </summary>
    public class KeyName
    {
        public int Key { get; set; }

        public string Name { get; set; }

        public OSGeo.OGR.FieldType Type { get; set; }
    }
}
