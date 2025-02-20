/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方信息界面
    /// </summary>
    public partial class VirtualPersonInfoPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 承包方信息
        /// </summary>
        private VirtualPersonPageContent vpContent;

        /// <summary>
        /// 当前承包方
        /// </summary>
        private VirtualPerson contractor;

        /// <summary>
        /// 地域
        /// </summary>
        private Zone currentZone;

        /// <summary>
        /// 业务
        /// </summary>
        private VirtualPersonBusiness business;

        private TaskQueue queue;

        private bool isAdd;

        #endregion

        #region Properties

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// 返回当前承包方
        /// </summary>
        public VirtualPerson Contractor { get { return contractor; } }

        /// <summary>
        /// 当前绑定项集合
        /// </summary>
        public List<VirtualPerson> Items { get; set; }

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine OtherDefine { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="isAdd">是否新增承包方</param>
        public VirtualPersonInfoPage(VirtualPerson vp, Zone zone, VirtualPersonBusiness business, bool isAdd = false, bool showExpand = true, bool isLock = false)
        {
            InitializeComponent();
            this.contractor = vp;
            this.currentZone = zone;
            this.business = business;
            this.isAdd = isAdd;
            vpContent = new VirtualPersonPageContent();
            vpContent.Visibility = Visibility.Hidden;
            if (!showExpand)
            {
                vpContent.tbitemExpand.Visibility = Visibility.Collapsed;
            }
            queue = new TaskQueueDispatcher(Dispatcher);
            gridContent.Children.Add(vpContent);
            vpContent.CurrentZone = zone;
            vpContent.Business = business;
            vpContent.IsAdd = isAdd;
            vpContent.Contractor = vp;
            vpContent.IsLock = isLock;
            Confirm += InfoPage_Confirm;
            vpContent.Visibility = Visibility.Visible;
            loadIcon.Visibility = Visibility.Collapsed;
        }


        //private bool hasError = false;
        //protected override void OnConfirmStarted()
        //{
        //    hasError = vpContent.tbControl.Validate();
        //}

        /// <summary>
        /// 提交数据
        /// </summary>
        private void InfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            //if (hasError)
            //{
            //    e.Parameter = false;
            //    Result = false;
            //    return;
            //}

            if (business == null)
            {
                return;
            }
            bool isSuccess = true;
            try
            {
                if (vpContent.IsAdd)
                {
                    isSuccess = business.Add(contractor);
                }
                else
                {
                    isSuccess = business.Update(contractor);
                }
                if (!isSuccess)
                {
                    Dispatcher.Invoke(new Action(() => { ShowBox("添加承包方信息失败!"); }));
                    e.Parameter = false;
                    Result = false;
                    return;
                }
                e.Parameter = true;
                Result = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "添加承包方", ex.Message + ex.StackTrace);
                e.Parameter = false;
                Result = false;
            }
        }

        #endregion

        #region Override

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            vpContent.Visibility = Visibility.Visible;
            loadIcon.Visibility = Visibility.Collapsed;
            if (contractor.Status == eVirtualPersonStatus.Lock)
                btnAdd.IsEnabled = false;
        }

        #endregion

        #region Private


        #endregion

        #region Events

        /// <summary>
        /// 提交数据
        /// </summary> 
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //通过配置文件进行判断
            contractor = vpContent.GetVirtualPerson();
            bool isRepeat = false;
            bool isNumberRepeat = false;
            foreach (var item in Items)
            {
                if (item.ID == contractor.ID)                       //承包方ID
                    continue;
                if (!string.IsNullOrEmpty(item.FamilyNumber)
                    && !string.IsNullOrEmpty(contractor.FamilyNumber)
                    && item.FamilyNumber == contractor.FamilyNumber.TrimStart('0')
                    && item.FamilyExpand.ContractorType == contractor.FamilyExpand.ContractorType)    //户编号
                {
                    isNumberRepeat = true;
                }
                foreach (Person bp in item.SharePersonList)
                {
                    if (bp.Name == contractor.Name && bp.ICN == contractor.Number)   //姓名
                    {
                        isRepeat = true;
                    }
                    if (bp.Name == contractor.Name && bp.Name.Contains("集体"))
                    {
                        isRepeat = true;
                    }
                }
            }
            string errorMsg = vpContent.CheckSubmit(OtherDefine);
            if (isNumberRepeat)
            {
                ShowBox(VirtualPersonInfo.PersonNumberRe);
                return;
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                ShowBox(errorMsg);
                return;
            }
            if (isRepeat)
            {
                ShowBox(VirtualPersonInfo.PersonInZoneExist);
                return;
            }
            if (vpContent.txtSurveyDate.Value > vpContent.txtCheckDate.Value)
            {
                ShowBox(VirtualPersonInfo.SurveyDatePassCheckDate);
                return;
            }
            if (vpContent.txtCheckDate.Value > vpContent.txtPubDate.Value)
            {
                ShowBox(VirtualPersonInfo.CheckDatePassPubDate);
                return;
            }
            if (string.IsNullOrEmpty(vpContent.txt_Name.Text))
            {
                ShowBox("承包方名称为空!");
                return;
            }
            ConfirmAsync();
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        private void ShowBox(string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = isAdd ? VirtualPersonInfo.AddVirtualPerson : VirtualPersonInfo.EditData,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }
        #endregion

        #endregion
    }
}
