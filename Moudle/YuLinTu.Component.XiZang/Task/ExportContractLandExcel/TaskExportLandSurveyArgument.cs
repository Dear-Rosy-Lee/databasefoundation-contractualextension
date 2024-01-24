/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出承包地块调查表任务参数
    /// </summary>
    public class TaskExportLandSurveyArgument : TaskArgument
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
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 选择文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 待导出调查表的承包方集合
        /// </summary>
        public List<YuLinTu.Library.Entity.VirtualPerson> SelectedPersons { get; set; }
     
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportLandSurveyArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
