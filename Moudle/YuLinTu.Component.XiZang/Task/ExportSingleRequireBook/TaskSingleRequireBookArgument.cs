/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出家庭承包单户申请书参数
    /// </summary>
    public class TaskSingleRequireBookArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 选中的承包方集合
        /// </summary>
        public List<YuLinTu.Library.Entity.VirtualPerson> SelectContractor { get; set; }

        /// <summary>
        /// 任务地域描述
        /// </summary>
        public string TaskDesc { get; set; }

        #endregion
    }
}
