/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 定义了主页
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Components.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;


namespace YuLinTu.Component.ZoneInvestigation
{   
    /// <summary>
    /// 定义了继承tgis map的类
    /// </summary>
    public class YuLinTuMapFoundation:MapPage
    {
        public Zone CurrentZone
        { get; set; }

         public YuLinTuMapFoundation()
        {
            Title = "鱼鳞图权属";
        }
    }
}
