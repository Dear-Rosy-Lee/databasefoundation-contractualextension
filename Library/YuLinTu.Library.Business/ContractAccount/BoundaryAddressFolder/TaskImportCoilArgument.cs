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
    /// 导入界址线任务参数
    /// </summary>
    public class TaskImportCoilArgument : TaskArgument
    {
        #region Fields

        #endregion

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
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 当前地域下匹配的界址点数据
        /// </summary>
        public List<BuildLandBoundaryAddressDot> CurrentZoneDotList { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportCoilArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
