using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Data.Dynamic;
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
        internal IVectorService VectorService { get; set; }
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
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as DownLoadVectorDataBeforeDodedArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            VectorService=new VectorService();
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

                    var batchs = VectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize).ToList();
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
                        batchs = VectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize).ToList();
                        if (batchs.Count == 0) break;
                       
                        var batchCodes = batchs.Select(t => t.BatchCode).ToList();
                        VectorService.UpdateBatchsStaus(batchCodes);
                        foreach (var batchCode in batchCodes)
                        {
                            int pageIndexOneBatchData = 1; int pageSizeOneBatchData = 200;
                            while (true)
                            {
                                List<FeatureObject> data = VectorService.DownVectorDataByBatchCode(batchCode, pageIndexOneBatchData, pageSizeOneBatchData);
                                if (data.Count == 0) break;
                                data.ForEach(obj =>
                                {
                                    if (obj.Geometry == null)
                                        return;
                                    row = CreateFeature(row, file, obj, dic, converter);
                                    file.WriteWKB(row - 1, writer.Write(obj.Geometry.Instance));
                                });
                                pageIndexOneBatchData++;
                            }

                      
                        }
                        pageIndex++;


                    };


                   
                    var info = spatialReference.ToEsriString();
                    if (info.IsNullOrBlank())
                    { File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info); }
                }



            }
            catch (ArgumentException ex)
            {

                
            }
            catch (Exception ex)
            {

             
            }
          
     


            // TODO : 任务的逻辑实现
            //查询已送审和待处理的批次

            //更新批次状态为待处理

            //下载待处理批次的数据

            //生成矢量文件



            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
