using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.StockRightBase
{
    public class SpecialBookmarks
    {
        /// <summary>
        /// 共用地块的台账面积之和
        /// </summary>
        public const string TotalTableArea = "TotalTableArea";
        /// <summary>
        /// 台账面积占村（组）耕地总面积
        /// </summary>
        public const string TotalTableAreaRatio = "TotalTableAreaRatio";
        /// <summary>
        /// 实测面积占村（组）耕地总面积
        /// </summary>
        public const string TotalActualAreaRatio = "TotalActualAreaRatio";
        /// <summary>
        /// 量化到户地块数
        /// </summary>
        public const string StockLandCount = "StockLandCount";
        /// <summary>
        /// 承包开始结束日期
        /// </summary>
        public const string ConcordDate = "ConcordDate";
        /// <summary>
        /// 承包方编码
        /// </summary>
        public const string ContractorCode = "ContractorCode";
        /// <summary>
        /// 人均共有面积
        /// </summary>
        public const string TableAreaPerPerson = "TableAreaPerPerson";
        /// <summary>
        /// 量化到户总面积
        /// </summary>
        public const string TotalQuantificationArea = "TotalQuantificationArea";

        #region 确权确面积确大四至或确股不确地村（组）耕地面积（股份）量化调查表
        /// <summary>
        /// 家庭承包户总数（户）
        /// </summary>
        public const string SenderFamilyTotal = "SenderFamilyTotal";
        /// <summary>
        /// 本集体经济组织成员总数（人） 
        /// </summary>
        public const string SenderPersonTotal = "SenderPersonTotal";
        /// <summary>
        /// 共用地块数量（块）
        /// </summary>
        public const string SenderStockLandTotal = "SenderStockLandTotal";
        /// <summary>
        /// 共用地块总面积（亩）
        /// </summary>
        public const string SenderStockLandTableAreaTotal = "SenderStockLandTableAreaTotal";
        /// <summary>
        /// 人均共有面积（亩）
        /// </summary>
        public const string SenderStockLandTableAreaPerPerson = "SenderStockLandTableAreaPerPerson";
        #endregion
    }
}
