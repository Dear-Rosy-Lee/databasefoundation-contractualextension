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
    /// 二轮台账配置信息实体类
    /// </summary>
    public class SecondTableExportDefine : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        /// 导出勘界调查表不以二轮台账内容填充确权登记内容
        /// </summary>
        [DisplayLanguage("不以二轮台账内容填充确权登记内容", IsLanguageName = false)]
        [DescriptionLanguage("导出勘界调查表不以二轮台账内容填充确权登记内容", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮台账表导出设置", Catalog = "",
             Builder = typeof(PropertyDescriptorBoolean))]
        public bool ExportBoundarySettleTable
        {
            get { return exportBoundarySettleTable; }
            set { exportBoundarySettleTable = value; NotifyPropertyChanged("ExportBoundarySettleTable"); }
        }
        private bool exportBoundarySettleTable;

        /// <summary>
        /// 输出数据时四至显示东南西北
        /// </summary>
        [DisplayLanguage("输出数据时四至显示东南西北", IsLanguageName = false)]
        [DescriptionLanguage("输出数据时四至显示东南西北", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮台账表导出设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ShowSecondTableDataNeighborWithDirection
        {
            get { return showSecondTableDataNeighborWithDirection; }
            set { showSecondTableDataNeighborWithDirection = value; NotifyPropertyChanged("ShowSecondTableDataNeighborWithDirection"); }
        }
        private bool showSecondTableDataNeighborWithDirection;


        #endregion

        #region Ctor

        /// <summary>
        /// 默认值
        /// </summary>
        public SecondTableExportDefine()
        {
            showSecondTableDataNeighborWithDirection = true;
            exportBoundarySettleTable = true;
        }


        #endregion

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static SecondTableExportDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<SecondTableExportDefine>();
            var section = profile.GetSection<SecondTableExportDefine>();
            return  section.Settings;
        }
    }
}
