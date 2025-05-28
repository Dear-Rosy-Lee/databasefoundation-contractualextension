/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Command;
using YuLinTu.Component.MapFoundation;
using YuLinTu.Data;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "承包台账",
        Description = "承包方和承包地块管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Component.ContractAccount;component/Resources/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/ContractLand78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf279",
        IsNeedAuthenticated = true)]
    public partial class ContractAccountFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包台账命令
        /// </summary>
        private ContractAccountCommand command = new ContractAccountCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                contractAccountPanel.CurrentZone = value;
            }
        }

        /// <summary>
        /// 是否需要授权
        /// </summary>
        public override bool IsNeedAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string LandType { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数,初始化主界面
        /// </summary>
        public ContractAccountFramePage()
        {
            InitializeComponent();
            contractAccountPanel.MenueEnableMethod += SetControlsEnable;
            SingleInstance = true;
            NavigatorType = eNavigatorType.TreeView;
            InitializeLandControl();
        }

        private void InitializeLandControl()
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                var message = new TabMessageBoxDialog
                {
                    Header = "连接数据源",
                    Message = DataBaseSource.ConnectionError,
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                };
                if (Workpage == null)
                {
                    Log.WriteError(this, "提示", DataBaseSource.ConnectionError);
                    return;
                }
                Workpage.Page.ShowMessageBox(message);
                return;
            }
            DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
            List<Dictionary> dictList = dictBusiness.GetAll();
            if (dictList == null || dictList.Count == 0)
            {
                return;
            }
            List<Dictionary> listDKLB = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DKLB && !string.IsNullOrEmpty(c.Code));
            var spLandType = btnLandType.DropDownContent as StackPanel;
            listDKLB.Sort((a, b) =>
            {
                int aNumber = int.Parse(a.Code);
                int bNumber = int.Parse(b.Code);
                return aNumber.CompareTo(bNumber);
            });
            for (int i = 0; i < listDKLB.Count; i++)
            {
                MetroButton btn = CreateButton(listDKLB[i]);
                spLandType.Children.Add(btn);
            }
        }

        /// <summary>
        /// 创建按钮
        /// </summary>
        /// <returns></returns>
        private MetroButton CreateButton(Dictionary landtype)
        {
            MetroButton btnmetroButton = new MetroButton();
            ImageTextItem textItem = new ImageTextItem();
            textItem.ImagePosition = eDirection.Left;
            textItem.Text = landtype.Name;
            textItem.ToolTip = "仅显示" + landtype.AliseName + "信息";
            switch (landtype.Code)
            {
                case "10":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/承包地32.png"));
                    //SetCommandBinding(btnmetroButton, command.ContractLand, command.ContractLandBind);
                    break;

                case "21":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/自留地32.png"));
                    //SetCommandBinding(btnmetroButton, command.OwnerLand, command.OwnerLandBind);
                    break;

                case "22":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/机动地32.png"));
                    //SetCommandBinding(btnmetroButton, command.MotorLand, command.MotorLandBind);
                    break;

                case "23":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/集体农用地.png"));
                    //SetCommandBinding(btnmetroButton, command.CultivateLand, command.CultivateLandBind);
                    break;

                case "3":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/经济地块.png"));
                    //SetCommandBinding(btnmetroButton, command.EconomyLand, command.EconomyLandBind);
                    break;

                case "8":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/四荒地.png"));
                    //SetCommandBinding(btnmetroButton, command.WasteLand, command.WasteLandBind);
                    break;

                case "99":
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/其它土地.png"));
                    //SetCommandBinding(btnmetroButton, command.OtherLand, command.OtherLandBind);
                    break;

                default:
                    textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/其它土地.png"));
                    //SetCommandBinding(btnmetroButton, command.OtherLand, command.OtherLandBind);
                    break;
            }
            btnmetroButton.Tag = landtype.Code;
            btnmetroButton.Padding = new Thickness(8, 4, 8, 4);
            btnmetroButton.VerticalContentAlignment = VerticalAlignment.Stretch;
            btnmetroButton.HorizontalContentAlignment = HorizontalAlignment.Left;
            btnmetroButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            btnmetroButton.Content = textItem;
            btnmetroButton.Click += BtnmetroButton_Click;
            return btnmetroButton;
        }

        private void BtnmetroButton_Click(object sender, RoutedEventArgs e)
        {
            string code = (string)(sender as MetroButton).Tag;
            contractAccountPanel.LandTypeFilter(code);

            btnLandType.IsOpen = false;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 初始化设置按钮绑定和针对承包台账数据界面进行设置
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();
            SetCommandToControl();
            contractAccountPanel.TheWorkPage = Workpage;
            contractAccountPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(mbtnAdd, command.ContractLandAdd, command.ContractLandAddBind);
            SetCommandBinding(mbtnEdit, command.ContractLandEdit, command.ContractLandEditBind);
            SetCommandBinding(mbtnDel, command.ContractLandDel, command.ContractLandDelBind);
            SetCommandBinding(mbtnImportLandExcel, command.ImportLandExcel, command.ImportLandExcelBind);
            SetCommandBinding(mbtnImportLandTiesExcel, command.ImportLandTiesExcel, command.ImportLandTiesExcelBind);
            //SetCommandBinding(mbtnImportSurveyFormExcel, command.ImportSurveyFormExcel, command.ImportSurveyFormExcelBind);
            SetCommandBinding(mbtnImportBoundaryExcel, command.ImportBoundaryExcel, command.ImportBoundaryExcelBind);
            SetCommandBinding(mbtnImportVector, command.ImportVector, command.ImportVectorBind);
            SetCommandBinding(mbtnImportPoints, command.ImportPoints, command.ImportPointsBind);
            SetCommandBinding(mbtnImportLines, command.ImportLines, command.ImportLinesBind);
            SetCommandBinding(mbtnImportZip, command.ImportZip, command.ImportZipBind);
            SetCommandBinding(mbtnExportVectorShape, command.ExportVectorShape, command.ExportVectorShapeBind);
            SetCommandBinding(mbtnExportPoints, command.ExportVectorDotShape, command.ExportVectorDotShapeBind);
            SetCommandBinding(mbtnExportLines, command.ExportVectorCoilShape, command.ExportVectorCoilShapeBind);
            SetCommandBinding(mbtnSenderExcelTemplate, command.SenderExcelTemplate, command.SenderExcelTemplateBind);
            SetCommandBinding(mbtnVPExcelTemplate, command.VPExcelTemplate, command.VPExcelTemplateBind);
            SetCommandBinding(mbtnLandExcelTemplate, command.LandExcelTemplate, command.LandExcelTemplateBind);
            SetCommandBinding(mbtnBoundaryExcelTemplate, command.BoundaryExcelTemplate, command.BoundaryExcelTemplateBind);
            SetCommandBinding(mbtnSenderWordTemplate, command.SenderWordTemplate, command.SenderWordTemplateBind);
            SetCommandBinding(mbtnVPWordTemplate, command.VPWordTemplate, command.VPWordTemplateBind);
            SetCommandBinding(mbtnLandWordTemplate, command.LandWordTemplate, command.LandWordTemplateBind);
            SetCommandBinding(btnExportSenderExcel, command.ExportSenderExcel, command.ExportSenderExcelBind);
            SetCommandBinding(btnExportVPExcel, command.ExportVPExcel, command.ExportVPExcelBind);
            SetCommandBinding(btnExportLandExcel, command.ExportLandExcel, command.ExportLandExcelBind);
            SetCommandBinding(btnExportBoundaryExcel, command.ExportBoundaryExcel, command.ExportBoundaryExcelBind);
            SetCommandBinding(btnExportPublishExcel, command.ExportPublishExcel, command.ExportPublishExcelBind);
            SetCommandBinding(btnExportVerifyExcel, command.ExportVerifyExcel, command.ExportVerifyExcelBind);
            SetCommandBinding(btnExportVerifyExcelPrint, command.ExportVerifyExcelPrint, command.ExportVerifyExcelPrintBind);
            SetCommandBinding(btnExportSenderWord, command.ExportSenderWord, command.ExportSenderWordBind);
            SetCommandBinding(btnExportVPWord, command.ExportVPWord, command.ExportVPWordBind);
            SetCommandBinding(btnExportLandWord, command.ExportLandWord, command.ExportLandWordBind);
            SetCommandBinding(btnExportPublishWord, command.ExportPublishWord, command.ExportPublishWordBind);
            SetCommandBinding(btnExportAccount, command.ExportAccount, command.ExportAccountBind);
            SetCommandBinding(btnExportContractInformation, command.ExportContractInformation, command.ExportContractInformationBind);
            SetCommandBinding(btnExportSingle, command.ExportSingle, command.ExportSingleBind);
            //SetCommandBinding(btnExportPublish, command.ExportPublish, command.ExportPublishBind);
            //SetCommandBinding(btnExportSign, command.ExportSign, command.ExportSignBind);
            SetCommandBinding(btnExportVillagesPub, command.ExportVillagesPub, command.ExportVillagesPubBind);
            SetCommandBinding(btnExportVillagesDeclare, command.ExportVillagesDeclare, command.ExportVillagesDeclareBind);
            SetCommandBinding(btnExportSingleAffirm, command.ExportSingleAffirm, command.ExportSingleAffirmBind);
            SetCommandBinding(btnExportSummary, command.ExportSummary, command.ExportSummaryBind);
            SetCommandBinding(btnExportCategorySummary, command.CategorySummary, command.ExportCategoryBind);
            SetCommandBinding(btnSearchNeighor, command.SearchNeighor, command.SearchNeighorBind);
            SetCommandBinding(btnResultExcelExpot, command.ResultExcelExpot, command.ResultExcelExpotBind);
            //SetCommandBinding(btnParcelSetting, command.ParcelSetting, command.ParcelSettingBind);
            //SetCommandBinding(btnMutiParcelExportSamll, command.MultiParcelSmallExport, command.MultiParcelExportSmallBind);
            SetCommandBinding(btnMultiParcelExport, command.MultiParcelExport, command.MultiParcelExportBind);
            SetCommandBinding(btnExportVerifyExcelSingle, command.ExportVerifyExcelSingle, command.ExportVerifyExcelSingleBind);
            // 确权确股命令绑定
            SetCommandBinding(btnMultiParcelStockImage, command.MultiParcelStockExport, command.MultiParcelStockExportBind);

            SetCommandBinding(btnAllLand, command.AllLand, command.AllLandBind);
            //SetCommandBinding(btnContractLand, command.ContractLand, command.ContractLandBind);
            //SetCommandBinding(btnOwnerLand, command.OwnerLand, command.OwnerLandBind);
            //SetCommandBinding(btnMotorLand, command.MotorLand, command.MotorLandBind);
            //SetCommandBinding(btnCultivateLand, command.CultivateLand, command.CultivateLandBind);
            //SetCommandBinding(btnOtherLand, command.OtherLand, command.OtherLandBind);
            //SetCommandBinding(btnEconomyLand, command.EconomyLand, command.EconomyLandBind);
            //SetCommandBinding(btnFodderLand, command.FodderLand, command.FodderLandBind);
            //SetCommandBinding(btnWasteLand, command.WasteLand, command.WasteLandBind);
            SetCommandBinding(btnInitial, command.Initial, command.InitialBind);
            SetCommandBinding(btnBoundaryData, command.BoundaryData, command.BoundaryDataBind);
            SetCommandBinding(btnLandCoilData, command.LandCoilData, command.LandCoilBind);
            SetCommandBinding(btnMapNumber, command.MapNumber, command.MapNumberBind);
            SetCommandBinding(btnAreaMeasurement, command.AreaMeasurement, command.AreaMeasurementBind);
            SetCommandBinding(btnSetAreaNumericFormat, command.AreaNumericFormat, command.AreaNumericFormatBind);
            SetCommandBinding(btnFarmerLand, command.FarmerLand, command.FarmerLandBind);
            //SetCommandBinding(btnGetLandCode, command.GetLandCode, command.GetLandCodeBind);
            SetCommandBinding(btnLandNameNull, command.LandNameNull, command.LandNameNullBind);
            SetCommandBinding(btnContractAreaNull, command.ContractAreaNull, command.ContractAreaNullBind);
            SetCommandBinding(btnActualAreaNull, command.ActualAreaNull, command.ActualAreaNullBind);
            SetCommandBinding(btnAwareAreaNull, command.AwareAreaNull, command.AwareAreaNullBind);
            SetCommandBinding(btnFarmerLandNull, command.FarmerLandNull, command.FarmerLandNullBind);
            SetCommandBinding(btnLandLevelNull, command.LandLevelNull, command.LandLevelNullBind);
            SetCommandBinding(btnLandShapeNull, command.LandShapeNull, command.LandShapeNullBind);
            SetCommandBinding(mbtnFind, command.Find, command.FindBind);
            //SetCommandBinding(mbtnExportAllDelayTable, command.ExportAllDelayTable, command.ExportAllDelayTableBind);
            SetCommandBinding(mbtnDataQuality, command.DataQuality, command.DataQualityBind);
            //SetCommandBinding(mbtnAdjustLand, command.AdjustLand, command.AdjustLandBind);
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);
            SetCommandBinding(mbtnDownLoad, command.DownLoad, command.DownLoadBind);
            SetCommandBinding(mbtnUpdateData, command.UpdateData, command.UpdateDataBind);
            SetCommandBinding(mbtnExportZip, command.ExportPackage, command.ExportPackageBind);
        }

        /// <summary>
        /// 创建命令绑定
        /// </summary>
        private void SetCommandBinding(MetroButton button, RoutedCommand cmd, CommandBinding bind)
        {
            bind.CanExecute += CommandBinding_CanExecute;
            bind.Executed += CommandBinding_Executed;
            button.CommandBindings.Add(bind);
            button.Command = cmd;
            button.CommandParameter = cmd.Name;   //设置命令参数
        }

        /// <summary>
        /// 是否可以执行命令
        /// </summary>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Source != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnLandType.IsOpen = false;
            btnInitialData.IsOpen = false;
            btnImportExcel.IsOpen = false;
            btnSurveyTable.IsOpen = false;
            btnFilterData.IsOpen = false;
            btnAccountTable.IsOpen = false;
            //btnCadastralData.IsOpen = false;
            btnImportData.IsOpen = false;
            //btnParcelLand.IsOpen = false;
            btnLandImage.IsOpen = false;
            btnExportData.IsOpen = false;
            btnTemplate.IsOpen = false;
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case ContractAccountCommand.ContractLandAddName:
                    contractAccountPanel.ContractLandAdd();//添加承包地块
                    break;

                case ContractAccountCommand.ContractLandEditName:
                    contractAccountPanel.ContractLandEdit();//编辑承包地块
                    break;

                case ContractAccountCommand.ContractLandDelName:
                    contractAccountPanel.ContractLandDel();//删除承包地块
                    break;

                case ContractAccountCommand.ImportLandExcelName:
                    contractAccountPanel.ImportLandExcel();//导入地块调查表
                    break;

                case ContractAccountCommand.ImportLandTiesExcelName:
                    contractAccountPanel.ImportLandTiesExcel();//导入调查成果表
                    break;

                case ContractAccountCommand.ImportSurveyFormExcelName:
                    contractAccountPanel.ImportSurveyFormExcel();//导入摸底调查表
                    break;

                case ContractAccountCommand.ImportBoundaryExcelName:
                    contractAccountPanel.ImportBoundaryData();//导入界址调查表
                    break;

                case ContractAccountCommand.ImportVectorName:
                    contractAccountPanel.ImportVectorName();//导入地块图斑
                    break;

                case ContractAccountCommand.ImportPointsName:
                    contractAccountPanel.ImportBoundaryAddressDot();//导入界址点图斑数据
                    break;

                case ContractAccountCommand.ImportLinesName:
                    contractAccountPanel.ImportBoundaryAddressCoil();//导入界址线
                    break;

                case ContractAccountCommand.ImportZipName:
                    contractAccountPanel.ImportZipData();//导入压缩包数据
                    break;

                case ContractAccountCommand.ExportLandShapeDataName:
                    contractAccountPanel.ExportLandShapeData();//导出地域下地块图斑
                    break;

                case ContractAccountCommand.ExportLandDotShapeDataName:
                    contractAccountPanel.ExportLandDotShapeData();//导出地域下地块界址点图斑
                    break;

                case ContractAccountCommand.ExportLandCoilShapeDataName:
                    contractAccountPanel.ExportLandCoilShapeData();//导出地域下地块界址线图斑
                    break;

                case ContractAccountCommand.SenderExcelTemplateName:
                    contractAccountPanel.ExportSenderSurveyExcelTemplate();//导出发包方
                    break;

                case ContractAccountCommand.VPExcelTemplateName:
                    contractAccountPanel.ExportVirtualPersonSurveyExcelTemplate();
                    break;

                case ContractAccountCommand.VPWordTemplateName:
                    contractAccountPanel.ExportVirtualPersonSurveyWordTemplate();
                    break;

                case ContractAccountCommand.LandExcelTemplateName:
                    contractAccountPanel.ExportContractLandSurveyExcelTemplate();
                    break;

                case ContractAccountCommand.BoundaryExcelTemplateName:
                    contractAccountPanel.ExportBoundarySurveyExcelTemplate();
                    break;

                case ContractAccountCommand.SenderWordTemplateName:
                    contractAccountPanel.ExportSenderSurveyWordTemplate();
                    break;

                case ContractAccountCommand.LandWordTemplateName:
                    contractAccountPanel.ExportContractLandSurveyWordTemplate();
                    break;

                case ContractAccountCommand.ExportSenderExcelName:
                    contractAccountPanel.ExportSenderExcel();
                    break;

                case ContractAccountCommand.ExportVPExcelName:
                    contractAccountPanel.ExportVPExcel();
                    break;

                case ContractAccountCommand.ExportLandExceleName:
                    contractAccountPanel.ExportAccountLandNameExcel(false);
                    break;

                case ContractAccountCommand.ExportBoundaryExcelName:   //界址信息调查表
                    contractAccountPanel.ExportBoundaryInfoExcel();
                    break;

                case ContractAccountCommand.ExportPublishExcelName:
                    contractAccountPanel.ExportPublishExcel();
                    break;

                case ContractAccountCommand.ExportVerifyExcelName:
                    contractAccountPanel.ExportVerifyExcel();
                    break;

                case ContractAccountCommand.ExportVerifyExcelPrintName:
                    contractAccountPanel.ExportVerifyPrintExcel();
                    break;

                case ContractAccountCommand.ExportSenderWordName:
                    contractAccountPanel.ExportSenderWord();
                    break;

                case ContractAccountCommand.ExportVPWordName:
                    contractAccountPanel.ExportVPWord();
                    break;

                case ContractAccountCommand.ExportLandWordName:
                    contractAccountPanel.ExportLandWord();
                    break;

                case ContractAccountCommand.ExportPublishWordName:
                    contractAccountPanel.ExportPublishWord();
                    break;

                case ContractAccountCommand.ExportAccountName:
                    contractAccountPanel.ExportAccountNameExcel();
                    break;

                case ContractAccountCommand.ExportContractInformationName:
                    contractAccountPanel.ExportContractInformationExcel();
                    break;

                case ContractAccountCommand.ExportSingleName:
                    contractAccountPanel.ExportLandSingleSurveyTable();
                    break;

                case ContractAccountCommand.ExportPublishName:
                    contractAccountPanel.ExportLandRegPubTable();
                    break;

                case ContractAccountCommand.ExportSignName:
                    contractAccountPanel.ExportRegSignTable();
                    break;

                case ContractAccountCommand.ExportVillagesPubName:
                    contractAccountPanel.ExportVillageGroupTable();
                    break;

                case ContractAccountCommand.ExportVillagesDeclareName:
                    contractAccountPanel.ExportVillagesDeclare();
                    break;

                case ContractAccountCommand.ExportSingleAffirmName:
                    contractAccountPanel.ExportFamilyConfirmTable();
                    break;

                case ContractAccountCommand.ExportSummaryName:
                    contractAccountPanel.ExportSummaryExcel();//数据汇总表
                    break;

                case ContractAccountCommand.ExportCategorySummary:
                    contractAccountPanel.ExportCategorySummary();//地块类别汇总表
                    break;

                case ContractAccountCommand.SearchNeighorName:
                    contractAccountPanel.SeekLandNeighbor();
                    break;

                case ContractAccountCommand.ResultExcelExpotName:
                    contractAccountPanel.BoundaryAddressDotResultExport();  //导出界址点成果表
                    break;

                case ContractAccountCommand.ParcelSettingName:
                    break;

                case ContractAccountCommand.MultiParcelExportName:
                    contractAccountPanel.MultiParcelExport();    //导出地块示意图
                    break;

                case ContractAccountCommand.MultiParcelStockExportName:
                    contractAccountPanel.MultiParcelStockExport();    // 导出确权确股地块示意图
                    break;

                case ContractAccountCommand.AllLandName:
                    btnLandType.IsOpen = false;
                    contractAccountPanel.AllLandFilter();
                    break;
                //case ContractAccountCommand.ContractLandName:
                //    //contractAccountPanel.LandTypeFilter();
                //    contractAccountPanel.ContractLandFilter();
                //    break;
                //case ContractAccountCommand.OwnerLandName:
                //    contractAccountPanel.OwnerLandFilter();
                //    break;
                //case ContractAccountCommand.MotorLandName:
                //    contractAccountPanel.MotorLandFilter();
                //    break;
                //case ContractAccountCommand.CultivateLandName:
                //    contractAccountPanel.CultivateLandFilter();
                //    break;
                //case ContractAccountCommand.OtherLandName:
                //    contractAccountPanel.OtherLandFilter();
                //    break;
                //case ContractAccountCommand.EconomyLandName:
                //    contractAccountPanel.EconomyLandFilter();
                //    break;
                //case ContractAccountCommand.FodderLandName:
                //    contractAccountPanel.FodderLandFilter();
                //    break;
                //case ContractAccountCommand.WasteLandName:
                //    contractAccountPanel.WasteLandFilter();
                //    break;
                case ContractAccountCommand.InitialName:
                    contractAccountPanel.InitialLandInfo();
                    break;

                case ContractAccountCommand.BoundaryDataName:
                    contractAccountPanel.InitializeLandDotCoilInfo();
                    break;

                case ContractAccountCommand.LandCoilName:
                    contractAccountPanel.InitializeLandCoilInfo();//根据有效界址点初始化界址线
                    break;

                case ContractAccountCommand.MapNumberName:
                    contractAccountPanel.InitialMapNumber();  //初始化图幅编号
                    break;

                case ContractAccountCommand.AreaMeasurementName:
                    contractAccountPanel.InitialArea();
                    break;

                case ContractAccountCommand.AreaNumericFormatName:
                    contractAccountPanel.SetAreaNumericFormat();
                    break;

                case ContractAccountCommand.ExportVerifyExcelSingleName:
                    contractAccountPanel.ExportVerifyExcelSingle();
                    break;

                case ContractAccountCommand.FarmerLandName:
                    contractAccountPanel.InitialIsFarmer();
                    break;

                case ContractAccountCommand.GetLandCodeName:
                    contractAccountPanel.GetLandCode();
                    break;

                case ContractAccountCommand.LandNameNullName:
                    contractAccountPanel.LandNameNullSearch();
                    break;

                case ContractAccountCommand.ContractAreaNullName:
                    contractAccountPanel.ContractAreaNullSearch();
                    break;

                case ContractAccountCommand.ActualAreaNullName:
                    contractAccountPanel.ActualAreaNullSearch();
                    break;

                case ContractAccountCommand.AwareAreaNullName:
                    contractAccountPanel.AwareAreaNullSearch();
                    break;

                case ContractAccountCommand.FarmerLandNullName:
                    contractAccountPanel.FarmerLandNullSearch();
                    break;

                case ContractAccountCommand.LandLevelNullName:
                    contractAccountPanel.LandLevelNullSearch();
                    break;

                case ContractAccountCommand.LandShapeNullName:
                    contractAccountPanel.LandShapeNullSearch();
                    break;

                case ContractAccountCommand.FindName:
                    FindLand();    //空间查看
                    break;

                case ContractAccountCommand.ExportAllDelayTableName:
                    contractAccountPanel.ExportAllDelayTable();
                    break;

                case ContractAccountCommand.DataQualityName:
                    contractAccountPanel.DataQuality();    
                    break;

                case ContractAccountCommand.AdjustLandName:
                    contractAccountPanel.AdjustLand();
                    break;

                case ContractAccountCommand.ClearName:
                    contractAccountPanel.Clear();
                    break;

                case ContractAccountCommand.RefreshName:
                    contractAccountPanel.Refresh();
                    break;

                case ContractAccountCommand.DownLoadName:
                    contractAccountPanel.DownLoadData();
                    break;

                case ContractAccountCommand.UpdateDataName:
                    contractAccountPanel.UpLoadData();
                    break;

                case ContractAccountCommand.ExportPackageName:
                    contractAccountPanel.ExportZipData();
                    break;

                default:
                    break;
            }
        }

        #endregion Methods

        #region Method - 辅助方法

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                mbtnAdd.IsEnabled = isEnable;
                mbtnEdit.IsEnabled = isEnable;
                mbtnDel.IsEnabled = isEnable;
                mbtnClear.IsEnabled = isEnable;
                mbtnFind.IsEnabled = isEnable;
                btnLandType.IsEnabled = isEnable;
                btnInitialData.IsEnabled = isEnable;
                btnImportExcel.IsEnabled = isEnable;
                btnSurveyTable.IsEnabled = isEnable;
                btnFilterData.IsEnabled = isEnable;
                btnAccountTable.IsEnabled = isEnable;
                //btnCadastralData.IsEnabled = isEnable;
                btnImportData.IsEnabled = isEnable;
                //btnParcelLand.IsEnabled = isEnable;
                btnExportData.IsEnabled = isEnable;
                btnTemplate.IsEnabled = isEnable;
                btnExportContractInformation.IsEnabled = isEnable;
                mbtnDataQuality.IsEnabled = isEnable;
                //mbtnAdjustLand.IsEnabled = isEnable;
            }));
        }

        /// <summary>
        /// 查找空间地块信息
        /// </summary>
        private void FindLand()
        {
            try
            {
                List<ContractLand> lands = contractAccountPanel.FindGeometry();
                if (lands == null)
                {
                    return;
                }
                var mapPage = Workpage.Workspace.Workpages.FirstOrDefault(c => c.Page.Content is YuLinTuMapFoundation);   //获取鱼鳞图地图工作页
                if (mapPage == null)
                {
                    //此时当前工作空间中没有打开鱼鳞图地图模块(工作页)
                    //ShowBox(ContractAccountInfo.FindGeometryTool, "未打开鱼鳞图地图模块,请打开后再做空间查看操作!");
                    //return;
                    mapPage = Workpage.Workspace.AddWorkpage<YuLinTuMapFoundation>();
                }
                mapPage.Page.Activate();   //激活鱼鳞图地图工作页
                mapPage.Message.Send(this,
                    MessageExtend.ContractAccountMsg(CreateDb(), ContractAccountMessage.CONTRACTACCOUNT_FINDLANDS_COMPLETE, lands));
            }
            catch (Exception ex)
            {
                ShowBox(ContractAccountInfo.FindGeometryTool, "空间查看失败");
                YuLinTu.Library.Log.Log.WriteException(this, "FindLand(空间查看失败失败!)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <returns></returns>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 提示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var showDlg = new TabMessageBoxDialog()
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            };
            Workpage.Page.ShowMessageBox(showDlg);
        }

        #endregion Method - 辅助方法

        #region Methods - 编辑地块示意图

        private EditLandParcelDialog dlg = null;
        private EditPersonLandParcelDialog pdlg = null;

        private bool isFirst = true;
        private bool isPersonFirst = true;

        private void mbtnEditDiagram_Click(object sender, RoutedEventArgs e)
        {
            var land = contractAccountPanel.CurrentLandBinding;
            var person = contractAccountPanel.CurrentAccountItem;

            if (land == null)
            {
                if (person != null)
                {
                    if (pdlg == null)
                    {
                        pdlg = new EditPersonLandParcelDialog(person.Children.Count);
                        pdlg.ExceptionAct += (ex) =>
                        {
                            Workpage.Page.ShowMessageBox($"保存地块示意图发生错误:" + ex.Message);
                            Log.WriteException(this, "EditPersonLandParcelDialog", ex.ToString());
                        };
                    }

                    pdlg.SelectedLand = person.Children[0].Tag;
                    if (pdlg.personItem == null)
                        pdlg.personItem = person;
                    // if (!isPersonFirst)
                    {
                        if (pdlg.personItem.Tag == person.Tag)
                        {
                            pdlg.personItem = person;
                            pdlg.Refresh();
                            pdlg.ApplyTemplate();
                        }
                        else
                        {
                            pdlg = new EditPersonLandParcelDialog(person.Children.Count);
                            pdlg.SelectedLand = person.Children[0].Tag;
                            pdlg.personItem = person;
                        }
                    }
                    // else
                    //     pdlg.personItem = person;

                    pdlg.contractAccountPanel = contractAccountPanel;
                    Workpage.Page.ShowDialog(pdlg);
                    isPersonFirst = false;
                }
            }
            else
            {
                //if (land == null || land.Tag == null)
                //{
                //    Workpage.Page.ShowDialog(new MessageDialog()
                //    {
                //        Header = "错误",
                //        Message = string.Format("您当前没有选择地块，无法编辑其宗地图。"),
                //        MessageGrade = eMessageGrade.Error
                //    });
                //    return;
                //}
                if (land.Tag.Shape == null)
                {
                    Workpage.Page.ShowDialog(new MessageDialog()
                    {
                        Header = "错误",
                        Message = string.Format("您选择的地块没有空间数据，无法编辑其宗地图。"),
                        MessageGrade = eMessageGrade.Error
                    });
                    return;
                }
                if (!land.Tag.Shape.IsValid())
                {
                    Workpage.Page.ShowDialog(new MessageDialog()
                    {
                        Header = "错误",
                        Message = string.Format("您选择的地块的空间数据有拓扑错误，无法编辑器宗地图，请先将地块修改正确。"),
                        MessageGrade = eMessageGrade.Error
                    });
                    return;
                }

                if (dlg == null)
                    dlg = new EditLandParcelDialog();

                dlg.SelectedLand = land.Tag;
                //dlg.personItem = person;
                if (!isFirst)
                    dlg.Refresh();

                Workpage.Page.ShowDialog(dlg);
                isFirst = false;
            }

            //if (!isFirst)
            //    dlg.Refresh();

            //Workpage.Page.ShowDialog(dlg);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (dlg != null)
                dlg.Dispose();
        }

        #endregion Methods - 编辑地块示意图
    }
}