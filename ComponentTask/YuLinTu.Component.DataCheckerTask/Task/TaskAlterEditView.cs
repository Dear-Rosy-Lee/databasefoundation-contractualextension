/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Data;
using System.Windows.Threading;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;

namespace YuLinTu.Component.DataCheckerTask
{
    /// <summary>
    /// 错误信息编辑界面参数
    /// </summary>
    public class TaskAlterEditView<T> : TaskAlertMetadata
    {
        #region Ctor

        public TaskAlterEditView()
        {
        }

        #endregion

        #region Fields

        #endregion

        #region Property

        /// <summary>
        /// 信息提示参数
        /// </summary>
        public TaskAlertEventArgs AlterArgs { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 当前地域下的所有承包方集合
        /// </summary>
        public List<VirtualPerson> CurListVirtualPerson { get; set; }

        /// <summary>
        /// 当前地域下的所有承包地块集合
        /// </summary>
        public List<ContractLand> CurListContractLand { get; set; }

        /// <summary>
        /// 当前地域下的所有承包合同集合
        /// </summary>
        public List<ContractConcord> CurListConcord { get; set; }

        /// <summary>
        /// 当前地域下的所有权证集合
        /// </summary>
        public List<ContractRegeditBook> CurListBook { get; set; }

        /// <summary>
        /// 当前发包方
        /// </summary>
        public CollectivityTissue CurrentTissue { get; set; }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson CurrentVirtualPerson { get; set; }

        /// <summary>
        /// 当前共有人
        /// </summary>
        public Person CurrentPerson { get; set; }

        /// <summary>
        /// 当前承包地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        /// <summary>
        /// 当前承包合同
        /// </summary>
        public ContractConcord CurrentConcord { get; set; }

        /// <summary>
        /// 当前承包权证
        /// </summary>
        public ContractRegeditBook CurrentRegeditBook { get; set; }

        #endregion

        #region Method

        #region Sender

        /// <summary>
        /// 通过编辑修改发包方错误/警告信息
        /// </summary>
        public void EditSenderAlterDetails()
        {
            try
            {
                if (Workpage == null || CurrentTissue == null)
                    return;
                SenderEditPage editPage = new SenderEditPage(Workpage);
                editPage.CurrentTissue = CurrentTissue.Clone() as CollectivityTissue;
                Workpage.Page.ShowMessageBox(editPage, (b, a) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    if (!editPage.Result)
                    {
                        ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, SenderInfo.EditFail, eMessageGrade.Error, false, true);
                    }
                    var args = MessageExtend.SenderMsg(DbContext, SenderMessage.SENDER_UPDATE_COMPLETE, (editPage.Result ? editPage.CurrentTissue : null));
                    SendMessasge(args);
                    if (!AlterArgs.Description.Contains("已完成"))
                        AlterArgs.Description += " (已完成)";
                });
            }
            catch
            {
                ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, "修改发包方信息失败!", eMessageGrade.Error, false, true);
            }
        }

        #endregion

        #region VirtualPerson

        /// <summary>
        /// 通过编辑修改承包方/共有人错误/警告信息
        /// </summary>
        public void EditPersonAlterDetails()
        {
            try
            {
                if (CurrentVirtualPerson == null && CurrentPerson == null)
                {
                    ToolShowDialog.ShowBox(Workpage, VirtualPersonInfo.EditData, VirtualPersonInfo.EditDataNo, eMessageGrade.Error, false, true);
                    return;
                }
                if (CurrentVirtualPerson.Status == eVirtualPersonStatus.Lock)
                {
                    Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((b, e) =>
                    {
                        if (!(bool)b)
                            return;
                        StartEdit(true);
                    });
                    ToolShowDialog.ShowBox(Workpage, VirtualPersonInfo.EditData, VirtualPersonInfo.EditPersonLock, eMessageGrade.Warn, false, true, action);
                }
                else
                {
                    StartEdit(false);
                }
            }
            catch
            {
                ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, "修改承包方信息失败!", eMessageGrade.Error, false, true);
            }
        }

        /// <summary>
        /// 开始编辑
        /// </summary>
        /// <param name="isLock">是否锁定</param>
        private void StartEdit(bool isLock)
        {
            if (CurrentVirtualPerson != null)
            {
                if (CurrentPerson == null)
                {
                    UpdateVirtualPerson(isLock);
                }
                else
                {
                    UpdateSharePerson(isLock);
                }
            }
        }

        /// <summary>
        /// 更新承包方
        /// </summary>
        public void UpdateVirtualPerson(bool isLock)
        {
            VirtualPersonBusiness vpBus = new VirtualPersonBusiness(DbContext);
            vpBus.VirtualType = eVirtualType.Land;
            VirtualPersonInfoPage personPage = new VirtualPersonInfoPage(CurrentVirtualPerson, CurrentZone, vpBus, isLock: isLock);
            personPage.Items = CurListVirtualPerson;
            personPage.Workpage = Workpage;
            personPage.Header = "编辑承包方";
            personPage.OtherDefine = FamilyOtherDefine.GetIntence();
            Workpage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (!personPage.Result)
                {
                    ToolShowDialog.ShowBox(Workpage, VirtualPersonInfo.AddVirtualPerson, VirtualPersonInfo.AddVPFail, eMessageGrade.Error, false, true);
                    return;
                }
                var args = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.VIRTUALPERSON_EDIT_COMPLATE, personPage.Contractor);
                SendMessasge(args);
                CurrentVirtualPerson = personPage.Contractor;
                if (!AlterArgs.Description.Contains("已完成"))
                    AlterArgs.Description += " (已完成)";
            });
        }

        /// <summary>
        /// 编辑共有人
        /// </summary>
        public void UpdateSharePerson(bool isLock)
        {
            VirtualPersonBusiness vpBus = new VirtualPersonBusiness(DbContext);
            vpBus.VirtualType = eVirtualType.Land;
            PersonInfoPage personPage = new PersonInfoPage(CurrentVirtualPerson, false);
            personPage.Business = vpBus;
            personPage.Person = CurrentPerson;
            personPage.PersonItems = CurListVirtualPerson;
            personPage.Workpage = Workpage;
            personPage.OtherDefine = FamilyOtherDefine.GetIntence();
            personPage.IsLock = isLock;
            Workpage.Page.ShowMessageBox(personPage, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                if (!personPage.Result)
                {
                    ToolShowDialog.ShowBox(Workpage, VirtualPersonInfo.EditData, VirtualPersonInfo.EditDataFail, eMessageGrade.Error, false, true);
                    return;
                }
                MultiObjectArg arg = new MultiObjectArg() { ParameterA = CurrentVirtualPerson, ParameterB = personPage.Person };
                var msgArg = MessageExtend.VirtualPersonMsg(DbContext, VirtualPersonMessage.SHAREPERSON_EDIT_COMPLATE, arg);
                SendMessasge(msgArg);
                if (!AlterArgs.Description.Contains("已完成"))
                    AlterArgs.Description += " (已完成)";
            });
        }

        #endregion

        #region ContractLand

        /// <summary>
        /// 通过编辑修改承包地块错误/警告信息
        /// </summary>
        public void EditContractLandAlterDetails()
        {
            try
            {
                if (CurrentZone == null)
                {
                    //地域为空
                    ToolShowDialog.ShowBox(Workpage, ContractAccountInfo.ContractLandEdit, ContractAccountInfo.CurrentZoneNoSelected, eMessageGrade.Error, false, true);
                    return;
                }
                if (CurrentLand == null)
                {
                    //此时在界面上没有选中承包地块信息
                    ToolShowDialog.ShowBox(Workpage, ContractAccountInfo.ContractLandEdit, ContractAccountInfo.LandEditNoSelected, eMessageGrade.Error, false, true);
                    return;
                }
                bool isLock = CurrentVirtualPerson.Status == eVirtualPersonStatus.Lock ? true : false;
                if (isLock)
                {
                    Action<bool?, eCloseReason> action = new Action<bool?, eCloseReason>((m, n) =>
                    {
                        if (!(bool)m)
                            return;
                        StartEditLand(isLock);
                    });
                    ToolShowDialog.ShowBox(Workpage, ContractAccountInfo.ContractLandEdit, "当前地块被锁定,只能查看地块信息", eMessageGrade.Infomation, false, true, action);
                }
                else
                {
                    StartEditLand(isLock);
                }
            }
            catch
            {
                ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, "修改承包地块信息失败!", eMessageGrade.Error, false, true);
            }
        }

        /// <summary>
        /// 开始编辑地块
        /// </summary>
        private void StartEditLand(bool isLock)
        {
            ContractLandPage editPage = new ContractLandPage();
            editPage.Workpage = Workpage;
            editPage.CurrentZone = CurrentZone;
            editPage.VirtpersonList = VirtualPersonFilterForEdit();
            editPage.CurrentPerson = CurrentVirtualPerson;
            editPage.IsLock = isLock;
            //editPage.spInitialDotCoil.Visibility = Visibility.Collapsed;
            var landTemp = CurrentLand.Clone() as ContractLand;
            editPage.CurrentLand = landTemp;
            Workpage.Page.ShowMessageBox(editPage, (b, r) =>
            {
                if (!(bool)b)
                    return;
                if (!AlterArgs.Description.Contains("已完成"))
                    AlterArgs.Description += " (已完成)";
            });
        }

        /// <summary>
        /// 过滤显示承包方(编辑时使用)
        /// </summary>
        private List<VirtualPerson> VirtualPersonFilterForEdit()
        {
            List<VirtualPerson> retVps = null;
            try
            {
                if (DbContext == null || CurListVirtualPerson == null)
                    return null;
                ContractBusinessSettingDefine define = ContractBusinessSettingDefine.GetIntence();
                Dictionary<string, VirtualPerson> vpDict = new Dictionary<string, VirtualPerson>();
               
                if (define.DisplayCollectUsingCBdata)
                {
                    //if (vpDict.ContainsKey("集体"))
                    //{
                    //    vpDict.Remove("集体");
                    //}
                    
                    CurListVirtualPerson.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }
                CurListVirtualPerson.ForEach(c => vpDict.Add(c.Name, c));
                
                if (vpDict.Count > 0)
                {
                    retVps = new List<VirtualPerson>(vpDict.Count);
                    foreach (var item in vpDict)
                    {
                        retVps.Add(item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "VitualPersonFilter(承包方过滤失败!)", ex.Message + ex.StackTrace);
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = "过滤承包方",
                    Message = "过滤承包方出错!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                });
            }
            return retVps;
        }

        #endregion

        #region ContractConcord

        /// <summary>
        /// 通过编辑修改承包合同错误/警告信息
        /// </summary>
        public void EditContractConcordAlterDetails()
        {
            try
            {
                if (CurrentZone == null)
                {
                    ToolShowDialog.ShowBox(Workpage, ContractConcordInfo.EditConcord, ContractConcordInfo.CurrentZoneNoSelected, eMessageGrade.Error, false, true);
                    return;
                }
                if (CurrentConcord == null)
                {
                    ToolShowDialog.ShowBox(Workpage, ContractConcordInfo.EditConcord, ContractConcordInfo.EditDataNoSelected, eMessageGrade.Error, false, true);
                    return;
                }
                List<VirtualPerson> listPerosn = new List<VirtualPerson>();
                listPerosn.Add(CurrentVirtualPerson);
                ConcordInfoPage editPage = new ConcordInfoPage();
                editPage.Workpage = Workpage;
                editPage.CurrentVp = CurrentVirtualPerson;
                editPage.IsAdd = false;
                editPage.CurrentZone = CurrentZone;
                editPage.Business = new ConcordBusiness(DbContext);
                editPage.ContracterList = listPerosn;
                editPage.LandList = CurListContractLand;
                editPage.dbContext = DbContext;
                editPage.ConcordSettingDefine = ContractConcordSettingDefine.GetIntence(); //多种合同，配置文件管理
                ContractConcord concord = CurListConcord.Find(c => c.ID == CurrentConcord.ID);
                if (concord == null)
                {
                    ToolShowDialog.ShowBox(Workpage, ContractConcordInfo.EditConcord, ContractConcordInfo.EditDataNoData, eMessageGrade.Error, false, true);
                    return;
                }
                var concordTemp = concord.Clone() as ContractConcord;
                editPage.Concord = concordTemp;
                Workpage.Page.ShowMessageBox(editPage, (b, r) =>
                {
                    if (!(bool)b)
                    {
                        return;
                    }
                    ModuleMsgArgs args = MessageExtend.ContractConcordMsg(DbContext, ConcordMessage.CONCORD_EDIT_COMPLATE, concordTemp, CurrentZone.FullCode);
                    SendMessasge(args);
                    if (!AlterArgs.Description.Contains("已完成"))
                        AlterArgs.Description += " (已完成)";
                });
            }
            catch
            {
                ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, "修改承包合同信息失败!", eMessageGrade.Error, false, true);
            }
        }

        #endregion

        #region ContractRegeditBook

        /// <summary>
        /// 通过编辑修改承包权证错误/警告信息
        /// </summary>
        public void EditContractRegeditBookAlterDetails()
        {
            try
            {
                if (CurrentZone == null)
                {
                    ToolShowDialog.ShowBox(Workpage, "编辑权证", "请选择编辑权证所在行政区域!", eMessageGrade.Error, false, true);
                    return;
                }
                if (CurrentRegeditBook == null)
                {
                    ToolShowDialog.ShowBox(Workpage, "编辑权证", "请选择待编辑的权证!", eMessageGrade.Error, false, true);
                    return;
                }
                ContractConcord concord = CurListConcord.Find(c => c.ID == CurrentRegeditBook.ID);
                Edit(concord);
            }
            catch (Exception)
            {
                ToolShowDialog.ShowBox(Workpage, SenderInfo.SenderEdit, "修改承包权证信息失败!", eMessageGrade.Error, false, true);
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="concord"></param>
        public void Edit(ContractConcord concord)
        {
            if (concord == null)
                return;
            if (CurrentRegeditBook == null)
                return;
            RegeditBookInfoPage editPage = new RegeditBookInfoPage();
            editPage.dbContext = DbContext;
            editPage.AccountLandBusiness = new AccountLandBusiness(DbContext);
            editPage.ConcordBusiness = new ConcordBusiness(DbContext);
            editPage.ContractRegeditBookBusiness = new ContractRegeditBookBusiness(DbContext);
            editPage.personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            editPage.CurrentZone = CurrentZone;
            editPage.Concord = concord;
            editPage.CurrentVp = CurrentVirtualPerson;
            editPage.CurrentRegeditBook = CurrentRegeditBook;
            bool canContinue = editPage.InitalizeContractor(false);
            editPage.Workpage = Workpage;
            Workpage.Page.ShowDialog(editPage, (b, e) =>
            {
                if (!(bool)b)
                    return;
                if (!AlterArgs.Description.Contains("已完成"))
                    AlterArgs.Description += " (已完成)";
            });
        }

        #endregion

        #endregion

        #region Helper

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            Workpage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            Workpage.Workspace.Message.Send(this, args);
        }

        #endregion
    }
}
