using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 所有权性质
    /// </summary>
    public sealed class OwnershipConvert : DictionaryConvert
    {
        public static readonly OwnershipConvert Instance = new OwnershipConvert();//单例模式

        public OwnershipConvert()
        {
            GroupCode = DictionaryTypeInfo.SYQXZ;
        }


    }
}
