using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方工具类
    /// </summary>
    public static class TissueHelper
    {
        /// <summary>
        /// 根据配置设置发包方名称
        /// </summary>
        /// <param name="systemSet"></param>
        /// <param name="tissue"></param>
        /// <returns></returns>
        public static string ReplaceSenderName(SystemSetDefine systemSet, CollectivityTissue tissue)
        {
            if (tissue == null) throw new Exception("获取发包方失败！");
            if(systemSet == null) systemSet = GetSystemSet();
            if (string.IsNullOrWhiteSpace(systemSet.SenderNameReplacement))
                return tissue.Name;
            return string.Format(systemSet.SenderNameReplacement, tissue.Name, tissue.NewName);
        }

        public static string ReplaceSenderName(CollectivityTissue tissue)
        {
            var systemSet = GetSystemSet();
            return ReplaceSenderName(systemSet, tissue);
        }

        public static string ReplaceSenderName(SystemSetDefine systemSet, Zone zone)
        {
            var tissue = GetTissueByZone(zone);
            return ReplaceSenderName(systemSet, tissue);
        }

        public static string ReplaceSenderName(Zone zone)
        {
            var tissue = GetTissueByZone(zone);
            var systemSet = GetSystemSet();
            return ReplaceSenderName(systemSet, tissue);
        }

        private static CollectivityTissue GetTissueByZone(Zone zone)
        {
            if (zone == null) throw new Exception("获取行政地域失败！");
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            return dbContext.CreateSenderWorkStation().GetByCode(zone.FullCode);
        }

        private static SystemSetDefine GetSystemSet()
        {
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<SystemSetDefine>();
            var section = profile.GetSection<SystemSetDefine>();
            return section.Settings as SystemSetDefine;
        }
    }
}
