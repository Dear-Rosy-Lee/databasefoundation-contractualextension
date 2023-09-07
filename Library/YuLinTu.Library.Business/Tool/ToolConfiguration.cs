/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 配置工具
    /// </summary>
    public class ToolConfiguration
    {
        /// <summary>
        /// 连接字符串是否存在
        /// </summary>
        /// <param name="list">连接列表</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static bool HasConnectionStringName(ConnectionStringSettingsCollection list, string name)
        {
            foreach (ConnectionStringSettings settings in list)
                if (settings.Name == name)
                    return true;

            return false;
        }

        /// <summary>
        /// 根据名称获取连接字符串
        /// </summary>
        /// <param name="list">连接列表</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionStringSettingsByName(ConnectionStringSettingsCollection list, string name)
        {
            foreach (ConnectionStringSettings settings in list)
                if (settings.Name == name)
                    return settings;

            return null;
        }

        /// <summary>
        /// 获取当前配置文件
        /// </summary>
        /// <returns></returns>
        public static Configuration GetCurrentAssemblyConfiguration()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        /// <summary>
        /// 刷新连接字符串
        /// </summary>
        public static void RefreshSectionConnectionString()
        {
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// 获取连接字符串列表
        /// </summary>
        /// <returns></returns>
        public static List<ConnectionStringSettings> GetConnectionStringSettingsList()
        {
            List<ConnectionStringSettings> list = new List<ConnectionStringSettings>();

            foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
                list.Add(settings);

            return list;
        }

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
        /// 判断配置节是否存在
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static bool AppSettingIsExist(string key)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
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
            if (config.AppSettings.Settings[key] != null)
            {
                return;
            }
            config.AppSettings.Settings.Add(key, value);
            RefreshAndSaveSection(config);
        }

        /// <summary>
        /// 创建AppSetting
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">AppSetting值</param>
        public static void CreateAppSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
            {
                return;
            }
            configuration.AppSettings.Settings.Add(key, value);
            RefreshAndSaveSection(configuration);
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetAppSettingValue(Configuration config, string key)
        {
            bool isExist = AppSettingIsExist(config, key);
            if (!isExist)
            {
                return "";
            }
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element == null)
            {
                return "";
            }
            string val = element.Value;
            if (string.IsNullOrEmpty(val))
            {
                return "";
            }
            val = val.Trim();
            if (val == ",")
            {
                return "";
            }
            return val;
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetAppSettingValue(string key)
        {
            bool isExist = AppSettingIsExist(key);
            if (!isExist)
            {
                return "";
            }
            string val = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(val))
            {
                return "";
            }
            val = val.Trim();
            if (val == ",")
            {
                return "";
            }
            return val;
        }

        /// <summary>
        /// 获取特殊配置节值
        /// </summary>
        /// <param name="configuration">配置对象</param>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string GetSpecialAppSettingValue(Configuration configuration, string key, string value)
        {
            bool isExist = AppSettingIsExist(configuration, key);
            if (isExist)
            {
                return GetAppSettingValue(configuration, key);
            }
            CreateAppSetting(configuration, key, value);
            return value;
        }

        /// <summary>
        /// 获取特殊配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string GetSpecialAppSettingValue(string key, string value)
        {
            bool isExist = AppSettingIsExist(key);
            if (isExist)
            {
                return GetAppSettingValue(key);
            }
            CreateAppSetting(key, value);
            return value;
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
            bool isExist = AppSettingIsExist(config, key);
            if (!isExist)
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
            bool isExist = AppSettingIsExist(key);
            if (!isExist)
            {
                return;
            }
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            RefreshAndSaveSection(configuration);
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetSpecialAppSettingValue(Configuration config, string key, string value)
        {
            bool isExist = AppSettingIsExist(config, key);
            if (!isExist)
            {
                CreateAppSetting(config, key, value);
                return;
            }
            SetAppSettingValue(config, key, value);//设置配置节值
        }

        /// <summary>
        /// 获取配置节值
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetSpecialAppSettingValue(string key, string value)
        {
            bool isExist = AppSettingIsExist(key);
            if (!isExist)
            {
                CreateAppSetting(key, value);
                return;
            }
            SetAppSettingValue(key, value);//设置配置节值
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
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 获取指定connectionStrings配置节值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnectionValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
