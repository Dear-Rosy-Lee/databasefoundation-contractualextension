using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地权属类型
    /// </summary>
    public enum eLandPropertyRightType
    {
        /// <summary>
        /// 集体土地
        /// </summary>
        [EntityEnumName("集体土地", IsLanguageName = false)]
        CollectiveLand = 1,

        /// <summary>
        /// 集体建设用地
        /// </summary>
        [EntityEnumName("集体建设用地", IsLanguageName = false)]
        ConstructionLand = 2,

        /// <summary>
        /// 农村宅基地
        /// </summary>
        [EntityEnumName("农村宅基地", IsLanguageName = false)]
        HomeSteadLand = 3,

        /// <summary>
        /// 集体农用地
        /// </summary>
        [EntityEnumName("集体农用地", IsLanguageName = false)]
        AgricultureLand = 4,

        /// <summary>
        /// 房屋
        /// </summary>
        [EntityEnumName("房屋", IsLanguageName = false)]
        HouseEntity = 5,

        /// <summary>
        /// 林地
        /// </summary>
        [EntityEnumName("林地", IsLanguageName = false)]
        Wood = 6,

        /// <summary>
        /// 水系水利
        /// </summary>
        [EntityEnumName("水系水利", IsLanguageName = false)]
        Irrigation = 7,

        /// <summary>
        /// 其它
        /// </summary>
        [EntityEnumName("其它", IsLanguageName = false)]
        Other = 20,
    };
}
