using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 界标类型 D代表地理
    /// </summary>
    public sealed class DGuidepostTypeConvert :DictionaryConvert
    {
        public static readonly DGuidepostTypeConvert Instance = new DGuidepostTypeConvert();//单例模式
        public DGuidepostTypeConvert()
        {
            GroupCode = DictionaryTypeInfo.JBLX;
        }
    }
}
