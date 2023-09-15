using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 撰写表头信息
    /// </summary>
    public partial class ExportContractorSurveyExcel
    {
        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            string titleName = "农村土地承包经营权调查信息表";
            switch (TableType)
            {
                case 2:
                    titleName = "农村土地承包经营权确权登记公示表";
                    break;

                case 3:
                    titleName = "农村土地承包经营权确权登记签字表";
                    break;

                case 4:
                    titleName = "农村土地承包经营确权登记公示确认表(村组公示)";
                    break;

                case 5:
                    titleName = "农村土地承包经营权单户确认表";
                    break;
            }
            //titleName = InitalizeTitleName(titleName);
            titleName = UnitName + titleName;
            if (!string.IsNullOrEmpty(titleName))
            {
                SetRange("A1", PublicityConfirmDefine.GetColumnValue((TableType != 3 && TableType != 5) ? contractLandOutputSurveyDefine.ColumnCount : contractLandOutputSurveyDefine.ColumnCount + 1) + "1", 32.25, 18, true, titleName);
            }
            InitalizeRangeValue("A3", "A5", "承包方编号");
            SetRange("A3", "A5", "承包方编号");
            InitalizeRangeValue("B3", "B5", "承包方名称");
            SetRange("B3", "B5", "承包方名称");
            SetRange("E2", PublicityConfirmDefine.GetColumnValue((TableType != 3 && TableType != 5) ? contractLandOutputSurveyDefine.ColumnCount : contractLandOutputSurveyDefine.ColumnCount + 1) + "2", 21.75, 11, false, 3, 2, "日期:" + GetDate() + "               ");
            SetRange("A2", "D2", "单位:  " + ExcelName);
            columnIndex = 2;
            columnIndex = WriteContractorTitle(columnIndex);//撰写承包方表头信息
            columnIndex = WriterCensusTitle(columnIndex);//撰写户籍表头信息
            columnIndex = WrireSecondContractorTitle(columnIndex);//撰写二轮承包方表头信息
            columnIndex = WrireContractorSurveyTitle(columnIndex);//撰写承包方调查信息
            columnIndex = WriterAgricultureLandTitle(columnIndex);//撰写承包地块信息
            columnIndex = WriteAgricultureLandExpandTitle(columnIndex);//撰写承包地块扩展信息
            columnIndex = WriteSecondLandTitle(columnIndex);//撰写二轮承包地块信息
            columnIndex = WriteLandSurvyeTitle(columnIndex);//撰写地块调查表头
            if (TableType == 3 || TableType == 5 || TableType==4)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "签字人(按印)", true);
            }
        }

        /// <summary>
        /// 撰写承包方表头信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteContractorTitle(int columnIndex)
        {
            if (contractLandOutputSurveyDefine.ContractorTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方类型", true);
            }
            if (contractLandOutputSurveyDefine.NumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭成员数(个)", true);
            }
            if (contractLandOutputSurveyDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
            }
            if (contractLandOutputSurveyDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "性别", true);
            }
            if (contractLandOutputSurveyDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "年龄", true);
            }
            if (contractLandOutputSurveyDefine.NumberCartTypeValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证件类型", 15, true);
            }
            if (contractLandOutputSurveyDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证件号码", 20, true);
            }
            if (contractLandOutputSurveyDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
            }
            if (contractLandOutputSurveyDefine.AgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出生日期", true);
            }
            if (contractLandOutputSurveyDefine.NationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "民族", true);
            }
            if (contractLandOutputSurveyDefine.AccountNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户口性质", true);
            }
            if (contractLandOutputSurveyDefine.IsSharedLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否共有人", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            if (contractLandOutputSurveyDefine.FamilyOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "共有人修改意见", true);
            }
            if (contractLandOutputSurveyDefine.CencueCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户籍备注", true);
            }
            if (columnIndex > 2)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(3) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "家庭成员情况（含户主）", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写户籍信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriterCensusTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.IsSourceContractorValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否是原承包户", true);
            }
            if (contractLandOutputSurveyDefine.ContractorNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "现承包人数", true);
            }
            if (contractLandOutputSurveyDefine.LaborNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "总劳力数", true);
            }
            //if (contractLandOutputSurveyDefine.CencueCommentValue)
            //{
            //    columnIndex++;
            //    SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户籍备注", true);
            //}
            if (contractLandOutputSurveyDefine.FarmerNatureValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "农户性质", true);
            }
            if (contractLandOutputSurveyDefine.SourceMoveValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "从何处迁入", true);
            }
            if (contractLandOutputSurveyDefine.MoveTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入时间", true);
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入前土地类型", true);
            }
            if (contractLandOutputSurveyDefine.MoveFormerlyLandAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "迁入前土地面积", true);
            }
            if (contractLandOutputSurveyDefine.IsNinetyNineSharePersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否为99年共有人", true);
            }

            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "户籍籍贯信息", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写二轮承包方表头信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WrireSecondContractorTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.SecondNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "户主姓名", true);//PersonCount
            }
            if (contractLandOutputSurveyDefine.SecondNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭成员数（个）", true);//PersonCount
            }
            if (contractLandOutputSurveyDefine.SecondNumberNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberGenderValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "性别", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "年龄", true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "身份证号码", 20, true);
            }
            if (contractLandOutputSurveyDefine.SecondNumberRelatioinValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
            }
            if (contractLandOutputSurveyDefine.SecondNationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "民族", true);
            }
            if (contractLandOutputSurveyDefine.SecondAgeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出生日期", true);
            }
            if (contractLandOutputSurveyDefine.FirstContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "一轮承包人数", true);
            }
            if (contractLandOutputSurveyDefine.FirstContractAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "一轮承包面积", true);
            }
            if (contractLandOutputSurveyDefine.SecondContractorPersonNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮承包人数", true);
            }
            if (contractLandOutputSurveyDefine.SecondExtensionPackAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮延包面积", true);
            }
            if (contractLandOutputSurveyDefine.FoodCropAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "粮食种植面积", true);
            }
            if (contractLandOutputSurveyDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮土地延包姓名", true);
            }
            if (contractLandOutputSurveyDefine.ExPackageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "延包土地份数", true);
            }
            if (contractLandOutputSurveyDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "已死亡人员", true);
            }
            if (contractLandOutputSurveyDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "出嫁后未退承包地人员", true);
            }
            if (contractLandOutputSurveyDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "农转非后未退承包地人员", true);
            }
            if (contractLandOutputSurveyDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "婚进在婚出地未退承包地人员", true);
            }
            if (contractLandOutputSurveyDefine.SecondFamilyCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            //if (contractLandOutputSurveyDefine.IsContainTableValue)
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, contractLandOutputSurveyDefine.ExPackageNameValue ? "二轮土地延包人口和现有家庭人口变动情况" : "二轮承包家庭成员情况（含户主）", true);
            }
            startIndex = 0;
            return columnIndex;
        }

        /// <summary>
        /// 撰写承包方调查信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WrireContractorSurveyTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实际分配人数(人)", true);
            }
            if (contractLandOutputSurveyDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方地址", true);
            }
            if (contractLandOutputSurveyDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "邮政编码", true);
            }
            if (contractLandOutputSurveyDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "电话号码", true);
            }
            if (contractLandOutputSurveyDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包合同编号", true);
            }
            if (contractLandOutputSurveyDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "经营权证编号", true);
            }
            if (contractLandOutputSurveyDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包起始日期", true);
            }
            if (contractLandOutputSurveyDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包结束日期", true);
            }
            if (contractLandOutputSurveyDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "取得承包方式", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查员", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查日期", true);
            }
            if (contractLandOutputSurveyDefine.FamilySurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查记事", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核人", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核日期", true);
            }
            if (contractLandOutputSurveyDefine.FamilyCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核意见", true);
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "承包方调查信息", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写承包地块信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriterAgricultureLandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.LandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块名称", true);
            }
            if (contractLandOutputSurveyDefine.CadastralNumberValue)
            {
                columnIndex++;
                SetRangeWidth(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块编码", 23.25, true);
            }
            if (contractLandOutputSurveyDefine.ImageNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "图幅编号", true);
            }
            if (contractLandOutputSurveyDefine.TableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮合同面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "二轮合同总面积", true);
            }
            if (contractLandOutputSurveyDefine.ActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalActualAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "实测总面积", true);
            }
            if (contractLandOutputSurveyDefine.ContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "延包面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalContractDelayAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "延包总面积", true);
            }
            if (contractLandOutputSurveyDefine.AwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "确权面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalAwareAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "确权总面积", true);
            }
            if (contractLandOutputSurveyDefine.MotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "机动地面积", true);
            }
            if (contractLandOutputSurveyDefine.TotalMotorizeAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "机动地总面积", true);
            }
            if (contractLandOutputSurveyDefine.LandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "东", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "南", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "西", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "北", true);
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex - 3) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, "四至", true);
            }
            if (contractLandOutputSurveyDefine.LandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地用途", true);
            }
            if (contractLandOutputSurveyDefine.LandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "等级", true);
            }
            if (contractLandOutputSurveyDefine.LandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地利用类型", true);
            }
            if (contractLandOutputSurveyDefine.IsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否基本农田", true);
            }
            if (contractLandOutputSurveyDefine.ReferPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "指界人", true);
            }
            if (contractLandOutputSurveyDefine.ArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块类别", true);
            }
            if (contractLandOutputSurveyDefine.ConstructModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "承包方式", true);
            }
            if (contractLandOutputSurveyDefine.PlotNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "畦数", true);
            }
            if (contractLandOutputSurveyDefine.PlatTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "种植类型", true);
            }
            if (contractLandOutputSurveyDefine.ManagementTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "经营方式", true);
            }
            if (contractLandOutputSurveyDefine.LandPlantValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "耕保类型", true);
            }
            if (contractLandOutputSurveyDefine.SourceNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "原户主姓名（曾经耕种）", true);
            }
            if (contractLandOutputSurveyDefine.LandLocationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "宗地座落方位描述", true);
            }
            if (contractLandOutputSurveyDefine.ConcordValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "合同编号", true);
            }
            if (contractLandOutputSurveyDefine.RegeditBookValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "证书编号", true);
            }
            if (contractLandOutputSurveyDefine.CommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            if (contractLandOutputSurveyDefine.OpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块修改意见", true);
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "农村土地承包经营权承包地块详细信息", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写承包地块扩展信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteAgricultureLandExpandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.IsTransterValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否流转", true);
            }
            if (contractLandOutputSurveyDefine.TransterModeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转方式", true);
            }
            if (contractLandOutputSurveyDefine.TransterTermValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转期限", true);
            }
            if (contractLandOutputSurveyDefine.TransterAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "流转面积", true);
            }
            if (contractLandOutputSurveyDefine.IsTransterValue || contractLandOutputSurveyDefine.TransterModeValue || contractLandOutputSurveyDefine.TransterTermValue)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "承包地流转情况", true);
            }
            startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.UseSituationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "利用情况", true); ;//利用情况
            }
            if (contractLandOutputSurveyDefine.YieldValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "产量情况", true);//产量情况
            }
            if (contractLandOutputSurveyDefine.OutputValueValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "产值情况", true);//产值情况
            }
            if (contractLandOutputSurveyDefine.IncomeSituationValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "收益情况", true);//收益情况
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "承包地收益情况", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 撰写二轮台帐地块信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteSecondLandTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.SecondLandNumberValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块编码", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandNameValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块名称", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地类", true);
            }
            if (contractLandOutputSurveyDefine.SecondArableTypeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地类型", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandLevelValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "地块等级", true);
            }
            if (contractLandOutputSurveyDefine.SecondTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "台账面积", true);
            }
            if (contractLandOutputSurveyDefine.SecondTotalTableAreaValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "台账总面积", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandNeighborValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "东", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "南", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "西", true);
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "北", true);
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex - 3) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, "四至", true);
            }
            if (contractLandOutputSurveyDefine.SecondIsFarmerLandValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "是否基本农田", true);
            }
            if (contractLandOutputSurveyDefine.SecondLandPurposeValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "土地用途", true);
            }
            if (contractLandOutputSurveyDefine.SecondCommentValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "备注", true);
            }
            //if (contractLandOutputSurveyDefine.IsContainTablelandValue)
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "二轮农村土地承包情况", true);
            }
            return columnIndex;
        }

        /// <summary>
        /// 书写调查信息
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private int WriteLandSurvyeTitle(int columnIndex)
        {
            int startIndex = columnIndex + 1;
            if (contractLandOutputSurveyDefine.LandSurveyPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查员", true);
            }
            if (contractLandOutputSurveyDefine.LandSurveyDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查日期", true);
            }
            if (contractLandOutputSurveyDefine.LandSurveyChronicleValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "调查记事", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckPersonValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核人", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckDateValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核日期", true);
            }
            if (contractLandOutputSurveyDefine.LandCheckOpinionValue)
            {
                columnIndex++;
                SetRange(PublicityConfirmDefine.GetColumnValue(columnIndex) + 4, PublicityConfirmDefine.GetColumnValue(columnIndex) + 5, "审核意见", true);
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(PublicityConfirmDefine.GetColumnValue(startIndex) + 3, PublicityConfirmDefine.GetColumnValue(columnIndex) + 3, "承包地块调查情况", true);
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