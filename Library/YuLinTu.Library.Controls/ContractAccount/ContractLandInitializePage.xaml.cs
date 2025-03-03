/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地块初始化数据界面
    /// </summary>
    public partial class ContractLandInitializePage : InfoPageBase
    {
        #region Ctor


        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitializePage(bool isBatch = false)
        {
            InitializeComponent();
            DataContext = this;
            this.Header = isBatch ? "批量初始化地块基本属性信息" : "初始化地块基本属性信息";
            listDict = new List<Dictionary>();
            dbContext = DataBaseSource.GetDataBaseSource();
            rbCombination.IsEnabled = false;
            cbLandNumberByUpdown.IsEnabled = false;
            rbNew.IsEnabled = false;
            CofirmCommand = new RelayCommand(CofirmExcute, CanCofirmExcute);
        }

        #endregion

        #region Fields

        private int count;//记录修改计数
        private Zone currentZone;
        private List<Dictionary> listDict;
        private IDbContext dbContext;
        private Dictionary landName;
        private Dictionary landLevel;
        private Dictionary landPurpose;
        private bool? isFamer;
        private Dictionary qsxz;
        private bool awareAreaEqualActual = true;
        private bool initialLandName;   //是否初始化地块类别
        private bool initialLandLevel;  //是否初始化地力等级
        private bool initialIsFamer;    //是否初始化基本农田
        private bool initialAwareArea;  //是否初始化确权面积等于
        private bool initialLandPurpose;  //是否初始化土地用途
        private bool initialLandNumber;   //是否初始化地块编码
        private bool initialLandOldNumber = true;   //是否初始化确权地块编码
        private bool initialLandNumberByUpDown;//是否初始化地块编码-从上往下，从左到右
        private bool handleContractLand;  //是否只处理承包地块
        private AgricultureLandExpand landExpand = null;
        private bool isCombination;  //地块编码是否按组合方式生成
        private bool isNew;   //地块编码是否按统一重新生成
        private bool isNewPart;   //地块编码是否按统一重新生成
        //private bool initialLandExpand; //是否初始化地块调查信息

        private bool initialMapNumber;     //是否初始化图幅编号
        private bool initialSurveyPerson;   //是否初始化调查员
        private bool initialSurveyDate;    //是否初始化调查日期
        private bool initialSurveyInfo;    //是否初始化调查记事
        private bool initialCheckPerson;   //是否初始化审核员
        private bool initialCheckDate;    //是否初始化审核日期
        private bool initialCheckInfo;    //是否初始化审核意见
        private bool initialReferPerson;   //是否初始化指界人
        private bool initialQSXZ;//是否初始化所有权性质

        private bool initializeNull;   //只初始化空项
        private bool initialReferPersonByOwner;//以地块当前承包方为指界人

        private bool villageInlitialSet;//按村级进行户号、地块编码统一初始及签订

        private bool initLandComment;//初始化地块备注
        private string landComment = string.Empty;//地块备注

        private bool initialLandNeighbor;//是否获取四至(前信息
        private bool initialLandNeighborInfo;//是否获取地块周边地块集合
        #endregion

        #region Properties

        /// <summary>
        /// 承包台账地块数据业务
        /// </summary>
        public AccountLandBusiness LandBusiness { get; set; }

        public RelayCommand CofirmCommand { get; set; }

        /// <summary>
        /// 当前选择的行政地域
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
        /// 土地利用类型
        /// </summary>
        public Dictionary LandName
        {
            get { return landName; }
            set { landName = value; }
        }
        public Dictionary QSXZ
        {
            get { return qsxz; }
            set { qsxz = value; }
        }
        /// <summary>
        /// 土地等级
        /// </summary>
        public Dictionary LandLevel
        {
            get { return landLevel; }
            set { landLevel = value; }
        }

        /// <summary>
        /// 土地用途
        /// </summary>
        public Dictionary LandPurpose
        {
            get { return landPurpose; }
            set { landPurpose = value; }
        }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public bool? IsFamer
        {
            get { return isFamer; }
            set { isFamer = value; }
        }

        /// <summary>
        /// 确权面积等于
        /// </summary>
        public bool AwareAreaEqualActual
        {
            get { return awareAreaEqualActual; }
            set { awareAreaEqualActual = value; }
        }

        /// <summary>
        /// 是否初始化地块类别
        /// </summary>
        public bool InitialLandName
        {
            get { return initialLandName; }
            set { initialLandName = value; }
        }

        /// <summary>
        /// 是否初始化地力等级
        /// </summary>
        public bool InitialLandLevel
        {
            get { return initialLandLevel; }
            set { initialLandLevel = value; }
        }

        /// <summary>
        /// 是否初始化基本农田
        /// </summary>
        public bool InitialIsFamer
        {
            get { return initialIsFamer; }
            set { initialIsFamer = value; }
        }

        /// <summary>
        /// 是否初始化确权面积等于
        /// </summary>
        public bool InitialAwareArea
        {
            get { return initialAwareArea; }
            set { initialAwareArea = value; }
        }

        /// <summary>
        /// 是否初始化土地用途
        /// </summary>
        public bool InitialLandPurpose
        {
            get { return initialLandPurpose; }
            set { initialLandPurpose = value; }
        }
        public bool InitialQSXZ
        {
            get { return initialQSXZ; }
            set { initialQSXZ = value; }
        }

        /// <summary>
        /// 是否初始化地块编码
        /// </summary>
        public bool InitialLandNumber
        {
            get { return initialLandNumber; }
            set { initialLandNumber = value; }
        }

        /// <summary>
        /// 是否初始化地块编码
        /// </summary>
        public bool InitialLandOldNumber
        {
            get { return initialLandOldNumber; }
            set { initialLandOldNumber = value; }
        }

        /// <summary>
        /// 是否初始化地块编码-从上往下
        /// </summary>
        public bool InitialLandNumberByUpDown
        {
            get { return initialLandNumberByUpDown; }
            set { initialLandNumberByUpDown = value; }
        }

        /// <summary>
        /// 是否只处理承包地块
        /// </summary>
        public bool HandleContractLand
        {
            get { return handleContractLand; }
            set { handleContractLand = value; }
        }

        /// <summary>
        /// 地块编码是否按组合方式生成
        /// </summary>
        public bool IsCombination
        {
            get { return isCombination; }
            set { isCombination = value; }
        }

        /// <summary>
        /// 地块编码是否按统一重新生成
        /// </summary>
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        /// <summary>
        /// 地块编码是否按统一重新生成
        /// </summary>
        public bool IsNewPart
        {
            get { return isNewPart; }
            set { isNewPart = value; }
        }

        /// <summary>
        /// 承包地块扩展信息
        /// </summary>
        public AgricultureLandExpand LandExpand
        {
            get { return landExpand; }
            set { landExpand = value; }
        }

        /// <summary>
        /// 是否初始化图幅编号
        /// </summary>
        public bool InitialMapNumber
        {
            get { return initialMapNumber; }
            set { initialMapNumber = value; }
        }
        /// <summary>
        /// 是否初始化调查员
        /// </summary>
        public bool InitialSurveyPerson
        {
            get { return initialSurveyPerson; }
            set { initialSurveyPerson = value; }
        }

        /// <summary>
        /// 是否初始化调查日期
        /// </summary>
        public bool InitialSurveyDate
        {
            get { return initialSurveyDate; }
            set { initialSurveyDate = value; }
        }

        /// <summary>
        /// 是否初始化调查记事
        /// </summary>
        public bool InitialSurveyInfo
        {
            get { return initialSurveyInfo; }
            set { initialSurveyInfo = value; }
        }

        /// <summary>
        /// 是否初始化审核员
        /// </summary>
        public bool InitialCheckPerson
        {
            get { return initialCheckPerson; }
            set { initialCheckPerson = value; }
        }

        /// <summary>
        /// 是否初始化审核日期
        /// </summary>
        public bool InitialCheckDate
        {
            get { return initialCheckDate; }
            set { initialCheckDate = value; }
        }

        /// <summary>
        /// 是否初始化审核意见
        /// </summary>
        public bool InitialCheckInfo
        {
            get { return initialCheckInfo; }
            set { initialCheckInfo = value; }
        }

        /// <summary>
        /// 是否初始化指界人
        /// </summary>
        public bool InitialReferPerson
        {
            get { return initialReferPerson; }
            set { initialReferPerson = value; }
        }

        /// <summary>
        /// 是否获取(前的四至信息
        /// </summary>
        public bool InitialLandNeighbor
        {
            get { return initialLandNeighbor; }
            set { initialLandNeighbor = value; }
        }

        /// <summary>
        /// 是否地块周边地块信息
        /// </summary>
        public bool InitialLandNeighborInfo
        {
            get { return initialLandNeighborInfo; }
            set { initialLandNeighborInfo = value; }
        }

        /// <summary>
        /// 以地块当前承包方为指界人
        /// </summary>
        public bool InitialReferPersonByOwner
        {
            get { return initialReferPersonByOwner; }
            set { initialReferPersonByOwner = value; }
        }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet
        {
            get { return villageInlitialSet; }
            set { villageInlitialSet = value; }
        }
        /// <summary>
        /// 是否只初始化空项
        /// </summary>
        public bool InitializeNull
        {
            get { return initializeNull; }
            set { initializeNull = value; }
        }
        /// <summary>
        /// 是否初始化地块备注
        /// </summary>
        public bool InitLandComment
        {
            get { return initLandComment; }
            set { initLandComment = value; }
        }
        /// <summary>
        /// 地块备注
        /// </summary>
        public string LandComment
        {
            get { return landComment; }
            set { landComment = value; }
        }
        #endregion

        #region Methods-Override

        /// <summary>
        /// 初始化控件开始
        /// </summary>
        protected override void OnInitializeGo()
        {
            isCombination = true;
            isNew = false;
            initialLandNumberByUpDown = false;
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            listDict = dictBusiness.GetAll();    //获得数据字典里的内容
        }

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            //初始化土地利用类型
            var landNameList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDLYLX && (!string.IsNullOrEmpty(c.Code))
            && c.Code.Length == 2);
            cmbLandName.ItemsSource = landNameList;
            cmbLandName.DisplayMemberPath = "Name";
            cmbLandName.SelectedIndex = 0;

            //初始化是否基本农田
            //var isFarmerList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.SF && (!string.IsNullOrEmpty(c.Code)));
            //cmbIsFarmer.ItemsSource = isFarmerList;
            //cmbIsFarmer.DisplayMemberPath = "Name";
            //cmbIsFarmer.SelectedIndex = 0;

            //初始化地力等级
            var landLevelList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ && (!string.IsNullOrEmpty(c.Code)));
            cmbLandLevel.ItemsSource = landLevelList;
            cmbLandLevel.DisplayMemberPath = "Name";
            cmbLandLevel.SelectedIndex = 0;

            //初始化确权面积等于
            List<string> awareAreaList = new List<string>() { "实测面积", "二轮合同面积" };
            cmbAwareArea.ItemsSource = awareAreaList;
            cmbAwareArea.SelectedIndex = 0;

            //初始化土地用途
            var landPurposeList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.TDYT && (!string.IsNullOrEmpty(c.Code)));
            cmbLandPurpose.ItemsSource = landPurposeList;
            cmbLandPurpose.DisplayMemberPath = "Name";
            cmbLandPurpose.SelectedIndex = 0;

            //初始化所有权性质
            var qsxzList = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.SYQXZ && (!string.IsNullOrEmpty(c.Code)));
            cmbQSXZ.ItemsSource = qsxzList;
            cmbQSXZ.DisplayMemberPath = "Name";
            cmbQSXZ.SelectedIndex = 0;
        }

        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (LandBusiness == null)
            {
                return;
            }

            var lands = LandBusiness.GetCollection(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            if (!lands.Any(t => string.IsNullOrEmpty(t.OldLandNumber)))
            {
                var message = new TabMessageBoxDialog
                {
                    Header = "提示",
                    Message = "地块数据中已有 确权地块编码，若确权地块编码存在错误才执行覆盖，是否覆盖？",
                    MessageGrade = eMessageGrade.Warn,
                    ConfirmButtonText = "覆盖",
                    CancelButtonText = "不覆盖",
                    CloseButtonVisibility = Visibility.Collapsed
                };
                Workpage.Page.ShowDialog(message, (b, c) =>
                {
                    if ((bool)b)
                    {
                        InitialLandOldNumber = true;
                    }
                    else
                    {
                        InitialLandOldNumber = false;
                    }
                    ConfirmAsync(goCallback =>
                    {
                        return SetAndCheckValue();
                    }, completedCallback =>
                    {
                        Close(true);
                    });

                });
            }
            else
            {
                ConfirmAsync(goCallback =>
                {
                    return SetAndCheckValue();
                }, completedCallback =>
                {
                    Close(true);
                });
            }



            // Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 设置和检查值
        /// </summary>
        private bool SetAndCheckValue()
        {
            bool canContinue = true;
            Dispatcher.Invoke(new Action(() =>
            {
                initialLandName = (bool)cbLandName.IsChecked;
                initialLandLevel = (bool)cbLandLevel.IsChecked;
                initialLandPurpose = (bool)cbLandPurpose.IsChecked;
                initialLandNumber = (bool)cbLandNumber.IsChecked;
                initialLandNumberByUpDown = (bool)cbLandNumberByUpdown.IsChecked;
                initialIsFamer = (bool)cbIsFarmer.IsChecked;
                initialAwareArea = (bool)cbAwareArea.IsChecked;
                handleContractLand = (bool)cbHandleContractLand.IsChecked;
                isCombination = (bool)rbCombination.IsChecked;
                isNew = (bool)rbNew.IsChecked;
                IsNewPart = (bool)rbNewPart.IsChecked;
                //initialMapNumber = (bool)cbMapNumber.IsChecked;
                initialQSXZ = (bool)cbQSXZ.IsChecked;
                initialSurveyPerson = (bool)cbSurveyPerson.IsChecked;
                initialSurveyDate = (bool)cbSurveyDate.IsChecked;
                initialSurveyInfo = (bool)cbSurveyInfo.IsChecked;
                initialCheckPerson = (bool)cbCheckPerson.IsChecked;
                initialCheckDate = (bool)cbCheckDate.IsChecked;
                initialCheckInfo = (bool)cbCheckInfo.IsChecked;
                initialReferPerson = (bool)cbReferPerson.IsChecked;
                initializeNull = (bool)cbInitializeNull.IsChecked;
                initialLandNeighbor = (bool)cbInitalContractLandNeighborInfo.IsChecked;
                initialLandNeighborInfo = (bool)cbInitializeLandNeighborInfo.IsChecked;
                initialReferPersonByOwner = (bool)cbReferPersonExpand.IsChecked;
                initLandComment = (bool)cbLandComment.IsChecked;
            }));
            if (!initialQSXZ && !initialLandName && !initialLandLevel && !initialLandPurpose && !initialLandNumber && !initialIsFamer
             && !initialAwareArea && !handleContractLand && !initialMapNumber && !initialSurveyPerson && !initialLandNeighbor && !initialLandNeighborInfo
             && !initialSurveyDate && !initialSurveyInfo && !initialCheckPerson && !initialCheckDate && !initialCheckInfo && !initialReferPerson && !initLandComment)
            {
                return false;
            }

            Dispatcher.Invoke(new Action(() =>
            {
                //地块扩展信息
                landExpand = new AgricultureLandExpand();
                //landExpand.ImageNumber = txtMapNumber.Text.Trim();
                landExpand.SurveyPerson = txtSurveyPerson.Text.Trim();
                landExpand.SurveyDate = txtSurveyDate.Value;
                landExpand.SurveyChronicle = txtSurveyInfo.Text.Trim();
                landExpand.CheckPerson = txtCheckPerson.Text.Trim();
                landExpand.CheckDate = txtCheckDate.Value;
                landExpand.CheckOpinion = txtCheckInfo.Text.Trim();
                landExpand.ReferPerson = txtReferPerson.Text.Trim();
                landComment = txtLandComment.Text.Trim();
            }));

            Dispatcher.Invoke(new Action(() =>
            {
                if (landExpand.SurveyDate > landExpand.CheckDate)
                {
                    this.Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                    {
                        Header = "提示",
                        Message = "调查日期必须小于审核日期！",
                        MessageGrade = eMessageGrade.Warn,
                        CancelButtonText = "取消",
                    });
                    canContinue = false;
                }
            }));

            return canContinue;
        }

        /// <summary>
        /// 土地利用类型改变
        /// </summary>
        private void cmbLandName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            landName = cmbLandName.SelectedItem as Dictionary;
            if (landName == null)
            {
                return;
            }
            var landNameSecond = listDict.FindAll(c => !string.IsNullOrEmpty(c.Code) &&
                                                        c.Code.StartsWith(landName.Code) &&
                                                        c.Code.Length == 3 && c.GroupCode == landName.GroupCode);
            cmb_SecondLandType.ItemsSource = landNameSecond;
            cmb_SecondLandType.DisplayMemberPath = "Name";
            cmb_SecondLandType.SelectedIndex = 0;

        }

        /// <summary>
        /// 确权面积等于改变
        /// </summary>
        private void cmbAwareArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string awareAreaEqual = (string)cmbAwareArea.SelectedItem;
            if (string.IsNullOrEmpty(awareAreaEqual))
            {
                return;
            }
            if (awareAreaEqual == "实测面积")
            {
                awareAreaEqualActual = true;  //此时确权面积等于实测面积
            }
            else
            {
                awareAreaEqualActual = false;  //此时确权面积等于二轮合同面积
            }
        }

        /// <summary>
        /// 地力等级改变
        /// </summary>
        private void cmbLandLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            landLevel = cmbLandLevel.SelectedItem as Dictionary;
            if (landLevel == null)
            {
                return;
            }
        }

        /// <summary>
        /// 是否基本农田改变
        /// </summary>
        private void cmbIsFarmer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //isFamer = cmbIsFarmer.SelectedItem as Dictionary;
            //if (isFamer == null)
            //{
            //    return;
            //}
            if (cmbIsFarmer.SelectedIndex == 0)
                isFamer = true;
            else if (cmbIsFarmer.SelectedIndex == 1)
                isFamer = false;
            else
                isFamer = null;
        }

        /// <summary>
        /// 土地用途改变
        /// </summary>
        private void cmbLandPurpose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            landPurpose = cmbLandPurpose.SelectedItem as Dictionary;
            if (landPurpose == null)
            {
                return;
            }
        }

        #endregion

        private void cbReferPersonExpand_Click_1(object sender, RoutedEventArgs e)
        {
            txtReferPerson.IsEnabled = cbReferPersonExpand.IsChecked.Value ? false : true;
        }

        private void cbLandNumber_Checked(object sender, RoutedEventArgs e)
        {
            if (cbLandNumber.IsChecked.Value)
            {
                rbCombination.IsEnabled = true;
                rbNew.IsEnabled = true;
                if (villageInlitialSet)
                {
                    rbCombination.IsChecked = false;
                    rbCombination.IsEnabled = false;
                    rbNew.IsEnabled = true;
                }
                else
                {
                    rbCombination.IsChecked = false;
                }
                cbLandNumberByUpdown.IsEnabled = true;

            }
            else
            {
                cbLandNumberByUpdown.IsEnabled = false;
                rbCombination.IsEnabled = false;
                rbNew.IsEnabled = false;
            }
        }



        public bool CanCofirmExcute()
        {
            return count != 0;
        }

        public void CofirmExcute()
        {
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            count++;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            count--;
        }

        private void cmb_SecondLandType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            landName = cmb_SecondLandType.SelectedItem as Dictionary;
            if (landName == null)
            {
                return;
            }
        }

        private void cmbQSXZ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            qsxz = cmbQSXZ.SelectedItem as Dictionary;
            if (qsxz == null)
            {
                return;
            }
        }

        private void rbNew_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            cbLandNumber.IsChecked = btn.IsChecked;

        }

        //private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        //private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
