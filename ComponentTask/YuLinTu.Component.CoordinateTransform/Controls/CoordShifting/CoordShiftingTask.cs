using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.CShapeExport;
using DotSpatial.Projections;
using NetTopologySuite.IO;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 数据坐标偏移
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "数据偏移", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CoordShiftingTask : Task
    {
        #region Fields   
        private CoordShiftingArgument args;
        private SqliteTopology sqliteTopology;
        private object lockobj = new object();
        #endregion

        #region Ctor
        public CoordShiftingTask()
        {
            Name = "数据偏移";
            Description = "矢量文件数据进行偏移";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            sqliteTopology = new SqliteTopology();
            args = Argument as CoordShiftingArgument;

            if (!ValidateArgs())
                return;
            int index = 0;
            foreach (var sp in args.OldShapePath)
            {
                TransformNoArgs(sp, args.NewShapePath, args.OldShapePrj, args.OldShapePath.Count, index);
                index++;
            }
            this.ReportProgress(100, "完成");
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs()
        {
            if (args.IsReChangeCoord)
                if (args.OldShapePrj.CreateProjectionInfo().Name.Equals(
                args.ChangeDestinationPrj.CreateProjectionInfo().Name))
                {
                    this.ReportError("原坐标系不能与目标坐标系相同！");
                    return false;
                }
            if (args.OldShapePath == null || args.OldShapePath.Count == 0)
            {
                this.ReportError("请选择源Shape文件路径!");
                return false;
            }
            if (string.IsNullOrEmpty(args.NewShapePath))
            {
                this.ReportError("请选择新Shape文件的保存路径!");
                return false;
            }
            var listfolder = new List<string>();
            foreach (var item in args.OldShapePath)
            {
                listfolder.Add(Path.GetDirectoryName(item));
            }
            if (listfolder.Distinct().Any(a => a == args.NewShapePath))
            {
                this.ReportError(" 新Shape文件的保存路径不能和原路径相同");
                return false;
            }
            this.ReportInfomation(string.Format("参数设置正确。"));
            return true;
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 转换数据
        /// </summary>
        public void TransformNoArgs(string oldPath, string newPath, SpatialReference destprj, int shapecount, int index)
        {
            this.ReportInfomation(string.Format("开始转换Shape文件: {0}。", Path.GetFileName(oldPath)));

            string filepath = System.IO.Path.GetDirectoryName(oldPath);
            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldPath);
            string dstshppath = System.IO.Path.Combine(newPath, oldShpNname + ".shp");
            string dstnoshppath = System.IO.Path.Combine(newPath, oldShpNname + "_wpy.shp");

            string errormsg = OperateShapeFile.DeleteShapeFile(dstshppath);
            if (!string.IsNullOrEmpty(errormsg))
            {
                this.ReportError(errormsg);
                return;
            }

            ShapefileDataReader sdr = new ShapefileDataReader(oldPath, NetTopologySuite.Geometries.GeometryFactory.Default);
            int count = sdr.RecordCount;
            if (count == 0)
            {
                this.ReportAlert("", "shape文件为空！");
                return;
            }
            var fieldList = new List<FieldInfo>();
            foreach (var item in sdr.DbaseHeader.Fields)
            {
                fieldList.Add(new FieldInfo()
                {
                    Name = item.Name,
                    Type = item.Type.ConvertType(),
                    Length = item.Length,
                    Precision = item.DecimalCount
                });
            }
            var shpType = sdr.ShapeHeader.ShapeType;
            sdr.Dispose();
            var singecount = 100 / shapecount;
            var shpecount = index * singecount;
            var result = TransData(oldPath, destprj, dstshppath, dstnoshppath, shpType, fieldList);
            if (result)
            {
                this.ReportInfomation(string.Format("Shape文件转换完成。"));
            }
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        private bool TransData(string srcshppath, SpatialReference destprj,
            string dstshppath, string dstysshppath, ShapeGeometryType shpType, List<FieldInfo> fieldList)
        {
            this.ReportProgress(1, "参数匹配中...");
            CoordinateParam cpx = CoordinateParam.Getinstance();//Ceatecpx(); //
            var evlist = sqliteTopology.GetEnCenterDatas("512022", destprj.WKID);
            this.ReportProgress(2, "数据分组中...");
            List<IndexCode> indexCodes = LandEnvelopInsert(evlist, srcshppath, destprj.WKID);
            if (indexCodes.Count > 0)
                this.ReportInfomation($"未在分组中的数据{indexCodes.Count}条");
            fieldList = CreateFileist();

            this.ReportProgress(3, "数据分组完成...");
            ShapeFileExporter<object> writer = new ShapeFileExporter<object>();
            ShapeLayer layer = null;
            string infostr = destprj.ToEsriString();
            if (string.IsNullOrEmpty(infostr))
            {
                ProjectionInfo info = destprj.CreateProjectionInfo();
                if (info != null)
                    infostr = info.ToEsriString();
            }
            layer = writer.InitiallLayer(dstshppath, (Common.CShapeExport.EShapeType)shpType, infostr, fieldList);
            if (layer == null)
            {
                this.ReportError(string.Format("创建图层失败，无法转换"));
                return false;
            }
            //var codelist = new List<string>() { //"5120220012020600141",
            //    //"5120220012020500115",
            //    //"5120220022050200451",
            //   // "5120220022050200400",
            //    //"5120220012061200128",
            //    "5120221070030600651",
            //    //"5120221070030600717"
            //    };
            this.ReportProgress(4, "数据处理中...");
            int index = 1;
            int processdatacount = 0;
            int processccount = 0;
            int processncount = 0;
            int evcount = 0;
            int encount = 0;
            evlist.RemoveAll(r => r.AllLandList.Count == 0);
            double p = 95.0 / evlist.Count;
            List<object> nodatalist = new List<object>();
            foreach (var item in evlist)
            {
                //if (!item.AllLandList.Any(t => codelist.Contains(t.Code)))
                //    continue;
                encount += item.AllLandList.Count;
                this.ReportProgress(3 + ((int)(p * (index - 1))), $"({index}/{evlist.Count})数据处理中...");
                var paramlist = sqliteTopology.GetEnDatas(item.ID.ToString(), destprj.WKID);
                var line = item.EnvelopShape.ToPolylines()[0];
                var bufparamlist = sqliteTopology.GetByShapeinserts(item.ZoneCode, line.Buffer(10));
                foreach (var bfp in bufparamlist)
                {
                    if (!paramlist.Any(t => t.ID == bfp.ID))
                        paramlist.Add(bfp);
                }
                var rowlist = item.AllLandList.Select(s => s.RowNumber).ToList();
                evcount += rowlist.Count;
                if (evcount != encount)
                {

                }
                List<object> cgdatalist = new List<object>();
                List<object> ngdatalist = new List<object>();
                ShapeFileDatas(srcshppath, rowlist, (datas) =>
                {
                    if (IsStopPending)
                        return;
                    processdatacount += datas.Count;
                    if (datas.Count == 0)
                    {
                        return;
                    }
                    ChageDataCoordnate(cpx, datas, paramlist, destprj);
                    foreach (var d in datas)
                    {
                        if (d.SFPY == 0)
                        {
                            nodatalist.Add(d);
                            ngdatalist.Add(d);
                            processncount += 1;
                        }
                        else
                        {
                            processccount += 1;
                            cgdatalist.Add(d);
                        }
                    }
                    writer.WriteVectorFile(layer, cgdatalist);
                });
                index++;
            }
            if (layer != null)
                layer.Dispose();

            if (indexCodes.Count > 0)
            {
                ShapeFileDatas(srcshppath, indexCodes.Select(s => s.Index).ToList(), (datas) =>
                {
                    nodatalist.AddRange(datas);
                });
            }
            if (nodatalist.Count > 0)
            {
                ShapeLayer nolayer = writer.InitiallLayer(dstysshppath, (Common.CShapeExport.EShapeType)shpType, infostr, fieldList);
                writer.WriteVectorFile(nolayer, nodatalist);
                nolayer.Dispose();
            }
            this.ReportInfomation($"共处理{processdatacount}条数据,其中偏移{processccount}条，未偏移{processncount}条");
            return true;
        }

        private List<FieldInfo> CreateFileist()
        {
            var fieldList = new List<FieldInfo>
            {
                new FieldInfo()
                {
                    Name = "DKMC",
                    Type = Common.CShapeExport.DBFFieldType.FTString,
                    Length = 20,
                    Precision = 0
                },
                new FieldInfo()
                {
                    Name = "DKBM",
                    Type = Common.CShapeExport.DBFFieldType.FTString,
                    Length = 50,
                    Precision = 0
                },
                new FieldInfo()
                {
                    Name = "SCMJ",
                    Type = Common.CShapeExport.DBFFieldType.FTDouble,
                    Length = 18,
                    Precision = 2
                },
                new FieldInfo()
                {
                    Name = "SFPY",
                    Type = Common.CShapeExport.DBFFieldType.FTInteger,
                    Length = 18,
                    Precision = 0
                }
            };
            return fieldList;
        }

        /// <summary>
        /// 插入集合
        /// </summary>
        private List<IndexCode> LandEnvelopInsert(List<EnCenterData> evlist, string shppath, int srid)
        {
            List<IndexCode> list = new List<IndexCode>();
            int datacount = 0;
            int containscount = 0;
            int intersectcount = 0;
            int emptycount = 0;
            HashSet<string> codes = new HashSet<string>();
            var err = ShapeFileProcess(shppath, (rd, i, count) =>
            {
                if (datacount == 0)
                {
                    datacount = count;
                }
                var geo = rd.GetGeometry(i, srid);
                var dkbm = GetFiledValue(rd, i, "DKBM");
                if (!codes.Contains(dkbm))
                {
                    codes.Add(dkbm);
                }
                else
                {
                    this.ReportError($"地块编码重复: {dkbm} !");
                }
                var en = evlist.FirstOrDefault(t => t.EnvelopShape.Contains(geo));
                if (en != null)
                {
                    containscount++;
                    en.AllLandList.Add(new CodeData() { Code = dkbm, RowNumber = i });
                }
                else
                {
                    var enlist = evlist.Where(t => t.EnvelopShape.Intersects(geo)).ToList();
                    if (enlist.Count == 0)
                    {
                        emptycount++;
                        list.Add(new IndexCode() { Code = dkbm, Index = i });
                    }
                    else
                    {
                        EnCenterData encenter = null;
                        double maxarea = 0;
                        foreach (var item in enlist)
                        {
                            var area = item.EnvelopShape.Intersection(geo).Area();
                            if (encenter == null)
                            {
                                maxarea = area;
                                encenter = item;
                            }
                            else
                            {
                                if (area > maxarea)
                                {
                                    maxarea = area;
                                    encenter = item;
                                }
                            }
                        }
                        intersectcount++;
                        encenter.AllLandList.Add(new CodeData() { Code = dkbm, RowNumber = i });
                    }
                }
            });
            if (!string.IsNullOrEmpty(err))
                throw new Exception(err);
            this.ReportInfomation($"共分组处理{datacount}条数据! 范围包含{containscount}条，范围相交{intersectcount}条，不在范围的{emptycount}条");
            return list;
        }

        private CoordinateParam Ceatecpx()
        {
            string oldshpath = @"D:\广汉\shape\24脱密\24脱密.shp";// D:\广汉\shape\24脱密\24脱密.shp";//原始
            string newshpath = @"D:\广汉\shape\24完整\DK5120222024.shp";//脱密
            var cpx = CoordinateParam.Getinstance();
            cpx.IsCancel += () => { return IsStopPending; };

            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldshpath);
            string newShpNname = System.IO.Path.GetFileNameWithoutExtension(newshpath);

            var ds = ProviderShapefile.CreateDataSourceByFileName(oldshpath, false) as IDbContext;
            var provider = ds.DataSource as IProviderShapefile;
            var shapePrj = provider.GetSpatialReference(oldShpNname);

            var codelist = new Dictionary<string, int>();
            var dicLand = new Dictionary<string, CodeData>();

            //var serdatapath = "D:\\Enceterdata.xml";
            //string filepath = "D:\\fileindexDic.xml";
            HashSet<EnCenterData> enlist = new HashSet<EnCenterData>();
            HashSet<EnCenterData> bigenlist = new HashSet<EnCenterData>();

            SqliteTopology sqliteTopology = new SqliteTopology();

            //if (File.Exists(serdatapath))
            //{
            //    enlist = ToolSerialization.DeserializeXml(serdatapath, typeof(HashSet<EnCenterData>)) as HashSet<EnCenterData>;
            //    foreach (var item in enlist)
            //    {
            //        foreach (var land in item.AllLandList)
            //        {
            //            codelist.Add(land.Code, 0);
            //        }
            //    }
            //}
            //else
            //{
            this.ReportProgress(1, $"数据准备中...");
            Dictionary<string, int> dic = new Dictionary<string, int>();
            string zonecode = string.Empty;
            bigenlist = CreateEnvelopeFromShp(oldshpath, 4000);
            //sList<CodeData> cdlist = new List<CodeData>(); 
            ShapeFileProcess(oldshpath, (dataReader, i, count) =>
            {
                var shapewkb = dataReader.GetWKB(i);
                var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
                var dkbm = GetFiledValue(dataReader, i, "DKBM");
                var envelops = bigenlist.Where(t => t.EnvelopShape.Contains(geo)).ToList();

                foreach (var item in envelops)
                {
                    var cd = new CodeData() { EnId = item.ID, Code = dkbm, RowNumber = i };
                    if (string.IsNullOrEmpty(zonecode))
                    {
                        zonecode = dkbm.Substring(0, 6);
                    }
                    if (i % 10000 == 0)
                        this.ReportProgress(1, $"数据准备中({i}/{count})...");
                    //cdlist.Add(cd);
                    item.AllLandList.Add(cd);
                    //if (cdlist.Count == 10000)
                    //{
                    //    sqliteTopology.InsertCodeData(cdlist, "CodeData", shapePrj.WKID);
                    //    cdlist.Clear();
                    //} 
                }
            });
            //if (cdlist.Count > 0)
            //{
            //    sqliteTopology.InsertCodeData(cdlist, "CodeData", 4544);
            //}

            foreach (var item in bigenlist)
            {
                item.ZoneCode = zonecode;
            }
            sqliteTopology.InsertEnCenterData(bigenlist.ToList(), "EnCenterData", shapePrj.WKID);

            ShapeFileProcess(newshpath, (dataReader, i, count) =>
            {
                var dkbm = GetFiledValue(dataReader, i, "DKBM");
                if (!dic.ContainsKey(dkbm))
                {
                    dic.Add(dkbm, i);
                }
            });
            this.ReportProgress(2, $"参数计算中...");
            var oldReader = new ShapeFile();
            var err = oldReader.Open(oldshpath);
            if (!string.IsNullOrEmpty(err))
                return null;
            var newReader = new ShapeFile();
            err = newReader.Open(newshpath);
            if (!string.IsNullOrEmpty(err))
                return null;
            int index = 1;
            foreach (var item in bigenlist)
            {
                if (IsStopPending)
                {
                    break;
                }
                this.ReportProgress(2, $"参数计算中({index}/{bigenlist.Count})...");
                var insertList = new List<EnData>();
                var tempenlist = SplitEnvelopeToList(item.EnvelopShape.GetEnvelope(), 100, item.HH, item.LH);
                foreach (var land in item.AllLandList)
                {
                    var geo = oldReader.GetGeometry(land.RowNumber, shapePrj.WKID);
                    var dkbm = land.Code;
                    var envelop = tempenlist.FirstOrDefault(t => t.EnvelopShape.Contains(geo));
                    if (envelop != null && envelop.AllLandList.Count < 201)
                    {
                        land.Shape = geo;
                        envelop.AllLandList.Add(land);
                    }
                }

                foreach (var envelop in tempenlist)
                {
                    if (IsStopPending)
                    {
                        break;
                    }
                    if (envelop.AllLandList.Count == 0)
                        continue;
                    var targetDatas = new List<CodeData>();
                    foreach (var land in envelop.AllLandList)
                    {
                        if (dic.ContainsKey(land.Code))
                        {
                            var d = new CodeData()
                            {
                                Code = land.Code,
                                Shape = newReader.GetGeometry(dic[land.Code], shapePrj.WKID)
                            };
                            targetDatas.Add(d);
                        }
                    }
                    var paramdata = cpx.CoordnateCalcParam(envelop, targetDatas);
                    if (paramdata != null)
                    {
                        paramdata.ID = item.ID;
                        paramdata.XIndex = envelop.HH;
                        paramdata.YIndex = envelop.LH;
                        paramdata.ZoneCode = zonecode;
                        insertList.Add(paramdata);
                    }
                }
                sqliteTopology.InsertEnData(insertList, "EnData", shapePrj.WKID);
                index++;
            }
            newReader.Close();
            oldReader.Close();

            //foreach (var land in item.AllLandList)
            //{
            //    var shapewkb = oldReader.GetWKB(land.RowNumber);
            //    var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);

            //    var dkbm = land.Code;
            //    var envelops = tempenlist.Where(t => t.EnvelopShape.Contains(geo)).ToList();
            //    foreach (var en in envelops)
            //    {
            //        if (en.AllLandList.Count > 149)
            //            break;

            //        var cd = new CodeData() { EnId = en.ID, Code = dkbm, Shape = geo, IsBase = 1 };
            //        en.AllLandList.Add(cd);
            //        if (!codelist.ContainsKey(dkbm))
            //        {
            //            codelist.Add(dkbm, 0);
            //        }
            //    }

            //}
            //Parallel.ForEach(item.AllLandList, (land =>
            //{
            //    lock (lockobj)
            //    {
            //        var shapewkb = dataReader.GetWKB((land as CodeDataEX).index);
            //        var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
            //        //var geo = YuLinTu.ObjectExtension.GetPropertyValue(d, "Shape") as YuLinTu.Spatial.Geometry;
            //        var dkbm = land.Code;
            //        var envelops = tempenlist.Where(t => t.EnvelopShape.Contains(geo)).ToList();
            //        foreach (var en in envelops)
            //        {
            //            if (en.AllLandList.Count > 149)
            //                break;

            //            var cd = new CodeData() { EnId = en.ID, Code = dkbm, Shape = geo, IsBase = 1 };
            //            en.AllLandList.Add(cd);
            //            if (!codelist.ContainsKey(dkbm))
            //            {
            //                codelist.Add(dkbm, 0);
            //            }
            //        }
            //    }
            //})); 
            //foreach (var item in bigenlist)
            //{
            //    this.ReportProgress(3, $"{index}/{bigenlist.Count}组织数据中...");
            //    var tempenlist = SplitEnvelopeToList(item.EnvelopShape.GetEnvelope(), 100, item.HH, item.LH);
            //    Parallel.ForEach(item.AllLandList, (land =>
            //    {
            //        lock (lockobj)
            //        {
            //            var shapewkb = dataReader.GetWKB((land as CodeDataEX).index);
            //            var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
            //            //var geo = YuLinTu.ObjectExtension.GetPropertyValue(d, "Shape") as YuLinTu.Spatial.Geometry;
            //            var dkbm = land.Code;
            //            var envelops = tempenlist.Where(t => t.EnvelopShape.Contains(geo)).ToList();
            //            foreach (var en in envelops)
            //            {
            //                if (en.AllLandList.Count > 149)
            //                    break;

            //                var cd = new CodeData() { EnId = en.ID, Code = dkbm, Shape = geo, IsBase = 1 };
            //                en.AllLandList.Add(cd);
            //                if (!codelist.ContainsKey(dkbm))
            //                {
            //                    codelist.Add(dkbm, 0);
            //                }
            //            }
            //        }
            //    }));
            //    foreach (var en in tempenlist)
            //    {
            //        if (en.AllLandList.Count > 0)
            //        {
            //            en.HH = item.HH;
            //            en.LH = item.LH;
            //            enlist.Add(en);
            //        }
            //    }
            //    GC.Collect();
            //    index++;
            //}
            ////    ToolSerialization.SerializeXml(serdatapath, enlist);
            ////}
            //this.ReportProgress(4, $"数据匹配中...");
            //this.ReportInfomation(DateTime.Now.ToString());

            //if (File.Exists(filepath))
            //{
            //    var templist = ToolSerialization.DeserializeXml(filepath, typeof(List<IndexCode>)) as List<IndexCode>;
            //    foreach (var item in templist)
            //    {
            //        if (codelist.ContainsKey(item.Code))
            //            codelist[item.Code] = item.Index;
            //    }
            //}
            //else
            //{
            //    int searchcount = codelist.Count;
            //    int comparcount = 0;
            //    ShapeFileProcess(newshpath, (dataReader, i) =>
            //    {
            //        var dkbm = GetFiledValue(dataReader, i, "DKBM");
            //        if (codelist.ContainsKey(dkbm))
            //        {
            //            comparcount++;
            //            codelist[dkbm] = i;
            //            if (dicLand.Count == searchcount)
            //                return;
            //        }
            //        this.ReportProgress(4, $"{comparcount}/{searchcount}数据匹配中...");
            //    });
            //    List<IndexCode> slist = new List<IndexCode>();
            //    foreach (var item in codelist)
            //    {
            //        slist.Add(new IndexCode() { Index = item.Value, Code = item.Key });
            //    }
            //    ToolSerialization.SerializeXml(filepath, slist);
            //}
            //using (var dataReader = new ShapeFile())
            //{
            //    var err = dataReader.Open(newshpath);
            //    if (!string.IsNullOrEmpty(err))
            //        return null;
            //    foreach (var item in codelist)
            //    {
            //        if (!dicLand.ContainsKey(item.Key))
            //        {
            //            var shapewkb = dataReader.GetWKB(item.Value);
            //            var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
            //            CodeData codeData = new CodeData();
            //            codeData.Code = item.Key;
            //            codeData.Shape = geo;
            //            dicLand.Add(item.Key, codeData);
            //        }
            //    }
            //}
            //this.ReportInfomation(DateTime.Now.ToString());
            //this.ReportProgress(5, $"参数计算中（{enlist.Count}）...");
            ////SqliteTopology sqliteTopology = new SqliteTopology();
            ////sqliteTopology.InsertEnCenterData(enlist.ToList(), "EnCenterData", 4544);
            ////sqliteTopology.InsertCodeData(dicLand.Values.ToList(), "CodeData", 4544);


            //cpx.CoordnateCalcParam(enlist, dicLand);
            //var insertList = new List<EnData>();
            //foreach (var item in enlist)
            //{
            //    var pa = cpx.coordChangeParams.FirstOrDefault(t => t.envelope == item.EnvelopShape);
            //    if (pa == null)
            //        pa = new CoordChangeParam();
            //    var data = new EnData()
            //    {
            //        ID = item.ID,
            //        XIndex = item.HH,
            //        YIndex = item.LH,
            //        Shape = item.EnvelopShape,
            //        A0 = pa.A0,
            //        A1 = pa.A1,
            //        A2 = pa.A2,
            //        B0 = pa.B0,
            //        B1 = pa.B1,
            //        B2 = pa.B2,
            //        ZoneCode = item.ZoneCode
            //    };
            //    insertList.Add(data);
            //}

            //sqliteTopology.InsertEnData(insertList, "EnData", 4544);
            return cpx;
        }

        /// <summary>
        /// 从文件中获取范围
        /// </summary>
        private HashSet<EnCenterData> CreateEnvelopeFromShp(string shpfile, int distance)
        {
            string filepath = System.IO.Path.GetDirectoryName(shpfile);
            string filename = System.IO.Path.GetFileNameWithoutExtension(shpfile);
            var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
            var provider = ds.DataSource as IProviderShapefile;
            var prj = provider.GetSpatialReference(filename);
            Spatial.Envelope envelope = null;
            ShapeFileProcess(shpfile, (dataReader, i, count) =>
            {
                var shapewkb = dataReader.GetWKB(i);
                var geo = Geometry.FromBytes(shapewkb, prj.WKID);
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
        /// 从数据库中获取范围
        /// </summary>
        private HashSet<EnCenterData> CreateEnvelopeFromBase(SqliteTopology sqlite, int distance)
        {
            //string filepath = System.IO.Path.GetDirectoryName(shpfile);
            //string filename = System.IO.Path.GetFileNameWithoutExtension(shpfile);
            //var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
            //var provider = ds.DataSource as IProviderShapefile;
            //var prj = provider.GetSpatialReference(filename);
            //Spatial.Envelope envelope = null;
            //ShapeFileProcess(filepath, (dataReader, i) =>
            //{
            //    var shapewkb = dataReader.GetWKB(i);
            //    var geo = Geometry.FromBytes(shapewkb, prj.WKID);
            //    if (envelope == null)
            //    {
            //        envelope = geo.GetEnvelope();
            //    }
            //    else
            //    {
            //        envelope.Union(geo.GetEnvelope());
            //    }
            //});
            var bigenlist = SplitEnvelopeToList(null, distance);
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

        /// <summary>
        /// 导出成shap文件
        /// </summary>
        /// <param name="shppath">文件地址</param>
        /// <param name="writer"></param>
        /// <param name="infostr">坐标系文件</param>
        /// <param name="enlist">对象列表</param>
        /// <param name="ChangeData">转换方法</param>
        private void ExportToShape(string shppath, ShapeFileExporter<object> writer, string infostr,
            HashSet<EnCenterData> enlist, Func<EnCenterData, int, List<EnData>> ChangeData)
        {
            var layer = writer.InitiallLayer(shppath, Common.CShapeExport.EShapeType.SHPT_POLYGON, infostr, EnData.FieldInfos());
            List<object> shplist = new List<object>();
            int index = 1;
            foreach (var item in enlist)
            {
                shplist.AddRange(ChangeData(item, index));
                index++;
            }
            writer.WriteVectorFile(layer, shplist);
            layer.Dispose();
        }

        /// <summary>
        /// 转换数据
        /// </summary> 
        public List<string> ChageDataCoordnate(CoordinateParam cpx, List<Land> datalist,
            List<EnData> paralist, SpatialReference destprj, List<string> codelist = null)
        {
            var list = new List<string>();
            if (paralist.Count == 0)
                return list;
            foreach (var land in datalist)
            {
                if (IsStopPending)
                    break;
                if (codelist != null)
                    if (!codelist.Contains(land.DKBM))
                        continue;
                try
                {
                    var palist = paralist.Where(t => t.Shape.Intersects(land.Shape)).ToList();
                    if (palist.Count > 0)
                    {
                        palist.RemoveAll(p => p.A0 > 5000 || p.A0 < -5000 || p.B0 > 5000 || p.B0 < -5000);
                        if (palist.Count == 0)
                        {
                            continue;
                        }
                        var pa = palist.FirstOrDefault(t => t.Shape.Contains(land.Shape));
                        if (pa != null)
                        {
                            ShiftingGeometry(cpx, land, pa, destprj.WKID);
                        }
                        else
                        {
                            ChageDataMultiParams(land, palist, cpx, destprj.WKID);
                        }
                    }
                    else
                    {
                        var pa = SearchParam(land.Shape.GetEnvelope().ToGeometry(), paralist, 50);
                        if (pa != null)
                        {
                            ShiftingGeometry(cpx, land, pa, destprj.WKID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    list.Add(land.DKBM);
                    this.ReportError(string.Format("转换失败，请确定数据是否可以执行转换" + ex.Message));
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// 偏移图形
        /// </summary>
        private void ShiftingGeometry(CoordinateParam cpx, Land land, EnData pa, int srid)
        {
            var plg = land.Shape.Instance as NetTopologySuite.Geometries.Polygon;
            if (plg.Holes.Count() == 0)
            {
                var clist = land.Shape.ToCoordinates();
                var coordinate = cpx.CalcDataByParams(clist, pa);
                land.SFPY = 1;
                land.Shape = YuLinTu.Spatial.Geometry.CreatePolygon(coordinate.ToList(), srid);
            }
            else
            {
                //var plg = land.Shape.Instance as NetTopologySuite.Geometries.Polygon;
                var tempcoordinate = cpx.CalcDataByParams(ConvertCoord(plg.Shell.Coordinates), pa);
                NetTopologySuite.Geometries.LinearRing linear = new NetTopologySuite.Geometries.LinearRing(ConvertCoord(tempcoordinate));
                var nholes = new List<NetTopologySuite.Geometries.LinearRing>();
                foreach (var hole in plg.Holes)
                {
                    tempcoordinate = cpx.CalcDataByParams(ConvertCoord(hole.Coordinates), pa);
                    var holelinear = new NetTopologySuite.Geometries.LinearRing(ConvertCoord(tempcoordinate));
                    nholes.Add(holelinear);
                }
                var apigeo = new NetTopologySuite.Geometries.Polygon(linear, nholes.ToArray());
                apigeo.SRID = srid;
                land.SFPY = 1;
                land.Shape = YuLinTu.Spatial.Geometry.FromInstance(apigeo);
            }
        }

        private Coordinate[] ConvertCoord(GeoAPI.Geometries.Coordinate[] coordinates)
        {
            var cods = new List<Coordinate>();
            foreach (var item in coordinates)
            {
                cods.Add(new Coordinate() { X = item.X, Y = item.Y });
            }
            return cods.ToArray();
        }

        private GeoAPI.Geometries.Coordinate[] ConvertCoord(Coordinate[] coordinates)
        {
            var cods = new List<GeoAPI.Geometries.Coordinate>();
            foreach (var item in coordinates)
            {
                cods.Add(new GeoAPI.Geometries.Coordinate() { X = item.X, Y = item.Y });
            }
            return cods.ToArray();
        }

        private EnData SearchParam(Geometry geo, List<EnData> paralist, int distance)
        {
            if (paralist.Count == 0)
                return null;
            if (distance > 300)
                return null;
            var ng = geo.Buffer(distance);
            var p = paralist.FirstOrDefault(t => t.Shape.Intersects(ng));
            if (p != null)
                return p;
            else
                return SearchParam(geo, paralist, distance * 2);
        }

        private void ChageDataMultiParams(Land land, List<EnData> palist, CoordinateParam cpx, int srid)
        {
            var plg = land.Shape.Instance as NetTopologySuite.Geometries.Polygon;
            if (plg.Holes.Count() == 0)
            {
                var clist = land.Shape.ToCoordinates();
                var celist = PartShifting(cpx, clist, palist, srid);
                land.SFPY = 1;
                land.Shape = YuLinTu.Spatial.Geometry.CreatePolygon(celist.Select(s => s.NCoordinate).ToList(), srid);
            }
            else
            {
                var celist = PartShifting(cpx, ConvertCoord(plg.Shell.Coordinates), palist, srid);
                var tempcoords = celist.Select(s => s.NCoordinate).ToList();
                NetTopologySuite.Geometries.LinearRing linear = new NetTopologySuite.Geometries.LinearRing(ConvertCoord(tempcoords.ToArray()));
                var nholes = new List<NetTopologySuite.Geometries.LinearRing>();
                foreach (var hole in plg.Holes)
                {
                    var tempcoordinate = PartShifting(cpx, ConvertCoord(hole.Coordinates), palist, srid);
                    tempcoords = celist.Select(s => s.NCoordinate).ToList();
                    var holelinear = new NetTopologySuite.Geometries.LinearRing(ConvertCoord(tempcoords.ToArray()));
                    nholes.Add(holelinear);
                }
                var apigeo = new NetTopologySuite.Geometries.Polygon(linear, nholes.ToArray());
                apigeo.SRID = srid;
                land.SFPY = 1;
                land.Shape = YuLinTu.Spatial.Geometry.FromInstance(apigeo);
            }
        }

        private List<CoorEntity> PartShifting(CoordinateParam cpx, Coordinate[] clist, List<EnData> palist, int srid)
        {
            var celist = new List<CoorEntity>();
            for (int i = 0; i < clist.Length; i++)
            {
                celist.Add(new CoorEntity() { Index = i, SCoordinate = clist[i] });
            }
            foreach (var p in palist)
            {
                var pcelist = celist.FindAll(t => p.Shape.Contains(Geometry.CreatePoint(t.SCoordinate, srid))).ToList();
                foreach (var item in pcelist)
                {
                    item.NCoordinate = cpx.CalcSingeData(item.SCoordinate, p);
                }
            }
            var changepts = celist.Where(s => s.NCoordinate != null).ToList();
            bool needrechang = false;
            for (int i = 0; i < changepts.Count - 1; i++)
            {
                var yds = changepts[i].NCoordinate.Y - changepts[i + 1].NCoordinate.Y;
                var nyds = changepts[i].SCoordinate.Y - changepts[i + 1].SCoordinate.Y;

                var xds = changepts[i].NCoordinate.X - changepts[i + 1].NCoordinate.X;
                var nxds = changepts[i].SCoordinate.X - changepts[i + 1].SCoordinate.X;

                if (Math.Abs(yds - nyds) > 0.11 || Math.Abs(xds - nxds) > 1)
                {
                    needrechang = true;
                    break;
                }
            }
            if (needrechang)
            {
                foreach (var item in celist)
                {
                    item.NCoordinate = cpx.CalcSingeData(item.SCoordinate, palist[0]);
                }
            }
            else
            {
                foreach (var item in celist)
                {
                    if (item.NCoordinate == null)
                        item.NCoordinate = cpx.CalcSingeData(item.SCoordinate, palist[0]);
                }
            }
            return celist;
        }


        /// <summary>
        /// 分块计算数据
        /// </summary>
        private List<string> ChageDataCoordnate2(List<object> datalist, SpatialReference destprj, CoordinateParam cpx)
        {
            List<string> nochangelist = new List<string>();
            if (cpx == null)
                return nochangelist;
            YuLinTu.Spatial.Geometry geo = null;
            foreach (var d in datalist)
            {
                try
                {
                    geo = YuLinTu.ObjectExtension.GetPropertyValue(d, "Shape") as YuLinTu.Spatial.Geometry;
                    if (geo == null)
                    {
                        this.ReportError(string.Format("shape数据无效，无法转换"));
                        continue;
                    }
                    var palist = cpx.coordChangeParams.Where(t => t.envelope.Intersects(geo)).ToList();
                    if (palist.Count == 0)
                    {
                        string landcode = YuLinTu.ObjectExtension.GetPropertyValue(d, "DKBM") as string;
                        nochangelist.Add(landcode);
                        this.ReportError($"地块{landcode} 未获取到偏移参数,未进行偏移！");
                        continue;
                    }
                    var clist = cpx.CalcChangeByShape(geo);
                    var newgeo = YuLinTu.Spatial.Geometry.CreatePolygon(clist.ToList(), destprj.WKID);
                    YuLinTu.ObjectExtension.SetPropertyValue(d, "Shape", newgeo);
                }
                catch (Exception ex)
                {
                    string dkbm = YuLinTu.ObjectExtension.GetPropertyValue(d, "DKBM") as string;
                    var geostr = geo == null ? "" : geo.ToText();
                    this.ReportError($"转换失败，请确定{dkbm}数据是否可以执行转换：{ex.Message} ");
                }
            }
            return nochangelist;
        }

        /// <summary>
        /// 获取shape全部数据
        /// </summary>
        public string ShapeFileProcess(string shapepath, Action<ShapeFile, int, int> Process)
        {
            using (var dataReader = new ShapeFile())
            {
                var err = dataReader.Open(shapepath);
                if (!string.IsNullOrEmpty(err))
                    return err;
                var count = dataReader.GetRecordCount();
                for (int i = 0; i < count; i++)
                {
                    if (IsStopPending)
                        break;
                    if (Process != null)
                        Process(dataReader, i, count);
                }
            }
            return null;
        }

        /// <summary>
        /// 指定id的数据
        /// </summary>
        /// <param name="shapepath"></param>
        /// <param name="Process"></param>
        /// <returns></returns>
        public string ShapeFileDatas(string shapepath, List<int> numbers, Action<List<Land>> Process)
        {
            using (var dataReader = new ShapeFile())
            {
                var err = dataReader.Open(shapepath);
                if (!string.IsNullOrEmpty(err))
                    return err;

                int mcIndex = dataReader.FindField("DKMC");
                int bmIndex = dataReader.FindField("DKBM");
                int mjIndex = dataReader.FindField("SCMJ");
                List<Land> landlist = new List<Land>();
                foreach (var row in numbers)
                {
                    var land = new Land();
                    land.DKBM = dataReader.GetFieldString(row, bmIndex);
                    land.SCMJ = dataReader.GetFieldDouble(row, mjIndex).Value;
                    land.DKMC = dataReader.GetFieldString(row, mcIndex);
                    land.Shape = dataReader.GetGeometry(row);
                    landlist.Add(land);
                }
                Process(landlist);
            }
            return null;
        }



        /// <summary>
        /// 获取子段值
        /// </summary>
        protected string GetFiledValue(ShapefileDataReader dataReader, string fieldName)
        {
            int fieldIndex = GetFiledIndex(dataReader, fieldName.Trim());
            if (fieldIndex == -1)
            {
                return "NoField";
            }
            object value = dataReader.GetValue(fieldIndex);
            if (value == null)
            {
                return "";
            }
            else
            {
                string returnVaule = value.ToString().Replace("\0", "");
                return string.IsNullOrEmpty(returnVaule) ? "" : returnVaule;
            }
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

        /// <summary>
        /// 获取子段值
        /// </summary>
        protected int? GetFiledIntValue(ShapefileDataReader dataReader, string fieldName)
        {
            int fieldIndex = GetFiledIndex(dataReader, fieldName.Trim());
            if (fieldIndex == -1)
            {
                return null;
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

        /// <summary>
        /// 获取子段值
        /// </summary>
        protected long? GetFiledIntValue(ShapeFile dataReader, int rowid, string fieldName)
        {
            int fieldIndex = dataReader.FindField(fieldName.Trim());// ShapeFileInfo.GetFiledIndex(dataReader,);
            if (fieldIndex == -1)
            {
                return null;
            }
            var value = dataReader.GetFieldDouble(rowid, fieldIndex);//.GetValue(fieldIndex);
            if (value == null)
            {
                return 0;
            }
            return (long)value;
        }

        /// <summary>
        /// 获取子段面积值
        /// </summary>
        protected double? GetFiledArea(ShapefileDataReader dataReader, string fieldName)
        {
            int fieldIndex = GetFiledIndex(dataReader, fieldName.Trim());
            if (fieldIndex == -1)
            {
                return null;
            }
            object value = dataReader.GetValue(fieldIndex);
            if (value == null)
            {
                return 0;
            }
            else
            {
                double valulue = 0;
                double.TryParse(value.ToString(), out valulue);
                return valulue;
            }
        }

        /// <summary>
        /// 获取子段面积值
        /// </summary>
        protected double? GetFiledArea(ShapeFile dataReader, int rowid, string fieldName)
        {
            int fieldIndex = dataReader.FindField(fieldName.Trim());// ShapeFileInfo.GetFiledIndex(dataReader,);
            if (fieldIndex == -1)
            {
                return null;
            }
            var value = dataReader.GetFieldDouble(rowid, fieldIndex);//.GetValue(fieldIndex);
            if (value == null)
            {
                return 0;
            }
            return (double)value;
        }
        public int GetFiledIndex(ShapefileDataReader dataReader, string filedName)
        {
            int index = -1;
            DbaseFieldDescriptor[] fields = dataReader.DbaseHeader.Fields;
            if (fields == null || fields.Length == 0)
            {
                return index;
            }
            for (int i = 0; i < fields.Length; i++)
            {
                DbaseFieldDescriptor dfd = fields[i];
                if (dfd.Name.Equals(filedName))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        #endregion
    }

    public class CoorEntity
    {
        public Coordinate SCoordinate { get; set; }
        public int Index { get; set; }

        public Coordinate NCoordinate { get; set; }
    }

    public class Land
    {
        public string DKMC { get; set; }
        public string DKBM { get; set; }
        public double SCMJ { get; set; }

        public int SFPY { get; set; }

        public Geometry Shape { get; set; }
    }
}
