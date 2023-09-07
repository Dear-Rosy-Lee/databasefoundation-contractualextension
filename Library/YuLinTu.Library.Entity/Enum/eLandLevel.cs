using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地级别
    /// </summary>
    public enum eLandLevel
    {
        /// <summary>
        /// 综合一级
        /// </summary>
        [EntityEnumName("key41161", IsLanguageName = true)]
        ComplexOneLevel = 101,

        /// <summary>
        ///  综合二级
        /// </summary>
        [EntityEnumName("key41162", IsLanguageName = true)]
        ComplexTwoLevel = 102,

        /// <summary>
        /// 综合三级
        /// </summary>
        [EntityEnumName("key41163", IsLanguageName = true)]
        ComplexThreeLevel = 103,

        /// <summary>
        /// 综合四级
        /// </summary>
        [EntityEnumName("key41164", IsLanguageName = true)]
        ComplexFourLevel = 104,

        /// <summary>
        /// 综合五级
        /// </summary>
        [EntityEnumName("key41165", IsLanguageName = true)]
        ComplexFiveLevel = 105,

        /// <summary>
        /// 综合六级
        /// </summary>
        [EntityEnumName("key41166", IsLanguageName = true)]
        ComplexSixLevel = 106,

        /// <summary>
        /// 综合七级
        /// </summary>
        [EntityEnumName("key41167", IsLanguageName = true)]
        ComplexSevenLevel = 107,

        /// <summary>
        /// 综合八级
        /// </summary>
        [EntityEnumName("key41168", IsLanguageName = true)]
        ComplexEightLevel = 108,

        /// <summary>
        /// 综合九级
        /// </summary>
        [EntityEnumName("key41169", IsLanguageName = true)]
        ComplexNineLevel = 109,

        /// <summary>
        /// 综合十级
        /// </summary>
        [EntityEnumName("key41170", IsLanguageName = true)]
        ComplexTenLevel = 110,

        /// <summary>
        /// 综合十一级
        /// </summary>
        [EntityEnumName("key41171", IsLanguageName = true)]
        ComplexElevenLevel = 111,

        /// <summary>
        /// 综合十二级
        /// </summary>
        [EntityEnumName("key41172", IsLanguageName = true)]
        ComplexTwelveLevel = 112,

        /// <summary>
        /// 商业一级
        /// </summary>
        [EntityEnumName("key41173", IsLanguageName = true)]
        BussinessOneLevel = 201,

        /// <summary>
        /// 商业二级
        /// </summary>
        [EntityEnumName("key41174", IsLanguageName = true)]
        BussinessTwoLevel = 202,

        /// <summary>
        /// 商业三级
        /// </summary>
        [EntityEnumName("key41175", IsLanguageName = true)]
        BussinessThreeLevel = 203,

        /// <summary>
        /// 商业四级
        /// </summary>
        [EntityEnumName("key41176", IsLanguageName = true)]
        BussinessFourLevel = 204,

        /// <summary>
        /// 商业五级
        /// </summary>
        [EntityEnumName("key41177", IsLanguageName = true)]
        BussinessFiveLevel = 205,

        /// <summary>
        /// 商业六级
        /// </summary>
        [EntityEnumName("key41178", IsLanguageName = true)]
        BussinessSixLevel = 206,

        /// <summary>
        /// 商业七级
        /// </summary>
        [EntityEnumName("key41179", IsLanguageName = true)]
        BussinessSevenLevel = 207,

        /// <summary>
        /// 商业八级
        /// </summary>
        [EntityEnumName("key41180", IsLanguageName = true)]
        BussinessEightLevel = 208,

        /// <summary>
        /// 商业九级
        /// </summary>
        [EntityEnumName("key41181", IsLanguageName = true)]
        BussinessNineLevel = 209,

        /// <summary>
        /// 商业十级
        /// </summary>
        [EntityEnumName("key41182", IsLanguageName = true)]
        BussinessTenLevel = 210,

        /// <summary>
        /// 商业十一级
        /// </summary>
        [EntityEnumName("key41183", IsLanguageName = true)]
        BussinessElevenLevel = 211,

        /// <summary>
        /// 商业十二级
        /// </summary>
        [EntityEnumName("key41184", IsLanguageName = true)]
        BussinessTwelveLevel = 212,

        /// <summary>
        /// 住宅一级
        /// </summary>
        [EntityEnumName("key41185", IsLanguageName = true)]
        LiveOneLevel = 301,

        /// <summary>
        /// 住宅二级
        /// </summary>
        [EntityEnumName("key41186", IsLanguageName = true)]
        LiveTwoLevel = 302,

        /// <summary>
        /// 住宅三级
        /// </summary>
        [EntityEnumName("key41187", IsLanguageName = true)]
        LiveThreeLevel = 303,

        /// <summary>
        /// 住宅四级
        /// </summary>
        [EntityEnumName("key41188", IsLanguageName = true)]
        LiveFourLevel = 304,

        /// <summary>
        /// 住宅五级
        /// </summary>
        [EntityEnumName("key41189", IsLanguageName = true)]
        LiveFiveLevel = 305,

        /// <summary>
        /// 住宅六级
        /// </summary>
        [EntityEnumName("key41190", IsLanguageName = true)]
        LiveSixLevel = 306,

        /// <summary>
        /// 住宅七级
        /// </summary>
        [EntityEnumName("key41191", IsLanguageName = true)]
        LiveSevenLevel = 307,

        /// <summary>
        /// 住宅八级
        /// </summary>
        [EntityEnumName("key41192", IsLanguageName = true)]
        LiveEightLevel = 308,

        /// <summary>
        /// 住宅九级
        /// </summary>
        [EntityEnumName("key41193", IsLanguageName = true)]
        LiveNineLevel = 309,

        /// <summary>
        /// 住宅十级
        /// </summary>
        [EntityEnumName("key41194", IsLanguageName = true)]
        LiveTenLevel = 310,

        /// <summary>
        /// 住宅十一级
        /// </summary>
        [EntityEnumName("key41195", IsLanguageName = true)]
        LiveElevenLevel = 311,

        /// <summary>
        /// 住宅十二级
        /// </summary>
        [EntityEnumName("key41196", IsLanguageName = true)]
        LiveTwelveLevel = 312,

        /// <summary>
        /// 工业一级
        /// </summary>
        [EntityEnumName("key41197", IsLanguageName = true)]
        IndustryOneLevel = 401,

        /// <summary>
        /// 工业二级
        /// </summary>
        [EntityEnumName("key41198", IsLanguageName = true)]
        IndustryTwoLevel = 402,

        /// <summary>
        /// 工业三级
        /// </summary>
        [EntityEnumName("key41199", IsLanguageName = true)]
        IndustryThreeLevel = 403,

        /// <summary>
        /// 工业四级
        /// </summary>
        [EntityEnumName("key41200", IsLanguageName = true)]
        IndustryFourLevel = 404,

        /// <summary>
        /// 工业五级
        /// </summary>
        [EntityEnumName("key41201", IsLanguageName = true)]
        IndustryFiveLevel = 405,

        /// <summary>
        /// 工业六级
        /// </summary>
        [EntityEnumName("key41202", IsLanguageName = true)]
        IndustrySixLevel = 406,

        /// <summary>
        /// 工业七级
        /// </summary>
        [EntityEnumName("key41203", IsLanguageName = true)]
        IndustrySevenLevel = 407,

        /// <summary>
        /// 工业八级
        /// </summary>
        [EntityEnumName("key41204", IsLanguageName = true)]
        IndustryEightLevel = 408,

        /// <summary>
        /// 工业九级
        /// </summary>
        [EntityEnumName("key41205", IsLanguageName = true)]
        IndustryNineLevel = 409,

        /// <summary>
        /// 工业十级
        /// </summary>
        [EntityEnumName("key41206", IsLanguageName = true)]
        IndustryTenLevel = 410,

        /// <summary>
        /// 工业十一级
        /// </summary>
        [EntityEnumName("key41207", IsLanguageName = true)]
        IndustryElevenLevel = 411,

        /// <summary>
        /// 工业十二级代码
        /// </summary>
        [EntityEnumName("key41208", IsLanguageName = true)]
        IndustryTwelveLevel = 412,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("key41209", IsLanguageName = true)]
        UnKnow = 900,
    }
}
