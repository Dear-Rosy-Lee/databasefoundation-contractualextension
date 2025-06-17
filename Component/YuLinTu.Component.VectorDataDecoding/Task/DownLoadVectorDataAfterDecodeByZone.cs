using DotSpatial.Projections;
using Microsoft.Scripting.Actions;
using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownLoadVectorDataAfterDecodeByZoneArgument),
        Name = "下载当前地域下已脱密矢量数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownLoadVectorDataAfterDecodeByZone : DownLoadVectorDataBase
    {
        //private IVectorService vectorService;
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownLoadVectorDataAfterDecodeByZone()
        {
            Name = "下载当前地域下已脱密矢量数据";
            Description = "下载当前地域下所有已脱密矢量数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as DownLoadVectorDataAfterDecodeByZoneArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            if (args.ResultFilePath.IsNullOrEmpty())
            {
                this.ReportError("请选择脱密数据存放路径！");
                return;
            }
            base.OnGo();
            // TODO : 任务的逻辑实现
            //vectorService = new VectorService();
            // TODO : 任务的逻辑实现
            int count = vectorService.StaticsLandByZoneCode(args.ZoneCode).ytm;
            if (count == 0)
            {
                this.ReportWarn($"{args.ZoneName}({args.ZoneCode})下未查询到已脱密数据！");
                return;
            }
            int endTag = 0;
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
                  
                var result = vectorService.DownLoadVectorDataAfterDecodelData(args.ZoneCode, pageIndex, pageSize,string.Empty);
                if (endTag > count || result.Count == 0) break;
                endTag = endTag+ result.Count;
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
                    shpfileName = args.ZoneCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + shpIndex + ".shp";
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
                shpfileName = args.ZoneCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + shpIndex + ".shp";
                shpFullPath = Path.Combine(args.ResultFilePath, shpfileName);
                ExportToShape(shpFullPath, landEntites, dreproject);
                
            }
            string message= vectorService.UpdateDownLoadNum(args.ZoneCode,"1");
            this.ReportInfomation(message);
            if (args.AutoComprass)
            {

            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
