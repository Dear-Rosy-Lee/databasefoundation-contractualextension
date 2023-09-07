// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地籍区
    /// </summary>
    [DataTable("CadastralZone")]
    [Serializable]
    public partial class CadastralZone : Zone
    {
        #region Ctor

        static CadastralZone()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eCadastralZoneLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_Zone);
        }

        #endregion

        #region Fields

        public const int ZONE_CADASTRALREGION_LENGTH = 9;//地籍区长度
        public const int ZONE_CADASTRALSUBREGION_LENGTH = 12;//地籍子区长度

        #endregion

        #region Ctor

        public CadastralZone()
        {
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
        }

        #endregion
    }
}
