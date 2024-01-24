/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 汇总导出-选择表单参数
    /// </summary>
    public class ExportDataSummarySelectTable
    {
        /// <summary>
        /// 承包方调查表
        /// </summary>
        public bool ExportVPSurvyTable { set; get; }
        /// <summary>
        /// 发包方调查表
        /// </summary>
        public bool ExportSenderSurvyTable { set; get; }
        /// <summary>
        /// 调查信息公示表
        /// </summary>
        public bool ExportSurvyInfoPublishTable { set; get; }
        /// <summary>
        /// 承包合同
        /// </summary>
        public bool ExportConcord{ set; get; }
        /// <summary>
        /// 公示结果归户表
        /// </summary>
        public bool ExportPublicDataWord { set; get; }
        /// <summary>
        /// 地块调查表
        /// </summary>
        public bool ExportLandSurvyTable { set; get; } 
        /// <summary>
        /// 登记薄
        /// </summary>
        public bool ExportRegeditBook { set; get; }
        /// <summary>
        /// 农村土地承包经营权确权登记审批表
        /// </summary>
        public bool ExportDataDJBZSPTableTable { set; get; }
        /// <summary>
        /// 农村土地承包经营权确权登记申请书
        /// </summary>
        public bool ExportDJBZSQSTable { set; get; } 
        /// <summary>
        /// 户主声明书
        /// </summary>
        public bool ExportStatement { set; get; }
        /// <summary>
        /// 委托代理声明书
        /// </summary>
        public bool ExportAttorney { set; get; }
        /// <summary>
        /// 地块示意图
        /// </summary>
        public bool ExportLandParcel { set; get; }
    }
}
