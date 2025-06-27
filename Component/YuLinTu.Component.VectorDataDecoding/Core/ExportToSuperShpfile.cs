using Microsoft.Scripting.Actions;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;


namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal class ExportToSuperShpfile
    {
        public string DestinationFileName { get; set; }
        public eGeometryType GeometryType { get; set; }
        public PropertyMetadata[] propertyMetadata { get; set; }    
        public SpatialReference spatialReference { get; set; }  

        public Func<List<FeatureObject>>GetShpData { get; set; }
        #region Methods

        #region Methods - Override


        #endregion

        #region Methods - Private

        public bool ExportData()
        {
            try
            {

                int cnt = 0;
                if (System.IO.File.Exists(DestinationFileName))
                {
                    
                    var files = Directory.GetFiles(
                        Path.GetDirectoryName(DestinationFileName),
                        string.Format("{0}.*", Path.GetFileNameWithoutExtension(DestinationFileName)));

                    files.ToList().ForEach(c => File.Delete(c));
                }


                YuLinTu.IO.PathHelper.CreateDirectory(DestinationFileName);

                using (ShapeFile file = new ShapeFile()) 
                {
                    var result = file.Create(DestinationFileName, GetWkbGeometryType(GeometryType));
                    if (!result.IsNullOrBlank())
                        throw new YltException(result);

                    var cols = propertyMetadata; 

                    var dic = CreateFields(file, cols);
                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
                    while(true)
                    {
                      var data=  GetShpData.Invoke();
                      if (data.Count == 0) break;
                      
                        data.ForEach(obj =>
                        {
                            if (obj.Geometry == null)
                                return ;
                            row = CreateFeature(row, file, obj, dic, converter);
                            file.WriteWKB(row - 1, writer.Write(obj.Geometry.Instance));                                                                                  
                        });
                    }
       

                    var info = spatialReference.ToEsriString();
                    if (info.IsNullOrBlank())
                    { File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info); }
                }

              

            }
            catch (ArgumentException ex)
            {
               
                return false;
            }
            catch (Exception ex)
            {
              
                return false;
            }

            return true;
        }

       
    

        private int CreateFeature(int index, ShapeFile file, FeatureObject obj,
            Dictionary<int, PropertyMetadata> dic, DotNetTypeConverter converter)
        {
            foreach (var item in dic)
            {
                var col = item.Key;
                var val = obj.Object.GetPropertyValue(item.Value.ColumnName);
                if (val == null)
                    continue;

                switch (item.Value.ColumnType)
                {
                    case eDataType.Byte:
                    case eDataType.Int16:
                    case eDataType.Int32:
                    case eDataType.Int64:
                        var valInt = converter.To<int>(val);
                        file.WriteFieldInt(index, col, valInt);
                        break;
                    case eDataType.Boolean:
                        var valBool = converter.To<bool>(val);
                        file.WriteFieldBool(index, col, valBool);
                        break;
                    case eDataType.Float:
                    case eDataType.Double:
                    case eDataType.Decimal:
                        var valDouble = converter.To<double>(val);
                        file.WriteFieldDouble(index, col, valDouble);
                        break;
                    case eDataType.String:
                    case eDataType.Guid:
                        var valString = converter.To<string>(val);
                        file.WriteFieldString(index, col, valString);
                        break;
                    case eDataType.DateTime:
                        var valDatetime = converter.To<DateTime>(val);
                        file.WriteFieldDate(index, col, valDatetime);
                        break;
                    case eDataType.Image:
                    case eDataType.Object:
                    case eDataType.Binary:
                    case eDataType.Geometry:
                    default:
                        throw new NotSupportedException();
                }

            }

            return index + 1;
        }

        private Dictionary<int, PropertyMetadata> CreateFields(ShapeFile file, PropertyMetadata[] cols)
        {
            var dic = new Dictionary<int, PropertyMetadata>();
            int index = 0;

            foreach (var col in cols)
            {
                if (col.ColumnName.ToLower() == "shape")
                    continue;
                if (col.ColumnName == ProviderShapefile.stringFIDFieldName)
                    continue;
                if (col.ColumnType == eDataType.Geometry)
                    continue;

                file.AddField(col.ColumnName, GetType(col), GetLength(col), GetPrecision(col));
                dic[index++] = col;

                //    var oField = new FieldDefn(col.ColumnName, GetType(col));
                //    //oField.SetWidth(GetLength(col));
                //    //oField.SetPrecision(GetPrecision(col));

                //    dic[oField] = col;
                //    oLayer.CreateField(oField, 1);
            }

            return dic;
        }

        private EShapeType GetWkbGeometryType(eGeometryType geometryType)
        {
            switch (geometryType)
            {
                case eGeometryType.Point:
                    return EShapeType.SHPT_POINT;
                case eGeometryType.MultiPoint:
                    return EShapeType.SHPT_MULTIPOINT;
                case eGeometryType.Polyline:
                case eGeometryType.MultiPolyline:
                    return EShapeType.SHPT_ARC;
                case eGeometryType.Polygon:
                case eGeometryType.MultiPolygon:
                    return EShapeType.SHPT_POLYGON;
                default:
                    throw new NotSupportedException();
            }
        }

        private int GetPrecision(PropertyMetadata pc)
        {
            //if (pc.DataColumn.Precision > 0)
            //    return pc.DataColumn.Precision;

            switch (pc.ColumnType)
            {
                case eDataType.Float:
                case eDataType.Double:
                case eDataType.Decimal:
                    return 6;
                case eDataType.Boolean:
                case eDataType.Byte:
                case eDataType.Int16:
                case eDataType.Int32:
                case eDataType.Int64:
                case eDataType.String:
                case eDataType.Guid:
                case eDataType.DateTime:
                case eDataType.Image:
                case eDataType.Object:
                case eDataType.Binary:
                    return 0;
                case eDataType.Geometry:
                default:
                    throw new NotSupportedException();
            }
        }

        private int GetLength(PropertyMetadata pc)
        {
            //if (pc.DataColumn.Size > 0)
            //    return pc.DataColumn.Size;

            switch (pc.ColumnType)
            {
                case eDataType.Boolean:
                    return 1;
                case eDataType.Float:
                case eDataType.Double:
                case eDataType.Decimal:
                    return 10;
                case eDataType.Byte:
                case eDataType.Int16:
                case eDataType.Int32:
                case eDataType.Int64:
                    return 10;
                case eDataType.Image:
                case eDataType.Object:
                case eDataType.String:
                case eDataType.Guid:
                case eDataType.Binary:
                    return 254;
                case eDataType.DateTime:
                    return 8;
                case eDataType.Geometry:
                default:
                    throw new NotSupportedException();
            }
        }

        private DBFFieldType GetType(PropertyMetadata pc)
        {
            switch (pc.ColumnType)
            {
                case eDataType.Byte:
                case eDataType.Int16:
                case eDataType.Int32:
                case eDataType.Int64:
                    return DBFFieldType.FTInteger;
                case eDataType.Boolean:
                    return DBFFieldType.FTLogical;
                case eDataType.Float:
                case eDataType.Double:
                case eDataType.Decimal:
                    return DBFFieldType.FTDouble;
                case eDataType.String:
                case eDataType.Guid:
                    return DBFFieldType.FTString;
                case eDataType.DateTime:
                    return DBFFieldType.FTDate;
                case eDataType.Image:
                case eDataType.Object:
                case eDataType.Binary:
                    return DBFFieldType.FTInvalid;
                case eDataType.Geometry:
                default:
                    throw new NotSupportedException();
            }
        }

    

        #endregion

        #endregion
    }
}
