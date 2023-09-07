/*
 * (C) 2020  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账下载设置
    /// </summary>
    public class DownloadSettingDefine : NotifyCDObject
    {
        private bool _DownloadInvestigationData;
        private bool _DownloadRegistrationData;

        /// <summary>
        /// 是否下载调查数据
        /// </summary>
        public bool DownloadInvestigationData
        {
            get { return _DownloadInvestigationData; }
            set { _DownloadInvestigationData = value; NotifyPropertyChanged("DownloadInvestigationData"); }
        }

        /// <summary>
        /// 是否下载登记数据
        /// </summary>
        public bool DownloadRegistrationData
        {
            get { return _DownloadRegistrationData; }
            set { _DownloadRegistrationData = value; NotifyPropertyChanged("DownloadRegistrationData"); }
        }
        public DownloadSettingDefine()
        {
            DownloadInvestigationData = true;
            DownloadRegistrationData = true;
        }

        private static DownloadSettingDefine _DownloadSettingDefine;
        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static DownloadSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<DownloadSettingDefine>();
            var section = profile.GetSection<DownloadSettingDefine>();
            return _DownloadSettingDefine = section.Settings;
        }
    }
}
