/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 定义了主页
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Components.Diagrams;
using YuLinTu.Components.tGIS;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Components.Visio;
using YuLinTu.Appwork;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// 定义了继承tgis map的类
    /// </summary>
    [Newable(false)]
    public class YuLinTuDiagramFoundation : VisioPage
    {
        #region Fields

        #endregion

        #region Properties


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
        public YuLinTuDiagramFoundation()
        {
            Title = "制图";
            Description = "鱼鳞图制图工具";
            Category = "应用";
            CanAutoOpen = false;
            SingleInstance = false;
        }

        #endregion
    }
}
