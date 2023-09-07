/*
 * (C) 2014 鱼鳞图公司版权所有，保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 业务记录扩展
    /// </summary>
    public class AgricultureAffairExpand
    {
        #region Propertys

        /// <summary>
        /// 户唯一标识
        /// </summary>
        public Guid FamilyId { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public eTaskType TaskType { get; set; }

        /// <summary>
        /// 源ID
        /// </summary>
        public Guid? SourceId { get; set; }

        #endregion

        #region Ctor

        public AgricultureAffairExpand()
        {
        }

        #endregion
    }
}
