using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data.Dynamic;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal interface IVectorService
    {
        ObservableCollection<VectorDecodeBatchModel> QueryBatchTask(string zoneCode, int page = 1, int pageSize = 200,string BatchType="all");
        List<SpaceLandEntity> DownLoadVectorDataPrimevalData(string zoneCode, int pageIndex, int pageSize);
        List<SpaceLandEntity> DownLoadVectorDataAfterDecodelData(string zoneCode, int pageIndex, int pageSize, string batchCode);
        string UpLoadVectorDataAfterDecodeToSever(List<SpaceLandEntity> Data, out bool isSucess);
        string UpLoadVectorDataAfterDecodeToSever(List<DecodeLandEntity> Data, out bool isSucess);
        string UpLoadVectorDataPrimevalToSever(List<SpaceLandEntity> Data, out bool isSucess);

        StaticsLandJsonEn StaticsLandByZoneCode(string zoneCode,string batchCode="");

        /// <summary>
        /// 更新送审状态
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        string UpdateUpdateApprovalStatus(string batchCode);
        
        /// <summary>
        /// 更新下载次数
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="batchCode">批次号为空表示该地域所有批次下的数据下载次数+1</param>
        /// <returns></returns>
        string UpdateDownLoadNum(string zoneCode, string type = "1",string batchCode = "");
        /// <summary>
        /// 清空该批次的数据
        /// </summary>
        /// <param name="batchCode"></param>
        string ClearBatchData(string batchCode);
        /// <summary>
        /// 删除批次
        /// </summary>
        /// <param name="batchCode"></param>
        string DeletBatchTask(string batchCode);
        //string UpLoadVectorDataPrimevalToSever(List<LandJsonEn> list, string batchCode,  bool isCover, out bool sucess1);
        string UpLoadVectorDataPrimevalToSever(List<LandJsonEn> list, string batchCode, UploadDataModel model, out bool sucess1);
        string UpLoadVectorMeata(string batchCode, List<PropertyMetadata> properties);
        string ChangeBatchDataStatus(string batchCode,out bool sucess1);
        string UpLoadBatchDataNum(string batchCode);
        string CancleBatchDataSendStatus(string batchCode, out bool sucess);

        void WriteLog(LogEn log);

        string UpdateBatchsStaus(List<string> batchCodes, out bool statusSucess);
        List<FeatureObject> DownVectorDataByBatchCode(string batchCodes,int pageIndex, int pageSize);
        string UpLoadProcessStatusByBatchCodes(List<string> batchCodesStaus, out bool statusSucess);
        string UpdateBatchStatusByBatchCode(string batchCode, out bool statusSucess);
        string UpdateBatchStaus(List<string> codes, string type, out bool statusSucess);
        List<FeatureObject> DownVectorDataAfterDecodeByBatchCode(string batchCode, int pageIndexOneBatchData, int pageSizeOneBatchData);
        ObservableCollection<LogEn> QueryLogsByBatchCode(string batchCode);
        string UpLoadProveFile(ProveFileEn en, out bool sucess);
        List<ProveFileEn> DownLoadProveFile(string zoneCode, int pageIndex = 1, int pageSize = 200);

        List<ZoneJsonEn> GetChildrenByZoneCode(string zoneCode);
        int GetBatchsCountByZoneCode(string zoneCode, ClientEum clientType);
        BatchsStausCode GetBatchStatusByCode(string batchCode);
    }
}
