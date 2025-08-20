using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskAdjustSenderArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        public string NewSenderName { get; set; }
        public string NewSenderCode { get; set; }
        /// <summary>
        /// 变更承包方
        /// </summary>
        public List<VirtualPerson> VirtualPeoples { get; set; }

        #endregion

        #region Ctor
        public TaskAdjustSenderArgument()
        {
                
        }
        #endregion
    }
}
