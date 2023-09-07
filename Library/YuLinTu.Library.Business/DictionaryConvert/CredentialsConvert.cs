using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 证件类型
    /// </summary>
    public sealed class CredentialsConvert : DictionaryConvert
    {
        public static readonly CredentialsConvert Instance = new CredentialsConvert();//单例模式

        public CredentialsConvert()
        {
            GroupCode = DictionaryTypeInfo.ZJLX;
        }
    }
}
