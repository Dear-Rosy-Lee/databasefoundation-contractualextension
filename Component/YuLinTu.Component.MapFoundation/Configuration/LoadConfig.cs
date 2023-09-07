using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.MapFoundation.Configuration
{
    /// <summary>
    /// 装载配置
    /// </summary>
    /// <param name="columnList">读取的shp字段列表进行装载用</param>
    public class LoadConfig<T> where T :NotifyCDObject,new()
    {
        public object GetConfig()
        {
            SettingsProfileCenter systemCenter;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<T>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<T>();
            T config = (section.Settings as T);   //得到经反序列化后的对象
            var configCopy = config.Clone();           
            return configCopy;
        }
        public PropertyInfo[] getPropertyInfo()
        {
            return typeof(T).GetProperties();
        }
        public void SaveConfig(object ImportDataDefine)
        {
            SettingsProfileCenter systemCenter;
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<T>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
            var section = profile.GetSection<T>();
            T config = (section.Settings as T);
            config.CopyPropertiesFrom(ImportDataDefine);
            systemCenter.Save<T>();
        }
    }
}
