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
    /// 承包台账上传设置
    /// </summary>
   public class UploadSettingDefine : NotifyCDObject
    {

        private bool uploadInvestigationData;
        private bool initalizeRegistrationData;

        /// <summary>
        /// 是否上传调查数据
        /// </summary>
        public bool UploadInvestigationData
        {
            get { return uploadInvestigationData; }
            set { uploadInvestigationData = value; NotifyPropertyChanged("UploadInvestigationData"); }
        }

        /// <summary>
        /// 是否上传登记数据
        /// </summary>
        public bool InitalizeRegistrationData
        {
            get { return initalizeRegistrationData; }
            set { initalizeRegistrationData = value; NotifyPropertyChanged("InitalizeRegistrationData"); }
        }
        public UploadSettingDefine()
        {
            UploadInvestigationData = true;
            InitalizeRegistrationData = true;
        }

        private static UploadSettingDefine _familyOtherDefine;
        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static UploadSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<UploadSettingDefine>();
            var section = profile.GetSection<UploadSettingDefine>();
            return  _familyOtherDefine = section.Settings;
        }

    }
}
