/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据交换实体统计
    /// </summary>
    public class ExhangeCount
    {
        #region Propertys

        /// <summary>
        /// 户总数
        /// </summary>
        public int FamilyCount { get; set; }

        /// <summary>
        /// 人总数
        /// </summary>
        public int PersonCount { get; set; }

        /// <summary>
        /// 地总数
        /// </summary>
        public int LandCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            FamilyCount = 0;
            PersonCount = 0;
            LandCount = 0;
        }

        #endregion
    }
}
