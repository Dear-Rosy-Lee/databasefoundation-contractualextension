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
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方合户
    /// </summary>
    public partial class PersonCombingPage : InfoPageBase
    {
        #region Fields

        /// <summary>
        /// 合并承包方
        /// </summary>
        private VirtualPersonItem sourceItem = null;

        /// <summary>
        /// 目标承包方
        /// </summary>
        private VirtualPersonItem destinationItem = null;

        /// <summary>
        /// 承包方集合
        /// </summary>
        private ObservableCollection<VirtualPersonItem> personItems;

        /// <summary>
        /// 去除锁定的承包方集合
        /// </summary>
        private ObservableCollection<VirtualPersonItem> vpItems;

        #endregion

        #region Propertys

        /// <summary>
        /// 合并承包方
        /// </summary>
        public VirtualPersonItem SourceItem
        {
            get { return sourceItem; }
        }

        /// <summary>
        /// 目标承包方
        /// </summary>
        public VirtualPersonItem DestinationItem
        {
            get { return destinationItem; }
        }

        /// <summary>
        /// 当前地域承包方集合
        /// </summary>
        public ObservableCollection<VirtualPersonItem> PersonItems
        {
            get { return personItems; }
            set
            {
                personItems = value;
                if (personItems != null && personItems.Count > 0)
                {
                    foreach (var item in personItems)
                    {
                        if (item.Status != eVirtualPersonStatus.Lock)
                            vpItems.Add(item);
                    }
                    InitiallControl();
                }
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
        public PersonCombingPage()
        {
            InitializeComponent();
            vpItems = new ObservableCollection<VirtualPersonItem>();
            Confirm += PersonInfoPage_Confirm;
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitiallControl()
        {
            cbDestination.ItemsSource = vpItems;
            cbDestination.DisplayMemberPath = "Tag.Name";
            cbDestination.SelectedIndex = 0;
            //cbSource.ItemsSource = personItems;
            cbSource.ItemsSource = vpItems;
            cbSource.DisplayMemberPath = "Tag.Name";
            cbSource.SelectedIndex = 1;
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        private void PersonInfoPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            Result = false;
            var dbContext = DataBaseSourceWork.GetDataBaseSource();
            if (dbContext == null)
                return;
            try
            {
                //Business.BeganTranscation();
                dbContext.BeginTransaction();
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = dbContext.CreateContractLandWorkstation();
                VirtualPerson desPerson = destinationItem.Tag;
                VirtualPerson srcPerson = sourceItem.Tag;
                if (desPerson == null || srcPerson == null)
                {
                    return;
                }
                
                List<Person> desList = desPerson.SharePersonList;
                List<Person> srcList = srcPerson.SharePersonList;
                foreach (var item in srcList)
                {
                    item.FamilyID = destinationItem.Tag.ID;
                    item.FamilyNumber = destinationItem.Tag.FamilyNumber;
                    if (item.Relationship == "户主")
                        item.Relationship = "共有人";
                    desList.Add(item);
                }
                desPerson.SharePersonList = desList;
                UpdateContractLand(desPerson, srcPerson, landStation);   //更新地块信息
                personStation.Update(desPerson);
                personStation.Delete(srcPerson.ID);
                dbContext.CommitTransaction();
                //Business.Update(desPerson);
                //Business.Delete(srcPerson.ID);
                //Business.Commit();
                Result = true;
            }
            catch
            {
                //Business.RollBack();
                dbContext.RollbackTransaction();
            }
            e.Parameter = true;
        }

        /// <summary>
        /// 更新地块信息
        /// </summary>
        private int UpdateContractLand(VirtualPerson desPerson, VirtualPerson sourcePerson, IContractLandWorkStation landStation)
        {
            int upCount = 0;
            if (landStation == null)
                return upCount;
            var sourceLands = landStation.GetCollection(sourcePerson.ID);
            if (sourceLands == null || sourceLands.Count == 0)
                return upCount;
            sourceLands.ForEach(c =>
            {
                c.OwnerId = desPerson.ID;
                c.OwnerName = desPerson.Name;
            });
            upCount = landStation.UpdateRange(sourceLands);
            return upCount;
        }

        #endregion

        #region Events

        /// <summary>
        /// 提交数据
        /// </summary> 
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            destinationItem = cbDestination.SelectedItem as VirtualPersonItem;
            sourceItem = cbSource.SelectedItem as VirtualPersonItem;
            if (destinationItem != null && sourceItem != null)
            {
                ConfirmAsync();
            }
        }

        /// <summary>
        /// 选择目标户
        /// </summary>
        private void cbDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VirtualPersonItem desItem = cbDestination.SelectedItem as VirtualPersonItem;
            VirtualPersonItem srcItem = cbSource.SelectedItem as VirtualPersonItem;
            if (desItem != null && srcItem != null && desItem.ID == srcItem.ID)
            {
                VirtualPersonItem item = vpItems.First(t => t.ID != desItem.ID);
                cbSource.SelectedItem = item;
            }
        }

        /// <summary>
        /// 选择合并户
        /// </summary>
        private void cbSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VirtualPersonItem desItem = cbDestination.SelectedItem as VirtualPersonItem;
            VirtualPersonItem srcItem = cbSource.SelectedItem as VirtualPersonItem;
            if (desItem != null && srcItem != null && desItem.ID == srcItem.ID)
            {
                VirtualPersonItem item = vpItems.First(t => t.ID != desItem.ID);
                cbDestination.SelectedItem = item;
            }
        }
        #endregion

        #region Helper

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string header, string desc, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = header,
                Message = desc,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #endregion
    }
}
