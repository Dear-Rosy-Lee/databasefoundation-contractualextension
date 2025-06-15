using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal interface IVectorService
    {
        ObservableCollection<VectorDecodeBatchModel> QueryBatchTask(string zoneCode, int page = 1, int pageSize = 2000);
        List<SpaceLandEntity> DownLoadVectorDataPrimevalData(string zoneCode, int pageIndex, int pageSize);
        List<SpaceLandEntity> DownLoadVectorDataAfterDecodelData(string zoneCode, int pageIndex, int pageSize, string batchCode);
        string UpLoadVectorDataAfterDecodeToSever(List<SpaceLandEntity> Data, out bool isSucess);
        string UpLoadVectorDataPrimevalToSever(List<SpaceLandEntity> Data, out bool isSucess);
    }
}
