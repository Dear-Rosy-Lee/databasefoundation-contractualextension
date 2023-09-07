using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public  class LandPurposeConvert :DictionaryConvert
    {
        public static readonly LandPurposeConvert Instance = new LandPurposeConvert();//单例模式

        public LandPurposeConvert()
        {
            GroupCode = DictionaryTypeInfo.TDYT;
        }
    }
}
