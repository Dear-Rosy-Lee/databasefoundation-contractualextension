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
    /// 承包台账设置实体类
    /// </summary>
    public class ContractBusinessSettingDefineWork : NotifyCDObject
    {
        #region Fields

        private bool rightAreaMoreActualArea;
        private bool writeActualArea;
        private bool writeRightArea;
        private bool displayCollectUsingCBdata;
        private bool clearVirtualPersonData;
        private bool checkVirtualPersonSurveyTable;
        private bool survyInfoTableLandDisplay;
        private bool landSurvyTableDataCondition;
        private bool surveyLimitFillWay;
        private bool contractSketchMapOutPattern;
        private bool selectLandToHandle;

        #endregion

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
        /// 设置地块调查表中显示数据条件
        /// </summary>
        public bool LandSurvyTableDataCondition
        {
            get { return landSurvyTableDataCondition; }
            set { landSurvyTableDataCondition = value; NotifyPropertyChanged("LandSurvyTableDataCondition"); }
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
        /// 设置承包示意图图出图方式
        /// </summary>
        public bool ContractSketchMapOutPattern
        {
            get { return contractSketchMapOutPattern; }
            set { contractSketchMapOutPattern = value; NotifyPropertyChanged("ContractSketchMapOutPattern"); }
        }

        /// <summary>
        /// 设置处理地块示意图时是否可以选择地块进行处理
        /// </summary>
        public bool SelectLandToHandle
        {
            get { return selectLandToHandle; }
            set { selectLandToHandle = value; NotifyPropertyChanged("SelectLandToHandle"); }
        }

        #endregion

        #region Ctor

        public ContractBusinessSettingDefineWork()
        {
            RightAreaMoreActualArea = true;
            WriteActualArea = true;
            WriteRightArea = true;
            DisplayCollectUsingCBdata = true;
            ClearVirtualPersonData = true;
            CheckVirtualPersonSurveyTable = false;
            SurvyInfoTableLandDisplay = false;
            LandSurvyTableDataCondition = false;
            SurveyLimitFillWay = false;
            ContractSketchMapOutPattern = false;
            SelectLandToHandle = false;
        }

        #endregion
    }
}
