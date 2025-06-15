using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data;
using YuLinTu.DF;
using YuLinTu.DF.Data;

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
        public ObservableCollection<VectorDecodeBatchModel> QueryBatchTask(string zoneCode,int page=1,int pageSize=2000)
        {
            apiCaller.client = new HttpClient();
            ObservableCollection< VectorDecodeBatchModel >result= new ObservableCollection<VectorDecodeBatchModel >();
            string url = Constants.baseUrl + Constants.Methold_QueryVectorDecTask;         
            Dictionary<string,object> body = new Dictionary<string, object>();
            body.Add("page", page);
            body.Add("pageSize",pageSize);
            body.Add("dybm",zoneCode);
            var en = apiCaller.PostResultListAsync<BatchTaskJsonEn>(url, AppHeaders, JsonSerializer.Serialize(body));
            en.ForEach(e =>
            {
                var model =new VectorDecodeBatchModel();
                model.BatchCode = e.upload_batch_num;
                model.ZoneCode = e.dybm;
                model.NumbersOfDownloads = e.download_num;
                model.UplaodTime = e.updtime;
                model.DataCount= e.data_count;
                model.DecodeStaus = e.is_desensitized == "1" ? "是" : "否";
                model.DecodeProgress = e.process_status == "1" ? "已送审" : "未送审";
                model.DataStaus = e.data_status;
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
                        var shapeText = EncrypterSM.DecryptSM4(item.desensitized_geometry, Constants.Sm4Key)+"#4490";
                
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
            string url = Constants.baseUrl + Constants.Methold_unclassified;


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
                    var shapeText = EncrypterSM.DecryptSM4(item.original_geometry_data, Constants.Sm4Key) + "#4490";//这个地方等佳佳接口写好了要改成脱密后数据

                    land.Shape = YuLinTu.Spatial.Geometry.FromString(shapeText);
                    landJsonEntites.Add(land);
                }
            }
            return landJsonEntites;
        }


        public string UpLoadVectorDataPrimevalToSever(List<SpaceLandEntity> Data, out bool isSucess)
        {
            isSucess = false;
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
            }
            var jsonData = JsonSerializer.Serialize(uplaodList);
            var en = apiCaller.PostDataAsync(url, AppHeaders, jsonData, out isSucess);
            return en;
        }

     
    }
}

