using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Common.CShapeExport;
using NetTopologySuite.IO;
using YuLinTu;
using YuLinTu.Component.CoordinateTransformTask;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace ShapeProcess
{
    /// <summary>
    /// 文件数据筛选
    /// </summary>
    public class FilterShapeData
    {
        public void Start()
        {
            string oldshpath = ""; string newshpath = ""; string zonecode = "";
            if (string.IsNullOrEmpty(oldshpath) && string.IsNullOrEmpty(newshpath))
            {
                oldshpath = @"D:\广汉\shape\24脱密\DKLZ.shp";// D:\广汉\shape\24脱密\24脱密.shp";//原始
                newshpath = @"D:\广汉\shape\24完整\DK5120222024.shp";//脱密
            }
            var cpx = CoordinateParam.Getinstance();

            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldshpath);
            var ds = ProviderShapefile.CreateDataSourceByFileName(oldshpath, false) as IDbContext;
            var provider = ds.DataSource as IProviderShapefile;
            var shapePrj = provider.GetSpatialReference(oldShpNname);
            ShapefileDataReader sdr = new ShapefileDataReader(oldshpath, NetTopologySuite.Geometries.GeometryFactory.Default);
            int count = sdr.RecordCount;
            if (count == 0)
            {
                return;
            }
            var fieldList = new List<FieldInfo>();
            foreach (var item in sdr.DbaseHeader.Fields)
            {
                if (item.Name.EndsWith("ID") || item.Name.StartsWith("YL"))
                    continue;
                fieldList.Add(new FieldInfo()
                {
                    Name = item.Name,
                    Type = item.Type.ConvertType(),
                    Length = item.Length,
                    Precision = item.DecimalCount
                });
            }

            ds.Dispose();

            var codelist = new Dictionary<string, int>();
            var dicLand = new Dictionary<string, CodeData>();

            HashSet<EnCenterData> enlist = new HashSet<EnCenterData>();
            HashSet<EnCenterData> bigenlist = new HashSet<EnCenterData>();

            SqliteTopology sqliteTopology = new SqliteTopology();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            bigenlist = CreateEnvelopeFromShp(oldshpath, 4000, shapePrj.WKID);

            ShapeFileProcess(oldshpath, (dataReader, i) =>
            {
                var shapewkb = dataReader.GetWKB(i);
                var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
                var dkbm = GetFiledValue(dataReader, i, "DKBM");
                var envelop = bigenlist.FirstOrDefault(t => t.EnvelopShape.Contains(geo));
                if (envelop != null)
                {
                    var cd = new CodeData() { EnId = envelop.ID, Code = dkbm, RowNumber = i };
                    if (string.IsNullOrEmpty(zonecode))
                    {
                        zonecode = dkbm.Substring(0, 6);
                    }
                    envelop.AllLandList.Add(cd);
                }
            });
            foreach (var item in bigenlist)
            {
                item.ZoneCode = zonecode;
            }
            ShapeFileProcess(newshpath, (dataReader, i) =>
            {
                var dkbm = GetFiledValue(dataReader, i, "DKBM");
                if (!dic.ContainsKey(dkbm))
                {
                    dic.Add(dkbm, i);
                }
            });

            var oldReader = new ShapeFile();
            var err = oldReader.Open(oldshpath);
            if (!string.IsNullOrEmpty(err))
            {
                return;
            }
            var newReader = new ShapeFile();
            err = newReader.Open(newshpath);
            if (!string.IsNullOrEmpty(err))
            {
                return;
            }
            int index = 1;

            List<string> shapestrings = new List<string>();
            foreach (var item in bigenlist)
            {
                var insertList = new List<EnData>();
                foreach (var land in item.AllLandList)
                {
                    var geo = oldReader.GetGeometry(land.RowNumber, shapePrj.WKID);
                    var dkbm = land.Code;
                    if (dic.ContainsKey(land.Code))
                    {
                        var ngeo = newReader.GetGeometry(dic[land.Code], shapePrj.WKID);
                        if (!ShapeCompare.ShapeCompareByTol(geo, ngeo, 0.5))
                        {
                            continue;
                        }
                        shapestrings.Add(dkbm.ToString());
                    }
                }
                index++;
            }
            newReader.Close();
            oldReader.Close();

            ShapeFileExporter<object> writer = new ShapeFileExporter<object>();
            ShapeLayer layer = writer.InitiallLayer("D:\\pysj1.shp", Common.CShapeExport.EShapeType.SHPT_POLYGON, shapePrj.ToEsriString(), fieldList);
            try
            {
                ExportData(shapestrings, oldshpath, writer, layer, fieldList, shapePrj.WKID);
            }
            catch {
            }
            layer.Dispose();
        }

        private void ExportData(List<string> shapestrings, string oldpath,
            ShapeFileExporter<object> nwriter, ShapeLayer layer, List<FieldInfo> fieldList, int srid)
        {
            var writer = new ShapeFileExporter<object>();
            ShapeFile shapeLayer = new ShapeFile();
            string text = shapeLayer.Open(oldpath);
            if (!string.IsNullOrEmpty(text))
            {
                throw new Exception(text);
            }
            List<string> strings = new List<string>() {
                "DKBM", "DKMC", "DKDZ", "DKNZ", "DKBZ", "DKXZ", "YDKBM"
            };

            ShapeFileProcess(oldpath, strings, (datareader, i, data) =>
            {
                IDictionary<string, object> dictionary = data as ExpandoObject;
                var dkbm = dictionary["DKBM"] as string;
                if (!shapestrings.Contains(dkbm))
                    return;
                var geo = YuLinTu.Spatial.Geometry.FromBytes(datareader.GetWKB(i)); //YuLinTu.ObjectExtension.GetPropertyValue(data, "Shape") as YuLinTu.Spatial.Geometry;
                if (geo == null)
                    return;
                dictionary["Shape"]=geo;
                ExpandoObjectDynamicWrapper dynamicWrapper = new ExpandoObjectDynamicWrapper(data as ExpandoObject);
                
                nwriter.WriteVectorFile(layer, new List<object>() { data });
            });


            //Dictionary<string, int> fdic = new Dictionary<string, int>();
            //foreach (var field in strings)
            //{
            //    fdic.Add(field, layer.FindField(field));
            //}

            //for (int i = 0; i < shapeLayer.GetRecordCount(); i++)
            //{
            //    ExpandoObject expandoObject = new ExpandoObject();
            //    IDictionary<string, object> dictionary = expandoObject;
            //    foreach (var field in fdic)
            //    {
            //        object fieldValue = shapeLayer.GetFieldValue(i, 48);
            //        dictionary[field.Key] = fieldValue;
            //    }
            //    var dkbm = dictionary["DKBM"] as string;
            //    if (!shapestrings.Contains(dkbm))
            //        continue;
            //    var geo = Geometry.FromBytes(shapeLayer.GetWKB(i), srid);// YuLinTu.ObjectExtension.GetPropertyValue(expandoObject, "Shape") as YuLinTu.Spatial.Geometry;
            //    if (geo == null)
            //        continue;
            //    nwriter.WriteVectorFile(layer, new List<object>() { expandoObject });
            //}

            //writer.ReadLayerEnum(oldpath, (l, d) =>
            //{
            //    try
            //    {
            //        var dkbm = YuLinTu.ObjectExtension.GetPropertyValue(d, "DKBM") as string;
            //        if (!shapestrings.Contains(dkbm))
            //            return;
            //        var geo = YuLinTu.ObjectExtension.GetPropertyValue(d, "Shape") as YuLinTu.Spatial.Geometry;
            //        if (geo == null) return;
            //        nwriter.WriteVectorFile(layer, new List<object>() { d });
            //    }
            //    catch (Exception ex)
            //    {
            //        return;
            //    }
            //});
            //var operateShape = new OperateShapeFile(oldpath, newpath);


            //operateShape.ReadShapeData((shpLandItem, dataCount) =>
            //{
            //    try
            //    {
            //        var dkbm = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "DKBM") as string;
            //        if (!shapestrings.Contains(dkbm))
            //            return;
            //        var geo = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
            //        if (geo == null) return;
            //        if (!operateShape.WriteShapeFile(shpLandItem, geo))
            //        {
            //            operateShape.Dispose();
            //            return;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        return;
            //    }
            //}, () =>
            //{
            //    operateShape.Dispose();
            //});
        }


        /// <summary>
        /// 从文件中获取范围
        /// </summary>
        private HashSet<EnCenterData> CreateEnvelopeFromShp(string shpfile, int distance, int srid)
        {
            YuLinTu.Spatial.Envelope envelope = null;
            ShapeFileProcess(shpfile, (dataReader, i) =>
            {
                var shapewkb = dataReader.GetWKB(i);
                var geo = Geometry.FromBytes(shapewkb, srid);
                if (envelope == null)
                {
                    envelope = geo.GetEnvelope();
                }
                else
                {
                    envelope.Union(geo.GetEnvelope());
                }
            });
            var bigenlist = SplitEnvelopeToList(envelope, distance);
            return bigenlist;
        }

        /// <summary>
        /// 将Envelope分为多个范围
        /// </summary>
        /// <param name="envelope"></param>
        /// <param name="distance">分割的长度/宽度</param>
        /// <returns></returns>
        private HashSet<EnCenterData> SplitEnvelopeToList(Envelope envelope, int distance, int h = 0, int l = 0)
        {
            var dx = envelope.MaxX - envelope.MinX;
            var dy = envelope.MaxY - envelope.MinY;
            //int part = distance / 5;
            var numx = Math.Ceiling(dx / distance);
            var numy = Math.Ceiling(dy / distance);
            var enlist = new HashSet<EnCenterData>();
            for (int i = 0; i < numx; i++)
            {
                for (int j = 0; j < numy; j++)
                {
                    EnCenterData data = new EnCenterData();
                    var en = new Envelope();

                    en.MinX = envelope.MinX + i * distance;
                    en.MinY = envelope.MinY + j * distance;

                    en.MaxX = en.MinX + distance;
                    en.MaxY = en.MinY + distance;
                    data.EnvelopShape = en.ToGeometry();
                    data.HH = h * 100 + i + 1;
                    data.LH = l * 100 + j + 1;
                    enlist.Add(data);
                }
            }
            return enlist;
        }

        private EnData SearchParam(Geometry geo, List<EnData> paralist, int distance)
        {
            if (paralist.Count == 0) return null;
            var ng = geo.Buffer(distance);
            var p = paralist.FirstOrDefault(t => t.Shape.Intersects(ng));
            if (p != null)
                return p;
            else
                return SearchParam(ng, paralist, distance);
        }

        /// <summary>
        /// 获取shape全部数据
        /// </summary>
        public string ShapeFileProcess(string shapepath, Action<ShapeFile, int> Process)
        {
            using (var dataReader = new ShapeFile())
            {
                var err = dataReader.Open(shapepath);
                if (!string.IsNullOrEmpty(err))
                    return err;
                for (int i = 0; i < dataReader.GetRecordCount(); i++)
                {
                    if (Process != null)
                        Process(dataReader, i);
                }
            }
            return null;
        }


        /// <summary>
        /// 获取shape全部数据
        /// </summary>
        public string ShapeFileProcess(string shapepath, List<string> filedlist, Action<ShapeFile, int, object> Process)
        {
            using (var dataReader = new ShapeFile())
            {
                var err = dataReader.Open(shapepath);
                if (!string.IsNullOrEmpty(err))
                    return err;

                Dictionary<string, int> map = new Dictionary<string, int>();
                foreach (var f in filedlist)
                {
                    map.Add(f, dataReader.FindField(f));
                }

                for (int i = 0; i < dataReader.GetRecordCount(); i++)
                { 
                    ExpandoObject expandoObject = new ExpandoObject(); 
                    IDictionary<string, object> dictionary = expandoObject;
                    foreach (var field in map)
                    {
                        object fieldValue = dataReader.GetFieldValue(i, field.Value); 
                        dictionary[field.Key] = fieldValue;
                    }
                    dictionary["Shape"] = dataReader.GetWKB(i);
                    if (Process != null)
                        Process(dataReader, i, expandoObject);
                }
            }
            return null;
        }


        /// <summary>
        /// 获取子段值
        /// </summary>
        protected string GetFiledValue(ShapeFile dataReader, int rowid, string fieldName)
        {
            int fieldIndex = dataReader.FindField(fieldName.Trim());
            if (fieldIndex == -1)
            {
                return "NoField";
            }
            string returnVaule = dataReader.GetFieldString(rowid, fieldIndex);
            return string.IsNullOrEmpty(returnVaule) ? "" : returnVaule;
        }

    }

    public class ExpandoObjectDynamicWrapper : DynamicObject
    {
        private readonly ExpandoObject _expando;

        public ExpandoObjectDynamicWrapper(ExpandoObject expando)
        {
            _expando = expando;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var dict = _expando as IDictionary<string, object>;
            if (dict.TryGetValue(binder.Name, out result))
            {
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var dict = _expando as IDictionary<string, object>;
            if (dict.ContainsKey(binder.Name))
            {
                dict[binder.Name] = value;
                return true;
            }
            return base.TrySetMember(binder, value);
        }
    }
}
