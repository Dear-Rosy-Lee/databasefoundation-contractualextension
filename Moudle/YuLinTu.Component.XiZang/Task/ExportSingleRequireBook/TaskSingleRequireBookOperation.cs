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
using System.Diagnostics;
using System.IO;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出家庭承包单户申请书
    /// </summary>
    public class TaskSingleRequireBookOperation : Task
    {
        #region Fields

        private bool returnValue;
        private string openFilePath;  //打开文件路径

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskSingleRequireBookOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskSingleRequireBookArgument metadata = Argument as TaskSingleRequireBookArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = false;
            openFilePath = metadata.FileName;
            returnValue = ExportSingleExportRequireBook(metadata);
            if (returnValue)
                CanOpenResult = true;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        #endregion

        #region 承包方式下单户申请书

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        private bool ExportSingleExportRequireBook(TaskSingleRequireBookArgument metadata)
        {
            var curZone = metadata.CurrentZone;
            if (curZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return false;
            }
            var exportVps = metadata.SelectContractor;
            if (exportVps == null || exportVps.Count == 0)
            {
                this.ReportProgress(100, null);
                this.ReportWarn("未获取要导出的承包方数据!");
                return false;
            }
            var dbContext = metadata.Database;
            var fileName = metadata.FileName;
            var taskDesc = metadata.TaskDesc;

            ContractConcordBusiness concordBus = new ContractConcordBusiness();
            concordBus.DictList = metadata.DictList;
            concordBus.PublishDateSetting = metadata.PublishDateSetting;

            double percent = 100 / (double)exportVps.Count;
            int index = 1;
            int successCount = 0;
            foreach (var vp in exportVps)
            {
                string warnMsg;
                bool isSuccess = concordBus.PrintRequireBookWord(curZone, vp, dbContext, fileName, out warnMsg);
                this.ReportProgress((int)(percent * index), string.Format("{0}", taskDesc + vp.Name));
                if (!string.IsNullOrEmpty(warnMsg))
                    this.ReportWarn(warnMsg);
                index++;

                if (isSuccess)
                    successCount++;
            }

            this.ReportProgress(100, null);
            string info = successCount == 0 ? string.Format("未导出家庭承包方式单户申请书") : string.Format("共导出{0}家庭承包方式单户申请书", successCount);
            this.ReportInfomation(info);
            return successCount == 0 ? false : true;
        }

        #endregion
    }
}
