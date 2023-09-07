using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace YuLinTu.Library.WorkStation
{
    public class WorksheetConfig
    {
        public List<Region> Regions { get; set; } = new List<Region>();
    }

    [Serializable]
    public class Region
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "IsEnabled")]
        public bool IsEnabled { get; set; } = false;

        [XmlAttribute(AttributeName = "AssemblyName")]
        public string AssemblyName { get; set; }

        [XmlAttribute(AttributeName = "Namespace")]
        public string Namespace { get; set; }

        public List<Template> Templates { get; set; } = new List<Template>();
    }

    [Serializable]
    public class Template
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }

        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }

        // 是否需要特别处理，如湖南的承包地块调查表
        [XmlAttribute(AttributeName = "IsSpecial")]
        public bool IsSpecial { get; set; } = false;
    }
}
