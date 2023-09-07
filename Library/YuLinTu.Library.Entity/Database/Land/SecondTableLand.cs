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
    /// 二轮承包地块
    /// </summary>
    [Serializable]
    [DataTable("ZD_ELTZ")]
    public class SecondTableLand : ContractLand
    {
        #region Ctor

        static SecondTableLand()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eConstructType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_ePlantProtectType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandSlopelLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPurposeType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eManageType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTransferType);
        }

        #endregion
    }

    /// <summary>
    /// 二轮承包地块集合
    /// </summary>
    [Serializable]
    public class SecondTableLandCollection : List<SecondTableLand>
    {
    }
}