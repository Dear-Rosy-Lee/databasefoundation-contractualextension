/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Component.ContractRegeditBook
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
            //TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyImportDefine));
            //TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyOutputDefine));
            //TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyDefine));
            //RegisterWorkstationContext<AccountLandMessageContext>();
            RegisterWorkstationContext<ContractRegeditBookMessageContext>();
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

            var uplist = new List<Library.Entity.ContractRegeditBook>();
            //var ContractRegeditBooks = dbContext.CreateQuery<Library.Entity.ContractRegeditBook>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode)).ToList();
            var djbquery = dbContext.CreateQuery<Library.Entity.ContractRegeditBook>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode));

            //if (ZoneDefine.SyncCode == true)
            //{
            djbquery.ForEach((i, p, x) =>
            {
                if (ZoneDefine.SyncCode == true)
                {
                    x.Number = zdiNew.FullCode + x.Number.Substring(zdiNew.FullCode.Length);
                    x.RegeditNumber = zdiNew.FullCode + x.Number.Substring(zdiNew.FullCode.Length);
                }
                x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
                uplist.Add(x);
                if (uplist.Count == 1000)
                {
                    UpRegeditBook(dbContext, uplist);
                    uplist.Clear();
                }
                return true;
            });

            //    ContractRegeditBooks.ForEach(x =>
            //{
            //    x.Number = zdiNew.FullCode + x.Number.Substring(zdiNew.FullCode.Length);
            //    x.RegeditNumber = zdiNew.FullCode + x.Number.Substring(zdiNew.FullCode.Length);
            //    x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //});
            if (uplist.Count > 0)
                UpRegeditBook(dbContext, uplist);
            //}
            //else
            //{
            //    ContractRegeditBooks.ForEach(x =>
            //    {
            //        x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //    });
            //    UpRegeditBook(dbContext, ContractRegeditBooks);
            //}
        }

        private void UpRegeditBook(IDbContext dbContext, List<Library.Entity.ContractRegeditBook> ContractRegeditBooks)
        {
            ContainerFactory factory = new ContainerFactory(dbContext);
            var regeditBookRep = factory.CreateRepository<IContractRegeditBookRepository>();
            foreach (var entity in ContractRegeditBooks)
            {
                regeditBookRep.Update(entity,true);
            }
            regeditBookRep.SaveChanges();
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
        #endregion

    }
}
