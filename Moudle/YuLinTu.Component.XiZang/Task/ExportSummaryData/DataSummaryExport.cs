/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 合并SQLite数据库任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "导出农村土地承包方汇总数据",
        Gallery = "导出农村土地承包方汇总数据",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DataSummaryExport : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSummaryExport()
        {
            Name = "导出承包方汇总数据";
            Description = "导出承包方汇总数据";
            isCoverDataByZoneLevel = false;
            isBatchCombination = false;
        }

        #endregion

        #region Fields

        private Zone currentZone; //当前地域
        private IDbContext dbContextTarget;  //待合并数据源
        private IDbContext dbContextLocal;  //本地数据源
        private bool isCoverDataByZoneLevel;  //是否覆盖数据
        private bool isBatchCombination;  //是否批量合并数据库
        private double averagePercent;  //平均百分比
        private double currentPercent;  //当前百分比
        private bool isBatch;  //标记是否批量

        #endregion

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {

        }

        #endregion

        #region Method - Private - Validate


        #endregion



        #endregion
    }
}
