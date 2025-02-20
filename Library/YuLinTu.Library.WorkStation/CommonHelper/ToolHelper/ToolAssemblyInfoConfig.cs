/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class ToolAssemblyInfoConfig
    {
        private static string applicationPath = Application.StartupPath + @"\YuLinTuTool.exe";

        /// <summary>
        /// 判断配置节是否存在
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static bool AppSettingIsExist(Configuration config, string key)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建AppSetting
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <param name="value">AppSetting值</param>
        public static void CreateAppSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
                RefreshAndSaveSection(config);
            }
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetAppSettingValue(Configuration config, string key)
        {
            if (AppSettingIsExist(config, key))
            {
                return config.AppSettings.Settings[key].Value.Trim();
            }
            return null;
        }

        /// <summary>
        /// 获取特殊配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string GetSpecialAppSettingValue(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(applicationPath);
            if (AppSettingIsExist(configuration, key))
            {
                return configuration.AppSettings.Settings[key].Value.Trim();
            }
            else
            {
                CreateAppSetting(configuration, key, value);
            }
            return configuration.AppSettings.Settings[key].Value.Trim();
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetAppSettingValue(Configuration config, string key, string value)
        {
            if (!AppSettingIsExist(config, key))
            {
                return;
            }
            config.AppSettings.Settings[key].Value = value;
            RefreshAndSaveSection(config);
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetAppSettingValue(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(applicationPath);
            if (!AppSettingIsExist(configuration, key))
            {
                return;
            }
            configuration.AppSettings.Settings[key].Value = value;
            RefreshAndSaveSection(configuration);
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetSpecialAppSettingValue(string key, string value)
        {
            GetSpecialAppSettingValue(key, value);//获取特殊值
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(applicationPath);
            SetAppSettingValue(configuration, key, value);//设置配置节值
        }

        /// <summary>
        /// 刷新并保存修改项
        /// </summary>
        /// <param name="config">配置对象</param>
        public static void RefreshAndSaveSection(Configuration config)
        {
            string configPath = config.FilePath;
            FileInfo fileInfo = new FileInfo(configPath);
            fileInfo.Attributes = FileAttributes.Normal;
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// 获取指定connectionStrings配置节值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(applicationPath);
            if (config == null || config.ConnectionStrings.ConnectionStrings.Count == 0)
            {
                return string.Empty;
            }

            if (config.ConnectionStrings.ConnectionStrings[key] == null)
                return string.Empty;

            return config.ConnectionStrings.ConnectionStrings[key].ConnectionString;
        }
    }
}
