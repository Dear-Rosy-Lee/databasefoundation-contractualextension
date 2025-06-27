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
        string UpLoadVectorDataAfterDecodeToSever(string BatchCode,List<DecodeLandEntity> Data, out bool isSucess);
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
        string UpLoadVectorDataPrimevalToSever(List<LandJsonEn> list, string batchCode,  bool isCover, out bool sucess1);
        string UpLoadVectorMeata(string batchCode, List<PropertyMetadata> properties);
        string ChangeBatchDataStatus(string batchCode,out bool sucess1);
        string UpLoadBatchDataNum(string batchCode);
        string CancleBatchDataSendStatus(string batchCode, out bool sucess);

        void WriteLog(LogEn log);
  
        void UpdateBatchsStaus(List<string> batchCodes);
        List<FeatureObject> DownVectorDataByBatchCode(string batchCodes,int pageIndex, int pageSize);
    }
}
