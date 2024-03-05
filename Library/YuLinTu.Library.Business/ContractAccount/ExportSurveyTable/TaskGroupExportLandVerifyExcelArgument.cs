using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskGroupExportLandVerifyExcelArgument : TaskArgument
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
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 选择文件路径
        /// </summary>
        public string FileName { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportLandVerifyExcelArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion Ctor
    }
}