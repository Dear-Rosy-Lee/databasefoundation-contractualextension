/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.WorkStation
{
    public static class LandNumericFormatHelperWork
    {
        /// <summary>
        /// 截取地块编码位数
        /// </summary>
        /// <param name="oldNumberList">需要截取地块编码位数的地块集合</param>
        /// <param name="systemset">系统设置</param>
        public static void LandNumberFormat(this List<ContractLand> oldNumberList, SystemSetDefineWork systemset)
        {
            if (!systemset.LandNumericFormatSet || oldNumberList == null)
            {
                return;
            }
            int getlandnumcount = systemset.LandNumericFormatValueSet;
            foreach (var oldNumberItem in oldNumberList)
            {
                if (string.IsNullOrEmpty(oldNumberItem.LandNumber))
                    continue;
                int length = oldNumberItem.LandNumber.Length;
                if (length > getlandnumcount)
                {
                    oldNumberItem.LandNumber = oldNumberItem.LandNumber.Substring(getlandnumcount, length - getlandnumcount);
                }
            }
        }

        /// <summary>
        /// 截取地块编码位数
        /// </summary>
        /// <param name="oldNumberList">需要截取地块编码位数的地块</param>
        /// <param name="systemset">系统设置</param>
        public static void LandNumberFormat(this ContractLand oldLand, SystemSetDefineWork systemset)
        {
            if (!systemset.LandNumericFormatSet || oldLand == null || oldLand.LandNumber == null)
            {
                return;
            }
            int getlandnumcount = systemset.LandNumericFormatValueSet;
            int length = oldLand.LandNumber.Length;
            if (length > getlandnumcount)
            {
                oldLand.LandNumber = oldLand.LandNumber.Substring(getlandnumcount, length - getlandnumcount);
            }
        }

        /// <summary>
        /// 截取地块编码位数
        /// </summary>
        /// <param name="oldNumberList">需要截取地块编码位数的地块编码</param>
        /// <param name="systemset">系统设置</param>
        public static void SimpleLandNumberFormat(this string oldNumber, SystemSetDefineWork systemset)
        {
            if (!systemset.LandNumericFormatSet)
            {
                return;
            }
            int getlandnumcount = systemset.LandNumericFormatValueSet;

            if (string.IsNullOrEmpty(oldNumber))
                return;
            int length = oldNumber.Length;
            if (length > getlandnumcount)
            {
                oldNumber = oldNumber.Substring(getlandnumcount, length - getlandnumcount);
            }
        }

        /// <summary>
        /// 返回截取地块编码位数
        /// </summary>
        /// <param name="oldNumberList">需要截取地块编码位数的地块编码</param>
        /// <param name="systemset">系统设置</param>
        public static string SimpleLandNumberFormatValue(this string oldNumber, SystemSetDefineWork systemset)
        {
            if (!systemset.LandNumericFormatSet)
            {
                return oldNumber;
            }
            int getlandnumcount = systemset.LandNumericFormatValueSet;

            if (string.IsNullOrEmpty(oldNumber))
                return oldNumber;
            int length = oldNumber.Length;
            if (length > getlandnumcount)
            {
                oldNumber = oldNumber.Substring(getlandnumcount, length - getlandnumcount);
            }
            return oldNumber;
        }

    }
}
