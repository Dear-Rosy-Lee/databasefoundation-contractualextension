/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地域数据参数类型
    /// </summary>
    public enum eZoneArgType
    {
        #region Fields
        
        /// <summary>
        /// 导出数据
        /// </summary>
        ExportData = 1,

        /// <summary>
        /// 导出图斑
        /// </summary>
        ExportShape = 2,

        /// <summary>
        /// 导出压缩
        /// </summary>
        ExportPackage = 3,

        /// <summary>
        /// 导入数据
        /// </summary>
        ImportData = 4,

        /// <summary>
        /// 导入图斑
        /// </summary>
        ImportShape = 5,

        #endregion
    }
}