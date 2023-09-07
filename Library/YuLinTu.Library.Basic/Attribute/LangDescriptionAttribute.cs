using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace YuLinTu.Library.Basic
{
    public class LangDescriptionAttribute : DescriptionAttribute
    {
        #region Properties

        public bool IsLanguageKey { get; set; }

        public override string Description
        {
            get
            {
                if (IsLanguageKey)
                    return LanguageAttribute.GetLanguage(base.Description);

                return base.Description;
            }
        }

        #endregion

        #region Ctor

        public LangDescriptionAttribute()
            : base()
        {
        }

        public LangDescriptionAttribute(string description)
            : base(description)
        {
        }

        #endregion
    }
}
