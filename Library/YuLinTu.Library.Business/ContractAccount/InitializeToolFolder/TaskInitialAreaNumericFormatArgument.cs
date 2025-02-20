/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 截取地块面积任务参数
    /// </summary>
    public class TaskInitialAreaNumericFormatArgument : TaskArgument
    {
        #region Properties - 截取承包地块面积小数位

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric { get; set; }

        /// <summary>
        /// 面积截取模式
        /// </summary>
        public int ToAreaModule { get; set; }

        public bool ToAwareArea;   //确权面积
        public bool ToActualArea;//实测面积
        public bool ToTableArea;//二轮合同面积


        /// <summary>
        /// 当前地域下的所有地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialAreaNumericFormatArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}

