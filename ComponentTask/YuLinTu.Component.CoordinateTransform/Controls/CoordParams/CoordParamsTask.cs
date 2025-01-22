using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShapeProcess;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 偏移参数计算
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "偏移参数计算", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CoordParamsTask : Task
    {
        #region Fields   
        private CoordParamsArgument args;
        private SqliteTopology sqliteTopology;
        private object lockobj = new object();
        #endregion

        #region Ctor
        public CoordParamsTask()
        {
            Name = "偏移参数计算";
            Description = "矢量文件数据进行偏移参数计算";
            //new PadFind();
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            FilterShapeData filterShapeData = new FilterShapeData();
            filterShapeData.Start(); 
            sqliteTopology = new SqliteTopology();
            args = Argument as CoordParamsArgument;

            if (!ValidateArgs())
                return;
            int index = 0;

            ConBinShapeFile conBinShapeFile = new ConBinShapeFile();
            string oldpath = args.OldShapePath[0];
            string newpath = args.NewShapePath[0];
            var extName = "合并";
            if (args.OldShapePath.Count > 1)
            {
                oldpath = $"{Path.GetDirectoryName(oldpath)}/{Path.GetDirectoryName(oldpath)}{extName}.shp";
                conBinShapeFile.ConbinFile(args.OldShapePath, oldpath);
            }
            if (args.NewShapePath.Count > 1)
            {
                newpath = $"{Path.GetDirectoryName(newpath)}/{Path.GetDirectoryName(newpath)}{extName}.shp";
                conBinShapeFile.ConbinFile(args.NewShapePath, newpath);
            }
            Ceatecpx(oldpath, newpath);
            //TransformNoArgs(oldpath, newpath, args.OldShapePrj, args.OldShapePath.Count, index);
            this.ReportProgress(100, "完成");
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs()
        {
            if (args.OldShapePath == null || args.OldShapePath.Count == 0)
            {
                this.ReportError("请选择源Shape文件路径!");
                return false;
            }

            this.ReportInfomation(string.Format("参数设置正确。"));
            return true;
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 计算参数
        /// </summary>
        /// <param name="oldshpath"></param>
        /// <param name="newshpath"></param>
        /// <returns></returns>
        private CoordinateParam Ceatecpx(string oldshpath = "", string newshpath = "", string zonecode = "")
        {
            if (string.IsNullOrEmpty(oldshpath) && string.IsNullOrEmpty(newshpath))
            {
                oldshpath = @"D:\广汉\shape\24脱密\24脱密.shp";// D:\广汉\shape\24脱密\24脱密.shp";//原始
                newshpath = @"D:\广汉\shape\24完整\DK5120222024.shp";//脱密
            }
            var cpx = CoordinateParam.Getinstance();
            cpx.IsCancel += () => { return IsStopPending; };

            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldshpath);
            var ds = ProviderShapefile.CreateDataSourceByFileName(oldshpath, false) as IDbContext;
            var provider = ds.DataSource as IProviderShapefile;
            var shapePrj = provider.GetSpatialReference(oldShpNname);
            ds.Dispose();

            var codelist = new Dictionary<string, int>();
            var dicLand = new Dictionary<string, CodeData>();

            HashSet<EnCenterData> enlist = new HashSet<EnCenterData>();
            HashSet<EnCenterData> bigenlist = new HashSet<EnCenterData>();

            SqliteTopology sqliteTopology = new SqliteTopology();
            this.ReportProgress(1, $"数据准备中...");
            Dictionary<string, int> dic = new Dictionary<string, int>();
            bigenlist = CreateEnvelopeFromShp(oldshpath, 4000, shapePrj.WKID);

            ShapeFileProcess(oldshpath, (dataReader, i) =>
            {
                var shapewkb = dataReader.GetWKB(i);
                var geo = Geometry.FromBytes(shapewkb, shapePrj.WKID);
                var dkbm = GetFiledValue(dataReader, i, "DKBM");
                var envelops = bigenlist.Where(t => t.EnvelopShape.Contains(geo)).ToList();
                foreach (var item in envelops)
                {
                    if (item.AllLandList.Count < 40000)
                    {
                        var cd = new CodeData() { EnId = item.ID, Code = dkbm, RowNumber = i };
                        if (string.IsNullOrEmpty(zonecode))
                        {
                            zonecode = dkbm.Substring(0, 6);
                        }
                        item.AllLandList.Add(cd);
                    }
                }
            });
            foreach (var item in bigenlist)
            {
                item.ZoneCode = zonecode;
            }

            sqliteTopology.deleteDataByzoneCode($"delete from EnCenterData where ZoneCode='{zonecode}'");
            sqliteTopology.InsertEnCenterData(bigenlist.ToList(), "EnCenterData", shapePrj.WKID);

            ShapeFileProcess(newshpath, (dataReader, i) =>
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
            {
                this.ReportError($"打开文件{oldshpath}报错：{err}");
                return null;
            }
            var newReader = new ShapeFile();
            err = newReader.Open(newshpath);
            if (!string.IsNullOrEmpty(err))
            {
                this.ReportError($"打开文件{newshpath}报错：{err}");
                return null;
            }
            int index = 1;
            double percent = 90.0 / bigenlist.Count;
            sqliteTopology.deleteDataByzoneCode($"delete from EnData where ZoneCode='{zonecode}'");
            foreach (var item in bigenlist)
            {
                if (IsStopPending)
                {
                    break;
                }
                this.ReportProgress(2 + (int)(index * percent), $"参数计算中({index}/{bigenlist.Count})...");
                var insertList = new List<EnData>();
                var tempenlist = SplitEnvelopeToList(item.EnvelopShape.GetEnvelope(), 200, item.HH, item.LH);
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
            return cpx;
        }

        /// <summary>
        /// 从文件中获取范围
        /// </summary>
        private HashSet<EnCenterData> CreateEnvelopeFromShp(string shpfile, int distance, int srid)
        {
            Spatial.Envelope envelope = null;
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
                    if (IsStopPending)
                        break;
                    if (Process != null)
                        Process(dataReader, i);
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

        #endregion
    }
}
