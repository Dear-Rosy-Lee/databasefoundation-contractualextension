/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
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
        /// 计算起始到结束间期限
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

        /// <summary>
        /// 计算起始到结束间期限
        /// </summary>
        public static string CalcateTerm(DateTime? startTime, DateTime? endTime)
        {
            if (startTime == null || endTime == null)
            {
                return null;
            }
            int year = endTime.Value.Year - startTime.Value.Year;
            int month = endTime.Value.Month - startTime.Value.Month;
            int day = endTime.Value.Day - startTime.Value.Day;
            if (day == 30 && month == 11)
            {
                year += 1;
            }
            return year.ToString();
        }

        /// <summary>
        /// 根据时间转换为年龄
        /// </summary>
        public static int GetAge(DateTime? time)
        {
            if (time == null || time.Value == null || time.Value.ToString().Length < 11)
            {
                return -1;
            }
            try
            {
                string birthday = time.Value.Year + "-" + time.Value.Month + "-" + time.Value.Day;
                if (birthday == null || birthday.Length < 8)
                    return -1;
                string[] birthdayList = birthday.Split('-');
                if (birthdayList.Length != 3)
                    return -1;

                int year = int.Parse(birthdayList[0]);
                int nowYear = DateTime.Now.Year;

                int month = int.Parse(birthdayList[1]);
                int nowMonth = DateTime.Now.Month;

                int day = int.Parse(birthdayList[2]);
                int nowDay = DateTime.Now.Day;

                if (nowYear >= year)
                {
                    year = nowYear - year;

                    if (month > nowMonth)
                        return (year - 1) < 200 ? (year - 1) : -1;
                    if (month < nowMonth)
                        return year < 200 ? year : -1;
                    if (month == nowMonth)
                    {
                        if (day < nowDay)
                            return year < 200 ? year : -1;
                        if (day > nowDay)
                            return (year - 1) < 200 ? (year - 1) : -1;
                        if (day == nowDay)
                            return year < 200 ? year : -1;
                    }
                }
            }
            catch
            { }
            return -1;
        }
    }
}
