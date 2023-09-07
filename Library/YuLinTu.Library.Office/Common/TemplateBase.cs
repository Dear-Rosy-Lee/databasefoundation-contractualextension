using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Office
{
    public class TemplateBase
    {
        /// <summary>
        /// 模板类型：Word或者Excel
        /// </summary>
        public TemplateType TemplateType { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 为构造函数带有参数的类，提供数据
        /// </summary>
        public object[] Tags { get; set; }

        /// <summary>
        /// 提供额外的数据，如湖南工单的地块调查表—传递是否需要特殊处理的bool值进来
        /// </summary>
        public object Tag { get; set; }
    }

    public enum TemplateType
    {
        Word,
        Excel
    }
}
