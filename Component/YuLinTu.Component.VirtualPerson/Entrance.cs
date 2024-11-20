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

            var ListPerson = dbContext.CreateQuery<LandVirtualPerson>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode)).ToList();
            var cbfquery = dbContext.CreateQuery<LandVirtualPerson>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode));

            var uplist = new List<LandVirtualPerson>();
            cbfquery.ForEach((i, p, x) =>
            {
                if (string.IsNullOrEmpty(x.OldVirtualCode))
                    x.OldVirtualCode = x.ZoneCode.PadRight(14, '0') + x.FamilyNumber.PadLeft(4, '0');
                x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                uplist.Add(x);
                if (uplist.Count == 1000)
                {
                    UpVirtualPerson(dbContext, uplist);
                    uplist.Clear();
                }
                return true;
            });


            //ListPerson.ForEach(x =>
            //{
            //    if (string.IsNullOrEmpty(x.OldVirtualCode))
            //        x.OldVirtualCode = x.ZoneCode.PadRight(14, '0') + x.FamilyNumber.PadLeft(4, '0');
            //    x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //});
            if (uplist.Count > 0)
                UpVirtualPerson(dbContext, uplist);
        }

        private void UpVirtualPerson(IDbContext dbContext, List<LandVirtualPerson> ListPerson)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var virtualPersonRep = factory.CreateRepository<IVirtualPersonRepository<LandVirtualPerson>>();
            foreach (var entity in ListPerson)
            {
                virtualPersonRep.Update(entity, true);
            }
            virtualPersonRep.SaveChanges();
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