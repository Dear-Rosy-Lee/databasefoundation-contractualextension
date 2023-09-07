using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public enum eLogGrade
    {
        /// <summary>
        /// 消息
        /// </summary>
        [EnumName("key25001", IsLanguageName = true)]
        Infomation = 0,

        /// <summary>
        /// 警告
        /// </summary>
        [EnumName("key25002", IsLanguageName = true)]
        Warn = 1,

        /// <summary>
        /// 错误
        /// </summary>
        [EnumName("key25003", IsLanguageName = true)]
        Error = 2,

        /// <summary>
        /// 异常
        /// </summary>
        [EnumName("key25004", IsLanguageName = true)]
        Exception = 3,
    }
}
