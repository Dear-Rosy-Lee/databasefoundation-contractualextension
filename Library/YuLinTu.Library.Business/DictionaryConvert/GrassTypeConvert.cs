using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 草原类型
    /// </summary>
    public sealed class LandTypeConvert : DictionaryConvert
    {
        public static readonly LandTypeConvert Instance = new LandTypeConvert();//单例模式

        public LandTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.DKLB;
        }

    }
}
