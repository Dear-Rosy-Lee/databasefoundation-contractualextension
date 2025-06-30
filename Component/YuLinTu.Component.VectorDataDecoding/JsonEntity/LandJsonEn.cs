using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.JsonEntity
{
    [Serializable]
    internal class LandJsonEn
    {
        /// <summary>
        /// 
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string upload_batch_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string business_identification { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string original_geometry_data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string desensitized_geometry { get; set; }
        /// <summary>
        /// 张三
        /// </summary>
        public string creator { get; set; }
      
        public object metadata_json { get; set; }

        //public Dictionary<string, object> metadata { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string data_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dybm { get; set; }
        /// <summary>
        /// 第一条测试数据
        /// </summary>
        public string remarks { get; set; }
        public string business_identification_owner { get; set; }
    }

    [Serializable]
    internal class LandJsonEn2
    {
        /// <summary>
        /// 
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string upload_batch_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string business_identification { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string original_geometry_data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string desensitized_geometry { get; set; }
        /// <summary>
        /// 张三
        /// </summary>
        public string creator { get; set; }
        [JsonIgnore]
        public Dictionary<string, object> metadata { get; set; }
        public string metadata_json { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string data_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dybm { get; set; }
        /// <summary>
        /// 第一条测试数据
        /// </summary>
        public string remarks { get; set; }
        public string business_identification_owner { get; set; }



    }
}
