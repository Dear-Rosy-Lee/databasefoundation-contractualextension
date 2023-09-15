using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方导出表格内容实体类
    /// </summary>
    public class FamilyOutputDefine : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        /// 户主名索引
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NameValue
        {
            get { return nameValue; }
            set { nameValue = value; NotifyPropertyChanged("NameValue"); }
        }

        private bool nameValue;

        /// <summary>
        /// 承包方类型索引
        /// </summary>
        [DisplayLanguage("承包方类型", IsLanguageName = false)]
        [DescriptionLanguage("承包方类型索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractorTypeValue
        {
            get { return contractorTypeValue; }
            set { contractorTypeValue = value; NotifyPropertyChanged("ContractorTypeValue"); }
        }

        private bool contractorTypeValue;

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        [DisplayLanguage("家庭成员个数", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员个数索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberValue
        {
            get { return numberValue; }
            set { numberValue = value; NotifyPropertyChanged("NumberValue"); }
        }

        private bool numberValue;

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        [DisplayLanguage("家庭成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员姓名索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberNameValue
        {
            get { return numberNameValue; }
            set { numberNameValue = value; NotifyPropertyChanged("NumberNameValue"); }
        }

        private bool numberNameValue;

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        [DisplayLanguage("家庭成员性别", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员性别索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberGenderValue
        {
            get { return numberGenderValue; }
            set { numberGenderValue = value; NotifyPropertyChanged("NumberGenderValue"); }
        }

        private bool numberGenderValue;

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        [DisplayLanguage("家庭成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员年龄索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberAgeValue
        {
            get { return numberAgeValue; }
            set { numberAgeValue = value; NotifyPropertyChanged("NumberAgeValue"); }
        }

        private bool numberAgeValue;

        /// <summary>
        /// 家庭成员证件类型索引
        /// </summary>
        [DisplayLanguage("家庭成员证件类型", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员证件类型索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberCartTypeValue
        {
            get { return numberCartTypeValue; }
            set { numberCartTypeValue = value; NotifyPropertyChanged("numberCartTypeValue"); }
        }

        private bool numberCartTypeValue;

        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        [DisplayLanguage("家庭成员身份证号", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员身份证号索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberIcnValue
        {
            get { return numberIcnValue; }
            set { numberIcnValue = value; NotifyPropertyChanged("NumberIcnValue"); }
        }

        private bool numberIcnValue;

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        [DisplayLanguage("家庭成员关系", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员关系索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberRelatioinValue
        {
            get { return numberRelatioinValue; }
            set { numberRelatioinValue = value; NotifyPropertyChanged("NumberRelatioinValue"); }
        }

        private bool numberRelatioinValue;

        /// <summary>
        /// 家庭成员备注
        /// </summary>
        [DisplayLanguage("家庭成员备注", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CommentValue
        {
            get { return commentValue; }
            set { commentValue = value; NotifyPropertyChanged("CommentValue"); }
        }

        private bool commentValue;

        /// <summary>
        /// 共有人信息修改意见
        /// </summary>
        [DisplayLanguage("共有人信息修改意见", IsLanguageName = false)]
        [DescriptionLanguage("共有人信息修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CommonOpinion
        {
            get { return commonOpinion; }
            set { commonOpinion = value; NotifyPropertyChanged("CommonOpinion"); }
        }

        private bool commonOpinion;

        ///// <summary>
        ///// 承包地共有人
        ///// </summary>
        //[DisplayLanguage("承包地共有人", IsLanguageName = false)]
        //[DescriptionLanguage("承包地共有人", IsLanguageName = false)]
        //[PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
        //    Builder = typeof(PropertyDescriptorBoolean))]
        //public bool SharePersonValue
        //{
        //    get { return sharePersonValue; }
        //    set { sharePersonValue = value; NotifyPropertyChanged("SharePersonValue"); }
        //}
        //private bool sharePersonValue;

        /// <summary>
        /// 是否享有承包地
        /// </summary>
        [DisplayLanguage("是否共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否共有人", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsSharedLandValue
        {
            get { return isSharedLandValue; }
            set { isSharedLandValue = value; NotifyPropertyChanged("IsSharedLandValue"); }
        }

        private bool isSharedLandValue;

        /// <summary>
        /// 实际分配人数
        /// </summary>
        [DisplayLanguage("实际分配人数", IsLanguageName = false)]
        [DescriptionLanguage("实际分配人数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AllocationPersonValue
        {
            get { return allocationPersonValue; }
            set { allocationPersonValue = value; NotifyPropertyChanged("AllocationPersonValue"); }
        }

        private bool allocationPersonValue;

        /// <summary>
        /// 承包方地址索引
        /// </summary>
        [DisplayLanguage("承包方地址", IsLanguageName = false)]
        [DescriptionLanguage("承包方地址索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractorAddressValue
        {
            get { return contractorAddressValue; }
            set { contractorAddressValue = value; NotifyPropertyChanged("ContractorAddressValue"); }
        }

        private bool contractorAddressValue;

        /// <summary>
        /// 邮政编码索引
        /// </summary>
        [DisplayLanguage("邮政编码", IsLanguageName = false)]
        [DescriptionLanguage("邮政编码索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PostNumberValue
        {
            get { return postNumberValue; }
            set { postNumberValue = value; NotifyPropertyChanged("PostNumberValue"); }
        }

        private bool postNumberValue;

        /// <summary>
        /// 电话号码索引
        /// </summary>
        [DisplayLanguage("电话号码", IsLanguageName = false)]
        [DescriptionLanguage("电话号码索引", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TelephoneValue
        {
            get { return telephoneValue; }
            set { telephoneValue = value; NotifyPropertyChanged("TelephoneValue"); }
        }

        private bool telephoneValue;

        /// <summary>
        /// 民族信息
        /// </summary>
        [DisplayLanguage("民族信息", IsLanguageName = false)]
        [DescriptionLanguage("民族信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NationValue
        {
            get { return nationValue; }
            set { nationValue = value; NotifyPropertyChanged("NationValue"); }
        }

        private bool nationValue;

        /// <summary>
        /// 调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SurveyPersonValue
        {
            get { return surveyPersonValue; }
            set { surveyPersonValue = value; NotifyPropertyChanged("SurveyPersonValue"); }
        }

        private bool surveyPersonValue;

        /// <summary>
        /// 调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SurveyDateValue
        {
            get { return surveyDateValue; }
            set { surveyDateValue = value; NotifyPropertyChanged("SurveyDateValue"); }
        }

        private bool surveyDateValue;

        /// <summary>
        /// 调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SurveyChronicleValue
        {
            get { return surveyChronicleValue; }
            set { surveyChronicleValue = value; NotifyPropertyChanged("SurveyChronicleValue"); }
        }

        private bool surveyChronicleValue;

        /// <summary>
        /// 审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CheckPersonValue
        {
            get { return checkPersonValue; }
            set { checkPersonValue = value; NotifyPropertyChanged("CheckPersonValue"); }
        }

        private bool checkPersonValue;

        /// <summary>
        /// 审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CheckDateValue
        {
            get { return checkDateValue; }
            set { checkDateValue = value; NotifyPropertyChanged("CheckDateValue"); }
        }

        private bool checkDateValue;

        /// <summary>
        /// 审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CheckOpinionValue
        {
            get { return checkOpinionValue; }
            set { checkOpinionValue = value; NotifyPropertyChanged("CheckOpinionValue"); }
        }

        private bool checkOpinionValue;

        /// <summary>
        /// 户口性质
        /// </summary>
        [DisplayLanguage("户口性质", IsLanguageName = false)]
        [DescriptionLanguage("户口性质", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AccountNatureValue
        {
            get { return accountNatureValue; }
            set { accountNatureValue = value; NotifyPropertyChanged("AccountNatureValue"); }
        }

        private bool accountNatureValue;

        /// <summary>
        /// 合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondConcordNumberValue
        {
            get { return secondConcordNumberValue; }
            set { secondConcordNumberValue = value; NotifyPropertyChanged("SecondConcordNumberValue"); }
        }

        private bool secondConcordNumberValue;

        /// <summary>
        /// 证书编号
        /// </summary>
        [DisplayLanguage("证书编号", IsLanguageName = false)]
        [DescriptionLanguage("证书编号", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondWarrantNumberValue
        {
            get { return secondWarrantNumberValue; }
            set { secondWarrantNumberValue = value; NotifyPropertyChanged("SecondWarrantNumberValue"); }
        }

        private bool secondWarrantNumberValue;

        /// <summary>
        ///户籍备注
        /// </summary>
        [DisplayLanguage("户籍备注", IsLanguageName = false)]
        [DescriptionLanguage("户籍备注", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool CencueCommentValue
        {
            get { return cencueCommentValue; }
            set { cencueCommentValue = value; NotifyPropertyChanged("CencueCommentValue"); }
        }

        private bool cencueCommentValue;

        /// <summary>
        /// 二轮承包合同总面积
        /// </summary>
        [DisplayLanguage("二轮承包合同总面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包合同总面积", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
             Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondConcordTotalAreaValue
        {
            get { return secondConcordTotalAreaValue; }
            set { secondConcordTotalAreaValue = value; NotifyPropertyChanged("SecondConcordTotalAreaValue"); }
        }

        private bool secondConcordTotalAreaValue;

        /// <summary>
        /// 二轮承包合同地块总数
        /// </summary>
        [DisplayLanguage("二轮承包合同地块总数", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包合同地块总数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondConcordTotalLandCountValue
        {
            get { return secondConcordTotalLandCountValue; }
            set { secondConcordTotalLandCountValue = value; NotifyPropertyChanged("SecondConcordTotalLandCountValue"); }
        }

        private bool secondConcordTotalLandCountValue;

        /// <summary>
        /// 二轮延包姓名
        /// </summary>
        [DisplayLanguage("二轮延包姓名", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包姓名", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ExPackageNameValue
        {
            get { return exPackageNameValue; }
            set { exPackageNameValue = value; NotifyPropertyChanged("ExPackageNameValue"); }
        }

        private bool exPackageNameValue;

        /// <summary>
        /// 延包土地份数
        /// </summary>
        [DisplayLanguage("延包土地份数", IsLanguageName = false)]
        [DescriptionLanguage("延包土地份数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ExPackageNumberValue
        {
            get { return exPackageNumberValue; }
            set { exPackageNumberValue = value; NotifyPropertyChanged("ExPackageNumberValue"); }
        }

        private bool exPackageNumberValue;

        /// <summary>
        /// 合同开始时间
        /// </summary>
        [DisplayLanguage("开始时间", IsLanguageName = false)]
        [DescriptionLanguage("开始时间", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool StartTimeValue
        {
            get { return startTimeValue; }
            set { startTimeValue = value; NotifyPropertyChanged("StartTimeValue"); }
        }

        private bool startTimeValue;

        /// <summary>
        /// 合同结束时间
        /// </summary>
        [DisplayLanguage("结束时间", IsLanguageName = false)]
        [DescriptionLanguage("结束时间", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool EndTimeValue
        {
            get { return endTimeValue; }
            set { endTimeValue = value; NotifyPropertyChanged("EndTimeValue"); }
        }

        private bool endTimeValue;

        /// <summary>
        /// 承包方式
        /// </summary>
        [DisplayLanguage("取得承包方式", IsLanguageName = false)]
        [DescriptionLanguage("取得承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConstructTypeValue
        {
            get { return constructTypeValue; }
            set { constructTypeValue = value; NotifyPropertyChanged("ConstructTypeValue"); }
        }

        private bool constructTypeValue;

        /// <summary>
        /// 已死亡人员
        /// </summary>
        [DisplayLanguage("已死亡人员", IsLanguageName = false)]
        [DescriptionLanguage("已死亡人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsDeadedValue
        {
            get { return isDeadedValue; }
            set { isDeadedValue = value; NotifyPropertyChanged("isDeadedValue"); }
        }

        private bool isDeadedValue;

        /// <summary>
        /// 出嫁后未退承包地人员
        /// </summary>
        [DisplayLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LocalMarriedRetreatLandValue
        {
            get { return localMarriedRetreatLandValue; }
            set { localMarriedRetreatLandValue = value; NotifyPropertyChanged("LocalMarriedRetreatLandValue"); }
        }

        private bool localMarriedRetreatLandValue;

        /// <summary>
        /// 农转非后未退承包地人员
        /// </summary>
        [DisplayLanguage("农转非后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("农转非后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PeasantsRetreatLandValue
        {
            get { return peasantsRetreatLandValue; }
            set { peasantsRetreatLandValue = value; NotifyPropertyChanged("PeasantsRetreatLandValue"); }
        }

        private bool peasantsRetreatLandValue;

        /// <summary>
        /// 婚进但在非出地未退承包地人员
        /// </summary>
        [DisplayLanguage("婚进在但非出地未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("婚进但在非出地未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ForeignMarriedRetreatLandValue
        {
            get { return foreignMarriedRetreatLandValue; }
            set { foreignMarriedRetreatLandValue = value; NotifyPropertyChanged("ForeignMarriedRetreatLandValue"); }
        }

        private bool foreignMarriedRetreatLandValue;

        /// <summary>
        /// 入股份数
        /// </summary>
        [DisplayLanguage("入股份数", IsLanguageName = false)]
        [DescriptionLanguage("入股份数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool EquityNumberValue
        {
            get { return equityNumberValue; }
            set { equityNumberValue = value; NotifyPropertyChanged("EquityNumberValue"); }
        }

        private bool equityNumberValue;

        /// <summary>
        /// 入股面积
        /// </summary>
        [DisplayLanguage("入股面积", IsLanguageName = false)]
        [DescriptionLanguage("入股面积", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool EquityAreaValue
        {
            get { return equityAreaValue; }
            set { equityAreaValue = value; NotifyPropertyChanged("EquityAreaValue"); }
        }

        private bool equityAreaValue;

        /// <summary>
        /// 是否含有共有人信息
        /// </summary>
        [DisplayLanguage("共有人信息", IsLanguageName = false)]
        [DescriptionLanguage("是否含有共有人信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsSharePersonValue
        {
            get
            {
                return (NumberAgeValue || NumberGenderValue || IsSharedLandValue
                       || NumberNameValue || NumberCartTypeValue || NumberIcnValue
                       || NumberRelatioinValue || NationValue || AccountNatureValue
                       || CommentValue || IsSharedLandValue);
            }
        }

        /// <summary>
        /// 是否扩展信息
        /// </summary>
        [DisplayLanguage("是否扩展信息", IsLanguageName = false)]
        [DescriptionLanguage("是否扩展信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsExpandValue
        {
            get
            {
                return ExPackageNameValue || ExPackageNumberValue || IsDeadedValue
                    || LocalMarriedRetreatLandValue || PeasantsRetreatLandValue || ForeignMarriedRetreatLandValue;
            }
        }

        /// <summary>
        /// 列数
        /// </summary>
        [DisplayLanguage("列数", IsLanguageName = false)]
        [DescriptionLanguage("列数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "")]
        [Enabled(false)]
        public int ColumnCount
        {
            get { return columnCount; }
            set { columnCount = getColumnCount(); NotifyPropertyChanged("ColumnCount"); }
        }

        private int columnCount;

        #endregion Propertys

        #region Ctor

        public FamilyOutputDefine()
        {
            NameValue = true;
            ContractorTypeValue = true;
            NumberValue = true;
            NumberNameValue = true;
            NumberCartTypeValue = true;
            NumberIcnValue = true;
            NumberGenderValue = true;
            NumberAgeValue = false;
            NumberRelatioinValue = true;
            IsSharedLandValue = true;
            CommentValue = true;
            ContractorAddressValue = true;
            PostNumberValue = true;
            TelephoneValue = true;
            SurveyPersonValue = true;
            SurveyDateValue = true;
            SurveyChronicleValue = true;
            CheckPersonValue = true;
            CheckDateValue = true;
            CheckOpinionValue = true;
            SecondConcordTotalAreaValue = false;
            SecondConcordTotalLandCountValue = false;
            ExPackageNameValue = false;
            ExPackageNumberValue = false;
            IsDeadedValue = false;
            LocalMarriedRetreatLandValue = false;
            PeasantsRetreatLandValue = false;
            ForeignMarriedRetreatLandValue = false;
            //SharePersonValue = false;
            AllocationPersonValue = false;
            NationValue = false;
            AccountNatureValue = false;
            SecondConcordNumberValue = false;
            SecondWarrantNumberValue = false;
            StartTimeValue = false;
            EndTimeValue = false;
            ConstructTypeValue = false;
            EquityNumberValue = false;
            EquityAreaValue = false;
            CencueCommentValue = false;
            CommonOpinion = true;
            ColumnCount = 20;
        }

        /// <summary>
        /// 根据配置初始化列表栏个数
        /// </summary>
        /// <returns></returns>
        public int getColumnCount()
        {
            int count = 1;
            count += NameValue ? 1 : 0;
            count += ContractorTypeValue ? 1 : 0;
            count += NumberValue ? 1 : 0;
            count += NumberNameValue ? 1 : 0;
            count += NumberCartTypeValue ? 1 : 0;
            count += NumberIcnValue ? 1 : 0;
            count += NumberGenderValue ? 1 : 0;
            count += NumberAgeValue ? 1 : 0;
            count += NumberRelatioinValue ? 1 : 0;
            count += IsSharedLandValue ? 1 : 0;
            count += CommentValue ? 1 : 0;
            count += ContractorAddressValue ? 1 : 0;
            count += PostNumberValue ? 1 : 0;
            count += TelephoneValue ? 1 : 0;
            count += SurveyPersonValue ? 1 : 0;
            count += SurveyDateValue ? 1 : 0;
            count += SurveyChronicleValue ? 1 : 0;
            count += CheckPersonValue ? 1 : 0;
            count += CheckDateValue ? 1 : 0;
            count += CheckOpinionValue ? 1 : 0;
            count += ExPackageNameValue ? 1 : 0;
            count += ExPackageNumberValue ? 1 : 0;
            count += IsDeadedValue ? 1 : 0;
            count += LocalMarriedRetreatLandValue ? 1 : 0;
            count += PeasantsRetreatLandValue ? 1 : 0;
            count += ForeignMarriedRetreatLandValue ? 1 : 0;
            //count += SharePersonValue ? 1 : 0;
            count += AllocationPersonValue ? 1 : 0;
            count += NationValue ? 1 : 0;
            count += AccountNatureValue ? 1 : 0;
            count += SecondConcordNumberValue ? 1 : 0;
            count += SecondWarrantNumberValue ? 1 : 0;
            count += StartTimeValue ? 1 : 0;
            count += EndTimeValue ? 1 : 0;
            count += ConstructTypeValue ? 1 : 0;
            count += EquityNumberValue ? 1 : 0;
            count += EquityAreaValue ? 1 : 0;
            count += SecondConcordTotalAreaValue ? 1 : 0;
            count += SecondConcordTotalLandCountValue ? 1 : 0;
            count += CencueCommentValue ? 1 : 0;
            count += CommonOpinion ? 1 : 0;
            return count;
        }

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static FamilyOutputDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<FamilyOutputDefine>();
            var section = profile.GetSection<FamilyOutputDefine>();
            return section.Settings;
        }

        #endregion Ctor
    }
}