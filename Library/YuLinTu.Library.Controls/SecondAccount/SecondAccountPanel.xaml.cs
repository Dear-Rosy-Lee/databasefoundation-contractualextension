/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 二轮台账管理界面
    /// </summary>
    public partial class SecondAccountPanel : UserControl
    {
        #region Fields

        private string zoneName; //地域名称
        private TaskQueue queueQuery;//获取数据
        private TaskQueue queueFilter;//过滤数据
        private Zone currentZone;
        private bool allCheck = false;
        private eVirtualType virtualType;

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        /// <summary>
        /// 当前地域改变
        /// </summary>
        public delegate void PanelZoneChanged(Zone zone);

        /// <summary>
        /// 二轮承包方绑定集合
        /// </summary>
        private ObservableCollection<SecondVirtualPersonItem> secondItems = new ObservableCollection<SecondVirtualPersonItem>();

        /// <summary>
        /// 当前选择承包方绑定实体
        /// </summary>
        private SecondVirtualPersonItem currentSecondItem;

        /// <summary>
        /// 当前二轮地块
        /// </summary>
        private SecondTableLand currentSecondLand;

        /// <summary>
        /// 用于存储过滤后的二轮地块绑定集合
        /// </summary>
        private List<SecondLandBinding> landFilter = new List<SecondLandBinding>();

        /// <summary>
        /// 用于存储过滤后的二轮承包方绑定集合
        /// </summary>
        private ObservableCollection<SecondVirtualPersonItem> secondItemsFilter = new ObservableCollection<SecondVirtualPersonItem>();

        /// <summary>
        /// 当前选择地域下的所有承包方集合
        /// </summary>
        private List<VirtualPerson> currentListPerson = new List<VirtualPerson>();

        /// <summary>
        /// 当前地域下的所有地块集合
        /// </summary>
        private List<SecondTableLand> listLand = new List<SecondTableLand>();

        /// <summary>
        /// 用于标记主界面是否选中项
        /// </summary>
        private bool flag = false;

        #endregion

        #region Popertys

        /// <summary>
        /// 当前地域变化
        /// </summary>
        public PanelZoneChanged ZoneChanged { get; set; }

        /// <summary>
        /// 二轮台账统计信息
        /// </summary>
        public SecondAccountSummary SecondSummary { get; set; }

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage
        {
            get;
            set;
        }

        /// <summary>
        /// 承包方数据业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 二轮台账地块数据处理业务
        /// </summary>
        public SecondTableLandBusiness TableLandBusiness { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                if (currentZone != null)
                {
                    InlitialControl(currentZone.FullCode);
                    currentSecondItem = null;
                }
                if (ZoneChanged != null)
                {
                    ZoneChanged(currentZone);
                }
            }
        }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set
            {
                virtualType = value;
                PersonBusiness.VirtualType = value;
            }
        }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 二轮台账导出表设置
        /// </summary>
        public SecondTableExportDefine SecondTableExportSet
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<SecondTableExportDefine>();
                var section = profile.GetSection<SecondTableExportDefine>();
                var config = section.Settings as SecondTableExportDefine;
                return config;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SecondAccountPanel()
        {
            InitializeComponent();
            SecondSummary = new SecondAccountSummary();
            virtualType = eVirtualType.SecondTable;
            DataContext = this;
            treeViewLandInfo.ItemsSource = secondItems;  //treeViewLandInfo绑定数据源
            queueQuery = new TaskQueueDispatcher(Dispatcher);
            queueFilter = new TaskQueueDispatcher(Dispatcher);
        }

        #endregion

        #region Methods

        #region  Public

        #region 获取/绑定数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InlitialControl(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
            {
                return;
            }
            queueQuery.Cancel();
            queueQuery.DoWithInterruptCurrent(
                go =>
                {
                    DoWork(go);
                },
                completed =>
                {
                    DataCount();
                    lbzone.Text = zoneName;
                },
                terminated =>
                {
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                },
                start =>
                {
                    secondItems.Clear();
                }, null, null, null, null);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void DoWork(TaskGoEventArgs arg)
        {
            if (PersonBusiness == null)
            {
                return;
            }
            string zoneCode = currentZone.FullCode;
            currentListPerson = PersonBusiness.GetByZone(zoneCode);
            listLand = TableLandBusiness.GetCollection(zoneCode);
            if (currentListPerson != null && currentListPerson.Count > 0 && listLand != null)
            {
                foreach (var item in currentListPerson)
                {
                    if (arg.Instance.IsStopPending)
                        break;
                    SecondVirtualPersonItem svpi = SecondVirtualPersonItemHelper.ConvertToItem(item, listLand);
                    arg.Instance.ReportProgress(50, svpi);
                }
            }
            //获得地域的名称
            zoneName = PersonBusiness.GetUinitName(currentZone);
        }

        /// <summary>
        /// 获取数据完成
        /// </summary>
        private void Changed(TaskProgressChangedEventArgs arg)
        {
            SecondVirtualPersonItem item = arg.UserState as SecondVirtualPersonItem;
            if (item != null)
            {
                secondItems.Add(item);
            }
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        private void DataCount()
        {
            SecondSummary.SecondLandCount = 0;
            SecondSummary.VirtualPersonCount = 0;
            SecondSummary.TotalTableArea = 0;
            foreach (var item in secondItems)
            {
                SecondSummary.VirtualPersonCount += item.Visibility == Visibility.Visible ? 1 : 0;
                SecondSummary.TotalTableArea += (item.Visibility == Visibility.Visible) && (item.TableArea != null) ? (double)item.TableArea : 0;
                foreach (var land in item.Children)
                {
                    SecondSummary.SecondLandCount += land.Visibility == Visibility.Visible ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// 是否有子节点
        /// </summary>
        private void treeViewLandInfo_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
            var item = e.Object as SecondVirtualPersonItem;
            if (item == null)
            {
                return;
            }
            e.HasItems = item.Children.Count > 0;
        }

        #endregion

        #region 承包方基本操作

        /// <summary>
        /// 承包方管理
        /// </summary>
        public void ManageVirtualPerson()
        {

            SecondVirtualPersonPage personPage = new SecondVirtualPersonPage();
            if (CurrentZone == null)
            {
                return;
            }
            personPage.CurrentZone = CurrentZone;
            personPage.ThePage = ThePage;
            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
            });
        }

        /// <summary>
        /// 二轮台账承包方编辑
        /// </summary>
        public void EditVirtualPerson()
        {
            if (CurrentZone == null)
            {
                return;
            }
            if (currentSecondItem == null)
            {
                ShowBox(SecondAccountInfo.EditData, SecondAccountInfo.EditDataNo);
                return;
            }
            else if (currentSecondItem != null && currentSecondLand == null)
            {
                VirtualPerson selectedPerson = currentSecondItem.Tag as VirtualPerson;
                var secondPersonPage = new SecondAccountVirtualPersonInfo(selectedPerson, currentZone, PersonBusiness);
                secondPersonPage.SecondItems = secondItems;
                secondPersonPage.Workpage = ThePage;
                ThePage.Page.ShowMessageBox(secondPersonPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    VirtualPerson contractor = secondPersonPage.Contractor;
                    TheBns.Current.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_EDIT_COMPLATE, contractor));
                    ThePage.Workspace.Message.Send(this, MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_EDIT_COMPLATE, contractor));
                });
            }

        }

        /// <summary>
        /// 二轮台账承包方删除
        /// </summary>
        public void DeltVirtualPerson()
        {
            if (CurrentZone == null)
            {
                return;
            }
            if (currentSecondItem == null)
            {
                ShowBox(SecondAccountInfo.DelData, SecondAccountInfo.DelDataNo);
                return;
            }
            else if (currentSecondItem != null && currentSecondLand == null)
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    try
                    {
                        PersonBusiness.Delete(currentSecondItem.Tag.ID);
                        ModuleMsgArgs args = MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_DELT_COMPLETE, currentSecondItem.Tag);
                        ThePage.Workspace.Message.Send(this, args);
                        TheBns.Current.Message.Send(this, args);
                    }
                    catch (Exception ex)
                    {
                        ShowBox(SecondAccountInfo.DelVirtualPerson, SecondAccountInfo.DelVPersonFail);
                        YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
                    }
                });
                ShowBox(SecondAccountInfo.DelVirtualPerson, SecondAccountInfo.DelVPersonWarring, eMessageGrade.Warn, action);

                //TabMessageBoxDialog message = new TabMessageBoxDialog()
                //{
                //    Header = SecondAccountInfo.DelVirtualPerson,
                //    Message = SecondAccountInfo.DelVPersonWarring,
                //    MessageGrade = eMessageGrade.Warn
                //};
                //ThePage.Page.ShowMessageBox(message, (b, e) =>
                //{
                //    if (!(bool)b)
                //    {
                //        return;
                //    }
                //    try
                //    {
                //        PersonBusiness.Delete(currentSecondItem.Tag.ID);
                //        ModuleMsgArgs args = MessageExtend.VirtualPersonMsg(CreateDb(), SecondTableLandMessage.SECONDPERSON_DELT_COMPLETE, currentSecondItem.Tag);
                //        ThePage.Workspace.Message.Send(this, args);
                //        TheBns.Current.Message.Send(this, args);
                //    }
                //    catch (Exception ex)
                //    {
                //        ShowBox(SecondAccountInfo.DelVirtualPerson, SecondAccountInfo.DelVPersonFail);
                //        YuLinTu.Library.Log.Log.WriteException(this, "DelVirtualPerson(删除承包方)", ex.Message + ex.StackTrace);
                //    }
                //});
            }
        }

        #endregion

        #region 承包地基本操作

        /// <summary>
        /// 地块添加
        /// </summary>
        public void AddLand()
        {
            if (secondItems == null || secondItems.Count == 0)
            {
                ShowBox(SecondAccountInfo.SecondLandAdd, SecondAccountInfo.ZoneNoVirtualPerson);
                return;
            }
            ModuleMsgArgs arg = MessageExtend.SecondTableMsg(CreateDb(), SecondTableLandMessage.SECONDLAND_GET_DICTIONARY, "", "");
            TheBns.Current.Message.Send(this, arg);
            List<Dictionary> dicList = arg.ReturnValue as List<Dictionary>;
            SecondTableLand land = new SecondTableLand();
            SecondTableLandOperator.InitiallizeLand(land, dicList);
            var landInfoDlg = new SecondAccountLandInfo(true); ;
            landInfoDlg.CurrentZone = CurrentZone;
            landInfoDlg.CurrentItems = secondItems;
            landInfoDlg.Flag = flag;
            landInfoDlg.CurrentItem = currentSecondItem;
            landInfoDlg.Workpage = ThePage;
            landInfoDlg.CurrentSecondLand = land;
            landInfoDlg.CurrentSecondLand.LandName = landInfoDlg.cmbLandNameAlise.Text;
            ThePage.Page.ShowMessageBox(landInfoDlg, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    currentSecondLand = landInfoDlg.CurrentSecondLand;
                    if (!flag)
                    {
                        currentSecondItem = secondItems.FirstOrDefault(c => c.ID == landInfoDlg.CurrentItem.ID);
                        currentSecondItem.Children.Add(currentSecondLand.ConvertTo<SecondLandBinding>());
                    }
                    else
                    {
                        currentSecondItem.Children.Add(currentSecondLand.ConvertTo<SecondLandBinding>());
                    }
                    //数据统计
                    DataCount();
                    //刷新改变主界面上的地块数量
                    currentSecondItem.Name = SecondVirtualPersonItemHelper.CreateItemName(currentSecondItem.Tag, currentSecondItem.Children.Count);
                }));
            });
        }

        /// <summary>
        /// 地块编辑
        /// </summary>
        public void EditLand()
        {
            if (currentSecondLand == null)
            {
                ShowBox(SecondAccountInfo.SecondLandEdit, SecondAccountInfo.LandEditSelected);
                return;
            }
            var landEditDlg = new SecondAccountLandInfo();
            landEditDlg.CurrentZone = CurrentZone;
            landEditDlg.CurrentItems = secondItems;
            landEditDlg.CurrentItem = currentSecondItem;
            landEditDlg.Workpage = ThePage;
            var landTemp = currentSecondLand.Clone().ConvertTo<SecondTableLand>();
            landEditDlg.CurrentSecondLand = landTemp;
            ThePage.Page.ShowMessageBox(landEditDlg, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    if (currentSecondItem != landEditDlg.CurrentItem)
                    {
                        var entity = currentSecondItem.Children.FirstOrDefault(c => c.ID == landTemp.ID);
                        currentSecondItem.Children.Remove(entity);
                        currentSecondItem.Name = SecondVirtualPersonItemHelper.CreateItemName(currentSecondItem.Tag, currentSecondItem.Children.Count);
                        if (currentSecondItem.Children.Count == 0)
                            currentSecondItem.Visibility = Visibility.Collapsed;
                        landEditDlg.CurrentItem.Children.Add(landTemp.ConvertTo<SecondLandBinding>());
                        landEditDlg.CurrentItem.Name = SecondVirtualPersonItemHelper.CreateItemName(landEditDlg.CurrentItem.Tag, landEditDlg.CurrentItem.Children.Count);
                    }
                    currentSecondLand.CopyPropertiesFrom(landTemp);
                    DataCount();
                }));
            });
        }

        /// <summary>
        /// 地块删除
        /// </summary>
        public void DeltLand()
        {
            if (currentSecondLand == null)
            {
                ShowBox(SecondAccountInfo.SecondLandDel, SecondAccountInfo.LandDelSelected);
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    TableLandBusiness.DeleteLand(currentSecondLand);
                    var entity = currentSecondItem.Children.FirstOrDefault(t => t.ID == currentSecondLand.ID);
                    currentSecondItem.Children.Remove(entity);
                    //刷新改变主界面上的地块数量
                    currentSecondItem.Name = SecondVirtualPersonItemHelper.CreateItemName(currentSecondItem.Tag, currentSecondItem.Children.Count);
                    if (currentSecondItem.Children.Count == 0)
                        currentSecondItem.Visibility = Visibility.Visible;
                    //数据统计
                    DataCount();
                }));
            });
            ShowBox(SecondAccountInfo.SecondLandDel, SecondAccountInfo.LandDelConfirm, eMessageGrade.Infomation, action);

            //var landDelDlg = new TabMessageBoxDialog()
            //{
            //    Header = SecondAccountInfo.SecondLandDel,
            //    Message = SecondAccountInfo.LandDelConfirm,
            //    MessageGrade = eMessageGrade.Infomation,
            //};
            //ThePage.Page.ShowMessageBox(landDelDlg, (b, r) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        TableLandBusiness.DeleteLand(currentSecondLand);
            //        var entity = currentSecondItem.Children.FirstOrDefault(t => t.ID == currentSecondLand.ID);
            //        currentSecondItem.Children.Remove(entity);
            //        //刷新改变主界面上的地块数量
            //        currentSecondItem.Name = SecondVirtualPersonItemHelper.CreateItemName(currentSecondItem.Tag, currentSecondItem.Children.Count);
            //        if (currentSecondItem.Children.Count == 0)
            //            currentSecondItem.Visibility = Visibility.Visible;
            //        //数据统计
            //        DataCount();
            //    }));
            //});
        }

        #endregion

        #region 台账调查表数据导入

        /// <summary>
        /// 台账调查表数据导入
        /// </summary>
        public void ImportQueryTbl()
        {
            if (currentZone == null)
            {
                ShowBox(SecondAccountInfo.ImportData, SecondAccountInfo.ImportNoZone);
                return;
            }
            if ((currentZone.Level != eZoneLevel.Group) && (currentZone.Level != eZoneLevel.Village))
            {
                ShowBox(SecondAccountInfo.ImportData, SecondAccountInfo.ImportErrorZone);
                return;
            }
            else
            {
                Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((c, m) =>
                {
                    if (!(bool)c)
                    {
                        return;
                    }
                    ImportDataPage importLand = new ImportDataPage(ThePage, "导入台账调查表", "");
                    importLand.Workpage = ThePage;
                    ThePage.Page.ShowMessageBox(importLand, (b, r) =>
                    {
                        if (string.IsNullOrEmpty(importLand.FileName) || (bool)b == false)
                        {
                            return;
                        }
                        IDbContext dbContext = CreateDb();
                        TaskSecondTableArgument metaData = new TaskSecondTableArgument();
                        metaData.UserName = "Admin";
                        metaData.CurrentZone = CurrentZone;
                        metaData.FileName = importLand.FileName;
                        metaData.Database = CreateDb();
                        metaData.IsClear = true;
                        metaData.UserState = System.IO.Path.GetTempPath();
                        metaData.VirtualType = virtualType;
                        metaData.ArgType = eSecondTableArgType.ImportData;
                        TaskSecondTableOperation import = new TaskSecondTableOperation();
                        import.Argument = metaData;
                        import.Description = SecondAccountInfo.ImportDataComment;
                        import.Name = SecondAccountInfo.ImportData;
                        import.Completed += new TaskCompletedEventHandler((o, t) =>
                        {
                            Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                            TheBns.Current.Message.Send(this, MessageExtend.SecondTableMsg(dbContext, SecondTableLandMessage.SECONDLAND_IMPORT_COMPLETE, CurrentZone.FullCode, ""));
                        });
                        import.Terminated += new TaskTerminatedEventHandler((o, t) =>
                        {
                            ShowBox(SecondAccountInfo.ImportData, SecondAccountInfo.ImportDataFail);
                        });
                        ThePage.TaskCenter.Add(import);
                        if (ShowTaskViewer != null)
                        {
                            ShowTaskViewer();
                        }
                        import.StartAsync();
                    });
                });
                ShowBox(SecondAccountInfo.ImportData, SecondAccountInfo.ImportTableDataSure, eMessageGrade.Infomation, action);

                //ThePage.Page.ShowMessageBox(new TabMessageBoxDialog
                //{
                //    Header = SecondAccountInfo.ImportData,
                //    Message = SecondAccountInfo.ImportTableDataSure,
                //    MessageGrade = eMessageGrade.Infomation,
                //}, (c, m) =>
                //{
                //    if (!(bool)c)
                //    {
                //        return;
                //    }
                //    ImportDataPage importLand = new ImportDataPage(ThePage, "导入台账调查表", "");
                //    importLand.Workpage = ThePage;
                //    ThePage.Page.ShowMessageBox(importLand, (b, r) =>
                //    {
                //        if (string.IsNullOrEmpty(importLand.FileName) || (bool)b == false)
                //        {
                //            return;
                //        }
                //        IDbContext dbContext = CreateDb();
                //        TaskSecondTableArgument metaData = new TaskSecondTableArgument();
                //        metaData.UserName = "Admin";
                //        metaData.CurrentZone = CurrentZone;
                //        metaData.FileName = importLand.FileName;
                //        metaData.Database = CreateDb();
                //        metaData.IsClear = true;
                //        metaData.UserState = System.IO.Path.GetTempPath();
                //        metaData.VirtualType = virtualType;
                //        metaData.ArgType = eSecondTableArgType.ImportData;
                //        TaskSecondTableOperation import = new TaskSecondTableOperation();
                //        import.Argument = metaData;
                //        import.Description = SecondAccountInfo.ImportDataComment;
                //        import.Name = SecondAccountInfo.ImportData;
                //        import.Completed += new TaskCompletedEventHandler((o, t) =>
                //        {
                //            Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                //            TheBns.Current.Message.Send(this, MessageExtend.SecondTableMsg(dbContext, SecondTableLandMessage.SECONDLAND_IMPORT_COMPLETE, CurrentZone.FullCode, ""));
                //        });
                //        import.Terminated += new TaskTerminatedEventHandler((o, t) =>
                //        {
                //            ShowBox(SecondAccountInfo.ImportData, SecondAccountInfo.ImportDataFail);
                //        });
                //        ThePage.TaskCenter.Add(import);
                //        if (ShowTaskViewer != null)
                //        {
                //            ShowTaskViewer();
                //        }
                //        import.StartAsync();
                //    });
                //});
            }
        }

        #endregion

        #region 导出二轮台账

        /// <summary>
        /// 摸底调查表
        /// </summary>
        public void ExportRealQueryTbl()
        {
            if (CurrentZone == null)
            {
                ShowBox(VirtualPersonInfo.ExportData, VirtualPersonInfo.ExportNoZone);
                return;
            }
            ExportCommonOperate(eSecondTableArgType.RealQueryExcel, SecondAccountInfo.ExportRealQueryExcel, SecondAccountInfo.ExportRealQueryTable);
        }

        /// <summary>
        /// 摸底调查公示表
        /// </summary>
        public void ExportPublicityTbl()
        {
            if (CurrentZone == null)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoZone);
                return;
            }
            ExportCommonOperate(eSecondTableArgType.PublicityExcel, SecondAccountInfo.ExportPublicityExcel, SecondAccountInfo.ExportPublicityTable);
        }

        ///<summary>
        /// 摸底调查公示确认表
        ///</summary>
        public void ExportIdentifyTbl()
        {
            if (CurrentZone == null)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoZone);
                return;
            }
            ExportCommonOperate(eSecondTableArgType.IdentifyExcel, SecondAccountInfo.ExportIdentifyExcel, SecondAccountInfo.ExportIdentifyTable);
        }

        /// <summary>
        /// 用户确认表
        /// </summary>
        public void ExportUserIdentifyTbl()
        {
            if (CurrentZone == null)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoZone);
                return;
            }
            ExportCommonOperate(currentZone.FullName, SecondAccountInfo.ExportUserIdentifyTable,
                eSecondTableArgType.UserIdentify, SecondAccountInfo.ExportUserIdentifyExcel, SecondAccountInfo.ExportUserIdentifyTable);
        }

        /// <summary>
        /// 导出户籍表
        /// </summary>
        public void ExportVirtualPersonWord()
        {

        }

        #endregion

        #region  导出勘界确权数据

        /// <summary>
        /// 单户调查表
        /// </summary>
        public void ExportUserQueryTbl()
        {
            if (currentZone == null)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoZone);
                return;
            }
            if (secondItems == null || secondItems.Count == 0)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoData);
                return;
            }
            ExportDataPage export = new ExportDataPage(currentZone.FullName, ThePage, "批量导出单户调查表", "", "");
            export.Workpage = ThePage;
            ThePage.Page.ShowMessageBox(export, (b, r) =>
            {
                if (!(bool)b || string.IsNullOrEmpty(export.FileName))
                {
                    return;
                }
                ExportCommonOperate(eSecondTableArgType.ExportSingleFamilyExcel, SecondAccountInfo.ExportSingleFamilyExcel, SecondAccountInfo.ExportSingleFamilyTable, export.FileName);
            });
        }

        /// <summary>
        /// 勘界调查表
        /// </summary>
        public void ExportHumphreyQueryTbl()
        {
            if (currentZone == null)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoZone);
                return;
            }
            if (secondItems == null || secondItems.Count == 0)
            {
                ShowBox(SecondAccountInfo.ExportData, SecondAccountInfo.ExportNoData);
                return;
            }
            ExportCommonOperate(eSecondTableArgType.ExportBoundarySettleExcel, SecondAccountInfo.ExportBoundarySettleExcel, SecondAccountInfo.ExportBoundaryTable);
        }

        #endregion

        #region  清空数据

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            if (currentZone == null || secondItems.Count == 0)
            {
                return;
            }
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                TableLandBusiness.DeleteLandByZoneCode(currentZone.FullCode); //删除二轮地块数据库中的数据
                PersonBusiness.ClearZoneData(currentZone.FullCode);  //删除二轮承包方数据库中的数据
                secondItems.Clear();
                DataCount();
                //清空当期地域下的所有地块数据时发出该消息
                TheBns.Current.Message.Send(this, MessageExtend.SecondTableMsg(CreateDb(), SecondTableLandMessage.SECONDLAND_CLEAR_COMPLETE, "", ""));
            });
            ShowBox(SecondAccountInfo.Clear, SecondAccountInfo.ClearConfirm, eMessageGrade.Warn, action);

            //TabMessageBoxDialog clearDlg = new TabMessageBoxDialog()
            //{
            //    Header = SecondAccountInfo.Clear,
            //    Message = SecondAccountInfo.ClearConfirm,
            //    MessageGrade = eMessageGrade.Warn,
            //};
            //ThePage.Page.ShowMessageBox(clearDlg, (b, r) =>
            //{
            //    if (!(bool)b)
            //    {
            //        return;
            //    }
            //    TableLandBusiness.DeleteLandByZoneCode(currentZone.FullCode); //删除二轮地块数据库中的数据
            //    PersonBusiness.ClearZoneData(currentZone.FullCode);  //删除二轮承包方数据库中的数据
            //    secondItems.Clear();
            //    DataCount();
            //    //清空当期地域下的所有地块数据时发出该消息
            //    TheBns.Current.Message.Send(this, MessageExtend.SecondTableMsg(CreateDb(), SecondTableLandMessage.SECONDLAND_CLEAR_COMPLETE, "", ""));
            //});
        }

        #endregion

        #region 数据过滤

        /// <summary>
        /// 快速过滤
        /// </summar
        private void txtWhere_TextChanged(object sender, TextChangedEventArgs e)
        {
            string whName = txtWhere.Text;
            if (string.IsNullOrEmpty(whName))
            {
                foreach (var item in secondItems)
                {
                    item.Visibility = Visibility.Visible;
                    foreach (var land in item.Children)
                    {
                        land.Visibility = Visibility.Visible;
                    }
                }
                DataCount();
            }
            else
            {
                DataFilter(whName);
            }
        }

        /// <summary>
        /// 数据过滤
        /// </summary>
        private void DataFilter(string whName)
        {
            queueFilter.Cancel();
            queueFilter.DoWithInterruptCurrent(
                go =>
                {
                    SetItemVisible(go);
                },
                completed =>
                {
                    DataCount();
                },
                terminated =>
                {
                },
                progressChanged =>
                {
                    Changed(progressChanged);
                }, null, null, null, null, whName);
        }

        /// <summary>
        /// 设置数据可见性
        /// </summary>
        private void SetItemVisible(TaskGoEventArgs arg)
        {
            string whString = arg.Instance.Argument.UserState.ToString();
            foreach (var item in secondItems)
            {
                bool isContains = JudgeItem(whString, allCheck, item);
                if (isContains)
                {
                    continue;
                }
                bool allHidden = true;
                foreach (var p in item.Children)
                {
                    p.Visibility = Visibility.Collapsed;
                    string name = string.IsNullOrEmpty(p.Name) ? "" : p.Name;
                    string landName = string.IsNullOrEmpty(p.LandName) ? "" : p.LandName;
                    string neighborNorth = string.IsNullOrEmpty(p.NeighborNorth) ? "" : p.NeighborNorth;
                    string neighborEast = string.IsNullOrEmpty(p.NeighborEast) ? "" : p.NeighborEast;
                    string neighborSouth = string.IsNullOrEmpty(p.NeighborSouth) ? "" : p.NeighborSouth;
                    string neighborWest = string.IsNullOrEmpty(p.NeighborWest) ? "" : p.NeighborWest;
                    string area = p.TableArea == null ? "" : p.TableArea.ToString();
                    string comment = string.IsNullOrEmpty(p.Comment) ? "" : p.Comment;
                    bool condition1 = (allCheck) && (name.Equals(whString) || landName.Equals(whString)
                        || area.Equals(whString) || neighborNorth.Equals(whString) || neighborEast.Equals(whString)
                        || neighborSouth.Equals(whString) || neighborWest.Equals(whString) || comment.Equals(whString));
                    bool condition2 = (!allCheck) && (name.Contains(whString) || landName.Contains(whString)
                         || area.Contains(whString) || neighborNorth.Contains(whString) || neighborEast.Contains(whString)
                        || neighborSouth.Contains(whString) || neighborWest.Contains(whString) || comment.Contains(whString));
                    if (condition1 || condition2)
                    {
                        allHidden = false;
                        p.Visibility = Visibility.Visible;
                    }
                }
                if (allHidden)
                {
                    item.Visibility = Visibility.Collapsed;
                }
                else
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 项的过滤:如果项符合条件,则全显示(true),否则不显示(false)
        /// </summary>
        private bool JudgeItem(string whString, bool allInfo, SecondVirtualPersonItem item)
        {
            string name = string.IsNullOrEmpty(item.Tag.Name) ? "" : item.Tag.Name;
            string area = item.TableArea == null ? "" : item.TableArea.ToString();
            bool condition1 = (allCheck) && name.Equals(whString);
            bool condition2 = allCheck && area.Equals(whString);
            bool condition3 = (!allCheck) && name.Contains(whString);
            bool condition4 = !allCheck && area.Contains(whString);
            if (condition1 || condition2 || condition3 || condition4)
            {
                item.Visibility = Visibility.Visible;
                foreach (var land in item.Children)
                {
                    land.Visibility = Visibility.Visible;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 全匹配
        /// </summary>
        private void cb_ComplateInfo_Click(object sender, RoutedEventArgs e)
        {
            string whName = txtWhere.Text.Trim();
            if (cb_ComplateInfo.IsChecked == false)
            {
                allCheck = false;
            }
            else
            {
                allCheck = true;
            }
            if (string.IsNullOrEmpty(whName))
            {
                foreach (var item in secondItems)
                {
                    item.Visibility = Visibility.Visible;
                    foreach (var land in item.Children)
                    {
                        land.Visibility = Visibility.Visible;
                    }
                }
                DataCount();
            }
            else
            {
                DataFilter(whName);
            }
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="header">弹出框</param>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(string zoneName, string header, eSecondTableArgType type, string taskDes, string taskName, string messageName = "")
        {
            ExportDataPage extPage = new ExportDataPage(zoneName, ThePage, header);
            extPage.Workpage = ThePage;
            ThePage.Page.ShowMessageBox(extPage, (b, r) =>
            {
                if (string.IsNullOrEmpty(extPage.FileName) || b == false)
                {
                    return;
                }
                ExportCommonOperate(type, taskDes, taskName, extPage.FileName, messageName);
            });
        }

        /// <summary>
        /// 导出文件操作
        /// </summary>
        /// <param name="type">导出类型</param>
        /// <param name="taskDes">任务描述</param>
        /// <param name="taskName">任务名称</param>
        /// <param name="messageName">消息名称</param>
        private void ExportCommonOperate(eSecondTableArgType type, string taskDes, string taskName, string filePath = "", string messageName = "")
        {
            IDbContext dbContext = CreateDb();
            TaskSecondTableArgument meta = new TaskSecondTableArgument();
            meta.IsClear = false;
            meta.FileName = filePath;
            meta.ArgType = type;
            meta.Database = dbContext;
            meta.CurrentZone = currentZone;
            meta.virtualType = virtualType;
            TaskSecondTableOperation import = new TaskSecondTableOperation();
            import.SecondTableDefine = SecondTableExportSet;
            import.Argument = meta;
            import.Description = taskDes;
            import.Name = taskName;
            import.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                if (string.IsNullOrEmpty(messageName))
                {
                    return;
                }
                TheBns.Current.Message.Send(this, MessageExtend.SenderMsg(dbContext, messageName, true));
            });
            ThePage.TaskCenter.Add(import);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            import.StartAsync();
        }

        #endregion

        #region Methods - Events

        #region 按键功能

        /// <summary>
        /// 刷新按钮事件
        /// </summary>
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            txtWhere.Text = string.Empty;
            Refresh();
        }

        /// <summary>
        /// 选择项改变  二轮台账数据显示主界面
        /// </summary>
        private void treeViewLandInfo_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GetSelectItem();
        }

        /// <summary>
        /// 鼠标双击  双击显示的数据项
        /// </summary>
        private void treeViewLandInfo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetSelectItem();
            if (currentSecondLand == null)
            {
                EditVirtualPerson();
            }
            else
            {
                EditLand();
            }
        }

        /// <summary>
        /// 右键菜单弹出时
        /// </summary>
        private void treeViewLandInfo_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            GetSelectItem();
            SetButtonEnable();
        }

        #endregion

        #region 右键菜单

        /// <summary>
        /// 编辑承包方
        /// </summary>
        private void miEditPerson_Click(object sender, RoutedEventArgs e)
        {
            if (miEditPerson.IsEnabled)
            {
                EditVirtualPerson();
            }
        }

        /// <summary>
        /// 删除承包方
        /// </summary>
        private void miDeltPerson_Click(object sender, RoutedEventArgs e)
        {
            if (miDeltPerson.IsEnabled)
            {
                DeltVirtualPerson();
            }
        }

        /// <summary>
        /// 添加承包地
        /// </summary>
        private void miAddLand_Click(object sender, RoutedEventArgs e)
        {
            if (miAddLand.IsEnabled)
            {
                AddLand();
            }
        }

        /// <summary>
        /// 编辑承包地
        /// </summary>
        private void miAEditLand_Click(object sender, RoutedEventArgs e)
        {
            if (miAEditLand.IsEnabled)
            {
                EditLand();
            }
        }

        /// <summary>
        /// 删除承包地
        /// </summary>
        private void miDeltLand_Click(object sender, RoutedEventArgs e)
        {
            if (miDeltLand.IsEnabled)
            {
                DeltLand();
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        private void miClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 二轮台账导出表设置 
        /// </summary>
        private void miTableExport_Click(object sender, RoutedEventArgs e)
        {
            CommonConfigPage personPage = new CommonConfigPage(ThePage);
            personPage.Header = "二轮台账导出表设置";
            var profile = ThePage.Workspace.GetUserProfile();
            var section = profile.GetSection<SecondTableExportDefine>();
            var config = (section.Settings as SecondTableExportDefine);
            var configCopy = config.Clone();
            personPage.ProGrid.Object = configCopy;
            ThePage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                else
                {
                    config.CopyPropertiesFrom(configCopy);
                }
            });
        }


        #endregion

        #region 辅助功能

        /// <summary>
        /// 设置导出表日期
        /// </summary>
        /// <returns></returns>
        private DateTime? SetTableDatetime(int type)
        {
            return null;
        }

        /// <summary>
        /// 获取当前选择项
        /// </summary>
        private void GetSelectItem()
        {
            currentSecondItem = null;
            currentSecondLand = null;
            var item = treeViewLandInfo.SelectedItem;
            if (item == null)
            {
                flag = false;
            }
            else if (item is SecondTableLand)
            {
                currentSecondLand = treeViewLandInfo.SelectedItem as SecondTableLand;
                currentSecondItem = secondItems.Where(c => c.ID == currentSecondLand.OwnerId).FirstOrDefault();
                flag = true;
            }
            else if (item is SecondVirtualPersonItem)
            {
                currentSecondItem = treeViewLandInfo.SelectedItem as SecondVirtualPersonItem;
                flag = true;
            }
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            ThePage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 设置按钮可用性
        /// </summary>
        private void SetButtonEnable()
        {
            if (currentSecondLand == null && currentSecondItem != null)
            {
                miAddLand.IsEnabled = true;
                miAEditLand.IsEnabled = false;
                miDeltLand.IsEnabled = false;
                miEditPerson.IsEnabled = true;
                miDeltPerson.IsEnabled = true;
            }
            else if (currentSecondLand != null && currentSecondItem != null)
            {
                miAddLand.IsEnabled = true;
                miAEditLand.IsEnabled = true;
                miDeltLand.IsEnabled = true;
                miEditPerson.IsEnabled = false;
                miDeltPerson.IsEnabled = false;
            }
            else if (currentSecondLand == null && currentSecondItem == null)
            {
                miAddLand.IsEnabled = false;
                miAEditLand.IsEnabled = false;
                miDeltLand.IsEnabled = false;
                miEditPerson.IsEnabled = false;
                miDeltPerson.IsEnabled = false;
            }
        }

        #endregion

        #region 统计数据

        /// <summary>
        /// 刷新统计及界面
        /// </summary>
        public void Refresh()
        {
            currentSecondItem = null;
            if (currentZone == null)
            {
                return;
            }
            InlitialControl(currentZone.FullCode);
        }

        #endregion

        #endregion

        #endregion
    }
}

