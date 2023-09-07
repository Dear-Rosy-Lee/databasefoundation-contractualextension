using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 标石类型
    /// </summary>
    public sealed class StoneTypeConvert : DictionaryConvert
    {
        public static readonly StoneTypeConvert Instance = new StoneTypeConvert();//单例模式

        public StoneTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.BSLX;
        }


    }
}
