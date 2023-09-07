using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskRepairSharePersonOperationArgument : TaskArgument
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

        #endregion Properties

        #region Ctor

        public TaskRepairSharePersonOperationArgument()
        {
        }

        #endregion Ctor
    }
}