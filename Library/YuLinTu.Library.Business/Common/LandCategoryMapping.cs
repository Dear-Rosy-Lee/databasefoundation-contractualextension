/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地块类别映射
    /// </summary>
    public class LandCategoryMapping
    {
        /// <summary>
        /// 地块类别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LandCategoryNameMapping(eLandCategoryType name)
        {
            switch (name)
            {
                case eLandCategoryType.ContractLand:
                    return "10";
                case eLandCategoryType.PrivateLand:
                    return "21";
                case eLandCategoryType.MotorizeLand:
                    return "22";
                case eLandCategoryType.WasteLand:
                    return "23";
                default:
                    return "99";
            }
        }

        /// <summary>
        /// 地块类别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static eLandCategoryType LandCategoryCodeMapping(string code)
        {
            switch (code)
            {
                case "10":
                    return eLandCategoryType.ContractLand;
                case "21":
                    return eLandCategoryType.PrivateLand;
                case "22":
                    return eLandCategoryType.MotorizeLand;
                case "23":
                    return eLandCategoryType.WasteLand;
                default:
                    return eLandCategoryType.CollectiveLand;
            }
        }

        /// <summary>
        /// 映射土地承包方式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LandModeNameMapping(eConstructMode name)
        {
            switch (name)
            {
                case eConstructMode.Family:
                    return "110";
                case eConstructMode.Tenderee:
                    return "121";
                case eConstructMode.Vendue:
                    return "122";
                case eConstructMode.Consensus:
                    return "123";
                case eConstructMode.Transfer:
                    return "200";
                case eConstructMode.Exchange:
                    return "300";
                default:
                    return "900";
            }
        }

        /// <summary>
        /// 映射土地承包方式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static eConstructMode LandModeCodeMapping(string code)
        {
            switch (code)
            {
                case "110":
                    return eConstructMode.Family;
                case "121":
                    return eConstructMode.Tenderee;
                case "122":
                    return eConstructMode.Vendue;
                case "123":
                    return eConstructMode.Consensus;
                case "200":
                    return eConstructMode.Transfer;
                case "300":
                    return eConstructMode.Exchange;
                default:
                    return eConstructMode.Other;
            }
        }

        /// <summary>
        /// 性别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenderNameMapping(eGender gender)
        {
            switch (gender)
            {
                case eGender.Male:
                    return "1";
                case eGender.Female:
                    return "2";
                default:
                    return "0";
            }
        }

        /// <summary>
        /// 性别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static eGender GenderCodeMapping(string code)
        {
            switch (code)
            {
                case "1":
                    return eGender.Male;
                case "2":
                    return eGender.Female;
                default:
                    return eGender.Unknow;
            }
        }

        /// <summary>
        /// 性别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SharePersonCommentNameMapping(string comment)
        {
            if (string.IsNullOrEmpty(ToolString.ExceptSpaceString(comment)))
            {
                return "";
            }
            switch (comment)
            {
                case "外嫁女":
                    return "1";
                case "入赘男":
                    return "2";
                case "在校大学生":
                    return "3";
                case "国家公职人员":
                    return "4";
                case "军人(军官、士兵)":
                    return "5";
                case "新生儿":
                    return "6";
                case "去世":
                    return "7";
                default:
                    return "9";
            }
        }

        /// <summary>
        /// 性别映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SharePersonCommentCodeMapping(string code)
        {
            switch (code)
            {
                case "1":
                    return "外嫁女";
                case "2":
                    return "入赘男";
                case "3":
                    return "在校大学生";
                case "4":
                    return "国家公职人员";
                case "5":
                    return "军人(军官、士兵)";
                case "6":
                    return "新生儿";
                case "7":
                    return "去世";
                case "9":
                    return "其他备注";
                default:
                    return "";
            }
        }
    }
}
