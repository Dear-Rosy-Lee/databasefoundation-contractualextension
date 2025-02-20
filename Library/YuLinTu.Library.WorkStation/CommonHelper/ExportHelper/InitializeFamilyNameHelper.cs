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

namespace YuLinTu.Library.WorkStation
{
    
    /// <summary>
    /// 处理承包方名称
    /// </summary>
    public static class InitializeFamilyNameHelper
    {        
        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName">承包方名称</param>
        public static string InitalizeFamilyName(this string familyName, bool except = true)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "";
            }
            if(!except)
            {
                return familyName;
            }           
            string number = ToolString.GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }
    }
}
