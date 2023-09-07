using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace YuLinTu.Library.Basic
{
    public class LangCategoryAttribute : CategoryAttribute
    {
        #region Properties

        public bool IsLanguageKey { get; set; }

        #endregion

        #region Ctor

        public LangCategoryAttribute()
            : base()
        {
        }

        public LangCategoryAttribute(string category)
            : base(category)
        {
        }

        #endregion

        #region Methods

        protected override string GetLocalizedString(string value)
        {
            if (IsLanguageKey)
                return LanguageAttribute.GetLanguage(base.Category);

            return base.Category;
        }

        #endregion
    }
}
