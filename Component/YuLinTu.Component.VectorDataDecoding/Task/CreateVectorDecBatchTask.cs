using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(CreateVectorDecBatchTaskArgument),
        Name = "CreateVectorDecBatchTask", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CreateVectorDecBatchTask : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public CreateVectorDecBatchTask()
        {
            Name = "CreateVectorDecBatchTask";
            Description = "This is CreateVectorDecBatchTask";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as CreateVectorDecBatchTaskArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现


            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion

        #endregion
    }
}
