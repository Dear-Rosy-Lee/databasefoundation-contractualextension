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
    /// 导入界址点图斑设置实体
    /// </summary>
    public class ImportBoundaryAddressDotDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        ///地域代码
        /// </summary>
        [DisplayLanguage("地域代码", IsLanguageName = false)]
        [DescriptionLanguage("地域代码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ZoneCodeIndex
        {
            get { return zoneCodeIndex; }
            set { zoneCodeIndex = value; NotifyPropertyChanged("ZoneCodeIndex"); }
        }
        private string zoneCodeIndex;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandNumberIndex
        {
            get { return landNumberIndex; }
            set { landNumberIndex = value; NotifyPropertyChanged("LandNumberIndex"); }
        }
        private string landNumberIndex;

        /// <summary>
        ///界址点号
        /// </summary>
        [DisplayLanguage("界址点号", IsLanguageName = false)]
        [DescriptionLanguage("界址点号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string DotNumberIndex
        {
            get { return dotNumberIndex; }
            set { dotNumberIndex = value; NotifyPropertyChanged("DotNumberIndex"); }
        }
        private string dotNumberIndex;

        /// <summary>
        ///界址点类型
        /// </summary>
        [DisplayLanguage("界址点类型", IsLanguageName = false)]
        [DescriptionLanguage("界址点类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string DotTypeIndex
        {
            get { return dotTypeIndex; }
            set { dotTypeIndex = value; NotifyPropertyChanged("DotTypeIndex"); }
        }
        private string dotTypeIndex;

        /// <summary>
        /// 界标类型
        /// </summary>
        [DisplayLanguage("界标类型", IsLanguageName = false)]
        [DescriptionLanguage("界标类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandMarkTypeIndex
        {
            get { return landMarkTypeIndex; }
            set { landMarkTypeIndex = value; NotifyPropertyChanged("LandMarkTypeIndex"); }
        }
        private string landMarkTypeIndex;

        /// <summary>
        /// 关键节点
        /// </summary>
        [DisplayLanguage("关键节点", IsLanguageName = false)]
        [DescriptionLanguage("关键节点", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string KeyDotIndex
        {
            get { return keyDotIndex; }
            set { keyDotIndex = value; NotifyPropertyChanged("KeyDotIndex"); }
        }
        private string keyDotIndex;

        /// <summary>
        /// 统编号
        /// </summary>
        [DisplayLanguage("统编号", IsLanguageName = false)]
        [DescriptionLanguage("统编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string UnitDotNumberIndex
        {
            get { return unitDotNumberIndex; }
            set { unitDotNumberIndex = value; NotifyPropertyChanged("UnitDotNumberIndex"); }
        }
        private string unitDotNumberIndex;

        /// <summary>
        /// 使用权ID
        /// </summary>
        [DisplayLanguage("使用权ID", IsLanguageName = false)]
        [DescriptionLanguage("使用权ID", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandIdIndex
        {
            get { return landId; }
            set { landId = value; NotifyPropertyChanged("LandIdIndex"); }
        }
        private string landId;

        /// <summary>
        /// ID
        /// </summary>
        [DisplayLanguage("ID", IsLanguageName = false)]
        [DescriptionLanguage("ID", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "界址点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointIdIndex
        {
            get { return pointId; }
            set { pointId = value; NotifyPropertyChanged("PointIdIndex"); }
        }
        private string pointId;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportBoundaryAddressDotDefine()
        {
            LandNumberIndex = "None";
            DotNumberIndex = "None";
            DotTypeIndex = "None";
            LandMarkTypeIndex = "None";
            ZoneCodeIndex = "None";
            keyDotIndex = "None";
            unitDotNumberIndex = "None";
            landId = "None";
            pointId = "None";
        }

        #endregion


        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ImportBoundaryAddressDotDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportBoundaryAddressDotDefine>();
            var section = profile.GetSection<ImportBoundaryAddressDotDefine>();
            return section.Settings;
        }

    }
}
