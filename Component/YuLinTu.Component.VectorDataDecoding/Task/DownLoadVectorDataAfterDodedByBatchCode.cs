using GeoAPI.IO;
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
using YuLinTu.DF;
using YuLinTu.Security;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownLoadVectorDataAfterDodedByBatchCodeArgument),
        Name = "下载矢量数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownLoadVectorDataAfterDodedByBatchCode :DownLoadVectorDataBase
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownLoadVectorDataAfterDodedByBatchCode()
        {
            Name = "下载所选批次的矢量数据";
            Description = "下载已处理完成所选批次号的矢量数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
            base.OnGo();
            var args = Argument as DownLoadVectorDataAfterDodedByBatchCodeArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            var clientID = Constants.client_id; //new Authenticate().GetApplicationKey();
            // TODO : 任务的逻辑实现
            int pageIndex = 1; int pageSize = 200;

            DestinationFileName = Path.Combine(args.ResultFilePath, $"{args.BatchCode}_{args.BatchName}_{DateTime.Now.ToString("yyyyMMdd")}.shp");
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
                //预留了以后可能多选多个批次下载的代码
                using (ShapeFile file = new ShapeFile())
                {
                    var result = file.Create(DestinationFileName, GetWkbGeometryType(GeometryType));
                    if (!result.IsNullOrBlank())
                        throw new YltException(result);

                    var batchs = vectorService.QueryBatchTask(args.ZoneCode, pageIndex, pageSize, ((int)BatchsStausCode.处理完成).ToString()).ToList();//查询已处理完成的批次号,接口需要修改
                    if (batchs.Count == 0) return;
                    batchs = batchs.Where(t=>t.BatchCode.Equals(args.BatchCode)).ToList();
                    if (propertyMetadata == null || propertyMetadata.Count() == 0)
                    {
                        propertyMetadata = JsonSerializer.Deserialize<PropertyMetadata[]>(batchs[0].PropertyMetadata);// batchs[0].mes
                    }
                    var cols = propertyMetadata;

                    var dic = CreateFields(file, cols);

                    var converter = new DotNetTypeConverter();
                    var row = 0;
                    var writer = new WKBWriter();
    

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

                        }
                    string msg = vectorService.UpdateDownLoadNumByBatchCodes(batchCodes, out bool sucess);
                    if (!sucess)
                    {
                        this.ReportError(msg);
                    }
                    pageIndex++;




                    writer = null;
                    var info = spatialReference.ToEsriString();
                    if (!info.IsNullOrBlank())
                    {
                        File.WriteAllText(Path.ChangeExtension(DestinationFileName, "prj"), info);
                    }

                    //写下载日志
                    //更新批次任务为处理中


                }

                if (args.DownLoadModel != DownLoadModel.批次号)
                {
                    string extend = args.BatchCode.Substring(args.BatchCode.Length - 4, 4);
                    var scuess = SplitVector(DestinationFileName, args.DownLoadModel, DataTypeEum.承包地.GetStringValue(), extend);
                    if (scuess)
                    {
                        DeleteShpFile(DestinationFileName);
                    }
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
        private static void DeleteShpFile(string DestinationFileName)
        {
            if (System.IO.File.Exists(DestinationFileName))
            {

                var files = Directory.GetFiles(
                    Path.GetDirectoryName(DestinationFileName),
                    string.Format("{0}.*", Path.GetFileNameWithoutExtension(DestinationFileName)));

                files.ToList().ForEach(c => File.Delete(c));
            }
        }

        private bool SplitVector(string destinationFileName, DownLoadModel downLoadModel, string splitField, string extend)
        {
            bool scuess = true;
            VectorSplitter vectorSplitter = new VectorSplitter();
            var desDir = Path.GetDirectoryName(destinationFileName);
            try { 
            vectorSplitter.SplitVectorFile(destinationFileName, desDir, splitField, (splitFieldVaule =>
            {
                switch (downLoadModel)
                {
                    case DownLoadModel.村级地域:
                        splitFieldVaule = splitFieldVaule?.Substring(0, 12);
                        break;
                    case DownLoadModel.组级地域:
                        splitFieldVaule = splitFieldVaule?.Substring(0, 14);
                        break;
                    default:
                        break;
                }
                return splitFieldVaule;
            }), 50, extend);
            }catch(Exception ex)
            {
                scuess = false;
                this.ReportError(ex.Message);
            }
            return scuess;
        }

        private void WriteLog(string ZoneCode, string clientID, string batchCode, int dataCount)
        {
            LogEn log = new LogEn();
            log.scope = ZoneCode;
            log.owner = batchCode;
            log.user_id = clientID;
            log.description = $"下载批次号为{batchCode}已处理完成的数据{dataCount}条！";
            vectorService.WriteLog(log);
        }
        #endregion
    }
}
