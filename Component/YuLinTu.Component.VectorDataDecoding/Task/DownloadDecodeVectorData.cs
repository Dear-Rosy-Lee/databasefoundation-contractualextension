using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownloadDecodeVectorDataArgument),
        Name = "DownloadDecodeVectorData", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownloadDecodeVectorData : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        private IVectorService vectorService { get; set; }
        #endregion

        #region Ctor

        public DownloadDecodeVectorData()
        {
            Name = "下载数据";
            Description = "This is DownloadDecodeVectorData";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
            vectorService = new VectorService();
            var args = Argument as DownloadDecodeVectorDataArgument;
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
