using YuLinTu.Library.Entity;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    public partial class ExportLandVerifyExcelTable
    {
        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            columnIndex = 2;
            columnIndex = WriteContractorTitle(columnIndex);//撰写承包方表头信息
            columnIndex = WriterAgricultureLandTitle(columnIndex);//撰写承包地块信息
            columnIndex = WrireContractorSurveyTitle(columnIndex);//撰写承包方调查信息
            columnIndex = WriterCensusTitle(columnIndex);//撰写户籍表头信息
            columnIndex = WrireSecondContractorTitle(columnIndex);//撰写二轮承包方表头信息
            columnIndex = WriteAgricultureLandExpandTitle(columnIndex);//撰写承包地块扩展信息
            columnIndex = WriteSecondLandTitle(columnIndex);//撰写二轮承包地块信息
            columnIndex = WriteLandSurvyeTitle(columnIndex);//撰写地块调查表头
            string title = GetRangeToValue("A1", "A1").ToString();
            if (!string.IsNullOrEmpty(title))
            {
                SetRange("A1", PublicityConfirmDefine.GetColumnValue(columnIndex+1) + 1, 32.25, 18, true, title);
            }
            SetRange("A2", "D2", "单位:  " + ExcelName);
            SetRange("E2", PublicityConfirmDefine.GetColumnValue(columnIndex+1) + 2, 21.75, 11, false, 3, 2, "日期:" + GetDate() + "               ");
            InitalizeRangeValue("A3", "A5", "承包方编号");
            SetRange("A3", "A5", "承包方编号");
            InitalizeRangeValue("B3", "B5", "承包方名称");
            SetRange("B3", "B5", "承包方名称");
            InitalizeRangeValue("B3", "B5", "承包方名称");
            SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex+1) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex+1) + 5, "签字确认");
            //columnIndex = WriterCensusTitle(columnIndex);//撰写户籍表头信息
            //columnIndex = WrireSecondContractorTitle(columnIndex);//撰写二轮承包方表头信息

            //columnIndex = WriteAgricultureLandExpandTitle(columnIndex);//撰写承包地块扩展信息
            //columnIndex = WriteSecondLandTitle(columnIndex);//撰写二轮承包地块信息
            //columnIndex = WriteLandSurvyeTitle(columnIndex);//撰写地块调查表头
            //if (TableType == 3 || TableType == 5)
            //{
            //    columnIndex++;
            //    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "签字人(按印)", true);
            //}
        }

        /// <summary>
        /// 撰写承包方表头信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteContractorTitle(int columnIndex)
        {
            if (landVerifyDefineDefine.ContractorTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方类型", true);
            }
            if (landVerifyDefineDefine.NumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭成员数(个)", true);
            }
            if (landVerifyDefineDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
            }
            if (landVerifyDefineDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "性别", true);
            }
            if (landVerifyDefineDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "年龄", true);
            }
            if (landVerifyDefineDefine.NumberCartTypeValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证件类型", 15, true);
            }
            if (landVerifyDefineDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证件号码", 20, true);
            }
            if (landVerifyDefineDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
            }
            if (landVerifyDefineDefine.AgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出生日期", true);
            }
            if (landVerifyDefineDefine.NationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "民族", true);
            }
            if (landVerifyDefineDefine.AccountNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户口性质", true);
            }
            if (landVerifyDefineDefine.IsSharedLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否共有人", true);
            }
            if (landVerifyDefineDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方地址", true);
            }
            if (landVerifyDefineDefine.FamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            if (landVerifyDefineDefine.FamilyOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "共有人修改意见", true);
            }
            
            if (landVerifyDefineDefine.CencueCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户籍备注", true);
            }
           
            return columnIndex;
        }

        /// <summary>
        /// 撰写户籍信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WriterCensusTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.IsSourceContractorValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否是原承包户", true);
            }
            if (landVerifyDefineDefine.ContractorNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "现承包人数", true);
            }
            if (landVerifyDefineDefine.LaborNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "总劳力数", true);
            }
            //if (landVerifyDefineDefine.CencueCommentValue)
            //{
            //    columnIndex++;
            //    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户籍备注", true);
            //}
            if (landVerifyDefineDefine.FarmerNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "农户性质", true);
            }
            if (landVerifyDefineDefine.SourceMoveValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "从何处迁入", true);
            }
            if (landVerifyDefineDefine.MoveTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入时间", true);
            }
            if (landVerifyDefineDefine.MoveFormerlyLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入前土地类型", true);
            }
            if (landVerifyDefineDefine.MoveFormerlyLandAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入前土地面积", true);
            }
            if (landVerifyDefineDefine.IsNinetyNineSharePersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否为99年共有人", true);
            }

            return columnIndex;
        }

        /// <summary>
        /// 撰写二轮承包方表头信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WrireSecondContractorTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.SecondNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户主姓名", true);//PersonCount
            }
            if (landVerifyDefineDefine.SecondNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭成员数（个）", true);//PersonCount
            }
            if (landVerifyDefineDefine.SecondNumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
            }
            if (landVerifyDefineDefine.SecondNumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "性别", true);
            }
            if (landVerifyDefineDefine.SecondNumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "年龄", true);
            }
            if (landVerifyDefineDefine.SecondNumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "身份证号码", 20, true);
            }
            if (landVerifyDefineDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
            }
            if (landVerifyDefineDefine.SecondNationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "民族", true);
            }
            if (landVerifyDefineDefine.SecondAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出生日期", true);
            }
            if (landVerifyDefineDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "一轮承包人数", true);
            }
            if (landVerifyDefineDefine.FirstContractAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "一轮承包面积", true);
            }
            if (landVerifyDefineDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮承包人数", true);
            }
            if (landVerifyDefineDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮延包面积", true);
            }
            if (landVerifyDefineDefine.FoodCropAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "粮食种植面积", true);
            }
            if (landVerifyDefineDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮土地延包姓名", true);
            }

            if (landVerifyDefineDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "已死亡人员", true);
            }
            if (landVerifyDefineDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出嫁后未退承包地人员", true);
            }
            if (landVerifyDefineDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "农转非后未退承包地人员", true);
            }
            if (landVerifyDefineDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "婚进在婚出地未退承包地人员", true);
            }
            if (landVerifyDefineDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            //if (landVerifyDefineDefine.IsContainTableValue)
            
            startIndex = 0;
            return columnIndex;
        }

        /// <summary>
        /// 撰写承包方调查信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WrireContractorSurveyTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实际分配人数(人)", true);
            }
           
            if (landVerifyDefineDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "邮政编码", true);
            }
            if (landVerifyDefineDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "电话号码", true);
            }
            if (landVerifyDefineDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包合同编号", true);
            }
            if (landVerifyDefineDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "经营权证编号", true);
            }
            if (landVerifyDefineDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包起始日期", true);
            }
            if (landVerifyDefineDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包结束日期", true);
            }
            if (landVerifyDefineDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "取得承包方式", true);
            }
            if (landVerifyDefineDefine.FamilySurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查员", true);
            }
            if (landVerifyDefineDefine.FamilySurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查日期", true);
            }
            if (landVerifyDefineDefine.FamilySurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查记事", true);
            }
            if (landVerifyDefineDefine.FamilyCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核人", true);
            }
            if (landVerifyDefineDefine.FamilyCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核日期", true);
            }
            if (landVerifyDefineDefine.FamilyCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核意见", true);
            }

            return columnIndex;
        }

        /// <summary>
        /// 撰写承包地块信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WriterAgricultureLandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;

            
            if (landVerifyDefineDefine.PlotNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "宗地数量", true);
            }
            if (landVerifyDefineDefine.ExPackageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "宗地数量", true);
            }
            if (landVerifyDefineDefine.LandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块名称", true);
            }
            if (landVerifyDefineDefine.CadastralNumberValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块编码", 23.25, true);
            }
            if (landVerifyDefineDefine.ImageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "图幅编号", true);
            }
            if (landVerifyDefineDefine.TableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮合同面积", true);
            }
            if (landVerifyDefineDefine.TotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮合同总面积", true);
            }
            if (landVerifyDefineDefine.ActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测面积", true);
            }

            if (landVerifyDefineDefine.ContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "延包面积", true);
            }

            if (landVerifyDefineDefine.AwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "确权面积", true);
            }
            if (landVerifyDefineDefine.TotalAwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "确权总面积", true);
            }
            if (landVerifyDefineDefine.TotalActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测总面积", true);
            }

            if (landVerifyDefineDefine.TotalContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "延包总面积", true);
            }
            if (landVerifyDefineDefine.MotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "机动地面积", true);
            }
            if (landVerifyDefineDefine.TotalMotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "机动地总面积", true);
            }
            if (landVerifyDefineDefine.LandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "东", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "南", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "西", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "北", true);
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex - 3) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "四至", true);
            }
            if (landVerifyDefineDefine.LandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地用途", true);
            }
            if (landVerifyDefineDefine.LandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "等级", true);
            }
            if (landVerifyDefineDefine.LandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地利用类型", true);
            }
            if (landVerifyDefineDefine.IsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否基本农田", true);
            }
            if (landVerifyDefineDefine.ReferPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "指界人", true);
            }
            if (landVerifyDefineDefine.ArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块类别", true);
            }
            if (landVerifyDefineDefine.ConstructModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方式", true);
            }
            if (landVerifyDefineDefine.PlatTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "种植类型", true);
            }
            if (landVerifyDefineDefine.ManagementTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "经营方式", true);
            }
            if (landVerifyDefineDefine.LandPlantValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "耕保类型", true);
            }
            if (landVerifyDefineDefine.SourceNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "原户主姓名（曾经耕种）", true);
            }
            if (landVerifyDefineDefine.LandLocationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "宗地座落方位描述", true);
            }

            if (landVerifyDefineDefine.RegeditBookValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证书编号", true);
            }
            if (landVerifyDefineDefine.ConcordValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "合同编号", true);
            }
            if (landVerifyDefineDefine.CommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            if (landVerifyDefineDefine.OpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块修改意见", true);
            }
            return columnIndex;
        }

        /// <summary>
        ///撰写承包地块扩展信息
        /// </summary>
        /// /<param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WriteAgricultureLandExpandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.IsTransterValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否流转", true);
            }
            if (landVerifyDefineDefine.TransterModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转方式", true);
            }
            if (landVerifyDefineDefine.TransterTermValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转期限", true);
            }
            if (landVerifyDefineDefine.TransterAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转面积", true);
            }
            if (landVerifyDefineDefine.IsTransterValue || landVerifyDefineDefine.TransterModeValue || landVerifyDefineDefine.TransterTermValue)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "承包地流转情况", true);
            }
            startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.UseSituationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "利用情况", true); ;//利用情况
            }
            if (landVerifyDefineDefine.YieldValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "产量情况", true);//产量情况
            }
            if (landVerifyDefineDefine.OutputValueValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "产值情况", true);//产值情况
            }
            if (landVerifyDefineDefine.IncomeSituationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "收益情况", true);//收益情况
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写二轮台帐地块信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WriteSecondLandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.SecondLandNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块编码", true);
            }
            if (landVerifyDefineDefine.SecondLandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块名称", true);
            }
            if (landVerifyDefineDefine.SecondLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地类", true);
            }
            if (landVerifyDefineDefine.SecondArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地类型", true);
            }
            if (landVerifyDefineDefine.SecondLandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块等级", true);
            }
            if (landVerifyDefineDefine.SecondTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "台账面积", true);
            }
            if (landVerifyDefineDefine.SecondTotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "台账总面积", true);
            }
            if (landVerifyDefineDefine.SecondLandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "东", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "南", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "西", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "北", true);
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex - 3) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "四至", true);
            }
            if (landVerifyDefineDefine.SecondIsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否基本农田", true);
            }
            if (landVerifyDefineDefine.SecondLandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地用途", true);
            }
            if (landVerifyDefineDefine.SecondCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            //if (landVerifyDefineDefine.IsContainTablelandValue)
          
            return columnIndex;
        }

        /// <summary>
        /// 书写调查信息
        /// </summary>
        /// <param name = "columnIndex" ></ param >
        /// < returns ></ returns >
        private int WriteLandSurvyeTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (landVerifyDefineDefine.LandSurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查员", true);
            }
            if (landVerifyDefineDefine.LandSurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查日期", true);
            }
            if (landVerifyDefineDefine.LandSurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查记事", true);
            }
            if (landVerifyDefineDefine.LandCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核人", true);
            }
            if (landVerifyDefineDefine.LandCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核日期", true);
            }
            if (landVerifyDefineDefine.LandCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核意见", true);
            }

            return columnIndex;
        }

        /// <summary>
        /// 初始化标题名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeTitleName(string titleName)
        {
            if (AgricultureSetting.UseTemplateTitle)
            {
                return InitalizeTitle() + titleName;
            }
            if (AgricultureSetting.UseTableSourceTitle)
            {
                return GetTitle() + "区、县（市）" + titleName;
            }
            return AgricultureSetting.InitalizeTitle(currentZone) + titleName;
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        private string GetTitle()
        {
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                //Zone county = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                //Zone city = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));

                AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
                if (currentZone.Level == eZoneLevel.Group)
                {
                    Zone group = alb.GetParent(currentZone);
                    Zone village = alb.GetParent(group);
                    Zone town = alb.GetParent(village);
                    Zone county = alb.GetParent(town);
                    Zone city = alb.GetParent(county);

                    if (city != null && county != null)
                    {
                        string zoneName = county.FullName.Replace(city.FullName, "");
                        return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                    }
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        private string InitalizeTitle()
        {
            AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
            Zone group = new Zone();
            Zone village = new Zone();
            Zone town = new Zone();
            Zone county = new Zone();
            Zone city = new Zone();

            if (currentZone.Level == eZoneLevel.Group)
            {
                group = currentZone;
                village = alb.GetParent(group);
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                village = currentZone;
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Town)
            {
                town = currentZone;
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone != null && currentZone.FullCode.Length > Zone.ZONE_COUNTY_LENGTH)
            {
                //Zone county = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                return county != null ? county.Name : "";
            }
            return "";
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        /// <returns></returns>
        private string GetUnitName()
        {
            AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
            Zone group = new Zone();
            Zone village = new Zone();
            Zone town = new Zone();
            Zone county = new Zone();
            Zone city = new Zone();

            if (currentZone.Level == eZoneLevel.Group)
            {
                group = currentZone;
                village = alb.GetParent(group);
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                village = currentZone;
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone.Level == eZoneLevel.Town)
            {
                town = currentZone;
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                //Zone city = DB.Zone.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                if (city != null)
                {
                    return currentZone.FullName.Replace(city.FullName, "");
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        /// <param name="code"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        //private string GetZoneName(string code, eZoneLevel level)
        //{
        //    Zone tempZone = DB.Zone.Get(code);
        //    if (tempZone.Level != level)
        //    {
        //        return GetZoneName(tempZone.UpLevelCode, level);
        //    }
        //    return tempZone.Name;
        //}

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        private string GetName(string str, string[] parms)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string temp = str.Substring(str.Length - 1);
                foreach (string item in parms)
                {
                    if (temp == item)
                    {
                        return str.Substring(0, str.Length - 1);
                    }
                }
            }
            return str;
        }
    }
}