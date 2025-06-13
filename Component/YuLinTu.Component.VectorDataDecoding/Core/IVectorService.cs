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
        List<SpaceLandEntity> DownLoadRawData(string zoneCode, int pageIndex, int pageSize);
    }
}
