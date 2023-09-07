using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 性别
    /// </summary>
    public sealed class GenderConvert : DictionaryConvert
    {
        public static readonly GenderConvert Instance = new GenderConvert();//单例模式

        public GenderConvert()
        {
            GroupCode = DictionaryTypeInfo.TDLYLX;
        }



    }
}
