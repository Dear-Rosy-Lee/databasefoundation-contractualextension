/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
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
        /// 当前选择地域
        /// </summary>
        private CollectivityTissue currentTissue;

        /// <summary>
        /// 临时地域
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
                if (IsNameRepeat())
                {
                    ShowBox(SenderInfo.SenderAdd, SenderInfo.SenderNameRepeat, eMessageGrade.Error);
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
                currentTissue = tempTissue;
                if (!pageContent.CheckEntity(tempTissue, isAdd))
                {
                    return;
                }
                if (IsNameRepeat())
                {
                    ShowBox(SenderInfo.SenderAdd, SenderInfo.SenderNameRepeat, eMessageGrade.Error);

                    //Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                    //{
                    //    Header = SenderInfo.SenderAdd,
                    //    Message = SenderInfo.SenderNameRepeat,
                    //    MessageGrade = eMessageGrade.Error
                    //});
                    return;
                }
                pageContent.CheckEntity(tempTissue, isAdd);
                ModuleMsgArgs args = new ModuleMsgArgs();
                args.Datasource = DataBaseSourceWork.GetDataBaseSource();
                args.Name = SenderMessage.SENDER_UPDATE;
                args.Parameter = currentTissue;
                TheBns.Current.Message.Send(this, args);
                Result = (bool)args.ReturnValue;
            }
            catch
            {
                Result = false;
            }
            Workpage.Page.CloseMessageBox(true);
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
        /// 名称是否重复
        /// </summary>
        private bool IsNameRepeat()
        {
            bool exit = false;
            try
            {
                ModuleMsgArgs argsAdd = new ModuleMsgArgs();
                argsAdd.Datasource = DataBaseSourceWork.GetDataBaseSource();
                argsAdd.Name = SenderMessage.SENDER_NAMEEXIT;
                argsAdd.Parameter = currentTissue;
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
