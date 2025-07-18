using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data.Dynamic;
using YuLinTu.DF.Enums;
using YuLinTu.Security;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownLoadVectorDataBeforeDodedArgument),
        Name = "下载脱密前矢量数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownLoadVectorDataBeforeDoded : DownLoadVectorDataBase
    {
        #region Properties
        //  internal IVectorService VectorService { get; set; }
        public int BatchTotalCount { get; set; }
        public int BatchHandelCount{ get; set; }
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownLoadVectorDataBeforeDoded()
        {
            Name = "下载脱密前矢量数据";
            Description = "下载该地域下脱密前矢量数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            base.OnGo();
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
           
            var args = Argument as DownLoadVectorDataBeforeDodedArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            GeometryType = eGeometryType.Polygon;

             int pageSize = 200;int progess = 0;
            spatialReference = new SpatialReference(Constants.DefualtSrid, Constants.DefualtPrj);
            HashSet<string> batchCodesHash = new HashSet<string>();

            int pageSizeB = 200; int pageIndexB = 1;
            while (true)
            {
                var batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndexB, pageSizeB, ((int)BatchsStausCode.已送审).ToString()).ToList();
                if (batchs.Count == 0) break;
                var batchCodes = batchs.Select(x => x.BatchCode).ToList();

                batchCodes.ForEach(t =>
                {
                    batchCodesHash.Add(t);
                });
                pageIndexB++;
            }
            if (batchCodesHash.Count == 0)
            {
                this.ReportWarn($"{args.ZoneCode}{args.ZoneName}未查询到处于已送审和处理中状态的批次，无可下载数据！");
                return;
            }
            this.ReportProgress(20);
            BatchTotalCount = batchCodesHash.Count;

            if (args.ZoneCode.Length<=6)
            {         
               var zoneCodes = batchCodesHash.Select(t => t.Substring(0,6)).Distinct().ToList();
                var index = 0;
                zoneCodes.ForEach(code =>
                {
                    var batchCodes = batchCodesHash.Where(t => t.StartsWith(code)).ToList();
                    index++;
                    DownLoadFileByBatchsCode(args, code, "", batchCodes,false);
                    this.ReportProgress(20 + index * 80 / zoneCodes.Count);
                });
              
            }
            else if(args.ZoneCode.Length>6)
            {
                // DownLoadFileByZoneCode(args, args.ZoneCode, args.ZoneName, pageSize);
                DownLoadFileByBatchsCode(args, args.ZoneCode, args.ZoneName, batchCodesHash.ToList<string>(),true);
            }

          
          

            #region MyRegion
            //try
            //{

            //    int cnt = 0;
            //    if (System.IO.File.Exists(DestinationFileName))
            //    {

            //        var files = Directory.GetFiles(
            //            Path.GetDirectoryName(DestinationFileName),
            //            string.Format("{0}.*", Path.GetFileNameWithoutExtension(DestinationFileName)));

            //        files.ToList().ForEach(c => File.Delete(c));
            //    }


            //    YuLinTu.IO.PathHelper.CreateDirectory(DestinationFileName);

            //    using (ShapeFile file = new ShapeFile())
            //    {
            //        var result = file.Create(DestinationFileName, GetWkbGeometryType(GeometryType));
            //        if (!result.IsNullOrBlank())
            //            throw new YltException(result);

            //        var batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.已送审).ToString()).ToList();
            //        if (batchs.Count == 0) return;
            //        if (propertyMetadata == null || propertyMetadata.Count() == 0)
            //        {
            //            propertyMetadata = JsonSerializer.Deserialize<PropertyMetadata[]>(batchs[0].PropertyMetadata);// batchs[0].mes
            //        }
            //        var cols = propertyMetadata;

            //        var dic = CreateFields(file, cols);

            //        var converter = new DotNetTypeConverter();
            //        var row = 0;
            //        var writer = new WKBWriter();
            //        while (true)
            //        {
            //            batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.已送审).ToString()).ToList();
            //            if (batchs.Count == 0) break;

            //            var batchCodes = batchs.Select(t => t.BatchCode).ToList();
            //            var statusMsg = vectorService.UpdateBatchsStaus(batchCodes, out bool updateSataus);
            //            if (!updateSataus)
            //            {
            //                this.ReportWarn(statusMsg);
            //            }
            //            foreach (var batchCode in batchCodes)
            //            {
            //                int dataCount = 0;
            //                int pageIndexOneBatchData = 1; int pageSizeOneBatchData = 200;
            //                while (true)
            //                {
            //                    List<FeatureObject> data = vectorService.DownVectorDataByBatchCode(batchCode, pageIndexOneBatchData, pageSizeOneBatchData);
            //                    if (data.Count == 0)
            //                    {

            //                        break;
            //                    }

            //                    data.ForEach(obj =>
            //                    {
            //                        if (obj.Geometry == null)
            //                            return;
            //                        row = CreateFeature(row, file, obj, dic, converter);
            //                        dataCount++;
            //                        file.WriteWKB(row - 1, writer.Write(obj.Geometry.Instance));
            //                    });
            //                    pageIndexOneBatchData++;
            //                }

            //                WriteLog(args, batchCode, dataCount);
            //                var codes = batchs.Select(t => t.BatchCode).ToList();
            //                string msg = vectorService.UpdateBatchStaus(codes, ((int)BatchsStausCode.待处理).ToString(), out bool statusSucess);
            //                if (!statusSucess)
            //                {


            //                    this.ReportWarn(msg);
            //                }
            //            }
            //            pageIndex++;


            //        };


            //        writer = null;
            //        var info = spatialReference.ToEsriString();
            //        if (!info.IsNullOrBlank())
            //        {
            //            File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info);
            //        }

            //        //写下载日志
            //        //更新批次任务为处理中


            //    }



            //}
            //catch (ArgumentException ex)
            //{
            //    this.ReportError(ex.Message);

            //}
            //catch (Exception ex)
            //{

            //    this.ReportError(ex.Message);
            //} 
            #endregion


            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        private void DownLoadFileByZoneCode(DownLoadVectorDataBeforeDodedArgument args, string ZoneCode,string ZoneName, int pageSize)
        {
            var shpfilePath = Path.Combine(args.ResultFilePath, $"{ZoneCode}{ZoneName}_{DateTime.Now.ToString("yyyyMMddhhmm")}.shp");
            var msg = DownLoadFileByZoneCode(shpfilePath, ZoneCode, ZoneName, pageSize, out bool scuess);
            if (!scuess)
            {
                this.ReportWarn(msg);
            }
            else
            {
                this.ReportInfomation(msg);
            }
        }
        private void DownLoadFileByBatchsCode(DownLoadVectorDataBeforeDodedArgument args, string ZoneCode, string ZoneName, List<string> BatchCodes, bool progess = false)
        {
            var shpfilePath = Path.Combine(args.ResultFilePath, $"{ZoneCode}{ZoneName}_{DateTime.Now.ToString("yyyyMMddhhmm")}.shp");
            var msg = DownLoadFileByBatchCodes(shpfilePath, ZoneCode, ZoneName, BatchCodes, out bool scuess, progess);
            if (!scuess)
            {
                this.ReportWarn(msg);
            }
            else
            {
                this.ReportInfomation(msg);
            }
        }

        #endregion


        #region MyRegion
        private string DownLoadFileByZoneCode(string DestinationFileName, string ZoneCode, string zoneName, int pageSize,out bool sucesss)
        {
            sucesss = false;
            string message=string.Empty;
            int pageIndex = 1;
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
                var batchs = vectorService.QueryBatchTask(ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.已送审).ToString()).ToList();
                if (batchs.Count == 0) return $"地域{ZoneCode}{zoneName}未查询到已送审数据！";

                YuLinTu.IO.PathHelper.CreateDirectory(DestinationFileName);

                using (ShapeFile file = new ShapeFile())
                {
                    var result = file.Create(DestinationFileName, GetWkbGeometryType(GeometryType));
                    if (!result.IsNullOrBlank())
                        throw new YltException(result);

                  
                    if (propertyMetadata == null || propertyMetadata.Count() == 0)
                    {
                        propertyMetadata = JsonSerializer.Deserialize<PropertyMetadata[]>(batchs[0].PropertyMetadata);// batchs[0].mes
                    }
                    var cols = propertyMetadata;

                    var dic = CreateFields(file, cols,true);

                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
                    while (true)
                    {
                        batchs = vectorService.QueryBatchTask(ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.已送审).ToString()).ToList();
                        if (batchs.Count == 0) break;

                        var batchCodes = batchs.Select(t => t.BatchCode).ToList();
                        var statusMsg = vectorService.UpdateBatchsStaus(batchCodes, out bool updateSataus);
                        if (!updateSataus)
                        {
                            this.ReportWarn(statusMsg);
                        }
                        foreach (var batchCode in batchCodes)
                        {
                            int dataCount = 0;
                            int pageIndexOneBatchData = 1; int pageSizeOneBatchData = 200;
                            while (true)
                            {
                                List<FeatureObject> data = vectorService.DownVectorDataByBatchCode(batchCode, pageIndexOneBatchData, pageSizeOneBatchData);
                                if (data.Count == 0)
                                {

                                    break;
                                }

                                data.ForEach(obj =>
                                {
                                    if (obj.Geometry == null)
                                        return;
                                    row = CreateFeature(row, file, obj, dic, converter);
                                    dataCount++;
                                    file.WriteWKB(row - 1, writer.Write(obj.Geometry.Instance));
                                });
                                pageIndexOneBatchData++;
                            }

                            WriteLog(ZoneCode, batchCode, dataCount);
                            var codes = batchs.Select(t => t.BatchCode).ToList();
                            string msg = vectorService.UpdateBatchStaus(codes, ((int)BatchsStausCode.待处理).ToString(), out bool statusSucess);
                            if (!statusSucess)
                            {


                                this.ReportWarn(msg);
                            }
                        }
                        pageIndex++;


                    };


                    writer = null;
                    var info = spatialReference.ToEsriString();
                    if (!info.IsNullOrBlank())
                    {
                        File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info);
                    }
                    
                    //写下载日志
                    //更新批次任务为处理中


                }

                sucesss = true;
                message = $"成功导出{ZoneCode}{zoneName}矢量数据：{DestinationFileName}";
                return message;

            }
            catch (ArgumentException ex)
            {
                this.ReportError(ex.Message);
                return message;
            }
            catch (Exception ex)
            {

                this.ReportError(ex.Message);
                return message;
            }
        }

        private string DownLoadFileByBatchCodes(string DestinationFileName, string ZoneCode, string zoneName,List<string> BatchCodes, out bool sucesss,bool progess)
        {
            sucesss = false;
            string message = string.Empty;
          
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

                var batchs = vectorService.QueryBatchTask(ZoneCode, 1, 1, ((int)BatchsStausCode.已送审).ToString()).ToList();
                if (batchs.Count == 0) return $"地域{ZoneCode}{zoneName}未查询到已送审数据！";
                using (ShapeFile file = new ShapeFile())
                {
                    var result = file.Create(DestinationFileName, GetWkbGeometryType(GeometryType));
                    if (!result.IsNullOrBlank())
                        throw new YltException(result);

                    if (propertyMetadata == null || propertyMetadata.Count() == 0)
                    {
                        propertyMetadata = JsonSerializer.Deserialize<PropertyMetadata[]>(batchs[0].PropertyMetadata);// batchs[0].mes
                    }
                    var cols = propertyMetadata;

                    var dic = CreateFields(file, cols, true);

                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
                  

                        //var batchCodes = batchs.Select(t => t.BatchCode).ToList();
                        var statusMsg = vectorService.UpdateBatchsStaus(BatchCodes, out bool updateSataus);
                        if (!updateSataus)
                        {
                            this.ReportWarn(statusMsg);
                        }
                        foreach (var batchCode in BatchCodes)
                        {
                        BatchHandelCount++;
                        if(progess) this.ReportProgress(20+BatchHandelCount*80/ BatchTotalCount);
                        int dataCount = 0;
                            int pageIndexOneBatchData = 1; int pageSizeOneBatchData = 200;
                            while (true)
                            {
                                List<FeatureObject> data = vectorService.DownVectorDataByBatchCode(batchCode, pageIndexOneBatchData, pageSizeOneBatchData);
                                if (data.Count == 0)
                                {

                                    break;
                                }

                                data.ForEach(obj =>
                                {
                                    if (obj.Geometry == null)
                                        return;
                                    row = CreateFeature(row, file, obj, dic, converter);
                                    dataCount++;
                                    file.WriteWKB(row - 1, writer.Write(obj.Geometry.Instance));
                                });
                                pageIndexOneBatchData++;
                            }

                            WriteLog(ZoneCode, batchCode, dataCount);
                        //var codes = BatchCodes.Select(t => t.BatchCode).ToList();
                        

                        }
                        string msg = vectorService.UpdateBatchStaus(BatchCodes, ((int)BatchsStausCode.待处理).ToString(), out bool statusSucess);
                        if (!statusSucess)
                        {

                            this.ReportWarn(msg);
                        }
                  


                  


                    writer = null;
                    var info = spatialReference.ToEsriString();
                    if (!info.IsNullOrBlank())
                    {
                        File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info);
                    }

                    //写下载日志
                    //更新批次任务为处理中


                }

                sucesss = true;
                message = $"成功导出{ZoneCode}{zoneName}矢量数据：{DestinationFileName}";
                return message;

            }
            catch (ArgumentException ex)
            {
                this.ReportError(ex.Message);
                return message;
            }
            catch (Exception ex)
            {

                this.ReportError(ex.Message);
                return message;
            }
        }
        #endregion
       
        private void WriteLog(string ZoneCode, string batchCode, int dataCount)
        {
            LogEn log = new LogEn();
            log.scope = ZoneCode;
            log.owner = batchCode;
            log.sub_type = "下载未处理数据";
            log.description = $"测绘局端下载于{DateTime.Now.ToString("g")}成功下载批次号为{batchCode}的待处理数据{dataCount}条！";
            vectorService.WriteLog(log);
        }
        #endregion
    }
}
