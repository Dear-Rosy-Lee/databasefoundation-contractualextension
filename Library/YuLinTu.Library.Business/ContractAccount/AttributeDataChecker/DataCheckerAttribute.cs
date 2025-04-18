using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NavigationItemAttribute : Attribute
    {
        public string Title { get; }
        public NavigationItemAttribute(string title)
        {
            Title = title;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CheckItemAttribute : Attribute
    {
        public string Name { get;}
        public string Label { get; }
        public string Description { get; }  // 新增描述信息

        public CheckItemAttribute(string name,string label, string description = "")
        {
            Name = name;
            Label = label;
            Description = description;
        }
    }

}
