/*
 * (C) 2025-2018 鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地块初始化界址点界址线
    /// </summary>
    public partial class ContractLandInitializeDotCoil : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitializeDotCoil(IWorkpage page)
        {
            InitializeComponent();
            listDict = new List<Dictionary>();
            dbContext = DataBaseSource.GetDataBaseSource();
            this.Workpage = page;

            cmbAddressPointPrefix.Text = "J";
            wtAddressLinedbiDistance.Text = "1.5";
        }

        #endregion

        #region Fields

        private List<Dictionary> listDict;
        private IDbContext dbContext;
        private InstallDotCoilArg installArg;
        private SetDotCoilArg setArg;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public InstallDotCoilArg InstallArg { get; set; }

        /// <summary>
        /// 赋值参数
        /// </summary>
        public SetDotCoilArg SetArg { get; set; }

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsSetInstallArg { get; set; }

        /// <summary>
        /// 是否赋值
        /// </summary>
        public bool IsSetProperty { get; set; }

        #endregion

        #region Methods-Override

        /// <summary>
        /// 初始化控件开始
        /// </summary>
        protected override void OnInitializeGo()
        {
            var dictBusiness = new DictionaryBusiness(dbContext);
            listDict = dictBusiness.GetAll();    //获得数据字典里的内容
        }

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            //初始化界址点类型
            var AddressDotTypeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JZDLX && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressDotType.ItemsSource = AddressDotTypeList;
            cmbAddressDotType.DisplayMemberPath = "Name";
            cmbAddressDotType.SelectedIndex = 0;

            //初始化界标类型
            var AddressDotMarkTypeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JBLX && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressDotMarkType.ItemsSource = AddressDotMarkTypeList;
            cmbAddressDotMarkType.DisplayMemberPath = "Name";
            cmbAddressDotMarkType.SelectedIndex = 0;

            //初始化界线性质
            var AddressLineTypeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JXXZ && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLineType.ItemsSource = AddressLineTypeList;
            cmbAddressLineType.DisplayMemberPath = "Name";
            cmbAddressLineType.SelectedIndex = 0;

            //初始化界址线类型
            var AddressLineCatalogList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JZXLB && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLineCatalog2.ItemsSource = AddressLineCatalogList;
            cmbAddressLineCatalog2.DisplayMemberPath = "Name";
            cmbAddressLineCatalog2.SelectedIndex = 0;

            //初始化界址线位置
            var AddressLinePositionList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.JZXWZ && (!string.IsNullOrEmpty(c.Code)));
            cmbAddressLinePosition.ItemsSource = AddressLinePositionList;
            cmbAddressLinePosition.DisplayMemberPath = "Name";
            cmbAddressLinePosition.SelectedIndex = 0;

            cbFilterDot.IsChecked = true; cbDescribtionUseUnitNumber.IsChecked = true;
        }

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            GetInterfaceSet();
            if (IsSetInstallArg)//如果需要初始化
            {
                if (wtAddressLinedbiDistance.Value == null)
                {
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.NeighborDistanceValueError);
                    return;
                }
                if (installArg.IsFilterDot && (wtMinAngle.Value == null || wtMaxAngle.Value == null))
                {
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.AngleFilterNoAngleValue);
                    return;
                }
                if (installArg.IsFilterDot && wtMinAngle.Value > wtMaxAngle.Value)
                {
                    ShowBox(ContractAccountInfo.InitialLandDotCoil, ContractAccountInfo.AngleFilterValueError);
                    return;
                }
            }
            if (!IsSetInstallArg && !IsSetProperty)
            {
                ShowBox(ContractAccountInfo.InitialLandDotCoil, "请至少选择一项功能！");
                return;
            }
            installArg.Jzxlbdics = GetDicsInfo();
            InstallArg = installArg;
            SetArg = setArg;

            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 初始化字典信息，界址线类别
        /// </summary>
        /// <param name="meta"></param>
        private List<Dictionary> GetDicsInfo()
        {
            var dicstation = dbContext.CreateDictWorkStation();
            var jzxlbdics = dicstation.GetByGroupCode(DictionaryTypeInfo.JZXLB, false);
            return jzxlbdics;
        }

        /// <summary>
        /// 获取界面配置
        /// </summary>
        private void GetInterfaceSet()
        {
            installArg = new InstallDotCoilArg();
            setArg = new SetDotCoilArg();

            IsSetInstallArg = cbInstall.IsChecked.Value;//是否初始化
            IsSetProperty = cbSetProperty.IsChecked.Value;//是否重新赋值

            installArg.AddressDotType = (cmbAddressDotType.SelectedItem as Dictionary).Code;
            installArg.AddressDotMarkType = (cmbAddressDotMarkType.SelectedItem as Dictionary).Code;

            installArg.IsFilterDot = cbFilterDot.IsChecked.Value;
            installArg.MinAngleFileter = wtMinAngle.Value;
            installArg.MaxAngleFilter = wtMaxAngle.Value;
            installArg.UseAddAlgorithm = cbUseArgorithm.IsChecked.Value;
            installArg.AddressLineType = "600001";//已定界
            installArg.AddressLineCatalog = "01";//田埂
            installArg.AddressLinePosition = "3";//内/外 //如果找不到任何地物就设置为外
            installArg.AddressLinedbiDistance = (double)wtAddressLinedbiDistance.Value;
            installArg.AddressPointPrefix = cmbAddressPointPrefix.Text;
            installArg.LineDescription = (EnumDescription)cmbLineComment.SelectedIndex;
            installArg.IsSetAddressLinePosition = ChangecblineVpNameUsCustom.IsChecked.Value;
            installArg.IsUnit = cbDescribtionUseUnitNumber.IsChecked.Value;

            setArg.IsSetLinePropery = lbAddressLineType.IsChecked.Value;
            setArg.SetAddressLineType = (cmbAddressLineType.SelectedItem as Dictionary).Code;

            setArg.IsSetLineType = lbAddressLineCatalog2.IsChecked.Value;
            setArg.SetAddressLineCatalog = (cmbAddressLineCatalog2.SelectedItem as Dictionary).Code;

            setArg.IsSetLinePosition = cbAddressLinePosition.IsChecked.Value;
            setArg.SetAddressLinePosition = (cmbAddressLinePosition.SelectedItem as Dictionary).Code;

            setArg.IsSetLineDescription = cbLineDescription.IsChecked.Value;
            setArg.SetLineDescription = (EnumDescription)cmbLineComment2.SelectedIndex;

            setArg.IsReplaceLinePerson = lbAddressPointPrefix3.IsChecked.Value;
            setArg.IsReplaceLineRefer = lbAddressPointPrefix4.IsChecked.Value;
            setArg.ReplaceLinePersonFrom = cmbAddressPointPrefix3.Text;
            setArg.ReplaceLinePersonTo = cmbAddressPointPrefix4.Text;
            setArg.ReplaceLineReferFrom = cmbAddressPointPrefix5.Text;
            setArg.ReplaceLineReferTo = cmbAddressPointPrefix6.Text;

            setArg.IsReplaceLinePersonFromMult = lbAddressPointPrefix5.IsChecked.Value;
            setArg.ReplaceLinePersonFromMultTo = cmbAddressPointPrefix7.Text;

            //setArg.IsSetDotLinePrefix = lbAddressPointPrefix2.IsChecked.Value;
            setArg.InitialJZXInfoUseSN = cbInitialJZXInfoUseSN.IsChecked.Value;
            setArg.InitialJZXInfoSet = txtEmptyReplace.Text;
            setArg.IsSetAddressLinePosition = ChangecblineVpNameUsCustom2.IsChecked.Value;
            setArg.IsUnit = cbDescribtionUseUnitNumber2.IsChecked.Value;
        }

        private void cbInitialJZXInfoUseSN_Checked(object sender, RoutedEventArgs e)
        {
            if (cbInitialJZXInfoUseSN.IsChecked.Value)
            {
                gpCoilProperty2.IsEnabled = false;
            }
            else
            {
                gpCoilProperty2.IsEnabled = true;
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region 单个地块初始化处理(针对承包台账模块)

        private ContractLand currentLand;

        /// <summary>
        /// 当前界面选择地块
        /// </summary>
        public ContractLand CurrentLand
        {
            get { return currentLand; }
            set
            {
                currentLand = value;
                if (currentLand != null)
                {
                    tbItem2.Visibility = Visibility.Collapsed;
                    Header = "初始化单个地块界址数据";
                    cbInstall.IsEnabled = false;
                }
            }
        }

        #endregion

        private void cbLineDescription_Checked(object sender, RoutedEventArgs e)
        {
            if (!cbLineDescription.IsChecked.Value)
            {
                cbDescribtionUseUnitNumber2.IsChecked = false;
            }
        }
    }
}
