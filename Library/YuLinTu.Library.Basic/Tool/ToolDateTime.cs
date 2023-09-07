using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class ToolDateTime
    {
        /// <summary>
        /// 获取长日期
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetLongDateString(DateTime date)
        {
            return date.Year.ToString() + "年" + date.Month.ToString() + "月" + date.Day.ToString() + "日";//2009年12月17日 
        }

        /// <summary>
        /// 获取全长日期
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetFullDateString(DateTime date)
        {
            return string.Format("{0:yyyy-MM-dd}", date);//2009-12-17
        }

        /// <summary>
        /// 获取短日期
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetShortDateString(DateTime date)
        {
            return string.Format("{0:yyyy-MM-dd}", date);//2009-12-17
        }

        /// <summary>
        /// 获取日期年
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDateYear(DateTime date)
        {
            return date.Year.ToString() + "年";
        }

        /// <summary>
        /// 获取日期月
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDateMonth(DateTime date)
        {
            return date.Day.ToString() + "月";
        }

        /// <summary>
        /// 获取日期日
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDateDay(DateTime date)
        {
            return date.Day.ToString() + "日";
        }

        /// <summary>
        /// 获取日期年日
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static string GetDateWithYearAndMonth(DateTime date)
        {
            return date.Year.ToString() + "." + date.Month.ToString();
        }

        /// <summary>
        /// 计算起始到结束见期限
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static string CalcateTerm(DateTime startTime, DateTime endTime)
        {
            int year = endTime.Year - startTime.Year;//年
            int month = endTime.Month - startTime.Month;//月
            int day = endTime.Day - startTime.Day;//日
            if (day == 30 && month == 11)
            {
                year += 1;
            }
            return year.ToString();
        }
    }
}
