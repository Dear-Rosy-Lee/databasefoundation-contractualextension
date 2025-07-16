using DotSpatial.Projections;
using GeoAPI.Geometries;
using Microsoft.Scripting.Actions;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Interop;
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
using YuLinTu.Spatial;
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
            bool result= ValidateArgument(args);
            if (!result)
            {
                return;
            }
            vectorService = new VectorService();
            var clientID = Constants.client_id; //new Authenticate().GetApplicationKey();
            // TODO : 任务的逻辑实现
            //var ShapeFilePath = args.ShapeFilePath;
            var ShapeFilePath = string.Empty;int index = 0;
            //先根据批次号查询状态，未送审才能继续上传  需增加接口
            BatchsStausCode batchsStaus = vectorService.GetBatchStatusByCode(args.BatchCode);
            if (batchsStaus != BatchsStausCode.未送审)
            {
                this.ReportError($"批次：{args.BatchCode}{args.BatchName} 当前不是 【未送审】 状态！");
                return;
            }
            foreach (var item in args.ShpFilesInfo)
            {
                index++;
                string msg=  UploadVectorData(args, item.FullPath, clientID, index,out bool sucess);
                if(!sucess)
                {
                    this.ReportError(msg);
                    if (args.UploadModel == UploadDataModel.追加上传)
                    {
                        vectorService.UpLoadBatchDataNum(args.BatchCode);
                        return;
                    }
                }
                else
                {
                    this.ReportInfomation(msg);
                }
            }

            string info = vectorService.UpLoadBatchDataNum(args.BatchCode);
            this.ReportInfomation(info);
            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        private bool ValidateArgument(UploadVectorDataToBatchArgument args)
        {
            //验证是否有非本地域下的地块数据
            //验证面积是否超限
            bool result =true;
            double shpArea = 0.00;
            //var ds = ProviderShapefile.CreateDataSource(args.ResultFilePath, false);
            //var dq = new DynamicQuery(ds);
            var mustFiled = args.DataType.GetStringValue();
            this.ReportInfomation("开始检测矢量文件结构和数据范围！");
            foreach (var item in args.ShpFilesInfo)
            {
                var ds = ProviderShapefile.CreateDataSourceByFileName(item.FullPath, false);
                var dq = new DynamicQuery(ds);
                var tableName = Path.GetFileNameWithoutExtension(item.FileName);
                var cloums = dq.GetElementProperties(null, tableName).Select(t=>t.ColumnName).ToArray();
                if(!cloums.Contains(mustFiled))
                {
                    this.ReportError($"矢量数据中未包含必需的字段 {mustFiled} ,文件路径：{item.FullPath}");
                    result = false;
                    continue;
                }
                //dq.w
                //var dataOut = dq.Any(null, tableName,  QuerySection.Column(mustFiled).StartsWith(QuerySection.Parameter(args.ZoneCode)));
                //if (dataOut)
                //{
                //    result = false;
                //    this.ReportError($"矢量数据中为包含地域{args.ZoneCode}之外的数据 ,文件路径：{item.FullPath}");
                //}
            }
            this.ReportInfomation("矢量文件结构检查通过！");
            this.ReportProgress(10);
            foreach (var item in args.ShpFilesInfo)
            {
                 bool breakTag=false;
                var ds = ProviderShapefile.CreateDataSourceByFileName(item.FullPath, false);
                var dq = new DynamicQuery(ds);
                var tableName = Path.GetFileNameWithoutExtension(item.FileName);
                dq.ForEach(null, tableName, (i, cnt, obj) =>
                {                    
                              
                    YuLinTu.Spatial.Geometry geo = obj.GetPropertyValue<YuLinTu.Spatial.Geometry>("Shape");
                    var mustFiledVaule= obj.GetPropertyValue<string>(mustFiled);
                    if(mustFiledVaule?.Length< args.ZoneCode.Length||!mustFiledVaule.StartsWith(args.ZoneCode))
                    {
                        this.ReportError($"矢量数据中为包含地域{args.ZoneCode}之外的数据 ,文件路径：{item.FullPath}");
                        breakTag = true;
                        return false;
                    }

                    //geo.Project(4490);//注意坐标系文件有问题时此处计算面积会出问题，后期需要处理
                    var shpAreaTpem= geo.Area();
                    if (shpAreaTpem < 0)
                    {

                    }
                    else
                    {
                        shpArea = shpArea+shpAreaTpem;
                        Console.WriteLine(shpArea.ToString());
                    }
         
                    if (shpArea>Constants.SpitialDataAreaLimint)
                    {
                        this.ReportError($"检测到图斑总面积超过25平方千米！请分批加载数据！");
                        breakTag = true;
                        return false;
                    }

                    return true;
                } ,QuerySection.Property("Shape", "Shape"), QuerySection.Property(mustFiled, mustFiled));

                if (breakTag)   
                {
                    result = false;
                    break;
                }
               
            }
            this.ReportProgress(20);

            return result;
        }

        private string UploadVectorData(UploadVectorDataToBatchArgument args,string ShapeFilePath, string clientID,int fileIndex,out bool scuess, bool isBreak=false)
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
            ShapeFileRepostiory.PagingTrans<LandJsonEn>(dqSource, schemaName, shpName, keyName, where, (list, count, breakTag) =>
            {
               var progess =20+ (80 * (fileIndex-1) )/ fileCount+  dataCount*80/(fileCount* count);
                this.ReportProgress(progess);
                dataCount += list.Count;
                var msg = vectorService.UpLoadVectorDataPrimevalToSever(list, args.BatchCode, args.UploadModel, out bool sucess);
                if (!sucess)
                {
                    this.ReportError(msg);
                    if(args.UploadModel == UploadDataModel.追加上传)
                    {

                        breakTag=true;
                        isBreak= true;
                    }
                    
                }
                else
                {
                    //成功的提示信息
                }
            },
            (i, count, obj, breakTag) =>
            {
                
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

            if(isBreak)
            {
                scuess = false;
                return "追加模式下发现重复数据中断！";
            }
            scuess = true;
            
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
