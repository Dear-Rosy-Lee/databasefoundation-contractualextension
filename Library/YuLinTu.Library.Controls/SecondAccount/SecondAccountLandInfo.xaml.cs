/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 添加与编辑二轮台账地块信息窗体
    /// </summary>
    public partial class SecondAccountLandInfo : InfoPageBase
    {
        #region Property

        /// <summary>
        /// 当前地域属性
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                txtZoneName.Text = currentZone.FullName;
            }
        }

        /// <summary>
        /// 当前选择地域下所有承包方信息
        /// </summary>
        public ObservableCollection<SecondVirtualPersonItem> CurrentItems
        {
            get { return currentItems; }
            set
            {
                currentItems = value;
                //初始化承包方集合
                cmbVirtualPersonName.ItemsSource = currentItems;
                cmbVirtualPersonName.DisplayMemberPath = "Tag.Name";
                cmbVirtualPersonName.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 当前二轮地块属性
        /// </summary>
        public SecondTableLand CurrentSecondLand
        {
            get { return currentSecondLand; }
            set
            {
                currentSecondLand = value;
                this.DataContext = CurrentSecondLand;
                SetComBox(currentSecondLand);
            }
        }

        /// <summary>
        /// 当前选择承包方
        /// </summary>
        public SecondVirtualPersonItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                //设置控件可用性
                SetControlEnable();
            }
        }

        /// <summary>
        /// 标记主界面是否选中项属性
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 定义委托属性-用于向主界面传值
        /// </summary>
        public SetValueToPanel DeliveryValueToPanel { get; set; }

        #endregion

        #region Fields
        /// <summary>
        /// 当前二轮地块
        /// </summary>
        private SecondTableLand currentSecondLand;

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        /// <summary>
        /// 当前选择地域下的所以承包方信息
        /// </summary>
        private ObservableCollection<SecondVirtualPersonItem> currentItems;

        /// <summary>
        /// 是否添加地块
        /// </summary>
        private bool isAdd;

        /// <summary>
        /// 当前选择承包方信息
        /// </summary>
        private SecondVirtualPersonItem currentItem;

        /// <summary>
        /// 当前选择承包方下的所有共有人信息
        /// </summary>
        private List<Person> listPerson;

        /// <summary>
        /// 二轮地块业务
        /// </summary>
        private SecondTableLandBusiness landBusiness;

        /// <summary>
        /// 数据字典业务
        /// </summary>
        private DictionaryBusiness dictBusiness;

        /// <summary>

        /// 二轮承包方业务
        /// </summary>
        private VirtualPersonBusiness personBusiness;


        /// 获取数据字典集合
        /// </summary>
        private List<Dictionary> list;

        /// <summary>
        /// 定义委托方法—用于向主界面传值
        /// </summary>
        /// <param name="item">当前选择承包方</param>
        public delegate void SetValueToPanel(SecondVirtualPersonItem item);

        /// <summary>
        /// 统计共有人个数
        /// </summary>
        private int count;

        /// <summary>
        /// 当前选择一级地类名称
        /// </summary>
        private Dictionary currentLandName;

        /// <summary>
        /// 当前选择二级地类名称
        /// </summary>
        private Dictionary currentLandNameSecond;

        /// <summary>
        /// 地类别名是否选中
        /// </summary>
        private bool isAliseChecked;

        /// <summary>
        /// 二级地类是否选中
        /// </summary>
        private bool isChecked;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SecondAccountLandInfo(bool isAdd = false)
        {
            InitializeComponent();
            this.isAdd = isAdd;
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            landBusiness = new SecondTableLandBusiness(dbContext);
            dictBusiness = new DictionaryBusiness();
            personBusiness = new VirtualPersonBusiness(dbContext);
            personBusiness.VirtualType = eVirtualType.SecondTable;
            dictBusiness.Station = dbContext == null ? null : dbContext.CreateDictWorkStation();
            personBusiness = new VirtualPersonBusiness(dbContext);
            personBusiness.VirtualType = eVirtualType.SecondTable;
            InitialControl();
            Confirm += SecondAccountLandInfo_Confirm;
        }

        #endregion

        #region Method

        /// <summary>
        /// 初始化控件
        /// </summary>
        public void InitialControl()
        {
            list = dictBusiness.GetByGroupCodeDict(DictionaryTypeInfo.TDLYLX);
            if (list != null && list.Count > 0)
            {
                var landNameList = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.Code.Length == 2);
                cmbLandName.ItemsSource = landNameList;
                cmbLandName.DisplayMemberPath = "Name";
                cmbLandName.SelectedIndex = 0;
            }
            isChecked = true;
            isAliseChecked = true;
            cbLandNameSecond.IsChecked = true;
            cbLandNameAlise.IsChecked = true;
            cmbLandNameAlise.Text = currentLandNameSecond.Name;
        }

        /// <summary>
        /// 设置下拉控件
        /// </summary>
        public void SetComBox(SecondTableLand land)
        {
            if (land == null)
            {
                return;
            }
            if (isAdd)
            {
                cbLandNameSecond.IsChecked = true;
                cbLandNameAlise.IsChecked = true;
                cmbLandNameAlise.Text = currentLandNameSecond.Name;
            }
            else
            {
                if (!string.IsNullOrEmpty(land.LandCode))
                {
                    Dictionary dict = list.Find(c => c.Code == land.LandCode);
                    if (dict != null && land.LandCode.Length == 2 && land.LandName != dict.Name)
                    {
                        cmbLandName.Text = dict.Name;
                        cmbLandNameAlise.Text = land.LandName;
                        cbLandNameSecond.IsChecked = false;
                        cbLandNameAlise.IsChecked = false;
                        isAliseChecked = false;
                        isChecked = false;
                    }
                    else if (dict != null && land.LandCode.Length == 2 && land.LandName == dict.Name)
                    {
                        cmbLandName.Text = dict.Name;
                        cmbLandNameAlise.Text = dict.Name;
                        cbLandNameSecond.IsChecked = false;
                        cbLandNameAlise.IsChecked = true;
                        isAliseChecked = true;
                        isChecked = false;
                    }
                    else if (dict != null && land.LandCode.Length == 3 && land.LandName != dict.Name)
                    {
                        Dictionary dictParent = list.Find(c => (!string.IsNullOrEmpty(c.Code)) && c.Code == land.LandCode.Substring(0, 2));
                        cmbLandName.Text = dictParent.Name;
                        cmbLandNameSecond.Text = dict.Name;
                        cmbLandNameAlise.Text = land.LandName;
                        cbLandNameSecond.IsChecked = true;
                        cbLandNameAlise.IsChecked = false;
                        isAliseChecked = false;
                        isChecked = true;
                    }
                    else if (dict != null && land.LandCode.Length == 3 && land.LandName == dict.Name)
                    {
                        Dictionary dictParent = list.Find(c => (!string.IsNullOrEmpty(c.Code)) && c.Code == land.LandCode.Substring(0, 2));
                        cmbLandName.Text = dictParent.Name;
                        cmbLandNameSecond.Text = dict.Name;
                        cmbLandNameAlise.Text = dict.Name;
                        cbLandNameSecond.IsChecked = true;
                        cbLandNameAlise.IsChecked = true;
                        isAliseChecked = true;
                        isChecked = true;
                    }
                }
            }
        }

        /// <summary>
        /// 提交地块操作数据
        /// </summary>
        private void SecondAccountLandInfo_Confirm(object sender, MsgEventArgs<bool> e)
        {
            try
            {
                currentSecondLand.LocationCode = currentZone.FullCode;
                currentSecondLand.LocationName = currentZone.FullName;
                currentSecondLand.OwnerId = currentItem.ID;
                currentSecondLand.OwnerName = currentItem.Tag.Name;
                Dispatcher.Invoke(new Action(() =>
                {
                    if (cmbLandNameSecond.IsEnabled)
                    {
                        //二级编码
                        currentSecondLand.LandCode = list.Find(c => c.Name == currentLandNameSecond.Name).Code;
                        if (!isAliseChecked)
                        {
                            //此时土地利用类型名称应该自定义别名
                            currentSecondLand.LandName = cmbLandNameAlise.Text;
                        }
                        else
                        {
                            currentSecondLand.LandName = currentLandNameSecond.Name;
                        }
                    }
                    else
                    {
                        //一级编码
                        currentSecondLand.LandCode = list.Find(c => c.Name == currentLandName.Name).Code;
                        if (!isAliseChecked)
                        {
                            currentSecondLand.LandName = cmbLandNameAlise.Text;
                        }
                        else
                        {
                            currentSecondLand.LandName = currentLandName.Name;
                        }
                    }
                }));
                if (isAdd)
                {
                    landBusiness.AddLand(currentSecondLand);
                }
                else
                {
                    landBusiness.ModifyLand(currentSecondLand);
                }
                e.Parameter = true;
            }
            catch
            {
                ShowBox(SecondAccountInfo.SecondLandPro, SecondAccountInfo.SecondLandProFail);
            }
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        private void SetControlEnable()
        {
            if (Flag)
            {
                cmbVirtualPersonName.IsEnabled = false;
                txtOwnerName.IsEnabled = false;
            }
            cmbVirtualPersonName.SelectedItem = currentItem;
        }

        #endregion

        #region Event

        /// <summary>
        /// 响应添加或者编辑二轮台账地块信息确定按钮
        /// </summary>
        private void mbtnLandOK_Click(object sender, RoutedEventArgs e)
        {
            //此处做输入值判断
            if (currentSecondLand.TableArea == 0)
            {
                ShowBox(SecondAccountInfo.SecondLandPro, SecondAccountInfo.TableAreaNoZero);
                return;
            }
            ConfirmAsync();
        }

        /// <summary>
        /// 选择承包方
        /// </summary>
        private void cmbVirtualPersonName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtOwnerName.Text = "";
            currentItem = cmbVirtualPersonName.SelectedItem as SecondVirtualPersonItem;
            if (currentItem == null)
            {
                return;
            }
            List<VirtualPerson> listVirtualPerson = personBusiness.GetByZone(currentZone.FullCode);
            listPerson = listVirtualPerson.Find(c => c.ID == currentItem.ID).SharePersonList;
            count = listPerson.Count;
            foreach (var person in listPerson)
            {
                if (count > 1)
                {
                    txtOwnerName.Text += person.Name + "、";
                }
                else
                {
                    txtOwnerName.Text += person.Name;
                }
                count--;
            }
        }

        /// <summary>
        /// 选择一级土地利用类型
        /// </summary>
        private void cmbLandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
            {
                return;
            }
            string code = currentLandName.Code;
            var landNameSecond = list.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.Code.StartsWith(code) && c.Code.Length == 3);
            cmbLandNameSecond.ItemsSource = landNameSecond;
            cmbLandNameSecond.DisplayMemberPath = "Name";
            cmbLandNameSecond.SelectedIndex = 0;
            if (!cmbLandNameSecond.IsEnabled)
            {
                cmbLandNameAlise.Text = currentLandName.Name;
            }
        }

        /// <summary>
        /// 选择二级级土地利用类型
        /// </summary>
        private void cmbLandNameSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
            if (currentLandNameSecond == null)
            {
                return;
            }
            if (isChecked)
            {
                if (isAliseChecked)
                {
                    cmbLandNameAlise.Text = currentLandNameSecond.Name;
                }
                else
                {
                    cmbLandNameAlise.Text = "其它";
                }
            }
        }

        /// <summary>
        /// 是否选中二级地类
        /// </summary>
        private void cbLandNameSecond_Click(object sender, RoutedEventArgs e)
        {
            isChecked = (bool)cbLandNameSecond.IsChecked;
            if (isAliseChecked)
            {
                if (!isChecked)
                {
                    cmbLandNameSecond.IsEnabled = false;
                    cmbLandNameAlise.Text = currentLandName.Name;
                }
                else
                {
                    cmbLandNameSecond.IsEnabled = true;
                    cmbLandNameAlise.Text = currentLandNameSecond.Name;
                }
            }
            else
            {
                if (!isChecked)
                {
                    cmbLandNameSecond.IsEnabled = false;
                    cmbLandNameAlise.Text = "其它";
                }
                else
                {
                    cmbLandNameSecond.IsEnabled = true;
                    cmbLandNameAlise.Text = "其它";
                }
            }
        }

        /// <summary>
        /// 是否选中地类名称
        /// </summary>
        private void cbLandNameAlise_Click(object sender, RoutedEventArgs e)
        {
            isAliseChecked = (bool)cbLandNameAlise.IsChecked;
            if (isChecked)
            {
                if (isAliseChecked)
                {
                    cmbLandNameAlise.Text = currentLandNameSecond.Name;
                }
                else
                {
                    cmbLandNameAlise.Text = "其它";
                }
            }
            else
            {
                if (isAliseChecked)
                {
                    cmbLandNameAlise.Text = currentLandName.Name;
                }
                else
                {
                    cmbLandNameAlise.Text = "其它";
                }
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion
    }
}
