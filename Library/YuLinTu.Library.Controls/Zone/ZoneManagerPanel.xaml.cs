/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;
using YuLinTu.Data;
using YuLinTu.Library.Log;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Repository;
using YuLinTu.Spatial;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 行政地域管理界面
    /// </summary>
    public partial class ZoneManagerPanel : UserControl
    {
        #region Fields

        /// <summary>
        /// 地域业务
        /// </summary>
        private ZoneDataBusiness business;

        /// <summary>
        /// 地域集合
        /// </summary>
        private List<Zone> zoneList;

        /// <summary>
        /// 当前地域
        /// </summary>
        private ZoneDataItem selectItem;

        /// <summary>
        /// 定义后台线程
        /// </summary>
        private BackgroundWorker worker;

        /// <summary>
        /// 绑定集合
        /// </summary>
        private ObservableCollection<ZoneDataItem> bindList = new ObservableCollection<ZoneDataItem>();

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        #endregion

        #region Properties

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 行政地域集合
        /// </summary>
        public List<Zone> ZoneList
        {
            get { return zoneList; }
            set
            {
                zoneList = value;
                if (zoneList != null && zoneList.Count > 0)
                {
                    InitializeControl(zoneList);
                }
            }
        }

        /// <summary>
        /// 当前选择地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 根级地域编码
        /// </summary>
        public string RootZoneCode { get; set; }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenuEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenuEnableControl MenuEnable { get; set; }

        /// <summary>
        /// 行政地域常规设置
        /// </summary>
        public ZoneDefine ZoneDefine
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<ZoneDefine>();
                var section = profile.GetSection<ZoneDefine>();
                var config = section.Settings as ZoneDefine;
                return config;
            }
        }

        #endregion

        #region Ctor

        public ZoneManagerPanel()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            LanguageAttribute.AddLanguage(YuLinTu.Library.Entity.Properties.Resources.langChs_eZoneLevel);
            LanguageAttribute.AddLanguage(YuLinTu.Library.Entity.Properties.Resources.langChs_Zone);
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        /// <summary>
        /// 设置控件数据地域编码
        /// </summary>
        public void SetControlData(string zoneCode = "", IWorkpage workPage = null)
        {
            if (!string.IsNullOrEmpty(zoneCode))
            {
                RootZoneCode = zoneCode;
            }
            if (workPage != null)
            {
                this.ThePage = workPage;
            }
            worker.RunWorkerAsync(RootZoneCode);
        }

        /// <summary>
        /// 任务完成
        /// </summary> 
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MenuEnable();
                view.ItemsSource = null;
                view.Items.Clear();
                List<Zone> zoneList = e.Result as List<Zone>;
                if (zoneList != null && zoneList.Count > 0)
                {
                    InitializeControl(zoneList);
                }
                else
                {
                    view.Items.Add(new
                    {
                        Name = "未能显示数据,请检查数据源连接及数据库数据!",
                        Img = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Identify16.png"))
                    });
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(ex.Source, "行政地域获取", ex.StackTrace);
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MenuEnable(false);
            string zoneCode = e.Argument.ToString();
            business = new ZoneDataBusiness(CreateDb());
            if (!string.IsNullOrEmpty(zoneCode) && zoneCode != "86")
            {
                e.Result = business.ZonesByCode(zoneCode);
            }
            else
            {
                e.Result = business.GetAllZone();
            }
        }

        #endregion

        #region Methods

        #region Method-public

        /// <summary>
        /// 添加地域
        /// </summary>
        public void Add()
        {
            if (!CanContinue(ZoneInfo.ZoneAdd, ZoneInfo.AddNoUp))
            {
                return;
            }
            Zone zone = selectItem as Zone;
            if (zone.Level == eZoneLevel.Group)
            {
                ShowBox(ZoneInfo.ZoneAdd, ZoneInfo.AddGroup);
                return;
            }
            IDbContext db = CreateDb();
            ZoneEditPage editPage = new ZoneEditPage(true);
            editPage.Workpage = ThePage;
            editPage.Station = db.CreateZoneWorkStation();
            var count = selectItem.Children.Count;
            while (selectItem.Children.Any(t => t.Code == count.ToString().PadLeft(t.Code.Length, '0')))
            {
                count++;
            }
            var cZone = ZoneDataItemHelper.CreateTempZone(zone.Level, zone.FullCode, zone.FullName);
            if (count <= 1)
                count = 1;
            cZone.Code = count.ToString().PadLeft(GetLevelLength(cZone.Level), '0');
            editPage.CurrentZone = cZone;
            ThePage.Page.ShowDialog(editPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (editPage.Result)
                {
                    selectItem.Children.Add(ZoneDataItemHelper.ConvertToDataItem(editPage.CurrentZone));
                    var sItem = view.FindTreeListViewItem(selectItem);
                    if (sItem != null)
                        sItem.IsExpanded = true;
                }
                else
                {
                    ShowBox(ZoneInfo.ZoneAdd, ZoneInfo.AddDataFaile);
                }
                ModuleMsgArgs arg = MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_ADD_COMPLETE, (editPage.Result ? editPage.CurrentZone : null));
                TheBns.Current.Message.Send(this, arg);
                ThePage.Workspace.Message.Send(this, arg);
            });
        }

        /// <summary>
        /// 编辑地域
        /// </summary>
        public void Edit()
        {
            if (!CanContinue(ZoneInfo.ZoneEdit, ZoneInfo.EditNull))
            {
                return;
            }
            ZoneDataItem srcItem = selectItem.Clone() as ZoneDataItem;
            var cZone = selectItem.Clone() as Zone;
            if (cZone.Level < eZoneLevel.Province && string.IsNullOrEmpty(cZone.UpLevelName))
            {
                var list = view.ItemsSource as ObservableCollection<ZoneDataItem>;
                SearchUpZone(list == null ? null : list[0], cZone);
            }
            IDbContext db = CreateDb();
            ZoneEditPage editPage = new ZoneEditPage();
            editPage.Workpage = ThePage;
            editPage.Station = db.CreateZoneWorkStation();
            editPage.CurrentZone = cZone;
            ThePage.Page.ShowDialog(editPage, (b, a) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (editPage.Result)
                {
                    UpDataToItem(selectItem, editPage.CurrentItem);
                    UpdataChildrenItem(selectItem.Children, editPage.CurrentItem.Children);
                }
                ModuleMsgArgs arg = MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_UPDATE_COMPLETE, (editPage.Result ? new MultiObjectArg() { ParameterA = selectItem, ParameterB = srcItem } : null));
                ThePage.Workspace.Message.Send(this, arg);
                TheApp.Current.Message.Send(ThePage, arg);
            });
        }

        private void SearchUpZone(ZoneDataItem item, Zone zone)
        {
            if (item == null || item.Level <= zone.Level)
            {
                return;
            }
            if (item.FullCode == zone.UpLevelCode)
            {
                zone.UpLevelName = item.FullName;
                return;
            }
            foreach (var c in item.Children)
            {
                SearchUpZone(c, zone);
            }
        }

        /// <summary>
        /// 更新数据项
        /// </summary>
        private void UpdataChildrenItem(ObservableCollection<ZoneDataItem> selectItem, ObservableCollection<ZoneDataItem> curItem)
        {
            if (selectItem.Count == 0 || curItem.Count == 0)
            {
                return;
            }
            foreach (var item in selectItem)
            {
                ZoneDataItem di = curItem.Where(t => t.ID == item.ID).FirstOrDefault();
                if (di != null)
                {
                    UpDataToItem(item, di);
                    UpdataChildrenItem(item.Children, di.Children);
                }
            }
        }

        /// <summary>
        /// 删除地域
        /// </summary>
        public void Del()
        {
            if (!CanContinue(ZoneInfo.ZoneDel, ZoneInfo.DelNull))
            {
                return;
            }
            if (selectItem.Level == eZoneLevel.State)
            {
                ShowBox(ZoneInfo.ZoneDel, ZoneInfo.DelTop);
                return;
            }
            if (selectItem.Children != null && selectItem.Children.Count > 0)
            {
                ShowBox(ZoneInfo.ZoneDel, ZoneInfo.DelContainsChildren);
                return;
            }
            business = new ZoneDataBusiness(CreateDb());
            if (business.HasBusinessInZone(selectItem.FullCode))
            {
                ShowBox(ZoneInfo.ZoneDel, ZoneInfo.DelContainsRelation);
                return;
            }
            string content = selectItem.Children.Count > 0 ? "确定删除选择地域及其子地域?" : "确定删除选择地域?";
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                Zone currentZone = selectItem as Zone;
                bool result = business.DeleteZone(currentZone.FullCode);
                if (result)
                {
                    RemoveFromParent(bindList, currentZone.ID);
                }
                else
                {
                    ShowBox(ZoneInfo.ZoneDel, ZoneInfo.DelDataFaile);
                }
                ModuleMsgArgs arg = MessageExtend.ZoneMsg(CreateDb(), ZoneMessage.ZONE_DELETE_COMPLETE, (result ? currentZone : null));
                TheBns.Current.Message.Send(this, arg);
                ThePage.Workspace.Message.Send(this, arg);
            });
            ShowBox(ZoneInfo.ZoneDel, content, eMessageGrade.Warn, action);

            //TabMessageBoxDialog messagePage = new TabMessageBoxDialog()
            //{
            //    Header = ZoneInfo.ZoneDel,
            //    Message = content,
            //    MessageGrade = eMessageGrade.Warn
            //};
            //ThePage.Page.ShowDialog(messagePage, (b, r) =>
            //{
            //    if (b == false)
            //    {
            //        return;
            //    }
            //    Zone currentZone = selectItem as Zone;
            //    bool result = business.DeleteZone(currentZone.FullCode);
            //    if (result)
            //    {
            //        RemoveFromParent(bindList, currentZone.ID);
            //    }
            //    else
            //    {
            //        ShowBox(ZoneInfo.ZoneDel, ZoneInfo.DelDataFaile);
            //    }
            //    ModuleMsgArgs arg = MessageExtend.ZoneMsg(CreateDb(), ZoneMessage.ZONE_DELETE_COMPLETE, (result ? currentZone : null));
            //    TheBns.Current.Message.Send(this, arg);
            //    ThePage.Workspace.Message.Send(this, arg);
            //});
        }

        /// <summary>
        /// 导入地域数据
        /// </summary>
        public void ImportData()
        {
            ImportDataPage importZone = new ImportDataPage(ThePage, "导入行政地域");
            ThePage.Page.ShowDialog(importZone, (b, r) =>
            {
                if (string.IsNullOrEmpty(importZone.FileName) || b == false)
                {
                    return;
                }
                IDbContext db = CreateDb();
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.FileName = importZone.FileName;
                meta.IsClear = false;
                meta.ArgType = eZoneArgType.ImportData;
                meta.Database = db;
                meta.Define = GetSystemSetting();
                TaskZoneOperation import = new TaskZoneOperation();
                import.Argument = meta;
                import.Description = ZoneInfo.ImportDataComment;
                import.Name = ZoneInfo.ImportData;
                import.Completed += new TaskCompletedEventHandler((o, t) =>
                {
                    Dispatcher.Invoke(new Action(() => { Refresh(); }), null);
                    ModuleMsgArgs arg = MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_IMPORTTABLE_COMPLETE, import.ReturnValue);
                    TheBns.Current.Message.Send(this, arg);
                    ThePage.Workspace.Message.Send(this, arg);
                });
                ThePage.TaskCenter.Add(import);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                import.StartAsync();
            });
        }

        /// <summary>
        /// 导入地域图斑
        /// </summary>
        public void ImportShape()
        {
            IDbContext db = CreateDb();
            ImportZoneShip zoneShip = new ImportZoneShip();
            zoneShip.Workpage = ThePage;
            zoneShip.Db = db;
            ThePage.Page.ShowDialog(zoneShip, (b, r) =>
            {
                if (string.IsNullOrEmpty(zoneShip.FileName) || b == false)
                {
                    return;
                }
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.FileName = zoneShip.FileName;
                meta.IsClear = false;
                meta.ArgType = eZoneArgType.ImportShape;
                meta.Database = db;
                meta.Define = GetSystemSetting();
                TaskZoneOperation import = new TaskZoneOperation();
                import.Argument = meta;
                import.Description = ZoneInfo.ImportShapeComment;
                import.Name = ZoneInfo.ImportShape;
                import.Completed += new TaskCompletedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_IMPORTSHAPE_COMPLETE, true));
                });
                import.Terminated += new TaskTerminatedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_IMPORTSHAPE_COMPLETE, false));
                });
                ThePage.TaskCenter.Add(import);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                import.StartAsync();
            });
        }

        /// <summary>
        /// 导出地域图斑
        /// </summary>
        public void ExportShape()
        {
            if (!CanContinue(ZoneInfo.ExportShape, ZoneInfo.ExportShapeNoUp))
            {
                return;
            }
            IDbContext db = CreateDb();
            ExportDataPage messagePage = new ExportDataPage(CurrentZone.FullName, ThePage, "导出图斑");
            ThePage.Page.ShowDialog(messagePage, (b, r) =>
            {
                if (b == false || string.IsNullOrEmpty(messagePage.FileName))
                {
                    return;
                }
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.Database = db;
                meta.CurrentZone = CurrentZone;
                meta.FileName = messagePage.FileName;
                meta.FilePath = messagePage.FileName;
                meta.ArgType = eZoneArgType.ExportShape;
                meta.Define = GetSystemSetting();
                TaskZoneOperation zoneToShape = new TaskZoneOperation();
                zoneToShape.Argument = meta;
                zoneToShape.Name = ZoneInfo.ExportShape;
                zoneToShape.Description = string.Format(ZoneInfo.ExportShapeComment, CurrentZone.Name);
                zoneToShape.Completed += new TaskCompletedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_EXPORTSHAPE_COMPLETE, true));
                });
                zoneToShape.Terminated += new TaskTerminatedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_EXPORTSHAPE_COMPLETE, false));
                });
                ThePage.TaskCenter.Add(zoneToShape);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                zoneToShape.StartAsync();
            });
        }

        /// <summary>
        /// 导出地域数据
        /// </summary>
        public void ExportData()
        {
            if (!CanContinue(ZoneInfo.ExportData, ZoneInfo.ExportDataNoUp))
            {
                return;
            }
            ExportDataPage messagePage = new ExportDataPage(CurrentZone.FullName, ThePage, ZoneInfo.ExportData);
            ThePage.Page.ShowDialog(messagePage, (b, r) =>
            {
                if (b == false || string.IsNullOrEmpty(messagePage.FileName))
                {
                    return;
                }
                IDbContext db = CreateDb();
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.Database = db;
                meta.FileName = messagePage.FileName;
                meta.FilePath = messagePage.FileName;
                meta.Define = GetSystemSetting();
                meta.CurrentZone = CurrentZone;
                meta.ArgType = eZoneArgType.ExportData;
                TaskZoneOperation zoneToExcel = new TaskZoneOperation();
                zoneToExcel.Argument = meta;
                zoneToExcel.Name = ZoneInfo.ExportData;
                zoneToExcel.Description = string.Format(ZoneInfo.ExportDataComment, CurrentZone.Name);
                zoneToExcel.Completed += new TaskCompletedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_EXPORTTABLE_COMPLETE, true));
                });
                zoneToExcel.Terminated += new TaskTerminatedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(db, ZoneMessage.ZONE_EXPORTTABLE_COMPLETE, false));
                });
                ThePage.TaskCenter.Add(zoneToExcel);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                zoneToExcel.StartAsync();
            });
        }

        /// <summary>
        /// 导出地域压缩包
        /// </summary>
        public void ExportPackage()
        {
            if (!CanContinue(ZoneInfo.ExportPackage, ZoneInfo.ExportPackageNoUp))
            {
                return;
            }
            IDbContext dbContext = CreateDb();
            ExportDataPage messagePage = new ExportDataPage(CurrentZone.FullName, ThePage, "导出压缩包");
            ThePage.Page.ShowDialog(messagePage, (b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.Database = dbContext;
                meta.CurrentZone = CurrentZone;
                meta.FileName = messagePage.FileName + "\\" + CurrentZone.FullCode + ".xml";
                meta.FilePath = messagePage.FileName;
                meta.ArgType = eZoneArgType.ExportPackage;
                meta.Define = GetSystemSetting();
                TaskZoneOperation zoneToPackage = new TaskZoneOperation();
                zoneToPackage.Argument = meta;
                zoneToPackage.Name = ZoneInfo.ExportPackage;
                zoneToPackage.Description = string.Format(ZoneInfo.ExportPackageComment, CurrentZone.Name);
                zoneToPackage.Completed += new TaskCompletedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(dbContext, ZoneMessage.ZONE_EXPORTPACKAGE_COMPLETE, true));
                });
                zoneToPackage.Terminated += new TaskTerminatedEventHandler((o, e) =>
                {
                    ThePage.Workspace.Message.Send(this, MessageExtend.ZoneMsg(dbContext, ZoneMessage.ZONE_EXPORTPACKAGE_COMPLETE, false));
                });
                ThePage.TaskCenter.Add(zoneToPackage);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                zoneToPackage.StartAsync();
            });
        }

        /// <summary>
        /// 清空地域
        /// </summary>
        public void Clear()
        {
            IDbContext dbContext = CreateDb();
            business = new ZoneDataBusiness(dbContext);
            if (business.HasBusiness() && !ZoneDefine.ClearData)
            {
                ShowBox(ZoneInfo.Clear, ZoneInfo.DelContainsRelation);
                return;
            }

            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (!(bool)b)
                    return;
                bool result = true;
                try
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    int clearResult = 0;
                    int addResult = 1;
                    if (ZoneDefine.ClearData)
                    {
                        //仅清空图斑数据
                        var allZones = zoneStation.Get();
                        if (!allZones.Any(c => c.Shape != null))
                        {
                            ShowBox(ZoneInfo.Clear, "未获取地域图斑数据,无法执行清空操作!");
                            return;
                        }
                        clearResult = zoneStation.ClearZoneShape();
                    }
                    else
                    {
                        //图斑和属性数据一并清空
                        clearResult = zoneStation.Delete();
                        Zone china = ZoneHelper.China;
                        china.ID = Guid.NewGuid();
                        addResult = zoneStation.Add(china);

                        var senderstation = dbContext.CreateSenderWorkStation();
                        senderstation.Clear();

                    }
                    result = (clearResult > 0 && addResult > 0) ? true : false;
                }
                catch (NullReferenceException nx)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", nx.Message + nx.StackTrace);
                    ShowBox(ZoneInfo.Clear, "数据库可能正在使用,无法完成清空操作！");
                    result = false;
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", ex.Message + ex.StackTrace);
                    result = false;
                }
                if (result)
                {
                    if (ZoneDefine.ClearData)
                    {
                        ShowBox(ZoneInfo.Clear, ZoneInfo.ClearShapeSuccess, eMessageGrade.Infomation);
                    }
                    else
                    {
                        Refresh();
                    }
                }
                else
                {
                    ShowBox(ZoneInfo.Clear, ZoneInfo.ClearFaile);
                }
                ModuleMsgArgs arg = MessageExtend.ZoneMsg(dbContext, ZoneMessage.ZONE_CLEAR_COMPLETE, result);
                SendMessasge(arg);
            });
            string message = ZoneDefine.ClearData ? ZoneInfo.ClearShapeConfirm : ZoneInfo.ClearConfirm;
            ShowBox(ZoneInfo.Clear, message, eMessageGrade.Infomation, action);

            //TabMessageBoxDialog mbPage = new TabMessageBoxDialog()
            //{
            //    Header = ZoneInfo.Clear,
            //    Message = ZoneInfo.ClearConfirm,
            //    MessageGrade = eMessageGrade.Infomation,
            //};
            //ThePage.Page.ShowMessageBox(mbPage, (b, r) =>
            //{
            //    if (!(bool)b)
            //        return;
            //    bool result = true;
            //    try
            //    {
            //        var zoneStation = dbContext.CreateZoneWorkStation();
            //        int clearResult = zoneStation.Delete();
            //        int addResult = 1;
            //        Zone china = ZoneHelper.China;
            //        china.ID = Guid.NewGuid();
            //        addResult = zoneStation.Add(china);
            //        result = (clearResult > 0 && addResult > 0) ? true : false;
            //    }
            //    catch (NullReferenceException nx)
            //    {
            //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", nx.Message + nx.StackTrace);
            //        ShowBox(ZoneInfo.Clear, "数据库可能正在使用,无法完成清空操作！");
            //        result = false;
            //    }
            //    catch (Exception ex)
            //    {
            //        YuLinTu.Library.Log.Log.WriteException(this, "OnClearZone(清除地域数据)", ex.Message + ex.StackTrace);
            //        result = false;
            //    }
            //    if (result)
            //        Refresh();
            //    else
            //        ShowBox(ZoneInfo.Clear, ZoneInfo.ClearFaile);
            //    ModuleMsgArgs arg = MessageExtend.ZoneMsg(dbContext, ZoneMessage.ZONE_CLEAR_COMPLETE, result);
            //    SendMessasge(arg);
            //});
        }

        /// <summary>
        /// 刷新地域
        /// </summary>
        public void Refresh()
        {
            //CurrentZone = null;
            var system = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = system.GetProfile<CommonBusinessDefine>();
            var section = profile.GetSection<CommonBusinessDefine>();
            var config = section.Settings as CommonBusinessDefine;
            string code = config.CurrentZoneFullCode;
            RefreshContent(code);
        }

        /// <summary>
        /// 刷新内容
        /// </summary>
        /// <param name="zoneCode"></param>
        public void RefreshContent(string code = "")
        {
            if (worker == null)
            {
                return;
            }
            if (worker.IsBusy)
            {
                return;
            }
            worker.RunWorkerAsync(string.IsNullOrEmpty(code) ? RootZoneCode : code);
        }

        /// <summary>
        /// 上传数据至服务
        /// </summary>
        public void UpdateToServie()
        {
            ServiceSetDefine service = GetServiceSetting();
            if (string.IsNullOrEmpty(service.BusinessDataAddress))
            {
                ShowBox(ZoneInfo.ZoneUpService, ZoneInfo.ZoneNoServiceAddress);
                return;
            }
            if (CurrentZone == null)
            {
                ShowBox(ZoneInfo.ZoneUpService, ZoneInfo.ZoneUpServiceRoot);
                return;
            }
            IDbContext dbContext = CreateDb();
            Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, r) =>
            {
                if (b == false)
                {
                    return;
                }
                Guid? id = ThePage.Workspace.GetUserProfile().SessionCode;
                TaskZoneArgument meta = new TaskZoneArgument();
                meta.UserName = ThePage.Workspace.GetUserProfile().Name;
                meta.SessionCode = id.HasValue ? id.Value.ToString() : "";
                meta.Database = dbContext;
                meta.CurrentZone = CurrentZone;
                meta.ServiceDefine = service;
                meta.Define = ZoneDefine;
                meta.ArgType = eZoneArgType.UpServiceData;
                TaskZoneOperation task = new TaskZoneOperation();
                task.Argument = meta;
                task.Name = "上传数据";
                task.Description = string.Format("上传{0}地域数据至服务器", CurrentZone.Name);
                ThePage.TaskCenter.Add(task);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                task.StartAsync();
            });
            ShowBox(ZoneInfo.ZoneUpService, ZoneInfo.ZoneUpServiceMessage, eMessageGrade.Infomation, action);

            //TabMessageBoxDialog messagePage = new TabMessageBoxDialog()
            //{
            //    Header = ZoneInfo.ZoneUpService,
            //    Message = ZoneInfo.ZoneUpServiceMessage,
            //    MessageGrade = eMessageGrade.Infomation
            //};
            //ThePage.Page.ShowDialog(messagePage, (b, r) =>
            //{
            //    if (b == false)
            //    {
            //        return;
            //    }
            //    Guid? id = ThePage.Workspace.GetUserProfile().SessionCode;
            //    TaskZoneArgument meta = new TaskZoneArgument();
            //    meta.UserName = ThePage.Workspace.GetUserProfile().Name;
            //    meta.SessionCode = id.HasValue ? id.Value.ToString() : "";
            //    meta.Database = dbContext;
            //    meta.CurrentZone = CurrentZone;
            //    meta.ServiceDefine = service;
            //    meta.ArgType = eZoneArgType.UpServiceData;
            //    TaskZoneOperation task = new TaskZoneOperation();
            //    task.Argument = meta;
            //    task.Name = ZoneInfo.ExportPackage;
            //    task.Description = string.Format(ZoneInfo.ExportPackageComment, CurrentZone.Name);
            //    ThePage.TaskCenter.Add(task);
            //    if (ShowTaskViewer != null)
            //    {
            //        ShowTaskViewer();
            //    }
            //    task.StartAsync();
            //});
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue(string header, string message)
        {
            if (CurrentZone == null)
            {
                ShowBox(header, message);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControl(List<Zone> list)
        {
            if (list == null || list.Count == 0)
                return;
            eZoneLevel zonelevel = list.Max(t => t.Level);
            bindList.Clear();
            List<Zone> zones = list.FindAll(t => t.Level == zonelevel);
            foreach (Zone z in zones)
            {
                ZoneDataItem node = ZoneDataItemHelper.ConvertToDataItem(z);
                List<Zone> childrenZone = list.FindAll(t => t.UpLevelCode == z.FullCode);
                CreatTree(childrenZone, node, list);
                bindList.Add(node);
            }
            view.ItemsSource = bindList;
            view.ExpandAsync(objs =>
            {
                foreach (var item in objs)
                {
                    var obj = item as ZoneDataItem;
                    if (obj == null)
                        continue;
                    if ((obj.Level > eZoneLevel.Town) || obj.FullCode == "86")
                        return obj;
                }
                return null;
            });
        }

        /// <summary>
        /// 创建树
        /// </summary> 
        private void CreatTree(List<Zone> zones, ZoneDataItem node, List<Zone> allZone)
        {
            foreach (Zone z in zones)
            {
                ZoneDataItem nodeChildren = ZoneDataItemHelper.ConvertToDataItem(z);
                node.Children.Add(nodeChildren);
                List<Zone> childrenZone = allZone.FindAll(t => t.UpLevelCode == z.FullCode);
                CreatTree(childrenZone, nodeChildren, allZone);
            }
        }

        /// <summary>
        /// 更新数据实体
        /// </summary>
        private void UpDataToItem(ZoneDataItem desticItem, ZoneDataItem sourceItem)
        {
            if (sourceItem == null || desticItem == null)
            {
                return;
            }
            desticItem.Code = sourceItem.Code;
            desticItem.Comment = sourceItem.Comment;
            desticItem.FullCode = sourceItem.FullCode;
            desticItem.FullName = sourceItem.FullName;
            desticItem.ID = sourceItem.ID;
            desticItem.Img = ZoneDataItemHelper.GetImgByLevel(sourceItem.Level);
            desticItem.Level = sourceItem.Level;
            desticItem.Name = sourceItem.Name;
            desticItem.Shape = sourceItem.Shape;
            desticItem.UpLevelCode = sourceItem.UpLevelCode;
            desticItem.UpLevelName = sourceItem.UpLevelName;
            desticItem.Visibility = Visibility.Visible;
            desticItem.CreateTime = sourceItem.CreateTime;
            desticItem.CreateUser = sourceItem.CreateUser;
            desticItem.LastModifyTime = sourceItem.LastModifyTime;
            desticItem.LastModifyUser = sourceItem.LastModifyUser;
        }

        /// <summary>
        /// 将节点从父节点中移除
        /// </summary>
        private void RemoveFromParent(ObservableCollection<ZoneDataItem> list, Guid id)
        {
            if (list.Count == 0)
                return;
            ZoneDataItem dataItem = null;
            foreach (ZoneDataItem item in list)
            {
                dataItem = item.Children.Where(t => t.ID == id).FirstOrDefault();
                if (dataItem != null)
                {
                    item.Children.Remove(dataItem);
                    break;
                }
                else
                {
                    RemoveFromParent(item.Children, id);
                }
            }
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        private ZoneDefine GetSystemSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            return section.Settings;
        }

        /// <summary>
        /// 获取服务配置
        /// </summary>
        private ServiceSetDefine GetServiceSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ServiceSetDefine>();
            var section = profile.GetSection<ServiceSetDefine>();
            return section.Settings;
        }

        #endregion

        #endregion

        #region Event

        /// <summary>
        /// 双击节点事件
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// 选择节点
        /// </summary>
        private void view_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectItem = null;
            CurrentZone = null;
            ZoneDataItem item = view.SelectedItem as ZoneDataItem;
            if (item != null)
            {
                selectItem = item;
                CurrentZone = item.ConvertTo<Zone>();
            }
        }

        /// <summary>
        /// 是否显示展开符号
        /// </summary>
        private void view_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
            var item = e.Object as ZoneDataItem;
            if (item == null)
                return;
            e.HasItems = item.Children.Count > 0;
        }

        #endregion

        #region Helper

        /// <summary>
        /// 设置编码长度
        /// </summary>
        /// <param name="level"></param>
        private int GetLevelLength(eZoneLevel level)
        {
            int length = 0;
            switch (level)
            {
                case eZoneLevel.Group:
                    length = 2;
                    break;
                case eZoneLevel.Village:
                    length = 3;
                    break;
                case eZoneLevel.Town:
                    length = 3;
                    break;
                case eZoneLevel.County:
                    length = 2;
                    break;
                case eZoneLevel.City:
                    length = 2;
                    break;
                case eZoneLevel.Province:
                    length = 2;
                    break;
                case eZoneLevel.State:
                    length = 2;
                    break;
                default:
                    length = 4;
                    break;
            }
            return length;
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ThePage.Page.ShowDialog(new TabMessageBoxDialog()
                {
                    Header = title,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                }, action);
            }));
        }

        /// <summary>
        /// 创建数据连接
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource(); //DataSource.Create<DbContext>(TheBns.Current.GetDataSourceName());
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            ThePage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            ThePage.Workspace.Message.Send(this, args);
        }

        #endregion

        #region ContextMenue

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        /// <summary>
        /// 删除
        /// </summary>
        private void miDel_Click(object sender, RoutedEventArgs e)
        {
            Del();
        }

        ///// <summary>
        ///// 设置
        ///// </summary>
        //private void miSet_Click(object sender, RoutedEventArgs e)
        //{
        //}

        #endregion
    }
}
