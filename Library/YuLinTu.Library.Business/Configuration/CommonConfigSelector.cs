/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 通用获取配置列表信息
    /// </summary>
    public class CommonConfigSelector
    {
        /// <summary>
        /// 设置直接显示的字段列表
        /// </summary>
        public static List<string> DisplayValueList
        {
            get;
            set;
        }

        #region methods

        /// <summary>
        /// 获取针对对Excel列号列表
        /// </summary>
        /// <param name="count">可供选择的列数</param>
        /// <returns>返回配置信息列表</returns>
        public static List<KeyValue<int, string>> GetConfigColumnInfo(int count)
        {
            List<KeyValue<int, string>> info = new List<KeyValue<int, string>>();
            KeyValue<int, string> keyValue = new KeyValue<int, string>(-1, "None");
            info.Add(keyValue);
            for (int i = 1; i <= count; i++)
            {
                info.Add(new KeyValue<int, string>((i - 1), LetterToNumber.GetCharacterByNumber(i)));
            }
            return info;
        }

        /// <summary>
        /// 获取自定义插入列表
        /// </summary>
        /// <param name="count">自定义传入列数</param>
        /// <returns>返回配置信息列表</returns>
        public static List<KeyValue<int, string>> GetUserConfigColumnInfo(int count)
        {
            if (DisplayValueList == null) return null;
            List<KeyValue<int, string>> info = new List<KeyValue<int, string>>();
            KeyValue<int, string> keyValue = new KeyValue<int, string>(-1, "None");
            info.Add(keyValue);
            for (int i = 1; i <= count; i++)
            {
                info.Add(new KeyValue<int, string>((i - 1), DisplayValueList[i - 1]));
            }
            return info;
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static CommonConfigSelector GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<CommonConfigSelector>();
            var section = profile.GetSection<CommonConfigSelector>();
            return section.Settings;
        }


        #endregion
    }
}
