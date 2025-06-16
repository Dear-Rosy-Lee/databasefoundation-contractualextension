using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal interface IVectorService
    {
        ObservableCollection<VectorDecodeBatchModel> QueryBatchTask(string zoneCode, int page = 1, int pageSize = 2000);
        List<SpaceLandEntity> DownLoadVectorDataPrimevalData(string zoneCode, int pageIndex, int pageSize);
        List<SpaceLandEntity> DownLoadVectorDataAfterDecodelData(string zoneCode, int pageIndex, int pageSize, string batchCode);
        string UpLoadVectorDataAfterDecodeToSever(List<SpaceLandEntity> Data, out bool isSucess);
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
    }
}
