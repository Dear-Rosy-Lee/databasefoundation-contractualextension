using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    [Serializable]
    public class Region
    {
        [XmlAttribute]
        public string Code { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }
    [Serializable]
    public class AllowedRegion
    {
        [XmlArray("Regions")]
        [XmlArrayItem("Region")]
        public List<Region> Regions { get; set; }
    }
}
