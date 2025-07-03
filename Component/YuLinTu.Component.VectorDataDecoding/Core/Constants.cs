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

        internal const string appidVaule = "8cebb535fc5f40aaa56290faa43d8648";
        internal const string moduleUniqueKey = "9A75153E-E8E3-4219-A3D3-3670CAB204E9";

        internal const string appKeyName = "t-open-api-app-key";
        //以下配置应登录账号后获取，目前先硬编码写死

        internal const string appKeyVaule = "9bdq7mR4BkaRVmU3JKgG38ISz9D+VY7optuGREIIA3gXDgCi9jyaTQ==";

        internal const string Sm4Key = "efd4a17e7c2a89ea5a9d9a283fe03ae1";//"efd4a17e7c2a89ea";//5a9d9a283fe03ae1";
        //internal const int Sm4Key_16 = #efd4a17e7c2a89ea5a9d9a283fe03ae1;

        internal const string Methold_CreateVectorDecTask = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/add";
        internal const string Methold_QueryVectorDecTask = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/paging";
        internal const string Methold_unclassified = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/download/unclassified";
        internal const string Methold_decryption = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/download/decryption";
        internal const string Methold_upload = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/upload";
        internal const string Methold_upload_decryption = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/upload/decryption";
        internal const string Methold_StaticsLand = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/unclassified/statistics/dybm";
        internal const string Methold_UpdateApprovalStatus = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/submit/approval";
        internal const string Methold_UpdateDownNum = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/pull/num";
        internal const string Methold_children_filter = "/stackcloud/api/open/api/xzdy/children/filter/codes";
        internal const string Methold_Update_metadata = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/metadata_json";
        internal const string Methold_Delete_BatchTask = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/delete";
        internal const string Methold_Delete_ClearBatchData = "/stackcloud/api/open/api/dynamic/onlineDecryption/data/delete/by/upload_batch_num";
        //internal const string Methold_Delete_ClearBatchData = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/submit/approval";
        internal const string Methold_UpLoadBatchDataNum = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/data/num";
        public const string CancleBatchDataSendStatus = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/cancel/submit/approval";
        internal const string Methold_WriteLog = "/stackcloud/api/open/api/dynamic/onlineDecryption/log/add";
        internal const string Methold_uploadBatchNums= "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/uploadBatchNums";
        internal const string Methold_QueryStatusByBatchNums = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/process_status/by/upload_batch_num";
        internal const string Methold_UpdateStatusSucessByBatchNum = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/process_status/success";
        internal const string Methold_UpdateStatusByBatchNums = "/stackcloud/api/open/api/dynamic/onlineDecryption/task/update/by/upload_batch_num";
        internal const string Methold_QueryLogsByBatchCode = "/stackcloud/api/open/api/dynamic/onlineDecryption/log/paging";
        internal const string Methold_UpLoadProveFile = "/stackcloud/api/open/api/dynamic/onlineDecryption/file/upload";
        internal const string Methold_DownLoadProveFile = "/stackcloud/api/open/api/dynamic/onlineDecryption/file/download";
        internal const int VectorDecodeSrid = 4490;

        internal const string clientName = "测绘局";
        internal const string HelpFileName = "空间匹配工具使用说明.pdf";
        internal static string client_id { get; set; }
        internal static List<string> ZonesCodes = new List<string>();
        internal static Dictionary<string,string> tempZonesDic = new Dictionary<string, string>();





        internal const int DefualtSrid = 4490;
        internal const string DefualtPrj = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
    }
}
