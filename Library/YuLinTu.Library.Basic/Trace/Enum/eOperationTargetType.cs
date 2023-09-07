using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public enum eOperationTargetType
    {
        /// <summary>
        /// 一般对象
        /// </summary>
        [EnumName("key25100", IsLanguageName = true)]
        General = 0,

        /// <summary>
        /// 内存
        /// </summary>
        [EnumName("key25120", IsLanguageName = true)]
        Memory = 20,
        /// <summary>
        /// 文件
        /// </summary>
        [EnumName("key25121", IsLanguageName = true)]
        File = 21,
        /// <summary>
        /// 数据库
        /// </summary>
        [EnumName("key25122", IsLanguageName = true)]
        Database = 22,
        /// <summary>
        /// 网络
        /// </summary>
        [EnumName("key25123", IsLanguageName = true)]
        Network = 23,
        /// <summary>
        /// 权限
        /// </summary>
        [EnumName("key25124", IsLanguageName = true)]
        Security = 24,
        /// <summary>
        /// Win32
        /// </summary>
        [EnumName("key25125", IsLanguageName = true)]
        Win32 = 25,
        /// <summary>
        /// Document
        /// </summary>
        [EnumName("key25126", IsLanguageName = true)]
        Document = 26,

        /// <summary>
        /// 消息
        /// </summary>
        [EnumName("key25150", IsLanguageName = true)]
        Message = 50,

    }
}
