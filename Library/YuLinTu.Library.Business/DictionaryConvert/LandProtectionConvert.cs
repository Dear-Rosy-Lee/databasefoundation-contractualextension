using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 耕保类型
    /// </summary>
    public sealed class LandProtectionConvert : DictionaryConvert
    {
        public static readonly LandProtectionConvert Instance = new LandProtectionConvert();//单例模式


        public LandProtectionConvert()
        {
            GroupCode = DictionaryTypeInfo.GBZL;
        }

    }
}
