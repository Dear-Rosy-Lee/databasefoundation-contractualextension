/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量截取地块面积任务参数
    /// </summary>
    public class TaskGroupInitialAreaNumericFormatArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 全部子级地域(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }
        public bool ToAwareArea;   //确权面积
        public bool ToActualArea;//实测面积
        public bool ToTableArea;//二轮合同面积

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric { get; set; }

        /// <summary>
        /// 面积截取模式
        /// </summary>
        public int ToAreaModule { get; set; }

        /// <summary>
        /// 面积种类选择
        /// </summary>
        public int ToAreaSelect { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialAreaNumericFormatArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
