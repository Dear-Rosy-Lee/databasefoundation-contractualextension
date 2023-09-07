/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 导入基本农田保护区图斑配置实体
    /// </summary>
    public class ImportFarmLandConserveDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DisplayLanguage("标识码", IsLanguageName = false)]
        [DescriptionLanguage("标识码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "基本农田保护区信息", Gallery = "",
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
        [PropertyDescriptor(Catalog = "基本农田保护区信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string FeatureCode
        {
            get { return featureCode; }
            set { featureCode = value; NotifyPropertyChanged("FeatureCode"); }
        }
        private string featureCode;

        /// <summary>
        ///保护区编号
        /// </summary>
        [DisplayLanguage("保护区编号", IsLanguageName = false)]
        [DescriptionLanguage("保护区编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "基本农田保护区信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ConserveNumber
        {
            get { return conserveNumber; }
            set { conserveNumber = value; NotifyPropertyChanged("ConserveNumber"); }
        }
        private string conserveNumber;

        /// <summary>
        /// 基本农田面积
        /// </summary>
        [DisplayLanguage("基本农田面积", IsLanguageName = false)]
        [DescriptionLanguage("基本农田面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "基本农田保护区信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string FarmLandArea
        {
            get { return farmLandArea; }
            set { farmLandArea = value; NotifyPropertyChanged("FarmLandArea"); }
        }
        private string farmLandArea;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportFarmLandConserveDefine()
        {
            Code = "None";
            FeatureCode = "None";
            ConserveNumber = "None";
            FarmLandArea = "None";
        }

        #endregion
    }
}
