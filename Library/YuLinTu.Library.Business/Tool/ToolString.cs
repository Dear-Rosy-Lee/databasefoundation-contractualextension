/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 字符串处理
    /// </summary>
    public class ToolString
    {
        /// <summary>
        /// 清除字符串末尾空格
        /// </summary>
        public static string TrimSafe(string source)
        {
            if (source == null)
                return null;

            return source.Trim();
        }

        /// <summary>
        /// 返回非空字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SetNullOrEmptyToBlank(string source)
        {
            if (string.IsNullOrEmpty(source))
                return " ";

            return source;
        }

        /// <summary>
        /// 去除空字符串
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static string ExceptSpaceString(string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return sourceString;
            }
            return sourceString.Trim().Replace(" ", "");
        }

        /// <summary>
        /// 去除特定标识字符串
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="specialString">标识字符串</param>
        /// <returns></returns>
        public static string ExceptSpecialString(string sourceString, string specialString)
        {
            string[] results = System.Text.RegularExpressions.Regex.Split(sourceString, @"[specialString]+");
            string resultString = string.Empty;//结果字符串
            foreach (string result in results)
            {
                resultString += result;
            }
            return resultString;
        }

        /// <summary>
        /// 获取字符串中最左边的数字字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static string GetLeftNumberWithInString(string value)
        {
            string number = string.Empty;
            foreach (char item in value)
            {
                if (item >= 48 && item <= 58)
                {
                    number += item;
                }
                else
                {
                    break;
                }
            }
            return number;
        }

        /// <summary>
        /// 获取字符串中所有数字字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static string GetAllNumberWithInString(string value)
        {
            string number = string.Empty;
            foreach (char item in value)
            {
                if (item >= 48 && item <= 58)
                {
                    number += item;
                }
            }
            return number;
        }

        /// <summary>
        /// 获取字符串第一个字母为大写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static string GetPascelCase(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 1)
            {
                return value;
            }
            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        /// <summary>
        /// 获取字符串第一个字母为小写字符串
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public static string GetCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 1)
            {
                return value;
            }
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        /// <summary>
        /// 固定长度插入字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="inChar">插入字符</param>
        /// <param name="partLength">指定长度</param>
        /// <returns></returns>
        public static string SplitIn(string source, char inChar, int partLength)
        {
            if (partLength <= 0 || string.IsNullOrEmpty(source))
                return source;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                sb.Append(source[i]);

                if ((i + 1) % partLength == 0 &&
                    (i + 1) != source.Length)
                    sb.Append(inChar);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将byte数组转换为16进制字符串输出
        /// </summary>
        public static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null)
                return null;

            string txt = string.Empty;

            for (int i = 0; i < bytes.Length; i++)
            {
                string part = bytes[i].ToString("X2");
                txt += part;
            }

            return txt;
        }
    }
}
