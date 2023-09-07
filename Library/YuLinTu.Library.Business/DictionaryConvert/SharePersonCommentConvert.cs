using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 共有成员备注
    /// </summary>
    public sealed class SharePersonCommentConvert : DictionaryConvert
    {
        public static readonly SharePersonCommentConvert Instance = new SharePersonCommentConvert();//单例模式

        public SharePersonCommentConvert()
        {
            GroupCode = DictionaryTypeInfo.CYBZ;
        }

    }
}
