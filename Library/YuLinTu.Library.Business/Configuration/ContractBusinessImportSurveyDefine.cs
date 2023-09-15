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
    /// 导入调查表设置实体
    /// </summary>
    public class ContractBusinessImportSurveyDefine : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NameIndex
        {
            get { return nameIndex; }
            set { nameIndex = value; NotifyPropertyChanged("NameIndex"); }
        }

        private int nameIndex;

        /// <summary>
        ///承包方类型
        /// </summary>
        [DisplayLanguage("承包方类型", IsLanguageName = false)]
        [DescriptionLanguage("承包方类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractorTypeIndex
        {
            get { return contractorTypeIndex; }
            set { contractorTypeIndex = value; NotifyPropertyChanged("ContractorTypeIndex"); }
        }

        private int contractorTypeIndex;

        /// <summary>
        ///成员个数
        /// </summary>
        [DisplayLanguage("成员个数", IsLanguageName = false)]
        [DescriptionLanguage("成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberIndex
        {
            get { return numberIndex; }
            set { numberIndex = value; NotifyPropertyChanged("NumberIndex"); }
        }

        private int numberIndex;

        /// <summary>
        ///成员姓名
        /// </summary>
        [DisplayLanguage("成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberNameIndex
        {
            get { return numberNameIndex; }
            set { numberNameIndex = value; NotifyPropertyChanged("NumberNameIndex"); }
        }

        private int numberNameIndex;

        /// <summary>
        ///成员性别
        /// </summary>
        [DisplayLanguage("成员性别", IsLanguageName = false)]
        [DescriptionLanguage("成员性别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberGenderIndex
        {
            get { return numberGenderIndex; }
            set { numberGenderIndex = value; NotifyPropertyChanged("NumberGenderIndex"); }
        }

        private int numberGenderIndex;

        /// <summary>
        ///成员年龄
        /// </summary>
        [DisplayLanguage("成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("成员年龄", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberAgeIndex
        {
            get { return numberAgeIndex; }
            set { numberAgeIndex = value; NotifyPropertyChanged("NumberAgeIndex"); }
        }

        private int numberAgeIndex;

        /// <summary>
        ///证件类型
        /// </summary>
        [DisplayLanguage("证件类型", IsLanguageName = false)]
        [DescriptionLanguage("证件类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberCartTypeIndex
        {
            get { return numberCartTypeIndex; }
            set { numberCartTypeIndex = value; NotifyPropertyChanged("NumberCartTypeIndex"); }
        }

        private int numberCartTypeIndex;

        /// <summary>
        ///证件号码
        /// </summary>
        [DisplayLanguage("证件号码", IsLanguageName = false)]
        [DescriptionLanguage("证件号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberIcnIndex
        {
            get { return numberIcnIndex; }
            set { numberIcnIndex = value; NotifyPropertyChanged("NumberIcnIndex"); }
        }

        private int numberIcnIndex;

        /// <summary>
        ///成员关系
        /// </summary>
        [DisplayLanguage("成员关系", IsLanguageName = false)]
        [DescriptionLanguage("成员关系", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NumberRelatioinIndex
        {
            get { return numberRelatioinIndex; }
            set { numberRelatioinIndex = value; NotifyPropertyChanged("NumberRelatioinIndex"); }
        }

        private int numberRelatioinIndex;

        /// <summary>
        ///是否共有人
        /// </summary>
        [DisplayLanguage("是否共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否共有人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsSharedLandIndex
        {
            get { return isSharedLandIndex; }
            set { isSharedLandIndex = value; NotifyPropertyChanged("IsSharedLandIndex"); }
        }

        private int isSharedLandIndex;

        /// <summary>
        ///成员备注
        /// </summary>
        [DisplayLanguage("成员备注", IsLanguageName = false)]
        [DescriptionLanguage("成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilyCommentIndex
        {
            get { return familyCommentIndex; }
            set { familyCommentIndex = value; NotifyPropertyChanged("FamilyCommentIndex"); }
        }

        private int familyCommentIndex;

        /// <summary>
        ///共有人信息修改意见
        /// </summary>
        [DisplayLanguage("共有人信息修改意见", IsLanguageName = false)]
        [DescriptionLanguage("共有人信息修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilyOpinionIndex
        {
            get { return familyOpinionIndex; }
            set { familyOpinionIndex = value; NotifyPropertyChanged("FamilyOpinionIndex"); }
        }

        private int familyOpinionIndex;

        /// <summary>
        ///承包方地址
        /// </summary>
        [DisplayLanguage("承包方地址", IsLanguageName = false)]
        [DescriptionLanguage("承包方地址", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractorAddressIndex
        {
            get { return contractorAddressIndex; }
            set { contractorAddressIndex = value; NotifyPropertyChanged("ContractorAddressIndex"); }
        }

        private int contractorAddressIndex;

        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandNameIndex
        {
            get { return landNameIndex; }
            set { landNameIndex = value; NotifyPropertyChanged("LandNameIndex"); }
        }

        private int landNameIndex;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CadastralNumberIndex
        {
            get { return cadastralNumberIndex; }
            set { cadastralNumberIndex = value; NotifyPropertyChanged("CadastralNumberIndex"); }
        }

        private int cadastralNumberIndex;

        /// <summary>
        ///图幅编号
        /// </summary>
        [DisplayLanguage("图幅编号", IsLanguageName = false)]
        [DescriptionLanguage("图幅编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ImageNumberIndex
        {
            get { return imageNumberIndex; }
            set { imageNumberIndex = value; NotifyPropertyChanged("ImageNumberIndex"); }
        }

        private int imageNumberIndex;

        /// <summary>
        ///二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TableAreaIndex
        {
            get { return tableAreaIndex; }
            set { tableAreaIndex = value; NotifyPropertyChanged("TableAreaIndex"); }
        }

        private int tableAreaIndex;

        /// <summary>
        ///实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ActualAreaIndex
        {
            get { return actualAreaIndex; }
            set { actualAreaIndex = value; NotifyPropertyChanged("ActualAreaIndex"); }
        }

        private int actualAreaIndex;

        /// <summary>
        ///四至东
        /// </summary>
        [DisplayLanguage("四至东", IsLanguageName = false)]
        [DescriptionLanguage("四至东", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int EastIndex
        {
            get { return eastIndex; }
            set { eastIndex = value; NotifyPropertyChanged("EastIndex"); }
        }

        private int eastIndex;

        /// <summary>
        ///四至南
        /// </summary>
        [DisplayLanguage("四至南", IsLanguageName = false)]
        [DescriptionLanguage("四至南", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SourthIndex
        {
            get { return sourthIndex; }
            set { sourthIndex = value; NotifyPropertyChanged("SourthIndex"); }
        }

        private int sourthIndex;

        /// <summary>
        ///四至西
        /// </summary>
        [DisplayLanguage("四至西", IsLanguageName = false)]
        [DescriptionLanguage("四至西", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int WestIndex
        {
            get { return westIndex; }
            set { westIndex = value; NotifyPropertyChanged("WestIndex"); }
        }

        private int westIndex;

        /// <summary>
        ///四至北
        /// </summary>
        [DisplayLanguage("四至北", IsLanguageName = false)]
        [DescriptionLanguage("四至北", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NorthIndex
        {
            get { return northIndex; }
            set { northIndex = value; NotifyPropertyChanged("NorthIndex"); }
        }

        private int northIndex;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandPurposeIndex
        {
            get { return landPurposeIndex; }
            set { landPurposeIndex = value; NotifyPropertyChanged("LandPurposeIndex"); }
        }

        private int landPurposeIndex;

        /// <summary>
        ///地力等级
        /// </summary>
        [DisplayLanguage("地力等级", IsLanguageName = false)]
        [DescriptionLanguage("地力等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandLevelIndex
        {
            get { return landLevelIndex; }
            set { landLevelIndex = value; NotifyPropertyChanged("LandLevelIndex"); }
        }

        private int landLevelIndex;

        /// <summary>
        ///土地利用类型
        /// </summary>
        [DisplayLanguage("土地利用类型", IsLanguageName = false)]
        [DescriptionLanguage("土地利用类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandTypeIndex
        {
            get { return landTypeIndex; }
            set { landTypeIndex = value; NotifyPropertyChanged("LandTypeIndex"); }
        }

        private int landTypeIndex;

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DisplayLanguage("是否基本农田", IsLanguageName = false)]
        [DescriptionLanguage("是否基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsFarmerLandIndex
        {
            get { return isFarmerLandIndex; }
            set { isFarmerLandIndex = value; NotifyPropertyChanged("IsFarmerLandIndex"); }
        }

        private int isFarmerLandIndex;

        /// <summary>
        ///指界人
        /// </summary>
        [DisplayLanguage("指界人", IsLanguageName = false)]
        [DescriptionLanguage("指界人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ReferPersonIndex
        {
            get { return referPersonIndex; }
            set { referPersonIndex = value; NotifyPropertyChanged("ReferPersonIndex"); }
        }

        private int referPersonIndex;

        /// <summary>
        ///地块类别
        /// </summary>
        [DisplayLanguage("地块类别", IsLanguageName = false)]
        [DescriptionLanguage("地块类别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ArableTypeIndex
        {
            get { return arableTypeIndex; }
            set { arableTypeIndex = value; NotifyPropertyChanged("ArableTypeIndex"); }
        }

        private int arableTypeIndex;

        /// <summary>
        ///合同总面积
        /// </summary>
        [DisplayLanguage("合同总面积", IsLanguageName = false)]
        [DescriptionLanguage("合同总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TotalTableAreaIndex
        {
            get { return totalTableAreaIndex; }
            set { totalTableAreaIndex = value; NotifyPropertyChanged("TotalTableAreaIndex"); }
        }

        private int totalTableAreaIndex;

        /// <summary>
        ///实测总面积
        /// </summary>
        [DisplayLanguage("实测总面积", IsLanguageName = false)]
        [DescriptionLanguage("实测总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TotalActualAreaIndex
        {
            get { return totalActualAreaIndex; }
            set { totalActualAreaIndex = value; NotifyPropertyChanged("TotalActualAreaIndex"); }
        }

        private int totalActualAreaIndex;

        /// <summary>
        ///确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AwareAreaIndex
        {
            get { return awareAreaIndex; }
            set { awareAreaIndex = value; NotifyPropertyChanged("AwareAreaIndex"); }
        }

        private int awareAreaIndex;

        /// <summary>
        ///确权总面积
        /// </summary>
        [DisplayLanguage("确权总面积", IsLanguageName = false)]
        [DescriptionLanguage("确权总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TotalAwareAreaIndex
        {
            get { return totalAwareAreaIndex; }
            set { totalAwareAreaIndex = value; NotifyPropertyChanged("TotalAwareAreaIndex"); }
        }

        private int totalAwareAreaIndex;

        /// <summary>
        ///机动地面积
        /// </summary>
        [DisplayLanguage("机动地面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int MotorizeAreaIndex
        {
            get { return motorizeAreaIndex; }
            set { motorizeAreaIndex = value; NotifyPropertyChanged("MotorizeAreaIndex"); }
        }

        private int motorizeAreaIndex;

        /// <summary>
        ///机动地总面积
        /// </summary>
        [DisplayLanguage("机动地总面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TotalMotorizeAreaIndex
        {
            get { return totalMotorizeAreaIndex; }
            set { totalMotorizeAreaIndex = value; NotifyPropertyChanged("TotalMotorizeAreaIndex"); }
        }

        private int totalMotorizeAreaIndex;

        /// <summary>
        ///延包面积
        /// </summary>
        [DisplayLanguage("延包面积", IsLanguageName = false)]
        [DescriptionLanguage("延包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractDelayAreaIndex
        {
            get { return contractDelayAreaIndex; }
            set { contractDelayAreaIndex = value; NotifyPropertyChanged("ContractDelayAreaIndex"); }
        }

        private int contractDelayAreaIndex;

        /// <summary>
        ///延包总面积
        /// </summary>
        [DisplayLanguage("延包总面积", IsLanguageName = false)]
        [DescriptionLanguage("延包总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TotalContractDelayAreaIndex
        {
            get { return totalContractDelayAreaIndex; }
            set { totalContractDelayAreaIndex = value; NotifyPropertyChanged("TotalContractDelayAreaIndex"); }
        }

        private int totalContractDelayAreaIndex;

        /// <summary>
        ///地块的承包方式
        /// </summary>
        [DisplayLanguage("承包方式", IsLanguageName = false)]
        [DescriptionLanguage("承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ConstructModeIndex
        {
            get { return constructModeIndex; }
            set { constructModeIndex = value; NotifyPropertyChanged("ConstructModeIndex"); }
        }

        private int constructModeIndex;

        /// <summary>
        ///畦数
        /// </summary>
        [DisplayLanguage("畦数", IsLanguageName = false)]
        [DescriptionLanguage("畦数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PlotNumberIndex
        {
            get { return plotNumberIndex; }
            set { plotNumberIndex = value; NotifyPropertyChanged("PlotNumberIndex"); }
        }

        private int plotNumberIndex;

        /// <summary>
        ///种植类型
        /// </summary>
        [DisplayLanguage("种植类型", IsLanguageName = false)]
        [DescriptionLanguage("种植类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PlatTypeIndex
        {
            get { return platTypeIndex; }
            set { platTypeIndex = value; NotifyPropertyChanged("PlatTypeIndex"); }
        }

        private int platTypeIndex;

        /// <summary>
        ///经营方式
        /// </summary>
        [DisplayLanguage("经营方式", IsLanguageName = false)]
        [DescriptionLanguage("经营方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ManagementTypeIndex
        {
            get { return managementTypeIndex; }
            set { managementTypeIndex = value; NotifyPropertyChanged("ManagementTypeIndex"); }
        }

        private int managementTypeIndex;

        /// <summary>
        ///耕保类型
        /// </summary>
        [DisplayLanguage("耕保类型", IsLanguageName = false)]
        [DescriptionLanguage("耕保类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandPlantIndex
        {
            get { return landPlantIndex; }
            set { landPlantIndex = value; NotifyPropertyChanged("LandPlantIndex"); }
        }

        private int landPlantIndex;

        /// <summary>
        ///原户主姓名
        /// </summary>
        [DisplayLanguage("原户主姓名", IsLanguageName = false)]
        [DescriptionLanguage("原户主姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SourceNameIndex
        {
            get { return sourceNameIndex; }
            set { sourceNameIndex = value; NotifyPropertyChanged("SourceNameIndex"); }
        }

        private int sourceNameIndex;

        /// <summary>
        ///座落方位
        /// </summary>
        [DisplayLanguage("座落方位", IsLanguageName = false)]
        [DescriptionLanguage("座落方位", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandLocationIndex
        {
            get { return landLocationIndex; }
            set { landLocationIndex = value; NotifyPropertyChanged("LandLocationIndex"); }
        }

        private int landLocationIndex;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("承包地共有人", IsLanguageName = false)]
        [DescriptionLanguage("承包地共有人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SharePersonIndex
        {
            get { return sharePersonIndex; }
            set { sharePersonIndex = value; NotifyPropertyChanged("SharePersonIndex"); }
        }

        private int sharePersonIndex;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ConcordIndex
        {
            get { return concordIndex; }
            set { concordIndex = value; NotifyPropertyChanged("ConcordIndex"); }
        }

        private int concordIndex;

        /// <summary>
        ///权证编号
        /// </summary>
        [DisplayLanguage("权证编号", IsLanguageName = false)]
        [DescriptionLanguage("权证编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int RegeditBookIndex
        {
            get { return regeditBookIndex; }
            set { regeditBookIndex = value; NotifyPropertyChanged("RegeditBookIndex"); }
        }

        private int regeditBookIndex;

        /// <summary>
        ///是否流转
        /// </summary>
        [DisplayLanguage("是否流转", IsLanguageName = false)]
        [DescriptionLanguage("是否流转", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsTransterIndex
        {
            get { return isTransterIndex; }
            set { isTransterIndex = value; NotifyPropertyChanged("IsTransterIndex"); }
        }

        private int isTransterIndex;

        /// <summary>
        ///流转方式
        /// </summary>
        [DisplayLanguage("流转方式", IsLanguageName = false)]
        [DescriptionLanguage("流转方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TransterModeIndex
        {
            get { return transterModeIndex; }
            set { transterModeIndex = value; NotifyPropertyChanged("TransterModeIndex"); }
        }

        private int transterModeIndex;

        /// <summary>
        ///流转期限
        /// </summary>
        [DisplayLanguage("流转期限", IsLanguageName = false)]
        [DescriptionLanguage("流转期限", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TransterTermIndex
        {
            get { return transterTermIndex; }
            set { transterTermIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }

        private int transterTermIndex;

        /// <summary>
        ///流转面积
        /// </summary>
        [DisplayLanguage("流转面积", IsLanguageName = false)]
        [DescriptionLanguage("流转面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TransterAreaIndex
        {
            get { return transterAreaIndex; }
            set { transterAreaIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }

        private int transterAreaIndex;

        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CommentIndex
        {
            get { return commentIndex; }
            set { commentIndex = value; NotifyPropertyChanged("CommentIndex"); }
        }

        private int commentIndex;

        /// <summary>
        /// 地块信息修改意见
        /// </summary>
        [DisplayLanguage("地块信息修改意见", IsLanguageName = false)]
        [DescriptionLanguage("地块信息修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "实测地块信息", Gallery = "调查信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int OpinionIndex
        {
            get { return opinionIndex; }
            set { opinionIndex = value; NotifyPropertyChanged("OpinionIndex"); }
        }

        private int opinionIndex;

        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNameIndex
        {
            get { return secondNameIndex; }
            set { secondNameIndex = value; NotifyPropertyChanged("SecondNameIndex"); }
        }

        private int secondNameIndex;

        /// <summary>
        ///家庭成员个数
        /// </summary>
        [DisplayLanguage("家庭成员个数", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberIndex
        {
            get { return secondNumberIndex; }
            set { secondNumberIndex = value; NotifyPropertyChanged("SecondNumberIndex"); }
        }

        private int secondNumberIndex;

        /// <summary>
        ///家庭成员姓名
        /// </summary>
        [DisplayLanguage("家庭成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberNameIndex
        {
            get { return secondNumberNameIndex; }
            set { secondNumberNameIndex = value; NotifyPropertyChanged("SecondNumberNameIndex"); }
        }

        private int secondNumberNameIndex;

        /// <summary>
        ///家庭成员性别
        /// </summary>
        [DisplayLanguage("家庭成员性别", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员性别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberGenderIndex
        {
            get { return secondNumberGenderIndex; }
            set { secondNumberGenderIndex = value; NotifyPropertyChanged("SecondNumberGenderIndex"); }
        }

        private int secondNumberGenderIndex;

        /// <summary>
        ///家庭成员年龄
        /// </summary>
        [DisplayLanguage("家庭成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员年龄", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberAgeIndex
        {
            get { return secondNumberAgeIndex; }
            set { secondNumberAgeIndex = value; NotifyPropertyChanged("SecondNumberAgeIndex"); }
        }

        private int secondNumberAgeIndex;

        /// <summary>
        ///成员身份证号
        /// </summary>
        [DisplayLanguage("成员身份证号", IsLanguageName = false)]
        [DescriptionLanguage("成员身份证号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberIcnIndex
        {
            get { return secondNumberIcnIndex; }
            set { secondNumberIcnIndex = value; NotifyPropertyChanged("SecondNumberIcnIndex"); }
        }

        private int secondNumberIcnIndex;

        /// <summary>
        ///家庭成员关系
        /// </summary>
        [DisplayLanguage("家庭成员关系", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员关系", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNumberRelatioinIndex
        {
            get { return secondNumberRelatioinIndex; }
            set { secondNumberRelatioinIndex = value; NotifyPropertyChanged("SecondNumberRelatioinIndex"); }
        }

        private int secondNumberRelatioinIndex;

        /// <summary>
        ///家庭成员备注
        /// </summary>
        [DisplayLanguage("家庭成员备注", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondFamilyCommentIndex
        {
            get { return secondFamilyCommentIndex; }
            set { secondFamilyCommentIndex = value; NotifyPropertyChanged("SecondFamilyCommentIndex"); }
        }

        private int secondFamilyCommentIndex;

        /// <summary>
        ///二轮延包姓名
        /// </summary>
        [DisplayLanguage("二轮延包姓名", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ExPackageNameIndex
        {
            get { return exPackageNameIndex; }
            set { exPackageNameIndex = value; NotifyPropertyChanged("ExPackageNameIndex"); }
        }

        private int exPackageNameIndex;

        /// <summary>
        ///延包土地份数
        /// </summary>
        [DisplayLanguage("延包土地份数", IsLanguageName = false)]
        [DescriptionLanguage("延包土地份数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ExPackageNumberIndex
        {
            get { return exPackageNumberIndex; }
            set { exPackageNumberIndex = value; NotifyPropertyChanged("ExPackageNumberIndex"); }
        }

        private int exPackageNumberIndex;

        /// <summary>
        ///已死亡人员
        /// </summary>
        [DisplayLanguage("已死亡人员", IsLanguageName = false)]
        [DescriptionLanguage("已死亡人员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsDeadedIndex
        {
            get { return isDeadedIndex; }
            set { isDeadedIndex = value; NotifyPropertyChanged("IsDeadedIndex"); }
        }

        private int isDeadedIndex;

        /// <summary>
        ///出嫁后未退承包地人员
        /// </summary>
        [DisplayLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LocalMarriedRetreatLandIndex
        {
            get { return localMarriedRetreatLandIndex; }
            set { localMarriedRetreatLandIndex = value; NotifyPropertyChanged("LocalMarriedRetreatLandIndex"); }
        }

        private int localMarriedRetreatLandIndex;

        /// <summary>
        ///农转非未退承包地
        /// </summary>
        [DisplayLanguage("农转非未退承包地", IsLanguageName = false)]
        [DescriptionLanguage("农转非未退承包地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PeasantsRetreatLandIndex
        {
            get { return peasantsRetreatLandIndex; }
            set { peasantsRetreatLandIndex = value; NotifyPropertyChanged("PeasantsRetreatLandIndex"); }
        }

        private int peasantsRetreatLandIndex;

        /// <summary>
        ///婚进未退承包地
        /// </summary>
        [DisplayLanguage("婚进未退承包地", IsLanguageName = false)]
        [DescriptionLanguage("婚进未退承包地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ForeignMarriedRetreatLandIndex
        {
            get { return foreignMarriedRetreatLandIndex; }
            set { foreignMarriedRetreatLandIndex = value; NotifyPropertyChanged("ForeignMarriedRetreatLandIndex"); }
        }

        private int foreignMarriedRetreatLandIndex;

        /// <summary>
        ///民族
        /// </summary>
        [DisplayLanguage("民族", IsLanguageName = false)]
        [DescriptionLanguage("民族", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNationIndex
        {
            get { return secondNationIndex; }
            set { secondNationIndex = value; NotifyPropertyChanged("SecondNationIndex"); }
        }

        private int secondNationIndex;

        /// <summary>
        ///出生日期
        /// </summary>
        [DisplayLanguage("出生日期", IsLanguageName = false)]
        [DescriptionLanguage("出生日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondAgeIndex
        {
            get { return secondAgeIndex; }
            set { secondAgeIndex = value; NotifyPropertyChanged("SecondAgeIndex"); }
        }

        private int secondAgeIndex;

        /// <summary>
        ///一轮承包人数
        /// </summary>
        [DisplayLanguage("一轮承包人数", IsLanguageName = false)]
        [DescriptionLanguage("一轮承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FirstContractorPersonNumberIndex
        {
            get { return firstContractorPersonNumberIndex; }
            set { firstContractorPersonNumberIndex = value; NotifyPropertyChanged("FirstContractorPersonNumberIndex"); }
        }

        private int firstContractorPersonNumberIndex;

        /// <summary>
        ///一轮承包面积
        /// </summary>
        [DisplayLanguage("一轮承包面积", IsLanguageName = false)]
        [DescriptionLanguage("一轮承包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FirstContractAreaIndex
        {
            get { return firstContractAreaIndex; }
            set { firstContractAreaIndex = value; NotifyPropertyChanged("FirstContractAreaIndex"); }
        }

        private int firstContractAreaIndex;

        /// <summary>
        ///二轮承包人数
        /// </summary>
        [DisplayLanguage("二轮承包人数", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondContractorPersonNumberIndex
        {
            get { return secondContractorPersonNumberIndex; }
            set { secondContractorPersonNumberIndex = value; NotifyPropertyChanged("SecondContractorPersonNumberIndex"); }
        }

        private int secondContractorPersonNumberIndex;

        /// <summary>
        ///二轮延包面积
        /// </summary>
        [DisplayLanguage("二轮延包面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondExtensionPackAreaIndex
        {
            get { return secondExtensionPackAreaIndex; }
            set { secondExtensionPackAreaIndex = value; NotifyPropertyChanged("SecondExtensionPackAreaIndex"); }
        }

        private int secondExtensionPackAreaIndex;

        /// <summary>
        ///粮食种植面积
        /// </summary>
        [DisplayLanguage("粮食种植面积", IsLanguageName = false)]
        [DescriptionLanguage("粮食种植面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FoodCropAreaIndex
        {
            get { return foodCropAreaIndex; }
            set { foodCropAreaIndex = value; NotifyPropertyChanged("FoodCropAreaIndex"); }
        }

        private int foodCropAreaIndex;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondConcordNumberIndex
        {
            get { return secondConcordNumberIndex; }
            set { secondConcordNumberIndex = value; NotifyPropertyChanged("SecondConcordNumberIndex"); }
        }

        private int secondConcordNumberIndex;

        /// <summary>
        ///权证编号
        /// </summary>
        [DisplayLanguage("权证编号", IsLanguageName = false)]
        [DescriptionLanguage("权证编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondWarrantNumberIndex
        {
            get { return secondWarrantNumberIndex; }
            set { secondWarrantNumberIndex = value; NotifyPropertyChanged("SecondWarrantNumberIndex"); }
        }

        private int secondWarrantNumberIndex;

        /// <summary>
        ///起始日期
        /// </summary>
        [DisplayLanguage("起始日期", IsLanguageName = false)]
        [DescriptionLanguage("起始日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int StartTimeIndex
        {
            get { return startTimeIndex; }
            set { startTimeIndex = value; NotifyPropertyChanged("StartTimeIndex"); }
        }

        private int startTimeIndex;

        /// <summary>
        ///结束日期
        /// </summary>
        [DisplayLanguage("结束日期", IsLanguageName = false)]
        [DescriptionLanguage("结束日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int EndTimeIndex
        {
            get { return endTimeIndex; }
            set { endTimeIndex = value; NotifyPropertyChanged("EndTimeIndex"); }
        }

        private int endTimeIndex;

        /// <summary>
        ///取得承包方式
        /// </summary>
        [DisplayLanguage("取得承包方式", IsLanguageName = false)]
        [DescriptionLanguage("取得承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ConstructTypeIndex
        {
            get { return constructTypeIndex; }
            set { constructTypeIndex = value; NotifyPropertyChanged("ConstructTypeIndex"); }
        }

        private int constructTypeIndex;

        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondLandNameIndex
        {
            get { return secondLandNameIndex; }
            set { secondLandNameIndex = value; NotifyPropertyChanged("SecondLandNameIndex"); }
        }

        private int secondLandNameIndex;

        /// <summary>
        ///地类
        /// </summary>
        [DisplayLanguage("地类", IsLanguageName = false)]
        [DescriptionLanguage("地类", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondLandTypeIndex
        {
            get { return secondLandTypeIndex; }
            set { secondLandTypeIndex = value; NotifyPropertyChanged("SecondLandTypeIndex"); }
        }

        private int secondLandTypeIndex;

        /// <summary>
        ///台账面积
        /// </summary>
        [DisplayLanguage("台账面积", IsLanguageName = false)]
        [DescriptionLanguage("台账面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondTableAreaIndex
        {
            get { return secondTableAreaIndex; }
            set { secondTableAreaIndex = value; NotifyPropertyChanged("SecondTableAreaIndex"); }
        }

        private int secondTableAreaIndex;

        /// <summary>
        ///台账总面积
        /// </summary>
        [DisplayLanguage("台账总面积", IsLanguageName = false)]
        [DescriptionLanguage("台账总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondTotalTableAreaIndex
        {
            get { return secondTotalTableAreaIndex; }
            set { secondTotalTableAreaIndex = value; NotifyPropertyChanged("SecondTotalTableAreaIndex"); }
        }

        private int secondTotalTableAreaIndex;

        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondCommentIndex
        {
            get { return secondCommentIndex; }
            set { secondCommentIndex = value; NotifyPropertyChanged("SecondCommentIndex"); }
        }

        private int secondCommentIndex;

        /// <summary>
        ///四至东
        /// </summary>
        [DisplayLanguage("四至东", IsLanguageName = false)]
        [DescriptionLanguage("四至东", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondEastIndex
        {
            get { return secondEastIndex; }
            set { secondEastIndex = value; NotifyPropertyChanged("SecondEastIndex"); }
        }

        private int secondEastIndex;

        /// <summary>
        ///四至南
        /// </summary>
        [DisplayLanguage("四至南", IsLanguageName = false)]
        [DescriptionLanguage("四至南", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondSourthIndex
        {
            get { return secondSourthIndex; }
            set { secondSourthIndex = value; NotifyPropertyChanged("SecondSourthIndex"); }
        }

        private int secondSourthIndex;

        /// <summary>
        ///四至西
        /// </summary>
        [DisplayLanguage("四至西", IsLanguageName = false)]
        [DescriptionLanguage("四至西", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondWestIndex
        {
            get { return secondWestIndex; }
            set { secondWestIndex = value; NotifyPropertyChanged("SecondWestIndex"); }
        }

        private int secondWestIndex;

        /// <summary>
        ///四至北
        /// </summary>
        [DisplayLanguage("四至北", IsLanguageName = false)]
        [DescriptionLanguage("四至北", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondNorthIndex
        {
            get { return secondNorthIndex; }
            set { secondNorthIndex = value; NotifyPropertyChanged("SecondNorthIndex"); }
        }

        private int secondNorthIndex;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondLandNumberIndex
        {
            get { return secondLandNumberIndex; }
            set { secondLandNumberIndex = value; NotifyPropertyChanged("SecondLandNumberIndex"); }
        }

        private int secondLandNumberIndex;

        /// <summary>
        ///土地类型
        /// </summary>
        [DisplayLanguage("土地类型", IsLanguageName = false)]
        [DescriptionLanguage("土地类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondArableTypeIndex
        {
            get { return secondArableTypeIndex; }
            set { secondArableTypeIndex = value; NotifyPropertyChanged("SecondArableTypeIndex"); }
        }

        private int secondArableTypeIndex;

        /// <summary>
        ///基本农田
        /// </summary>
        [DisplayLanguage("基本农田", IsLanguageName = false)]
        [DescriptionLanguage("基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondIsFarmerLandIndex
        {
            get { return secondIsFarmerLandIndex; }
            set { secondIsFarmerLandIndex = value; NotifyPropertyChanged("SecondIsFarmerLandIndex"); }
        }

        private int secondIsFarmerLandIndex;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondLandPurposeIndex
        {
            get { return secondLandPurposeIndex; }
            set { secondLandPurposeIndex = value; NotifyPropertyChanged("SecondLandPurposeIndex"); }
        }

        private int secondLandPurposeIndex;

        /// <summary>
        ///地块等级
        /// </summary>
        [DisplayLanguage("地块等级", IsLanguageName = false)]
        [DescriptionLanguage("地块等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包地块情况", Gallery = "二轮信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SecondLandLevelIndex
        {
            get { return secondLandLevelIndex; }
            set { secondLandLevelIndex = value; NotifyPropertyChanged("SecondLandLevelIndex"); }
        }

        private int secondLandLevelIndex;

        /// <summary>
        ///出生日期
        /// </summary>
        [DisplayLanguage("出生日期", IsLanguageName = false)]
        [DescriptionLanguage("出生日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AgeIndex
        {
            get { return ageIndex; }
            set { ageIndex = value; NotifyPropertyChanged("AgeIndex"); }
        }

        private int ageIndex;

        /// <summary>
        ///民族
        /// </summary>
        [DisplayLanguage("民族", IsLanguageName = false)]
        [DescriptionLanguage("民族", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int NationIndex
        {
            get { return nationIndex; }
            set { nationIndex = value; NotifyPropertyChanged("NationIndex"); }
        }

        private int nationIndex;

        /// <summary>
        ///户口性质
        /// </summary>
        [DisplayLanguage("户口性质", IsLanguageName = false)]
        [DescriptionLanguage("户口性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AccountNatureIndex
        {
            get { return accountNatureIndex; }
            set { accountNatureIndex = value; NotifyPropertyChanged("AccountNatureIndex"); }
        }

        private int accountNatureIndex;

        /// <summary>
        ///农户性质
        /// </summary>
        [DisplayLanguage("农户性质", IsLanguageName = false)]
        [DescriptionLanguage("农户性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FarmerNatureIndex
        {
            get { return farmerNatureIndex; }
            set { farmerNatureIndex = value; NotifyPropertyChanged("FarmerNatureIndex"); }
        }

        private int farmerNatureIndex;

        /// <summary>
        ///是否原承包户
        /// </summary>
        [DisplayLanguage("是否原承包户", IsLanguageName = false)]
        [DescriptionLanguage("是否原承包户", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsSourceContractorIndex
        {
            get { return isSourceContractorIndex; }
            set { isSourceContractorIndex = value; NotifyPropertyChanged("IsSourceContractorIndex"); }
        }

        private int isSourceContractorIndex;

        /// <summary>
        ///现承包人数
        /// </summary>
        [DisplayLanguage("现承包人数", IsLanguageName = false)]
        [DescriptionLanguage("现承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int ContractorNumberIndex
        {
            get { return contractorNumberIndex; }
            set { contractorNumberIndex = value; NotifyPropertyChanged("ContractorNumberIndex"); }
        }

        private int contractorNumberIndex;

        /// <summary>
        ///总劳力数
        /// </summary>
        [DisplayLanguage("总劳力数", IsLanguageName = false)]
        [DescriptionLanguage("总劳力数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LaborNumberIndex
        {
            get { return laborNumberIndex; }
            set { laborNumberIndex = value; NotifyPropertyChanged("LaborNumberIndex"); }
        }

        private int laborNumberIndex;

        /// <summary>
        ///户籍备注
        /// </summary>
        [DisplayLanguage("户籍备注", IsLanguageName = false)]
        [DescriptionLanguage("户籍备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int CencueCommentIndex
        {
            get { return cencueCommentIndex; }
            set { cencueCommentIndex = value; NotifyPropertyChanged("CencueCommentIndex"); }
        }

        private int cencueCommentIndex;

        /// <summary>
        ///从何处迁入
        /// </summary>
        [DisplayLanguage("从何处迁入", IsLanguageName = false)]
        [DescriptionLanguage("从何处迁入", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int SourceMoveIndex
        {
            get { return sourceMoveIndex; }
            set { sourceMoveIndex = value; NotifyPropertyChanged("SourceMoveIndex"); }
        }

        private int sourceMoveIndex;

        /// <summary>
        ///迁入时间
        /// </summary>
        [DisplayLanguage("迁入时间", IsLanguageName = false)]
        [DescriptionLanguage("迁入时间", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int MoveTimeIndex
        {
            get { return moveTimeIndex; }
            set { moveTimeIndex = value; NotifyPropertyChanged("MoveTimeIndex"); }
        }

        private int moveTimeIndex;

        /// <summary>
        ///迁入前土地类型
        /// </summary>
        [DisplayLanguage("迁入前土地类型", IsLanguageName = false)]
        [DescriptionLanguage("迁入前土地类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int MoveFormerlyLandTypeIndex
        {
            get { return moveFormerlyLandTypeIndex; }
            set { moveFormerlyLandTypeIndex = value; NotifyPropertyChanged("MoveFormerlyLandTypeIndex"); }
        }

        private int moveFormerlyLandTypeIndex;

        /// <summary>
        ///迁入前土地面积
        /// </summary>
        [DisplayLanguage("迁入前土地面积", IsLanguageName = false)]
        [DescriptionLanguage("迁入前土地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int MoveFormerlyLandAreaIndex
        {
            get { return moveFormerlyLandAreaIndex; }
            set { moveFormerlyLandAreaIndex = value; NotifyPropertyChanged("MoveFormerlyLandAreaIndex"); }
        }

        private int moveFormerlyLandAreaIndex;

        /// <summary>
        ///是否为99年共有人
        /// </summary>
        [DisplayLanguage("是否为99年共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否为99年共有人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IsNinetyNineSharePersonIndex
        {
            get { return isNinetyNineSharePersonIndex; }
            set { isNinetyNineSharePersonIndex = value; NotifyPropertyChanged("IsNinetyNineSharePersonIndex"); }
        }

        private int isNinetyNineSharePersonIndex;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilySurveyPersonIndex
        {
            get { return familySurveyPersonIndex; }
            set { familySurveyPersonIndex = value; NotifyPropertyChanged("FamilySurveyPersonIndex"); }
        }

        private int familySurveyPersonIndex;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilySurveyDateIndex
        {
            get { return familySurveyDateIndex; }
            set { familySurveyDateIndex = value; NotifyPropertyChanged("FamilySurveyDateIndex"); }
        }

        private int familySurveyDateIndex;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilySurveyChronicleIndex
        {
            get { return familySurveyChronicleIndex; }
            set { familySurveyChronicleIndex = value; NotifyPropertyChanged("FamilySurveyChronicleIndex"); }
        }

        private int familySurveyChronicleIndex;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilyCheckPersonIndex
        {
            get { return familyCheckPersonIndex; }
            set { familyCheckPersonIndex = value; NotifyPropertyChanged("FamilyCheckPersonIndex"); }
        }

        private int familyCheckPersonIndex;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilyCheckDateIndex
        {
            get { return familyCheckDateIndex; }
            set { familyCheckDateIndex = value; NotifyPropertyChanged("FamilyCheckDateIndex"); }
        }

        private int familyCheckDateIndex;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int FamilyCheckOpinionIndex
        {
            get { return familyCheckOpinionIndex; }
            set { familyCheckOpinionIndex = value; NotifyPropertyChanged("FamilyCheckOpinionIndex"); }
        }

        private int familyCheckOpinionIndex;

        /// <summary>
        ///邮政编码
        /// </summary>
        [DisplayLanguage("邮政编码", IsLanguageName = false)]
        [DescriptionLanguage("邮政编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int PostNumberIndex
        {
            get { return postNumberIndex; }
            set { postNumberIndex = value; NotifyPropertyChanged("PostNumberIndex"); }
        }

        private int postNumberIndex;

        /// <summary>
        ///电话号码
        /// </summary>
        [DisplayLanguage("电话号码", IsLanguageName = false)]
        [DescriptionLanguage("电话号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int TelephoneIndex
        {
            get { return telephoneIndex; }
            set { telephoneIndex = value; NotifyPropertyChanged("TelephoneIndex"); }
        }

        private int telephoneIndex;

        /// <summary>
        ///实际分配人数
        /// </summary>
        [DisplayLanguage("实际分配人数", IsLanguageName = false)]
        [DescriptionLanguage("实际分配人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int AllocationPersonIndex
        {
            get { return allocationPersonIndex; }
            set { allocationPersonIndex = value; NotifyPropertyChanged("AllocationPersonIndex"); }
        }

        private int allocationPersonIndex;

        /// <summary>
        ///承包方扩展里承包方式
        /// </summary>
        [DisplayLanguage("取得承包方式", IsLanguageName = false)]
        [DescriptionLanguage("取得承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int VPConstructModeIndex
        {
            get { return vPConstructModeIndex; }
            set { vPConstructModeIndex = value; NotifyPropertyChanged("VPConstructModeIndex"); }
        }

        private int vPConstructModeIndex;

        /// <summary>
        ///利用情况
        /// </summary>
        [DisplayLanguage("利用情况", IsLanguageName = false)]
        [DescriptionLanguage("利用情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int UseSituationIndex
        {
            get { return useSituationIndex; }
            set { useSituationIndex = value; NotifyPropertyChanged("UseSituationIndex"); }
        }

        private int useSituationIndex;

        /// <summary>
        ///产量情况
        /// </summary>
        [DisplayLanguage("产量情况", IsLanguageName = false)]
        [DescriptionLanguage("产量情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int YieldIndex
        {
            get { return yieldIndex; }
            set { yieldIndex = value; NotifyPropertyChanged("YieldIndex"); }
        }

        private int yieldIndex;

        /// <summary>
        ///产值情况
        /// </summary>
        [DisplayLanguage("产值情况", IsLanguageName = false)]
        [DescriptionLanguage("产值情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int OutputValueIndex
        {
            get { return outputValueIndex; }
            set { outputValueIndex = value; NotifyPropertyChanged("OutputValueIndex"); }
        }

        private int outputValueIndex;

        /// <summary>
        ///收益情况
        /// </summary>
        [DisplayLanguage("收益情况", IsLanguageName = false)]
        [DescriptionLanguage("收益情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int IncomeSituationIndex
        {
            get { return incomeSituationIndex; }
            set { incomeSituationIndex = value; NotifyPropertyChanged("IncomeSituationIndex"); }
        }

        private int incomeSituationIndex;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandSurveyPersonIndex
        {
            get { return landSurveyPersonIndex; }
            set { landSurveyPersonIndex = value; NotifyPropertyChanged("LandSurveyPersonIndex"); }
        }

        private int landSurveyPersonIndex;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandSurveyDateIndex
        {
            get { return landSurveyDateIndex; }
            set { landSurveyDateIndex = value; NotifyPropertyChanged("LandSurveyDateIndex"); }
        }

        private int landSurveyDateIndex;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandSurveyChronicleIndex
        {
            get { return landSurveyChronicleIndex; }
            set { landSurveyChronicleIndex = value; NotifyPropertyChanged("LandSurveyChronicleIndex"); }
        }

        private int landSurveyChronicleIndex;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandCheckPersonIndex
        {
            get { return landCheckPersonIndex; }
            set { landCheckPersonIndex = value; NotifyPropertyChanged("LandCheckPersonIndex"); }
        }

        private int landCheckPersonIndex;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandCheckDateIndex
        {
            get { return landCheckDateIndex; }
            set { landCheckDateIndex = value; NotifyPropertyChanged("LandCheckDateIndex"); }
        }

        private int landCheckDateIndex;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
           Builder = typeof(PropertyDescriptorBuilderSelector), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public int LandCheckOpinionIndex
        {
            get { return landCheckOpinionIndex; }
            set { landCheckOpinionIndex = value; NotifyPropertyChanged("LandCheckOpinionIndex"); }
        }

        private int landCheckOpinionIndex;

        /// <summary>
        /// 是否包含二轮台账承包方数据
        /// </summary>
        public bool IsContainTableValue
        {
            get { return InitalizeTableValue(); }
        }

        /// <summary>
        /// 是否检查二轮台账户籍数据
        /// </summary>
        public bool CanCheckTableValue
        {
            get { return IsCheckTableValue(); }
        }

        /// <summary>
        /// 是否包含承包地块信息
        /// </summary>
        public bool IsContainTableLandValue
        {
            get { return InitalizeTableLandValue(); }
        }

        /// <summary>
        /// 是否含有户籍籍贯信息
        /// </summary>
        public bool IsCencusTableValue
        {
            get
            {
                return InitalzieCencusTableValue();
            }
        }

        #endregion Propertys

        #region Ctor

        public ContractBusinessImportSurveyDefine()
        {
            NameIndex = 1;
            ContractorTypeIndex = 2;
            SecondNameIndex = -1;
            NumberIndex = 3;
            SecondNumberIndex = -1;
            NumberNameIndex = 4;
            SecondNumberNameIndex = -1;
            NumberGenderIndex = 5;
            SecondNumberGenderIndex = -1;
            NumberAgeIndex = -1;
            SecondNumberAgeIndex = -1;
            NumberCartTypeIndex = 6;
            NumberIcnIndex = 7;
            SecondNumberIcnIndex = -1;
            NumberRelatioinIndex = 8;
            SecondNumberRelatioinIndex = -1;
            IsSharedLandIndex = 9;
            FamilyCommentIndex = 10;
            FamilyOpinionIndex = 11;
            ContractorAddressIndex = 12;
            PostNumberIndex = 13;
            TelephoneIndex = 14;
            FamilySurveyPersonIndex = 15;
            FamilySurveyDateIndex = 16;
            FamilySurveyChronicleIndex = 17;
            FamilyCheckPersonIndex = 18;
            FamilyCheckDateIndex = 19;
            FamilyCheckOpinionIndex = 120;
            LandNameIndex = 21;
            CadastralNumberIndex = 22;
            ImageNumberIndex = 23;
            TableAreaIndex = 24;
            ActualAreaIndex = 25;
            ContractDelayAreaIndex = 26;
            AwareAreaIndex = -1;   //确权面积
            EastIndex = 27;
            SourthIndex = 28;
            WestIndex = 29;
            NorthIndex = 30;
            LandPurposeIndex = 31;
            LandLevelIndex = 32;
            LandTypeIndex = 33;
            IsFarmerLandIndex = 34;
            ReferPersonIndex = 35;
            ArableTypeIndex = 36;//地块类别
            CommentIndex = 37;
            OpinionIndex = 38;
            LandSurveyPersonIndex = 39;
            LandSurveyDateIndex = 40;
            LandSurveyChronicleIndex = 41;
            LandCheckPersonIndex = 42;
            LandCheckDateIndex = 43;
            LandCheckOpinionIndex = 44;
            SecondFamilyCommentIndex = -1;
            ExPackageNameIndex = -1;
            ExPackageNumberIndex = -1;
            IsDeadedIndex = -1;
            LocalMarriedRetreatLandIndex = -1;
            PeasantsRetreatLandIndex = -1;
            ForeignMarriedRetreatLandIndex = -1;
            SharePersonIndex = -1;
            ConcordIndex = -1;
            RegeditBookIndex = -1;
            SecondLandNameIndex = -1;
            PlotNumberIndex = -1;
            TotalActualAreaIndex = -1;
            TotalAwareAreaIndex = -1;
            MotorizeAreaIndex = -1;
            TotalMotorizeAreaIndex = -1;
            SecondTableAreaIndex = -1;
            TotalTableAreaIndex = -1;
            SecondTotalTableAreaIndex = -1;
            SecondLandTypeIndex = -1;
            ManagementTypeIndex = -1;
            SecondEastIndex = -1;
            SecondSourthIndex = -1;
            SecondWestIndex = -1;
            SecondNorthIndex = -1;
            SourceNameIndex = -1;
            LandLocationIndex = -1;
            ConstructModeIndex = -1;
            IsTransterIndex = -1;
            TransterModeIndex = -1;
            TransterTermIndex = -1;
            TransterAreaIndex = -1;
            PlatTypeIndex = -1;
            LandPlantIndex = -1;

            SecondCommentIndex = -1;
            AllocationPersonIndex = -1;
            UseSituationIndex = -1;
            YieldIndex = -1;
            OutputValueIndex = -1;
            IncomeSituationIndex = -1;
            LaborNumberIndex = -1;
            IsSourceContractorIndex = -1;
            FarmerNatureIndex = -1;
            ContractorNumberIndex = -1;
            MoveFormerlyLandTypeIndex = -1;
            MoveFormerlyLandAreaIndex = -1;
            FirstContractorPersonNumberIndex = -1;
            FirstContractAreaIndex = -1;
            SecondContractorPersonNumberIndex = -1;
            SecondExtensionPackAreaIndex = -1;
            FoodCropAreaIndex = -1;
            AgeIndex = -1;
            AccountNatureIndex = -1;
            SourceMoveIndex = -1;
            MoveTimeIndex = -1;
            IsNinetyNineSharePersonIndex = -1;
            SecondAgeIndex = -1;
            SecondNationIndex = -1;
            NationIndex = -1;
            SecondLandNumberIndex = -1;
            SecondArableTypeIndex = -1;
            SecondIsFarmerLandIndex = -1;
            SecondLandPurposeIndex = -1;
            SecondLandLevelIndex = -1;
            CencueCommentIndex = -1;
            SecondConcordNumberIndex = -1;
            SecondWarrantNumberIndex = -1;
            StartTimeIndex = -1;
            EndTimeIndex = -1;
            ConstructTypeIndex = -1;//地块的
            VPConstructModeIndex = -1;//人的承包方式
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 初始化二轮台账信息
        /// </summary>
        /// <returns></returns>
        public bool InitalizeTableValue()
        {
            if (SecondNameIndex != -1)
            {
                return true;
            }
            if (ExPackageNameIndex != -1)
            {
                return true;
            }
            if (ExPackageNumberIndex != -1)
            {
                return true;
            }
            if (IsDeadedIndex != -1)
            {
                return true;
            }
            if (LocalMarriedRetreatLandIndex != -1)
            {
                return true;
            }
            if (PeasantsRetreatLandIndex != -1)
            {
                return true;
            }
            if (ForeignMarriedRetreatLandIndex != -1)
            {
                return true;
            }
            if (SharePersonIndex != -1)
            {
                return true;
            }
            if (SecondNumberIndex != -1)
            {
                return true;
            }
            if (SecondNumberNameIndex != -1)
            {
                return true;
            }
            if (SecondNumberGenderIndex != -1)
            {
                return true;
            }
            if (SecondNumberAgeIndex != -1)
            {
                return true;
            }
            if (SecondNumberIcnIndex != -1)
            {
                return true;
            }
            if (SecondNumberRelatioinIndex != -1)
            {
                return true;
            }
            if (SecondFamilyCommentIndex != -1)
            {
                return true;
            }
            if (SecondLandNameIndex != -1)
            {
                return true;
            }
            if (SecondLandTypeIndex != -1)
            {
                return true;
            }
            if (SecondTableAreaIndex != -1)
            {
                return true;
            }
            if (SecondTotalTableAreaIndex != -1)
            {
                return true;
            }
            if (SecondEastIndex != -1)
            {
                return true;
            }
            if (SecondSourthIndex != -1)
            {
                return true;
            }
            if (SecondWestIndex != -1)
            {
                return true;
            }
            if (SecondNorthIndex != -1)
            {
                return true;
            }
            if (SecondCommentIndex != -1)
            {
                return true;
            }
            if (SecondNationIndex != -1)
            {
                return true;
            }
            if (SecondAgeIndex != -1)
            {
                return true;
            }
            if (FirstContractorPersonNumberIndex != -1)
            {
                return true;
            }
            if (FirstContractAreaIndex != -1)
            {
                return true;
            }
            if (SecondContractorPersonNumberIndex != -1)
            {
                return true;
            }
            if (SecondExtensionPackAreaIndex != -1)
            {
                return true;
            }
            if (FoodCropAreaIndex != -1)
            {
                return true;
            }
            if (SecondLandNumberIndex != -1)
            {
                return true;
            }
            if (SecondLandPurposeIndex != -1)
            {
                return true;
            }
            if (SecondLandLevelIndex != -1)
            {
                return true;
            }
            if (SecondArableTypeIndex != -1)
            {
                return true;
            }
            if (SecondIsFarmerLandIndex != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否检查二轮信息
        /// </summary>
        /// <returns></returns>
        public bool IsCheckTableValue()
        {
            if (SecondNameIndex != -1)
            {
                return true;
            }
            if (ExPackageNameIndex != 1)
            {
                return true;
            }
            if (SecondNumberNameIndex != -1)
            {
                return true;
            }
            if (SecondNumberIcnIndex != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 初始化二轮台账地块信息
        /// </summary>
        /// <returns></returns>
        public bool InitalizeTableLandValue()
        {
            if (SecondLandNameIndex != -1)
            {
                return true;
            }
            if (SecondLandTypeIndex != -1)
            {
                return true;
            }
            if (SecondTableAreaIndex != -1)
            {
                return true;
            }
            if (SecondTotalTableAreaIndex != -1)
            {
                return true;
            }
            if (SecondEastIndex != -1)
            {
                return true;
            }
            if (SecondSourthIndex != -1)
            {
                return true;
            }
            if (SecondWestIndex != -1)
            {
                return true;
            }
            if (SecondNorthIndex != -1)
            {
                return true;
            }
            if (SecondCommentIndex != -1)
            {
                return true;
            }
            if (SecondLandNumberIndex != -1)
            {
                return true;
            }
            if (SecondLandPurposeIndex != -1)
            {
                return true;
            }
            if (SecondLandLevelIndex != -1)
            {
                return true;
            }
            if (SecondArableTypeIndex != -1)
            {
                return true;
            }
            if (SecondIsFarmerLandIndex != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 初始化二轮台账信息值
        /// </summary>
        public void InitalizeLedgerTableValue()
        {
            //string value = ToolConfiguration.GetSpecialAppSettingValue("ImportSecondTableWithFamilyComment", "true");//二轮台账备注
            //bool hasFamilyComment = value.ToLower() == "true";
            //NameIndex = -1;
            //SecondNameIndex = 1;
            //NumberIndex = -1;
            //SecondNumberIndex = value.ToLower() == "true" ? 8 : 7; ;
            //NumberNameIndex = -1;
            //SecondNumberNameIndex = 3;
            //NumberGenderIndex = -1;
            //SecondNumberGenderIndex = 4;
            //NumberAgeIndex = -1;
            //SecondNumberAgeIndex = -1;
            //NumberIcnIndex = -1;
            //SecondNumberIcnIndex = 5;
            //NumberRelatioinIndex = -1;
            //SecondNumberRelatioinIndex = 6;
            //FamilyCommentIndex = -1;
            //SecondFamilyCommentIndex = value.ToLower() == "true" ? 7 : -1;
            //ExPackageNameIndex = -1;
            //ExPackageNumberIndex = -1;
            //IsDeadedIndex = -1;
            //LocalMarriedRetreatLandIndex = -1;
            //PeasantsRetreatLandIndex = -1;
            //ForeignMarriedRetreatLandIndex = -1;
            //SharePersonIndex = -1;
            //IsSharedLandIndex = -1;
            //ConcordIndex = -1;
            //RegeditBookIndex = -1;
            //CadastralNumberIndex = -1;
            //LandNameIndex = -1;
            //SecondLandNameIndex = value.ToLower() == "true" ? 12 : 11;
            //PlotNumberIndex = -1;
            //ActualAreaIndex = -1;
            //TotalActualAreaIndex = -1;
            //AwareAreaIndex = -1;
            //TotalAwareAreaIndex = -1;
            //MotorizeAreaIndex = -1;
            //TotalMotorizeAreaIndex = -1;
            //TableAreaIndex = -1;
            //SecondTableAreaIndex = value.ToLower() == "true" ? 10 : 9;
            //TotalTableAreaIndex = -1;
            //SecondTotalTableAreaIndex = value.ToLower() == "true" ? 9 : 8;
            //LandTypeIndex = -1;
            //SecondLandTypeIndex = value.ToLower() == "true" ? 11 : 10;
            //ManagementTypeIndex = -1;
            //IsFarmerLandIndex = -1;
            //EastIndex = -1;
            //SecondEastIndex = value.ToLower() == "true" ? 13 : 12;
            //SourthIndex = -1;
            //SecondSourthIndex = value.ToLower() == "true" ? 14 : 13;
            //WestIndex = -1;
            //SecondWestIndex = value.ToLower() == "true" ? 15 : 14;
            //NorthIndex = -1;
            //SecondNorthIndex = value.ToLower() == "true" ? 16 : 15;
            //SourceNameIndex = -1;
            //LandLocationIndex = -1;
            //ConstructModeIndex = -1;
            //IsTransterIndex = -1;
            //TransterModeIndex = -1;
            //TransterTermIndex = -1;
            //TransterAreaIndex = -1;
            //PlatTypeIndex = -1;
            //LandLevelIndex = -1;
            //LandPlantIndex = -1;
            //ArableTypeIndex = -1;
            //TelephoneIndex = -1;
            //CommentIndex = -1;
            //SecondCommentIndex = value.ToLower() == "true" ? 17 : 16;
            //AllocationPersonIndex = -1;
            //UseSituationIndex = -1;
            //YieldIndex = -1;
            //OutputValueIndex = -1;
            //IncomeSituationIndex = -1;
            //LaborNumberIndex = -1;
            //IsSourceContractorIndex = -1;
            //FarmerNatureIndex = -1;
            //ContractorNumberIndex = -1;
            //MoveFormerlyLandTypeIndex = -1;
            //MoveFormerlyLandAreaIndex = -1;
            //FirstContractorPersonNumberIndex = -1;
            //FirstContractAreaIndex = -1;
            //SecondContractorPersonNumberIndex = -1;
            //SecondExtensionPackAreaIndex = -1;
            //FoodCropAreaIndex = -1;
            //AgeIndex = -1;
            //AccountNatureIndex = -1;
            //SourceMoveIndex = -1;
            //MoveTimeIndex = -1;
            //IsNinetyNineSharePersonIndex = -1;
            //SecondAgeIndex = -1;
            //SecondNationIndex = -1;
            //NationIndex = -1;
            //SecondLandNumberIndex = -1;
            //SecondArableTypeIndex = -1;
            //SecondIsFarmerLandIndex = -1;
            //SecondLandPurposeIndex = -1;
            //SecondLandLevelIndex = -1;
            throw new NotImplementedException();
        }

        /// <summary>
        /// 是否户籍信息
        /// </summary>
        /// <returns></returns>
        public bool InitalzieCencusTableValue()
        {
            if (ContractorTypeIndex != -1)
            {
                return true;
            }
            if (ContractorAddressIndex != -1)
            {
                return true;
            }
            if (FamilySurveyPersonIndex != -1)
            {
                return true;
            }
            if (FamilySurveyDateIndex != -1)
            {
                return true;
            }
            if (FamilySurveyChronicleIndex != -1)
            {
                return true;
            }
            if (FamilyCheckPersonIndex != -1)
            {
                return true;
            }
            if (FamilyCheckDateIndex != -1)
            {
                return true;
            }
            if (FamilyCheckOpinionIndex != -1)
            {
                return true;
            }
            if (AllocationPersonIndex != -1)
            {
                return true;
            }
            if (LaborNumberIndex != -1)
            {
                return true;
            }
            if (IsSourceContractorIndex != -1)
            {
                return true;
            }
            if (FarmerNatureIndex != -1)
            {
                return true;
            }
            if (ContractorNumberIndex != -1)
            {
                return true;
            }
            if (MoveFormerlyLandTypeIndex != -1)
            {
                return true;
            }
            if (MoveFormerlyLandAreaIndex != -1)
            {
                return true;
            }
            if (AccountNatureIndex != -1)
            {
                return true;
            }
            if (SourceMoveIndex != -1)
            {
                return true;
            }
            if (MoveTimeIndex != -1)
            {
                return true;
            }
            if (IsNinetyNineSharePersonIndex != -1)
            {
                return true;
            }
            if (CencueCommentIndex != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否包含二轮承包方
        /// </summary>
        /// <returns></returns>
        public bool InitalizeTableFamilyValue()
        {
            if (FirstContractorPersonNumberIndex != -1)
            {
                return true;
            }
            if (FirstContractAreaIndex != -1)
            {
                return true;
            }
            if (SecondContractorPersonNumberIndex != -1)
            {
                return true;
            }
            if (SecondExtensionPackAreaIndex != -1)
            {
                return true;
            }
            if (FoodCropAreaIndex != -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractBusinessImportSurveyDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractBusinessImportSurveyDefine>();
            var section = profile.GetSection<ContractBusinessImportSurveyDefine>();
            return section.Settings;
        }

        #endregion Methods
    }
}