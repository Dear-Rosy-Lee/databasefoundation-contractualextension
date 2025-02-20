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
    /// 数据汇总表设置
    /// </summary>
    public class DataSummaryDefine : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        /// 承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方汇总信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyName
        {
            get { return familyName; }
            set { familyName = value; NotifyPropertyChanged("FamilyName"); }
        }
        private bool familyName;

        /// <summary>
        /// 家庭成员个数
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
        /// 家庭成员姓名
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
        /// 性别
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
        /// 年龄
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
        /// 身份证号码
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
        /// 家庭关系
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
        /// 备注
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
        /// 宗地数
        /// </summary>
        [DisplayLanguage("宗地数", IsLanguageName = false)]
        [DescriptionLanguage("宗地数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCount
        {
            get { return landCount; }
            set { landCount = value; NotifyPropertyChanged("LandCount"); }
        }
        private bool landCount;

        /// <summary>
        /// 二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TableArea
        {
            get { return tableArea; }
            set { tableArea = value; NotifyPropertyChanged("TableArea"); }
        }
        private bool tableArea;

        /// <summary>
        /// 实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ActualArea
        {
            get { return actualArea; }
            set { actualArea = value; NotifyPropertyChanged("ActualArea"); }
        }
        private bool actualArea;

        /// <summary>
        /// 确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地汇总信息",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool AwareArea
        {
            get { return awareArea; }
            set { awareArea = value; NotifyPropertyChanged("AwareArea"); }
        }
        private bool awareArea;

        /// <summary>
        /// 合同编号
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
        /// 权证编号
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

        /// <summary>
        /// 列数
        /// </summary>
        [DisplayLanguage("列数", IsLanguageName = false)]
        [Enabled(false)]
        public int ColumnCount
        {
            get
            {
                return columnCount;
            }
            set
            {
                columnCount = GetColumnCount();
            }
        }
        private int columnCount;

        #endregion

        #region Ctor

        public DataSummaryDefine()
        {
            FamilyName = true;
            NumberValue = true;
            NumberNameValue = true;
            NumberGenderValue = true;
            NumberAgeValue = true;
            NumberIcnValue = true;
            NumberRelatioinValue = true;
            CommentValue = true;
            LandCount = true;
            TableArea = true;
            ActualArea = true;
            AwareArea = true;
            ConcordNumberValue = false;
            WarrantNumber = false;
            ColumnCount = 1;   //初始列数为1
        }

        #endregion

        #region Method

        /// <summary>
        /// 汇总表列数统计
        /// </summary>
        /// <returns></returns>
        private int GetColumnCount()
        {
            int count = 1;   //初始列数为1
            count += FamilyName ? 1 : 0;
            count += NumberValue ? 1 : 0;
            count += NumberNameValue ? 1 : 0;
            count += NumberGenderValue ? 1 : 0;
            count += NumberAgeValue ? 1 : 0;
            count += NumberIcnValue ? 1 : 0;
            count += NumberRelatioinValue ? 1 : 0;
            count += CommentValue ? 1 : 0;
            count += LandCount ? 1 : 0;
            count += TableArea ? 1 : 0;
            count += ActualArea ? 1 : 0;
            count += AwareArea ? 1 : 0;
            count += ConcordNumberValue ? 1 : 0;
            count += WarrantNumber ? 1 : 0;
            return count;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static DataSummaryDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<DataSummaryDefine>();
            var section = profile.GetSection<DataSummaryDefine>();
            return  section.Settings;
        }

        #endregion
    }
}
