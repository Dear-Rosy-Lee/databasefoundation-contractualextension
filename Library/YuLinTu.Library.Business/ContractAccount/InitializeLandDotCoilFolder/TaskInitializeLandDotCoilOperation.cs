/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化界址点线数据操作任务类
    /// </summary>
    public class TaskInitializeLandDotCoilOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitializeLandDotCoilOperation()
        {
        }
        #endregion

        #region Field

        #endregion

        #region Methods

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            var metadata = Argument as TaskInitializeLandDotCoilArgument;
            if (metadata == null)
            {
                return;
            }
            var importDot = new InitializeLandDotCoilTask();

            #region 通过反射等机制定制化具体的业务处理类
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.BaseType != null && t.BaseType.Equals(typeof(InitializeLandDotCoilTask))))
                .ToArray();
            if (types.Length > 0)
            {
                importDot = Activator.CreateInstance(types[0]) as InitializeLandDotCoilTask;
            }

            #endregion
            importDot.ProgressChanged += ReportPercent;
            importDot.Alert += ReportInfo;
            importDot.Argument = metadata;

            //var sw = System.Diagnostics.Stopwatch.StartNew();
            importDot.ContractLandInitialTool();
            //sw.Stop();
            //this.ReportInfomation(string.Format("子任务共耗时：{0}", sw.Elapsed));
        }

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }
        #endregion
    }

    //public class InitializeLandDotCoilSS : InitializeLandDotCoilTask
    //{
    //    public override void EvaluateDotCoils(TaskInitializeLandDotCoilArgument meta, string zoneCode,
    //         IDbContext db, IBuildLandBoundaryAddressCoilWorkStation coilStation)
    //    {
    //        var setArg = meta.SetArg;
    //        if (setArg.InitialJZXInfoUseSN)//使用四至算法更新界址线性质
    //        {
    //            this.ReportProgress(0, "开始更新界址线信息");
    //            InitialJZXInfoUseSN initialJZXInfoUseSN = new InitialJZXInfoUseSN(meta);
    //            initialJZXInfoUseSN.ProgressChanged += ReportPercent;
    //            initialJZXInfoUseSN.Alert += ReportInfo;
    //            initialJZXInfoUseSN.MainHandle();
    //            this.ReportProgress(100, "完成更新界址线信息");
    //            //return;
    //        }
    //    }
    //}
}
