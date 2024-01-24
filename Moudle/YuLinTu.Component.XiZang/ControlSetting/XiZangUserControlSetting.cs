/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.XiZangLZ
{
   public class XiZangUserControlSettingDefine : NotifyCDObject
    {
        private bool isFilterDotInLandSurveyDoc;

        /// <summary>
        /// 是否在地块调查表过滤界址点
        /// </summary>
        public bool IsFilterDotInLandSurveyDoc
        {
            get { return isFilterDotInLandSurveyDoc; }
            set { isFilterDotInLandSurveyDoc = value; NotifyPropertyChanged("IsFilterDotInLandSurveyDoc"); }
        }

        public XiZangUserControlSettingDefine()
        {
            IsFilterDotInLandSurveyDoc = true;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static XiZangUserControlSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<XiZangUserControlSettingDefine>();
            var section = profile.GetSection<XiZangUserControlSettingDefine>();
            return section.Settings;
        }


    }
}
