using DotSpatial.Projections;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data;
using System.IO;
using System.Diagnostics;
using Microsoft.Scripting.Actions;
using System.Windows.Documents;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownloadVectorDataByZoneArgument),
        Name = "下载待脱密原始数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownloadVectorDataByZone : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        private IVectorService vectorService { get; set; }
        #endregion

        #region Ctor

        public DownloadVectorDataByZone()
        {
            Name = "下载待脱密原始数据";
            Description = "下载待脱密原始数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as DownloadVectorDataByZoneArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            vectorService = new VectorService();
            // TODO : 任务的逻辑实现
            int count = 10000; int endTag = 0;
            int pageSize = 200;int shpLandCountLimit = 5000;int shpIndex = 0;
            int pageIndex = 0; int dataIndex = 0;
            int progess = 0;
            string prj4490 = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(prj4490);
            //List<SpaceLandEntity> landJsonEntites = new List<SpaceLandEntity>();
            List<IFeature> landEntites = new List<IFeature>();
            string shpfileName=string.Empty;
            string shpFullPath=string.Empty;
            while (true)
            {
                pageIndex++;
                endTag = pageIndex * pageSize;
                 var result = vectorService.DownLoadRawData(args.ZoneCode, pageIndex, pageSize);
                if (endTag > count|| result.Count==0) break;
                foreach (var item in result)
                {
                    var attributes = CreateAttributesSimple(item);
                    Feature feature = new Feature(item.Shape.Instance, attributes);
                    landEntites.Add(feature);
                }
                //landJsonEntites.AddRange(result);
                if (landEntites.Count== shpLandCountLimit)
                {
                    //   生成矢量文件
                    
                   
                    shpIndex++;
                    shpfileName = args.ZoneCode+"_"+DateTime.Now.ToString("yyyyMMdd")+ "_"+shpIndex+".shp";
                    shpFullPath = Path.Combine(args.ResultFilePath, shpfileName);
                    ExportToShape(shpFullPath, landEntites, dreproject);
                    landEntites.Clear();
                }

                
                dataIndex += result.Count;
                progess += (endTag * 100 / count);
                this.ReportProgress(progess, "已下载数据条数：" + dataIndex);
            }
            if(landEntites.Count!=0)
            {
                shpfileName = args.ZoneCode + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + shpIndex + ".shp";
                shpFullPath = Path.Combine(args.ResultFilePath, shpfileName);
                ExportToShape(shpFullPath, landEntites, dreproject);
            }
 

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion
        public void ExportToShape(string filename, List<IFeature> list, ProjectionInfo prjinfo)
        {
            var builder = new ShapefileConnectionStringBuilder();
            builder.DirectoryName = Path.GetDirectoryName(filename);
            var elementName = Path.GetFileNameWithoutExtension(filename);
            if (list == null || list.Count == 0)
            {
                return;
            }
            var ds = DataSource.Create<IDbContext>(ShapefileConnectionStringBuilder.ProviderType, builder.ConnectionString);
            var provider = ds.DataSource as IProviderShapefile;
            ClearExistFiles(provider, elementName);
            var writer = provider.CreateShapefileDataWriter(elementName);
            writer.Header = CreateHeader();
            writer.Header.NumRecords = list.Count;
            writer.Write(list);
            prjinfo.SaveAs(Path.ChangeExtension(filename, "prj"));
            this.ReportInfomation(string.Format("成功导出{0}条数据", list.Count));
        }
        internal AttributesTable CreateAttributesSimple(SpaceLandEntity en)
        {
            AttributesTable attributes = new AttributesTable();
            attributes.AddAttribute("DKBM", en.DKBM);
            attributes.AddAttribute("DKMC", en.DKMC);
          
            attributes.AddAttribute("CBFBM", en.CBFBM);
            return attributes;
        }
        /// <summary>
        /// 创建表头
        /// </summary> 
        /// <returns></returns>
        private DbaseFileHeader CreateHeader()
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            header.AddColumn("DKBM", 'C', 19, 0);
            header.AddColumn("DKMC", 'C', 50, 0);
         
            header.AddColumn("CBFBM", 'C', 18, 0);
            return header;
        }
        /// <summary>
        /// 删除已存在的文件
        /// </summary>
        private void ClearExistFiles(IProviderShapefile provider, string elementName)
        {
            string path = string.Empty;
            try
            {
                var files = Directory.GetFiles(provider.DirectoryName, string.Format("{0}.*", elementName));
                files.ToList().ForEach(c => File.Delete(path = c));
            }
            catch (Exception ex)
            {
                //YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
                throw new Exception("删除文件" + path + "时发生错误！");
            }
        }
        #endregion
    }
}
