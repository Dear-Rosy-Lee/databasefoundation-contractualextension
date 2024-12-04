/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包经营权书签
    /// </summary>
    public partial class AgricultureBookMark : WordBookMark
    {
        //承包方
        public const string ContractorName = "ContractorName";//承包方姓名

        public const string ContractorGender = "ContractorGender";//承包方性别
        public const string ContractorAge = "ContractorAge";//承包方年龄
        public const string ContractorBirthMonthDay = "ContractorBirthMonthDay";//承包方出生日期年月
        public const string ContractorBirthday = "ContractorBirthday";//承包方出生日期
        public const string ContractorAllBirthday = "ContractorAllBirthday";//承包方出生全日期
        public const string ContractorNumber = "ContractorNumber";//承包方户号
        public const string ContractorAllNumber = "ContractorAllNumber";//承包方全户号
        public const string ContractorIdentifyNumber = "ContractorIdentifyNumber";//承包方身份证号码
        public const string ContractorOtherCardNumber = "ContractorOtherCardNumber";//其他证件号码
        public const string ContractorCount = "ContractorCount";//承包方家庭成员个数
        public const string ContractorMainCount = "ContractorMainCount";//承包方家庭成员个数 不包括：去世、外嫁、迁出 人数
        public const string ContractorAllocationPerson = "ContractorAllocationPerson";//承包方实际分配人数
        public const string ContractorComment = "ContractorComment";//承包方备注
        public const string ContractorLocation = "ContractorLocation";//坐落地域
        public const string ContractorCommunicate = "ContractorCommunicate";//通信地址
        public const string ContractorTelephone = "ContractorTelephone";//承包方电话号码
        public const string ContractorPostNumber = "ContractorPostNumber";//承包方邮政编码
        public const string ContractorAddress = "ContractorAddress";//承包方地址
        public const string ContractorAddressTown = "ContractorAddressTown";//承包方地址到镇
        public const string ContractorAddressVillage = "ContractorAddressVillage";//承包方地址到村
        public const string ContractorAddressGroup = "ContractorAddressGroup";//承包方地址到组
        public const string ContractorSurveyPerson = "ContractorSurveyPerson";//承包方调查员
        public const string ContractorSurveyDate = "ContractorSurveyDate";//承包方调查日期
        public const string ContractorSurveyChronicle = "ContractorSurveyChronicle";//承包方调查记事
        public const string ContractorCheckPerson = "ContractorCheckPerson";//承包方审核员
        public const string ContractorCheckDate = "ContractorCheckDate";//承包方审核日期
        public const string ContractorCheckOpinion = "ContractorCheckOpinion";//承包方审核意见
        public const string ContractorPublicityPerson = "PublicityChroniclePerson";//承包方记事人
        public const string ContractorPublicityDate = "PublicityDate";//承包方审核日期
        public const string ContractorPublicityChronicle = "PublicityChronicle";//承包方审核意见
        public const string ContractorConcordNumber = "ContractorConcordNumber";//承包方 合同编码


        public const string ConcordStartYear = "ConcordStartYear";//承包起始年
        public const string ConcordStartMonth = "ConcordStartMonth";//承包起始月
        public const string ConcordStartDay = "ConcordStartDay";//承包起始日
        public const string ConcordEndYear = "ConcordEndYear";//承包起始年
        public const string ConcordEndMonth = "ConcordEndMonth";//承包起始月
        public const string ConcordEndDay = "ConcordEndDay";//承包起始日

        //地块
        public const string AgricultureCount = "LandCount";//地块总数

        public const string AgricultureContractLandCount = "ContractLandCount";//承包地地块总数
        public const string AgricultureOtherCount = "OtherLandCount";//非承包地地块总数
        public const string AgricultureActualAreaCount = "ActualAreaCount";//地块总实测面积
        public const string AgricultureContractLandActualAreaCount = "ContractLandActualAreaCount";//承包地块总实测面积
        public const string AgricultureOtherLandActualAreaCount = "OtherLandActualAreaCount";//非承包地块总实测面积
        public const string AgricultureAwareAreaCount = "AwareAreaCount";//地块总确权面积
        public const string AgricultureContractLandAwareAreaCount = "ContractLandAwareAreaCount";//承包地块总确权面积
        public const string AgricultureOtherLandAwareAreaCount = "OtherLandAwareAreaCount";//非承包地块总确权面积
        public const string AgricultureTableAreaCount = "TableAreaCount";//地块总合同面积
        public const string AgricultureContractLandTableAreaCount = "ContractLandTableAreaCount";//承包地块总合同面积
        public const string AgricultureOtherLandTableAreaCount = "OtherLandTableAreaCount";//非承包地块总合同面积
        public const string AgricultureModoAreaCount = "ModoAreaCount";//地块总机动地面积
        public const string AgricultureContractLandModoAreaCount = "ContractLandModoAreaCount";//承包地块总机动地面积
        public const string AgricultureOtherLandModoAreaCount = "OtherLandModoAreaCount";//非承包地块总机动地面积
        public const string AgricultureName = "LandName";//地块名称
        public const string AgricultureNumber = "LandNumber";//地块编码
        public const string AgricultureActualArea = "ActualArea";//地块实测面积
        public const string AgricultureAwareArea = "AwareArea";//地块确权面积
        public const string AgricultureTableArea = "TableArea";//地块二轮台账面积
        public const string AgricultureModoArea = "ModoArea";//地块机动地面积
        public const string AgricultureSmallNumber = "LandSmallNumber";//地块角码编号
        public const string AgricultureLandLevel = "LandLevel";//地力等级
        public const string AgricultureLandType = "LandType";//地类
        public const string AgricultureAllLandsNeighbor = "AllLandsNeighbor";//所有的四至内容，只有人名，不包括道路 田埂 沟渠
        public const string AgricultureNeighbor = "LandNeighbor";//四至内容
        public const string AgricultureNoPreNeighbor = "NoPreLandNeighbor";//四至内容 无前面的 东南西北或者 东西南北的
        public const string AgricultureNeighborFigure = "LandNeighborFigrue";//四至见附图
        public const string AgricultureIsFarmarLand = "LandIsFarmerLand";//是否基本农田
        public const string AgricultureComment = "LandComment";//备注
        public const string AgricultureEast = "East";//四至东
        public const string AgricultureSouth = "South";//四至南
        public const string AgricultureWest = "West";//四至西
        public const string AgricultureNorth = "North";//四至北
        public const string AgricultureEastName = "EastName";//四至东
        public const string AgricultureSouthName = "SouthName";//四至南
        public const string AgricultureWestName = "WestName";//四至西
        public const string AgricultureNorthName = "NorthName";//四至北
        public const string AgricultureConstructMode = "LandConstructMode";//承包方式
        public const string AgricultureConstractType = "LandConstractType";//地块类别
        public const string AgriculturePlotNumber = "LandPlotNumber";//地块畦数
        public const string AgricultureManagerType = "LandManagerType";//地块经营方式
        public const string AgricultureSourceFamilyName = "LandSourceFamilyName";//原户主姓名
        public const string AgriculturePlantType = "LandPlantType";//耕保类型
        public const string AgricultureTransterMode = "LandTransterMode";//流转方式
        public const string AgricultureTransterTerm = "LandTransterTerm";//流转期限
        public const string AgricultureTransterArea = "LandTransterArea";//流转面积
        public const string AgriculturePlatType = "LandPlatType";//种植类型
        public const string AgriculturePurpose = "LandPurpose";//土地用途
        public const string AgricultureUseSituation = "LandUseSituation";//土地利用情况
        public const string AgricultureYield = "LandYield";//土地产量情况
        public const string AgricultureOutputValue = "LandOutputValue";//土地产值情况
        public const string AgricultureIncomeSituation = "LandIncomeSituation";//土地收益情况
        public const string AgricultureElevation = "LandElevation";//高程
        public const string AgricultureSurveyPerson = "LandSurveyPerson";//地块调查员
        public const string AgricultureSurveyDate = "LandSurveyDate";//地块调查日期
        public const string AgricultureSurveyChronicle = "LandSurveyChronicle";//地块调查记事
        public const string AgricultureCheckPerson = "LandCheckPerson";//地块审核员
        public const string AgricultureCheckDate = "LandCheckDate";//地块审核日期
        public const string AgricultureCheckOpinion = "LandCheckOpinion";//地块审核意见
        public const string AgricultureImageNumber = "LandImageNumber";//地块图幅号
        public const string AgricultureFefer = "LandFefer";//地块指界人
        public const string AgricultureShape = "AgricultureShape";//地块形状
        public const string AgricultureAllShape = "AgricultureAllShape";//地块鹰眼
        public const string AgricultureString = "AgricultureString";//地块字符串

        //地块扩展
        public const string AgricultureReclationActualArea = "ReclamationActualArea";//开垦地实测面积

        public const string AgricultureReclationAwareArea = "ReclamationAwareArea";//开垦地确权面积
        public const string AgricultureReclationTableArea = "ReclamationTableArea";//开垦地台帐面积
        public const string AgricultureReclationModoArea = "ReclamationModeArea";//开垦地机动地面积
        public const string AgricultureRetainActualArea = "RetainActualArea";//总实测面积-开垦地二轮面积
        public const string AgricultureRetainAwareArea = "RetainAwareArea";//总实测面积-开垦地总确权面积
        public const string AgricultureRetainTableArea = "RetainTableArea";//总二轮面积-开垦地二轮面积
        public const string AgricultureRetainModoArea = "RetainModeArea";//总机动地面积-开垦地机动地面积

        //界址点
        public const string AgricultureLandPointNumber = "LandPointNumber";//界址点号

        public const string AgricultureLandPointType = "LandPointType";//界址点类型
        public const string AgricultureLandPointMarkType = "LandPointMarkType";//界标类型

        //界址线
        public const string AgricultureLandLineNature = "LandLineNature";//界址线性质

        public const string AgricultureLandLineCategory = "LandLineCategory";//界址线类别
        public const string AgricultureLandLinePosition = "LandLinePosition";//界标线位置
        public const string AgricultureLandLineDescription = "LandLineDescription";//界标线说明
        public const string AgricultureLandLineNeighborPerson = "LineNeighborPerson";//界标线邻宗地承包方
        public const string AgricultureLandLineNeighborFefer = "LandLineNeighborFefer";//界标线邻宗地指界人

        //地块示意图
        public const string AgricultureLandDrawPerson = "ParcelDrawPerson";//制图员

        public const string AgricultureLandDrawDate = "ParcelDrawDate";//制图日期
        public const string AgricultureLandCheckPerson = "ParcelCheckPerson";//审核员
        public const string AgricultureLandCheckDate = "ParcelCheckDate";//审核日期
        public const string AgricultureDrawTablePerson = "DrawTablePerson";//制表人
        public const string AgricultureDrawTableDate = "DrawTableDate";//制表日期
        public const string AgricultureCheckTablePerson = "CheckTablePerson";//审核人
        public const string AgricultureCheckTableDate = "CheckTableDate";//审核日期

        //发包方
        public const string SenderName = "SenderName";//发包方名称
        public const string SenderLawyerCode = "SenderLawyerCode";//发包方社会信用编码
        public const string SenderNameExpress = "SenderNameExpress";//发包方名称扩展如(第一村民小组)。
        public const string SenderLawyerName = "SenderLawyerName";//发包方法人名称
        public const string SenderLawyerTelephone = "SenderLawyerTelephone";//发包方法人联系方式
        public const string SenderLawyerAddress = "SenderLawyerAddress";//发包方法人地址
        public const string SenderLawyerPostNumber = "SenderLawyerPostNumber";//发包方法人邮政编码
        public const string SenderLawyerCredentType = "SenderLawyerCredentType";//发包方法人证件类型
        public const string SenderLawyerCredentNumber = "SenderLawyerCredentNumber";//发包方法人证件号码
        public const string SenderCode = "SenderCode";//发包方代码
        public const string SenderCountyName = "bmSenderCounty";//发包方到县
        public const string SenderTownName = "bmSenderTown";//发包方到镇
        public const string SenderVillageName = "bmSenderVillage";//发包方到村
        public const string SenderGroupName = "bmSenderGroup";//发包方到组
        public const string SenderSurveyChronicle = "SenderSurveyChronicle";//调查记事
        public const string SenderSurveyPerson = "SenderSurveyPerson";//调查员
        public const string SenderSurveyDate = "SenderSurveyDate";//调查日期
        public const string SenderChenkOpinion = "SenderChenkOpinion";//审核意见
        public const string SenderCheckPerson = "SenderCheckPerson";//审核员
        public const string SenderCheckDate = "SenderCheckDate";//审核日期

        //合同
        public const string ConcordNumber = "ConcordNumber";//合同编号

        public const string ConcordTrem = "ConcordTrem";//合同期限
        public const string ConcordStartDate = "ConcordStartDate";//合同开始日期
        public const string ConcordEndDate = "ConcordEndDate";//合同结束日期
        public const string ConcordMode = "ConcordMode";//合同承包方式
        public const string ConcordPurpose = "ConcordPurpose";//合同土地用途
        public const string ConcordStartYearDate = "ConcordStartYearDate";//合同起始时间-年
        public const string ConcordStartMonthDate = "ConcordStartMonthDate";//合同起始时间-月
        public const string ConcordStartDayDate = "ConcordStartDayDate";//合同起始时间-日
        public const string ConcordEndYearDate = "ConcordEndYearDate";//合同结束时间-年
        public const string ConcordEndMonthDate = "ConcordEndMonthDate";//合同结束时间-月
        public const string ConcordEndDayDate = "ConcordEndDayDate";//合同结束时间-日
        public const string ConcordDate = "ConcordDate";//承包开始结束时间。如：自XX起至XX止
        public const string ConcordLandCount = "ConcordLandCount";//合同中地块总数
        public const string ConcordActualAreaCount = "ConcordActualAreaCount";//合同总实测面积
        public const string ConcordAwareAreaCount = "ConcordAwareAreaCount";//合同总确权面积
        public const string ConcordTableAreaCount = "ConcordTableAreaCount";//合同块总二轮台账面积
        public const string ConcordModoAreaCount = "ConcordModoAreaCount";//合同总机动地面积
        public const string ConcordAddress = "ConcordAddress";//合同中承包方地址
        public const string ConcordLongTime = "ConcordLongTime";//合同中长久日期

        //集体申请书
        public const string RequireFamilyCount = "RequireFamilyCount";//申请总户数

        public const string RequireLandCount = "RequireLandCount";//申请地块总数
        public const string RequireLandArea = "RequireLandArea";//申请地块总面积
        public const string RequireContractMode = "RequireContractMode";//申请承包方式
        public const string RequireTerm = "RequireTerm";//申请承包期限

        //证书
        public const string BookNumber = "BookNumber";//编号

        public const string BookOrganName = "BookOrganName";//发证机关名称
        public const string BookYear = "BookYear";//年号
        public const string BookWarrantNumber = "BookWarrantNumber";//权证编号
        public const string BookSerialNumber = "BookSerialNumber";//六位流水号
        public const string BookFullSerialNumber = "BookFullSerialNumber";//所有权证编号(包括发证机关、年号、流水号)
        public const string BookContractRegeditBookexcursus = "ContractRegeditBookexcursus";//权证附记
        public const string BookContractRegeditBookTime = "ContractRegeditBookTime";//权证登记时间
        public const string BookContractRegeditBookPerson = "ContractRegeditBookPerson";//权证登簿人
        public const string BookAllNumber = "BookAllNumber";//所有权证编号(包括发证机关、年号、权证号)
        public const string BookAwareOneYear = "BookAwareOneYear";//打印日期到年倒数第二位
        public const string BookAwareLastYear = "BookAwareTwoYear";//打印日期到年最后一位
        public const string BookAwareYear = "BookAwareYear";//打印日期到年
        public const string BookAwareFirstYear = "BookAwareFirstYear";//打印日期到年第一位
        public const string BookAwareSecondYear = "BookAwareSecondYear";//打印日期到年第二位
        public const string BookAwareThreeYear = "BookAwareThreeYear";//打印日期到年第三位
        public const string BookAwareFourYear = "BookAwareFourYear";//打印日期到年第四位
        public const string BookAwareShortYear = "BookAwareShortYear";//打印日期到年后两位
        public const string BookAwareMonth = "BookAwareMonth";//打印日期到月
        public const string BookAwareDay = "BookAwareDay";//打印日期到日
        public const string BookAllAwareDate = "BookAllAwareDate";//打印所有颁证日期 yyyy-m-d
        public const string BookAllAwareFullDate = "BookAllAwareFullDate";//打印所有颁证日期 yyyy-mm-dd
        public const string BookAllWriteDate = "BookAllWriteDate";//打印所有填证日期
        public const string BookWriteOneYear = "BookWriteOneYear";//填写日期到年
        public const string BookWriteLastYear = "BookWriteTwoYear";//填写日期到年
        public const string BookWriteYear = "BookWriteYear";//填写日期到年
        public const string BookWriteFirstYear = "BookWriteFirstYear";//打印日期到年第一位
        public const string BookWriteSecondYear = "BookWriteSecondYear";//打印日期到年第二位
        public const string BookWriteThreeYear = "BookWriteThreeYear";//打印日期到年第三位
        public const string BookWriteFourYear = "BookWriteFourYear";//打印日期到年第四位
        public const string BookWriteShortYear = "BookWriteShortYear";//打印日期到年后两位
        public const string BookWriteMonth = "BookWriteMonth";//填写日期到月
        public const string BookWriteDay = "BookWriteDay";//填写日期到日

        //其它
        public const string GovernmentName = "Government";//政府名称

        //承包方式
        public const string FamilyContract = "FamilyContract";//家庭承包

        public const string TendereeContract = "TendereeContract";//招标
        public const string VendueContract = "VendueContract";//拍卖
        public const string ConsensusContract = "ConsensusContract";//公开协商
        public const string TransferContract = "TransferContract";//转让
        public const string ExchangeContract = "ExchangeContract";//互换
        public const string OtherContract = "OtherContract";//其他

        public const string EnCode = "EnCode";//二维码
    }
}