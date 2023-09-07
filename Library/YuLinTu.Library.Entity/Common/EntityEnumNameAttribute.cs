using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    internal class EntityEnumNameAttribute : EnumNameAttribute
    {
        static EntityEnumNameAttribute()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eAppBuildType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eAppPersont);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryLineCategory);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryLinePosition);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryNatrueType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBoundaryPointType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBuildFashion);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eBuildHouseType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eConstructMode);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eConstructType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eConsturctionPurpose);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eContracterType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eCredentialsType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eGender);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eHavingLandType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eHouseStructure);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eInvestScale);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandMarkType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPurposeType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandSlopelLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandUsePersonType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandUseRightType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eManageType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eMapSelect);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eNation);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_ePlantProtectType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eStatus);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTissueMemberType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTissueType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eTransferType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eVirtualPersonType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eVirtualType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eZoneLevel);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_Zone);
        }

        public EntityEnumNameAttribute(string description)
            : base(description)
        {
        }
    }
}