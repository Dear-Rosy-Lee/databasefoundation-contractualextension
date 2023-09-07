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
    public class CommonBusinessDefine : NotifyCDObject
    {
        #region Fields

        private string currentDataSource;
        private string currentZoneFullName;
        private string currentZoneFullCode;

        private bool navUniformity;
        private bool registerFormat;

        private string backUpPath;

        #endregion

        #region Properties

        #region 数据源

        /// <summary>
        /// 采用注册模板风格
        /// </summary>
        public string CurrentDataSource
        {
            get { return currentDataSource; }
            set { currentDataSource = value; NotifyPropertyChanged("CurrentDataSource"); }
        }

        #endregion

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

        /// <summary>
        /// 保持导航一致性
        /// </summary>
        public bool NavUniformity
        {
            get { return navUniformity; }
            set { navUniformity = value; NotifyPropertyChanged("NavUniformity"); }
        }

        /// <summary>
        /// 采用注册模板风格
        /// </summary>
        public bool RegisterFormat
        {
            get { return registerFormat; }
            set { registerFormat = value; NotifyPropertyChanged("RegisterFormat"); }
        }

        /// <summary>
        /// 数据库备份所在路径
        /// </summary>
        public string BackUpPath
        {
            get { return backUpPath; }
            set { backUpPath = value; NotifyPropertyChanged("BackUpPath"); }
        }

        #endregion

        #endregion

        #region Ctor

        public CommonBusinessDefine()
        {
            navUniformity= true;
            registerFormat = false;
            currentZoneFullCode = "86";
            currentZoneFullName = "中国";

             backUpPath = "";
             CurrentDataSource = "";
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static CommonBusinessDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter(); //系统配置
            var profile = systemCenter.GetProfile<CommonBusinessDefine>();
            var section = profile.GetSection<CommonBusinessDefine>();
            return  section.Settings;
        }

        #endregion

    }
}
