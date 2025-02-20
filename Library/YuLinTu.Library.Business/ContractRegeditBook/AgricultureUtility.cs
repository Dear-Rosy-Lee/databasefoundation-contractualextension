/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    public class AgricultureUtility
    {
        /// <summary>
        /// 截取乡镇名称前缀
        /// </summary>
        /// <param name="townName"></param>
        /// <returns></returns>
        public static string ChockTownForwardName(string townName)
        {
            if (string.IsNullOrEmpty(townName))
            {
                return "";
            }
            int index = townName.IndexOf("乡");
            if (index == townName.Length - 1)
            {
                return townName.Substring(0, index);
            }
            index = townName.IndexOf("镇");
            if (index == townName.Length - 1)
            {
                return townName.Substring(0, index);
            }
            //index = townName.IndexOf("街道办事处");
            //if (index == townName.Length - 1)
            //{
            //    return townName.Substring(0, index);
            //}
            return townName;
        }

        /// <summary>
        /// 截取村名称前缀
        /// </summary>
        /// <param name="townName"></param>
        /// <returns></returns>
        public static string ChockVillageForwardName(string villageName)
        {
            if (string.IsNullOrEmpty(villageName))
            {
                return "";
            }
            int index = villageName.IndexOf("村");
            if (index == villageName.Length - 1)
            {
                return villageName.Substring(0, index);
            }
            //index = villageName.IndexOf("社区");
            //if (index == villageName.Length - 1)
            //{
            //    return villageName.Substring(0, index);
            //}
            return villageName;
        }

        /// <summary>
        /// 截取组名称前缀
        /// </summary>
        /// <param name="townName"></param>
        /// <returns></returns>
        public static string ChockGroupForwardName(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return "";
            }
            int index = groupName.IndexOf("组");
            if (index == groupName.Length - 1)
            {
                return groupName.Substring(0, index);
            }
            index = groupName.IndexOf("社");
            if (index == groupName.Length - 1)
            {
                return groupName.Substring(0, index);
            }
            index = groupName.IndexOf("队");
            if (index == groupName.Length - 1)
            {
                return groupName.Substring(0, index);
            }
            index = groupName.IndexOf("屯");
            if (index == groupName.Length - 1)
            {
                return groupName.Substring(0, index);
            }
            return groupName;
        }

        /// <summary>
        /// 检查值是否有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CheckValueIsValide(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "     ";
            }
            return value;
        }

    }
}
