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
    /// 批量初始化承包方基本信息任务类
    /// </summary>
    public class TaskGroupInitialVirtualPersonOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialVirtualPersonOperation()
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
            TaskGroupInitialVirtualPersonArgument groupArgument = Argument as TaskGroupInitialVirtualPersonArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.Database;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> allZones = new List<Zone>();
            List<VirtualPerson> listAllPersons = new List<VirtualPerson>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                listAllPersons = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
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
            List<VirtualPerson> currentVps = new List<VirtualPerson>();
            int[] farmerFamilyNumberIndex = new int[] { 1 };  //农户
            int[] personalFamilyNumberIndex = new int[] { 8001 };  //个人
            int[] unitFamilyNumberIndex = new int[] { 9001 };  //单位
            foreach (var zone in allZones)
            {
                currentVps = listAllPersons.FindAll(c => c.ZoneCode.Equals(zone.FullCode));
                TaskInitialVirtualPersonArgument argument = new TaskInitialVirtualPersonArgument();
                argument.Database = dbContext;
                argument.CurrentZone = currentZone;
                argument.InitiallNumber = groupArgument.InitiallNumber;
                argument.InitiallNation = groupArgument.InitiallNation;
                argument.InitiallZip = groupArgument.InitiallZip;
                argument.InitPersonComment = groupArgument.InitPersonComment;
                argument.InitSharePersonComment = groupArgument.InitSharePersonComment;
                argument.InitiallVpAddress = groupArgument.InitiallVpAddress;
                argument.InitiallSurveyPerson = groupArgument.InitiallSurveyPerson;
                argument.InitiallSurveyDate = groupArgument.InitiallSurveyDate;
                argument.InitiallSurveyAccount = groupArgument.InitiallSurveyAccount;
                argument.InitiallCheckPerson = groupArgument.InitiallCheckPerson;
                argument.InitiallCheckDate = groupArgument.InitiallCheckDate;
                argument.InitiallCheckOpinion = groupArgument.InitiallCheckOpinion;
                argument.InitiallPublishAccountPerson = groupArgument.InitiallPublishAccountPerson;
                argument.InitiallPublishDate = groupArgument.InitiallPublishDate;
                argument.InitiallPublishCheckPerson = groupArgument.InitiallPublishCheckPerson;
                argument.InitiallcbPublishAccount = groupArgument.InitiallcbPublishAccount;
                argument.InitialNull = groupArgument.InitialNull;
                argument.InitialContractWay = groupArgument.InitialContractWay;
                argument.InitConcordNumber = groupArgument.InitConcordNumber;
                argument.InitWarrentNumber = groupArgument.InitWarrentNumber;
                argument.InitStartTime = groupArgument.InitStartTime;
                argument.InitEndTime = groupArgument.InitEndTime;
                argument.Address = groupArgument.Address;
                argument.ZipCode = groupArgument.ZipCode;
                argument.PersonComment = groupArgument.PersonComment;
                argument.Expand = groupArgument.Expand;
                argument.CNation = groupArgument.CNation;
                argument.VirtualType = groupArgument.VirtualType;
                argument.ListPerson = VirtualPersonFilter(groupArgument, currentVps);
                argument.FarmerFamilyNumberIndex = farmerFamilyNumberIndex;
                argument.PersonalFamilyNumberIndex = personalFamilyNumberIndex;
                argument.UnitFamilyNumberIndex = unitFamilyNumberIndex;
                argument.VillageInlitialSet = groupArgument.VillageInlitialSet;
                TaskInitialVirtualPersonOperation operation = new TaskInitialVirtualPersonOperation();
                operation.Argument = argument;
                operation.Name = "批量初始化承包方基本信息";
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

        /// <summary>
        /// 过滤显示承包方
        /// </summary>
        private List<VirtualPerson> VirtualPersonFilter(TaskGroupInitialVirtualPersonArgument groupArgument, List<VirtualPerson> listPersons)
        {
            List<VirtualPerson> vps = new List<VirtualPerson>();
            if (listPersons == null || listPersons.Count == 0)
                return vps;
            try
            {
                if (groupArgument.FamilyOtherSet.ShowFamilyInfomation)
                {
                    var persons = listPersons.FindAll(c => c.Name == "集体");
                    foreach (var vp in persons)
                    {
                        listPersons.Remove(vp);
                    }
                    persons.Clear();
                }
                listPersons.ForEach(c => vps.Add(c));
                listPersons.Clear();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VitualPersonFilter(承包方过滤失败!)", ex.Message + ex.StackTrace);
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "过滤承包方",
                    Message = "过滤承包方出错!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                });
            }
            return vps;
        }

        #endregion
    }
}
