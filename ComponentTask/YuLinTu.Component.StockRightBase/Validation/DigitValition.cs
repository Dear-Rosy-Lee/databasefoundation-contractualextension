using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace YuLinTu.Component.StockRightBase.Validation
{
    /// <summary>
    /// 校验输入数字大于0
    /// </summary>
    class DigitValition : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                if (d > 0)
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "请输入大于0数据");
        }
    }
}
