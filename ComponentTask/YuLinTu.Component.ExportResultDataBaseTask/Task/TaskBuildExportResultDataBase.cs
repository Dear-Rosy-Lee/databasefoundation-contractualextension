using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    [TaskDescriptor(IsLanguageName = false, Name = "导出汇交成果数据", Gallery = "汇交数据库成果",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/arrow-270.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/arrow-270.png")]
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

        ///// <summary>
        ///// 系统信息常规设置
        ///// </summary>
        //public SystemSetDefine SystemSet
        //{
        //    get
        //    {
        //        var center = TheApp.Current.GetSystemSettingsProfileCenter();
        //        var profile = center.GetProfile<SystemSetDefine>();
        //        var section = profile.GetSection<SystemSetDefine>();
        //        var config = section.Settings as SystemSetDefine;
        //        return config;
        //    }
        //}

        #endregion Fields

        #region Ctor

        public TaskBuildExportResultDataBase()
        {
            Name = "导出数据库成果";
            Description = "导出数据库成果";
        }

        #endregion Ctor

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
            var systemSet = SystemSetDefine.GetIntence();
            #region 新版本入口

            ArcDataExportProgress dataProgress = new ArcDataExportProgress(dbContext);

            #region 通过反射等机制定制化具体的业务处理类
            var temp = WorksheetConfigHelper.GetInstance(dataProgress);
            if (temp != null)
            {
                if (temp is ArcDataExportProgress)
                {
                    dataProgress = (ArcDataExportProgress)temp;
                    dataProgress.DbContext = dbContext;
                }
            }

            #endregion

            dataProgress.ProgressChanged += ReportPercent;
            dataProgress.Alert += ReportInfo;
            dataProgress.Argument = args;
            dataProgress.currentZone = selectZone;
            dataProgress.Folder = args.SaveFolderName;
            dataProgress.ContainMatrical = args.HasAffixData;
            //dataProgress.IsReportErrorICN = args.IsReportErrorICN;
            dataProgress.UnitName = args.UnitName;
            dataProgress.LinkMan = args.ContactPerson;
            dataProgress.Telephone = args.ContactPhone;
            dataProgress.Address = args.MailingAddress;
            dataProgress.PosterNumber = args.PostCode;
            dataProgress.ContainDotLine = true;
            dataProgress.CanChecker = args.InspectionData;
            dataProgress.DictList = DictList;
            dataProgress.CheckCardNumber = args.InspectionData; //args.InspectionDocNumRepeat;
            dataProgress.IsReportNoConcordLands = args.IsReportNoConcordLands;
            dataProgress.IsSaveParcelPathAsPDF = args.IsSaveParcelPathAsPDF;
            dataProgress.IsReportNoConcordNoLandsFamily = args.IsReportNoConcordNoLandsFamily;
            dataProgress.TaskExportLandDotCoilDefine = TaskExportLandDotCoilDefine;
            dataProgress.CBDKXXAwareAreaExportSet = args.CBDKXXAwareAreaExportSet;
            dataProgress.KeepRepeatFlag = systemSet.KeepRepeatFlag;
            dataProgress.DecimalPlaces = systemSet.DecimalPlaces;
            dataProgress.OnlyKey = args.OnlyExportKey;
            dataProgress.UseUniteNumberExport = args.UseUniteNumberExport;
            dataProgress.OnlyExportLandResult = args.OnlyExportLandResult;
            dataProgress.ContainDotLine = args.ContainDotLine;
            dataProgress.ExportLandCode = args.ExportLandCode;
            dataProgress.Export();
            #endregion 新版本入口

        }

        #endregion Methods - Override

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
            //string authCode = ToolAuthenticate.InitalizeAuthZoneCode();
            //if (zone != null && authCode != "86" && !zone.FullCode.Contains(authCode))
            //{
            //    DialogResult result = MessageBox.Show("选择行政区域未授权,是否重新授权?", "数据检查", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //    if (result == DialogResult.Yes)
            //    {
            //        AuthenticateForm authForm = new AuthenticateForm();
            //        if (authForm.ShowDialog() != DialogResult.OK)
            //        {
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
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
            //if (string.IsNullOrEmpty(args.UnitName))
            //{
            //    this.ReportError("单位名称不允许为空!");
            //    canContinue = false;
            //}
            //if (string.IsNullOrEmpty(args.ContactPerson))
            //{
            //    this.ReportError("联系人不允许为空!");
            //    canContinue = false;
            //}
            //if (!string.IsNullOrEmpty(args.ContactPhone))
            //{
            //    bool isRight = ToolMath.MatchAllNumber(args.ContactPhone.Replace("+", "").Replace("-", ""));
            //    if (!isRight)
            //    {
            //        this.ReportError("联系电话不符合数字要求！");
            //        canContinue = false;
            //    }
            //}
            //else
            //{
            //    this.ReportError("联系电话不允许为空!");
            //    canContinue = false;
            //}
            //if (string.IsNullOrEmpty(args.MailingAddress))
            //{
            //    this.ReportError("通信地址不允许为空!");
            //    canContinue = false;
            //}
            //if (!string.IsNullOrEmpty(args.PostCode))
            //{
            //    bool isRight = args.PostCode.Length == 6 && ToolMath.MatchAllNumber(args.PostCode);
            //    if (!isRight)
            //    {
            //        this.ReportError( "邮政编码不符合数字要求！");
            //        canContinue = false;
            //    }
            //}
            //else
            //{
            //    this.ReportError("邮政编码不允许为空!");
            //    canContinue = false;
            //}

            this.ReportInfomation(string.Format("导出参数设置正确。"));

            return canContinue;
        }

        #endregion Methods - Validate

        #region 提示信息

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

        #endregion 提示信息

        #endregion Methods
    }
}