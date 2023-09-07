using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    partial class CadastralZone
    {
        #region Properties

        public static CadastralZone China
        {
            get
            {
                if (zoneChina == null)
                {
                    zoneChina = new CadastralZone();
                    zoneChina.FullCode = ZONE_STATE_CODE;
                    zoneChina.Code = ZONE_STATE_CODE;
                    zoneChina.UpLevelCode = string.Empty;
                    zoneChina.FullName = LanguageAttribute.GetLanguage("lang46000");
                    zoneChina.Level = eZoneLevel.State;
                    zoneChina.Name = LanguageAttribute.GetLanguage("lang46000");
                }
                return zoneChina;
            }
        }

        private static CadastralZone zoneChina;

        #endregion

        #region Methods

        public static CadastralZone Find(CadastralList<Zone> zones, string codeZone)
        {
            if (zones == null)
                return null;

            return zones.Find((zone =>
            {
                if (zone.FullCode == codeZone)
                    return true;

                return false;
            }));
            throw new NotImplementedException();
        }

        public static CadastralList<Zone> FindChildren(CadastralList<Zone> zones, string codeZone)
        {
            if (zones == null)
                return null;

            CadastralList<Zone> children = new CadastralList<Zone>();

            zones.ForEach((zone =>
            {
                if (zone.UpLevelCode == codeZone)
                    children.Add(zone);
            }));

            return children;
            throw new NotImplementedException();
        }

        #endregion
    }
}
