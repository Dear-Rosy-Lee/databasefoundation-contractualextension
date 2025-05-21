/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 导入区域界线图斑配置实体
    /// </summary>
    public class ImportZoneBoundaryDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DisplayLanguage("标识码", IsLanguageName = false)]
        [DescriptionLanguage("标识码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Code
        {
            get { return code; }
            set { code = value; NotifyPropertyChanged("Code"); }
        }
        private string code;

        /// <summary>
        ///要素代码
        /// </summary>
        [DisplayLanguage("要素代码", IsLanguageName = false)]
        [DescriptionLanguage("要素代码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string FeatureCode
        {
            get { return featureCode; }
            set { featureCode = value; NotifyPropertyChanged("FeatureCode"); }
        }
        private string featureCode;

        /// <summary>
        /// 界线类型
        /// </summary>
        [DisplayLanguage("界线类型", IsLanguageName = false)]
        [DescriptionLanguage("界线类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string BoundaryLineType
        {
            get { return boundaryLineType; }
            set { boundaryLineType = value; NotifyPropertyChanged("BoundaryLineType"); }
        }
        private string boundaryLineType;

        /// <summary>
        /// 界线性质
        /// </summary>
        [DisplayLanguage("界线性质", IsLanguageName = false)]
        [DescriptionLanguage("界线性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string BoundaryLineNature
        {
            get { return boundaryLineNature; }
            set { boundaryLineNature = value; NotifyPropertyChanged("BoundaryLineNature"); }
        }
        private string boundaryLineNature;
        /// <summary>
        /// 地域名称
        /// </summary>
        [DisplayLanguage("地域名称", IsLanguageName = false)]
        [DescriptionLanguage("地域名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; NotifyPropertyChanged("ZoneName"); }
        }
        private string zoneName;
        /// <summary>
        /// 地域编码
        /// </summary>
        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "区域界线信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ZoneCode
        {
            get { return zoneCode; }
            set { zoneCode = value; NotifyPropertyChanged("ZoneCode"); }
        }
        private string zoneCode;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportZoneBoundaryDefine()
        {
            Code = "None";
            FeatureCode = "None";
            BoundaryLineType = "None";
            BoundaryLineNature = "None";
            ZoneCode = "None";
            ZoneName = "None";
        }

        #endregion
    }
}
