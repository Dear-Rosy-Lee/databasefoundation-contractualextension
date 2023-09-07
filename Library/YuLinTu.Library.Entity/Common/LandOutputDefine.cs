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
    public class LandOutputDefine : NameableObject  //(修改前)YltEntityIDName
    {
        #region Propertys

        /// <summary>
        /// 户主名索引
        /// </summary>
        public bool NameValue { get; set; }

        /// <summary>
        /// 家庭成员个数索引
        /// </summary>
        public bool NumberValue { get; set; }

        /// <summary>
        /// 家庭成员姓名索引
        /// </summary>
        public bool NumberNameValue { get; set; }

        /// <summary>
        /// 承包方类型索引
        /// </summary>
        public bool ContractorTypeValue { get; set; }

        /// <summary>
        /// 家庭成员证件类型索引
        /// </summary>
        public bool NumberCartTypeValue { get; set; }

        /// <summary>
        /// 家庭成员身份证号索引
        /// </summary>
        public bool NumberIcnValue { get; set; }

        /// <summary>
        /// 家庭成员性别索引
        /// </summary>
        public bool NumberGenderValue { get; set; }

        /// <summary>
        /// 家庭成员年龄索引
        /// </summary>
        public bool NumberAgeValue { get; set; }

        /// <summary>
        /// 家庭成员关系索引
        /// </summary>
        public bool NumberRelatioinValue { get; set; }

        /// <summary>
        /// 二轮户主名索引
        /// </summary>
        public bool SecondNameValue { get; set; }

        /// <summary>
        /// 二轮家庭成员个数索引
        /// </summary>
        public bool SecondNumberValue { get; set; }

        /// <summary>
        /// 二轮家庭成员姓名索引
        /// </summary>
        public bool SecondNumberNameValue { get; set; }

        /// <summary>
        /// 二轮家庭成员身份证号索引
        /// </summary>
        public bool SecondNumberIcnValue { get; set; }

        /// <summary>
        /// 二轮家庭成员性别索引
        /// </summary>
        public bool SecondNumberGenderValue { get; set; }

        /// <summary>
        /// 二轮家庭成员年龄索引
        /// </summary>
        public bool SecondNumberAgeValue { get; set; }

        /// <summary>
        /// 二轮家庭成员关系索引
        /// </summary>
        public bool SecondNumberRelatioinValue { get; set; }

        /// <summary>
        /// 二轮承包方备注索引
        /// </summary>
        public bool SecondFamilyCommentValue { get; set; }

        /// <summary>
        /// 二轮延包姓名
        /// </summary>
        public bool ExPackageNameValue { get; set; }

        /// <summary>
        /// 延包土地份数
        /// </summary>
        public bool ExPackageNumberValue { get; set; }

        /// <summary>
        /// 已死亡人员
        /// </summary>
        public bool IsDeadedValue { get; set; }

        /// <summary>
        /// 出嫁后未退承包地人员
        /// </summary>
        public bool LocalMarriedRetreatLandValue { get; set; }

        /// <summary>
        /// 农转非后未退承包地人员
        /// </summary>
        public bool PeasantsRetreatLandValue { get; set; }

        /// <summary>
        /// 婚进但在非出地未退承包地人员
        /// </summary>
        public bool ForeignMarriedRetreatLandValue { get; set; }

        /// <summary>
        /// 承包地共有人
        /// </summary>
        public bool SharePersonValue { get; set; }

        /// <summary>
        /// 是否享有承包地
        /// </summary>
        public bool IsSharedLandValue { get; set; }

        /// <summary>
        /// 合同编号索引
        /// </summary>
        public bool ConcordValue { get; set; }

        /// <summary>
        /// 权证编号索引
        /// </summary>
        public bool RegeditBookValue { get; set; }

        /// <summary>
        /// 地籍号索引
        /// </summary>
        public bool CadastralNumberValue { get; set; }

        /// <summary>
        /// 小地名索引
        /// </summary>
        public bool LandNameValue { get; set; }

        /// <summary>
        /// 图幅编号
        /// </summary>
        public bool ImageNumberValue { get; set; }

        /// <summary>
        /// 二轮小地名索引
        /// </summary>
        public bool SecondLandNameValue { get; set; }

        /// <summary>
        /// 畦数索引
        /// </summary>
        public bool PlotNumberValue { get; set; }

        /// <summary>
        /// 实测面积索引
        /// </summary>
        public bool ActualAreaValue { get; set; }

        /// <summary>
        /// 实测总面积索引
        /// </summary>
        public bool TotalActualAreaValue { get; set; }

        /// <summary>
        /// 确权面积索引
        /// </summary>
        public bool AwareAreaValue { get; set; }

        /// <summary>
        /// 确权总面积索引
        /// </summary>
        public bool TotalAwareAreaValue { get; set; }

        /// <summary>
        /// 机动地面积索引
        /// </summary>
        public bool MotorizeAreaValue { get; set; }

        /// <summary>
        /// 机动地总面积索引
        /// </summary>
        public bool TotalMotorizeAreaValue { get; set; }

        /// <summary>
        /// 二轮台账面积索引
        /// </summary>
        public bool TableAreaValue { get; set; }

        /// <summary>
        /// 二轮台账面积索引
        /// </summary>
        public bool  SecondTableAreaValue { get; set; }

        /// <summary>
        /// 二轮台账总面积索引
        /// </summary>
        public bool TotalTableAreaValue { get; set; }

        /// <summary>
        /// 二轮台账总面积索引
        /// </summary>
        public bool SecondTotalTableAreaValue { get; set; }

        /// <summary>
        /// 地类索引
        /// </summary>
        public bool LandTypeValue { get; set; }

        /// <summary>
        /// 二轮地类索引
        /// </summary>
        public bool SecondLandTypeValue { get; set; }

        /// <summary>
        /// 经营方式索引
        /// </summary>
        public bool ManagementTypeValue { get; set; }

        /// <summary>
        /// 四至索引
        /// </summary>
        public bool LandNeighborValue { get; set; }

        /// <summary>
        /// 二轮四至索引
        /// </summary>
        public bool SecondLandNeighborValue { get; set; }

        /// <summary>
        /// 原户主名索引
        /// </summary>
        public bool SourceNameValue { get; set; }

        /// <summary>
        /// 宗地座落索引
        /// </summary>
        public bool LandLocationValue { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public bool ConstructModeValue { get; set; }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public bool IsFarmerLandValue { get; set; }

        /// <summary>
        /// 土地用途
        /// </summary>
        public bool LandPurposeValue { get; set; }

        /// <summary>
        /// 是否流转索引
        /// </summary>
        public bool IsTransterValue { get; set; }

        /// <summary>
        /// 流转方式索引
        /// </summary>
        public bool TransterModeValue { get; set; }

        /// <summary>
        /// 流转期限索引
        /// </summary>
        public bool TransterTermValue { get; set; }

        /// <summary>
        /// 流转面积索引
        /// </summary>
        public bool TransterAreaValue { get; set; }

        /// <summary>
        /// 种植类型索引
        /// </summary>
        public bool PlatTypeValue { get; set; }

        /// <summary>
        /// 承包方地址索引
        /// </summary>
        public bool ContractorAddressValue { get; set; }

        /// <summary>
        /// 邮政编码索引
        /// </summary>
        public bool PostNumberValue { get; set; }

        /// <summary>
        /// 电话号码索引
        /// </summary>
        public bool TelephoneValue { get; set; }

        /// <summary>
        /// 备注索引
        /// </summary>
        public bool CommentValue { get; set; }

        /// <summary>
        /// 二轮备注索引
        /// </summary>
        public bool SecondCommentValue { get; set; }

        /// <summary>
        /// 承包方备注索引
        /// </summary>
        public bool FamilyCommentValue { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public bool LandLevelValue { get; set; }

        /// <summary>
        /// 耕保等级
        /// </summary>
        public bool LandPlantValue { get; set; }

        /// <summary>
        /// 耕地类型
        /// </summary>
        public bool ArableTypeValue { get; set; }

        /// <summary>
        /// 实际分配人数
        /// </summary>
        public bool AllocationPersonValue { get; set; }

        /// <summary>
        /// 利用情况
        /// </summary>
        public bool UseSituationValue { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public bool YieldValue { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public bool OutputValueValue { get; set; }

        /// <summary>
        /// 收益情况
        /// </summary>
        public bool IncomeSituationValue { get; set; }

        /// <summary>
        /// 总劳力数
        /// </summary>
        public bool LaborNumberValue { get; set; }

        /// <summary>
        /// 是否是原承包户
        /// </summary>
        public bool IsSourceContractorValue { get; set; }

        /// <summary>
        /// 现承包人数
        /// </summary>
        public bool ContractorNumberValue { get; set; }

        /// <summary>
        /// 农户性质
        /// </summary>
        public bool FarmerNatureValue { get; set; }

        /// <summary>
        /// 迁入前土地类型
        /// </summary>
        public bool MoveFormerlyLandTypeValue { get; set; }

        /// <summary>
        /// 迁入前土地面积
        /// </summary>
        public bool MoveFormerlyLandAreaValue { get; set; }

        /// <summary>
        /// 一轮承包人数
        /// </summary>
        public bool FirstContractorPersonNumberValue { get; set; }

        /// <summary>
        /// 一轮承包面积
        /// </summary>
        public bool FirstContractAreaValue { get; set; }

        /// <summary>
        /// 二轮承包人数
        /// </summary>
        public bool SecondContractorPersonNumberValue { get; set; }

        /// <summary>
        /// 二轮延包面积
        /// </summary>
        public bool SecondExtensionPackAreaValue { get; set; }

        /// <summary>
        /// 粮食种植面积
        /// </summary>
        public bool FoodCropAreaValue { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public bool AgeValue { get; set; }

        /// <summary>
        /// 户口性质
        /// </summary>
        public bool AccountNatureValue { get; set; }

        /// <summary>
        /// 从何处迁入
        /// </summary>
        public bool SourceMoveValue { get; set; }

        /// <summary>
        /// 迁入时间
        /// </summary>
        public bool MoveTimeValue { get; set; }

        /// <summary>
        /// 是否为99年共有人
        /// </summary>
        public bool IsNinetyNineSharePersonValue { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public bool NationValue { get; set; }

        /// <summary>
        /// 二轮年龄
        /// </summary>
        public bool SecondAgeValue { get; set; }

        /// <summary>
        /// 二轮民族
        /// </summary>
        public bool SecondNationValue { get; set; }

        /// <summary>
        /// 二轮地块编码
        /// </summary>
        public bool SecondLandNumberValue { get; set; }

        /// <summary>
        /// 二轮地块类型
        /// </summary>
        public bool SecondArableTypeValue { get; set; }

        /// <summary>
        /// 二轮是否基本农田
        /// </summary>
        public bool SecondIsFarmerLandValue { get; set; }

        /// <summary>
        /// 二轮土地用途
        /// </summary>
        public bool SecondLandPurposeValue { get; set; }

        /// <summary>
        /// 二轮土地等级
        /// </summary>
        public bool SecondLandLevelValue { get; set; }

        /// <summary>
        /// 户籍备注
        /// </summary>
        public bool CencueCommentValue { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public bool SecondConcordNumberValue { get; set; }

        /// <summary>
        /// 证书编号
        /// </summary>
        public bool SecondWarrantNumberValue { get; set; }

        /// <summary>
        /// 合同开始时间
        /// </summary>
        public bool StartTimeValue { get; set; }

        /// <summary>
        /// 合同结束时间
        /// </summary>
        public bool EndTimeValue { get; set; }

        /// <summary>
        /// 承包方式
        /// </summary>
        public bool ConstructTypeValue { get; set; }

        /// <summary>
        /// 调查员
        /// </summary>
        public bool FamilySurveyPersonValue { get; set; }

        /// <summary>
        /// 调查日期
        /// </summary>
        public bool FamilySurveyDateValue { get; set; }

        /// <summary>
        /// 调查记事
        /// </summary>
        public bool FamilySurveyChronicleValue { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public bool FamilyCheckPersonValue { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public bool FamilyCheckDateValue { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public bool FamilyCheckOpinionValue { get; set; }

        /// <summary>
        /// 调查员
        /// </summary>
        public bool LandSurveyPersonValue { get; set; }

        /// <summary>
        /// 调查日期
        /// </summary>
        public bool LandSurveyDateValue { get; set; }

        /// <summary>
        /// 调查记事
        /// </summary>
        public bool LandSurveyChronicleValue { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public bool LandCheckPersonValue { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public bool LandCheckDateValue { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public bool LandCheckOpinionValue { get; set; }

        /// <summary>
        /// 指界人
        /// </summary>
        public bool ReferPersonValue { get; set; }

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
        public int ColumnCount { get; set; }

        #endregion

        #region Ctor

        public LandOutputDefine()
        {
            NameValue = true;
            SecondNameValue = false;
            ContractorTypeValue = true;
            NumberValue = true;
            SecondNumberValue = false;
            NumberNameValue = true;
            SecondNumberNameValue = false;
            NumberCartTypeValue = true;
            NumberIcnValue = true;
            SecondNumberIcnValue = false;
            NumberGenderValue = true;
            SecondNumberGenderValue = false;
            NumberAgeValue = false;
            SecondNumberAgeValue = false;
            NumberRelatioinValue = true;
            SecondNumberRelatioinValue = false;
            ExPackageNameValue = false;
            ExPackageNumberValue = false;
            IsDeadedValue = false;
            LocalMarriedRetreatLandValue = false;
            PeasantsRetreatLandValue = false;
            ForeignMarriedRetreatLandValue = false;
            SharePersonValue = false;
            IsSharedLandValue = true;
            ConcordValue = false;
            RegeditBookValue = false;
            CadastralNumberValue = true;
            LandNameValue = true;
            SecondLandNameValue = false;
            PlotNumberValue = false;
            ActualAreaValue = true;
            TotalActualAreaValue = false;
            AwareAreaValue = false;
            TotalAwareAreaValue = false;
            MotorizeAreaValue = false;
            TotalMotorizeAreaValue = false;
            TableAreaValue = true;
            SecondTableAreaValue = false;
            TotalTableAreaValue = false;
            SecondTotalTableAreaValue = false;
            LandTypeValue = true;
            SecondLandTypeValue = false;
            ManagementTypeValue = false;
            LandNeighborValue = true;
            SourceNameValue = false;
            LandLocationValue = false;
            CommentValue = true;
            SecondCommentValue = false;
            IsFarmerLandValue = true;
            ConstructModeValue = false;
            IsTransterValue = false;
            TransterModeValue = false;
            TransterTermValue = false;
            TransterAreaValue = false;
            PlatTypeValue = false;
            TelephoneValue = true;
            FamilyCommentValue = true;
            SecondFamilyCommentValue = false;
            LandLevelValue = true;
            LandPlantValue = false;
            ArableTypeValue = false;
            AllocationPersonValue = false;
            UseSituationValue = false;
            YieldValue = false;
            OutputValueValue = false;
            IncomeSituationValue = false;
            LaborNumberValue = false;
            IsSourceContractorValue = false;
            ContractorNumberValue = false;
            FarmerNatureValue = false;
            MoveFormerlyLandTypeValue = false;
            MoveFormerlyLandAreaValue = false;
            FirstContractorPersonNumberValue = false;
            FirstContractAreaValue = false;
            SecondContractorPersonNumberValue = false;
            SecondExtensionPackAreaValue = false;
            FoodCropAreaValue = false;
            AgeValue = false;
            AccountNatureValue = false;
            SourceMoveValue = false;
            MoveTimeValue = false;
            IsNinetyNineSharePersonValue = false;
            CencueCommentValue = false;
            FamilySurveyPersonValue = true;
            FamilySurveyDateValue = true;
            FamilySurveyChronicleValue = true;
            FamilyCheckPersonValue = true;
            FamilyCheckDateValue = true;
            FamilyCheckOpinionValue = true;
            LandSurveyPersonValue = true;
            LandSurveyDateValue = true;
            LandSurveyChronicleValue = true;
            LandCheckPersonValue = true;
            LandCheckDateValue = true;
            LandCheckOpinionValue = true;
            ReferPersonValue = true;
            ContractorAddressValue = true;
            PostNumberValue = true;
            LandPurposeValue = true;
            ImageNumberValue = true;
            SecondConcordNumberValue = false;
            SecondWarrantNumberValue = false;
            StartTimeValue = false;
            EndTimeValue = false;
            ConstructTypeValue = false;
            ColumnCount = 41;
        }

        #endregion

        #region Methods

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

        #endregion
    }

    public class LandOutputDefineCollection //(修改前): YltEntityCollection<LandOutputDefine>
    {
    }
}
