/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Sender
{
    /// <summary>
    /// 插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
            RegisterWorkspaceContext<WorkspaceContext>();
            RegisterWorkstationContext<SenderMessageContext>();
        }

        /// <summary>
        /// 地域初始化完成
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_UPDATE_COMPLETE)]
        private void OnInstallZoneComplate(object sender, ModuleMsgArgs arg)
        {
            IDbContext dbContext = arg.Datasource;
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            var config = (section.Settings);
            var ZoneDefine = config.Clone() as ZoneDefine;
            IWorkpage page = sender as IWorkpage;
            MultiObjectArg multiObject = arg.Parameter as MultiObjectArg;
            if (page == null || multiObject == null)
            {
                return;
            }
            ZoneDataItem zdiOld = multiObject.ParameterB as ZoneDataItem;
            ZoneDataItem zdiNew = multiObject.ParameterA as ZoneDataItem;
            if (zdiOld == null || zdiNew == null)
            {
                return;
            }
            List<Zone> newZoneList = new List<Zone>();
            List<Zone> oldZoneList = new List<Zone>();
            GetZoneList(zdiOld as ZoneDataItem, oldZoneList);
            GetZoneList(zdiNew as ZoneDataItem, newZoneList);
            MultiObjectArg paraMeter = new MultiObjectArg() { ParameterA = newZoneList, ParameterB = oldZoneList };
            var Tissues = dbContext.CreateQuery<CollectivityTissue>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode)).ToList();
            if (ZoneDefine.SyncCode == true)
            {
                if (zdiNew.Name != zdiOld.Name)
                {
                    //县级包括县以下的修改名称时需要提示陈泽林20161009
                    if (zdiNew.Level <= eZoneLevel.County)
                    {
                        string content = "是否修改默认发包方名称?";
                        TabMessageBoxDialog messagePage = new TabMessageBoxDialog()
                        {
                            Header = SenderInfo.SenderEdit,
                            Message = content,
                            MessageGrade = eMessageGrade.Warn,
                            CancelButtonText = "取消",
                        };
                        page.Page.ShowMessageBox(messagePage, (b, r) =>
                        {
                            if (zdiNew.FullCode != zdiOld.FullCode || (bool)b)
                            {
                                paraMeter.ParameterC = (bool)b;
                                SendUpMessage(sender, arg.Datasource, paraMeter);
                            }
                        });
                    }
                }
                else
                {
                    SendUpMessage(sender, arg.Datasource, paraMeter);
                }
            }
            else
            {
                Tissues.ForEach(x =>
                {
                    x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                });
                UpTissues(dbContext, Tissues);
            }
        }

        private void UpTissues(IDbContext dbContext, List<CollectivityTissue> Tissues)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var tissueRep = factory.CreateRepository<ICollectivityTissueRepository>();
            foreach (var entity in Tissues)
            {
                tissueRep.Update(entity);
            }
            tissueRep.SaveChanges();
        }

        /// <summary>
        /// 发送更新消息
        /// </summary>
        private void SendUpMessage(object sender, IDbContext db, MultiObjectArg arg)
        {
            ModuleMsgArgs sArg = new ModuleMsgArgs();
            sArg.Datasource = db;
            sArg.Name = SenderMessage.SENDER_UPDATEZONE;
            sArg.Parameter = arg;
            TheBns.Current.Message.Send(sender, sArg);
        }

        /// <summary>
        /// 列表中获取集合
        /// </summary>
        private void GetZoneList(ZoneDataItem item, List<Zone> list)
        {
            if (item == null)
            {
                return;
            }
            if (item.Children != null && item.Children.Count > 0)
            {
                foreach (var c in item.Children)
                {
                    GetZoneList(c, list);
                }
            }
            list.Add(item.ConvertTo<Zone>());
        }

        #endregion Methods
    }
}