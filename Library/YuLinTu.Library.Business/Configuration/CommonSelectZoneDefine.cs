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
    /// 常用业务信息设置
    /// </summary>
    public class CommonSelectZoneDefine : NotifyCDObject
    {
        #region Fields
        
        private string currentZoneFullName;
        private string currentZoneFullCode;     

        #endregion

        #region Properties

        #region 导航选项

        /// <summary>
        /// 选择地域全名称
        /// </summary>
        public string CurrentZoneFullName
        {
            get { return currentZoneFullName; }
            set { currentZoneFullName = value; NotifyPropertyChanged("CurrentZoneFullName"); }
        }

        /// <summary>
        /// 选择地域全编码
        /// </summary>
        public string CurrentZoneFullCode
        {
            get { return currentZoneFullCode; }
            set { currentZoneFullCode = value; NotifyPropertyChanged("CurrentZoneFullCode"); }
        }

        #endregion

        #endregion

        #region Ctor

        public CommonSelectZoneDefine()
        {
            currentZoneFullCode = "86";
            currentZoneFullName = "中国";
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static CommonSelectZoneDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter(); //系统配置
            var profile = systemCenter.GetProfile<CommonSelectZoneDefine>();
            var section = profile.GetSection<CommonSelectZoneDefine>();
            return  section.Settings;
        }

        #endregion

    }
}
