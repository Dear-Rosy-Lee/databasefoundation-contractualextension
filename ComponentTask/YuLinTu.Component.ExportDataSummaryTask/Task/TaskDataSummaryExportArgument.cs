/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Component.Common;

namespace YuLinTu.Component.ExportDataSummaryTask
{
    /// <summary>
    /// 导出数据汇总任务参数
    /// </summary>
    public class TaskDataSummaryExportArgument: TaskArgument
    {
        #region Fields

        private string selectZoneAndPersonInfo;   //当前地域名称+编码+选定人
        private string summaryDataFilePath;   //汇总数据路径  

        //导出参数
        private bool exportVPSurvyTable;    //承包方调查表
        private bool exportPublicDataWord;  //公示结果归户表
        private bool exportLandWordParcel;   //地块示意图
        private bool exportLandSurvyTable;   //地块调查表
        private bool exportSingleRequireBook; //单户申请书
        private bool exportContractConcord;   //承包合同
        private bool exportRegeditBook;        //登记薄
        private bool exportContractRegeditBook;  //承包权证
        private bool exportVPDelegateBook;        //户主委托书
        private bool exportVPApplyBook;          //户主声明书
        #endregion

        #region Properties

        [DisplayLanguage("行政地域")]
        [DescriptionLanguage("请选择行政地域")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedZoneAndVPTextBox),
            Trigger = typeof(PropertyTriggerZoneAndVP),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string SelectZoneAndPersonInfo
        {
            get { return selectZoneAndPersonInfo; }
            set { selectZoneAndPersonInfo = value; NotifyPropertyChanged("SelectZoneAndPersonInfo"); }
        }

        [DisplayLanguage("汇总数据路径")]
        [DescriptionLanguage("请选择汇总数据保存路径")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowser),
            Trigger = typeof(PropertyTriggerFile),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SummaryDataFilePath
        {
            get { return summaryDataFilePath; }
            set { summaryDataFilePath = value; NotifyPropertyChanged("SummaryDataFilePath"); }
        }

        [DisplayLanguage("承包方调查表")]
        [DescriptionLanguage("导出承包方调查表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportVPSurvyTable
        {
            get { return exportVPSurvyTable; }
            set { exportVPSurvyTable = value; NotifyPropertyChanged("ExportVPSurvyTable"); }
        }

        [DisplayLanguage("公示结果归户表")]
        [DescriptionLanguage("导出公示结果归户表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportPublicDataWord
        {
            get { return exportPublicDataWord; }
            set { exportPublicDataWord = value; NotifyPropertyChanged("ExportPublicDataWord"); }
        }

        [DisplayLanguage("地块示意图")]
        [DescriptionLanguage("导出地块示意图")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportLandWordParcel
        {
            get { return exportLandWordParcel; }
            set { exportLandWordParcel = value; NotifyPropertyChanged("ExportLandWordParcel"); }
        }

        [DisplayLanguage("地块调查表")]
        [DescriptionLanguage("导出地块调查表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportLandSurvyTable
        {
            get { return exportLandSurvyTable; }
            set { exportLandSurvyTable = value; NotifyPropertyChanged("ExportLandSurvyTable"); }
        }

        [DisplayLanguage("单户申请书")]
        [DescriptionLanguage("导出单户申请书")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportSingleRequireBook
        {
            get { return exportSingleRequireBook; }
            set { exportSingleRequireBook = value; NotifyPropertyChanged("ExportSingleRequireBook"); }
        }

        [DisplayLanguage("承包合同")]
        [DescriptionLanguage("导出承包合同")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportContractConcord
        {
            get { return exportContractConcord; }
            set { exportContractConcord = value; NotifyPropertyChanged("ExportContractConcord"); }
        }

        [DisplayLanguage("登记薄")]
        [DescriptionLanguage("导出登记薄")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportRegeditBook
        {
            get { return exportRegeditBook; }
            set { exportRegeditBook = value; NotifyPropertyChanged("ExportRegeditBook"); }
        }
        [DisplayLanguage("承包权证")]
        [DescriptionLanguage("导出承包权证")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportContractRegeditBook
        {
            get { return exportContractRegeditBook; }
            set { exportContractRegeditBook = value; NotifyPropertyChanged("ExportContractRegeditBook"); }
        }

        [DisplayLanguage("户主委托书")]
        [DescriptionLanguage("导出户主委托书")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportVPDelegateBook
        {
            get { return exportVPDelegateBook; }
            set { exportVPDelegateBook = value; NotifyPropertyChanged("ExportVPDelegateBook"); }
        }

        [DisplayLanguage("户主声明书")]
        [DescriptionLanguage("导出户主声明书")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportVPApplyBook
        {
            get { return exportVPApplyBook; }
            set { exportVPApplyBook = value; NotifyPropertyChanged("ExportVPApplyBook"); }
        }
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskDataSummaryExportArgument()
        { }

        #endregion
    }
}
