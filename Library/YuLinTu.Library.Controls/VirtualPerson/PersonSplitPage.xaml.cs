/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 分户界面
    /// </summary>
    public partial class PersonSplitPage : InfoPageBase
    {
        #region Fields

        private bool drag = false;

        /// <summary>
        /// 分户承包方项
        /// </summary>
        private VirtualPersonItem currentItem;

        /// <summary>
        /// 原承包方成员
        /// </summary>
        private ObservableCollection<BindPerson> sourceList;

        /// <summary>
        /// 新承包方成员
        /// </summary>
        private ObservableCollection<BindPerson> newList;

        /// <summary>
        /// 原承包地块集合
        /// </summary>
        private ObservableCollection<ContractLandBinding> sourceLandBindingList;

        /// <summary>
        /// 新承包地块集合
        /// </summary>
        private ObservableCollection<ContractLandBinding> newLandBindingList;

        /// <summary>
        /// 原承包地块集合
        /// </summary>
        private List<ContractLand> sourceLandList;

        #endregion

        #region Propertys

        /// <summary>
        /// 原户主名称
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// 新户主名称
        /// </summary>
        public string DestinName
        {
            get { return (string)gbdest.Header; }
            private set { gbdest.Header = value; }
        }

        /// <summary>
        /// 新户
        /// </summary>
        public VirtualPersonItem NewItem { get; private set; }

        /// <summary>
        /// 当前地域承包方项
        /// </summary>
        public VirtualPersonItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                sourceList.Clear();
                newList.Clear();
                if (currentItem != null && currentItem.Children.Count > 1)
                {
                    SourceName = "户主:" + currentItem.Tag.Name;
                    DestinName = "户主:";
                    foreach (var item in currentItem.Children)
                    {
                        if (currentItem.Tag.Name == item.Name)
                            continue;
                        sourceList.Add(item);
                    }
                }
                viewSource.ItemsSource = sourceList;
                viewDest.ItemsSource = newList;
            }
        }
        
        /// <summary>
        /// 原地块集合
        /// </summary>
        public List<ContractLand> SourceLandList
        {
            get { return sourceLandList; }
            set
            {
                sourceLandList = value;
                if (value == null)
                    sourceLandList = new List<ContractLand>();
                sourceLandBindingList.Clear();
                newLandBindingList.Clear();
                sourceLandList.ForEach(c => { sourceLandBindingList.Add(new ContractLandBinding(c)); });
                viewLandSource.ItemsSource = sourceLandBindingList;
                viewLandDest.ItemsSource = newLandBindingList;
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
        public PersonSplitPage()
        {
            InitializeComponent();
            sourceList = new ObservableCollection<BindPerson>();
            newList = new ObservableCollection<BindPerson>();
            sourceLandBindingList = new ObservableCollection<ContractLandBinding>();
            newLandBindingList = new ObservableCollection<ContractLandBinding>();
            Confirm += PersonSplitPage_Confirm;
            this.DataContext = this;
            btnAdd.IsEnabled = false;
        }

        #endregion

        #region Private

        /// <summary>
        /// 提交数据
        /// </summary>
        private void PersonSplitPage_Confirm(object sender, MsgEventArgs<bool> e)
        {
            Result = false;
            try
            {
                List<Person> srcPersonList = currentItem.Tag.SharePersonList;
                List<Person> newPerosnList = new List<Person>();
                foreach (var item in newList)
                {
                    Person p = srcPersonList.FirstOrDefault(t => t.ID == item.ID);
                    if (p != null)
                    {
                        //p.Relationship = "户主";
                        newPerosnList.Add(p);
                    }
                    srcPersonList.Remove(p);
                }
                newPerosnList[0].Relationship = "户主";
                currentItem.Tag.SharePersonList = srcPersonList;
                VirtualPerson vp = VirtualPersonItemHelper.CreateVirtualPerson(newPerosnList[0]);
                vp.FamilyExpand = currentItem.Tag.FamilyExpand;
                vp.FamilyNumber = Business.GetFamilyNumber(currentItem.ZoneCode, vp.FamilyExpand.ContractorType);
                vp.Address = currentItem.Tag.Address;
                newPerosnList.ForEach(t =>
                {
                    t.FamilyID = vp.ID;
                    t.FamilyNumber = vp.FamilyNumber;
                });
                vp.SharePersonList = newPerosnList;
                vp.PersonCount = newPerosnList.Count.ToString();
                Business.BeganTranscation();
                Business.Update(currentItem.Tag);
                Business.Add(vp);
                Business.Commit();
                NewItem = VirtualPersonItemHelper.ConvertToItem(vp);
                Result = true;
            }
            catch (Exception ex)
            {
                Business.RollBack();
                YuLinTu.Library.Log.Log.WriteError(this, "(分离数据页面)SplitPage", ex.Message + ex.StackTrace);
                Result = false;
            }
            e.Parameter = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// 提交数据
        /// </summary> 
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in newList)
            {
                currentItem.Children.Remove(item);
            }
            ConfirmAsync();
        }

        /// <summary>
        /// 右移
        /// </summary>
        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            MetroButton mbtn = sender as MetroButton;
            if (mbtn == null)
                return;
            BindPerson bp = sourceList.FirstOrDefault(t => t.ID == (Guid)mbtn.Tag);
            //bp.Relationship = "户主";
            if (bp != null)
            {
                sourceList.Remove(bp);
                newList.Add(bp);
                if (newList.Count == 1)
                {
                    DestinName = "户主:" + bp.Name;
                }
            }
            if (newList.Count > 0)
            {
                btnAdd.IsEnabled = true;
            }
        }

        /// <summary>
        /// 左移
        /// </summary>
        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            MetroButton mbtn = sender as MetroButton;
            if (mbtn == null)
                return;
            BindPerson bp = newList.FirstOrDefault(t => t.ID == (Guid)mbtn.Tag);
            if (bp != null)
            {
                newList.Remove(bp);
                sourceList.Add(bp);
                if (newList.Count == 0)
                {
                    DestinName = "户主:";
                }
            }
            if (newList.Count == 0)
            {
                btnAdd.IsEnabled = false;
            }
            else
            {
                DestinName = "户主:" + newList[0].Name;
            }
        }

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        private void btnImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                var el = sender as UIElement;
                drag = true;
                el.CaptureMouse();
            }
        }

        /// <summary>
        /// 左侧鼠标移动
        /// </summary>
        private void btnImg_LeftMouseMove(object sender, MouseEventArgs e)
        {
            if (!drag)
            {
                return;
            }
            lock (this)
            {
                var el = sender as MetroButton;
                Guid id = (Guid)el.Tag;
                DoDragDrop(id, sourceList);
                drag = false;
            }
        }

        /// <summary>
        /// 右侧鼠标移动
        /// </summary>
        private void btnImg_RightMouseMove(object sender, MouseEventArgs e)
        {
            if (!drag)
            {
                return;
            }
            lock (this)
            {
                var el = sender as MetroButton;
                Guid id = (Guid)el.Tag;
                DoDragDrop(id, newList);
                drag = false;
            }
        }

        /// <summary>
        /// 鼠标弹起
        /// </summary>
        private void btnImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                drag = false;
                var el = sender as UIElement;
                el.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// 鼠标拖放操作
        /// </summary>
        private void DoDragDrop(Guid id, ObservableCollection<BindPerson> list)
        {
            if (id == null || id == Guid.Empty)
                return;
            BindPerson person = list.FirstOrDefault(t => t.ID == id);
            if (person == null)
                return;
            DataObject dataObject = new DataObject(person);
            DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
        }

        /// <summary>
        /// 右侧拖放操作响应
        /// </summary>
        private void viewDistin_Drop(object sender, DragEventArgs e)
        {
            if (!drag)
            {
                return;
            }
            BindPerson bp = e.Data.GetData(typeof(BindPerson)) as BindPerson;
            if (bp != null)
            {
                bool exit = newList.Any(t => t.ID == bp.ID);
                if (exit)
                {
                    return;
                }
                sourceList.Remove(bp);
                newList.Add(bp);
                if (newList.Count == 1)
                {
                    DestinName = "户主:" + bp.Name;
                }
            }
            if (newList.Count > 0)
            {
                btnAdd.IsEnabled = true;
            }
        }

        /// <summary>
        /// 左侧拖放操作响应
        /// </summary>
        private void viewSource_Drop(object sender, DragEventArgs e)
        {
            if (!drag)
            {
                return;
            }
            BindPerson bp = e.Data.GetData(typeof(BindPerson)) as BindPerson;
            if (bp != null)
            {
                bool exit = sourceList.Any(t => t.ID == bp.ID);
                if (exit)
                {
                    return;
                }
                newList.Remove(bp);
                sourceList.Add(bp);
                if (newList.Count == 0)
                {
                    DestinName = "户主:";
                }
            }
            if (newList.Count == 0)
            {
                btnAdd.IsEnabled = false;
            }
            else
            {
                DestinName = "户主:" + newList[0].Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void viewLandDest_Drop(object sender, DragEventArgs e)
        {

        }

        private void viewLandSource_Drop(object sender, DragEventArgs e)
        {

        }

        private void btnLandImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnLandImgg_LeftMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void btnLandImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnLandRight_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLandLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
