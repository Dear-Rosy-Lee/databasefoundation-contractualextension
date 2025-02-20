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
    /// 承包方导入调查表实体类
    /// </summary>
    public class FamilyImportDefine : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        /// 户主名索引
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NameIndex
        {
            get { return nameIndex; }
            set { nameIndex = value; NotifyPropertyChanged("NameIndex"); }
        }
        private int nameIndex;

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        [DisplayLanguage("家庭成员个数", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberIndex
        {
            get { return numberIndex; }
            set { numberIndex = value; NotifyPropertyChanged("NumberIndex"); }
        }
        private int numberIndex;

        /// <summary>
        /// 户主类型索引
        /// </summary>
        [DisplayLanguage("承包方类型", IsLanguageName = false)]
        [DescriptionLanguage("承包方类型", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractorTypeIndex
        {
            get { return contractorTypeIndex; }
            set { contractorTypeIndex = value; NotifyPropertyChanged("ContractorTypeIndex"); }
        }
        private int contractorTypeIndex;

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        [DisplayLanguage("家庭成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberNameIndex
        {
            get { return numberNameIndex; }
            set { numberNameIndex = value; NotifyPropertyChanged("NumberNameIndex"); }
        }
        private int numberNameIndex;

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        [DisplayLanguage("家庭成员性别", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员性别", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberGenderIndex
        {
            get { return numberGenderIndex; }
            set { numberGenderIndex = value; NotifyPropertyChanged("NumberGenderIndex"); }
        }
        private int numberGenderIndex;

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        [DisplayLanguage("家庭成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员年龄", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberAgeIndex
        {
            get { return numberAgeIndex; }
            set { numberAgeIndex = value; NotifyPropertyChanged("NumberAgeIndex"); }
        }
        private int numberAgeIndex;

        /// <summary>
        /// 家庭成员证件类型索引
        /// </summary>
        [DisplayLanguage("家庭成员证件类型", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员证件类型", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberCartTypeIndex
        {
            get { return numberCartTypeIndex; }
            set { numberCartTypeIndex = value; NotifyPropertyChanged("NumberCartTypeIndex"); }
        }
        private int numberCartTypeIndex;
        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        [DisplayLanguage("家庭成员身份证号", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员身份证号", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberIcnIndex
        {
            get { return numberIcnIndex; }
            set { numberIcnIndex = value; NotifyPropertyChanged("NumberIcnIndex"); }
        }
        private int numberIcnIndex;

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        [DisplayLanguage("家庭成员关系", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员关系", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberRelatioinIndex
        {
            get { return numberRelatioinIndex; }
            set { numberRelatioinIndex = value; NotifyPropertyChanged("NumberRelatioinIndex"); }
        }
        private int numberRelatioinIndex;

        /// <summary>
        /// 家庭成员备注索引
        /// </summary>
        [DisplayLanguage("家庭成员备注", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CommentIndex
        {
            get { return commentIndex; }
            set { commentIndex = value; NotifyPropertyChanged("CommentIndex"); }
        }
        private int commentIndex;

        /// <summary>
        /// 共有人修改意见索引
        /// </summary>
        [DisplayLanguage("共有人修改意见", IsLanguageName = false)]
        [DescriptionLanguage("共有人修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int OpinionIndex
        {
            get { return opinionIndex; }
            set { opinionIndex = value; NotifyPropertyChanged("OpinionIndex"); }
        }
        private int opinionIndex;
        /// <summary>
        /// 承包地共有人索引
        /// </summary>
        [DisplayLanguage("承包地共有人", IsLanguageName = false)]
        [DescriptionLanguage("承包地共有人", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SharePersonIndex
        {
            get { return sharePersonIndex; }
            set { sharePersonIndex = value; NotifyPropertyChanged("SharePersonIndex"); }
        }
        private int sharePersonIndex;

        /// <summary>
        /// 是否享有承包地索引
        /// </summary>
        [DisplayLanguage("是否共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否共有人", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsSharedLandIndex
        {
            get { return isSharedLandIndex; }
            set { isSharedLandIndex = value; NotifyPropertyChanged("IsSharedLandIndex"); }
        }
        private int isSharedLandIndex;

        /// <summary>
        /// 实际分配人数
        /// </summary>
        [DisplayLanguage("实际分配人数", IsLanguageName = false)]
        [DescriptionLanguage("实际分配人数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AllocationPersonIndex
        {
            get { return allocationPersonIndex; }
            set { allocationPersonIndex = value; NotifyPropertyChanged("AllocationPersonIndex"); }
        }
        private int allocationPersonIndex;

        /// <summary>
        /// 承包方地址索引
        /// </summary>
        [DisplayLanguage("承包方地址", IsLanguageName = false)]
        [DescriptionLanguage("承包方地址", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractorAddressIndex
        {
            get { return contractorAddressIndex; }
            set { contractorAddressIndex = value; NotifyPropertyChanged("ContractorAddressIndex"); }
        }
        private int contractorAddressIndex;

        /// <summary>
        /// 邮政编码索引
        /// </summary>
        [DisplayLanguage("邮政编码", IsLanguageName = false)]
        [DescriptionLanguage("邮政编码", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PostNumberIndex
        {
            get { return postNumberIndex; }
            set { postNumberIndex = value; NotifyPropertyChanged("PostNumberIndex"); }
        }
        private int postNumberIndex;

        /// <summary>
        /// 电话号码索引
        /// </summary>
        [DisplayLanguage("电话号码", IsLanguageName = false)]
        [DescriptionLanguage("电话号码", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TelephoneIndex
        {
            get { return telephoneIndex; }
            set { telephoneIndex = value; NotifyPropertyChanged("TelephoneIndex"); }
        }
        private int telephoneIndex;

        /// <summary>
        /// 民族信息
        /// </summary>
        [DisplayLanguage("民族信息", IsLanguageName = false)]
        [DescriptionLanguage("民族信息", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NationIndex
        {
            get { return nationIndex; }
            set { nationIndex = value; NotifyPropertyChanged("NationIndex"); }
        }
        private int nationIndex;

        /// <summary>
        /// 户口性质
        /// </summary>
        [DisplayLanguage("户口性质", IsLanguageName = false)]
        [DescriptionLanguage("户口性质", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AccountNatureIndex
        {
            get { return accountNatureIndex; }
            set { accountNatureIndex = value; NotifyPropertyChanged("AccountNatureIndex"); }
        }
        private int accountNatureIndex;

        /// <summary>
        /// 合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondConcordNumberIndex
        {
            get { return secondConcordNumberIndex; }
            set { secondConcordNumberIndex = value; NotifyPropertyChanged("SecondConcordNumberIndex"); }
        }
        private int secondConcordNumberIndex;

        /// <summary>
        /// 证书编号
        /// </summary>
        [DisplayLanguage("证书编号", IsLanguageName = false)]
        [DescriptionLanguage("证书编号", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondWarrantNumberIndex
        {
            get { return secondWarrantNumberIndex; }
            set { secondWarrantNumberIndex = value; NotifyPropertyChanged("SecondWarrantNumberIndex"); }
        }
        private int secondWarrantNumberIndex;

        /// <summary>
        /// 调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SurveyPersonIndex
        {
            get { return surveyPersonIndex; }
            set { surveyPersonIndex = value; NotifyPropertyChanged("SurveyPersonIndex"); }
        }
        private int surveyPersonIndex;

        /// <summary>
        /// 调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SurveyDateIndex
        {
            get { return surveyDateIndex; }
            set { surveyDateIndex = value; NotifyPropertyChanged("SurveyDateIndex"); }
        }
        private int surveyDateIndex;

        /// <summary>
        /// 调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SurveyChronicleIndex
        {
            get { return surveyChronicleIndex; }
            set { surveyChronicleIndex = value; NotifyPropertyChanged("SurveyChronicleIndex"); }
        }
        private int surveyChronicleIndex;

        /// <summary>
        /// 审核员
        /// </summary>
        [DisplayLanguage("审核员", IsLanguageName = false)]
        [DescriptionLanguage("审核员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CheckPersonIndex
        {
            get { return checkPersonIndex; }
            set { checkPersonIndex = value; NotifyPropertyChanged("CheckPersonIndex"); }
        }
        private int checkPersonIndex;

        /// <summary>
        /// 审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CheckDateIndex
        {
            get { return checkDateIndex; }
            set { checkDateIndex = value; NotifyPropertyChanged("CheckDateIndex"); }
        }
        private int checkDateIndex;

        /// <summary>
        /// 审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CheckOpinionIndex
        {
            get { return checkOpinionIndex; }
            set { checkOpinionIndex = value; NotifyPropertyChanged("CheckOpinionIndex"); }
        }
        private int checkOpinionIndex;

        /// <summary>
        ///户籍备注
        /// </summary>
        [DisplayLanguage("户籍备注", IsLanguageName = false)]
        [DescriptionLanguage("户籍备注", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
           Builder = typeof(PropertyDescriptorBuilderSelector), 
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CencueCommentIndex
        {
            get { return cencueCommentIndex; }
            set { cencueCommentIndex = value; NotifyPropertyChanged("CencueCommentIndex"); }
        }
        private int cencueCommentIndex;
        
        /// <summary>
        /// 二轮承包合同总面积
        /// </summary>
        [DisplayLanguage("二轮承包合同总面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包合同总面积", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondConcordTotalArea
        {
            get { return secondConcordTotalArea; }
            set { secondConcordTotalArea = value; NotifyPropertyChanged("SecondConcordTotalArea"); }
        }
        private int secondConcordTotalArea;

        /// <summary>
        /// 二轮承包合同地块总数
        /// </summary>
        [DisplayLanguage("二轮承包合同地块总数", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包合同地块总数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondConcordTotalLandCount
        {
            get { return secondConcordTotalLandCount; }
            set { secondConcordTotalLandCount = value; NotifyPropertyChanged("SecondConcordTotalLandCount"); }
        }
        private int secondConcordTotalLandCount;

        /// <summary>
        /// 扩展字段
        /// </summary>
        [DisplayLanguage("扩展字段", IsLanguageName = false)]
        [DescriptionLanguage("扩展字段", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "承包方及家庭成员信息", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ExtendName
        {
            get { return extendName; }
            set { extendName = value; NotifyPropertyChanged("ExtendName"); }
        }
        private int extendName;
        
        /// <summary>
        /// 二轮延包姓名索引
        /// </summary>
        [DisplayLanguage("二轮延包姓名", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包姓名", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ExPackageNameIndex
        {
            get { return exPackageNameIndex; }
            set { exPackageNameIndex = value; NotifyPropertyChanged("ExPackageNameIndex"); }
        }
        private int exPackageNameIndex;


        /// <summary>
        /// 延包土地份数索引
        /// </summary>
        [DisplayLanguage("延包土地份数", IsLanguageName = false)]
        [DescriptionLanguage("延包土地份数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ExPackageNumberIndex
        {
            get { return exPackageNumberIndex; }
            set { exPackageNumberIndex = value; NotifyPropertyChanged("ExPackageNumberIndex"); }
        }
        private int exPackageNumberIndex;

        /// <summary>
        /// 合同开始时间
        /// </summary>
        [DisplayLanguage("开始时间", IsLanguageName = false)]
        [DescriptionLanguage("开始时间", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int StartTimeIndex
        {
            get { return startTimeIndex; }
            set { startTimeIndex = value; NotifyPropertyChanged("StartTimeIndex"); }
        }
        private int startTimeIndex;

        /// <summary>
        /// 合同结束时间
        /// </summary>
        [DisplayLanguage("结束时间", IsLanguageName = false)]
        [DescriptionLanguage("结束时间", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int EndTimeIndex
        {
            get { return endTimeIndex; }
            set { endTimeIndex = value; NotifyPropertyChanged("EndTimeIndex"); }
        }
        private int endTimeIndex;

        /// <summary>
        /// 承包方式
        /// </summary>
        [DisplayLanguage("取得承包方式", IsLanguageName = false)]
        [DescriptionLanguage("取得承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ConstructTypeIndex
        {
            get { return constructTypeIndex; }
            set { constructTypeIndex = value; NotifyPropertyChanged("ConstructTypeIndex"); }
        }
        private int constructTypeIndex;

        /// <summary>
        /// 已死亡人员索引
        /// </summary>
        [DisplayLanguage("已死亡人员", IsLanguageName = false)]
        [DescriptionLanguage("已死亡人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsDeadedIndex
        {
            get { return isDeadedIndex; }
            set { isDeadedIndex = value; NotifyPropertyChanged("IsDeadedIndex"); }
        }
        private int isDeadedIndex;

        /// <summary>
        /// 出嫁后未退承包地人员索引
        /// </summary>
        [DisplayLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LocalMarriedRetreatLandIndex
        {
            get { return localMarriedRetreatLandIndex; }
            set { localMarriedRetreatLandIndex = value; NotifyPropertyChanged("LocalMarriedRetreatLandIndex"); }
        }
        private int localMarriedRetreatLandIndex;

        /// <summary>
        /// 农转非后未退承包地人员索引
        /// </summary>
        [DisplayLanguage("农转非后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("农转非后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PeasantsRetreatLandIndex
        {
            get { return peasantsRetreatLandIndex; }
            set { peasantsRetreatLandIndex = value; NotifyPropertyChanged("PeasantsRetreatLandIndex"); }
        }
        private int peasantsRetreatLandIndex;

        /// <summary>
        /// 婚进但在非出地未退承包地人员索引
        /// </summary>
        [DisplayLanguage("婚进但在非出地未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("婚进但在非出地未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ForeignMarriedRetreatLandIndex
        {
            get { return foreignMarriedRetreatLandIndex; }
            set { foreignMarriedRetreatLandIndex = value; NotifyPropertyChanged("ForeignMarriedRetreatLandIndex"); }
        }
        private int foreignMarriedRetreatLandIndex;

        /// <summary>
        /// 入股份数
        /// </summary>
        [DisplayLanguage("入股份数", IsLanguageName = false)]
        [DescriptionLanguage("入股份数", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int EquityNumberIndex
        {
            get { return equityNumberIndex; }
            set { equityNumberIndex = value; NotifyPropertyChanged("EquityNumberIndex"); }
        }
        private int equityNumberIndex;

        /// <summary>
        /// 入股面积
        /// </summary>
        [DisplayLanguage("入股面积", IsLanguageName = false)]
        [DescriptionLanguage("入股面积", IsLanguageName = false)]
        [PropertyDescriptor(Gallery = "二轮土地延包家庭情况", Catalog = "",
            Builder = typeof(PropertyDescriptorBuilderSelector),
            Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int EquityAreaIndex
        {
            get { return equityAreaIndex; }
            set { equityAreaIndex = value; NotifyPropertyChanged("EquityAreaIndex"); }
        }
        private int equityAreaIndex;

        

        #endregion

        #region Ctor

        /// <summary>
        /// 默认值
        /// </summary>
        public FamilyImportDefine()
        {
            NameIndex = 1;
            ContractorTypeIndex = 2;
            NumberIndex = 3;
            NumberNameIndex = 4;
            NumberGenderIndex = 5;
            NumberAgeIndex = -1;
            NumberCartTypeIndex = 6;
            NumberIcnIndex = 7;
            NumberRelatioinIndex = 8;
            IsSharedLandIndex = 9;
            CommentIndex = 10;
            OpinionIndex = 11;
            ContractorAddressIndex = 12;
            PostNumberIndex = 13;
            TelephoneIndex = 14;
            SurveyPersonIndex = 15;
            SurveyDateIndex = 16;
            SurveyChronicleIndex = 17;
            CheckPersonIndex = 18;
            CheckDateIndex = 19;
            CheckOpinionIndex = 20;
            AccountNatureIndex = -1;
            ExPackageNameIndex = -1;
            ExPackageNumberIndex = -1;
            IsDeadedIndex = -1;
            LocalMarriedRetreatLandIndex = -1;
            PeasantsRetreatLandIndex = -1;
            ForeignMarriedRetreatLandIndex = -1;
            SharePersonIndex = -1;
            AllocationPersonIndex = -1;
            NationIndex = -1;
            AllocationPersonIndex = -1;
            SecondConcordNumberIndex = -1;
            SecondWarrantNumberIndex = -1;
            StartTimeIndex = -1;
            EndTimeIndex = -1;
            ConstructTypeIndex = -1;
            EquityAreaIndex = -1;
            EquityNumberIndex = -1;
            SecondConcordTotalArea = -1;
            SecondConcordTotalLandCount = -1;
            CencueCommentIndex = -1;
            extendName = -1;
        }

        #endregion

        #region Method

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static FamilyImportDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<FamilyImportDefine>();
            var section = profile.GetSection<FamilyImportDefine>();
            return  section.Settings;
        }

        #endregion
    }
}
