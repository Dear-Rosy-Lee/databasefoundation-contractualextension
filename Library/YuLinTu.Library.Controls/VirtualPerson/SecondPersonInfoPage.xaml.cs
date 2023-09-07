/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方信息界面
    /// </summary>
    public partial class SecondPersonInfoPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 承包方信息
        /// </summary>
        private SecondVirtualPersonPageContent vpContent;

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
        public ObservableCollection<VirtualPersonItem> Items { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="isAdd">是否新增承包方</param>
        public SecondPersonInfoPage(VirtualPerson vp, Zone zone, VirtualPersonBusiness business, bool isAdd = false)
        {
            InitializeComponent();
            this.contractor = vp;
            this.currentZone = zone;
            this.business = business;
            this.isAdd = isAdd;
            vpContent = new SecondVirtualPersonPageContent();
            vpContent.Visibility = Visibility.Hidden;
            queue = new TaskQueueDispatcher(Dispatcher);
            gridContent.Children.Add(vpContent);
            vpContent.CurrentZone = zone;
            vpContent.Business = business;
            vpContent.IsAdd = isAdd;
            vpContent.Contractor = vp;
            Confirm += InfoPage_Confirm;
            vpContent.Visibility = Visibility.Visible;
            loadIcon.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        private void InfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            if (business == null)
            {
                return;
            }
            try
            {
                if (vpContent.IsAdd)
                {
                    business.Add(contractor);
                }
                else
                {
                    business.Update(contractor);
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
            contractor = vpContent.GetVirtualPerson();
            bool isRepeat = false;
            bool isNumberRepeat = false;
            foreach (var item in Items)
            {
                if (item.ID == contractor.ID)
                    continue;
                if (item.FamilyNumber == contractor.FamilyNumber)
                {
                    isNumberRepeat = true;
                }
                foreach (BindPerson bp in item.Children)
                {
                    if (bp.Name == contractor.Name && bp.ICN == contractor.Number)
                    {
                        isRepeat = true;
                    }
                    if (bp.Name == contractor.Name && bp.Name.Contains("集体"))
                    {
                        isRepeat = true;
                    }
                }
            }
            string errorMsg = vpContent.CheckSubmit();
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
            if (isNumberRepeat)
            {
                ShowBox(VirtualPersonInfo.PersonNumberRe);
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
