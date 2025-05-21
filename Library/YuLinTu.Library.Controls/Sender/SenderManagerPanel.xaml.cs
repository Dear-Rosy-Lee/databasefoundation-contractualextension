/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方管理界面
    /// </summary>
    public partial class SenderManagerPanel : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private int sendercount;

        /// <summary>
        /// 发包方业务
        /// </summary>
        private SenderDataBusiness business;

        /// <summary>
        /// 发包方集合
        /// </summary>
        private List<CollectivityTissue> tissueList;

        /// <summary>
        /// 当前发包方
        /// </summary>
        private SenderDataItem selectItem;

        /// <summary>
        /// 定义后台线程
        /// </summary>
        private BackgroundWorker worker;

        /// <summary>
        /// 绑定集合
        /// </summary>
        private ObservableCollection<SenderDataItem> bindList = new ObservableCollection<SenderDataItem>();

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenuEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenuEnableControl MenuEnable { get; set; }
        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        public int TissueCount
        {
            get { return sendercount; }
            set
            {
                sendercount = value;
                NotifyPropertyChanged("TissueCount");
            }
        }

        /// <summary>
        /// 行政发包方集合
        /// </summary>
        public List<CollectivityTissue> TissueList
        {
            get { return tissueList; }
            set
            {
                tissueList = value;
                //if (tissueList != null && tissueList.Count > 0)
                //{
                //    InitializeControl(tissueList);
                //}
            }
        }

        /// <summary>
        /// 界面绑定发包方集合
        /// </summary>
        public ObservableCollection<SenderDataItem> BindTissueList
        {
            get { return bindList; }
            set
            {
                bindList = value;
                NotifyPropertyChanged("BindTissueList");
                TissueCount = bindList.Count;
            }
        }


        public Zone currentZone;

        /// <summary>
        /// 当前选择发包方
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; NotifyPropertyChanged("CurrentZone"); }
        }

        /// <summary>
        /// 根级发包方编码
        /// </summary>
        public string RootZoneCode { get; set; }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        #endregion

        #region Ctor

        public SenderManagerPanel()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        /// <summary>
        /// 设置控件数据发包方编码
        /// </summary>
        public void SetControlData(Zone zone, IWorkpage workPage = null)
        {
            CurrentZone = zone;
            if (workPage != null)
            {
                this.ThePage = workPage;
            }
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(CurrentZone == null ? "" : CurrentZone.FullCode);
            }
        }

        /// <summary>
        /// 任务完成
        /// </summary> 
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MenuEnable();
                BindTissueList.Clear();
                List<CollectivityTissue> tissueList = e.Result as List<CollectivityTissue>;
                if (tissueList != null && tissueList.Count > 0)
                {
                    InitializeControl(tissueList);
                }
                TissueCount = BindTissueList.Count;
                cbIsbatch.IsChecked = false;
                DataContext = this;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(ex.Source, "发包方获取", ex.StackTrace);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MenuEnable(false);
            string zoneCode = e.Argument as string;
            business = new SenderDataBusiness(CreateDb());
            if (zoneCode != null && zoneCode.Length > 6)
            {
                object returnValue = business.SendersByCode(zoneCode);
                e.Result = returnValue as List<CollectivityTissue>;
            }
            else
            {
                e.Result = new List<CollectivityTissue>();
            }
        }
        #endregion

        #region Methods

        #region Method-public

        /// <summary>
        /// 添加发包方
        /// </summary>
        public void Add()
        {
            if (!ZoneNull(SenderInfo.SenderAdd, SenderInfo.AddNoZone))
            {
                return;
            }
            if (!ZoneRight(SenderInfo.SenderAdd, SenderInfo.AddZoneError))
            {
                return;
            }
            business = new SenderDataBusiness(CreateDb());
            CollectivityTissue tissue = new CollectivityTissue();
            tissue.ZoneCode = CurrentZone.FullCode;
            tissue.Name = CurrentZone.FullName;
            tissue.SurveyDate = DateTime.Now;
            tissue.Code = business.CreatSenderCode(CurrentZone, tissue);
            SenderEditPage editPage = new SenderEditPage(ThePage, true);
            editPage.CurrentTissue = tissue;
            editPage.CurrentZone = currentZone;
            ThePage.Page.ShowMessageBox(editPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (editPage.Result)
                {
                    bindList.Add(SenderDataItem.ConvertToItem(editPage.CurrentTissue));
                }
                else
                {
                    ShowBox(SenderInfo.SenderAdd, SenderInfo.AddDataFaile);
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(CreateDb(), SenderMessage.SENDER_ADD_COMPLETE, (editPage.Result ? editPage.CurrentTissue : null)));
            });
        }

        /// <summary>
        /// 编辑发包方
        /// </summary>
        public void Edit()
        {
            if (!SelectNull(SenderInfo.SenderEdit, SenderInfo.EditNull))
            {
                return;
            }
            CollectivityTissue tissue = selectItem.ConvertTo<CollectivityTissue>();
            SenderEditPage editPage = new SenderEditPage(ThePage);
            editPage.CurrentTissue = tissue.Clone() as CollectivityTissue;
            editPage.CurrentZone = currentZone;
            ThePage.Page.ShowMessageBox(editPage, (b, a) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (editPage.Result)
                {
                    SenderDataItem.SetItemValue(selectItem, editPage.CurrentTissue);
                }
                else
                {
                    ShowBox(SenderInfo.SenderEdit, SenderInfo.EditFail);
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(CreateDb(), SenderMessage.SENDER_UPDATE_COMPLETE, (editPage.Result ? editPage.CurrentTissue : null)));
            });
        }

        /// <summary>
        /// 删除发包方
        /// </summary>
        public void Del()
        {
            if (!SelectNull(SenderInfo.SenderDel, SenderInfo.DelNull))
            {
                return;
            }
            business = new SenderDataBusiness(CreateDb());
            CollectivityTissue tissue = selectItem.ConvertTo<CollectivityTissue>();
            bool isDefault = business.IsDefaultSender(tissue);
            if (isDefault)
            {
                ShowBox(SenderInfo.SenderDel, SenderInfo.ForbidDelDefault);
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }

                object returnValue = business.DeleteSender(tissue);
                if ((bool)returnValue)
                {
                    bindList.Remove(selectItem);
                }
                else
                {
                    ShowBox(SenderInfo.SenderDel, SenderInfo.DelDataFaile);
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(CreateDb(), SenderMessage.SENDER_DELETE_COMPLETE, (((bool)returnValue) ? tissue : null)));
            });
            ShowBox(SenderInfo.SenderDel, SenderInfo.DelAffirm, eMessageGrade.Infomation, action);

            //TabMessageBoxDialog messagePage = new TabMessageBoxDialog()
            //{
            //    Header = SenderInfo.SenderDel,
            //    Message = SenderInfo.DelAffirm,
            //    MessageGrade = eMessageGrade.Infomation
            //};
            //ThePage.Page.ShowMessageBox(messagePage, (b, r) =>
            //{
            //    if (b == false)
            //    {
            //        return;
            //    }

            //    object returnValue = business.DeleteSender(tissue);
            //    if ((bool)returnValue)
            //    {
            //        bindList.Remove(selectItem);
            //    }
            //    else
            //    {
            //        ShowBox(SenderInfo.SenderDel, SenderInfo.DelDataFaile);
            //    }
            //    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(CreateDb(), SenderMessage.SENDER_DELETE_COMPLETE, (((bool)returnValue) ? tissue : null)));
            //});
        }

        /// <summary>
        /// 导入发包方数据
        /// </summary>
        public void ImportData()
        {
            IDbContext dbContext = CreateDb();
            ImportDataPage importSender = new ImportDataPage(ThePage, "导入发包方");
            ThePage.Page.ShowMessageBox(importSender, (b, r) =>
            {
                if (string.IsNullOrEmpty(importSender.FileName) || b == false)
                {
                    return;
                }
                TaskSenderArgument meta = new TaskSenderArgument();
                meta.FileName = importSender.FileName;
                meta.IsClear = false;
                meta.ArgType = eSenderArgType.ImportData;
                meta.Database = dbContext;
                TaskSenderOperation import = new TaskSenderOperation();
                import.Workpage = this.ThePage;
                import.Argument = meta;
                import.Description = SenderInfo.ImportDataComment;
                import.Name = SenderInfo.ImportData;
                import.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_IMPORTTABLE_COMPLETE, true));
                });
                ThePage.TaskCenter.Add(import);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                import.StartAsync();
            });
        }

        /// <summary>
        /// 导出发包方word
        /// </summary>
        public void ExportWord()
        {
            if (!ZoneNull(SenderInfo.ExportWord, SenderInfo.ExportNoZone))
            {
                return;
            }
            SenderDataItem item = view.SelectedItem as SenderDataItem;
            if (item != null)
            {
                ExportSingleWord(item);
                return;
            }
            IDbContext dbContext = CreateDb();
            if (CurrentZone.Level <= eZoneLevel.Town)
            {
                ExportDataPage messagePage = new ExportDataPage(CurrentZone.FullName, ThePage, "导出发包方");
                ThePage.Page.ShowMessageBox(messagePage, (b, r) =>
                {
                    if (b == false)
                    {
                        return;
                    }
                    TaskSenderArgument meta = new TaskSenderArgument();
                    meta.Database = dbContext;
                    meta.CurrentZone = CurrentZone;
                    meta.FileName = messagePage.FileName;
                    meta.ArgType = eSenderArgType.ExportWord;
                    TaskSenderOperation senderTask = new TaskSenderOperation();
                    senderTask.Workpage = this.ThePage;
                    senderTask.Argument = meta;
                    senderTask.Name = SenderInfo.ExportTable;
                    senderTask.Description = string.Format(SenderInfo.ExportWordComment, CurrentZone.Name);
                    senderTask.Completed += new TaskCompletedEventHandler((o, e) =>
                    {
                        TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTWORD_COMPLETE, true));
                    });
                    senderTask.Terminated += new TaskTerminatedEventHandler((o, e) =>
                    {
                        TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTWORD_COMPLETE, false));
                    });
                    ThePage.TaskCenter.Add(senderTask);
                    if (ShowTaskViewer != null)
                    {
                        ShowTaskViewer();
                    }
                    senderTask.StartAsync();
                });
            }
            else
            {
                ShowBox(SenderInfo.ExportWord, SenderInfo.SelectedZoneError);
                return;
            }
        }

        /// <summary>
        /// 导出发包方文档
        /// </summary>
        private void ExportSingleWord(SenderDataItem item)
        {
            try
            {
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord);
                ExportSenderWord senderTable = new ExportSenderWord();

                #region 通过反射等机制定制化具体的业务处理类
                var temp = YuLinTu.Library.WorkStation.WorksheetConfigHelper.GetInstance(senderTable);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportSenderWord)
                        senderTable = (ExportSenderWord)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                senderTable.OpenTemplate(tempPath);
                senderTable.PrintPreview(item.ConvertTo<CollectivityTissue>(), SystemSet.DefaultPath + "\\" + item.Name + "(" + item.Code + ")");
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportWord, ex.Message);
            }
        }

        /// <summary>
        /// 导出发包方Word模板
        /// </summary>
        public void ExportWordTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, SenderInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWordTemplate(导出发包方Word模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方Excel模板
        /// </summary>
        public void ExportExcelTemplate()
        {
            try
            {
                string tempFile = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);
                System.Diagnostics.Process.Start(tempFile);
            }
            catch (Exception ex)
            {
                ShowBox(SenderInfo.ExportTemplate, SenderInfo.ExportTemplateNotExit);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportExcelTemplate(导出发包方Excel模板)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出发包方数据
        /// </summary>
        public void ExportExcelData()
        {
            if (!ZoneNull(SenderInfo.ExportExcel, SenderInfo.ExportNoZone))
            {
                return;
            }
            IDbContext dbContext = CreateDb();
            if (CurrentZone.Level <= eZoneLevel.Town)
            {
                var sender = dbContext.CreateSenderWorkStation();
                var listTissue = sender.GetTissues(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                string tempName = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);

                ExportSenderSurveyTable export = new ExportSenderSurveyTable();
                export.TissueCollection = listTissue;
                export.ShowValue = true;
                export.UnitName = SystemSet.GetTableHeaderStr(CurrentZone);
                export.SaveFileName = SystemSet.DefaultPath + "\\" + CurrentZone.FullCode + "-发包方调查表.xls";
                export.BeginToZone(tempName);
                export.PrintView(SystemSet.DefaultPath + "\\" + CurrentZone.FullCode + "-发包方调查表.xls");


                //ExportDataPage messagePage = new ExportDataPage(CurrentZone.FullName, ThePage, "导出发包方");
                //ThePage.Page.ShowMessageBox(messagePage, (b, r) =>
                //{
                //    if (b == false)
                //    {
                //        return;
                //    }
                //TaskSenderArgument meta = new TaskSenderArgument();
                //meta.Database = dbContext;
                //meta.CurrentZone = CurrentZone;
                //meta.ArgType = eSenderArgType.ExportExcel;
                ////meta.FileName = messagePage.FileName;
                //meta.FileName = SystemSet.DefaultPath;
                //TaskSenderOperation taskSender = new TaskSenderOperation();
                //taskSender.Workpage = this.ThePage;
                //taskSender.Argument = meta;
                //taskSender.Name = SenderInfo.ExportData;
                //taskSender.Description = string.Format(SenderInfo.ExportDataComment, CurrentZone.Name);
                //taskSender.Completed += new TaskCompletedEventHandler((o, e) =>
                //{
                //    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTEXCEL_COMPLETE, true));
                //});
                //taskSender.Terminated += new TaskTerminatedEventHandler((o, e) =>
                //{
                //    TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, SenderMessage.SENDER_EXPORTEXCEL_COMPLETE, false));
                //});
                //ThePage.TaskCenter.Add(taskSender);
                //if (ShowTaskViewer != null)
                //{
                //    ShowTaskViewer();
                //}
                //taskSender.StartAsync();
                //});
            }
            else
            {
                ShowBox(SenderInfo.ExportExcel, SenderInfo.SelectedZoneError);
                return;
            }
        }

        /// <summary>
        /// 清空界面数据
        /// </summary>
        public void ClearItem()
        {
            view.Items.Clear();
            view.Items.Refresh();
            view.ItemsSource = null;
        }

        /// <summary>
        /// 刷新发包方
        /// </summary>
        public void Refresh()
        {
            if (worker == null)
            {
                return;
            }
            if (worker.IsBusy)
            {
                return;
            }
            cbIsbatch.IsChecked = false;
            worker.RunWorkerAsync(CurrentZone == null ? "" : CurrentZone.FullCode);
        }

        /// <summary>
        /// 初始化发包方
        /// </summary>
        public void InitializeSender()
        {
            if (CurrentZone == null)
            {
                ShowBox("初始化发包方", "请选择地域进行初始化发包方");
                return;
            }
            var db = CreateDb();
            var zoneStation = db.CreateZoneWorkStation();
            List<Zone> childrenZone = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            TaskSenderArgument meta = new TaskSenderArgument();
            meta.ArgType = eSenderArgType.InitialSender;
            meta.Database = db;
            meta.ChildrenZone = childrenZone;
            TaskSenderOperation initialOperation = new TaskSenderOperation();
            initialOperation.Workpage = ThePage;
            initialOperation.Argument = meta;
            initialOperation.Description = "初始化地域下(包含子级地域)所有发包方";
            initialOperation.Name = "初始化发包方";
            initialOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            ThePage.TaskCenter.Add(initialOperation);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            initialOperation.StartAsync();
        }


        /// <summary>
        /// 合并发包方
        /// </summary>
        public void CombinSenderData()
        {
            var selectlist = bindList.Where(w => w.DataChecked).ToList();
            if (selectlist.Count < 2)
            {
                ShowBox("合并发包方", "合并发包方数量不能少于两个!");
                return;
            }
            var editPage = new SenderCombinPage(ThePage);
            editPage.CurrentZone = currentZone;
            editPage.TissueList = selectlist.ConvertAll(t => t.ConvertTo<CollectivityTissue>());
            ThePage.Page.ShowMessageBox(editPage, (b, a) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (editPage.Result)
                {
                    Refresh();
                }
                else
                {
                    ShowBox("合并发包方", SenderInfo.CombinFail);
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(CreateDb(), SenderMessage.SENDER_UPDATE_COMPLETE, (editPage.Result ? editPage.CurrentTissue : null)));
            });



            //if (CurrentZone == null)
            //{
            //    ShowBox("初始化发包方", "请选择地域进行初始化发包方");
            //    return;
            //}
            //var db = CreateDb();
            //var zoneStation = db.CreateZoneWorkStation();
            //List<Zone> childrenZone = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            //TaskSenderArgument meta = new TaskSenderArgument();
            //meta.ArgType = eSenderArgType.InitialSender;
            //meta.Database = db;
            //meta.ChildrenZone = childrenZone;
            //TaskSenderOperation initialOperation = new TaskSenderOperation();
            //initialOperation.Workpage = ThePage;
            //initialOperation.Argument = meta;
            //initialOperation.Description = "初始化地域下(包含子级地域)所有发包方";
            //initialOperation.Name = "初始化发包方";
            //initialOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            //{
            //});
            //ThePage.TaskCenter.Add(initialOperation);
            //if (ShowTaskViewer != null)
            //    ShowTaskViewer();
            //initialOperation.StartAsync();
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool SelectNull(string header, string message)
        {
            if (selectItem == null)
            {
                ShowBox(header, message);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool ZoneNull(string header, string message)
        {
            if (CurrentZone == null)
            {
                ShowBox(header, message);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool ZoneRight(string header, string message)
        {
            if (CurrentZone.Level > eZoneLevel.Town)
            {
                ShowBox(header, message);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControl(List<CollectivityTissue> list)
        {
            if (list == null || list.Count == 0)
                return;
            list = list.OrderBy(l => l.Code).ToList();
            foreach (CollectivityTissue z in list)
            {
                SenderDataItem node = SenderDataItem.ConvertToItem(z);
                BindTissueList.Add(node);
            }
            //view.Roots = BindTissueList;
        }

        #endregion

        #region Event

        /// <summary>
        /// 双击节点事件
        /// </summary> 
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// 选择节点
        /// </summary>
        private void view_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectItem = view.SelectedItem as SenderDataItem;
        }

        #endregion

        #region Helper

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        #endregion

        #endregion

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectItem = view.SelectedItemInner as SenderDataItem;
        }


        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbIsbatch_Checked(object sender, RoutedEventArgs e)
        {
            var check = ((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked;
            if (bindList.Count > 0)
            {
                foreach (var item in bindList)
                {
                    item.DataChecked = check == null ? false : check.Value;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var check = ((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked;
            var data = ((System.Windows.FrameworkElement)sender).DataContext as SenderDataItem;
            if (data != null)
            {
                data.DataChecked = check == null ? false : check.Value;
            }
        }
    }
}
