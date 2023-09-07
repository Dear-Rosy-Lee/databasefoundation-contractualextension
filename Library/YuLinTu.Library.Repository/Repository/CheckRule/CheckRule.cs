// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 用于对传入参数类型为String、GUID的检查及有限的修改
    /// </summary>
    public class CheckRule
    {
        /// <summary>
        /// 检查目标字符串是否为null、空字符串（或仅有空格字符）;如果目标字符串含有空格字符，将被去除。
        /// </summary>
        /// <param name="target">待检测字符串</param>
        /// <returns>true(为null、空（empty）/false(不为null、空（empty）))</returns>
        public static bool CheckStringNullOrEmpty(ref string target) 
        {
            if (string.IsNullOrEmpty(target))
                return false;
            if (string.IsNullOrEmpty(target.Trim()))
                return false;
            return true;
        }

        /// <summary>
        /// 检查目标GUID是否为null、空（empty）类型;
        /// </summary>
        /// <param name="target">待检测字符串</param>
        /// <returns>true(为null、空（empty）/false(不为null、空（empty）))</returns>
        public static bool CheckGuidNullOrEmpty(Guid target)
        {
            if (target==null)
                return false;
            if (target == Guid.Empty)
                return false;
            return true;
        }

    }
}
