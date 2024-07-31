using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Component.StockRightBase.Helper;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.ConvertUI
{
    /// <summary>
    /// 土地利用类型转换器
    /// </summary>
    public class LandUseTypeConvert : DictionaryConvert
    {
        public LandUseTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.TDLYLX;
        }
    }
}
