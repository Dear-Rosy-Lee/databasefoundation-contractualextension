/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 导出成果数据库实体设置
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

        public string UnitName
        {
            get { return _UnitName; }
            set { _UnitName = value; NotifyPropertyChanged("UnitName"); }
        }

        private string _UnitName;

        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; NotifyPropertyChanged("ContactPerson"); }
        }

        private string _ContactPerson;

        public string ContactPhone
        {
            get { return _ContactPhone; }
            set { _ContactPhone = value; NotifyPropertyChanged("ContactPhone"); }
        }

        private string _ContactPhone;

        public string MailingAddress
        {
            get { return _MailingAddress; }
            set { _MailingAddress = value; NotifyPropertyChanged("MailingAddress"); }
        }

        private string _MailingAddress;

        public string PostCode
        {
            get { return _PostCode; }
            set { _PostCode = value; NotifyPropertyChanged("PostCode"); }
        }

        private string _PostCode;

        //public string HasJZDX
        //{
        //    get { return _HasJZDX; }
        //    set { _HasJZDX = value; NotifyPropertyChanged("HasJZDX"); }
        //}
        //private string _HasJZDX;

        //public string HasJZX
        //{
        //    get { return _HasJZX; }
        //    set { _HasJZX = value; NotifyPropertyChanged("HasJZX"); }
        //}
        //private string _HasJZX;

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

        public string OnlyExportKey
        {
            get { return _OnlyExportKey; }
            set { _OnlyExportKey = value; NotifyPropertyChanged("OnlyExportKey"); }
        }

        private string _OnlyExportKey;

        public string UseUniteNumberExport
        {
            get { return _UseUniteNumberExport; }
            set { _UseUniteNumberExport = value; NotifyPropertyChanged("UseUniteNumberExport"); }
        }

        private string _UseUniteNumberExport;

        /// <summary>
        /// 只导出地块、界址点、界址线
        /// </summary>
        public string OnlyExportLandResult
        {
            get { return _OnlyExportLandResult; }
            set { _OnlyExportLandResult = value; NotifyPropertyChanged("OnlyExportLandResult"); }
        }

        private string _OnlyExportLandResult;

        /// <summary>
        /// 包含界址点、界址线
        /// </summary>
        public string ContainDotLine
        {
            get { return _ContainDotLine; }
            set { _ContainDotLine = value; NotifyPropertyChanged("ContainDotLine"); }
        }

        private string _ContainDotLine;

        /// <summary>
        /// 导出地域编码，地域名称
        /// </summary>
        public string ExportLandCode
        {
            get { return _ExportLandCode; }
            set { _ExportLandCode = value; NotifyPropertyChanged("ExportLandCode"); }
        }

        private string _ExportLandCode;

        public ExportLastResSetDefine()
        {
            UseUniteNumberExport = "false";
            //HasJZDX = "false";
            //HasJZX = "false";
            HasScanDataFolder = "false";
            InspectionData = "false";
            InspectionDocNumRepeat = "false";
            IsReportErrorICN = "false";
            IsReportNoConcordLands = "true";
            IsReportNoConcordNoLandsFamily = "true";
            IsSaveParcelPathAsPDF = "true";
            OnlyExportKey = "true";
            _OnlyExportLandResult = "false";
            _ContainDotLine = "false";
            _ExportLandCode = "false";
            CBDKXXAwareAreaExportSet = CbdkxxAwareAreaExportEnum.实测面积;
        }

        #endregion Properties
    }
}