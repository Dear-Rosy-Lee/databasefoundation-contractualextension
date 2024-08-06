using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Component.StockRightShuShan.Helper;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.ConvertUI
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
