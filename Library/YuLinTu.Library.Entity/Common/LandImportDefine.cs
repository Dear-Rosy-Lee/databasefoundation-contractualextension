/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地块自定义
    /// </summary>
    [Serializable]
    public class LandImportDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 户主名索引
        /// </summary>
        public int NameIndex { get; set; }

        /// <summary>
        /// 户主类型索引
        /// </summary>
        public int ContractorTypeIndex { get; set; }

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        public int NumberIndex { get; set; }

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        public int NumberNameIndex { get; set; }

        /// <summary>
        /// 家庭成员证件类型索引
        /// </summary>
        public int NumberCartTypeIndex { get; set; }

        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        public int NumberIcnIndex { get; set; }

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        public int NumberGenderIndex { get; set; }

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        public int NumberAgeIndex { get; set; }

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        public int NumberRelatioinIndex { get; set; }

        /// <summary>
        /// 二轮户主名索引
        /// </summary>
        public int SecondNameIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员个数索引
        /// </summary>
        public int SecondNumberIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员姓名索引
        /// </summary>
        public int SecondNumberNameIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员身份证号索引
        /// </summary>
        public int SecondNumberIcnIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员性别索引
        /// </summary>
        public int SecondNumberGenderIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员年龄索引
        /// </summary>
        public int SecondNumberAgeIndex { get; set; }

        /// <summary>
        /// 二轮家庭成员关系索引
        /// </summary>
        public int SecondNumberRelatioinIndex { get; set; }

        /// <summary>
        /// 二轮承包方备注索引
        /// </summary>
        public int SecondFamilyCommentIndex { get; set; }

        /// <summary>
        /// 二轮延包姓名索引
        /// </summary>
        public int ExPackageNameIndex { get; set; }

        /// <summary>
        /// 延包土地份数索引
        /// </summary>
        public int ExPackageNumberIndex { get; set; }

        /// <summary>
        /// 已死亡人员索引
        /// </summary>
        public int IsDeadedIndex { get; set; }

        /// <summary>
        /// 出嫁后未退承包地人员索引
        /// </summary>
        public int LocalMarriedRetreatLandIndex { get; set; }

        /// <summary>
        /// 农转非后未退承包地人员索引
        /// </summary>
        public int PeasantsRetreatLandIndex { get; set; }

        /// <summary>
        /// 婚进但在非出地未退承包地人员索引
        /// </summary>
        public int ForeignMarriedRetreatLandIndex { get; set; }

        /// <summary>
        /// 承包地共有人索引
        /// </summary>
        public int SharePersonIndex { get; set; }

        /// <summary>
        /// 是否享有承包地索引
        /// </summary>
        public int IsSharedLandIndex { get; set; }

        /// <summary>
        /// 合同编号索引
        /// </summary>
        public int ConcordIndex { get; set; }

        /// <summary>
        /// 权证编号索引
        /// </summary>
        public int RegeditBookIndex { get; set; }

        /// <summary>
        /// 地籍号索引
        /// </summary>
        public int CadastralNumberIndex { get; set; }

        /// <summary>
        /// 小地名索引
        /// </summary>
        public int LandNameIndex { get; set; }

        /// <summary>
        /// 图幅编号索引
        /// </summary>
        public int ImageNumberIndex { get; set; }

        /// <summary>
        /// 二轮小地名索引
        /// </summary>
        public int SecondLandNameIndex { get; set; }

        /// <summary>
        /// 畦数索引
        /// </summary>
        public int PlotNumberIndex { get; set; }

        /// <summary>
        /// 实测面积索引
        /// </summary>
        public int ActualAreaIndex { get; set; }

        /// <summary>
        /// 实测总面积索引
        /// </summary>
        public int TotalActualAreaIndex { get; set; }

        /// <summary>
        /// 确权面积索引
        /// </summary>
        public int AwareAreaIndex { get; set; }

        /// <summary>
        /// 确权总面积索引
        /// </summary>
        public int TotalAwareAreaIndex { get; set; }

        /// <summary>
        /// 机动地面积索引
        /// </summary>
        public int MotorizeAreaIndex { get; set; }

        /// <summary>
        /// 机动地总面积索引
        /// </summary>
        public int TotalMotorizeAreaIndex { get; set; }

        /// <summary>
        /// 二轮台账面积索引
        /// </summary>
        public int TableAreaIndex { get; set; }

        /// <summary>
        /// 二轮台账面积索引
        /// </summary>
        public int SecondTableAreaIndex { get; set; }

        /// <summary>
        /// 二轮台账总面积索引
        /// </summary>
        public int TotalTableAreaIndex { get; set; }

        /// <summary>
        /// 二轮台账总面积索引
        /// </summary>
        public int SecondTotalTableAreaIndex { get; set; }

        /// <summary>
        /// 地类索引
        /// </summary>
        public int LandTypeIndex { get; set; }

        /// <summary>
        /// 二轮地类索引
        /// </summary>
        public int SecondLandTypeIndex { get; set; }

        /// <summary>
        /// 经营方式索引
        /// </summary>
        public int ManagementTypeIndex { get; set; }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public int IsFarmerLandIndex { get; set; }

        /// <summary>
        /// 土地用途
        /// </summary>
        public int LandPurposeIndex { get; set; }

        /// <summary>
        /// 四至东索引
        /// </summary>
        public int EastIndex { get; set; }

        /// <summary>
        /// 二轮四至东索引
        /// </summary>
        public int SecondEastIndex { get; set; }

        /// <summary>
        /// 四至南索引
        /// </summary>
        public int SourthIndex { get; set; }

        /// <summary>
        /// 二轮四至南索引
        /// </summary>
        public int SecondSourthIndex { get; set; }

        /// <summary>
        /// 四至西索引
        /// </summary>
        public int WestIndex { get; set; }

        /// <summary>
        /// 二轮四至西索引
        /// </summary>
        public int SecondWestIndex { get; set; }

        /// <summary>
        /// 四至北索引
        /// </summary>
        public int NorthIndex { get; set; }

        /// <summary>
        /// 二轮四至北索引
        /// </summary>
        public int SecondNorthIndex { get; set; }

        /// <summary>
        /// 原户主名索引
        /// </summary>
        public int SourceNameIndex { get; set; }

        /// <summary>
        /// 宗地座落索引
        /// </summary>
        public int LandLocationIndex { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public int ConstructModeIndex { get; set; }

        /// <summary>
        /// 是否流转索引
        /// </summary>
        public int IsTransterIndex { get; set; }

        /// <summary>
        /// 流转方式索引
        /// </summary>
        public int TransterModeIndex { get; set; }

        /// <summary>
        /// 流转期限索引
        /// </summary>
        public int TransterTermIndex { get; set; }

        /// <summary>
        /// 流转面积
        /// </summary>
        public int TransterAreaIndex { get; set; }

        /// <summary>
        /// 种植类型索引
        /// </summary>
        public int PlatTypeIndex { get; set; }

        /// <summary>
        /// 承包方地址索引
        /// </summary>
        public int ContractorAddressIndex { get; set; }

        /// <summary>
        /// 邮政编码索引
        /// </summary>
        public int PostNumberIndex { get; set; }

        /// <summary>
        /// 电话号码索引
        /// </summary>
        public int TelephoneIndex { get; set; }

        /// <summary>
        /// 备注索引
        /// </summary>
        public int CommentIndex { get; set; }

        /// <summary>
        /// 二轮备注索引
        /// </summary>
        public int SecondCommentIndex { get; set; }

        /// <summary>
        /// 承包方备注索引
        /// </summary>
        public int FamilyCommentIndex { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int LandLevelIndex { get; set; }

        /// <summary>
        /// 耕保等级
        /// </summary>
        public int LandPlantIndex { get; set; }

        /// <summary>
        /// 耕地类型
        /// </summary>
        public int ArableTypeIndex { get; set; }

        /// <summary>
        /// 实际分配人数
        /// </summary>
        public int AllocationPersonIndex { get; set; }

        /// <summary>
        /// 利用情况
        /// </summary>
        public int UseSituationIndex { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public int YieldIndex { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public int OutputValueIndex { get; set; }

        /// <summary>
        /// 收益情况
        /// </summary>
        public int IncomeSituationIndex { get; set; }

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
        /// 总劳力数
        /// </summary>
        public int LaborNumberIndex { get; set; }

        /// <summary>
        /// 是否是原承包户
        /// </summary>
        public int IsSourceContractorIndex { get; set; }

        /// <summary>
        /// 现承包人数
        /// </summary>
        public int ContractorNumberIndex { get; set; }

        /// <summary>
        /// 农户性质
        /// </summary>
        public int FarmerNatureIndex { get; set; }

        /// <summary>
        /// 迁入前土地类型
        /// </summary>
        public int MoveFormerlyLandTypeIndex { get; set; }

        /// <summary>
        /// 迁入前土地面积
        /// </summary>
        public int MoveFormerlyLandAreaIndex { get; set; }

        /// <summary>
        /// 一轮承包人数
        /// </summary>
        public int FirstContractorPersonNumberIndex { get; set; }

        /// <summary>
        /// 一轮承包面积
        /// </summary>
        public int FirstContractAreaIndex { get; set; }

        /// <summary>
        /// 二轮承包人数
        /// </summary>
        public int SecondContractorPersonNumberIndex { get; set; }

        /// <summary>
        /// 二轮延包面积
        /// </summary>
        public int SecondExtensionPackAreaIndex { get; set; }

        /// <summary>
        /// 粮食种植面积
        /// </summary>
        public int FoodCropAreaIndex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int AgeIndex { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public int NationIndex { get; set; }

        /// <summary>
        /// 二轮年龄
        /// </summary>
        public int SecondAgeIndex { get; set; }

        /// <summary>
        /// 二轮民族
        /// </summary>
        public int SecondNationIndex { get; set; }

        /// <summary>
        /// 二轮地块编码
        /// </summary>
        public int SecondLandNumberIndex { get; set; }

        /// <summary>
        /// 二轮地块类型
        /// </summary>
        public int SecondArableTypeIndex { get; set; }

        /// <summary>
        /// 二轮是否基本农田
        /// </summary>
        public int SecondIsFarmerLandIndex { get; set; }

        /// <summary>
        /// 二轮土地用途
        /// </summary>
        public int SecondLandPurposeIndex { get; set; }

        /// <summary>
        /// 二轮土地等级
        /// </summary>
        public int SecondLandLevelIndex { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public int AccountNatureIndex { get; set; }

        /// <summary>
        /// 从何处迁入
        /// </summary>
        public int SourceMoveIndex { get; set; }

        /// <summary>
        /// 迁入时间
        /// </summary>
        public int MoveTimeIndex { get; set; }

        /// <summary>
        /// 是否为99年共有人
        /// </summary>
        public int IsNinetyNineSharePersonIndex { get; set; }

        /// <summary>
        /// 户籍备注
        /// </summary>
        public int CencueCommentIndex { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public int SecondConcordNumberIndex { get; set; }

        /// <summary>
        /// 证书编号
        /// </summary>
        public int SecondWarrantNumberIndex { get; set; }

        /// <summary>
        /// 合同开始时间
        /// </summary>
        public int StartTimeIndex { get; set; }

        /// <summary>
        /// 合同结束时间
        /// </summary>
        public int EndTimeIndex { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public int ConstructTypeIndex { get; set; }

        /// <summary>
        /// 户籍调查员
        /// </summary>
        public int FamilySurveyPersonIndex { get; set; }

        /// <summary>
        /// 户籍调查日期
        /// </summary>
        public int FamilySurveyDateIndex { get; set; }

        /// <summary>
        /// 户籍调查记事
        /// </summary>
        public int FamilySurveyChronicleIndex { get; set; }

        /// <summary>
        /// 户籍审核员
        /// </summary>
        public int FamilyCheckPersonIndex { get; set; }

        /// <summary>
        /// 户籍审核日期
        /// </summary>
        public int FamilyCheckDateIndex { get; set; }

        /// <summary>
        /// 户籍审核意见
        /// </summary>
        public int FamilyCheckOpinionIndex { get; set; }

        /// <summary>
        /// 地块调查员
        /// </summary>
        public int LandSurveyPersonIndex { get; set; }

        /// <summary>
        /// 地块调查日期
        /// </summary>
        public int LandSurveyDateIndex { get; set; }

        /// <summary>
        /// 地块调查记事
        /// </summary>
        public int LandSurveyChronicleIndex { get; set; }

        /// <summary>
        /// 地块审核员
        /// </summary>
        public int LandCheckPersonIndex { get; set; }

        /// <summary>
        /// 地块审核日期
        /// </summary>
        public int LandCheckDateIndex { get; set; }

        /// <summary>
        /// 地块审核意见
        /// </summary>
        public int LandCheckOpinionIndex { get; set; }

        /// <summary>
        /// 指界人
        /// </summary>
        public int ReferPersonIndex { get; set; }

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

        #endregion

        #region Ctor

        public LandImportDefine()
        {
            //NameIndex = 1;
            //ContractorTypeIndex = -1;
            //SecondNameIndex = 1;
            //NumberIndex = 2;
            //SecondNumberIndex = 2;
            //NumberNameIndex = 3;
            //SecondNumberNameIndex = 3;
            //NumberGenderIndex = 4;
            //SecondNumberGenderIndex = 4;
            //NumberAgeIndex = 5;
            //SecondNumberAgeIndex = 5;
            //NumberCartTypeIndex = -1;
            //NumberIcnIndex = 6;
            //SecondNumberIcnIndex = 6;
            //NumberRelatioinIndex = 7;
            //SecondNumberRelatioinIndex = 7;
            //IsSharedLandIndex = -1;
            //FamilyCommentIndex = 8;
            //ContractorAddressIndex = -1;
            //PostNumberIndex = -1;
            //TelephoneIndex = 32;
            //FamilySurveyPersonIndex = -1;
            //FamilySurveyDateIndex = -1;
            //FamilySurveyChronicleIndex = -1;
            //FamilyCheckPersonIndex = -1;
            //FamilyCheckDateIndex = -1;
            //FamilyCheckOpinionIndex = -1;
            //LandNameIndex = 23;
            //CadastralNumberIndex = -1;
            //ImageNumberIndex = -1;
            //TableAreaIndex = 11;
            //ActualAreaIndex = 20;
            //AwareAreaIndex = -1;
            //EastIndex = 25;
            //NorthIndex = 28;
            //WestIndex = 27;
            //SourthIndex = 26;
            //LandPurposeIndex = -1;
            //LandLevelIndex = -1;
            //LandTypeIndex = 22;
            //IsFarmerLandIndex = -1;
            //ReferPersonIndex = -1;
            //CommentIndex = 33;
            //LandSurveyPersonIndex = -1;
            //LandSurveyDateIndex = -1;
            //LandSurveyChronicleIndex = -1;
            //LandCheckPersonIndex = -1;
            //LandCheckDateIndex = -1;
            //LandCheckOpinionIndex = -1;
            //SecondFamilyCommentIndex = 8;
            //ExPackageNameIndex = -1;
            //ExPackageNumberIndex = -1;
            //IsDeadedIndex = -1;
            //LocalMarriedRetreatLandIndex = -1;
            //PeasantsRetreatLandIndex = -1;
            //ForeignMarriedRetreatLandIndex = -1;
            //SharePersonIndex = -1;
            //ConcordIndex = -1;
            //RegeditBookIndex = -1;
            //SecondLandNameIndex = 13;
            //PlotNumberIndex = -1;
            //TotalActualAreaIndex = 19;
            //TotalAwareAreaIndex = -1;
            //MotorizeAreaIndex = -1;
            //TotalMotorizeAreaIndex = -1;
            //SecondTableAreaIndex = 11;
            //TotalTableAreaIndex = 10;
            //SecondTotalTableAreaIndex = 10;
            //SecondLandTypeIndex = 12;
            //ManagementTypeIndex = -1;
            //SecondEastIndex = 14;
            //SecondSourthIndex = 15;
            //SecondWestIndex = 16;
            //SecondNorthIndex = 17;
            //SourceNameIndex = -1;
            //LandLocationIndex = -1;
            //ConstructModeIndex = 27;
            //IsTransterIndex = 28;
            //TransterModeIndex = 29;
            //TransterTermIndex = 30;
            //TransterAreaIndex = -1;
            //PlatTypeIndex = 31;
            //LandPlantIndex = -1;
            //ArableTypeIndex = -1;
            //SecondCommentIndex = 33;
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
            //SecondContractorPersonNumberIndex = 9;
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
            //SecondLandNumberIndex = 18;
            //SecondArableTypeIndex = -1;
            //SecondIsFarmerLandIndex = -1;
            //SecondLandPurposeIndex = -1;
            //SecondLandLevelIndex = -1;
            //CencueCommentIndex = -1;
            //SecondConcordNumberIndex = -1;
            //SecondWarrantNumberIndex = -1;
            //StartTimeIndex = -1;
            //EndTimeIndex = -1;
            //ConstructTypeIndex = 27;

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
            ContractorAddressIndex = 11;
            PostNumberIndex = 12;
            TelephoneIndex = 13;
            FamilySurveyPersonIndex = 14;
            FamilySurveyDateIndex = 15;
            FamilySurveyChronicleIndex = 16;
            FamilyCheckPersonIndex = 17;
            FamilyCheckDateIndex = 18;
            FamilyCheckOpinionIndex = 19;
            LandNameIndex = 20;
            CadastralNumberIndex = 21;
            ImageNumberIndex = 22;
            TableAreaIndex = 23;
            ActualAreaIndex = 24;
            AwareAreaIndex = 24;
            EastIndex = 25;
            NorthIndex = 28;
            WestIndex = 27;
            SourthIndex = 26;
            LandPurposeIndex = 29;
            LandLevelIndex = 30;
            LandTypeIndex = 31;
            IsFarmerLandIndex = 32;
            ReferPersonIndex = 33;
            CommentIndex = 34;
            LandSurveyPersonIndex = 35;
            LandSurveyDateIndex = 36;
            LandSurveyChronicleIndex = 37;
            LandCheckPersonIndex = 38;
            LandCheckDateIndex = 39;
            LandCheckOpinionIndex = 40;
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
            ArableTypeIndex = -1;
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
            ConstructTypeIndex = -1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化二轮台账信息
        /// </summary>
        /// <returns></returns>
        private bool InitalizeTableValue()
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
        private bool IsCheckTableValue()
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
        private bool InitalizeTableLandValue()
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

        #endregion
    }

    public class LandImportDefineCollection //(修改前): YltEntityCollection<LandImportDefine>
    {
    }
}
