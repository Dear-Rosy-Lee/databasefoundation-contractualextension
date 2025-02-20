/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Windows;
using YuLinTu.Library.WorkStation;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方数据操作任务类
    /// </summary>
    public class TaskSenderOperation : Task
    {
        #region Fields

        private bool isErrorRecord;//含有错误记录

        private string openFilePath;

        #endregion

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        #endregion

        #region Ctor

        public TaskSenderOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        /// <param name="arg"></param>
        protected override void OnGo()
        {
            TaskSenderArgument metadata = Argument as TaskSenderArgument;
            if (metadata == null)
            {
                return;
            }
            bool result = true;
            switch (metadata.ArgType)
            {
                case eSenderArgType.ImportData:
                    ImportSenderData(metadata);
                    result = false;
                    break;
                case eSenderArgType.ExportWord:
                    result = ExportSenderTable(metadata);
                    break;
                case eSenderArgType.ExportExcel:
                    result = ExportSenderExcel(metadata);
                    break;
                case eSenderArgType.InitialSender:
                    InitializeSender(metadata.Database, metadata.ChildrenZone);
                    result = false;
                    break;
                default:
                    break;
            }
            if (result)
                CanOpenResult = true;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            if (string.IsNullOrEmpty(openFilePath))
                return;

            System.IO.FileInfo info = new System.IO.FileInfo(openFilePath);
            if (info.Attributes == System.IO.FileAttributes.Directory)
            {
                if (!System.IO.Directory.Exists(openFilePath))
                    return;
            }
            else
            {
                if (!System.IO.File.Exists(openFilePath))
                    return;
            }
            System.Diagnostics.Process.Start(openFilePath);
        }

        /// <summary>
        /// 导入发包方数据
        /// </summary>
        private void ImportSenderData(TaskSenderArgument argument)
        {
            isErrorRecord = false;
            try
            {
                using (ImportSenderData import = new ImportSenderData())
                {
                    import.ProgressChanged += ReportPercent;
                    import.Alert += ReportInfo;
                    import.DataInstance = argument.Database;
                    import.IsClear = argument.IsClear;
                    this.ReportProgress(1, "开始读取数据");
                    import.ReadZoneInformation(argument.FileName);
                    this.ReportProgress(10, "开始检查数据");
                    if (!isErrorRecord)
                    {
                        this.ReportProgress(20, "开始处理数据");
                        import.InportSenderEntity();
                        this.ReportProgress(100, "完成");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowBox("导入发包方", "导入发包方失败!");
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ImportSenderData(导入发包方数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方Word
        /// </summary>
        private bool ExportSenderTable(TaskSenderArgument argument)
        {
            bool isSuccess = true;
            try
            {
                this.ReportProgress(1, "正在获取发包方数据");
                string messageName = SenderMessage.SENDER_GETCHILDRENDATA;
                ModuleMsgArgs args = new ModuleMsgArgs();
                args.Name = messageName;
                args.Parameter = argument.CurrentZone.FullCode;
                args.Datasource = DataBaseSource.GetDataBaseSource();
                TheBns.Current.Message.Send(this, args);
                List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
                this.ReportProgress(10, "完成发包方数据获取");
                if (list.Count > 0)
                {
                    var dbContext = args.Datasource;
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    List<Zone> selfAndSubZones = zoneStation.GetChildren(argument.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    double percent = 90 / (double)selfAndSubZones.Count;
                    int index = 1;
                    string tempPath = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord);
                    foreach (var zone in selfAndSubZones)
                    {
                        string desc = ExportZoneListDir(zoneStation, zone);
                        var tissues = list.FindAll(c => c.ZoneCode.Equals(zone.FullCode));
                        foreach (var tissue in tissues)
                        {
                            string path = argument.FileName + "\\" + tissue.Name + "(" + tissue.Code + ")";
                            ExportSenderWord senderTable = new ExportSenderWord();

                            #region 通过反射等机制定制化具体的业务处理类
                            var temp = WorksheetConfigHelper.GetInstance(senderTable);
                            if (temp != null && temp.TemplatePath != null)
                            {
                                if (temp is ExportSenderWord)
                                    senderTable = (ExportSenderWord)temp;
                                tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                            }
                            #endregion

                            senderTable.OpenTemplate(tempPath);
                            senderTable.SaveAs(tissue, path);
                        }
                        this.ReportProgress((int)(index * percent) + 10, desc);
                        index++;

                        //提示信息
                        if ((argument.CurrentZone.Level == eZoneLevel.Town || argument.CurrentZone.Level == eZoneLevel.Village) && tissues.Count == 0)
                        {
                            //在镇、村下没有发包方数据(提示信息不显示)
                            continue;
                        }
                        if (tissues.Count > 0)
                        {
                            //地域下有发包方数据
                            this.ReportInfomation(string.Format("{0}导出{1}条发包方数据", desc, tissues.Count));
                        }
                        else
                        {
                            //地域下无发包方数据
                            this.ReportWarn(string.Format("{0}无发包方数据", desc));
                        }
                    }
                    this.ReportProgress(100);
                    this.ReportInfomation(string.Format("成功导出{0}个发包方调查表Word", list.Count));
                    openFilePath = argument.FileName;
                }
                else
                {
                    this.ReportWarn(string.Format("{0}及子级地域未获取到发包方数据!", argument.CurrentZone.FullName));
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                //ShowBox("导出发包方", "导出发包方失败!");
                this.ReportError("导出发包方失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderWord(导出发包方)", ex.Message + ex.StackTrace);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 导出发包方调查表-excel
        /// </summary>
        /// <param name="argument"></param>
        private bool ExportSenderExcel(TaskSenderArgument argument)
        {
            bool isSuccess = true;
            try
            {
                this.ReportProgress(1, "正在获取发包方数据");
                string messageName = SenderMessage.SENDER_GETCHILDRENDATA;
                ModuleMsgArgs args = new ModuleMsgArgs();
                args.Name = messageName;
                args.Parameter = argument.CurrentZone.FullCode;
                args.Datasource = DataBaseSource.GetDataBaseSource();
                TheBns.Current.Message.Send(this, args);
                List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
                this.ReportProgress(10, "完成发包方数据获取");
                if (list.Count > 0)
                {
                    string fileName = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);
                    string path = argument.FileName + @"\" + argument.CurrentZone.FullName + TemplateFile.SenderSurveyExcel + ".xls";
                    using (ExportSenderSurveyTable export = new ExportSenderSurveyTable())
                    {
                        export.TissueCollection = list;
                        export.PostProgressEvent += export_PostProgressEvent;
                        export.ShowValue = false;
                        export.SaveFileName = path;
                        export.BeginToZone(fileName);
                        export.PrintView(path);
                    }
                    this.ReportProgress(100);
                    this.ReportInfomation(string.Format("成功导出{0}条发包方数据", list.Count));
                    openFilePath = path;
                }
                else
                {
                    this.ReportError("未获取到发包方数据!");
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ShowBox("导出发包方调查表", "导出发包方调查表失败!");
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderExcel(导出发包方调查表)", ex.Message + ex.StackTrace);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 初始化发包方
        /// </summary>
        private void InitializeSender(IDbContext dbContex, List<Zone> childrenZone)
        {
            try
            {
                MultiObjectArg args = new MultiObjectArg();
                args.ParameterA = childrenZone;
                args.ParameterB = childrenZone;
                args.ParameterC = true;
                args.ParameterD = ZoneDefine.GetIntence();
                SenderDataBusiness senderBus = new SenderDataBusiness(dbContex);
                Action<int, string> reportProgress = new Action<int, string>((i, msg) =>
                {
                    this.ReportProgress(i, msg);
                });
                senderBus.DbContext = dbContex;
                senderBus.ProcessZoneComplateForImport(args, reportProgress);
                this.ReportProgress(100, null);
                this.ReportInfomation("完成发包方初始化。");
            }
            catch (Exception)
            { }
        }

        #endregion

        #region 辅助方法

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
            if (e.Grade == eMessageGrade.Error)
                isErrorRecord = true;
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

        /// <summary>
        /// 报告进度
        /// </summary>
        /// <param name="progress"></param>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        /// 提示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var showDlg = new TabMessageBoxDialog()
            {
                Header = header,
                Message = msg,
                CancelButtonText = "取消",
            };
            Workpage.Page.ShowMessageBox(showDlg);
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary> 
        private string ExportZoneListDir(IZoneWorkStation zoneStation, Zone zone)
        {
            string zoneName = string.Empty;
            if (zoneStation == null || zone == null)
            {
                return zoneName;
            }
            if (zone.Level == eZoneLevel.Group)
            {
                Zone vZone = zoneStation.Get(c => c.FullCode == zone.UpLevelCode).FirstOrDefault();
                if (vZone == null)
                {
                    return zone.Name;
                }
                Zone tZone = zoneStation.Get(c => c.FullCode == vZone.UpLevelCode).FirstOrDefault();
                zoneName = (tZone == null ? string.Empty : tZone.Name) + vZone.Name + zone.Name;
                vZone = null;
                tZone = null;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                Zone tZone = zoneStation.Get(c => c.FullCode == zone.UpLevelCode).FirstOrDefault();
                if (tZone == null)
                {
                    return zone.Name;
                }
                zoneName = tZone.Name + zone.Name;
                tZone = null;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                zoneName = zone.Name;
            }
            return zoneName;
        }

        #endregion
    }
}
