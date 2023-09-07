using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace YuLinTu.Library.Basic
{
    public class LangDisplayNameAttribute : DisplayNameAttribute
    {
        #region Properties

        public bool IsLanguageKey { get; set; }

        public override string DisplayName
        {
            get
            {
                if (IsLanguageKey)
                    return LanguageAttribute.GetLanguage(base.DisplayName);

                return base.DisplayName;
            }
        }

        #endregion

        #region Ctor

        public LangDisplayNameAttribute()
            : base()
        {
        }

        public LangDisplayNameAttribute(string name)
            : base(name)
        {
        }

        #endregion
    }
}
