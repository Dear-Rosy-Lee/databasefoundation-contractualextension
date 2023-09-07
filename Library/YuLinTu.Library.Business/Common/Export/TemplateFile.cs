/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 模板文件帮助类
    /// </summary>
    public class TemplateFile
    {
        #region Tmelate

        #region 发包方

        /// <summary>
        /// 发包方调查表Word
        /// </summary>
        public const string SenderSurveyWord = "农村土地承包经营权发包方调查表";

        /// <summary>
        /// 发包方调查表Excel
        /// </summary>
        public const string SenderSurveyExcel = "农村土地承包经营权发包方调查表";


        #endregion

        #region 承包方

        /// <summary>
        /// 承包方调查表Word
        /// </summary>
        public const string VirtualPersonSurveyWord = "农村土地承包经营权承包方调查表";

        /// <summary>
        /// 承包方调查表Excel
        /// </summary>
        public const string VirtualPersonSurveyExcel = "农村土地承包经营权承包方调查表";

        /// <summary>
        /// 户主代表声明书
        /// </summary>
        public const string VirtualPersonApplyBook = "户主代表声明书";

        /// <summary>
        /// 公示无异议声明书
        /// </summary>
        public const string VirtualPersonIdeaBook = "公示无异议声明书";

        /// <summary>
        /// 委托代理声明书
        /// </summary>
        public const string VirtualPersonDelegateBook = "委托代理声明书";

        /// <summary>
        /// 测绘申请书
        /// </summary>
        public const string VirtualPersonSurveyBook = "测绘申请书";


        #endregion

        #region 二轮台账

        /// <summary>
        /// 二轮台账模拟调查表
        /// </summary>
        public const string SecondTableRealQueryExcel = "农村土地承包经营权二轮台帐摸底调查表";

        /// <summary>
        /// 二轮台账模拟调查公示表
        /// </summary>
        public const string SecondTablePublicityExcel = "农村土地承包经营权二轮台帐摸底调查公示表";

        /// <summary>
        /// 二轮台帐摸底调查公示确认表
        /// </summary>
        public const string SecondTableSignExcel = "农村土地承包经营权二轮台帐摸底调查公示确认表";

        /// <summary>
        /// 二轮台帐单用户确认表
        /// </summary>
        public const string SecondTableUserSignExcel = "农村土地承包经营权单户摸底调查公示确认表";

        /// <summary>
        /// 二轮台账勘界调查表
        /// </summary>
        public const string SecondTableBoundarySettleExcel = "农村土地承包经营权勘界确权调查表";

        /// <summary>
        /// 二轮台账单户调查表
        /// </summary>
        public const string SecondTableSingleFamilyExcel = "农村土地承包经营权勘界确权单户调查表";

        #endregion

        #region 承包地块

        /// <summary>
        /// 承包地块调查表Word
        /// </summary>
        public const string ContractLandSurveyWord = "农村土地承包经营权承包地块调查表";

        /// <summary>
        /// 承包地块调查表Excel 
        /// </summary>
        public const string ContractLandSurveyExceltemp = "农村土地承包经营权调查表";

        /// <summary>
        /// 承包地块单户调查表Excel
        /// </summary>
        public const string ContractLandSingleSurveyExceltemp = "农村土地承包经营权勘界确权单户签字表";

        /// <summary>
        /// 承包方调查表Word
        /// </summary>
        public const string ContractSurveyWord = "农村土地承包经营权承包方调查表";

        /// <summary>
        /// 发包方调查表Word
        /// </summary>
        public const string SenderLandSurveyWord = "农村土地承包经营权发包方调查表";

        /// <summary>
        /// 承包地块调查表Word
        /// </summary>
        public const string ContractAccountLandSurveyWord = "农村土地承包经营权承包地块调查表";

        /// <summary>
        /// 承包地块调查表Word(2页)
        /// </summary>
        public const string ContractAccountLandSurveyWordTwo = "农村土地承包经营权承包地块调查表2页";

        /// <summary>
        /// 承包地块调查表Word(3页)
        /// </summary>
        public const string ContractAccountLandSurveyWordThree = "农村土地承包经营权承包地块调查表3页";

        /// <summary>
        /// 承包地块调查表Word(其它)
        /// </summary>
        public const string ContractAccountLandSurveyWordOther = "农村土地承包经营权承包地块调查表其它";

        /// <summary>
        /// 公示结果归户表Word
        /// </summary>
        public const string PublicityWord = "农村土地承包经营权公示结果归户表";

        /// <summary>
        /// 数据汇总表
        /// </summary>
        public const string ContractSummaryExcel = "农村土地承包经营权数据统计表";

        /// <summary>
        /// 承包地块调查表Excel
        /// </summary>
        public const string ContractLandSurveyExcel = "农村土地承包经营权承包地块调查表";

        /// <summary>
        /// 导出调查信息公示表Excel
        /// </summary>
        public const string LandInfomationExcel = "农村土地承包经营权调查信息公示表";

        /// <summary>
        /// 村组公示公告Word
        /// </summary>
        public const string AnnouncementWord = "农村土地承包经营权村组公示公告";

        /// <summary>
        /// 地块示意图Word
        /// </summary>
        public const string ParcelWord = "农村土地承包经营权标准地块示意图";

        /// <summary>
        /// 界址点成果表Excel
        /// </summary>
        public const string BoundaryAddressDotResultExcel = "农村土地承包经营权界址点成果表";

        #endregion

        #region 界址信息

        /// <summary>
        /// 界址信息调查表Excel
        /// </summary>
        public const string BoundarySurveyExcel = "农村土地承包经营权界址信息表";

        #endregion

        #region 承包合同

        /// <summary>
        /// 农村土地承包经营权单户登记申请书
        /// </summary>
        public const string ContractFamilyRequireBook = "农村土地承包经营权单户登记申请书";

        /// <summary>
        /// 农村土地承包经营权证其他登记申请书
        /// </summary>
        public const string ContractFamilyOtherRequireBook = "农村土地承包经营权证其他登记申请书";

        /// <summary>
        /// 农村土地承包经营权集体登记申请书
        /// </summary>
        public const string ContractCollectApplicationBook = "农村土地承包经营权集体登记申请书";

        /// <summary>
        /// 农村土地承包经营权承包合同
        /// </summary>
        public const string ContractConcordWord = "农村土地承包经营权承包合同";

        #endregion

        #region 承包权证

        /// <summary>
        /// 农村土地承包经营权证Word
        /// </summary>
        public const string ContractRegeditBookWord = "农村土地承包经营权证";

        /// <summary>
        /// 农村土地承包经营权登记簿Word
        /// </summary>
        public const string PrivewRegeditBookWord = "农村土地承包经营权登记簿";


        /// <summary>
        /// 农村土地承包经营权颁证清册
        /// </summary>
        public const string AwareInventoryTable = "农村土地承包经营权颁证清册";

        /// <summary>
        /// 农村土地承包经营权单户确认表
        /// </summary>
        public const string SingleFamilyConfirmTable = "农村土地承包经营权勘界确权单户确认表";

        /// <summary>
        /// 农村土地承包经营权证共有人扩展word
        /// </summary>       
        public const string RegeditBookSharePersonExtendWord = "农村土地承包经营权证共有人扩展";

        /// <summary>
        /// 农村土地承包经营权证共有人扩展excel
        /// </summary>       
        public const string RegeditBookSharePersonExtendExcel = "农村土地承包经营权证共有人扩展";

        /// <summary>
        /// 农村土地承包经营权证地块扩展word
        /// </summary>       
        public const string RegeditBookLandExtendWord = "农村土地承包经营权证地块扩展";

        /// <summary>
        /// 农村土地承包经营权证地块扩展excel
        /// </summary>       
        public const string RegeditBookLandExtendExcel = "农村土地承包经营权证地块扩展";

        #endregion

        #endregion
    }
}