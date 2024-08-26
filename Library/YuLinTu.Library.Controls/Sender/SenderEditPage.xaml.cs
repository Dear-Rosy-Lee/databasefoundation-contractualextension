/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方编辑页面
    /// </summary>
    public partial class SenderEditPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 当前选择发包方
        /// </summary>
        private CollectivityTissue currentTissue;

        /// <summary>
        /// 临时发包方
        /// </summary>
        private CollectivityTissue tempTissue;

        /// <summary>
        /// 是否添加
        /// </summary>
        private bool isAdd;

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
                this.DataContext = tempTissue;
                SetComboxSelect(tempTissue);
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
        public SenderEditPage(IWorkpage page, bool isAdd = false)
        {
            InitializeComponent();
            this.Workpage = page;
            pageContent.ThePage = page;
            pageContent.Visibility = Visibility.Hidden;
            loadIcon.Visibility = Visibility.Visible;
            this.isAdd = isAdd;
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

        #endregion

        #region Private

        #endregion

        #region Events

        /// <summary>
        /// 提交按钮
        /// </summary>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (isAdd)
            {
                AddData();
            }
            else
            {
                EditData();
            }
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        private void AddData()
        {
            try
            {
                GetComboxSelect();
                currentTissue = tempTissue;
                if (!pageContent.CheckEntity(tempTissue, isAdd))
                {
                    return;
                }
                if (IsNameRepeat(currentTissue))
                {
                    ShowBox(SenderInfo.SenderAdd, SenderInfo.SenderNameCodeRepeat, eMessageGrade.Error);
                    //Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                    //{
                    //    Header = SenderInfo.SenderAdd,
                    //    Message = SenderInfo.SenderNameRepeat,
                    //    MessageGrade = eMessageGrade.Error
                    //});
                    return;
                }
                ModuleMsgArgs argsAdd = new ModuleMsgArgs();
                argsAdd.Datasource = DataBaseSourceWork.GetDataBaseSource();
                argsAdd.Name = SenderMessage.SENDER_ADD;
                argsAdd.Parameter = currentTissue;
                TheBns.Current.Message.Send(this, argsAdd);
                Result = (bool)argsAdd.ReturnValue;
            }
            catch
            {
                Result = false;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        private void EditData()
        {
            try
            {
                GetComboxSelect();
                tempTissue.Code = pageContent.mtbCode.Text;
                if (!pageContent.CheckEntity(tempTissue, isAdd))
                {
                    return;
                }
                if (IsNameRepeat(tempTissue) && currentTissue.ZoneCode == tempTissue.ZoneCode &&
                    currentTissue.Code == tempTissue.Code)
                {
                    ShowBox(SenderInfo.SenderEdit, "该地域下已存在此发包方名称!", eMessageGrade.Error);

                    //Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                    //{
                    //    Header = SenderInfo.SenderAdd,
                    //    Message = SenderInfo.SenderNameRepeat,
                    //    MessageGrade = eMessageGrade.Error
                    //});
                    return;
                }
                ExcuteSaveData();
            }
            catch
            {
                Result = false;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        private void ExcuteSaveData()
        {
            pageContent.CheckEntity(tempTissue, isAdd);
            currentTissue = tempTissue;
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Datasource = DataBaseSourceWork.GetDataBaseSource();
            args.Name = SenderMessage.SENDER_UPDATE;
            args.Parameter = currentTissue;
            TheBns.Current.Message.Send(this, args);
            Result = (bool)args.ReturnValue;
        }

        /// <summary>
        /// 设置Combox值
        /// </summary>
        /// <param name="tissue"></param>
        private void SetComboxSelect(CollectivityTissue tissue)
        {
            if (isAdd)
            {
                return;
            }
            foreach (var item in pageContent.cbCredtype.Items)
            {
                EnumStore<eCredentialsType> type = item as EnumStore<eCredentialsType>;
                if (type.Value == tissue.LawyerCredentType)
                    pageContent.cbCredtype.SelectedItem = item;
            }
            if (!tissue.Code.StartsWith(tissue.ZoneCode))
            {
                pageContent.mtbCode.IsEnabled = true;
            }
            //foreach (var item in pageContent.cbSenderKind.Items)
            //{
            //    EnumStore<eTissueType> type = item as EnumStore<eTissueType>;
            //    if (type.Value == tissue.Type)
            //        pageContent.cbSenderKind.SelectedItem = item;
            //}
            //foreach (var item in pageContent.cbStatus.Items)
            //{
            //    EnumStore<eStatus> type = item as EnumStore<eStatus>;
            //    if (type.Value == tissue.Status)
            //        pageContent.cbStatus.SelectedItem = item;
            //}
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
            //EnumStore<eTissueType> tissuetype = pageContent.cbSenderKind.SelectedItem as EnumStore<eTissueType>;
            //if (tissuetype != null)
            //{
            //    tempTissue.Type = tissuetype.Value;
            //}
            //EnumStore<eStatus> statustype = pageContent.cbStatus.SelectedItem as EnumStore<eStatus>;
            //if (statustype != null)
            //{
            //    tempTissue.Status = statustype.Value;
            //}
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
        /// 名称或编码是否重复
        /// </summary>
        private bool IsNameRepeat(CollectivityTissue tissue)
        {
            bool exit = false;
            try
            {
                ModuleMsgArgs argsAdd = new ModuleMsgArgs();
                argsAdd.Datasource = DataBaseSourceWork.GetDataBaseSource();
                argsAdd.Name = SenderMessage.SENDER_NAMECODEEXIT;
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
