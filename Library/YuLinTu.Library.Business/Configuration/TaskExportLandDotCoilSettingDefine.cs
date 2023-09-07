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
    /// 任务-导出确权成果库地块与界址点、线匹配设置
    /// </summary>
    public class TaskExportLandDotCoilSettingDefine: NotifyCDObject
    {
        #region Propertys

        /// <summary>
        ///农户
        /// </summary>
        [DisplayLanguage("农户", IsLanguageName = false)]
        [DescriptionLanguage("农户", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方类型",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool Farmer
        {
            get { return farmer; }
            set { farmer = value; NotifyPropertyChanged("Farmer"); }
        }
        private bool farmer;

        /// <summary>
        ///个人
        /// </summary>
        [DisplayLanguage("个人", IsLanguageName = false)]
        [DescriptionLanguage("个人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方类型",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool Personal
        {
            get { return personal; }
            set { personal = value; NotifyPropertyChanged("Personal"); }
        }
        private bool personal;

        /// <summary>
        ///单位
        /// </summary>
        [DisplayLanguage("单位", IsLanguageName = false)]
        [DescriptionLanguage("单位", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方类型",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool Unit
        {
            get { return unit; }
            set { unit = value; NotifyPropertyChanged("Unit"); }
        }
        private bool unit;

        /// <summary>
        ///承包地块
        /// </summary>
        [DisplayLanguage("承包地块", IsLanguageName = false)]
        [DescriptionLanguage("承包地块", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractLand
        {
            get { return contractLand; }
            set { contractLand = value; NotifyPropertyChanged("ContractLand"); }
        }
        private bool contractLand;

        /// <summary>
        ///自留地
        /// </summary>
        [DisplayLanguage("自留地", IsLanguageName = false)]
        [DescriptionLanguage("自留地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PrivateLand
        {
            get { return privateLand; }
            set { privateLand = value; NotifyPropertyChanged("NameValue"); }
        }
        private bool privateLand;

        /// <summary>
        ///机动地
        /// </summary>
        [DisplayLanguage("机动地", IsLanguageName = false)]
        [DescriptionLanguage("机动地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool MotorizeLand
        {
            get { return motorizeLand; }
            set { motorizeLand = value; NotifyPropertyChanged("MotorizeLand"); }
        }
        private bool motorizeLand;

        /// <summary>
        ///开荒地
        /// </summary>
        [DisplayLanguage("开荒地", IsLanguageName = false)]
        [DescriptionLanguage("开荒地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool WasteLand
        {
            get { return wasteLand; }
            set { wasteLand = value; NotifyPropertyChanged("WasteLand"); }
        }
        private bool wasteLand;

        /// <summary>
        ///其他集体土地
        /// </summary>
        [DisplayLanguage("其他集体土地", IsLanguageName = false)]
        [DescriptionLanguage("其他集体土地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CollectiveLand
        {
            get { return collectiveLand; }
            set { collectiveLand = value; NotifyPropertyChanged("CollectiveLand"); }
        }
        private bool collectiveLand;

        /// <summary>
        ///经济地
        /// </summary>
        [DisplayLanguage("经济地", IsLanguageName = false)]
        [DescriptionLanguage("经济地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool EncollecLand
        {
            get { return encollecLand; }
            set { encollecLand = value; NotifyPropertyChanged("EncollecLand"); }
        }
        private bool encollecLand;

        /// <summary>
        ///饲料地
        /// </summary>
        [DisplayLanguage("饲料地", IsLanguageName = false)]
        [DescriptionLanguage("饲料地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FeedLand
        {
            get { return feedLand; }
            set { feedLand = value; NotifyPropertyChanged("FeedLand"); }
        }
        private bool feedLand;

        /// <summary>
        ///撂荒地
        /// </summary>
        [DisplayLanguage("撂荒地", IsLanguageName = false)]
        [DescriptionLanguage("撂荒地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块类别",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AbandonedLand
        {
            get { return abandonedLand; }
            set { abandonedLand = value; NotifyPropertyChanged("AbandonedLand"); }
        }
        private bool abandonedLand;
       

        #endregion

        #region Ctor
        public TaskExportLandDotCoilSettingDefine()
        {
            //默认勾选承包方中农户和承包地块
            Farmer = true;
            Personal = true;
            Unit = true;

            ContractLand = true;
            PrivateLand = true;
            MotorizeLand = true;
            WasteLand = true;
            CollectiveLand = true;
            EncollecLand = true;
            FeedLand = true;
            AbandonedLand = true;
        }
        #endregion

        private static TaskExportLandDotCoilSettingDefine _familyOtherDefine;
        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static TaskExportLandDotCoilSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<TaskExportLandDotCoilSettingDefine>();
            var section = profile.GetSection<TaskExportLandDotCoilSettingDefine>();
            return _familyOtherDefine = section.Settings;
        }

    }
}
