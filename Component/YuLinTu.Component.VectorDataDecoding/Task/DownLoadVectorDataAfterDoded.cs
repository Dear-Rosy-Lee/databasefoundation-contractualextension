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
using YuLinTu.DF.Enums;
using YuLinTu.Security;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownLoadVectorDataAfterDodedArgument),
        Name = "下载处理完成矢量数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownLoadVectorDataAfterDoded : DownLoadVectorDataBase
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownLoadVectorDataAfterDoded()
        {
            Name = "下载处理完成矢量数据";
            Description = "下载处理完成矢量数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as DownLoadVectorDataAfterDodedArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            base.OnGo();
            // TODO : 任务的逻辑实现
            int pageIndex = 1; int pageSize = 200;
            var clientID = Constants.client_id; //new Authenticate().GetApplicationKey();
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
                var batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.处理完成).ToString()).ToList();
                if (batchs.Count == 0) 
                {
                    this.ReportWarn($"{args.ZoneCode}{args.ZoneName}下未找到已处理完成的批次！");
                   
                    return; 
                }
                
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

                    var dic = CreateFields(file, cols);

                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
                    while (true)
                    {
                        if (pageIndex >= 1) {
                            batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.处理完成).ToString()).ToList();//查询已处理完成的批次号,接口需要修改
                        };
                        if (batchs.Count == 0) break;

                        var batchCodes = batchs.Select(t => t.BatchCode).ToList();
                        
                        foreach (var batchCode in batchCodes)
                        {
                            int dataCount = 0;
                            int pageIndexOneBatchData = 1; int pageSizeOneBatchData = 200;
                            while (true)
                            {
                                List<FeatureObject> data = vectorService.DownVectorDataAfterDecodeByBatchCode(batchCode, pageIndexOneBatchData, pageSizeOneBatchData);
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

                            WriteLog(args.ZoneCode, clientID, batchCode, dataCount);
                            //更新下载次数
                            //vectorService.UpdateDownLoadNum(batchCode);
                        }
                        pageIndex++;
                        string msg=  vectorService.UpdateDownLoadNumByBatchCodes(batchCodes,out bool sucess);
                        if(!sucess)
                        {
                            this.ReportError(msg);
                        }

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

                if (args.AutoComprass)
                {
                    //文件处理为压缩包
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
        private void WriteLog(string ZoneCode,string clientID, string batchCode, int dataCount)
        {
            LogEn log = new LogEn();
            log.scope = ZoneCode;
            log.owner = batchCode; 
            log.user_id = clientID;
            log.sub_type= "下载处理后数据";
            log.description = $"下载批次号为{batchCode}已处理完成的数据{dataCount}条！";
            vectorService.WriteLog(log);
        }
        #endregion
    }
}
