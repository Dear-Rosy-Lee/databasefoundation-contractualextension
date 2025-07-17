using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal  class Constants
    {
       
        internal static string baseUrl = ConfigurationManager.AppSettings.TryGetValue<string>("DefaultBusinessAPIAdress", "https://api.yizhangtu.com");
        internal const string appidName = "t-open-api-app-id";

        internal const string appidVaule = "56f8f0f619244eacae1c3495ba803e91";
        internal const string moduleUniqueKey = "9A75153E-E8E3-4219-A3D3-3670CAB204E9";

        internal const string appKeyName = "t-open-api-app-key";
        //以下配置应登录账号后获取，目前先硬编码写死

        internal const string appKeyVaule = "gPxd7u8UhffDeVEh3YxENuie5TEkfanFznnvjlKljXI/9ZUj9/fuTA==";

        internal const string Sm4Key = "efd4a17e7c2a89ea5a9d9a283fe03ae1";//"efd4a17e7c2a89ea";//5a9d9a283fe03ae1";
        //internal const int Sm4Key_16 = #efd4a17e7c2a89ea5a9d9a283fe03ae1;
        internal const string Methold_Prefix =  "/stackcloud/api/open/api/dynamic/onlineDecryption/";
        internal const string Methold_CreateVectorDecTask = Methold_Prefix + "task/add";
        internal const string Methold_QueryVectorDecTask = Methold_Prefix + "task/paging";
        internal const string Methold_unclassified = Methold_Prefix + "data/download/unclassified";
        internal const string Methold_decryption = Methold_Prefix + "data/download/decryption";
        internal const string Methold_upload = Methold_Prefix + "data/upload";
        internal const string Methold_upload_decryption = Methold_Prefix + "data/upload/decryption";
        internal const string Methold_StaticsLand = Methold_Prefix + "data/unclassified/statistics/dybm";
        internal const string Methold_UpdateApprovalStatus = Methold_Prefix + "task/submit/approval";
        internal const string Methold_UpdateDownNum = Methold_Prefix + "task/update/pull/num";

        internal const string Methold_Update_metadata = Methold_Prefix + "task/update/metadata_json";
        internal const string Methold_Delete_BatchTask = Methold_Prefix + "task/delete";
        internal const string Methold_Delete_ClearBatchData = Methold_Prefix + "data/delete/by/upload_batch_num";
      //internal const string Methold_Delete_ClearBatchData = 
        internal const string Methold_UpLoadBatchDataNum = Methold_Prefix + "task/update/data/num";
        public const string CancleBatchDataSendStatus = Methold_Prefix + "task/cancel/submit/approval";
        internal const string Methold_WriteLog = Methold_Prefix + "log/add";
        internal const string Methold_uploadBatchNums= Methold_Prefix + "task/update/uploadBatchNums";
        internal const string Methold_QueryStatusByBatchNums = Methold_Prefix + "task/statusmsg/by/upload_batch_num";
        internal const string Methold_UpdateStatusSucessByBatchNum = Methold_Prefix+ "task/update/process_status/success";
        internal const string Methold_UpdateStatusByBatchNums = Methold_Prefix+ "task/update/by/upload_batch_num";
        internal const string Methold_QueryLogsByBatchCode = Methold_Prefix+ "log/paging";
        internal const string Methold_UpLoadProveFile = Methold_Prefix + "file/upload";
        internal const string Methold_DownLoadProveFile = Methold_Prefix + "file/download"; 
        internal const string Methold_BatchCount = Methold_Prefix + "task/count";
        internal const string Methold_BatchStaus = Methold_Prefix + "task/process_status/by/upload_batch_num";
        internal const string Methold_UpdateDownLoadNumByBatchCodes = Methold_Prefix + "task/update/download_num";
        internal const string Methold_UpdateBatchInfoByBatchCode = Methold_Prefix + "task/update/info";

        //行政地域
        internal const string Methold_children_filter = "/stackcloud/api/open/api/xzdy/children/filter/codes";
        internal const int VectorDecodeSrid = 4490;

        internal const string clientName = "测绘局";
        internal static ClientEum ClientType = ClientEum.UploadRowDataClient;
        internal const string HelpFileName = "空间匹配工具使用说明.pdf";
        internal static string client_id { get; set; }
        internal static List<string> ZonesCodes = new List<string>();
        internal const double SpitialDataAreaLimint = 25000000;

        internal const string BatchCodeShpFileldName = "batchCode";





        internal const int DefualtSrid = 4490;
        internal const string DefualtPrj = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
    }
}
