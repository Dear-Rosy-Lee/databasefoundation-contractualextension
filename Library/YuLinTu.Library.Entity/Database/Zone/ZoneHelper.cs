using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    public class ZoneHelper
    {
        #region Static

        /// <summary>
        /// 中国
        /// </summary>
        public static Zone China
        {
            get
            {
                if (zoneChina == null)
                {
                    zoneChina = new Zone();
                    zoneChina.FullCode = Zone.ZONE_STATE_CODE;
                    zoneChina.Code = Zone.ZONE_STATE_CODE;
                    zoneChina.UpLevelCode = string.Empty;
                    zoneChina.FullName = LanguageAttribute.GetLanguage("lang46000");
                    zoneChina.Level = eZoneLevel.State;
                    zoneChina.Name = LanguageAttribute.GetLanguage("lang46000");
                }
                return zoneChina;
            }
        }

        private static Zone zoneChina;

        #endregion

        #region Methods

        /// <summary>
        /// 查找数据
        /// </summary>
        public static Zone Find(List<Zone> zones, string codeZone)
        {
            if (zones == null)
                return null;

            return zones.Find((zone =>
            {
                if (zone.FullCode == codeZone)
                    return true;

                return false;
            }));
        }

        /// <summary>
        /// 查找数据集合
        /// </summary>
        /// <param name="zones"></param>
        /// <param name="codeZone"></param>
        /// <returns></returns>
        public static List<Zone> FindChildren(List<Zone> zones, string codeZone)
        {
            if (zones == null)
                return null;

            List<Zone> children = new List<Zone>();

            zones.ForEach((zone =>
            {
                if (zone.UpLevelCode == codeZone && !children.Contains(zone))
                {
                    children.Add(zone);
                }
            }));

            List<Zone> zoneCollection = new List<Zone>();
            var orders = children.OrderBy(ze =>
            {
                int num = 0;
                Int32.TryParse(ze.Code, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            foreach (var zone in orders)
            {
                zoneCollection.Add(zone);
            }
            children.Clear();
            return zoneCollection;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将16位编码转化为14位
        /// </summary>
        public static string ChangeCodeShort(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode) || zoneCode.Length != 16)
            {
                return zoneCode;
            }
            return zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
        }

        /// <summary>
        /// 将14位编码转化为16位
        /// </summary>
        public static string ChangeCodeLong(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode) || zoneCode.Length != 14)
            {
                return zoneCode;
            }
            return zoneCode.Substring(0, 12) + "00" + zoneCode.Substring(12, 2);
        }
        #endregion
    }
}
