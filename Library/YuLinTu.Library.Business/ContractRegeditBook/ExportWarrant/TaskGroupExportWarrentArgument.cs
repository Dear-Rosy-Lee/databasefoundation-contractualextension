/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 批量证书任务参数
    /// </summary>
    public class TaskGroupExportWarrentArgument : TaskArgument
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
        /// 文件夹路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine { get; set; }

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum { get; set; }

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum { get; set; }


        /// <summary>
        /// 证书编码设置-证书编码样式设置
        /// </summary>
        public string BookNumSetting { get; set; }

        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ExtendUseExcelDefine { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportWarrentArgument()
        {
        }

        #endregion
    }
}
