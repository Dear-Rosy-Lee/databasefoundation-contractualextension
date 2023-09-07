/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 新增配置实体
    /// </summary>
    public class DictLeftSidebarStateConfig:CDObject
    {
        #region Properties

        /// <summary>
        /// 侧边栏是否隐藏
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// 侧边栏的宽度
        /// </summary>
        public double Width { get; set; }

        #endregion

        #region Ctro

        /// <summary>
        /// 构造函数—设置默认的侧边栏属性值
        /// </summary>
        public DictLeftSidebarStateConfig()
        {
            IsExpanded = true;
            Width = 300;
        }

        #endregion
    }
}
