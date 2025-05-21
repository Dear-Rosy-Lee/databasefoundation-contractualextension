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
    /// Shape表头语言
    /// </summary>
    public class HeaderLanguage
    {
        /// <summary>
        /// 获取语言 CN/EN
        /// </summary>
        public static eLanguage GetLanguage()
        {
            string lang = ToolConfiguration.GetAppSettingValue("HeaderLanguage");
            if (string.IsNullOrEmpty(lang) && !ToolConfiguration.AppSettingIsExist("HeaderLanguage"))
            {
                ToolConfiguration.CreateAppSetting("HeaderLanguage", ((int)eLanguage.CN).ToString());
                return eLanguage.CN;
            }
            int langValue = 0;
            int.TryParse(lang, out langValue);
            return (eLanguage)(langValue > 1 ? 0 : langValue);
        }

        /// <summary>
        /// 设置语言 CN/EN
        /// </summary>
        public static void SetLanguage(eLanguage language)
        {
            ToolConfiguration.CreateAppSetting("HeaderLanguage", ((int)language).ToString());
        }
    }
}
