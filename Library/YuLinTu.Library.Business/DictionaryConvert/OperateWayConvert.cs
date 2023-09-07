using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 经营方式
    /// </summary>
    public sealed class OperateWayConvert:DictionaryConvert
    {
        public static OperateWayConvert Intance=new OperateWayConvert();
        public OperateWayConvert()
        {
            GroupCode = DictionaryTypeInfo.JYFS;
        }
    }
}
