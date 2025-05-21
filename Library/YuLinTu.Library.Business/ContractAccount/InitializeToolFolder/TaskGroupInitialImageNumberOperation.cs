/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 批量初始化地块图幅编号任务类
    /// </summary>
    public class TaskGroupInitialImageNumberOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialImageNumberOperation()
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
            TaskGroupInitialImageNumberArgument groupArgument = Argument as TaskGroupInitialImageNumberArgument;
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
                List<ContractLand> listAllLands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                InitialLands = listAllLands == null ? new List<ContractLand>() : listAllLands.FindAll(c => c.Shape != null);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包地块数据失败)", ex.Message + ex.StackTrace);
                ShowBox("初始化图幅编号", string.Format("获取{0}下的承包地块数据失败", currentZone.FullName));
                return;
            }
            List<ContractLand> currentGeoLands = new List<ContractLand>();
            foreach (var zone in allZones)
            {
                currentGeoLands = InitialLands.FindAll(c => c.ZoneCode.Equals(zone.FullCode));
                TaskInitialImageNumberArgument argument = new TaskInitialImageNumberArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.ListGeoLand = currentGeoLands;
                argument.ScropeIndex = groupArgument.ScropeIndex;
                argument.ScalerIndex = groupArgument.ScalerIndex;
                argument.IsUseYX = groupArgument.IsUseYX;
                argument.IsInitialAllImageNumber = groupArgument.IsInitialAllImageNumber;
                TaskInitialImageNumberOperation operation = new TaskInitialImageNumberOperation();
                operation.Argument = argument;
                operation.Name = "初始化图幅编号";
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
