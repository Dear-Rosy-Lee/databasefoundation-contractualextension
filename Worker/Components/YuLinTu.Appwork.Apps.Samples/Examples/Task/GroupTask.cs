using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [TaskDescriptor(TypeArgument = typeof(GroupTaskArgument),
        Name = "GroupTask", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class GroupTask : YuLinTu.TaskGroup
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public GroupTask()
        {
            Name = "GroupTask";
            Description = "This is GroupTask";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as GroupTaskArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            // TODO : 任务的逻辑实现

            Clear();

            var rd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < args.Parameter2; i++)
            {
                var task = new SingleTask();
                task.Argument = new SingleTaskArgument()
                {
                    // 使用随机数来模拟用户的输入参数，在任务中将根据参数值的不同执行不同的逻辑
                    Parameter2 = rd.Next(400) - 100,
                };

                Add(task);
            }

            base.OnGo();
        }

        #endregion

        #endregion
    }
}
