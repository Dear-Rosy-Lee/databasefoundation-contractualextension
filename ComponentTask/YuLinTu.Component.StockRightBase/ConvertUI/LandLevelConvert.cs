using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.ConvertUI
{
    public class LandLevelConvert :DictionaryConvert
    {
        public LandLevelConvert()
        {
            GroupCode = DictionaryTypeInfo.DLDJ;
        }
    }
}
