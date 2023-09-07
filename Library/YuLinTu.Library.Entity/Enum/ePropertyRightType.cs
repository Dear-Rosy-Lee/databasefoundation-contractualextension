/*
 * (C) 2012-2016 鱼鳞图公司版权所有，保留所有权利
*/
using System;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 枚举权属类型。
    /// </summary>
    [Serializable]
    public enum ePropertyRightType
    {
        /// <summary>
        /// 承包经营权。
        /// </summary>
        [EntityEnumName("承包经营权", IsLanguageName = false)]
        ContractLand = 0,

        /// <summary>
        /// 集体土地所有权。
        /// </summary>
        [EntityEnumName("集体土地所有权", IsLanguageName = false)]
        CollectiveLand = 1,

        /// <summary>
        /// 集体建设用地使用权。
        /// </summary>
        [EntityEnumName("集体建设用地使用权", IsLanguageName = false)]
        ConstructionLand = 2,

        /// <summary>
        /// 宅基地使用权。
        /// </summary>
        [EntityEnumName("宅基地使用权", IsLanguageName = false)]
        HomeStead = 3,

        /// <summary>
        /// 房屋所有权。
        /// </summary>
        [EntityEnumName("房屋所有权", IsLanguageName = false)]
        Housing = 4
    }
}