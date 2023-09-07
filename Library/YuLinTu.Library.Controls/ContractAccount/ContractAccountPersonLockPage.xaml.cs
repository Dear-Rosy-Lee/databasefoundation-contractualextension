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
    public partial class ContractAccountPersonLockPage : InfoPageBase
    {
        #region Fields

        private List<VirtualPerson> personItems;
        private ObservableCollection<LockPreson> lockList;  //(选中/未选中)人集合
        bool isChecked = false;  //是否被选中
        private bool allLock;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域承包方集合
        /// </summary>
        public List<VirtualPerson> PersonItems
        {
            get { return personItems; }
            set
            {
                personItems = value;
            }
        }

        public List<VirtualPerson> selectVirtualPersons { get; set; }

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
        public ContractAccountPersonLockPage()
        {
            InitializeComponent();
            personItems = new List<VirtualPerson>();
            lockList = new ObservableCollection<LockPreson>();
            selectVirtualPersons = new List<VirtualPerson>();
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
            Dispatcher.Invoke(new Action(() => { lockList.Clear(); }));
            selectVirtualPersons.Clear();
            allLock = true;
            if (personItems != null && personItems.Count > 0)
            {
                foreach (var item in personItems)
                {
                    isChecked = true;  //选中
                    LockPreson person = new LockPreson() { ID = item.ID, Name = item.Name, Status = isChecked };
                    Dispatcher.Invoke(new Action(() => { lockList.Add(person); }));
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
            foreach (var item in lbItem.Items)
            {
                LockPreson person = item as LockPreson;
                if (person.Status)
                {
                    VirtualPerson selectPerson = personItems.Where(c => c.ID == person.ID).FirstOrDefault();
                    VirtualPerson hasPerson = selectVirtualPersons.Find(c => c.ID == selectPerson.ID);
                    if (hasPerson != null)
                    {
                        //已经存在
                        continue;
                    }
                    else
                    {
                        //不存在
                        selectVirtualPersons.Add(selectPerson);
                    }
                }
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
            isChecked = (bool)cbAllSelect.IsChecked;
            foreach (var item in lbItem.Items)
            {
                LockPreson vpitem = item as LockPreson;
                vpitem.Status = isChecked;
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
                isChecked = (bool)cbItem.IsChecked;
                LockPreson vpitem = lockList.Where(t => t.ID == (Guid)cbItem.Tag).FirstOrDefault();
                vpitem.Status = isChecked;
            }
            if (lockList.Any(t => t.Status == false))
                cbAllSelect.IsChecked = false;
            else
                cbAllSelect.IsChecked = true;
        }

        #endregion

        #endregion

        #region Class

        /// <summary>
        /// 新定义实体类—锁定承包方
        /// </summary>
        class LockPreson : NotifyCDObject
        {
            private bool status;   //状态

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            public bool Status
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
