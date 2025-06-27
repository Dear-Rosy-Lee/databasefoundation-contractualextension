using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Data.Dynamic;
using YuLinTu.DF.Zones;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Controls
{
    internal class DataPagerProviderVectorDecode : IDataPagerProvider
    {
        public IZone CurrentZone { get; set; }
        public IVectorService VectorService { get; set; }
        public DataPagerProviderVectorDecode(IZone currentZone, IVectorService vectorService)
        {
            CurrentZone = currentZone;
            VectorService = vectorService;
           // vectorService= new VectorService();
        }

        public int Count()
        {
            return 2;//需修改为调用接口获取
        }

        public PropertyMetadata[] GetAttributeProperties()
        {
            return GetProperties();
           
        }

        public string GetDefaultOrderPropertyName()
        {

            return "BatchCode";
        }

        public PropertyMetadata[] GetProperties()
        {
            return new PropertyMetadata[0];
        }

        public List<object> Paging(int pageIndex, int pageSize, string orderPropertyName, eOrder order)
        {if(CurrentZone==null)return null;
          return VectorService.QueryBatchTask(CurrentZone.FullCode, pageIndex, pageSize).Cast<object>().ToList();   
        }
    }
}
