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
    /// 导入控制点图斑配置实体
    /// </summary>
    public class ImportControlPointDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DisplayLanguage("标识码", IsLanguageName = false)]
        [DescriptionLanguage("标识码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
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
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string FeatureCode
        {
            get { return featureCode; }
            set { featureCode = value; NotifyPropertyChanged("FeatureCode"); }
        }
        private string featureCode;

        /// <summary>
        /// 控制点名称
        /// </summary>
        [DisplayLanguage("控制点名称", IsLanguageName = false)]
        [DescriptionLanguage("控制点名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointName
        {
            get { return pointName; }
            set { pointName = value; NotifyPropertyChanged("PointName"); }
        }
        private string pointName;

        /// <summary>
        /// 控制点点号
        /// </summary>
        [DisplayLanguage("控制点点号", IsLanguageName = false)]
        [DescriptionLanguage("控制点点号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointNumber
        {
            get { return pointNumber; }
            set { pointNumber = value; NotifyPropertyChanged("PointNumber"); }
        }
        private string pointNumber;

        /// <summary>
        /// 控制点类型
        /// </summary>
        [DisplayLanguage("控制点类型", IsLanguageName = false)]
        [DescriptionLanguage("控制点类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointType
        {
            get { return pointType; }
            set { pointType = value; NotifyPropertyChanged("PointType"); }
        }
        private string pointType;

        /// <summary>
        /// 控制点等级
        /// </summary>
        [DisplayLanguage("控制点等级", IsLanguageName = false)]
        [DescriptionLanguage("控制点等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointRank
        {
            get { return pointRank; }
            set { pointRank = value; NotifyPropertyChanged("PointRank"); }
        }
        private string pointRank;

        /// <summary>
        /// 标石类型
        /// </summary>
        [DisplayLanguage("标石类型", IsLanguageName = false)]
        [DescriptionLanguage("标石类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string BsType
        {
            get { return bsType; }
            set { bsType = value; NotifyPropertyChanged("BsType"); }
        }
        private string bsType;

        /// <summary>
        /// 标志类型
        /// </summary>
        [DisplayLanguage("标志类型", IsLanguageName = false)]
        [DescriptionLanguage("标志类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string BzType
        {
            get { return bzType; }
            set { bzType = value; NotifyPropertyChanged("BzType"); }
        }
        private string bzType;

        /// <summary>
        /// 控制点状态
        /// </summary>
        [DisplayLanguage("控制点状态", IsLanguageName = false)]
        [DescriptionLanguage("控制点状态", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PointState
        {
            get { return pointState; }
            set { pointState = value; NotifyPropertyChanged("PointState"); }
        }
        private string pointState;

        /// <summary>
        /// 点之记
        /// </summary>
        [DisplayLanguage("点之记", IsLanguageName = false)]
        [DescriptionLanguage("点之记", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Dzj
        {
            get { return dzj; }
            set { dzj = value; NotifyPropertyChanged("Dzj"); }
        }
        private string dzj;

        /// <summary>
        /// X_2000a
        /// </summary>
        [DisplayLanguage("X_2000a", IsLanguageName = false)]
        [DescriptionLanguage("X_2000a", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string X2000
        {
            get { return x2000; }
            set { x2000 = value; NotifyPropertyChanged("X2000"); }
        }
        private string x2000;

        /// <summary>
        /// Y_2000a
        /// </summary>
        [DisplayLanguage("Y_2000a", IsLanguageName = false)]
        [DescriptionLanguage("Y_2000a", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Y2000
        {
            get { return y2000; }
            set { y2000 = value; NotifyPropertyChanged("Y2000"); }
        }
        private string y2000;

        /// <summary>
        /// X(E)_XA80a
        /// </summary>
        [DisplayLanguage("X(E)_XA80a", IsLanguageName = false)]
        [DescriptionLanguage("X(E)_XA80a", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string X80
        {
            get { return x80; }
            set { x80 = value; NotifyPropertyChanged("X80"); }
        }
        private string x80;

        /// <summary>
        /// Y(E)_XA80a
        /// </summary>
        [DisplayLanguage("Y(E)_XA80a", IsLanguageName = false)]
        [DescriptionLanguage("Y(E)_XA80a", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "控制点信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string Y80
        {
            get { return y80; }
            set { y80 = value; NotifyPropertyChanged("Y80"); }
        }
        private string y80;

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportControlPointDefine()
        {
            Code = "None";
            FeatureCode = "None";
            PointName = "None";
            PointNumber = "None";
            PointType = "None";
            PointRank = "None";
            BsType = "None";
            BzType = "None";
            PointState = "None";
            Dzj = "None";
            X2000 = "None";
            Y2000 = "None";
            X80 = "None";
            Y80 = "None";
        }

        #endregion
    }
}
