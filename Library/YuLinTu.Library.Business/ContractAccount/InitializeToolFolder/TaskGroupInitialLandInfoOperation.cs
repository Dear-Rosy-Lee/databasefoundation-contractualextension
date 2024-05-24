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
    /// 批量初始化地块基本信息任务类
    /// </summary>
    public class TaskGroupInitialLandInfoOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialLandInfoOperation()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupInitialLandInfoArgument groupArgument = Argument as TaskGroupInitialLandInfoArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> allZones = groupArgument.AllZones;
            List<ContractLand> listAllLands = new List<ContractLand>();
            try
            {
                var landStation = dbContext.CreateContractLandWorkstation();
                listAllLands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                if (listAllLands == null)
                    listAllLands = new List<ContractLand>();
                //if (listAllLands.Count == 0)
                //{
                //    ShowBox("初始化地块基本信息", "未获取地块信息,无法执行初始化操作!");
                //    return;
                //}
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包地块数据失败)", ex.Message + ex.StackTrace);
                ShowBox("初始化地块基本信息", string.Format("获取{0}下的承包地块数据失败", currentZone.FullName));
                return;
            }
            List<ContractLand> currentLands = new List<ContractLand>();
            int[] combinationLandNumber = new int[1] { 1 };
            foreach (var zone in allZones)
            {
                currentLands = listAllLands.FindAll(c => c.ZoneCode.Equals(zone.FullCode));
                //if (currentLands == null || currentLands.Count <= 0)
                //    continue;
                var argument = new TaskInitialLandInfoArgument();
                argument.LandName = groupArgument.LandName;
                argument.LandLevel = groupArgument.LandLevel;
                argument.LandPurpose = groupArgument.LandPurpose;
                argument.IsFamer = groupArgument.IsFamer;
                argument.IsCombination = groupArgument.IsCombination;
                argument.IsNew = groupArgument.IsNew;
                argument.AwareAreaEqualActual = groupArgument.AwareAreaEqualActual;
                argument.InitialAwareArea = groupArgument.InitialAwareArea;
                argument.InitialIsFamer = groupArgument.InitialIsFamer;
                argument.InitialLandLevel = groupArgument.InitialLandLevel;
                argument.InitialLandName = groupArgument.InitialLandName;
                argument.InitialLandNumber = groupArgument.InitialLandNumber;
                argument.InitialLandPurpose = groupArgument.InitialLandPurpose;
                argument.HandleContractLand = groupArgument.HandleContractLand;
                argument.InitialMapNumber = groupArgument.InitialMapNumber;
                argument.InitialSurveyPerson = groupArgument.InitialSurveyPerson;
                argument.InitialSurveyDate = groupArgument.InitialSurveyDate;
                argument.InitialSurveyInfo = groupArgument.InitialSurveyInfo;
                argument.InitialCheckPerson = groupArgument.InitialCheckPerson;
                argument.InitialCheckDate = groupArgument.InitialCheckDate;
                argument.InitialCheckInfo = groupArgument.InitialCheckInfo;
                argument.InitialReferPerson = groupArgument.InitialReferPerson;
                argument.LandExpand = groupArgument.LandExpand;
                argument.InitialReferPersonByOwner = groupArgument.InitialReferPersonByOwner;
                argument.VillageInlitialSet = groupArgument.VillageInlitialSet;
                argument.InitialLandNeighbor = groupArgument.InitialLandNeighbor;
                argument.InitialLandNeighborInfo = groupArgument.InitialLandNeighborInfo;
                argument.VirtualType = groupArgument.VirtualType;
                argument.InitialNull = groupArgument.InitialNull;
                argument.InitialQSXZ = groupArgument.InitialQSXZ;
                argument.QSXZ = groupArgument.QSXZ;
                argument.CurrentZone = zone;
                argument.Database = dbContext;
                argument.CurrentZoneLandList = currentLands;
                argument.CombinationLandNumber = combinationLandNumber;
                argument.InitLandComment = groupArgument.InitLandComment;
                argument.LandComment = groupArgument.LandComment;
                TaskInitialLandInfoOperation operation = new TaskInitialLandInfoOperation();
                operation.Argument = argument;
                operation.Name = "初始化" + zone.Name + "地块基本信息";
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
