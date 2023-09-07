using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class ToolConvert
    {
        public static int ToInt32(string value)
        {
            try { return Convert.ToInt32(value); }
            catch { return 0; }
        }

        public static int ToInt32Safe(object value)
        {
            try { return Convert.ToInt32(value); }
            catch { return 0; }
        }

        public static double ToDoubleSafe(object value)
        {
            try { return Convert.ToDouble(value); }
            catch { return 0; }
        }

        public static double? ToDoubleSafe_Nullable(object value)
        {
            try { return Convert.ToDouble(value); }
            catch { return null; }
        }

        public static DateTime ToDateTimeSafe(object value)
        {
            try { return Convert.ToDateTime(value); }
            catch { return DateTime.MinValue; }
        }

        public static DateTime? ToDateTimeSafe_Nullable(object value)
        {
            try { return Convert.ToDateTime(value); }
            catch { return null; }
        }
    }
}
