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
    /// 导入地块调查表任务参数
    /// </summary>
    public class TaskImportLandTableArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 区分确权确股
        /// </summary>
        public bool IsNotland { get; set; }


        public bool IsCheckLandNumberRepeat { get; set; }

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

        /// <summary>
        /// 导出方式
        /// </summary>
        public eImportTypes ImportType { get; set; }

        public SystemSetDefine SystemSet
        {
            get; set;
        }

        public ContractBusinessSettingDefine SettingDefine
        {
            get;
            set;
        }

        public ContractBusinessImportSurveyDefine ContractLandImportSurveyDefine
        { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportLandTableArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
