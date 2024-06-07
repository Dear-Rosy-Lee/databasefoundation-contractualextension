/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * (C) 2008-2017 鱼鳞图公司版权所有，保留所有权利
 * Website: http://www.yulintu.com
 * Author: James Zhang
 * Date: 2017/9/25 11:41:50
 * Description:
 * CLR Version: 4.0.30319.42000
 * Modifier: 
 * Modify Date: 
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// XML数据字典
    /// </summary>
    public class XDictionary
    {
        [XmlAttribute("ID")]
        public Guid ID { get; set; } = Guid.NewGuid();

        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        public List<XDictionaryItem> Items { get; set; } = new List<XDictionaryItem>();     
    }
}
