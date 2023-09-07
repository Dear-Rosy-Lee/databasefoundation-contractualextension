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
    /// 共有人显示
    /// </summary>
    public partial class InvestigationSharePerson : UserControl
    {
        #region Fields

        private Person currentPerson;  //当前选择人
        //private bool drag = false;

        #endregion

        #region Properties

        /// <summary>
        /// 共有人集合
        /// </summary>
        public PersonCollection sharePersons { get; private set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public ObservableCollection<Person> Items { get; private set; }

        /// <summary>
        /// 选择当前共有人
        /// </summary>
        public Person CurrentPerson
        {
            get { return currentPerson; }
            private set { currentPerson = value; }
        }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentContractor { get; set; }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 删除代理
        /// </summary>
        public delegate void DelSharePerson();
        public DelSharePerson delSharePerson { get; set; }

        /// <summary>
        /// 编辑代理
        /// </summary>
        public delegate void EditSharePerson();
        public EditSharePerson editSharePerson { get; set; }

        /// <summary>
        /// 添加代理
        /// </summary>
        public delegate void AddSharePerson();
        public AddSharePerson addSharePerson { get; set; }

        #endregion

        #region Ctor

        public InvestigationSharePerson()
        {
            InitializeComponent();
            DataContext = this;
            Items = new ObservableCollection<Person>();
        }

        #endregion

        #region Methods - Publics

        public void Set(PersonCollection persons)
        {

            if (CurrentContractor!= null && CurrentContractor.Status == eVirtualPersonStatus.Lock)
            {
                sharePersonDelBtn.IsEnabled = false;
                sharePersonEditBtn.IsEnabled = false;
                sharePersonAddBtn.IsEnabled = false;
            }
            else if (CurrentContractor != null && CurrentContractor.Status != eVirtualPersonStatus.Lock)
            {
                sharePersonDelBtn.IsEnabled = true;
                sharePersonEditBtn.IsEnabled = true;
                sharePersonAddBtn.IsEnabled = true;
            }


            sharePersons = persons;
            Items.Clear();
            if (sharePersons == null || sharePersons.Count == 0)
            {
                return;
            }
            persons.ForEach(t => Items.Add(t));
        }

        #endregion

        #region Methods - Privates

        #endregion

        #region Methods - Events

        private void MetroListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GetSelectItem();
            if (CurrentPerson == null)
            {
                return;
            }
            if (CurrentContractor.Status == eVirtualPersonStatus.Lock) return;
            editSharePerson();
        }

        /// <summary>
        /// 选择项改变
        /// </summary>
        private void MetroListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectItem();
        }

        /// <summary>
        /// 获取当前选择项
        /// </summary>
        private void GetSelectItem()
        {
            CurrentPerson = null;
            var item = MetroListBox.SelectedItem;
            if (item is Person)
            {
                CurrentPerson = item as Person;
            }
        }

        /// <summary>
        /// 添加共有人
        /// </summary>
        private void sharePersonAddBtn_Click(object sender, RoutedEventArgs e)
        {
            addSharePerson();
        }

        /// <summary>
        /// 编辑共有人
        /// </summary>
        private void sharePersonEditBtn_Click(object sender, RoutedEventArgs e)
        {
            editSharePerson();
        }

        /// <summary>
        /// 删除共有人
        /// </summary>
        private void sharePersonDelBtn_Click(object sender, RoutedEventArgs e)
        {
            delSharePerson();
        }

        #endregion

        #region Methods - 右键

        /// <summary>
        /// 右键展开时调用
        /// </summary>
        private void MetroListBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            GetSelectItem();
            if (CurrentPerson == null || CurrentContractor == null || (CurrentPerson != null && CurrentContractor.Status == eVirtualPersonStatus.Lock))
            {
                sharePersonDel.IsEnabled = false;
                sharePersonEdit.IsEnabled = false;
                sharePersonAdd.IsEnabled = false;
            }
            else if (CurrentPerson != null && CurrentContractor.Status != eVirtualPersonStatus.Lock)
            {
                sharePersonDel.IsEnabled = true;
                sharePersonEdit.IsEnabled = true;
                sharePersonAdd.IsEnabled = true;
            }
        }

        /// <summary>
        /// 增加共有人
        /// </summary>
        private void sharePersonAdd_Click_1(object sender, RoutedEventArgs e)
        {
            addSharePerson();
        }

        /// <summary>
        /// 编辑共有人
        /// </summary>
        private void sharePersonEdit_Click_1(object sender, RoutedEventArgs e)
        {
            editSharePerson();
        }

        /// <summary>
        /// 删除共有人
        /// </summary>
        private void sharePersonDel_Click_1(object sender, RoutedEventArgs e)
        {
            delSharePerson();
        }

        #endregion

    }
}
