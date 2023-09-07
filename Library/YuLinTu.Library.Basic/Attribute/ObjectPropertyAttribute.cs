using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YuLinTu.Library.Basic
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ObjectPropertyAttribute : Attribute
    {
        #region Properties

        public string Gallery
        {
            get
            {
                if (IsGalleryLanguageKey)
                    return LanguageAttribute.GetLanguage(gallery);
                return gallery;
            }
            set { gallery = value; }
        }

        public bool IsGalleryLanguageKey { get; set; }

        public string Name
        {
            get
            {
                if (IsNameLanguageKey)
                    return LanguageAttribute.GetLanguage(name);
                return name;
            }
            set { name = value; }
        }

        public bool IsNameLanguageKey { get; set; }

        public object Value { get; set; }

        public Image Image { get; set; }

        #endregion

        #region Fields

        private string name;
        private string gallery;

        #endregion

        #region Ctor

        public ObjectPropertyAttribute()
        {

        }

        #endregion
    }

    public class ObjectProperties : List<ObjectPropertyAttribute>
    {
    }
}
