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
    /// 承包方其他设置实体类
    /// </summary>
    public class FamilyDefine : NotifyCDObject
    {
        #region Properties

        /// <summary>
        /// 统计时只统计家庭相关信息
        /// </summary>
        [DisplayLanguage("统计时只统计家庭相关信息", IsLanguageName = false)]
        [DescriptionLanguage("统计时只统计家庭相关信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
             Builder = typeof(YuLinTu.Library.Business.PropertyDescriptorBoolean))]
        public bool FamilyRelationValue
        {
            get { return familyRelationValue; }
            set { familyRelationValue = value; NotifyPropertyChanged("FamilyRelationValue"); }
        }
        private bool familyRelationValue;

        /// <summary>
        /// 设置公示表中只公示家庭户信息
        /// </summary>
        [DisplayLanguage("设置公示表中只公示家庭户信息", IsLanguageName = false)]
        [DescriptionLanguage("设置公示表中只公示家庭户信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyShowValue
        {
            get { return familyShowValue; }
            set { familyShowValue = value; NotifyPropertyChanged("FamilyShowValue"); }
        }
        private bool familyShowValue;

        /// <summary>
        /// 设置户主代表声明书日期
        /// </summary>
        [DisplayLanguage("设置户主代表声明书日期", IsLanguageName = false)]
        [DescriptionLanguage("设置户主代表声明书日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyInstructionDate
        {
            get { return familyInstructionDate; }
            set { familyInstructionDate = value; NotifyPropertyChanged("FamilyInstructionDate"); }
        }
        private bool familyInstructionDate;

        /// <summary>
        /// 设置委托代理声明书日期
        /// </summary>
        [DisplayLanguage("设置委托代理声明书日期", IsLanguageName = false)]
        [DescriptionLanguage("设置委托代理声明书日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ProxyDefineDate
        {
            get { return proxyDefineDate; }
            set { proxyDefineDate = value; NotifyPropertyChanged("ProxyDefineDate"); }
        }
        private bool proxyDefineDate;

        /// <summary>
        /// 设置公示无异议说明书日期
        /// </summary>
        [DisplayLanguage("设置公示无异议说明书日期", IsLanguageName = false)]
        [DescriptionLanguage("设置公示无异议说明书日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool UniqueInstructionDate
        {
            get { return uniqueInstructionDate; }
            set { uniqueInstructionDate = value; NotifyPropertyChanged("UniqueInstructionDate"); }
        }
        private bool uniqueInstructionDate;

        /// <summary>
        /// 设置测绘申请书日期
        /// </summary>
        [DisplayLanguage("设置测绘申请书日期", IsLanguageName = false)]
        [DescriptionLanguage("设置测绘申请书日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool GeoInstructionDate
        {
            get { return geoInstructionDate; }
            set { geoInstructionDate = value; NotifyPropertyChanged("GeoInstructionDate"); }
        }
        private bool geoInstructionDate;

        /// <summary>
        /// 按户号排列导出表格
        /// </summary>
        [DisplayLanguage("按户号排列导出表格", IsLanguageName = false)]
        [DescriptionLanguage("按户号排列导出表格", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ImportTableByFamilyID
        {
            get { return importTableByFamilyID; }
            set { importTableByFamilyID = value; NotifyPropertyChanged("ImportTableByFamilyID"); }
        }
        private bool importTableByFamilyID;

        /// <summary>
        /// 导出表格中编号按户号输出
        /// </summary>
        [DisplayLanguage("导出表格中编号按户号输出", IsLanguageName = false)]
        [DescriptionLanguage("导出表格中编号按户号输出", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberOuptutByFamilyID
        {
            get { return numberOuptutByFamilyID; }
            set { numberOuptutByFamilyID = value; NotifyPropertyChanged("NumberOuptutByFamilyID"); }
        }
        private bool numberOuptutByFamilyID;

        /// <summary>
        /// 允许调查表中身份证号码重复存在
        /// </summary>
        [DisplayLanguage("允许调查表中身份证号码重复存在", IsLanguageName = false)]
        [DescriptionLanguage("允许调查表中身份证号码重复存在", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方设置", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberIcnValueRepeat
        {
            get { return numberIcnValueRepeat; }
            set { numberIcnValueRepeat = value; NotifyPropertyChanged("NumberIcnValueRepeat"); }
        }
        private bool numberIcnValueRepeat;

        #endregion

        #region Ctor

        public FamilyDefine()
        {
            FamilyShowValue = true;
            FamilyRelationValue = true;
            FamilyInstructionDate = false;
            ProxyDefineDate = true;
            UniqueInstructionDate = false;
            GeoInstructionDate = false;
            ImportTableByFamilyID = false;
            NumberOuptutByFamilyID = true;
            NumberIcnValueRepeat = false;
        }

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static FamilyDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<FamilyDefine>();
            var section = profile.GetSection<FamilyDefine>();
            return section.Settings;
        }

        #endregion
    }
}
