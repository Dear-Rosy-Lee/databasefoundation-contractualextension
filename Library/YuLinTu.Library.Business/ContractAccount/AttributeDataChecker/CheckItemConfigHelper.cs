using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace YuLinTu.Library.Business
{
    public static class CheckItemConfigHelper
    {
        private static string configFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
        private static string configPath = Path.Combine(configFolder, "CheckItemConfig.xml");

        public static void Save(List<CheckItemConfig> configs)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CheckItemConfig>));
            using (var writer = new StreamWriter(configPath))
            {
                serializer.Serialize(writer, configs);
            }
        }

        public static List<CheckItemConfig> Load()
        {
            if (File.Exists(configPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<CheckItemConfig>));
                using (var reader = new StreamReader(configPath))
                {
                    return (List<CheckItemConfig>)serializer.Deserialize(reader);
                }
            }

            var assembly = Assembly.Load("YuLinTu.Library.Business");

            var checkItemList = new List<CheckItemConfig>();

            // 找到带有 [NavigationItem] 的类
            var classes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<NavigationItemAttribute>() != null);

            foreach (var cls in classes)
            {
                var props = cls.GetProperties()
                    .Where(p => p.GetCustomAttribute<CheckItemAttribute>() != null);

                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttribute<CheckItemAttribute>();
                    checkItemList.Add(new CheckItemConfig
                    {
                        TypeFullName = cls.FullName,
                        DisplayName = attr.Name,
                        IsChecked = true
                    });
                }
            }

            // 自动保存初始配置
            Save(checkItemList);

            return checkItemList;
        }
    }
}
