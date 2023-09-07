/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Component.MapFoundation;
using YuLinTu.Windows;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 打开地图页面
    /// </summary>
    public class MapPageOpen
    {
        /// <summary>
        /// 打开地图页面
        /// </summary>
        public static IWorkpage OpenMapPage(ITheWorkpage workPage)
        {
            if (workPage == null)
                return null;
            var page = workPage.Workspace.Workpages.Find(c => c.Page.Content is YuLinTuMapFoundation);
            if (page == null)
                page = workPage.Workspace.AddWorkpage<YuLinTuMapFoundation>();
            page.Page.Activate();
            return page;
        }
    }
}
