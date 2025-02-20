/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包台账地块管理界面
    /// </summary>
    public partial class ContractAccountPanel
    {
        #region 导入数据

        #region 导入界址调查表

        /// <summary>
        /// 导入界址调查表数据
        /// </summary>
        public void ImportBoundaryData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportBoundaryData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext db = DataBaseSource.GetDataBaseSource();
            LanderType landerType = LanderType.AgricultureLand;
            if (currentZone.Level == eZoneLevel.Group)
            {
                bool canContinue = CanContinue(landerType, db);
                if (!canContinue)
                {
                    return;
                }
                ImportSingeSurvey(db, landerType);
            }
            else if (currentZone.Level == eZoneLevel.Village)
            {
                List<Zone> childrenZone = null;
                try
                {
                    var zoneStation = db.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                }
                catch (Exception ex)
                {
                    Log.Log.WriteException(this, "ImportBoundaryData", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.ImportBoundaryData, "查询地域数据出错，无法执行导入数据");
                    return;
                }
                if (childrenZone == null || childrenZone.Count == 0)
                {
                    bool canContinue = CanContinue(landerType, db);
                    if (!canContinue)
                    {
                        return;
                    }
                    ImportSingeSurvey(db, landerType);
                }
                else
                {
                    bool canContinue = CanContinue(landerType, db, true);
                    if (!canContinue)
                    {
                        return;
                    }
                    childrenZone.Add(currentZone);
                    ImportBitchSurvery(db, landerType, childrenZone);
                }
            }
            else
            {
                //选择地域为镇(或大于镇)
                ShowBox(ContractAccountInfo.ImportBoundaryData, ContractAccountInfo.ImportErrorZone);
                return;
            }
        }

        /// <summary>
        /// 批量导入调查表
        /// </summary>
        private void ImportBitchSurvery(IDbContext db, LanderType landerType, List<Zone> zoneList)
        {
            ExportDataPage importLand = new ExportDataPage(currentZone.FullName, TheWorkPage, "批量导入界址调查表", "导入选择文件目录下的文件", "导入", "导入路径:");
            TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
            {
                if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                {
                    return;
                }
                var dicStation = db.CreateDictWorkStation();
                TaskImportBoundarytArgument metadata = new TaskImportBoundarytArgument()
                {
                    CurrentZone = currentZone,
                    LandorType = landerType,
                    ZoneList = zoneList,
                    Dbcontext = db,
                    FileName = importLand.FileName
                };
                try
                {
                    metadata.DicList = dicStation.Get();
                }
                catch (Exception ex)
                {
                    Log.Log.WriteException(this, "承包台账(ImportBitchSurvery)", ex.Message + ex.StackTrace);
                }
                if (metadata.DicList == null || metadata.DicList.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ImportBoundaryData, "获取数据字典内容失败，不能进行界址调查信息表导入");
                    return;
                }
                TaskGroupImportBoundary taskGroup = new TaskGroupImportBoundary();
                taskGroup.Name = "批量导入界址调查表";
                taskGroup.Description = "导入选择文件目录下的界址调查表";
                taskGroup.Argument = metadata;
                taskGroup.Completed += (s, e) =>
                {
                    var arg = MessageExtend.CreateMsg(db, ContractAccountMessage.CONTRACTACCOUNT_MULTIIMPORTBOUNDARY_COMPLETE, currentZone.FullCode);
                    TheBns.Current.Message.Send(this, arg);
                    TheWorkPage.Message.Send(this, arg);
                    TheWorkPage.Workspace.Message.Send(this, arg);
                };
                TheWorkPage.TaskCenter.Add(taskGroup);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                taskGroup.StartAsync();
            });
        }

        /// <summary>
        /// 导入单个调查表
        /// </summary>
        private void ImportSingeSurvey(IDbContext db, LanderType landerType)
        {
            ImportDataPage importLand = new ImportDataPage(TheWorkPage, "导入界址调查表");
            TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
            {
                if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                {
                    return;
                }
                TaskImportBoundarytArgument metadata = new TaskImportBoundarytArgument();
                var dicStation = db.CreateDictWorkStation();
                try
                {
                    metadata.DicList = dicStation.Get();
                }
                catch (Exception ex)
                {
                    Log.Log.WriteException(this, "承包台账(ImportBoundaryData)", ex.Message + ex.StackTrace);
                }
                if (metadata.DicList == null || metadata.DicList.Count == 0)
                {
                    ShowBox(ContractAccountInfo.ImportBoundaryData, "获取数据字典内容失败，不能进行界址调查信息表导入");
                    return;
                }
                metadata.Dbcontext = db;
                metadata.CurrentZone = currentZone;
                metadata.LandorType = landerType;
                metadata.FileName = importLand.FileName;
                AgricultureLandDotSurvey importTable = new AgricultureLandDotSurvey();
                importTable.Name = ContractAccountInfo.ImportBoundaryData;
                importTable.Description = "导入选择的界址调查表";
                importTable.Argument = metadata;
                importTable.Completed += (s, e) =>
                {
                    var arg = MessageExtend.CreateMsg(db, ContractAccountMessage.CONTRACTACCOUNT_IMPORTBOUNDARY_COMPLETE, currentZone.FullCode);
                    TheBns.Current.Message.Send(this, arg);
                    TheWorkPage.Message.Send(this, arg);
                    TheWorkPage.Workspace.Message.Send(this, arg);
                };
                TheWorkPage.TaskCenter.Add(importTable);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                importTable.StartAsync();
            });
        }

        /// <summary>
        /// 是否可继续
        /// </summary>
        private bool CanContinue(LanderType landerType, IDbContext dbContext, bool isbitch = false)
        {
            bool reslut = true;
            string errorInfo = string.Empty;
            if (currentZone == null)
            {
                errorInfo = "当前地域无效";
                reslut = false;
            }
            if (dbContext == null)
            {
                errorInfo += ((string.IsNullOrEmpty(errorInfo)) ? "" : ",") + "数据连接无效";
                reslut = false;
            }
            int count = 0;
            try
            {
                switch (landerType)
                {
                    case LanderType.AgricultureLand:
                        var landStation = dbContext.CreateContractLandWorkstation();
                        count = landStation.Count(currentZone.FullCode, eLevelOption.Self);
                        break;
                    case LanderType.CollectiveLand:
                        //count = db.CollectiveLand.SL_Count("SenderCode", currentZone.FullCode, Library.Data.ConditionOption.Equal);
                        break;
                    case LanderType.HomeSteadLand:
                        // count = db.BuildLandProperty.SL_Count("LandLocatedCode", currentZone.FullCode, Library.Data.ConditionOption.Equal);
                        break;
                    case LanderType.WoodLand:
                        // count = db.ForestryLand.SL_Count("SenderCode", currentZone.FullCode, Library.Data.ConditionOption.Equal);
                        break;
                    case LanderType.Irrigation:
                        // count = db.Irrigation.SL_Count("SenderCode", currentZone.FullCode, Library.Data.ConditionOption.Equal);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Log.WriteException(this, "承包台账界址信息导入(CanContinue)", ex.Message + ex.StackTrace);
            }
            if (count <= 0 && !isbitch)
            {
                errorInfo += ((string.IsNullOrEmpty(errorInfo)) ? "" : ",") + "当前行政区域下没有地块数据可供操作!";
                reslut = false;
            }
            if (!reslut)
                ShowBox(ContractAccountInfo.ImportBoundaryData, errorInfo);
            return reslut;
        }

        #endregion

        #region 导入压缩包

        /// <summary>
        /// 导入压缩包数据
        /// </summary>
        public void ImportZipData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ImportLandZipData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext db = DataBaseSource.GetDataBaseSource();
            LanderType landerType = LanderType.AgricultureLand;
            if (currentZone.Level == eZoneLevel.Group)
            {
                ImportSingeZip(db, landerType);
            }
            else
            {
                List<Zone> childrenZone = null;
                try
                {
                    var zoneStation = db.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                }
                catch (Exception ex)
                {
                    Log.Log.WriteException(this, "ImportZipData", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.ImportBoundaryData, "查询地域数据出错，无法执行导入数据");
                    return;
                }
                if (childrenZone == null || childrenZone.Count == 0)
                {
                    ImportSingeZip(db, landerType);
                }
                else
                {
                    childrenZone.Add(currentZone);
                    ImportBitchZip(db, landerType, childrenZone);
                }
            }
        }

        /// <summary>
        /// 导入单个压缩包
        /// </summary>
        private void ImportSingeZip(IDbContext db, LanderType landerType)
        {
            ImportDataPage importLand = new ImportDataPage(TheWorkPage, "导入土地调查压缩包", "", "ZIP压缩包(*.zip)|*.zip");
            TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
            {
                if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                {
                    return;
                }
                ArcLandImporArgument metadata = new ArcLandImporArgument();
                //var dicStation = db.CreateDictWorkStation();
                //try
                //{
                //    metadata.DicList = dicStation.Get();
                //}
                //catch (Exception ex)
                //{
                //    Log.Log.WriteException(this, "承包台账(ImportBoundaryData)", ex.Message + ex.StackTrace);
                //}
                //if (metadata.DicList == null || metadata.DicList.Count == 0)
                //{
                //    ShowBox(ContractAccountInfo.ImportBoundaryData, "获取数据字典内容失败，不能进行界址调查信息表导入");
                //    return;
                //}
                metadata.Database = db;
                metadata.CurrentZone = currentZone;
                metadata.LandorType = landerType;
                metadata.FileName = importLand.FileName;
                ArcLandImportProgress importZip = new ArcLandImportProgress();
                importZip.Name = ContractAccountInfo.ImportLandZipData;
                importZip.Description = "导入选择的土地调查压缩包数据";
                importZip.Argument = metadata;
                importZip.Completed += (s, e) =>
                {
                    this.Refresh();
                    var arg = MessageExtend.CreateMsg(db, ContractAccountMessage.CONTRACTACCOUNT_IMPORTZIP_COMPLATE, currentZone.FullCode);
                    TheBns.Current.Message.Send(this, arg);
                    TheWorkPage.Message.Send(this, arg);
                    TheWorkPage.Workspace.Message.Send(this, arg);
                };
                TheWorkPage.TaskCenter.Add(importZip);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                importZip.StartAsync();
            });
        }

        /// <summary>
        /// 批量导入压缩包
        /// </summary>
        private void ImportBitchZip(IDbContext db, LanderType landerType, List<Zone> childrenZone)
        {
            ExportDataPage importLand = new ExportDataPage(currentZone.FullName, TheWorkPage, "批量导入土地调查压缩包", "导入选择文件目录下的调查压缩包", "导入","导入路径");
           
            TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
            {
                if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                {
                    return;
                }
                var dicStation = db.CreateDictWorkStation();
                ArcLandImporArgument metadata = new ArcLandImporArgument()
                {
                    CurrentZone = currentZone,
                    LandorType = landerType,
                    ZoneList = childrenZone,
                    Database = db,
                    FileName = importLand.FileName
                };
                TaskGroupImportZip taskGroup = new TaskGroupImportZip();
                taskGroup.Name = "批量导入压缩包";
                taskGroup.Description = "导入选择文件目录下的压缩包数据";
                taskGroup.Argument = metadata;
                taskGroup.Completed += (s, e) =>
                {
                    this.Refresh();
                    var arg = MessageExtend.CreateMsg(db, ContractAccountMessage.CONTRACTACCOUNT_IMPORTZIP_COMPLATE, currentZone.FullCode);
                    TheBns.Current.Message.Send(this, arg);
                    TheWorkPage.Message.Send(this, arg);
                    TheWorkPage.Workspace.Message.Send(this, arg);
                };
                TheWorkPage.TaskCenter.Add(taskGroup);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                taskGroup.StartAsync();
            });
        }
        #endregion

        #region 导出压缩包

        /// <summary>
        /// 导出压缩包数据
        /// </summary>
        public void ExportZipData()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.ExportLandZipData, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext db = DataBaseSource.GetDataBaseSource();
            LanderType landerType = LanderType.AgricultureLand;
            if (currentZone.Level == eZoneLevel.Group)
            {
                ExportSingeZip(db, landerType);
            }
            else
            {
                List<Zone> childrenZone = null;
                try
                {
                    var zoneStation = db.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.Subs);
                }
                catch (Exception ex)
                {
                    Log.Log.WriteException(this, "ExportZipData", ex.Message + ex.StackTrace);
                    ShowBox(ContractAccountInfo.ExportLandZipData, "查询地域数据出错，无法执行导出数据");
                    return;
                }
                if (childrenZone == null || childrenZone.Count == 0)
                {
                    ExportSingeZip(db, landerType);
                }
                else
                {
                    //childrenZone.Add(currentZone);
                    ExportBitchZip(db, landerType, childrenZone);
                }
            }
        }

        /// <summary>
        /// 导出单个压缩包
        /// </summary>
        private void ExportSingeZip(IDbContext db, LanderType landerType)
        {
            ExportDataPage exportZip = new ExportDataPage(currentZone.FullName, TheWorkPage, "导出土地调查压缩包");
            TheWorkPage.Page.ShowMessageBox(exportZip, (b, r) =>
            {
                if (string.IsNullOrEmpty(exportZip.FileName) || b == false)
                {
                    return;
                }
                ArcLandImporArgument zoneMeta = new ArcLandImporArgument();
                zoneMeta.Database = db;
                zoneMeta.OpratorName = "Package";
                zoneMeta.CurrentZone = currentZone;
                zoneMeta.FileName = exportZip.FileName;
                ArcLandExportProgress dataProgress = new ArcLandExportProgress();
                dataProgress.Name = "导出压缩包";
                dataProgress.Argument = zoneMeta;
                dataProgress.Description = string.Format("导出{0}下的数据压缩包", currentZone.FullName);
                TheWorkPage.TaskCenter.Add(dataProgress);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                dataProgress.StartAsync();
            });
        }

        /// <summary>
        /// 批量导出压缩包
        /// </summary>
        private void ExportBitchZip(IDbContext db, LanderType landerType, List<Zone> childrenZone)
        {
            ExportDataPage importLand = new ExportDataPage(currentZone.FullName, TheWorkPage, "批量导出土地调查压缩包", "导出调查压缩包的文件目录", "导出");
            TheWorkPage.Page.ShowMessageBox(importLand, (b, r) =>
            {
                if (string.IsNullOrEmpty(importLand.FileName) || b == false)
                {
                    return;
                }
                var dicStation = db.CreateDictWorkStation();
                ArcLandImporArgument metadata = new ArcLandImporArgument()
                {
                    CurrentZone = currentZone,
                    LandorType = landerType,
                    ZoneList = childrenZone,
                    Database = db,
                    FileName = importLand.FileName
                };
                TaskGroupExportZip taskGroup = new TaskGroupExportZip();
                taskGroup.Name = "批量导出压缩包";
                taskGroup.Description = "导出压缩包数据到选择文件目录下";
                taskGroup.Argument = metadata;
                taskGroup.Completed += (s, e) =>
                {
                    this.Refresh();
                    var arg = MessageExtend.CreateMsg(db, ContractAccountMessage.CONTRACTACCOUNT_IMPORTZIP_COMPLATE, currentZone.FullCode);
                    TheBns.Current.Message.Send(this, arg);
                    TheWorkPage.Message.Send(this, arg);
                    TheWorkPage.Workspace.Message.Send(this, arg);
                };
                TheWorkPage.TaskCenter.Add(taskGroup);
                if (ShowTaskViewer != null)
                {
                    ShowTaskViewer();
                }
                taskGroup.StartAsync();
            });
        }
        #endregion

        #endregion

        #region 工具

        #region 初始数据

        /// <summary>
        /// 初始化图幅编号
        /// </summary>
        public void InitialMapNumber()
        {
            if (CurrentZone == null)
            {
                ShowBox(ContractAccountInfo.InitialImageNumber, ContractAccountInfo.CurrentZoneNoSelected);
                return;
            }
            IDbContext dbContext = this.DbContext;
            List<Zone> allZones = new List<Zone>();
            allZones = GetAllChildrenZones() as List<Zone>;
            List<Zone> childrenZone = new List<Zone>();
            childrenZone = allZones.FindAll(c => c.FullCode != currentZone.FullCode);
            var landstation = DbContext.CreateContractLandWorkstation();
            landList = landstation.GetCollection(CurrentZone.FullCode, eLevelOption.Self);
            List<ContractLand> listGeoLand = landList == null ? new List<ContractLand>() : landList.FindAll(c => c.Shape != null);
            if ((currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0)) && (listGeoLand == null || listGeoLand.Count == 0))
            {
                //地域下没有空间地块
                ShowBox(ContractAccountInfo.InitialImageNumber, ContractAccountInfo.CurrentZoneNoGeoLand);
                return;
            }
            if (currentZone.Level > eZoneLevel.Town)
            {
                //选择地域大于镇级
                ShowBox(ContractAccountInfo.InitialImageNumber, ContractAccountInfo.InitialColumnSelectedZoneError);
                return;
            }
            ContractLandInitialImageNumber initialImageNumber = new ContractLandInitialImageNumber();
            initialImageNumber.Workpage = TheWorkPage;
            TheWorkPage.Page.ShowMessageBox(initialImageNumber, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (currentZone.Level == eZoneLevel.Group || (currentZone.Level > eZoneLevel.Group && childrenZone.Count == 0))
                {
                    //选择地域为组级地域或者大于组级地域时其下不存在子级地域(执行单个任务)
                    InitialMapNumberTask(initialImageNumber, listGeoLand);
                }
                else if (currentZone.Level > eZoneLevel.Group && childrenZone.Count > 0)
                {
                    //选择地域为大于组级地域并且其下有子级地域(执行组任务)
                    InitialMapNumberGroupTask(initialImageNumber, allZones);
                }
            });
        }

        /// <summary>
        /// 批量初始化图幅编号任务(组任务)
        /// </summary>
        /// <param name="imageNumberPage">初始化图幅编号界面</param>
        /// <param name="listGeoLand">空间地块集合(当前地域下及子级地域下)</param>
        /// <param name="allZones">所有地域集合(当前地域和其下的子级地域)</param>
        private void InitialMapNumberGroupTask(ContractLandInitialImageNumber imageNumberPage, List<Zone> allZones)
        {
            TaskGroupInitialImageNumberArgument groupArgument = new TaskGroupInitialImageNumberArgument();
            groupArgument.CurrentZone = CurrentZone;
            groupArgument.DbContext = this.DbContext;
            groupArgument.AllZones = allZones;
            groupArgument.ScropeIndex = imageNumberPage.ScropeIndex;
            groupArgument.ScalerIndex = imageNumberPage.ScalerIndex;
            groupArgument.IsUseYX = imageNumberPage.IsUseYX;
            groupArgument.IsInitialAllImageNumber = imageNumberPage.IsInitialAllImageNumber;
            TaskGroupInitialImageNumberOperation groupOperation = new TaskGroupInitialImageNumberOperation();
            groupOperation.Argument = groupArgument;
            groupOperation.Name = ContractAccountInfo.InitialImageNumber;
            groupOperation.Description = ContractAccountInfo.InitialImageNumber;
            groupOperation.Workpage = TheWorkPage;
            groupOperation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(this.DbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALIMAGENUMBER_COMPLETE, groupOperation.InitialLands, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(groupOperation);
            if (ShowTaskViewer != null)
                ShowTaskViewer();
            groupOperation.StartAsync();
        }

        /// <summary>
        /// 初始化图幅编号任务(单个任务)
        /// </summary>
        /// <param name="imageNumberPage">初始化图幅编号界面</param>
        /// <param name="listGeoLand">空间地块集合(当前地域下及子级地域下)</param>
        public void InitialMapNumberTask(ContractLandInitialImageNumber imageNumberPage, List<ContractLand> listGeoLand)
        {
            IDbContext dbContext = CreateDb();
            TaskInitialImageNumberArgument argument = new TaskInitialImageNumberArgument();
            argument.CurrentZone = CurrentZone;
            argument.DbContext = dbContext;
            argument.ListGeoLand = listGeoLand;
            argument.ScropeIndex = imageNumberPage.ScropeIndex;
            argument.ScalerIndex = imageNumberPage.ScalerIndex;
            argument.IsUseYX = imageNumberPage.IsUseYX;
            argument.IsInitialAllImageNumber = imageNumberPage.IsInitialAllImageNumber;
            TaskInitialImageNumberOperation operation = new TaskInitialImageNumberOperation();
            operation.Argument = argument;
            operation.Name = ContractAccountInfo.InitialImageNumber;
            operation.Description = ContractAccountInfo.InitialImageNumber;
            operation.Completed += new TaskCompletedEventHandler((o, t) =>
            {
                Refresh();
                ModuleMsgArgs args = MessageExtend.ContractAccountMsg(dbContext, ContractAccountMessage.CONTRACTACCOUNT_INITIALIMAGENUMBER_COMPLETE, listGeoLand, CurrentZone.FullCode);
                SendMessasge(args);
            });
            TheWorkPage.TaskCenter.Add(operation);
            if (ShowTaskViewer != null)
            {
                ShowTaskViewer();
            }
            operation.StartAsync();
        }

        #endregion

        #endregion
    }
}
