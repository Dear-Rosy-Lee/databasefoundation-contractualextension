/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 承包合同常规设置类
    /// </summary>
    public class ContractConcordSettingDefine: NotifyCDObject
    {
        #region Fields

        private bool singleRequireDate;
        private bool agricultureConcordAllowMultiType;
        private string _landCommentReplacement;
        private int _sendNamePrefixLevel;
        private int choosearea;
        private int choosebatch;
        private int _contractorNamePrefixLevel;

        #endregion

        #region Properties

        /// <summary>
        /// 设置承包方下是否可以签订多种承包方式合同
        /// </summary>
        public bool AgricultureConcordAllowMultiType
        {
            get { return agricultureConcordAllowMultiType; }
            set { agricultureConcordAllowMultiType = value; NotifyPropertyChanged("AgricultureConcordAllowMultiType"); }
        }

        /// <summary>
        /// 设置合同承包地块备注信息
        /// </summary>
        public string LandCommentReplacement
        {
            get { return _landCommentReplacement; }
            set { _landCommentReplacement = value; NotifyPropertyChanged("LandCommentReplacement"); }
        }

        /// <summary>
        /// 单户申请书日期
        /// </summary>
        public bool SingleRequireDate
        {
            get { return singleRequireDate; }
            set { singleRequireDate = value; NotifyPropertyChanged("SingleRequireDate"); }
        }
        /// <summary>
        /// 0-二轮合同面积，1-实测面积，2-确权面积
        /// </summary>
        public int ChooseArea
        {
            get { return choosearea; }
            set { choosearea = value;NotifyPropertyChanged("ChooseArea"); }
        }
        /// <summary>
        /// 设置发包方开头地域级别,0-乡镇级开头，1-区县级开头
        /// </summary>
        public int SendNamePrefixLevel
        {
            get { return _sendNamePrefixLevel; }
            set { _sendNamePrefixLevel = value; NotifyPropertyChanged("SendNamePrefixLevel"); }
        }
        /// <summary>
        /// 设置承包方开头地域级别,0-乡镇级开头，1-区县级开头
        /// </summary>
        public int ContractorNamePrefixLevel
        {
            get { return _contractorNamePrefixLevel; }
            set { _contractorNamePrefixLevel = value; NotifyPropertyChanged("ContractorNamePrefixLevel"); }
        }
        /// <summary>
        /// 0-批量，1-不批量
        /// </summary>
        public int ChooseBatch
        {
            get { return choosebatch; }
            set { choosebatch = value; NotifyPropertyChanged("ChooseBatch"); }
        }
        #endregion

        #region Ctor

        public ContractConcordSettingDefine()
        {
            AgricultureConcordAllowMultiType = true;
            SingleRequireDate = true;
            ChooseArea = 2;
            ChooseBatch = 0;
            LandCommentReplacement = "";
            SendNamePrefixLevel = 0;
            ContractorNamePrefixLevel = 0;
        }

        #endregion
        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractConcordSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractConcordSettingDefine>();
            var section = profile.GetSection<ContractConcordSettingDefine>();
            return section.Settings;
        }
    }
}
