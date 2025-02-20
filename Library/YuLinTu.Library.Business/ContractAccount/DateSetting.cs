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
    /// 公示调查表日期设置
    /// </summary>
    public class DateSetting
    {
        #region Properties

        /// <summary>
        /// 公示开始日期
        /// </summary>
        public DateTime? PublishStartDate { get; set; }

        /// <summary>
        /// 公示结束日期
        /// </summary>
        public DateTime? PublishEndDate { get; set; }

        /// <summary>
        /// 制表日期
        /// </summary>
        public DateTime? CreateTableDate { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? CheckTableDate { get; set; }

        /// <summary>
        /// 制表人
        /// </summary>
        public string CreateTablePerson { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckTablePerson { get; set; }

        /// <summary>
        /// 申请单位与地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 盖章单位
        /// </summary>
        public string StampUnit { get; set; }

        #endregion

        #region Ctor

        public DateSetting()
        {
            PublishStartDate = null;
            PublishEndDate = null;
            CreateTableDate = null;
            CheckTableDate = null;
            CreateTablePerson = "";
            CheckTablePerson = "";
            Address = "";
            StampUnit = "";
        }

        #endregion
    }
}
