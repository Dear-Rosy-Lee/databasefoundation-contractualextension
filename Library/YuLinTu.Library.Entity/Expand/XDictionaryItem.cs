/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * (C) 2008-2017 鱼鳞图公司版权所有，保留所有权利
 * Website: http://www.yulintu.com
 * Author: James Zhang
 * Date: 2017/9/25 11:42:03
 * Description:
 * CLR Version: 4.0.30319.42000
 * Modifier: 
 * Modify Date: 
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Xml.Serialization;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// XML数据字典项
    /// </summary>
    public class XDictionaryItem
    {
        [XmlElement("Code")]
        public string Code { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("IsCustom")]
        public bool IsCustom { get; set; }

        [XmlElement("CustomName")]
        public string CustomName { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
    }
}
