/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 定义了主页
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Components.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;


namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 定义了继承tgis map的类
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = true,
        Name = "lang3070000",
        Description = "lang3070001",
        Category = "lang应用",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/地图78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf041",
        IsNeedAuthenticated = true)]
    public class YuLinTuMapFoundation : MapPage
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 是否需要授权
        /// </summary>
        public override bool IsNeedAuthenticated
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Ctro

        /// <summary>
        /// 构造函数
        /// </summary>
        public YuLinTuMapFoundation()
        {
            Title = "鱼鳞图";
            Category = "应用";
            SingleInstance = true;
            NavigatorType = Windows.Wpf.Metro.Components.eNavigatorType.TreeView;
        }

        #endregion
    }
}
