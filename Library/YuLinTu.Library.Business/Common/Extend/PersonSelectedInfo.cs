/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 选择承包方信息
    /// </summary>
    public class PersonSelectedInfo
    {
        #region Properties

        /// <summary>
        /// 承包方标识
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 承包方姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 承包方状态
        /// </summary>
        public eVirtualPersonStatus Status { get; set; }

        #endregion
    }
}
