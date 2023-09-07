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
using System.Collections.ObjectModel;
using System.Collections;

namespace YuLinTu.Library.Business
{
    public class TaskExportBoundaryInfoExcelArgument : TaskArgument
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
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 是否预览
        /// </summary>
        public bool IsShow { get; set; }

        #endregion
    }
}
