/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Component.Common;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出数据汇总任务参数
    /// </summary>
    public class DataSummaryExportArgument : TaskArgument
    {
        #region Fields

        private string selectZoneAndPersonInfo;   //当前地域名称+编码+选定人
        private string summaryDataFilePath;   //汇总数据路径  

        //导出参数
        private bool exportVPSurvyTable;    //承包方调查表
        private bool exportPublicDataWord;  //公示结果归户表
        private bool exportSurvyInfoPublishTable;   //调查信息公示表
        private bool exportLandSurvyTable;   //地块调查表
        private bool exportZYSurvySummaryTable; //资源汇总
        private bool exportDataSurvySummaryTable;   //数据汇总
        private bool exportRegeditBook;        //登记薄       
        private bool exportSenderSurvyTable;        //发包方调查表

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

        [DisplayLanguage("调查信息公示表")]
        [DescriptionLanguage("导出调查信息公示表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportSurvyInfoPublishTable
        {
            get { return exportSurvyInfoPublishTable; }
            set { exportSurvyInfoPublishTable = value; NotifyPropertyChanged("ExportSurvyInfoPublishTable"); }
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

        [DisplayLanguage("资源调查汇总表")]
        [DescriptionLanguage("导出资源调查汇总表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportZYSurvySummaryTable
        {
            get { return exportZYSurvySummaryTable; }
            set { exportZYSurvySummaryTable = value; NotifyPropertyChanged("ExportZYSurvySummaryTable"); }
        }

        [DisplayLanguage("经营权调查汇总表")]
        [DescriptionLanguage("导出农村土地承包经营权调查汇总表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportDataSurvySummaryTable
        {
            get { return exportDataSurvySummaryTable; }
            set { exportDataSurvySummaryTable = value; NotifyPropertyChanged("ExportDataSurvySummaryTable"); }
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
        //[DisplayLanguage("承包权证")]
        //[DescriptionLanguage("导出承包权证")]
        //[PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        //public bool ExportContractRegeditBook
        //{
        //    get { return exportContractRegeditBook; }
        //    set { exportContractRegeditBook = value; NotifyPropertyChanged("ExportContractRegeditBook"); }
        //}

        [DisplayLanguage("发包方调查表")]
        [DescriptionLanguage("发包方调查表")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportSenderSurvyTable
        {
            get { return exportSenderSurvyTable; }
            set { exportSenderSurvyTable = value; NotifyPropertyChanged("ExportSenderSurvyTable"); }
        }

        //[DisplayLanguage("户主声明书")]
        //[DescriptionLanguage("导出户主声明书")]
        //[PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        //public bool ExportVPApplyBook
        //{
        //    get { return exportVPApplyBook; }
        //    set { exportVPApplyBook = value; NotifyPropertyChanged("ExportVPApplyBook"); }
        //}
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSummaryExportArgument()
        { }

        #endregion
    }
}
