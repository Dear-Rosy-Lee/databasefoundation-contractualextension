using DotSpatial.Projections;
using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.Primitives;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.DF.Data;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadVectorDataTolocalDBArgument),
        Name = "UploadVectorDataTolocalDB", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadVectorDataTolocalDB : YuLinTu.Task
    {
        #region Properties


        protected IDbContext DbContext { get => Db.GetInstance(); }
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public UploadVectorDataTolocalDB()
        {
            Name = "检查并处理矢量数据";
            Description = "为该批次任务准备矢量数据，检查后存储到本地";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as UploadVectorDataTolocalDBArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现
            ReadeData(args);

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        private void ReadeData(UploadVectorDataTolocalDBArgument argument)
        {
            int srid = 0;
            string prjstr = "";
            string zonecode = "";
            var landprj = Path.ChangeExtension(argument.ShapeFilePath, "prj");
            using (var sreader = new StreamReader(landprj))
            {
                prjstr = sreader.ReadToEnd();
                int.TryParse(GetMarkValue(prjstr, "EPSG", 6), out srid);
            }
            var landShapeList = VectorDataProgress.InitiallShapeLandList(argument.ShapeFilePath, srid, "");
            if (landShapeList == null)
            {
                return ;
            }
            string prj4490 = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
            ProjectionInfo sreproject = ProjectionInfo.FromEsriString(prj4490);
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(prjstr);
            List<SpaceLandEntity> entities = new List<SpaceLandEntity>();
            int step = 300;int tag = 0;
           var filedata=  CommitFileInformationData(argument);
            foreach (var entity in landShapeList)
            {
                SpaceLandEntity en=new SpaceLandEntity();
                en.CBFBM = entity.CBFBM;
                en.DKBM = entity.DKBM;
                en.DKMC=entity.DKMC;
                en.BatchCode = argument.BatchCode;
                en.FileID = filedata.FileID;
                var ygeo = entity.Shape as YuLinTu.Spatial.Geometry;
                en.Shape  = VectorDataProgress.ReprojectShape(ygeo, dreproject, sreproject, 4490);
                entities.Add(en);
                tag++;
                if (tag==300)
                {
                    CommitData(entities);
                }
            }
            CommitData(entities);
        
        }

        private void CommitData(List<SpaceLandEntity> entities)
        {
            if (entities.Count == 0) return;
            DbContext.BeginTransaction();
            var qc = DbContext.CreateQuery<SpaceLandEntity>();
            foreach (var t in entities)
            {
                qc.Add(t).Save();
            }
            DbContext.CommitTransaction();
            entities.Clear();
        }
        private VectorDecodeMode CommitFileInformationData(UploadVectorDataTolocalDBArgument arg)
        {
            var item = new VectorDecodeMode();
            item.FileID=Guid.NewGuid();
            item.ShapeFileName = Path.GetFileName(arg.ShapeFilePath);
            item.DataCount = (arg.LandCount).Value;
            item.UplaodTime = DateTime.Now;
            item.FilePath = arg.ShapeFilePath;
            item.BatchCode = arg.BatchCode;
          
            DbContext.BeginTransaction();
            var qc = DbContext.CreateQuery<VectorDecodeMode>();         
                qc.Add(item).Save();          
            DbContext.CommitTransaction();
            return item;
        }

        /// <summary>
        /// 获取标签值
        /// </summary> 
        private string GetMarkValue(string str, string name, int length)
        {
            var restr = string.Empty;
            var indexstart = str.LastIndexOf(name);
            if (indexstart == -1)
            {
                return "error";
            }
            if (indexstart != -1)
            {
                restr = str.Substring(indexstart + length);
                int indexend = restr.IndexOf("]]");
                if (indexend != -1)
                {
                    restr = restr.Substring(0, indexend);
                }
            }
            return restr;
        }
        #endregion

        #endregion
    }
}
