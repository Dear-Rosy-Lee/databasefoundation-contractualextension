using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 水利工程类别
    /// </summary>
    public enum eIrrigationType
    {
        /// <summary>
        /// 水库
        /// </summary>
        [EntityEnumName("水库", IsLanguageName = false)]
        Reservoir = 1,

        /// <summary>
        /// 山坪塘
        /// </summary>
        [EntityEnumName("山坪塘", IsLanguageName = false)]
        Mountain = 2,

        /// <summary>
        /// 石河堰
        /// </summary>
        [EntityEnumName("石河堰", IsLanguageName = false)]
        StoneWeir = 3,

        /// <summary>
        /// 提灌站
        /// </summary>
        [EntityEnumName("提灌站", IsLanguageName = false)]
        PumpStation = 4,

        /// <summary>
        /// 渠道
        /// </summary>
        [EntityEnumName("渠道", IsLanguageName = false)]
        Channel = 5,

        /// <summary>
        /// 蓄水池
        /// </summary>
        [EntityEnumName("蓄水池", IsLanguageName = false)]
        WaterReservoir = 6,

        /// <summary>
        /// 节水工程
        /// </summary>
        [EntityEnumName("节水工程", IsLanguageName = false)]
        Conservation = 7,

        /// <summary>
        /// 提防工程
        /// </summary>
        [EntityEnumName("提防工程", IsLanguageName = false)]
        BewareProject = 8,

        /// <summary>
        /// 供水工程
        /// </summary>
        [EntityEnumName("供水工程", IsLanguageName = false)]
        WaterSupply = 9,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        UnKnow = 900,

    }
}
