using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public enum eOperateType
    {
        /// <summary>
        /// 其他
        /// </summary>
        [EnumName("key25204", IsLanguageName = true)]
        General = -1,

        /// <summary>
        /// 添加
        /// </summary>
        [EnumName("key25200", IsLanguageName = true)]
        Add = 0,

        /// <summary>
        /// 删除
        /// </summary>
        [EnumName("key25201", IsLanguageName = true)]
        Delete = 1,

        /// <summary>
        /// 更新
        /// </summary>
        [EnumName("key25202", IsLanguageName = true)]
        Update = 2,

        /// <summary>
        /// 获取
        /// </summary>
        [EnumName("key25203", IsLanguageName = true)]
        Get = 3,

        /// <summary>
        /// 创建
        /// </summary>
        [EnumName("key25210", IsLanguageName = true)]
        Create = 10,

        /// <summary>
        /// 验证
        /// </summary>
        [EnumName("key25220", IsLanguageName = true)]
        Verify = 20,

        /// <summary>
        /// 访问
        /// </summary>
        [EnumName("key25225", IsLanguageName = true)]
        Access = 25,

        //Open = 30,
        //Close = 31,
        //BeginTransaction = 32,
        //Commit = 33,
        //Rollback = 34,
    }
}
