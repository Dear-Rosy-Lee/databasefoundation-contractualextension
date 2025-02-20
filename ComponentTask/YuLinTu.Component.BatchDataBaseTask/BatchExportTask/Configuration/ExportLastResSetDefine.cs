/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 导出数据库实体设置
    /// </summary>
    public class ExportLastResSetDefine : NotifyCDObject
    {
        #region Properties

        public string SelectZoneName
        {
            get { return _SelectZoneName; }
            set { _SelectZoneName = value.TrimSafe(); NotifyPropertyChanged("SelectZoneName"); }
        }
        private string _SelectZoneName;

        public string SaveFolderName
        {
            get { return _SaveFolderName; }
            set { _SaveFolderName = value; NotifyPropertyChanged("SaveFolderName"); }
        }
        private string _SaveFolderName;

        public string HasJZDX
        {
            get { return _HasJZDX; }
            set { _HasJZDX = value; NotifyPropertyChanged("HasJZDX"); }
        }
        private string _HasJZDX;

        /// <summary>
        /// 导出权属来源附件
        /// </summary>
        public string HasScanDataFolder
        {
            get { return _HasScanDataFolder; }
            set { _HasScanDataFolder = value; NotifyPropertyChanged("HasScanDataFolder"); }
        }
        private string _HasScanDataFolder;

        public string InspectionData
        {
            get { return _InspectionData; }
            set { _InspectionData = value; NotifyPropertyChanged("InspectionData"); }
        }
        private string _InspectionData;

        public string InspectionDocNumRepeat
        {
            get { return _InspectionDocNumRepeat; }
            set { _InspectionDocNumRepeat = value; NotifyPropertyChanged("InspectionDocNumRepeat"); }
        }
        private string _InspectionDocNumRepeat;

        public string IsReportErrorICN
        {
            get { return _IsReportErrorICN; }
            set { _IsReportErrorICN = value; NotifyPropertyChanged("IsReportErrorICN"); }
        }
        private string _IsReportErrorICN;

        public string IsReportNoConcordNoLandsFamily
        {
            get { return _IsReportNoLandsFamily; }
            set { _IsReportNoLandsFamily = value; NotifyPropertyChanged("IsReportNoLandsFamily"); }
        }
        private string _IsReportNoLandsFamily;

        public string IsReportNoConcordLands
        {
            get { return _IsReportNoConcordLands; }
            set { _IsReportNoConcordLands = value; NotifyPropertyChanged("IsReportNoConcordLands"); }
        }
        private string _IsReportNoConcordLands;

        public string IsSaveParcelPathAsPDF
        {
            get { return _IsSaveParcelPathAsPDF; }
            set { _IsSaveParcelPathAsPDF = value; NotifyPropertyChanged("IsSaveParcelPathAsPDF"); }
        }
        private string _IsSaveParcelPathAsPDF;

        public CbdkxxAwareAreaExportEnum CBDKXXAwareAreaExportSet
        {
            get { return _CBDKXXAwareAreaExportSet; }
            set { _CBDKXXAwareAreaExportSet = value; NotifyPropertyChanged("CBDKXXAwareAreaExportSet"); }
        }
        private CbdkxxAwareAreaExportEnum _CBDKXXAwareAreaExportSet;
        
        public ExportLastResSetDefine()
        {

            HasJZDX = "false";
            HasScanDataFolder = "false";
            InspectionData = "false";
            InspectionDocNumRepeat = "false";
            IsReportErrorICN = "false";
            IsReportNoConcordLands = "true";
            IsReportNoConcordNoLandsFamily = "true";
            IsSaveParcelPathAsPDF = "true";
            CBDKXXAwareAreaExportSet = CbdkxxAwareAreaExportEnum.实测面积;
        }

        #endregion
    }
}
