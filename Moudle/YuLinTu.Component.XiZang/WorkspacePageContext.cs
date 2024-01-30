/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using YuLinTu.Appwork;
using YuLinTu.Component.Concord;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Diagrams;
using System.IO;
using YuLinTu.Common.Office;
using YuLinTu.Component.ContractAccount;
using YuLinTu.Component.ContractRegeditBook;
using System.Collections.ObjectModel;

namespace YuLinTu.Component.XiZangLZ
{
    public class WorkspacePageContext : TheNavigatableWorkpageContext
    {
        #region Fields

        private DropDownButton btnSurveyTable;
        private DropDownButton btnParcelTable;
        private DropDownButton btnAccountTable;
        private DropDownButton btnSingleBook;
        private ContractLandBinding currentLandBinding;
        private ContractLandPersonItem currentAccountItem;
        private ContractRegeditBookItem currentRegiditItem;
        private BindContractRegeditBook currentRegeditBook;//当前选择权证
        private BindConcord currentConcord;
        private ConcordItem currentItem;
        private string zoneName;
        private string ExportBook = "导出审批表";

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone = new Zone();

        /// <summary>
        /// 承包台账主界面
        /// </summary>
        private ContractAccountPanel contractAccountPanel;

        /// <summary>
        /// 承包合同主界面
        /// </summary>
        private ConcordPanel concordPanel;

        /// <summary>
        /// 权证主界面
        /// </summary>
        private ContractRegeditBookPanel contractRegeditBookPanel;

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        /// <summary>
        /// 系统设置
        /// </summary>
        private SystemSetDefine m_systemSet = SystemSetDefine.GetIntence();

        public static ContractConcordSettingDefine concordSetting = ContractConcordSettingDefine.GetIntence();

        public delegate void TaskViewerShowDelegate();

        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public YuLinTu.Library.Business.SystemSetDefine SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                var config = section.Settings as SystemSetDefine;
                return config;
            }
        }

        #endregion Fields

        #region Ctor

        public WorkspacePageContext(IWorkpage workpage) : base(workpage)
        {
        }

        #endregion Ctor

        #region Methods

        #region Methods-Message

        /// <summary>
        /// 注册导航模板
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
        }

        protected override void OnSettingsChanged(object sender, SettingsProfileChangedEventArgs e)
        {
            DataHelper.Refresh();
        }

        /// <summary>
        /// 加载工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnInstalToolbar(object sender, InstallUIElementsEventArgs e)
        {
            //承包合同模块
            ConcordFramePage concordPage = PageContent as ConcordFramePage;
            if (concordPage != null)
            {
                this.concordPanel = concordPage.concordPanel;
                btnSingleBook = e.FindByName<DropDownButton>("mbtnSingleBook");
                // 导出承包合同调查表
                concordPage.mbtnExportConcord.Command = null;
                concordPage.mbtnExportConcord.CommandBindings.Clear();
                concordPage.mbtnExportConcord.CommandParameter = "";
                concordPage.mbtnExportConcord.Click += exportConcord_Click;

                // 预览承包合同
                concordPage.mbtnPreviewConcord.Command = null;
                concordPage.mbtnPreviewConcord.CommandBindings.Clear();
                concordPage.mbtnPreviewConcord.CommandParameter = "";
                concordPage.mbtnPreviewConcord.Click += previewConcord_Click;

                //预览单户申请书
                concordPage.mbtnPrintRequireBook.Command = null;
                concordPage.mbtnPrintRequireBook.CommandBindings.Clear();
                concordPage.mbtnPrintRequireBook.Click += PrintRequireBook;

                //导出单户申请书
                concordPage.mbtnExportApplicationByFamily.Command = null;
                concordPage.mbtnExportApplicationByFamily.CommandBindings.Clear();
                concordPage.mbtnExportApplicationByFamily.Click += ExportRequireBook;

                concordPage.OtherContainer.Visibility = Visibility.Visible;
                var superBtndjspb = CreatedjspbDropDownButton();
                //添加登记审批表下拉控件
                concordPage.OtherContainer.Items.Add(superBtndjspb);
            }
            //承包台账模块
            ContractAccountFramePage contractAccountPage = PageContent as ContractAccountFramePage;
            if (contractAccountPage != null)
            {
                this.contractAccountPanel = contractAccountPage.contractAccountPanel;
                //导出承包地块调查表Excel,旧模板导出，暂时不用
                //contractAccountPage.btnExportLandExcel.Command = null;
                //contractAccountPage.btnExportLandExcel.CommandBindings.Clear();
                //contractAccountPage.btnExportLandExcel.CommandParameter = "";
                //contractAccountPage.btnExportLandExcel.Click += btnExportLandExcel_Click;

                //导出承包地块调查表Word
                contractAccountPage.btnExportLandWord.Command = null;
                contractAccountPage.btnExportLandWord.CommandBindings.Clear();
                contractAccountPage.btnExportLandWord.CommandParameter = "";
                contractAccountPage.btnExportLandWord.Click += btnExportLandExcel_Click;

                //导出承包方调查表
                contractAccountPage.btnExportVPWord.Command = null;
                contractAccountPage.btnExportVPWord.CommandBindings.Clear();
                contractAccountPage.btnExportVPWord.CommandParameter = "";
                contractAccountPage.btnExportVPWord.Click += btnExportVPWord_Click;

                //导出发包方调查表
                contractAccountPage.btnExportSenderWord.Command = null;
                contractAccountPage.btnExportSenderWord.CommandBindings.Clear();
                contractAccountPage.btnExportSenderWord.CommandParameter = "";
                contractAccountPage.btnExportSenderWord.Click += btnSenderTable_Click;

                //公示结果归户表
                contractAccountPage.btnExportPublishWord.Command = null;
                contractAccountPage.btnExportPublishWord.CommandBindings.Clear();
                contractAccountPage.btnExportPublishWord.CommandParameter = "";
                contractAccountPage.btnExportPublishWord.Click += btnAgriLandTable_Click;

                //创建按钮--公示结果归户表
                btnSurveyTable = contractAccountPage.btnSurveyTable;// e.FindByName<DropDownButton>("btnSurveyTable"); //btnInitial
                //SeparatorV sepv = new SeparatorV() { Height = 1, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0, 1, 0, 0), Margin = new Thickness(3, 0, 3, 0) };
                //var spSurveyTable = btnSurveyTable.DropDownContent as StackPanel;
                //var superBtnHandleDispute = CreateSurveyCover();
                //spSurveyTable.Children.Add(sepv);
                //spSurveyTable.Children.Add(superBtnHandleDispute);

                //创建按钮--农户基本信息公示表
                //var gsbBtnHandleDispute = CreateFamilyGSBBtn();
                //spSurveyTable.Children.Add(gsbBtnHandleDispute);

                //创建按钮--登记调查确认书
                //var confirmBookBtnHandleDispute = CreateConfirmBookBtn();
                //spSurveyTable.Children.Add(confirmBookBtnHandleDispute);

                //界址点成果表
                //btnAccountTable = e.FindByName<DropDownButton>("btnAccountTable"); //btnInitial
                //contractAccountPage.btnResultExcelExpot.Command = null;
                //contractAccountPage.btnResultExcelExpot.CommandBindings.Clear();
                //contractAccountPage.btnResultExcelExpot.CommandParameter = "";
                //contractAccountPage.btnResultExcelExpot.Click += btnExportDotResult_Click;

                //创建同步系统设置的地块示意图导出按钮
                //var btnExportParcelBySet = CreatedBtnExportParcelBySet();
                //btnParcelTable = contractAccountPage.btnLandImage;
                //var spAccountTableLand = contractAccountPage.btnLandImage.DropDownContent as StackPanel;
                //spAccountTableLand.Children.Insert(0, btnExportParcelBySet);

                //原导出标准地块示意图逻辑，屏蔽
                //contractAccountPage.btnMultiParcelExport.Command = null;
                //contractAccountPage.btnMultiParcelExport.CommandBindings.Clear();
                //contractAccountPage.btnMultiParcelExport.CommandParameter = "";
                //contractAccountPage.btnMultiParcelExport.Click += MultiParcelExport;

                //导出调查信息公示表
                contractAccountPage.btnExportPublishExcel.Command = null;
                contractAccountPage.btnExportPublishExcel.CommandBindings.Clear();
                contractAccountPage.btnExportPublishExcel.CommandParameter = "";
                contractAccountPage.btnExportPublishExcel.Click += ExportPublishExcel;
            }

            //登记簿
            ContractRegeditBookFramePage contractRegeditBookPage = PageContent as ContractRegeditBookFramePage;
            if (contractRegeditBookPage != null)
            {
                this.contractRegeditBookPanel = contractRegeditBookPage.contractRegeditBookPanel;
                //预览登记簿
                contractRegeditBookPage.mbtnPrivewRegeditBook.Command = null;
                contractRegeditBookPage.mbtnPrivewRegeditBook.CommandBindings.Clear();
                contractRegeditBookPage.mbtnPrivewRegeditBook.CommandParameter = "";
                contractRegeditBookPage.mbtnPrivewRegeditBook.Click += PrivewRegeditBook;

                //导出登记簿
                contractRegeditBookPage.mbtnExportRegeditBook.Command = null;
                contractRegeditBookPage.mbtnExportRegeditBook.CommandBindings.Clear();
                contractRegeditBookPage.mbtnExportRegeditBook.CommandParameter = "";
                contractRegeditBookPage.mbtnExportRegeditBook.Click += ExportRegeditBook;
            }
            //证书
            if (contractRegeditBookPage != null)
            {
                this.contractRegeditBookPanel = contractRegeditBookPage.contractRegeditBookPanel;
                //预览登记簿
                contractRegeditBookPage.mbtnPrintViewWarrant.Command = null;
                contractRegeditBookPage.mbtnPrintViewWarrant.CommandBindings.Clear();
                contractRegeditBookPage.mbtnPrintViewWarrant.CommandParameter = "";
                contractRegeditBookPage.mbtnPrintViewWarrant.Click += PrintViewWarrant;

                //导出登记簿
                contractRegeditBookPage.mbtnExportWarrant.Command = null;
                contractRegeditBookPage.mbtnExportWarrant.CommandBindings.Clear();
                contractRegeditBookPage.mbtnExportWarrant.CommandParameter = "";
                contractRegeditBookPage.mbtnExportWarrant.Click += ExportWarrant;
            }
        }

        #region 创建按钮

        /// <summary>
        /// 创建登记审批表下拉按钮
        /// </summary>
        /// <returns></returns>
        private SuperButton CreatedjspbDropDownButton()
        {
            SuperButton handledjspbBtn = new SuperButton();
            handledjspbBtn.IsSplit = false;
            handledjspbBtn.ImagePosition = eDirection.Top;
            handledjspbBtn.DropDownPosition = eDirection.Bottom;
            handledjspbBtn.Padding = new Thickness(4, 2, 4, 2);
            handledjspbBtn.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/htdjspb.png"));
            handledjspbBtn.Content = "登记审批表";
            handledjspbBtn.ToolTip = "登记审批表";

            var menu = new ContextMenu() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left };

            var previewMenu = new MenuItem() { };
            previewMenu.Icon = new Image
            {
                Source = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/MeasureBook.png")),
            };
            previewMenu.Header = "预览登记审批表";
            previewMenu.ToolTip = "预览登记审批表";
            previewMenu.Click += btnPriviewDJSPB_Click;

            var exportMenu = new MenuItem() { };
            exportMenu.Icon = new Image
            {
                Source = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/导出word_32.png")),
            };
            exportMenu.Header = "导出登记审批表";
            exportMenu.ToolTip = "导出登记审批表";
            exportMenu.Click += btnExportDJSPB_Click;

            menu.Items.Add(previewMenu);
            menu.Items.Add(exportMenu);

            handledjspbBtn.DropDownMenu = menu;

            return handledjspbBtn;
        }

        /// <summary>
        /// 登记调查确认书
        /// </summary>
        /// <returns></returns>
        private MetroButton CreateConfirmBookBtn()
        {
            MetroButton btnConfirmBook = new MetroButton();
            ImageTextItem textItem = new ImageTextItem();
            textItem.ImagePosition = eDirection.Left;
            textItem.Text = "登记调查确认书";
            textItem.ToolTip = "登记调查确认书";
            textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Excel.png"));
            SetDefaultSytle(btnConfirmBook);
            btnConfirmBook.Content = textItem;
            btnConfirmBook.Click += btnExportConfirmBook_Click;
            return btnConfirmBook;
        }

        /// <summary>
        ///创建农户基本信息公示表按钮
        /// </summary>
        private MetroButton CreateFamilyGSBBtn()
        {
            MetroButton btnFamilyGSB = new MetroButton();
            ImageTextItem textItem = new ImageTextItem();
            textItem.ImagePosition = eDirection.Left;
            textItem.Text = "农户基本信息公示表";
            textItem.ToolTip = "农户基本信息公示表";
            textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Excel.png"));

            SetDefaultSytle(btnFamilyGSB);
            btnFamilyGSB.Content = textItem;
            btnFamilyGSB.Click += btnFamilyGSB_Click;
            return btnFamilyGSB;
        }

        /// <summary>
        ///创建根据设置导出地块示意图按钮
        /// </summary>
        private MetroButton CreatedBtnExportParcelBySet()
        {
            MetroButton btnExportParcelBySet = new MetroButton();
            ImageTextItem textItem = new ImageTextItem();
            textItem.ImagePosition = eDirection.Left;
            textItem.Text = "地块示意图（西藏林芝）";
            textItem.ToolTip = "地块示意图";
            textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/BoundaryLine.png"));

            SetDefaultSytle(btnExportParcelBySet);
            btnExportParcelBySet.Content = textItem;
            btnExportParcelBySet.Click += btnExportParcelBySet_Click;
            return btnExportParcelBySet;
        }

        /// <summary>
        ///创建公示结果归户表按钮
        /// </summary>
        private MetroButton CreateSurveyCover()
        {
            MetroButton btnSurveyCover = new MetroButton();
            ImageTextItem textItem = new ImageTextItem();
            textItem.ImagePosition = eDirection.Left;
            textItem.Text = "公示结果归户表(西藏)";
            textItem.ToolTip = "公示结果归户表(西藏)";
            textItem.Image = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Excel.png"));
            SetDefaultSytle(btnSurveyCover);
            btnSurveyCover.Content = textItem;
            btnSurveyCover.Click += btnAgriLandTable_Click;
            return btnSurveyCover;
        }

        /// <summary>
        /// 设置Button的默认风格
        /// </summary>
        /// <param name="btn"></param>
        private void SetDefaultSytle(MetroButton btn)
        {
            btn.Padding = new Thickness(4, 2, 4, 2);
            btn.VerticalContentAlignment = VerticalAlignment.Stretch;
            btn.HorizontalContentAlignment = HorizontalAlignment.Left;
            btn.Padding = new Thickness(8, 4, 8, 4);
        }

        #endregion 创建按钮

        #endregion Methods-Message

        #region 登记簿

        /// <summary>
        /// 预览登记薄
        /// </summary>
        public void PrivewRegeditBook(object sender, RoutedEventArgs e)
        {
            GetContractRegeditBookSelectItem();
            if (contractRegeditBookPanel.CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            if (currentRegeditBook == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "请选择待预览的登记簿!");
                return;
            }
            ContractConcord concord = contractRegeditBookPanel.ConcordBusiness.Get(currentRegeditBook.Tag.ID);
            if (concord == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "请选择的权证没有合同!");
                return;
            }
            PrivewRegeditBookWord(contractRegeditBookPanel.CurrentZone, concord);
        }

        /// <summary>
        /// 导出登记簿
        /// </summary>
        public void ExportRegeditBook(object sender, RoutedEventArgs e)
        {
            GetContractRegeditBookSelectItem();
            if (contractRegeditBookPanel.CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                var zoneStation = contractRegeditBookPanel.DbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(contractRegeditBookPanel.CurrentZone.FullCode, eLevelOption.Subs);
                if (contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Group || (contractRegeditBookPanel.CurrentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractRegeditBookPanel.Items == null || contractRegeditBookPanel.Items.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //界面选择批量
                    if (contractRegeditBookPanel.IsBatch)
                    {
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = contractRegeditBookPanel.ThePage;
                        foreach (var item in contractRegeditBookPanel.Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        contractRegeditBookPanel.ThePage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportRegeditBookTableNoSelected);
                                return;
                            }
                            ExportDataCommonOperateTask(contractRegeditBookPanel.CurrentZone.FullName, ContractRegeditBookInfo.ExportRegeditBookTable, eContractRegeditBookType.ExportRegeditBookData,
                                ContractRegeditBookInfo.ExportRegeditBookTableDesc, ContractRegeditBookInfo.ExportWarrentData, selectPage.SelectedPersons);
                        });
                    }
                    else
                    {
                        //界面未选择登记簿
                        if (currentRegiditItem == null)
                        {
                            ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.ExportRegeditBookTableNoSelected);
                            return;
                        }
                        else
                        {
                            ExportDataPage extPage = new ExportDataPage(contractRegeditBookPanel.CurrentZone.FullName, contractRegeditBookPanel.ThePage, ContractRegeditBookInfo.ExportRegeditBookTable);
                            extPage.Workpage = contractRegeditBookPanel.ThePage;
                            contractRegeditBookPanel.ThePage.Page.ShowMessageBox(extPage, (b, r) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                                {
                                    return;
                                }
                                string fileDir = extPage.FileName;

                                ContractConcord concord = contractRegeditBookPanel.ConcordBusiness.Get(currentRegeditBook.Tag.ID);
                                if (concord == null)
                                {
                                    ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "选择的权证没有合同!");
                                    return;
                                }
                                PrivewRegeditBookWord(contractRegeditBookPanel.CurrentZone, concord, fileDir, false); //保存
                                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, "导出登记簿成功", eMessageGrade.Infomation);
                            });
                        }
                    }
                }
                else if ((contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Village || contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportDataCommonOperateTask(contractRegeditBookPanel.CurrentZone.FullName, ContractRegeditBookInfo.ExportRegeditBookTable, eContractRegeditBookType.BatchExportRegeditBookData,
                ContractRegeditBookInfo.ExportRegeditBookTableDesc, ContractRegeditBookInfo.ExportWarrentData);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "ExportRegeditBook(导出登记簿)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出登记簿(单个任务)
        /// </summary>
        private void ExportRegeditBookTask(string fileName, string taskDes, string taskName, List<VirtualPerson> listPerson)
        {
            TaskExportRegeditBookArgument meta = new TaskExportRegeditBookArgument();
            meta.FileName = fileName;
            meta.DbContext = contractRegeditBookPanel.DbContext;
            meta.CurrentZone = contractRegeditBookPanel.CurrentZone;
            //meta.DateValue = time;
            //meta.PubDateValue = pubTime;
            meta.SystemDefine = contractRegeditBookPanel.SystemSet;
            meta.SelectedPersons = listPerson;
            TaskExportRegeditBookOperation import = new TaskExportRegeditBookOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(contractRegeditBookPanel.DbContext, "", true));
            });
            contractRegeditBookPanel.ThePage.TaskCenter.Add(import);
            if (contractRegeditBookPanel.ShowTaskViewer != null)
            {
                contractRegeditBookPanel.ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导出登记簿(组任务)
        /// </summary>
        private void ExportRegeditBookTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportRegeditBookArgument groupArgument = new TaskGroupExportRegeditBookArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = contractRegeditBookPanel.DbContext;
            groupArgument.CurrentZone = contractRegeditBookPanel.CurrentZone;
            //meta.DateValue = time;
            //meta.PubDateValue = pubTime;
            groupArgument.SystemDefine = contractRegeditBookPanel.SystemSet;
            TaskGroupExportRegeditBookOperation groupOperation = new TaskGroupExportRegeditBookOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            contractRegeditBookPanel.ThePage.TaskCenter.Add(groupOperation);
            if (contractRegeditBookPanel.ShowTaskViewer != null)
            {
                contractRegeditBookPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 登记簿预览
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <param name="concord">合同</param>
        public void PrivewRegeditBookWord(Zone zone, ContractConcord concord, string fileName = "", bool isPrint = true)
        {
            try
            {
                if (zone == null)
                {
                    return;
                }
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                List<XZDW> listLine = new List<XZDW>(1000);
                List<DZDW> listPoint = new List<DZDW>(1000);
                List<MZDW> listPolygon = new List<MZDW>(1000);
                List<ContractLand> listGeoLand = new List<ContractLand>(1000);
                List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
                List<Dictionary> dictDKLB = new List<Dictionary>(1000);
                List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
                List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
                List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
                DiagramsView viewOfAllMultiParcel = null;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                var PointStation = dbContext.CreateDZDWWorkStation();
                var PolygonStation = dbContext.CreateMZDWWorkStation();
                var dicStation = dbContext.CreateDictWorkStation();
                var VillageZone = GetParent(zone, dbContext);
                var listDict = dicStation.Get();
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                listDot = dotStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(zone.FullCode);
                listPoint = PointStation.GetByZoneCode(zone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(zone.FullCode);
                listLine.RemoveAll(l => l.Shape == null);
                listPoint.RemoveAll(l => l.Shape == null);
                listPolygon.RemoveAll(l => l.Shape == null);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 336;
                    viewOfAllMultiParcel.Paper.Model.Height = 357;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;
                }));

                ContractRegeditBookPrinterData data = new ContractRegeditBookPrinterData(concord);
                data.CurrentZone = zone;
                data.DbContext = contractRegeditBookPanel.DbContext;
                data.SystemDefine = contractRegeditBookPanel.SystemSet;
                data.AccountLandBusiness = contractRegeditBookPanel.AccountLandBusiness;
                data.ConcordBusiness = contractRegeditBookPanel.ConcordBusiness;
                data.DictBusiness = contractRegeditBookPanel.DictBusiness;
                data.PersonBusiness = contractRegeditBookPanel.PersonBusiness;
                data.SystemDefine = contractRegeditBookPanel.SystemSet;
                data.InitializeInnerData();
                string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记簿");
                ContractRegeditBookWork regeditBookWord = new ContractRegeditBookWork(dbContext);
                regeditBookWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                regeditBookWord.CurrentZone = zone;
                regeditBookWord.VillageZone = VillageZone;
                regeditBookWord.DictList = listDict;
                regeditBookWord.DictDKLB = dictDKLB;
                regeditBookWord.ListGeoLand = listGeoLand;
                regeditBookWord.ListLineFeature = listLine;
                regeditBookWord.ListPointFeature = listPoint;
                regeditBookWord.ListPolygonFeature = listPolygon;
                regeditBookWord.DictList = contractRegeditBookPanel.DictBusiness.GetAll();
                regeditBookWord.SavePathOfImage = System.IO.Path.GetTempPath();
                regeditBookWord.OpenTemplate(tempPath);
                if (isPrint)
                {
                    regeditBookWord.PrintPreview(data);
                }
                else
                {
                    VirtualPerson vp = contractRegeditBookPanel.PersonBusiness.Get(concord.ContracterId.Value);
                    string filePath = fileName + @"\" + vp.FamilyNumber + "-" + concord.ContracterName.InitalizeFamilyName() + "-" + concord.ConcordNumber + "-" + TemplateFile.PrivewRegeditBookWord;
                    regeditBookWord.SaveAs(data, filePath);
                }
                data = null;
            }
            catch (Exception ex)
            {
                ShowBox(ContractRegeditBookInfo.ExportRegeditBookTable, ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "(导出数据到Word文档)", ex.Message + ex.StackTrace);
                return;
            }
        }

        #endregion 登记簿

        #region 证书

        /// <summary>
        /// 证书预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintViewWarrant(object sender, RoutedEventArgs e)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            ConcordBusiness concordBusiness = new ConcordBusiness(dbContext);
            DictionaryBusiness DictBusiness = new DictionaryBusiness(dbContext);
            GetContractRegeditBookSelectItem();
            if (currentRegiditItem == null)
            {
                ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "请选择数据项进行证书预览!");
                return;
            }
            try
            {
                var bookStaion = dbContext.CreateRegeditBookStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var zoneStaion = dbContext.CreateZoneWorkStation();
                var zonelist = GetParentZone(contractRegeditBookPanel.CurrentZone, dbContext);
                zonelist.Add(contractRegeditBookPanel.CurrentZone);
                var concords = concordBusiness.GetCollection(currentRegiditItem.Tag.ID);
                if (concords == null || concords.Count == 0)
                {
                    ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "未获取合同数据,无法进行证书的预览!");
                    return;
                }
                List<Guid> ids = new List<Guid>(concords.Count);
                concords.ForEach(c => ids.Add(c.ID));
                var books = bookStaion.GetByConcordIds(ids.ToArray());
                if (books == null || books.Count == 0)
                {
                    ShowBox(ContractRegeditBookInfo.PrintViewWarrant, "未获取权证数据,无法进行证书的预览!");
                    return;
                }
                var lands = landStation.GetByConcordIds(ids.ToArray());
                VirtualPerson vpn = currentRegiditItem.Tag;
                var familyConcord = concords.Find(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                var familyBook = familyConcord == null ? null : books.Find(c => c.ID == familyConcord.ID);
                var otherConcord = concords.Find(c => c.ArableLandType != ((int)eConstructMode.Family).ToString());
                var otherBook = otherConcord == null ? null : books.Find(c => c.ID == otherConcord.ID);
                var parentsToProvince = zoneStaion.GetParentsToProvince(contractRegeditBookPanel.CurrentZone);
                CollectivityTissue Tissue = concordBusiness.GetSenderById(concords[0].SenderId);
                if (Tissue == null)
                {
                    var senderStation = dbContext.CreateSenderWorkStation();
                    Tissue = senderStation.Get(contractRegeditBookPanel.CurrentZone.ID);
                }
                string tempPath = TemplateHelper.WordTemplate("西藏农村土地承包经营权证");
                List<Dictionary> DictList = DictBusiness.GetAll();
                lands.LandNumberFormat(SystemSet);
                ContractWarrantPrinter printContract = new ContractWarrantPrinter();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(printContract);
                if (temp != null && temp is ContractWarrantPrinter)
                {
                    printContract = (ContractWarrantPrinter)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                printContract.dbContext = dbContext;
                printContract.ZoneList = zonelist;
                printContract.ParentsToProvince = parentsToProvince;
                printContract.CurrentZone = contractRegeditBookPanel.CurrentZone;
                printContract.RegeditBook = familyBook;
                printContract.Book = familyBook;
                printContract.Concord = familyConcord;
                printContract.OtherBook = otherBook;
                printContract.OtherConcord = otherConcord;
                printContract.LandCollection = lands;
                printContract.Contractor = vpn;
                printContract.Tissue = Tissue;
                printContract.DictList = DictList;
                printContract.TempleFilePath = tempPath;
                printContract.UseExcel = contractRegeditBookPanel.ExtendUseExcelDefine.WarrantExtendByExcel;
                printContract.BookPersonNum = contractRegeditBookPanel.BookPersonNum;
                printContract.BookLandNum = contractRegeditBookPanel.BookLandNum;
                printContract.BookNumSetting = contractRegeditBookPanel.BookNumSetting;
                printContract.PrintContractLand(false);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 导出证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportWarrant(object sender, RoutedEventArgs e)
        {
            //导出excel表业务类型，默认为承包方调查表
            if (contractRegeditBookPanel.CurrentZone == null)
            {
                ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportNoZone);
                return;
            }
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource(); // CreateDb();
                ContractRegeditBookBusiness ContractRegeditBookBusiness = new ContractRegeditBookBusiness(dbContext);
                GetContractRegeditBookSelectItem();
                if (dbContext == null)
                {
                    ShowBox(ContractRegeditBookInfo.ExportWarrant, DataBaseSource.ConnectionError, eMessageGrade.Error, null);
                    return;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                VirtualPersonBusiness PersonBusiness = new VirtualPersonBusiness(dbContext);
                int childrenCount = zoneStation.Count(contractRegeditBookPanel.CurrentZone.FullCode, eLevelOption.Subs);
                if (contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Group || (contractRegeditBookPanel.CurrentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractRegeditBookPanel.Items == null || contractRegeditBookPanel.Items.Count == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportTableDataNoWarrents);
                        return;
                    }
                    List<VirtualPerson> getpersonst = new List<VirtualPerson>();
                    if (contractRegeditBookPanel.IsBatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage caplp = new ContractRegeditBookPersonLockPage();
                        foreach (var item in contractRegeditBookPanel.Items)
                        {
                            getpersonst.Add(item.Tag);
                        }
                        if (getpersonst == null) return;
                        caplp.PersonItems = getpersonst;
                        caplp.Business = PersonBusiness;
                        contractRegeditBookPanel.ThePage.Page.ShowMessageBox(caplp, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (caplp.SelectedPersons == null || caplp.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportWarrantNoSelected);
                                return;
                            }
                            ExportDataCommonOperateTask(contractRegeditBookPanel.CurrentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.ExportWarrant,
                                ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant, caplp.SelectedPersons);
                        });
                    }
                    else
                    {
                        if (currentRegiditItem == null)
                        {
                            ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.ExportWarrantNoSelected);
                            return;
                        }
                        getpersonst.Add(currentRegiditItem.Tag);
                        ExportDataCommonOperateTask(contractRegeditBookPanel.CurrentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.ExportWarrant,
                              ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant, getpersonst);
                    }
                }
                else if ((contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Village || contractRegeditBookPanel.CurrentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    var warrantsNum = ContractRegeditBookBusiness.Count(contractRegeditBookPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    if (warrantsNum == 0)
                    {
                        ShowBox(ContractRegeditBookInfo.ExportWarrant, "当前地域下没有权证数据", eMessageGrade.Error);
                        return;
                    }
                    ExportDataCommonOperateTask(contractRegeditBookPanel.CurrentZone.FullName, ContractRegeditBookInfo.ExportWarrant, eContractRegeditBookType.BatchExportWarrant,
                        ContractRegeditBookInfo.ExportWarrantWord, ContractRegeditBookInfo.ExportWarrant);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractRegeditBookInfo.ExportWarrant, ContractRegeditBookInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrant(导出证书)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出证书(单任务)
        /// </summary>
        private void ExportWarrentTask(string fileName, string taskDes, string taskName, List<VirtualPerson> listPerson)
        {
            TaskExportWarrentArgument meta = new TaskExportWarrentArgument();
            meta.FileName = fileName;
            meta.DbContext = DataBaseSource.GetDataBaseSource();
            meta.CurrentZone = contractRegeditBookPanel.CurrentZone;
            meta.SystemDefine = this.SystemSet;
            meta.SelectedPersons = listPerson;
            meta.BookPersonNum = contractRegeditBookPanel.BookPersonNum;
            meta.BookLandNum = contractRegeditBookPanel.BookLandNum;
            meta.BookNumSetting = contractRegeditBookPanel.BookNumSetting;
            meta.ExtendUseExcelDefine = contractRegeditBookPanel.ExtendUseExcelDefine;
            TaskExportWarrentOperation import = new TaskExportWarrentOperation();
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;

            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(DbContext, "", true));
            });
            Workpage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        /// <summary>
        /// 批量导出证书(组任务)
        /// </summary>
        private void ExportWarrentTaskGroup(string fileName, string taskDes, string taskName)
        {
            string BookNumSetting = "NO.J";
            TaskGroupExportWarrentArgument groupArgument = new TaskGroupExportWarrentArgument();
            groupArgument.FileName = fileName;
            groupArgument.DbContext = DataBaseSource.GetDataBaseSource();
            groupArgument.CurrentZone = contractRegeditBookPanel.CurrentZone;
            groupArgument.SystemDefine = this.SystemSet;
            groupArgument.ExtendUseExcelDefine = contractRegeditBookPanel.ExtendUseExcelDefine;
            groupArgument.BookLandNum = contractRegeditBookPanel.BookLandNum;
            groupArgument.BookPersonNum = contractRegeditBookPanel.BookPersonNum;
            groupArgument.BookNumSetting = BookNumSetting;
            TaskGroupExportWarrentOperation groupOperation = new TaskGroupExportWarrentOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 证书

        #region 审批表

        /// <summary>
        /// 预览登记审批表
        /// </summary>
        public void btnPriviewDJSPB_Click(object sender, RoutedEventArgs e)
        {
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox("预览登记审批表", DataBaseSource.ConnectionError);
                return;
            }
            if (concordPanel.CurrentZone == null)
            {
                ShowBox("预览登记审批表", ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            var vpstation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landstation = dbContext.CreateContractLandWorkstation();
            var senderstation = dbContext.CreateSenderWorkStation();
            var concordstation = dbContext.CreateConcordStation();
            List<YuLinTu.Library.Entity.VirtualPerson> vps = vpstation.GetByZoneCode(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            List<ContractLand> alllands = landstation.GetCollection(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            int allsendersCount = senderstation.Count(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);

            vps.RemoveAll(vp => vp.Name.Contains("集体"));
            alllands.RemoveAll(l => l.OwnerName != null && l.OwnerName.Contains("集体"));

            if (concordPanel.CurrentZone.Level == eZoneLevel.Village || concordPanel.CurrentZone.Level == eZoneLevel.Town)
            {
                ExportDJSPBBusiness exp = new ExportDJSPBBusiness();//预览
                exp.PrintRequreBook(alllands, allsendersCount, vps);
            }
            else
            {
                //选择地域为镇(或大于镇)
                ShowBox("提示", "不包括组级地域，请选择在镇级或者村级进行预览!");
                return;
            }
        }

        /// <summary>
        /// 导出登记审批表-导出到村
        /// </summary>
        public void btnExportDJSPB_Click(object sender, RoutedEventArgs e)
        {
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox(ContractConcordInfo.PreviewConcord, DataBaseSource.ConnectionError);
                return;
            }
            if (concordPanel.CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            List<Zone> allZones = new List<Zone>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取子级地域失败)", ex.Message + ex.StackTrace);
                ShowBox("提示", "获取当前地域下的子级地域失败!");
                return;
            }
            allZones.RemoveAll(a => a.Level != eZoneLevel.Village && a.Level != eZoneLevel.Town);
            ExportDataPage extPage = new ExportDataPage(concordPanel.CurrentZone.FullName, Workpage, "导出登记审批表");
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (c, d) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || c == false)
                {
                    return;
                }

                if (concordPanel.CurrentZone.Level == eZoneLevel.Village)
                {
                    var vpstation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                    var landstation = dbContext.CreateContractLandWorkstation();
                    var senderstation = dbContext.CreateSenderWorkStation();
                    var concordstation = dbContext.CreateConcordStation();
                    List<YuLinTu.Library.Entity.VirtualPerson> vps = vpstation.GetByZoneCode(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    List<ContractLand> alllands = landstation.GetCollection(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    int allsendersCount = senderstation.Count(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    List<ContractConcord> allConcords = concordstation.GetContractsByZoneCode(concordPanel.CurrentZone.FullCode, eLevelOption.SelfAndSubs);
                    if (vps.Count() == 0 && alllands.Count == 0 && allsendersCount == 0)
                    {
                        ShowBox("提示", "本地域下没有发包方、承包方、地块数据");
                        return;
                    }
                    vps.RemoveAll(vp => vp.Name.Contains("集体"));
                    alllands.RemoveAll(l => l.OwnerName != null && l.OwnerName.Contains("集体"));

                    //执行单个任务
                    TaskExportDJSPBArgument meta = new TaskExportDJSPBArgument();
                    meta.CurrentZone = concordPanel.CurrentZone;
                    meta.FileName = extPage.FileName;
                    meta.Database = dbContext;
                    meta.VirtualPersons = vps;
                    meta.ALLLands = alllands;
                    meta.SendersCount = allsendersCount;
                    TaskExportDJSPBOperation import = new TaskExportDJSPBOperation();
                    import.Argument = meta;
                    import.Description = "导出登记审批表";
                    import.Name = "导出登记审批表";
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                    });
                    Workpage.TaskCenter.Add(import);
                    if (concordPanel.ShowTaskViewer != null)
                        concordPanel.ShowTaskViewer();
                    import.StartAsync();
                }
                else if (concordPanel.CurrentZone.Level == eZoneLevel.Town)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupExportDJSPBArgument groupMeta = new TaskGroupExportDJSPBArgument();
                    groupMeta.FileName = extPage.FileName;
                    groupMeta.Database = dbContext;
                    groupMeta.AllZones = allZones;
                    TaskGroupExportDJSPBOperation taskGroup = new TaskGroupExportDJSPBOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = "导出登记审批表";
                    taskGroup.Name = "导出登记审批表";
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                    });
                    Workpage.TaskCenter.Add(taskGroup);
                    if (concordPanel.ShowTaskViewer != null)
                        concordPanel.ShowTaskViewer();
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox("提示", "请选择在镇级或者村级进行导出!");
                    return;
                }
            });
        }

        #endregion 审批表

        #region 单户登记申请书

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        private void PrintRequireBook(object sender, RoutedEventArgs e)
        {
            btnSingleBook.IsOpen = false;
            PreviewSingleRequireBook();
        }

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        private void PreviewSingleRequireBook()
        {
            currentZone = concordPanel.CurrentZone;

            if (currentZone == null)
            {
                ShowBox("预览单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            GetConcordSelectItem();
            if (currentItem == null)
            {
                ShowBox("预览单户申请书", "请选择预览申请书的承包方或合同!");
                return;
            }
            if (currentConcord != null && currentConcord.Tag.ArableLandType != ((int)eConstructMode.Family).ToString())
            {
                ShowBox("预览单户申请书", "请选择家庭承包的合同!");
                return;
            }
            if (concordPanel.ConcordSettingDefine.SingleRequireDate)
            {
                ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                dateSetting.WorkPage = Workpage;
                Workpage.Page.ShowMessageBox(dateSetting, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    ContractConcordBusiness concordBus = new ContractConcordBusiness();
                    concordBus.PublishDateSetting = dateSetting.DateTimeSetting;
                    concordBus.DictList = concordPanel.DictList;
                    string message = null;
                    bool flag = concordBus.PrintRequireBookWord(currentZone, currentItem.Tag, concordPanel.DbContext, null, out message); //打印预览单户申请表
                    if (flag == false)
                    {
                        ShowBox("预览申请书", message);
                    }
                });
            }
            else
            {
                var publishSetting = new DateSetting();
                publishSetting.PublishStartDate = null;
                publishSetting.PublishEndDate = null;
                ContractConcordBusiness concordBus = new ContractConcordBusiness();
                concordBus.PublishDateSetting = publishSetting;
                concordBus.DictList = concordPanel.DictList;
                string message = null;
                bool flag = concordBus.PrintRequireBookWord(currentZone, currentItem.Tag, concordPanel.DbContext, null, out message); //打印预览单户申请表
                if (flag == false)
                {
                    if (string.IsNullOrEmpty(message))
                        message = "预览申请书失败!";
                    ShowBox("预览申请书", message);
                }
            }
        }

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        private void ExportRequireBook(object sender, RoutedEventArgs e)
        {
            btnSingleBook.IsOpen = false;
            ExportApplicationByFamily();
        }

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public void ExportApplicationByFamily()
        {
            currentZone = concordPanel.CurrentZone;
            zoneName = concordPanel.PersonBusiness.GetUinitName(currentZone);

            if (currentZone == null)
            {
                ShowBox("导出单户申请书", "请选择单户申请书所在行政区域!");
                return;
            }
            if (currentZone.Level > eZoneLevel.Town)
            {
                ShowBox("导出单户申请书", "批量导出时请选择镇级(包括镇级)以下的地域!");
                return;
            }
            var dbContext = DataBaseSource.GetDataBaseSource();
            var concordStation = dbContext.CreateConcordStation();
            var tempConcords = concordStation.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (tempConcords == null || tempConcords.Count == 0)
            {
                ShowBox("导出单户申请书", "当前地域没有合同数据!");
                return;
            }
            if (currentZone.Level == eZoneLevel.Group && currentItem == null)//选择组，但是弹出承包方选择框
            {
                ExportModeByVirtualPersons(zoneName, dbContext);
            }
            if ((currentZone.Level <= eZoneLevel.Town && currentZone.Level >= eZoneLevel.Village))
            {
                ExportModeBatch(zoneName, dbContext);
            }
            if (currentZone.Level == eZoneLevel.Group && currentItem != null)  //在组下并且选择了某个承包方数据
            {
                ExportModeByCurVirtualPerson(zoneName, dbContext);
            }
        }

        /// <summary>
        /// 导出模式一：按承包方集合进行导出
        /// </summary>
        private void ExportModeByVirtualPersons(string zoneName, IDbContext dbContext)
        {
            ContractAccountPersonLockPage caplp = new ContractAccountPersonLockPage();
            List<YuLinTu.Library.Entity.VirtualPerson> getpersonst = new List<YuLinTu.Library.Entity.VirtualPerson>();
            foreach (var item in concordPanel.Items)
            {
                getpersonst.Add(item.Tag);
            }
            if (getpersonst == null) return;
            caplp.PersonItems = getpersonst;
            caplp.Business = concordPanel.PersonBusiness;
            Workpage.Page.ShowMessageBox(caplp, (bb, e) =>
            {
                if (!(bool)bb)
                {
                    return;
                }
                if (caplp.selectVirtualPersons == null) return;
                if (concordPanel.ConcordSettingDefine.SingleRequireDate)
                {
                    ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                    dateSetting.WorkPage = Workpage;
                    Workpage.Page.ShowMessageBox(dateSetting, (s, t) =>
                    {
                        if (!(bool)s)
                        {
                            return;
                        }
                        ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "导出单户申请书");
                        extPage.Workpage = Workpage;
                        Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                            {
                                return;
                            }
                            string fileDir = extPage.FileName;
                            TaskExport(fileDir, dbContext, dateSetting.DateTimeSetting, caplp.selectVirtualPersons);
                        });
                    });
                }
                else
                {
                    ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "导出单户申请书");
                    extPage.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                        {
                            return;
                        }
                        string fileDir = extPage.FileName;
                        var publishDateSetting = new DateSetting();
                        publishDateSetting.PublishStartDate = null;
                        publishDateSetting.PublishEndDate = null;
                        TaskExport(fileDir, dbContext, publishDateSetting, caplp.selectVirtualPersons);
                    });
                }
            });
        }

        /// <summary>
        /// 导出模式二：批量导出
        /// </summary>
        private void ExportModeBatch(string zoneName, IDbContext dbContext)
        {
            var zoneStation = dbContext.CreateZoneWorkStation();
            List<Zone> selfAndSubZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<Zone> allZones = zoneStation.GetAllZones(currentZone);

            //批量导出
            if (concordPanel.ConcordSettingDefine.SingleRequireDate)
            {
                ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                dateSetting.WorkPage = Workpage;
                Workpage.Page.ShowMessageBox(dateSetting, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "批量导出单户申请书");
                    extPage.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(extPage, (m, n) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                        {
                            return;
                        }
                        string fileDir = extPage.FileName;
                        TaskGroupExport(fileDir, dbContext, dateSetting.DateTimeSetting, selfAndSubZones, allZones);
                    });
                });
            }
            else
            {
                ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "批量导出单户申请书");
                extPage.Workpage = Workpage;
                Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                    {
                        return;
                    }
                    string fileDir = extPage.FileName;
                    var publishDateSetting = new DateSetting();
                    publishDateSetting.PublishStartDate = null;
                    publishDateSetting.PublishEndDate = null;
                    TaskGroupExport(fileDir, dbContext, publishDateSetting, selfAndSubZones, allZones);
                });
            }
        }

        /// <summary>
        /// 导出模式三：按当前选择承包方进行导出
        /// </summary>
        private void ExportModeByCurVirtualPerson(string zoneName, IDbContext dbContext)
        {
            if (currentConcord != null && currentConcord.Tag.ArableLandType != ((int)eConstructMode.Family).ToString())
            {
                ShowBox("预览单户申请书", "请选择家庭承包的合同!");
                return;
            }
            if (concordPanel.ConcordSettingDefine.SingleRequireDate)
            {
                ContractConcordDateSetting dateSetting = new ContractConcordDateSetting();
                dateSetting.WorkPage = Workpage;
                Workpage.Page.ShowMessageBox(dateSetting, (s, t) =>
                {
                    if (!(bool)s)
                    {
                        return;
                    }
                    ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "导出单户申请书");
                    extPage.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                        {
                            return;
                        }
                        string fileDir = extPage.FileName;
                        List<YuLinTu.Library.Entity.VirtualPerson> listPerson = new List<YuLinTu.Library.Entity.VirtualPerson>();
                        listPerson.Add(currentItem.Tag);
                        TaskExport(fileDir, dbContext, dateSetting.DateTimeSetting, listPerson);
                    });
                });
            }
            else
            {
                ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, "导出单户申请书");
                extPage.Workpage = Workpage;
                Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                {
                    if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                    {
                        return;
                    }
                    string fileDir = extPage.FileName;
                    var publishDateSetting = new DateSetting();
                    publishDateSetting.PublishStartDate = null;
                    publishDateSetting.PublishEndDate = null;
                    List<YuLinTu.Library.Entity.VirtualPerson> listPerson = new List<YuLinTu.Library.Entity.VirtualPerson>();
                    listPerson.Add(currentItem.Tag);
                    TaskExport(fileDir, dbContext, publishDateSetting, listPerson);
                });
            }
        }

        /// <summary>
        /// 单任务导出
        /// </summary>
        private void TaskExport(string filePath, IDbContext dbContext, DateSetting publishDateSetting, List<YuLinTu.Library.Entity.VirtualPerson> listVp)
        {
            TaskSingleRequireBookArgument meta = new TaskSingleRequireBookArgument();
            meta.FileName = filePath;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.PublishDateSetting = publishDateSetting;
            meta.SelectContractor = listVp;
            meta.TaskDesc = currentZone.Name;
            meta.DictList = concordPanel.DictList;
            TaskSingleRequireBookOperation taskConcord = new TaskSingleRequireBookOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "家庭承包单户申请书";
            taskConcord.Name = "导出家庭承包单户申请书";
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(taskConcord);
            if (concordPanel.ShowTaskViewer != null)
            {
                concordPanel.ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 组任务导出
        /// </summary>
        private void TaskGroupExport(string filePath, IDbContext dbContext, DateSetting publishDateSetting, List<Zone> selfAndSubsZones, List<Zone> allZones)
        {
            TaskGroupSingleRequireBookArgument meta = new TaskGroupSingleRequireBookArgument();
            meta.FileName = filePath;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.SelfAndSubsZones = selfAndSubsZones;
            meta.AllZones = allZones;
            meta.PublishDateSetting = publishDateSetting;
            meta.DictList = concordPanel.DictList;
            TaskGroupSingleRequireBookOperation taskConcord = new TaskGroupSingleRequireBookOperation();
            taskConcord.Argument = meta;
            taskConcord.Description = "导出" + currentZone.FullName + "家庭承包单户申请书";
            taskConcord.Name = "批量导出家庭承包单户申请书";
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(taskConcord);
            if (concordPanel.ShowTaskViewer != null)
            {
                concordPanel.ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        #endregion 单户登记申请书

        #region 登记调查确认书

        /// <summary>
        /// 登记调查确认书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportConfirmBook_Click(object sender, EventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            string titletip = "登记调查确认书";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                var dicStation = DbContext.CreateDictWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<Dictionary> dictList = dicStation.Get();
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(titletip, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //批量导出
                    if (contractAccountPanel.IsBatch)
                    {
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(titletip, "请选择承包方");
                                return;
                            }
                            ExportConfirmBookTask(titletip, titletip, selectPage.SelectedPersons, DbContext, currentZone);
                        });
                    }
                    //else if (currentAccountItem != null && currentLandBinding == null)
                    //{
                    //    listPerson.Add(currentAccountItem.Tag);
                    //    ExportConfirmBookTask(titletip, titletip, listPerson, DbContext, currentZone);
                    //}
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox("导出登记调查确认书", "请选择一条数据进行预览！");
                            return;
                        }
                        bool flag = ExportSingleConfirmBook(currentZone, currentAccountItem.Tag, dictList);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportConfirmBookGroupTask(titletip, titletip, DbContext, currentZone);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(titletip, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVPWord(导出登记调查确认书)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 批量导出登记调查确认书
        /// </summary>
        private void ExportConfirmBookGroupTask(string taskDes, string taskName, IDbContext DbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, "导出登记调查确认书");
            extPage.Workpage = Workpage;
            var dicStation = DbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                TaskGroupExportConfirBookArgument groupArgument = new TaskGroupExportConfirBookArgument();
                groupArgument.DbContext = DbContext;
                groupArgument.CurrentZone = currentZone;
                groupArgument.FileName = extPage.FileName;
                groupArgument.DictList = dictList;
                groupArgument.SystemDefine = contractAccountPanel.SystemSet;
                TaskGroupExportConfirBookOperation groupOperation = new TaskGroupExportConfirBookOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Description = taskDes;
                groupOperation.Name = taskName;
                groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(groupOperation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                groupOperation.StartAsync();
            });
        }

        /// <summary>
        /// 导出调查确认书(单任务)
        /// </summary>
        private void ExportConfirmBookTask(string taskDes, string taskName, List<VirtualPerson> selectedPersons, IDbContext dbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, "导出登记调查确认书");
            extPage.Workpage = Workpage;
            var dicStation = dbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }

                TaskExportConfirBookArgument argument = new TaskExportConfirBookArgument();
                argument.CurrentZone = currentZone;
                argument.FileName = extPage.FileName;
                argument.DbContext = dbContext;
                argument.SelectedPersons = selectedPersons;
                argument.FileName = extPage.FileName;
                argument.DictList = dictList;
                argument.SystemDefine = contractAccountPanel.SystemSet;
                TaskExportConfirBookOperation operation = new TaskExportConfirBookOperation();
                operation.Argument = argument;
                operation.Description = taskDes;
                operation.Name = taskName;
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(operation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 单个导出登记调查确认书
        /// </summary>
        private bool ExportSingleConfirmBook(Zone zone, VirtualPerson vp, List<Dictionary> lstDict)
        {
            bool flag = true;
            List<XZDW> listLine = new List<XZDW>(1000);
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox("导出登记调查确认书", "系统数据源无效!");
                return false;
            }
            GetContractAccountSelectItem();
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var concords = concordStation.GetByZoneCode(zone.FullCode);
                //var vpconcord = concords.Find(dd => dd.ContracterId == vp.ID && dd.ArableLandType == "110");
                var lineStation = dbContext.CreateXZDWWorkStation();
                listLine = lineStation.GetByZoneCode(zone.FullCode);
                DiagramsView viewOfNeighorParcels = null;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfNeighorParcels = new DiagramsView();
                    viewOfNeighorParcels.Paper.Model.Width = 150;
                    viewOfNeighorParcels.Paper.Model.Height = 150;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                var vpconcord = concords.FirstOrDefault(dd => dd.ContracterId == vp.ID);
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                listLand.LandNumberFormat(contractAccountPanel.SystemSet);
                var landsOfFamily = listLand.FindAll(c => c.OwnerId == vp.ID);
                var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
                string tempPath = TemplateHelper.ExcelTemplate("西藏农村土地承包经营权登记调查确认书");  //模板文件
                ExportFamilySurveyBook export = new ExportFamilySurveyBook(tempPath, dictoryName);
                export.VirtualPerson = vp;
                export.ViewOfNeighorParcels = viewOfNeighorParcels;
                export.CurrentZone = zone;
                export.DbContext = dbContext;
                export.Concord = vpconcord;
                export.ListLineFeature = listLine;
                export.Lands = landsOfFamily;
                export.FilePath = SystemSet.DefaultPath;
                export.Write();
                export.Show();
            }
            catch (Exception ex)
            {
                flag = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        #endregion 登记调查确认书

        #region 地块示意图（同步设置）

        /// <summary>
        /// 地块示意图配置
        /// </summary>
        public ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        private void btnExportParcelBySet_Click(object sender, RoutedEventArgs e)
        {
            GetContractAccountSelectItem();
            btnParcelTable.IsOpen = false;
            var currentZone = contractAccountPanel.CurrentZone;
            var dbContext = DataBaseSource.GetDataBaseSource();

            if (currentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (dbContext == null)
            {
                return;
            }
            try
            {
                //如果选中的是确股地块，则不能使用此功能使用-使用确股查件要在专门的确股模块里面看确股模板的图
                if (currentLandBinding != null && currentLandBinding.IsStockLand == true)
                {
                    ShowBox("导出地块示意图", "请不要选择确股地块导出");
                    return;
                }

                var zoneStation = dbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (!contractAccountPanel.IsBatch)
                {
                    if (currentAccountItem == null)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ViewDataNo);
                        return;
                    }
                    var landStation = dbContext.CreateContractLandWorkstation();
                    var geoLands = ContractLandHeler.GetParcelLands(currentZone.FullCode, dbContext);// landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                    if (geoLands == null || geoLands.Count == 0)
                    {
                        //当前地域没有空间地块数据
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                        return;
                    }
                    // 界面上有当前选择承包方项(此时做预览操作),预览确权的地块时，要排除掉股地
                    List<ContractLand> geoLandsOfFamily = geoLands.FindAll(c => c.OwnerId == contractAccountPanel.CurrentAccountItem.Tag.ID && !c.IsStockLand);
                    if (geoLandsOfFamily == null || geoLandsOfFamily.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLands);
                        return;
                    }
                    var geoLandOfFamily = InitalizeAgricultureLandSortValue(geoLandsOfFamily);
                    if (geoLandOfFamily == null || geoLandOfFamily.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLandsBySetting);
                        return;
                    }
                    var confirmPage = new ConfirmPage(Workpage, ContractAccountInfo.PreviewMultiParcelOfFamily,
                        string.Format("是否预览{0}地块示意图?", contractAccountPanel.CurrentAccountItem.Tag.Name));
                    confirmPage.Confirm += (a, c) =>
                    {
                        try
                        {
                            string fileName = SystemSet.DefaultPath;
                            var listDict = dbContext.CreateDictWorkStation().Get();
                            var listLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                            if (listLand == null || listLand.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, string.Format("{0}未获取承包地块数据!", currentZone.FullName));
                                return;
                            }
                            ExportMultiParcelWord(currentZone, currentAccountItem.Tag, listLand, listDict, fileName);
                        }
                        catch (Exception ex)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ex.Message);
                            return;
                        }
                    };
                    Workpage.Page.ShowMessageBox(confirmPage, (b, r) =>
                    {
                    });
                }
                else
                {
                    if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                    {
                        //批量导出(选择地域大于组级并且当前地域下有子级地域)
                        ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.VolumnExportMultiParcelOfFamily,
                            ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily);
                    }
                    else if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                    {
                        //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                        var landStation = dbContext.CreateContractLandWorkstation();
                        var geoLands = landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                        if (geoLands == null || geoLands.Count == 0)
                        {
                            //当前地域没有空间地块数据
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                            return;
                        }
                        List<VirtualPerson> listPerson = new List<VirtualPerson>();
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelNoSelected);
                                return;
                            }
                            ExportDataCommonOperate(currentZone.FullName, ContractAccountInfo.ExportMultiParcelOfFamily, eContractAccountType.ExportMultiParcelOfFamily,
                         ContractAccountInfo.ExportMultiParcelOfFamilyDesc, ContractAccountInfo.ExportMultiParcelOfFamily, 1, selectPage.SelectedPersons);
                        });
                    }
                    else
                    {
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.VolumnExportZoneError);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "MultiParcelExport(导出地块示意图)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyFail);
                return;
            }
        }

        /// <summary>
        /// 导出单户标准地块示意图
        /// </summary>
        public bool ExportMultiParcelWord(Zone currentZone, VirtualPerson person, List<ContractLand> listLand,
            List<Dictionary> listDict, string filePath)
        {
            bool result = false;
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            List<XZDW> listLine = new List<XZDW>(1000);
            List<DZDW> listPoint = new List<DZDW>(1000);
            List<MZDW> listPolygon = new List<MZDW>(1000);
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<YuLinTu.Library.Entity.ContractRegeditBook> listBook = new List<YuLinTu.Library.Entity.ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(1000);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            try
            {
                if (currentZone == null)
                {
                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, "未选择导出数据的地域!");
                    return result;
                }
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                var PointStation = dbContext.CreateDZDWWorkStation();
                var PolygonStation = dbContext.CreateMZDWWorkStation();
                listConcord = concordStation.GetByZoneCode(currentZone.FullCode);
                listBook = bookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                listTissue = landStation.GetTissuesByConcord(currentZone);
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);
                var villageZone = GetParent(currentZone, dbContext);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 326;
                    viewOfAllMultiParcel.Paper.Model.Height = 398;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;

                    viewOfNeighorParcels = new DiagramsView();
                    viewOfNeighorParcels.Paper.Model.Width = 150;
                    viewOfNeighorParcels.Paper.Model.Height = 150;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                string markDesc = GetMarkDesc(currentZone, dbContext);
                string templatePath = TemplateHelper.WordTemplate("农村土地承包经营权标准地块示意图");
                string savePathOfImage = System.IO.Path.GetTempPath();
                bool isExsitGeo = listGeoLand.Any(c => c.OwnerId == person.ID);
                string familyNuber = Library.Business.ToolString.ExceptSpaceString(person.FamilyNumber);
                string savePathOfWord = InitalizeLandImageName(filePath, person).Replace("\\\\", "\\");  //filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";
                ExportContractLandParcelWord parcelWord = new ExportContractLandParcelWord(dbContext);
                parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                parcelWord.CurrentZone = currentZone;
                parcelWord.VillageZone = villageZone;
                parcelWord.SavePathOfImage = savePathOfImage;
                parcelWord.SavePathOfWord = savePathOfWord;
                parcelWord.DictList = listDict;
                parcelWord.DictDKLB = dictDKLB;
                parcelWord.ListGeoLand = listGeoLand;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListConcord = listConcord;
                parcelWord.ListBook = listBook;
                parcelWord.ListTissue = listTissue;
                parcelWord.ListDot = listDot;
                parcelWord.ListValidDot = listValidDot;
                parcelWord.ListCoil = listCoil;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListPointFeature = listPoint;
                parcelWord.ListPolygonFeature = listPolygon;
                parcelWord.ParcelCheckDate = DateTime.Now;
                parcelWord.ParcelDrawDate = DateTime.Now;
                parcelWord.SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
                parcelWord.OpenTemplate(templatePath);
                parcelWord.PrintPreview(person);
                if (isExsitGeo)
                {
                    var savePath = savePathOfWord.Replace("\\\\", "\\");
                    parcelWord.SaveAsMultiFile(person, savePathOfWord, ContractBusinessParcelWordSettingDefine.GetIntence().SaveParcelPCAsPDF);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandMultiParcelWord(批量导出标准地块示意图)", ex.Message + ex.StackTrace);
            }
            finally
            {
                listLand.Clear();
                listLand = null;
                listGeoLand.Clear();
                listGeoLand = null;
                listLine.Clear();
                listLine = null;
                listConcord.Clear();
                listConcord = null;
                listBook.Clear();
                listBook = null;
                listTissue.Clear();
                listTissue = null;
                dictDKLB.Clear();
                dictDKLB = null;
                listDot.Clear();
                listDot = null;
                listCoil.Clear();
                listCoil = null;
                listValidDot.Clear();
                listValidDot = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (viewOfAllMultiParcel != null)
                    {
                        viewOfAllMultiParcel.Dispose();
                        viewOfAllMultiParcel = null;
                    }
                    if (viewOfNeighorParcels != null)
                    {
                        viewOfNeighorParcels.Dispose();
                        viewOfNeighorParcels = null;
                    }
                }));

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return result;
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="TableType">表格类型(适用于台账报表中4个公用底层的表格)</param>
        /// <param name="listPerson">承包方集合</param>
        public void ExportDataCommonOperate(string zoneName, string header, eContractAccountType type, string taskDes, string taskName,
           int TableType = 1, List<VirtualPerson> listPerson = null, bool? isStockLand = false)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, Workpage, header);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                string saveFilePath = extPage.FileName;
                if (string.IsNullOrEmpty(saveFilePath) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractAccountType.ExportMultiParcelOfFamily: //地块示意图
                        ExportMultiParcelTask(saveFilePath, listPerson, taskDes, taskName, isStockLand);
                        break;

                    case eContractAccountType.VolumnExportMultiParcelOfFamily:  //批量导出地块示意图
                        ExportMultiParcelTaskGroup(saveFilePath, taskName, taskDes, isStockLand);
                        break;
                }
            });
        }

        /// <summary>
        /// 按照设置进行地块类别筛选导出
        /// </summary>
        private List<ContractLand> InitalizeAgricultureLandSortValue(List<ContractLand> geoLandCollection)
        {
            if (geoLandCollection.Count == 0) return new List<ContractLand>();
            if (ParcelWordSettingDefine.ExportContractLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportPrivateLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportMotorizeLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportWasteLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.WasteLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportCollectiveLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.CollectiveLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportEncollecLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.EncollecLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportFeedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.FeedLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportAbandonedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.AbandonedLand).ToString());
            }
            return geoLandCollection;
        }

        /// <summary>
        /// 导出地块示意图(单个任务)
        /// </summary>
        private void ExportMultiParcelTask(string fileName, List<VirtualPerson> selectedPersons, string taskDes, string taskName, bool? isStockLand)
        {
            TaskExportMultiParcelWordArgument argument = new TaskExportMultiParcelWordArgument();
            var dbContext = contractAccountPanel.DbContext;
            argument.DbContext = dbContext;
            argument.CurrentZone = contractAccountPanel.CurrentZone;
            argument.FileName = fileName;
            argument.SelectedPersons = selectedPersons;
            TaskExportMultiParcelWordOperation operation = new TaskExportMultiParcelWordOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            if (isStockLand != null)
                operation.Name = (bool)isStockLand ? "导出确股地块示意图" : taskName;
            else
                operation.Name = "导出确权确股地块示意图";
            operation.IsStockLand = isStockLand;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(operation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出地块示意图(多个任务)
        /// </summary>
        private void ExportMultiParcelTaskGroup(string fileName, string taskName, string taskDesc, bool? isStockLand)
        {
            TaskGroupExportMultiParcelWordArgument groupArgument = new TaskGroupExportMultiParcelWordArgument();
            var dbContext = contractAccountPanel.DbContext;
            groupArgument.CurrentZone = contractAccountPanel.CurrentZone;
            groupArgument.DbContext = dbContext;
            groupArgument.FileName = fileName;
            TaskGroupExportMultiParcelWordOperation groupOperation = new TaskGroupExportMultiParcelWordOperation();
            groupOperation.Argument = groupArgument;
            if (isStockLand != null)
                groupOperation.Name = (bool)isStockLand ? "导出确股地块示意图" : taskName;
            else
                groupOperation.Name = "导出确权确股地块示意图";
            groupOperation.Description = taskDesc;
            groupOperation.IsStockLand = isStockLand;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 地块示意图（同步设置）

        #region 标准地块示意图

        /// <summary>
        /// 导出单户多宗
        /// </summary>
        public void MultiParcelExport(object sender, RoutedEventArgs e)
        {
            GetContractAccountSelectItem();
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null) return;
                var zoneStation = dbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //仅导出当前选择地域下的数据(选择地域为组级地域或者当为大于组级地域同时没有子级地域)
                    var landStation = dbContext.CreateContractLandWorkstation();
                    var dicStation = dbContext.CreateDictWorkStation();
                    var geoLands = landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Self);
                    if (geoLands == null || geoLands.Count == 0)
                    {
                        //当前地域没有空间地块数据
                        ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentZoneNoGeoLand);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //界面选择批量(此时弹出承包方选择界面)
                    if (contractAccountPanel.IsBatch)
                    {
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelNoSelected);
                                return;
                            }
                            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportMultiParcelOfFamily);
                            extPage.Workpage = Workpage;
                            Workpage.Page.ShowMessageBox(extPage, (b1, r1) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b1 == false)
                                {
                                    return;
                                }
                                ExportMultiParcelTask(currentZone, dbContext, extPage.FileName, selectPage.SelectedPersons, ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyDesc);
                            });
                        });
                    }
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, "请选择一条数据进行预览！");
                            return;
                        }

                        var confirmPage = new ConfirmPage(Workpage, ContractAccountInfo.PreviewMultiParcelOfFamily,
                        string.Format("是否预览{0}地块示意图?", currentAccountItem.Tag.Name));
                        confirmPage.Confirm += (a, c) =>
                        {
                            try
                            {
                                //界面上有当前选择承包方项
                                string fileName = SystemSet.DefaultPath;
                                List<ContractLand> geoLandOfFamily = geoLands.FindAll(s => s.OwnerId == contractAccountPanel.CurrentAccountItem.Tag.ID);
                                if (geoLandOfFamily == null || geoLandOfFamily.Count == 0)
                                {
                                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLands);
                                    return;
                                }
                                var listDict = dicStation.Get();
                                var listLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                                if (listLand == null || listLand.Count == 0)
                                {
                                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, string.Format("{0}未获取承包地块数据!", currentZone.FullName));
                                    return;
                                }
                                bool canExport = ExportLandMultiParcelWord(currentZone, currentAccountItem.Tag, listLand, listDict, fileName);
                                //if (!canExport)
                                //{
                                //    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, "导出标准地块调查表失败！");
                                //    return;
                                //}
                            }
                            catch (Exception ex)
                            {
                                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ex.Message);
                                return;
                            }
                        };
                        Workpage.Page.ShowMessageBox(confirmPage, (b, r) =>
                        {
                        });

                        ////界面上有当前选择承包方项
                        //List<ContractLand> geoLandOfFamily = geoLands.FindAll(c => c.OwnerId == contractAccountPanel.CurrentAccountItem.Tag.ID);
                        //if (geoLandOfFamily == null || geoLandOfFamily.Count == 0)
                        //{
                        //    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.CurrentPersonNoGeoLands);
                        //    return;
                        //}
                        //var listDict = dicStation.Get();
                        //var listLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                        //if (listLand == null || listLand.Count == 0)
                        //{
                        //    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, string.Format("{0}未获取承包地块数据!", currentZone.FullName));
                        //    return;
                        //}
                        //bool canExport = ExportLandMultiParcelWord(currentZone, currentAccountItem.Tag, listLand, listDict,SystemSet.DefaultPath);
                        //if(!canExport)
                        //{
                        //    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, "导出标准地块调查表失败！");
                        //    return;
                        //}
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportMultiParcelOfFamily);
                    extPage.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                        {
                            return;
                        }
                        ExportMultiParcelTaskGroup(currentZone, dbContext, extPage.FileName, ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyDesc);
                    });
                }
                else
                {
                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "MultiParcelExport(导出单户多宗示意图)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, ContractAccountInfo.ExportMultiParcelOfFamilyFail);
                return;
            }
        }

        /// <summary>
        /// 导出单户标准地块示意图
        /// </summary>
        public bool ExportLandMultiParcelWord(Zone currentZone, VirtualPerson person, List<ContractLand> listLand,
            List<Dictionary> listDict, string filePath)
        {
            bool result = false;
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            List<XZDW> listLine = new List<XZDW>(1000);
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<YuLinTu.Library.Entity.ContractRegeditBook> listBook = new List<YuLinTu.Library.Entity.ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(1000);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            try
            {
                if (currentZone == null)
                {
                    ShowBox(ContractAccountInfo.ExportMultiParcelOfFamily, "未选择导出数据的地域!");
                    return result;
                }
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                listConcord = concordStation.GetByZoneCode(currentZone.FullCode);
                listBook = bookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                listTissue = landStation.GetTissuesByConcord(currentZone);
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 326;
                    viewOfAllMultiParcel.Paper.Model.Height = 398;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;

                    viewOfNeighorParcels = new DiagramsView();
                    viewOfNeighorParcels.Paper.Model.Width = 150;
                    viewOfNeighorParcels.Paper.Model.Height = 150;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                string markDesc = GetMarkDesc(currentZone, dbContext);
                string templatePath = TemplateHelper.WordTemplate("农村土地承包经营权标准地块示意图");
                string savePathOfImage = System.IO.Path.GetTempPath();
                bool isExsitGeo = listGeoLand.Any(c => c.OwnerId == person.ID);
                string familyNuber = Library.Business.ToolString.ExceptSpaceString(person.FamilyNumber);
                string savePathOfWord = InitalizeLandImageName(filePath, person).Replace("\\\\", "\\");  //filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";
                ExportContractLandParcelWordLZ parcelWord = new ExportContractLandParcelWordLZ(dbContext);
                parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                parcelWord.CurrentZone = currentZone;
                parcelWord.SavePathOfImage = savePathOfImage;
                parcelWord.SavePathOfWord = savePathOfWord;
                parcelWord.DictList = listDict;
                parcelWord.DictDKLB = dictDKLB;
                parcelWord.ListGeoLand = listGeoLand;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListConcord = listConcord;
                parcelWord.ListBook = listBook;
                parcelWord.ListTissue = listTissue;
                parcelWord.ListDot = listDot;
                parcelWord.ListValidDot = listValidDot;
                parcelWord.ListCoil = listCoil;
                parcelWord.ParcelCheckDate = DateTime.Now;
                parcelWord.ParcelDrawDate = DateTime.Now;
                parcelWord.SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
                parcelWord.OpenTemplate(templatePath);
                parcelWord.PrintPreview(person);
                if (isExsitGeo)
                {
                    var savePath = savePathOfWord.Replace("\\\\", "\\");
                    parcelWord.SaveAsMultiFile(person, savePathOfWord, ContractBusinessParcelWordSettingDefine.GetIntence().SaveParcelPCAsPDF);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandMultiParcelWord(批量导出标准地块示意图)", ex.Message + ex.StackTrace);
            }
            finally
            {
                listLand.Clear();
                listLand = null;
                listGeoLand.Clear();
                listGeoLand = null;
                listLine.Clear();
                listLine = null;
                listConcord.Clear();
                listConcord = null;
                listBook.Clear();
                listBook = null;
                listTissue.Clear();
                listTissue = null;
                dictDKLB.Clear();
                dictDKLB = null;
                listDot.Clear();
                listDot = null;
                listCoil.Clear();
                listCoil = null;
                listValidDot.Clear();
                listValidDot = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (viewOfAllMultiParcel != null)
                    {
                        viewOfAllMultiParcel.Dispose();
                        viewOfAllMultiParcel = null;
                    }
                    if (viewOfNeighorParcels != null)
                    {
                        viewOfNeighorParcels.Dispose();
                        viewOfNeighorParcels = null;
                    }
                }));

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return result;
        }

        /// <summary>
        /// 导出单户多宗(单个任务)
        /// </summary>
        private void ExportMultiParcelTask(Zone currentZone, IDbContext dbContext, string fileName, List<VirtualPerson> selectedPersons, string taskDes, string taskName)
        {
            TaskExportMultiParcelWordArgumentLZ argument = new TaskExportMultiParcelWordArgumentLZ();
            argument.DbContext = dbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.SelectedPersons = selectedPersons;
            argument.SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence(); //contractAccountPanel.SettingDefine;
            TaskExportMultiParcelWordOperationLZ operation = new TaskExportMultiParcelWordOperationLZ();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(operation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出单户多宗(多个任务)
        /// </summary>
        private void ExportMultiParcelTaskGroup(Zone currentZone, IDbContext dbContext, string fileName, string taskName, string taskDesc)
        {
            TaskGroupExportMultiParcelWordArgumentLZ groupArgument = new TaskGroupExportMultiParcelWordArgumentLZ();
            groupArgument.SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence(); //contractAccountPanel.SettingDefine;
            groupArgument.CurrentZone = currentZone;
            groupArgument.DbContext = dbContext;
            groupArgument.FileName = fileName;
            TaskGroupExportMultiParcelWordOperationLZ groupOperation = new TaskGroupExportMultiParcelWordOperationLZ();
            groupOperation.Argument = groupArgument;
            groupOperation.Name = taskName;
            groupOperation.Description = taskDesc;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 标准地块示意图

        #region 界址点成果表

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        public void btnExportDotResult_Click(object sender, RoutedEventArgs e)
        {
            btnAccountTable.IsOpen = false;
            GetContractAccountSelectItem();
            currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            List<Zone> allZones = new List<Zone>();
            allZones = GetAllChildrenZones() as List<Zone>;
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            List<ContractLand> listGeoLand = new List<ContractLand>();
            List<ContractLand> listAllLand = new List<ContractLand>();
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                return;
            }
            var landStation = dbContext.CreateContractLandWorkstation();
            listAllLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
            listGeoLand = listAllLand.FindAll(c => c.Shape != null);
            var personStaion = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            listPerson = personStaion.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (listGeoLand == null || listGeoLand.Count == 0)
            {
                //地域下没有空间地块
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.SubAndSelfZoneNoGeoLand);
                return;
            }

            if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
            {
                var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                int vpscount = vpStation.CountByZone(currentZone.FullCode);
                if (vpscount == 0)
                {
                    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoPersonData);
                    return;
                }
                if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentZoneNoPersonData);
                    return;
                }
                //选择批量
                if (contractAccountPanel.IsBatch)
                {
                    List<VirtualPerson> persons = new List<VirtualPerson>();
                    foreach (var item in contractAccountPanel.accountLandItems)
                    {
                        var chilren = item.Children;
                        if (chilren.Any(c => c.Tag.Shape != null))
                            persons.Add(item.Tag);
                    }
                    ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                    selectPage.Workpage = Workpage;
                    selectPage.PersonItems = persons;
                    Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportDotResultExcel);
                        Workpage.Page.ShowMessageBox(exportPage, (m, n) =>
                        {
                            if (!(bool)m || string.IsNullOrEmpty(exportPage.FileName))
                            {
                                return;
                            }
                            List<VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.ExportDotResultExcelNoSelectedPerson);
                                return;
                            }
                            //批量任务处理(两级任务)
                            ExportDotResultTaskGroup(selectedPersons, listGeoLand, exportPage.FileName);
                        });
                    });
                }
                else
                {
                    if (currentAccountItem == null || (currentAccountItem != null && currentLandBinding == null))
                    {
                        ShowBox(ContractAccountInfo.ExportDotResultExcel, "请选择一条数据进行预览！");
                        return;
                        ////选中承包方没选中地块项
                        //var currentListGeoLand = listGeoLand.FindAll(c => c.OwnerId == currentAccountItem.Tag.ID);
                        //if (currentListGeoLand == null || currentListGeoLand.Count == 0)
                        //{
                        //    ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentPersonNoGeoLands);
                        //    return;
                        //}
                        //ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportDotResultExcel);
                        //Workpage.Page.ShowMessageBox(exportPage, (b, r) =>
                        //{
                        //    if (!(bool)b || string.IsNullOrEmpty(exportPage.FileName))
                        //    {
                        //        return;
                        //    }

                        //    //单个任务处理
                        //    ExportDotResultTask(exportPage.FileName, currentListGeoLand);
                        //});
                    }
                    else if (currentAccountItem != null && currentLandBinding != null)
                    {
                        //选中地块项，直接预览
                        if (currentLandBinding.Tag == null)
                        {
                            ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.CurrentPersonNoGeoLands);
                            return;
                        }
                        ExportDotResultExcel(currentAccountItem.Tag, currentLandBinding.Tag, SystemSet.DefaultPath);
                    }
                }
            }
            else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
            {
                //批量导出(选择地域大于组级并且当前地域下有子级地域)
                ExportDataPage exportPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportDotResultExcel);
                Workpage.Page.ShowMessageBox(exportPage, (x, y) =>
                {
                    if (!(bool)x || string.IsNullOrEmpty(exportPage.FileName))
                    {
                        return;
                    }
                    //批量导出任务(三级任务)
                    ExportDotResultTaskTopGroup(listPerson, listGeoLand, exportPage.FileName, allZones);
                });
            }
            else
            {
                ShowBox(ContractAccountInfo.ExportDotResultExcel, ContractAccountInfo.VolumnExportZoneError);
                return;
            }
        }

        /// <summary>
        /// 单个地块直接预览界址点
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="land">承包地块</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="exportType">导出类型(直接预览：1、打印：2、保存：3)</param>
        public bool ExportDotResultExcel(VirtualPerson contractor, ContractLand land, string filePath = "", int exportType = 1)
        {
            bool isSuccess = true;
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                //land.LandNumberFormat(SystemSet);
                string tempPath = TemplateHelper.ExcelTemplate("西藏农村土地承包经营权界址点成果表");
                var export = new ExportDotResultTable(tempPath);
                filePath += land.LandNumber + "-" + "西藏农村土地承包经营权界址点成果表";
                export.Contractor = contractor;
                export.DotCollection = dotStation.GetByLandID(land.ID);
                export.LineCollection = coilStation.GetByLandID(land.ID);
                export.CurrentZone = currentZone;
                export.CurrentLand = land;
                //export.Contractor = currentPerson;
                export.Write();
                export.SaveAs(filePath);
                System.Diagnostics.Process.Start(filePath + ".xls");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDotResultExcel(导出界址点成果表Excel)", ex.Message + ex.StackTrace);
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量导出界址点成果表任务(三级任务)
        /// </summary>
        /// <param name="listPerson">承包方集合(当前地域和其子级地域下)</param>
        /// <param name="listGeoLand">空间地块集合(当前地域和子级地域下)</param>
        /// <param name="filName">保存文件路径</param>
        /// <param name="allZones">所有地域集合(当前地域及其子级地域)</param>
        private void ExportDotResultTaskTopGroup(List<VirtualPerson> listPerson, List<ContractLand> listGeoLand, string filName, List<Zone> allZones)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            TaskTopGropExportDotResultArgument groupTopArguemnt = new TaskTopGropExportDotResultArgument();
            groupTopArguemnt.ListPerson = listPerson;
            groupTopArguemnt.ListGeoLand = listGeoLand;
            groupTopArguemnt.CurrentZone = currentZone;
            groupTopArguemnt.Database = dbContext;
            groupTopArguemnt.FileName = filName;
            groupTopArguemnt.VirtualType = eVirtualType.Land;
            groupTopArguemnt.AllZones = allZones;
            TaskTopGroupExportDotResultOperation groupTopOperation = new TaskTopGroupExportDotResultOperation();
            groupTopOperation.Argument = groupTopArguemnt;
            groupTopOperation.Name = ContractAccountInfo.ExportDotResultExcel;
            groupTopOperation.Description = currentZone.FullName;
            groupTopOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupTopOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupTopOperation.StartAsync();
        }

        /// <summary>
        /// 批量导出界址点成果表任务(两级任务)
        /// </summary>
        /// <param name="selectedPersons">选中的承包方</param>
        /// <param name="listGeoLand">空间地块集合(当前地域和子级地域下)</param>
        /// <param name="fileName">保存文件路径</param>
        private void ExportDotResultTaskGroup(List<VirtualPerson> selectedPersons, List<ContractLand> listGeoLand, string fileName)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            TaskGroupExportDotResultArgument groupArgument = new TaskGroupExportDotResultArgument();
            groupArgument.ListPerson = selectedPersons;
            groupArgument.ListGeoLand = listGeoLand;
            groupArgument.CurrentZone = currentZone;
            groupArgument.Database = dbContext;
            groupArgument.FileName = fileName;
            //TODO 类型可能有误
            groupArgument.VirtualType = eVirtualType.Land;
            TaskGroupExportDotResultOperation groupOperation = new TaskGroupExportDotResultOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Name = ContractAccountInfo.ExportDotResultExcel;
            groupOperation.Description = currentZone.FullName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 导出界址点成果表任务
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="currentListGeoLand">当前承包方下所有空间地块</param>
        private void ExportDotResultTask(string fileName, List<ContractLand> currentListGeoLand)
        {
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
            TaskExportDotResultArgument argument = new TaskExportDotResultArgument();
            argument.ListGeoLand = currentListGeoLand;
            argument.Database = dbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = fileName;
            argument.CurrentPerson = currentAccountItem.Tag;
            TaskExportDotResultOperation opertion = new TaskExportDotResultOperation();
            opertion.Argument = argument;
            opertion.Name = ContractAccountInfo.ExportDotResultExcel;
            opertion.Description = ContractAccountInfo.ExportDotResultExcel;
            opertion.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(opertion);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            opertion.StartAsync();
        }

        #endregion 界址点成果表

        #region 承包合同

        /// <summary>
        /// 预览合同
        /// </summary>
        public void previewConcord_Click(object sender, RoutedEventArgs e)
        {
            GetConcordSelectItem();
            if (concordPanel.CurrentZone == null)
            {
                ShowBox(ContractConcordInfo.PreviewConcord, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            if (currentConcord == null)
            {
                ShowBox(ContractConcordInfo.PreviewConcord, ContractConcordInfo.PreviewConcordNoSelected);
                return;
            }
            var concord = currentConcord.Tag;
            var virtualPerson = currentItem.Tag;
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    ShowBox(ContractConcordInfo.PreviewConcord, DataBaseSource.ConnectionError);
                    return;
                }
                var landStation = dbContext.CreateContractLandWorkstation();
                var dictStation = dbContext.CreateDictWorkStation();
                var zoneStation = dbContext.CreateZoneWorkStation();
                var tissueStation = dbContext.CreateCollectivityTissueWorkStation();
                List<ContractLand> landsOfConcord = landStation.GetByConcordId(concord.ID);
                List<Dictionary> allDicts = dictStation.Get();
                CollectivityTissue tissue = tissueStation.Get(concord.SenderId);
                string templatePath = TemplateHelper.WordTemplate("西藏农村土地承包经营权农村土地承包合同书");
                string dictoryName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\西藏字典.xlsx");
                ExportAgricultureConcord exportConcord = new ExportAgricultureConcord(dictoryName);
                exportConcord.dbContext = dbContext;
                exportConcord.Tissue = tissue;
                exportConcord.VirtualPerson = virtualPerson;

                exportConcord.CurrentZone = currentZone;
                exportConcord.ListLand = landsOfConcord == null ? new List<ContractLand>() : landsOfConcord;
                exportConcord.ListDict = allDicts;

                exportConcord.CurrentZone = concordPanel.CurrentZone;
                exportConcord.Contractor = virtualPerson == null ? new YuLinTu.Library.Entity.VirtualPerson() { Name = "" } : virtualPerson;
                exportConcord.LandCollection = landsOfConcord;
                exportConcord.DictList = allDicts;

                exportConcord.OpenTemplate(templatePath);  //打开模板
                exportConcord.PrintPreview(concord);  //打开预览
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "previewConcord_Click(预览农村土地承包经营权承包合同书)", ex.Message + ex.StackTrace);
                ShowBox(ContractConcordInfo.PreviewConcord, "预览农村土地承包经营权承包合同失败!");
            }
        }

        /// <summary>
        /// 导出承包合同
        /// </summary>
        public void exportConcord_Click(object sender, RoutedEventArgs e)
        {
            GetConcordSelectItem();
            var currentZone = concordPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.CurrentZoneNoSelected);
                return;
            }
            List<Zone> allZones = new List<Zone>();
            List<Zone> childrenZone = new List<Zone>();
            List<ContractConcord> concords = new List<ContractConcord>();
            List<Dictionary> listDict = new List<Dictionary>();
            List<ContractLand> landsOfConcord = new List<ContractLand>();
            List<YuLinTu.Library.Entity.VirtualPerson> listPerson = new List<YuLinTu.Library.Entity.VirtualPerson>();
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    ShowBox(ContractConcordInfo.ExportConcord, DataBaseSource.ConnectionError);
                    return;
                }
                var concordStation = dbContext.CreateConcordStation();
                var dictStation = dbContext.CreateDictWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();

                var zoneStation = dbContext.CreateZoneWorkStation();
                allZones = GetAllChildrenZones(currentZone, ContractConcordInfo.ExportConcord, dbContext) as List<Zone>;
                childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //选择地域为组级地域或者当为大于组级地域同时没有子级地域
                    if (concordPanel.Items == null || concordPanel.Items.Count == 0)
                    {
                        ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.CurrentZoneNoPersons);
                        return;
                    }
                    if (currentItem == null)
                    {
                        //界面上没有选中项(此时弹出承包方选择界面)
                        foreach (var item in concordPanel.Items)
                        {
                            listPerson.Add(item.Tag);
                        }
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            List<YuLinTu.Library.Entity.VirtualPerson> selectedPersons = selectPage.SelectedPersons;
                            if (selectedPersons == null || selectedPersons.Count == 0)
                            {
                                ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordNoSelected);
                                return;
                            }
                            ExportDataPage savePage = new ExportDataPage(currentZone.FullName, Workpage, ContractConcordInfo.ExportConcord);
                            Workpage.Page.ShowMessageBox(savePage, (m, n) =>
                            {
                                if (string.IsNullOrEmpty(savePage.FileName) || m == false)
                                {
                                    return;
                                }
                                listDict = dictStation.Get();
                                TaskExportFamilyConcord(currentZone, zoneStation, selectedPersons, listDict, dbContext, savePage.FileName);
                            });
                        });
                    }
                    else
                    {
                        //有界面选择项
                        currentConcord = currentItem.Children.FirstOrDefault(c => c.Tag.ArableLandType == ((int)eConstructMode.Family).ToString());
                        if (currentConcord == null)
                        {
                            ShowBox(ContractConcordInfo.ExportConcord, "未获取家庭承包方式类型合同,无法执行导出!");
                            return;
                        }
                        landsOfConcord = landStation.GetByConcordId(currentConcord.Tag.ID);
                        ExportDataPage savePage = new ExportDataPage(currentZone.FullName, Workpage, ContractConcordInfo.ExportConcord);
                        savePage.Confirm += (s, a) =>
                        {
                            try
                            {
                                listDict = dictStation.Get();
                                bool isSuccess = ExportFamilyConcord(currentZone, zoneStation, currentConcord.Tag, landsOfConcord, listDict, savePage.FileName);
                                if (isSuccess)
                                {
                                    savePage.Dispatcher.Invoke(new Action(() =>
                                    {
                                        ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordComplete, eMessageGrade.Infomation);
                                    }));
                                }
                                else
                                {
                                    a.Parameter = false;
                                    return;
                                }
                            }
                            catch
                            {
                                savePage.Dispatcher.Invoke(new Action(() =>
                                {
                                    ShowBox(ContractConcordInfo.ExportConcord, "导出西藏农村土地承包合同书失败!");
                                }));
                                a.Parameter = false;
                                return;
                            }
                            finally
                            {
                                listDict.Clear();
                                listDict = null;
                                landsOfConcord = null;
                            }
                            a.Parameter = true;
                        };
                        Workpage.Page.ShowMessageBox(savePage, (b, r) =>
                        {
                            if (string.IsNullOrEmpty(savePage.FileName) || b == false)
                            {
                                return;
                            }
                            savePage.ConfirmAsync();
                        });
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenZone != null && childrenZone.Count > 0)
                {
                    //按照组任务导出(选择地域大于组级并且当前地域下有子级地域)
                    ExportDataPage savePage = new ExportDataPage(currentZone.FullName, Workpage, ContractConcordInfo.ExportConcord);
                    Workpage.Page.ShowMessageBox(savePage, (m, n) =>
                    {
                        if (string.IsNullOrEmpty(savePage.FileName) || m == false)
                        {
                            return;
                        }
                        TaskGroupExportFamilyConcord(currentZone, dbContext, savePage.FileName);
                    });
                }
                else
                {
                    ShowBox(ContractConcordInfo.ExportConcord, ContractConcordInfo.ExportConcordSelectZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "exportConcord_Click(导出家庭承包方式合同)", ex.Message + ex.StackTrace);
                ShowBox(ContractConcordInfo.ExportConcord, "导出西藏农村土地承包合同失败!");
            }
        }

        /// <summary>
        /// 导出家庭承包方式合同
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="concord">导出合同</param>
        /// <param name="lands">合同对应地块集合</param>
        /// <param name="dictList">数据字典集合</param>
        /// <param name="savePath">保存文件路径</param>
        private bool ExportFamilyConcord(Zone currentZone, IZoneWorkStation zoneStation, ContractConcord concord, List<ContractLand> lands, List<Dictionary> dictList, string savePath)
        {
            //单个导出合同（无进度）
            if (concord == null || lands == null)
                return false;
            bool result = true;
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                string templateName = "西藏农村土地承包经营权农村土地承包合同书";
                string templatePath = TemplateHelper.WordTemplate(templateName);
                string fullPath = savePath + @"\" + currentItem.Tag.FamilyNumber + "-" + concord.ContracterName + "-" + templateName;
                string zoneNameCounty = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.County, zoneStation);
                string zoneNameTown = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Town, zoneStation);
                string zoneNameVillage = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Village, zoneStation);
                string zoneNameGroup = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Group, zoneStation);
                var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
                var fbfStation = dbContext.CreateSenderWorkStation();
                ExportAgricultureConcord exportConcord = new ExportAgricultureConcord(dictoryName);
                exportConcord.dbContext = dbContext;
                exportConcord.CurrentZone = currentZone;
                exportConcord.VirtualPerson = currentItem.Tag;
                exportConcord.Systemset = concordPanel.SystemSet;
                exportConcord.ZoneNameCounty = string.IsNullOrEmpty(zoneNameCounty) ? "" : zoneNameCounty;
                exportConcord.ZoneNameTown = string.IsNullOrEmpty(zoneNameTown) ? "" : zoneNameTown;
                exportConcord.ZoneNameVillage = string.IsNullOrEmpty(zoneNameVillage) ? "" : zoneNameVillage;
                exportConcord.ZoneNameGroup = string.IsNullOrEmpty(zoneNameGroup) ? "" : zoneNameGroup;
                exportConcord.ListLand = lands;
                exportConcord.ListDict = dictList;
                exportConcord.OpenTemplate(templatePath);  //打开模板
                exportConcord.SaveAs(concord, fullPath);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportFamilyConcord(导出家庭承包方式合同)", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 导出家庭承包方式合同(单个任务)
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="zoneStation">地域业务逻辑层</param>
        /// <param name="listPerson">承包方集合</param>
        /// <param name="dbContext">数据库</param>
        /// <param name="filePath">保存文件路径</param>
        private void TaskExportFamilyConcord(Zone currentZone, IZoneWorkStation zoneStation, List<YuLinTu.Library.Entity.VirtualPerson> listPerson, List<Dictionary> listDict, IDbContext dbContext, string filePath)
        {
            if (currentZone == null || listPerson == null || dbContext == null || string.IsNullOrEmpty(filePath))
                return;
            string zoneNameCounty = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.County, zoneStation);
            string zoneNameTown = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Town, zoneStation);
            string zoneNameVillage = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Village, zoneStation);
            string zoneNameGroup = GetZoneNameByLevel(concordPanel.CurrentZone.FullCode, eZoneLevel.Group, zoneStation);
            TaskExportFamilyConcordArgument meta = new TaskExportFamilyConcordArgument();
            meta.FileName = filePath;
            meta.DbContext = dbContext;
            meta.CurrentZone = currentZone;
            meta.ListPerson = listPerson;
            meta.ListDict = listDict;
            meta.ZoneNameCounty = zoneNameCounty;
            meta.systemset = concordPanel.SystemSet;
            meta.ZoneNameTown = zoneNameTown;
            meta.ZoneNameVillage = zoneNameVillage;
            meta.ZoneNameGroup = zoneNameGroup;
            TaskExportFamilyConcordOperation taskConcord = new TaskExportFamilyConcordOperation();
            taskConcord.Workpage = Workpage;
            taskConcord.Argument = meta;
            taskConcord.Description = ContractConcordInfo.ExportConcordDesc;
            taskConcord.Name = ContractConcordInfo.ExportConcord;
            taskConcord.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(taskConcord);
            if (concordPanel.ShowTaskViewer != null)
            {
                concordPanel.ShowTaskViewer();
            }
            taskConcord.StartAsync();
        }

        /// <summary>
        /// 导出家庭承包方式合同(组任务)
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="dbContext">数据库</param>
        /// <param name="filePath">保存文件路径</param>
        private void TaskGroupExportFamilyConcord(Zone currentZone, IDbContext dbContext, string filePath)
        {
            if (currentZone == null || dbContext == null || string.IsNullOrEmpty(filePath))
                return;
            TaskGroupExportFamilyConcordArgument groupArgument = new TaskGroupExportFamilyConcordArgument();
            groupArgument.FileName = filePath;
            groupArgument.DbContext = dbContext;
            groupArgument.CurrentZone = currentZone;
            groupArgument.systemset = concordPanel.SystemSet;
            TaskGroupExportFamilyConcordOperation groupOperation = new TaskGroupExportFamilyConcordOperation();
            groupOperation.Workpage = Workpage;
            groupOperation.Argument = groupArgument;
            groupOperation.Description = ContractConcordInfo.ExportConcordDesc;
            groupOperation.Name = ContractConcordInfo.ExportConcord;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(groupOperation);
            if (concordPanel.ShowTaskViewer != null)
            {
                concordPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 承包合同

        #region 发包方调查表

        /// <summary>
        /// 导出发包方调查表（Word）
        /// </summary>
        private void btnSenderTable_Click(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSenderNoZone);
                return;
            }
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    var landStation = dbContext.CreateAgriculturalLandWorkstation();
                    var senderStation = dbContext.CreateCollectivityTissueWorkStation();
                    var tissues = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
                    if (tissues == null || tissues.Count == 0)
                    {
                        ShowBox("导出发包方Word调查表", "未获取发包方数据!");
                        return;
                    }
                    if (tissues.Count == 1)
                    {
                        //landStation.ExportSenderSurveyWord(tissues.FirstOrDefault(), SystemSet.DefaultPath);
                        ExportSenderSurveyWord(tissues.FirstOrDefault(), SystemSet.DefaultPath);
                    }
                    else
                    {
                        ExportSenderWordTask(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSenderDataExcel, dbContext, currentZone);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportSenderWordTaskGroup(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.ExportSenderDataExcel, dbContext, currentZone);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportSenderDataExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderWord(导出发包方Word表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出单个发包方Word调查表
        /// </summary>
        public void ExportSenderSurveyWord(CollectivityTissue tissue, string DefaultPath)
        {
            if (tissue == null)
            {
                return;
            }
            var dbContext = DataBaseSource.GetDataBaseSource();
            var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
            string fileTemplate = TemplateHelperWork.WordTemplate("西藏农村土地承包经营权发包方调查表");
            ExportSenderTable senderTable = new ExportSenderTable(dictoryName);
            var dictBusiness = new DictionaryBusiness(dbContext);   //数据字典
            senderTable.DictList = dictBusiness.GetAll();
            senderTable.OpenTemplate(fileTemplate);
            senderTable.PrintPreview(tissue, DefaultPath + "\\" + tissue.Name + "-发包方调查表");
        }

        /// <summary>
        /// 导出发包方Word(单个任务)
        /// </summary>
        private void ExportSenderWordTask(string taskDes, string taskName, IDbContext dbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportSenderDataExcel);
            extPage.Workpage = Workpage;
            var dicStation = dbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }

                TaskExportSenderWordArgument argument = new TaskExportSenderWordArgument();
                argument.DbContext = dbContext;
                argument.CurrentZone = currentZone;
                argument.FileName = extPage.FileName;
                TaskExportSenderWordOperation operation = new TaskExportSenderWordOperation();
                operation.Argument = argument;
                operation.Description = taskDes;
                operation.Name = taskName;
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(operation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 批量导出发包方Word(组任务)
        /// </summary>
        private void ExportSenderWordTaskGroup(string taskDes, string taskName, IDbContext dbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportSenderDataExcel);
            extPage.Workpage = Workpage;
            var dicStation = dbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }

                TaskGroupExportSenderWordArgument groupArgument = new TaskGroupExportSenderWordArgument();
                groupArgument.DbContext = dbContext;
                groupArgument.CurrentZone = currentZone;
                groupArgument.FileName = extPage.FileName;
                TaskGroupExportSenderWordOperation groupOperation = new TaskGroupExportSenderWordOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Description = taskDes;
                groupOperation.Name = taskName;
                groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(groupOperation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                groupOperation.StartAsync();
            });
        }

        #endregion 发包方调查表

        #region 调查信息公示表

        /// <summary>
        /// 导出调查信息公示表Excel
        /// </summary>
        public void ExportPublishExcel(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            if (contractAccountPanel.CurrentZone == null)
            {
                //没有选择导出地域
                ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportNoZone);
                return;
            }
            var currentZone = contractAccountPanel.CurrentZone;
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null) return;
                var zoneStation = dbContext.CreateZoneWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }
                    var dateSetting = new ContractAccountDateSetting();
                    dateSetting.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        contractAccountPanel.PublishDateSetting = dateSetting.DateTimeSetting;
                        ExportPublishExcelTask(SystemSet.DefaultPath, ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportSurveyTableData);
                    });
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ContractAccountDateSetting dateSetting = new ContractAccountDateSetting();
                    dateSetting.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(dateSetting, (b, r) =>
                    {
                        if (!(bool)b)
                        {
                            return;
                        }
                        ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportLandSurveyInFoPubExcel);
                        extPage.Workpage = Workpage;
                        Workpage.Page.ShowMessageBox(extPage, (k, s) =>
                        {
                            string saveFilePath = extPage.FileName;
                            if (string.IsNullOrEmpty(saveFilePath) || k == false)
                            {
                                return;
                            }

                            contractAccountPanel.PublishDateSetting = dateSetting.DateTimeSetting;
                            ExportPublishExcelTaskGroup(saveFilePath, ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.ExportSurveyTableData);
                        });
                    });
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandSurveyInFoPubExcel, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishExcel(导出调查信息公示表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出调查信息公示表Excel(单任务)
        /// </summary>
        private void ExportPublishExcelTask(string fileName, string taskDes, string taskName)
        {
            TaskExportSurveyPublishExcelArgument argument = new TaskExportSurveyPublishExcelArgument();
            argument.DbContext = contractAccountPanel.DbContext;
            argument.CurrentZone = contractAccountPanel.CurrentZone;
            argument.FileName = fileName;
            argument.VirtualType = contractAccountPanel.VirtualType;
            argument.PublishDateSetting = contractAccountPanel.PublishDateSetting;
            argument.SystemSet = SystemSetDefine.GetIntence(); //contractAccountPanel.SystemSet;
            argument.SettingDefine = ContractBusinessSettingDefine.GetIntence(); //contractAccountPanel.SettingDefine;
            TaskExportSurveyPublishExcelOperation operation = new TaskExportSurveyPublishExcelOperation();
            operation.Argument = argument;
            operation.Description = taskDes;
            operation.Name = taskName;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            contractAccountPanel.TheWorkPage.TaskCenter.Add(operation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            operation.StartAsync();
        }

        /// <summary>
        /// 批量导出调查信息公示表Excel(组任务)
        /// </summary>
        private void ExportPublishExcelTaskGroup(string fileName, string taskDes, string taskName)
        {
            TaskGroupExportSurveyPublishExcelArgument groupArgument = new TaskGroupExportSurveyPublishExcelArgument();
            groupArgument.DbContext = contractAccountPanel.DbContext;
            groupArgument.CurrentZone = contractAccountPanel.CurrentZone;
            groupArgument.FileName = fileName;
            groupArgument.VirtualType = contractAccountPanel.VirtualType;
            groupArgument.PublishDateSetting = contractAccountPanel.PublishDateSetting;
            groupArgument.SystemSet = SystemSetDefine.GetIntence(); //contractAccountPanel.SystemSet;
            groupArgument.SettingDefine = ContractBusinessSettingDefine.GetIntence(); //contractAccountPanel.SettingDefine;
            TaskGroupExportSurveyPublishExcelOperation groupOperation = new TaskGroupExportSurveyPublishExcelOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Description = taskDes;
            groupOperation.Name = taskName;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                //TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            contractAccountPanel.TheWorkPage.TaskCenter.Add(groupOperation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            groupOperation.StartAsync();
        }

        #endregion 调查信息公示表

        #region 农户基本信息公示表

        /// <summary>
        /// 农户基本信息公示表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFamilyGSB_Click(object sender, EventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            string titletip = "农户基本信息公示表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                var dicStation = DbContext.CreateDictWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<Dictionary> dictList = dicStation.Get();
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(titletip, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //界面上选择批量(此时弹出承包方选择界面)
                    if (contractAccountPanel.IsBatch)
                    {
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(titletip, "请选择承包方");
                                return;
                            }
                            ExportFamilySurveyInfo(titletip, titletip, selectPage.SelectedPersons, DbContext, currentZone);
                        });
                    }
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox("导出农户基本信息公示表", "请选择一条数据进行预览！");
                            return;
                        }
                        ExportSingleGSMTable(currentZone, currentAccountItem.Tag, dictList);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportBatchSurveyInfo(titletip, titletip, DbContext, currentZone);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(titletip, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSingleVPTable(导出农户基本信息公示表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 批量导出公示表
        /// </summary>
        private void ExportBatchSurveyInfo(string taskDes, string taskName, IDbContext DbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, "导出农户基本信息公示表");
            extPage.Workpage = Workpage;
            var dicStation = DbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                TaskGroupExportFamilyGSBArgument groupArgument = new TaskGroupExportFamilyGSBArgument();
                groupArgument.DbContext = DbContext;
                groupArgument.CurrentZone = currentZone;
                groupArgument.FileName = extPage.FileName;
                groupArgument.DictList = dictList;
                groupArgument.SystemDefine = contractAccountPanel.SystemSet;
                TaskGroupExportFamilyGSBOperation groupOperation = new TaskGroupExportFamilyGSBOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Description = taskDes;
                groupOperation.Name = taskName;
                groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(groupOperation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                groupOperation.StartAsync();
            });
        }

        /// <summary>
        /// 导出公示表(单任务)
        /// </summary>
        private void ExportFamilySurveyInfo(string taskDes, string taskName, List<VirtualPerson> selectedPersons, IDbContext dbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, "导出农户基本信息公示表");
            extPage.Workpage = Workpage;
            var dicStation = dbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }

                TaskExportFamilyGSBArgument argument = new TaskExportFamilyGSBArgument();
                argument.CurrentZone = currentZone;
                argument.FileName = extPage.FileName;
                argument.DbContext = dbContext;
                argument.SelectedPersons = selectedPersons;
                argument.DictList = dictList;
                argument.SystemDefine = m_systemSet;
                TaskExportFamilyGSBtOperation operation = new TaskExportFamilyGSBtOperation();
                operation.Argument = argument;
                operation.Description = taskDes;
                operation.Name = taskName;
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(operation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 单个导出农基本信息公示表
        /// </summary>
        private bool ExportSingleGSMTable(Zone zone, VirtualPerson vp, List<Dictionary> lstDict)
        {
            bool flag = true;
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox("地块调查表", "系统数据源无效!");
                return false;
            }
            GetContractAccountSelectItem();
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var concords = concordStation.GetByZoneCode(zone.FullCode);
                var vpconcord = concords.Find(dd => dd.ContracterId == vp.ID && dd.ArableLandType == "110");
                var senderStation = dbContext.CreateSenderWorkStation();
                var tissue = senderStation.Get(zone.ID);
                if (tissue == null && zone != null)
                {
                    tissue = senderStation.Get(zone.ID);
                }
                if (SystemSet.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        tissue = Senders[0];
                    }
                }
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                listLand.LandNumberFormat(contractAccountPanel.SystemSet);
                var landsOfFamily = listLand.FindAll(c => c.OwnerId == vp.ID);
                var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
                string tempPath = TemplateHelper.ExcelTemplate("西藏农村土地承包经营权农户基本信息公示表");  //模板文件
                ExportFamilyGSB export = new ExportFamilyGSB(tempPath, dictoryName);
                export.Lands = landsOfFamily == null ? new List<ContractLand>() : landsOfFamily;  //地块集合
                export.VirtualPerson = vp;
                export.CurrentZone = zone;
                export.SystemSet = m_systemSet;
                export.Concord = vpconcord;
                export.Write();
                export.Show();
            }
            catch (Exception ex)
            {
                flag = false;
                Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        #endregion 农户基本信息公示表

        #region 承包方调查表

        /// <summary>
        /// 承包方调查表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportVPWord_Click(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            string titletip = "承包方调查表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                var dicStation = DbContext.CreateDictWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                List<Dictionary> dictList = dicStation.Get();
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.CurrentZoneNoPersonData);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    //界面上选择批量导出，弹出承包方选择界面
                    if (contractAccountPanel.IsBatch)
                    {
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportVPWordNoSelected);
                                return;
                            }
                            ExportVPWordTask(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportDataWord, selectPage.SelectedPersons, DbContext, currentZone);
                        });
                    }
                    //else if (currentAccountItem != null && currentLandBinding == null)
                    //{
                    //    listPerson.Add(currentAccountItem.Tag);
                    //    ExportVPWordTask(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportDataWord, listPerson, DbContext, currentZone);
                    //}
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox(ContractAccountInfo.ExportDataWord, "请选择承包方进行预览！");
                            return;
                        }
                        //currentLandBinding.Tag.LandNumberFormat(SystemSet);
                        bool flag = ExportSingleVPTable(currentZone, currentAccountItem.Tag, dictList);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportVPWordTaskGroup(ContractAccountInfo.ExportDataWord, ContractAccountInfo.ExportDataWord, DbContext, currentZone);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVPWord(导出承包方Word调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出承包方Word调查表(单个任务)
        /// </summary>
        private void ExportVPWordTask(string taskDes, string taskName, List<VirtualPerson> selectedPersons, IDbContext dbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportDataWord);
            extPage.Workpage = Workpage;
            var dicStation = dbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }

                TaskExportVPWordArgument argument = new TaskExportVPWordArgument();
                argument.CurrentZone = currentZone;
                argument.FileName = extPage.FileName;
                argument.DbContext = dbContext;
                //TODO 参数是否有用
                //argument.VirtualType = VirtualType;
                argument.SelectedPersons = selectedPersons;
                argument.DictList = dictList;
                TaskExportVPWordOperation operation = new TaskExportVPWordOperation();
                operation.Argument = argument;
                operation.Description = taskDes;
                operation.Name = taskName;
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(operation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 批量导出承包方Word调查表(组任务)
        /// </summary>
        private void ExportVPWordTaskGroup(string taskDes, string taskName, IDbContext DbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportDataWord);
            extPage.Workpage = Workpage;
            var dicStation = DbContext.CreateDictWorkStation();
            List<Dictionary> dictList = dicStation.Get();
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                TaskGroupExportVPWordArgument groupArgument = new TaskGroupExportVPWordArgument();
                groupArgument.DbContext = DbContext;
                groupArgument.CurrentZone = currentZone;
                groupArgument.FileName = extPage.FileName;
                groupArgument.DictList = dictList;
                TaskGroupExportVPWordOperation groupOperation = new TaskGroupExportVPWordOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Description = taskDes;
                groupOperation.Name = taskName;
                groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(groupOperation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                groupOperation.StartAsync();
            });
        }

        /// <summary>
        /// 单个导出农村土地承包经营权承包方调查表
        /// </summary>
        private bool ExportSingleVPTable(Zone zone, VirtualPerson vp, List<Dictionary> lstDict)
        {
            bool flag = true;
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox("地块调查表", "系统数据源无效!");
                return false;
            }
            GetContractAccountSelectItem();
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
                ExportContractorTable export = new ExportContractorTable(dictoryName);
                string tempPath = TemplateHelper.WordTemplate("西藏农村土地承包经营权承包方调查表");  //模板文件
                export.Contractor = vp;
                export.CurrentZone = zone;
                export.OpenTemplate(tempPath);
                export.PrintPreview(vp);
            }
            catch (Exception ex)
            {
                flag = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        #endregion 承包方调查表

        //#region 承包地块调查表
        //private void btnExportLandExcel_Click(object sender, RoutedEventArgs e)
        //{
        //    btnSurveyTable.IsOpen = false;
        //    GetContractAccountSelectItem();
        //    string titletip = "承包地块调查表";
        //    var currentZone = contractAccountPanel.CurrentZone;
        //    if (currentZone == null)
        //    {
        //        ShowBox(titletip, ContractAccountInfo.ExportNoZone);
        //        return;
        //    }
        //    IDbContext DbContext = DataBaseSource.GetDataBaseSource();
        //    try
        //    {
        //        var zoneStation = DbContext.CreateZoneWorkStation();
        //        var dicStation = DbContext.CreateDictWorkStation();
        //        List<Dictionary> DictList = dicStation.Get();
        //        int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
        //        if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
        //        {
        //            //单个任务
        //            if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
        //            {
        //                ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.CurrentZoneNoLand);
        //                return;
        //            }
        //            List<VirtualPerson> listPerson = new List<VirtualPerson>();
        //            if (contractAccountPanel.IsBatch)
        //            {
        //                //界面上没有选择承包方项(此时弹出承包方选择界面)
        //                ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
        //                selectPage.Workpage = Workpage;
        //                foreach (var item in contractAccountPanel.accountLandItems)
        //                {
        //                    listPerson.Add(item.Tag);
        //                }
        //                selectPage.PersonItems = listPerson;
        //                Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
        //                {
        //                    if (!(bool)b)
        //                    {
        //                        return;
        //                    }
        //                    if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
        //                    {
        //                        ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportLandDataWordNoSelected);
        //                        return;
        //                    }
        //                    ExportLandWordTask(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, selectPage.SelectedPersons, DbContext, currentZone);
        //                });
        //            }
        //            else
        //            {
        //                if (currentAccountItem == null || (currentAccountItem != null && currentLandBinding == null))
        //                {
        //                    ShowBox(ContractAccountInfo.ExportLandDataWord, "请选择一条数据进行预览！");
        //                    return;
        //                }
        //                else
        //                {
        //                    bool flag = ExportLandSurveyTable(currentZone, currentLandBinding.Tag, currentAccountItem.Tag, DictList);
        //                }
        //            }
        //        }
        //        else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
        //        {
        //            //组任务
        //            ExportLandWordTaskGroup(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, DbContext, currentZone);
        //        }
        //        else
        //        {
        //            //选择地域大于镇
        //            ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.VolumnExportZoneError);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出承包地块Excel调查表)", ex.Message + ex.StackTrace);
        //        return;
        //    }
        //}
        ///// <summary>
        ///// 导出地块调查表(单个任务)
        ///// </summary>
        //private void ExportLandWordTask(string taskDes, string taskName, List<VirtualPerson> selectedPersons, IDbContext DbContext, Zone currentZone)
        //{
        //    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportLandDataWord);
        //    extPage.Workpage = Workpage;
        //    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
        //    {
        //        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
        //        {
        //            return;
        //        }
        //        TaskExportLandSurveyArgument argument = new TaskExportLandSurveyArgument();
        //        argument.DbContext = DbContext;
        //        argument.CurrentZone = currentZone;
        //        argument.FileName = extPage.FileName;
        //        argument.SelectedPersons = selectedPersons;
        //        argument.SystemDefine = contractAccountPanel.SystemSet;
        //        TaskExportLandSurveyOperation operation = new TaskExportLandSurveyOperation();
        //        operation.Argument = argument;
        //        operation.Description = ContractAccountInfo.ExportLandDataWord;
        //        operation.Name = ContractAccountInfo.ExportLandDataWord;
        //        operation.Completed += new TaskCompletedEventHandler((o, t) =>
        //        {
        //        });
        //        Workpage.TaskCenter.Add(operation);
        //        if (contractAccountPanel.ShowTaskViewer != null)
        //        {
        //            contractAccountPanel.ShowTaskViewer();
        //        }
        //        operation.StartAsync();
        //    });
        //}

        ///// <summary>
        ///// 批量导出地块调查表(组任务)
        ///// </summary>
        //private void ExportLandWordTaskGroup(string taskDes, string taskName, IDbContext DbContext, Zone currentZone)
        //{
        //    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportLandDataWord);
        //    extPage.Workpage = Workpage;
        //    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
        //    {
        //        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
        //        {
        //            return;
        //        }
        //        TaskGroupExportLandSurveyArgument groupArgument = new TaskGroupExportLandSurveyArgument();
        //        groupArgument.DbContext = DbContext;
        //        groupArgument.CurrentZone = currentZone;
        //        groupArgument.FileName = extPage.FileName;
        //        TaskGroupExportLandSurveyOperation groupOperation = new TaskGroupExportLandSurveyOperation();
        //        groupOperation.Argument = groupArgument;
        //        groupOperation.Description = taskDes;
        //        groupOperation.Name = taskName;
        //        groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
        //        {
        //        });
        //        Workpage.TaskCenter.Add(groupOperation);
        //        if (contractAccountPanel.ShowTaskViewer != null)
        //        {
        //            contractAccountPanel.ShowTaskViewer();
        //        }
        //        groupOperation.StartAsync();
        //    });
        //}

        ///// <summary>
        ///// 单个导出农村土地承包经营权地块调查表
        ///// </summary>
        //private bool ExportLandSurveyTable(Zone zone, ContractLand land, VirtualPerson vp, List<Dictionary> lstDict)
        //{
        //    bool flag = true;
        //    var dbContext = DataBaseSource.GetDataBaseSource();
        //    if (dbContext == null)
        //    {
        //        ShowBox("地块调查表", "系统数据源无效!");
        //        return false;
        //    }
        //    GetContractAccountSelectItem();
        //    int dotCount = 0;
        //    try
        //    {
        //        var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
        //        var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
        //        var concordStation = dbContext.CreateConcordStation();
        //        var senderStation = dbContext.CreateSenderWorkStation();
        //        var concord = (land.ConcordId != null && land.ConcordId.HasValue) ? concordStation.Get(land.ConcordId.Value) : null;
        //        var tissue = concord != null ? senderStation.Get(concord.SenderId) : null;
        //        if (tissue == null && zone != null)
        //        {
        //            tissue = senderStation.Get(zone.ID);
        //        }
        //        if (SystemSet.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
        //        {
        //            var Senders = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
        //            if (Senders.Count > 0)
        //            {
        //                tissue = Senders[0];
        //            }
        //        }
        //        var listDot = dotStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
        //        var listCoil = coilStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
        //        var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
        //        var listLandCoil = listCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listCoil.FindAll(c => c.LandID == land.ID);
        //        var listLandDot= listDot == null ? new List<BuildLandBoundaryAddressDot>() : listDot.FindAll(c => c.LandID == land.ID);
        //        ExportSingleLandTable export = new ExportSingleLandTable(dictoryName);
        //        string templatePath = TemplateHelper.ExcelTemplate("西藏农村土地承包经营权承包地块调查表");  //模板文件
        //        string landNumber = land.LandNumber.Length > 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber;
        //        var filePath = SystemSet.DefaultPath + vp.FamilyNumber.PadLeft(4, '0') + "-" + vp.Name + "-" + landNumber + "-" + TemplateFile.ContractAccountLandSurveyWord;
        //        export.TempletePath = templatePath;
        //        export.CurrentLand = land;
        //        export.Contracter = vp;
        //        export.CurrentZone = zone;
        //        export.Concord = concord;
        //        export.Tissue = tissue;
        //        export.LineList = listLandCoil;
        //        export.DotList = listLandDot;
        //        export.Write();
        //        export.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        flag = false;
        //        YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Excel表)", ex.Message + ex.StackTrace);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //    return flag;

        //}
        //#endregion

        #region 承包地块调查表_新表（以户为单位导出）

        private void btnExportLandExcel_Click(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            string titletip = "承包地块调查表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            IDbContext DbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                var dicStation = DbContext.CreateDictWorkStation();
                List<Dictionary> DictList = dicStation.Get();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.CurrentZoneNoLand);
                        return;
                    }
                    List<VirtualPerson> listPerson = new List<VirtualPerson>();
                    if (contractAccountPanel.IsBatch)
                    {
                        //界面上没有选择承包方项(此时弹出承包方选择界面)
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportLandDataWordNoSelected);
                                return;
                            }
                            //导出承包方集合
                            ExportLandWordTask(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, selectPage.SelectedPersons, DbContext, currentZone);
                        });
                    }
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox(ContractAccountInfo.ExportLandDataWord, "请选择承包方进行导出！");
                            return;
                        }
                        else
                        {
                            //导出单户承包方
                            ExportSingleLandSurveyTable(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, currentAccountItem.Tag, DbContext, currentZone);
                        }
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    //组任务
                    ExportLandWordTaskGroup(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.ExportSurveyTableData, DbContext, currentZone);
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportLandDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出承包地块Excel调查表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出地块调查表(单个任务)
        /// </summary>
        private void ExportLandWordTask(string taskDes, string taskName, List<VirtualPerson> selectedPersons, IDbContext DbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportLandDataWord);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                TaskExportLandWordArgument argument = new TaskExportLandWordArgument();
                argument.DbContext = DbContext;
                argument.CurrentZone = currentZone;
                argument.FileName = extPage.FileName;
                argument.SelectedPersons = selectedPersons;
                TaskExportLandWordOperation operation = new TaskExportLandWordOperation();
                operation.Argument = argument;
                operation.Description = ContractAccountInfo.ExportLandDataWord;
                operation.Name = ContractAccountInfo.ExportLandDataWord;
                operation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(operation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                operation.StartAsync();
            });
        }

        /// <summary>
        /// 批量导出地块调查表(组任务)
        /// </summary>
        private void ExportLandWordTaskGroup(string taskDes, string taskName, IDbContext DbContext, Zone currentZone)
        {
            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportLandDataWord);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                TaskGroupExportLandWordArgument groupArgument = new TaskGroupExportLandWordArgument();
                groupArgument.DbContext = DbContext;
                groupArgument.CurrentZone = currentZone;
                groupArgument.FileName = extPage.FileName;
                TaskGroupExportLandWordOperation groupOperation = new TaskGroupExportLandWordOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Description = taskDes;
                groupOperation.Name = taskName;
                groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                });
                Workpage.TaskCenter.Add(groupOperation);
                if (contractAccountPanel.ShowTaskViewer != null)
                {
                    contractAccountPanel.ShowTaskViewer();
                }
                groupOperation.StartAsync();
            });
        }

        /// <summary>
        /// 单户导出农村土地承包经营权地块调查表，直接预览
        /// </summary>
        private bool ExportSingleLandSurveyTable(string taskDes, string taskName, VirtualPerson contractor, IDbContext DbContext, Zone currentZone)
        {
            bool flag = true;
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                ShowBox("地块调查表", "系统数据源无效!");
                return false;
            }
            GetContractAccountSelectItem();
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var listLand = landStation.GetCollection(contractor.ID);
                var dicStation = dbContext.CreateDictWorkStation();
                var concord = concordStation.Get(currentZone.ID);
                var dicList = dicStation.Get();
                var tissue = senderStation.Get(currentZone.ID);
                if (SystemSet.ExportTableSenderDesToVillage && currentZone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(currentZone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        tissue = Senders[0];
                    }
                }
                ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                string templatePath = TemplateHelper.WordTemplate("西藏农村土地承包经营权承包地块调查表");  //模板文件
                var filePath = SystemSet.DefaultPath + contractor.FamilyNumber.PadLeft(4, '0') + "-" + contractor.Name + "-" + TemplateFile.ContractAccountLandSurveyWord;
                export.CurrentZone = currentZone;
                export.Concord = concord;
                export.Tissue = tissue;
                export.Contractor = contractor;
                export.LandCollection = listLand;
                export.DictList = dicList;
                export.OpenTemplate(templatePath);
                export.PrintPreview(contractor);
            }
            catch (Exception ex)
            {
                flag = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        #endregion 承包地块调查表_新表（以户为单位导出）

        #region 公示结果归户表

        private void btnAgriLandTable_Click(object sender, RoutedEventArgs e)
        {
            btnSurveyTable.IsOpen = false;
            GetContractAccountSelectItem();
            string titletip = "公示结果归户表";
            var currentZone = contractAccountPanel.CurrentZone;
            if (currentZone == null)
            {
                ShowBox(titletip, ContractAccountInfo.ExportNoZone);
                return;
            }
            try
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                var zoneStation = dbContext.CreateZoneWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var fbfStation = dbContext.CreateSenderWorkStation();
                int childrenCount = zoneStation.Count(currentZone.FullCode, eLevelOption.Subs);
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenCount == 0))
                {
                    //单个任务
                    if (contractAccountPanel.accountLandItems == null || contractAccountPanel.accountLandItems.Count == 0)
                    {
                        ShowBox(titletip, ContractAccountInfo.CurrentZoneNoPerson);
                        return;
                    }
                    //界面上选择批量(此时弹出承包方选择界面)
                    if (contractAccountPanel.IsBatch)
                    {
                        List<VirtualPerson> listPerson = new List<VirtualPerson>();
                        ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
                        selectPage.Workpage = Workpage;
                        foreach (var item in contractAccountPanel.accountLandItems)
                        {
                            listPerson.Add(item.Tag);
                        }
                        selectPage.PersonItems = listPerson;
                        Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                        {
                            if (!(bool)b)
                            {
                                return;
                            }
                            if (selectPage.SelectedPersons == null || selectPage.SelectedPersons.Count == 0)
                            {
                                ShowBox(titletip, "请选择组上需要导出的承包方");
                                return;
                            }
                            ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, titletip);
                            extPage.Workpage = Workpage;
                            Workpage.Page.ShowMessageBox(extPage, (bb, rr) =>
                            {
                                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                                {
                                    return;
                                }
                                ExportLandPublicityTask(currentZone, selectPage.SelectedPersons, dbContext, extPage.FileName);
                            });
                        });
                    }
                    else
                    {
                        if (currentAccountItem == null)
                        {
                            ShowBox(ContractAccountInfo.ExportPublicDataWord, "请选择一条数据进行预览！");
                            return;
                        }
                        var filename = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏农村承包经营权公示结果归户表.dot";
                        var dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
                        var export = new ExportLandPublicityTable(filename, dictoryName);
                        export.Contractor = currentAccountItem.Tag;
                        export.CurrentZone = currentZone;
                        export.Tissue = fbfStation.Get(currentZone.ID);
                        export.Concord = concordStation.GetAllConcordByFamilyID(export.Contractor.ID).FirstOrDefault();
                        export.LandCollection = landStation.GetCollection(export.Contractor.ID);  //地块集合

                        export.OpenTemplate(filename);
                        export.PrintPreview(currentAccountItem.Tag);
                    }
                }
                else if ((currentZone.Level == eZoneLevel.Village || currentZone.Level == eZoneLevel.Town) && childrenCount > 0)
                {
                    ExportDataPage extPage = new ExportDataPage(currentZone.FullName, Workpage, ContractAccountInfo.ExportPublicDataWord);
                    extPage.Workpage = Workpage;
                    Workpage.Page.ShowMessageBox(extPage, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                        {
                            return;
                        }
                        //组任务
                        var argument = new TaskGroupExportLandPublicityArgument();
                        argument.DbContext = dbContext;
                        argument.CurrentZone = currentZone;
                        argument.FileName = extPage.FileName;
                        argument.SystemDefine = contractAccountPanel.SystemSet;

                        var operation = new TaskGroupExportLandPublicityOperation();
                        operation.Argument = argument;
                        operation.Description = ContractAccountInfo.ExportPublicDataWord;
                        operation.Name = ContractAccountInfo.ExportPublicDataWord;
                        operation.Completed += new TaskCompletedEventHandler((o, t) =>
                        {
                        });
                        Workpage.TaskCenter.Add(operation);
                        if (contractAccountPanel.ShowTaskViewer != null)
                        {
                            contractAccountPanel.ShowTaskViewer();
                        }
                        operation.StartAsync();
                    });
                }
                else
                {
                    //选择地域大于镇
                    ShowBox(ContractAccountInfo.ExportPublicDataWord, ContractAccountInfo.VolumnExportZoneError);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishWord(导出公示结果归户表)", ex.Message + ex.StackTrace);
                return;
            }
        }

        /// <summary>
        /// 导出公示结果归户表
        /// </summary>
        private void ExportLandPublicityTask(Zone currentZone, List<YuLinTu.Library.Entity.VirtualPerson> listPerson, IDbContext dbContext, string filePath)
        {
            var argument = new TaskExportLandPublicityArgument();
            argument.DbContext = dbContext;
            argument.CurrentZone = currentZone;
            argument.FileName = filePath;
            argument.SelectedPersons = listPerson;
            argument.SystemDefine = contractAccountPanel.SystemSet;

            var operation = new TaskExportLandPublicityOperation();
            operation.Argument = argument;
            operation.Description = ContractAccountInfo.ExportPublicDataWord;
            operation.Name = ContractAccountInfo.ExportPublicDataWord;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
            });
            Workpage.TaskCenter.Add(operation);
            if (contractAccountPanel.ShowTaskViewer != null)
            {
                contractAccountPanel.ShowTaskViewer();
            }
            operation.StartAsync();
        }

        #endregion 公示结果归户表

        #region 其他

        #region Method-Concord

        /// <summary>
        /// 公共导出文件操作任务
        /// </summary>
        private void ExportDataCommonOperateTask(string zoneName, string header, eContractRegeditBookType type,
            string taskDes, string taskName, List<VirtualPerson> listPerson = null, DateTime? time = null, DateTime? pubTime = null)
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, contractRegeditBookPanel.ThePage, header);
            extPage.Workpage = contractRegeditBookPanel.ThePage;
            contractRegeditBookPanel.ThePage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                switch (type)
                {
                    case eContractRegeditBookType.BatchExportRegeditBookData:
                        ExportRegeditBookTaskGroup(extPage.FileName, taskDes, taskName);
                        break;

                    case eContractRegeditBookType.ExportRegeditBookData:
                        ExportRegeditBookTask(extPage.FileName, taskDes, taskName, listPerson);
                        break;

                    case eContractRegeditBookType.ExportWarrant:
                        ExportWarrentTask(extPage.FileName, taskDes, taskName, listPerson);
                        break;

                    case eContractRegeditBookType.BatchExportWarrant:
                        ExportWarrentTaskGroup(extPage.FileName, taskDes, taskName);
                        break;
                }
            });
        }

        /// <summary>
        /// 初始化地块示意图名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeLandImageName(string filePath, VirtualPerson family)
        {
            if (family == null)
            {
                return "";
            }
            string imagePath = filePath + "\\" + family.Name;   //取承包方名作为上级文件夹名
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string imageName = imagePath + "\\" + "DKSYT" + family.ZoneCode;
            int number = 0;
            Int32.TryParse(family.FamilyNumber, out number);
            imageName += string.Format("{0:D4}", number);
            imageName += "J";
            return imageName;
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext dbContext)
        {
            string excelName = string.Empty;
            if (zone == null || dbContext == null)
                return excelName;
            Zone parent = GetParent(zone, dbContext);  //获取上级地域
            string parentName = parent == null ? "" : parent.Name;
            if (zone.Level == eZoneLevel.County)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parentName + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, dbContext);
                string parentTownName = parentTowm == null ? "" : parentTowm.Name;
                excelName = parentTownName + parentName + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        ///  获取上级地域
        /// </summary>
        private Zone GetParent(Zone zone, IDbContext dbContext)
        {
            Zone parentZone = null;
            if (zone == null || dbContext == null)
                return parentZone;
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                parentZone = zoneStation.Get(zone.UpLevelCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取父级地域失败!)", ex.Message + ex.StackTrace);
            }
            return parentZone;
        }

        /// <summary>
        /// 获取子级地域(包括当前地域)
        /// </summary>
        public List<Zone> GetAllChildrenZones()
        {
            List<Zone> allZones = new List<Zone>();
            IDbContext dbContext = DataBaseSource.GetDataBaseSource();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取子级地域失败)", ex.Message + ex.StackTrace);
                //ShowBox(ContractAccountInfo.ImportBoundaryAddressDot, "获取当前地域下的子级地域失败!");
                return null;
            }
            return allZones;
        }

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary>
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level, IZoneWorkStation zoneStation)
        {
            Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level, zoneStation);
        }

        /// <summary>
        /// 获取合同选择项
        /// </summary>
        private void GetConcordSelectItem()
        {
            currentItem = null;
            currentConcord = null;
            var item = concordPanel.view.SelectedItem;
            if (item is BindConcord)
            {
                BindConcord bp = item as BindConcord;
                currentConcord = bp;
                currentItem = concordPanel.Items.FirstOrDefault(t => t.ID == bp.Tag.ContracterId);
            }
            if (item is ConcordItem)
            {
                currentItem = concordPanel.view.SelectedItem as ConcordItem;
            }
        }

        #endregion Method-Concord

        /// <summary>
        /// 获取承包台账选择项
        /// </summary>
        private void GetContractAccountSelectItem()
        {
            currentAccountItem = null;
            currentLandBinding = null;
            var item = contractAccountPanel.view.SelectedItem;
            if (item is ContractLandPersonItem)
            {
                //界面上选中的是承包方(界面实体)
                currentAccountItem = item as ContractLandPersonItem;
            }
            if (item is ContractLandBinding)
            {
                //界面上选中的是承包地块(界面实体)
                currentLandBinding = item as ContractLandBinding;
                var personItem = contractAccountPanel.accountLandItems.FirstOrDefault(c => c.Tag.ID == currentLandBinding.Tag.OwnerId);
                currentAccountItem = personItem;
            }
        }

        /// <summary>
        /// 获取当前选择项
        /// </summary>
        private void GetContractRegeditBookSelectItem()
        {
            currentRegiditItem = null;
            currentConcord = null;
            currentRegeditBook = null;
            var item = contractRegeditBookPanel.view.SelectedItem;
            if (item is YuLinTu.Library.Controls.BindContractRegeditBook)
            {
                YuLinTu.Library.Controls.BindContractRegeditBook bp = item as YuLinTu.Library.Controls.BindContractRegeditBook;
                currentRegeditBook = bp;
                ContractConcord concord = contractRegeditBookPanel.ConcordBusiness.Get(bp.Tag.ID);
                if (concord == null) return;
                currentRegiditItem = contractRegeditBookPanel.Items.FirstOrDefault(t => t.ID == concord.ContracterId);
            }
            if (item is ContractRegeditBookItem)
            {
                currentRegiditItem = contractRegeditBookPanel.view.SelectedItem as YuLinTu.Library.Controls.ContractRegeditBookItem;
            }
        }

        public List<Zone> GetParentZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DataBaseSource.GetDataBaseSource();
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取子级地域(包括当前地域)
        /// </summary>
        public List<Zone> GetAllChildrenZones(Zone currentZone, string header = "", IDbContext dbContext = null)
        {
            List<Zone> allZones = new List<Zone>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetAllChildrenZones(获取子级地域失败)", ex.Message + ex.StackTrace);
                ShowBox(header, "获取当前地域下的子级地域失败!");
                return allZones;
            }
            return allZones;
        }

        /// <summary>
        /// 是否继续操作
        /// </summary>
        /// <returns></returns>
        private bool CanContinue(string tip)
        {
            if (contractAccountPanel.CurrentZone == null)
            {
                ShowBox(tip, "当前行政区域无效!");
                return false;
            }
            IDbContext db = DataBaseSource.GetDataBaseSource();
            var vpstation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            int count = vpstation.Count(c => c.ZoneCode.StartsWith(contractAccountPanel.CurrentZone.FullCode));
            if (count <= 0)
            {
                ShowBox(tip, "当前行政区域下没有数据可供操作!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                }, action);
            }));
        }

        #endregion 其他

        #endregion Methods
    }
}