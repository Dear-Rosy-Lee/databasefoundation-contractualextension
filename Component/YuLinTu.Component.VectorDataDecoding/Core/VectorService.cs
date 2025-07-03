using AutoMapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Interop;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.DF;
using YuLinTu.DF.Data;
using YuLinTu.DF.Enums;
using YuLinTu.DF.Logging;
using YuLinTu.DF.Zones;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal class VectorService: IVectorService
    {
        protected IDbContext DbContext { get => Db.GetInstance(); }
        private string baseUrl = Constants.baseUrl;
        private Dictionary<string, string> AppHeaders;
        ApiCaller apiCaller { get; set; }

        public VectorService()
        {
            AppHeaders = new Dictionary<string, string>();
            AppHeaders.Add(Constants.appidName, Constants.appidVaule);
            AppHeaders.Add(Constants.appKeyName, Constants.appKeyVaule);
            apiCaller = new ApiCaller();
           
        }
        public ObservableCollection<VectorDecodeBatchModel> QueryBatchTask(string zoneCode,int page=1,int pageSize=2000, string BatchType = "all")
        {
            apiCaller.client = new HttpClient();
            ObservableCollection< VectorDecodeBatchModel >result= new ObservableCollection<VectorDecodeBatchModel >();
            string url = Constants.baseUrl + Constants.Methold_QueryVectorDecTask;         
            Dictionary<string,object> body = new Dictionary<string, object>();
            body.Add("page", page);
            body.Add("pageSize",pageSize);
            body.Add("dybm",zoneCode);
            body.Add("type", BatchType);
            var en = apiCaller.PostResultListAsync<BatchTaskJsonEn>(url, AppHeaders, JsonSerializer.Serialize(body));
            en.ForEach(e =>
            {
                var model =new VectorDecodeBatchModel();
                model.BatchName = e.upload_batch_name;
                model.BatchCode = e.upload_batch_num;
                model.ZoneCode = e.dybm;
                model.NumbersOfDownloads = e.download_num;
                model.UplaodTime = e.updtime;
                model.DataCount =e.data_num;// e.data_count;//
                model.DecodeStaus = e.is_desensitized == "1" ? "是" : "否";
                //model.DecodeProgress = e.process_status == "1" ? "已送审" : "未送审";
                model.DataStaus = e.process_status;
                model.PropertyMetadata = e.metadata_json;
                 var child = DbContext.CreateQuery<VectorDecodeMode>().Where(t => t.BatchCode.Equals(model.BatchCode)).ToObservableCollection<VectorDecodeMode>();
                if(child != null&& child.Count > 0)
                {
                    model.Children = child;
                }

                 result.Add(model);
            });
            apiCaller.client.Dispose();
            return result;
        }

        public List<SpaceLandEntity>DownLoadVectorDataPrimevalData(string zoneCode,int pageIndex,int pageSize)
        {
            apiCaller.client = new HttpClient();
            List<SpaceLandEntity> landJsonEntites = new List<SpaceLandEntity>();
            string url = Constants.baseUrl + Constants.Methold_unclassified;
          
  
                Dictionary<string, object> body = new Dictionary<string, object>();
                body.Add("page", pageIndex);
                body.Add("pageSize", pageSize);
                body.Add("dybm", zoneCode);
            
                var result = apiCaller.PostResultListAsync<LandJsonEn>(url, AppHeaders, JsonSerializer.Serialize(body));
                
                if(result != null)
                {
                    
                    foreach (var item in result)
                    {
                       var land=new SpaceLandEntity();
                        land.DKBM = item.business_identification;
                        land.CBFBM = item.business_identification_owner;
                        land.BatchCode = item.upload_batch_num;
                        var shapeText = EncrypterSM.DecryptSM4(item.original_geometry_data, Constants.Sm4Key)+"#4490";
                
                        land.Shape= YuLinTu.Spatial.Geometry.FromString(shapeText);
                    landJsonEntites.Add(land);
                    }
                }
                  return landJsonEntites;
            
        
        }

        public List<SpaceLandEntity> DownLoadVectorDataAfterDecodelData(string zoneCode, int pageIndex, int pageSize, string batchCode)
        {
            apiCaller.client = new HttpClient();
            List<SpaceLandEntity> landJsonEntites = new List<SpaceLandEntity>();
            string url = Constants.baseUrl + Constants.Methold_decryption;


            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("page", pageIndex);
            body.Add("pageSize", pageSize);
            body.Add("dybm", zoneCode);
            body.Add("business_identification", batchCode);
            var result = apiCaller.PostResultListAsync<LandJsonEn>(url, AppHeaders, JsonSerializer.Serialize(body));

            if (result != null)
            {

                foreach (var item in result)
                {
                    var land = new SpaceLandEntity();
                    land.DKBM = item.business_identification;
                    land.CBFBM = item.business_identification_owner;
                    land.BatchCode = item.upload_batch_num;
                    var shapeText = EncrypterSM.DecryptSM4(item.desensitized_geometry, Constants.Sm4Key) + "#4490";

                    land.Shape = YuLinTu.Spatial.Geometry.FromString(shapeText);
                    landJsonEntites.Add(land);
                }
            }
            return landJsonEntites;
        }


        public string UpLoadVectorDataPrimevalToSever(List<SpaceLandEntity> Data, out bool isSucess)
        {
            isSucess = false;
            apiCaller.client = new HttpClient();
            string url = baseUrl + Constants.Methold_upload;
            List<LandJsonEn> landJsonEntites = new List<LandJsonEn>();
            Data.ForEach(land =>
            {
                LandJsonEn jsonEn = new LandJsonEn();
                jsonEn.dybm = land.DKBM.Substring(0, 14);
                jsonEn.upload_batch_num = land.BatchCode;
                jsonEn.business_identification = land.DKBM;
                jsonEn.business_identification = land.DKBM;
                jsonEn.data_type = "1";
                jsonEn.original_geometry_data = land.Shape.AsText();
                jsonEn.original_geometry_data = EncrypterSM.EncryptSM4(jsonEn.original_geometry_data, Constants.Sm4Key);
                //数据加密
                landJsonEntites.Add(jsonEn);

            });

            var jsonData = JsonSerializer.Serialize(landJsonEntites);
            var en = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out isSucess);
            return en;
        }

        public string UpLoadVectorDataAfterDecodeToSever(List<SpaceLandEntity> Data, out bool isSucess)
        {
            isSucess = false;
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_upload_decryption;
            List<Dictionary<string, string>> uplaodList = new List<Dictionary<string, string>>();
            foreach (var land in Data)
            {
                Dictionary<string, string> body = new Dictionary<string, string>();
                body.Add("business_identification", land.DKBM);
                var geoText = land.Shape.AsText();
                geoText = EncrypterSM.EncryptSM4(geoText, Constants.Sm4Key);
                body.Add("desensitized_geometry", geoText);
                uplaodList.Add(body);
            }
            var jsonData = JsonSerializer.Serialize(uplaodList);
            var en = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out isSucess);
            return en;
        }

        public StaticsLandJsonEn StaticsLandByZoneCode(string zoneCode, string batchCode = "")
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_StaticsLand;
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("dybm", zoneCode);
            body.Add("upload_batch_num", batchCode); 
            var jsonData = JsonSerializer.Serialize(body);
            var en = apiCaller.PostResultAsync<StaticsLandJsonEn>(url, AppHeaders, jsonData);
            return en;
        }

 

        public string UpdateUpdateApprovalStatus(string batchCode)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpdateApprovalStatus;
            Dictionary<string, string> parms = new Dictionary<string, string>();

            parms.Add("upload_batch_num", batchCode);
          
            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out bool isSucess);
            return message;
        }

     

        public string UpdateDownLoadNum(string zoneCode,string type="1", string batchCode = "")
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpdateDownNum;
            Dictionary<string, string> parms = new Dictionary<string, string>();

            parms.Add("dybm", zoneCode);
            parms.Add("type", type);
            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out bool isSucess);
            message = GetReposneMessage(isSucess, message);
            return message;
        }

        public string ClearBatchData(string batchCode)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_Delete_ClearBatchData;
            Dictionary<string, string> parms = new Dictionary<string, string>();                  
            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out bool isSucess);
            message = GetReposneMessage(isSucess, message);
            return message;
        }

        public string DeletBatchTask(string batchCode)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_Delete_BatchTask;
            Dictionary<string, string> parms = new Dictionary<string, string>();



            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out bool isSucess);
            message = GetReposneMessage(isSucess, message);
            return message;
        }

        public string UpLoadVectorDataPrimevalToSever(List<LandJsonEn> list,  string batchCode, UploadDataModel model, out bool sucess)
        {
            apiCaller.client = new HttpClient();
            string url = baseUrl + Constants.Methold_upload;
            //var jsonData = JsonSerializer.Serialize(list);
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("upload_batch_num", batchCode);

            body.Add("dataOperation", model.GetStringValue());
            body.Add("data", list);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,                // 保持缩进格式
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // 统一命名策略
                //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // 忽略 null 值
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 保留特殊字符
                //PropertyOrder = 1,                   // 需要自定义属性顺序（见下文扩展方案）
            };
            var bodyJson = JsonSerializer.Serialize(body, options);
            string msg = apiCaller.PostDataAsync(url, AppHeaders, bodyJson, out sucess);
            msg = GetReposneMessage(sucess, msg);
            
            return msg;
        }

        private static string GetReposneMessage(bool sucess, string msg,Func<string, string> SucessMsgHandel=null)
        {
            if (!sucess)
            {
           
                msg = JsonSerializer.Deserialize<Dictionary<string, object>>(msg)["message"].ToString();
            }
            else
            {
                try
                {
                    msg = JsonSerializer.Deserialize<Dictionary<string, object>>(msg)["data"].ToString();
                }
                catch
                {

                }
                msg= SucessMsgHandel==null? msg:SucessMsgHandel?.Invoke(msg);
            }

            return msg;
        }

        public string UpLoadVectorMeata(string batchCode, List<PropertyMetadata> properties)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_Update_metadata;
            var metaJson = JsonSerializer.Serialize(properties);
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("upload_batch_num", batchCode);
            body.Add("metadata_json", metaJson);            
            
            var jsonData = JsonSerializer.Serialize(body);
            //var entity = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
            //var jj = entity["metadata_json"].ToString();
            //var ff = JsonSerializer.Deserialize<List<PropertyMetadata>>(jj);
            var msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out bool isSucess);
           
            msg = GetReposneMessage(isSucess, msg);
            return msg;
        }
        public string ChangeBatchDataStatus(string batchCode, out bool sucess1)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpdateApprovalStatus;
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out  sucess1);
            message = GetReposneMessage(sucess1, message);
            return message;
        }

        public string UpLoadBatchDataNum(string batchCode)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpLoadBatchDataNum;
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("upload_batch_num", batchCode);

        
            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out bool isSucess);
            message = GetReposneMessage(isSucess, message, (tag) =>{
                if (tag == "1")
                {
                   return  "更新数据量成功！";
                }
                else
                {
                    return "更新数据量失败！";
                }
            });
            return message;
        }

        public string CancleBatchDataSendStatus(string batchCode, out bool sucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.CancleBatchDataSendStatus;
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out sucess);
            message = GetReposneMessage(sucess, message);
            return message;
        }

        public void WriteLog(LogEn log)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_WriteLog;
            var jsonData = JsonSerializer.Serialize(log);
            string msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out bool sucess);
            msg = GetReposneMessage(sucess, msg);

        }

        public string UpdateBatchsStaus(List<string> batchCodes,out bool statusSucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_uploadBatchNums;
            Dictionary<string, List<string>> body = new Dictionary<string, List<string>>();
            body.Add("uploadBatchNums", batchCodes);
         
            var jsonData = JsonSerializer.Serialize(body);
            string msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData,out statusSucess);
            msg = GetReposneMessage(statusSucess, msg);
            return msg;
        }

        public List<FeatureObject> DownVectorDataByBatchCode(string batchCode, int pageIndex, int pageSize  )
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_unclassified;
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("page", pageIndex);
            body.Add("pageSize", pageSize);
            body.Add("upload_batch_num", batchCode);
            var jsonData = JsonSerializer.Serialize(body);
            var result = apiCaller.PostResultListAsync<LandJsonEn2>(url, AppHeaders, jsonData);
            List<FeatureObject> re=new List<FeatureObject>();
            if (result != null)
            {

                foreach (var item in result)
                {
                    var fo = new FeatureObject(); 
                    var data= JsonSerializer.Deserialize<Dictionary<string, object>>(item.metadata_json);
                    if (!data.Keys.Contains("batchCode"))
                    { data.Add("batchCode", batchCode); }
                    fo.Object = data; // item.metadata_json;
                    var shapeText = EncrypterSM.DecryptSM4(item.original_geometry_data, Constants.Sm4Key) + "#4490";
                    fo.Geometry = YuLinTu.Spatial.Geometry.FromString(shapeText);
                  
                    fo.GeometryPropertyName = "Shape";
                    re.Add(fo);
           
                }
            }
            return re;

        }
        public List<FeatureObject> DownVectorDataAfterDecodeByBatchCode(string batchCode, int pageIndexOneBatchData, int pageSizeOneBatchData)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_decryption;
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("page", pageIndexOneBatchData);
            body.Add("pageSize", pageSizeOneBatchData);
            body.Add("upload_batch_num", batchCode);
            var jsonData = JsonSerializer.Serialize(body);
            var result = apiCaller.PostResultListAsync<LandJsonEn2>(url, AppHeaders, jsonData);
            List<FeatureObject> re = new List<FeatureObject>();
            if (result != null)
            {

                foreach (var item in result)
                {
                    var fo = new FeatureObject();
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(item.metadata_json);
                    data.Add("batchCode", batchCode);
                    fo.Object = data; // item.metadata_json;
                    var shapeText = EncrypterSM.DecryptSM4(item.desensitized_geometry, Constants.Sm4Key) + "#4490";
                    fo.Geometry = YuLinTu.Spatial.Geometry.FromString(shapeText);

                    fo.GeometryPropertyName = "Shape";
                    re.Add(fo);

                }
            }
            return re;
        }

        public string UpLoadVectorDataAfterDecodeToSever(List<DecodeLandEntity> Data, out bool isSucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_upload_decryption;
            var batchCodes = Data.Select(t => t.upload_batch_num).Distinct();
            Dictionary<string,object> body =new Dictionary<string, object>();
            string msg = string.Empty;
            bool Sucess = false;
            foreach (var code in batchCodes)
            {
                var datas = Data.Where(t => t.upload_batch_num.Equals(code)).ToList();
                var decodeData = datas.Select(t => new { desensitized_geometry = EncrypterSM.EncryptSM4(t.Shape.AsText(), Constants.Sm4Key), business_identification = t.DKBM }).ToList(); 

                body.Add("upload_batch_num", code);
                body.Add("data", decodeData);
                var jsonData = JsonSerializer.Serialize(body);
                if(AppHeaders.Count>2)
                {

                }
                msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out Sucess) ;
                if (!Sucess) {
                    msg = GetReposneMessage(Sucess, msg);
                    break; 
                }
                body.Clear();
            }          
            isSucess= Sucess;      
            return msg;
        }

        public string UpLoadProcessStatusByBatchCodes(List<string> batchCodesStaus, out bool statusSucess)
        {
            statusSucess = false;
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_QueryStatusByBatchNums;
       
                Dictionary<string, List<string>> body = new Dictionary<string, List<string>>();
                body.Add("uploadBatchNums", batchCodesStaus);
    
           
            var jsonData = JsonSerializer.Serialize(body);
            var msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out statusSucess);
            msg = GetReposneMessage(statusSucess, msg);
            return msg;
        }

        public string UpdateBatchStatusByBatchCode(string batchCode, out bool statusSucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpdateStatusSucessByBatchNum;
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("upload_batch_num", batchCode);

            var message = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms, out statusSucess);
            message = GetReposneMessage(statusSucess, message);
            return message;
        }

        public string UpdateBatchStaus(List<string> codes, string type, out bool statusSucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpdateStatusByBatchNums;
            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("uploadBatchNums", codes);
            body.Add("operationType", type);
            var jsonData = JsonSerializer.Serialize(body);
            var en = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out statusSucess);
            en = GetReposneMessage(statusSucess, en);
            return en;
        }

        public ObservableCollection<LogEn> QueryLogsByBatchCode(string batchCode)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_QueryLogsByBatchCode;
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("page", "1");
            body.Add("pageSize", "200");
            body.Add("owner", batchCode);
            var jsonData = JsonSerializer.Serialize(body);
            var result = apiCaller.PostResultListAsync<LogEn>(url, AppHeaders, jsonData);
           
            return result.ToObservableCollection<LogEn>();
        }

        public string UpLoadProveFile(ProveFileEn en, out bool sucess)
        {
            apiCaller.client = new HttpClient();
            string url = Constants.baseUrl + Constants.Methold_UpLoadProveFile;
          
            var jsonData = JsonSerializer.Serialize(en);
            string msg = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out sucess);
            msg = GetReposneMessage(sucess, msg);
            return msg;
        }
        public List<ProveFileEn> DownLoadProveFile(string zoneCode, int pageIndex = 1, int pageSize = 200)
        {
            apiCaller.client = new HttpClient();
            List<ProveFileEn> landJsonEntites = new List<ProveFileEn>();
            string url = Constants.baseUrl + Constants.Methold_DownLoadProveFile;


            Dictionary<string, object> body = new Dictionary<string, object>();
            body.Add("page", pageIndex);
            body.Add("pageSize", pageSize);
            body.Add("dybm", zoneCode);

            var result = apiCaller.PostResultListAsync<ProveFileEn>(url, AppHeaders, JsonSerializer.Serialize(body));

           
            return result;
        }

        public List<ZoneJsonEn> GetChildrenByZoneCode(string zoneCode)
        {
            string url = string.Empty;
            if (zoneCode.Length >= 9) return null;
            
         
           url = baseUrl + Constants.Methold_children_filter + "?code=" + zoneCode;
     
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            var jsondata = JsonSerializer.Serialize(Constants.ZonesCodes);
            var zones = apiCaller.PostResultListAsync2<ZoneJsonEn>(url, AppHeaders, jsondata);            
            return zones;
        }

        public int GetBatchsCountByZoneCode(string zoneCode, ClientEum clientType)
        {
            apiCaller.client = new HttpClient();
            List<ProveFileEn> landJsonEntites = new List<ProveFileEn>();
            string url = Constants.baseUrl + Constants.Methold_BatchCount;

            string type=string.Empty;
            switch (clientType)
            {
                case ClientEum.UploadRowDataClient:
                    type = "";
                    break;
                case ClientEum.UploaDeclassifyDataClient:
                    type = "1";
                    break;
                default:
                    break;
            }
            Dictionary<string, string> body = new Dictionary<string, string>();         
            body.Add("type", type);
            body.Add("dybm", zoneCode);

            var result = apiCaller.PostDataAsync(url,  AppHeaders, JsonSerializer.Serialize(body), out bool sucess);
            int count = 0;
            if (sucess)
                try { count = int.Parse(result); }
                catch (Exception ex) { 

                }

            return count;
        }

        public BatchsStausCode GetBatchStatusByCode(string batchCode)
        {
             var   url = Constants.baseUrl + Constants.Methold_BatchStaus;
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            Dictionary<string, string> parms = new Dictionary<string, string>();     
            parms.Add("upload_batch_num", batchCode);
            BatchsStausCode result= BatchsStausCode.未送审;
            var en = apiCaller.GetResultMessageStringAsync(url, AppHeaders, parms,out bool sucess);
            if (sucess)
            {
                int.TryParse(en, out int type);
                result=(BatchsStausCode)type;
            }

            return result;
        }
    }
}

