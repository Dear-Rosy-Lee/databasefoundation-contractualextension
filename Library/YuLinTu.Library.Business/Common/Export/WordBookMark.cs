using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// Word书签
    /// </summary>
    public partial class WordBookMark
    {
        //承包方
        public const string VirtualPersonName = "VirtualPersonName";//承包方姓名
        public const string VirtualPersonGender = "VirtualPersonGender";//承包方性别
        public const string VirtualPersonAge = "VirtualPersonAge";//承包方年龄
        public const string VirtualPersonBirthMonthDay = "VirtualPersonBirthMonthDay";//承包方出生日期年月
        public const string VirtualPersonBirthday = "VirtualPersonBirthday";//承包方出生日期
        public const string VirtualPersonAllBirthday = "VirtualPersonAllBirthday";//承包方出生全日期
        public const string VirtualPersonNumber = "VirtualPersonNumber";//承包方户号
        public const string VirtualPersonAllNumber = "VirtualPersonAllNumber";//承包方全户号
        public const string VirtualPersonCartType = "VirtualPersonCartType";//承包方证件类型
        public const string VirtualPersonIdentifyNumber = "VirtualPersonIdentifyNumber";//承包方身份证号码
        public const string VirtualPersonCount = "VirtualPersonCount";//承包方家庭成员个数
        public const string VirtualPersonAllocationPerson = "VirtualPersonAllocationPerson";//承包方实际分配人数
        public const string VirtualPersonComment = "VirtualPersonComment";//承包方备注
        public const string VirtualPersonLocation = "VirtualPersonLocation";//坐落地域  
        public const string VirtualPersonCommunicate = "VirtualPersonCommunicate";//通信地址
        public const string VirtualPersonTelephone = "VirtualPersonTelephone";//承包方电话号码
        public const string VirtualPersonPostNumber = "VirtualPersonPostNumber";//承包方邮政编码
        public const string VirtualPersonAddress = "VirtualPersonAddress";//承包方地址
        public const string VirtualPersonAddressTown = "VirtualPersonAddressTown";//承包方地址到镇
        public const string VirtualPersonAddressVillage = "VirtualPersonAddressVillage";//承包方地址到村
        public const string VirtualPersonAddressGroup = "VirtualPersonAddressGroup";//承包方地址到组
        public const string VirtualPersonSurveyPerson = "VirtualPersonSurveyPerson";//承包方调查员
        public const string VirtualPersonSurveyDate = "VirtualPersonSurveyDate";//承包方调查日期
        public const string VirtualPersonSurveyChronicle = "VirtualPersonSurveyChronicle";//承包方调查记事
        public const string VirtualPersonCheckPerson = "VirtualPersonCheckPerson";//承包方审核员
        public const string VirtualPersonCheckDate = "VirtualPersonCheckDate";//承包方审核日期
        public const string VirtualPersonCheckOpinion = "VirtualPersonCheckOpinion";//承包方审核意见
        //共有人
        public const string SharePersonCount = "SharePersonCount";//共有人数
        public const string SharePersonName = "SharePersonName";//共有人姓名
        public const string SharePersonGender = "SharePersonGender";//共有人性别
        public const string SharePersonAge = "SharePersonAge";//共有人年龄
        public const string SharePersonBirthday = "SharePersonBirthday";//共有人出生日期年月日
        public const string SharePersonBirthMonthDay = "SharePersonBirthMonthDay";//共有人出生日期年月
        public const string SharePersonAllBirthday = "SharePersonAllBirthday";//共有人出生日期2014.1.1
        public const string SharePersonRelation = "SharePersonRelation";//共有人关系
        public const string SharePersonNumber = "SharePersonNumber";//共有人身份证号码
        public const string SharePersonComment = "SharePersonComment";//共有人备注
        public const string SharePersonNation = "SharePersonNation";//共有人民族
        public const string SharePersonAccountNature = "SharePersonAccountNature";//共有人性质
        public const string SharePersonString = "SharePersonString";//共有人字符串
        //界址点
        public const string LandPointNumber = "LandPointNumber";//界址点号
        public const string LandPointType = "LandPointType";//界址点类型
        public const string LandPointMarkType = "LandPointMarkType";//界标类型
        //界址线
        public const string LandLineNature = "LandLineNature";//界址线性质
        public const string LandLineCategory = "LandLineCategory";//界址线类别
        public const string LandLinePosition = "LandLinePosition";//界标线位置
        public const string LandLineDescription = "LandLineDescription";//界标线说明
        public const string LineNeighborPerson = "LineNeighborPerson";//界标线邻宗地承包方
        public const string LandLineNeighborFefer = "LandLineNeighborFefer";//界标线邻宗地指界人
        //宗地(示意)图
        public const string ParcelDrawPerson = "ParcelDrawPerson";//制图员
        public const string ParcelDrawDate = "ParcelDrawDate";//制图日期
        public const string ParcelCheckPerson = "ParcelCheckPerson";//审核员
        public const string ParcelCheckDate = "ParcelCheckDate";//审核日期
        public const string DrawTablePerson = "DrawTablePerson";//制表人
        public const string DrawTableDate = "DrawTableDate";//制表日期
        public const string CheckTablePerson = "CheckTablePerson";//审核人
        public const string CheckTableDate = "CheckTableDate";//审核日期
        //日期
        public const string NowYear = "NowYear";//年
        public const string NowMonth = "NowMonth";//月
        public const string NowDay = "NowDay";//日
        public const string YearName = "Year";//年
        public const string MonthName = "Month";//月
        public const string DayName = "Day";//日
        public const string CheckYearName = "CheckYear";//审核年
        public const string CheckMonthName = "CheckMonth";//审核月
        public const string CheckDayName = "CheckDay";//审核日
        public const string FullDate = "FullDate";//如：2013年5月23日。
        public const string ChineseYearName = "ChineseYear";//大写年。如：二〇一三。
        public const string ChineseMonthName = "ChineseMonth";//大写月。如：五。
        public const string ChineseDayName = "ChineseDay";//大写日。如：二十三。
        public const string FullChineseDate = "FullChineseDate";//大写全日期。如：二〇一三年五月二十三日。
        //地域
        public const string ProviceName = "Province";//省级地域名称
        public const string SmallProviceName = "SmallProvince";//不加地域单位省级地域名称
        public const string SimpleProviceName = "SimpleProvince";//省级名称简写
        public const string CityName = "City";//市级地域名称
        public const string SmallCityName = "SmallCity";//不加地域单位市级地域名称
        public const string CountyName = "County";//县级地域名称
        public const string SmallCountyName = "SmallCounty";//不加地域单位县级地域名称
        public const string TownName = "Town";//镇级地域名称
        public const string SmallTownName = "SmallTown";//不加地域单位镇级地域名称
        public const string VillageName = "Village";//村级地域名称
        public const string SmallVillageName = "SmallVillage";//不加地域单位村级地域名称
        public const string GroupName = "Group";//组级地域名称
        public const string ChineseGroupName = "ChineseGroup";//大写组级地域名称
        public const string SmallGroupName = "SmallGroup";//不加地域单位组级地域名称
        public const string SmallChineseGroupName = "GroupName";//不加地域单位组级大写地域名称
        public const string SmallChineseGroupNoZoneName = "GroupNoZoneName";//不加地域单位组级大写地域名称,无组 社区的，例如一
        public const string ZoneName = "SenderName";//地域全名称
        public const string LocationName = "ZoneName";//坐落地域
        public const string CountyUnitName = "CountyUnitName";//地域县、镇、村、组名称
        public const string CountyVillageName = "CountyVillageName";//地域县、镇、村名称
        public const string TownUnitName = "TownUnitName";//地域镇、村、组名称
        public const string VillageUnitName = "VillageUnitName";//地域村、组名称
        public const string CountyCode = "CountyCode";//县级地域编码
        //证件类型
        public const string IdentifyCard = "IdentifyCard";//居民身份证
        public const string OfficerCard = "OfficerCard";//军官证
        public const string Passport = "Passport";//护照
        public const string AgentCard = "AgentCard";//行政、企事业单位机构代码证或法人代码证
        public const string ResidenceBooklet = "ResidenceBooklet";//户口簿
        public const string CredentialOther = "CredentialOther";//证件其他
        //界标类型
        public const string DotFixTure = "DotFixTure";//钢钉
        public const string DotCement = "DotCement";//水泥桩
        public const string DotLime = "DotLime";//石灰桩
        public const string DotShoot = "DotShoot";//喷涂
        public const string DotChinaFlag = "DotChinaFlag";//瓷标志
        public const string DotNoFlag = "DotNoFlag";//无标志
        public const string DotOther = "DotOther";//其他
        public const string DotDeadman = "DotDeadman";//木桩
        public const string DotBuriedStone = "DotBuriedStone";//埋石
        public const string DotBaulk = "DotBaulk";//田埂
        //界址线类别
        public const string LineEnclosure = "LineEnclosure";//围墙
        public const string LineWall = "LineWall";//墙壁
        public const string LineRaster = "LineRaster";//栅栏
        public const string LineBaulk = "LineBaulk";//田埂
        public const string LineKennel = "LineKennel";//沟渠
        public const string LineRoad = "LineRoad";//道路
        public const string LineLinage = "LineLinage";//行树
        public const string LineLinkLine = "LineLinkLine";//两点连线
        public const string LineOther = "LineOther";//其他
        //界址线位置
        public const string LineWithin = "LineWithin";//内
        public const string LineMiddle = "LineMiddle";//中
        public const string LineOutline = "LineOutline";//外
    }
}
