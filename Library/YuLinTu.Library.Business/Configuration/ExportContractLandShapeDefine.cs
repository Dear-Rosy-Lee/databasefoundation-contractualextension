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
    /// 导出地块Shape数据
    /// </summary>
    public class ExportContractLandShapeDefine : NotifyCDObject
    {
        /// <summary>
        /// 地块编码截取位数
        /// </summary>
        [DisplayLanguage("地块编码截取位数", IsLanguageName = false)]
        [DescriptionLanguage("地块编码截取位数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "")]
        public int LandNumberGetCount
        {
            get { return landNumberGetCount; }
            set { landNumberGetCount = value; NotifyPropertyChanged("LandNumberGetCount"); }
        }
        private int landNumberGetCount;

        /// <summary>
        ///承包方户号
        /// </summary>
        [DisplayLanguage("承包方户号", IsLanguageName = false)]
        [DescriptionLanguage("承包方户号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool FamilyIndex
        {
            get { return familyIndex; }
            set { familyIndex = value; NotifyPropertyChanged("FamilyIndex"); }
        }
        private bool familyIndex;

        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool NameIndex
        {
            get { return nameIndex; }
            set { nameIndex = value; NotifyPropertyChanged("NameIndex"); }
        }
        private bool nameIndex;

        /// <summary>
        ///证件号码
        /// </summary>
        [DisplayLanguage("证件号码", IsLanguageName = false)]
        [DescriptionLanguage("证件号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool VPNumberIndex
        {
            get { return vpNumberIndex; }
            set { vpNumberIndex = value; NotifyPropertyChanged("VPNumberIndex"); }
        }
        private bool vpNumberIndex;

        /// <summary>
        ///户主备注
        /// </summary>
        [DisplayLanguage("户主备注", IsLanguageName = false)]
        [DescriptionLanguage("户主备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool VPCommentIndex
        {
            get { return vpCommentIndex; }
            set { vpCommentIndex = value; NotifyPropertyChanged("VPCommentIndex"); }
        }
        private bool vpCommentIndex;


        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandNameIndex
        {
            get { return landNameIndex; }
            set { landNameIndex = value; NotifyPropertyChanged("LandNameIndex"); }
        }
        private bool landNameIndex;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool CadastralNumberIndex
        {
            get { return cadastralNumberIndex; }
            set { cadastralNumberIndex = value; NotifyPropertyChanged("CadastralNumberIndex"); }
        }
        private bool cadastralNumberIndex;

        /// <summary>
        ///调查编码
        /// </summary>
        [DisplayLanguage("调查编码", IsLanguageName = false)]
        [DescriptionLanguage("调查编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool SurveyNumberIndex
        {
            get { return surveyNumberIndex; }
            set { surveyNumberIndex = value; NotifyPropertyChanged("SurveyNumberIndex"); }
        }
        private bool surveyNumberIndex;



        /// <summary>
        ///图幅编号
        /// </summary>
        [DisplayLanguage("图幅编号", IsLanguageName = false)]
        [DescriptionLanguage("图幅编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ImageNumberIndex
        {
            get { return imageNumberIndex; }
            set { imageNumberIndex = value; NotifyPropertyChanged("ImageNumberIndex"); }
        }
        private bool imageNumberIndex;

        /// <summary>
        ///二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TableAreaIndex
        {
            get { return tableAreaIndex; }
            set { tableAreaIndex = value; NotifyPropertyChanged("TableAreaIndex"); }
        }
        private bool tableAreaIndex;

        /// <summary>
        ///实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ActualAreaIndex
        {
            get { return actualAreaIndex; }
            set { actualAreaIndex = value; NotifyPropertyChanged("ActualAreaIndex"); }
        }
        private bool actualAreaIndex;

        /// <summary>
        ///四至东
        /// </summary>
        [DisplayLanguage("四至东", IsLanguageName = false)]
        [DescriptionLanguage("四至东", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool EastIndex
        {
            get { return eastIndex; }
            set { eastIndex = value; NotifyPropertyChanged("EastIndex"); }
        }
        private bool eastIndex;

        /// <summary>
        ///四至南
        /// </summary>
        [DisplayLanguage("四至南", IsLanguageName = false)]
        [DescriptionLanguage("四至南", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool SourthIndex
        {
            get { return sourthIndex; }
            set { sourthIndex = value; NotifyPropertyChanged("SourthIndex"); }
        }
        private bool sourthIndex;

        /// <summary>
        ///四至西
        /// </summary>
        [DisplayLanguage("四至西", IsLanguageName = false)]
        [DescriptionLanguage("四至西", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool WestIndex
        {
            get { return westIndex; }
            set { westIndex = value; NotifyPropertyChanged("WestIndex"); }
        }
        private bool westIndex;

        /// <summary>
        ///四至北
        /// </summary>
        [DisplayLanguage("四至北", IsLanguageName = false)]
        [DescriptionLanguage("四至北", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool NorthIndex
        {
            get { return northIndex; }
            set { northIndex = value; NotifyPropertyChanged("NorthIndex"); }
        }
        private bool northIndex;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandPurposeIndex
        {
            get { return landPurposeIndex; }
            set { landPurposeIndex = value; NotifyPropertyChanged("LandPurposeIndex"); }
        }
        private bool landPurposeIndex;

        /// <summary>
        ///地力等级
        /// </summary>
        [DisplayLanguage("地力等级", IsLanguageName = false)]
        [DescriptionLanguage("地力等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandLevelIndex
        {
            get { return landLevelIndex; }
            set { landLevelIndex = value; NotifyPropertyChanged("LandLevelIndex"); }
        }
        private bool landLevelIndex;

        /// <summary>
        ///土地利用类型
        /// </summary>
        [DisplayLanguage("土地利用类型", IsLanguageName = false)]
        [DescriptionLanguage("土地利用类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandTypeIndex
        {
            get { return landTypeIndex; }
            set { landTypeIndex = value; NotifyPropertyChanged("LandTypeIndex"); }
        }
        private bool landTypeIndex;

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DisplayLanguage("是否基本农田", IsLanguageName = false)]
        [DescriptionLanguage("是否基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsFarmerLandIndex
        {
            get { return isFarmerLandIndex; }
            set { isFarmerLandIndex = value; NotifyPropertyChanged("IsFarmerLandIndex"); }
        }
        private bool isFarmerLandIndex;

        /// <summary>
        ///指界人
        /// </summary>
        [DisplayLanguage("指界人", IsLanguageName = false)]
        [DescriptionLanguage("指界人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ReferPersonIndex
        {
            get { return referPersonIndex; }
            set { referPersonIndex = value; NotifyPropertyChanged("ReferPersonIndex"); }
        }
        private bool referPersonIndex;


        /// <summary>
        ///地类名称
        /// </summary>
        [DisplayLanguage("地类名称", IsLanguageName = false)]
        [DescriptionLanguage("地类名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandTypeNameIndex
        {
            get { return landTypeNameIndex; }
            set { landTypeNameIndex = value; NotifyPropertyChanged("LandTypeNameIndex"); }
        }
        private bool landTypeNameIndex;

        /// <summary>
        ///是否飞地
        /// </summary>
        [DisplayLanguage("是否飞地", IsLanguageName = false)]
        [DescriptionLanguage("是否飞地", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsFlyLandIndex
        {
            get { return isFlyLandIndex; }
            set { isFlyLandIndex = value; NotifyPropertyChanged("IsFlyLandIndex"); }
        }
        private bool isFlyLandIndex;

        /// <summary>
        ///电话号码
        /// </summary>
        [DisplayLanguage("电话号码", IsLanguageName = false)]
        [DescriptionLanguage("电话号码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool VPTelephoneIndex
        {
            get { return vpTelephoneIndex; }
            set { vpTelephoneIndex = value; NotifyPropertyChanged("VPTelephoneIndex"); }
        }
        private bool vpTelephoneIndex;


        /// <summary>
        ///海拔高度
        /// </summary>
        [DisplayLanguage("海拔高度", IsLanguageName = false)]
        [DescriptionLanguage("海拔高度", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ElevationIndex
        {
            get { return elevationIndex; }
            set { elevationIndex = value; NotifyPropertyChanged("ElevationIndex"); }
        }
        private bool elevationIndex;


        /// <summary>
        ///地块类别
        /// </summary>
        [DisplayLanguage("地块类别", IsLanguageName = false)]
        [DescriptionLanguage("地块类别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ArableTypeIndex
        {
            get { return arableTypeIndex; }
            set { arableTypeIndex = value; NotifyPropertyChanged("ArableTypeIndex"); }
        }
        private bool arableTypeIndex;

        /// <summary>
        ///合同总面积
        /// </summary>
        //[DisplayLanguage("合同总面积", IsLanguageName = false)]
        //[DescriptionLanguage("合同总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool TotalTableAreaIndex
        //{
        //    get { return totalTableAreaIndex; }
        //    set { totalTableAreaIndex = value; NotifyPropertyChanged("TotalTableAreaIndex"); }
        //}
        //private bool totalTableAreaIndex;

        /// <summary>
        ///实测总面积
        /// </summary>
        //[DisplayLanguage("实测总面积", IsLanguageName = false)]
        //[DescriptionLanguage("实测总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool TotalActualAreaIndex
        //{
        //    get { return totalActualAreaIndex; }
        //    set { totalActualAreaIndex = value; NotifyPropertyChanged("TotalActualAreaIndex"); }
        //}
        //private bool totalActualAreaIndex;

        /// <summary>
        ///确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool AwareAreaIndex
        {
            get { return awareAreaIndex; }
            set { awareAreaIndex = value; NotifyPropertyChanged("AwareAreaIndex"); }
        }
        private bool awareAreaIndex;

        /// <summary>
        ///确权总面积
        /// </summary>
        //[DisplayLanguage("确权总面积", IsLanguageName = false)]
        //[DescriptionLanguage("确权总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool TotalAwareAreaIndex
        //{
        //    get { return totalAwareAreaIndex; }
        //    set { totalAwareAreaIndex = value; NotifyPropertyChanged("TotalAwareAreaIndex"); }
        //}
        //private bool totalAwareAreaIndex;

        /// <summary>
        ///机动地面积
        /// </summary>
        [DisplayLanguage("机动地面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool MotorizeAreaIndex
        {
            get { return motorizeAreaIndex; }
            set { motorizeAreaIndex = value; NotifyPropertyChanged("MotorizeAreaIndex"); }
        }
        private bool motorizeAreaIndex;

        /// <summary>
        ///机动地总面积
        /// </summary>
        //[DisplayLanguage("机动地总面积", IsLanguageName = false)]
        //[DescriptionLanguage("机动地总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool TotalMotorizeAreaIndex
        //{
        //    get { return totalMotorizeAreaIndex; }
        //    set { totalMotorizeAreaIndex = value; NotifyPropertyChanged("TotalMotorizeAreaIndex"); }
        //}
        //private bool totalMotorizeAreaIndex;


        /// <summary>
        ///承包方式
        /// </summary>
        [DisplayLanguage("承包方式", IsLanguageName = false)]
        [DescriptionLanguage("承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ConstructModeIndex
        {
            get { return constructModeIndex; }
            set { constructModeIndex = value; NotifyPropertyChanged("ConstructModeIndex"); }
        }
        private bool constructModeIndex;

        /// <summary>
        ///畦数
        /// </summary>
        [DisplayLanguage("畦数", IsLanguageName = false)]
        [DescriptionLanguage("畦数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool PlotNumberIndex
        {
            get { return plotNumberIndex; }
            set { plotNumberIndex = value; NotifyPropertyChanged("PlotNumberIndex"); }
        }
        private bool plotNumberIndex;

        /// <summary>
        ///种植类型
        /// </summary>
        [DisplayLanguage("种植类型", IsLanguageName = false)]
        [DescriptionLanguage("种植类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool PlatTypeIndex
        {
            get { return platTypeIndex; }
            set { platTypeIndex = value; NotifyPropertyChanged("PlatTypeIndex"); }
        }
        private bool platTypeIndex;

        /// <summary>
        ///经营方式
        /// </summary>
        [DisplayLanguage("经营方式", IsLanguageName = false)]
        [DescriptionLanguage("经营方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool ManagementTypeIndex
        {
            get { return managementTypeIndex; }
            set { managementTypeIndex = value; NotifyPropertyChanged("ManagementTypeIndex"); }
        }
        private bool managementTypeIndex;

        /// <summary>
        ///耕保类型
        /// </summary>
        [DisplayLanguage("耕保类型", IsLanguageName = false)]
        [DescriptionLanguage("耕保类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandPlantIndex
        {
            get { return landPlantIndex; }
            set { landPlantIndex = value; NotifyPropertyChanged("LandPlantIndex"); }
        }
        private bool landPlantIndex;

        /// <summary>
        ///原户主姓名
        /// </summary>
        [DisplayLanguage("原户主姓名", IsLanguageName = false)]
        [DescriptionLanguage("原户主姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool SourceNameIndex
        {
            get { return sourceNameIndex; }
            set { sourceNameIndex = value; NotifyPropertyChanged("SourceNameIndex"); }
        }
        private bool sourceNameIndex;

        /// <summary>
        ///座落方位
        /// </summary>
        [DisplayLanguage("座落方位", IsLanguageName = false)]
        [DescriptionLanguage("座落方位", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandLocationIndex
        {
            get { return landLocationIndex; }
            set { landLocationIndex = value; NotifyPropertyChanged("LandLocationIndex"); }
        }
        private bool landLocationIndex;

        /// <summary>
        ///承包地共有人
        /// </summary>
        //[DisplayLanguage("承包地共有人", IsLanguageName = false)]
        //[DescriptionLanguage("承包地共有人", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool SharePersonIndex
        //{
        //    get { return sharePersonIndex; }
        //    set { sharePersonIndex = value; NotifyPropertyChanged("SharePersonIndex"); }
        //}
        //private bool sharePersonIndex;

        /// <summary>
        ///合同编号
        /// </summary>
        //[DisplayLanguage("合同编号", IsLanguageName = false)]
        //[DescriptionLanguage("合同编号", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool ConcordIndex
        //{
        //    get { return concordIndex; }
        //    set { concordIndex = value; NotifyPropertyChanged("ConcordIndex"); }
        //}
        //private bool concordIndex;

        /// <summary>
        ///权证编号
        /// </summary>
        //[DisplayLanguage("权证编号", IsLanguageName = false)]
        //[DescriptionLanguage("权证编号", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBoolean))]
        //public bool RegeditBookIndex
        //{
        //    get { return regeditBookIndex; }
        //    set { regeditBookIndex = value; NotifyPropertyChanged("RegeditBookIndex"); }
        //}
        //private bool regeditBookIndex;

        /// <summary>
        ///是否流转
        /// </summary>
        [DisplayLanguage("是否流转", IsLanguageName = false)]
        [DescriptionLanguage("是否流转", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool IsTransterIndex
        {
            get { return isTransterIndex; }
            set { isTransterIndex = value; NotifyPropertyChanged("IsTransterIndex"); }
        }
        private bool isTransterIndex;

        /// <summary>
        ///流转方式
        /// </summary>
        [DisplayLanguage("流转方式", IsLanguageName = false)]
        [DescriptionLanguage("流转方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterModeIndex
        {
            get { return transterModeIndex; }
            set { transterModeIndex = value; NotifyPropertyChanged("TransterModeIndex"); }
        }
        private bool transterModeIndex;

        /// <summary>
        ///流转期限
        /// </summary>
        [DisplayLanguage("流转期限", IsLanguageName = false)]
        [DescriptionLanguage("流转期限", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterTermIndex
        {
            get { return transterTermIndex; }
            set { transterTermIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }
        private bool transterTermIndex;

        /// <summary>
        ///流转面积
        /// </summary>
        [DisplayLanguage("流转面积", IsLanguageName = false)]
        [DescriptionLanguage("流转面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool TransterAreaIndex
        {
            get { return transterAreaIndex; }
            set { transterAreaIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }
        private bool transterAreaIndex;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyPersonIndex
        {
            get { return landSurveyPersonIndex; }
            set { landSurveyPersonIndex = value; NotifyPropertyChanged("LandSurveyPersonIndex"); }
        }
        private bool landSurveyPersonIndex;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyDateIndex
        {
            get { return landSurveyDateIndex; }
            set { landSurveyDateIndex = value; NotifyPropertyChanged("LandSurveyDateIndex"); }
        }
        private bool landSurveyDateIndex;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandSurveyChronicleIndex
        {
            get { return landSurveyChronicleIndex; }
            set { landSurveyChronicleIndex = value; NotifyPropertyChanged("LandSurveyChronicleIndex"); }
        }
        private bool landSurveyChronicleIndex;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckPersonIndex
        {
            get { return landCheckPersonIndex; }
            set { landCheckPersonIndex = value; NotifyPropertyChanged("LandCheckPersonIndex"); }
        }
        private bool landCheckPersonIndex;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckDateIndex
        {
            get { return landCheckDateIndex; }
            set { landCheckDateIndex = value; NotifyPropertyChanged("LandCheckDateIndex"); }
        }
        private bool landCheckDateIndex;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool LandCheckOpinionIndex
        {
            get { return landCheckOpinionIndex; }
            set { landCheckOpinionIndex = value; NotifyPropertyChanged("LandCheckOpinionIndex"); }
        }
        private bool landCheckOpinionIndex;


        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("地块备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBoolean))]
        public bool CommentIndex
        {
            get { return commentIndex; }
            set { commentIndex = value; NotifyPropertyChanged("CommentIndex"); }
        }
        private bool commentIndex;



        public ExportContractLandShapeDefine()
        {
            NameIndex = true;
            //ShapeIndex = false;
            LandNameIndex = true;
            CadastralNumberIndex = true;
            SurveyNumberIndex = true;
            ImageNumberIndex = true;
            TableAreaIndex = true;
            ActualAreaIndex = true;
            AwareAreaIndex = true;
            EastIndex = true;
            NorthIndex = true;
            WestIndex = true;
            SourthIndex = true;
            LandPurposeIndex = true;
            LandLevelIndex = true;
            LandTypeIndex = true;
            IsFarmerLandIndex = false;
            ReferPersonIndex = false;
            CommentIndex = false;
            //SharePersonIndex = false;
            //ConcordIndex = false;
            //RegeditBookIndex = false;       
            PlotNumberIndex = false;
            //TotalActualAreaIndex = false;
            //TotalAwareAreaIndex = false;
            MotorizeAreaIndex = false;
            //TotalMotorizeAreaIndex = false;       
            //TotalTableAreaIndex = false;         
            ManagementTypeIndex = false;
            SourceNameIndex = false;
            LandLocationIndex = false;
            ConstructModeIndex = false;
            IsTransterIndex = false;
            TransterModeIndex = false;
            TransterTermIndex = false;
            TransterAreaIndex = false;
            PlatTypeIndex = false;
            LandPlantIndex = false;
            ArableTypeIndex = false;

            LandSurveyPersonIndex = false;
            LandSurveyDateIndex = false;
            LandSurveyChronicleIndex = false;
            LandCheckPersonIndex = false;
            LandCheckDateIndex = false;
            LandCheckOpinionIndex = false;
            LandNumberGetCount = 0;//截取位数默认为0
            familyIndex = true;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ExportContractLandShapeDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ExportContractLandShapeDefine>();
            var section = profile.GetSection<ExportContractLandShapeDefine>();
            return section.Settings;
        }

    }
}
