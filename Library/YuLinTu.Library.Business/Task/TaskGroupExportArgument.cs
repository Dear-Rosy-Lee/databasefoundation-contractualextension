using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskGroupExportArgument : TaskArgument
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

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 本地域及以下集合
        /// </summary>
        public List<Zone> SelfAndSubsZones { get; set; }

        /// <summary>
        /// 包括镇、村、组地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion
    }
}
