/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.FuSui
{
    /// <summary>
    /// 公示确认表设置实体
    /// </summary>
    public class PublicityConfirmDefineFuSui : NotifyCDObject
    {
        #region Propertys

        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NameValue
        {
            get { return nameValue; }
            set { nameValue = value; NotifyPropertyChanged("NameValue"); }
        }

        private bool nameValue;

        /// <summary>
        ///承包方类型
        /// </summary>
        [DisplayLanguage("承包方类型", IsLanguageName = false)]
        [DescriptionLanguage("承包方类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractorTypeValue
        {
            get { return contractorTypeValue; }
            set { contractorTypeValue = value; NotifyPropertyChanged("ContractorTypeValue"); }
        }

        private bool contractorTypeValue;

        /// <summary>
        ///成员个数
        /// </summary>
        [DisplayLanguage("成员个数", IsLanguageName = false)]
        [DescriptionLanguage("成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberValue
        {
            get { return numberValue; }
            set { numberValue = value; NotifyPropertyChanged("NumberValue"); }
        }

        private bool numberValue;

        /// <summary>
        ///成员姓名
        /// </summary>
        [DisplayLanguage("成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberNameValue
        {
            get { return numberNameValue; }
            set { numberNameValue = value; NotifyPropertyChanged("NumberNameValue"); }
        }

        private bool numberNameValue;

        /// <summary>
        ///成员性别
        /// </summary>
        [DisplayLanguage("成员性别", IsLanguageName = false)]
        [DescriptionLanguage("成员性别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberGenderValue
        {
            get { return numberGenderValue; }
            set { numberGenderValue = value; NotifyPropertyChanged("NumberGenderValue"); }
        }

        private bool numberGenderValue;

        /// <summary>
        ///成员年龄
        /// </summary>
        [DisplayLanguage("成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("成员年龄", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberAgeValue
        {
            get { return numberAgeValue; }
            set { numberAgeValue = value; NotifyPropertyChanged("NumberAgeValue"); }
        }

        private bool numberAgeValue;

        /// <summary>
        ///证件类型
        /// </summary>
        [DisplayLanguage("证件类型", IsLanguageName = false)]
        [DescriptionLanguage("证件类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberCartTypeValue
        {
            get { return numberCartTypeValue; }
            set { numberCartTypeValue = value; NotifyPropertyChanged("NumberCartTypeValue"); }
        }

        private bool numberCartTypeValue;

        /// <summary>
        ///证件号码
        /// </summary>
        [DisplayLanguage("证件号码", IsLanguageName = false)]
        [DescriptionLanguage("证件号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberIcnValue
        {
            get { return numberIcnValue; }
            set { numberIcnValue = value; NotifyPropertyChanged("NumberIcnValue"); }
        }

        private bool numberIcnValue;

        /// <summary>
        ///成员关系
        /// </summary>
        [DisplayLanguage("成员关系", IsLanguageName = false)]
        [DescriptionLanguage("成员关系", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NumberRelatioinValue
        {
            get { return numberRelatioinValue; }
            set { numberRelatioinValue = value; NotifyPropertyChanged("NumberRelatioinValue"); }
        }

        private bool numberRelatioinValue;

        /// <summary>
        ///是否共有人
        /// </summary>
        [DisplayLanguage("是否共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否共有人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsSharedLandValue
        {
            get { return isSharedLandValue; }
            set { isSharedLandValue = value; NotifyPropertyChanged("IsSharedLandValue"); }
        }

        private bool isSharedLandValue;

        /// <summary>
        ///成员备注
        /// </summary>
        [DisplayLanguage("成员备注", IsLanguageName = false)]
        [DescriptionLanguage("成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyCommentValue
        {
            get { return familyCommentValue; }
            set { familyCommentValue = value; NotifyPropertyChanged("FamilyCommentValue"); }
        }

        private bool familyCommentValue;

        /// <summary>
        /// 共有人修改意见
        /// </summary>
        [DisplayLanguage("共有人修改意见", IsLanguageName = false)]
        [DescriptionLanguage("共有人修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyOpinionValue
        {
            get { return familyOpinionValue; }
            set { familyOpinionValue = value; NotifyPropertyChanged("FamilyOpinionValue"); }
        }

        private bool familyOpinionValue;

        /// <summary>
        ///承包方地址
        /// </summary>
        [DisplayLanguage("承包方地址", IsLanguageName = false)]
        [DescriptionLanguage("承包方地址", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractorAddressValue
        {
            get { return contractorAddressValue; }
            set { contractorAddressValue = value; NotifyPropertyChanged("ContractorAddressValue"); }
        }

        private bool contractorAddressValue;

        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandNameValue
        {
            get { return landNameValue; }
            set { landNameValue = value; NotifyPropertyChanged("LandNameValue"); }
        }

        private bool landNameValue;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CadastralNumberValue
        {
            get { return cadastralNumberValue; }
            set { cadastralNumberValue = value; NotifyPropertyChanged("CadastralNumberValue"); }
        }

        private bool cadastralNumberValue;

        /// <summary>
        ///图幅编号
        /// </summary>
        [DisplayLanguage("图幅编号", IsLanguageName = false)]
        [DescriptionLanguage("图幅编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ImageNumberValue
        {
            get { return imageNumberValue; }
            set { imageNumberValue = value; NotifyPropertyChanged("ImageNumberValue"); }
        }

        private bool imageNumberValue;

        /// <summary>
        ///二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TableAreaValue
        {
            get { return tableAreaValue; }
            set { tableAreaValue = value; NotifyPropertyChanged("TableAreaValue"); }
        }

        private bool tableAreaValue;

        /// <summary>
        ///二轮合同总面积
        /// </summary>
        [DisplayLanguage("二轮合同总面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TotalTableAreaValue
        {
            get { return totalTableAreaValue; }
            set { totalTableAreaValue = value; NotifyPropertyChanged("TotalTableAreaValue"); }
        }

        private bool totalTableAreaValue;

        /// <summary>
        ///实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ActualAreaValue
        {
            get { return actualAreaValue; }
            set { actualAreaValue = value; NotifyPropertyChanged("ActualAreaValue"); }
        }

        private bool actualAreaValue;

        /// <summary>
        ///实测总面积
        /// </summary>
        [DisplayLanguage("实测总面积", IsLanguageName = false)]
        [DescriptionLanguage("实测总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TotalActualAreaValue
        {
            get { return totalActualAreaValue; }
            set { totalActualAreaValue = value; NotifyPropertyChanged("TotalActualAreaValue"); }
        }

        private bool totalActualAreaValue;

        /// <summary>
        ///确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AwareAreaValue
        {
            get { return awareAreaValue; }
            set { awareAreaValue = value; NotifyPropertyChanged("AwareAreaValue"); }
        }

        private bool awareAreaValue;

        /// <summary>
        ///确权总面积
        /// </summary>
        [DisplayLanguage("确权总面积", IsLanguageName = false)]
        [DescriptionLanguage("确权总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TotalAwareAreaValue
        {
            get { return totalAwareAreaValue; }
            set { totalAwareAreaValue = value; NotifyPropertyChanged("TotalAwareAreaValue"); }
        }

        private bool totalAwareAreaValue;

        /// <summary>
        ///机动地面积
        /// </summary>
        [DisplayLanguage("机动地面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool MotorizeAreaValue
        {
            get { return motorizeAreaValue; }
            set { motorizeAreaValue = value; NotifyPropertyChanged("MotorizeAreaValue"); }
        }

        private bool motorizeAreaValue;

        /// <summary>
        ///机动地总面积
        /// </summary>
        [DisplayLanguage("机动地总面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TotalMotorizeAreaValue
        {
            get { return totalMotorizeAreaValue; }
            set { totalMotorizeAreaValue = value; NotifyPropertyChanged("TotalMotorizeAreaValue"); }
        }

        private bool totalMotorizeAreaValue;

        /// <summary>
        ///延包面积
        /// </summary>
        [DisplayLanguage("延包面积", IsLanguageName = false)]
        [DescriptionLanguage("延包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractDelayAreaValue
        {
            get { return contractDelayAreaValue; }
            set { contractDelayAreaValue = value; NotifyPropertyChanged("ContractDelayAreaValue"); }
        }

        private bool contractDelayAreaValue;

        /// <summary>
        ///延包总面积
        /// </summary>
        [DisplayLanguage("延包总面积", IsLanguageName = false)]
        [DescriptionLanguage("延包总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TotalContractDelayAreaValue
        {
            get { return totalContractDelayAreaValue; }
            set { totalContractDelayAreaValue = value; NotifyPropertyChanged("TotalContractDelayAreaValue"); }
        }

        private bool totalContractDelayAreaValue;

        /// <summary>
        ///四至
        /// </summary>
        [DisplayLanguage("四至", IsLanguageName = false)]
        [DescriptionLanguage("四至", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandNeighborValue
        {
            get { return landNeighborValue; }
            set { landNeighborValue = value; NotifyPropertyChanged("LandNeighborValue"); }
        }

        private bool landNeighborValue;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandPurposeValue
        {
            get { return landPurposeValue; }
            set { landPurposeValue = value; NotifyPropertyChanged("LandPurposeValue"); }
        }

        private bool landPurposeValue;

        /// <summary>
        ///地力等级
        /// </summary>
        [DisplayLanguage("地力等级", IsLanguageName = false)]
        [DescriptionLanguage("地力等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandLevelValue
        {
            get { return landLevelValue; }
            set { landLevelValue = value; NotifyPropertyChanged("LandLevelValue"); }
        }

        private bool landLevelValue;

        /// <summary>
        ///土地利用类型
        /// </summary>
        [DisplayLanguage("土地利用类型", IsLanguageName = false)]
        [DescriptionLanguage("土地利用类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandTypeValue
        {
            get { return landTypeValue; }
            set { landTypeValue = value; NotifyPropertyChanged("LandTypeValue"); }
        }

        private bool landTypeValue;

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DisplayLanguage("是否基本农田", IsLanguageName = false)]
        [DescriptionLanguage("是否基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsFarmerLandValue
        {
            get { return isFarmerLandValue; }
            set { isFarmerLandValue = value; NotifyPropertyChanged("IsFarmerLandValue"); }
        }

        private bool isFarmerLandValue;

        /// <summary>
        ///指界人
        /// </summary>
        [DisplayLanguage("指界人", IsLanguageName = false)]
        [DescriptionLanguage("指界人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ReferPersonValue
        {
            get { return referPersonValue; }
            set { referPersonValue = value; NotifyPropertyChanged("ReferPersonValue"); }
        }

        private bool referPersonValue;

        /// <summary>
        ///地块类别
        /// </summary>
        [DisplayLanguage("地块类别", IsLanguageName = false)]
        [DescriptionLanguage("地块类别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ArableTypeValue
        {
            get { return arableTypeValue; }
            set { arableTypeValue = value; NotifyPropertyChanged("ArableTypeValue"); }
        }

        private bool arableTypeValue;

        /// <summary>
        ///承包方式
        /// </summary>
        [DisplayLanguage("承包方式", IsLanguageName = false)]
        [DescriptionLanguage("承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConstructModeValue
        {
            get { return constructModeValue; }
            set { constructModeValue = value; NotifyPropertyChanged("ConstructModeValue"); }
        }

        private bool constructModeValue;

        /// <summary>
        ///畦数
        /// </summary>
        [DisplayLanguage("畦数", IsLanguageName = false)]
        [DescriptionLanguage("畦数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PlotNumberValue
        {
            get { return plotNumberValue; }
            set { plotNumberValue = value; NotifyPropertyChanged("PlotNumberValue"); }
        }

        private bool plotNumberValue;

        /// <summary>
        ///是否流转
        /// </summary>
        [DisplayLanguage("是否流转", IsLanguageName = false)]
        [DescriptionLanguage("是否流转", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsTransterValue
        {
            get { return isTransterValue; }
            set { isTransterValue = value; NotifyPropertyChanged("IsTransterValue"); }
        }

        private bool isTransterValue;

        /// <summary>
        ///流转方式
        /// </summary>
        [DisplayLanguage("流转方式", IsLanguageName = false)]
        [DescriptionLanguage("流转方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterModeValue
        {
            get { return transterModeValue; }
            set { transterModeValue = value; NotifyPropertyChanged("TransterModeValue"); }
        }

        private bool transterModeValue;

        /// <summary>
        ///流转期限
        /// </summary>
        [DisplayLanguage("流转期限", IsLanguageName = false)]
        [DescriptionLanguage("流转期限", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterTermValue
        {
            get { return transterTermValue; }
            set { transterTermValue = value; NotifyPropertyChanged("TransterTermValue"); }
        }

        private bool transterTermValue;

        /// <summary>
        ///流转面积
        /// </summary>
        [DisplayLanguage("流转面积", IsLanguageName = false)]
        [DescriptionLanguage("流转面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterAreaValue
        {
            get { return transterAreaValue; }
            set { transterAreaValue = value; NotifyPropertyChanged("TransterTermValue"); }
        }

        private bool transterAreaValue;

        /// <summary>
        ///种植类型
        /// </summary>
        [DisplayLanguage("种植类型", IsLanguageName = false)]
        [DescriptionLanguage("种植类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PlatTypeValue
        {
            get { return platTypeValue; }
            set { platTypeValue = value; NotifyPropertyChanged("PlatTypeValue"); }
        }

        private bool platTypeValue;

        /// <summary>
        ///经营方式
        /// </summary>
        [DisplayLanguage("经营方式", IsLanguageName = false)]
        [DescriptionLanguage("经营方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ManagementTypeValue
        {
            get { return managementTypeValue; }
            set { managementTypeValue = value; NotifyPropertyChanged("ManagementTypeValue"); }
        }

        private bool managementTypeValue;

        /// <summary>
        ///耕保类型
        /// </summary>
        [DisplayLanguage("耕保类型", IsLanguageName = false)]
        [DescriptionLanguage("耕保类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandPlantValue
        {
            get { return landPlantValue; }
            set { landPlantValue = value; NotifyPropertyChanged("LandPlantValue"); }
        }

        private bool landPlantValue;

        /// <summary>
        ///原户主姓名
        /// </summary>
        [DisplayLanguage("原户主姓名", IsLanguageName = false)]
        [DescriptionLanguage("原户主姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SourceNameValue
        {
            get { return sourceNameValue; }
            set { sourceNameValue = value; NotifyPropertyChanged("SourceNameValue"); }
        }

        private bool sourceNameValue;

        /// <summary>
        ///座落方位
        /// </summary>
        [DisplayLanguage("座落方位", IsLanguageName = false)]
        [DescriptionLanguage("座落方位", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandLocationValue
        {
            get { return landLocationValue; }
            set { landLocationValue = value; NotifyPropertyChanged("LandLocationValue"); }
        }

        private bool landLocationValue;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConcordValue
        {
            get { return concordValue; }
            set { concordValue = value; NotifyPropertyChanged("ConcordValue"); }
        }

        private bool concordValue;

        /// <summary>
        ///权证编号
        /// </summary>
        [DisplayLanguage("权证编号", IsLanguageName = false)]
        [DescriptionLanguage("权证编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool RegeditBookValue
        {
            get { return regeditBookValue; }
            set { regeditBookValue = value; NotifyPropertyChanged("RegeditBookValue"); }
        }

        private bool regeditBookValue;

        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CommentValue
        {
            get { return commentValue; }
            set { commentValue = value; NotifyPropertyChanged("CommentValue"); }
        }

        private bool commentValue;

        /// <summary>
        /// 地块信息修改意见
        /// </summary>
        [DisplayLanguage("地块信息修改意见", IsLanguageName = false)]
        [DescriptionLanguage("地块信息修改意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方地块信息", Gallery = "调查信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool OpinionValue
        {
            get { return opinionValue; }
            set { opinionValue = value; NotifyPropertyChanged("OpinionValue"); }
        }

        private bool opinionValue;

        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNameValue
        {
            get { return secondNameValue; }
            set { secondNameValue = value; NotifyPropertyChanged("SecondNameValue"); }
        }

        private bool secondNameValue;

        /// <summary>
        ///家庭成员个数
        /// </summary>
        [DisplayLanguage("家庭成员个数", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员个数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberValue
        {
            get { return secondNumberValue; }
            set { secondNumberValue = value; NotifyPropertyChanged("SecondNumberValue"); }
        }

        private bool secondNumberValue;

        /// <summary>
        ///家庭成员姓名
        /// </summary>
        [DisplayLanguage("家庭成员姓名", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberNameValue
        {
            get { return secondNumberNameValue; }
            set { secondNumberNameValue = value; NotifyPropertyChanged("SecondNumberNameValue"); }
        }

        private bool secondNumberNameValue;

        /// <summary>
        ///家庭成员性别
        /// </summary>
        [DisplayLanguage("家庭成员性别", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员性别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberGenderValue
        {
            get { return secondNumberGenderValue; }
            set { secondNumberGenderValue = value; NotifyPropertyChanged("SecondNumberGenderValue"); }
        }

        private bool secondNumberGenderValue;

        /// <summary>
        ///家庭成员年龄
        /// </summary>
        [DisplayLanguage("家庭成员年龄", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员年龄", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberAgeValue
        {
            get { return secondNumberAgeValue; }
            set { secondNumberAgeValue = value; NotifyPropertyChanged("SecondNumberAgeValue"); }
        }

        private bool secondNumberAgeValue;

        /// <summary>
        ///成员身份证号
        /// </summary>
        [DisplayLanguage("成员身份证号", IsLanguageName = false)]
        [DescriptionLanguage("成员身份证号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberIcnValue
        {
            get { return secondNumberIcnValue; }
            set { secondNumberIcnValue = value; NotifyPropertyChanged("SecondNumberIcnValue"); }
        }

        private bool secondNumberIcnValue;

        /// <summary>
        ///家庭成员关系
        /// </summary>
        [DisplayLanguage("家庭成员关系", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员关系", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNumberRelatioinValue
        {
            get { return secondNumberRelatioinValue; }
            set { secondNumberRelatioinValue = value; NotifyPropertyChanged("SecondNumberRelatioinValue"); }
        }

        private bool secondNumberRelatioinValue;

        /// <summary>
        ///家庭成员备注
        /// </summary>
        [DisplayLanguage("家庭成员备注", IsLanguageName = false)]
        [DescriptionLanguage("家庭成员备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondFamilyCommentValue
        {
            get { return secondFamilyCommentValue; }
            set { secondFamilyCommentValue = value; NotifyPropertyChanged("SecondFamilyCommentValue"); }
        }

        private bool secondFamilyCommentValue;

        /// <summary>
        ///二轮延包姓名
        /// </summary>
        [DisplayLanguage("二轮延包姓名", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ExPackageNameValue
        {
            get { return exPackageNameValue; }
            set { exPackageNameValue = value; NotifyPropertyChanged("ExPackageNameValue"); }
        }

        private bool exPackageNameValue;

        /// <summary>
        ///延包土地份数
        /// </summary>
        [DisplayLanguage("延包土地份数", IsLanguageName = false)]
        [DescriptionLanguage("延包土地份数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ExPackageNumberValue
        {
            get { return exPackageNumberValue; }
            set { exPackageNumberValue = value; NotifyPropertyChanged("ExPackageNumberValue"); }
        }

        private bool exPackageNumberValue;

        /// <summary>
        ///已死亡人员
        /// </summary>
        [DisplayLanguage("已死亡人员", IsLanguageName = false)]
        [DescriptionLanguage("已死亡人员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsDeadedValue
        {
            get { return isDeadedValue; }
            set { isDeadedValue = value; NotifyPropertyChanged("IsDeadedValue"); }
        }

        private bool isDeadedValue;

        /// <summary>
        ///出嫁后未退承包地人员
        /// </summary>
        [DisplayLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [DescriptionLanguage("出嫁后未退承包地人员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LocalMarriedRetreatLandValue
        {
            get { return localMarriedRetreatLandValue; }
            set { localMarriedRetreatLandValue = value; NotifyPropertyChanged("LocalMarriedRetreatLandValue"); }
        }

        private bool localMarriedRetreatLandValue;

        /// <summary>
        ///农转非未退承包地
        /// </summary>
        [DisplayLanguage("农转非未退承包地", IsLanguageName = false)]
        [DescriptionLanguage("农转非未退承包地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PeasantsRetreatLandValue
        {
            get { return peasantsRetreatLandValue; }
            set { peasantsRetreatLandValue = value; NotifyPropertyChanged("PeasantsRetreatLandValue"); }
        }

        private bool peasantsRetreatLandValue;

        /// <summary>
        ///婚进在婚出地未退承包地
        /// </summary>
        [DisplayLanguage("婚进在婚出地未退承包地", IsLanguageName = false)]
        [DescriptionLanguage("婚进在婚出地未退承包地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ForeignMarriedRetreatLandValue
        {
            get { return foreignMarriedRetreatLandValue; }
            set { foreignMarriedRetreatLandValue = value; NotifyPropertyChanged("ForeignMarriedRetreatLandValue"); }
        }

        private bool foreignMarriedRetreatLandValue;

        /// <summary>
        ///民族
        /// </summary>
        [DisplayLanguage("民族", IsLanguageName = false)]
        [DescriptionLanguage("民族", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondNationValue
        {
            get { return secondNationValue; }
            set { secondNationValue = value; NotifyPropertyChanged("SecondNationValue"); }
        }

        private bool secondNationValue;

        /// <summary>
        ///出生日期
        /// </summary>
        [DisplayLanguage("出生日期", IsLanguageName = false)]
        [DescriptionLanguage("出生日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondAgeValue
        {
            get { return secondAgeValue; }
            set { secondAgeValue = value; NotifyPropertyChanged("SecondAgeValue"); }
        }

        private bool secondAgeValue;

        /// <summary>
        ///一轮承包人数
        /// </summary>
        [DisplayLanguage("一轮承包人数", IsLanguageName = false)]
        [DescriptionLanguage("一轮承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FirstContractorPersonNumberValue
        {
            get { return firstContractorPersonNumberValue; }
            set { firstContractorPersonNumberValue = value; NotifyPropertyChanged("FirstContractorPersonNumberValue"); }
        }

        private bool firstContractorPersonNumberValue;

        /// <summary>
        ///一轮承包面积
        /// </summary>
        [DisplayLanguage("一轮承包面积", IsLanguageName = false)]
        [DescriptionLanguage("一轮承包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FirstContractAreaValue
        {
            get { return firstContractAreaValue; }
            set { firstContractAreaValue = value; NotifyPropertyChanged("FirstContractAreaValue"); }
        }

        private bool firstContractAreaValue;

        /// <summary>
        ///二轮承包人数
        /// </summary>
        [DisplayLanguage("二轮承包人数", IsLanguageName = false)]
        [DescriptionLanguage("二轮承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondContractorPersonNumberValue
        {
            get { return secondContractorPersonNumberValue; }
            set { secondContractorPersonNumberValue = value; NotifyPropertyChanged("SecondContractorPersonNumberValue"); }
        }

        private bool secondContractorPersonNumberValue;

        /// <summary>
        ///二轮延包面积
        /// </summary>
        [DisplayLanguage("二轮延包面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮延包面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondExtensionPackAreaValue
        {
            get { return secondExtensionPackAreaValue; }
            set { secondExtensionPackAreaValue = value; NotifyPropertyChanged("SecondExtensionPackAreaValue"); }
        }

        private bool secondExtensionPackAreaValue;

        /// <summary>
        ///粮食种植面积
        /// </summary>
        [DisplayLanguage("粮食种植面积", IsLanguageName = false)]
        [DescriptionLanguage("粮食种植面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮承包家庭及成员信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FoodCropAreaValue
        {
            get { return foodCropAreaValue; }
            set { foodCropAreaValue = value; NotifyPropertyChanged("FoodCropAreaValue"); }
        }

        private bool foodCropAreaValue;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandNumberValue
        {
            get { return secondLandNumberValue; }
            set { secondLandNumberValue = value; NotifyPropertyChanged("SecondLandNumberValue"); }
        }

        private bool secondLandNumberValue;

        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandNameValue
        {
            get { return secondLandNameValue; }
            set { secondLandNameValue = value; NotifyPropertyChanged("SecondLandNameValue"); }
        }

        private bool secondLandNameValue;

        /// <summary>
        ///地类
        /// </summary>
        [DisplayLanguage("地类", IsLanguageName = false)]
        [DescriptionLanguage("地类", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandTypeValue
        {
            get { return secondLandTypeValue; }
            set { secondLandTypeValue = value; NotifyPropertyChanged("SecondLandTypeValue"); }
        }

        private bool secondLandTypeValue;

        /// <summary>
        ///台账面积
        /// </summary>
        [DisplayLanguage("台账面积", IsLanguageName = false)]
        [DescriptionLanguage("台账面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondTableAreaValue
        {
            get { return secondTableAreaValue; }
            set { secondTableAreaValue = value; NotifyPropertyChanged("SecondTableAreaValue"); }
        }

        private bool secondTableAreaValue;

        /// <summary>
        ///台账总面积
        /// </summary>
        [DisplayLanguage("台账总面积", IsLanguageName = false)]
        [DescriptionLanguage("台账总面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondTotalTableAreaValue
        {
            get { return secondTotalTableAreaValue; }
            set { secondTotalTableAreaValue = value; NotifyPropertyChanged("SecondTotalTableAreaValue"); }
        }

        private bool secondTotalTableAreaValue;

        /// <summary>
        ///四至
        /// </summary>
        [DisplayLanguage("四至", IsLanguageName = false)]
        [DescriptionLanguage("四至", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandNeighborValue
        {
            get { return secondLandNeighborValue; }
            set { secondLandNeighborValue = value; NotifyPropertyChanged("SecondLandNeighborValue"); }
        }

        private bool secondLandNeighborValue;

        /// <summary>
        ///土地类型
        /// </summary>
        [DisplayLanguage("土地类型", IsLanguageName = false)]
        [DescriptionLanguage("土地类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondArableTypeValue
        {
            get { return secondArableTypeValue; }
            set { secondArableTypeValue = value; NotifyPropertyChanged("SecondArableTypeValue"); }
        }

        private bool secondArableTypeValue;

        /// <summary>
        ///基本农田
        /// </summary>
        [DisplayLanguage("基本农田", IsLanguageName = false)]
        [DescriptionLanguage("基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondIsFarmerLandValue
        {
            get { return secondIsFarmerLandValue; }
            set { secondIsFarmerLandValue = value; NotifyPropertyChanged("SecondIsFarmerLandValue"); }
        }

        private bool secondIsFarmerLandValue;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandPurposeValue
        {
            get { return secondLandPurposeValue; }
            set { secondLandPurposeValue = value; NotifyPropertyChanged("SecondLandPurposeValue"); }
        }

        private bool secondLandPurposeValue;

        /// <summary>
        ///地力等级
        /// </summary>
        [DisplayLanguage("地力等级", IsLanguageName = false)]
        [DescriptionLanguage("地力等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondLandLevelValue
        {
            get { return secondLandLevelValue; }
            set { secondLandLevelValue = value; NotifyPropertyChanged("SecondLandLevelValue"); }
        }

        private bool secondLandLevelValue;

        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "二轮地块信息", Gallery = "二轮信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondCommentValue
        {
            get { return secondCommentValue; }
            set { secondCommentValue = value; NotifyPropertyChanged("SecondCommentValue"); }
        }

        private bool secondCommentValue;

        /// <summary>
        ///出生日期
        /// </summary>
        [DisplayLanguage("出生日期", IsLanguageName = false)]
        [DescriptionLanguage("出生日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AgeValue
        {
            get { return ageValue; }
            set { ageValue = value; NotifyPropertyChanged("AgeValue"); }
        }

        private bool ageValue;

        /// <summary>
        ///民族
        /// </summary>
        [DisplayLanguage("民族", IsLanguageName = false)]
        [DescriptionLanguage("民族", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool NationValue
        {
            get { return nationValue; }
            set { nationValue = value; NotifyPropertyChanged("NationValue"); }
        }

        private bool nationValue;

        /// <summary>
        ///户口性质
        /// </summary>
        [DisplayLanguage("户口性质", IsLanguageName = false)]
        [DescriptionLanguage("户口性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AccountNatureValue
        {
            get { return accountNatureValue; }
            set { accountNatureValue = value; NotifyPropertyChanged("AccountNatureValue"); }
        }

        private bool accountNatureValue;

        /// <summary>
        ///是否原承包户
        /// </summary>
        [DisplayLanguage("是否原承包户", IsLanguageName = false)]
        [DescriptionLanguage("是否原承包户", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsSourceContractorValue
        {
            get { return isSourceContractorValue; }
            set { isSourceContractorValue = value; NotifyPropertyChanged("IsSourceContractorValue"); }
        }

        private bool isSourceContractorValue;

        /// <summary>
        ///现承包人数
        /// </summary>
        [DisplayLanguage("现承包人数", IsLanguageName = false)]
        [DescriptionLanguage("现承包人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ContractorNumberValue
        {
            get { return contractorNumberValue; }
            set { contractorNumberValue = value; NotifyPropertyChanged("ContractorNumberValue"); }
        }

        private bool contractorNumberValue;

        /// <summary>
        ///总劳力数
        /// </summary>
        [DisplayLanguage("总劳力数", IsLanguageName = false)]
        [DescriptionLanguage("总劳力数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LaborNumberValue
        {
            get { return laborNumberValue; }
            set { laborNumberValue = value; NotifyPropertyChanged("LaborNumberValue"); }
        }

        private bool laborNumberValue;

        /// <summary>
        ///户籍备注
        /// </summary>
        [DisplayLanguage("户籍备注", IsLanguageName = false)]
        [DescriptionLanguage("户籍备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool CencueCommentValue
        {
            get { return cencueCommentValue; }
            set { cencueCommentValue = value; NotifyPropertyChanged("CencueCommentValue"); }
        }

        private bool cencueCommentValue;

        /// <summary>
        ///农户性质
        /// </summary>
        [DisplayLanguage("农户性质", IsLanguageName = false)]
        [DescriptionLanguage("农户性质", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FarmerNatureValue
        {
            get { return farmerNatureValue; }
            set { farmerNatureValue = value; NotifyPropertyChanged("FarmerNatureValue"); }
        }

        private bool farmerNatureValue;

        /// <summary>
        ///从何处迁入
        /// </summary>
        [DisplayLanguage("从何处迁入", IsLanguageName = false)]
        [DescriptionLanguage("从何处迁入", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SourceMoveValue
        {
            get { return sourceMoveValue; }
            set { sourceMoveValue = value; NotifyPropertyChanged("SourceMoveValue"); }
        }

        private bool sourceMoveValue;

        /// <summary>
        ///迁入时间
        /// </summary>
        [DisplayLanguage("迁入时间", IsLanguageName = false)]
        [DescriptionLanguage("迁入时间", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool MoveTimeValue
        {
            get { return moveTimeValue; }
            set { moveTimeValue = value; NotifyPropertyChanged("MoveTimeValue"); }
        }

        private bool moveTimeValue;

        /// <summary>
        ///迁入前土地类型
        /// </summary>
        [DisplayLanguage("迁入前土地类型", IsLanguageName = false)]
        [DescriptionLanguage("迁入前土地类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool MoveFormerlyLandTypeValue
        {
            get { return moveFormerlyLandTypeValue; }
            set { moveFormerlyLandTypeValue = value; NotifyPropertyChanged("MoveFormerlyLandTypeValue"); }
        }

        private bool moveFormerlyLandTypeValue;

        /// <summary>
        ///实际分配人数
        /// </summary>
        [DisplayLanguage("实际分配人数", IsLanguageName = false)]
        [DescriptionLanguage("实际分配人数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool AllocationPersonValue
        {
            get { return allocationPersonValue; }
            set { allocationPersonValue = value; NotifyPropertyChanged("AllocationPersonValue"); }
        }

        private bool allocationPersonValue;

        /// <summary>
        ///迁入前土地面积
        /// </summary>
        [DisplayLanguage("迁入前土地面积", IsLanguageName = false)]
        [DescriptionLanguage("迁入前土地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool MoveFormerlyLandAreaValue
        {
            get { return moveFormerlyLandAreaValue; }
            set { moveFormerlyLandAreaValue = value; NotifyPropertyChanged("MoveFormerlyLandAreaValue"); }
        }

        private bool moveFormerlyLandAreaValue;

        /// <summary>
        ///是否为99年共有人
        /// </summary>
        [DisplayLanguage("是否为99年共有人", IsLanguageName = false)]
        [DescriptionLanguage("是否为99年共有人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsNinetyNineSharePersonValue
        {
            get { return isNinetyNineSharePersonValue; }
            set { isNinetyNineSharePersonValue = value; NotifyPropertyChanged("IsNinetyNineSharePersonValue"); }
        }

        private bool isNinetyNineSharePersonValue;

        /// <summary>
        ///邮政编码
        /// </summary>
        [DisplayLanguage("邮政编码", IsLanguageName = false)]
        [DescriptionLanguage("邮政编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool PostNumberValue
        {
            get { return postNumberValue; }
            set { postNumberValue = value; NotifyPropertyChanged("PostNumberValue"); }
        }

        private bool postNumberValue;

        /// <summary>
        ///联系电话
        /// </summary>
        [DisplayLanguage("联系电话", IsLanguageName = false)]
        [DescriptionLanguage("联系电话", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool TelephoneValue
        {
            get { return telephoneValue; }
            set { telephoneValue = value; NotifyPropertyChanged("TelephoneValue"); }
        }

        private bool telephoneValue;

        /// <summary>
        ///合同编号
        /// </summary>
        [DisplayLanguage("合同编号", IsLanguageName = false)]
        [DescriptionLanguage("合同编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondConcordNumberValue
        {
            get { return secondConcordNumberValue; }
            set { secondConcordNumberValue = value; NotifyPropertyChanged("SecondConcordNumberValue"); }
        }

        private bool secondConcordNumberValue;

        /// <summary>
        ///权证编号
        /// </summary>
        [DisplayLanguage("权证编号", IsLanguageName = false)]
        [DescriptionLanguage("权证编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool SecondWarrantNumberValue
        {
            get { return secondWarrantNumberValue; }
            set { secondWarrantNumberValue = value; NotifyPropertyChanged("SecondWarrantNumberValue"); }
        }

        private bool secondWarrantNumberValue;

        /// <summary>
        ///起始日期
        /// </summary>
        [DisplayLanguage("起始日期", IsLanguageName = false)]
        [DescriptionLanguage("起始日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool StartTimeValue
        {
            get { return startTimeValue; }
            set { startTimeValue = value; NotifyPropertyChanged("StartTimeValue"); }
        }

        private bool startTimeValue;

        /// <summary>
        ///结束日期
        /// </summary>
        [DisplayLanguage("结束日期", IsLanguageName = false)]
        [DescriptionLanguage("结束日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool EndTimeValue
        {
            get { return endTimeValue; }
            set { endTimeValue = value; NotifyPropertyChanged("EndTimeValue"); }
        }

        private bool endTimeValue;

        /// <summary>
        ///取得承包方式
        /// </summary>
        [DisplayLanguage("取得承包方式", IsLanguageName = false)]
        [DescriptionLanguage("取得承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConstructTypeValue
        {
            get { return constructTypeValue; }
            set { constructTypeValue = value; NotifyPropertyChanged("ConstructTypeValue"); }
        }

        private bool constructTypeValue;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilySurveyPersonValue
        {
            get { return familySurveyPersonValue; }
            set { familySurveyPersonValue = value; NotifyPropertyChanged("FamilySurveyPersonValue"); }
        }

        private bool familySurveyPersonValue;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilySurveyDateValue
        {
            get { return familySurveyDateValue; }
            set { familySurveyDateValue = value; NotifyPropertyChanged("FamilySurveyDateValue"); }
        }

        private bool familySurveyDateValue;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilySurveyChronicleValue
        {
            get { return familySurveyChronicleValue; }
            set { familySurveyChronicleValue = value; NotifyPropertyChanged("FamilySurveyChronicleValue"); }
        }

        private bool familySurveyChronicleValue;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyCheckPersonValue
        {
            get { return familyCheckPersonValue; }
            set { familyCheckPersonValue = value; NotifyPropertyChanged("FamilyCheckPersonValue"); }
        }

        private bool familyCheckPersonValue;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyCheckDateValue
        {
            get { return familyCheckDateValue; }
            set { familyCheckDateValue = value; NotifyPropertyChanged("FamilyCheckDateValue"); }
        }

        private bool familyCheckDateValue;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "承包方及家庭成员扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyCheckOpinionValue
        {
            get { return familyCheckOpinionValue; }
            set { familyCheckOpinionValue = value; NotifyPropertyChanged("FamilyCheckOpinionValue"); }
        }

        private bool familyCheckOpinionValue;

        /// <summary>
        ///利用情况
        /// </summary>
        [DisplayLanguage("利用情况", IsLanguageName = false)]
        [DescriptionLanguage("利用情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool UseSituationValue
        {
            get { return useSituationValue; }
            set { useSituationValue = value; NotifyPropertyChanged("UseSituationValue"); }
        }

        private bool useSituationValue;

        /// <summary>
        ///产量情况
        /// </summary>
        [DisplayLanguage("产量情况", IsLanguageName = false)]
        [DescriptionLanguage("产量情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool YieldValue
        {
            get { return yieldValue; }
            set { yieldValue = value; NotifyPropertyChanged("YieldValue"); }
        }

        private bool yieldValue;

        /// <summary>
        ///产值情况
        /// </summary>
        [DisplayLanguage("产值情况", IsLanguageName = false)]
        [DescriptionLanguage("产值情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool OutputValueValue
        {
            get { return outputValueValue; }
            set { outputValueValue = value; NotifyPropertyChanged("OutputValueValue"); }
        }

        private bool outputValueValue;

        /// <summary>
        ///收益情况
        /// </summary>
        [DisplayLanguage("收益情况", IsLanguageName = false)]
        [DescriptionLanguage("收益情况", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool IncomeSituationValue
        {
            get { return incomeSituationValue; }
            set { incomeSituationValue = value; NotifyPropertyChanged("IncomeSituationValue"); }
        }

        private bool incomeSituationValue;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyPersonValue
        {
            get { return landSurveyPersonValue; }
            set { landSurveyPersonValue = value; NotifyPropertyChanged("LandSurveyPersonValue"); }
        }

        private bool landSurveyPersonValue;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyDateValue
        {
            get { return landSurveyDateValue; }
            set { landSurveyDateValue = value; NotifyPropertyChanged("LandSurveyDateValue"); }
        }

        private bool landSurveyDateValue;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyChronicleValue
        {
            get { return landSurveyChronicleValue; }
            set { landSurveyChronicleValue = value; NotifyPropertyChanged("LandSurveyChronicleValue"); }
        }

        private bool landSurveyChronicleValue;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckPersonValue
        {
            get { return landCheckPersonValue; }
            set { landCheckPersonValue = value; NotifyPropertyChanged("LandCheckPersonValue"); }
        }

        private bool landCheckPersonValue;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckDateValue
        {
            get { return landCheckDateValue; }
            set { landCheckDateValue = value; NotifyPropertyChanged("LandCheckDateValue"); }
        }

        private bool landCheckDateValue;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "宗地扩展信息", Gallery = "扩展信息",
            Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckOpinionValue
        {
            get { return landCheckOpinionValue; }
            set { landCheckOpinionValue = value; NotifyPropertyChanged("LandCheckOpinionValue"); }
        }

        private bool landCheckOpinionValue;

        /// <summary>
        /// 是否含有二轮承包方信息
        /// </summary>
        public bool IsContainTableValue
        {
            get { return InitalizeTableValue(); }
        }

        /// <summary>
        /// 是否含有二轮承包地块信息
        /// </summary>
        public bool IsContainTablelandValue
        {
            get { return InitalizeTableLandValue(); }
        }

        /// <summary>
        /// 是否含有户籍信息
        /// </summary>
        /// <returns></returns>
        public bool IsContainCensusValue
        {
            get { return InitalizeCensusValue(); }
        }

        /// <summary>
        /// 列数
        /// </summary>
        [DisplayLanguage("列数", IsLanguageName = false)]
        [Enabled(false)]
        public int ColumnCount
        {
            get { return columnCount; }
            set { columnCount = getColumnCount(); }
        }

        private int columnCount;

        #endregion Propertys

        #region Ctor

        public PublicityConfirmDefineFuSui()
        {
            nameValue = true;
            secondNameValue = false;
            contractorTypeValue = false;
            numberValue = true;
            numberNameValue = true;
            secondNumberNameValue = false;
            numberCartTypeValue = false;
            numberIcnValue = true;
            secondNumberIcnValue = false;
            numberGenderValue = false;
            secondNumberGenderValue = false;
            numberAgeValue = false;
            secondNumberAgeValue = false;
            numberRelatioinValue = true;
            secondNumberRelatioinValue = false;
            exPackageNameValue = false;
            exPackageNumberValue = false;
            isDeadedValue = false;
            localMarriedRetreatLandValue = false;
            peasantsRetreatLandValue = false;
            foreignMarriedRetreatLandValue = false;
            isSharedLandValue = false;
            concordValue = false;
            regeditBookValue = false;
            cadastralNumberValue = true;
            landNameValue = true;
            secondLandNameValue = false;
            plotNumberValue = false;
            actualAreaValue = true;
            totalActualAreaValue = true;
            awareAreaValue = true;
            totalAwareAreaValue = true;
            motorizeAreaValue = false;
            totalMotorizeAreaValue = false;
            tableAreaValue = false;
            ContractDelayAreaValue = false;
            TotalContractDelayAreaValue = false;
            secondTableAreaValue = false;
            totalTableAreaValue = false;
            secondTotalTableAreaValue = false;
            landTypeValue = false;
            secondLandTypeValue = false;
            managementTypeValue = false;
            landNeighborValue = true;
            sourceNameValue = false;
            landLocationValue = false;
            commentValue = true;
            OpinionValue = true;
            secondCommentValue = false;
            isFarmerLandValue = false;
            constructModeValue = false;
            isTransterValue = false;
            transterModeValue = false;
            transterTermValue = false;
            transterAreaValue = false;
            platTypeValue = false;
            telephoneValue = true;
            familyCommentValue = false;
            FamilyOpinionValue = true;
            secondFamilyCommentValue = false;
            landLevelValue = false;
            landPlantValue = false;
            arableTypeValue = true;
            allocationPersonValue = false;
            incomeSituationValue = false;
            laborNumberValue = false;
            isSourceContractorValue = false;
            contractorNumberValue = false;
            farmerNatureValue = false;
            moveFormerlyLandTypeValue = false;
            moveFormerlyLandAreaValue = false;
            firstContractorPersonNumberValue = false;
            firstContractAreaValue = false;
            secondContractorPersonNumberValue = false;
            secondExtensionPackAreaValue = false;
            foodCropAreaValue = false;
            ageValue = false;
            accountNatureValue = false;
            sourceMoveValue = false;
            moveTimeValue = false;
            isNinetyNineSharePersonValue = false;
            cencueCommentValue = false;
            familySurveyPersonValue = false;
            familySurveyDateValue = false;
            familySurveyChronicleValue = false;
            familyCheckPersonValue = false;
            familyCheckDateValue = false;
            landSurveyPersonValue = false;
            landSurveyDateValue = false;
            landSurveyChronicleValue = false;
            landCheckPersonValue = false;
            landCheckDateValue = false;
            landCheckOpinionValue = false;
            referPersonValue = false;
            contractorAddressValue = false;
            postNumberValue = false;
            landPurposeValue = false;
            imageNumberValue = false;
            secondConcordNumberValue = false;
            secondWarrantNumberValue = false;
            startTimeValue = false;
            endTimeValue = false;
            constructTypeValue = false;
            secondNumberValue = false;
            secondNationValue = false;

            secondAgeValue = false;
            secondLandNumberValue = false;
            secondLandNeighborValue = false;
            secondArableTypeValue = false;
            secondIsFarmerLandValue = false;
            secondLandPurposeValue = false;
            secondLandLevelValue = false;
            nationValue = false;
            useSituationValue = false;
            outputValueValue = false;
            familyCheckOpinionValue = false;
            yieldValue = false;
            ColumnCount = getColumnCount();
        }

        #endregion Ctor

        /// <summary>
        /// 根据配置初始化列表栏个数
        /// </summary>
        /// <returns></returns>
        public int getColumnCount()
        {
            int count = 1;
            count += nameValue ? 1 : 0;
            count += secondNameValue ? 1 : 0;
            count += contractorTypeValue ? 1 : 0;
            count += numberValue ? 1 : 0;
            count += numberNameValue ? 1 : 0;
            count += secondNumberNameValue ? 1 : 0;
            count += numberCartTypeValue ? 1 : 0;
            count += numberIcnValue ? 1 : 0;
            count += secondNumberIcnValue ? 1 : 0;
            count += numberGenderValue ? 1 : 0;
            count += secondNumberGenderValue ? 1 : 0;
            count += numberAgeValue ? 1 : 0;
            count += secondNumberAgeValue ? 1 : 0;
            count += numberRelatioinValue ? 1 : 0;
            count += secondNumberRelatioinValue ? 1 : 0;
            count += exPackageNameValue ? 1 : 0;
            count += exPackageNumberValue ? 1 : 0;
            count += isDeadedValue ? 1 : 0;
            count += localMarriedRetreatLandValue ? 1 : 0;
            count += peasantsRetreatLandValue ? 1 : 0;
            count += foreignMarriedRetreatLandValue ? 1 : 0;
            count += isSharedLandValue ? 1 : 0;
            count += concordValue ? 1 : 0;
            count += regeditBookValue ? 1 : 0;
            count += cadastralNumberValue ? 1 : 0;
            count += landNameValue ? 1 : 0;
            count += secondLandNameValue ? 1 : 0;
            count += plotNumberValue ? 1 : 0;
            count += actualAreaValue ? 1 : 0;
            count += totalActualAreaValue ? 1 : 0;
            count += awareAreaValue ? 1 : 0;
            count += totalAwareAreaValue ? 1 : 0;
            count += motorizeAreaValue ? 1 : 0;
            count += totalMotorizeAreaValue ? 1 : 0;
            count += tableAreaValue ? 1 : 0;
            count += contractDelayAreaValue ? 1 : 0;
            count += totalContractDelayAreaValue ? 1 : 0;
            count += secondTableAreaValue ? 1 : 0;
            count += totalTableAreaValue ? 1 : 0;
            count += secondTotalTableAreaValue ? 1 : 0;
            count += landTypeValue ? 1 : 0;
            count += secondLandTypeValue ? 1 : 0;
            count += managementTypeValue ? 1 : 0;
            count += landNeighborValue ? 4 : 0;
            count += sourceNameValue ? 1 : 0;
            count += landLocationValue ? 1 : 0;
            count += commentValue ? 1 : 0;
            count += opinionValue ? 1 : 0;
            count += secondCommentValue ? 1 : 0;
            count += isFarmerLandValue ? 1 : 0;
            count += constructModeValue ? 1 : 0;
            count += isTransterValue ? 1 : 0;
            count += transterModeValue ? 1 : 0;
            count += transterTermValue ? 1 : 0;
            count += transterAreaValue ? 1 : 0;
            count += platTypeValue ? 1 : 0;
            count += telephoneValue ? 1 : 0;
            count += familyCommentValue ? 1 : 0;
            count += familyOpinionValue ? 1 : 0;
            count += secondFamilyCommentValue ? 1 : 0;
            count += landLevelValue ? 1 : 0;
            count += landPlantValue ? 1 : 0;
            count += arableTypeValue ? 1 : 0;
            count += allocationPersonValue ? 1 : 0;
            count += incomeSituationValue ? 1 : 0;
            count += laborNumberValue ? 1 : 0;
            count += isSourceContractorValue ? 1 : 0;
            count += contractorNumberValue ? 1 : 0;
            count += farmerNatureValue ? 1 : 0;
            count += moveFormerlyLandTypeValue ? 1 : 0;
            count += moveFormerlyLandAreaValue ? 1 : 0;
            count += firstContractorPersonNumberValue ? 1 : 0;
            count += firstContractAreaValue ? 1 : 0;
            count += secondContractorPersonNumberValue ? 1 : 0;
            count += secondExtensionPackAreaValue ? 1 : 0;
            count += foodCropAreaValue ? 1 : 0;
            count += ageValue ? 1 : 0;
            count += accountNatureValue ? 1 : 0;
            count += sourceMoveValue ? 1 : 0;
            count += moveTimeValue ? 1 : 0;
            count += isNinetyNineSharePersonValue ? 1 : 0;
            count += cencueCommentValue ? 1 : 0;
            count += familySurveyPersonValue ? 1 : 0;
            count += familySurveyDateValue ? 1 : 0;
            count += familySurveyChronicleValue ? 1 : 0;
            count += familyCheckPersonValue ? 1 : 0;
            count += familyCheckDateValue ? 1 : 0;
            count += landSurveyPersonValue ? 1 : 0;
            count += landSurveyDateValue ? 1 : 0;
            count += landSurveyChronicleValue ? 1 : 0;
            count += landCheckPersonValue ? 1 : 0;
            count += landCheckDateValue ? 1 : 0;
            count += landCheckOpinionValue ? 1 : 0;
            count += referPersonValue ? 1 : 0;
            count += contractorAddressValue ? 1 : 0;
            count += postNumberValue ? 1 : 0;
            count += landPurposeValue ? 1 : 0;
            count += imageNumberValue ? 1 : 0;
            count += secondConcordNumberValue ? 1 : 0;
            count += secondWarrantNumberValue ? 1 : 0;
            count += startTimeValue ? 1 : 0;
            count += endTimeValue ? 1 : 0;
            count += constructTypeValue ? 1 : 0;
            count += secondNumberValue ? 1 : 0;
            count += secondNationValue ? 1 : 0;
            count += secondAgeValue ? 1 : 0;
            count += secondLandNumberValue ? 1 : 0;
            count += secondLandNeighborValue ? 1 : 0;
            count += secondArableTypeValue ? 1 : 0;
            count += secondIsFarmerLandValue ? 1 : 0;
            count += secondLandPurposeValue ? 1 : 0;
            count += secondLandLevelValue ? 1 : 0;
            count += nationValue ? 1 : 0;
            count += useSituationValue ? 1 : 0;
            count += outputValueValue ? 1 : 0;
            count += familyCheckOpinionValue ? 1 : 0;
            count += yieldValue ? 1 : 0;
            return count;
        }

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetColumnValue(int value)
        {
            string columnName = string.Empty;
            switch (value)
            {
                case 1:
                    columnName = "A";
                    break;

                case 2:
                    columnName = "B";
                    break;

                case 3:
                    columnName = "C";
                    break;

                case 4:
                    columnName = "D";
                    break;

                case 5:
                    columnName = "E";
                    break;

                case 6:
                    columnName = "F";
                    break;

                case 7:
                    columnName = "G";
                    break;

                case 8:
                    columnName = "H";
                    break;

                case 9:
                    columnName = "I";
                    break;

                case 10:
                    columnName = "J";
                    break;

                case 11:
                    columnName = "K";
                    break;

                case 12:
                    columnName = "L";
                    break;

                case 13:
                    columnName = "M";
                    break;

                case 14:
                    columnName = "N";
                    break;

                case 15:
                    columnName = "O";
                    break;

                case 16:
                    columnName = "P";
                    break;

                case 17:
                    columnName = "Q";
                    break;

                case 18:
                    columnName = "R";
                    break;

                case 19:
                    columnName = "S";
                    break;

                case 20:
                    columnName = "T";
                    break;

                case 21:
                    columnName = "U";
                    break;

                case 22:
                    columnName = "V";
                    break;

                case 23:
                    columnName = "W";
                    break;

                case 24:
                    columnName = "X";
                    break;

                case 25:
                    columnName = "Y";
                    break;

                case 26:
                    columnName = "Z";
                    break;

                case 27:
                    columnName = "AA";
                    break;

                case 28:
                    columnName = "AB";
                    break;

                case 29:
                    columnName = "AC";
                    break;

                case 30:
                    columnName = "AD";
                    break;

                case 31:
                    columnName = "AE";
                    break;

                case 32:
                    columnName = "AF";
                    break;

                case 33:
                    columnName = "AG";
                    break;

                case 34:
                    columnName = "AH";
                    break;

                case 35:
                    columnName = "AI";
                    break;

                case 36:
                    columnName = "AJ";
                    break;

                case 37:
                    columnName = "AK";
                    break;

                case 38:
                    columnName = "AL";
                    break;

                case 39:
                    columnName = "AM";
                    break;

                case 40:
                    columnName = "AN";
                    break;

                case 41:
                    columnName = "AO";
                    break;

                case 42:
                    columnName = "AP";
                    break;

                case 43:
                    columnName = "AQ";
                    break;

                case 44:
                    columnName = "AR";
                    break;

                case 45:
                    columnName = "AS";
                    break;

                case 46:
                    columnName = "AT";
                    break;

                case 47:
                    columnName = "AU";
                    break;

                case 48:
                    columnName = "AV";
                    break;

                case 49:
                    columnName = "AW";
                    break;

                case 50:
                    columnName = "AX";
                    break;

                case 51:
                    columnName = "AY";
                    break;

                case 52:
                    columnName = "AZ";
                    break;

                case 53:
                    columnName = "BA";
                    break;

                case 54:
                    columnName = "BB";
                    break;

                case 55:
                    columnName = "BC";
                    break;

                case 56:
                    columnName = "BD";
                    break;

                case 57:
                    columnName = "BE";
                    break;

                case 58:
                    columnName = "BF";
                    break;

                case 59:
                    columnName = "BG";
                    break;

                case 60:
                    columnName = "BH";
                    break;

                case 61:
                    columnName = "BI";
                    break;

                case 62:
                    columnName = "BJ";
                    break;

                case 63:
                    columnName = "BK";
                    break;

                case 64:
                    columnName = "BL";
                    break;

                case 65:
                    columnName = "BM";
                    break;

                case 66:
                    columnName = "BN";
                    break;

                case 67:
                    columnName = "BO";
                    break;

                case 68:
                    columnName = "BP";
                    break;

                case 69:
                    columnName = "BQ";
                    break;

                case 70:
                    columnName = "BR";
                    break;

                case 71:
                    columnName = "BS";
                    break;

                case 72:
                    columnName = "BT";
                    break;

                case 73:
                    columnName = "BU";
                    break;

                case 74:
                    columnName = "BV";
                    break;

                case 75:
                    columnName = "BW";
                    break;

                case 76:
                    columnName = "BX";
                    break;

                case 77:
                    columnName = "BY";
                    break;

                case 78:
                    columnName = "BZ";
                    break;

                case 79:
                    columnName = "CA";
                    break;

                case 80:
                    columnName = "CB";
                    break;

                case 81:
                    columnName = "CC";
                    break;

                case 82:
                    columnName = "CD";
                    break;

                case 83:
                    columnName = "CE";
                    break;

                case 84:
                    columnName = "CF";
                    break;

                case 85:
                    columnName = "CG";
                    break;

                case 86:
                    columnName = "CH";
                    break;

                case 87:
                    columnName = "CI";
                    break;

                case 88:
                    columnName = "CJ";
                    break;

                case 89:
                    columnName = "CK";
                    break;

                case 90:
                    columnName = "CL";
                    break;

                case 91:
                    columnName = "CM";
                    break;

                case 92:
                    columnName = "CN";
                    break;

                case 93:
                    columnName = "CO";
                    break;

                case 94:
                    columnName = "CP";
                    break;

                case 95:
                    columnName = "CQ";
                    break;

                case 96:
                    columnName = "CR";
                    break;

                case 97:
                    columnName = "CS";
                    break;

                case 98:
                    columnName = "CT";
                    break;

                case 99:
                    columnName = "CU";
                    break;

                case 100:
                    columnName = "CV";
                    break;

                case 101:
                    columnName = "CW";
                    break;

                case 102:
                    columnName = "CX";
                    break;

                case 103:
                    columnName = "CY";
                    break;

                case 104:
                    columnName = "CZ";
                    break;

                case 105:
                    columnName = "DA";
                    break;

                case 106:
                    columnName = "DB";
                    break;

                case 107:
                    columnName = "DC";
                    break;

                case 108:
                    columnName = "DD";
                    break;

                case 109:
                    columnName = "DE";
                    break;

                case 110:
                    columnName = "DF";
                    break;

                case 111:
                    columnName = "DG";
                    break;

                case 112:
                    columnName = "DH";
                    break;

                case 113:
                    columnName = "DI";
                    break;

                case 114:
                    columnName = "DJ";
                    break;

                case 115:
                    columnName = "DK";
                    break;

                case 116:
                    columnName = "DL";
                    break;

                case 117:
                    columnName = "DM";
                    break;

                case 118:
                    columnName = "DN";
                    break;

                case 119:
                    columnName = "DO";
                    break;

                case 120:
                    columnName = "DP";
                    break;

                case 121:
                    columnName = "DQ";
                    break;

                case 122:
                    columnName = "DR";
                    break;

                case 123:
                    columnName = "DS";
                    break;

                case 124:
                    columnName = "DT";
                    break;

                case 125:
                    columnName = "DU";
                    break;

                case 126:
                    columnName = "DV";
                    break;

                case 127:
                    columnName = "DW";
                    break;

                case 128:
                    columnName = "DX";
                    break;

                case 129:
                    columnName = "DY";
                    break;

                case 130:
                    columnName = "DZ";
                    break;
            }
            return columnName;
        }

        /// <summary>
        /// 初始化二轮承包方信息
        /// </summary>
        /// <returns></returns>
        public bool InitalizeTableValue()
        {
            bool hasContractor = SecondNameValue || SecondNumberValue || SecondNumberNameValue
                || SecondNumberGenderValue || SecondNumberAgeValue || SecondNumberIcnValue || SecondAgeValue
                || SecondNumberRelatioinValue || SecondFamilyCommentValue || SecondNationValue;
            bool extend = ExPackageNameValue || ExPackageNumberValue || IsDeadedValue
                || LocalMarriedRetreatLandValue || PeasantsRetreatLandValue || ForeignMarriedRetreatLandValue;
            bool other = FirstContractorPersonNumberValue || FirstContractAreaValue || SecondContractorPersonNumberValue
                || SecondExtensionPackAreaValue || FoodCropAreaValue;
            return hasContractor || extend || other;
        }

        /// <summary>
        /// 初始化二轮承包方信息
        /// </summary>
        /// <returns></returns>
        public bool InitalizeTableLandValue()
        {
            return SecondLandNameValue || SecondLandTypeValue || SecondTableAreaValue || SecondTotalTableAreaValue
                || SecondLandNeighborValue || SecondCommentValue || SecondLandNumberValue || SecondArableTypeValue
                || SecondIsFarmerLandValue || SecondLandPurposeValue || SecondLandLevelValue;
        }

        /// <summary>
        /// 初始化户籍信息
        /// </summary>
        /// <returns></returns>
        public bool InitalizeCensusValue()
        {
            return LaborNumberValue || IsSourceContractorValue || ContractorNumberValue || MoveFormerlyLandTypeValue
                   || MoveFormerlyLandAreaValue || AccountNatureValue || SourceMoveValue || MoveTimeValue
                   || IsNinetyNineSharePersonValue || CencueCommentValue || FarmerNatureValue;
        }

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static PublicityConfirmDefineFuSui GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<PublicityConfirmDefineFuSui>();
            var section = profile.GetSection<PublicityConfirmDefineFuSui>();
            return section.Settings;
        }
    }
}