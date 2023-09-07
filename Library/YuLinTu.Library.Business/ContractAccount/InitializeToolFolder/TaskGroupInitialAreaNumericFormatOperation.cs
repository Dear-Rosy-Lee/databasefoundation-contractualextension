/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量截取地块面积任务任务类
    /// </summary>
    public class TaskGroupInitialAreaNumericFormatOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialAreaNumericFormatOperation()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        /// <summary>
        /// 初始化图幅编号的地块集合
        /// </summary>
        public List<ContractLand> InitialLands { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupInitialAreaNumericFormatArgument groupArgument = Argument as TaskGroupInitialAreaNumericFormatArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> allZones = groupArgument.AllZones;
            try
            {
                var landStation = dbContext.CreateContractLandWorkstation();
                InitialLands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                if (InitialLands == null)
                    InitialLands = new List<ContractLand>();
                //if (InitialLands.Count == 0)
                //{
                //    ShowBox("截取地块面积", "未获取地块信息,无法执行初始化操作!");
                //    return;
                //}
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包地块数据失败)", ex.Message + ex.StackTrace);
                ShowBox("截取地块面积", string.Format("获取{0}下的承包地块数据失败", currentZone.FullName));
                return;
            }
            List<ContractLand> currentLands = new List<ContractLand>();
            foreach (var zone in allZones)
            {
                currentLands = InitialLands.FindAll(c => c.LocationCode.Equals(zone.FullCode));
                TaskInitialAreaNumericFormatArgument argument = new TaskInitialAreaNumericFormatArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.ListLand = currentLands;
                argument.ToAreaNumeric = groupArgument.ToAreaNumeric;
                argument.ToAreaModule = groupArgument.ToAreaModule;

                argument.ToTableArea = groupArgument.ToTableArea;
                argument.ToActualArea = groupArgument.ToActualArea;
                argument.ToAwareArea = groupArgument.ToAwareArea;
                TaskInitialAreaNumericFormatOperation operation = new TaskInitialAreaNumericFormatOperation();
                operation.Argument = argument;
                operation.Name = "截取地块面积";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        #endregion

        #region Method - 辅助

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                });
            })); ;
        }

        #endregion
    }
}
