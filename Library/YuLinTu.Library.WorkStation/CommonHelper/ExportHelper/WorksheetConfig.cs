using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using YuLinTu.Library.Office;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.WorkStation
{
    public static class WorksheetConfigHelper
    {
        private static WorksheetConfig _worksheet;
        public static TemplateBase GetInstance(TemplateBase target)
        {
            var configPath = Path.Combine(TheApp.GetApplicationPath(), @"Template\WorksheetConfig.xml");
            if (!File.Exists(configPath))
                return null;
            TemplateBase template = null;
            try
            {
                if (_worksheet == null)
                {
                    _worksheet = Serializer.DeserializeFromXmlFile<WorksheetConfig>(Path.Combine(TheApp.GetApplicationPath(), @"Template\WorksheetConfig.xml"));
                }

                // 必须是Enabled的Region,并且Module、Template的名称和Type完全匹配
                var region = _worksheet.Regions.Find(r => r.IsEnabled);
                Template temp = region.Templates.Find(t => t.Name.Equals(target.TemplateName) && t.Type.Equals(target.TemplateType.ToString()));
                    
                if (temp.ClassName != null)
                {
                    var assembly = Assembly.LoadFrom(Path.Combine(TheApp.GetApplicationPath(), region.AssemblyName));
                    if (target.Tags != null && target.Tags.Length > 0)
                    {
                        // 基类的构造函数带有参数，需要传递参数                 
                        template = (TemplateBase)assembly.CreateInstance(region.Namespace + "." + temp.ClassName, true, BindingFlags.Default, null, target.Tags, null, null);
                    }
                    else
                    {
                        template = (TemplateBase)assembly.CreateInstance(region.Namespace + "." + temp.ClassName);
                    }
                }
                else
                {
                    // 只需要更改模板的
                    template = new TemplateBase();
                }

                // 需要特殊处理
                if (temp.IsSpecial)
                    template.Tag = true;
                else
                    template.Tag = false;
                template.TemplatePath = temp.Path;
            }
            catch (Exception e)
            {
                YuLinTu.Library.Log.Log.WriteException("", "配置文件未配置完整!", e.Message + e.StackTrace);
            }
            return template;
        }
    }
}
