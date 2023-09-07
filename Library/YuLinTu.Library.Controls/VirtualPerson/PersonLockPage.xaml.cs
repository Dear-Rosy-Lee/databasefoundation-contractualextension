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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方锁定页面
    /// </summary>
    public partial class PersonLockPage : InfoPageBase
    {
        #region Fields

        private List<VirtualPersonItem> personItems;
        private List<VirtualPersonItem> sourceItems;
        private ObservableCollection<LockPreson> lockList = new ObservableCollection<LockPreson>();
        private eVirtualPersonStatus status;
        private bool allLock;

        #endregion

        #region Propertys

        /// <summary>
        /// 当前地域承包方集合
        /// </summary>
        public List<VirtualPersonItem> PersonItems
        {
            get { return personItems; }
            set
            {
                sourceItems = value;
            }
        }

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// 业务
        /// </summary>
        public VirtualPersonBusiness Business { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 构造方法
        /// </summary>
        public PersonLockPage()
        {
            InitializeComponent();
            personItems = new List<VirtualPersonItem>();
            Confirm += PersonInfoPage_Confirm;
            this.DataContext = this;
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitializeGo()
        {
            personItems.Clear();
            Dispatcher.Invoke(new Action(() => { lockList.Clear(); }));
            allLock = true;
            if (sourceItems != null && sourceItems.Count > 0)
            {
                foreach (var item in sourceItems)
                {
                    if (item.Status == eVirtualPersonStatus.Right)
                    {
                        allLock = false;
                    }
                    LockPreson lp = new LockPreson() { ID = item.ID, Name = item.Tag.Name, Status = item.Status };
                    Dispatcher.Invoke(new Action(() => { lockList.Add(lp); }));
                }
            }
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            lbItem.ItemsSource = lockList;
            if (allLock)
            {
                cbAllSelect.IsChecked = true;
            }
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        private void PersonInfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            Result = false;
            try
            {
                Business.BeganTranscation();
                foreach (var item in lockList)
                {
                    var srcItem = sourceItems.Where(t => t.ID == item.ID).FirstOrDefault();
                    personItems.Add(srcItem);
                    if (item.Status == srcItem.Status)
                    {
                        continue;
                    }
                    srcItem.Status = item.Status;
                    srcItem.Tag.Status = item.Status;
                    Business.Update(srcItem.Tag);
                }
                Business.Commit();
                Result = true;
            }
            catch
            {
                Business.RollBack();
                personItems.Clear();
            }
            e.Parameter = true;
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAsync();
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void cbAllSelect_Click(object sender, RoutedEventArgs e)
        {
            bool check = (bool)cbAllSelect.IsChecked;
            status = check ? eVirtualPersonStatus.Lock : eVirtualPersonStatus.Right;
            foreach (var item in lbItem.Items)
            {
                LockPreson vpitem = item as LockPreson;
                vpitem.Status = status;
            }
        }

        /// <summary>
        /// 选择项
        /// </summary>
        private void vpItem_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbItem = sender as CheckBox;
            if (cbItem != null)
            {
                status = (bool)cbItem.IsChecked ? eVirtualPersonStatus.Lock : eVirtualPersonStatus.Right;
                LockPreson vpitem = lockList.Where(t => t.ID == (Guid)cbItem.Tag).FirstOrDefault();
                if (vpitem != null)
                    vpitem.Status = status;
            }
            if (lockList.Any(t => t.Status != eVirtualPersonStatus.Lock))
                cbAllSelect.IsChecked = false;
            else
                cbAllSelect.IsChecked = true;
        }

        #endregion

        #endregion

        #region Class

        class LockPreson : NotifyCDObject
        {
            private eVirtualPersonStatus status;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            public eVirtualPersonStatus Status
            {
                get { return status; }
                set { status = value; NotifyPropertyChanged("Status"); }
            }

            /// <summary>
            /// ID
            /// </summary>
            public Guid ID { get; set; }
        }

        #endregion

    }
}
