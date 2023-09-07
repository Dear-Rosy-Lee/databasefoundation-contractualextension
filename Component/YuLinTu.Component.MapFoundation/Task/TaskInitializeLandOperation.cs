/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskInitializeLandOperation : Task
    {
        public TaskInitializeLandOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskInitializeLandArgument metadata = Argument as TaskInitializeLandArgument;
            if (metadata.land == null)
            {
                return;
            }
            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
            if (dbContext == null)
            {
                this.ReportError("数据连接失败");
                this.ReportProgress(100);
                return;
            }
            GenerateDataShape(dbContext, metadata.land);
            this.ReportProgress(100);
        }

        #region privateMethod

        /// <summary>
        /// 初始化选择区域地块信息
        /// </summary>
        private void GenerateDataShape(IDbContext dbContext, ContractLand land)
        {

            IContractLandWorkStation zonestation = dbContext.CreateContractLandWorkstation();
            this.ReportProgress(0, "开始");
            List<ContractLand> lands = zonestation.GetLandsBYGraph(land.Shape);
            this.ReportProgress(10, "获取地块");
            double Percent = 0.0;  //百分比
            if(lands==null || lands.Count<=0)
            {
                this.ReportWarn("当前区域没有地块");
                return;
            }
            Percent = 98 / (double)lands.Count;
            int index = 0;   //索引
           
            try
            {
                //dbContext.BeginTransaction();
                for (int i = 0; i < lands.Count; i++)
                {
                    lands[i].Name = land.Name;
                    lands[i].OwnRightType = land.OwnRightType; //所有权性质
                    lands[i].ConstructMode = land.ConstructMode;//承包方式
                    lands[i].LandCategory = land.LandCategory;//地块类别
                    lands[i].LandLevel = land.LandLevel;//地力等级
                    lands[i].Purpose = land.Purpose;//土地用途
                    lands[i].PlantType = land.PlantType;//耕保类型
                    lands[i].ManagementType = land.ManagementType;//经营方式
                    lands[i].LandCode = land.LandCode;
                    lands[i].LandName = land.LandName;
                    lands[i].IsFarmerLand = land.IsFarmerLand;
                    lands[i].IsFlyLand = land.IsFlyLand;

                    AgricultureLandExpand landexpand = lands[i].LandExpand;
                    landexpand.Name = land.Name;
                    landexpand.SurveyPerson = land.LandExpand.SurveyPerson;                    
                    landexpand.SurveyDate = land.LandExpand.SurveyDate;
                    landexpand.SurveyChronicle = land.LandExpand.SurveyChronicle;
                    landexpand.CheckPerson = land.LandExpand.CheckPerson;
                    landexpand.CheckDate = land.LandExpand.CheckDate;
                    landexpand.CheckOpinion = land.LandExpand.CheckOpinion;

                    lands[i].LandExpand = landexpand;
                    index++;
                    this.ReportProgress((int)(Percent * index), string.Format("生成第{0}个数据", index.ToString()));
                }
                int InsertInt = zonestation.UpdateRange(lands);
                if (InsertInt <= 0)
                {
                    this.ReportProgress(100, "更新失败.");
                    this.ReportError(string.Format("共生成0个数据"));
                }
                //dbContext.CommitTransaction();
                this.ReportProgress(100, "赋值完成.");
                this.ReportInfomation(string.Format("共赋值{0}地块数据", index.ToString()));
            }
            catch (Exception ex)
            {
                this.ReportError("异常：" + ex.Message);
                this.ReportProgress(100, "异常：" + ex.Message);
                //dbContext.RollbackTransaction();
                return;
            }

        }


        #endregion




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

    }




}
