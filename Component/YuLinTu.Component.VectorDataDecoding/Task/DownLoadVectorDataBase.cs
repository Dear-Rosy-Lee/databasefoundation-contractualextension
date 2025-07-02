using DotSpatial.Projections;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data;
using YuLinTu.Component.VectorDataDecoding.Core;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using YuLinTu.Spatial;
using YuLinTu.Data.Dynamic;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{

    public class DownLoadVectorDataBase : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        internal IVectorService vectorService { get; set; }
        #endregion

        #region Ctor

        public DownLoadVectorDataBase()
        {
            Name = "DownLoadVectorDataBase";
            Description = "This is DownLoadVectorDataBase";
        
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected  override void OnGo()
        {
           
            vectorService = new VectorService();
            // TODO : 任务的逻辑实现


        
        }
        #region Old
        internal void ExportToShape(string filename, List<IFeature> list, ProjectionInfo prjinfo)
        {
            var builder = new ShapefileConnectionStringBuilder();
            builder.DirectoryName = Path.GetDirectoryName(filename);
            var elementName = Path.GetFileNameWithoutExtension(filename);
            if (list == null || list.Count == 0)
            {
                return;
            }
            var ds = DataSource.Create<IDbContext>(ShapefileConnectionStringBuilder.ProviderType, builder.ConnectionString);
            var provider = ds.DataSource as IProviderShapefile;
            ClearExistFiles(provider, elementName);
            var writer = provider.CreateShapefileDataWriter(elementName);
            writer.Header = CreateHeader();
            writer.Header.NumRecords = list.Count;
            writer.Write(list);
            prjinfo.SaveAs(Path.ChangeExtension(filename, "prj"));
            this.ReportInfomation(string.Format("成功导出{0}条数据", list.Count));
        }

        internal void CompressFolder(string sourceFile, string zipFilePath, string zonecode)
        {
            var fileInfo = new FileInfo(sourceFile);
            string[] matchingFiles = Directory.GetFiles(fileInfo.DirectoryName, $"{fileInfo.Name}.*", SearchOption.AllDirectories);

            using (var fsOut = File.Create(zipFilePath))
            {
                using (var zipStream = new ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(5); // 压缩级别，0-9，9为最大压缩
                    zipStream.Password = zonecode + "Ylt@dzzw";
                    foreach (string matchingFile in matchingFiles)
                    {
                        var matchingFileInfo = new FileInfo(matchingFile);
                        string matchingFileInfoName = matchingFileInfo.Name;
                        string matchingFileEntryName = matchingFileInfoName; // 移除任何相对路径
                        var newEntry = new ZipEntry(matchingFileEntryName);
                        newEntry.DateTime = matchingFileInfo.LastWriteTime;
                        newEntry.Size = matchingFileInfo.Length;
                        zipStream.PutNextEntry(newEntry);
                        // 将文件内容写入zip流
                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(matchingFile))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                    }
                    zipStream.CloseEntry();
                }
            }
        }
        internal AttributesTable CreateAttributesSimple(SpaceLandEntity en)
        {
            AttributesTable attributes = new AttributesTable();
            attributes.AddAttribute("DKBM", en.DKBM);
            attributes.AddAttribute("DKMC", en.DKMC);

            attributes.AddAttribute("CBFBM", en.CBFBM);
            return attributes;
        }
        /// <summary>
        /// 创建表头
        /// </summary> 
        /// <returns></returns>
        internal DbaseFileHeader CreateHeader()
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            header.AddColumn("DKBM", 'C', 19, 0);
            header.AddColumn("DKMC", 'C', 50, 0);

            header.AddColumn("CBFBM", 'C', 18, 0);
            return header;
        }
        /// <summary>
        /// 删除已存在的文件
        /// </summary>
        internal void ClearExistFiles(IProviderShapefile provider, string elementName)
        {
            string path = string.Empty;
            try
            {
                var files = Directory.GetFiles(provider.DirectoryName, string.Format("{0}.*", elementName));
                files.ToList().ForEach(c => File.Delete(path = c));
            }
            catch (Exception ex)
            {
                //YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
                throw new Exception("删除文件" + path + "时发生错误！");
            }
        }
        #endregion

        #region new 
        public string DestinationFileName { get; set; }
        public eGeometryType GeometryType { get; set; }
        public PropertyMetadata[] propertyMetadata { get; set; }
        public SpatialReference spatialReference { get; set; }

        public Func<List<FeatureObject>> GetShpData { get; set; }

        #region Help


        public int CreateFeature(int index, ShapeFile file, FeatureObject obj,
         Dictionary<int, PropertyMetadata> dic, DotNetTypeConverter converter)
        {
            var dataDic = (obj.Object as Dictionary<string, object>);
            foreach (var item in dic)
            {
                var col = item.Key;
                if (!dataDic.Keys.Contains(item.Value.ColumnName)) continue;
                var val = dataDic[item.Value.ColumnName]?.ToString();// obj.Object.GetPropertyValue(item.Value.ColumnName);
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

        public Dictionary<int, PropertyMetadata> CreateFields(ShapeFile file, PropertyMetadata[] cols)
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
            PropertyMetadata batchCodeFiled = new PropertyMetadata();
            batchCodeFiled.ColumnName = "batchCode";
            batchCodeFiled.ColumnType= eDataType.String;
            batchCodeFiled.AliasName = "批次号";
            file.AddField(batchCodeFiled.ColumnName, GetType(batchCodeFiled), GetLength(batchCodeFiled), GetPrecision(batchCodeFiled));
            dic[index++] = batchCodeFiled;
            return dic;
        }

        public EShapeType GetWkbGeometryType(eGeometryType geometryType)
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

        public int GetPrecision(PropertyMetadata pc)
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

        public int GetLength(PropertyMetadata pc)
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

        public DBFFieldType GetType(PropertyMetadata pc)
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
        #endregion

        #endregion
    }
}
