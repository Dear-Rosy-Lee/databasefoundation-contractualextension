using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class IcnValidationAttribute : ValidationAttribute
    {
        public IcnValidationAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            if (!(value is string))
                return false;

            var icn = value as string;
            if (icn.TrimSafe().IsNullOrBlank())
                return false;

            try
            {
                return IcnHelper.Check(icn);
            }
            catch
            {
                return false;
            }
        }
    }
}
