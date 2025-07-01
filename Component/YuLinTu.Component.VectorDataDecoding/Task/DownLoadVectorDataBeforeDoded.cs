using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data.Dynamic;
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
           
            int pageIndex = 1; int pageSize = 200;
          
            DestinationFileName = Path.Combine(args.ResultFilePath, $"{args.ZoneCode}_{args.ZoneName}_{DateTime.Now.ToString("yyyyMMdd")}.shp");
            GeometryType = eGeometryType.Polygon;
            spatialReference = new SpatialReference(Constants.DefualtSrid, Constants.DefualtPrj);

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

                    var batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, "1").ToList();
                    if (batchs.Count == 0) return;
                    if (propertyMetadata == null || propertyMetadata.Count() == 0)
                    {
                        propertyMetadata = JsonSerializer.Deserialize<PropertyMetadata[]>(batchs[0].PropertyMetadata);// batchs[0].mes
                    }
                    var cols = propertyMetadata;

                    var dic = CreateFields(file, cols);
           
                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
                    while (true)
                    {
                        batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize,"1").ToList();
                        if (batchs.Count == 0) break;
                       
                        var batchCodes = batchs.Select(t => t.BatchCode).ToList();
                       var statusMsg= vectorService.UpdateBatchsStaus(batchCodes ,out bool updateSataus);
                        if(!updateSataus)
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
                                if (data.Count == 0) { 
                                    
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

                            WriteLog(args, batchCode, dataCount);
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



            }
            catch (ArgumentException ex)
            {
                this.ReportError(ex.Message);

            }
            catch (Exception ex)
            {

                this.ReportError(ex.Message);
            }
           

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion
        private void WriteLog(DownLoadVectorDataBeforeDodedArgument args,string batchCode,  int dataCount)
        {
            LogEn log = new LogEn();
            log.scope = args.ZoneCode;
            log.owner = batchCode;
            log.sub_type = "下载未处理数据";
            log.description = $"测绘局端下载于{DateTime.Now.ToString("g")}成功下载批次号为{batchCode}的待处理数据{dataCount}条！";
            vectorService.WriteLog(log);
        }
        #endregion
    }
}
