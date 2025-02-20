/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 汇总导出-选择表单参数
    /// </summary>
    public class ExportDataSummarySelectTableWork
    {
        //导出参数
        public bool ExportVPSurvyTable { set; get; }    //承包方调查表
        public bool ExportPublicDataWord { set; get; }   //公示结果归户表
        public bool ExportLandWordParcel { set; get; }    //地块示意图
        public bool ExportLandSurvyTable { set; get; }    //地块调查表
        public bool ExportSingleRequireBook { set; get; }  //单户申请书
        public bool ExportContractConcord { set; get; }    //承包合同
        public bool ExportRegeditBook { set; get; }         //登记薄
        public bool ExportContractRegeditBook { set; get; }   //承包权证
        public bool ExportVPDelegateBook { set; get; }         //户主委托书
        public bool ExportVPApplyBook { set; get; }           //户主声明书
    }
}
