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
    /// 单户调查表设置实体
    /// </summary>
   public class SingleFamilySurveyDefine: NotifyCDObject
    {
        #region Propertys

        /// <summary>
        /// 地类
        /// </summary>
        [DisplayLanguage("地类", IsLanguageName = false)]
        [DescriptionLanguage("地类", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮台账信息设置", 
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandType
        {
            get { return secondLandType; }
            set { secondLandType = value; NotifyPropertyChanged("SecondLandType"); }
        }
        private bool secondLandType;

        /// <summary>
        /// 地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandName
        {
            get { return secondLandName; }
            set { secondLandName = value; NotifyPropertyChanged("SecondLandName"); }
        }
        private bool secondLandName;

        /// <summary>
        /// 四至
        /// </summary>
        [DisplayLanguage("四至", IsLanguageName = false)]
        [DescriptionLanguage("四至", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandNeighbor
        {
            get { return secondLandNeighbor; }
            set { secondLandNeighbor = value; NotifyPropertyChanged("SecondLandNeighbor"); }
        }
        private bool secondLandNeighbor;

        /// <summary>
        /// 地类
        /// </summary>
        [DisplayLanguage("地类", IsLanguageName = false)]
        [DescriptionLanguage("地类", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandType
        {
            get { return landType; }
            set { landType = value; NotifyPropertyChanged("LandType"); }
        }
        private bool landType;

        /// <summary>
        /// 四至
        /// </summary>
        [DisplayLanguage("四至", IsLanguageName = false)]
        [DescriptionLanguage("四至", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandNeighbor
        {
            get { return landNeighbor; }
            set { landNeighbor = value; NotifyPropertyChanged("LandNeighbor"); }
        }
        private bool landNeighbor;

        /// <summary>
        /// 承包方式
        /// </summary>
        [DisplayLanguage("承包方式", IsLanguageName = false)]
        [DescriptionLanguage("承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandContractMode
        {
            get { return landContractMode; }
            set { landContractMode = value; NotifyPropertyChanged("LandContractMode"); }
        }
        private bool landContractMode;

        /// <summary>
        /// 是否流转
        /// </summary>
        [DisplayLanguage("是否流转", IsLanguageName = false)]
        [DescriptionLanguage("是否流转", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsTransfer
        {
            get { return isTransfer; }
            set { isTransfer = value; NotifyPropertyChanged("IsTransfer"); }
        }
        private bool isTransfer;

        /// <summary>
        /// 流转方式
        /// </summary>
        [DisplayLanguage("流转方式", IsLanguageName = false)]
        [DescriptionLanguage("流转方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransferMode
        {
            get { return transferMode; }
            set { transferMode = value; NotifyPropertyChanged("TransferMode"); }
        }
        private bool transferMode;

        /// <summary>
        /// 流转期限
        /// </summary>
        [DisplayLanguage("流转期限", IsLanguageName = false)]
        [DescriptionLanguage("流转期限", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransferTerm
        {
            get { return transferTerm; }
            set { transferTerm = value; NotifyPropertyChanged("TransferTerm"); }
        }
        private bool transferTerm;

        /// <summary>
        /// 种植类型
        /// </summary>
        [DisplayLanguage("种植类型", IsLanguageName = false)]
        [DescriptionLanguage("种植类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandPlatType
        {
            get { return landPlatType; }
            set { landPlatType = value; NotifyPropertyChanged("LandPlatType"); }
        }
        private bool landPlatType;

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包台账信息设置",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandComment
        {
            get { return landComment; }
            set { landComment = value; NotifyPropertyChanged("LandComment"); }
        }
        private bool landComment;
               

        #endregion

        #region Ctor

        public SingleFamilySurveyDefine()
        {
            secondLandType = true;
            secondLandName = true;
            secondLandNeighbor = true;

            landType = true;
            landNeighbor = true;
            landContractMode = true;

            isTransfer = false;
            transferMode = false;
            transferTerm = false;

            landPlatType = false;
            landComment = false; 
        }
        #endregion

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static SingleFamilySurveyDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<SingleFamilySurveyDefine>();
            var section = profile.GetSection<SingleFamilySurveyDefine>();
            return  section.Settings;
        }
    }
}
