using DotSpatial.Projections;
using GeoAPI.Geometries;
using Microsoft.Scripting.Actions;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Component.VectorDataDecoding.Repository;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.DF;
using YuLinTu.DF.Tasks;
using YuLinTu.Security;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(UploadVectorDataToBatchArgument),
        Name = "上传矢量数据", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class UploadVectorDataToBatch : YuLinTu.Task
    {
        internal IVectorService vectorService { get; set; }

        #region Ctor

        public UploadVectorDataToBatch()
        {
            Name = "上传矢量数据";
            Description = "上传矢量数据";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as UploadVectorDataToBatchArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            vectorService = new VectorService();
            var clientID = Constants.client_id; //new Authenticate().GetApplicationKey();
            // TODO : 任务的逻辑实现
            //var ShapeFilePath = args.ShapeFilePath;
            var ShapeFilePath = string.Empty;int index = 0;
            foreach (var item in args.ShpFilesInfo)
            {
                index++;
                string msg=  UploadVectorData(args, item.FullPath, clientID, index,out bool sucess);
                if(!sucess)
                {
                    this.ReportWarn(msg);
                }
                else
                {
                    this.ReportInfomation(msg);
                }
            }
          

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        private string UploadVectorData(UploadVectorDataToBatchArgument args,string ShapeFilePath, string clientID,int fileIndex,out bool scuess)
        {
            scuess = false;
            string message = string.Empty;
            int fileCount = args.ShpFilesInfo.Count;
             var dbSource = ProviderShapefile.CreateDataSourceByFileName(ShapeFilePath, false);
            var dqSource = new DynamicQuery(dbSource);
            var schemaName = string.Empty;
            var shpName = Path.GetFileNameWithoutExtension(ShapeFilePath);
            var keyName = args.DataType.GetStringValue();
            var properties = dqSource.GetElementProperties(schemaName, shpName);//结构信息
                                                                                //更新批次的元数据
            vectorService.UpLoadVectorMeata(args.BatchCode, properties);
            var shapeColumn = properties.Find(t => t.ColumnType == eDataType.Geometry);
            var sreproject = GetPrjInfo(ShapeFilePath, out int SreSrid);
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(Constants.DefualtPrj); //$"{keyName}.StartsWith({args.ZoneCode})"
            var where = QuerySection.Column(keyName).StartsWith(QuerySection.Parameter(args.ZoneCode));
            int dataCount = 0;
            ShapeFileRepostiory.PagingTrans<LandJsonEn>(dqSource, schemaName, shpName, keyName, where, (list, count) =>
            {
               var progess = (100 * (fileIndex-1) )/ fileCount+  dataCount*100/(fileCount* count);
                this.ReportProgress(progess);
                dataCount += list.Count;
                var msg = vectorService.UpLoadVectorDataPrimevalToSever(list, args.BatchCode, args.IsCover, out bool sucess);
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
                this.ReportProgress(fileCount);
                var key = obj.FastGetValue<string>(keyName);
                var shape = obj.FastGetValue<Spatial.Geometry>(shapeColumn.ColumnName);
                if (key is null || shape is null)
                    return null;
                LandJsonEn jsonEn = new LandJsonEn();
                jsonEn.dybm = key.Substring(0, 14);
                jsonEn.upload_batch_num = args.BatchCode;
                jsonEn.business_identification = key;
                jsonEn.data_type = args.DataType.GetDisplayName();
                jsonEn.original_geometry_data = SreSrid == Constants.DefualtSrid ? shape.AsText() : VectorDataProgress.ReprojectShape(shape, sreproject, dreproject, 4490).AsText();
                jsonEn.original_geometry_data = EncrypterSM.EncryptSM4(jsonEn.original_geometry_data, Constants.Sm4Key);

                Dictionary<string, object> metadataS = new Dictionary<string, object>();
                foreach (var cloum in properties)
                {
                    if (cloum.ColumnType == eDataType.Geometry) continue;
                    var temp = obj.FastGetValue(cloum.ColumnName);
                    metadataS.Add(cloum.ColumnName, temp);
                }


                jsonEn.metadata_json = Serializer.SerializeToJsonString(metadataS);//metadataS; //
                return jsonEn;
            });
            scuess = true;
            string info = vectorService.UpLoadBatchDataNum(args.BatchCode);
            this.ReportInfomation(info);
            message = $"上传文件{shpName}中{dataCount}条数据";
            WriteLog(args, clientID, shpName, ShapeFilePath, dataCount);
            return message;
        }

        private  void WriteLog(UploadVectorDataToBatchArgument args, string clientID, string shpName, string ShapeFilePath, int dataCount)
        {
            LogEn log = new LogEn();
            log.scope = args.ZoneCode;
            log.owner = args.BatchCode;
            log.sub_type = "上传矢量数据";
            log.user_id = clientID;
            log.description = $"上传文件{shpName}中{dataCount}条数据，文件全路径为：{ShapeFilePath}";
            vectorService.WriteLog(log);
        }

        private ProjectionInfo GetPrjInfo(string  shpFilePath, out int srid )
        {
            var landprj = Path.ChangeExtension(shpFilePath, "prj");
           
            string prjstr = "";
            using (var sreader = new StreamReader(landprj))
            {
                prjstr = sreader.ReadToEnd();
                int.TryParse(GetMarkValue(prjstr, "EPSG", 6), out srid);
            }
            ProjectionInfo sreproject = ProjectionInfo.FromEsriString(prjstr);
            return sreproject;
        }


        // <summary>
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
