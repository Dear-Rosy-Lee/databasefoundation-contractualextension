/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 系统管理-服务设置信息
    /// </summary>
    public class ServiceSetDefine : NotifyCDObject
    {
        #region Fields

        private bool useSafeConfirm;
        private string businessSecurityAddress;
        private string businessLoadAddress;
        private string businessDataAddress;
        private string surveySecurityAddress;
        private string surveyLoadAddress;

        #endregion

        #region Properties

        /// <summary>
        /// 启用安全服务验证
        /// </summary>
        public bool UseSafeConfirm
        {
            get { return useSafeConfirm; }
            set { useSafeConfirm = value; NotifyPropertyChanged("UseSafeConfirm"); }
        }

        /// <summary>
        /// 安全服务地址
        /// </summary>
        public string BusinessSecurityAddress
        {
            get { return businessSecurityAddress; }
            set { businessSecurityAddress = value; NotifyPropertyChanged("BusinessSecurityAddress"); }
        }

        /// <summary>
        /// 登录服务地址
        /// </summary>
        public string BusinessLoadAddress
        {
            get { return businessLoadAddress; }
            set { businessLoadAddress = value; NotifyPropertyChanged("BusinessLoadAddress"); }
        }

        /// <summary>
        /// 数据服务器地址
        /// </summary>
        public string BusinessDataAddress
        {
            get { return businessDataAddress; }
            set { businessDataAddress = value; NotifyPropertyChanged("BusinessDataAddress"); }
        }

        /// <summary>
        /// 调查业务安全服务地址
        /// </summary>
        public string SurveySecurityAddress
        {
            get { return surveySecurityAddress; }
            set { surveySecurityAddress = value; NotifyPropertyChanged("SurveySecurityAddress"); }
        }

        /// <summary>
        /// 调查业务登录服务地址
        /// </summary>
        public string SurveyLoadAddress
        {
            get { return surveyLoadAddress; }
            set { surveyLoadAddress = value; NotifyPropertyChanged("SurveySecurityAddress"); }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ServiceSetDefine()
        {
            useSafeConfirm = false;
            businessSecurityAddress = TheApp.Current.TryGetValue<String>("DefaultBusinessSecurityAddress", "");
            businessDataAddress= TheApp.Current.TryGetValue<String>("DefaultBusinessDataAddress", "");
        }

        #endregion

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ServiceSetDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ServiceSetDefine>();
            var section = profile.GetSection<ServiceSetDefine>();
            return section.Settings;
        }

    }
}
