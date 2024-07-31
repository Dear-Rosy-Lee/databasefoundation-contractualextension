using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Component.StockRightBase.Bussiness;
using YuLinTu.Component.StockRightBase.Model;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Component.StockRightBase.ExportTask;
using YuLinTu.Component.StockRightBase.Helper;

namespace YuLinTu.Component.StockRightBase.Control
{
    /// <summary>
    /// MainFramePage.xaml 的交互逻辑
    /// </summary>
    //[Newable(true,
    // Name = "确股",
    // Category = "应用",
    // Icon = "pack://application:,,,/YuLinTu.NationalSurvey.Resources;component/Resources/_2D饼.png",
    // Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/统计78.png",
    // Order = 0,
    // IsNeedAuthenticated = true,
    // IsLanguageName = false)]
    public partial class MainFramePage : NavigatableWorkpageFrame
    {

        private IDbContext _dbContext;
        private ImportLandBussness _bussinessData;
        private Zone _currentZone;

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

        public IDbContext DbContext
        {
            get
            {
                return _dbContext;
            }
            set
            {
                _dbContext = value;
                personPanel.DbContext = _dbContext;
                landPanel.DbContext = _dbContext;
                new UpdateDatabaseHelper().AddTable(DbContext);
                ImportLandBussness = new ImportLandBussness(DbContext, CurrentZone);
            }
        }

        public ImportLandBussness ImportLandBussness
        {
            get { return _bussinessData; }
            set { _bussinessData = value; }
        }

        public Zone CurrentZone
        {
            get { return _currentZone; }
            set
            {
                _currentZone = value;
            }
        }

        public MainFramePage()
        {
            InitializeComponent();
            NavigatorType = Windows.Wpf.Metro.Components.eNavigatorType.TreeView;
            bool designTime =
                (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
            landPanel.SelectChangedAction = SelectChangeAction;
            landPanel.ItemDoubleClick += ItemDoubleClick;
            landPanel.SelectChangedAction += SelectChangeAction;
            personPanel.personGrid.view.SelectionChanged += Selector_OnSelectionChanged;

            personPanel.Initlized();
            landPanel.Initlized();
            //ImportLandBussness = new ImportLandBussness(DbContext, CurrentZone);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DbContext == null || CurrentZone == null)
            {
                return;
            }
            ObservableCollection<ContractLand> landList = new ObservableCollection<ContractLand>();
            var currentPerson = personPanel.personGrid.SelectedItem as PersonGridModel;
            if (currentPerson != null)
            {
                try
                {
                    var person = currentPerson.VirtualPerson;
                    var importLandBussness = new ImportLandBussness(DbContext, CurrentZone);
                    var data = importLandBussness.GetBussinessObject(_currentZone);
                    if (person != null && data.ContractLands != null)
                    {
                        var currentPersonID = person.ID;
                        var belongRelationStation = _dbContext.CreateBelongRelationWorkStation();
                        var personRelations = belongRelationStation.Get().Where(p => p.VirtualPersonID == currentPersonID).ToList();
                        var personLands = belongRelationStation.GetLandByPerson(currentPersonID, _currentZone.FullCode)?.Where(s => s.IsStockLand).ToList();//获取人的确股地
                        if (personRelations != null && personRelations.Count > 0 && personLands != null && personLands.Count > 0)
                        {
                            foreach (var land in personLands)
                            {
                                var relation = personRelations.FirstOrDefault(s => s.LandID == land.ID);
                                var quantificationArea = relation != null ? relation.QuanficationArea : 0;
                                land.QuantificicationArea = quantificationArea;
                                land.TableArea = relation != null ? relation.TableArea : 0;
                                landList.Add(land);
                            }
                        }
                    }
                    landPanel.LandGrid.view.Roots = landList;
                }
                catch (Exception ex)
                {
                    ShowBox("导入调查信息表", "数据读取异常，可能需要升级数据库！\n" + ex.Message);
                    return;
                }
            }
        }

        public void SwitchZoneCode(Zone zone)
        {
            personPanel.CurrentZone = zone;
            landPanel.CurrentZone = zone;
            CurrentZone = zone;
        }

        protected override void OnWorkpageChanged()
        {
            base.OnWorkpageChanged();
            personPanel.Workpage = Workpage;
            landPanel.Workpage = Workpage;
        }

        /// <summary>
        /// 选择地块只显示当前属于当前地块的股农，事件委托实现方法
        /// </summary>
        public void SelectChangeAction()
        {
        }

        /// <summary>
        /// 消息显示框
        /// </summary>
        public void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        #region 股农管理
        /// <summary>
        /// 添加股农
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddPerson_Click(object sender, RoutedEventArgs e)
        {
            PersonManage(true);
        }

        /// <summary>
        /// 删除股农
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DeletePerson_Click(object sender, RoutedEventArgs e)
        {
            PersonManage(false);
        }

        /// <summary>
        /// 股农管理
        /// </summary>
        /// <param name="isAdd">true 添加 false 删除</param>
        private void PersonManage(bool isAdd)
        {
            var selectPage = new ContractRegeditBookPersonLockPage { Workpage = Workpage };
            selectPage.PersonItems = DbContext.CreateVirtualPersonStation<LandVirtualPerson>().Get(o => o.IsStockFarmer == !isAdd && o.ZoneCode == CurrentZone.FullCode).Cast<VirtualPerson>().ToList();
            Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(selectPage, (b, r) =>
                {
                    if (b != null && !(bool)b)
                    {
                        return;
                    }
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        foreach (var item in selectPage.SelectedPersons)
                        {
                            item.IsStockFarmer = isAdd;
                        }
                        var pnStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                        pnStation.UpdatePersonList(selectPage.SelectedPersons);
                        Dispatcher.Invoke(new Action(() => Refresh()));
                    });
                });
            }));
        }
        #endregion

        #region 地块管理
        /// <summary>
        /// 添加地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddLand_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (CurrentZone.Level != eZoneLevel.Village && CurrentZone.Level != eZoneLevel.Group)
            {
                //选择地域不是村或者组
                ShowBox(ContractAccountInfo.ContractLandAdd, ContractAccountInfo.ZoneSelectedErrorForAdd);
                return;
            }
            ContractLandPage addPage = new ContractLandPage(true);
            addPage.Workpage = Workpage;
            addPage.CurrentZone = CurrentZone;
            addPage.IsStockRight = true;
            addPage.CurrentLand = new ContractLand();
            addPage.CurrentLand.IsStockLand = true;
            Workpage.Page.ShowMessageBox(addPage, (b, r) =>
            {
                if (b != null && !(bool)b)
                {
                    return;
                }
                var land = addPage.CurrentLand;
                Dispatcher.Invoke(new Action(() => Refresh()));
            });
        }

        private void ItemDoubleClick()
        {
            Btn_EditLand_Click(null, null);
        }

        /// <summary>
        /// 编辑地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_EditLand_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandEdit, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            if (landPanel.SelectedItem == null)
            {
                ShowBox(ContractAccountInfo.ContractLandEdit, "未选择编辑地块！");
                return;
            }
            ContractLandPage editPage = new ContractLandPage();
            editPage.Workpage = Workpage;
            var landTemp = landPanel.SelectedItem;
            editPage.CurrentLand = landTemp;
            editPage.CurrentZone = CurrentZone;
            editPage.IsStockRight = true;
            Workpage.Page.ShowMessageBox(editPage, (b, r) =>
            {
                if (b != null && !(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() => Refresh()));
            });
        }

        /// <summary>
        /// 删除地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DeleteLand_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                //地域为空
                ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            var land = landPanel.SelectedItem;
            if (land == null)
            {
                ShowBox(ContractAccountInfo.ContractLandDel, "未选择承包地块");
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b != null && !(bool)b)
                {
                    return;
                }
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var station = DbContext.CreateContractLandWorkstation();
                    station.Delete(land.ID);
                    var result = DbContext.CreateBelongRelationWorkStation().Delete(o => o.LandID == land.ID);
                    Dispatcher.Invoke(new Action(() => Refresh()));
                });
            });
            ShowBox(ContractAccountInfo.ContractLandDel, ContractAccountInfo.CurrentLandDelSure, eMessageGrade.Infomation, action);
        }

        #endregion

        #region 股权分配

        /// <summary>
        ///股权量化界面校验
        /// </summary>
        private bool StockQuanlification()
        {
            if (CurrentZone == null)
            {
                ShowBox("股权量化", "请选择行政地域！");
                return false;
            }
            if (personPanel.Items.Count == 0)
            {
                ShowBox("股权量化", "请导入股农数据！");
                return false;
            }
            if (landPanel.LandGrid.view.Items.Count == 0)
            {
                ShowBox("股权量化", "请导入地块数据！");
                return false;
            }
            return true;

        }


        /// <summary>
        /// 股权量化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StockQuanlification_Click(object sender, RoutedEventArgs e)
        {
            //if (!StockQuanlification())
            //    return;
            //var dlg = new StockQuanlification
            //{
            //    CurrentZone = CurrentZone,
            //    Owner = Workpage.Workspace.Window.Instance as Window
            //};
            //var bussnessObject = _bussinessData.GetBussinessObject(CurrentZone);
            //dlg.Model.AreaTotality = bussnessObject.ContractLands.Sum(o => o.QuantificicationArea);
            //foreach (var person in bussnessObject.VirtualPersons)
            //{
            //    dlg.Model.StockTotality += person.SharePersonList.Count;
            //}
            //dlg.Model.SingleStockArea = Math.Round(dlg.Model.AreaTotality / dlg.Model.StockTotality, 2);
            //Workpage.Workspace.ApplyTheme(dlg);
            //if (dlg.ShowDialog() == true)
            //{
            //    foreach (var person in bussnessObject.VirtualPersons)
            //    {
            //        var sharePersonList = new List<Person>();
            //        person.SharePersonList.ForEach(o =>
            //        {
            //            o.StockQuantity = 1;
            //            o.StockArea = dlg.Model.SingleStockArea;
            //            sharePersonList.Add(o);
            //        });
            //        person.SharePersonList = sharePersonList;
            //        person.StockTotality = person.SharePersonList.Count;
            //        person.StockAreaTotality = person.StockTotality * dlg.Model.SingleStockArea;
            //        _bussinessData.UpdataPerson(person);
            //    }
            //    Dispatcher.Invoke(new Action(() => Refresh()));
            //}
        }

        /// <summary>
        /// 股权分配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_StockDistribution_Click(object sender, RoutedEventArgs e)
        {
            if (!StockQuanlification()) return;
            var bussnessObject = _bussinessData.GetBussinessObject(CurrentZone);
            var dlg = new StockDistribution();
            dlg.Model.BussinessObject = bussnessObject;
            dlg.Model.StockTotality = bussnessObject.VirtualPersons.Sum(o => o.SharePersonList.Count);
            dlg.Model.AreaAll = bussnessObject.ContractLands.Sum(o => o.ActualArea);
            dlg.Model.CollectiveRemainStock = bussnessObject.ContractLands.Sum(o => o.ObligateArea);
            dlg.Model.IsPersonWay = true;
            //dlg.Model.UnitStock = dlg.Model.StockAreaAll / dlg.Model.StockTotality;
            Workpage.Page.ShowMessageBox(dlg, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (dlg.Model.StockDistributionWays == Enum.StockDistributionWays.按人)
                {
                    foreach (var person in bussnessObject.VirtualPersons)
                    {
                        var sharePersonList = new List<Person>();
                        person.SharePersonList.ForEach(o =>
                        {
                            o.StockQuantity = dlg.Model.UnitStockNum;
                            o.StockArea = dlg.Model.UnitStockArea;
                            sharePersonList.Add(o);
                        });
                        person.SharePersonList = sharePersonList;
                        person.StockTotality = person.SharePersonList.Count;
                        person.StockAreaTotality = person.StockTotality * dlg.Model.UnitStockArea;
                        _bussinessData.UpdatePerson(person);
                    }

                }
                if (dlg.Model.StockDistributionWays == Enum.StockDistributionWays.按户)
                {
                    bussnessObject.VirtualPersons.ToList().ForEach(o =>
                    {
                        o.StockTotality = dlg.Model.UnitStockNum;
                        o.StockAreaTotality = dlg.Model.UnitStockArea;
                        _bussinessData.UpdatePerson(o);
                    });
                }
                //if (dlg.Model.StockDistributionWays == Enum.StockDistributionWays.自定义)
                //{
                //}
                Dispatcher.Invoke(new Action(() => Refresh()));
                ShowBox("股权分配", "股权分配成功", eMessageGrade.Infomation);
            });
        }
        #endregion 

        #region  工具

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }


        /// <summary>
        /// 清空股农
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ClearPerson_Click(object sender, RoutedEventArgs e)
        {
            if (new ImportLandBussness(_dbContext, _currentZone).DeleteStockPerson())
            {
                ShowBox("清空股农", "清空股农成功", eMessageGrade.Infomation);
                Refresh();
            }
            else
                ShowBox("清空股农", "清空股农失败");
        }

        /// <summary>
        /// 清空地块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ClearLand_Click(object sender, RoutedEventArgs e)
        {
            if (new ImportLandBussness(_dbContext, _currentZone).DeleteLand())
            {
                ShowBox("清空地块", "清空地块成功", eMessageGrade.Infomation);
                Refresh();
            }
            else
                ShowBox("清空地块", "清空地块失败");
        }

        /// <summary>
        /// 清空股权关系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ClearRelation_Click(object sender, RoutedEventArgs e)
        {
            if (new ImportLandBussness(_dbContext, _currentZone).DeleteRelation())
            {
                ShowBox("清空地块", "清空股权关系成功", eMessageGrade.Infomation);
                Refresh();
            }
            else
                ShowBox("清空地块", "清空股权关系失败");
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            landPanel.Refresh();
            personPanel.Refresh();
            RefreshContractAcount();
        }

        /// <summary>
        /// 通知承包台账刷新
        /// </summary>
        private void RefreshContractAcount()
        {
            var arg = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_Refresh, null);
            Workpage.Workspace.Message.Send(this, arg);
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DateBaseExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentZone == null)
                {
                    ShowBox("数据转换", "请选择地域！");
                    return;
                }
                DbContext.CreateBelongRelationWorkStation().DataBaseExchange(CurrentZone?.FullCode);
                Refresh();
                ShowBox("数据转换", "数据转换成功", eMessageGrade.Infomation);
            }
            catch
            {
                ShowBox("数据转换", "数据转换失败!请不要重复转换数据");
            }
        }
        #endregion

        #region 导入数据
        /// <summary>
        /// 导入调查信息表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Importland_Click(object sender, RoutedEventArgs e)
        {
            this.btnImportExcel.IsOpen = false;
            ImportExcel(new ReadExcelBase(), new ImportLandBussness(DbContext, CurrentZone), "导入确股地块调查表");
        }

        /// <summary>
        /// 导入调查表任务
        /// </summary>
        /// <param name="fileName">选择文件路径</param>
        private void ImportInformationTask(string fileName, ReadExcelBase importExcelBase, BussinessData bussinessData, string taskName)
        {
            TaskImportLandTableArgument meta = new TaskImportLandTableArgument();
            meta.DbContext = DbContext;       //当前使用的数据库
            meta.CurrentZone = CurrentZone;    //当前地域
            meta.FileName = fileName;
            meta.VirtualType = eVirtualType.Land;
            ImportStockRightBaseTask import = new ImportStockRightBaseTask();
            import.BussinessData = bussinessData;
            import.ImportExcelBase = importExcelBase;
            import.Argument = meta;
            import.Description = taskName + "数据";
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(Refresh), null);
            });
            import.Terminated += new TaskTerminatedEventHandler((o, t) =>
            {
                Dispatcher.Invoke(new Action(() => { ShowBox(VirtualPersonInfo.ImportData, VirtualPersonInfo.ImportDataFail); }));
            });
            Workpage.TaskCenter.Add(import);
            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            import.StartAsync();

        }

        /// <summary>
        /// 导入Excel公用方法
        /// </summary>
        /// <param name="importExcelBase"></param>
        private void ImportExcel(ReadExcelBase importExcelBase, BussinessData bussinessData, string taskName)
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportZone, ContractAccountInfo.ImportZone);
                return;
            }
            List<Zone> childrenZones = DbContext.CreateZoneWorkStation().GetChildren(CurrentZone.FullCode, eLevelOption.Subs);
            if ((CurrentZone.Level == eZoneLevel.Group) || (CurrentZone.Level == eZoneLevel.Village && childrenZones.Count == 0))
            {
                //选择为组级地域或者选择为村级地域的同时地域下没有子级地域(单个表格导入,执行单个任务)
                ImportDataPage importLand = new ImportDataPage(Workpage, "导入地块数据调查表");
                Workpage.Page.ShowMessageBox(importLand, (b, r) =>
                {
                    if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                    {
                        return;
                    }
                    ImportInformationTask(importLand.FileName, importExcelBase, bussinessData, taskName);
                });
            }
            else
            {
                ShowBox(ContractAccountInfo.ImportZone, ContractAccountInfo.ImportErrorZone);
                return;
            }
        }

        #endregion

        #region 导出表格
        /// <summary>
        /// 导出地块调查表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Exportland_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 导出股农调查表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportPerson_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 登记簿册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportRegister_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 导出公示表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportPublic_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 导出股权量化章程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportStock_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 图斑
        /// <summary>
        /// 导出矢量图斑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ExportShape_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            ExportContractLandShapePage exportPage = new ExportContractLandShapePage(Workpage, "导出当前地域下地块Shape数据");
            Workpage.Page.ShowMessageBox(exportPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                List<Zone> allZones = new List<Zone>();
                allZones = GetAllChildrenZones();
                List<Zone> allChildrenZones = new List<Zone>();
                allChildrenZones = allZones.FindAll(c => c.FullCode != CurrentZone.FullCode);
                var vpstation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> vps = vpstation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);

                if (CurrentZone.Level == eZoneLevel.Group || (CurrentZone.Level > eZoneLevel.Group && allChildrenZones.Count == 0))
                {
                    //执行单个任务
                    TaskExportLandShapeArgument meta = new TaskExportLandShapeArgument();
                    meta.CurrentZone = CurrentZone;
                    meta.FileName = exportPage.FileName;
                    meta.Database = DbContext;
                    meta.DictList = DbContext.CreateDictWorkStation().Get();
                    meta.vps = vps;
                    TaskExportLandShapeOperation import = new TaskExportLandShapeOperation();
                    import.Argument = meta;
                    import.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandShapeData;
                    import.Name = ContractAccountInfo.ExportLandShapeData;
                    import.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_EXPORTLANDSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    Workpage.TaskCenter.Add(import);
                    Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
                    import.StartAsync();
                }
                else if ((CurrentZone.Level == eZoneLevel.Town || CurrentZone.Level == eZoneLevel.Village) && allChildrenZones.Count > 0)
                {
                    //执行批量任务(含有子任务)
                    TaskGroupExportLandShapeArgument groupMeta = new TaskGroupExportLandShapeArgument();
                    groupMeta.CurrentZone = CurrentZone;
                    groupMeta.AllZones = allZones;
                    groupMeta.Database = DbContext;
                    groupMeta.vps = vps;
                    groupMeta.FileName = exportPage.FileName;
                    groupMeta.DictList = DbContext.CreateDictWorkStation().Get();
                    TaskGroupExportLandShapeOperation taskGroup = new TaskGroupExportLandShapeOperation();
                    taskGroup.Argument = groupMeta;
                    taskGroup.Description = "导出" + CurrentZone.FullName + ContractAccountInfo.ExportLandShapeData;
                    taskGroup.Name = ContractAccountInfo.ExportLandShapeData;
                    taskGroup.Completed += new TaskCompletedEventHandler((o, t) =>
                    {
                        ModuleMsgArgs args = MessageExtend.ContractAccountMsg(DbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTDOTSHAPE_COMPLETE, exportPage.FileName, CurrentZone.FullCode);
                        TheBns.Current.Message.Send(this, args);
                    });
                    Workpage.TaskCenter.Add(taskGroup);
                    Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
                    taskGroup.StartAsync();
                }
                else
                {
                    //选择地域为镇(或大于镇)
                    ShowBox(ContractAccountInfo.ExportLandShapeData, ContractAccountInfo.ExportZoneError);
                    return;
                }
            });
        }

        /// <summary>
        /// 导入矢量图斑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ImportShape_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportZone, "未选择行政地域");
                return;
            }
            ImportLandShapePage addPage = new ImportLandShapePage(Workpage, "导入地块图斑");
            addPage.ThePage = Workpage;
            addPage.Db = DbContext;
            Workpage.Page.ShowMessageBox(addPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                TaskContractAccountArgument meta = new TaskContractAccountArgument();
                meta.FileName = addPage.FileName;
                meta.IsClear = true;
                meta.ArgType = eContractAccountType.ImportLandShapeData;
                meta.Database = DbContext;
                meta.CurrentZone = CurrentZone;
                meta.VirtualType = eVirtualType.Land;
                meta.UseContractorInfoImport = addPage.UseContractorInfoImport;
                meta.UseLandCodeBindImport = addPage.UseLandCodeBindImport;
                meta.shapeAllcolNameList = addPage.shapeAllcolNameList;
                ImportLandShapeData(meta, DbContext);
            });
        }

        /// <summary>
        /// 导入图斑方法
        /// </summary>
        private void ImportLandShapeData(TaskContractAccountArgument meta, IDbContext dbContext)
        {
            TaskContractAccountOperation import = new TaskContractAccountOperation();
            import.Argument = meta;
            import.Description = ContractAccountInfo.ImportShapeData;
            import.Name = ContractAccountInfo.ImportShpData;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                TheBns.Current.Message.Send(this, MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_IMPORTSHP_COMPLETE, "", CurrentZone.FullCode));
                Refresh();
            });
            Workpage.TaskCenter.Add(import);
            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            import.StartAsync();
        }

        public List<Zone> GetAllChildrenZones()
        {
            List<Zone> allZones = new List<Zone>();
            try
            {
                var zoneStation = DbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取子级地域失败)", ex.Message + ex.StackTrace);
                ShowBox(ContractAccountInfo.ImportBoundaryAddressDot, "获取当前地域下的子级地域失败!");
                return null;
            }
            return allZones;
        }
        #endregion

        #region 合同
        private void Btn_ConcordSet_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ConcordSetting();
            dlg.Workpage = Workpage;
            dlg.DbContext = DbContext;
            dlg.CurrentZone = CurrentZone;
            dlg.Model.Sender = CurrentZone?.FullName;
            Workpage.Page.ShowMessageBox(dlg, (b, r) =>
            {

            });

        }


        private void Btn_PreviewConcord_Click(object sender, RoutedEventArgs e)
        {
            Preview("合同", "农村土地（耕地）承包合同", new ConcordWord());
        }

        private void Btn_ExportConcord_Click(object sender, RoutedEventArgs e)
        {
            Export("合同", "农村土地（耕地）承包合同", new ConcordWord());
        }

        #endregion

        #region 权证

        private void Btn_ExportBook_Click(object sender, RoutedEventArgs e)
        {
            Export("农村土地承包经营权证", "农村土地承包经营权证", new WarrantWord());
        }

        private void Btn_PreviewWarrant_CLick(object sender, RoutedEventArgs e)
        {
            Preview("农村土地承包经营权证", "农村土地承包经营权证", new WarrantWord());
        }


        private void Btn_PreviewRegesterBook_Click(object sender, RoutedEventArgs e)
        {
            Preview("农村土地承包经营权登记簿", "农村土地承包经营权登记簿", new RegisterBookWord());
        }


        private void Btn_ExportRegisterBook_Click(object sender, RoutedEventArgs e)
        {
            Export("农村土地承包经营权登记簿", "农村土地承包经营权登记簿", new RegisterBookWord());
        }

        private void Btn_WarrantSet_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WarrantSetting(DbContext, CurrentZone);
            dlg.Model.CurrentZone = CurrentZone;
            dlg.Model.DbContext = DbContext;
            dlg.Workpage = Workpage;
            dlg.DbContext = DbContext;
            dlg.CurrentZone = CurrentZone;
            Workpage.Page.ShowMessageBox(dlg, (b, r) =>
            {

            });
        }

        #endregion


        #region 导出表，预览表通用方法，目前支持word,Excel需要后期扩展
        /// <summary>
        /// 导出表
        /// </summary>
        /// <param name="name">导表名</param>
        /// <param name="tempName">模板名</param>
        /// <param name="book">导表具体类</param>
        /// <param name="isGroupMode">是否是一组为单位导出（一组导出一张汇总表）</param>
        private void Export(string name, string tempName, AgricultureWordBook book, bool isGroupMode = false)
        {
            try
            {
                if (CurrentZone == null)
                {
                    ShowBox("导出" + name, ContractAccountInfo.ExportNoZone);
                    return;
                }
                if (CurrentZone.Level == eZoneLevel.Group)
                {
                    if (personPanel.Items.Count == 0)
                    {
                        ShowBox("导出" + name, "当前地域下没有股农！");
                        return;
                    }
                    //界面选择批量
                    if (landPanel.LandStatistics.IsBatch)
                    {
                        ExportGroup(name, tempName, book);
                        //ExportGroupMode(name, tempName, book);
                        return;
                    }
                    else
                    {
                        if (personPanel.SelectedItem != null)//导出一户人的数据
                        {
                            ExportSingle(name, tempName, book, new List<VirtualPerson>() { personPanel.SelectedItem.VirtualPerson });
                        }
                        else
                        {
                            ShowBox("导出" + name, "请选择要导出的承包方");
                            return;
                        }
                    }
                }
                else if (CurrentZone.Level == eZoneLevel.Village || CurrentZone.Level == eZoneLevel.Town) //批量多组导出
                {
                    ExportMulti(name, tempName, book, isGroupMode ? new ExportWordGroupMode() : new ExportWordTask());
                }
                else
                {
                    //选择地域大于镇
                    ShowBox("导出" + name, ContractAccountInfo.VolumnExportZoneError);
                }
            }
            catch (Exception ex)
            {
                ShowBox("导出" + name + "失败,请查看日志", "导出" + name);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportPublishWord(" + name + ")", ex.Message + ex.StackTrace);
            }
        }


        private void ExportSingle(string name, string tempName, AgricultureWordBook book, List<VirtualPerson> persons)
        {
            ExportDataPage extPage = new ExportDataPage(CurrentZone.FullName, Workpage, name);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (bb, rr) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || bb == false)
                {
                    return;
                }
                ExportSingleTask(name, tempName, book, persons, extPage.FileName, new ExportWordTask());
            });
        }

        private void ExportSingleTask(string name, string tempName, AgricultureWordBook book, List<VirtualPerson> listPerson, string filePath, ExportWordTask task)
        {
            task.DbContext = DbContext;
            task.CurrentZone = CurrentZone;
            task.FileName = filePath;
            task.SelectedPersons = listPerson;
            task.Description = "导出" + name;
            task.Name = name;
            task.TempName = tempName;
            task.Book = book;
            Workpage.TaskCenter.Add(task);
            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            task.StartAsync();
        }

        private void ExportGroup(string name, string tempName, AgricultureWordBook book)
        {
            List<VirtualPerson> listPerson = new List<VirtualPerson>();
            ContractRegeditBookPersonLockPage selectPage = new ContractRegeditBookPersonLockPage();
            selectPage.Workpage = Workpage;
            foreach (var item in personPanel.Items)
            {
                listPerson.Add(item.VirtualPerson);
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
                    ShowBox("导出" + name, "请选择需要导出的承包方");
                    return;
                }
                ExportDataPage extPage = new ExportDataPage(CurrentZone.FullName, Workpage, name);
                extPage.Workpage = Workpage;
                Workpage.Page.ShowMessageBox(extPage, (bb, rr) =>
                {
                    if (string.IsNullOrEmpty(extPage.FileName) || bb == false)
                    {
                        return;
                    }
                    ExportSingleTask(name, tempName, book, selectPage.SelectedPersons, extPage.FileName, new ExportWordTask());
                });
            });
        }

        private void ExportGroupMode(string name, string tempName, AgricultureWordBook book)
        {
            var listPerson = personPanel.Items.Select(item => item.VirtualPerson).ToList();
            ExportDataPage extPage = new ExportDataPage(CurrentZone.FullName, Workpage, name);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (bb, rr) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || bb == false)
                {
                    return;
                }
                ExportSingleTask(name, tempName, book, listPerson, extPage.FileName, new ExportWordGroupMode());
            });
        }

        private void ExportMulti(string name, string tempName, AgricultureWordBook book, ExportWordTask task)
        {
            ExportDataPage extPage = new ExportDataPage(CurrentZone.FullName, Workpage, "导出" + name);
            extPage.Workpage = Workpage;
            Workpage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                ExportMultiTask(name, tempName, book, CurrentZone, DbContext, extPage.FileName, task);
            });
        }

        private void ExportMultiTask(string name, string tempName, AgricultureWordBook book, Zone currentZone, IDbContext dbContext, string filePath, ExportWordTask task)
        {
            var taskGroup = new ExportWordGroupTask();
            taskGroup.CurrentZone = currentZone;
            taskGroup.DbContext = DbContext;
            taskGroup.FileName = filePath;
            taskGroup.TempName = tempName;
            taskGroup.Book = book;
            taskGroup.Description = "导出" + name;
            taskGroup.Name = name;
            Workpage.TaskCenter.Add(taskGroup);
            Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            taskGroup.StartAsync();
        }

        /// <summary>
        /// 预览
        /// </summary>
        private void Preview(string name, string tempName, AgricultureWordBook Book)
        {
            if (CurrentZone == null)
            {
                ShowBox("预览" + name, ContractAccountInfo.ExportNoZone);
                return;
            }
            if (personPanel.SelectedItem == null)
            {
                ShowBox("预览" + name, "请选择需要预览的承包方!");
                return;
            }

            try
            {
                var stockLandsvp = DbContext.CreateBelongRelationWorkStation().GetLandByPerson(personPanel.SelectedItem.VirtualPerson.ID, CurrentZone.FullCode);
                Book.StockLands = stockLandsvp;
                if (stockLandsvp.Count == 0)
                {
                    ShowBox("预览" + name, "当前股农没有确股地块!");
                    return;
                }
                var concord = DbContext.CreateStockConcordWorkStation().Get(o => o.ZoneCode == CurrentZone.FullCode && o.ContracterId == personPanel.SelectedItem.VirtualPerson.ID).FirstOrDefault();
                if (concord == null)
                {
                    ShowBox("预览" + name, "当前股农没有合同!");
                    return;
                }
                var book = DbContext.CreateStockWarrantWorkStation().Get(o => o.ID == concord.ID).FirstOrDefault();
                if (Book as RegisterBookWord != null || Book as WarrantWord != null)
                {
                    if (book == null)
                    {
                        ShowBox("预览" + name, "当前股农没有权证!");
                        return;
                    }
                }
                Book.Tissue = DbContext.CreateSenderWorkStation().Get(CurrentZone.ID);
                Book.Contractor = personPanel.SelectedItem.VirtualPerson;
                Book.DbContext = DbContext;
                var sender = DbContext.CreateSenderWorkStation().Get(CurrentZone.ID);
                Book.Tissue = sender;
                Book.TemplateName = tempName;
                Book.Concord = concord;
                Book.Book = book;
                Book.ListLandDots = DbContext.CreateBoundaryAddressDotWorkStation().Get(o => o.ZoneCode == CurrentZone.FullCode);
                Book.Contractor = personPanel.SelectedItem.VirtualPerson;
                Book.DictList = DbContext.CreateDictWorkStation().Get();
                Book.ZoneList = DbContext.CreateZoneWorkStation().Get();
                Book.CurrentZone = CurrentZone;
                Book.OpenTemplate(TemplateHelper.WordTemplate(tempName));
                if (Book as WarrantWord == null)
                {
                    Book.PrintPreview(null);
                }
                else
                {
                    var warrant = Book as WarrantWord;
                    warrant.PrintContractLand(false);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "预览" + name, ex.Message + ex.StackTrace);
                return;
            }
        }




        #endregion


    }
}
