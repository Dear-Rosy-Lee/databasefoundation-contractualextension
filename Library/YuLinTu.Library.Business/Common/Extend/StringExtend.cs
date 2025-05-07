using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public static class StringExtend
    {
        private static SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        /// <summary>
        /// 截取字符串后几位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        public static string GetLastString(this string value, int number)
        {
            if (string.IsNullOrEmpty((value)) || value.Count() <= number)
            {
                return value;
            }
            return value.Substring(value.Length - number, number);
        }

        /// <summary>
        /// 面积格式化输出,（不四舍五入且小数后位数没有时补0）,默认两位,默认为0时不用斜杠
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <param name="isSprit"></param>
        /// <returns></returns>
        public static string AreaFormat(this double value, int number = 2, bool isSprit = false)
        {
            var str = value > 0 ? ToolMath.SetNumbericFormat(ToolMath.RoundNumericFormat(value, number).ToString(), number) : AgricultureSetting.InitalizeAreaString(number, isSprit);
            return str;
        }

        /// <summary>
        /// 面积格式化输出,（不四舍五入且小数后位数没有时补0）,默认两位,默认为0时不用斜杠
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number"></param>
        /// <param name="isSprit"></param>
        /// <returns></returns>
        public static string AreaFormat(this double? value, int number = 2, bool isSprit = false)
        {
            if (value == null)
                return AgricultureSetting.InitalizeAreaString(number, isSprit);
            return value.Value.AreaFormat(number, isSprit);
        }

        /// <summary>
        /// 获取当前枚举项的描述, 请在枚举前加EnumNameAttribute
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static string EnumToString(this object sourceValue)
        {
            try
            {
                foreach (var f in sourceValue.GetType().GetFields())
                {
                    if (f.Name != sourceValue.ToString()) continue;
                    if (f.GetAttribute<EnumNameAttribute>() != null)
                        return f.GetAttribute<EnumNameAttribute>().Description;
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("枚举转换失败！" + e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// 通过枚举描述string获得枚举项,使用前请加上EnumNameAttribute特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value) where T : struct
        {
            foreach (var f in typeof(T).GetFields())
            {
                if (f.GetAttribute<EnumNameAttribute>() == null) continue;
                if (f.GetAttribute<EnumNameAttribute>().Description == value)
                    return (T)f.GetValue(null);
            }
            return default(T);
        }

        /// <summary>
        /// 根据设置替换空字符串
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        public static string GetSettingEmptyReplacement(this string srcStr)
        {
            var replacement = SystemSet?.EmptyReplacement;

            return srcStr.IsNullOrEmpty() ? replacement : srcStr;
        }
    }
}