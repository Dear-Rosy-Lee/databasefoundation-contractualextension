/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入地块图斑设置实体
    /// </summary>
    public class ImportZDBZDefine : NotifyCDObject
    {
        /// <summary>
        ///承包方名称
        /// </summary>
        [DisplayLanguage("承包方名称", IsLanguageName = false)]
        [DescriptionLanguage("承包方名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string NameIndex
        {
            get { return nameIndex; }
            set { nameIndex = value; NotifyPropertyChanged("NameIndex"); }
        }
        private string nameIndex;

        /// <summary>
        ///地块名称
        /// </summary>
        [DisplayLanguage("地块名称", IsLanguageName = false)]
        [DescriptionLanguage("地块名称", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandNameIndex
        {
            get { return landNameIndex; }
            set { landNameIndex = value; NotifyPropertyChanged("LandNameIndex"); }
        }
        private string landNameIndex;

        /// <summary>
        ///地块编码
        /// </summary>
        [DisplayLanguage("地块编码", IsLanguageName = false)]
        [DescriptionLanguage("地块编码", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string CadastralNumberIndex
        {
            get { return cadastralNumberIndex; }
            set { cadastralNumberIndex = value; NotifyPropertyChanged("CadastralNumberIndex"); }
        }
        private string cadastralNumberIndex;

        /// <summary>
        ///图幅编号
        /// </summary>
        [DisplayLanguage("图幅编号", IsLanguageName = false)]
        [DescriptionLanguage("图幅编号", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ImageNumberIndex
        {
            get { return imageNumberIndex; }
            set { imageNumberIndex = value; NotifyPropertyChanged("ImageNumberIndex"); }
        }
        private string imageNumberIndex;

        /// <summary>
        ///二轮合同面积
        /// </summary>
        [DisplayLanguage("二轮合同面积", IsLanguageName = false)]
        [DescriptionLanguage("二轮合同面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string TableAreaIndex
        {
            get { return tableAreaIndex; }
            set { tableAreaIndex = value; NotifyPropertyChanged("TableAreaIndex"); }
        }
        private string tableAreaIndex;

        /// <summary>
        ///实测面积
        /// </summary>
        [DisplayLanguage("实测面积", IsLanguageName = false)]
        [DescriptionLanguage("实测面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ActualAreaIndex
        {
            get { return actualAreaIndex; }
            set { actualAreaIndex = value; NotifyPropertyChanged("ActualAreaIndex"); }
        }
        private string actualAreaIndex;

        /// <summary>
        ///四至东
        /// </summary>
        [DisplayLanguage("四至东", IsLanguageName = false)]
        [DescriptionLanguage("四至东", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string EastIndex
        {
            get { return eastIndex; }
            set { eastIndex = value; NotifyPropertyChanged("EastIndex"); }
        }
        private string eastIndex;

        /// <summary>
        ///四至南
        /// </summary>
        [DisplayLanguage("四至南", IsLanguageName = false)]
        [DescriptionLanguage("四至南", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string SourthIndex
        {
            get { return sourthIndex; }
            set { sourthIndex = value; NotifyPropertyChanged("SourthIndex"); }
        }
        private string sourthIndex;

        /// <summary>
        ///四至西
        /// </summary>
        [DisplayLanguage("四至西", IsLanguageName = false)]
        [DescriptionLanguage("四至西", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string WestIndex
        {
            get { return westIndex; }
            set { westIndex = value; NotifyPropertyChanged("WestIndex"); }
        }
        private string westIndex;

        /// <summary>
        ///四至北
        /// </summary>
        [DisplayLanguage("四至北", IsLanguageName = false)]
        [DescriptionLanguage("四至北", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string NorthIndex
        {
            get { return northIndex; }
            set { northIndex = value; NotifyPropertyChanged("NorthIndex"); }
        }
        private string northIndex;

        /// <summary>
        ///土地用途
        /// </summary>
        [DisplayLanguage("土地用途", IsLanguageName = false)]
        [DescriptionLanguage("土地用途", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandPurposeIndex
        {
            get { return landPurposeIndex; }
            set { landPurposeIndex = value; NotifyPropertyChanged("LandPurposeIndex"); }
        }
        private string landPurposeIndex;

        /// <summary>
        ///地力等级
        /// </summary>
        [DisplayLanguage("地力等级", IsLanguageName = false)]
        [DescriptionLanguage("地力等级", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandLevelIndex
        {
            get { return landLevelIndex; }
            set { landLevelIndex = value; NotifyPropertyChanged("LandLevelIndex"); }
        }
        private string landLevelIndex;

        /// <summary>
        ///土地利用类型
        /// </summary>
        [DisplayLanguage("土地利用类型", IsLanguageName = false)]
        [DescriptionLanguage("土地利用类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandTypeIndex
        {
            get { return landTypeIndex; }
            set { landTypeIndex = value; NotifyPropertyChanged("LandTypeIndex"); }
        }
        private string landTypeIndex;

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DisplayLanguage("是否基本农田", IsLanguageName = false)]
        [DescriptionLanguage("是否基本农田", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string IsFarmerLandIndex
        {
            get { return isFarmerLandIndex; }
            set { isFarmerLandIndex = value; NotifyPropertyChanged("IsFarmerLandIndex"); }
        }
        private string isFarmerLandIndex;

        /// <summary>
        ///指界人
        /// </summary>
        [DisplayLanguage("指界人", IsLanguageName = false)]
        [DescriptionLanguage("指界人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ReferPersonIndex
        {
            get { return referPersonIndex; }
            set { referPersonIndex = value; NotifyPropertyChanged("ReferPersonIndex"); }
        }
        private string referPersonIndex;

        /// <summary>
        ///地块类别
        /// </summary>
        [DisplayLanguage("地块类别", IsLanguageName = false)]
        [DescriptionLanguage("地块类别", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ArableTypeIndex
        {
            get { return arableTypeIndex; }
            set { arableTypeIndex = value; NotifyPropertyChanged("ArableTypeIndex"); }
        }
        private string arableTypeIndex;

        /// <summary>
        ///合同总面积
        /// </summary>
        //[DisplayLanguage("合同总面积", IsLanguageName = false)]
        //[DescriptionLanguage("合同总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string TotalTableAreaIndex
        //{
        //    get { return totalTableAreaIndex; }
        //    set { totalTableAreaIndex = value; NotifyPropertyChanged("TotalTableAreaIndex"); }
        //}
        //private string totalTableAreaIndex;

        /// <summary>
        ///实测总面积
        /// </summary>
        //[DisplayLanguage("实测总面积", IsLanguageName = false)]
        //[DescriptionLanguage("实测总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string TotalActualAreaIndex
        //{
        //    get { return totalActualAreaIndex; }
        //    set { totalActualAreaIndex = value; NotifyPropertyChanged("TotalActualAreaIndex"); }
        //}
        //private string totalActualAreaIndex;

        /// <summary>
        ///确权面积
        /// </summary>
        [DisplayLanguage("确权面积", IsLanguageName = false)]
        [DescriptionLanguage("确权面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string AwareAreaIndex
        {
            get { return awareAreaIndex; }
            set { awareAreaIndex = value; NotifyPropertyChanged("AwareAreaIndex"); }
        }
        private string awareAreaIndex;

        /// <summary>
        ///确权总面积
        /// </summary>
        //[DisplayLanguage("确权总面积", IsLanguageName = false)]
        //[DescriptionLanguage("确权总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string TotalAwareAreaIndex
        //{
        //    get { return totalAwareAreaIndex; }
        //    set { totalAwareAreaIndex = value; NotifyPropertyChanged("TotalAwareAreaIndex"); }
        //}
        //private string totalAwareAreaIndex;

        /// <summary>
        ///机动地面积
        /// </summary>
        [DisplayLanguage("机动地面积", IsLanguageName = false)]
        [DescriptionLanguage("机动地面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string MotorizeAreaIndex
        {
            get { return motorizeAreaIndex; }
            set { motorizeAreaIndex = value; NotifyPropertyChanged("MotorizeAreaIndex"); }
        }
        private string motorizeAreaIndex;

        /// <summary>
        ///机动地总面积
        /// </summary>
        //[DisplayLanguage("机动地总面积", IsLanguageName = false)]
        //[DescriptionLanguage("机动地总面积", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string TotalMotorizeAreaIndex
        //{
        //    get { return totalMotorizeAreaIndex; }
        //    set { totalMotorizeAreaIndex = value; NotifyPropertyChanged("TotalMotorizeAreaIndex"); }
        //}
        //private string totalMotorizeAreaIndex;


        /// <summary>
        ///承包方式
        /// </summary>
        [DisplayLanguage("承包方式", IsLanguageName = false)]
        [DescriptionLanguage("承包方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ConstructModeIndex
        {
            get { return constructModeIndex; }
            set { constructModeIndex = value; NotifyPropertyChanged("ConstructModeIndex"); }
        }
        private string constructModeIndex;

        /// <summary>
        ///畦数
        /// </summary>
        [DisplayLanguage("畦数", IsLanguageName = false)]
        [DescriptionLanguage("畦数", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PlotNumberIndex
        {
            get { return plotNumberIndex; }
            set { plotNumberIndex = value; NotifyPropertyChanged("PlotNumberIndex"); }
        }
        private string plotNumberIndex;

        /// <summary>
        ///种植类型
        /// </summary>
        [DisplayLanguage("种植类型", IsLanguageName = false)]
        [DescriptionLanguage("种植类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string PlatTypeIndex
        {
            get { return platTypeIndex; }
            set { platTypeIndex = value; NotifyPropertyChanged("PlatTypeIndex"); }
        }
        private string platTypeIndex;

        /// <summary>
        ///经营方式
        /// </summary>
        [DisplayLanguage("经营方式", IsLanguageName = false)]
        [DescriptionLanguage("经营方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string ManagementTypeIndex
        {
            get { return managementTypeIndex; }
            set { managementTypeIndex = value; NotifyPropertyChanged("ManagementTypeIndex"); }
        }
        private string managementTypeIndex;

        /// <summary>
        ///耕保类型
        /// </summary>
        [DisplayLanguage("耕保类型", IsLanguageName = false)]
        [DescriptionLanguage("耕保类型", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandPlantIndex
        {
            get { return landPlantIndex; }
            set { landPlantIndex = value; NotifyPropertyChanged("LandPlantIndex"); }
        }
        private string landPlantIndex;

        /// <summary>
        ///原户主姓名
        /// </summary>
        [DisplayLanguage("原户主姓名", IsLanguageName = false)]
        [DescriptionLanguage("原户主姓名", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string SourceNameIndex
        {
            get { return sourceNameIndex; }
            set { sourceNameIndex = value; NotifyPropertyChanged("SourceNameIndex"); }
        }
        private string sourceNameIndex;

        /// <summary>
        ///座落方位
        /// </summary>
        [DisplayLanguage("座落方位", IsLanguageName = false)]
        [DescriptionLanguage("座落方位", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandLocationIndex
        {
            get { return landLocationIndex; }
            set { landLocationIndex = value; NotifyPropertyChanged("LandLocationIndex"); }
        }
        private string landLocationIndex;

        /// <summary>
        ///承包地共有人
        /// </summary>
        //[DisplayLanguage("承包地共有人", IsLanguageName = false)]
        //[DescriptionLanguage("承包地共有人", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string SharePersonIndex
        //{
        //    get { return sharePersonIndex; }
        //    set { sharePersonIndex = value; NotifyPropertyChanged("SharePersonIndex"); }
        //}
        //private string sharePersonIndex;

        /// <summary>
        ///合同编号
        /// </summary>
        //[DisplayLanguage("合同编号", IsLanguageName = false)]
        //[DescriptionLanguage("合同编号", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string ConcordIndex
        //{
        //    get { return concordIndex; }
        //    set { concordIndex = value; NotifyPropertyChanged("ConcordIndex"); }
        //}
        //private string concordIndex;

        /// <summary>
        ///权证编号
        /// </summary>
        //[DisplayLanguage("权证编号", IsLanguageName = false)]
        //[DescriptionLanguage("权证编号", IsLanguageName = false)]
        //[PropertyDescriptor(Catalog = "地块信息", Gallery = "",
        //   Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        //public string RegeditBookIndex
        //{
        //    get { return regeditBookIndex; }
        //    set { regeditBookIndex = value; NotifyPropertyChanged("RegeditBookIndex"); }
        //}
        //private string regeditBookIndex;

        /// <summary>
        ///是否流转
        /// </summary>
        [DisplayLanguage("是否流转", IsLanguageName = false)]
        [DescriptionLanguage("是否流转", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string IsTransterIndex
        {
            get { return isTransterIndex; }
            set { isTransterIndex = value; NotifyPropertyChanged("IsTransterIndex"); }
        }
        private string isTransterIndex;

        /// <summary>
        ///流转方式
        /// </summary>
        [DisplayLanguage("流转方式", IsLanguageName = false)]
        [DescriptionLanguage("流转方式", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string TransterModeIndex
        {
            get { return transterModeIndex; }
            set { transterModeIndex = value; NotifyPropertyChanged("TransterModeIndex"); }
        }
        private string transterModeIndex;

        /// <summary>
        ///流转期限
        /// </summary>
        [DisplayLanguage("流转期限", IsLanguageName = false)]
        [DescriptionLanguage("流转期限", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string TransterTermIndex
        {
            get { return transterTermIndex; }
            set { transterTermIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }
        private string transterTermIndex;

        /// <summary>
        ///流转面积
        /// </summary>
        [DisplayLanguage("流转面积", IsLanguageName = false)]
        [DescriptionLanguage("流转面积", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string TransterAreaIndex
        {
            get { return transterAreaIndex; }
            set { transterAreaIndex = value; NotifyPropertyChanged("TransterTermIndex"); }
        }
        private string transterAreaIndex;

        /// <summary>
        ///调查员
        /// </summary>
        [DisplayLanguage("调查员", IsLanguageName = false)]
        [DescriptionLanguage("调查员", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandSurveyPersonIndex
        {
            get { return landSurveyPersonIndex; }
            set { landSurveyPersonIndex = value; NotifyPropertyChanged("LandSurveyPersonIndex"); }
        }
        private string landSurveyPersonIndex;

        /// <summary>
        ///调查日期
        /// </summary>
        [DisplayLanguage("调查日期", IsLanguageName = false)]
        [DescriptionLanguage("调查日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandSurveyDateIndex
        {
            get { return landSurveyDateIndex; }
            set { landSurveyDateIndex = value; NotifyPropertyChanged("LandSurveyDateIndex"); }
        }
        private string landSurveyDateIndex;

        /// <summary>
        ///调查记事
        /// </summary>
        [DisplayLanguage("调查记事", IsLanguageName = false)]
        [DescriptionLanguage("调查记事", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandSurveyChronicleIndex
        {
            get { return landSurveyChronicleIndex; }
            set { landSurveyChronicleIndex = value; NotifyPropertyChanged("LandSurveyChronicleIndex"); }
        }
        private string landSurveyChronicleIndex;

        /// <summary>
        ///审核人
        /// </summary>
        [DisplayLanguage("审核人", IsLanguageName = false)]
        [DescriptionLanguage("审核人", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandCheckPersonIndex
        {
            get { return landCheckPersonIndex; }
            set { landCheckPersonIndex = value; NotifyPropertyChanged("LandCheckPersonIndex"); }
        }
        private string landCheckPersonIndex;

        /// <summary>
        ///审核日期
        /// </summary>
        [DisplayLanguage("审核日期", IsLanguageName = false)]
        [DescriptionLanguage("审核日期", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandCheckDateIndex
        {
            get { return landCheckDateIndex; }
            set { landCheckDateIndex = value; NotifyPropertyChanged("LandCheckDateIndex"); }
        }
        private string landCheckDateIndex;

        /// <summary>
        ///审核意见
        /// </summary>
        [DisplayLanguage("审核意见", IsLanguageName = false)]
        [DescriptionLanguage("审核意见", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string LandCheckOpinionIndex
        {
            get { return landCheckOpinionIndex; }
            set { landCheckOpinionIndex = value; NotifyPropertyChanged("LandCheckOpinionIndex"); }
        }
        private string landCheckOpinionIndex;


        /// <summary>
        ///备注
        /// </summary>
        [DisplayLanguage("备注", IsLanguageName = false)]
        [DescriptionLanguage("备注", IsLanguageName = false)]
        [PropertyDescriptor(Catalog = "地块信息", Gallery = "",
           Builder = typeof(PropertyDescriptorBuilderString), Trigger = typeof(PropertyTriggerFamilyImportConfigState))]
        public string CommentIndex
        {
            get { return commentIndex; }
            set { commentIndex = value; NotifyPropertyChanged("CommentIndex"); }
        }
        private string commentIndex;

        public ImportZDBZDefine()
        {
            NameIndex = "None";
            //ShapeIndex = "None";
            LandNameIndex = "None";
            CadastralNumberIndex = "None";
            ImageNumberIndex = "None";
            TableAreaIndex = "None";
            ActualAreaIndex = "None";
            AwareAreaIndex = "None";
            EastIndex = "None";
            NorthIndex = "None";
            WestIndex = "None";
            SourthIndex = "None";
            LandPurposeIndex = "None";
            LandLevelIndex = "None";
            LandTypeIndex = "None";
            IsFarmerLandIndex = "None";
            ReferPersonIndex = "None";
            CommentIndex = "None";
            //SharePersonIndex = "None";
            //ConcordIndex = "None";
            //RegeditBookIndex = "None";       
            PlotNumberIndex = "None";
            //TotalActualAreaIndex = "None";
            //TotalAwareAreaIndex = "None";
            MotorizeAreaIndex = "None";
            //TotalMotorizeAreaIndex = "None";       
            //TotalTableAreaIndex = "None";         
            ManagementTypeIndex = "None";
            SourceNameIndex = "None";
            LandLocationIndex = "None";
            ConstructModeIndex = "None";
            IsTransterIndex = "None";
            TransterModeIndex = "None";
            TransterTermIndex = "None";
            TransterAreaIndex = "None";
            PlatTypeIndex = "None";
            LandPlantIndex = "None";
            ArableTypeIndex = "None";

            LandSurveyPersonIndex = "None";
            LandSurveyDateIndex = "None";
            LandSurveyChronicleIndex = "None";
            LandCheckPersonIndex = "None";
            LandCheckDateIndex = "None";
            LandCheckOpinionIndex = "None";
        }


        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static ImportZDBZDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ImportZDBZDefine>();
            var section = profile.GetSection<ImportZDBZDefine>();
            var setting = section.Settings as ImportZDBZDefine;
            return setting.Clone() as ImportZDBZDefine ;
            //systemSet = (section.Settings as SystemSetDefine);
            //SystemSettingDefine = systemSet.Clone() as SystemSetDefine;
        }

    }
}
