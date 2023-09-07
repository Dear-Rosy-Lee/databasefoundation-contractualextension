/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using Microsoft.Practices.Unity;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地域扩展
    /// </summary>
    public class ZoneCodeExtend
    {
        #region 地域

        /// <summary>
        /// 组级编码转换农业部标准编码
        /// </summary>
        public static string ChangeGroupStandCode(string zoneCode)
        {
            string code = zoneCode;
            if (!string.IsNullOrEmpty(zoneCode) && zoneCode.Length == 16)
            {
                code = zoneCode.Substring(0, 12) + zoneCode.Substring(12, 2);
            }
            return code;
        }

        /// <summary>
        /// 组级编码转换农业部标准编码
        /// </summary>
        public static string ChangeToSenderCode(string zoneCode)
        {
            string code = zoneCode;
            if (string.IsNullOrEmpty(zoneCode))
            {
                return code;
            }
            if (zoneCode.Length < 16)
            {
                code = zoneCode.PadRight(14, '0');
            }
            else if (zoneCode.Length == 16)
            {
                code = zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
            }
            return code;
        }


        #endregion
    }
}
