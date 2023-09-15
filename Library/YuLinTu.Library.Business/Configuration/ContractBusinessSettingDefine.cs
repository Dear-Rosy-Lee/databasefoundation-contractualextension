/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账设置实体类
    /// </summary>
    public class ContractBusinessSettingDefine : NotifyCDObject
    {
        #region Fields

        private bool rightAreaMoreActualArea;
        private bool writeActualArea;
        private bool writeRightArea;
        private bool contractDelayArea;
        private bool unContractDelayArea;
        private bool unWriteActualArea;
        private bool unWriteRightArea;
        private bool unWriteConcordArea;
        private bool displayCollectUsingCBdata;
        private bool clearVirtualPersonData;
        private bool checkVirtualPersonSurveyTable;
        private bool survyInfoTableLandDisplay;
        private bool surveyLimitFillWay;
        private bool exportCompatibleOldDataExchange;
        private bool exportLandTableUseUnitNumber;
        private bool _isCheckedRibbing;
        private bool _isCheckedBaulk;

        //private bool exportVPTableCountContainsDiedPerson;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 设置是否允许调查表中确权面积大于实测面积
        /// </summary>
        public bool RightAreaMoreActualArea
        {
            get { return rightAreaMoreActualArea; }
            set { rightAreaMoreActualArea = value; NotifyPropertyChanged("RightAreaMoreActualArea"); }
        }

        /// <summary>
        /// 设置是否允许调查表中不填写实测面积
        /// </summary>
        public bool WriteActualArea
        {
            get { return writeActualArea; }
            set { writeActualArea = value; NotifyPropertyChanged("WriteActualArea"); }
        }

        /// <summary>
        /// 设置是否允许调查表中不填写确权面积
        /// </summary>
        public bool WriteRightArea
        {
            get { return writeRightArea; }
            set { writeRightArea = value; NotifyPropertyChanged("writeRightArea"); }
        }

        public bool ContractDelayArea
        {
            get { return contractDelayArea; }
            set { contractDelayArea = value; NotifyPropertyChanged("contractDelayArea"); }
        }

        /// <summary>
        /// 设置是否导出时调查表中不填写实测面积
        /// </summary>
        public bool UnWriteActualArea
        {
            get { return unWriteActualArea; }
            set { unWriteActualArea = value; NotifyPropertyChanged("UnWriteActualArea"); }
        }

        public bool UnContractDelayArea
        {
            get { return unContractDelayArea; }
            set { unContractDelayArea = value; NotifyPropertyChanged("UnContractDelayArea"); }
        }

        /// <summary>
        /// 设置是否导出时调查表中不填写确权面积
        /// </summary>
        public bool UnWriteRightArea
        {
            get { return unWriteRightArea; }
            set { unWriteRightArea = value; NotifyPropertyChanged("UnWriteRightArea"); }
        }

        /// <summary>
        /// 设置是否导出时调查表中不填写合同面积
        /// </summary>
        public bool UnWriteConcordArea
        {
            get { return unWriteConcordArea; }
            set { unWriteConcordArea = value; NotifyPropertyChanged("UnWriteConcordArea"); }
        }

        /// <summary>
        /// 呈现承包方数据或导出承包方数据时不显示集体户信息
        /// </summary>
        public bool DisplayCollectUsingCBdata
        {
            get { return displayCollectUsingCBdata; }
            set { displayCollectUsingCBdata = value; NotifyPropertyChanged("DisplayCollectUsingCBdata"); }
        }

        /// <summary>
        /// 设置清空数据时是否清空承包方数据
        /// </summary>
        public bool ClearVirtualPersonData
        {
            get { return clearVirtualPersonData; }
            set { clearVirtualPersonData = value; NotifyPropertyChanged("ClearVirtualPersonData"); }
        }

        /// <summary>
        /// 设置导入承包方调查表的检查
        /// </summary>
        public bool CheckVirtualPersonSurveyTable
        {
            get { return checkVirtualPersonSurveyTable; }
            set { checkVirtualPersonSurveyTable = value; NotifyPropertyChanged("CheckVirtualPersonSurveyTable"); }
        }

        /// <summary>
        /// 设置调查信息公示表中的地块数显示
        /// </summary>
        public bool SurvyInfoTableLandDisplay
        {
            get { return survyInfoTableLandDisplay; }
            set { survyInfoTableLandDisplay = value; NotifyPropertyChanged("SurvyInfoTableLandDisplay"); }
        }

        /// <summary>
        /// 设置界址线说明的填充方式
        /// </summary>
        public bool SurveyLimitFillWay
        {
            get { return surveyLimitFillWay; }
            set { surveyLimitFillWay = value; NotifyPropertyChanged("surveyLimitFillWay"); }
        }

        /// <summary>
        /// 导出压缩包时符合老版本标准
        /// </summary>
        public bool ExportCompatibleOldDataExchange
        {
            get { return exportCompatibleOldDataExchange; }
            set { exportCompatibleOldDataExchange = value; NotifyPropertyChanged("exportCompatibleOldDataExchange"); }
        }

        public bool ExportLandTableUseUnitNumber
        {
            get { return exportLandTableUseUnitNumber; }
            set { exportLandTableUseUnitNumber = value; NotifyPropertyChanged("exportLandTableUseUnitNumber"); }
        }

        /// <summary>
        /// 勾选田垄
        /// </summary>
        public bool IsCheckedRibbing
        {
            get { return _isCheckedRibbing; }
            set { _isCheckedRibbing = value; NotifyPropertyChanged("IsCheckedRibbing"); }
        }

        /// <summary>
        /// 勾选田埂
        /// </summary>
        public bool IsCheckedBaulk
        {
            get { return _isCheckedBaulk; }
            set { _isCheckedBaulk = value; NotifyPropertyChanged("IsCheckedBaulk"); }
        }

        #endregion Properties

        #region Ctor

        public ContractBusinessSettingDefine()
        {
            RightAreaMoreActualArea = true;
            WriteActualArea = true;
            WriteRightArea = true;
            ContractDelayArea = true;
            UnContractDelayArea = true;
            UnWriteActualArea = false;
            UnWriteRightArea = false;
            UnWriteConcordArea = false;
            DisplayCollectUsingCBdata = true;
            ClearVirtualPersonData = true;
            CheckVirtualPersonSurveyTable = false;
            SurvyInfoTableLandDisplay = false;
            SurveyLimitFillWay = false;
            ExportLandTableUseUnitNumber = false;
            IsCheckedRibbing = true;
            IsCheckedBaulk = false;
        }

        #endregion Ctor

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractBusinessSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractBusinessSettingDefine>();
            var section = profile.GetSection<ContractBusinessSettingDefine>();
            return section.Settings;
        }
    }
}