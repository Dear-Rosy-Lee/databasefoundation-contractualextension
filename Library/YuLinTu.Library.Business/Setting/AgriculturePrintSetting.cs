/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 配置承包证数据
    /// </summary>
    public class AgriculturePrintSetting
    {
        #region 打印年号设置
        
        public const string BOOKNUMBERPRINTMEDIAN = "BookNumberPrintMedian";//编号打印位数
        
        /// <summary>
        /// 打印编号位数
        /// </summary>
        public static int BookNumberPrintMedian
        {
            get
            {
                bool config = AgricultureSetting.CheckConfiguration();
                int median = 0;
                string value = string.Empty;
                if (config)
                {
                    if (ToolConfiguration.AppSettingIsExist(BOOKNUMBERPRINTMEDIAN))
                    {
                        value = ToolConfiguration.GetAppSettingValue(BOOKNUMBERPRINTMEDIAN);
                    }
                }
                else
                {
                    value = ToolAssemblyInfoConfig.GetSpecialAppSettingValue(BOOKNUMBERPRINTMEDIAN, "0");
                }
                Int32.TryParse(value, out median);
                return median;
            }
        }

        #endregion

    }
}
