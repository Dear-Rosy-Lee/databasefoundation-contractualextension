using DotSpatial.Projections;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Component.VectorDataDecoding.Repository;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.DF;
using YuLinTu.DF.Files;
using YuLinTu.Security;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadVectorDataAfterDecodeByBatchArgument),
        Name = "上传脱密数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadVectorDataAfterDecodeByBatch : YuLinTu.Task
    {
        #region Properties
        private IVectorService vectorService { get; set; }
        private int UploadDataLimit = 200;
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public UploadVectorDataAfterDecodeByBatch()
        {
            Name = "上传脱密数据";
            Description = "上传脱密后数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as UploadVectorDataAfterDecodeByBatchArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            if (args.ResultFilePath.IsNullOrEmpty())
            {
                this.ReportError("请选择存放数据的文件夹");
                return;
            }
            if (args.ShpFilesInfo == null || args.ShpFilesInfo.Count == 0)
            {
                this.ReportError("未识别到矢量数据");
                return;
            }
            // TODO : 任务的逻辑实现
            vectorService = new VectorService();
            var files = args.ShpFilesInfo;
            int sucessCount = 0; int fileIndex = 0; int progess = 0;int pageSize = 200;
            int progessTag = files.Count<100?100 / files.Count:1;
            HashSet<string> BatchCodes = new HashSet<string>();
          
            foreach (var item in files)
            {
                List<string> batchCodesStaus = new List<string>();
                progess = fileIndex * 100 / files.Count;
                fileIndex++;
                if (item.Description.IsNotNullOrEmpty()) continue;
                var dbSource = ProviderShapefile.CreateDataSourceByFileName(item.FullPath, false);
                var dqSource = new DynamicQuery(dbSource);
                var schemaName = string.Empty;
                var shpName = Path.GetFileNameWithoutExtension(item.FullPath);
                var keyName = args.DataType.GetStringValue();
                var properties = dqSource.GetElementProperties(schemaName, shpName);//结构信息
                                                                                    //更新批次的元数据
           
                var shapeColumn = properties.Find(t => t.ColumnType == eDataType.Geometry);
               
                ShapeFileRepostiory.PagingTrans<DecodeLandEntity>(dqSource, schemaName, shpName, keyName, null, (list, count) =>
                {
                  
                    var nums = list.Select(t => t.upload_batch_num).Distinct().ToList();
                    nums.ForEach(t =>
                    {
                        if (!BatchCodes.Contains(t))
                        {
                            BatchCodes.Add(t);
                            batchCodesStaus.Add(t);
                         }
                    });
                   var  statusMsg=  vectorService.UpLoadProcessStatusByBatchCodes(batchCodesStaus, out bool statusSucess);
                    if (!statusSucess)
                    {
                        this.ReportWarn(statusMsg);
                    }
                    batchCodesStaus.Clear();
                    var msg = vectorService.UpLoadVectorDataAfterDecodeToSever(list, out bool sucess);
                    if (!sucess)
                    {
                        this.ReportError(msg);
                    }
                    else
                    {
                        //成功的提示信息
                    }
                },
          (i, count, obj) =>
          {
            
        
              var key = obj.FastGetValue<string>(keyName);
              this.ReportProgress(progess,$"处理批次号{key}");
              var shape = obj.FastGetValue<Spatial.Geometry>(shapeColumn.ColumnName);
              var upload_batch_num = obj.FastGetValue<string>("batchCode");//batchCode
              if (key is null || shape is null)
                  return null;
              DecodeLandEntity jsonEn = new DecodeLandEntity();           
              jsonEn.upload_batch_num = upload_batch_num;
              jsonEn.DKBM = key;
              jsonEn.Shape = shape;                      
              return jsonEn;
          },pageSize);

            
            }

            //更新批次信息
            BatchCodes.ForEach(batchCode => {
                
                var statusMsg = vectorService.UpdateBatchStatusByBatchCode(batchCode, out bool statusSucess);
                if(!statusSucess) {
                    this.ReportWarn(statusMsg);
                }
                else
                {
                    this.ReportWarn($"批次号为{batchCode}的数据处理完成！");
                }

            });

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
