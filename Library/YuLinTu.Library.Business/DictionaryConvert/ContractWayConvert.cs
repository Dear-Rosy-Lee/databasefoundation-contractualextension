using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方式
    /// </summary>
    public sealed class ContractWayConvert : DictionaryConvert
    {
        public static readonly ContractWayConvert Instance = new ContractWayConvert();//单例模式

        public ContractWayConvert()
        {
            GroupCode = DictionaryTypeInfo.CBJYQQDFS;
        }


    }
}
