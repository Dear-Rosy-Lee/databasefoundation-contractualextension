using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 种植类型
    /// </summary>
    public sealed class PlantTypeConvert : DictionaryConvert
    {
        public static readonly PlantTypeConvert Instance = new PlantTypeConvert();//单例模式

        public PlantTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.ZZLX;
        }
    }
}
