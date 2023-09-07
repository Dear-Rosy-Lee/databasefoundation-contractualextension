using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 界址线位置 D代表地理
    /// </summary>
    public sealed class DLocationConvert :DictionaryConvert
    {
        public static readonly DLocationConvert Instance = new DLocationConvert();//单例模式

        public DLocationConvert()
        {
            GroupCode = DictionaryTypeInfo.JZXWZ;
        }
    }
}
