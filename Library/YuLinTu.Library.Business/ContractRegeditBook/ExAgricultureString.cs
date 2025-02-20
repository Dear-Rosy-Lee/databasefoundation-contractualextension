/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包地打印设置
    /// </summary>
   public class ExAgricultureString
   {
       //打印年号设置
       public const string ISPRINTBOOKNUMBER = "IsPrintBookNumber";//是否打印编号
       public const string BOOKNUMBERPRINTMEDIAN = "BookNumberPrintMedian";//编号打印位数
       public const string WARRANTNUMBERPRINTMEDIAN = "WarrantNumberPrintMedian";//编号打印位数
       public const string ISPRINTORGANNAME = "IsPrintOrganName";//是否打印发证机关缩写
       public const string ORGANNAME = "OrganName";//填证机关缩写
       public const string ISPRINTYEAR = "IsPrintYear";//是否打印年号
       public const string PRINTYEARDATE = "PrintYearDate";

       //设置颁证日期
       public const string ISPRINTDATETIME = "IsPrintDateTime";
       public const string PRINTDATATIME = "PrintDataTime";
       public const string ISPRINTTOYEAR = "IsPrintToYear";
       public const string ISPRINTTOMONTH = "IsPrintToMonth";
       public const string ISPRINTTODAY = "IsPrintToDay";
       public const string ISPRINTTWOYEAR = "IsPrintTwoYear";

       //设置承包期限
       public const string ISENDTIMETOLONG = "IsEndTimeToLong";
       public const string CONTRACTSTARTTIME = "ContractStartTime";
       public const string CONTRACTENDTIME = "ContractEndTime";

       //常规设置
       public const string ISSENDERSHOWGROUP = "IsSenderShowGroup";
       public const string ISSHOWCONTRACTTYPE = "IsShowContractType";
       public const string ISSHOWLANDPURPOSE = "IsShowLandPurpose";
       public const string ISPRINTCONTRACTTERM = "IsPrintContractTerm";
       public const string ISSHOWBIRTHDAY = "IsShowBirthday";
       public const string ISPRINTLANDNUMBER = "IsPrintLandNumber";
       public const string ISSHOWLANDLEVEL = "IsShowLandLevel";
       public const string ISSHOWLANDTYPE = "IsShowLandType";
       public const string ISSHOWLANDFIGURE = "IsShowLandFigure";
       public const string ISSHOWLANDNEIGHBOR = "IsShowLandNeighbor";
       public const string ISSHOWBASICFARMERLAND = "IsShowBasicFarmerLand";
       public const string ISSHOWTABAREA = "IsShowTabArea";

       //设置填证日期
       public const string ISWRITEDATETIME = "IsWriteDateTime";
       public const string WRITEDATATIME = "WriteDataTime";
       public const string ISWRITETOYEAR = "IsWriteToYear";
       public const string ISWRITETOMONTH = "IsWriteToMonth";
       public const string ISWRITETODAY = "IsWriteToDay";
       public const string ISWRITETWOYEAR = "IsWriteTwoYear";

       //打印图形
       public const string ISPRINTAllFIGURE = "IsPrintAllFigure";

       public const string ISSHOWLANDNEIGHBORDIRECTION = "IsShowLandNeighborDirection";//显示四至方向名称

       public const string ISSHOWLANDNUMBERPERFIX = "IsShowLandNumberPerfix";//显示地块编码地域前缀

       public const string ISSHOWLANDNUMBERFAMILYNUMBER = "IsShowLandNumberFamilyNumber";//显示地块编码户编码

       public const string ISSHOWLANDNUMBERCONTRACTMODE = "IsShowLandNumberContractMode";//显示地块编码承包方式

       public const string WARRANTSHAREPERSONCOUNT = "WarrantSharePersonCountValue";//证书上共有人数

       public const string WARRANTLANDCOUNT = "WarrantLandCountValue";//证书上地块数

       //打印证书书签
       public const string BOOKNUMBER = "bmBookNumber";//编号
       public const string BOOKORANGNAME = "bmOrganName";//发证机关名称
       public const string BOOKYEAR = "bmYear";//年号
       public const string BOOKREGNUMBER = "bmRegNumber";//权证编号
       public const string BOOKRALLREGNUMBER = "bmAllRegNumber";//所有权证编号(包括发证机关、年号、权证号)
       public const string BOOKAWAREONEYEAR = "bmAwareOneYear";//打印日期到年倒数第二位
       public const string BOOKAWARETWOYEAR = "bmAwareTwoYear";//打印日期到年最后一位
       public const string BOOKAWAREYEAR = "bmAwareYear";//打印日期到年
       public const string BOOKAWAREMONTH = "bmAwareMonth";//打印日期到月
       public const string BOOKAWAREDAY = "bmAwareDay";//打印日期到日
       public const string BOOKALLAWAREDATE = "bmAllAwareDate";//打印所有颁证日期
       public const string BOOKSENDERNAME = "bmSenderName";//发包方全称
       public const string BOOKSENDERTOWN = "bmSenderTown";//发包方到镇
       public const string BOOKSENDERVILLAGE = "bmSenderVillage";//发包方到村
       public const string BOOKSENDERGROUP = "bmSenderGroup";//发包方到组
       public const string BOOKSENDEREXTENDGROUP = "bmSenderExtendGroup";//发包方扩展组
       public const string BOOKCONTRACTER = "bmContracter";//承包方名称
       public const string BOOKCONTRACTMODE = "bmContractMode";//承包方式
       public const string BOOKPOSTORNUMBER = "bmPosterNumber";//邮政编码
       public const string BOOKTELEPHONE = "bmTelephone";//电话号码
       public const string BOOKIDENTIFYNUMBER = "bmIdentifyNumber";//承包方身份证号码
       public const string BOOKCONTRACTERADDRESS = "bmContracterAddress";//承包方地址
       public const string BOOKADDRESSNAME = "bmAddressName";//承包方地址
       public const string BOOKADDRESSTOWN = "bmAddressTown";//承包方地址到镇
       public const string BOOKADDRESSVILLAGE = "bmAddressVillage";//承包方地址到村
       public const string BOOKADDRESSGROUP = "bmAddressGroup";//承包方地址到组
       public const string BOOKCONCORDNUMBER = "bmConcordNumber";//合同编号
       public const string BOOKCONTRACTERTIME = "bmStartAndEndTime";//承包开始、结束时间
       public const string BOOKSTARTTIMEYEAR = "bmStartYear";//承包开始时间到年
       public const string BOOKSTARTTIMEMONTH = "bmStartMonth";//承包开始时间到月
       public const string BOOKSTARTTIMEDAY = "bmStartDay";//承包开始时间到日
       public const string BOOKENDTIMEEYEAR = "bmEndYear";//承包结束时间到年
       public const string BOOKENDTIMEMONTH = "bmEndMonth";//承包结束时间到月
       public const string BOOKTERMTOLONG = "bmTermToLong";//承包期限为长久
       public const string BOOKENDTIMEDAY = "bmEndDay";//承包结束时间到日
       public const string BOOKLANDPURPOSE = "bmLandPurpose";//承包土地用途
       public const string BOOKALLWRITEDATE = "bmAllWriteDate";//打印所有填证日期
       public const string BOOKWRITEONEYEAR = "bmWriteOneYear";//填写日期到年
       public const string BOOKWRITETWOYEAR = "bmWriteTwoYear";//填写日期到年
       public const string BOOKWRITEYEAR = "bmWriteYear";//填写日期到年
       public const string BOOKWRITEMONTH = "bmWriteMonth";//填写日期到月
       public const string BOOKWRITEDAY = "bmWriteDay";//填写日期到日

       //共有人书签
       public const string BOOKSHAREPERSONNAME = "bmSharePersonName";//共有人姓名
       public const string BOOKSHAREPERSONGENDER = "bmSharePersonGender";//共有人性别
       public const string BOOKSHAREPERSONAGE = "bmSharePersonAge";//共有人年龄
       public const string BOOKSHAREPERSONRELATION = "bmSharePersonRelation";//共有人关系
       public const string BOOKSHAREPERSONRENUMBER = "bmSharePersonNumber";//共有人身份证号码
       public const string BOOKSHAREPERSONRECOMMENT = "bmSharePersonComment";//共有人备注

       //地块书签
       public const string BOOKLANDCOUNT = "bmLandCount";//地块总数
       public const string BOOKLANDNAME = "bmLandName";//地块名称
       public const string BOOKLANDNUMBER = "bmLandNumber";//地块编码
       public const string BOOKLANDAREA = "bmLandArea";//地块实测面积
       public const string BOOKLANDAWAREAREA = "bmAwareArea";//地块确权面积
       public const string BOOKLANDTABLEAREA = "bmTableArea";//地块二轮台账面积
       public const string BOOKSMALLLAND = "bmSmallLand";//地块角码编号
       public const string BOOKLANDLEVEL = "bmLandLevel";//等级
       public const string BOOKLANDTYPE = "bmLandType";//地类
       public const string BOOKLANDNEIGHBOR = "bmLandNeighbor";//四至
       public const string BOOKISFARMERLAN = "bmLandIsFarmerLand";//是否基本农田
       public const string BOOKLANDCOMMENT = "bmLandComment";//备注

       //数据检查设置
       public const string CHECKLANDLEVEL = "CheckLandLevel";//检查等级
       public const string CHECKLANDTYPE = "CheckLandType";//检查地类
       public const string CHECKLANDISFARMERLAND = "CheckLandIsFarmerLand";//检查是否基本农田

       //地区设置
       public const string ISCHENGDUREGION = "IsChengDuRengion";//是否成都地区
       public const string ISDOUJIANGYAN = "IsDouJiangYan";//是否都江堰地区
       public const string ISNINGXIA = "IsNingXia";//是否宁夏地区

   }
}
