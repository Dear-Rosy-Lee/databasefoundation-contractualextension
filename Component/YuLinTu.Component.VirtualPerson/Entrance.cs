/*
 * (C) 2024 鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Windows;

namespace YuLinTu.Component.VirtualPerson
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
            RegisterWorkspaceContext<WorkspaceContext>();
            RegisterWorkstationContext<VirtualPersonMessageContext>();
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyImportDefine));
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyOutputDefine));
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyDefine));
            RegisterWorkstationContext<AccountLandMessageContext>();
        }

        /// <summary>
        /// 地域初始化完成
        /// </summary>
        [MessageHandler(Name = ZoneMessage.ZONE_UPDATE_COMPLETE)]
        private void OnInstallZoneComplate(object sender, ModuleMsgArgs arg)
        {
            IDbContext dbContext = arg.Datasource;
            IWorkpage page = sender as IWorkpage;
            MultiObjectArg multiObject = arg.Parameter as MultiObjectArg;
            if (page == null || multiObject == null)
            {
                return;
            }
            ZoneDataItem zdiOld = multiObject.ParameterA as ZoneDataItem;
            ZoneDataItem zdiNew = multiObject.ParameterB as ZoneDataItem;
            if (zdiOld == null || zdiNew == null)
            {
                return;
            }
            List<Zone> newZoneList = new List<Zone>();
            List<Zone> oldZoneList = new List<Zone>();
            GetZoneList(zdiOld as ZoneDataItem, oldZoneList);
            GetZoneList(zdiNew as ZoneDataItem, newZoneList);

            //var ListPerson = dbContext.CreateQuery<LandVirtualPerson>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode)).ToList();
            var cbfquery = dbContext.CreateQuery<LandVirtualPerson>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode));

            ContainerFactory factory = new ContainerFactory(dbContext);
            var virtualPersonRep = factory.CreateRepository<IVirtualPersonRepository<LandVirtualPerson>>();

            var uplist = new List<LandVirtualPerson>();
            cbfquery.ForEach((i, p, x) =>
            {
                if (string.IsNullOrEmpty(x.OldVirtualCode))
                    x.OldVirtualCode = x.ZoneCode.PadRight(14, '0') + x.FamilyNumber.PadLeft(4, '0');
                x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                uplist.Add(x);
                if (uplist.Count == 1500)
                {
                    virtualPersonRep.UpdateListZoneCode(uplist);
                    virtualPersonRep.SaveChanges();
                    uplist.Clear();
                }
                return true;
            });

            if (uplist.Count > 0)
            {
                virtualPersonRep.UpdateListZoneCode(uplist);
                virtualPersonRep.SaveChanges();
            }
            UpdateAllLand(dbContext, zdiOld, zdiNew);
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

        /// <summary>
        /// 更新地块，原本功能在 YuLinTu.Component.ContractAccount中，传递的新旧地域好像有问题，移动到这里
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="zdiOld"></param>
        /// <param name="zdiNew"></param>
        private void UpdateAllLand(IDbContext dbContext, ZoneDataItem zdiOld, ZoneDataItem zdiNew)
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            var config = (section.Settings);
            var ZoneDefine = config.Clone() as ZoneDefine;
            List<Zone> newZoneList = new List<Zone>();
            List<Zone> oldZoneList = new List<Zone>();

            ContainerFactory factory = new ContainerFactory(dbContext);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            var dkquery = dbContext.CreateQuery<ContractLand>().Where(x => x.ZoneCode.Equals(zdiOld.FullCode));//.ToList();
            var uplist = new List<ContractLand>();
            dkquery.ForEach((i, p, x) =>
            {
                if (ZoneDefine.SyncCode == true)
                {
                    x.LandNumber = zdiNew.FullCode + x.LandNumber.Substring(zdiNew.FullCode.Length);
                    x.CadastralNumber = string.IsNullOrEmpty(x.CadastralNumber) ? "" : zdiNew.FullCode + x.CadastralNumber.Substring(zdiNew.FullCode.Length);
                }
                x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                x.SenderCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                uplist.Add(x);
                if (uplist.Count == 1500)
                {
                    landRep.UpdateListZoneCode(uplist);
                    landRep.SaveChanges();
                    uplist.Clear();
                }
                return true;
            });
            if (uplist.Count > 0)
            {
                landRep.UpdateListZoneCode(uplist);
                landRep.SaveChanges();
            }

            UpdateBelongLand(dbContext, zdiOld, zdiNew);
        }


        /// <summary>
        /// 更新确股地块信息
        /// </summary>
        /// <param name="dbContext"></param>
        private void UpdateBelongLand(IDbContext dbContext, ZoneDataItem zdiOld, ZoneDataItem zdiNew)
        {
            var belongRelationStation = dbContext.CreateBelongRelationWorkStation();
            var landList = belongRelationStation.GetdDataByZoneCode(zdiOld.FullCode, eLevelOption.Self);
            if (landList.Count == 0)
                return;
            var query = dbContext.CreateQuery<BelongRelation>();
            try
            {
                dbContext.BeginTransaction();
                foreach (var item in landList)
                {
                    item.ZoneCode = zdiNew.FullCode;
                    query.Where(t => t.ID == item.ID).Update(item).Save();
                }
                dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                throw ex;
            }
        }

        #endregion Methods
    }
}