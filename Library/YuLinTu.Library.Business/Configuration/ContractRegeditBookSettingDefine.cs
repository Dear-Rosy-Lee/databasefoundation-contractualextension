/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包权证常规设置类
    /// </summary>
    public class ContractRegeditBookSettingDefine : NotifyCDObject
    {
        #region Properties

        #region 基本设定

        [DisplayLanguage("权证扩展页采用excel格式")]
        [DescriptionLanguage("土地承包经营权证书扩展是否采用excel格式")]
        [PropertyDescriptor(Catalog = "基本设定")]
        public bool WarrantExtendByExcel
        {
            get { return _WarrantExtendByExcel; }
            set { _WarrantExtendByExcel = value; NotifyPropertyChanged(() => WarrantExtendByExcel); }
        }

        private bool _WarrantExtendByExcel;

        #endregion 基本设定

        #region 流水号设置

        [DisplayLanguage("最小值设置")]
        [DescriptionLanguage("最小值设置")]
        [PropertyDescriptor(Catalog = "流水号设置")]
        [Range(1, 999999)]
        public int MinNumber
        {
            get { return _MinNumber; }
            set { _MinNumber = value; NotifyPropertyChanged(() => MinNumber); }
        }

        private int _MinNumber;

        [DisplayLanguage("最大值设置")]
        [DescriptionLanguage("最大值设置")]
        [PropertyDescriptor(Catalog = "流水号设置")]
        [Range(1, 999999)]
        public int MaxNumber
        {
            get { return _MaxNumber; }
            set { _MaxNumber = value; NotifyPropertyChanged(() => MaxNumber); }
        }

        private int _MaxNumber;

        #endregion 流水号设置

        #region 证书设置

        [DisplayLanguage("登记簿空间坐标代号")]
        [DescriptionLanguage("登记簿空间坐标代号")]
        [PropertyDescriptor(Catalog = "证书设置")]
        public string RegisterSpaceCode
        {
            get { return _RegisterSpaceCode; }
            set { _RegisterSpaceCode = value; NotifyPropertyChanged(() => RegisterSpaceCode); }
        }

        private string _RegisterSpaceCode;

        [DisplayLanguage("证书编号字母")]
        [DescriptionLanguage("证书编号字母")]
        [PropertyDescriptor(Catalog = "证书设置")]
        public string WarrantNumberLetter
        {
            get { return _WarrantNumberLetter; }
            set { _WarrantNumberLetter = value; NotifyPropertyChanged(() => WarrantNumberLetter); }
        }

        private string _WarrantNumberLetter;

        #endregion 证书设置


        #region 登记簿设置

        [Enabled(false)]
        [DisplayLanguage("删除首页")]
        [DescriptionLanguage("删除首页")]
        [PropertyDescriptor(Catalog = "登记簿设置")]
        public bool IsDeleteIndexPage
        {
            get { return _IsDeleteIndexPage; }
            set { _IsDeleteIndexPage = value; NotifyPropertyChanged(() => IsDeleteIndexPage); }
        }

        private bool _IsDeleteIndexPage;

        [Enabled(false)]
        [DisplayLanguage("删除附记页")]
        [DescriptionLanguage("删除附记页")]
        [PropertyDescriptor(Catalog = "登记簿设置")]
        public bool IsDeleteRegistratorPage
        {
            get { return _IsDeleteRegistratorPage; }
            set { _IsDeleteRegistratorPage = value; NotifyPropertyChanged(() => IsDeleteRegistratorPage); }
        }

        private bool _IsDeleteRegistratorPage;

        [DisplayLanguage("删除附图页")]
        [DescriptionLanguage("删除附图页")]
        [PropertyDescriptor(Catalog = "登记簿设置")]
        public bool IsDeleteDrawingPage
        {
            get { return _IsDeleteDrawingPage; }
            set { _IsDeleteDrawingPage = value; NotifyPropertyChanged(() => IsDeleteDrawingPage); }
        }

        private bool _IsDeleteDrawingPage;

        #endregion 登记簿设置

        #endregion Properties

        #region Ctor

        public ContractRegeditBookSettingDefine()
        {
            WarrantExtendByExcel = false;

            MinNumber = 1;
            MaxNumber = 999999;
        }

        #endregion Ctor

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractRegeditBookSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractRegeditBookSettingDefine>();
            var section = profile.GetSection<ContractRegeditBookSettingDefine>();
            return section.Settings;
        }
    }
}