using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 控制点等级
    /// </summary>
    public sealed class ControlDotLevelConvert : DictionaryConvert
    {
        public static readonly ControlDotLevelConvert Instance = new ControlDotLevelConvert();//单例模式

        public ControlDotLevelConvert()
        {
            GroupCode = DictionaryTypeInfo.KZDLX;
        }


    }
}
