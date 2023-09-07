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
    /// 承包权证明数据汇总表设置
    /// </summary>
   public class ContractRegeditBookDataSummaryDefine: NotifyCDObject
    {
        #region Propertys
        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NameValue
        {
            get { return nameValue; }
            set { nameValue = value; NotifyPropertyChanged("NameValue"); }
        }
        private bool nameValue;
        
        /// <summary>
        ///家庭成员个数
        /// </summary>
        [DisplayLanguage("家庭成员个数", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberValue
        {
            get { return numberValue; }
            set { numberValue = value; NotifyPropertyChanged("NumberValue"); }
        }
        private bool numberValue;

        /// <summary>
        ///家庭成员姓名
        /// </summary>
        [DisplayLanguage("家庭成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息", 
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberNameValue
        {
            get { return numberNameValue; }
            set { numberNameValue = value; NotifyPropertyChanged("NumberNameValue"); }
        }
        private bool numberNameValue;

        /// <summary>
        ///性别
        /// </summary>
        [DisplayLanguage("性别", IsLanguageName = false)]
        [DescriptionLanguage("性别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息", 
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberGenderValue
        {
            get { return numberGenderValue; }
            set { numberGenderValue = value; NotifyPropertyChanged("NumberGenderValue"); }
        }
        private bool numberGenderValue;

        /// <summary>
        ///年龄
        /// </summary>
        [DisplayLanguage("年龄", IsLanguageName = false)]
        [DescriptionLanguage("年龄", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberAgeValue
        {
            get { return numberAgeValue; }
            set { numberAgeValue = value; NotifyPropertyChanged("NumberAgeValue"); }
        }
        private bool numberAgeValue;

        /// <summary>
        ///身份证号码
        /// </summary>
        [DisplayLanguage("身份证号码", IsLanguageName = false)]
        [DescriptionLanguage("身份证号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberIcnValue
        {
            get { return numberIcnValue; }
            set { numberIcnValue = value; NotifyPropertyChanged("NumberIcnValue"); }
        }
        private bool numberIcnValue;

        /// <summary>
        ///家庭关系
        /// </summary>
        [DisplayLanguage("家庭关系", IsLanguageName = false)]
        [DescriptionLanguage("家庭关系", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberRelatioinValue
        {
            get { return numberRelatioinValue; }
            set { numberRelatioinValue = value; NotifyPropertyChanged("NumberRelatioinValue"); }
        }
        private bool numberRelatioinValue;

        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息", 
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CommentValue
        {
            get { return commentValue; }
            set { commentValue = value; NotifyPropertyChanged("CommentValue"); }
        }
        private bool commentValue;

        /// <summary>
        ///宗地数
        /// </summary>
        [DisplayLanguage("宗地数", IsLanguageName = false)]
        [DescriptionLanguage("宗地数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SituationNumber
        {
            get { return situationNumber; }
            set { situationNumber = value; NotifyPropertyChanged("SituationNumber"); }
        }
        private bool situationNumber;

        /// <summary>
        ///二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TableAreaValue
        {
            get { return tableAreaValue; }
            set { tableAreaValue = value; NotifyPropertyChanged("TableAreaValue"); }
        }
        private bool tableAreaValue;

        /// <summary>
        ///实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ActualAreaValue
        {
            get { return actualAreaValue; }
            set { actualAreaValue = value; NotifyPropertyChanged("ActualAreaValue"); }
        }
        private bool actualAreaValue;

        /// <summary>
        ///确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息", 
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AwareAreaValue
        {
            get { return awareAreaValue; }
            set { awareAreaValue = value; NotifyPropertyChanged("AwareAreaValue"); }
        }
        private bool awareAreaValue;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConcordNumberValue
        {
            get { return concordNumberValue; }
            set { concordNumberValue = value; NotifyPropertyChanged("ConcordNumberValue"); }
        }
        private bool concordNumberValue;

        /// <summary>
        ///权证编号
        /// </summary>
        [DisplayLanguage("权证编号", IsLanguageName = false)]
        [DescriptionLanguage("权证编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool WarrantNumber
        {
            get { return warrantNumber; }
            set { warrantNumber = value; NotifyPropertyChanged("WarrantNumber"); }
        }
        private bool warrantNumber;  

        #endregion

        #region Ctor
        public ContractRegeditBookDataSummaryDefine()
        {
            nameValue = true;
            numberValue = true;
            numberNameValue = true;
            numberGenderValue = true;
            numberAgeValue = true;
            numberIcnValue = true;
            numberRelatioinValue = true;
            commentValue = true;
            situationNumber = true;
            tableAreaValue = true;
            actualAreaValue = true;
            awareAreaValue = true;
            concordNumberValue = true;
            warrantNumber = true;
        }
        #endregion

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractRegeditBookDataSummaryDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractRegeditBookDataSummaryDefine>();
            var section = profile.GetSection<ContractRegeditBookDataSummaryDefine>();
            return  section.Settings;
        }
    }
}
