using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [TaskDescriptor(TypeArgument = typeof(SingleTaskArgument),
        Name = "SingleTask", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class SingleTask : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public SingleTask()
        {
            Name = "SingleTask";
            Description = "This is SingleTask";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as SingleTaskArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现

            this.ReportInfomation("开始执行第一项工作...");
            this.ReportProgress(1, "开始执行第一项工作...");
            System.Threading.Thread.Sleep(500);
            this.ReportInfomation("第一项工作执行完成");

            this.ReportInfomation("开始执行第二项工作...");
            this.ReportProgress(30, "开始执行第二项工作...");
            System.Threading.Thread.Sleep(500);

            try
            {
                if (args.Parameter2 < -50)
                    throw new Data.DataException($"值为 {args.Parameter2}，小于 -50，不符合规定");
                else if (args.Parameter2 < 0)
                {
                    this.ReportError($"值为 {args.Parameter2}，为负，不符合规定，任务中止");
                    return;
                }
                else if (args.Parameter2 < 50)
                    this.ReportWarn($"值为 {args.Parameter2}，小于 50，但不影响功能的正常执行");
            }
            catch (Exception ex)
            {
                this.ReportException(ex, $"任务执行过程中出现异常，任务中止，内容为 {ex}");
                return;
            }

            this.ReportInfomation("第二项工作执行完成");

            this.ReportInfomation("开始执行第三项工作...");
            this.ReportProgress(70, "开始执行第三项工作...");
            System.Threading.Thread.Sleep(500);
            this.ReportInfomation("第三项工作执行完成");


            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
