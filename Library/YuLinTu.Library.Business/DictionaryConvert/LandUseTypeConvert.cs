using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 草地利用类型转换器
    /// </summary>
    public  class LandUseTypeConvert : DictionaryConvert
    {
        public static readonly LandUseTypeConvert Instance = new LandUseTypeConvert();//单例模式
        public LandUseTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.TDLYLX;
        }
    }
}
