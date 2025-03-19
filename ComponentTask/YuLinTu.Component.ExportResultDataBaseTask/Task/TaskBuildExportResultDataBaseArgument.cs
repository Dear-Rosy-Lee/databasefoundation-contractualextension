using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Component.Common;
using System.ComponentModel.DataAnnotations;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    public class TaskBuildExportResultDataBaseArgument : TaskArgument
    {
        private SettingsProfileCenter center;

        public ExportLastResSetDefine ExportLastResSettingDefine;

        #region Properties

        [DisplayLanguage("行政地域")]
        [DescriptionLanguage("行政地域")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderSelectedZoneTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string SelectZoneName
        {
            get { return _SelectZoneName; }
            set
            {
                _SelectZoneName = value.TrimSafe();
                ExportLastResSettingDefine.SelectZoneName = _SelectZoneName;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("SelectZoneName");
            }
        }

        private string _SelectZoneName;

        [DisplayLanguage("数据保存路径")]
        [DescriptionLanguage("数据保存路径")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SaveFolderName
        {
            get { return _SaveFolderName; }
            set
            {
                _SaveFolderName = value;
                ExportLastResSettingDefine.SaveFolderName = _SaveFolderName;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("SaveFolderName");
            }
        }

        private string _SaveFolderName;

        [DisplayLanguage("单位名称")]
        [DescriptionLanguage("单位名称")]
        [PropertyDescriptor(
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string UnitName
        {
            get { return _UnitName; }
            set
            {
                _UnitName = value;
                ExportLastResSettingDefine.UnitName = _UnitName;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("UnitName");
            }
        }

        private string _UnitName;

        [DisplayLanguage("联系人")]
        [DescriptionLanguage("联系人")]
        [PropertyDescriptor(
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set
            {
                _ContactPerson = value;
                ExportLastResSettingDefine.ContactPerson = _ContactPerson;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("ContactPerson");
            }
        }

        private string _ContactPerson;

        [DisplayLanguage("联系电话")]
        [DescriptionLanguage("联系电话")]
        [PropertyDescriptor(
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string ContactPhone
        {
            get { return _ContactPhone; }
            set
            {
                _ContactPhone = value;
                ExportLastResSettingDefine.ContactPhone = _ContactPhone;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("ContactPhone");
            }
        }

        private string _ContactPhone;

        [DisplayLanguage("通信地址")]
        [DescriptionLanguage("通信地址")]
        [PropertyDescriptor(
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string MailingAddress
        {
            get { return _MailingAddress; }
            set
            {
                _MailingAddress = value;
                ExportLastResSettingDefine.MailingAddress = _MailingAddress;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("MailingAddress");
            }
        }

        private string _MailingAddress;

        [DisplayLanguage("邮政编码")]
        [DescriptionLanguage("邮政编码")]
        [StringLength(6)]
        [PropertyDescriptor(
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string PostCode
        {
            get { return _PostCode; }
            set
            {
                _PostCode = value;
                ExportLastResSettingDefine.PostCode = _PostCode;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("PostCode");
            }
        }

        private string _PostCode;

        //[DisplayLanguage("包含界址线")]
        //[DescriptionLanguage("包含界址线")]
        //[PropertyDescriptor(
        //     Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
        //     UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        //public bool HasJZX
        //{
        //    get { return _HasJZX; }
        //    set
        //    {
        //        _HasJZX = value;
        //        ExportLastResSettingDefine.HasJZX = _HasJZX.ToString();
        //        NotifyPropertyChanged("HasJZX");
        //    }
        //}
        //private bool _HasJZX;

        //[DisplayLanguage("导出错误证件号码")]
        //[DescriptionLanguage("是否导出错误证件号码")]
        //[PropertyDescriptor(
        //     Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        //public bool IsReportErrorICN
        //{
        //    get { return _IsReportErrorICN; }
        //    set
        //    {
        //        _IsReportErrorICN = value;
        //        ExportLastResSettingDefine.IsReportErrorICN = _IsReportErrorICN.ToString();
        //        NotifyPropertyChanged("IsReportErrorICN");
        //    }
        //}
        //private bool _IsReportErrorICN;

        [DisplayLanguage("导出无合同空户至权属")]
        [DescriptionLanguage("是否导出没签合同的空户")]
        [PropertyDescriptor(
            Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsReportNoConcordNoLandsFamily
        {
            get { return _IsReportNoConcordNoLandsFamily; }
            set
            {
                _IsReportNoConcordNoLandsFamily = value;
                ExportLastResSettingDefine.IsReportNoConcordNoLandsFamily = _IsReportNoConcordNoLandsFamily.ToString();
                NotifyPropertyChanged("IsReportNoConcordNoLandsFamily");
            }
        }

        private bool _IsReportNoConcordNoLandsFamily;

        [DisplayLanguage("导出无合同地块至矢量")]
        [DescriptionLanguage("是否导出没签合同的地块")]
        [PropertyDescriptor(
            Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsReportNoConcordLands
        {
            get { return _IsReportNoConcordLands; }
            set
            {
                _IsReportNoConcordLands = value;
                ExportLastResSettingDefine.IsReportNoConcordLands = _IsReportNoConcordLands.ToString();
                NotifyPropertyChanged("IsReportErrorICN");
            }
        }

        private bool _IsReportNoConcordLands;

        [DisplayLanguage("导出权属来源资料附件")]
        [DescriptionLanguage("导出权属来源资料附件")]
        [PropertyDescriptor(
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool HasAffixData
        {
            get { return _HasAffixData; }
            set
            {
                _HasAffixData = value;
                ExportLastResSettingDefine.HasScanDataFolder = _HasAffixData.ToString();
                NotifyPropertyChanged("HasScanDataFolder");
            }
        }

        private bool _HasAffixData;

        [DisplayLanguage("检查数据")]
        [DescriptionLanguage("检查数据")]
        [PropertyDescriptor(
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool InspectionData
        {
            get { return _InspectionData; }
            set
            {
                _InspectionData = value;
                ExportLastResSettingDefine.InspectionData = _InspectionData.ToString();
                NotifyPropertyChanged("InspectionData");
            }
        }

        private bool _InspectionData;

        //[DisplayLanguage("检查证件号码重复")]
        //[DescriptionLanguage("检查证件号码重复")]
        //[PropertyDescriptor(
        //     Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        //public bool InspectionDocNumRepeat
        //{
        //    get { return _InspectionDocNumRepeat; }
        //    set
        //    {
        //        _InspectionDocNumRepeat = value;
        //        ExportLastResSettingDefine.InspectionDocNumRepeat = _InspectionDocNumRepeat.ToString();
        //        NotifyPropertyChanged("InspectionDocNumRepeat");
        //    }
        //}

        //private bool _InspectionDocNumRepeat;

        [DisplayLanguage("导出示意图路径为PDF")]
        [DescriptionLanguage("导出示意图路径为PDF")]
        [PropertyDescriptor(
            Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsSaveParcelPathAsPDF
        {
            get { return _IsSaveParcelPathAsPDF; }
            set
            {
                _IsSaveParcelPathAsPDF = value;
                ExportLastResSettingDefine.IsSaveParcelPathAsPDF = _IsSaveParcelPathAsPDF.ToString();
                NotifyPropertyChanged("IsSaveParcelPathAsPDF");
            }
        }

        private bool _IsSaveParcelPathAsPDF;

        [DisplayLanguage("导出确权面积设置")]
        [DescriptionLanguage("导出确权(合同)面积设置")]
        [PropertyDescriptor(
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public CbdkxxAwareAreaExportEnum CBDKXXAwareAreaExportSet
        {
            get { return _CBDKXXAwareAreaExportSet; }
            set
            {
                _CBDKXXAwareAreaExportSet = value;
                ExportLastResSettingDefine.CBDKXXAwareAreaExportSet = _CBDKXXAwareAreaExportSet;
                NotifyPropertyChanged("CBDKXXAwareAreaExportSet");
            }
        }

        private CbdkxxAwareAreaExportEnum _CBDKXXAwareAreaExportSet;

        [DisplayLanguage("只导出关键界址点")]
        [DescriptionLanguage("只导出关键界址点")]
        [PropertyDescriptor(
           Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
          UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool OnlyExportKey
        {
            get { return _OnlyExportKey; }
            set
            {
                _OnlyExportKey = value;
                ExportLastResSettingDefine.IsSaveParcelPathAsPDF = _OnlyExportKey.ToString();
                NotifyPropertyChanged("OnlyExportKey");
            }
        }

        private bool _OnlyExportKey;

        [DisplayLanguage("使用统编号导出")]
        [DescriptionLanguage("使用统编号导出")]
        [PropertyDescriptor(
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool UseUniteNumberExport
        {
            get { return _UseUniteNumberExport; }
            set
            {
                _UseUniteNumberExport = value;
                ExportLastResSettingDefine.UseUniteNumberExport = _UseUniteNumberExport.ToString();
                NotifyPropertyChanged("UseUniteNumberExport");
            }
        }

        private bool _UseUniteNumberExport;

        [DisplayLanguage("只导出地块、界址点、线")]
        [DescriptionLanguage("只导出地块、界址点、界址线等矢量文件")]
        [PropertyDescriptor(
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool OnlyExportLandResult
        {
            get { return _OnlyExportLandResult; }
            set
            {
                _OnlyExportLandResult = value;
                ExportLastResSettingDefine.OnlyExportLandResult = _OnlyExportLandResult.ToString();
                NotifyPropertyChanged("OnlyExportLandResult");
            }
        }

        private bool _OnlyExportLandResult;

        [DisplayLanguage("同时导出界址点、界址线")]
        [DescriptionLanguage("导出矢量文件时，同时导出界址点、界址线")]
        [PropertyDescriptor(
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ContainDotLine
        {
            get { return _ContainDotLine; }
            set
            {
                _ContainDotLine = value;
                ExportLastResSettingDefine.ContainDotLine = _ContainDotLine.ToString();
                NotifyPropertyChanged("ContainDotLine");
            }
        }

        private bool _ContainDotLine;

        [DisplayLanguage("导出地域编码、地域名称")]
        [DescriptionLanguage("导出权属单位代码表新增地域编码、地域名称")]
        [PropertyDescriptor(
            Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBool),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool ExportLandCode
        {
            get { return _ExportLandCode; }
            set
            {
                _ExportLandCode = value;
                ExportLastResSettingDefine.ExportLandCode = _ExportLandCode.ToString();
                NotifyPropertyChanged("ExportLandCode");
            }
        }

        private bool _ExportLandCode;

        #endregion Properties

        #region Ctor

        public TaskBuildExportResultDataBaseArgument()
        {
            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<ExportLastResSetDefine>();
            var section = profile.GetSection<ExportLastResSetDefine>();
            ExportLastResSettingDefine = (section.Settings as ExportLastResSetDefine);

            SelectZoneName = ExportLastResSettingDefine.SelectZoneName;
            SaveFolderName = ExportLastResSettingDefine.SaveFolderName;
            UnitName = ExportLastResSettingDefine.UnitName;
            ContactPerson = ExportLastResSettingDefine.ContactPerson;
            ContactPhone = ExportLastResSettingDefine.ContactPhone;
            MailingAddress = ExportLastResSettingDefine.MailingAddress;
            PostCode = ExportLastResSettingDefine.PostCode;
            UseUniteNumberExport = bool.Parse(ExportLastResSettingDefine.UseUniteNumberExport);
            OnlyExportLandResult = bool.Parse(ExportLastResSettingDefine.OnlyExportLandResult);
            ContainDotLine = bool.Parse(ExportLastResSettingDefine.ContainDotLine);
            ExportLandCode = bool.Parse(ExportLastResSettingDefine.ExportLandCode);
            //HasJZDX = bool.Parse(ExportLastResSettingDefine.HasJZDX);
            // HasJZX = bool.Parse(ExportLastResSettingDefine.HasJZX);
            HasAffixData = bool.Parse(ExportLastResSettingDefine.HasScanDataFolder);
            InspectionData = bool.Parse(ExportLastResSettingDefine.InspectionData);
            //InspectionDocNumRepeat = bool.Parse(ExportLastResSettingDefine.InspectionDocNumRepeat);
            //IsReportErrorICN = bool.Parse(ExportLastResSettingDefine.IsReportErrorICN);
            IsReportNoConcordLands = bool.Parse(ExportLastResSettingDefine.IsReportNoConcordLands);
            IsReportNoConcordNoLandsFamily = bool.Parse(ExportLastResSettingDefine.IsReportNoConcordNoLandsFamily);
            IsSaveParcelPathAsPDF = bool.Parse(ExportLastResSettingDefine.IsSaveParcelPathAsPDF);
            CBDKXXAwareAreaExportSet = ExportLastResSettingDefine.CBDKXXAwareAreaExportSet;
            OnlyExportKey = bool.Parse(ExportLastResSettingDefine.OnlyExportKey);
        }

        #endregion Ctor

        #region Methods

        private void SaveExportLastResSetDefine()
        {
            center.Save<ExportLastResSetDefine>();
        }

        #endregion Methods
    }

    /// <summary>
    /// 导出MDB库cbdkxx表确权(合同)面积下拉选项，选择以哪种面积导出
    /// </summary>
    public enum CbdkxxAwareAreaExportEnum
    {
        /// <summary>
        /// 确权面积
        /// </summary>
        确权面积 = 0,

        /// <summary>
        /// 实测面积
        /// </summary>
        实测面积 = 1,
    }
}