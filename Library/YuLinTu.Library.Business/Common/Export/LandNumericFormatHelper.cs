/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;
using System.Collections.ObjectModel;
using YuLinTu.Spatial;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data.Dynamic;
using System.Collections;
using System.Reflection;
using YuLinTu.Diagrams;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public static class LandNumericFormatHelper
    {
        /// <summary>
        /// 截取地块编码位数
        /// </summary>
        /// <param name="oldNumberList">需要截取地块编码位数的地块集合</param>
        /// <param name="systemset">系统设置</param>
        public static void LandNumberFormat(this List<ContractLand> oldNumberList, SystemSetDefine systemset)
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
        public static void LandNumberFormat(this ContractLand oldLand, SystemSetDefine systemset)
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
        public static void SimpleLandNumberFormat(this string oldNumber, SystemSetDefine systemset)
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
        public static string SimpleLandNumberFormatValue(this string oldNumber, SystemSetDefine systemset)
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
