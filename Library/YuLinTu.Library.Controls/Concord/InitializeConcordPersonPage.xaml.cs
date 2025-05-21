/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 签订合同选择承包方界面
    /// </summary>
    public partial class InitializeConcordPersonPage : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public InitializeConcordPersonPage()
        {
            InitializeComponent();
            this.DataContext = this;
            SelectPersonCollection = SelectPersonCollection == null ? new List<VirtualPerson>() : SelectPersonCollection;
            FamilyCollection = FamilyCollection == null ? new List<VirtualPerson>() : FamilyCollection;
            Confirm += InitializeConcordPersonPage_Confirm;
        }

        #endregion

        #region Fields

        private List<SelectPerson> allPersons = new List<SelectPerson>();

        #endregion

        #region Properties

        /// <summary>
        /// 承包方集合—用于绑定数据源
        /// </summary>
        public List<VirtualPerson> FamilyCollection { get; set; }

        /// <summary>
        /// 承包方集合—用于存储选中的承包方
        /// </summary>
        public List<VirtualPerson> SelectPersonCollection { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 承包方选择
        /// </summary>
        private void vpItem_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbItem = sender as CheckBox;
            if (cbItem != null)
            {
                var status = (bool)cbItem.IsChecked ? eVirtualPersonStatus.Lock : eVirtualPersonStatus.Right;
                SelectPerson person = allPersons.Find(c => c.ID == (Guid)cbItem.Tag);
                person.Status = status;
            }
            if (allPersons.Any(c => c.Status != eVirtualPersonStatus.Lock))
            {
                cbAllSelect.IsChecked = false;
            }
            else
            {
                cbAllSelect.IsChecked = true;
            }
            VirtualPerson vpItem = new VirtualPerson();
            vpItem = FamilyCollection.Find(c => c.ID == (Guid)cbItem.Tag);
            var hasSelected = SelectPersonCollection.Find(c => c.ID == vpItem.ID);
            if ((bool)cbItem.IsChecked)
            {
                //选中处理
                if (hasSelected != null)
                {
                    return;
                }
                else
                {
                    SelectPersonCollection.Add(vpItem);
                }
            }
            else
            {
                //没选中处理
                if (hasSelected == null)
                {
                    return;
                }
                else
                {
                    SelectPersonCollection.Remove(vpItem);
                }
            }
        }

        /// <summary>
        /// 全选按钮
        /// </summary>
        private void cbAllSelect_Click(object sender, RoutedEventArgs e)
        {
            bool allCheck = (bool)cbAllSelect.IsChecked;
            var status = allCheck ? eVirtualPersonStatus.Lock : eVirtualPersonStatus.Right;
            foreach (var person in allPersons)
            {
                person.Status = status;
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            List<PersonSelectedInfo> infos = new List<PersonSelectedInfo>();
            foreach (var item in lbItem.Items)
            {
                PersonSelectedInfo info = new PersonSelectedInfo();
                SelectPerson stPerson = item as SelectPerson;
                info.ID = stPerson.ID;
                info.Name = stPerson.Name;
                info.Status = stPerson.Status;
                infos.Add(info);
            }
            ConcordExtend.SerializeSelectedInfo(infos);   //系列化文件

            foreach (var person in allPersons)
            {
                VirtualPerson hasSelect = SelectPersonCollection.Find(c => c.ID == person.ID);
                if (hasSelect == null && person.Status == eVirtualPersonStatus.Lock)
                {
                    SelectPersonCollection.Add(FamilyCollection.Find(c => c.ID == person.ID));
                }
                else if (hasSelect != null && person.Status == eVirtualPersonStatus.Right)
                {
                    SelectPersonCollection.Remove(hasSelect);
                }
            }

            ConfirmAsync();
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        public void InitializeConcordPersonPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            e.Parameter = true;   //等价于 Workpage.Page.CloseMessageBox(true);
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override void OnInitializeGo()
        {
            SelectPersonCollection.Clear();
            allPersons.Clear();
            if (FamilyCollection != null || FamilyCollection.Count > 0)
            {
                foreach (var family in FamilyCollection)
                {
                    SelectPerson person = new SelectPerson()
                    {
                        ID = family.ID,
                        Name = family.Name,
                        Status = family.Status,
                    };
                    allPersons.Add(person);
                    SelectPersonCollection.Add(family);
                }
            }
        }

        /// <summary>
        /// 初始化界面完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            if (FamilyCollection == null)
            {
                return;
            }
            lbItem.ItemsSource = allPersons;  //绑定数据源
            List<PersonSelectedInfo> infos = ConcordExtend.DeserializeSelectedInfo();
            foreach (var person in allPersons)
            {
                if (infos == null || infos.Count == 0 || infos.Find(c => c.ID == person.ID) == null)
                {
                    person.Status = eVirtualPersonStatus.Lock;
                }
                else
                {
                    PersonSelectedInfo info = infos.Find(c => c.ID == person.ID);
                    person.Status = info.Status;
                }
            }
            if (allPersons.Any(c => c.Status != eVirtualPersonStatus.Lock))
            {
                cbAllSelect.IsChecked = false;
            }
            else
            {
                cbAllSelect.IsChecked = true;
            }
        }

        #endregion       

    }
}
