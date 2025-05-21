using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.BatchDataBaseTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "批量导出地块Shape数据", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/table-export.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/table-export.png")]
    public class TaskBuildExportResultDataBase : Task
    {
        #region Fields

        private Zone selectZone;
        private IDbContext dbContext;

        /// <summary>
        /// 任务-导出确权成果库地块与界址点、线匹配设置
        /// </summary>
        public TaskExportLandDotCoilSettingDefine TaskExportLandDotCoilDefine
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<TaskExportLandDotCoilSettingDefine>();
                var section = profile.GetSection<TaskExportLandDotCoilSettingDefine>();
                var config = (section.Settings as TaskExportLandDotCoilSettingDefine);
                return config;
            }
        }

        /// <summary>
        /// 数据字典
        /// </summary>
        [Enabled(false)]
        public List<Dictionary> DictList
        {
            get;
            set;
        }

        #endregion

        #region Ctor

        public TaskBuildExportResultDataBase()
        {
            Name = "批量导出数据库";
            Description = "批量导出数据库";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            dbContext = DataBaseSource.GetDataBaseSource();
            if (!ValidateArgs(dbContext))
                return;
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            DictList = dictBusiness.GetAll();
            BuildPyramidProc(dbContext);
        }

        private void BuildPyramidProc(IDbContext dbContext)
        {
            var args = Argument as TaskBuildExportResultDataBaseArgument;

            #region 新版本入口
            ArcDataExportProgress dataProgress = new ArcDataExportProgress(dbContext);
            dataProgress.ProgressChanged += ReportPercent;
            dataProgress.Alert += ReportInfo;
            dataProgress.Argument = args;
            dataProgress.CurrentZone = selectZone;
            dataProgress.Folder = args.SaveFolderName;
            dataProgress.Export();
            #endregion            
        }

        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs(IDbContext dbContext)
        {
            var args = Argument as TaskBuildExportResultDataBaseArgument;
            bool canContinue = true;
            var zoneStation = dbContext.CreateZoneWorkStation();
            if (args.SelectZoneName == null || args.SelectZoneName == "")
            {
                this.ReportError("未选择行政区域!");
                canContinue = false;
                return canContinue;
            }
            string zoneCode = "";
            try
            {
                zoneCode = args.SelectZoneName.Split('#')[1];
            }
            catch
            {
                this.ReportError("未选择行政区域或行政区域节点名称有误，请检查!");
                canContinue = false;
                return canContinue;
            }
            selectZone = zoneStation.Get(z => z.FullCode == zoneCode).FirstOrDefault();

            if (selectZone == null)
            {
                this.ReportError("行政区域无效!");
                canContinue = false;
            }
            if (selectZone != null && selectZone.Level > eZoneLevel.County)
            {
                this.ReportError("行政区域不能超过县级行政区域!");
                canContinue = false;
            }
            if (selectZone != null)
            {
                this.ReportInfomation(string.Format("导出{0}下数据", selectZone.FullName));
            }

            if (string.IsNullOrEmpty(args.SaveFolderName))
            {
                this.ReportError("请选择数据保存路径!");
                canContinue = false;
            }
            if (!System.IO.Directory.Exists(args.SaveFolderName))
            {
                this.ReportError("数据保存路径无效!");
                canContinue = false;
            }

            this.ReportInfomation(string.Format("导出参数设置正确。"));

            return canContinue;
        }

        #endregion

        #region  提示信息

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

        #endregion
    }
}
