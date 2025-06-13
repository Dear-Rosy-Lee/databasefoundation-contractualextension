using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.JsonEntity
{
    internal class BatchTaskJsonEn
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dybm { get; set; }
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
        public string upload_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadata_json { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string process_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pull_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_desensitized { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status_batch_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int download_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string data_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string error_msg { get; set; }
        /// <summary>
        /// 测试数据上传
        /// </summary>
        public string remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cretime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updtime { get; set; }

        public int data_count { get; set; }
    }
}
