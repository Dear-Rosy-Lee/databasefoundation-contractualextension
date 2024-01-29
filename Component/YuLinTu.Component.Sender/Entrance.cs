/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Sender
{
    /// <summary>
    /// 插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Properties

        public ZoneDefine ZoneDefine { get; set; }

        public List<LandVirtualPerson> ListPerson { get; set; }

        public List<ContractLand> ContractLands { get; set; }
        public List<ContractConcord> ContractConcords { get; set; }
        public List<ContractRegeditBook> ContractRegeditBooks { get; set; }
        public List<CollectivityTissue> Tissues { get; set; }

        #endregion Properties

        #region Fields

        private ZoneDefine config;

        private SettingsProfileCenter systemCenter;

        #endregion Fields

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
            systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            var tissueStation = dbContext.CreateCollectivityTissueWorkStation();
            config = (section.Settings);
            ZoneDefine = config.Clone() as ZoneDefine;
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
            if (zdiNew.Level == eZoneLevel.Group)
            {
                Tissues = dbContext.CreateQuery<CollectivityTissue>().Where(x => x.ZoneCode == zdiOld.FullCode).ToList();
                Tissues.ForEach(x => x.ZoneCode = zdiNew.FullCode);
                ContractConcords = dbContext.CreateQuery<ContractConcord>().Where(x => x.ZoneCode == zdiOld.FullCode).ToList();
                ContractConcords.ForEach(x => x.ZoneCode = zdiNew.FullCode);
                ContractLands = dbContext.CreateQuery<ContractLand>().Where(x => x.LocationCode == zdiOld.FullCode).ToList();
                ContractLands.ForEach(x =>
                {
                    x.LocationCode = zdiNew.FullCode;
                    var Tissue = new CollectivityTissue();
                    Tissue = tissueStation.Get(zdiNew.FullCode);
                    x.ZoneCode = Tissue.Code;
                });
                ContractRegeditBooks = dbContext.CreateQuery<ContractRegeditBook>().Where(x => x.ZoneCode == zdiOld.FullCode).ToList();
                ContractRegeditBooks.ForEach(x => x.ZoneCode = zdiNew.FullCode);
                ListPerson = dbContext.CreateQuery<LandVirtualPerson>().Where(x => x.ZoneCode == zdiOld.FullCode).ToList();
                ListPerson.ForEach(x => x.ZoneCode = zdiNew.FullCode);
                UpTissues(dbContext);
                UpContractConcord(dbContext);
                UpRegeditBook(dbContext);
                UpContractLand(dbContext);
                UpVirtualPerson(dbContext);
            }
            else
            {
                dbContext.ExecuteBySQL($"UPDATE JCSJ_FBF SET XZDYBM = REPLACE(XZDYBM, '{zdiOld.FullCode}', '{zdiNew.FullCode}') WHERE XZDYBM LIKE '%{zdiOld.FullCode}%';");

                dbContext.ExecuteBySQL($"UPDATE ZD_CBD SET ZLDM = REPLACE(ZLDM, '{zdiOld.FullCode}', '{zdiNew.FullCode}') WHERE ZLDM LIKE '%{zdiOld.FullCode}%';");

                dbContext.ExecuteBySQL($"UPDATE CBJYQ_HT SET DYBM = REPLACE(DYBM, '{zdiOld.FullCode}', '{zdiNew.FullCode}') WHERE DYBM LIKE '%{zdiOld.FullCode}%';");

                dbContext.ExecuteBySQL($"UPDATE CBJYQ_QZ SET DYDM = REPLACE(DYDM, '{zdiOld.FullCode}', '{zdiNew.FullCode}') WHERE DYDM LIKE '%{zdiOld.FullCode}%';");

                dbContext.ExecuteBySQL($"UPDATE QLR_CBF SET DYBM = REPLACE(DYBM, '{zdiOld.FullCode}', '{zdiNew.FullCode}') WHERE DYBM LIKE '%{zdiOld.FullCode}%';");
            }
        }

        private void UpVirtualPerson(IDbContext dbContext)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var virtualPersonRep = factory.CreateRepository<IVirtualPersonRepository<LandVirtualPerson>>();
            foreach (var entity in ListPerson)
            {
                virtualPersonRep.Update(entity);
            }
            virtualPersonRep.SaveChanges();
        }

        private void UpTissues(IDbContext dbContext)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var tissueRep = factory.CreateRepository<ICollectivityTissueRepository>();
            foreach (var entity in Tissues)
            {
                tissueRep.Update(entity);
            }
            tissueRep.SaveChanges();
        }

        private void UpContractConcord(IDbContext dbContext)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var concordRep = factory.CreateRepository<IContractConcordRepository>();

            foreach (var entity in ContractConcords)
            {
                concordRep.Update(entity);
            }
            concordRep.SaveChanges();
        }

        private void UpRegeditBook(IDbContext dbContext)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var regeditBookRep = factory.CreateRepository<IContractRegeditBookRepository>();
            foreach (var entity in ContractRegeditBooks)
            {
                regeditBookRep.Update(entity);
            }
            regeditBookRep.SaveChanges();
        }

        private void UpContractLand(IDbContext dbContext)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            foreach (var entity in ContractLands)
            {
                landRep.Update(entity);
            }
            landRep.SaveChanges();
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