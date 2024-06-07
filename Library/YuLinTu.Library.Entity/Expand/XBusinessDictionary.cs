/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * (C) 2008-2017 鱼鳞图公司版权所有，保留所有权利
 * Website: http://www.yulintu.com
 * Author: James Zhang
 * Date: 2017/9/25 11:41:40
 * Description:
 * CLR Version: 4.0.30319.42000
 * Modifier: 
 * Modify Date: 
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Collections.Generic;
using System.IO;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// XML业务数据字典
    /// </summary>
    public class XBusinessDictionary
    {
        public List<XDictionary> Items { get; set; } = new List<XDictionary>();

        /// <summary>
        /// XML反序列化到对象
        /// </summary>
        public void Deserialize()
        {
            var current = Serializer.DeserializeFromXmlFile<XBusinessDictionary>(Path.Combine(AppGlobalSettings.GetApplicationPath(), @"Data\Business\Dictionary.xml"));
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
            Serializer.SerializeToXmlFile(Path.Combine(AppGlobalSettings.GetApplicationPath(), @"Data\Business\Dictionary.xml"), this);
        }
    }
}
