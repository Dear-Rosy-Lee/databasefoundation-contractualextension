using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方类型
    /// </summary>
    public sealed class ContractorTypeConvert : DictionaryConvert
    {
        public static readonly ContractorTypeConvert Instance = new ContractorTypeConvert();//单例模式

        public ContractorTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.CBFLX;
        }

    }
}
