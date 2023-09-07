/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using Microsoft.Practices.Unity;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方扩展类
    /// </summary>
    public static class VirtualPersonExtend
    {
        public static string NameString(this List<Person> list)
        {
            if (list == null)
                return "";
            string str = string.Empty;
            foreach (var item in list)
            {
                str += item.Name + "、";
            }
            return str.TrimEnd('、');
        }
    }
}
