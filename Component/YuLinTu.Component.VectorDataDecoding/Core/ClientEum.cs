using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.DF.Attributes;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    public enum ClientEum
    {
        UploadRowDataClient = 0,
        UploaDeclassifyDataClient=1

    }

    internal enum BatchsStausCode
    {
        [StringValue("未送审")]
        未送审 =0,
        [StringValue("已送审")]
        已送审 =1,
        [StringValue("待处理")]
        待处理 =2,
        [StringValue("处理中")]
        处理中 = 3,
        [StringValue("处理完成")]
        处理完成 = 4,
        [StringValue("处理失败")]
        处理失败 = 5,
    }

    public enum UploadDataModel
    {
        [EnumName("追加模式，不允许重复上传数据")]
        [StringValue("1")]
        追加上传 = 1,


        
        [EnumName("覆盖模式,数据已存在时更新，不存在则新增")]
        [StringValue("2")]
        覆盖上传 =2,

    }
  

}
