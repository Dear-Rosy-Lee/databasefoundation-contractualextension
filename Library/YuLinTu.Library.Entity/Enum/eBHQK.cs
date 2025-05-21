using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    public enum eBHQK
    {
        /// <summary>
        /// 人地信息均不变
        /// </summary>
        [EntityEnumName("人地信息均不变")]
        [EnumName("人地信息均不变")]
        RDXXJBB = 1101,

        /// <summary>
        /// 仅成员信息变化
        /// </summary>
        [EntityEnumName("仅成员信息变化")]
        [EnumName("仅成员信息变化")]
        CYXXBH = 1102,

        /// <summary>
        /// 其他直接顺延
        /// </summary>
        [EntityEnumName("其他直接顺延")]
        [EnumName("其他直接顺延")]
        QTZZSY = 1103,

        /// <summary>
        /// 承包方依法分立
        /// </summary>
        [EntityEnumName("承包方依法分立")]
        [EnumName("承包方依法分立")]
        CBFYFFL = 1201,

        /// <summary>
        /// 承包方依法合并
        /// </summary>
        [EntityEnumName("承包方依法合并")]
        [EnumName("承包方依法合并")]
        CBFYFBH = 1202,

        /// <summary>
        /// 承包地被征收
        /// </summary>
        [EntityEnumName("承包地被征收")]
        [EnumName("承包地被征收")]
        CBDBZS = 1203,

        /// <summary>
        /// 自然灾害毁损
        /// </summary>
        [EntityEnumName("自然灾害毁损")]
        [EnumName("自然灾害毁损")]
        ZRZHSH = 1204,

        /// <summary>
        /// 土地承包经营权互换
        /// </summary>
        [EntityEnumName("土地承包经营权互换")]
        [EnumName("土地承包经营权互换")]
        CBJYQHH = 1205,

        /// <summary>
        /// 土地承包经营权部分转让
        /// </summary>
        [EntityEnumName("土地承包经营权部分转让")]
        [EnumName("土地承包经营权部分转让")]
        CBJYQBFZR = 1206,


        /// <summary>
        /// 用途改变
        /// </summary>
        [EntityEnumName("用途改变")]
        [EnumName("用途改变")]
        YTGB = 1207,

        /// <summary>
        /// 集体使用机动地
        /// </summary>
        [EntityEnumName("集体使用机动地")]
        [EnumName("集体使用机动地")]
        SYJDD = 1208,

        /// <summary>
        /// 退包地
        /// </summary>
        [EntityEnumName("退包地")]
        [EnumName("退包地")]
        TBD = 1209,

        /// <summary>
        /// 复垦地等补分承包地
        /// </summary>
        [EntityEnumName("复垦地等补分包地")]
        [EnumName("复垦地等补分承包地")]
        FKDDBFCBD = 2101,

        /// <summary>
        /// 其他承包地变化顺延
        /// </summary>
        [EntityEnumName("其他承包地变化顺延")]
        [EnumName("其他承包地变化顺延")]
        QTCBDBHSY = 2102,

        /// <summary>
        /// 调整增地
        /// </summary>
        [EntityEnumName("调整增地")]
        [EnumName("调整增地")]
        TZZD = 2103,

        /// <summary>
        /// 调整减地
        /// </summary>
        [EntityEnumName("调整减地")]
        [EnumName("调整减地")]
        TZJD = 2201,

        /// <summary>
        /// 调整减地—其他
        /// </summary>
        [EntityEnumName("调整减地—其他")]
        [EnumName("调整减地—其他")]
        TZJDQT = 2202,

        /// <summary>
        /// 合同注销
        /// </summary>
        [EntityEnumName("合同注销")]
        [EnumName("合同注销")]
        HTZX = 9901

    }
}
