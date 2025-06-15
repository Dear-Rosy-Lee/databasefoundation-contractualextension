using DotSpatial.Projections;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownloadDecodeVectorDataByBatchCodeArgument),
        Name = "下载当前批次已脱密数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownloadDecodeVectorDataByBatchCode : DownLoadVectorDataBase
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownloadDecodeVectorDataByBatchCode()
        {
            Name = "下载当前批次已脱密数据";
            Description = "下载当前选中批次任务已脱密数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
            vectorService = new VectorService();
            var args = Argument as DownloadDecodeVectorDataByBatchCodeArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现
            int count = 10000; int endTag = 0;
            int pageSize = 200; int shpLandCountLimit = 5000; int shpIndex = 0;
            int pageIndex = 0; int dataIndex = 0;
            int progess = 0;
            string prj4490 = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(prj4490);
            //List<SpaceLandEntity> landJsonEntites = new List<SpaceLandEntity>();
            List<IFeature> landEntites = new List<IFeature>();
            string shpfileName = string.Empty;
            string shpFullPath = string.Empty;
            while (true)
            {
                pageIndex++;
                endTag = pageIndex * pageSize;
                var result = vectorService.DownLoadVectorDataAfterDecodelData(string.Empty, pageIndex, pageSize, args.BatchCode);
                if (endTag > count || result.Count == 0) break;
                foreach (var item in result)
                {
                    var attributes = CreateAttributesSimple(item);
                    Feature feature = new Feature(item.Shape.Instance, attributes);
                    landEntites.Add(feature);
                }
                //landJsonEntites.AddRange(result);
                if (landEntites.Count == shpLandCountLimit)
                {
                    //   生成矢量文件


                    shpIndex++;
                    shpfileName = args.BatchCode + "_"  + shpIndex + ".shp";
                    shpFullPath = Path.Combine(args.ResultFilePath, shpfileName);
                    ExportToShape(shpFullPath, landEntites, dreproject);
                    landEntites.Clear();
                }


                dataIndex += result.Count;
                progess = (endTag * 100 / count);
                this.ReportProgress(progess, "已下载数据条数：" + dataIndex);
            }
            if (landEntites.Count != 0)
            {
                shpfileName = args.BatchCode + "_" + shpIndex + ".shp";
                shpFullPath = Path.Combine(args.ResultFilePath, shpfileName);
                ExportToShape(shpFullPath, landEntites, dreproject);
            }
            if(args.AutoComprass)
            {
                 
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
