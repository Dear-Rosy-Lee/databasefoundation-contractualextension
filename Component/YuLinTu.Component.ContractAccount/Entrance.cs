/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Business;
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Method

        /// <summary>
        /// 重写注册工作空间方法
        /// 应用程序上下文，其中注册了一个工作空间上下文
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Diagrams;component/Resources/Res.xaml") });
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

            ContainerFactory factory = new ContainerFactory(dbContext);
            var landRep = factory.CreateRepository<IContractLandRepository>();
            //var ContractLands = dbContext.CreateQuery<ContractLand>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode)).ToList();
            var dkquery = dbContext.CreateQuery<ContractLand>().Where(x => x.ZoneCode.StartsWith(zdiOld.FullCode));//.ToList();
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

            //if (ZoneDefine.SyncCode == true)
            //{
            //    ContractLands.ForEach(x =>
            //    {
            //        x.LandNumber = zdiNew.FullCode + x.LandNumber.Substring(zdiNew.FullCode.Length);
            //        x.CadastralNumber = x.CadastralNumber == null ? "" : zdiNew.FullCode + x.CadastralNumber.Substring(zdiNew.FullCode.Length);
            //        x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //        x.SenderCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //    });
            //    UpContractLand(dbContext, ContractLands);
            //}
            //else
            //{
            //    ContractLands.ForEach(x =>
            //    {
            //        x.ZoneCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //        x.SenderCode = zdiNew.FullCode + x.ZoneCode.Substring(zdiNew.FullCode.Length);
            //    });
            //    UpContractLand(dbContext, ContractLands);
            //}
        }

        #endregion
    }
}
