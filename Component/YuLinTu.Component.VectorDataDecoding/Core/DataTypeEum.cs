using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.DF.Attributes;

namespace YuLinTu.Component.VectorDataDecoding.Core
{

    /// 数据库类型
    /// </summary>
    [Description("数据类型")]
    public enum DataTypeEum
    {
       
        [Description("承包地")]
        [StringValue("DKBM")]
        承包地 = 01,

      
        [Description("宅基地")]
        [StringValue("ZJDDM")]
        宅基地 = 02,

        [Description("行政地域")]
        [StringValue("XZDYDM")]
        行政地域 = 03
    }

    //[Description("是否")]
    //public enum DataTypeEum
    //{

    //    [Description("是")]
       
    //    是 = 01,


    //    [Description("否")]
       
    //    否 = 02,

      
    //}
}
