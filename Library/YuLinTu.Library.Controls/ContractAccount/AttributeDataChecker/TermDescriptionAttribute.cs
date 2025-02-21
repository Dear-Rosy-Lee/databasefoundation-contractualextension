using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Controls
{
    public class TermDescriptionAttribute : Attribute
    {
        #region Fields
        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        #endregion

        #region Property

        /// <summary>
        /// 分类(大类别)
        /// </summary>
        public string Category { get; set; }

        

        /// <summary>
        /// 图片
        /// </summary>
        public string UriImage32 { get; set; }

        

        #endregion

        #region Ctor

        public TermDescriptionAttribute()
        {
            
        }

        public TermDescriptionAttribute(string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        public static AttrrInfo GetAttributeProperty(PropertyInfo info)
        {
            AttrrInfo aif = null;

            TermDescriptionAttribute tcAttr = (TermDescriptionAttribute)
                Attribute.GetCustomAttribute(info, typeof(TermDescriptionAttribute));

            if (tcAttr != null)
            {
                aif = new AttrrInfo()
                {
                    Category = tcAttr.Category,
                    Name = info.Name,
                    Img = tcAttr.UriImage32
                };
            }
            return aif;
        }

        public static List<AttrrInfo> GetAttributeList<T>()
        {
            var dic = new List<AttrrInfo>();
            var prpList = typeof(T).GetProperties();

            if (prpList == null)
                return dic;

            for (int i = 0; i < prpList.Length; i++)
            {
                var p = prpList[i];
                var info = GetAttributeProperty(p);
                if (info != null)
                    dic.Add(info);
            }
            return dic;
        }

        #endregion
    }

    public class AttrrInfo
    {
        /// <summary>
        /// 大分组
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 检查项名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }
    }
}
