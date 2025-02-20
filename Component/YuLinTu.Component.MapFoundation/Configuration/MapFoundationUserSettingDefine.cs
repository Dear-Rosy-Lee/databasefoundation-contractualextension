/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 鱼鳞图用户设置实体类
    /// </summary>
    public class MapFoundationUserSettingDefine : NotifyCDObject
    {
        #region Fields

        private bool ishandleGraphicToPu;
        private bool handleGraphicToPuUseMD;
        private bool graphicToPuSaveNewData;
        private bool graphicToPuSavebfData;
        private bool isUseSelectZoneShowData;
        #endregion

        #region Properties

        //是否在绘制面状要素时处理拓扑(从所绘制地块中檫除相交部分、从相交地块中檫除相交部分)
        public bool IshandleGraphicToPu
        {
            get { return ishandleGraphicToPu; }
            set { ishandleGraphicToPu = value;NotifyPropertyChanged("IshandleGraphicToPu"); }
        }

        ///使用弹出框处理绘制面状拓扑选项      
        public bool HandleGraphicToPuUseMD
        {
            get { return handleGraphicToPuUseMD; }
            set { handleGraphicToPuUseMD = value; NotifyPropertyChanged("HandleGraphicToPuUseMD"); }
        }

        //使用弹出框处理绘制面状拓扑选项-保留新绘制数据
        public bool GraphicToPuSaveNewData
        {
            get { return graphicToPuSaveNewData; }
            set { graphicToPuSaveNewData = value; NotifyPropertyChanged("GraphicToPuSaveNewData"); }
        }

        //使用弹出框处理绘制面状拓扑选项-保留以前数据
        public bool GraphicToPuSavebfData
        {
            get { return graphicToPuSavebfData; }
            set { graphicToPuSavebfData = value; NotifyPropertyChanged("GraphicToPuSavebfData"); }
        }

        //是否根据选择的行政地域显示要素
        public bool IsUseSelectZoneShowData
        {
            get { return isUseSelectZoneShowData; }
            set { isUseSelectZoneShowData = value; NotifyPropertyChanged("IsUseSelectZoneShowData"); }
        }
        #endregion

        #region Ctor
        public MapFoundationUserSettingDefine()
        {
            IshandleGraphicToPu = false;
            HandleGraphicToPuUseMD = true;
            GraphicToPuSaveNewData = false;
            GraphicToPuSavebfData = true;
            IsUseSelectZoneShowData = false;
        }
        #endregion

       public static MapFoundationUserSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<MapFoundationUserSettingDefine>();
            var section = profile.GetSection<MapFoundationUserSettingDefine>();
            return section.Settings;
        }
    }
}
