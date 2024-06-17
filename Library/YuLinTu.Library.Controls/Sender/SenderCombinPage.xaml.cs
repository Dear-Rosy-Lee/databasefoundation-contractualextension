/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方合并页面
    /// </summary>
    public partial class SenderCombinPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 当前选择发包方
        /// </summary>
        private CollectivityTissue currentTissue;
        private List<CollectivityTissue> tissues;
        private IDbContext db;

        /// <summary>
        /// 临时发包方
        /// </summary>
        private CollectivityTissue tempTissue;

        /// <summary>
        /// 是否删除地域及原发包方
        /// </summary>
        private bool isdel;

        /// <summary>
        /// 定义后台线程
        /// </summary>
        private BackgroundWorker worker;

        #endregion

        #region Propertys

        /// <summary>
        /// 当前发包方
        /// </summary>
        public CollectivityTissue CurrentTissue
        {
            get { return currentTissue; }
            set
            {
                currentTissue = value;
                if (currentTissue == null)
                {
                    tempTissue = new CollectivityTissue();
                }
                if (currentTissue.ID == Guid.Empty)
                {
                    currentTissue.ID = Guid.NewGuid();
                }
                tempTissue = value.Clone() as CollectivityTissue;
                tempTissue.ID = Guid.NewGuid();
                this.DataContext = tempTissue;
                SetComboxSelect(tempTissue);
            }
        }

        public List<CollectivityTissue> TissueList
        {
            get { return tissues; }
            set
            {
                tissues = value;
                var collectivityTissue = tissues[0].Clone() as CollectivityTissue;
                collectivityTissue.ID = Guid.NewGuid();
                if (CurrentZone != null)
                {
                    collectivityTissue.ID = CurrentZone.ID;
                    collectivityTissue.ZoneCode = CurrentZone.FullCode;
                    collectivityTissue.Code = CurrentZone.FullCode.PadRight(14, '0');
                }
                CurrentTissue = collectivityTissue;
            }
        }

        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; private set; }

        #endregion

        #region ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SenderCombinPage(IWorkpage page)
        {
            InitializeComponent();
            this.Workpage = page;
            pageContent.ThePage = page;
            pageContent.Visibility = Visibility.Hidden;
            loadIcon.Visibility = Visibility.Visible;
            pageContent.mtbCode.IsEnabled = true;
            db = DataBaseSourceWork.GetDataBaseSource();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
        }

        #endregion

        #region Methods

        #region Override

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            pageContent.Visibility = Visibility.Visible;
            loadIcon.Visibility = Visibility.Collapsed;
            pageContent.CurrentZone = CurrentZone;
        }

        /// <summary>
        /// 任务完成
        /// </summary> 
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = (bool)e.Result;
            this.CanClose = true;
            if (Result)

                Workpage.Page.CloseMessageBox(true);
            MenueEnable(true);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var tissue = e.Argument as CollectivityTissue;
            try
            {
                SenderDataBusiness business = CreateBusiness();
                Result = business.AddSender(tissue);
                if (!Result)
                    throw new Exception("  添加发包方出错!");
                if (TissueList != null && TissueList.Count > 0)
                {
                    var vpbs = new VirtualPersonBusiness(db);
                    var cbs = new ConcordBusiness(db);
                    var landBusiness = new AccountLandBusiness(db);
                    var crBusiness = new ContractRegeditBookBusiness(db);
                    var zBusiness = new ZoneDataBusiness(db);
                    int index = 1;
                    foreach (var t in TissueList)
                    {
                        db.BeginTransaction();
                        worker.ReportProgress(index, t.Name);
                        var zonecode = t.ZoneCode;
                        vpbs.UpdataSenderCode(zonecode, tempTissue);
                        cbs.UpdataSenderCode(zonecode, tempTissue);
                        landBusiness.UpdateLands(zonecode, tempTissue);
                        crBusiness.UpdateList(zonecode, tempTissue);
                        db.CommitTransaction();
                        if (isdel)
                        {
                            business.DeleteSender(t);
                            if (tissue.ZoneCode != t.ZoneCode)
                                zBusiness.DeleteZone(t.ZoneCode);
                        }
                        index++;
                    }
                }
                e.Result = true;
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                e.Result = false;
                Dispatcher.Invoke(() =>
                {
                    lbTip.Text = innermessge(ex);
                    lbTip.Foreground = System.Windows.Media.Brushes.Red;
                });
            }
        }

        private string innermessge(Exception ex)
        {
            if (ex.InnerException != null)
                return innermessge(ex.InnerException);
            return ex.Message;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbTip.Text = $" 数据合并中：{e.UserState.ToString()} ({e.ProgressPercentage}/{tissues.Count})";
        }

        public void MenueEnable(bool enable)
        {
            this.CanClose = enable;
            this.CloseButtonVisibility = enable ? Visibility.Visible : Visibility.Hidden;
            btnSubmit.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            gridContent.IsEnabled = enable;
        }

        #endregion

        #region Private

        #endregion

        #region Events

        /// <summary>
        /// 提交按钮
        /// </summary>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            GetComboxSelect();
            if (!pageContent.CheckEntity(tempTissue))
            {
                return;
            }
            if (IsNameRepeat(tempTissue))
            {
                ShowBox(SenderInfo.SenderAdd, SenderInfo.SenderNameRepeat, eMessageGrade.Error);
                return;
            }
            isdel = cb_recode.IsChecked == null ? false : cb_recode.IsChecked.Value;
            MenueEnable(false);
            if (!worker.IsBusy)
                worker.RunWorkerAsync(tempTissue);
        }

        /// <summary>
        /// 创建业务逻辑
        /// </summary>
        private SenderDataBusiness CreateBusiness()
        {
            IDbContext db = DataBaseSourceWork.GetDataBaseSource();
            SenderDataBusiness business = new SenderDataBusiness();
            business.DbContext = db;
            business.Station = db.CreateSenderWorkStation();
            return business;
        }

        /// <summary>
        /// 设置Combox值
        /// </summary>
        /// <param name="tissue"></param>
        private void SetComboxSelect(CollectivityTissue tissue)
        {
            foreach (var item in pageContent.cbCredtype.Items)
            {
                EnumStore<eCredentialsType> type = item as EnumStore<eCredentialsType>;
                if (type.Value == tissue.LawyerCredentType)
                    pageContent.cbCredtype.SelectedItem = item;
            }
        }

        /// <summary>
        /// 获取Combox值
        /// </summary>
        private void GetComboxSelect()
        {
            EnumStore<eCredentialsType> cretype = pageContent.cbCredtype.SelectedItem as EnumStore<eCredentialsType>;
            if (cretype != null)
            {
                tempTissue.LawyerCredentType = cretype.Value;
            }
            if (!string.IsNullOrEmpty(pageContent.selectZoneCode) && tempTissue.ZoneCode != pageContent.selectZoneCode)
                tempTissue.ZoneCode = pageContent.selectZoneCode;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="state"></param>
        private void SetState(bool state = false)
        {
            this.CanClose = state;
            gridContent.IsEnabled = state;
            Result = state;
            btnSubmit.IsEnabled = state;
            btnCancel.IsEnabled = state;
        }

        /// <summary>
        /// 名称是否重复
        /// </summary>
        private bool IsNameRepeat(CollectivityTissue tissue)
        {
            bool exit = false;
            try
            {
                ModuleMsgArgs argsAdd = new ModuleMsgArgs();
                argsAdd.Datasource = DataBaseSourceWork.GetDataBaseSource();
                argsAdd.Name = SenderMessage.SENDER_NAMEEXIT;
                argsAdd.Parameter = tissue;
                TheBns.Current.Message.Send(this, argsAdd);
                exit = (bool)argsAdd.ReturnValue;
            }
            catch (Exception ex)
            {
                exit = true;
                YuLinTu.Library.Log.Log.WriteException(this, "IsNameRepeat(发包方名称是否重复)", ex.Message + ex.StackTrace);
            }
            return exit;
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #endregion
    }
}
