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
    /// 行政地域设置实体类
    /// </summary>
    public class ZoneDefine : NotifyCDObject
    {
        #region Fields

        private bool clearData;
        private bool intiallData;
        private bool shapeToBase;
        private bool exportExcel;

        #endregion

        #region Properties

        /// <summary>
        /// 清空数据设置
        /// </summary>
        public bool ClearData
        {
            get { return clearData; }
            set { clearData = value; NotifyPropertyChanged("ClearData"); }
        }

        /// <summary>
        /// 初始数据设置
        /// </summary>
        public bool IntiallData
        {
            get { return intiallData; }
            set { intiallData = value; NotifyPropertyChanged("IntiallData"); }
        }

        /// <summary>
        /// 图斑入库设置
        /// </summary>
        public bool ShapeToBase
        {
            get { return shapeToBase; }
            set { shapeToBase = value; NotifyPropertyChanged("ShapeToBase"); }
        }

        /// <summary>
        /// 使用标准编码
        /// </summary>
        public bool UseStandCode
        {
            get { return exportExcel; }
            set { exportExcel = value; NotifyPropertyChanged("UseStandCode"); }
        }

        #endregion

        #region Ctor

        public ZoneDefine()
        {
            clearData = false;
            intiallData = true;
            shapeToBase = true;
            exportExcel = true;
        }

        ///private static ZoneDefine _configDefine;
        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ZoneDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            return section.Settings;
        }

        #endregion
    }
}
