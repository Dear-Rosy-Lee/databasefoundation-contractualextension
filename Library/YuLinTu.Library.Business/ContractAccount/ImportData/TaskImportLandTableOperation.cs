/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入地块调查表任务类
    /// </summary>
    public class TaskImportLandTableOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportLandTableOperation()
        { }

        #endregion

        #region Field

        #endregion

        #region Property

        /// <summary>
        /// 是否确权确股
        /// </summary>
        public bool IsStockRight
        {
            get; set;
        }

        public SystemSetDefine SystemSet
        {
            get { return SystemSetDefine.GetIntence(); }
        }

        public ContractBusinessSettingDefine SettingDefine
        {
            get { return ContractBusinessSettingDefine.GetIntence(); }
        }

        public ContractBusinessImportSurveyDefine ContractLandImportSurveyDefine { get { return ContractBusinessImportSurveyDefine.GetIntence(); } }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportLandTableArgument argument = Argument as TaskImportLandTableArgument;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportWarn("没有找到" + argument.CurrentZone.Name + "对应的调查表");
                this.ReportProgress(100);
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            var landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            landBusiness.VirtualType = argument.VirtualType;
            landBusiness.IsCheckLandNumberRepeat = argument.IsCheckLandNumberRepeat;
            landBusiness.IsCheckLandNumberRepeat=false;
            //导入地块调查表
            bool flag=landBusiness.ImportData(currentZone, fileName, argument.IsNotland);
            this.ReportProgress(100, null);
        }

        #endregion

        #region Method—Helper

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
}
