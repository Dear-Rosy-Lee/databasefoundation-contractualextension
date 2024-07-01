using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 行政区划代码数据字典
    /// </summary>
    public class DivisionCodeDictionary
    {
        public List<XDictionary> Items { get; set; } = new List<XDictionary>();

        /// <summary>
        /// XML反序列化到对象
        /// </summary>
        public void Deserialize()
        {
            if (Items != null && Items.Count > 0) return;
            var current = Serializer.DeserializeFromXmlFile<DivisionCodeDictionary>(Path.Combine(AppGlobalSettings.GetApplicationPath(), @"Config\DivisionCodeDictionary.xml"));
            if (current != null)
            {
                Items = current.Items;
            }
        }

        /// <summary>
        /// 对象序列化到XML
        /// </summary>
        public void Serialize()
        {
            Serializer.SerializeToXmlFile(Path.Combine(AppGlobalSettings.GetApplicationPath(), @"Config\DivisionCodeDictionary.xml"), this);
        }
    }
}
