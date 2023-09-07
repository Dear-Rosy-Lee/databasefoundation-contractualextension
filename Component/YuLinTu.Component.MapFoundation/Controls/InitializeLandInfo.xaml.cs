using System;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation.Controls
{
    /// <summary>
    /// InitializeLandInfo.xaml 的交互逻辑
    /// </summary>
    public partial class InitializeLandInfo : InfoPageBase
    {
        #region Ctor
        public InitializeLandInfo()
        {
            InitializeComponent();
            listDict = new List<Dictionary>();
            dbContext = DataBaseSource.GetDataBaseSource();
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            listDict = dictBusiness.GetAll();    //获得数据字典里的内容
            DataContext = this;
            if (LandExpand == null)
                LandExpand = new AgricultureLandExpand();
        }
        #endregion

        #region Properties
        public ContractLand land { get; set; }
        private List<Dictionary> listDict;
        private IDbContext dbContext;
        private AgricultureLandExpand landExpand;//地块扩展
        /// <summary>
        /// 地块扩展信息
        /// </summary>
        public AgricultureLandExpand LandExpand
        {
            get { return landExpand; }
            private set
            {
                landExpand = value;
                NotifyPropertyChanged("LandExpand");
            }
        }
        #endregion

        private bool GetContext()
        {
            try
            {
                if (land == null)
                    land = new ContractLand();
                land.Name = txtName.Text;
                land.OwnRightType = GetComboboxSelectValue(cmbOwnerRight); //所有权性质
                land.ConstructMode = GetComboboxSelectValue(cmbContractWay);//承包方式
                land.LandCategory = GetComboboxSelectValue(cmbLandType);//地块类别
                land.LandLevel = GetComboboxSelectValue(cmbQuliaty);//地力等级
                land.Purpose = GetComboboxSelectValue(cmbPropous);//土地用途
                land.PlantType = GetComboboxSelectValue(cbPLowType);//耕保类型
                land.ManagementType = GetComboboxSelectValue(cbTanscateType);//经营方式
                Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
                if (currentLandName == null)
                {
                    return false;
                }
                Dictionary currentLandNameSecond = cmbLandNameSecond.SelectedItem as Dictionary;
                if (cmbLandNameSecond != null)
                {
                    //二级编码
                    land.LandCode = currentLandNameSecond.Code;//list.Find(c => c.Name == currentLandNameSecond.Name).Code;
                    land.LandName = currentLandNameSecond.Name;
                }
                else
                {
                    //一级编码
                    land.LandCode = currentLandName.Code; //list.Find(c => c.Name == currentLandName.Name).Code;
                    land.LandName = currentLandName.Name;
                }
                if (cmbIsBase.SelectedIndex == 0)
                    land.IsFarmerLand = true;
                else if (cmbIsBase.SelectedIndex == 1)
                    land.IsFarmerLand = false;
                else
                    land.IsFarmerLand = null;
                if (cbIsFly.SelectedIndex == 1)
                    land.IsFlyLand = false;
                else
                    land.IsFlyLand = true;


                land.LandExpand = LandExpand;
                return true;

            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 设置下拉控件选择项 
        /// </summary>
        private string GetComboboxSelectValue(MetroComboBox combobox)
        {
            if (combobox == null || combobox.SelectedItem == null)
            {
                return "";
            }
            Dictionary dic = combobox.SelectedItem as Dictionary;
            if (dic == null)
            {
                return "";
            }
            return dic.Code;
        }
        protected override void OnInitializeCompleted()
        {
            //初始化所有权性质
            var qsxzList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.SYQXZ && (!string.IsNullOrEmpty(c.Code)));
            cmbOwnerRight.ItemsSource = qsxzList;
            cmbOwnerRight.DisplayMemberPath = "Name";
            cmbOwnerRight.SelectedIndex = 1;

            //初始化土地利用类型
            var landNameList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX && (!string.IsNullOrEmpty(c.Code))
            && c.Code.Length == 2);
            cmbLandName.ItemsSource = landNameList;
            cmbLandName.DisplayMemberPath = "Name";
            cmbLandName.SelectedIndex = 0;

            //初始化是否基本农田
            //var isFarmerList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.SF && (!string.IsNullOrEmpty(c.Code)));
            //cmbIsBase.ItemsSource = isFarmerList;
            //cmbIsBase.DisplayMemberPath = "Name";
            cmbIsBase.SelectedIndex = 2;

            //初始化地力等级
            var landLevelList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ && (!string.IsNullOrEmpty(c.Code)));
            cmbQuliaty.ItemsSource = landLevelList;
            cmbQuliaty.DisplayMemberPath = "Name";
            cmbQuliaty.SelectedItem = landLevelList.Find(c => c.Code == "900");

            //承包方式
            var landCbfs = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS && (!string.IsNullOrEmpty(c.Code)));
            cmbContractWay.ItemsSource = landCbfs;
            cmbContractWay.DisplayMemberPath = "Name";
            cmbContractWay.SelectedIndex = 0;

            //地块类型
            var landDklb = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB && (!string.IsNullOrEmpty(c.Code)));
            cmbLandType.ItemsSource = landDklb;
            cmbLandType.DisplayMemberPath = "Name";
            cmbLandType.SelectedIndex = 0;

            //初始化土地用途
            var landPurposeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT && (!string.IsNullOrEmpty(c.Code)));
            cmbPropous.ItemsSource = landPurposeList;
            cmbPropous.DisplayMemberPath = "Name";
            cmbPropous.SelectedIndex = 0;

            //耕保类型
            var landPLowType = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.GBZL && (!string.IsNullOrEmpty(c.Code)));
            cbPLowType.ItemsSource = landPLowType;
            cbPLowType.DisplayMemberPath = "Name";
            cbPLowType.SelectedItem = landPLowType.Find(c => c.Code == "3");
            //经营方式
            var landJyfs = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JYFS && (!string.IsNullOrEmpty(c.Code)));
            cbTanscateType.ItemsSource = landJyfs;
            cbTanscateType.DisplayMemberPath = "Name";
            cbTanscateType.SelectedIndex = 0;

            //是否飞地            
            cbIsFly.SelectedIndex = 1;

        }
        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">内容</param>
        public void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, bool hasConfirm = true, bool hasCancel = true, Action<bool?, eCloseReason> action = null)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = title,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                    ConfirmButtonVisibility = hasConfirm ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                    CancelButtonVisibility = hasCancel ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed,
                }, action);
            }));
        }

        #region Event
        private void mbtnLandOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool flag = GetContext();
            if (flag)
            {
                Workpage.Page.CloseMessageBox(true);
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    ShowBox("获取数据", "获取数据失败");
                }));
            }

        }
        private void cmbLandName_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Dictionary currentLandName = cmbLandName.SelectedItem as Dictionary;
            if (currentLandName == null)
            {
                return;
            }
            string code = currentLandName.Code;
            var landNameSecond = listDict.FindAll(c => !string.IsNullOrEmpty(c.Code) && c.Code.StartsWith(code) && c.Code.Length == 3 && c.GroupCode == currentLandName.GroupCode);
            cmbLandNameSecond.ItemsSource = landNameSecond;
            cmbLandNameSecond.DisplayMemberPath = "Name";
            cmbLandNameSecond.SelectedIndex = 0;

        }
        #endregion
    }
}
