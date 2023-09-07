/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导航地域
    /// </summary>
    public class NavigateZone
    {
        /// <summary>
        /// 获取配置文件根级地域
        /// </summary>
        /// <returns></returns>
        public static string RootZoneCode()
        {
            string rootZone = ToolConfiguration.GetAppSettingValue("DefaultRootZone");
            if (string.IsNullOrEmpty(rootZone) && !ToolConfiguration.AppSettingIsExist("DefaultRootZone"))
            {
                ToolConfiguration.CreateAppSetting("DefaultRootZone", "86");
            }
            return rootZone;
        }

        public static string GetRootZoneCode()
        {
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<CommonBusinessDefine>();
            var section = profile.GetSection<CommonBusinessDefine>();
            var rootZone = section.Settings.CurrentZoneFullCode;
            if (string.IsNullOrEmpty(rootZone))
            {
                ToolConfiguration.CreateAppSetting("DefaultRootZone", "86");
            }
            return rootZone;
        }

    }
}
