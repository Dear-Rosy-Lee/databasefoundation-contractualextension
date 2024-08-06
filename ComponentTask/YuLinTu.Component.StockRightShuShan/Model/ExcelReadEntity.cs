using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.StockRightShuShan.Model
{
    /// <summary>
    /// excel解析帮助实体
    /// </summary>
    public class ExcelReadEntity
    {
        /// <summary>
        /// 承包方编号
        /// </summary>
        public string ContractorNumber { get; set; }
        /// <summary>
        /// 承包方姓名
        /// </summary>
        public string ContractorName { get; set; }
        /// <summary>
        /// 家庭成员数
        /// </summary>
        public int SharePersonCount { get; set; }
        /// <summary>
        /// 地块数
        /// </summary>
        public int LandCount { get; set; }
        /// <summary>
        /// 承包方信息开始读取的行数
        /// </summary>
        public int StartRow { get; set; }
    }
}
