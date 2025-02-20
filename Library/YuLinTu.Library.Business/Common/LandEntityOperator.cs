/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 土地类型
    /// </summary>
    public enum LanderType
    {
        [EnumName("集体土地所有权", IsLanguageName = false)]
        CollectiveLand = 1,//集体土地

        [EnumName("建设用地使用权", IsLanguageName = false)]
        ConstructionLand = 2,//集体建设用地

        [EnumName("宅基地使用权", IsLanguageName = false)]
        HomeSteadLand = 3,//农村宅基地

        [EnumName("土地承包经营权", IsLanguageName = false)]
        AgricultureLand = 4,//土地承包经营权

        [EnumName("林权", IsLanguageName = false)]
        WoodLand = 6,//林地

        [EnumName("草地承包权", IsLanguageName = false)]
        GrassLand = 7,//草地

        [EnumName("承包合同", IsLanguageName = false)]
        LandContract = 8,//承包合同

        [EnumName("承包权证", IsLanguageName = false)]
        LandWarrant = 9,//承包权证

        [EnumName("发包方", IsLanguageName = false)]
        LandSender = 10,//发包方

        [EnumName("承包方", IsLanguageName = false)]
        VirtualPerson = 11,//承包方

        [EnumName("水利工程产权", IsLanguageName = false)]
        Irrigation = 12,//水利工程

        [EnumName("房屋所有权", IsLanguageName = false)]
        House = 15,//房屋

        [EnumName("二轮台账", IsLanguageName = false)]
        SecondTable = 16,//二轮台账

        [EnumName("所有地块", IsLanguageName = false)]
        AllLand = 20,//所有地块

        [EnumName("其它", IsLanguageName = false)]
        Other = 30,//其它

        [EnumName("未知", IsLanguageName = false)]
        UnKnown = 40,//未知

    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum LandEntityType
    {
        ExportAgricultureLandBoundayTable = 1,//导出农村土地承包经营权界址线成果表
    }
}
