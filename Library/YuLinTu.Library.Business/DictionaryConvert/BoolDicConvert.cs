using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据字典里的bool转换是否
    /// </summary>
    public sealed class BoolConvertDic:DictionaryConvert
    {

        public static readonly BoolConvertDic Instance = new BoolConvertDic();//单例模式

        public BoolConvertDic ()
        {
            GroupCode = DictionaryTypeInfo.SF;
        }


    }
}
